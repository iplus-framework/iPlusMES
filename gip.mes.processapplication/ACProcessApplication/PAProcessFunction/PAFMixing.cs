using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using System.ComponentModel;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;

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

        protected override MsgWithDetails CompleteACMethodOnSMStarting(ACMethod acMethod, ACMethod previousParams)
        {
            if (IsSimulationOn)
            {
                foreach (PAEEMotorBase motor in ParentACComponent?.FindChildComponents<PAEEMotorBase>(c => c is PAEEMotorBase))
                {
                    motor.ActivateRouteItemOnSimulation(null, false);
                }
            }

            return base.CompleteACMethodOnSMStarting(acMethod, previousParams);
        }

        protected override CompleteResult AnalyzeACMethodResult(ACMethod acMethod, out MsgWithDetails msg, CompleteResult completeResult)
        {
            if (IsSimulationOn)
            {
                bool switchOff = true;
                ACValue acValue = acMethod.ParameterValueList.GetACValue("LeaveOn");
                if (acValue != null && acValue.ValueT<bool>())
                    switchOff = false;
                if (switchOff)
                {
                    acValue = acMethod.ParameterValueList.GetACValue("SwitchOff");
                    if (acValue == null || !acValue.ValueT<bool>())
                    {
                        acValue = acMethod.ParameterValueList.GetACValue("Duration");
                        if (acValue == null || acValue.ValueT<TimeSpan>().TotalSeconds <= 0.000001)
                        {
                            acValue = acMethod.ParameterValueList.GetACValue("Temperature");
                            if (acValue == null || Math.Abs(acValue.ValueT<double>()) <= Double.Epsilon)
                            {
                                switchOff = false;
                            }
                        }
                    }
                }

                if (switchOff)
                {
                    foreach (PAEEMotorBase motor in ParentACComponent?.FindChildComponents<PAEEMotorBase>(c => c is PAEEMotorBase))
                    {
                        motor.ActivateRouteItemOnSimulation(null, true);
                    }
                }
            }

            return base.AnalyzeACMethodResult(acMethod, out msg, completeResult);
        }
        #endregion

        #region Private

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("SwitchOff", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("SwitchOff", "en{'Switch off'}de{'Ausschalten'}");
            method.ParameterValueList.Add(new ACValue("Speed", typeof(Int16), 0, Global.ParamOption.Optional));
            paramTranslation.Add("Speed", "en{'Speed'}de{'Geschwindigkeit'}");
            method.ParameterValueList.Add(new ACValue("MinWeight", typeof(Double), 0, Global.ParamOption.Optional));
            paramTranslation.Add("MinWeight", "en{'Minimum weight'}de{'Mindestgewicht'}");
            method.ParameterValueList.Add(new ACValue("Direction", typeof(PAFMixingDirectionEnum), PAFMixingDirectionEnum.Right, Global.ParamOption.Optional));
            paramTranslation.Add("Direction", "en{'Direction'}de{'Richtung'}");
            method.ParameterValueList.Add(new ACValue("DurationRight", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            paramTranslation.Add("DurationRight", "en{'Duration Right'}de{'Dauer Rechts'}");
            method.ParameterValueList.Add(new ACValue("DurationLeft", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            paramTranslation.Add("DurationLeft", "en{'Duration Left'}de{'Dauer Links'}");
            method.ParameterValueList.Add(new ACValue("DurationPause", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            paramTranslation.Add("DurationPause", "en{'Duration Pause'}de{'Dauer Pause'}");
            method.ParameterValueList.Add(new ACValue("DurationPause2", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            paramTranslation.Add("DurationPause2", "en{'Duration Pause 2'}de{'Dauer Pause 2'}");
            method.ParameterValueList.Add(new ACValue("Temperature", typeof(Double), 0.0, Global.ParamOption.Required));
            paramTranslation.Add("Temperature", "en{'Temperature'}de{'Temperatur'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, null);
        }

        protected static ACMethodWrapper CreateVirtualTimeMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("LeaveOn", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("LeaveOn", "en{'Leave motor switched on'}de{'Motor eingeschaltet lassen'}");
            method.ParameterValueList.Add(new ACValue("Duration", typeof(TimeSpan), 0, Global.ParamOption.Required));
            paramTranslation.Add("Duration", "en{'Duration'}de{'Dauer'}");
            method.ParameterValueList.Add(new ACValue("Speed", typeof(Int16), 0, Global.ParamOption.Optional));
            paramTranslation.Add("Speed", "en{'Speed'}de{'Geschwindigkeit'}");
            method.ParameterValueList.Add(new ACValue("MinWeight", typeof(Double), 0, Global.ParamOption.Optional));
            paramTranslation.Add("MinWeight", "en{'Minimum weight'}de{'Mindestgewicht'}");
            method.ParameterValueList.Add(new ACValue("Direction", typeof(PAFMixingDirectionEnum), PAFMixingDirectionEnum.Right, Global.ParamOption.Optional));
            paramTranslation.Add("Direction", "en{'Direction'}de{'Richtung'}");
            method.ParameterValueList.Add(new ACValue("DurationRight", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            paramTranslation.Add("DurationRight", "en{'Duration Right'}de{'Dauer Rechts'}");
            method.ParameterValueList.Add(new ACValue("DurationLeft", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            paramTranslation.Add("DurationLeft", "en{'Duration Left'}de{'Dauer Links'}");
            method.ParameterValueList.Add(new ACValue("DurationPause", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            paramTranslation.Add("DurationPause", "en{'Duration Pause'}de{'Dauer Pause'}");
            method.ParameterValueList.Add(new ACValue("DurationPause2", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            paramTranslation.Add("DurationPause2", "en{'Duration Pause 2'}de{'Dauer Pause 2'}");
            method.ParameterValueList.Add(new ACValue("Temperature", typeof(Double), 0.0, Global.ParamOption.Optional));
            paramTranslation.Add("Temperature", "en{'Temperature'}de{'Temperatur'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ActDuration", typeof(TimeSpan), 0, Global.ParamOption.Required));
            resultTranslation.Add("ActDuration", "en{'Duration'}de{'Dauer'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        protected static ACMethodWrapper CreateVirtualTemperatureMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("LeaveOn", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("LeaveOn", "en{'Leave motor switched on'}de{'Motor eingeschaltet lassen'}");
            method.ParameterValueList.Add(new ACValue("Temperature", typeof(Double), 0.0, Global.ParamOption.Required));
            paramTranslation.Add("Temperature", "en{'Temperature'}de{'Temperatur'}");
            method.ParameterValueList.Add(new ACValue("Speed", typeof(Int16), 0, Global.ParamOption.Optional));
            paramTranslation.Add("Speed", "en{'Speed'}de{'Geschwindigkeit'}");
            method.ParameterValueList.Add(new ACValue("MinWeight", typeof(Double), 0, Global.ParamOption.Optional));
            paramTranslation.Add("MinWeight", "en{'Minimum weight'}de{'Mindestgewicht'}");
            method.ParameterValueList.Add(new ACValue("Direction", typeof(PAFMixingDirectionEnum), PAFMixingDirectionEnum.Right, Global.ParamOption.Optional));
            paramTranslation.Add("Direction", "en{'Direction'}de{'Richtung'}");
            method.ParameterValueList.Add(new ACValue("DurationRight", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            paramTranslation.Add("DurationRight", "en{'Duration Right'}de{'Dauer Rechts'}");
            method.ParameterValueList.Add(new ACValue("DurationLeft", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            paramTranslation.Add("DurationLeft", "en{'Duration Left'}de{'Dauer Links'}");
            method.ParameterValueList.Add(new ACValue("DurationPause", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            paramTranslation.Add("DurationPause", "en{'Duration Pause'}de{'Dauer Pause'}");
            method.ParameterValueList.Add(new ACValue("DurationPause2", typeof(TimeSpan), 0, Global.ParamOption.Optional));
            paramTranslation.Add("DurationPause2", "en{'Duration Pause 2'}de{'Dauer Pause 2'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ActTemperature", typeof(Double), 0.0, Global.ParamOption.Required));
            resultTranslation.Add("ActTemperature", "en{'Temperature'}de{'Temperatur'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        #endregion

    }

    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(ACKind = Global.ACKinds.TACEnum, ACCaptionTranslation = "en{'Mixing direction'}de{'Mischen Richtung'}", QRYConfig = "gip.mes.processapplication.ACValueListPAFMixingDirectionEnum")]
    public enum PAFMixingDirectionEnum : short
    {
        [EnumMember]
        Right = 0,
        [EnumMember]
        Left = 1,
        [EnumMember]
        Interval = 2
    }

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Mixing direction'}de{'Mischen Richtung'}", Global.ACKinds.TACEnumACValueList)]
    public class ACValueListPAFMixingDirectionEnum : ACValueItemList
    {
        public ACValueListPAFMixingDirectionEnum() : base("PAFMixingDirectionEnum")
        {
            AddEntry(PAFMixingDirectionEnum.Right, "en{'Right'}de{'Rechts'}");
            AddEntry(PAFMixingDirectionEnum.Left, "en{'Left'}de{'Links'}");
            AddEntry(PAFMixingDirectionEnum.Interval, "en{'Interval'}de{'Intervall'}");
        }
    }
}
