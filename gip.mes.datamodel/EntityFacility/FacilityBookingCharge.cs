using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    // FacilityBookingCharge 
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Stock Movement of Quant'}de{'Lagerbewegung Quant'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "FacilityBookingChargeNo", "en{'Posting No.'}de{'Buchungsnr.'}", "", "", true)]
    [ACPropertyEntity(2, "FacilityBookingTypeIndex", "en{'Posting Type'}de{'Buchungsart'}", typeof(GlobalApp.FacilityBookingType), Const.ContextDatabase + "\\FacilityBookingTypeList", "", true)]
    [ACPropertyEntity(3, "InwardQuantity", ConstApp.InwardQuantity, "", "", true)]
    [ACPropertyEntity(4, "InwardQuantityUOM", "en{'Inward Quantity(UOM)'}de{'Zugangsmenge(ME)'}", "", "", true)]
    [ACPropertyEntity(5, "InwardTargetQuantity", ConstApp.InwardTargetQuantity, "", "", true)]
    [ACPropertyEntity(6, "InwardTargetQuantityUOM", "en{'Inward Target Qty(UOM)'}de{'Eingangmenge Soll(BME)'}", "", "", true)]
    [ACPropertyEntity(7, "OutwardQuantity", ConstApp.OutwardQuantity, "", "", true)]
    [ACPropertyEntity(8, "OutwardQuantityUOM", "en{'Outward Quantity(UOM)'}de{'Abgangsmenge(BME)'}", "", "", true)]
    [ACPropertyEntity(9, "OutwardTargetQuantity", ConstApp.OutwardTargetQuantity, "", "", true)]
    [ACPropertyEntity(10, "OutwardTargetQuantityUOM", "en{'Outward Target Qty(UOM)'}de{'Abgangsmenge Soll(BME)'}", "", "", true)]
    [ACPropertyEntity(11, MDUnit.ClassName, "en{'Unit of Measurement(UOM)'}de{'Maßeinheit(ME)'}", Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(12, "InwardMaterial", "en{'Material (Inward Post)'}de{'Material (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(13, "InwardFacility", "en{'Storage Bin (Inward post)'}de{'Lagerplatz (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(14, "InwardFacilityLot", "en{'Lot/Batch Stock (Inward Post)'}de{'Los/Charge (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + FacilityLot.ClassName, "", true)]
    [ACPropertyEntity(15, "InwardFacilityCharge", "en{'Quant (Inward Post)'}de{'Quant (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + FacilityCharge.ClassName, "", true)]
    [ACPropertyEntity(16, "InwardFacilityLocation", "en{'Storage Location (Inward Post)'}de{'Lagerort (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(17, "InwardPartslist", "en{'Inward Bill of Materials'}de{'Eingang Stückliste'}", Const.ContextDatabase + "\\" + Partslist.ClassName, "", true)]
    [ACPropertyEntity(18, "InwardCompanyMaterialID", "en{'Manufact. Material (In)'}de{'Materialhersteller (Eing.)'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName, "", true)]
    [ACPropertyEntity(19, "InwardCPartnerCompMatID", "en{'Contr. Partner Material (In)'}de{'Vertragspart. Material (Eing.)'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName, "", true)]
    [ACPropertyEntity(20, "OutwardMaterial", "en{'Material (Outward Post)'}de{'Material (Ausgangsbuchung)'}", Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(21, "OutwardFacility", "en{'Storage Bin (Outward Post)'}de{'Lagerplatz (Ausgangsbuchung)'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(22, "OutwardFacilityLot", "en{'Lot/Charge (Outward Post)'}de{'Los/Charge (Ausgangsbuchung)'}", Const.ContextDatabase + "\\" + FacilityLot.ClassName, "", true)]
    [ACPropertyEntity(23, "OutwardFacilityCharge", "en{'Quant (Outward Post)'}de{'Quant (Ausgangsbuchung)'}", Const.ContextDatabase + "\\" + FacilityCharge.ClassName, "", true)]
    [ACPropertyEntity(24, "OutwardFacilityLocation", "en{'Storage Location (Outward Post)'}de{'Lagerort (Ausgangsbuchung)'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(25, "OutwardPartslist", "en{'Outward Bill of Materials'}de{'Ausgang Stückliste'}", Const.ContextDatabase + "\\" + Partslist.ClassName, "", true)]
    [ACPropertyEntity(26, "OutwardCompanyMaterialID", "en{'Manufact. Material (Out)'}de{'Materialhersteller (Ausg.)'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName, "", true)]
    [ACPropertyEntity(27, "OutwardCPartnerCompMatID", "en{'Contr. Partner Material (Out)'}de{'Vertragspart. Material (Ausg.)'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName, "", true)]
    [ACPropertyEntity(28, MDBookingNotAvailableMode.ClassName, "en{'Mode for Zero Stock'}de{'Modus bei Nullbestand'}", Const.ContextDatabase + "\\" + MDBookingNotAvailableMode.ClassName, "", true)]
    [ACPropertyEntity(29, "DontAllowNegativeStock", "en{'No negative stocks'}de{'Keine negativen Bestände'}", "", "", true)]
    [ACPropertyEntity(30, "IgnoreManagement", "en{'Ignore Management'}de{'Ignoriere Verwaltungskennzeichen'}", "", "", true)]
    [ACPropertyEntity(31, MDBalancingMode.ClassName, "en{'Balancing Mode'}de{'Bilanzierungsmodus'}", Const.ContextDatabase + "\\" + MDBalancingMode.ClassName, "", true)]
    [ACPropertyEntity(32, "QuantityIsAbsolute", "en{'Quantity is absolute'}de{'Menge ist absolut'}", "", "", true)]
    [ACPropertyEntity(33, "ShiftBookingReverse", "en{'Reverse Posting'}de{'Rückbuchung'}", "", "", true)]
    [ACPropertyEntity(34, MDZeroStockState.ClassName, "en{'Set to zero Stock'}de{'Auf Nullbestand setzen'}", Const.ContextDatabase + "\\" + MDZeroStockState.ClassName, "", true)]
    [ACPropertyEntity(35, MDReleaseState.ClassName, ConstApp.ESReleaseState, Const.ContextDatabase + "\\" + MDReleaseState.ClassName, "", true)]
    [ACPropertyEntity(36, "SetCompleted", ConstApp.SetCompleted, "", "", true)]
    [ACPropertyEntity(37, MDReservationMode.ClassName, ConstApp.ESReservationMode, Const.ContextDatabase + "\\" + MDReservationMode.ClassName, "", true)]
    [ACPropertyEntity(38, MDMovementReason.ClassName, ConstApp.ESMovementReason, Const.ContextDatabase + "\\" + MDMovementReason.ClassName, "", true)]
    [ACPropertyEntity(39, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(40, "ExpirationDate", ConstApp.ExpirationDate, "", "", true)]
    [ACPropertyEntity(41, "ProductionDate", ConstApp.ProductionDate, "", "", true)]
    [ACPropertyEntity(42, "StorageDate", ConstApp.StorageDate, "", "", true)]
    [ACPropertyEntity(43, "StorageLife", ConstApp.StorageLife, "", "", true)]
    [ACPropertyEntity(44, "MinimumDurability", "en{'Minimum Durability'}de{'Mindesthaltbarkeit'}", "", "", true)]
    [ACPropertyEntity(45, "RecipeOrFactoryInfo", "en{'Bill of Materials- Or Factoryinfo'}", "", "", true)]
    [ACPropertyEntity(46, CompanyMaterial.ClassName, "en{'Material manufact.'}de{'Materialhersteller'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName, "", true)]
    [ACPropertyEntity(47, InOrderPos.ClassName, "en{'Inorderpos'}de{'Bestellposition'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName, "", true)]
    [ACPropertyEntity(48, OutOrderPos.ClassName, "en{'Outorderpos'}de{'Auftragsposition'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName, "", true)]
    [ACPropertyEntity(49, "BookingMessage", "en{'Posting message'}de{'Buchungsmeldung'}", "", "", true)]
    [ACPropertyEntity(50, "InwardQuantityUOMAmb", "en{'Inward Qty Ambient'}de{'Zugangsbuchung Menge ambient'}", "", "", true)]
    [ACPropertyEntity(51, "InwardTargetQuantityUOMAmb", "en{'Inward Target Qty Ambient'}de{'Zugangsbuchung Sollmenge ambient'}", "", "", true)]
    [ACPropertyEntity(52, "OutwardQuantityUOMAmb", "en{'Outward Qty Ambient'}de{'Abgangsbuchung Menge ambient'}", "", "", true)]
    [ACPropertyEntity(53, "OutwardTargetQuantityUOMAmb", "en{'Outward target quantity ambient'}de{'Ausbuchungs-Sollmenge ambient'}", "", "", true)]
    [ACPropertyEntity(54, ProdOrderPartslistPos.ClassName, "en{'Bill of Materials line'}de{'Stücklistenposition'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName, "", true)]
    [ACPropertyEntity(55, ProdOrderPartslistPosRelation.ClassName, "en{'Bill of Materials relation'}de{'Entnahmeposition'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPosRelation.ClassName, "", true)]
    [ACPropertyEntity(56, PickingPos.ClassName, "en{'Picking line'}de{'Kommissionierposition'}", Const.ContextDatabase + "\\" + PickingPos.ClassName, "", true)]
    [ACPropertyEntity(9999, FacilityBooking.ClassName, "en{'Stock movement'}de{'Lagerbewegung'}", Const.ContextDatabase + "\\" + FacilityBooking.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + FacilityBookingCharge.ClassName, "en{'Facilitybookingcharge'}de{'Lagerbewegung Charge'}", typeof(FacilityBookingCharge), FacilityBookingCharge.ClassName, "FacilityBookingChargeNo", "FacilityBookingChargeNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<FacilityBookingCharge>) })]
    public partial class FacilityBookingCharge
    {
        public const string ClassName = "FacilityBookingCharge";
        public const string NoColumnName = "FacilityBookingChargeNo";
        public const string FormatNewNo = "FBC{0}";

        #region New/Delete
        public static FacilityBookingCharge NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            FacilityBookingCharge entity = new FacilityBookingCharge();
            entity.FacilityBookingChargeID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.FacilityBookingChargeNo = secondaryKey;
            if (parentACObject is FacilityBooking)
            {
                FacilityBooking faciliyBooking = parentACObject as FacilityBooking;
                entity.FacilityBooking = faciliyBooking;
                faciliyBooking.FacilityBookingCharge_FacilityBooking.Add(entity);
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            string tmpString = FacilityBooking.FacilityBookingNo + "/" + FacilityBookingChargeNo + "/";

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
                return FacilityBookingChargeNo;
            }
        }

        /// <summary>
        /// Returns FacilityBooking
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to FacilityBooking</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return FacilityBooking;
            }
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
            if (string.IsNullOrEmpty(FacilityBookingChargeNo))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "FacilityBookingChargeNo",
                    Message = "FacilityBookingChargeNo is null",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "FacilityBookingChargeNo"), 
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
                return "FacilityBookingChargeNo";
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
