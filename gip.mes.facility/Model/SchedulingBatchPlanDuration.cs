using System;

namespace gip.mes.facility
{
    public class SchedulingBatchPlanDuration
    {

        public Guid ProdOrderBatchPlanID { get; set; }
        public int BatchTargetCount { get; set; }
        public double BatchSize { get; set; }

        public double? StartOffsetSecAVG { get; set; }
        public double? DurationSecAVG { get; set; }

        public double? StartOffsetSecAVGPerUnit { get; set; }

        public double? DurationSecAVGPerUnit { get; set; }

        public bool IncludeInCalc { get; set; }

        public override string ToString()
        {
            return string.Format(@"[{0}] {1} x {2} Duration | Offset: [{3}|{4}] Duration | Offset (per unit): [{5}|{6}]", 
                IncludeInCalc, BatchTargetCount, BatchSize, DurationSecAVG, StartOffsetSecAVG, DurationSecAVGPerUnit, StartOffsetSecAVGPerUnit);
        }
    }
}
