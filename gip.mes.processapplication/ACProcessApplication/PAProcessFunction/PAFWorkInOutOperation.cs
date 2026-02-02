// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static gip.core.autocomponent.PAProcessFunction;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'In out operation on scan'}de{'In out operation on scan'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, nameof(PWWorkInOutOperation), true)]
    public class PAFWorkInOutOperation : PAFWorkTaskScanBase
    {
        #region c'tors

        static string VMethodName_InOutOperation = "WorkInOutOperation";

        static PAFWorkInOutOperation()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFWorkInOutOperation), ACStateConst.TMStart, CreateVirtualMethod(VMethodName_InOutOperation,
                                           "en{'In/out operation on scan'}de{'Ein/Aus-Betrieb beim Scannen'}", typeof(PWWorkInOutOperation)));
        }

        public PAFWorkInOutOperation(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();
            OperationLogItems.ValueT = GetOperationLogList();

            ApplicationManager.ProjectWorkCycleR10sec += ApplicationManager_ProjectWorkCycleR10sec;

            return result;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            bool result = await base.ACDeInit(deleteACClassTask);
            OperationLogItems.ValueT = null;
            return result;
        }

        #endregion

        #region Properties

        [ACPropertyBindingSource(800, "", "en{'Operation log'}de{'Operation log'}", "", true, true)]
        public IACContainerTNet<OperationLogItemList> OperationLogItems
        {
            get;
            set;
        }

        [ACPropertyInfo(9999, "", "en{'Clean up operation logs after [s]'}de{'Bereinigen Sie die Betriebsprotokolle nach [s]'}", "", true, IsPersistable = true, DefaultValue = 300)]
        public int CleanUpOperationLogsAfter
        {
            get;
            set;
        }

        private DateTime _LastCleanUpRun = DateTime.MinValue;

        #endregion

        #region Methods

        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();

            method.ParameterValueList.Add(new ACValue("MinDuration", typeof(TimeSpan), null, Global.ParamOption.Optional));
            paramTranslation.Add("MinDuration", "en{'Minimum duration'}de{'Minimum duration'}");

            method.ParameterValueList.Add(new ACValue("Duration", typeof(TimeSpan), null, Global.ParamOption.Optional));
            paramTranslation.Add("Duration", "en{'Duration'}de{'Duration'}");

            method.ParameterValueList.Add(new ACValue("Hint", typeof(TimeSpan), null, Global.ParamOption.Optional));
            paramTranslation.Add("Hint", "en{'Hint'}de{'Hint'}");

            method.ParameterValueList.Add(new ACValue("MaxDuration", typeof(TimeSpan), null, Global.ParamOption.Optional));
            paramTranslation.Add("MaxDuration", "en{'Maximum duration'}de{'Maximum duration'}");

            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();

            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        protected override bool PWWorkTaskScanDeSelector(IACComponent c)
        {
            return c is PWWorkInOutOperation;
        }

        protected override bool PWWorkTaskScanSelector(IACComponent c)
        {
            return c is PWWorkInOutOperation;
        }

        public override WorkTaskScanResult OnScanEvent(BarcodeSequenceBase sequence, PAProdOrderPartslistWFInfo selectedPOLWf, Guid facilityChargeID, int scanSequence, short? sQuestionResult, PAProdOrderPartslistWFInfo lastInfo, bool? malfunction, Guid? oeeReason)
        {
            WorkTaskScanResult result = null;

            if (scanSequence == 1 || sequence.State == BarcodeSequenceBase.ActionState.Selection)
            {
                result = base.OnScanEvent(sequence, selectedPOLWf, facilityChargeID, scanSequence, sQuestionResult, lastInfo, malfunction, oeeReason);

                if (result.Result.State == BarcodeSequenceBase.ActionState.Selection ||result.Result.State == BarcodeSequenceBase.ActionState.Cancelled)
                {
                    result.Result.State = BarcodeSequenceBase.ActionState.SelectionScanAgain;
                }
            }
            else
            {
                result = new WorkTaskScanResult();

                PAProcessModuleVB pModule = ParentACComponent as PAProcessModuleVB;
                if (pModule == null)
                {
                    sequence.Message = new Msg(eMsgLevel.Error, "Process module is not available.");
                    sequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                    result.Result = sequence;
                    return result;
                }

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    OperationLog inOperationLog = dbApp.OperationLog.Include(c => c.FacilityCharge)
                                                                    .Include(c => c.FacilityCharge.Material)
                                                                    .Include(c => c.FacilityCharge.FacilityLot)
                                                                    .Include(c => c.FacilityCharge.Partslist)
                                                                    .Where(c => c.RefACClassID == ComponentClass.ACClassID
                                                                                 && c.FacilityChargeID != null
                                                                                 && c.FacilityChargeID == facilityChargeID
                                                                                 && c.OperationState == (short)OperationLogStateEnum.Open)
                                                                        .OrderBy(o => o.OperationTime)
                                                                        .FirstOrDefault();

                    FacilityCharge fc = inOperationLog != null ? inOperationLog.FacilityCharge : dbApp.FacilityCharge
                                                                                                      .Include(c => c.Material)
                                                                                                      .Include(c => c.FacilityLot)
                                                                                                      .Include(c => c.Partslist)
                                                                                                      .FirstOrDefault(c => c.FacilityChargeID == facilityChargeID);
                    if (fc == null)
                    {
                        sequence.Message = new Msg(eMsgLevel.Error, "Facility charge is not available.");
                        sequence.State = BarcodeSequenceBase.ActionState.Cancelled;

                        result.Result = sequence;
                        return result;
                    }

                    ProdOrder pOrder = dbApp.ProdOrder.FirstOrDefault(c => c.ProgramNo == fc.ProdOrderProgramNo);

                    Guid materialID = fc.MaterialID;
                    if (fc.Partslist != null)
                    {
                        materialID = fc.Partslist.MaterialID;
                    }
                    ProdOrderPartslist poPl = pOrder?.ProdOrderPartslist_ProdOrder.FirstOrDefault(c => c.Partslist.MaterialID == materialID);
                    // if charge is for mixure (OperationLog)
                    if(poPl == null)
                    {
                        poPl=
                            pOrder?.
                            ProdOrderPartslist_ProdOrder
                            .Where(c=>
                                      c.ProdOrderPartslistPos_ProdOrderPartslist
                                      .SelectMany(x=>x.FacilityBookingCharge_ProdOrderPartslistPos)
                                      .Where(x=>x.InwardMaterialID == materialID)
                                      .Any())
                            .FirstOrDefault();
                    }

                    PAProdOrderPartslistWFInfo orderForOccup = null;
                    PAProdOrderPartslistWFInfo orderForRelease = null;

                    if (poPl != null)
                    {

                        List<PAProdOrderPartslistWFInfo> ordersForOccupation = GetWaitingProdOrderPartslistWFInfo();
                        List<PAProdOrderPartslistWFInfo> ordersForRelease = GetActivatedProdOrderPartslistWFInfo();

                        if (ordersForOccupation.Any() && ordersForRelease.Any())
                        {
                            foreach (PAProdOrderPartslistWFInfo info in ordersForRelease)
                            {
                                ordersForOccupation.RemoveAll(c => c.ACClassWFId == info.ACClassWFId && c.POPPosId == info.POPPosId);
                            }
                        }

                        orderForOccup = ordersForOccupation.FirstOrDefault(c => c.POPId == poPl.ProdOrderPartslistID);
                        orderForRelease = ordersForRelease.FirstOrDefault(c => c.POPId == poPl.ProdOrderPartslistID);
                    }

                    if (inOperationLog == null && sequence.QuestionSequence == 0)
                    {
                        WorkTaskScanResult occupationResult = null;
                        PWWorkTaskScanBase pwNode = null;
                        if (orderForOccup != null)
                        {
                            pwNode = ACUrlCommand(orderForOccup.ACUrlWF) as PWWorkTaskScanBase;
                            occupationResult = OnOccupyingProcessModuleOnScan(pModule, pwNode, orderForOccup, sequence, orderForOccup, facilityChargeID, scanSequence, sQuestionResult);
                        }
                        else if (orderForRelease != null)
                        {
                            pwNode = ACUrlCommand(orderForRelease.ACUrlWF) as PWWorkTaskScanBase;
                        }

                        ACMethod currentACMethod = GetParameters(pwNode, dbApp, materialID);

                        CreateOperationLog(dbApp, fc, sequence, currentACMethod);

                        if (occupationResult != null && occupationResult.Result.Message != null && occupationResult.Result.Message.MessageLevel > eMsgLevel.Info)
                            sequence.Message.Message += " " + occupationResult.Result.Message.Message;

                        sequence.State = BarcodeSequenceBase.ActionState.Completed;
                        result.Result = sequence;
                        return result;
                    }
                    else
                    {
                        PWWorkTaskGeneric pwNode = null;
                        ACMethod currentACMethod = null;
                        if (orderForRelease != null)
                        {
                            pwNode = ACUrlCommand(orderForRelease.ACUrlWF) as PWWorkTaskGeneric;
                            currentACMethod = GetParameters(pwNode, dbApp, materialID);
                        }

                        if (sequence.QuestionSequence < 2)
                        {
                            CloseOperationLog(dbApp, inOperationLog, fc, sequence, sQuestionResult, currentACMethod);
                            result.Result = sequence;

                            if (sequence.State == BarcodeSequenceBase.ActionState.Question || sequence.State == BarcodeSequenceBase.ActionState.Cancelled)
                                return result;
                        }

                        if (orderForRelease != null)
                        {
                            if (sequence.QuestionSequence == 2 && sQuestionResult.HasValue)
                            {
                                if ((Global.MsgResult)sQuestionResult.Value == Global.MsgResult.Yes)
                                    orderForRelease.Pause = true;

                                WorkTaskScanResult releaseResult = OnReleasingProcessModuleOnScan(pwNode, orderForRelease, sequence, orderForRelease, facilityChargeID, scanSequence, sQuestionResult);
                                return releaseResult;
                            }

                            bool openOperationLog = dbApp.OperationLog.Where(c => c.RefACClassID == ComponentClass.ACClassID && c.FacilityChargeID != null
                                                                                                                             && c.FacilityCharge.MaterialID == fc.MaterialID
                                                                                                                             && c.FacilityCharge.FacilityID == fc.FacilityID
                                                                                                                             && c.OperationState == (short)OperationLogStateEnum.Open)
                                                                      .ToArray()
                                                                      .Any(c => c.FacilityCharge.ProdOrderProgramNo == fc.ProdOrderProgramNo);

                            if (!openOperationLog)
                            {
                                if (pwNode != null)
                                {
                                    PWGroupVB pwGroup = pwNode.ParentPWGroup as PWGroupVB;

                                    var assignedProcessModules = pwGroup.TrySemaphore.ConnectionList.Select(c => c.ValueT as PAProcessModule).ToList();
                                    if (assignedProcessModules.Count > 1)
                                    {
                                        WorkTaskScanResult releaseResult = OnReleasingProcessModuleOnScan(pwNode, orderForRelease, sequence, orderForRelease, facilityChargeID, scanSequence, sQuestionResult);

                                        sequence.State = BarcodeSequenceBase.ActionState.Completed;
                                        result.Result = sequence;
                                        return result;
                                    }
                                    else
                                    {
                                        string translationID = "Question50100";
                                        if (poPl != null && inOperationLog != null && inOperationLog.FacilityCharge.Partslist != null)
                                        {
                                            List<FacilityCharge> facilityCharges =
                                                poPl
                                                .ProdOrderPartslistPos_ProdOrderPartslist
                                                .SelectMany(c => c.FacilityBookingCharge_ProdOrderPartslistPos)
                                                .Select(c => c.InwardFacilityCharge)
                                                .Where(c => c.MaterialID == inOperationLog.FacilityCharge.MaterialID)
                                                .GroupBy(c => c.FacilityChargeID)
                                                .Select(c => c.FirstOrDefault())
                                                .ToList();

                                            facilityCharges = facilityCharges.Where(c => !c.OperationLog_FacilityCharge.Any()).ToList();

                                            if (facilityCharges.Any())
                                            {
                                                translationID = "Question50101";
                                            }

                                            // Question50100
                                            // Do you want pause order on machine. Answer with <Yes> if you want, with <No> you will release machine?
                                            // Möchten Sie die Bestellung am Maschine pausieren? Antworten Sie mit <Ja>, mit <Nein> geben Sie die Maschine frei?

                                            // Question50101
                                            // On Facility exist not processed quants! Do you want pause order on machine. Answer with <Yes> if you want, with <No> you will release machine? 
                                            // Auf der Anlage liegen keine verarbeiteten Mengen vor! Möchten Sie die Bestellung am Automaten unterbrechen? Antworten Sie mit <Ja>, wenn Sie möchten, mit <Nein> geben Sie die Maschine frei?

                                            sequence.QuestionSequence = 2;
                                            sequence.State = BarcodeSequenceBase.ActionState.Question;
                                            sequence.Message = new Msg(this, eMsgLevel.Question, nameof(PAFInOutOperationOnScan), "OutOperationOnScan(40)", 40, translationID, eMsgButton.YesNo);
                                            result.Result = sequence;
                                            return result;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        protected virtual void CreateOperationLog(DatabaseApp dbApp, FacilityCharge facilityCharge, BarcodeSequenceBase resultSequence, ACMethod parameters)
        {
            OperationLog inOperationLog = OperationLog.NewACObject(dbApp, null);
            inOperationLog.RefACClassID = this.ComponentClass.ACClassID;
            inOperationLog.FacilityCharge = facilityCharge;
            inOperationLog.Operation = (short)OperationLogEnum.RegisterEntityOnScan;
            inOperationLog.OperationState = (short)OperationLogStateEnum.Open;
            inOperationLog.OperationTime = DateTime.Now;

            PAProcessModuleVB moduleVB = ParentACComponent as PAProcessModuleVB;

            if (moduleVB != null && moduleVB.CurrentProgramLog != null)
            {
                inOperationLog.ACProgramLogID = moduleVB.CurrentProgramLog.ACProgramLogID;
            }

            dbApp.OperationLog.Add(inOperationLog);

            Msg msg = dbApp.ACSaveChanges();
            if (msg != null)
            {
                resultSequence.Message = msg;
                resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
            }
            else
            {
                AddToOperationLogList(inOperationLog, parameters, dbApp);
            }

            // Info50091
            // Operation is successfully performed!
            // Die Operation wurde erfolgreich durchgeführt!
            resultSequence.Message = new Msg(this, eMsgLevel.Info, nameof(PAFInOutOperationOnScan), "OnScanEvent(40)", 40, "Info50091");
            resultSequence.State = BarcodeSequenceBase.ActionState.Completed;
        }

        protected virtual void CloseOperationLog(DatabaseApp dbApp, OperationLog inOperationLog, FacilityCharge facilityCharge, BarcodeSequenceBase sequence, short? questionResult, ACMethod acMethod)
        {
            if (questionResult != null && sequence.QuestionSequence == 1)
            {
                if ((Global.MsgResult)questionResult.Value == Global.MsgResult.Yes)
                {
                    OutOperationOnScan(sequence, dbApp, inOperationLog, facilityCharge, acMethod, true);
                }
                else
                {
                    //Error50568: Output operation is cancelled.
                    sequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFInOutOperationOnScan), "OnScanEvent(40)", 40, "Error50568");
                    sequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                }
            }
            else if (sequence.QuestionSequence < 1)
            {
                OutOperationOnScan(sequence, dbApp, inOperationLog, facilityCharge, acMethod);
            }
        }

        private void OutOperationOnScan(BarcodeSequenceBase resultSequence, DatabaseApp dbApp, OperationLog inOperationLog, FacilityCharge fc, ACMethod acMethod, bool skipValidation = false)
        {
            if (!skipValidation)
            {
                if (acMethod != null)
                {
                    TimeSpan durationToCheck = DateTime.Now - inOperationLog.OperationTime;

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
                                // Question50099
                                // The quant has not been processed long enough! Do you want to continue with a output operation?
                                // Das Quantum wurde nicht lange genug verarbeitet! Möchten Sie mit einem -- fortfahren?
                                resultSequence.State = BarcodeSequenceBase.ActionState.Question;
                                resultSequence.QuestionSequence = 1;
                                resultSequence.Message = new Msg(this, eMsgLevel.Question, nameof(PAFInOutOperationOnScan), "OutOperationOnScan(40)", 40, "Question50099", eMsgButton.YesNo);
                                return;
                            }
                        }
                    }
                }
            }

            CompleteResult completeResult = CompleteResult.Succeeded;
            MsgWithDetails msgError = null;
            if (ACStateConverter != null)
            {
                completeResult = ACStateConverter.ReceiveACMethodResult(this, acMethod, out msgError);
                if(msgError != null && msgError.MsgDetails.Any())
                {
                    resultSequence.Message = msgError.MsgDetails.FirstOrDefault();
                    Messages.LogMessageMsg(msgError);
                }
            }

            Msg msg = OperationLog.CloseOperationLog(dbApp, inOperationLog, acMethod);
            if (msg != null)
            {
                resultSequence.Message = msg;
                resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
            }

            RemoveFromOperationLogList(inOperationLog);

            // Info50092
            // Output operation is successfully performed!
            // Der Ausgabevorgang wurde erfolgreich durchgeführt!
            resultSequence.Message = new Msg(this, eMsgLevel.Info, nameof(PAFInOutOperationOnScan), "OnScanEvent(40)", 40, "Info50092");
            resultSequence.State = BarcodeSequenceBase.ActionState.Completed;
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
                    Direction = RouteDirections.Backwards,
                    DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID == parentACClass.ACClassID,
                    DBIncludeInternalConnections = true,
                    AutoDetachFromDBContext = false
                };

                var parentModule = ACRoutingService.DbSelectRoutesFromPoint(thisACClass, this.PAPointMatIn1.PropertyInfo, routingParameters).FirstOrDefault();
                var sourcePoint = parentModule?.FirstOrDefault()?.SourceACPoint?.PropertyInfo;
                if (sourcePoint == null)
                    return;

                routingParameters.DBSelector = (c, p, r) => c.ACKind == Global.ACKinds.TPAProcessModule && c.ACClassID != parentACClass.ACClassID;

                var routes = ACRoutingService.DbSelectRoutesFromPoint(parentACClass, sourcePoint, routingParameters);
                if (routes != null && routes.Any())
                {
                    string virtMethodName = VMethodName_InOutOperation;
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
                    PAMSilo pamSilo = sourceRouteItem.SourceACComponent as PAMSilo;
                    if (pamSilo != null)
                    {
                        if (pamSilo.Facility != null && pamSilo.Facility.ValueT != null && pamSilo.Facility.ValueT.ValueT != null)
                        {
                            Guid? materialID = pamSilo.Facility.ValueT.ValueT.MaterialID;
                            if (materialID.HasValue && materialID != Guid.Empty)
                            {
                                Guid acClassIdOfParent = ParentACComponent.ComponentClass.ACClassID;
                                using (var dbApp = new DatabaseApp())
                                {
                                    // 1. Hole Material-Konfiguration spezielle für diesen Weg
                                    materialConfigList = dbApp.MaterialConfig.Where(c => c.VBiACClassPropertyRelationID == logicalRelation.ACClassPropertyRelationID && c.MaterialID == materialID.Value).AsNoTracking().ToList();
                                    var wayIndependent = dbApp.MaterialConfig.Where(c => c.MaterialID == materialID.Value && c.VBiACClassID == acClassIdOfParent).AsNoTracking();
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
                && config.EntityState != EntityState.Added
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
            if (config.EntityState == EntityState.Added || isNewDefaultedMethod)
                config.Value = storedACMethod;
            else if (isConfigInitialization)
            {
                if (differentVirtualMethod)
                    config.Value = storedACMethod;
                else
                    config.Value = acMethod;
            }
            if (config.EntityState == EntityState.Added || logicalRelation.EntityState == EntityState.Added || isNewDefaultedMethod || isConfigInitialization || differentVirtualMethod)
            {
                MsgWithDetails msg = db.ACSaveChanges();
                if (msg != null)
                    return msg;
            }
            return null;
        }

        public void GetConfigForMaterial(DatabaseApp dbApp, Guid materialID, ACMethod acMethod)
        {
            if (acMethod == null)
                acMethod = ACUrlACTypeSignature("!" + VMethodName_InOutOperation);

            Guid acClassIdOfParent = ParentACComponent.ComponentClass.ACClassID;

            var wayIndependent = dbApp.MaterialConfig.Where(c => c.MaterialID == materialID
                                                              && c.VBiACClassID == acClassIdOfParent).AsNoTracking();

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
        }

        public ACMethod GetParameters(PWWorkTaskScanBase pwNode, DatabaseApp dbApp, Guid materialID)
        {
            ACMethod acMethod = null;
            if (pwNode != null)
            {
                core.datamodel.ACClassMethod refPAACClassMethod = pwNode.RefACClassMethodOfContentWF;
                acMethod = refPAACClassMethod?.TypeACSignature();
                pwNode.ExecuteMethod(nameof(PWWorkTaskScanBase.GetConfigForACMethod), acMethod, true);
            }

            if (acMethod == null)
            {
                acMethod = ACUrlACTypeSignature("!" + VMethodName_InOutOperation);
            }
            GetConfigForMaterial(dbApp, materialID, acMethod);

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
                //    newACMethod.ParameterValueList[nameof(Route)] = null;
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

        public void AddToOperationLogList(OperationLog operationLog, ACMethod parameters, DatabaseApp dbApp)
        {
            OperationLogItem logItem = NewOperationLogItem(operationLog);
            WriteParametersToOperationLogItem(logItem, parameters);

            ApplicationManager.ApplicationQueue.Add(() =>
            {
                OperationLogItemList itemList = null;
                if (OperationLogItems.ValueT != null || OperationLogItems.ValueT.Any())
                {
                    itemList = new OperationLogItemList(OperationLogItems.ValueT.ToList());
                }
                else
                {
                    itemList = new OperationLogItemList();
                }

                itemList.Add(logItem);
                OperationLogItems.ValueT = itemList;
            });
        }


        public void RemoveFromOperationLogList(OperationLog operationLog)
        {
            ApplicationManager.ApplicationQueue.Add(() =>
            {
                if (OperationLogItems.ValueT == null || !OperationLogItems.ValueT.Any())
                return;

                var tempList = new OperationLogItemList(OperationLogItems.ValueT.ToList());
                OperationLogItem logItem = tempList.FirstOrDefault(c => c.FacilityChargeID == operationLog.FacilityChargeID);
                if (logItem != null)
                {
                    tempList.Remove(logItem);

                    OperationLogItems.ValueT = tempList;
                }
            });
        }

        private static OperationLogItem NewOperationLogItem(OperationLog operationLog)
        {
            OperationLogItem operationLogItem = new OperationLogItem();
            operationLogItem.FacilityChargeID = operationLog.FacilityChargeID.Value;

            Material material = operationLog.FacilityCharge.Material;
            if (operationLog.FacilityCharge.Partslist != null)
            {
                material = operationLog.FacilityCharge.Partslist.Material;
            }
            operationLogItem.MaterialID = material.MaterialID;
            operationLogItem.MaterialNo = material.MaterialNo;
            operationLogItem.MaterialName = material.MaterialName1;

            operationLogItem.LotNo = operationLog.FacilityCharge.FacilityLot?.LotNo;
            operationLogItem.SplitNo = operationLog.FacilityCharge.SplitNo;
            operationLogItem.TimeEntered = operationLog.OperationTime;
            operationLogItem.ProgramNo = operationLog.FacilityCharge.ProdOrderProgramNo;
            return operationLogItem;
        }

        private void WriteParametersToOperationLogItem(OperationLogItem logItem, ACMethod parameters)
        {
            if (logItem == null || parameters == null)
                return;

            try
            {
                ACValue durationValue = parameters.ParameterValueList.GetACValue("Duration");
                if (durationValue != null && durationValue.Value != null)
                {
                    TimeSpan ts = durationValue.ParamAsTimeSpan;
                    logItem.Duration = ts;
                }

                ACValue hintValue = parameters.ParameterValueList.GetACValue("Hint");
                if (hintValue != null && hintValue.Value != null)
                {
                    TimeSpan ts = hintValue.ParamAsTimeSpan;
                    logItem.HintDuration = ts;
                }

                ACValue maxDurationValue = parameters.ParameterValueList.GetACValue("MaxDuration");
                if (maxDurationValue != null && maxDurationValue.Value != null)
                {
                    TimeSpan ts = maxDurationValue.ParamAsTimeSpan;
                    logItem.MaxDuration = ts;
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), "WriteParametersToOperationLogItem(0)", e.Message);
            }
        }

        public OperationLogItemList GetOperationLogList()
        {
            OperationLogItemList result = new OperationLogItemList();

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                var operationLogs = dbApp.OperationLog.Include(c => c.FacilityCharge.Material)
                                                      .Include(c => c.FacilityCharge.FacilityLot)
                                                      .Include(c => c.FacilityCharge.Partslist)
                                                      .Where(c => c.RefACClassID == this.ComponentClass.ACClassID
                                                               && c.OperationState == (short)OperationLogStateEnum.Open)
                                                      .OrderBy(c => c.InsertDate)
                                                      .ToArray();

                List<PAProdOrderPartslistWFInfo> ordersForOccupation = GetWaitingProdOrderPartslistWFInfo();
                List<PAProdOrderPartslistWFInfo> ordersForRelease = GetActivatedProdOrderPartslistWFInfo();

                foreach (OperationLog operationLog in operationLogs)
                {
                    OperationLogItem logItem = NewOperationLogItem(operationLog);

                    ProdOrder pOrder = dbApp.ProdOrder.FirstOrDefault(c => c.ProgramNo == logItem.ProgramNo);

                    ProdOrderPartslist poPl = pOrder?.ProdOrderPartslist_ProdOrder.FirstOrDefault(c => c.Partslist.MaterialID == logItem.MaterialID);

                    PWWorkTaskScanBase pwNode = null;

                    if (poPl != null)
                    {
                        var order = ordersForOccupation.FirstOrDefault(c => c.POPId == poPl.ProdOrderPartslistID);
                        if (order == null)
                            order = ordersForRelease.FirstOrDefault(c => c.POPId == poPl.ProdOrderPartslistID);

                        if (order != null)
                            pwNode = ACUrlCommand(order.ACUrlWF) as PWWorkTaskGeneric;
                    }

                    ACMethod parameters = GetParameters(pwNode, dbApp, logItem.MaterialID);
                    WriteParametersToOperationLogItem(logItem, parameters);

                    result.Add(logItem);
                }
            }
            return result;
        }

        private void ApplicationManager_ProjectWorkCycleR10sec(object sender, EventArgs e)
        {
            TimeSpan duration = DateTime.Now - _LastCleanUpRun;
            if (duration.TotalSeconds > CleanUpOperationLogsAfter)
            {
                ApplicationManager.ApplicationQueue.Add(() => CheckAvailableQuantsAndCleanUpOperationLogs());
                _LastCleanUpRun = DateTime.Now;
            }
        }

        public void CheckAvailableQuantsAndCleanUpOperationLogs()
        {
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                

                var operationLogsToClose = dbApp.OperationLog.Where(c => c.RefACClassID == this.ComponentClass.ACClassID
                                                                      && c.OperationState == (short)OperationLogStateEnum.Open
                                                                      && c.FacilityCharge.NotAvailable)
                                                             .OrderBy(c => c.InsertDate)
                                                             .ToArray();

                if (!operationLogsToClose.Any())
                    return;

                if (OperationLogItems.ValueT == null || !OperationLogItems.ValueT.Any())
                    return;

                OperationLogItemList tempList = null;
                if (OperationLogItems.ValueT != null && OperationLogItems.ValueT.Any())
                    tempList = new OperationLogItemList(OperationLogItems.ValueT.ToList());

                foreach (OperationLog logToClose in operationLogsToClose)
                {
                    OperationLog.CloseOperationLog(dbApp, logToClose, null);

                    if (tempList != null)
                    {
                        OperationLogItem logItem = tempList.FirstOrDefault(c => c.FacilityChargeID == logToClose.FacilityChargeID);
                        if (logItem != null)
                            tempList.Remove(logItem);
                    }
                }

                if (tempList != null)
                    OperationLogItems.ValueT = tempList;
            }
        }

        #endregion
    }
}
