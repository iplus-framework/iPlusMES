using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    // FacilityBooking (Lagerbewegung)
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Stock Movement'}de{'Lagerbewegung'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "FacilityBookingNo", "en{'Posting No.'}de{'Buchungsnr.'}", "", "", true)]
    [ACPropertyEntity(2, "FacilityBookingTypeIndex", "en{'Posting Type'}de{'Buchungsart'}", typeof(GlobalApp.FacilityBookingType), Const.ContextDatabase + "\\FacilityBookingTypeList", "", true)]
    [ACPropertyEntity(3, "InwardQuantity", ConstApp.InwardQuantity, "", "", true)]
    [ACPropertyEntity(49, "InwardTargetQuantity", ConstApp.InwardTargetQuantity, "", "", true)]
    [ACPropertyEntity(4, "OutwardQuantity", ConstApp.OutwardQuantity, "", "", true)]
    [ACPropertyEntity(5, "OutwardTargetQuantity", ConstApp.OutwardTargetQuantity, "", "", true)]
    [ACPropertyEntity(6, MDUnit.ClassName, ConstApp.MDUnit, Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(7, "InwardMaterial", "en{'Material (Inward)'}de{'Material (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(8, "InwardFacility", "en{'Storage Bin (Inward)'}de{'Lagerplatz (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9, "InwardFacilityLot", "en{'Lot/Batch Stock (Inward)'}de{'Los/Charge (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + FacilityLot.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(10, "InwardFacilityCharge", "en{'Quant (Inward)'}de{'Quant (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + FacilityCharge.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(11, "InwardFacilityLocation", "en{'Storage Location (Inward)'}de{'Lagerort (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(12, "InwardPartslist", "en{'Inward Bill of Materials'}de{'Eingang Stückliste'}", Const.ContextDatabase + "\\" + Partslist.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(13, "InwardCompanyMaterial", "en{'Material Manufact. (Inward)'}de{'Materialhersteller (Eingang)'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(14, "OutwardMaterial", "en{'Material (Outward)'}de{'Material (Abgangsbuchung)'}", Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(15, "OutwardFacility", "en{'Storage Bin (Outward Posting)'}de{'Lagerplatz (Abgangsbuchung)'}", Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(16, "OutwardFacilityLot", "en{'Lot/Charge (Outward Posting)'}de{'Los/Charge (Abgangsbuchung)'}", Const.ContextDatabase + "\\" + FacilityLot.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(17, "OutwardFacilityCharge", "en{'Quant (Outward Posting)'}de{'Quant (Abgangsbuchung)'}", Const.ContextDatabase + "\\" + FacilityCharge.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(18, "OutwardFacilityLocation", "en{'Storage Location (Outward Posting)'}de{'Lagerort (Abgangsbuchung)'}", Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(19, "OutwardPartslist", "en{'Outward Bill of Materials'}de{'Abgang Stückliste'}", Const.ContextDatabase + "\\" + Partslist.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(20, "OutwardCompanyMaterial", "en{'Material Manufact. (Outward)'}de{'Materialhersteller (Abgang)'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(21, MDBookingNotAvailableMode.ClassName, "en{'Mode zero stock'}de{'Modus bei Nullbestand'}", Const.ContextDatabase + "\\" + MDBookingNotAvailableMode.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(22, "DontAllowNegativeStock", "en{'No negative stocks'}de{'Keine negativen Bestände'}", "", "", true)]
    [ACPropertyEntity(23, "IgnoreManagement", "en{'Ignore management'}de{'Ignoriere Verwaltungskennzeichen'}", "", "", true)]
    [ACPropertyEntity(24, MDBalancingMode.ClassName, "en{'Balancing Mode'}de{'Bilanzierungsmodus'}", Const.ContextDatabase + "\\" + MDBalancingMode.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(25, "QuantityIsAbsolute", "en{'Quantity is absolute'}de{'Menge ist absolut'}", "", "", true)]
    [ACPropertyEntity(26, "ShiftBookingReverse", "en{'Reverse Posting'}de{'Rückbuchung'}", "", "", true)]
    [ACPropertyEntity(27, MDZeroStockState.ClassName, "en{'Set to zero Stock'}de{'Auf Nullbestand setzen'}", Const.ContextDatabase + "\\" + MDZeroStockState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(28, MDReleaseState.ClassName, ConstApp.ESReleaseState, Const.ContextDatabase + "\\" + MDReleaseState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(29, "SetCompleted", ConstApp.SetCompleted, "", "", true)]
    [ACPropertyEntity(30, MDReservationMode.ClassName, "en{'Reservationmode'}de{'Reservierungsmodus'}", Const.ContextDatabase + "\\" + MDReservationMode.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(31, MDMovementReason.ClassName, "en{'Movement Reason'}de{'Buchungsgrund'}", Const.ContextDatabase + "\\" + MDMovementReason.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(32, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(33, "ExpirationDate", ConstApp.ExpirationDate, "", "", true)]
    [ACPropertyEntity(34, "ProductionDate", ConstApp.ProductionDate, "", "", true)]
    [ACPropertyEntity(35, "StorageDate", ConstApp.StorageDate, "", "", true)]
    [ACPropertyEntity(36, "StorageLife", ConstApp.StorageLife, "", "", true)]
    [ACPropertyEntity(37, "MinimumDurability", "en{'Minimum Durability'}de{'Mindesthaltbarkeit'}", "", "", true)]
    [ACPropertyEntity(38, "RecipeOrFactoryInfo", "en{'Bill of Materials- Or Factoryinfo'}de{'Stücklisten oder Firmeninfo'}", "", "", true)]
    [ACPropertyEntity(39, "CPartnerCompany", "en{'Contractual partner'}de{'Vertragspartner'}", Const.ContextDatabase + "\\CPartnerCompanyList", "", true)]
    [ACPropertyEntity(40, "BookingMessage", "en{'Posting Message'}de{'Buchungsmeldung'}", "", "", true)]
    [ACPropertyEntity(41, InOrderPos.ClassName, "en{'Inorderpos'}de{'Bestellposition'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(42, OutOrderPos.ClassName, "en{'Outorderpos'}de{'Auftragsposition'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(43, "InwardQuantityAmb", "en{'Inward quantity ambient'}de{'Zugangsbuchung Menge ambient'}", "", "", true)]
    [ACPropertyEntity(44, "InwardTargetQuantityAmb", "en{'Inward target quantity ambient'}de{'Zugangsbuchung Sollmenge ambient'}", "", "", true)]
    [ACPropertyEntity(45, "OutwardQuantityAmb", "en{'Outward quantity ambient'}de{'Abgangsbuchung Menge ambient'}", "", "", true)]
    [ACPropertyEntity(46, "OutwardTargetQuantityAmb", "en{'Outward target quantity ambient'}de{'Abgangsbuchung Sollmenge ambient'}", "", "", true)]
    [ACPropertyEntity(47, ProdOrderPartslistPos.ClassName, "en{'Bill of Materials line'}de{'Stücklistenposition'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(48, ProdOrderPartslistPosRelation.ClassName, "en{'Bill of Materials relation'}de{'Entnahmeposition'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPosRelation.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(49, PickingPos.ClassName, "en{'Picking line'}de{'Kommissionierposition'}", Const.ContextDatabase + "\\" + PickingPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(50, "MaterialProcessStateIndex", "en{'Material process state'}de{'Materialverarbeitungsstatus'}", typeof(GlobalApp.MaterialProcessState), Const.ContextDatabase + "\\MaterialProcessStateList", "", true)]
    [ACPropertyEntity(51, "PropertyACUrl", "en{'ACUrl'}de{'ACUrl'}", "", "", true)]
    [ACPropertyEntity(9999, "InwardHandlingUnit", "en{'Inward Handling Unit'}de{'Eingang Handlingunit'}", "", "", true)]
    [ACPropertyEntity(9999, "InwardXMLIdentification", "en{'Inward XMLIdentification'}de{'Eingang XMLIdentification'}")]
    [ACPropertyEntity(9999, "OutwardHandlingUnit", "en{'Outward Handling Unit'}de{'Ausgang Handlingunit'}", "", "", true)]
    [ACPropertyEntity(9999, "OutwardXMLIdentification", "en{'Outward XMLIdentification'}de{'Ausgang XMLIdentification'}")]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + FacilityBooking.ClassName, "en{'Stock Movement'}de{'Lagerbewegung'}", typeof(FacilityBooking), FacilityBooking.ClassName, "FacilityBookingNo", "FacilityBookingNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<FacilityBooking>) })]
    public partial class FacilityBooking
    {
        [NotMapped]
        public const string ClassName = "FacilityBooking";
        [NotMapped]
        public const string NoColumnName = "FacilityBookingNo";
        [NotMapped]
        public const string FormatNewNo = "FB{0}";


        #region New/Delete
        public static FacilityBooking NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            FacilityBooking entity = new FacilityBooking();
            entity.FacilityBookingID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            //entity.MDFacilityBookingType = MDFacilityBookingType.DefaultMDFacilityBookingType(database);
            entity.MDMovementReason = MDMovementReason.DefaultMDMovementReason(dbApp);
            //entity.MDQuantityUnit = MDUnit.DefaultMDQuantityUnit(database);
            entity.FacilityBookingNo = secondaryKey;
            entity.MaterialProcessState = GlobalApp.MaterialProcessState.New;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        public void CopyFrom(FacilityBooking from)
        {
            //this.FacilityBookingID = from.FacilityBookingID;
            this.FacilityBookingTypeIndex = from.FacilityBookingTypeIndex;
            this.BookingSucceeded = from.BookingSucceeded;
            //this.FacilityBookingNo = from.FacilityBookingNo;
            this.DontAllowNegativeStock = from.DontAllowNegativeStock;
            this.VBiStackCalculatorACClassID = from.VBiStackCalculatorACClassID;
            this.IgnoreManagement = from.IgnoreManagement;
            this.QuantityIsAbsolute = from.QuantityIsAbsolute;
            this.ShiftBookingReverse = from.ShiftBookingReverse;
            this.MDReleaseStateID = from.MDReleaseStateID;
            this.MDZeroStockStateID = from.MDZeroStockStateID;
            this.MDBookingNotAvailableModeID = from.MDBookingNotAvailableModeID;
            this.MDBalancingModeID = from.MDBalancingModeID;
            this.MDReservationModeID = from.MDReservationModeID;
            this.SetCompleted = from.SetCompleted;
            this.NoInwardOutwardBalancing = from.NoInwardOutwardBalancing;
            this.MDMovementReasonID = from.MDMovementReasonID;
            this.InwardMaterialID = from.InwardMaterialID;
            this.InwardHandlingUnit = from.InwardHandlingUnit;
            this.InwardFacilityID = from.InwardFacilityID;
            this.InwardFacilityLotID = from.InwardFacilityLotID;
            this.InwardFacilityChargeID = from.InwardFacilityChargeID;
            this.InwardFacilityLocationID = from.InwardFacilityLocationID;
            this.InwardPartslistID = from.InwardPartslistID;
            this.InwardCompanyMaterialID = from.InwardCompanyMaterialID;
            this.InwardXMLIdentification = from.InwardXMLIdentification;
            this.InwardQuantity = from.InwardQuantity;
            this.InwardTargetQuantity = from.InwardTargetQuantity;
            this.OutwardMaterialID = from.OutwardMaterialID;
            this.OutwardHandlingUnit = from.OutwardHandlingUnit;
            this.OutwardFacilityID = from.OutwardFacilityID;
            this.OutwardFacilityLotID = from.OutwardFacilityLotID;
            this.OutwardFacilityChargeID = from.OutwardFacilityChargeID;
            this.OutwardFacilityLocationID = from.OutwardFacilityLocationID;
            this.OutwardPartslistID = from.OutwardPartslistID;
            this.OutwardCompanyMaterialID = from.OutwardCompanyMaterialID;
            this.OutwardXMLIdentification = from.OutwardXMLIdentification;
            this.OutwardQuantity = from.OutwardQuantity;
            this.OutwardTargetQuantity = from.OutwardTargetQuantity;
            this.MDUnitID = from.MDUnitID;
            this.InOrderPosID = from.InOrderPosID;
            this.OutOrderPosID = from.OutOrderPosID;
            this.CPartnerCompanyID = from.CPartnerCompanyID;
            this.ProdOrderPartslistPosID = from.ProdOrderPartslistPosID;
            this.ProdOrderPartslistPosRelationID = from.ProdOrderPartslistPosRelationID;
            this.PickingPosID = from.PickingPosID;
            this.PropertyACUrl = from.PropertyACUrl;
            this.StorageDate = from.StorageDate;
            this.StorageLife = from.StorageLife;
            this.ProductionDate = from.ProductionDate;
            this.ExpirationDate = from.ExpirationDate;
            this.MinimumDurability = from.MinimumDurability;
            this.Comment = from.Comment;
            this.RecipeOrFactoryInfo = from.RecipeOrFactoryInfo;
            this.HistoryID = from.HistoryID;
            this.XMLConfig = from.XMLConfig;
            this.BookingMessage = from.BookingMessage;
            this.InsertName = from.InsertName;
            this.InsertDate = from.InsertDate;
            this.InwardQuantityAmb = from.InwardQuantityAmb;
            this.InwardTargetQuantityAmb = from.InwardTargetQuantityAmb;
            this.OutwardQuantityAmb = from.OutwardQuantityAmb;
            this.OutwardTargetQuantityAmb = from.OutwardTargetQuantityAmb;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            string tmpString = FacilityBookingNo + "/";

            string facilityLotNo = "-";
            if (InwardFacilityChargeID != null && InwardFacilityCharge.FacilityLotID != null)
                facilityLotNo = InwardFacilityCharge.FacilityLot.LotNo;
            if (OutwardFacilityChargeID != null && OutwardFacilityCharge.FacilityLotID != null)
                facilityLotNo = OutwardFacilityCharge.FacilityLot.LotNo;
            tmpString += facilityLotNo + "/";

            if (InwardFacilityID != null)
                tmpString += InwardFacility.ToString() + "/";
            if (OutwardFacilityID != null)
                tmpString += OutwardFacility.ToString() + "/";

            if (ProdOrderPartslistPosID != null)
            {
                tmpString += Environment.NewLine;
                tmpString += ProdOrderPartslistPos.ToString() + "/";
            }

            if (ProdOrderPartslistPosRelationID != null)
            {
                tmpString += Environment.NewLine;
                tmpString += ProdOrderPartslistPosRelation.ToString() + "/";
            }

            if (InOrderPosID != null)
            {
                tmpString += Environment.NewLine;
                tmpString += InOrderPos.ToString() + "/";
            }

            return tmpString;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                if (InwardMaterial != null)
                    return FacilityBookingNo + " " + InwardMaterial.ACCaption;
                if (OutwardMaterial != null)
                    return FacilityBookingNo + " " + OutwardMaterial.ACCaption;
                return FacilityBookingNo;
            }
        }

        /// <summary>
        /// Returns a related EF-Object which is in a Child-Relationship to this.
        /// THREAD-SAFE (QueryLock_1X000)
        /// </summary>
        /// <param name="className">Classname of the Table/EF-Object</param>
        /// <param name="filterValues">Search-Parameters</param>
        /// <returns>A Entity-Object as IACObject</returns>
        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && className == FacilityBookingCharge.ClassName)
            {
                return this.FacilityBookingCharge_FacilityBooking.Where(c => c.FacilityBookingChargeNo == filterValues[0]).FirstOrDefault();
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
            if (string.IsNullOrEmpty(FacilityBookingNo))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "FacilityBookingNo",
                    Message = "FacilityBookingNo is null",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "FacilityBookingNo"), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }

        // Buchungssätze werden nur einmalig eingefügt und danach nie
        // mehr geändert
        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "FacilityBookingNo";
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

        /// <summary>
        /// Gets or sets the FacilityBookingType
        /// </summary>
        /// <value>Dimension</value>
        [NotMapped]
        public GlobalApp.FacilityBookingType FacilityBookingType
        {
            get
            {
                return (GlobalApp.FacilityBookingType)FacilityBookingTypeIndex;
            }
            set
            {
                FacilityBookingTypeIndex = (Int16)value;
            }
        }

        [NotMapped]
        public GlobalApp.MaterialProcessState MaterialProcessState
        {
            get
            {
                return (GlobalApp.MaterialProcessState)MaterialProcessStateIndex;
            }
            set
            {
                MaterialProcessStateIndex = (Int16)value;
            }
        }


        [NotMapped]
        FacilityStock _InwardFacilityStock;
        [NotMapped]
        public FacilityStock InwardFacilityStock
        {
            get
            {
                return _InwardFacilityStock;
            }
            set
            {
                _InwardFacilityStock = value;
            }
        }

        [NotMapped]
        MaterialStock _InwardMaterialStock;
        [NotMapped]
        public MaterialStock InwardMaterialStock
        {
            get
            {
                return _InwardMaterialStock;
            }
            set
            {
                _InwardMaterialStock = value;
            }
        }

        [NotMapped]
        FacilityLotStock _InwardFacilityLotStock;
        [NotMapped]
        public FacilityLotStock InwardFacilityLotStock
        {
            get
            {
                return _InwardFacilityLotStock;
            }
            set
            {
                _InwardFacilityLotStock = value;
            }
        }

        [NotMapped]
        PartslistStock _InwardPartslistStock;
        [NotMapped]
        public PartslistStock InwardPartslistStock
        {
            get
            {
                return _InwardPartslistStock;
            }
            set
            {
                _InwardPartslistStock = value;
            }
        }

        [NotMapped]
        CompanyMaterialStock _InwardCompanyMaterialStock;
        [NotMapped]
        public CompanyMaterialStock InwardCompanyMaterialStock
        {
            get
            {
                return _InwardCompanyMaterialStock;
            }
            set
            {
                _InwardCompanyMaterialStock = value;
            }
        }

        [NotMapped]
        CompanyMaterialStock _InwardCPartnerCompMatStock;
        [NotMapped]
        public CompanyMaterialStock InwardCPartnerCompMatStock
        {
            get
            {
                return _InwardCPartnerCompMatStock;
            }
            set
            {
                _InwardCPartnerCompMatStock = value;
            }
        }


        [NotMapped]
        FacilityStock _OutwardFacilityStock;
        [NotMapped]
        public FacilityStock OutwardFacilityStock
        {
            get
            {
                return _OutwardFacilityStock;
            }
            set
            {
                _OutwardFacilityStock = value;
            }
        }

        [NotMapped]
        MaterialStock _OutwardMaterialStock;
        [NotMapped]
        public MaterialStock OutwardMaterialStock
        {
            get
            {
                return _OutwardMaterialStock;
            }
            set
            {
                _OutwardMaterialStock = value;
            }
        }

        [NotMapped]
        FacilityLotStock _OutwardFacilityLotStock;
        [NotMapped]
        public FacilityLotStock OutwardFacilityLotStock
        {
            get
            {
                return _OutwardFacilityLotStock;
            }
            set
            {
                _OutwardFacilityLotStock = value;
            }
        }

        [NotMapped]
        PartslistStock _OutwardPartslistStock;
        [NotMapped]
        public PartslistStock OutwardPartslistStock
        {
            get
            {
                return _OutwardPartslistStock;
            }
            set
            {
                _OutwardPartslistStock = value;
            }
        }

        [NotMapped]
        CompanyMaterialStock _OutwardCompanyMaterialStock;
        [NotMapped]
        public CompanyMaterialStock OutwardCompanyMaterialStock
        {
            get
            {
                return _OutwardCompanyMaterialStock;
            }
            set
            {
                _OutwardCompanyMaterialStock = value;
            }
        }

        [NotMapped]
        CompanyMaterialStock _OutwardCPartnerCompMatStock;
        [NotMapped]
        public CompanyMaterialStock OutwardCPartnerCompMatStock
        {
            get
            {
                return _OutwardCPartnerCompMatStock;
            }
            set
            {
                _OutwardCPartnerCompMatStock = value;
            }
        }

        /// <summary>
        /// Is Booking without reference to a Businessprocess
        /// e.g. Zero-Stock-Bookings, Stock-Corrections or Inventory-Bookings
        /// </summary>
        [NotMapped]
        public bool IsAdjustmentBooking
        {
            get
            {
                return !this.InOrderPosID.HasValue
                    && !this.OutOrderPosID.HasValue
                    && !this.PickingPosID.HasValue
                    && !this.ProdOrderPartslistPosID.HasValue
                    && !this.ProdOrderPartslistPosRelationID.HasValue;
            }
        }

        [NotMapped]
        private gip.core.datamodel.ACClass _StackCalculatorACClass;
        [ACPropertyInfo(9999, "", "en{'Stack Posting Type'}de{'Stapelbuchungsart'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClass.ClassName)]
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

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName == nameof(VBiStackCalculatorACClassID))
            {
                base.OnPropertyChanged("StackCalculatorACClass");
            }
            base.OnPropertyChanged(propertyName);
        }

    }
}
