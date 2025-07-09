// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using gip.core.datamodel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace gip.mes.datamodel
{
    public static class EntityObjectExtensionApp
    {
        public static TEntityApp FromAppContext<TEntityApp>(this VBEntityObject entityIPlus, DatabaseApp dbApp) where TEntityApp : VBEntityObject
        {
            if (entityIPlus == null || entityIPlus.EntityKey == null || dbApp == null)
                return default(TEntityApp);
            Type typeOfTargetContext = dbApp.GetType();
            string fullName = typeOfTargetContext.Namespace + "." + entityIPlus.EntityKey.EntitySetName;
            Type typeOfTargetEntity = typeOfTargetContext.Assembly.GetType(fullName);
            if (typeOfTargetEntity == null)
                throw new ArgumentException(String.Format("Type {0} not found in assembly {1}", fullName, typeOfTargetContext.Assembly.ToString()));

            EntityKey key = new EntityKey(dbApp.GetQualifiedEntitySetNameForEntityKey(entityIPlus.EntityKey.EntitySetName), entityIPlus.EntityKey.EntityKeyValues);
            object obj = null;
            using (ACMonitor.Lock(dbApp.QueryLock_1X000))
            {
                if (!dbApp.TryGetObjectByKey(key, out obj))
                    return default(TEntityApp);
            }
            return (TEntityApp)obj;
        }

        public static T AttachToContext<T>(VBEntityObject detachedObject, IACEntityObjectContext attachToContext) where T : VBEntityObject
        {
            DatabaseApp databaseApp = attachToContext as DatabaseApp;
            T entity = detachedObject as T;
            if (entity == null)
                return null;
            return entity.FromAppContext<T>(databaseApp);
        }
    }

    [ACClassInfo(Const.PackName_VarioSystem, "en{'Database application'}de{'Datenbank Anwendung'}", Global.ACKinds.TACDBAManager, Global.ACStorableTypes.NotStorable, false, false)]
    public partial class DatabaseApp : iPlusMESV5Context, IACEntityObjectContext, INotifyPropertyChanged
    {
        #region c'tors
        public DatabaseApp()
            : base(EntityObjectExtension.DbContextOptions<iPlusMESV5Context>(ConnectionString))
        {
            _ObjectContextHelper = new ACObjectContextHelper(this);
        }

        public DatabaseApp(string connectionString)
            : base(EntityObjectExtension.DbContextOptions<iPlusMESV5Context>(connectionString))
        {
            _ObjectContextHelper = new ACObjectContextHelper(this);
        }

        public DatabaseApp(Database contextIPlus)
            : base(EntityObjectExtension.DbContextOptions<iPlusMESV5Context>(ConnectionString))
        {
            _ObjectContextHelper = new ACObjectContextHelper(this);
            _ContextIPlus = contextIPlus;
        }

        public DatabaseApp(string connectionString, Database contextIPlus)
            : base(EntityObjectExtension.DbContextOptions<iPlusMESV5Context>(connectionString))
        {
            _ObjectContextHelper = new ACObjectContextHelper(this);
            _ContextIPlus = contextIPlus;
        }

        public DatabaseApp(bool createSeparateConnection)
            : this(new SqlConnection(ConnectionString))
        {
        }

        public DatabaseApp(bool createSeparateConnection, Database contextIPlus)
            : this(new SqlConnection(ConnectionString))
        {
        }

        public DatabaseApp(DbConnection connection)
            : base(EntityObjectExtension.DbContextOptions<iPlusMESV5Context>(connection))
        {
            _SeparateConnection = connection;
            _ObjectContextHelper = new ACObjectContextHelper(this);
        }

        public DatabaseApp(DbConnection connection, Database contextIPlus)
            : base(EntityObjectExtension.DbContextOptions<iPlusMESV5Context>(connection))
        {
            _SeparateConnection = connection;
            _ObjectContextHelper = new ACObjectContextHelper(this);
            _ContextIPlus = contextIPlus;
        }

        static DatabaseApp()
        {
            //if (!Global.ContextMenuCategoryList.Where(c => (short)c.Value == (short)Global.ContextMenuCategory.ProdPlanLog).Any())
            //    Global.ContextMenuCategoryList.Add(new ACValueItem("en{'Production, Plannung & Logistics'}de{'Production, Plannung & Logistics'}", (short)Global.ContextMenuCategory.ProdPlanLog, null, null, 250));
        }

        public override void Dispose()
        {
            if (_ObjectContextHelper != null)
                _ObjectContextHelper.Dispose();
            _ObjectContextHelper = null;
            _ContextIPlus = null;
            base.Dispose();
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
                        ConnectionStringSettings setting = CommandLineHelper.ConfigCurrentDir.ConnectionStrings.ConnectionStrings[C_DefaultContainerName];
                        return setting.ConnectionString;
                    }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        if (gip.core.datamodel.Database.Root != null && gip.core.datamodel.Database.Root.Messages != null && gip.core.datamodel.Database.Root.InitState == ACInitState.Initialized)
                            gip.core.datamodel.Database.Root.Messages.LogException("DatabaseApp", "ConnectionString", msg);
                    }
                }
                return ConfigurationManager.ConnectionStrings[C_DefaultContainerName].ConnectionString;
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
                return _ContextIPlus == null ? gip.core.datamodel.Database.GlobalDatabase : _ContextIPlus;
            }
        }

        public bool IsSeparateIPlusContext
        {
            get
            {
                if (_ContextIPlus == null)
                    return false;
                else if (_ContextIPlus == gip.core.datamodel.Database.GlobalDatabase)
                    return false;
                return true;
            }
        }

        public DbConnection Connection
        {
            get
            {
                return ContextIPlus.Connection;
            }
        }

        DbConnection _SeparateConnection;
        public DbConnection SeparateConnection
        {
            get
            {
                return _SeparateConnection;
            }
        }

        private string _UserName;
        public string UserName
        {
            get
            {
                if (!String.IsNullOrEmpty(_UserName))
                    return _UserName;
                if (Database.Root == null
                    || !gip.core.datamodel.Database.Root.Initialized
                    || gip.core.datamodel.Database.Root.Environment == null
                    || gip.core.datamodel.Database.Root.Environment.User == null)
                    return "Init";
                _UserName = gip.core.datamodel.Database.Root.Environment.User.Initials;
                return _UserName;
            }
            set
            {
                _UserName = value;
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

        public const string C_DefaultContainerName = "iPlusMESV5_Entities";
        /// <summary>
        /// Compatibility for legacy code that uses EntityKey from EF4
        /// used in EF4 to identify the context, now it is the namespace of the DbContext to be able to build a assemby qualified name to consturct an assembly qualified name for the EntityKey
        /// </summary>
        public string DefaultContainerName
        {
            get
            {
                return this._ObjectContextHelper.DefaultContainerName;
            }
        }

        public string DefaultContainerNameV4
        {
            get
            {
                return C_DefaultContainerName;
            }
        }

        public string GetQualifiedEntitySetNameForEntityKey(string entitySetName)
        {
            return this._ObjectContextHelper.GetQualifiedEntitySetNameForEntityKey(entitySetName);
        }

        public event ACChangesEventHandler ACChangesExecuted;

        public bool PreventOnContextACChangesExecuted { get; set; }

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
        /// UNSAFE. Use QueryLock_1X000 outside
        /// </summary>
        /// <returns></returns>
        public void DetachAllEntitiesAndDispose(bool detach = false, bool dispose = true)
        {
            if (_ObjectContextHelper != null && detach)
                _ObjectContextHelper.DetachAllEntities();
            if (dispose)
                Dispose();
        }

        public void Detach(object entity)
        {
            _ObjectContextHelper.Detach(entity);
        }

        public override EntityEntry Attach(object entity)
        {
            VBEntityObject vbobj = entity as VBEntityObject;
            vbobj.Context = this;
            return base.Attach(entity);
        }

        public override EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
        {
            VBEntityObject vbobj = entity as VBEntityObject;
            vbobj.Context = this;
            return base.Attach(entity);
        }

        public override void AttachRange(IEnumerable<object> entities)
        {
            foreach (var entity in entities)
            {
                VBEntityObject vbobj = entity as VBEntityObject;
                vbobj.Context = this;
            }
            base.AttachRange(entities);
        }

        public override void AttachRange(params object[] entities)
        {
            foreach (var entity in entities)
            {
                VBEntityObject vbobj = entity as VBEntityObject;
                vbobj.Context = this;
            }
            base.AttachRange(entities);
        }

        public void AcceptAllChanges()
        {
            ((IACEntityObjectContext)ContextIPlus).ChangeTracker.AcceptAllChanges();
        }

        public DbSet<TEntity> CreateObjectSet<TEntity>() where TEntity : class
        {
            return ((IACEntityObjectContext)ContextIPlus).CreateObjectSet<TEntity>();
        }

        public DbSet<TEntity> CreateObjectSet<TEntity>(string entitySetName) where TEntity : class
        {
            return ((IACEntityObjectContext)ContextIPlus).CreateObjectSet<TEntity>(entitySetName);
        }

        public object GetObjectByKey(EntityKey key)
        {
            return _ObjectContextHelper.GetObjectByKey(key);
        }

        public bool TryGetObjectByKey(EntityKey key, out object entity)
        {
            return _ObjectContextHelper.TryGetObjectByKey(key, out entity);
        }

        public void Refresh(RefreshMode refreshMode, object entity)
        {
            _ObjectContextHelper.Refresh(refreshMode, entity);
        }

        /// <summary>
        /// Saves all changes in this DatabaseApp-Context as well as in the iPlus-Context
        /// If ContextIPlus is not seperate  (Property IsSeperateIPlusContext == false / ContextIPlus == Database.GlobalDatabase) then SaveChanges will not be invoked for the global database.
        /// If you wan't that, then you have to pass an new iPlus-Context-Instance to the constructor of the DatabaseApp-Context!
        /// UNSAFE. Use QueryLock_1X000 outside for Custom-Database-Context as well as for the seperate iPlus-Context
        /// </summary>
        /// <returns></returns>
        [ACMethodInfo("", "", 9999)]
        public MsgWithDetails ACSaveChanges(bool autoSaveContextIPlus = true, bool acceptAllChangesOnSuccess = true, bool validationOff = false, bool writeUpdateInfo = true)
        {
            MsgWithDetails result = _ObjectContextHelper.ACSaveChanges(autoSaveContextIPlus, acceptAllChangesOnSuccess, validationOff, writeUpdateInfo);
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
        public MsgWithDetails ACSaveChangesWithRetry(ushort? retries = null, bool autoSaveContextIPlus = true, bool acceptAllChangesOnSuccess = true, bool validationOff = false, bool writeUpdateInfo = true)
        {
            MsgWithDetails result = _ObjectContextHelper.ACSaveChangesWithRetry(retries, autoSaveContextIPlus, acceptAllChangesOnSuccess, validationOff, writeUpdateInfo);
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
        /// returns Entities with EntityState.Added-State only
        /// </summary>
        /// <returns></returns>
        public IList<T> GetAddedEntities<T>(Func<T, bool> selector = null) where T : class
        {
            return _ObjectContextHelper.GetChangedEntities<T>(EntityState.Added, selector);
        }

        /// <summary>
        /// THREAD-SAFE. Uses QueryLock_1X000
        /// returns Entities with EntityState.Modified-State only
        /// </summary>
        /// <returns></returns>
        public IList<T> GetModifiedEntities<T>(Func<T, bool> selector = null) where T : class
        {
            return _ObjectContextHelper.GetChangedEntities<T>(EntityState.Modified, selector);
        }

        /// <summary>
        /// THREAD-SAFE. Uses QueryLock_1X000
        /// returns Entities with EntityState.Deleted-State only
        /// </summary>
        /// <returns></returns>
        public IList<T> GetDeletedEntities<T>(Func<T, bool> selector = null) where T : class
        {
            return _ObjectContextHelper.GetChangedEntities<T>(EntityState.Deleted, selector);
        }

        /// <summary>
        /// THREAD-SAFE. Uses QueryLock_1X000
        /// returns Entities with EntityState.Detached-State only
        /// </summary>
        /// <returns></returns>
        public IList<T> GetDetachedEntities<T>(Func<T, bool> selector = null) where T : class
        {
            return _ObjectContextHelper.GetChangedEntities<T>(EntityState.Detached, selector);
        }

        /// <summary>
        /// THREAD-SAFE. Uses QueryLock_1X000
        /// returns Entities with EntityState.Unchanged-State only
        /// </summary>
        /// <returns></returns>
        public IList<T> GetUnchangedEntities<T>(Func<T, bool> selector = null) where T : class
        {
            return _ObjectContextHelper.GetChangedEntities<T>(EntityState.Unchanged, selector);
        }

        /// <summary>
        /// THREAD-SAFE. Uses QueryLock_1X000
        /// returns EntityState.Modified | EntityState.Added | EntityState.Deleted
        /// </summary>
        /// <returns></returns>
        public IList<T> GetChangedEntities<T>(Func<T, bool> selector = null) where T : class
        {
            return _ObjectContextHelper.GetChangedEntities<T>(EntityState.Modified | EntityState.Added | EntityState.Deleted, selector);
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
        public void AutoRefresh(VBEntityObject entityObject)
        {
            _ObjectContextHelper.AutoRefresh(entityObject);
        }

        /// <summary>
        /// Refreshes all EntityObjects in the EntityCollection if not in modified state. Else it leaves it untouched.
        /// Attention: This method will only refresh the entities with entity keys that are tracked by the ObjectContext. 
        /// If changes are made in background on the database you shoud use the method AutoLoad, to retrieve new Entries from the Database!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityCollection"></param>
        public void AutoRefresh<T>(ICollection<T> entityCollection, CollectionEntry entry) where T : class
        {
            _ObjectContextHelper.AutoRefresh<T>(entityCollection, entry);
        }

        /// <summary>
        /// Queries the Database an refreshes the collection if not in modified state. MergeOption.OverwriteChanges
        /// Els if in modified state, then colletion is only refreshed with MergeOption.AppendOnly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityCollection"></param>
        public void AutoLoad<T>(ICollection<T> entityCollection, CollectionEntry entry) where T : class
        {
            _ObjectContextHelper.AutoLoad<T>(entityCollection, entry);
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

        public bool ACUrlTypeInfo(string acUrl, ref ACUrlTypeInfo acUrlTypeInfo)
        {
            if (_ObjectContextHelper == null)
                return false;
            return _ObjectContextHelper.ACUrlTypeInfo(acUrl, ref acUrlTypeInfo);
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

        public void FullDetach(VBEntityObject obj)
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

        [NotMapped]
        private ACMonitorObject _11000_QueryLock_ = new ACMonitorObject(11000);
        [NotMapped]
        public ACMonitorObject QueryLock_1X000
        {
            get
            {
                return _11000_QueryLock_;
            }
        }

        #endregion

        #endregion

        #region DBSetToList

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACAssembly> ACAssemblyList
        {
            get
            {
                return ACAssembly.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACChangeLog> ACChangeLogList
        {
            get
            {
                return ACChangeLog.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClass> ACClassList
        {
            get
            {
                return ACClass.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClassConfig> ACClassConfigList
        {
            get
            {
                return ACClassConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClassDesign> ACClassDesignList
        {
            get
            {
                return ACClassDesign.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClassMessage> ACClassMessageList
        {
            get
            {
                return ACClassMessage.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClassMethod> ACClassMethodList
        {
            get
            {
                return ACClassMethod.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClassMethodConfig> ACClassMethodConfigList
        {
            get
            {
                return ACClassMethodConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClassProperty> ACClassPropertyList
        {
            get
            {
                return ACClassProperty.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClassPropertyRelation> ACClassPropertyRelationList
        {
            get
            {
                return ACClassPropertyRelation.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClassTask> ACClassTaskList
        {
            get
            {
                return ACClassTask.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClassTaskValue> ACClassTaskValueList
        {
            get
            {
                return ACClassTaskValue.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClassTaskValuePos> ACClassTaskValuePosList
        {
            get
            {
                return ACClassTaskValuePos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClassText> ACClassTextList
        {
            get
            {
                return ACClassText.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClassWF> ACClassWFList
        {
            get
            {
                return ACClassWF.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACClassWFEdge> ACClassWFEdgeList
        {
            get
            {
                return ACClassWFEdge.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACPackage> ACPackageList
        {
            get
            {
                return ACPackage.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACProgram> ACProgramList
        {
            get
            {
                return ACProgram.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACProgramConfig> ACProgramConfigList
        {
            get
            {
                return ACProgramConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACProgramLog> ACProgramLogList
        {
            get
            {
                return ACProgramLog.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACProgramLogTask> ACProgramLogTaskList
        {
            get
            {
                return ACProgramLogTask.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACProgramLogView> ACProgramLogViewList
        {
            get
            {
                return ACProgramLogView.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACProject> ACProjectList
        {
            get
            {
                return ACProject.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACPropertyLog> ACPropertyLogList
        {
            get
            {
                return ACPropertyLog.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACPropertyLogRule> ACPropertyLogRuleList
        {
            get
            {
                return ACPropertyLogRule.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<Calendar> CalendarList
        {
            get
            {
                return Calendar.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<CalendarHoliday> CalendarHolidayList
        {
            get
            {
                return CalendarHoliday.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<CalendarShift> CalendarShiftList
        {
            get
            {
                return CalendarShift.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<CalendarShiftPerson> CalendarShiftPersonList
        {
            get
            {
                return CalendarShiftPerson.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<Company> CompanyList
        {
            get
            {
                return Company.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<CompanyAddress> CompanyAddressList
        {
            get
            {
                return CompanyAddress.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<CompanyAddressDepartment> CompanyAddressDepartmentList
        {
            get
            {
                return CompanyAddressDepartment.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<CompanyAddressUnloadingpoint> CompanyAddressUnloadingpointList
        {
            get
            {
                return CompanyAddressUnloadingpoint.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<CompanyMaterial> CompanyMaterialList
        {
            get
            {
                return CompanyMaterial.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<CompanyMaterialHistory> CompanyMaterialHistoryList
        {
            get
            {
                return CompanyMaterialHistory.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<CompanyMaterialPickup> CompanyMaterialPickupList
        {
            get
            {
                return CompanyMaterialPickup.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<CompanyMaterialStock> CompanyMaterialStockList
        {
            get
            {
                return CompanyMaterialStock.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<CompanyPerson> CompanyPersonList
        {
            get
            {
                return CompanyPerson.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<CompanyPersonRole> CompanyPersonRoleList
        {
            get
            {
                return CompanyPersonRoleList.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<DeliveryNote> DeliveryNoteList
        {
            get
            {
                return DeliveryNote.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<DeliveryNotePos> DeliveryNotePosList
        {
            get
            {
                return DeliveryNotePos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<DemandOrder> DemandOrderList
        {
            get
            {
                return DemandOrder.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<DemandOrderPos> DemandOrderPosList
        {
            get
            {
                return DemandOrderPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<DemandPrimary> DemandPrimaryList
        {
            get
            {
                return DemandPrimary.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<DemandProdOrder> DemandProdOrderList
        {
            get
            {
                return DemandProdOrder.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<Facility> FacilityList
        {
            get
            {
                return Facility.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityBooking> FacilityBookingList
        {
            get
            {
                return FacilityBooking.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityBookingCharge> FacilityBookingChargeList
        {
            get
            {
                return FacilityBookingCharge.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityCharge> FacilityChargeList
        {
            get
            {
                return FacilityCharge.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityHistory> FacilityHistoryList
        {
            get
            {
                return FacilityHistory.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityInventory> FacilityInventoryList
        {
            get
            {
                return FacilityInventory.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityInventoryPos> FacilityInventoryPosList
        {
            get
            {
                return FacilityInventoryPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityLot> FacilityLotList
        {
            get
            {
                return FacilityLot.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityLotStock> FacilityLotStockList
        {
            get
            {
                return FacilityLotStock.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityMaterial> FacilityMaterialList
        {
            get
            {
                return FacilityMaterial.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityMaterialOEE> FacilityMaterialOEEList
        {
            get
            {
                return FacilityMaterialOEE.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityMDSchedulingGroup> FacilityMDSchedulingGroupList
        {
            get
            {
                return FacilityMDSchedulingGroup.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityPreBooking> FacilityPreBookingList
        {
            get
            {
                return FacilityPreBooking.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityReservation> FacilityReservationList
        {
            get
            {
                return FacilityReservation.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<FacilityStock> FacilityStockList
        {
            get
            {
                return FacilityStock.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<History> HistoryList
        {
            get
            {
                return History.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<HistoryConfig> HistoryConfigList
        {
            get
            {
                return HistoryConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<InOrder> InOrderList
        {
            get
            {
                return InOrder.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<InOrderConfig> InOrderConfigList
        {
            get
            {
                return InOrderConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<InOrderPos> InOrderPosList
        {
            get
            {
                return InOrderPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<InRequest> InRequestList
        {
            get
            {
                return InRequest.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<InRequestConfig> InRequestConfigList
        {
            get
            {
                return InRequestConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<InRequestPos> InRequestPosList
        {
            get
            {
                return InRequestPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<Invoice> InvoiceList
        {
            get
            {
                return Invoice.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<InvoicePos> InvoicePosList
        {
            get
            {
                return InvoicePos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<JobTableRecalcActualQuantity> JobTableRecalcActualQuantityList
        {
            get
            {
                return JobTableRecalcActualQuantity.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<Label> LabelList
        {
            get
            {
                return Label.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<LabelTranslation> LabelTranslationList
        {
            get
            {
                return LabelTranslation.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<LabOrder> LabOrderList
        {
            get
            {
                return LabOrder.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<LabOrderPos> LabOrderPosList
        {
            get
            {
                return LabOrderPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MachineMaterialPosView> MachineMaterialPosViewList
        {
            get
            {
                return MachineMaterialPosView.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MachineMaterialRelView> MachineMaterialRelViewList
        {
            get
            {
                return MachineMaterialRelView.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MachineMaterialView> MachineMaterialViewList
        {
            get
            {
                return MachineMaterialView.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaintACClass> MaintACClassList
        {
            get
            {
                return MaintACClass.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaintACClassProperty> MaintACClassPropertyList
        {
            get
            {
                return MaintACClassProperty.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaintOrder> MaintOrderList
        {
            get
            {
                return MaintOrder.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaintOrderAssignment> MaintOrderAssignmentList
        {
            get
            {
                return MaintOrderAssignment.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaintOrderPos> MaintOrderPosList
        {
            get
            {
                return MaintOrderPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaintOrderProperty> MaintOrderPropertyList
        {
            get
            {
                return MaintOrderProperty.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaintOrderTask> MaintOrderTaskList
        {
            get
            {
                return MaintOrderTask.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<Material> MaterialList
        {
            get
            {
                return Material.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaterialCalculation> MaterialCalculationList
        {
            get
            {
                return MaterialCalculation.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaterialConfig> MaterialConfigList
        {
            get
            {
                return MaterialConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaterialGMPAdditive> MaterialGMPAdditiveList
        {
            get
            {
                return MaterialGMPAdditive.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaterialHistory> MaterialHistoryList
        {
            get
            {
                return MaterialHistory.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaterialStock> MaterialStockList
        {
            get
            {
                return MaterialStock.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaterialUnit> MaterialUnitList
        {
            get
            {
                return MaterialUnit.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaterialWF> MaterialWFList
        {
            get
            {
                return MaterialWF.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaterialWFACClassMethod> MaterialWFACClassMethodList
        {
            get
            {
                return MaterialWFACClassMethod.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaterialWFACClassMethodConfig> MaterialWFACClassMethodConfigList
        {
            get
            {
                return MaterialWFACClassMethodConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaterialWFConnection> MaterialWFConnectionList
        {
            get
            {
                return MaterialWFConnection.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MaterialWFRelation> MaterialWFRelationList
        {
            get
            {
                return MaterialWFRelation.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDBalancingMode> MDBalancingModeList
        {
            get
            {
                return MDBalancingMode.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDBatchPlanGroup> MDBatchPlanGroupList
        {
            get
            {
                return MDBatchPlanGroup.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDBookingNotAvailableMode> MDBookingNotAvailableModeList
        {
            get
            {
                return MDBookingNotAvailableMode.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDCostCenter> MDCostCenterList
        {
            get
            {
                return MDCostCenter.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDCountry> MDCountryList
        {
            get
            {
                return MDCountry.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDCountryLand> MDCountryLandList
        {
            get
            {
                return MDCountryLand.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDCountrySalesTax> MDCountrySalesTaxList
        {
            get
            {
                return MDCountrySalesTax.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDCountrySalesTaxMaterial> MDCountrySalesTaxMaterialList
        {
            get
            {
                return MDCountrySalesTaxMaterial.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDCountrySalesTaxMDMaterialGroup> MDCountrySalesTaxMDMaterialGroupList
        {
            get
            {
                return MDCountrySalesTaxMDMaterialGroup.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDCurrency> MDCurrencyList
        {
            get
            {
                return MDCurrency.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDCurrencyExchange> MDCurrencyExchangeList
        {
            get
            {
                return MDCurrencyExchange.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDDelivNoteState> MDDelivNoteStateList
        {
            get
            {
                return MDDelivNoteState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDDelivPosLoadState> MDDelivPosLoadStateList
        {
            get
            {
                return MDDelivPosLoadState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDDelivPosState> MDDelivPosStateList
        {
            get
            {
                return MDDelivPosState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDDelivType> MDDelivTypeList
        {
            get
            {
                return MDDelivType.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDDemandOrderState> MDDemandOrderStateList
        {
            get
            {
                return MDDemandOrderState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDFacilityInventoryPosState> MDFacilityInventoryPosStateList
        {
            get
            {
                return MDFacilityInventoryPosState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDFacilityInventoryState> MDFacilityInventoryStateList
        {
            get
            {
                return MDFacilityInventoryState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDFacilityManagementType> MDFacilityManagementTypeList
        {
            get
            {
                return MDFacilityManagementType.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDFacilityType> MDFacilityTypeList
        {
            get
            {
                return MDFacilityType.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDFacilityVehicleType> MDFacilityVehicleTypeList
        {
            get
            {
                return MDFacilityVehicleType.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDGMPAdditive> MDGMPAdditiveList
        {
            get
            {
                return MDGMPAdditive.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDGMPMaterialGroup> MDGMPMaterialGroupList
        {
            get
            {
                return MDGMPMaterialGroup.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDGMPMaterialGroupPos> MDGMPMaterialGroupPosList
        {
            get
            {
                return MDGMPMaterialGroupPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDInOrderPosState> MDInOrderPosStateList
        {
            get
            {
                return MDInOrderPosState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDInOrderState> MDInOrderStateList
        {
            get
            {
                return MDInOrderState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDInOrderType> MDInOrderTypeList
        {
            get
            {
                return MDInOrderType.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDInRequestState> MDInRequestStateList
        {
            get
            {
                return MDInRequestState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDInventoryManagementType> MDInventoryManagementTypeList
        {
            get
            {
                return MDInventoryManagementType.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDInvoiceState> MDInvoiceStateList
        {
            get
            {
                return MDInvoiceState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDInvoiceType> MDInvoiceTypeList
        {
            get
            {
                return MDInvoiceType.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDLabOrderPosState> MDLabOrderPosStateList
        {
            get
            {
                return MDLabOrderPosState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDLabOrderState> MDLabOrderStateList
        {
            get
            {
                return MDLabOrderState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDLabTag> MDLabTagList
        {
            get
            {
                return MDLabTag.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDMaintMode> MDMaintModeList
        {
            get
            {
                return MDMaintMode.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDMaintOrderPropertyState> MDMaintOrderPropertyStateList
        {
            get
            {
                return MDMaintOrderPropertyState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDMaintOrderState> MDMaintOrderStateList
        {
            get
            {
                return MDMaintOrderState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDMaintTaskState> MDMaintTaskStateList
        {
            get
            {
                return MDMaintTaskState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDMaterialGroup> MDMaterialGroupList
        {
            get
            {
                return MDMaterialGroup.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDMaterialType> MDMaterialTypeList
        {
            get
            {
                return MDMaterialType.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDMovementReason> MDMovementReasonList
        {
            get
            {
                return MDMovementReason.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDOutOfferState> MDOutOfferStateList
        {
            get
            {
                return MDOutOfferState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDOutOrderPlanState> MDOutOrderPlanStateList
        {
            get
            {
                return MDOutOrderPlanState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDOutOrderPosState> MDOutOrderPosStateList
        {
            get
            {
                return MDOutOrderPosState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDOutOrderState> MDOutOrderStateList
        {
            get
            {
                return MDOutOrderState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDOutOrderType> MDOutOrderTypeList
        {
            get
            {
                return MDOutOrderType.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDPickingType> MDPickingTypeList
        {
            get
            {
                return MDPickingType.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDProcessErrorAction> MDProcessErrorActionList
        {
            get
            {
                return MDProcessErrorAction.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDProdOrderPartslistPosState> MDProdOrderPartslistPosStateList
        {
            get
            {
                return MDProdOrderPartslistPosState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDProdOrderState> MDProdOrderStateList
        {
            get
            {
                return MDProdOrderState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDRatingComplaintType> MDRatingComplaintTypeList
        {
            get
            {
                return MDRatingComplaintType.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDReleaseState> MDReleaseStateList
        {
            get
            {
                return MDReleaseState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDReservationMode> MDReservationModeList
        {
            get
            {
                return MDReservationMode.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDSchedulingGroup> MDSchedulingGroupList
        {
            get
            {
                return MDSchedulingGroup.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDSchedulingGroupWF> MDSchedulingGroupWFList
        {
            get
            {
                return MDSchedulingGroupWF.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDTermOfPayment> MDTermOfPaymentList
        {
            get
            {
                return MDTermOfPayment.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDTimeRange> MDTimeRangeList
        {
            get
            {
                return MDTimeRange.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDToleranceState> MDToleranceStateList
        {
            get
            {
                return MDToleranceState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDTour> MDTourList
        {
            get
            {
                return MDTour.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDTourplanPosState> MDTourplanPosStateList
        {
            get
            {
                return MDTourplanPosState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDTourplanState> MDTourplanStateList
        {
            get
            {
                return MDTourplanState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDTransportMode> MDTransportModeList
        {
            get
            {
                return MDTransportMode.ToArray();
            }
        }
        /*
        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                return MDUnit.ToArray();
            }
        }
        */
        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDUnitConversion> MDUnitConversionList
        {
            get
            {
                return MDUnitConversion.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDVisitorCard> MDVisitorCardList
        {
            get
            {
                return MDVisitorCard.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDVisitorCardState> MDVisitorCardStateList
        {
            get
            {
                return MDVisitorCardState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDVisitorVoucherState> MDVisitorVoucherStateList
        {
            get
            {
                return MDVisitorVoucherState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDZeroStockState> MDZeroStockStateList
        {
            get
            {
                return MDZeroStockState.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MsgAlarmLog> MsgAlarmLogList
        {
            get
            {
                return MsgAlarmLog.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<OperationLog> OperationLogList
        {
            get
            {
                return OperationLog.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<OrderLog> OrderLogList
        {
            get
            {
                return OrderLog.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<OrderLogPosMachines> OrderLogPosMachinesList
        {
            get
            {
                return OrderLogPosMachines.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<OrderLogPosView> OrderLogPosViewList
        {
            get
            {
                return OrderLogPosView.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<OrderLogRelView> OrderLogRelViewList
        {
            get
            {
                return OrderLogRelView.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<OutOffer> OutOfferList
        {
            get
            {
                return OutOffer.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<OutOfferConfig> OutOfferConfigList
        {
            get
            {
                return OutOfferConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<OutOfferPos> OutOfferPosList
        {
            get
            {
                return OutOfferPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<OutOrder> OutOrderList
        {
            get
            {
                return OutOrder.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<OutOrderConfig> OutOrderConfigList
        {
            get
            {
                return OutOrderConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<OutOrderPos> OutOrderPosList
        {
            get
            {
                return OutOrderPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<OutOrderPosSplit> OutOrderPosSplitList
        {
            get
            {
                return OutOrderPosSplit.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<OutOrderPosUtilization> OutOrderPosUtilizationList
        {
            get
            {
                return OutOrderPosUtilization.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<Partslist> PartslistList
        {
            get
            {
                return Partslist.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<PartslistACClassMethod> PartslistACClassMethodList
        {
            get
            {
                return PartslistACClassMethod.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<PartslistConfig> PartslistConfigList
        {
            get
            {
                return PartslistConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<PartslistPos> PartslistPosList
        {
            get
            {
                return PartslistPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<PartslistPosRelation> PartslistPosRelationList
        {
            get
            {
                return PartslistPosRelation.ToArray();
            }
        }
        
        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<PartslistPosSplit> PartslistPosSplitList
        {
            get
            {
                return PartslistPosSplit.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<PartslistStock> PartslistStockList
        {
            get
            {
                return PartslistStock.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<Picking> PickingList
        {
            get
            {
                return Picking.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<PickingConfig> PickingConfigList
        {
            get
            {
                return PickingConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<PickingPos> PickingPosList
        {
            get
            {
                return PickingPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<PickingPosProdOrderPartslistPos> PickingPosProdOrderPartslistPosList
        {
            get
            {
                return PickingPosProdOrderPartslistPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<PlanningMR> PlanningMRList
        {
            get
            {
                return PlanningMR.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<PlanningMRProposal> PlanningMRProposalList
        {
            get
            {
                return PlanningMRProposal.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<PriceList> PriceListList
        {
            get
            {
                return PriceList.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<PriceListMaterial> PriceListMaterialList
        {
            get
            {
                return PriceListMaterial.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ProdOrder> ProdOrderList
        {
            get
            {
                return ProdOrder.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ProdOrderBatch> ProdOrderBatchList
        {
            get
            {
                return ProdOrderBatch.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ProdOrderBatchPlan> ProdOrderBatchPlanList
        {
            get
            {
                return ProdOrderBatchPlan.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ProdOrderConnectionsDetailView> ProdOrderConnectionsDetailViewList
        {
            get
            {
                return ProdOrderConnectionsDetailView.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ProdOrderConnectionsView> ProdOrderConnectionsViewList
        {
            get
            {
                return ProdOrderConnectionsView.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ProdOrderInwardsView> ProdOrderInwardsViewList
        {
            get
            {
                return ProdOrderInwardsView.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ProdOrderOutwardsView> ProdOrderOutwardsViewList
        {
            get
            {
                return ProdOrderOutwardsView.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ProdOrderPartslist> ProdOrderPartslistList
        {
            get
            {
                return ProdOrderPartslist.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ProdOrderPartslistConfig> ProdOrderPartslistConfigList
        {
            get
            {
                return ProdOrderPartslistConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ProdOrderPartslistPos> ProdOrderPartslistPosList
        {
            get
            {
                return ProdOrderPartslistPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ProdOrderPartslistPosFacilityLot> ProdOrderPartslistPosFacilityLotList
        {
            get
            {
                return ProdOrderPartslistPosFacilityLot.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ProdOrderPartslistPosRelation> ProdOrderPartslistPosRelationList
        {
            get
            {
                return ProdOrderPartslistPosRelation.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ProdOrderPartslistPosSplit> ProdOrderPartslistPosSplitList
        {
            get
            {
                return ProdOrderPartslistPosSplit.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<Rating> RatingList
        {
            get
            {
                return Rating.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<RatingComplaint> RatingComplaintList
        {
            get
            {
                return RatingComplaint.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3FilterTracking> TandTv3FilterTrackingList
        {
            get
            {
                return TandTv3FilterTracking.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3FilterTrackingMaterial> TandTv3FilterTrackingMaterialList
        {
            get
            {
                return TandTv3FilterTrackingMaterial.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MDBookingDirection> TandTv3MDBookingDirectionList
        {
            get
            {
                return TandTv3MDBookingDirection.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MDTrackingDirection> TandTv3MDTrackingDirectionList
        {
            get
            {
                return TandTv3MDTrackingDirection.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MDTrackingStartItemType> TandTv3MDTrackingStartItemTypeList
        {
            get
            {
                return TandTv3MDTrackingStartItemType.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MixPoint> TandTv3MixPointList
        {
            get
            {
                return TandTv3MixPoint.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MixPointDeliveryNotePos> TandTv3MixPointDeliveryNotePosList 
        {
            get
            {
                return TandTv3MixPointDeliveryNotePos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MixPointFacility> TandTv3MixPointFacilityList
        {
            get
            {
                return TandTv3MixPointFacility.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MixPointFacilityBookingCharge> TandTv3MixPointFacilityBookingChargeList
        {
            get
            {
                return TandTv3MixPointFacilityBookingCharge.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MixPointFacilityLot> TandTv3MixPointFacilityLotList
        {
            get
            {
                return TandTv3MixPointFacilityLot.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MixPointFacilityPreBooking> TandTv3MixPointFacilityPreBookingList
        {
            get
            {
                return TandTv3MixPointFacilityPreBooking.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MixPointInOrderPos> TandTv3MixPointInOrderPosList
        {
            get
            {
                return TandTv3MixPointInOrderPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MixPointOutOrderPos> TandTv3MixPointOutOrderPosList
        {
            get
            {
                return TandTv3MixPointOutOrderPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MixPointPickingPos> TandTv3MixPointPickingPosList
        {
            get
            {
                return TandTv3MixPointPickingPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MixPointProdOrderPartslistPos> TandTv3MixPointProdOrderPartslistPosList
        {
            get
            {
                return TandTv3MixPointProdOrderPartslistPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MixPointProdOrderPartslistPosRelation> TandTv3MixPointProdOrderPartslistPosRelationList
        {
            get
            {
                return TandTv3MixPointProdOrderPartslistPosRelation.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3MixPointRelation> TandTv3MixPointRelationList
        {
            get
            {
                return TandTv3MixPointRelation.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TandTv3Step> TandTv3StepList
        {
            get
            {
                return TandTv3Step.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<Tourplan> TourplanList
        {
            get
            {
                return Tourplan.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TourplanConfig> TourplanConfigList
        {
            get
            {
                return TourplanConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<TourplanPos> TourplanPosList
        {
            get
            {
                return TourplanPos.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<UserSettings> UserSettingsList
        {
            get
            {
                return UserSettings.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBConfig> VBConfigList
        {
            get
            {
                return VBConfig.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBGroup> VBGroupList
        {
            get
            {
                return VBGroup.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBGroupRight> VBGroupRightList
        {
            get
            {
                return VBGroupRight.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBLanguage> VBLanguageList
        {
            get
            {
                return VBLanguage.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBLicense> VBLicenseList
        {
            get
            {
                return VBLicense.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBNoConfiguration> VBNoConfigurationList
        {
            get
            {
                return VBNoConfiguration.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBSystem> VBSystemList
        {
            get
            {
                return VBSystem.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBSystemColumns> VBSystemColumnsList
        {
            get
            {
                return VBSystemColumns.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBTranslationView> VBTranslationViewList
        {
            get
            {
                return VBTranslationView.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBUser> VBUserList
        {
            get
            {
                return VBUser.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBUserACClassDesign> VBUserACClassDesignList
        {
            get
            {
                return VBUserACClassDesign.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBUserACProject> VBUserACProjectList
        {
            get
            {
                return VBUserACProject.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBUserGroup> VBUserGroupList
        {
            get
            {
                return VBUserGroup.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VBUserInstance> VBUserInstanceList
        {
            get
            {
                return VBUserInstance.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<Visitor> VisitorList
        {
            get
            {
                return Visitor.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<VisitorVoucher> VisitorVoucherList
        {
            get
            {
                return VisitorVoucher.ToArray();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<Weighing> WeighingList
        {
            get
            {
                return Weighing.ToArray();
            }
        }

        #endregion

        #region Lists

        [NotMapped]
        public IEnumerable<ACValueItem> InOrderStatesList
        {
            get
            {
                return gip.mes.datamodel.MDInOrderState.InOrderStatesList;
            }
        }


        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> ZeroStockStatesList
        {
            get
            {
                return gip.mes.datamodel.MDZeroStockState.ZeroStockStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> ReservationModesList
        {
            get
            {
                return gip.mes.datamodel.MDReservationMode.ReservationModesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> ReleaseStatesList
        {
            get
            {
                return gip.mes.datamodel.MDReleaseState.ReleaseStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> OutOrderStatesList
        {
            get
            {
                return gip.mes.datamodel.MDOutOrderState.OutOrderStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> InvoiceStatesList
        {
            get
            {
                return gip.mes.datamodel.MDInvoiceState.InvoiceStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> OutOrderPosStatesList
        {
            get
            {
                return gip.mes.datamodel.MDOutOrderPosState.OutOrderPosStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> OutOrderPlanStatesList
        {
            get
            {
                return gip.mes.datamodel.MDOutOrderPlanState.OutOrderPlanStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> OutOfferStatesList
        {
            get
            {
                return gip.mes.datamodel.MDOutOfferState.OutOfferStatesList;
            }
        }

        [NotMapped]
        private IEnumerable<ACValueItem> _MovementReasonsList;

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> MovementReasonsList
        {
            get
            {

                if (_MovementReasonsList == null)
                {
                    gip.core.datamodel.ACClass enumClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(MDMovementReason));
                    if (enumClass != null && enumClass.ACValueListForEnum != null)
                        _MovementReasonsList = enumClass.ACValueListForEnum;
                    else
                        _MovementReasonsList = new ACValueListMovementReasonsEnum();
                }
                return _MovementReasonsList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> InRequestStatesList
        {
            get
            {
                return gip.mes.datamodel.MDInRequestState.InRequestStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> InOrderPosStatesList
        {
            get
            {
                return gip.mes.datamodel.MDInOrderPosState.InOrderPosStatesList;
            }
        }


        [NotMapped]
        ACValueItemList _FacilityTypesEnumList = null;
        [ACPropertyInfo(9999)]
        [NotMapped]
        public ACValueItemList FacilityTypesList
        {
            get
            {
                if (_FacilityTypesEnumList == null)
                {
                    gip.core.datamodel.ACClass enumClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(FacilityTypesEnum));
                    if (enumClass != null && enumClass.ACValueListForEnum != null)
                        _FacilityTypesEnumList = enumClass.ACValueListForEnum;
                    else
                        _FacilityTypesEnumList = new ACValueListFacilityTypesEnum();
                }
                return _FacilityTypesEnumList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> FacilityManagementTypesList
        {
            get
            {
                return gip.mes.datamodel.MDFacilityManagementType.FacilityManagementTypesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> BookingNotAvailableModesList
        {
            get
            {
                return gip.mes.datamodel.MDBookingNotAvailableMode.BookingNotAvailableModesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> BalancingModesList
        {
            get
            {
                return gip.mes.datamodel.MDBalancingMode.BalancingModesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> LabOrderPosStatesList
        {
            get
            {
                return gip.mes.datamodel.MDLabOrderPosState.LabOrderPosStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> LabOrderStatesList
        {
            get
            {
                return gip.mes.datamodel.MDLabOrderState.LabOrderStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> LabTagsList
        {
            get
            {
                return gip.mes.datamodel.MDLabTag.LabTagsList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> MaterialGroupTypesList
        {
            get
            {
                return gip.mes.datamodel.MDMaterialGroup.MaterialGroupTypesList.ToList();
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> VisitorVoucherStatesList
        {
            get
            {
                return gip.mes.datamodel.MDVisitorVoucherState.VisitorVoucherStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> VisitorCardStatesList
        {
            get
            {
                return gip.mes.datamodel.MDVisitorCardState.VisitorCardStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> DelivPosLoadStateList
        {
            get
            {
                return gip.mes.datamodel.MDDelivPosLoadState.DelivPosLoadStateList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> DelivPosStatesList
        {
            get
            {
                return gip.mes.datamodel.MDDelivPosState.DelivPosStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> DelivNoteStatesList
        {
            get
            {
                return gip.mes.datamodel.MDDelivNoteState.DelivNoteStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> DelivTypesList
        {
            get
            {
                return gip.mes.datamodel.MDDelivType.DelivTypesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> TransportModesList
        {
            get
            {
                return gip.mes.datamodel.MDTransportMode.TransportModesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> TourplanPosStatesList
        {
            get
            {
                return gip.mes.datamodel.MDTourplanPosState.TourplanPosStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> TourplanStatesList
        {
            get
            {
                return gip.mes.datamodel.MDTourplanState.TourplanStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> ProdOrderPartslistPosStatesList
        {
            get
            {
                return gip.mes.datamodel.MDProdOrderPartslistPosState.ProdOrderPartslistPosStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> ProcessErrorActionsList
        {
            get
            {
                return gip.mes.datamodel.MDProcessErrorAction.ProcessErrorActionList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> DemandOrderStatesList
        {
            get
            {
                return gip.mes.datamodel.MDDemandOrderState.DemandOrderStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> ToleranceStateList
        {
            get
            {
                return gip.mes.datamodel.MDToleranceState.ToleranceStatesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> MaintOrderStateList
        {
            get
            {
                return gip.mes.datamodel.MDMaintOrderState.MaintOrderStateList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> MaintModesList
        {
            get
            {
                return gip.mes.datamodel.MDMaintMode.MaintModesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> FacilityVehicleTypesList
        {
            get
            {
                return gip.mes.datamodel.MDFacilityVehicleType.FacilityVehicleTypesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> OrderTypesList
        {
            get
            {
                return GlobalApp.OrderTypesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> InvoiceTypesList
        {
            get
            {
                return GlobalApp.InvoiceTypesList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> FacilityBookingTypeList
        {
            get
            {
                return GlobalApp.FacilityBookingTypeList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> SIDimensionList
        {
            get
            {
                return GlobalApp.SIDimensionList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> PetroleumGroupList
        {
            get
            {
                return GlobalApp.PetroleumGroupList;
            }
        }

        [NotMapped]
        static ACValueItemList _MRPProcedureList = null;
        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> MRPProcedureList
        {
            get
            {
                if(_MRPProcedureList == null)
                {
                    var acClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(MRPProcedure));
                    if (acClass != null)
                        _MRPProcedureList = acClass.ACValueListForEnum;
                }
                return _MRPProcedureList;
            }
        }

        [NotMapped]
        static ACValueItemList _MRPPlanningPhaseList = null;
        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> MRPPlanningPhaseList
        {
            get
            {
                if (_MRPPlanningPhaseList == null)
                {
                    var acClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(MRPPlanningPhaseEnum));
                    if (acClass != null)
                        _MRPPlanningPhaseList = acClass.ACValueListForEnum;
                }
                return _MRPPlanningPhaseList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> DeliveryNoteTypeList
        {
            get
            {
                return GlobalApp.DeliveryNoteTypeList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> PickingTypeList
        {
            get
            {
                return GlobalApp.PickingTypeList;
            }
        }

        [NotMapped]
        private ACValueItemList _PickingStateList;
        [ACPropertyInfo(9999)]
        [NotMapped]
        public ACValueItemList PickingStateList
        {
            get
            {
                if (_PickingStateList == null)
                {
                    gip.core.datamodel.ACClass enumClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(PickingStateEnum));
                    if (enumClass != null && enumClass.ACValueListForEnum != null)
                        _PickingStateList = enumClass.ACValueListForEnum;
                    else
                        _PickingStateList = new ACValueListPickingStateEnum();
                }
                return _PickingStateList;
            }
        }

        [NotMapped]
        private ACValueItemList _PickingPreparationStatusList;
        [ACPropertyInfo(9999)]
        [NotMapped]
        public ACValueItemList PickingPreparationStatusList
        {
            get
            {
                if (_PickingPreparationStatusList == null)
                {
                    gip.core.datamodel.ACClass enumClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(PickingPreparationStatusEnum));
                    if (enumClass != null && enumClass.ACValueListForEnum != null)
                        _PickingPreparationStatusList = enumClass.ACValueListForEnum;
                    else
                        _PickingPreparationStatusList = new ACValueListPickingPreparationStatusEnum();
                }
                return _PickingPreparationStatusList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> TimePeriodsList
        {
            get
            {
                return GlobalApp.TimePeriodsList;
            }
        }

        [NotMapped]
        ACValueItemList _PostingBehaviourEnumList = null;
        [ACPropertyInfo(9999)]
        [NotMapped]
        public ACValueItemList PostingBehaviourEnumList
        {
            get
            {
                if (_PostingBehaviourEnumList == null)
                {
                    gip.core.datamodel.ACClass enumClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(PostingBehaviourEnum));
                    if (enumClass != null && enumClass.ACValueListForEnum != null)
                        _PostingBehaviourEnumList = enumClass.ACValueListForEnum;
                    else
                        _PostingBehaviourEnumList = new ACValueListPostingBehaviourEnum();
                }
                return _PostingBehaviourEnumList;
            }
        }

        /// <summary>
        /// Liste aller Adressen, die eine Factory sind und dem eigenen Unternehmen angehören
        /// </summary>
        [ACPropertyInfo(9999, "")]
        [NotMapped]
        public IEnumerable<CompanyAddress> FactoryCompanyAddressList
        {
            get
            {
                using (ACMonitor.Lock(QueryLock_1X000))
                {
                    return this.CompanyAddress.Where(c => c.IsFactory && c.Company.IsOwnCompany).ToList();
                }
            }
        }


        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                using (ACMonitor.Lock(QueryLock_1X000))
                {
                    return MDUnit.Where(c => c.IsQuantityUnit).OrderBy(c => c.SortIndex).ToList();
                }
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<Facility> FacilityLocation
        {
            get
            {
                using (ACMonitor.Lock(QueryLock_1X000))
                {
                    return Facility.Where(c => c.ParentFacilityID != null).ToList();
                }
            }
        }

        [ACPropertyInfo(9999, "", "en{'Tenant/Contractual partner'}de{'Mandant/Vertragspartner'}")]
        [NotMapped]
        public IEnumerable<Company> CPartnerCompanyList
        {
            get
            {
                using (ACMonitor.Lock(QueryLock_1X000))
                {
                    return Company.Where(c => c.IsTenant == true).ToList();
                }
            }
        }


        [NotMapped]
        static ACValueItemList _BatchPlanModeList = null;
        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> BatchPlanModeList
        {
            get
            {
                if (_BatchPlanModeList == null)
                {
                    var acClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(BatchPlanMode));
                    if (acClass != null)
                        _BatchPlanModeList = acClass.ACValueListForEnum;
                }
                return _BatchPlanModeList;
            }
        }

        [NotMapped]
        static ACValueItemList _BatchPlanStartModeEnumList = null;
        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> BatchPlanStartModeEnumList
        {
            get
            {
                if (_BatchPlanStartModeEnumList == null)
                {
                    var acClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(BatchPlanStartModeEnum));
                    if (acClass != null)
                        _BatchPlanStartModeEnumList = acClass.ACValueListForEnum;
                }
                return _BatchPlanStartModeEnumList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> BatchPlanStateList
        {
            get
            {
                return GlobalApp.BatchPlanStateList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> ReservationStateList
        {
            get
            {
                return GlobalApp.ReservationStateList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> LabOrderTypeList
        {
            get
            {
                return GlobalApp.LabOrderTypeList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> TrackAndTracingSearchModelList
        {
            get
            {
                return GlobalApp.TrackingAndTracingSearchModelSearchModelList;
            }
        }

        [ACPropertyInfo(9999)]
        [NotMapped]
        public IEnumerable<ACValueItem> MaterialProcessStateList
        {
            get
            {
                return GlobalApp.MaterialProcessStateList;
            }
        }

        [NotMapped]
        private ACValueItemList _WeighingStateList;
        [ACPropertyInfo(9999)]
        [NotMapped]
        public ACValueItemList WeighingStateList
        {
            get
            {
                if (_WeighingStateList == null)
                {
                    gip.core.datamodel.ACClass enumClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(WeighingStateEnum));
                    if (enumClass != null && enumClass.ACValueListForEnum != null)
                        _WeighingStateList = enumClass.ACValueListForEnum;
                    else
                        _WeighingStateList = new ACValueListWeighingStateEnum();
                }
                return _WeighingStateList;
            }
        }

        [NotMapped]
        ACValueItemList _PreferredParamStateList = null;
        [ACPropertyInfo(9999)]
        [NotMapped]
        public ACValueItemList PreferredParamStateList
        {
            get
            {
                if (_PreferredParamStateList == null)
                {
                    gip.core.datamodel.ACClass enumClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(PreferredParamStateEnum));
                    if (enumClass != null && enumClass.ACValueListForEnum != null)
                        _PreferredParamStateList = enumClass.ACValueListForEnum;
                    else
                        _PreferredParamStateList = new ACValueListPreferredParamStateEnum();
                }
                return _PreferredParamStateList;
            }
        }


        [ACPropertyInfo(9999)]
        public IEnumerable<ACValueItem> ProdOrderStatesList
        {
            get
            {
                return gip.mes.datamodel.MDProdOrderState.ProdOrderStatesList;
            }
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;


        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region precompiled Queries
        public static readonly Func<DatabaseApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates, IEnumerable<MDProdOrderPartslistPosState>> s_cQry_GetMDProdOrderPosState =
        EF.CompileQuery<DatabaseApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates, IEnumerable<MDProdOrderPartslistPosState>>(
            (ctx, state) => ctx.MDProdOrderPartslistPosState.Where(c => c.MDProdOrderPartslistPosStateIndex == (short)state)
        );

        public static readonly Func<DatabaseApp, MDDelivPosState.DelivPosStates, IEnumerable<MDDelivPosState>> s_cQry_GetMDDelivPosState =
        EF.CompileQuery<DatabaseApp, MDDelivPosState.DelivPosStates, IEnumerable<MDDelivPosState>>(
            (ctx, state) => ctx.MDDelivPosState.Where(c => c.MDDelivPosStateIndex == (short)state)
        );

        public static readonly Func<DatabaseApp, string, IEnumerable<MDPickingType>> s_cQry_GetMDPickingType =
        EF.CompileQuery<DatabaseApp, string, IEnumerable<MDPickingType>>(
            (ctx, key) => ctx.MDPickingType.Where(c => c.MDKey == key)
        );

        public static readonly Func<DatabaseApp, MDDelivPosLoadState.DelivPosLoadStates, IEnumerable<MDDelivPosLoadState>> s_cQry_GetMDDelivPosLoadState =
        EF.CompileQuery<DatabaseApp, MDDelivPosLoadState.DelivPosLoadStates, IEnumerable<MDDelivPosLoadState>>(
            (ctx, state) => ctx.MDDelivPosLoadState.Where(c => c.MDDelivPosLoadStateIndex == (short)state)
        );

        public static readonly Func<DatabaseApp, MDProdOrderState.ProdOrderStates, IEnumerable<MDProdOrderState>> s_cQry_GetMDProdOrderState =
        EF.CompileQuery<DatabaseApp, MDProdOrderState.ProdOrderStates, IEnumerable<MDProdOrderState>>(
            (ctx, state) => ctx.MDProdOrderState.Where(c => c.MDProdOrderStateIndex == (short)state)
        );

        public static readonly Func<DatabaseApp, MDDelivNoteState.DelivNoteStates, IEnumerable<MDDelivNoteState>> s_cQry_GetMDDelivNoteState =
        EF.CompileQuery<DatabaseApp, MDDelivNoteState.DelivNoteStates, IEnumerable<MDDelivNoteState>>(
            (ctx, state) => ctx.MDDelivNoteState.Where(c => c.MDDelivNoteStateIndex == (short)state)
        );

        public static readonly Func<DatabaseApp, MDBookingNotAvailableMode.BookingNotAvailableModes, IEnumerable<MDBookingNotAvailableMode>> s_cQry_GetMDBookingNotAvailableMode =
        EF.CompileQuery<DatabaseApp, MDBookingNotAvailableMode.BookingNotAvailableModes, IEnumerable<MDBookingNotAvailableMode>>(
            (ctx, mode) => ctx.MDBookingNotAvailableMode.Where(c => c.MDBookingNotAvailableModeIndex == (short)mode)
        );

        public static readonly Func<DatabaseApp, MDBalancingMode.BalancingModes, IEnumerable<MDBalancingMode>> s_cQry_GetMDBalancingMode =
        EF.CompileQuery<DatabaseApp, MDBalancingMode.BalancingModes, IEnumerable<MDBalancingMode>>(
            (ctx, mode) => ctx.MDBalancingMode.Where(c => c.MDBalancingModeIndex == (short)mode)
        );

        #endregion

    }
}
