using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.processapplication;
using gip.mes.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.processapplication;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Sample weighing'}de{'Gewichtsprüfung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, SortIndex = 400)]
    public class BSOSampleWeighing : BSOWorkCenterChild
    {
        public BSOSampleWeighing(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            DeActivate();
            return base.ACDeInit(deleteACClassTask);
        }

        public const string ClassName = "BSOSampleWeighing";

        #region Properties

        [ACPropertyInfo(601)]
        public ACComponent ProcessModule
        {
            get;
            set;
        }

        public ACComponent ProcessFunction
        {
            get;
            set;
        }

        public IACContainerTNet<ACStateEnum> PAFACState
        {
            get;
            set;
        }

        private ACRef<ACComponent> _ScaleRef;
        [ACPropertyInfo(602)]
        public ACComponent Scale
        {
            get
            {
                if (_ScaleRef != null)
                    return _ScaleRef.ValueT;
                return null;
            }
        }

        #endregion

        #region Methods

        public override void Activate(ACComponent selectedProcessModule)
        {
            DeActivate();

            var pafManWeighingRef = new ACRef<IACComponent>(selectedProcessModule, this);

            ProcessModule = selectedProcessModule;
            ProcessFunction = ItemFunction?.ProcessFunction;

            var processModuleChildComps = ProcessModule.ACComponentChildsOnServer;

            var scale = processModuleChildComps.FirstOrDefault(c => typeof(PAEScaleGravimetric).IsAssignableFrom(c.ComponentClass.ObjectType)) as ACComponent;
            if (scale == null)
            {
                //Error50325: The child component Scale can not be found on the current process module.
                Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "Activate(10)", 114, "Error50325");
                Messages.LogMessageMsg(msg);
                Messages.Msg(msg);
                return;
            }

            _ScaleRef = new ACRef<ACComponent>(scale, this);

            PAFACState = ProcessFunction.GetPropertyNet(nameof(PAProcessFunction.ACState)) as IACContainerTNet<ACStateEnum>;
            if (PAFACState == null)
            {
                //Error50326: The property ACState can not be found on the current process function.
                Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "Activate(20)", 114, "Error50326");
                Messages.LogMessageMsg(msg);
                Messages.Msg(msg);
                return;
            }
        }

        public override void DeActivate()
        {
            if (_ScaleRef != null)
                _ScaleRef.Detach();
            _ScaleRef = null;
            ProcessFunction = null;
            ProcessModule = null;
            PAFACState = null;
        }

        [ACMethodInfo("", "en{'Register sample weight'}de{'Stichprobengewicht registrieren'}", 601, true)]
        public void RegisterSampleWeight()
        {
            Msg msg = ProcessFunction.ExecuteMethod(nameof(PAFSampleWeighing.RegisterSampleWeight)) as Msg;
            if (msg != null)
                Messages.Msg(msg);
        }

        public bool IsEnabledRegisterSampleWeight()
        {
            if (ProcessFunction == null)
                return false;

            if (PAFACState == null)
                return false;

            if (PAFACState.ValueT != ACStateEnum.SMRunning)
                return false;

            return true;
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;

            switch(acMethodName)
            {
                case nameof(RegisterSampleWeight):
                    RegisterSampleWeight();
                    return true;
                case nameof(IsEnabledRegisterSampleWeight):
                    result = IsEnabledRegisterSampleWeight();
                    return true;
            }

            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}
