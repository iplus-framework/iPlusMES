using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{

    // FacilityCharge (Chargenplatz)
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Batch Location'}de{'Chargenplatz'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOFacilityBookCharge")]
    [ACPropertyEntity(1, FacilityLot.ClassName, ConstApp.Lot, Const.ContextDatabase + "\\" + FacilityLot.ClassName, "", true)]
    [ACPropertyEntity(2, "SplitNo", "en{'Split No.'}de{'Splitnr'}", "", "", true)]
    [ACPropertyEntity(3, MDReleaseState.ClassName, ConstApp.ESReleaseState, Const.ContextDatabase + "\\" + MDReleaseState.ClassName, "", true)]
    [ACPropertyEntity(4, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(5, "Partslist", ConstApp.BOM, Const.ContextDatabase + "\\" + Partslist.ClassName, "", true)]
    [ACPropertyEntity(6, "StockQuantity", ConstApp.StockQuantity, "", "", true)]
    [ACPropertyEntity(7, "StockQuantityUOM", "en{'Stock Quantity(UOM)'}de{'Lagermenge(ME)'}", "", "", true)]
    [ACPropertyEntity(10, "ReservedInwardQuantity", ConstApp.ReservedInwardQuantity, "", "", true)]
    [ACPropertyEntity(11, "ReservedOutwardQuantity", ConstApp.ReservedOutwardQuantity, "", "", true)]
    [ACPropertyEntity(12, MDUnit.ClassName, "en{'Unit of Measurement(UOM)'}de{'Maßeinheit(ME)'}", Const.ContextDatabase + "\\" + MDUnit.ClassName, "", true)]
    [ACPropertyEntity(13, CompanyMaterial.ClassName, "en{'Material Manufact.'}de{'Materialhersteller'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName, "", true)]
    // 12 ReservedQuantity
    // 13 AvailableQuantity
    [ACPropertyEntity(14, Const.IsEnabled, Const.EntityIsEnabled, "", "", true)]
    [ACPropertyEntity(25, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(26, "CostFix", ConstApp.CostFix, "", "", true)]
    [ACPropertyEntity(27, "CostGeneral", ConstApp.CostGeneral, "", "", true)]
    [ACPropertyEntity(28, "CostHandlingFix", "en{'Fixed Handling Cost'}de{'Fixkosten Handling'}", "", "", true)]
    [ACPropertyEntity(29, "CostHandlingVar", "en{'Var. Handling Cost'}de{'Var. Kosten Handling'}", "", "", true)]
    [ACPropertyEntity(30, "CostLoss", ConstApp.CostLoss, "", "", true)]
    [ACPropertyEntity(31, "CostMat", ConstApp.CostMat, "", "", true)]
    [ACPropertyEntity(32, "CostPack", ConstApp.CostPack, "", "", true)]
    [ACPropertyEntity(33, "CostReQuantity", "en{'ReQuantity Cost'}de{'ReQuantitykosten'}", "", "", true)]
    [ACPropertyEntity(34, "CostVar", ConstApp.CostVar, "", "", true)]
    [ACPropertyEntity(35, "ExpirationDate", ConstApp.ExpirationDate, "", "", true)]
    [ACPropertyEntity(36, "FillingDate", ConstApp.FillingDate, "", "", true)]
    [ACPropertyEntity(38, "Lock", ConstApp.Lock, "", "", true)]
    [ACPropertyEntity(39, "NotAvailable", ConstApp.NotAvailable, "", "", true)]
    [ACPropertyEntity(43, "ProductionDate", ConstApp.ProductionDate, "", "", true)]
    [ACPropertyEntity(44, "StorageLife", ConstApp.StorageLife, "", "", true)]
    [ACPropertyEntity(45, Facility.ClassName, ConstApp.Facility, Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(46, "FacilityChargeSortNo", "en{'Charge Sort No.'}de{'Chargensortiernr'}", "", "", true)]
    [ACPropertyEntity(52, "CPartnerCompanyMaterial", "en{'Contractual Partner'}de{'Vertragspartner'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName, "", true)]
    [ACPropertyEntity(53, "StockQuantityUOMAmb", "en{'Stock Quantity(UOM) Ambient'}de{'Lagermenge(ME) ambient'}", "", "", true)]
    [ACPropertyEntity(9999, "HandlingUnit", "en{'Handling Unit'}de{'Handling Unit'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + FacilityCharge.ClassName, "en{'Facilitycharge'}de{'Chargenplatz'}", typeof(FacilityCharge), FacilityCharge.ClassName, FacilityLot.ClassName + "\\LotNo", FacilityLot.ClassName + "\\LotNo,SplitNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<FacilityCharge>) })]
    public partial class FacilityCharge
    {
        public const string ClassName = "FacilityCharge";

        #region New/Delete
        public static FacilityCharge NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            FacilityCharge entity = new FacilityCharge();
            entity.FacilityChargeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDReleaseState = MDReleaseState.DefaultMDReleaseState(dbApp);
            entity.FillingDate = DateTime.Now;
            entity.SplitNo = 0;
            if (parentACObject is Material)
            {
                Material material = parentACObject as Material;
                entity.CloneFrom(dbApp, material, null, null, null, true);
            }
            else if (parentACObject is Facility)
            {
                Facility facility = parentACObject as Facility;
                entity.CloneFrom(dbApp, null, facility, null, null, true);
            }
            else if (parentACObject is FacilityLot)
            {
                FacilityLot facilityLot = parentACObject as FacilityLot;
                entity.CloneFrom(dbApp, null, null, facilityLot, null, true);
            }
            else if (parentACObject is Partslist)
            {
                Partslist Partslist = parentACObject as Partslist;
                entity.CloneFrom(dbApp, null, null, null, Partslist, true);
            }
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


            // 1. Abhängige Daten löschen
            try
            {
                foreach (FacilityBookingCharge fbc in this.FacilityBookingCharge_InwardFacilityCharge.ToArray())
                {
                    MsgWithDetails msg = fbc.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }

                foreach (FacilityBookingCharge fbc in this.FacilityBookingCharge_OutwardFacilityCharge.ToArray())
                {
                    MsgWithDetails msg = fbc.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }

                foreach (FacilityBooking fb in this.FacilityBooking_InwardFacilityCharge.ToArray())
                {
                    MsgWithDetails msg = fb.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }

                foreach (FacilityBooking fb in this.FacilityBooking_OutwardFacilityCharge.ToArray())
                {
                    MsgWithDetails msg = fb.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }

                foreach (FacilityInventoryPos fi in this.FacilityInventoryPos_FacilityCharge.ToArray())
                {
                    MsgWithDetails msg = fi.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }

                foreach (FacilityReservation fr in this.FacilityReservation_FacilityCharge.ToArray())
                {
                    MsgWithDetails msg = fr.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
            }
            catch (Exception e)
            {
                MsgWithDetails msg = new MsgWithDetails { Source = "FacilityCharge", MessageLevel = eMsgLevel.Error, ACIdentifier = "DeleteACObject", Message = Database.Root.Environment.TranslateMessage(Database.GlobalDatabase, "Error00035") };
                ACObjectContextHelper.ParseExceptionStatic(msg, e);
                return msg;
            }

            // 2. Referenzen auflösen

            database.Remove(this);
            return null;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            try
            {
                return String.Format("No:{0}/{1}/Lot:{2}", FacilityChargeSortNo, NotAvailable ? "NotAvailable" : "Available", FacilityLot != null ? FacilityLot.LotNo : "NULL");
            }
            catch
            {
            }
            return "";
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                if (FacilityLot == null)
                    return FacilityChargeSortNo.ToString();
                return FacilityLot.ACCaption + " " + FacilityChargeSortNo;
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

        public void CloneFrom(DatabaseApp dbApp, Material material, Facility facility, FacilityLot facilityLot, Partslist Partslist, bool bAddToParentList)
        {
            bool unitSet = false;
            if (material != null)
            {
                this.Material = material;
                if (bAddToParentList)
                    material.FacilityCharge_Material.Add(this);
                this.MDUnit = this.Material.BaseMDUnit;
                unitSet = true;
            }
            if (facility != null)
            {
                this.Facility = facility;
                if (bAddToParentList)
                    facility.FacilityCharge_Facility.Add(this);
                if ((this.Material == null) && (facility.Material != null))
                {
                    this.Material = facility.Material;
                    this.MDUnit = this.Material.BaseMDUnit;
                    unitSet = true;
                }
                else if ((this.Material == null) && (facility.Material == null))
                {
                    if (!unitSet)
                    {
                        if (facility.MDFacilityType != null)
                        {
                            if (facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                            {
                                if (facility.MDUnit != null)
                                    this.MDUnit = facility.MDUnit;
                                else
                                    this.MDUnit = MDUnit.GetSIUnit(dbApp, GlobalApp.SIDimensions.Mass);
                                unitSet = true;
                            }
                        }
                    }
                }
            }
            if (facilityLot != null)
            {
                this.FacilityLot = facilityLot;
                if (bAddToParentList)
                    facilityLot.FacilityCharge_FacilityLot.Add(this);
                if (this.Material == null)
                {
                    if (facilityLot.Material != null)
                    {
                        this.Material = facilityLot.Material;
                        this.MDUnit = this.Material.BaseMDUnit;
                    }
                }
                this.MDReleaseState = facilityLot.MDReleaseState;
                this.Comment = facilityLot.Comment;
                this.ExpirationDate = facilityLot.ExpirationDate;
                this.ProductionDate = facilityLot.ProductionDate;
                this.StorageLife = facilityLot.StorageLife;
            }
            if (Partslist != null)
            {
                this.Partslist = Partslist;
                if (bAddToParentList)
                    this.Partslist.FacilityCharge_Partslist.Add(this);
                if (this.Material == null)
                {
                    if (this.Partslist.Material != null)
                    {
                        this.Material = this.Partslist.Material;
                        this.MDUnit = this.Material.BaseMDUnit;
                    }
                }
            }
        }

        public void CloneFrom(FacilityCharge facilityCharge, bool bWithStock)
        {
            if (facilityCharge == null)
                return;
            this.Material = facilityCharge.Material;
            this.Facility = facilityCharge.Facility;
            this.FacilityLot = facilityCharge.FacilityLot;
            this.SplitNo = facilityCharge.SplitNo;
            this.Partslist = facilityCharge.Partslist;
            this.CompanyMaterial = facilityCharge.CompanyMaterial;

            this.MDUnit = facilityCharge.MDUnit;

            if (bWithStock)
            {
                this.StockQuantity = facilityCharge.StockQuantity;
                this.ReservedInwardQuantity = facilityCharge.ReservedInwardQuantity;
                this.ReservedOutwardQuantity = facilityCharge.ReservedOutwardQuantity;
            }
            else
            {
                this.StockQuantity = 0;
                this.ReservedInwardQuantity = 0;
                this.ReservedOutwardQuantity = 0;
            }

            this.IsEnabled = facilityCharge.IsEnabled;
            this.Comment = facilityCharge.Comment;
            this.CostFix = facilityCharge.CostFix;
            this.CostGeneral = facilityCharge.CostGeneral;
            this.CostHandlingFix = facilityCharge.CostHandlingFix;
            this.CostHandlingVar = facilityCharge.CostHandlingVar;
            this.CostLoss = facilityCharge.CostLoss;
            this.CostMat = facilityCharge.CostMat;
            this.CostPack = facilityCharge.CostPack;
            this.CostReQuantity = facilityCharge.CostReQuantity;
            this.CostVar = facilityCharge.CostVar;
            this.ExpirationDate = facilityCharge.ExpirationDate;
            this.FillingDate = facilityCharge.FillingDate;
            this.Lock = facilityCharge.Lock;
            this.MDReleaseState = facilityCharge.MDReleaseState;
            this.NotAvailable = facilityCharge.NotAvailable;
            this.ProductionDate = facilityCharge.ProductionDate;
            this.StorageLife = facilityCharge.StorageLife;
            this.XMLConfig = facilityCharge.XMLConfig;
        }

        public void CopyFrom(FacilityCharge from, bool withReferences, bool bWithStock)
        {
            if (withReferences)
            {
                FacilityLotID = from.FacilityLotID;
                FacilityID = from.FacilityID;
                MaterialID = from.MaterialID;
                PartslistID = from.PartslistID;
                MDReleaseStateID = from.MDReleaseStateID;
                CompanyMaterialID = from.CompanyMaterialID;
                MDUnitID = from.MDUnitID;
            }

            if (bWithStock)
            {
                StockQuantity = from.StockQuantity;
                ReservedInwardQuantity = from.ReservedInwardQuantity;
                ReservedOutwardQuantity = from.ReservedOutwardQuantity;
            }
            else
            {
                StockQuantity = 0;
                ReservedInwardQuantity = 0;
                ReservedOutwardQuantity = 0;
            }

            FacilityChargeSortNo = from.FacilityChargeSortNo;
            SplitNo = from.SplitNo;
            NotAvailable = from.NotAvailable;
            HandlingUnit = from.HandlingUnit;
            FillingDate = from.FillingDate;
            StorageLife = from.StorageLife;
            ProductionDate = from.ProductionDate;
            ExpirationDate = from.ExpirationDate;
            CostReQuantity = from.CostReQuantity;
            CostMat = from.CostMat;
            CostVar = from.CostVar;
            CostFix = from.CostFix;
            CostPack = from.CostPack;
            CostGeneral = from.CostGeneral;
            CostLoss = from.CostLoss;
            CostHandlingVar = from.CostHandlingVar;
            CostHandlingFix = from.CostHandlingFix;
            Comment = from.Comment;
            Lock = from.Lock;
            IsEnabled = from.IsEnabled;
            XMLConfig = from.XMLConfig;
            RowVersion = from.RowVersion;
            CPartnerCompanyMaterialID = from.CPartnerCompanyMaterialID;
            StockQuantityUOMAmb = from.StockQuantityUOMAmb;
        }

        public void AddToParentsList()
        {
            if (this.Material != null)
                this.Material.FacilityCharge_Material.Add(this);
            if (this.Facility != null)
                this.Facility.FacilityCharge_Facility.Add(this);
            if (this.FacilityLot != null)
                FacilityLot.FacilityCharge_FacilityLot.Add(this);
            if (this.Partslist != null)
                this.Partslist.FacilityCharge_Partslist.Add(this);
        }

        public bool IsStockZero
        {
            get
            {
                if (Math.Abs(StockQuantity - 0) > double.Epsilon)
                    return false;
                return true;
            }
        }

        static public string KeyACIdentifier
        {
            get
            {
                return "FacilityLot\\LotNo,SplitNo";
            }
        }
        #endregion

        #region IEntityProperty Members

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

        #region additional properties

        /// <summary>
        /// There are no comments for Property ReservedInwardQuantity in the schema.
        /// </summary>
        public void RecalcReservedInwardQuantity()
        {
            if (this.Material == null)
                return;

            this.ReservedInwardQuantity = 0;
            // TODO: OR-Klausel einfügen für Produktionsaufträge
            IEnumerable<FacilityReservation> facilityReservationList = FacilityReservation_FacilityCharge.Where(c => c.InOrderPosID.HasValue);
            foreach (FacilityReservation facilityReservation in facilityReservationList)
            {
                // Die ActualQuantity gibt an, wieviel bereits auf dem Material gebucht worden ist
                // Die TargetQuantity gibt an, wieviel Reserviert ist
                // Die Differenz gibt an, wieviel noch geliefert wird
                // TODO: Zugänge von Produktionsaufträge mit einrechnen
                // TODO: Mit MaterialUnit rechnen anstatt MDQuantityUnit
                //this.ReservedInwardQuantity +=
                //        this.Material.QuantityToQuantity(facilityReservation.InOrderPos.TargetQuantity,
                //                                    facilityReservation.InOrderPos.Material.StorageMDQuantityUnit,
                //                                    this.MaterialUnit.MDQuantityUnit)
                //        - this.Material.QuantityToQuantity(facilityReservation.InOrderPos.ActualQuantity,
                //                                    facilityReservation.InOrderPos.Material.StorageMDQuantityUnit,
                //                                    this.MaterialUnit.MDQuantityUnit);

            }
        }

        /// <summary>
        /// There are no comments for Property ReservedOutwardQuantity in the schema.
        /// </summary>
        public void RecalcReservedOutwardQuantity()
        {
            if (this.Material == null)
                return;

            this.ReservedOutwardQuantity = 0;
            // TODO: OR-Klausel einfügen für Produktionsaufträge
            IEnumerable<FacilityReservation> facilityReservationList = FacilityReservation_FacilityCharge.Where(c => c.OutOrderPosID.HasValue);
            foreach (FacilityReservation facilityReservation in facilityReservationList)
            {
                // Die ActualQuantity gibt an, wieviel bereits auf dem Material gebucht worden ist
                // Die TargetQuantity gibt an, wieviel Reserviert ist
                // Die Differenz gibt an, wieviel noch abgebucht wird
                // TODO: Abgänge von Produktionsaufträgen mit einrechnen
                // TODO: Mit MaterialUnit rechnen anstatt MDQuantityUnit
                //this.ReservedOutwardQuantity +=
                //        this.FacilityLot.Material.QuantityToQuantity(facilityReservation.OutOrderPos.TargetQuantity,
                //                                    facilityReservation.OutOrderPos.Material.StorageMDQuantityUnit,
                //                                    this.MaterialUnit.MDQuantityUnit)
                //        - this.FacilityLot.Material.QuantityToQuantity(facilityReservation.OutOrderPos.ActualQuantity,
                //                                    facilityReservation.OutOrderPos.Material.StorageMDQuantityUnit,
                //                                    this.MaterialUnit.MDQuantityUnit);

            }
        }

        /// <summary>
        /// There are no comments for Property ReservedQuantity in the schema.
        /// </summary>
        [ACPropertyInfo(12, "", "en{'Reserved Quantity'}de{'Reservierte Menge'}")]
        public Double ReservedQuantity
        {
            get
            {
                return ReservedOutwardQuantity - ReservedInwardQuantity;
            }
        }

        /// <summary>
        /// There are no comments for Property AvailableQuantity in the schema.
        /// </summary>
        [ACPropertyInfo(13, "", "en{'Available Quantity'}de{'Verfügbare Menge'}")]
        public Double AvailableQuantity
        {
            get
            {
                return StockQuantity - ReservedQuantity;
            }
        }

        #region FacilityCharge Origin

        /// <summary>
        /// Charge is direct input into stock from outside
        /// recived from distributor company
        /// </summary>
        public bool IsInputOutside
        {
            get
            {
                return
                    FacilityBooking_InwardFacilityCharge.Any() && FacilityBooking_InwardFacilityCharge.Any(x => x.InOrderPosID != null)
                    ||
                    FacilityBookingCharge_InwardFacilityCharge.Any() && FacilityBookingCharge_InwardFacilityCharge.Select(x => x.FacilityBooking).Any(x => x.InOrderPosID != null);
            }
        }

        /// <summary>
        /// Charge belong to final product (placed on stock but shuld not be delivered until yet
        /// </summary>
        public bool IsFinalOutput
        {
            get
            {
                return FinalRootPositionFromFB != null || FinalRootPositionFromFbc != null;
            }
        }

        /// <summary>
        /// Charge belong to delivered final product
        /// </summary>
        public bool IsFinalOutputDelivered
        {
            get
            {
                return
                     FacilityBooking_OutwardFacilityCharge.Any() && FacilityBooking_OutwardFacilityCharge.Any(x => x.OutOrderPosID != null)
                     ||
                     FacilityBookingCharge_OutwardFacilityCharge.Any() && FacilityBookingCharge_OutwardFacilityCharge.Any(x => x.OutOrderPosID != null);
            }
        }

        /// <summary>
        /// @aagincic: this is implemented temporaly before InOrder preparing (for export purpurose)
        /// </summary>
        public bool IsNoConnectionWithProduction
        {
            get
            {
                return
                FacilityBooking_InwardFacilityCharge.Any() && FacilityBooking_InwardFacilityCharge.Any(x => x.ProdOrderPartslistPosID == null && x.ProdOrderPartslistPosRelationID == null)
                ||
                FacilityBookingCharge_InwardFacilityCharge.Any() && FacilityBookingCharge_InwardFacilityCharge.Select(x => x.FacilityBooking).Any(x => x.ProdOrderPartslistPosID == null && x.ProdOrderPartslistPosRelationID == null);
            }
        }

        public ProdOrderPartslistPos FinalRootPositionFromFB
        {
            get
            {
                //if (!FacilityBooking_OutwardFacilityCharge.Any()) return null;
                //return FacilityBooking_OutwardFacilityCharge
                //    .Where(x => x.ProdOrderPartslistPosID != null)
                //    .Select(x => x.ProdOrderPartslistPos.ParentProdOrderPartslistPosID != null ? x.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos : x.ProdOrderPartslistPos).FirstOrDefault(x => x.IsFinallMixure);
                if (!FacilityBooking_InwardFacilityCharge.Any()) return null;
                return FacilityBooking_InwardFacilityCharge.Where(a => a.ProdOrderPartslistPosID != null)
                                                 .Select(a => a.ProdOrderPartslistPos.ParentProdOrderPartslistPosID != null ? a.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos : a.ProdOrderPartslistPos)
                                                 .Where(a => (a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern || a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern))
                                                 .FirstOrDefault();
                //.Select(a => a.ProdOrderPartslist.ProdOrder.ProgramNo);

            }
        }

        public ProdOrderPartslistPos FinalPositionFromFB
        {
            get
            {
                if (!FacilityBooking_InwardFacilityCharge.Any()) return null;
                return FacilityBooking_InwardFacilityCharge.Where(a => a.ProdOrderPartslistPosID != null)
                                                 .Select(a => a.ProdOrderPartslistPos)
                                                 .Where(a => (a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern || a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern))
                                                 .FirstOrDefault();
            }
        }

        public ProdOrderPartslistPos FinalRootPositionFromFbc
        {
            get
            {
                if (!FacilityBookingCharge_InwardFacilityCharge.Any()) return null;
                //return FacilityBookingCharge_InwardFacilityCharge
                //     .Select(x => x.FacilityBooking)
                //     .Where(x => x.ProdOrderPartslistPosID != null)
                //        .Select(x => x.ProdOrderPartslistPos.ParentProdOrderPartslistPosID != null ? x.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos : x.ProdOrderPartslistPos).FirstOrDefault(x => x.IsFinallMixure);
                return FacilityBookingCharge_InwardFacilityCharge.Select(a => a.FacilityBooking)
                                                .Where(a => a.ProdOrderPartslistPosID != null)
                                                .Select(a => a.ProdOrderPartslistPos.ParentProdOrderPartslistPosID != null ? a.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos : a.ProdOrderPartslistPos)
                                                .Where(a => (a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern || a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern))
                                                .FirstOrDefault();
            }
        }

        public ProdOrderPartslistPos FinalPositionFromFbc
        {
            get
            {
                if (!FacilityBookingCharge_InwardFacilityCharge.Any()) return null;
                return FacilityBookingCharge_InwardFacilityCharge.Select(a => a.FacilityBooking)
                                                .Where(a => a.ProdOrderPartslistPosID != null)
                                                .Select(a => a.ProdOrderPartslistPos)
                                                .Where(a => (a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern || a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern))
                                                .FirstOrDefault();
            }
        }

        #endregion

        [ACPropertyInfo(999, "ProdOrderProgramNo", "en{'Order No.'}de{'Auftragsnummer'}")]
        public string ProdOrderProgramNo
        {
            get
            {
                string programNo = "";
                if (IsFinalOutput)
                {
                    ProdOrderPartslistPos pos = null;
                    pos = FinalRootPositionFromFB;
                    if (pos == null)
                        pos = FinalRootPositionFromFbc;
                    if (pos != null)
                        programNo = pos.ProdOrderPartslist.ProdOrder.ProgramNo;
                }
                return programNo;
            }
        }

        [ACPropertyInfo(999, "BatchNo", "en{'Batch No.'}de{'Batchnummer'}")]
        public string BatchNo
        {
            get
            {
                string programNo = "";
                if (IsFinalOutput)
                {
                    ProdOrderPartslistPos pos = null;
                    pos = FinalPositionFromFB;
                    if (pos == null)
                        pos = FinalPositionFromFbc;
                    if (pos != null && pos.ProdOrderBatchID.HasValue)
                        programNo = String.Format("{0} ({1})", pos.ProdOrderBatch.BatchSeqNo, pos.ProdOrderBatch.ProdOrderBatchNo);
                }
                return programNo;
            }
        }


        [ACPropertyInfo(999, "InOrderNo", "en{'Purchase Order Number'}de{'Bestellnummer'}")]
        public string InOrderNo
        {
            get
            {
                string inOrderNo = "";
                if (IsInputOutside)
                {
                    FacilityBooking fb = null;
                    if (FacilityBooking_InwardFacilityCharge.Any(x => x.InOrderPosID != null))
                    {
                        fb = FacilityBooking_InwardFacilityCharge.FirstOrDefault(x => x.InOrderPosID != null);
                    }
                    else if (FacilityBookingCharge_InwardFacilityCharge.Any())
                    {
                        fb = FacilityBookingCharge_InwardFacilityCharge.Select(x => x.FacilityBooking).FirstOrDefault(x => x.InOrderPosID != null);
                    }
                    if (fb != null)
                        inOrderNo = fb.InOrderPos.InOrder.InOrderNo;
                }
                return inOrderNo;
            }
        }

        public ProdOrderPartslistPos GetFinalRootPositionFromFB()
        {
            if (!this.FacilityBooking_InwardFacilityCharge.Any()) return null;
            return this.FacilityBooking_InwardFacilityCharge.Where(a => a.ProdOrderPartslistPosID != null)
                                             .Select(a => a.ProdOrderPartslistPos.ParentProdOrderPartslistPosID != null ? a.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos : a.ProdOrderPartslistPos)
                                             .Where(a => (a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern || a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern))
                                             .FirstOrDefault();
        }

        public ProdOrderPartslistPos GetFinalPositionFromFB()
        {
            if (!this.FacilityBooking_InwardFacilityCharge.Any()) return null;
            return this.FacilityBooking_InwardFacilityCharge.Where(a => a.ProdOrderPartslistPosID != null)
                                             .Select(a => a.ProdOrderPartslistPos)
                                             .Where(a => (a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern || a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern))
                                             .FirstOrDefault();
        }

        public string GetProdOrderProgramNo()
        {
            string programNo = "";
            ProdOrderPartslistPos pos = GetFinalRootPositionFromFB();
            if (pos != null)
                programNo = pos.ProdOrderPartslist.ProdOrder.ProgramNo;
            return programNo;
        }

        public string GetBatchNo()
        {
            string programNo = "";
            ProdOrderPartslistPos pos = null;
            pos = this.GetFinalPositionFromFB();
            if (pos != null && pos.ProdOrderBatchID.HasValue)
                programNo = String.Format("{0} ({1})", pos.ProdOrderBatch.BatchSeqNo, pos.ProdOrderBatch.ProdOrderBatchNo);
            return programNo;
        }


        /// <summary>
        /// Charge belong to delivered final product
        /// </summary>
        public bool GetIsFinalOutputDelivered()
        {
            return (this.FacilityBooking_OutwardFacilityCharge.Any() && this.FacilityBooking_OutwardFacilityCharge.Any(x => x.OutOrderPosID != null))
                 || (this.FacilityBookingCharge_OutwardFacilityCharge.Any() && this.FacilityBookingCharge_OutwardFacilityCharge.Any(x => x.OutOrderPosID != null));
        }

        /// <summary>
        /// @aagincic: this is implemented temporaly before InOrder preparing (for export purpurose)
        /// </summary>
        public bool GetIsConnectedWithProdOrder()
        {
            return (this.FacilityBooking_InwardFacilityCharge.Any() && this.FacilityBooking_InwardFacilityCharge.Any(x => x.ProdOrderPartslistPosID != null || x.ProdOrderPartslistPosRelationID != null))
                || (this.FacilityBookingCharge_InwardFacilityCharge.Any() && this.FacilityBookingCharge_InwardFacilityCharge.Select(x => x.FacilityBooking).Any(x => x.ProdOrderPartslistPosID != null || x.ProdOrderPartslistPosRelationID != null));
        }

        public static IQueryable<FacilityChargeModel> GetFacilityChargeModelList(DatabaseApp dbApp, IQueryable<FacilityCharge> fcs)
        {
            var queryStep1 =
                fcs
                .GroupJoin(dbApp.FacilityBookingCharge,
                    fc => fc.FacilityChargeID,
                    fbc => fbc.InwardFacilityChargeID, (fc, fbc) => new { fc, fbc })
                .Select(c => new
                {
                    c.fc.FacilityChargeID,
                    IntermediateItem = c
                                        .fbc
                                        .Where(fbc1 => fbc1.ProdOrderPartslistPosID != null)
                                        .Select(x => x.ProdOrderPartslistPos)
                                        .Where(a => (a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern || a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern))
                                        .FirstOrDefault(),
                    LotNo = c.fc.FacilityLotID != null ? c.fc.FacilityLot.LotNo : "",
                    ExternLotNo = c.fc.FacilityLotID != null ? c.fc.FacilityLot.ExternLotNo : "",
                    ExternLotNo2 = c.fc.FacilityLotID != null ? c.fc.FacilityLot.ExternLotNo2 : "",
                    c.fc.Material.MaterialNo,
                    c.fc.Material.MaterialName1,
                    c.fc.Facility.FacilityNo,
                    c.fc.Facility.FacilityName,
                    c.fc.Comment,
                    c.fc.InsertDate,
                    c.fc.MDUnit.TechnicalSymbol,
                    ProdOrderProgramNo = "",
                    BatchNo = "",
                    MachineName = " ",
                    IsFinalOutput = false,
                    StockQuantity = c.fc.StockQuantity
                });

            var queryStep2 =
            queryStep1
            .Select(c => new FacilityChargeModel()
            {
                FacilityChargeID = c.FacilityChargeID,
                IntermediateItem = c.IntermediateItem,
                LotNo = c.LotNo,
                ExternLotNo = c.ExternLotNo,
                ExternLotNo2 = c.ExternLotNo2,
                MaterialNo = c.MaterialNo,
                MaterialName1 = c.MaterialName1,
                FacilityNo = c.FacilityNo,
                FacilityName = c.FacilityName,
                Comment = c.Comment,
                InsertDate = c.InsertDate,
                MDUnitName = c.TechnicalSymbol,

                ProdOrderProgramNo = (c.IntermediateItem != null) ? c.IntermediateItem.ProdOrderPartslist.ProdOrder.ProgramNo : "",
                ProdOrderInsertDate = c.IntermediateItem.ProdOrderPartslist.ProdOrder.InsertDate,
                BatchNo = c.IntermediateItem != null && c.IntermediateItem.ProdOrderBatchID != null ? c.IntermediateItem.ProdOrderBatch.ProdOrderBatchNo : "",
                MachineName = c.IntermediateItem != null ?
                    c.IntermediateItem.ProdOrderBatch.ProdOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan
                    .Where(fr => fr.VBiACClassID != null && !(fr.ParentFacilityReservationID != null))
                    .OrderBy(fr => fr.Sequence)
                    .Select(fr => fr.VBiACClass.ACIdentifier)
                    .FirstOrDefault()
                    : "",
                StockQuantity = c.StockQuantity
            });


            return queryStep2;
        }

        #endregion

    }
}
