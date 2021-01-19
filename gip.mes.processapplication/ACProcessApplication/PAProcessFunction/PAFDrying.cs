using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Drying'}de{'Trocknen'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWDrying.PWClassName, true)]
    public class PAFDrying : PAProcessFunction
    {
       
        #region Constructors

        static PAFDrying()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFDrying), ACStateConst.TMStart, CreateVirtualMethod("Drying", "en{'Drying'}de{'Trocknen'}", typeof(PWDrying)));
            RegisterExecuteHandler(typeof(PAFDrying), HandleExecuteACMethod_PAFDrying);
        }

        public PAFDrying(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #endregion 

        #region Public 

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAFDrying(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        #endregion

        #region Private

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            //Method.ParameterValueList.Add(new ACValue("Temperature", typeof(Double), 0.0, Global.ParamOption.Required));

            return new ACMethodWrapper(method, captionTranslation, pwClass);
        }

        #endregion

    }

}
