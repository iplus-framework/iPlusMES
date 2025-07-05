// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityReservationModelBase'}de{'FacilityReservationModelBase'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class FacilityReservationModelBase : INotifyPropertyChanged
    {

        #region Properties


        [NotMapped]
        protected bool _IsSelected;
        [ACPropertyInfo(1, "IsSelected", Const.Select)]
        [NotMapped]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        private double _TotalReservedQuantity;
        [ACPropertyInfo(2, "", ConstApp.TotalReservedQuantity)]
        public double TotalReservedQuantity
        {
            get
            {
                return _TotalReservedQuantity;
            }
            set
            {
                if (_TotalReservedQuantity != value)
                {
                    _TotalReservedQuantity = value;
                    OnPropertyChanged(nameof(TotalReservedQuantity));
                }
            }
        }

        private double _UsedQuantity;
        [ACPropertyInfo(3, "", "en{'Used quantity'}de{'Used quantity'}")]
        public double UsedQuantity
        {
            get
            {
                return _UsedQuantity;
            }
            set
            {
                if (_UsedQuantity != value)
                {
                    _UsedQuantity = value;
                    OnPropertyChanged(nameof(UsedQuantity));
                }
            }
        }

        private double _FreeQuantity;
        [ACPropertyInfo(4, "", ConstApp.FreeQuantity)]
        public double FreeQuantity
        {
            get
            {
                return _FreeQuantity;
            }
            set
            {
                if (_FreeQuantity != value)
                {
                    _FreeQuantity = value;
                    OnPropertyChanged(nameof(FreeQuantity));
                    OnPropertyChanged(nameof(FreeQuantityNegative));
                }
            }
        }

        #endregion

        #region Additional properties

        public Dictionary<string, double> OriginalValues { get; set; }

        public bool FreeQuantityNegative
        {
            get
            {
                return FreeQuantity < 0;
            }
        }

        public bool IsOnlyStockMovement { get; set; }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Methods
        public void SetSelected(bool value)
        {
            _IsSelected = value;
        }

        #endregion
    }
}
