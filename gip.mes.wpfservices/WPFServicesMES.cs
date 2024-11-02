// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.core.wpfservices;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.wpfservices
{
    public class WPFServicesMES : WPFServices
    {
        VBDesignerServiceMES _VBDesignerServiceMES  = new VBDesignerServiceMES();
        public override IVBDesignerService DesignerService { get { return _VBDesignerServiceMES; } }
    }
}
