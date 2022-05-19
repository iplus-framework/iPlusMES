using gip.core.datamodel;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'OverviewMaterial'}de{'OverviewMaterial.'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class OverviewMaterial
    {
        [ACPropertyInfo(101, "MaterialNo", "en{'MaterialNo'}de{'MaterialNo.'}")]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(102, "MaterialName", "en{'Material name'}de{'Materialname'}")]
        public string MaterialName { get; set; }

        [ACPropertyInfo(103, "SumOutwardTargetQuantityUOM", "en{'Target Outward'}de{'Soll Zugang'}")]
        public double SumOutwardTargetQuantityUOM { get; set; }

        [ACPropertyInfo(104, "SumOutwardActualQuantityUOM", "en{'Actual Outward'}de{'Ist Zugang'}")]
        public double SumOutwardActualQuantityUOM { get; set; }

        [ACPropertyInfo(105, "DifferenceOutwardQuantityUOM", "en{'Diff. Outwards'}de{'Differenz Zugang'}")]
        public double DifferenceOutwardQuantityUOM { get; set; }

        [ACPropertyInfo(106, "SumInwardTargetQuantityUOM", "en{'Target Inward'}de{'Soll Abgang'}")]
        public double SumInwardTargetQuantityUOM { get; set; }

        [ACPropertyInfo(107, "SumInwardActualQuantityUOM", "en{'Actual Inward'}de{'Ist Abgang'}")]
        public double SumInwardActualQuantityUOM { get; set; }

        [ACPropertyInfo(108, "DifferenceInwardQuantityUOM", "en{'Diff. Inwards'}de{'Differenz Abgang'}")]
        public double DifferenceInwardQuantityUOM { get; set; }

        [ACPropertyInfo(109, "RestQuantityUOM", "en{'Diff. Stock'}de{'Differenz Bestand'}")]
        public double RestQuantityUOM { get; set; }
    }
}
