using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using System.ComponentModel;
using gip.core.datamodel;
using gip.core.autocomponent;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Mixing'}de{'Mischen'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWMixing.PWClassName, true)]
    public class PAFMixing : PAProcessFunction, IPAFSwitchable
    {
        #region Constructors

        static PAFMixing()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFMixing), ACStateConst.TMStart, CreateVirtualMethod("Mixing", "en{'Mixing On/Off'}de{'Mischen Ein/Aus'}", typeof(PWMixing)));
            ACMethod.RegisterVirtualMethod(typeof(PAFMixing), ACStateConst.TMStart, CreateVirtualTimeMethod("MixingTime", "en{'Mixing Time'}de{'Mischen Zeit'}", typeof(PWMixing)));
            ACMethod.RegisterVirtualMethod(typeof(PAFMixing), ACStateConst.TMStart, CreateVirtualTemperatureMethod("MixingTemperature", "en{'Mixing Temp.'}de{'Mischen Temp.'}", typeof(PWMixing)));
            RegisterExecuteHandler(typeof(PAFMixing), HandleExecuteACMethod_PAFMixing);
        }

        public PAFMixing(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #endregion 

        #region Public 

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PAFMixing(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
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
            method.ParameterValueList.Add(new ACValue("SwitchOff", typeof(bool), 0, Global.ParamOption.Optional));
            paramTranslation.Add("SwitchOff", "en{'Switch off'}de{'Ausschalten'}");
            method.ParameterValueList.Add(new ACValue("Speed", typeof(Int16), 0, Global.ParamOption.Optional));
            paramTranslation.Add("Speed", "en{'Speed'}de{'Geschwindigkeit'}");
            method.ParameterValueList.Add(new ACValue("MinWeight", typeof(Double), 0, Global.ParamOption.Optional));
            paramTranslation.Add("MinWeight", "en{'Minimum weight'}de{'Mindestgewicht'}");
            method.ParameterValueList.Add(new ACValue("Direction", typeof(PAFMixingDirectionEnum), PAFMixingDirectionEnum.Right, Global.ParamOption.Optional));
            paramTranslation.Add("Direction", "en{'Direction'}de{'Richtung'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, null);
        }

        protected static ACMethodWrapper CreateVirtualTimeMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("Duration", typeof(TimeSpan), 0, Global.ParamOption.Required));
            paramTranslation.Add("Duration", "en{'Duration'}de{'Dauer'}");
            method.ParameterValueList.Add(new ACValue("Speed", typeof(Int16), 0, Global.ParamOption.Optional));
            paramTranslation.Add("Speed", "en{'Speed'}de{'Geschwindigkeit'}");
            method.ParameterValueList.Add(new ACValue("MinWeight", typeof(Double), 0, Global.ParamOption.Optional));
            paramTranslation.Add("MinWeight", "en{'Minimum weight'}de{'Mindestgewicht'}");
            method.ParameterValueList.Add(new ACValue("Direction", typeof(PAFMixingDirectionEnum), PAFMixingDirectionEnum.Right, Global.ParamOption.Optional));
            paramTranslation.Add("Direction", "en{'Direction'}de{'Richtung'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ActDuration", typeof(TimeSpan), 0, Global.ParamOption.Required));
            resultTranslation.Add("ActDuration", "en{'Duration'}de{'Dauer'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        protected static ACMethodWrapper CreateVirtualTemperatureMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("Temperature", typeof(Double), 0.0, Global.ParamOption.Required));
            paramTranslation.Add("Temperature", "en{'Temperature'}de{'Temperatur'}");
            method.ParameterValueList.Add(new ACValue("Speed", typeof(Int16), 0, Global.ParamOption.Optional));
            paramTranslation.Add("Speed", "en{'Speed'}de{'Geschwindigkeit'}");
            method.ParameterValueList.Add(new ACValue("MinWeight", typeof(Double), 0, Global.ParamOption.Optional));
            paramTranslation.Add("MinWeight", "en{'Minimum weight'}de{'Mindestgewicht'}");
            method.ParameterValueList.Add(new ACValue("Direction", typeof(PAFMixingDirectionEnum), PAFMixingDirectionEnum.Right, Global.ParamOption.Optional));
            paramTranslation.Add("Direction", "en{'Direction'}de{'Richtung'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ActTemperature", typeof(Double), 0.0, Global.ParamOption.Required));
            resultTranslation.Add("ActTemperature", "en{'Temperature'}de{'Temperatur'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        #endregion

    }

    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ACKind = Global.ACKinds.TACEnum)]
    public enum PAFMixingDirectionEnum : short
    {
        [EnumMember]
        Right = 0,
        [EnumMember]
        Left = 1,
        [EnumMember]
        Interval = 2
    }
}
