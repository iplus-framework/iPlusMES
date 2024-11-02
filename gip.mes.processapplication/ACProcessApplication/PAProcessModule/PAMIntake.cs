// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System.Linq;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.processapplication;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Intake place'}de{'Annahmestelle'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMIntake : PAMLoadingstation
    {
        public const string SelRuleID_PAMIntake = "PAMIntake";

        static PAMIntake()
        {
            RegisterExecuteHandler(typeof(PAMIntake), HandleExecuteACMethod_PAMIntake);
            ACRoutingService.RegisterSelectionQuery(SelRuleID_PAMIntake, (c, p) => c.ComponentInstance is PAMIntake, (c, p) => c.ComponentInstance is PAProcessModule);
        }

        public PAMIntake(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMIntake(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAMLoadingstation(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region Properties
        //public PAEScaleGravimetric Scale
        //{
        //    get
        //    {
        //        return FindChildComponents<PAEScaleGravimetric>(c => c is PAEScaleGravimetric, null, 1).FirstOrDefault();
        //    }
        //}
        #endregion

    }
}
