using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using vd = gip.mes.datamodel;
using System.Text;
using System.Threading.Tasks;
using gip.mes.datamodel;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Register entity on scan'}de{'Entität beim Scannen registrieren'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false)]
    public class PAFUnregisterEntityOnScan : PAProcessFunction
    {
        static string VMethodName_UnregisterEntityOnScan = "UnegisterEntityOnScan";

        public PAFUnregisterEntityOnScan(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        static PAFUnregisterEntityOnScan()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFUnregisterEntityOnScan), ACStateConst.TMStart, CreateVirtualMethod(VMethodName_UnregisterEntityOnScan,
                               "en{'Unegister entity on scan'}de{'Unegister entity on scan'}", typeof(PWWorkTaskGeneric)));
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }

        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();

            if (result)
            {
                PAFRegisterEntityOnScan registerEntityOnScan = ParentACComponent.FindChildComponents<PAFRegisterEntityOnScan>().FirstOrDefault();
                if (registerEntityOnScan == null)
                {
                    return false;
                }

                RegisterEntityOnScan = new ACRef<PAFRegisterEntityOnScan>(registerEntityOnScan, this);
            }

            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (RegisterEntityOnScan != null)
            {
                RegisterEntityOnScan.Detach();
                RegisterEntityOnScan = null;
            }

            return base.ACDeInit(deleteACClassTask);
        }

        public ACRef<PAFRegisterEntityOnScan> RegisterEntityOnScan
        {
            get;
            set;
        }

        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
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
                    string virtMethodName = VMethodName_UnregisterEntityOnScan;
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

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue("MinDuration", typeof(TimeSpan), null, Global.ParamOption.Optional));
            paramTranslation.Add("MinDuration", "en{'Minimum duration'}de{'Minimum duration'}");

            method.ParameterValueList.Add(new ACValue("MaxDuration", typeof(TimeSpan), null, Global.ParamOption.Optional));
            paramTranslation.Add("MaxDuration", "en{'Maximum duration'}de{'Maximum duration'}");


            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        protected MsgWithDetails GetACMethodFromConfig(Database db, Route route, ACMethod acMethod, bool isConfigInitialization = false)
        {
            if (route == null || !route.Any())
            {
                //Error50360: The route is null or empty.
                return new MsgWithDetails(this, eMsgLevel.Error, nameof(PAFUnregisterEntityOnScan), "GetACMethodFromConfig(10)", 446, "Error50360");
            }
            if (IsMethodChangedFromClient)
                return null;
            RouteItem targetRouteItem = route.LastOrDefault();
            if (targetRouteItem.Target.ACKind == Global.ACKinds.TPAProcessFunction)
            {
                if (route.Count < 2)
                {
                    //Error50361: The route has not enough route items.
                    return new MsgWithDetails(this, eMsgLevel.Error, nameof(PAFUnregisterEntityOnScan), "GetACMethodFromConfig(20)", 456, "Error50361");
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

        [ACMethodInfo("Function", "en{'Inherirt params from config'}de{'Übernehme Dosierparameter aus Konfiguration'}", 9999)]
        public virtual void InheritParamsFromConfig(ACMethod newACMethod, ACMethod configACMethod, bool isConfigInitialization)
        {
            if (isConfigInitialization)
            {
                newACMethod.ParameterValueList.CopyValues(configACMethod.ParameterValueList);

                //try
                //{
                //    newACMethod.ParameterValueList[vd.Material.ClassName] = "";
                //    newACMethod.ParameterValueList["PLPosRelation"] = Guid.Empty;
                //    newACMethod.ParameterValueList["FacilityCharge"] = Guid.Empty;
                //    newACMethod.ParameterValueList["Facility"] = Guid.Empty;
                //    newACMethod.ParameterValueList["Route"] = null;
                //    newACMethod.ParameterValueList["TargetQuantity"] = (double)0.0;
                //}
                //catch (Exception ec)
                //{
                //    string msg = ec.Message;
                //    if (ec.InnerException != null && ec.InnerException.Message != null)
                //        msg += " Inner:" + ec.InnerException.Message;

                //    Messages.LogException("PAFDosing", "InheritParamsFromConfig", msg);
                //}
            }
            else
            {
                //double targetQ = newACMethod.ParameterValueList.GetDouble("TargetQuantity");
                //double tolPlus = newACMethod.ParameterValueList.GetDouble("TolerancePlus");
                //if (Math.Abs(tolPlus) <= Double.Epsilon)
                //    tolPlus = configACMethod.ParameterValueList.GetDouble("TolerancePlus");

                //tolPlus = PAFDosing.RecalcAbsoluteTolerance(tolPlus, targetQ, null);
                //newACMethod["TolerancePlus"] = tolPlus;

                //double tolMinus = newACMethod.ParameterValueList.GetDouble("ToleranceMinus");
                //if (Math.Abs(tolMinus) <= Double.Epsilon)
                //    tolMinus = configACMethod.ParameterValueList.GetDouble("ToleranceMinus");

                //tolMinus = PAFDosing.RecalcAbsoluteTolerance(tolMinus, targetQ, null);
                //newACMethod["ToleranceMinus"] = tolMinus;
            }
        }

        [ACMethodInfo("OnScanEvent", "en{'OnScanEvent'}de{'OnScanEvent'}", 503)]
        public BarcodeSequenceBase OnScanEvent(BarcodeSequenceBase sequence, bool previousLotConsumed, Guid facilityID, Guid facilityChargeID, int scanSequence, short? questionResult)
        {
            BarcodeSequenceBase resultSequence = new BarcodeSequenceBase();
            if (scanSequence == 1)
            {
                // Info50050: Scan a lot number or a other identifier to identify the material or quant. (Scannen Sie eine Los- bzw. Chargennummer oder ein anderes Kennzeichen zur Identifikation des Materials bzw. Quants.)
                resultSequence.Message = new Msg(this, eMsgLevel.Info, nameof(PAFUnregisterEntityOnScan), "OnScanEvent(10)", 10, "Info50050");
                resultSequence.State = BarcodeSequenceBase.ActionState.ScanAgain;
            }
            else
            {
                if (facilityChargeID == Guid.Empty && facilityID == Guid.Empty)
                {
                    // Error50354: Unsupported command sequence!  (Nicht unterstützte Befehlsfolge!)
                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFUnregisterEntityOnScan), "OnScanEvent(20)", 20, "Error50354");
                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                }
                else
                {
                    using(DatabaseApp dbApp = new DatabaseApp())
                    {
                        Guid regEntOnScanID = RegisterEntityOnScan.ValueT.ComponentClass.ACClassID;

                        OperationLog inOperationLog = dbApp.OperationLog.FirstOrDefault(c => c.RefACClassID == regEntOnScanID
                                                                    && c.FacilityChargeID != null
                                                                    && c.FacilityChargeID == facilityChargeID
                                                                    && c.OperationState == (short)OperationLogStateEnum.Open);

                        if (inOperationLog != null)
                        {
                            if (!questionResult.HasValue)
                            {
                                TimeSpan durationToCheck = DateTime.Now - inOperationLog.OperationTime;

                                FacilityCharge fc = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == facilityChargeID);
                                if (fc == null)
                                {
                                    // Error50... : Facility charge error TODO
                                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFUnregisterEntityOnScan), "OnScanEvent(20)", 20, "Error50354");
                                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                                }

                                ACMethod acMethod = RegisterEntityOnScan.ValueT.GetConfigForMaterial(dbApp, fc.MaterialID);

                                if (acMethod == null)
                                {
                                    // Error50... : Facility charge error TODO
                                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFUnregisterEntityOnScan), "OnScanEvent(20)", 20, "Error50354");
                                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                                }

                                ACValue minDurationValue = acMethod.ParameterValueList.GetACValue("MinDuration");
                                if (minDurationValue == null)
                                {
                                    // Error50... : Facility charge error TODO
                                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFUnregisterEntityOnScan), "OnScanEvent(20)", 20, "Error50354");
                                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                                }

                                if (minDurationValue.Value != null)
                                {
                                    TimeSpan minDuration = minDurationValue.ParamAsTimeSpan;

                                    if (minDuration.TotalSeconds > 0)
                                    {
                                        if (durationToCheck < minDuration)
                                        {
                                            // Error50... : Facility charge error TODO
                                            resultSequence.Message = new Msg(this, eMsgLevel.Question, nameof(PAFUnregisterEntityOnScan), "OnScanEvent(20)", 20, "Error50354");
                                            return resultSequence;
                                        }
                                    }
                                }

                                ACValue maxDurationValue = acMethod.ParameterValueList.GetACValue("MaxDuration");
                                if (maxDurationValue == null)
                                {
                                    // Error50... : Facility charge error TODO
                                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFUnregisterEntityOnScan), "OnScanEvent(20)", 20, "Error50354");
                                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                                }

                                if (maxDurationValue.Value != null)
                                {
                                    TimeSpan maxDuration = maxDurationValue.ParamAsTimeSpan;
                                    if (maxDuration.TotalSeconds > 0)
                                    {
                                        if (durationToCheck > maxDuration)
                                        {
                                            // Error50... : Facility charge error TODO
                                            resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFRegisterEntityOnScan), "OnScanEvent(20)", 20, "Error50354");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if ((Global.MsgResult)questionResult.Value != Global.MsgResult.Yes)
                                {
                                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFRegisterEntityOnScan), "OnScanEvent(20)", 20, "Error50354");
                                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                                }
                            }

                            inOperationLog.OperationState = (short)OperationLogStateEnum.Closed;

                            OperationLog outOperationLog = OperationLog.NewACObject(dbApp, null);
                            outOperationLog.RefACClassID = this.ComponentClass.ACClassID;
                            outOperationLog.FacilityChargeID = facilityChargeID;
                            outOperationLog.Operation = (short)OperationLogEnum.UnregisterEntityOnScan;
                            outOperationLog.OperationState = (short)OperationLogStateEnum.Closed;
                            outOperationLog.OperationTime = DateTime.Now;

                            dbApp.OperationLog.AddObject(outOperationLog);

                            Msg msg = dbApp.ACSaveChanges();
                            if (msg != null)
                            {
                                resultSequence.Message = msg;
                                resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                            }

                            resultSequence.State = BarcodeSequenceBase.ActionState.Completed;

                        }
                        else
                        {
                            // Error50... : The scanned entity is not registered on this machine/place! TODO
                            resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFRegisterEntityOnScan), "OnScanEvent(20)", 20, "Error50354");
                            resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                        }
                    }
                }
            }
            return resultSequence;
        }
    }
}
