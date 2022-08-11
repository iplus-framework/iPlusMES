using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.manager;
using gip.core.reporthandler.Configuration;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Linq;
using static gip.mes.datamodel.GlobalApp;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Production Order'}de{'Produktionsauftrag'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + ProdOrder.ClassName)]
    [ACQueryInfo(Const.PackName_VarioManufacturing, Const.QueryPrefix + "BookingFacility", "en{'Storage Bin'}de{'Lagerplatz'}", typeof(Facility), Facility.ClassName, MDFacilityType.ClassName + "\\MDFacilityTypeIndex", "FacilityNo")]
    [ACQueryInfo(Const.PackName_VarioManufacturing, Const.QueryPrefix + ProdOrderPartslistPos.ClassName, "en{'Prod. Order Pos.'}de{'Produktionsauftrag Position'}", typeof(ProdOrderPartslistPos), ProdOrderPartslistPos.ClassName, "ProdOrderPartslistPos1_ParentProdOrderPartslistPos", Material.ClassName + "\\MaterialNo")]
    public partial class BSOProdOrder : ACBSOvbNav, IACBSOConfigStoreSelection, IACBSOACProgramProvider, IOnTrackingCall
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOProdOrder"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOProdOrder(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

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

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _ProdOrderManager = ACProdOrderManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("ProdOrderManager not configured");

            _PartslistManager = ACPartslistManager.ACRefToServiceInstance(this);
            if (_PartslistManager == null)
                throw new Exception("PartslistManager not configured");

            _MatReqManager = ACMatReqManager.ACRefToServiceInstance(this);
            if (_MatReqManager == null)
                throw new Exception("MatReqManager not configured");

            AccessFilterPlanningMR.NavSearch();
            SelectedFilterPlanningMR = null;
            SelectedPOPosTimeFilterMode = POPosTimeFilterModeList?.FirstOrDefault();
            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;
            ACPartslistManager.DetachACRefFromServiceInstance(this, _PartslistManager);
            _PartslistManager = null;
            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;
            ACMatReqManager.DetachACRefFromServiceInstance(this, _MatReqManager);
            _MatReqManager = null;

            _AccessInBookingFacility = null;
            _AccessOutBookingFacility = null;
            _ACFacilityManager = null;
            _AddPartslistSequence = null;
            _AddPartslistTargetQuantity = null;
            _AlternativeProdOrderPartslistPosList = null;
            _AlternativeSelectedProdOrderPartslistPos = null;
            _BatchList = null;
            _BookingFacilityLotNo = null;
            _CurrentProdOrderPartListExpand = null;

            if (_AccessInputMaterial != null)
                _AccessInputMaterial.NavSearchExecuting -= _AccessInputMaterial_NavSearchExecuting;
            _AccessInputMaterial = null;

            _IntermediateList = null;
            _OpenPostingsList = null;
            _PartslistChangeTargetQuantityInput = null;
            _presenter = null;
            _ProcessWorkflow = null;
            _ProdOrderIntermediateBatchList = null;
            _ProdOrderPartListExpandList = null;
            _ProdOrderPartslistList = null;
            _ProdOrderPartslistPosList = null;
            _SelectedBatch = null;
            _SelectedIntermediate = null;
            _SelectedInwardACMethodBookingDummy = null;
            _SelectedInwardFacilityBooking = null;
            _SelectedInwardFacilityBookingCharge = null;
            _SelectedInwardFacilityPreBooking = null;
            _SelectedOutwardACMethodBookingDummy = null;
            _SelectedOutwardFacilityBooking = null;
            _SelectedOutwardFacilityBookingCharge = null;
            _SelectedOutwardFacilityPreBooking = null;
            _SelectedOutwardPartslistPos = null;
            _SelectedProdOrderIntermediateBatch = null;
            _SelectedProdOrderPartslist = null;
            _SelectedProdOrderPartslistPos = null;
            _UnSavedUnAssignedPartslistPos = null;

            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

            bool baseResult = base.ACDeInit(deleteACClassTask);

            if (_AccessInBookingFacility != null)
            {
                _AccessInBookingFacility.ACDeInit(false);
                _AccessInBookingFacility = null;
            }
            if (_AccessOutBookingFacility != null)
            {
                _AccessOutBookingFacility.ACDeInit(false);
                _AccessOutBookingFacility = null;
            }
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            _BSOPartslistExplorer_Child = null;
            _AccessFilterPlanningMR = null;
            return baseResult;
        }

        #endregion

        #region Managers

        protected ACRef<ACProdOrderManager> _ProdOrderManager = null;
        public ACProdOrderManager ProdOrderManager
        {
            get
            {
                if (_ProdOrderManager == null)
                    return null;
                return _ProdOrderManager.ValueT;
            }
        }

        protected ACRef<ACPartslistManager> _PartslistManager = null;
        public ACPartslistManager PartslistManager
        {
            get
            {
                if (_PartslistManager == null)
                    return null;
                return _PartslistManager.ValueT;
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

        protected ACRef<ACMatReqManager> _MatReqManager = null;
        public ACMatReqManager MatReqManager
        {
            get
            {
                if (_MatReqManager == null)
                    return null;
                return _MatReqManager.ValueT;
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

        #endregion

        #region Properties


        #region Properties -> Local BSOs

        ACChildItem<BSOPartslistExplorer> _BSOPartslistExplorer_Child;
        [ACPropertyInfo(590)]
        [ACChildInfo("BSOPartslistExplorer_Child", typeof(BSOPartslistExplorer))]
        public ACChildItem<BSOPartslistExplorer> BSOPartslistExplorer_Child
        {
            get
            {
                if (_BSOPartslistExplorer_Child == null)
                    _BSOPartslistExplorer_Child = new ACChildItem<BSOPartslistExplorer>(this, "BSOPartslistExplorer_Child");
                return _BSOPartslistExplorer_Child;
            }
        }


        #endregion

        #region Properties -> Other

        private List<PartslistPos> _UnSavedUnAssignedPartslistPos = new List<PartslistPos>();

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
                OnPropertyChanged("BookingFacilityLotNo");
            }
        }

        #endregion

        #region Properties -> FilterProdOrderState

        public gip.mes.datamodel.MDProdOrderState.ProdOrderStates? FilterProdOrderState
        {
            get
            {
                if (SelectedFilterProdOrderState == null) return null;
                return (gip.mes.datamodel.MDProdOrderState.ProdOrderStates)Enum.Parse(typeof(gip.mes.datamodel.MDProdOrderState.ProdOrderStates), SelectedFilterProdOrderState.Value.ToString());
            }
        }


        private ACValueItem _SelectedFilterProdOrderState;
        /// <summary>
        /// Selected property for ACValueItem
        /// </summary>
        /// <value>The selected FilterProdOrderState</value>
        [ACPropertySelected(9999, "FilterProdOrderState", "en{'Production Status'}de{'Produktionsstatus'}")]
        public ACValueItem SelectedFilterProdOrderState
        {
            get
            {
                return _SelectedFilterProdOrderState;
            }
            set
            {
                if (_SelectedFilterProdOrderState != value)
                {
                    _SelectedFilterProdOrderState = value;
                    OnPropertyChanged("SelectedFilterProdOrderState");
                }
            }
        }


        private ACValueItemList _FilterProdOrderStateList;
        /// <summary>
        /// List property for ACValueItem
        /// </summary>
        /// <value>The FilterProdOrderState list</value>
        [ACPropertyList(9999, "FilterProdOrderState")]
        public ACValueItemList FilterProdOrderStateList
        {
            get
            {
                if (_FilterProdOrderStateList == null)
                {
                    _FilterProdOrderStateList = new ACValueItemList("ProdOrderStatesList");
                    _FilterProdOrderStateList.AddRange(DatabaseApp.MDProdOrderState.ToList().Select(x => new ACValueItem(x.MDProdOrderStateName, x.MDProdOrderStateIndex, null)).ToList());
                }
                return _FilterProdOrderStateList;
            }
        }

        #endregion

        #region Properties - > FilterOutputMaterial

        private Material _SelectedFilterOutputMaterial;
        [ACPropertySelected(9999, "FilterOutputMaterial", "en{'Produced material'}de{'Produziertes Material'}")]
        public Material SelectedFilterOutputMaterial
        {
            get
            {
                return _SelectedFilterOutputMaterial;
            }
            set
            {
                if (_SelectedFilterOutputMaterial != value)
                {
                    _SelectedFilterOutputMaterial = value;
                    OnPropertyChanged("SelectedFilterOutputMaterial");
                }
            }
        }

        private string _MaterialNoName;
        [ACPropertySelected(9999, "MaterialNoName", "en{'Material No/Name'}de{'Material No/Name'}")]
        public string FilterMaterialNoName
        {
            get
            {
                return _MaterialNoName;
            }
            set
            {
                _MaterialNoName = value;
                OnPropertyChanged("MaterialNoName");
            }
        }

        // check property for execute a query they filling FilterOutputMaterialList
        private DateTime? _FilterOutputMaterialLastProdOrderUpdateDate;
        private List<Material> _FilterOutputMaterialList;

        [ACPropertyList(9999, "FilterOutputMaterial")]
        public List<Material> FilterOutputMaterialList
        {
            get
            {
                return _FilterOutputMaterialList;
            }
        }

        private void LoadFilterOutputMaterialList()
        {
            DateTime? lastProdOrderUpdateTime = DatabaseApp.ProdOrder.Select(c => c.UpdateDate).OrderByDescending(c => c).FirstOrDefault();
            if (lastProdOrderUpdateTime != null && (_FilterOutputMaterialLastProdOrderUpdateDate == null || lastProdOrderUpdateTime.Value > _FilterOutputMaterialLastProdOrderUpdateDate.Value))
            {
                _FilterOutputMaterialLastProdOrderUpdateDate = lastProdOrderUpdateTime;
                _FilterOutputMaterialList =
                DatabaseApp.ProdOrder
                .SelectMany(p => p.ProdOrderPartslist_ProdOrder)
                .Select(p => p.Partslist)
                .Select(p => p.Material.MaterialNo)
                .Distinct()
                .OrderBy(p => p)
                .Join(DatabaseApp.Material, materialNo => materialNo, material => material.MaterialNo, (materialNo, material) => new { material = material })
                .Select(p => p.material)
                .ToList();
            }
        }

        #region Properties - > Preselected

        public Guid? PreselectedProdOrderID { get; set; }
        public Guid? PreselectedProdorderPartslistID { get; set; }
        public Guid? PreselectedOutwarPosID { get; set; }
        public Guid? PreselectedInwardPosID { get; set; }
        public Guid? PreselectedInwardBatchPosID { get; set; }
        public Guid? PreselectedBatchID { get; set; }
        public Guid? PreselecteRelationID { get; set; }
        public Guid? PreselectedAlternativeSelectedProdOrderPartslistPosID { get; set; }

        public void PreselectedClear()
        {
            PreselectedProdOrderID = null;
            PreselectedProdorderPartslistID = null;
            PreselectedOutwarPosID = null;
            PreselectedInwardPosID = null;
            PreselectedInwardBatchPosID = null;
            PreselectedBatchID = null;
            PreselecteRelationID = null;
            PreselectedAlternativeSelectedProdOrderPartslistPosID = null;
        }

        public void PreselectedLoadCurrent()
        {
            PreselectedProdOrderID = SelectedProdOrder?.ProdOrderID;
            PreselectedProdorderPartslistID = SelectedProdOrderPartslist?.ProdOrderPartslistID;
            PreselectedOutwarPosID = SelectedProdOrderPartslistPos?.ProdOrderPartslistPosID;
            PreselectedInwardPosID = SelectedIntermediate?.ProdOrderPartslistPosID;
            PreselectedInwardBatchPosID = SelectedProdOrderIntermediateBatch?.ProdOrderPartslistPosID;
            PreselectedBatchID = SelectedBatch?.ProdOrderBatchID;
            PreselecteRelationID = SelectedOutwardPartslistPos?.ProdOrderPartslistPosRelationID;
            PreselectedAlternativeSelectedProdOrderPartslistPosID = AlternativeSelectedProdOrderPartslistPos?.ProdOrderPartslistPosID;
        }

        public void PreselectedClearSelected()
        {
            _SelectedProdOrderPartslist = null;
            _SelectedProdOrderPartslist = null;
            _SelectedProdOrderPartslistPos = null;
            _SelectedIntermediate = null;
            _SelectedProdOrderIntermediateBatch = null;
            _SelectedBatch = null;
            _SelectedOutwardPartslistPos = null;
            _AlternativeSelectedProdOrderPartslistPos = null;
        }

        #endregion

        #endregion

        #region Properties => FilterProdOrderPosStartDate

        private ACValueItem _SelectedPOPosTimeFilterMode;
        [ACPropertySelected(9999, "POPosTimeFilterMode", "en{'Time filter mode'}de{'Time filter mode'}")]
        public ACValueItem SelectedPOPosTimeFilterMode
        {
            get => _SelectedPOPosTimeFilterMode;
            set
            {
                _SelectedPOPosTimeFilterMode = value;
                OnPropertyChanged();
            }
        }

        private ACValueItemList _POPosTimeFilterModeList;
        [ACPropertyList(9999, "POPosTimeFilterMode", "")]
        public ACValueItemList POPosTimeFilterModeList
        {
            get
            {
                if (_POPosTimeFilterModeList == null)
                {
                    using (Database db = new core.datamodel.Database())
                    {
                        gip.core.datamodel.ACClass enumClass = db.GetACType(typeof(POPosTimeFilterTypeEnum));
                        if (enumClass != null && enumClass.ACValueListForEnum != null)
                            _POPosTimeFilterModeList = enumClass.ACValueListForEnum;
                        else
                            _POPosTimeFilterModeList = new ACValueListPOPosTimeFilterTypeEnum();
                    }
                }
                return _POPosTimeFilterModeList;
            }
        }

        private DateTime? _ProdOrderPosFilterFrom;
        [ACPropertyInfo(9999, "", "en{'From'}de{'Von'}")]
        public DateTime? ProdOrderPosFilterFrom
        {
            get => _ProdOrderPosFilterFrom;
            set
            {
                _ProdOrderPosFilterFrom = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _ProdOrderPosFilterTo;
        [ACPropertyInfo(9999, "", "en{'To'}de{'Nach'}")]
        public DateTime? ProdOrderPosFilterTo
        {
            get => _ProdOrderPosFilterTo;
            set
            {
                _ProdOrderPosFilterTo = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool _FilterProdOrderSelectAll;
        [ACPropertyInfo(999, "", ConstApp.SelectAll)]
        public bool FilterProdOrderSelectAll
        {
            get
            {
                return _FilterProdOrderSelectAll;
            }
            set
            {
                if (_FilterProdOrderSelectAll != value)
                {
                    _FilterProdOrderSelectAll = value;
                    if (ProdOrderList != null && ProdOrderList.Any())
                    {
                        ProdOrderList.ToList().ForEach(c => c.IsSelected = value);
                        OnPropertyChanged(nameof(ProdOrderList));
                    }
                    OnPropertyChanged(nameof(FilterProdOrderSelectAll));
                }
            }
        }


        #endregion

        #endregion

        #region Properties -> FilterPlanningMR

        #region FilterPlanningMR

        ACAccessNav<PlanningMR> _AccessFilterPlanningMR;
        [ACPropertyAccess(100, "FilterPlanningMR")]
        public ACAccessNav<PlanningMR> AccessFilterPlanningMR
        {
            get
            {
                if (_AccessFilterPlanningMR == null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + PlanningMR.ClassName, PlanningMR.ClassName);
                    _AccessFilterPlanningMR = navACQueryDefinition.NewAccessNav<PlanningMR>(PlanningMR.ClassName, this);
                    _AccessFilterPlanningMR.AutoSaveOnNavigation = false;
                }
                return _AccessFilterPlanningMR;
            }
        }

        /// <summary>
        /// Gets or sets the selected FilterPlanningMR.
        /// </summary>
        /// <value>The selected FilterPlanningMR.</value>
        [ACPropertySelected(101, PlanningMR.ClassName, ConstApp.PlanningMR)]
        public PlanningMR SelectedFilterPlanningMR
        {
            get
            {
                if (AccessFilterPlanningMR == null)
                    return null;
                return AccessFilterPlanningMR.Selected;
            }
            set
            {
                if (AccessFilterPlanningMR == null)
                    return;
                AccessFilterPlanningMR.Selected = value;
                OnPropertyChanged("SelectedFilterPlanningMR");
            }
        }

        /// <summary>
        /// Gets the FilterPlanningMR list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(102, "FilterPlanningMR")]
        public IEnumerable<PlanningMR> FilterPlanningMRList
        {
            get
            {
                if (AccessFilterPlanningMR == null)
                    return null;
                return AccessFilterPlanningMR.NavList;
            }
        }

        #endregion


        #endregion

        #region BSO handlers

        protected override bool OnUndoSave()
        {
            bool successUndoSave = base.OnUndoSave();
            Search(SelectedProdOrder, SelectedProdOrderPartslist);
            return successUndoSave;
        }

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



            if (!String.IsNullOrEmpty(vbControl.VBContent) && vbControl.VBContent.StartsWith("SelectedOutwardACMethodBooking"))
            {
                if (SelectedOutwardACMethodBooking == null)
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
                            if (acValueId == "OutwardFacilityCharge" || acValueId == "OutwardFacilityLot")
                            {
                                if (!IsEnabledOutwardFacilityChargeSelect)
                                {
                                    result = Global.ControlModes.Hidden;
                                    SelectedOutwardACMethodBooking.OutwardFacilityCharge = null;
                                    SelectedOutwardACMethodBooking.OutwardFacilityLot = null;
                                }
                            }
                        }
                    }
                }
            }


            if (!String.IsNullOrEmpty(vbControl.VBContent) && vbControl.VBContent.StartsWith("SelectedInwardACMethodBooking"))
            {
                if (SelectedInwardACMethodBooking == null)
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

        public override ACMenuItemList GetMenu(string vbContent, string vbControl)
        {
            ACMenuItemList aCMenuItems = base.GetMenu(vbContent, vbControl);
            Dictionary<BSOProdOrder_TrackingPropertiesEnum, string> trackingVBContents = ((BSOProdOrder_TrackingPropertiesEnum[])Enum.GetValues(typeof(BSOProdOrder_TrackingPropertiesEnum))).ToDictionary(key => key, val => val.ToString());
            if (!string.IsNullOrEmpty(vbContent) && trackingVBContents.Values.Contains(vbContent))
            {
                TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
                BSOProdOrder_TrackingPropertiesEnum trackingProperty = (BSOProdOrder_TrackingPropertiesEnum)Enum.Parse(typeof(BSOProdOrder_TrackingPropertiesEnum), vbContent);
                ACMenuItemList trackingAndTracingMenuItems = null;
                switch (trackingProperty)
                {
                    case BSOProdOrder_TrackingPropertiesEnum.SelectedInwardFacilityPreBooking:
                        if (SelectedInwardFacilityPreBooking != null)
                        {
                            trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedInwardFacilityPreBooking);
                        }
                        break;
                    case BSOProdOrder_TrackingPropertiesEnum.SelectedInwardFacilityBooking:
                        if (SelectedInwardFacilityBooking != null)
                        {
                            trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedInwardFacilityBooking);
                        }
                        break;
                    case BSOProdOrder_TrackingPropertiesEnum.SelectedOutwardFacilityPreBooking:
                        if (SelectedOutwardFacilityPreBooking != null)
                        {
                            trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedOutwardFacilityPreBooking);
                        }
                        break;
                    case BSOProdOrder_TrackingPropertiesEnum.SelectedOutwardFacilityBooking:
                        if (SelectedOutwardFacilityBooking != null)
                        {
                            trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedOutwardFacilityBooking);
                        }
                        break;
                }
                if (trackingAndTracingMenuItems != null)
                    aCMenuItems.AddRange(trackingAndTracingMenuItems);
            }

            return aCMenuItems;
        }

        [ACMethodInfo("OnTrackingCall", "en{'OnTrackingCall'}de{'OnTrackingCall'}", 9999, false)]
        public void OnTrackingCall(GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter, TrackingEnginesEnum engine)
        {
            TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
            trackingCommonStart.DoTracking(this, direction, itemForTrack, additionalFilter, engine);
        }
        #endregion

        #region Message

        public void SendMessage(object result)
        {
            Msg msg = result as Msg;
            if (msg != null)
            {
                SendMessage(msg);
            }
        }

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(528, "Message", "en{'Message'}de{'Meldung'}")]
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
        [ACPropertyList(529, "Message", "en{'Messagelist'}de{'Meldungsliste'}")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged(nameof(MsgList));
        }

        public void ClearMessages()
        {
            MsgList.Clear();
            OnPropertyChanged(nameof(MsgList));
        }

        #endregion

        #region ProdOrder

        #region ProdOrder -> Access Nav

        protected IQueryable<ProdOrder> _AccessPrimary_NavSearchExecuting(IQueryable<ProdOrder> result)
        {
            ObjectQuery<ProdOrder> query = result as ObjectQuery<ProdOrder>;
            if (query != null)
            {
                query.Include(c => c.CPartnerCompany);
                query.Include(c => c.MDProdOrderState);
                query.Include("ProdOrderPartslist_ProdOrder");
                query.Include("ProdOrderPartslist_ProdOrder.Partslist");
                query.Include("ProdOrderPartslist_ProdOrder.Partslist.Material");
            }

            if (SelectedFilterPlanningMR != null)
                result = result.Where(c => c.PlanningMRProposal_ProdOrder.Any(x => x.PlanningMRID == SelectedFilterPlanningMR.PlanningMRID));
            else
                result = result.Where(c => !c.PlanningMRProposal_ProdOrder.Any());

            if (FilterProdOrderState != null)
                result = result.Where(x => x.MDProdOrderState.MDProdOrderStateIndex == (short)FilterProdOrderState);

            if (SelectedFilterOutputMaterial != null)
                result = result.Where(c => c.ProdOrderPartslist_ProdOrder.Any(p => p.Partslist.Material.MaterialID == SelectedFilterOutputMaterial.MaterialID));
            else if (!string.IsNullOrEmpty(FilterMaterialNoName))
                result = result.Where(c => c.ProdOrderPartslist_ProdOrder.Any(p =>
                    p.Partslist.Material.MaterialNo.Contains(FilterMaterialNoName) ||
                    p.Partslist.Material.MaterialName1.Contains(FilterMaterialNoName)
                ));

            result = FilterProdOrderByPOPosTime(result);


            return result;
        }

        private IQueryable<ProdOrder> FilterProdOrderByPOPosTime(IQueryable<ProdOrder> prodOrders)
        {
            if (ProdOrderPosFilterFrom.HasValue && ProdOrderPosFilterTo.HasValue && SelectedPOPosTimeFilterMode != null)
            {
                DateTime fromDT = ProdOrderPosFilterFrom.Value.Date;
                DateTime toDT = ProdOrderPosFilterTo.Value.Date.AddDays(1).AddTicks(-1);

                if ((POPosTimeFilterTypeEnum)SelectedPOPosTimeFilterMode.Value == POPosTimeFilterTypeEnum.ProdOrderPosStartTime)
                {
                    return prodOrders.Where(c => c.ProdOrderPartslist_ProdOrder.Any(x => x.StartDate >= fromDT && x.StartDate <= toDT));
                }

                if ((POPosTimeFilterTypeEnum)SelectedPOPosTimeFilterMode.Value == POPosTimeFilterTypeEnum.ProdOrderPosEndTime)
                {
                    return prodOrders.Where(c => c.ProdOrderPartslist_ProdOrder.Any(x => x.EndDate >= fromDT && x.EndDate <= toDT));
                }

                if ((POPosTimeFilterTypeEnum)SelectedPOPosTimeFilterMode.Value == POPosTimeFilterTypeEnum.ProdOrderPosStartEndTime)
                {
                    return prodOrders.Where(c => c.ProdOrderPartslist_ProdOrder.Any(x => x.StartDate >= fromDT && x.EndDate <= toDT));
                }
            }

            return prodOrders;
        }

        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        protected ACAccessNav<ProdOrder> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, ProdOrder.ClassName)]
        public virtual ACAccessNav<ProdOrder> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceColumnsIfDifferent(NavigationqueryDefaultFilter, NavigationqueryDefaultSort);
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<ProdOrder>(ProdOrder.ClassName, this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessPrimary;
            }
        }

        protected virtual List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "ProgramNo", Global.LogicalOperators.contains, Global.Operators.or, null, true, true)
                };
            }
        }

        protected virtual List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("ProgramNo", Global.SortDirections.descending, true)
                };
            }
        }

        #endregion

        #region ProdOrder -> Select, (Current,) List

        /// <summary>
        /// Gets or sets the selected production order.
        /// </summary>
        /// <value>The selected production order.</value>
        [ACPropertySelected(600, ProdOrder.ClassName)]
        public ProdOrder SelectedProdOrder
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                if (AccessPrimary.Selected != value)
                {
                    if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                    CurrentProdOrder = value;
                    RefreshProdOrder(value);
                    OnPropertyChanged("SelectedProdOrder");
                }
            }
        }

        /// <summary>
        /// Gets or sets the current production order.
        /// </summary>
        /// <value>The current production order.</value>
        [ACPropertyCurrent(601, ProdOrder.ClassName)]
        public ProdOrder CurrentProdOrder
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return;
                if (AccessPrimary.Current != value)
                {
                    AccessPrimary.Current = value;
                    SelectedProdOrder = value;
                    RefreshProdOrder(value);
                    OnPropertyChanged("CurrentProdOrder");
                }
            }
        }

        private ProdOrder _LastRefreshedProdOrder = null;

        private void RefreshProdOrder(ProdOrder prodOrder)
        {
            if (_LastRefreshedProdOrder != prodOrder)
            {
                PreselectedClear();
                SearchProdOrderPartslist();
                _LastRefreshedProdOrder = prodOrder;
            }
        }

        /// <summary>
        /// Gets the production order list.
        /// </summary>
        /// <value>The production order list.</value>
        [ACPropertyList(602, ProdOrder.ClassName)]
        public IEnumerable<ProdOrder> ProdOrderList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }

        #endregion

        #region ProdOrder -> Methods

        [ACMethodInteraction(ProdOrder.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedProdOrder", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (AccessPrimary == null) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(ProdOrder), ProdOrder.NoColumnName, ProdOrder.FormatNewNo, this);
            ProdOrder prodOrder = ProdOrder.NewACObject(base.DatabaseApp, null, secondaryKey);
            DatabaseApp.ProdOrder.AddObject(prodOrder);
            AccessPrimary.NavList.Insert(0, prodOrder);
            SelectedProdOrder = prodOrder;
            OnPropertyChanged("ProdOrderList");
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(ProdOrder.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentProdOrder", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (AccessPrimary == null)
                return;
            Global.MsgResult result = Messages.Question(this, "Question50012", Global.MsgResult.Yes);
            if (result == Global.MsgResult.Yes)
            {
                var partsList = ProdOrderPartslistList.ToList();
                foreach (var item in partsList)
                {
                    Msg msg = ProdOrderManager.PartslistRemove(DatabaseApp, SelectedProdOrder, item);
                    if (msg != null)
                    {
                        Messages.Msg(msg);
                        return;
                    }
                }

                var selectedItem = SelectedProdOrder;
                if (AccessPrimary == null)
                    return;
                AccessPrimary.NavList.Remove(selectedItem);
                List<PlanningMRProposal> planningMRProposals = selectedItem.PlanningMRProposal_ProdOrder.ToList();
                foreach (var planningMRProposal in planningMRProposals)
                    planningMRProposal.DeleteACObject(DatabaseApp, false);
                selectedItem.DeleteACObject(DatabaseApp, false);
                SelectedProdOrder = AccessPrimary.NavList.FirstOrDefault();
            }
            PostExecute("Delete");
        }

        public bool IsEnabledDelete()
        {
            return SelectedProdOrder != null;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(ProdOrder.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
            OnPropertyChanged("SelectedProdOrder");
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        protected override Msg OnPreSave()
        {
            if (!ConfigManagerIPlus.MustConfigBeReloadedOnServer(this, VisitedMethods, this.Database))
                this.VisitedMethods = null;
            return base.OnPreSave();
        }

        protected override void OnPostSave()
        {
            ConfigManagerIPlus.ReloadConfigOnServerIfChanged(this, VisitedMethods, this.Database);
            this.VisitedMethods = null;
            base.OnPostSave();
        }


        [ACMethodCommand(ProdOrder.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        protected override void OnPostUndoSave()
        {
            this.VisitedMethods = null;
            base.OnPostUndoSave();
        }

        [ACMethodCommand(ProdOrder.ClassName, "en{'Recalculate Totals'}de{'Summen neu berechnen'}", 600, true)]
        public void RecalcAllQuantites()
        {
            if (!IsEnabledRecalcAllQuantites())
                return;
            BackgroundWorker.RunWorkerAsync(nameof(RecalcAllQuantities));
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledRecalcAllQuantites()
        {
            return CurrentProdOrder != null;
        }

        [ACMethodCommand(ProdOrder.ClassName, "en{'Recalculate Totals for all'}de{'Für alle Summen neu berechnen'}", 600, true)]
        public void RecalcAllQuantitesForSelected()
        {
            if (!IsEnabledRecalcAllQuantitesForSelected())
                return;
            BackgroundWorker.RunWorkerAsync(nameof(DoRecalcAllQuantitiesForSelected));
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledRecalcAllQuantitesForSelected()
        {
            return ProdOrderList != null && ProdOrderList.Any(c => c.IsSelected);
        }

        [ACMethodCommand(ProdOrder.ClassName, "en{'Finish Order'}de{'Auftrag beenden'}", 601, true)]
        public virtual Global.MsgResult FinishOrder()
        {
            if (!IsEnabledFinishOrder())
                return Global.MsgResult.None;

            // Es sind noch offene Buchungen vorhanden.
            SearchOpenPostings();
            if (_OpenPostingsList.Count() > 0)
            {
                Msg msgHasOpenPostings = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "FinishOrder(2)",
                    // Es sind noch offene Buchungen vorhanden.
                    // Sie können den Auftrag erst beenden sobald alle Buchungen abgeschlossen sind.
                    Message = Root.Environment.TranslateMessage(this, "Info50035")
                };
                Global.MsgResult msgResultOpenPostings = Messages.Msg(msgHasOpenPostings, Global.MsgResult.OK);
                if (msgResultOpenPostings == Global.MsgResult.OK)
                    return Global.MsgResult.None;
            }

            // "Wollen Sie den Auftrag beenden?"
            Msg msgForAll = new Msg
            {
                Source = GetACUrl(),
                MessageLevel = eMsgLevel.Info,
                ACIdentifier = "FinishOrder(1)",
                Message = Root.Environment.TranslateMessage(this, "Question50042")
            };
            Global.MsgResult questionFinishOrder = Messages.Msg(msgForAll, Global.MsgResult.No, eMsgButton.YesNo);
            if (questionFinishOrder != Global.MsgResult.Yes)
            {
                return Global.MsgResult.None;
            }

            OnFinishOrder();

            ACSaveChanges();
            OnPropertyChanged("CurrentProdOrder");
            return Global.MsgResult.OK;
        }

        public virtual bool IsEnabledFinishOrder()
        {
            if (CurrentProdOrder == null || CurrentProdOrder.MDProdOrderState == null)
                return false;
            if (CurrentProdOrder.MDProdOrderState.ProdOrderState >= MDProdOrderState.ProdOrderStates.ProdFinished)
                return false;
            return true;
        }

        protected virtual void OnFinishOrder()
        {
            if (this.ProdOrderManager != null)
            {
                this.ProdOrderManager.FinishOrder(this.DatabaseApp, CurrentProdOrder);
            }
        }

        #endregion

        #region Open Postings

        #region Open Postings -> Select, (Current,) List

        private OpenPostingsWrapper _SelectedOpenPosting;
        /// <summary>
        /// Selected open posting.
        /// </summary>
        [ACPropertySelected(670, OpenPostingsWrapper.ClassName)]
        public OpenPostingsWrapper SelectedOpenPosting
        {
            get
            {
                return _SelectedOpenPosting;
            }
            set
            {
                if (_SelectedOpenPosting != value)
                {
                    _SelectedOpenPosting = value;
                    //OnPropertyChanged("SelectedOpenPosting");
                }
            }
        }

        private List<OpenPostingsWrapper> _OpenPostingsList;
        /// <summary>
        /// List with open postings in the current ProdOrder.
        /// </summary>
        [ACPropertyList(671, OpenPostingsWrapper.ClassName)]
        public List<OpenPostingsWrapper> OpenPostingsList
        {
            get
            {
                if (_OpenPostingsList == null)
                    _OpenPostingsList = new List<OpenPostingsWrapper>();
                return _OpenPostingsList;
            }
        }

        #region Open Postings -> Methods

        [ACMethodCommand(OpenPostingsWrapper.ClassName, "en{'Check for open postings'}de{'Prüfe auf offene Buchungen'}", 602, true)]
        public void CheckForOpenPostings()
        {
            SearchOpenPostings();
            if (!_OpenPostingsList.Any())
            {
                Msg noOpenPostings = new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Info,
                    ACIdentifier = "OpenPosting",
                    Message = Root.Environment.TranslateMessage(this, "Info50034")
                };
                Global.MsgResult msgResult = Messages.Msg(noOpenPostings, Global.MsgResult.OK, eMsgButton.OK);
                if (msgResult == Global.MsgResult.OK)
                    return;
            }
            ShowDialog(this, "OpenPostingsList");
        }
        /// <summary>
        /// Navigates through the tabs in the BSOProdOrder to the open posting (FacilityPreBooking).
        /// </summary>
        [ACMethodInteraction(OpenPostingsWrapper.ClassName, "en{'Navigate to open posting'}de{'Navigiere zu offener Buchung'}", 603, true, "SelectedOpenPosting")]
        public void NavigateToOpenPosting()
        {
            if (CurrentProdOrder != null && ProdOrderPartslistList != null && IntermediateList != null && BatchList != null)
            {
                // Stückliste auswählen (ProdOrderPartslist)
                ProdOrderPartslist pOPL = ProdOrderPartslistList.Where(c => c.Sequence == SelectedOpenPosting.SequenceBOM).FirstOrDefault();
                if (pOPL != null)
                    SelectedProdOrderPartslist = pOPL;

                // Zwischenprodukt auswählen (SelectedIntermediate)
                // Tab: Intermediate
                ProdOrderPartslistPos intermediateProd = IntermediateList.Where(c => c.Material.MaterialNo == SelectedOpenPosting.IntermediateProd).FirstOrDefault();
                if (intermediateProd != null)
                    SelectedIntermediate = intermediateProd;

                // Batch auswählen (SelectedProdOrderIntermediateBatch)
                ProdOrderPartslistPos intermediateBatch = ProdOrderIntermediateBatchList.Where(c => c.Sequence == SelectedOpenPosting.SequenceBatchNo).FirstOrDefault();
                if (intermediateBatch != null)
                    SelectedProdOrderIntermediateBatch = intermediateBatch;

                // Einsatz oder Ergebnis auswählen
                // Offene Buchung auswählen; Unterscheidung zwischen Einsatz und Ergebnis
                // Tab: IntermedateFormInput
                // Tab: IntermediateFormInputFormPlannedBookings
                if (SelectedOpenPosting.PostingEnum == OpenPostingsWrapper.PostingTypeEnum.Outward)
                {
                    FacilityPreBooking outwardFacilityPreBooking = OutwardFacilityPreBookingList.Where(c => c.FacilityPreBookingID == SelectedOpenPosting.CurrentFacilityPreBooking.FacilityPreBookingID).FirstOrDefault();
                    if (outwardFacilityPreBooking != null)
                    {
                        SelectedOutwardFacilityPreBooking = outwardFacilityPreBooking;
                        ShowDialog(this, "IntermediateFormInputFormPlannedBookings");
                        Save();
                        SearchOpenPostings();
                        if (_OpenPostingsList == null || !_OpenPostingsList.Any())
                            CloseTopDialog();
                    }
                }
                // Tab: IntermediateFormOutput
                // Tab: IntermediateFormOutputPlannedBookings
                else if (SelectedOpenPosting.PostingEnum == OpenPostingsWrapper.PostingTypeEnum.Inward)
                {
                    FacilityPreBooking inwardFacilityPreBooking = InwardFacilityPreBookingList.Where(c => c.FacilityPreBookingID == SelectedOpenPosting.CurrentFacilityPreBooking.FacilityPreBookingID).FirstOrDefault();
                    if (inwardFacilityPreBooking != null)
                    {
                        SelectedInwardFacilityPreBooking = inwardFacilityPreBooking;
                        ShowDialog(this, "IntermediateFormOutputPlannedBookings");
                        Save();
                        SearchOpenPostings();
                        if (_OpenPostingsList == null || !_OpenPostingsList.Any())
                            CloseTopDialog();
                    }
                }
            }
        }

        #endregion

        #region Open Postings -> Search
        public void SearchOpenPostings(OpenPostingsWrapper selectedItem = null)
        {
            _OpenPostingsList = this.DatabaseApp
                .FacilityPreBooking
                .Include(c => c.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material)
                .Include(c => c.ProdOrderPartslistPosReference)
                .Where(c => (c.ProdOrderPartslistPosID.HasValue
                          && c.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProdOrderID == this.CurrentProdOrder.ProdOrderID)
                         || (c.ProdOrderPartslistPosRelationID.HasValue
                          && c.ProdOrderPartslistPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProdOrderID == this.CurrentProdOrder.ProdOrderID))
                .Select(c => new OpenPostingsWrapper()
                {
                    PreBookingNo = c.FacilityPreBookingNo,
                    PreBookingDate = c.InsertDate,

                    MaterialName = c.ProdOrderPartslistPos != null
                        ? c.ProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialName1
                        : c.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ProdOrderPartslist.Partslist.Material.MaterialName1,

                    SequenceBOM = c.ProdOrderPartslistPos != null
                        ? c.ProdOrderPartslistPos.ProdOrderPartslist.Sequence
                        : c.ProdOrderPartslistPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.Sequence,

                    IntermediateProd = c.ProdOrderPartslistPos != null
                         ? c.ProdOrderPartslistPos.Material.MaterialNo
                         : c.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.Material.MaterialNo,

                    SequenceBatchNo = c.ProdOrderPartslistPos != null
                        ? c.ProdOrderPartslistPos.Sequence
                        : c.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.Sequence,

                    CurrentFacilityPreBooking = c,

                    PostingEnum = c.ProdOrderPartslistPosRelationID != null
                        ? OpenPostingsWrapper.PostingTypeEnum.Outward
                        : OpenPostingsWrapper.PostingTypeEnum.Inward
                })
                .ToList();
            if (_OpenPostingsList == null)
                SelectedOpenPosting = null;
            else
            {
                if (selectedItem != null)
                    SelectedOpenPosting = selectedItem;
                else
                    SelectedOpenPosting = OpenPostingsList.FirstOrDefault();
            }
            OnPropertyChanged("OpenPostingsList");
        }

        #endregion

        #endregion

        #endregion

        #region ProdOrder -> Search

        /// <summary>
        /// Searches the delivery note.
        /// </summary>
        [ACMethodCommand(ProdOrder.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search(ProdOrder selectedProdOrder = null, ProdOrderPartslist selectedProdOrderPartslist = null)
        {
            if (AccessPrimary == null)
                return;
            LoadFilterOutputMaterialList();
            AccessPrimary.NavSearch(DatabaseApp);
            if (selectedProdOrder != null)
                SelectedProdOrder = selectedProdOrder;
            PreselectedClear();
            if (selectedProdOrder != null)
                PreselectedProdOrderID = selectedProdOrder.ProdOrderID;
            if (selectedProdOrderPartslist != null)
                PreselectedProdorderPartslistID = selectedProdOrderPartslist.ProdOrderPartslistID;
            SearchProdOrderPartslist();
            OnPropertyChanged("ProdOrderList");
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
            if (acAccess == _AccessInBookingFacility)
            {
                _AccessInBookingFacility.NavSearch(this.DatabaseApp);
                OnPropertyChanged("BookingInwardFacilityList");
                return true;
            }
            else if (acAccess == _AccessOutBookingFacility)
            {
                _AccessOutBookingFacility.NavSearch(this.DatabaseApp);
                OnPropertyChanged("BookingOutwardFacilityList");
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }

        [ACMethodInteraction(ProdOrder.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedProdOrder", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {

            var query =
                DatabaseApp
                .ProdOrder
                .Include(c => c.ProdOrderPartslist_ProdOrder)
                .Where(c => c.ProdOrderID == SelectedProdOrder.ProdOrderID);

            LoadEntity<ProdOrder>(requery, () => SelectedProdOrder, () => CurrentProdOrder, c => CurrentProdOrder = c, query);

            if (requery)
            {
                PreselectedLoadCurrent();
                PreselectedClearSelected();
                SearchProdOrderPartslist();
                PreselectedClear();
                OnPropertyChanged("SelectedProdOrderPartslist");
                OnPropertyChanged("CurrentProdOrderPartListExpand");
            }
            PostExecute("Load");
        }

        #endregion

        #endregion

        #region ProdOrderPartslist

        #region ProdOrderPartslist -> Select, (Current,) List

        private ProdOrderPartslist _SelectedProdOrderPartslist;
        /// <summary>
        /// Selected production order partslist
        /// </summary>
        /// <value>The selected production order.</value>
        [ACPropertySelected(603, ProdOrderPartslist.ClassName)]
        public ProdOrderPartslist SelectedProdOrderPartslist
        {
            get
            {
                return _SelectedProdOrderPartslist;
            }
            set
            {
                if (_SelectedProdOrderPartslist != value)
                {
                    _SelectedProdOrderPartslist = value;

                    SearchProdOrderPartslistPos();
                    RecalculateComponentRemainingQuantity();

                    SearchIntermediate();
                    SearchBatch();

                    this.LoadProcessWorkflows();
                    LoadMaterialWorkflows();
                    OnPropertyChanged("SelectedProdOrderPartslist");
                    OnPropertyChanged("CurrentProdOrderPartListExpand");
                }
            }
        }

        private List<ProdOrderPartslist> _ProdOrderPartslistList;
        /// <summary>
        /// Production order parts list
        /// </summary>
        /// <value>The production order list.</value>
        [ACPropertyList(604, ProdOrderPartslist.ClassName)]
        public List<ProdOrderPartslist> ProdOrderPartslistList
        {
            get
            {
                if (_ProdOrderPartslistList == null)
                    _ProdOrderPartslistList = new List<ProdOrderPartslist>();
                return _ProdOrderPartslistList;
            }
        }

        #endregion

        #region ProdOrderPartslist -> Methods

        #region ProdOrderPartslist - Adding

        [ACMethodInteraction(ProdOrder.ClassName, "en{'Add'}de{'Neu'}", (short)MISort.New, true, "SelectedProdOrderPartslist", Global.ACKinds.MSMethodPrePost)]
        public void AddPartslist()
        {
            if (!PreExecute("AddPartslist")) return;
            AddPartslistSequence = 1;
            AddPartslistSequence = ProdOrderPartslistList.Count() + 1;
            ShowDialog(this, "NewProdOrderPartslistDlg");
            PostExecute("AddPartslist");
        }

        public bool IsEnabledAddPartslist()
        {
            return CurrentProdOrder != null && CurrentProdOrder.EntityState != EntityState.Added;
        }

        [ACMethodInteraction(ProdOrder.ClassName, "en{'OK'}de{'Ok'}", (short)MISort.Okay, true, "SelectedProdOrderPartslist", Global.ACKinds.MSMethodPrePost)]
        public void AddPartslistDlgOk()
        {
            if (!IsEnabledAddPartslistDlgOk()) return;
            Msg isPartslistNotValid = IsPartslistValid(BSOPartslistExplorer_Child.Value.SelectedPartslist);
            if (isPartslistNotValid != null)
            {
                Root.Messages.Msg(isPartslistNotValid);
                return;
            }
            int sequence = AddPartslistSequence ?? 0;
            if (sequence == 0)
                sequence = ProdOrderPartslistList.Count() + 1;
            ProdOrderPartslist prodOrderPartslist = null;
            Msg msg = ProdOrderManager.PartslistAdd(DatabaseApp, SelectedProdOrder, BSOPartslistExplorer_Child.Value.SelectedPartslist, AddPartslistSequence.Value, AddPartslistTargetQuantity.Value, out prodOrderPartslist);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            Save();
            PreselectedProdorderPartslistID = prodOrderPartslist.ProdOrderPartslistID;
            SearchProdOrderPartslist();
            CloseTopDialog();
        }

        public bool IsEnabledAddPartslistDlgOk()
        {
            return BSOPartslistExplorer_Child != null
                && BSOPartslistExplorer_Child.Value != null
                && BSOPartslistExplorer_Child.Value.SelectedPartslist != null
                && AddPartslistSequence.HasValue
                && AddPartslistSequence.Value > 0
                && AddPartslistTargetQuantity.HasValue
                && AddPartslistTargetQuantity.Value > 0;
        }

        public Msg IsPartslistValid(Partslist partslist)
        {
            Msg msg = null;
            if (!partslist.IsEnabled)
                msg = new Msg(this, eMsgLevel.Error, "Partslist", "AddPartslistDlgOk", 595, "Error50328", partslist.PartslistNo);
            else
            {
                if (!partslist.IsInEnabledPeriod ?? false)
                    msg = new Msg(this, eMsgLevel.Error, "Partslist", "AddPartslistDlgOk", 595, "Error50329", partslist.PartslistNo, partslist.EnabledFrom, partslist.EnabledTo);
            }
            return msg;
        }

        [ACMethodInteraction(ProdOrder.ClassName, "en{'Cancel'}de{'Beenden'}", (short)MISort.Cancel, true, "SelectedDataItem", Global.ACKinds.MSMethodPrePost)]
        public void AddPartslistDlgCancel()
        {
            CloseTopDialog();
        }

        #endregion

        #region ProdOrderPartslist - Delete

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(ProdOrderPartslist.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentProdOrderPartslist", Global.ACKinds.MSMethodPrePost)]
        public void DeleteProdOrderPartslist()
        {
            if (!PreExecute("DeleteProdOrderPartslist")) return;
            Global.MsgResult result = Messages.Question(this, "Question50011", Global.MsgResult.Yes, false, SelectedProdOrderPartslist.Partslist.PartslistNo);
            if (result == Global.MsgResult.Yes)
            {
                ProdOrderPartslistList.Remove(SelectedProdOrderPartslist);
                Msg msg = ProdOrderManager.PartslistRemove(DatabaseApp, SelectedProdOrder, SelectedProdOrderPartslist);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }
                if (ProdOrderPartslistList.Any())
                {
                    SequenceManager<ProdOrderPartslist>.Order(ref _ProdOrderPartslistList);
                }
                SelectedProdOrderPartslist = ProdOrderPartslistList.FirstOrDefault();
            }
            OnPropertyChanged("ProdOrderPartslistList");
            PostExecute("DeleteProdOrderPartslist");
        }

        public bool IsEnabledDeleteProdOrderPartslist()
        {
            return SelectedProdOrderPartslist != null;
        }

        #endregion

        #region ProdOrderPartslist - Starting
        [ACMethodInteraction(ProdOrderPartslist.ClassName, "en{'Start'}de{'Starten'}", (short)MISort.Start, true, "CurrentProdOrderPartslist", Global.ACKinds.MSMethodPrePost)]
        public virtual void StartProdOrderPartslist()
        {
            if (!PreExecute("StartProdOrderPartslist")) return;

            // TODO: Verification checks

            PostExecute("StartProdOrderPartslist");
        }

        public virtual bool IsEnabledStartProdOrderPartslist()
        {
            return SelectedProdOrderPartslist != null;
        }
        #endregion

        #endregion

        #region ProdOrderPartslist -> Search

        /// <summary>
        /// Search prod order partslist
        /// </summary>
        public void SearchProdOrderPartslist()
        {
            _ProdOrderPartslistList = null;
            if (CurrentProdOrder != null)
            {
                _ProdOrderPartslistList = DatabaseApp
                    .ProdOrderPartslist
                    .Include(x => x.Partslist)
                    .Include(x => x.Partslist.MaterialWF)
                    .Include(x => x.Partslist.Material)
                    .Include(x => x.MDProdOrderState)
                    .Where(x => x.ProdOrderID == CurrentProdOrder.ProdOrderID)
                    .OrderBy(x => x.Sequence)
                    .AutoMergeOption()
                    .ToList();

                if (PreselectedProdorderPartslistID == null)
                    SelectedProdOrderPartslist = _ProdOrderPartslistList.FirstOrDefault();
                else
                    SelectedProdOrderPartslist = _ProdOrderPartslistList.FirstOrDefault(c => c.ProdOrderPartslistID == PreselectedProdorderPartslistID);
            }
            else
            {
                SelectedProdOrderPartslist = null;
            }
            OnPropertyChanged("ProdOrderPartslistList");
        }

        #endregion

        #region ProdOrderPartslist -> Change quantity

        #region ProdOrderPartslist -> Change quantity -> Methods
        [ACMethodInteraction(ProdOrder.ClassName, "en{'Change Quantity'}de{'Menge ändern'}", 604, true, "SelectedProdOrderPartslist", Global.ACKinds.MSMethodPrePost)]
        public void PartslistChangeTargetQuantityDlg()
        {
            ShowDialog(this, "PartslistChangeTargetQuantityDlg");
        }


        [ACMethodInteraction(ProdOrder.ClassName, "en{'Ok'}de{'Ok'}", (short)MISort.Okay, true, "SelectedProdOrderPartslist", Global.ACKinds.MSMethodPrePost)]
        public void PartslistChangeTargetQuantityDlgOk()
        {
            if (RecalculateQuantities)
            {
                if (_ProdOrderPartslistPosList != null && _ProdOrderPartslistPosList.Any())
                {
                    foreach (var outwardRootPosItem in _ProdOrderPartslistPosList)
                    {
                        outwardRootPosItem.PropertyChanged -= OutwardRootPosItem_PropertyChanged;
                    }
                }

                Msg msg = ProdOrderManager.ProdOrderPartslistChangeTargetQuantity(DatabaseApp, SelectedProdOrderPartslist, PartslistChangeTargetQuantityInput.Value);

                if (_ProdOrderPartslistPosList != null && _ProdOrderPartslistPosList.Any())
                {
                    foreach (var outwardRootPosItem in _ProdOrderPartslistPosList)
                    {
                        outwardRootPosItem.PropertyChanged += OutwardRootPosItem_PropertyChanged;
                    }
                }

                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }
                PreselectedProdorderPartslistID = SelectedProdOrderPartslist?.ProdOrderPartslistID;
                SearchProdOrderPartslist();
            }
            else
            {
                SelectedProdOrderPartslist.TargetQuantity = PartslistChangeTargetQuantityInput.Value;
                OnPropertyChanged("SelectedProdOrderPartslist");
                OnPropertyChanged("ProdOrderPartslistList");
            }
            CloseTopDialog();
        }

        [ACMethodInteraction(ProdOrder.ClassName, "en{'Cancel'}de{'Schliessen'}", (short)MISort.Cancel, true, "SelectedProdOrderPartslist", Global.ACKinds.MSMethodPrePost)]
        public void PartslistChangeTargetQuantityDlgCancel()
        {
            PartslistChangeTargetQuantityInput = null;
            CloseTopDialog();
        }

        #region ProdOrderPartslist -> Change quantity -> Methods -> IsEnabled
        public bool IsEnabledPartslistChangeTargetQuantityDlg()
        {
            return SelectedProdOrderPartslist != null;
        }

        public bool IsEnabledPartslistChangeTargetQuantityDlgOk()
        {
            return SelectedProdOrderPartslist != null &&
                PartslistChangeTargetQuantityInput.HasValue &&
                PartslistChangeTargetQuantityInput.Value > 0;
        }
        #endregion

        #endregion

        private double? _PartslistChangeTargetQuantityInput;
        [ACPropertyInfo(605, ProdOrder.ClassName, "en{'New Target Quantity'}de{'Neue Sollmenge'}")]
        public double? PartslistChangeTargetQuantityInput
        {
            get
            {
                return _PartslistChangeTargetQuantityInput;
            }
            set
            {
                if (_PartslistChangeTargetQuantityInput != value)
                {
                    _PartslistChangeTargetQuantityInput = value;
                    OnPropertyChanged("PartslistChangeTargetQuantityInput");
                }
            }
        }

        private bool _RecalculateQuantities = true;
        /// <summary>
        /// Doc  MakeQuantityRecalculation
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "RecalculateQuantities", "en{'Recalc all quantities'}de{'Alle Mengen neu berechnen'}")]
        public bool RecalculateQuantities
        {
            get
            {
                return _RecalculateQuantities;
            }
            set
            {
                if (_RecalculateQuantities != value)
                {
                    _RecalculateQuantities = value;
                    OnPropertyChanged("RecalculateQuantities");
                }
            }
        }

        #endregion

        #endregion

        #region ProdOrderPartListExpand
        private PartslistExpand rootProdOrderPartListExpand;
        private PartslistExpand _CurrentProdOrderPartListExpand;
        /// <summary>
        /// 
        /// </summary>
        [ACPropertyCurrent(606, "ProdOrderPartListExpand")]
        public PartslistExpand CurrentProdOrderPartListExpand
        {
            get
            {
                return _CurrentProdOrderPartListExpand;
            }
            set
            {
                if (_CurrentProdOrderPartListExpand != value)
                {
                    _CurrentProdOrderPartListExpand = value;
                    _ProdOrderPartListExpandList = null;
                }
            }
        }

        private List<PartslistExpand> _ProdOrderPartListExpandList;
        [ACPropertyList(607, "ProdOrderPartListExpand")]
        public List<PartslistExpand> ProdOrderPartListExpandList
        {
            get
            {
                if (_ProdOrderPartListExpandList == null)
                    _ProdOrderPartListExpandList = new List<PartslistExpand>();
                if (CurrentProdOrderPartListExpand != null)
                    _ProdOrderPartListExpandList.Add(CurrentProdOrderPartListExpand);
                return _ProdOrderPartListExpandList;
            }
        }


        #region ProdOrderPartListExpand -> Methods

        [ACMethodInteraction("ProdOrderPartListExpand", "en{'BOM-Expand'}de{'Auflösen'}", 605, true, "SelectedDataItem", Global.ACKinds.MSMethodPrePost)]
        public void BOMExplosion()
        {
            double treeQuantityRatio = SelectedProdOrderPartslist.TargetQuantity / SelectedProdOrderPartslist.Partslist.TargetQuantityUOM;
            rootProdOrderPartListExpand = new PartslistExpand(SelectedProdOrderPartslist.Partslist, treeQuantityRatio);
            rootProdOrderPartListExpand.IsChecked = true;
            rootProdOrderPartListExpand.LoadTree();
            rootProdOrderPartListExpand.IsEnabled = false;
            rootProdOrderPartListExpand.IsChecked = false;

            _ProdOrderPartListExpandList = null;
            _CurrentProdOrderPartListExpand = rootProdOrderPartListExpand;

            ShowDialog(this, "ProdOrderPartslistExpandDlg");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsEnabledBOMExplosion()
        {
            return SelectedProdOrderPartslist != null;
        }


        [ACMethodInteraction("ConnectSourceProdOrderPartslist", "en{'Connect production order lines'}de{'Produktionsauftragszeilen verbinden'}", 606, true, "SelectedProdOrder", Global.ACKinds.MSMethodPrePost)]
        public void ConnectSourceProdOrderPartslist()
        {
            if (!IsEnabledConnectSourceProdOrderPartslist())
                return;
            ProdOrderManager.ConnectSourceProdOrderPartslist(SelectedProdOrder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsEnabledConnectSourceProdOrderPartslist()
        {
            return SelectedProdOrder != null;
        }

        [ACMethodInteraction("ProdOrderPartListExpand", "en{'OK'}de{'OK'}", (short)MISort.Okay, true, "SelectedProdOrderPartslist", Global.ACKinds.MSMethodPrePost)]
        public void BOMExplosionDlgOk()
        {
            List<ExpandResult> treeResult = rootProdOrderPartListExpand.BuildTreeList();
            treeResult =
                treeResult
                .Where(x =>
                    x.Item.IsChecked
                    && x.Item.IsEnabled)
                .OrderBy(x => x.TreeVersion)
                .ToList();
            if (treeResult.Any())
            {
                int selectedSequence = treeResult.Count;
                SelectedProdOrderPartslist.Sequence = selectedSequence + 1;
                foreach (ExpandResult item in treeResult)
                {
                    ProdOrderPartslist prodOrderPartslist = null;
                    Msg msg = ProdOrderManager.PartslistAdd(DatabaseApp, SelectedProdOrder, item.Item.PartslistForPosition, selectedSequence, item.Item.TargetQuantityUOM, out prodOrderPartslist);
                    selectedSequence--;
                }
                ProdOrderManager.ConnectSourceProdOrderPartslist(SelectedProdOrder);
                Save();
                SearchProdOrderPartslist();
            }
            CloseTopDialog();
        }

        public bool IsEnabledBOMExplosionDlgOk()
        {
            return true;
        }

        [ACMethodInteraction("ProdOrderPartListExpand", "en{'Cancel'}de{'Beenden'}", (short)MISort.Cancel, true, "SelectedDataItem", Global.ACKinds.MSMethodPrePost)]
        public void BOMExplosionDlgCancel()
        {
            CloseTopDialog();
        }


        #endregion

        #endregion

        #region ProdOrderPartslistPos

        #region ProdOrderPartslistPos -> Select, (Current,) List

        private ProdOrderPartslistPos _SelectedProdOrderPartslistPos;
        /// <summary>
        /// Selected production order partslist
        /// </summary>
        /// <value>The selected production order.</value>
        [ACPropertySelected(608, ProdOrderPartslistPos.ClassName)]
        public ProdOrderPartslistPos SelectedProdOrderPartslistPos
        {
            get
            {
                return _SelectedProdOrderPartslistPos;
            }
            set
            {
                if (_SelectedProdOrderPartslistPos != value)
                {
                    if (_SelectedProdOrderPartslistPos != null)
                        _SelectedProdOrderPartslistPos.PropertyChanged -= _SelectedProdOrderPartslistPos_PropertyChanged;
                    _SelectedProdOrderPartslistPos = value;
                    if (_SelectedProdOrderPartslistPos != null)
                        _SelectedProdOrderPartslistPos.PropertyChanged += _SelectedProdOrderPartslistPos_PropertyChanged;
                    SearchAlternative();
                    OnPropertyChanged("SelectedProdOrderPartslistPos");
                    OnPropertyChanged("SelectedInputMaterial");

                    _ComponentBasedOnPlPosList = null;
                    _SelectedComponentBasedOnPlPos = null;
                    if (value != null)
                        _SelectedComponentBasedOnPlPos = ComponentBasedOnPlPosList.Where(c => c.PartslistPosID == value.BasedOnPartslistPosID).FirstOrDefault();

                    OnPropertyChanged("SelectedComponentBasedOnPlPos");
                    OnPropertyChanged("ComponentBasedOnPlPosList");

                }
            }
        }

        private void _SelectedProdOrderPartslistPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MaterialID")
            {
                OnPropertyChanged("SelectedProdOrderPartslistPos");
            }
        }

        private ObservableCollection<ProdOrderPartslistPos> _ProdOrderPartslistPosList;
        /// <summary>
        /// Production order parts list
        /// </summary>
        /// <value>The production order list.</value>
        [ACPropertyList(609, ProdOrderPartslistPos.ClassName)]
        public ObservableCollection<ProdOrderPartslistPos> ProdOrderPartslistPosList
        {
            get
            {
                if (_ProdOrderPartslistPosList == null)
                    _ProdOrderPartslistPosList = new ObservableCollection<ProdOrderPartslistPos>();
                return _ProdOrderPartslistPosList;
            }
        }

        #endregion

        #region ProdOrderPartslistPos -> ComponentBasedOnPlPos


        #region ComponentBasedOnPlPos
        private PartslistPos _SelectedComponentBasedOnPlPos;
        /// <summary>
        /// Selected property for PartslistPos
        /// </summary>
        /// <value>The selected ComponentBasedOnPlPos</value>
        [ACPropertySelected(9999, "ComponentBasedOnPlPos", "en{'Based on BM. line'}de{'Basiert auf Stückl. Linie'}")]
        public PartslistPos SelectedComponentBasedOnPlPos
        {
            get
            {
                return _SelectedComponentBasedOnPlPos;
            }
            set
            {
                if (_SelectedComponentBasedOnPlPos != value)
                {
                    _SelectedComponentBasedOnPlPos = value;
                    if (SelectedProdOrderPartslistPos != null)
                    {
                        if (value != null)
                            SelectedProdOrderPartslistPos.BasedOnPartslistPos = value;
                        else
                            SelectedProdOrderPartslistPos.BasedOnPartslistPos = null;
                    }

                    OnPropertyChanged("SelectedComponentBasedOnPlPos");
                }
            }
        }


        private List<PartslistPos> _ComponentBasedOnPlPosList;
        /// <summary>
        /// List property for PartslistPos
        /// </summary>
        /// <value>The ComponentBasedOnPlPos list</value>
        [ACPropertyList(9999, "ComponentBasedOnPlPos")]
        public List<PartslistPos> ComponentBasedOnPlPosList
        {
            get
            {
                if (_ComponentBasedOnPlPosList == null)
                    _ComponentBasedOnPlPosList = LoadComponentBasedOnPlPosList();
                return _ComponentBasedOnPlPosList;
            }
        }

        private List<PartslistPos> LoadComponentBasedOnPlPosList()
        {
            if (SelectedProdOrderPartslist == null)
                return new List<PartslistPos>();
            List<Guid> allocatedPlPosIDs = new List<Guid>();
            if (ProdOrderPartslistPosList != null && ProdOrderPartslistPosList.Any())
            {
                allocatedPlPosIDs = ProdOrderPartslistPosList.Where(c => c.BasedOnPartslistPos != null).Select(c => c.BasedOnPartslistPosID ?? Guid.Empty).ToList();
                if (SelectedProdOrderPartslistPos != null && SelectedProdOrderPartslistPos.BasedOnPartslistPos != null)
                    allocatedPlPosIDs.Remove(SelectedProdOrderPartslistPos.BasedOnPartslistPosID ?? Guid.Empty);
            }
            return SelectedProdOrderPartslist.Partslist.PartslistPos_Partslist.Where(c => !allocatedPlPosIDs.Contains(c.PartslistPosID)).ToList();
        }
        #endregion


        #endregion

        #region ProdOrderPartslist -> Change with Partlist Quantities [ChangeViaPartslist]

        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "ChangeViaPartslistPosTargetQuantityUOM", "en{'Original target quantity according to BOM'}de{'Ursprüngliche Sollmenge laut Stückliste'}")]
        public double ChangeViaPartslistPosTargetQuantityUOM
        {
            get
            {
                if (SelectedProdOrderPartslistPos == null || SelectedProdOrderPartslistPos.BasedOnPartslistPos == null)
                    return 0;
                return SelectedProdOrderPartslistPos.BasedOnPartslistPos.TargetQuantityUOM;
            }
        }

        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "ChangeViaPartslistPlTargetQuantityUOM", "en{'BOM reference size'}de{'Stücklistenbezugsgröße'}")]
        public double ChangeViaPartslistPlTargetQuantityUOM
        {
            get
            {
                if (SelectedProdOrderPartslistPos == null)
                    return 0;
                return SelectedProdOrderPartslistPos.ProdOrderPartslist.Partslist.TargetQuantityUOM;
            }
        }

        private double _ChangeViaPartslistNewTargetQuantityUOM;
        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "ChangeViaPartslistNewQuantityUOM", "en{'New target quantity according to BOM'}de{'Neue Sollmenge laut Stückliste'}")]
        public double ChangeViaPartslistNewTargetQuantityUOM
        {
            get
            {
                return _ChangeViaPartslistNewTargetQuantityUOM;
            }
            set
            {
                if (_ChangeViaPartslistNewTargetQuantityUOM != value)
                {
                    _ChangeViaPartslistNewTargetQuantityUOM = value;
                    OnPropertyChanged("ChangeViaPartslistNewTargetQuantityUOM");
                    OnPropertyChanged("ChangeViaPartslistNewOrderTargetQuantityUOM");
                }
            }
        }

        /// <summary>
        /// Selected property for 
        /// </summary>
        /// <value>The selected </value>
        [ACPropertyInfo(999, "ChangeViaPartslistNewOrderQuantityUOM", "en{'New target quantity in order'}de{'Neue Sollmenge im Auftrag:'}")]
        public double ChangeViaPartslistNewOrderTargetQuantityUOM
        {
            get
            {
                if (SelectedProdOrderPartslistPos.ProdOrderPartslist.Partslist.TargetQuantityUOM == 0)
                    return 0;
                double factorOrder = SelectedProdOrderPartslistPos.ProdOrderPartslist.TargetQuantity / SelectedProdOrderPartslistPos.ProdOrderPartslist.Partslist.TargetQuantityUOM;
                return ChangeViaPartslistNewTargetQuantityUOM * factorOrder;
            }
        }

        #endregion

        #region ProdOrderPartslistPos -> Methods

        #region ProdOrderPartslistPos -> Methods -> Manipulate

        [ACMethodInteraction(ProdOrderPartslistPos.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedProdOrderPartslistPos", Global.ACKinds.MSMethodPrePost)]
        public void NewProdOrderPartslistPos()
        {
            if (!PreExecute("NewProdOrderPartslistPos")) return;
            ProdOrderPartslistPos newComponent = ProdOrderPartslistPos.NewACObject(DatabaseApp, SelectedProdOrderPartslist);
            newComponent.Sequence = 1;
            if (ProdOrderPartslistPosList != null)
            {
                newComponent.Sequence = ProdOrderPartslistPosList.Max(x => x.Sequence) + 1;
            }
            DatabaseApp.ProdOrderPartslistPos.AddObject(newComponent);
            PreselectedProdorderPartslistID = newComponent.ProdOrderPartslistPosID;
            SearchProdOrderPartslistPos();
            PostExecute("NewProdOrderPartslistPos");
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(ProdOrderPartslistPos.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentProdOrderPartslist", Global.ACKinds.MSMethodPrePost)]
        public void DeleteProdOrderPartslistPos()
        {
            if (!PreExecute("DeleteProdOrderPartslistPos")) return;
            Msg msg = SelectedProdOrderPartslistPos.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
            }
            else
            {
                SearchProdOrderPartslistPos();
            }
            PostExecute("DeleteProdOrderPartslistPos");
        }

        [ACMethodInteraction("Dialog", "en{'New Lab Order'}de{'Neuer Laborauftrag'}", (short)MISort.New, false, "CreateNewLabOrderFromProdOrderPartslist", Global.ACKinds.MSMethodPrePost)]
        public void CreateNewLabOrderFromProdOrderPartslist()
        {
            ACComponent childBSO = ACUrlCommand("?LabOrderDialogProd") as ACComponent;
            if (childBSO == null)
            {
                childBSO = StartComponent("LabOrderDialogProd", null, new object[] { }) as ACComponent;
                if (SelectedProdOrderIntermediateBatch != null)
                    childBSO.ExecuteMethod(nameof(BSOLabOrder.NewLabOrderDialog), null, null, SelectedProdOrderIntermediateBatch, null);
                else
                    childBSO.ExecuteMethod(nameof(BSOLabOrder.NewLabOrderDialog), null, null, SelectedIntermediate, null);
            }
            if (childBSO == null)
            {
                return;
            }
        }

        public bool IsEnabledCreateNewLabOrderFromProdOrderPartslist()
        {
            return SelectedIntermediate != null || SelectedProdOrderIntermediateBatch != null;
            //if (SelectedIntermediate != null)
            //{
            //    if (SelectedIntermediate.LabOrder_ProdOrderPartslistPos.Any() && SelectedProdOrderIntermediateBatch == null)
            //        return false;
            //    else if (SelectedProdOrderIntermediateBatch != null)
            //    {
            //        if (SelectedProdOrderIntermediateBatch.LabOrder_ProdOrderPartslistPos.Any())
            //            return false;
            //        else
            //            return true;
            //    }
            //    else
            //        return true;
            //}
            //return false;
        }

        [ACMethodInfo("Dialog", "en{'Lab Report View'}de{'Laborbericht'}", (short)MISort.QueryPrintDlg)]
        public void ShowLabOrderFromProdOrder()
        {
            ACComponent childBSO = ACUrlCommand("?LabOrderViewDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("LabOrderViewDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            if (SelectedProdOrderIntermediateBatch != null)
                childBSO.ExecuteMethod(nameof(BSOLabOrder.ShowLabOrderViewDialog), null, null, SelectedProdOrderIntermediateBatch, null, null, true, null);
            else
                childBSO.ExecuteMethod(nameof(BSOLabOrder.ShowLabOrderViewDialog), null, null, SelectedIntermediate, null, null, true, null);
            childBSO.Stop();
        }

        public bool IsEnabledShowLabOrderFromProdOrder()
        {
            if (SelectedIntermediate != null)
            {
                if (!SelectedIntermediate.LabOrder_ProdOrderPartslistPos.Any() && SelectedProdOrderIntermediateBatch == null)
                    return false;
                else if (SelectedProdOrderIntermediateBatch != null)
                {
                    if (!SelectedProdOrderIntermediateBatch.LabOrder_ProdOrderPartslistPos.Any())
                        return false;
                    else
                        return true;
                }
                else
                    return true;
            }
            return false;
        }

        [ACMethodInfo("Dialog", "en{'Lab Report MES View'}de{'Laborbericht MES'}", (short)MISort.QueryPrintDlg)]
        public void ShowLabOrderMESFromProdOrder()
        {
            ACComponent childBSO = ACUrlCommand("?LabOrderMESViewDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("LabOrderMESViewDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            if (SelectedProdOrderIntermediateBatch != null)
                childBSO.ExecuteMethod(nameof(BSOLabOrder.ShowLabOrderViewDialog), null, null, SelectedProdOrderIntermediateBatch, null, null, true, null);
            else
                childBSO.ExecuteMethod(nameof(BSOLabOrder.ShowLabOrderViewDialog), null, null, SelectedIntermediate, null, null, true, null);
            childBSO.Stop();
        }

        public bool IsEnabledShowLabOrderMESFromProdOrder()
        {
            if (SelectedIntermediate != null)
            {
                if (!SelectedIntermediate.LabOrder_ProdOrderPartslistPos.Any() && SelectedProdOrderIntermediateBatch == null)
                    return false;
                else if (SelectedProdOrderIntermediateBatch != null)
                {
                    if (!SelectedProdOrderIntermediateBatch.LabOrder_ProdOrderPartslistPos.Any())
                        return false;
                    else
                        return true;
                }
                else
                    return true;
            }
            return false;
        }

        #endregion

        #region ProdOrderPartslistPos -> Methods -> IsEnabled
        public bool IsEnabledNewProdOrderPartslistPos()
        {
            return SelectedProdOrderPartslist != null;
        }

        public bool IsEnabledDeleteProdOrderPartslistPos()
        {
            return SelectedProdOrderPartslistPos != null;
        }
        #endregion

        #region ProdOrderPartslistPos -> Methods -> Change with Partlist Quantities [ChangeViaPartslist]

        /// <summary>
        /// ChangeViaPartslistDlg
        /// </summary>
        [ACMethodInfo("ChangeViaPartslistDlg", "en{'Change target quantity according to BOM'}de{'Sollmenge entsprechend Stückliste ändern'}", 999)]
        public void ChangeViaPartslistDlg()
        {
            if (!IsEnabledChangeViaPartslistDlg())
                return;
            ShowDialog(this, "ChangeViaPartslistDlg");
        }

        /// <summary>
        /// IsEnabledChangeViaPartslistDlg
        /// </summary>
        /// <returns><c>true</c> if [is enabled new SubPropertyName]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledChangeViaPartslistDlg()
        {
            if (SelectedProdOrderPartslistPos != null)
                return true;
            return false;
        }

        /// <summary>
        /// ChangeViaPartslistDlg
        /// </summary>
        [ACMethodInfo("ChangeViaPartslistOk", "en{'Ok'}de{'Ok'}", 999)]
        public void ChangeViaPartslistOk()
        {
            if (!IsEnabledChangeViaPartslistOk())
                return;
            SelectedProdOrderPartslistPos.TargetQuantityUOM = ChangeViaPartslistNewOrderTargetQuantityUOM;
            CloseTopDialog();
            OnPropertyChanged("SelectedProdOrderPartslistPos");
            OnPropertyChanged("SelectedProdOrderPartslistPos\\TargetQuantityUOM");
        }

        /// <summary>
        /// IsEnabledChangeViaPartslistDlg
        /// </summary>
        /// <returns><c>true</c> if [is enabled new SubPropertyName]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledChangeViaPartslistOk()
        {
            return ChangeViaPartslistNewOrderTargetQuantityUOM > 0;
        }


        /// <summary>
        /// ChangeViaPartslistDlg
        /// </summary>
        [ACMethodInfo("ChangeViaPartslistCancel", "en{'Cancel'}de{'Abbrechen'}", 999)]
        public void ChangeViaPartslistCancel()
        {
            CloseTopDialog();
        }

        #endregion

        #endregion

        #region ProdOrderPartslistPos -> Search
        public void SearchProdOrderPartslistPos()
        {
            _ProdOrderPartslistPosList = null;
            if (SelectedProdOrderPartslist != null)
            {
                var baseItems = DatabaseApp
               .ProdOrderPartslistPos
               .Include(x => x.Material)
               .Include(x => x.Material.BaseMDUnit)
               .Include(x => x.MDUnit)
               .Where(x =>
                    x.ProdOrderPartslistID == SelectedProdOrderPartslist.ProdOrderPartslistID &&
                    x.AlternativeProdOrderPartslistPosID == null &&
                    x.MaterialPosTypeIndex == (short)(GlobalApp.MaterialPosTypes.OutwardRoot) &&
                    x.ParentProdOrderPartslistPosID == null &&
                    x.AlternativeProdOrderPartslistPosID == null)
                .OrderBy(x => x.Sequence)
                .AutoMergeOption()
                .ToList()
                .Where(x => x.EntityState != EntityState.Deleted)
                .ToList();

                var localItems = DatabaseApp.GetAddedEntities<ProdOrderPartslistPos>()
                    .Where(x =>
                    x.ProdOrderPartslistID == SelectedProdOrderPartslist.ProdOrderPartslistID &&
                    x.AlternativeProdOrderPartslistPosID == null &&
                    x.MaterialPosTypeIndex == (short)(GlobalApp.MaterialPosTypes.OutwardRoot) &&
                    x.ParentProdOrderPartslistPosID == null &&
                    x.AlternativeProdOrderPartslistPosID == null)
                .OrderBy(x => x.Sequence)
                .ToList();

                var list = baseItems.Union(localItems).ToList();
                _ProdOrderPartslistPosList = new ObservableCollection<ProdOrderPartslistPos>(list);
                if (_ProdOrderPartslistPosList != null && _ProdOrderPartslistPosList.Any())
                {
                    foreach (var outwardRootPosItem in _ProdOrderPartslistPosList)
                    {
                        outwardRootPosItem.PropertyChanged -= OutwardRootPosItem_PropertyChanged;
                        outwardRootPosItem.PropertyChanged += OutwardRootPosItem_PropertyChanged;
                    }
                }
            }
            if (_ProdOrderPartslistPosList == null)
                SelectedProdOrderPartslistPos = null;
            else
            {
                if (PreselectedOutwarPosID != null)
                    SelectedProdOrderPartslistPos = _ProdOrderPartslistPosList.FirstOrDefault(c => c.ProdOrderPartslistPosID == PreselectedOutwarPosID);
                else
                    SelectedProdOrderPartslistPos = _ProdOrderPartslistPosList.FirstOrDefault();
            }

            OnPropertyChanged("ProdOrderPartslistPosList");
        }

        private void OutwardRootPosItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TargetQuantityUOM")
            {
                ProdOrderPartslistPos outwardRootPosItem = sender as ProdOrderPartslistPos;
                if (outwardRootPosItem.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any()
                    && outwardRootPosItem.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Count() == 1)
                {
                    ProdOrderPartslistPosRelation relation = outwardRootPosItem.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.FirstOrDefault();
                    relation.TargetQuantityUOM = outwardRootPosItem.TargetQuantityUOM;
                }
            }
        }
        #endregion

        #region ProdOrderPartslistPos -> Additional lists

        #region ProdOrderPartslistPos -> ACPropertyAccess(InputMaterial)
        ACAccess<Material> _AccessInputMaterial;
        [ACPropertyAccess(691, "InputMaterial")]
        public ACAccess<Material> AccessInputMaterial
        {
            get
            {
                if (_AccessInputMaterial == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Material.ClassName, ACType.ACIdentifier);
                    if (navACQueryDefinition.ACFilterColumns.Count > 0)
                    {
                        foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            filterItem.SearchWord = "";
                        }
                    }
                    _AccessInputMaterial = navACQueryDefinition.NewAccessNav<Material>(Material.ClassName, this);
                    _AccessInputMaterial.NavSearchExecuting += _AccessInputMaterial_NavSearchExecuting;
                    _AccessInputMaterial.NavSearch();
                }
                return _AccessInputMaterial;
            }
        }

        private IQueryable<Material> _AccessInputMaterial_NavSearchExecuting(IQueryable<Material> result)
        {
            ObjectQuery<Material> query = result as ObjectQuery<Material>;
            if (query != null)
                query.Include(c => c.BaseMDUnit);
            return result.Where(x => !(x.MaterialWFRelation_TargetMaterial.Any() || x.MaterialWFRelation_SourceMaterial.Any()));
        }

        /// <summary>
        /// All available materials for input in partslist
        /// </summary>
        [ACPropertyList(610, "InputMaterial")]
        public IEnumerable<Material> InputMaterialList
        {
            get
            {
                if (AccessInputMaterial == null)
                    return null;
                return AccessInputMaterial.NavList;
            }
        }


        [ACPropertySelected(611, "InputMaterial", "en{'Material'}de{'Material'}")]
        public Material SelectedInputMaterial
        {
            get
            {
                if (SelectedProdOrderPartslistPos == null) return null;
                return SelectedProdOrderPartslistPos.Material;
            }
            set
            {
                if (SelectedProdOrderPartslistPos != null && SelectedProdOrderPartslistPos.Material != value)
                {
                    SelectedProdOrderPartslistPos.Material = value;
                    OnPropertyChanged("SelectedInputMaterial");
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region ProdOrderAlternativePartslistPos

        #region AlternativeProdOrderPartslistPos -> Select, (Current,) List

        private ProdOrderPartslistPos _AlternativeSelectedProdOrderPartslistPos;
        /// <summary>
        /// Gets or sets the selected partslist.
        /// </summary>
        /// <value>The selected partslist.</value>
        [ACPropertySelected(612, "AlternativeProdOrderPartslistPos")]
        public ProdOrderPartslistPos AlternativeSelectedProdOrderPartslistPos
        {
            get
            {
                return _AlternativeSelectedProdOrderPartslistPos;
            }
            set
            {
                if (_AlternativeSelectedProdOrderPartslistPos != value)
                {
                    if (_AlternativeSelectedProdOrderPartslistPos != null)
                        _AlternativeSelectedProdOrderPartslistPos.PropertyChanged -= _AlternativeSelectedProdOrderPartslistPos_PropertyChanged;
                    _AlternativeSelectedProdOrderPartslistPos = value;
                    if (_AlternativeSelectedProdOrderPartslistPos != null)
                        _AlternativeSelectedProdOrderPartslistPos.PropertyChanged += _AlternativeSelectedProdOrderPartslistPos_PropertyChanged;
                    OnPropertyChanged("AlternativeSelectedProdOrderPartslistpos");
                }
            }
        }

        private void _AlternativeSelectedProdOrderPartslistPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MaterialID")
            {
                OnPropertyChanged("AlternativeSelectedProdOrderPartslistPos");
            }
        }

        private List<ProdOrderPartslistPos> _AlternativeProdOrderPartslistPosList;
        [ACPropertyList(613, "AlternativeProdOrderPartslistPos")]
        public List<ProdOrderPartslistPos> AlternativeProdOrderPartslistPosList
        {
            get
            {
                if (_AlternativeProdOrderPartslistPosList == null)
                    _AlternativeProdOrderPartslistPosList = new List<ProdOrderPartslistPos>();
                return _AlternativeProdOrderPartslistPosList;
            }
        }

        #endregion

        #region AlternativeProdOrderPartslistPos -> Methods

        [ACMethodInteraction("AlternativeNewProdOrderPartslistPos", "en{'Alternate Component'}de{'Alternativkomponente'}", (short)MISort.New, true, "SelectedPartslistPos", Global.ACKinds.MSMethodPrePost)]
        public void AlternativeNewProdOrderPartslistPos()
        {
            if (!PreExecute("AlternativeNewProdOrderPartslistpos")) return;
            ProdOrderPartslistPos alternativePartslistpos = ProdOrderPartslistPos.NewAlternativeProdOrderPartslistPos(DatabaseApp, SelectedProdOrderPartslist, SelectedProdOrderPartslistPos);
            alternativePartslistpos.Sequence = AlternativeProdOrderPartslistPosList.Count() + 1;
            DatabaseApp.ProdOrderPartslistPos.AddObject(alternativePartslistpos);
            PreselectedAlternativeSelectedProdOrderPartslistPosID = alternativePartslistpos.ProdOrderPartslistPosID;
            SearchAlternative();
            ACState = Const.SMNew;
            PostExecute("AlternativeNewProdOrderPartslistpos");
        }

        public bool IsEnabledAlternativeNewProdOrderPartslistPos()
        {
            return SelectedProdOrderPartslistPos != null;
        }

        [ACMethodInteraction("AlternativeDeleteProdOrderPartslistPos", "en{'Delete Alternative Component'}de{'Alternative Komponente löschen'}", (short)MISort.Delete, true, "CurrentPartslistPos", Global.ACKinds.MSMethodPrePost)]
        public void AlternativeDeleteProdOrderPartslistPos()
        {
            if (!PreExecute("AlternativeDeleteProdOrderPartslistPos")) return;
            if (AlternativeSelectedProdOrderPartslistPos != null)
            {
                Msg msg = null;
                if (AlternativeSelectedProdOrderPartslistPos.EntityState == EntityState.Added)
                {
                    msg = AlternativeSelectedProdOrderPartslistPos.DeleteACObject(Database, true);
                }
                AlternativeProdOrderPartslistPosList.Remove(AlternativeSelectedProdOrderPartslistPos);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }
                else
                {
                    SearchAlternative();
                }
                PostExecute("AlternativeDeleteProdOrderPartslistPos");
            }
        }

        public bool IsEnabledAlternativeDeleteProdOrderPartslistPos()
        {
            return AlternativeSelectedProdOrderPartslistPos != null;
        }

        #endregion

        #region AlternativeProdOrderPartslistPos-> Search

        /// <summary>
        /// Searches this instance.
        /// </summary>
        public void SearchAlternative()
        {
            _AlternativeProdOrderPartslistPosList = new List<ProdOrderPartslistPos>();
            if (SelectedProdOrderPartslistPos != null)
            {

                var baseItems =
                            DatabaseApp
                            .ProdOrderPartslistPos
                            .Include(c => c.Material)
                            .Include(c => c.Material.BaseMDUnit)
                            .Include(c => c.MDUnit)
                            .Where(c => c.ProdOrderPartslistID == SelectedProdOrderPartslist.ProdOrderPartslistID &&
                                c.AlternativeProdOrderPartslistPosID == SelectedProdOrderPartslistPos.ProdOrderPartslistPosID &&
                                c.MaterialPosTypeIndex == (int)(GlobalApp.MaterialPosTypes.OutwardRoot) &&
                                c.ParentProdOrderPartslistPosID == null)
                            .OrderBy(c => c.Sequence)
                            .AutoMergeOption()
                           .ToList()
                            .Where(x => x.EntityState != EntityState.Deleted)
                            .ToList();

                var localItems = DatabaseApp.GetAddedEntities<ProdOrderPartslistPos>()
                .Where(c =>
                    c.ProdOrderPartslistID == SelectedProdOrderPartslist.ProdOrderPartslistID &&
                    c.AlternativeProdOrderPartslistPosID == SelectedProdOrderPartslistPos.ProdOrderPartslistPosID &&
                    c.MaterialPosTypeIndex == (int)(gip.mes.datamodel.GlobalApp.MaterialPosTypes.OutwardRoot) &&
                    c.ParentProdOrderPartslistPosID == null)
                    .OrderBy(c => c.Sequence)
                    .ToList();
                _AlternativeProdOrderPartslistPosList = baseItems.Union(localItems).ToList();
            }
            if (_AlternativeProdOrderPartslistPosList == null)
                AlternativeSelectedProdOrderPartslistPos = null;
            else
            {
                if (PreselectedAlternativeSelectedProdOrderPartslistPosID != null)
                    AlternativeSelectedProdOrderPartslistPos = _AlternativeProdOrderPartslistPosList.FirstOrDefault(c => c.ProdOrderPartslistPosID == PreselectedAlternativeSelectedProdOrderPartslistPosID);
                else
                    AlternativeSelectedProdOrderPartslistPos = _AlternativeProdOrderPartslistPosList.FirstOrDefault();
            }
            OnPropertyChanged("AlternativeProdOrderPartslistPosList");
        }

        #endregion

        #endregion

        #region ProdOrderIntermediate

        #region Intermediate -> Select, (Current,) List

        private ProdOrderPartslistPos _SelectedIntermediate;
        [ACPropertySelected(614, "Intermediate")]
        public ProdOrderPartslistPos SelectedIntermediate
        {
            get
            {
                return _SelectedIntermediate;
            }
            set
            {
                if (_SelectedIntermediate != value)
                {
                    _SelectedIntermediate = value;
                    SearchProdOrderIntermediateBatch();
                    // Searching components of no batches are there
                    if (SelectedProdOrderIntermediateBatch == null)
                        SearchOutwardPartslistPos();
                    OnPropertyChanged("SelectedIntermediate");
                    OnPropertyChanged("InwardFacilityPreBookingList");
                    OnPropertyChanged("InwardFacilityBookingList");
                }
            }
        }

        private List<ProdOrderPartslistPos> _IntermediateList;
        [ACPropertyList(615, "Intermediate")]
        public List<ProdOrderPartslistPos> IntermediateList
        {
            get
            {
                if (_IntermediateList == null)
                    _IntermediateList = new List<ProdOrderPartslistPos>();
                return _IntermediateList;
            }
        }

        #endregion

        #region Intermediate -> Methods

        [ACMethodInteraction("IntermediateParts", "en{'Recalculate Totals'}de{'Summenberechnung'}", 606, true, "SelectedIntermediate", Global.ACKinds.MSMethodPrePost)]
        public void RecalcIntermediateSum()
        {
            if (!IsEnabledRecalcIntermediateSum()) return;
            ProdOrderPartslistPos lastIntermediateProduct = IntermediateList.Where(c => c.IsFinalMixure).FirstOrDefault();
            Global.MsgResult mr = Root.Messages.YesNoCancel(this, "Question50059", Global.MsgResult.Yes);
            if (mr != Global.MsgResult.Cancel)
            {
                ProdOrderManager.RecalcIntermediateItem(lastIntermediateProduct, mr == Global.MsgResult.Yes);
                ProdOrderManager.RecalcRemainingQuantity(lastIntermediateProduct);
            }

            OnPropertyChanged("IntermediateList");
        }

        public bool IsEnabledRecalcIntermediateSum()
        {
            return IntermediateList != null && IntermediateList.Any();
        }

        #endregion

        #region Intermediate -> Search

        [ACMethodCommand("Intermediate", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void SearchIntermediate()
        {
            _IntermediateList = null;
            if (SelectedProdOrderPartslist != null)
                _IntermediateList = DatabaseApp
                       .ProdOrderPartslistPos
                       .Include(c => c.Material)
                       .Include(c => c.MDUnit)
                       .Where(c => c.ProdOrderPartslistID == SelectedProdOrderPartslist.ProdOrderPartslistID && c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                       .AutoMergeOption()
                       .OrderBy(c => c.Sequence)
                       .ThenBy(c => c.Material != null ? c.Material.MaterialNo : "")
                       .ToList();
            if (_IntermediateList == null)
                SelectedIntermediate = null;
            else
            {
                if (PreselectedInwardPosID != null)
                    SelectedIntermediate = _IntermediateList.FirstOrDefault(c => c.ProdOrderPartslistPosID == PreselectedInwardPosID);
                else
                    SelectedIntermediate = IntermediateList.FirstOrDefault();
            }
            OnPropertyChanged("IntermediateList");
        }

        #endregion

        #endregion

        #region Batch

        #region Batch -> Select, (Current,) List

        private ProdOrderBatch _SelectedBatch;
        [ACPropertySelected(616, "Batch")]
        public ProdOrderBatch SelectedBatch
        {
            get
            {
                return _SelectedBatch;
            }
            set
            {
                if (_SelectedBatch != value)
                {
                    _SelectedBatch = value;
                    OnPropertyChanged("SelectedBatch");
                }
            }
        }


        private List<ProdOrderBatch> _BatchList;
        [ACPropertyList(617, "Batch")]
        public List<ProdOrderBatch> BatchList
        {
            get
            {
                if (_BatchList == null)
                    _BatchList = new List<ProdOrderBatch>();
                return _BatchList;
            }
        }

        #endregion

        #region Batch -> Methods

        [ACMethodInteraction("Batch", "en{'Add'}de{'Neu'}", (short)MISort.New, true, "SelectedBatch", Global.ACKinds.MSMethodPrePost)]
        public void BatchAdd()
        {
            if (!PreExecute("BatchAddDialog")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(ProdOrderBatch), ProdOrderBatch.NoColumnName, ProdOrderBatch.FormatNewNo, this);
            ProdOrderBatch newProdOrderBatch = ProdOrderBatch.NewACObject(DatabaseApp, null, secondaryKey);
            newProdOrderBatch.ProdOrderPartslist = SelectedProdOrderPartslist;
            DatabaseApp.ProdOrderBatch.AddObject(newProdOrderBatch);
            PreselectedBatchID = newProdOrderBatch.ProdOrderBatchID;
            SearchBatch();
            PostExecute("BatchAddDialog");
        }

        [ACMethodInteraction("Batch", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "SelectedBatch", Global.ACKinds.MSMethodPrePost)]
        public void BatchDelete()
        {
            if (!PreExecute("BatchDelete")) return;
            List<ProdOrderPartslistPosRelation> relations =
                DatabaseApp.ProdOrderPartslistPosRelation.Where(x => (x.ProdOrderBatchID ?? Guid.Empty) == SelectedBatch.ProdOrderBatchID).ToList();
            List<ProdOrderPartslistPos> pos =
                DatabaseApp.ProdOrderPartslistPos.Where(x => (x.ProdOrderBatchID ?? Guid.Empty) == SelectedBatch.ProdOrderBatchID).ToList();
            bool existRelations = relations.Any() || pos.Any();
            Global.MsgResult result = Global.MsgResult.No;
            if (existRelations)
            {
                result = Messages.Question(this, "Question50010", Global.MsgResult.Yes, false, SelectedProdOrderPartslist.Partslist.PartslistNo);
                if (result != Global.MsgResult.Yes)
                    return;
                relations.ForEach(x => x.DeleteACObject(DatabaseApp, false));
                pos.ForEach(x => x.DeleteACObject(DatabaseApp, false));
            }
            SelectedBatch.DeleteACObject(DatabaseApp, false);
            SelectedIntermediate = null;
            SearchBatch();
            SearchIntermediate();
            PostExecute("BatchDelete");
        }


        [ACMethodInteraction("Batch", "en{'Delete All'}de{'Löschen alle'}", (short)MISort.Delete, true, "SelectedBatch", Global.ACKinds.MSMethodPrePost)]
        public void BatchDeleteAll()
        {
            if (!PreExecute("BatchDeleteAll")) return;
            List<ProdOrderBatch> batches =
                DatabaseApp
                .ProdOrderBatch
                .Where(x => x.ProdOrderPartslistID == SelectedProdOrderPartslist.ProdOrderPartslistID)
                .ToList();
            List<Guid> batchIds = batches.Select(x => x.ProdOrderBatchID).ToList();

            List<ProdOrderPartslistPosRelation> relations = DatabaseApp
                .ProdOrderPartslistPosRelation
                .Where(x => batchIds.Contains(x.ProdOrderBatchID ?? Guid.Empty)).ToList();

            List<ProdOrderPartslistPos> postions = DatabaseApp
                .ProdOrderPartslistPos
                .Where(x => batchIds.Contains(x.ProdOrderBatchID ?? Guid.Empty)).ToList();

            relations.ForEach(x => x.DeleteACObject(DatabaseApp, false));
            postions.ForEach(x => x.DeleteACObject(DatabaseApp, false));
            batches.ForEach(x => x.DeleteACObject(DatabaseApp, false));

            SelectedIntermediate = null;
            SearchBatch();
            SearchIntermediate();
            PostExecute("BatchDeleteAll");
        }
        #region Batch -> Methods -> IsEnabled
        public bool IsEnabledBatchAdd()
        {
            return SelectedProdOrderPartslist != null;
        }

        public bool IsEnabledBatchDelete()
        {
            return SelectedBatch != null;
        }

        public bool IsEnabledBatchAllDelete()
        {
            return SelectedProdOrderPartslist != null && BatchList != null && BatchList.Any();
        }
        #endregion

        #endregion

        #region Batch -> Search
        /// <summary>
        /// Selected a batch
        /// </summary>
        public void SearchBatch()
        {
            _BatchList = null;
            if (SelectedProdOrderPartslist != null)
            {
                var baseItems =
                    DatabaseApp
                    .ProdOrderBatch
                    .Where(x => x.ProdOrderPartslistID == SelectedProdOrderPartslist.ProdOrderPartslistID)
                    .OrderBy(x => x.BatchSeqNo)
                    .AutoMergeOption()
                    .ToList()
                    .Where(x => x.EntityState != EntityState.Deleted)
                    .ToList();
                var localItems = DatabaseApp.GetAddedEntities<ProdOrderBatch>()
                    .Where(x => x.ProdOrderPartslistID == SelectedProdOrderPartslist.ProdOrderPartslistID)
                    .ToList();
                _BatchList = baseItems.Union(localItems).OrderBy(x => x.ProdOrderBatchNo).ToList();
            }
            if (_BatchList == null)
                SelectedBatch = null;
            else
            {
                if (PreselectedBatchID != null)
                    SelectedBatch = _BatchList.FirstOrDefault(c => c.ProdOrderBatchID == PreselectedBatchID);
                else
                    SelectedBatch = _BatchList.FirstOrDefault();
            }
            OnPropertyChanged("BatchList");
        }
        #endregion

        #endregion

        #region ProdOrderIntermediateBatch

        #region ProdOrderIntermediateBatch -> Select, (Current,) List

        private ProdOrderPartslistPos _SelectedProdOrderIntermediateBatch;

        [ACPropertySelected(618, "ProdOrderIntermediateBatch")]
        public ProdOrderPartslistPos SelectedProdOrderIntermediateBatch
        {
            get
            {
                return _SelectedProdOrderIntermediateBatch;
            }
            set
            {
                if (_SelectedProdOrderIntermediateBatch != value)
                {
                    _SelectedProdOrderIntermediateBatch = value;
                    if (_SelectedProdOrderIntermediateBatch != null
                        && _SelectedProdOrderIntermediateBatch.EntityState != EntityState.Added
                        && _SelectedProdOrderIntermediateBatch.EntityState != EntityState.Detached)
                    {
                        _SelectedProdOrderIntermediateBatch.LabOrder_ProdOrderPartslistPos.AutoLoad(this.DatabaseApp);
                    }
                    SearchOutwardPartslistPos();
                    OnPropertyChanged("SelectedProdOrderIntermediateBatch");
                    OnPropertyChanged("InwardFacilityPreBookingList");
                    OnPropertyChanged("InwardFacilityBookingList");
                    OnPropertyChanged("ProdOrderIntermediateBatchClearSelectionTitle");
                    OnPropertyChanged("ProdOrderIntermediateBatchLotList");
                }
                else if (value == null)
                {
                    SearchOutwardPartslistPos();
                }
            }
        }

        private List<ProdOrderPartslistPos> _ProdOrderIntermediateBatchList;
        [ACPropertyList(619, "ProdOrderIntermediateBatch")]
        public List<ProdOrderPartslistPos> ProdOrderIntermediateBatchList
        {
            get
            {
                if (_ProdOrderIntermediateBatchList == null)
                    _ProdOrderIntermediateBatchList = new List<ProdOrderPartslistPos>();
                return _ProdOrderIntermediateBatchList;
            }
        }

        /// <summary>
        /// Change Title from button in two states
        /// 1. state when batch is selected and intention is to unselect him
        /// 2. state when is batch allearedy unselected and there is no oportunity to another batch form preselected one
        /// </summary>
        [ACPropertyInfo(620, "ProdOrderIntermediateBatch")]
        public string ProdOrderIntermediateBatchClearSelectionTitle
        {
            get
            {
                string unassignBatch = Root.Environment.TranslateText(this, "lblUnassignBatch"); ;
                string selectFirstBatch = Root.Environment.TranslateText(this, "lblSelectFirstBatch"); ;
                return SelectedIntermediate != null && SelectedProdOrderIntermediateBatch == null ? selectFirstBatch : unassignBatch;
            }
        }
        #endregion

        #region ProdOrderIntermediateBatch -> Methods

        [ACMethodInteraction("ProdOrderIntermediateBatch", "en{'New Batch'}de{'Neuer Batch'}", (short)MISort.New, true, "SelectedProdOrderIntermediateBatch", Global.ACKinds.MSMethodPrePost)]
        public void ProdOrderIntermediateBatchCreateDlg()
        {
            if (!PreExecute("ProdOrderIntermediateBatchCreateDlg")) return;
            ShowDialog(this, "PosBatchCreateDlg");
            PostExecute("ProdOrderIntermediateBatchCreateDlg");
        }

        [ACMethodInteraction("ProdOrderIntermediateBatch", "en{'Assign Batch'}de{'Batch Zuordnen'}", (short)MISort.New, true, "SelectedProdOrderIntermediateBatch", Global.ACKinds.MSMethodPrePost)]
        public void ProdOrderIntermediateBatchAssign()
        {
            if (!PreExecute("ProdOrderIntermediateBatchAssign")) return;
            // TODO: @aagincic - ProdOrderIntermediateBatchAssign - transform this creation in some kind of background operation
            List<ProdOrderPartslistPosRelation> relations =
                DatabaseApp.ProdOrderPartslistPosRelation.Where(x => x.TargetProdOrderPartslistPosID == SelectedIntermediate.ProdOrderPartslistPosID).ToList();
            ProdOrderPartslistPos batchItem = ProdOrderPartslistPos.NewACObject(DatabaseApp, SelectedIntermediate);
            batchItem.ProdOrderBatch = SelectedBatch;
            List<ProdOrderPartslistPosRelation> batchRelations = new List<ProdOrderPartslistPosRelation>();
            foreach (var item in relations)
            {
                ProdOrderPartslistPosRelation batchRelation = ProdOrderPartslistPosRelation.NewACObject(DatabaseApp, item);
                batchRelation.Sequence = item.Sequence;
                batchRelation.TargetProdOrderPartslistPos = batchItem;
                batchRelation.SourceProdOrderPartslistPos = item.SourceProdOrderPartslistPos;
                batchRelation.ProdOrderBatch = SelectedBatch;
                batchRelations.Add(batchRelation);
            }
            DatabaseApp.ProdOrderPartslistPos.AddObject(batchItem);
            batchRelations.ForEach(x => DatabaseApp.ProdOrderPartslistPosRelation.AddObject(x));
            PreselectedInwardBatchPosID = batchItem?.ProdOrderPartslistPosID;
            SearchProdOrderIntermediateBatch();
            PostExecute("ProdOrderIntermediateBatchAssign");
        }

        [ACMethodInteraction("ProdOrderIntermediateBatch", "en{'Unassign'}de{'Zuordnung entfernen'}", (short)MISort.Delete, true, "SelectedProdOrderIntermediateBatch", Global.ACKinds.MSMethodPrePost)]
        public void ProdOrderIntermediateBatchUnAssign()
        {
            if (!PreExecute("ProdOrderIntermediateBatchUnAssign")) return;
            List<ProdOrderPartslistPosRelation> relatedComponents = DatabaseApp.ProdOrderPartslistPosRelation
                .Where(x => x.ProdOrderBatchID == SelectedProdOrderIntermediateBatch.ProdOrderBatchID).ToList();
            relatedComponents.ForEach(x => x.DeleteACObject(DatabaseApp, false));
            SelectedProdOrderIntermediateBatch.DeleteACObject(DatabaseApp, false);

            var selectedIntermediate = SelectedIntermediate;
            SelectedIntermediate = null;
            PreselectedInwardPosID = selectedIntermediate?.ProdOrderPartslistPosID;
            SearchIntermediate();
            PostExecute("ProdOrderIntermediateBatchUnAssign");
        }

        [ACMethodInteraction("ProdOrderIntermediateBatch", "en{'Unselect'}de{'Auswahl aufheben'}", (short)MISort.Delete, true, "SelectedProdOrderIntermediateBatch", Global.ACKinds.MSMethodPrePost)]
        public void ProdOrderIntermediateBatchClearSelection()
        {
            if (!PreExecute("ProdOrderIntermediateBatchClearSelection")) return;
            if (SelectedProdOrderIntermediateBatch == null)
            {
                SelectedProdOrderIntermediateBatch = ProdOrderIntermediateBatchList.FirstOrDefault();
            }
            else
            {
                SelectedProdOrderIntermediateBatch = null;
            }
            OnPropertyChanged("ProdOrderIntermediateBatchClearSelectionTitle");
            PostExecute("ProdOrderIntermediateBatchClearSelection");
        }

        [ACMethodInteraction("ProdOrderIntermediateBatch", "en{'New overall lot'}de{'Gesamtlos erzeugen'}", (short)MISort.New, true, "SelectedProdOrderIntermediateBatch", Global.ACKinds.MSMethodPrePost)]
        public void GenerateLotNumber()
        {
            if (!IsEnabledGenerateLotNumber()) return;
            ACComponent childBSO = ACUrlCommand("?" + ConstApp.BSOFacilityLot_ChildName) as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent(ConstApp.BSOFacilityLot_ChildName, null, new object[] { }) as ACComponent;
            if (childBSO == null) return;
            ProdOrderPartslistPos item = SelectedIntermediate;
            if (SelectedProdOrderIntermediateBatch != null)
                item = SelectedProdOrderIntermediateBatch;
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!" + ConstApp.BSOFacilityLot_Dialog_ShowDialogNewLot, "", item.Material);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                item = SelectedIntermediate;
                if (SelectedProdOrderIntermediateBatch != null)
                    item = SelectedProdOrderIntermediateBatch;

                FacilityLot result = dlgResult.ReturnValue as FacilityLot;
                item.FacilityLot = result;
                // Clear lot in parent / children sequence from new lot defined membmerd
                if (SelectedProdOrderIntermediateBatch != null && SelectedIntermediate.FacilityLot != null)
                {
                    SelectedProdOrderIntermediateBatch.ProdOrderPartslistPos1_ParentProdOrderPartslistPos.FacilityLot = null;
                }
                else if (SelectedProdOrderIntermediateBatch == null)
                {
                    SelectedIntermediate.ProdOrderPartslistPos_ParentProdOrderPartslistPos.ToList().ForEach(x => x.FacilityLot = null);
                }
                OnPropertyChanged("IntermediateList");
                OnPropertyChanged("ProdOrderIntermediateBatchList");
                Save();
            }
            if (childBSO != null)
                childBSO.Stop();
        }

        [ACMethodInteraction("ProdOrderIntermediateBatchLot", "en{'New partial lot'}de{'Teillose erzeugen'}", (short)MISort.New, true, "SelectedProdOrderIntermediateBatch", Global.ACKinds.MSMethodPrePost)]
        public void GeneratePartLotNumber()
        {
            if (!IsEnabledGeneratePartLotNumber()) return;
            ACComponent childBSO = ACUrlCommand("?" + ConstApp.BSOFacilityLot_ChildName) as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent(ConstApp.BSOFacilityLot_ChildName, null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!" + ConstApp.BSOFacilityLot_Dialog_ShowDialogNewLot, "", SelectedProdOrderIntermediateBatch.Material);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                FacilityLot facilityLot = dlgResult.ReturnValue as FacilityLot;
                ProdOrderPartslistPosFacilityLot item = ProdOrderPartslistPosFacilityLot.NewACObject(DatabaseApp, SelectedProdOrderIntermediateBatch);
                item.FacilityLot = facilityLot;
                SelectedProdOrderIntermediateBatch.ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos.Add(item);
                SelectedProdOrderIntermediateBatchLot = item;
                OnPropertyChanged("ProdOrderIntermediateBatchLotList");
                Save();
            }
            if (childBSO != null)
                childBSO.Stop();
        }

        [ACMethodInteraction("ProdOrderIntermediateBatchLot", "en{'Delete partial lot'}de{'Teillose löschen'}", (short)MISort.New, true, "SelectedProdOrderIntermediateBatch", Global.ACKinds.MSMethodPrePost)]
        public void RemovePartLotNumber()
        {
            if (!IsEnabledRemovePartLotNumber()) return;
            SelectedProdOrderIntermediateBatch.ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos.Remove(SelectedProdOrderIntermediateBatchLot);
            SelectedProdOrderIntermediateBatchLot.DeleteACObject(DatabaseApp, false);
            SelectedProdOrderIntermediateBatchLot = ProdOrderIntermediateBatchLotList.FirstOrDefault();
            OnPropertyChanged("ProdOrderIntermediateBatchLotList");
        }

        [ACMethodInteraction("ProdOrderIntermediateBatch", "en{'Sum bookings to Actual Quantity'}de{'Summiere Buchungen auf Istmenge'}", (short)MISort.New, true, "SelectedProdOrderIntermediateBatch")]
        public void RecalcProdOrderIntermediateBatch()
        {
            SelectedProdOrderIntermediateBatch.RecalcActualQuantity();
        }

        public bool IsEnabledProdOrderIntermediateBatch()
        {
            return SelectedProdOrderIntermediateBatch != null;
        }

        #region ProdOrderIntermediateBatch -> Methods -> IsEnabled

        public bool IsEnabledProdOrderIntermediateBatchCreateDlg()
        {
            // Create new set of batches: not have any batch jet!
            return SelectedIntermediate != null && SelectedIntermediate.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern;
            //&& (ProdOrderIntermediateBatchList == null || !ProdOrderIntermediateBatchList.Any());
        }

        public bool IsEnabledProdOrderIntermediateBatchAssign()
        {
            return SelectedBatch != null && SelectedIntermediate != null;
        }

        public bool IsEnabledProdOrderIntermediateBatchUnAssign()
        {
            return SelectedProdOrderIntermediateBatch != null;
        }

        public bool IsEnabledProdOrderIntermediateBatchClearSelection()
        {
            return SelectedIntermediate != null;
        }

        public bool IsEnabledGenerateLotNumber()
        {
            Material material = null;
            if (SelectedProdOrderIntermediateBatch != null)
                material = SelectedProdOrderIntermediateBatch.BookingMaterial;
            else if (SelectedIntermediate != null)
                material = SelectedIntermediate.BookingMaterial;
            if (material == null)
                return false;
            return material.IsLotManaged &&
                ((SelectedProdOrderIntermediateBatch != null && SelectedProdOrderIntermediateBatch.FacilityLot == null) ||
                (SelectedProdOrderIntermediateBatch == null && SelectedIntermediate != null && SelectedIntermediate.FacilityLot == null));
        }

        public bool IsEnabledGeneratePartLotNumber()
        {
            return SelectedProdOrderIntermediateBatch != null;
        }

        public bool IsEnabledRemovePartLotNumber()
        {
            return SelectedProdOrderIntermediateBatchLot != null && SelectedProdOrderIntermediateBatchLot != null;
        }
        #endregion

        #endregion

        #region ProdOrderIntermediateBatch -> Search

        public void SearchProdOrderIntermediateBatch()
        {
            _ProdOrderIntermediateBatchList = null;
            if (SelectedIntermediate != null)
            {
                var baseItems = DatabaseApp
                    .ProdOrderPartslistPos
                    .Where(x => x.ParentProdOrderPartslistPosID == SelectedIntermediate.ProdOrderPartslistPosID)
                    .OrderBy(x => x.Sequence)
                    .AutoMergeOption()
                    .ToList()
                    .Where(x => x.EntityState != EntityState.Deleted)
                    .ToList();

                var localItems = DatabaseApp.GetAddedEntities<ProdOrderPartslistPos>()
                    .Where(x => x.ParentProdOrderPartslistPosID == SelectedIntermediate.ProdOrderPartslistPosID)
                    .OrderBy(x => x.Sequence)
                    .ToList();

                _ProdOrderIntermediateBatchList = baseItems.Union(localItems).ToList();
            }
            if (_ProdOrderIntermediateBatchList == null)
                SelectedProdOrderIntermediateBatch = null;
            else
            {
                if (PreselectedInwardBatchPosID != null)
                    SelectedProdOrderIntermediateBatch = _ProdOrderIntermediateBatchList.FirstOrDefault(c => c.ProdOrderPartslistPosID == PreselectedInwardBatchPosID);
                else
                    SelectedProdOrderIntermediateBatch = _ProdOrderIntermediateBatchList.FirstOrDefault();
            }
            OnPropertyChanged("ProdOrderIntermediateBatchList");
        }
        #endregion

        #endregion

        #region ProdOrderIntermediateBatchLots

        #region ProdOrderIntermediateBatchLot

        private ProdOrderPartslistPosFacilityLot _SelectedProdOrderIntermediateBatchLot;
        /// <summary>
        /// Selected property for ProdOrderPartslistPosFacilityLot
        /// </summary>
        /// <value>The selected ProdOrderIntermediateBatchLots</value>
        [ACPropertySelected(621, "ProdOrderIntermediateBatchLot", "en{'TODO: ProdOrderIntermediateBatchLots'}de{'TODO: ProdOrderIntermediateBatchLots'}")]
        public ProdOrderPartslistPosFacilityLot SelectedProdOrderIntermediateBatchLot
        {
            get
            {
                return _SelectedProdOrderIntermediateBatchLot;
            }
            set
            {
                if (_SelectedProdOrderIntermediateBatchLot != value)
                {
                    _SelectedProdOrderIntermediateBatchLot = value;
                    OnPropertyChanged("SelectedProdOrderIntermediateBatchLot");
                }
            }
        }

        /// <summary>
        /// List property for ProdOrderPartslistPosFacilityLot
        /// </summary>
        /// <value>The ProdOrderIntermediateBatchLots list</value>
        [ACPropertyList(622, "ProdOrderIntermediateBatchLot")]
        public IEnumerable<ProdOrderPartslistPosFacilityLot> ProdOrderIntermediateBatchLotList
        {
            get
            {
                if (SelectedProdOrderIntermediateBatch == null) return null;
                return SelectedProdOrderIntermediateBatch
               .ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos
               .OrderBy(c => c.IsActive)
               .ThenBy(c => c.FacilityLot.LotNo);
            }
        }

        #endregion

        #endregion

        #region BatchCreate

        #region BatchCreate -> Automatically (BatchCreateAutomatically)

        #region BatchCreate -> Automatically -> Input Properties

        private bool _BatchCreateAutomaticallyViaBatchSize;
        [ACPropertyInfo(623, "BatchCreateAutomaticallyViaBatchSize", "en{'Create batch via batch size'}de{'Autom. Batchgenerierung (Gesamtmenge/Batchgröße)'}")]
        public bool BatchCreateAutomaticallyViaBatchSize
        {
            get
            {
                return _BatchCreateAutomaticallyViaBatchSize;
            }
            set
            {
                if (_BatchCreateAutomaticallyViaBatchSize != value)
                {
                    _BatchCreateAutomaticallyViaBatchSize = value;
                    OnPropertyChanged("BatchCreateAutomaticallyViaBatchSize");
                }
            }
        }

        private double? _BatchCreateAutomaticallyBatchSize;
        [ACPropertyInfo(624, "BatchCreateAutomaticallyBatchSize", "en{'Batch size'}de{'Batchgröße [ME]'}")]
        public double? BatchCreateAutomaticallyBatchSize
        {
            get
            {
                return _BatchCreateAutomaticallyBatchSize;
            }
            set
            {
                if (_BatchCreateAutomaticallyBatchSize != value)
                {
                    _BatchCreateAutomaticallyBatchSize = value;
                    OnPropertyChanged("BatchCreateAutomaticallyBatchSize");
                }
            }
        }

        private int? _BatchCreateAutomaticallyBatchCount;
        [ACPropertyInfo(625, "BatchCreateAutomaticallyBatchCount", "en{'Batch count'}de{'Anzahl Batche'}")]
        public int? BatchCreateAutomaticallyBatchCount
        {
            get
            {
                return _BatchCreateAutomaticallyBatchCount;
            }
            set
            {
                if (_BatchCreateAutomaticallyBatchCount != value)
                {
                    _BatchCreateAutomaticallyBatchCount = value;
                    OnPropertyChanged("BatchCreateAutomaticallyBatchCount");
                }
            }
        }

        #endregion

        #region  BatchCreate -> Automatically -> CreateBatchAutomaticallyCalc
        private List<BatchQuantityModel> _CreateBatchAutomaticallyCalcList;
        [ACPropertyList(626, "CreateBatchAutomaticallyCalc", "en{'CreateBatchCalc'}de{'CreateBatchCalc'}")]
        public List<BatchQuantityModel> CreateBatchAutomaticallyCalcList
        {
            get
            {
                return _CreateBatchAutomaticallyCalcList;
            }
            set
            {
                _CreateBatchAutomaticallyCalcList = value;
                OnPropertyChanged("CreateBatchAutomaticallyCalcList");
            }
        }

        private BatchQuantityModel _SelectedCreateBatchAutomaticallyCalc;
        [ACPropertySelected(627, "CreateBatchAutomaticallyCalc", "en{'CreateBatchCalc'}de{'CreateBatchCalc'}")]
        public BatchQuantityModel SelectedCreateBatchAutomaticallyCalc
        {
            get
            {
                return _SelectedCreateBatchAutomaticallyCalc;
            }
            set
            {
                if (_SelectedCreateBatchAutomaticallyCalc != value)
                {
                    _SelectedCreateBatchAutomaticallyCalc = value;
                    OnPropertyChanged("SelectedCreateBatchAutomaticallyCalc");
                }
            }
        }
        #endregion

        #region BatchCreate -> Automatically -> Methods

        [ACMethodInfo("BatchCreateAutomaticallyCalculate", "en{'Calculate'}de{'Berechnen'}", 607)]
        public void BatchCreateAutomaticallyCalculate()
        {
            if (!IsEnabledBatchCreateAutomaticallyCalculate()) return;
            CreateBatchAutomaticallyCalcList = CreateBatchAutomaticallyGetDefinitionList();
        }

        [ACMethodInfo("BatchCreateAutomaticallyCreateDlgOk", "en{'Ok'}de{'Ok'}", (short)MISort.Okay)]
        public void BatchCreateAutomaticallyCreateDlgOk()
        {
            if (!IsEnabledBatchCreateAutomaticallyCreateDlgOk()) return;
            var batches = CreateBatchAutomaticallyGetDefinitionList();
            if (batches.Any())
            {
                BatchCreateGenerateBatch(batches, BatchCreateDepthList.Where(x => x.Selected).OrderBy(x => x.Depth).ToList(), RestHandleModeEnum.ToFirstBatch);
            }
        }

        public bool IsEnabledBatchCreateAutomaticallyCalculate()
        {
            return SelectedIntermediate != null && BatchCreateAutomaticallyViaBatchSize ? BatchCreateAutomaticallyBatchSize.HasValue : BatchCreateAutomaticallyBatchCount.HasValue;
        }

        public bool IsEnabledBatchCreateAutomaticallyCreateDlgOk()
        {
            if (!BatchCreateDepthList.Any()) return false;
            if (BatchCreateAutomaticallyViaBatchSize)
            {
                return BatchCreateAutomaticallyBatchSize.HasValue && BatchCreateAutomaticallyBatchSize.Value > 0;
            }
            else
            {
                return BatchCreateAutomaticallyBatchCount.HasValue && BatchCreateAutomaticallyBatchCount.Value > 0;
            }
        }

        #endregion

        #region  BatchCreate -> Automatically -> Calculation / Decision methods

        /// <summary>
        /// Define batch list for automatic by batch size
        /// </summary>
        /// <param name="batchSize"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private List<BatchQuantityModel> BatchCreateAutomaticallyDefineBatchByBatchSize(double batchSize, double quantity)
        {
            List<BatchQuantityModel> batches = new List<BatchQuantityModel>();
            int clcBatchNr = 0;
            if (batchSize > quantity || quantity == 0)
                return null;
            clcBatchNr = (int)(quantity / batchSize);
            double restQuantity = quantity % batchSize;
            for (int i = 0; i < clcBatchNr; i++)
                batches.Add(new BatchQuantityModel() { Sequence = i + 1, TargetQuantity = batchSize });
            // create last addional batch with rest of quantity
            if (restQuantity > 0)
                batches.Add(new BatchQuantityModel() { Sequence = clcBatchNr + 1, TargetQuantity = restQuantity });
            return batches;
        }

        /// <summary>
        /// Deifine batch list for automatic by batch count
        /// quantity is rounded on int value - rest is added to first batch
        /// </summary>
        /// <param name="batchCount"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        private List<BatchQuantityModel> BatchCreateAutomaticallyDefineBatchByBatchCount(int batchCount, double quantity)
        {
            List<BatchQuantityModel> batches = new List<BatchQuantityModel>();
            double clcBatchSize = 0;
            clcBatchSize = (int)(quantity / batchCount);

            double restOfQuantity = quantity - (clcBatchSize * batchCount);

            for (int i = 0; i < batchCount; i++)
            {
                if (i == 0 && restOfQuantity > 0)
                {
                    batches.Add(new BatchQuantityModel() { Sequence = i + 1, TargetQuantity = clcBatchSize + restOfQuantity });
                }
                else
                {
                    batches.Add(new BatchQuantityModel() { Sequence = i + 1, TargetQuantity = clcBatchSize });
                }
            }
            return batches;
        }

        /// <summary>
        /// Finall GUI decision by calculation batch size in automatic
        /// </summary>
        /// <returns></returns>
        private List<BatchQuantityModel> CreateBatchAutomaticallyGetDefinitionList()
        {
            List<BatchQuantityModel> list = null;
            if (BatchCreateAutomaticallyViaBatchSize)
            {
                list = BatchCreateAutomaticallyDefineBatchByBatchSize(BatchCreateAutomaticallyBatchSize ?? 0, SelectedIntermediate.TargetQuantityUOM);
            }
            else
            {
                list = BatchCreateAutomaticallyDefineBatchByBatchCount(BatchCreateAutomaticallyBatchCount ?? 0, SelectedIntermediate.TargetQuantityUOM);
            }
            return list;
        }

        #endregion

        #endregion

        #region BatchCreate -> Manually

        #region BatchCreate -> Manually -> Properties

        private int? _BatchCreateManuallyBatchCount;
        [ACPropertyInfo(628, "BatchCreateManuallyBatchCount", "en{'Batch count'}de{'Anzahl Batche'}")]
        public int? BatchCreateManuallyBatchCount
        {
            get
            {
                return _BatchCreateManuallyBatchCount;
            }
            set
            {
                if (_BatchCreateManuallyBatchCount != value)
                {
                    _BatchCreateManuallyBatchCount = value;
                    OnPropertyChanged("BatchCreateManuallyBatchCount");
                }
            }
        }

        private double? _BatchCreateManuallyBatchSize;
        [ACPropertyInfo(629, "BatchCreateManuallyBatchSize", "en{'Batch size'}de{'Batchgröße [ME]'}")]
        public double? BatchCreateManuallyBatchSize
        {
            get
            {
                return _BatchCreateManuallyBatchSize;
            }
            set
            {
                if (_BatchCreateManuallyBatchSize != value)
                {
                    _BatchCreateManuallyBatchSize = value;
                    OnPropertyChanged("BatchCreateManuallyBatchSize");
                }
            }
        }

        #endregion

        #region BatchCreate -> Manually -> Methods

        [ACMethodCommand("BatchCreateManuallyCmd", "en{'Ok'}de{'Ok'}", (short)MISort.Okay)]
        public void BatchCreateManuallyCmd()
        {
            if (!IsEnabledBatchCreateManuallyCmd()) return;
            List<BatchQuantityModel> batches = new List<BatchQuantityModel>();
            for (int i = 1; i <= BatchCreateManuallyBatchCount.Value; i++)
            {
                batches.Add(new BatchQuantityModel() { Sequence = i, TargetQuantity = BatchCreateManuallyBatchSize.Value });
            }
            BatchCreateGenerateBatch(batches, BatchCreateDepthList.Where(x => x.Selected).OrderBy(x => x.Depth).ToList(), RestHandleModeEnum.DoNothing);
        }

        private bool IsEnabledBatchCreateManuallyCmd()
        {
            return (BatchCreateManuallyBatchCount ?? 0) > 0 && (BatchCreateManuallyBatchSize ?? 0) > 0 && BatchCreateDepthList.Any();
        }

        #endregion

        #endregion

        #region BatchCreate -> Depth (BatchCreateDepth)

        private PosIntermediateDepthWrap _SelectedBatchCreateDepth;
        /// <summary>
        /// Selected property for PosIntermediateDepthWrap
        /// </summary>
        /// <value>The selected BatchCreateDepth</value>
        [ACPropertySelected(630, "BatchCreateDepth", "en{'TODO: BatchCreateDepth'}de{'TODO: BatchCreateDepth'}")]
        public PosIntermediateDepthWrap SelectedBatchCreateDepth
        {
            get
            {
                return _SelectedBatchCreateDepth;
            }
            set
            {
                if (_SelectedBatchCreateDepth != value)
                {
                    _SelectedBatchCreateDepth = value;
                    OnPropertyChanged("SelectedBatchCreateDepth");
                }
            }
        }


        private List<PosIntermediateDepthWrap> _BatchCreateDepthList;
        /// <summary>
        /// List property for PosIntermediateDepthWrap
        /// </summary>
        /// <value>The BatchCreateDepth list</value>
        [ACPropertyList(631, "BatchCreateDepth")]
        public List<PosIntermediateDepthWrap> BatchCreateDepthList
        {
            get
            {
                if (_BatchCreateDepthList == null && SelectedIntermediate != null)
                {
                    _BatchCreateDepthList = ProdOrderManager.BatchCreateBuildIntermediateIncludedList(SelectedIntermediate, CurrentProcessWorkflow);
                }
                return _BatchCreateDepthList;
            }
        }

        #endregion

        #region BatchCreate -> Common
        /// <summary>
        /// Define a decimal precision by rounding batch size
        /// </summary>
        private int BatchCreateBatchSizeDecimalPrecision = 2;

        private void BatchCreateCleanUpBatch()
        {
            CleanUpBatchAutomatic();
            CleanUpBatchManually();
            CleanUpDepth();
            CloseTopDialog();
        }

        private void CleanUpDepth()
        {
            _BatchCreateDepthList = null;
        }

        private void CleanUpBatchManually()
        {
            BatchCreateManuallyBatchSize = 0;
            BatchCreateManuallyBatchCount = 0;
        }

        private void CleanUpBatchAutomatic()
        {
            BatchCreateAutomaticallyBatchCount = null;
            BatchCreateAutomaticallyBatchSize = null;
            CreateBatchAutomaticallyCalcList = null;
        }

        /// <summary>Generate batch set for every intermediate</summary>
        /// <param name="batchQuantityDefinition"></param>
        /// <param name="intermediateList"></param>
        /// <param name="batchHandleModel"></param>
        private void BatchCreateGenerateBatch(List<BatchQuantityModel> batchQuantityDefinition, List<PosIntermediateDepthWrap> intermediateList, RestHandleModeEnum batchHandleModel)
        {
            if (intermediateList == null || !intermediateList.Any() || batchQuantityDefinition == null || !batchQuantityDefinition.Any()) return;

            List<BatchPercentageModel> batchPercentageModel = BatchSizeCalculation.GetPercentageModel(intermediateList.First().TargetQuantityUOM, batchQuantityDefinition);
            Msg msg = ProdOrderManager.BatchCreateCascade(DatabaseApp, batchPercentageModel, intermediateList, batchHandleModel, BatchCreateBatchSizeDecimalPrecision);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            // Finally refresh a current batch
            SearchBatch();
            SearchProdOrderIntermediateBatch();
            BatchCreateDlgCancel();
        }

        [ACMethodInteraction("ProdOrderIntermediateBatch", "en{'Cancel'}de{'Löschen'}", (short)MISort.Cancel, true, "SelectedProdOrderIntermediateBatch", Global.ACKinds.MSMethodPrePost)]
        public void BatchCreateDlgCancel()
        {
            BatchCreateCleanUpBatch();
        }

        #endregion

        #endregion

        #region Others

        #region Adding Partslist Properties

        private int? _AddPartslistSequence;
        [ACPropertyInfo(633, "AddPartslist", "en{'Sequence'}de{'Folge'}")]
        public int? AddPartslistSequence
        {
            get
            {
                return _AddPartslistSequence;
            }
            set
            {
                if (_AddPartslistSequence != value)
                {
                    _AddPartslistSequence = value;
                    OnPropertyChanged("AddPartslistSequence");
                }
            }
        }

        private double? _AddPartslistTargetQuantity;
        [ACPropertyInfo(634, "AddPartslist", ConstApp.TargetQuantity)]
        public double? AddPartslistTargetQuantity
        {
            get
            {
                return _AddPartslistTargetQuantity;
            }
            set
            {
                if (_AddPartslistTargetQuantity != value)
                {
                    _AddPartslistTargetQuantity = value;
                    OnPropertyChanged("AddPartslistTargetQuantity");
                }
            }
        }

        #endregion

        #region Report helper

        private List<gip.core.datamodel.MsgAlarmLog> _ReportAlarmList;
        [ACPropertyInfo(9999)]
        public IEnumerable<gip.core.datamodel.MsgAlarmLog> ReportAlarmList
        {
            get
            {
                if (_ReportAlarmList == null)
                    _ReportAlarmList = new List<core.datamodel.MsgAlarmLog>();
                _ReportAlarmList.Clear();

                if (ReportACProgramLogList == null)
                    return null;

                foreach (var log in ReportACProgramLogList)
                {
                    if (log.MsgAlarmLog_ACProgramLog.Any())
                        foreach (var alarm in log.MsgAlarmLog_ACProgramLog)
                            _ReportAlarmList.Add(alarm);

                    foreach (var childlog in log.ACProgramLog_ParentACProgramLog)
                        if (childlog.MsgAlarmLog_ACProgramLog.Any())
                            foreach (var alarm in childlog.MsgAlarmLog_ACProgramLog)
                                _ReportAlarmList.Add(alarm);
                }
                return _ReportAlarmList;
            }
        }

        [ACPropertyInfo(9999)]
        public core.datamodel.ACProgramLog ReportACProgramLog
        {
            get
            {
                if (SelectedProdOrderIntermediateBatch == null || SelectedProdOrderIntermediateBatch.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos == null)
                    return null;
                return SelectedProdOrderIntermediateBatch.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Where(c => c.OrderLog_ProdOrderPartslistPosRelation.Any())
                        .SelectMany(x => x.OrderLog_ProdOrderPartslistPosRelation).Where(k => k.ACProgramLog != null).Select(t => t.ACProgramLog).FirstOrDefault();
            }
        }

        //private List<core.datamodel.ACProgramLog> _ReportACProgramLogList;

        //public void FillList()
        //{
        //    if (ReportACProgramLog == null)
        //        return;
        //    if (_ReportACProgramLogList == null)
        //        _ReportACProgramLogList = new List<core.datamodel.ACProgramLog>();
        //    _ReportACProgramLogList.Clear();

        //    core.datamodel.ACProgramLog rootLog = ReportACProgramLog;

        //    while (rootLog.ACProgramLog1_ParentACProgramLog != null)
        //        rootLog = rootLog.ACProgramLog1_ParentACProgramLog;
        //    FillLog(rootLog);

        //}

        //private void FillLog(core.datamodel.ACProgramLog childLog)
        //{
        //    foreach(var item in childLog.ACProgramLog_ParentACProgramLog)
        //    {
        //        if (!item.ACProgramLog_ParentACProgramLog.Any())
        //            _ReportACProgramLogList.Add(item);
        //        FillLog(item);
        //    }
        //}

        [ACPropertyInfo(9999)]
        public IEnumerable<core.datamodel.ACProgramLog> ReportACProgramLogList
        {
            get
            {
                if (SelectedBatch == null)
                    return null;
                return SelectedBatch.ProdOrderPartslistPosRelation_ProdOrderBatch.SelectMany(c => c.OrderLog_ProdOrderPartslistPosRelation).Select(x => x.ACProgramLog);
            }
        }

        [ACPropertyInfo(9999)]
        public ProdOrderPartslistPos ReportLastIntermediate
        {
            get
            {
                if (SelectedBatch != null)
                    return SelectedBatch.ProdOrderPartslistPos_ProdOrderBatch.FirstOrDefault(c => c.IsFinalMixureBatch);
                return null;
            }
        }

        public List<FacilityBookingCharge> _ReportTargetBookings;
        [ACPropertyInfo(9999)]
        public IEnumerable<FacilityBookingCharge> ReportTargetBookings
        {
            get
            {
                if (ReportLastIntermediate == null)
                    return null;

                if (_ReportTargetBookings == null)
                    _ReportTargetBookings = new List<FacilityBookingCharge>();
                _ReportTargetBookings.Clear();
                foreach (var charge in ReportLastIntermediate.FacilityBooking_ProdOrderPartslistPos.ToList().SelectMany(x => x.FacilityBookingCharge_FacilityBooking))
                    _ReportTargetBookings.Add(charge);

                return _ReportTargetBookings;
            }
        }

        [ACPropertyInfo(9999)]
        public List<ReportConfigurationWrapper> ReportConfigs
        {
            get
            {
                if (SelectedProdOrderPartslist != null)
                    return CreateReportConfigWrappers(SelectedProdOrderPartslist.ConfigurationEntries);
                return null;
            }
        }

        [ACPropertyInfo(9999)]
        public List<ReportConfigurationWrapper> ReportConfigsPartslist
        {
            get
            {
                if (SelectedProdOrderPartslist != null)
                    return CreateReportConfigWrappers(SelectedProdOrderPartslist.Partslist.ConfigurationEntries);
                return null;
            }
        }

        [ACPropertyInfo(9999)]
        public List<ReportConfigurationWrapper> ReportConfigsMaterialWF
        {
            get
            {
                if (SelectedProdOrderPartslist != null)
                    return CreateReportConfigWrappers(SelectedProdOrderPartslist.Partslist.MaterialWF.MaterialWFACClassMethod_MaterialWF.FirstOrDefault(c => c.ACClassMethod == CurrentProcessWorkflow.ACClassMethod).ConfigurationEntries);
                return null;
            }
        }

        [ACPropertyInfo(9999)]
        public List<ReportConfigurationWrapper> ReportConfigsWF
        {
            get
            {
                if (SelectedProdOrderPartslist != null)
                    return CreateReportConfigWrappers(SelectedProdOrderPartslist.Partslist.MaterialWF.MaterialWFACClassMethod_MaterialWF.FirstOrDefault(c => c.ACClassMethod == CurrentProcessWorkflow.ACClassMethod).ACClassMethod.FromIPlusContext<core.datamodel.ACClassMethod>().ConfigurationEntries);
                return null;
            }
        }

        public List<ReportConfigurationWrapper> CreateReportConfigWrappers(IEnumerable<IACConfig> configItemsSource)
        {
            List<ReportConfigurationWrapper> wrapperList = new List<ReportConfigurationWrapper>();
            foreach (var config in configItemsSource.OrderBy(c => c.PreConfigACUrl).ThenBy(x => x.LocalConfigACUrl))
            {
                core.datamodel.ACClassWF acClassWF = null;
                if (config is ProdOrderPartslistConfig)
                    acClassWF = ((ProdOrderPartslistConfig)config).VBiACClassWF.FromIPlusContext<core.datamodel.ACClassWF>();
                else if (config is PartslistConfig)
                    acClassWF = ((PartslistConfig)config).VBiACClassWF.FromIPlusContext<core.datamodel.ACClassWF>();
                else if (config is MaterialWFACClassMethodConfig)
                    acClassWF = ((MaterialWFACClassMethodConfig)config).VBiACClassWF.FromIPlusContext<core.datamodel.ACClassWF>();
                else if (config is core.datamodel.ACClassMethodConfig)
                    acClassWF = ((core.datamodel.ACClassMethodConfig)config).ACClassWF;

                ReportConfigurationWrapper wrapper = wrapperList.FirstOrDefault(c => c.ConfigACClassWF == acClassWF);
                if (wrapper != null && wrapper.ConfigACClassWF != null)
                    wrapper.ConfigItems.Add(config);
                else
                {
                    wrapper = new ReportConfigurationWrapper();
                    wrapper.ConfigACClassWF = acClassWF;
                    if (wrapper.ConfigACClassWF != null)
                    {
                        wrapper.ConfigItems.Add(config);
                        wrapperList.Add(wrapper);
                    }
                }
            }
            return wrapperList;
        }

        #endregion

        #endregion

        #region Workflows

        private VBPresenterMethod _presenter = null;
        private MaterialWFACClassMethod _ProcessWorkflow;
        private VBPresenterMaterialWF _MaterialWFPresenter;
        bool _MatPresenterRightsChecked = false;
        public VBPresenterMaterialWF MaterialWFPresenter
        {
            get
            {
                if (_MaterialWFPresenter == null)
                {
                    _MaterialWFPresenter = this.ACUrlCommand("VBPresenterMaterialWF(CurrentDesign)") as VBPresenterMaterialWF;
                    if (_MaterialWFPresenter == null && !_MatPresenterRightsChecked)
                        Messages.Error(this, "This user has no rights for viewing workflows. Assign rights for VBPresenterMaterialWF in the group management!", true);
                    _MatPresenterRightsChecked = true;
                }
                return _MaterialWFPresenter;
            }
        }

        public void LoadMaterialWorkflows()
        {
            if (MaterialWFPresenter != null)
            {
                if (SelectedProdOrderPartslist == null || SelectedProdOrderPartslist.Partslist == null)
                    MaterialWFPresenter.Load(null);
                else //if (SelectedProdOrderPartslist != null && SelectedProdOrderPartslist.Partslist != null)
                    MaterialWFPresenter.Load(SelectedProdOrderPartslist.Partslist.MaterialWF);
            }
        }

        [ACMethodInfo("Material", "en{'SetSelectedMaterial'}de{'SetSelectedMaterial'}", 608)]
        public void SetSelectedMaterial(Material value, bool selectPWNode = false)
        {
            if (SelectedIntermediate != null && IntermediateList != null && SelectedIntermediate.Material != value)
            {
                SelectedIntermediate = IntermediateList.FirstOrDefault(c => c.MaterialID == value.MaterialID);
            }
        }

        #region - Properties
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

        [ACPropertyList(635, "ProcessWorkflow")]
        public ICollection<MaterialWFACClassMethod> ProcessWorkflowList
        {
            get
            {
                if (this.SelectedProdOrderPartslist == null || this.SelectedProdOrderPartslist.Partslist == null || this.SelectedProdOrderPartslist.Partslist.MaterialWF == null)
                    return new MaterialWFACClassMethod[0];
                else
                {
                    return this.SelectedProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.ToArray().Select(c => c.MaterialWFACClassMethod).ToArray();
                }
            }
        }

        [ACPropertyCurrent(636, "ProcessWorkflow")]
        public MaterialWFACClassMethod CurrentProcessWorkflow
        {
            get
            {
                return _ProcessWorkflow;
            }
            set
            {
                if (ProcessWorkflowPresenter != null)
                {
                    _ProcessWorkflow = value;
                    if (_ProcessWorkflow == null)
                        this.ProcessWorkflowPresenter.Load(null);
                    else
                        this.ProcessWorkflowPresenter.Load(_ProcessWorkflow.ACClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>());
                    OnPropertyChanged("CurrentProcessWorkflow");
                }
            }
        }

        #endregion

        #region - Private

        private void LoadProcessWorkflows()
        {
            OnPropertyChanged("ProcessWorkflowList");
            if (ProcessWorkflowList != null)
                this.CurrentProcessWorkflow = this.ProcessWorkflowList.FirstOrDefault();
            else
                this.CurrentProcessWorkflow = null;
        }

        #endregion

        #endregion

        #region Dialog
        public VBDialogResult DialogResult
        {
            get;
            set;
        }

        [ACMethodInfo("Dialog", "en{'Dialog Production order'}de{'Dialog Produktionsauftrag'}", (short)MISort.QueryPrintDlg)]
        public void ShowDialogOrder(string orderNo, Guid prodOrderPartslistID, Guid intermPosID, Guid intermBatchPosID, Guid? facilityPreBookingID = null, Guid? facilityBookingID = null, Guid? planningMRID = null)
        {
            if (AccessPrimary == null)
                return;
            //AccessPrimary.NavACQueryDefinition.SearchWord = facilityNo;
            ACFilterItem filterItem = null;
            var query = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "ProgramNo");
            if (query.Count() > 1)
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Clear();
            else if (query.Count() == 1)
                filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "ProgramNo").FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, "ProgramNo", Global.LogicalOperators.contains, Global.Operators.and, orderNo, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            else
                filterItem.SearchWord = orderNo;

            if (planningMRID != null)
                SelectedFilterPlanningMR = FilterPlanningMRList.FirstOrDefault(c => c.PlanningMRID == planningMRID);

            this.Search();
            if (CurrentProdOrder != null && ProdOrderPartslistList != null && prodOrderPartslistID != Guid.Empty)
            {
                ProdOrderPartslist poPL = ProdOrderPartslistList.Where(c => c.ProdOrderPartslistID == prodOrderPartslistID).FirstOrDefault();
                SelectedProdOrderPartslist = poPL;
                if (poPL != null && IntermediateList != null && intermBatchPosID != Guid.Empty)
                {
                    ProdOrderPartslistPos intermBatchPos = null;
                    ProdOrderPartslistPos intermPos = null;
                    if (intermPosID == Guid.Empty)
                    {
                        intermBatchPos = poPL.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.ProdOrderPartslistPosID == intermBatchPosID).FirstOrDefault();
                        if (intermBatchPos != null)
                            intermPos = intermBatchPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos;
                    }
                    else
                    {
                        intermPos = IntermediateList.Where(c => c.ProdOrderPartslistPosID == intermPosID).FirstOrDefault();
                        //if (intermPos != null)
                        //    intermBatchPos = intermPos.ProdOrderPartslistPos_ParentProdOrderPartslistPos.Where(c => c.ProdOrderPartslistPosID == intermBatchPosID).FirstOrDefault();
                    }

                    if (intermPos != null)
                    {
                        SelectedIntermediate = intermPos;
                        if (ProdOrderIntermediateBatchList != null)
                        {
                            if (intermBatchPos != null)
                            {
                                SelectedProdOrderIntermediateBatch = intermBatchPos;
                            }
                            else
                            {
                                intermPos = ProdOrderIntermediateBatchList.Where(c => c.ProdOrderPartslistPosID == intermBatchPosID).FirstOrDefault();
                                if (intermPos != null)
                                    SelectedProdOrderIntermediateBatch = intermPos;
                            }

                            if (facilityBookingID != null)
                            {
                                FacilityBooking facilityBooking = DatabaseApp.FacilityBooking.FirstOrDefault(c => c.FacilityBookingID == facilityBookingID);
                                if (facilityBooking != null)
                                {
                                    if (facilityBooking.ProdOrderPartslistPosID != null && facilityBooking.ProdOrderPartslistPosID == intermPos.ProdOrderPartslistPosID)
                                    {
                                        SelectedInwardFacilityBooking = facilityBooking;
                                        FocusView = BSOProdOrderTab.InwardBookingHistory.ToString();
                                    }
                                    else if (facilityBooking.ProdOrderPartslistPosRelationID != null
                                        && intermPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Any(x => x.ProdOrderPartslistPosRelationID == facilityBooking.ProdOrderPartslistPosRelationID))
                                    {
                                        SelectedOutwardPartslistPos = intermPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.FirstOrDefault(x => x.ProdOrderPartslistPosRelationID == facilityBooking.ProdOrderPartslistPosRelationID);
                                        SelectedOutwardFacilityBooking = facilityBooking;
                                        FocusView = BSOProdOrderTab.OutwardBookingHistory.ToString();
                                    }
                                }
                            }
                            else if (facilityPreBookingID != null)
                            {
                                FacilityPreBooking facilityPreBooking = DatabaseApp.FacilityPreBooking.FirstOrDefault(c => c.FacilityPreBookingID == facilityPreBookingID);
                                if (facilityPreBooking != null)
                                {
                                    if (facilityPreBooking.ProdOrderPartslistPosID != null && facilityPreBooking.ProdOrderPartslistPosID == intermPos.ProdOrderPartslistPosID)
                                    {
                                        SelectedInwardFacilityPreBooking = facilityPreBooking;
                                        FocusView = BSOProdOrderTab.InwardPreBookingHistory.ToString();
                                    }
                                    else if (facilityPreBooking.ProdOrderPartslistPosRelationID != null
                                        && intermPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Any(x => x.ProdOrderPartslistPosRelationID == facilityPreBooking.ProdOrderPartslistPosRelationID))
                                    {
                                        SelectedOutwardPartslistPos = intermPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.FirstOrDefault(x => x.ProdOrderPartslistPosRelationID == facilityPreBooking.ProdOrderPartslistPosRelationID);
                                        SelectedOutwardFacilityPreBooking = facilityPreBooking;
                                        FocusView = BSOProdOrderTab.OutwardPreBookingHistory.ToString();
                                    }
                                }
                            }
                            else
                                FocusView = BSOProdOrderTab.Intermediate.ToString();

                        }
                        else
                            FocusView = BSOProdOrderTab.Intermediate.ToString();

                    }
                }
            }
            ShowDialog(this, "DisplayOrderDialog");
            this.ParentACComponent.StopComponent(this);
            _IsEnabledACProgram = true;
        }

        //private bool InShowDialogOrderInfo = false;

        [ACMethodInfo("Dialog", "en{'Dialog Production order'}de{'Dialog Produktionsauftrag'}", (short)MISort.QueryPrintDlg + 1)]
        public void ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;

            //InShowDialogOrderInfo = true;
            // Falls Produktionsauftrag
            ProdOrderPartslistPosRelation relation = null;
            ProdOrderBatch batch = null;
            ProdOrderPartslistPos partslistPos = null;
            ProdOrderPartslist poPartslist = null;
            FacilityBooking facilityBooking = null;
            FacilityPreBooking facilityPreBooking = null;
            PlanningMR planningMR = null;

            foreach (var entry in paOrderInfo.Entities)
            {
                if (entry.EntityName == ProdOrderPartslistPosRelation.ClassName)
                {
                    relation = this.DatabaseApp.ProdOrderPartslistPosRelation
                        .Include(c => c.TargetProdOrderPartslistPos)
                        .Include(c => c.TargetProdOrderPartslistPos.ProdOrderPartslist)
                        .Include(c => c.TargetProdOrderPartslistPos.ProdOrderPartslist.ProdOrder)
                        .Where(c => c.ProdOrderPartslistPosRelationID == entry.EntityID)
                        .FirstOrDefault();
                    break;
                }
                else if (entry.EntityName == ProdOrderBatch.ClassName)
                {
                    batch = this.DatabaseApp.ProdOrderBatch
                        .Include(c => c.ProdOrderPartslistPos_ProdOrderBatch)
                        .Include(c => c.ProdOrderPartslist)
                        .Include(c => c.ProdOrderPartslist.ProdOrder)
                        .Where(c => c.ProdOrderBatchID == entry.EntityID).FirstOrDefault();
                }
                else if (entry.EntityName == ProdOrderPartslistPos.ClassName)
                {
                    partslistPos = DatabaseApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == entry.EntityID);
                }
                else if (entry.EntityName == OrderLog.ClassName)
                {
                    _IsEnabledACProgram = false;
                    OrderLog currentOrderLog = DatabaseApp.OrderLog.FirstOrDefault(c => c.VBiACProgramLogID == entry.EntityID);
                    if (currentOrderLog == null || (currentOrderLog.ProdOrderPartslistPos == null && currentOrderLog.ProdOrderPartslistPosRelation == null))
                        return;
                    relation = currentOrderLog.ProdOrderPartslistPosRelation;
                    partslistPos = currentOrderLog.ProdOrderPartslistPos;
                }
                else if (entry.EntityName == ProdOrderPartslist.ClassName)
                {
                    poPartslist = this.DatabaseApp.ProdOrderPartslist
                        .Where(c => c.ProdOrderPartslistID == entry.EntityID)
                        .FirstOrDefault();
                }
                else if (entry.EntityName == FacilityBooking.ClassName)
                {
                    facilityBooking = DatabaseApp.FacilityBooking.FirstOrDefault(c => c.FacilityBookingID == entry.EntityID);
                }
                else if (entry.EntityName == FacilityPreBooking.ClassName)
                {
                    facilityPreBooking = DatabaseApp.FacilityPreBooking.FirstOrDefault(c => c.FacilityPreBookingID == entry.EntityID);
                }
                else if (entry.EntityName == PlanningMR.ClassName)
                {
                    planningMR = DatabaseApp.PlanningMR.FirstOrDefault(c => c.PlanningMRID == entry.EntityID);
                }
            }

            if (batch == null && relation == null && partslistPos == null && poPartslist == null)
                return;

            string orderNo = "";
            Guid prodOrderPartslistID = Guid.Empty;
            Guid intermPosID = Guid.Empty;
            Guid intermBatchPosID = Guid.Empty;
            Guid? facilityBookingID = null;
            Guid? facilityPreBookingID = null;
            Guid? planningMRID = null;
            if (planningMR != null)
                planningMRID = planningMR.PlanningMRID;

            if (facilityBooking != null)
            {
                if (facilityBooking.ProdOrderPartslistPosRelationID != null)
                {
                    orderNo = facilityBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                    prodOrderPartslistID = facilityBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ProdOrderPartslistID;
                    if (facilityBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ParentProdOrderPartslistPosID == null)
                    {
                        intermPosID = facilityBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPosID;
                        intermBatchPosID = facilityBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPosID;
                    }
                    else
                    {
                        intermPosID = facilityBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ParentProdOrderPartslistPosID ?? Guid.Empty;
                        intermBatchPosID = facilityBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPosID;
                    }
                }
                else if (facilityBooking.ProdOrderPartslistPosID != null)
                {
                    orderNo = facilityBooking.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                    prodOrderPartslistID = facilityBooking.ProdOrderPartslistPos.ProdOrderPartslistID;
                    if (facilityBooking.ProdOrderPartslistPos.ParentProdOrderPartslistPosID == null)
                    {
                        intermPosID = facilityBooking.ProdOrderPartslistPosID ?? Guid.Empty;
                        intermBatchPosID = facilityBooking.ProdOrderPartslistPosID ?? Guid.Empty;
                    }
                    else
                    {
                        intermPosID = facilityBooking.ProdOrderPartslistPos.ParentProdOrderPartslistPosID ?? Guid.Empty;
                        intermBatchPosID = facilityBooking.ProdOrderPartslistPosID ?? Guid.Empty;
                    }
                }
                facilityBookingID = facilityBooking.FacilityBookingID;
            }
            else if (facilityPreBooking != null)
            {
                if (facilityPreBooking.ProdOrderPartslistPosRelationID != null)
                {
                    orderNo = facilityPreBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                    prodOrderPartslistID = facilityPreBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ProdOrderPartslistID;
                    if (facilityPreBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ParentProdOrderPartslistPosID == null)
                    {
                        intermPosID = facilityPreBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPosID;
                        intermBatchPosID = facilityPreBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPosID;
                    }
                    else
                    {
                        intermPosID = facilityPreBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ParentProdOrderPartslistPosID ?? Guid.Empty;
                        intermBatchPosID = facilityPreBooking.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPosID;
                    }
                }
                else if (facilityPreBooking.ProdOrderPartslistPosID != null)
                {
                    orderNo = facilityPreBooking.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                    prodOrderPartslistID = facilityPreBooking.ProdOrderPartslistPos.ProdOrderPartslistID;
                    if (facilityPreBooking.ProdOrderPartslistPos.ParentProdOrderPartslistPosID == null)
                    {
                        intermPosID = facilityPreBooking.ProdOrderPartslistPosID ?? Guid.Empty;
                        intermBatchPosID = facilityPreBooking.ProdOrderPartslistPosID ?? Guid.Empty;
                    }
                    else
                    {
                        intermPosID = facilityPreBooking.ProdOrderPartslistPos.ParentProdOrderPartslistPosID ?? Guid.Empty;
                        intermBatchPosID = facilityPreBooking.ProdOrderPartslistPosID ?? Guid.Empty;
                    }
                }
                facilityPreBookingID = facilityPreBooking.FacilityPreBookingID;
            }
            else if (relation != null)
            {
                intermBatchPosID = relation.TargetProdOrderPartslistPos.ProdOrderPartslistPosID;
                intermPosID = relation.TargetProdOrderPartslistPos.ParentProdOrderPartslistPosID.HasValue ? relation.TargetProdOrderPartslistPos.ParentProdOrderPartslistPosID.Value : Guid.Empty;
                prodOrderPartslistID = relation.TargetProdOrderPartslistPos.ProdOrderPartslist.ProdOrderPartslistID;
                orderNo = relation.TargetProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
            }
            else if (batch != null)
            {
                var poPos = batch.ProdOrderPartslistPos_ProdOrderBatch.FirstOrDefault();
                if (poPos != null)
                {
                    intermBatchPosID = poPos.ProdOrderPartslistPosID;
                    intermPosID = poPos.ParentProdOrderPartslistPosID.HasValue ? poPos.ParentProdOrderPartslistPosID.Value : Guid.Empty;
                }
                prodOrderPartslistID = batch.ProdOrderPartslist.ProdOrderPartslistID;
                orderNo = batch.ProdOrderPartslist.ProdOrder.ProgramNo;
            }
            else
            {
                if (partslistPos != null)
                {
                    poPartslist = partslistPos.ProdOrderPartslist;
                    intermPosID = partslistPos.ProdOrderPartslistPosID;
                }
                orderNo = poPartslist.ProdOrder.ProgramNo;
                prodOrderPartslistID = poPartslist.ProdOrderPartslistID;
            }
            ShowDialogOrder(orderNo, prodOrderPartslistID, intermPosID, intermBatchPosID, facilityPreBookingID, facilityBookingID, planningMRID);
            //InShowDialogOrderInfo = false;
        }

        [ACMethodCommand("Dialog", "en{'OK'}de{'OK'}", (short)MISort.Okay)]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", "en{'Cancel'}de{'Abbrechen'}", (short)MISort.Cancel)]
        public void DialogCancel()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            CloseTopDialog();
        }
        #endregion

        #region IACBSOConfigStoreSelection

        /// <summary>
        /// Get mandatory items for conf
        /// </summary>
        public List<IACConfigStore> MandatoryConfigStores
        {
            get
            {
                List<IACConfigStore> listOfSelectedStores = new List<IACConfigStore>();
                if (SelectedProdOrderPartslist != null)
                {
                    listOfSelectedStores.Add(SelectedProdOrderPartslist);
                }
                return listOfSelectedStores;
            }
        }

        public IACConfigStore CurrentConfigStore
        {
            get
            {
                if (SelectedProdOrderPartslist == null) return null;
                return SelectedProdOrderPartslist;
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
                OnPropertyChanged("VisitedMethods");
            }
        }
        public void AddVisitedMethods(core.datamodel.ACClassMethod acClassMethod)
        {
            if (!VisitedMethods.Contains(acClassMethod))
                VisitedMethods.Add(acClassMethod);
        }

        #endregion

        #region IACBSOACProgramProvider

        public IEnumerable<core.datamodel.ACProgram> GetACProgram()
        {
            string wfACUrl = WorkflowACUrl;
            _WorkflowACUrl = null;

            if (string.IsNullOrEmpty(wfACUrl))
                return null;

            return DatabaseApp.OrderLog.Where(c => ((c.ProdOrderPartslistPos != null &&
                                                     c.ProdOrderPartslistPos.ProdOrderPartslistID == SelectedProdOrderPartslist.ProdOrderPartslistID)
                                                 || (c.ProdOrderPartslistPosRelation != null
                                                 && c.ProdOrderPartslistPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslistID == SelectedProdOrderPartslist.ProdOrderPartslistID))
                                                 && c.VBiACProgramLog.ACProgramLog1_ParentACProgramLog == null
                                                 && c.VBiACProgramLog.ACProgramLog_ParentACProgramLog.Any(pl => pl.ACUrl.EndsWith("\\" + wfACUrl)))
                                       .ToArray()
                                       .Select(x => x.ACProgramLog.ACProgram)
                                       .Distinct()
                                       .OrderBy(c => c.ProgramNo);
        }

        private string _WorkflowACUrl;

        public string WorkflowACUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_WorkflowACUrl))
                {
                    if (ProcessWorkflowPresenter != null)
                    {
                        if (ProcessWorkflowPresenter.SelectedRootWFNode.ACIdentifier == ProcessWorkflowPresenter.SelectedACUrl)
                            return null;
                        else if (ProcessWorkflowPresenter.SelectedWFNode == null)
                            return null;
                        _WorkflowACUrl = ProcessWorkflowPresenter.SelectedWFNode.GetACUrl(ProcessWorkflowPresenter.SelectedRootWFNode);
                    }
                    else
                        return null;
                }
                return _WorkflowACUrl;
            }
        }
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

        public string GetProdOrderProgramNo()
        {
            return CurrentProdOrder.ProgramNo;
        }

        public DateTime GetProdOrderInsertDate()
        {
            return CurrentProdOrder.InsertDate;
        }

        #endregion

        #region Facility & FacilityCharge Dialog

        #region  Facility & FacilityCharge Dialog -> FacilityPreBooking -> Available quants

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
                .Take(ACQueryDefinition.C_DefaultTakeCount)
                .ToList();
        }



        #endregion

        private void ShowDlgFacility(Facility preselectedFacility)
        {

            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(preselectedFacility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                switch (FacilitySelectLoctation)
                {
                    case FacilitySelectLoctation.PrebookingInward:
                        SelectedInwardACMethodBooking.InwardFacility = facility;
                        OnPropertyChanged("SelectedInwardACMethodBooking");
                        break;
                    case FacilitySelectLoctation.PrebookingOutward:
                        SelectedOutwardACMethodBooking.OutwardFacility = facility;
                        OnPropertyChanged("SelectedOutwardACMethodBooking");
                        break;
                    default:
                        break;
                }
            }
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
            SelectedOutwardACMethodBooking.OutwardFacility = SelectedPreBookingAvailableQuants.Facility;
            SelectedOutwardACMethodBooking.OutwardFacilityCharge = SelectedPreBookingAvailableQuants;
            SelectedOutwardACMethodBooking.OutwardMaterial = null;
            SelectedOutwardACMethodBooking.OutwardFacilityLot = null;
            OnPropertyChanged("SelectedOutwardACMethodBooking");
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

        #endregion
        public override object Clone()
        {
            BSOProdOrder clone = base.Clone() as BSOProdOrder;
            clone.CurrentProdOrder = this.CurrentProdOrder;
            clone.SelectedProdOrderPartslist = this.SelectedProdOrderPartslist;
            clone.SelectedIntermediate = this.SelectedIntermediate;
            clone.SelectedProdOrderIntermediateBatch = this.SelectedProdOrderIntermediateBatch;
            clone.SelectedBatch = this.SelectedBatch;
            return clone;
        }

        #region ACAction

        /// <summary>
        /// ACAction is called when one IACInteractiveObject (Source) wants to inform another IACInteractiveObject (Target) about an relevant interaction-event.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public override void ACAction(ACActionArgs actionArgs)
        {
            if (actionArgs.ElementAction == Global.ElementActionType.VBDesignLoaded)
                OnVBDesignLoaded(actionArgs.DropObject.VBContent);

            else
                base.ACAction(actionArgs);
        }

        private void OnVBDesignLoaded(string vbContent)
        {
            if (vbContent == "SelectedRootWFNode"
                && SelectedIntermediate != null
                && MaterialWFPresenter != null)
            {
                MaterialWFPresenter.SelectMaterial(SelectedIntermediate.Material);
            }
        }

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(DlgAvailableQuantsOk):
                    DlgAvailableQuantsOk();
                    return true;
                case nameof(IsEnabledDlgAvailableQuantsOk):
                    result = IsEnabledDlgAvailableQuantsOk();
                    return true;
                case nameof(DlgAvailableQuantsCancel):
                    DlgAvailableQuantsCancel();
                    return true;
                case nameof(BookSelectedInwardACMethodBooking):
                    BookSelectedInwardACMethodBooking(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : true);
                    return true;
                case nameof(IsEnabledBookSelectedInwardACMethodBooking):
                    result = IsEnabledBookSelectedInwardACMethodBooking();
                    return true;
                case nameof(BookAllInwardACMBookings):
                    BookAllInwardACMBookings();
                    return true;
                case nameof(IsEnabledBookAllInwardACMBookings):
                    result = IsEnabledBookAllInwardACMBookings();
                    return true;
                case nameof(NewInwardFacilityPreBooking):
                    NewInwardFacilityPreBooking();
                    return true;
                case nameof(IsEnabledNewInwardFacilityPreBooking):
                    result = IsEnabledNewInwardFacilityPreBooking();
                    return true;
                case nameof(DeleteInwardFacilityPreBooking):
                    DeleteInwardFacilityPreBooking();
                    return true;
                case nameof(IsEnabledDeleteInwardFacilityPreBooking):
                    result = IsEnabledDeleteInwardFacilityPreBooking();
                    return true;
                case nameof(CancelInwardFacilityPreBooking):
                    CancelInwardFacilityPreBooking();
                    return true;
                case nameof(IsEnabledCancelInwardFacilityPreBooking):
                    result = IsEnabledCancelInwardFacilityPreBooking();
                    return true;
                case nameof(ShowDlgInwardFacility):
                    ShowDlgInwardFacility();
                    return true;
                case nameof(IsEnabledShowDlgInwardFacility):
                    result = IsEnabledShowDlgInwardFacility();
                    return true;
                case nameof(NewOutwardPartslistPos):
                    NewOutwardPartslistPos();
                    return true;
                case nameof(IsEnabledNewOutwardPartslistPos):
                    result = IsEnabledNewOutwardPartslistPos();
                    return true;
                case nameof(DeleteOutwardPartslistPos):
                    DeleteOutwardPartslistPos();
                    return true;
                case nameof(IsEnabledDeleteOutwardPartslistPos):
                    result = IsEnabledDeleteOutwardPartslistPos();
                    return true;
                case nameof(RecalcOutwardPartslistPos):
                    RecalcOutwardPartslistPos();
                    return true;
                case nameof(IsEnabledRecalcOutwardPartslistPos):
                    result = IsEnabledRecalcOutwardPartslistPos();
                    return true;
                case nameof(BookSelectedOutwardACMethodBooking):
                    BookSelectedOutwardACMethodBooking(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : true);
                    return true;
                case nameof(IsEnabledBookSelectedOutwardACMethodBooking):
                    result = IsEnabledBookSelectedOutwardACMethodBooking();
                    return true;
                case nameof(BookAllOutwardACMBookings):
                    BookAllOutwardACMBookings();
                    return true;
                case nameof(IsEnabledBookAllOutwardACMBookings):
                    result = IsEnabledBookAllOutwardACMBookings();
                    return true;
                case nameof(CancelOutwardFacilityPreBooking):
                    CancelOutwardFacilityPreBooking();
                    return true;
                case nameof(IsEnabledCancelOutwardFacilityPreBooking):
                    result = IsEnabledCancelOutwardFacilityPreBooking();
                    return true;
                case nameof(NewOutwardFacilityPreBooking):
                    NewOutwardFacilityPreBooking();
                    return true;
                case nameof(IsEnabledNewOutwardFacilityPreBooking):
                    result = IsEnabledNewOutwardFacilityPreBooking();
                    return true;
                case nameof(DeleteOutwardFacilityPreBooking):
                    DeleteOutwardFacilityPreBooking();
                    return true;
                case nameof(IsEnabledDeleteOutwardFacilityPreBooking):
                    result = IsEnabledDeleteOutwardFacilityPreBooking();
                    return true;
                case nameof(ShowDlgOutwardFacility):
                    ShowDlgOutwardFacility();
                    return true;
                case nameof(IsEnabledShowDlgOutwardFacility):
                    result = IsEnabledShowDlgOutwardFacility();
                    return true;
                case nameof(ShowDlgOutwardAvailableQuants):
                    ShowDlgOutwardAvailableQuants();
                    return true;
                case nameof(IsEnabledShowDlgOutwardAvailableQuants):
                    result = IsEnabledShowDlgOutwardAvailableQuants();
                    return true;
                case nameof(ChangeViaPartslistCancel):
                    ChangeViaPartslistCancel();
                    return true;
                case nameof(AlternativeNewProdOrderPartslistPos):
                    AlternativeNewProdOrderPartslistPos();
                    return true;
                case nameof(IsEnabledAlternativeNewProdOrderPartslistPos):
                    result = IsEnabledAlternativeNewProdOrderPartslistPos();
                    return true;
                case nameof(AlternativeDeleteProdOrderPartslistPos):
                    AlternativeDeleteProdOrderPartslistPos();
                    return true;
                case nameof(IsEnabledAlternativeDeleteProdOrderPartslistPos):
                    result = IsEnabledAlternativeDeleteProdOrderPartslistPos();
                    return true;
                case nameof(RecalcIntermediateSum):
                    RecalcIntermediateSum();
                    return true;
                case nameof(IsEnabledRecalcIntermediateSum):
                    result = IsEnabledRecalcIntermediateSum();
                    return true;
                case nameof(SearchIntermediate):
                    SearchIntermediate();
                    return true;
                case nameof(BatchAdd):
                    BatchAdd();
                    return true;
                case nameof(BatchDelete):
                    BatchDelete();
                    return true;
                case nameof(BatchDeleteAll):
                    BatchDeleteAll();
                    return true;
                case nameof(IsEnabledBatchAdd):
                    result = IsEnabledBatchAdd();
                    return true;
                case nameof(IsEnabledBatchDelete):
                    result = IsEnabledBatchDelete();
                    return true;
                case nameof(IsEnabledBatchAllDelete):
                    result = IsEnabledBatchAllDelete();
                    return true;
                case nameof(ProdOrderIntermediateBatchCreateDlg):
                    ProdOrderIntermediateBatchCreateDlg();
                    return true;
                case nameof(ProdOrderIntermediateBatchAssign):
                    ProdOrderIntermediateBatchAssign();
                    return true;
                case nameof(ProdOrderIntermediateBatchUnAssign):
                    ProdOrderIntermediateBatchUnAssign();
                    return true;
                case nameof(ProdOrderIntermediateBatchClearSelection):
                    ProdOrderIntermediateBatchClearSelection();
                    return true;
                case nameof(GenerateLotNumber):
                    GenerateLotNumber();
                    return true;
                case nameof(GeneratePartLotNumber):
                    GeneratePartLotNumber();
                    return true;
                case nameof(RemovePartLotNumber):
                    RemovePartLotNumber();
                    return true;
                case nameof(RecalcProdOrderIntermediateBatch):
                    RecalcProdOrderIntermediateBatch();
                    return true;
                case nameof(IsEnabledProdOrderIntermediateBatch):
                    result = IsEnabledProdOrderIntermediateBatch();
                    return true;
                case nameof(IsEnabledProdOrderIntermediateBatchCreateDlg):
                    result = IsEnabledProdOrderIntermediateBatchCreateDlg();
                    return true;
                case nameof(IsEnabledProdOrderIntermediateBatchAssign):
                    result = IsEnabledProdOrderIntermediateBatchAssign();
                    return true;
                case nameof(IsEnabledProdOrderIntermediateBatchUnAssign):
                    result = IsEnabledProdOrderIntermediateBatchUnAssign();
                    return true;
                case nameof(IsEnabledProdOrderIntermediateBatchClearSelection):
                    result = IsEnabledProdOrderIntermediateBatchClearSelection();
                    return true;
                case nameof(IsEnabledGenerateLotNumber):
                    result = IsEnabledGenerateLotNumber();
                    return true;
                case nameof(IsEnabledGeneratePartLotNumber):
                    result = IsEnabledGeneratePartLotNumber();
                    return true;
                case nameof(IsEnabledRemovePartLotNumber):
                    result = IsEnabledRemovePartLotNumber();
                    return true;
                case nameof(BatchCreateAutomaticallyCalculate):
                    BatchCreateAutomaticallyCalculate();
                    return true;
                case nameof(BatchCreateAutomaticallyCreateDlgOk):
                    BatchCreateAutomaticallyCreateDlgOk();
                    return true;
                case nameof(IsEnabledBatchCreateAutomaticallyCalculate):
                    result = IsEnabledBatchCreateAutomaticallyCalculate();
                    return true;
                case nameof(IsEnabledBatchCreateAutomaticallyCreateDlgOk):
                    result = IsEnabledBatchCreateAutomaticallyCreateDlgOk();
                    return true;
                case nameof(BatchCreateManuallyCmd):
                    BatchCreateManuallyCmd();
                    return true;
                case nameof(BatchCreateDlgCancel):
                    BatchCreateDlgCancel();
                    return true;
                case nameof(SetSelectedMaterial):
                    SetSelectedMaterial((gip.mes.datamodel.Material)acParameter[0], acParameter.Count() == 2 ? (System.Boolean)acParameter[1] : false);
                    return true;
                case nameof(ShowDialogOrder):
                    ShowDialogOrder((System.String)acParameter[0], (System.Guid)acParameter[1], (System.Guid)acParameter[2], (System.Guid)acParameter[3], acParameter.Count() == 5 ? (System.Nullable<System.Guid>)acParameter[4] : null, acParameter.Count() == 6 ? (System.Nullable<System.Guid>)acParameter[5] : null, acParameter.Count() == 7 ? (System.Nullable<System.Guid>)acParameter[6] : null);
                    return true;
                case nameof(ShowDialogOrderInfo):
                    ShowDialogOrderInfo((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(DialogOK):
                    DialogOK();
                    return true;
                case nameof(DialogCancel):
                    DialogCancel();
                    return true;
                case nameof(OnTrackingCall):
                    OnTrackingCall((TrackingAndTracingSearchModel)acParameter[0], (gip.core.datamodel.IACObject)acParameter[1], (System.Object)acParameter[2], (TrackingEnginesEnum)acParameter[3]);
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
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
                case nameof(RecalcAllQuantites):
                    RecalcAllQuantites();
                    return true;
                case nameof(FinishOrder):
                    result = FinishOrder();
                    return true;
                case nameof(IsEnabledFinishOrder):
                    result = IsEnabledFinishOrder();
                    return true;
                case nameof(CheckForOpenPostings):
                    CheckForOpenPostings();
                    return true;
                case nameof(NavigateToOpenPosting):
                    NavigateToOpenPosting();
                    return true;
                case nameof(Search):
                    Search(acParameter.Count() == 1 ? (gip.mes.datamodel.ProdOrder)acParameter[0] : null, acParameter.Count() == 2 ? (gip.mes.datamodel.ProdOrderPartslist)acParameter[1] : null);
                    return true;
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case nameof(AddPartslist):
                    AddPartslist();
                    return true;
                case nameof(IsEnabledAddPartslist):
                    result = IsEnabledAddPartslist();
                    return true;
                case nameof(AddPartslistDlgOk):
                    AddPartslistDlgOk();
                    return true;
                case nameof(IsEnabledAddPartslistDlgOk):
                    result = IsEnabledAddPartslistDlgOk();
                    return true;
                case nameof(AddPartslistDlgCancel):
                    AddPartslistDlgCancel();
                    return true;
                case nameof(DeleteProdOrderPartslist):
                    DeleteProdOrderPartslist();
                    return true;
                case nameof(IsEnabledDeleteProdOrderPartslist):
                    result = IsEnabledDeleteProdOrderPartslist();
                    return true;
                case nameof(StartProdOrderPartslist):
                    StartProdOrderPartslist();
                    return true;
                case nameof(IsEnabledStartProdOrderPartslist):
                    result = IsEnabledStartProdOrderPartslist();
                    return true;
                case nameof(PartslistChangeTargetQuantityDlg):
                    PartslistChangeTargetQuantityDlg();
                    return true;
                case nameof(PartslistChangeTargetQuantityDlgOk):
                    PartslistChangeTargetQuantityDlgOk();
                    return true;
                case nameof(PartslistChangeTargetQuantityDlgCancel):
                    PartslistChangeTargetQuantityDlgCancel();
                    return true;
                case nameof(IsEnabledPartslistChangeTargetQuantityDlg):
                    result = IsEnabledPartslistChangeTargetQuantityDlg();
                    return true;
                case nameof(IsEnabledPartslistChangeTargetQuantityDlgOk):
                    result = IsEnabledPartslistChangeTargetQuantityDlgOk();
                    return true;
                case nameof(BOMExplosion):
                    BOMExplosion();
                    return true;
                case nameof(IsEnabledBOMExplosion):
                    result = IsEnabledBOMExplosion();
                    return true;
                case nameof(BOMExplosionDlgOk):
                    BOMExplosionDlgOk();
                    return true;
                case nameof(IsEnabledBOMExplosionDlgOk):
                    result = IsEnabledBOMExplosionDlgOk();
                    return true;
                case nameof(BOMExplosionDlgCancel):
                    BOMExplosionDlgCancel();
                    return true;
                case nameof(NewProdOrderPartslistPos):
                    NewProdOrderPartslistPos();
                    return true;
                case nameof(DeleteProdOrderPartslistPos):
                    DeleteProdOrderPartslistPos();
                    return true;
                case nameof(CreateNewLabOrderFromProdOrderPartslist):
                    CreateNewLabOrderFromProdOrderPartslist();
                    return true;
                case nameof(IsEnabledCreateNewLabOrderFromProdOrderPartslist):
                    result = IsEnabledCreateNewLabOrderFromProdOrderPartslist();
                    return true;
                case nameof(ShowLabOrderFromProdOrder):
                    ShowLabOrderFromProdOrder();
                    return true;
                case nameof(IsEnabledShowLabOrderFromProdOrder):
                    result = IsEnabledShowLabOrderFromProdOrder();
                    return true;
                case nameof(IsEnabledNewProdOrderPartslistPos):
                    result = IsEnabledNewProdOrderPartslistPos();
                    return true;
                case nameof(IsEnabledDeleteProdOrderPartslistPos):
                    result = IsEnabledDeleteProdOrderPartslistPos();
                    return true;
                case nameof(ChangeViaPartslistDlg):
                    ChangeViaPartslistDlg();
                    return true;
                case nameof(IsEnabledChangeViaPartslistDlg):
                    result = IsEnabledChangeViaPartslistDlg();
                    return true;
                case nameof(ChangeViaPartslistOk):
                    ChangeViaPartslistOk();
                    return true;
                case nameof(IsEnabledChangeViaPartslistOk):
                    result = IsEnabledChangeViaPartslistOk();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region Behavior

        #region Behavior -> FocusView

        private string _FocusView;
        public string FocusView
        {
            get
            {
                return _FocusView;
            }
            set
            {
                if (_FocusView != value)
                {
                    _FocusView = value;
                    OnPropertyChanged("FocusView");
                }
            }
        }

        #endregion

        #endregion

        #region BackgroundWorker

        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();

            worker.ProgressInfo.OnlyTotalProgress = true;
            worker.ProgressInfo.AddSubTask(command, 0, 9);
            string message = Translator.GetTranslation("en{'Running {0}...'}de{'{0} läuft...'}");
            worker.ProgressInfo.ReportProgress(command, 0, string.Format(message, command));

            string updateName = Root.Environment.User.Initials;

            switch (command)
            {
                case nameof(RecalcAllQuantities):
                    e.Result = RecalcAllQuantities(DatabaseApp, CurrentProdOrder, true);
                    break;
                case nameof(DoRecalcAllQuantitiesForSelected):
                    ProdOrder[] prodOrders = ProdOrderList.Where(c => c.IsSelected).ToArray();
                    e.Result = DoRecalcAllQuantitiesForSelected(DatabaseApp, prodOrders);
                    break;

            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = worker.EventArgs.Argument.ToString();
            MsgWithDetails resultMsg = null;
            ClearMessages();
            if (e.Cancelled)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Info, Message = string.Format(@"Operation {0} canceled by user!", command) });
            }
            if (e.Error != null)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format(@"Error by doing {0}! Message:{1}", command, e.Error.Message) });
            }
            else
            {
                switch (command)
                {
                    case nameof(RecalcAllQuantities):
                        resultMsg = (MsgWithDetails)e.Result;
                        if (resultMsg == null || resultMsg.IsSucceded())
                        {
                            Load(true);
                            OnPropertyChanged("CurrentProdOrder");
                        }
                        else
                        {
                            SendMessage(resultMsg);
                        }
                        break;
                    case nameof(DoRecalcAllQuantitiesForSelected):
                        resultMsg = (MsgWithDetails)e.Result;
                        if (resultMsg == null || resultMsg.IsSucceded())
                        {
                            Load(true);
                            OnPropertyChanged("CurrentProdOrder");
                        }
                        else
                        {
                            SendMessage(resultMsg);
                        }
                        break;

                }
            }
        }

        #endregion

        #region BackgroundWorker -> DoMehtods


        private MsgWithDetails RecalcAllQuantities(DatabaseApp databaseApp, ProdOrder prodOrder, bool saveChanges)
        {
            return ProdOrderManager.RecalcAllQuantitesAndStatistics(databaseApp, prodOrder, saveChanges);
        }

        private MsgWithDetails DoRecalcAllQuantitiesForSelected(DatabaseApp databaseApp, ProdOrder[] prodOrders)
        {
            MsgWithDetails msg = new MsgWithDetails();
            foreach (ProdOrder prodOrder in prodOrders)
            {
                MsgWithDetails detMsg = RecalcAllQuantities(databaseApp, prodOrder, false);
                if (detMsg != null)
                {
                    msg.AddDetailMessage(detMsg);
                }
            }
            MsgWithDetails saveMsg = databaseApp.ACSaveChanges();
            if (saveMsg != null)
            {
                msg.AddDetailMessage(saveMsg);
            }
            return msg;
        }

        #endregion
    }

}
#region TabEnums

public enum BSOProdOrderTab
{
    Intermediate,
    OutwardBookingHistory,
    InwardBookingHistory,
    OutwardPreBookingHistory,
    InwardPreBookingHistory
}


#endregion