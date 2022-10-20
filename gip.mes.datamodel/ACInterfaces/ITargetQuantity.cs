using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    public interface ITargetQuantity
    {
        double TargetQuantity { get; set; }
    }

    public interface ITargetQuantityUOM : ITargetQuantity
    {
        double TargetQuantityUOM { get; set; }
    }
}
