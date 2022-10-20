using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'PlanningTargetStockPreview'}de{'PlanningTargetStockPreview'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class PlanningTargetStockPreview
    {

        [ACPropertyInfo(1, "", ConstApp.FacilityNo)]
        public string FacilityNo { get; set; }

        [ACPropertyInfo(2, "", ConstApp.Name)]
        public string FacilityName { get; set; }

        [ACPropertyInfo(3, "", ConstApp.StockQuantity)]
        public double ActualStockQuantity { get; set; }

        [ACPropertyInfo(4, "", ConstApp.OptStockQuantity)]
        public double? OptStockQuantity { get; set; }

        [ACPropertyInfo(5, "", "en{'Ordered quantity'}de{'Bestellte Menge'}")]
        public double OrderedQuantity { get; set; }

        [ACPropertyInfo(6, "", "en{'Planned quantity'}de{'Geplante Menge'}")]
        public double NewPlannedStockQuantity { get; set; }

        [ACPropertyInfo(7, "", "en{'In range'}de{'In Range'}")]
        public int IsInRange { get; set; }

        [ACPropertyInfo(8, "", ConstApp.PickingType)]
        public MDPickingType MDPickingType { get; set; }

        public Facility Facility { get; set; }
    }
}
