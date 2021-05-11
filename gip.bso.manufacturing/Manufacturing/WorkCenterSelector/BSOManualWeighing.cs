using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using vd = gip.mes.datamodel;
using System.Text;
using System.Threading.Tasks;
using gip.mes.processapplication;
using System.ComponentModel;
using System.Data;
using gip.core.processapplication;
using System.Threading;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Manual weighing'}de{'Handverwiegung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, SortIndex = 100)]
    public class BSOManualWeighing : BSOWorkCenterChild
    {
        #region c'tors

        public BSOManualWeighing(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            var result = base.ACInit(startChildMode);
            if (!result)
                return result;

            MainSyncContext = SynchronizationContext.Current;

            using (ACMonitor.Lock(_20015_LockValue))
            {
                _ApplicationQueue = new ACDelegateQueue(this.GetACUrl() + ";AppQueue");
                _ApplicationQueue.StartWorkerThread();
            }

            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_ApplicationQueue != null)
            {
                _ApplicationQueue.StopWorkerThread();
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _ApplicationQueue = null;
                }
            }

            DeactivateManualWeighingModel();

            _DefaultMaterialIcon = null;
            _MScaleWFNodes = null;

            MainSyncContext = null;

            return base.ACDeInit(deleteACClassTask);
        }

        public const string ClassName = "BSOManualWeighing";

        #endregion

        #region Properties

        #region Database

        private vd.DatabaseApp _DatabaseApp;
        public override vd.DatabaseApp DatabaseApp
        {
            get
            {
                if (_DatabaseApp == null)
                    _DatabaseApp = ACObjectContextManager.GetOrCreateContext<vd.DatabaseApp>(this.GetACUrl());
                return _DatabaseApp;
            }
        }

        #endregion

        internal SynchronizationContext MainSyncContext;

        private ACDelegateQueue _ApplicationQueue;

        public ACDelegateQueue ApplicationQueue
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _ApplicationQueue;
                }
            }
        }

        private ACComponent _CurrentProcessModule;
        [ACPropertyInfo(601)]
        public ACComponent CurrentProcessModule
        {
            get => _CurrentProcessModule;
            set
            {
                DeactivateManualWeighingModel();
                _CurrentProcessModule = value;
                OnPropertyChanged("CurrentProcessModule");
                if (CurrentProcessModule != null)
                    ActivateManualWeighingModel();
            }
        }

        private ACComponent CurrentProcessFunction
        {
            get;
            set;
        }

        private ACClassDesign _DefaultMaterialIcon;
        internal ACClassDesign DefaultMaterialIcon
        {
            get
            {
                if (_DefaultMaterialIcon == null)
                    _DefaultMaterialIcon = this.GetDesign("MaterialIcon");
                return _DefaultMaterialIcon;
            }
        }

        private string _AlarmsAsTextCache = null;


        #region Private fields

        private bool _CallPWLotChange = false, _IsLotConsumed = false, _StartWeighingFromF_FC = false;

        private object _UserAckNodeLock = new object(), _PWNodeACStateLock = new object(), _LoadWFNodeLock = new object();

        private static ACClass[] _PWUserAckClasses;
        public static ACClass[] PWUserAckClasses
        {
            get
            {
                if (_PWUserAckClasses == null)
                {
                    var acClass = gip.core.datamodel.Database.GlobalDatabase.GetACType(typeof(PWNodeUserAck));
                    if (acClass != null)
                    {
                        var derivedClasses = acClass.DerivedClasses;
                        derivedClasses.Add(acClass);
                        _PWUserAckClasses = derivedClasses.ToArray();
                    }
                }
                return _PWUserAckClasses;
            }
        }

        private IACContainerT<string> _OrderInfo;
        private IACContainerT<List<ACChildInstanceInfo>> _WFNodes;
        private IACContainerT<bool> _ScaleHasAlarms;
        private IACContainerTNet<string> _AlarmsAsText;
        private IACContainerT<WeighingComponentInfo> _WeighingComponentInfo;
        //private List<IACContainerT<ACStateEnum>> _PWNodeACState;
        private IACContainerT<double> _ScaleActualWeight;

        #endregion

        #region Properties => ProcessFunction

        private IACContainerT<ACMethod> _PAFCurrentACMethod;

        private ACRef<IACComponent> _CurrentPAFManualWeighing;

        private IACContainerT<double> _PAFManuallyAddedQuantity;

        private IACContainerT<short> _TareScaleState;

        [ACPropertyInfo(602)]
        public IACComponent CurrentPAFManualWeighing
        {
            get => _CurrentPAFManualWeighing?.ValueT;
        }

        #endregion

        #region Properties => ScaleObjects

        ACRef<IACComponent>[] _ProcessModuleScales;

        public ACValueItem _CurrentScaleObject;
        [ACPropertyCurrent(603, "ScaleObject")]
        public ACValueItem CurrentScaleObject
        {
            get => _CurrentScaleObject;
            set
            {
                _CurrentScaleObject = value;
                if (value != null)
                {
                    DeactivateScale();
                    var scaleRef = _ProcessModuleScales?.FirstOrDefault(c => c.ACUrl == _CurrentScaleObject.Value.ToString());
                    if (scaleRef != null)
                        ActivateScale(scaleRef);
                }
                else
                    DeactivateScale();

                OnPropertyChanged("CurrentScaleObject");
            }
        }

        private List<ACValueItem> _ScaleObjectsList;
        [ACPropertyList(604, "ScaleObject")]
        public List<ACValueItem> ScaleObjectsList
        {
            get
            {
                //if (_ScaleObjectsList == null && _ProcessModuleScales != null && _ProcessModuleScales.Any())
                //{
                //    _ScaleObjectsList = new List<ACValueItem>(_ProcessModuleScales.Select(c => new ACValueItem(c.ValueT.ACCaption, c.ACUrl, null)));
                //}
                return _ScaleObjectsList;
            }
            set
            {
                _ScaleObjectsList = value;
                OnPropertyChanged("ScaleObjectsList");
            }
        }

        /// <summary>
        /// Represents the sum of ScaleAddAcutalWeight and Scale RealWeight
        /// </summary>
        [ACPropertyInfo(605)]
        public virtual double ScaleActualWeight
        {
            get => ScaleAddAcutalWeight + ScaleRealWeight;
        }

        protected double _ScaleAddAcutalWeight;
        /// <summary>
        /// The weight which is manually added from sack or etc.
        /// </summary>
        public virtual double ScaleAddAcutalWeight
        {
            get => _ScaleAddAcutalWeight;
            set
            {
                _ScaleAddAcutalWeight = value;
                ScaleBckgrState = DetermineBackgroundState(_TolerancePlus, _ToleranceMinus, TargetWeight, value + ScaleRealWeight);
                OnPropertyChanged("ScaleActualWeight");
                OnPropertyChanged("ScaleDifferenceWeight");
            }
        }

        private double _ScaleRealWeight;
        /// <summary>
        /// The weight which is really in a pyhisical scale.
        /// </summary>
        public double ScaleRealWeight
        {
            get => _ScaleRealWeight;
            set
            {
                _ScaleRealWeight = value;
                ScaleBckgrState = DetermineBackgroundState(_TolerancePlus, _ToleranceMinus, TargetWeight, value + ScaleAddAcutalWeight);
                OnPropertyChanged("ScaleActualWeight");
                OnPropertyChanged("ScaleDifferenceWeight");
            }
        }

        private double _TargetWeight;
        [ACPropertyInfo(606)]
        public double TargetWeight
        {
            get => _TargetWeight;
            set
            {
                _TargetWeight = value;
                OnPropertyChanged("TargetWeight");
                OnPropertyChanged("ScaleDifferenceWeight");
            }
        }

        [ACPropertyInfo(607)]
        public double ScaleDifferenceWeight
        {
            get
            {
                return ScaleActualWeight - TargetWeight;
            }
        }

        private ScaleBackgroundState _ScaleBckgrState;
        public ScaleBackgroundState ScaleBckgrState
        {
            get => _ScaleBckgrState;
            set
            {
                _ScaleBckgrState = value;
                OnPropertyChanged("ScaleBckgrState");
            }
        }

        #endregion

        #region Properties => WFNodes

        private string _CurrentOrderInfoValue;
        private object _CurrentOrderInfoValueLock = new object();

        [ACPropertyInfo(608)]
        public ACRef<IACComponentPWNode> ComponentPWNode
        {
            get;
            set;
        }

        [ACPropertyInfo(609)]
        public IACComponentPWNode CurrentComponentPWNode
        {
            get => ComponentPWNode?.ValueT;
        }

        public List<ManualWeighingPWNode> ComponentPWNodesList
        {
            get;
            set;
        }

        #endregion

        #region Properties => Messages

        private List<ACChildInstanceInfo> _MScaleWFNodes;
        public List<ACChildInstanceInfo> MScaleWFNodes
        {
            get => _MScaleWFNodes;
            set
            {
                if (_UserAckNodeLock == null)
                    return;

                lock (_UserAckNodeLock)
                {
                    if (_MScaleWFNodes != value)
                    {
                        _MScaleWFNodes = value;
                        HandleWFNodes(_MScaleWFNodes);
                    }
                }
            }
        }

        private List<MessageItem> _MessagesList = new List<MessageItem>();
        [ACPropertyList(610, "Messages")]
        public List<MessageItem> MessagesList
        {
            get
            {
                return _MessagesList;
            }
            set
            {
                _MessagesList = value;
                OnPropertyChanged("MessagesList");
            }
        }

        //private MessageItem _SelectedMessage;
        [ACPropertySelected(611, "Messages")]
        public MessageItem SelectedMessage
        {
            get;
            set;
        }

        #endregion

        #region Properties => PW&PAFConfiguration

        protected double _TolerancePlus = 0, _ToleranceMinus = 0;

        internal bool WeighingMaterialsFSM
        {
            get;
            set;
        }

        private bool _EnterLotManually = false;
        [ACPropertyInfo(612)]
        public bool EnterLotManually
        {
            get => _EnterLotManually;
            set
            {
                _EnterLotManually = value;
                OnPropertyChanged("EnterLotManually");
            }
        }

        #endregion

        #region Properties => OrderInfo

        private vd.ProdOrderPartslistPos _EndBatchPos;
        [ACPropertyInfo(613)]
        public vd.ProdOrderPartslistPos EndBatchPos
        {
            get => _EndBatchPos;
            set
            {
                _EndBatchPos = value;
                //if (_EndBatchPos == null)
                //{
                //    ProdOrderProgramNo = null;
                //    BatchSeqNo = null;
                //    ProdOrderBatchNo = null;
                //    EBPMaterialName = null;
                //    EBPMaterialNo = null;
                //}
                //else
                //{
                //    ProdOrderProgramNo = _EndBatchPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                //    BatchSeqNo = _EndBatchPos.ProdOrderBatch?.BatchSeqNo;
                //    ProdOrderBatchNo = _EndBatchPos.ProdOrderBatch?.ProdOrderBatchNo;
                //    EBPMaterialName = _EndBatchPos.BookingMaterial.MaterialNo + "  " +  _EndBatchPos.BookingMaterial.MaterialName1;
                //    //EBPMaterialNo = _EndBatchPos.BookingMaterial.MaterialNo;
                //}
                OnPropertyChanged("EndBatchPos");
            }
        }

        //private string _ProdOrderProgramNo;
        //[ACPropertyInfo(614, "", "en{'Order Number'}de{'Auftragsnummer'}")]
        //public string ProdOrderProgramNo
        //{
        //    get => _ProdOrderProgramNo;
        //    set
        //    {
        //        _ProdOrderProgramNo = value;
        //        OnPropertyChanged("ProdOrderProgramNo");
        //    }
        //}

        //private int? _BatchSeqNo;
        //[ACPropertyInfo(615, "", "en{'Batch'}de{'Batch'}")]
        //public int? BatchSeqNo
        //{
        //    get => _BatchSeqNo;
        //    set
        //    {
        //        _BatchSeqNo = value;
        //        OnPropertyChanged("BatchSeqNo");
        //    }
        //}

        //private string _ProdOrderBatchNo;
        //[ACPropertyInfo(616, "", "en{'Batch-No.'}de{'Batch-Nr.'}")]
        //public string ProdOrderBatchNo
        //{
        //    get => _ProdOrderBatchNo;
        //    set
        //    {
        //        _ProdOrderBatchNo = value;
        //        OnPropertyChanged("ProdOrderBatchNo");
        //    }
        //}

        //private string _EBPMaterialName;
        //[ACPropertyInfo(617, "", "en{'Material'}de{'Material'}")]
        //public string EBPMaterialName
        //{
        //    get => _EBPMaterialName;
        //    set
        //    {
        //        _EBPMaterialName = value;
        //        OnPropertyChanged("EBPMaterialName");
        //    }
        //}

        //private string _EBPMaterialNo;
        //[ACPropertyInfo(618, "", "en{'Material No.'}de{'Material-Nr.'}")]
        //public string EBPMaterialNo
        //{
        //    get => _EBPMaterialNo;
        //    set
        //    {
        //        _EBPMaterialNo = value;
        //        OnPropertyChanged("EBPMaterialNo");
        //    }
        //}

        #endregion

        #region Properties => Misc.

        [ACPropertyInfo(619, "", "en{'Module ACUrl'}de{'Module ACUrl'}")]
        public string PAProcessModuleACUrl
        {
            get;
            set;
        }

        [ACPropertyInfo(619, "", "en{'Module'}de{'Modul'}")]
        public string PAProcessModuleACCaption
        {
            get;
            set;
        }

        private bool _IsBinChangeAvailable = false;
        [ACPropertyInfo(620)]
        public bool IsBinChangeAvailable
        {
            get => _IsBinChangeAvailable;
            set
            {
                _IsBinChangeAvailable = value;
                OnPropertyChanged("IsBinChangeAvailable");
            }
        }

        private bool _BtnWeighBlink = false;
        [ACPropertyInfo(621)]
        public bool BtnWeighBlink
        {
            get => _BtnWeighBlink;
            set
            {
                _BtnWeighBlink = value;
                OnPropertyChanged("BtnWeighBlink");
            }
        }

        private bool _BtnAckBlink = false;
        [ACPropertyInfo(622)]
        public bool BtnAckBlink
        {
            get => _BtnAckBlink;
            set
            {
                _BtnAckBlink = value;
                OnPropertyChanged("BtnAckBlink");
            }
        }

        private ManualWeighingTaskInfo _NextTaskInfo;
        [ACPropertyInfo(623)]
        public ManualWeighingTaskInfo NextTaskInfo
        {
            get => _NextTaskInfo;
            set
            {
                _NextTaskInfo = value;
                OnPropertyChanged("NextTaskInfo");
            }
        }

        #endregion

        #region Properties => Components and Facility/FacilityCharge selection

        private WeighingMaterial _SelectedWeighingMaterial;
        [ACPropertySelected(624, "WeighingMaterial")]
        public WeighingMaterial SelectedWeighingMaterial
        {
            get => _SelectedWeighingMaterial;
            set
            {
                if (_SelectedWeighingMaterial != value)
                {
                    _SelectedWeighingMaterial = value;
                    _FacilityChargeList = null;
                    _FacilityList = null;
                    OnPropertyChanged("SelectedWeighingMaterial");
                    OnPropertyChanged("MaterialF_FCList");
                    FacilityChargeNo = null;
                    ScaleAddAcutalWeight = _PAFManuallyAddedQuantity != null ? _PAFManuallyAddedQuantity.ValueT : 0;
                    SelectedMaterialF_FC = null;
                    if (_SelectedWeighingMaterial != null)
                        ShowSelectFacilityLotInfo = true;
                    BtnWeighBlink = false;
                }
            }
        }

        private IEnumerable<WeighingMaterial> _WeighingMaterialList;
        [ACPropertyList(625, "WeighingMaterial")]
        public IEnumerable<WeighingMaterial> WeighingMaterialList
        {
            get => _WeighingMaterialList;
            set
            {
                _WeighingMaterialList = value;
                OnPropertyChanged("WeighingMaterialList");
            }
        }

        [ACPropertySelected(626, "F_FC")]
        public VBEntityObject SelectedMaterialF_FC
        {
            get
            {
                if (SelectedWeighingMaterial == null)
                    return null;
                if (SelectedWeighingMaterial.IsLotManaged)
                    return _SelectedFacilityCharge;
                return _SelectedFacility;
            }
            set
            {
                if (SelectedWeighingMaterial == null || SelectedWeighingMaterial.PosRelation == null || SelectedWeighingMaterial.PosRelation.SourceProdOrderPartslistPos == null)
                {
                    SelectedFacilityCharge = null;
                    SelectedFacility = null;
                }
                else if (SelectedWeighingMaterial.IsLotManaged)
                {
                    if (SelectedFacilityCharge != value)
                    {
                        SelectedFacilityCharge = value as vd.FacilityCharge;
                        //ParentBSO?.OnSetFacilityCharge(SelectedFacilityCharge, this);
                    }
                }
                else
                {
                    if (SelectedFacility != value)
                    {
                        SelectedFacility = value as vd.Facility;
                        //ParentBSO?.OnSetFacility(SelectedFacility, this);
                    }
                }
                OnPropertyChanged("SelectedMaterialF_FC");
            }
        }

        [ACPropertyList(627, "F_FC")]
        public IEnumerable<VBEntityObject> MaterialF_FCList
        {
            get
            {
                if (SelectedWeighingMaterial == null || WeighingMaterialList == null)
                    return null;
                if (SelectedWeighingMaterial.IsLotManaged)
                    return FacilityChargeList;
                return FacilityList;
            }
        }

        private vd.Facility _SelectedFacility;
        public vd.Facility SelectedFacility
        {
            get
            {
                return _SelectedFacility;
            }
            set
            {
                _SelectedFacility = value;
                if (value != null)
                    ShowSelectFacilityLotInfo = false;

                OnPropertyChanged("SelectedFacility");

                if (_StartWeighingFromF_FC)
                {
                    StartWeighing(false);
                    _StartWeighingFromF_FC = false;
                }
                else if (_CallPWLotChange && value != null && ComponentPWNode != null && ComponentPWNode.ValueT != null)
                {
                    ComponentPWNode.ValueT.ACUrlCommand("!LotChange", value.EntityKey, ScaleActualWeight, _IsLotConsumed, false);
                    _CallPWLotChange = false;
                }
                else if (_SelectedFacility != null && WeighingMaterialsFSM && SelectedWeighingMaterial != null
                        && SelectedWeighingMaterial.WeighingMatState != WeighingComponentState.InWeighing)
                {
                    CurrentComponentPWNode?.ACUrlCommand("ManualWeihgingNextTask", ManualWeighingTaskInfo.WaitForStart);
                }
            }
        }

        private vd.Facility[] _FacilityList;
        public virtual IEnumerable<vd.Facility> FacilityList
        {
            get
            {
                try
                {
                    if (_FacilityList == null && SelectedWeighingMaterial != null)
                    {
                        using (vd.DatabaseApp dbApp = new vd.DatabaseApp())
                        {
                            ACValueList facilities = ComponentPWNode?.ValueT?.ACUrlCommand("!GetAvailableFacilities", SelectedWeighingMaterial.PosRelation.ProdOrderPartslistPosRelationID) as ACValueList;
                            if (facilities == null)
                                return null;
                            _FacilityList = facilities.Select(c => dbApp.Facility.FirstOrDefault(x => x.FacilityID == c.ParamAsGuid)).ToArray();
                        }
                    }
                    return _FacilityList;
                }
                catch (Exception e)
                {
                    string message = null;
                    if (e.InnerException != null)
                        message = string.Format("ManualWeighingModel(FacilityList): {0}, {1} {2} {3}", e.Message, e.InnerException.Message, System.Environment.NewLine, e.StackTrace);
                    else
                        message = string.Format("ManualWeighingModel(FacilityList): {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace);

                    Messages.Error(this, message, true);
                    return null;
                }
            }
        }

        private vd.FacilityCharge _SelectedFacilityCharge;
        [ACPropertySelected(628, "FacilityCharge")]
        public vd.FacilityCharge SelectedFacilityCharge
        {
            get => _SelectedFacilityCharge;
            set
            {
                _SelectedFacilityCharge = value;
                if (value != null)
                    ShowSelectFacilityLotInfo = false;

                OnPropertyChanged("SelectedFacilityCharge");

                if (_SelectedFacilityCharge != null && _StartWeighingFromF_FC)
                {
                    if (SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.Selected)
                    {
                        Msg msg = StartWeighing(false);
                        if (msg != null)
                        {
                            Global.MsgResult result = Messages.Msg(msg, Global.MsgResult.Cancel, msg.MessageButton);
                            if (result == Global.MsgResult.Yes)
                            {
                                msg = StartWeighing(true);
                                if (msg != null)
                                {
                                    Messages.Msg(msg);
                                    ShowSelectFacilityLotInfo = true;
                                    return;
                                }
                            }
                            else
                            {
                                ShowSelectFacilityLotInfo = true;
                                return;
                            }
                        }
                        _SelectedFacilityCharge = null;
                    }
                    else
                        ShowSelectFacilityLotInfo = false;
                    _StartWeighingFromF_FC = false;
                }
                else if (_CallPWLotChange && value != null && ComponentPWNode != null && ComponentPWNode.ValueT != null)
                {
                    Msg msg = ComponentPWNode.ValueT.ACUrlCommand("!LotChange", value.FacilityChargeID, ScaleActualWeight, _IsLotConsumed, false) as Msg;
                    if (msg != null)
                    {
                        _SelectedFacilityCharge = null;
                        Messages.Msg(msg);
                        return;
                    }

                    _CallPWLotChange = false;
                    _IsLotConsumed = false;
                }
                else if (_SelectedFacilityCharge != null && WeighingMaterialsFSM && SelectedWeighingMaterial != null
                      && SelectedWeighingMaterial.WeighingMatState != WeighingComponentState.InWeighing)
                {
                    BtnWeighBlink = true;
                }
            }
        }

        vd.FacilityCharge[] _FacilityChargeList;
        [ACPropertyList(629, "FacilityCharge")]
        public virtual IEnumerable<vd.FacilityCharge> FacilityChargeList
        {
            get
            {
                try
                {
                    if (_FacilityChargeList == null && SelectedWeighingMaterial != null)
                    {
                        ACValueList facilityCharges = ComponentPWNode?.ValueT?.ACUrlCommand("!GetAvailableFacilityCharges", SelectedWeighingMaterial.PosRelation.ProdOrderPartslistPosRelationID) as ACValueList;
                        if (facilityCharges == null)
                            return null;
                        using (vd.DatabaseApp dbApp = new vd.DatabaseApp())
                            _FacilityChargeList = facilityCharges.Select(c => dbApp.FacilityCharge.Include("FacilityLot").Include("MDUnit").Include("Material")
                                                                                                             .FirstOrDefault(x => x.FacilityChargeID == c.ParamAsGuid)).ToArray();
                    }
                    return _FacilityChargeList;
                }
                catch (Exception e)
                {
                    string message = null;
                    if (e.InnerException != null)
                        message = string.Format("ManualWeighingModel(FacilityChargeList): {0}, {1} {2} {3}", e.Message, e.InnerException.Message, System.Environment.NewLine, e.StackTrace);
                    else
                        message = string.Format("ManualWeighingModel(FacilityChargeList): {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace);

                    Messages.Error(this, message, true);
                }
                return null;
            }
        }

        private string _FacilityChargeNo;
        [ACPropertyInfo(630, "FacilityCharge", "en{'Charge:'}de{'Charge:'}")]
        public string FacilityChargeNo
        {
            get => _FacilityChargeNo;
            set
            {
                _FacilityChargeNo = value;
                OnPropertyChanged("FacilityChargeNo");
            }
        }

        private bool _ShowSelectFacilityLotInfo = false;
        [ACPropertyInfo(631)]
        public bool ShowSelectFacilityLotInfo
        {
            get => _ShowSelectFacilityLotInfo;
            set
            {
                _ShowSelectFacilityLotInfo = value;
                OnPropertyChanged("ShowSelectFacilityLotInfo");
            }
        }

        private string _PAFCurrentMaterial;
        private IACContainerTNet<ManualWeighingTaskInfo> _NextTaskInfoProperty;

        [ACPropertyInfo(632)]
        public string PAFCurrentMaterial
        {
            get => _PAFCurrentMaterial;
            set
            {
                _PAFCurrentMaterial = value;
                OnPropertyChanged("PAFCurrentMaterial");
            }
        }

        #endregion

        #endregion

        #region Methods

        public override void Activate(ACComponent selectedProcessModule)
        {
            CurrentProcessModule = selectedProcessModule;
        }

        public override void DeActivate()
        {
            CurrentProcessModule = null;
        }

        #region Methods => Commands

        [ACMethodInfo("", "en{'Weigh'}de{'Wiegen'}", 601)]
        public virtual void Weigh()
        {
            var result = StartWeighing(false);
            if (result != null)
                Messages.Msg(result);
        }

        public virtual bool IsEnabledWeigh()
        {
            return ComponentPWNode != null && ComponentPWNode.ValueT != null;
        }

        public Msg StartWeighing(bool forceSetFC_F)
        {
            if (ComponentPWNode == null || ComponentPWNode.ValueT == null)
            {
                //Error50330: Can't start the weighing because the Reference to the workflow node is null.
                // Die Verwiegung kann nicht gstartet werden weil die Referenz zum Workflowknoten null ist.
                return new Msg(this, eMsgLevel.Error, "ManualWeihgingModel", "StartWeighing", 889, "Error50330");
            }

            if (SelectedWeighingMaterial != null)
            {
                if (SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.ReadyToWeighing ||
                    SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.Selected)
                {
                    Msg msg = ComponentPWNode.ValueT.ACUrlCommand("!StartWeighing", SelectedWeighingMaterial.PosRelation.ProdOrderPartslistPosRelationID,
                                                                          SelectedFacilityCharge?.FacilityChargeID, SelectedFacility?.FacilityID, forceSetFC_F) as Msg;
                    return msg;
                }
                else
                {
                    //Info50036: The component or material {0} has already been weighed or is already in the weighing process!
                    // Die Komponente bzw. Material {0} wurde bereits verwogen oder befindet sich schon im Wiegeprozess!
                    return new Msg(this, eMsgLevel.Error, "ManualWeihgingModel", "StartWeighing", 904, "Info50036", SelectedWeighingMaterial.PosRelation.SourceProdOrderPartslistPos.MaterialName);
                }
            }
            //Info50037: The component or material and the lot (quant) has not been selected. Select it and then press the "Weighing" button.
            // Die Komponente bzw. Material und die Charge (Quant) wurde nicht ausgewählt. Wählen Sie es aus und drücken anschliessend die Taste "Wiegen".
            return new Msg(this, eMsgLevel.Error, "ManualWeihgingModel", "StartWeighing", 908, "Info50037");
        }

        [ACMethodInfo("", "en{'Acknowledge'}de{'Quittieren'}", 602)]
        public virtual void Acknowledge()
        {
            if (!IsEnabledAcknowledge())
                return;

            var messageToAck = MessagesList.FirstOrDefault(c => !c.IsAlarmMessage && c.HandleByAcknowledgeButton);

            if (messageToAck != null)
                messageToAck.Acknowledge();
            else if (ComponentPWNode != null && ComponentPWNode.ValueT != null)
                ComponentPWNode.ValueT.ACUrlCommand("!CompleteWeighing", ScaleActualWeight);
        }

        public virtual bool IsEnabledAcknowledge()
        {
            return (MessagesList.Any(c => !c.IsAlarmMessage && c.HandleByAcknowledgeButton) ||
                                                       ScaleBckgrState == ScaleBackgroundState.InTolerance);
        }

        [ACMethodInfo("", "en{'Tare'}de{'Tarieren'}", 603)]
        public void Tare()
        {
            if (!IsEnabledTare())
                return;

            CurrentPAFManualWeighing?.ACUrlCommand("!TareActiveScale");
        }

        public bool IsEnabledTare()
        {
            return true; //TODO
        }

        [ACMethodInfo("", "en{'Lot change'}de{'Chargenwechsel'}", 604)]
        public virtual void LotChange()
        {
            if (!IsEnabledLotChange())
                return;

            if (SelectedWeighingMaterial != null && SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.InWeighing)
            {
                ShowSelectFacilityLotInfo = true;
                OnPropertyChanged("SelectedMaterialF_FC");
                _CallPWLotChange = true;
                _IsLotConsumed = false;
                if (SelectedWeighingMaterial.IsLotManaged)
                {
                    //Question50047: Was the material with the lot number {0} used up?
                    // Wurde das Material mit der Chargennummer{0} aufgebraucht?
                    Global.MsgResult result = Messages.Question(this, "Question50047", Global.MsgResult.Cancel, false, SelectedMaterialF_FC?.ACCaption);
                    if (result == Global.MsgResult.Yes)
                    {
                        var zeroBookTolerance = SelectedWeighingMaterial?.PosRelation?.SourceProdOrderPartslistPos?.Material?.ZeroBookingTolerance;
                        if (zeroBookTolerance.HasValue && zeroBookTolerance.Value < SelectedFacilityCharge.StockQuantityUOM)
                        {
                            //Question50048: The remaining stock of the batch (quant) is too large. Are you sure the batch is used up?
                            // Der Restbestand der Charge (Quant) ist zu groß. Sind Sie sicher dass die Charge aufgebraucht ist?
                            if (Messages.Question(this, "Question50048", Global.MsgResult.Cancel) == Global.MsgResult.Yes)
                                _IsLotConsumed = true;
                        }
                        else
                            _IsLotConsumed = true;
                    }
                    else if (result == Global.MsgResult.Cancel)
                    {
                        ShowSelectFacilityLotInfo = false;
                        _CallPWLotChange = false;
                    }
                }
            }
            else
            {
                //Info50038: A batch change can only be carried out during weighing.
                // Ein Chargenwechsel kann erst während der Verwiegung durchgeführt werden.
                Messages.Info(this, "Info50038");
            }
        }

        public virtual bool IsEnabledLotChange()
        {
            return SelectedWeighingMaterial != null;
        }

        [ACMethodInfo("", "en{'Bin change'}de{'Eimerwechsel'}", 605)]
        public virtual void BinChange()
        {
            if (!IsEnabledBinChange())
                return;

            if (ComponentPWNode != null && ComponentPWNode.ValueT != null)
                ComponentPWNode.ValueT.ExecuteMethod("BinChange");
        }

        public virtual bool IsEnabledBinChange()
        {
            return IsBinChangeAvailable;
        }

        [ACMethodInfo("", "en{'Abort'}de{'Abbrechen'}", 606)]
        public virtual void Abort()
        {
            if (!IsEnabledAbort())
                return;

            //Question50043: Do you want to abort the current weighing?
            // Möchten Sie die aktuelle Verwiegung abbrechen?
            if (Messages.YesNoCancel(this, "Question50043", Global.MsgResult.No) == Global.MsgResult.Yes)
            {
                //Question50049: Do you no longer want to weigh this material in the following batches? (e.g. for rework if it has been used up)
                // Möchten Sie dieses Material in den nachfolgenden Batchen nicht mehr verwiegen? (z.B. bei Rework wenn es aufgebraucht worden ist)
                if (Messages.Question(this, "Question50049") == Global.MsgResult.Yes)
                {
                    ComponentPWNode.ValueT.ACUrlCommand("!Abort", true);
                    return;
                }
                ComponentPWNode.ValueT.ACUrlCommand("!Abort", false);
            }
        }

        public bool IsEnabledAbort()
        {
            return ComponentPWNode != null;
        }

        [ACMethodInfo("", "en{'Apply charge/lot'}de{'Charge/Los anwenden'}", 607)]
        public virtual void ApplyLot()
        {
            if (!IsEnabledApplyLot())
                return;

            Msg msg;

            if (ComponentPWNode == null || ComponentPWNode.ValueT == null)
            {
                //Error50330: Can't start the weighing because the Reference to the workflow node is null.
                // Die Verwiegung kann nicht gstartet werden weil die Referenz zum Workflowknoten null ist.
                msg = new Msg(this, eMsgLevel.Error, "ManualWeighingModel", "OnApplyManuallyEnteredLot", 1035, "Error50330");
                Messages.Msg(msg);
                return;
            }

            msg = ComponentPWNode.ValueT.ACUrlCommand("!OnApplyManuallyEnteredLot", FacilityChargeNo, SelectedWeighingMaterial.PosRelation.ProdOrderPartslistPosRelationID) as Msg;
            if (msg != null)
                Messages.Msg(msg);
        }

        public virtual bool IsEnabledApplyLot()
        {
            return EnterLotManually && ShowSelectFacilityLotInfo;
        }

        [ACMethodInfo("", "en{'+ 1 kg'}de{'+ 1 kg'}", 608, true)]
        public void AddKg()
        {
            if (IsEnabledAddKg())
                _PAFManuallyAddedQuantity.ValueT = SelectedWeighingMaterial.AddKg(_PAFManuallyAddedQuantity.ValueT);
        }

        public bool IsEnabledAddKg()
        {
            return SelectedWeighingMaterial != null;
        }

        [ACMethodInfo("", "en{'- 1 kg'}de{'- 1 kg'}", 609, true)]
        public void RemoveKg()
        {
            if (IsEnabledRemoveKg())
                _PAFManuallyAddedQuantity.ValueT = SelectedWeighingMaterial.RemoveKg(_PAFManuallyAddedQuantity.ValueT);
        }

        public bool IsEnabledRemoveKg()
        {
            return SelectedWeighingMaterial != null;
        }

        #endregion

        #region Methods => Activation/Load

        public bool ActivateManualWeighingModel()
        {
            if (CurrentProcessModule == null)
            {
                //Error50283: The manual weighing module can not be initialized. The property CurrentProcessModule is null.
                // Die Handverwiegungsstation konnte nicht initialisiert werden. Die Eigenschaft CurrentProcessModule ist null.
                Messages.Error(this, "Error50283");
                return false;
            }

            PAProcessModuleACUrl = CurrentProcessModule.ACUrl;
            PAProcessModuleACCaption = CurrentProcessModule.ACCaption;

            if (CurrentProcessModule.ConnectionState == ACObjectConnectionState.DisConnected)
            {
                //Info50040: The server is unreachable. Reopen the program once the connection to the server has been established.
                // Der Server ist nicht erreichbar. Öffnen Sie das Programm erneut sobal die Verbindung zum Server wiederhergestellt wurde.
                Messages.Info(this, "Info50040");
                return false;
            }

            var processModuleChildComps = CurrentProcessModule.ACComponentChildsOnServer;
            IEnumerable<IACComponent> scalesObjects = null;
            IACComponent pafManWeighing = GetTargetFunction(processModuleChildComps);

            if (pafManWeighing == null)
            {
                //Error50286: The manual weighing component can not be initialized. The process module {0} has not a child component of type PAFManualWeighing.
                // Die Verwiegekomponente konnte nicht initialisiert werden. Das Prozessmodul {0} hat keine Kindkomponente vom Typ PAFManualWeighing.
                Messages.Info(this, "Error50286", false, PAProcessModuleACUrl);
                return false;
            }

            var pafManWeighingRef = new ACRef<IACComponent>(pafManWeighing, this);

            ACValueList availableScales = pafManWeighingRef.ValueT.ACUrlCommand("!GetAvailableScaleObjects") as ACValueList;

            if (availableScales == null)
            {
                using (ACMonitor.Lock(Database.QueryLock_1X000))
                {
                    scalesObjects = processModuleChildComps.Where(c => typeof(PAEScaleBase).IsAssignableFrom(c.ComponentClass.ObjectType)).ToArray();
                }
            }
            else
            {
                scalesObjects = processModuleChildComps.Where(c => typeof(PAEScaleBase).IsAssignableFrom(c.ComponentClass.ObjectType)
                                                                && availableScales.Any(x => x.Value is Guid && x.ParamAsGuid == c.ComponentClass.ACClassID)).ToArray();
            }

            if (scalesObjects != null && scalesObjects.Any())
            {
                _ProcessModuleScales = scalesObjects.Select(c => new ACRef<IACComponent>(c, this)).ToArray();
                ScaleObjectsList = new List<ACValueItem>(_ProcessModuleScales.Select(c => new ACValueItem(c.ValueT.ACCaption, c.ACUrl, null)));
            }

            var orderInfo = CurrentProcessModule.GetPropertyNet("OrderInfo");
            if (orderInfo == null)
            {
                //Error50285: Initialization error: The process module doesn't have the property {0}.
                // Initialisierungsfehler: Das Prozessmodul besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50285", false, "OrderInfo");
                return false;
            }

            var wfNodes = CurrentProcessModule.GetPropertyNet("WFNodes");
            if (wfNodes == null)
            {
                //Error50285: Initialization error: The process module doesn't have the property {0}.
                // Initialisierungsfehler: Das Prozessmodul besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50285", false, "WFNodes");
                return false;
            }

            var hasAlarms = CurrentProcessModule.GetPropertyNet("HasAlarms");
            if (hasAlarms == null)
            {
                //Error50285: Initialization error: The process module doesn't have the property {0}.
                // Initialisierungsfehler: Das Prozessmodul besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50285", false, "HasAlarms");
                return false;
            }

            var alarmsAsText = CurrentProcessModule.GetPropertyNet("AlarmsAsText");
            if (alarmsAsText == null)
            {
                //Error50285: Initialization error: The process module doesn't have the property {0}.
                // Initialisierungsfehler: Das Prozessmodul besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50285", false, "AlarmsAsText");
                return false;
            }

            LoadPAFManualWeighing(pafManWeighingRef);

            if (ScaleObjectsList != null && ScaleObjectsList.Any())
            {
                SelectActiveScaleObject(true);
            }

            _OrderInfo = orderInfo as IACContainerTNet<string>;
            (_OrderInfo as IACPropertyNetBase).PropertyChanged += OrderInfoPropertyChanged;

            if (_CurrentOrderInfoValueLock == null)
                _CurrentOrderInfoValueLock = new object();
            if (_PWNodeACStateLock == null)
                _PWNodeACStateLock = new object();
            LoadWFNode();

            _WFNodes = wfNodes as IACContainerTNet<List<ACChildInstanceInfo>>;
            (_WFNodes as IACPropertyNetBase).PropertyChanged += WFNodes_PropertyChanged;
            if (_WFNodes.ValueT != null)
                ApplicationQueue.Add(() => MScaleWFNodes = _WFNodes.ValueT);

            _AlarmsAsText = alarmsAsText as IACContainerTNet<string>;

            _ScaleHasAlarms = hasAlarms as IACContainerTNet<bool>;
            (_ScaleHasAlarms as IACPropertyNetBase).PropertyChanged += ScaleHasAlarms_PropertyChanged;
            bool hasAlarmsTemp = _ScaleHasAlarms.ValueT;
            ApplicationQueue.Add(() => HandleAlarms(hasAlarmsTemp));

            return true;
        }

        public virtual IACComponent GetTargetFunction(IEnumerable<IACComponent> processModuleChildrenComponents)
        {
            using (ACMonitor.Lock(Database.QueryLock_1X000))
            {
                return processModuleChildrenComponents.FirstOrDefault(c => typeof(PAFManualWeighing).IsAssignableFrom(c.ComponentClass.ObjectType)
                                                                      && !typeof(PAFManualAddition).IsAssignableFrom(c.ComponentClass.ObjectType));
            }
        }

        public virtual void OnGetPWGroup(IACComponentPWNode pwGroup)
        {

        }

        private void LoadPAFManualWeighing(ACRef<IACComponent> pafManWeighing)
        {
            UnloadPAFManualWeighing();

            _CurrentPAFManualWeighing = pafManWeighing;

            var currentACMethod = _CurrentPAFManualWeighing.ValueT.GetPropertyNet("CurrentACMethod");
            if (currentACMethod == null)
            {
                //Error50287: Initialization error: The weighing function doesn't have the property {0}.
                // Initialisierungsfehler: Die Verwiegefunktion besitzt nicht die Eigenschaft {0}.
                Messages.Info(this, "Error50287", false, "CurrentACMethod");
                return;
            }

            var manuallyAddedQuantity = _CurrentPAFManualWeighing.ValueT.GetPropertyNet("ManuallyAddedQuantity");
            if (manuallyAddedQuantity == null)
            {
                //Error50287: Initialization error: The weighing function doesn't have the property {0}.
                // Initialisierungsfehler: Die Verwiegefunktion besitzt nicht die Eigenschaft {0}.
                Messages.Info(this, "Error50287", false, "ManuallyAddedQuantity");
                return;
            }

            var tareScaleState = _CurrentPAFManualWeighing.ValueT.GetPropertyNet("TareScaleState");
            if (tareScaleState == null)
            {
                //Error50287: Initialization error: The weighing function doesn't have the property {0}.
                // Initialisierungsfehler: Die Verwiegefunktion besitzt nicht die Eigenschaft {0}.
                Messages.Info(this, "Error50287", false, "TareScaleState");
                return;
            }

            _PAFCurrentACMethod = currentACMethod as IACContainerTNet<ACMethod>;
            (_PAFCurrentACMethod as IACPropertyNetBase).PropertyChanged += PAFCurrentACMethodPropChanged;
            ACMethod temp = _PAFCurrentACMethod?.ValueT?.Clone() as ACMethod;
            ApplicationQueue.Add(() => HandlePAFCurrentACMethod(temp));

            _PAFManuallyAddedQuantity = manuallyAddedQuantity as IACContainerTNet<double>;
            (_PAFManuallyAddedQuantity as IACPropertyNetBase).PropertyChanged += PAFManuallyAddedQuantityPropChanged;
            ScaleAddAcutalWeight = _PAFManuallyAddedQuantity.ValueT;

            _TareScaleState = tareScaleState as IACContainerT<short>;
        }

        private void ActivateScale(ACRef<IACComponent> scale)
        {
            if (scale == null || scale.ValueT == null)
                return;

            var actWeightProp = scale.ValueT.GetPropertyNet("ActualWeight");
            if (actWeightProp == null)
            {
                //Error50292: Initialization error: The scale component doesn't have the property {0}.
                // Initialisierungsfehler: Die Waagen-Komponente besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50292", false, "ActualWeight");
                return;
            }

            _ScaleActualWeight = actWeightProp as IACContainerTNet<double>;
            (_ScaleActualWeight as IACPropertyNetBase).PropertyChanged += ActWeightProp_PropertyChanged;
            ScaleRealWeight = _ScaleActualWeight.ValueT;

            if (CurrentPAFManualWeighing != null && scale != null && scale.ValueT != null)
            {
                CurrentPAFManualWeighing.ACUrlCommand("!SetActiveScaleObject", scale.ValueT.ACIdentifier);
            }

            OnPropertyChanged("CurrentScaleObject");
        }

        private void LoadWFNode()
        {
            lock (_CurrentOrderInfoValueLock)
            {
                if (_CurrentOrderInfoValue == _OrderInfo.ValueT)
                    return;
                _CurrentOrderInfoValue = _OrderInfo.ValueT;
            }

            lock (_PWNodeACStateLock)
            {
                UnloadWFNode();
            }

            if (!string.IsNullOrEmpty(_CurrentOrderInfoValue))
            {
                try
                {
                    string[] accessArr = (string[])CurrentProcessModule?.ACUrlCommand("!SemaphoreAccessedFrom");
                    if (accessArr == null || !accessArr.Any())
                        return;

                    string pwGroupACUrl = accessArr[0];
                    IACComponentPWNode pwGroup = null;

                    pwGroup = Root.ACUrlCommand(pwGroupACUrl) as IACComponentPWNode;
                    if (pwGroup == null)
                        return;

                    OnGetPWGroup(pwGroup);

                    IEnumerable<ACChildInstanceInfo> pwNodes;

                    using (Database db = new Database())
                    {
                        var pwClass = db.ACClass.FirstOrDefault(c => c.ACProject.ACProjectTypeIndex == (short)Global.ACProjectTypes.ClassLibrary &&
                                                                                                        c.ACIdentifier == PWManualWeighing.PWClassName);
                        ACRef<ACClass> refClass = new ACRef<ACClass>(pwClass, true);
                        pwNodes = pwGroup.GetChildInstanceInfo(1, new ChildInstanceInfoSearchParam() { OnlyWorkflows = true, TypeOfRoots = refClass });
                        refClass.Detach();
                    }

                    if (pwNodes == null || !pwNodes.Any())
                        return;

                    List<ManualWeighingPWNode> mwPWNodes = new List<ManualWeighingPWNode>();
                    var availableWFNodes = FindWFNodes(pwNodes, pwGroup);

                    foreach (var pwNode in availableWFNodes)
                    {
                        ManualWeighingPWNode mwPWNode = new ManualWeighingPWNode(pwNode);

                        if (mwPWNode.ComponentPWNodeACState == null)
                        {
                            //Error50289: Initialization error: The reference to the ACState-Property in Workflownode {0} is null.
                            // Initialisierungsfehler: Die Referenz zur ACState-Eigenschaft von Workflowknoten {0} ist null.
                            Messages.Error(this, "Error50289", false, mwPWNode?.ComponentPWNode?.ACUrl);
                            continue;
                        }

                        mwPWNode.ComponentPWNodeACState.PropertyChanged += PWNodeACStatePropertyChanged;
                        if (mwPWNode.ComponentPWNode.ValueT.ConnectionState == ACObjectConnectionState.ValuesReceived)
                            HandlePWNodeACState(mwPWNode, mwPWNode.ComponentPWNodeACState.ValueT);

                        mwPWNodes.Add(mwPWNode);
                    }

                    ComponentPWNodesList = mwPWNodes;
                }
                catch (Exception e)
                {
                    string message = null;
                    if (e.InnerException != null)
                        message = string.Format("ManualWeighingModel(LoadWFNode): {0}, {1} {2} {3}", e.Message, e.InnerException.Message, System.Environment.NewLine, e.StackTrace);
                    else
                        message = string.Format("ManualWeighingModel(LoadWFNode): {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace);

                    Messages.Error(this, message, true);
                }
            }
        }

        public virtual List<ACRef<IACComponentPWNode>> FindWFNodes(IEnumerable<ACChildInstanceInfo> availablePWNodes, IACComponentPWNode pwGroup)
        {
            List<ACRef<IACComponentPWNode>> result = new List<ACRef<IACComponentPWNode>>();

            var nodes = availablePWNodes.Where(c => !typeof(PWManualAddition).IsAssignableFrom(c.ACType.ValueT.ObjectType));

            foreach (var node in nodes)
            {
                IACComponentPWNode pwNode = pwGroup.ACUrlCommand(node.ACUrlParent + "\\" + node.ACIdentifier) as IACComponentPWNode;
                if (pwNode == null)
                {
                    //Error50290: The user does not have access rights for class PWManualWeighing ({0}).
                    // Der Benutzer hat keine Zugriffsrechte auf Klasse PWManualWeighing ({0}).
                    Messages.Error(this, "Error50290", false, node.ACUrlParent + "\\" + node.ACIdentifier);
                    continue;
                }
                var refPWNode = new ACRef<IACComponentPWNode>(pwNode, this);
                result.Add(refPWNode);
            }

            return result;
        }

        private void ActivateWFNode(ACRef<IACComponentPWNode> pwNode)
        {
            if (ComponentPWNode != null)
            {
                return;
            }

            var weighingCompInfo = pwNode.ValueT.GetPropertyNet("CurrentWeighingComponentInfo");
            if (weighingCompInfo == null)
            {
                //Error50291: Initialization error: The reference to the property {1} in Workflownode {0} is null.
                // Initialisierungsfehler: Die Referenz zur Eigenschaft {1} von Workflowknoten {0} ist null.
                Messages.Error(this, "Error50291", false, pwNode?.ACUrl, "CurrentWeighingComponentInfo");
                return;
            }

            ComponentPWNode = pwNode;

            if (!IsBinChangeAvailable)
            {
                bool? isBinChangeAvailable = ComponentPWNode.ValueT.ACUrlCommand("!IsBinChangeLoopNodeAvailable") as bool?;
                IsBinChangeAvailable = isBinChangeAvailable ?? false;
            }

            var temp = DatabaseApp.ProdOrderPartslistPos.FirstOrDefault();

            LoadPWConfiguration();
            SetupModel(DatabaseApp, DefaultMaterialIcon);

            _NextTaskInfoProperty = ComponentPWNode.ValueT.GetPropertyNet("ManualWeihgingNextTask") as IACContainerTNet<ManualWeighingTaskInfo>;
            if (_NextTaskInfoProperty != null)
            {
                _NextTaskInfoProperty.PropertyChanged += _NextTaskInfoProperty_PropertyChanged;
                NextTaskInfo = _NextTaskInfoProperty.ValueT;
            }

            _WeighingComponentInfo = weighingCompInfo as IACContainerTNet<WeighingComponentInfo>;
            if (_WeighingComponentInfo != null)
            {
                WeighingComponentInfo tempCompInfo = _WeighingComponentInfo.ValueT;
                ApplicationQueue.Add(() => HandleWeighingComponentInfo(tempCompInfo));
                (_WeighingComponentInfo as IACPropertyNetBase).PropertyChanged += WeighingComponentInfoPropChanged;
            }
        }

        private void _NextTaskInfoProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT && _NextTaskInfoProperty != null && NextTaskInfo != _NextTaskInfoProperty.ValueT)
            {
                NextTaskInfo = _NextTaskInfoProperty.ValueT;
            }
        }

        public void SetupModel(vd.DatabaseApp dbApp, ACClassDesign iconDesign = null)
        {
            try
            {
                Messages.LogInfo("ManualWeighingModel", "SetupModel", "SetupModel after GetEndBatchPos.");
                MainSyncContext.Send((object state) =>
                {
                    WeighingMaterialList = GetWeighingMaterials(dbApp, iconDesign);
                }, new object());
                Messages.LogInfo("ManualWeighingModel", "SetupModel", "SetupModel after GetWeighingMaterials. Count " + WeighingMaterialList?.Count());
            }
            catch (Exception e)
            {
                string message = null;
                if (e.InnerException != null)
                    message = string.Format("ManualWeighingModel(Setup model): {0}, {1} {2} {3}", e.Message, e.InnerException.Message, System.Environment.NewLine, e.StackTrace);
                else
                    message = string.Format("ManualWeighingModel(Setup model): {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace);
                Messages.Error(this, message, true);
            }
        }

        public List<WeighingMaterial> GetWeighingMaterials(vd.DatabaseApp db, ACClassDesign iconDesign = null)
        {
            if (ComponentPWNode == null || ComponentPWNode.ValueT == null)
                return null;

            ACValue acValue = ComponentPWNode.ValueT.ACUrlCommand("WeighingComponentsInfo\\ValueT") as ACValue;
            Dictionary<string, string> valueList = acValue?.Value as Dictionary<string, string>;
            if (valueList != null && valueList.Any())
            {
                List<WeighingMaterial> weihgingMaterials = new List<WeighingMaterial>();
                foreach (var valueItem in valueList.ToArray())
                {
                    if (Guid.TryParse(valueItem.Key, out Guid PLPosRel))
                    {
                        if (short.TryParse(valueItem.Value, out short weighingState))
                        {
                            vd.ProdOrderPartslistPosRelation posRelation = db.ProdOrderPartslistPosRelation.Include(s => s.SourceProdOrderPartslistPos.Material.MDFacilityManagementType)
                                                                             .FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == PLPosRel);
                            if (posRelation != null)
                            {
                                weihgingMaterials.Add(new WeighingMaterial(posRelation, (WeighingComponentState)weighingState, iconDesign, this));
                            }
                        }
                    }
                }
                return weihgingMaterials.OrderBy(c => c.PosRelation.Sequence).ToList();
            }
            return null;
        }

        public void LoadPWConfiguration()
        {
            ACMethod acMethod = ComponentPWNode?.ValueT?.ACUrlCommand("MyConfiguration") as ACMethod;
            if (acMethod == null)
            {
                //Error50288: The configuration(ACMethod) for the workflow node cannot be found!
                // Die Konfiguration (ACMethod) für den Workflow-Knoten kann nicht gefunden werden!
                Messages.Error(this, "Error50288");
                return;
            }

            var freeSelectionMode = acMethod.ParameterValueList.GetACValue("FreeSelectionMode");
            if (freeSelectionMode != null)
                WeighingMaterialsFSM = freeSelectionMode.ParamAsBoolean;

            var enterLotManually = acMethod.ParameterValueList.GetACValue("EnterLotManually");
            if (enterLotManually != null)
                EnterLotManually = enterLotManually.ParamAsBoolean;

            //TODO: lot order
        }

        //internal void GetEndBatchPos(vd.DatabaseApp db)
        //{
        //    EntityKey endBatchPosKey = ComponentPWNode.ValueT.ACUrlCommand("CurrentEndBatchPosKey\\ValueT") as EntityKey;
        //    if (endBatchPosKey == null)
        //        endBatchPosKey = ComponentPWNode.ValueT.ACUrlCommand("CurrentEndBatchPosKey\\ValueT") as EntityKey;

        //    if (endBatchPosKey != null)
        //        using (ACMonitor.Lock(db.QueryLock_1X000))
        //        {
        //            var keyMember = endBatchPosKey.EntityKeyValues.FirstOrDefault();
        //            if (keyMember != null)
        //            {
        //                Guid? id = keyMember.Value as Guid?;
        //                if (id.HasValue)
        //                    EndBatchPos = db.ProdOrderPartslistPos.Include(p => p.ProdOrderBatch).Include(pl => pl.ProdOrderPartslist.ProdOrder)
        //                                                          .Include(m => m.Material).FirstOrDefault(c => c.ProdOrderPartslistPosID == id);
        //            }

        //            //.GetObjectByKey(endBatchPosKey) as vd.ProdOrderPartslistPos;
        //        }
        //}

        #endregion

        #region Methods => Deactivation/Unload

        public void DeactivateManualWeighingModel()
        {
            UnloadWFNode();
            DeactivateScale();
            UnloadPAFManualWeighing();

            if (_OrderInfo != null)
            {
                (_OrderInfo as IACPropertyNetBase).PropertyChanged -= OrderInfoPropertyChanged;
                _OrderInfo = null;
            }

            if (_CurrentOrderInfoValueLock != null)
            {
                lock (_CurrentOrderInfoValueLock)
                {
                    _CurrentOrderInfoValue = null;
                }
            }

            if (_ScaleHasAlarms != null)
            {
                (_ScaleHasAlarms as IACPropertyNetBase).PropertyChanged -= ScaleHasAlarms_PropertyChanged;
                _ScaleHasAlarms = null;
            }

            if (_WFNodes != null)
            {
                (_WFNodes as IACPropertyNetBase).PropertyChanged -= WFNodes_PropertyChanged;
                _WFNodes = null;
            }


            if (_ProcessModuleScales != null && _ProcessModuleScales.Any())
            {
                foreach (var scaleRef in _ProcessModuleScales)
                    scaleRef.Detach();
            }
            _ProcessModuleScales = null;

            MessagesList.Clear();

            BtnAckBlink = false;
            BtnWeighBlink = false;
            NextTaskInfo = ManualWeighingTaskInfo.None;

            _LoadWFNodeLock = null;
            _PAFManuallyAddedQuantity = null;
            _TareScaleState = null;
            _CallPWLotChange = false;
            _CurrentOrderInfoValueLock = null;
            _PWNodeACStateLock = null;
            _MScaleWFNodes = null;
            _ScaleObjectsList = null;
            _AlarmsAsTextCache = null;
        }

        private void DeactivateWFNode(bool reset = false)
        {
            //EndBatchPos = null;
            WeighingMaterialList = null;
            SelectedWeighingMaterial = null;
            IsBinChangeAvailable = false;

            if (_NextTaskInfoProperty != null)
            {
                _NextTaskInfoProperty.PropertyChanged -= _NextTaskInfoProperty_PropertyChanged;
                NextTaskInfo = ManualWeighingTaskInfo.None;
                _NextTaskInfoProperty = null;
            }


            if (_WeighingComponentInfo != null)
            {
                (_WeighingComponentInfo as IACPropertyNetBase).PropertyChanged -= WeighingComponentInfoPropChanged;
                _WeighingComponentInfo = null;
            }

            if (!reset)
            {
                PAFCurrentMaterial = "";
                TargetWeight = 0;
                ScaleBckgrState = ScaleBackgroundState.Weighing;
            }

            ComponentPWNode = null;
        }

        public void UnloadWFNode()
        {
            IsBinChangeAvailable = false;

            DeactivateWFNode();

            // EndBatchPos = null;

            if (ComponentPWNodesList == null)
                return;

            foreach (var node in ComponentPWNodesList)
            {
                if (node.ComponentPWNodeACState != null)
                    node.ComponentPWNodeACState.PropertyChanged -= PWNodeACStatePropertyChanged;

                node.Deinit();
            }

            ComponentPWNodesList = null;

        }

        private void UnloadPAFManualWeighing()
        {
            //TargetWeight = 0;
            if (_PAFCurrentACMethod != null)
            {
                (_PAFCurrentACMethod as IACPropertyNetBase).PropertyChanged -= PAFCurrentACMethodPropChanged;
                _PAFCurrentACMethod = null;
            }

            if (_PAFManuallyAddedQuantity != null)
            {
                (_PAFManuallyAddedQuantity as IACPropertyNetBase).PropertyChanged -= PAFManuallyAddedQuantityPropChanged;
                _PAFManuallyAddedQuantity = null;
            }

            _TareScaleState = null;

            if (_CurrentPAFManualWeighing != null)
            {
                _CurrentPAFManualWeighing.Detach();
                _CurrentPAFManualWeighing = null;
            }
        }

        private void DeactivateScale()
        {
            if (_ScaleActualWeight != null)
            {
                (_ScaleActualWeight as IACPropertyNetBase).PropertyChanged -= ActWeightProp_PropertyChanged;
                _ScaleActualWeight = null;
            }
        }

        #endregion

        #region Methods => PropertyChanged

        private void OrderInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
                ApplicationQueue.Add(() => LoadWFNode());
        }

        private void PAFCurrentACMethodPropChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                ACMethod temp = _PAFCurrentACMethod?.ValueT?.Clone() as ACMethod;
                ApplicationQueue.Add(() => HandlePAFCurrentACMethod(temp));
            }
        }

        private void PWNodeACStatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT && ComponentPWNodesList != null)
            {
                var targetNode = ComponentPWNodesList.FirstOrDefault(c => c.ComponentPWNodeACState == sender);
                if (targetNode != null && (CurrentComponentPWNode == null || targetNode.ComponentPWNode.ValueT.ACUrl == CurrentComponentPWNode.ACUrl))
                {
                    ACStateEnum tempACState = targetNode.ComponentPWNodeACState.ValueT;
                    ApplicationQueue.Add(() => HandlePWNodeACState(targetNode, tempACState));
                }
            }
        }

        private void WFNodes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
                ApplicationQueue.Add(() => MScaleWFNodes = _WFNodes.ValueT);
        }

        private void ScaleHasAlarms_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                bool hasAlarms = _ScaleHasAlarms.ValueT;
                ApplicationQueue.Add(() => HandleAlarms(hasAlarms));
            }
        }

        private void ActWeightProp_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT && _ScaleActualWeight != null)
            {
                double tempValue = _ScaleActualWeight.ValueT;
                ApplicationQueue.Add(() => ScaleRealWeight = tempValue);
            }
        }

        private void WeighingComponentInfoPropChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                if (_WeighingComponentInfo != null && _WeighingComponentInfo.ValueT != null)
                {
                    var temp = _WeighingComponentInfo.ValueT.Clone() as WeighingComponentInfo;
                    ApplicationQueue.Add(() => HandleWeighingComponentInfo(temp));
                }
            }
        }

        private void PAFManuallyAddedQuantityPropChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT && _PAFManuallyAddedQuantity != null)
            {
                double tempValue = _PAFManuallyAddedQuantity.ValueT;
                ApplicationQueue.Add(() => ScaleAddAcutalWeight = tempValue);
            }
        }

        #endregion

        #region Methods => HandlePropertyChanged

        private void HandlePWNodeACState(ManualWeighingPWNode mwPWNode, ACStateEnum pwNodeACstate)
        {
            try
            {
                if (_PWNodeACStateLock == null)
                {
                    Messages.LogMessage(eMsgLevel.Error, "ManualWeighingModel", "HandlePWNodeACState", "PWNodeACStateLock is null!");
                    return;
                }

                lock (_PWNodeACStateLock)
                {
                    if (pwNodeACstate == ACStateEnum.SMRunning)
                        ActivateWFNode(mwPWNode.ComponentPWNode);
                    else if (pwNodeACstate == ACStateEnum.SMCompleted)
                        DeactivateWFNode();
                    else if (pwNodeACstate == ACStateEnum.SMResetting)
                        DeactivateWFNode(true);
                }
            }
            catch (Exception e)
            {
                string message = null;
                if (e.InnerException != null)
                    message = string.Format("ManualWeighingModel(HandlePWNodeACState): {0}, {1} {2} {3}", e.Message, e.InnerException.Message, System.Environment.NewLine, e.StackTrace);
                else
                    message = string.Format("ManualWeighingModel(HandlePWNodeACState): {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace);

                Messages.Error(this, message, true);
            }
        }

        private void HandlePAFCurrentACMethod(ACMethod currentACMethod)
        {
            if (currentACMethod != null)
            {
                _TolerancePlus = currentACMethod.ParameterValueList.GetDouble("TolerancePlus");
                _ToleranceMinus = currentACMethod.ParameterValueList.GetDouble("ToleranceMinus");
            }

            if (currentACMethod != null)
            {
                PAFCurrentMaterial = currentACMethod.ParameterValueList.GetString("Material");
                TargetWeight = currentACMethod.ParameterValueList.GetDouble("TargetQuantity");
            }
            else
            {
                PAFCurrentMaterial = "";
                TargetWeight = 0;
                ScaleBckgrState = ScaleBackgroundState.Weighing;
            }
        }

        private void HandleWeighingComponentInfo(WeighingComponentInfo compInfo)
        {
            if (WeighingMaterialList == null)
                return;

            if (compInfo == null)
                return;

            //string msg = "InfoType: " + compInfo.WeighingComponentInfoType + " CompState: " + compInfo.WeighingComponentState;
            //ParentBSO.Messages.LogInfo("ManualWeighingModel", "HandleWeighingComponentInfo", msg);

            bool canContinue = OnHandleWeighingComponentInfo(compInfo);
            if (!canContinue)
                return;

            WeighingComponentInfoType compInfoType = (WeighingComponentInfoType)compInfo.WeighingComponentInfoType;
            try
            {
                switch (compInfoType)
                {
                    case WeighingComponentInfoType.State:
                        {
                            WeighingMaterial comp = WeighingMaterialList.FirstOrDefault(c => c.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation);
                            if (comp != null)
                            {
                                comp.ChangeComponentState((WeighingComponentState)compInfo.WeighingComponentState, DatabaseApp);
                                SelectActiveScaleObject(comp);
                                if (comp.WeighingMatState == WeighingComponentState.InWeighing)
                                    BtnWeighBlink = false;
                            }
                            break;
                        }
                    case WeighingComponentInfoType.StateSelectCompAndFC_F:
                        {
                            MainSyncContext.Send((object state) =>
                            {
                                WeighingMaterial comp = WeighingMaterialList.FirstOrDefault(c => c.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation);
                                if (comp == null)
                                    return;
                                if (SelectedWeighingMaterial != comp)
                                    SelectedWeighingMaterial = comp;

                                comp.ChangeComponentState((WeighingComponentState)compInfo.WeighingComponentState, DatabaseApp);
                                SelectActiveScaleObject(comp);
                                if (comp.WeighingMatState == WeighingComponentState.InWeighing)
                                    BtnWeighBlink = false;

                                if (compInfo.FacilityCharge != null)
                                {
                                    var fcItem = FacilityChargeList.FirstOrDefault(c => c.FacilityChargeID == compInfo.FacilityCharge);
                                    SelectedMaterialF_FC = fcItem;
                                }
                                else if (compInfo.Facility != null)
                                {
                                    var fItem = FacilityList.FirstOrDefault(c => c.FacilityID == compInfo.Facility);
                                    SelectedMaterialF_FC = fItem;
                                }
                            }, new object());
                            break;
                        }
                    case WeighingComponentInfoType.SelectCompReturnFC_F:
                        {
                            WeighingMaterial comp = WeighingMaterialList.FirstOrDefault(c => c.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation);
                            if (SelectedWeighingMaterial != comp)
                            {
                                SelectedWeighingMaterial = comp;
                                SelectedWeighingMaterial.ChangeComponentState(WeighingComponentState.Selected, DatabaseApp);
                                _StartWeighingFromF_FC = true;
                            }
                            break;
                        }
                    case WeighingComponentInfoType.StateSelectFC_F:
                    case WeighingComponentInfoType.SelectFC_F:
                        {
                            if (compInfoType == WeighingComponentInfoType.StateSelectFC_F && (WeighingComponentState)compInfo.WeighingComponentState == WeighingComponentState.InWeighing
                                && SelectedWeighingMaterial == null)
                            {
                                WeighingMaterial comp = WeighingMaterialList.FirstOrDefault(c => c.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation);
                                if (comp != null)
                                    SelectedWeighingMaterial = comp;
                            }

                            if (compInfoType == WeighingComponentInfoType.StateSelectFC_F && SelectedWeighingMaterial != null &&
                                SelectedWeighingMaterial.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation)
                            {
                                SelectedWeighingMaterial.ChangeComponentState((WeighingComponentState)compInfo.WeighingComponentState, DatabaseApp);
                                SelectActiveScaleObject(SelectedWeighingMaterial);
                            }

                            MainSyncContext.Send((object state) =>
                            {
                                if (compInfo.FacilityCharge != null)
                                {
                                    bool autoRefresh = compInfo.FC_FAutoRefresh;
                                    if (SelectedMaterialF_FC == null || _CallPWLotChange || compInfo.IsLotChange)
                                    {
                                        if (autoRefresh)
                                        {
                                            SelectedMaterialF_FC = null; //check if this needed
                                            _FacilityChargeList = null;
                                            OnPropertyChanged("MaterialF_FCList");
                                        }

                                        var fcItem = FacilityChargeList?.FirstOrDefault(c => c.FacilityChargeID == compInfo.FacilityCharge);
                                        SelectedMaterialF_FC = fcItem;
                                    }
                                }
                                else if (compInfo.Facility != null)
                                {
                                    var fItem = FacilityList.FirstOrDefault(c => c.FacilityID == compInfo.Facility);
                                    if (SelectedMaterialF_FC == null)
                                    {
                                        SelectedMaterialF_FC = fItem;
                                        OnPropertyChanged("SelectedMaterialF_FC");
                                    }
                                }
                            }, new object());
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                string message = null;
                if (e.InnerException != null)
                    message = string.Format("ManualWeighingModel(HandleWeighingComponentInfo): {0}, {1} {2} {3}", e.Message, e.InnerException.Message, System.Environment.NewLine, e.StackTrace);
                else
                    message = string.Format("ManualWeighingModel(HandleWeighingComponentInfo): {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace);

                Messages.Error(this, message, true);
            }
        }

        public virtual bool OnHandleWeighingComponentInfo(WeighingComponentInfo compInfo)
        {
            if (compInfo.IsManualAddition)
                return false;
            return true;
        }

        private void HandleWFNodes(List<ACChildInstanceInfo> connectionList)
        {
            if (PWUserAckClasses == null || !PWUserAckClasses.Any())
                return;

            if (connectionList == null)
            {
                BtnAckBlink = false;
                var itemsToRemove = MessagesList.Where(c => c.UserAckPWNode != null).ToArray();
                foreach (var itemToRemove in itemsToRemove)
                {
                    RemoveFromMessageList(itemToRemove);
                    itemToRemove.DeInit();
                }

                return;
            }


            var pwInstanceInfos = connectionList.Where(c => PWUserAckClasses.Contains(c.ACType.ValueT));

            var userAckItemsToRemove = MessagesList.Where(c => c.UserAckPWNode != null && !pwInstanceInfos.Any(x => x.ACUrlParent + "\\" + x.ACIdentifier == c.UserAckPWNode.ACUrl)).ToArray();
            foreach (var itemToRemove in userAckItemsToRemove)
            {
                RemoveFromMessageList(itemToRemove);
                itemToRemove.DeInit();
                if (BtnAckBlink && !MessagesList.Any(c => c.HandleByAcknowledgeButton && !c.IsAlarmMessage))
                    BtnAckBlink = false;
            }

            foreach (var instanceInfo in pwInstanceInfos)
            {
                string instanceInfoACUrl = instanceInfo.ACUrlParent + "\\" + instanceInfo.ACIdentifier;
                if (MessagesList.Any(c => c.UserAckPWNode != null && c.UserAckPWNode.ACUrl == instanceInfoACUrl))
                    continue;

                var pwNode = Root.ACUrlCommand(instanceInfoACUrl) as IACComponent;
                if (pwNode == null)
                    continue;

                var userAckItem = new MessageItem(pwNode, this);
                AddToMessageList(userAckItem);
                if (!BtnAckBlink)
                    BtnAckBlink = true;

                //if (TaskPresenter != null && TaskPresenter.PresenterACWorkflowNode == null)
                //    TaskPresenter.LoadWFInstance((userAckItem?.UserAckPWNode.ValueT as IACComponentPWNode).ParentRootWFNode);
            }
        }

        private void HandleAlarms(bool hasAlarms)
        {
            string alarmsAsText = _AlarmsAsText.ValueT;
            if (alarmsAsText == _AlarmsAsTextCache)
                return;

            if (hasAlarms)
            {
                var alarms = CurrentProcessModule?.ExecuteMethod("GetAlarms", true, true, true) as List<Msg>;
                if (alarms == null)
                    return;

                var messagesToRemove = MessagesList.Where(c => c.GetType() == typeof(MessageItem) && c.UserAckPWNode == null && !alarms.Any(x => BuildAlarmMessage(x) == c.Message)).ToArray();
                foreach (var messageToRemove in messagesToRemove)
                    RemoveFromMessageList(messageToRemove);

                foreach (var alarm in alarms)
                {
                    MessageItem msgItem = new MessageItem(null, null);
                    msgItem.Message = BuildAlarmMessage(alarm);
                    AddToMessageList(msgItem);
                }
            }
            else if (MessagesList != null)
            {
                var messageList = MessagesList.Where(c => c.UserAckPWNode == null && c.HandleByAcknowledgeButton).ToArray();
                foreach (var messageItem in messageList)
                    RemoveFromMessageList(messageItem);
            }

            _AlarmsAsTextCache = _AlarmsAsText.ValueT;
        }

        private string BuildAlarmMessage(Msg msg)
        {
            return string.Format("{0}: {1} {2}", msg.Source, msg.ACCaption, msg.Message);
        }

        #endregion

        #region Methods => Misc.

        [ACMethodInfo("", "en{'Refresh material and lots'}de{'Refresh material and lots'}", 650)]
        public void RefreshMaterialOrFC_F()
        {
            OnPropertyChanged("WeighingMaterialList");
            if (SelectedWeighingMaterial != null)
            {
                _FacilityChargeList = null;
                _FacilityList = null;
                OnPropertyChanged("MaterialF_FCList");
            }
        }

        public bool IsEnabledRefreshMaterialOrFC_F()
        {
            return SelectedMaterialF_FC == null || _CallPWLotChange;
        }

        public bool AddToMessageList(MessageItem messageItem)
        {
            if (MessagesList == null)
                return false;

            if (MessagesList.Any(c => c.IsAlarmMessage && c.Message == messageItem.Message))
                return true;

            List<MessageItem> msgList = new List<MessageItem>(MessagesList);
            if (messageItem.IsAlarmMessage)
                msgList.Add(messageItem);
            else
                msgList.Insert(0, messageItem);

            MessagesList = msgList;
            return true;
        }

        public bool RemoveFromMessageList(MessageItem messageItem)
        {
            if (MessagesList == null)
                return false;

            List<MessageItem> msgList = new List<MessageItem>(MessagesList);
            msgList.Remove(messageItem);

            MessagesList = msgList;
            return true;
        }

        protected ScaleBackgroundState DetermineBackgroundState(double? tolPlus, double? tolMinus, double target, double actual)
        {
            if (!tolPlus.HasValue)
                tolPlus = 0;

            if (!tolMinus.HasValue)
                tolMinus = 0;

            if (SelectedWeighingMaterial != null && SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.InWeighing && tolPlus.HasValue && tolMinus.HasValue && target > 0)
            {
                ScaleBackgroundState? result = OnDetermineBackgroundState(tolPlus, tolMinus, target, actual);
                if (result.HasValue)
                    return result.Value;

                //if (CurrentScaleObject != null && CurrentScaleObject.Value != null && !IsManualAddition)
                //{
                //    var activeScale = _ProcessModuleScales?.FirstOrDefault(c => c.ACUrl == CurrentScaleObject.GetStringValue());
                //    if (activeScale != null && activeScale.ValueT != null)
                //    {
                //        bool? isTared = activeScale.ValueT.ACUrlCommand("IsTared") as bool?;
                //        if (isTared != null && !isTared.Value)
                //            return ScaleBackgroundState.Weighing;
                //    }
                //}

                double act = Math.Round(actual, 3);

                if (act > target)
                {
                    if (act <= Math.Round(target + tolPlus.Value, 3))
                        return ScaleBackgroundState.InTolerance;
                    else
                        return ScaleBackgroundState.OutTolerance;
                }
                else
                {
                    if (act >= Math.Round(target - tolMinus.Value, 3))
                        return ScaleBackgroundState.InTolerance;
                }
            }
            return ScaleBackgroundState.Weighing;
        }

        public virtual ScaleBackgroundState? OnDetermineBackgroundState(double? tolPlus, double? tolMinus, double target, double actual)
        {
            //if (CurrentScaleObject != null && CurrentScaleObject.Value != null)
            //{
            //    var activeScale = _ProcessModuleScales?.FirstOrDefault(c => c.ACUrl == CurrentScaleObject.GetStringValue());
            //    if (activeScale != null && activeScale.ValueT != null)
            //    {
            //        bool? isTareRequestAck = activeScale.ValueT.ACUrlCommand("IsTareRequestAcknowledged") as bool?;
            //        if (isTareRequestAck != null && isTareRequestAck.Value)
            //            return ScaleBackgroundState.Weighing;
            //    }
            //}

            if (_TareScaleState.ValueT != (short)PAFManualWeighing.TareScaleStateEnum.TareOK)
                return ScaleBackgroundState.Weighing;
            return null;
        }

        private void SelectActiveScaleObject(WeighingMaterial weighingMaterial)
        {
            if (ScaleObjectsList != null && weighingMaterial.WeighingMatState == WeighingComponentState.InWeighing)
            {
                SelectActiveScaleObject();
            }
            else
                OnPropertyChanged("CurrentScaleObject");
        }

        private void SelectActiveScaleObject(bool setIfNotSelected = false)
        {
            if (ScaleObjectsList == null)
                return;

            if (ScaleObjectsList.Count > 1)
            {
                string activeScaleACUrl = CurrentPAFManualWeighing?.ACUrlCommand("!GetActiveScaleObjectACUrl") as string;
                if (!string.IsNullOrEmpty(activeScaleACUrl))
                {
                    MainSyncContext.Send((object state) =>
                    {
                        CurrentScaleObject = ScaleObjectsList.FirstOrDefault(c => c.Value as string == activeScaleACUrl);
                    }, new object());
                }
                else if (setIfNotSelected)
                {
                    MainSyncContext.Send((object state) =>
                    {
                        CurrentScaleObject = ScaleObjectsList.FirstOrDefault();
                    }, new object());
                }
            }
            else
            {
                MainSyncContext.Send((object state) =>
                {
                    CurrentScaleObject = ScaleObjectsList.FirstOrDefault();
                }, new object());
            }
        }

        public virtual void OnComponentStateChanged(WeighingMaterial weighingMaterial)
        {
            if (weighingMaterial != null && (weighingMaterial.WeighingMatState == WeighingComponentState.WeighingCompleted || weighingMaterial.WeighingMatState == WeighingComponentState.Aborted))
            {
                BSOWorkCenterSelector sel = ParentACComponent as BSOWorkCenterSelector;
                if (sel != null)
                {
                    sel.LoadPartslist();
                }
            }
        }

        #endregion

        /// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        /// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        /// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        /// <returns>ControlModesInfo</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            Global.ControlModes result = base.OnGetControlModes(vbControl);

            if (vbControl == null || string.IsNullOrEmpty(vbControl.VBContent))
                return result;

            switch (vbControl.VBContent)
            {
                case "SelectedWeighingMaterial":
                    {
                        if (WeighingMaterialsFSM)
                            return Global.ControlModes.Enabled;
                        return Global.ControlModes.Disabled;
                    }
                case "SelectedMaterialF_FC":
                    {
                        if (ShowSelectFacilityLotInfo)
                            return Global.ControlModes.Enabled;
                        return Global.ControlModes.Disabled;
                    }
                case "EnterLotManually":
                    {
                        if (EnterLotManually)
                            return Global.ControlModes.Enabled;
                        return Global.ControlModes.Collapsed;
                    }
                case "CurrentScaleObject":
                    {
                        if (SelectedWeighingMaterial != null && SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.InWeighing)
                            return Global.ControlModes.Disabled;
                        return Global.ControlModes.Enabled;
                    }
            }

            return result;
        }

        #endregion 

        #region Handle execute helper

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "Weigh":
                    Weigh();
                    return true;
                case "IsEnabledWeigh":
                    result = IsEnabledWeigh();
                    return true;
                case "Acknowledge":
                    Acknowledge();
                    return true;
                case "IsEnabledAcknowledge":
                    result = IsEnabledAcknowledge();
                    return true;
                case "Tare":
                    Tare();
                    return true;
                case "IsEnabledTare":
                    result = IsEnabledTare();
                    return true;
                case "LotChange":
                    LotChange();
                    return true;
                case "IsEnabledLotChange":
                    result = IsEnabledLotChange();
                    return true;
                case "BinChange":
                    BinChange();
                    return true;
                case "IsEnabledBinChange":
                    result = IsEnabledBinChange();
                    return true;
                case "Abort":
                    Abort();
                    return true;
                case "IsEnabledAbort":
                    result = IsEnabledAbort();
                    return true;
                case "ApplyLot":
                    ApplyLot();
                    return true;
                case "IsEnabledApplyLot":
                    result = IsEnabledApplyLot();
                    return true;
                case "AddKg":
                    AddKg();
                    return true;
                case "IsEnabledAddKg":
                    result = IsEnabledAddKg();
                    return true;
                case "RemoveKg":
                    RemoveKg();
                    return true;
                case "IsEnabledRemoveKg":
                    result = IsEnabledRemoveKg();
                    return true;
                case "RefreshMaterialOrFC_F":
                    RefreshMaterialOrFC_F();
                    return true;
                case "IsEnabledRefreshMaterialOrFC_F":
                    result = IsEnabledRefreshMaterialOrFC_F();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }

    #region Helper

    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'WeighingMaterial'}de{'WeighingMaterial'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, true)]
    public class WeighingMaterial : IACObject, INotifyPropertyChanged
    {
        #region c'tors

        public WeighingMaterial(vd.ProdOrderPartslistPosRelation posRelation, WeighingComponentState state, ACClassDesign materialIconDesign, IACObject parent)
        {
            PosRelation = posRelation;
            MaterialUnitList = PosRelation?.SourceProdOrderPartslistPos?.Material.MaterialUnit_Material.ToArray();
            WeighingMatState = state;
            MaterialIconDesign = materialIconDesign;
            _ParentACObject = parent;
            OnPropertyChanged("MaterialUnitList");
        }

        #endregion

        #region Properties

        private vd.ProdOrderPartslistPosRelation _PosRelation;
        [ACPropertyInfo(100)]
        public vd.ProdOrderPartslistPosRelation PosRelation
        {
            get => _PosRelation;
            set
            {
                _PosRelation = value;
                if (_PosRelation == null)
                {
                    MaterialName = null;
                    MaterialNo = null;
                    IsLotManaged = false;
                    TargetQuantity = 0;
                    ActualQuantity = 0;
                }
                else
                {
                    MaterialName = _PosRelation.SourceProdOrderPartslistPos?.Material?.MaterialName1;
                    MaterialNo = _PosRelation.SourceProdOrderPartslistPos?.Material?.MaterialNo;
                    IsLotManaged = _PosRelation.SourceProdOrderPartslistPos != null && _PosRelation.SourceProdOrderPartslistPos.Material != null ?
                                   _PosRelation.SourceProdOrderPartslistPos.Material.IsLotManaged : false;

                    if (_PosRelation.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)vd.MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed ||
                        _PosRelation.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)vd.MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled)
                    {
                        TargetQuantity = _PosRelation.TargetQuantityUOM;
                        ActualQuantity = _PosRelation.ActualQuantityUOM;
                    }
                    else
                    {
                        TargetQuantity = Math.Abs(PosRelation.RemainingDosingQuantityUOM);
                        ActualQuantity = 0;
                    }
                }
                OnPropertyChanged("PosRelation");
            }
        }

        private string _MaterialName;
        [ACPropertyInfo(101, "", "en{'Material Desc. 1'}de{'Materialbez. 1'}")]
        public string MaterialName
        {
            get => _MaterialName;
            set
            {
                _MaterialName = value;
                OnPropertyChanged("MaterialName");
            }
        }

        private string _MaterialNo;
        [ACPropertyInfo(102, "", "en{'Material No.'}de{'Material-Nr.'}")]
        public string MaterialNo
        {
            get => _MaterialNo;
            set
            {
                _MaterialNo = value;
                OnPropertyChanged("MaterialNo");
            }
        }

        private bool _IsLotManaged = false;
        [ACPropertyInfo(103)]
        public bool IsLotManaged
        {
            get => _IsLotManaged;
            set
            {
                _IsLotManaged = value;
                OnPropertyChanged("IsLotManaged");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        internal void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private WeighingComponentState _WeighingMaterialState;
        [ACPropertyInfo(104, "", "en{'Weighed'}de{'Gewogen'}")]
        public WeighingComponentState WeighingMatState
        {
            get => _WeighingMaterialState;
            set
            {

                _WeighingMaterialState = value;
                OnPropertyChanged("WeighingMatState");
            }
        }

        [ACPropertyInfo(105)]
        public ACClassDesign MaterialIconDesign
        {
            get;
            set;
        }

        private double _TargetQuantity;
        [ACPropertyInfo(106)]
        public double TargetQuantity
        {
            get => _TargetQuantity;
            set
            {
                _TargetQuantity = value;
                OnPropertyChanged("TargetQuantity");
            }
        }

        private double _ActualQuantity;
        [ACPropertyInfo(107)]
        public double ActualQuantity
        {
            get => _ActualQuantity;
            set
            {
                _ActualQuantity = value;
                OnPropertyChanged("ActualQuantity");
            }
        }

        public BSOManualWeighing ParentBSO
        {
            get => ParentACObject as BSOManualWeighing;
        }

        #endregion

        #region IACObject

        /// <summary>Unique Identifier in a Parent-/Child-Relationship.</summary>
        /// <value>The Unique Identifier as string</value>
        public string ACIdentifier => PosRelation?.ACIdentifier;

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        public string ACCaption => PosRelation?.ACCaption;

        /// <summary>
        /// Metadata (iPlus-Type) of this instance. ATTENTION: IACType are EF-Objects. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>  iPlus-Type (EF-Object from ACClass*-Tables)</value>
        public IACType ACType => this.ReflectACType();

        /// <summary>
        /// A "content list" contains references to the most important data that this instance primarily works with. It is primarily used to control the interaction between users, visual objects, and the data model in a generic way. For example, drag-and-drop or context menu operations. A "content list" can also be null.
        /// </summary>
        /// <value> A nullable list ob IACObjects.</value>
        public IEnumerable<IACObject> ACContentList => this.ReflectGetACContentList();

        private IACObject _ParentACObject = null;
        /// <summary>
        /// Returns the parent object
        /// </summary>
        /// <value>Reference to the parent object</value>
        public IACObject ParentACObject
        {
            get { return _ParentACObject; }
        }

        /// <summary>
        /// The ACUrlCommand is a universal method that can be used to query the existence of an instance via a string (ACUrl) to:
        /// 1. get references to components,
        /// 2. query property values,
        /// 3. execute method calls,
        /// 4. start and stop Components,
        /// 5. and send messages to other components.
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>Result if a property was accessed or a method was invoked. Void-Methods returns null.</returns>
        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectACUrlCommand(acUrl, acParameter);
        }

        /// <summary>
        /// This method is called before ACUrlCommand if a method-command was encoded in the ACUrl
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>true if ACUrlCommand can be invoked</returns>
        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
        }

        /// <summary>
        /// Returns a ACUrl relatively to the passed object.
        /// If the passed object is null then the absolute path is returned
        /// </summary>
        /// <param name="rootACObject">Object for creating a realtive path to it</param>
        /// <returns>ACUrl as string</returns>
        public string GetACUrl(IACObject rootACObject = null)
        {
            return this.ReflectGetACUrl(rootACObject);
        }

        /// <summary>
        /// Method that returns a source and path for WPF-Bindings by passing a ACUrl.
        /// </summary>
        /// <param name="acUrl">ACUrl of the Component, Property or Method</param>
        /// <param name="acTypeInfo">Reference to the iPlus-Type (ACClass)</param>
        /// <param name="source">The Source for WPF-Databinding</param>
        /// <param name="path">Relative path from the returned source for WPF-Databinding</param>
        /// <param name="rightControlMode">Information about access rights for the requested object</param>
        /// <returns><c>true</c> if binding could resolved for the passed ACUrl<c>false</c> otherwise</returns>
        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        #endregion

        #region Properties => MaterialUnit/Quantity

        private double? _AddValue;
        [ACPropertyInfo(108, "", "en{'Quantity to add'}de{'Menge zum Hinzufügen'}")]
        public double? AddValue
        {
            get => _AddValue;
            set
            {
                _AddValue = value;
                OnPropertyChanged("AddValue");
            }
        }

        private vd.MaterialUnit _SelectedMaterialUnit;
        [ACPropertySelected(109, "ManAddMaterialUnit", "en{'Material unit to add'}de{'Materialeinheit zum Hinzufügen'}")]
        public vd.MaterialUnit SelectedMaterialUnit
        {
            get => _SelectedMaterialUnit;
            set
            {
                _SelectedMaterialUnit = value;
                OnPropertyChanged("SelectedMaterialUnit");
            }
        }

        private IEnumerable<vd.MaterialUnit> _MaterialUnitList;
        [ACPropertyList(110, "ManAddMaterialUnit")]
        public IEnumerable<vd.MaterialUnit> MaterialUnitList
        {
            get
            {
                //return _MaterialUnitList;
                if (PosRelation != null && PosRelation.SourceProdOrderPartslistPos != null)
                    return PosRelation.SourceProdOrderPartslistPos.Material.MaterialUnit_Material.ToArray();
                return null;
            }
            set
            {
                _MaterialUnitList = value;
                OnPropertyChanged("MaterialUnitList");
            }
        }

        #endregion

        #region Methods

        public double AddKg(double currentWeight)
        {
            if (AddValue.HasValue)
            {
                currentWeight += AddValue.Value;
                return currentWeight;
            }

            if (SelectedMaterialUnit != null)
            {
                currentWeight += SelectedMaterialUnit.Multiplier;
                return currentWeight;
            }

            var calcWeight = currentWeight + 1;
            return calcWeight;
        }

        public double RemoveKg(double currentWeight)
        {
            if (currentWeight <= 0)
                return currentWeight;

            if (AddValue.HasValue)
            {
                if (currentWeight - AddValue.Value > 0)
                    currentWeight -= AddValue.Value;
                else
                    currentWeight = 0;
                return currentWeight;
            }

            if (SelectedMaterialUnit != null)
            {
                if (currentWeight - SelectedMaterialUnit.Multiplier > 0)
                    currentWeight -= SelectedMaterialUnit.Multiplier;
                else
                    currentWeight = 0;
                return currentWeight;
            }

            var calcWeight = currentWeight - 1;
            return calcWeight--;
        }

        public void ChangeComponentState(WeighingComponentState newState, vd.DatabaseApp dbApp)
        {
            if (_WeighingMaterialState == WeighingComponentState.InWeighing && (newState == WeighingComponentState.WeighingCompleted || newState == WeighingComponentState.Aborted))
            {
                try
                {
                    using (ACMonitor.Lock(dbApp.QueryLock_1X000))
                        PosRelation.AutoRefresh();
                    ActualQuantity = TargetQuantity + PosRelation.RemainingDosingQuantityUOM;
                }
                catch
                {

                }
            }
            else if ((_WeighingMaterialState == WeighingComponentState.Aborted || _WeighingMaterialState == WeighingComponentState.WeighingCompleted) &&
                      (newState == WeighingComponentState.ReadyToWeighing))
            {
                try
                {
                    using (ACMonitor.Lock(dbApp.QueryLock_1X000))
                    {
                        PosRelation.AutoRefresh();
                        PosRelation.FacilityBooking_ProdOrderPartslistPosRelation.AutoLoad();
                    }
                    TargetQuantity = Math.Abs(PosRelation.RemainingDosingQuantityUOM);
                    ActualQuantity = 0;
                }
                catch
                {

                }
            }
            WeighingMatState = newState;
            ParentBSO?.OnComponentStateChanged(this);
        }

        #endregion
    }

    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'UserAckNode'}de{'UserAckNode'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, true)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + "UserAckNode", "", typeof(MessageItem), "UserAckNode", "", "")]
    public class MessageItem : INotifyPropertyChanged
    {

        public MessageItem(IACComponent pwNode, IACComponent bso)
        {
            if (pwNode != null)
            {
                UserAckPWNode = new ACRef<IACComponent>(pwNode, bso);
                _AlarmsAsText = UserAckPWNode.ValueT.GetPropertyNet("AlarmsAsText");
                if (_AlarmsAsText != null)
                {
                    _AlarmsAsText.PropertyChanged += AlarmsAsText_PropertyChanged;
                    Message = _AlarmsAsText.Value as string;
                }
            }
        }

        private IACPropertyNetBase _AlarmsAsText;

        private string _Message;
        [ACPropertyInfo(100)]
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                if (_Message != value)
                {
                    _Message = value;
                    OnPropertyChanged("Message");
                }
            }
        }

        public virtual bool IsAlarmMessage => UserAckPWNode == null;

        public virtual bool HandleByAcknowledgeButton
        {
            get
            {
                return true;
            }
        }

        public ACRef<IACComponent> UserAckPWNode
        {
            get;
            set;
        }

        private void AlarmsAsText_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
                Task.Run(() => Message = _AlarmsAsText.Value as string);
        }

        public virtual void Acknowledge()
        {
            if (UserAckPWNode != null && UserAckPWNode.ValueT != null)
            {
                UserAckPWNode.ValueT.ACUrlCommand("!" + PWNodeUserAck.MN_AckStartClient);
                //MN_AckStartClient
                //PWNodeUserAck.AckStartClient(UserAckPWNode.ValueT);
            }
        }

        public virtual void DeInit()
        {
            if (UserAckPWNode == null)
                return;

            if (_AlarmsAsText != null)
                _AlarmsAsText.PropertyChanged -= AlarmsAsText_PropertyChanged;
            _AlarmsAsText = null;
            UserAckPWNode.Detach();
            UserAckPWNode = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class ManualWeighingPWNode
    {
        public ManualWeighingPWNode(ACRef<IACComponentPWNode> componentPWNode)
        {
            ComponentPWNode = componentPWNode;
        }

        private ACRef<IACComponentPWNode> _ComponentPWNode;
        public ACRef<IACComponentPWNode> ComponentPWNode
        {
            get => _ComponentPWNode;
            set
            {
                if (value != null)
                {
                    ComponentPWNodeACState = value.ValueT.GetPropertyNet(Const.ACState) as IACContainerTNet<ACStateEnum>;
                }
                _ComponentPWNode = value;
            }
        }

        public IACContainerTNet<ACStateEnum> ComponentPWNodeACState
        {
            get;
            set;
        }

        public void Deinit()
        {
            ComponentPWNodeACState = null;
            ComponentPWNode?.Detach();
            ComponentPWNode = null;
        }

    }

    public enum ScaleBackgroundState : short
    {
        Weighing = 0,
        InTolerance = 10,
        OutTolerance = 20
    }

    #endregion
}
