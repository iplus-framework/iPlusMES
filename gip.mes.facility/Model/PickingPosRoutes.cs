using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public class PickingPosRoutes
    {
        public PickingPos PickingPosition
        {
            get;
            set;
        }

        public IEnumerable<Route> Routes
        {
            get;
            set;
        }
    }
}
