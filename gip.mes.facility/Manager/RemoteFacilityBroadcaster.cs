using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Globalization;
using System.Data.Objects;

namespace gip.mes.facility
{
    public interface IRemoteFacilityForwarder : IACComponent
    {
        IACContainerTNet<bool> IsActive { get; set; }
        bool ConnectionDisabled { get; set; }
        SafeList<RemoteStorePostingData> RemoteStorePostings { get; set; }
        ApplicationManager ApplicationManager { get; }
        List<core.datamodel.ACClassConfig> RemoteFacilityManager { get; }
        bool LoggingOn { get; set; }

        /// <summary>
        /// Name of method that should be called on the remote facility manager that receives the changed data
        /// </summary>
        string MethodNameOfRFM { get; }

        void StartRedirection();
        bool IsEnabledStartRedirection();
        void StopRedirection();
        bool IsEnabledStopRedirection();
    }

    public class RemoteFacilityBroadcaster
    {
        #region c'tors
        public RemoteFacilityBroadcaster(IRemoteFacilityForwarder parentComponent)
        {
            _ParentComponent = parentComponent;
        }
        #endregion

        #region Internal class
        public class RemoteInstance
        {
            public ACRef<ACComponentProxy> Recipient { get; set; }
            public RemoteStoreConfig Config { get; set; }
        }
        #endregion

        #region Properties
        private IRemoteFacilityForwarder _ParentComponent;
        private int _CountConfigReads = 0;
        private List<RemoteInstance> _RemoteInstances = new List<RemoteInstance>();
        public IEnumerable<RemoteInstance> RemoteInstances
        {
            get
            {
                using (ACMonitor.Lock((_ParentComponent as ACComponent)._20015_LockValue))
                {
                    return _RemoteInstances.ToArray();
                }
            }
        }
        #endregion

        #region Methods
        public List<core.datamodel.ACClassConfig> GetRemoteFacilityManager()
        {
            string keyACUrl = ".\\ACClassProperty(RemoteFacilityManager)";
            List<core.datamodel.ACClassConfig> result = null;
            ACClassTaskQueue.TaskQueue.ProcessAction(() =>
            {
                try
                {
                    (_ParentComponent as ACComponent).ACTypeFromLiveContext.ACClassConfig_ACClass.Load(MergeOption.OverwriteChanges);
                    var query = (_ParentComponent as ACComponent).ACTypeFromLiveContext.ACClassConfig_ACClass.Where(c => c.KeyACUrl == keyACUrl);
                    if (query.Any())
                        result = query.ToList();
                    else
                        result = new List<core.datamodel.ACClassConfig>();
                }
                catch (Exception e)
                {
                    _ParentComponent.Messages.LogException(_ParentComponent.GetACUrl(), "RemoteFacilityBroadcaster", e.Message);
                }
            });
            return result;
        }

        private void Proxy_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ACComponentProxy proxy = sender as ACComponentProxy;
            if (proxy == null)
                return;
            // Resend persisted messages after reconnection
            if (e.PropertyName == "ConnectionState" && proxy.ConnectionState >= ACObjectConnectionState.Connected)
            {
                var appManager = _ParentComponent.ApplicationManager;
                if (appManager != null && appManager.ApplicationQueue != null)
                {
                    appManager.ApplicationQueue.Add(() => { ResendData(proxy); });
                }
            }
        }

        private void ResendUnsent()
        {
            foreach (RemoteStorePostingData postingData in _ParentComponent.RemoteStorePostings.ToArray())
            {
                RemoteInstance remoteInstance = RemoteInstances.Where(c => c.Recipient.ACUrl == postingData.Recipient).FirstOrDefault();
                if (remoteInstance != null && remoteInstance.Recipient.ValueT.ConnectionState >= ACObjectConnectionState.Connected)
                {
                    if (_ParentComponent.LoggingOn)
                        _ParentComponent.Messages.LogDebug(_ParentComponent.GetACUrl(), "RemoteFacilityBroadcaster.ResendUnsent(10)", remoteInstance.Recipient.ValueT.GetACUrl());
                    ResendData(remoteInstance.Recipient.ValueT as ACComponentProxy, postingData);
                }
                else
                    break;
            }
            _ParentComponent.OnPropertyChanged(nameof(IRemoteFacilityForwarder.RemoteStorePostings)); // Persist
        }

        private void ResendData(ACComponentProxy proxy)
        {
            if (_ParentComponent.LoggingOn)
                _ParentComponent.Messages.LogDebug(_ParentComponent.GetACUrl(), "RemoteFacilityBroadcaster.ResendData(10)", proxy.GetACUrl());
            string acUrlOfProxy = proxy.GetACUrl();
            RemoteStorePostingData postingData = _ParentComponent.RemoteStorePostings.Where(c => c.Recipient == acUrlOfProxy).FirstOrDefault();
            if (postingData != null)
            {
                ResendData(proxy, postingData);
                _ParentComponent.OnPropertyChanged(nameof(IRemoteFacilityForwarder.RemoteStorePostings)); // Persist
            }
        }

