using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;
using System.Data.Objects;
using gip.mes.datamodel;
using gip.mes.facility;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Shared store (remote)'}de{'Gemeinsam verwendetes Lager'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMRemoteStore : PAMParkingspace
    {
        #region c'tors
        static PAMRemoteStore()
        {
            RegisterExecuteHandler(typeof(PAMRemoteStore), HandleExecuteACMethod_PAMRemoteStore);
        }

        public PAMRemoteStore(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool initialized = base.ACInit(startChildMode);
            if (initialized && RemoteStorePostings == null)
                RemoteStorePostings = new SafeList<RemoteStorePostingData>();
            return initialized;
        }

        public override bool ACPostInit()
        {
            StartRedirection();
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            StopRedirection();
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Points
        [ACPropertyPointConfig(9999, "", typeof(RemoteStoreConfig), "en{'Remote Facility Manager'}de{'Entfernte Facility Manager'}")]
        public List<core.datamodel.ACClassConfig> RemoteFacilityManager
        {
            get
            {
                string keyACUrl = ".\\ACClassProperty(RemoteFacilityManager)";
                List<core.datamodel.ACClassConfig> result = null;
                ACClassTaskQueue.TaskQueue.ProcessAction(() =>
                {
                    try
                    {
                        ACTypeFromLiveContext.ACClassConfig_ACClass.Load(MergeOption.OverwriteChanges);
                        var query = ACTypeFromLiveContext.ACClassConfig_ACClass.Where(c => c.KeyACUrl == keyACUrl);
                        if (query.Any())
                            result = query.ToList();
                        else
                            result = new List<core.datamodel.ACClassConfig>();
                    }
                    catch (Exception e)
                    {
                        Messages.LogException(this.GetACUrl(), "RemoteFacilityManager", e.Message);
                    }
                });
                return result;
            }
        }
        private int _CountConfigReads = 0;
        #endregion

        #region Internal Class
        public class RemoteInstance
        {
            public ACRef<ACComponentProxy> Recipient { get; set; }
            public RemoteStoreConfig Config { get; set; }
        }
        #endregion

        #region Properties
        [ACPropertyInfo(true, 400, DefaultValue = false)]
        public bool ConnectionDisabled
        {
            get;
            set;
        }

        private List<RemoteInstance> _RemoteInstances = new List<RemoteInstance>();
        protected IEnumerable<RemoteInstance> RemoteInstances
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _RemoteInstances.ToArray();
                }
            }
        }

        [ACPropertyBindingSource]
        public IACContainerTNet<bool> IsActive { get; set; }

        SafeList<RemoteStorePostingData> _RemoteStorePostings;
        [ACPropertyInfo(true, 520, "", "en{'RemoteStorePostings'}de{'RemoteStorePostings'}", "", false)]
        public SafeList<RemoteStorePostingData> RemoteStorePostings
        {
            get
            {
                return _RemoteStorePostings;
            }
            set
            {
                _RemoteStorePostings = value;
                OnPropertyChanged("RemoteStorePostings");
            }
        }


        #endregion


        #region Methods
        public override void RefreshFacility(bool preventBroadcast, Guid? fbID)
        {
            base.RefreshFacility(preventBroadcast, fbID);

            if (   Facility.ValueT == null 
                || Facility.ValueT.ValueT == null 
                || preventBroadcast
                || !RemoteInstances.Any()
                || !IsActive.ValueT)
                return;
            SendToRemoteStore(FacilityBooking.ClassName, fbID);
        }

        [ACMethodInfo("", "en{'Send Picking order'}de{'Sende Kommissionierauftrag'}", 401, true)]
        public virtual void SendPicking(bool preventBroadcast, Guid? pickingID)
        {
            if (Facility.ValueT == null
                || Facility.ValueT.ValueT == null
                || preventBroadcast
                || !RemoteInstances.Any()
                || !IsActive.ValueT)
                return;

            SendToRemoteStore(Picking.ClassName, pickingID);
        }

        protected virtual void SendToRemoteStore(string entityType, Guid? keyID)
        {
            var appManager = this.ApplicationManager;
            if (appManager == null)
                return;
            foreach (RemoteInstance remoteInstance in RemoteInstances)
            {
                RemoteStorePostingData postingData = RemoteStorePostings.Where(c => c.Recipient == remoteInstance.Recipient.ACUrl).FirstOrDefault();
                if (remoteInstance.Recipient.ValueT.ConnectionState >= ACObjectConnectionState.Connected)
                {
                    if (postingData != null)
                    {
                        RemoteStorePostings.Remove(postingData);
                        OnPropertyChanged("RemoteStorePostings"); // Persist
                    }
                    else
                    {
                        postingData = new RemoteStorePostingData(remoteInstance.Recipient.ACUrl, remoteInstance.Config.FacilityUrlOfRecipient);
                        if (keyID.HasValue)
                            postingData.FBIds.Add(new RSPDEntry() { EntityType = entityType, KeyId = keyID.Value });
                    }
                    if (appManager.ApplicationQueue != null)
                    {
                        appManager.ApplicationQueue.Add(() => { remoteInstance.Recipient.ValueT.ACUrlCommand(ACUrlHelper.Delimiter_InvokeMethod + gip.mes.datamodel.Facility.MN_RefreshFacility, this.GetACUrl(), postingData); });
                    }
                }
                // If no connection to remote side, than start queuing messages
                else
                {
                    if (postingData != null)
                    {
                        if (!keyID.HasValue)
                            return;
                    }
                    else
                    {
                        postingData = new RemoteStorePostingData(remoteInstance.Recipient.ACUrl, remoteInstance.Config.FacilityUrlOfRecipient);
                        RemoteStorePostings.Add(postingData);
                    }
                    if (keyID.HasValue)
                        postingData.FBIds.Add(new RSPDEntry() { EntityType = entityType, KeyId = keyID.Value });
                    OnPropertyChanged("RemoteStorePostings"); // Persist
                }
            }
        }

        private void Proxy_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ACComponentProxy proxy = sender as ACComponentProxy;
            if (proxy == null)
                return;
            // Resend persisted messages after reconnection
            if (e.PropertyName == "ConnectionState" && proxy.ConnectionState >= ACObjectConnectionState.Connected)
            {
                ResendData(proxy);
            }
        }

        private void ResendUnsent()
        {
            foreach (RemoteStorePostingData postingData in RemoteStorePostings)
            {
                RemoteInstance remoteInstance = RemoteInstances.Where(c => c.Recipient.ACUrl == postingData.Recipient).FirstOrDefault();
                if (remoteInstance.Recipient.ValueT.ConnectionState >= ACObjectConnectionState.Connected)
                {
                    ResendData(remoteInstance.Recipient.ValueT as ACComponentProxy, postingData);
                }
            }
        }

        private void ResendData(ACComponentProxy proxy)
        {
            string acUrlOfProxy = proxy.GetACUrl();
            RemoteStorePostingData postingData = RemoteStorePostings.Where(c => c.Recipient == acUrlOfProxy).FirstOrDefault();
            if (postingData != null)
                ResendData(proxy, postingData);
        }

        private void ResendData(ACComponentProxy proxy, RemoteStorePostingData postingData)
        {
            if (postingData == null || proxy == null)
                return;
            var appManager = this.ApplicationManager;
            if (appManager == null)
                return;

            string acUrlOfProxy = proxy.GetACUrl();
            RemoteStorePostings.Remove(postingData);
            OnPropertyChanged("RemoteStorePostings"); // Persist
            if (appManager.ApplicationQueue != null)
            {
                appManager.ApplicationQueue.Add(() => { proxy.ACUrlCommand(ACUrlHelper.Delimiter_InvokeMethod + gip.mes.datamodel.Facility.MN_RefreshFacility, this.GetACUrl(), postingData); });
            }
        }


        [ACMethodInteraction("Run", "en{'Activate redirection'}de{'Umleitung aktivieren'}", 200, true)]
        public void StartRedirection()
        {
            if (!IsEnabledStartRedirection())
                return;

            var propertyQuery = RemoteFacilityManager;
            if (propertyQuery == null || !propertyQuery.Any())
                return;
            if (_CountConfigReads > 0)
                propertyQuery.ForEach(c => c.ACProperties.Refresh());
            _CountConfigReads++;
            IEnumerable<IACConfig> redirectList = propertyQuery.Where(c => c.Value != null
                                                && (c.Value is RemoteStoreConfig));
            if (redirectList == null || !redirectList.Any())
                return;

            foreach (IACConfig acConfig in redirectList)
            {
                RemoteStoreConfig redirectConfig = acConfig.Value as RemoteStoreConfig;
                if (redirectConfig == null)
                    continue;
                if (String.IsNullOrEmpty(redirectConfig.Recipient))
                    continue;

                ACComponentProxy proxy = ACUrlCommand(redirectConfig.Recipient) as ACComponentProxy;
                if (proxy != null)
                {
                    using (ACMonitor.Lock(_20015_LockValue))
                    {
                        proxy.PropertyChanged += Proxy_PropertyChanged;
                        _RemoteInstances.Add(new RemoteInstance() { Recipient = new ACRef<ACComponentProxy>(proxy, this), Config = redirectConfig });
                    }
                }
            }

            IsActive.ValueT = RemoteInstances.Any();
            ResendUnsent();
        }

        public virtual bool IsEnabledStartRedirection()
        {
            return !ConnectionDisabled && !RemoteInstances.Any();
        }

        [ACMethodInteraction("Run", "en{'Stop redirection'}de{'Umleitung beenden'}", 201, true)]
        public void StopRedirection()
        {
            if (!IsEnabledStopRedirection())
                return;
            using (ACMonitor.Lock(_20015_LockValue))
            {
                foreach (var remoteInstance in RemoteInstances)
                {
                    if (remoteInstance.Recipient.IsAttached)
                    {
                        remoteInstance.Recipient.ValueT.PropertyChanged -= Proxy_PropertyChanged;
                        remoteInstance.Recipient.Detach();
                    }
                }
                _RemoteInstances = new List<RemoteInstance>();
            }
            IsActive.ValueT = false;
        }

        public virtual bool IsEnabledStopRedirection()
        {
            return RemoteInstances != null && RemoteInstances.Any();
        }

        #endregion


        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "StartRedirection":
                    StartRedirection();
                    return true;
                case "StopRedirection":
                    StopRedirection();
                    return true;
                case Const.IsEnabledPrefix + "StartRedirection":
                    result = IsEnabledStartRedirection();
                    return true;
                case Const.IsEnabledPrefix + "StopRedirection":
                    result = IsEnabledStopRedirection();
                    return true;
                case gip.mes.datamodel.Facility.MN_SendPicking:
                    SendPicking((bool)acParameter[0], (Guid?)acParameter[1]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PAMRemoteStore(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAMParkingspace(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion
    }

}
