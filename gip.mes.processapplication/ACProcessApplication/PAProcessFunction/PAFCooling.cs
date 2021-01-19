using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Cooling'}de{'Kühlen'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWCooling.PWClassName, true)]
    public class PAFCooling : PAProcessFunction
    {
      
        #region Constructors

        static PAFCooling()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFCooling), ACStateConst.TMStart, CreateVirtualMethod("Cooling", "en{'Cooling'}de{'Kühlen'}", typeof(PWCooling)));
            RegisterExecuteHandler(typeof(PAFCooling), HandleExecuteACMethod_PAFCooling);
        }

        public PAFCooling(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #endregion 

        #region Public 

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAFCooling(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
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

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue("Temperature", typeof(Double), 0.0, Global.ParamOption.Required));
            paramTranslation.Add("Temperature", "en{'Temperature'}de{'Temperatur'}");

            method.ParameterValueList.Add(new ACValue("FlowTemperature", typeof(Double), 0.0, Global.ParamOption.Optional));
            paramTranslation.Add("FlowTemperature", "en{'Flow Temperature'}de{'Vorlauf Temperatur'}");

            method.ParameterValueList.Add(new ACValue("HoldTemperature", typeof(Int16), 0, Global.ParamOption.Optional));
            paramTranslation.Add("HoldTemperature", "en{'Hold Temperature'}de{'Temperatur halten'}");

            method.ParameterValueList.Add(new ACValue("HoldTime", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            paramTranslation.Add("HoldTime", "en{'Hold Time'}de{'Haltezeit'}");

            method.ParameterValueList.Add(new ACValue("MinWeightSwitchOn", typeof(Double), 0, Global.ParamOption.Optional));
            paramTranslation.Add("MinWeightSwitchOn", "en{'Min. Weight Switch On'}de{'Einschaltgewicht'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ReachedTemperature", typeof(Double), 0.0, Global.ParamOption.Required));
            resultTranslation.Add("ReachedTemperature", "en{'Reached Temperature'}de{'Ist-Temperatur'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        #endregion

    }

}
