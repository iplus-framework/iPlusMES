// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Tank'}de{'Tank'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMTank : PAMSilo
    {
        static PAMTank()
        {
            RegisterExecuteHandler(typeof(PAMTank), HandleExecuteACMethod_PAMTank);
            RegisterExecuteHandlerAsync(typeof(PAMTank), HandleExecuteACMethodAsync_PAMTank);
        }

        public PAMTank(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMTank(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAMSilo(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        public static async Task<object> HandleExecuteACMethodAsync_PAMTank(IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return await HandleExecuteACMethodAsync_PAMSilo(acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

    }
}
