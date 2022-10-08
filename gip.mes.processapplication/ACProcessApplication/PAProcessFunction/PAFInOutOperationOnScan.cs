using gip.core.autocomponent;
using gip.core.datamodel;
using vd = gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.mes.datamodel;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Input/output operation on scan'}de{'Ein/Aus-Betrieb beim Scannen'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, PWInfoACClass = nameof(PWWorkTaskGeneric))]
    public class PAFInOutOperationOnScan : PAProcessFunction
    {
        static string VMethodName_InOutOperationOnScan = "InOutOperationOnScan";

        #region c'tors

        public PAFInOutOperationOnScan(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }
        static PAFInOutOperationOnScan()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFInOutOperationOnScan), ACStateConst.TMStart, CreateVirtualMethod(VMethodName_InOutOperationOnScan,
                                           "en{'In/out operation on scan'}de{'Ein/Aus-Betrieb beim Scannen'}", typeof(PWWorkTaskGeneric)));
        }

        #endregion

        #region Methods

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
                    string virtMethodName = VMethodName_InOutOperationOnScan;
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
                return new MsgWithDetails(this, eMsgLevel.Error, nameof(PAFInOutOperationOnScan), "GetACMethodFromConfig(10)", 446, "Error50360");
            }
            if (IsMethodChangedFromClient)
                return null;
            RouteItem targetRouteItem = route.LastOrDefault();
            if (targetRouteItem.Target.ACKind == Global.ACKinds.TPAProcessFunction)
            {
                if (route.Count < 2)
                {
                    //Error50361: The route has not enough route items.
                    return new MsgWithDetails(this, eMsgLevel.Error, nameof(PAFInOutOperationOnScan), "GetACMethodFromConfig(20)", 456, "Error50361");
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

        public ACMethod GetConfigForMaterial(DatabaseApp dbApp, Guid materialID)
        {
            ACMethod acMethod = ACUrlACTypeSignature("!" + VMethodName_InOutOperationOnScan);

            Guid acClassIdOfParent = ParentACComponent.ComponentClass.ACClassID;

            var wayIndependent = dbApp.MaterialConfig.Where(c => c.MaterialID == materialID
                                                              && c.VBiACClassID == acClassIdOfParent).SetMergeOption(System.Data.Objects.MergeOption.NoTracking);

            foreach (var matConfig in wayIndependent)
            {
                ACValue acValue = acMethod.ParameterValueList.Where(c => c.ACIdentifier == matConfig.LocalConfigACUrl).FirstOrDefault();
                if (acValue != null/* && acValue.HasDefaultValue*/)
                    acValue.Value = matConfig.Value;
                if (acMethod != null)
                {
                    acValue = acMethod.ParameterValueList.Where(c => c.ACIdentifier == matConfig.LocalConfigACUrl).FirstOrDefault();
                    if (acValue != null/* && acValue.HasDefaultValue*/)
                        acValue.Value = matConfig.Value;
                }
            }

            return acMethod;
        }

        [ACMethodInfo("Function", "en{'Inherit params from config'}de{'Übernehme Dosierparameter aus Konfiguration'}", 9999)]
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
        public virtual BarcodeSequenceBase OnScanEvent(BarcodeSequenceBase sequence, bool previousLotConsumed, Guid facilityID, Guid facilityChargeID, int scanSequence, short? questionResult)
        {
            BarcodeSequenceBase resultSequence = new BarcodeSequenceBase();
            if (scanSequence == 1)
            {
                // Info50085: Scan a lot number or a other identifier to identify the material or quant. (Scannen Sie eine Los- bzw. Chargennummer oder ein anderes Kennzeichen zur Identifikation des Materials bzw. Quants.)
                resultSequence.Message = new Msg(this, eMsgLevel.Info, nameof(PAFInOutOperationOnScan), "OnScanEvent(10)", 10, "Info50085");
                resultSequence.State = BarcodeSequenceBase.ActionState.ScanAgain;
            }
            else
            {
                if (facilityChargeID == Guid.Empty && facilityID == Guid.Empty)
                {
                    // Error50563: Unsupported command sequence!  (Nicht unterstützte Befehlsfolge!)
                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFInOutOperationOnScan), "OnScanEvent(20)", 20, "Error50563");
                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                }
                else
                {
                    using (DatabaseApp dbApp = new DatabaseApp())
                    {
                        OperationLog inOperationLog = dbApp.OperationLog.Where(c => c.RefACClassID == ComponentClass.ACClassID
                                                                                 && c.FacilityChargeID != null
                                                                                 && c.FacilityChargeID == facilityChargeID
                                                                                 && c.OperationState == (short)OperationLogStateEnum.Open)
                                                                        .OrderBy(o => o.OperationTime)
                                                                        .FirstOrDefault();

                        if (inOperationLog != null)
                        {
                            if (questionResult != null && sequence.QuestionSequence == 1 && (Global.MsgResult)questionResult.Value == Global.MsgResult.Yes)
                            {
                                OutOperationOnScan(resultSequence, dbApp, inOperationLog, facilityChargeID);
                            }
                            else if (questionResult != null && sequence.QuestionSequence > 1)
                            {
                                if ((Global.MsgResult)questionResult.Value == Global.MsgResult.Yes)
                                {
                                    OutOperationOnScan(resultSequence, dbApp, inOperationLog, facilityChargeID, true);
                                }
                                else
                                {
                                    //Error50568: Output operation is cancelled.
                                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFInOutOperationOnScan), "OnScanEvent(40)", 40, "Error50568");
                                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                                    return resultSequence;
                                }
                            }
                            else if (sequence.QuestionSequence < 2)
                            {

                                FacilityCharge fc = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == facilityChargeID);

                                if (fc == null)
                                {
                                    //Error50564: The quant with ID: {0} not exist in the database.
                                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFInOutOperationOnScan), "OnScanEvent(30)", 30, "Error50564", facilityChargeID);
                                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                                    return resultSequence;
                                }

                                if (fc.SplitNo > 0)
                                {
                                    var result = OutOperationOnScan(resultSequence, dbApp, inOperationLog, facilityChargeID);
                                    if (result != null)
                                        return result;
                                }
                                else
                                {
                                    // Question50091 : Output operation on entity?
                                    resultSequence.QuestionSequence = 1;
                                    resultSequence.Message = new Msg(this, eMsgLevel.Question, nameof(PAFInOutOperationOnScan), "OnScanEvent(40)", 40, "Question50091", eMsgButton.YesNo);
                                    return resultSequence;
                                }
                            }
                        }
                        else
                        {
                            inOperationLog = OperationLog.NewACObject(dbApp, null);
                            inOperationLog.RefACClassID = this.ComponentClass.ACClassID;
                            inOperationLog.FacilityChargeID = facilityChargeID;
                            inOperationLog.Operation = (short)OperationLogEnum.RegisterEntityOnScan;
                            inOperationLog.OperationState = (short)OperationLogStateEnum.Open;
                            inOperationLog.OperationTime = DateTime.Now;

                            PAProcessModuleVB moduleVB = ParentACComponent as PAProcessModuleVB;

                            if (moduleVB != null && moduleVB.CurrentProgramLog != null)
                            {
                                inOperationLog.ACProgramLogID = moduleVB.CurrentProgramLog.ACProgramLogID;
                            }

                            dbApp.OperationLog.AddObject(inOperationLog);

                            Msg msg = dbApp.ACSaveChanges();
                            if (msg != null)
                            {
                                resultSequence.Message = msg;
                                resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                            }
                        }
                    }

                    // Info50086: Operation is successfully performed!
                    resultSequence.Message = new Msg(this, eMsgLevel.Info, nameof(PAFInOutOperationOnScan), "OnScanEvent(40)", 40, "Info50086");
                    resultSequence.State = BarcodeSequenceBase.ActionState.Completed;
                }
            }
            return resultSequence;
        }

        private BarcodeSequenceBase OutOperationOnScan(BarcodeSequenceBase resultSequence, DatabaseApp dbApp, OperationLog inOperationLog, Guid facilityChargeID, bool skipValidation = false)
        {
            if (!skipValidation)
            {
                TimeSpan durationToCheck = DateTime.Now - inOperationLog.OperationTime;

                FacilityCharge fc = dbApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == facilityChargeID);
                if (fc == null)
                {
                    //Error50564: The quant with ID: {0} not exist in the database.
                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFInOutOperationOnScan), "OutOperationOnScan(10)", 10, "Error50564", facilityChargeID);
                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                }

                ACMethod acMethod = GetConfigForMaterial(dbApp, fc.MaterialID);

                if (acMethod == null)
                {
                    // Error50565 : Can't get configuration for material.
                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFInOutOperationOnScan), "OutOperationOnScan(20)", 20, "Error50565");
                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                }

                ACValue minDurationValue = acMethod.ParameterValueList.GetACValue("MinDuration");
                if (minDurationValue == null)
                {
                    // Error50566 : Minimum duration setting is not exist.
                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFInOutOperationOnScan), "OutOperationOnScan(30)", 30, "Error50566");
                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                }

                if (minDurationValue.Value != null)
                {
                    TimeSpan minDuration = minDurationValue.ParamAsTimeSpan;

                    if (minDuration.TotalSeconds > 0)
                    {
                        if (durationToCheck < minDuration)
                        {
                            // The quant is not long enough in a object. Do you want to continue with a output operation?
                            // Question50092: Kvant nije dovoljno dugo u objektu? Želite li ipak nastaviti s izlaznom operacijom? 
                            resultSequence.QuestionSequence = 2;
                            resultSequence.Message = new Msg(this, eMsgLevel.Question, nameof(PAFInOutOperationOnScan), "OutOperationOnScan(40)", 40, "Question50092", eMsgButton.YesNo);
                            return resultSequence;
                        }
                    }
                }

                ACValue maxDurationValue = acMethod.ParameterValueList.GetACValue("MaxDuration");
                if (maxDurationValue == null)
                {
                    // Error50567 : Maximum duration setting is not exist.
                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFInOutOperationOnScan), "OutOperationOnScan(50)", 50, "Error50567");
                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                }

                if (maxDurationValue.Value != null)
                {
                    TimeSpan maxDuration = maxDurationValue.ParamAsTimeSpan;
                    if (maxDuration.TotalSeconds > 0)
                    {
                        if (durationToCheck > maxDuration)
                        {
                            // Warning50057 : The quant was too long in the object.
                            resultSequence.Message = new Msg(this, eMsgLevel.Warning, nameof(PAFInOutOperationOnScan), "OutOperationOnScan(60)", 60, "Warning50057");
                        }
                    }
                }
            }

            inOperationLog.OperationState = (short)OperationLogStateEnum.Closed;

            OperationLog outOperationLog = OperationLog.NewACObject(dbApp, null);
            outOperationLog.RefACClassID = this.ComponentClass.ACClassID;
            outOperationLog.FacilityChargeID = facilityChargeID;
            outOperationLog.Operation = (short)OperationLogEnum.UnregisterEntityOnScan;
            outOperationLog.OperationState = (short)OperationLogStateEnum.Closed;
            outOperationLog.OperationTime = DateTime.Now;
            outOperationLog.ACProgramLogID = inOperationLog.ACProgramLogID;

            dbApp.OperationLog.AddObject(outOperationLog);

            Msg msg = dbApp.ACSaveChanges();
            if (msg != null)
            {
                resultSequence.Message = msg;
                resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
            }

            resultSequence.State = BarcodeSequenceBase.ActionState.Completed;

            return null;
        }

        #endregion
    }
}
