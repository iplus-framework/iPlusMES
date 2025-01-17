// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using VD = gip.mes.datamodel;
using gip.mes.processapplication;
using System.ComponentModel;
using System.Data;
using gip.core.processapplication;
using System.Threading;
using gip.mes.facility;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Manual weighing'}de{'Handverwiegung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, SortIndex = 100)]
    public class BSOManualWeighing : BSOWorkCenterMessages
    {
        #region c'tors

        public BSOManualWeighing(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _InformUserWithMsgNegQuantStock = new ACPropertyConfigValue<bool>(this, nameof(InformUserWithMsgNegQuantStock), false);
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
            ReworkInfoItems = null;
            SelectedReworkInfo = null;

            return base.ACDeInit(deleteACClassTask);
        }

        public const string ClassName = nameof(BSOManualWeighing);

        #endregion

        #region Properties

        #region Database

        private VD.DatabaseApp _DatabaseApp;
        public override VD.DatabaseApp DatabaseApp
        {
            get
            {
                if (_DatabaseApp == null)
                    _DatabaseApp = ACObjectContextManager.GetOrCreateContext<VD.DatabaseApp>(this.GetACUrl());
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
        public override ACComponent CurrentProcessModule
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
                ParentBSOWCS.ApplicationQueue.Add(() => DeactivateManualWeighingModel());
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    _CurrentProcessModule = value;
                }
                OnPropertyChanged();
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
        public ACClassDesign DefaultMaterialIcon
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
        protected IACContainerT<double> _ScaleActualWeight;
        protected IACContainerT<double> _ScaleActualValue;

        protected Type _PAFManualWeighingType = typeof(PAFManualWeighing);
        protected Type _PAFManualAdditionType = typeof(PAFManualAddition);

        #endregion

        #region Properties => ProcessFunction

        private IACContainerT<ACMethod> _PAFCurrentACMethod;

        private ACRef<IACComponent> _CurrentPAFManualWeighing;
        [ACPropertyInfo(602)]
        public IACComponent CurrentPAFManualWeighing
        {
            get => _CurrentPAFManualWeighing?.ValueT;
        }


        protected IACContainerT<double> _PAFManuallyAddedQuantity;

        private IACContainerT<short> _TareScaleState;



        protected IACContainerTNet<string> _ActiveScaleObjectACUrl;

        #endregion

        #region Properties => ScaleObjects

        protected ACRef<IACComponent>[] _ProcessModuleScales;

        public ACValueItem _CurrentScaleObject;
        [ACPropertyCurrent(603, "ScaleObject")]
        public ACValueItem CurrentScaleObject
        {
            get => _CurrentScaleObject;
            set
            {
                _CurrentScaleObject = value;
                OnPropertyChanged();

                if (value != null)
                {
                    DeactivateScale();
                    ACRef<IACComponent> scaleRef = null;
                    ACRef<IACComponent>[] scalesRef = null;
                    using (ACMonitor.Lock(_70750_ProcessModuleScalesLock))
                    {
                        scalesRef = _ProcessModuleScales?.ToArray();
                    }

                    scaleRef = scalesRef?.FirstOrDefault(c => c.ACUrl == _CurrentScaleObject.Value.ToString());

                    if (scaleRef != null)
                    {
                        IACComponent scaleComp = scaleRef.ValueT;
                        ActivateScale(scaleComp);
                    }
                }
                else
                    DeactivateScale();
            }
        }

        private ObservableCollection<ACValueItem> _ScaleObjectsList;
        [ACPropertyList(604, "ScaleObject")]
        public ObservableCollection<ACValueItem> ScaleObjectsList
        {
            get
            {
                return _ScaleObjectsList;
            }
            set
            {
                _ScaleObjectsList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Represents the sum of ScaleAddActualWeight and Scale RealWeight
        /// </summary>
        [ACPropertyInfo(605)]
        public virtual double ScaleActualWeight
        {
            get => ScaleAddActualWeight + ScaleRealWeight;
        }

        protected double _ScaleAddActualWeight;
        /// <summary>
        /// The weight which is manually added from sack or etc.
        /// </summary>
        public virtual double ScaleAddActualWeight
        {
            get => _ScaleAddActualWeight;
            set
            {
                _ScaleAddActualWeight = value;
                ScaleBckgrState = DetermineBackgroundState(_TolerancePlus, _ToleranceMinus, TargetWeight, value + ScaleRealWeight);
                OnPropertyChanged(nameof(ScaleActualWeight));
                OnPropertyChanged(nameof(ScaleDifferenceWeight));
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
                ScaleBckgrState = DetermineBackgroundState(_TolerancePlus, _ToleranceMinus, TargetWeight, value + ScaleAddActualWeight);
                OnPropertyChanged(nameof(ScaleActualWeight));
                OnPropertyChanged(nameof(ScaleDifferenceWeight));
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
                OnPropertyChanged(nameof(TargetWeight));
                OnPropertyChanged(nameof(ScaleDifferenceWeight));
                ScaleBckgrState = DetermineBackgroundState(_TolerancePlus, _ToleranceMinus, _TargetWeight, ScaleActualWeight);
                OnTargetWeightChanged(value);
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

        private string _ScalePrecisionFormat = "F3";
        [ACPropertyInfo(605)]
        public string ScalePrecisionFormat
        {
            get => _ScalePrecisionFormat;
            set
            {
                _ScalePrecisionFormat = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        private IACContainerTNet<double> _NetWeightOfLargestScale = null;
        [ACPropertyInfo(608,"", "en{'Net weight largest scale'}de{'Nettowert größte Waage'}", IsProxyProperty = true)]
        public IACContainerTNet<double> NetWeightOfLargestScale
        {
            get
            {
                return _NetWeightOfLargestScale;
            }
            set 
            {
                _NetWeightOfLargestScale = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        public LotSelectionRuleEnum? MultipleLotsSelectionRule
        {
            get;
            private set;
        }

        protected bool ScaleOtherComponentOnAbort = false;

        public bool _DiffWeighing;
        public bool DiffWeighing
        {
            get => _DiffWeighing;
            set
            {
                _DiffWeighing = value;
                OnPropertyChanged();
            }
        }

        public bool _AutoSelectLot;
        public bool AutoSelectLot
        {
            get => _AutoSelectLot;
            set
            {
                _AutoSelectLot = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Properties => OrderInfo

        //TODO: check if this necessary
        private VD.ProdOrderPartslistPos _EndBatchPos;
        [ACPropertyInfo(613)]
        public VD.ProdOrderPartslistPos EndBatchPos
        {
            get => _EndBatchPos;
            set
            {
                _EndBatchPos = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        private VD.FacilityCharge _SelectedLastUsedLot;
        [ACPropertySelected(690, "LastUsedLot")]
        public VD.FacilityCharge SelectedLastUsedLot
        {
            get => _SelectedLastUsedLot;
            set
            {
                _SelectedLastUsedLot = value;
                OnPropertyChanged();
            }
        }

        private List<VD.FacilityCharge> _LastUsedLotList;
        [ACPropertyList(690, "LastUsedLot")]
        public List<VD.FacilityCharge> LastUsedLotList
        {
            get => _LastUsedLotList;
            set
            {
                _LastUsedLotList = value;
                OnPropertyChanged();
            }
        }

        private double? _InInterdischargingQ = null;

        public double? InInterdischargingQ
        {
            get => _InInterdischargingQ;
            set
            {
                _InInterdischargingQ = value;
                OnPropertyChanged();
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
            AbortComponentScaleOtherComponents = 15,
            AbortComponentSwitchToEmptyingMode = 20,
            SwitchToEmptyingMode = 30,
            Interdischarging = 40,
        }

        private AbortModeEnum _AbortMode;

        private ACPropertyConfigValue<bool> _InformUserWithMsgNegQuantStock;
        [ACPropertyConfig("en{'Inform user with message dialog about negative quant stock'}de{'Benutzer mit Meldungsdialog über negativen Quantenbestand informieren'}")]
        public bool InformUserWithMsgNegQuantStock
        {
            get => _InformUserWithMsgNegQuantStock.ValueT;
            set
            {
                _InformUserWithMsgNegQuantStock.ValueT = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Properties => Components and FacilityCharge selection

        protected WeighingMaterial _SelectedWeighingMaterial;
        [ACPropertySelected(624, "WeighingMaterial")]
        public virtual WeighingMaterial SelectedWeighingMaterial
        {
            get => _SelectedWeighingMaterial;
            set
            {
                if (_SelectedWeighingMaterial != value)
                {

                    if (WeighingMaterialsFSM && _SelectedWeighingMaterial != null && _SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.Selected)
                    {
                        _SelectedWeighingMaterial.ChangeComponentState(WeighingComponentState.ReadyToWeighing, DatabaseApp);
                    }

                    SelectedFacilityCharge = null;
                    _SelectedWeighingMaterial = value;
                    FacilityChargeList = null;
                    FacilityChargeListCount = 0;
                    OnPropertyChanged();
                    FacilityChargeList = FillFacilityChargeList();
                    FacilityChargeNo = null;
                    ScaleAddActualWeight = _PAFManuallyAddedQuantity != null ? _PAFManuallyAddedQuantity.ValueT : 0;

                    if (_SelectedWeighingMaterial != null && WeighingMaterialsFSM)
                        ShowSelectFacilityLotInfo = true;

                    if (WeighingMaterialsFSM && _SelectedWeighingMaterial != null && _SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.ReadyToWeighing)
                    {
                        _StartWeighingFromF_FC = true;
                        _SelectedWeighingMaterial.ChangeComponentState(WeighingComponentState.Selected, DatabaseApp);

                        if (DiffWeighing && SelectedWeighingMaterial.DiffWeighOnEnd && SelectedWeighingMaterial.QuantInWeighing.HasValue)
                        {
                            var fc = FacilityChargeList.FirstOrDefault(c => c.FacilityChargeID == SelectedWeighingMaterial.QuantInWeighing.Value);
                            if (fc != null)
                            {
                                SelectedFacilityCharge = fc;
                            }
                        }
                        else if (AutoSelectLot)
                        {
                            if (FacilityChargeListCount > 1 && MultipleLotsSelectionRule.HasValue && MultipleLotsSelectionRule.Value > LotSelectionRuleEnum.None)
                            {
                                return;
                            }

                            StartWeighing(false);
                        }
                    }
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
                OnPropertyChanged();
            }
        }

        protected FacilityChargeItem _SelFacilityCharge;

        protected FacilityChargeItem _SelectedFacilityCharge;
        [ACPropertySelected(628, "FacilityCharge")]
        public virtual FacilityChargeItem SelectedFacilityCharge
        {
            get => _SelectedFacilityCharge;
            set
            {
                _SelectedFacilityCharge = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowAllQuants));

                if (value != null)
                    ShowSelectFacilityLotInfo = false;

                if (value != null && InformUserWithMsgNegQuantStock
                        && (_SelFacilityCharge == null
                            || _SelFacilityCharge.FacilityChargeID != value.FacilityChargeID))
                {
                    CheckIsQuantStockNegAndInformUser(value);
                    _SelFacilityCharge = value;
                }

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

                if (_SelFacilityCharge != null && value == null && SelectedWeighingMaterial == null)
                {
                    _SelFacilityCharge = null;
                }
            }
        }

        public int FacilityChargeListCount
        {
            get;
            protected set;
        }

        protected ObservableCollection<FacilityChargeItem> _FacilityChargeList;
        [ACPropertyList(629, "FacilityCharge")]
        public virtual ObservableCollection<FacilityChargeItem> FacilityChargeList
        {
            get
            {
                return _FacilityChargeList;
            }
            set
            {
                _FacilityChargeList = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
                ShowScaleGross = string.IsNullOrEmpty(_PAFCurrentMaterial) || DiffWeighing;
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

        private bool _ShowAllQuants;
        [ACPropertyInfo(630, "", "en{'All quants'}de{'Alle Quants'}", "", true)]
        public bool ShowAllQuants
        {
            get => _ShowAllQuants;
            set
            {
                if (_ShowAllQuants != value)
                {
                    _ShowAllQuants = value;
                    ParentBSOWCS.ApplicationQueue.Add(() =>
                    {
                        _FacilityChargeList = null;
                        FacilityChargeList = FillFacilityChargeList();
                    });
                }
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
                OnPropertyChanged();
                if (_SelectedSingleDosingItem != null)
                {
                    BroadcastToVBControls(Const.CmdFocusAndSelectAll, nameof(SingleDosTargetQuantity), null);
                }
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
                OnPropertyChanged();
            }
        }

        protected ACClass _TempSelectedSingleDosTargetStorage;
        protected ACClass _SelectedSingleDosTargetStorage;

        [ACPropertySelected(653, "SingleDosTargetStorage", "en{'Destination'}de{'Ziel'}")]
        public virtual ACClass SelectedSingleDosTargetStorage
        {
            get => _SelectedSingleDosTargetStorage;
            set
            {
                _SelectedSingleDosTargetStorage = value;
                OnPropertyChanged();
            }
        }

        [ACPropertyList(653, "SingleDosTargetStorage")]
        public virtual IEnumerable<ACClass> SingleDosTargetStorageList
        {
            get;
            set;
        }

        private int _SingleDosNumberOfRepetitions;
        [ACPropertyInfo(851, "", "en{'Number of repetitions'}de{'Anzahl der Wiederholungen'}")]
        public int SingleDosNumberOfRepetitions
        {
            get => _SingleDosNumberOfRepetitions;
            set
            {
                _SingleDosNumberOfRepetitions = value;
                OnPropertyChanged();
            }
        }

        [ACPropertyInfo(true, 9999)]
        public int MaxSingleDosNumOfRepetitions
        {
            get;
            set;
        }

        #endregion

        #region Properties => Rework

        protected bool IsReworkEnabled = false;

        private ReworkInfo _SelectedReworkInfo;
        [ACPropertySelected(800, "ReworkInfo", "en{''}de{''}")]
        public ReworkInfo SelectedReworkInfo
        {
            get => _SelectedReworkInfo;
            set
            {
                _SelectedReworkInfo = value;
                OnPropertyChanged();
            }
        }

        private ReworkInfoList _ReworkInfoItems;
        [ACPropertyList(800, "ReworkInfo")]
        public ReworkInfoList ReworkInfoItems
        {
            get => _ReworkInfoItems;
            set
            {
                _ReworkInfoItems = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Methods

        public override void Activate(ACComponent selectedProcessModule)
        {
            CurrentProcessModule = selectedProcessModule;
            base.Activate(selectedProcessModule);
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

        [ACMethodInfo("", "en{'Weigh'}de{'Wiegen'}", 601, true)]
        public virtual void Weigh()
        {
            IACComponentPWNode componentPWNode = ComponentPWNodeLocked;
            if (componentPWNode != null)
            {
                componentPWNode.ExecuteMethod(nameof(PWManualWeighing.WeighDifference));
            }
        }

        public virtual bool IsEnabledWeigh()
        {
            return DiffWeighing;
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
                return new Msg(this, eMsgLevel.Error, nameof(BSOManualWeighing), nameof(StartWeighing) + "10", 889, "Error50330");
            }

            if (SelectedWeighingMaterial != null)
            {
                if (SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.ReadyToWeighing ||
                    SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.Selected)
                {
                    Guid posID = Guid.Empty;
                    if (SelectedWeighingMaterial.PosRelation != null)
                        posID = SelectedWeighingMaterial.PosRelation.ProdOrderPartslistPosRelationID;
                    else if (SelectedWeighingMaterial.PickingPosition != null)
                        posID = SelectedWeighingMaterial.PickingPosition.PickingPosID;

                    Msg msg = componentPWNode.ExecuteMethod(nameof(PWManualWeighing.StartWeighing), posID,
                                                                             SelectedFacilityCharge?.FacilityChargeID, null, forceSetFC_F) as Msg;
                    return msg;
                }
                else
                {
                    //Info50036: The component or material {0} has already been weighed or is already in the weighing process!
                    // Die Komponente bzw. Material {0} wurde bereits verwogen oder befindet sich schon im Wiegeprozess!
                    return new Msg(this, eMsgLevel.Error, nameof(BSOManualWeighing), nameof(StartWeighing) + "20", 904, "Info50036", SelectedWeighingMaterial.PosRelation.SourceProdOrderPartslistPos.MaterialName);
                }
            }
            //Info50037: The component or material and the lot (quant) has not been selected. Select it and then press the "Weighing" button.
            // Die Komponente bzw. Material und die Charge (Quant) wurde nicht ausgewählt. Wählen Sie es aus und drücken anschliessend die Taste "Wiegen".
            return new Msg(this, eMsgLevel.Error, nameof(BSOManualWeighing), nameof(StartWeighing) + "30", 908, "Info50037");
        }

        [ACMethodInfo("", "en{'Acknowledge'}de{'Quittieren'}", 602, true)]
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
                    //if (ScaleBckgrState != ScaleBackgroundState.InTolerance)
                    //{
                    //    //Question50072 : Are you sure that you want acknowledge current component?
                    //    if (Messages.Question(this, "Question50072") != Global.MsgResult.Yes)
                    //        return;
                    //}

                    componentPWNode.ExecuteMethod(nameof(PWManualWeighing.CompleteWeighing), ScaleActualWeight, false);
                }
            }
        }

        public virtual bool IsEnabledAcknowledge()
        {
            return (MessagesList.Any(c => !c.IsAlarmMessage && c.HandleByAcknowledgeButton) || (MaxScaleWeight.HasValue && TargetWeight > MaxScaleWeight)
                                                                                            || ScaleBckgrState == ScaleBackgroundState.InTolerance);
        }

        [ACMethodInfo("", "en{'Acknowledge'}de{'Quittieren'}", 602, true)]
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

        [ACMethodInfo("", "en{'Yes'}de{'Ja'}", 602, true)]
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

        [ACMethodInfo("", "en{'No'}de{'Nein'}", 602, true)]
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

        [ACMethodInfo("", "en{'Tare'}de{'Tarieren'}", 603, true)]
        public virtual void Tare()
        {
            if (!IsEnabledTare())
                return;

            CurrentPAFManualWeighing?.ExecuteMethod(nameof(PAFManualWeighing.TareActiveScale));
        }

        public bool IsEnabledTare()
        {
            return true; //TODO
        }

        [ACMethodInfo("", "en{'Lot change'}de{'Chargenwechsel'}", 604, true)]
        public virtual void LotChange()
        {
            if (!IsEnabledLotChange())
                return;

            if (SelectedWeighingMaterial != null && SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.InWeighing)
            {
                ShowSelectFacilityLotInfo = true;
                OnPropertyChanged(nameof(SelectedFacilityCharge));
                _CallPWLotChange = true;
                _IsLotConsumed = false;
                if (SelectedWeighingMaterial.IsLotManaged)
                {
                    double? zeroBookTolerance = SelectedWeighingMaterial?.PosRelation?.SourceProdOrderPartslistPos?.Material?.ZeroBookingTolerance;
                    if (zeroBookTolerance.HasValue && Math.Abs(zeroBookTolerance.Value) > double.Epsilon)
                    {
                        double stockAfterBooking = SelectedFacilityCharge.StockQuantityUOM - ScaleActualWeight;
                        if (zeroBookTolerance > stockAfterBooking)
                        {
                            _IsLotConsumed = true;
                        }
                    }

                    if (!_IsLotConsumed)
                    {
                        //Question50047: Was the material with the lot number {0} used up?
                        // Wurde das Material mit der Chargennummer{0} aufgebraucht?
                        Global.MsgResult result = Messages.Question(this, "Question50047", Global.MsgResult.Cancel, false, SelectedFacilityCharge.FacilityChargeNo);
                        if (result == Global.MsgResult.Yes)
                        {
                            //Question50048: Are you sure the batch is used up?
                            // Sind Sie sicher dass die Charge aufgebraucht ist?
                            if (Messages.Question(this, "Question50048", Global.MsgResult.Cancel) == Global.MsgResult.Yes)
                                _IsLotConsumed = true;
                        }
                        else if (result == Global.MsgResult.Cancel)
                        {
                            ShowSelectFacilityLotInfo = false;
                            _CallPWLotChange = false;
                        }
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
            return SelectedWeighingMaterial != null && !SelectedWeighingMaterial.DiffWeighOnEnd;
        }

        public virtual double OnDetermineLotChangeActualQuantity()
        {
            return ScaleActualWeight;
        }

        [ACMethodInfo("", "en{'Bin change'}de{'Eimerwechsel'}", 605, true)]
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

        [ACMethodInfo("", "en{'Abort'}de{'Abbrechen'}", 606, true)]
        public virtual void Abort()
        {
            if (!IsEnabledAbort())
                return;

            IACComponentPWNode componentPWNode = ComponentPWNodeLocked;

            if (componentPWNode != null)
            {
                if (DiffWeighing)
                {
                    // Question500 : Are you sure that you want complete weighing of all components?
                    if (Messages.Question(this, "Question50096") == Global.MsgResult.Yes)
                    {
                        componentPWNode.ExecuteMethod(nameof(PWManualWeighing.CompleteWeighing), ScaleActualWeight, false);
                    }
                }
                else
                {
                    InterdischargeStart(componentPWNode, true);

                    _AbortMode = AbortModeEnum.Cancel;
                    ShowDialog(this, "AbortDialog", "", false, Global.ControlModes.Hidden, Global.ControlModes.Hidden);

                    if (_AbortMode == AbortModeEnum.AbortComponent)
                    {
                        ShowSelectFacilityLotInfo = false;
                        //Question50049: Do you still want to weigh this material in the following batches? 
                        // Möchten Sie diese Komponente in den nachfolgenden Batchen weiter verwiegen?
                        if (Messages.Question(this, "Question50049") == Global.MsgResult.No)
                        {
                            componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.Abort), true, false);
                            return;
                        }
                        componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.Abort), false, false);

                    }
                    else if (_AbortMode == AbortModeEnum.AbortComponentScaleOtherComponents)
                    {
                        ShowSelectFacilityLotInfo = false;
                        if (Messages.Question(this, "Question50049") == Global.MsgResult.No)
                        {
                            componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.Abort), true, true);
                            return;
                        }
                        componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.Abort), false, true);
                    }
                    else if (_AbortMode == AbortModeEnum.AbortComponentSwitchToEmptyingMode)
                    {
                        ShowSelectFacilityLotInfo = false;
                        //Question50049: Do you no longer want to weigh this material in the following batches? (e.g. for rework if it has been used up)
                        // Möchten Sie dieses Material in den nachfolgenden Batchen nicht mehr verwiegen? (z.B. bei Rework wenn es aufgebraucht worden ist)

                        Global.MsgResult msgResult = Messages.Question(this, "Question50049");

                        ParentBSOWCS?.SelectExtraDisTargetOnPWGroup();

                        if (msgResult == Global.MsgResult.No)
                        {
                            componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.Abort), true, false);
                            return;
                        }
                        componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.Abort), false, false);
                    }
                    else if (_AbortMode == AbortModeEnum.SwitchToEmptyingMode)
                    {
                        ParentBSOWCS?.SelectExtraDisTargetOnPWGroup();
                    }
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

        [ACMethodInfo("", "en{'Apply charge/lot'}de{'Charge/Los anwenden'}", 607, true)]
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
                msg = new Msg(this, eMsgLevel.Error, nameof(BSOManualWeighing), nameof(ApplyLot) + "10", 1035, "Error50330");
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
        public virtual void AddKg()
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
        public virtual void RemoveKg()
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
            NetWeightOfLargestScale = null;
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
                var scalesArray = scaleObjects.Select(c => new ACRef<IACComponent>(c, this)).ToArray();
                scaleObjectInfoList = new List<ACValueItem>(scalesArray.Select(c => new ACValueItem(c.ValueT.ACCaption, c.ACUrl, null)));

                using (ACMonitor.Lock(_70750_ProcessModuleScalesLock))
                {
                    _ProcessModuleScales = scalesArray;
                    if (_ProcessModuleScales != null)
                    {
                        var scale = _ProcessModuleScales.LastOrDefault();
                        if (scale != null)
                            NetWeightOfLargestScale = scale.ValueT.GetPropertyNet(nameof(PAEScaleBase.ActualWeight)) as IACContainerTNet<double>;
                    }
                    ScaleObjectsList = new ObservableCollection<ACValueItem>(scaleObjectInfoList);
                }
            }

            var orderInfo = currentProcessModule.GetPropertyNet(nameof(PAProcessModuleVB.OrderInfo));
            if (orderInfo == null)
            {
                //Error50285: Initialization error: The process module doesn't have the property {0}.
                // Initialisierungsfehler: Das Prozessmodul besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50285", false, nameof(PAProcessModuleVB.OrderInfo));
                return false;
            }

            ACMethod acMethod = LoadPAFManualWeighing(pafManWeighingRef);

            //if (scaleObjectInfoList != null && scaleObjectInfoList.Any())
            //{
            //    //HandleActiveScaleObject(scaleObjectInfoList, null);
            //    //SelectActiveScaleObject(scaleObjectInfoList, true);
            //}

            _OrderInfo = orderInfo as IACContainerTNet<string>;

            LoadWFNode(currentProcessModule, _OrderInfo.ValueT);

            HandlePAFCurrentACMethod(acMethod);

            InitRework(currentProcessModule);

            orderInfo.PropertyChanged += OrderInfoPropertyChanged;

            return true;
        }

        public virtual IACComponent GetTargetFunction(IEnumerable<IACComponent> processModuleChildrenComponents)
        {
            return processModuleChildrenComponents.FirstOrDefault(c => _PAFManualWeighingType.IsAssignableFrom(c.ComponentClass.ObjectType)
                                                                    && !_PAFManualAdditionType.IsAssignableFrom(c.ComponentClass.ObjectType));
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
                Messages.Info(this, "Error50287", false, nameof(PAProcessFunction.CurrentACMethod));
                return null;
            }

            var manuallyAddedQuantity = pafManWeighing.ValueT.GetPropertyNet(nameof(PAFManualWeighing.ManuallyAddedQuantity));
            if (manuallyAddedQuantity == null)
            {
                //Error50287: Initialization error: The weighing function doesn't have the property {0}.
                // Initialisierungsfehler: Die Verwiegefunktion besitzt nicht die Eigenschaft {0}.
                Messages.Info(this, "Error50287", false, nameof(PAFManualWeighing.ManuallyAddedQuantity));
                return null;
            }

            var tareScaleState = pafManWeighing.ValueT.GetPropertyNet(nameof(PAFManualWeighing.TareScaleState));
            if (tareScaleState == null)
            {
                //Error50287: Initialization error: The weighing function doesn't have the property {0}.
                // Initialisierungsfehler: Die Verwiegefunktion besitzt nicht die Eigenschaft {0}.
                Messages.Info(this, "Error50287", false, nameof(PAFManualWeighing.TareScaleState));
                return null;
            }

            var activeScaleObjectACUrl = pafManWeighing.ValueT.GetPropertyNet(nameof(PAFManualWeighing.ActiveScaleObjectUrl));
            if (activeScaleObjectACUrl == null)
            {
                //Error50287: Initialization error: The weighing function doesn't have the property {0}.
                // Initialisierungsfehler: Die Verwiegefunktion besitzt nicht die Eigenschaft {0}.
                Messages.Info(this, "Error50287", false, nameof(PAFManualWeighing.ActiveScaleObjectUrl));
                return null;
            }

            _ActiveScaleObjectACUrl = activeScaleObjectACUrl as IACContainerTNet<string>;
            _ActiveScaleObjectACUrl.PropertyChanged += _ActiveScaleObjectACUrl_PropertyChanged;
            HandleActiveScaleObject(ScaleObjectsList, _ActiveScaleObjectACUrl.ValueT, true);

            var tempMethod = currentACMethod as IACContainerTNet<ACMethod>;
            tempMethod.PropertyChanged += PAFCurrentACMethodPropChanged;
            ACMethod temp = tempMethod?.ValueT?.Clone() as ACMethod;

            using (ACMonitor.Lock(_70700_PrivateMemberLock))
            {
                _PAFCurrentACMethod = tempMethod;
            }

            _PAFManuallyAddedQuantity = manuallyAddedQuantity as IACContainerTNet<double>;
            (_PAFManuallyAddedQuantity as IACPropertyNetBase).PropertyChanged += PAFManuallyAddedQuantityPropChanged;
            ScaleAddActualWeight = _PAFManuallyAddedQuantity.ValueT;

            _TareScaleState = tareScaleState as IACContainerT<short>;

            return temp;
        }

        protected void ActivateScale(IACComponent scale)
        {
            if (scale == null)
                return;

            var actWeightProp = scale.GetPropertyNet(nameof(PAEScaleBase.ActualWeight));
            if (actWeightProp == null)
            {
                //Error50292: Initialization error: The scale component doesn't have the property {0}.
                // Initialisierungsfehler: Die Waagen-Komponente besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50292", false, nameof(PAEScaleBase.ActualWeight));
                return;
            }

            MaxScaleWeight = null;
            var actValProp = scale.GetPropertyNet(nameof(PAEScaleBase.ActualValue)) as IACContainerTNet<double>;
            if (actValProp == null)
            {
                //Error50292: Initialization error: The scale component doesn't have the property {0}.
                // Initialisierungsfehler: Die Waagen-Komponente besitzt nicht die Eigenschaft {0}.
                Messages.Error(this, "Error50292", false, nameof(PAEScaleBase.ActualValue));
                return;
            }

            double digitWeight = 1.0;
            var digitWeightProp = scale.GetPropertyNet(nameof(PAEScaleBase.DigitWeight));
            if (digitWeightProp != null)
            {
                digitWeight = (digitWeightProp as IACContainerTNet<double>).ValueT;
                if (digitWeight <= double.Epsilon)
                    digitWeight = 1.0;
            }
            if (digitWeight >= 999.99999)
                ScalePrecisionFormat = "F0";
            else if (digitWeight >= 99.99999)
                ScalePrecisionFormat = "F1";
            else if (digitWeight >= 9.99999)
                ScalePrecisionFormat = "F2";
            else if (digitWeight >= 0.99999)
                ScalePrecisionFormat = "F3";
            else if (digitWeight >= 0.09999)
                ScalePrecisionFormat = "F4";
            else if (digitWeight >= 0.00999)
                ScalePrecisionFormat = "F5";
            else if (digitWeight >= 0.00099)
                ScalePrecisionFormat = "F6";

            _ScaleActualValue = actValProp;

            _ScaleActualWeight = actWeightProp as IACContainerTNet<double>;
            (_ScaleActualWeight as IACPropertyNetBase).PropertyChanged += ActWeightProp_PropertyChanged;
            (_ScaleActualValue as IACPropertyNetBase).PropertyChanged += ScaleActualValue_PropertyChanged;
            ScaleRealWeight = _ScaleActualWeight.ValueT;
            ScaleGrossWeight = _ScaleActualValue.ValueT;
            OnPropertyChanged(nameof(TargetWeight));
            OnPropertyChanged(nameof(ScaleDifferenceWeight));

            if (_ActiveScaleObjectACUrl != null && _ActiveScaleObjectACUrl.ValueT != scale.ACUrl)
            {
                _ActiveScaleObjectACUrl.ValueT = scale.ACUrl;
            }

            //if (CurrentPAFManualWeighing != null && scale != null)
            //{
            //    CurrentPAFManualWeighing.ExecuteMethod(nameof(PAFManualWeighing.SetActiveScaleObject), scale.ACIdentifier);
            //}

            OnPropertyChanged(nameof(CurrentScaleObject));
        }

        [ACMethodInfo("", "en{'ConvertWeightToUIText'}de{'ConvertWeightToUIText'}", 998)]
        public string ConvertWeightToUIText(double value)
        {
            return value.ToString(ScalePrecisionFormat);
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
                            Messages.LogError(this.GetACUrl(), nameof(LoadWFNode) + "(10)", "Returned");
                            return;
                        }
                    }

                    string pwGroupACUrl = accessArr[0];
                    IACComponentPWNode pwGroup = null;

                    pwGroup = Root.ACUrlCommand(pwGroupACUrl) as IACComponentPWNode;
                    if (pwGroup == null)
                    {
                        pwGroup = Root.ACUrlCommand(pwGroupACUrl) as IACComponentPWNode;

                        if (pwGroup == null)
                        {
                            using (ACMonitor.Lock(_70600_CurrentOrderInfoValLock))
                            {
                                _CurrentOrderInfoValue = null;
                                Messages.LogError(this.GetACUrl(), nameof(LoadWFNode) + "(20)", "Returned");
                                return;
                            }
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

                        if (pwNodes == null)
                        {
                            pwNodes = pwGroup.GetChildInstanceInfo(1, new ChildInstanceInfoSearchParam() { OnlyWorkflows = true, TypeOfRoots = refClass });
                            
                            if (pwNodes == null)
                            {
                                Thread.Sleep(500);
                                pwNodes = pwGroup.GetChildInstanceInfo(1, new ChildInstanceInfoSearchParam() { OnlyWorkflows = true, TypeOfRoots = refClass });
                            }
                        }

                        refClass.Detach();
                    }

                    if (pwNodes == null || !pwNodes.Any())
                    {
                        using (ACMonitor.Lock(_70600_CurrentOrderInfoValLock))
                        {
                            _CurrentOrderInfoValue = null;
                            Messages.LogError(this.GetACUrl(), nameof(LoadWFNode) + "(30)", "Returned - pwNodes is null = " + (pwNodes == null) as string);
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

                    var mwNode = mwPWNodes?.FirstOrDefault();
                    if (mwNode != null)
                    {
                        string configStoresInfo = mwNode.ComponentPWNode?.ValueT?.ExecuteMethod(nameof(PWManualWeighing.GetPWParametersInfo)) as string;
                        if (!string.IsNullOrEmpty(configStoresInfo))
                        {
                            AddToMessageList(new MessageItemPWInfo(null, null, eMsgLevel.Info) { Message = configStoresInfo, HandleByAcknowledgeButton = false });
                            RefreshMessageList();
                        }
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
                    pwNode = pwGroup.ACUrlCommand(node.ACUrlParent + "\\" + node.ACIdentifier) as IACComponentPWNode;
                }
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
                    Messages.LogError(this.GetACUrl(), nameof(ActivateWFNode) + "(10)", "Returned");
                    return;
                }

                ComponentPWNode = pwNode;
            }

            var weighingCompInfo = pwNode.ValueT.GetPropertyNet(nameof(PWManualWeighing.CurrentWeighingComponentInfo));
            if (weighingCompInfo == null)
            {
                //Error50291: Initialization error: The reference to the property {1} in Workflownode {0} is null.
                // Initialisierungsfehler: Die Referenz zur Eigenschaft {1} von Workflowknoten {0} ist null.
                Messages.LogError(this.GetACUrl(), nameof(ActivateWFNode), "Weighing component info is null");
                Messages.Error(this, "Error50291", false, pwNode?.ACUrl, "CurrentWeighingComponentInfo");
                return;
            }

            if (!IsBinChangeAvailable)
            {
                IACContainerTNet<bool> binChangeProp = pwNode.ValueT.GetPropertyNet(nameof(PWManualWeighing.IsBinChangeLoopNodeAvailable)) as IACContainerTNet<bool>;
                IsBinChangeAvailable = binChangeProp != null ? binChangeProp.ValueT : false;
            }

            using (ACMonitor.Lock(DatabaseApp.QueryLock_1X000))
                _ = DatabaseApp.ProdOrderPartslistPos.FirstOrDefault();

            LoadPWConfiguration(pwNode?.ValueT);

            try
            {
                //Messages.LogInfo("ManualWeighingModel", "SetupModel", "GetMaterials start.");

                WeighingMaterialList = GetWeighingMaterials(pwNode.ValueT, DatabaseApp, DefaultMaterialIcon);

                //Messages.LogInfo("ManualWeighingModel", "SetupModel", "After GetWeighingMaterials. Count: " + WeighingMaterialList?.Count());
            }
            catch (Exception e)
            {
                string message;
                if (e.InnerException != null)
                    message = string.Format("ManualWeighingModel(Setup model): {0}, {1} {2} {3}", e.Message, e.InnerException.Message, System.Environment.NewLine, e.StackTrace);
                else
                    message = string.Format("ManualWeighingModel(Setup model): {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace);

                Messages.LogError(this.GetACUrl(), nameof(ActivateWFNode), message);
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

        public List<WeighingMaterial> GetWeighingMaterials(IACComponentPWNode pwNode, VD.DatabaseApp db, ACClassDesign iconDesign = null)
        {
            if (pwNode == null)
                return null;

            ACValue acValue = pwNode.ACUrlCommand(nameof(PWManualWeighing.WeighingComponentsInfo) + "\\" + Const.ValueT) as ACValue;
            Dictionary<string, string> valueList = acValue?.Value as Dictionary<string, string>;

            if (valueList != null && valueList.Any())
            {
                List<WeighingMaterial> weighingMaterials = new List<WeighingMaterial>();

                var intermediateChildPos = valueList.FirstOrDefault(c => c.Value == null);
                Guid intermediateChildPosPOPartslistID;
                if (intermediateChildPos.Key != null && Guid.TryParse(intermediateChildPos.Key, out intermediateChildPosPOPartslistID))
                {
                    using (ACMonitor.Lock(db.QueryLock_1X000))
                    {
                        VD.ProdOrderPartslistPosRelation[] queryOpenDosings = new VD.ProdOrderPartslistPosRelation[0];
                        try
                        {
                            queryOpenDosings = PWManualWeighing.Qry_WeighMaterials(db, intermediateChildPosPOPartslistID);
                        }
                        catch (Exception ex)
                        {
                            Messages.LogException(this.GetACUrl(), nameof(GetWeighingMaterials) + "(10)", ex);
                            try
                            {
                                queryOpenDosings = PWManualWeighing.Qry_WeighMaterials(db, intermediateChildPosPOPartslistID);
                            }
                            catch (Exception exc)
                            {
                                Messages.LogException(this.GetACUrl(), nameof(GetWeighingMaterials) + "(15)", exc);
                            }
                        }

                        foreach (VD.ProdOrderPartslistPosRelation rel in queryOpenDosings)
                        {
                            try
                            {
                                rel.AutoRefresh();
                            }
                            catch (Exception e)
                            {
                                Messages.LogException(this.GetACUrl(), nameof(GetWeighingMaterials) + "(20)", e);
                            }

                            var valueItem = valueList.FirstOrDefault(c => c.Key == rel.ProdOrderPartslistPosRelationID.ToString());
                            if (valueItem.Key == null)
                                continue;

                            if (short.TryParse(valueItem.Value, out short weighingState))
                            {
                                weighingMaterials.Add(new WeighingMaterial(rel, (WeighingComponentState)weighingState, iconDesign, this));
                            }
                        }
                    }
                }
                else
                {
                    foreach (var valueItem in valueList.ToArray())
                    {
                        if (Guid.TryParse(valueItem.Key, out Guid PLPosRel))
                        {
                            if (short.TryParse(valueItem.Value, out short weighingState))
                            {
                                using (ACMonitor.Lock(db.QueryLock_1X000))
                                {
                                    if (ParentBSOWCS != null && ParentBSOWCS.CurrentPicking != null)
                                    {
                                        VD.PickingPos pickingPos = db.PickingPos.Include(c => c.PickingMaterial).FirstOrDefault(c => c.PickingPosID == PLPosRel);
                                        if (pickingPos != null)
                                        {
                                            try
                                            {
                                                pickingPos.AutoRefresh();
                                            }
                                            catch (Exception e)
                                            {
                                                Messages.LogException(this.GetACUrl(), nameof(GetWeighingMaterials), e);
                                            }
                                            weighingMaterials.Add(new WeighingMaterial(pickingPos, (WeighingComponentState)weighingState, iconDesign));
                                        }
                                    }
                                    else
                                    {
                                        VD.ProdOrderPartslistPosRelation posRelation = db.ProdOrderPartslistPosRelation.Include(s => s.SourceProdOrderPartslistPos.Material.MDFacilityManagementType)
                                                                                     .FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == PLPosRel);
                                        if (posRelation != null)
                                        {
                                            try
                                            {
                                                posRelation.AutoRefresh();
                                            }
                                            catch (Exception e)
                                            {
                                                Messages.LogException(this.GetACUrl(), nameof(GetWeighingMaterials), e);
                                            }
                                            weighingMaterials.Add(new WeighingMaterial(posRelation, (WeighingComponentState)weighingState, iconDesign, this));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (ParentBSOWCS != null && ParentBSOWCS.CurrentPicking != null)
                    return weighingMaterials.OrderBy(c => c.PickingPosition.Sequence).ToList();

                return weighingMaterials.OrderBy(c => c.PosRelation.Sequence).ToList();
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

            var scaleOtherComp = acMethod.ParameterValueList.GetACValue("ScaleOtherComp");
            if (scaleOtherComp != null)
                ScaleOtherComponentOnAbort = scaleOtherComp.ParamAsBoolean;

            var multipleLotSelRule = acMethod.ParameterValueList.GetACValue("RuleForSelMulLots");
            if (multipleLotSelRule != null)
                MultipleLotsSelectionRule = multipleLotSelRule.Value as LotSelectionRuleEnum?;

            var diffWeighing = acMethod.ParameterValueList.GetACValue("DiffWeighing");
            if (diffWeighing != null)
                DiffWeighing = diffWeighing.ParamAsBoolean;

            var autoSelectLot = acMethod.ParameterValueList.GetACValue("AutoSelectLot");
            if (autoSelectLot != null)
                AutoSelectLot = autoSelectLot.ParamAsBoolean;

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
            CurrentScaleObject = null;
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

            NetWeightOfLargestScale = null;
            ACRef<IACComponent>[] scales = null;
            using (ACMonitor.Lock(_70750_ProcessModuleScalesLock))
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
            //BtnWeighBlink = false;
            NextTaskInfo = ManualWeighingTaskInfo.None;

            _PAFManuallyAddedQuantity = null;
            _TareScaleState = null;
            _CallPWLotChange = false;
            _ScaleObjectsList = null;
        }

        private void DeactivateWFNode(bool reset = false)
        {
            WeighingMaterialList = null;
            SelectedWeighingMaterial = null;
            ShowSelectFacilityLotInfo = false;

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

            var msgPWInfo = MessagesListSafe.FirstOrDefault(c => c is MessageItemPWInfo);
            if (msgPWInfo != null)
            {
                RemoveFromMessageList(msgPWInfo);
                RefreshMessageList();
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

            if (_ActiveScaleObjectACUrl != null)
            {
                _ActiveScaleObjectACUrl.PropertyChanged -= _ActiveScaleObjectACUrl_PropertyChanged;
                _ActiveScaleObjectACUrl = null;
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
                using (ACMonitor.Lock(_70700_PrivateMemberLock))
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

        protected void ActWeightProp_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
                ParentBSOWCS?.ApplicationQueue.Add(() => ScaleAddActualWeight = tempValue);
            }
        }

        protected void OnTargetWeightChanged(double targetWeight)
        {
            var facilityCharges = FacilityChargeList;
            if (facilityCharges == null || !facilityCharges.Any())
                return;

            foreach (var fc in facilityCharges)
            {
                fc.OnTargetQunatityChanged(targetWeight);
            }
        }

        private void _ActiveScaleObjectACUrl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Const.ValueT)
            {
                IACContainerTNet<string> activeScaleObjectACUrl = sender as IACContainerTNet<string>;
                if (activeScaleObjectACUrl != null)
                {
                    string scaleACUrl = activeScaleObjectACUrl.ValueT;
                    ParentBSOWCS?.ApplicationQueue.Add(() => HandleActiveScaleObject(ScaleObjectsList, scaleACUrl));
                }
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
                    Messages.LogError(this.GetACUrl(), nameof(HandlePWNodeACState) + "(10)", "ComponentPWNodesList is null!");
                    return;
                }

                pwNode = ComponentPWNodesList.FirstOrDefault(c => c.ComponentPWNodeACState == senderProp);
                if (pwNode == null)
                {
                    Messages.LogError(this.GetACUrl(), nameof(HandlePWNodeACState) + "(20)", "pwNode is null!");
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
                            Messages.LogError(this.GetACUrl(), nameof(HandlePWNodeACState) + "(30)", "ComponentPWNode is different than sender!");
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

        protected virtual void HandlePAFCurrentACMethod(ACMethod currentACMethod)
        {
            if (currentACMethod != null)
            {
                _TolerancePlus = currentACMethod.ParameterValueList.GetDouble("TolerancePlus");
                _ToleranceMinus = currentACMethod.ParameterValueList.GetDouble("ToleranceMinus");

                ACValue paramMaterial = currentACMethod.ParameterValueList.GetACValue("Material");
                if (paramMaterial != null)
                    PAFCurrentMaterial = paramMaterial.ParamAsString;
                TargetWeight = currentACMethod.ParameterValueList.GetDouble("TargetQuantity");
            }
            else
            {
                PAFCurrentMaterial = "";
                TargetWeight = 0;
                ScaleBckgrState = ScaleBackgroundState.Weighing;
            }

            CorrectManualWeighingItems();
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
                            WeighingMaterial comp = null;
                            if (ParentBSOWCS != null && ParentBSOWCS.CurrentPicking != null && compInfo.PickingPos != Guid.Empty)
                                comp = WeighingMaterialList?.FirstOrDefault(c => c.PickingPosition.PickingPosID == compInfo.PickingPos);
                            else
                                comp = WeighingMaterialList?.FirstOrDefault(c => c.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation);
                            
                            if (comp != null)
                            {
                                comp.ChangeComponentState((WeighingComponentState)compInfo.WeighingComponentState, DatabaseApp);
                                OnPropertyChanged(nameof(CurrentScaleObject));
                            }
                            break;
                        }
                    case WeighingComponentInfoType.StateSelectCompAndFC_F:
                        {
                            WeighingMaterial comp = null;
                            if (ParentBSOWCS != null && ParentBSOWCS.CurrentPicking != null && compInfo.PickingPos != Guid.Empty)
                                comp = WeighingMaterialList?.FirstOrDefault(c => c.PickingPosition.PickingPosID == compInfo.PickingPos);
                            else
                                comp = WeighingMaterialList?.FirstOrDefault(c => c.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation);

                            if (comp == null)
                                return;

                            if (SelectedWeighingMaterial != comp)
                            {
                                SelectedWeighingMaterial = comp;
                                if (compInfo.FacilityCharge == null)
                                    ShowSelectFacilityLotInfo = true;
                            }

                            comp.ChangeComponentState((WeighingComponentState)compInfo.WeighingComponentState, DatabaseApp);
                            OnPropertyChanged(nameof(CurrentScaleObject));

                            if (compInfo.FacilityCharge != null)
                            {
                                var fcItem = FacilityChargeList?.FirstOrDefault(c => c.FacilityChargeID == compInfo.FacilityCharge);
                                SelectedFacilityCharge = fcItem;
                                Thread.Sleep(500);
                                OnPropertyChanged(nameof(SelectedFacilityCharge));
                            }

                            break;
                        }
                    case WeighingComponentInfoType.SelectCompReturnFC_F:
                        {
                            WeighingMaterial comp = null;
                            if (ParentBSOWCS != null && ParentBSOWCS.CurrentPicking != null && compInfo.PickingPos != Guid.Empty)
                                comp = WeighingMaterialList?.FirstOrDefault(c => c.PickingPosition.PickingPosID == compInfo.PickingPos);
                            else
                                comp = WeighingMaterialList?.FirstOrDefault(c => c.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation);

                            if (SelectedWeighingMaterial != comp)
                            {
                                SelectedWeighingMaterial = comp;
                                SelectedWeighingMaterial.ChangeComponentState(WeighingComponentState.Selected, DatabaseApp);
                            }

                            if (SelectedWeighingMaterial != null && SelectedFacilityCharge == null)
                            {
                                _StartWeighingFromF_FC = true;
                                ShowSelectFacilityLotInfo = true;
                            }

                            break;
                        }
                    case WeighingComponentInfoType.StateSelectFC_F:
                    case WeighingComponentInfoType.SelectFC_F:
                        {
                            WeighingMaterial comp = null;
                            if (ParentBSOWCS != null && ParentBSOWCS.CurrentPicking != null && compInfo.PickingPos != Guid.Empty)
                                comp = WeighingMaterialList.FirstOrDefault(c => c.PickingPosition.PickingPosID == compInfo.PickingPos);
                            else
                                comp = WeighingMaterialList?.FirstOrDefault(c => c.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation);

                            if ((WeighingComponentState)compInfo.WeighingComponentState == WeighingComponentState.InWeighing
                            && SelectedWeighingMaterial == null)
                            {
                                if (comp != null)
                                    SelectedWeighingMaterial = comp;
                            }

                            if (SelectedWeighingMaterial != null && 
                                ((compInfo.PLPosRelation != Guid.Empty && SelectedWeighingMaterial.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation)
                                || compInfo.PickingPos != Guid.Empty && SelectedWeighingMaterial.PickingPosition.PickingPosID == compInfo.PickingPos))
                            {
                                if (SelectedWeighingMaterial.WeighingMatState != (WeighingComponentState)compInfo.WeighingComponentState)
                                {
                                    SelectedWeighingMaterial.ChangeComponentState((WeighingComponentState)compInfo.WeighingComponentState, DatabaseApp);
                                }

                                OnPropertyChanged(nameof(CurrentScaleObject));
                            }

                            if (compInfo.FacilityCharge != null)
                            {
                                bool autoRefresh = compInfo.FC_FAutoRefresh;
                                if (SelectedFacilityCharge == null || _CallPWLotChange || compInfo.IsLotChange)
                                {
                                    if (autoRefresh)
                                    {
                                        SelectedFacilityCharge = null;
                                        FacilityChargeList = null;
                                        FacilityChargeListCount = 0;

                                        FacilityChargeList = FillFacilityChargeList();
                                    }

                                    var fcItem = FacilityChargeList?.FirstOrDefault(c => c.FacilityChargeID == compInfo.FacilityCharge);
                                    SelectedFacilityCharge = fcItem;
                                    Thread.Sleep(500);
                                    OnPropertyChanged(nameof(SelectedFacilityCharge));
                                }
                            }
                            break;
                        }
                    case WeighingComponentInfoType.RefreshCompTargetQ:
                        {
                            var refreshItems = WeighingMaterialList?.Where(c => c.WeighingMatState <= WeighingComponentState.InWeighing
                                                                            || c.WeighingMatState == WeighingComponentState.PartialCompleted).ToList();

                            try
                            {
                                foreach (WeighingMaterial refreshItem in refreshItems)
                                {
                                    refreshItem.PosRelation.AutoRefresh();
                                }
                            }
                            catch
                            {
                            }
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

        private void HandleActiveScaleObject(IEnumerable<ACValueItem> scaleObjects, string newScaleObjectACUrl, bool setIfNotSelected = false)
        {
            if (newScaleObjectACUrl != null)
            {
                CurrentScaleObject = scaleObjects?.FirstOrDefault(c => c.Value as string == newScaleObjectACUrl);
            }
            else if (setIfNotSelected)
            {
                CurrentScaleObject = scaleObjects?.FirstOrDefault();
            }
        }

        #endregion

        #region Methods => Misc.

        [ACMethodInfo("", "en{'Refresh material and lots'}de{'Refresh material and lots'}", 650, true)]
        public virtual void RefreshMaterialOrFC_F()
        {
            OnPropertyChanged(nameof(WeighingMaterialList));
            if (SelectedWeighingMaterial != null)
            {
                FacilityChargeList = null;
                FacilityChargeListCount = 0;
                FacilityChargeList = FillFacilityChargeList();
            }
        }

        public virtual bool IsEnabledRefreshMaterialOrFC_F()
        {
            return SelectedFacilityCharge == null || _CallPWLotChange;
        }

        protected virtual ScaleBackgroundState DetermineBackgroundState(double? tolPlus, double? tolMinus, double target, double actual)
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

                double act = Math.Round(actual, 5);

                if (act > target)
                {
                    if (act <= Math.Round(target + tolPlus.Value, 5))
                        return ScaleBackgroundState.InTolerance;
                    else
                        return ScaleBackgroundState.OutTolerance;
                }
                else
                {
                    if (act >= Math.Round(target - tolMinus.Value, 5))
                        return ScaleBackgroundState.InTolerance;
                }
            }
            return ScaleBackgroundState.Weighing;
        }

        public virtual ScaleBackgroundState? OnDetermineBackgroundState(double? tolPlus, double? tolMinus, double target, double actual)
        {
            if (_TareScaleState.ValueT != (short)PAFManualWeighing.TareScaleStateEnum.TareOK)
                return ScaleBackgroundState.Weighing;
            return null;
        }

        //private void SelectActiveScaleObject(WeighingMaterial weighingMaterial)
        //{
        //    List<ACValueItem> scaleObjectsList = null;
        //    using (ACMonitor.Lock(_70750_ProcessModuleScalesLock))
        //    {
        //        scaleObjectsList = ScaleObjectsList?.ToList();
        //    }

        //    if (scaleObjectsList != null && weighingMaterial.WeighingMatState == WeighingComponentState.InWeighing)
        //    {
        //        SelectActiveScaleObject(scaleObjectsList);
        //    }
        //    else
        //        OnPropertyChanged(nameof(CurrentScaleObject));
        //}

        //private void SelectActiveScaleObject(List<ACValueItem> scaleObjects, bool setIfNotSelected = false)
        //{
        //    if (scaleObjects == null)
        //        return;

        //    if (scaleObjects.Count > 1)
        //    {
        //        string activeScaleACUrl = CurrentPAFManualWeighing?.ExecuteMethod(nameof(PAFManualWeighing.GetActiveScaleObjectACUrl)) as string;
        //        if (!string.IsNullOrEmpty(activeScaleACUrl))
        //        {
        //            CurrentScaleObject = scaleObjects.FirstOrDefault(c => c.Value as string == activeScaleACUrl);
        //        }
        //        else if (setIfNotSelected)
        //        {
        //            CurrentScaleObject = scaleObjects.FirstOrDefault();
        //        }
        //    }
        //    else
        //    {
        //        CurrentScaleObject = scaleObjects.FirstOrDefault();
        //    }
        //}

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

            using (VD.DatabaseApp dbApp = new VD.DatabaseApp())
            {
                var lastUsedLotConfigs = dbApp.MaterialConfig.Where(c => c.KeyACUrl == PWManualWeighing.MaterialConfigLastUsedLotKeyACUrl
                                                                      && c.VBiACClassID == currentProcessModule.ComponentClass.ACClassID);

                List<VD.FacilityCharge> lastUsedLots = new List<VD.FacilityCharge>();

                foreach (VD.MaterialConfig lotConfig in lastUsedLotConfigs)
                {
                    Guid? fcID = lotConfig.Value as Guid?;
                    if (fcID.HasValue)
                    {
                        VD.FacilityCharge fc = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == fcID);
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

        [ACMethodInfo("", "en{'Remove last used lot suggestion'}de{'Zuletzt verwendeten Chargenvorschlag entfernen'}", 691, true)]
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

            VD.DatabaseApp dbApp = lastUsedLotConfig.GetObjectContext<VD.DatabaseApp>();
            if (dbApp != null)
                dbApp.Remove(lastUsedLotConfig);

            Msg msg = dbApp.ACSaveChanges();
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            LastUsedLotList.Remove(SelectedLastUsedLot);
            OnPropertyChanged(nameof(LastUsedLotList));
        }

        public bool IsEnabledRemoveLastUsedLot()
        {
            return SelectedLastUsedLot != null;
        }

        public ACMethod GetPAFCurrentACMethod()
        {
            ACMethod result = null;

            using (ACMonitor.Lock(_70700_PrivateMemberLock))
            {
                result = _PAFCurrentACMethod?.ValueT?.Clone() as ACMethod;
            }

            return result;
        }

        protected void CheckIsQuantStockNegAndInformUser(FacilityChargeItem facilityCharge)
        {
            if (facilityCharge == null)
                return;

            if (facilityCharge.StockQuantityUOM < 0)
            {
                //Info50084 : The quant quantity is negative, please check if you use right quant/lot. If current lot is consumed, please peform the Lot change task...
                Messages.Info(this, "Info50084");
            }
        }

        protected virtual ObservableCollection<FacilityChargeItem> FillFacilityChargeList()
        {
            try
            {
                return FillFacilityChargeListInternal();
            }
            catch (Exception e)
            {
                string message = null;
                if (e.InnerException != null)
                    message = string.Format("ManualWeighingModel(FacilityChargeList): {0}, {1} {2} {3}", e.Message, e.InnerException.Message, System.Environment.NewLine, e.StackTrace);
                else
                    message = string.Format("ManualWeighingModel(FacilityChargeList): {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace);

                Messages.LogError(this.GetACUrl(), nameof(FillFacilityChargeList), message);
            }

            try
            {
                return FillFacilityChargeListInternal();
            }
            catch (Exception e)
            {
                string message = null;
                if (e.InnerException != null)
                    message = string.Format("ManualWeighingModel(FacilityChargeList): {0}, {1} {2} {3}", e.Message, e.InnerException.Message, System.Environment.NewLine, e.StackTrace);
                else
                    message = string.Format("ManualWeighingModel(FacilityChargeList): {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace);

                Messages.LogError(this.GetACUrl(), nameof(FillFacilityChargeList), message);
                Messages.Error(this, "Load quants problem, please check the log file!");
            }
            return null;
        }

        protected virtual ObservableCollection<FacilityChargeItem> FillFacilityChargeListInternal()
        {
            IACComponentPWNode componentPWNode = ComponentPWNodeLocked;

            if (_FacilityChargeList == null && SelectedWeighingMaterial != null)
            {
                Guid posID = Guid.Empty;
                if (SelectedWeighingMaterial != null)
                {
                    if (SelectedWeighingMaterial.PosRelation != null)
                        posID = SelectedWeighingMaterial.PosRelation.ProdOrderPartslistPosRelationID;
                    else if (SelectedWeighingMaterial.PickingPosition != null)
                        posID = SelectedWeighingMaterial.PickingPosition.PickingPosID;
                }

                if (posID == Guid.Empty)
                    return _FacilityChargeList;

                ACValueList facilities = componentPWNode?.ExecuteMethod(nameof(PWManualWeighing.GetRoutableFacilities),
                                                                        posID) as ACValueList;

                var facilityIDs = facilities?.Select(c => c.ParamAsGuid).ToArray();
                if (facilityIDs == null)
                    return null;

                using (VD.DatabaseApp dbApp = new VD.DatabaseApp())
                {
                    //var facilitesDB = dbApp.Facility.Include(i => i.FacilityCharge_Facility).Where(c => facilityIDs.Contains(c.FacilityID));

                    if (_ACFacilityManager == null)
                    {
                        _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
                        if (_ACFacilityManager == null)
                        {
                            //Error50432: The facility manager is null.
                            Messages.Error(this, "Error50432");
                        }
                    }


                    if (SelectedWeighingMaterial != null)
                    {
                        if (SelectedWeighingMaterial.PosRelation != null)
                        {
                            Guid? materialID = SelectedWeighingMaterial?.PosRelation?.SourceProdOrderPartslistPos?.MaterialID;
                            if (materialID.HasValue)
                            {
                                IEnumerable<VD.FacilityCharge> quants = ACFacilityManager?.ManualWeighingFacilityChargeListQuery(dbApp, facilityIDs, materialID);
                                if (!ShowAllQuants)
                                {
                                    ACPartslistManager.QrySilosResult silosResult = new ACPartslistManager.QrySilosResult(quants);
                                    silosResult.ApplyLotReservationFilter(SelectedWeighingMaterial.PosRelation, 0);
                                    if (silosResult.HasLotReservations)
                                        quants = silosResult.FilteredResult.SelectMany(c => c.FacilityCharges.Where(p => p.IsReservedLot).Select(x => x.Quant));
                                }

                                _FacilityChargeList = new ObservableCollection<FacilityChargeItem>(quants.Select(s => new FacilityChargeItem(s, TargetWeight)));
                            }
                        }
                        else if (SelectedWeighingMaterial.PickingPosition != null)
                        {
                            Guid? materialID = SelectedWeighingMaterial?.PickingPosition?.Material?.MaterialID;
                            if (materialID.HasValue)
                            {
                                IEnumerable<VD.FacilityCharge> quants = ACFacilityManager?.ManualWeighingFacilityChargeListQuery(dbApp, facilityIDs, materialID);
                                if (!ShowAllQuants)
                                {
                                    ACPartslistManager.QrySilosResult silosResult = new ACPartslistManager.QrySilosResult(quants);
                                    silosResult.ApplyLotReservationFilter(SelectedWeighingMaterial.PickingPosition, 0);
                                    if (silosResult.HasLotReservations)
                                        quants = silosResult.FilteredResult.SelectMany(c => c.FacilityCharges.Where(p => p.IsReservedLot).Select(x => x.Quant));
                                }

                                _FacilityChargeList = new ObservableCollection<FacilityChargeItem>(quants.Select(s => new FacilityChargeItem(s, TargetWeight)));
                            }
                        }
                    }

                    if (_FacilityChargeList != null)
                        FacilityChargeListCount = _FacilityChargeList.Count();
                }
            }

            if (_FacilityChargeList == null)
                return null;

            return _FacilityChargeList;
        }

        private void CorrectManualWeighingItems()
        {
            var componentPWNode = ComponentPWNodeLocked;

            if (componentPWNode == null)
                return;

            if (WeighingMaterialList == null)
            {
                try
                {
                    WeighingMaterialList = GetWeighingMaterials(componentPWNode, DatabaseApp, DefaultMaterialIcon);
                    Messages.LogInfo(this.GetACUrl(), nameof(CorrectManualWeighingItems), "WeighingMaterialList is corrected!");
                }
                catch (Exception e)
                {
                    string message;
                    if (e.InnerException != null)
                        message = string.Format("ManualWeighingModel(Setup model): {0}, {1} {2} {3}", e.Message, e.InnerException.Message, System.Environment.NewLine, e.StackTrace);
                    else
                        message = string.Format("ManualWeighingModel(Setup model): {0} {1} {2}", e.Message, System.Environment.NewLine, e.StackTrace);

                    Messages.LogError(this.GetACUrl(), nameof(ActivateWFNode), message);
                    Messages.Error(this, message, true);

                    return;
                }
            }

            WeighingComponentInfo compInfo = _WeighingComponentInfo?.ValueT;
            if (compInfo != null)
            {
                if (SelectedWeighingMaterial == null)
                {
                    if (compInfo.PLPosRelation != Guid.Empty)
                    {
                        SelectedWeighingMaterial = WeighingMaterialList.FirstOrDefault(c => c.PosRelation != null && c.PosRelation.ProdOrderPartslistPosRelationID == compInfo.PLPosRelation);
                        SelectedWeighingMaterial.ChangeComponentState(WeighingComponentState.Selected, DatabaseApp);
                        Messages.LogInfo(this.GetACUrl(), nameof(CorrectManualWeighingItems), "SelectedWeighinMaterial is corrected!");
                    }
                    else if (compInfo.PickingPos != Guid.Empty)
                    {
                        SelectedWeighingMaterial = WeighingMaterialList.FirstOrDefault(c => c.PickingPosition != null && c.PickingPosition.PickingPosID == compInfo.PickingPos);
                        SelectedWeighingMaterial.ChangeComponentState(WeighingComponentState.Selected, DatabaseApp);
                        Messages.LogInfo(this.GetACUrl(), nameof(CorrectManualWeighingItems), "SelectedWeighinMaterial is corrected! (Picking)");
                    }
                }

                if (SelectedWeighingMaterial != null && SelectedFacilityCharge == null)
                {
                    if (FacilityChargeList == null)
                    {
                        RefreshMaterialOrFC_F();
                        Messages.LogInfo(this.GetACUrl(), nameof(CorrectManualWeighingItems), "FacilityChargeList is corrected!");
                    }

                    if (compInfo.FacilityCharge.HasValue)
                    {
                        SelectedFacilityCharge = FacilityChargeList.FirstOrDefault(c => c.FacilityChargeID == compInfo.FacilityCharge.Value);
                        Messages.LogInfo(this.GetACUrl(), nameof(CorrectManualWeighingItems), "SelectedFacilityCharge is corrected!");
                    }
                }
            }
        }

        #endregion

        #region Methods => SingleDosing

        [ACMethodInfo("", "en{'Single dosing'}de{'Einzeldosierung'}", 660, true)]
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

            using (Database db = new core.datamodel.Database())
            {
                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    RoutingService = this.RoutingService,
                    Database = db,
                    AttachRouteItemsToContext = true,
                    SelectionRuleID = PAMParkingspace.SelRuleID_ParkingSpace,
                    Direction = RouteDirections.Forwards,
                    MaxRouteAlternativesInLoop = ACRoutingService.DefaultAlternatives,
                    IncludeReserved = true,
                    IncludeAllocated = true
                };

                RoutingResult rResult = ACRoutingService.FindSuccessors(currentProcessModule.ComponentClass, routingParameters);

                if (rResult == null || rResult.Routes == null)
                {
                    //Error50431: Can not find any target storage for this station.
                    Messages.Error(this, "Error50431");
                    return;
                }

                SingleDosTargetStorageList = rResult.Routes.SelectMany(c => c.GetRouteTargets()).Select(x => x.Target);
            }

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

            SingleDosNumberOfRepetitions = 0;
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

        [ACMethodInfo("", "en{'Single dosing'}de{'Einzeldosierung'}", 661, true)]
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

            if (SingleDosTargetStorageList.Count() > 1)
            {
                ShowDialog(this, "TargetStorageDialog");
            }
            else
            {
                SelectedSingleDosTargetStorage = SingleDosTargetStorageList.FirstOrDefault();
            }

            if (SelectedSingleDosTargetStorage == null)
                return;

            using (Database db = new core.datamodel.Database())
            using (VD.DatabaseApp dbApp = new VD.DatabaseApp(db))
            {

                VD.Facility inwardFacility = dbApp.Facility.FirstOrDefault(c => c.VBiFacilityACClassID == SelectedSingleDosTargetStorage.ACClassID);

                if (inwardFacility == null)
                {
                    //Error50434: Can not find any facility according target storage ID: {0}
                    Messages.Error(this, "Error50434", false, SelectedSingleDosTargetStorage.ACClassID);
                    return;
                }

                VD.Material material = dbApp.Material.FirstOrDefault(c => c.MaterialNo == SelectedSingleDosingItem.MaterialNo);
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

                var wfConfigs = material.MaterialConfig_Material.Where(c => c.KeyACUrl == VD.MaterialConfig.PWMethodNodeConfigKeyACUrl);

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

                var workflow = wfConfig.VBiACClassWF.FromIPlusContext<ACClassWF>(db);
                var acClassMethod = workflow.ACClassMethod;

                CurrentBookParamRelocation.InwardFacility = inwardFacility;
                //CurrentBookParamRelocation.OutwardFacility = outwardFacility;
                CurrentBookParamRelocation.OutwardMaterial = material;
                CurrentBookParamRelocation.InwardQuantity = SingleDosTargetQuantity;
                CurrentBookParamRelocation.OutwardQuantity = SingleDosTargetQuantity;

                ACComponent processModule = CurrentProcessModule;

                bool runOnce = true;

                if (SingleDosNumberOfRepetitions > 1)
                {
                    int maxRep = MaxSingleDosNumOfRepetitions;
                    if (maxRep <= 0)
                        maxRep = 50;


                    if (SingleDosNumberOfRepetitions > maxRep)
                    {
                        runOnce = true;
                    }
                    else
                    {
                        //Question50083: The number of repetitions is set to {0}. Are you sure that you want dose {0} times?
                        if (Messages.Question(this, "Question50083", Global.MsgResult.No, false, SingleDosNumberOfRepetitions) == Global.MsgResult.Yes)
                        {
                            RunWorkflow(dbApp, workflow, acClassMethod, processModule, false, true, PARole.ValidationBehaviour.Strict, false);

                            for (int i = 1; i < SingleDosNumberOfRepetitions; i++)
                            {
                                RunWorkflow(dbApp, workflow, acClassMethod, processModule, false, true, PARole.ValidationBehaviour.Strict, true);
                            }

                            runOnce = false;
                        }
                        else
                        {
                            runOnce = true;
                        }
                    }
                }

                if (runOnce)
                {
                    RunWorkflow(dbApp, workflow, acClassMethod, processModule, false);
                }

                SingleDosTargetQuantity = 0;
                SingleDosNumberOfRepetitions = 0;
                SelectedSingleDosTargetStorage = null;
            }
            CloseTopDialog();
        }

        public bool IsEnabledSingleDosingStart()
        {
            return SelectedSingleDosingItem != null && SingleDosTargetQuantity > 0.0001;
        }

        [ACMethodInfo("", "en{'Select'}de{'Select'}", 661, true)]
        public virtual void SelectTargetStorage()
        {
            _TempSelectedSingleDosTargetStorage = SelectedSingleDosTargetStorage;
            this.CloseTopDialog();
            SelectedSingleDosTargetStorage = _TempSelectedSingleDosTargetStorage;
            _TempSelectedSingleDosTargetStorage = null;
        }

        public virtual MsgWithDetails ValidateSingleDosingStart(ACComponent currentProcessModule)
        {
            if (currentProcessModule != null)
            {
                using (Database db = new core.datamodel.Database())
                {
                    ACClass componentClass = currentProcessModule.ComponentClass?.FromIPlusContext<ACClass>(db);
                    if (componentClass == null)
                        return null;

                    double maxWeight = 0;
                    ACClassProperty maxWeightProp = componentClass.GetProperty(nameof(PAProcessModule.MaxWeightCapacity));
                    if (maxWeightProp != null && maxWeightProp.Value != null && maxWeightProp.Value is string)
                        maxWeight = (double)ACConvert.ChangeType(maxWeightProp.Value as string, typeof(double), true, db);

                    maxWeight = maxWeight - (maxWeight * 0.2);

                    if (SingleDosTargetQuantity > maxWeight)
                    {
                        //Error50487:The dosing quantity is {0} kg but the maximum dosing qunatity is {1} kg.
                        Msg msg = new Msg(this, eMsgLevel.Error, ClassName, nameof(ValidateSingleDosingStart), 2469, "Error50487", SingleDosTargetQuantity, Math.Round(maxWeight, 2));
                        return new MsgWithDetails(new Msg[] { msg });
                    }
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
                case nameof(SelectedFacilityCharge):
                    {
                        if (ShowSelectFacilityLotInfo && (!MultipleLotsSelectionRule.HasValue || FacilityChargeListCount < 2
                                                          || (MultipleLotsSelectionRule.Value != LotSelectionRuleEnum.OnlyScan)))
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
                case nameof(ShowAllQuants):
                    {
                        if (SelectedFacilityCharge != null)
                            return Global.ControlModes.Disabled;
                        return Global.ControlModes.Enabled;
                    }
            }

            return result;

        }

        #region Methods => AbortDialog

        [ACMethodInfo("", "en{'Yes -> Abort'}de{'Ja -> Abbrechen'}", 695, true)]
        public void AbortComponent()
        {
            _AbortMode = AbortModeEnum.AbortComponent;
            InInterdischargingQ = null;
            _IsEnabledCompleteInterdischarging = false;
            CloseTopDialog();
        }

        [ACMethodInfo("", "en{'Abort and scale other comp.'}de{'Abbrechen und Restliche Komp. anpassen'}", 695, true)]
        public void AbortComponentScaleOther()
        {
            _AbortMode = AbortModeEnum.AbortComponentScaleOtherComponents;
            InInterdischargingQ = null;
            _IsEnabledCompleteInterdischarging = false;
            CloseTopDialog();
        }

        public bool IsEnabledAbortComponentScaleOther()
        {
            return ScaleOtherComponentOnAbort;
        }

        [ACMethodInfo("", "en{'Abort and emptying mode'}de{'Abbrechen und Leerfahren'}", 695, true)]
        public void AbortComponentEmptyingMode()
        {
            _AbortMode = AbortModeEnum.AbortComponentSwitchToEmptyingMode;
            InInterdischargingQ = null;
            _IsEnabledCompleteInterdischarging = false;
            CloseTopDialog();
        }

        [ACMethodInfo("", "en{'Switch to emptying mode'}de{'Leerfahrmodus aktivieren'}", 695, true)]
        public void SwitchEmptyingMode()
        {
            _AbortMode = AbortModeEnum.SwitchToEmptyingMode;
            CloseTopDialog();
        }

        [ACMethodInfo("", "en{'Close'}de{'Schließen'}", 696, true)]
        public void CloseAbortDialog()
        {
            CloseTopDialog();
        }

        public bool IsEnabledCloseAbortDialog()
        {
            return InInterdischargingQ.HasValue ? false : true;
        }

        [ACMethodInfo("", "en{'Interdischarge'}de{'Ja -> Zwischenentleeren'}", 696, true)]
        public virtual void Interdischarge()
        {
            IACComponentPWNode currentPWNode = ComponentPWNodeLocked;
            InterdischargeStart(currentPWNode, false);
        }

        public virtual bool IsEnabledInterdischarge()
        {
            return InInterdischargingQ.HasValue ? false : true;
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
                    _IsEnabledCompleteInterdischarging = false;

                    VerifyIsActualQLower(_ScaleActualValue.ValueT);
                }
            }
        }

        protected void ScaleActualValue_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        [ACMethodInfo("", "en{'Interdischarging complete'}de{'Interdischarging complete'}", 696, true)]
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

        #region Methods => Rework

        private void InitRework(ACComponent currentProcessModule)
        {
            ACClass processModuleClass = null;

            using (Database db = new core.datamodel.Database())
            {
                processModuleClass = currentProcessModule?.ComponentClass.FromIPlusContext<ACClass>(db);

                bool reworkEnabled = false;

                var config = processModuleClass?.ConfigurationEntries.FirstOrDefault(c => c.KeyACUrl == processModuleClass.ACConfigKeyACUrl && c.LocalConfigACUrl == nameof(PAProcessModuleVB.ReworkEnabled));
                if (config != null)
                {
                    bool? val = config.Value as bool?;
                    if (val.HasValue)
                        reworkEnabled = val.Value;

                    IsReworkEnabled = reworkEnabled;
                }
                else
                {
                    //Messages.Error(this, "Can not find the configuration property ReworkEnabled on the process module!");
                }
            }
        }

        [ACMethodInfo("", "en{'Rework'}de{'Nacharbeit'}", 700, true)]
        public void OpenReworkDialog()
        {
            ACRef<IACComponentPWNode> pwNode;

            using (ACMonitor.Lock(_70500_ComponentPWNodeLock))
            {
                pwNode = ComponentPWNodesList?.FirstOrDefault()?.ComponentPWNode;
            }

            if (pwNode == null)
                return;

            SelectedReworkInfo = null;
            IACComponentPWNode compPWNode = pwNode.ValueT;
            ReworkInfoItems = compPWNode.ExecuteMethod(nameof(PWManualWeighing.GetReworkStatus)) as ReworkInfoList;

            ShowDialog(this, "ReworkDialog");
        }

        public bool IsEnabledOpenReworkDialog()
        {
            return IsReworkEnabled;
        }

        [ACMethodInfo("", "en{'Add rework material'}de{'Rework-Material hinzufügen'}", 701, true)]
        public void AddReworkMaterial()
        {
            ManualWeighingPWNode pwNode = null;

            using (ACMonitor.Lock(_70500_ComponentPWNodeLock))
            {
                pwNode = ComponentPWNodesList?.FirstOrDefault();
            }

            if (pwNode == null)
                return;

            ReworkInfoItems = pwNode.ComponentPWNode.ValueT.ExecuteMethod(nameof(PWManualWeighing.ActivateRework)) as ReworkInfoList;
        }

        public bool IsEnabledAddReworkMaterial()
        {
            return true;
        }

        #endregion

        #region Methods => Print

        [ACMethodInfo("", "en{'Print last quant'}de{'Letztes Quant drucken'}", 9999, true)]
        public virtual void PrintLastQuant()
        {
            var currentProcessModule = CurrentProcessModule;

            if (currentProcessModule == null)
                return;

            Guid acClassID;

            using (ACMonitor.Lock(core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
                acClassID = currentProcessModule.ComponentClass.ACClassID;

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

            MsgWithDetails msg = ACFacilityManager.PrintLastQuant(currentProcessModule.ACUrl, acClassID);
            if (msg != null)
            {
                Messages.Msg(msg);
            }
        }

        public bool IsEnabledPrintLastQuant()
        {
            return !IsCurrentProcessModuleNull;
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
                case nameof(OpenReworkDialog):
                    OpenReworkDialog();
                    return true;
                case nameof(IsEnabledOpenReworkDialog):
                    result = IsEnabledOpenReworkDialog();
                    return true;
                case nameof(AddReworkMaterial):
                    AddReworkMaterial();
                    return true;
                case nameof(IsEnabledAddReworkMaterial):
                    result = IsEnabledAddReworkMaterial();
                    return true;
                case nameof(ConvertWeightToUIText):
                    result = ConvertWeightToUIText((double)acParameter[0]);
                    return true;
                case nameof(PrintLastQuant):
                    PrintLastQuant();
                    return true;
                case nameof(IsEnabledPrintLastQuant):
                    result = IsEnabledPrintLastQuant();
                    return true;
                case nameof(IsEnabledAbortComponentScaleOther):
                    result = IsEnabledAbortComponentScaleOther();
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
