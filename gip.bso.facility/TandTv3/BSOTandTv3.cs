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
using TandTv3 = gip.mes.facility.TandTv3;

namespace gip.bso.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Tracking and Tracing'}de{'Verfolgung/Rückverfolgung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, "")]
    [ACQueryInfo(Const.PackName_VarioFacility, Const.QueryPrefix + "BSOTandTv3", "en{'Tracking and Tracing'}de{'Verfolgung/Rückverfolgung'}", typeof(TandTv3FilterTracking), TandTv3FilterTracking.ClassName, "ItemSystemNo", "StartTime")]
    [ACClassConstructorInfo(
       new object[]
       {
            new object[] { "TrackingFilter", Global.ParamOption.Optional, typeof(TandTv3FilterTracking) }
       }
   )]
    public class BSOTandTv3 : ACBSOvbNav, IFactoryTandTPointPresenterComponent
    {
        #region constants
        public const string BGWorkerMehtod_DoSearch = @"DoSearch";
        public const string BGWorkerMehtod_DoSelect = @"DoSelect";
        public const string BGWorkerMehtod_DoDelete = @"DoDelete";
        public const string BGWorkerMehtod_DoDeleteCache = @"DoDeleteCache";
        public const string BGWorkerMehtod_DoRewrawGraph = @"DoRewrawGraph";
        public const bool BuildGraphic = false;
        #endregion

        #region Properties
        public TandTv3.TandTResult Result { get; set; }

        public Guid ActualGraphJobID { get; set; }

        private IACObject _GraphItem;
        [ACPropertyInfo(9999, "CurrentTandTPoint", "en{'CurrentTandTPoint'}de{'CurrentTandTPoint'}")]

        public IACObject GraphItem
        {
            get
            {
                return _GraphItem;
            }
            set
            {
                if (_GraphItem != value)
                {
                    _GraphItem = value;
                    OnPropertyChanged("GraphItem");
                }
            }
        }


        private GraphAction _SelectedGraphAction;
        [ACPropertyInfo(9999, "SelectedGraphAction", "en{'SelectedGraphAction'}de{'SelectedGraphAction'}")]
        public GraphAction SelectedGraphAction
        {
            get
            {
                return _SelectedGraphAction;
            }
            set
            {
                _SelectedGraphAction = value;
                OnPropertyChanged("SelectedGraphAction");
            }
        }

        #endregion

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOTandTv3(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            TandTPointPresenterClass = Root.Database.ContextIPlus.GetACType(typeof(TandTPointPresenter));
            DummyClass = Root.Database.ContextIPlus.GetACType(Const.UnknownClass);
            AllGeneratedTandTPointPresenterComponents = new List<TandTPointPresenter>();
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


            _TandTv3Manager = gip.mes.facility.TandTv3Manager.ACRefToServiceInstance(this);
            if (_TandTv3Manager == null)
                throw new Exception("TandTv3Manager not configured");

            _RoutingService = ACRoutingService.ACRefToServiceInstance(this);

            GraphCommand = new GraphCommand(DatabaseApp, this, RoutingService);

            // Warmup for components in graph
            // TandTv3Manager.CallFactoryComponents(this);

            AccessPrimary.NavSearch(DatabaseApp);
            AccessPrimary.Selected = ParameterValue(gip.mes.facility.TandTv3Manager.SearchModel_ParamValueKey) as TandTv3FilterTracking;
            if (SelectedFilter != null)
            {
                if (!SelectedFilter.IsReport)
                {
                    WriteFilter(SelectedFilter);
                    SelectedGraphAction = GraphAction.StartGraphProgress;
                    BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoSearch);
                    ShowDialog(this, DesignNameProgressBar);
                }
            }
            else
            {
                SelectedFilter = FilterList.FirstOrDefault();
            }
            ActiveTandTObjects = new List<IACObject>();
            ActiveTandTPaths = new List<IACObject>();
            InitSelectionManger(Const.SelectionManagerCDesign_ClassName);


            //Test();
            return true;
        }


        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            DoItemsClear();

            gip.mes.facility.TandTv3Manager.DetachACRefFromServiceInstance(this, _TandTv3Manager);
            _TandTv3Manager = null;

            Result = null;
            _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

            ACRoutingService.DetachACRefFromServiceInstance(this, _RoutingService);
            _RoutingService = null;

            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region BSO -> Clone

        public override object Clone()
        {
            ACValueList parameter = new ACValueList();
            TandTv3FilterTracking filterTracking = DatabaseApp.TandTv3FilterTracking.FirstOrDefault(c => c.TandTv3FilterTrackingID == SelectedFilter.TandTv3FilterTrackingID);
            filterTracking.IsReport = true;
            parameter.Add(new ACValue(gip.mes.facility.TandTv3Manager.SearchModel_ParamValueKey, filterTracking));
            BSOTandTv3 clone = FactoryTandTBSO(parameter);
            CloneJob(clone);
            return clone;
        }

        public virtual BSOTandTv3 FactoryTandTBSO(ACValueList acValueList)
        {
            return (ParentACComponent as ACComponent).StartComponent(this.ACType as core.datamodel.ACClass, this.Content, acValueList, Global.ACStartTypes.Automatic, IsProxy) as BSOTandTv3;
        }

        public virtual void CloneJob(BSOTandTv3 clone)
        {
            clone.Result = Result;
        }

        #endregion

        #region BSO overrides

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "DeleteAllCacheDlg":
                    DeleteAllCacheDlg();
                    return true;
                case "IsEnabledDeleteAllCacheDlg":
                    result = IsEnabledDeleteAllCacheDlg();
                    return true;
                case "SwitchDisplayType":
                    SwitchDisplayType();
                    return true;
                case "IsEnabledSwitchDisplayType":
                    result = IsEnabledSwitchDisplayType();
                    return true;
                case "Save":
                    Save();
                    return true;
                case "IsEnabledSave":
                    result = IsEnabledSave();
                    return true;
                case "Filter":
                    Filter();
                    return true;
                case "IsEnabledFilter":
                    result = IsEnabledFilter();
                    return true;
                case "SearchFilter":
                    SearchFilter();
                    return true;
                case "IsEnabledSearchFilter":
                    result = IsEnabledSearchFilter();
                    return true;
                case "Search":
                    Search();
                    return true;
                case "IsEnabledSearch":
                    result = IsEnabledSearch();
                    return true;
                case "Delete":
                    Delete();
                    return true;
                case "IsEnabledDelete":
                    result = IsEnabledDelete();
                    return true;
                case "Load":
                    Load(acParameter != null && acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case "IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
                case "ShowDetails":
                    ShowDetails((gip.bso.facility.TandTPointPresenter)acParameter[0]);
                    return true;
                case "RecalcEdgesRoute":
                    RecalcEdgesRoute();
                    return true;
                case "ShowLaborInfoForDn":
                    ShowLaborInfoForDn();
                    return true;
                case "IsEnabledShowLaborInfoForDn":
                    result = IsEnabledShowLaborInfoForDn();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region Managers

        protected ACRef<ITandTv3Manager> _TandTv3Manager = null;
        protected ITandTv3Manager TandTv3Manager
        {
            get
            {
                if (_TandTv3Manager == null)
                    return null;
                return _TandTv3Manager.ValueT;
            }
        }

        #region Managers -> SelectionManager
        private ACRef<VBBSOSelectionManager> _SelectionManager;
        public VBBSOSelectionManager SelectionManager
        {
            get
            {
                if (_SelectionManager != null)
                    return _SelectionManager.ValueT;
                return null;
            }
        }

        private bool _IsSelectionManagerInitialized = false;

        private void InitSelectionManger(string acIdentifier)
        {
            if (_IsSelectionManagerInitialized || SelectionManager != null)
                return;

            var childComp = GetChildComponent(acIdentifier) as VBBSOSelectionManager;
            if (childComp == null)
                childComp = StartComponent(acIdentifier, this, null) as VBBSOSelectionManager;

            if (childComp == null)
                return;

            _SelectionManager = new ACRef<VBBSOSelectionManager>(childComp, this);

            SelectionManager.PropertyChanged += SelectionManager_PropertyChanged;

            _IsSelectionManagerInitialized = true;
        }

        private void SelectionManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            VBBSOSelectionManager selectionManager = sender as VBBSOSelectionManager;
            if (e.PropertyName == "SelectedACObject")
                if (selectionManager.SelectedACObject != null)
                {
                    if (selectionManager.SelectedACObject is TandTEdge)
                    {
                        var activePaths = ActiveTandTPaths.ToList();
                        if (!activePaths.Contains(selectionManager.SelectedACObject))
                            activePaths.Add(selectionManager.SelectedACObject);
                        else
                            activePaths.Remove(selectionManager.SelectedACObject);
                        ActiveTandTPaths = activePaths;
                    }
                    else if (selectionManager.SelectedACObject is IACObject)
                    {
                        var activeObjects = ActiveTandTPaths.ToList();
                        if (!activeObjects.Contains(selectionManager.SelectedACObject))
                            activeObjects.Add(selectionManager.SelectedACObject);
                        else
                            activeObjects.Remove(selectionManager.SelectedACObject);
                        ActiveTandTObjects = activeObjects;
                    }
                }
                else
                {
                    ActiveTandTObjects = new List<IACObject>();
                    ActiveTandTObjects = new List<IACObject>();
                }

            //Console.WriteLine(string.Format(@"SelectionManager_PropertyChanged:{0}", e.PropertyName));
            //Console.WriteLine((sender as VBBSOSelectionManager).SelectedACObject.ToString());
            //Console.WriteLine("-----------------------");
        }
        #endregion

        public GraphCommand GraphCommand { get; private set; }

        #endregion

        #region IACObject overrides

        #endregion

        #region IFactoryTandTPointPresenterComponent

        public gip.core.datamodel.ACClass TandTPointPresenterClass { get; set; }
        public gip.core.datamodel.ACClass DummyClass { get; set; }

        public List<TandTPointPresenter> AllGeneratedTandTPointPresenterComponents { get; set; }

        //TODO: clean up this method
        public static int ComponentNo = 0;
        public TandTPointPresenter FactoryComponent(Guid jobID, MDTrackingStartItemTypeEnum itemType, TandTv3Point mixPoint, Guid? representMixPointID, Guid acObjectID, IACObject content)
        {
            TandTPointPresenter component =
                AllGeneratedTandTPointPresenterComponents
                .FirstOrDefault(c =>
                    c.ACObjectID == acObjectID
                    && c.JobIds.Contains(jobID));
            if (component == null)
            {
                component = new TandTPointPresenter(content, this) { ACIdentifier = "TandTPP" + ComponentNo };
                ComponentNo++;

                lock (AllGeneratedTandTPointPresenterComponents)
                {
                    AllGeneratedTandTPointPresenterComponents.Add(component);
                }
                component.ItemType = itemType.ToString();
                if (mixPoint != null)
                    component.MixPointIds.Add(mixPoint.MixPointID);
                component.RepresentMixPointID = representMixPointID;
                component.ACObjectID = acObjectID;
            }
            if (!component.JobIds.Contains(jobID))
            {
                lock (component.JobIds)
                {
                    component.JobIds.Add(jobID);
                }
            }
            return component;
        }

        #endregion

        #region TabExplorer

        #region  TabExplorer -> SelectedFilter

        #region TabExplorer -> SelectedFilter -> Primary Navigation

        #region TabExplorer -> SelectedFilter-> BSO -> ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<TandTv3FilterTracking> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, TandTv3FilterTracking.ClassName)]
        public ACAccessNav<TandTv3FilterTracking> AccessPrimary
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
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<TandTv3FilterTracking>(TandTv3FilterTracking.ClassName, this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessPrimary;
            }
        }

        IQueryable<TandTv3FilterTracking> _AccessPrimary_NavSearchExecuting(IQueryable<TandTv3FilterTracking> result)
        {
            ObjectQuery<TandTv3FilterTracking> query = result as ObjectQuery<TandTv3FilterTracking>;
            if (query != null)
            {
                query.Include(c => c.TandTv3MDTrackingDirection);
                query.Include(c => c.TandTv3MDTrackingStartItemType);
            }
            return result;
        }

        protected virtual List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "ItemSystemNo", Global.LogicalOperators.contains, Global.Operators.or, null, true, true)
                };
            }
        }

        protected virtual List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("StartTime", Global.SortDirections.descending, true)
                };
            }
        }
        #endregion

        /// <summary>
        /// job changing active
        /// </summary>

        /// <summary>
        /// Selected property for TandTv3Job
        /// </summary>
        /// <value>The selected Job</value>
        [ACPropertySelected(9999, TandTv3FilterTracking.ClassName, "en{'TODO: Job'}de{'TODO: Job'}")]
        public TandTv3FilterTracking SelectedFilter
        {
            get
            {
                if (AccessPrimary != null && AccessPrimary.Selected != null)
                    AccessPrimary.Selected.AggregateOrderData = true;
                return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary.Selected != value)
                {
                    if (value != null)
                        value.AggregateOrderData = true;
                    AccessPrimary.Selected = value;
                    OnPropertyChanged("SelectedFilter");
                    if (value != null && !BackgroundWorker.IsBusy && FilterSearchNo != value.ItemSystemNo)
                        SelectedFilter_Changed();
                }
            }
        }

        /// <summary>
        /// List property for TandTv3Job
        /// </summary>
        /// <value>The Job list</value>
        [ACPropertyList(9999, TandTv3FilterTracking.ClassName)]
        public IEnumerable<TandTv3FilterTracking> FilterList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        public virtual void SelectedFilter_Changed()
        {
            WriteFilter(SelectedFilter);
            SelectedGraphAction = GraphAction.StartGraphProgress;
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoSelect);
            ShowDialog(this, DesignNameProgressBar);
        }

        #endregion

        #endregion

        [ACMethodInfo(TandTv3FilterTracking.ClassName, "en{'Delete all results'}de{'Lösche alle Ergebnisse'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public void DeleteAllCacheDlg()
        {
            MsgResult msgResult = Messages.Question(this, "Question50042");
            if (msgResult == MsgResult.Yes)
            {
                SelectedGraphAction = GraphAction.StartGraphProgress;
                BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoDeleteCache);
                ShowDialog(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledDeleteAllCacheDlg()
        {
            return FilterList != null && FilterList.Any();
        }

        #endregion

        #region Messages

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged("MsgList");
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
                OnPropertyChanged("CurrentMsg");
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

        #region Filter

        #region Filter -> TrackingStyle

        ACValueItem _SelectedTrackingStyle;
        [ACPropertySelected(9999, "TrackingStyle", "en{'Direction'}de{'Richtung'}")]
        public ACValueItem SelectedTrackingStyle
        {
            get
            {
                if (_SelectedTrackingStyle == null)
                {
                    _SelectedTrackingStyle = TrackingStyleList.FirstOrDefault(x => ((short)x.Value) == (short)MDTrackingDirectionEnum.Backward);
                }
                return _SelectedTrackingStyle;
            }
            set
            {
                if (_SelectedTrackingStyle != value)
                {
                    _SelectedTrackingStyle = value;
                    OnPropertyChanged("SelectedTrackingStyle");
                }
            }
        }

        public MDTrackingDirectionEnum FilterTrackingStyle
        {
            get
            {
                return (MDTrackingDirectionEnum)(short)SelectedTrackingStyle.Value;
            }
            set
            {
                SelectedTrackingStyle = TrackingStyleList.FirstOrDefault(x => ((short)x.Value) == (short)value);
            }
        }

        private ACValueItemList _TrackingStyleList;
        [ACPropertyList(9999, "TrackingStyle")]
        public IEnumerable<ACValueItem> TrackingStyleList
        {
            get
            {
                if (_TrackingStyleList == null)
                {
                    _TrackingStyleList = new ACValueItemList("TrackingStyleList");
                    _TrackingStyleList.AddEntry((short)MDTrackingDirectionEnum.Backward, ConstApp.Backward);
                    _TrackingStyleList.AddEntry((short)MDTrackingDirectionEnum.Forward, ConstApp.Forward);
                }
                return _TrackingStyleList;
            }
        }

        #endregion

        #region Filter -> ItemType

        ACValueItem _SelectedItemType;
        [ACPropertySelected(9999, "ItemType", "en{'Tracing Item'}de{'Rückverfolgende Element'}")]
        public ACValueItem SelectedItemType
        {
            get
            {
                return _SelectedItemType;
            }
            set
            {
                if (_SelectedItemType != value)
                {
                    _SelectedItemType = value;
                    OnPropertyChanged("SelectedItemType");
                }
            }
        }

        public MDTrackingStartItemTypeEnum FilterItemType
        {
            get
            {
                if (SelectedItemType == null) return MDTrackingStartItemTypeEnum.FacilityBooking;
                return (MDTrackingStartItemTypeEnum)Enum.Parse(typeof(MDTrackingStartItemTypeEnum), SelectedItemType.Value.ToString());
            }
        }

        private ACValueItemList _ItemTypeList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        [ACPropertyList(9999, "ItemType")]
        public IEnumerable<ACValueItem> ItemTypeList
        {
            get
            {
                if (_ItemTypeList == null)
                {
                    _ItemTypeList = new ACValueItemList("ItemTypeList");
                    var query =
                        DatabaseApp
                        .TandTv3MDTrackingStartItemType
                        .GroupJoin(DatabaseApp.ACClass, it => it.TandTv3MDTrackingStartItemTypeID, acl => acl.ACIdentifier, (it, acl) => new { ID = it.TandTv3MDTrackingStartItemTypeID, Translation = acl.Select(c => c.ACCaptionTranslation).FirstOrDefault() });
                    foreach (var item in query)
                        _ItemTypeList.AddEntry(item.ID, item.Translation);
                }
                return _ItemTypeList;
            }
        }

        #endregion

        #region Filter -> Fields

        private string _FilterSearchNo;
        [ACPropertyInfo(9999, "FilterSearchNo", "en{'Number'}de{'Nummer'}")]
        public string FilterSearchNo
        {
            get
            {
                return _FilterSearchNo;
            }
            set
            {
                if (_FilterSearchNo != value)
                {
                    _FilterSearchNo = value;
                    OnPropertyChanged("FilterSearchNo");
                }
            }
        }

        private DateTime? _FilterDateFrom;
        [ACPropertyInfo(9999, "FilterDateFrom", Const.From)]
        public DateTime? FilterDateFrom
        {
            get
            {
                return _FilterDateFrom;
            }
            set
            {
                if (_FilterDateFrom != value)
                {
                    _FilterDateFrom = value;
                    OnPropertyChanged("FilterDateFrom");
                }
            }
        }

        private DateTime? _FilterDateTo;
        [ACPropertyInfo(9999, "FilterDateTo", Const.To)]
        public DateTime? FilterDateTo
        {
            get
            {
                return _FilterDateTo;
            }
            set
            {
                if (_FilterDateTo != value)
                {
                    _FilterDateTo = value;
                    OnPropertyChanged("FilterDateTo");
                }
            }
        }

        private bool _FilterRecalcAgain;
        [ACPropertyInfo(9999, "FilterRecalcAgain", "en{'Recalc'}de{'Recalc'}")]
        public bool FilterRecalcAgain
        {
            get
            {
                return _FilterRecalcAgain;
            }
            set
            {
                if (_FilterRecalcAgain != value)
                {
                    _FilterRecalcAgain = value;
                    OnPropertyChanged("FilterRecalcAgain");
                }
            }
        }

        #endregion

        #region Filter -> DisplayType

        private ACValueItemList _DisplayTypeTitle = null;
        public IEnumerable<ACValueItem> DisplayTypeTitle
        {
            get
            {
                if (_DisplayTypeTitle == null)
                {
                    _DisplayTypeTitle = new ACValueItemList("DisplayTypeTitle");
                    _DisplayTypeTitle.AddEntry(TandTv3.Model.DisplayGroupEnum.Storage, "en{'Storage places and container'}de{'Lagerplätze und Behältnisse'}");
                    // _DisplayTypeTitle.AddEntry(DisplayGroupEnum.Quants, "en{'Facilitycharge'}de{'Chargenplatz'}");
                    // _DisplayTypeTitle.AddEntry(DisplayGroupEnum.Lots, "en{'Lot'}de{'Los'}");
                    _DisplayTypeTitle.AddEntry(TandTv3.Model.DisplayGroupEnum.Orders, "en{'Receival/Issue/Production'}de{'Eingang/Ausgang/Produktion'}");
                    // _DisplayTypeTitle.AddEntry(DisplayGroupEnum.Bookings, @"en{'Stock movement /of quantum'}de{'Lagerbewegung /Quant'}");
                    _DisplayTypeTitle.AddEntry(TandTv3.Model.DisplayGroupEnum.Machines, "en{'Machines and work centers'}de{'Maschinen und Arbeitsplätze'}");
                    _DisplayTypeTitle.AddEntry(TandTv3.Model.DisplayGroupEnum.MixPointGroup, "en{'Compressed view'}de{'Komprimierte Darstellung'}");
                    _DisplayTypeTitle.AddEntry(TandTv3.Model.DisplayGroupEnum.MixPoint, "en{'Detail view'}de{'Einzeldarstellung'}");
                    _DisplayTypeTitle.AddEntry(TandTv3.Model.DisplayGroupEnum.Material, "en{'Used and manufactured'}de{'Eingesetzt und Hergestellt'}");
                }
                return _DisplayTypeTitle;
            }
        }

        private ACValueItemList _DisplayTypeSubTitle = null;
        public IEnumerable<ACValueItem> DisplayTypeSubTitle
        {
            get
            {
                if (_DisplayTypeSubTitle == null)
                {
                    _DisplayTypeSubTitle = new ACValueItemList("DisplayTypeSubTitle");
                    _DisplayTypeSubTitle.AddEntry(TandTv3.Model.DisplayGroupEnum.Storage, "en{'Warehouse'}de{'Lager'}");
                    // _DisplayTypeSubTitle.AddEntry(DisplayGroupEnum.Quants, "en{'Quants'}de{'Quanten'}");
                    // _DisplayTypeSubTitle.AddEntry(DisplayGroupEnum.Lots, "en{'Lots'}de{'Lots'}");
                    _DisplayTypeSubTitle.AddEntry(TandTv3.Model.DisplayGroupEnum.Orders, "en{'Orders'}de{'Aufträge'}");
                    //_DisplayTypeSubTitle.AddEntry(DisplayGroupEnum.Bookings, "en{'Bookings'}de{'Buchungen'}");
                    _DisplayTypeSubTitle.AddEntry(TandTv3.Model.DisplayGroupEnum.Machines, "en{'Machines'}de{'Maschinen'}");
                    _DisplayTypeSubTitle.AddEntry(TandTv3.Model.DisplayGroupEnum.MixPointGroup, "en{'Lot/Batch'}de{'Los/Charge'}");
                    _DisplayTypeSubTitle.AddEntry(TandTv3.Model.DisplayGroupEnum.MixPoint, "en{'Lots/Batches'}de{'Lose/Chargen'}");
                    _DisplayTypeSubTitle.AddEntry(TandTv3.Model.DisplayGroupEnum.Material, "en{'Materials'}de{'Materialien'}");
                }
                return _DisplayTypeSubTitle;
            }
        }

        private TrackingDisplayItemType _SelectedDisplayType;
        [ACPropertySelected(9999, "DisplayType", "en{'DisplayType'}de{'DisplayType'}")]
        public TrackingDisplayItemType SelectedDisplayType
        {
            get
            {
                return _SelectedDisplayType;
            }
            set
            {
                if (_SelectedDisplayType != value)
                {
                    _SelectedDisplayType = value;
                    OnPropertyChanged("SelectedDisplayType");
                }
            }
        }

        private List<TrackingDisplayItemType> _DisplayTypeList;
        [ACPropertyList(9999, "DisplayType")]
        public List<TrackingDisplayItemType> DisplayTypeList
        {
            get
            {
                if (_DisplayTypeList == null)
                    _DisplayTypeList = LoadDisplayTypeList();
                return _DisplayTypeList;
            }
        }

        private List<TrackingDisplayItemType> LoadDisplayTypeList()
        {
            List<TrackingDisplayItemType> result = new List<TrackingDisplayItemType>();
            result = DisplayTypeTitle
                .Join(DisplayTypeSubTitle, st => st.Value, tit => tit.Value, (st, tit) => new { Title = tit, SubTitle = st })
                .Select(c =>
                new TrackingDisplayItemType()
                {
                    Title = c.Title.ACCaption,
                    SubTitle = c.SubTitle.ACCaption,
                    IsIncluded = new List<TandTv3.Model.DisplayGroupEnum>()
                        {
                            TandTv3.Model.DisplayGroupEnum.MixPointGroup,
                            TandTv3.Model.DisplayGroupEnum.Storage
                        }.Contains((TandTv3.Model.DisplayGroupEnum)c.Title.Value),
                    ItemType = (TandTv3.Model.DisplayGroupEnum)c.Title.Value
                })
                .ToList();
            return result;
        }

        [ACMethodInfo("SwitchDisplayType", "en{'SwitchDisplayType'}de{'Berechnen und Aktualisieren'}", 9999, false)]
        public void SwitchDisplayType()
        {
            if (!IsEnabledSwitchDisplayType()) return;
            SelectedDisplayType.IsIncluded = !SelectedDisplayType.IsIncluded;
            if (SelectedDisplayType.ItemType == TandTv3.Model.DisplayGroupEnum.Material && SelectedDisplayType.IsIncluded)
            {
                DisplayTypeList.Where(c => c.ItemType == TandTv3.Model.DisplayGroupEnum.MixPoint).ToList().ForEach(x => x.IsIncluded = false);
                DisplayTypeList.Where(c => c.ItemType == TandTv3.Model.DisplayGroupEnum.MixPointGroup).ToList().ForEach(x => x.IsIncluded = false);
                DisplayTypeList.Where(c => c.ItemType == TandTv3.Model.DisplayGroupEnum.Storage).ToList().ForEach(x => x.IsIncluded = false);
            }
            else
            {
                bool mixPointIncluded = SelectedDisplayType.ItemType == TandTv3.Model.DisplayGroupEnum.MixPoint && SelectedDisplayType.IsIncluded;
                bool mixPointGroupIncluded = SelectedDisplayType.ItemType == TandTv3.Model.DisplayGroupEnum.MixPointGroup && SelectedDisplayType.IsIncluded;
                if (mixPointIncluded || mixPointGroupIncluded)
                {
                    DisplayTypeList.Where(c => c.ItemType == TandTv3.Model.DisplayGroupEnum.MixPoint).ToList().ForEach(x => x.IsIncluded = mixPointIncluded);
                    DisplayTypeList.Where(c => c.ItemType == TandTv3.Model.DisplayGroupEnum.MixPointGroup).ToList().ForEach(x => x.IsIncluded = mixPointGroupIncluded);
                }
            }
        }

        public bool IsEnabledSwitchDisplayType()
        {
            return SelectedDisplayType != null;
        }

        public List<TandTv3.Model.DisplayGroupEnum> IncludedGraphItems
        {
            get
            {
                return DisplayTypeList.Where(c => c.IsIncluded).Select(c => c.ItemType).ToList();
            }
        }

        /// <summary>
        /// Temp list for check is changed graph item
        /// </summary>
        public List<TandTv3.Model.DisplayGroupEnum> SavedIncludedDisplayTypeList { get; set; }


        public bool UseGroupResult
        {
            get
            {
                return DisplayTypeList.Where(c => c.IsIncluded).Select(c => c.ItemType).Contains(TandTv3.Model.DisplayGroupEnum.MixPointGroup);
            }
        }

        #endregion

        #region Filter -> Methods
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(TandTv3FilterTracking.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodInfo(TandTv3FilterTracking.ClassName, "en{'Filter'}de{'Filter'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void Filter()
        {
            if (!IsEnabledFilter()) return;
            SelectedGraphAction = GraphAction.StartGraphProgress;
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoRewrawGraph);
            //ShowWindow(this, DesignNameProgress1Bar, false,
            //  Global.VBDesignContainer.DockableWindow,
            //  Global.VBDesignDockState.FloatingWindow,
            //  Global.VBDesignDockPosition.Right);
        }

        public virtual bool IsEnabledFilter()
        {
            return !BackgroundWorker.IsBusy && Result != null &&
                Result.Success &&
                Result.MixPoints.Any() &&
                (
                    IncludedGraphItems.Except(SavedIncludedDisplayTypeList).Any() ||
                    SavedIncludedDisplayTypeList.Except(IncludedGraphItems).Any()
                );
        }

        [ACMethodInfo(TandTv3FilterTracking.ClassName, "en{'Search'}de{'Suchen'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void Search()
        {
            if (!IsEnabledSearch()) return;
            AccessPrimary.NavSearch();
            SelectedGraphAction = GraphAction.StartGraphProgress;
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoSearch);
            ShowDialog(this, DesignNameProgressBar);
        }
        public bool IsEnabledSearch()
        {
            return !BackgroundWorker.IsBusy && !string.IsNullOrEmpty(FilterSearchNo);
        }


        [ACMethodInfo(TandTv3FilterTracking.ClassName, "en{'Start tracking'}de{'Chargenverfolgung starten'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void SearchFilter()
        {
            if (!IsEnabledSearchFilter()) return;
            SelectedFilter = ReadFilter();
            SelectedGraphAction = GraphAction.StartGraphProgress;
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoSearch);
            ShowDialog(this, DesignNameProgressBar);
        }
        public bool IsEnabledSearchFilter()
        {
            return !BackgroundWorker.IsBusy && !string.IsNullOrEmpty(FilterSearchNo);
        }

        [ACMethodInteraction(Partslist.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedPartslist", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!IsEnabledLoad()) return;
            FilterRecalcAgain = true;
            SelectedFilter = ReadFilter();
            SelectedGraphAction = GraphAction.StartGraphProgress;
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoSearch);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledLoad()
        {
            return !BackgroundWorker.IsBusy && !string.IsNullOrEmpty(FilterSearchNo);
        }


        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(TandTv3FilterTracking.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "SelectedFilter", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!IsEnabledDelete()) return;
            SelectedGraphAction = GraphAction.StartGraphProgress;
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoDelete);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledDelete()
        {
            return SelectedFilter != null;
        }

        public void WriteFilter(TandTv3FilterTracking filter)
        {
            if (filter != null)
            {
                SelectedItemType = ItemTypeList.FirstOrDefault(c => c.Value.ToString() == filter.TandTv3MDTrackingStartItemTypeID.ToString());
                FilterSearchNo = filter.ItemSystemNo;
                SelectedTrackingStyle = TrackingStyleList.FirstOrDefault(c => ((MDTrackingDirectionEnum)Enum.Parse(typeof(MDTrackingDirectionEnum), c.Value.ToString())) == filter.MDTrackingDirectionEnum);
                FilterDateFrom = filter.FilterDateFrom;
                FilterDateTo = filter.FilterDateTo;
            }
            else
            {
                SelectedItemType = null;
                FilterSearchNo = null;
                SelectedTrackingStyle = null;
                FilterDateFrom = null;
                FilterDateTo = null;
            }
        }

        public List<MDTrackingStartItemTypeEnum> GetIncludedItemTypes()
        {
            List<MDTrackingStartItemTypeEnum> displayTypesForPrevew = new List<MDTrackingStartItemTypeEnum>();
            foreach (var item in DisplayTypeList)
            {
                if (item.IsIncluded)
                    displayTypesForPrevew.AddRange(TandTv3.CastDisplayGroupToItemTypeEnum.Cast(item.ItemType));
            }
            return displayTypesForPrevew;
        }

        private TandTv3FilterTracking ReadFilter()
        {
            TandTv3FilterTracking filter = new TandTv3FilterTracking();
            filter.MDTrackingStartItemTypeEnum = FilterItemType;
            switch (FilterItemType)
            {
                case MDTrackingStartItemTypeEnum.FacilityBooking:
                    FacilityBooking fb = DatabaseApp.FacilityBooking.FirstOrDefault(c => c.FacilityBookingNo == FilterSearchNo);
                    if (fb != null)
                    {
                        filter.ItemSystemNo = fb.FacilityBookingNo;
                        filter.PrimaryKeyID = fb.FacilityBookingID;
                    }
                    break;
                case MDTrackingStartItemTypeEnum.FacilityPreBooking:
                    FacilityPreBooking fbPre = DatabaseApp.FacilityPreBooking.FirstOrDefault(c => c.FacilityPreBookingNo == FilterSearchNo);
                    if (fbPre != null)
                    {
                        filter.ItemSystemNo = fbPre.FacilityPreBookingNo;
                        filter.PrimaryKeyID = fbPre.FacilityPreBookingID;
                    }
                    break;
                case MDTrackingStartItemTypeEnum.InOrderPosPreview:
                    break;
                case MDTrackingStartItemTypeEnum.OutOrderPosPreview:
                    break;
                case MDTrackingStartItemTypeEnum.DeliveryNotePos:
                    break;
                default:
                    break;
            }
            filter.MDTrackingDirectionEnum = FilterTrackingStyle;
            filter.FilterDateFrom = FilterDateFrom;
            filter.FilterDateTo = FilterDateTo;
            filter.RecalcAgain = FilterRecalcAgain;
            return filter;
        }

        private void WriteNavFilter(ACQueryDefinition navACQueryDefinition, TandTv3FilterTracking filter)
        {
            navACQueryDefinition.FactoryACFilterItem("ItemTypeID", filter != null ? filter.TandTv3MDTrackingStartItemTypeID : "");
            navACQueryDefinition.FactoryACFilterItem("ItemSystemNo", filter != null ? filter.ItemSystemNo : "");
            navACQueryDefinition.FactoryACFilterItem("TrackingStyleID", filter != null ? filter.TandTv3MDTrackingDirectionID : "");

            navACQueryDefinition.FactoryACFilterItem("FilterDateFrom", (filter != null && filter.FilterDateFrom != null) ? filter.FilterDateFrom.ToString() : "");
            navACQueryDefinition.FactoryACFilterItem("FilterDateTo", (filter != null && filter.FilterDateTo != null) ? filter.FilterDateTo.ToString() : "");
        }

        #endregion

        #endregion

        #region TabData

        #region TabData -> SelectedDeliveryNote


        private DeliveryNotePosPreview _SelectedDeliveryNote;
        /// <summary>
        /// Selected property for DeliveryNote
        /// </summary>
        /// <value>The selected DeliveryNote</value>
        [ACPropertySelected(9999, "DeliveryNote", "en{'TODO: DeliveryNote'}de{'TODO: DeliveryNote'}")]
        public DeliveryNotePosPreview SelectedDeliveryNote
        {
            get
            {
                return _SelectedDeliveryNote;
            }
            set
            {
                if (_SelectedDeliveryNote != value)
                {
                    _SelectedDeliveryNote = value;
                    OnPropertyChanged("SelectedDeliveryNote");
                }
            }
        }


        /// <summary>
        /// List property for DeliveryNote
        /// </summary>
        /// <value>The DeliveryNote list</value>
        [ACPropertyList(9999, "DeliveryNote")]
        public List<DeliveryNotePosPreview> DeliveryNoteList
        {
            get
            {
                if (Result == null) return null;
                return Result.DeliveryNotes;
            }
        }

        #endregion

        #region TabData -> SelectedFacilityCharge

        private FacilityChargeModel _SelectedFacilityCharge;
        /// <summary>
        /// Selected property for FacilityCharge
        /// </summary>
        /// <value>The selected FacilityCharge</value>
        [ACPropertySelected(9999, "FacilityCharge", "en{'TODO: FacilityCharge'}de{'TODO: FacilityCharge'}")]
        public FacilityChargeModel SelectedFacilityCharge
        {
            get
            {
                return _SelectedFacilityCharge;
            }
            set
            {
                if (_SelectedFacilityCharge != value)
                {
                    _SelectedFacilityCharge = value;
                    OnPropertyChanged("SelectedFacilityCharge");
                }
            }
        }

        /// <summary>
        /// List property for FacilityCharge
        /// </summary>
        /// <value>The FacilityCharge list</value>
        [ACPropertyList(9999, "FacilityCharge")]
        public List<FacilityChargeModel> FacilityChargeList
        {
            get
            {
                if (Result == null) return null;
                return Result.FacilityCharges;
            }
        }

        #endregion

        #region TabData -> TandTMixPoint

        private TandTv3Point _SelectedTandTMixPoint;
        /// <summary>
        /// Selected property for TandTv3StepItem
        /// </summary>
        /// <value>The selected StepItem</value>
        [ACPropertySelected(9999, "TandTMixPoint", "en{'TODO: StepItem'}de{'TODO: StepItem'}")]
        public TandTv3Point SelectedTandTMixPoint
        {
            get
            {
                return _SelectedTandTMixPoint;
            }
            set
            {
                if (_SelectedTandTMixPoint != value)
                {
                    _SelectedTandTMixPoint = value;
                    if (SelectedTandTMixPoint != null)
                        if (SelectedTandTMixPoint.ExistLabOrder)
                            SelectedACObjectWithLabOrder = SelectedTandTMixPoint.ItemsWithLabOrder.FirstOrDefault();
                    OnPropertyChanged("SelectedTandTMixPoint");
                    OnPropertyChanged("ACObjectWithLabOrderList");
                }
            }
        }

        /// <summary>
        /// List property for TandTv3StepItem
        /// </summary>
        /// <value>The StepItem list</value>
        [ACPropertyList(9999, "TandTMixPoint")]
        public List<TandTv3Point> TandTMixPointList
        {
            get
            {
                if (Result == null) return null;
                return Result.MixPoints;
            }
        }

        #endregion

        #endregion

        #region Graphic route preview
        private List<List<TandTPath>> _AvailableTandTEdges;
        [ACPropertyInfo(999)]
        public List<List<TandTPath>> AvailableTandTEdges
        {
            get
            {
                return _AvailableTandTEdges;
            }
            set
            {
                _AvailableTandTEdges = value;
                OnPropertyChanged("AvailableTandTEdges");
            }
        }

        public IACObject SelectedTandTPointPresenter
        {
            get;
            set;
        }

        private List<IACObject> _ActiveTandTObjects;
        [ACPropertyInfo(999)]
        public List<IACObject> ActiveTandTObjects
        {
            get { return _ActiveTandTObjects; }
            set
            {
                _ActiveTandTObjects = value;
                OnPropertyChanged("ActiveTandTObjects");
            }
        }

        private List<IACObject> _ActiveTandTPaths;
        [ACPropertyInfo(999)]
        public List<IACObject> ActiveTandTPaths
        {
            get { return _ActiveTandTPaths; }
            set
            {
                _ActiveTandTPaths = value;
                OnPropertyChanged("ActiveTandTPaths");
            }
        }

        [ACMethodInfo("", "", 999)]
        public void ShowDetails(TandTPointPresenter pointPresenter)
        {
            GraphItem = pointPresenter.Content;
            string designName = pointPresenter.Content.GetType().Name + "_ControlDialog";
            ShowDialog(this, designName);
        }

        private bool _UseEdgesRouting = false;
        [ACPropertyInfo(999, "", "en{'Crossing avoidance'}de{'Kreuzungen minimieren'}")]
        public bool UseEdgesRouting
        {
            get => _UseEdgesRouting;
            set
            {
                _UseEdgesRouting = value;
                OnPropertyChanged("UseEdgesRouting");
            }
        }

        [ACMethodInfo("", "en{'Recalc. edges'}de{'Kanten neu berech.'}", 999)]
        public void RecalcEdgesRoute()
        {
            SelectedGraphAction = GraphAction.None;
            SelectedGraphAction = GraphAction.RecalcEdgesRoute;
        }

        #endregion

        #region TabLabOrder

        #region TabLabOrder -> ACObjectWithLabOrder

        #region ACObjectWithLabOrder
        private MixPointLabOrder _SelectedACObjectWithLabOrder;
        /// <summary>
        /// Selected property for IACObject
        /// </summary>
        /// <value>The selected ACObjectWithLabOrder</value>
        [ACPropertySelected(9999, "ACObjectWithLabOrder", "en{'TODO: ACObjectWithLabOrder'}de{'TODO: ACObjectWithLabOrder'}")]
        public MixPointLabOrder SelectedACObjectWithLabOrder
        {
            get
            {
                return _SelectedACObjectWithLabOrder;
            }
            set
            {
                if (_SelectedACObjectWithLabOrder != value)
                {
                    _SelectedACObjectWithLabOrder = value;
                    if (SelectedACObjectWithLabOrder != null)
                        SelectedLabOrder = SelectedACObjectWithLabOrder.LabOrderWithItems.Keys.FirstOrDefault();
                    OnPropertyChanged("SelectedACObjectWithLabOrder");
                    OnPropertyChanged("LabOrderList");
                }
            }
        }

        /// <summary>
        /// List property for IACObject
        /// </summary>
        /// <value>The ACObjectWithLabOrder list</value>
        [ACPropertyList(9999, "ACObjectWithLabOrder")]
        public List<MixPointLabOrder> ACObjectWithLabOrderList
        {
            get
            {
                if (SelectedTandTMixPoint == null) return null;
                return _SelectedTandTMixPoint.ItemsWithLabOrder;
            }
        }

        #endregion

        #endregion

        #region TabLabOrder -> LabOrder

        private LabOrder _SelectedLabOrder;
        /// <summary>
        /// Selected property for LabOrder
        /// </summary>
        /// <value>The selected LabOrder</value>
        [ACPropertySelected(9999, "LabOrder", "en{'TODO: LabOrder'}de{'TODO: LabOrder'}")]
        public LabOrder SelectedLabOrder
        {
            get
            {
                return _SelectedLabOrder;
            }
            set
            {
                if (_SelectedLabOrder != value)
                {
                    _SelectedLabOrder = value;
                    if (SelectedLabOrder != null &&
                        SelectedACObjectWithLabOrder != null &&
                        SelectedACObjectWithLabOrder.LabOrderWithItems != null &&
                        SelectedACObjectWithLabOrder.LabOrderWithItems.Values.FirstOrDefault() != null)
                        SelectedLabOrderPos = SelectedACObjectWithLabOrder.LabOrderWithItems.Values.FirstOrDefault().FirstOrDefault();
                    OnPropertyChanged("SelectedLabOrder");
                    OnPropertyChanged("LabOrderPosList");
                }
            }
        }

        /// <summary>
        /// List property for LabOrder
        /// </summary>
        /// <value>The LabOrder list</value>
        [ACPropertyList(9999, "LabOrder")]
        public List<LabOrder> LabOrderList
        {
            get
            {
                if (SelectedTandTMixPoint == null || SelectedACObjectWithLabOrder == null) return null;
                return
                    SelectedTandTMixPoint
                    .ItemsWithLabOrder
                    .Where(c => c.ID == SelectedACObjectWithLabOrder.ID)
                    .SelectMany(c => c.LabOrderWithItems.Keys)
                    .ToList();
            }
        }

        [ACMethodInteraction("DeliveryNoteTypeList", "en{'Show LabOrder'}de{'Zeige Laborauftrag'}", 50, false)]
        public virtual void ShowLaborInfoForDn()
        {
            if (IsEnabledShowLaborInfoForDn())
            {
                //TandTv3StepItem stepItem = StepItemList.FirstOrDefault(c => c.PrimaryKeyID == SelectedDeliveryNote.DeliveryNotePosID);// DatabaseApp.DeliveryNotePos.Where(x => x.DeliveryNotePosID == SelectedDeliveryNote.DeliveryNotePosID).FirstOrDefault();
                //LoadLabOrderList(stepItem);
                //ShowDialog(this, "LabOrder");
            }
        }

        public bool IsEnabledShowLaborInfoForDn()
        {
            return SelectedDeliveryNote != null;
        }

        #endregion

        #region TabLabOrder -> LabOrderPos

        private LabOrderPos _SelectedLabOrderPos;
        /// <summary>
        /// Selected property for LabOrderPos
        /// </summary>
        /// <value>The selected LabOrderPos</value>
        [ACPropertySelected(9999, "LabOrderPos", "en{'TODO: LabOrderPos'}de{'TODO: LabOrderPos'}")]
        public LabOrderPos SelectedLabOrderPos
        {
            get
            {
                return _SelectedLabOrderPos;
            }
            set
            {
                if (_SelectedLabOrderPos != value)
                {
                    _SelectedLabOrderPos = value;
                    OnPropertyChanged("SelectedLabOrderPos");
                }
            }
        }

        /// <summary>
        /// List property for LabOrderPos
        /// </summary>
        /// <value>The LabOrderPos list</value>
        [ACPropertyList(9999, "LabOrderPos")]
        public List<LabOrderPos> LabOrderPosList
        {
            get
            {
                if (SelectedTandTMixPoint == null || SelectedACObjectWithLabOrder == null || SelectedLabOrder == null) return null;
                return
                    SelectedTandTMixPoint
                    .ItemsWithLabOrder
                    .Where(c => c.ID == SelectedACObjectWithLabOrder.ID)
                    .SelectMany(c => c.LabOrderWithItems)
                    .Where(c => c.Key.LabOrderID == SelectedLabOrder.LabOrderID)
                    .SelectMany(c => c.Value)
                    .ToList();
            }
        }

        #endregion

        #endregion

        #region BackgroundWorker

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
            switch (command)
            {
                case BGWorkerMehtod_DoSearch:
                    e.Result = DoSearchAsync(worker, e);
                    break;
                case BGWorkerMehtod_DoSelect:
                    e.Result = DoSelectAsync(worker, e);
                    break;
                case BGWorkerMehtod_DoDelete:
                    e.Result = DoDeleteAsync(worker, e);
                    break;
                case BGWorkerMehtod_DoDeleteCache:
                    e.Result = DoDeleteCacheAsync(worker, e);
                    break;
                case BGWorkerMehtod_DoRewrawGraph:
                    e.Result = DoRewrawGraph(worker, e);
                    break;
            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);
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
                    case BGWorkerMehtod_DoSearch:
                        DoSearchFinish(e.Result as TandTv3.TandTResult);
                        break;
                    case BGWorkerMehtod_DoSelect:
                        DoSearchFinish(e.Result as TandTv3.TandTResult);
                        break;
                    case BGWorkerMehtod_DoDelete:
                        KeyValuePair<TandTv3.TandTResult, MsgWithDetails> deleteResult = new KeyValuePair<TandTv3.TandTResult, MsgWithDetails>();
                        if (e.Result != null)
                        {
                            deleteResult = (KeyValuePair<TandTv3.TandTResult, MsgWithDetails>)e.Result;
                            DoDeleteFinish(deleteResult);
                        }
                        break;
                    case BGWorkerMehtod_DoDeleteCache:
                        bool isDeleteCacheSuccess = (bool)e.Result;
                        if (isDeleteCacheSuccess)
                            DoDeleteChacheFinish();
                        break;
                    case BGWorkerMehtod_DoRewrawGraph:
                        TandTGraphModel graphModel = (TandTGraphModel)e.Result;
                        DoRedrawGraph(graphModel);
                        break;
                }
            }
        }
        #endregion

        #region BackgroundWorker -> BGWorker mehtods -> Methods for call

        public TandTv3.TandTResult DoSearchAsync(ACBackgroundWorker worker, DoWorkEventArgs e)
        {
            SelectedFilter.BackgroundWorker = worker;
            SelectedFilter.DoWorkEventArgs = e;
            TandTv3.TandTResult result = TandTv3Manager.DoTracking(DatabaseApp, SelectedFilter, Root.Environment.User.Initials, UseGroupResult);
            return result;
        }

        public TandTv3.TandTResult DoSelectAsync(ACBackgroundWorker worker, DoWorkEventArgs e)
        {
            if (SelectedFilter == null) return null;
            SelectedFilter.BackgroundWorker = worker;
            SelectedFilter.DoWorkEventArgs = e;
            TandTv3.TandTResult result = TandTv3Manager.DoSelect(DatabaseApp, SelectedFilter, Root.Environment.User.Initials, UseGroupResult);
            return result;
        }

        public virtual KeyValuePair<TandTv3.TandTResult, MsgWithDetails> DoDeleteAsync(ACBackgroundWorker worker, DoWorkEventArgs e)
        {
            SelectedFilter.BackgroundWorker = worker;
            SelectedFilter.DoWorkEventArgs = e;
            MsgWithDetails msg = TandTv3Manager.DoDeleteTracking(DatabaseApp, SelectedFilter);
            TandTv3FilterTracking nextFilter = FilterList.FirstOrDefault(c => c.TandTv3FilterTrackingID != SelectedFilter.TandTv3FilterTrackingID);
            TandTv3.TandTResult result = null;
            if (nextFilter != null)
                result = TandTv3Manager.DoSelect(DatabaseApp, nextFilter, Root.Environment.User.Initials, UseGroupResult);
            else
                result = new TandTv3.TandTResult(TandTv3Manager.TandTv3Command) { Success = false };
            return new KeyValuePair<TandTv3.TandTResult, MsgWithDetails>(result, msg);
        }

        public virtual bool DoDeleteCacheAsync(ACBackgroundWorker worker, DoWorkEventArgs e)
        {
            bool success = false;
            try
            {
                DatabaseApp.udpTandTv3FilterTrackingDelete(null);
                success = true;
            }
            catch (Exception ec)
            {
                SendMessage(new Msg()
                {
                    Message = ec.Message,
                    MessageLevel = eMsgLevel.Error
                });
            }
            return success;
        }

        public virtual TandTGraphModel DoRewrawGraph(ACBackgroundWorker worker, DoWorkEventArgs e)
        {
            return GraphCommand.BuildGraphResult(Result, IncludedGraphItems, GetGraphModel());
        }

        public TandTGraphModel GetGraphModel()
        {
            return new TandTGraphModel()
            {
                AllGeneratedTandTPointPresenterComponents = AllGeneratedTandTPointPresenterComponents,
                TandTPointPresenterClass = TandTPointPresenterClass,
                DummyClass = DummyClass
            };
        }

        #endregion

        #region  BackgroundWorker -> BGWorker methods -> Callback methods (Finish / Completed)
        public virtual void DoSearchFinish(TandTv3.TandTResult result)
        {
            DoItemsClear();
            Result = result;
            if (result != null)
            {
                if (result.Success)
                {
                    TandTv3FilterTracking tmpFilterTracking = DatabaseApp.TandTv3FilterTracking.FirstOrDefault(c => c.TandTv3FilterTrackingID == result.Filter.TandTv3FilterTrackingID);
                    if (tmpFilterTracking != null)
                    {
                        WriteFilter(tmpFilterTracking);

                        if (result.Filter.RecalcAgain)
                            AccessPrimary.NavList.Remove(tmpFilterTracking);
                        bool addToList = !AccessPrimary.NavList.Any(c => c.MDTrackingDirectionEnum == tmpFilterTracking.MDTrackingDirectionEnum && c.ItemSystemNo == tmpFilterTracking.ItemSystemNo);
                        if (addToList)
                            AccessPrimary.NavList.Add(tmpFilterTracking);
                        if (result.Filter.RecalcAgain || addToList)
                        {
                            AccessPrimary.NavSearch();
                            OnPropertyChanged(nameof(FilterList));
                        }
                        SelectedFilter = tmpFilterTracking;
                        var graphModel = GraphCommand.BuildGraphResult(result, IncludedGraphItems, GetGraphModel());
                        ProcessGraph(graphModel);
                        SelectedGraphAction = GraphAction.InitGraphSurface;
                        SelectedTandTMixPoint = result.MixPoints.FirstOrDefault(c => c.ExistLabOrder);
                    }
                }
                else
                {
                    SelectedGraphAction = GraphAction.CleanUpGraph;
                }
                if (result.ErrorMsg != null)
                {
                    SendMessage(result.ErrorMsg);
                    if (result.ErrorMsg.MsgDetails != null)
                        foreach (var subMessage in result.ErrorMsg.MsgDetails)
                            SendMessage(subMessage);
                }
            }
            DoItemsOnPropertyChanged();
        }

        public virtual void DoDeleteFinish(KeyValuePair<TandTv3.TandTResult, MsgWithDetails> deleteResult)
        {
            MsgList.Add(deleteResult.Value);
            CurrentMsg = deleteResult.Value;
            if (CurrentMsg.MessageLevel == eMsgLevel.Info)
            {
                AccessPrimary.NavList.Remove(SelectedFilter);
                if (Result.Success)
                    SelectedFilter = AccessPrimary.NavList.FirstOrDefault(c => c.TandTv3FilterTrackingID == Result.Filter.TandTv3FilterTrackingID);
                else
                {
                    SelectedFilter = null;
                    Result = null;
                }
            }
            if (deleteResult.Key != null)
                DoSearchFinish(deleteResult.Key);
            else
            {
                DoModelCleanUp();
                SelectedGraphAction = GraphAction.CleanUpGraph;
            }
        }


        public virtual void DoDeleteChacheFinish()
        {
            Result = null;
            DoModelCleanUp();
            SelectedGraphAction = GraphAction.CleanUpGraph;
        }

        public virtual void DoRedrawGraph(TandTGraphModel graphModel)
        {
            ProcessGraph(graphModel);
            SelectedGraphAction = GraphAction.InitGraphSurface;
        }

        public void ProcessGraph(TandTGraphModel graphModel)
        {
            RemoveGraphJob(ActualGraphJobID);
            CleanUpGrapModel();
            if (graphModel.Success)
            {
                ActualGraphJobID = graphModel.JobID;
                TandTPath tmp = new TandTPath();
                tmp.AddRange(graphModel.GraphEdges);
                AvailableTandTEdges = new List<List<TandTPath>>() { new List<TandTPath>() { tmp } };
            }
            else if (graphModel.Error != null)
            {
                MsgList.Add(graphModel.GetDetailedMessage());
            }
            SavedIncludedDisplayTypeList = IncludedGraphItems.ToList();
        }

        public virtual void RemoveGraphJob(Guid jobID)
        {
            foreach (var item in AllGeneratedTandTPointPresenterComponents)
            {
                item.JobIds.Remove(jobID);
            }
        }

        public virtual void DoModelCleanUp()
        {
            DoItemsClear();
            DoItemsOnPropertyChanged();
            AccessNav.NavSearch();
            OnPropertyChanged("FilterList");
            CleanUpGrapModel();
            OnPropertyChanged("AvailableRoutes");
        }

        public virtual void CleanUpGrapModel()
        {
            ActiveTandTObjects = new List<IACObject>();
            ActiveTandTPaths = new List<IACObject>();
            AvailableTandTEdges = null;
        }

        #endregion

        #endregion

        #region Metods -> helper methods

        public virtual void DoItemsClear()
        {
            SelectedDeliveryNote = null;
            SelectedFacilityCharge = null;
            SelectedACObjectWithLabOrder = null;
            SelectedLabOrder = null;
            SelectedLabOrderPos = null;
        }

        public virtual void DoItemsOnPropertyChanged()
        {
            OnPropertyChanged("FilterList");

            OnPropertyChanged("TandTMixPointList");

            OnPropertyChanged("DeliveryNoteList");
            OnPropertyChanged("FacilityChargeList");
            OnPropertyChanged("ACObjectWithLabOrderList");
            OnPropertyChanged("LabOrderList");
            OnPropertyChanged("LabOrderPosList");

        }

        #endregion

        #region Routing

        protected ACRef<ACComponent> _RoutingService = null;
        protected ACComponent RoutingService
        {
            get
            {
                if (_RoutingService == null)
                    return null;
                return _RoutingService.ValueT;
            }
        }

        public bool IsRoutingServiceAvailable
        {
            get
            {
                return RoutingService != null && RoutingService.ConnectionState != ACObjectConnectionState.DisConnected;
            }
        }

        public void Test()
        {
            core.datamodel.ACClass from = DatabaseApp.ContextIPlus.ACClass.FirstOrDefault(c => c.ACURLComponentCached == @"\AppRoesterei\NachBehWa24");
            //core.datamodel.ACClass to = DatabaseApp.ContextIPlus.ACClass.FirstOrDefault(c => c.ACURLComponentCached == @"\AppRoesterei\SiloGrp2\Silo328");
            RoutingResult result = ACRoutingService.MemSelectRoutes(Database.ContextIPlus, @"\AppAnnahme\SiloGrp4\Silo227", @"\AppRoesterei\SiloGrp2\Silo328", RouteDirections.Forwards, "", 1, true, true, null, RoutingService);
            var test = ACRoutingService.FindSuccessors(RoutingService, DatabaseApp.ContextIPlus, false, from, PAProcessModule.SelRuleID_ProcessModule, RouteDirections.Forwards, null,
                                                       (c, p, r) => typeof(PAProcessModule).IsAssignableFrom(c.ObjectFullType), null, 1, true, true, false, false, 1, false);
        }

        #endregion

    }
}
