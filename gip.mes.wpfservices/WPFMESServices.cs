using gip.core.datamodel;
using gip.core.wpfservices;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.wpfservices
{
    public class WPFMESServices : IWPFMESServices
    {
        VBMESDesignerService _VBMESDesignerService = new VBMESDesignerService();
        public IVBMESDesignerService DesignerMESService { get { return _VBMESDesignerService; } }
    }
}
