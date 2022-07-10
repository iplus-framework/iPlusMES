using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'InputOverview'}de{'InputOverview.'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]

    public class InputOverview
    {

        #region General

        [ACPropertyInfo(101, "General", "en{'MaterialNo'}de{'MaterialNo.'}")]
        public string MaterialNo { get; set; }

        [ACPropertyInfo(102, "General", "en{'Material name'}de{'Materialname'}")]
        public string MaterialName { get; set; }

        [ACPropertyInfo(103, "General", ConstApp.MDUnit)]
        public string MDUnitName { get; set; }

        #endregion


        #region Quantities

        [ACPropertyInfo(403, "Quantities", "en{'Planned (P)'}de{'Planned (P)'}")]
        public double PlannedQuantityUOM { get; set; }

        [ACPropertyInfo(403, "Quantities", "en{'Target (T)'}de{'Soll (S)'}")]
        public double TargetQuantityUOM { get; set; }

        [ACPropertyInfo(403, "Quantities", "en{'Actual (A)'}de{'Ist (I)'}")]
        public double ActualQuantityUOM { get; set; }


        [ACPropertyInfo(403, "Quantities", "en{'Diff = A - T'}de{'Diff = I - S'}")]
        public double DiffQuantityUOM { get; set; }

        [ACPropertyInfo(403, "Quantities", "en{'Actual (A)'}de{'Ist (I)'}")]
        public double ZeroPostingQuantityUOM { get; set; }

        [ACPropertyInfo(403, "Quantities", "en{'Diff = T - P'}de{'Diff = S - P'}")]
        public double DiffTargetPlanedQuantityUOM { get; set; }

        #endregion

        #region Methods


        public void CalculateDiff()
        {
            DiffQuantityUOM = ActualQuantityUOM - TargetQuantityUOM;
            DiffTargetPlanedQuantityUOM = TargetQuantityUOM - PlannedQuantityUOM;
        }
        #endregion
    }
}
