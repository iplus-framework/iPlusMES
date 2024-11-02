// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.communication;

namespace gip2006.variobatch.processapplication
{
    /// <summary>
    /// Baseclass for multiplexing Configuration-Parameter
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006ParamMUX'}de{'GIPConv2006ParamMUX'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, true)]
    public class GIPConv2006ParamMUX : ACComponent
    {
        #region c'tors
        public GIPConv2006ParamMUX(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            if (ParamValue != null && ParamValue is IACPropertyNetServer)
                (ParamValue as IACPropertyNetServer).ValueUpdatedOnReceival += ParamValue_ValueUpdatedOnReceival;
            ReqParamNoT.ValueUpdatedOnReceival += ReqParamNo_ValueUpdatedOnReceival;
            if (MUXCommandPLCT != null)
                MUXCommandPLCT.ValueUpdatedOnReceival += MUXCommandPLC_ValueUpdatedOnReceival;

            StartPollingIntern(false);
            var propertiesToQuery = DAPropertiesToQuery;
            if (propertiesToQuery != null)
            {
                foreach (IACPropertyNetSource sourceProperty in propertiesToQuery)
                {
                    sourceProperty.ValueUpdatedOnReceival += ModelProperty_ValueUpdatedOnReceival;
                }
            }

            return true;
        }


        public override bool ACPostInit()
        {
            var daPropertiesToQuery = DAPropertiesToQuery;
            if (daPropertiesToQuery != null && daPropertiesToQuery.Any())
                SubscribeToProjectWorkCycle();
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            SwitchOffPolling();

            var daProperties = DAProperties;
            if (daProperties != null)
            {
                foreach (IACPropertyNetSource sourceProperty in daProperties)
                {
                    sourceProperty.ValueUpdatedOnReceival -= ModelProperty_ValueUpdatedOnReceival;
                }
            }

            _Session = null;
            _SessionCheckCounter = 0;
            _SessionChecked = false;

            if (ParamValue != null && ParamValue is IACPropertyNetServer)
                (ParamValue as IACPropertyNetServer).ValueUpdatedOnReceival -= ParamValue_ValueUpdatedOnReceival;
            ReqParamNoT.ValueUpdatedOnReceival -= ReqParamNo_ValueUpdatedOnReceival;
            if (MUXCommandPLCT != null)
                MUXCommandPLCT.ValueUpdatedOnReceival -= MUXCommandPLC_ValueUpdatedOnReceival;


            return base.ACDeInit(deleteACClassTask);
        }
        #endregion


        #region Properties

        #region private
        protected IEnumerable<IACPropertyNetSource> DAProperties
        {
            get
            {

                using (ACMonitor.Lock(LockMemberList_20020))
                {
                    try
                    {
                        var query = ACMemberList.Where(c => c is IACPropertyNetSource
                                                                        && c.ACType != null
                                                                         && c.ACType.ACKind == Global.ACKinds.PSPropertyExt
                                                                         && c.ACType.SortIndex >= 0
                                                                         && c.ACType.SortIndex < 9999)
                                                            .Select(c => c as IACPropertyNetSource)
                                                            .ToArray();
                        return query;
                    }
                    catch (Exception e)
                    {
                        string msg = e.Message;
                        if (e.InnerException != null && e.InnerException.Message != null)
                            msg += " Inner:" + e.InnerException.Message;

                        Messages.LogException("GIPConv2006ParamMUX", "DAProperties", msg);
                    }
                }
                return null;
            }
        }

