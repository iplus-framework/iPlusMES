using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    public interface IPartslistPosRelation : ITargetQuantityUOM
    {
        IPartslistPos I_SourcePartslistPos
        {
            get;
        }

        IPartslistPos I_TargetPartslistPos
        {
            get;
        }
    }
}
