using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace gip.mes.facility
{
    public class RemoteFacilityManagerInfo : INotifyPropertyChanged
    {

        #region ctor's
        
        public RemoteFacilityManagerInfo(ACComponent remoteFacilityManager)
        {
            ACUrl = remoteFacilityManager.ACUrl;
            RemoteConnString = GetRemoteFacilityManagerConnectionString(remoteFacilityManager);
            RemoteFacilityManager = remoteFacilityManager;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _ACUrl;
        [ACPropertyInfo(999, "ACUrl", "en{'TODO:ACUrl'}de{'TODO:ACUrl'}")]
        public string ACUrl
        {
            get
            {
                return _ACUrl;
            }
            set
            {
                if (_ACUrl != value)
                {
                    _ACUrl = value;
                    OnPropertyChanged(nameof(ACUrl));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _RemoteConnString;
        [ACPropertyInfo(999, "RemoteConnString", "en{'Remote Connection String'}de{'Remote Connection string'}")]
        public string RemoteConnString
        {
            get
            {
                return _RemoteConnString;
            }
            set
            {
                if (_RemoteConnString != value)
                {
                    _RemoteConnString = value;
                    OnPropertyChanged(nameof(RemoteConnString));
                }
            }
        }


        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ACComponent RemoteFacilityManager { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        private string GetRemoteFacilityManagerConnectionString(ACComponent remoteFacilityManager)
        {
            List<string> hierarchy = ACUrlHelper.ResolveParents(remoteFacilityManager.ACUrl);
            string remoteProxyACurl = hierarchy.FirstOrDefault();
            if (String.IsNullOrEmpty(remoteProxyACurl))
                return null;
            ACComponent remoteAppManager = remoteFacilityManager.Root.ACUrlCommand(remoteProxyACurl) as ACComponent;
            if (remoteAppManager == null)
                return null;
            return remoteAppManager[nameof(RemoteAppManager.RemoteConnString)] as string;
        }

        #endregion
    }
}