        private SafeList<IACPropertyNetSource> _DAPropertiesToQuery = null;
        protected SafeList<IACPropertyNetSource> DAPropertiesToQuery
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _DAPropertiesToQuery;
                }
            }
        }

        private IACPropertyNetSource _CurrentProp2Query = null;
        public IACPropertyNetSource CurrentProp2Query
        {
            get
            {

                using (ACMonitor.Lock(_40090_MUXMutex))
                {
                    return _CurrentProp2Query;
                }
            }
            set
            {

                using (ACMonitor.Lock(_40090_MUXMutex))
                {
                    _CurrentProp2Query = value;
                }
            }
        }

        private IACPropertyNetSource _CurrentProp2Write = null;

        public IACPropertyNetSource CurrentProp2Write
        {
            get
            {

                using (ACMonitor.Lock(_40090_MUXMutex))
                {
                    return _CurrentProp2Write;
                }
            }
            set
            {

                using (ACMonitor.Lock(_40090_MUXMutex))
                {
                    _CurrentProp2Write = value;
                }
            }
        }

        private bool _LockResend = false;
        private bool _LockRewrite = false;
        private short _ParamValueQueried = 0;
        private short _MUXCmdQueried = 0;
        private bool _IsReadyForWriting = false;
        private readonly ACMonitorObject _40090_MUXMutex = new ACMonitorObject(40090);

        public Type GetDataTypeOfProp(IACPropertyBase property)
        {
            if (property == null)
                return null;
            if ((property.Value != null) && (property.Value is ACCustomTypeBase))
                return (property.Value as ACCustomTypeBase).TypeOfValueT;
            else
                return property.ACType.ObjectType;
        }
        private int _CurrentProp2QueryLoopCount = 0;


        public ApplicationManager ApplicationManager
        {
            get
            {
                return FindParentComponent<ApplicationManager>(c => c is ApplicationManager);
            }
        }

        private bool _SubscribedToWorkCycle = false;
        public bool IsSubscribedToWorkCycle
        {
            get
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _SubscribedToWorkCycle;
                }
            }
        }

        private int _SessionCheckCounter = 0;
        private bool _SessionChecked = false;
        private ACSession _Session = null;
        public ACSession Session
        {
            get
            {
                if (_Session == null)
                {
                    if (_SessionChecked)
                        return null;
                    IACPropertyNetSource sourcePropDA = (ParamValue as IACPropertyNetTarget).Source;
                    if (sourcePropDA != null)
                    {
                        ParamValue.ForceBroadcast = true;
                        sourcePropDA.ForceBroadcast = true;
                        if ((sourcePropDA.ParentACComponent != null) // Sollte ACSubscription sein
                            && (sourcePropDA.ParentACComponent.ParentACComponent != null)) // Sollte ACSession sein
                        {
                            _Session = sourcePropDA.ParentACComponent.ParentACComponent as ACSession;
                            _SessionChecked = true;
                        }
                    }
                    else if (Root.Initialized)
                    {
                        _SessionCheckCounter++;
                        if (_SessionCheckCounter > 60)
                            _SessionChecked = true;
                    }
                }
                return _Session;
            }
        }

        public bool IsReadyForSending
        {
            get
            {
                if (!this.Root.Initialized)
                    return false;
                if (this.Session != null)
                    return Session.IsReadyForWriting;
                return false;
            }
        }

        #endregion


        #region Binding Properties
        [ACPropertyBindingTarget(241, "Read/Write PLC", "en{'ParamValue'}de{'ParamValue'}", "", false, false)]
        public IACContainerTNet<Double> ParamValue { get; set; }
        private void ParamValue_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {

                using (ACMonitor.Lock(_40090_MUXMutex))
                {
                    if (_CurrentProp2Query != null)
                    {
                        _ParamValueQueried = 2;
                        _LockResend = true;
                        try
                        {
                            Type propType = GetDataTypeOfProp(_CurrentProp2Query);
                            if ((propType != null) && (propType != typeof(Double)))
                            {
                                _CurrentProp2Query.Value = Convert.ChangeType(e.ValueEvent.ChangedValue, propType);
                            }
                            else
                            {
                                _CurrentProp2Query.Value = e.ValueEvent.ChangedValue;
                            }
                        }
                        catch (Exception ex)
                        {
                            string msg = ex.Message;
                            if (ex.InnerException != null && ex.InnerException.Message != null)
                                msg += " Inner:" + ex.InnerException.Message;

                            Messages.LogException("GIPConv2006ParamMUX", "OnSetParamValue", msg);
                        }
                        _LockResend = false;

                        ManageQueryResponseState();
                    }
                    else if ((_CurrentProp2Write != null) && !_LockRewrite)
                    {
                        _ParamValueQueried = 2;
                        _LockResend = true;
                        try
                        {
                            Type propType = GetDataTypeOfProp(_CurrentProp2Write);
                            if ((propType != null) && (propType != typeof(Double)))
                            {
                                _CurrentProp2Write.Value = Convert.ChangeType(e.ValueEvent.ChangedValue, propType);
                            }
                            else
                            {
                                _CurrentProp2Write.Value = e.ValueEvent.ChangedValue;
                            }
                        }
                        catch (Exception ex)
                        {
                            string msg = ex.Message;
                            if (ex.InnerException != null && ex.InnerException.Message != null)
                                msg += " Inner:" + ex.InnerException.Message;

                            Messages.LogException("GIPConv2006ParamMUX", "OnSetParamValue", msg);
                        }
                        _LockResend = false;
                        ManageWriteResponseState();
                    }
                }
            }
        }

        public ACPropertyNetTarget<Double> ParamValueT
        {
            get
            {
                return ParamValue as ACPropertyNetTarget<Double>;
            }
        }


        [ACPropertyBindingTarget(250, "Write to PLC", "en{'ReqParamNo'}de{'ReqParamNo'}", "", false, false)]
        public IACContainerTNet<Int16> ReqParamNo { get; set; }
        private void ReqParamNo_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
        }

        public ACPropertyNetTarget<Int16> ReqParamNoT
        {
            get
            {
                return ReqParamNo as ACPropertyNetTarget<Int16>;
            }
        }


        [ACPropertyBindingTarget(251, "Write to PLC", "en{'MUX-Command (AGG_DIAG'}de{'MUX-Command (AGG_DIAG)'}", "", false, false)]
        public IACContainerTNet<Int16> MUXCommandPLC { get; set; }
        private void MUXCommandPLC_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.AfterBroadcast)
                return;
            if ((e.ValueEvent.Sender == EventRaiser.Source) && (e.ValueEvent.EventType == EventTypes.ValueChangedInSource))
            {

                using (ACMonitor.Lock(_40090_MUXMutex))
                {
                    if (_CurrentProp2Query != null)
                    {
                        if ((Int16)e.ValueEvent.ChangedValue == 0)
                            _MUXCmdQueried = 2;
                        ManageQueryResponseState();
                    }
                    else if (_CurrentProp2Write != null)
                    {
                        if ((Int16)e.ValueEvent.ChangedValue == 0)
                            _MUXCmdQueried = 2;
                        ManageWriteResponseState();
                    }
                }
            }
        }

        public ACPropertyNetTarget<Int16> MUXCommandPLCT
        {
            get
            {
                return MUXCommandPLC as ACPropertyNetTarget<Int16>;
            }
        }

        #endregion



        #endregion

        #region Methods

        #region Handler
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "StartPolling":
                    StartPolling();
                    return true;
                case "SwitchOffPolling":
                    SwitchOffPolling();
                    return true;
                case Const.IsEnabledPrefix + "StartPolling":
                    result = IsEnabledStartPolling();
                    return true;
                case Const.IsEnabledPrefix + "SwitchOffPolling":
                    result = IsEnabledSwitchOffPolling();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        #region Private


        #region Cyclic Reading & Writing Logic

        private void objectManager_ProjectWorkCycle(object sender, EventArgs e)
        {
            //EnterLockMemberList();

            try
            {
                if (!IsReadyForSending)
                {
                    if (Session == null || !Session.ConnectionDisabled)
                        return;
                    if (Session.ConnectionDisabled)
                    {
                        SwitchOffPollingIntern(true);
                        return;
                    }
                }

                var daPropertiesToQuery = DAPropertiesToQuery;
                if (daPropertiesToQuery == null || !daPropertiesToQuery.Any())
                {
                    SwitchOffPollingIntern(true);
                    return;
                }


                using (ACMonitor.Lock(_40090_MUXMutex))
                {
                    if (_CurrentProp2Query != null)
                    {
                        _CurrentProp2QueryLoopCount++;
                        if (_CurrentProp2QueryLoopCount > 20) // Timeout
                        {
                            try
                            {
                                if (daPropertiesToQuery.Contains(_CurrentProp2Query))
                                    daPropertiesToQuery.Remove(_CurrentProp2Query);
                            }
                            catch (Exception ex)
                            {
                                Messages.LogException("GIPConv2006ParamMUX", "objectManager_ProjectWorkCycle", ex.Message);
                            }
                            _CurrentProp2Query = null;
                            _CurrentProp2QueryLoopCount = 0;
                        }
                        return;
                    }

                    _CurrentProp2Query = daPropertiesToQuery.First();
                    _CurrentProp2QueryLoopCount = 0;
                    _ParamValueQueried = 1;
                    _MUXCmdQueried = 1;
                }
                ReqParamNo.ValueT = _CurrentProp2Query.ACType.SortIndex;
                MUXCommandPLC.ValueT = 1; // Read
            }
            finally
            {
                //ExitLockMemberList();
            }
        }


        protected virtual void ModelProperty_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            try
            {
                if (phase == ACPropertyChangedPhase.AfterBroadcast)
                    return;
                if (!_IsReadyForWriting || !IsReadyForSending)
                    return;
                if (_LockResend
                    || CurrentProp2Query != null
                    || CurrentProp2Write != null)
                {
                    return;
                }

                IACPropertyNetSource currentProp2Write = sender as IACPropertyNetSource;
                CurrentProp2Write = currentProp2Write;

                _LockRewrite = true;
                ((IACMember)ParamValue).Value = Convert.ChangeType(currentProp2Write.Value, typeof(Double));
                _LockRewrite = false;

                ReqParamNo.ValueT = currentProp2Write.ACType.SortIndex;
                MUXCommandPLC.ValueT = 2; // Write

                using (ACMonitor.Lock(_40090_MUXMutex))
                {
                    _ParamValueQueried = 1;
                    _MUXCmdQueried = 1;
                }
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException("GIPConv2006ParamMUX", "ModelProperty_ValueUpdatedOnReceival", msg);

                _LockRewrite = false;
            }
            return;
        }


        private void ManageQueryResponseState()
        {
            if ((_MUXCmdQueried != 2) || (_ParamValueQueried != 2))
                return;
            var daProperties2Query = DAPropertiesToQuery;
            if (daProperties2Query != null)
            {
                try
                {
                    if (daProperties2Query.Contains(_CurrentProp2Query))
                        daProperties2Query.Remove(_CurrentProp2Query);
                }
                catch (Exception e)
                {
                    Messages.LogException("GIPConv2006ParamMUX", "ManageQueryResponseState", e.Message);
                }
            }
            _CurrentProp2Query = null;
            _CurrentProp2QueryLoopCount = 0;
            _ParamValueQueried = 0;
            _MUXCmdQueried = 0;
        }


        private void ManageWriteResponseState()
        {
            if ((_MUXCmdQueried != 2) || (_ParamValueQueried != 2))
                return;
            _CurrentProp2Write = null;
            _ParamValueQueried = 0;
            _MUXCmdQueried = 0;
        }

        #endregion


        #region Starting Polling

        private void StartPollingIntern(bool subscribeToWorkCycle)
        {
            SafeList<IACPropertyNetSource> daProperties2Query = null;
            IEnumerable<IACPropertyNetSource> daProperties = null;
            bool canStartPolling = IsEnabledStartPollingIntern(out daProperties2Query, out daProperties);
            if (!canStartPolling)
                return;

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _DAPropertiesToQuery = new SafeList<IACPropertyNetSource>(daProperties);
            }

            if (subscribeToWorkCycle)
                SubscribeToProjectWorkCycle();
        }

        private bool IsEnabledStartPollingIntern(out SafeList<IACPropertyNetSource> daProperties2Query, out IEnumerable<IACPropertyNetSource> daProperties)
        {
            daProperties2Query = DAPropertiesToQuery;
            if (daProperties2Query != null && daProperties2Query.Any())
            {
                daProperties = null;
                return false;
            }
            daProperties = DAProperties;
            return daProperties != null && daProperties.Any();
        }


        private void SwitchOffPollingIntern(bool unsubscribeToWorkCycle)
        {
            SafeList<IACPropertyNetSource> daProperties2Query = null;
            bool isEnabled = IsEnabledSwitchOffPollingIntern(out daProperties2Query);

            if (daProperties2Query != null && daProperties2Query.Any())
            {

                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _DAPropertiesToQuery = null;
                }
                daProperties2Query = null;
            }
            if (daProperties2Query == null || !daProperties2Query.Any())
                _IsReadyForWriting = true;

            UnSubscribeToProjectWorkCycle();


            using (ACMonitor.Lock(_40090_MUXMutex))
            {
                _ParamValueQueried = 2;
                _MUXCmdQueried = 2;
                ManageQueryResponseState();
            }
        }

        private bool IsEnabledSwitchOffPollingIntern(out SafeList<IACPropertyNetSource> daProperties2Query)
        {
            daProperties2Query = DAPropertiesToQuery;
            return IsSubscribedToWorkCycle && daProperties2Query != null && daProperties2Query.Any();
        }


        /// <summary>
        /// Aktiviert zyklisches Polling, damit zyklisch die State-Methoden aufgerufen werden
        /// </summary>
        protected void SubscribeToProjectWorkCycle()
        {

            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (!IsEnabledSubscribeToProjectWorkCycle())
                    return;
                ApplicationManager.ProjectWorkCycleR100ms += objectManager_ProjectWorkCycle;
                _SubscribedToWorkCycle = true;
            }
        }

        protected bool IsEnabledSubscribeToProjectWorkCycle()
        {

            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (_SubscribedToWorkCycle || ApplicationManager == null)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Deaktiviert zyklisches Polling
        /// </summary>
        protected void UnSubscribeToProjectWorkCycle()
        {

            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (!IsEnabledUnSubscribeToProjectWorkCycle())
                    return;
                ApplicationManager.ProjectWorkCycleR100ms -= objectManager_ProjectWorkCycle;
                _SubscribedToWorkCycle = false;
            }
        }

        protected bool IsEnabledUnSubscribeToProjectWorkCycle()
        {

            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (!_SubscribedToWorkCycle || ApplicationManager == null)
                    return false;
                return true;
            }
        }

        #endregion
        
        
        #endregion


        #region Public, Interaction

        [ACMethodInteraction("Process", "en{'Start Polling'}de{'Starte Polling'}", (short)303, true, "", Global.ACKinds.MSMethod, false, Global.ContextMenuCategory.ProcessCommands)]
        public void StartPolling()
        {
            StartPollingIntern(true);
        }


        public bool IsEnabledStartPolling()
        {
            SafeList<IACPropertyNetSource> daProperties2Query = null;
            IEnumerable<IACPropertyNetSource> daProperties = null;
            return IsEnabledStartPollingIntern(out daProperties2Query, out daProperties);
        }


        [ACMethodInteraction("Process", "en{'Switch off polling'}de{'Polling ausschalten'}", 303, true, "", Global.ACKinds.MSMethod, false, Global.ContextMenuCategory.ProcessCommands)]
        public void SwitchOffPolling()
        {
            SwitchOffPollingIntern(false);
        }

        public bool IsEnabledSwitchOffPolling()
        {
            SafeList<IACPropertyNetSource> daProperties2Query = null;
            return IsEnabledSwitchOffPollingIntern(out daProperties2Query);
        }
        
        #endregion


        #endregion
    }
}
