using gip.core.datamodel;
using gip.core.autocomponent;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Linq;

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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Loading loop'}de{'Verladeschleife'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWLoadingLoop : PWNodeDecisionFunc
    {
        #region c´tors
        static PWLoadingLoop()
        {
            RegisterExecuteHandler(typeof(PWLoadingLoop), HandleExecuteACMethod_PWLoadingLoop);
        }

        public PWLoadingLoop(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        public static bool HandleExecuteACMethod_PWLoadingLoop(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeDecisionFunc(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        public override void SMStarting()
        {
            RecalcTimeInfo();
            if (CreateNewProgramLog(NewACMethodWithConfiguration()) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                return;

            bool? loop = null;
            IPWNodeReceiveMaterial receiveMat = PWPointIn.ConnectionList.Where(c => c.ValueT is IPWNodeReceiveMaterial).FirstOrDefault()?.ValueT as IPWNodeReceiveMaterial;
            if (receiveMat != null)
                loop = receiveMat.HasAnyMaterialToProcess;
            else
            {
                receiveMat = PWPointElseOut.ConnectionList.Where(c => c.ValueT is IPWNodeReceiveMaterial).FirstOrDefault()?.ValueT as IPWNodeReceiveMaterial;
                if (receiveMat != null)
                    loop = receiveMat.HasAnyMaterialToProcess;
            }

            if (!loop.HasValue)
            {
                PWGroupVB pwGroupVB = PWPointIn.ConnectionList.Where(c => c.ValueT is PWGroupVB).FirstOrDefault()?.ValueT as PWGroupVB;
                if (pwGroupVB != null)
                    loop = pwGroupVB.HasAnyMaterialToProcess;
                else
                {
                    pwGroupVB = PWPointElseOut.ConnectionList.Where(c => c.ValueT is PWGroupVB).FirstOrDefault()?.ValueT as PWGroupVB;
                    if (pwGroupVB != null)
                        loop = pwGroupVB.HasAnyMaterialToProcess;
                }
            }

            if (!loop.HasValue)
            {
                receiveMat = FindPredecessors<IPWNodeReceiveMaterial>(true, c => c is IPWNodeReceiveMaterial, null, 10).FirstOrDefault();
                if (receiveMat != null)
                    loop = receiveMat.HasAnyMaterialToProcess;
            }


            if (loop.HasValue && loop.Value && _PreviousLoopTime.HasValue)
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

            if (loop.HasValue && loop.Value)
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
