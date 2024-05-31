using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [Flags]
    /// <summary>
    /// EmptySensor + FullSensor = 0x3 (3)
    /// EmptySensor + ScaleWithStock = 0x5 (5)
    /// FullSensor + ScaleWithStock = 0x6 (6)
    /// EmptySensor + FullSensor + ScaleWithStock = 0x7 (7)
    /// </summary>
    public enum FacilityFillValidation
    {
        None = 0x0,
        EmptySensor = 0x1,
        FullSensor = 0x2,
        ScaleWithStock = 0x4
    }


    // Facility (Lagerplatz)
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.Facility, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "FacilityNo", ConstApp.Number, "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, "FacilityName", ConstApp.Name, "", "", true, MinLength = 1)]
    [ACPropertyEntity(3, "Facility1_ParentFacility", "en{'Parent Facility'}de{'Übergeorneter Lagerplatz'}", Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(4, "MDFacilityType", "en{'Type'}de{'Typ'}", Const.ContextDatabase + "\\MDFacilityType" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(5, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, "InwardEnabled", "en{'Inward Enabled'}de{'Eingang möglich'}", "", "", true)]
    [ACPropertyEntity(7, "OutwardEnabled", "en{'Outward Enabled'}de{'Ausgang möglich'}", "", "", true)]
    [ACPropertyEntity(8, "ReservedQuantity", "en{'Reserved Quantity'}de{'Reservierte Menge'}", "", "", true)]
    [ACPropertyEntity(9, "OrderedQuantity", "en{'Ordered Quantity'}de{'Bestellte Menge'}", "", "", true)]
    [ACPropertyEntity(10, "Facility1_IncomingFacility", "en{'Incoming Facility'}de{'Eingangslagerplatz'}", Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(11, "Facility1_OutgoingFacility", "en{'Outgoing Facility'}de{'Ausgangslagerplatz'}", Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(12, "Facility1_LockedFacility", "en{'Locked Facility'}de{'Gesperrter Lagerplatz'}", Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(13, Company.ClassName, ConstApp.Company, Const.ContextDatabase + "\\" + Company.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(14, CompanyPerson.ClassName, "en{'Driver'}de{'Fahrer'}", Const.ContextDatabase + "\\" + CompanyPerson.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(15, "VisitorCardFacilityCharge", "en{'Chip Card Charge'}de{'Chipkarte Charge'}", Const.ContextDatabase + "\\" + FacilityCharge.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(16, "Tara", "en{'Tara'}de{'Tara'}", "", "", true)]
    [ACPropertyEntity(17, "MaxWeightCapacity", "en{'Max. Weight Capacity'}de{'Max. Gewichtskapazität'}", "", "", true)]
    [ACPropertyEntity(18, "MaxVolumeCapacity", "en{'Max. Volume Capacity'}de{'Max. Volumenkapazität'}", "", "", true)]
    [ACPropertyEntity(19, "Drivername", "en{'Driver Name'}de{'Fahrername'}", "", "", true)]
    [ACPropertyEntity(20, "Tolerance", "en{'Empty Tolerance'}de{'Leer-Toleranz'}", "", "", true)]
    [ACPropertyEntity(21, "HighLidNo", "en{'Fill level validation flags'}de{'Füllstandsvalidierung-flags'}", "", "", true)]
    [ACPropertyEntity(22, "FittingsDistanceFront", "en{'Fittings Distance Front'}de{'Anbauten Abstand vorne'}", "", "", true)]
    [ACPropertyEntity(23, "FittingsDistanceBehind", "en{'Fittings Distance Back'}de{'Anbauten Abstand hinten'}", "", "", true)]
    [ACPropertyEntity(24, "DistanceFront", "en{'Distance Front'}de{'Abstand vorne'}", "", "", true)]
    [ACPropertyEntity(25, "DistanceBehind", "en{'Distance Back'}de{'Abstand hinten'}", "", "", true)]
    [ACPropertyEntity(27, "Partslist", ConstApp.BOM, Const.ContextDatabase + "\\" + Partslist.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(28, MDUnit.ClassName, ConstApp.MDUnit, Const.ContextDatabase + "\\MDWeightUnitList", "", true)]
    [ACPropertyEntity(29, "Density", "en{'Density [g/dm³]'}de{'Dichte [g/dm³]'}", "", "", true)]
    [ACPropertyEntity(30, "DensityAmb", "en{'Ambient Density'}de{'Umgebungsdichte'}", "", "", true)]
    [ACPropertyEntity(31, "MDFacilityVehicleType", "en{'Vehicle Type'}de{'Fahrzeugtyp'}", Const.ContextDatabase + "\\MDFacilityVehicleType" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(32, "MinStockQuantity", ConstApp.MinStockQuantity, "", "", true)]
    [ACPropertyEntity(33, "OptStockQuantity", ConstApp.OptStockQuantity, "", "", true)]
    [ACPropertyEntity(34, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(35, "DisabledForMobile", "en{'Disabled for mobile application'}de{'Deaktiviert für mobile Anwendung'}")]
    [ACPropertyEntity(36, nameof(SkipPrintQuestion), "en{'Skip print question'}de{'Druckfrage überspringen'}")]
    [ACPropertyEntity(36, "OrderPostingOnEmptying", "en{'Post remaining quantity to order on emptying'}de{'Restmenge bei Entleerung in Auftrag buchen'}", "", "", true)]
    [ACPropertyEntity(37, "PostingBehaviourIndex", "en{'Posting behaviour'}de{'Buchungsverhalten'}", typeof(PostingBehaviourEnum), Const.ContextDatabase + "\\PostingBehaviourEnumList", "", true)]
    [ACPropertyEntity(38, "ClassCode", "en{'Classification code'}de{'Klassifizierungscode'}", "", "", true)]
    [ACPropertyEntity(39, nameof(LeaveMaterialOccupation), ConstApp.LeaveMaterialOccupation, "", "", true)]
    [ACPropertyEntity(9999, "LastFCSortNo", "en{'Charge Sort No.'}de{'Charge Sortiernr.'}", "", "", true)]
    [ACPropertyEntity(9999, "LastFCSortNoReverse", "en{'Charge Sort No.'}de{'Charge Sortiernr2.'}", "", "", true)]
    [ACPropertyEntity(38, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + Facility.ClassName, ConstApp.Facility, typeof(Facility), Facility.ClassName, "FacilityNo", "FacilityNo")] // TODO: Define Child-Entites for Import/Export
    [ACSerializeableInfo(new Type[] { typeof(ACRef<Facility>) })]
    [NotMapped]
    public partial class Facility : ICloneable
    {
        [NotMapped]
        public const string ClassName = "Facility";

        #region New/Delete
        public static Facility NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            Facility entity = new Facility();
            entity.FacilityID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is Facility)
            {
                entity.Facility1_ParentFacility = parentACObject as Facility;
            }
            entity.OrderPostingOnEmptying = false;
            entity.DisabledForMobile = false;
            //entity.MDUnit = MDUnit.DefaultMDWeightUnit(database);
            // AClass darf Facility-Entität nicht kennen
            //ACClass.NewACObjectForFacility(database, entity);
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        /// <summary>
        /// Deletes this entity-object from the database
        /// </summary>
        /// <param name="database">Entity-Framework databasecontext</param>
        /// <param name="withCheck">If set to true, a validation happens before deleting this EF-object. If Validation fails message ís returned.</param>
        /// <param name="softDelete">If set to true a delete-Flag is set in the dabase-table instead of a physical deletion. If  a delete-Flag doesn't exit in the table the record will be deleted.</param>
        /// <returns>If a validation or deletion failed a message is returned. NULL if sucessful.</returns>
        public override MsgWithDetails DeleteACObject(IACEntityObjectContext database, bool withCheck, bool softDelete = false)
        {
            if (withCheck)
            {
                MsgWithDetails msg = IsEnabledDeleteACObject(database);
                if (msg != null)
                    return msg;
            }

            // Löschen
            try
            {
                //database.ExecuteStoreCommand("DELETE FROM FacilityBookingCharge WHERE InwardFacilityID={0}", this.FacilityID);
                foreach (FacilityBookingCharge fbc in this.FacilityBookingCharge_InwardFacility.ToArray())
                {
                    MsgWithDetails msg = fbc.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.FacilityBookingCharge_InwardFacility.Clear();

                //database.ExecuteStoreCommand("DELETE FROM FacilityBookingCharge WHERE InwardFacilityLocationID={0}", this.FacilityID);
                foreach (FacilityBookingCharge fbc in this.FacilityBookingCharge_InwardFacilityLocation.ToArray())
                {
                    MsgWithDetails msg = fbc.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.FacilityBookingCharge_InwardFacilityLocation.Clear();

                //database.ExecuteStoreCommand("DELETE FROM FacilityBookingCharge WHERE OutwardFacilityID={0}", this.FacilityID);
                foreach (FacilityBookingCharge fbc in this.FacilityBookingCharge_OutwardFacility.ToArray())
                {
                    MsgWithDetails msg = fbc.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.FacilityBookingCharge_OutwardFacility.Clear();

                //database.ExecuteStoreCommand("DELETE FROM FacilityBookingCharge WHERE OutwardFacilityLocationID={0}", this.FacilityID);
                foreach (FacilityBookingCharge fbc in this.FacilityBookingCharge_OutwardFacilityLocation.ToArray())
                {
                    MsgWithDetails msg = fbc.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.FacilityBookingCharge_OutwardFacilityLocation.Clear();

                //database.ExecuteStoreCommand("DELETE FROM FacilityBooking WHERE InwardFacilityID={0}", this.FacilityID);
                foreach (FacilityBooking fb in this.FacilityBooking_InwardFacility.ToArray())
                {
                    MsgWithDetails msg = fb.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.FacilityBooking_InwardFacility.Clear();

                //database.ExecuteStoreCommand("DELETE FROM FacilityBooking WHERE InwardFacilityLocationID={0}", this.FacilityID);
                foreach (FacilityBooking fb in this.FacilityBooking_InwardFacilityLocation.ToArray())
                {
                    MsgWithDetails msg = fb.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.FacilityBooking_InwardFacilityLocation.Clear();

                //database.ExecuteStoreCommand("DELETE FROM FacilityBooking WHERE OutwardFacilityID={0}", this.FacilityID);
                foreach (FacilityBooking fb in this.FacilityBooking_OutwardFacility.ToArray())
                {
                    MsgWithDetails msg = fb.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.FacilityBooking_OutwardFacility.Clear();

                //database.ExecuteStoreCommand("DELETE FROM FacilityBooking WHERE OutwardFacilityLocationID={0}", this.FacilityID);
                foreach (FacilityBooking fb in this.FacilityBooking_OutwardFacilityLocation.ToArray())
                {
                    MsgWithDetails msg = fb.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.FacilityBooking_OutwardFacilityLocation.Clear();

                //database.ExecuteStoreCommand("DELETE FROM FacilityReservation WHERE FacilityID={0}", this.FacilityID);
                foreach (FacilityReservation fr in this.FacilityReservation_Facility.ToArray())
                {
                    MsgWithDetails msg = fr.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.FacilityReservation_Facility.Clear();

                //database.ExecuteStoreCommand("DELETE FROM FacilityCharge WHERE FacilityID={0}", this.FacilityID);
                foreach (FacilityCharge fc in this.FacilityCharge_Facility.ToArray())
                {
                    MsgWithDetails msg = fc.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.FacilityCharge_Facility.Clear();

                //database.ExecuteStoreCommand("DELETE FROM FacilityStock WHERE FacilityID={0}", this.FacilityID);
                foreach (FacilityStock fs in this.FacilityStock_Facility.ToArray())
                {
                    MsgWithDetails msg = fs.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.FacilityStock_Facility.Clear();
            }
            catch (Exception e)
            {
                MsgWithDetails msg = new MsgWithDetails { Source = "Facility", MessageLevel = eMsgLevel.Error, ACIdentifier = "DeleteACObject", Message = Database.Root.Environment.TranslateMessage(Database.GlobalDatabase, "Error00035") };
                ACObjectContextHelper.ParseExceptionStatic(msg, e);
                return msg;
            }

            // Referenzen auflösen:
            //database.ExecuteStoreCommand("UPDATE Facility SET IncomingFacilityID = NULL WHERE IncomingFacilityID={0}", this.FacilityID);
            foreach (Facility fc in this.Facility_IncomingFacility.ToArray())
            {
                fc.Facility1_IncomingFacility = null;
            }
            this.Facility_IncomingFacility.Clear();

            //database.ExecuteStoreCommand("UPDATE Facility SET LockedFacilityID = NULL WHERE LockedFacilityID={0}", this.FacilityID);
            foreach (Facility fc in this.Facility_LockedFacility.ToArray())
            {
                fc.Facility1_LockedFacility = null;
            }
            this.Facility_LockedFacility.Clear();

            //database.ExecuteStoreCommand("UPDATE Facility SET OutgoingFacilityID = NULL WHERE OutgoingFacilityID={0}", this.FacilityID);
            foreach (Facility fc in this.Facility_OutgoingFacility.ToArray())
            {
                fc.Facility1_OutgoingFacility = null;
            }
            this.Facility_OutgoingFacility.Clear();

            //database.ExecuteStoreCommand("UPDATE Facility SET ParentFacilityID = NULL WHERE ParentFacilityID={0}", this.FacilityID);
            foreach (Facility fc in this.Facility_ParentFacility.ToArray())
            {
                fc.Facility1_ParentFacility = null;
            }
            this.Facility_ParentFacility.Clear();

            //database.ExecuteStoreCommand("UPDATE Material SET InFacilityID = NULL WHERE InFacilityID={0}", this.FacilityID);
            foreach (Material mat in this.Material_InFacility.ToArray())
            {
                mat.InFacility = null;
            }
            this.Material_InFacility.Clear();

            //database.ExecuteStoreCommand("UPDATE Material SET OutFacilityID = NULL WHERE OutFacilityID={0}", this.FacilityID);
            foreach (Material mat in this.Material_OutFacility.ToArray())
            {
                mat.OutFacility = null;
            }
            this.Material_OutFacility.Clear();

            //database.ExecuteStoreCommand("UPDATE PickingPos SET FromFacilityID = NULL WHERE FromFacilityID={0}", this.FacilityID);
            foreach (PickingPos pp in this.PickingPos_FromFacility.ToArray())
            {
                pp.FromFacility = null;
            }
            this.PickingPos_FromFacility.Clear();

            //database.ExecuteStoreCommand("UPDATE PickingPos SET ToFacilityID = NULL WHERE ToFacilityID={0}", this.FacilityID);
            foreach (PickingPos pp in this.PickingPos_ToFacility.ToArray())
            {
                pp.ToFacility = null;
            }
            this.PickingPos_ToFacility.Clear();

            //database.ExecuteStoreCommand("UPDATE Tourplan SET TrailerFacilityID = NULL WHERE TrailerFacilityID={0}", this.FacilityID);
            foreach (Tourplan tp in this.Tourplan_TrailerFacility.ToArray())
            {
                tp.TrailerFacility = null;
            }
            this.Tourplan_TrailerFacility.Clear();

            //database.ExecuteStoreCommand("UPDATE Tourplan SET VehicleFacilityID = NULL WHERE VehicleFacilityID={0}", this.FacilityID);
            foreach (Tourplan tp in this.Tourplan_VehicleFacility.ToArray())
            {
                tp.VehicleFacility = null;
            }
            this.Tourplan_VehicleFacility.Clear();

            //database.ExecuteStoreCommand("UPDATE Visitor SET TrailerFacilityID = NULL WHERE TrailerFacilityID={0}", this.FacilityID);
            foreach (Visitor tp in this.Visitor_TrailerFacility.ToArray())
            {
                tp.TrailerFacility = null;
            }
            this.Visitor_TrailerFacility.Clear();

            //database.ExecuteStoreCommand("UPDATE Visitor SET VehicleFacilityID = NULL WHERE VehicleFacilityID={0}", this.FacilityID);
            foreach (Visitor tp in this.Visitor_VehicleFacility.ToArray())
            {
                tp.VehicleFacility = null;
            }
            this.Visitor_VehicleFacility.Clear();

            //database.ExecuteStoreCommand("UPDATE VisitorVoucher SET TrailerFacilityID = NULL WHERE TrailerFacilityID={0}", this.FacilityID);
            foreach (VisitorVoucher tp in this.VisitorVoucher_TrailerFacility.ToArray())
            {
                tp.TrailerFacility = null;
            }
            this.VisitorVoucher_TrailerFacility.Clear();

            //database.ExecuteStoreCommand("UPDATE VisitorVoucher SET VehicleFacilityID = NULL WHERE VehicleFacilityID={0}", this.FacilityID);
            foreach (VisitorVoucher tp in this.VisitorVoucher_VehicleFacility.ToArray())
            {
                tp.VehicleFacility = null;
            }
            this.VisitorVoucher_VehicleFacility.Clear();

            database.Remove(this);

            return null;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            return FacilityNo;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return FacilityNo + " " + FacilityName;
            }
        }

        /// <summary>
        /// Returns a related EF-Object which is in a Child-Relationship to this.
        /// </summary>
        /// <param name="className">Classname of the Table/EF-Object</param>
        /// <param name="filterValues">Search-Parameters</param>
        /// <returns>A Entity-Object as IACObject</returns>
        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            // TODO: Define Child-Entites for Import/Export
            if (filterValues.Any() && className == FacilityStock.ClassName)
            {
                return this.FacilityStock_Facility.Where(c => c.Facility.FacilityNo == filterValues[0]).FirstOrDefault();
            }
            return null;
        }

        #endregion

        #region IACObjectEntity Members
        /// <summary>
        /// Method for validating values and references in this EF-Object.
        /// Is called from Change-Tracking before changes will be saved for new unsaved entity-objects.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">Entity-Framework databasecontext</param>
        /// <returns>NULL if sucessful otherwise a Message-List</returns>
        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            if (string.IsNullOrEmpty(FacilityNo))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "FacilityNo",
                    Message = "FacilityNo is null",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "FacilityName"), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            else if (string.IsNullOrEmpty(FacilityName))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "FacilityName",
                    Message = "FacilityName is null",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "FacilityName"), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }

        [NotMapped]
        private bool _ContextEventSubscr = false;
        /// <summary>
        /// Method for validating values and references in this EF-Object.
        /// Is called from Change-Tracking before changes will be saved for changed entity-objects.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">Entity-Framework databasecontext</param>
        /// <returns>NULL if sucessful otherwise a Message-List</returns>
        public override IList<Msg> EntityCheckModified(string user, IACEntityObjectContext context)
        {
            var baseResult = base.EntityCheckModified(user, context);
            if (!_ContextEventSubscr)
            {
                context.ACChangesExecuted += new ACChangesEventHandler(OnContextACChangesExecuted);
                _ContextEventSubscr = true;
            }
            return baseResult;
        }

        [NotMapped]
        public const string MN_RefreshFacility = "RefreshFacility";

        void OnContextACChangesExecuted(object sender, ACChangesEventArgs e)
        {
            IACEntityObjectContext context = this.GetObjectContext();
            if (context != null && context.PreventOnContextACChangesExecuted)
            {
                return;
            }

            if (e.Succeeded && e.ChangeType == ACChangesEventArgs.ACChangesType.ACSaveChanges)
            {
                CallRefreshFacility(false, null);
            }
            (sender as IACEntityObjectContext).ACChangesExecuted -= new ACChangesEventHandler(OnContextACChangesExecuted);
            _ContextEventSubscr = false;
        }

        public void CallRefreshFacility(bool preventBroadcast, Guid? fbID)
        {
            CallMethodOnInstance(MN_RefreshFacility, preventBroadcast, fbID);
        }

        [NotMapped]
        public const string MN_SendPicking = "SendPicking";
        public void CallSendPicking(bool preventBroadcast, Guid? keyID)
        {
            CallMethodOnInstance(MN_SendPicking, preventBroadcast, keyID);
        }

        public void CallMethodOnInstance(string methodName, bool preventBroadcast, Guid? keyID)
        {
            // Aktualisiere Facility-ACComponent
            gip.core.datamodel.ACClass facilityACClass = FacilityACClass;
            if (facilityACClass == null)
                return;
            string acUrl = facilityACClass.GetACUrlComponent();
            if (String.IsNullOrEmpty(acUrl))
                return;
            ApplicationManager appManager = null;
            List<string> parents = ACUrlHelper.ResolveParents(acUrl);
            if (parents != null && parents.Any())
                appManager = Database.Root.ACUrlCommand(parents.First()) as ApplicationManager;

            if (appManager != null && appManager.ApplicationQueue != null)
            {
                appManager.ApplicationQueue.Add(() => { Database.Root.ACUrlCommand(acUrl + ACUrlHelper.Delimiter_InvokeMethod + methodName, preventBroadcast, keyID); });
            }
            else
            {
                ThreadPool.QueueUserWorkItem((object state) =>
                {
                    Database.Root.ACUrlCommand(acUrl + ACUrlHelper.Delimiter_InvokeMethod + methodName, preventBroadcast, keyID);
                });
            }
        }

        public FacilityStock GetFacilityStock_InsertIfNotExists(DatabaseApp dbApp)
        {
            FacilityStock facilityStock = FacilityStock_Facility.FirstOrDefault();
            if (facilityStock != null)
                return facilityStock;
            facilityStock = FacilityStock.NewACObject(dbApp, this);
            return facilityStock;
        }

        public int GetNextFCSortNo(DatabaseApp dbApp)
        {
            return this.LastFCSortNo++;
        }

        public int GetNextFCSortNoReverse(DatabaseApp dbApp)
        {
            return this.LastFCSortNoReverse--;
        }

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "FacilityNo";
            }
        }
        #endregion

        #region IEntityProperty Members

        [NotMapped]
        bool bRefreshConfig = false;
        protected override void OnPropertyChanging<T>(T newValue, string propertyName, bool afterChange)
        {
            if (propertyName == nameof(XMLConfig))
            {
                string xmlConfig = newValue as string;
                if (afterChange)
                {
                    if (bRefreshConfig)
                        ACProperties.Refresh();
                }
                else
                {
                    bRefreshConfig = false;
                    if (this.EntityState != EntityState.Detached && (!(String.IsNullOrEmpty(xmlConfig) && String.IsNullOrEmpty(XMLConfig)) && xmlConfig != XMLConfig))
                        bRefreshConfig = true;
                }
            }
            base.OnPropertyChanging(newValue, propertyName, afterChange);
        }

        /// <summary>
        /// There are no comments for Property CurrentFacilityStock in the schema.
        /// </summary>
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(9999, "", ConstApp.NotAvailable)]
        [NotMapped]
        public bool NotAvailable
        {
            get
            {
                if (!this.FacilityCharge_Facility.Any())
                    return true;
                try
                {
                    return !FacilityCharge_Facility.Where(c => !c.NotAvailable).Any();
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException("Facility", "NotAvailable", msg);

                    return true;
                }
            }
        }

        [NotMapped]
        private FacilityStock _CurrentFacilityStock;
        [NotMapped]
        private bool _CurrentFacilityStockLoaded;
        /// <summary>
        /// There are no comments for Property CurrentFacilityStock in the schema.
        /// </summary>
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(9999, "", "en{'Current facility stock'} de{'Aktueller Lagerbestand'}")]
        [NotMapped]
        public FacilityStock CurrentFacilityStock
        {
            get
            {
                if (!_CurrentFacilityStockLoaded)
                {
                    _CurrentFacilityStock = this.FacilityStock_Facility.FirstOrDefault();
                    if (_CurrentFacilityStock != null)
                        _CurrentFacilityStockLoaded = true;
                }
                return _CurrentFacilityStock;
            }
            set
            {
                _CurrentFacilityStock = value;
                _CurrentFacilityStockLoaded = true;
            }
        }
        #endregion

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(VBiFacilityACClassID):
                    base.OnPropertyChanged("FacilityACClass");
                    break;
                case nameof(VBiStackCalculatorACClassID):
                    base.OnPropertyChanged("StackCalculatorACClass");
                    break;
            }
            base.OnPropertyChanged(propertyName);
        }

        #region VBIplus-Context
        [NotMapped]
        private gip.core.datamodel.ACClass _FacilityACClass;
        [ACPropertyInfo(9999, "", "en{'Module'}de{'Modul'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClass.ClassName + Const.DBSetAsEnumerablePostfix)]
        [NotMapped]
        public gip.core.datamodel.ACClass FacilityACClass
        {
            get
            {
                if (this.VBiFacilityACClassID == null || this.VBiFacilityACClassID == Guid.Empty)
                    return null;
                if (_FacilityACClass != null)
                    return _FacilityACClass;
                if (this.VBiFacilityACClass == null)
                {
                    DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();

                    using (ACMonitor.Lock(dbApp.ContextIPlus.QueryLock_1X000))
                    {
                        _FacilityACClass = dbApp.ContextIPlus.ACClass.Where(c => c.ACClassID == this.VBiFacilityACClassID).FirstOrDefault();
                    }
                    return _FacilityACClass;
                }
                else
                {
                    _FacilityACClass = this.VBiFacilityACClass.FromIPlusContext<gip.core.datamodel.ACClass>();
                    return _FacilityACClass;
                }
            }
            set
            {
                if (value == null)
                {
                    if (this.VBiFacilityACClass == null)
                        return;
                    _FacilityACClass = null;
                    this.VBiFacilityACClass = null;
                }
                else
                {
                    if (_FacilityACClass != null && value == _FacilityACClass)
                        return;
                    gip.mes.datamodel.ACClass value2 = value.FromAppContext<gip.mes.datamodel.ACClass>(this.GetObjectContext<DatabaseApp>());
                    // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                    if (value2 == null)
                    {
                        this.VBiFacilityACClassID = value.ACClassID;
                        throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                        //return;
                    }
                    _FacilityACClass = value;
                    if (value2 == this.VBiFacilityACClass)
                        return;
                    this.VBiFacilityACClass = value2;
                }
            }
        }

        public gip.core.datamodel.ACClass GetFacilityACClass(Database db)
        {
            if (this.VBiFacilityACClassID == null || this.VBiFacilityACClassID == Guid.Empty)
                return null;
            if (this.VBiFacilityACClass == null)
            {

                using (ACMonitor.Lock(db.QueryLock_1X000))
                {
                    return db.ACClass.Where(c => c.ACClassID == this.VBiFacilityACClassID).FirstOrDefault();
                }
            }
            else
                return this.VBiFacilityACClass.FromIPlusContext<gip.core.datamodel.ACClass>(db);
        }

        [NotMapped]
        public bool IsMirroredOnMoreDatabases
        {
            get
            {
                if (!VBiFacilityACClassID.HasValue
                    || FacilityACClass == null
                    || FacilityACClass.ObjectType == null)
                    return false;
                return FacilityACClass.ObjectType.FullName.Contains("RemoteStore");
            }
        }

        [NotMapped]
        private gip.core.datamodel.ACClass _StackCalculatorACClass;
        [ACPropertyInfo(9999, "", "en{'Stack Posting Type'}de{'Stapelbuchungsart'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClass.ClassName + Const.DBSetAsEnumerablePostfix)]
        [NotMapped]
        public gip.core.datamodel.ACClass StackCalculatorACClass
        {
            get
            {
                if (this.VBiStackCalculatorACClassID == null || this.VBiStackCalculatorACClassID == Guid.Empty)
                    return null;
                if (_StackCalculatorACClass != null)
                    return _StackCalculatorACClass;
                if (this.VBiStackCalculatorACClass == null)
                {
                    DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                    _StackCalculatorACClass = dbApp.ContextIPlus.ACClass.Where(c => c.ACClassID == this.VBiStackCalculatorACClassID).FirstOrDefault();
                    return _StackCalculatorACClass;
                }
                else
                {
                    _StackCalculatorACClass = this.VBiStackCalculatorACClass.FromIPlusContext<gip.core.datamodel.ACClass>();
                    return _StackCalculatorACClass;
                }
            }
            set
            {
                if (value == null)
                {
                    if (this.VBiStackCalculatorACClass == null)
                        return;
                    _StackCalculatorACClass = null;
                    this.VBiStackCalculatorACClass = null;
                }
                else
                {
                    if (_StackCalculatorACClass != null && value == _StackCalculatorACClass)
                        return;
                    gip.mes.datamodel.ACClass value2 = value.FromAppContext<gip.mes.datamodel.ACClass>(this.GetObjectContext<DatabaseApp>());
                    // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                    if (value2 == null)
                    {
                        this.VBiStackCalculatorACClassID = value.ACClassID;
                        throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                        //return;
                    }
                    _StackCalculatorACClass = value;
                    if (value2 == this.VBiStackCalculatorACClass)
                        return;
                    this.VBiStackCalculatorACClass = value2;
                }
            }
        }

        public FacilityFillValidation FillValidationMode()
        {
            FacilityFillValidation fl = FacilityFillValidation.None;
            try
            {
                fl = (FacilityFillValidation)this.HighLidNo;
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (gip.core.datamodel.Database.Root != null && gip.core.datamodel.Database.Root.Messages != null &&
                                                                      gip.core.datamodel.Database.Root.InitState == gip.core.datamodel.ACInitState.Initialized)
                    gip.core.datamodel.Database.Root.Messages.LogException("FacilityExt", "FillValidationMode", msg);
            }
            return fl;
        }

        #endregion

        #region Addititional Properties
        [ACPropertyInfo(36, "", "en{'First Quant'}de{'Erstes Quant'}")]
        [NotMapped]
        public FacilityCharge FirstQuant
        {
            get
            {
                return this.FacilityCharge_Facility.Where(c => c.NotAvailable == false).OrderBy(c => c.FacilityChargeSortNo).FirstOrDefault();
            }
        }
        [ACPropertyInfo(37, "", "en{'Last Quant'}de{'Letztes Quant'}")]
        [NotMapped]
        public FacilityCharge LastQuant
        {
            get
            {
                return this.FacilityCharge_Facility.Where(c => c.NotAvailable == false).OrderByDescending(c => c.FacilityChargeSortNo).FirstOrDefault();
            }
        }

        [ACPropertyInfo(38, "", "en{'First Expired Quant'}de{'Erstes abgelaufenes Quant'}")]
        [NotMapped]
        public FacilityCharge FirstExpiredQuant
        {
            get
            {
                return this.FacilityCharge_Facility.Where(c => c.NotAvailable == false && c.ExpirationDate.HasValue).OrderBy(c => c.ExpirationDate).FirstOrDefault();
            }
        }
        [ACPropertyInfo(39, "", "en{'Last Expired Quant'}de{'Letztes abgelaufenes Quant'}")]
        [NotMapped]
        public FacilityCharge LastExpiredQuant
        {
            get
            {
                return this.FacilityCharge_Facility.Where(c => c.NotAvailable == false && c.ExpirationDate.HasValue).OrderByDescending(c => c.ExpirationDate).FirstOrDefault();
            }
        }

        [ACPropertyInfo(40, "", "en{'First Storage Date'}de{'Erstes Einlagerdatum'}")]
        [NotMapped]
        public DateTime? FirstQuantDate
        {
            get
            {
                if (FirstQuant != null)
                    return FirstQuant.InsertDate;
                return null;
            }
        }
        [ACPropertyInfo(41, "", "en{'Last Inward Date'}de{'Letztes Einlagerdatum'}")]
        [NotMapped]
        public DateTime? LastQuantDate
        {
            get
            {
                if (LastQuant != null)
                    return LastQuant.InsertDate;
                return null;
            }
        }

        [ACPropertyInfo(42, "", "en{'First Expiration Date'}de{'Ältestes MHD Ablaufdatum'}")]
        [NotMapped]
        public DateTime? FirstExpirationQuantDate
        {
            get
            {
                if (FirstExpiredQuant != null)
                    return FirstExpiredQuant.ExpirationDate;
                return null;
            }
        }

        [ACPropertyInfo(43, "", "en{'Last Expiration Date'}de{'Letztes MHD Ablaufdatum'}")]
        [NotMapped]
        public DateTime? LastExpirationQuantDate
        {
            get
            {
                if (LastExpiredQuant != null)
                    return LastExpiredQuant.ExpirationDate;
                return null;
            }
        }

        [ACPropertyInfo(44, "", "en{'Has blocked Quant'}de{'Hat gesperrte Quants'}")]
        [NotMapped]
        public bool HasBlockedQuant
        {
            get
            {
                return this.FacilityCharge_Facility.Where(c => c.NotAvailable == false
                                                                && (c.MDReleaseStateID.HasValue && c.MDReleaseState.MDReleaseStateIndex >= (short)MDReleaseState.ReleaseStates.Locked)
                                                                    || (c.FacilityLotID.HasValue && c.FacilityLot.MDReleaseStateID.HasValue && c.FacilityLot.MDReleaseState.MDReleaseStateIndex >= (short)MDReleaseState.ReleaseStates.Locked))
                                                   .Any();
            }
        }

        [NotMapped]
        public IQueryable<FacilityCharge> QryHasBlockedQuants
        {
            get
            {
                return Context.Entry(this).Collection(c => c.FacilityCharge_Facility).Query().Where(c => c.NotAvailable == false
                                                                && (c.MDReleaseStateID.HasValue && c.MDReleaseState.MDReleaseStateIndex >= (short)MDReleaseState.ReleaseStates.Locked)
                                                                    || (c.FacilityLotID.HasValue && c.FacilityLot.MDReleaseStateID.HasValue && c.FacilityLot.MDReleaseState.MDReleaseStateIndex >= (short)MDReleaseState.ReleaseStates.Locked));
            }
        }

        [NotMapped]
        public static Func<FacilityCharge, bool> FuncHasBlockedQuants =
            c => c.NotAvailable == false
               && (c.MDReleaseStateID.HasValue && c.MDReleaseState.MDReleaseStateIndex >= (short)MDReleaseState.ReleaseStates.Locked)
                   || (c.FacilityLotID.HasValue && c.FacilityLot.MDReleaseStateID.HasValue && c.FacilityLot.MDReleaseState.MDReleaseStateIndex >= (short)MDReleaseState.ReleaseStates.Locked);


        [NotMapped]
        public IQueryable<FacilityCharge> QryHasFreeQuants
        {
            get
            {
                return Context.Entry(this).Collection(c => c.FacilityCharge_Facility).Query()
                                                    .Where(c => c.NotAvailable == false
                                                            && (!c.MDReleaseStateID.HasValue || c.MDReleaseState.MDReleaseStateIndex <= (short)MDReleaseState.ReleaseStates.AbsFree)
                                                            && (!c.FacilityLotID.HasValue || !c.FacilityLot.MDReleaseStateID.HasValue || c.FacilityLot.MDReleaseState.MDReleaseStateIndex <= (short)MDReleaseState.ReleaseStates.AbsFree));
            }
        }

        [NotMapped]
        public static Func<FacilityCharge, bool> FuncHasFreeQuants =
            c => c.NotAvailable == false
                && (!c.MDReleaseStateID.HasValue || c.MDReleaseState.MDReleaseStateIndex <= (short)MDReleaseState.ReleaseStates.AbsFree)
                && (!c.FacilityLotID.HasValue || !c.FacilityLot.MDReleaseStateID.HasValue || c.FacilityLot.MDReleaseState.MDReleaseStateIndex <= (short)MDReleaseState.ReleaseStates.AbsFree);

        [ACPropertyInfo(45, "", "en{'Available Space'}de{'Verfügbarer Platz'}")]
        [NotMapped]
        public double AvailableSpace
        {
            get
            {
                if (CurrentFacilityStock != null)
                    return MaxWeightCapacity - CurrentFacilityStock.StockQuantity;
                return MaxWeightCapacity;
            }
        }

        public Facility GetFirstParentOfType(FacilityTypesEnum type)
        {
            Facility parent = this;
            while (parent != null)
            {
                if (parent.MDFacilityType == null)
                    return null;
                if (parent.MDFacilityType.FacilityType == type)
                    return parent;
                parent = parent.Facility1_ParentFacility;
            }
            return null;
        }

        [ACPropertyInfo(37, "", "en{'Posting behaviour'}de{'Buchungsverhalten'}", Const.ContextDatabase + "\\PostingBehaviourEnumList", true)]
        [NotMapped]
        public PostingBehaviourEnum PostingBehaviour
        {
            get
            {
                return (PostingBehaviourEnum)PostingBehaviourIndex;
            }
            set
            {
                PostingBehaviourIndex = (short)value;
            }
        }


        private bool? _ShouldLeaveMaterialOccupation;
        /// <summary>
        /// Check if LeaveMaterialOccupation is there in Facility or any parent Facility defined and return value
        /// if not returns default value (false)
        /// </summary>
        public bool ShouldLeaveMaterialOccupation
        {
            get
            {
                if(_ShouldLeaveMaterialOccupation == null)
                {
                    Facility facility = this;
                    while(facility != null)
                    {
                        if(facility.LeaveMaterialOccupation != null)
                        {
                            _ShouldLeaveMaterialOccupation = facility.LeaveMaterialOccupation.Value;
                            break;
                        }
                        facility = facility.Facility1_ParentFacility;
                    }

                    if(_ShouldLeaveMaterialOccupation == null)
                    {
                        _ShouldLeaveMaterialOccupation = false;
                    }
                }
                return _ShouldLeaveMaterialOccupation ?? false;
            }
        }

        #endregion

        /// <summary>
        /// Check if is facility located under given facility. Check max 5 levels up.
        /// </summary>
        /// <param name="parentFacility"></param>
        /// <returns></returns>
        public bool IsLocatedIn(Guid parentFacilityID)
        {
            return this.FacilityID == parentFacilityID

                      || (this.ParentFacilityID.HasValue && this.ParentFacilityID == parentFacilityID) //1. Level

                      || (this.Facility1_ParentFacility != null                                         //2. Level
                          && this.Facility1_ParentFacility.ParentFacilityID.HasValue
                          && this.Facility1_ParentFacility.ParentFacilityID == parentFacilityID)

                      || (this.Facility1_ParentFacility != null
                          && this.Facility1_ParentFacility.Facility1_ParentFacility != null                //3.Level
                          && this.Facility1_ParentFacility.Facility1_ParentFacility.ParentFacilityID.HasValue
                          && this.Facility1_ParentFacility.Facility1_ParentFacility.ParentFacilityID == parentFacilityID)

                      || (this.Facility1_ParentFacility != null
                          && this.Facility1_ParentFacility.Facility1_ParentFacility != null
                          && this.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility != null //4.Level
                          && this.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility.ParentFacilityID.HasValue
                          && this.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility.ParentFacilityID == parentFacilityID)

                      || (this.Facility1_ParentFacility != null
                          && this.Facility1_ParentFacility.Facility1_ParentFacility != null
                          && this.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility != null //5.Level
                          && this.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility != null
                          && this.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility.ParentFacilityID.HasValue
                          && this.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility.ParentFacilityID == parentFacilityID);

        }

        public object Clone()
        {
            Facility clonedObject = new Facility();
            clonedObject.FacilityID = this.FacilityID;
            clonedObject.CopyFrom(this, true);
            return clonedObject;
        }

        public void CopyFrom(Facility from, bool withReferences)
        {
            if (withReferences)
            {
                ParentFacilityID = from.ParentFacilityID;
                VBiFacilityACClassID = from.VBiFacilityACClassID;
                MDFacilityTypeID = from.MDFacilityTypeID;
                VBiStackCalculatorACClassID = from.VBiStackCalculatorACClassID;
                PartslistID = from.PartslistID;
                LockedFacilityID = from.LockedFacilityID;
                OutgoingFacilityID = from.OutgoingFacilityID;
                IncomingFacilityID = from.IncomingFacilityID;
                CompanyID = from.CompanyID;
                CompanyPersonID = from.CompanyPersonID;
                MDFacilityVehicleTypeID = from.MDFacilityVehicleTypeID;
            }

            FacilityNo = from.FacilityNo;
            FacilityName = from.FacilityName;
            MaterialID = from.MaterialID;
            MDUnitID = from.MDUnitID;
            InwardEnabled = from.InwardEnabled;
            OutwardEnabled = from.OutwardEnabled;
            LastFCSortNo = from.LastFCSortNo;
            LastFCSortNoReverse = from.LastFCSortNoReverse;
            ReservedQuantity = from.ReservedQuantity;
            OrderedQuantity = from.OrderedQuantity;
            Comment = from.Comment;
            XMLConfig = from.XMLConfig;
            Tara = from.Tara;
            MaxWeightCapacity = from.MaxWeightCapacity;
            MaxVolumeCapacity = from.MaxVolumeCapacity;
            Drivername = from.Drivername;
            Tolerance = from.Tolerance;
            HighLidNo = from.HighLidNo;
            FittingsDistanceFront = from.FittingsDistanceFront;
            FittingsDistanceBehind = from.FittingsDistanceBehind;
            DistanceFront = from.DistanceFront;
            DistanceBehind = from.DistanceBehind;
            InsertName = from.InsertName;
            InsertDate = from.InsertDate;
            UpdateName = from.UpdateName;
            UpdateDate = from.UpdateDate;
            Density = from.Density;
            DensityAmb = from.DensityAmb;
            MinStockQuantity = from.MinStockQuantity;
            OptStockQuantity = from.OptStockQuantity;
            OrderPostingOnEmptying = from.OrderPostingOnEmptying;
            DisabledForMobile = from.DisabledForMobile;
            KeyOfExtSys = from.KeyOfExtSys;
            PostingBehaviourIndex = from.PostingBehaviourIndex;
        }

        //public static readonly Func<DatabaseApp, Facility, Guid, bool> s_cQry_IsLocatedIn =
        //    EF.CompileQuery<DatabaseApp, Facility, Guid, bool>((ctx, facility, parentFacilityID) => 
        //                  facility.FacilityID == parentFacilityID
        //              || (facility.ParentFacilityID.HasValue && facility.ParentFacilityID == parentFacilityID) //1. Level
        //              || (   facility.Facility1_ParentFacility != null                                         //2. Level
        //                  && facility.Facility1_ParentFacility.ParentFacilityID.HasValue 
        //                  && facility.Facility1_ParentFacility.ParentFacilityID == parentFacilityID)
        //              || (   facility.Facility1_ParentFacility.Facility1_ParentFacility != null                //3.Level
        //                  && facility.Facility1_ParentFacility.Facility1_ParentFacility.ParentFacilityID.HasValue
        //                  && facility.Facility1_ParentFacility.Facility1_ParentFacility.ParentFacilityID == parentFacilityID)
        //              || (   facility.Facility1_ParentFacility.Facility1_ParentFacility != null                //4.Level
        //                  && facility.Facility1_ParentFacility.Facility1_ParentFacility.ParentFacilityID.HasValue
        //                  && facility.Facility1_ParentFacility.Facility1_ParentFacility.ParentFacilityID == parentFacilityID)
        //              || (   facility.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility != null //5.Level
        //                  && facility.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility.ParentFacilityID.HasValue
        //                  && facility.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility.ParentFacilityID == parentFacilityID)
        //              );
    }
}
