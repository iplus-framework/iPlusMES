﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Flowmeter'}de{'Durchflussmesser'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMFlowmeter : PAProcessModuleVB
    {
        static PAMFlowmeter()
        {
            RegisterExecuteHandler(typeof(PAMFlowmeter), HandleExecuteACMethod_PAMFlowmeter);
        }

        public PAMFlowmeter(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _PAPointMatIn1 = new PAPoint(this, nameof(PAPointMatIn1));
            _PAPointMatOut1 = new PAPoint(this, nameof(PAPointMatOut1));
        }

        #region Points
        PAPoint _PAPointMatIn1;
        [ACPropertyConnectionPoint(9999, "PointMaterial")]
        public PAPoint PAPointMatIn1
        {
            get
            {
                return _PAPointMatIn1;
            }
        }

        PAPoint _PAPointMatOut1;
        [ACPropertyConnectionPoint(9999, "PointMaterial")]
        [ACPointStateInfo(GlobalProcApp.AvailabilityStatePropName, GlobalProcApp.AvailabilityState.Idle, GlobalProcApp.AvailabilityStateGroupName, "", Global.Operators.none)]
        public PAPoint PAPointMatOut1
        {
            get
            {
                return _PAPointMatOut1;
            }
        }
        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAMFlowmeter(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessModuleVB(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

    }
}