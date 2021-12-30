using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace gip.mes.facility
{
    [DataContract]
    [ACSerializeableInfo(new Type[] { typeof(RemoteStorePostingData), typeof(SafeList<RemoteStorePostingData>) })]
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'RemoteStorePostingData'}de{'RemoteStorePostingData'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class RemoteStorePostingData : RemoteStoreConfig
    {
        public RemoteStorePostingData() : base()
        {
            _FBIds = new SafeList<Guid>();
        }

        public RemoteStorePostingData(string recipient, string facilityUrlOfRecipient) : base(recipient, facilityUrlOfRecipient)
        {
            _FBIds = new SafeList<Guid>();
        }

        [DataMember]
        private SafeList<Guid> _FBIds;
        [IgnoreDataMember]
        [ACPropertyInfo(102, "", "en{'Facility-BookingIDs'}de{'Facility-BookingIDs'}")]
        public SafeList<Guid> FBIds
        {
            get
            {
                return _FBIds;
            }
            set
            {
                _FBIds = value;
                OnPropertyChanged("FBIds");
            }
        }
    }

}
