// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Sample magazine'}de{'Probemagazin'}", Global.ACKinds.TPAModule, Global.ACStorableTypes.Required, false, true)]
    public class PAESampleMagazine : PAModule
    {
        #region c'tors

        static PAESampleMagazine()
        {
            RegisterExecuteHandler(typeof(PAESampleMagazine), HandleExecuteACMethod_PAESampleMagazine);
        }

        public PAESampleMagazine(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _SampleMagazineSize = new ACPropertyConfigValue<short>(this, "SampleMagazineSize", 10);
            _PAFSamplingACUrl = new ACPropertyConfigValue<string>(this, "PAFSamplingACUrl", "");
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACPostInit()
        {
            ThreadPool.QueueUserWorkItem((object state) => SimulateNextMagazinePosition(1000));
            _LabOrderManager = ACLabOrderManager.ACRefToServiceInstance(this);
            return base.ACPostInit();
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            if (_LabOrderManager != null)
                ACLabOrderManager.DetachACRefFromServiceInstance(this, _LabOrderManager);
            _LabOrderManager = null;
            return await base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        [ACPropertyBindingTarget(999, "Write to PLC", "en{'Target position'}de{'Sollposition'}", IsPersistable = true)]
        public IACContainerTNet<short> TargetPosition
        {
            get;
            set;
        }

        [ACPropertyBindingTarget(999, "Read from PLC", "en{'Actual position'}de{'Ist-Position'}")]
        public IACContainerTNet<short> ActualPosition
        {
            get;
            set;
        }

        public short SimActualPosition
        {
            get;
            set;
        }

        [ACPropertyBindingSource(210, "Error", "en{'State alarm'}de{'Zustandsalarm'}", "", false, false)]
        public IACContainerTNet<PANotifyState> StateAlarm { get; set; }
        public const string PropNameStateAlarm = "StateAlarm";

        private ACPropertyConfigValue<short> _SampleMagazineSize;
        [ACPropertyConfig("en{'Sample magazine size'}de{'Probemagazin-Größe'}")]
        public short SampleMagazineSize
        {
            get => _SampleMagazineSize.ValueT;
            set
            {
                _SampleMagazineSize.ValueT = value;
                OnPropertyChanged("SampleMagazineSize");
            }
        }

        private ACPropertyConfigValue<string> _PAFSamplingACUrl;
        [ACPropertyConfig("en{'PAFSampling ACUrl'}de{'PAFSampling ACUrl'}")]
        public string PAFSamplingACUrl
        {
            get => _PAFSamplingACUrl.ValueT;
            set => _PAFSamplingACUrl.ValueT = value;
        }

        private ACRef<ACLabOrderManager> _LabOrderManager;
        private ACLabOrderManager LabOrderManager
        {
            get => _LabOrderManager?.ValueT;
        }

        #endregion

        #region Methods

        [ACMethodInteractionClient("", "en{'Empty magazine'}de{'Karusell entleeren'}", 450, false, "", false)]
        public static async void EmptySampleMagazine(IACComponent acComponent)
        {
            if (acComponent == null || !IsEnabledEmptySampleMagazine(acComponent))
                return;

            if(await acComponent.Messages.QuestionAsync(acComponent, "Question50044", Global.MsgResult.Yes) == Global.MsgResult.Yes)
                acComponent.ACUrlCommand("!OnEmptySampleMagazine");
        }

        [ACMethodInfo("","",999)]
        public virtual void OnEmptySampleMagazine()
        {
            if (!IsEnabledOnEmptySampleMagazine())
                return;

            LabOrderManager?.OnEmptySampleMagazine();
            ReqEmptyMagazine();
        }

        public static bool IsEnabledEmptySampleMagazine(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;

            bool? result = acComponent.ACUrlCommand("!IsEnabledOnEmptySampleMagazine") as bool?;

            return result.HasValue ? result.Value : false;
        }

        [ACMethodInfo("", "", 999)]
        public virtual bool IsEnabledOnEmptySampleMagazine()
        {
            SampleMagazineState state = GetSampleMagazineState();

            if (state != SampleMagazineState.Idle && state != SampleMagazineState.Full)
                return false;

            if (string.IsNullOrEmpty(PAFSamplingACUrl))
                return false;

            PAProcessFunction processFunction = ACUrlCommand(PAFSamplingACUrl) as PAProcessFunction;
            if (processFunction == null)
                return false;

            if (processFunction.CurrentACState != ACStateEnum.SMIdle)
                return false;

            if (LabOrderManager == null)
                return false;

            return true;
        }

        [ACMethodInfo("", "", 999)]
        public virtual SampleMagazineState GetSampleMagazineState()
        {
            if(ActualPosition == null || TargetPosition == null)
            {
                //The TargetPosition or AcutalPosition is null!
                Msg msg = new Msg(this, eMsgLevel.Error, "PAESampleMagazine", "GetSampleMagazineState(10)", 158, "Error50297");
                Messages.LogMessageMsg(msg);
                OnNewAlarmOccurred(StateAlarm, msg);
                return SampleMagazineState.WrongState;
            }

            if (TargetPosition.ValueT == ActualPosition.ValueT || 
                (ApplicationManager != null && ApplicationManager.IsSimulationOn && TargetPosition.ValueT == SimActualPosition))
            {
                if (TargetPosition.ValueT >= SampleMagazineSize)
                    return SampleMagazineState.Full;
                return SampleMagazineState.Idle;
            }
            else
            {
                if (TargetPosition.ValueT == 0)
                    return SampleMagazineState.Emptying;
                else if (TargetPosition.ValueT > ActualPosition.ValueT || 
                    (ApplicationManager != null && ApplicationManager.IsSimulationOn && TargetPosition.ValueT > SimActualPosition))
                    return SampleMagazineState.Turning;
                return SampleMagazineState.WrongState;
            }
        }

        public MsgWithDetails LogExamples()
        {
            // Error-Message for Line 100 in Method LogExamples() with translated text from table ACClassMessage where ACIdentifier == "Error50297"
            Msg msg = new Msg(this, eMsgLevel.Error, GetType().Name, "LogExamples(100)", 100, "Error50297");
            Messages.LogMessageMsg(msg);

            // Warning-Message for Line 100 in Method LogExamples() with translated text from table ACClassMessage where ACIdentifier == "Warning50298"
            // The translated text contains two placeholders {0} and {1}. They are replaced by String.Format() 
            // with the last two optional parameters that are passed in the constructor of Msg.
            Msg msg2 = new Msg(this, eMsgLevel.Error, GetType().Name, "LogExamples(110)", 110, "Warning50298", "Param1", (int)2);
            Messages.LogMessageMsg(msg2);

            MsgWithDetails mergedMessages = new MsgWithDetails(this, eMsgLevel.Error, GetType().Name, "LogExamples(120)", 120, "Error50299");
            mergedMessages.MsgDetails.Add(msg);
            mergedMessages.MsgDetails.Add(msg2);
            Messages.LogMessageMsg(mergedMessages);

            return mergedMessages;
        }

        [ACMethodInfo("", "", 999)]
        public virtual bool ReqNextMagazinePosition()
        {
            if (GetSampleMagazineState() != SampleMagazineState.Idle)
                return false;

            TargetPosition.ValueT++;
            ThreadPool.QueueUserWorkItem((object state) => SimulateNextMagazinePosition(5000));
            return true;
        }

        private void SimulateNextMagazinePosition(int changePosAfterMillisec)
        {
            if(ApplicationManager != null && ApplicationManager.IsSimulationOn)
            {
                Thread.Sleep(changePosAfterMillisec);
                SimActualPosition = TargetPosition.ValueT;
            }
        }

        [ACMethodInfo("", "", 999)]
        public virtual void ReqEmptyMagazine()
        {
            TargetPosition.ValueT = 0;
        }

        [ACMethodInfo("","",999)]
        public virtual void CompleteEmtpying()
        {
            if(ApplicationManager != null && ApplicationManager.IsSimulationOn)
            {
                ActualPosition.ValueT = 0;
            }
        }

        #endregion

        #region Handle execute helpers

        public static bool HandleExecuteACMethod_PAESampleMagazine(out object result, IACComponent acComponent, string acMethodName, ACClassMethod acClassMethod, object[] acParameter)
        {
            result = null;
            switch(acMethodName)
            {
                case "EmptySampleMagazine":
                    EmptySampleMagazine(acComponent);
                    return true;
                case "IsEnabledEmptySampleMagazine":
                    result = IsEnabledEmptySampleMagazine(acComponent);
                    return true;
            }
            return HandleExecuteACMethod_PAModule(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "OnEmptySampleMagazine":
                    OnEmptySampleMagazine();
                    return true;
                case "IsEnabledOnEmptySampleMagazine":
                    result = IsEnabledOnEmptySampleMagazine();
                    return true;
                case "GetSampleMagazineState":
                    result = GetSampleMagazineState();
                    return true;
                case "ReqNextMagazinePosition":
                    ReqNextMagazinePosition();
                    return true;
                case "ReqEmptyMagazine":
                    ReqEmptyMagazine();
                    return true;
                case "CompleteEmtpying":
                    CompleteEmtpying();
                    return true;
            }

            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }

    public enum SampleMagazineState : short
    {
        Idle = 0,
        Turning = 10,
        Full = 20,
        Emptying = 30,
        WrongState = 40
    }
}
