﻿using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using static gip.core.datamodel.Global;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Prodorder overview'}de{'Auftrag Überblick'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOProdOrderOverview : ACBSOvb
    {
        #region const

        public const string BGWorkerMehtod_Search = "Search";
        public const string BGWorkerMehtod_SearchInput = "SearchInput";
        public const string BGWorkerMehtod_LoadOrderPositionsForInputList = "LoadOrderPositionsForInputList";

        #endregion

        #region ctor's

        public BSOProdOrderOverview(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _CalculateStatistics = new ACPropertyConfigValue<bool>(this, "CalculateStatistics", true);
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

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;
            var b = base.ACDeInit(deleteACClassTask);

            return b;
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
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(FilterFacilityNavigationqueryDefaultFilter);

                    }

                    _AccessFilterFacility = navACQueryDefinition.NewAccessNav<Facility>("FilterFacility", this);
                    //_AccessFilterFromFacility.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessFilterFacility;
            }
        }

        [ACPropertyInfo(9999, "FilterFacility")]
        public IEnumerable<Facility> FilterFacilityList
        {
            get
            {
                return AccessFilterFacility.NavList;
            }
        }

        private Facility _SelectedFilterFacility;
        [ACPropertySelected(9999, "FilterFacility", ConstApp.Facility)]
        public Facility SelectedFilterFacility
        {
            get
            {
                return _SelectedFilterFacility;
            }
            set
            {
                if (_SelectedFilterFacility != value)
                {
                    _SelectedFilterFacility = value;
                    OnPropertyChanged(nameof(SelectedFilterFacility));
                }
            }
        }

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



        #region Input

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
        [ACMethodInfo("Search", "en{'Search'}de{'Suchen'}", 999)]
        public void Search()
        {
            if (!IsEnabledSearch())
                return;

            _OverviewProdOrderPartslistList = null;
            _OverviewMaterialList = null;

            OnPropertyChanged(nameof(OverviewProdOrderPartslistList));
            OnPropertyChanged(nameof(OverviewMaterialList));

            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_Search);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledSearch()
        {
            return FilterStartDate != null && FilterEndDate != null;
        }


        [ACMethodInteraction("NavigateToProdOrder", "en{'Show Order'}de{'Auftrag anzeigen'}", 502, false, nameof(SelectedOverviewProdOrderPartslist))]
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

        /// <summary>
        /// 
        /// </summary>
        [ACMethodInfo("ShowDlgFilterFacility", "en{'Choose facility'}de{'Lager auswählen'}", 999)]
        public void ShowDlgFilterFacility()
        {
            if (!IsEnabledShowDlgFilterToFacility())
                return;
            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(SelectedFilterFacility != null ? SelectedFilterFacility : null);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                if (facility != null)
                    if (!AccessFilterFacility.NavList.Contains(facility))
                        AccessFilterFacility.NavList.Add(facility);

                if (SelectedFilterFacility != facility)
                {
                    SelectedFilterFacility = facility;
                }
            }
        }

        public bool IsEnabledShowDlgFilterToFacility()
        {
            return true;
        }

        #endregion

        #region Methods -> Input

        /// <summary>
        /// Source Property: SearchInputs
        /// </summary>
        [ACMethodInfo("SearchInputs", "en{'Search'}de{'Suchen'}", 999)]
        public void SearchInputs()
        {
            if (!IsEnabledSearchInputs())
                return;

            _InputList = null;
            OnPropertyChanged(nameof(InputList));

            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_SearchInput);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledSearchInputs()
        {
            return IsEnabledSearch();
        }

        #endregion

        #region Methods -> RecalculateAllStats

        [ACMethodInfo("ShowDlgFilterFacility", "en{'Recalculate Statistics'}de{'Recalculate Statistics'}", 999, true)]
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

            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_LoadOrderPositionsForInputList);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledShowOrderPositionsForInput()
        {
            return SelectedInput != null && !string.IsNullOrEmpty(SelectedInput.MaterialNo);
        }

        [ACMethodInteraction("NavigateToProdOrder1", "en{'Show Order'}de{'Auftrag anzeigen'}", 505, false, nameof(SelectedOrderPositionsForInput))]
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
            //worker.ProgressInfo.AddSubTask(command, 0, 9);
            //string message = Translator.GetTranslation("en{'Running {0}...'}de{'{0} läuft...'}");
            //worker.ProgressInfo.ReportProgress(command, 0, string.Format(message, command));
            worker.ProgressInfo.TotalProgress.ProgressText = "Calculating...";

            string updateName = Root.Environment.User.Initials;
            switch (command)
            {
                case BGWorkerMehtod_Search:
                    e.Result = DoSearch(command, true, false);
                    break;

                case BGWorkerMehtod_SearchInput:
                    e.Result = DoSearch(command, false, true);
                    break;
                case nameof(DoRecalculateAllStatsAsync):
                    e.Result = DoRecalculateAllStatsAsync();
                    break;
                case BGWorkerMehtod_LoadOrderPositionsForInputList:
                    bool loadRelatedLists = OverviewProdOrderPartslistList == null || !OverviewProdOrderPartslistList.Any();
                    e.Result = DoLoadOrderPositionsForInputList(DatabaseApp, SelectedInput.MaterialNo, OverviewProdOrderPartslistList, loadRelatedLists);
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
                    case BGWorkerMehtod_Search:
                        result = e.Result as BSOProdOrderOverview_SearchResult;
                        if (result != null)
                        {
                            _OverviewProdOrderPartslistList = result.OverviewProdOrderPartslist;
                            _OverviewMaterialList = result.OverviewMaterial;

                            OnPropertyChanged(nameof(OverviewProdOrderPartslistList));
                            OnPropertyChanged(nameof(OverviewMaterialList));
                        }
                        break;
                    case BGWorkerMehtod_SearchInput:
                        result = e.Result as BSOProdOrderOverview_SearchResult;
                        if (result != null)
                        {
                            _InputList = result.InputOverview;
                            OnPropertyChanged(nameof(InputList));
                        }
                        break;

                    case nameof(DoRecalculateAllStatsAsync):
                        break;

                    case BGWorkerMehtod_LoadOrderPositionsForInputList:
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

        private BSOProdOrderOverview_SearchResult DoSearch(string operationName, bool searchPlAndMt, bool searchInputs)
        {
            DateTime startTime = DateTime.Now;

            DateTime? startProdTime = FilterStartDate;
            DateTime? endProdTime = FilterEndDate;

            DateTime? startBookingTime = null;
            DateTime? endBookingTime = null;

            string facilityNo = null;

            if (SelectedFilterFacility != null)
            {
                facilityNo = SelectedFilterFacility.FacilityNo;
            }

            if (FilterTimeFilterType != null && FilterTimeFilterType == TimeFilterTypeEnum.BookingTime)
            {
                startBookingTime = FilterStartDate;
                endBookingTime = FilterEndDate;

                startProdTime = null;
                endProdTime = null;
            }

            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                databaseApp.CommandTimeout = 60 * 3;

                List<ProdOrderPartslistOverview> overviewPl = null;
                List<ProdOrderPartslistOverview> overviewMt = null;

                if (searchPlAndMt)
                {
                    overviewPl = LoadOverviewProdOrderPartslist(databaseApp, startProdTime, endProdTime, startBookingTime, endBookingTime, FilterProgramNo, FilterMaterialNo, FilterDepartmentName);
                    if (overviewPl != null)
                        overviewMt = LoadOverviewMaterialList(overviewPl);
                }

                List<InputOverview> inputOverview = null;
                if (searchInputs)
                {
                    inputOverview = LoadInputList(databaseApp, startProdTime, endProdTime, startBookingTime, endBookingTime, FilterProgramNo, FilterMaterialNo, FilterDepartmentName, facilityNo);
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
                        this.ProdOrderManager.RecalcAllQuantitesAndStatistics(databaseApp, prodOrder, true);
                        CurrentProgressInfo.TotalProgress.ProgressCurrent++;
                    }
                }
            }
            return true;
        }

        private BSOProdOrderOverview_SearchResult DoLoadOrderPositionsForInputList(DatabaseApp databaseApp, string materialNo, List<ProdOrderPartslistOverview> prodOrderPartslistOverviews, bool loadRelatedLists)
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

            Material material = databaseApp.Material.FirstOrDefault(c => c.MaterialNo == materialNo);
            if (material != null && prodOrderPartslistOverviews != null)
            {
                Guid[] plIds = prodOrderPartslistOverviews.Select(c => c.ProdOrderPartslist.ProdOrderPartslistID).ToArray();
                list =
                    databaseApp
                    .ProdOrderPartslist
                    .Where(c => plIds.Contains(c.ProdOrderPartslistID))
                    .SelectMany(c => c.ProdOrderPartslistPos_ProdOrderPartslist)
                    .Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot && c.Material.MaterialNo == materialNo)
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

        protected static readonly Func<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, IQueryable<ProdOrderPartslistOverview>> s_cQry_ProdOrderPartslistOverview =
        CompiledQuery.Compile<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, IQueryable<ProdOrderPartslistOverview>>(
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


        protected static readonly Func<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, string, bool, IQueryable<InputOverview>> s_cQry_Inputs =
       CompiledQuery.Compile<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, string, bool, IQueryable<InputOverview>>(
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
    }


    public class BSOProdOrderOverview_SearchResult
    {
        public List<ProdOrderPartslistOverview> OverviewProdOrderPartslist { get; set; }
        public List<ProdOrderPartslistOverview> OverviewMaterial { get; set; }
        public List<InputOverview> InputOverview { get; set; }
        public List<ProdOrderPartslistPos> OrderPositionsForInputList { get; set; }
        public DateTime OperationStartTime { get; set; }
        public DateTime OperationEndTime { get; set; }
        public string OperationName { get; set; }
    }
}