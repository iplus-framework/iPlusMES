using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace gip.mes.datamodel
{
    // FacilityReservation (ReservArtPlatz)
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.FacilityReservation, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "FacilityReservationNo", "en{'FacilityReservationNo'}de{'de-FacilityReservationNo'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, Facility.ClassName, ConstApp.Facility, Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(4, FacilityLot.ClassName, ConstApp.Lot, Const.ContextDatabase + "\\" + FacilityLot.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(5, FacilityCharge.ClassName, "en{'Charge'}de{'Charge'}", Const.ContextDatabase + "\\" + FacilityCharge.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, InOrderPos.ClassName, "en{'InOrderPos'}de{'de-InOrderPos'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(7, OutOrderPos.ClassName, "en{'OutOrderPos'}de{'de-OutOrderPos'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(8, ProdOrderPartslistPos.ClassName, "en{'Bill of Materials Pos.'}de{'Stücklistenposition'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9, ProdOrderBatchPlan.ClassName, "en{'Batchplan'}de{'Batchplan'}", Const.ContextDatabase + "\\" + ProdOrderBatchPlan.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(10, "ParentFacilityReservation", "en{'Subline from'}de{'Unterposition von'}", Const.ContextDatabase + "\\" + FacilityReservation.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(11, ProdOrderPartslistPosRelation.ClassName, "en{'Bill of Materials Pos.'}de{'Stücklistenunterposition'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPosRelation.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(12, "ReservationStateIndex", "en{'State'}de{'Status'}", typeof(GlobalApp.ReservationState), Const.ContextDatabase + "\\ReservationStateList", "", true)]
    [ACPropertyEntity(13, "Sequence", "en{'Sequence'}de{'Reihenfolge'}", "", "", true)]
    [ACPropertyEntity(14, PickingPos.ClassName, "en{'Picking Line'}de{'Kommissionierposition'}", Const.ContextDatabase + "\\" + PickingPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(15, nameof(FacilityReservation.ReservedQuantityUOM), ConstApp.ReservedQuantity, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + FacilityReservation.ClassName, "en{'Reservation'}de{'Reservierung'}", typeof(FacilityReservation), FacilityReservation.ClassName, "FacilityReservationNo", "FacilityReservationNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<FacilityReservation>) })]
    [NotMapped]
    public partial class FacilityReservation
    {
        [NotMapped]
        public const string ClassName = "FacilityReservation";
        [NotMapped]
        public const string NoColumnName = "FacilityReservationNo";
        [NotMapped]
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
            else if (parentACObject != null && parentACObject is PickingPos)
            {
                PickingPos pickingPos = parentACObject as PickingPos;
                entity.PickingPos = pickingPos;
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        public void CopyFrom(FacilityReservation from, bool withReferences)
        {
            if (withReferences)
            {
                OutOrderPosID = from.OutOrderPosID;
                InOrderPosID = from.InOrderPosID;
                ProdOrderPartslistPosID = from.ProdOrderPartslistPosID;
                ProdOrderBatchPlanID = from.ProdOrderBatchPlanID;
                ProdOrderPartslistPosRelationID = from.ProdOrderPartslistPosRelationID;
                InOrderPosID = from.InOrderPosID;
                PickingPosID = from.PickingPosID;
                MaterialID = from.MaterialID;
                FacilityLotID = from.FacilityLotID;
                FacilityChargeID = from.FacilityChargeID;
                FacilityID = from.FacilityID;
                VBiACClassID = from.VBiACClassID;
            }

            ReservedQuantityUOM = from.ReservedQuantityUOM;
            Sequence = from.Sequence;
            XMLConfig = from.XMLConfig;
            InsertName = from.InsertName;
            InsertDate = from.InsertDate;
            UpdateName = from.UpdateName;
            UpdateDate = from.UpdateDate;
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
        [NotMapped]
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
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return Facility;
            }
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "FacilityReservationNo";
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

        #endregion

        #region Additional Properties
        [NotMapped]
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


        [NotMapped]
        private gip.core.datamodel.ACClass _FacilityACClass;
        [ACPropertyInfo(9999, "", "en{'Module'}de{'Modul'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClass.ClassName + Const.DBSetAsEnumerablePostfix)]
        [NotMapped]
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

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName == nameof(VBiACClassID))
            {
                OnPropertyChanged("FacilityACClass");
            }
            base.OnPropertyChanged(propertyName);
        }

        [NotMapped]
        private Route _PredefinedRoute;
        [NotMapped]
        [ACPropertyInfo(999, "", "en{'Route'}de{'Route'}")]
        public Route PredefinedRoute
        {
            get
            {
                if (_PredefinedRoute != null)
                    return _PredefinedRoute;

                ACPropertyExt acPropertyExt = this.ACProperties.Properties.Where(x => x.ACIdentifier == Route.ClassName).FirstOrDefault();
                if (acPropertyExt != null && acPropertyExt.Value != null)
                    _PredefinedRoute = acPropertyExt.Value as Route;

                return _PredefinedRoute;
            }
            set
            {
                ACPropertyExt acPropertyExt = this.ACProperties.Properties.Where(x => x.ACIdentifier == Route.ClassName).FirstOrDefault();
                if (acPropertyExt == null)
                {
                    acPropertyExt = new ACPropertyExt();
                    acPropertyExt.ACIdentifier = Route.ClassName;
                    acPropertyExt.ObjectType = typeof(Route);
                    acPropertyExt.AttachTo(this.ACProperties);
                }

                Route savedRoute = acPropertyExt.Value as Route;
                if (savedRoute == null || !savedRoute.SequenceEqual(value))
                {
                    _PredefinedRoute = value;
                    acPropertyExt.Value = value;
                    this.ACProperties.Properties.Add(acPropertyExt);
                    this.ACProperties.Serialize();
                }
                OnPropertyChanged(nameof(PredefinedRoute));
            }
        }
        #endregion

        #region Query
        public static Func<FacilityReservation, bool> ProdOrderComponentReservations(Guid materialID, Guid lotID)
        {
            return c=>
                    !c.VBiACClassID.HasValue
                    && c.MaterialID.HasValue
                    && c.MaterialID == materialID
                    && c.FacilityLotID.HasValue
                    && c.FacilityLotID == lotID
                    && c.ProdOrderPartslistPos != null
                    && c.ProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot;
        }

        public static Func<FacilityReservation, bool> PickingPosReservations(Guid materialID, Guid lotID)
        {
            return c =>
                    !c.VBiACClassID.HasValue
                    && c.MaterialID.HasValue
                    && c.MaterialID == materialID
                    && c.FacilityLotID.HasValue
                    && c.FacilityLotID == lotID
                    && c.PickingPos != null;
        }

        public static Func<FacilityReservation, bool> OutOrderPosReservations(Guid materialID, Guid lotID)
        {
            return c =>
                    !c.VBiACClassID.HasValue
                    && c.MaterialID.HasValue
                    && c.MaterialID == materialID
                    && c.FacilityLotID.HasValue
                    && c.FacilityLotID == lotID
                    && c.OutOrderPos != null;
        }

        #endregion
    }
}
