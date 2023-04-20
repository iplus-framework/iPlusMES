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
        static PWNodeLoop()
        {
            ACMethod method;
            method = new ACMethod(ACStateConst.SMStarting);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue("OnLoopACIdentifier", typeof(string), "", Global.ParamOption.Required));
            paramTranslation.Add("OnLoopACIdentifier", "en{'On loop inpoint ACIdentifier'}de{'On loop inpoint ACIdentifier'}");

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

        #endregion

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
            }
        }

        public virtual void OnLoopComplete()
        {
            _IsInCompleteState = true;
            RaiseOutEventAndComplete();
        }

        public static bool HandleExecuteACMethod_PWNodeLoop(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PWNodeDecisionFunc(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
    }
}