        private void ResendData(ACComponentProxy proxy, RemoteStorePostingData postingData)
        {
            if (postingData == null || proxy == null)
                return;
            var appManager = _ParentComponent.ApplicationManager;
            if (appManager == null)
                return;

            string acUrlOfProxy = proxy.GetACUrl();
            _ParentComponent.RemoteStorePostings.Remove(postingData);
            if (appManager.ApplicationQueue != null)
            {
                appManager.ApplicationQueue.Add(() => { proxy.ACUrlCommand(ACUrlHelper.Delimiter_InvokeMethod + _ParentComponent.MethodNameOfRFM, _ParentComponent.GetACUrl(), postingData); });
            }
        }

        public void SendToRemoteStore(string entityType, Guid? keyID)
        {
            var appManager = _ParentComponent.ApplicationManager;
            if (appManager == null)
                return;
            if (_ParentComponent.LoggingOn)
                _ParentComponent.Messages.LogDebug(_ParentComponent.GetACUrl(), "RemoteFacilityBroadcaster.SendToRemoteStore(10)", String.Format("entityType: {0}, keyID {1}", entityType, keyID.HasValue ? keyID.Value : Guid.Empty));
            foreach (RemoteInstance remoteInstance in RemoteInstances)
            {
                RemoteStorePostingData postingData = _ParentComponent.RemoteStorePostings.Where(c => c.Recipient == remoteInstance.Recipient.ACUrl).FirstOrDefault();
                if (remoteInstance.Recipient.ValueT.ConnectionState >= ACObjectConnectionState.Connected)
                {
                    if (postingData != null)
                    {
                        _ParentComponent.RemoteStorePostings.Remove(postingData);
                        _ParentComponent.OnPropertyChanged(nameof(IRemoteFacilityForwarder.RemoteStorePostings)); // Persist
                    }
                    else
                    {
                        postingData = new RemoteStorePostingData(remoteInstance.Recipient.ACUrl, remoteInstance.Config.FacilityUrlOfRecipient);
                        if (keyID.HasValue)
                            postingData.FBIds.Add(new RSPDEntry() { EntityType = entityType, KeyId = keyID.Value });
                    }
                    if (appManager.ApplicationQueue != null)
                    {
                        if (_ParentComponent.LoggingOn)
                            _ParentComponent.Messages.LogDebug(_ParentComponent.GetACUrl(), "RemoteFacilityBroadcaster.SendToRemoteStore(20)", String.Format("entityType: {0}, keyID {1}", entityType, keyID.HasValue ? keyID.Value : Guid.Empty));
                        appManager.ApplicationQueue.Add(() => { remoteInstance.Recipient.ValueT.ACUrlCommand(ACUrlHelper.Delimiter_InvokeMethod + _ParentComponent.MethodNameOfRFM, _ParentComponent.GetACUrl(), postingData); });
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
                        _ParentComponent.RemoteStorePostings.Add(postingData);
                    }
                    if (keyID.HasValue)
                        postingData.FBIds.Add(new RSPDEntry() { EntityType = entityType, KeyId = keyID.Value });
                    if (_ParentComponent.LoggingOn)
                        _ParentComponent.Messages.LogDebug(_ParentComponent.GetACUrl(), "RemoteFacilityBroadcaster.SendToRemoteStore(30)", String.Format("entityType: {0}, keyID {1}", entityType, keyID.HasValue ? keyID.Value : Guid.Empty));
                    _ParentComponent.OnPropertyChanged(nameof(IRemoteFacilityForwarder.RemoteStorePostings)); // Persist
                }
            }
        }

        public void StartRedirection()
        {
            if (!IsEnabledStartRedirection())
                return;

            var propertyQuery = _ParentComponent.RemoteFacilityManager;
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

                ACComponentProxy proxy = _ParentComponent.ACUrlCommand(redirectConfig.Recipient) as ACComponentProxy;
                if (proxy != null)
                {
                    using (ACMonitor.Lock((_ParentComponent as ACComponent)._20015_LockValue))
                    {
                        proxy.PropertyChanged += Proxy_PropertyChanged;
                        _RemoteInstances.Add(new RemoteInstance() { Recipient = new ACRef<ACComponentProxy>(proxy, _ParentComponent), Config = redirectConfig });
                    }
                }
            }

            _ParentComponent.IsActive.ValueT = RemoteInstances.Any();
            ResendUnsent();
        }

        public bool IsEnabledStartRedirection()
        {
            return !_ParentComponent.ConnectionDisabled && !RemoteInstances.Any();
        }

        public void StopRedirection()
        {
            if (!IsEnabledStopRedirection())
                return;
            using (ACMonitor.Lock((_ParentComponent as ACComponent)._20015_LockValue))
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
            _ParentComponent.IsActive.ValueT = false;
        }

        public bool IsEnabledStopRedirection()
        {
            return RemoteInstances != null && RemoteInstances.Any();
        }
        #endregion
    }

}
