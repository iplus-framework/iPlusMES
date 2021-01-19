﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    public interface IPartslistPos : ITargetQuantityUOM
    {
        Material Material
        {
            get;
            set;
        }

        string MaterialNo
        {
            get;
        }

        string MaterialName
        {
            get;
        }

        IEnumerable<IPartslistPosRelation> I_PartslistPosRelation_TargetPartslistPos
        {
            get;
        }

        IEnumerable<IPartslistPosRelation> I_PartslistPosRelation_SourcePartslistPos
        {
            get;
        }
    }
}
