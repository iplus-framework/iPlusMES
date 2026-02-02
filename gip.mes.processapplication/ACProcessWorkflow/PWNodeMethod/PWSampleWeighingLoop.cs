using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Sample weighing loop'}de{'Stichproben Schleife'}", Global.ACKinds.TPWNodeStatic, Global.ACStorableTypes.Optional, false, PWMethodVBBase.PWClassName, true)]
    public class PWSampleWeighingLoop : PWNodeDecisionFunc
    {
        public PWSampleWeighingLoop(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACPostInit()
        {
            return base.ACPostInit();
        }

        public override bool ACPreDeInit(bool deleteACClassTask = false)
        {
            return base.ACPreDeInit(deleteACClassTask);
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            _IsInCompleteState = false;
            return await base.ACDeInit(deleteACClassTask);
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {
            _IsInCompleteState = false;
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }

        public override void SMStarting()
        {
        }

        private bool _IsInCompleteState = false;

        public override void PWPointInCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            if (_IsInCompleteState)
                return;

            PWSampleWeighing sampleWeighing = null;

            PWBaseInOut senderComp = sender?.ParentACComponent as PWBaseInOut;
            if (senderComp is PWGroup)
            {
                sampleWeighing = senderComp.FindChildComponents<PWSampleWeighing>(c => c is PWSampleWeighing).FirstOrDefault();
            }

            if (sampleWeighing == null)
            {
                var possiblePWGroups = senderComp.FindPredecessors<PWGroup>(false, c => c.ACComponentChilds.Any(x => x is PWSampleWeighing), null);
                if (possiblePWGroups.Count > 1)
                {
                    RaiseOutEventAndComplete();
                    return;
                }

                var targetGroup = possiblePWGroups.FirstOrDefault();
                if (targetGroup != null)
                {
                    sampleWeighing = targetGroup.FindChildComponents<PWSampleWeighing>(c => c is PWSampleWeighing).FirstOrDefault();
                }
            }

            if (sampleWeighing != null)
                RaiseElseEventAndComplete();
            else
                CompleteSampleWeighing();

            //base.PWPointInCallback(sender, e, wrapObject);
        }

        [ACMethodInfo("","",999)]
        public virtual void CompleteSampleWeighing()
        {
            _IsInCompleteState = true;

            var possiblePWGroups = FindPredecessors<PWGroup>(false, c => c.ACComponentChilds.Any(x => x is PWSampleWeighing), null);
            if (possiblePWGroups.Count > 1)
                throw new Exception("The only one PWGroup with node PWSampleWeighing can operate with PWSampleWeighingLoop node.");

            var targetGroup = possiblePWGroups.FirstOrDefault();
            if(targetGroup != null && targetGroup.CurrentACState != ACStateEnum.SMIdle)
            {
                PWSampleWeighing sampleWeighing = targetGroup.FindChildComponents<PWSampleWeighing>(c => c is PWSampleWeighing).FirstOrDefault();
                if(sampleWeighing != null)
                {
                    if (sampleWeighing.CurrentACState != ACStateEnum.SMIdle)
                    {
                        PAFSampleWeighing pafSampleWeighing = sampleWeighing.CurrentExecutingFunction<PAFSampleWeighing>();
                        if (pafSampleWeighing != null && pafSampleWeighing.CurrentACState != ACStateEnum.SMIdle)
                        {
                            pafSampleWeighing.Abort();
                        }
                        else
                        {
                            sampleWeighing.RaiseOutEvent();
                            sampleWeighing.Reset();
                        }
                    }
                }
            }

            var possiblePWNodesWait = FindPredecessors<PWNodeWait>(false, c => c is PWNodeWait);
            if(possiblePWNodesWait.Count > 1)
                throw new Exception("The only one PWNodeWait can operate with PWSampleWeighingLoop node.");

            PWNodeWait waitNode = possiblePWNodesWait.FirstOrDefault();
            if (waitNode != null && waitNode.CurrentACState != ACStateEnum.SMIdle)
            {
                waitNode.UnSubscribeToProjectWorkCycle();
                waitNode.Reset();
            }

            _IsInCompleteState = false;

            RaiseOutEventAndComplete();
        }

        public override object ExecuteMethod(AsyncMethodInvocationMode invocationMode, string acMethodName, params object[] acParameter)
        {
            return base.ExecuteMethod(invocationMode, acMethodName, acParameter);
        }

    }
}
