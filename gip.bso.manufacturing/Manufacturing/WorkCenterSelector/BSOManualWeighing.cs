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
using gip.mes.facility;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Manual weighing'}de{'Handverwiegung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, SortIndex = 100)]
    public class BSOManualWeighing : BSOWorkCenterMessages
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

            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            DeactivateManualWeighingModel();
            _DefaultMaterialIcon = null;

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

        protected bool IsCurrentProcessModuleNull
        {
            get;
            private set;
        }
        private ACComponent _CurrentProcessModule;
        [ACPropertyInfo(601)]
        public override  ACComponent CurrentProcessModule
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentProcessModule;
                }
            }
            protected set
            {
                DeactivateManualWeighingModel();
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _CurrentProcessModule = value;
                }
                OnPropertyChanged("CurrentProcessModule");
                if (value != null)
                {
                    IsCurrentProcessModuleNull = false;
                    ParentBSOWCS.ApplicationQueue.Add(() => ActivateManualWeighingModel());
                }
                else
                {
                    IsCurrentProcessModuleNull = true;
                }
            }
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


        #region Private fields

        private bool _CallPWLotChange = false, _IsLotConsumed = false, _StartWeighingFromF_FC = false;

        private ACMonitorObject _70500_ComponentPWNodeLock = new ACMonitorObject(70500);
        private ACMonitorObject _70600_CurrentOrderInfoValLock = new ACMonitorObject(70600);
        private ACMonitorObject _70700_PrivateMemberLock = new ACMonitorObject(70700);
        private ACMonitorObject _70750_ProcessModuleScalesLock = new ACMonitorObject(70750);

        private IACContainerT<string> _OrderInfo;
        private IACContainerT<WeighingComponentInfo> _WeighingComponentInfo;
        private IACContainerT<double> _ScaleActualWeight;
        private IACContainerT<double> _ScaleActualValue;

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
                    ACRef<IACComponent> scaleRef = null;
                    using (ACMonitor.Lock(_70750_ProcessModuleScalesLock))
                    {
                        scaleRef = _ProcessModuleScales?.FirstOrDefault(c => c.ACUrl == _CurrentScaleObject.Value.ToString());
                    }
                    if (scaleRef != null)
                        ActivateScale(scaleRef.ValueT);
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

        protected double _ScaleRealWeight;
        /// <summary>
        /// The weight which is really in a pyhisical scale.
        /// </summary>
        public virtual double ScaleRealWeight
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

        private double _ScaleGrossWeight;
        [ACPropertyInfo(605)]
        public double ScaleGrossWeight
        {
            get => _ScaleGrossWeight;
            set
            {
                _ScaleGrossWeight = value;
                OnPropertyChanged();
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
                ScaleBckgrState = DetermineBackgroundState(_TolerancePlus, _ToleranceMinus, _TargetWeight, ScaleActualWeight);

                DelegateToMainThread((object state) => OnTargetWeightChanged(value));
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

        public double? MaxScaleWeight
        {
            get;
            set;
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

        [ACPropertyInfo(608)]
        public ACRef<IACComponentPWNode> ComponentPWNode
        {
            get;
            set;
        }

        public IACComponentPWNode ComponentPWNodeLocked
        {
            get
            {
                using (ACMonitor.Lock(_70500_ComponentPWNodeLock))
                {
                    return ComponentPWNode?.ValueT;
                }    
            }
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

        #region Properties => PW&PAFConfiguration

        protected double _TolerancePlus = 0, _ToleranceMinus = 0;

        private bool _WeighingMaterialsFSM;
        public bool WeighingMaterialsFSM
        {
            get => _WeighingMaterialsFSM;
            set
            {
                _WeighingMaterialsFSM = value;
                OnPropertyChanged("WeighingMaterialsFSM");
            }
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

        //TODO: check if this necessary
        private vd.ProdOrderPartslistPos _EndBatchPos;
        [ACPropertyInfo(613)]
        public vd.ProdOrderPartslistPos EndBatchPos
        {
            get => _EndBatchPos;
            set
            {
                _EndBatchPos = value;
                OnPropertyChanged("EndBatchPos");
            }
        }

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

        private IACContainerTNet<ManualWeighingTaskInfo> _NextTaskInfoProperty;

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

        private vd.FacilityCharge _SelectedLastUsedLot;
        [ACPropertySelected(690, "LastUsedLot")]
        public vd.FacilityCharge SelectedLastUsedLot
        {
            get => _SelectedLastUsedLot;
            set
            {
                _SelectedLastUsedLot = value;
                OnPropertyChanged("SelectedLastUsedLot");
            }
        }

        private List<vd.FacilityCharge> _LastUsedLotList;
        [ACPropertyList(690, "LastUsedLot")]
        public List<vd.FacilityCharge> LastUsedLotList
        {
            get => _LastUsedLotList;
            set
            {
                _LastUsedLotList = value;
                OnPropertyChanged("LastUsedLotList");
            }
        }

        private double? _InInterdischargingQ = null;

        public double? InInterdischargingQ
        {
            get => _InInterdischargingQ;
            set
            {
                _InInterdischargingQ = value;
                OnPropertyChanged("InInterdischargingQ");
                InterdischargingCompleteQ = _InInterdischargingQ.HasValue ? _InInterdischargingQ / 2 : _InInterdischargingQ;
            }
        }

        public virtual double? InterdischargingCompleteQ
        {
            get;
            set;
        }

        private bool _IsEnabledCompleteInterdischarging = false;

        public enum AbortModeEnum : short
        {
            Cancel = 0,
            AbortComponent = 10,
            AbortComponentSwitchToEmptyingMode = 20,
            SwitchToEmptyingMode = 30,
            Interdischarging = 40
        }

        private AbortModeEnum _AbortMode;


        #endregion

        #region Properties => Components and Facility/FacilityCharge selection

        private WeighingMaterial _SelectedWeighingMaterial;
        [ACPropertySelected(624, "WeighingMaterial")]
        public virtual WeighingMaterial SelectedWeighingMaterial
        {
            get => _SelectedWeighingMaterial;
            set
            {
                if (_SelectedWeighingMaterial != value)
                {
                    _SelectedWeighingMaterial = value;
                    _FacilityChargeList = null;
                    OnPropertyChanged("SelectedWeighingMaterial");
                    OnPropertyChanged("FacilityChargeList");
                    FacilityChargeNo = null;
                    ScaleAddAcutalWeight = _PAFManuallyAddedQuantity != null ? _PAFManuallyAddedQuantity.ValueT : 0;
                    SelectedFacilityCharge = null;
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

        private FacilityChargeItem _SelectedFacilityCharge;
        [ACPropertySelected(628, "FacilityCharge")]
        public virtual FacilityChargeItem SelectedFacilityCharge
        {
            get => _SelectedFacilityCharge;
            set
            {
                _SelectedFacilityCharge = value;
                if (value != null)
                    ShowSelectFacilityLotInfo = false;

                OnPropertyChanged("SelectedFacilityCharge");

                IACComponentPWNode componentPWNode = ComponentPWNodeLocked;

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
                else if (_CallPWLotChange && value != null && componentPWNode != null)
                {
                    double weight = OnDetermineLotChangeActualQuantity();
                    Msg msg = componentPWNode.ExecuteMethod(nameof(PWManualWeighing.LotChange), value.FacilityChargeID, weight, _IsLotConsumed, false) as Msg;
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

        FacilityChargeItem[] _FacilityChargeList;
        [ACPropertyList(629, "FacilityCharge")]
        public virtual IEnumerable<FacilityChargeItem> FacilityChargeList
        {
            get
            {
                try
                {
                    IACComponentPWNode componentPWNode = ComponentPWNodeLocked;

                    if (_FacilityChargeList == null && SelectedWeighingMaterial != null)
                    {
                        ACValueList facilityCharges = componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.GetAvailableFacilityCharges), 
                                                                                     SelectedWeighingMaterial.PosRelation.ProdOrderPartslistPosRelationID) as ACValueList;
                        if (facilityCharges == null)
                            return null;
                        using (vd.DatabaseApp dbApp = new vd.DatabaseApp())
                            _FacilityChargeList = facilityCharges.Select(c => new FacilityChargeItem(dbApp.FacilityCharge.Include("FacilityLot").Include("MDUnit").Include("Material")
                                                                                                             .FirstOrDefault(x => x.FacilityChargeID == c.ParamAsGuid), TargetWeight)).ToArray();
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
        [ACPropertyInfo(632)]
        public string PAFCurrentMaterial
        {
            get => _PAFCurrentMaterial;
            set
            {
                _PAFCurrentMaterial = value;
                OnPropertyChanged("PAFCurrentMaterial");
                ShowScaleGross = string.IsNullOrEmpty(_PAFCurrentMaterial);
            }
        }

        private bool _ShowScaleGross;
        [ACPropertyInfo(632)]
        public bool ShowScaleGross
        {
            get => _ShowScaleGross;
            set
            {
                _ShowScaleGross = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Properties => SingleDosing

        private SingleDosingItem _SelectedSingleDosingItem;
        [ACPropertySelected(650, "SingleDosing", "en{'Single dosing'}de{'Single dosing'}")]
        public SingleDosingItem SelectedSingleDosingItem
        {
            get => _SelectedSingleDosingItem;
            set
            {
                _SelectedSingleDosingItem = value;
                OnPropertyChanged("SelectedSingleDosingItem");
            }
        }

        [ACPropertyList(651, "SingleDosing", "")]
        public SingleDosingItems SingleDosingItemList
        {
            get;
            set;
        }

        private double _SingleDosTargetQuantity;
        [ACPropertyInfo(652, "", "en{'Target weight'}de{'Sollgewicht'}")]
        public double SingleDosTargetQuantity
        {
            get => _SingleDosTargetQuantity;
            set
            {
                _SingleDosTargetQuantity = value;
                OnPropertyChanged("SingleDosTargetQuantity");
            }
        }

        protected ACClass _SelectedSingleDosTargetStorage;
        
        [ACPropertySelected(653, "SingleDosTargetStorage", "en{'Destination'}de{'Ziel'}")]
        public virtual ACClass SelectedSingleDosTargetStorage
        {
            get => _SelectedSingleDosTargetStorage;
            set
            {
                _SelectedSingleDosTargetStorage = value;
                OnPropertyChanged("SelectedSingleDosTargetStorage");
            }
        }

        [ACPropertyList(653, "SingleDosTargetStorage")]
        public virtual IEnumerable<ACClass> SingleDosTargetStorageList
        {
            get;
            set;
        }

        #endregion

        #endregion

        #region Methods

        public override void Activate(ACComponent selectedProcessModule)
        {
            base.Activate(selectedProcessModule);
            CurrentProcessModule = selectedProcessModule;
        }

        public override void DeActivate()
        {
            base.DeActivate();
            CurrentProcessModule = null;

            if (_RoutingService != null)
            {
                _RoutingService.Detach();
                _RoutingService = null;
            }

            if (_ACFacilityManager != null)
            {
                _ACFacilityManager.Detach();
                _ACFacilityManager = null;
            }
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
            return true; //ComponentPWNode != null && ComponentPWNode.ValueT != null;
        }

        public Msg StartWeighing(bool forceSetFC_F)
        {
            IACComponentPWNode componentPWNode = null;

            using (ACMonitor.Lock(_70500_ComponentPWNodeLock))
            {
                componentPWNode = CurrentComponentPWNode;
            }

            if (componentPWNode == null)
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
                    Msg msg = componentPWNode.ExecuteMethod(nameof(PWManualWeighing.StartWeighing), SelectedWeighingMaterial.PosRelation.ProdOrderPartslistPosRelationID,
                                                                             SelectedFacilityCharge?.FacilityChargeID, null, forceSetFC_F) as Msg;
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

            var messagesToAck = MessagesList.Where(c => !c.IsAlarmMessage && c.HandleByAcknowledgeButton).ToList();

            IACComponentPWNode componentPWNode = ComponentPWNodeLocked;

            if (messagesToAck.Count > 1 || (messagesToAck.Any() && (ScaleBckgrState == ScaleBackgroundState.InTolerance)) 
                                        || messagesToAck.Any(x => x.MessageLevel == eMsgLevel.Question))
            {
                if (ScaleBckgrState == ScaleBackgroundState.InTolerance)
                {
                    string text = Root.Environment.TranslateText(this, "txtAckWeighComp");
                    if (string.IsNullOrEmpty(text))
                        text = "Acknowledge weighing component: {0} {1} ";

                    MessageItem msgItem = new MessageItem(componentPWNode, this);
                    msgItem.Message = string.Format(text, SelectedWeighingMaterial?.MaterialNo, SelectedWeighingMaterial.MaterialName);
                    messagesToAck.Add(msgItem);
                }

                AckMessageList = messagesToAck;
                ShowDialog(this, "MsgAckDialog");
            }
            else
            {
                var messageToAck = messagesToAck.FirstOrDefault();

                if (messageToAck != null)
                    messageToAck.AcknowledgeMsg();
                else if (componentPWNode != null)
                {
                    if (ScaleBckgrState != ScaleBackgroundState.InTolerance)
                    {
                        //Question50072 : Are you sure that you want acknowledge current component?
                        if (Messages.Question(this, "Question50072") != Global.MsgResult.Yes)
                            return;
                    }

                    componentPWNode.ExecuteMethod(nameof(PWManualWeighing.CompleteWeighing), ScaleActualWeight, false);
                }
            }
        }

        public virtual bool IsEnabledAcknowledge()
        {
            return (MessagesList.Any(c => !c.IsAlarmMessage && c.HandleByAcknowledgeButton) || (MaxScaleWeight.HasValue && TargetWeight > MaxScaleWeight)
                                                                                            || ScaleBckgrState == ScaleBackgroundState.InTolerance);
        }

        [ACMethodInfo("", "en{'Acknowledge'}de{'Quittieren'}", 602)]
        public void AcknowledgeMsg(MessageItem item)
        {
            if (item != null)
            {
                item.AcknowledgeMsg();
                AckMessageList.Remove(item);
                AckMessageList = AckMessageList.ToList();

                if (!AckMessageList.Any())
                {
                    CloseTopDialog();
                }
            }
        }

        [ACMethodInfo("", "en{'Yes'}de{'Ja'}", 602)]
        public void QuestionYes(MessageItem item)
        {
            if (item != null)
            {
                item.QuestionYes();
                AckMessageList.Remove(item);
                AckMessageList = AckMessageList.ToList();

                if (!AckMessageList.Any())
                {
                    CloseTopDialog();
                }
            }
        }

        [ACMethodInfo("", "en{'No'}de{'Nein'}", 602)]
        public void QuestionNo(MessageItem item)
        {
            if (item != null)
            {
                item.QuestionNo();
                AckMessageList.Remove(item);
                AckMessageList = AckMessageList.ToList();

                if (!AckMessageList.Any())
                {
                    CloseTopDialog();
                }
            }
        }

        [ACMethodInfo("", "en{'Tare'}de{'Tarieren'}", 603)]
        public void Tare()
        {
            if (!IsEnabledTare())
                return;

            CurrentPAFManualWeighing?.ExecuteMethod(nameof(PAFManualWeighing.TareActiveScale));
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
                OnPropertyChanged("SelectedFacilityCharge");
                _CallPWLotChange = true;
                _IsLotConsumed = false;
                if (SelectedWeighingMaterial.IsLotManaged)
                {
                    //Question50047: Was the material with the lot number {0} used up?
                    // Wurde das Material mit der Chargennummer{0} aufgebraucht?
                    Global.MsgResult result = Messages.Question(this, "Question50047", Global.MsgResult.Cancel, false, SelectedFacilityCharge.FacilityChargeNo);
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

        public virtual double OnDetermineLotChangeActualQuantity()
        {
            return ScaleActualWeight;
        }

        [ACMethodInfo("", "en{'Bin change'}de{'Eimerwechsel'}", 605)]
        public virtual void BinChange()
        {
            if (!IsEnabledBinChange())
                return;

            IACComponentPWNode componentPWNode = ComponentPWNodeLocked;

            if (componentPWNode != null)
                componentPWNode.ExecuteMethod(nameof(PWManualWeighing.BinChange));
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

            IACComponentPWNode componentPWNode = ComponentPWNodeLocked;

            if (componentPWNode != null)
            {
                InterdischargeStart(componentPWNode, true);

                _AbortMode = AbortModeEnum.Cancel;
                ShowDialog(this, "AbortDialog", "", false, Global.ControlModes.Hidden, Global.ControlModes.Hidden);

                if (_AbortMode == AbortModeEnum.AbortComponent)
                {
                    ShowSelectFacilityLotInfo = false;
                    //Question50049: Do you no longer want to weigh this material in the following batches? (e.g. for rework if it has been used up)
                    // Möchten Sie dieses Material in den nachfolgenden Batchen nicht mehr verwiegen? (z.B. bei Rework wenn es aufgebraucht worden ist)
                    if (Messages.Question(this, "Question50049") == Global.MsgResult.Yes)
                    {
                        componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.Abort), true);
                        return;
                    }
                    componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.Abort), false);

                }
                else if (_AbortMode == AbortModeEnum.AbortComponentSwitchToEmptyingMode)
                {
                    ShowSelectFacilityLotInfo = false;
                    //Question50049: Do you no longer want to weigh this material in the following batches? (e.g. for rework if it has been used up)
                    // Möchten Sie dieses Material in den nachfolgenden Batchen nicht mehr verwiegen? (z.B. bei Rework wenn es aufgebraucht worden ist)

                    Global.MsgResult msgResult = Messages.Question(this, "Question50049");

                    ParentBSOWCS?.SelectExtraDisTargetOnPWGroup();

                    if (msgResult == Global.MsgResult.Yes)
                    {
                        componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.Abort), true);
                        return;
                    }
                    componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.Abort), false);
                }
                else if (_AbortMode == AbortModeEnum.SwitchToEmptyingMode)
                {
                    ParentBSOWCS?.SelectExtraDisTargetOnPWGroup();
                }
            }
            else
            {
                ParentBSOWCS?.SelectExtraDisTargetOnPWGroup();
            }
        }

        public bool IsEnabledAbort()
        {
            return true; //ComponentPWNode != null;
        }

        [ACMethodInfo("", "en{'Apply charge/lot'}de{'Charge/Los anwenden'}", 607)]
        public virtual void ApplyLot()
        {
            if (!IsEnabledApplyLot())
                return;

            Msg msg;

            IACComponentPWNode componentPWNode = ComponentPWNodeLocked;

            if (componentPWNode == null)
            {
                //Error50330: Can't start the weighing because the Reference to the workflow node is null.
                // Die Verwiegung kann nicht gstartet werden weil die Referenz zum Workflowknoten null ist.
                msg = new Msg(this, eMsgLevel.Error, "ManualWeighingModel", "OnApplyManuallyEnteredLot", 1035, "Error50330");
                Messages.Msg(msg);
                return;
            }

            msg = componentPWNode.ExecuteMethod(nameof(PWManualWeighing.OnApplyManuallyEnteredLot), FacilityChargeNo, 
                                                SelectedWeighingMaterial?.PosRelation?.ProdOrderPartslistPosRelationID) as Msg;
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
            {
                _PAFManuallyAddedQuantity.ValueT = SelectedWeighingMaterial.AddKg(_PAFManuallyAddedQuantity.ValueT);
            }
        }

        public bool IsEnabledAddKg()
        {
            return SelectedWeighingMaterial != null;
        }

        [ACMethodInfo("", "en{'- 1 kg'}de{'- 1 kg'}", 609, true)]
        public void RemoveKg()
        {
            if (IsEnabledRemoveKg())
            {
                _PAFManuallyAddedQuantity.ValueT = SelectedWeighingMaterial.RemoveKg(_PAFManuallyAddedQuantity.ValueT);
            }
        }

        public bool IsEnabledRemoveKg()
        {
            return SelectedWeighingMaterial != null;
        }

        #endregion

        #region Methods => Activation/Load

        public bool ActivateManualWeighingModel()
        {
            ACComponent currentProcessModule = CurrentProcessModule;
            if (currentProcessModule == null)
            {
                //Error50283: The manual weighing module can not be initialized. The property CurrentProcessModule is null.
                // Die Handverwiegungsstation konnte nicht initialisiert werden. Die Eigenschaft CurrentProcessModule ist null.
                Messages.Error(this, "Error50283");
                return false;
            }

            PAProcessModuleACUrl = currentProcessModule.ACUrl;
            PAProcessModuleACCaption = currentProcessModule.ACCaption;

            if (currentProcessModule.ConnectionState == ACObjectConnectionState.DisConnected)
            {
                //Info50040: The server is unreachable. Reopen the program once the connection to the server has been established.
                // Der Server ist nicht erreichbar. Öffnen Sie das Programm erneut sobal die Verbindung zum Server wiederhergestellt wurde.
                //Messages.Info(this, "Info50040");
                return false;
            }

            var processModuleChildComps = currentProcessModule.ACComponentChildsOnServer;
            IEnumerable<IACComponent> scaleObjects = null;
            IACComponent pafManWeighing = GetTargetFunction(processModuleChildComps);

            if (pafManWeighing == null)
            {
                //Error50286: The manual weighing component can not be initialized. The process module {0} has not a child component of type PAFManualWeighing.
                // Die Verwiegekomponente konnte nicht initialisiert werden. Das Prozessmodul {0} hat keine Kindkomponente vom Typ PAFManualWeighing.
                Messages.Info(this, "Error50286", false, PAProcessModuleACUrl);
                return false;
            }

            var pafManWeighingRef = new ACRef<IACComponent>(pafManWeighing, this);

            ACValueList availableScales = pafManWeighingRef.ValueT.ExecuteMethod(nameof(PAFManualWeighing.GetAvailableScaleObjects)) as ACValueList;

            if (availableScales == null)
            {
                scaleObjects = processModuleChildComps.Where(c => typeof(PAEScaleBase).IsAssignableFrom(c.ComponentClass.ObjectType)).ToArray();
            }
            else
            {
                scaleObjects = availableScales.Select(c => pafManWeighing.ACUrlCommand(c.ACIdentifier) as IACComponent);
                //scaleObjects = processModuleChildComps.Where(c => typeof(PAEScaleBase).IsAssignableFrom(c.ComponentClass.ObjectType)
                //                                                && availableScales.Any(x => x.Value is Guid && x.ParamAsGuid == c.ComponentClass.ACClassID)).ToArray();
            }

            List<ACValueItem> scaleObjectInfoList = null;

            if (scaleObjects != null && scaleObjects.Any())
            {
                using (ACMonitor.Lock(_70750_ProcessModuleScalesLock))
                {
                    _ProcessModuleScales = scaleObjects.Select(c => new ACRef<IACComponent>(c, this)).ToArray();
                    scaleObjectInfoList = new List<ACValueItem>(_ProcessModuleScales.Select(c => new ACValueItem(c.ValueT.ACCaption, c.ACUrl, null)));
                    ScaleObjectsList = scaleObjectInfoList;
                }
            }

            var orderInfo = currentProcessModule.GetPropertyNet(nameof(PAProcessModuleVB.OrderInfo));
            if (orderInfo == null)
            {
                //Error50285: Initialization error: The process module doesn't have the property {0}.
                // Initialisierungsfehler: Das Prozessmodul besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50285", false, "OrderInfo");
                return false;
            }

            ACMethod acMethod = LoadPAFManualWeighing(pafManWeighingRef);

            if (scaleObjectInfoList != null && scaleObjectInfoList.Any())
            {
                SelectActiveScaleObject(scaleObjectInfoList, true);
            }

            _OrderInfo = orderInfo as IACContainerTNet<string>;

            LoadWFNode(currentProcessModule, _OrderInfo.ValueT);

            HandlePAFCurrentACMethod(acMethod);

            orderInfo.PropertyChanged += OrderInfoPropertyChanged;

            return true;
        }

        public virtual IACComponent GetTargetFunction(IEnumerable<IACComponent> processModuleChildrenComponents)
        {
            return processModuleChildrenComponents.FirstOrDefault(c => typeof(PAFManualWeighing).IsAssignableFrom(c.ComponentClass.ObjectType)
                                                                    && !typeof(PAFManualAddition).IsAssignableFrom(c.ComponentClass.ObjectType));
        }

        public virtual void OnGetPWGroup(IACComponentPWNode pwGroup)
        {

        }

        private ACMethod LoadPAFManualWeighing(ACRef<IACComponent> pafManWeighing)
        {
            UnloadPAFManualWeighing();

            _CurrentPAFManualWeighing = pafManWeighing;

            var currentACMethod = pafManWeighing.ValueT.GetPropertyNet(nameof(PAProcessFunction.CurrentACMethod));
            if (currentACMethod == null)
            {
                //Error50287: Initialization error: The weighing function doesn't have the property {0}.
                // Initialisierungsfehler: Die Verwiegefunktion besitzt nicht die Eigenschaft {0}.
                Messages.Info(this, "Error50287", false, "CurrentACMethod");
                return null;
            }

            var manuallyAddedQuantity = pafManWeighing.ValueT.GetPropertyNet(nameof(PAFManualWeighing.ManuallyAddedQuantity));
            if (manuallyAddedQuantity == null)
            {
                //Error50287: Initialization error: The weighing function doesn't have the property {0}.
                // Initialisierungsfehler: Die Verwiegefunktion besitzt nicht die Eigenschaft {0}.
                Messages.Info(this, "Error50287", false, "ManuallyAddedQuantity");
                return null;
            }

            var tareScaleState = pafManWeighing.ValueT.GetPropertyNet(nameof(PAFManualWeighing.TareScaleState));
            if (tareScaleState == null)
            {
                //Error50287: Initialization error: The weighing function doesn't have the property {0}.
                // Initialisierungsfehler: Die Verwiegefunktion besitzt nicht die Eigenschaft {0}.
                Messages.Info(this, "Error50287", false, "TareScaleState");
                return null;
            }

            var tempMethod = currentACMethod as IACContainerTNet<ACMethod>;
            tempMethod.PropertyChanged += PAFCurrentACMethodPropChanged;
            ACMethod temp = tempMethod?.ValueT?.Clone() as ACMethod;

            using (ACMonitor.Lock(_70700_PrivateMemberLock))
            {
                _PAFCurrentACMethod = tempMethod;
            }

            _PAFManuallyAddedQuantity = manuallyAddedQuantity as IACContainerTNet<double>;
            (_PAFManuallyAddedQuantity as IACPropertyNetBase).PropertyChanged += PAFManuallyAddedQuantityPropChanged;
            ScaleAddAcutalWeight = _PAFManuallyAddedQuantity.ValueT;

            _TareScaleState = tareScaleState as IACContainerT<short>;

            return temp;
        }

        private void ActivateScale(IACComponent scale)
        {
            if (scale == null)
                return;

            var actWeightProp = scale.GetPropertyNet(nameof(PAEScaleBase.ActualWeight));
            if (actWeightProp == null)
            {
                //Error50292: Initialization error: The scale component doesn't have the property {0}.
                // Initialisierungsfehler: Die Waagen-Komponente besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50292", false, "ActualWeight");
                return;
            }

            MaxScaleWeight = null;
            var actValProp = scale.GetPropertyNet(nameof(PAEScaleBase.ActualValue)) as IACContainerTNet<double>;
            if (actValProp == null)
            {
                //Error50292: Initialization error: The scale component doesn't have the property {0}.
                // Initialisierungsfehler: Die Waagen-Komponente besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50292", false, "ActualValue");
                return;
            }

            _ScaleActualValue = actValProp;

            _ScaleActualWeight = actWeightProp as IACContainerTNet<double>;
            (_ScaleActualWeight as IACPropertyNetBase).PropertyChanged += ActWeightProp_PropertyChanged;
            (_ScaleActualValue as IACPropertyNetBase).PropertyChanged += ScaleActualValue_PropertyChanged;
            ScaleRealWeight = _ScaleActualWeight.ValueT;
            ScaleGrossWeight = _ScaleActualValue.ValueT;


            if (CurrentPAFManualWeighing != null && scale != null )
            {
                CurrentPAFManualWeighing.ExecuteMethod(nameof(PAFManualWeighing.SetActiveScaleObject), scale.ACIdentifier);
            }

            OnPropertyChanged(nameof(CurrentScaleObject));
        }

        private void LoadWFNode(ACComponent currentProcessModule, string orderInfo)
        {
            //string orderInfo = null;
            using (ACMonitor.Lock(_70600_CurrentOrderInfoValLock))
            {
                if (_CurrentOrderInfoValue == orderInfo)
                    return;
                _CurrentOrderInfoValue = orderInfo;
            }

            UnloadWFNode();

            if (!string.IsNullOrEmpty(orderInfo))
            {
                try
                {
                    string[] accessArr = (string[])currentProcessModule?.ExecuteMethod(nameof(PAProcessModule.SemaphoreAccessedFrom));
                    if (accessArr == null || !accessArr.Any())
                    {
                        using (ACMonitor.Lock(_70600_CurrentOrderInfoValLock))
                        {
                            _CurrentOrderInfoValue = null;
                            Messages.LogError(this.GetACUrl(), "LoadWFNode(10)", "Returned");
                            return;
                        }
                    }

                    string pwGroupACUrl = accessArr[0];
                    IACComponentPWNode pwGroup = null;

                    pwGroup = Root.ACUrlCommand(pwGroupACUrl) as IACComponentPWNode;
                    if (pwGroup == null)
                    {
                        using (ACMonitor.Lock(_70600_CurrentOrderInfoValLock))
                        {
                            _CurrentOrderInfoValue = null;
                            Messages.LogError(this.GetACUrl(), "LoadWFNode(20)", "Returned");
                            return;
                        }
                    }

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
                    {
                        using (ACMonitor.Lock(_70600_CurrentOrderInfoValLock))
                        {
                            _CurrentOrderInfoValue = null;
                            Messages.LogError(this.GetACUrl(), "LoadWFNode(30)", "Returned");
                            return;
                        }
                    }

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

                        mwPWNodes.Add(mwPWNode);
                    }

                    using (ACMonitor.Lock(_70500_ComponentPWNodeLock))
                    {
                        ComponentPWNodesList = mwPWNodes;
                    }

                    ManualWeighingPWNode runningNode = mwPWNodes.FirstOrDefault(c => c.ComponentPWNodeACState.ValueT == ACStateEnum.SMRunning);
                    if (runningNode != null && runningNode.ComponentPWNode.ValueT.ConnectionState == ACObjectConnectionState.ValuesReceived)
                    {
                        //ParentBSOWCS?.ApplicationQueue.Add(() => HandlePWNodeACState(runningNode.ComponentPWNodeACState, runningNode.ComponentPWNodeACState.ValueT));
                        HandlePWNodeACState(runningNode.ComponentPWNodeACState, runningNode.ComponentPWNodeACState.ValueT);
                    }
                }
                catch (Exception e)
                {
                    string message = null;
                    if (e.InnerException != null)
                        message = string.Format("ManualWeighingModel(LoadWFNode): {0}, {1} {2} {3}", e.Message, e.InnerException.Message, System.Environment.NewLine, e.StackTrace);
                    else
                        message = string.Format("ManualWeighingModel(LoadWFNode): {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace);

                    Messages.Error(this, message, true);
                    using (ACMonitor.Lock(_70600_CurrentOrderInfoValLock))
                    {
                        _CurrentOrderInfoValue = null;
                    }
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
            using (ACMonitor.Lock(_70500_ComponentPWNodeLock))
            {
                if (ComponentPWNode != null)
                {
                    Messages.LogError(this.GetACUrl(), "ActivateWFNode(10)", "Returned");
                    return;
                }

                ComponentPWNode = pwNode;
            }

            var weighingCompInfo = pwNode.ValueT.GetPropertyNet(nameof(PWManualWeighing.CurrentWeighingComponentInfo));
            if (weighingCompInfo == null)
            {
                //Error50291: Initialization error: The reference to the property {1} in Workflownode {0} is null.
                // Initialisierungsfehler: Die Referenz zur Eigenschaft {1} von Workflowknoten {0} ist null.
                Messages.Error(this, "Error50291", false, pwNode?.ACUrl, "CurrentWeighingComponentInfo");
                return;
            }

            if (!IsBinChangeAvailable)
            {
                bool? isBinChangeAvailable = pwNode.ValueT.ExecuteMethod(nameof(PWManualWeighing.IsBinChangeLoopNodeAvailable)) as bool?;
                IsBinChangeAvailable = isBinChangeAvailable ?? false;
            }

            var temp = DatabaseApp.ProdOrderPartslistPos.FirstOrDefault();

            LoadPWConfiguration(pwNode?.ValueT);

            try
            {
                Messages.LogInfo("ManualWeighingModel", "SetupModel", "GetMaterials start.");

                WeighingMaterialList = GetWeighingMaterials(pwNode.ValueT, DatabaseApp, DefaultMaterialIcon);

                Messages.LogInfo("ManualWeighingModel", "SetupModel", "After GetWeighingMaterials. Count: " + WeighingMaterialList?.Count());
            }
            catch (Exception e)
            {
                string message;
                if (e.InnerException != null)
                    message = string.Format("ManualWeighingModel(Setup model): {0}, {1} {2} {3}", e.Message, e.InnerException.Message, System.Environment.NewLine, e.StackTrace);
                else
                    message = string.Format("ManualWeighingModel(Setup model): {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace);
                Messages.Error(this, message, true);
            }

            _NextTaskInfoProperty = pwNode.ValueT.GetPropertyNet(nameof(PWManualWeighing.ManualWeighingNextTask)) as IACContainerTNet<ManualWeighingTaskInfo>;
            if (_NextTaskInfoProperty != null)
            {
                NextTaskInfo = _NextTaskInfoProperty.ValueT;
                _NextTaskInfoProperty.PropertyChanged += NextTaskInfoProperty_PropertyChanged;
            }

            _WeighingComponentInfo = weighingCompInfo as IACContainerTNet<WeighingComponentInfo>;
            if (_WeighingComponentInfo != null)
            {
                WeighingComponentInfo tempCompInfo = _WeighingComponentInfo.ValueT;
                //ParentBSOWCS?.ApplicationQueue.Add(() => HandleWeighingComponentInfo(tempCompInfo));
                HandleWeighingComponentInfo(tempCompInfo);
                (_WeighingComponentInfo as IACPropertyNetBase).PropertyChanged += WeighingComponentInfoPropChanged;
            }
        }

        private void NextTaskInfoProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                IACContainerTNet<ManualWeighingTaskInfo> nextTaskInfo = sender as IACContainerTNet<ManualWeighingTaskInfo>;
                if (nextTaskInfo != null)
                {
                    NextTaskInfo = nextTaskInfo.ValueT;
                }
            }
        }

        public List<WeighingMaterial> GetWeighingMaterials(IACComponentPWNode pwNode, vd.DatabaseApp db, ACClassDesign iconDesign = null)
        {
            if (pwNode == null)
                return null;

            ACValue acValue = pwNode.ACUrlCommand(nameof(PWManualWeighing.WeighingComponentsInfo) +"\\"+ Const.ValueT) as ACValue;
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

        public void LoadPWConfiguration(IACComponentPWNode pwNode)
        {
            ACMethod acMethod = pwNode?.ACUrlCommand(nameof(PWManualWeighing.MyConfiguration)) as ACMethod;
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

            OnLoadPWConfiguration(acMethod);

            //TODO: lot order
        }

        public virtual void OnLoadPWConfiguration(ACMethod acMethod)
        {

        }

        #endregion

        #region Methods => Deactivation/Unload

        public void DeactivateManualWeighingModel()
        {
            UnloadWFNode();
            DeactivateScale();
            UnloadPAFManualWeighing();

            using (ACMonitor.Lock(_70600_CurrentOrderInfoValLock))
            {
                if (_OrderInfo != null)
                {
                    (_OrderInfo as IACPropertyNetBase).PropertyChanged -= OrderInfoPropertyChanged;
                    _OrderInfo = null;
                }

                _CurrentOrderInfoValue = null;
            }

            ACRef<IACComponent>[] scales = null;
            using(ACMonitor.Lock(_70750_ProcessModuleScalesLock))
            {
                scales = _ProcessModuleScales;
                _ProcessModuleScales = null;
            }

            if (scales != null && scales.Any())
            {
                foreach (var scaleRef in scales)
                    scaleRef.Detach();
            }
            scales = null;

            BtnAckBlink = false;
            BtnWeighBlink = false;
            NextTaskInfo = ManualWeighingTaskInfo.None;

            _PAFManuallyAddedQuantity = null;
            _TareScaleState = null;
            _CallPWLotChange = false;
            _ScaleObjectsList = null;
        }

        private void DeactivateWFNode(bool reset = false)
        {
            DelegateToMainThread((object state) =>
            {
                WeighingMaterialList = null;
                SelectedWeighingMaterial = null;
            });
            
            
            IsBinChangeAvailable = false;

            if (_NextTaskInfoProperty != null)
            {
                _NextTaskInfoProperty.PropertyChanged -= NextTaskInfoProperty_PropertyChanged;
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

            using (ACMonitor.Lock(_70500_ComponentPWNodeLock))
            {
                ComponentPWNode = null;
            }
        }

        public virtual void UnloadWFNode()
        {
            IsBinChangeAvailable = false;

            DeactivateWFNode();

            using (ACMonitor.Lock(_70500_ComponentPWNodeLock))
            {
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
        }

        private void UnloadPAFManualWeighing()
        {
            using (ACMonitor.Lock(_70700_PrivateMemberLock))
            {
                if (_PAFCurrentACMethod != null)
                {
                    (_PAFCurrentACMethod as IACPropertyNetBase).PropertyChanged -= PAFCurrentACMethodPropChanged;
                    _PAFCurrentACMethod = null;
                }
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
            if (_ScaleActualValue != null)
            {
                (_ScaleActualValue as IACPropertyNetBase).PropertyChanged -= ScaleActualValue_PropertyChanged;
                _ScaleActualValue = null;
            }

        }

        #endregion

        #region Methods => PropertyChanged

        private void OrderInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                ACComponent currentProcessModule = CurrentProcessModule;
                IACContainerTNet<string> senderProp = sender as IACContainerTNet<string>;
                string orderInfo = null;
                if (senderProp != null)
                    orderInfo = senderProp.ValueT;

                ParentBSOWCS?.ApplicationQueue.Add(() => LoadWFNode(currentProcessModule, orderInfo));
            }
        }

        private void PAFCurrentACMethodPropChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                ACMethod acMethod = null;
                using(ACMonitor.Lock(_70700_PrivateMemberLock))
                {
                    acMethod = _PAFCurrentACMethod?.ValueT;
                }

                ACMethod cloned = acMethod?.Clone() as ACMethod;
                ParentBSOWCS?.ApplicationQueue.Add(() => HandlePAFCurrentACMethod(cloned));
            }
        }

        private void PWNodeACStatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                IACContainerTNet<ACStateEnum> senderProp = sender as IACContainerTNet<ACStateEnum>;
                if (senderProp == null)
                    return;

                ACStateEnum tempState = senderProp.ValueT;
                ParentBSOWCS?.ApplicationQueue.Add(() => HandlePWNodeACState(senderProp, tempState));
            }
        }

        private void ActWeightProp_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT && _ScaleActualWeight != null)
            {
                double tempValue = _ScaleActualWeight.ValueT;
                ParentBSOWCS?.ApplicationQueue.Add(() => ScaleRealWeight = tempValue);
            }
        }

        private void WeighingComponentInfoPropChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                IACContainerTNet<WeighingComponentInfo> weighingComponentInfo = sender as IACContainerTNet<WeighingComponentInfo>;
                if (weighingComponentInfo != null && weighingComponentInfo.ValueT != null)
                {
                    var temp = weighingComponentInfo.ValueT.Clone() as WeighingComponentInfo;
                    ParentBSOWCS?.ApplicationQueue.Add(() => HandleWeighingComponentInfo(temp));
                }
            }
        }

        private void PAFManuallyAddedQuantityPropChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT && _PAFManuallyAddedQuantity != null)
            {
                double tempValue = _PAFManuallyAddedQuantity.ValueT;
                ParentBSOWCS?.ApplicationQueue.Add(() => ScaleAddAcutalWeight = tempValue);
            }
        }

        private void OnTargetWeightChanged(double targetWeight)
        {
            var facilityCharges = FacilityChargeList;
            if (facilityCharges == null || !facilityCharges.Any())
                return;

            foreach (var fc in facilityCharges)
            {
                fc.OnTargetQunatityChanged(targetWeight);
            }
        }

        #endregion

        #region Methods => HandlePropertyChanged

        private void HandlePWNodeACState(IACContainerTNet<ACStateEnum> senderProp, ACStateEnum acState)
        {
            ManualWeighingPWNode pwNode = null;
            bool deactivateFirst = false;

            using (ACMonitor.Lock(_70500_ComponentPWNodeLock))
            {
                if (ComponentPWNodesList == null)
                {
                    Messages.LogError(this.GetACUrl(), "HandlePWNodeACState(10)", "ComponentPWNodesList is null!");
                    return;
                }

                pwNode = ComponentPWNodesList.FirstOrDefault(c => c.ComponentPWNodeACState == senderProp);
                if (pwNode == null)
                {
                    Messages.LogError(this.GetACUrl(), "HandlePWNodeACState(20)", "pwNode is null!");
                    return;
                }

                if (ComponentPWNode != null)
                {
                    if (pwNode.ComponentPWNode.ValueT.ACUrl != CurrentComponentPWNode.ACUrl)
                    {
                        if (acState == ACStateEnum.SMRunning)
                        {
                            deactivateFirst = true;
                        }
                        else
                        {
                            Messages.LogError(this.GetACUrl(), "HandlePWNodeACState(30)", "ComponentPWNode is different than sender!");
                            return;
                        }
                    }
                }
            }

            try
            {
                if (deactivateFirst)
                    DeactivateWFNode();

                if (acState == ACStateEnum.SMRunning)
                    ActivateWFNode(pwNode.ComponentPWNode);
                else if (acState == ACStateEnum.SMCompleted)
                    DeactivateWFNode();
                else if (acState == ACStateEnum.SMResetting)
                    DeactivateWFNode(true);
            }
            catch (Exception e)
            {
                string message;
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
                //Messages.LogError(this.GetACUrl(), "HandlePAFCurrentACMethod(10)", "PAFCurrentMaterial is setted: " + PAFCurrentMaterial);
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
                            WeighingMaterial comp = WeighingMaterialList.FirstOrDefault(c => c.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation);
                            if (comp == null)
                                return;

                                DelegateToMainThread((object state) =>
                                {
                                    if (SelectedWeighingMaterial != comp)
                                        SelectedWeighingMaterial = comp;

                                    comp.ChangeComponentState((WeighingComponentState)compInfo.WeighingComponentState, DatabaseApp);
                                    SelectActiveScaleObject(comp);
                                    if (comp.WeighingMatState == WeighingComponentState.InWeighing)
                                        BtnWeighBlink = false;

                                    if (compInfo.FacilityCharge != null)
                                    {
                                        var fcItem = FacilityChargeList?.FirstOrDefault(c => c.FacilityChargeID == compInfo.FacilityCharge);
                                        SelectedFacilityCharge = fcItem;
                                    }
                                });
                            break;
                        }
                    case WeighingComponentInfoType.SelectCompReturnFC_F:
                        {
                            WeighingMaterial comp = WeighingMaterialList.FirstOrDefault(c => c.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation);
                            DelegateToMainThread((object state) =>
                            {
                                if (SelectedWeighingMaterial != comp)
                                {
                                    SelectedWeighingMaterial = comp;
                                    SelectedWeighingMaterial.ChangeComponentState(WeighingComponentState.Selected, DatabaseApp);
                                    _StartWeighingFromF_FC = true;
                                }
                            });
                            break;
                        }
                    case WeighingComponentInfoType.StateSelectFC_F:
                    case WeighingComponentInfoType.SelectFC_F:
                        {
                            WeighingMaterial comp = WeighingMaterialList.FirstOrDefault(c => c.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation);

                            DelegateToMainThread((object state) =>
                            {
                                if (/*compInfoType == WeighingComponentInfoType.StateSelectFC_F && */(WeighingComponentState)compInfo.WeighingComponentState == WeighingComponentState.InWeighing
                                && SelectedWeighingMaterial == null)
                                {
                                    if (comp != null)
                                        SelectedWeighingMaterial = comp;
                                }

                                if (SelectedWeighingMaterial != null && SelectedWeighingMaterial.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation)
                                {
                                    if (SelectedWeighingMaterial.WeighingMatState != (WeighingComponentState)compInfo.WeighingComponentState)
                                    {
                                        SelectedWeighingMaterial.ChangeComponentState((WeighingComponentState)compInfo.WeighingComponentState, DatabaseApp);
                                    }

                                    if (compInfoType == WeighingComponentInfoType.StateSelectFC_F)
                                    {
                                        SelectActiveScaleObject(SelectedWeighingMaterial);
                                    }
                                    else
                                    {
                                        OnPropertyChanged("CurrentScaleObject");
                                    }    
                                }

                                if (compInfo.FacilityCharge != null)
                                {
                                    bool autoRefresh = compInfo.FC_FAutoRefresh;
                                    if (SelectedFacilityCharge == null || _CallPWLotChange || compInfo.IsLotChange)
                                    {
                                        if (autoRefresh)
                                        {
                                            SelectedFacilityCharge = null; //check if this needed
                                            _FacilityChargeList = null;
                                            OnPropertyChanged("FacilityChargeList");
                                        }

                                        var fcItem = FacilityChargeList?.FirstOrDefault(c => c.FacilityChargeID == compInfo.FacilityCharge);
                                        SelectedFacilityCharge = fcItem;
                                    }
                                }
                            });
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

        public override void OnHandleWFNodes(List<ACChildInstanceInfo> connectionList)
        {
            if (connectionList == null)
            {
                BtnAckBlink = false;
            }
            else
            {
                bool blink = MessagesListSafe.Any(c => c.HandleByAcknowledgeButton && !c.IsAlarmMessage);
                if (BtnAckBlink != blink)
                    BtnAckBlink = blink;
            }

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
                //_FacilityList = null;
                OnPropertyChanged("FacilityChargeList");
            }
        }

        public bool IsEnabledRefreshMaterialOrFC_F()
        {
            return SelectedFacilityCharge == null || _CallPWLotChange;
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
            List<ACValueItem> scaleObjectsList = null;
            using (ACMonitor.Lock(_20015_LockValue))
            {
                scaleObjectsList = ScaleObjectsList;
            }

            if (scaleObjectsList != null && weighingMaterial.WeighingMatState == WeighingComponentState.InWeighing)
            {
                SelectActiveScaleObject(scaleObjectsList);
            }
            else
                OnPropertyChanged("CurrentScaleObject");
        }

        private void SelectActiveScaleObject(List<ACValueItem> scaleObjects, bool setIfNotSelected = false)
        {
            if (scaleObjects == null)
                return;

            if (scaleObjects.Count > 1)
            {
                string activeScaleACUrl = CurrentPAFManualWeighing?.ExecuteMethod(nameof(PAFManualWeighing.GetActiveScaleObjectACUrl)) as string;
                if (!string.IsNullOrEmpty(activeScaleACUrl))
                {
                    DelegateToMainThread((object state) =>
                    {
                        CurrentScaleObject = scaleObjects.FirstOrDefault(c => c.Value as string == activeScaleACUrl);
                    });
                }
                else if (setIfNotSelected)
                {
                    DelegateToMainThread((object state) =>
                    {
                        CurrentScaleObject = scaleObjects.FirstOrDefault();
                    });
                }
            }
            else
            {
                DelegateToMainThread((object state) =>
                {
                    CurrentScaleObject = scaleObjects.FirstOrDefault();
                });
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

        [ACMethodInteraction("", "en{'Settings'}de{'Einstellungen'}", 690, true)]
        public void OpenSettings()
        {
            ACComponent currentProcessModule = CurrentProcessModule;
            if (currentProcessModule == null)
            {
                //Error50283: The manual weighing module can not be initialized. The property CurrentProcessModule is null.
                // Die Handverwiegungsstation konnte nicht initialisiert werden. Die Eigenschaft CurrentProcessModule ist null.
                Messages.Error(this, "Error50283");
                return;
            }

            using (vd.DatabaseApp dbApp = new vd.DatabaseApp())
            {
                var lastUsedLotConfigs = dbApp.MaterialConfig.Where(c => c.KeyACUrl == PWManualWeighing.MaterialConfigLastUsedLotKeyACUrl 
                                                                      && c.VBiACClassID == currentProcessModule.ComponentClass.ACClassID);

                List<vd.FacilityCharge> lastUsedLots = new List<vd.FacilityCharge>();

                foreach (vd.MaterialConfig lotConfig in lastUsedLotConfigs)
                {
                    Guid? fcID = lotConfig.Value as Guid?;
                    if (fcID.HasValue)
                    {
                        vd.FacilityCharge fc = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == fcID);
                        if (fc != null)
                        {
                            lastUsedLots.Add(fc);
                        }
                    }
                }

                LastUsedLotList = lastUsedLots.OrderBy(c => c.Material.MaterialNo).ToList();

                ShowDialog(this, "SettingsDialog");
            }
        }

        public bool IsEnabledOpenSettings()
        {
            return !IsCurrentProcessModuleNull;
        }

        [ACMethodInfo("", "en{'Remove last used lot suggestion'}de{'Zuletzt verwendeten Chargenvorschlag entfernen'}", 691)]
        public void RemoveLastUsedLot()
        {
            ACComponent currentProcessModule = CurrentProcessModule;
            if (currentProcessModule == null)
            {
                //Error50283: The manual weighing module can not be initialized. The property CurrentProcessModule is null.
                // Die Handverwiegungsstation konnte nicht initialisiert werden. Die Eigenschaft CurrentProcessModule ist null.
                Messages.Error(this, "Error50283");
                return;
            }

            var lastUsedLotConfig = SelectedLastUsedLot.Material.MaterialConfig_Material.FirstOrDefault(c => c.KeyACUrl == PWManualWeighing.MaterialConfigLastUsedLotKeyACUrl
                                                                                                 && c.VBiACClassID == currentProcessModule.ComponentClass.ACClassID);

            if (lastUsedLotConfig == null)
                return;

            vd.DatabaseApp dbApp = lastUsedLotConfig.GetObjectContext<vd.DatabaseApp>();
            if (dbApp != null)
                dbApp.DeleteObject(lastUsedLotConfig);

            Msg msg = dbApp.ACSaveChanges();
            if (msg != null)
            {
                Root.Messages.Msg(msg);
                return;
            }

            LastUsedLotList.Remove(SelectedLastUsedLot);
            OnPropertyChanged("LastUsedLotList");
        }

        public bool IsEnabledRemoveLastUsedLot()
        {
            return SelectedLastUsedLot != null;
        }

        public ACMethod GetPAFCurrentACMethod()
        {
            ACMethod result = null;

            using(ACMonitor.Lock(_70700_PrivateMemberLock))
            {
                result = _PAFCurrentACMethod?.ValueT?.Clone() as ACMethod;
            }

            return result;
        }

        #endregion

        #region Methods => SingleDosing

        [ACMethodInfo("", "en{'Single dosing'}de{'Einzeldosierung'}", 660)]
        public virtual void ShowSingleDosingDialog()
        {
            ACComponent currentProcessModule = CurrentProcessModule;
            if (currentProcessModule == null)
            {
                //Error50283: The manual weighing module can not be initialized. The property CurrentProcessModule is null.
                // Die Handverwiegungsstation konnte nicht initialisiert werden. Die Eigenschaft CurrentProcessModule ist null.
                Messages.Error(this, "Error50283");
                return;
            }

            if (_RoutingService == null)
            {
                _RoutingService = ACRoutingService.ACRefToServiceInstance(this);
                if (_RoutingService == null)
                {
                    //Error50430: The routing service is unavailable.
                    Messages.Error(this, "Error50430");
                    return; 
                }
            }

            if (!IsRoutingServiceAvailable)
            {
                //Error50430: The routing service is unavailable.
                Messages.Error(this, "Error50430");
                return;
            }

            RoutingResult rResult = ACRoutingService.FindSuccessors(RoutingService, DatabaseApp.ContextIPlus, true, currentProcessModule.ComponentClass, PAMParkingspace.SelRuleID_ParkingSpace, 
                                                                    RouteDirections.Forwards, null, null, null, 0, true, true);

            if (rResult == null || rResult.Routes == null)
            {
                //Error50431: Can not find any target storage for this station.
                Messages.Error(this, "Error50431");
                return;
            }

            SingleDosTargetStorageList = rResult.Routes.SelectMany(c => c.GetRouteTargets()).Select(x => x.Target);


            if (_ACFacilityManager == null)
            {
                _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
                if (_ACFacilityManager == null)
                {
                    //Error50432: The facility manager is null.
                    Messages.Error(this, "Error50432");
                    return;
                }
            }

            _ACPickingManager = ACRefToPickingManager();

            var result = currentProcessModule.ExecuteMethod(nameof(PAProcessModuleVB.GetDosableComponents), false) as SingleDosingItems;
            if (result == null)
            {
                //Error50433: Can not get dosable components for single dosing.
                Messages.Error(this, "Error50433");
                return;
            }

            if (result.Error != null)
            {
                Messages.Msg(result.Error);
                return;
            }

            ClearBookingData();

            result = OnFilterSingleDosingItems(result);

            result.ForEach(c => c.MaterialIconDesign = DefaultMaterialIcon);

            SingleDosingItemList = result;
            ShowDialog(this, "SingleDosingDialog");
        }

        public bool IsEnabledShowSingleDosingDialog()
        {
            return !IsCurrentProcessModuleNull;
        }

        public virtual SingleDosingItems OnFilterSingleDosingItems(IEnumerable<SingleDosingItem> items)
        {
            return new SingleDosingItems(items.OrderBy(c => c.MaterialName != null ? c.MaterialName : ""));
        }

        [ACMethodInfo("", "en{'Single dosing'}de{'Einzeldosierung'}", 661)]
        public virtual void SingleDosingStart()
        {
            if (SingleDosTargetStorageList == null || !SingleDosTargetStorageList.Any())
            {
                //Error50431: Can not find any target storage for this station.
                Messages.Error(this, "Error50431");
                return;
            }

            ACComponent currentProcessModule = CurrentProcessModule;
            if (currentProcessModule == null)
            {
                //Error50283: The manual weighing module can not be initialized. The property CurrentProcessModule is null.
                // Die Handverwiegungsstation konnte nicht initialisiert werden. Die Eigenschaft CurrentProcessModule ist null.
                Messages.Error(this, "Error50283");
                return;
            }

            if (SingleDosTargetStorageList.Count() > 1 && SelectedSingleDosTargetStorage == null)
            {
                //todo Select dosing target
            }
            else
            {
                SelectedSingleDosTargetStorage = SingleDosTargetStorageList.FirstOrDefault();
            }

            vd.Facility inwardFacility = DatabaseApp.Facility.FirstOrDefault(c => c.VBiFacilityACClassID == SelectedSingleDosTargetStorage.ACClassID);
            
            if (inwardFacility == null)
            {
                //Error50434: Can not find any facility according target storage ID: {0}
                Messages.Error(this, "Error50434", false, SelectedSingleDosTargetStorage.ACClassID);
                return;
            }

            //vd.Facility outwardFacility = DatabaseApp.Facility.FirstOrDefault(c => c.FacilityNo == SelectedSingleDosingItem.FacilityNo);

            //if (outwardFacility == null)
            //{
            //    //Error50435: Can not find any outward facility with FacilityNo: {0}
            //    Messages.Error(this, "Error50435",false, SelectedSingleDosingItem.FacilityNo);
            //    return;
            //}

            vd.Material material = DatabaseApp.Material.FirstOrDefault(c => c.MaterialNo == SelectedSingleDosingItem.MaterialNo);
            if (material == null)
            {
                //Error50436: The material with MaterialNo: {0} can not be found in database.
                Messages.Error(this, "Error50436", false, SelectedSingleDosingItem.MaterialNo);
                return;
            }

            MsgWithDetails msg = ValidateSingleDosingStart(currentProcessModule);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            var wfConfigs = material.MaterialConfig_Material.Where(c => c.KeyACUrl == vd.MaterialConfig.PWMethodNodeConfigKeyACUrl);

            if (wfConfigs == null || !wfConfigs.Any())
            {
                //Error50437: The single dosing workflow is not assigned to the material. Please assign single dosing workflow for this material in bussiness module Material. 
                Messages.Error(this, "Error50437");
                return;
            }

            var wfConfig = wfConfigs.FirstOrDefault(c => c.VBiACClassID == currentProcessModule.ComponentClass.ACClassID);
            if (wfConfig == null)
            {
                wfConfig = wfConfigs.FirstOrDefault(c => !c.VBiACClassID.HasValue);
            }

            if (wfConfig == null)
            {
                //Error50438: The single dosing workflow is not assigned for this station. Please assign single dosing workflow for this station. 
                Messages.Error(this, "Error50438");
                return;
            }

            var workflow = wfConfig.VBiACClassWF.FromIPlusContext<ACClassWF>(DatabaseApp.ContextIPlus);
            var acClassMethod = workflow.ACClassMethod;

            CurrentBookParamRelocation.InwardFacility = inwardFacility;
            //CurrentBookParamRelocation.OutwardFacility = outwardFacility;
            CurrentBookParamRelocation.OutwardMaterial = material;
            CurrentBookParamRelocation.InwardQuantity = SingleDosTargetQuantity;
            CurrentBookParamRelocation.OutwardQuantity = SingleDosTargetQuantity;

            ACComponent processModule = CurrentProcessModule;

            RunWorkflow(workflow, acClassMethod, processModule, false);

            SingleDosTargetQuantity = 0;
            SelectedSingleDosTargetStorage = null;

            CloseTopDialog();
        }

        public bool IsEnabledSingleDosingStart()
        {
            return SelectedSingleDosingItem != null && SingleDosTargetQuantity > 0.0001;
        }

        public virtual MsgWithDetails ValidateSingleDosingStart(ACComponent currentProcessModule)
        {
            if (currentProcessModule != null)
            {
                ACClass componentClass = currentProcessModule.ComponentClass?.FromIPlusContext<ACClass>(DatabaseApp.ContextIPlus);
                if (componentClass == null)
                    return null;

                double maxWeight = 0;
                ACClassProperty maxWeightProp = componentClass.GetProperty(PAProcessModule.PropNameMaxWeightCapacity);
                if (maxWeightProp != null && maxWeightProp.Value != null && maxWeightProp.Value is string)
                    maxWeight = (double)ACConvert.ChangeType(maxWeightProp.Value as string, typeof(double), true, DatabaseApp.ContextIPlus);

                maxWeight = maxWeight - (maxWeight * 0.2);

                if (SingleDosTargetQuantity > maxWeight)
                {
                    //Error50487:The dosing quantity is {0} kg but the maximum dosing qunatity is {1} kg.
                    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "ValidateStart", 2469, "Error50487", SingleDosTargetQuantity, Math.Round(maxWeight,2));
                    return new MsgWithDetails(new Msg[]{ msg });
                }
            }

            return null;
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
                //case "SelectedWeighingMaterial":
                //    {
                //        if (WeighingMaterialsFSM)
                //            return Global.ControlModes.Enabled;
                //        return Global.ControlModes.Disabled;
                //    }
                case nameof(SelectedFacilityCharge):
                    {
                        if (ShowSelectFacilityLotInfo)
                            return Global.ControlModes.Enabled;
                        return Global.ControlModes.Disabled;
                    }
                case nameof(EnterLotManually):
                    {
                        if (EnterLotManually)
                            return Global.ControlModes.Enabled;
                        return Global.ControlModes.Collapsed;
                    }
                case nameof(CurrentScaleObject):
                    {
                        if (SelectedWeighingMaterial != null && SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.InWeighing)
                            return Global.ControlModes.Disabled;
                        return Global.ControlModes.Enabled;
                    }
            }

            return result;
        }

        #region Methods => AbortDialog

        [ACMethodInfo("", "en{'Yes -> Abort'}de{'Ja -> Abbrechen'}", 695)]
        public void AbortComponent()
        {
            _AbortMode = AbortModeEnum.AbortComponent;
            InInterdischargingQ = null;
            _IsEnabledCompleteInterdischarging = false;
            CloseTopDialog();
        }

        [ACMethodInfo("", "en{'Abort and emptying mode'}de{'Abbrechen und Leerfahren'}", 695)]
        public void AbortComponentEmptyingMode()
        {
            _AbortMode = AbortModeEnum.AbortComponentSwitchToEmptyingMode;
            InInterdischargingQ = null;
            _IsEnabledCompleteInterdischarging = false;
            CloseTopDialog();
        }

        [ACMethodInfo("", "en{'Switch to emptying mode'}de{'Leerfahrmodus aktivieren'}", 695)]
        public void SwitchEmptyingMode()
        {
            _AbortMode = AbortModeEnum.SwitchToEmptyingMode;
            CloseTopDialog();
        }

        [ACMethodInfo("", "en{'Close'}de{'Schließen'}", 696)]
        public void CloseAbortDialog()
        {
            CloseTopDialog();
        }

        public bool IsEnabledCloseAbortDialog()
        {
            return InInterdischargingQ.HasValue ? false : true;
        }

        [ACMethodInfo("", "en{'Interdischarge'}de{'Interdischarge'}", 696)]
        public void Interdischarge()
        {
            IACComponentPWNode currentPWNode = ComponentPWNodeLocked;
            InterdischargeStart(currentPWNode, false);
        }

        private void InterdischargeStart(IACComponentPWNode currentPWNode, bool onlyCheck)
        {
            _AbortMode = AbortModeEnum.Interdischarging;

            if (currentPWNode != null)
            {
                double? storedActualValue = null;
                if (onlyCheck)
                {
                    double parsedValue = 0;
                    string storedValue = currentPWNode.ACUrlCommand(nameof(PWManualWeighing.InterdischargingScaleActualValue)) as string;
                    if (!string.IsNullOrEmpty(storedValue) && double.TryParse(storedValue, out parsedValue))
                    {
                        storedActualValue = parsedValue;
                    }
                }
                else
                {
                    storedActualValue = currentPWNode.ExecuteMethod(nameof(PWManualWeighing.InterdischargingStart)) as double?;
                }

                if (!storedActualValue.HasValue)
                {
                    //Error
                    return;
                }

                InInterdischargingQ = storedActualValue;

                _IsEnabledCompleteInterdischarging = true;
                if (_ScaleActualValue != null)
                {
                    //(_ScaleActualValue as IACPropertyNetBase).PropertyChanged -= ScaleActualValue_PropertyChanged;
                    //(_ScaleActualValue as IACPropertyNetBase).PropertyChanged += ScaleActualValue_PropertyChanged;

                    _IsEnabledCompleteInterdischarging = false;

                    VerifyIsActualQLower(_ScaleActualValue.ValueT);
                }
            }
        }

        private void ScaleActualValue_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                IACContainerTNet<double> propVal = sender as IACContainerTNet<double>;
                if (propVal != null)
                {
                    ScaleGrossWeight = propVal.ValueT;
                    if (InInterdischargingQ.HasValue)
                    {
                        VerifyIsActualQLower(propVal.ValueT);
                    }
                }
            }
        }

        private void VerifyIsActualQLower(double actualValue)
        {
            if (actualValue <= InterdischargingCompleteQ)
            {
                _IsEnabledCompleteInterdischarging = true;
            }
        }

        public bool IsEnabledInterdischarge()
        {
            return InInterdischargingQ.HasValue ? false : true;
        }

        [ACMethodInfo("", "en{'Interdischarging complete'}de{'Interdischarging complete'}", 696)]
        public void CompleteInterdischarging()
        {
            IACComponentPWNode currentPWNode = CurrentComponentPWNode;

            if (currentPWNode != null)
            {
                currentPWNode.ExecuteMethod(nameof(PWManualWeighing.CompleteInterdischarging));
            }

            InInterdischargingQ = null;
            _IsEnabledCompleteInterdischarging = false;
            CloseAbortDialog();
        }

        public bool IsEnabledCompleteInterdischarging()
        {
            return _IsEnabledCompleteInterdischarging;
        }

        #endregion

        #endregion

        #region Handle execute helper

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Weigh):
                    Weigh();
                    return true;
                case nameof(IsEnabledWeigh):
                    result = IsEnabledWeigh();
                    return true;
                case nameof(Acknowledge):
                    Acknowledge();
                    return true;
                case nameof(IsEnabledAcknowledge):
                    result = IsEnabledAcknowledge();
                    return true;
                case nameof(Tare):
                    Tare();
                    return true;
                case nameof(IsEnabledTare):
                    result = IsEnabledTare();
                    return true;
                case nameof(LotChange):
                    LotChange();
                    return true;
                case nameof(IsEnabledLotChange):
                    result = IsEnabledLotChange();
                    return true;
                case nameof(BinChange):
                    BinChange();
                    return true;
                case nameof(IsEnabledBinChange):
                    result = IsEnabledBinChange();
                    return true;
                case nameof(Abort):
                    Abort();
                    return true;
                case nameof(IsEnabledAbort):
                    result = IsEnabledAbort();
                    return true;
                case nameof(ApplyLot):
                    ApplyLot();
                    return true;
                case nameof(IsEnabledApplyLot):
                    result = IsEnabledApplyLot();
                    return true;
                case nameof(AddKg):
                    AddKg();
                    return true;
                case nameof(IsEnabledAddKg):
                    result = IsEnabledAddKg();
                    return true;
                case nameof(RemoveKg):
                    RemoveKg();
                    return true;
                case nameof(IsEnabledRemoveKg):
                    result = IsEnabledRemoveKg();
                    return true;
                case nameof(RefreshMaterialOrFC_F):
                    RefreshMaterialOrFC_F();
                    return true;
                case nameof(IsEnabledRefreshMaterialOrFC_F):
                    result = IsEnabledRefreshMaterialOrFC_F();
                    return true;

                case nameof(OpenSettings):
                    OpenSettings();
                    return true;

                case nameof(IsEnabledOpenSettings):
                    result = IsEnabledOpenSettings();
                    return true;

                case nameof(RemoveLastUsedLot):
                    RemoveLastUsedLot();
                    return true;

                case nameof(IsEnabledRemoveLastUsedLot):
                    result = IsEnabledRemoveLastUsedLot();
                    return true;

                case nameof(ShowSingleDosingDialog):
                    ShowSingleDosingDialog();
                    return true;

                case nameof(IsEnabledShowSingleDosingDialog):
                    result = IsEnabledShowSingleDosingDialog();
                    return true;

                case nameof(SingleDosingStart):
                    SingleDosingStart();
                    return true;

                case nameof(IsEnabledSingleDosingStart):
                    result = IsEnabledSingleDosingStart();
                    return true;

                case nameof(AbortComponent):
                    AbortComponent();
                    return true;
                case nameof(AbortComponentEmptyingMode):
                    AbortComponentEmptyingMode();
                    return true;
                case nameof(SwitchEmptyingMode):
                    SwitchEmptyingMode();
                    return true;
                case nameof(CloseAbortDialog):
                    CloseAbortDialog();
                    return true;
                case nameof(IsEnabledCloseAbortDialog):
                    result = IsEnabledCloseAbortDialog();
                    return true;
                case nameof(Interdischarge):
                    Interdischarge();
                    return true;
                case nameof(IsEnabledInterdischarge):
                    result = IsEnabledInterdischarge();
                    return true;
                case nameof(CompleteInterdischarging):
                    CompleteInterdischarging();
                    return true;
                case nameof(IsEnabledCompleteInterdischarging):
                    result = IsEnabledCompleteInterdischarging();
                    return true;

            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }

    #region Enum

    public enum ScaleBackgroundState : short
    {
        Weighing = 0,
        InTolerance = 10,
        OutTolerance = 20
    }

    #endregion
}
