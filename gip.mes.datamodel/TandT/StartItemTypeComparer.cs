using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    public class StartItemTypeComparer : IComparer<MDTrackingStartItemTypeEnum>
    {
        public int Compare(MDTrackingStartItemTypeEnum a, MDTrackingStartItemTypeEnum b)
        {
            return ((short)a) - ((short)b);
        }
    }
}
