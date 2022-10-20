using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public class TandTPointConnection
    {
        public string GroupConnectionName { get; set; }

        public TandTPoint From { get; set; }
        public TandTPoint To { get; set; }
    }
}
