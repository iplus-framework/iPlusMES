using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

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

        [ACPropertyInfo(403, "Quantities", "en{'Zero posting'}de{'Null Buchung'}")]
        public double ZeroPostingQuantityUOM { get; set; }

        [ACPropertyInfo(403, "Quantities", "en{'Diff = T - P'}de{'Diff = S - P'}")]
        public double DiffTargetPlanedQuantityUOM { get; set; }

        [ACPropertyInfo(500, "", "en{'InputQForActual'}de{'InputQForActual'}")]
        public InputQForActual InputQForActual { get; set; }

        #endregion

        public IEnumerable<ProdOrderPartslistPos> GroupedPos { get; set; }

        #region Methods


        public void CalculateDiff()
        {
            DiffQuantityUOM = ActualQuantityUOM - TargetQuantityUOM;
            DiffTargetPlanedQuantityUOM = TargetQuantityUOM - PlannedQuantityUOM;
        }

        public void CalculateStatistics()
        {
            if(GroupedPos == null)
                return;
            InputQForActual = new InputQForActual()
            {
                InputQForActualOutput = GroupedPos.Select(c => (c.InputQForActualOutput ?? 0)).DefaultIfEmpty().Sum(),
                InputQForActualOutputDiff = GroupedPos.Select(c => c.InputQForActualOutputDiff).DefaultIfEmpty().Sum(),
                InputQForActualOutputPer = GroupedPos.Select(c => c.InputQForActualOutputPer).DefaultIfEmpty().Average(),

                InputQForGoodActualOutput = GroupedPos.Select(c => (c.InputQForGoodActualOutput ?? 0)).DefaultIfEmpty().Sum(),
                InputQForGoodActualOutputDiff = GroupedPos.Select(c => c.InputQForGoodActualOutputDiff).DefaultIfEmpty().Sum(),
                InputQForGoodActualOutputPer = GroupedPos.Select(c => c.InputQForGoodActualOutputPer).DefaultIfEmpty().Average(),

                InputQForScrapActualOutput = GroupedPos.Select(c => (c.InputQForScrapActualOutput ?? 0)).DefaultIfEmpty().Sum(),
                InputQForScrapActualOutputDiff = GroupedPos.Select(c => c.InputQForScrapActualOutputDiff).DefaultIfEmpty().Sum(),
                InputQForScrapActualOutputPer = GroupedPos.Select(c => c.InputQForScrapActualOutputPer).DefaultIfEmpty().Average(),


                InputQForFinalActualOutput = GroupedPos.Select(c => (c.InputQForFinalActualOutput ?? 0)).DefaultIfEmpty().Sum(),
                InputQForFinalActualOutputDiff = GroupedPos.Select(c => c.InputQForFinalActualOutputDiff).DefaultIfEmpty().Sum(),
                InputQForFinalActualOutputPer = GroupedPos.Select(c => c.InputQForFinalActualOutputPer).DefaultIfEmpty().Average(),

                InputQForFinalGoodActualOutput = GroupedPos.Select(c => (c.InputQForFinalGoodActualOutput ?? 0)).DefaultIfEmpty().Sum(),
                InputQForFinalGoodActualOutputDiff = GroupedPos.Select(c => c.InputQForFinalGoodActualOutputDiff).DefaultIfEmpty().Sum(),
                InputQForFinalGoodActualOutputPer = GroupedPos.Select(c => c.InputQForFinalGoodActualOutputPer).DefaultIfEmpty().Average(),

                InputQForFinalScrapActualOutput = GroupedPos.Select(c => (c.InputQForFinalScrapActualOutput ?? 0)).DefaultIfEmpty().Sum(),
                InputQForFinalScrapActualOutputDiff = GroupedPos.Select(c => c.InputQForFinalScrapActualOutputDiff).DefaultIfEmpty().Sum(),
                InputQForFinalScrapActualOutputPer = GroupedPos.Select(c => c.InputQForFinalScrapActualOutputPer).DefaultIfEmpty().Average(),
            };
        }
        #endregion
    }
}
