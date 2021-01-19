namespace gip.mes.facility
{
    public class SchedulingDurationAVG
    {
        public double? StartOffsetSecAVG { get; set; }
        public double? DurationSecAVG { get; set; }

        public override string ToString()
        {
            return string.Format(@"Duration | Offset: [{0}|{1}]", DurationSecAVG, StartOffsetSecAVG);
        }
    }
}
