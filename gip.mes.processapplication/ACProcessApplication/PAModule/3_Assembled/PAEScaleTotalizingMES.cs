// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.core.processapplication;
using gip.core.autocomponent;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scale totalizing'}de{'Waage totalisierend (SWT)'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAEScaleTotalizingMES : PAEScaleTotalizing
    {
        static PAEScaleTotalizingMES()
        {
            RegisterExecuteHandler(typeof(PAEScaleTotalizingMES), HandleExecuteACMethod_PAEScaleTotalizingMES);
        }

        public PAEScaleTotalizingMES(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #region Handle execute helpers
        public static bool HandleExecuteACMethod_PAEScaleTotalizingMES(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAEScaleTotalizing(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            //switch (acMethodName)
            //{
            //}
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        public override void SimulateAlibi()
        {
            PAEScaleCalibratableMES.SimulateAlibiStatic(this);
        }


        public override Msg SaveAlibiWeighing(PAOrderInfoEntry entity = null)
        {
            return PAEScaleCalibratableMES.SaveAlibiWeighingStatic(this, entity);
        }
    }
}
