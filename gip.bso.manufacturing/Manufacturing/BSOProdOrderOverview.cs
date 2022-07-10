using gip.bso.masterdata;
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

        #endregion

        #region ctor's

        public BSOProdOrderOverview(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

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

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit(deleteACClassTask);


            return b;
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

        private ProdOrderPartslistiOverview _SelectedOverviewProdOrderPartslist;
        /// <summary>
        /// Selected property for OverviewProdOrderPartslist
        /// </summary>
        /// <value>The selected OverviewProdOrderPartslist</value>
        [ACPropertySelected(9999, "OverviewProdOrderPartslist", "en{'TODO: OverviewProdOrderPartslist'}de{'TODO: OverviewProdOrderPartslist'}")]
        public ProdOrderPartslistiOverview SelectedOverviewProdOrderPartslist
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

        private List<ProdOrderPartslistiOverview> _OverviewProdOrderPartslistList;
        /// <summary>
        /// List property for OverviewProdOrderPartslist
        /// </summary>
        /// <value>The OverviewProdOrderPartslist list</value>
        [ACPropertyList(9999, "OverviewProdOrderPartslist")]
        public List<ProdOrderPartslistiOverview> OverviewProdOrderPartslistList
        {
            get
            {
                return _OverviewProdOrderPartslistList;
            }
        }

        private List<ProdOrderPartslistiOverview> LoadOverviewProdOrderPartslistList(DatabaseApp databaseApp, DateTime? filterProdStartDate, DateTime? filterProdEndDate,
            DateTime? filterStartBookingDate, DateTime? filterEndBookingDate, string filterProgramNo, string filterMaterialNo, string filterDepartmentName)
        {

            List<ProdOrderPartslistiOverview> list = s_cQry_ProdOrderPartslistiOverview_Program(databaseApp, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo, filterDepartmentName).ToList();
            foreach (ProdOrderPartslistiOverview item in list)
            {
                item.CalculateDiff();
            }
            return list;
        }

        #endregion

        #region Properties -> OverviewMaterial

        private ProdOrderPartslistiOverview _SelectedOverviewMaterial;
        /// <summary>
        /// Selected property for OverviewMaterial
        /// </summary>
        /// <value>The selected OverviewMaterial</value>
        [ACPropertySelected(9999, "OverviewMaterial", "en{'TODO: OverviewMaterial'}de{'TODO: OverviewMaterial'}")]
        public ProdOrderPartslistiOverview SelectedOverviewMaterial
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

        private List<ProdOrderPartslistiOverview> _OverviewMaterialList;
        /// <summary>
        /// List property for OverviewMaterial
        /// </summary>
        /// <value>The OverviewMaterial list</value>
        [ACPropertyList(9999, "OverviewMaterial")]
        public List<ProdOrderPartslistiOverview> OverviewMaterialList
        {
            get
            {
                return _OverviewMaterialList;
            }
        }

        private List<ProdOrderPartslistiOverview> LoadOverviewMaterialList()
        {
            List<ProdOrderPartslistiOverview> list = new List<ProdOrderPartslistiOverview>();
            if (_OverviewProdOrderPartslistList != null && _OverviewProdOrderPartslistList.Any())
            {
                list =
                    _OverviewProdOrderPartslistList
                    .GroupBy(c => new { c.MaterialNo, c.MaterialName })
                    .Select(c => new ProdOrderPartslistiOverview()
                    {
                        MaterialNo = c.Key.MaterialNo,
                        MaterialName = c.Key.MaterialName,
                        MDUnitName = c.Select(x => x.MDUnitName).DefaultIfEmpty().FirstOrDefault(),

                        OutwardTargetQuantityUOM = c.Sum(x => x.OutwardTargetQuantityUOM),
                        OutwardActualQuantityUOM = c.Sum(x => x.OutwardActualQuantityUOM),

                        InwardTargetQuantityUOM = c.Sum(x => x.InwardTargetQuantityUOM),
                        InwardActualQuantityUOM = c.Sum(x => x.InwardActualQuantityUOM),

                        UsageTargetQuantityUOM = c.Sum(x => x.UsageTargetQuantityUOM),
                        UsageActualQuantityUOM = c.Sum(x => x.UsageActualQuantityUOM)
                    })
                    .OrderBy(c => c.MaterialNo)
                    .ToList();

                foreach (ProdOrderPartslistiOverview item in list)
                {
                    item.CalculateDiff();
                }
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
            List<InputOverview> items = s_cQry_Input_Program(databaseApp, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo, filterDepartmentName, filterFacilityNo).ToList();
            foreach (InputOverview item in items)
            {
                item.CalculateDiff();
            }
            return items;
        }


        #endregion

        #endregion


        #endregion

        #endregion

        #region Methods

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
            _InputList = null;

            OnPropertyChanged(nameof(OverviewProdOrderPartslistList));
            OnPropertyChanged(nameof(OverviewMaterialList));
            OnPropertyChanged(nameof(InputList));

            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_Search);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledSearch()
        {
            return FilterStartDate != null && FilterEndDate != null;
        }

        private BSOProdOrderOverview_SearchResult GetSearch(bool searchPlAndMt, bool searchInputs)
        {
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

                List<ProdOrderPartslistiOverview> overviewPl = null;
                List<ProdOrderPartslistiOverview> overviewMt = null;

                if (searchPlAndMt)
                {
                    overviewPl = LoadOverviewProdOrderPartslistList(databaseApp, startProdTime, endProdTime, startBookingTime, endBookingTime, FilterProgramNo, FilterMaterialNo, FilterDepartmentName);
                    overviewMt = LoadOverviewMaterialList();
                }

                List<InputOverview> inputOverview = null;
                if(searchInputs)
                {
                    LoadInputList(databaseApp, startProdTime, endProdTime, startBookingTime, endBookingTime, FilterProgramNo, FilterMaterialNo, FilterDepartmentName, facilityNo);
                }

                return new BSOProdOrderOverview_SearchResult()
                {
                    OverviewProdOrderPartslist = overviewPl,
                    OverviewMaterial = overviewMt,
                    InputOverview = inputOverview
                };
            }

        }

        [ACMethodInteraction("NavigateToProdOrder", "en{'Show Order'}de{'Auftrag anzeigen'}", 502, false, nameof(SelectedOverviewProdOrderPartslist))]
        public void NavigateToProdOrder()
        {
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = SelectedOverviewProdOrderPartslist.ProdOrderPartslistID,
                    EntityName = ProdOrderPartslist.ClassName
                });
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToProdOrder()
        {
            return SelectedOverviewProdOrderPartslist != null;
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

                    _InputList = null;

                    OnPropertyChanged(nameof(InputList));

                    BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_SearchInput);
                    ShowDialog(this, DesignNameProgressBar);
                }
            }
        }

        public bool IsEnabledShowDlgFilterToFacility()
        {
            return true;
        }

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
            worker.ProgressInfo.AddSubTask(command, 0, 9);
            string message = Translator.GetTranslation("en{'Running {0}...'}de{'{0} läuft...'}");
            worker.ProgressInfo.ReportProgress(command, 0, string.Format(message, command));

            string updateName = Root.Environment.User.Initials;
            switch (command)
            {
                case BGWorkerMehtod_Search:
                    e.Result = GetSearch(true, false);
                    break;

                case BGWorkerMehtod_SearchInput:
                    e.Result = GetSearch(false, true);
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
                switch (command)
                {
                    case BGWorkerMehtod_Search:
                        BSOProdOrderOverview_SearchResult result = e.Result as BSOProdOrderOverview_SearchResult;
                        if (result != null)
                        {
                            _OverviewProdOrderPartslistList = result.OverviewProdOrderPartslist;
                            _OverviewMaterialList = result.OverviewMaterial;
                            _InputList = result.InputOverview;

                            OnPropertyChanged(nameof(OverviewProdOrderPartslistList));
                            OnPropertyChanged(nameof(OverviewMaterialList));
                            OnPropertyChanged(nameof(InputList));
                        }
                        break;
                    case BGWorkerMehtod_SearchInput:
                        BSOProdOrderOverview_SearchResult result1 = e.Result as BSOProdOrderOverview_SearchResult;
                        if (result1 != null)
                        {
                            _InputList = result1.InputOverview;
                            OnPropertyChanged(nameof(InputList));
                        }
                        break;
                }
            }
        }

        #endregion


        #region Precompiled Queries (ProdOrderPartslistiOverview)


        protected static readonly Func<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, IQueryable<ProdOrderPartslistiOverview>> s_cQry_ProdOrderPartslistiOverview_Program =
        CompiledQuery.Compile<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, IQueryable<ProdOrderPartslistiOverview>>(
            (ctx, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo, filterDepartmentName) =>
                ctx
                .ProdOrderPartslist
                .Include("Partslist")
                .Include("Partslist.Material")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.FacilityBooking_ProdOrderPartslistPos")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.FacilityBooking_ProdOrderPartslistPosRelation")

                .Where(c =>
                    (filterProdStartDate == null || c.StartDate >= filterProdStartDate)
                    && (filterProdEndDate == null || c.StartDate < filterProdEndDate)
                    && (filterStartBookingDate == null || c.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos).Where(x => x.InsertDate >= filterStartBookingDate).Any())
                    && (filterEndBookingDate == null || c.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos).Where(x => x.InsertDate < filterEndBookingDate).Any())
                    && (string.IsNullOrEmpty(filterProgramNo) || c.ProdOrder.ProgramNo.Contains(filterProgramNo))
                    && (string.IsNullOrEmpty(filterMaterialNo) || c.Partslist.Material.MaterialNo.Contains(filterMaterialNo) || c.Partslist.Material.MaterialName1.Contains(filterMaterialNo))
                    && (string.IsNullOrEmpty(filterDepartmentName) || c.DepartmentUserName.Contains(filterDepartmentName))
                )

                .Select(c => new ProdOrderPartslistiOverview()
                {
                    // General
                    ProdOrderPartslistID = c.ProdOrderPartslistID,
                    ProgramNo = c.ProdOrder.ProgramNo,
                    MaterialNo = c.Partslist.Material.MaterialNo,
                    MaterialName = c.Partslist.Material.MaterialName1,
                    MDUnitName = c.Partslist.MDUnit.TechnicalSymbol,

                    // Input
                    OutwardTargetQuantityUOM =
                                    c.ProdOrderPartslistPos_ProdOrderPartslist
                                    .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern)
                                    .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                                    .Where(x =>

                                            x.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
                                            && (x.SourceProdOrderPartslistPos.Anterograde == null || !(x.SourceProdOrderPartslistPos.Anterograde ?? true))

                                    )
                                    .Select(x => x.TargetQuantityUOM)
                                    .DefaultIfEmpty()
                                    .Sum(),

                    OutwardActualQuantityUOM =
                                    c.ProdOrderPartslistPos_ProdOrderPartslist
                                    .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern)
                                    .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                                    .Where(x =>

                                            x.SourceProdOrderPartslistPos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
                                            && (x.SourceProdOrderPartslistPos.Anterograde == null || !(x.SourceProdOrderPartslistPos.Anterograde ?? true))

                                    )
                                    .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                                    .Select(x => x.OutwardQuantity)
                                    .DefaultIfEmpty()
                                    .Sum(),

                    // Output
                    InwardTargetQuantityUOM =
                                    c.ProdOrderPartslistPos_ProdOrderPartslist
                                    .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern && !x.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any())
                                    .SelectMany(x => x.ProdOrderPartslistPos_ParentProdOrderPartslistPos)
                                    .Select(x => x.TargetQuantityUOM)
                                    .DefaultIfEmpty()
                                    .Sum(),

                    InwardActualQuantityUOM =
                                    c.ProdOrderPartslistPos_ProdOrderPartslist
                                    .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern && !x.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any())
                                    .SelectMany(x => x.ProdOrderPartslistPos_ParentProdOrderPartslistPos)
                                    .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos)
                                    .Select(x => x.InwardQuantity)
                                    .DefaultIfEmpty()
                                    .Sum(),

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


        protected static readonly Func<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, string, IQueryable<InputOverview>> s_cQry_Input_Program =
       CompiledQuery.Compile<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, string, string, IQueryable<InputOverview>>(
           (ctx, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo, filterDepartmentName, filterFacilityNo) =>
               ctx
               .ProdOrderPartslist


               .Where(c =>
                   (filterProdStartDate == null || c.StartDate >= filterProdStartDate)
                   && (filterProdEndDate == null || c.StartDate < filterProdEndDate)
                   && (filterStartBookingDate == null || c.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos).Where(x => x.InsertDate >= filterStartBookingDate).Any())
                   && (filterEndBookingDate == null || c.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos).Where(x => x.InsertDate < filterEndBookingDate).Any())
                   && (string.IsNullOrEmpty(filterProgramNo) || c.ProdOrder.ProgramNo.Contains(filterProgramNo))
                   && (string.IsNullOrEmpty(filterMaterialNo) || c.Partslist.Material.MaterialNo.Contains(filterMaterialNo) || c.Partslist.Material.MaterialName1.Contains(filterMaterialNo))
                   && (string.IsNullOrEmpty(filterDepartmentName) || c.DepartmentUserName.Contains(filterDepartmentName))
               )

                .SelectMany(c => c.ProdOrderPartslistPos_ProdOrderPartslist)
                .Where(c =>
                        c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
                        && (string.IsNullOrEmpty(filterFacilityNo) || c.FacilityBooking_ProdOrderPartslistPos.Where(x => x.OutwardFacility != null && x.OutwardFacility.FacilityNo == filterFacilityNo).Any())
                )

                .GroupBy(c => new { c.Material.MaterialNo, c.Material.MaterialName1 })

               .Select(c => new InputOverview()
               {
                   // General
                   MaterialNo = c.Key.MaterialNo,
                   MaterialName = c.Key.MaterialName1,
                   MDUnitName = c.Where(x => x.MDUnit != null).Select(x => x.MDUnit.TechnicalSymbol).DefaultIfEmpty().FirstOrDefault(),

                   // Quantites
                   PlannedQuantityUOM =
                                   c.Select(x => x.TargetQuantityUOM)
                                   .DefaultIfEmpty()
                                   .Sum(),

                   TargetQuantityUOM =
                                   c.SelectMany(x => x.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                                   .Select(x => x.TargetQuantityUOM)
                                   .DefaultIfEmpty()
                                   .Sum(),

                   // Output
                   ActualQuantityUOM =
                                    c.SelectMany(x => x.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                                    .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                                   .Select(x => x.OutwardQuantity)
                                   .DefaultIfEmpty()
                                   .Sum(),

                   ZeroPostingQuantityUOM = 0


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
        public List<ProdOrderPartslistiOverview> OverviewProdOrderPartslist { get; set; }
        public List<ProdOrderPartslistiOverview> OverviewMaterial { get; set; }
        public List<InputOverview> InputOverview { get; set; }
    }
}
