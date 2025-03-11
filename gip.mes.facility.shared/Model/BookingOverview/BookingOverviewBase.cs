#if NETFRAMEWORK
using gip.core.datamodel;
using gip.mes.datamodel;

#endif
using System;
using System.Runtime.Serialization;

namespace gip.mes.facility
{
    /// <summary>
    /// Model used for Facility Overview (BSOFacilityOverview)
    /// </summary>
#if NETFRAMEWORK
    [ACClassInfo(Const.PackName_VarioFacility, "en{'BookingOverviewBase'}de{'BookingOverviewBase'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
#endif
    [DataContract(Name = "cBOB")]
    public class BookingOverviewBase
    {
        #region common data
#if NETFRAMEWORK
        [ACPropertyInfo(9999, "InsertDate", Const.EntityTransInsertDate)]
#endif
        [DataMember(Name = "IDt")]
        public DateTime InsertDate { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "InsertName", Const.EntityTransInsertName)]
#endif
        [DataMember(Name = "INa")]
        public string InsertName { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "FacilityBookingTypeIndex", "en{'Posting Type'}de{'Buchungsart'}")]
#endif
        [DataMember(Name = "FBTI")]
        public int FacilityBookingTypeIndex { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "FacilityBookingTypeIndexName", "en{'Posting Type'}de{'Buchungsart'}")]
#endif
        [DataMember(Name = "FBTINa")]
        public string FacilityBookingTypeIndexName { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "MDMovementReasonName", "en{'Movement Reason'}de{'Buchungsgrund'}")]
#endif
        [DataMember(Name = "MDMRNa")]
        public string MDMovementReasonName { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "Comment", "en{'Comment'}de{'Kommentar'}")]
#endif
        [DataMember(Name = "CM")]
        public string Comment { get; set; }
        #endregion

        #region outward data

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "OutwardQuantityUOM", "en{'Outward Quantity(UOM)'}de{'Abgangsmenge(BME)'}")]
#endif
        [DataMember(Name = "OQUOM")]
        public double OutwardQuantityUOM { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "OutwardMaterialNo", "en{'O. Material No.'}de{'A. Material-Nr.'}")]
#endif
        [DataMember(Name = "OMNo")]
        public string OutwardMaterialNo { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "OutwardMaterialName", "en{'O. Material Desc. 1'}de{'A. Materialbez. 1'}")]
#endif
        [DataMember(Name = "OMNa")]
        public string OutwardMaterialName { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "OutwardFacilityNo", "en{'O. Storage Bin No.'}de{'A. Lagerplatz-Nr.'}")]
#endif
        [DataMember(Name = "OFNo")]
        public string OutwardFacilityNo { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "OutwardFacilityName", "en{'O. Storage Bin'}de{'A. Lagerplatzname'}")]
#endif
        [DataMember(Name = "OFNa")]
        public string OutwardFacilityName { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(801, "OutwardFacilityChargeExternLotNo", ConstApp.ExternLotNo)]
#endif
        [DataMember(Name = "OFCELNo")]
        public string OutwardFacilityChargeExternLotNo { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(901, "OutwardFacilityChargeExternLotNo2", ConstApp.ExternLotNo2)]
#endif
        [DataMember(Name = "OFCELNo2")]
        public string OutwardFacilityChargeExternLotNo2 { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "OutwardFacilityChargeLotNo", "en{'O. Lot No.'}de{'A. Los-Nr.'}")]
#endif
        [DataMember(Name = "OFCLNo")]
        public string OutwardFacilityChargeLotNo { get; set; }
        #endregion

#if NETFRAMEWORK
        [ACPropertyInfo(903, nameof(OutwardFacilityChargeSplitNo), ConstApp.SplitNo)]
#endif
        [DataMember(Name = "OFCSpNo")]
        public int OutwardFacilityChargeSplitNo { get; set; }

        #region inward data

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "InwardQuantityUOM", "en{'Inward Quantity(UOM)'}de{'Zugangsmenge(BME)'}")]
#endif
        [DataMember(Name = "IQUOM")]
        public double InwardQuantityUOM { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "InwardMaterialNo", "en{'I. Material No.'}de{'Z. Material-Nr.'}")]
#endif
        [DataMember(Name = "IMNo")]
        public string InwardMaterialNo { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "InwardMaterialName", "en{'I. Material Desc. 1'}de{'Z. Materialbez. 1'}")]
#endif
        [DataMember(Name = "IMNa")]
        public string InwardMaterialName { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "InwardFacilityNo", "I. Storage Bin No.'}de{'Z. Lagerplatz-Nr.'}")]
#endif
        [DataMember(Name = "IFNo")]
        public string InwardFacilityNo { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "InwardFacilityName", "en{'I. Storage Bin'}de{'Z. Lagerplatzname'}")]
#endif
        [DataMember(Name = "IFNa")]
        public string InwardFacilityName { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "InwardFacilityChargeLotNo", "en{'I. Lot No.'}de{'Z. Los-Nr.'}")]
#endif
        [DataMember(Name = "IFCLNo")]
        public string InwardFacilityChargeLotNo { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(802, "InwardFacilityChargeExternLotNo", ConstApp.ExternLotNo)]
