using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'BatchQuantityModel'}de{'BatchQuantityModel'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class BatchQuantityModel
    {
        [ACPropertyInfo(1, "Sequence", "en{'Sequence'}de{'Reihenfolge'}")]
        public int Sequence { get; set; }

        [ACPropertyInfo(2, "TargetQuantity", ConstApp.TargetQuantity)]
        public double TargetQuantity { get; set; }
    }
}
