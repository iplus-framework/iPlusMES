using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.core.autocomponent;
using vd = gip.mes.datamodel;
using gip.core.processapplication;
using gip.mes.datamodel;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Manual weighing'}de{'Handzugabe'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, true, PWInfoACClass = PWManualWeighing.PWClassName, BSOConfig = "BSOManualWeighing", SortIndex = 100)]
    public class PAFManualWeighing : PAProcessFunction, IPAFuncScaleConfig
    {
        #region Constructors

        public const string ClassName = "PAFManualWeighing";
        public const string VMethodName_ManualWeighing = "ManualWeighing";

        static PAFManualWeighing()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFManualWeighing), ACStateConst.TMStart, CreateVirtualMethod(VMethodName_ManualWeighing, "en{'Manual weighing'}de{'Handzugabe'}", typeof(PWManualWeighing)));
            RegisterExecuteHandler(typeof(PAFManualWeighing), HandleExecuteACMethod_PAFManualWeighing);
        }

        public PAFManualWeighing(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _FuncScaleConfig = new ACPropertyConfigValue<string>(this, PAScaleMappingHelper<IACComponent>.FuncScaleConfigName, "");
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool result = base.ACInit(startChildMode);
            _ = FuncScaleConfig;
            return result;
        }

        public override bool ACPostInit()
        {
            if (ScaleMappingHelper.AssignedScales.Any())
            {
                _HasParentPMMultipleScaleObjects = ScaleMappingHelper.AssignedScales.OfType<PAEScaleGravimetric>().Count() > 1;
            }
            else
            {
                _HasParentPMMultipleScaleObjects = ParentACComponent?.FindChildComponents<PAEScaleGravimetric>().Count > 1;
            }
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            CurrentWeighingMaterial = null;
            ReleaseActiveScaleObject();
            _IsManuallyCompleted = false;
            _VerifyQuantity = 0;
            _HasParentPMMultipleScaleObjects = false;

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

        #region Properties

        #region Properties => Fields

        protected double? _SimulationScaleIncrement = null;

        private readonly ACMonitorObject _65000_IsManCompLock = new ACMonitorObject(65000);

        private bool _IsManuallyCompleted = false;

        private double _VerifyQuantity = 0;

        private bool _HasParentPMMultipleScaleObjects = false;

        #endregion

        [ACPropertyBindingTarget(IsPersistable = true)]
        public IACContainerTNet<double> ManuallyAddedQuantity
        {
            get;
            set;
        }

        [ACPropertyBindingTarget]
        public IACContainerTNet<string> CurrentWeighingMaterial
        {
            get;
            set;
        }

        [ACPropertyInfo(400)]
        public PWManualWeighing ManualWeighingPW
        {
            get
            {
                if (CurrentTask != null && CurrentTask.ValueT != null)
                    return CurrentTask.ValueT as PWManualWeighing;
                return null;
            }
        }

        [ACPropertyBindingSource(IsPersistable = true)]
        public IACContainerTNet<short> TareScaleState
        {
            get;
            set;
        }

        private PAEScaleGravimetric _ActiveScaleObject;
        public PAEScaleGravimetric ActiveScaleObject
        {
            get
            {
                return _ActiveScaleObject;
            }
            set
            {
                _ActiveScaleObject = value;
            }
        }

        public virtual bool TareCheck => true;

        public virtual bool CheckInToleranceOnlyManuallyAddedQuantity => false;

        public virtual bool SimulateWeightIfSimulationOn => true;

        public PAEScaleBase CurrentScaleForWeighing => ActiveScaleObject;

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

        #endregion

        #region Methods

        public Msg RunManualWeighing()
        {
            if (!CanStart())
            {
                return new Msg(eMsgLevel.Error, "Can't start Manual weighing function!");
            }

            if (!CheckInToleranceOnlyManuallyAddedQuantity)
                DetermineTargetScaleObject();

            CurrentACState = ACStateEnum.SMRunning;

            return null;
        }

        private bool OccupyAndSetActiveScaleObject(PAEScaleGravimetric scale)
        {
            if (scale == null)
                return false;
            //PAEScaleGravimetric occupiedScale = ActiveScaleObject;
            //if (occupiedScale == scale && occupiedScale.IsScaleOccupied() == this.GetACUrl())
            //    return true;
            if (scale.OccupyScale(this.GetACUrl(), true, false))
            {
                ActiveScaleObject = scale;
                return true;
            }
            else
            {
                ActiveScaleObject = null;
                return false;
            }
        }

        private bool ReleaseActiveScaleObject()
        {
            PAEScaleGravimetric scale = ActiveScaleObject;
            if (scale == null)
                return true;
            if (scale.OccupyScale(this.GetACUrl(), true, true))
            {
                //ActiveScaleObject = null;
                return true;
            }
            else
                return false;
        }

        [ACMethodInfo("", "", 999)]
        public void SetActiveScaleObject(string scaleACIdentifier)
        {
            if (!CheckInToleranceOnlyManuallyAddedQuantity)
            {
                if (ActiveScaleObject == null || ActiveScaleObject.ACIdentifier != scaleACIdentifier)
                {
                    PAEScaleGravimetric targetScale = ParentACComponent.ACUrlCommand(scaleACIdentifier) as PAEScaleGravimetric;
                    if (targetScale != null)
                        OccupyAndSetActiveScaleObject(targetScale);
                }
            }
        }

        [ACMethodInfo("", "", 999)]
        public string GetActiveScaleObjectACUrl()
        {
            return ActiveScaleObject?.ACUrl;
        }

        [ACMethodInfo("", "", 400)]
        public ACValueList GetAvailableScaleObjects()
        {
            var assignedScaleObjects = ScaleMappingHelper.AssignedScales;
            if (assignedScaleObjects == null || !assignedScaleObjects.Any())
                return null;

            return new ACValueList(assignedScaleObjects.Select(c => new ACValue(c.ComponentClass.ACUrlComponent, c.ComponentClass.ACClassID)).ToArray());
        }

        internal void CompleteWeighing(double actualQuantity, bool isForBinChange = false)
        {
            using (ACMonitor.Lock(_65000_IsManCompLock))
            {
                _VerifyQuantity = actualQuantity;
                _IsManuallyCompleted = true;
            }

            if (isForBinChange)
            {
                PAEScaleGravimetric scale = ActiveScaleObject;
                if (scale != null && TareScaleState.ValueT == (short)TareScaleStateEnum.TareOK)
                {
                    SetPAFResult(scale.ActualWeight.ValueT + ManuallyAddedQuantity.ValueT);
                    scale.TareScale(false, IsSimulationOn);
                    TareScaleState.ValueT = (short)TareScaleStateEnum.None;
                }
                else
                    SetPAFResult(ManuallyAddedQuantity.ValueT);

                CurrentACState = ACStateEnum.SMCompleted;
            }
        }

        internal void SetPAFResult(double actualQuantity, bool isComponentConsumed = false)
        {
            CurrentACMethod.ValueT.ResultValueList["ActualQuantity"] = actualQuantity;
            CurrentACMethod.ValueT.ResultValueList["IsComponentConsumed"] = isComponentConsumed;
        }

        [ACMethodInfo("Function", "en{'Default dosing parameters'}de{'Standard Dosierparameter'}", 9999)]
        public virtual void SetDefaultACMethodValues(ACMethod newACMethod)
        {
            newACMethod["TargetQuantity"] = (Double)0;
            newACMethod["TolerancePlus"] = (Double)0.0;
            newACMethod["ToleranceMinus"] = (Double)0.0;
            newACMethod["IsLastWeighingMaterial"] = false;
        }

        [ACMethodInfo("Function", "en{'Inherirt params from config'}de{'Übernehme Dosierparameter aus Konfiguration'}", 9999)]
        public virtual void InheritParamsFromConfig(ACMethod newACMethod, ACMethod configACMethod, bool isConfigInitialization)
        {
            if (isConfigInitialization)
            {
                newACMethod.ParameterValueList.CopyValues(configACMethod.ParameterValueList);

                try
                {
                    newACMethod.ParameterValueList[vd.Material.ClassName] = "";
                    newACMethod.ParameterValueList["PLPosRelation"] = Guid.Empty;
                    newACMethod.ParameterValueList["FacilityCharge"] = Guid.Empty;
                    newACMethod.ParameterValueList["Facility"] = Guid.Empty;
                    newACMethod.ParameterValueList["Route"] = null;
                    newACMethod.ParameterValueList["TargetQuantity"] = (double)0.0;
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    Messages.LogException("PAFDosing", "InheritParamsFromConfig", msg);
                }
            }
            else
            {
                double targetQ = newACMethod.ParameterValueList.GetDouble("TargetQuantity");
                double tolPlus = newACMethod.ParameterValueList.GetDouble("TolerancePlus");
                if (Math.Abs(tolPlus) <= Double.Epsilon)
                    tolPlus = configACMethod.ParameterValueList.GetDouble("TolerancePlus");

                tolPlus = PAFDosing.RecalcAbsoluteTolerance(tolPlus, targetQ, null);
                newACMethod["TolerancePlus"] = tolPlus;

                double tolMinus = newACMethod.ParameterValueList.GetDouble("ToleranceMinus");
                if (Math.Abs(tolMinus) <= Double.Epsilon)
                    tolMinus = configACMethod.ParameterValueList.GetDouble("ToleranceMinus");

                tolMinus = PAFDosing.RecalcAbsoluteTolerance(tolMinus, targetQ, null);
                newACMethod["ToleranceMinus"] = tolMinus;
            }
        }

        #region Methods => Overrides

        protected override MsgWithDetails CompleteACMethodOnSMStarting(ACMethod acMethod)
        {
            string matName = CurrentACMethod.ValueT[vd.Material.ClassName] as string;
            CurrentWeighingMaterial.ValueT = matName;

            ACValue value = acMethod.ParameterValueList.GetACValue("Route");
            if (value == null)
            {
                MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "CompleteACMethodOnSMStarting(1)", Message = "Route is empty." };
                return msg;
            }
            Route route = value.ValueT<Route>();
            RouteItem sourceRouteItem = route.FirstOrDefault();
            if (sourceRouteItem == null)
            {
                if (CurrentACState == ACStateEnum.SMRunning)
                {
                    MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "CompleteACMethodOnSMStarting(2)", Message = "Last RouteItem is null." };
                    return msg;
                }
                return null;
            }

            using (var db = new Database())
            {
                try
                {
                    route.AttachTo(db);
                    MsgWithDetails msg = GetACMethodFromConfig(db, route, acMethod);
                    if (msg != null)
                        return msg;
                }
                catch (Exception e)
                {
                    MsgWithDetails msg = new MsgWithDetails() { Source = this.GetACUrl(), MessageLevel = eMsgLevel.Error, ACIdentifier = "CompleteACMethodOnSMStarting(3)", Message = e.Message };
                    return msg;
                }
                finally
                {
                    if (route != null)
                        route.Detach(true);
                    //sourceRouteItem.DetachEntitesFromDbContext();
                }
            }

            //var targetQ = acMethod.ParameterValueList.GetACValue("TargetQuantity");
            //if (targetQ != null)
            //{
            //    double targetQuantity = targetQ.ParamAsDouble;
            //    var scale = ParentACComponent as IPAMContScale;
            //    if (scale != null)
            //    {
            //        double? remainingWeight = null;
            //        if (scale.RemainingWeightCapacity.HasValue)
            //            remainingWeight = scale.RemainingWeightCapacity.Value;
            //        else if ((scale as PAProcessModuleVB).MaxWeightCapacity.ValueT > 0.00000001)
            //            remainingWeight = (scale as PAProcessModuleVB).MaxWeightCapacity.ValueT;

            //        if (remainingWeight.HasValue
            //            && (targetQuantity > (remainingWeight.Value * 1.01))) // Toleranz 1%
            //        {
            //            MsgWithDetails msg = new MsgWithDetails()
            //            {
            //                Source = this.GetACUrl(),
            //                MessageLevel = eMsgLevel.Error,
            //                ACIdentifier = "CompleteACMethodOnSMStarting(4)",
            //                Message = String.Format("TargetQuantity of {0} kg exceeds the remaining scale capacity!", targetQuantity)
            //            };
            //            return msg;
            //        }
            //    }
            //}
            return null;
        }

        public override void InitializeRouteAndConfig(Database dbIPlus)
        {
            gip.core.datamodel.ACClass thisACClass = this.ComponentClass;
            gip.core.datamodel.ACClass parentACClass = ParentACComponent.ComponentClass;
            try
            {
                var parentModule = ACRoutingService.DbSelectRoutesFromPoint(dbIPlus, thisACClass, this.PAPointMatIn1.PropertyInfo, (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID == parentACClass.ACClassID, null, RouteDirections.Backwards, true, false).FirstOrDefault();
                var sourcePoint = parentModule?.FirstOrDefault()?.SourceACPoint?.PropertyInfo;
                if (sourcePoint == null)
                    return;

                var routes = ACRoutingService.DbSelectRoutesFromPoint(dbIPlus, parentACClass, sourcePoint, (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != parentACClass.ACClassID, null, RouteDirections.Backwards, true, false);
                if (routes != null && routes.Any())
                {
                    string virtMethodName = VMethodName_ManualWeighing;
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
            if (route == null || !route.Any())
            {
                //Error50360: The route is null or empty.
                return new MsgWithDetails(this, eMsgLevel.Error, ClassName, "GetACMethodFromConfig(10)", 446, "Error50360");
            }
            if (IsMethodChangedFromClient)
                return null;
            RouteItem targetRouteItem = route.LastOrDefault();
            if (targetRouteItem.Target.ACKind == Global.ACKinds.TPAProcessFunction)
            {
                if (route.Count < 2)
                {
                    //Error50361: The route has not enough route items.
                    return new MsgWithDetails(this, eMsgLevel.Error, ClassName, "GetACMethodFromConfig(20)", 456, "Error50361");
                }
                targetRouteItem = route[route.Count - 2];
            }
            RouteItem sourceRouteItem = route.FirstOrDefault();

            List<vd.MaterialConfig> materialConfigList = null;
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
                logicalRelation.TargetACClassPropertyID = targetRouteItem.TargetProperty.ACClassPropertyID;
                logicalRelation.ConnectionType = Global.ConnectionTypes.DynamicConnection;
            }
            else
            {
                config = logicalRelation.ACClassConfig_ACClassPropertyRelation.FirstOrDefault();
                if (!isConfigInitialization)
                {
                    PAMSilo pamSilo = sourceRouteItem.SourceACComponent as PAMSilo;
                    if (pamSilo != null)
                    {
                        if (pamSilo.Facility != null && pamSilo.Facility.ValueT != null && pamSilo.Facility.ValueT.ValueT != null)
                        {
                            Guid? materialID = pamSilo.Facility.ValueT.ValueT.MaterialID;
                            if (materialID.HasValue && materialID != Guid.Empty)
                            {
                                Guid acClassIdOfParent = ParentACComponent.ComponentClass.ACClassID;
                                using (var dbApp = new vd.DatabaseApp())
                                {
                                    // 1. Hole Material-Konfiguration spezielle für diesen Weg
                                    materialConfigList = dbApp.MaterialConfig.Where(c => c.VBiACClassPropertyRelationID == logicalRelation.ACClassPropertyRelationID && c.MaterialID == materialID.Value).SetMergeOption(System.Data.Objects.MergeOption.NoTracking).ToList();
                                    var wayIndependent = dbApp.MaterialConfig.Where(c => c.MaterialID == materialID.Value && c.VBiACClassID == acClassIdOfParent).SetMergeOption(System.Data.Objects.MergeOption.NoTracking);
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

        protected override bool CyclicWaitIfSimulationOn()
        {
            PAEScaleGravimetric scale = ActiveScaleObject;
            if (scale != null)
            {
                var targetQ = CurrentACMethod.ValueT.ParameterValueList.GetACValue("TargetQuantity");
                double targetQuantity = 0;
                if (targetQ != null)
                    targetQuantity = targetQ.ParamAsDouble;

                SubscribeToProjectWorkCycle();

                if (IsSimulationOn && ( ApplicationManager != null && !ApplicationManager.IsManualSimulation) 
                                   && TareScaleState.ValueT == (short)TareScaleStateEnum.TareOK)
                {
                    if (!_SimulationScaleIncrement.HasValue)
                    {
                        Random random = new Random();
                        int steps = random.Next(5, 20);
                        _SimulationScaleIncrement = targetQuantity / steps;
                    }

                    double actWeight = Math.Round(scale.ActualWeight.ValueT + ManuallyAddedQuantity.ValueT, 3);
                    double tQuantity = Math.Round(targetQuantity, 3);

                    if (actWeight < tQuantity)
                    {
                        if (actWeight + _SimulationScaleIncrement.Value >= tQuantity)
                            _SimulationScaleIncrement = tQuantity - actWeight;

                        scale.SimulateWeight(_SimulationScaleIncrement.Value);
                        return true;
                    }
                }
            }
            _SimulationScaleIncrement = null;
            return false;
        }

        protected override CompleteResult CompleteResultAndCallback(Global.ACMethodResultState resultState)
        {
            return base.CompleteResultAndCallback(resultState);
        }

        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        //protected override void OnChangingCurrentACMethod(ACMethod currentACMethod, ACMethod newACMethod)
        //{
        //    base.OnChangingCurrentACMethod(currentACMethod, newACMethod);

        //    Guid? fc_f = newACMethod?.ParameterValueList["FacilityCharge"] as Guid?;
        //    if (fc_f.HasValue)
        //    {
        //        Guid? plPosRelation = newACMethod.ParameterValueList["PLPosRelation"] as Guid?;
        //        if (plPosRelation.HasValue)
        //        {
        //            using (DatabaseApp dbApp = new DatabaseApp())
        //            {
        //                ProdOrderPartslistPosRelation rel = dbApp.ProdOrderPartslistPosRelation.Include(c => c.SourceProdOrderPartslistPos.Material)
        //                                                                                  .FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == plPosRelation.Value);
        //                if (rel != null)
        //                {
        //                    Material mat = rel.SourceProdOrderPartslistPos.Material;
        //                    if (mat != null)
        //                    {
        //                        FacilityCharge fc = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == fc_f.Value);
        //                        if (fc != null)
        //                        {
        //                            if (fc.MaterialID != mat.MaterialID)
        //                            {
        //                                Messages.LogError(this.GetACUrl(), "Wrong quant(A20)", String.Format("Quant is wrong, ID:{0}, material ID: {1}, function state:{2}",
        //                                                                                       fc.FacilityChargeID, mat.MaterialID, CurrentACState));

        //                                string stackTrace = System.Environment.StackTrace;
        //                                Messages.LogError(this.GetACUrl(), "Wrong quant(A21)", stackTrace);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion

        #region Methods => ACState

        public override void SMStarting()
        {
            SubscribeToProjectWorkCycle();

            if (!CanStart())
            {
                UnSubscribeToProjectWorkCycle();
                return;
            }

            if (!CheckInToleranceOnlyManuallyAddedQuantity)
                DetermineTargetScaleObject();

            base.SMStarting();
        }

        private bool CanStart()
        {
            if (CurrentACMethod == null || CurrentACMethod.ValueT == null)
                return false;

            Guid? fc_f = CurrentACMethod.ValueT.ParameterValueList["FacilityCharge"] as Guid?;
            if (fc_f.HasValue)
            {
                Guid? plPosRelation = CurrentACMethod.ValueT.ParameterValueList["PLPosRelation"] as Guid?;
                if (plPosRelation.HasValue)
                {
                    if (fc_f.Value == Guid.Empty)
                    {
                        Messages.LogError(this.GetACUrl(), "Wrong quant(A10)", "Quant is empty");
                        //if (ManualWeighingPW != null)
                        //{
                        //    var temp = ManualWeighingPW.CurrentFacilityCharge;
                        //    Messages.LogError(this.GetACUrl(), "Wrong quant(A10)", String.Format("PW CurrentFacilityCharge is {0}", temp));
                        //}
                    }
                    //else
                    //{
                    //    using (DatabaseApp dbApp = new DatabaseApp())
                    //    {
                    //        ProdOrderPartslistPosRelation rel = dbApp.ProdOrderPartslistPosRelation.Include(c => c.SourceProdOrderPartslistPos.Material)
                    //                                                                          .FirstOrDefault(c => c.ProdOrderPartslistPosRelationID == plPosRelation.Value);
                    //        if (rel != null)
                    //        {
                    //            Material mat = rel.SourceProdOrderPartslistPos.Material;
                    //            if (mat != null)
                    //            {
                    //                FacilityCharge fc = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == fc_f.Value);
                    //                if (fc != null)
                    //                {
                    //                    if (fc.MaterialID != mat.MaterialID)
                    //                    {
                    //                        Messages.LogError(this.GetACUrl(), "Wrong quant(A15)", String.Format("Quant is wrong, ID:{0}, material ID: {1}",
                    //                                                                               fc.FacilityChargeID, mat.MaterialID));
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }
                return true;
            }

            fc_f = CurrentACMethod.ValueT.ParameterValueList["Facility"] as Guid?;

            return fc_f.HasValue;
        }

        private void DetermineTargetScaleObject()
        {
            if (!_HasParentPMMultipleScaleObjects)
            {
                IEnumerable<PAEScaleGravimetric> scalesGravimetric = ScaleMappingHelper.AssignedScales.OfType<PAEScaleGravimetric>();
                if (scalesGravimetric.Any())
                {
                    var scale = scalesGravimetric.FirstOrDefault();

                    bool isOccupied = OccupyAndSetActiveScaleObject(scale);
                    if (!isOccupied)
                    {
                        //Error50554: The scale is currently occupied from a another work place. Please wait until this alarm disappears, then you can continue with weighing.
                        Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "DetermineTargetScaleObject(10)", 697, "Error50554");
                        OnNewAlarmOccurred(FunctionError, msg, true);
                        if (IsAlarmActive(FunctionError, msg.Message) == null)
                            Messages.LogMessageMsg(msg);
                    }
                    else
                    {
                        Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "DetermineTargetScaleObject(10)", 697, "Error50554");
                        if (IsAlarmActive(FunctionError, msg.Message) != null)
                        {
                            AcknowledgeAlarms();
                        }
                    }
                }
                else
                {
                    var scale = ParentACComponent as IPAMContScale;
                    OccupyAndSetActiveScaleObject(scale?.Scale);
                }
                return;
            }

            if (CurrentACMethod.ValueT == null)
                return;

            var targetScale = CurrentACMethod.ValueT.ParameterValueList.GetACValue("TargetScaleACIdentifier");
            string targetScaleACIdentifier = null;
            if (targetScale != null)
                targetScaleACIdentifier = targetScale.ParamAsString;

            if (!string.IsNullOrEmpty(targetScaleACIdentifier))
            {
                PAEScaleGravimetric scale = null;

                if (ActiveScaleObject != null && ActiveScaleObject.ACIdentifier == targetScaleACIdentifier)
                    return;

                if (ScaleMappingHelper.AssignedScales != null && ScaleMappingHelper.AssignedScales.Any())
                {
                    scale = ScaleMappingHelper.AssignedScales.FirstOrDefault(c => c.ACIdentifier == targetScaleACIdentifier) as PAEScaleGravimetric;
                }
                else
                {
                    scale = ParentACComponent?.FindChildComponents<PAEScaleGravimetric>(c => c is PAEScaleGravimetric && c.ACIdentifier == targetScaleACIdentifier)
                                                                 .FirstOrDefault();
                }
                if (scale == null)
                {
                    //Error50362: The scale object with ACIdentifier: {0} can not be found! Please check your scale configuration!
                    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "DetermineTargetScaleObject(10)", 697, "Error50362", targetScaleACIdentifier);
                    OnNewAlarmOccurred(FunctionError, msg, true);
                    if (IsAlarmActive(FunctionError, msg.Message) == null)
                        Messages.LogMessageMsg(msg);
                }
                bool isOccupied = OccupyAndSetActiveScaleObject(scale);
                if (!isOccupied)
                {
                    //Error50554: The scale is currently occupied from a another work place. Please wait until this alarm disappears, then you can continue with weighing.
                    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "DetermineTargetScaleObject(10)", 697, "Error50554");
                    OnNewAlarmOccurred(FunctionError, msg, true);
                    if (IsAlarmActive(FunctionError, msg.Message) == null)
                        Messages.LogMessageMsg(msg);
                }
                else
                {
                    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "DetermineTargetScaleObject(10)", 697, "Error50554");
                    if (IsAlarmActive(FunctionError, msg.Message) != null)
                    {
                        AcknowledgeAlarms();
                    }
                }

                return;
            }

            var targetQ = CurrentACMethod.ValueT.ParameterValueList.GetACValue("TargetQuantity");
            double targetQuantity = 0;
            if (targetQ != null)
                targetQuantity = targetQ.ParamAsDouble;

            if (targetQuantity > 0)
            {
                IEnumerable<PAEScaleGravimetric> possibleScales;
                if (ScaleMappingHelper.AssignedScales.Any())
                {
                    possibleScales = ScaleMappingHelper.AssignedScales.OfType<PAEScaleGravimetric>();
                }
                else
                {
                    possibleScales = ParentACComponent.FindChildComponents<PAEScaleGravimetric>(c => c is PAEScaleGravimetric);
                }

                PAEScaleGravimetric scale = null;

                if (ScaleMappingHelper.AssignedScales.Any())
                {
                    scale = possibleScales.Where(c => c.MinDosingWeight.ValueT <= targetQuantity && targetQuantity <= c.MaxScaleWeight.ValueT).FirstOrDefault();

                    if (scale == null)
                        scale = possibleScales.OrderByDescending(c => c.MaxScaleWeight.ValueT).FirstOrDefault();
                }
                else
                {
                    scale = possibleScales.Where(c => c.MinDosingWeight?.ValueT <= targetQuantity && targetQuantity <= c.MaxScaleWeight.ValueT).FirstOrDefault();
                    if (scale == null)
                        scale = possibleScales.OrderByDescending(c => c.MaxScaleWeight.ValueT).FirstOrDefault();
                }
                if (scale == null)
                {
                    //Error50363: Can not determine appropriate scale object. Please check your scale configuration (MinDosingWeight, MaxScaleWeight or FunctionScaleConfiguration).
                    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "DetermineTargetScaleObject(20)", 737, "Error50363");
                    OnNewAlarmOccurred(FunctionError, msg, true);
                    if (IsAlarmActive(FunctionError, msg.Message) == null)
                        Messages.LogMessageMsg(msg);

                    return;
                }
                bool isOccupied = OccupyAndSetActiveScaleObject(scale);
                if (!isOccupied)
                {
                    scale = possibleScales.FirstOrDefault(c => c.ACUrl != scale.ACUrl && c.MinDosingWeight.ValueT <= targetQuantity);
                    if (scale == null)
                    {
                        //Error50554: The scale is currently occupied from a another work place. Please wait until this alarm disappears, then you can continue with weighing.
                        Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "DetermineTargetScaleObject(10)", 697, "Error50554");
                        OnNewAlarmOccurred(FunctionError, msg, true);
                        if (IsAlarmActive(FunctionError, msg.Message) == null)
                            Messages.LogMessageMsg(msg);

                        return;
                    }

                    isOccupied = OccupyAndSetActiveScaleObject(scale);
                    if (!isOccupied)
                    {
                        //Error50554: The scale is currently occupied from a another work place. Please wait until this alarm disappears, then you can continue with weighing.
                        Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "DetermineTargetScaleObject(10)", 697, "Error50554");
                        OnNewAlarmOccurred(FunctionError, msg, true);
                        if (IsAlarmActive(FunctionError, msg.Message) == null)
                            Messages.LogMessageMsg(msg);

                        return;
                    }
                }

                if (isOccupied)
                {
                    Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "DetermineTargetScaleObject(10)", 697, "Error50554");
                    if (IsAlarmActive(FunctionError, msg.Message) != null)
                    {
                        AcknowledgeAlarms();
                    }

                    CurrentACMethod.ValueT.ParameterValueList["TargetScaleACIdentifier"] = scale.ACIdentifier;
                }
            }
        }

        public override void SMRunning()
        {
            if (FuncConvMode == (short)FuncConverterMode.OnlyOnPLCSide)
            {
                UnSubscribeToProjectWorkCycle();
                return;
            }

            SubscribeToProjectWorkCycle();

            if (ParentTaskExecComp != null && CurrentACMethod.ValueT != null && LastACState != CurrentACState && LastACState == ACStateEnum.SMStarting)
                ParentTaskExecComp.CallbackTask(CurrentACMethod.ValueT, CreateNewMethodEventArgs(CurrentACMethod.ValueT, Global.ACMethodResultState.InProcess), PointProcessingState.Accepted);

            if (!Root.Initialized)
                return;

            if (!CheckInToleranceOnlyManuallyAddedQuantity)
                DetermineTargetScaleObject();

            if (TareCheck && !WaitForTareScale())
                return;

            //TareScaleState.ValueT = true;

            if (SimulateWeightIfSimulationOn && CyclicWaitIfSimulationOn())
                return;

            if (!CheckIsScaleInTol())
                return;

            if (WaitForAcknowledge())
                return;

            PAEScaleGravimetric scale = ActiveScaleObject;
            if (scale != null && !CheckInToleranceOnlyManuallyAddedQuantity)
            {
                SetPAFResult(scale.ActualWeight.ValueT + ManuallyAddedQuantity.ValueT);
                scale.TareScale(false, IsSimulationOn);
            }
            else
                SetPAFResult(ManuallyAddedQuantity.ValueT);

            UnSubscribeToProjectWorkCycle();

            TareScaleState.ValueT = (short)TareScaleStateEnum.None;

            if (ACStateConverter != null)
                CurrentACState = ACStateConverter.GetNextACState(this);
            else
                CurrentACState = PAFuncStateConvBase.GetDefaultNextACState(CurrentACState);
        }

        private bool WaitForTareScale()
        {
            bool result = false;

            PAEScaleGravimetric scale = ActiveScaleObject;
            if (scale == null)
            {
                //Error50364: The active scale could not be determined.
                Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "WaitForTareScale(10)", 806, "Error50364");
                if (IsAlarmActive(FunctionError, msg.Message) != null)
                {
                    Messages.LogMessageMsg(msg);
                    FunctionError.ValueT = PANotifyState.AlarmOrFault;
                    OnNewAlarmOccurred(FunctionError, msg, true);
                }

                return false;
            }

            if (TareScaleState.ValueT == (short)TareScaleStateEnum.None && ManualWeighingPW != null && ManualWeighingPW.ScaleAutoTare)
                TareScale(scale);

            if (TareScaleState.ValueT == (short)TareScaleStateEnum.TareReq && scale.IsTareRequestAcknowledged)
                TareScaleState.ValueT = (short)TareScaleStateEnum.TareOK;

            if (TareScaleState.ValueT == (short)TareScaleStateEnum.TareOK)
                result = true;

            if (ManualWeighingPW != null)
            {
                if (!result && ManualWeighingPW.ManualWeighingNextTask.ValueT != ManualWeighingTaskInfo.WaitForTare)
                    ManualWeighingPW.ManualWeighingNextTask.ValueT = ManualWeighingTaskInfo.WaitForTare;
                else if (result && ManualWeighingPW.ManualWeighingNextTask.ValueT == ManualWeighingTaskInfo.WaitForTare)
                    ManualWeighingPW.ManualWeighingNextTask.ValueT = ManualWeighingTaskInfo.None;
            }

            return result;
        }

        [ACMethodInfo("", "", 400)]
        public void TareActiveScale()
        {
            TareScale(ActiveScaleObject);
        }

        private void TareScale(PAEScaleGravimetric scale)
        {
            if (scale == null)
                return;

            scale.TareScale(true, IsSimulationOn);
            if (ManualWeighingPW != null)
                TareScaleState.ValueT = (short)TareScaleStateEnum.TareReq;
            else if (TareScaleState.ValueT != (short)TareScaleStateEnum.None)
                TareScaleState.ValueT = (short)TareScaleStateEnum.None;
            scale.TareScale(false, IsSimulationOn);
        }

        protected virtual bool CheckIsScaleInTol()
        {
            bool result = false;

            double checkQuantity = ManuallyAddedQuantity.ValueT;

            PAEScaleGravimetric scale = ActiveScaleObject;
            if (scale != null)
            {
                // If scale is not in stillstand
                if (scale.NotStandStill.ValueT)
                    return false;
                if (!CheckInToleranceOnlyManuallyAddedQuantity)
                    checkQuantity += scale.ActualWeight.ValueT;
            }

            var targetQ = CurrentACMethod.ValueT.ParameterValueList.GetACValue("TargetQuantity");
            double targetQuantity = 0;
            if (targetQ != null)
                targetQuantity = targetQ.ParamAsDouble;
            double tolPlus = (double)CurrentACMethod.ValueT["TolerancePlus"];
            double tolMinus = (double)CurrentACMethod.ValueT["ToleranceMinus"];


            if (scale != null)
            {
                if (Math.Abs(tolPlus) <= Double.Epsilon)
                {
                    tolPlus = scale.TolerancePlus;
                    tolPlus = PAFDosing.RecalcAbsoluteTolerance(tolPlus, targetQuantity);


                    //if (tolPlus < -0.0000001)
                    //{
                    //    if (Math.Abs(targetQuantity) > Double.Epsilon)
                    //        tolPlus = targetQuantity * tolPlus * -0.01;
                    //    else
                    //        tolPlus = 0.001;
                    //}
                    //else if (Math.Abs(tolPlus) <= Double.Epsilon)
                    //{
                    //    if (Math.Abs(targetQuantity) > Double.Epsilon)
                    //        tolPlus = targetQuantity * 0.05;
                    //    else
                    //        tolPlus = 0.001;
                    //}

                    tolPlus = scale.VerifyScaleTolerance(tolPlus);
                    CurrentACMethod.ValueT["TolerancePlus"] = tolPlus;
                }

                if (Math.Abs(tolMinus) <= Double.Epsilon)
                {
                    tolMinus = scale.ToleranceMinus;
                    tolMinus = PAFDosing.RecalcAbsoluteTolerance(tolMinus, targetQuantity);

                    //if (tolMinus < -0.0000001)
                    //{
                    //    if (Math.Abs(targetQuantity) > Double.Epsilon)
                    //        tolMinus = targetQuantity * tolMinus * -0.01;
                    //    else
                    //        tolMinus = 0.001;
                    //}
                    //else if (Math.Abs(tolMinus) <= Double.Epsilon)
                    //{
                    //    if (Math.Abs(targetQuantity) > Double.Epsilon)
                    //        tolMinus = targetQuantity * 0.05;
                    //    else
                    //        tolMinus = 0.001;
                    //}

                    tolMinus = scale.VerifyScaleTolerance(tolMinus);
                    CurrentACMethod.ValueT["ToleranceMinus"] = tolMinus;
                }
            }

            if (scale == null)
            {
                ACValue tolCheck = CurrentACMethod.ValueT.ParameterValueList.GetACValue("SkipToleranceCheck");
                bool? tolCheckValue = tolCheck?.ParamAsBoolean;
                if (tolCheckValue.HasValue && tolCheckValue.Value)
                {
                    return true;
                }
            }

            double actWeight = Math.Round(checkQuantity, 5);

            if (actWeight >= Math.Round(targetQuantity - tolMinus, 5))
            {
                if (actWeight <= Math.Round(targetQuantity + tolPlus, 5))
                    result = true;
            }
            return result;
        }

        protected virtual bool WaitForAcknowledge()
        {
            bool mustWait = true;
            if (ManualWeighingPW != null && ManualWeighingPW.AutoAcknowledge)
                mustWait = false;
            else
                mustWait = !_IsManuallyCompleted;

            if (ManualWeighingPW != null)
            {
                if (mustWait && ManualWeighingPW.ManualWeighingNextTask.ValueT != ManualWeighingTaskInfo.WaitForAcknowledge)
                    ManualWeighingPW.ManualWeighingNextTask.ValueT = ManualWeighingTaskInfo.WaitForAcknowledge;
                else if (!mustWait && ManualWeighingPW.ManualWeighingNextTask.ValueT == ManualWeighingTaskInfo.WaitForAcknowledge)
                    ManualWeighingPW.ManualWeighingNextTask.ValueT = ManualWeighingTaskInfo.None;
            }

            return mustWait;
        }

        public override void SMIdle()
        {
            base.SMIdle();
            if (!string.IsNullOrEmpty(CurrentWeighingMaterial.ValueT))
                CurrentWeighingMaterial.ValueT = "";

            ManuallyAddedQuantity.ValueT = 0;
            ReleaseActiveScaleObject();
            TareScaleState.ValueT = (short)TareScaleStateEnum.None;

            using (ACMonitor.Lock(_65000_IsManCompLock))
            {
                _VerifyQuantity = 0;
                _IsManuallyCompleted = false;
            }

            CurrentACMethod.ValueT = null;
        }

        public void Abort(bool isConsumed)
        {
            PAEScaleGravimetric scale = ActiveScaleObject;
            if (scale != null && !CheckInToleranceOnlyManuallyAddedQuantity)
            {
                if (TareScaleState.ValueT == (short)TareScaleStateEnum.TareOK)
                    SetPAFResult(scale.ActualWeight.ValueT + ManuallyAddedQuantity.ValueT, isConsumed);
                else
                    SetPAFResult(ManuallyAddedQuantity.ValueT, isConsumed);
                scale.TareScale(false, IsSimulationOn);
            }
            else
                SetPAFResult(ManuallyAddedQuantity.ValueT, isConsumed);

            Abort();
        }

        public override void Abort()
        {
            if (CurrentACState == ACStateEnum.SMStarting)
                Reset();
            TareScaleState.ValueT = (short)TareScaleStateEnum.None;
            base.Abort();
        }

        public override void Reset()
        {
            if (CurrentACMethod != null && CurrentACMethod.ValueT != null)
            {
                CurrentACMethod.ValueT.ParameterValueList["FacilityCharge"] = null;
                CurrentACMethod.ValueT.ParameterValueList["Facility"] = null;
            }
            TareScaleState.ValueT = (short)TareScaleStateEnum.None;
            base.Reset();
        }

        #endregion

        #region Lot-Handling
        [ACMethodInfo("", "", 999)]
        public virtual Msg LotChange(Guid? newFacilityCharge, LotUsedUpEnum? isConsumed, bool forceSetFC_F)
        {
            if (ManualWeighingPW == null)
            {
                //Error50365: This function was not invoked by a workflow node. (Diese Funktion wurde nicht durch einen Worfkowknoten gestartet.)
                return new Msg(this, eMsgLevel.Error, "PAFManualWeighing", "LotChange(10)", 978, "Error50365");
            }

            if (!newFacilityCharge.HasValue)
            {
                //Error50366: The parameter newFacilityCharge is null!
                return new Msg(this, eMsgLevel.Error, "PAFManualWeighing", "LotChange(20)", 984, "Error50366");
            }

            double quantity = ManuallyAddedQuantity.ValueT;

            if (!CheckInToleranceOnlyManuallyAddedQuantity && ActiveScaleObject == null)
                DetermineTargetScaleObject();

            if (!CheckInToleranceOnlyManuallyAddedQuantity && ActiveScaleObject == null)
            {
                //Error50364: Can not get the active scale object! Please check your scale configuration (MinDosingWeight, MaxScaleWeight) or manual weighing ScaleConfiguration.
                return new Msg(this, eMsgLevel.Error, ClassName, "LotChange(30)", 993, "Error50364");
            }
            else if (!CheckInToleranceOnlyManuallyAddedQuantity)
                quantity += ActiveScaleObject.ActualWeight.ValueT;

            return ManualWeighingPW.SelectFCFromPAF(newFacilityCharge, quantity, isConsumed, forceSetFC_F);
        }

        [ACMethodInfo("OnScanEvent", "en{'OnScanEvent'}de{'OnScanEvent'}", 503)]
        public BarcodeSequenceBase OnScanEvent(BarcodeSequenceBase sequence, bool previousLotConsumed, Guid facilityID, Guid facilityChargeID, int scanSequence, short? questionResult)
        {
            BarcodeSequenceBase resultSequence = new BarcodeSequenceBase();
            if (scanSequence == 1)
            {
                // Info50050: Scan a lot number or a other identifier to identify the material or quant. (Scannen Sie eine Los- bzw. Chargennummer oder ein anderes Kennzeichen zur Identifikation des Materials bzw. Quants.)
                resultSequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent(10)", 10, "Info50050");
                resultSequence.State = BarcodeSequenceBase.ActionState.ScanAgain;
            }
            else
            {
                LotUsedUpEnum? isLotConsumed = null;

                if (facilityChargeID == Guid.Empty && facilityID == Guid.Empty)
                {
                    // Error50354: Unsupported command sequence!  (Nicht unterstützte Befehlsfolge!)
                    resultSequence.Message = new Msg(this, eMsgLevel.Error, ClassName, "OnScanEvent(20)", 20, "Error50354");
                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                }
                else
                {
                    bool forceSetFacilityCharge = false;
                    if (questionResult.HasValue)
                    {
                        if (sequence.Message.TranslID == "Question50089")
                        {
                            if ((Global.MsgResult)questionResult.Value == Global.MsgResult.Yes)
                            {
                                isLotConsumed = LotUsedUpEnum.Yes;
                            }
                            else if ((Global.MsgResult)questionResult.Value == Global.MsgResult.No)
                            {
                                isLotConsumed = LotUsedUpEnum.No;
                            }
                            else
                            {
                                // Info50051: Lot change has been cancelled. (Chargenwechsel wurde abgebrochen.)
                                resultSequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent(30)", 30, "Info50051");
                                resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                                return resultSequence;
                            }
                        }
                        else if (sequence.Message.TranslID == "Question50090")
                        {
                            if ((Global.MsgResult)questionResult.Value == Global.MsgResult.Yes)
                            {
                                isLotConsumed = LotUsedUpEnum.YesVerified;
                            }
                            else if ((Global.MsgResult)questionResult.Value == Global.MsgResult.No)
                            {
                                isLotConsumed = LotUsedUpEnum.No;
                            }
                            else
                            {
                                // Info50051: Lot change has been cancelled. (Chargenwechsel wurde abgebrochen.)
                                resultSequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent(30)", 30, "Info50051");
                                resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                                return resultSequence;
                            }
                        }
                        else
                        {
                            if ((Global.MsgResult)questionResult.Value == Global.MsgResult.Yes)
                            {
                                forceSetFacilityCharge = true;
                            }
                            else
                            {
                                // Info50051: Lot change has been cancelled. (Chargenwechsel wurde abgebrochen.)
                                resultSequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent(30)", 30, "Info50051");
                                resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                                return resultSequence;
                            }
                        }
                    }

                    Msg msg = LotChange(facilityChargeID, isLotConsumed, forceSetFacilityCharge);
                    if (msg != null)
                    {
                        resultSequence.Message = msg;
                        if (msg.MessageLevel != eMsgLevel.Question)
                            resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                    }
                    else
                    {
                        // Info50052: A new lot was activated or changed. (Neue Charge wurde aktiviert bzw. gewechselt.)
                        resultSequence.Message = new Msg(this, eMsgLevel.Info, ClassName, "OnScanEvent(40)", 40, "Info50052");
                        resultSequence.State = BarcodeSequenceBase.ActionState.Completed;
                    }
                }
            }
            return resultSequence;
        }

        #endregion

        #endregion

        #region ACMethod

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue(vd.Material.ClassName, typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add(vd.Material.ClassName, "en{'Material'}de{'Material'}");

            method.ParameterValueList.Add(new ACValue("PLPosRelation", typeof(Guid), null, Global.ParamOption.Optional));
            paramTranslation.Add("PLPosRelation", "en{'Order position'}de{'Auftragsposition'}");

            method.ParameterValueList.Add(new ACValue("FacilityCharge", typeof(Guid), null, Global.ParamOption.Optional));
            paramTranslation.Add("FacilityCharge", "en{'Batch Location'}de{'Chargenplatz'}");

            method.ParameterValueList.Add(new ACValue("Facility", typeof(Guid), null, Global.ParamOption.Optional));
            paramTranslation.Add("Facility", vd.ConstApp.Facility);

            method.ParameterValueList.Add(new ACValue("Route", typeof(Route), null, Global.ParamOption.Required));
            paramTranslation.Add("Route", "en{'Route'}de{'Route'}");

            method.ParameterValueList.Add(new ACValue("TargetQuantity", typeof(Double), (Double)0.0, Global.ParamOption.Required));
            paramTranslation.Add("TargetQuantity", "en{'Target Quantity'}de{'Sollmenge'}");

            method.ParameterValueList.Add(new ACValue("TolerancePlus", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("TolerancePlus", "en{'Tolerance + [+=kg/-=%]'}de{'Toleranz + [+=kg/-=%]'}");

            method.ParameterValueList.Add(new ACValue("ToleranceMinus", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            paramTranslation.Add("ToleranceMinus", "en{'Tolerance - [+=kg/-=%]'}de{'Toleranz - [+=kg/-=%]'}");

            method.ParameterValueList.Add(new ACValue("SkipToleranceCheck", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("SkipToleranceCheck", "en{'Skip tolerance check'}de{'Toleranzprüfung überspringen'}");

            method.ParameterValueList.Add(new ACValue("TargetScaleACIdentifier", typeof(string), null, Global.ParamOption.Optional));
            paramTranslation.Add("TargetScaleACIdentifier", "en{'Weighing only on Scale (ACIdentifier)'}de{'Verwiegung nur auf Waage (ACIdentifier)'}");

            method.ParameterValueList.Add(new ACValue("IsLastWeighingMaterial", typeof(bool), false, Global.ParamOption.Optional));
            paramTranslation.Add("IsLastWeighingMaterial", "en{'Is last weighing material'}de{'Ist das letzte Wiegegut'}");


            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            method.ResultValueList.Add(new ACValue("ActualQuantity", typeof(Double), (Double)0.0, Global.ParamOption.Optional));
            resultTranslation.Add("ActualQuantity", "en{'Actual quantity'}de{'Istgewicht'}");
            method.ResultValueList.Add(new ACValue("IsComponentConsumed", typeof(bool), false, Global.ParamOption.Optional));
            resultTranslation.Add("IsComponentConsumed", "en{'Is component consumed'}de{'Wird Komponente verbraucht'}");

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        #endregion

        #region Enums

        public enum TareScaleStateEnum : short
        {
            None = 0,
            TareReq = 1,
            TareOK = 2
        }

        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(LotChange):
                    result = LotChange(acParameter[0] as Guid?, (LotUsedUpEnum?)acParameter[1], (bool)acParameter[2]);
                    return true;
                case nameof(TareActiveScale):
                    TareActiveScale();
                    return true;
                case nameof(InheritParamsFromConfig):
                    InheritParamsFromConfig(acParameter[0] as ACMethod, acParameter[1] as ACMethod, (bool)acParameter[2]);
                    return true;
                case nameof(GetActiveScaleObjectACUrl):
                    result = GetActiveScaleObjectACUrl();
                    return true;
                case nameof(SetDefaultACMethodValues):
                    SetDefaultACMethodValues(acParameter[0] as ACMethod);
                    return true;
                case nameof(SetActiveScaleObject):
                    SetActiveScaleObject(acParameter[0] as string);
                    return true;
                case nameof(OnScanEvent):
                    result = OnScanEvent((BarcodeSequenceBase)acParameter[0], (bool)acParameter[1], (Guid)acParameter[2], (Guid)acParameter[3], (int)acParameter[4], (short?)acParameter[5]);
                    return true;
                case nameof(GetAvailableScaleObjects):
                    result = GetAvailableScaleObjects();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PAFManualWeighing(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion
    }
}
