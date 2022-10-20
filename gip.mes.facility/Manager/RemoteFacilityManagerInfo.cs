using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public class RemoteFacilityManagerInfo : INotifyPropertyChanged
    {
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

        public RemoteFacilityManager RemoteFacilityManager { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
