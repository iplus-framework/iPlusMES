using gip.core.datamodel;
using System;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'ProductionOpenPositions'}de{'ProductionOpenPositions'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class ProductionOpenPosition
    {
        public Guid ProdOrderPartslistPosID { get; set; }

        [ACPropertyInfo(100, "Sn", "en{'Sn'}de{'Sn'}")]
        public int Sn { get; set; }

        #region Position

        [ACPropertyInfo(101, "ProgramNo", "en{'Ordernumber'}de{'Auftragsnummer'}")]
        public string ProgramNo { get; set; }

        [ACPropertyInfo(102, "PartslistSequence", "en{'Partslist Sequence'}de{'Stückliste Reihenfolge'}")]
        public int PartslistSequence { get; set; }

        [ACPropertyInfo(103, "ParentPosSequence", "en{'Parent Sequence'}de{'Unterposition Reihenfolge'}")]
        public int ParentPosSequence { get; set; }

        [ACPropertyInfo(104, "PosSequence", "en{'Position Sequence'}de{'Komp. Reihenfolge'}")]
        public int PosSequence { get; set; }

        [ACPropertyInfo(105, "BatchNo", "en{'Batch sequence'}de{'Batchfolgenummer'}")]
        public string BatchNo { get; set; }

        [ACPropertyInfo(106, "InsertDate", "en{'Insert date'}de{'Anlegedatum'}")]
        public DateTime InsertDate { get; set; }

        #endregion

        #region Outward

        [ACPropertyInfo(107, "OutwardMaterialNo", "en{'Outward MaterialNo'}de{'Ein. MaterialNo'}")]
        public string OutwardMaterialNo { get; set; }

        [ACPropertyInfo(108, "OutwardMaterialName", "en{'Outward Name'}de{'Ein. Name'}")]
        public string OutwardMaterialName { get; set; }

        [ACPropertyInfo(110, "OutwardTargetQuantityUOM", "en{'Outward Target Qty(UOM)'}de{'Abgangsmenge Soll(BME)'}")]
        public double OutwardTargetQuantityUOM { get; set; }

        [ACPropertyInfo(112, "en{'Output Actual Quantity'}de{'Einsatz-Ist'}")]
        public double OutwardActualQuantityUOM { get; set; }

        [ACPropertyInfo(113, "OutwardPreBookingCount", "en{'Outward PreBook. Cnt.'}de{'Ein. Gepl. LagBw. Nm.'}")]
        public int OutwardPreBookingCount { get; set; }

        [ACPropertyInfo(114, "OutwardBookingCount", "en{'Outward Book. Cnt.'}de{'Ein. Lbw. Nm.'}")]
        public int OutwardBookingCount { get; set; }

        #endregion

        #region Inward

        [ACPropertyInfo(115, "MaterialNo", "en{'Inward MaterialNo'}de{'Erg. MaterialNo'}")]
        public string InwardMaterialNo { get; set; }

        [ACPropertyInfo(116, "MaterialName", "en{'Inward Name'}de{'Erg. Name'}")]
        public string InwardMaterialName { get; set; }

        [ACPropertyInfo(117, "InwardTargetQuantityUOM", "en{'Target Inward Qty.'}de{'Ergebnismenge Soll'}")]
        public double InwardTargetQuantityUOM { get; set; }

        [ACPropertyInfo(118, "InwardActualQuantityUOM", "en{'Output Actual Quantity'}de{'Ergebnis-Ist'}")]
        public double InwardActualQuantityUOM { get; set; }

        [ACPropertyInfo(119, "InwardPreBookingCount", "en{'Inward PreBook. Cnt.'}de{'Erg. Gepl. LagBw. Nm.'}")]
        public int InwardPreBookingCount { get; set; }

        [ACPropertyInfo(120, "InwardBookingCount", "en{'Inward Book. Cnt.'}de{'Erg. Lbw. Nm.'}")]
        public int InwardBookingCount { get; set; }

        #endregion

    }
}
