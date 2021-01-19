using System;

namespace gip.mes.facility.TandTv3
{
    public class MixPointRelation
    {
        public TandTv3Point SourceMixPoint { get; set; }
        public TandTv3Point TargetMixPoint { get; set; }

        public override string ToString()
        {
            return string.Format(@"{0} => {1}", SourceMixPoint, TargetMixPoint);
        }
    }
}
