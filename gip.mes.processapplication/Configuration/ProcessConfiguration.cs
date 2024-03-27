using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    public class ProcessConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("DeactivateProcessConverter", DefaultValue = false, IsRequired = false)]
        public bool DeactivateProcessConverter
        {
            get
            {
                return (bool)this[nameof(DeactivateProcessConverter)];
            }
            set
            {
                this[nameof(DeactivateProcessConverter)] = value;
            }
        }

        private static ProcessConfiguration _StaticConfig = null;
        public static ProcessConfiguration StaticConfig
        {
            get
            {
                if (_StaticConfig != null)
                    return _StaticConfig;
                try
                {
                    _StaticConfig = (ProcessConfiguration)CommandLineHelper.ConfigCurrentDir.GetSection("Process/ProcessConfiguration");
                }
                catch (Exception)
                {
                }
                return null;
            }
        }
    }
}
