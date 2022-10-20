using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PWUnload'}de{'PWUnload'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWUnload : PWNodeProcessMethod, IPWNodeDeliverMaterial
    {
        public const string PWClassName = "PWUnload";

        public Route CurrentDischargingRoute { get { return null; } set { } }

        #region c´tors
        static PWUnload()
        {
            RegisterExecuteHandler(typeof(PWUnload), HandleExecuteACMethod_PWUnload);
        }

        public PWUnload(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }
        #endregion

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWUnload(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeProcessMethod(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

    }
}
