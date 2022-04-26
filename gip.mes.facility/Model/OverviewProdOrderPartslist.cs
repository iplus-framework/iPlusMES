using gip.core.datamodel;

namespace gip.mes.facility
{

    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'OverviewProdOrderPartslist'}de{'OverviewProdOrderPartslist'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class OverviewProdOrderPartslist
    {
        [ACPropertyInfo(100, "OrderNo", "en{'Program'}de{'AuftragNo.'}")]
        public string OrderNo { get; set; }

        [ACPropertyInfo(101, "MaterialNo", "en{'MaterialNo'}de{'MaterialNo.'}")]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(102, "en{'Material name'}de{'Materialname'}")]
        public string MaterialName { get; set; }

        [ACPropertyInfo(103, "TargetInwardQuantityUOM", "en{'Target Output'}de{'Soll Ergebnis'}")]
        public double TargetInwardQuantityUOM { get; set; }

        [ACPropertyInfo(104, "TargetActualQuantityUOM", "en{'Actual Output'}de{'Ist Ergebnis'}")]
        public double TargetActualQuantityUOM { get; set; }

        [ACPropertyInfo(105, "DifferenceQuantityUOM", "en{'Diff. Output'}de{'Diff. Ergebnis'}")]
        public double DifferenceQuantityUOM { get; set; }

        [ACPropertyInfo(106, "SumComponentsActualQuantity", "en{'Sum Inputs'}de{'Summe Einsatz'}")]
        public double SumComponentsActualQuantity { get; set; }

        [ACPropertyInfo(107, "RestQuantityUOM", "en{'Diff. Input/Output'}de{'Differenz Einsatz/Ergebnis'}")]
        public double RestQuantityUOM { get; set; }

    }
}
