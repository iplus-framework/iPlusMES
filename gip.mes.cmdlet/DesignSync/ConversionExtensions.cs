using System;

namespace gip.mes.cmdlet.DesignSync
{
    public static class ConversionExtensions
    {
        public static object Convert(this object value, Type t)
        {
            Type underlyingType = Nullable.GetUnderlyingType(t);

            if (underlyingType != null && value == null)
            {
                return null;
            }
            Type basetype = underlyingType == null ? t : underlyingType;
            return System.Convert.ChangeType(value, basetype);
        }

        public static T Convert<T>(this object value)
        {
            return (T)value.Convert(typeof(T));
        }
    }
}
