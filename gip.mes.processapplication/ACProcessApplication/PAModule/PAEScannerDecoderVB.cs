﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Scan decoder vb'}de{'Scan decoder vb'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAEScannerDecoderVB : PAEScannerDecoder
    {
        static PAEScannerDecoderVB()
        {
            RegisterExecuteHandler(typeof(PAEScannerDecoderVB), HandleExecuteACMethod_PAEScannerDecoderVB);
        }

        public PAEScannerDecoderVB(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #region Handle execute helpers

        public static bool HandleExecuteACMethod_PAEScannerDecoderVB(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAEScannerDecoder(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }

}
