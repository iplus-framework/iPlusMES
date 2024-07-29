using gip.core.datamodel;
using gip.core.autocomponent;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System;

namespace gip.mes.processapplication
{
    /// <summary>
    /// Process-Knoten zur implementierung eines untergeordneten (asynchronen) ACClassMethod-Aufruf auf die Model-Welt
    /// 
    /// Methoden zur Steuerung von außen: 
    /// -Start()    Starten des Processes
    ///
    /// Mögliche ACState:
    /// SMIdle      (Definiert in ACComponent)
    /// SMStarting (Definiert in PWNode)
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Skip on Dosing'}de{'Überspringe bei Dosierung'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWDosingSkip : PWNodeDecisionFunc
    {
        #region c´tors
        static PWDosingSkip()
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("SkipCondition", typeof(short), (short)0, Global.ParamOption.Required));
            paramTranslation.Add("SkipCondition", "en{'Condition for skipping'}de{'Bedingung zum überspringen'}");
            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWDosingSkip), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWDosingSkip), ACStateConst.SMStarting, wrapper);

            RegisterExecuteHandler(typeof(PWDosingSkip), HandleExecuteACMethod_PWDosingSkip);
        }

        public PWDosingSkip(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            _PreviousLoopTime = null;
            _EndlessLoopPreventionCounter = 0;
            return base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }
        #endregion


        #region Properties

        protected short SkipCondition
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("SkipCondition");
                    if (acValue != null)
                    {
                        return acValue.ParamAsInt16;
                    }
                }
                return 0;
            }
        }

        DateTime? _PreviousLoopTime;
        int _EndlessLoopPreventionCounter = 0;
        #endregion


        #region Methods

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWDosingSkip(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeDecisionFunc(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        public override void SMStarting()
        {
            RecalcTimeInfo();
            if (CreateNewProgramLog(NewACMethodWithConfiguration()) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                return;

            bool jump = false;
            var pwGroup = ParentPWGroup;
            if (pwGroup == null) // Is null when Service-Application is shutting down
            {
                if (this.InitState == ACInitState.Initialized)
                    Messages.LogError(this.GetACUrl(), "SMStarting()", "ParentPWGroup is null");
                return;
            }

            if (SkipCondition == 0)
            {
                bool hasRunSomeDosings = false;
                List<PWDosing> previousDosings = PWDosing.FindPreviousDosingsInPWGroup<PWDosing>(this);
                if (previousDosings != null)
                    hasRunSomeDosings = previousDosings.Where(c => c.HasRunSomeDosings).Any();
                if (!hasRunSomeDosings)
                    jump = true;
            }

            if (jump && _PreviousLoopTime.HasValue)
            {
                if ((DateTime.Now - _PreviousLoopTime.Value).TotalSeconds < 2)
                    _EndlessLoopPreventionCounter++;
                else
                    _EndlessLoopPreventionCounter = 0;
                if (_EndlessLoopPreventionCounter >= 5)
                {
                    jump = false;
                    string errorMsg = "Endless loop detected! Please investigate the problem and raise event yourself!";
                    OnNewAlarmOccurred(ProcessAlarm, errorMsg);
                    Messages.LogError(this.GetACUrl(), "SMStarting()", errorMsg);
                    Reset();
                    return;
                }
            }

            if (jump)
            {
                _PreviousLoopTime = DateTime.Now;
                RaiseElseEventAndComplete();
            }
            else
            {
                _EndlessLoopPreventionCounter = 0;
                RaiseOutEventAndComplete();
            }
        }
        #endregion

        #region Dumping
        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList, ref DumpStats dumpStats)
        {
            base.DumpPropertyList(doc, xmlACPropertyList, ref dumpStats);

            //XmlElement xmlChild = xmlACPropertyList["_LastSubStateResetCounter"];
            //if (xmlChild == null)
            //{
            //    xmlChild = doc.CreateElement("_LastSubStateResetCounter");
            //    if (xmlChild != null)
            //        xmlChild.InnerText = _LastSubStateResetCounter.ToString();
            //    xmlACPropertyList.AppendChild(xmlChild);
            //}
        }
        #endregion
    }
}
