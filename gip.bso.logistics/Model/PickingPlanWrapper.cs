// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.mes.datamodel;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.bso.logistics
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Picking order Plan'}de{'Kommisionierauftrag Plan'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class PickingPlanWrapper : EntityBase, IScheduledOrder
    {
        private static ACValueItemList _PickingStateList;
        [ACPropertyInfo(9999)]
        public static ACValueItemList PickingStateList
        {
            get
            {
                if (_PickingStateList == null)
                    _PickingStateList = new ACValueListPickingStateEnum();
                return _PickingStateList;
            }
        }

        public PickingPlanWrapper(Picking picking)
        {
            Picking = picking;
        }

        private Picking _Picking;
        [ACPropertyInfo(1, "", "en{'Picking Order'}de{'Kommissionierauftrag'}")]
        public Picking Picking
        {
            get { return _Picking; }
            set { SetProperty<Picking>(ref _Picking, value); }
        }

        Material _Material;
        [ACPropertyInfo(2, "", ConstApp.Material)]
        public Material Material
        {
            get
            {
                if (_Material != null)
                    return _Material;
                if (   Picking == null
                    || !Picking.PickingPos_Picking.Any())
                {
                    return null;
                }
                _Material = Picking.PickingPos_Picking.FirstOrDefault().Material;
                return _Material;
            }
        }

        public MaterialUnit AltMaterialUnit
        {
            get
            {
                return Material != null ? Material.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0).FirstOrDefault() : null;
            }
        }

        [ACPropertyInfo(3, "", "en{'Base UOM'}de{'Basiseinheit'}")]
        public MDUnit MDUnit
        {
            get
            {
                if (Material == null)
                    return null;
                return AltMaterialUnit != null ? AltMaterialUnit.ToMDUnit : Material.BaseMDUnit;
            }
        }

        [ACPropertyInfo(4, "", "en{'Alt. Unit of Measure'}de{'Alt. Einheit'}")]
        public MDUnit AltMDUnit
        {
            get
            {
                if (Material == null)
                    return null;
                return AltMaterialUnit != null ? AltMaterialUnit.ToMDUnit : Material.BaseMDUnit;
            }
        }

        List<PickingPlanPosWrapper> _Lines;
        [ACPropertyList(10, "Lines", "en{'Lines'}de{'Positionen'}")]
        public IEnumerable<PickingPlanPosWrapper> Lines
        {
            get
            {
                if (_Lines == null)
                    BuildLines();
                return _Lines;
            }
        }

        protected virtual void BuildLines()
        {
            _Lines = new List<PickingPlanPosWrapper>();
            foreach (PickingPos pos in Picking.PickingPos_Picking.OrderBy(c => c.Sequence))
            {
                _Lines.Add(OnCreateNewPosWrapper(pos));
            }
        }


        protected virtual PickingPlanPosWrapper OnCreateNewPosWrapper(PickingPos pos)
        {
            return new PickingPlanPosWrapper(pos);
        }

        private string _ReservationInfo;
        [ACPropertyInfo(4, "", "en{'Reservation'}de{'Reservierung'}")]
        public string ReservationInfo
        {
            get
            {
                if (_ReservationInfo != null)
                    return _ReservationInfo;
                StringBuilder sb = new StringBuilder();
                foreach (PickingPos pickingPos in Picking.PickingPos_Picking.OrderBy(c => c.Sequence))
                {
                    foreach (FacilityReservation reservation in pickingPos.FacilityReservation_PickingPos
                        .Where(c => c.FacilityLot != null)
                        .OrderBy(c => c.FacilityLot.ExternLotNo))
                    {
                        sb.AppendLine(String.Format("{0} ({1}) {2}", reservation.FacilityLot.LotNo, reservation.FacilityLot.ExternLotNo, reservation.FacilityLot.Comment));
                    }
                }
                _ReservationInfo = sb.ToString();
                return _ReservationInfo;
            }
        }

        #region Exposed Properties

        [ACPropertyInfo(999)]
        public EntityState EntityState
        {
            get
            {
                return Picking.EntityState;
            }
        }

        public Guid PickingID
        {
            get => Picking.PickingID;
            set { Picking.PickingID = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(1, "PickingNo", ConstApp.PickingNo)]
        public string PickingNo
        {
            get => Picking.PickingNo;
            set { Picking.PickingNo = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(2, "PickingTypeIndex", "en{'Picking Type'}de{'Kommissioniertyp'}")]
        public GlobalApp.PickingType PickingType
        {
            get => Picking.PickingType;
        }

        [ACPropertyInfo(3, "PickingState", "en{'Picking Status'}de{'Status'}", Const.ContextDatabase + "\\PickingStateList", true)]
        public PickingStateEnum PickingState
        {
            get => Picking.PickingState;
            set { Picking.PickingState = value; OnPropertyChanged(); }
        }


        [ACPropertyInfo(3, "PickingState", "en{'Picking Status'}de{'Status'}", Const.ContextDatabase + "\\PickingStateList", true)]
        public short PickingStateIndex
        {
            get => Picking.PickingStateIndex;
            set { Picking.PickingStateIndex = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(3, "", "en{'State'}de{'Status'}")]
        public string PickingIndexName
        {
            get
            {
                ACValueItem acValueItem = PickingStateList.FirstOrDefault(c => ((short)c.Value) == PickingStateIndex);
                return acValueItem.ACCaption;
            }
        }

        [ACPropertyInfo(4, "DeliveryDateFrom", "en{'Date from'}de{'Datum von'}", "", true)]
        public DateTime DeliveryDateFrom
        {
            get => Picking.DeliveryDateFrom;
            set { Picking.DeliveryDateFrom = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(5, "DeliveryDateTo", "en{'Date to'}de{'Datum bis'}", "", true)]
        public DateTime DeliveryDateTo
        {
            get => Picking.DeliveryDateTo;
            set { Picking.DeliveryDateTo = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(6, "Comment", ConstApp.Comment, "",  true)]
        public string Comment
        {
            get => Picking.Comment;
            set { Picking.Comment = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(7, "Comment2", ConstApp.Comment2, "", true)]
        public string Comment2
        {
            get => Picking.Comment2;
            set { Picking.Comment2 = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(8, VisitorVoucher.ClassName, "en{'Visitor Voucher'}de{'Besucherbeleg'}", Const.ContextDatabase + "\\" + VisitorVoucher.ClassName, true)]
        public VisitorVoucher VisitorVoucher
        {
            get => Picking.VisitorVoucher;
            set { Picking.VisitorVoucher = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(9, gip.core.datamodel.ACClassMethod.ClassName, "en{'Workflow'}de{'Workflow'}", Const.ContextDatabase + "\\" + gip.core.datamodel.ACClassMethod.ClassName, true)]
        public mes.datamodel.ACClassMethod ACClassMethod
        {
            get => Picking.ACClassMethod;
            set { Picking.ACClassMethod = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(10, MDPickingType.ClassName, "en{'Picking type'}de{'Kommissionierung Typ'}", Const.ContextDatabase + "\\" + MDPickingType.ClassName, true)]
        public MDPickingType MDPickingType
{
            get => Picking.MDPickingType;
            set { Picking.MDPickingType = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(11, "DeliveryCompanyAddress", "en{'Delivery Address'}de{'Lieferadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName, true)]
        public CompanyAddress DeliveryCompanyAddress
{
            get => Picking.DeliveryCompanyAddress;
            set { Picking.DeliveryCompanyAddress = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(12, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", true)]
        public string KeyOfExtSys
{
            get => Picking.KeyOfExtSys;
            set { Picking.KeyOfExtSys = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(17, "ScheduledStartDate", "en{'Planned Start Date'}de{'Geplante Startzeit'}", "", true)]
        public DateTime? ScheduledStartDate
        {
            get => Picking.ScheduledStartDate;
            set { Picking.ScheduledStartDate = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(18, "ScheduledEndDate", "en{'Planned End Date'}de{'Geplante Endezeit'}", "", true)]
        public DateTime? ScheduledEndDate
        {
            get => Picking.ScheduledEndDate;
            set { Picking.ScheduledEndDate = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(19, "CalculatedStartDate", "en{'Calculated Start Date'}de{'Berechnete Startzeit'}", "", true)]
        public DateTime? CalculatedStartDate
        {
            get => Picking.CalculatedStartDate;
            set { Picking.CalculatedStartDate = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(20, "CalculatedEndDate", "en{'Calculated End Date'}de{'Berechnete Endezeit'}", "", true)]
        public DateTime? CalculatedEndDate
        {
            get => Picking.CalculatedEndDate;
            set { Picking.CalculatedEndDate = value; OnPropertyChanged(); }
        }

        [NotMapped]
        [ACPropertyInfo(10, "IsSelected", "en{'Select'}de{'Auswahl'}")]
        public bool IsSelected
        {
            get
            {
                return Picking.IsSelected;
            }
            set
            {
                Picking.IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        [ACPropertyInfo(16, "ScheduledOrder", "en{'Scheduled Order'}de{'Reihenfolge Plan'}")]
        public int? ScheduledOrder
        {
            get => Picking.ScheduledOrder;
            set { Picking.ScheduledOrder = value; OnPropertyChanged(); }
        }


        [ACPropertyInfo(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
        public DateTime InsertDate
        {
            get => Picking.InsertDate;
            set { Picking.InsertDate = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(497, Const.EntityInsertName, Const.EntityTransInsertName)]
        public string InsertName
        {
            get => Picking.InsertName;
            set { Picking.InsertName = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
        public DateTime UpdateDate
        {
            get => Picking.UpdateDate;
            set { Picking.UpdateDate = value; OnPropertyChanged(); }
        }

        [ACPropertyInfo(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
        public string UpdateName
        {
            get => Picking.UpdateName;
            set { Picking.UpdateName = value; OnPropertyChanged(); }
        }


        #endregion
    }


    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Picking line'}de{'Kommissionierposition'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class PickingPlanPosWrapper : EntityBase
    {
        public PickingPlanPosWrapper(PickingPos pickingPos)
        {
            PickingPos = pickingPos;
        }

        private PickingPos _PickingPos;
        [ACPropertyInfo(1, "", "en{'Picking line'}de{'Kommissionierposition'}")]
        public PickingPos PickingPos
        {
            get { return _PickingPos; }
            set { SetProperty<PickingPos>(ref _PickingPos, value); }
        }

        List<PickingPlanReservWrapper> _Reservations;
        [ACPropertyList(10, "Lines", "en{'Reservations'}de{'Reservierungen'}")]
        public IEnumerable<PickingPlanReservWrapper> Reservations
        {
            get
            {
                if (_Reservations == null)
                    BuildReservations();
                return _Reservations;
            }
        }

        List<PickingPlanTargetWrapper> _Targets;
        [ACPropertyList(10, "Lines", "en{'Destinations'}de{'Ziele'}")]
        public IEnumerable<PickingPlanTargetWrapper> Targets
        {
            get
            {
                if (_Targets == null)
                    BuildReservations();
                return _Targets;
            }
        }

        protected virtual void BuildReservations()
        {
            _Reservations = new List<PickingPlanReservWrapper>();
            _Targets = new List<PickingPlanTargetWrapper>();
            PickingPos.FacilityReservation_PickingPos.AutoRefresh(PickingPos.FacilityReservation_PickingPosReference, PickingPos);
            foreach (FacilityReservation reservation in PickingPos.FacilityReservation_PickingPos)
            {
                if (!reservation.VBiACClassID.HasValue && reservation.FacilityLot != null)
                    _Reservations.Add(OnCreateNewReservWrapper(reservation));
                else if (reservation.VBiACClassID.HasValue && reservation.Facility != null)
                    _Targets.Add(OnCreateNewTargetWrapper(reservation));
            }

            if (_Reservations.Any())
                _Reservations = _Reservations.OrderBy(c => c.Lot.Lot.LotNo).ToList();
            if (_Targets.Any())
                _Targets = _Targets.OrderBy(c => c.Reservation.Facility.FacilityNo).ToList();
        }

        protected virtual PickingPlanReservWrapper OnCreateNewReservWrapper(FacilityReservation reservation)
        {
            return new PickingPlanReservWrapper(reservation);
        }

        protected virtual PickingPlanTargetWrapper OnCreateNewTargetWrapper(FacilityReservation reservation)
        {
            return new PickingPlanTargetWrapper(reservation);
        }
    }


    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Reservation'}de{'Reservierung'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class PickingPlanReservWrapper : EntityBase
    {
        public PickingPlanReservWrapper(FacilityReservation reservation)
        {
            Reservation = reservation;
        }

        private FacilityReservation _Reservation;
        [ACPropertyInfo(1, "", "en{'Reservation'}de{'Reservierung'}")]
        public FacilityReservation Reservation
        {
            get { return _Reservation; }
            set { SetProperty<FacilityReservation>(ref _Reservation, value); }
        }

        private PickingPlanLotWrapper _Lot;
        [ACPropertyInfo(2, "", "en{'Lot'}de{'Los'}")]
        public PickingPlanLotWrapper Lot
        {
            get
            {
                if (_Lot == null)
                    _Lot = OnCreateLot();
                return _Lot;
            }
        }

        protected virtual PickingPlanLotWrapper OnCreateLot() 
        {
            return new PickingPlanLotWrapper(Reservation.FacilityLot);
        }
    }


    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Lot'}de{'Los'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class PickingPlanLotWrapper : EntityBase
    {
        public PickingPlanLotWrapper(FacilityLot lot)
        {
            Lot = lot;
        }

        private FacilityLot _Lot;
        [ACPropertyInfo(1, "", "en{'Lot'}de{'Los'}")]
        public FacilityLot Lot
        {
            get { return _Lot; }
            set { SetProperty<FacilityLot>(ref _Lot, value); }
        }

        [ACPropertyInfo(50, "", "en{'ExtString1'}de{'ExtString1'}")]
        public virtual string ExtString1
        {
            get
            {
                return null;
            }
        }

        [ACPropertyInfo(51, "", "en{'ExtString2'}de{'ExtString2'}")]
        public virtual string ExtString2
        {
            get
            {
                return null;
            }
        }

        [ACPropertyInfo(52, "", "en{'ExtString3'}de{'ExtString3'}")]
        public virtual string ExtString3
        {
            get
            {
                return null;
            }
        }

        [ACPropertyInfo(53, "", "en{'ExtString4'}de{'ExtString4'}")]
        public virtual string ExtString4
        {
            get
            {
                return null;
            }
        }

        [ACPropertyInfo(54, "", "en{'ExtString5'}de{'ExtString5'}")]
        public virtual string ExtString5
        {
            get
            {
                return null;
            }
        }

        [ACPropertyInfo(55, "", "en{'ExtString6'}de{'ExtString6'}")]
        public virtual string ExtString6
        {
            get
            {
                return null;
            }
        }

        [ACPropertyInfo(56, "", "en{'ExtString7'}de{'ExtString7'}")]
        public virtual string ExtString7
        {
            get
            {
                return null;
            }
        }

        [ACPropertyInfo(57, "", "en{'ExtString8'}de{'ExtString8'}")]
        public virtual string ExtString8
        {
            get
            {
                return null;
            }
        }

        [ACPropertyInfo(58, "", "en{'ExtString9'}de{'ExtString9'}")]
        public virtual string ExtString9
        {
            get
            {
                return null;
            }
        }

        [ACPropertyInfo(59, "", "en{'ExtString10'}de{'ExtString10'}")]
        public virtual string ExtString10
        {
            get
            {
                return null;
            }
        }

        [ACPropertyInfo(60, "", "en{'ExtString11'}de{'ExtString11'}")]
        public virtual string ExtString11
        {
            get
            {
                return null;
            }
        }

        [ACPropertyInfo(60, "", "en{'ExtDouble1'}de{'ExtDouble1'}")]
        public virtual double? ExtDouble1
        {
            get
            {
                return null;
            }
        }

        [ACPropertyInfo(61, "", "en{'ExtDouble2'}de{'ExtDouble2'}")]
        public virtual double? ExtDouble2
        {
            get
            {
                return null;
            }
        }

        [ACPropertyInfo(62, "", "en{'ExtDouble3'}de{'ExtDouble3'}")]
        public virtual double? ExtDouble3
        {
            get
            {
                return null;
            }
        }
    }


    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Destination'}de{'Ziele'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class PickingPlanTargetWrapper : EntityBase
    {
        public PickingPlanTargetWrapper(FacilityReservation reservation)
        {
            Reservation = reservation;
        }

        private FacilityReservation _Reservation;
        [ACPropertyInfo(1, "", "en{'Reservation'}de{'Reservierung'}")]
        public FacilityReservation Reservation
        {
            get { return _Reservation; }
            set { SetProperty<FacilityReservation>(ref _Reservation, value); }
        }
    }

}


