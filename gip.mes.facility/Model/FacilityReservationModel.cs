using gip.core.datamodel;
using gip.mes.datamodel;
using System;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityReservationModel'}de{'FacilityReservationModel'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class FacilityReservationModel : FacilityReservationModelBase
    {
        #region Material & Lot


        [ACPropertyInfo(1, "", ConstApp.Material)]
        public Material Material { get; set; }

        [ACPropertyInfo(2, "", ConstApp.LotNo)]
        public FacilityLot FacilityLot { get; set; }

        #endregion

        #region Quantities

        private double _ReservedQuantity;
        [ACPropertyInfo(4, "", ConstApp.ReservedQuantity)]
        public double ReservedQuantity
        {
            get
            {
                return _ReservedQuantity;
            }
            set
            {
                if (_ReservedQuantity != value)
                {
                    _ReservedQuantity = value;
                    if (FacilityReservation != null)
                    {
                        if (Math.Abs((FacilityReservation.ReservedQuantityUOM ?? 0) - _ReservedQuantity) > double.Epsilon)
                        {
                            FacilityReservation.ReservedQuantityUOM = _ReservedQuantity;
                        }
                        IsRecalculated = false;
                    }
                    OnPropertyChanged(nameof(ReservedQuantity));
                }
            }
        }

        public double OriginalReservedQuantity { get; set; }

        private bool _IsRecalculated;
        public bool IsRecalculated
        {
            get
            {
                return _IsRecalculated;
            }
            set
            {
                if (_IsRecalculated != value)
                {
                    _IsRecalculated = value;
                    OnPropertyChanged(nameof(IsRecalculated));
                }
            }
        }

        #endregion

        public void CopyFrom(FacilityReservationModelBase model)
        {
            TotalReservedQuantity = model.TotalReservedQuantity;
            UsedQuantity = model.UsedQuantity;
            FreeQuantity = model.FreeQuantity;
            IsRecalculated = true;
        }

        public FacilityReservation FacilityReservation { get; set; }

        [ACPropertyInfo(5, "", "en{'Oldest charge date'}de{'Ältestes Quant-Datum'}")]
        public DateTime? OldestFacilityChargeDate { get; set; }

    }
}
