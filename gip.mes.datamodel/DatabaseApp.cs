using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data;
using System.ComponentModel;
using System.Threading;

namespace gip.mes.datamodel
{
    public static class EntityObjectExtensionApp
    {
        public static TEntityApp FromAppContext<TEntityApp>(this EntityObject entityIPlus, DatabaseApp dbApp) where TEntityApp : EntityObject
        {
            if (entityIPlus == null || dbApp == null)
                return default(TEntityApp);
            EntityKey key = new EntityKey(dbApp.DefaultContainerName + "." + entityIPlus.EntityKey.EntitySetName, entityIPlus.EntityKey.EntityKeyValues);
            //key.EntityContainerName = entityApp.DefaultContainerName;
            object obj = null;

            using (ACMonitor.Lock(dbApp.QueryLock_1X000))
            {
                if (!dbApp.TryGetObjectByKey(key, out obj))
                    return default(TEntityApp);
            }
            return (TEntityApp)obj;
        }
    }

    [ACClassInfo(Const.PackName_VarioSystem, "en{'Database application'}de{'Datenbank Anwendung'}", Global.ACKinds.TACDBAManager, Global.ACStorableTypes.NotStorable, false, false)]
    public partial class DatabaseApp : iPlusMESV4_Entities, IACEntityObjectContext, INotifyPropertyChanged
    {
        #region c'tors
        public DatabaseApp()
            : base(ConnectionString)
        {
            _ObjectContextHelper = new ACObjectContextHelper(this);
        }

        public DatabaseApp(string connectionString)
            : base(connectionString)
        {
            _ObjectContextHelper = new ACObjectContextHelper(this);
        }

        public DatabaseApp(Database contextIPlus)
            : base(ConnectionString)
        {
            _ObjectContextHelper = new ACObjectContextHelper(this);
            _ContextIPlus = contextIPlus;
        }

        public DatabaseApp(string connectionString, Database contextIPlus)
            : base(connectionString)
        {
            _ObjectContextHelper = new ACObjectContextHelper(this);
            _ContextIPlus = contextIPlus;
        }

        public DatabaseApp(bool createSeparateConnection)
            : this(new EntityConnection(ConnectionString))
        {
        }

        public DatabaseApp(bool createSeparateConnection, Database contextIPlus)
            : this(new EntityConnection(ConnectionString))
        {
        }

        public DatabaseApp(EntityConnection connection)
            : base(connection)
        {
            _SeparateConnection = connection;
            _ObjectContextHelper = new ACObjectContextHelper(this);
        }

        public DatabaseApp(EntityConnection connection, Database contextIPlus)
            : base(connection)
        {
            _SeparateConnection = connection;
            _ObjectContextHelper = new ACObjectContextHelper(this);
            _ContextIPlus = contextIPlus;
        }

        static DatabaseApp()
        {
            if (!Global.ContextMenuCategoryList.Where(c => (short)c.Value == (short)Global.ContextMenuCategory.ProdPlanLog).Any())
                Global.ContextMenuCategoryList.Add(new ACValueItem("en{'Production, Plannung & Logistics'}de{'Production, Plannung & Logistics'}", (short)Global.ContextMenuCategory.ProdPlanLog, null, null, 250));
        }

        protected override void Dispose(bool disposing)
        {
            if (_ObjectContextHelper != null)
                _ObjectContextHelper.Dispose();
            _ObjectContextHelper = null;
            _ContextIPlus = null;
            base.Dispose(disposing);
            if (SeparateConnection != null)
                SeparateConnection.Dispose();
            _SeparateConnection = null;
        }

