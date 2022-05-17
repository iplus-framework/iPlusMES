#if NETFRAMEWORK
using gip.core.datamodel;
#endif
using System;
using System.Runtime.Serialization;

namespace gip.mes.facility
{
    public static class FacilityConst
    {
        public const double C_ZeroStockCompare = 0.000000001;
        public const double C_ZeroCompare = double.Epsilon * 10;
        public static bool IsDoubleZeroForPosting(double value)
        {
            return Math.Abs(value) < C_ZeroCompare;
        }

        public const double C_MaxQuantityPerPosting = 100000000;
    }
}
