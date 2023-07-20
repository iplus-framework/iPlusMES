using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Workflow loop'}de{'Workflow Schleife'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWNodeLoop : PWNodeDecisionFunc
    {
        #region c'tors

        static PWNodeLoop()
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue("OnLoopACIdentifier", typeof(string), "", Global.ParamOption.Required));
            paramTranslation.Add("OnLoopACIdentifier", "en{'On loop inpoint ACIdentifier'}de{'On loop inpoint ACIdentifier'}");

            method.ParameterValueList.Add(new ACValue("CompletePWGroupsOnInpointLoop", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("CompletePWGroupsOnInpointLoop", "en{'Complete PWGroups(SMStarting) on inpoint loop'}de{'Complete PWGroups(SMStarting) on inpoint loop'}");

            method.ParameterValueList.Add(new ACValue("Repeats", typeof(UInt32), 0, Global.ParamOption.Optional));
            paramTranslation.Add("Repeats", "en{'Repeats'}de{'Wiederholungen'}");

            var wrapper = new ACMethodWrapper(method, "en{'Configuration'}de{'Konfiguration'}", typeof(PWNodeLoop), paramTranslation, null);
            ACMethod.RegisterVirtualMethod(typeof(PWNodeLoop), ACStateConst.SMStarting, wrapper);
            RegisterExecuteHandler(typeof(PWNodeLoop), HandleExecuteACMethod_PWNodeLoop);
        }

        public PWNodeLoop(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACPostInit()
        {
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _IsInCompleteState = false;
            return base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            _IsInCompleteState = false;
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }

        #endregion

        #region Properties

        protected bool _IsInCompleteState = false;

        protected string OnLoopACIdentifier
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("OnLoopACIdentifier");
                    if (acValue != null)
                    {
                        return acValue.ParamAsString;
                    }
                }
                return "";
            }
        }

        protected bool CompletePWGroupsOnInpointLoop
        {
            get
            {
                var method = MyConfiguration;
                if (method != null)
                {
                    var acValue = method.ParameterValueList.GetACValue("CompletePWGroupsOnInpointLoop");
                    if (acValue != null)
                    {
                        return acValue.ParamAsBoolean;
                    }
                }
                return false;
            }
        }

        #endregion

        #region Methods

        [ACMethodState("en{'Executing'}de{'Ausführend'}", 20, true)]
        public override void SMStarting()
        {
            base.SMStarting();
        }

        public override void Reset()
        {
            base.Reset();
            _IsInCompleteState = false;
        }

        public override void PWPointInCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            bool raiseElseEvent = (wrapObject as ACRef<ACComponent>)?.ParentACObject?.ACIdentifier == OnLoopACIdentifier;

            if (raiseElseEvent)
            {
                if (_IsInCompleteState)
                {
                    RaiseOutEventAndComplete();
                    return;
                }

                ApplicationManager.ApplicationQueue.Add(() => RaiseElseEventAndComplete());
            }
            else
            {
                _IsInCompleteState = true;

                if (CompletePWGroupsOnInpointLoop)
                {
                    PWGroup pwGroup = FindPredecessors<PWGroup>(true, c => c is PWGroup && c.ACIdentifier == OnLoopACIdentifier).FirstOrDefault();
                    if (pwGroup != null && pwGroup.CurrentACState == ACStateEnum.SMStarting)
                    {
                        pwGroup.ResetAndComplete();
                    }
                }
            }
        }

        public static bool HandleExecuteACMethod_PWNodeLoop(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeDecisionFunc(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}
