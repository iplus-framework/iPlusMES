using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.core.processapplication;
using System.Xml;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Silo'}de{'Silo'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMSilo : PAProcessModule
    {
        #region Const
        protected enum VSSInvoker
        {
            Unknown,
            SensorState,
            ActualValue,
            OnRefreshFacility,
            AcknowledgeAlarms
        }

        public const string SelRuleID_Silo = "PAMSilo";
        public const string SelRuleID_Storage = "Storage";
        public const string SelRuleID_Silo_Deselector = "PAMSilo.Deselector";
        public const string SelRuleID_DosingFunc = "PAMSilo.DosingFunc";
        public const string SelRuleID_DischargingFunc = "PAMSilo.DischargingFunc";
        #endregion


        #region c'tors
        static PAMSilo()
        {
            RegisterExecuteHandler(typeof(PAMSilo), HandleExecuteACMethod_PAMSilo);
            ACRoutingService.RegisterSelectionQuery(SelRuleID_Silo, (c, p) => c.Component.ValueT is PAMSilo, null);
            ACRoutingService.RegisterSelectionQuery(SelRuleID_Storage, (c, p) => c.Component.ValueT is PAMSilo || c.Component.ValueT is PAMParkingspace || c.Component.ValueT is PAMIntermediatebin, null);
            ACRoutingService.RegisterSelectionQuery(SelRuleID_Silo_Deselector, null, (c, p) => c.Component.ValueT is PAMSilo || c.Component.ValueT is PAMParkingspace);
            ACRoutingService.RegisterSelectionQuery(SelRuleID_DosingFunc,
                                                    (c, p) => c.Component.ValueT is PAProcessModuleVB
                                                                && !(c.Component.ValueT is PAMSilo)
                                                                && !(c.Component.ValueT is PAMParkingspace)
                                                                && c.Component.ValueT.FindChildComponents<PAFDosing>(d => d is PAFDosing, null, 1).Any(),
                                                    (c, p) => c.Component.ValueT is PAMSilo || c.Component.ValueT is PAMParkingspace || c.Component.ValueT is PAMIntermediatebin);
            ACRoutingService.RegisterSelectionQuery(SelRuleID_DischargingFunc,
                                                    (c, p) => c.Component.ValueT is PAProcessModuleVB
                                                                && !(c.Component.ValueT is PAMSilo)
                                                                && !(c.Component.ValueT is PAMParkingspace)
                                                                && c.Component.ValueT.FindChildComponents<PAFDischarging>(d => d is PAFDischarging, null, 1).Any(),
                                                    (c, p) => c.Component.ValueT is PAMSilo || c.Component.ValueT is PAMParkingspace || c.Component.ValueT is PAMIntermediatebin);
        }


        public PAMSilo(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _PAPointMatIn1 = new PAPoint(this, Const.PAPointMatIn1);
            _PAPointMatOut1 = new PAPoint(this, Const.PAPointMatOut1);
        }


        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            DatabaseApp appDB = Database as DatabaseApp;
            if (appDB != null)
            {
                gip.mes.datamodel.ACClass thisACClass = ComponentClass.FromAppContext<gip.mes.datamodel.ACClass>(appDB);
                if (thisACClass != null)
                {
                    Facility facilitySilo = thisACClass.Facility_VBiFacilityACClass.FirstOrDefault();
                    Facility.ValueT = new ACRef<Facility>(facilitySilo, this);
                    //RefreshFacility();
                }
            }

            return true;
        }


        public override bool ACPreDeInit(bool deleteACClassTask = false)
        {
            if (MatSensorFilling != null)
                (MatSensorFilling.SensorState as IACPropertyNetTarget).ValueUpdatedOnReceival -= ChildProperties_ValueUpdatedOnReceival;
            if (MatSensorEmtpy != null)
                (MatSensorEmtpy.SensorState as IACPropertyNetTarget).ValueUpdatedOnReceival -= ChildProperties_ValueUpdatedOnReceival;
            if (MatSensorFull != null)
                (MatSensorFull.SensorState as IACPropertyNetTarget).ValueUpdatedOnReceival -= ChildProperties_ValueUpdatedOnReceival;
            if (ScaleSensor != null)
                (ScaleSensor.ActualValue as IACPropertyNetTarget).ValueUpdatedOnReceival -= ChildProperties_ValueUpdatedOnReceival;
            UnSubscribeAllTransportFunctions();
            return base.ACPreDeInit(deleteACClassTask);
        }


        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_MatSensorFilling != null)
            {
                _MatSensorFilling.Detach();
                _MatSensorFilling = null;
            }
            _MatSensorFillingChecked = false;

            if (_MatSensorEmtpy != null)
            {
                _MatSensorEmtpy.Detach();
                _MatSensorEmtpy = null;
            }
            _MatSensorEmtpyChecked = false;

            if (_MatSensorFull != null)
            {
                _MatSensorFull.Detach();
                _MatSensorFull = null;
            }
            _MatSensorFullChecked = false;

            if (_ScaleSensor != null)
            {
                _ScaleSensor.Detach();
                _ScaleSensor = null;
            }
            _ScaleSensorChecked = false;

            return base.ACDeInit(deleteACClassTask);
        }


        public override bool ACPostInit()
        {
            if (MatSensorFilling != null)
                (MatSensorFilling.SensorState as IACPropertyNetTarget).ValueUpdatedOnReceival += ChildProperties_ValueUpdatedOnReceival;
            if (MatSensorEmtpy != null)
                (MatSensorEmtpy.SensorState as IACPropertyNetTarget).ValueUpdatedOnReceival += ChildProperties_ValueUpdatedOnReceival;
            if (MatSensorFull != null)
                (MatSensorFull.SensorState as IACPropertyNetTarget).ValueUpdatedOnReceival += ChildProperties_ValueUpdatedOnReceival;
            if (ScaleSensor != null)
                (ScaleSensor.ActualValue as IACPropertyNetTarget).ValueUpdatedOnReceival += ChildProperties_ValueUpdatedOnReceival;

            bool result =  base.ACPostInit();

            //CurrentTransportFunction = null;

            RefreshFacility();
            return result;
        }
        #endregion


        #region Points
        PAPoint _PAPointMatIn1;
        [ACPropertyConnectionPoint(9999, "PointMaterial")]
        public PAPoint PAPointMatIn1
        {
            get
            {
                return _PAPointMatIn1;
            }
        }

        PAPoint _PAPointMatOut1;
        [ACPropertyConnectionPoint(9999, "PointMaterial")]
        public PAPoint PAPointMatOut1
        {
            get
            {
                return _PAPointMatOut1;
            }
        }
        #endregion


        #region References to Objects
        public IEnumerable<PAESensorDigital> MaterialSensors
        {
            get
            {
                return this.FindChildComponents<PAESensorDigital>(c => c is PAESensorDigital, null, 1);
            }
        }

        private bool _MatSensorFillingChecked = false;
        private ACRef<PAESensorDigital> _MatSensorFilling = null;
        public PAESensorDigital MatSensorFilling
        {
            get
            {
                if (_MatSensorFilling != null)
                    return _MatSensorFilling.ValueT;
                else if (_MatSensorFillingChecked)
                    return null;
                _MatSensorFillingChecked = true;
                var sensor = MaterialSensors.Where(c => c.ACIdentifier == "FM").FirstOrDefault();
                if (sensor != null)
                {
                    _MatSensorFilling = new ACRef<PAESensorDigital>(sensor, this);
                    return sensor;
                }
                return null;
            }
        }

        private bool _MatSensorEmtpyChecked = false;
        private ACRef<PAESensorDigital> _MatSensorEmtpy = null;
        public PAESensorDigital MatSensorEmtpy
        {
            get
            {
                if (_MatSensorEmtpy != null)
                    return _MatSensorEmtpy.ValueT;
                else if (_MatSensorEmtpyChecked)
                    return null;
                _MatSensorEmtpyChecked = true;
                var sensor = MaterialSensors.Where(c => c.ACIdentifier == "LM").FirstOrDefault();
                if (sensor != null)
                {
                    _MatSensorEmtpy = new ACRef<PAESensorDigital>(sensor, this);
                    return sensor;
                }
                return null;
            }
        }

        private bool _MatSensorFullChecked = false;
        private ACRef<PAESensorDigital> _MatSensorFull = null;
        public PAESensorDigital MatSensorFull
        {
            get
            {
                if (_MatSensorFull != null)
                    return _MatSensorFull.ValueT;
                else if (_MatSensorFullChecked)
                    return null;
                _MatSensorFullChecked = true;
                var sensor = MaterialSensors.Where(c => c.ACIdentifier == "VM").FirstOrDefault();
                if (sensor != null)
                {
                    _MatSensorFull = new ACRef<PAESensorDigital>(sensor, this);
                    return sensor;
                }
                return null;
            }
        }

        private bool _ScaleSensorChecked = false;
        private ACRef<PAEScaleGravimetric> _ScaleSensor = null;
        public PAEScaleGravimetric ScaleSensor
        {
            get
            {
                if (_ScaleSensor != null)
                    return _ScaleSensor.ValueT;
                else if (_ScaleSensorChecked)
                    return null;
                _ScaleSensorChecked = true;
                var sensor = this.FindChildComponents<PAEScaleGravimetric>(c => c is PAEScaleGravimetric, null, 1).FirstOrDefault();
                if (sensor != null)
                {
                    _ScaleSensor = new ACRef<PAEScaleGravimetric>(sensor, this);
                    return sensor;
                }
                return null;
            }
        }

        private List<IPAFuncTransportMaterial> _CurrentTransportFunctions = new List<IPAFuncTransportMaterial>();
        public IEnumerable<IPAFuncTransportMaterial> CurrentTransportFunctions
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentTransportFunctions.ToArray();
                }
            }
        }

        public IEnumerable<IPAFuncReceiveMaterial> CurrentDosingFunctions
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentTransportFunctions
                        .Where(c => c is IPAFuncReceiveMaterial)
                        .Select(c => c as IPAFuncReceiveMaterial).ToArray();
                }
            }
        }

        public IEnumerable<IPAFuncDeliverMaterial> CurrentDischargingFunctions
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentTransportFunctions
                        .Where(c => c is IPAFuncDeliverMaterial)
                        .Select(c => c as IPAFuncDeliverMaterial).ToArray();
                }
            }
        }
        
        
        public IEnumerable<IPAFuncScaleConfig> CurrentScaleFunctions
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    return _CurrentTransportFunctions
                        .Where(c => c is IPAFuncScaleConfig)
                        .Select(c => c as IPAFuncScaleConfig).ToArray();
                }
            }
        }        
        #endregion


        #region Properties, Range 400
        [ACPropertyBindingSource(401, "Configuration", "en{'Facility'}de{'Lagerplatz'}", "", true, false)]
        public IACContainerTNet<ACRef<Facility>> Facility { get; set; }

        [ACPropertyBindingTarget(441, "Read from PLC", "en{'Fill level'}de{'Füllstand'}", "", false, false, RemotePropID=20)]
        public IACContainerTNet<Double> FillLevel { get; set; }

        [ACPropertyBindingTarget(442, "Read from PLC", "en{'Materialname'}de{'Materialname'}", "", false, false, RemotePropID=21)]
        public IACContainerTNet<String> MaterialName { get; set; }

        [ACPropertyBindingTarget(443, "Read from PLC", "en{'Inward enabled'}de{'Einlagerung freigegeben'}", "", false, false, RemotePropID=22)]
        public IACContainerTNet<Boolean> InwardEnabled { get; set; }

        [ACPropertyBindingTarget(444, "Read from PLC", "en{'Outward enabled'}de{'Auslagerung freigegeben'}", "", false, false, RemotePropID=23)]
        public IACContainerTNet<Boolean> OutwardEnabled { get; set; }

        [ACPropertyBindingTarget(445, "Read from PLC", "en{'Materialno'}de{'Material-Nr.'}", "", false, false, RemotePropID=24)]
        public IACContainerTNet<String> MaterialNo { get; set; }

        [ACPropertyBindingTarget(446, "Read from PLC", "en{'Dosing learning mode'}de{'Dosierung Lernmodus'}", "", false, true, RemotePropID=25)]
        public IACContainerTNet<Boolean> LearnDosing { get; set; }

        [ACPropertyBindingTarget(447, "Read from PLC", "en{'Has hint'}de{'Hat Hinweis'}", "", false, false)]
        public IACContainerTNet<Boolean> HasHint { get; set; }

        [ACPropertyBindingTarget(448, "Read from PLC", "en{'Hint'}de{'Hinweis'}", "", false, false)]
        public IACContainerTNet<String> Hint { get; set; }

        [ACPropertyBindingTarget(449, "Read from PLC", "en{'Fill level measured'}de{'Füllstand gemessen'}", "", false, false)]
        public IACContainerTNet<Double> FillLevelScale { get; set; }

        public bool HasBoundFillLevel
        {
            get
            {
                var propTarget = this.FillLevel as IACPropertyNetTarget;
                if (propTarget == null)
                    return false;
                return propTarget.Source != null;
            }
        }

        public bool HasBoundFillLevelScale
        {
            get
            {
                var propTarget = this.FillLevelScale as IACPropertyNetTarget;
                if (propTarget == null)
                    return false;
                return propTarget.Source != null;
            }
        }

        [ACPropertyBindingSource(450, "Error", "en{'Validation error'}de{'Validierungsfehler'}", "", false, false)]
        public IACContainerTNet<PANotifyState> ValidationError { get; set; }
        public const string PropNameValidationError = "ValidationError";

        protected override bool IsDisplayingOrderInfo
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsSiloFullFromSensorStates
        {
            get
            {
                if (MatSensorFull != null && MatSensorFull.SensorState != null && MatSensorFull.SensorState.ValueT != PANotifyState.Off)
                    return true;
                if (ScaleSensor != null && this.MaxWeightCapacity.ValueT > 0.0001 && ScaleSensor.ActualValue.ValueT > this.MaxWeightCapacity.ValueT)
                    return true;
                return false;
            }
        }

        protected double _LastStock = 0;
        protected double _CurrentStock = 0;
        public double CurrentStock
        {
            get
            {
                return _CurrentStock;
            }
        }

        public bool IsSimulationOn
        {
            get
            {
                if (ACOperationMode != ACOperationModes.Live)
                    return true;
                if (ApplicationManager == null)
                    return false;
                return ApplicationManager.IsSimulationOn;
            }
        }


        #endregion


        #region Methods

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "RefreshFacility":
                    RefreshFacility();
                    return true;
                case "RebuildCurrentTransportFunctions":
                    RebuildCurrentTransportFunctions();
                    return true;
                case Const.IsEnabledPrefix + "RebuildCurrentTransportFunctions":
                    result = IsEnabledRebuildCurrentTransportFunctions();
                    return true;
                case "GetBSONameForShowBooking":
                    result = GetBSONameForShowBooking(acParameter[0] as string);
                    return true;
                case "GetBSONameForShowOverview":
                    result = GetBSONameForShowOverview(acParameter[0] as string);
                    return true;
                case "GetBSONameForShowReservation":
                    result = GetBSONameForShowReservation(acParameter[0] as string);
                    return true;
                case "IsDischargingToThisSiloActive":
                    result = IsDischargingToThisSiloActive();
                    return true;
                case "IsDosingActiveFromThisSilo":
                    result = IsDosingActiveFromThisSilo();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PAMSilo(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "FacilityBookingDialogOn":
                    FacilityBookingDialogOn(acComponent);
                    return true;
                case "ShowFacilityOverview":
                    ShowFacilityOverview(acComponent);
                    return true;
                case "ShowReservationDialog":
                    ShowReservationDialog(acComponent);
                    return true;
                case "WorkflowDialogOn":
                    WorkflowDialogOn(acComponent);
                    return true;
                case Const.IsEnabledPrefix + "FacilityBookingDialogOn":
                    result = IsEnabledFacilityBookingDialogOn(acComponent);
                    return true;
                case Const.IsEnabledPrefix + "ShowFacilityOverview":
                    result = IsEnabledShowFacilityOverview(acComponent);
                    return true;
                case Const.IsEnabledPrefix + "ShowReservationDialog":
                    result = IsEnabledShowReservationDialog(acComponent);
                    return true;
            }
            return HandleExecuteACMethod_PAProcessModule(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        #region Cyclic/Event methods
        private void ChildProperties_ValueUpdatedOnReceival(object sender, ACPropertyChangedEventArgs e, ACPropertyChangedPhase phase)
        {
            if (phase == ACPropertyChangedPhase.BeforeBroadcast)
                return;
            if (e.PropertyName == "SensorState")
            {
                ValidateSensorState(VSSInvoker.SensorState);
            }
            else if (e.PropertyName == "ActualValue")
            {
                RecalcFillLevelScale();
                ValidateSensorState(VSSInvoker.ActualValue);
            }
        }

        private void ActualWeight_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RecalcFillLevelScale();
        }
        #endregion


        #region Public Network Methods
        public override void Reset()
        {
            UnSubscribeAllTransportFunctions(true);
            base.Reset();
        }

        [ACMethodInfo("", "en{'Refresh Facility'}de{'Aktualisiere Lagerplatz'}", 400, true)]
        public virtual void RefreshFacility()
        {
            if (Facility.ValueT == null || Facility.ValueT.ValueT == null)
                return;
            Facility facilitySilo = Facility.ValueT.ValueT;
            if (facilitySilo != null)
            {
                try
                {
                    RootDbOpQueue.AppContextQueue.ProcessAction(() =>
                    {
                        try
                        {
                            facilitySilo.AutoRefresh();
                        }
                        catch (Exception qEx)
                        {
                            Messages.LogException(this.GetACUrl(), "RefreshFacility(2)", qEx.Message);
                        }
                    });
                    if (Math.Abs((Double)(this.MaxVolumeCapacity.ValueT - (Double)facilitySilo.MaxVolumeCapacity)) > Double.Epsilon)
                    {
                        this.MaxVolumeCapacity.ValueT = facilitySilo.MaxVolumeCapacity;
                    }
                    if (Math.Abs((Double)(this.MaxWeightCapacity.ValueT - (Double)facilitySilo.MaxWeightCapacity)) > Double.Epsilon)
                    {
                        this.MaxWeightCapacity.ValueT = facilitySilo.MaxWeightCapacity;
                    }

                    OnBuildMaterialInfo(facilitySilo);

                    bool informDischargings = InwardEnabled.ValueT != facilitySilo.InwardEnabled;
                    InwardEnabled.ValueT = facilitySilo.InwardEnabled;
                    bool informDosings = OutwardEnabled.ValueT != facilitySilo.OutwardEnabled;
                    OutwardEnabled.ValueT = facilitySilo.OutwardEnabled;
                    OnRefreshFacility(facilitySilo);
                    if (informDosings && this.Root.Initialized)
                    {
                        IEnumerable<PAFDosing> dosingList = GetActiveDosingsFromThisSilo();
                        if (dosingList != null && dosingList.Any())
                        {
                            foreach (PAFDosing pafDosing in dosingList)
                            {
                                if (pafDosing.CurrentACState != ACStateEnum.SMIdle)
                                    pafDosing.OnSiloStateChanged(this, OutwardEnabled.ValueT);
                            }
                        }
                    }
                    if (informDischargings && this.Root.Initialized)
                    {
                        IEnumerable<PAFDischarging> dischargingList = GetActiveDischargingsToThisSilo();
                        if (dischargingList != null && dischargingList.Any())
                        {
                            foreach (PAFDischarging pafDischarging in dischargingList)
                            {
                                if (pafDischarging.CurrentACState != ACStateEnum.SMIdle)
                                    pafDischarging.OnSiloStateChanged(this, InwardEnabled.ValueT);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Messages.LogException(this.GetACUrl(), "RefreshFacility(1)", e.Message);
                }
            }
        }


        [ACMethodInteraction("", "en{'Refresh active transport functions'}de{'Aktive Transportfunktionen aktualisieren'}", 401, true, "", Global.ACKinds.MSMethod, false, Global.ContextMenuCategory.ProcessCommands)]
        public void RebuildCurrentTransportFunctions()
        {
            if (this.ApplicationManager == null)
                return;
            if (this.ApplicationManager.ApplicationQueue != null)
            {
                this.ApplicationManager.ApplicationQueue.Add(() => 
                {
                    UnSubscribeAllTransportFunctions();
                    List<PAFDosing> dosings = RebuildDosingsFromThisSilo();
                    if (dosings != null && dosings.Any())
                    {
                        foreach (var dosing in dosings)
                        {
                            SubscribeTransportFunction(dosing);
                        }
                    }
                    List<PAFDischarging> dischargings = RebuildDischargingsToThisSilo();
                    if (dischargings != null && dischargings.Any())
                    {
                        foreach (var discharging in dischargings)
                        {
                            SubscribeTransportFunction(discharging);
                        }
                    }
                });
            }
        }

        public virtual bool IsEnabledRebuildCurrentTransportFunctions()
        {
            return true;
            //return true;
        }


        [ACMethodInfo("", "", 9999)]
        public virtual string GetBSONameForShowBooking(string defaultBSOName)
        {
            return defaultBSOName;
        }


        [ACMethodInfo("", "", 9999)]
        public virtual string GetBSONameForShowOverview(string defaultBSOName)
        {
            return defaultBSOName;
        }


        [ACMethodInfo("", "", 9999)]
        public virtual string GetBSONameForShowReservation(string defaultBSOName)
        {
            return defaultBSOName;
        }


        [ACMethodInfo("", "en{'Is Discharging active to this Silo'}de{'Entleerung zu diesem Silo aktiv'}", 401, true)]
        public virtual string[] IsDischargingToThisSiloActive()
        {
            IEnumerable<PAFDischarging> activeDischargings = GetActiveDischargingsToThisSilo();
            if (activeDischargings == null)
                return null;
            return activeDischargings.Select(c => c.GetACUrl()).ToArray();
        }


        [ACMethodInfo("", "en{'Is Dosing active from this Silo'}de{'Dosierung aus diesem Silo aktiv'}", 402, true)]
        public virtual string[] IsDosingActiveFromThisSilo()
        {
            IEnumerable<PAFDosing> activeDosings = GetActiveDosingsFromThisSilo();
            if (activeDosings == null)
                return null;
            return activeDosings.Select(c => c.GetACUrl()).ToArray();
        }
        #endregion


        #region Public Methods
        public void SubscribeTransportFunction(IPAFuncTransportMaterial paFunction)
        {
            if (paFunction == null)
                return;
            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (_CurrentTransportFunctions.Where(c => c == paFunction).Any())
                    return;
                IPAFuncScaleConfig configScale = paFunction as IPAFuncScaleConfig;
                PAEScaleBase scaleForWeighing = configScale?.CurrentScaleForWeighing;
                if (scaleForWeighing != null)
                {
                    if (!_CurrentTransportFunctions.Where(c => c is IPAFuncScaleConfig && (c as IPAFuncScaleConfig).CurrentScaleForWeighing == scaleForWeighing).Any())
                    {
                        PAEScaleTotalizing totalizingScale = scaleForWeighing as PAEScaleTotalizing;
                        if (totalizingScale != null)
                            totalizingScale.TotalActualWeight.PropertyChanged += ActualWeight_PropertyChanged;
                        else
                            scaleForWeighing.ActualWeight.PropertyChanged += ActualWeight_PropertyChanged;
                    }
                }
                _CurrentTransportFunctions.Add(paFunction);
                OnCurrentTransportFunctionChanged(paFunction, true);
            }
            RecalcFillLevelScale();
        }


        public void UnSubscribeTransportFunction(IPAFuncTransportMaterial paFunction)
        {
            if (paFunction == null)
                return;
            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (!_CurrentTransportFunctions.Where(c => c == paFunction).Any())
                    return;
                _CurrentTransportFunctions.Remove(paFunction);
                IPAFuncScaleConfig configScale = paFunction as IPAFuncScaleConfig;
                PAEScaleBase scaleForWeighing = configScale?.CurrentScaleForWeighing;
                if (scaleForWeighing != null)
                {
                    if (!_CurrentTransportFunctions.Where(c => c is IPAFuncScaleConfig && (c as IPAFuncScaleConfig).CurrentScaleForWeighing == scaleForWeighing).Any())
                    {
                        PAEScaleTotalizing totalizingScale = scaleForWeighing as PAEScaleTotalizing;
                        if (totalizingScale != null)
                            totalizingScale.TotalActualWeight.PropertyChanged -= ActualWeight_PropertyChanged;
                        else
                            scaleForWeighing.ActualWeight.PropertyChanged -= ActualWeight_PropertyChanged;
                    }
                }
                OnCurrentTransportFunctionChanged(paFunction, false);
            }
            RecalcFillLevelScale();
        }


        public void UnSubscribeAllTransportFunctions(bool refreshFillLevel = false)
        {
            using (ACMonitor.Lock(_20015_LockValue))
            {
                foreach (IPAFuncTransportMaterial paFunction in _CurrentTransportFunctions.ToArray())
                {
                    IPAFuncScaleConfig configScale = paFunction as IPAFuncScaleConfig;
                    PAEScaleBase scaleForWeighing = configScale?.CurrentScaleForWeighing;
                    if (scaleForWeighing != null)
                    {
                        PAEScaleTotalizing totalizingScale = scaleForWeighing as PAEScaleTotalizing;
                        if (totalizingScale != null)
                            totalizingScale.TotalActualWeight.PropertyChanged -= ActualWeight_PropertyChanged;
                        else
                            scaleForWeighing.ActualWeight.PropertyChanged -= ActualWeight_PropertyChanged;
                    }
                    _CurrentTransportFunctions.Remove(paFunction);
                    OnCurrentTransportFunctionChanged(paFunction, false);
                }
            }
            if (refreshFillLevel)
                RecalcFillLevelScale();
        }
        #endregion


        #region Virtual Methods
        protected virtual void OnBuildMaterialInfo(Facility facilitySilo)
        {
            Material material = null;
            RootDbOpQueue.AppContextQueue.ProcessAction(() =>                
            {
                try
                {
                    material = facilitySilo.Material;
                }
                catch (Exception qEx)
                {
                    Messages.LogException(this.GetACUrl(), "OnBuildMaterialInfo(1)", qEx.Message);
                }
            });
            if (material != null)
            {
                MaterialName.ValueT = material.MaterialName1;
                MaterialNo.ValueT = material.MaterialNo;
            }
            else
            {
                MaterialName.ValueT = null;
                MaterialNo.ValueT = null;
            }
        }

        protected virtual void OnRefreshFacility(Facility facilitySilo)
        {
            var propTarget = this.FillLevel as IACPropertyNetTarget;

            double? stockQ = null;
            //IACEntityObjectContext context = facilitySilo.GetObjectContext();
            //if (context != null)
            RootDbOpQueue.AppContextQueue.ProcessAction(() =>
            {
                try
                {
                    if (facilitySilo.CurrentFacilityStock == null)
                    {
                        facilitySilo.FacilityStock_Facility.AutoLoad();
                        if (facilitySilo.CurrentFacilityStock != null)
                        {
                            stockQ = facilitySilo.CurrentFacilityStock.StockQuantity;
                        }
                    }
                    else if (facilitySilo.CurrentFacilityStock != null)
                    {
                        FacilityStock facilityStock = facilitySilo.CurrentFacilityStock;
                        facilityStock.AutoRefresh();
                        stockQ = facilityStock.StockQuantity;
                    }
                }
                catch (Exception qEx)
                {
                    Messages.LogException(this.GetACUrl(), "OnRefreshFacility(1)", qEx.Message);
                }
            });

            _CurrentStock = stockQ.HasValue ? stockQ.Value : 0;
            // Falls Füllstand gebunden an eine Source-Variable, dann aktualiere den Füllstand nicht vom Bestand
            if (propTarget != null && propTarget.Source == null)
                this.FillLevel.ValueT = _CurrentStock;
            if (String.IsNullOrWhiteSpace(facilitySilo.Comment))
            {
                Hint.ValueT = "";
                HasHint.ValueT = false;
            }
            else
            {
                Hint.ValueT = facilitySilo.Comment;
                HasHint.ValueT = true;
            }
            RecalcFillLevelScale();
            ValidateSensorState(VSSInvoker.OnRefreshFacility, facilitySilo);
            _LastStock = _CurrentStock;
        }

        protected virtual void RecalcFillLevelScale()
        {
            if (!HasBoundFillLevel && !HasBoundFillLevelScale)
            {
                if (this.ScaleSensor != null)
                    this.FillLevelScale.ValueT = this.ScaleSensor.ActualValue.ValueT;
                else
                {
                    var weighingFunctions = CurrentScaleFunctions;
                    if (weighingFunctions.Where(c => c.CurrentScaleForWeighing != null).Any())
                    {
                        double currentFillLevel = FillLevel.ValueT;
                        foreach (IPAFuncScaleConfig weighingFunction in weighingFunctions)
                        {
                            PAEScaleBase currentScale = weighingFunction.CurrentScaleForWeighing;
                            if (currentScale != null && (weighingFunction as IPAFuncTransportMaterial).IsTransportActive)
                            {
                                PAEScaleTotalizing totalizingScale = currentScale as PAEScaleTotalizing;
                                if (weighingFunction is IPAFuncReceiveMaterial)
                                {
                                    if (totalizingScale != null)
                                        currentFillLevel -= totalizingScale.TotalActualWeight.ValueT - totalizingScale.StoredWeightForPosting.ValueT;
                                    else
                                        currentFillLevel -= currentScale.ActualWeight.ValueT;
                                }
                                else if (weighingFunction is IPAFuncDeliverMaterial)
                                {
                                    if (totalizingScale != null)
                                        currentFillLevel += Math.Abs(totalizingScale.TotalActualWeight.ValueT - totalizingScale.StoredWeightForPosting.ValueT);
                                    else
                                        currentFillLevel += Math.Abs(currentScale.ActualWeight.ValueT);
                                }
                            }
                        }
                        FillLevelScale.ValueT = currentFillLevel;
                    }
                    else
                    {
                        this.FillLevelScale.ValueT = this.FillLevel.ValueT;
                    }
                }
            }
        }


        public override void RunStateValidation()
        {
            ValidateCurrentTransportFuncs(true);
        }

        public bool ValidateCurrentTransportFuncs(bool repair)
        {
            bool valid = true;
            var dischargings = GetActiveDischargingsToThisSilo();
            if (dischargings != null && dischargings.Any())
            {
                foreach (var dis in dischargings)
                {
                    if (dis.CurrentDischargingSilo != this)
                    {
                        valid = false;
                        break;
                    }
                }
            }

            if (valid)
            {
                var dosings = GetActiveDosingsFromThisSilo();
                if (dosings != null && dosings.Any())
                {
                    foreach (var dos in dosings)
                    {
                        if (dos.CurrentDosingSilo != this)
                        {
                            valid = false;
                            break;
                        }
                    }
                }
            }

            if (!valid && repair)
            {
                RebuildCurrentTransportFunctions();
            }
            return valid;
        }

        private List<PAFDosing> RebuildDosingsFromThisSilo()
        {
            List<PAFDosing> activeDosings = null;
            if (ApplicationManager == null)
                return null;
            //int routeID = RouteItemIDAsNum;
            //if (routeID <= 0)
            //    return null;
            try
            {
                var routingService = this.RoutingService;
                if (routingService != null)
                {
                    activeDosings = new List<PAFDosing>();
                    var routingResult = ACRoutingService.MemFindSuccessors(routingService, null, this, SelRuleID_DosingFunc, RouteDirections.Forwards, 0, true, true);
                    if (routingResult != null && routingResult.Routes != null && routingResult.Routes.Any())
                    {
                        foreach (var route in routingResult.Routes)
                        {
                            var moduleWithDosing = route.Items.LastOrDefault();
                            if (moduleWithDosing != null)
                            {
                                foreach (PAFDosing pafDosing in moduleWithDosing.TargetACComponent.FindChildComponents<PAFDosing>(d => d is PAFDosing, null, 1))
                                {
                                    if (pafDosing.IsDosingActiveFromSilo(this))
                                        activeDosings.Add(pafDosing);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var appManager in this.Root.FindChildComponents<ApplicationManager>(c => c is ApplicationManager, null, 1))
                    {
                        var dosings = appManager.FindChildComponents<PAFDosing>(c => c is PAFDosing && (c as PAFDosing).IsDosingActiveFromSilo(this),
                                                                                    c => c is PWBase);
                        if (dosings != null && dosings.Any())
                            activeDosings.AddRange(dosings);
                    }
                }
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException(this.GetACUrl(), "RebuildDosingsFromThisSilo", msg);
            }
            return activeDosings;
        }

        private List<PAFDischarging> RebuildDischargingsToThisSilo()
        {
            if (ApplicationManager == null)
                return null;
            //int routeID = RouteItemIDAsNum;
            //if (routeID <= 0)
            //    return null;
            List<PAFDischarging> activeDischargings = new List<PAFDischarging>();
            try
            {
                var routingService = this.RoutingService;
                if (routingService != null)
                {
                    var routingResult = ACRoutingService.MemFindSuccessors(routingService, null, this, SelRuleID_DischargingFunc, RouteDirections.Backwards, 0, true, true);
                    if (routingResult != null && routingResult.Routes != null && routingResult.Routes.Any())
                    {
                        foreach (var route in routingResult.Routes)
                        {
                            var moduleWithDischarging = route.Items.FirstOrDefault();
                            if (moduleWithDischarging != null)
                            {
                                foreach (PAFDischarging pafDischarging in moduleWithDischarging.SourceACComponent.FindChildComponents<PAFDischarging>(d => d is PAFDischarging, null, 1))
                                {
                                    if (pafDischarging.IsDischargingActiveToSilo(this))
                                        activeDischargings.Add(pafDischarging);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var appManager in this.Root.FindChildComponents<ApplicationManager>(c => c is ApplicationManager, null, 1))
                    {
                        var dischargings = appManager.FindChildComponents<PAFDischarging>(c => c is PAFDischarging && (c as PAFDischarging).IsDischargingActiveToSilo(this),
                                                                                  c => c is PWBase);
                        if (dischargings != null && dischargings.Any())
                            activeDischargings.AddRange(dischargings);
                    }
                }
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                Messages.LogException(this.GetACUrl(), "RebuildDischargingsToThisSilo", msg);
            }
            return activeDischargings;
        }

        public virtual IEnumerable<PAFDischarging> GetActiveDischargingsToThisSilo()
        {
            return this.CurrentDischargingFunctions.Where(c => c is PAFDischarging).Select(c => c as PAFDischarging);
        }

        public virtual IEnumerable<PAFDosing> GetActiveDosingsFromThisSilo()
        {
            return CurrentDosingFunctions.Where(c => c is PAFDosing).Select(c => c as PAFDosing);
        }

        protected virtual void OnCurrentTransportFunctionChanged(IPAFuncTransportMaterial newFunction, bool added)
        {
        }

        protected void ValidateSensorState(VSSInvoker invoker = VSSInvoker.Unknown, Facility facilitySilo = null)
        {
            if (!this.Root.Initialized)
                return;
            if (IsSimulationOn)
                return;
            IACCommSession session = Session as IACCommSession;
            if (session == null || !session.IsConnected.ValueT)
                return;
            if (facilitySilo == null)
            {
                facilitySilo = Facility.ValueT.ValueT;
                if (facilitySilo == null)
                    return;
            }
            FacilityFillValidation validMode = facilitySilo.FillValidationMode();
            /// Root.Environment.TranslateMessage(this, "ErrorXXXXX")
            /// Wie Melder eingestellt werden müssen z.B.
            /// 
            /// |   V   |  Vollmedler z.B. 5100kg
            /// |-------|
            /// |   M   |  Maximaler Siloinhalt unterhalb des Vollmedler z.B. 5000kg
            /// |-------|
            /// |       |
            /// |       |
            /// |-------|
            /// |   T   |  Leertoleranz Silo z.B. 50kg
            /// |-------|
            /// |   L   |  Leermelder z.B. 30kg
            /// \       /
            ///  \     /
            ///   \   /
            ///   -----
            /// 
            if (MatSensorEmtpy != null
                && facilitySilo.Tolerance > 0.1
                && validMode.HasFlag(FacilityFillValidation.EmptySensor))
            {
                // Der Leermelder meldet dass das Silo leer ist jedoch ist der Bestand höher als die Leertoleranz
                if (MatSensorEmtpy.SensorState.ValueT != PANotifyState.Off
                    && _CurrentStock > facilitySilo.Tolerance)
                {
                    // Prüfung nur, wenn nicht herausdosiert wird, weil manchmal der Leerlmelder kommen kann
                    // und wenn nicht erstmalige Zugangsbuchung auf dem Silo und das Material noch nicht angekommen ist, weil der Transport noch läuft.
                    var activeDosings = GetActiveDosingsFromThisSilo();
                    if (   (activeDosings == null || !activeDosings.Any())
                        && !(invoker == VSSInvoker.OnRefreshFacility && _LastStock <= 0.00001))
                    {
                        //Error50179: The empty message indicates that the silo {0} is empty, but the stock is higher than the empty tolerance is set.
                        Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "ValidateSensorState(1)", 1000, "Error50179", this.ACIdentifier);

                        if (ValidationError.ValueT == PANotifyState.Off && (IsAlarmActive(ValidationError, msg.Message) == null))
                            Messages.LogWarning(this.GetACUrl(), msg.ACIdentifier, msg.Message);
                        ValidationError.ValueT = PANotifyState.AlarmOrFault;
                        OnNewAlarmOccurred(ValidationError, msg, true);
                    }
                    return;
                }
                // Der Leermelder meldet dass das Silo Material enthält jedoch ist der Bestand niedriger als 0kg
                else if (MatSensorEmtpy.SensorState.ValueT == PANotifyState.Off
                    && _CurrentStock <= 0.0001)
                {
                    var activeDischargings = GetActiveDischargingsToThisSilo();
                    if (activeDischargings == null || !activeDischargings.Any())
                    {
                        //Error50180: The empty detector reports that the silo {0} contains material but the stock is lower than 0kg.
                        Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "ValidateSensorState(2)", 1010, "Error50180", this.ACIdentifier);

                        if (ValidationError.ValueT == PANotifyState.Off && (IsAlarmActive(ValidationError, msg.Message) == null))
                            Messages.LogWarning(this.GetACUrl(), msg.ACIdentifier, msg.Message);
                        ValidationError.ValueT = PANotifyState.AlarmOrFault;
                        OnNewAlarmOccurred(ValidationError, msg, true);
                    }
                    return;
                }
            }
            if (MatSensorFull != null
                && facilitySilo.MaxWeightCapacity > 0.1
                && validMode.HasFlag(FacilityFillValidation.FullSensor))
            {
                // Der Vollmelder meldet dass das Silo voll ist jedoch ist der Bestand unterhalb des eingestellten Maximalen Inhalts
                if (MatSensorFull.SensorState.ValueT != PANotifyState.Off
                    && _CurrentStock < facilitySilo.MaxWeightCapacity)
                {
                    //Error50181: The full detector reports that the silo {0} is full, but the stock is below the specified maximum content.
                    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "ValidateSensorState(3)", 1020, "Error50181", this.ACIdentifier);

                    if (ValidationError.ValueT == PANotifyState.Off && (IsAlarmActive(ValidationError, msg.Message) == null))
                        Messages.LogWarning(this.GetACUrl(), msg.ACIdentifier, msg.Message);
                    ValidationError.ValueT = PANotifyState.AlarmOrFault;
                    OnNewAlarmOccurred(ValidationError, msg, true);
                    return;
                }
                // Der Bestand von Silo {0} ist über dem eingestellten Maximalen Inhalt, jedoch medlet der Vollmelder nicht
                else if (MatSensorFull.SensorState.ValueT == PANotifyState.Off
                    && _CurrentStock > facilitySilo.MaxWeightCapacity * 1.05) // + 5% Toleranz
                {
                    //Error50182: The stock of silo {0} is above the specified maximum content, but the full detector does not report..
                    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "ValidateSensorState(4)", 1030, "Error50182", this.ACIdentifier);

                    if (ValidationError.ValueT == PANotifyState.Off && (IsAlarmActive(ValidationError, msg.Message) == null))
                        Messages.LogWarning(this.GetACUrl(), msg.ACIdentifier, msg.Message);
                    ValidationError.ValueT = PANotifyState.AlarmOrFault;
                    OnNewAlarmOccurred(ValidationError, msg, true);
                    return;
                }
            }
            if (this.ScaleSensor != null
                && facilitySilo.MaxWeightCapacity > 0.1
                && validMode.HasFlag(FacilityFillValidation.ScaleWithStock))
            {
                // Falls Abweichung vom Waagenwert > 5% Toleranz
                double diff = Math.Abs(this.FillLevelScale.ValueT - _CurrentStock);
                if (diff > facilitySilo.MaxWeightCapacity * 0.05)
                {
                    //Error50183: The stock of silo {0} deviates too much from the measured scale value.
                    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "ValidateSensorState(3)", 1040, "Error50183", this.ACIdentifier);

                    if (ValidationError.ValueT == PANotifyState.Off && (IsAlarmActive(ValidationError, msg.Message) == null))
                        Messages.LogWarning(this.GetACUrl(), msg.ACIdentifier, msg.Message);
                    ValidationError.ValueT = PANotifyState.AlarmOrFault;
                    OnNewAlarmOccurred(ValidationError, msg, true);
                    return;
                }
            }
            if (ValidationError.ValueT == PANotifyState.AlarmOrFault)
                AcknowledgeAlarms();
        }

        public override void AcknowledgeAlarms()
        {
            if (!IsEnabledAcknowledgeAlarms())
                return;
            if (ValidationError.ValueT == PANotifyState.AlarmOrFault)
            {
                ValidationError.ValueT = PANotifyState.Off;
                if (this.ApplicationManager.ApplicationQueue != null)
                {
                    this.ApplicationManager.ApplicationQueue.Add(() => { ValidateSensorState(VSSInvoker.AcknowledgeAlarms); });
                }
            }
            base.AcknowledgeAlarms();
        }

        public override bool IsEnabledAcknowledgeAlarms()
        {
            if (ValidationError.ValueT == PANotifyState.AlarmOrFault)
                return true;
            return base.IsEnabledAcknowledgeAlarms();
        }
        #endregion


        #region Client-Methods
        [ACMethodInteractionClient("", "en{'Manage stock'}de{'Bestand verwalten'}", 450, false, "", false, Global.ContextMenuCategory.ProdPlanLog)]
        public static void FacilityBookingDialogOn(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            if (!IsEnabledFacilityBookingDialogOn(acComponent))
                return;

            PAShowDlgManagerVB serviceInstance = PAShowDlgManagerVB.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
            if (serviceInstance == null)
                return;
            serviceInstance.ShowFacilityBookCellDialog(acComponent);
        }

        public static bool IsEnabledFacilityBookingDialogOn(IACComponent acComponent)
        {
            PAShowDlgManagerVB serviceInstance = PAShowDlgManagerVB.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
            if (serviceInstance == null)
                return false;
            return serviceInstance.IsEnabledShowFacilityBookCellDialog(acComponent);
        }


        [ACMethodInteractionClient("", "en{'Stockhistory'}de{'Bestandshistorie'}", 450, false, "", false, Global.ContextMenuCategory.ProdPlanLog)]
        public static void ShowFacilityOverview(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            if (!IsEnabledShowFacilityOverview(acComponent))
                return;

            PAShowDlgManagerVB serviceInstance = PAShowDlgManagerVB.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
            if (serviceInstance == null)
                return;
            serviceInstance.ShowFacilityOverviewDialog(acComponent);
        }

        public static bool IsEnabledShowFacilityOverview(IACComponent acComponent)
        {
            PAShowDlgManagerVB serviceInstance = PAShowDlgManagerVB.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
            if (serviceInstance == null)
                return false;
            return serviceInstance.IsEnabledShowFacilityOverviewDialog(acComponent);
        }


        [ACMethodInteractionClient("", "en{'View reservation'}de{'Reservierungen anschauen'}", 450, false)]
        public static void ShowReservationDialog(IACComponent acComponent)
        {
            ACComponent _this = acComponent as ACComponent;
            if (!IsEnabledShowReservationDialog(acComponent))
                return;

            PAShowDlgManagerVB serviceInstance = PAShowDlgManagerVB.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
            if (serviceInstance == null)
                return;
            serviceInstance.ShowReservationDialog(acComponent);

            return;

        }

        public static bool IsEnabledShowReservationDialog(IACComponent acComponent)
        {
            PAShowDlgManagerVB serviceInstance = PAShowDlgManagerVB.GetServiceInstance(acComponent.Root as ACComponent) as PAShowDlgManagerVB;
            if (serviceInstance == null)
                return false;
            return serviceInstance.IsEnabledShowReservationDialog(acComponent);
        }

        #endregion

        #region Dumping and Testing
        protected override void DumpPropertyList(XmlDocument doc, XmlElement xmlACPropertyList)
        {
            base.DumpPropertyList(doc, xmlACPropertyList);

            XmlElement xmlChild = xmlACPropertyList["CurrentTransportFunctions"];
            if (xmlChild == null)
            {
                xmlChild = doc.CreateElement("CurrentTransportFunctions");
                if (xmlChild != null)
                {
                    if (CurrentTransportFunctions.Any())
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var functionURL in CurrentTransportFunctions.Select(c => c.GetACUrl()))
                        {
                            sb.AppendLine(functionURL);
                        }
                        xmlChild.InnerText = sb.ToString();
                    }
                    else
                    {
                        xmlChild.InnerText = "Empty";
                    }
                }
                xmlACPropertyList.AppendChild(xmlChild);
            }
        }
        #endregion
        
        #endregion
    }
}
