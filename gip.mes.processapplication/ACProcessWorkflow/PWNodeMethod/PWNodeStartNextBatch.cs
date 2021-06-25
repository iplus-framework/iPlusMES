using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using System.Xml;
using gip.core.autocomponent;

namespace gip.mes.processapplication
{

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Start next batch'}de{'Nächsten Batch starten'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWNodeStartNextBatch : PWBaseNodeProcess
    {
        public const string PWClassName = "PWNodeStartNextBatch";

        #region Properties
        #endregion

        #region Constructors

        static PWNodeStartNextBatch()
        {
            ACMethod TMP;
            TMP = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> translation = new Dictionary<string, string>();
            
            TMP.ParameterValueList.Add(new ACValue("SkipIfIterationCount", typeof(int), 0, Global.ParamOption.Optional));
            translation.Add("SkipIfIterationCount", "en{'Skip if iteration count greather than'}de{'Überspringen, wenn die Iteration mehr zählt als'}");

            var wrapper = new ACMethodWrapper(TMP, "en{'Start next batch'}de{'Nächsten Batch starten'}", typeof(PWNodeStartNextBatch), translation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWNodeStartNextBatch), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWNodeStartNextBatch), HandleExecuteACMethod_PWNodeStartNextBatch);
        }

        public PWNodeStartNextBatch(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ClearMyConfiguration();
            return base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            ClearMyConfiguration();
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }

        #endregion

        #region Public

        #region Execute-Helper-Handlers
        public static bool HandleExecuteACMethod_PWNodeStartNextBatch(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWBaseNodeProcess(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            //if (!PreExecute(PABaseState.SMStarting))
            //  return;
            RecalcTimeInfo();
            CreateNewProgramLog(NewACMethodWithConfiguration());

            var rootPW = RootPW;
            bool startNewBatch = true;
            if (SkipIfIterationCount > 0)
                startNewBatch = SkipIfIterationCount > IterationCount.ValueT;

            if (rootPW != null && (((ACSubStateEnum)rootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                                || ((ACSubStateEnum)rootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode)))
                startNewBatch = false;
            PWGroup pwGroup = ParentPWGroup;
            if (pwGroup != null && (((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                                || ((ACSubStateEnum)pwGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode)))
                startNewBatch = false;

            if (startNewBatch)
               SendNewBatchEvent2Invoker();

            //if (ACOperationMode == ACOperationModes.Live)
            //    CurrentACState = PABaseState.SMRunning;
            //else
            // Falls durch tiefere Callstacks der Status schon weitergeschaltet worden ist, dann schalte Status nicht weiter
            if (CurrentACState == ACStateEnum.SMStarting)
                CurrentACState = ACStateEnum.SMCompleted;
            //PostExecute(PABaseState.SMStarting);
        }

        [ACMethodState("en{'Running'}de{'Läuft'}", 30, true)]
        public override void SMRunning()
        {
        }

        [ACMethodState("en{'Completed'}de{'Beendet'}", 40, true)]
        public override void SMCompleted()
        {
            base.SMCompleted();
        }

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
            }
            this.HasRules.ValueT = 0;
        }

        public int SkipIfIterationCount
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("SkipIfIterationCount");
                    if (acValue != null)
                        return acValue.ParamAsInt32;
                }
                return 0;
            }
        }

        #endregion

        #region Protected

        public virtual bool SendNewBatchEvent2Invoker()
        {
            var rootPW = RootPW;
            if (rootPW != null && rootPW.ParentTaskExecComp != null)
            {
                IACComponentTaskExec taskExec = rootPW.ParentTaskExecComp;
                ACMethodEventArgs eventArgs = rootPW.CreateNewMethodEventArgs(rootPW.CurrentTask.ACMethod, Global.ACMethodResultState.InProcess);
                if (eventArgs != null)
                {
                    ACValue acValue = new ACValue(typeof(PWNodeStartNextBatch).Name, typeof(string), this.GetACUrl());
                    eventArgs.Add(acValue);
                    return taskExec.CallbackTask(rootPW.CurrentTask, eventArgs, PointProcessingState.Accepted);
                }
            }
            return false;
        }

        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);
        }

        #endregion
    }
}
