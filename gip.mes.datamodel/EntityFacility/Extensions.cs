using System;

namespace gip.mes.datamodel
{
    public static class Extensions
    {

        public static double RoundQuantity(this double value, int digits, string mdUnitName, string mdUnitsForRounding = null)
        {
            double result = value;
            if(mdUnitsForRounding != null && mdUnitsForRounding.Contains(mdUnitName))
            {
                result = Math.Round(value, digits);
            }
            return result;
        }
    }
}
