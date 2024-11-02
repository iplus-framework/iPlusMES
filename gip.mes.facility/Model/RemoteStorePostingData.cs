// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace gip.mes.facility
{
    [DataContract]
    [ACSerializeableInfo]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Remote store posting entry'}de{'Remote store posting entry'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class RSPDEntry
    {
        [DataMember(Name = "ET")]
        private string _EntityType;
        [IgnoreDataMember]
        [ACPropertyInfo(100, "", "en{'EntityType'}de{'EntityType'}")]
        public string EntityType
        {
            get
            {
                return _EntityType;
            }
            set
            {
                _EntityType = value;
            }
        }

        [DataMember(Name = "ID")]
        private Guid _KeyId;
        [IgnoreDataMember]
        [ACPropertyInfo(101, "", "en{'Key'}de{'Schlüssel'}")]
        public Guid KeyId
        {
            get
            {
                return _KeyId;
            }
            set
            {
                _KeyId = value;
            }
        }
    }

    [DataContract]
    [ACSerializeableInfo(new Type[] { typeof(RemoteStorePostingData), typeof(SafeList<RemoteStorePostingData>) })]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'RemoteStorePostingData'}de{'RemoteStorePostingData'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class RemoteStorePostingData : RemoteStoreConfig
    {
        public RemoteStorePostingData() : base()
        {
            _FBIds = new SafeList<RSPDEntry>();
        }

        public RemoteStorePostingData(string recipient, string facilityUrlOfRecipient) : base(recipient, facilityUrlOfRecipient)
        {
            _FBIds = new SafeList<RSPDEntry>();
        }

        [DataMember]
        private SafeList<RSPDEntry> _FBIds;
        [IgnoreDataMember]
        [ACPropertyInfo(102, "", "en{'Postings'}de{'Postings'}")]
        public SafeList<RSPDEntry> FBIds
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
