using gip.core.datamodel;
using gip.core.autocomponent;
using System.Collections.Generic;
using System.Xml;
using System;
using gip.mes.datamodel;
using static gip.core.communication.ISOonTCP.PLC;

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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Dosingloop'}de{'Dosierschleife'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWDosingLoop : PWNodeDecisionFunc
    {
        #region c´tors
        static PWDosingLoop()
        {
            RegisterExecuteHandler(typeof(PWDosingLoop), HandleExecuteACMethod_PWDosingLoop);
        }

        public PWDosingLoop(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            if (deleteACClassTask)
                _LastSubStateResetCounter = 0;
            _PreviousLoopTime = null;
            _EndlessLoopPreventionCounter = 0;
            return base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            _LastSubStateResetCounter = 0;
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }
        #endregion


        #region Properties
        int _LastSubStateResetCounter = 0;
        DateTime? _PreviousLoopTime;
        int _EndlessLoopPreventionCounter = 0;

        public override bool MustBeInsidePWGroup => true;
        #endregion


        #region Methods

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWDosingLoop(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeDecisionFunc(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        public override void SMStarting()
        {
            RecalcTimeInfo();
            if (CreateNewProgramLog(NewACMethodWithConfiguration()) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                return;

            bool loop = false;
            var pwGroup = ParentPWGroup;
            if (pwGroup == null) // Is null when Service-Application is shutting down
            {
                if (this.InitState == ACInitState.Initialized)
                    Messages.LogError(this.GetACUrl(), "SMStarting()", "ParentPWGroup is null");
                return;
            }

            if (   ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging)
                || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMDisThenNextComp))
            {
                bool hasOpenDosings = false;
                bool anyDosingNodeFound = false;
                bool isOneActive = false;
                List<IPWNodeReceiveMaterial> previousDosings = PWDosing.FindPreviousDosingsInPWGroup<IPWNodeReceiveMaterial>(this);
                if (previousDosings != null)
                {
                    foreach (var pwDosing in previousDosings)
                    {
                        if ((pwDosing as PWBase).CurrentACState >= ACStateEnum.SMStarting
                            && (pwDosing as PWBase).CurrentACState < ACStateEnum.SMCompleted)
                            isOneActive = true;
                    }
                    foreach (var pwDosing in previousDosings)
                    {
                        anyDosingNodeFound = true;
                        double dosedQuantity;
                        if (pwDosing.HasOpenDosings(out dosedQuantity))
                        {
                            PWDosing pwDosing2 = pwDosing as PWDosing;
                            if (pwDosing2 != null)
                                hasOpenDosings = pwDosing2.HasAndCanProcessAnyMaterialPicking(ParentPWGroup.AccessedProcessModule);
                            else
                                hasOpenDosings = true;
                            break;
                        }
                    }
                }
                loop = (!anyDosingNodeFound || hasOpenDosings);

                if (!isOneActive)
                {
                    ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMIdle;
                    _LastSubStateResetCounter = IterationCount.ValueT + 1;
                }

                if (loop && previousDosings != null)
                {
                    foreach (var pwDosing in previousDosings)
                    {
                        pwDosing.OnDosingLoopDecision(this, loop);
                    }
                }
            }

            if (loop && _PreviousLoopTime.HasValue)
            {
                if ((DateTime.Now - _PreviousLoopTime.Value).TotalSeconds < 2)
                    _EndlessLoopPreventionCounter++;
                else
                    _EndlessLoopPreventionCounter = 0;
                if (_EndlessLoopPreventionCounter >= 5)
                {
                    loop = false;
                    string errorMsg = "Endless loop detected! Please investigate the problem and raise event yourself!";
                    OnNewAlarmOccurred(ProcessAlarm, errorMsg);
                    Messages.LogError(this.GetACUrl(), "SMStarting()", errorMsg);
                    Reset();
                    return;
                }
            }

            if (loop)
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
        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);

            XmlElement xmlChild = xmlACPropertyList["_LastSubStateResetCounter"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("_LastSubStateResetCounter");
                if (xmlChild != null)
                    xmlChild.InnerText = _LastSubStateResetCounter.ToString();
                xmlACPropertyList.AppendChild(xmlChild);
            }
        }
        #endregion
    }
}
