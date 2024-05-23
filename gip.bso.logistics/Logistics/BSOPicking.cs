// ***********************************************************************
// Assembly         : gip.bso.purchasing
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOPicking.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.manager;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
using System.Linq;
using System.Windows.Controls;
using static gip.core.datamodel.Global;
using static gip.mes.datamodel.GlobalApp;
using gipCoreData = gip.core.datamodel;

namespace gip.bso.logistics
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Picking Order'}de{'Kommissionierauftrag'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Picking.ClassName)]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "InOrderPosOpen", "en{'Open Purchase Order Pos.'}de{'Offene Bestellposition'}", typeof(InOrderPos), InOrderPos.ClassName, "MDDelivPosState\\MDDelivPosStateIndex", "TargetDeliveryDate,Material\\MaterialNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "OutOrderPosOpen", "en{'Open Sales Order Pos.'}de{'Offene Auftragsposition'}", typeof(OutOrderPos), OutOrderPos.ClassName, "MDDelivPosState\\MDDelivPosStateIndex", "TargetDeliveryDate,Material\\MaterialNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "ProdOrderPartslistPosOpen", "en{'Open Prod. Order Pos.'}de{'Offene Prod.-auftragsposition'}", typeof(ProdOrderPartslistPos), ProdOrderPartslistPos.ClassName, "ProdOrderPartslistPos1_ParentProdOrderPartslistPos", "Material\\MaterialNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "BookingFacility", ConstApp.Facility, typeof(Facility), Facility.ClassName, MDFacilityType.ClassName + "\\MDFacilityTypeIndex", "FacilityNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "PositionFacilityFrom", ConstApp.Facility, typeof(Facility), Facility.ClassName, MDFacilityType.ClassName + "\\MDFacilityTypeIndex", "FacilityNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "BookingFacilityLot", ConstApp.Lot, typeof(FacilityLot), FacilityLot.ClassName, "LotNo", "LotNo")]
    [ACClassConstructorInfo(
       new object[]
       {
            new object[] { "PickingType", Global.ParamOption.Optional, typeof(string) },
            new object[] { "FromFacility", Global.ParamOption.Optional, typeof(string) },
            new object[] { "ToFacility", Global.ParamOption.Optional, typeof(string) },
            new object[] { "CompanyNo", Global.ParamOption.Optional, typeof(string) },
            new object[] { "PickingStateIndex", Global.ParamOption.Optional, typeof(short) }
       }
   )]

    public partial class BSOPicking : ACBSOvbNav, IACBSOConfigStoreSelection, IACBSOACProgramProvider
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOPicking"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOPicking(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
            _ForwardToRemoteStores = new ACPropertyConfigValue<bool>(this, nameof(ForwardToRemoteStores), false);
        }

        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _InDeliveryNoteManager = ACInDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_InDeliveryNoteManager == null)
                throw new Exception("InDeliveryNoteManager not configured");

            _OutDeliveryNoteManager = ACOutDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_OutDeliveryNoteManager == null)
                throw new Exception("InDeliveryNoteManager not configured");

            _PickingManager = ACPickingManager.ACRefToServiceInstance(this);
            if (_PickingManager == null)
                throw new Exception("PickingManager not configured");

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            bool skipSearchOnStart = ParameterValueT<bool>(Const.SkipSearchOnStart);
            if (!skipSearchOnStart)
            {
                AccessFilterFromFacility.NavSearch();
                AccessFilterToFacility.NavSearch();

                if (Parameters != null && Parameters.Any())
                    InitParams();

                Search(false);
            }

            if (BSOFacilityReservation_Child != null && BSOFacilityReservation_Child.Value != null)
            {
                BSOFacilityReservation_Child.Value.OnReservationChanged += BSOFacilityRservation_ReservationChanged;
            }

            return true;
        }

        public void InitParams()
        {
            object pickingTypeOb = Parameters["PickingType"];
            if (pickingTypeOb != null)
            {
                string pickingType = pickingTypeOb.ToString();
                SelectedFilterMDPickingType = FilterMDPickingTypeList.FirstOrDefault(c => c.MDKey == pickingType);
            }

            object fromFacilityOb = Parameters["FromFacility"];
            if (fromFacilityOb != null)
            {
                string fromFacilityNo = fromFacilityOb.ToString();
                Facility fromFacility = DatabaseApp.Facility.FirstOrDefault(c => c.FacilityNo == fromFacilityNo);
                if (!AccessFilterFromFacility.NavList.Contains(fromFacility))
                    AccessFilterFromFacility.NavList.Add(fromFacility);
                SelectedFilterFromFacility = fromFacility;
            }

            object toFacilityOb = Parameters["ToFacility"];
            if (toFacilityOb != null)
            {
                string toFacilityNo = toFacilityOb.ToString();
                Facility toFacility = DatabaseApp.Facility.FirstOrDefault(c => c.FacilityNo == toFacilityNo);
                if (!AccessFilterToFacility.NavList.Contains(toFacility))
                    AccessFilterToFacility.NavList.Add(toFacility);
                SelectedFilterToFacility = toFacility;
            }

            object companyNoOb = Parameters["CompanyNo"];
            string companyNo = null;
            if (companyNoOb != null)
            {
                companyNo = companyNoOb.ToString();
                AccessFilterDeliveryAddress.NavACQueryDefinition.SetSearchValue("Company\\CompanyNo", Global.LogicalOperators.equal, companyNo);
                AccessFilterDeliveryAddress.NavSearch();
                SelectedFilterDeliveryAddress = FilterDeliveryAddressList.FirstOrDefault();
            }

            object stateIndex = Parameters["PickingStateIndex"];
            if (stateIndex != null)
                SelectedFilterPickingState = FilterPickingStateList.FirstOrDefault(c => (short)c.Value == (short)stateIndex);
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_PickingManager != null)
                ACPickingManager.DetachACRefFromServiceInstance(this, _PickingManager);
            _PickingManager = null;

            if (_InDeliveryNoteManager != null)
                ACInDeliveryNoteManager.DetachACRefFromServiceInstance(this, _InDeliveryNoteManager);
            _InDeliveryNoteManager = null;

            if (_OutDeliveryNoteManager != null)
                ACOutDeliveryNoteManager.DetachACRefFromServiceInstance(this, _OutDeliveryNoteManager);
            _OutDeliveryNoteManager = null;

            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            this._AccessBookingFacility = null;
            this._AccessInOrderPos = null;
            this._AccessOutOrderPos = null;
            this._AccessPickingPos = null;
            this._AccessProdOrderPartslistPos = null;
            this._CurrentACMethodBookingDummy = null;
            this._CurrentDNoteInOrderPos = null;
            this._CurrentDNoteOutOrderPos = null;
            this._CurrentFacilityBooking = null;
            this._CurrentFacilityPreBooking = null;
            this._CurrentPickingPos = null;
            this._SelectedDNoteInOrderPos = null;
            this._SelectedDNoteOutOrderPos = null;
            this._SelectedFacilityBooking = null;
            this._SelectedFacilityBookingCharge = null;
            this._SelectedFacilityPreBooking = null;
            this._SelectedPickingPos = null;
            this._BookingFacilityLotNo = null;
            this._PartialQuantity = null;
            this._StateCompletelyAssigned = null;
            this._UnSavedUnAssignedInOrderPos = null;
            this._UnSavedUnAssignedOutOrderPos = null;
            this._UnSavedUnAssignedProdOrderPartslistPos = null;
            _PreBookingAvailableQuantsList = null;
            _SelectedPreBookingAvailableQuants = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPickingPos != null)
            {
                _AccessPickingPos.ACDeInit(false);
                _AccessPickingPos = null;
            }
            if (_AccessBookingFacility != null)
            {
                _AccessBookingFacility.ACDeInit(false);
                _AccessBookingFacility = null;
            }
            if (_AccessInOrderPos != null)
            {
                _AccessInOrderPos.ACDeInit(false);
                _AccessInOrderPos = null;
            }
            if (_AccessOutOrderPos != null)
            {
                _AccessOutOrderPos.ACDeInit(false);
                _AccessOutOrderPos = null;
            }
            if (_AccessProdOrderPartslistPos != null)
            {
                _AccessProdOrderPartslistPos.ACDeInit(false);
                _AccessProdOrderPartslistPos = null;
            }
            if (_AccessPrimary != null)
            {
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }

            if (_AccessBookingFacilityLot != null)
            {
                _AccessBookingFacilityLot.NavSearchExecuted -= _AccessBookingFacilityLot_NavSearchExecuted;
                _AccessBookingFacilityLot.ACDeInit(false);
                _AccessBookingFacilityLot = null;
            }

            if (_AccessBookingFacilityTarget != null)
            {
                _AccessBookingFacilityTarget.ACDeInit(false);
                _AccessBookingFacilityTarget = null;
            }

            if (_AccessFilterFromFacility != null)
            {
                //_AccessFilterFromFacility.NavSearchExecuted -= _AccessBookingFacilityLot_NavSearchExecuted;
                _AccessFilterFromFacility.ACDeInit(false);
                _AccessFilterFromFacility = null;
            }

            if (_AccessFilterToFacility != null)
            {
                //_AccessFilterToFacility.NavSearchExecuted -= _AccessBookingFacilityLot_NavSearchExecuted;
                _AccessFilterToFacility.ACDeInit(false);
                _AccessFilterToFacility = null;
            }

            _IsInward = false;
            _QuantDialogMaterial = null;


            if (BSOFacilityReservation_Child != null && BSOFacilityReservation_Child.Value != null)
            {
                BSOFacilityReservation_Child.Value.OnReservationChanged -= BSOFacilityRservation_ReservationChanged;
            }

            return b;
        }

        public override object Clone()
        {
            BSOPicking clone = base.Clone() as BSOPicking;
            if (clone != null)
            {
                clone._SelectedFacilityBooking = this._SelectedFacilityBooking;
                clone._SelectedFacilityBookingCharge = this._SelectedFacilityBookingCharge;
            }
            return clone;
        }


        private void BSOFacilityRservation_ReservationChanged()
        {
            if (CurrentPickingPos != null)
            {
                CurrentPickingPos.OnEntityPropertyChanged(nameof(PickingPos.PickingMaterial));
            }
        }

        #endregion

        #region ChildBSO

        ACChildItem<BSOFacilityExplorer> _BSOFacilityExplorer_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo("BSOFacilityExplorer_Child", typeof(BSOFacilityExplorer))]
        public ACChildItem<BSOFacilityExplorer> BSOFacilityExplorer_Child
        {
            get
            {
                if (_BSOFacilityExplorer_Child == null)
                    _BSOFacilityExplorer_Child = new ACChildItem<BSOFacilityExplorer>(this, "BSOFacilityExplorer_Child");
                return _BSOFacilityExplorer_Child;
            }
        }


        ACChildItem<BSOFacilityReservation> _BSOFacilityReservation_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo(nameof(BSOFacilityReservation_Child), typeof(BSOFacilityReservation))]
        public ACChildItem<BSOFacilityReservation> BSOFacilityReservation_Child
        {
            get
            {
                if (_BSOFacilityReservation_Child == null)
                    _BSOFacilityReservation_Child = new ACChildItem<BSOFacilityReservation>(this, nameof(BSOFacilityReservation_Child));
                return _BSOFacilityReservation_Child;
            }
        }

        ACChildItem<BSOPreferredParameters> _BSOPreferredParameters;
        [ACPropertyInfo(603)]
        [ACChildInfo(nameof(BSOPreferredParameters_Child), typeof(BSOPreferredParameters))]
        public ACChildItem<BSOPreferredParameters> BSOPreferredParameters_Child
        {
            get
            {
                if (_BSOPreferredParameters == null)
                    _BSOPreferredParameters = new ACChildItem<BSOPreferredParameters>(this, nameof(BSOPreferredParameters_Child));
                return _BSOPreferredParameters;
            }
        }

        #endregion

        #region BSO->ACProperty

        #region Picking -> Filter

        #region Picking -> Filter -> Default filters
        public List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                ACFilterItem acFilterPickingNo = new ACFilterItem(Global.FilterTypes.filter, "PickingNo", Global.LogicalOperators.contains, Global.Operators.and, null, true, true);
                aCFilterItems.Add(acFilterPickingNo);

                ACFilterItem acFilterPickingIndex = new ACFilterItem(Global.FilterTypes.filter, "MDPickingType\\MDKey", Global.LogicalOperators.equal, Global.Operators.and, null, true);
                aCFilterItems.Add(acFilterPickingIndex);

                ACFilterItem phOpen = new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true);
                aCFilterItems.Add(phOpen);

                ACFilterItem acFilterDeliveryDateFrom = new ACFilterItem(FilterTypes.filter, "DeliveryDateFrom", LogicalOperators.greaterThanOrEqual, Operators.and, "", true, false);
                aCFilterItems.Add(acFilterDeliveryDateFrom);

                ACFilterItem acFilterDeliveryDateTo = new ACFilterItem(FilterTypes.filter, "DeliveryDateTo", LogicalOperators.lessThan, Operators.and, "", true, false);
                aCFilterItems.Add(acFilterDeliveryDateTo);

                ACFilterItem phClose = new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true);
                aCFilterItems.Add(phClose);

                ACFilterItem acFilterPickingStateIndex = new ACFilterItem(FilterTypes.filter, "PickingStateIndex", LogicalOperators.equal, Operators.and, "", true);
                aCFilterItems.Add(acFilterPickingStateIndex);

                ACFilterItem acFilterCompanyNo = new ACFilterItem(FilterTypes.filter, "DeliveryCompanyAddress\\CompanyNo", LogicalOperators.equal, Operators.and, "", true);
                aCFilterItems.Add(acFilterCompanyNo);


                return aCFilterItems;
            }
        }

        private List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem acSortPickingNo = new ACSortItem("PickingNo", SortDirections.descending, true);
                acSortItems.Add(acSortPickingNo);

                return acSortItems;
            }
        }

        #endregion

        #region Picking -> Filter -> Properties

        public bool InFilterChange { get; set; }

        #region Picking -> Filter -> Properties -> FacilityLot
        /// <summary>
        /// Source Property:  "en{'Extern Lot No.'}de{'Externe Losnr.'}"
        /// </summary>
        private string _FilterLotNoFB;
        [ACPropertyInfo(999, nameof(FilterLotNoFB), ConstApp.LotNo)]
        public string FilterLotNoFB
        {
            get
            {
                return _FilterLotNoFB;
            }
            set
            {
                if (_FilterLotNoFB != value)
                {
                    _FilterLotNoFB = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterExternLotNoFB;
        [ACPropertyInfo(999, nameof(FilterExternLotNoFB), ConstApp.ExternLotNo)]
        public string FilterExternLotNoFB
        {
            get
            {
                return _FilterExternLotNoFB;
            }
            set
            {
                if (_FilterExternLotNoFB != value)
                {
                    _FilterExternLotNoFB = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterLotNoFR;
        [ACPropertyInfo(999, nameof(FilterLotNoFR), ConstApp.LotNo)]
        public string FilterLotNoFR
        {
            get
            {
                return _FilterLotNoFR;
            }
            set
            {
                if (_FilterLotNoFR != value)
                {
                    _FilterLotNoFR = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterExternLotNoFR;
        [ACPropertyInfo(999, nameof(FilterExternLotNoFR), ConstApp.ExternLotNo)]
        public string FilterExternLotNoFR
        {
            get
            {
                return _FilterExternLotNoFR;
            }
            set
            {
                if (_FilterExternLotNoFR != value)
                {
                    _FilterExternLotNoFR = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        [ACPropertyInfo(300, "FilterPickingPickingNo", "en{'Picking-No.'}de{'Kommissions-Nr.'}")]
        public string FilterPickingPickingNo
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("PickingNo");
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("PickingNo");
                if (tmp != value)
                {
                    InFilterChange = true;

                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>("PickingNo", value);
                    OnPropertyChanged(nameof(FilterPickingPickingNo));

                    InFilterChange = false;
                }
            }
        }

        #region Picking -> Filter -> Properties -> Date

        [ACPropertyInfo(301, "FilterDateFrom", "en{'From'}de{'Von'}")]
        public DateTime? FilterDateFrom
        {
            get
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("DeliveryDateFrom");
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<DateTime>("DeliveryDateFrom");
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("DeliveryDateFrom", LogicalOperators.greaterThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                {
                    if (value.HasValue)
                    {
                        AccessPrimary.NavACQueryDefinition.SetSearchValue("DeliveryDateFrom", Global.LogicalOperators.greaterThanOrEqual, value.Value);
                        OnPropertyChanged(nameof(FilterDateFrom));
                    }
                }
                else
                {
                    if (value.HasValue)
                    {
                        DateTime tmpdt = AccessInOrderPos.NavACQueryDefinition.GetSearchValue<DateTime>("DeliveryDateFrom", Global.LogicalOperators.greaterThanOrEqual);
                        if (tmpdt != value)
                        {
                            AccessPrimary.NavACQueryDefinition.SetSearchValue("DeliveryDateFrom", Global.LogicalOperators.greaterThanOrEqual, value.Value);
                            OnPropertyChanged(nameof(FilterDateFrom));
                        }
                    }
                    else
                    {
                        AccessPrimary.NavACQueryDefinition.SetSearchValue("DeliveryDateFrom", Global.LogicalOperators.greaterThanOrEqual, "");
                        OnPropertyChanged(nameof(FilterDateFrom));
                    }
                }
            }
        }

        [ACPropertyInfo(302, "FilterDateTo", "en{'to'}de{'bis'}")]
        public DateTime? FilterDateTo
        {
            get
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("DeliveryDateTo");
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<DateTime>("DeliveryDateTo");
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("DeliveryDateTo", LogicalOperators.lessThan);
                if (String.IsNullOrEmpty(tmp))
                {
                    if (value.HasValue)
                    {
                        AccessPrimary.NavACQueryDefinition.SetSearchValue("DeliveryDateTo", Global.LogicalOperators.lessThan, value.Value);
                        OnPropertyChanged(nameof(FilterDateTo));
                    }
                }
                else
                {
                    if (value.HasValue)
                    {
                        DateTime tmpdt = AccessInOrderPos.NavACQueryDefinition.GetSearchValue<DateTime>("DeliveryDateTo", Global.LogicalOperators.lessThan);
                        if (tmpdt != value)
                        {
                            AccessPrimary.NavACQueryDefinition.SetSearchValue("DeliveryDateTo", Global.LogicalOperators.lessThan, value.Value);
                            OnPropertyChanged(nameof(FilterDateTo));
                        }
                    }
                    else
                    {
                        AccessPrimary.NavACQueryDefinition.SetSearchValue("DeliveryDateTo", Global.LogicalOperators.lessThan, "");
                        OnPropertyChanged(nameof(FilterDateTo));
                    }
                }
            }
        }

        #endregion

        #region Picking -> Filter -> Properties -> FilterPickingTypeIndex -> FilterMDPickingType
        private MDPickingType _SelectedFilterMDPickingType;
        /// <summary>
        /// Selected property for MDPickingType
        /// </summary>
        /// <value>The selected FilterMDPickingType</value>
        [ACPropertySelected(303, "FilterMDPickingType", "en{'Picking Type'}de{'Kommissioniertyp'}")]
        public MDPickingType SelectedFilterMDPickingType
        {
            get
            {
                return _SelectedFilterMDPickingType;
            }
            set
            {
                if (_SelectedFilterMDPickingType != value)
                {
                    _SelectedFilterMDPickingType = value;
                    OnPropertyChanged(nameof(SelectedFilterMDPickingType));

                    string filterPickingKey = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("MDPickingType\\MDKey");
                    string newPickingKey = null;
                    if (value != null)
                        newPickingKey = value.MDKey;
                    if (newPickingKey != filterPickingKey)
                    {
                        InFilterChange = true;

                        AccessPrimary.NavACQueryDefinition.SetSearchValue<string>("MDPickingType\\MDKey", newPickingKey);
                        OnPropertyChanged(nameof(SelectedFilterMDPickingType));

                        InFilterChange = false;
                    }
                }
            }
        }


        private List<MDPickingType> _FilterMDPickingTypeList;
        /// <summary>
        /// List property for MDPickingType
        /// </summary>
        /// <value>The FilterMDPickingType list</value>
        [ACPropertyList(304, "FilterMDPickingType")]
        public List<MDPickingType> FilterMDPickingTypeList
        {
            get
            {
                if (_FilterMDPickingTypeList == null)
                    _FilterMDPickingTypeList = LoadFilterMDPickingTypeList();
                short? filterPickingTypeIndex = AccessPrimary.NavACQueryDefinition.GetSearchValue<short?>("PickingTypeIndex");
                if (filterPickingTypeIndex != null)
                    SelectedFilterMDPickingType = FilterMDPickingTypeList.FirstOrDefault(c => c.MDPickingTypeIndex == (short)filterPickingTypeIndex.Value);
                return _FilterMDPickingTypeList;
            }
        }

        private List<MDPickingType> LoadFilterMDPickingTypeList()
        {
            return DatabaseApp.MDPickingType.OrderBy(c => c.SortIndex).ToList();

        }
        #endregion

        #region Picking -> Filter -> Properties -> FilterPickingState
        private ACValueItem _SelectedFilterPickingState;
        /// <summary>
        /// Selected property for ACValueItem
        /// </summary>
        /// <value>The selected FilterPickingState</value>
        [ACPropertySelected(305, "FilterPickingState", "en{'Picking Status'}de{'Status'}")]
        public ACValueItem SelectedFilterPickingState
        {
            get
            {
                return _SelectedFilterPickingState;
            }
            set
            {
                if (_SelectedFilterPickingState != value)
                {
                    _SelectedFilterPickingState = value;
                    OnPropertyChanged(nameof(SelectedFilterPickingState));

                    short? filterPickingStateIndex = AccessPrimary.NavACQueryDefinition.GetSearchValue<short?>("PickingStateIndex");
                    short? newPickingStateIndex = null;
                    if (value != null)
                        newPickingStateIndex = (short)(value.Value);
                    if (newPickingStateIndex != filterPickingStateIndex)
                    {
                        InFilterChange = true;

                        AccessPrimary.NavACQueryDefinition.SetSearchValue<short?>("PickingStateIndex", newPickingStateIndex);
                        OnPropertyChanged(nameof(SelectedFilterPickingState));

                        InFilterChange = false;
                    }
                }
            }
        }

        private List<ACValueItem> _FilterPickingStateList;
        /// <summary>
        /// List property for ACValueItem
        /// </summary>
        /// <value>The FilterPickingState list</value>
        [ACPropertyList(306, "FilterPickingState")]
        public List<ACValueItem> FilterPickingStateList
        {
            get
            {
                if (_FilterPickingStateList == null)
                    _FilterPickingStateList = LoadFilterPickingStateList();
                short? filterPickingStateIndex = AccessPrimary.NavACQueryDefinition.GetSearchValue<short?>("PickingStateIndex");
                if (filterPickingStateIndex != null)
                {
                    SelectedFilterPickingState = _FilterPickingStateList.FirstOrDefault(c => (short)c.Value == (short)filterPickingStateIndex.Value);
                }
                return _FilterPickingStateList;
            }
        }

        private List<ACValueItem> LoadFilterPickingStateList()
        {
            return DatabaseApp.PickingStateList;
        }

        #endregion

        #region Picking -> Filter -> Properties -> FilterDeliveryAddress


        public List<ACFilterItem> FilterDeliveryAddressDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                ACFilterItem acFilterCompanyNo = new ACFilterItem(Global.FilterTypes.filter, "Company\\CompanyNo", Global.LogicalOperators.equal, Global.Operators.and, null, true, true);
                aCFilterItems.Add(acFilterCompanyNo);

                return aCFilterItems;
            }
        }

        private List<ACSortItem> FilterDeliveryAddressDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem acSortPickingNo = new ACSortItem("Company\\CompanyNo", SortDirections.descending, true);
                acSortItems.Add(acSortPickingNo);

                return acSortItems;
            }
        }

        ///
        ACAccess<CompanyAddress> _AccessFilterDeliveryAddress;
        [ACPropertyAccess(9999, "FilterDeliveryAddress")]
        public ACAccess<CompanyAddress> AccessFilterDeliveryAddress
        {
            get
            {
                if (_AccessFilterDeliveryAddress == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + CompanyAddress.ClassName, ACType.ACIdentifier);

                    acQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(FilterDeliveryAddressDefaultSort);
                    if (acQueryDefinition.TakeCount == 0)
                        acQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                    acQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(FilterDeliveryAddressDefaultFilter);

                    _AccessFilterDeliveryAddress = acQueryDefinition.NewAccessNav<CompanyAddress>("FilterDeliveryAddress", this);
                    _AccessFilterDeliveryAddress.NavSearch();
                }
                return _AccessFilterDeliveryAddress;
            }
        }

        [ACPropertyInfo(9999, "FilterDeliveryAddress")]
        public IEnumerable<CompanyAddress> FilterDeliveryAddressList
        {
            get
            {
                return AccessFilterDeliveryAddress.NavList;
            }
        }

        private CompanyAddress _SelectedFilterDeliveryAddress;
        [ACPropertySelected(9999, "FilterDeliveryAddress", "en{'Delivery Address'}de{'Lieferadresse'}")]
        public CompanyAddress SelectedFilterDeliveryAddress
        {
            get
            {
                return _SelectedFilterDeliveryAddress;
            }
            set
            {
                if (_SelectedFilterDeliveryAddress != value)
                {
                    _SelectedFilterDeliveryAddress = value;

                    OnPropertyChanged(nameof(SelectedFilterDeliveryAddress));
                }
            }
        }


        #endregion

        #region Filter -> From(To)Facility

        public List<ACFilterItem> FilterFacilityNavigationqueryDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                return aCFilterItems;
            }
        }

        private List<ACSortItem> FilterFacilityNavigationqueryDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem acSortPickingNo = new ACSortItem("FacilityNo", SortDirections.ascending, true);
                acSortItems.Add(acSortPickingNo);

                return acSortItems;
            }
        }


        ACAccess<Facility> _AccessFilterFromFacility;
        [ACPropertyAccess(9999, "FilterFromFacility")]
        public ACAccess<Facility> AccessFilterFromFacility
        {
            get
            {
                if (_AccessFilterFromFacility == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Facility.ClassName, ACType.ACIdentifier);

                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(FilterFacilityNavigationqueryDefaultSort);
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(FilterFacilityNavigationqueryDefaultFilter);

                        foreach (ACFilterItem aCFilterItem in navACQueryDefinition.ACFilterColumns)
                            aCFilterItem.PropertyChanged += ACFilterItem_PropertyChanged;
                    }

                    _AccessFilterFromFacility = navACQueryDefinition.NewAccessNav<Facility>("FilterFromFacility", this);
                    //_AccessFilterFromFacility.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessFilterFromFacility;
            }
        }

        [ACPropertyInfo(9999, "FilterFromFacility")]
        public IEnumerable<Facility> FilterFromFacilityList
        {
            get
            {
                return AccessFilterFromFacility.NavList;
            }
        }

        private Facility _SelectedFilterFromFacility;
        [ACPropertySelected(9999, "FilterFromFacility", "en{'From facility'}de{'Von Lagerplatz'}")]
        public Facility SelectedFilterFromFacility
        {
            get
            {
                return _SelectedFilterFromFacility;
            }
            set
            {
                if (_SelectedFilterFromFacility != value)
                {
                    _SelectedFilterFromFacility = value;
                    OnPropertyChanged(nameof(SelectedFilterFromFacility));
                }
            }
        }

        ACAccess<Facility> _AccessFilterToFacility;
        [ACPropertyAccess(9999, "FilterToFacility")]
        public ACAccess<Facility> AccessFilterToFacility
        {
            get
            {
                if (_AccessFilterToFacility == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Facility.ClassName, ACType.ACIdentifier);

                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(FilterFacilityNavigationqueryDefaultSort);
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(FilterFacilityNavigationqueryDefaultFilter);

                        foreach (ACFilterItem aCFilterItem in navACQueryDefinition.ACFilterColumns)
                            aCFilterItem.PropertyChanged += ACFilterItem_PropertyChanged;
                    }

                    _AccessFilterToFacility = navACQueryDefinition.NewAccessNav<Facility>("FilterToFacility", this);
                    //_AccessFilterToFacility.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessFilterToFacility;
            }
        }

        [ACPropertyInfo(9999, "FilterToFacility")]
        public IEnumerable<Facility> FilterToFacilityList
        {
            get
            {
                return AccessFilterToFacility.NavList;
            }
        }

        private Facility _SelectedFilterToFacility;
        [ACPropertySelected(9999, "FilterToFacility", "en{'To facility'}de{'Zur Lagerplatz'}")]
        public Facility SelectedFilterToFacility
        {
            get
            {
                return _SelectedFilterToFacility;
            }
            set
            {
                if (_SelectedFilterToFacility != value)
                {
                    _SelectedFilterToFacility = value;
                    OnPropertyChanged(nameof(SelectedFilterToFacility));
                }
            }
        }

        #endregion


        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterMaterialNo;
        [ACPropertyInfo(999, "FilterMaterialNo", ConstApp.Material)]
        public string FilterMaterialNo
        {
            get
            {
                return _FilterMaterialNo;
            }
            set
            {
                if (_FilterMaterialNo != value)
                {
                    _FilterMaterialNo = value;
                    OnPropertyChanged(nameof(FilterMaterialNo));
                }
            }
        }


        #endregion

        #endregion

        #region Picking
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<Picking> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, "Picking")]
        public virtual ACAccessNav<Picking> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);

                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(NavigationqueryDefaultSort);
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);

                        foreach (ACFilterItem aCFilterItem in navACQueryDefinition.ACFilterColumns)
                            aCFilterItem.PropertyChanged += ACFilterItem_PropertyChanged;
                    }

                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Picking>("Picking", this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;

                }
                return _AccessPrimary;
            }
        }

        private void ACFilterItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ACFilterItem aCFilterItem = sender as ACFilterItem;
            if (e.PropertyName == "SearchWord" && !InFilterChange)
            {
                switch (aCFilterItem.PropertyName)
                {
                    case nameof(Picking.PickingNo):
                        OnPropertyChanged(nameof(FilterPickingPickingNo));
                        break;
                    case nameof(Picking.DeliveryDateFrom):
                        OnPropertyChanged(nameof(FilterDateFrom));
                        OnPropertyChanged(nameof(FilterDateTo));
                        break;
                    case nameof(MDPickingType.MDPickingTypeIndex):
                        OnPropertyChanged(nameof(SelectedFilterMDPickingType));
                        break;
                    case nameof(Picking.PickingStateIndex):
                        OnPropertyChanged(nameof(SelectedFilterPickingState));
                        break;
                }
            }
        }

        protected IQueryable<Picking> _AccessPrimary_NavSearchExecuting(IQueryable<Picking> result)
        {
            ObjectQuery<Picking> query = result as ObjectQuery<Picking>;
            if (query != null)
            {
                query.Include(c => c.VisitorVoucher).Include(c => c.MDPickingType);
            }

            if (SelectedFilterDeliveryAddress != null)
                result = result.Where(c => c.DeliveryCompanyAddressID == SelectedFilterDeliveryAddress.CompanyAddressID);

            if (SelectedFilterFromFacility != null)
            {
                result =
                    result
                    .Where(c =>
                                !c.PickingPos_Picking.Any()// show picking without positions
                                || c.PickingPos_Picking
                                .Any(x =>
                                            x.FromFacility != null
                                            && x.FromFacility.FacilityID == SelectedFilterFromFacility.FacilityID
                                    )
                          );
            }

            if (SelectedFilterToFacility != null)
            {
                result =
                    result
                    .Where(c =>
                                !c.PickingPos_Picking.Any() // show picking without positions
                                || c.PickingPos_Picking
                                .Any(x =>
                                            x.ToFacility != null
                                            && x.ToFacility.FacilityID == SelectedFilterToFacility.FacilityID
                                    )
                            );
            }

            if (!string.IsNullOrEmpty(FilterMaterialNo))
                result =
                    result
                    .Where(c =>
                                c.PickingPos_Picking
                                .Any(x =>
                                            (
                                                x.InOrderPos != null
                                                && (x.InOrderPos.Material.MaterialNo.Contains(FilterMaterialNo) || x.InOrderPos.Material.MaterialName1.Contains(FilterMaterialNo))
                                            )
                                            || (
                                                x.OutOrderPos != null
                                                && (x.OutOrderPos.Material.MaterialNo.Contains(FilterMaterialNo) || x.OutOrderPos.Material.MaterialName1.Contains(FilterMaterialNo))
                                            ) || (
                                                x.PickingMaterial != null
                                                && (x.PickingMaterial.MaterialNo.Contains(FilterMaterialNo) || x.PickingMaterial.MaterialName1.Contains(FilterMaterialNo))
                                            )

                                )
                           );

            // FilterLotNoFB
            if (!string.IsNullOrEmpty(FilterLotNoFB))
            {
                result =
                    result
                    .Where(c =>
                            c.PickingPos_Picking
                            .SelectMany(x => x.FacilityBookingCharge_PickingPos)
                            .Where(x =>
                                        (
                                            x.InwardFacilityLot != null
                                            && x.InwardFacilityLot.LotNo.Contains(FilterLotNoFB)
                                        )
                                        ||
                                        (
                                            x.InwardFacilityCharge != null
                                            && x.InwardFacilityCharge.FacilityLot != null
                                            && x.InwardFacilityCharge.FacilityLot.LotNo.Contains(FilterLotNoFB)
                                        )
                                        ||
                                        (
                                            x.OutwardFacilityLot != null
                                            && x.OutwardFacilityLot.LotNo.Contains(FilterLotNoFB)
                                        )
                                        ||
                                        (
                                            x.OutwardFacilityCharge != null
                                            && x.OutwardFacilityCharge.FacilityLot != null
                                            && x.OutwardFacilityCharge.FacilityLot.LotNo.Contains(FilterLotNoFB)
                                        )
                                    )
                            .Any()
                        );
            }

            // FilterExternLotNoFB
            if (!string.IsNullOrEmpty(FilterExternLotNoFB))
            {
                result =
                    result
                    .Where(c =>
                            c.PickingPos_Picking
                            .SelectMany(x => x.FacilityBookingCharge_PickingPos)
                            .Where(x =>
                                        (
                                            x.InwardFacilityLot != null
                                            && !string.IsNullOrEmpty(x.InwardFacilityLot.ExternLotNo)
                                            && x.InwardFacilityLot.ExternLotNo.Contains(FilterExternLotNoFB)
                                        )
                                        ||
                                        (
                                            x.InwardFacilityCharge != null
                                            && x.InwardFacilityCharge.FacilityLot != null
                                            && !string.IsNullOrEmpty(x.InwardFacilityCharge.FacilityLot.ExternLotNo)
                                            && x.InwardFacilityCharge.FacilityLot.ExternLotNo.Contains(FilterExternLotNoFB)
                                        )
                                        ||
                                        (
                                            x.OutwardFacilityLot != null
                                            && !string.IsNullOrEmpty(x.OutwardFacilityLot.ExternLotNo)
                                            && x.OutwardFacilityLot.ExternLotNo.Contains(FilterExternLotNoFB)
                                        )
                                        ||
                                        (
                                            x.OutwardFacilityCharge != null
                                            && x.OutwardFacilityCharge.FacilityLot != null
                                            && !string.IsNullOrEmpty(x.OutwardFacilityCharge.FacilityLot.ExternLotNo)
                                            && x.OutwardFacilityCharge.FacilityLot.ExternLotNo.Contains(FilterExternLotNoFB)
                                        )
                                    )
                            .Any()
                        );
            }


            // FilterLotNoFR
            if (!string.IsNullOrEmpty(FilterLotNoFR))
            {
                result =
                    result
                    .Where(c =>
                            c.PickingPos_Picking
                            .SelectMany(x => x.FacilityReservation_PickingPos)
                            .Where(x =>
                                        x.FacilityLot != null
                                        && x.FacilityLot.LotNo.Contains(FilterLotNoFR)
                                    )
                            .Any()
                        );
            }

            // FilterExternLotNoFR
            if (!string.IsNullOrEmpty(FilterExternLotNoFR))
            {
                result =
                    result
                    .Where(c =>
                            c.PickingPos_Picking
                            .SelectMany(x => x.FacilityReservation_PickingPos)
                            .Where(x =>
                                        x.FacilityLot != null
                                        && !string.IsNullOrEmpty(x.FacilityLot.ExternLotNo)
                                        && x.FacilityLot.LotNo.Contains(FilterExternLotNoFR)
                                    )
                            .Any()
                        );
            }

            return result;
        }


        /// <summary>
        /// Gets or sets the current delivery note.
        /// </summary>
        /// <value>The current delivery note.</value>
        [ACPropertyCurrent(601, "Picking")]
        public Picking CurrentPicking
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Current = value;

                if (ProcessWorkflowList != null && ProcessWorkflowList.Any() && value != null && value.ACClassMethodID != null)
                    SelectedProcessWorkflow = ProcessWorkflowList.FirstOrDefault(p => p.ACClassMethodID == CurrentPicking.ACClassMethodID);
                else
                    SelectedProcessWorkflow = null;
                CurrentProcessWorkflow = SelectedProcessWorkflow;

                OnPropertyChanged(nameof(CurrentPicking));
                if (value != null && _InLoad)
                    value.PickingPos_Picking.AutoRefresh(this.DatabaseApp);
                OnPropertyChanged(nameof(PickingPosList));
                if (value != null && CurrentPickingPos != null)
                {
                    if (!value.PickingPos_Picking.Any())
                        CurrentPickingPos = null;
                }
                OnPropertyChanged(nameof(CurrentPickingPos));
                RefreshInOrderPosList();
                RefreshOutOrderPosList();
                RefreshProdOrderPartslistPosList();
                if (value != null
                    && ForwardToRemoteStores)
                {
                    if (_VisitedPickings == null)
                        _VisitedPickings = new List<Picking>();
                    if (!_VisitedPickings.Contains(value))
                        _VisitedPickings.Add(value);
                }
            }
        }

        private List<Picking> _PickingList;
        /// <summary>
        /// Gets the delivery note list.
        /// </summary>
        /// <value>The delivery note list.</value>
        [ACPropertyList(602, "Picking")]
        public List<Picking> PickingList
        {
            get
            {
                return _PickingList;
            }
            set
            {
                _PickingList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected delivery note.
        /// </summary>
        /// <value>The selected delivery note.</value>
        [ACPropertySelected(603, "Picking")]
        public Picking SelectedPicking
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Selected = value;
                OnPropertyChanged(nameof(SelectedPicking));
            }
        }

        [ACMethodInfo("", "en{'Finish order'}de{'Auftrag beenden'}", 650, true)]
        public virtual void FinishOrder()
        {
            if (PickingManager == null)
                return;

            MsgWithDetails msgWithDetails = PickingManager.FinishOrder(DatabaseApp, CurrentPicking, InDeliveryNoteManager, OutDeliveryNoteManager, ACFacilityManager);
            if (msgWithDetails != null)
            {
                if (Messages.Msg(msgWithDetails, MsgResult.No, eMsgButton.YesNo) == MsgResult.Yes)
                {
                    msgWithDetails = PickingManager.FinishOrder(DatabaseApp, CurrentPicking, InDeliveryNoteManager, OutDeliveryNoteManager, ACFacilityManager, true);
                    if (msgWithDetails != null)
                    {
                        Messages.Msg(msgWithDetails);
                    }
                }
            }
        }

        public virtual bool IsEnabledFinishOrder()
        {
            return CurrentPicking != null && CurrentPicking.PickingStateIndex < (short)PickingStateEnum.Finished;
        }

        #endregion

        #region PickingPos

        #region PickingPos -> PositionFacilityFrom

        private bool _FilterPositionFacilityFrom = true;
        [ACPropertyInfo(814, "", "en{'Only show source bins with material'}de{'Zeige Quelle-Lagerplätze mit Material'}")]
        public bool FilterPositionFacilityFrom
        {
            get
            {
                return _FilterPositionFacilityFrom;
            }
            set
            {
                if (_FilterPositionFacilityFrom != value)
                {
                    _FilterPositionFacilityFrom = value;
                    OnPropertyChanged();
                    RefreshFilterFacilityAccess(AccessPositionFacilityFrom, value);
                    OnPropertyChanged(nameof(PositionFacilityFromList));
                }
            }
        }

        ACAccessNav<Facility> _AccessPositionFacilityFrom;
        [ACPropertyAccess(815, "PositionFacilityFrom")]
        public ACAccessNav<Facility> AccessPositionFacilityFrom
        {
            get
            {
                if (_AccessPositionFacilityFrom == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(this, Const.QueryPrefix + "PositionFacilityFrom", ACType.ACIdentifier);
                    _AccessPositionFacilityFrom = navACQueryDefinition.NewAccessNav<Facility>("PositionFacilityFrom", this);
                    _AccessPositionFacilityFrom.AutoSaveOnNavigation = false;
                    RefreshFilterFacilityAccess(_AccessPositionFacilityFrom, FilterPositionFacilityFrom);
                }
                return _AccessBookingFacilityTarget;
            }
        }


        [ACPropertyList(817, "PositionFacilityFrom")]
        public IList<Facility> PositionFacilityFromList
        {
            get
            {
                return AccessPositionFacilityFrom?.NavList;
            }
        }

        #endregion

        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccess<PickingPos> _AccessPickingPos;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(691, "PickingPos")]
        public ACAccess<PickingPos> AccessPickingPos
        {
            get
            {
                if (_AccessPickingPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + "PickingPos") as ACQueryDefinition;
                    _AccessPickingPos = acQueryDefinition.NewAccess<PickingPos>("PickingPos", this);
                }
                return _AccessPickingPos;
            }
        }

        /// <summary>
        /// The _ current delivery note pos
        /// </summary>
        PickingPos _CurrentPickingPos;
        /// <summary>
        /// Gets or sets the current delivery note pos.
        /// </summary>
        /// <value>The current delivery note pos.</value>
        [ACPropertyCurrent(604, "PickingPos")]
        public PickingPos CurrentPickingPos
        {
            get
            {
                return _CurrentPickingPos;
            }
            set
            {
                if (_CurrentPickingPos != value)
                {
                    _CurrentPickingPos = value;
                    if (_CurrentPickingPos != null && _CurrentPickingPos.EntityState != EntityState.Added)
                    {
                        _CurrentPickingPos.AutoRefresh();
                        if (_CurrentPickingPos.InOrderPos != null)
                        {
                            _CurrentPickingPos.InOrderPos.AutoRefresh();
                            _CurrentPickingPos.InOrderPos.FacilityPreBooking_InOrderPos.AutoLoad();
                            _CurrentPickingPos.InOrderPos.FacilityPreBooking_InOrderPos.AutoRefresh();
                        }
                        else if (_CurrentPickingPos.OutOrderPos != null)
                        {
                            _CurrentPickingPos.OutOrderPos.AutoRefresh();
                            _CurrentPickingPos.OutOrderPos.FacilityPreBooking_OutOrderPos.AutoLoad();
                            _CurrentPickingPos.OutOrderPos.FacilityPreBooking_OutOrderPos.AutoRefresh();
                        }
                        _CurrentPickingPos.FacilityPreBooking_PickingPos.AutoLoad();
                        _CurrentPickingPos.FacilityPreBooking_PickingPos.AutoRefresh();

                        _CurrentPickingPos.FacilityBooking_PickingPos.AutoLoad();
                        _CurrentPickingPos.FacilityBooking_PickingPos.AutoRefresh();
                    }
                    OnPropertyChanged(nameof(CurrentPickingPos));
                    OnPropertyChanged(nameof(FacilityPreBookingList));
                    OnPropertyChanged(nameof(FacilityBookingList));
                    if (FacilityPreBookingList != null && FacilityPreBookingList.Any())
                        SelectedFacilityPreBooking = FacilityPreBookingList.First();
                    else
                        SelectedFacilityPreBooking = null;
                    RefreshBookingFilterFacilityAccess(AccessBookingFacility, BookingFilterMaterial);
                    RefreshBookingFilterFacilityAccess(AccessBookingFacilityTarget, BookingFilterMaterialTarget);
                    RefreshFilterFacilityAccess(AccessPositionFacilityFrom, FilterPositionFacilityFrom);
                    if (AccessBookingFacilityLot != null)
                        RefreshFilterFacilityLotAccess(_AccessBookingFacilityLot);
                    OnPropertyChanged(nameof(BookingFacilityList));
                    OnPropertyChanged(nameof(BookingFacilityListTarget));

                    if (CurrentPickingPos != null)
                        CurrentPickingPos.PropertyChanged += CurrentPickingPos_PropertyChanged;

                    CurrentMDUnit = CurrentPickingPos?.MDUnit;

                    if (BSOFacilityReservation_Child != null && BSOFacilityReservation_Child.Value != null)
                    {
                        BSOFacilityReservation_Child.Value.FacilityReservationOwner = value;
                    }
                }
            }
        }

        private void CurrentPickingPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentPickingPos.PickingMaterialID):
                    OnPropertyChanged(nameof(MDUnitList));
                    if (FilterPositionFacilityFrom)
                    {
                        RefreshFilterFacilityAccess(AccessPositionFacilityFrom, FilterPositionFacilityFrom);
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets the delivery note pos list.
        /// </summary>
        /// <value>The delivery note pos list.</value>
        [ACPropertyList(605, "PickingPos")]
        public IEnumerable<PickingPos> PickingPosList
        {
            get
            {
                if (CurrentPicking == null)
                    return null;
                //return AccessPickingPos.NavList;
                return CurrentPicking.PickingPos_Picking.OrderBy(c => c.Sequence);
                //return DatabaseApp.PickingPos.Include(c => c.InOrderPos)
                //    .Include(c => c.OutOrderPos)
                //    .Include(c => c.ProdOrderPartslistPos)
                //    .Where(c => c.PickingID == CurrentPicking.PickingID).AutoMergeOption();
            }
        }

        /// <summary>
        /// The _ selected delivery note pos
        /// </summary>
        PickingPos _SelectedPickingPos;
        /// <summary>
        /// Gets or sets the selected delivery note pos.
        /// </summary>
        /// <value>The selected delivery note pos.</value>
        [ACPropertySelected(606, "PickingPos")]
        public PickingPos SelectedPickingPos
        {
            get
            {
                return _SelectedPickingPos;
            }
            set
            {
                bool changed = _SelectedPickingPos != value;
                _SelectedPickingPos = value;
                if (changed)
                {
                    OnPropertyChanged(nameof(SelectedPickingPos));
                    RefreshWeighingList(true);
                }

                CurrentPickingPos = value;
            }
        }


        #region Properties -> InOrderPos -> MDUnit

        /// <summary>
        /// Gets the MU quantity unit list.
        /// </summary>
        /// <value>The MU quantity unit list.</value>
        [ACPropertyList(603, MDUnit.ClassName)]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                if (CurrentPickingPos == null)
                    return null;
                if (CurrentPickingPos.Material != null)
                    return CurrentPickingPos.Material.MDUnitList;
                if (CurrentInOrderPos != null && CurrentInOrderPos.Material != null)
                    return CurrentInOrderPos.Material.MDUnitList;
                if (CurrentOutOrderPos != null && CurrentOutOrderPos.Material != null)
                    return CurrentOutOrderPos.Material.MDUnitList;
                if (CurrentProdOrderPartslistPos != null && CurrentProdOrderPartslistPos.Material != null)
                    return CurrentProdOrderPartslistPos.Material.MDUnitList;
                return null;
            }
        }

        MDUnit _CurrentMDUnit;
        /// <summary>
        /// Gets or sets the current MU quantity unit.
        /// </summary>
        /// <value>The current MU quantity unit.</value>
        [ACPropertyCurrent(604, MDUnit.ClassName, "en{'New Unit'}de{'Neue Einheit'}")]
        public MDUnit CurrentMDUnit
        {
            get
            {
                return _CurrentMDUnit;
            }
            set
            {
                _CurrentMDUnit = value;
                if (CurrentPickingPos != null && CurrentPickingPos.MDUnit != value)
                {
                    CurrentPickingPos.MDUnit = value;
                    OnPropertyChanged(nameof(CurrentPickingPos));
                }
                OnPropertyChanged(nameof(CurrentMDUnit));
            }
        }

        #endregion
        #endregion

        #region FacilityPreBooking

        public FacilitySelectLoctation FacilitySelectLoctation { get; set; }


        #region FacilityPreBooking -> PreBooking
        FacilityPreBooking _CurrentFacilityPreBooking;
        [ACPropertyCurrent(607, "FacilityPreBooking")]
        public FacilityPreBooking CurrentFacilityPreBooking
        {
            get
            {
                return _CurrentFacilityPreBooking;
            }
            set
            {
                if (_CurrentFacilityPreBooking != value)
                {
                    if (_CurrentFacilityPreBooking != null && _CurrentFacilityPreBooking.ACMethodBooking != null)
                        _CurrentFacilityPreBooking.ACMethodBooking.PropertyChanged -= ACMethodBooking_PropertyChanged;

                    _CurrentFacilityPreBooking = value;
                    OnPropertyChanged(nameof(CurrentFacilityPreBooking));
                    OnPropertyChanged(nameof(CurrentACMethodBooking));
                    OnPropertyChanged(nameof(CurrentACMethodBookingLayout));
                    //OnPropertyChanged(nameof(BookableFacilityLots));
                    RefreshBookingFilterFacilityAccess(AccessBookingFacility, BookingFilterMaterial);
                    RefreshBookingFilterFacilityAccess(AccessBookingFacilityTarget, BookingFilterMaterialTarget);
                    if (AccessBookingFacilityLot != null)
                        RefreshFilterFacilityLotAccess(_AccessBookingFacilityLot);

                    if (_CurrentFacilityPreBooking != null && _CurrentFacilityPreBooking.ACMethodBooking != null)
                        _CurrentFacilityPreBooking.ACMethodBooking.PropertyChanged += ACMethodBooking_PropertyChanged;

                    OnPropertyChanged(nameof(BookingFacilityList));
                    OnPropertyChanged(nameof(BookingFacilityListTarget));
                }
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(608, "FacilityPreBooking")]
        public IEnumerable<FacilityPreBooking> FacilityPreBookingList
        {
            get
            {
                if (CurrentPickingPos == null || CurrentPicking.MDPickingType == null)
                    return null;
                if ((CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.Receipt
                  || CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.ReceiptVehicle)
                    && CurrentPickingPos.InOrderPos != null)
                {
                    return CurrentPickingPos.InOrderPos.FacilityPreBooking_InOrderPos.ToList();
                }
                else if ((CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.Issue
                       || CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.IssueVehicle)
                    && (CurrentPickingPos.OutOrderPos != null))
                {
                    return CurrentPickingPos.OutOrderPos.FacilityPreBooking_OutOrderPos.ToList();
                }
                else
                {
                    return CurrentPickingPos.FacilityPreBooking_PickingPos.ToList();
                    //if (CurrentPickingPos.ProdOrderPartslistPos == null)
                    //    return null;
                    //return CurrentPickingPos.ProdOrderPartslistPos.FacilityPreBooking_ProdOrderPartslistPos.ToList();
                }
                //return null;
            }
        }

        FacilityPreBooking _SelectedFacilityPreBooking;
        [ACPropertySelected(609, "FacilityPreBooking")]
        public FacilityPreBooking SelectedFacilityPreBooking
        {
            get
            {
                return _SelectedFacilityPreBooking;
            }
            set
            {
                _SelectedFacilityPreBooking = value;
                OnPropertyChanged(nameof(SelectedFacilityPreBooking));
                CurrentFacilityPreBooking = value;
                OnPropertyChanged(nameof(BookingFacilityList));
                OnPropertyChanged(nameof(OutwardFacilityChargeList));
                OnPropertyChanged(nameof(InwardFacilityChargeList));
                //OnPropertyChanged(nameof(BookableFacilityLots));
                if (AccessBookingFacilityLot != null)
                    AccessBookingFacilityLot.NavSearch(DatabaseApp);
            }
        }

        ACMethodBooking _CurrentACMethodBookingDummy = null; // Dummy-Parameter notwendig, damit Bindung an Oberfläche erfolgen kann, da abgeleitete Klasse
        [ACPropertyInfo(610, "", "en{'Posting Parameter'}de{'Buchungsparameter'}")]
        public ACMethodBooking CurrentACMethodBooking
        {
            get
            {
                if (CurrentFacilityPreBooking == null)
                {
                    if (_CurrentACMethodBookingDummy != null)
                        return _CurrentACMethodBookingDummy;
                    if (this.PickingManager == null)
                        return null;
                    ACMethodBooking acMethodClone = ACFacilityManager.BookParamInOrderPosInwardMovementClone(DatabaseApp);
                    if (acMethodClone != null)
                        _CurrentACMethodBookingDummy = acMethodClone.Clone() as ACMethodBooking;
                    return _CurrentACMethodBookingDummy;
                }
                _CurrentACMethodBookingDummy = null;
                return CurrentFacilityPreBooking?.ACMethodBooking as ACMethodBooking;
            }
            set
            {
                if (CurrentFacilityPreBooking != null)
                {
                    if (CurrentFacilityPreBooking.ACMethodBooking != null)
                        CurrentFacilityPreBooking.ACMethodBooking.PropertyChanged -= ACMethodBooking_PropertyChanged;
                    CurrentFacilityPreBooking.ACMethodBooking = value;
                    if (CurrentFacilityPreBooking.ACMethodBooking != null)
                        CurrentFacilityPreBooking.ACMethodBooking.PropertyChanged += ACMethodBooking_PropertyChanged; ;
                }
                else
                    _CurrentACMethodBookingDummy = null;
                OnPropertyChanged(nameof(CurrentACMethodBooking));
                OnPropertyChanged(nameof(CurrentACMethodBookingLayout));
            }
        }

        bool _UpdatingControlModeBooking = false;
        private void ACMethodBooking_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_UpdatingControlModeBooking)
                return;
            try
            {
                if (e.PropertyName == nameof(ACMethodBooking.OutwardFacility))
                {
                    _UpdatingControlModeBooking = true;
                    OnPropertyChanged(nameof(OutwardFacilityChargeList));
                    OnPropertyChanged(nameof(BookableFacilityLots));
                    if (CurrentACMethodBooking != null)
                    {
                        CurrentACMethodBooking.OnEntityPropertyChanged(nameof(ACMethodBooking.OutwardFacility));
                        CurrentACMethodBooking.OnEntityPropertyChanged(nameof(ACMethodBooking.OutwardFacilityCharge));
                        CurrentACMethodBooking.OnEntityPropertyChanged(nameof(ACMethodBooking.OutwardFacilityLot));
                    }
                }
                else if (e.PropertyName == nameof(ACMethodBooking.InwardFacility))
                {
                    _UpdatingControlModeBooking = true;
                    OnPropertyChanged(nameof(InwardFacilityChargeList));
                    OnPropertyChanged(nameof(BookableFacilityLots));
                    if (CurrentACMethodBooking != null)
                    {
                        CurrentACMethodBooking.OnEntityPropertyChanged(nameof(ACMethodBooking.InwardFacility));
                        CurrentACMethodBooking.OnEntityPropertyChanged(nameof(ACMethodBooking.InwardFacilityCharge));
                        CurrentACMethodBooking.OnEntityPropertyChanged(nameof(ACMethodBooking.InwardFacilityLot));
                    }
                }
            }
            finally
            {
                _UpdatingControlModeBooking = false;
            }
        }

        [ACPropertyInfo(611, "")]
        public string CurrentACMethodBookingLayout
        {
            get
            {
                gip.core.datamodel.ACClassDesign acClassDesign = null;
                if (ACType != null)
                {
                    if ((CurrentACMethodBooking != null)
                        && (CurrentACMethodBooking.InOrderPos != null
                            || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosInwardMovement
                            || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel
                            || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.PickingInward
                            ))
                        acClassDesign = ACType.GetDesign(this, Global.ACUsages.DULayout, Global.ACKinds.DSDesignLayout, "BookingInward");
                    else if ((CurrentACMethodBooking != null)
                        && (CurrentACMethodBooking.OutOrderPos != null
                        || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.OutOrderPosOutwardMovement
                        || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel
                        || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.PickingOutward
                        ))
                        acClassDesign = ACType.GetDesign(this, Global.ACUsages.DULayout, Global.ACKinds.DSDesignLayout, "BookingOutward");
                    else
                        acClassDesign = ACType.GetDesign(this, Global.ACUsages.DULayout, Global.ACKinds.DSDesignLayout, "BookingRelocation");
                }

                string layoutXAML = "<vb:VBDockPanel></vb:VBDockPanel>";
                if (acClassDesign != null)
                    layoutXAML = acClassDesign.XMLDesign;

                return layoutXAML;
            }
        }

        private bool _BookingFilterMaterial = true;
        [ACPropertyInfo(9999, "", "en{'Only show source bins with material'}de{'Zeige Quell-Lagerplätze mit Material'}")]
        public bool BookingFilterMaterial
        {
            get
            {
                return _BookingFilterMaterial;
            }
            set
            {
                _BookingFilterMaterial = value;
                OnPropertyChanged(nameof(BookingFilterMaterial));
                RefreshBookingFilterFacilityAccess(AccessBookingFacility, value);
                OnPropertyChanged(nameof(BookingFacilityList));
                OnPropertyChanged(nameof(OutwardFacilityChargeList));
                OnPropertyChanged(nameof(InwardFacilityChargeList));
                //OnPropertyChanged(nameof(BookableFacilityLots));
            }
        }

        [ACPropertyList(612, "OutwardFacilityCharge")]
        public IEnumerable<FacilityCharge> OutwardFacilityChargeList
        {
            get
            {
                if (CurrentACMethodBooking == null || CurrentACMethodBooking.OutwardFacility == null || CurrentPickingPos.Material == null)
                    return null;
                return CurrentACMethodBooking.OutwardFacility.FacilityCharge_Facility
                    .Where(x => x.MaterialID == CurrentPickingPos.Material.MaterialID && !x.NotAvailable).OrderByDescending(x => x.InsertDate);
            }
        }


        [ACPropertyList(612, "InwardFacilityCharge")]
        public IEnumerable<FacilityCharge> InwardFacilityChargeList
        {
            get
            {
                if (CurrentACMethodBooking == null || CurrentACMethodBooking.InwardFacility == null || CurrentPickingPos == null || CurrentPickingPos.Material == null)
                    return null;
                return CurrentACMethodBooking.InwardFacility.FacilityCharge_Facility
                    .Where(x => x.MaterialID == CurrentPickingPos.Material.MaterialID && !x.NotAvailable).OrderByDescending(x => x.InsertDate);
            }
        }

        ACAccessNav<Facility> _AccessBookingFacility;
        [ACPropertyAccess(613, "BookingFacility")]
        public ACAccessNav<Facility> AccessBookingFacility
        {
            get
            {
                if (_AccessBookingFacility == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "BookingFacility", ACType.ACIdentifier);
                    _AccessBookingFacility = navACQueryDefinition.NewAccessNav<Facility>("BookingFacility", this);
                    _AccessBookingFacility.AutoSaveOnNavigation = false;
                    RefreshBookingFilterFacilityAccess(_AccessBookingFacility, BookingFilterMaterial);
                }
                return _AccessBookingFacility;
            }
        }

        private bool _BookingFilterMaterialTarget = true;
        [ACPropertyInfo(614, "", "en{'Only show target bins with material'}de{'Zeige Ziel-Lagerplätze mit Material'}")]
        public bool BookingFilterMaterialTarget
        {
            get
            {
                return _BookingFilterMaterialTarget;
            }
            set
            {
                _BookingFilterMaterialTarget = value;
                OnPropertyChanged(nameof(BookingFilterMaterialTarget));
                RefreshBookingFilterFacilityAccess(AccessBookingFacilityTarget, value);
                OnPropertyChanged(nameof(BookingFacilityListTarget));
            }
        }

        ACAccessNav<Facility> _AccessBookingFacilityTarget;
        [ACPropertyAccess(615, "BookingFacilityTarget")]
        public ACAccessNav<Facility> AccessBookingFacilityTarget
        {
            get
            {
                if (_AccessBookingFacilityTarget == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "BookingFacility", ACType.ACIdentifier);
                    _AccessBookingFacilityTarget = navACQueryDefinition.NewAccessNav<Facility>("BookingFacility", this);
                    _AccessBookingFacilityTarget.AutoSaveOnNavigation = false;
                    RefreshBookingFilterFacilityAccess(_AccessBookingFacilityTarget, BookingFilterMaterialTarget);
                }
                return _AccessBookingFacilityTarget;
            }
        }


        private List<ACFilterItem> AccessBookingFacilityDefaultFilter_Material
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)FacilityTypesEnum.StorageBinContainer).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, "Material\\MaterialNo", Global.LogicalOperators.equal, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, "Material\\MaterialName1", Global.LogicalOperators.equal, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true)
                };
            }
        }

        private List<ACFilterItem> AccessBookingFacilityDefaultFilter_StorageBin
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityNo", Global.LogicalOperators.equal, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityName", Global.LogicalOperators.contains, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)FacilityTypesEnum.StorageBin).ToString(), true)
                };
            }
        }

        private List<ACSortItem> AccessBookingFacilityDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("FacilityNo", Global.SortDirections.ascending, true),
                };
            }
        }

        [ACPropertyList(616, "BookingFacility")]
        public IList<Facility> BookingFacilityList
        {
            get
            {
                return AccessBookingFacility?.NavList;
            }
        }

        [ACPropertyList(617, "BookingFacilityTarget")]
        public IList<Facility> BookingFacilityListTarget
        {
            get
            {
                return AccessBookingFacilityTarget?.NavList;
            }
        }

        private void RefreshBookingFilterFacilityAccess(ACAccessNav<Facility> accessNavFacility, bool bookingFilterMaterial)
        {
            if (accessNavFacility == null
                || accessNavFacility.NavACQueryDefinition == null
                || CurrentFacilityPreBooking == null
                || FacilityPreBookingList == null
                || !FacilityPreBookingList.Any())
                return;
            RefreshFilterFacilityAccess(accessNavFacility, bookingFilterMaterial);
        }

        private void RefreshFilterFacilityAccess(ACAccessNav<Facility> accessNavFacility, bool filterMaterial)
        {
            if (filterMaterial)
            {
                accessNavFacility.NavACQueryDefinition.CheckAndReplaceColumnsIfDifferent(AccessBookingFacilityDefaultFilter_Material, AccessBookingFacilityDefaultSort);
                var acFilter = accessNavFacility.NavACQueryDefinition.ACFilterColumns.Where(c => c.ACIdentifier == "Material\\MaterialNo").FirstOrDefault();

                if (CurrentPickingPos != null && CurrentPickingPos.Material != null)
                {
                    if (acFilter != null)
                        acFilter.SearchWord = CurrentPickingPos.Material.MaterialNo;
                    var acFilter2 = accessNavFacility.NavACQueryDefinition.ACFilterColumns.Where(c => c.ACIdentifier == "Material\\MaterialNo" && c != acFilter).FirstOrDefault();
                    if (acFilter2 != null)
                        acFilter2.SearchWord = CurrentPickingPos.Material.Material1_ProductionMaterial != null ? CurrentPickingPos.Material.Material1_ProductionMaterial.MaterialNo : "";
                }
            }
            else
            {
                accessNavFacility.NavACQueryDefinition.CheckAndReplaceColumnsIfDifferent(AccessBookingFacilityDefaultFilter_StorageBin, AccessBookingFacilityDefaultSort);
            }
            accessNavFacility.NavSearch(this.DatabaseApp);
        }


        [ACPropertyList(618, "FacilityLots")]
        public IEnumerable<FacilityLot> BookableFacilityLots
        {
            get
            {
                if (AccessBookingFacilityLot == null)
                    return null;

                return AccessBookingFacilityLot.NavList;
            }
        }

        ACAccessNav<FacilityLot> _AccessBookingFacilityLot;
        [ACPropertyAccess(613, "FacilityLots")]
        public ACAccessNav<FacilityLot> AccessBookingFacilityLot
        {
            get
            {
                if (_AccessBookingFacilityLot == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "BookingFacilityLot", ACType.ACIdentifier);
                    navACQueryDefinition.TakeCount = 20;
                    _AccessBookingFacilityLot = navACQueryDefinition.NewAccessNav<FacilityLot>("BookingFacilityLot", this);
                    _AccessBookingFacilityLot.AutoSaveOnNavigation = false;
                    _AccessBookingFacilityLot.NavSearchExecuted += _AccessBookingFacilityLot_NavSearchExecuted;
                    RefreshFilterFacilityLotAccess(_AccessBookingFacilityLot);
                }
                return _AccessBookingFacilityLot;
            }
        }

        private void _AccessBookingFacilityLot_NavSearchExecuted(object sender, IList<FacilityLot> result)
        {
            List<FacilityLot> bookableFacilityLots = null;
            if (FacilityPreBookingList != null && FacilityPreBookingList.Any())
            {
                if (FacilityPreBookingList.Where(c => c.InwardFacilityLot != null).Any())
                    bookableFacilityLots = FacilityPreBookingList.Where(c => c.InwardFacilityLot != null).Select(c => c.InwardFacilityLot).Distinct().ToList();
                else
                    bookableFacilityLots = FacilityPreBookingList.Where(c => c.OutwardFacilityLot != null).Select(c => c.OutwardFacilityLot).Distinct().ToList();
            }

            if (FacilityBookingList != null && FacilityBookingList.Any())
            {
                IEnumerable<FacilityLot> query2 = null;
                if (FacilityBookingList.Where(c => c.InwardFacilityLot != null).Any())
                    query2 = FacilityBookingList.Where(c => c.InwardFacilityLot != null).Select(c => c.InwardFacilityLot).Distinct();
                else
                    query2 = FacilityBookingList.Where(c => c.OutwardFacilityLot != null).Select(c => c.OutwardFacilityLot).Distinct();
                if (bookableFacilityLots == null)
                    bookableFacilityLots = query2.ToList();
                else
                {
                    var query3 = bookableFacilityLots.Union(query2);
                    if ((query3 != null) && (query3.Any()))
                        bookableFacilityLots = query3.ToList();
                }
            }

            if (bookableFacilityLots != null && bookableFacilityLots.Any())
            {
                foreach (var lot in bookableFacilityLots)
                {
                    result.Insert(0, lot);
                }
            }
        }

        private List<ACFilterItem> AccessBookingFacilityLotDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "LotNo", Global.LogicalOperators.equal, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, "Material\\MaterialNo", Global.LogicalOperators.equal, Global.Operators.or, "", true),
                };
            }
        }

        private List<ACSortItem> AccessBookingFacilityLotDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("LotNo", Global.SortDirections.ascending, true),
                };
            }
        }


        private void RefreshFilterFacilityLotAccess(ACAccessNav<FacilityLot> accessNavLot)
        {
            if (accessNavLot == null
                || accessNavLot.NavACQueryDefinition == null
                || CurrentPickingPos == null)
                return;
            accessNavLot.NavACQueryDefinition.CheckAndReplaceColumnsIfDifferent(AccessBookingFacilityLotDefaultFilter, AccessBookingFacilityLotDefaultSort);

            var acFilter = accessNavLot.NavACQueryDefinition.ACFilterColumns.Where(c => c.ACIdentifier == "Material\\MaterialNo").FirstOrDefault();
            if (acFilter != null && CurrentPickingPos.Material != null)
                acFilter.SearchWord = CurrentPickingPos.Material.MaterialNo;

            accessNavLot.NavSearch(this.DatabaseApp);
        }
        #endregion

        #region  FacilityPreBooking -> Available quants

        private bool _IsInward;
        private Material _QuantDialogMaterial;


        private FacilityCharge _SelectedPreBookingAvailableQuants;
        /// <summary>
        /// Selected property for FacilityCharge
        /// </summary>
        /// <value>The selected PreBookingAvailableQuants</value>
        [ACPropertySelected(500, "PropertyGroupName", "en{'TODO: PreBookingAvailableQuants'}de{'TODO: PreBookingAvailableQuants'}")]
        public FacilityCharge SelectedPreBookingAvailableQuants
        {
            get
            {
                return _SelectedPreBookingAvailableQuants;
            }
            set
            {
                if (_SelectedPreBookingAvailableQuants != value)
                {
                    _SelectedPreBookingAvailableQuants = value;
                    OnPropertyChanged(nameof(SelectedPreBookingAvailableQuants));
                }
            }
        }


        private List<FacilityCharge> _PreBookingAvailableQuantsList;
        /// <summary>
        /// List property for FacilityCharge
        /// </summary>
        /// <value>The PreBookingAvailableQuants list</value>
        [ACPropertyList(501, "PropertyGroupName")]
        public List<FacilityCharge> PreBookingAvailableQuantsList
        {
            get
            {
                if (_PreBookingAvailableQuantsList == null)
                    _PreBookingAvailableQuantsList = LoadPreBookingAvailableQuantsList();
                return _PreBookingAvailableQuantsList;
            }
        }

        private List<FacilityCharge> LoadPreBookingAvailableQuantsList()
        {
            if (_QuantDialogMaterial == null)
                return new List<FacilityCharge>();
            return
                DatabaseApp.FacilityCharge
                .Include(c => c.FacilityLot)
                .Include(c => c.Material)
                .Include(c => c.MDUnit)
                .Include(c => c.Facility)
                .Where(c => c.MaterialID == _QuantDialogMaterial.MaterialID && !c.NotAvailable)
                .OrderBy(c => c.ExpirationDate)
                .ThenBy(c => c.FillingDate)
                .Take(ACQueryDefinition.C_DefaultTakeCount)
                .ToList();
        }

        private void LoadPreBookingAvailableQuants()
        {
            _PreBookingAvailableQuantsList = null;
            OnPropertyChanged(nameof(PreBookingAvailableQuantsList));
        }

        #endregion

        #endregion

        #region FacilityBooking
        FacilityBooking _CurrentFacilityBooking;
        [ACPropertyCurrent(619, FacilityBooking.ClassName)]
        public FacilityBooking CurrentFacilityBooking
        {
            get
            {
                return _CurrentFacilityBooking;
            }
            set
            {
                _CurrentFacilityBooking = value;
                OnPropertyChanged(nameof(CurrentFacilityBooking));
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(620, FacilityBooking.ClassName)]
        public IEnumerable<FacilityBooking> FacilityBookingList
        {
            get
            {
                if (CurrentPickingPos == null
                    || CurrentPicking == null
                    || CurrentPickingPos.EntityState == EntityState.Added
                    || CurrentPickingPos.EntityState == EntityState.Deleted
                    || CurrentPickingPos.EntityState == EntityState.Detached)
                    return null;
                if ((CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.Receipt
                  || CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.ReceiptVehicle) && CurrentPickingPos.InOrderPos != null)
                {
                    CurrentPickingPos.InOrderPos.FacilityBooking_InOrderPos.AutoLoad();
                    CurrentPickingPos.InOrderPos.FacilityBooking_InOrderPos.AutoRefresh(this.DatabaseApp);
                    return CurrentPickingPos.InOrderPos.FacilityBooking_InOrderPos.OrderBy(c => c.FacilityBookingNo).ToList();
                }
                else if ((CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.Issue
                       || CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.IssueVehicle) && CurrentPickingPos.OutOrderPos != null)
                {
                    CurrentPickingPos.OutOrderPos.FacilityBooking_OutOrderPos.AutoLoad();
                    CurrentPickingPos.OutOrderPos.FacilityBooking_OutOrderPos.AutoRefresh(this.DatabaseApp);
                    return CurrentPickingPos.OutOrderPos.FacilityBooking_OutOrderPos.OrderBy(c => c.FacilityBookingNo).ToList();
                }
                List<FacilityBooking> bookingList = null;
                if (CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any())
                {
                    foreach (var pickingPosRef in CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos)
                        pickingPosRef.ProdorderPartslistPos.FacilityBooking_ProdOrderPartslistPos.AutoRefresh(this.DatabaseApp);
                    bookingList = CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.SelectMany(c => c.ProdorderPartslistPos.FacilityBooking_ProdOrderPartslistPos).OrderBy(c => c.FacilityBookingNo).ToList();
                }

                if (bookingList == null || !bookingList.Any())
                {
                    CurrentPickingPos.FacilityBooking_PickingPos.AutoLoad();
                    CurrentPickingPos.FacilityBooking_PickingPos.AutoRefresh(this.DatabaseApp);
                    bookingList = CurrentPickingPos.FacilityBooking_PickingPos.OrderBy(c => c.FacilityBookingNo).ToList();
                }
                return bookingList;
            }
        }

        FacilityBooking _SelectedFacilityBooking;
        [ACPropertySelected(621, FacilityBooking.ClassName)]
        public FacilityBooking SelectedFacilityBooking
        {
            get
            {
                return _SelectedFacilityBooking;
            }
            set
            {
                _SelectedFacilityBooking = value;
                OnPropertyChanged(nameof(SelectedFacilityBooking));
                CurrentFacilityBooking = value;
                OnPropertyChanged(nameof(FacilityBookingChargeList));
            }
        }
        #endregion

        #region FacilityBookingCharge

        [ACPropertyList(623, "FacilityBookingCharge")]
        public IEnumerable<FacilityBookingCharge> FacilityBookingChargeList
        {
            get
            {
                if (CurrentFacilityBooking != null)
                {
                    CurrentFacilityBooking.FacilityBookingCharge_FacilityBooking.AutoRefresh(this.DatabaseApp);
                    return CurrentFacilityBooking.FacilityBookingCharge_FacilityBooking.OrderBy(c => c.FacilityBookingChargeNo).ToList();
                }
                return null;
            }
        }

        private FacilityBookingCharge _SelectedFacilityBookingCharge;
        [ACPropertySelected(624, "FacilityBookingCharge")]
        public FacilityBookingCharge SelectedFacilityBookingCharge
        {
            get
            {
                return _SelectedFacilityBookingCharge;
            }
            set
            {
                _SelectedFacilityBookingCharge = value;
            }
        }

        #endregion

        #region Local Properties

        protected ACPropertyConfigValue<bool> _ForwardToRemoteStores;
        [ACPropertyConfig("en{'Forward to remote stores'}de{'An entfernte Läger weiterleiten'}")]
        public bool ForwardToRemoteStores
        {
            get
            {
                return _ForwardToRemoteStores.ValueT;
            }
            set
            {
                _ForwardToRemoteStores.ValueT = value;
            }
        }


        protected ACRef<ACInDeliveryNoteManager> _InDeliveryNoteManager = null;
        public ACInDeliveryNoteManager InDeliveryNoteManager
        {
            get
            {
                if (_InDeliveryNoteManager == null)
                    return null;
                return _InDeliveryNoteManager.ValueT;
            }
        }

        protected ACRef<ACOutDeliveryNoteManager> _OutDeliveryNoteManager = null;
        public ACOutDeliveryNoteManager OutDeliveryNoteManager
        {
            get
            {
                if (_OutDeliveryNoteManager == null)
                    return null;
                return _OutDeliveryNoteManager.ValueT;
            }
        }

        protected ACRef<ACPickingManager> _PickingManager = null;
        public ACPickingManager PickingManager
        {
            get
            {
                if (_PickingManager == null)
                    return null;
                return _PickingManager.ValueT;
            }
        }

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

        protected ACMethodBooking BookParamInwardMovementClone
        {
            get
            {
                return ACFacilityManager.BookParamInOrderPosInwardMovementClone(DatabaseApp);
            }
        }

        protected ACMethodBooking BookParamOutwardMovementClone
        {
            get
            {
                return ACFacilityManager.BookParamOutOrderPosOutwardMovementClone(DatabaseApp);
            }
        }

        protected ACMethodBooking BookParamInCancelClone
        {
            get
            {
                return ACFacilityManager.BookParamInCancelClone(DatabaseApp);
            }
        }

        protected ACMethodBooking BookParamOutCancelClone
        {
            get
            {
                return ACFacilityManager.BookParamOutCancelClone(DatabaseApp);
            }
        }

        MDDelivPosState _StateCompletelyAssigned = null;
        MDDelivPosState StateCompletelyAssigned
        {
            get
            {
                if (_StateCompletelyAssigned != null)
                    return _StateCompletelyAssigned;
                var queryDelivStateAssigned = DatabaseApp.MDDelivPosState.Where(c => c.MDDelivPosStateIndex == (Int16)MDDelivPosState.DelivPosStates.CompletelyAssigned);
                if (queryDelivStateAssigned.Any())
                    _StateCompletelyAssigned = queryDelivStateAssigned.First();
                return _StateCompletelyAssigned;
            }
        }

        protected List<InOrderPos> _UnSavedUnAssignedInOrderPos = new List<InOrderPos>();
        protected List<OutOrderPos> _UnSavedUnAssignedOutOrderPos = new List<OutOrderPos>();
        protected List<ProdOrderPartslistPos> _UnSavedUnAssignedProdOrderPartslistPos = new List<ProdOrderPartslistPos>();

        Nullable<double> _PartialQuantity;
        [ACPropertyInfo(625, "", "en{'Partial Quantity'}de{'Teilmenge'}")]
        public Nullable<double> PartialQuantity
        {
            get
            {
                return _PartialQuantity;
            }
            set
            {
                _PartialQuantity = value;
                OnPropertyChanged(nameof(PartialQuantity));
            }
        }

        private string _BookingFacilityLotNo;
        public string BookingFacilityLotNo
        {
            get
            {
                return _BookingFacilityLotNo;
            }
            set
            {
                _BookingFacilityLotNo = value;
                OnPropertyChanged(nameof(BookingFacilityLotNo));
            }
        }

        #endregion

        #region Message

        private MsgWithDetails _BSOMsg = new MsgWithDetails();
        [ACPropertyInfo(626, "Message")]
        public MsgWithDetails BSOMsg
        {
            get
            {
                return _BSOMsg;
            }
            set
            {
                _BSOMsg = value;
                OnPropertyChanged(nameof(BSOMsg));
            }
        }

        #endregion

        #region Forward
        private class ForwardToMirroredStore
        {
            public ForwardToMirroredStore(Facility store)
            {
                MirroredStore = store;
                Pickings = new List<Picking>();
            }
            public Facility MirroredStore { get; private set; }
            public List<Picking> Pickings { get; private set; }
        }
        private List<ForwardToMirroredStore> _ForwardPlan;
        private List<Picking> _VisitedPickings = null;
        #endregion

        #endregion

        #region BSO->ACMethod

        #region ControlMode
        /// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        /// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        /// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        /// <returns>ControlModesInfo</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            switch (vbControl.VBContent)
            {
                case "CurrentACMethodBooking\\InwardFacilityLot":
                    {
                        if (CurrentACMethodBooking != null && CurrentPickingPos != null)
                        {
                            if (CurrentPickingPos.Material == null
                                || !CurrentPickingPos.Material.IsLotManaged)
                                return Global.ControlModes.Disabled;
                        }
                        break;
                    }
                case "CurrentPickingPos\\PickingMaterial":
                    if (CurrentPickingPos == null
                       || CurrentPickingPos.InOrderPosID.HasValue
                       || CurrentPickingPos.OutOrderPosID.HasValue
                       || CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any()
                       || CurrentPickingPos.FacilityReservation_PickingPos.Any())
                        return Global.ControlModes.Disabled;
                    break;
                case "CurrentPickingPos\\PickingQuantityUOM":
                case "CurrentPickingPos\\TargetQuantity":
                case "CurrentPickingPos\\TargetQuantityUOM":
                    if (CurrentPickingPos == null
                        || CurrentPickingPos.InOrderPosID.HasValue
                        || CurrentPickingPos.OutOrderPosID.HasValue
                        || CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any())
                        return Global.ControlModes.Disabled;
                    break;
            }

            if (!String.IsNullOrEmpty(vbControl.VBContent) && vbControl.VBContent.StartsWith("CurrentACMethodBooking"))
            {
                if (CurrentACMethodBooking == null)
                    return Global.ControlModes.Disabled;
                int pos = vbControl.VBContent.IndexOf('\\');
                if (pos > 0)
                {
                    string methodId = vbControl.VBContent.Substring(0, pos);
                    string acValueId = vbControl.VBContent.Substring(pos + 1);
                    if (!String.IsNullOrEmpty(methodId) && !String.IsNullOrEmpty(acValueId))
                    {
                        ACMethodBooking acMethod = ACUrlCommand(methodId) as ACMethodBooking;
                        if (acMethod != null)
                        {
                            Global.ControlModesInfo subResult = acMethod.OnGetControlModes(vbControl, acValueId);
                            if (subResult.Mode != result)
                            {
                                result = subResult.Mode;
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        #region Picking
        public virtual void RebuildAccessPrimary()
        {
            if (_AccessPrimary == null || _AccessPrimary.NavACQueryDefinition == null || ACType == null)
                return;
            String checkedOut = ((short)MDVisitorVoucherState.VisitorVoucherStates.CheckedIn).ToString();
            _AccessPrimary.NavACQueryDefinition.ClearFilter(true);
            short filterDelivType = (short)GlobalApp.PickingType.Receipt;
            _AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "PickingNo", Global.LogicalOperators.contains, Global.Operators.and, "", true));
            _AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "PickingTypeIndex", Global.LogicalOperators.greaterThanOrEqual, Global.Operators.and, filterDelivType.ToString(), true));
        }

        /// <summary>
        /// Its invoked from a WPF-Itemscontrol that wants to refresh its CollectionView because the user has changed the LINQ-Expressiontree in the ACQueryDefinition-Property of IAccess. 
        /// The BSO should execute the query on the database first, to get the new results for refreshing the CollectionView of the control.
        /// If the bso don't want to handle this request or manipulate the ACQueryDefinition it returns false. The WPF-control invokes then the IAccess.NavSearch()-Method itself.  
        /// </summary>
        /// <param name="acAccess">Reference to IAccess that contains the changed query (Property NavACQueryDefinition)</param>
        /// <returns>True if the bso has handled this request and queried the database context. Otherwise it returns false.</returns>
        public override bool ExecuteNavSearch(IAccess acAccess)
        {
            if (acAccess == _AccessBookingFacility)
            {
                _AccessBookingFacility.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(BookingFacilityList));
                return true;
            }
            else if (acAccess == _AccessBookingFacilityTarget)
            {
                _AccessBookingFacility.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(BookingFacilityListTarget));
                return true;
            }
            else if (acAccess == _AccessPickingPos)
            {
                _AccessPickingPos.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(PickingPosList));
                return true;
            }
            else if (acAccess == _AccessInOrderPos)
            {
                _AccessInOrderPos.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(InOrderPosList));
                return true;
            }
            else if (acAccess == _AccessOutOrderPos)
            {
                _AccessOutOrderPos.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(OutOrderPosList));
                return true;
            }
            else if (acAccess == _AccessProdOrderPartslistPos)
            {
                _AccessProdOrderPartslistPos.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(ProdOrderPartslistPosList));
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }


        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("Picking", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        protected override Msg OnPreSave()
        {
            if (this.PickingManager != null)
            {
                this.PickingManager.ValidateOnSave(DatabaseApp, CurrentPicking);
            }

            _ForwardPlan = null;
            if (ForwardToRemoteStores
                && _VisitedPickings != null
                && _VisitedPickings.Any())
            {
                _ForwardPlan = new List<ForwardToMirroredStore>();
                IList<PickingPos> changedPositionsInThisBSO = DatabaseApp.GetChangedEntities<PickingPos>(c => c.Picking != null && _VisitedPickings.Contains(c.Picking));
                foreach (PickingPos changedPos in changedPositionsInThisBSO)
                {
                    if (changedPos.FromFacility != null && changedPos.FromFacility.IsMirroredOnMoreDatabases)
                    {
                        BroadcastFacility(changedPos.FromFacility, changedPos.Picking);
                    }
                    if (changedPos.ToFacility != null && changedPos.ToFacility.IsMirroredOnMoreDatabases)
                    {
                        BroadcastFacility(changedPos.ToFacility, changedPos.Picking);
                    }
                }

                // scenario for delete
                IList<PickingPos> deletedItems = DatabaseApp.GetDeletedEntities<PickingPos>(null);
                if (deletedItems.Any())
                {
                    foreach (PickingPos deletedItem in deletedItems)
                    {
                        DbDataRecord originalItem = DatabaseApp.GetOriginalValues(deletedItem.EntityKey);
                        Guid pickingID = (Guid)originalItem["PickingID"];
                        Picking picking = DatabaseApp.Picking.FirstOrDefault(c => c.PickingID == pickingID);
                        if (!string.IsNullOrEmpty(originalItem["FromFacilityID"].ToString()))
                        {
                            Guid fromFacilityID = (Guid)originalItem["FromFacilityID"];
                            Facility facility = DatabaseApp.Facility.FirstOrDefault(c => c.FacilityID == fromFacilityID);
                            if (facility.IsMirroredOnMoreDatabases)
                                BroadcastFacility(facility, picking);
                        }
                        if (!string.IsNullOrEmpty(originalItem["ToFacilityID"].ToString()))
                        {
                            Guid fromFacilityID = (Guid)originalItem["ToFacilityID"];
                            Facility facility = DatabaseApp.Facility.FirstOrDefault(c => c.FacilityID == fromFacilityID);
                            if (facility.IsMirroredOnMoreDatabases)
                                BroadcastFacility(facility, picking);
                        }
                    }
                }
            }
            return base.OnPreSave();
        }

        private void BroadcastFacility(Facility facility, Picking picking)
        {
            ForwardToMirroredStore broadcastEntry = _ForwardPlan.Where(c => c.MirroredStore == facility).FirstOrDefault();
            if (broadcastEntry == null)
            {
                broadcastEntry = new ForwardToMirroredStore(facility);
                _ForwardPlan.Add(broadcastEntry);
            }
            if (!broadcastEntry.Pickings.Contains(picking))
                broadcastEntry.Pickings.Add(picking);
        }

        protected override void OnPostSave()
        {
            _UnSavedUnAssignedInOrderPos = new List<InOrderPos>();
            _UnSavedUnAssignedOutOrderPos = new List<OutOrderPos>();
            _UnSavedUnAssignedProdOrderPartslistPos = new List<ProdOrderPartslistPos>();
            if (_ForwardPlan != null && _ForwardPlan.Any())
            {
                foreach (var broadcastEntry in _ForwardPlan)
                {
                    foreach (var picking in broadcastEntry.Pickings)
                    {
                        broadcastEntry.MirroredStore.CallSendPicking(false, picking.PickingID);
                    }
                }
            }
            if (ForwardToRemoteStores
                && CurrentPicking != null)
            {
                _VisitedPickings = new List<Picking>();
                _VisitedPickings.Add(CurrentPicking);
            }
            else
                _VisitedPickings = null;

            _ForwardPlan = null;

            RefreshInOrderPosList(false);
            RefreshOutOrderPosList(false);
            RefreshProdOrderPartslistPosList(false);

            LoadProcessWorkflowPresenter();
        }


        /// <summary>
        /// Determines whether [is enabled save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand("Picking", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        protected override void OnPostUndoSave()
        {
            _UnSavedUnAssignedInOrderPos = new List<InOrderPos>();
            _UnSavedUnAssignedOutOrderPos = new List<OutOrderPos>();
            _UnSavedUnAssignedProdOrderPartslistPos = new List<ProdOrderPartslistPos>();
            if (CurrentPicking.EntityState == EntityState.Modified)
            {
                RefreshInOrderPosList(true);
                RefreshOutOrderPosList(true);
                RefreshProdOrderPartslistPosList(true);
                if (CurrentPicking != null && CurrentPicking.EntityState != System.Data.EntityState.Detached)
                    CurrentPicking.PickingPos_Picking.Load();
                OnPropertyChanged(nameof(PickingPosList));
            }
            else
            {
                Search();
            }
            base.OnPostUndoSave();
        }


        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        private bool _InLoad = false;
        /// <summary>
        /// Loads this instance.
        /// </summary>
        [ACMethodInteraction("Picking", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedPicking", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            _InLoad = true;
            LoadEntity<Picking>(requery, () => SelectedPicking, () => CurrentPicking, c => CurrentPicking = c,
                        DatabaseApp.Picking
                        .Include(c => c.PickingPos_Picking)
                        .Include("PickingPos_Picking.OutOrderPos")
                        .Include("PickingPos_Picking.OutOrderPos.FacilityBooking_OutOrderPos")
                        .Include("PickingPos_Picking.OutOrderPos.FacilityPreBooking_OutOrderPos")
                        .Include("PickingPos_Picking.InOrderPos")
                        .Include("PickingPos_Picking.InOrderPos.FacilityBooking_InOrderPos")
                        .Include("PickingPos_Picking.InOrderPos.FacilityPreBooking_InOrderPos")
                        .Include("PickingPos_Picking.ToFacility")
                        .Include("PickingPos_Picking.FromFacility")
                        .Include("PickingPos_Picking.PickingMaterial")
                        .Include("PickingPos_Picking.MDDelivPosLoadState")
                        //.Include("PickingPos_Picking.ProdOrderPartslistPos")
                        //.Include("PickingPos_Picking.ProdOrderPartslistPos.MDProdOrderPartslistPosState")
                        .Include(c => c.VisitorVoucher)
                        .Include(c => c.MDPickingType)
                        .Where(c => c.PickingID == SelectedPicking.PickingID));

            _InLoad = false;
            if (CurrentPicking != null)
            {
                CurrentPicking.ACProperties.Refresh();
                if (CurrentPicking.VisitorVoucher != null)
                    CurrentPicking.VisitorVoucher.ACProperties.Refresh();

                CurrentPicking.PickingPos_Picking.AutoRefresh();
            }
            PostExecute("Load");
            OnPropertyChanged(nameof(PickingPosList));
            if (PickingPosList != null && PickingPosList.Any())
            {
                _CurrentPickingPos = null;
                CurrentPickingPos = PickingPosList.FirstOrDefault();
                bool refreshWeighing = requery && SelectedPickingPos == CurrentPickingPos;
                SelectedPickingPos = CurrentPickingPos;
                if (refreshWeighing)
                    RefreshWeighingList(true);
            }
            else
            {
                CurrentPickingPos = null;
                SelectedPickingPos = null;
            }
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedPicking != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("Picking", "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedPicking", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New"))
                return;
            if (AccessPrimary == null)
                return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Picking), Picking.NoColumnName, Picking.FormatNewNo, this);
            Picking newPicking = Picking.NewACObject(DatabaseApp, null, secondaryKey);
            newPicking.MDPickingType = DatabaseApp.MDPickingType.FirstOrDefault(c => c.MDPickingTypeIndex == (short)GlobalApp.PickingType.Receipt);
            DatabaseApp.Picking.AddObject(newPicking);
            if (SelectedFilterMDPickingType != null)
                newPicking.MDPickingType = SelectedFilterMDPickingType;
            if (SelectedFilterDeliveryAddress != null)
                newPicking.DeliveryCompanyAddress = SelectedFilterDeliveryAddress;
            CurrentPicking = newPicking;
            if (PickingList == null)
            {
                PickingList = new List<Picking>();
            }
            PickingList.Insert(0, newPicking);
            SelectedPicking = newPicking;
            ACState = Const.SMNew;
            PostExecute("New");
        }

        /// <summary>
        /// Determines whether [is enabled new delivery note].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new delivery note]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNew()
        {
            return true;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction("Picking", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentPicking", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (AccessPrimary == null)
                return;
            Msg msg = PickingManager.UnassignAllPickingPos(CurrentPicking, DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            msg = CurrentPicking.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            AccessPrimary.NavList.Remove(CurrentPicking);
            _PickingList = AccessPrimary.NavList.ToList();
            Picking firstPicking = AccessPrimary.NavList.FirstOrDefault();
            OnPropertyChanged(nameof(PickingList));
            CurrentPicking = firstPicking;
            SelectedPicking = firstPicking;
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete delivery note].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete delivery note]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return true;
        }

        /// <summary>
        /// Searches the delivery note.
        /// </summary>
        [ACMethodCommand("Picking", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public virtual void Search(bool refreshList = true)
        {
            if (AccessPrimary == null)
                return;

            _PickingList = null;
            if (AccessPrimary != null)
            {
                AccessPrimary.NavSearch(DatabaseApp, MergeOption.OverwriteChanges);
                _PickingList = AccessPrimary.NavList.ToList();
            }

            OnPropertyChanged(nameof(PickingList));

            if (refreshList)
            {
                RefreshInOrderPosList();
                RefreshOutOrderPosList();
                RefreshProdOrderPartslistPosList();
            }
        }

        [ACMethodCommand("Picking", "en{'Cancel Picking'}de{'Storniere Kommission'}", (short)MISort.Cancel, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void CancelPicking()
        {
            if (!PreExecute("CancelPicking"))
                return;
            if (!IsEnabledCancelPicking())
                return;
            // "Möchten Sie die Kommission stornieren"
            Msg msgForAll = new Msg
            {
                Source = GetACUrl(),
                MessageLevel = eMsgLevel.Info,
                ACIdentifier = "CancelPicking(0)",
                Message = Root.Environment.TranslateMessage(this, "Question50017")
            };
            Global.MsgResult msgResult = Messages.Msg(msgForAll, Global.MsgResult.No, eMsgButton.YesNo);
            if (msgResult == Global.MsgResult.No)
                return;
            var result = ACFacilityManager.CancelFacilityPreBooking(DatabaseApp, CurrentPicking);
            if (result != null && result.Any())
            {
                foreach (PickingPos pickingPos in PickingPosList.ToList())
                {
                    SelectedPickingPos = pickingPos;
                    if (CurrentPickingPos == pickingPos)
                    {
                        BookAllACMethodBookings();
                    }
                }
            }

            int countCancelled = 0;
            int countAssigned = 0;
            foreach (PickingPos pickingPos in PickingPosList.ToList())
            {
                if (pickingPos.InOrderPos != null || pickingPos.OutOrderPos != null)
                    countAssigned++;
                if ((pickingPos.InOrderPos != null)
                    && (pickingPos.InOrderPos.MDInOrderPosState.InOrderPosState == MDInOrderPosState.InOrderPosStates.Cancelled
                        || !pickingPos.InOrderPos.FacilityBooking_InOrderPos.Any()))
                {
                    countCancelled++;
                }
                else if ((pickingPos.OutOrderPos != null)
                    && (pickingPos.OutOrderPos.MDOutOrderPosState.OutOrderPosState == MDOutOrderPosState.OutOrderPosStates.Cancelled
                        || !pickingPos.OutOrderPos.FacilityBooking_OutOrderPos.Any()))
                {
                    countCancelled++;
                }
            }

            if (countCancelled == countAssigned
                && CurrentPicking.PickingState != PickingStateEnum.Cancelled)
            {
                CurrentPicking.PickingState = PickingStateEnum.Cancelled;
                Save();
            }

            OnPropertyChanged(nameof(PickingList));
            CurrentPicking = CurrentPicking;

            PostExecute("CancelPicking");
        }

        public bool IsEnabledCancelPicking()
        {
            if (CurrentPicking == null)
                return false;
            if (CurrentPicking.PickingState == PickingStateEnum.Cancelled)
                return false;
            if (!PickingPosList.Any())
                return false;
            return true;
        }


        /// <summary>
        /// Source Property: MirrorPicking
        /// </summary>
        [ACMethodInfo("MirrorPicking", "en{'Mirror picking'}de{'Kommissionierung spiegeln'}", 100)]
        public void MirrorPicking()
        {
            if (!IsEnabledMirrorPicking())
                return;
            Picking mirroredPicking = PickingManager.MirrorPicking(DatabaseApp, CurrentPicking);
            AccessPrimary.NavList.Add(mirroredPicking);
            OnPropertyChanged(nameof(PickingList));
            CurrentPicking = mirroredPicking;
            SelectedPicking = mirroredPicking;
        }

        public bool IsEnabledMirrorPicking()
        {
            return CurrentPicking != null && CurrentPicking.PickingPos_Picking.Any();
        }


        [ACMethodInfo("MirrorPicking", "en{'Create pickings for supply'}de{'Erstelle Bereitstellungsaufträge'}", 101)]
        public virtual void GenerateSubPickingsForSupply()
        {
            if (!IsEnabledGenerateSubPickingsForSupply())
                return;
            Picking mirroredPicking = null;
            IEnumerable<Picking> mirroredPickings = PickingManager.CreateSupplyPickings(DatabaseApp, CurrentPicking);
            if (mirroredPickings != null && mirroredPickings.Any())
            {
                foreach (var picking in mirroredPickings)
                {
                    if (mirroredPicking == null)
                        mirroredPicking = picking;
                    AccessPrimary.NavList.Add(picking);
                }
            }
            OnPropertyChanged(nameof(PickingList));
            if (mirroredPicking != null)
            {
                CurrentPicking = mirroredPicking;
                SelectedPicking = mirroredPicking;
            }
        }

        public virtual bool IsEnabledGenerateSubPickingsForSupply()
        {
            return CurrentPicking != null
                && CurrentPicking.PickingState < PickingStateEnum.Finished
                && CurrentPicking.MDPickingType?.PickingType != PickingType.AutomaticRelocation
                && CurrentPicking.MDPickingType?.PickingType != PickingType.InternalRelocation
                && CurrentPicking.MDPickingType?.PickingType != PickingType.Receipt
                && CurrentPicking.MDPickingType?.PickingType != PickingType.ReceiptVehicle;
        }

        #endregion

        #region Picking-Pos
        /// <summary>
        /// Unassigns the in order pos.
        /// </summary>
        [ACMethodCommand("PickingPos", "en{'Remove'}de{'Entfernen'}", 601, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void UnassignPickingPos()
        {
            if (!PreExecute("UnassignPickingPos"))
                return;

            // TODO: Es muss noch implementiert werden, wenn Teilmenge abgerufen worden ist bei den mehrere Kommissionierpositionen entstehen
            if (CurrentPickingPos.InOrderPos != null && CurrentPickingPos.InOrderPos.InOrderPos1_ParentInOrderPos != null)
            {
                InOrderPos parentInOrderPos = CurrentPickingPos.InOrderPos.InOrderPos1_ParentInOrderPos; // 2. Ebene
                parentInOrderPos = parentInOrderPos.InOrderPos1_ParentInOrderPos; // 1. Ebene

                Msg result = null;
                try
                {
                    result = PickingManager.UnassignPickingPos(CurrentPickingPos, this.DatabaseApp);
                    if (result != null)
                    {
                        Messages.Msg(result);
                        return;
                    }
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOPicking", "UnassignPickingPos", msg);
                }

                if (result == null && parentInOrderPos != null)
                {
                    if (!_UnSavedUnAssignedInOrderPos.Contains(parentInOrderPos))
                        _UnSavedUnAssignedInOrderPos.Add(parentInOrderPos);
                }
                RefreshInOrderPosList();
            }
            else if (CurrentPickingPos.OutOrderPos != null && CurrentPickingPos.OutOrderPos.OutOrderPos1_ParentOutOrderPos != null)
            {
                OutOrderPos parentOutOrderPos = CurrentPickingPos.OutOrderPos.OutOrderPos1_ParentOutOrderPos;
                parentOutOrderPos = parentOutOrderPos.OutOrderPos1_ParentOutOrderPos; // 1. Ebene

                Msg result = null;
                try
                {
                    result = PickingManager.UnassignPickingPos(CurrentPickingPos, this.DatabaseApp);
                    if (result != null)
                    {
                        Messages.Msg(result);
                        return;
                    }
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOPicking", "UnassignPickingPos(10)", msg);
                }

                if (result == null && parentOutOrderPos != null)
                {
                    if (!_UnSavedUnAssignedOutOrderPos.Contains(parentOutOrderPos))
                        _UnSavedUnAssignedOutOrderPos.Add(parentOutOrderPos);
                }
                RefreshOutOrderPosList();
            }
            else if (CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any())
            {
                List<ProdOrderPartslistPos> parentProdOrderPartslistPositions =
                    CurrentPickingPos
                    .PickingPosProdOrderPartslistPos_PickingPos
                    .Select(c => c.ProdorderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos).ToList();

                Msg result = null;
                try
                {
                    result = PickingManager.UnassignPickingPos(CurrentPickingPos, this.DatabaseApp);
                    if (result != null)
                    {
                        Messages.Msg(result);
                        return;
                    }
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOPicking", "UnassignPickingPos(20)", msg);
                }

                if (result == null && parentProdOrderPartslistPositions != null && parentProdOrderPartslistPositions.Any())
                {
                    foreach (var parentProdOrderPartslistPos in parentProdOrderPartslistPositions)
                        if (!_UnSavedUnAssignedProdOrderPartslistPos.Contains(parentProdOrderPartslistPos))
                            _UnSavedUnAssignedProdOrderPartslistPos.Add(parentProdOrderPartslistPos);
                }
                RefreshProdOrderPartslistPosList();
            }
            else
            {
                Msg result = null;
                try
                {
                    result = PickingManager.UnassignPickingPos(CurrentPickingPos, this.DatabaseApp);
                    if (result != null)
                    {
                        Messages.Msg(result);
                        return;
                    }
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOPicking", "UnassignPickingPos(30)", msg);
                }
            }

            OnPropertyChanged(nameof(PickingPosList));
            PartialQuantity = null;
        }

        /// <summary>
        /// Determines whether [is enabled unassign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled unassign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUnassignPickingPos()
        {
            if (CurrentPickingPos == null)
                return false;
            if (CurrentPickingPos.Picking == null || CurrentPickingPos.Picking.VisitorVoucher != null || CurrentPickingPos.Picking.Tourplan != null)
                return false;
            return true;
        }

        /// <summary>
        /// Adds a new pos witout a reference to other entities
        /// </summary>
        [ACMethodCommand("PickingPos", "en{'Add'}de{'Hinzufügen'}", 602, true)]
        public virtual void AddPickingPos()
        {
            if (!IsEnabledAddPickingPos())
                return;
            PickingPos pickingPos = PickingPos.NewACObject(DatabaseApp, CurrentPicking);
            if (pickingPos != null)
                CurrentPicking.PickingPos_Picking.Add(pickingPos);
            OnPropertyChanged(nameof(PickingPosList));
            pickingPos.FromFacility = SelectedFilterFromFacility;
            pickingPos.ToFacility = SelectedFilterToFacility;
            if (pickingPos.MDDelivPosLoadState == null)
                pickingPos.MDDelivPosLoadState = DatabaseApp.MDDelivPosLoadState.FirstOrDefault(c => c.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.NewCreated);
            CurrentPickingPos = pickingPos;
            SelectedPickingPos = pickingPos;
        }

        /// <summary>
        /// Determines whether [is enabled AddPickingPos in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled AddPickingPos in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAddPickingPos()
        {
            if (CurrentPicking == null)
                return false;
            return true;
        }


        /// <summary>
        /// Source Property: ShowDlgInwardFacility
        /// </summary>
        [ACMethodInfo("ShowDlgFromFacility", "en{'Choose facility'}de{'Lager auswählen'}", 999)]
        public void ShowDlgFromFacility()
        {
            if (!IsEnabledShowDlgFromFacility())
                return;

            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(CurrentPickingPos.FromFacility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                CurrentPickingPos.FromFacility = facility;
                OnPropertyChanged(nameof(CurrentPickingPos));
            }
        }

        public bool IsEnabledShowDlgFromFacility()
        {
            return CurrentPickingPos != null;
        }

        /// <summary>
        /// Source Property: ShowDlgInwardFacility
        /// </summary>
        [ACMethodInfo("ShowDlgToFacility", "en{'Choose facility'}de{'Lager auswählen'}", 999)]
        public void ShowDlgToFacility()
        {
            if (!IsEnabledShowDlgToFacility())
                return;

            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(CurrentPickingPos.ToFacility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                CurrentPickingPos.ToFacility = facility;
                OnPropertyChanged(nameof(CurrentPickingPos));
            }
        }

        public bool IsEnabledShowDlgToFacility()
        {
            return CurrentPickingPos != null;
        }

        #endregion

        #region FacilityPreBooking

        #region FacilityPreBooking ->  PreBooking

        [ACMethodInteraction("FacilityPreBooking", "en{'New Posting'}de{'Neue Buchung'}", (short)MISort.New, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void NewFacilityPreBooking()
        {
            if (!IsEnabledNewFacilityPreBooking())
                return;
            if (!PreExecute("NewFacilityPreBooking"))
                return;
            CurrentFacilityPreBooking = ACFacilityManager.NewFacilityPreBooking(DatabaseApp, CurrentPickingPos);
            if (CurrentFacilityPreBooking != null)
                ACFacilityManager.InitBookingParamsFromTemplate(CurrentFacilityPreBooking.ACMethodBooking as ACMethodBooking, CurrentPickingPos, CurrentFacilityPreBooking);

            OnPropertyChanged(nameof(CurrentACMethodBooking));
            OnPropertyChanged(nameof(CurrentACMethodBookingLayout));
            OnPropertyChanged(nameof(FacilityPreBookingList));
        }

        public bool IsEnabledNewFacilityPreBooking()
        {
            if (CurrentPickingPos == null)
                //|| (CurrentPickingPos.InOrderPos == null && CurrentPickingPos.OutOrderPos == null && !CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any()))
                return false;
            if (CurrentPickingPos.InOrderPos != null
                && (CurrentPickingPos.InOrderPos.MDInOrderPosState.InOrderPosState == MDInOrderPosState.InOrderPosStates.Cancelled
                 || CurrentPickingPos.InOrderPos.MDInOrderPosState.InOrderPosState == MDInOrderPosState.InOrderPosStates.Completed))
                return false;
            else if (CurrentPickingPos.OutOrderPos != null
                && (CurrentPickingPos.OutOrderPos.MDOutOrderPosState.OutOrderPosState == MDOutOrderPosState.OutOrderPosStates.Cancelled
                 || CurrentPickingPos.OutOrderPos.MDOutOrderPosState.OutOrderPosState == MDOutOrderPosState.OutOrderPosStates.Completed))
                return false;
            else if (CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any())
            {
                if (
                        CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any()
                        &&
                        CurrentPickingPos
                         .PickingPosProdOrderPartslistPos_PickingPos
                         .Select(c => c.ProdorderPartslistPos)
                         .Any(c => c.MDProdOrderPartslistPosState.ProdOrderPartslistPosState != MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created)
                    )
                    return false;
            }
            else
            {
                if (CurrentPickingPos.FromFacility == null && CurrentPickingPos.ToFacility == null)
                {
                    return false;
                }
            }
            return true;
        }

        [ACMethodInteraction("FacilityPreBooking", "en{'Cancel Posting'}de{'Buchung abbrechen'}", (short)MISort.Cancel, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void CancelFacilityPreBooking()
        {
            if (!IsEnabledCancelFacilityPreBooking())
                return;
            if (!PreExecute("CancelFacilityPreBooking"))
                return;
            var result = ACFacilityManager.CancelFacilityPreBooking(DatabaseApp, CurrentPickingPos);
            if (result != null && result.Any())
            {
                CurrentFacilityPreBooking = result.FirstOrDefault();
                OnPropertyChanged(nameof(CurrentACMethodBooking));
                OnPropertyChanged(nameof(CurrentACMethodBookingLayout));
                OnPropertyChanged(nameof(FacilityPreBookingList));
            }
        }

        public bool IsEnabledCancelFacilityPreBooking()
        {
            if (CurrentPickingPos == null
                || (CurrentPickingPos.InOrderPos == null && CurrentPickingPos.OutOrderPos == null)
                || (FacilityPreBookingList != null && FacilityPreBookingList.Any()))
                return false;
            if (CurrentPickingPos == null || (CurrentPickingPos.InOrderPos == null && CurrentPickingPos.OutOrderPos == null))
                return false;
            if (CurrentPickingPos.InOrderPos != null
                && CurrentPickingPos.InOrderPos.MDInOrderPosState.InOrderPosState == MDInOrderPosState.InOrderPosStates.Cancelled)
                return false;
            else if (CurrentPickingPos.OutOrderPos != null
                && CurrentPickingPos.OutOrderPos.MDOutOrderPosState.OutOrderPosState == MDOutOrderPosState.OutOrderPosStates.Cancelled)
                return false;
            return true;
        }

        [ACMethodInteraction("FacilityPreBooking", "en{'Delete Posting'}de{'Lösche Buchung'}", (short)MISort.Delete, true, "SelectedPickingPos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteFacilityPreBooking()
        {
            if (!IsEnabledDeleteFacilityPreBooking())
                return;
            if (!PreExecute("DeleteFacilityPreBooking"))
                return;
            DeleteFacilityPreBooking(CurrentFacilityPreBooking);

        }

        public void DeleteFacilityPreBooking(FacilityPreBooking CurrentFacilityPreBooking)
        {
            Msg msg = CurrentFacilityPreBooking.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            else
            {
                CurrentFacilityPreBooking = null;
                OnPropertyChanged(nameof(FacilityPreBookingList));
            }
        }

        public bool IsEnabledDeleteFacilityPreBooking()
        {
            return CurrentFacilityPreBooking != null;
        }

        [ACMethodInteraction("FacilityPreBooking", "en{'Post Item'}de{'Buche Position'}", (short)MISort.New, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void BookDeliveryPos()
        {
            if (!PreExecute("BookDeliveryPos")) return;
            PostExecute("BookDeliveryPos");
        }

        public bool IsEnabledBookDeliveryPos()
        {
            return true;
        }

        [ACMethodInteraction("FacilityPreBooking", "en{'Post'}de{'Buchen'}", (short)MISort.New, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void BookCurrentACMethodBooking()
        {
            if (!IsEnabledBookCurrentACMethodBooking())
                return;

            BookACMethodBooking(CurrentPickingPos, CurrentACMethodBooking, CurrentFacilityPreBooking, SetQuantToZeroMode.CheckIfZeroAskUser);
            PostExecute(nameof(BookCurrentACMethodBooking));
        }

        public enum SetQuantToZeroMode
        {
            Off = 0,
            CheckIfZeroAskUser = 1,
            Force = 2
        }

        public virtual bool BookACMethodBooking(PickingPos currentPickingPos, ACMethodBooking currentACMethodBooking, FacilityPreBooking currentFacilityPreBooking, SetQuantToZeroMode autoSetQuantToZero)
        {
            if (currentPickingPos.OutOrderPos != null)
            {
                if (currentACMethodBooking.OutOrderPos != currentPickingPos.OutOrderPos)
                    currentACMethodBooking.OutOrderPos = currentPickingPos.OutOrderPos;
                if (currentPickingPos.OutOrderPos.OutOrder.CPartnerCompany != null && currentACMethodBooking.CPartnerCompany != currentPickingPos.OutOrderPos.OutOrder.CPartnerCompany)
                    currentACMethodBooking.CPartnerCompany = currentPickingPos.OutOrderPos.OutOrder.CPartnerCompany;
            }
            else if (currentPickingPos.InOrderPos != null)
            {
                if (currentACMethodBooking.InOrderPos != currentPickingPos.InOrderPos)
                    currentACMethodBooking.InOrderPos = currentPickingPos.InOrderPos;
                if (currentPickingPos.InOrderPos.InOrder.CPartnerCompany != null && currentACMethodBooking.CPartnerCompany != currentPickingPos.InOrderPos.InOrder.CPartnerCompany)
                    currentACMethodBooking.CPartnerCompany = currentPickingPos.InOrderPos.InOrder.CPartnerCompany;
            }

            if (currentACMethodBooking.FacilityBooking != null)
            {
                if (currentACMethodBooking.FacilityBooking.OutOrderPos != currentACMethodBooking.OutOrderPos)
                    currentACMethodBooking.FacilityBooking.OutOrderPos = currentACMethodBooking.OutOrderPos;

                if (currentACMethodBooking.FacilityBooking.InOrderPos != currentACMethodBooking.InOrderPos)
                    currentACMethodBooking.FacilityBooking.InOrderPos = currentACMethodBooking.InOrderPos;

                if (currentACMethodBooking.FacilityBooking.PickingPos != currentACMethodBooking.PickingPos)
                    currentACMethodBooking.FacilityBooking.PickingPos = currentACMethodBooking.PickingPos;

                if (currentACMethodBooking.FacilityBooking.InwardMaterial != currentACMethodBooking.InwardMaterial)
                    currentACMethodBooking.FacilityBooking.InwardMaterial = currentACMethodBooking.InwardMaterial;

                if (currentACMethodBooking.FacilityBooking.OutwardMaterial != currentACMethodBooking.OutwardMaterial)
                    currentACMethodBooking.FacilityBooking.OutwardMaterial = currentACMethodBooking.OutwardMaterial;

                if (currentACMethodBooking.FacilityBooking.OutwardFacility != currentACMethodBooking.OutwardFacility)
                    currentACMethodBooking.FacilityBooking.OutwardFacility = currentACMethodBooking.OutwardFacility;

                if (currentACMethodBooking.FacilityBooking.InwardFacility != currentACMethodBooking.InwardFacility)
                    currentACMethodBooking.FacilityBooking.InwardFacility = currentACMethodBooking.InwardFacility;
            }

            OnValidateBookingParams(currentACMethodBooking);

            bool isCancellation = currentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel || currentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel;

            Save();
            if (DatabaseApp.IsChanged)
                return false;
            if (!PreExecute(nameof(BookCurrentACMethodBooking)))
                return false;

            currentACMethodBooking.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(currentACMethodBooking, this.DatabaseApp) as ACMethodEventArgs;
            if (!currentACMethodBooking.ValidMessage.IsSucceded() || currentACMethodBooking.ValidMessage.HasWarnings())
            {
                Messages.Msg(currentACMethodBooking.ValidMessage);
                return false;
            }
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
                OnPropertyChanged(nameof(FacilityBookingList));
                return false;
            }
            else
            {
                double changedQuantity = 0;
                FacilityCharge outwardFC = null;

                if (currentACMethodBooking != null)
                {
                    if (currentACMethodBooking.OutwardQuantity.HasValue)
                        changedQuantity = currentACMethodBooking.OutwardQuantity.Value;
                    else if (currentACMethodBooking.InwardQuantity.HasValue)
                        changedQuantity = currentACMethodBooking.InwardQuantity.Value;

                    outwardFC = currentACMethodBooking.OutwardFacilityCharge;
                }

                DeleteFacilityPreBooking(currentFacilityPreBooking);
                OnPropertyChanged(nameof(FacilityBookingList));
                ACFacilityManager.RecalcAfterPosting(DatabaseApp, currentPickingPos, changedQuantity, isCancellation, true);
                Save();

                Msg msg = null;
                if (autoSetQuantToZero == SetQuantToZeroMode.CheckIfZeroAskUser)
                    msg = ACFacilityManager.IsQuantStockConsumed(outwardFC, DatabaseApp);
                if (autoSetQuantToZero == SetQuantToZeroMode.Force || msg != null)
                {
                    MsgResult msgResult = MsgResult.No;
                    if (msg != null)
                        msgResult = Messages.Question(this, msg.Message, MsgResult.No, true);
                    if (autoSetQuantToZero == SetQuantToZeroMode.Force || msgResult == MsgResult.Yes)
                    {
                        if (ACFacilityManager == null)
                            return false;

                        ACMethodBooking fbtZeroBooking = PickingManager.BookParamZeroStockFacilityChargeClone(ACFacilityManager, DatabaseApp);
                        ACMethodBooking fbtZeroBookingClone = fbtZeroBooking.Clone() as ACMethodBooking;

                        fbtZeroBookingClone.InwardFacilityCharge = outwardFC;
                        fbtZeroBookingClone.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);

                        fbtZeroBookingClone.AutoRefresh = true;
                        ACMethodEventArgs resultZeroBook = ACFacilityManager.BookFacility(fbtZeroBookingClone, this.DatabaseApp);
                        if (!fbtZeroBookingClone.ValidMessage.IsSucceded() || fbtZeroBookingClone.ValidMessage.HasWarnings())
                        {
                            Messages.Msg(currentACMethodBooking.ValidMessage);
                            return false;
                        }
                        else if (resultZeroBook.ResultState == Global.ACMethodResultState.Failed || resultZeroBook.ResultState == Global.ACMethodResultState.Notpossible)
                        {
                            if (String.IsNullOrEmpty(result.ValidMessage.Message))
                                result.ValidMessage.Message = result.ResultState.ToString();
                            Messages.Msg(result.ValidMessage);
                            return false;
                        }
                    }
                }
            }
            return true;
        }


        public bool IsEnabledBookCurrentACMethodBooking()
        {
            if (_CurrentACMethodBookingDummy != null)
                return false;
            bool bRetVal = false;
            if (CurrentACMethodBooking != null)
            {
                OnValidateBookingParams(CurrentACMethodBooking);
                bRetVal = CurrentACMethodBooking.IsEnabled();
            }
            else
                return false;
            UpdateBSOMsg();
            return bRetVal;
        }

        protected virtual void OnValidateBookingParams(ACMethodBooking currentACMethodBooking)
        {
            if (currentACMethodBooking == null)
                return;
            if (currentACMethodBooking.BookingType < GlobalApp.FacilityBookingType.InOrderPosInwardMovement
                || currentACMethodBooking.BookingType > GlobalApp.FacilityBookingType.PickingOutwardCancel)
            {
                if (currentACMethodBooking.InwardQuantity.HasValue && !currentACMethodBooking.OutwardQuantity.HasValue)
                    currentACMethodBooking.OutwardQuantity = currentACMethodBooking.InwardQuantity;
                else if (!currentACMethodBooking.InwardQuantity.HasValue && currentACMethodBooking.OutwardQuantity.HasValue)
                    currentACMethodBooking.InwardQuantity = currentACMethodBooking.OutwardQuantity;
                else if (currentACMethodBooking.InwardQuantity != currentACMethodBooking.OutwardQuantity)
                    currentACMethodBooking.InwardQuantity = currentACMethodBooking.OutwardQuantity;
            }
        }

        [ACMethodInfo(nameof(BookAllACMethodBookings), "en{'Post All'}de{'Buche alle'}", 101, true)]
        public void BookAllACMethodBookings()
        {
            if (!IsEnabledBookAllACMethodBookings())
                return;
            foreach (FacilityPreBooking facilityPreBooking in FacilityPreBookingList.ToList())
            {
                SelectedFacilityPreBooking = facilityPreBooking;
                if (CurrentFacilityPreBooking == facilityPreBooking)
                    BookCurrentACMethodBooking();
            }
        }

        public bool IsEnabledBookAllACMethodBookings()
        {
            if (CurrentPickingPos == null || FacilityPreBookingList == null || !FacilityPreBookingList.Any())
                return false;
            return true;
        }

        /// <summary>
        /// Loop trough all picking lines and book prepared pre-bookings
        /// </summary>
        [ACMethodInfo(nameof(BookAllPositions), "en{'Post all positions'}de{'Buche alle line'}", 102, true)]
        public void BookAllPositions()
        {
            if (!IsEnabledBookAllPositions())
                return;
            PickingPos[] lines = PickingPosList.ToArray();
            foreach (PickingPos pos in lines)
            {
                CurrentPickingPos = pos;
                if (IsEnabledBookAllACMethodBookings())
                {
                    BookAllACMethodBookings();
                }
            }
        }

        public bool IsEnabledBookAllPositions()
        {
            return PickingPosList != null && PickingList.Any();
        }


        [ACMethodInfo("Dialog", "en{'New Lot'}de{'Neues Los'}", (short)MISort.New)]
        public void NewFacilityLot()
        {
            if (!IsEnabledNewFacilityLot())
                return;
            ACComponent childBSO = ACUrlCommand("?FacilityLotDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("FacilityLotDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!ShowDialogNewLot", "", CurrentPickingPos.Material);
            dlgResult_OnDialogResult(dlgResult);
            childBSO.Stop();
        }

        void dlgResult_OnDialogResult(VBDialogResult dlgResult)
        {
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                FacilityLot result = dlgResult.ReturnValue as FacilityLot;
                if (result != null)
                {
                    Save();
                    CurrentACMethodBooking.InwardFacilityLot = result;
                    OnNewCreatedFacilityLot(result);
                    if (AccessBookingFacilityLot != null)
                        AccessBookingFacilityLot.NavSearch(DatabaseApp);
                    Save();
                }
            }
        }

        public virtual void OnNewCreatedFacilityLot(FacilityLot lot)
        {
        }

        public bool IsEnabledNewFacilityLot()
        {
            // Nur bei Wareneingängen kann Charge ausgewählt werden
            return CurrentACMethodBooking != null
                && CurrentACMethodBooking.InwardFacilityLot == null
                && CurrentPickingPos != null
                && (CurrentACMethodBooking.InOrderPos != null
                    || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosInwardMovement
                    || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel
                    || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.PickingInward)
                && CurrentPickingPos.Material != null
                && CurrentPickingPos.Material.IsLotManaged;
        }

        [ACMethodInfo("Dialog", "en{'Show Lot'}de{'Los anzeigen'}", (short)MISort.New + 1)]
        public void ShowFacilityLot()
        {
            if (!IsEnabledShowFacilityLot())
                return;
            ACComponent childBSO = ACUrlCommand("?FacilityLotDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("FacilityLotDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            string lotNo = null;
            if (CurrentACMethodBooking.OutwardFacilityLot != null)
                lotNo = CurrentACMethodBooking.OutwardFacilityLot.LotNo;
            else if (CurrentACMethodBooking.InwardFacilityLot != null)
                lotNo = CurrentACMethodBooking.InwardFacilityLot.LotNo;
            else
                return;
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!ShowDialogOrder", lotNo);
            childBSO.Stop();
        }

        public bool IsEnabledShowFacilityLot()
        {
            return CurrentACMethodBooking != null
                    && (CurrentACMethodBooking.InwardFacilityLot != null
                        || CurrentACMethodBooking.OutwardFacilityLot != null);
        }

        #endregion

        #region FacilityPreBooking -> Available quants

        /// <summary>
        /// Source Property: ShowDlgAvailableQuants
        /// </summary>
        [ACMethodInfo("ShowDlgInwardAvailableQuants", "en{'Choose quant'}de{'Quant auswählen'}", 999)]
        public void ShowDlgInwardAvailableQuants()
        {
            if (!IsEnabledShowDlgInwardAvailableQuants())
                return;
            _IsInward = true;
            _QuantDialogMaterial = GetPreBookingInwardMaterial();
            _PreBookingAvailableQuantsList = null;
            ShowDialog(this, "DlgAvailableQuants");
        }

        public bool IsEnabledShowDlgInwardAvailableQuants()
        {
            return CurrentACMethodBooking != null && GetPreBookingInwardMaterial() != null;
        }

        [ACMethodInfo("ShowDlgOutwardAvailableQuants", "en{'Choose quant'}de{'Quant auswählen'}", 999)]
        public void ShowDlgOutwardAvailableQuants()
        {
            if (!IsEnabledShowDlgOutwardAvailableQuants())
                return;
            _IsInward = false;
            _QuantDialogMaterial = GetPreBookingOutwardMaterial();
            _PreBookingAvailableQuantsList = null;
            ShowDialog(this, "DlgAvailableQuants");
        }

        public bool IsEnabledShowDlgOutwardAvailableQuants()
        {
            return CurrentACMethodBooking != null && GetPreBookingOutwardMaterial() != null;
        }

        // DlgAvailableQuantsOk
        /// <summary>
        /// Source Property: DlgAvailableQuantsOk
        /// </summary>
        [ACMethodInfo("DlgAvailableQuantsOk", "en{'Ok'}de{'Ok'}", 999)]
        public void DlgAvailableQuantsOk()
        {
            if (!IsEnabledDlgAvailableQuantsOk())
                return;
            if (_IsInward)
            {
                CurrentACMethodBooking.InwardFacility = SelectedPreBookingAvailableQuants.Facility;
                CurrentACMethodBooking.InwardFacilityCharge = SelectedPreBookingAvailableQuants;
                CurrentACMethodBooking.InwardMaterial = null;
                CurrentACMethodBooking.InwardFacilityLot = null;
            }
            else
            {
                CurrentACMethodBooking.OutwardFacility = SelectedPreBookingAvailableQuants.Facility;
                CurrentACMethodBooking.OutwardFacilityCharge = SelectedPreBookingAvailableQuants;
                CurrentACMethodBooking.OutwardMaterial = null;
                CurrentACMethodBooking.OutwardFacilityLot = null;
            }
            OnPropertyChanged(nameof(CurrentACMethodBooking));
            CloseTopDialog();
        }

        public bool IsEnabledDlgAvailableQuantsOk()
        {
            return SelectedPreBookingAvailableQuants != null;
        }

        [ACMethodInfo("DlgAvailableQuantsCancel", "en{'Close'}de{'Schließen'}", 999)]
        public void DlgAvailableQuantsCancel()
        {
            CloseTopDialog();
        }


        private Material GetPreBookingOutwardMaterial()
        {
            if (CurrentACMethodBooking != null && CurrentPickingPos != null && CurrentPickingPos.Material != null)
                return CurrentPickingPos.Material;
            return null;
        }

        private Material GetPreBookingInwardMaterial()
        {
            if (CurrentACMethodBooking != null && CurrentPickingPos != null && CurrentPickingPos.Material != null)
                return CurrentPickingPos.Material;
            return null;
        }

        #endregion

        #region FacilityPreBooking -> Select Facility

        /// <summary>
        /// Source Property: ShowDlgInwardFacility
        /// </summary>
        [ACMethodInfo("ShowDlgInwardFacility", "en{'Choose facility'}de{'Lager auswählen'}", 999)]
        public void ShowDlgInwardFacility()
        {
            if (!IsEnabledShowDlgInwardFacility())
                return;
            FacilitySelectLoctation = FacilitySelectLoctation.PrebookingInward;
            ShowDlgFacility(CurrentACMethodBooking.InwardFacility);
        }

        public bool IsEnabledShowDlgInwardFacility()
        {
            return CurrentACMethodBooking != null;
        }


        /// <summary>
        /// Source Property: ShowDlgInwardFacility
        /// </summary>
        [ACMethodInfo("ShowDlgInwardFacility", "en{'Choose facility'}de{'Lager auswählen'}", 999)]
        public void ShowDlgOutwardFacility()
        {
            if (!IsEnabledShowDlgOutwardFacility())
                return;
            FacilitySelectLoctation = FacilitySelectLoctation.PrebookingOutward;
            ShowDlgFacility(CurrentACMethodBooking.OutwardFacility);
        }

        public bool IsEnabledShowDlgOutwardFacility()
        {
            return CurrentACMethodBooking != null;
        }

        /// <summary>
        /// 
        /// </summary>
        [ACMethodInfo("ShowDlgFilterFromFacility", "en{'Choose facility'}de{'Lager auswählen'}", 999)]
        public void ShowDlgFilterFromFacility()
        {
            if (!IsEnabledShowDlgFilterFromFacility())
                return;
            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(SelectedFilterFromFacility != null ? SelectedFilterFromFacility : null);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                if (facility != null)
                    if (!AccessFilterFromFacility.NavList.Contains(facility))
                        AccessFilterFromFacility.NavList.Add(facility);
                SelectedFilterFromFacility = facility;
            }
        }

        public bool IsEnabledShowDlgFilterFromFacility()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        [ACMethodInfo("ShowDlgFilterToFacility", "en{'Choose facility'}de{'Lager auswählen'}", 999)]
        public void ShowDlgFilterToFacility()
        {
            if (!IsEnabledShowDlgFilterToFacility())
                return;
            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(SelectedFilterToFacility != null ? SelectedFilterToFacility : null);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                if (facility != null)
                    if (!AccessFilterToFacility.NavList.Contains(facility))
                        AccessFilterToFacility.NavList.Add(facility);
                SelectedFilterToFacility = facility;
            }
        }

        public bool IsEnabledShowDlgFilterToFacility()
        {
            return true;
        }

        private void ShowDlgFacility(Facility preselectedFacility)
        {

            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(preselectedFacility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                switch (FacilitySelectLoctation)
                {
                    case FacilitySelectLoctation.PrebookingInward:
                        CurrentACMethodBooking.InwardFacility = facility;
                        break;
                    case FacilitySelectLoctation.PrebookingOutward:
                        CurrentACMethodBooking.OutwardFacility = facility;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #endregion

        #region Message
        protected virtual void UpdateBSOMsg()
        {
            if (CurrentACMethodBooking == null)
                return;
            if (!CurrentACMethodBooking.ValidMessage.IsSucceded() || CurrentACMethodBooking.ValidMessage.HasWarnings())
            {
                if (!BSOMsg.IsEqual(CurrentACMethodBooking.ValidMessage))
                {
                    BSOMsg.UpdateFrom(CurrentACMethodBooking.ValidMessage);
                }
            }
            else
            {
                if (BSOMsg.MsgDetailsCount > 0)
                    BSOMsg.ClearMsgDetails();
            }
        }
        #endregion

        #region Report
        //[ACMethodCommand("Picking", "en{'Print'}de{'Drucken'}", (short)MISort.QueryPrint, false, Global.ACKinds.MSMethodPrePost)]
        //public void ReportPrint()
        //{
        //    if (!PreExecute("ReportPrint")) return;
        //    // TODO: Drucken
        //    PostExecute("ReportPrint");
        //}
        #endregion

        #region Activation
        /// <summary>
        /// ACAction is called when one IACInteractiveObject (Source) wants to inform another IACInteractiveObject (Target) about an relevant interaction-event.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public override void ACAction(ACActionArgs actionArgs)
        {
            if (actionArgs.ElementAction == Global.ElementActionType.TabItemActivated)
            {
                OnActivate(actionArgs.DropObject.VBContent);
            }
            else
                base.ACAction(actionArgs);
        }

        protected bool _ActivateInOpen = false;
        protected bool _ActivateInDNote = false;
        protected bool _ActivateOutOpen = false;
        protected bool _ActivateOutDNote = false;
        protected bool _ActivateProdOpen = false;
        [ACMethodInfo("Picking", "en{'Activate'}de{'Aktivieren'}", 602, true, Global.ACKinds.MSMethodPrePost)]
        public void OnActivate(string page)
        {
            if (!PreExecute("OnActivate"))
                return;
            switch (page)
            {
                case "*AssignInOrderPos":
                case "ActivateInOpen":
                    if (!_ActivateInOpen)
                    {
                        _ActivateInOpen = true;
                        RefreshInOrderPosList();
                    }
                    break;
                case "ActivateInDNote":
                    if (!_ActivateInDNote)
                    {
                        _ActivateInDNote = true;
                        OnPropertyChanged(nameof(DNoteInOrderPosList));
                    }
                    break;
                case "*AssignOutOrderPos":
                case "ActivateOutOpen":
                    if (!_ActivateOutOpen)
                    {
                        _ActivateOutOpen = true;
                        RefreshOutOrderPosList();
                    }
                    break;
                case "ActivateOutDNote":
                    if (!_ActivateOutDNote)
                    {
                        _ActivateOutDNote = true;
                        OnPropertyChanged(nameof(DNoteOutOrderPosList));
                    }
                    break;
                case "*AssignProdOrderPos":
                    if (!_ActivateProdOpen)
                    {
                        _ActivateProdOpen = true;
                        RefreshProdOrderPartslistPosList();
                    }
                    break;
                //case "*PickingPos":
                //    OnPropertyChanged(nameof(CurrentPickingPos));
                //    break;
                default:
                    break;
            }
            PostExecute("OnActivate");
        }
        #endregion

        #region Validation
        [ACMethodCommand("", "en{'Check Routes'}de{'Routenprüfung'}", (short)MISort.Cancel)]
        public void ValidateRoutes()
        {
            ACSaveChanges();
            if (!IsEnabledValidateRoutes())
                return;
            ValidateRoutesInternal();
        }

        protected MsgWithDetails ValidateRoutesInternal()
        {
            MsgWithDetails msg = null;
            using (var dbIPlus = new Database())
            {
                msg = this.PickingManager.ValidateRoutes(this.DatabaseApp, dbIPlus, CurrentPicking,
                                                            MandatoryConfigStores,
                                                            PARole.ValidationBehaviour.Laxly);
                if (msg != null)
                {
                    if (!msg.IsSucceded())
                    {
                        if (String.IsNullOrEmpty(msg.Message))
                        {
                            // Die Stückliste wäre nicht produzierbar weil:
                            msg.Message = Root.Environment.TranslateMessage(this, "Info50020");
                        }
                        Messages.Msg(msg, Global.MsgResult.OK, eMsgButton.OK);
                        return msg;
                    }
                    else if (msg.HasWarnings())
                    {
                        if (String.IsNullOrEmpty(msg.Message))
                        {
                            // Es gäbe folgende Probleme wenn Sie einen Auftrag anlegen und starten würden:
                            msg.Message = Root.Environment.TranslateMessage(this, "Info50021");
                        }
                        Messages.Msg(msg, Global.MsgResult.OK, eMsgButton.OK);
                        return msg;
                    }
                }
                // Die Routenprüfung war erflogreich. Die Stückliste ist produzierbar.
                Messages.Info(this, "Info50022");
                return msg;
            }
        }

        public bool IsEnabledValidateRoutes()
        {
            if (CurrentPicking == null || this.PickingManager == null)
                return false;
            return true;
        }
        #endregion

        #region Start Workflow
        //private static bool _IsStartingWF = false;
        [ACMethodCommand("Start", "en{'Start Transports'}de{'Transporte starten'}", 604, true)]
        public void StartWorkflow()
        {
            //_IsStartingWF = true;
            try
            {
                if (!IsEnabledStartWorkflow())
                    return;

                MsgWithDetails msg = null;
                using (var dbIPlus = new Database())
                {
                    msg = this.PickingManager.ValidateStart(this.DatabaseApp, dbIPlus, CurrentPicking,
                                                                MandatoryConfigStores,
                                                                PARole.ValidationBehaviour.Strict);
                    if (msg != null)
                    {
                        if (!msg.IsSucceded())
                        {
                            if (String.IsNullOrEmpty(msg.Message))
                            {
                                // Der Auftrag kann nicht gestartet werden weil:
                                msg.Message = Root.Environment.TranslateMessage(this, "Question50027");
                            }
                            Messages.Msg(msg, Global.MsgResult.OK, eMsgButton.OK);
                            return;
                        }
                        else if (msg.HasWarnings())
                        {
                            if (String.IsNullOrEmpty(msg.Message))
                            {
                                //Möchten Sie den Auftrag wirklich starten? Es gibt nämlich folgende Probleme:
                                msg.Message = Root.Environment.TranslateMessage(this, "Question50028");
                            }
                            var userResult = Messages.Msg(msg, Global.MsgResult.No, eMsgButton.YesNo);
                            if (userResult == Global.MsgResult.No || userResult == Global.MsgResult.Cancel)
                                return;
                        }
                    }
                }

                gip.core.datamodel.ACClassMethod acClassMethod = CurrentPicking.ACClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>(this.DatabaseApp.ContextIPlus);
                if (acClassMethod == null)
                    return;
                gip.core.datamodel.ACProject project = acClassMethod.ACClass.ACProject as gip.core.datamodel.ACProject;

                AppManagersList = this.Root.FindChildComponents(project.RootClass, 1);
                if (AppManagersList.Count > 1)
                {
                    DialogResult = null;
                    ShowDialog(this, "SelectAppManager");
                    if (DialogResult == null || DialogResult.SelectedCommand != eMsgButton.OK)
                        return;
                }
                else
                    SelectedAppManager = AppManagersList.FirstOrDefault();

                ACComponent pAppManager = SelectedAppManager as ACComponent;
                if (pAppManager == null)
                    return;
                if (pAppManager.IsProxy && pAppManager.ConnectionState == ACObjectConnectionState.DisConnected)
                {
                    // TODO: Message
                    return;
                }

                msg = this.PickingManager.StartPicking(this.DatabaseApp, pAppManager, CurrentPicking, acClassMethod, SelectedPWNodeProcessWorkflow, true);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOPicking", "StartWorkflow", msg);
            }
            finally
            {
                //_IsStartingWF = false;
            }

        }

        public bool IsEnabledStartWorkflow()
        {
            if (this.CurrentPicking == null
                || CurrentPicking.ACClassMethod == null
                || this.CurrentPicking.PickingState > PickingStateEnum.InProcess
                || PickingPosList == null
                || !PickingPosList.Any()
                || !PickingPosList.Where(c => c.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad).Any()
                || PickingManager == null)
                return false;

            if (ProcessWorkflowPresenter != null)
                return SelectedPWNodeProcessWorkflow != null;

            return true;
        }


        protected gip.core.datamodel.ACClassWF SelectedPWNodeProcessWorkflow
        {
            get
            {
                if (ProcessWorkflowPresenter == null
                    || ProcessWorkflowPresenter.SelectedWFNode == null
                    || ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF == null
                    || ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF.PWACClass == null)
                    return null;
                if (typeof(PWNodeProcessWorkflow).IsAssignableFrom(ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF.PWACClass.ObjectType))
                    return ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF;
                return null;
            }
        }

        protected IACComponent _SelectedAppManager;
        [ACPropertySelected(630, "AppManagers")]
        public IACComponent SelectedAppManager
        {
            get
            {
                return _SelectedAppManager;
            }
            set
            {
                _SelectedAppManager = value;
                OnPropertyChanged(nameof(SelectedAppManager));
            }
        }

        protected List<IACComponent> _AppManagersList;
        [ACPropertyList(631, "AppManagers")]
        public List<IACComponent> AppManagersList
        {
            get
            {
                return _AppManagersList;
            }
            set
            {
                _AppManagersList = value;
                OnPropertyChanged(nameof(AppManagersList));
            }
        }

        public VBDialogResult DialogResult { get; set; }

        [ACMethodCommand("Dialog", "en{'OK'}de{'OK'}", (short)MISort.Okay)]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            var selected = SelectedAppManager;
            CloseTopDialog();
            SelectedAppManager = selected;
        }

        [ACMethodCommand("Dialog", "en{'Cancel'}de{'Abbrechen'}", (short)MISort.Cancel)]
        public void DialogCancel()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            var selected = SelectedAppManager;
            CloseTopDialog();
            SelectedAppManager = selected;
        }

        public virtual PickingPos GetPickingPosForWFTargetSelector()
        {
            return SelectedPickingPos;
        }
        #endregion

        #region Show order dialog

        [ACMethodInfo("Dialog", "en{'Dialog Picking Order'}de{'Dialog Kommissionierauftrag'}", (short)MISort.QueryPrintDlg)]
        public void ShowDialogOrder(string pickingNo, Guid pickingPosID)
        {
            if (AccessPrimary == null)
                return;
            //AccessPrimary.NavACQueryDefinition.SearchWord = facilityNo;
            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "PickingNo").FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, "PickingNo", Global.LogicalOperators.contains, Global.Operators.and, pickingNo, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            else
                filterItem.SearchWord = pickingNo;

            this.Search();
            if (PickingList != null && PickingList.Count() > 1)
                CurrentPicking = PickingList.FirstOrDefault(c => c.PickingNo == pickingNo);
            if (CurrentPicking != null && pickingPosID != Guid.Empty)
            {
                if (this.PickingPosList != null && this.PickingPosList.Any())
                {
                    var pickingPos = this.PickingPosList.Where(c => c.PickingPosID == pickingPosID).FirstOrDefault();
                    if (pickingPos != null)
                        SelectedPickingPos = pickingPos;
                }
            }
            ShowDialog(this, "DisplayOrderDialog");
            this.ParentACComponent.StopComponent(this);
            _IsEnabledACProgram = true;
        }

        [ACMethodInfo("Dialog", "en{'Dialog Picking Order'}de{'Dialog Kommissionierauftrag'}", (short)MISort.QueryPrintDlg + 1)]
        public void ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;

            // Falls Produktionsauftrag
            PickingPos pickingPos = null;
            Picking picking = null;
            foreach (var entry in paOrderInfo.Entities)
            {
                if (entry.EntityName == Picking.ClassName)
                {
                    picking = this.DatabaseApp.Picking
                        .Where(c => c.PickingID == entry.EntityID)
                        .FirstOrDefault();
                }
                else if (entry.EntityName == PickingPos.ClassName)
                {
                    pickingPos = this.DatabaseApp.PickingPos
                        .Include(c => c.Picking)
                        .Where(c => c.PickingPosID == entry.EntityID)
                        .FirstOrDefault();
                    if (pickingPos != null)
                        picking = pickingPos.Picking;
                }
                else if (entry.EntityName == OrderLog.ClassName)
                {
                    _IsEnabledACProgram = false;
                    OrderLog currentOrderLog = DatabaseApp.OrderLog.FirstOrDefault(c => c.VBiACProgramLogID == entry.EntityID);
                    if (currentOrderLog == null || currentOrderLog.PickingPos == null)
                        return;
                    pickingPos = currentOrderLog.PickingPos;
                    picking = pickingPos.Picking;
                }
            }

            if (picking == null)
                return;

            ShowDialogOrder(picking.PickingNo, pickingPos != null ? pickingPos.PickingPosID : Guid.Empty);
            paOrderInfo.DialogResult = this.DialogResult;
        }
        #endregion

        #region Context menu (Tracking)

        #region Tracking
        public override ACMenuItemList GetMenu(string vbContent, string vbControl)
        {
            ACMenuItemList aCMenuItems = base.GetMenu(vbContent, vbControl);
            if (vbContent == nameof(SelectedFacilityBooking) && SelectedFacilityBooking != null)
            {
                TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
                ACMenuItemList trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedFacilityBooking);
                aCMenuItems.AddRange(trackingAndTracingMenuItems);
            }
            return aCMenuItems;
        }

        [ACMethodInfo("OnTrackingCall", "en{'OnTrackingCall'}de{'OnTrackingCall'}", 600, false)]
        public void OnTrackingCall(GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter, TrackingEnginesEnum engine)
        {
            TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
            trackingCommonStart.DoTracking(this, direction, itemForTrack, additionalFilter, engine);
        }

        #endregion

        #endregion

        #region ACMethods -> Filter

        /// <summary>
        /// Source Property: FilterClear
        /// </summary>
        [ACMethodInfo("FilterClear", "en{'Clear'}de{'Löschen'}", 307)]
        public void FilterClear()
        {
            if (!IsEnabledFilterClear())
                return;

            FilterPickingPickingNo = null;
            FilterDateFrom = null;
            FilterDateTo = null;
            FilterMaterialNo = null;
            FilterLotNoFB = null;
            FilterExternLotNoFB = null;
            FilterLotNoFR = null;
            FilterExternLotNoFR = null;
            SelectedFilterMDPickingType = null;
            SelectedFilterPickingState = null;
            SelectedFilterDeliveryAddress = null;
        }

        public bool IsEnabledFilterClear()
        {
            return true;
        }

        #endregion

        #region BroadCasting

        [ACMethodCommand("BroadCastPicking", "en{'Broadcast picking'}de{'Sende Kommissionierauftrag'}", 800, true)]
        public void BroadCastPicking()
        {
            if (!IsEnabledBroadCastPicking())
                return;
            PickingPos[] pickingPositions = CurrentPicking.PickingPos_Picking.ToArray();

            List<Facility> facilities = new List<Facility>();
            foreach (PickingPos pickingPos in pickingPositions)
            {
                if (pickingPos.FromFacility != null && pickingPos.FromFacility.IsMirroredOnMoreDatabases && !facilities.Contains(pickingPos.FromFacility))
                {
                    facilities.Add(pickingPos.FromFacility);
                }
                if (pickingPos.ToFacility != null && pickingPos.ToFacility.IsMirroredOnMoreDatabases && !facilities.Contains(pickingPos.ToFacility))
                {
                    facilities.Add(pickingPos.ToFacility);
                }
            }

            foreach (Facility facility in facilities)
            {
                facility.CallSendPicking(false, CurrentPicking.PickingID);
            }

            facilities = new List<Facility>();
            FacilityBooking[] facilityBookings = CurrentPicking.PickingPos_Picking.SelectMany(c => c.FacilityBooking_PickingPos).ToArray();

            foreach (FacilityBooking facilityBooking in facilityBookings)
            {
                if (facilityBooking.InwardFacility != null && facilityBooking.InwardFacility.IsMirroredOnMoreDatabases && !facilities.Contains(facilityBooking.InwardFacility))
                {
                    facilities.Add(facilityBooking.InwardFacility);
                }
                if (facilityBooking.OutwardFacility != null && facilityBooking.OutwardFacility.IsMirroredOnMoreDatabases && !facilities.Contains(facilityBooking.OutwardFacility))
                {
                    facilities.Add(facilityBooking.OutwardFacility);
                }
            }

            foreach (Facility facility in facilities)
            {
                foreach (FacilityBooking facilityBooking in facilityBookings)
                {
                    facility.CallRefreshFacility(true, facilityBooking.FacilityBookingID);
                }
            }

        }

        public bool IsEnabledBroadCastPicking()
        {
            return CurrentPicking != null && CurrentPicking.PickingPos_Picking.Any();
        }

        #endregion

        #region LabOrder

        [ACMethodInteraction("Dialog", "en{'New Lab Order'}de{'Neuer Laborauftrag'}", 605, false, "CreateNewLabOrder", Global.ACKinds.MSMethodPrePost)]
        public virtual void CreateNewLabOrder()
        {
            Save();
            if (this.DatabaseApp.IsChanged || SelectedPickingPos == null)
                return;
            ACComponent childBSO = ACUrlCommand("?LabOrderDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("LabOrderDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            childBSO.ACUrlCommand("!" + nameof(BSOLabOrder.NewLabOrderDialog), null, null, null, null, SelectedPickingPos);
            childBSO.Stop();
        }

        public bool IsEnabledCreateNewLabOrder()
        {
            if (SelectedPickingPos != null)
            {
                if (SelectedPickingPos.LabOrder_PickingPos.Any())
                    return false;
            }

            return true;
        }

        [ACMethodInfo("Dialog", "en{'Lab Report'}de{'Laborbericht'}", 606)]
        public void ShowLabOrder()
        {
            if (this.DatabaseApp.IsChanged || SelectedPickingPos == null)
                return;
            ACComponent childBSO = ACUrlCommand("?LabOrderDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("LabOrderDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            childBSO.ACUrlCommand("!" + nameof(BSOLabOrder.ShowLabOrderViewDialog), null, null, null, null, SelectedPickingPos, null, true, null);
            childBSO.Stop();
        }

        public bool IsEnabledShowLabOrder()
        {
            if (SelectedPickingPos != null)
            {
                if (!SelectedPickingPos.LabOrder_PickingPos.Any())
                    return false;
            }

            return true;
        }

        #endregion

        #region Navigation

        [ACMethodInteraction("Picking", "en{'Show Visitor voucher'}de{'Besucherbeleg anzeigen'}", 633, false, "SelectedPicking")]
        public void NavigateToVisitorVoucher()
        {
            if (!IsEnabledNavigateToVisitorVoucher())
                return;
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = SelectedPicking.VisitorVoucher.VisitorVoucherID,
                    EntityName = VisitorVoucher.ClassName
                });
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToVisitorVoucher()
        {
            return SelectedPicking != null && SelectedPicking.VisitorVoucher != null;
        }

        #endregion

        #region ShowParamDialog

        [ACMethodCommand(nameof(ShowParamDialog), ConstApp.PrefParam, 656, true)]
        public void ShowParamDialog()
        {
            if (!IsEnabledShowParamDialog())
                return;

            BSOPreferredParameters_Child.Value.ShowParamDialog(
                ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF.ACClassWFID,
                null,
                null,
                CurrentPicking.PickingID);
        }

        public bool IsEnabledShowParamDialog()
        {
            return
                ProcessWorkflowPresenter != null
                && ProcessWorkflowPresenter.SelectedWFNode != null
                && ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF != null;
        }

        #endregion

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(AssignProdOrderPartslistPos):
                    AssignProdOrderPartslistPos();
                    return true;
                case nameof(IsEnabledAssignProdOrderPartslistPos):
                    result = IsEnabledAssignProdOrderPartslistPos();
                    return true;
                case nameof(FilterDialogProdOrderPartslistPos):
                    result = FilterDialogProdOrderPartslistPos();
                    return true;
                case nameof(ShowDlgInwardFacility):
                    ShowDlgInwardFacility();
                    return true;
                case nameof(IsEnabledShowDlgInwardFacility):
                    result = IsEnabledShowDlgInwardFacility();
                    return true;
                case nameof(ShowDlgOutwardFacility):
                    ShowDlgOutwardFacility();
                    return true;
                case nameof(IsEnabledShowDlgOutwardFacility):
                    result = IsEnabledShowDlgOutwardFacility();
                    return true;
                case nameof(ShowDlgFilterFromFacility):
                    ShowDlgFilterFromFacility();
                    return true;
                case nameof(IsEnabledShowDlgFilterFromFacility):
                    result = IsEnabledShowDlgFilterFromFacility();
                    return true;
                case nameof(ShowDlgFilterToFacility):
                    ShowDlgFilterToFacility();
                    return true;
                case nameof(IsEnabledShowDlgFilterToFacility):
                    result = IsEnabledShowDlgFilterToFacility();
                    return true;
                case nameof(OnActivate):
                    OnActivate((System.String)acParameter[0]);
                    return true;
                case nameof(ValidateRoutes):
                    ValidateRoutes();
                    return true;
                case nameof(IsEnabledValidateRoutes):
                    result = IsEnabledValidateRoutes();
                    return true;
                case nameof(StartWorkflow):
                    StartWorkflow();
                    return true;
                case nameof(IsEnabledStartWorkflow):
                    result = IsEnabledStartWorkflow();
                    return true;
                case nameof(DialogOK):
                    DialogOK();
                    return true;
                case nameof(DialogCancel):
                    DialogCancel();
                    return true;
                case nameof(ShowDialogOrder):
                    ShowDialogOrder((System.String)acParameter[0], (System.Guid)acParameter[1]);
                    return true;
                case nameof(ShowDialogOrderInfo):
                    ShowDialogOrderInfo((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(OnTrackingCall):
                    OnTrackingCall((TrackingAndTracingSearchModel)acParameter[0], (gip.core.datamodel.IACObject)acParameter[1], (System.Object)acParameter[2], (TrackingEnginesEnum)acParameter[3]);
                    return true;
                case nameof(FilterClear):
                    FilterClear();
                    return true;
                case nameof(IsEnabledFilterClear):
                    result = IsEnabledFilterClear();
                    return true;
                case nameof(ProcessWorkflowAssign):
                    ProcessWorkflowAssign();
                    return true;
                case nameof(IsEnabledProcessWorkflowAssign):
                    result = IsEnabledProcessWorkflowAssign();
                    return true;
                case nameof(ProcessWorkflowCancel):
                    ProcessWorkflowCancel();
                    return true;
                case nameof(IsEnabledProcessWorkflowCancel):
                    result = IsEnabledProcessWorkflowCancel();
                    return true;
                case nameof(AssignInOrderPos):
                    AssignInOrderPos();
                    return true;
                case nameof(IsEnabledAssignInOrderPos):
                    result = IsEnabledAssignInOrderPos();
                    return true;
                case nameof(FilterDialogInOrderPos):
                    result = FilterDialogInOrderPos();
                    return true;
                case nameof(AssignDNoteInOrderPos):
                    AssignDNoteInOrderPos();
                    return true;
                case nameof(IsEnabledAssignDNoteInOrderPos):
                    result = IsEnabledAssignDNoteInOrderPos();
                    return true;
                case nameof(AssignOutOrderPos):
                    AssignOutOrderPos();
                    return true;
                case nameof(IsEnabledAssignOutOrderPos):
                    result = IsEnabledAssignOutOrderPos();
                    return true;
                case nameof(FilterDialogOutOrderPos):
                    result = FilterDialogOutOrderPos();
                    return true;
                case nameof(AssignDNoteOutOrderPos):
                    AssignDNoteOutOrderPos();
                    return true;
                case nameof(IsEnabledAssignDNoteOutOrderPos):
                    result = IsEnabledAssignDNoteOutOrderPos();
                    return true;
                case nameof(Save):
                    Save();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(CancelPicking):
                    CancelPicking();
                    return true;
                case nameof(IsEnabledCancelPicking):
                    result = IsEnabledCancelPicking();
                    return true;
                case nameof(MirrorPicking):
                    MirrorPicking();
                    return true;
                case nameof(IsEnabledMirrorPicking):
                    result = IsEnabledMirrorPicking();
                    return true;
                case nameof(GenerateSubPickingsForSupply):
                    GenerateSubPickingsForSupply();
                    return true;
                case nameof(IsEnabledGenerateSubPickingsForSupply):
                    result = IsEnabledGenerateSubPickingsForSupply();
                    return true;
                case nameof(UnassignPickingPos):
                    UnassignPickingPos();
                    return true;
                case nameof(IsEnabledUnassignPickingPos):
                    result = IsEnabledUnassignPickingPos();
                    return true;
                case nameof(AddPickingPos):
                    AddPickingPos();
                    return true;
                case nameof(IsEnabledAddPickingPos):
                    result = IsEnabledAddPickingPos();
                    return true;
                case nameof(ShowDlgFromFacility):
                    ShowDlgFromFacility();
                    return true;
                case nameof(IsEnabledShowDlgFromFacility):
                    result = IsEnabledShowDlgFromFacility();
                    return true;
                case nameof(ShowDlgToFacility):
                    ShowDlgToFacility();
                    return true;
                case nameof(IsEnabledShowDlgToFacility):
                    result = IsEnabledShowDlgToFacility();
                    return true;
                case nameof(NewFacilityPreBooking):
                    NewFacilityPreBooking();
                    return true;
                case nameof(IsEnabledNewFacilityPreBooking):
                    result = IsEnabledNewFacilityPreBooking();
                    return true;
                case nameof(CancelFacilityPreBooking):
                    CancelFacilityPreBooking();
                    return true;
                case nameof(IsEnabledCancelFacilityPreBooking):
                    result = IsEnabledCancelFacilityPreBooking();
                    return true;
                case nameof(DeleteFacilityPreBooking):
                    DeleteFacilityPreBooking();
                    return true;
                case nameof(IsEnabledDeleteFacilityPreBooking):
                    result = IsEnabledDeleteFacilityPreBooking();
                    return true;
                case nameof(BookDeliveryPos):
                    BookDeliveryPos();
                    return true;
                case nameof(IsEnabledBookDeliveryPos):
                    result = IsEnabledBookDeliveryPos();
                    return true;
                case nameof(BookCurrentACMethodBooking):
                    BookCurrentACMethodBooking();
                    return true;
                case nameof(IsEnabledBookCurrentACMethodBooking):
                    result = IsEnabledBookCurrentACMethodBooking();
                    return true;
                case nameof(BookAllACMethodBookings):
                    BookAllACMethodBookings();
                    return true;
                case nameof(IsEnabledBookAllACMethodBookings):
                    result = IsEnabledBookAllACMethodBookings();
                    return true;
                case nameof(NewFacilityLot):
                    NewFacilityLot();
                    return true;
                case nameof(IsEnabledNewFacilityLot):
                    result = IsEnabledNewFacilityLot();
                    return true;
                case nameof(ShowFacilityLot):
                    ShowFacilityLot();
                    return true;
                case nameof(IsEnabledShowFacilityLot):
                    result = IsEnabledShowFacilityLot();
                    return true;
                case nameof(ShowDlgInwardAvailableQuants):
                    ShowDlgInwardAvailableQuants();
                    return true;
                case nameof(IsEnabledShowDlgInwardAvailableQuants):
                    result = IsEnabledShowDlgInwardAvailableQuants();
                    return true;
                case nameof(ShowDlgOutwardAvailableQuants):
                    ShowDlgOutwardAvailableQuants();
                    return true;
                case nameof(IsEnabledShowDlgOutwardAvailableQuants):
                    result = IsEnabledShowDlgOutwardAvailableQuants();
                    return true;
                case nameof(DlgAvailableQuantsOk):
                    DlgAvailableQuantsOk();
                    return true;
                case nameof(IsEnabledDlgAvailableQuantsOk):
                    result = IsEnabledDlgAvailableQuantsOk();
                    return true;
                case nameof(DlgAvailableQuantsCancel):
                    DlgAvailableQuantsCancel();
                    return true;
                case nameof(FinishOrder):
                    FinishOrder();
                    return true;
                case nameof(IsEnabledFinishOrder):
                    result = IsEnabledFinishOrder();
                    return true;
                case nameof(RegisterWeight):
                    RegisterWeight();
                    return true;
                case nameof(IsEnabledRegisterWeight):
                    result = IsEnabledRegisterWeight();
                    return true;
                case nameof(BroadCastPicking):
                    BroadCastPicking();
                    return true;
                case nameof(IsEnabledBroadCastPicking):
                    result = IsEnabledBroadCastPicking();
                    return true;
                case nameof(CreateNewLabOrder):
                    CreateNewLabOrder();
                    return true;
                case nameof(IsEnabledCreateNewLabOrder):
                    result = IsEnabledCreateNewLabOrder();
                    return true;
                case nameof(NavigateToVisitorVoucher):
                    NavigateToVisitorVoucher();
                    return true;
                case nameof(IsEnabledNavigateToVisitorVoucher):
                    result = IsEnabledNavigateToVisitorVoucher();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region Workflow

        #region Workflow -> private 
        private VBPresenterMethod _presenter = null;
        #endregion

        #region Workflow -> Properties
        private bool _ProcessWorkflowShowAllWorkflows = true;
        [ACPropertyInfo(627, "InputMaterials", "en{'All Workflows'}de{'Alle Steuerrezepte'}")]
        public bool ProcessWorkflowShowAllWorkflows
        {
            get
            {
                return _ProcessWorkflowShowAllWorkflows;
            }
            set
            {
                if (_ProcessWorkflowShowAllWorkflows = value)
                {
                    _ProcessWorkflowShowAllWorkflows = value;
                    OnPropertyChanged(nameof(ProcessWorkflowShowAllWorkflows));
                    _ProcessWorkflowList = null;
                    OnPropertyChanged(nameof(ProcessWorkflowList));
                }
            }
        }

        bool _PresenterRightsChecked = false;
        public VBPresenterMethod ProcessWorkflowPresenter
        {
            get
            {
                if (_presenter == null)
                {
                    _presenter = this.ACUrlCommand("VBPresenterMethod(CurrentDesign)") as VBPresenterMethod;
                    if (_presenter == null && !_PresenterRightsChecked)
                        Messages.Error(this, "This user has no rights for viewing workflows. Assign rights for VBPresenterMethod in the group management!", true);
                    _PresenterRightsChecked = true;
                }
                return _presenter;
            }
        }

        private gipCoreData.ACClassMethod _CurrentProcessWorkflow;
        [ACPropertySelected(628, "", "en{'Process Workflow'}de{'Prozess-Workflow'}")]
        public gipCoreData.ACClassMethod CurrentProcessWorkflow
        {
            get => _CurrentProcessWorkflow;
            set
            {
                _CurrentProcessWorkflow = value;
                OnPropertyChanged();
            }
        }

        private gipCoreData.ACClassMethod _SelectedProcessWorkflow;
        /// <summary>
        /// Selected property for ACClassMethod
        /// </summary>
        /// <value>The selected ProcessWorkflow</value>
        [ACPropertySelected(628, "ProcessWorkflow", "en{'Process Workflow'}de{'Prozess-Workflow'}")]
        public gipCoreData.ACClassMethod SelectedProcessWorkflow
        {
            get
            {
                return _SelectedProcessWorkflow;
            }
            set
            {
                if (_SelectedProcessWorkflow != value)
                {
                    _SelectedProcessWorkflow = value;
                    if (ProcessWorkflowPresenter != null)
                    {
                        if (CurrentPicking != null)
                        {
                            LoadProcessWorkflowPresenter();
                        }
                    }
                    OnPropertyChanged(nameof(SelectedProcessWorkflow));
                }
            }
        }

        private List<gipCoreData.ACClassMethod> _ProcessWorkflowList;
        /// <summary>
        /// List property for ACClassMethod
        /// </summary>
        /// <value>The ProcessWorkflow list</value>
        [ACPropertyList(629, "ProcessWorkflow")]
        public List<gipCoreData.ACClassMethod> ProcessWorkflowList
        {
            get
            {
                if (_ProcessWorkflowList == null)
                    _ProcessWorkflowList = LoadProcessWorkflowList();
                return _ProcessWorkflowList;
            }
        }

        private List<gipCoreData.ACClassMethod> LoadProcessWorkflowList()
        {
            string pwClassNameOfRoot = nameof(PWMethodTransportBase);

            List<gipCoreData.ACClassMethod> tempList = DatabaseApp.ContextIPlus.ACClassMethod
                                                                  .Where(c => c.ACKindIndex == (Int16)Global.ACKinds.MSWorkflow
                                                                           && c.ACClassWF_ACClassMethod
                                                                               .Any(wf => !wf.ParentACClassWFID.HasValue
                                                                                       && (wf.PWACClass.ACIdentifier == pwClassNameOfRoot
                                                                                       || (wf.PWACClass.BasedOnACClassID.HasValue
                                                                                       && (wf.PWACClass.ACClass1_BasedOnACClass.ACIdentifier == pwClassNameOfRoot
                                                                                       || (wf.PWACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                        && wf.PWACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pwClassNameOfRoot))))
                                                                                       && !wf.ACClassMethod.IsSubMethod))
                                                                  .ToArray()
                                                                  .OrderBy(c => c.ACCaption)
                                                                  .ThenBy(c => c.ACIdentifier)
                                                                  .ToList();

            MDMaterialGroup pickingGroup = DatabaseApp.MDMaterialGroup.FirstOrDefault(c => c.MDMaterialGroupIndex == (short)MDMaterialGroup.MaterialGroupTypes.Picking);

            MaterialWF materialWf = DatabaseApp.MaterialWF.Include(c => c.MaterialWFACClassMethod_MaterialWF)
                                                          .FirstOrDefault(c => c.MaterialWFRelation_MaterialWF
                                                                                .Any(x => x.SourceMaterial.MDMaterialGroupID == pickingGroup.MDMaterialGroupID
                                                                                       || x.TargetMaterial.MDMaterialGroupID == pickingGroup.MDMaterialGroupID));

            if (materialWf != null)
            {
                List<Guid> relatedACClassMethods = materialWf.MaterialWFACClassMethod_MaterialWF.OrderBy(x => x.ACClassMethod.ACCaption)
                                                                                                .ThenBy(x => x.ACClassMethod.ACIdentifier)
                                                                                                .Select(c => c.ACClassMethodID).ToList();
                if (relatedACClassMethods.Any())
                {
                    tempList = tempList.OrderBy(c =>
                                        {
                                            int index = relatedACClassMethods.IndexOf(c.ACClassMethodID);
                                            if (index == -1)
                                                index = tempList.IndexOf(c) + 10000;
                                            return index;
                                        }).ToList();
                }
            }

            return tempList;
        }

        #endregion

        #region Workflow -> Methods

        #region Workflow -> Methods -> Interaction
        [ACMethodInteraction("ProcessWorkflow", "en{'Add'}de{'Hinzufügen'}", (short)MISort.New, true, "CurrentProcessWorkflow")]
        public void ProcessWorkflowAssign()
        {
            if (!IsEnabledProcessWorkflowAssign())
                return;

            ShowDialog(this, "SelectWorkflowDialog");
        }

        public bool IsEnabledProcessWorkflowAssign()
        {
            return CurrentPicking != null;
        }

        [ACMethodInteraction("ProcessWorkflow", "en{'Add'}de{'Hinzufügen'}", (short)MISort.New, true, "CurrentProcessWorkflow")]
        public void ProcessWorkflowAssignOk()
        {
            CurrentPicking.ACClassMethod = SelectedProcessWorkflow.FromAppContext<gip.mes.datamodel.ACClassMethod>(DatabaseApp);
            CurrentProcessWorkflow = SelectedProcessWorkflow;
            OnPropertyChanged(nameof(IsEnabledProcessWorkflowAssign));
            OnPropertyChanged(nameof(IsEnabledProcessWorkflowCancel));

            LoadProcessWorkflowPresenter();
            CloseTopDialog();
        }

        private void LoadProcessWorkflowPresenter()
        {
            if (ProcessWorkflowPresenter != null)
            {
                gipCoreData.ACClassMethod method = (CurrentPicking != null && CurrentPicking.ACClassMethod != null) ?
                                                   CurrentPicking.ACClassMethod.FromIPlusContext<gipCoreData.ACClassMethod>(DatabaseApp.ContextIPlus) :
                                                   null;
                ProcessWorkflowPresenter.Load(method);
            }
        }

        public bool IsEnabledProcessWorkflowAssignOk()
        {
            return SelectedProcessWorkflow != null;
        }

        [ACMethodInteraction("ProcessWorkflow", "en{'Remove'}de{'Entfernen'}", (short)MISort.Delete, true, "CurrentProcessWorkflow")]
        public void ProcessWorkflowCancel()
        {
            if (!IsEnabledProcessWorkflowCancel()) return;
            CurrentPicking.ACClassMethod = null;
            CurrentPicking.ACClassMethodID = null;

            if (CurrentProcessWorkflow != SelectedProcessWorkflow)
                SelectedProcessWorkflow = CurrentProcessWorkflow;

            SelectedProcessWorkflow = null;
            CurrentProcessWorkflow = null;
            OnPropertyChanged(nameof(IsEnabledProcessWorkflowAssign));
            OnPropertyChanged(nameof(IsEnabledProcessWorkflowCancel));
        }

        public bool IsEnabledProcessWorkflowCancel()
        {
            return CurrentPicking != null && CurrentPicking.ACClassMethodID != null;
        }

        #endregion

        #endregion

        #endregion

        #region IACBSOConfigStoreSelection

        public List<IACConfigStore> MandatoryConfigStores
        {
            get
            {
                List<IACConfigStore> listOfSelectedStores = new List<IACConfigStore>();
                if (CurrentPicking != null)
                {
                    listOfSelectedStores.Add(CurrentPicking);
                }
                return listOfSelectedStores;
            }
        }

        public IACConfigStore CurrentConfigStore
        {
            get
            {
                if (CurrentPicking == null) return null;
                return CurrentPicking;
            }
        }

        public bool IsReadonly
        {
            get
            {
                return false;
            }
        }

        private List<core.datamodel.ACClassMethod> _VisitedMethods;
        public List<core.datamodel.ACClassMethod> VisitedMethods
        {
            get
            {
                if (_VisitedMethods == null)
                    _VisitedMethods = new List<core.datamodel.ACClassMethod>();
                return _VisitedMethods;
            }
            set
            {
                _VisitedMethods = value;
                OnPropertyChanged(nameof(VisitedMethods));
            }
        }
        public void AddVisitedMethods(core.datamodel.ACClassMethod acClassMethod)
        {
            if (!VisitedMethods.Contains(acClassMethod))
                VisitedMethods.Add(acClassMethod);
        }

        #endregion

        #region IACBSOACProgramProvider

        private bool _IsEnabledACProgram = true;
        public bool IsEnabledACProgram
        {
            get
            {
                return _IsEnabledACProgram;
            }
            set
            {
                _IsEnabledACProgram = value;
            }
        }

        public string WorkflowACUrl
        {
            get
            {
                if (ProcessWorkflowPresenter == null)
                    return null;
                if (ProcessWorkflowPresenter.SelectedRootWFNode.ACIdentifier == ProcessWorkflowPresenter.SelectedACUrl)
                    return null;
                else
                    return ProcessWorkflowPresenter.SelectedWFNode.GetACUrl(ProcessWorkflowPresenter.SelectedRootWFNode);
            }
        }

        public IEnumerable<gipCoreData.ACProgram> GetACProgram()
        {
            return
                DatabaseApp
                .OrderLog
                .Where(c => c.PickingPos.PickingID == CurrentPicking.PickingID)
                .ToList()
                .Select(x => x.VBiACProgramLog.ACProgram.FromIPlusContext<gip.core.datamodel.ACProgram>(DatabaseApp.ContextIPlus))
                .Distinct();
        }

        public string GetProdOrderProgramNo()
        {
            return CurrentPicking.ACCaption;
        }

        public DateTime GetProdOrderInsertDate()
        {
            return CurrentPicking.InsertDate;
        }

        #endregion

        #region Messages
        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged(nameof(MsgList));
        }

        #region Messages -> Properties

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(9999, "Message", "en{'Message'}de{'Meldung'}")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged(nameof(CurrentMsg));
            }
        }

        private ObservableCollection<Msg> msgList;
        /// <summary>
        /// Gets the MSG list.
        /// </summary>
        /// <value>The MSG list.</value>
        [ACPropertyList(9999, "Message", "en{'Messagelist'}de{'Meldungsliste'}")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        #endregion

        #endregion

    }

}
