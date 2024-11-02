// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Work center selector BSO base'}de{'Work center selector BSO base'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, "", true)]
    public abstract class PAFWorkCenterSelItemBase : PAProcessFunction
    {
        static PAFWorkCenterSelItemBase()
        {
            RegisterExecuteHandler(typeof(PAFWorkCenterSelItemBase), HandleExecuteACMethod_PAFWorkCenterSelItemBase);
        }

        public PAFWorkCenterSelItemBase(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        [ACPropertyBindingSource]
        public IACContainerTNet<bool> NeedWork
        {
            get;
            set;
        }

        public static bool HandleExecuteACMethod_PAFWorkCenterSelItemBase(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
    }
}