        /// <summary>
        /// Dient zum erstmaligen initialiseren der Datenban beim Startup
        /// damit csdl und msal flies geladen werden.
        /// Dadurch wird das erstmalige Öffnen eines BSO's und der erste Speichervorgang beschleunigt.
        /// </summary>
        public static void InitializeDBOnStartup()
        {
            if (_AppDBOnStartupInitialized)
                return;
            _AppDBOnStartupInitialized = true;
            // Nicht über die Queue, sondern es muss der Statvorgang abgewartet werden!
            //RootDbOpQueue.ACClassTaskQueue.Add(() =>
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    dbApp.ACAssembly.FirstOrDefault();
                }
            }
            //);
        }

        #endregion

        #region Properties

        #region Private
        private ACObjectContextHelper _ObjectContextHelper;
        private Database _ContextIPlus;
        private static bool _AppDBOnStartupInitialized = false;
        #endregion

        #region Public Static
        public static string ConnectionString
        {
            get
            {
                if (CommandLineHelper.ConfigCurrentDir != null && CommandLineHelper.ConfigCurrentDir.ConnectionStrings != null)
                {
                    try
                    {
                        ConnectionStringSettings setting = CommandLineHelper.ConfigCurrentDir.ConnectionStrings.ConnectionStrings["iPlusMESV4_Entities"];
                        return setting.ConnectionString;
                    }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        if (Database.Root != null && Database.Root.Messages != null && Database.Root.InitState == ACInitState.Initialized)
                            Database.Root.Messages.LogException("DatabaseApp", "ConnectionString", msg);
                    }
                }
                return "name=iPlusMESV4_Entities";
            }
        }

        /// <summary>
        /// Method for changing Connection-String to generate own connectionpool
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string ModifiedConnectionString(string appName)
        {
            var connString = ConnectionString.Replace("iPlus_dbApp", appName);
            return connString;
        }
        #endregion

        #region Public
        [ACPropertyInfo(9999)]
        public Database ContextIPlus
        {
            get
            {
                return _ContextIPlus == null ? Database.GlobalDatabase : _ContextIPlus;
            }
        }

        public bool IsSeparateIPlusContext
        {
            get
            {
                if (_ContextIPlus == null)
                    return false;
                else if (_ContextIPlus == Database.GlobalDatabase)
                    return false;
                return true;
            }
        }

        EntityConnection _SeparateConnection;
        public EntityConnection SeparateConnection
        {
            get
            {
                return _SeparateConnection;
            }
        }


        /// <summary>
        /// THREAD-SAFE. Uses QueryLock_1X000
        /// </summary>
        /// <returns></returns>
        [ACPropertyInfo(9999)]
        public bool IsChanged
        {
            get
            {
                if (_ObjectContextHelper == null)
                    return false;
                return _ObjectContextHelper.IsChanged;
            }
        }

        public MergeOption RecommendedMergeOption
        {
            get
            {
                return IsChanged ? MergeOption.AppendOnly : MergeOption.OverwriteChanges;
            }
        }

        public event ACChangesEventHandler ACChangesExecuted;

        #endregion

        #region IACUrl Member
        /// <summary>
        /// Returns the parent object
        /// </summary>
        /// <value>Reference to the parent object</value>
        public IACObject ParentACObject
        {
            get
            {
                return gip.core.datamodel.Database.Root;
            }
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public string ACCaption
        {
            get
            {
                return ACIdentifier;
            }
        }

        /// <summary>Unique Identifier in a Parent-/Child-Relationship.</summary>
        /// <value>The Unique Identifier as string</value>
        [ACPropertyInfo(9999)]
        public string ACIdentifier
        {
            get
            {
                return GetType().Name;
            }
        }

        /// <summary>
        /// A "content list" contains references to the most important data that this instance primarily works with. It is primarily used to control the interaction between users, visual objects, and the data model in a generic way. For example, drag-and-drop or context menu operations. A "content list" can also be null.
        /// </summary>
        /// <value> A nullable list ob IACObjects.</value>
        public IEnumerable<IACObject> ACContentList
        {
            get
            {
                return this.ReflectGetACContentList();
            }
        }

        /// <summary>
        /// Metadata (iPlus-Type) of this instance. ATTENTION: IACType are EF-Objects. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>  iPlus-Type (EF-Object from ACClass*-Tables)</value>
        public IACType ACType
        {
            get
            {
                return this.ReflectACType();
            }
        }
        #endregion

        #endregion

        #region Methods

        #region public
        /// <summary>
        /// Saves all changes in this DatabaseApp-Context as well as in the iPlus-Context
        /// If ContextIPlus is not seperate  (Property IsSeperateIPlusContext == false / ContextIPlus == Database.GlobalDatabase) then SaveChanges will not be invoked for the global database.
        /// If you wan't that, then you have to pass an new iPlus-Context-Instance to the constructor of the DatabaseApp-Context!
        /// UNSAFE. Use QueryLock_1X000 outside for Custom-Database-Context as well as for the seperate iPlus-Context
        /// </summary>
        /// <returns></returns>
        [ACMethodInfo("", "", 9999)]
        public MsgWithDetails ACSaveChanges(bool autoSaveContextIPlus = true, SaveOptions saveOptions = SaveOptions.AcceptAllChangesAfterSave, bool validationOff = false, bool writeUpdateInfo = true)
        {
            MsgWithDetails result = _ObjectContextHelper.ACSaveChanges(autoSaveContextIPlus, saveOptions, validationOff, writeUpdateInfo);
            if (result == null)
            {
                if (ACChangesExecuted != null)
                    ACChangesExecuted.Invoke(this, new ACChangesEventArgs(ACChangesEventArgs.ACChangesType.ACSaveChanges, true));
            }
            else
            {
                if (ACChangesExecuted != null)
                    ACChangesExecuted.Invoke(this, new ACChangesEventArgs(ACChangesEventArgs.ACChangesType.ACSaveChanges, false));
            }
            return result;
        }

        /// <summary>
        /// Invokes ACSaveChanges. If a transaction error occurs ACSaveChanges is called again.
        /// If parameter retries ist not set, then ACObjectContextHelper.C_NumberOfRetriesOnTransError is used to limit the Retry-Loop.
        /// </summary>
        [ACMethodInfo("", "", 9999)]
        public MsgWithDetails ACSaveChangesWithRetry(ushort? retries = null, bool autoSaveContextIPlus = true, SaveOptions saveOptions = SaveOptions.AcceptAllChangesAfterSave, bool validationOff = false, bool writeUpdateInfo = true)
        {
            MsgWithDetails result = _ObjectContextHelper.ACSaveChangesWithRetry(retries, autoSaveContextIPlus, saveOptions, validationOff, writeUpdateInfo);
            if (result == null)
            {
                if (ACChangesExecuted != null)
                    ACChangesExecuted.Invoke(this, new ACChangesEventArgs(ACChangesEventArgs.ACChangesType.ACSaveChanges, true));
            }
            else
            {
                if (ACChangesExecuted != null)
                    ACChangesExecuted.Invoke(this, new ACChangesEventArgs(ACChangesEventArgs.ACChangesType.ACSaveChanges, false));
            }
            return result;
        }

        /// <summary>
        /// Undoes all changes in the DatabaseApp-Context as well as in the iPlus-Context
        /// If ContextIPlus is not seperate  (Property IsSeperateIPlusContext == false / ContextIPlus == Database.GlobalDatabase) then Undo will not be invoked for the global database.
        /// If you wan't that, then you have to pass an new iPlus-Context-Instance to the constructor of the DatabaseApp-Context!
        /// UNSAFE. Use QueryLock_1X000 outside for DatabaseApp-Context as well as for the seperate iPlus-Context
        /// </summary>
        /// <returns></returns>
        [ACMethodInfo("", "", 9999)]
        public MsgWithDetails ACUndoChanges(bool autoUndoContextIPlus = true)
        {
            MsgWithDetails result = _ObjectContextHelper.ACUndoChanges(autoUndoContextIPlus);
            if (result == null)
            {
                if (ACChangesExecuted != null)
                    ACChangesExecuted.Invoke(this, new ACChangesEventArgs(ACChangesEventArgs.ACChangesType.ACUndoChanges, true));
            }
            else
            {
                if (ACChangesExecuted != null)
                    ACChangesExecuted.Invoke(this, new ACChangesEventArgs(ACChangesEventArgs.ACChangesType.ACUndoChanges, false));
            }
            return result;
        }

        /// <summary>
        /// THREAD-SAFE. Uses QueryLock_1X000
        /// </summary>
        /// <returns></returns>
        public bool HasModifiedObjectStateEntries()
        {
            return _ObjectContextHelper.HasModifiedObjectStateEntries();
        }

        /// <summary>
        /// THREAD-SAFE. Uses QueryLock_1X000
        /// </summary>
        /// <returns></returns>
        public bool HasAddedEntities<T>() where T : class
        {
            return _ObjectContextHelper.HasAddedEntities<T>();
        }

        /// <summary>
        /// THREAD-SAFE. Uses QueryLock_1X000
        /// </summary>
        /// <returns></returns>
        public IList<T> GetAddedEntities<T>() where T : class
        {
            return _ObjectContextHelper.GetAddedEntities<T>();
        }

        /// <summary>
        /// UNSAFE. Use QueryLock_1X000 outside
        /// </summary>
        /// <returns></returns>
        public IList<Msg> CheckChangedEntities()
        {
            return _ObjectContextHelper.CheckChangedEntities();
        }

        /// <summary>
        /// Refreshes the EntityObject if not in modified state. Else it leaves it untouched.
        /// </summary>
        /// <param name="entityObject"></param>
        /// <param name="refreshMode"></param>
        public void AutoRefresh(EntityObject entityObject, RefreshMode refreshMode = RefreshMode.StoreWins)
        {
            _ObjectContextHelper.AutoRefresh(entityObject, refreshMode);
        }

        /// <summary>
        /// Refreshes all EntityObjects in the EntityCollection if not in modified state. Else it leaves it untouched.
        /// Attention: This method will only refresh the entities with entity keys that are tracked by the ObjectContext. 
        /// If changes are made in background on the database you shoud use the method AutoLoad, to retrieve new Entries from the Database!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityCollection"></param>
        /// <param name="refreshMode"></param>
        public void AutoRefresh<T>(EntityCollection<T> entityCollection, RefreshMode refreshMode = RefreshMode.StoreWins) where T : class
        {
            _ObjectContextHelper.AutoRefresh<T>(entityCollection, refreshMode);
        }

        /// <summary>
        /// Queries the Database an refreshes the collection if not in modified state. MergeOption.OverwriteChanges
        /// Els if in modified state, then colletion is only refreshed with MergeOption.AppendOnly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityCollection"></param>
        public void AutoLoad<T>(EntityCollection<T> entityCollection) where T : class
        {
            _ObjectContextHelper.AutoLoad<T>(entityCollection);
        }

        public void ParseException(MsgWithDetails msg, Exception e)
        {
            _ObjectContextHelper.ParseException(msg, e);
        }

        #region IACUrl Member
        /// <summary>
        /// The ACUrlCommand is a universal method that can be used to query the existence of an instance via a string (ACUrl) to:
        /// 1. get references to components,
        /// 2. query property values,
        /// 3. execute method calls,
        /// 4. start and stop Components,
        /// 5. and send messages to other components.
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>Result if a property was accessed or a method was invoked. Void-Methods returns null.</returns>
        public object ACUrlCommand(string acUrl, params Object[] acParameter)
        {
            if (_ObjectContextHelper == null)
                return null;
            return _ObjectContextHelper.ACUrlCommand(acUrl, acParameter);
        }

        /// <summary>
        /// This method is called before ACUrlCommand if a method-command was encoded in the ACUrl
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>true if ACUrlCommand can be invoked</returns>
        public bool IsEnabledACUrlCommand(string acUrl, params Object[] acParameter)
        {
            return true;
        }

        /// <summary>
        /// Method that returns a source and path for WPF-Bindings by passing a ACUrl.
        /// </summary>
        /// <param name="acUrl">ACUrl of the Component, Property or Method</param>
        /// <param name="acTypeInfo">Reference to the iPlus-Type (ACClass)</param>
        /// <param name="source">The Source for WPF-Databinding</param>
        /// <param name="path">Relative path from the returned source for WPF-Databinding</param>
        /// <param name="rightControlMode">Information about access rights for the requested object</param>
        /// <returns><c>true</c> if binding could resolved for the passed ACUrl<c>false</c> otherwise</returns>
        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            if (_ObjectContextHelper == null)
                return false;
            return _ObjectContextHelper.ACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        [ACMethodInfo("", "", 9999)]
        public string GetACUrlComponent(IACObject rootACObject = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a ACUrl relatively to the passed object.
        /// If the passed object is null then the absolute path is returned
        /// </summary>
        /// <param name="rootACObject">Object for creating a realtive path to it</param>
        /// <returns>ACUrl as string</returns>
        [ACMethodInfo("", "", 9999)]
        public string GetACUrl(IACObject rootACObject = null)
        {
            return ACIdentifier;
        }

        public void FullDetach(EntityObject obj)
        {
            Detach(obj);
            // General Problem of ObjectContext-MAnager
            // When a object should be detached, then the object which have a relational relationship will not be deatched
            // The Information about the relation are stored in the internal Member _danglingForeignKeys of the ObjectContextManager
            // This entries will never be deleted - so the memory increases for long term open contexts
            // See under: http://referencesource.microsoft.com/#System.Data.Entity/System/Data/Objects/ObjectStateManager.cs
            // The following code is a first attempt to empty this cache:
            /*if (this.ObjectStateManager == null)
                return;
            ObjectStateEntry entry = null;
            if (!this.ObjectStateManager.TryGetObjectStateEntry(obj.EntityKey, out entry))
                return;
            try
            {
                this.Detach(obj);
                Type tOSM = this.ObjectStateManager.GetType();
                //RemoveForeignKeyFromIndex(EntityKey foreignKey)
                //MethodInfo mi = tOSM.GetMethod("FixupKey", BindingFlags.Instance | BindingFlags.NonPublic);
                //mi.Invoke(this.ObjectStateManager, new object[] { entry });
                MethodInfo mi = tOSM.GetMethod("RemoveForeignKeyFromIndex", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(this.ObjectStateManager, new object[] { obj.EntityKey });
            }
            ctch
            {
            }*/
        }
        #endregion

        #endregion

        #region Critical Section
        public void EnterCS()
        {
            _ObjectContextHelper.EnterCS();
        }

        public void EnterCS(bool DeactivateEntityCheck)
        {
            _ObjectContextHelper.EnterCS(DeactivateEntityCheck);
        }

        public void LeaveCS()
        {
            _ObjectContextHelper.LeaveCS();
        }

        private ACMonitorObject _11000_QueryLock_ = new ACMonitorObject(11000);
        public ACMonitorObject QueryLock_1X000
        {
            get
            {
                return _11000_QueryLock_;
            }
        }

        #endregion

        #endregion

        #region Lists

        public IEnumerable<ACValueItem> InOrderStatesList
        {
            get
            {
                return gip.mes.datamodel.MDInOrderState.InOrderStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> FacilityInventoryPosStatesList
        {
            get
            {
                return gip.mes.datamodel.MDFacilityInventoryPosState.FacilityInventoryPosStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> ZeroStockStatesList
        {
            get
            {
                return gip.mes.datamodel.MDZeroStockState.ZeroStockStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> ReservationModesList
        {
            get
            {
                return gip.mes.datamodel.MDReservationMode.ReservationModesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> ReleaseStatesList
        {
            get
            {
                return gip.mes.datamodel.MDReleaseState.ReleaseStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> OutOrderStatesList
        {
            get
            {
                return gip.mes.datamodel.MDOutOrderState.OutOrderStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> InvoiceStatesList
        {
            get
            {
                return gip.mes.datamodel.MDInvoiceState.InvoiceStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> OutOrderPosStatesList
        {
            get
            {
                return gip.mes.datamodel.MDOutOrderPosState.OutOrderPosStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> OutOrderPlanStatesList
        {
            get
            {
                return gip.mes.datamodel.MDOutOrderPlanState.OutOrderPlanStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> OutOfferStatesList
        {
            get
            {
                return gip.mes.datamodel.MDOutOfferState.OutOfferStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> MovementReasonsList
        {
            get
            {
                return gip.mes.datamodel.MDMovementReason.MovementReasonsList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> InRequestStatesList
        {
            get
            {
                return gip.mes.datamodel.MDInRequestState.InRequestStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> InOrderPosStatesList
        {
            get
            {
                return gip.mes.datamodel.MDInOrderPosState.InOrderPosStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> FacilityTypesList
        {
            get
            {
                return gip.mes.datamodel.MDFacilityType.FacilityTypesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> FacilityManagementTypesList
        {
            get
            {
                return gip.mes.datamodel.MDFacilityManagementType.FacilityManagementTypesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> BookingNotAvailableModesList
        {
            get
            {
                return gip.mes.datamodel.MDBookingNotAvailableMode.BookingNotAvailableModesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> BalancingModesList
        {
            get
            {
                return gip.mes.datamodel.MDBalancingMode.BalancingModesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> LabOrderPosStatesList
        {
            get
            {
                return gip.mes.datamodel.MDLabOrderPosState.LabOrderPosStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> LabOrderStatesList
        {
            get
            {
                return gip.mes.datamodel.MDLabOrderState.LabOrderStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> LabTagsList
        {
            get
            {
                return gip.mes.datamodel.MDLabTag.LabTagsList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> MaterialGroupTypesList
        {
            get
            {
                return gip.mes.datamodel.MDMaterialGroup.MaterialGroupTypesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> VisitorVoucherStatesList
        {
            get
            {
                return gip.mes.datamodel.MDVisitorVoucherState.VisitorVoucherStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> VisitorCardStatesList
        {
            get
            {
                return gip.mes.datamodel.MDVisitorCardState.VisitorCardStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> DelivPosLoadStateList
        {
            get
            {
                return gip.mes.datamodel.MDDelivPosLoadState.DelivPosLoadStateList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> DelivPosStatesList
        {
            get
            {
                return gip.mes.datamodel.MDDelivPosState.DelivPosStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> DelivNoteStatesList
        {
            get
            {
                return gip.mes.datamodel.MDDelivNoteState.DelivNoteStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> DelivTypesList
        {
            get
            {
                return gip.mes.datamodel.MDDelivType.DelivTypesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> TransportModesList
        {
            get
            {
                return gip.mes.datamodel.MDTransportMode.TransportModesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> TourplanPosStatesList
        {
            get
            {
                return gip.mes.datamodel.MDTourplanPosState.TourplanPosStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> TourplanStatesList
        {
            get
            {
                return gip.mes.datamodel.MDTourplanState.TourplanStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> ProdOrderPartslistPosStatesList
        {
            get
            {
                return gip.mes.datamodel.MDProdOrderPartslistPosState.ProdOrderPartslistPosStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> ProcessErrorActionsList
        {
            get
            {
                return gip.mes.datamodel.MDProcessErrorAction.ProcessErrorActionList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> DemandOrderStatesList
        {
            get
            {
                return gip.mes.datamodel.MDDemandOrderState.DemandOrderStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> ToleranceStateList
        {
            get
            {
                return gip.mes.datamodel.MDToleranceState.ToleranceStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> MaintOrderStateList
        {
            get
            {
                return gip.mes.datamodel.MDMaintOrderState.MaintOrderStateList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> MaintModesList
        {
            get
            {
                return gip.mes.datamodel.MDMaintMode.MaintModesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> FacilityVehicleTypesList
        {
            get
            {
                return gip.mes.datamodel.MDFacilityVehicleType.FacilityVehicleTypesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> OrderTypesList
        {
            get
            {
                return GlobalApp.OrderTypesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> InvoiceTypesList
        {
            get
            {
                return GlobalApp.InvoiceTypesList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> FacilityBookingTypeList
        {
            get
            {
                return GlobalApp.FacilityBookingTypeList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> SIDimensionList
        {
            get
            {
                return GlobalApp.SIDimensionList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> PetroleumGroupList
        {
            get
            {
                return GlobalApp.PetroleumGroupList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> DeliveryNoteTypeList
        {
            get
            {
                return GlobalApp.DeliveryNoteTypeList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> PickingTypeList
        {
            get
            {
                return GlobalApp.PickingTypeList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> PickingStateList
        {
            get
            {
                return GlobalApp.PickingStateList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> TimePeriodsList
        {
            get
            {
                return GlobalApp.TimePeriodsList;
            }
        }

        /// <summary>
        /// Liste aller Adressen, die eine Factory sind und dem eigenen Unternehmen angehören
        /// </summary>
        [ACPropertyInfo(9999, "")]
        public IEnumerable<CompanyAddress> FactoryCompanyAddressList
        {
            get
            {
                using (ACMonitor.Lock(QueryLock_1X000)) 
                {
                    return this.CompanyAddress.Where(c => c.IsFactory && c.Company.IsOwnCompany);
                }
            }
        }

        [ACPropertyInfo(9999)]
        public IQueryable<MDUnit> MDUnitList
        {
            get
            {
                using (ACMonitor.Lock(QueryLock_1X000))
                {
                    return from c in this.MDUnit where c.IsQuantityUnit orderby c.SortIndex select c;
                }
            }
        }

        [ACPropertyInfo(9999)]
        public IQueryable<Facility> FacilityLocation
        {
            get
            {
                using (ACMonitor.Lock(QueryLock_1X000))
                {
                    return Facility.Where(c => c.ParentFacilityID != null);
                }
            }
        }

        [ACPropertyInfo(9999, "", "en{'Tenant/Contractual partner'}de{'Mandant/Vertragspartner'}")]
        public IQueryable<Company> CPartnerCompanyList
        {
            get
            {
                using (ACMonitor.Lock(QueryLock_1X000))
                {
                    return Company.Where(c => c.IsTenant == true);
                }
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> BatchPlanModeList
        {
            get
            {
                return GlobalApp.BatchPlanModeList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> BatchPlanStateList
        {
            get
            {
                return GlobalApp.BatchPlanStateList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> ReservationStateList
        {
            get
            {
                return GlobalApp.ReservationStateList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> LabOrderTypeList
        {
            get
            {
                return GlobalApp.LabOrderTypeList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> TrackAndTracingSearchModelList
        {
            get
            {
                return GlobalApp.TrackingAndTracingSearchModelSearchModelList;
            }
        }

        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> MaterialProcessStateList
        {
            get
            {
                return GlobalApp.MaterialProcessStateList;
            }
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;


        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region precompiled Queries
        public static readonly Func<DatabaseApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates, IQueryable<MDProdOrderPartslistPosState>> s_cQry_GetMDProdOrderPosState =
        CompiledQuery.Compile<DatabaseApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates, IQueryable<MDProdOrderPartslistPosState>>(
            (ctx, state) => from c in ctx.MDProdOrderPartslistPosState
                            where c.MDProdOrderPartslistPosStateIndex == (short)state
                            select c
        );

        public static readonly Func<DatabaseApp, MDDelivPosState.DelivPosStates, IQueryable<MDDelivPosState>> s_cQry_GetMDDelivPosState =
        CompiledQuery.Compile<DatabaseApp, MDDelivPosState.DelivPosStates, IQueryable<MDDelivPosState>>(
            (ctx, state) => from c in ctx.MDDelivPosState
                            where c.MDDelivPosStateIndex == (short)state
                            select c
        );

        public static readonly Func<DatabaseApp, string, IQueryable<MDPickingType>> s_cQry_GetMDPickingType =
        CompiledQuery.Compile<DatabaseApp, string, IQueryable<MDPickingType>>(
            (ctx, key) => from c in ctx.MDPickingType
                          where c.MDKey == key
                          select c
        );

        public static readonly Func<DatabaseApp, MDDelivPosLoadState.DelivPosLoadStates, IQueryable<MDDelivPosLoadState>> s_cQry_GetMDDelivPosLoadState =
        CompiledQuery.Compile<DatabaseApp, MDDelivPosLoadState.DelivPosLoadStates, IQueryable<MDDelivPosLoadState>>(
            (ctx, state) => from c in ctx.MDDelivPosLoadState
                            where c.MDDelivPosLoadStateIndex == (short)state
                            select c
        );

        public static readonly Func<DatabaseApp, MDProdOrderState.ProdOrderStates, IQueryable<MDProdOrderState>> s_cQry_GetMDProdOrderState =
        CompiledQuery.Compile<DatabaseApp, MDProdOrderState.ProdOrderStates, IQueryable<MDProdOrderState>>(
            (ctx, state) => from c in ctx.MDProdOrderState
                            where c.MDProdOrderStateIndex == (short)state
                            select c
        );

        public static readonly Func<DatabaseApp, MDDelivNoteState.DelivNoteStates, IQueryable<MDDelivNoteState>> s_cQry_GetMDDelivNoteState =
        CompiledQuery.Compile<DatabaseApp, MDDelivNoteState.DelivNoteStates, IQueryable<MDDelivNoteState>>(
            (ctx, state) => from c in ctx.MDDelivNoteState
                            where c.MDDelivNoteStateIndex == (short)state
                            select c
        );

        public static readonly Func<DatabaseApp, MDBookingNotAvailableMode.BookingNotAvailableModes, IQueryable<MDBookingNotAvailableMode>> s_cQry_GetMDBookingNotAvailableMode =
        CompiledQuery.Compile<DatabaseApp, MDBookingNotAvailableMode.BookingNotAvailableModes, IQueryable<MDBookingNotAvailableMode>>(
            (ctx, mode) => from c in ctx.MDBookingNotAvailableMode
                           where c.MDBookingNotAvailableModeIndex == (short)mode
                           select c
        );

        public static readonly Func<DatabaseApp, MDBalancingMode.BalancingModes, IQueryable<MDBalancingMode>> s_cQry_GetMDBalancingMode =
        CompiledQuery.Compile<DatabaseApp, MDBalancingMode.BalancingModes, IQueryable<MDBalancingMode>>(
            (ctx, mode) => from c in ctx.MDBalancingMode
                           where c.MDBalancingModeIndex == (short)mode
                           select c
        );

        #endregion

        /// <summary>
        /// UNSAFE. Use QueryLock_1X000 outside
        /// </summary>
        /// <returns></returns>
        public void DetachAllEntitiesAndDispose(bool detach = false, bool dispose = true)
        {
            if (_ObjectContextHelper != null && detach)
                _ObjectContextHelper.DetachAllEntities();
            if (dispose)
                Dispose(true);
        }

    }
}
