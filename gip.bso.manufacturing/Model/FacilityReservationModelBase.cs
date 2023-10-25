

using gip.core.datamodel;
using gip.mes.datamodel;
using System.ComponentModel;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityReservationModelBase'}de{'FacilityReservationModelBase'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class FacilityReservationModelBase : INotifyPropertyChanged
    {
        private double _TotalReservedQuantity;
        [ACPropertyInfo(1, "", ConstApp.BlockedQuantity)]
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
        [ACPropertyInfo(2, "", "en{'Used quantity'}de{'Used quantity'}")]
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
        [ACPropertyInfo(3, "", ConstApp.FreeQuantity)]
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

        public bool FreeQuantityNegative
        {
            get
            {
                return FreeQuantity < 0;
            }
        }

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
    }
}
