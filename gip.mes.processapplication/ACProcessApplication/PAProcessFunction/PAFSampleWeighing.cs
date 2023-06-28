using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.processapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Sample weighing'}de{'Gewichtsprüfung'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWSampleWeighing.PWClassName, true, BSOConfig = "BSOSampleWeighing")]
    public class PAFSampleWeighing : PAProcessFunction, IPAFuncScaleConfig
    {
        #region Constructors

        public const string ClassName = "PAFSampleWeighing";

        static PAFSampleWeighing()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFSampleWeighing), ACStateConst.TMStart, CreateVirtualMethod("SampleWeighing", "en{'Sample weighing'}de{'Gewichtsprüfung'}", typeof(PWSampleWeighing)));
            RegisterExecuteHandler(typeof(PAFSampleWeighing), HandleExecuteACMethod_PAFSampleWeighing);
        }

        public PAFSampleWeighing(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _FuncScaleConfig = new ACPropertyConfigValue<string>(this, PAScaleMappingHelper<IACComponent>.FuncScaleConfigName, "");
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool result = base.ACInit(startChildMode);
            PAESamplePiLightBox.InitializeExtPropInLabOrderPos(this);
            _ = FuncScaleConfig;
            return result;

        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (_ScaleMappingHelper != null)
                {
                    _ScaleMappingHelper.DetachAndRemove();
                    _ScaleMappingHelper = null;
                }
            }

            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties
        protected ACPropertyConfigValue<string> _FuncScaleConfig;
        [ACPropertyConfig("en{'Assigned Scales'}de{'Zugeordnete Waagen'}")]
        public string FuncScaleConfig
        {
            get
            {
                return _FuncScaleConfig.ValueT;
            }
        }

        public PAEScaleBase CurrentScaleForWeighing
        {
            get
            {
                if (ScaleMappingHelper.AssignedScales != null && ScaleMappingHelper.AssignedScales.Any())
                    return ScaleMappingHelper.AssignedScales.FirstOrDefault();
                else
                    return ParentACComponent.FindChildComponents<PAEScaleBase>(c => c is PAEScaleBase).FirstOrDefault();
            }
        }

        private PAScaleMappingHelper<PAEScaleBase> _ScaleMappingHelper;
        public PAScaleMappingHelper<PAEScaleBase> ScaleMappingHelper
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_ScaleMappingHelper == null)
                        _ScaleMappingHelper = new PAScaleMappingHelper<PAEScaleBase>(this, this);
                }
                return _ScaleMappingHelper;
            }
        }
        #endregion

        #region Methods

        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }


        public override void SMStarting()
        {
            base.SMStarting();
        }

        public override void SMRunning()
        {
            UnSubscribeToProjectWorkCycle();
        }

        protected override MsgWithDetails CompleteACMethodOnSMStarting(ACMethod acMethod)
        {
            return base.CompleteACMethodOnSMStarting(acMethod);
        }

        [ACMethodInfo("", "", 410)]
        public virtual Msg RegisterSampleWeight()
        {
            if (CurrentScaleForWeighing == null)
            {
                return new Msg("The scale for register sample weight can not be found!", this, eMsgLevel.Error, ClassName, "RegisterSampleWeight(10)", 124);
            }

            PAEScaleBase scale = CurrentScaleForWeighing;

            //if(ScaleCalibratable == null)
            //    return new Msg("ScaleCalibratable is null!", this, eMsgLevel.Error, ClassName, "RegisterSampleWeight(10)", 88);

            //Guid? weighingID = ScaleCalibratable.OnRegisterAlibiWeight(null);

            //if(!weighingID.HasValue)
            //    return new Msg("Problem with register alibi weight. Please ensure that scale is stillstand or check the alarms!", this, eMsgLevel.Error, ClassName, "RegisterSampleWeight(20)", 93);

            

            PAEScaleCalibratable calibScale = scale as PAEScaleCalibratable;
            double actualWeight = 0.0;
            if (calibScale != null)
            {
                Msg msg = calibScale.OnRegisterAlibiWeight(null);
                if (msg != null)
                {
                    OnNewAlarmOccurred(FunctionError, msg.Message);
                    FunctionError.ValueT = PANotifyState.AlarmOrFault;
                    return msg;
                }

                if (CurrentACMethod != null && CurrentACMethod.ValueT != null)
                {
                    if (!IsSimulationOn)
                        actualWeight = calibScale.AlibiWeight.ValueT;
                    CurrentACMethod.ValueT.ResultValueList["AlibiNo"] = calibScale.AlibiNo.ValueT;
                }
            }
            else if (scale != null)
            {
                if (CurrentACMethod != null)
                    actualWeight = scale.ActualWeight.ValueT;
            }

            if (Math.Abs(actualWeight) <= Double.Epsilon && IsSimulationOn && CurrentACMethod != null)
            {
                ACMethod acMethod = CurrentACMethod.ValueT;
                if (acMethod != null)
                {
                    actualWeight = (double) acMethod.ParameterValueList["TargetQuantity"];
                    double tol = (double) acMethod.ParameterValueList["TolerancePlus"];
                    if (Math.Abs(tol) > Double.Epsilon)
                    {
                        Random rnd = new Random();
                        actualWeight -= (rnd.NextDouble() * tol);
                    }
                }
            }

            if (CurrentACMethod != null && CurrentACMethod.ValueT != null)
                CurrentACMethod.ValueT.ResultValueList["ActualWeight"] = actualWeight;


            CurrentACState = ACStateEnum.SMCompleted;
            return null;
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(RegisterSampleWeight):
                    result = RegisterSampleWeight();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PAFSampleWeighing(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue("PLPos", typeof(Guid), null, Global.ParamOption.Optional));
            paramTranslation.Add("PLPos", "en{'Order position'}de{'Auftragsposition'}");
            method.ParameterValueList.Add(new ACValue("TargetQuantity", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("TargetQuantity", "en{'Target Quantity'}de{'Sollmenge'}");
            method.ParameterValueList.Add(new ACValue("TolerancePlus", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("TolerancePlus", "en{'Tolerance + [+=kg/-=%]'}de{'Toleranz + [+=kg/-=%]'}");
            method.ParameterValueList.Add(new ACValue("ToleranceMinus", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("ToleranceMinus", "en{'Tolerance - [+=kg/-=%]'}de{'Toleranz - [+=kg/-=%]'}");
            method.ParameterValueList.Add(new ACValue("AckInTol", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("AckInTol", "en{'Can Acknowledge if in Tolerance'}de{'Bestätigung nur innerhalb der Toleranz'}");

            method.ResultValueList.Add(new ACValue("ActualWeight", typeof(double), null, Global.ParamOption.Optional));
            resultTranslation.Add("ActualWeight", "en{'Actual weight'}de{'Ist-Gewicht'}");
            method.ResultValueList.Add(new ACValue("AlibiNo", typeof(string), null, Global.ParamOption.Optional));
            resultTranslation.Add("AlibiNo", "en{'Alibi'}de{'Alibi'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        #endregion
    }
}
