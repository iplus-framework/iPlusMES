using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using gip.core.processapplication;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Discharging'}de{'Entleeren'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWDischarging.PWClassName, true)]
    public class PAFDischarging : PAProcessFunction, IPAFuncDeliverMaterial, IPAFuncScaleConfig
    {
        
        #region Properties

        #region Tolerance State
        [ACPropertyBindingTarget(635, "Read from PLC", "en{'State of Tolerance'}de{'Status Toleranz'}", "", false, false, RemotePropID = 19)]
        public IACContainerTNet<PANotifyState> StateTolerance { get; set; }
        public void OnSetStateTolerance(IACPropertyNetValueEvent valueEvent)
        {
            PANotifyState newSensorState = (valueEvent as ACPropertyValueEvent<PANotifyState>).Value;
            if (FaultAckTolerance.ValueT && newSensorState != PANotifyState.AlarmOrFault)
                FaultAckTolerance.ValueT = false;
            if (newSensorState != StateTolerance.ValueT)
            {
                if (newSensorState == PANotifyState.AlarmOrFault)
                    _StateToleranceAlarmChanged = PAAlarmChangeState.NewAlarmOccurred;
                else if (newSensorState == PANotifyState.Off)
                    _StateToleranceAlarmChanged = PAAlarmChangeState.AlarmDisappeared;
            }
        }

        private PAAlarmChangeState _StateToleranceAlarmChanged = PAAlarmChangeState.NoChange;
        void StateTolerance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_StateToleranceAlarmChanged != PAAlarmChangeState.NoChange && e.PropertyName == Const.ValueT)
            {
                OnAlarmDisappeared(StateTolerance);
                if (_StateToleranceAlarmChanged == PAAlarmChangeState.NewAlarmOccurred)
                    OnNewAlarmOccurred(StateTolerance);
                else
                    OnAlarmDisappeared(StateTolerance);
                _StateToleranceAlarmChanged = PAAlarmChangeState.NoChange;
            }
        }
        [ACPropertyBindingTarget(654, "Write to PLC", "en{'Fault acknowledge Tolerance'}de{'Toeranzquittung'}", "", true, false, RemotePropID = 20)]
        public IACContainerTNet<bool> FaultAckTolerance { get; set; }
        #endregion


        #region DestinationFull State
        private bool _StateDestinationFullForced = false;
        public bool IsStateDestinationFullForced
        {
            get
            {
                return _StateDestinationFullForced;
            }
        }

        [ACPropertyBindingTarget(635, "Read from PLC", "en{'State of DestinationFull'}de{'Ziel voll'}", "", false, false, RemotePropID = 21)]
        public IACContainerTNet<PANotifyState> StateDestinationFull { get; set; }
        public void OnSetStateDestinationFull(IACPropertyNetValueEvent valueEvent)
        {
            PANotifyState newSensorState = (valueEvent as ACPropertyValueEvent<PANotifyState>).Value;
            if (FaultAckDestinationFull.ValueT && newSensorState != PANotifyState.AlarmOrFault)
                FaultAckDestinationFull.ValueT = false;
            if (newSensorState != StateDestinationFull.ValueT)
            {
                if (newSensorState == PANotifyState.AlarmOrFault)
                    _StateDestinationFullAlarmChanged = PAAlarmChangeState.NewAlarmOccurred;
                else if (newSensorState == PANotifyState.Off)
                    _StateDestinationFullAlarmChanged = PAAlarmChangeState.AlarmDisappeared;
            }
        }

        private PAAlarmChangeState _StateDestinationFullAlarmChanged = PAAlarmChangeState.NoChange;
        void StateDestinationFull_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_StateDestinationFullAlarmChanged != PAAlarmChangeState.NoChange && e.PropertyName == Const.ValueT)
            {
                OnAlarmDisappeared(StateDestinationFull);
                if (_StateDestinationFullAlarmChanged == PAAlarmChangeState.NewAlarmOccurred)
                    OnNewAlarmOccurred(StateDestinationFull);
                else
                    OnAlarmDisappeared(StateDestinationFull);
                _StateDestinationFullAlarmChanged = PAAlarmChangeState.NoChange;
            }
        }
        [ACPropertyBindingTarget(654, "Write to PLC", "en{'Fault acknowledge DestinationFull'}de{'Ziel voll Quittung'}", "", true, false, RemotePropID = 22)]
        public IACContainerTNet<bool> FaultAckDestinationFull { get; set; }
        #endregion

        #region Misc
        public const string VMethodName_Discharging = "Discharging";

        private PAScaleMappingHelper<PAEScaleBase> _ScaleMappingHelper;
        public PAScaleMappingHelper<PAEScaleBase> ScaleMappingHelper
        {
            get
            {
                using (ACMonitor.Lock(_20015_LockValue))
                {
                    if (_ScaleMappingHelper == null)
                        _ScaleMappingHelper = new PAScaleMappingHelper<PAEScaleBase>(this, this);
                }
                return _ScaleMappingHelper;
            }
        }

        public virtual PAEScaleBase CurrentScaleForWeighing
        {
            get
            {
                PAEScaleBase scale = ScaleMappingHelper.AssignedScales.FirstOrDefault();
                if (scale != null)
                    return scale;
                var queryScales = ParentACComponent.FindChildComponents<PAEScaleBase>(c => c is PAEScaleBase);
                if (!queryScales.Any())
                    return null;
                if (queryScales.Count == 1)
                    return queryScales.FirstOrDefault();
                var charLast = this.ACIdentifier.Last();
                var foundScale = queryScales.Where(c => c.ACIdentifier.Last() == charLast).FirstOrDefault();
                if (foundScale != null)
                    return foundScale;
                return queryScales.FirstOrDefault();
            }
        }

        public PAMSilo CurrentDischargingSilo
        {
            get
            {
                try
                {
                    if (IsTransportActive
                        && CurrentACMethod != null
                        && CurrentACMethod.ValueT != null)
                    {
                        return GetTargetSiloFromMethod(CurrentACMethod.ValueT);
                    }
                }
                catch (Exception e)
                {
                    if (Messages != null)
                        Messages.LogException(this.GetACUrl(), "CurrentDischargingSilo", e);
                }
                return null;
            }
        }

        #endregion

        #endregion

        #region Config
        protected ACPropertyConfigValue<string> _FuncScaleConfig;
        [ACPropertyConfig("en{'Assigned Scales'}de{'Zugeordnete Waagen'}")]
        public string FuncScaleConfig
        {
            get
            {
                return _FuncScaleConfig.ValueT;
            }
        }
        #endregion

		
        #region Constructors

        static PAFDischarging()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFDischarging), ACStateConst.TMStart, CreateVirtualMethod(VMethodName_Discharging, "en{'Discharge'}de{'Entleeren'}", typeof(PWDischarging)));
            ACMethod.RegisterVirtualMethod(typeof(PAFDischarging), ACStateConst.TMStart, CreateVirtualMethod(VMethodName_Discharging + "Intake", "en{'Discharge intake'}de{'Entleeren Annahme'}", typeof(PWDischarging)));
            RegisterExecuteHandler(typeof(PAFDischarging), HandleExecuteACMethod_PAFDischarging);
        }

        public PAFDischarging(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _FuncScaleConfig = new ACPropertyConfigValue<string>(this, PAScaleMappingHelper<IACComponent>.FuncScaleConfigName, "");
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _ = FuncScaleConfig;
            StateDestinationFull.PropertyChanged += StateDestinationFull_PropertyChanged;
            StateTolerance.PropertyChanged += StateTolerance_PropertyChanged;

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            StateDestinationFull.PropertyChanged -= StateDestinationFull_PropertyChanged;
            StateTolerance.PropertyChanged -= StateTolerance_PropertyChanged;
            using (ACMonitor.Lock(_20015_LockValue))
            {
                if (_ScaleMappingHelper != null)
                {
                    _ScaleMappingHelper.DetachAndRemove();
                    _ScaleMappingHelper = null;
                }
            }
            return base.ACDeInit(deleteACClassTask);
        }

        public override bool ACPostInit()
        {
            bool succ = base.ACPostInit();

            try
            {
                if (succ
                    && IsTransportActive
                    && CurrentACMethod != null
                    && CurrentACMethod.ValueT != null)
                {
                    PAMSilo pamSilo = GetTargetSiloFromMethod(CurrentACMethod.ValueT);
                    if (pamSilo != null)
                        pamSilo.SubscribeTransportFunction(this);
                }
            }
            catch (Exception e)
            {
                if (Messages != null)
                    Messages.LogException(this.GetACUrl(), "ACPostInit", e);
            }
            return succ;
        }


        #endregion

        #region override abstract methods

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "ForceChangeDest":
                    ForceChangeDest();
                    return true;
                case Const.IsEnabledPrefix + "ForceChangeDest":
                    result = IsEnabledForceChangeDest();
                    return true;
                case "InheritParamsFromConfig":
                    InheritParamsFromConfig(acParameter[0] as ACMethod, acParameter[1] as ACMethod, (bool)acParameter[2]);
                    return true;
                case "SetDefaultACMethodValues":
                    SetDefaultACMethodValues(acParameter[0] as ACMethod);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PAFDischarging(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion

        protected override MsgWithDetails CompleteACMethodOnSMStarting(ACMethod acMethod, ACMethod previousParams)
        {
            ACValue value = acMethod.ParameterValueList.GetACValue(nameof(Route));
            if (value == null)
            {
                MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "CompleteACMethodOnSMStarting(1)", Message = "Route is empty." };
                return msg;
            }

            Route newR = value.ValueT<Route>();
            if (newR == null)
            {
                MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "CompleteACMethodOnSMStarting(2)", Message = "Route is null." };
                return msg;
            }

            ACValue valuePrev = null;
            Route prevR = null;
            if (previousParams != null && IsSimulationOn)
            {
                valuePrev = previousParams.ParameterValueList.GetACValue(nameof(Route));
                if (valuePrev != null)
                    prevR = valuePrev.ValueT<Route>();
            }

            //Route clonedR = originalR.Clone() as Route;
            //RouteItem targetRouteItem = clonedR.LastOrDefault();
            RouteItem targetRouteItem = newR.LastOrDefault();
            if (targetRouteItem == null)
            {
                MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "CompleteACMethodOnSMStarting(2)", Message = "Last RouteItem is null." };
                return msg;
            }

            if (newR.IsPredefinedRoute)
                RoutingService.ExecuteMethod(nameof(ACRoutingService.OnRouteUsed), newR);

            using (var db = new Database())
            {
                try
                {
                    newR?.AttachTo(db); // Global context
                    prevR?.AttachTo(db);

                    MsgWithDetails msg = GetACMethodFromConfig(db, newR, acMethod);
                    if (msg != null)
                        return msg;

                    if (IsSimulationOn)
                    {
                        if (prevR != null && acMethod != previousParams)
                            PAEControlModuleBase.ActivateRouteOnSimulation(prevR, true);
                        PAEControlModuleBase.ActivateRouteOnSimulation(newR, false);
                    }
                }
                catch (Exception e)
                {
                    Messages.LogException(this.GetACUrl(), "CompleteACMethodOnSMStarting()", e);
                    MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "CompleteACMethodOnSMStarting(3)", Message = e.Message };
                    return msg;
                }
                finally
                {
                    newR?.Detach(true);
                    prevR?.Detach(true);
                    //targetRouteItem.DetachEntitesFromDbContext();
                    // Update Route, that Route-serializers can access ACComponent-Properties of RouteItems
                    //if (!originalR.AreACUrlInfosSet)
                    //value.Value = clonedR;
                }
            }


            if (targetRouteItem != null && IsSimulationOn && !IsManualSimulation)
            {
                PAEScaleGravimetric scale = this.CurrentScaleForWeighing as PAEScaleGravimetric;
                IACCommSession session = Session as IACCommSession;
                if (scale != null
                    && !IsManualSimulation
                    && CurrentACMethod != null && CurrentACMethod.ValueT != null
                    && (session == null || !session.IsConnected.ValueT))
                {
                    var targetScale = targetRouteItem.TargetACComponent as IPAMContScale;
                    if (targetScale != null && targetScale.Scale != null)
                        targetScale.Scale.ActualValue.ValueT += scale.ActualValue.ValueT;
                }
            }
            return null;
        }

        protected override PAProcessFunction.CompleteResult AnalyzeACMethodResult(ACMethod acMethod, out MsgWithDetails msg, CompleteResult completeResult)
        {
            PAMSilo pamSilo = GetTargetSiloFromMethod(acMethod);
            if (pamSilo != null)
                pamSilo.UnSubscribeTransportFunction(this);

            if (this.IsSimulationOn)
            {
                if (!IsManualSimulation)
                {
                    var acValue = acMethod.ResultValueList.GetACValue("ActualQuantity");
                    PAEScaleGravimetric scale = this.CurrentScaleForWeighing as PAEScaleGravimetric;
                    if (scale != null
                        && acValue != null
                        && acValue.ParamAsDouble < 0.0000001)
                    {
                        double actualQuantity = acValue.ParamAsDouble;
                        if (scale.StoredTareWeight.ValueT > 0.00001)
                        {
                            actualQuantity = scale.StoredTareWeight.ValueT;
                            if (CurrentACState == ACStateEnum.SMAborted || CurrentACState == ACStateEnum.SMAborting)
                                actualQuantity = scale.StoredTareWeight.ValueT - scale.ActualValue.ValueT;
                        }
                        acMethod.ResultValueList["ActualQuantity"] = actualQuantity;
                    }
                }
                if (CurrentACState >= ACStateEnum.SMCompleted)
                {
                    ACValue value = acMethod.ParameterValueList.GetACValue(nameof(Route));
                    if (value != null)
                    {
                        using (var db = new Database())
                        {
                            core.datamodel.Route route = value.ValueT<core.datamodel.Route>().Clone() as core.datamodel.Route;
                            try
                            {
                                route.AttachTo(db); // Global context
                                PAEControlModuleBase.ActivateRouteOnSimulation(route, true);

                            }
                            catch (Exception e)
                            {
                                Messages.LogException(this.GetACUrl(), "AnalyzeACMethodResult(20)", e);
                            }
                            finally
                            {
                                route.Detach(true);
                            }
                        }
                    }
                }
            }

            var container = ParentACComponent as IPAMCont;
            if (container != null)
            {
                container.ResetFillVolume();
            }
            msg = null;
            return completeResult;
        }

        public override void InitializeRouteAndConfig(Database dbIPlus)
        {
            gip.core.datamodel.ACClass thisACClass = this.ComponentClass;
            gip.core.datamodel.ACClass parentACClass = ParentACComponent.ComponentClass;
            try
            {
                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    Database = dbIPlus,
                    Direction = RouteDirections.Forwards,
                    DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != parentACClass.ACClassID,
                    DBIncludeInternalConnections = true,
                    AutoDetachFromDBContext = false
                };

                var routes = ACRoutingService.DbSelectRoutesFromPoint(thisACClass, this.PAPointMatOut1.PropertyInfo, routingParameters);
                if (routes != null && routes.Any())
                {
                    string virtMethodName = VMethodName_Discharging;
                    IReadOnlyList<ACMethodWrapper> virtualMethods = ACMethod.GetVirtualMethodInfos(this.GetType(), ACStateConst.TMStart);
                    if (virtualMethods != null && virtualMethods.Any())
                        virtMethodName = virtualMethods.FirstOrDefault().Method.ACIdentifier;
                    virtMethodName = OnGetVMethodNameForRouteInitialization(virtMethodName);

                    foreach (Route route in routes)
                    {
                        ACMethod acMethod = ACUrlACTypeSignature("!" + virtMethodName);
                        GetACMethodFromConfig(dbIPlus, route, acMethod, true);
                    }
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), "InitializeRouteAndConfig(0)", e.Message);
            }
        }

        protected MsgWithDetails GetACMethodFromConfig(Database db, Route route, ACMethod acMethod, bool isConfigInitialization = false)
        {
            if (route == null || route.Count < 1 || ParentACComponent == null)
                return new MsgWithDetails();
            if (IsMethodChangedFromClient)
                return null;
            RouteItem sourceRouteItem = route.Where(c => c.SourceGuid == ParentACComponent.ComponentClass.ACClassID).FirstOrDefault();
            if (sourceRouteItem == null)
                sourceRouteItem = route.FirstOrDefault();
            if (sourceRouteItem.Source.ACKind == Global.ACKinds.TPAProcessFunction)
            {
                if (route.Count < 2)
                    return new MsgWithDetails();
                sourceRouteItem = route[1];
            }
            RouteItem targetRouteItem = route.LastOrDefault();

            ACValue valueDest = acMethod.ParameterValueList.GetACValue("Destination");
            if (valueDest != null && targetRouteItem != null)
            {
                try
                {
                    bool setTarget = false;
                    if (valueDest.Value is Int16)
                    {
                        setTarget = valueDest.ParamAsInt16 <= 0;
                    }
                    else if (valueDest.Value is UInt16)
                    {
                        setTarget = valueDest.ParamAsUInt16 <= 0;
                    }

                    if (setTarget)
                    {
                        PAProcessModule pamTarget = targetRouteItem.TargetACComponent as PAProcessModule;
                        if (pamTarget != null)
                        {
                            if (valueDest.Value is Int16)
                                valueDest.Value = Convert.ToInt16(pamTarget.RouteItemIDAsNum);
                            else if (valueDest.Value is UInt16)
                                valueDest.Value = Convert.ToUInt16(pamTarget.RouteItemIDAsNum);
                        }
                    }
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    Messages.LogException("PAFDischarging", "GetACMethodFromConfig", msg);
                }
            }

            OnSetRouteItemData(acMethod, targetRouteItem, sourceRouteItem, route, isConfigInitialization);


            List<MaterialConfig> materialConfigList = null;
            gip.core.datamodel.ACClass thisACClass = ComponentClass.FromIPlusContext<gip.core.datamodel.ACClass>(db);
            gip.core.datamodel.ACClassConfig config = null;
            gip.core.datamodel.ACClassPropertyRelation logicalRelation = db.ACClassPropertyRelation
                .Where(c => c.SourceACClassID == sourceRouteItem.Source.ACClassID
                            && c.SourceACClassPropertyID == sourceRouteItem.SourceProperty.ACClassPropertyID
                            && c.TargetACClassID == targetRouteItem.Target.ACClassID
                            && c.TargetACClassPropertyID == targetRouteItem.TargetProperty.ACClassPropertyID)
                .FirstOrDefault();
            if (logicalRelation == null)
            {
                logicalRelation = gip.core.datamodel.ACClassPropertyRelation.NewACObject(db, null);
                logicalRelation.SourceACClass = sourceRouteItem.Source;
                logicalRelation.SourceACClassProperty = sourceRouteItem.SourceProperty;
                logicalRelation.TargetACClass = targetRouteItem.Target;
                logicalRelation.TargetACClassProperty = targetRouteItem.TargetProperty;
                logicalRelation.ConnectionType = Global.ConnectionTypes.DynamicConnection;
            }
            else
            {
                config = logicalRelation.ACClassConfig_ACClassPropertyRelation.FirstOrDefault();
                if (!isConfigInitialization)
                {
                    PAMSilo pamSilo = targetRouteItem.TargetACComponent as PAMSilo;
                    if (pamSilo != null)
                    {
                        if (pamSilo.Facility != null && pamSilo.Facility.ValueT != null && pamSilo.Facility.ValueT.ValueT != null)
                        {
                            Guid? materialID = pamSilo.Facility.ValueT.ValueT.MaterialID;
                            if (materialID.HasValue && materialID != Guid.Empty)
                            {
                                Guid acClassIDOfParent = ParentACComponent.ComponentClass.ACClassID;
                                using (var dbApp = new DatabaseApp())
                                {
                                    // 1. Hole Material-Konfiguration spezielle fü diesen Weg
                                    materialConfigList = dbApp.MaterialConfig.Where(c => c.VBiACClassPropertyRelationID == logicalRelation.ACClassPropertyRelationID && c.MaterialID == materialID.Value).SetMergeOption(System.Data.Objects.MergeOption.NoTracking).ToList();
                                    var wayIndependent = dbApp.MaterialConfig.Where(c => c.MaterialID == materialID.Value && c.VBiACClassID == acClassIDOfParent).SetMergeOption(System.Data.Objects.MergeOption.NoTracking);
                                    foreach (var matConfigIndepedent in wayIndependent)
                                    {
                                        if (!materialConfigList.Where(c => c.LocalConfigACUrl == matConfigIndepedent.LocalConfigACUrl).Any())
                                            materialConfigList.Add(matConfigIndepedent);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ACMethod storedACMethod = null;
            if (config == null)
            {
                config = thisACClass.NewACConfig(null, db.GetACType(typeof(ACMethod))) as gip.core.datamodel.ACClassConfig;
                config.KeyACUrl = logicalRelation.GetKey();
                config.ACClassPropertyRelation = logicalRelation;
            }
            else
                storedACMethod = config.Value as ACMethod;

            bool isNewDefaultedMethod = false;
            bool differentVirtualMethod = false;
            if (storedACMethod == null || storedACMethod.ACIdentifier != acMethod.ACIdentifier)
            {
                if (storedACMethod != null && storedACMethod.ACIdentifier != acMethod.ACIdentifier)
                {
                    differentVirtualMethod = true;
                    var clonedMethod = acMethod.Clone() as ACMethod;
                    clonedMethod.CopyParamValuesFrom(storedACMethod);
                    storedACMethod = clonedMethod;
                }
                else
                {
                    isNewDefaultedMethod = true;
                    storedACMethod = acMethod.Clone() as ACMethod;
                    ACUrlCommand("!SetDefaultACMethodValues", storedACMethod);
                }
            }
            // Überschreibe Parameter mit materialabhängigen Einstellungen
            if (!isConfigInitialization
                && config.EntityState != System.Data.EntityState.Added 
                && materialConfigList != null 
                && materialConfigList.Any())
            {
                foreach (var matConfig in materialConfigList)
                {
                    ACValue acValue = acMethod.ParameterValueList.Where(c => c.ACIdentifier == matConfig.LocalConfigACUrl).FirstOrDefault();
                    if (acValue != null/* && acValue.HasDefaultValue*/)
                        acValue.Value = matConfig.Value;
                    if (storedACMethod != null)
                    {
                        acValue = storedACMethod.ParameterValueList.Where(c => c.ACIdentifier == matConfig.LocalConfigACUrl).FirstOrDefault();
                        if (acValue != null/* && acValue.HasDefaultValue*/)
                            acValue.Value = matConfig.Value;
                    }
                }
            }
            if (!isNewDefaultedMethod)
                ACUrlCommand("!InheritParamsFromConfig", acMethod, storedACMethod, isConfigInitialization);
            if (config.EntityState == System.Data.EntityState.Added || isNewDefaultedMethod)
                config.Value = storedACMethod;
            else if (isConfigInitialization)
            {
                if (differentVirtualMethod)
                    config.Value = storedACMethod;
                else
                    config.Value = acMethod;
            }
            if (config.EntityState == System.Data.EntityState.Added || logicalRelation.EntityState == System.Data.EntityState.Added || isNewDefaultedMethod || isConfigInitialization || differentVirtualMethod)
            {
                MsgWithDetails msg = db.ACSaveChanges();
                if (msg != null)
                    return msg;
            }
            return null;
        }

        protected virtual void OnSetRouteItemData(ACMethod acMethod, RouteItem targetItem, RouteItem sourceItem, Route route, bool isConfigInitialization)
        {
            if (targetItem != null)
            {
                PAMSilo acCompTarget = targetItem.TargetACComponent as PAMSilo;
                if (acCompTarget != null)
                    acCompTarget.SubscribeTransportFunction(this);
            }
        }

        protected override void OnChangingCurrentACMethod(ACMethod currentACMethod, ACMethod newACMethod)
        {
            base.OnChangingCurrentACMethod(currentACMethod, newACMethod);

            ACValue value = currentACMethod.ParameterValueList.GetACValue(nameof(Route));
            if (value != null)
            {
                Route originalR = value.ValueT<Route>();
                if (originalR != null && !originalR.AreACUrlInfosSet)
                {
                    using (var db = new Database())
                    {
                        try
                        {
                            originalR.AttachTo(db); // Global context
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            originalR.Detach(true);
                        }
                    }
                }
            }

            bool unsubscribe = true;
            if (IsMethodChangedFromClient)
            {
                unsubscribe = false;
                ACValue acValueNew = newACMethod.ParameterValueList.GetACValue("Destination");
                ACValue acValueOld = currentACMethod.ParameterValueList.GetACValue("Destination");
                if (acValueNew != null && acValueOld != null)
                {
                    if (acValueNew.Value is Int16)
                    {
                        if (   acValueNew.ParamAsInt16 > 0
                            && acValueNew.ParamAsInt16 != acValueOld.ParamAsInt16)
                            unsubscribe = true;
                    }
                    else if (acValueNew.Value is UInt16)
                    {
                        if (   acValueNew.ParamAsUInt16 > 0
                            && acValueNew.ParamAsUInt16 != acValueOld.ParamAsUInt16)
                            unsubscribe = true;
                    }
                }
            }

            // If Destination will be changed from PWDischarging, than previous silo must be unsubscribed
            if (unsubscribe)
            {
                PAMSilo pamSilo = GetTargetSiloFromMethod(currentACMethod);
                if (pamSilo != null)
                    pamSilo.UnSubscribeTransportFunction(this);
            }
        }

        #endregion

        #region Public

        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            StateDestinationFull.ValueT = PANotifyState.Off;
            StateTolerance.ValueT = PANotifyState.Off;
            _StateDestinationFullForced = false;
            return base.Start(acMethod);
        }


        [ACMethodInteraction("Process", "en{'Activate destination Change'}de{'Zielwechsel aktivieren'}", (short)800, true)]
        public virtual void ForceChangeDest()
        {
            if (!IsEnabledForceChangeDest())
                return;
            StateDestinationFull.ValueT = PANotifyState.AlarmOrFault;
            _StateDestinationFullForced = true;
            string user = "Service";
            if (Root.CurrentInvokingUser != null)
                user = Root.CurrentInvokingUser.VBUserName;
            Messages.LogDebug(this.GetACUrl(), "ForceChangeDest()", user);
        }

        public virtual bool IsEnabledForceChangeDest()
        {
            return StateDestinationFull.ValueT == PANotifyState.Off || !_StateDestinationFullForced;
        }

        public override void SMIdle()
        {
            base.SMIdle();
            if (CurrentACState == ACStateEnum.SMIdle)
            {
                StateDestinationFull.ValueT = PANotifyState.Off;
                StateTolerance.ValueT = PANotifyState.Off;
                _StateDestinationFullForced = false;
            }
        }
        #endregion

        #region Simulation
        protected double? _SimulationScaleIncrement = null;

        protected override bool CyclicWaitIfSimulationOn()
        {
            if (ACOperationMode != ACOperationModes.Live || !IsSimulationOn)
            {
                _SimulationScaleIncrement = null;
                return base.CyclicWaitIfSimulationOn();
            }

            var container = ParentACComponent as IPAMContScale;
            PAEScaleGravimetric scale = this.CurrentScaleForWeighing as PAEScaleGravimetric;
            IACCommSession session = Session as IACCommSession;
            if (container != null && scale != null 
                && !IsManualSimulation 
                && CurrentACMethod != null && CurrentACMethod.ValueT != null 
                && (session == null || !session.IsConnected.ValueT))
            {
                double maxScaleRange = 0;
                if (scale.UpperLimit2.ValueT > 0.00000001)
                    maxScaleRange = scale.UpperLimit2.ValueT;                    
                else if (container.MaxWeightCapacity.ValueT > 0.00000001)
                    maxScaleRange = container.MaxWeightCapacity.ValueT;

                if (maxScaleRange > 0.00000001)
                {
                    SubscribeToProjectWorkCycle();
                    scale.DesiredWeight.ValueT = 0.0;
                    double emptyRange = scale.LowerLimit2.ValueT;
                    if (scale.IsTareRequestAcknowledged)
                        scale.TareScale(true, true);
                    else if (scale.ActualValue.ValueT <= emptyRange)
                    {
                        if (!_SimulationScaleIncrement.HasValue && _SimulationWait == 0 && (Math.Abs(scale.ActualValue.ValueT - 0) <= Double.Epsilon))
                            scale.ActualValue.ValueT = maxScaleRange;
                        else
                        {
                            scale.TareScale(false, true);
                            UnSubscribeToProjectWorkCycle();
                            scale.ActualValue.ValueT = 0;
                            _SimulationWait = 0;
                            _SimulationScaleIncrement = null;
                            return false;
                        }
                    }
                    if (!_SimulationScaleIncrement.HasValue)
                    {
                        Random random = new Random();
                        int steps = random.Next(5, 20);
                        if (scale.ActualValue.ValueT > 10)
                            _SimulationScaleIncrement = (scale.ActualValue.ValueT / steps) * -1;
                        else
                            _SimulationScaleIncrement = (maxScaleRange / steps) * -1;
                    }
                    scale.SimulateWeight(_SimulationScaleIncrement.Value);
                    return true;
                }
            }
            _SimulationScaleIncrement = null;
            return base.CyclicWaitIfSimulationOn();
        }
        #endregion

        #region Misc

        public bool IsTransportActive
        {
            get
            {
                return CurrentACState >= ACStateEnum.SMRunning
                    && CurrentACState < ACStateEnum.SMResetting;
            }
        }

        public virtual void OnSiloStateChanged(PAMSilo silo, bool inwardEnabled)
        {
            if (IsDischargingActiveToSilo(silo) && !inwardEnabled)
                ForceChangeDest();
            //this.ACStateConverter.OnProjSpecFunctionEvent(this, "OnSiloStateChanged", silo, inwardEnabled);
        }

        public virtual bool IsDischargingActiveToSilo(PAMSilo silo)
        {
            if (!IsTransportActive || silo == null)
                return false;
            var acMethod = CurrentACMethod.ValueT;
            if (acMethod == null || this.ACStateConverter == null)
                return false;
            PAMSilo currentDest = GetTargetSiloFromMethod(acMethod);
            return currentDest != null && silo == currentDest;
            //var valueDest = acMethod.ParameterValueList.GetACValue("Destination");
            //if (valueDest == null)
            //    return false;
            //int currentDest = 0;
            //if (valueDest.Value is Int16)
            //    currentDest = valueDest.ParamAsInt16;
            //else if (valueDest.Value is UInt16)
            //    currentDest = valueDest.ParamAsUInt16;
            //if (currentDest <= 0)
            //    return false;
            //return silo.RouteItemIDAsNum == currentDest;
        }
        #endregion


        #region Private 
        public static PAMSilo GetTargetSiloFromMethod(ACMethod acMethod)
        {
            if (acMethod == null)
                return null;
            ACValue value = acMethod.ParameterValueList.GetACValue(nameof(Route));
            if (value == null)
                return null;
            Route route = value.ValueT<Route>();
            if (route == null)
                return null;
            RouteItem targetRouteItem = route.LastOrDefault();
            if (targetRouteItem == null)
                return null;
            PAMSilo pamSilo = targetRouteItem.TargetACComponent as PAMSilo;
            if (pamSilo != null)
                return pamSilo;
            if (targetRouteItem.IsAttached)
                return null;
            using (var db = new Database())
            {
                try
                {
                    targetRouteItem.AttachTo(db);
                    pamSilo = targetRouteItem.TargetACComponent as PAMSilo;
                }
                finally
                {
                    if (targetRouteItem != null)
                        targetRouteItem.Detach();
                }
            }
            return pamSilo;
        }

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);

            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            method.ParameterValueList.Add(new ACValue("Source", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("Source", "en{'Source'}de{'Quelle'}");
            method.ParameterValueList.Add(new ACValue(nameof(Route), typeof(Route), null, Global.ParamOption.Required));
            paramTranslation.Add(nameof(Route), "en{'Route'}de{'Route'}");
            method.ParameterValueList.Add(new ACValue("Destination", typeof(Int16), (Int16)0, Global.ParamOption.Required));
            paramTranslation.Add("Destination", "en{'Destination'}de{'Ziel'}");
            method.ParameterValueList.Add(new ACValue("EmptyWeight", typeof(Double?), null, Global.ParamOption.Optional));
            paramTranslation.Add("EmptyWeight", "en{'Empty weight'}de{'Leergewicht'}");
            method.ParameterValueList.Add(new ACValue("DischargingTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional));
            paramTranslation.Add("DischargingTime", "en{'Discharging time'}de{'Entleerzeit'}");
            method.ParameterValueList.Add(new ACValue("PulsationTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional));
            paramTranslation.Add("PulsationTime", "en{'Pulsation time'}de{'Pulszeit'}");
            method.ParameterValueList.Add(new ACValue("PulsationPauseTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional));
            paramTranslation.Add("PulsationPauseTime", "en{'Pulsation pause time'}de{'Puls-Pause Zeit'}");
            method.ParameterValueList.Add(new ACValue("IdleCurrent", typeof(Double), (Double)0.0, Global.ParamOption.Optional)); // Leerlaufstrom
            paramTranslation.Add("IdleCurrent", "en{'Idle current'}de{'Leerlaufstrom'}");
            method.ParameterValueList.Add(new ACValue("InterDischarging", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("InterDischarging", "en{'Inter discharging'}de{'Zwischenentleerung'}");
            method.ParameterValueList.Add(new ACValue("Vibrator", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add("Vibrator", "en{'Vibrator'}de{'Rüttler'}");
            method.ParameterValueList.Add(new ACValue("Power", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("Power", "en{'Power'}de{'Leistung'}");
            method.ParameterValueList.Add(new ACValue("Tolerance", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("Tolerance", "en{'Tolerance+'}de{'Toleranz+'}");
            method.ParameterValueList.Add(new ACValue("ToleranceMin", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("ToleranceMin", "en{'Tolerance-'}de{'Toleranz-'}");
            method.ParameterValueList.Add(new ACValue(PWMethodVBBase.IsLastBatchParamName, typeof(Int16), (Int16)0, Global.ParamOption.Optional));
            paramTranslation.Add(PWMethodVBBase.IsLastBatchParamName, "en{'Last Batch?'}de{'Letzter Batch?'}");
            method.ParameterValueList.Add(new ACValue("Sieve", typeof(Int16), (Int16)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("Sieve", "en{'Sieve'}de{'Sieb'}");
            method.ParameterValueList.Add(new ACValue("TargetQuantity", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("TargetQuantity", "en{'Target quantity (Set value if negative)'}de{'Sollmenge (Setze Wert falls negativ)'}");
            method.ParameterValueList.Add(new ACValue("ScaleBatchWeight", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("ScaleBatchWeight", "en{'Scale batch weight (SWT tip weight)'}de{'Kippgewicht Waage (SWT)'}");
            method.ParameterValueList.Add(new ACValue("HandOver", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("HandOver", "en{'Hand over active function to next Batch'}de{'Übergebe aktive Funktion an nächsten Batch'}");

            if (acIdentifier == "DischargingIntake")
            {
                method.ParameterValueList.Add(new ACValue("Scale", typeof(Int16), (Int16)0, Global.ParamOption.Optional));
                paramTranslation.Add("Scale", "en{'Scale'}de{'Waage'}");
            }

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ActualQuantity", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            resultTranslation.Add("ActualQuantity", "en{'Actual quantity'}de{'Istmenge'}");
            method.ResultValueList.Add(new ACValue("ScaleTotalWeight", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            resultTranslation.Add("ScaleTotalWeight", "en{'Total quantity'}de{'Gesamtgewicht'}");
            method.ResultValueList.Add(new ACValue("DischargingTime", typeof(TimeSpan), TimeSpan.Zero, Global.ParamOption.Optional));
            resultTranslation.Add("DischargingTime", "en{'Discharging time'}de{'Entleerzeit'}");
            method.ResultValueList.Add(new ACValue("GaugeCode", typeof(string), "", Global.ParamOption.Optional));
            resultTranslation.Add("GaugeCode", "en{'Gauge code/Alibi-No.'}de{'Wägeid/Alibi-No.'}");
            //method.ResultValueList.Add(new ACValue("GaugeCodeStart", typeof(string), "", Global.ParamOption.Optional));
            //resultTranslation.Add("GaugeCodeStart", "en{'Gauge code start/Alibi-No.'}de{'Wägeid Start/Alibi-No.'}");
            //method.ResultValueList.Add(new ACValue("GaugeCodeEnd", typeof(string), "", Global.ParamOption.Optional));
            //resultTranslation.Add("GaugeCodeEnd", "en{'Gauge code end/Alibi-No.'}de{'Wägeid Ende/Alibi-No.'}");


            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        public override void AcknowledgeAlarms()
        {
            if (!IsEnabledAcknowledgeAlarms())
                return;
            if (StateTolerance.ValueT != PANotifyState.Off)
                FaultAckTolerance.ValueT = true;
            if (StateDestinationFull.ValueT != PANotifyState.Off)
            {
                if (_StateDestinationFullForced == true)
                {
                    StateDestinationFull.ValueT = PANotifyState.Off;
                    _StateDestinationFullForced = false;
                }
                else
                    FaultAckDestinationFull.ValueT = true;
            }
            base.AcknowledgeAlarms();
        }

        public override bool IsEnabledAcknowledgeAlarms()
        {
            if (   (StateTolerance.ValueT != PANotifyState.Off) && (!FaultAckTolerance.ValueT)
                || (StateDestinationFull.ValueT != PANotifyState.Off && !FaultAckDestinationFull.ValueT))
                return true;
            return base.IsEnabledAcknowledgeAlarms();
        }

        [ACMethodInfo("Function", "en{'Default discharging paramters'}de{'Standard Entleerparameter'}", 9999)]
        public virtual void SetDefaultACMethodValues(ACMethod newACMethod)
        {
        }

        [ACMethodInfo("Function", "en{'Inherirt params from config'}de{'Übernehme Dosierparameter aus Konfiguration'}", 9999)]
        public virtual void InheritParamsFromConfig(ACMethod newACMethod, ACMethod configACMethod, bool isConfigInitialization)
        {
            if (isConfigInitialization)
            {
                object valueDestination = null;
                ACValue acValue = newACMethod.ParameterValueList.GetACValue("Destination");
                if (acValue != null)
                    valueDestination = acValue.Value;

                newACMethod.ParameterValueList.CopyValues(configACMethod.ParameterValueList);

                try
                {
                    if (acValue != null)
                        newACMethod.ParameterValueList["Destination"] = valueDestination;
                    newACMethod.ParameterValueList[nameof(Route)] = null;
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    Messages.LogException("PAFDischarging", "InheritParamsFromConfig", msg);
                }

                //newACMethod["EmptyWeight"] = configACMethod.ParameterValueList.GetACValue("EmptyWeight").Value;
                //newACMethod["DischargingTime"] = configACMethod.ParameterValueList.GetTimeSpan("DischargingTime");
                //newACMethod["PulsationTime"] = configACMethod.ParameterValueList.GetTimeSpan("PulsationTime");
                //newACMethod["PulsationPauseTime"] = configACMethod.ParameterValueList.GetTimeSpan("PulsationPauseTime");
                //newACMethod["IdleCurrent"] = configACMethod.ParameterValueList.GetDouble("IdleCurrent");
                //newACMethod["InterDischarging"] = configACMethod.ParameterValueList.GetInt16("InterDischarging");
                //newACMethod["Vibrator"] = configACMethod.ParameterValueList.GetInt16("Vibrator");
                //newACMethod["Power"] = configACMethod.ParameterValueList.GetDouble("Power");
                //newACMethod["Tolerance"] = configACMethod.ParameterValueList.GetDouble("Tolerance");
            }
            else
            {
                var acValue = newACMethod.ParameterValueList.GetACValue("EmptyWeight");
                if (!acValue.ValueT<Double?>().HasValue)
                {
                    acValue = configACMethod.ParameterValueList.GetACValue("EmptyWeight");
                    newACMethod["EmptyWeight"] = acValue.Value;
                }
                if (newACMethod.ParameterValueList.GetTimeSpan("DischargingTime") == TimeSpan.Zero)
                    newACMethod["DischargingTime"] = configACMethod.ParameterValueList.GetTimeSpan("DischargingTime");
                if (newACMethod.ParameterValueList.GetTimeSpan("PulsationTime") == TimeSpan.Zero)
                    newACMethod["PulsationTime"] = configACMethod.ParameterValueList.GetTimeSpan("PulsationTime");
                if (newACMethod.ParameterValueList.GetTimeSpan("PulsationPauseTime") == TimeSpan.Zero)
                    newACMethod["PulsationPauseTime"] = configACMethod.ParameterValueList.GetTimeSpan("PulsationPauseTime");
                if (newACMethod.ParameterValueList.GetDouble("IdleCurrent") <= 0.000001)
                    newACMethod["IdleCurrent"] = configACMethod.ParameterValueList.GetDouble("IdleCurrent");
                if (newACMethod.ParameterValueList.GetInt16("InterDischarging") <= 0)
                    newACMethod["InterDischarging"] = configACMethod.ParameterValueList.GetInt16("InterDischarging");
                if (newACMethod.ParameterValueList.GetInt16("Vibrator") <= 0)
                    newACMethod["Vibrator"] = configACMethod.ParameterValueList.GetInt16("Vibrator");
                if (newACMethod.ParameterValueList.GetDouble("Power") <= 0.000001)
                    newACMethod["Power"] = configACMethod.ParameterValueList.GetDouble("Power");
                if (newACMethod.ParameterValueList.GetDouble("Tolerance") <= 0.000001)
                    newACMethod["Tolerance"] = configACMethod.ParameterValueList.GetDouble("Tolerance");
            }
        }
        #endregion

    }

}
