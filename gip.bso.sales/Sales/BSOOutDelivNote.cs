using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.sales
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'Sales Delivery Note'}de{'Lieferschein (Verkauf)'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + DeliveryNote.ClassName)]
    [ACQueryInfo(Const.PackName_VarioSales, Const.QueryPrefix + "OutOrderPosOpen", "en{'Open Sales Order Pos.'}de{'Offene Auftragsposition'}", typeof(OutOrderPos), OutOrderPos.ClassName, "MDDelivPosState\\MDDelivPosStateIndex", "TargetDeliveryDate,Material\\MaterialNo")]
    [ACQueryInfo(Const.PackName_VarioSales, Const.QueryPrefix + "BookingFacility", ConstApp.Facility, typeof(Facility), Facility.ClassName, "MDFacilityType\\MDFacilityTypeIndex", "FacilityNo")]
    [ACQueryInfo(Const.PackName_VarioSales, Const.QueryPrefix + "BookingFacilityLot", ConstApp.Lot, typeof(FacilityLot), FacilityLot.ClassName, "LotNo", "LotNo")]
    public class BSOOutDeliveryNote : ACBSOvbNav
    {
        #region c´tors

        public BSOOutDeliveryNote(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _OutDeliveryNoteManager = ACOutDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_OutDeliveryNoteManager == null)
                throw new Exception("OutDeliveryNoteManager not configured");

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _PickingManager = ACPickingManager.ACRefToServiceInstance(this);
            if (_PickingManager == null)
                throw new Exception("PickingManager not configured");

            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACOutDeliveryNoteManager.DetachACRefFromServiceInstance(this, _OutDeliveryNoteManager);
            _OutDeliveryNoteManager = null;
            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            if (_PickingManager != null)
            {
                ACPickingManager.DetachACRefFromServiceInstance(this, _PickingManager);
                _PickingManager = null;
            }

            this._AccessBookingFacility = null;
            this._AccessDeliveryNotePos = null;
            this._AccessOutOrderPos = null;
            this._BSOMsg = null;
            this._CurrentACMethodBookingDummy = null;
            this._CurrentDeliveryNotePos = null;
            this._CurrentFacilityBooking = null;
            this._CurrentFacilityPreBooking = null;
            this._CurrentOutOrderPosFromPicking = null;
            this._PartialQuantity = null;
            this._SelectedDeliveryNotePos = null;
            this._SelectedFacilityBooking = null;
            this._SelectedFacilityBookingCharge = null;
            this._SelectedFacilityPreBooking = null;
            this._SelectedOutOrderPosFromPicking = null;
            this._StateCompletelyAssigned = null;
            this._UnSavedAssignedPickingOutOrderPos = null;
            this._UnSavedUnAssignedOutOrderPos = null;
            _PreBookingAvailableQuantsList = null;
            _SelectedPreBookingAvailableQuants = null;
            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessDeliveryNotePos != null)
            {
                _AccessDeliveryNotePos.ACDeInit(false);
                _AccessDeliveryNotePos = null;
            }
            if (_AccessOutOrderPos != null)
            {
                _AccessOutOrderPos.ACDeInit(false);
                _AccessOutOrderPos = null;
            }
            if (_AccessBookingFacility != null)
            {
                _AccessBookingFacility.ACDeInit(false);
                _AccessBookingFacility = null;
            }
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            if (_AccessBookingFacilityLot != null)
            {
                _AccessBookingFacilityLot.NavSearchExecuted -= _AccessBookingFacilityLot_NavSearchExecuted;
                _AccessBookingFacilityLot.ACDeInit(false);
                _AccessBookingFacilityLot = null;
            }

            _QuantDialogMaterial = null;
            return b;
        }

        #endregion

        #region Managers

        protected ACRef<ACOutDeliveryNoteManager> _OutDeliveryNoteManager = null;
        protected ACOutDeliveryNoteManager OutDeliveryNoteManager
        {
            get
            {
                if (_OutDeliveryNoteManager == null)
                    return null;
                return _OutDeliveryNoteManager.ValueT;
            }
        }

        protected ACRef<ACComponent> _ACFacilityManager = null;
        protected FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }

        private ACRef<ACPickingManager> _PickingManager;
        protected ACPickingManager PickingManager
        {
            get
            {
                return _PickingManager?.ValueT;
            }
        }

        #endregion

        #region BSO->ACProperty

        #region DeliveryNote
        protected IQueryable<DeliveryNote> _AccessPrimary_NavSearchExecuting(IQueryable<DeliveryNote> result)
        {
            IQueryable<DeliveryNote> query = result as IQueryable<DeliveryNote>;
            if (query != null)
            {
                query.Include(c => c.MDDelivNoteState)
                    .Include(c => c.TourplanPos)
                    .Include(c => c.VisitorVoucher)
                    .Include(c => c.DeliveryCompanyAddress)
                    .Include(c => c.ShipperCompanyAddress)
                    .Include(c => c.Delivery2CompanyAddress)
                    .Include("DeliveryNotePos_DeliveryNote")
                    .Include("DeliveryNotePos_DeliveryNote.OutOrderPos")
                    .Include("DeliveryNotePos_DeliveryNote.OutOrderPos.Material");
            }
            if (FilterDelivNoteState.HasValue)
                result = result.Where(x => x.MDDelivNoteState.MDDelivNoteStateIndex == (short)FilterDelivNoteState.Value);
            return result;
        }

        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        ACAccessNav<DeliveryNote> _AccessPrimary;
        [ACPropertyAccessPrimary(690, DeliveryNote.ClassName)]
        public ACAccessNav<DeliveryNote> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(NavigationqueryDefaultSort);
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<DeliveryNote>(DeliveryNote.ClassName, this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessPrimary;
            }
        }

        public List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();
                aCFilterItems.Add(new ACFilterItem(Global.FilterTypes.filter, "DeliveryNoteNo", Global.LogicalOperators.contains, Global.Operators.and, "", true, true));
                aCFilterItems.Add(new ACFilterItem(Global.FilterTypes.filter, "DeliveryNoteTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)GlobalApp.DeliveryNoteType.Issue).ToString(), true));
                aCFilterItems.Add(new ACFilterItem(Global.FilterTypes.filter, "Comment", Global.LogicalOperators.contains, Global.Operators.and, "", false, false));
                return aCFilterItems;
            }
        }

        private List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();
                acSortItems.Add(new ACSortItem("DeliveryNoteNo", Global.SortDirections.descending, true));
                return acSortItems;
            }
        }


        [ACPropertyCurrent(600, DeliveryNote.ClassName)]
        public DeliveryNote CurrentDeliveryNote
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentDeliveryNote");
                OnPropertyChanged("DeliveryNotePosList");
                RefreshOutOrderPosList();
            }
        }

        [ACPropertyList(601, DeliveryNote.ClassName)]
        public IEnumerable<DeliveryNote> DeliveryNoteList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, DeliveryNote.ClassName)]
        public DeliveryNote SelectedDeliveryNote
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedDeliveryNote");
            }
        }
        #endregion

        #region DeliveryNotePos
        ACAccess<DeliveryNotePos> _AccessDeliveryNotePos;
        [ACPropertyAccess(691, "DeliveryNotePos")]
        public ACAccess<DeliveryNotePos> AccessDeliveryNotePos
        {
            get
            {
                if (_AccessDeliveryNotePos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + DeliveryNotePos.ClassName) as ACQueryDefinition;
                    _AccessDeliveryNotePos = acQueryDefinition.NewAccess<DeliveryNotePos>("DeliveryNotePos", this);
                }
                return _AccessDeliveryNotePos;
            }
        }

        DeliveryNotePos _CurrentDeliveryNotePos;
        [ACPropertyCurrent(603, "DeliveryNotePos")]
        public DeliveryNotePos CurrentDeliveryNotePos
        {
            get
            {
                return _CurrentDeliveryNotePos;
            }
            set
            {
                _CurrentDeliveryNotePos = value;
                OnPropertyChanged("CurrentDeliveryNotePos");
                OnPropertyChanged("FacilityPreBookingList");
                if (FacilityPreBookingList != null)
                    SelectedFacilityPreBooking = FacilityPreBookingList.FirstOrDefault();
                RefreshFilterFacilityAccess();
                if (AccessBookingFacilityLot != null)
                    RefreshFilterFacilityLotAccess(_AccessBookingFacilityLot);
                OnPropertyChanged("BookingFacilityList");
                OnPropertyChanged("FacilityBookingList");
            }
        }

        [ACPropertyList(604, "DeliveryNotePos")]
        public IEnumerable<DeliveryNotePos> DeliveryNotePosList
        {
            get
            {
                if (CurrentDeliveryNote == null)
                    return null;
                return CurrentDeliveryNote.DeliveryNotePos_DeliveryNote.ToList();

            }
        }

        DeliveryNotePos _SelectedDeliveryNotePos;
        [ACPropertySelected(605, "DeliveryNotePos")]
        public DeliveryNotePos SelectedDeliveryNotePos
        {
            get
            {
                return _SelectedDeliveryNotePos;
            }
            set
            {
                if (_SelectedDeliveryNotePos != value)
                {
                    _SelectedDeliveryNotePos = value;
                    if (_SelectedDeliveryNotePos != null && _SelectedDeliveryNotePos.OutOrderPos != null)
                        _SelectedDeliveryNotePos.OutOrderPos.LabOrder_OutOrderPos.AutoLoad(_SelectedDeliveryNotePos.OutOrderPos.LabOrder_OutOrderPosReference, _SelectedDeliveryNotePos);
                    OnPropertyChanged("SelectedDeliveryNotePos");
                }
                CurrentDeliveryNotePos = value;
            }
        }
        #endregion

        #region Open Order lines
        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<OutOrderPos> _AccessOutOrderPos;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(692, OutOrderPos.ClassName)]
        public ACAccessNav<OutOrderPos> AccessOutOrderPos
        {
            get
            {
                if (_AccessOutOrderPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDef = Root.Queries.CreateQuery(null, Const.QueryPrefix + "OutOrderPosOpen", ACType.ACIdentifier);
                    if (acQueryDef != null)
                    {
                        acQueryDef.CheckAndReplaceColumnsIfDifferent(AccessOutOrderPosDefaultFilter, AccessOutOrderPosDefaultSort, true, true);
                    }

                    _AccessOutOrderPos = acQueryDef.NewAccessNav<OutOrderPos>(OutOrderPos.ClassName, this);
                    _AccessOutOrderPos.AutoSaveOnNavigation = false;
                    _AccessOutOrderPos.NavSearch(DatabaseApp);
                }
                return _AccessOutOrderPos;
            }
        }


        protected virtual List<ACFilterItem> AccessOutOrderPosDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "MDDelivPosState\\MDDelivPosStateIndex", Global.LogicalOperators.lessThan, Global.Operators.and, ((short) MDDelivPosState.DelivPosStates.CompletelyAssigned).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                        new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.or, null, true),
                            new ACFilterItem(Global.FilterTypes.filter, "OutOrderPos1_ParentOutOrderPos", Global.LogicalOperators.isNull, Global.Operators.and, "", true),
                            new ACFilterItem(Global.FilterTypes.filter, OutOrder.ClassName + "\\" + MDOutOrderType.ClassName + "\\OrderTypeIndex", Global.LogicalOperators.notEqual, Global.Operators.and, ((short) GlobalApp.OrderTypes.Contract).ToString(), true),
                        new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.or, null, true),
                        new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.or, null, true),
                            new ACFilterItem(Global.FilterTypes.filter, "OutOrderPos1_ParentOutOrderPos", Global.LogicalOperators.isNotNull, Global.Operators.and, "", true),
                            new ACFilterItem(Global.FilterTypes.filter, "OutOrderPos1_ParentOutOrderPos\\" + OutOrder.ClassName + "\\" + MDOutOrderType.ClassName + "\\OrderTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short) GlobalApp.OrderTypes.Contract).ToString(), true),
                        new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.or, null, true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, _CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CTransportModeProperty, Global.LogicalOperators.equal, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, _CMaterialNoProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CMaterialNameProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                };
            }
        }

        protected virtual List<ACSortItem> AccessOutOrderPosDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("TargetDeliveryDate", Global.SortDirections.ascending, true),
                    new ACSortItem("Material\\MaterialNo", Global.SortDirections.ascending, true),
                };
            }
        }


        /// <summary>
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(606, OutOrderPos.ClassName)]
        public OutOrderPos CurrentOutOrderPos
        {
            get
            {
                if (AccessOutOrderPos == null)
                    return null;
                return AccessOutOrderPos.Current;
            }
            set
            {
                AccessOutOrderPos.Current = value;
                OnPropertyChanged("CurrentOutOrderPos");
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

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(607, OutOrderPos.ClassName)]
        public IEnumerable<OutOrderPos> OutOrderPosList
        {
            get
            {
                if (AccessOutOrderPos == null)
                    return null;
                if (CurrentDeliveryNote != null)
                {
                    IEnumerable<OutOrderPos> addedPositions = CurrentDeliveryNote.DeliveryNotePos_DeliveryNote.Where(c => c.EntityState == EntityState.Added
                        && c.OutOrderPos != null
                        && c.OutOrderPos.OutOrderPos1_ParentOutOrderPos != null
                        && c.OutOrderPos.OutOrderPos1_ParentOutOrderPos.MDDelivPosState == StateCompletelyAssigned
                        ).Select(c => c.OutOrderPos.OutOrderPos1_ParentOutOrderPos);
                    if (addedPositions.Any())
                    {
                        if (_UnSavedUnAssignedOutOrderPos.Any())
                            return AccessOutOrderPos.NavList.Except(addedPositions).Union(_UnSavedUnAssignedOutOrderPos);
                        else
                            return AccessOutOrderPos.NavList.Except(addedPositions);
                    }
                    else if (_UnSavedUnAssignedOutOrderPos.Any())
                    {
                        return AccessOutOrderPos.NavList.Union(_UnSavedUnAssignedOutOrderPos);
                    }
                }
                return AccessOutOrderPos.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(608, OutOrderPos.ClassName)]
        public OutOrderPos SelectedOutOrderPos
        {
            get
            {
                if (AccessOutOrderPos == null)
                    return null;
                return AccessOutOrderPos.Selected;
            }
            set
            {
                AccessOutOrderPos.Selected = value;
                OnPropertyChanged("SelectedOutOrderPos");
                CurrentOutOrderPos = value;
            }
        }

        #region Filter
        public const string _CMaterialNoProperty = Material.ClassName + "\\MaterialNo";
        public const string _CMaterialNameProperty = Material.ClassName + "\\MaterialName1";
        [ACPropertyInfo(713, "Filter", "en{'Material'}de{'Material'}")]
        public string FilterMaterial
        {
            get
            {
                return AccessOutOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CMaterialNoProperty);
            }
            set
            {
                string tmp = AccessOutOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CMaterialNoProperty);
                if (tmp != value)
                {
                    AccessOutOrderPos.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNoProperty, value);
                    AccessOutOrderPos.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNameProperty, value);
                    OnPropertyChanged("FilterMaterial");
                }
            }
        }


        [ACPropertyList(616, "TransportMode")]
        public IEnumerable<MDTransportMode> MDTransportModeList
        {
            get
            {
                return DatabaseApp.MDTransportMode.ToList();
            }
        }


        public const string _CTransportModeProperty = "MDTransportMode\\MDKey";
        [ACPropertySelected(617, "TransportMode", "en{'Filter mode of transport'}de{'Filter Transportart'}")]
        public MDTransportMode FilterTransportMode
        {
            get
            {
                string tmp = AccessOutOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CTransportModeProperty, Global.LogicalOperators.equal);
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return DatabaseApp.MDTransportMode.Where(c => c.MDKey == tmp).FirstOrDefault();
            }

            set
            {
                string tmp = AccessOutOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CTransportModeProperty, Global.LogicalOperators.equal);
                if (value == null)
                {
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        AccessOutOrderPos.NavACQueryDefinition.SetSearchValue(_CTransportModeProperty, Global.LogicalOperators.equal, "");
                        OnPropertyChanged("FilterTransportMode");
                    }
                }
                else
                {
                    if (tmp != value.MDKey)
                    {
                        AccessOutOrderPos.NavACQueryDefinition.SetSearchValue(_CTransportModeProperty, Global.LogicalOperators.equal, value.MDKey);
                        OnPropertyChanged("FilterTransportMode");
                    }
                }
            }
        }


        public const string _CTargetDeliveryDateProperty = "TargetDeliveryDate";

        [ACPropertyInfo(618, "", "en{'Deliv.date from'}de{'Lieferdatum von'}")]
        public DateTime? FilterDelivDateFrom
        {
            get
            {
                string tmp = AccessOutOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessOutOrderPos.NavACQueryDefinition.GetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual);
            }
            set
            {
                string tmp = AccessOutOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                {
                    if (value.HasValue)
                    {
                        AccessOutOrderPos.NavACQueryDefinition.SetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual, value.Value);
                        OnPropertyChanged("FilterDelivDateFrom");
                    }
                }
                else
                {
                    if (value.HasValue)
                    {
                        DateTime tmpdt = AccessOutOrderPos.NavACQueryDefinition.GetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual);
                        if (tmpdt != value.Value)
                        {
                            AccessOutOrderPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual, value.Value);
                            OnPropertyChanged("FilterDelivDateFrom");
                        }
                    }
                    else
                    {
                        AccessOutOrderPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual, "");
                        OnPropertyChanged("FilterDelivDateFrom");
                    }
                }
            }
        }

        [ACPropertyInfo(619, "", "en{'Deliv.date to'}de{'Lieferdatum bis'}")]
        public DateTime? FilterDelivDateTo
        {
            get
            {
                string tmp = AccessOutOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessOutOrderPos.NavACQueryDefinition.GetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual);
            }
            set
            {
                string tmp = AccessOutOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                {
                    if (value.HasValue)
                    {
                        AccessOutOrderPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual, value.Value);
                        OnPropertyChanged("FilterDelivDateTo");
                    }
                }
                else
                {
                    if (value.HasValue)
                    {
                        DateTime tmpdt = AccessOutOrderPos.NavACQueryDefinition.GetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual);
                        if (tmpdt != value)
                        {
                            AccessOutOrderPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual, value.Value);
                            OnPropertyChanged("FilterDelivDateTo");
                        }
                    }
                    else
                    {
                        AccessOutOrderPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual, "");
                        OnPropertyChanged("FilterDelivDateTo");
                    }
                }
            }
        }
        #endregion


        #endregion

        #region OutOrderPos from Picking
        OutOrderPos _CurrentOutOrderPosFromPicking;
        /// <summary>
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(609, "OutOrderPosFromPicking")]
        public OutOrderPos CurrentOutOrderPosFromPicking
        {
            get
            {
                return _CurrentOutOrderPosFromPicking;
            }
            set
            {
                _CurrentOutOrderPosFromPicking = value;
                OnPropertyChanged("CurrentOutOrderPosFromPicking");
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(610, "OutOrderPosFromPicking")]
        public IEnumerable<OutOrderPos> OutOrderPosFromPickingList
        {
            get
            {
                var query = DatabaseApp.PickingPos.Where(c => (c.Picking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.IssueVehicle
                                                            || c.Picking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.Issue)
                                                          && c.OutOrderPos != null
                                                          && !c.OutOrderPos.OutOrderPos1_ParentOutOrderPos.DeliveryNotePos_OutOrderPos.Any())
                                             .Select(c => c.OutOrderPos.OutOrderPos1_ParentOutOrderPos)
                                             .ToList().Distinct(); // Distinct auf Clientseite ausführen lassen (nach ToList), weil SQL-Server Abfrage nicht auswerten kann wenn Distinct vorher aufgerufen wird (= Serverseitig ausgeführt werden soll)
                if (query.Any())
                {
                    if (_UnSavedAssignedPickingOutOrderPos.Count <= 0)
                        return query;
                    else
                        return query.Except(_UnSavedAssignedPickingOutOrderPos);
                }
                else
                    return _UnSavedAssignedPickingOutOrderPos;
            }
        }

        OutOrderPos _SelectedOutOrderPosFromPicking;
        /// <summary>
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(611, "OutOrderPosFromPicking")]
        public OutOrderPos SelectedOutOrderPosFromPicking
        {
            get
            {
                return _SelectedOutOrderPosFromPicking;
            }
            set
            {
                _SelectedOutOrderPosFromPicking = value;
                OnPropertyChanged("SelectedOutOrderPosFromPicking");
            }
        }
        #endregion

        #region FacilityPreBooking
        FacilityPreBooking _CurrentFacilityPreBooking;
        [ACPropertyCurrent(612, "FacilityPreBooking")]
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
                    OnPropertyChanged("CurrentFacilityPreBooking");
                    OnPropertyChanged("CurrentACMethodBooking");
                    OnPropertyChanged("BookableFacilityLots");
                    RefreshFilterFacilityAccess();
                    if (AccessBookingFacilityLot != null)
                        RefreshFilterFacilityLotAccess(_AccessBookingFacilityLot);

                    if (_CurrentFacilityPreBooking != null && _CurrentFacilityPreBooking.ACMethodBooking != null)
                        _CurrentFacilityPreBooking.ACMethodBooking.PropertyChanged += ACMethodBooking_PropertyChanged;

                    OnPropertyChanged("BookingFacilityList");
                }
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(613, "FacilityPreBooking")]
        public IEnumerable<FacilityPreBooking> FacilityPreBookingList
        {
            get
            {
                if ((CurrentDeliveryNotePos == null) || (CurrentDeliveryNotePos.OutOrderPos == null))
                    return null;
                return CurrentDeliveryNotePos.OutOrderPos.FacilityPreBooking_OutOrderPos.ToList();
            }
        }

        FacilityPreBooking _SelectedFacilityPreBooking;
        [ACPropertySelected(614, "FacilityPreBooking")]
        public FacilityPreBooking SelectedFacilityPreBooking
        {
            get
            {
                return _SelectedFacilityPreBooking;
            }
            set
            {
                _SelectedFacilityPreBooking = value;
                OnPropertyChanged("SelectedFacilityPreBooking");
                CurrentFacilityPreBooking = value;
                OnPropertyChanged("BookingFacilityList");
                OnPropertyChanged("OuwardFacilityChargeList");
                if (AccessBookingFacilityLot != null)
                    AccessBookingFacilityLot.NavSearch(DatabaseApp);
            }
        }

        ACMethodBooking _CurrentACMethodBookingDummy = null; // Dummy-Parameter notwendig, damit Bindung an Oberfläche erfolgen kann, da abgeleitete Klasse
        [ACPropertyInfo(615, "", "en{'Posting Parameter'}de{'Buchungsparameter'}")]
        public ACMethodBooking CurrentACMethodBooking
        {
            get
            {
                if (CurrentFacilityPreBooking == null)
                {
                    if (_CurrentACMethodBookingDummy != null)
                        return _CurrentACMethodBookingDummy;
                    ACMethodBooking acMethodClone = OutDeliveryNoteManager?.BookParamOutwardMovementClone(this.ACFacilityManager, this.DatabaseApp);
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
                    CurrentFacilityPreBooking.ACMethodBooking = value;
                else
                    _CurrentACMethodBookingDummy = null;
                OnPropertyChanged("CurrentACMethodBooking");
            }
        }


        bool _UpdatingControlModeBooking = false;
        private void ACMethodBooking_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_UpdatingControlModeBooking)
                return;
            try
            {
                if (e.PropertyName == "OutwardFacility")
                {
                    _UpdatingControlModeBooking = true;
                    OnPropertyChanged("OutwardFacilityChargeList");
                    OnPropertyChanged("BookableFacilityLots");
                    if (CurrentACMethodBooking != null)
                    {
                        CurrentACMethodBooking.OnEntityPropertyChanged("OutwardFacility");
                        CurrentACMethodBooking.OnEntityPropertyChanged("OutwardFacilityCharge");
                        CurrentACMethodBooking.OnEntityPropertyChanged("OutwardFacilityLot");
                    }
                }
            }
            finally
            {
                _UpdatingControlModeBooking = false;
            }
        }


        private bool _BookingFilterMaterial = true;
        [ACPropertyInfo(617, "", "en{'Show only bins w. material'}de{'Nur Lagerplätze mit Mat. anzeigen'}")]
        public bool BookingFilterMaterial
        {
            get
            {
                return _BookingFilterMaterial;
            }
            set
            {
                _BookingFilterMaterial = value;
                OnPropertyChanged("BookingFilterMaterial");
                RefreshFilterFacilityAccess();
                OnPropertyChanged("BookingFacilityList");
                OnPropertyChanged("OutwardFacilityChargeList");
            }
        }

        [ACPropertyList(612, "OutwardFacilityCharge")]
        public IEnumerable<FacilityCharge> OutwardFacilityChargeList
        {
            get
            {
                if (CurrentACMethodBooking == null || CurrentACMethodBooking.OutwardFacility == null || CurrentDeliveryNotePos == null || CurrentDeliveryNotePos.Material == null)
                    return null;
                return CurrentACMethodBooking.OutwardFacility.FacilityCharge_Facility
                    .Where(x => x.MaterialID == CurrentDeliveryNotePos.Material.MaterialID && !x.NotAvailable).OrderByDescending(x => x.InsertDate);
            }
        }


        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<Facility> _AccessBookingFacility;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(693, "BookingFacility")]
        public ACAccessNav<Facility> AccessBookingFacility
        {
            get
            {
                if (_AccessBookingFacility == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "BookingFacility", ACType.ACIdentifier);
                    _AccessBookingFacility = navACQueryDefinition.NewAccessNav<Facility>("BookingFacility", this);
                    _AccessBookingFacility.AutoSaveOnNavigation = false;
                    RefreshFilterFacilityAccess();
                }
                return _AccessBookingFacility;
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
                    new ACFilterItem(Global.FilterTypes.filter, "Material\\MaterialNo", Global.LogicalOperators.equal, Global.Operators.or, "", true),
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

        [ACPropertyList(620, "BookingFacility")]
        public IList<Facility> BookingFacilityList
        {
            get
            {
                if (AccessBookingFacility == null)
                    return null;
                return AccessBookingFacility.NavList;
            }
        }

        private void RefreshFilterFacilityAccess()
        {
            if (AccessBookingFacility == null || CurrentFacilityPreBooking == null || FacilityPreBookingList == null || !FacilityPreBookingList.Any())
                return;

            if (BookingFilterMaterial)
            {
                AccessBookingFacility.NavACQueryDefinition.CheckAndReplaceColumnsIfDifferent(AccessBookingFacilityDefaultFilter_Material, AccessBookingFacilityDefaultSort);
                var acFilter = AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Where(c => c.ACIdentifier == "Material\\MaterialNo").FirstOrDefault();
                if (acFilter != null)
                    acFilter.SearchWord = CurrentDeliveryNotePos.Material.MaterialNo;
                var acFilter2 = AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Where(c => c.ACIdentifier == "Material\\MaterialNo" && c != acFilter).FirstOrDefault();
                if (acFilter2 != null)
                    acFilter2.SearchWord = CurrentDeliveryNotePos.Material.Material1_ProductionMaterial != null ? CurrentDeliveryNotePos.Material.Material1_ProductionMaterial.MaterialNo : "";
            }
            else
            {
                AccessBookingFacility.NavACQueryDefinition.CheckAndReplaceColumnsIfDifferent(AccessBookingFacilityDefaultFilter_StorageBin, AccessBookingFacilityDefaultSort);
            }
            AccessBookingFacility.NavSearch(this.DatabaseApp);
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
                bookableFacilityLots = FacilityPreBookingList.Where(c => c.OutwardFacilityLot != null).Select(c => c.OutwardFacilityLot).Distinct().ToList();

            if (FacilityBookingList != null && FacilityBookingList.Any())
            {
                var query2 = FacilityBookingList.Where(c => c.InwardFacilityLot != null).Select(c => c.OutwardFacilityLot).Distinct();
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
                || CurrentDeliveryNotePos == null)
                return;
            accessNavLot.NavACQueryDefinition.CheckAndReplaceColumnsIfDifferent(AccessBookingFacilityLotDefaultFilter, AccessBookingFacilityLotDefaultSort);

            var acFilter = accessNavLot.NavACQueryDefinition.ACFilterColumns.Where(c => c.ACIdentifier == "Material\\MaterialNo").FirstOrDefault();
            if (acFilter != null && CurrentDeliveryNotePos.Material != null)
                acFilter.SearchWord = CurrentDeliveryNotePos.Material.MaterialNo;

            accessNavLot.NavSearch(this.DatabaseApp);
        }

        #endregion

        #region  FacilityPreBooking -> Available quants

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
                    OnPropertyChanged("SelectedPreBookingAvailableQuants");
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
                .Take(Root.Environment.AccessDefaultTakeCount)
                .ToList();
        }

        private void LoadPreBookingAvailableQuants()
        {
            _PreBookingAvailableQuantsList = null;
            OnPropertyChanged("PreBookingAvailableQuantsList");
        }


        #endregion


        #region FacilityBooking
        FacilityBooking _CurrentFacilityBooking;
        [ACPropertyCurrent(623, FacilityBooking.ClassName)]
        public FacilityBooking CurrentFacilityBooking
        {
            get
            {
                return _CurrentFacilityBooking;
            }
            set
            {
                _CurrentFacilityBooking = value;
                OnPropertyChanged("CurrentFacilityBooking");
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(624, FacilityBooking.ClassName)]
        public IEnumerable<FacilityBooking> FacilityBookingList
        {
            get
            {
                if ((CurrentDeliveryNotePos == null) || (CurrentDeliveryNotePos.OutOrderPos == null))
                    return null;
                return CurrentDeliveryNotePos.OutOrderPos.FacilityBooking_OutOrderPos.OrderBy(c => c.FacilityBookingNo).ToList();
            }
        }

        FacilityBooking _SelectedFacilityBooking;
        [ACPropertySelected(625, FacilityBooking.ClassName)]
        public FacilityBooking SelectedFacilityBooking
        {
            get
            {
                return _SelectedFacilityBooking;
            }
            set
            {
                _SelectedFacilityBooking = value;
                OnPropertyChanged("SelectedFacilityBooking");
                CurrentFacilityBooking = value;
                OnPropertyChanged("FacilityBookingChargeList");
            }
        }
        #endregion

        #region FacilityBookingCharge

        [ACPropertyList(626, "FacilityBookingCharge")]
        public IEnumerable<FacilityBookingCharge> FacilityBookingChargeList
        {
            get
            {
                if (CurrentFacilityBooking != null)
                {
                    CurrentFacilityBooking.FacilityBookingCharge_FacilityBooking.AutoRefresh(CurrentFacilityBooking.FacilityBookingCharge_FacilityBookingReference, CurrentFacilityBooking);
                    return CurrentFacilityBooking.FacilityBookingCharge_FacilityBooking.OrderBy(c => c.FacilityBookingChargeNo).ToList();
                }
                return null;
            }
        }

        private FacilityBookingCharge _SelectedFacilityBookingCharge;
        [ACPropertySelected(627, "FacilityBookingCharge")]
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


        protected ACMethodBooking BookParamOutwardMovementClone
        {
            get
            {
                return OutDeliveryNoteManager.BookParamOutwardMovementClone(ACFacilityManager, DatabaseApp);
            }
        }

        protected ACMethodBooking BookParamOutCancelClone
        {
            get
            {
                return OutDeliveryNoteManager.BookParamOutCancelClone(ACFacilityManager, DatabaseApp);
            }
        }

        private List<OutOrderPos> _UnSavedUnAssignedOutOrderPos = new List<OutOrderPos>();
        private List<OutOrderPos> _UnSavedAssignedPickingOutOrderPos = new List<OutOrderPos>();

        Nullable<double> _PartialQuantity;
        [ACPropertyInfo(628, "", "en{'Partial Quantity'}de{'Teilmenge'}")]
        public Nullable<double> PartialQuantity
        {
            get
            {
                return _PartialQuantity;
            }
            set
            {
                _PartialQuantity = value;
                OnPropertyChanged("PartialQuantity");
            }
        }

        #endregion

        #region Message
        private MsgWithDetails _BSOMsg = new MsgWithDetails();
        [ACPropertyInfo(629, "Message")]
        public MsgWithDetails BSOMsg
        {
            get
            {
                return _BSOMsg;
            }
            set
            {
                _BSOMsg = value;
                OnPropertyChanged("BSOMsg");
            }
        }
        #endregion

        #region Properties -> FilterDelivNoteState

        public gip.mes.datamodel.MDDelivNoteState.DelivNoteStates? FilterDelivNoteState
        {
            get
            {
                if (SelectedFilterDelivNoteState == null) return null;
                return (gip.mes.datamodel.MDDelivNoteState.DelivNoteStates)Enum.Parse(typeof(gip.mes.datamodel.MDDelivNoteState.DelivNoteStates), SelectedFilterDelivNoteState.Value.ToString());
            }
        }


        private ACValueItem _SelectedFilterDelivNoteState;
        [ACPropertySelected(9999, "FilterDelivNoteState", "en{'Delivery state'}de{'Lieferstatus'}")]
        public ACValueItem SelectedFilterDelivNoteState
        {
            get
            {
                return _SelectedFilterDelivNoteState;
            }
            set
            {
                if (_SelectedFilterDelivNoteState != value)
                {
                    _SelectedFilterDelivNoteState = value;
                    OnPropertyChanged("SelectedFilterDelivNoteState");
                }
            }
        }


        private ACValueItemList _FilterDelivNoteStateList;
        [ACPropertyList(9999, "FilterDelivNoteState")]
        public ACValueItemList FilterDelivNoteStateList
        {
            get
            {
                if (_FilterDelivNoteStateList == null)
                {
                    _FilterDelivNoteStateList = new ACValueItemList("DelivNoteStatesList");
                    _FilterDelivNoteStateList.AddRange(DatabaseApp.MDDelivNoteState.ToList().Select(x => new ACValueItem(x.MDDelivNoteStateName, x.MDDelivNoteStateIndex, null)).ToList());
                }
                return _FilterDelivNoteStateList;
            }
        }

        [ACPropertyInfo(301, nameof(FilterComment), ConstApp.Comment)]
        public string FilterComment
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("Comment");
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("Comment");
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>("Comment", value);
                    OnPropertyChanged();
                }
            }
        }
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
                case "CurrentACMethodBooking\\OutwardFacilityLot":
                    {
                        if (CurrentACMethodBooking != null && CurrentDeliveryNotePos != null)
                        {
                            if (CurrentDeliveryNotePos.Material == null
                                || !CurrentDeliveryNotePos.Material.IsLotManaged)
                                return Global.ControlModes.Disabled;
                        }
                        break;
                    }
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

        #region DeliveryNote
        [ACMethodCommand(DeliveryNote.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        protected override void OnPostSave()
        {
            _UnSavedUnAssignedOutOrderPos = new List<OutOrderPos>();
            _UnSavedAssignedPickingOutOrderPos = new List<OutOrderPos>();
            RefreshOutOrderPosList();
            base.OnPostSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(DeliveryNote.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        protected override void OnPostUndoSave()
        {
            _UnSavedUnAssignedOutOrderPos = new List<OutOrderPos>();
            _UnSavedAssignedPickingOutOrderPos = new List<OutOrderPos>();
            RefreshOutOrderPosList();
            if (CurrentDeliveryNote != null && CurrentDeliveryNote.EntityState != EntityState.Added)
                CurrentDeliveryNote.DeliveryNotePos_DeliveryNote.AutoLoad(CurrentDeliveryNote.DeliveryNotePos_DeliveryNoteReference, CurrentDeliveryNote);
            OnPropertyChanged("DeliveryNotePosList");
            OnPropertyChanged("OutOrderPosFromPickingList");
            base.OnPostUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedDeliveryNote", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<DeliveryNote>(requery, () => SelectedDeliveryNote, () => CurrentDeliveryNote, c => CurrentDeliveryNote = c,
                        DatabaseApp.DeliveryNote
                        .Where(c => c.DeliveryNoteID == SelectedDeliveryNote.DeliveryNoteID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedDeliveryNote != null;
        }

        [ACMethodInteraction(DeliveryNote.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedDeliveryNote", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(DeliveryNote), DeliveryNote.NoColumnName, DeliveryNote.FormatNewNo, this);
            CurrentDeliveryNote = DeliveryNote.NewACObject(DatabaseApp, null, secondaryKey);
            CurrentDeliveryNote.DeliveryNoteType = GlobalApp.DeliveryNoteType.Issue;
            DatabaseApp.DeliveryNote.Add(CurrentDeliveryNote);
            SelectedDeliveryNote = CurrentDeliveryNote;
            if (AccessPrimary != null)
                AccessPrimary.NavList.Add(CurrentDeliveryNote);
            ACState = Const.SMNew;
            PostExecute("New");
        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentDeliveryNote", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentDeliveryNote.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentDeliveryNote);
            SelectedDeliveryNote = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        public bool IsEnabledDelete()
        {
            return CurrentDeliveryNote != null;
        }

        [ACMethodCommand(DeliveryNote.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("DeliveryNoteList");
            RefreshOutOrderPosList();
            OnPropertyChanged("OutOrderPosFromPickingList");
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
            if (acAccess == _AccessOutOrderPos)
            {
                _AccessOutOrderPos.NavSearch(this.DatabaseApp);
                OnPropertyChanged("OutOrderPosList");
                return true;
            }
            else if (acAccess == _AccessBookingFacility)
            {
                _AccessBookingFacility.NavSearch(this.DatabaseApp);
                OnPropertyChanged("BookingFacilityList");
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }

        /// <summary>
        /// Deliveries the note ready.
        /// </summary>
        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Ready'}de{'Fertig'}", 600, false, "", Global.ACKinds.MSMethodPrePost)]
        public void DeliveryNoteReady()
        {
            if (!PreExecute("DeliveryNoteReady")) return;
            // TODO: 
            PostExecute("DeliveryNoteReady");
        }

        /// <summary>
        /// Determines whether [is enabled delivery note ready].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delivery note ready]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeliveryNoteReady()
        {
            // TODO: 
            return true;
        }

        /// <summary>
        /// Delivereds this instance.
        /// </summary>
        [ACMethodCommand(DeliveryNote.ClassName, "en{'Set Delivered'}de{'Geliefert setzen'}", 601, true, Global.ACKinds.MSMethodPrePost)]
        public void Delivered()
        {
            if (!PreExecute("Delivered")) return;
            // TODO: 
            PostExecute("Delivered");
        }

        /// <summary>
        /// Determines whether [is enabled delivered].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delivered]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelivered()
        {
            // TODO: 
            return true;
        }

        [ACMethodCommand(DeliveryNote.ClassName, "en{'Cancel Delivery Note'}de{'Storniere Lieferschein'}", (short)MISort.Cancel)]
        public void CancelDelivery()
        {
            if (!PreExecute("CancelDelivery"))
                return;
            if (!IsEnabledCancelDelivery())
                return;
            var result = OutDeliveryNoteManager.CancelFacilityPreBooking(ACFacilityManager, DatabaseApp, CurrentDeliveryNote);
            if (result != null && result.Any())
            {
                foreach (DeliveryNotePos deliveryNotePos in DeliveryNotePosList.ToList())
                {
                    SelectedDeliveryNotePos = deliveryNotePos;
                    if (CurrentDeliveryNotePos == deliveryNotePos)
                    {
                        BookAllACMethodBookings();
                    }
                }
                int countCancelled = 0;
                int countAssigned = 0;
                foreach (DeliveryNotePos deliveryNotePos in DeliveryNotePosList.ToList())
                {
                    if (deliveryNotePos.OutOrderPos != null)
                        countAssigned++;
                    if ((deliveryNotePos.OutOrderPos != null)
                        && (deliveryNotePos.OutOrderPos.MDOutOrderPosState.OutOrderPosState == MDOutOrderPosState.OutOrderPosStates.Cancelled
                            || !deliveryNotePos.OutOrderPos.FacilityBooking_OutOrderPos.Any()))
                    {
                        countCancelled++;
                    }
                }
                if (countCancelled == countAssigned)
                {
                    MDDelivNoteState state = DatabaseApp.MDDelivNoteState.Where(c => c.MDDelivNoteStateIndex == (short)MDDelivNoteState.DelivNoteStates.Cancelled).FirstOrDefault();
                    if (state != null)
                    {
                        CurrentDeliveryNote.MDDelivNoteState = state;
                        Save();
                    }
                }
                OnPropertyChanged("DeliveryNotePosList");
            }
            PostExecute("CancelDelivery");
        }

        public bool IsEnabledCancelDelivery()
        {
            if (CurrentDeliveryNote == null || CurrentDeliveryNote.MDDelivNoteState == null)
                return false;
            if (CurrentDeliveryNote.MDDelivNoteState.DelivNoteState == MDDelivNoteState.DelivNoteStates.Cancelled)
                return false;
            if (!DeliveryNotePosList.Any())
                return false;
            return true;
        }


        [ACMethodCommand(DeliveryNote.ClassName, "en{'Create Invoice'}de{'Rechnung erstellen'}", (short)MISort.Cancel)]
        public void CreateInvoice()
        {
            if (!PreExecute("CreateInvoice"))
                return;
            if (Root.Messages.Question(this, "Question50114", Global.MsgResult.Yes, false, CurrentDeliveryNote.DeliveryNoteNo) == Global.MsgResult.Yes)
            {
                Msg msg = OutDeliveryNoteManager.NewInvoiceFromOutDeliveryNote(DatabaseApp, CurrentDeliveryNote);
                if (msg != null)
                    Messages.Msg(msg);
            }
            PostExecute("CreateInvoice");
        }

        public bool IsEnabledCreateInvoice()
        {
            return CurrentDeliveryNote != null
                && OutDeliveryNoteManager != null
                && !CurrentDeliveryNote.DeliveryNotePos_DeliveryNote.Any(x => x.OutOrderPosID == null)
                && !CurrentDeliveryNote
                    .DeliveryNotePos_DeliveryNote
                    .Select(c => c.OutOrderPos.OutOrder)
                    .SelectMany(c => c.OutOrderPos_OutOrder)
                    .SelectMany(c => c.InvoicePos_OutOrderPos)
                    .Any();
        }

        #endregion

        #region DeliveryNotePos

        #region DeliveryNotePos -> Filter, Search, Show LabOrder

        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Filter'}de{'Filter'}", 602, false)]
        public bool FilterDialogOutOrderPos()
        {
            bool result = AccessOutOrderPos.ShowACQueryDialog();
            if (result)
            {
                RefreshOutOrderPosList();
            }
            return result;
        }

        [ACMethodInfo("Dialog", "en{'Lab Report'}de{'Laborbericht'}", (short)MISort.QueryPrintDlg)]
        public void ShowLabOrderFromOutOrder()
        {
            if (this.DatabaseApp.IsChanged || SelectedDeliveryNotePos == null)
                return;
            ACComponent childBSO = ACUrlCommand("?LabOrderDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("LabOrderDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            childBSO.ACUrlCommand("!" + nameof(BSOLabOrder.ShowLabOrderViewDialog), null, SelectedDeliveryNotePos.OutOrderPos, null, null, null, null, true, null);
            childBSO.Stop();
        }

        public bool IsEnabledShowLabOrderFromOutOrder()
        {
            if (SelectedDeliveryNotePos != null)
            {
                if (!SelectedDeliveryNotePos.OutOrderPos.LabOrder_OutOrderPos.Any())
                    return false;
            }

            return true;
        }

        #endregion

        #region DeliveryNotePos -> Manipulate (Delete, Assing, Unassing)

        /// <summary>
        /// Assigns the in order pos.
        /// </summary>
        [ACMethodCommand(DeliveryNote.ClassName, "en{'Assign'}de{'Zuordnen'}", 603, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void AssignOutOrderPos()
        {
            if (!PreExecute("AssignOutOrderPos"))
                return;

            if (!IsEnabledAssignOutOrderPos())
                return;

            try
            {
                List<object> resultNewEntities = new List<object>();
                Msg result = OutDeliveryNoteManager.AssignOutOrderPos(CurrentOutOrderPos, CurrentDeliveryNote, PartialQuantity, DatabaseApp, ACFacilityManager, resultNewEntities);
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

                Messages.LogException("BSOOutDeliveryNote", "AssignOutOrderPos", msg);

                return;
            }

            if (_UnSavedUnAssignedOutOrderPos.Contains(CurrentOutOrderPos))
                _UnSavedUnAssignedOutOrderPos.Remove(CurrentOutOrderPos);
            OnPropertyChanged("DeliveryNotePosList");
            RefreshOutOrderPosList();
            PartialQuantity = null;

            PostExecute("AssignOutOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled assign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignOutOrderPos()
        {
            if (CurrentOutOrderPos == null)
                return false;
            return true;
        }

        /// <summary>
        /// Unassigns the in order pos.
        /// </summary>
        [ACMethodCommand(DeliveryNote.ClassName, "en{'Remove'}de{'Entfernen'}", 604, true, Global.ACKinds.MSMethodPrePost)]
        public void UnassignOutOrderPos()
        {
            if (!PreExecute("UnassignOutOrderPos"))
                return;

            OutOrderPos parentOutOrderPos = null;
            OutOrderPos currentOutOrderPos = CurrentDeliveryNotePos.OutOrderPos;
            parentOutOrderPos = CurrentDeliveryNotePos.OutOrderPos.OutOrderPos1_ParentOutOrderPos;

            Msg result = null;
            try
            {
                result = OutDeliveryNoteManager.UnassignOutOrderPos(CurrentDeliveryNotePos, DatabaseApp);
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

                Messages.LogException("BSOOutDeliveryNote", "UnassignOutOrderPos", msg);

                return;
            }

            if (result == null && parentOutOrderPos != null)
            {
                if (!_UnSavedUnAssignedOutOrderPos.Contains(parentOutOrderPos))
                    _UnSavedUnAssignedOutOrderPos.Add(parentOutOrderPos);
            }

            if (_UnSavedAssignedPickingOutOrderPos.Contains(currentOutOrderPos))
            {
                _UnSavedAssignedPickingOutOrderPos.Remove(currentOutOrderPos);
                OnPropertyChanged("OutOrderPosFromPickingList");
            }

            OnPropertyChanged("DeliveryNotePosList");
            RefreshOutOrderPosList();
            PartialQuantity = null;
            PostExecute("UnassignOutOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled unassign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled unassign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUnassignOutOrderPos()
        {
            if (CurrentDeliveryNotePos == null)
                return false;
            return true;
        }

        [ACMethodInteraction("Dialog", "en{'New Lab Report'}de{'Neuer Laborauftrag'}", (short)MISort.New, false, "CreateNewLabOrderFromOutOrder", Global.ACKinds.MSMethodPrePost)]
        public void CreateNewLabOrderFromOutOrder()
        {
            if (this.DatabaseApp.IsChanged || SelectedDeliveryNotePos == null)
                return;
            ACComponent childBSO = ACUrlCommand("?LabOrderDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("LabOrderDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            childBSO.ACUrlCommand("!" + nameof(BSOLabOrder.NewLabOrderDialog), null, SelectedDeliveryNotePos, null, null, null);
            childBSO.Stop();
        }

        public bool IsEnabledCreateNewLabOrderFromOutOrder()
        {
            if (SelectedDeliveryNotePos != null)
            {
                if (SelectedDeliveryNotePos.OutOrderPos.LabOrder_OutOrderPos.Any())
                    return false;
            }

            return true;
        }

        #endregion

        #endregion

        #region Picking
        [ACMethodCommand(DeliveryNote.ClassName, "en{'Assign'}de{'Zuordnen'}", 605, true, Global.ACKinds.MSMethodPrePost)]
        public void AssignPicking()
        {
            if (!PreExecute("AssignPicking"))
                return;

            try
            {
                List<object> resultNewEntities = new List<object>();
                Msg result = OutDeliveryNoteManager.NewDeliveryNotePos(CurrentOutOrderPosFromPicking, CurrentDeliveryNote, DatabaseApp, resultNewEntities);
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

                Messages.LogException("BSOOutDeliveryNote", "AssignPicking", msg);
                return;
            }

            if (!_UnSavedAssignedPickingOutOrderPos.Contains(CurrentOutOrderPosFromPicking))
                _UnSavedAssignedPickingOutOrderPos.Add(CurrentOutOrderPosFromPicking);
            OnPropertyChanged("OutOrderPosFromPickingList");
            RefreshOutOrderPosList();
            OnPropertyChanged("DeliveryNotePosList");
            PartialQuantity = null;

            PostExecute("AssignPicking");
        }

        /// <summary>
        /// Determines whether [is enabled assign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignPicking()
        {
            if (this.CurrentOutOrderPosFromPicking == null || CurrentDeliveryNote == null)
                return false;
            return true;
        }

        [ACMethodInfo("", "en{'Create or update picking'}de{'Kommissionierung erstellen oder aktualisieren'}", 9999)]
        public void CreateOrUpdatePicking()
        {
            ACPickingManager pickingManager = PickingManager;
            if (pickingManager == null)
            {
                Messages.Error(this, "Der Kommissioniermanager ist nicht verfügbar!", true);
                return;
            }

            Picking picking = null;
            Msg msg = pickingManager.CreateOrUpdatePickingFromOutDeliveryNote(CurrentDeliveryNote, DatabaseApp, out picking);

            if (picking == null)
                return;

            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            string facilityNo = OnGetFacilityNoForPicking(DatabaseApp, picking);
            if (facilityNo == null)
                return;

            foreach (PickingPos pPos in picking.PickingPos_Picking)
            {
                ProcessPickingPos(DatabaseApp, pPos, facilityNo);
            }

            CurrentDeliveryNote.MDDelivNoteState = DatabaseApp.MDDelivNoteState.FirstOrDefault(c => c.MDDelivNoteStateIndex == (short)MDDelivNoteState.DelivNoteStates.Completed);
            ACSaveChanges();
        }

        public bool IsEnabledCreateOrUpdatePicking()
        {
            return CurrentDeliveryNote != null;
        }

        public virtual void ProcessPickingPos(DatabaseApp databaseApp, PickingPos pickingPos, string sourceFacilityNo)
        {
            if (pickingPos.EntityState == EntityState.Added)
                pickingPos.MDDelivPosLoadState = MDDelivPosLoadState.DefaultMDDelivPosLoadState(databaseApp);

            pickingPos.FromFacility = databaseApp.Facility.FirstOrDefault(c => c.FacilityNo == sourceFacilityNo);
        }

        public virtual string OnGetFacilityNoForPicking(DatabaseApp dbApp, Picking picking)
        {
            string facilityNo = null;

            Picking oldPicking = dbApp.Picking.Where(c => c.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.Issue
                                                       && c.PickingStateIndex >= (short)PickingStateEnum.Finished
                                                       && c.PickingPos_Picking.Any(x => x.FromFacilityID.HasValue)).FirstOrDefault();

            if (oldPicking != null)
            {
                PickingPos pPos = oldPicking.PickingPos_Picking.Where(c => c.FromFacilityID.HasValue).FirstOrDefault();
                if (pPos != null)
                {
                    facilityNo = pPos.FromFacility.FacilityNo;
                }
            }

            if (facilityNo == null)
            {
                facilityNo = dbApp.Facility.Where(c => c.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBin).FirstOrDefault()?.FacilityNo;
            }

            return facilityNo;
        }

        #endregion

        #region OutOrderPos

        #region OutOrderPos -> Refresh

        public void RefreshOutOrderPosList()
        {
            if (AccessOutOrderPos == null)
                return;
            if (_ActivateOutOpen)
                AccessOutOrderPos.NavSearch(DatabaseApp);
            OnPropertyChanged("OutOrderPosList");
        }

        #endregion

        #endregion

        #region FacilityPreBooking
        [ACMethodInteraction("FacilityPreBooking", "en{'New Posting'}de{'Neue Buchung'}", (short)MISort.New, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void NewFacilityPreBooking()
        {
            if (!IsEnabledNewFacilityPreBooking())
                return;
            if (!PreExecute("NewFacilityPreBooking"))
                return;
            CurrentFacilityPreBooking = OutDeliveryNoteManager.NewFacilityPreBooking(ACFacilityManager, DatabaseApp, CurrentDeliveryNotePos);
            OnPropertyChanged("CurrentACMethodBooking");
            OnPropertyChanged("FacilityPreBookingList");
            PostExecute("NewFacilityPreBooking");
        }

        public bool IsEnabledNewFacilityPreBooking()
        {
            if (CurrentDeliveryNotePos == null || CurrentDeliveryNotePos.OutOrderPos == null)
                return false;
            return true;
        }


        [ACMethodInteraction("FacilityPreBooking", "en{'Cancel Posting'}de{Buchung Abbrechen'}", (short)MISort.Cancel, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void CancelFacilityPreBooking()
        {
            if (!IsEnabledCancelFacilityPreBooking())
                return;
            if (!PreExecute("CancelFacilityPreBooking"))
                return;
            var result = OutDeliveryNoteManager.CancelFacilityPreBooking(ACFacilityManager, DatabaseApp, CurrentDeliveryNotePos);
            if (result != null && result.Any())
            {
                CurrentFacilityPreBooking = result.FirstOrDefault();
                OnPropertyChanged("CurrentACMethodBooking");
                OnPropertyChanged("FacilityPreBookingList");
            }
            PostExecute("CancelFacilityPreBooking");
        }

        public bool IsEnabledCancelFacilityPreBooking()
        {
            if (CurrentDeliveryNotePos == null
                || CurrentDeliveryNotePos.OutOrderPos == null
                || FacilityPreBookingList.Any())
                return false;
            return true;
        }


        [ACMethodInteraction("FacilityPreBooking", "en{'Delete Posting'}de{'Lösche Buchung'}", (short)MISort.Delete, true, "SelectedDeliveryNotePos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteFacilityPreBooking()
        {
            if (!IsEnabledDeleteFacilityPreBooking())
                return;
            if (!PreExecute("DeleteFacilityPreBooking"))
                return;
            Msg msg = CurrentFacilityPreBooking.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            else
            {
                CurrentFacilityPreBooking = null;
                OnPropertyChanged("FacilityPreBookingList");
            }
            PostExecute("DeleteFacilityPreBooking");
        }

        public bool IsEnabledDeleteFacilityPreBooking()
        {
            return CurrentFacilityPreBooking != null;
        }

        [ACMethodInteraction("FacilityPreBooking", "en{'Post Item'}de{'Buche Position'}", 606, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void BookDeliveryPos()
        {
            if (!PreExecute("BookDeliveryPos")) return;
            PostExecute("BookDeliveryPos");
        }

        public bool IsEnabledBookDeliveryPos()
        {
            return true;
        }

        [ACMethodInteraction("FacilityPreBooking", "en{'Post'}de{'Buchen'}", 607, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void BookCurrentACMethodBooking()
        {
            if (!IsEnabledBookCurrentACMethodBooking())
                return;

            if (CurrentACMethodBooking.OutOrderPos != CurrentDeliveryNotePos.OutOrderPos)
                CurrentACMethodBooking.OutOrderPos = CurrentDeliveryNotePos.OutOrderPos;
            if (CurrentDeliveryNotePos.OutOrderPos.OutOrder.CPartnerCompany != null && CurrentACMethodBooking.CPartnerCompany != CurrentDeliveryNotePos.OutOrderPos.OutOrder.CPartnerCompany)
                CurrentACMethodBooking.CPartnerCompany = CurrentDeliveryNotePos.OutOrderPos.OutOrder.CPartnerCompany;

            bool isCancellation = CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel;

            Save();
            if (DatabaseApp.IsChanged)
                return;
            if (!PreExecute("BookCurrentACMethodBooking"))
                return;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentACMethodBooking, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentACMethodBooking.ValidMessage.IsSucceded() || CurrentACMethodBooking.ValidMessage.HasWarnings())
                Messages.Msg(CurrentACMethodBooking.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
                OnPropertyChanged("FacilityBookingList");
            }
            else
            {
                DeleteFacilityPreBooking();
                OnPropertyChanged("FacilityBookingList");
                CurrentDeliveryNotePos.OutOrderPos.TopParentOutOrderPos.RecalcActualQuantity();
                if (isCancellation)
                {
                    MDOutOrderPosState state = DatabaseApp.MDOutOrderPosState.Where(c => c.MDOutOrderPosStateIndex == (short)MDOutOrderPosState.OutOrderPosStates.Cancelled).FirstOrDefault();
                    if (state != null)
                        CurrentDeliveryNotePos.OutOrderPos.MDOutOrderPosState = state;
                }
                else
                {
                    MDOutOrderPosState state = DatabaseApp.MDOutOrderPosState.Where(c => c.MDOutOrderPosStateIndex == (short)MDOutOrderPosState.OutOrderPosStates.Completed).FirstOrDefault();
                    if (state != null)
                        CurrentDeliveryNotePos.OutOrderPos.MDOutOrderPosState = state;
                }
                Save();
            }
            PostExecute("BookCurrentACMethodBooking");
        }

        public bool IsEnabledBookCurrentACMethodBooking()
        {
            if (_CurrentACMethodBookingDummy != null)
                return false;
            bool bRetVal = false;
            if (CurrentACMethodBooking != null)
                bRetVal = CurrentACMethodBooking.IsEnabled();
            else
                return false;
            UpdateBSOMsg();
            return bRetVal;
        }

        [ACMethodCommand("DeliveryNotePos", "en{'Post All'}de{'Buche alle'}", (short)MISort.Cancel)]
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
            if (CurrentDeliveryNotePos == null || !FacilityPreBookingList.Any())
                return false;
            return true;
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
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!ShowDialogNewLot", "", CurrentDeliveryNotePos.OutOrderPos.Material);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                FacilityLot result = dlgResult.ReturnValue as FacilityLot;
                if (result != null)
                {
                    Save();
                    CurrentACMethodBooking.OutwardFacilityLot = result;
                    if (AccessBookingFacilityLot != null)
                        AccessBookingFacilityLot.NavSearch(DatabaseApp);
                    Save();
                }
            }
            childBSO.Stop();
        }

        public bool IsEnabledNewFacilityLot()
        {
            return CurrentACMethodBooking != null;
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


        #region FacilityPreBooking -> Available quants

        [ACMethodInfo("ShowDlgOutwardAvailableQuants", "en{'Choose quant'}de{'Quant auswählen'}", 999)]
        public void ShowDlgOutwardAvailableQuants()
        {
            if (!IsEnabledShowDlgOutwardAvailableQuants())
                return;
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
        [ACMethodInfo("DlgAvailableQuantsOk", Const.Ok, 999)]
        public void DlgAvailableQuantsOk()
        {
            if (!IsEnabledDlgAvailableQuantsOk())
                return;
            CurrentACMethodBooking.OutwardFacility = SelectedPreBookingAvailableQuants.Facility;
            CurrentACMethodBooking.OutwardFacilityCharge = SelectedPreBookingAvailableQuants;
            CurrentACMethodBooking.OutwardMaterial = null;
            CurrentACMethodBooking.OutwardFacilityLot = null;
            OnPropertyChanged("CurrentACMethodBooking");
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
            if (CurrentACMethodBooking != null
                && CurrentDeliveryNotePos != null
                && CurrentDeliveryNotePos.Material != null)
                return CurrentDeliveryNotePos.Material;
            return null;
        }

        #endregion


        #endregion

        #region Tracking

        public override ACMenuItemList GetMenu(string vbContent, string vbControl)
        {
            ACMenuItemList aCMenuItems = base.GetMenu(vbContent, vbControl);
            Dictionary<OutDevNote_TrackingProperties, string> trackingVBContents = ((OutDevNote_TrackingProperties[])Enum.GetValues(typeof(OutDevNote_TrackingProperties))).ToDictionary(key => key, val => val.ToString());
            if (!string.IsNullOrEmpty(vbContent) && trackingVBContents.Values.Contains(vbContent))
            {
                TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
                OutDevNote_TrackingProperties trackingProperty = (OutDevNote_TrackingProperties)Enum.Parse(typeof(OutDevNote_TrackingProperties), vbContent);
                ACMenuItemList trackingAndTracingMenuItems = null;
                switch (trackingProperty)
                {
                    case OutDevNote_TrackingProperties.SelectedFacilityBooking:
                        if (SelectedFacilityBooking != null)
                        {
                            trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedFacilityBooking);
                        }
                        break;
                        //case OutDevNote_TrackingProperties.SelectedDeliveryNotePos:
                        //    if (SelectedDeliveryNotePos != null)
                        //    {
                        //        trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedDeliveryNotePos);
                        //    }
                        //    break;
                        //case OutDevNote_TrackingProperties.SelectedOutOrderPos:
                        //    if (SelectedOutOrderPos != null)
                        //    {
                        //        trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedOutOrderPos.TopParentOutOrderPos);
                        //    }
                        //    break;
                        //case OutDevNote_TrackingProperties.SelectedOutOrderPosFromPicking:
                        //    if (SelectedOutOrderPosFromPicking != null)
                        //    {
                        //        trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedOutOrderPosFromPicking.TopParentOutOrderPos);
                        //    }
                        //    break;
                }
                if (trackingAndTracingMenuItems != null)
                    aCMenuItems.AddRange(trackingAndTracingMenuItems);
            }

            return aCMenuItems;
        }

        [ACMethodInfo("OnTrackingCall", "en{'OnTrackingCall'}de{'OnTrackingCall'}", 608, false)]
        public void OnTrackingCall(GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter, TrackingEnginesEnum engine)
        {
            TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
            trackingCommonStart.DoTracking(this, direction, itemForTrack, additionalFilter, engine);
        }

        #endregion

        #region Message
        private void UpdateBSOMsg()
        {
            // TODO: Bei BSOFacilityBookCharge die Methode UpdateBSOMsg implementieren
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

        #region Dialog New DeliveryNote
        public VBDialogResult DialogResult { get; set; }

        [ACMethodInfo("Dialog", "en{'New Delivery Note'}de{'Neuer Lieferschein'}", (short)MISort.QueryPrintDlg)]
        public VBDialogResult ShowDialogNewDeliveryNote(string DeliveryNoteNo = "")
        {
            New();
            if (!String.IsNullOrEmpty(DeliveryNoteNo))
                CurrentDeliveryNote.DeliveryNoteNo = DeliveryNoteNo;
            ShowDialog(this, "DeliveryNoteDialog");
            this.ParentACComponent.StopComponent(this);
            return DialogResult;
        }

        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay)]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            DialogResult.ReturnValue = CurrentDeliveryNote;
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel)]
        public void DialogCancel()
        {
            Delete();
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            CloseTopDialog();
        }
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

        protected bool _ActivateOutOpen = false;
        [ACMethodInfo("Picking", "en{'Activate'}de{'Aktivieren'}", 609, true, Global.ACKinds.MSMethodPrePost)]
        public void OnActivate(string page)
        {
            if (!PreExecute("OnActivate"))
                return;
            switch (page)
            {
                case "*AssignOutOrderPos":
                case "AssignOutOrderPos":
                    if (!_ActivateOutOpen)
                    {
                        _ActivateOutOpen = true;
                        RefreshOutOrderPosList();
                    }
                    break;
                default:
                    break;
            }
            PostExecute("OnActivate");
        }
        #endregion

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


        [ACMethodInfo("Dialog", "en{'Dialog Delivery Note'}de{'Dialog Lieferschein'}", (short)MISort.QueryPrintDlg)]
        public void ShowDialogOrder(string deliveryNoteNo, Guid deliveryNotePosID)
        {
            if (AccessPrimary == null)
                return;
            //AccessPrimary.NavACQueryDefinition.SearchWord = facilityNo;
            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "DeliveryNoteNo").FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, "DeliveryNoteNo", Global.LogicalOperators.contains, Global.Operators.and, deliveryNoteNo, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            else
                filterItem.SearchWord = deliveryNoteNo;

            this.Search();
            if (CurrentDeliveryNote != null && deliveryNotePosID != Guid.Empty)
            {
                if (this.DeliveryNotePosList != null && this.DeliveryNotePosList.Any())
                {
                    var deliveryNotePos = this.DeliveryNotePosList.Where(c => c.DeliveryNotePosID == deliveryNotePosID).FirstOrDefault();
                    if (deliveryNotePos != null)
                        SelectedDeliveryNotePos = deliveryNotePos;
                }
            }
            ShowDialog(this, "DisplayOrderDialog");
            this.ParentACComponent.StopComponent(this);
            _IsEnabledACProgram = true;
        }

        [ACMethodInfo("Dialog", "en{'Dialog Delivery Note'}de{'Dialog Lieferschein'}", (short)MISort.QueryPrintDlg + 1)]
        public void ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;

            // Falls Produktionsauftrag
            DeliveryNotePos dnPos = null;
            DeliveryNote dn = null;
            foreach (var entry in paOrderInfo.Entities)
            {
                if (entry.EntityName == DeliveryNote.ClassName)
                {
                    dn = this.DatabaseApp.DeliveryNote
                        .Where(c => c.DeliveryNoteID == entry.EntityID)
                        .FirstOrDefault();
                }
                else if (entry.EntityName == DeliveryNotePos.ClassName)
                {
                    dnPos = this.DatabaseApp.DeliveryNotePos
                        .Include(c => c.DeliveryNote)
                        .Where(c => c.DeliveryNotePosID == entry.EntityID)
                        .FirstOrDefault();
                    if (dnPos != null)
                        dn = dnPos.DeliveryNote;
                }
                else if (entry.EntityName == OrderLog.ClassName)
                {
                    _IsEnabledACProgram = false;
                    OrderLog currentOrderLog = DatabaseApp.OrderLog.FirstOrDefault(c => c.VBiACProgramLogID == entry.EntityID);
                    if (currentOrderLog == null || currentOrderLog.DeliveryNotePos == null)
                        return;
                    dnPos = currentOrderLog.DeliveryNotePos;
                    dn = dnPos.DeliveryNote;
                }
            }

            if (dn == null)
                return;

            ShowDialogOrder(dn.DeliveryNoteNo, dnPos != null ? dnPos.DeliveryNotePosID : Guid.Empty);
            paOrderInfo.DialogResult = this.DialogResult;
        }

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(IsEnabledShowLabOrderFromOutOrder):
                    result = IsEnabledShowLabOrderFromOutOrder();
                    return true;
                case nameof(AssignPicking):
                    AssignPicking();
                    return true;
                case nameof(IsEnabledAssignPicking):
                    result = IsEnabledAssignPicking();
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
                case nameof(ShowDialogNewDeliveryNote):
                    result = ShowDialogNewDeliveryNote(acParameter.Count() == 1 ? (System.String)acParameter[0] : "");
                    return true;
                case nameof(DialogOK):
                    DialogOK();
                    return true;
                case nameof(DialogCancel):
                    DialogCancel();
                    return true;
                case nameof(OnActivate):
                    OnActivate((System.String)acParameter[0]);
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
                case nameof(DeliveryNoteReady):
                    DeliveryNoteReady();
                    return true;
                case nameof(IsEnabledDeliveryNoteReady):
                    result = IsEnabledDeliveryNoteReady();
                    return true;
                case nameof(Delivered):
                    Delivered();
                    return true;
                case nameof(IsEnabledDelivered):
                    result = IsEnabledDelivered();
                    return true;
                case nameof(CancelDelivery):
                    CancelDelivery();
                    return true;
                case nameof(IsEnabledCancelDelivery):
                    result = IsEnabledCancelDelivery();
                    return true;
                case nameof(AssignOutOrderPos):
                    AssignOutOrderPos();
                    return true;
                case nameof(IsEnabledAssignOutOrderPos):
                    result = IsEnabledAssignOutOrderPos();
                    return true;
                case nameof(UnassignOutOrderPos):
                    UnassignOutOrderPos();
                    return true;
                case nameof(IsEnabledUnassignOutOrderPos):
                    result = IsEnabledUnassignOutOrderPos();
                    return true;
                case nameof(FilterDialogOutOrderPos):
                    result = FilterDialogOutOrderPos();
                    return true;
                case nameof(RefreshOutOrderPosList):
                    RefreshOutOrderPosList();
                    return true;
                case nameof(CreateNewLabOrderFromOutOrder):
                    CreateNewLabOrderFromOutOrder();
                    return true;
                case nameof(IsEnabledCreateNewLabOrderFromOutOrder):
                    result = IsEnabledCreateNewLabOrderFromOutOrder();
                    return true;
                case nameof(ShowLabOrderFromOutOrder):
                    ShowLabOrderFromOutOrder();
                    return true;
                case nameof(ShowDialogOrder):
                    ShowDialogOrder((String)acParameter[0], (Guid)acParameter[1]);
                    return true;
                case nameof(ShowDialogOrderInfo):
                    ShowDialogOrderInfo((PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(CreateInvoice):
                    CreateInvoice();
                    return true;
                case nameof(IsEnabledCreateInvoice):
                    result = IsEnabledCreateInvoice();
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
                case nameof(CreateOrUpdatePicking):
                    CreateOrUpdatePicking();
                    return true;
                case nameof(IsEnabledCreateOrUpdatePicking):
                    result = IsEnabledCreateOrUpdatePicking();
                    return true;
                default:
                    break;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }

    enum OutDevNote_TrackingProperties
    {
        SelectedFacilityBooking,
        SelectedDeliveryNotePos,
        SelectedOutOrderPos,
        SelectedOutOrderPosFromPicking,
    }
}
