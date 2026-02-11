// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'BSOMaterialDetails'}de{'BSOMaterialDetails'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOMaterialDetails : ACBSOvb
    {
        #region ctor's

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOCompany"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOMaterialDetails(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseACInit = base.ACInit(startChildMode);

            FBSearchTo = DateTime.Now;
            FBSearchFrom = FBSearchTo.AddDays(-1);

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            return baseACInit;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            bool baseDeInit = await base.ACDeInit(deleteACClassTask);

            if (_ACFacilityManager != null)
            {
                FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            }

            _ACFacilityManager = null;

            CleanMaterialRelatedLists();

            return baseDeInit;
        }

        #endregion

        #region ChildBSO

        ACChildItem<BSOFacilityReservationOverview> _BSOFacilityReservationOverview_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo(nameof(BSOFacilityReservationOverview_Child), typeof(BSOFacilityReservationOverview))]
        public ACChildItem<BSOFacilityReservationOverview> BSOFacilityReservationOverview_Child
        {
            get
            {
                if (_BSOFacilityReservationOverview_Child == null)
                    _BSOFacilityReservationOverview_Child = new ACChildItem<BSOFacilityReservationOverview>(this, nameof(BSOFacilityReservationOverview_Child));
                return _BSOFacilityReservationOverview_Child;
            }
        }

        #endregion

        #region Managers

        protected ACRef<ACComponent> _ACFacilityManager = null;
        public FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }

        #endregion

        #region Properties
        public string[] FilterLotNos { get; set; }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private Guid? _CurrentMaterialID;
        public Guid? CurrentMaterialID
        {
            get
            {
                return _CurrentMaterialID;
            }
            set
            {
                if (_CurrentMaterialID != value)
                {
                    _CurrentMaterialID = value;
                    CleanMovements();
                    ClearReservation();
                    OnPropertyChanged();
                    LoadMaterialRelatedLists(value);
                }
            }
        }


        /// <summary>
        /// Source Property: 
        /// </summary>
        private MaterialStock _CurrentMaterialStock;
        [ACPropertyInfo(999, nameof(CurrentMaterialStock), "en{'TODO:CurrentMaterialStock'}de{'TODO:CurrentMaterialStock'}")]
        public MaterialStock CurrentMaterialStock
        {
            get
            {
                return _CurrentMaterialStock;
            }
            set
            {
                if (_CurrentMaterialStock != value)
                {
                    _CurrentMaterialStock = value;
                    OnPropertyChanged();
                }
            }
        }

        protected bool? _showNotAvailable = false;
        [ACPropertyInfo(709, "Filter", "en{'Show not available'}de{'Nicht verfügbare anzeigen'}")]
        public virtual bool? ShowNotAvailable
        {
            get
            {
                return _showNotAvailable;
            }
            set
            {
                if (_showNotAvailable != value)
                {
                    _showNotAvailable = value;
                    OnPropertyChanged();
                    LoadMaterialRelatedLists(CurrentMaterialID);
                }
            }
        }

        #region Properties -> FilterBooking

        DateTime _FBSearchFrom;
        [ACPropertyInfo(705, "", "en{'Search from'}de{'Suche von'}")]
        public DateTime FBSearchFrom
        {
            get
            {
                return _FBSearchFrom;
            }
            set
            {
                _FBSearchFrom = value;
                OnPropertyChanged();
            }
        }

        DateTime _FBSearchTo;
        [ACPropertyInfo(706, "", "en{'Search to'}de{'Suche bis'}")]
        public DateTime FBSearchTo
        {
            get
            {
                return _FBSearchTo;
            }
            set
            {
                _FBSearchTo = value;
                OnPropertyChanged();
            }
        }

        ACValueItem _FilterFBType;
        [ACPropertyInfo(707, "", "en{'Filter Posting Type'}de{'Filter Buchungsart'}")]
        public ACValueItem FilterFBType
        {
            get
            {
                return _FilterFBType;
            }
            set
            {
                _FilterFBType = value;
                OnPropertyChanged();
            }
        }

        [ACPropertyInfo(708, "", "en{'Grouped by Material'}de{'Gruppiert nach Material'}")]
        public bool FBGroupByMaterial
        {
            get;
            set;
        }

        #endregion

        #region Properties -> FacilityBookingOverview

        private FacilityBookingOverview _SelectedFacilityBookingOverview;

        [ACPropertySelected(701, "FacilityBookingOverview")]

        public FacilityBookingOverview SelectedFacilityBookingOverview
        {
            get
            {
                return _SelectedFacilityBookingOverview;
            }
            set
            {
                if (_SelectedFacilityBookingOverview != value)
                {
                    _SelectedFacilityBookingOverview = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<FacilityBookingOverview> _FacilityBookingOverviewList;
        /// <summary>
        /// Gets the facility booking list.
        /// </summary>
        /// <value>The facility booking list.</value>
        [ACPropertyList(702, "FacilityBookingOverview")]
        public virtual List<FacilityBookingOverview> FacilityBookingOverviewList
        {
            get
            {
                return _FacilityBookingOverviewList;
            }
        }


        #endregion

        #region  Properties -> FacilityBookingChargeOverview

        private FacilityBookingChargeOverview _SelectedFacilityBookingChargeOverview;

        [ACPropertySelected(703, "FacilityBookingChargeOverview")]
        public FacilityBookingChargeOverview SelectedFacilityBookingChargeOverview
        {
            get
            {
                return _SelectedFacilityBookingChargeOverview;
            }
            set
            {
                if (_SelectedFacilityBookingChargeOverview != value)
                {
                    _SelectedFacilityBookingChargeOverview = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<FacilityBookingChargeOverview> _FacilityBookingChargeOverviewList;
        /// <summary>
        /// Gets the facility booking charge list.
        /// </summary>
        /// <value>The facility booking charge list.</value>
        [ACPropertyList(704, "FacilityBookingChargeOverview")]
        public virtual List<FacilityBookingChargeOverview> FacilityBookingChargeOverviewList
        {
            get
            {
                return _FacilityBookingChargeOverviewList;
            }
        }

        #endregion

        #region Properties -> FacilityCharge

        /// <summary>
        /// The _ current facility charge
        /// </summary>
        FacilityCharge _CurrentFacilityCharge;
        /// <summary>
        /// Gets or sets the current facility charge.
        /// </summary>
        /// <value>The current facility charge.</value>
        [ACPropertyCurrent(809, FacilityCharge.ClassName)]
        public FacilityCharge CurrentFacilityCharge
        {
            get
            {
                return _CurrentFacilityCharge;
            }
            set
            {
                _CurrentFacilityCharge = value;
                OnPropertyChanged(nameof(CurrentFacilityCharge));
            }
        }

        IEnumerable<FacilityCharge> _FacilityChargeList;
        /// <summary>
        /// Gets the facility charge list.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(810, FacilityCharge.ClassName)]
        public IEnumerable<FacilityCharge> FacilityChargeList
        {
            get
            {
                return _FacilityChargeList;
            }
        }

        /// <summary>
        /// The _ selected facility charge
        /// </summary>
        FacilityCharge _SelectedFacilityCharge;
        /// <summary>
        /// Gets or sets the selected facility charge.
        /// </summary>
        /// <value>The selected facility charge.</value>
        [ACPropertySelected(811, FacilityCharge.ClassName)]
        public FacilityCharge SelectedFacilityCharge
        {
            get
            {
                return _SelectedFacilityCharge;
            }
            set
            {
                _SelectedFacilityCharge = value;
                OnPropertyChanged(nameof(SelectedFacilityCharge));
            }
        }

        #endregion

        #region Properties -> SumLocation

        /// <summary>
        /// SumLocation: A sum over all Charges/Batches of the actual Material grouped by Storage Location
        /// </summary>
        FacilityChargeSumLocationHelper _CurrentFacilityChargeSumLocationHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum location helper.
        /// </summary>
        /// <value>The current facility charge sum location helper.</value>
        [ACPropertyCurrent(812, "FacilityChargeSumLocationHelper")]
        public FacilityChargeSumLocationHelper CurrentFacilityChargeSumLocationHelper
        {
            get
            {
                return _CurrentFacilityChargeSumLocationHelper;
            }
            set
            {
                _CurrentFacilityChargeSumLocationHelper = value;
                OnPropertyChanged(nameof(CurrentFacilityChargeSumLocationHelper));
            }
        }

        private List<FacilityChargeSumLocationHelper> _FacilityChargeSumLocationHelperList;
        /// <summary>
        /// Gets the facility charge sum location helper list.
        /// </summary>
        /// <value>The facility charge sum location helper list.</value>
        [ACPropertyList(813, "FacilityChargeSumLocationHelper")]
        public List<FacilityChargeSumLocationHelper> FacilityChargeSumLocationHelperList
        {
            get
            {
                return _FacilityChargeSumLocationHelperList;
            }
        }
        /// <summary>
        /// The _ selected facility charge sum location helper
        /// </summary>
        FacilityChargeSumLocationHelper _SelectedFacilityChargeSumLocationHelper;
        /// <summary>
        /// Gets or sets the selected facility charge sum location helper.
        /// </summary>
        /// <value>The selected facility charge sum location helper.</value>
        [ACPropertySelected(814, "FacilityChargeSumLocationHelper")]
        public FacilityChargeSumLocationHelper SelectedFacilityChargeSumLocationHelper
        {
            get
            {
                return _SelectedFacilityChargeSumLocationHelper;
            }
            set
            {
                _SelectedFacilityChargeSumLocationHelper = value;
                OnPropertyChanged(nameof(SelectedFacilityChargeSumLocationHelper));
            }
        }

        #endregion

        #region Properties -> SumFacility
        /// <summary>
        /// SumLocation: A sum over all Charges/Batches of the actual Material grouped by Storage Area
        /// </summary>
        FacilityChargeSumFacilityHelper _CurrentFacilityChargeSumFacilityHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum facility helper.
        /// </summary>
        /// <value>The current facility charge sum facility helper.</value>
        [ACPropertyCurrent(815, "FacilityChargeSumFacilityHelper")]
        public FacilityChargeSumFacilityHelper CurrentFacilityChargeSumFacilityHelper
        {
            get
            {
                return _CurrentFacilityChargeSumFacilityHelper;
            }
            set
            {
                _CurrentFacilityChargeSumFacilityHelper = value;
                OnPropertyChanged(nameof(CurrentFacilityChargeSumFacilityHelper));
            }
        }

        private List<FacilityChargeSumFacilityHelper> _FacilityChargeSumFacilityHelperList;
        /// <summary>
        /// Gets the facility charge sum facility helper list.
        /// </summary>
        /// <value>The facility charge sum facility helper list.</value>
        [ACPropertyList(816, "FacilityChargeSumFacilityHelper")]
        public List<FacilityChargeSumFacilityHelper> FacilityChargeSumFacilityHelperList
        {
            get
            {
                return _FacilityChargeSumFacilityHelperList;
            }
        }

        /// <summary>
        /// The _ selected facility charge sum facility helper
        /// </summary>
        FacilityChargeSumFacilityHelper _SelectedFacilityChargeSumFacilityHelper;
        /// <summary>
        /// Gets or sets the selected facility charge sum facility helper.
        /// </summary>
        /// <value>The selected facility charge sum facility helper.</value>
        [ACPropertySelected(817, "FacilityChargeSumFacilityHelper")]
        public FacilityChargeSumFacilityHelper SelectedFacilityChargeSumFacilityHelper
        {
            get
            {
                return _SelectedFacilityChargeSumFacilityHelper;
            }
            set
            {
                _SelectedFacilityChargeSumFacilityHelper = value;
                OnPropertyChanged(nameof(SelectedFacilityChargeSumFacilityHelper));
            }
        }

        #endregion

        #region Properties -> FacilityLot

        /// <summary>
        /// The _ current facility charge sum lot helper
        /// </summary>
        FacilityChargeSumLotHelper _CurrentFacilityChargeSumLotHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum lot helper.
        /// </summary>
        /// <value>The current facility charge sum lot helper.</value>
        [ACPropertyCurrent(806, "FacilityChargeSumLotHelper")]
        public FacilityChargeSumLotHelper CurrentFacilityChargeSumLotHelper
        {
            get
            {
                return _CurrentFacilityChargeSumLotHelper;
            }
            set
            {
                _CurrentFacilityChargeSumLotHelper = value;
                OnPropertyChanged(nameof(CurrentFacilityChargeSumLotHelper));
            }
        }

        private List<FacilityChargeSumLotHelper> _FacilityChargeSumLotHelperList;
        /// <summary>
        /// Gets the facility charge sum lot helper list.
        /// </summary>
        /// <value>The facility charge sum lot helper list.</value>
        [ACPropertyList(807, "FacilityChargeSumLotHelper")]
        public List<FacilityChargeSumLotHelper> FacilityChargeSumLotHelperList
        {
            get
            {
                return _FacilityChargeSumLotHelperList;
            }
        }
        /// <summary>
        /// The _ selected facility charge sum lot helper
        /// </summary>
        FacilityChargeSumLotHelper _SelectedFacilityChargeSumLotHelper;
        /// <summary>
        /// Gets or sets the selected facility charge sum lot helper.
        /// </summary>
        /// <value>The selected facility charge sum lot helper.</value>
        [ACPropertySelected(808, "FacilityChargeSumLotHelper")]
        public FacilityChargeSumLotHelper SelectedFacilityChargeSumLotHelper
        {
            get
            {
                return _SelectedFacilityChargeSumLotHelper;
            }
            set
            {
                _SelectedFacilityChargeSumLotHelper = value;
                OnPropertyChanged(nameof(SelectedFacilityChargeSumLotHelper));
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Methods -> ACMethod

        [ACMethodCommand(FacilityBooking.ClassName, "en{'Refresh'}de{'Aktualisiere'}", 701)]
        public virtual async Task RefreshMovements()
        {
            if (!IsEnabledRefreshMovements())
                return;
            if (BackgroundWorker.IsBusy)
                return;
            _FacilityBookingOverviewList = null;
            _FacilityBookingChargeOverviewList = null;
            BackgroundWorker.RunWorkerAsync("DoFacilityBookingSearch");
            await ShowDialogAsync(this, DesignNameProgressBar);
        }

        public virtual bool IsEnabledRefreshMovements()
        {
            return
                !BackgroundWorker.IsBusy
                && CurrentMaterialID != null;
        }

        /// <summary>
        /// Source Property: LoadFacilityReservation
        /// </summary>
        [ACMethodInfo(nameof(LoadFacilityReservation), "en{'Show Facility Reservation'}de{'Reservierungen anzeigen'}", 999)]
        public void LoadFacilityReservation()
        {
            if (!IsEnabledLoadFacilityReservation())
                return;
            if (BSOFacilityReservationOverview_Child != null && BSOFacilityReservationOverview_Child.Value != null)
            {
                BSOFacilityReservationOverview_Child.Value.LoadReservation(CurrentMaterialStock.Material, FBSearchFrom, FBSearchTo);
            }
        }

        public bool IsEnabledLoadFacilityReservation()
        {
            return CurrentMaterialStock != null;
        }

        [ACMethodInteraction("", "en{'Manage Material'}de{'Verwalte Material'}", 785, true, nameof(SelectedFacilityCharge))]
        public void NavigateToMaterial()
        {
            if (!IsEnabledNavigateToMaterial())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(Material), SelectedFacilityCharge.MaterialID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToMaterial()
        {
            return SelectedFacilityCharge != null;
        }

        [ACMethodInteraction("", ConstApp.ShowProdOrder, 780, true, nameof(SelectedFacilityCharge))]
        public void NavigateToOrder()
        {
            if (!IsEnabledNavigateToOrder())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(ProdOrderPartslistPos), SelectedFacilityCharge.FinalPositionFromFbc.ProdOrderPartslistPosID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToOrder()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.FinalPositionFromFbc != null)
                return true;
            return false;
        }

        [ACMethodInteraction(nameof(NavigateToFacilityCharge), "en{'Manage Quant'}de{'Quant verwalten'}", 781, true, nameof(SelectedFacilityCharge))]
        public void NavigateToFacilityCharge()
        {
            if (!IsEnabledNavigateToFacilityCharge())
            {
                return;
            }

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityCharge), SelectedFacilityCharge.FacilityChargeID));
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacilityCharge.FacilityID));
                info.Entities.Add(new PAOrderInfoEntry(nameof(Material), SelectedFacilityCharge.MaterialID));

                if (SelectedFacilityCharge.FacilityLotID != null)
                {
                    info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityCharge.FacilityLotID ?? Guid.Empty));
                }

                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityCharge()
        {
            return SelectedFacilityCharge != null;
        }

        [ACMethodInteraction("", "en{'Manage Lot/Batch'}de{'Verwalte Los/Charge'}", 781, true, nameof(SelectedFacilityCharge))]
        public void NavigateToFacilityLot()
        {
            if (!IsEnabledNavigateToFacilityLot())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityCharge.FacilityLotID ?? Guid.Empty));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityLot()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.FacilityLot != null)
                return true;
            return false;
        }

        [ACMethodInteraction("", "en{'Show Lot Stock and History'}de{'Zeige Losbestand und Historie'}", 782, true, nameof(SelectedFacilityCharge))]
        public void NavigateToFacilityLotOverview()
        {
            if (!IsEnabledNavigateToFacilityLotOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityCharge.FacilityLotID ?? Guid.Empty));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityLotOverview()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.FacilityLot != null)
                return true;
            return false;
        }

        [ACMethodInteraction("", "en{'Show Bin Stock and History'}de{'Zeige Behälterbestand und Historie'}", 783, true, nameof(SelectedFacilityCharge))]
        public void NavigateToFacilityOverview()
        {
            if (!IsEnabledNavigateToFacilityOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacilityCharge.FacilityID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityOverview()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Facility != null && SelectedFacilityCharge.Facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                return true;
            return false;
        }

        [ACMethodInteraction("", "en{'Manage Stock of Bin'}de{'Verwalte Behälterbestand'}", 784, true, nameof(SelectedFacilityCharge))]
        public void NavigateToFacility()
        {
            if (!IsEnabledNavigateToFacility())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacilityCharge.FacilityID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacility()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Facility != null && SelectedFacilityCharge.Facility.MDFacilityType != null && SelectedFacilityCharge.Facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                return true;
            return false;
        }

        [ACMethodInfo(nameof(OnTrackingCall), "en{'OnTrackingCall'}de{'OnTrackingCall'}", 702, false)]
        public void OnTrackingCall(GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter, TrackingEnginesEnum engine)
        {
            TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
            trackingCommonStart.DoTracking(this, direction, itemForTrack, additionalFilter, engine);
        }

        #endregion


        #region Methods -> Overrides

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(RefreshMovements):
                    RefreshMovements();
                    return true;
                case nameof(IsEnabledRefreshMovements):
                    result = IsEnabledRefreshMovements();
                    return true;
                case nameof(LoadFacilityReservation):
                    LoadFacilityReservation();
                    return true;
                case nameof(IsEnabledLoadFacilityReservation):
                    result = IsEnabledLoadFacilityReservation();
                    return true;
                case nameof(NavigateToMaterial):
                    NavigateToMaterial();
                    return true;
                case nameof(IsEnabledNavigateToMaterial):
                    result = IsEnabledNavigateToMaterial();
                    return true;
                case nameof(NavigateToOrder):
                    NavigateToOrder();
                    return true;
                case nameof(IsEnabledNavigateToOrder):
                    result = IsEnabledNavigateToOrder();
                    return true;
                case nameof(NavigateToFacilityCharge):
                    NavigateToFacilityCharge();
                    return true;
                case nameof(IsEnabledNavigateToFacilityCharge):
                    result = IsEnabledNavigateToFacilityCharge();
                    return true;
                case nameof(NavigateToFacilityLot):
                    NavigateToFacilityLot();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLot):
                    result = IsEnabledNavigateToFacilityLot();
                    return true;
                case nameof(NavigateToFacilityLotOverview):
                    NavigateToFacilityLotOverview();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLotOverview):
                    result = IsEnabledNavigateToFacilityLotOverview();
                    return true;
                case nameof(NavigateToFacilityOverview):
                    NavigateToFacilityOverview();
                    return true;
                case nameof(IsEnabledNavigateToFacilityOverview):
                    result = IsEnabledNavigateToFacilityOverview();
                    return true;
                case nameof(NavigateToFacility):
                    NavigateToFacility();
                    return true;
                case nameof(IsEnabledNavigateToFacility):
                    result = IsEnabledNavigateToFacility();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override ACMenuItemList GetMenu(string vbContent, string vbControl)
        {
            ACMenuItemList aCMenuItems = base.GetMenu(vbContent, vbControl);

            if (vbContent == nameof(SelectedFacilityBookingOverview) && SelectedFacilityBookingOverview != null)
            {
                ACMenuItemList facilityBookingMenuItems = ACFacilityManager.GetMenuForFacilityBooking(this, SelectedFacilityBookingOverview);
                aCMenuItems.AddRange(facilityBookingMenuItems);
            }

            if (vbContent == nameof(SelectedFacilityBookingChargeOverview) && SelectedFacilityBookingChargeOverview != null)
            {
                ACMenuItemList facilityBookingChargeMenuItems = ACFacilityManager.GetMenuForFacilityBookingCharge(SelectedFacilityBookingChargeOverview);
                aCMenuItems.AddRange(facilityBookingChargeMenuItems);
            }

            return aCMenuItems;
        }

        /// <summary>
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public override void ACAction(ACActionArgs actionArgs)
        {
            base.ACAction(actionArgs);

            if (actionArgs.ElementAction == Global.ElementActionType.ACCommand
                && actionArgs.DropObject != null
                && actionArgs.DropObject.ACContentList != null)
            {
                ACCommand acCommand = actionArgs.DropObject.ACContentList.Where(c => c is ACCommand).FirstOrDefault() as ACCommand;
                if (acCommand != null)
                {
                    if (acCommand.ACUrl.StartsWith("SelectedFacilityBooking") && (SelectedFacilityBookingOverview != null || SelectedFacilityBookingChargeOverview != null))
                    {
                        using (DatabaseApp databaseApp = new DatabaseApp())
                        {
                            string menuItemTypeStr = "";
                            if (acCommand.ACUrl.StartsWith("SelectedFacilityBookingOverview\\"))
                            {
                                menuItemTypeStr = acCommand.ACUrl.Replace("SelectedFacilityBookingOverview\\", "");
                                ACFacilityManager.HandleMenuForACCommand(databaseApp, SelectedFacilityBookingOverview, menuItemTypeStr);
                            }
                            else if (acCommand.ACUrl.StartsWith("SelectedFacilityBookingChargeOverview\\"))
                            {
                                menuItemTypeStr = acCommand.ACUrl.Replace("SelectedFacilityBookingChargeOverview\\", "");
                                ACFacilityManager.HandleMenuForACCommand(databaseApp, SelectedFacilityBookingChargeOverview, menuItemTypeStr);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods -> Private

        private void CleanMaterialRelatedLists()
        {
            SelectedFacilityCharge = null;
            CurrentFacilityCharge = null;
            _FacilityChargeList = null;

            SelectedFacilityChargeSumLocationHelper = null;
            CurrentFacilityChargeSumLocationHelper = null;
            _FacilityChargeSumLocationHelperList = null;

            SelectedFacilityChargeSumFacilityHelper = null;
            CurrentFacilityChargeSumFacilityHelper = null;
            _FacilityChargeSumFacilityHelperList = null;

            SelectedFacilityChargeSumLotHelper = null;
            CurrentFacilityChargeSumLotHelper = null;
            _FacilityChargeSumLotHelperList = null;
            RefreshMaterialRelatedLists();
        }

        private void RefreshMaterialRelatedLists()
        {
            OnPropertyChanged(nameof(FacilityChargeList));
            OnPropertyChanged(nameof(FacilityChargeSumLocationHelperList));
            OnPropertyChanged(nameof(FacilityChargeSumFacilityHelperList));
            OnPropertyChanged(nameof(FacilityChargeSumLotHelperList));
        }

        private void LoadMaterialRelatedLists(Guid? materialID)
        {
            if (materialID != null)
            {
                _FacilityChargeList = FacilityManager.s_cQry_MatOverviewFacilityCharge(this.DatabaseApp, materialID ?? Guid.Empty, ShowNotAvailable).ToArray();
            }
            else
            {
                _FacilityChargeList = null;
            }

            if (_FacilityChargeList == null || !_FacilityChargeList.Any())
            {
                CleanMaterialRelatedLists();
            }
            else
            {
                _FacilityChargeSumLocationHelperList = ACFacilityManager.GetFacilityChargeSumLocationHelperList(_FacilityChargeList, new FacilityQueryFilter()).ToList();
                _FacilityChargeSumFacilityHelperList = ACFacilityManager.GetFacilityChargeSumFacilityHelperList(_FacilityChargeList, new FacilityQueryFilter()).ToList();
                _FacilityChargeSumLotHelperList = ACFacilityManager.GetFacilityChargeSumLotHelperList(_FacilityChargeList, new FacilityQueryFilter() { MaterialID = materialID }).ToList();
                if (FilterLotNos != null && FilterLotNos.Any())
                {
                    _FacilityChargeSumLotHelperList = _FacilityChargeSumLotHelperList.Where(c => !FilterLotNos.Contains(c.FacilityLot.LotNo)).ToList();
                }
                RefreshMaterialRelatedLists();
            }
        }


        #region Methods -> Private -> FacilityBookingOverview

        #endregion

        #region Methods -> Private -> FacilityBookingOverview

        public virtual void CleanMovements()
        {
            _SelectedFacilityBookingOverview = null;
            _SelectedFacilityBookingChargeOverview = null;
            _FacilityBookingOverviewList = null;
            _FacilityBookingChargeOverviewList = null;
            OnPropertyChanged(nameof(FacilityBookingOverviewList));
            OnPropertyChanged(nameof(FacilityBookingChargeOverviewList));
        }

        private FacilityQueryFilter GetFacilityBookingFilter()
        {
            FacilityQueryFilter filter = new FacilityQueryFilter();
            filter.SearchFrom = FBSearchFrom;
            filter.SearchTo = FBSearchTo;
            filter.MaterialID = CurrentMaterialID;
            bool isFilterFBType = FilterFBType != null && FilterFBType.Value != null;
            if (isFilterFBType)
                filter.FilterFBTypeValue = (short)FilterFBType.Value;
            return filter;
        }

        #endregion

        #region

        private void ClearReservation()
        {
            if (BSOFacilityReservationOverview_Child != null && BSOFacilityReservationOverview_Child.Value != null)
            {
                BSOFacilityReservationOverview_Child.Value.ClearReservation();
            }
        }

        #endregion

        #endregion

        #endregion


        #region Background-Worker

        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();
            switch (command)
            {
                case nameof(DoFacilityBookingSearch):
                    DoFacilityBookingSearch();
                    break;
            }
        }

        public virtual List<FacilityBookingOverview> GroupFacilityBookingOverviewList(IEnumerable<FacilityBookingOverview> query)
        {
            if (ACFacilityManager == null)
                return null;
            return this.ACFacilityManager.GroupFacilityBookingOverviewList(query);
        }

        public virtual Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> GetFacilityOverviewLists(DatabaseApp databaseApp, FacilityQueryFilter filter)
        {
            if (ACFacilityManager == null)
                return null;
            return this.ACFacilityManager.GetFacilityOverviewLists(databaseApp, filter);
        }

        public virtual void DoFacilityBookingSearch()
        {
            FacilityQueryFilter filter = GetFacilityBookingFilter();
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> fbList = GetFacilityOverviewLists(dbApp, filter);
                _FacilityBookingOverviewList = fbList.Keys.ToList();
                if (FBGroupByMaterial)
                    _FacilityBookingOverviewList = GroupFacilityBookingOverviewList(_FacilityBookingOverviewList);
                _FacilityBookingChargeOverviewList = fbList.SelectMany(c => c.Value).ToList();
                OnFacilityBookingSearchSum();
            }
        }

        public virtual void OnFacilityBookingSearchSum()
        {
            if (FacilityBookingOverviewList != null)
            {
                double sum = 0.0;
                foreach (var fb in FacilityBookingOverviewList)
                {
                    sum += fb.InwardQuantityUOM - fb.OutwardQuantityUOM;
                    fb.InOutSumUOM = sum;
                }
            }

            if (FacilityBookingChargeOverviewList != null)
            {
                double sum = 0.0;
                foreach (var fb in FacilityBookingChargeOverviewList)
                {
                    sum += fb.InwardQuantityUOM - fb.OutwardQuantityUOM;
                    fb.InOutSumUOM = sum;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            if (e.Error != null)
            {

            }
            else if (e.Cancelled)
            {

            }
            else
            {

            }
            CloseTopDialog();
            string command = worker.EventArgs.Argument.ToString();
            switch (command)
            {
                case nameof(DoFacilityBookingSearch):
                    OnPropertyChanged(nameof(FacilityBookingOverviewList));
                    OnPropertyChanged(nameof(FacilityBookingChargeOverviewList));
                    break;
            }
        }

        #endregion
    }
}
