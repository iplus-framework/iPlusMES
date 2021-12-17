using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [CollectionDataContract(Name = "cPPL")]
    public class PickingPosList : List<PickingPos>
    {
        public PickingPosList() : base ()
        {

        }

        public PickingPosList(IEnumerable<PickingPos> collection) : base (collection)
        {

        }
    }
}
