using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.facility;
using gip.mes.datamodel;

namespace gip.mes.processapplication
{

    [ACClassInfo(Const.PackName_VarioAutomation, "en{'PWMethodVBBase'}de{'PWMethodVBBase'}", Global.ACKinds.TPWMethod, Global.ACStorableTypes.Optional, true, true, "", "PWMethodVBBase", 20)]
    [ACClassConstructorInfo(
        new object[] 
        { 
            new object[] { core.datamodel.ACProgram.ClassName, Global.ParamOption.Required, typeof(Guid)},
            new object[] { core.datamodel.ACProgramLog.ClassName, Global.ParamOption.Optional, typeof(Guid)},
            new object[] {PWProcessFunction.C_InvocationCount, Global.ParamOption.Optional, typeof(int)},
            new object[] { PWMethodVBBase.IsLastBatchParamName, Global.ParamOption.Optional, typeof(Int16) }
        }
    )]
    public abstract class PWMethodVBBase : PWProcessFunction 
    {
        new public const string PWClassName = "PWMethodVBBase";
        public const string IsLastBatchParamName = "LastBatch";

        #region c´tors

        static PWMethodVBBase()
        {
            RegisterExecuteHandler(typeof(PWMethodVBBase), HandleExecuteACMethod_PWMethodVBBase);
        }

        public PWMethodVBBase(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool result = base.ACInit(startChildMode);
            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");
            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _ACFacilityManager = null;
                _ExtraDisTargetDest = null;
                _ExtraDisTargetComp = null;
                _IsLastBatch = null;
            }

            bool init = base.ACDeInit(deleteACClassTask);
            return init;
        }

        public override void Recycle(IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        {

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _ACFacilityManager = null;
                _ExtraDisTargetDest = null;
                _ExtraDisTargetComp = null;
                _IsLastBatch = null;
            }
            base.Recycle(content, parentACObject, parameter, acIdentifier);
        }


        #endregion


        #region Properties

