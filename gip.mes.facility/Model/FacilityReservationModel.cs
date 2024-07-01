using gip.core.datamodel;
using gip.core.processapplication;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using static gip.mes.datamodel.GlobalApp;

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
        public string DocumentNo { get; set; }


        [ACPropertyInfo(2, "", ConstApp.Material)]
        public Material Material { get; set; }

        [ACPropertyInfo(3, "", ConstApp.LotNo)]
        public FacilityLot FacilityLot { get; set; }

        private string _FacilityNos;
        [ACPropertyInfo(4, "", ConstApp.FacilityNo)]
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

        #region Navigation properties
        public Guid ProdOrderPartslistID { get; set; }
        public Guid PickingID { get; set; }
        #endregion

        #region Quantities

        public double _AssignedQuantity;
        [ACPropertyInfo(5, "", ConstApp.AssignedQuantity)]
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
                    if (!IsOnlyStockMovement)
                    {
                        FreeQuantity = FreeQuantity - difference;
                    }
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

        #region Additional members

        public FacilityReservation FacilityReservation { get; set; }

        [ACPropertyInfo(6, "", "en{'Oldest charge date'}de{'Ältestes Quant-Datum'}")]
        public DateTime? OldestFacilityChargeDate { get; set; }


        private bool _IsObserveQuantity;
        [ACPropertyInfo(7, "", "en{'Observe quantity'}de{'Menge beachten'}")]
        public bool IsObserveQuantity
        {
            get
            {
                if(FacilityReservation != null)
                {
                    if(FacilityReservation.ReservationStateIndex == (short)ReservationState.ObserveQuantity)
                    {
                        _IsObserveQuantity = true;
                    }
                    else
                    {
                        _IsObserveQuantity = false;
                    }
                }
                return _IsObserveQuantity;
            }
            set
            {
                _IsObserveQuantity = value;
                if (FacilityReservation != null)
                {
                    if (value)
                    {
                        FacilityReservation.ReservationStateIndex = (short)ReservationState.ObserveQuantity;
                    }
                    else
                    {
                        FacilityReservation.ReservationStateIndex = (short)ReservationState.New;
                    }
                }
            }
        }

        #endregion

        #region Methods

        public void CopyFrom(FacilityReservationModelBase model)
        {
            TotalReservedQuantity = model.TotalReservedQuantity;
            UsedQuantity = model.UsedQuantity;
            FreeQuantity = model.FreeQuantity;
            IsRecalculated = true;
            IsOnlyStockMovement = model.IsOnlyStockMovement;
        }

        #endregion
    }
}
