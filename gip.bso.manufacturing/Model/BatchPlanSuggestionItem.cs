using gip.core.datamodel;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'BatchPlanSuggestionItem'}de{'BatchPlanSuggestionItem'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class BatchPlanSuggestionItem
    {
        [ACPropertyInfo(100, "Nr", "en{'Batch-No.'}de{'Batch-Nr.'}")]
        public int Nr { get; set; }

        [ACPropertyInfo(200, "BatchTargetCount", "en{'Target Batch Count'}de{'Soll Batchanzahl'}")]
        public int BatchTargetCount { get; set; }

        [ACPropertyInfo(300, "BatchSize", "en{'Batch Size'}de{'Batchgröße'}")]
        public double BatchSize { get; set; }

        [ACPropertyInfo(400, "TotalBatchSize", "en{'Total Size'}de{'Gesamtgröße'}")]
        public double TotalBatchSize { get; set; }

        public override string ToString()
        {
            return string.Format(@"[BatchPlanSuggestion] #{0} {1} x {2} = {3}", Nr, BatchTargetCount, BatchSize, TotalBatchSize);
        }
    }
}
