// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace gip.mes.webservices
{
    [DataContract(Name = "cP")]
    public class Picking : EntityBase
    {
        [DataMember(Name = "ID")]
        public Guid PickingID
        {
            get; set;
        }

        [DataMember(Name = "PNo")]
        public string PickingNo
        {
            get;set;
        }

        [DataMember(Name = "DDF")]
        public DateTime DeliveryDateFrom
        {
            get; set;
        }

        [DataMember(Name = "PT")]
        public MDPickingType PickingType
        {
            get;
            set;
        }

        [DataMember(Name = "DCA")]
        public CompanyAddress DeliveryCompanyAddress
        {
            get;
            set;
        }

        [DataMember(Name = "COM")]
        public string Comment
        {
            get;
            set;
        }

        [DataMember(Name = "GI")]
        public string GroupItem
        {
            get;
            set;
        }

        [IgnoreDataMember]
        IEnumerable<PickingPos> _PickingPos_Picking;
        [DataMember(Name = "ixPos")]
        public IEnumerable<PickingPos> PickingPos_Picking
        {
            get
            {
                return _PickingPos_Picking;
            }
            set
            {
                SetProperty(ref _PickingPos_Picking, value);
            }
        }

        [IgnoreDataMember]
        public ObservableCollection<PickingPos> PickingPosObservable
        {
            get
            {
                if (PickingPos_Picking == null)
                    PickingPos_Picking = new ObservableCollection<PickingPos>();
                else if (!(PickingPos_Picking is ObservableCollection<PickingPos>))
                    PickingPos_Picking = new ObservableCollection<PickingPos>(PickingPos_Picking);
                return PickingPos_Picking as ObservableCollection<PickingPos>;
            }
            set
            {
                SetProperty(ref _PickingPos_Picking, value);
                OnPropertyChanged("PickingPosObservable");
            }
        }

        public void ReplacePickingPosItem(PickingPos item)
        {
            if (item == null)
                return;
            var oldPos = PickingPosObservable.Where(c => c.PickingPosID == item.PickingPosID).FirstOrDefault();
            int index = PickingPosObservable.IndexOf(oldPos);
            if (oldPos != null)
                PickingPosObservable.Remove(oldPos);

            if (index >= 0 && index <= PickingPosObservable.Count)
            {
                PickingPosObservable.Insert(index, item);
            }
            else
            {
                PickingPosObservable.Add(item);
            }
        }

        public void RefreshPickingPosInView()
        {
            OnPropertyChanged(nameof(PickingPos_Picking));
            OnPropertyChanged(nameof(PickingPosObservable));
        }
    }
}
