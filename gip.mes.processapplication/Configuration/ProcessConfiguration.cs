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
                return (bool)this["DeactivateProcessConverter"];
            }
            set
            {
                this["DeactivateProcessConverter"] = value;
            }
        }
    }
}
