using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Tracking and Tracing'}de{'Verfolgung/Rückverfolgung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, "")]
    [ACClassConstructorInfo(
        new object[] 
        { 
            new object[] {"CallerObject", Global.ParamOption.Optional, typeof(IACObject) },
            new object[] {"SearchModel", Global.ParamOption.Optional, typeof(GlobalApp.TrackingAndTracingSearchModel) },
            new object[] {"IsSearchIntermediately", Global.ParamOption.Optional, typeof(bool) },
            new object[] {"TandTFilter", Global.ParamOption.Optional, typeof(TandTFilter) },
        }
    )]
    public class BSOTrackingAndTracing : ACBSOvbNav
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOTrackingAndTracing(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

            _TrackingAndTracingManager = ACTrackingAndTracingManager.ACRefToServiceInstance(this);
            if (_TrackingAndTracingManager == null)
                throw new Exception("TrackingAndTracingManager not configured");

            _TandT2Manager = TandT2Manager.ACRefToServiceInstance(this);
            if (_TandT2Manager == null)
                new Exception("TandT2Manager not configured");

            ShowFacilityLot = true;
            ShowFacilityCharge = true;
            if (ParameterValue("CallerObject") != null)
            {

                CallerObject = ParameterValue("CallerObject") as IACObject;
                SearchModelValue = (GlobalApp.TrackingAndTracingSearchModel)ParameterValue("SearchModel");
                bool isSearchIntermediately = ParameterValue("IsSearchIntermediately") != null ? ((bool)ParameterValue("IsSearchIntermediately")) : false;
                if (ParameterValue("TandTFilter") != null)
                    TandTFilter = (TandTFilter)ParameterValue("TandTFilter");
                if (CallerObject != null)
                {
                    FilterPreselect(CallerObject);
                    if (isSearchIntermediately)
                    {
                        Search(false);
                    }
                }
            }
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACTrackingAndTracingManager.DetachACRefFromServiceInstance(this, _TrackingAndTracingManager);
            _TrackingAndTracingManager = null;

            TandT2Manager.DetachACRefFromServiceInstance(this, _TandT2Manager);
            _TandT2Manager = null;

            this._CurrentResult = null;
            this._CurrentResultRoot = null;
            this._SelectedFilterItemSequence = null;
            this._SelectedFilterItemTypes = null;
            this._SelectedFilterLotTypes = null;
            this._SelectedTrackingAndTracingCharge = null;
            this._SelectedTrackingAndTracingDeliveryNote = null;
            this._SearchModel = null;
            this._TrackingAndTracingChargeList = null;
            this._TrackingAndTracingDeliveryNoteList = null;
            return base.ACDeInit(deleteACClassTask);
        }


        #endregion

        #region Managers

        protected ACRef<ACTrackingAndTracingManager> _TrackingAndTracingManager = null;
        protected ACTrackingAndTracingManager TrackingAndTracingManager
        {
            get
            {
                if (_TrackingAndTracingManager == null)
                    return null;
                return _TrackingAndTracingManager.ValueT;
            }
        }

        protected ACRef<TandT2Manager> _TandT2Manager = null;
        public TandT2Manager TandT2Manager
        {
            get
            {
                if (_TandT2Manager == null)
                    return null;
                return _TandT2Manager.ValueT;
            }
        }

        #endregion

        #region ACBSOvbNav
        public override core.datamodel.IAccessNav AccessNav
        {
            get { return null; }
        }

        #endregion

        #region TrackingAndTracing

        #region TrackingAndTracing -> Select, Properites

        #region TrackingAndTracing -> Point result

        public TrackingAndTracingResult Result { get; set; }

        private TrackingAndTracingPoint _CurrentResult;
        [ACPropertyCurrent(9999, "TTResult", "en{'Result'}de{'Result'}")]
        public TrackingAndTracingPoint CurrentResult
        {
            get
            {
                return _CurrentResult;
            }
            set
            {
                if (_CurrentResult != value)
                {
                    _CurrentResult = value;
                    if (value != null)
                    {
                        SetupPreviewDesignNameAndTitle(value);
                        SelectedResult = value.Related;
                    }
                    OnPropertyChanged("CurrentResultRoot");
                    OnPropertyChanged("SelectedResult");
                    OnPropertyChanged("InwardFacilityBookingVisibility");
                    OnPropertyChanged("OutwardFacilityBookingVisibility");
                }
            }
        }

        private TrackingAndTracingPoint _CurrentResultRoot;
        [ACPropertyCurrent(9999, "TTResultRoot", "en{'Result'}de{'Result'}")]
        public TrackingAndTracingPoint CurrentResultRoot
        {
            get
            {
                return _CurrentResultRoot;
            }
            set
            {
                if (_CurrentResultRoot != value)
                {
                    _CurrentResultRoot = value;
                }
            }
        }

        [ACPropertyInfo(9999, "TrackingAndTracingResult", "en{'Preview Design Name'}de{'Preview Design Name'}")]
        public string PreviewDesignName { get; set; }

        #endregion

        #region TrackingAndTracing -> Search model


        ACValueItem _SearchModel;
        [ACPropertySelected(9999, "TrackingAndTracingSearchModel", "en{'Direction'}de{'Richtung'}")]
        public ACValueItem SearchModel
        {
            get
            {
                if (_SearchModel == null)
                {
                    _SearchModel = SearchModelList.FirstOrDefault(x => ((short)x.Value) == (short)GlobalApp.TrackingAndTracingSearchModel.Backward);
                }
                return _SearchModel;
            }
            set
            {
                if (_SearchModel != value)
                {
                    _SearchModel = value;
                    OnPropertyChanged("SearchModel");
                }
            }
        }

        public GlobalApp.TrackingAndTracingSearchModel SearchModelValue
        {
            get
            {
                return (GlobalApp.TrackingAndTracingSearchModel)(short)SearchModel.Value;
            }
            set
            {
                SearchModel = SearchModelList.FirstOrDefault(x => ((short)x.Value) == (short)value);
            }
        }

        [ACPropertyList(9999, "TrackingAndTracingSearchModel")]
        public IEnumerable<ACValueItem> SearchModelList
        {
            get
            {
                return DatabaseApp.TrackAndTracingSearchModelList;
            }
        }
        #endregion

        #region TrackingAndTracing -> TrackingAndTracingCharge

        private FacilityChargeModel _SelectedTrackingAndTracingCharge;
        [ACPropertySelected(9999, "TrackingAndTracingCharge", "en{'Facility Charge'}de{'Anlage Ladung'}")]
        public FacilityChargeModel SelectedTrackingAndTracingCharge
        {
            get
            {
                return _SelectedTrackingAndTracingCharge;
            }
            set
            {
                if (_SelectedTrackingAndTracingCharge != value)
                {
                    _SelectedTrackingAndTracingCharge = value;
                    OnPropertyChanged("SelectedTrackingAndTracingCharge");
                }
            }
        }

        private List<FacilityChargeModel> _TrackingAndTracingChargeList;
        [ACPropertyList(9999, "TrackingAndTracingCharge", "en{'Facility Charge'}de{'Anlage Ladung'}")]
        public List<FacilityChargeModel> TrackingAndTracingChargeList
        {
            get
            {
                if (_TrackingAndTracingChargeList == null)
                    _TrackingAndTracingChargeList = new List<FacilityChargeModel>();
                var list = _TrackingAndTracingChargeList;
                switch (FilterLotTypes)
                {
                    case TandTLotPreviewEnum.All:
                        break;
                    case TandTLotPreviewEnum.Input:
                        list = _TrackingAndTracingChargeList.Where(x => !string.IsNullOrEmpty(x.ExternLotNo)).ToList();
                        break;
                    case TandTLotPreviewEnum.Production:
                        // list = _TrackingAndTracingChargeList.Where(x => x.GetFinalRootPositionFromFB() != null).ToList();
                        break;
                    default:
                        break;
                }
                return list;
            }
        }

        #endregion

        #region TrackingAndTracing -> TrackingAndTracingDeliveryNote

        private DeliveryNotePosPreview _SelectedTrackingAndTracingDeliveryNote;
        [ACPropertySelected(9999, "TrackingAndTracingDeliveryNote", "en{'Facility Charge'}de{'Anlage Ladung'}")]
        public DeliveryNotePosPreview SelectedTrackingAndTracingDeliveryNote
        {
            get
            {
                return _SelectedTrackingAndTracingDeliveryNote;
            }
            set
            {
                if (_SelectedTrackingAndTracingDeliveryNote != value)
                {
                    _SelectedTrackingAndTracingDeliveryNote = value;
                    OnPropertyChanged("SelectedTrackingAndTracingDeliveryNote");
                }
            }
        }

        private List<DeliveryNotePosPreview> _TrackingAndTracingDeliveryNoteList;
        [ACPropertyList(9999, "TrackingAndTracingDeliveryNote", "en{'Facility Charge'}de{'Anlage Ladung'}")]
        public List<DeliveryNotePosPreview> TrackingAndTracingDeliveryNoteList
        {
            get
            {
                if (_TrackingAndTracingDeliveryNoteList == null)
                    _TrackingAndTracingDeliveryNoteList = new List<DeliveryNotePosPreview>();
                return _TrackingAndTracingDeliveryNoteList;
            }
        }
        #endregion

        #region TrackingAndTracing -> TrackingAndTracingFacilityBooking

        private FacilityBooking _SelectedTrackingAndTracingFacilityBooking;
        /// <summary>
        /// Selected property for FacilityBooking
        /// </summary>
        /// <value>The selected TrackingAndTracingFacilityBooking</value>
        [ACPropertySelected(9999, "TrackingAndTracingFacilityBooking", "en{'TODO: TrackingAndTracingFacilityBooking'}de{'TODO: TrackingAndTracingFacilityBooking'}")]
        public FacilityBooking SelectedTrackingAndTracingFacilityBooking
        {
            get
            {
                return _SelectedTrackingAndTracingFacilityBooking;
            }
            set
            {
                if (_SelectedTrackingAndTracingFacilityBooking != value)
                {
                    _SelectedTrackingAndTracingFacilityBooking = value;
                    OnPropertyChanged("SelectedTrackingAndTracingFacilityBooking");
                }
            }
        }

        private List<FacilityBooking> _TrackingAndTracingFacilityBookingList;
        /// <summary>
        /// List property for FacilityBooking
        /// </summary>
        /// <value>The TrackingAndTracingFacilityBooking list</value>
        [ACPropertyList(9999, "TrackingAndTracingFacilityBooking")]
        public List<FacilityBooking> TrackingAndTracingFacilityBookingList
        {
            get
            {
                if (_TrackingAndTracingFacilityBookingList == null)
                    _TrackingAndTracingFacilityBookingList = new List<FacilityBooking>();
                var list = _TrackingAndTracingFacilityBookingList;
                switch (FilterLotTypes)
                {
                    case TandTLotPreviewEnum.All:
                        break;
                    case TandTLotPreviewEnum.Input:
                        list = _TrackingAndTracingFacilityBookingList.Where(x => x.InOrderPosID != null).ToList();
                        break;
                    case TandTLotPreviewEnum.Production:
                        list = _TrackingAndTracingFacilityBookingList.Where(x => x.ProdOrderPartslistPosID != null || x.ProdOrderPartslistPosRelationID != null).ToList();
                        break;
                    default:
                        break;
                }
                return list;
            }
        }

        #endregion

        #region TrackingAndTracing -> Preview Item

        [ACPropertyInfo(999)]
        public IACObjectEntity SelectedResult { get; set; }

        public IACObject CallerObject { get; set; }
        [ACPropertyInfo(9999, "TrackingAndTracingResult", "en{'Preview Design'}de{'Preview Design'}")]
        public string PreviewDesign
        {
            get
            {
                gip.core.datamodel.ACClassDesign acClassDesign = ACType.GetDesign(this, Global.ACUsages.DUMain, Global.ACKinds.DSDesignLayout, PreviewDesignName);
                string layoutXAML = null;
                if (acClassDesign != null && acClassDesign.ACIdentifier != "UnknowMainlayout")
                {
                    layoutXAML = acClassDesign.XMLDesign;
                }
                else
                {
                    layoutXAML = "<vb:VBDockPanel><vb:VBTextBox ACCaption=\"Unknown:\" Text=\"" + PreviewDesignName + "\"></vb:VBTextBox></vb:VBDockPanel>";
                }
                return layoutXAML;
            }
        }

        private string previewDesignTitle;
        [ACPropertyInfo(9999, "TrackingAndTracingResult", "en{'Title'}de{'Title'}")]
        public string PreviewDesignTitle
        {
            get
            {
                return previewDesignTitle;
            }
            set
            {
                if (previewDesignTitle != value)
                {
                    previewDesignTitle = value;
                    OnPropertyChanged("PreviewDesignTitle");
                }
            }
        }
        #endregion

        #endregion

        #region TrackingAndTracing -> Filter

        private TandTFilter _TandTFilter;
        [ACPropertyInfo(9999)]
        public TandTFilter TandTFilter
        {
            get
            {
                if (_TandTFilter == null)
                    _TandTFilter = new TandTFilter();
                return _TandTFilter;
            }
            set
            {
                _TandTFilter = value;
            }
        }

        public static readonly Func<IACObjectEntity, TandTFilter, bool> TandTFilterOnlyTime = new Func<IACObjectEntity, TandTFilter, bool>((item, filter) =>
        {
            bool isValid = true;
            IInsertInfo info = item as IInsertInfo;
            if (info != null)
                isValid = info.IsInPeriod(filter);
            return isValid;
        });


        #region TrackingAndTracing -> Filter -> FilterTree

        private bool _ShowFaciltiyBooking;
        [ACPropertyInfo(9999, "TrackingAndTracingResult", "en{'Stock movement'}de{'Lagerbewegung'}")]
        public bool ShowFaciltiyBooking
        {
            get
            {
                return _ShowFaciltiyBooking;
            }
            set
            {
                if (_ShowFaciltiyBooking != value)
                {
                    _ShowFaciltiyBooking = value;
                    OnPropertyChanged("ShowFaciltiyBooking");
                }
            }
        }

        private bool _ShowFacilityLot;
        [ACPropertyInfo(9999, "TrackingAndTracingResult", "en{'Lot'}de{'Los'}")]
        public bool ShowFacilityLot
        {
            get
            {
                return _ShowFacilityLot;
            }
            set
            {
                if (_ShowFacilityLot != value)
                {
                    _ShowFacilityLot = value;
                    OnPropertyChanged("ShowFacilityLot");
                }
            }
        }

        private bool _ShowFacilityCharge;
        [ACPropertyInfo(9999, "TrackingAndTracingResult", "en{'Quantum'}de{'Quant'}")]
        public bool ShowFacilityCharge
        {
            get
            {
                return _ShowFacilityCharge;
            }
            set
            {
                if (_ShowFacilityCharge != value)
                {
                    _ShowFacilityCharge = value;
                    OnPropertyChanged("ShowFacilityLot");
                }
            }
        }

        private bool _ShowDelivery;
        [ACPropertyInfo(9999, "TrackingAndTracingResult", "en{'Deliverynote'}de{'Eingangslieferschein'}")]
        public bool ShowDelivery
        {
            get
            {
                return _ShowDelivery;
            }
            set
            {
                if (_ShowDelivery != value)
                {
                    _ShowDelivery = value;
                    OnPropertyChanged("ShowDelivery");
                }
            }
        }

        private bool _ShowOrders;
        [ACPropertyInfo(9999, "TrackingAndTracingResult", "en{'Orders'}de{'Bestellungen'}")]
        public bool ShowOrders
        {
            get
            {
                return _ShowOrders;
            }
            set
            {
                if (_ShowOrders != value)
                {
                    _ShowOrders = value;
                    OnPropertyChanged("ShowOrders");
                }
            }
        }

        private bool _ShowProductionSteps;
        [ACPropertyInfo(9999, "TrackingAndTracingResult", "en{'Production'}de{'Produktion'}")]
        public bool ShowProductionSteps
        {
            get
            {
                return _ShowProductionSteps;
            }
            set
            {
                if (_ShowProductionSteps != value)
                {
                    _ShowProductionSteps = value;
                    OnPropertyChanged("ShowProductionSteps");
                }
            }
        }

        #endregion

        #region TrackingAndTracing -> Filter -> FilterItemTypes

        ACValueItem _SelectedFilterItemTypes;
        [ACPropertySelected(9999, "FilterItemTypes", "en{'Tracing Item'}de{'Rückverfolgende Element'}")]
        public ACValueItem SelectedFilterItemTypes
        {
            get
            {
                return _SelectedFilterItemTypes;
            }
            set
            {
                if (_SelectedFilterItemTypes != value)
                {
                    _SelectedFilterItemTypes = value;
                    OnPropertyChanged("SelectedFilterItemTypes");
                }
            }
        }

        public TandTStartPointEnum FilterItemType
        {
            get
            {
                if (SelectedFilterItemTypes == null) return TandTStartPointEnum.FacilityBooking;
                return (TandTStartPointEnum)Enum.Parse(typeof(TandTStartPointEnum), SelectedFilterItemTypes.Value.ToString());
            }
        }

        private ACValueItemList filterItemTypesList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        [ACPropertyList(9999, "FilterItemTypes")]
        public IEnumerable<ACValueItem> FilterItemTypesList
        {
            get
            {
                if (filterItemTypesList == null)
                {
                    filterItemTypesList = new ACValueItemList("FilterItemTypesList");
                    filterItemTypesList.AddEntry(TandTStartPointEnum.FacilityBooking, "en{'Facilitybooking'}de{'Lagerbewegung'}");
                    filterItemTypesList.AddEntry(TandTStartPointEnum.FacilityPreBooking, "en{'FacilityPreBooking'}de{'Gepl. Lagerbewegung'}");
                    filterItemTypesList.AddEntry(TandTStartPointEnum.InOrderPos, "en{'Inorder'}de{'Bestellung'}");
                    filterItemTypesList.AddEntry(TandTStartPointEnum.OutOrderPos, "en{'Outorder'}de{'Auftrag'}");
                    filterItemTypesList.AddEntry(TandTStartPointEnum.DeliveryNotePos, "en{'Delivery Note'}de{'Lieferschein'}");
                }
                return filterItemTypesList;
            }
        }

        #endregion

        #region TrackingAndTracing -> Filter -> FilterItemSequence

        ACValueItem _SelectedFilterItemSequence;
        [ACPropertySelected(9999, "FilterItemSequence", "en{'Tracing Item'}de{'Rückverfolgende Element'}")]
        public ACValueItem SelectedFilterItemSequence
        {
            get
            {
                return _SelectedFilterItemSequence;
            }
            set
            {
                if (_SelectedFilterItemSequence != value)
                {
                    _SelectedFilterItemSequence = value;
                    OnPropertyChanged("SelectedFilterItemSequence");
                }
            }
        }

        public Guid? FilterItemSequence
        {
            get
            {
                if (SelectedFilterItemSequence == null) return null;
                return new Guid(SelectedFilterItemSequence.Value.ToString());
            }
        }

        private ACValueItemList filterItemSequence = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        [ACPropertyList(9999, "FilterItemSequence")]
        public IEnumerable<ACValueItem> FilterItemSequenceList
        {
            get
            {
                if (filterItemSequence == null)
                {
                    filterItemSequence = new ACValueItemList("FilterItemSequenceList");
                    switch (FilterItemType)
                    {
                        case TandTStartPointEnum.FacilityBooking:
                            break;
                        case TandTStartPointEnum.FacilityPreBooking:
                            break;
                        case TandTStartPointEnum.InOrderPos:
                            if (FilterInOrderSelected != null)
                            {
                                List<InOrderPos> elements = FilterInOrderSelected.InOrderPos_InOrder.Where(x => x.ParentInOrderPosID == null).OrderBy(x => x.Sequence).ToList();
                                elements.ForEach(x => filterItemSequence.AddEntry(x.InOrderPosID, x.ACCaption));
                            }
                            break;
                        case TandTStartPointEnum.OutOrderPos:
                            if (FilterOutOrderSelected != null)
                            {
                                List<OutOrderPos> elements = FilterOutOrderSelected.OutOrderPos_OutOrder.Where(x => x.ParentOutOrderPosID == null).OrderBy(x => x.Sequence).ToList();
                                elements.ForEach(x => filterItemSequence.AddEntry(x.OutOrderPosID, x.ACCaption));
                            }
                            break;
                        case TandTStartPointEnum.DeliveryNotePos:
                            if (FilterDeliveryNoteSelected != null)
                            {
                                List<DeliveryNotePos> elements = FilterDeliveryNoteSelected.DeliveryNotePos_DeliveryNote.OrderBy(x => x.Sequence).ToList();
                                elements.ForEach(x => filterItemSequence.AddEntry(x.DeliveryNotePosID, x.ACCaption));
                            }
                            break;
                        default:
                            break;
                    }
                }
                return filterItemSequence;
            }
        }

        #endregion

        #region TrackingAndTracing -> Filter -> FilterLotTypes

        ACValueItem _SelectedFilterLotTypes;
        [ACPropertySelected(9999, "FilterLotTypes", "en{'Tracing Item'}de{'Rückverfolgende Element'}")]
        public ACValueItem SelectedFilterLotTypes
        {
            get
            {
                return _SelectedFilterLotTypes;
            }
            set
            {
                if (_SelectedFilterLotTypes != value)
                {
                    _SelectedFilterLotTypes = value;
                    OnPropertyChanged("SelectedFilterLotTypes");
                }
            }
        }

        public TandTLotPreviewEnum FilterLotTypes
        {
            get
            {
                if (SelectedFilterLotTypes == null) return TandTLotPreviewEnum.All;
                return (TandTLotPreviewEnum)Enum.Parse(typeof(TandTLotPreviewEnum), SelectedFilterLotTypes.Value.ToString());
            }
        }

        private ACValueItemList filterLotTypesList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        [ACPropertyList(9999, "FilterLotTypes")]
        public IEnumerable<ACValueItem> FilterLotTypesList
        {
            get
            {
                if (filterLotTypesList == null)
                {
                    filterLotTypesList = new ACValueItemList("FilterLotTypesList");
                    filterLotTypesList.AddEntry(TandTLotPreviewEnum.All, "en{'All'}de{'Alle'}");
                    filterLotTypesList.AddEntry(TandTLotPreviewEnum.Input, "en{'Input'}de{'Wareneingang'}");
                    filterLotTypesList.AddEntry(TandTLotPreviewEnum.Production, "en{'From Production'}de{'Aus Produktion'}");
                }
                return filterLotTypesList;
            }
        }

        #endregion

        #region TrackingAndTracing -> Filter -> Selected Items for Analyzing

        protected FacilityBooking _FilterFacilityBookingSelected;
        public virtual FacilityBooking FilterFacilityBookingSelected
        {
            get
            {
                return _FilterFacilityBookingSelected;
            }
            set
            {
                _FilterFacilityBookingSelected = value;
                OnPropertyChanged("FilterFacilityBookingSelected");
            }
        }

        public FacilityPreBooking FilterFacilityPreBookingSelected { get; set; }
        public InOrder FilterInOrderSelected { get; set; }
        public OutOrder FilterOutOrderSelected { get; set; }
        public DeliveryNote FilterDeliveryNoteSelected { get; set; }

        [ACPropertyInfo(9999, "FilterSearchNo", "en{'Number'}de{'Nummer'}")]
        public string FilterSearchNo { get; set; }

        #endregion

        #endregion

        #region  Methods

        #region Methods -> Search, Filter, Refresh

        [ACMethodInteraction("TTResult", "en{'Show'}de{'Anzeigen'}", (short)MISort.Search, true, "CurrentResult", Global.ACKinds.MSMethodPrePost)]
        public virtual void Search(bool retriveCallerObject = true)
        {
            if (retriveCallerObject)
                CallerObject = RetriveCallerObjectFromFilter();
            DefineFilterFunctions();
            QueryResult();
            FilterTree();
            OnPropertyChanged("CurrentResult");
            OnPropertyChanged("CurrentResultRoot");
        }

        public virtual void DefineFilterFunctions()
        {
            TandTFilter.FilterFunctions.Clear();
            if (TandTFilter.StartTime == null && TandTFilter.EndTime == null)
                return;
            TandTFilter.FilterFunctions.Add(TandTFilterOnlyTime);
        }

        public bool IsEnabledSearch()
        {
            return
                FilterFacilityBookingSelected != null ||
                FilterFacilityPreBookingSelected != null ||
                FilterInOrderSelected != null ||
                FilterOutOrderSelected != null ||
                FilterDeliveryNoteSelected != null;
        }

        private bool applyFilterInProgress;
        [ACMethodInteraction("TTResult", "en{'Show'}de{'Zeigen'}", (short)MISort.Search, true, "CurrentResult", Global.ACKinds.MSMethodPrePost)]
        public void ApplyFilter()
        {
            applyFilterInProgress = true;
            RetriveResult();
            FilterTree();
            OnPropertyChanged("CurrentResult");
            OnPropertyChanged("CurrentResultRoot");
            applyFilterInProgress = false;
        }

        [ACMethodCommand("TrackingAndTracing", "en{'Filter'}de{'Filter'}", (short)MISort.Search)]
        public virtual void Filter()
        {
            OnPropertyChanged("TrackingAndTracingChargeList");
            OnPropertyChanged("TrackingAndTracingFacilityBookingList");
        }

        /// <summary>
        /// Populate sub items presented in combo box by via form T and T of InOrder(Pos) and DeliveryNote(Pos) via InOrderNo and DeliveryNoteNo
        /// </summary>
        [ACMethodCommand("Filter", "en{'Refresh'}de{'Aktualisieren'}", (short)MISort.Search)]
        public void RefreshRelated()
        {
            if (!string.IsNullOrEmpty(FilterSearchNo))
            {
                switch (FilterItemType)
                {
                    case TandTStartPointEnum.FacilityBooking:
                        FilterFacilityBookingSelected = DatabaseApp.FacilityBooking.FirstOrDefault(x => x.FacilityBookingNo == FilterSearchNo);
                        break;
                    case TandTStartPointEnum.FacilityPreBooking:
                        FilterFacilityPreBookingSelected = DatabaseApp.FacilityPreBooking.FirstOrDefault(x => x.FacilityPreBookingNo == FilterSearchNo);
                        break;
                    case TandTStartPointEnum.InOrderPos:
                        FilterInOrderSelected = DatabaseApp.InOrder.FirstOrDefault(x => x.InOrderNo == FilterSearchNo);
                        break;
                    case TandTStartPointEnum.OutOrderPos:
                        FilterOutOrderSelected = DatabaseApp.OutOrder.FirstOrDefault(x => x.OutOrderNo == FilterSearchNo);
                        break;
                    case TandTStartPointEnum.DeliveryNotePos:
                        FilterDeliveryNoteSelected = DatabaseApp.DeliveryNote.FirstOrDefault(x => x.DeliveryNoteNo == FilterSearchNo);
                        break;
                    default:
                        break;
                }
                filterItemSequence = null;
                OnPropertyChanged("FilterItemSequenceList");
            }
        }

        #endregion

        #region Methods -> TrackingAndTracing

        [ACMethodInteraction("InwardFacilityPreBooking", "en{'Forward Track and Trace'}de{'Vorwärtsverfolgung'}", 9999, true, "SelectedTrackingAndTracingDeliveryNote")]
        public void DNSearchForward()
        {
            if (!IsEnabledDNSearchForward()) return;
            DeliveryNotePos dnp = DatabaseApp.DeliveryNotePos.FirstOrDefault(x => x.DeliveryNotePosID == SelectedTrackingAndTracingDeliveryNote.DeliveryNotePosID);
            if (dnp != null)
                TrackingAndTracingManager.SearchLot(this, dnp, GlobalApp.TrackingAndTracingSearchModel.Forward, TandTFilter);
        }

        private bool IsEnabledDNSearchForward()
        {
            return SelectedTrackingAndTracingDeliveryNote != null;
        }

        #endregion

        #region Methods -> TandTv2

        [ACMethodInteraction("InwardFacilityPreBooking", "en{'Forward Track and Trace (2)'}de{'Vorwärtsverfolgung (2)'}", 9999, true, "SelectedTrackingAndTracingDeliveryNote")]
        public void DNSearchForward2()
        {
            if (!IsEnabledDNSearchForward2()) return;
            DeliveryNotePos dnp = DatabaseApp.DeliveryNotePos.FirstOrDefault(x => x.DeliveryNotePosID == SelectedTrackingAndTracingDeliveryNote.DeliveryNotePosID);
            if (dnp == null) return;
            TandTv2Job job = new TandTv2Job();
            job.ItemSystemNo = dnp.Sequence.ToString();
            job.PrimaryKeyID = dnp.DeliveryNotePosID;
            job.ItemTypeEnum = TandTv2ItemTypeEnum.DeliveryNotePos;
            job.TrackingStyleEnum = TandTv2TrackingStyleEnum.Forward;
            TandT2Manager.StartTandTBSO(this, job);
        }

        private bool IsEnabledDNSearchForward2()
        {
            return SelectedTrackingAndTracingDeliveryNote != null;
        }

        #endregion

        #region TrackingAndTracing -> Methods -> IsEnabled

        public bool IsEnabledTrackingAndTracingDialogOk()
        {
            return CallerObject != null;
        }

        public bool IsEnabledApplyFilter()
        {
            return Result != null && Result.RootPoint != null && !applyFilterInProgress;
        }

        #endregion

        #region TrackingAndTracing -> Methods -> Build a Result

        /// <summary>
        /// For independent usage form without predefined Caller Object
        /// Using Item number and selected position (InOrder, OutOrder, DeliveryNote) for retrive objects 
        /// where we search T and T: FacilityBooking, FacilityPreBooking, InOrderPos, OutOrderPos, DeliveryNotePos
        /// </summary>
        private IACObject RetriveCallerObjectFromFilter()
        {
            IACObject caller = null;
            switch (FilterItemType)
            {
                case TandTStartPointEnum.FacilityBooking:
                    caller = FilterFacilityBookingSelected;
                    break;
                case TandTStartPointEnum.FacilityPreBooking:
                    caller = FilterFacilityPreBookingSelected;
                    break;
                case TandTStartPointEnum.InOrderPos:
                    if (FilterInOrderSelected != null && FilterItemSequence.HasValue)
                    {
                        InOrderPos iop = DatabaseApp.InOrderPos.FirstOrDefault(x => x.InOrderPosID == (FilterItemSequence ?? Guid.Empty));
                        caller = iop;
                    }
                    break;
                case TandTStartPointEnum.OutOrderPos:
                    if (FilterOutOrderSelected != null && FilterItemSequence.HasValue)
                    {
                        OutOrderPos ops = DatabaseApp.OutOrderPos.FirstOrDefault(x => x.OutOrderPosID == (FilterItemSequence ?? Guid.Empty));
                        caller = ops;
                    }
                    break;
                case TandTStartPointEnum.DeliveryNotePos:
                    if (FilterDeliveryNoteSelected != null && FilterItemSequence.HasValue)
                    {
                        DeliveryNotePos dnp = DatabaseApp.DeliveryNotePos.FirstOrDefault(x => x.DeliveryNotePosID == (FilterItemSequence ?? Guid.Empty));
                        caller = dnp;
                    }
                    break;
                default:
                    break;
            }
            return caller;
        }

        /// <summary>
        ///  Retrieve result tree from T and T layer
        /// </summary>
        private void RetriveResult()
        {
            Result = null;
            try
            {
                if (CallerObject is FacilityBooking)
                    Result = TrackingAndTracingManager.TrackingAndTracing(DatabaseApp, CallerObject as FacilityBooking, SearchModelValue, TandTFilter);

                if (CallerObject is FacilityPreBooking)
                    Result = TrackingAndTracingManager.TrackingAndTracing(DatabaseApp, CallerObject as FacilityPreBooking, SearchModelValue, TandTFilter);


                if (CallerObject is InOrderPos)
                    Result = TrackingAndTracingManager.TrackingAndTracing(DatabaseApp, CallerObject as InOrderPos, SearchModelValue, TandTFilter);

                if (CallerObject is OutOrderPos)
                    Result = TrackingAndTracingManager.TrackingAndTracing(DatabaseApp, CallerObject as OutOrderPos, SearchModelValue, TandTFilter);


                if (CallerObject is DeliveryNotePos)
                    Result = TrackingAndTracingManager.TrackingAndTracing(DatabaseApp, CallerObject as DeliveryNotePos, (GlobalApp.TrackingAndTracingSearchModel)((short)SearchModel.Value), TandTFilter);


                if (CallerObject is ProdOrderPartslistPos)
                    Result = TrackingAndTracingManager.TrackingAndTracing(DatabaseApp, CallerObject as ProdOrderPartslistPos, (GlobalApp.TrackingAndTracingSearchModel)((short)SearchModel.Value), TandTFilter);

                if (CallerObject is FacilityCharge)
                    Result = TrackingAndTracingManager.TrackingAndTracing(DatabaseApp, CallerObject as FacilityCharge, (GlobalApp.TrackingAndTracingSearchModel)((short)SearchModel.Value), TandTFilter);

                if (CallerObject is FacilityLot)
                    Result = TrackingAndTracingManager.TrackingAndTracing(DatabaseApp, CallerObject as FacilityLot, (GlobalApp.TrackingAndTracingSearchModel)((short)SearchModel.Value), TandTFilter);

            }
            catch (Exception ec)
            {
                string message = string.Format("CallerObject: {0}, Error: {1}", CallerObject.ACCaption, ec.Message);
                Messages.LogError(this.GetACUrl(), "RetriveResult()", message);
            }

            CurrentResultRoot = null;
            CurrentResultRoot = Result.RootPoint;
        }

        /// <summary>
        /// Preview result on from
        /// </summary>
        public void QueryResult()
        {
            RetriveResult();
            if (Result != null)
            {
                LoadExtractedList(Result);
            }
        }

        public virtual void LoadExtractedList(TrackingAndTracingResult result)
        {
            _TrackingAndTracingChargeList = result.FacilityChargeModelList;
            if (_TrackingAndTracingChargeList != null)
                SelectedTrackingAndTracingCharge = _TrackingAndTracingChargeList.FirstOrDefault();
            OnPropertyChanged("TrackingAndTracingChargeList");

            _TrackingAndTracingDeliveryNoteList = Result.DeliveryNoteList;
            if (_TrackingAndTracingDeliveryNoteList != null)
                SelectedTrackingAndTracingDeliveryNote = _TrackingAndTracingDeliveryNoteList.FirstOrDefault();
            OnPropertyChanged("TrackingAndTracingDeliveryNoteList");

            _TrackingAndTracingFacilityBookingList = Result.FacilityBookingList;
            if (_TrackingAndTracingFacilityBookingList != null)
                SelectedTrackingAndTracingFacilityBooking = _TrackingAndTracingFacilityBookingList.FirstOrDefault();
            OnPropertyChanged("TrackingAndTracingFacilityBookingList");

            LoadLabOrderList();
        }

        private void LoadLabOrderList()
        {
            _TandTLabOrderList = null;
            if (Result.ItemsWithLabOrder != null && Result.ItemsWithLabOrder.Any())
            {
                _TandTLabOrderList = new List<TandTItemWithLabOrder>();

                foreach (var item in Result.ItemsWithLabOrder.OrderBy(x => ((IInsertInfo)x.Related).InsertDate))
                {
                    TandTItemWithLabOrder labListItem = new TandTItemWithLabOrder();
                    labListItem.Name = item.ACCaption;
                    labListItem.Related = item.Related;
                    Type itemType = item.Related.GetType();
                    ACClassInfo acClassInfo = (ACClassInfo)(itemType.GetCustomAttributes(typeof(ACClassInfo), false)[0]);
                    labListItem.TypeName = Translator.GetTranslation(acClassInfo.ACCaptionTranslation);
                    labListItem.InsertDate = ((IInsertInfo)item.Related).InsertDate;
                    _TandTLabOrderList.Add(labListItem);
                }
                if (_TandTLabOrderList != null && _TandTLabOrderList.Any())
                {
                    SelectedTandTLabOrder = _TandTLabOrderList.FirstOrDefault();
                }
            }
            OnPropertyChanged("TandTLabOrderList");
        }

        /// <summary>
        /// Setup filter content to be well populated 
        /// based on outer CallerObject input
        /// Search by booth methods shuld be same
        /// </summary>
        /// <param name="CallerObject"></param>
        private void FilterPreselect(IACObject CallerObject)
        {
            switch (CallerObject.GetType().Name)
            {
                case "FacilityBooking":
                    FacilityBooking fb = CallerObject as FacilityBooking;
                    SelectedFilterItemTypes = FilterItemTypesList.FirstOrDefault(x => x.Value.ToString() == TandTStartPointEnum.FacilityBooking.ToString());
                    FilterSearchNo = fb.FacilityBookingNo;
                    filterItemSequence = null;
                    FilterFacilityBookingSelected = fb;
                    OnPropertyChanged("FilterItemSequenceList");
                    break;
                case "FacilityPreBooking":
                    FacilityPreBooking fpb = CallerObject as FacilityPreBooking;
                    SelectedFilterItemTypes = FilterItemTypesList.FirstOrDefault(x => x.Value.ToString() == TandTStartPointEnum.FacilityPreBooking.ToString());
                    FilterSearchNo = fpb.FacilityPreBookingNo;
                    filterItemSequence = null;
                    OnPropertyChanged("FilterItemSequenceList");
                    break;
                case "InOrderPos":
                    InOrderPos iop = CallerObject as InOrderPos;
                    SelectedFilterItemTypes = FilterItemTypesList.FirstOrDefault(x => x.Value.ToString() == TandTStartPointEnum.InOrderPos.ToString());
                    FilterInOrderSelected = iop.InOrder;
                    FilterSearchNo = iop.InOrder.InOrderNo;
                    filterItemSequence = null;
                    OnPropertyChanged("FilterItemSequenceList");
                    SelectedFilterItemSequence = FilterItemSequenceList.FirstOrDefault(x => x.Value.ToString() == iop.InOrderPosID.ToString());
                    break;
                case "OutOrderPos":
                    OutOrderPos oop = CallerObject as OutOrderPos;
                    SelectedFilterItemTypes = FilterItemTypesList.FirstOrDefault(x => x.Value.ToString() == TandTStartPointEnum.OutOrderPos.ToString());
                    FilterOutOrderSelected = oop.OutOrder;
                    FilterSearchNo = oop.OutOrder.OutOrderNo;
                    filterItemSequence = null;
                    OnPropertyChanged("FilterItemSequenceList");
                    SelectedFilterItemSequence = FilterItemSequenceList.FirstOrDefault(x => x.Value.ToString() == oop.OutOrderPosID.ToString());
                    break;
                case "DeliveryNotePos":
                    DeliveryNotePos dnp = CallerObject as DeliveryNotePos;
                    SelectedFilterItemTypes = FilterItemTypesList.FirstOrDefault(x => x.Value.ToString() == TandTStartPointEnum.DeliveryNotePos.ToString());
                    FilterDeliveryNoteSelected = dnp.DeliveryNote;
                    FilterSearchNo = dnp.DeliveryNote.DeliveryNoteNo;
                    filterItemSequence = null;
                    OnPropertyChanged("FilterItemSequenceList");
                    SelectedFilterItemSequence = FilterItemSequenceList.FirstOrDefault(x => x.Value.ToString() == dnp.DeliveryNotePosID.ToString());
                    break;
            }
        }

        /// <summary>
        /// Apply filter on result
        /// </summary>
        private void FilterTree()
        {
            if (ShowFaciltiyBooking && ShowFacilityLot && ShowFacilityCharge && ShowDelivery && ShowOrders && ShowProductionSteps) return;
            List<string> filter = new List<string>();

            if (ShowFaciltiyBooking)
            {
                filter.Add(typeof(FacilityBooking).Name);
                filter.Add(typeof(FacilityPreBooking).Name);
                filter.Add(typeof(FacilityBookingCharge).Name);
            }

            if (ShowFacilityLot)
            {
                filter.Add(typeof(FacilityLot).Name);
            }

            if (ShowFacilityCharge)
            {
                filter.Add(typeof(FacilityCharge).Name);
            }

            if (ShowDelivery)
            {
                filter.Add(typeof(DeliveryNote).Name);
                filter.Add(typeof(DeliveryNotePos).Name);
            }

            if (ShowOrders)
            {
                filter.Add(typeof(InOrder).Name);
                filter.Add(typeof(InOrderPos).Name);
                filter.Add(typeof(OutOrder).Name);
                filter.Add(typeof(OutOrderPos).Name);
            }

            if (ShowProductionSteps)
            {
                filter.Add(typeof(ProdOrderPartslistPos).Name);
                filter.Add(typeof(ProdOrderPartslistPosRelation).Name);
            }

            Result.Filter = filter;
            Result.ApplyFilter();
        }

        #endregion

        #endregion

        #region TrackingAndTracing -> Helper, helper methods

        private void SetupPreviewDesignNameAndTitle(TrackingAndTracingPoint CurrentResult)
        {

            PreviewDesignName = "view_" + CurrentResult.Related.GetType().Name;
            switch (CurrentResult.Related.GetType().Name)
            {
                case "ProdOrderPartslistPos":
                    ProdOrderPartslistPos item = CurrentResult.Related as ProdOrderPartslistPos;
                    if (item.MaterialPosType == GlobalApp.MaterialPosTypes.OutwardRoot)
                    {
                        PreviewDesignTitle = Root.Environment.TranslateText(this, "lblProdOrderPartslistPos");
                    }
                    else
                    {
                        if (item.ProdOrderPartslist.Partslist.Material == item.BookingMaterial)
                        {
                            PreviewDesignTitle = Root.Environment.TranslateText(this, "lblProdOrderPartslistPosEndProduct");
                        }
                        else
                        {
                            PreviewDesignTitle = Root.Environment.TranslateText(this, "lblProdOrderPartslistPosMixure");
                        }
                    }
                    break;
            }
            OnPropertyChanged("PreviewDesign");
        }

        #endregion

        #endregion

        #region Position Preview

        [ACPropertyInfo(9999)]
        public bool InwardFacilityPreBookingVisibility
        {
            get
            {
                if (SelectedResult == null || !(SelectedResult is FacilityPreBooking)) return false;
                return (SelectedResult as FacilityPreBooking).InwardMaterial != null;
            }
        }

        [ACPropertyInfo(9999)]
        public bool OutwardFacilityPreBookingVisibility
        {
            get
            {
                return !InwardFacilityPreBookingVisibility;
            }
        }

        [ACPropertyInfo(9999)]
        public bool InwardFacilityBookingVisibility
        {
            get
            {
                if (SelectedResult == null || !(SelectedResult is FacilityBooking)) return false;
                return (SelectedResult as FacilityBooking).InwardMaterialID != null;
            }
        }

        [ACPropertyInfo(9999)]
        public bool OutwardFacilityBookingVisibility
        {
            get
            {
                return !InwardFacilityBookingVisibility;
            }
        }

        #endregion

        #region LabOrder

        // TandTLabOrder - this is special list - extracted elements IACObjectEntity with LabOrders
        #region LabOrder -> TandTLabOrder -> Select, (Current,) List

        #region TandTLabOrder
        private TandTItemWithLabOrder _SelectedTandTLabOrder;
        /// <summary>
        /// Selected property for IACObjectEntity
        /// </summary>
        /// <value>The selected TandTLabOrder</value>
        [ACPropertySelected(9999, "TandTLabOrder", "en{'TODO: TandTLabOrder'}de{'TODO: TandTLabOrder'}")]
        public TandTItemWithLabOrder SelectedTandTLabOrder
        {
            get
            {
                return _SelectedTandTLabOrder;
            }
            set
            {
                if (_SelectedTandTLabOrder != value)
                {
                    _SelectedTandTLabOrder = value;
                    IACObjectEntity related = null;
                    if (value != null)
                        related = value.Related;
                    LoadLabOrderList(related);
                    OnPropertyChanged("SelectedTandTLabOrder");
                }
            }
        }

        private List<TandTItemWithLabOrder> _TandTLabOrderList;
        /// <summary>
        /// List property for IACObjectEntity
        /// </summary>
        /// <value>The TandTLabOrder list</value>
        [ACPropertyList(9999, "TandTLabOrder")]
        public IEnumerable<TandTItemWithLabOrder> TandTLabOrderList
        {
            get
            {
                if (_TandTLabOrderList == null)
                    _TandTLabOrderList = new List<TandTItemWithLabOrder>();
                return _TandTLabOrderList;
            }
        }
        #endregion


        #endregion

        #region LabOrder -> Selected, (Current,) List

        LabOrder _SelectedLabOrder;
        [ACPropertySelected(9999, "LabOrder")]
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
                    LoadLabOrderPosList();
                    OnPropertyChanged("SelectedLabOrder");
                }
            }
        }

        private IEnumerable<LabOrder> labOrderList;
        [ACPropertyList(9999, "LabOrder")]
        public IEnumerable<LabOrder> LabOrderList
        {
            get
            {
                if (labOrderList == null)
                    labOrderList = new List<LabOrder>();
                return labOrderList;
            }
        }

        #endregion

        #region LabOrderPos -> Selected, (Current,) List

        LabOrderPos _SelectedLabOrderPos;
        [ACPropertySelected(9999, "LabOrderPos")]
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

        [ACPropertyList(9999, "LabOrderPos")]
        public IEnumerable<LabOrderPos> LabOrderPosList
        {
            get
            {
                if (SelectedLabOrder == null) return null;
                return SelectedLabOrder.LabOrderPos_LabOrder.OrderBy(x => x.Sequence);
            }
        }

        private void LoadLabOrderPosList()
        {
            if (SelectedLabOrder != null)
                SelectedLabOrderPos = LabOrderPosList.FirstOrDefault();
            else
                SelectedLabOrderPos = null;

            OnPropertyChanged("LabOrderPosList");
        }

        #endregion

        #region LabOrder -> Methods

        protected void LoadLabOrderList(IACObjectEntity item)
        {
            labOrderList = null;
            if (item != null)
            {
                Type relatedItemType = item.GetType();
                switch (relatedItemType.Name)
                {
                    case "ProdOrderPartslistPos":
                        ProdOrderPartslistPos posItem = item as ProdOrderPartslistPos;
                        labOrderList = posItem.LabOrder_ProdOrderPartslistPos.OrderBy(x => x.SampleTakingDate);
                        break;
                    case "InOrderPos":
                        InOrderPos inOrderPos = item as InOrderPos;
                        labOrderList = inOrderPos.LabOrder_InOrderPos.OrderBy(x => x.SampleTakingDate);
                        break;
                    case "OutOrderPos":
                        OutOrderPos outOrderPos = item as OutOrderPos;
                        labOrderList = outOrderPos.LabOrder_OutOrderPos.OrderBy(x => x.SampleTakingDate);
                        break;
                    case "FacilityLot":
                        FacilityLot lot = item as FacilityLot;
                        labOrderList = lot.LabOrder_FacilityLot.OrderBy(x => x.SampleTakingDate);
                        break;
                }
                if (labOrderList != null && labOrderList.Any())
                    SelectedLabOrder = labOrderList.FirstOrDefault();
                else
                    SelectedLabOrder = null;
            }
            OnPropertyChanged("LabOrderPosList");
        }

        [ACMethodInteraction("TrackingAndTracingDeliveryNote", "en{'Show LabOrder'}de{'Zeige Laborauftrag'}", 50, false)]
        public virtual void ShowLaborInfoForDn()
        {
            if (IsEnabledShowLaborInfoForDn())
            {
                DeliveryNotePos dnp = DatabaseApp.DeliveryNotePos.Where(x => x.DeliveryNotePosID == SelectedTrackingAndTracingDeliveryNote.DeliveryNotePosID).FirstOrDefault();
                if (dnp.InOrderPosID != null)
                {
                    LoadLabOrderList(dnp.InOrderPos);
                }
                if (dnp.OutOrderPosID != null)
                {
                    LoadLabOrderList(dnp.OutOrderPos);
                }
                ShowDialog(this, "LaborDlg");
            }
        }

        public bool IsEnabledShowLaborInfoForDn()
        {
            return SelectedTrackingAndTracingDeliveryNote != null;
        }
        #endregion

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"Search":
                    Search(acParameter.Count() == 1 ? (Boolean)acParameter[0] : true);
                    return true;
                case"IsEnabledSearch":
                    result = IsEnabledSearch();
                    return true;
                case"ApplyFilter":
                    ApplyFilter();
                    return true;
                case"Filter":
                    Filter();
                    return true;
                case"RefreshRelated":
                    RefreshRelated();
                    return true;
                case"DNSearchForward":
                    DNSearchForward();
                    return true;
                case"IsEnabledTrackingAndTracingDialogOk":
                    result = IsEnabledTrackingAndTracingDialogOk();
                    return true;
                case"IsEnabledApplyFilter":
                    result = IsEnabledApplyFilter();
                    return true;
                case"ShowLaborInfoForDn":
                    ShowLaborInfoForDn();
                    return true;
                case"IsEnabledShowLaborInfoForDn":
                    result = IsEnabledShowLaborInfoForDn();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
