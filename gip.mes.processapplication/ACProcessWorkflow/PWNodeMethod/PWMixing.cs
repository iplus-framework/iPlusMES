using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Xml;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PWMixing'}de{'PWMixing'}", Global.ACKinds.TPWNodeMethod, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWMixing : PWNodeProcessMethod, IACMyConfigCache
    {
        public const string PWClassName = "PWMixing";

        #region c´tors
        static PWMixing()
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("SkipIfCountComp", typeof(int), 0, Global.ParamOption.Required));
            paramTranslation.Add("SkipIfCountComp", "en{'Skip if count components lower than'}de{'Überspringe wenn Komponentenanzahl kleiner als'}");
            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWMixing), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWMixing), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWMixing), HandleExecuteACMethod_PWMixing);
        }

        public PWMixing(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
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
        public static bool HandleExecuteACMethod_PWMixing(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeProcessMethod(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region Properties
        private ACMethod _MyConfiguration;
        public ACMethod MyConfiguration
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_MyConfiguration != null)
                        return _MyConfiguration;
                }

                var myNewConfig = NewACMethodWithConfiguration();
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _MyConfiguration = myNewConfig;
                }
                return myNewConfig;
            }
        }

        public void ClearMyConfiguration()
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                _MyConfiguration = null;
                _SkipInvocTries = 0;
            }
            this.HasRules.ValueT = 0;
        }

        private int _SkipInvocTries = 0;
        protected int SkipIfCountComp
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("SkipIfCountComp");
                    if (acValue != null)
                    {
                        return acValue.ParamAsInt32;
                    }
                }
                return 0;
            }
        }
        #endregion

        #region Methods
        public override void SMIdle()
        {
            _SkipInvocTries = 0;
            base.SMIdle();
        }

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            if (SkipIfCountComp > 0)
            {
                int countDosings = 0;
                List<PWDosing> previousDosings = PWDosing.FindPreviousDosingsInPWGroup<PWDosing>(this);
                if (previousDosings != null)
                    countDosings = previousDosings.Sum(c => c.CountRunDosings);

                if (countDosings < SkipIfCountComp)
                {
                    // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
                    if (CurrentACState == ACStateEnum.SMStarting)
                        CurrentACState = ACStateEnum.SMCompleted;
                    return;
                }
            }


            if (ParentPWGroup != null
                && this.ContentACClassWF != null)
            {
                ACClassMethod refPAACClassMethod = null;

                using (ACMonitor.Lock(this.ContextLockForACClassWF))
                {
                    refPAACClassMethod = this.ContentACClassWF.RefPAACClassMethod;
                }

                if (refPAACClassMethod != null)
                {
                    PAProcessModule module = null;
                    if (ParentPWGroup.NeedsAProcessModule && (ACOperationMode == ACOperationModes.Live || ACOperationMode == ACOperationModes.Simulation))
                        module = ParentPWGroup.AccessedProcessModule;
                    // Testmode
                    else
                        module = ParentPWGroup.ProcessModuleForTestmode;

                    if (module == null)
                    {
                        // TODO: Meldung: Programmfehler, darf nicht vorkommen
                        return;
                    }
                    ACMethod paramMethod = refPAACClassMethod.TypeACSignature();

                    if (!(bool)ExecuteMethod("GetConfigForACMethod", paramMethod, true))
                    {
                        // TODO: Meldung
                        return;
                    }

                    RecalcTimeInfo();
                    if (CreateNewProgramLog(paramMethod) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                        return;
                    _ExecutingACMethod = paramMethod;

#if DEBUG
                    module.TaskInvocationPoint.ClearMyInvocations(this);
#endif
                    if (!IsTaskStarted(module.TaskInvocationPoint.AddTask(paramMethod, this)))
                    {
                        _SkipInvocTries++;
                        if (_SkipInvocTries > 3)
                        {
                            _SkipInvocTries = 0;
                            UnSubscribeToProjectWorkCycle();
                            if (CurrentACState == ACStateEnum.SMStarting)
                                CurrentACState = ACStateEnum.SMCompleted;
                            return;
                        }
                        else
                            SubscribeToProjectWorkCycle();
                        return;
                    }
                    else
                    {
                        UnSubscribeToProjectWorkCycle();
                    }
                    UpdateCurrentACMethod();
                }
            }

            // Falls module.AddTask synchron ausgeführt wurde, dann ist der Status schon weiter
            if (IsACStateMethodConsistent(ACStateEnum.SMStarting) < ACStateCompare.WrongACStateMethod)
            {
                CurrentACState = ACStateEnum.SMRunning;
                //PostExecute(PABaseState.SMStarting);
            }

        }
        #endregion

        #region Planning and Testing
        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);

            XmlElement xmlChild = xmlACPropertyList["SkipIfCountComp"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("SkipIfCountComp");
                if (xmlChild != null)
                    xmlChild.InnerText = SkipIfCountComp.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }

        }
        #endregion
    }
}
