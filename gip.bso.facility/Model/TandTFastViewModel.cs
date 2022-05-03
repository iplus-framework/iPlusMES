using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.bso.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTFastViewModel'}de{'TandTFastViewModel'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]

    public class TandTFastViewModel
    {
        #region Order info
        [ACPropertyInfo(100, "OrderNo", "en{'Program'}de{'AuftragNo.'}")]
        public string OrderNo { get; set; }

        [ACPropertyInfo(101, "ConsumptionActualQuantity", "en{'Consumption Act. Quantity'}de{'Verbrauchsgesetz. Menge'}")]
        public double ConsumptionActualQuantity { get; set; }

        [ACPropertyInfo(102, "ConsMDUnit", ConstApp.MDUnit)]
        public MDUnit ConsMDUnit { get; set; }
        #endregion

        #region ProdOrderPartslist info

        [ACPropertyInfo(200, "MaterialNo", "en{'MaterialNo'}de{'MaterialNo.'}")]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(201, "en{'Material name'}de{'Materialname'}")]
        public string MaterialName { get; set; }

        [ACPropertyInfo(202, "TargetActualQuantityUOM", "en{'Actual Output'}de{'Ist Ergebnis'}")]
        public double TargetActualQuantityUOM { get; set; }

        [ACPropertyInfo(203, "MDUnit", ConstApp.MDUnit)]
        public MDUnit MDUnit { get; set; }
        #endregion

        #region Final product info

        [ACPropertyInfo(300, "FinalMaterialNo", "en{'Final MaterialNo'}de{'Finale MaterialNo.'}")]
        public string FinalMaterialNo { get; set; }

        [ACPropertyInfo(301, "FinalMaterialName", "en{'Final Material name'}de{'Finale Materialname'}")]
        public string FinalMaterialName { get; set; }

        [ACPropertyInfo(302, "FinalTargetActualQuantityUOM", "en{'Final Actual Output'}de{'Finale Ist Ergebnis'}")]
        public double FinalTargetActualQuantityUOM { get; set; }

        [ACPropertyInfo(303, "FinalMDUnit", ConstApp.MDUnit)]
        public MDUnit FinalMDUnit { get; set; }
        #endregion

    }
}
