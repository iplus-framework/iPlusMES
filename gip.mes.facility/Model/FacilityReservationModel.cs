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

        [ACPropertyInfo(8, "", "en{'Reservation'}de{'Reservierung'}")]
        public FacilityReservation FacilityReservation { get; set; }

        [ACPropertyInfo(6, "", "en{'Oldest charge date'}de{'Ältestes Quant-Datum'}")]
        public DateTime? OldestFacilityChargeDate { get; set; }


        #region Additional members -> ReservationState

        public ReservationState ReservationState
        {
            get
            {
                ReservationState reservationState = ReservationState.New;
                if(SelectedReservationState != null)
                {
                    reservationState = (ReservationState)SelectedReservationState.Value;
                }
                return reservationState;
            }
        }

        ACValueItem _SelectedReservationState;
        [ACPropertySelected(9999, nameof(ReservationState), ConstApp.FacilityReservation)]
        public ACValueItem SelectedReservationState
        {
            get
            {
                return _SelectedReservationState;
            }
            set
            {
                if (_SelectedReservationState != value)
                {
                    _SelectedReservationState = value;
                    OnPropertyChanged(nameof(SelectedReservationState));
                }
            }
        }

        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        [ACPropertyList(9999, nameof(ReservationState))]
        public IEnumerable<ACValueItem> ReservationStateList
        {
            get
            {
                return GlobalApp.ReservationStateList;
            }
        }

        #endregion

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
