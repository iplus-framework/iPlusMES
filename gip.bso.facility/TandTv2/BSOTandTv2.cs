using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static gip.core.datamodel.Global;
using static gip.mes.datamodel.GlobalApp;

namespace gip.bso.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Tracking and Tracing'}de{'Verfolgung/Rückverfolgung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, "")]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + TandTv2Job.ClassName, "en{'Tracking and Tracing'}de{'Verfolgung/Rückverfolgung'}", typeof(TandTv2Job), TandTv2Job.ClassName, "ItemSystemNo", "StartTime")]
    [ACClassConstructorInfo(
       new object[]
       {
            new object[] { "JobFilter", Global.ParamOption.Optional, typeof(TandTv2Job) }
       }
   )]
    public class BSOTandTv2 : ACBSOvbNav, IMsgObserver
    {
        #region constants
        public const string BGWorkerMehtod_DoSearch = @"DoSearch";
        public const string BGWorkerMehtod_DoSelect = @"DoSelect";
        public const string BGWorkerMehtod_DoDelete = @"DoDelete";
        public const string BGWorkerMehtod_DoDeleteCache = @"DoDeleteCache";
        public const bool BuildGraphic = false;
        #endregion

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOProdOrderGeneric"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOTandTv2(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            InitializeTrackingManager();

            AccessPrimary.NavSearch(DatabaseApp);
            AccessPrimary.Selected = ParameterValue(TandT2Manager.SearchModel_ParamValueKey) as TandTv2Job;
            if (SelectedJob != null)
            {
                if (!SelectedJob.IsReport)
                {
                    BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoSearch);
                    ShowDialog(this, DesignNameProgressBar);
                }
            }
            else
            {
                SelectedJobChangingActive = true;
                SelectedJob = JobList.FirstOrDefault();
            }
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            DoItemsListClear();
            DoGrahpicalItemsClear();
            DeInitializeTrackingManager();
            Result = null;
            return base.ACDeInit(deleteACClassTask);
        }

        public virtual void InitializeTrackingManager()
        {
            _TandT2Manager = TandT2Manager.ACRefToServiceInstance(this);
            if (_TandT2Manager == null)
                throw new Exception("TrackingAndTracingManager not configured");
        }

        public virtual void DeInitializeTrackingManager()
        {
            TandT2Manager.DetachACRefFromServiceInstance(this, _TandT2Manager);
            _TandT2Manager = null;
        }

        #endregion

        #region BSO -> Clone

        public override object Clone()
        {
            ACValueList parameter = new ACValueList();
            TandTv2Job job = DatabaseApp.TandTv2Job.FirstOrDefault(c => c.TandTv2JobID == SelectedJob.TandTv2JobID);
            job.IsReport = true;
            parameter.Add(new ACValue(TandT2Manager.SearchModel_ParamValueKey, job));
            BSOTandTv2 clone = FactoryTandTBSO(parameter);
            CloneJob(clone);
            return clone;
        }

        public virtual BSOTandTv2 FactoryTandTBSO(ACValueList acValueList)
        {
            return (ParentACComponent as ACComponent).StartComponent(this.ACType as core.datamodel.ACClass, this.Content, acValueList, Global.ACStartTypes.Automatic, IsProxy) as BSOTandTv2;
        }

        public virtual void CloneJob(BSOTandTv2 clone)
        {
            clone._SelectedDeliveryNote = _SelectedDeliveryNote;
            clone._DeliveryNoteList = _DeliveryNoteList;
            clone._SelectedFacilityCharge = _SelectedFacilityCharge;
            clone._FacilityChargeList = _FacilityChargeList;
            clone._SelectedLabOrder = _SelectedLabOrder;
            clone._LabOrderList = _LabOrderList;
            clone._SelectedLabOrderPos = _SelectedLabOrderPos;
            clone._LabOrderPosList = _LabOrderPosList;
            clone._SelectedItemsWithLabOrder = _SelectedItemsWithLabOrder;
            clone._ItemsWithLabOrderList = _ItemsWithLabOrderList;
            clone._SelectedItemType = _SelectedItemType;
            clone._ItemTypeList = _ItemTypeList;
        }

        #endregion

        #region Managers

        protected ACRef<TandT2Manager> _TandT2Manager = null;
        protected TandT2Manager TandT2Manager
        {
            get
            {
                if (_TandT2Manager == null)
                    return null;
                return _TandT2Manager.ValueT;
            }
        }

        #endregion

        #region TrackingAndTracing -> Filter

        #region TrackingAndTracing -> Filter -> JobFilter

        #endregion

        #region TrackingAndTracing -> Filter -> TrackingStyle

        ACValueItem _SelectedTrackingStyle;
        [ACPropertySelected(9999, "TrackingStyle", "en{'Direction'}de{'Richtung'}")]
        public ACValueItem SelectedTrackingStyle
        {
            get
            {
                if (_SelectedTrackingStyle == null)
                {
                    _SelectedTrackingStyle = TrackingStyleList.FirstOrDefault(x => ((short)x.Value) == (short)TandTv2TrackingStyleEnum.Backward);
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

        public TandTv2TrackingStyleEnum FilterTrackingStyle
        {
            get
            {
                return (TandTv2TrackingStyleEnum)(short)SelectedTrackingStyle.Value;
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
                    _TrackingStyleList.AddEntry((short)TandTv2TrackingStyleEnum.Backward, "en{'Backward'}de{'Zurück'}");
                    _TrackingStyleList.AddEntry((short)TandTv2TrackingStyleEnum.Forward, "en{'Forward'}de{'Vorwärts'}");
                }
                return _TrackingStyleList;
            }
        }

        #endregion

        #region TrackingAndTracing -> Filter -> ItemType

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

        public TandTv2ItemTypeEnum FilterItemType
        {
            get
            {
                if (SelectedItemType == null) return TandTv2ItemTypeEnum.FacilityBooking;
                return (TandTv2ItemTypeEnum)Enum.Parse(typeof(TandTv2ItemTypeEnum), SelectedItemType.Value.ToString());
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
                        .TandTv2ItemType
                        .GroupJoin(DatabaseApp.ACClass, it => it.TandTv2ItemTypeID, acl => acl.ACIdentifier, (it, acl) => new { ID = it.TandTv2ItemTypeID, Translation = acl.Select(c => c.ACCaptionTranslation).FirstOrDefault() });
                    foreach (var item in query)
                        _ItemTypeList.AddEntry(item.ID, item.Translation);
                }
                return _ItemTypeList;
            }
        }

        #endregion

        #region TrackingAndTracing -> Filter -> Fields

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
        [ACPropertyInfo(9999, "FilterDateFrom", "en{'From'}de{'Von'}")]
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
        [ACPropertyInfo(9999, "FilterDateTo", "en{'To'}de{'Bis'}")]
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

        #endregion

        #region TrackingAndTracing -> Filter -> DisplayType

        private ACValueItemList _DisplayTypeTitle = null;
        public IEnumerable<ACValueItem> DisplayTypeTitle
        {
            get
            {
                if (_DisplayTypeTitle == null)
                {
                    _DisplayTypeTitle = new ACValueItemList("DisplayTypeTitle");
                    _DisplayTypeTitle.AddEntry(DisplayGroupEnum.Storage, "en{'Storage'}de{'Behälter'}");
                    _DisplayTypeTitle.AddEntry(DisplayGroupEnum.Quants, "en{'Quants'}de{'Quanten'}");
                    _DisplayTypeTitle.AddEntry(DisplayGroupEnum.Lots, "en{'Lots'}de{'Lots'}");
                    _DisplayTypeTitle.AddEntry(DisplayGroupEnum.Orders, "en{'Orders'}de{'Auftrag'}");
                    _DisplayTypeTitle.AddEntry(DisplayGroupEnum.Bookings, "en{'Bookings'}de{'Buchungen'}");
                    _DisplayTypeTitle.AddEntry(DisplayGroupEnum.Machines, "en{'Machines'}de{'Maschinen'}");
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
                    _DisplayTypeSubTitle.AddEntry(DisplayGroupEnum.Storage, "en{'Facility'}de{'Lagerplatz'}");
                    _DisplayTypeSubTitle.AddEntry(DisplayGroupEnum.Quants, "en{'Facilitycharge'}de{'Chargenplatz'}");
                    _DisplayTypeSubTitle.AddEntry(DisplayGroupEnum.Lots, "en{'Lot'}de{'Los'}");
                    _DisplayTypeSubTitle.AddEntry(DisplayGroupEnum.Orders, "en{'Production Order'}de{'Auftrag'}");
                    _DisplayTypeSubTitle.AddEntry(DisplayGroupEnum.Bookings, @"en{'Stock movement /of quantum'}de{'Lagerbewegung /Quant'}");
                    _DisplayTypeSubTitle.AddEntry(DisplayGroupEnum.Machines, "en{'Machines'}de{'Machines'}");
                }
                return _DisplayTypeSubTitle;
            }
        }

        private TandTv2DisplayItemType _SelectedDisplayType;
        [ACPropertySelected(9999, "DisplayType", "en{'DisplayType'}de{'DisplayType'}")]
        public TandTv2DisplayItemType SelectedDisplayType
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

        private List<TandTv2DisplayItemType> _DisplayTypeList;
        [ACPropertyList(9999, "DisplayType")]
        public List<TandTv2DisplayItemType> DisplayTypeList
        {
            get
            {
                if (_DisplayTypeList == null)
                    _DisplayTypeList = LoadDisplayTypeList();
                return _DisplayTypeList;
            }
        }

        private List<TandTv2DisplayItemType> LoadDisplayTypeList()
        {
            List<TandTv2DisplayItemType> result = new List<TandTv2DisplayItemType>();
            result = DisplayTypeTitle
                .Join(DisplayTypeSubTitle, st => st.Value, tit => tit.Value, (st, tit) => new { Title = tit, SubTitle = st })
                .Select(c =>
                new TandTv2DisplayItemType()
                {
                    Title = c.Title.ACCaption,
                    SubTitle = c.SubTitle.ACCaption,
                    IsIncluded = new List<DisplayGroupEnum>() { DisplayGroupEnum.Lots, DisplayGroupEnum.Bookings }.Contains((DisplayGroupEnum)c.Title.Value),
                    ItemType = (DisplayGroupEnum)c.Title.Value
                })
                .ToList();
            return result;
        }

        [ACMethodInfo("SwitchDisplayType", "en{'SwitchDisplayType'}de{'Berechnen und Aktualisieren'}", 9999, false)]
        public void SwitchDisplayType()
        {
            if (!IsEnabledSwitchDisplayType()) return;
            SelectedDisplayType.IsIncluded = !SelectedDisplayType.IsIncluded;
        }

        public bool IsEnabledSwitchDisplayType()
        {
            return SelectedDisplayType != null;
        }

        #endregion

        #endregion

        #region Graphic route preveiw

        [ACPropertyInfo(999)]
        public List<List<ACRoutingPath>> AvailableRoutes
        {
            get;
            set;
        }

        private List<IACComponent> _ActiveRouteComponents;
        [ACPropertyInfo(999)]
        public List<IACComponent> ActiveRouteComponents
        {
            get { return _ActiveRouteComponents; }
            set
            {
                _ActiveRouteComponents = value;
                OnPropertyChanged("ActiveRouteComponents");
            }
        }

        private List<IACObject> _ActiveRoutePaths;
        [ACPropertyInfo(999)]
        public List<IACObject> ActiveRoutePaths
        {
            get { return _ActiveRoutePaths; }
            set
            {
                _ActiveRoutePaths = value;
                OnPropertyChanged("ActiveRoutePaths");
            }
        }

        #endregion

        #region  TrackingAndTracing -> SelectedJob

        #region Primary Navigation

        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<TandTv2Job> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "MDLabOrderState")]
        public ACAccessNav<TandTv2Job> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<TandTv2Job>("TandTv2Job", this);
                    navACQueryDefinition.ClearSort(true);
                    navACQueryDefinition.ACSortColumns.Add(new ACSortItem("StartTime", Global.SortDirections.descending, true));
                    //_AccessPrimary.NavACQueryDefinition.filter
                }
                return _AccessPrimary;
            }
        }
        #endregion

        /// <summary>
        /// job changing active
        /// </summary>
        public bool SelectedJobChangingActive { get; set; }

        /// <summary>
        /// Selected property for TandTv2Job
        /// </summary>
        /// <value>The selected Job</value>
        [ACPropertySelected(9999, "Job", "en{'TODO: Job'}de{'TODO: Job'}")]
        public TandTv2Job SelectedJob
        {
            get
            {
                return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary.Selected != value)
                {
                    AccessPrimary.Selected = value;
                    OnPropertyChanged("SelectedJob");
                    if (SelectedJobChangingActive)
                        SelectedJob_Changed();
                }
            }
        }


        /// <summary>
        /// List property for TandTv2Job
        /// </summary>
        /// <value>The Job list</value>
        [ACPropertyList(9999, "Job")]
        public IEnumerable<TandTv2Job> JobList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }


        private void SelectedJob_Changed()
        {
            WriteFilter(SelectedJob);
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoSelect);
            ShowDialog(this, DesignNameProgressBar);
        }

        #endregion

        #endregion

        #region TrackingAndTracing -> TandT Lists

        public TandTv2Result Result { get; set; }

        // Step
        #region TrackingAndTracing -> TandT List -> TandTv2Step

        private TandTv2Step _SelectedStep;
        /// <summary>
        /// Selected property for TandTv2Step
        /// </summary>
        /// <value>The selected Step</value>
        [ACPropertySelected(9999, "Step", "en{'TODO: Step'}de{'TODO: Step'}")]
        public TandTv2Step SelectedStep
        {
            get
            {
                return _SelectedStep;
            }
            set
            {
                if (_SelectedStep != value)
                {
                    _SelectedStep = value;
                    OnPropertyChanged("SelectedStep");
                }
            }
        }


        private List<TandTv2Step> _StepList;
        /// <summary>
        /// List property for TandTv2Step
        /// </summary>
        /// <value>The Step list</value>
        [ACPropertyList(9999, "Step")]
        public List<TandTv2Step> StepList
        {
            get
            {
                return _StepList;
            }
        }

        #endregion

        // StepItem
        #region TrackingAndTracing -> TandT Lists -> StepItem
        private TandTv2StepItem _SelectedStepItem;
        /// <summary>
        /// Selected property for TandTv2StepItem
        /// </summary>
        /// <value>The selected StepItem</value>
        [ACPropertySelected(9999, "StepItem", "en{'TODO: StepItem'}de{'TODO: StepItem'}")]
        public TandTv2StepItem SelectedStepItem
        {
            get
            {
                return _SelectedStepItem;
            }
            set
            {
                if (_SelectedStepItem != value)
                {
                    _SelectedStepItem = value;
                    OnPropertyChanged("SelectedStepItem");
                }
            }
        }


        public List<TandTv2StepItem> _StepItemList;
        /// <summary>
        /// List property for TandTv2StepItem
        /// </summary>
        /// <value>The StepItem list</value>
        [ACPropertyList(9999, "StepItem")]
        public List<TandTv2StepItem> StepItemList
        {
            get
            {
                return _StepItemList;
            }
        }

        #region FacilityCharge
        private FacilityCharge _SelectedFacilityCharge;
        /// <summary>
        /// Selected property for FacilityCharge
        /// </summary>
        /// <value>The selected FacilityCharge</value>
        [ACPropertySelected(9999, "FacilityCharge", "en{'TODO: FacilityCharge'}de{'TODO: FacilityCharge'}")]
        public FacilityCharge SelectedFacilityCharge
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


        private List<FacilityChargeModel> _FacilityChargeList;
        /// <summary>
        /// List property for FacilityCharge
        /// </summary>
        /// <value>The FacilityCharge list</value>
        [ACPropertyList(9999, "FacilityCharge")]
        public List<FacilityChargeModel> FacilityChargeList
        {
            get
            {
                return _FacilityChargeList;
            }
        }

        #endregion


        #endregion

        // StepLot
        #region TrackingAndTracing -> TandT List -> StepLot
        private TandTv2StepLot _SelectedStepLot;
        /// <summary>
        /// Selected property for TandTv2StepLot
        /// </summary>
        /// <value>The selected StepLot</value>
        [ACPropertySelected(9999, "StepLot", "en{'TODO: StepLot'}de{'TODO: StepLot'}")]
        public TandTv2StepLot SelectedStepLot
        {
            get
            {
                return _SelectedStepLot;
            }
            set
            {
                if (_SelectedStepLot != value)
                {
                    _SelectedStepLot = value;
                    OnPropertyChanged("SelectedStepLot");
                }
            }
        }

        private List<TandTv2StepLot> _StepLotList;
        /// <summary>
        /// List property for TandTv2StepLot
        /// </summary>
        /// <value>The StepLot list</value>
        [ACPropertyList(9999, "StepLot")]
        public List<TandTv2StepLot> StepLotList
        {
            get
            {
                return _StepLotList;
            }
        }

        #endregion

        // ItemsWithLabOrder
        #region TrackingAndTracing -> TandT Lists -> ItemsWithLabOrder

        private TandTv2StepItem _SelectedItemsWithLabOrder;
        /// <summary>
        /// Selected property for TandTv2StepItem
        /// </summary>
        /// <value>The selected ItemsWithLabOrder</value>
        [ACPropertySelected(9999, "ItemsWithLabOrder", "en{'TODO: ItemsWithLabOrder'}de{'TODO: ItemsWithLabOrder'}")]
        public TandTv2StepItem SelectedItemsWithLabOrder
        {
            get
            {
                return _SelectedItemsWithLabOrder;
            }
            set
            {
                if (_SelectedItemsWithLabOrder != value)
                {
                    _SelectedItemsWithLabOrder = value;
                    OnPropertyChanged("SelectedItemsWithLabOrder");

                    _LabOrderList = LoadLabOrderList(SelectedItemsWithLabOrder);
                    OnPropertyChanged("LabOrderList");

                    if (_LabOrderList == null)
                        SelectedLabOrder = null;
                    else
                        SelectedLabOrder = _LabOrderList.FirstOrDefault();
                }
            }
        }

        private List<TandTv2StepItem> _ItemsWithLabOrderList;
        /// <summary>
        /// List property for TandTv2StepItem
        /// </summary>
        /// <value>The ItemsWithLabOrder list</value>
        [ACPropertyList(9999, "ItemsWithLabOrder")]
        public List<TandTv2StepItem> ItemsWithLabOrderList
        {
            get
            {
                return _ItemsWithLabOrderList;
            }
        }

        #endregion

        // DeliveryNote
        #region TrackingAndTracing -> TandT Lists -> DeliveryNote


        #region DeliveryNote
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


        private List<DeliveryNotePosPreview> _DeliveryNoteList;
        /// <summary>
        /// List property for DeliveryNote
        /// </summary>
        /// <value>The DeliveryNote list</value>
        [ACPropertyList(9999, "DeliveryNote")]
        public List<DeliveryNotePosPreview> DeliveryNoteList
        {
            get
            {
                return _DeliveryNoteList;
            }
        }

        #endregion


        #endregion

        #endregion

        #region TrackingAndTracking -> LabOrder

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
                    OnPropertyChanged("SelectedLabOrder");

                    _LabOrderPosList = LoadLabOrderPosList();
                    OnPropertyChanged("LabOrderPosList");
                }
            }
        }


        private List<LabOrder> _LabOrderList;
        /// <summary>
        /// List property for LabOrder
        /// </summary>
        /// <value>The LabOrder list</value>
        [ACPropertyList(9999, "LabOrder")]
        public List<LabOrder> LabOrderList
        {
            get
            {
                if (_LabOrderList == null)
                    _LabOrderList = new List<LabOrder>();
                return _LabOrderList;
            }
        }

        private List<LabOrder> LoadLabOrderList(TandTv2StepItem stepItem)
        {
            if (SelectedItemsWithLabOrder == null) return null;
            List<LabOrder> labOrders = new List<LabOrder>();

            if (SelectedItemsWithLabOrder.InOrderPosID != null)
                labOrders = stepItem.InOrderPos.LabOrder_InOrderPos.OrderBy(c => c.LabOrderNo).ToList();

            if (SelectedItemsWithLabOrder.OutOrderPosID != null)
                labOrders = stepItem.OutOrderPos.LabOrder_OutOrderPos.OrderBy(c => c.LabOrderNo).ToList();

            if (SelectedItemsWithLabOrder.ProdOrderPartslistPosID != null)
                labOrders = stepItem.ProdOrderPartslistPos.LabOrder_ProdOrderPartslistPos.OrderBy(c => c.LabOrderNo).ToList();

            return labOrders;
        }


        [ACMethodInteraction("DeliveryNoteTypeList", "en{'Show LabOrder'}de{'Zeige Laborauftrag'}", 50, false)]
        public virtual void ShowLaborInfoForDn()
        {
            if (IsEnabledShowLaborInfoForDn())
            {
                TandTv2StepItem stepItem = StepItemList.FirstOrDefault(c => c.PrimaryKeyID == SelectedDeliveryNote.DeliveryNotePosID);// DatabaseApp.DeliveryNotePos.Where(x => x.DeliveryNotePosID == SelectedDeliveryNote.DeliveryNotePosID).FirstOrDefault();
                LoadLabOrderList(stepItem);
                ShowDialog(this, "LabOrder");
            }
        }

        public bool IsEnabledShowLaborInfoForDn()
        {
            return SelectedDeliveryNote != null;
        }

        #endregion

        #region TrackingAndTracking -> LabOrderPos


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


        private List<LabOrderPos> _LabOrderPosList;
        /// <summary>
        /// List property for LabOrderPos
        /// </summary>
        /// <value>The LabOrderPos list</value>
        [ACPropertyList(9999, "LabOrderPos")]
        public List<LabOrderPos> LabOrderPosList
        {
            get
            {
                if (_LabOrderPosList == null)
                    _LabOrderPosList = LoadLabOrderPosList();
                return _LabOrderPosList;
            }
        }

        private List<LabOrderPos> LoadLabOrderPosList()
        {
            if (SelectedLabOrder == null) return null;
            return SelectedLabOrder.LabOrderPos_LabOrder.OrderBy(c => c.Sequence).ToList();
        }

        #endregion

        #region Actions 

        [ACMethodInfo("Search", "en{'Start tracking'}de{'Chargenverfolgung starten'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void Search()
        {
            if (!IsEnabledSearch()) return;
            SelectedJobChangingActive = false;
            SelectedJob = ReadFilter();
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoSearch);
            ShowDialog(this, DesignNameProgressBar);
        }
        public bool IsEnabledSearch()
        {
            return !BackgroundWorker.IsBusy && !string.IsNullOrEmpty(FilterSearchNo);
        }

        [ACMethodInfo("Filter", "en{'Filter'}de{'Filter'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void Filter()
        {
            if (!IsEnabledFilter()) return;
            SelectedJobChangingActive = false;
            _StepItemList = null;
            TandT2Manager.ApplyFilter(Result, GetIncludedItemTypes(), BuildGraphic);
            _StepItemList = Result.FilteredStepItems;
            OnPropertyChanged("StepItemList");
            DoGraphicalFinish(Result);
            SelectedJobChangingActive = true;
        }

        public virtual bool IsEnabledFilter()
        {
            return !BackgroundWorker.IsBusy && Result != null && Result.Success;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction("SelectedJobDelete", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "SelectedJob", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!IsEnabledDelete()) return;
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoDelete);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledDelete()
        {
            return !BackgroundWorker.IsBusy && SelectedJob != null;
        }

        [ACMethodInfo("Filter", "en{'Delete all results'}de{'Lösche alle Ergebnisse'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public void DeleteAllCacheDlg()
        {
            MsgResult msgResult = Messages.Question(this, "Question50040");
            if (msgResult == MsgResult.Yes)
            {
                BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoDeleteCache);
                ShowDialog(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledDeleteAllCacheDlg()
        {
            return !BackgroundWorker.IsBusy && JobList != null && JobList.Any();
        }

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
            SelectedJobChangingActive = false;
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();
            switch (command)
            {
                case BGWorkerMehtod_DoSearch:
                    e.Result = DoSearchAsync();
                    break;
                case BGWorkerMehtod_DoSelect:
                    e.Result = DoSelectAsync();
                    break;
                case BGWorkerMehtod_DoDelete:
                    e.Result = DoDeleteAsync();
                    break;
                case BGWorkerMehtod_DoDeleteCache:
                    e.Result = DoDeleteCacheAsync();
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
                        Result = e.Result as TandTv2Result;
                        DoSearchFinish(Result);
                        break;
                    case BGWorkerMehtod_DoSelect:
                        Result = e.Result as TandTv2Result;
                        DoSearchFinish(Result);
                        break;
                    case BGWorkerMehtod_DoDelete:
                        KeyValuePair<TandTv2Result, KeyValuePair<Msg, TandTv2Job>> deleteResult = (KeyValuePair<TandTv2Result, KeyValuePair<Msg, TandTv2Job>>)e.Result;
                        if(deleteResult.Value.Key.MessageLevel == eMsgLevel.Info)
                        {
                            Result = deleteResult.Key;
                            AccessPrimary.NavList.Remove(deleteResult.Value.Value);
                            DoSearchFinish(Result);
                        }
                        MsgList.Add(deleteResult.Value.Key);
                        CurrentMsg = deleteResult.Value.Key;
                        
                        break;
                    case BGWorkerMehtod_DoDeleteCache:
                        bool isDeleteCacheSuccess = (bool)e.Result;
                        if (isDeleteCacheSuccess)
                            DoDeleteCacheFinish();
                        break;
                }
            }

        }
        #endregion

        #region BackgroundWorker -> BGWorker mehtods -> Methods for call

        public TandTv2Result DoSearchAsync()
        {
            TandTv2Result result = TandT2Manager.DoTracking(SelectedJob, Root.CurrentInvokingUser.Initials);
            TandT2Manager.ApplyFilter(result, GetIncludedItemTypes(), BuildGraphic);
            return result;
        }

        public TandTv2Result DoSelectAsync()
        {
            if (SelectedJob == null) return null;
            TandTv2Result result = TandT2Manager.DoSelect(SelectedJob);
            TandT2Manager.ApplyFilter(result, GetIncludedItemTypes(), BuildGraphic);
            return result;
        }

        public virtual KeyValuePair<TandTv2Result, KeyValuePair<Msg, TandTv2Job>> DoDeleteAsync()
        {
            KeyValuePair<Msg, TandTv2Job> deleteResult = TandT2Manager.DoDeleteTracking(SelectedJob);
            TandTv2Job nextJob = JobList.FirstOrDefault(c => c.TandTv2JobID != SelectedJob.TandTv2JobID);
            TandTv2Result tandTv2Result = null;
            if (nextJob != null)
                tandTv2Result = TandT2Manager.DoSelect(nextJob);
            return new KeyValuePair<TandTv2Result, KeyValuePair<Msg, TandTv2Job>>(tandTv2Result, deleteResult);
        }

        public virtual bool DoDeleteCacheAsync()
        {
            bool success = false;
            try
            {
                DatabaseApp.udpTandTv2JobDelete(null);
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

        #endregion

        #region  BackgroundWorker -> BGWorker mehtods -> Callback methods (Finish / Completed)
        public void DoSearchFinish(TandTv2Result result)
        {
            DoItemsListClear();
            _StepItemList = null;
            if (result != null)
            {
                if (result.Success)
                {
                    // DataItems
                    _StepList = result.Steps;
                    _StepItemList = result.FilteredStepItems;
                    DoGraphicalFinish(result);

                    _StepLotList = result.StepLots;
                    _FacilityChargeList = result.FacilityChargeModels;
                    _ItemsWithLabOrderList = result.ItemsWithLabOrder;
                    _DeliveryNoteList = result.DeliveryNotes;

                    TandTv2Job tmpJob = DatabaseApp.TandTv2Job.FirstOrDefault(c => c.TandTv2JobID == result.Job.TandTv2JobID);
                    if (!AccessPrimary.NavList.Any(c => c.TrackingStyleEnum == result.Job.TrackingStyleEnum && c.ItemSystemNo == result.Job.ItemSystemNo))
                        AccessPrimary.NavList.Add(tmpJob);
                    SelectedJob = tmpJob;
                    WriteFilter(SelectedJob);
                }
                else
                {
                    if (result.ErrorMsg != null)
                    {
                        SendMessage(result.ErrorMsg);
                        if(result.ErrorMsg.MsgDetails != null)
                            foreach(var subMessage in result.ErrorMsg.MsgDetails)
                                SendMessage(subMessage);
                    }
                }
            }
            DoItemsOnPropertyChanged();
            SelectedJobChangingActive = true;
        }

        public void DoGraphicalFinish(TandTv2Result result)
        {
            DoGrahpicalItemsClear();
            if (result.Edges != null && result.Edges.Any())
            {
                AvailableRoutes = result.AvailableRoutes;
                ActiveRouteComponents = result.ActiveRouteComponents;
                ActiveRoutePaths = result.ActiveRoutePaths;
            }
            BroadcastToVBControls("RelayoutElements", "InitVBControl");
        }

        public virtual void DoDeleteCacheFinish()
        {
            DoItemsListClear();
            DoGrahpicalItemsClear();
            DoItemsOnPropertyChanged();
            _StepItemList = null;
            OnPropertyChanged("StepItemList");
            AccessPrimary.NavList.Clear();
            OnPropertyChanged("JobList");
            SelectedJob = null;
        }

        #endregion

        #endregion

        #region IMsgObserver

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged("MsgList");
        }
        #endregion

        #region Messages

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

        #region Filter Methods

        public void WriteFilter(TandTv2Job jobFilter)
        {
            if (jobFilter != null)
            {
                SelectedItemType = ItemTypeList.FirstOrDefault(c => c.Value.ToString() == jobFilter.ItemTypeEnum.ToString());
                FilterSearchNo = jobFilter.ItemSystemNo;
                SelectedTrackingStyle = TrackingStyleList.FirstOrDefault(c => ((TandTv2TrackingStyleEnum)Enum.Parse(typeof(TandTv2TrackingStyleEnum), c.Value.ToString())) == jobFilter.TrackingStyleEnum);
                FilterDateFrom = jobFilter.FilterDateFrom;
                FilterDateTo = jobFilter.FilterDateTo;
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

        public List<TandTv2ItemTypeEnum> GetIncludedItemTypes()
        {
            List<TandTv2ItemTypeEnum> displayTypesForPrevew = new List<TandTv2ItemTypeEnum>();
            foreach (var item in DisplayTypeList)
            {
                if (item.IsIncluded)
                    displayTypesForPrevew.AddRange(CastDisplayGroupToItemTypeEnum.Cast(item.ItemType));
            }
            return displayTypesForPrevew;
        }

        private TandTv2Job ReadFilter()
        {
            TandTv2Job jobFilter = new TandTv2Job();
            jobFilter.ItemTypeEnum = FilterItemType;
            switch (FilterItemType)
            {
                case TandTv2ItemTypeEnum.FacilityBooking:
                    FacilityBooking fb = DatabaseApp.FacilityBooking.FirstOrDefault(c => c.FacilityBookingNo == FilterSearchNo);
                    if (fb != null)
                    {
                        jobFilter.ItemSystemNo = fb.FacilityBookingNo;
                        jobFilter.PrimaryKeyID = fb.FacilityBookingID;
                    }
                    break;
                case TandTv2ItemTypeEnum.FacilityPreBooking:
                    FacilityPreBooking fbPre = DatabaseApp.FacilityPreBooking.FirstOrDefault(c => c.FacilityPreBookingNo == FilterSearchNo);
                    if (fbPre != null)
                    {
                        jobFilter.ItemSystemNo = fbPre.FacilityPreBookingNo;
                        jobFilter.PrimaryKeyID = fbPre.FacilityPreBookingID;
                    }
                    break;
                case TandTv2ItemTypeEnum.InOrderPos:
                    break;
                case TandTv2ItemTypeEnum.OutOrderPos:
                    break;
                case TandTv2ItemTypeEnum.DeliveryNotePos:
                    break;
                default:
                    break;
            }
            jobFilter.TrackingStyleEnum = FilterTrackingStyle;
            jobFilter.FilterDateFrom = FilterDateFrom;
            jobFilter.FilterDateTo = FilterDateTo;
            jobFilter.RecalcAgain = true;
            return jobFilter;
        }

        private void WriteNavFilter(ACQueryDefinition navACQueryDefinition, TandTv2Job jobFilter)
        {
            navACQueryDefinition.FactoryACFilterItem("ItemTypeID", jobFilter != null ? jobFilter.TandTv2ItemTypeID : "");
            navACQueryDefinition.FactoryACFilterItem("ItemSystemNo", jobFilter != null ? jobFilter.ItemSystemNo : "");
            navACQueryDefinition.FactoryACFilterItem("TrackingStyleID", jobFilter != null ? jobFilter.TandTv2TrackingStyleID : "");

            navACQueryDefinition.FactoryACFilterItem("FilterDateFrom", (jobFilter != null && jobFilter.FilterDateFrom != null) ? jobFilter.FilterDateFrom.ToString() : "");
            navACQueryDefinition.FactoryACFilterItem("FilterDateTo", (jobFilter != null && jobFilter.FilterDateTo != null) ? jobFilter.FilterDateTo.ToString() : "");
        }

        #endregion

        #region Methods


        private void DoItemsOnPropertyChanged()
        {
            OnPropertyChanged("StepList");
            OnPropertyChanged("StepItemList");
            OnPropertyChanged("StepLotList");
            OnPropertyChanged("ItemsWithLabOrderList");
            OnPropertyChanged("DeliveryNoteList");
            OnPropertyChanged("JobList");
            OnPropertyChanged("FacilityChargeList");
        }
        #endregion

        #region Metods -> Clean up

        public virtual void DoItemsListClear()
        {

            // Data items
            _StepList = null;

            _StepLotList = null;

            _ItemsWithLabOrderList = null;
            _DeliveryNoteList = null;
            _FacilityChargeList = null;
        }

        public virtual void DoGrahpicalItemsClear()
        {
            // Graphical Items
            AvailableRoutes = null;
            ActiveRouteComponents = null;
            ActiveRoutePaths = null;
        }
        #endregion



    }
}
