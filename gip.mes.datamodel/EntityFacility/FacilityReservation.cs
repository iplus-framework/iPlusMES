using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    // FacilityReservation (ReservArtPlatz)
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Reservation'}de{'Reservierung'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "FacilityReservationNo", "en{'FacilityReservationNo'}de{'de-FacilityReservationNo'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(3, Facility.ClassName, ConstApp.Facility, Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(4, FacilityLot.ClassName, ConstApp.Lot, Const.ContextDatabase + "\\" + FacilityLot.ClassName, "", true)]
    [ACPropertyEntity(5, FacilityCharge.ClassName, "en{'Charge'}de{'Charge'}", Const.ContextDatabase + "\\" + FacilityCharge.ClassName, "", true)]
    [ACPropertyEntity(6, InOrderPos.ClassName, "en{'InOrderPos'}de{'de-InOrderPos'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName, "", true)]
    [ACPropertyEntity(7, OutOrderPos.ClassName, "en{'OutOrderPos'}de{'de-OutOrderPos'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName, "", true)]
    [ACPropertyEntity(8, ProdOrderPartslistPos.ClassName, "en{'Bill of Materials Pos.'}de{'Stücklistenposition'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName, "", true)]
    [ACPropertyEntity(9, ProdOrderBatchPlan.ClassName, "en{'Batchplan'}de{'Batchplan'}", Const.ContextDatabase + "\\" + ProdOrderBatchPlan.ClassName, "", true)]
    [ACPropertyEntity(10, "ParentFacilityReservation", "en{'Subline from'}de{'Unterposition von'}", Const.ContextDatabase + "\\" + FacilityReservation.ClassName, "", true)]
    [ACPropertyEntity(11, ProdOrderPartslistPosRelation.ClassName, "en{'Bill of Materials Pos.'}de{'Stücklistenunterposition'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPosRelation.ClassName, "", true)]
    [ACPropertyEntity(12, "ReservationStateIndex", "en{'State'}de{'Status'}", typeof(GlobalApp.ReservationState), Const.ContextDatabase + "\\ReservationStateList", "", true)]
    [ACPropertyEntity(13, "Sequence", "en{'Sequence'}de{'Reihenfolge'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + FacilityReservation.ClassName, "en{'Reservation'}de{'Reservierung'}", typeof(FacilityReservation), FacilityReservation.ClassName, "FacilityReservationNo", "FacilityReservationNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<FacilityReservation>) })]
    public partial class FacilityReservation
    {
        public const string ClassName = "FacilityReservation";
        public const string NoColumnName = "FacilityReservationNo";
        public const string FormatNewNo = "FR{0}";

        #region New/Delete
        public static FacilityReservation NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            FacilityReservation entity = new FacilityReservation();
            entity.FacilityReservationID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.ReservationState = GlobalApp.ReservationState.New;
            entity.FacilityReservationNo = secondaryKey;
            if (parentACObject != null && parentACObject is FacilityReservation)
            {
                FacilityReservation parentReservation = parentACObject as FacilityReservation;
                entity.FacilityReservation1_ParentFacilityReservation = parentReservation;
            }
            else if (parentACObject != null && parentACObject is ProdOrderPartslistPos)
            {
                ProdOrderPartslistPos parentPos = parentACObject as ProdOrderPartslistPos;
                entity.ProdOrderPartslistPos = parentPos;
            }
            else if (parentACObject != null && parentACObject is ProdOrderBatchPlan)
            {
                ProdOrderBatchPlan parentPos = parentACObject as ProdOrderBatchPlan;
                entity.ProdOrderBatchPlan = parentPos;
            }
            else if (parentACObject != null && parentACObject is InOrderPos)
            {
                InOrderPos parentPos = parentACObject as InOrderPos;
                entity.InOrderPos = parentPos;
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            return ACCaption;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                return FacilityReservationNo;
            }
        }

        /// <summary>
        /// Returns Facility
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Facility</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return Facility;
            }
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "FacilityReservationNo";
            }
        }
        #endregion

        #region IEntityProperty Members

        bool bRefreshConfig = false;
        partial void OnXMLConfigChanging(global::System.String value)
        {
            bRefreshConfig = false;
            if (this.EntityState != System.Data.EntityState.Detached && (!(String.IsNullOrEmpty(value) && String.IsNullOrEmpty(XMLConfig)) && value != XMLConfig))
                bRefreshConfig = true;
        }

        partial void OnXMLConfigChanged()
        {
            if (bRefreshConfig)
                ACProperties.Refresh();
        }

        #endregion

        #region Additional Properties
        public GlobalApp.ReservationState ReservationState
        {
            get
            {
                return (GlobalApp.ReservationState)ReservationStateIndex;
            }
            set
            {
                ReservationStateIndex = (Int16)value;
            }
        }


        private gip.core.datamodel.ACClass _FacilityACClass;
        [ACPropertyInfo(9999, "", "en{'Module'}de{'Modul'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClass.ClassName)]
        public gip.core.datamodel.ACClass FacilityACClass
        {
            get
            {
                if (this.VBiACClassID == null || this.VBiACClassID == Guid.Empty)
                    return null;
                if (_FacilityACClass != null)
                    return _FacilityACClass;
                if (this.VBiACClass == null)
                {
                    _FacilityACClass = GetFacilityACClass(this.GetObjectContext<DatabaseApp>().ContextIPlus);
                    return _FacilityACClass;
                }
                else
                {
                    _FacilityACClass = this.VBiACClass.FromIPlusContext<gip.core.datamodel.ACClass>();
                    return _FacilityACClass;
                }
            }
            set
            {
                if (value == null)
                {
                    if (this.VBiACClass == null)
                        return;
                    _FacilityACClass = null;
                    this.VBiACClass = null;
                }
                else
                {
                    if (_FacilityACClass != null && value == _FacilityACClass)
                        return;
                    gip.mes.datamodel.ACClass value2 = value.FromAppContext<gip.mes.datamodel.ACClass>(this.GetObjectContext<DatabaseApp>());
                    // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                    if (value2 == null)
                    {
                        this.VBiACClassID = value.ACClassID;
                        throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                        //return;
                    }
                    _FacilityACClass = value;
                    if (value2 == this.VBiACClass)
                        return;
                    this.VBiACClass = value2;
                }
            }
        }

        public gip.core.datamodel.ACClass GetFacilityACClass(Database db)
        {
            if (this.VBiACClassID == null || this.VBiACClassID == Guid.Empty)
                return null;
            if (this.VBiACClass == null)
                return db.ACClass.Where(c => c.ACClassID == this.VBiACClassID).FirstOrDefault();
            else
                return this.VBiACClass.FromIPlusContext<gip.core.datamodel.ACClass>(db);
        }

        partial void OnVBiACClassIDChanged()
        {
            OnPropertyChanged("FacilityACClass");
        }
        #endregion
    }
}
