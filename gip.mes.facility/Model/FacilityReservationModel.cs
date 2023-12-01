using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'FacilityReservationModel'}de{'FacilityReservationModel'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class FacilityReservationModel : FacilityReservationModelBase
    {
        #region const
        public static string[] OriginalValueNames = new string[] { nameof(TotalReservedQuantity), nameof(FreeQuantity) };
        #endregion

        #region Material & Lot


        [ACPropertyInfo(1, "", ConstApp.Material)]
        public Material Material { get; set; }

        [ACPropertyInfo(2, "", ConstApp.LotNo)]
        public FacilityLot FacilityLot { get; set; }

        private string _FacilityNos;
        [ACPropertyInfo(3, "", ConstApp.FacilityNo)]
        public string FacilityNos
        {
            get
            {
                if (_FacilityNos == null && FacilityNoList != null && FacilityNoList.Any())
                {
                    _FacilityNos = string.Join(", ", FacilityNoList.OrderBy(c => c));
                }
                return _FacilityNos;
            }
            set
            {
                _FacilityNos = value;
            }
        }

        #endregion

        #region Quantities

        public double _AssignedQuantity;
        [ACPropertyInfo(4, "", ConstApp.AssignedQuantity)]
        public double AssignedQuantity
        {
            get
            {
                return _AssignedQuantity;
            }
            set
            {
                if (_AssignedQuantity != value)
                {
                    double difference = value - _AssignedQuantity;
                    FreeQuantity = FreeQuantity - difference;
                    TotalReservedQuantity = TotalReservedQuantity + difference;
                    _AssignedQuantity = value;
                    if (FacilityReservation != null)
                    {
                        if (Math.Abs((FacilityReservation.ReservedQuantityUOM ?? 0) - _AssignedQuantity) > double.Epsilon)
                        {
                            FacilityReservation.ReservedQuantityUOM = _AssignedQuantity;
                        }
                        IsRecalculated = false;
                    }
                    OnPropertyChanged(nameof(AssignedQuantity));
                }
            }
        }

        public double OriginalReservedQuantity { get; set; }

        public List<string> FacilityNoList { get; set; }

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