        protected string _ExtraDisTargetDest = null;
        [ACPropertyInfo(false, 9999)]
        public string ExtraDisTargetDest
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _ExtraDisTargetDest;
                }
            }
            set
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _ExtraDisTargetDest = value;
                    _ExtraDisTargetComp = null;
                }
                OnPropertyChanged("ExtraDisTargetDest");
            }
        }

        private ACComponent _ExtraDisTargetComp = null;
        public ACComponent ExtraDisTargetComp
        {
            get
            {
                // Benutzer hat noch gar kein Ziel definiert wo es hingehen soll
                if (ExtraDisTargetDest == null)
                {
                    return null;
                }

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_ExtraDisTargetComp != null)
                        return _ExtraDisTargetComp;
                }

                var extraDisTargetComp = ResolveExtraDisDest(this, ExtraDisTargetDest);

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _ExtraDisTargetComp = extraDisTargetComp;
                    return _ExtraDisTargetComp;
                }
            }
        }

        protected ACRef<ACComponent> _ACFacilityManager = null;
        public FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }


        public readonly ACMonitorObject _62000_PWGroupLockObj = new ACMonitorObject(62000);

        protected PADosingLastBatchEnum? _IsLastBatch = null;
        public PADosingLastBatchEnum IsLastBatch
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_IsLastBatch.HasValue)
                        return _IsLastBatch.Value;
                }
                PADosingLastBatchEnum isLastBatch = PADosingLastBatchEnum.None;
                if (CurrentTask != null || CurrentACMethod.ValueT != null)
                {
                    ACValue acValue = null;
                    // Falls asynchron aufgerufen
                    if (CurrentTask != null)
                        acValue = CurrentTask.ACMethod.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName);
                    // Sons synchron aufgerufen
                    else
                        acValue = CurrentACMethod.ValueT.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName);
                    if (acValue != null && acValue.Value != null)
                        isLastBatch = (PADosingLastBatchEnum)acValue.ParamAsInt16;
                    else
                        isLastBatch = PADosingLastBatchEnum.None;
                }
                else
                    isLastBatch = PADosingLastBatchEnum.None;


                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _IsLastBatch = isLastBatch;
                    return _IsLastBatch.Value;
                }
            }
            set
            {
                bool changed = false;

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    changed = _IsLastBatch != value;
                    _IsLastBatch = value;
                }
                if (changed && (CurrentTask != null || CurrentACMethod.ValueT != null))
                {
                    ACValue acValue = null;
                    // Falls asynchron aufgerufen
                    if (CurrentTask != null)
                        acValue = CurrentTask.ACMethod.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName);
                    // Sons synchron aufgerufen
                    else
                        acValue = CurrentACMethod.ValueT.ParameterValueList.GetACValue(PWMethodVBBase.IsLastBatchParamName);
                    if (acValue != null)
                        acValue.Value = (Int16)value;
                    if (CurrentTask != null)
                        OnPropertyChanged("CurrentTask");
                }
            }
        }


        #endregion

        #region overrides
        public override void Reset()
        {

            using (ACMonitor.Lock(_20015_LockValue))
            {
                ExtraDisTargetDest = null;
                _ExtraDisTargetComp = null;
                _IsLastBatch = null;
            }
            base.Reset();
        }
        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "SwitchToEmptyingMode":
                    SwitchToEmptyingMode();
                    return true;
                case Const.IsEnabledPrefix + "SwitchToEmptyingMode":
                    result = IsEnabledSwitchToEmptyingMode();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PWMethodVBBase(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case Const.AskUserPrefix + "SwitchToEmptyingMode":
                    result = AskUserSwitchToEmptyingMode(acComponent);
                    return true;
            }
            return HandleExecuteACMethod_PWProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region Public
        public static ACComponent ResolveExtraDisDest(ACComponent invoker, string acUrlExtraDisDest)
        {
            if (String.IsNullOrWhiteSpace(acUrlExtraDisDest))
                return null;
            acUrlExtraDisDest = acUrlExtraDisDest.Trim();
            if (String.IsNullOrEmpty(acUrlExtraDisDest))
                return null;
            ACComponent module = null;
            if (ACUrlValidation.ContainsACUrlDelimiters(acUrlExtraDisDest))
            {
                module = invoker.ACUrlCommand(acUrlExtraDisDest) as ACComponent;
                if (module != null)
                    return module;
            }
            using (var dbApp = new DatabaseApp())
            {
                Facility facility = dbApp.Facility.Where(c => c.FacilityNo == acUrlExtraDisDest).FirstOrDefault();
                if (facility == null)
                    return null;
                if (!facility.VBiFacilityACClassID.HasValue)
                    return null;
                var acClass = facility.FacilityACClass;
                if (acClass == null)
                    return null;
                string acUrlComponent = acClass.GetACUrlComponent();
                if (String.IsNullOrEmpty(acUrlComponent))
                    return null;
                module = invoker.ACUrlCommand(acUrlComponent) as ACComponent;
                if (module != null)
                    return module;
            }
            return null;
        }
        #endregion

        #region User Interaction Methods
        [ACMethodInteraction("", "en{'Switch to emptying mode'}de{'Leerfahrmodus aktivieren'}", 297, true)]
        public virtual void SwitchToEmptyingMode()
        {
            if (!IsEnabledSwitchToEmptyingMode())
                return;
            CurrentACSubState = (uint) ACSubStateEnum.SMEmptyingMode;
        }

        public virtual bool IsEnabledSwitchToEmptyingMode()
        {
            return !((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode) 
                && !((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode);
        }

        public static bool AskUserSwitchToEmptyingMode(IACComponent acComponent)
        {
            if (acComponent == null)
                return false;
            ACComponent _this = acComponent as ACComponent;
            // "Question50032" Do you wan't to switch to emtying mode?
            Global.MsgResult questionRes = acComponent.Messages.Question(acComponent, "Question50032", Global.MsgResult.Yes);
            if (questionRes == Global.MsgResult.Yes)
            {
                EnterExtraDisTargetDest(acComponent);
                return true;
            }
            return false;
        }

        public override bool IsInSkippingMode
        {
            get
            {
                return ((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled)
                    || ((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                    || ((ACSubStateEnum)CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode);
            }
        }



        private static ACClassInfoWithItems.VisibilityFilters _ExtraDisCompSelectorFilter;
        public static ACClassInfoWithItems.VisibilityFilters ExtraDisCompSelectorFilter
        {
            get
            {
                if (_ExtraDisCompSelectorFilter == null)
                {
                    _ExtraDisCompSelectorFilter = new ACClassInfoWithItems.VisibilityFilters()
                    {
                        IncludeTypes = new List<Type>() { typeof(PAMSilo), typeof(PAMParkingspace), typeof(PAMIntermediatebin) }
                    };
                }
                return _ExtraDisCompSelectorFilter;
            }
        }


        public static void EnterExtraDisTargetDest(IACComponent acComponent, string questionID= "Question50033")
        {
            core.datamodel.ACClass selectedClass = null;

            string bsoName = "BSOComponentSelector(Dialog)";
            IACBSO childBSO = acComponent.Root.Businessobjects.ACUrlCommand("?" + bsoName) as IACBSO;
            if (childBSO == null)
                childBSO = acComponent.Root.Businessobjects.StartComponent(bsoName, null, new object[] { }) as IACBSO;
            if (childBSO != null)
            {
                acComponent.Messages.Info(acComponent, questionID);
                // childBSO.ProjectFilterTypes = new Global.ACProjectTypes[1] { Global.ACProjectTypes.Applicationproject };

                string filterProject = "";
                IAppManager appManager = acComponent as IAppManager;
                if (appManager == null)
                    appManager = acComponent.FindParentComponent<IAppManager>(c => c is IAppManager);
                if (appManager != null)
                    filterProject = appManager.ComponentClass.ACProject.ACIdentifier;

                selectedClass = childBSO.ACUrlCommand("!ShowComponentSelector", ExtraDisCompSelectorFilter, filterProject, "") as core.datamodel.ACClass;
                childBSO.Stop();
            }

            string extraDisTargetDest = "";
            if (selectedClass == null)
            {
                // "Question50033" Please enter the Facility-No. or the address (ACUrl) of the alternative Target if you want to reject the batch. If you wan't to transport it to the originally planned target leave it blank.?
                string header = acComponent.Root.Environment.TranslateMessage(acComponent, questionID);
                extraDisTargetDest = acComponent.Messages.InputBox(header, "");
            }
            else
            {
                extraDisTargetDest = selectedClass.GetACUrlComponent();
            }
            if (String.IsNullOrEmpty(extraDisTargetDest))
                extraDisTargetDest = " ";
            acComponent.ACUrlCommand("ExtraDisTargetDest", extraDisTargetDest);
        }
        #endregion
    }
}