#endif
        [DataMember(Name = "IFCELNo")]
        public string InwardFacilityChargeExternLotNo { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(902, "InwardFacilityChargeExternLotNo2", ConstApp.ExternLotNo2)]
#endif
        [DataMember(Name = "IFCELNo2")]
        public string InwardFacilityChargeExternLotNo2 { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(903, nameof(InwardFacilityChargeSplitNo), ConstApp.SplitNo)]
#endif
        [DataMember(Name = "IFCSpNo")]
        public int InwardFacilityChargeSplitNo { get; set; }

        #endregion

        #region sum calculation

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "InOutSumUOM", "en{'Sum Quantity(UOM)'}de{'Summe Mengen(BME)'}")]
#endif
        [DataMember(Name = "IOSUOM")]
        public double InOutSumUOM { get; set; }


        private string MergeColumn(string outField, string inField)
        {
            if (String.IsNullOrEmpty(outField))
                return inField;
            else if (String.IsNullOrEmpty(inField))
                return outField;
            else if (outField != inField)
                return outField + ">" + inField;
            return outField;
        }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "MaterialNo", "en{'Material No.'}de{'Material-Nr.'}")]
#endif
        [IgnoreDataMember]
        public string MergedMaterialNo
        {
            get
            {
                return MergeColumn(OutwardMaterialNo, InwardMaterialNo);
            }
        }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "xxx", "en{'Material Desc. 1'}de{'Materialbez. 1'}")]
#endif
        [IgnoreDataMember]
        public string MergedMaterialName
        {
            get
            {
                return MergeColumn(OutwardMaterialName, InwardMaterialName);
            }
        }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "xxx", "en{'Storage Bin No.'}de{'Lagerplatz-Nr.'}")]
#endif
        [IgnoreDataMember]
        public string MergedFacilityNo
        {
            get
            {
                return MergeColumn(OutwardFacilityNo, InwardFacilityNo);
            }
        }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "xxx", "en{'Storage Bin'}de{'Lagerplatzname'}")]
#endif
        [IgnoreDataMember]
        public string MergedFacilityName
        {
            get
            {
                return MergeColumn(OutwardFacilityName, InwardFacilityName);
            }
        }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "xxx", "en{'Lot No.'}de{'Los-Nr.'}")]
#endif
        [IgnoreDataMember]
        public string MergedFacilityChargeLotNo
        {
            get
            {
                return MergeColumn(OutwardFacilityChargeLotNo, OutwardFacilityChargeLotNo);
            }
        }

        public enum QuantitySign : short
        {
            /// <summary>
            /// Relocaton
            /// </summary>
            None = 0,

            /// <summary>
            /// Outward
            /// </summary>
            Subtraction = 1,

            /// <summary>
            /// Inward
            /// </summary>
            Addition = 2,

        }

        [IgnoreDataMember]
        public QuantitySign QSign
        {
            get
            {
                if (Math.Abs(OutwardQuantityUOM - 0) <= Double.Epsilon)
                    return QuantitySign.Addition;
                else if (Math.Abs(InwardQuantityUOM - 0) <= Double.Epsilon)
                    return QuantitySign.Subtraction;
                return QuantitySign.None;
            }
        }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "xxx", "en{'Sign'}de{'Vorzeichen'}")]
#endif
        [IgnoreDataMember]
        public string Sign
        {
            get
            {
                if (QSign == QuantitySign.None)
                    return "#";
                else if (QSign == QuantitySign.Addition)
                    return "+";
                else
                    return "-";
            }
        }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "Quantity", "en{'Quantity(UOM)'}de{'Menge(BME)'}")]
#endif
        [IgnoreDataMember]
        public double MergedQuantityUOM
        {
            get
            {
                if (QSign == QuantitySign.None)
                    return Math.Abs(OutwardQuantityUOM);
                return OutwardQuantityUOM + InwardQuantityUOM;
            }
        }
        #endregion

        #region Connection to ProdOrder, DeliveryNote and In/OutOrder
#if NETFRAMEWORK
        [ACPropertyInfo(9999, "InwardFacilityChargeInOrderNo", "en{'Purchase-O. No.'}de{'Bestell-Nr.'}")]
#endif
        [DataMember(Name = "IFCIONo")]
        public string InwardFacilityChargeInOrderNo { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "InwardFacilityChargeProdOrderProgramNo", "en{'Order No.'}de{'Auftrags-Nr.'}")]
#endif
        [DataMember(Name = "IFCPOPNo")]
        public string InwardFacilityChargeProdOrderProgramNo { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "DeliveryNoteNo", "en{'Deliverynote No.'}de{'Lieferschein-Nr.'}")]
#endif
        [DataMember(Name = "DNNo")]
        public string DeliveryNoteNo { get; set; }

#if NETFRAMEWORK
        [ACPropertyInfo(9999, "PickingNo", "en{'Picking-No.'}de{'Kommissions-Nr.'}")]
#endif
        [DataMember(Name = "PKNo")]
        public string PickingNo { get; set; }

        #endregion

    }
}
