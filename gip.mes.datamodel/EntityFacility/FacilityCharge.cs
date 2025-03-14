using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using static gip.mes.datamodel.GlobalApp;

namespace gip.mes.datamodel
{

    // FacilityCharge (Chargenplatz)
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Batch Location'}de{'Chargenplatz'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOFacilityBookCharge")]
    [ACPropertyEntity(1, FacilityLot.ClassName, ConstApp.Lot, Const.ContextDatabase + "\\" + FacilityLot.ClassName, "", true)]
    [ACPropertyEntity(2, nameof(SplitNo), ConstApp.SplitNo, "", "", true)]
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

            database.DeleteObject(this);
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

        private bool _IsSelected;
        [ACPropertyInfo(503, nameof(IsSelected), Const.Select)]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }


        // avoid handling IsSelected for others when one is treated
        public bool InIsSelectedProcess { get; set; }

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


        public void ResetCachedValues()
        {
            _RelocationQuantity = 0;
            _IsInputOutside = null;
            _HasDeliveryPostings = null;
            _HasProductionPostings = null;
            _FinalPositionFromFbcRead = false;
            _FinalRootPositionFromFbc = null;
            _FinalPositionFromFbc = null;
            _InOrderNo = null;
            _relatedCompanyLoaded = false;
            _RelatedCompany = null;
            _CompanyNo = null;
            _CompanyName = null;
            _FinalRootPositionFromFBRead = false;
            _FinalRootPositionFromFB = null;
            _FinalPositionFromFBRead = false;
            _FinalPositionFromFB = null;
        }

        #region Additional Properties -> Quantities

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

        private double _RelocationQuantity;
        /// <summary>
        /// Quantity at default is AvailableQuantity
        /// used by relocation quant dialog
        /// </summary>
        [ACPropertyInfo(15, "", "en{'Relocation Quantity'}de{'Umlagerung Menge'}")]
        public double RelocationQuantity
        {
            get
            {
                return _RelocationQuantity;
            }
            set
            {
                _RelocationQuantity = value;
                OnPropertyChanged(nameof(RelocationQuantity));
            }
        }

        #endregion

        #region Additional Properties -> FacilityCharge Origin

        private bool? _IsInputOutside;
        /// <summary>
        /// Charge is direct input into stock from outside
        /// recived from distributor company
        /// </summary>
        public bool IsInputOutside
        {
            get
            {
                if (_IsInputOutside.HasValue)
                    return _IsInputOutside.Value;
                _IsInputOutside = FacilityBookingCharge_InwardFacilityCharge.Where(x => x.InOrderPosID != null).Any();
                return _IsInputOutside.Value;
            }
        }

        /// <summary>
        /// Charge belong to final product (placed on stock but shuld not be delivered until yet
        /// </summary>
        public bool IsFinalOutput
        {
            get
            {
                return FinalRootPositionFromFbc != null;
            }
        }

        private bool? _HasDeliveryPostings;
        /// <summary>
        /// Has Postings that are related to an Delivery (OutOrder)
        /// </summary>
        public bool HasDeliveryPostings
        {
            get
            {
                if (_HasDeliveryPostings.HasValue)
                    return _HasDeliveryPostings.Value;
                _HasDeliveryPostings = FacilityBookingCharge_OutwardFacilityCharge.Where(x => x.OutOrderPosID != null).Any();
                return _HasDeliveryPostings.Value;
            }
        }

        private bool? _HasProductionPostings = null;
        /// <summary>
        /// Has Postings that are related to In- or Outword postings of Production Orders
        /// </summary>
        public bool HasProductionPostings
        {
            get
            {
                if (_HasProductionPostings.HasValue)
                    return _HasProductionPostings.Value;
                _HasProductionPostings = FacilityBookingCharge_InwardFacilityCharge.Where(x => x.ProdOrderPartslistPosID == null && x.ProdOrderPartslistPosRelationID == null).Any();
                return _HasProductionPostings.Value;
            }
        }

        //public ProdOrderPartslistPos FinalRootPositionFromFB
        //{
        //    get
        //    {
        //        //if (!FacilityBooking_OutwardFacilityCharge.Any()) return null;
        //        //return FacilityBooking_OutwardFacilityCharge
        //        //    .Where(x => x.ProdOrderPartslistPosID != null)
        //        //    .Select(x => x.ProdOrderPartslistPos.ParentProdOrderPartslistPosID != null ? x.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos : x.ProdOrderPartslistPos).FirstOrDefault(x => x.IsFinallMixure);
        //        if (!FacilityBooking_InwardFacilityCharge.Any()) 
        //            return null;
        //        return FacilityBooking_InwardFacilityCharge.Where(a => a.ProdOrderPartslistPosID != null)
        //                                         .Select(a => a.ProdOrderPartslistPos.ParentProdOrderPartslistPosID != null ? a.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos : a.ProdOrderPartslistPos)
        //                                         .Where(a => (a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern || a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern))
        //                                         .FirstOrDefault();
        //        //.Select(a => a.ProdOrderPartslist.ProdOrder.ProgramNo);

        //    }
        //}

        //public ProdOrderPartslistPos FinalPositionFromFB
        //{
        //    get
        //    {
        //        //if (!FacilityBooking_InwardFacilityCharge.Any()) 
        //        //    return null;
        //        return FacilityBooking_InwardFacilityCharge.Where(a => a.ProdOrderPartslistPosID != null)
        //                                         .Select(a => a.ProdOrderPartslistPos)
        //                                         .Where(a => (a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern || a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern))
        //                                         .FirstOrDefault();
        //    }
        //}

        private ProdOrderPartslistPos _FinalRootPositionFromFbc;
        public ProdOrderPartslistPos FinalRootPositionFromFbc
        {
            get
            {
                if (_FinalRootPositionFromFbc != null)
                    return _FinalRootPositionFromFbc;

                if (FinalPositionFromFbc != null)
                {
                    _FinalRootPositionFromFbc = FinalPositionFromFbc.ProdOrderPartslistPos1_ParentProdOrderPartslistPos;
                    if (_FinalRootPositionFromFbc == null)
                        _FinalRootPositionFromFbc = FinalPositionFromFbc;
                }
                return _FinalRootPositionFromFbc;
            }
        }

        private bool _FinalPositionFromFbcRead = false;
        private ProdOrderPartslistPos _FinalPositionFromFbc;
        public ProdOrderPartslistPos FinalPositionFromFbc
        {
            get
            {
                if (_FinalPositionFromFbcRead)
                    return _FinalPositionFromFbc;

                _FinalPositionFromFbc = FacilityBookingCharge_InwardFacilityCharge
                    .Where(c => c.ProdOrderPartslistPosID != null)
                    .OrderByDescending(c => c.InsertDate)
                    .Select(c => c.ProdOrderPartslistPos)
                    .FirstOrDefault();
                _FinalPositionFromFbcRead = true;

                return _FinalPositionFromFbc;
            }
        }

        #endregion

        #region Additional Properties -> Order (ProdOrder, InOrder)

        [ACPropertyInfo(9999, "ProdOrderProgramNo", "en{'Order No.'}de{'Auftragsnummer'}")]
        public string ProdOrderProgramNo
        {
            get
            {
                if (IsFinalOutput)
                {
                    ProdOrderPartslistPos pos = FinalPositionFromFbc;
                    if (pos != null)
                        return pos.ProdOrderPartslist?.ProdOrder?.ProgramNo;
                }
                return "";
            }
        }

        [ACPropertyInfo(9999, "BatchNo", "en{'Batch No.'}de{'Batchnummer'}")]
        public string BatchNo
        {
            get
            {
                string programNo = "";
                if (IsFinalOutput)
                {
                    ProdOrderPartslistPos pos = FinalPositionFromFbc;
                    if (pos != null && pos.ProdOrderBatchID.HasValue)
                        programNo = String.Format("{0} ({1})", pos.ProdOrderBatch.BatchSeqNo, pos.ProdOrderBatch.ProdOrderBatchNo);
                }
                return programNo;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Prod. order position sequence'}de{'Reihenfolge der PA positionen'}")]
        public int ProdOrderPosSequence
        {
            get
            {
                return FinalPositionFromFbc != null ? FinalPositionFromFbc.Sequence : 1;
            }
        }


        string _InOrderNo = null;
        [ACPropertyInfo(999, "InOrderNo", "en{'Purchase Order Number'}de{'Bestellnummer'}")]
        public string InOrderNo
        {
            get
            {
                if (!IsInputOutside)
                    return null;
                if (_InOrderNo == null)
                {
                    _InOrderNo = FacilityBooking_InwardFacilityCharge.Where(x => x.InOrderPosID != null).Select(x => x.InOrderPos.InOrder.InOrderNo).FirstOrDefault();
                    if (_InOrderNo == null)
                        _InOrderNo = "";
                }
                //FacilityBooking fb = FacilityBooking_InwardFacilityCharge.Where(x => x.InOrderPosID != null).Select(x => x.InOrderPos.InOrder.InOrderNo).FirstOrDefault();
                //if (FacilityBooking_InwardFacilityCharge.Any(x => x.InOrderPosID != null))
                //{
                //    fb = FacilityBooking_InwardFacilityCharge.FirstOrDefault(x => x.InOrderPosID != null);
                //}
                //else if (FacilityBookingCharge_InwardFacilityCharge.Any())
                //{
                //    fb = FacilityBookingCharge_InwardFacilityCharge.Select(x => x.FacilityBooking).FirstOrDefault(x => x.InOrderPosID != null);
                //}
                //if (fb != null)
                //return fb.InOrderPos.InOrder.InOrderNo;
                return _InOrderNo;
            }
        }

        #endregion

        #region Additional Properties -> Related Company 

        private bool _relatedCompanyLoaded;
        private Company _RelatedCompany;
        protected Company RelatedCompany
        {
            get
            {
                if (!_relatedCompanyLoaded && Material != null)
                {
                    _relatedCompanyLoaded = true;
                    _RelatedCompany =
                        Material
                        .CompanyMaterial_Material
                        .OrderByDescending(c => c.InsertDate)
                        .Select(c => c.Company)
                        .FirstOrDefault();
                }
                return _RelatedCompany;
            }
        }

        private string _CompanyNo;
        [ACPropertyInfo(500, nameof(CompanyNo), ConstApp.CompanyNo)]
        public string CompanyNo
        {
            get
            {
                if (_CompanyNo != null)
                    return _CompanyNo;
                if (RelatedCompany != null)
                    _CompanyNo = RelatedCompany.CompanyNo;
                if (_CompanyNo == null)
                    _CompanyNo = "";
                return _CompanyNo;
            }
        }

        private string _CompanyName;
        [ACPropertyInfo(501, nameof(CompanyName), ConstApp.CompanyName)]
        public string CompanyName
        {
            get
            {
                if (_CompanyName != null)
                    return _CompanyName;
                if (RelatedCompany != null)
                    _CompanyName = RelatedCompany.CompanyName;
                if (_CompanyName == null)
                    _CompanyName = "";
                return _CompanyName;
            }
        }

        #endregion

        #region Additional Properties -> ReservationState

        public ReservationState ReservationState
        {
            get
            {
                ReservationState reservationState = ReservationState.New;
                if (SelectedReservationState != null)
                {
                    reservationState = (ReservationState)SelectedReservationState.Value;
                }
                return reservationState;
            }
            set
            {
                SelectedReservationState = ReservationStateList.Where(c => ((ReservationState)c.Value) == value).FirstOrDefault();
            }
        }

        ACValueItem _SelectedReservationState;
        [ACPropertySelected(9999, nameof(ReservationState), ConstApp.FacilityReservation)]
        public ACValueItem SelectedReservationState
        {
            get
            {
                return _SelectedReservationState;
            }
            set
            {
                if (_SelectedReservationState != value)
                {
                    _SelectedReservationState = value;
                    OnPropertyChanged(nameof(SelectedReservationState));
                }
            }
        }

        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        [ACPropertyList(9999, nameof(ReservationState))]
        public IEnumerable<ACValueItem> ReservationStateList
        {
            get
            {
                return GlobalApp.ReservationStateList;
            }
        }

        #endregion

        #endregion

        #region Methods

        protected override void OnPropertyChanged(string property)
        {
            if (property == nameof(StockQuantity))
                OnPropertyChanged(nameof(AvailableQuantity));

            if (property == nameof(ReservedOutwardQuantity))
            {
                OnPropertyChanged(nameof(ReservedQuantity));
                OnPropertyChanged(nameof(AvailableQuantity));
            }

            if (property == nameof(ReservedInwardQuantity))
            {
                OnPropertyChanged(nameof(ReservedQuantity));
                OnPropertyChanged(nameof(AvailableQuantity));
            }

            base.OnPropertyChanged(property);
        }

        //public void RecalcReservedInwardQuantity()
        //{
        //    if (this.Material == null)
        //        return;

        //    this.ReservedInwardQuantity = 0;
        //    // TODO: OR-Klausel einfügen für Produktionsaufträge
        //    IEnumerable<FacilityReservation> facilityReservationList = FacilityReservation_FacilityCharge.Where(c => c.InOrderPosID.HasValue);
        //    foreach (FacilityReservation facilityReservation in facilityReservationList)
        //    {
        //        // Die ActualQuantity gibt an, wieviel bereits auf dem Material gebucht worden ist
        //        // Die TargetQuantity gibt an, wieviel Reserviert ist
        //        // Die Differenz gibt an, wieviel noch geliefert wird
        //        // TODO: Zugänge von Produktionsaufträge mit einrechnen
        //        // TODO: Mit MaterialUnit rechnen anstatt MDQuantityUnit
        //        //this.ReservedInwardQuantity +=
        //        //        this.Material.QuantityToQuantity(facilityReservation.InOrderPos.TargetQuantity,
        //        //                                    facilityReservation.InOrderPos.Material.StorageMDQuantityUnit,
        //        //                                    this.MaterialUnit.MDQuantityUnit)
        //        //        - this.Material.QuantityToQuantity(facilityReservation.InOrderPos.ActualQuantity,
        //        //                                    facilityReservation.InOrderPos.Material.StorageMDQuantityUnit,
        //        //                                    this.MaterialUnit.MDQuantityUnit);

        //    }
        //}


        //public void RecalcReservedOutwardQuantity()
        //{
        //    if (this.Material == null)
        //        return;

        //    this.ReservedOutwardQuantity = 0;
        //    // TODO: OR-Klausel einfügen für Produktionsaufträge
        //    IEnumerable<FacilityReservation> facilityReservationList = FacilityReservation_FacilityCharge.Where(c => c.OutOrderPosID.HasValue);
        //    foreach (FacilityReservation facilityReservation in facilityReservationList)
        //    {
        //        // Die ActualQuantity gibt an, wieviel bereits auf dem Material gebucht worden ist
        //        // Die TargetQuantity gibt an, wieviel Reserviert ist
        //        // Die Differenz gibt an, wieviel noch abgebucht wird
        //        // TODO: Abgänge von Produktionsaufträgen mit einrechnen
        //        // TODO: Mit MaterialUnit rechnen anstatt MDQuantityUnit
        //        //this.ReservedOutwardQuantity +=
        //        //        this.FacilityLot.Material.QuantityToQuantity(facilityReservation.OutOrderPos.TargetQuantity,
        //        //                                    facilityReservation.OutOrderPos.Material.StorageMDQuantityUnit,
        //        //                                    this.MaterialUnit.MDQuantityUnit)
        //        //        - this.FacilityLot.Material.QuantityToQuantity(facilityReservation.OutOrderPos.ActualQuantity,
        //        //                                    facilityReservation.OutOrderPos.Material.StorageMDQuantityUnit,
        //        //                                    this.MaterialUnit.MDQuantityUnit);

        //    }
        //}

        bool _FinalRootPositionFromFBRead = false;
        ProdOrderPartslistPos _FinalRootPositionFromFB = null;
        public ProdOrderPartslistPos GetFinalRootPositionFromFB()
        {
            if (_FinalRootPositionFromFBRead)
                return _FinalRootPositionFromFB;
            _FinalRootPositionFromFB = this.FacilityBooking_InwardFacilityCharge.Where(a => a.ProdOrderPartslistPosID != null)
                                             .Select(a => a.ProdOrderPartslistPos.ParentProdOrderPartslistPosID != null ? a.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos : a.ProdOrderPartslistPos)
                                             .Where(a => (a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern || a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern))
                                             .FirstOrDefault();
            _FinalRootPositionFromFBRead = true;
            return _FinalRootPositionFromFB;
        }

        bool _FinalPositionFromFBRead = false;
        ProdOrderPartslistPos _FinalPositionFromFB = null;
        public ProdOrderPartslistPos GetFinalPositionFromFB()
        {
            if (_FinalPositionFromFBRead)
                return _FinalPositionFromFB;
            _FinalPositionFromFB = this.FacilityBooking_InwardFacilityCharge.Where(a => a.ProdOrderPartslistPosID != null)
                                             .Select(a => a.ProdOrderPartslistPos)
                                             .Where(a => (a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern || a.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern))
                                             .FirstOrDefault();
            _FinalPositionFromFBRead = true;
            return _FinalPositionFromFB;
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
                ProdOrderInsertDate = (c.IntermediateItem != null) ? c.IntermediateItem.ProdOrderPartslist.ProdOrder.InsertDate : DateTime.MinValue,
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
