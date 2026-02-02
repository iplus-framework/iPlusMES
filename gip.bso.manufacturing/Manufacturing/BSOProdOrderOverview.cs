// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static gip.core.datamodel.Global;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Prodorder overview'}de{'Auftrag Überblick'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOProdOrderOverview : ACBSOvb
    {

        #region ctor's

        public BSOProdOrderOverview(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _CalculateStatistics = new ACPropertyConfigValue<bool>(this, nameof(CalculateStatistics), true);
            _CommandTimeout = new ACPropertyConfigValue<int>(this, nameof(CommandTimeout), 3);
        }


        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            // Default filter values
            FilterEndDate = DateTime.Now.Date.AddDays(1);
            FilterStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            _FilterTimeFilterTypeList = LoadFilterTimeFilterTypeList();
            SelectedFilterTimeFilterType = _FilterTimeFilterTypeList.FirstOrDefault();

            _ProdOrderManager = ACProdOrderManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("ProdOrderManager not configured");


            return true;
        }

        public override bool ACPostInit()
        {
            bool postInit = base.ACPostInit();
            _ = CalculateStatistics;
            _ = CommandTimeout;
            return postInit;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;
            var b = await base.ACDeInit(deleteACClassTask);

            return b;
        }

        #endregion

        #region Database

        private DatabaseApp _DatabaseApp;
        public override DatabaseApp DatabaseApp
        {
            get
            {
                if (_DatabaseApp == null)
                {
                    _DatabaseApp = GetDatabaseApp();
                }

                return _DatabaseApp;
            }
        }

        #endregion

        #region Manager
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
        #endregion

        #region Configuration

        private ACPropertyConfigValue<bool> _CalculateStatistics;
        [ACPropertyConfig("CalculateStatistics")]
        public bool CalculateStatistics
        {
            get
            {
                return _CalculateStatistics.ValueT;
            }
            set
            {
                _CalculateStatistics.ValueT = value;
            }
        }

        private ACPropertyConfigValue<int> _CommandTimeout;
        [ACPropertyConfig("en{'CommandTimeout (min)'}de{'CommandTimeout (min)'}")]
        public int CommandTimeout
        {
            get
            {
                return _CommandTimeout.ValueT;
            }
            set
            {
                _CommandTimeout.ValueT = value;
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

        #region Properties -> Filter

        /// <summary>
        /// Source Property: 
        /// </summary>
        private DateTime? _FilterStartDate;
        [ACPropertySelected(999, "FilterStartDate", "en{'From'}de{'Von'}")]
        public DateTime? FilterStartDate
        {
            get
            {
                return _FilterStartDate;
            }
            set
            {
                if (_FilterStartDate != value)
                {
                    _FilterStartDate = value;
                    OnPropertyChanged(nameof(FilterStartDate));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private DateTime? _FilterEndDate;
        [ACPropertySelected(999, "FilterEndDate", "en{'to'}de{'Bis'}")]
        public DateTime? FilterEndDate
        {
            get
            {
                return _FilterEndDate;
            }
            set
            {
                if (_FilterEndDate != value)
                {
                    _FilterEndDate = value;
                    OnPropertyChanged(nameof(FilterEndDate));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterProgramNo;
        [ACPropertySelected(999, "FilterProgramNo", "en{'Program No.'}de{'AuftragNr.'}")]
        public string FilterProgramNo
        {
            get
            {
                return _FilterProgramNo;
            }
            set
            {
                if (_FilterProgramNo != value)
                {
                    _FilterProgramNo = value;
                    OnPropertyChanged(nameof(FilterProgramNo));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterMaterialNo;
        [ACPropertySelected(999, "FilterMaterialNo", "en{'Material'}de{'Material'}")]
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
                    OnPropertyChanged("FilterMaterialNo");
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterDepartmentName;
        [ACPropertyInfo(999, "FilterDepartmentName", "en{'Department'}de{'Abteilung'}")]
        public string FilterDepartmentName
        {
            get
            {
                return _FilterDepartmentName;
            }
            set
            {
                if (_FilterDepartmentName != value)
                {
                    _FilterDepartmentName = value;
                    OnPropertyChanged(nameof(FilterDepartmentName));
                }
            }
        }

        #region Properties -> Filter -> FilterTimeFilterType (TimeFilterTypeEnum)

        public TimeFilterTypeEnum? FilterTimeFilterType
        {
            get
            {
                if (SelectedFilterTimeFilterType == null)
                    return null;
                return (TimeFilterTypeEnum)SelectedFilterTimeFilterType.Value;
            }
        }


        private ACValueItem _SelectedFilterTimeFilterType;
        /// <summary>
        /// Selected property for ACValueItem
        /// </summary>
        /// <value>The selected TimeFilterType</value>
        [ACPropertySelected(305, "FilterTimeFilterType", "en{'Time filter'}de{'Zeitfilter'}")]
        public ACValueItem SelectedFilterTimeFilterType
        {
            get
            {
                return _SelectedFilterTimeFilterType;
            }
            set
            {
                if (_SelectedFilterTimeFilterType != value)
                {
                    _SelectedFilterTimeFilterType = value;
                    OnPropertyChanged(nameof(SelectedFilterTimeFilterType));
                }
            }
        }

        private List<ACValueItem> _FilterTimeFilterTypeList;
        /// <summary>
        /// List property for ACValueItem
        /// </summary>
        /// <value>The FilterPickingState list</value>
        [ACPropertyList(306, "FilterTimeFilterType")]
        public List<ACValueItem> FilterTimeFilterTypeList
        {
            get
            {
                if (_FilterTimeFilterTypeList == null)
                    _FilterTimeFilterTypeList = LoadFilterTimeFilterTypeList();
                return _FilterTimeFilterTypeList;
            }
        }

        public ACValueItemList LoadFilterTimeFilterTypeList()
        {
            ACValueItemList list = null;
            gip.core.datamodel.ACClass enumClass = Database.ContextIPlus.GetACType(typeof(TimeFilterTypeEnum));
            if (enumClass != null && enumClass.ACValueListForEnum != null)
                list = enumClass.ACValueListForEnum;
            else
                list = new ACValueListTimeFilterTypeEnum();
            return list;
        }

        #endregion

        #region Properties -> Filter -> Facility


        #region Properties -> Filter -> Facility -> Common
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

        ACAccess<Facility> _AccessFilterFacility;
        [ACPropertyAccess(9999, "FilterFacility")]
        public ACAccess<Facility> AccessFilterFacility
        {
            get
            {
                if (_AccessFilterFacility == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Facility.ClassName, ACType.ACIdentifier);

                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(FilterFacilityNavigationqueryDefaultSort);
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(FilterFacilityNavigationqueryDefaultFilter);
                    }

                    _AccessFilterFacility = navACQueryDefinition.NewAccessNav<Facility>("FilterFacility", this);
                    //_AccessFilterFromFacility.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessFilterFacility;
            }
        }

        #endregion

        #region Properties -> Filter -> Facility -> Input

        [ACPropertyInfo(9999, "InputFilterFacility")]
        public IEnumerable<Facility> InputFilterFacilityList
        {
            get
            {
                return AccessFilterFacility.NavList;
            }
        }

        private Facility _SelectedInputFilterFacility;
        [ACPropertySelected(9999, "InputFilterFacility", ConstApp.Facility)]
        public Facility SelectedInputFilterFacility
        {
            get
            {
                return _SelectedInputFilterFacility;
            }
            set
            {
                if (_SelectedInputFilterFacility != value)
                {
                    _SelectedInputFilterFacility = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Properties -> Filter -> Facility -> FinalInput

        [ACPropertyInfo(9999, "FinalInputFilterFacility")]
        public IEnumerable<Facility> FinalInputFilterFacilityList
        {
            get
            {
                return AccessFilterFacility.NavList;
            }
        }

        private Facility _SelectedFinalInputFilterFacility;
        [ACPropertySelected(9999, "FinalInputFilterFacility", ConstApp.Facility)]
        public Facility SelectedFinalInputFilterFacility
        {
            get
            {
                return _SelectedFinalInputFilterFacility;
            }
            set
            {
                if (_SelectedFinalInputFilterFacility != value)
                {
                    _SelectedFinalInputFilterFacility = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region Properties -> Messages

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

        #region Properties -> OverviewProdOrderPartslist

        private ProdOrderPartslistOverview _SelectedOverviewProdOrderPartslist;
        /// <summary>
        /// Selected property for OverviewProdOrderPartslist
        /// </summary>
        /// <value>The selected OverviewProdOrderPartslist</value>
        [ACPropertySelected(9999, "OverviewProdOrderPartslist", "en{'TODO: OverviewProdOrderPartslist'}de{'TODO: OverviewProdOrderPartslist'}")]
        public ProdOrderPartslistOverview SelectedOverviewProdOrderPartslist
        {
            get
            {
                return _SelectedOverviewProdOrderPartslist;
            }
            set
            {
                if (_SelectedOverviewProdOrderPartslist != value)
                {
                    _SelectedOverviewProdOrderPartslist = value;
                    OnPropertyChanged(nameof(SelectedOverviewProdOrderPartslist));
                }
            }
        }

        private List<ProdOrderPartslistOverview> _OverviewProdOrderPartslistList;
        /// <summary>
        /// List property for OverviewProdOrderPartslist
        /// </summary>
        /// <value>The OverviewProdOrderPartslist list</value>
        [ACPropertyList(9999, "OverviewProdOrderPartslist")]

        public List<ProdOrderPartslistOverview> OverviewProdOrderPartslistList
        {
            get
            {
                return _OverviewProdOrderPartslistList;
            }
        }

        private List<ProdOrderPartslistOverview> LoadOverviewProdOrderPartslist(DatabaseApp databaseApp, DateTime? filterProdStartDate, DateTime? filterProdEndDate,
            DateTime? filterStartBookingDate, DateTime? filterEndBookingDate, string filterProgramNo, string filterMaterialNo, string filterDepartmentName)
        {

            List<ProdOrderPartslistOverview> list = s_cQry_ProdOrderPartslistOverview(databaseApp, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo, filterDepartmentName).ToList();
            foreach (ProdOrderPartslistOverview item in list)
            {
                item.CalculateDiff();
                if (CalculateStatistics)
                    item.CalculateStatistics();
            }
            return list;
        }

        #endregion

        #region Properties -> OverviewMaterial

        private ProdOrderPartslistOverview _SelectedOverviewMaterial;
        /// <summary>
        /// Selected property for OverviewMaterial
        /// </summary>
        /// <value>The selected OverviewMaterial</value>
        [ACPropertySelected(9999, "OverviewMaterial", "en{'TODO: OverviewMaterial'}de{'TODO: OverviewMaterial'}")]
        public ProdOrderPartslistOverview SelectedOverviewMaterial
        {
            get
            {
                return _SelectedOverviewMaterial;
            }
            set
            {
                if (_SelectedOverviewMaterial != value)
                {
                    _SelectedOverviewMaterial = value;
                    OnPropertyChanged(nameof(SelectedOverviewMaterial));
                }
            }
        }

        private List<ProdOrderPartslistOverview> _OverviewMaterialList;
        /// <summary>
        /// List property for OverviewMaterial
        /// </summary>
        /// <value>The OverviewMaterial list</value>
        [ACPropertyList(9999, "OverviewMaterial")]
        public List<ProdOrderPartslistOverview> OverviewMaterialList
        {
            get
            {
                return _OverviewMaterialList;
            }
        }

        private List<ProdOrderPartslistOverview> LoadOverviewMaterialList(List<ProdOrderPartslistOverview> prodOrderOverview)
        {
            List<ProdOrderPartslistOverview> list =
                prodOrderOverview
                .GroupBy(c => new { c.MaterialNo, c.MaterialName })
                .Select(c => new ProdOrderPartslistOverview()
                {
                    MaterialNo = c.Key.MaterialNo,
                    MaterialName = c.Key.MaterialName,
                    MDUnitName = c.Select(x => x.MDUnitName).DefaultIfEmpty().FirstOrDefault(),
                    GroupedOverview = c.Select(x => x).ToArray()
                })
                .OrderBy(c => c.MaterialNo)
                .ToList();

            foreach (ProdOrderPartslistOverview item in list)
            {
                item.CalculateSums();
                item.CalculateDiff();
                if (CalculateStatistics)
                    item.CalculateGroupedStatistics();
            }

            return list;
        }

        #endregion

        #region Properties -> Input

        #region Properties -> Input -> FinalInput

        private InputOverview _SelectedFinalInput;
        /// <summary>
        /// Selected property for InputOverview
        /// </summary>
        /// <value>The selected Input</value>
        [ACPropertySelected(9999, "FinalInput", "en{'TODO: Input'}de{'TODO: Input'}")]
        public InputOverview SelectedFinalInput
        {
            get
            {
                return _SelectedFinalInput;
            }
            set
            {
                if (_SelectedFinalInput != value)
                {
                    _SelectedFinalInput = value;
                    OnPropertyChanged(nameof(SelectedFinalInput));
                }
            }
        }


        private List<InputOverview> _FinalInputListFull;
        private List<InputOverview> _FinalInputList;
        /// <summary>
        /// List property for InputOverview
        /// </summary>
        /// <value>The Input list</value>
        [ACPropertyList(9999, "FinalInput")]
        public List<InputOverview> FinalInputList
        {
            get
            {
                if (_FinalInputList == null)
                    _FinalInputList = new List<InputOverview>();
                return _FinalInputList;
            }
        }

        #endregion

        #region Properties -> Input -> Input

        private InputOverview _SelectedInput;
        /// <summary>
        /// Selected property for InputOverview
        /// </summary>
        /// <value>The selected Input</value>
        [ACPropertySelected(9999, "Input", "en{'TODO: Input'}de{'TODO: Input'}")]
        public InputOverview SelectedInput
        {
            get
            {
                return _SelectedInput;
            }
            set
            {
                if (_SelectedInput != value)
                {
                    _SelectedInput = value;
                    OnPropertyChanged(nameof(SelectedInput));
                }
            }
        }


        private List<InputOverview> _InputListFull;
        private List<InputOverview> _InputList;
        /// <summary>
        /// List property for InputOverview
        /// </summary>
        /// <value>The Input list</value>
        [ACPropertyList(9999, "Input")]
        public List<InputOverview> InputList
        {
            get
            {
                if (_InputList == null)
                    _InputList = new List<InputOverview>();
                return _InputList;
            }
        }

        private List<InputOverview> LoadInputList(DatabaseApp databaseApp, DateTime? filterProdStartDate, DateTime? filterProdEndDate,
            DateTime? filterStartBookingDate, DateTime? filterEndBookingDate, string filterProgramNo, string filterMaterialNo,
            string filterDepartmentName, string filterFacilityNo)
        {
            List<InputOverview> items = s_cQry_Inputs(databaseApp, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo, filterDepartmentName, filterFacilityNo, CalculateStatistics).ToList();
            foreach (InputOverview item in items)
            {
                if (CalculateStatistics)
                    item.CalculateStatistics();
                item.CalculateDiff();
            }
            return items;
        }

        private List<InputOverview> LoadInputList1(List<ProdOrderPartslistOverview> overviewPl, string filterFacilityNo, bool calculateStatistics)
        {
            List<InputOverview> items = new List<InputOverview>();

            foreach (ProdOrderPartslistOverview pl in overviewPl)
            {
                List<InputOverview> tempItems =
                    pl
                    .ProdOrderPartslist
                    .ProdOrderPartslistPos_ProdOrderPartslist
                    .Where(c =>
                     c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
                        && (
                                string.IsNullOrEmpty(filterFacilityNo)
                                || c
                                    .ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                                    .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                                    .Where(x =>
                                                    x.OutwardFacility != null
                                                    && (
                                                            x.OutwardFacility.FacilityNo == filterFacilityNo
                                                            || (x.OutwardFacility.Facility1_ParentFacility != null && x.OutwardFacility.Facility1_ParentFacility.FacilityNo == filterFacilityNo)
                                                        )
                                            ).Any()
                    )).GroupBy(c => new { c.Material.MaterialNo, c.Material.MaterialName1 })

                   .Select(c => new InputOverview()
                   {
                       // General
                       MaterialNo = c.Key.MaterialNo,
                       MaterialName = c.Key.MaterialName1,
                       MDUnitName = c.Where(x => x.Material.BaseMDUnit != null).Select(x => x.Material.BaseMDUnit.TechnicalSymbol).DefaultIfEmpty().FirstOrDefault(),

                       // Quantites
                       PlannedQuantityUOM = calculateStatistics ? 0 :
                                       c.Select(x => x.TargetQuantityUOM)
                                       .DefaultIfEmpty()
                                       .Sum(),

                       TargetQuantityUOM =
                                       c.SelectMany(x => x.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                                       .Where(x => x.TargetProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern)
                                       .Select(x => x.TargetQuantityUOM)
                                       .DefaultIfEmpty()
                                       .Sum(),

                       // Output
                       ActualQuantityUOM = calculateStatistics ? 0 :
                                        c.SelectMany(x => x.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                                        .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                                        .Select(x => x.OutwardQuantity)
                                        .DefaultIfEmpty()
                                        .Sum(),

                       ZeroPostingQuantityUOM = c.SelectMany(x => x.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                                        .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                                        .Where(x => x.OutwardFacilityCharge != null)
                                        .Select(x => x.OutwardFacilityCharge)
                                        .SelectMany(x => x.FacilityBooking_InwardFacilityCharge)
                                        .Where(x => x.FacilityBookingTypeIndex == (short)GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge)
                                        .SelectMany(x => x.FacilityBookingCharge_FacilityBooking)
                                        .Select(x => new { x.FacilityBookingChargeID, x.InwardQuantityUOM, x.OutwardQuantityUOM })
                                        .Distinct()
                                        .Select(x => x.InwardQuantityUOM - x.OutwardQuantityUOM)
                                        .DefaultIfEmpty()
                                        .Sum(),
                       GroupedPos = c.Where(x => calculateStatistics)


                   })
                   .OrderBy(c => c.MaterialNo)
                   .ToList();

                items.AddRange(tempItems);
            }

            foreach (InputOverview item in items)
            {
                if (CalculateStatistics)
                    item.CalculateStatistics();
                item.CalculateDiff();
            }
            return items;
        }


        #endregion

        #endregion

        #region Properties -> Input -> OrderPositionsForInput

        private ProdOrderPartslistPos _SelectedOrderPositionsForInput;
        /// <summary>
        /// Selected property for ProdOrderPartslistPos
        /// </summary>
        /// <value>The selected OrderPositionsForInput</value>
        [ACPropertySelected(9999, "OrderPositionsForInput", "en{'TODO: OrderPositionsForInput'}de{'TODO: OrderPositionsForInput'}")]
        public ProdOrderPartslistPos SelectedOrderPositionsForInput
        {
            get
            {
                return _SelectedOrderPositionsForInput;
            }
            set
            {
                if (_SelectedOrderPositionsForInput != value)
                {
                    _SelectedOrderPositionsForInput = value;
                    OnPropertyChanged(nameof(SelectedOrderPositionsForInput));
                }
            }
        }


        private List<ProdOrderPartslistPos> _OrderPositionsForInputList;
        /// <summary>
        /// List property for ProdOrderPartslistPos
        /// </summary>
        /// <value>The OrderPositionsForInput list</value>
        [ACPropertyList(9999, "OrderPositionsForInput")]
        public List<ProdOrderPartslistPos> OrderPositionsForInputList
        {
            get
            {
                if (_OrderPositionsForInputList == null)
                    _OrderPositionsForInputList = new List<ProdOrderPartslistPos>();
                return _OrderPositionsForInputList;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Methods -> OverviewProdOrderPartslist

        /// <summary>
        /// Source Property: Search
        /// </summary>
        [ACMethodInfo("Search", "en{'Show Orders & Materials'}de{'Aufträge & Materialien anzeigen'}", 100)]
        public void Search()
        {
            if (!IsEnabledSearch())
                return;

            _OverviewProdOrderPartslistList = null;
            _OverviewMaterialList = null;

            OnPropertyChanged(nameof(OverviewProdOrderPartslistList));
            OnPropertyChanged(nameof(OverviewMaterialList));

            BackgroundWorker.RunWorkerAsync(nameof(DoLoadOverviewProdOrderPartslistAndMaterial));
            ShowDialog(this, DesignNameProgressBar);
        }

        /// <summary>
        /// Source Property: Search
        /// </summary>
        [ACMethodInfo("Search2", "en{'Show all'}de{'Alles anzeigen'}", 101)]
        public void Search2()
        {
            if (!IsEnabledSearch())
                return;

            _OverviewProdOrderPartslistList = null;
            _OverviewMaterialList = null;
            _InputListFull = null;
            _InputList = null;
            _FinalInputList = null;
            _FinalInputListFull = null;

            SelectedInputFilterFacility = null;
            SelectedFinalInputFilterFacility = null;

            OnPropertyChanged(nameof(OverviewProdOrderPartslistList));
            OnPropertyChanged(nameof(OverviewMaterialList));
            OnPropertyChanged(nameof(InputList));
            OnPropertyChanged(nameof(FinalInputList));

            BackgroundWorker.RunWorkerAsync(nameof(DoSearch2));
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledSearch()
        {
            return FilterStartDate.HasValue
                    && FilterEndDate.HasValue
                    && FilterEndDate.Value > FilterStartDate.Value
                    && (FilterEndDate.Value - FilterStartDate.Value).Days <= 365;
        }


        [ACMethodInteraction("NavigateToProdOrder", ConstApp.ShowProdOrder, 502, false, nameof(SelectedOverviewProdOrderPartslist))]
        public void NavigateToProdOrder()
        {
            if (!IsEnabledNavigateToProdOrder())
                return;
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = SelectedOverviewProdOrderPartslist.ProdOrderPartslist.ProdOrderPartslistID,
                    EntityName = ProdOrderPartslist.ClassName
                });
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToProdOrder()
        {
            return SelectedOverviewProdOrderPartslist != null && SelectedOverviewProdOrderPartslist.ProdOrderPartslist != null;
        }

        private Tuple<bool, Facility> SelectFilterFacilityCommon(Facility preSelectedFacility)
        {
            Facility selectedFacility = null;
            bool userSelection = false;
            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(preSelectedFacility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                userSelection = true;
                Facility facility = dlgResult.ReturnValue as Facility;
                if (facility != null)
                    if (!AccessFilterFacility.NavList.Contains(facility))
                        AccessFilterFacility.NavList.Add(facility);

                if (selectedFacility != facility)
                {
                    selectedFacility = facility;
                }
            }
            return new Tuple<bool, Facility>(userSelection, selectedFacility);
        }

        /// <summary>
        /// 
        /// </summary>
        [ACMethodInfo("ShowDlgInputFilterFacility", "en{'Choose facility'}de{'Lager auswählen'}", 102)]
        public void ShowDlgInputFilterFacility()
        {
            if (!IsEnabledShowDlgFilterToFacility())
                return;
            Tuple<bool, Facility> userSelection = SelectFilterFacilityCommon(SelectedInputFilterFacility);
            if (userSelection.Item1)
            {
                SelectedInputFilterFacility = userSelection.Item2;
            }
        }

        public bool IsEnabledShowDlgFilterToFacility()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        [ACMethodInfo("ShowDlgInputFilterFacility", "en{'Choose facility'}de{'Lager auswählen'}", 103)]
        public void ShowDlgFinalInputFilterFacility()
        {
            if (!IsEnabledShowDlgFinalInputFilterFacility())
                return;
            Tuple<bool, Facility> userSelection = SelectFilterFacilityCommon(SelectedFinalInputFilterFacility);
            if (userSelection.Item1)
            {
                SelectedFinalInputFilterFacility = userSelection.Item2;
            }
        }

        public bool IsEnabledShowDlgFinalInputFilterFacility()
        {
            return true;
        }

        #endregion

        #region Methods -> Input

        /// <summary>
        /// Source Property: SearchInputs
        /// </summary>
        [ACMethodInfo("SearchInputs", "en{'Sum material inputs'}de{'Materialeinsatz summieren'}", 104)]
        public void SearchInputs()
        {
            if (!IsEnabledSearchInputs())
                return;

            _InputList = null;
            _InputListFull = null;
            OnPropertyChanged(nameof(InputList));

            BackgroundWorker.RunWorkerAsync(nameof(DoLoadInput));
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledSearchInputs()
        {
            return IsEnabledSearch();
        }

        /// <summary>
        /// Source Property: SearchInputs
        /// </summary>
        [ACMethodInfo("FilterFacilityInputs", "en{'Filter'}de{'Filter'}", 105)]
        public void FilterFacilityInputs()
        {
            if (!IsEnabledFilterFacilityInputs())
                return;

            _InputList = null;
            OnPropertyChanged(nameof(InputList));

            BackgroundWorker.RunWorkerAsync(nameof(DoFilterFacilityInputs));
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledFilterFacilityInputs()
        {
            return _InputListFull != null && _InputListFull.Any();
        }

        /// <summary>
        /// Source Property: SearchInputs
        /// </summary>
        [ACMethodInfo("FilterFacilityFinalInputs", "en{'Filter'}de{'Filter'}", 106)]
        public void FilterFacilityFinalInputs()
        {
            if (!IsEnabledFilterFacilityInputs())
                return;

            _FinalInputList = null;
            OnPropertyChanged(nameof(FinalInputList));

            BackgroundWorker.RunWorkerAsync(nameof(DoFilterFacilityFinalInputs));
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledFilterFacilityFinalInputs()
        {
            return _FinalInputListFull != null && _FinalInputListFull.Any();
        }

        #endregion

        #region Methods -> RecalculateAllStats

        [ACMethodInfo("ShowDlgFilterFacility", "en{'Recalculate Statistics'}de{'Statistiken neu berechnen'}", 107, true)]
        public void RecalculateAllStats()
        {
            if (!IsEnabledShowDlgFilterToFacility())
                return;

            BackgroundWorker.RunWorkerAsync(nameof(DoRecalculateAllStatsAsync));
            ShowDialog(this, DesignNameProgressBar);

        }

        public bool IsEnabledRecalculateAll()
        {
            return this.ProdOrderManager != null;
        }

        #endregion

        #region Methods -> OrderPositionsForInput

        [ACMethodInteraction("ShowOrderPositionsForInput", "en{'Show Order Inputs'}de{'Auftrag Einsats anzeigen'}", 507, false, nameof(SelectedInput))]
        public void ShowOrderPositionsForInput()
        {
            if (!IsEnabledShowOrderPositionsForInput())
                return;

            BackgroundWorker.RunWorkerAsync(nameof(DoLoadOrderPositionsForInputList));
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledShowOrderPositionsForInput()
        {
            return SelectedInput != null && !string.IsNullOrEmpty(SelectedInput.MaterialNo);
        }

        [ACMethodInteraction("NavigateToProdOrder1", ConstApp.ShowProdOrder, 505, false, nameof(SelectedOrderPositionsForInput))]
        public void NavigateToProdOrder1()
        {
            if (!IsEnabledNavigateToProdOrder1())
                return;
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = SelectedOrderPositionsForInput.ProdOrderPartslist.ProdOrderPartslistID,
                    EntityName = ProdOrderPartslist.ClassName
                });
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToProdOrder1()
        {
            return SelectedOrderPositionsForInput != null && SelectedOrderPositionsForInput.ProdOrderPartslist != null;
        }

        #endregion

        #endregion

        #region Background worker

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
            worker.ProgressInfo.TotalProgress.ProgressText = $"Doing {command}...";

            string updateName = Root.Environment.User.Initials;
            switch (command)
            {
                case nameof(DoLoadOverviewProdOrderPartslistAndMaterial):
                    e.Result = DoLoadOverviewProdOrderPartslistAndMaterial();
                    break;

                case nameof(DoLoadInput):
                    e.Result = DoLoadInput();
                    break;
                case nameof(DoRecalculateAllStatsAsync):
                    e.Result = DoRecalculateAllStatsAsync();
                    break;
                case nameof(DoLoadOrderPositionsForInputList):
                    bool loadRelatedLists = OverviewProdOrderPartslistList == null || !OverviewProdOrderPartslistList.Any();
                    e.Result = DoLoadOrderPositionsForInputList(SelectedInput.MaterialNo, OverviewProdOrderPartslistList, loadRelatedLists);
                    break;
                case nameof(DoSearch2):
                    e.Result = DoSearch2();
                    break;
                case nameof(DoFilterFacilityInputs):
                    e.Result = DoFilterFacilityInputs(SelectedInputFilterFacility != null ? SelectedInputFilterFacility.FacilityNo : "");
                    break;
                case nameof(DoFilterFacilityFinalInputs):
                    e.Result = DoFilterFacilityFinalInputs(SelectedFinalInputFilterFacility != null ? SelectedFinalInputFilterFacility.FacilityNo : "");
                    break;
            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);
            ClearMessages();
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = worker.EventArgs.Argument.ToString();

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
                BSOProdOrderOverview_SearchResult result = null;
                switch (command)
                {
                    case nameof(DoLoadOverviewProdOrderPartslistAndMaterial):
                        result = e.Result as BSOProdOrderOverview_SearchResult;
                        if (result != null)
                        {
                            _OverviewProdOrderPartslistList = result.OverviewProdOrderPartslist;
                            _OverviewMaterialList = result.OverviewMaterial;

                            OnPropertyChanged(nameof(OverviewProdOrderPartslistList));
                            OnPropertyChanged(nameof(OverviewMaterialList));
                        }
                        break;
                    case nameof(DoLoadInput):
                        result = e.Result as BSOProdOrderOverview_SearchResult;
                        if (result != null)
                        {
                            _InputList = result.InputOverview;
                            OnPropertyChanged(nameof(InputList));
                        }
                        break;
                    case nameof(DoRecalculateAllStatsAsync):
                        break;
                    case nameof(DoLoadOrderPositionsForInputList):
                        result = e.Result as BSOProdOrderOverview_SearchResult;
                        if (result != null)
                        {

                            if (result.OverviewProdOrderPartslist != null)
                            {
                                _OverviewProdOrderPartslistList = result.OverviewProdOrderPartslist;
                                _OverviewMaterialList = result.OverviewMaterial;

                                OnPropertyChanged(nameof(OverviewProdOrderPartslistList));
                                OnPropertyChanged(nameof(OverviewMaterialList));
                            }

                            _OrderPositionsForInputList = result.OrderPositionsForInputList;
                            ShowDialog(this, "OrderPositionsForInputDlg");
                        }
                        break;
                    case nameof(DoSearch2):
                        result = e.Result as BSOProdOrderOverview_SearchResult;
                        if (result != null)
                        {
                            _OverviewProdOrderPartslistList = result.OverviewProdOrderPartslist;
                            _OverviewMaterialList = result.OverviewMaterial;

                            OnPropertyChanged(nameof(OverviewProdOrderPartslistList));
                            OnPropertyChanged(nameof(OverviewMaterialList));

                            _InputList = result.InputOverview;
                            _InputListFull = result.InputOverview;
                            OnPropertyChanged(nameof(InputList));

                            _FinalInputList = result.FinalProductInputOverview;
                            _FinalInputListFull = result.FinalProductInputOverview;
                            OnPropertyChanged(nameof(FinalInputList));
                        }
                        break;
                    case nameof(DoFilterFacilityInputs):
                        _InputList = e.Result as List<InputOverview>;
                        OnPropertyChanged(nameof(InputList));
                        break;
                    case nameof(DoFilterFacilityFinalInputs):
                        _FinalInputList = e.Result as List<InputOverview>;
                        OnPropertyChanged(nameof(FinalInputList));
                        break;
                }
                if (result != null)
                {
                    TimeSpan timespan = result.OperationEndTime - result.OperationStartTime;
                    SendMessage(new Msg() { MessageLevel = eMsgLevel.Info, Message = string.Format(@"Operation {0} completed! Execution time:{1}", result.OperationName, timespan.ToString("mm\\:ss")) });
                }
            }
        }

        #endregion

        #region Background worker DoMethods

        private BSOProdOrderOverview_SearchResult DoLoadOverviewProdOrderPartslistAndMaterial()
        {
            return DoSearch(nameof(DoLoadOverviewProdOrderPartslistAndMaterial), true, false);
        }

        private BSOProdOrderOverview_SearchResult DoLoadInput()
        {
            return DoSearch(nameof(DoLoadInput), false, true);
        }

        private BSOProdOrderOverview_SearchResult DoSearch(string operationName, bool searchPlAndMt, bool searchInputs)
        {
            DateTime startTime = DateTime.Now;

            DateTime? startProdTime = FilterStartDate;
            DateTime? endProdTime = FilterEndDate;

            DateTime? startBookingTime = null;
            DateTime? endBookingTime = null;

            string facilityNo = null;

            if (SelectedInputFilterFacility != null)
            {
                facilityNo = SelectedInputFilterFacility.FacilityNo;
            }

            if (FilterTimeFilterType != null && FilterTimeFilterType == TimeFilterTypeEnum.BookingTime)
            {
                startBookingTime = FilterStartDate;
                endBookingTime = FilterEndDate;

                startProdTime = null;
                endProdTime = null;
            }


            List<ProdOrderPartslistOverview> overviewPl = null;
            List<ProdOrderPartslistOverview> overviewMt = null;

            if (searchPlAndMt)
            {
                overviewPl = LoadOverviewProdOrderPartslist(DatabaseApp, startProdTime, endProdTime, startBookingTime, endBookingTime, FilterProgramNo, FilterMaterialNo, FilterDepartmentName);
                if (overviewPl != null)
                    overviewMt = LoadOverviewMaterialList(overviewPl);
            }

            List<InputOverview> inputOverview = null;
            if (searchInputs)
            {
                inputOverview = LoadInputList(DatabaseApp, startProdTime, endProdTime, startBookingTime, endBookingTime, FilterProgramNo, FilterMaterialNo, FilterDepartmentName, facilityNo);
            }

            return new BSOProdOrderOverview_SearchResult()
            {
                OperationName = operationName,
                OperationStartTime = startTime,
                OperationEndTime = DateTime.Now,

                OverviewProdOrderPartslist = overviewPl,
                OverviewMaterial = overviewMt,
                InputOverview = inputOverview
            };
        }

        private bool DoRecalculateAllStatsAsync()
        {
            IEnumerable<Guid> prodOrders = null;
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                DateTime? startProdTime = FilterStartDate;
                DateTime? endProdTime = FilterEndDate;
                if (startProdTime.HasValue && endProdTime.HasValue)
                    prodOrders = databaseApp.ProdOrder.Where(c => c.InsertDate > startProdTime && c.InsertDate < endProdTime).Select(c => c.ProdOrderID).ToArray();
                else
                    prodOrders = databaseApp.ProdOrder.Select(c => c.ProdOrderID).ToArray();
            }

            CurrentProgressInfo.TotalProgress.ProgressRangeFrom = 0;
            CurrentProgressInfo.TotalProgress.ProgressRangeTo = prodOrders.Count();
            CurrentProgressInfo.TotalProgress.ProgressCurrent = 0;

            foreach (var prodOrderID in prodOrders)
            {
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    ProdOrder prodOrder = databaseApp.ProdOrder.FirstOrDefault(c => c.ProdOrderID == prodOrderID);
                    if (prodOrder != null)
                    {
                        this.ProdOrderManager.RecalcAllQuantitesAndStatistics(databaseApp, prodOrder, true, this);
                        CurrentProgressInfo.TotalProgress.ProgressCurrent++;
                    }
                }
            }
            return true;
        }

        private BSOProdOrderOverview_SearchResult DoLoadOrderPositionsForInputList(string materialNo, List<ProdOrderPartslistOverview> prodOrderPartslistOverviews, bool loadRelatedLists)
        {
            BSOProdOrderOverview_SearchResult result = new BSOProdOrderOverview_SearchResult()
            {
                OperationName = nameof(DoLoadOrderPositionsForInputList),
                OperationStartTime = DateTime.Now
            };


            if (loadRelatedLists)
            {
                BSOProdOrderOverview_SearchResult preparedData = DoSearch("", true, false);
                result.OverviewProdOrderPartslist = preparedData.OverviewProdOrderPartslist;
                result.OverviewMaterial = preparedData.OverviewMaterial;

                prodOrderPartslistOverviews = result.OverviewProdOrderPartslist;
            }

            List<ProdOrderPartslistPos> list = new List<ProdOrderPartslistPos>();

            Material material = DatabaseApp.Material.FirstOrDefault(c => c.MaterialNo == materialNo);
            if (material != null && prodOrderPartslistOverviews != null)
            {
                Guid[] plIds = prodOrderPartslistOverviews.Select(c => c.ProdOrderPartslist.ProdOrderPartslistID).ToArray();

                list =
                    DatabaseApp
                    .ProdOrderPartslistPos
                    .Include("ProdOrderPartslist")
                    .Include("ProdOrderPartslist.Partslist")
                    .Include("ProdOrderPartslist.ProdOrder")
                    .Where(c => plIds.Contains(c.ProdOrderPartslistID) && c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot && c.Material.MaterialNo == materialNo)
                    .ToList();
            }

            if (list.Any())
            {
                list = list.OrderByDescending(c => c.InputQForFinalScrapActualOutputPer).ToList();
            }

            result.OrderPositionsForInputList = list;

            result.OperationEndTime = DateTime.Now;
            return result;
        }

        private BSOProdOrderOverview_SearchResult DoSearch2()
        {
            BSOProdOrderOverview_SearchResult result = new BSOProdOrderOverview_SearchResult() { OperationStartTime = DateTime.Now, OperationName = nameof(DoSearch2) };

            DateTime startTime = DateTime.Now;

            DateTime? startProdTime = FilterStartDate;
            DateTime? endProdTime = FilterEndDate;

            DateTime? startBookingTime = null;
            DateTime? endBookingTime = null;

            string facilityNo = null;

            if (SelectedInputFilterFacility != null)
            {
                facilityNo = SelectedInputFilterFacility.FacilityNo;
            }

            if (FilterTimeFilterType != null && FilterTimeFilterType == TimeFilterTypeEnum.BookingTime)
            {
                startBookingTime = FilterStartDate;
                endBookingTime = FilterEndDate;

                startProdTime = null;
                endProdTime = null;
            }

            ProdOrderPartslistPos[] query = s_cQry_FinalInput(DatabaseApp, startProdTime, endProdTime, startBookingTime, endBookingTime, FilterProgramNo, FilterMaterialNo, FilterDepartmentName, CalculateStatistics).ToArray();

            result.OverviewProdOrderPartslist = new List<ProdOrderPartslistOverview>();
            result.OverviewMaterial = new List<ProdOrderPartslistOverview>();
            result.InputOverview = new List<InputOverview>();
            result.FinalProductInputOverview = new List<InputOverview>();
            result.InputOverview = new List<InputOverview>();


            foreach (ProdOrderPartslistPos pos in query)
            {
                AddFinalInput(result, pos);
                AddInput(result, pos);
                AddProdOrderPartslistOverview(result, pos);
            }


            foreach (var finalInputOverview in result.FinalProductInputOverview)
            {
                if (CalculateStatistics)
                    finalInputOverview.CalculateStatistics();
                finalInputOverview.CalculateDiff();
            }


            foreach (var inputOverview in result.InputOverview)
            {
                if (CalculateStatistics)
                    inputOverview.CalculateStatistics();
                inputOverview.CalculateDiff();
            }

            //result.InputOverview =
            //    result.FinalProductInputOverview
            //    .GroupBy(c => new { c.MaterialNo, c.MaterialName })
            //    .Select(c => new InputOverview()
            //    {
            //        MaterialNo = c.Key.MaterialNo,
            //        MaterialName = c.Key.MaterialName,

            //        PlannedQuantityUOM = c.Select(x => x.PlannedQuantityUOM).Sum(),
            //        TargetQuantityUOM = c.Select(x => x.TargetQuantityUOM).Sum(),
            //        ActualQuantityUOM = c.Select(x => x.ActualQuantityUOM).Sum(),
            //        ZeroPostingQuantityUOM = c.Select(x => x.ZeroPostingQuantityUOM).Sum(),

            //        GroupedPos = c.SelectMany(x => x.GroupedPos).ToList(),

            //        MDUnitName = c.Select(x => x.MDUnitName).FirstOrDefault()
            //    })
            //    .OrderBy(c => c.MaterialNo)
            //    .ToList();

            //foreach (var inputOverview in result.InputOverview)
            //{
            //    if (CalculateStatistics)
            //        inputOverview.CalculateStatistics();
            //    inputOverview.CalculateDiff();
            //}


            // Finalize
            result.OverviewProdOrderPartslist = result.OverviewProdOrderPartslist.OrderBy(c => c.ProgramNo).ThenBy(c => c.MaterialNo).ToList();
            result.OverviewMaterial = LoadOverviewMaterialList(result.OverviewProdOrderPartslist);
            result.FinalProductInputOverview = result.FinalProductInputOverview.OrderBy(c => c.FinalProductMaterialNo).ThenBy(c => c.MaterialNo).ToList();
            result.InputOverview = result.InputOverview.OrderBy(c => c.MaterialNo).ToList();

            result.OperationEndTime = DateTime.Now;

            return result;
        }

        private void AddProdOrderPartslistOverview(BSOProdOrderOverview_SearchResult result, ProdOrderPartslistPos pos)
        {
            // OverviewProdOrderPartslist
            ProdOrderPartslistOverview plOverview =
                result
                .OverviewProdOrderPartslist
                .Where(c => c.ProdOrderPartslist.ProdOrderPartslistID == pos.ProdOrderPartslistID)
                .FirstOrDefault();
            if (plOverview == null)
            {
                plOverview = new ProdOrderPartslistOverview();
                plOverview.ProdOrderPartslist = pos.ProdOrderPartslist;

                plOverview.ProgramNo = pos.ProdOrderPartslist.ProdOrder.ProgramNo;
                plOverview.MaterialNo = pos.ProdOrderPartslist.Partslist.Material.MaterialNo;
                plOverview.MaterialName = pos.ProdOrderPartslist.Partslist.Material.MaterialName1;
                plOverview.MDUnitName = pos.ProdOrderPartslist.Partslist.MDUnit != null ?
                    pos.ProdOrderPartslist.Partslist.MDUnit.TechnicalSymbol : pos.ProdOrderPartslist.Partslist.Material.BaseMDUnit.TechnicalSymbol;
                plOverview.DepartmentUserName = pos.ProdOrderPartslist.DepartmentUserName;

                // Output
                plOverview.InwardPlannedQuantityUOM = pos.ProdOrderPartslist.TargetQuantity;

                plOverview.InwardTargetQuantityUOM =
                                pos.ProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                                .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern && !x.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any())
                                .SelectMany(x => x.ProdOrderPartslistPos_ParentProdOrderPartslistPos)
                                .Select(x => x.TargetQuantityUOM)
                                .DefaultIfEmpty()
                                .Sum();

                plOverview.InwardActualQuantityUOM = pos.ProdOrderPartslist.ActualQuantity;

                plOverview.InwardActualQuantityScrapUOM = pos.ProdOrderPartslist.ActualQuantityScrapUOM;

                // Usage
                plOverview.UsageTargetQuantityUOM =
                         pos.ProdOrderPartslist.ProdOrderPartslistPos_SourceProdOrderPartslist
                        .Select(x => x.TargetQuantityUOM)
                        .DefaultIfEmpty()
                        .Sum();

                plOverview.UsageActualQuantityUOM =
                        pos.ProdOrderPartslist.ProdOrderPartslistPos_SourceProdOrderPartslist
                        .Select(x => x.ActualQuantityUOM)
                        .DefaultIfEmpty()
                        .Sum();

                plOverview.CalculateDiff();
                if (CalculateStatistics)
                    plOverview.CalculateStatistics();

                result.OverviewProdOrderPartslist.Add(plOverview);
            }
        }

        private void AddFinalInput(BSOProdOrderOverview_SearchResult result, ProdOrderPartslistPos pos)
        {
            string finalMaterialNo = pos.FinalProdOrderPartslist.Partslist.Material.MaterialNo;
            string finalMaterialName = pos.FinalProdOrderPartslist.Partslist.Material.MaterialName1;
            // FinalProductInputOverview
            InputOverview tmpOverview =
                result
                .FinalProductInputOverview
                .Where(c =>
                    c.FinalProductMaterialNo == finalMaterialNo
                    && c.MaterialNo == pos.Material.MaterialNo
                )
                .FirstOrDefault();

            if (tmpOverview == null)
            {
                tmpOverview = new InputOverview()
                {
                    FinalProductMaterialNo = finalMaterialNo,
                    FinalProductMaterialName = finalMaterialName,
                    MaterialNo = pos.Material.MaterialNo,
                    MaterialName = pos.Material.MaterialName1,
                    GroupedPos = new List<ProdOrderPartslistPos>(),
                    MDUnitName = pos.MDUnit != null ? pos.MDUnit.TechnicalSymbol : pos.Material.BaseMDUnit.TechnicalSymbol
                };
                result.FinalProductInputOverview.Add(tmpOverview);
            }
            InputOverviewProcessPos(pos, tmpOverview);
        }

        private void AddInput(BSOProdOrderOverview_SearchResult result, ProdOrderPartslistPos pos)
        {
            // FinalProductInputOverview
            InputOverview tmpOverview =
                result
                .InputOverview
                .Where(c =>
                     c.MaterialNo == pos.Material.MaterialNo
                )
                .FirstOrDefault();

            if (tmpOverview == null)
            {
                tmpOverview = new InputOverview()
                {
                    MaterialNo = pos.Material.MaterialNo,
                    MaterialName = pos.Material.MaterialName1,
                    GroupedPos = new List<ProdOrderPartslistPos>(),
                    MDUnitName = pos.MDUnit != null ? pos.MDUnit.TechnicalSymbol : pos.Material.BaseMDUnit.TechnicalSymbol
                };
                result.InputOverview.Add(tmpOverview);
            }
            InputOverviewProcessPos(pos, tmpOverview);
        }

        private void InputOverviewProcessPos(ProdOrderPartslistPos pos, InputOverview tmpOverview)
        {
            (tmpOverview.GroupedPos as List<ProdOrderPartslistPos>).Add(pos);

            tmpOverview.PlannedQuantityUOM += CalculateStatistics ? 0 : pos.TargetQuantityUOM;

            tmpOverview.TargetQuantityUOM +=
                               pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                               .Where(x => x.TargetProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern)
                               .Select(x => x.TargetQuantityUOM)
                               .DefaultIfEmpty()
                               .Sum();

            // Output
            tmpOverview.ActualQuantityUOM += CalculateStatistics ? 0 :
                            pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                            .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                            .Select(x => x.OutwardQuantity)
                            .DefaultIfEmpty()
                            .Sum();

            tmpOverview.ZeroPostingQuantityUOM += pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                            .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                            .Where(x => x.OutwardFacilityCharge != null)
                            .Select(x => x.OutwardFacilityCharge)
                            .SelectMany(x => x.FacilityBooking_InwardFacilityCharge)
                            .Where(x => x.FacilityBookingTypeIndex == (short)GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge)
                            .SelectMany(x => x.FacilityBookingCharge_FacilityBooking)
                            .Select(x => new { x.FacilityBookingChargeID, x.InwardQuantityUOM, x.OutwardQuantityUOM })
                            .Distinct()
                            .Select(x => x.InwardQuantityUOM - x.OutwardQuantityUOM)
                            .DefaultIfEmpty()
                            .Sum();
        }

        private List<InputOverview> DoFilterFacilityInputs(string facilityNo)
        {
            if(_InputListFull == null)
                return null;

            return 
                _InputListFull
                .Where(c=>
                    c.GroupedPos
                    .Where(x=>
                        string.IsNullOrEmpty(facilityNo)
                        || x
                            .ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                            .SelectMany(y => y.FacilityBooking_ProdOrderPartslistPosRelation)
                            .Where(y =>
                                            y.OutwardFacility != null
                                            && (
                                                    y.OutwardFacility.FacilityNo == facilityNo
                                                    || (y.OutwardFacility.Facility1_ParentFacility != null && y.OutwardFacility.Facility1_ParentFacility.FacilityNo == facilityNo)
                                                )
                                    ).Any()
                                )
                    .Any())
                .ToList();
        }

        private List<InputOverview> DoFilterFacilityFinalInputs(string facilityNo)
        {
            if (_FinalInputListFull == null)
                return null;

            return
                _FinalInputListFull
                .Where(c =>
                    c.GroupedPos
                    .Where(x =>
                        string.IsNullOrEmpty(facilityNo)
                        || x
                            .ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                            .SelectMany(y => y.FacilityBooking_ProdOrderPartslistPosRelation)
                            .Where(y =>
                                            y.OutwardFacility != null
                                            && (
                                                    y.OutwardFacility.FacilityNo == facilityNo
                                                    || (y.OutwardFacility.Facility1_ParentFacility != null && y.OutwardFacility.Facility1_ParentFacility.FacilityNo == facilityNo)
                                                )
                                    ).Any()
                                )
                    .Any())
                .ToList();
        }

        #endregion

        #region Precompiled Queries (ProdOrderPartslistiOverview)


        public static readonly Func<ProdOrderPartslist, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, bool> s_cQry_ProdOrderPartslistOverview_Query = (c, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo, filterDepartmentName) =>
        {
            return
             (filterProdStartDate == null || c.StartDate >= filterProdStartDate)
                    && (filterProdEndDate == null || c.StartDate < filterProdEndDate)
                    && (filterStartBookingDate == null || c.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos).Where(x => x.InsertDate >= filterStartBookingDate).Any())
                    && (filterEndBookingDate == null || c.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos).Where(x => x.InsertDate < filterEndBookingDate).Any())
                    && (string.IsNullOrEmpty(filterProgramNo) || c.ProdOrder.ProgramNo.Contains(filterProgramNo))
                    && (string.IsNullOrEmpty(filterMaterialNo) || c.Partslist.Material.MaterialNo.Contains(filterMaterialNo) || c.Partslist.Material.MaterialName1.Contains(filterMaterialNo))
                    && (string.IsNullOrEmpty(filterDepartmentName) || c.DepartmentUserName.Contains(filterDepartmentName));
        };

        protected static readonly Func<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, IEnumerable<ProdOrderPartslistOverview>> s_cQry_ProdOrderPartslistOverview =
        EF.CompileQuery<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, IEnumerable<ProdOrderPartslistOverview>>(
            (ctx, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo, filterDepartmentName) =>
                ctx
                .ProdOrderPartslist
                .Include("ProdOrder")
                .Include("Partslist")
                .Include("Partslist.Material")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.FacilityBooking_ProdOrderPartslistPos")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.FacilityBooking_ProdOrderPartslistPosRelation")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.SourceProdOrderPartslistPos")

                .Where(c =>
                    (filterProdStartDate == null || c.StartDate >= filterProdStartDate)
                        && (filterProdEndDate == null || c.StartDate < filterProdEndDate)
                        && (filterStartBookingDate == null || c.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos).Where(x => x.InsertDate >= filterStartBookingDate).Any())
                        && (filterEndBookingDate == null || c.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos).Where(x => x.InsertDate < filterEndBookingDate).Any())
                        && (string.IsNullOrEmpty(filterProgramNo) || c.ProdOrder.ProgramNo.Contains(filterProgramNo))
                        && (string.IsNullOrEmpty(filterMaterialNo) || c.Partslist.Material.MaterialNo.Contains(filterMaterialNo) || c.Partslist.Material.MaterialName1.Contains(filterMaterialNo))
                        && (string.IsNullOrEmpty(filterDepartmentName) || c.DepartmentUserName.Contains(filterDepartmentName))
                )

                .Select(c => new ProdOrderPartslistOverview()
                {
                    // General
                    ProdOrderPartslist = c,
                    ProgramNo = c.ProdOrder.ProgramNo,
                    MaterialNo = c.Partslist.Material.MaterialNo,
                    MaterialName = c.Partslist.Material.MaterialName1,
                    MDUnitName = c.Partslist.MDUnit.TechnicalSymbol,
                    DepartmentUserName = c.DepartmentUserName,
                    // Input
                    //OutwardTargetQuantityUOM =
                    //                c.ProdOrderPartslistPos_ProdOrderPartslist
                    //                .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern)
                    //                .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                    //                .Where(x =>

                    //                        x.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
                    //                        && (x.SourceProdOrderPartslistPos.Anterograde == null || !(x.SourceProdOrderPartslistPos.Anterograde ?? true))

                    //                )
                    //                .Select(x => x.TargetQuantityUOM)
                    //                .DefaultIfEmpty()
                    //                .Sum(),

                    //OutwardActualQuantityUOM =
                    //                c.ProdOrderPartslistPos_ProdOrderPartslist
                    //                .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern)
                    //                .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                    //                .Where(x =>

                    //                        x.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
                    //                        && (x.SourceProdOrderPartslistPos.Anterograde == null || !(x.SourceProdOrderPartslistPos.Anterograde ?? true))

                    //                )
                    //                .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                    //                .Select(x => x.OutwardQuantity)
                    //                .DefaultIfEmpty()
                    //                .Sum(),

                    // Output
                    InwardPlannedQuantityUOM = c.TargetQuantity,

                    InwardTargetQuantityUOM =
                                    c.ProdOrderPartslistPos_ProdOrderPartslist
                                    .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern && !x.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any())
                                    .SelectMany(x => x.ProdOrderPartslistPos_ParentProdOrderPartslistPos)
                                    .Select(x => x.TargetQuantityUOM)
                                    .DefaultIfEmpty()
                                    .Sum(),

                    InwardActualQuantityUOM = c.ActualQuantity,
                    //c.ProdOrderPartslistPos_ProdOrderPartslist
                    //.Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern && !x.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any())
                    //.SelectMany(x => x.ProdOrderPartslistPos_ParentProdOrderPartslistPos)
                    //.SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos)
                    //.Select(x => x.InwardQuantity)
                    //.DefaultIfEmpty()
                    //.Sum(),
                    InwardActualQuantityScrapUOM = c.ActualQuantityScrapUOM,

                    // Usage
                    UsageTargetQuantityUOM =
                             c.ProdOrderPartslistPos_SourceProdOrderPartslist
                            .Select(x => x.TargetQuantityUOM)
                            .DefaultIfEmpty()
                            .Sum(),

                    UsageActualQuantityUOM =
                            c.ProdOrderPartslistPos_SourceProdOrderPartslist
                            .Select(x => x.ActualQuantityUOM)
                            .DefaultIfEmpty()
                            .Sum()

                })
                .OrderBy(c => c.ProgramNo)
                .ThenBy(c => c.MaterialNo)
        );


        protected static readonly Func<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, string, bool, IEnumerable<InputOverview>> s_cQry_Inputs =
       EF.CompileQuery<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, string, bool, IEnumerable<InputOverview>>(
           (ctx, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo, filterDepartmentName, filterFacilityNo, calculateStatistics) =>
               ctx
               .ProdOrderPartslistPos

                .Include("ProdOrderPartslist")
                .Include("ProdOrderPartslist.Partslist")
                .Include("ProdOrderPartslist.ProdOrder")
                .Include("ProdOrderPartslist.Material")

                .Include("FacilityBooking_ProdOrderPartslistPos")

                .Include("Material")

                .Include("ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos")
                .Include("ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.TargetProdOrderPartslistPos")
                .Include("ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.FacilityBooking_ProdOrderPartslistPosRelation")
                .Include("ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.FacilityBooking_ProdOrderPartslistPosRelation.OutwardFacility")

                .Where(c =>

                        // filtering partslist
                        (filterProdStartDate == null || c.ProdOrderPartslist.StartDate >= filterProdStartDate)
                        && (filterProdEndDate == null || c.ProdOrderPartslist.StartDate < filterProdEndDate)
                        && (filterStartBookingDate == null || c.FacilityBooking_ProdOrderPartslistPos.Where(x => x.InsertDate >= filterStartBookingDate).Any())
                        && (filterEndBookingDate == null || c.FacilityBooking_ProdOrderPartslistPos.Where(x => x.InsertDate < filterEndBookingDate).Any())
                        && (string.IsNullOrEmpty(filterProgramNo) || c.ProdOrderPartslist.ProdOrder.ProgramNo.Contains(filterProgramNo))
                        && (string.IsNullOrEmpty(filterMaterialNo) || c.Material.MaterialNo.Contains(filterMaterialNo) || c.Material.MaterialName1.Contains(filterMaterialNo))
                        && (string.IsNullOrEmpty(filterDepartmentName) || c.ProdOrderPartslist.DepartmentUserName.Contains(filterDepartmentName))

                        // filtering pos
                        && c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
                        && (
                                string.IsNullOrEmpty(filterFacilityNo)
                                || c
                                    .ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
                                    .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                                    .Where(x =>
                                                    x.OutwardFacility != null
                                                    && (
                                                            x.OutwardFacility.FacilityNo == filterFacilityNo
                                                            || (x.OutwardFacility.Facility1_ParentFacility != null && x.OutwardFacility.Facility1_ParentFacility.FacilityNo == filterFacilityNo)
                                                        )
                                            ).Any()
                                )
                )
                .GroupBy(c => new { c.Material.MaterialNo, c.Material.MaterialName1 })

               .Select(c => new InputOverview()
               {
                   // General
                   MaterialNo = c.Key.MaterialNo,
                   MaterialName = c.Key.MaterialName1,
                   MDUnitName = c.Where(x => x.Material.BaseMDUnit != null).Select(x => x.Material.BaseMDUnit.TechnicalSymbol).DefaultIfEmpty().FirstOrDefault(),

                   // Quantites
                   PlannedQuantityUOM = calculateStatistics ? 0 :
                                   c.Select(x => x.TargetQuantityUOM)
                                   .DefaultIfEmpty()
                                   .Sum(),

                   TargetQuantityUOM =
                                   c.SelectMany(x => x.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                                   .Where(x => x.TargetProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern)
                                   .Select(x => x.TargetQuantityUOM)
                                   .DefaultIfEmpty()
                                   .Sum(),

                   // Output
                   ActualQuantityUOM = calculateStatistics ? 0 :
                                    c.SelectMany(x => x.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                                    .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                                    .Select(x => x.OutwardQuantity)
                                    .DefaultIfEmpty()
                                    .Sum(),

                   ZeroPostingQuantityUOM = c.SelectMany(x => x.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                                    .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                                    .Where(x => x.OutwardFacilityCharge != null)
                                    .Select(x => x.OutwardFacilityCharge)
                                    .SelectMany(x => x.FacilityBooking_InwardFacilityCharge)
                                    .Where(x => x.FacilityBookingTypeIndex == (short)GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge)
                                    .SelectMany(x => x.FacilityBookingCharge_FacilityBooking)
                                    .Select(x => new { x.FacilityBookingChargeID, x.InwardQuantityUOM, x.OutwardQuantityUOM })
                                    .Distinct()
                                    .Select(x => x.InwardQuantityUOM - x.OutwardQuantityUOM)
                                    .DefaultIfEmpty()
                                    .Sum(),
                   GroupedPos = c.Where(x => calculateStatistics)


               })
               .OrderBy(c => c.MaterialNo)
       );

        protected static readonly Func<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, bool, IEnumerable<ProdOrderPartslistPos>> s_cQry_FinalInput =
       EF.CompileQuery<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, bool, IEnumerable<ProdOrderPartslistPos>>(
           (ctx, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo, filterDepartmentName, calculateStatistics) =>
              ctx
              .ProdOrderPartslistPos

               .Include("ProdOrderPartslist")
               .Include("ProdOrderPartslist.Partslist")
               .Include("ProdOrderPartslist.ProdOrder")
               .Include("ProdOrderPartslist.Partslist.Material")

               .Include("FacilityBooking_ProdOrderPartslistPos")


               .Include("ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos")
               .Include("ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.TargetProdOrderPartslistPos")
               .Include("ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.FacilityBooking_ProdOrderPartslistPosRelation")
               .Include("ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.FacilityBooking_ProdOrderPartslistPosRelation.OutwardFacility")

               .Where(c =>

                        // filtering partslist
                        (filterProdStartDate == null || c.ProdOrderPartslist.StartDate >= filterProdStartDate)
                        && (filterProdEndDate == null || c.ProdOrderPartslist.StartDate < filterProdEndDate)
                        && (filterStartBookingDate == null || c.FacilityBooking_ProdOrderPartslistPos.Where(x => x.InsertDate >= filterStartBookingDate).Any())
                        && (filterEndBookingDate == null || c.FacilityBooking_ProdOrderPartslistPos.Where(x => x.InsertDate < filterEndBookingDate).Any())
                        && (string.IsNullOrEmpty(filterProgramNo) || c.ProdOrderPartslist.ProdOrder.ProgramNo.Contains(filterProgramNo))
                        && (string.IsNullOrEmpty(filterMaterialNo) || c.ProdOrderPartslist.Partslist.Material.MaterialNo.Contains(filterMaterialNo) || c.ProdOrderPartslist.Partslist.Material.MaterialName1.Contains(filterMaterialNo))
                        && (string.IsNullOrEmpty(filterDepartmentName) || c.ProdOrderPartslist.DepartmentUserName.Contains(filterDepartmentName))

                       // filtering pos
                       && c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
               //&& (
               //        string.IsNullOrEmpty(filterFacilityNo)
               //        || c
               //            .ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos
               //            .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
               //            .Where(x =>
               //                            x.OutwardFacility != null
               //                            && (
               //                                    x.OutwardFacility.FacilityNo == filterFacilityNo
               //                                    || (x.OutwardFacility.Facility1_ParentFacility != null && x.OutwardFacility.Facility1_ParentFacility.FacilityNo == filterFacilityNo)
               //                                )
               //                    ).Any()
               //        )
               )
      );

        #endregion

        #region Overrides

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Search):
                    Search();
                    return true;
                case nameof(IsEnabledSearch):
                    result = IsEnabledSearch();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region Private methods

        private DatabaseApp GetDatabaseApp()
        {
            DatabaseApp databaseApp = null;
            if (ParentACComponent is Businessobjects
                    || !(ParentACComponent is ACBSOvb || ParentACComponent is ACBSOvbNav))
            {
                databaseApp = ACObjectContextManager.GetOrCreateContext<DatabaseApp>(this.GetACUrl());
            }
            else
            {
                ACBSOvbNav parentNav = ParentACComponent as ACBSOvbNav;
                if (parentNav != null)
                    return parentNav.DatabaseApp;
                ACBSOvb parent = ParentACComponent as ACBSOvb;
                if (parent != null)
                    return parent.DatabaseApp;
                databaseApp = ACObjectContextManager.GetOrCreateContext<DatabaseApp>(this.GetACUrl());
            }

            if (databaseApp != null)
            {
                databaseApp.Database.SetCommandTimeout(CommandTimeout * 60);
            }
            return databaseApp;
        }

        #endregion
    }

    public class BSOProdOrderOverview_SearchResult
    {
        public List<ProdOrderPartslistOverview> OverviewProdOrderPartslist { get; set; }
        public List<ProdOrderPartslistOverview> OverviewMaterial { get; set; }
        public List<InputOverview> InputOverview { get; set; }
        public List<InputOverview> FinalProductInputOverview { get; set; }
        public List<ProdOrderPartslistPos> OrderPositionsForInputList { get; set; }
        public DateTime OperationStartTime { get; set; }
        public DateTime OperationEndTime { get; set; }
        public string OperationName { get; set; }
    }
}
