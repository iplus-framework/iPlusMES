using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System.Threading;
using static gip.mes.facility.ACPartslistManager.QrySilosResult;
using System.Text;

namespace gip.mes.processapplication
{
    public partial class PWDosing
    {
        #region Properties
        public ACProdOrderManager ProdOrderManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.ProdOrderManager : null;
            }
        }

        protected ACPartslistManager PartslistManager
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                return pwMethodProduction != null ? pwMethodProduction.PartslistManager : null;
            }
        }

        public static bool GetRelatedProdOrderPosForWFNode(PWBase pwNode, Database dbIPlus, DatabaseApp dbApp, PWMethodProduction pwMethodProduction,
            out ProdOrderPartslistPos intermediateChildPos, out ProdOrderPartslistPos intermediatePosition, out ProdOrderPartslistPos endBatchPos,
            out MaterialWFConnection matWFConnection, out ProdOrderBatch batch, out ProdOrderBatchPlan batchPlan, out MaterialWFConnection[] matWFConnections)
        {
            intermediateChildPos = null;
            matWFConnection = null;
            batch = null;
            batchPlan = null;
            intermediatePosition = null;
            matWFConnections = null;

            endBatchPos = pwMethodProduction.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
            if (pwMethodProduction.CurrentProdOrderBatch == null)
                return false;

            var contentACClassWFVB = pwNode.ContentACClassWF.FromAppContext<gip.mes.datamodel.ACClassWF>(dbApp);
            batch = pwMethodProduction.CurrentProdOrderBatch.FromAppContext<ProdOrderBatch>(dbApp);
            batchPlan = batch.ProdOrderBatchPlan;
            matWFConnection = null;
            if (batchPlan != null && batchPlan.MaterialWFACClassMethodID.HasValue)
            {
                ProdOrderBatchPlan batchPlan2 = batchPlan;

                matWFConnections = dbApp.MaterialWFConnection
                                        .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan2.MaterialWFACClassMethodID.Value
                                                && c.ACClassWFID == contentACClassWFVB.ACClassWFID).ToArray();

                matWFConnection = matWFConnections.FirstOrDefault();
            }
            else
            {
                PartslistACClassMethod plMethod = endBatchPos.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                if (plMethod != null)
                {
                    matWFConnections = dbApp.MaterialWFConnection
                                            .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID
                                                    && c.ACClassWFID == contentACClassWFVB.ACClassWFID).ToArray();

                    matWFConnection = matWFConnections.FirstOrDefault();
                }
                else
                {
                    ProdOrderPartslistPos endBatchPos2 = endBatchPos;
                    matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
                        .Where(c => c.MaterialWFACClassMethod.MaterialWFID == endBatchPos2.ProdOrderPartslist.Partslist.MaterialWFID
                                    && c.MaterialWFACClassMethod.PartslistACClassMethod_MaterialWFACClassMethod.Where(d => d.PartslistID == endBatchPos2.ProdOrderPartslist.PartslistID).Any())
                        .FirstOrDefault();
                }
            }

            if (matWFConnection == null)
                return false;

            MaterialWFConnection matWFConnection2 = matWFConnection;
            // Find intermediate position which is assigned to this Dosing-Workflownode
            var currentProdOrderPartslist = endBatchPos.ProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
            intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection2.MaterialID
                    && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                    && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();
            if (intermediatePosition == null)
                return false;

            // Lock, if a parallel Dosing also creates a child Position for this intermediate Position
            using (ACMonitor.Lock(pwMethodProduction._62000_PWGroupLockObj))
            {
                // Find intermediate child position, which is assigned to this Batch
                intermediateChildPos = intermediatePosition.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                    .Where(c => c.ProdOrderBatchID.HasValue
                                && c.ProdOrderBatchID.Value == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchID)
                    .FirstOrDefault();
            }
            return intermediateChildPos != null;
        }

        public virtual bool HasAnyMaterialToProcessProd
        {
            get
            {
                PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
                // If dosing is not for production, then do nothing
                if (pwMethodProduction == null)
                    return true;

                using (var dbIPlus = new Database())
                {
                    using (var dbApp = new DatabaseApp(dbIPlus))
                    {
                        ProdOrderPartslistPos intermediateChildPos;
                        MaterialWFConnection matWFConnection;
                        ProdOrderBatch batch;
                        ProdOrderBatchPlan batchPlan;
                        ProdOrderPartslistPos intermediatePos;
                        ProdOrderPartslistPos endBatchPos;
                        MaterialWFConnection[] matWFConnections;

                        bool posFound = GetRelatedProdOrderPosForWFNode(this, dbIPlus, dbApp, pwMethodProduction, out intermediateChildPos, out intermediatePos, 
                            out endBatchPos, out matWFConnection, out batch, out batchPlan, out matWFConnections);
                        if (!posFound)
                            return true;

                        DosingSkipMode skipComponentsMode = SkipComponentsMode;
                        ProdOrderPartslistPosRelation[] queryOpenDosings = OnGetOpenDosingsForNextComponent(dbIPlus, dbApp, intermediateChildPos);
                        if ((ComponentsSeqFrom > 0 || ComponentsSeqTo > 0) && queryOpenDosings != null && queryOpenDosings.Any())
                            queryOpenDosings = queryOpenDosings.Where(c => c.Sequence >= ComponentsSeqFrom && c.Sequence <= ComponentsSeqTo)
                                                                .OrderBy(c => c.Sequence)
                                                                .ToArray();

                        if (queryOpenDosings != null && queryOpenDosings.Any())
                        {
                            queryOpenDosings = OnSortOpenDosings(queryOpenDosings, dbIPlus, dbApp);
                            foreach (ProdOrderPartslistPosRelation relation in queryOpenDosings)
                            {
                                if (!relation.SourceProdOrderPartslistPos.Material.UsageACProgram)
                                    continue;

                                PAProcessFunction responsibleFunc = null;
                                core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
                                if (refPAACClassMethod == null)
                                    return true;
                                ACMethod acMethod = refPAACClassMethod.TypeACSignature();
                                if (acMethod == null)
                                    return true;

                                ACPartslistManager.QrySilosResult possibleSilos;

                                PAProcessModule module = ParentPWGroup.AccessedProcessModule != null ? ParentPWGroup.AccessedProcessModule : ParentPWGroup.FirstAvailableProcessModule;
                                if (module == null && ParentPWGroup.ProcessModuleList != null) // If all occupied, then use first that is generally possible 
                                    module = ParentPWGroup.ProcessModuleList.FirstOrDefault();
                                if (module == null)
                                    return true;
                                RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.StartDosing,
                                    OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                    null, null, ExcludedSilos, ReservationMode);
                                IEnumerable<Route> routes = GetRoutes(relation, dbApp, dbIPlus, queryParams, module, out possibleSilos);

                                // #SKIPMALZERS
                                // TODO: This is a temporary solution (Malzers) to prevent skipping dosings and afterwards it will be detected that the other node cannot dose the material.
                                // See also code below #SKIPMALZERS
                                if (   (routes == null || !routes.Any())
                                    && skipComponentsMode == DosingSkipMode.DifferentWFClasses)
                                {
                                    RouteQueryParams queryParams2 = new RouteQueryParams(RouteQueryPurpose.StartDosing, ACPartslistManager.SearchMode.AllSilos, null, null, ExcludedSilos, ReservationMode);
                                    ACPartslistManager.QrySilosResult possibleSilos2;
                                    IEnumerable<Route> routes2 = GetRoutes(relation, dbApp, dbIPlus, queryParams2, module, out possibleSilos2);
                                    if (routes2 != null && routes2.Any())
                                    {
                                        routes = routes2;
                                        possibleSilos = possibleSilos2;
                                    }
                                }

                                if (routes != null && routes.Any())
                                {
                                    List<Route> routesList = routes.ToList();
                                    module.GetACStateOfFunction(acMethod.ACIdentifier, out responsibleFunc);
                                    if (responsibleFunc == null)
                                    {
                                        return false;
                                    }

                                    PAFDosing dosingFunc = responsibleFunc as PAFDosing;
                                    if (dosingFunc != null)
                                    {
                                        foreach (Route currRoute in routes)
                                        {
                                            RouteItem lastRouteItem = currRoute.Items.LastOrDefault();
                                            if (lastRouteItem != null && lastRouteItem.TargetProperty != null)
                                            {
                                                // Gehe zur nächsten Komponente, weil es mehrere Dosierfunktionen gibt und der Eingangspunkt des Prozessmoduls nicht mit dem Eingangspunkt dieser Funktion übereinstimmt.
                                                // => eine andere Funktion ist dafür zuständig
                                                if (!dosingFunc.PAPointMatIn1.ConnectionList.Where(c => ((c as PAEdge).Source as PAPoint).ACIdentifier == lastRouteItem.TargetProperty.ACIdentifier).Any())
                                                {
                                                    routesList.Remove(currRoute);
                                                    //hasOpenDosings = true;
                                                    //continue;
                                                }
                                            }
                                        }
                                    }

                                    routes = routesList;
                                }

                                if (routes != null && routes.Any())
                                {
                                    return true;
                                }
                            }

                            return false;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        #endregion

        public virtual StartNextCompResult StartNextProdComponent(PAProcessModule module)
        {
            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            // If dosing is not for production, then do nothing
            if (pwMethodProduction == null)
            {
                return StartNextCompResult.Done;
            }

            Msg msg = null;
            if (ProdOrderManager == null)
            {
                // Error50167: ProdOrderManager is null.
                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(1)", 1000, "Error50167");

                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                return StartNextCompResult.CycleWait;
            }

            // Reduziere zyklische Datenbankabfragen über Zeitstempel
            var currentParallelPWDosings = CurrentParallelPWDosings;
            if (currentParallelPWDosings != null
                && currentParallelPWDosings.Where(c => c.CurrentACState != ACStateEnum.SMIdle).Any()
                && NextCheckIfPWDosingsFinished.HasValue && DateTime.Now < NextCheckIfPWDosingsFinished)
            {
                return StartNextCompResult.CycleWait;
            }

            using (var dbIPlus = new Database())
            {
                using (var dbApp = new DatabaseApp(dbIPlus))
                {
                    ProdOrderPartslistPos intermediateChildPos;
                    ProdOrderPartslistPos intermediatePosition;
                    MaterialWFConnection matWFConnection;
                    ProdOrderBatch batch;
                    ProdOrderBatchPlan batchPlan;
                    ProdOrderPartslistPos endBatchPos;
                    MaterialWFConnection[] matWFConnections;
                    bool posFound = GetRelatedProdOrderPosForWFNode(this, dbIPlus, dbApp, pwMethodProduction, 
                        out intermediateChildPos, out intermediatePosition, out endBatchPos, out matWFConnection, out batch, out batchPlan, out matWFConnections);
                    if (batch == null)
                    {
                        // Error50060: No batch assigned to last intermediate material of this workflow-process
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(2)", 1010, "Error50060");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return StartNextCompResult.CycleWait;
                    }
                    else if (matWFConnection == null)
                    {
                        // Error50059: No relation defined between Workflownode and intermediate material in Materialworkflow
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(3)", 1020, "Error50059");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return StartNextCompResult.CycleWait;
                    }
                    else if (intermediatePosition == null)
                    {
                        // Error50061: Intermediate line not found which is assigned to this Dosing-Workflownode
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(4)", 1030, "Error50061");

                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return StartNextCompResult.CycleWait;
                    }

                    using (ACMonitor.Lock(pwMethodProduction._62000_PWGroupLockObj))
                    {
                        // If intermediate child position not found, generate childposition for this Batch/Intermediate
                        if (intermediateChildPos == null)
                        {
                            List<object> resultNewEntities = new List<object>();
                            msg = ProdOrderManager.BatchCreate(dbApp, intermediatePosition, batch, endBatchPos.BatchFraction, batch.BatchSeqNo, resultNewEntities); // Toleranz ist max. ein Batch mehr
                            if (msg != null)
                            {
                                Messages.LogException(this.GetACUrl(), "StartNextProdComponent(5)", msg.InnerMessage);
                                dbApp.ACUndoChanges();
                                return StartNextCompResult.CycleWait;
                            }
                            else
                            {
                                dbApp.ACSaveChanges();
                            }
                            intermediateChildPos = resultNewEntities.Where(c => c is ProdOrderPartslistPos).FirstOrDefault() as ProdOrderPartslistPos;
                            if (intermediateChildPos != null && endBatchPos.FacilityLot != null)
                                endBatchPos.FacilityLot = endBatchPos.FacilityLot;
                        }
                    }

                    if (intermediateChildPos == null)
                    {
                        //Error50165:intermediateChildPos is null.
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(5a)", 1040, "Error50165");

                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return StartNextCompResult.CycleWait;
                    }

                    ProdOrderPartslistPosRelation[] queryOpenDosings = OnGetOpenDosingsForNextComponent(dbIPlus, dbApp, intermediateChildPos);
                    if ((ComponentsSeqFrom > 0 || ComponentsSeqTo > 0) && queryOpenDosings != null && queryOpenDosings.Any())
                        queryOpenDosings = queryOpenDosings.Where(c => c.Sequence >= ComponentsSeqFrom && c.Sequence <= ComponentsSeqTo)
                                                            .OrderBy(c => c.Sequence)
                                                            .ToArray();

                    var appManager = this.ApplicationManager;
                    ValueTracer vt = new ValueTracer(appManager != null ? appManager.ValueTraceOn : false);
                    bool hasOpenDosings = false;
                    bool? enoughMaterialScaleChangeNotNeeded = null;
                    bool isAnyCompDosableFromAnyRoutableSilo = false;
                    bool componentsSkippable = ComponentsSkippable;
                    DosingSkipMode skipComponentsMode = SkipComponentsMode;
                    StartNextCompResult openDosingsResult = StartNextCompResult.Done;

                    try
                    {
                        IEnumerable<IPWNodeReceiveMaterial> allParallelDosingWFs = GetParallelDosingWFs(dbApp, batchPlan, skipComponentsMode, intermediatePosition, endBatchPos);
                        IEnumerable<gip.core.datamodel.ACClass> allExcludedSilos = GetAllExcludedSilos(allParallelDosingWFs);

                        // Falls noch Dosierungen anstehen, dann dosiere nächste Komponente
                        if (queryOpenDosings != null && queryOpenDosings.Any())
                        {
                            queryOpenDosings = OnSortOpenDosings(queryOpenDosings, dbIPlus, dbApp);
                            foreach (ProdOrderPartslistPosRelation relation in queryOpenDosings)
                            {
                                if (!relation.SourceProdOrderPartslistPos.Material.UsageACProgram)
                                    continue;
                                double dosingWeight = relation.RemainingDosingWeight;
                                if (AdaptToTargetQ)
                                {
                                    var completedPos = dbApp.ProdOrderPartslistPosRelation.Where(c => c.SourceProdOrderPartslistPosID == relation.SourceProdOrderPartslistPosID
                                                                                    && c.TargetProdOrderPartslistPos.ParentProdOrderPartslistPosID == relation.TargetProdOrderPartslistPos.ParentProdOrderPartslistPosID
                                                                                    && c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed)
                                        .Select(c => new { TargetQ = c.TargetQuantityUOM, ActualQ = c.ActualQuantityUOM })
                                        .ToArray();
                                    double targetSum = 0;
                                    double actualSum = 0;
                                    foreach (var c in completedPos)
                                    {
                                        targetSum += c.TargetQ;
                                        actualSum += c.ActualQ;
                                    }
                                    if (targetSum > double.Epsilon && actualSum > double.Epsilon)
                                    {
                                        double restTargetQ = relation.SourceProdOrderPartslistPos.TargetQuantityUOM - targetSum;
                                        double restActualQ = relation.SourceProdOrderPartslistPos.TargetQuantityUOM - actualSum;
                                        // If quantity reached don't dose
                                        if (restTargetQ < FacilityConst.C_ZeroCompare || restActualQ < FacilityConst.C_ZeroCompare)
                                        {
                                            var posStateRelation = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                                            if (posStateRelation != null)
                                            {
                                                relation.MDProdOrderPartslistPosState = posStateRelation;
                                                dbApp.ACSaveChanges();
                                                continue;
                                            }
                                        }
                                        double correctionFactor = restActualQ / restTargetQ;
                                        dosingWeight = dosingWeight * correctionFactor;
                                    }
                                }

                                bool interDischargingNeeded = false;
                                IPAMContScale scale = module as IPAMContScale;
                                if (scale == null)
                                    scale = ParentPWGroup != null ? ParentPWGroup.AccessedProcessModule as IPAMContScale : null;
                                ScaleBoundaries scaleBoundaries = null;
                                gip.core.processapplication.PAEScaleTotalizing totalizingScale = TotalizingScaleIfSWT;
                                if (scale != null)
                                    scaleBoundaries = OnGetScaleBoundariesForDosing(scale, dbApp, queryOpenDosings, intermediateChildPos, intermediatePosition, matWFConnection, batch, batchPlan, endBatchPos);
                                if (scaleBoundaries != null && !IsAutomaticContinousWeighing)
                                {
                                    double? remainingWeight = null;
                                    if (scaleBoundaries.RemainingWeightCapacity.HasValue)
                                        remainingWeight = scaleBoundaries.RemainingWeightCapacity.Value;
                                    else if (scaleBoundaries.MaxWeightCapacity > 0.00000001)
                                        remainingWeight = scaleBoundaries.MaxWeightCapacity;
                                    if (!remainingWeight.HasValue)
                                    {
                                        if (!MaxWeightAlarmSet)
                                        {
                                            MaxWeightAlarmSet = true;
                                            //Error50162:MaxWeightCapacity of scale {0} is not configured.
                                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(5.1)", 1050, "Error50162", scale.GetACUrl());

                                            Messages.LogWarning(this.GetACUrl(), "StartNextProdComponent(5.1)", msg.InnerMessage);
                                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                            {
                                                Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                            }
                                        }
                                    }
                                    // FALL A:
                                    else if (Math.Abs(relation.RemainingDosingWeight) > remainingWeight.Value)
                                    {
                                        // Falls die Komponentensollmenge größer als die maximale Waagenkapazität ist, dann muss die Komponente gesplittet werden, 
                                        // ansonsten dosiere volle sollmenge nach der Zwischenentleerung
                                        if (scaleBoundaries.MaxWeightCapacity > 0.00000001 && relation.TargetWeight > scaleBoundaries.MaxWeightCapacity)
                                        {
                                            // Fall A.1:
                                            interDischargingNeeded = true;
                                            dosingWeight = remainingWeight.Value;
                                        }
                                        else
                                        {
                                            ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMInterDischarging;
                                            vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.Done);
                                            return StartNextCompResult.Done;
                                        }
                                    }

                                    if (scaleBoundaries.RemainingVolumeCapacity.HasValue
                                        && relation.SourceProdOrderPartslistPos.Material != null
                                        && relation.SourceProdOrderPartslistPos.Material.IsDensityValid)
                                    {
                                        double remainingDosingVolume = relation.SourceProdOrderPartslistPos.Material.ConvertToVolume(Math.Abs(relation.RemainingDosingQuantityUOM));
                                        if (remainingDosingVolume > scaleBoundaries.RemainingVolumeCapacity.Value)
                                        {
                                            double targetVolume = relation.SourceProdOrderPartslistPos.Material.ConvertToVolume(relation.TargetQuantityUOM);
                                            // FALL B:
                                            // Falls die Komponentenvolumen größer als die maximale Volumenkapazität ist, dann muss die Komponente gesplittet werden, 
                                            // ansonsten dosiere volle sollmenge nach der Zwischenentleerung
                                            if (scaleBoundaries.MaxVolumeCapacity > 0.00000001 && targetVolume > scaleBoundaries.MaxVolumeCapacity)
                                            {
                                                double dosingWeightAccordingVolume = (scaleBoundaries.RemainingVolumeCapacity.Value * relation.SourceProdOrderPartslistPos.Material.Density) / 1000;
                                                // Falls Dichte > 1000 g/dm³, dann kann das errechnete zu dosierende Teilgewicht größer als das Restgewicht in der Waage sein,
                                                // dann muss das Restgewicht genommen werden (interDischargingNeeded ist true wenn weiter oben die Restgewichtermittlung durchgeführt wurde 
                                                // und die komponentenmenge gesplittet werden musste. SIEHE FALL A.1)
                                                if (!interDischargingNeeded || dosingWeightAccordingVolume < dosingWeight)
                                                {
                                                    // FALL B.1:
                                                    dosingWeight = dosingWeightAccordingVolume;
                                                }
                                                // Prüfe erneut ob Restgewicht der Waage überschritten wird, falls ja reduziere die Restmenge
                                                // Dieser Fall kommt dann vor, wenn die Dichte > 1000 g/dm³ ist, jedoch die zu dosierende Komponentenmenge kleiner war als das Restgewicht der Waage.
                                                // Dann wurde interDischargingNeeded nicht gesetzt (FALL A ist nicht eingetreten).
                                                if (!remainingWeight.HasValue && dosingWeight > remainingWeight.Value)
                                                    dosingWeight = remainingWeight.Value;
                                                interDischargingNeeded = true;
                                            }
                                            else
                                            {
                                                ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMInterDischarging;
                                                vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.Done);
                                                return StartNextCompResult.Done;
                                            }
                                        }
                                    }
                                }

                                PAProcessFunction responsibleFunc = null;
                                core.datamodel.ACClassMethod refPAACClassMethod = RefACClassMethodOfContentWF;
                                ACMethod acMethod = refPAACClassMethod?.TypeACSignature();
                                if (acMethod == null)
                                {
                                    //Error50154: acMethod is null.
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(9a)", 1120, "Error50154");
                                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                    return StartNextCompResult.CycleWait;
                                }

                                MDProdOrderPartslistPosState posState;
                                ACPartslistManager.QrySilosResult possibleSilos;

                                // Finde heraus ob dieses Prozessmodul aus dem ältesten Silo dosieren kann.
                                RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.StartDosing,
                                    OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                    null, null, allExcludedSilos, ReservationMode);
                                IEnumerable<Route> routes = GetRoutes(relation, dbApp, dbIPlus, queryParams, null, out possibleSilos);

                                double correctedDosingWeight = dosingWeight;
                                Route preferredDosingRoute = null, alternativeDosingRoute = null;
                                ACPartslistManager.QrySilosResult.FacilitySumByLots preferredDosingFacility = null, alternativeDosingFacility = null, preferredDosingFacilityNotRoutableHere = null;

                                // Eine Route ist gefunden mit dem ältesten Silo auf diese Waage ist gefunden routes nicht leer ist!
                                if (routes != null && routes.Any())
                                {
                                    List<Route> routesList = routes.ToList();
                                    module.GetACStateOfFunction(acMethod.ACIdentifier, out responsibleFunc);
                                    if (responsibleFunc == null)
                                    {
                                        //Error50327: Responsible dosingfunction for ACMethod {0} not found. Please check your logical brige from the InPoints of the processmodule to the InPoint of the dosingfunction.
                                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(9b)", 1121, "Error50327", acMethod.ACIdentifier);
                                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                        vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                        return StartNextCompResult.CycleWait;
                                    }

                                    // Wenn Prozessmodul mehrere Eingangpunkte hat, dann entferne zuerst alle Routen, die nicht mit diesem Dosierknoten dosiert werden können
                                    PAFDosing dosingFunc = responsibleFunc as PAFDosing;
                                    if (dosingFunc != null)
                                    {
                                        foreach (Route currRoute in routes)
                                        {
                                            RouteItem lastRouteItem = currRoute.Items.LastOrDefault();
                                            if (lastRouteItem != null && lastRouteItem.TargetProperty != null)
                                            {
                                                // Gehe zur nächsten Komponente, weil es mehrere Dosierfunktionen gibt und der Eingangspunkt des Prozessmoduls nicht mit dem Eingangspunkt dieser Funktion übereinstimmt.
                                                // => eine andere Funktion ist dafür zuständig
                                                if (!dosingFunc.PAPointMatIn1.ConnectionList.Where(c => ((c as PAEdge).Source as PAPoint).ACIdentifier == lastRouteItem.TargetProperty.ACIdentifier).Any())
                                                {
                                                    routesList.Remove(currRoute);
                                                    //hasOpenDosings = true;
                                                    //continue;
                                                }
                                            }
                                        }
                                    }

                                    routes = routesList;

                                    bool areParallelDosingNodesStartable = HasStartableParallelDosingNodes(allParallelDosingWFs, skipComponentsMode, possibleSilos, relation);

                                    // Suche aus der Routenliste die Route heraus, die bevorzugt werden soll
                                    PriorizeSilosAndGetRoute(possibleSilos, ref routes, dosingWeight, areParallelDosingNodesStartable, OldestSilo,
                                                             out correctedDosingWeight, out preferredDosingRoute, out preferredDosingFacility, out alternativeDosingRoute,
                                                             out alternativeDosingFacility, out preferredDosingFacilityNotRoutableHere);
                                }

                                // OTHERWISE: If routes is empty, it may be that there are other silos that can be dosed on other process modules.
                                // In this case, possibleSilos is not empty && parallelDosingWFs has entries)
                                // or if the right silo was not found due to reservations
                                // check whether dosing would be possible on other dosing nodes
                                if ((routes == null || !routes.Any())
                                    || preferredDosingRoute == null)
                                {
                                    bool isOnlyTestForFindingBetterSiloWithReservedLots = routes != null && routes.Any() && preferredDosingRoute == null;

                                    bool hasOtherStartableDosingNodes = false;
                                    double correctedDosingWeight2 = 0.0;
                                    Route preferredDosingRoute2 = null, alternativeDosingRoute2 = null;
                                    ACPartslistManager.QrySilosResult.FacilitySumByLots preferredDosingFacility2 = null, alternativeDosingFacility2 = null, preferredDosingFacilityNotRoutableHere2 = null;

                                    if (allParallelDosingWFs != null
                                        && allParallelDosingWFs.Any()
                                        && (skipComponentsMode == DosingSkipMode.DifferentWFClasses || (possibleSilos != null && possibleSilos.FilteredResult != null && possibleSilos.FilteredResult.Any())))
                                    {
                                        List<IPWNodeReceiveMaterial> otherDosingWFs = allParallelDosingWFs.Where(c => (c as IPWNodeReceiveMaterial).IterationCount.ValueT <= 0
                                                                                                                || ((c as IPWNodeReceiveMaterial).ParentPWGroup != null
                                                                                                                    && (c as IPWNodeReceiveMaterial).ParentPWGroup.CurrentACSubState == (uint)ACSubStateEnum.SMInterDischarging))
                                                                                                       .ToList();
                                        // Remove potential WFNodes which are out of the SequenceRange
                                        if (otherDosingWFs.Any())
                                        {
                                            foreach (var otherDosingWF in otherDosingWFs.ToArray())
                                            {
                                                if (otherDosingWF.ComponentsSeqFrom > 0 && otherDosingWF.ComponentsSeqTo > 0
                                                    && (relation.Sequence < otherDosingWF.ComponentsSeqFrom || relation.Sequence > otherDosingWF.ComponentsSeqTo))
                                                {
                                                    otherDosingWFs.Remove(otherDosingWF);
                                                }
                                            }
                                        }

                                        // #SKIPMALZERS
                                        // This is a temporary solution (Malzers) to prevent skipping dosings and afterwards it will be detected that the other node cannot dose the material.
                                        // This temporaray solution checks if there are silos where the material is blocked, than the node should wait.
                                        // TODO: Better solution:
                                        // Check these other DosingsWF's if they are able to dose this material by checking their Routes.
                                        // Therefore it has to be determined which Processmodules will be mapped and then checked if the can dose from one of this possible silos
                                        // if not, then this Dosing node should not be completed, because the dosing has to happen here first.
                                        hasOtherStartableDosingNodes = otherDosingWFs.Any();
                                        if (!isOnlyTestForFindingBetterSiloWithReservedLots && hasOtherStartableDosingNodes && skipComponentsMode == DosingSkipMode.DifferentWFClasses)
                                        {
                                            RouteQueryParams queryParams2 = new RouteQueryParams(RouteQueryPurpose.StartDosing, ACPartslistManager.SearchMode.AllSilos, null, null, ExcludedSilos, ReservationMode);
                                            ACPartslistManager.QrySilosResult possibleSilos2;
                                            IEnumerable<Route> routes2 = GetRoutes(relation, dbApp, dbIPlus, queryParams2, null, out possibleSilos2);
                                            // If there are other blocked Silos on this scale:
                                            if (routes2 != null && routes2.Any())
                                            {
                                                hasOtherStartableDosingNodes = false;
                                            }
                                        }
                                    }

                                    // If there are other parallel nodes, find out if the others could be dosed
                                    // by indirectly checking if there are other silos that cannot be dosed on this scale.
                                    if (hasOtherStartableDosingNodes)
                                    {
                                        bool continueToNextComp = false;
                                        // search condition is, find all silos (regardless of prioritization) and check whether one of these silos could also be dosed on this process module
                                        queryParams = new RouteQueryParams(RouteQueryPurpose.StartDosing, ACPartslistManager.SearchMode.AllSilos, null, null, ExcludedSilos, ReservationMode);
                                        ACPartslistManager.QrySilosResult possibleSilos2;
                                        IEnumerable<Route> routes2 = GetRoutes(relation, dbApp, dbIPlus, queryParams, null, out possibleSilos2);
                                        List<FacilitySumByLots> silosNotDosableHere = null;
                                        if (this.DontWaitForChangeScale)
                                            silosNotDosableHere = GetFirstSilosNotDosableHere(routes2, possibleSilos2, allExcludedSilos);
                                        // Case A) routes2 is not empty if there are lower priority silos that can be dosed on this process module:
                                        if (routes2 != null && routes2.Any())
                                        {
                                            continueToNextComp = true;
                                            if (isOnlyTestForFindingBetterSiloWithReservedLots)
                                            {
                                                PriorizeSilosAndGetRoute(possibleSilos2, ref routes2, dosingWeight, hasOtherStartableDosingNodes, OldestSilo,
                                                                         out correctedDosingWeight2, out preferredDosingRoute2, out preferredDosingFacility2, out alternativeDosingRoute2, out alternativeDosingFacility2,
                                                                         out preferredDosingFacilityNotRoutableHere2);
                                                continueToNextComp = false;
                                                // If better Silo found on other scale, then continue dosing on other sale
                                                if (preferredDosingRoute2 == null
                                                    && preferredDosingFacilityNotRoutableHere2 != null
                                                    && preferredDosingFacilityNotRoutableHere != null
                                                    && preferredDosingFacilityNotRoutableHere2.StorageBin.FacilityID == preferredDosingFacilityNotRoutableHere.StorageBin.FacilityID)
                                                {
                                                    continueToNextComp = true;
                                                    vt.Set<bool>(ref isAnyCompDosableFromAnyRoutableSilo, true);
                                                }
                                                // This is the case, when a Silo is blocked on this scale that would have the right reserved lots
                                                // then dose from an alternative silo with free quants on this scale if allowed (ReservationMode = 0)
                                                else if (preferredDosingRoute2 != null)
                                                {
                                                    continueToNextComp = false;
                                                }
                                                // This is the case, when a Silo is blocked on another scale that would have the right reserved lots
                                                // then dose from an alternative silo with free quants on this scale if allowed (ReservationMode = 0)
                                                else if (preferredDosingFacilityNotRoutableHere2 != null
                                                         && preferredDosingFacilityNotRoutableHere != null
                                                         && preferredDosingFacilityNotRoutableHere2.StorageBin.FacilityID != preferredDosingFacilityNotRoutableHere.StorageBin.FacilityID)
                                                {
                                                    continueToNextComp = false;
                                                }
                                            }
                                            else
                                                vt.Set<bool>(ref isAnyCompDosableFromAnyRoutableSilo, true);
                                        }
                                        // Case B) There are other silos that CANNOT be dosed on this process module, then go to the next component and try to dose it
                                        else if (possibleSilos2 != null && possibleSilos2.FoundSilos.Any())
                                        {
                                            continueToNextComp = true;
                                        }
                                        // Case C) There is no other scale or silo => DO NOT go to the next component and wait
                                        else
                                            continueToNextComp = false;

                                        // Try to dose NEXT component first, because there are other dosing nodes that could take over the dosing
                                        if (continueToNextComp)
                                        {
                                            if (silosNotDosableHere != null && DontWaitForChangeScale)
                                            {
                                                double minStock = CalcMinStockForScaleChange(StockFactorForChangeScale, relation.RemainingDosingWeight);
                                                double sumStock = silosNotDosableHere.Sum(c => c.StockOfReservations.HasValue ? c.StockOfReservations.Value : (c.StockFree.HasValue ? c.StockFree.Value : 0));
                                                if (sumStock < minStock)
                                                {
                                                    if (!enoughMaterialScaleChangeNotNeeded.HasValue || enoughMaterialScaleChangeNotNeeded.Value)
                                                        vt.Set<bool?>(ref enoughMaterialScaleChangeNotNeeded, false);
                                                    vt.Set<bool>(ref hasOpenDosings, true);
                                                }
                                                else
                                                {
                                                    if (!enoughMaterialScaleChangeNotNeeded.HasValue)
                                                        vt.Set<bool?>(ref enoughMaterialScaleChangeNotNeeded, true);
                                                }
                                            }
                                            else
                                                vt.Set<bool>(ref hasOpenDosings, true);
                                            continue;
                                        }
                                    }

                                    // Wenn keine anderen Knoten gefunden worden (continue im oberen Block ist nicht durchgleaufen) sind aber noch offene Komponenten da, die dosiert werden müssten, dann gebe Fehlermeldung heraus.
                                    if (routes == null || !routes.Any())
                                    {
                                        // TODO: Was passiert wenn noch eine Komponente kommt, muss skaliert werden?
                                        if (NoSourceFoundForDosing.ValueT == 0)
                                        {
                                            // Warning50005: No Silo/Tank/Container found for component {0}
                                            msg = new Msg(this, eMsgLevel.Warning, PWClassName, "StartNextProdComponent(6)", 1060, "Warning50005",
                                                            relation.SourceProdOrderPartslistPos.Material.MaterialName1);

                                            NoSourceWait = DateTime.Now + TimeSpan.FromSeconds(10);
                                            NoSourceFoundForDosing.ValueT = 1;
                                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                                Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                        }
                                        else if (NoSourceFoundForDosing.ValueT == 2)
                                        {
                                            posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                                            if (posState == null)
                                            {
                                                // Error50062: posState ist null at Order {0}, BillofMaterial {1}, Line {2}
                                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(7)", 1070, "Error50062",
                                                                intermediateChildPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                                                intermediateChildPos.ProdOrderPartslist.Partslist.PartslistNo,
                                                                intermediateChildPos.BookingMaterial.MaterialName1);

                                                if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                                    Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                                vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                                return StartNextCompResult.CycleWait;
                                            }
                                            relation.MDProdOrderPartslistPosState = posState;
                                            dbApp.ACSaveChanges();
                                            vt.Set<bool>(ref hasOpenDosings, true);
                                            continue; // Gehe zur nächsten Komponente
                                        }

                                        HandleNoSourceFoundForDosing(relation, dbApp, dbIPlus, queryParams, possibleSilos);
                                        vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                        return StartNextCompResult.CycleWait;
                                    }
                                }
                                else if (NoSourceFoundForDosing.ValueT == 1)
                                {
                                    NoSourceFoundForDosing.ValueT = 0;
                                    AcknowledgeAlarms();
                                }

                                //If no Silo found and usage of other lots allowed, then use first Silo
                                if (preferredDosingRoute == null && ReservationMode == 1)
                                {
                                    preferredDosingRoute = alternativeDosingRoute;
                                    preferredDosingFacility = alternativeDosingFacility;
                                }

                                Route dosingRoute = preferredDosingRoute;

                                if (dosingRoute == null || double.IsNaN(relation.RemainingDosingWeight))
                                {
                                    //if (possibleSilos.HasLotReservations)

                                    if (NoSourceFoundForDosing.ValueT == 0 && dosingRoute == null)
                                    {
                                        NoSourceWait = DateTime.Now + TimeSpan.FromSeconds(10);
                                        NoSourceFoundForDosing.ValueT = 1;

                                        // Error50063: No Route found for dosing component {2} at Order {0}, bill of material{1}
                                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(8)", 1080, "Error50063",
                                                        intermediateChildPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                                                                 intermediateChildPos.ProdOrderPartslist.Partslist.PartslistNo,
                                                                                 relation.SourceProdOrderPartslistPos.Material.MaterialName1);

                                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                        vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                        return StartNextCompResult.CycleWait;
                                    }
                                    else if (NoSourceFoundForDosing.ValueT == 2)
                                    {
                                        posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                                        if (posState == null)
                                        {
                                            // Error50062: posState ist null at Order {0}, BillofMaterial {1}, Line {2}
                                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(9)", 1090, "Error50062",
                                                            intermediateChildPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                                            intermediateChildPos.ProdOrderPartslist.Partslist.PartslistNo,
                                                            intermediateChildPos.BookingMaterial.MaterialName1);

                                            if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                                Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                            vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                            return StartNextCompResult.CycleWait;
                                        }
                                        relation.MDProdOrderPartslistPosState = posState;
                                        dbApp.ACSaveChanges();
                                        vt.Set<bool>(ref hasOpenDosings, true);
                                        continue; // Gehe zur nächsten Komponente
                                    }
                                }
                                else if (NoSourceFoundForDosing.ValueT == 1)
                                {
                                    NoSourceFoundForDosing.ValueT = 0;
                                    AcknowledgeAlarms();
                                }

                                CurrentDosingRoute = dosingRoute;
                                NoSourceFoundForDosing.ValueT = 0;

                                // 4. Starte Dosierung von diesem Silo aus
                                #region Start Dosing on Module

                                PAMSilo sourceSilo = CurrentDosingSilo(null);
                                if (sourceSilo == null)
                                {
                                    // Error50064: Property sourceSilo is null at Order {0}, Bill of material {1}, Line {2}
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(8)", 1100, "Error50064",
                                                    intermediateChildPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                                    intermediateChildPos.ProdOrderPartslist.Partslist.PartslistNo,
                                                    intermediateChildPos.BookingMaterial.MaterialName1);
                                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                        Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                    return StartNextCompResult.CycleWait;
                                }
                                posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.InProcess).FirstOrDefault();
                                if (posState == null)
                                {
                                    // Error50065: MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.InProcess is null at Order {0}, Bill of material {1}, Line {2}
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(9)", 1110, "Error50065",
                                                    intermediateChildPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                                                             intermediateChildPos.ProdOrderPartslist.Partslist.PartslistNo,
                                                                             intermediateChildPos.BookingMaterial.MaterialName1);

                                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                        Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                    return StartNextCompResult.CycleWait;
                                }

                                responsibleFunc = CanStartProcessFunc(module, acMethod, dbApp, relation, endBatchPos, intermediatePosition, batch, sourceSilo);
                                if (responsibleFunc == null)
                                {
                                    vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                    return StartNextCompResult.CycleWait;
                                }

                                if (relation != null && double.IsNaN(relation.RemainingDosingWeight))
                                {
                                    NoSourceFoundForDosing.ValueT = 1;
                                    //Error50597: Dosing error on the component {0} {1}, {2};
                                    string error = relation.RemainingDosingWeightError;
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(9a)", 1111, "Error50597", relation.SourceProdOrderPartslistPos.MaterialNo, relation.SourceProdOrderPartslistPos.MaterialName, error);
                                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                    return StartNextCompResult.CycleWait;
                                }

                                if (double.IsNaN(correctedDosingWeight))
                                {
                                    //Error50597: Dosing error on the component {0} {1}, {2};
                                    string error = "NaN";
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(9b)", 1111, "Error50597", relation.SourceProdOrderPartslistPos.MaterialNo, relation.SourceProdOrderPartslistPos.MaterialName, error);
                                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                    return StartNextCompResult.CycleWait;
                                }

                                if (!(bool)ExecuteMethod(nameof(GetConfigForACMethod), acMethod, true, dbApp, relation, endBatchPos, intermediatePosition, batch, sourceSilo))
                                {
                                    vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                    return StartNextCompResult.CycleWait;
                                }

                                PADosingLastBatchEnum lastBatchMode = PADosingLastBatchEnum.None;
                                int countOpenDosings = queryOpenDosings.Count();
                                if ((pwMethodProduction != null
                                        && (((ACSubStateEnum)pwMethodProduction.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                                            || ((ACSubStateEnum)pwMethodProduction.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
                                     )
                                    || (ParentPWGroup != null
                                        && (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrder)
                                            || ((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
                                       )
                                    )
                                {
                                    lastBatchMode = countOpenDosings <= 1 ? PADosingLastBatchEnum.LastBatchAndComponent : PADosingLastBatchEnum.LastBatch;
                                }
                                else if (countOpenDosings <= 1)
                                    lastBatchMode = PADosingLastBatchEnum.LastComponent;
                                else if (pwMethodProduction != null)
                                    lastBatchMode = pwMethodProduction.IsLastBatch;

                                acMethod[PWMethodVBBase.IsLastBatchParamName] = (short)lastBatchMode;

                                acMethod["PLPosRelation"] = relation.ProdOrderPartslistPosRelationID;
                                if (!ValidateAndSetRouteForParam(acMethod, dosingRoute))
                                {
                                    vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                    return StartNextCompResult.CycleWait;
                                }
                                acMethod["Source"] = sourceSilo.RouteItemIDAsNum;
                                acMethod["TargetQuantity"] = Math.Abs(correctedDosingWeight);
                                if (IsAutomaticContinousWeighing && totalizingScale != null)
                                {
                                    var acValue = acMethod.ParameterValueList.GetACValue("SWTWeight");
                                    if (acValue != null)
                                        acValue.Value = totalizingScale.SWTTipWeight;
                                }
                                acMethod[Material.ClassName] = relation.SourceProdOrderPartslistPos.Material.MaterialName1;
                                if (relation.SourceProdOrderPartslistPos.Material.Density > 0.00001)
                                    acMethod["Density"] = relation.SourceProdOrderPartslistPos.Material.Density;
                                if (dosingRoute != null)
                                    dosingRoute.Detach(true);

                                if (!(bool)ExecuteMethod(nameof(AfterConfigForACMethodIsSet), acMethod, true, dbApp, relation, endBatchPos, intermediatePosition, batch, sourceSilo))
                                {
                                    vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                    return StartNextCompResult.CycleWait;
                                }

                                if (!acMethod.IsValid())
                                {
                                    // Error50066: Dosingtask not startable Order {0}, Bill of material {1}, line {2}
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(10)", 1130, "Error50066",
                                                    intermediateChildPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                                    intermediateChildPos.ProdOrderPartslist.Partslist.PartslistNo,
                                                    intermediateChildPos.BookingMaterial.MaterialName1);

                                    if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                        Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                    return StartNextCompResult.CycleWait;
                                }

                                RecalcTimeInfo(true);
                                CurrentDosingPos.ValueT = relation.ProdOrderPartslistPosRelationID;
                                if (CreateNewProgramLog(acMethod, _NewAddedProgramLog == null) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                                {
                                    vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                    return StartNextCompResult.CycleWait;
                                }
                                _ExecutingACMethod = acMethod;

                                module.TaskInvocationPoint.ClearMyInvocations(this);
                                _CurrentMethodEventArgs = null;
                                if (!IsTaskStarted(module.TaskInvocationPoint.AddTask(acMethod, this)))
                                {
                                    ACMethodEventArgs eM = _CurrentMethodEventArgs;
                                    if (eM == null || eM.ResultState != Global.ACMethodResultState.FailedAndRepeat)
                                    {
                                        // Error50066: Dosingtask not startable Order {0}, Bill of material {1}, line {2}
                                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(11)", 1140, "Error50066",
                                                    intermediateChildPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                                    intermediateChildPos.ProdOrderPartslist.Partslist.PartslistNo,
                                                    intermediateChildPos.BookingMaterial.MaterialName1);

                                        if (IsAlarmActive(ProcessAlarm, msg.Message) == null)
                                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    }
                                    CurrentDosingPos.ValueT = Guid.Empty;
                                    vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                    return StartNextCompResult.CycleWait;
                                }
                                UpdateCurrentACMethod();

                                if (interDischargingNeeded)
                                    ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMInterDischarging;

                                CachedEmptySiloHandlingOption = null;
                                relation.MDProdOrderPartslistPosState = posState;
                                MsgWithDetails msg2 = dbApp.ACSaveChanges();
                                if (msg2 != null)
                                {
                                    Messages.LogException(this.GetACUrl(), "StartNextProdComponent(5)", msg2.InnerMessage);
                                    OnNewAlarmOccurred(ProcessAlarm, new Msg(msg2.InnerMessage, this, eMsgLevel.Exception, PWClassName, "StartNextProdComponent", 1150), true);
                                    vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                                    return StartNextCompResult.CycleWait;
                                }
                                AcknowledgeAlarms();
                                ExecuteMethod(nameof(OnACMethodSended), acMethod, true, dbApp, relation, endBatchPos, intermediatePosition, batch, sourceSilo, responsibleFunc);
                                vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.NextCompStarted);
                                return StartNextCompResult.NextCompStarted;
                                #endregion
                            }
                        }


                        if ((hasOpenDosings && componentsSkippable) || !hasOpenDosings)
                            vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.Done);
                        else
                            vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);


                        // Check if there are any parallel steps that still dose the last component.
                        // Still waiting for these to be done, because otherwise it would no longer be possible to change scales
                        currentParallelPWDosings = CurrentParallelPWDosings;
                        vt.LogVariable((currentParallelPWDosings == null || !currentParallelPWDosings.Any()) ? "0" : "1", nameof(currentParallelPWDosings));
                        //if (currentParallelPWDosings == null
                        //    || currentParallelPWDosings.Where(c => c.CurrentACState != ACStateEnum.SMIdle).Any())
                        //{
                        //    // Reduziere zyklische Datenbankabfragen über Zeitstempel
                        //    if (NextCheckIfPWDosingsFinished.HasValue && DateTime.Now < NextCheckIfPWDosingsFinished)
                        //    {
                        //        vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                        //        return StartNextCompResult.CycleWait;
                        //    }

                            CurrentParallelPWDosings = null;
                            NextCheckIfPWDosingsFinished = null;
                            ProdOrderPartslistPosRelation[] queryActiveDosings = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray()
                                                                                .Where(c => c.MDProdOrderPartslistPosState != null
                                                                                            && c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.InProcess)
                                                                                .OrderBy(c => c.Sequence)
                                                                                .ToArray();
                            if (ComponentsSeqFrom > 0 && ComponentsSeqTo > 0 && queryActiveDosings != null && queryActiveDosings.Any())
                                queryActiveDosings = queryActiveDosings.Where(c => c.Sequence >= ComponentsSeqFrom && c.Sequence <= ComponentsSeqTo)
                                                                    .OrderBy(c => c.Sequence)
                                                                    .ToArray();
                            if ((queryActiveDosings == null || !queryActiveDosings.Any())
                                && openDosingsResult == StartNextCompResult.Done)
                            {
                                NextCheckIfPWDosingsFinished = null;
                                CurrentParallelPWDosings = null;
                                if (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging))
                                    ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMIdle;
                                vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.Done);
                                return StartNextCompResult.Done;
                            }


                            // Find the other nodes that are currently dosing
                            if (queryActiveDosings != null && queryActiveDosings.Any())
                            {
                                var openRelations = queryActiveDosings.Select(c => c.ProdOrderPartslistPosRelationID);
                                CurrentParallelPWDosings = RootPW.FindChildComponents<PWDosing>(c => c is PWDosing
                                                                        && c != this
                                                                        && (c as PWDosing).CurrentDosingPos.ValueT != Guid.Empty
                                                                        && openRelations.Contains((c as PWDosing).CurrentDosingPos.ValueT))
                                                                        .ToList();
                                currentParallelPWDosings = CurrentParallelPWDosings.ToList();
                                vt.LogVariable((currentParallelPWDosings == null || !currentParallelPWDosings.Any()) ? "0" : "1", nameof(currentParallelPWDosings));

                                // Check whether the parallel active dosings could still be dosed on this scale for a potential scale change
                                //if (!isAnyCompDosableFromAnyRoutableSilo)
                                //{
                                foreach (var activeDosing in queryActiveDosings)
                                {
                                    ACPartslistManager.QrySilosResult possibleSilos;
                                    RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.StartDosing, ACPartslistManager.SearchMode.AllSilos, null, null, ExcludedSilos, ReservationMode);
                                    IEnumerable<Route> routes = GetRoutes(activeDosing, dbApp, dbIPlus, queryParams, null, out possibleSilos);
                                    if (routes != null && routes.Any())
                                    {
                                        if (this.DontWaitForChangeScale)
                                        {
                                            PWDosing activePWDos = currentParallelPWDosings.Where(c => c.CurrentDosingPos.ValueT == activeDosing.ProdOrderPartslistPosRelationID).FirstOrDefault();
                                            if (activePWDos != null)
                                            {
                                                PAMSilo currentSilo = activePWDos.CurrentDosingSilo(null);
                                                if (currentSilo != null)
                                                {
                                                    double minStock = CalcMinStockForScaleChange(StockFactorForChangeScale, activeDosing.RemainingDosingWeight);
                                                    if (currentSilo.FillLevel.ValueT >= minStock)
                                                    {
                                                        currentParallelPWDosings.Remove(activePWDos);
                                                        vt.LogVariable((currentParallelPWDosings == null || !currentParallelPWDosings.Any()) ? "0" : "1", nameof(currentParallelPWDosings));
                                                    }
                                                    else
                                                    {
                                                        List<FacilitySumByLots> silosNotDosableHere = GetFirstSilosNotDosableHere(routes, possibleSilos, allExcludedSilos);
                                                        silosNotDosableHere.RemoveAll(c => c.StorageBin.VBiFacilityACClassID.HasValue && c.StorageBin.VBiFacilityACClassID.Value == currentSilo.ComponentClass.ACClassID);
                                                        double sumStock = silosNotDosableHere.Sum(c => c.StockOfReservations.HasValue ? c.StockOfReservations.Value : (c.StockFree.HasValue ? c.StockFree.Value : 0));
                                                        if (sumStock < minStock)
                                                        {
                                                            if (enoughMaterialScaleChangeNotNeeded.HasValue && enoughMaterialScaleChangeNotNeeded.Value)
                                                                vt.Set<bool?>(ref enoughMaterialScaleChangeNotNeeded, false);
                                                            vt.Set<bool>(ref isAnyCompDosableFromAnyRoutableSilo, true);
                                                        }
                                                        else
                                                        {
                                                            currentParallelPWDosings.Remove(activePWDos);
                                                            vt.LogVariable((currentParallelPWDosings == null || !currentParallelPWDosings.Any()) ? "0" : "1", nameof(currentParallelPWDosings));
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (enoughMaterialScaleChangeNotNeeded.HasValue && enoughMaterialScaleChangeNotNeeded.Value)
                                                        vt.Set<bool?>(ref enoughMaterialScaleChangeNotNeeded, false);
                                                    vt.Set<bool>(ref isAnyCompDosableFromAnyRoutableSilo, true);
                                                }
                                            }
                                            else
                                            {
                                                if (enoughMaterialScaleChangeNotNeeded.HasValue && enoughMaterialScaleChangeNotNeeded.Value)
                                                    vt.Set<bool?>(ref enoughMaterialScaleChangeNotNeeded, false);
                                                vt.Set<bool>(ref isAnyCompDosableFromAnyRoutableSilo, true);
                                            }
                                        }
                                        else
                                            vt.Set<bool>(ref isAnyCompDosableFromAnyRoutableSilo, true);
                                    }
                                }
                                //}
                            }


                            if (!isAnyCompDosableFromAnyRoutableSilo
                                || ((currentParallelPWDosings == null || !currentParallelPWDosings.Any() || (DontWaitForChangeScale && enoughMaterialScaleChangeNotNeeded.HasValue && enoughMaterialScaleChangeNotNeeded.Value))
                                    && openDosingsResult == StartNextCompResult.Done))
                            {
                                NextCheckIfPWDosingsFinished = null;
                                CurrentParallelPWDosings = null;
                                if (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging))
                                    ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMIdle;
                                vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.Done);
                                return StartNextCompResult.Done;
                            }

                            // Nächste Datenbankprüfung in 20 Sekunden
                            NextCheckIfPWDosingsFinished = DateTime.Now.AddSeconds(20);
                            vt.Set<StartNextCompResult>(ref openDosingsResult, StartNextCompResult.CycleWait);
                            return StartNextCompResult.CycleWait;
                        //}

                        //NextCheckIfPWDosingsFinished = null;
                        //CurrentParallelPWDosings = null;
                        //if (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging))
                        //    ParentPWGroup.CurrentACSubState = (uint)ACSubStateEnum.SMIdle;
                        //return openDosingsResult;
                    }
                    finally
                    {
                        if (vt.IsTracing && DontWaitForChangeScale)
                            Messages.LogDebug(this.GetACUrl(), String.Format("DontWaitForChangeScale({0})", openDosingsResult), vt.Trace);
                    }
                }
            }
        }

        protected void PriorizeSilosAndGetRoute(ACPartslistManager.QrySilosResult possibleSilos, ref IEnumerable<Route> routes,
            double dosingWeight, bool isParallelDosingNodesStartable, bool priorizeOldestSilo,
            out double correctedDosingWeight, 
            out Route preferredDosingRoute, out ACPartslistManager.QrySilosResult.FacilitySumByLots preferredDosingFacility,
            out Route alternativeDosingRoute, out ACPartslistManager.QrySilosResult.FacilitySumByLots alternativeDosingFacility,
            out ACPartslistManager.QrySilosResult.FacilitySumByLots preferredDosingFacilityNotRoutableHere)
        {
            correctedDosingWeight = Math.Abs(dosingWeight);
            preferredDosingRoute = null;
            preferredDosingFacility = null;
            alternativeDosingRoute = null;
            alternativeDosingFacility = null;
            preferredDosingFacilityNotRoutableHere = null;
            if (possibleSilos == null || possibleSilos.FilteredResult == null || !possibleSilos.FilteredResult.Any() || routes == null)
                return;

            bool reservationBasedSearch = possibleSilos.FilteredResult.Where(c => c.StockOfReservations.HasValue).Any();

            foreach (ACPartslistManager.QrySilosResult.FacilitySumByLots prioSilo in possibleSilos.FilteredResult)
            {
                if (!prioSilo.StorageBin.VBiFacilityACClassID.HasValue)
                    continue;
                Route dosingRoute = routes.Where(c => c.FirstOrDefault().Source.ACClassID == prioSilo.StorageBin.VBiFacilityACClassID).FirstOrDefault();
                
                if (dosingRoute != null)
                {
                    if (alternativeDosingRoute == null)
                    {
                        alternativeDosingRoute = preferredDosingRoute;
                        alternativeDosingFacility = prioSilo;
                    }
                    if (reservationBasedSearch)
                    {
                        // Reduce quantity to available stock of Lots
                        if (prioSilo.StockOfReservations.HasValue)
                        {
                            if (prioSilo.StockOfReservations.Value <= 0.0)
                            {
                                // If material from other lots, than dosing is not allowed from this silo
                                if (prioSilo.StockFree.HasValue)
                                    continue;
                                // No other lots in silo, then try to empty silo completely
                                else
                                {
                                    //correctedDosingWeight = correctedDosingWeight;
                                    preferredDosingFacility = prioSilo;
                                    preferredDosingRoute = dosingRoute;
                                    break;
                                }
                            }
                            else
                            {
                                // If foreign lots are in this silo, then reduce dosing weight to remaining stock of reserved lots
                                if (prioSilo.StockOfReservations.Value < correctedDosingWeight
                                    && prioSilo.StockFree.HasValue)
                                    correctedDosingWeight = prioSilo.StockOfReservations.Value;
                                // else try to dose the target weight or try to empty silo
                                preferredDosingFacility = prioSilo;
                                preferredDosingRoute = dosingRoute;
                                break;
                            }
                        }
                    }
                    else
                    {
                        preferredDosingFacility = prioSilo;
                        preferredDosingRoute = dosingRoute;
                        break;
                    }
                }
                else if (preferredDosingFacilityNotRoutableHere == null)
                {
                    if (reservationBasedSearch)
                    {
                        // Reduce quantity to available stock of Lots
                        if (prioSilo.StockOfReservations.HasValue)
                        {
                            if (prioSilo.StockOfReservations.Value <= 0.0)
                            {
                                // No other lots in silo, then try to empty silo completely
                                if (!prioSilo.StockFree.HasValue)
                                    preferredDosingFacilityNotRoutableHere = prioSilo;
                            }
                            else
                            {
                                preferredDosingFacilityNotRoutableHere = prioSilo;
                                //break;
                            }
                        }
                    }
                    else
                    {
                        preferredDosingFacilityNotRoutableHere = prioSilo;
                        if (isParallelDosingNodesStartable && priorizeOldestSilo)
                        {
                            routes = null;
                            break;
                        }
                    }
                }
            }

            correctedDosingWeight *= -1;

            // If no Silo found and usage of other lots allowed, then use first Silo
            //if (preferredDosingRoute == null && ReservationMode == 1)
            //{
            //    preferredDosingRoute = alternativeDosingRoute;
            //    preferredDosingFacility = alternativeDosingFacility;
            //}
        }

        /// <summary>
        /// This method returns all Silos which must be dosed on another scale before the first silo on this scale is needed 
        /// </summary>
        /// <param name="possibleSilos"></param>
        /// <param name="routes"></param>
        /// <returns></returns>
        protected List<FacilitySumByLots> GetFirstSilosNotDosableHere(IEnumerable<Route> routes, ACPartslistManager.QrySilosResult possibleSilos, IEnumerable<gip.core.datamodel.ACClass> allExcludedSilos, bool removeDisabledFirst = true)
        {
            List<FacilitySumByLots> notDosableHere = new List<FacilitySumByLots>();
            if (possibleSilos == null)
                return notDosableHere;
            List<FacilitySumByLots> validSilos = new List<FacilitySumByLots>();
            foreach (FacilitySumByLots prioSilo in possibleSilos.SortedFilteredResult)
            {
                if (!prioSilo.StorageBin.VBiFacilityACClassID.HasValue || (allExcludedSilos != null && allExcludedSilos.Where(c => c.ACClassID == prioSilo.StorageBin.VBiFacilityACClassID).Any()))
                    continue;
                if (removeDisabledFirst && !prioSilo.StorageBin.OutwardEnabled)
                    continue;
                validSilos.Add(prioSilo);
            }
            foreach (FacilitySumByLots prioSilo in validSilos)
            {
                if (routes != null)
                {
                    Route dosingRoute = routes.Where(c => c.FirstOrDefault().Source.ACClassID == prioSilo.StorageBin.VBiFacilityACClassID).FirstOrDefault();
                    if (dosingRoute == null)
                        notDosableHere.Add(prioSilo);
                    else
                        break;
                }
                else
                    notDosableHere.Add(prioSilo);
            }
            return notDosableHere;
        }

        protected virtual ProdOrderPartslistPosRelation[] OnSortOpenDosings(ProdOrderPartslistPosRelation[] queryOpenDosings, Database dbIPlus, DatabaseApp dbApp)
        {
            return queryOpenDosings;
        }

        protected virtual ProdOrderPartslistPosRelation[] OnGetOpenDosingsForNextComponent(Database dbIPlus, DatabaseApp dbApp, ProdOrderPartslistPos intermediateChildPos)
        {
            ProdOrderPartslistPosRelation[] queryOpenDosings = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray()
                                .Where(c => (c.RemainingDosingWeight < (MinDosQuantity * -1) || double.IsNaN(c.RemainingDosingWeight))
                                            && c.MDProdOrderPartslistPosState != null
                                            && (c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created
                                               || c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted))
                                .OrderBy(c => c.Sequence)
                                .ToArray();

            return queryOpenDosings;
        }

        private bool ManageDosingStateProd(ManageDosingStatesMode mode, DatabaseApp dbApp, out double sumQuantity)
        {
            sumQuantity = 0.0;
            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            if (pwMethodProduction == null)
                return false;
            ProdOrderPartslistPos endBatchPos = pwMethodProduction.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
            if (pwMethodProduction.CurrentProdOrderBatch == null)
                return false;

            var contentACClassWFVB = ContentACClassWF.FromAppContext<gip.mes.datamodel.ACClassWF>(dbApp);
            ProdOrderBatch batch = pwMethodProduction.CurrentProdOrderBatch.FromAppContext<ProdOrderBatch>(dbApp);
            ProdOrderBatchPlan batchPlan = batch.ProdOrderBatchPlan;

            MaterialWFConnection matWFConnection = null;
            if (batchPlan != null && batchPlan.MaterialWFACClassMethodID.HasValue)
            {
                matWFConnection = dbApp.MaterialWFConnection
                                        .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan.MaterialWFACClassMethodID.Value
                                                && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                        .FirstOrDefault();
            }
            else
            {
                PartslistACClassMethod plMethod = endBatchPos.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                if (plMethod != null)
                {
                    matWFConnection = dbApp.MaterialWFConnection
                                            .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID
                                                    && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                            .FirstOrDefault();
                }
                else
                {
                    matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
                        .Where(c => c.MaterialWFACClassMethod.MaterialWFID == endBatchPos.ProdOrderPartslist.Partslist.MaterialWFID
                                    && c.MaterialWFACClassMethod.PartslistACClassMethod_MaterialWFACClassMethod.Where(d => d.PartslistID == endBatchPos.ProdOrderPartslist.PartslistID).Any())
                        .FirstOrDefault();
                }
            }


            //MaterialWFConnection matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
            //    .Where(c => c.MaterialWFACClassMethod.MaterialWFID == endBatchPos.ProdOrderPartslist.Partslist.MaterialWFID)
            //    .FirstOrDefault();
            if (matWFConnection == null)
                return false;

            // Find intermediate position which is assigned to this Dosing-Workflownode
            var currentProdOrderPartslist = endBatchPos.ProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
            ProdOrderPartslistPos intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                    && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                    && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();
            if (intermediatePosition == null)
                return false;

            ProdOrderPartslistPos intermediateChildPos = null;
            // Lock, if a parallel Dosing also creates a child Position for this intermediate Position

            using (ACMonitor.Lock(pwMethodProduction._62000_PWGroupLockObj))
            {
                // Find intermediate child position, which is assigned to this Batch
                intermediateChildPos = intermediatePosition.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                    .Where(c => c.ProdOrderBatchID.HasValue
                                && c.ProdOrderBatchID.Value == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchID)
                    .FirstOrDefault();

                if (intermediateChildPos == null)
                    return false;
            }
            if (intermediateChildPos == null)
                return false;

            // Falls noch Dosierungen anstehen, dann dosiere nächste Komponente
            if (mode == ManageDosingStatesMode.ResetDosings || mode == ManageDosingStatesMode.SetDosingsCompleted)
            {
                if (ParentPWGroup == null || ParentPWGroup.AccessedProcessModule == null)
                    return false;
                string acUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();

                bool changed = false;
                var posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted).FirstOrDefault();
                if (posState != null)
                {
                    var queryDosings = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray();
                    foreach (var childPos in queryDosings)
                    {
                        // Suche alle Dosierungen die auf DIESER Waage stattgefunden haben
                        var unconfirmedBookings = childPos.FacilityBooking_ProdOrderPartslistPosRelation.Where(c => c.PropertyACUrl == acUrl && c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.New);
                        if (unconfirmedBookings.Any())
                        {
                            changed = true;
                            // Falls alle Komponenten entleert, setze Status auf Succeeded
                            foreach (var booking in unconfirmedBookings)
                            {
                                if (mode == ManageDosingStatesMode.SetDosingsCompleted)
                                    booking.MaterialProcessState = GlobalApp.MaterialProcessState.Processed;
                                else // (mode == ManageDosingStatesMode.ResetDosings)
                                    booking.MaterialProcessState = GlobalApp.MaterialProcessState.Discarded;
                                sumQuantity += booking.OutwardQuantity;
                            }
                            // Sonderentleerung, setze Status auf Teilerledigt
                            if (mode == ManageDosingStatesMode.ResetDosings)
                            {
                                childPos.MDProdOrderPartslistPosState = posState;
                            }
                        }
                    }
                }
                return changed;
            }
            else
            {
                if (mode == ManageDosingStatesMode.QueryOpenDosings)
                {
                    var queryOpenDosings = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray()
                                        .Where(c => c.RemainingDosingWeight < -1.0 // TODO: Unterdosierung ist Min-Dosiermenge auf Waage
                                                    && c.MDProdOrderPartslistPosState != null
                                                    && (c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created
                                                        || c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted))
                                        .OrderBy(c => c.Sequence);
                    bool any = queryOpenDosings.Any();
                    if (any)
                        sumQuantity = queryOpenDosings.Sum(c => c.RemainingDosingQuantityUOM);
                    return any;
                }
                else if (mode == ManageDosingStatesMode.QueryHasAnyDosings)
                {
                    bool any = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Any();
                    if (any)
                        sumQuantity = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.Sum(c => c.TargetQuantityUOM);
                    return any;
                }
                else //if (mode == ManageDosingStatesMode.QueryDosedComponents)
                {
                    if (ParentPWGroup == null)
                        return false;

                    PAProcessModule apm = ParentPWGroup.AccessedProcessModule != null ? ParentPWGroup.AccessedProcessModule : ParentPWGroup.PreviousAccessedPM;
                    if (apm == null)
                    {
                        apm = HandleNotFoundPMOnManageDosingStateProd(mode, dbApp);
                        if (apm == null)
                            return false;
                    }
                    string acUrl = apm.GetACUrl();
                    var queryDosings = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray();
                    bool hasDosings = false;
                    foreach (var childPos in queryDosings)
                    {
                        var bookings = childPos.FacilityBooking_ProdOrderPartslistPosRelation.Where(c => c.PropertyACUrl == acUrl 
                                                        && (c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.New
                                                           || c.MaterialProcessStateIndex == (short)GlobalApp.MaterialProcessState.Processed));
                        if (bookings.Any())
                        {
                            sumQuantity += bookings.Sum(c => c.OutwardQuantity);
                            hasDosings = true;
                        }
                    }
                    return hasDosings;
                }
            }
        }

        protected virtual bool HasAndCanProcessAnyMaterialProd(PAProcessModule module)
        {
            PWMethodProduction pwMethodProduction = ParentPWMethod<PWMethodProduction>();
            if (pwMethodProduction == null)
                return false;
            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                ProdOrderPartslistPos endBatchPos = pwMethodProduction.CurrentProdOrderPartslistPos.FromAppContext<ProdOrderPartslistPos>(dbApp);
                if (pwMethodProduction.CurrentProdOrderBatch == null)
                    return false;

                var contentACClassWFVB = ContentACClassWF.FromAppContext<gip.mes.datamodel.ACClassWF>(dbApp);
                ProdOrderBatch batch = pwMethodProduction.CurrentProdOrderBatch.FromAppContext<ProdOrderBatch>(dbApp);
                ProdOrderBatchPlan batchPlan = batch.ProdOrderBatchPlan;

                MaterialWFConnection matWFConnection = null;
                if (batchPlan != null && batchPlan.MaterialWFACClassMethodID.HasValue)
                {
                    matWFConnection = dbApp.MaterialWFConnection
                                            .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan.MaterialWFACClassMethodID.Value
                                                    && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                            .FirstOrDefault();
                }
                else
                {
                    PartslistACClassMethod plMethod = endBatchPos.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                    if (plMethod != null)
                    {
                        matWFConnection = dbApp.MaterialWFConnection
                                                .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID
                                                        && c.ACClassWFID == contentACClassWFVB.ACClassWFID)
                                                .FirstOrDefault();
                    }
                    else
                    {
                        matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
                            .Where(c => c.MaterialWFACClassMethod.MaterialWFID == endBatchPos.ProdOrderPartslist.Partslist.MaterialWFID
                                        && c.MaterialWFACClassMethod.PartslistACClassMethod_MaterialWFACClassMethod.Where(d => d.PartslistID == endBatchPos.ProdOrderPartslist.PartslistID).Any())
                            .FirstOrDefault();
                    }
                }


                //MaterialWFConnection matWFConnection = contentACClassWFVB.MaterialWFConnection_ACClassWF
                //    .Where(c => c.MaterialWFACClassMethod.MaterialWFID == endBatchPos.ProdOrderPartslist.Partslist.MaterialWFID)
                //    .FirstOrDefault();
                if (matWFConnection == null)
                    return false;

                // Find intermediate position which is assigned to this Dosing-Workflownode
                var currentProdOrderPartslist = endBatchPos.ProdOrderPartslist.FromAppContext<ProdOrderPartslist>(dbApp);
                ProdOrderPartslistPos intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                    .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                        && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                        && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();
                if (intermediatePosition == null)
                    return false;

                ProdOrderPartslistPos intermediateChildPos = null;
                // Lock, if a parallel Dosing also creates a child Position for this intermediate Position

                using (ACMonitor.Lock(pwMethodProduction._62000_PWGroupLockObj))
                {
                    // Find intermediate child position, which is assigned to this Batch
                    intermediateChildPos = intermediatePosition.ProdOrderPartslistPos_ParentProdOrderPartslistPos
                        .Where(c => c.ProdOrderBatchID.HasValue
                                    && c.ProdOrderBatchID.Value == pwMethodProduction.CurrentProdOrderBatch.ProdOrderBatchID)
                        .FirstOrDefault();

                    if (intermediateChildPos == null)
                        return false;
                }
                if (intermediateChildPos == null)
                    return false;
                var queryOpenDosings = intermediateChildPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray()
                                    .Where(c => c.RemainingDosingWeight < -1.0 // TODO: Unterdosierung ist Min-Dosiermenge auf Waage
                                                && c.MDProdOrderPartslistPosState != null
                                                && (c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created
                                                    || c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted))
                                    .OrderBy(c => c.Sequence);
                bool any = queryOpenDosings.Any();
                if (!any)
                    return false;

                if (queryOpenDosings != null && queryOpenDosings.Any())
                {
                    DosingSkipMode skipComponentsMode = SkipComponentsMode;
                    foreach (ProdOrderPartslistPosRelation relation in queryOpenDosings)
                    {
                        if (!relation.SourceProdOrderPartslistPos.Material.UsageACProgram)
                            continue;

                        ACPartslistManager.QrySilosResult possibleSilos;
                        IEnumerable<IPWNodeReceiveMaterial> parallelDosingWFs = GetParallelDosingWFs(dbApp, batchPlan, skipComponentsMode, intermediatePosition, endBatchPos);
                        IEnumerable<gip.core.datamodel.ACClass> allExcludedSilos = GetAllExcludedSilos(parallelDosingWFs);

                        RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.StartDosing,
                                OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                null, null, allExcludedSilos, ReservationMode);
                        IEnumerable<Route> routes = GetRoutes(relation, dbApp, dbIPlus, queryParams, module, out possibleSilos);
                        if (routes != null && routes.Any())
                            return true;
                    }
                }
            }

            return false;
        }

        protected virtual PAProcessModule HandleNotFoundPMOnManageDosingStateProd(ManageDosingStatesMode mode, DatabaseApp dbApp)
        {
            return null;
        }

        protected virtual EmptySiloHandlingOptions HandleAbortReasonOnEmptySiloProd(PAMSilo silo)
        {
            if (!IsProduction)
                return EmptySiloHandlingOptions.NoSilosAvailable;
            if (CachedEmptySiloHandlingOption.HasValue)
                return CachedEmptySiloHandlingOption.Value;

            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                ProdOrderPartslistPosRelation dosingPosRelation = dbApp.ProdOrderPartslistPosRelation.Where(c => c.ProdOrderPartslistPosRelationID == CurrentDosingPos.ValueT).FirstOrDefault();
                if (dosingPosRelation == null)
                {
                    CachedEmptySiloHandlingOption = EmptySiloHandlingOptions.NoSilosAvailable;
                    return CachedEmptySiloHandlingOption.Value;
                }

                ProdOrderPartslistPos batchPos = dosingPosRelation.TargetProdOrderPartslistPos;

                // Automatischer Silowechsel nur dann möglich wenn noch Routen vorhanden zu anderen Silos die dosiert werden können
                if (batchPos != null && dosingPosRelation != null)
                {
                    if (silo.Facility.ValueT == null || silo.Facility.ValueT.ValueT == null)
                    {
                        CachedEmptySiloHandlingOption = EmptySiloHandlingOptions.NoSilosAvailable;
                        return CachedEmptySiloHandlingOption.Value;
                    }

                    ACPartslistManager.QrySilosResult possibleSilos;
                    RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.HandleEmptySilo,
                        OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                        null, silo.Facility.ValueT.ValueT.FacilityID, ExcludedSilos, ReservationMode);
                    IEnumerable<Route> routes = GetRoutes(dosingPosRelation, dbApp, dbIPlus, queryParams, null, out possibleSilos);
                    if (routes == null || !routes.Any())
                    {
                        if (/*DontWaitForChangeScale && */possibleSilos != null && possibleSilos.FilteredResult != null && possibleSilos.FilteredResult.Any())
                        {
                            var parallelActiveDosings = RootPW.FindChildComponents<PWDosing>(c => c is PWDosing
                                    && c != this
                                    && (c as PWDosing).CurrentACState != ACStateEnum.SMIdle
                                    && (c as PWDosing).ParentPWGroup != null
                                    && (c as PWDosing).ParentPWGroup.AccessedProcessModule != null)
                                    .ToList();
                            if (parallelActiveDosings != null && parallelActiveDosings.Any())
                            {
                                foreach (var otherDosing in parallelActiveDosings)
                                {
                                    ACPartslistManager.QrySilosResult alternativeSilos;
                                    RouteQueryParams queryParams2 = new RouteQueryParams(RouteQueryPurpose.HandleEmptySilo,
                                        OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                        null, silo.Facility.ValueT.ValueT.FacilityID, ExcludedSilos, ReservationMode);
                                    IEnumerable<Route> alternativeRoutes = otherDosing.GetRoutes(dosingPosRelation, dbApp, dbIPlus, queryParams2, null, out alternativeSilos);
                                    if (alternativeRoutes != null && alternativeRoutes.Any())
                                    {
                                        CachedEmptySiloHandlingOption = EmptySiloHandlingOptions.OtherSilosAvailable | EmptySiloHandlingOptions.AvailableOnOtherModule;
                                        if (queryParams2.SuggestedOptionResult > 0)
                                            CachedEmptySiloHandlingOption |= queryParams2.SuggestedOptionResult;
                                        return CachedEmptySiloHandlingOption.Value;
                                    }
                                }
                            }
                        }
                        CachedEmptySiloHandlingOption = EmptySiloHandlingOptions.NoSilosAvailable;
                        if (queryParams.SuggestedOptionResult > 0)
                            CachedEmptySiloHandlingOption |= queryParams.SuggestedOptionResult;
                        return CachedEmptySiloHandlingOption.Value;
                    }
                    else
                    {
                        CachedEmptySiloHandlingOption = EmptySiloHandlingOptions.OtherSilosAvailable | EmptySiloHandlingOptions.AvailabeOnThisModule;
                        if (queryParams.SuggestedOptionResult > 0)
                            CachedEmptySiloHandlingOption |= queryParams.SuggestedOptionResult;
                        return CachedEmptySiloHandlingOption.Value;
                    }
                }
            }
            CachedEmptySiloHandlingOption = EmptySiloHandlingOptions.NoSilosAvailable;
            return CachedEmptySiloHandlingOption.Value;
        }

        public virtual Msg CanResumeDosingProd()
        {
            if (CurrentDosingPos == null || CurrentDosingPos.ValueT == Guid.Empty)
                return null;
            Msg msg = null;
            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                ProdOrderPartslistPosRelation dosingPosRelation = dbApp.ProdOrderPartslistPosRelation.Where(c => c.ProdOrderPartslistPosRelationID == CurrentDosingPos.ValueT).FirstOrDefault();
                if (dosingPosRelation == null)
                    return null;
                RouteItem dosingSource = CurrentDosingSource(null);
                if (dosingSource == null)
                {
                    // Error50081: No Route found for booking component {2} at Order {0}, bill of material{1}
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "CanResumeDosingProd(1)", 1310, "Error50081",
                                    dosingPosRelation.SourceProdOrderPartslistPos.BookingMaterial.MaterialName1,
                                    dosingPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                    dosingPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo);
                    return msg;
                }

                Facility outwardFacility = dbApp.Facility.Where(c => c.VBiFacilityACClassID == dosingSource.SourceGuid).FirstOrDefault();
                if (outwardFacility == null)
                {
                    // Error50082: Facitlity not found for booking component {2} at Order {0}, bill of material{1}
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "CanResumeDosingProd(2)", 1311, "Error50082",
                                    dosingPosRelation.SourceProdOrderPartslistPos.BookingMaterial.MaterialName1,
                                    dosingPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                    dosingPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo);
                    return msg;
                }

                if (!outwardFacility.MaterialID.HasValue)
                {
                    // Error50262: The currently dosing Source {0} is not occupied with a material.
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "CanResumeDosingProd(3)", 1312, "Error50262", outwardFacility.FacilityName);
                    return msg;
                }

                 

                if (!  (   (dosingPosRelation.SourceProdOrderPartslistPos.Material.ProductionMaterialID.HasValue && outwardFacility.MaterialID == dosingPosRelation.SourceProdOrderPartslistPos.Material.ProductionMaterialID)
                        || (!dosingPosRelation.SourceProdOrderPartslistPos.Material.ProductionMaterialID.HasValue && outwardFacility.MaterialID == dosingPosRelation.SourceProdOrderPartslistPos.MaterialID)))
                {
                    // Error50263: The dosing Material {0} / {1} doesn't match Material {2} / {3} in Source {4}.
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "CanResumeDosingProd(4)", 1313, "Error50263",
                                dosingPosRelation.SourceProdOrderPartslistPos.Material.MaterialNo, dosingPosRelation.SourceProdOrderPartslistPos.Material.MaterialName1,
                                outwardFacility.Material.MaterialNo, outwardFacility.Material.MaterialName1,
                                outwardFacility.FacilityName);
                    return msg;
                }

                if (outwardFacility.PartslistID.HasValue 
                    && dosingPosRelation.SourceProdOrderPartslistPos.Material.IsIntermediate 
                    && outwardFacility.PartslistID != dosingPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.PartslistID)
                {
                    // Error50264: The dosing bill of material (BOM) {0} / {1} doesn't match BOM {2} / {3} in Source {4}.
                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "CanResumeDosingProd(5)", 1314, "Error50264",
                                dosingPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo, dosingPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistName,
                                outwardFacility.Partslist.PartslistNo, outwardFacility.Partslist.PartslistName,
                                outwardFacility.FacilityName);
                    return msg;
                }
            }
            return null;
        }

        public virtual Msg DoDosingBookingProd(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject,
                                    PADosingAbortReason dosingFuncResultState, PAFDosing dosing,
                                    string dis2SpecialDest, bool reEvaluatePosState,
                                    double? actualQuantity, double? tolerancePlus, double? toleranceMinus, double? targetQuantity,
                                    bool isEndlessDosing, bool thisDosingIsInTol)
        {
            MsgWithDetails collectedMessages = new MsgWithDetails();
            Msg msg = null;
            if (!IsProduction)
            {
                return new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "DoDosingBookingProd(1)",
                    Message = "IsNotProduction"
                };
            }

            using (var dbIPlus = new Database())
            using (var dbApp = new DatabaseApp(dbIPlus))
            {
                try
                {
                    MDProdOrderPartslistPosState posState;
                    // Falls in Toleranz oder Dosierung abgebrochen ohne Grund, dann beende Position
                    if (thisDosingIsInTol
                        || dosingFuncResultState == PADosingAbortReason.CompCancelled
                        || dosingFuncResultState == PADosingAbortReason.EmptySourceEndBatchplan
                        || dosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenNextComp
                        || dosingFuncResultState == PADosingAbortReason.EndTolErrorDosingThenDisThenNextComp
                        || dosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenEnd
                        || dosingFuncResultState == PADosingAbortReason.EndTolErrorDosingThenDisThenEnd
                        || dosingFuncResultState == PADosingAbortReason.EmptySourceAbortAdjustOtherAndWait
                        )
                    {
                        posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                    }
                    else
                    {
                        posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted).FirstOrDefault();
                    }
                    if (posState == null)
                    {
                        // Error50080: MDProdOrderPartslistPosState for Completed-State doesn't exist
                        msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoDosingBookingProd(1)", 1160, "Error50080");

                        Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                        OnNewAlarmOccurred(ProcessAlarm, msg, true);
                        return msg;
                    }

                    ProdOrderPartslistPosRelation dosingPosRelation = dbApp.ProdOrderPartslistPosRelation.Where(c => c.ProdOrderPartslistPosRelationID == CurrentDosingPos.ValueT).FirstOrDefault();
                    if (dosingPosRelation != null)
                    {
                        bool changePosState = false;
                        //dosingPosRelation.MDProdOrderPartslistPosState = posState;

                        RouteItem dosingSource = CurrentDosingSource(null);
                        if (dosingSource == null)
                        {
                            // Error50081: No Route found for booking component {2} at Order {0}, bill of material{1}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoDosingBookingProd(2)", 1170, "Error50081",
                                            dosingPosRelation.SourceProdOrderPartslistPos.BookingMaterial.MaterialName1,
                                            dosingPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                            dosingPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo);

                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            return msg;
                        }

                        Facility outwardFacility = dbApp.Facility.Where(c => c.VBiFacilityACClassID == dosingSource.SourceGuid).FirstOrDefault();
                        if (outwardFacility == null)
                        {
                            // Error50082: Facitlity not found for booking component {2} at Order {0}, bill of material{1}
                            msg = new Msg(this, eMsgLevel.Error, PWClassName, "DoDosingBookingProd(3)", 1180, "Error50082",
                                            dosingPosRelation.SourceProdOrderPartslistPos.BookingMaterial.MaterialName1,
                                            dosingPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo,
                                            dosingPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.Partslist.PartslistNo);

                            Messages.LogError(this.GetACUrl(), msg.ACIdentifier, msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, msg, true);
                            return msg;
                        }

                        changePosState = true;
                        // Falls dosiert

                        OnDoDosingBookingRelationGetPostingQuantity(collectedMessages, sender, e, wrapObject, dbApp, dosingPosRelation, outwardFacility, dosingFuncResultState, ref actualQuantity);

                        ProcessBooking(collectedMessages, reEvaluatePosState, sender, e, wrapObject, dbApp, dosingPosRelation, outwardFacility,
                                       dosingFuncResultState, dosing, dis2SpecialDest, actualQuantity, tolerancePlus, toleranceMinus, targetQuantity,
                                       isEndlessDosing, thisDosingIsInTol, msg, ref posState, ref changePosState);

                        bool odbd = OnDosingBookingDone(collectedMessages, sender, e, wrapObject, dbApp, dosingPosRelation, outwardFacility, dosingFuncResultState);

                        if (odbd)
                        {
                            if (       (dosingFuncResultState == PADosingAbortReason.EmptySourceNextSource
                                    || dosingFuncResultState == PADosingAbortReason.EmptySourceEndBatchplan
                                    || dosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenEnd
                                    || dosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenNextComp
                                    || dosingFuncResultState == PADosingAbortReason.EmptySourceAbortAdjustOtherAndWait)
                                    && outwardFacility.ReservedQuantity > -0.0001) // Prevent setting to zero stock. Used for virtual silos like city water that never shorts
                            {
                                bool anyOtherFunctionActiveFromThisSilo = false;
                                // Before Silo is posted to Zero, ensure that other functions that are dosing from this same silo make their posting also
                                // Otherwise the stock not be correct
                                // Therefore only the last dosing that finishes can book this Silo to empty stock
                                if (outwardFacility.VBiFacilityACClass != null && !String.IsNullOrEmpty(outwardFacility.VBiFacilityACClass.ACURLComponentCached))
                                {
                                    PAMSilo currentSourceSilo = ACUrlCommand(outwardFacility.VBiFacilityACClass.ACURLComponentCached) as PAMSilo;
                                    if (currentSourceSilo != null)
                                    {
                                        IEnumerable<PAFDosing> dosingList = currentSourceSilo.GetActiveDosingsFromThisSilo();
                                        if (dosingList != null && dosingList.Any())
                                        {
                                            foreach (PAFDosing activeDosingFunct in dosingList)
                                            {
                                                if (activeDosingFunct != dosing
                                                    && activeDosingFunct.CurrentACState != ACStateEnum.SMIdle
                                                    && activeDosingFunct.IsTransportActive)
                                                {
                                                    if (activeDosingFunct.DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
                                                    {
                                                        // This Method invokes normally an Abort or Stop, an then this function is called rekursively again!
                                                        activeDosingFunct.SetAbortReasonEmptyForced();
                                                    }
                                                }
                                            }

                                            // Check again if one of those functions are active, beacuse function didn't complete. Therfore the stock mustn't be set to zero!
                                            foreach (PAFDosing activeDosingFunct in dosingList)
                                            {
                                                if (activeDosingFunct != dosing
                                                    && activeDosingFunct.CurrentACState != ACStateEnum.SMIdle
                                                    && activeDosingFunct.IsTransportActive)
                                                {
                                                    anyOtherFunctionActiveFromThisSilo = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!anyOtherFunctionActiveFromThisSilo)
                                {
                                    // Querytest, if antoher function has already posted silo to zero
                                    bool hasQuants = dbApp.FacilityCharge.Where(c => c.FacilityID == outwardFacility.FacilityID && c.NotAvailable == false).Any();

                                    //bool zeroBookSucceeded = false;
                                    if (hasQuants)
                                    {
                                        if (outwardFacility.OrderPostingOnEmptying)
                                        {
                                            OnBookingToOrderOnEmptying(collectedMessages, reEvaluatePosState, sender, e, wrapObject, dbApp, dosingPosRelation, outwardFacility,
                                                                        dosingFuncResultState, dosing, dis2SpecialDest, actualQuantity, tolerancePlus, toleranceMinus, targetQuantity,
                                                                        isEndlessDosing, thisDosingIsInTol, msg, posState, ref changePosState);
                                        }

                                        //zeroBookSucceeded = true;
                                        ACMethodBooking zeroBooking = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_Facility_BulkMaterial, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking;
                                        zeroBooking = zeroBooking.Clone() as ACMethodBooking;
                                        zeroBooking.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(dbApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
                                        zeroBooking.InwardFacility = outwardFacility;
                                        if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                                            zeroBooking.PropertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();
                                        //zeroBooking.OutwardFacility = outwardFacility;
                                        zeroBooking.IgnoreIsEnabled = true;
                                        ACMethodEventArgs resultBooking = ACFacilityManager.BookFacilityWithRetry(ref zeroBooking, dbApp);
                                        if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                                        {
                                            collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                            //zeroBookSucceeded = false;
                                            OnNewAlarmOccurred(ProcessAlarm, new Msg(zeroBooking.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoDosingBookingProd", 1240), true);
                                        }
                                        else
                                        {
                                            if (!zeroBooking.ValidMessage.IsSucceded() || zeroBooking.ValidMessage.HasWarnings())
                                            {
                                                if (!zeroBooking.ValidMessage.IsSucceded())
                                                    collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                                                Messages.LogError(this.GetACUrl(), "DoDosingBookingProd(10)", zeroBooking.ValidMessage.InnerMessage);
                                                OnNewAlarmOccurred(ProcessAlarm, new Msg(zeroBooking.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoDosingBookingProd", 1230), true);
                                            }
                                            //else
                                                //zeroBookSucceeded = true;
                                        }
                                    }

                                    // Handle ShouldLeaveMaterialOccupation when is not handled in FacilityManager
                                    if (!hasQuants && outwardFacility != null && outwardFacility.Material != null && !outwardFacility.ShouldLeaveMaterialOccupation)
                                    {
                                        PAMSilo sourceSilo = null;
                                        bool disChargingActive = false;
                                        if (outwardFacility.FacilityACClass != null)
                                        {
                                            string url = outwardFacility.FacilityACClass.GetACUrlComponent();
                                            if (!String.IsNullOrEmpty(url))
                                            {
                                                sourceSilo = ACUrlCommand(url) as PAMSilo;
                                                if (sourceSilo != null)
                                                {
                                                    IEnumerable<PAFDischarging> activeDischargings = sourceSilo.GetActiveDischargingsToThisSilo();
                                                    disChargingActive = activeDischargings != null && activeDischargings.Any();
                                                }
                                            }
                                        }

                                        // #iP-T-24-05-08-002
                                        // LeaveMaterialOccupation
                                        if (!disChargingActive)
                                        {
                                            outwardFacility.Material = null; // Automatisches Löschen der Belegung?
                                            outwardFacility.Partslist = null;
                                        }
                                    }

                                    msg = dbApp.ACSaveChangesWithRetry();
                                    if (msg != null)
                                    {
                                        collectedMessages.AddDetailMessage(msg);
                                        Messages.LogError(this.GetACUrl(), "DoDosingBookingProd(8)", msg.InnerMessage);
                                        OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoDosingBookingProd", 1220), true);
                                    }
                                    OnBookingEmptySource(collectedMessages, sender, e, wrapObject, dbApp, dosingPosRelation, dosingFuncResultState);
                                }
                            }
                        }

                        // Positionstate must be set at last because of conccurrency-Problems if another Scale(PWGroup) is waiting for starting this dosing in the Applicationthread
                        if (changePosState)
                            dosingPosRelation.MDProdOrderPartslistPosState = posState;
                        if (changePosState || dbApp.IsChanged)
                        { 
                            msg = dbApp.ACSaveChanges();
                            if (msg != null)
                            {
                                Messages.LogError(this.GetACUrl(), "DoDosingBookingProd(10b)", msg.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoDosingBookingProd", 1250), true);
                            }
                        }
                    }
                    // Sonst 
                    else
                    {
                        msg = dbApp.ACSaveChanges();
                        if (msg != null)
                        {
                            collectedMessages.AddDetailMessage(msg);
                            Messages.LogError(this.GetACUrl(), "DoDosingBookingProd(11)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoDosingBookingProd", 1260), true);
                        }
                    }
                }
                catch (Exception ec)
                {
                    string msgEc = ec.Message;
                    collectedMessages.AddDetailMessage(new Msg(eMsgLevel.Exception, msgEc));
                    Messages.LogException("PWDosing_Prod", "DoDosingBookingProd(98)", ec);
                    msgEc = ec.StackTrace;
                    if (!String.IsNullOrEmpty(msgEc))
                        Messages.LogException("PWDosing_Prod", "DoDosingBookingProd(99)", msgEc);
                }
                finally
                {
                    if (dbApp.IsChanged)
                    {
                        dbApp.ACSaveChanges();
                    }
                }
            }

            return collectedMessages.MsgDetailsCount > 0 ? collectedMessages : null;
        }


        public virtual void OnDoDosingBookingRelationGetPostingQuantity(MsgWithDetails collectedMessages, IACPointNetBase sender, ACEventArgs e, IACObject wrapObject,
                                        DatabaseApp dbApp, ProdOrderPartslistPosRelation dosingPosRelation, Facility outwardFacility,
                                        PADosingAbortReason dosingFuncResultState, ref double? postingQuantity)
        {
            if (BookTargetQIfZero == PostingMode.QuantityFromStore)
            {
                double stock = outwardFacility.CurrentFacilityStock.StockQuantity;
                if (Math.Abs(stock) > Double.Epsilon)
                    postingQuantity = stock;
            }
        }

        /// <summary>
        /// 4.2 Benachrichtigungsmethode, dass Siloabbuchung stattgefunden hat. Hier kann die Subclass noch zusätzliche stati setzten
        /// Falls Silo leer und das Silo leergebucht werden soll, dann muss true zurückgegeben werden andernfalls kann mit false die Mullbestandsbuchung in der Basisklasse ausgeschaltet werden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dbApp"></param>
        /// <param name="dosingPosRelation"></param>
        /// <returns>False if Silo ist empty and Booking to Zero is done in subclass</returns>
        public virtual bool OnDosingBookingDone(MsgWithDetails collectedMessages, IACPointNetBase sender, ACEventArgs e, IACObject wrapObject,
                                                DatabaseApp dbApp, ProdOrderPartslistPosRelation dosingPosRelation, Facility outwardFacility,
                                                PADosingAbortReason dosingFuncResultState)
        {
            if (((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode) 
                || ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
            {
                if (dosingPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist != null)
                {
                    dosingPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.MDProdOrderState = DatabaseApp.s_cQry_GetMDProdOrderState(dbApp, MDProdOrderState.ProdOrderStates.ProdFinished).FirstOrDefault();
                    ProdOrderPartslistPos batchPos = dosingPosRelation.TargetProdOrderPartslistPos;
                    if (batchPos != null)
                    {
                        batchPos.MDProdOrderPartslistPosState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                        if (batchPos.TopParentPartslistPos != null && ((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode))
                            batchPos.TopParentPartslistPos.MDProdOrderPartslistPosState = batchPos.MDProdOrderPartslistPosState;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 4.3 Benachrichtigungsmethode für Subclass, dass aktuelles Silo auf Nullbestand gebucht worden ist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="wrapObject"></param>
        /// <param name="dbApp"></param>
        /// <param name="dosingPosRelation"></param>
        public virtual void OnBookingEmptySource(MsgWithDetails collectedMessages, IACPointNetBase sender, ACEventArgs e, IACObject wrapObject,
            DatabaseApp dbApp, ProdOrderPartslistPosRelation dosingPosRelation, PADosingAbortReason dosingFuncResultState)
        {
            using (Database dbIPlus = new Database())
            {
                // Skaliere restliche Komponenten falls kein Rezeptabbruch und kein weiteres Silo mehr verfügbar
                if (   !((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMEmptyingMode)
                    && !((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMLastBatchEndOrderEmptyingMode)
                    && !((ACSubStateEnum)RootPW.CurrentACSubState).HasFlag(ACSubStateEnum.SMBatchCancelled))
                {
                    bool needKompScaling = false;
                    if (dosingFuncResultState == PADosingAbortReason.EmptySourceNextSource
                         || dosingFuncResultState == PADosingAbortReason.MachineMalfunction)
                    {
                        // Suche verfügbare Silos
                        // Keine Skalierung, sondern warte auf Benutzereingabe
                        //IList<Facility> possibleSilos;
                        //IReadOnlyList<Route> routes = GetRoutes(dosingPosRelation, db, dbIPlus, ACProdOrderManager.SearchMode.AllSilos, null, out possibleSilos, null);
                        //if (routes == null)
                        //needKompScaling = true;
                    }
                    else if (dosingFuncResultState == PADosingAbortReason.EmptySourceEndBatchplan
                        || dosingFuncResultState == PADosingAbortReason.EmptySourceAbortAdjustOtherAndWait)
                        needKompScaling = true;
                    if (needKompScaling && !ScaleOtherComp && dosingPosRelation.ProdOrderBatch != null)
                    {
                        var queryOpenDosings = dosingPosRelation.ProdOrderBatch.ProdOrderPartslistPosRelation_ProdOrderBatch
                            .Where(c => //c.RemainingDosingWeight < -0.1 // TODO: Unterdosierung ist Min-Dosiermenge auf Waage
                                c.MDProdOrderPartslistPosState != null
                            && (c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created
                                || c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted));

                        //ProdOrderPartslistPos batchPos = dosingPosRelation.TargetProdOrderPartslistPos;
                        //var queryOpenDosings = batchPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray()
                        //   .Where(c => c.RemainingDosingQuantityUOM < -1.0 // TODO: Unterdosierung ist Min-Dosiermenge auf Waage
                        //   && c.MDProdOrderPartslistPosState != null
                        //   && (c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created
                        //       || c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted))
                        //   .OrderBy(c => c.Sequence);
                        // Chargengröße anpassen für die anderen Komponenten
                        if (dosingPosRelation.ActualQuantityUOM > 0.1 && queryOpenDosings.Any())
                        {
                            double scaleFactor = dosingPosRelation.ActualQuantityUOM / dosingPosRelation.TargetQuantityUOM;
                            foreach (var plPos in queryOpenDosings)
                            {
                                if (plPos != dosingPosRelation)
                                {
                                    plPos.TargetQuantityUOM = plPos.TargetQuantityUOM * scaleFactor;
                                }
                            }
                        }
                    }
                }
            }
        }


        public virtual MDProdOrderPartslistPosState OnReEvaluatePosState(MsgWithDetails collectedMessages, bool reEvaluatePosState, MDProdOrderPartslistPosState suggestedPosState,
                                                IACPointNetBase sender, ACEventArgs e, IACObject wrapObject,
                                                DatabaseApp dbApp, ProdOrderPartslistPosRelation dosingPosRelation, Facility outwardFacility,
                                                PADosingAbortReason dosingFuncResultState, PAFDosing dosing,
                                                string dis2SpecialDest, double? actualQuantity, double? tolerancePlus, double? toleranceMinus, double? targetQuantity,
                                                bool isEndlessDosing, bool thisDosingIsInTol)
        {
            MDProdOrderPartslistPosState posState = suggestedPosState;
            // Falls in Toleranz oder Dosierung abgebrochen ohne Grund, dann beende Position
            if (thisDosingIsInTol
                || dosingFuncResultState == PADosingAbortReason.CompCancelled
                || dosingFuncResultState == PADosingAbortReason.EmptySourceEndBatchplan
                || dosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenNextComp
                || dosingFuncResultState == PADosingAbortReason.EndTolErrorDosingThenDisThenNextComp
                || dosingFuncResultState == PADosingAbortReason.EndDosingThenDisThenEnd
                || dosingFuncResultState == PADosingAbortReason.EndTolErrorDosingThenDisThenEnd
                || dosingFuncResultState == PADosingAbortReason.EmptySourceAbortAdjustOtherAndWait
                )
            {
                if (posState == null || posState.ProdOrderPartslistPosState != MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed)
                    posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                // Falls Dosierung abgebrochen und keine Zwischenentleerung, dann keine Gesamttoleranzprüfung (G1.1) durchführen
                if (reEvaluatePosState && ParentPWGroup != null && !((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging))
                    reEvaluatePosState = false;
            }
            else
            {
                if (posState == null || posState.ProdOrderPartslistPosState != MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted)
                    posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted).FirstOrDefault();
            }

            // Gesamttoleranzprüfung (G1.1): Falls die aktuelle Dosierung nicht innerhalb der geforderten Dosiertoleranz war oder Zwischenentleerung durcjgeführt wird, dann führe eine Gesamttoleranzprüfung durch:
            if (reEvaluatePosState && !isEndlessDosing) //&& !thisDosingIsInTol)
            {
                if (dosingPosRelation.ActualQuantityUOM >= (dosingPosRelation.TargetQuantityUOM - toleranceMinus))
                {
                    if (posState == null || posState.ProdOrderPartslistPosState != MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed)
                        posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                }
                else
                {
                    if (posState == null || posState.ProdOrderPartslistPosState != MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted)
                        posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted).FirstOrDefault();
                }
            }
            return posState;
        }

        public virtual void OnBookingToOrderOnEmptying(MsgWithDetails collectedMessages, bool reEvaluatePosState,
                                                        IACPointNetBase sender, ACEventArgs e, IACObject wrapObject,
                                                        DatabaseApp dbApp, ProdOrderPartslistPosRelation dosingPosRelation, Facility outwardFacility,
                                                        PADosingAbortReason dosingFuncResultState, PAFDosing dosing,
                                                        string dis2SpecialDest, double? actualQuantity, double? tolerancePlus, double? toleranceMinus, double? targetQuantity,
                                                        bool isEndlessDosing, bool thisDosingIsInTol, Msg msg, MDProdOrderPartslistPosState posState, ref bool changePosState)
        {
            if (outwardFacility != null && outwardFacility.OrderPostingOnEmptying)
            {
                actualQuantity = outwardFacility.CurrentFacilityStock.StockQuantity;

                ProcessBooking(collectedMessages, reEvaluatePosState, sender, e, wrapObject, dbApp, dosingPosRelation, outwardFacility,
                                dosingFuncResultState, dosing, dis2SpecialDest, actualQuantity, tolerancePlus, toleranceMinus, targetQuantity, isEndlessDosing, thisDosingIsInTol, msg,
                                ref posState, ref changePosState, true);
            }
        }


        protected void ProcessBooking(MsgWithDetails collectedMessages, bool reEvaluatePosState,
                                    IACPointNetBase sender, ACEventArgs e, IACObject wrapObject,
                                    DatabaseApp dbApp, ProdOrderPartslistPosRelation dosingPosRelation, Facility outwardFacility,
                                    PADosingAbortReason dosingFuncResultState, PAFDosing dosing,
                                    string dis2SpecialDest, double? actualQuantity, double? tolerancePlus, double? toleranceMinus, double? targetQuantity,
                                    bool isEndlessDosing, bool thisDosingIsInTol, Msg msg, ref MDProdOrderPartslistPosState posState, ref bool changePosState,
                                    bool onEmptyingFacility = false)
        {
            if (actualQuantity.HasValue && (actualQuantity > 0.00001 || actualQuantity < -0.00001))
            {
                // 1. Bereite Buchung vor
                FacilityPreBooking facilityPreBooking = ProdOrderManager.NewOutwardFacilityPreBooking(this.ACFacilityManager, dbApp, dosingPosRelation, onEmptyingFacility);
                ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                bookingParam.OutwardQuantity = (double)actualQuantity;
                bookingParam.OutwardFacility = outwardFacility;
                bookingParam.SetCompleted = dosingFuncResultState == PADosingAbortReason.EmptySourceNextSource
                                         || dosingFuncResultState == PADosingAbortReason.EmptySourceEndBatchplan
                                         || dosingFuncResultState == PADosingAbortReason.EmptySourceAbortAdjustOtherAndWait;

                if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                    bookingParam.PropertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();
                OnProcessBookingPre(facilityPreBooking, collectedMessages, reEvaluatePosState, sender, e, wrapObject, dbApp, dosingPosRelation, outwardFacility,
                                       dosingFuncResultState, dosing, dis2SpecialDest, actualQuantity, tolerancePlus, toleranceMinus, targetQuantity,
                                       isEndlessDosing, thisDosingIsInTol, msg, ref posState, ref changePosState, onEmptyingFacility);

                InsertNewWeighingIfAlibi(dbApp, actualQuantity.Value, e);

                msg = dbApp.ACSaveChangesWithRetry();

                // 2. Führe Buchung durch
                if (msg != null)
                {
                    collectedMessages.AddDetailMessage(msg);
                    Messages.LogError(this.GetACUrl(), "DoDosingBookingProd(5)", msg.InnerMessage);
                    OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoDosingBookingProd", 1190), true);
                    changePosState = false;
                }
                else if (facilityPreBooking != null)
                {
                    bookingParam.IgnoreIsEnabled = true;
                    ACMethodEventArgs resultBooking = ACFacilityManager.BookFacilityWithRetry(ref bookingParam, dbApp);
                    if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                    {
                        collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoDosingBookingProd", 1210), true);
                        changePosState = false;
                    }
                    else
                    {
                        if (!bookingParam.ValidMessage.IsSucceded() || bookingParam.ValidMessage.HasWarnings())
                        {
                            Messages.LogError(this.GetACUrl(), "DoDosingBookingProd(6)", bookingParam.ValidMessage.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "DoDosingBookingProd", 1200), true);
                            changePosState = false;
                        }
                        changePosState = true;
                        if (bookingParam.ValidMessage.IsSucceded())
                        {
                            facilityPreBooking.DeleteACObject(dbApp, true);
                            dosingPosRelation.IncreaseActualQuantityUOM(bookingParam.OutwardQuantity.Value);
                            msg = dbApp.ACSaveChangesWithRetry();
                            if (msg != null)
                            {
                                collectedMessages.AddDetailMessage(msg);
                                Messages.LogError(this.GetACUrl(), "DoDosingBookingProd(7b)", msg.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoDosingBookingProd", 1211), true);
                            }
                            //dosingPosRelation.TopParentPartslistPosRelation.RecalcActualQuantity();
                            //dosingPosRelation.SourceProdOrderPartslistPos.TopParentPartslistPos.RecalcActualQuantity();
                        }
                        else
                        {
                            collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                        }

                        posState = OnReEvaluatePosState(collectedMessages, reEvaluatePosState, posState,
                            sender, e, wrapObject, dbApp, dosingPosRelation, outwardFacility, dosingFuncResultState, dosing, dis2SpecialDest,
                            actualQuantity, tolerancePlus, toleranceMinus, targetQuantity, isEndlessDosing, thisDosingIsInTol);

                        if (ScaleOtherComp && !thisDosingIsInTol && dosingPosRelation.ProdOrderBatch != null)
                        {
                            posState = DatabaseApp.s_cQry_GetMDProdOrderPosState(dbApp, MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed).FirstOrDefault();
                            //dosingPosRelation.MDProdOrderPartslistPosState = posState;

                            var queryOpenDosings = dosingPosRelation.ProdOrderBatch.ProdOrderPartslistPosRelation_ProdOrderBatch
                                .Where(c => //c.RemainingDosingWeight < -0.1 // TODO: Unterdosierung ist Min-Dosiermenge auf Waage
                                   c.MDProdOrderPartslistPosState != null
                                && (c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created
                                    || c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted));

                            //ProdOrderPartslistPos intermediatePosition = currentProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                            //    .Where(c => c.MaterialID.HasValue && c.MaterialID == matWFConnection.MaterialID
                            //        && c.MaterialPosType == GlobalApp.MaterialPosTypes.InwardIntern
                            //        && !c.ParentProdOrderPartslistPosID.HasValue).FirstOrDefault();

                            //var queryOpenDosings = batchPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray()
                            //   .Where(c => c.RemainingDosingQuantityUOM < -1.0 // TODO: Unterdosierung ist Min-Dosiermenge auf Waage
                            //   && c.MDProdOrderPartslistPosState != null
                            //   && (c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created
                            //       || c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.PartialCompleted))
                            //   .OrderBy(c => c.Sequence);
                            // Chargengröße anpassen für die anderen Komponenten
                            if (dosingPosRelation.ActualQuantityUOM > 0.1 && queryOpenDosings.Any())
                            {
                                double scaleFactor = dosingPosRelation.ActualQuantityUOM / dosingPosRelation.TargetQuantityUOM;
                                foreach (var plPos in queryOpenDosings)
                                {
                                    if (plPos != dosingPosRelation)
                                    {
                                        plPos.TargetQuantityUOM = plPos.TargetQuantityUOM * scaleFactor;
                                    }
                                }
                            }
                        }

                        msg = dbApp.ACSaveChangesWithRetry();
                        if (msg != null)
                        {
                            collectedMessages.AddDetailMessage(msg);
                            Messages.LogError(this.GetACUrl(), "DoDosingBookingProd(8)", msg.InnerMessage);
                            OnNewAlarmOccurred(ProcessAlarm, new Msg(msg.Message, this, eMsgLevel.Error, PWClassName, "DoDosingBookingProd", 1220), true);
                        }
                        else
                        {
                            dosingPosRelation.RecalcActualQuantityFast();
                            if (dbApp.IsChanged)
                                dbApp.ACSaveChanges();
                            // Bei Restentleerung wird in ein Sonderziel gefahren
                            // => Es muss die selbe Menge wieder zurückgebucht werden auf ein Sonderlagerplatz
                            if (!String.IsNullOrEmpty(dis2SpecialDest))
                            {
                                Facility specialDest = dbApp.Facility.Where(c => c.FacilityNo == dis2SpecialDest).FirstOrDefault();
                                if (specialDest == null && outwardFacility.Facility1_ParentFacility != null)
                                {
                                    specialDest = dbApp.Facility.Where(c => c.ParentFacilityID.HasValue
                                        && c.ParentFacilityID == outwardFacility.ParentFacilityID
                                        && c.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBin).FirstOrDefault();
                                }
                                if (specialDest != null && specialDest.MDFacilityType != null && specialDest.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBin)
                                {
                                    var queryDoneOutwardBookings = dbApp.FacilityBookingCharge.Where(c => c.ProdOrderPartslistPosRelationID.HasValue && c.ProdOrderPartslistPosRelationID == dosingPosRelation.ProdOrderPartslistPosRelationID).ToArray();
                                    foreach (FacilityBookingCharge fbc in queryDoneOutwardBookings)
                                    {
                                        ReveseBookingToExtraFacility(collectedMessages, dbApp, fbc, specialDest, dosingPosRelation);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected virtual void OnProcessBookingPre(FacilityPreBooking facilityPreBooking, MsgWithDetails collectedMessages, bool reEvaluatePosState,
                                    IACPointNetBase sender, ACEventArgs e, IACObject wrapObject,
                                    DatabaseApp dbApp, ProdOrderPartslistPosRelation dosingPosRelation, Facility outwardFacility,
                                    PADosingAbortReason dosingFuncResultState, PAFDosing dosing,
                                    string dis2SpecialDest, double? actualQuantity, double? tolerancePlus, double? toleranceMinus, double? targetQuantity,
                                    bool isEndlessDosing, bool thisDosingIsInTol, Msg msg, ref MDProdOrderPartslistPosState posState, ref bool changePosState,
                                    bool onEmptyingFacility)
        {
        }


        /// <summary>
        /// Hilfsmethode um eine Entnahmebuchung von dem ursprünglichen Silo in ein Sonderziel zurckzubuchen
        /// </summary>
        /// <param name="dbApp"></param>
        /// <param name="fbc"></param>
        /// <param name="specialDest"></param>
        /// <param name="dosingPosRelation"></param>
        protected void ReveseBookingToExtraFacility(MsgWithDetails collectedMessages, DatabaseApp dbApp, FacilityBookingCharge fbc, Facility specialDest, ProdOrderPartslistPosRelation dosingPosRelation)
        {
            if (fbc.OutwardFacilityLotID.HasValue)
            {
                int splitNo = 1;
                var queryExistingQuants = dbApp.FacilityCharge.Where(c => c.MaterialID == fbc.OutwardMaterialID
                                        && c.FacilityID == specialDest.FacilityID
                                        && c.FacilityLotID.HasValue && c.FacilityLotID == fbc.OutwardFacilityLotID);
                if (queryExistingQuants.Any())
                    splitNo = queryExistingQuants.Max(c => c.SplitNo) + 1;
                FacilityCharge newQuant = FacilityCharge.NewACObject(dbApp, null);
                newQuant.Material = fbc.OutwardMaterial;
                newQuant.MDUnit = fbc.OutwardMaterial.BaseMDUnit;
                newQuant.Facility = specialDest;
                newQuant.FacilityLot = fbc.OutwardFacilityLot;
                newQuant.SplitNo = splitNo;
                newQuant.FacilityChargeSortNo = specialDest.GetNextFCSortNo(dbApp);
                newQuant.NotAvailable = false;
                newQuant.FillingDate = DateTime.Now;
                dbApp.FacilityCharge.Add(newQuant);

                // 1. Bereite Buchung vor
                FacilityPreBooking facilityPreBooking = ProdOrderManager.NewOutwardFacilityPreBooking(this.ACFacilityManager, dbApp, dosingPosRelation);
                ACMethodBooking bookingParam = facilityPreBooking.ACMethodBooking as ACMethodBooking;
                bookingParam.OutwardFacilityCharge = newQuant;
                bookingParam.OutwardQuantity = fbc.OutwardQuantity * -1;
                bookingParam.OutwardFacility = specialDest;
                if (ParentPWGroup != null && ParentPWGroup.AccessedProcessModule != null)
                    bookingParam.PropertyACUrl = ParentPWGroup.AccessedProcessModule.GetACUrl();

                Msg resultSave = dbApp.ACSaveChangesWithRetry();

                // 2. Führe Buchung durch
                if (resultSave != null)
                {
                    collectedMessages.AddDetailMessage(resultSave);
                    Messages.LogError(this.GetACUrl(), "ReveseBookingToExtraFacility.TaskCallback(5)", resultSave.InnerMessage);
                    dbApp.ACUndoChanges();
                }
                else if (facilityPreBooking != null)
                {
                    bookingParam.IgnoreIsEnabled = true;
                    ACMethodEventArgs resultBooking = ACFacilityManager.BookFacilityWithRetry(ref bookingParam, dbApp);
                    if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                    {
                        collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                        OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "ReverseBookingToExtraFacility", 1280), true);
                    }
                    else
                    {
                        if (!bookingParam.ValidMessage.IsSucceded() || bookingParam.ValidMessage.HasWarnings())
                        {
                            OnNewAlarmOccurred(ProcessAlarm, new Msg(bookingParam.ValidMessage.InnerMessage, this, eMsgLevel.Error, PWClassName, "ReverseBookingToExtraFacility", 1270), true);
                            Messages.LogError(this.GetACUrl(), "ReveseBookingToExtraFacility.TaskCallback(6)", bookingParam.ValidMessage.InnerMessage);
                        }
                        if (bookingParam.ValidMessage.IsSucceded())
                        {
                            facilityPreBooking.DeleteACObject(dbApp, true);
                            dosingPosRelation.IncreaseActualQuantityUOM(bookingParam.OutwardQuantity.Value);
                            //dosingPosRelation.TopParentPartslistPosRelation.RecalcActualQuantity();
                            //dosingPosRelation.SourceProdOrderPartslistPos.TopParentPartslistPos.RecalcActualQuantity();
                        }
                        else
                        {
                            collectedMessages.AddDetailMessage(resultBooking.ValidMessage);
                        }

                        resultSave = dbApp.ACSaveChangesWithRetry();
                        if (resultSave != null)
                        {
                            collectedMessages.AddDetailMessage(resultSave);
                            Messages.LogError(this.GetACUrl(), "ReveseBookingToExtraFacility.TaskCallback(8)", resultSave.InnerMessage);
                        }
                        else
                        {
                            dosingPosRelation.RecalcActualQuantityFast();
                            if (dbApp.IsChanged)
                                dbApp.ACSaveChanges();
                        }
                    }
                }
            }
        }


        protected virtual void HandleNoSourceFoundForDosing(ProdOrderPartslistPosRelation relation,
                                        DatabaseApp dbApp, Database dbIPlus,
                                        RouteQueryParams queryParams,
                                        ACPartslistManager.QrySilosResult possibleSilos)
        {
        }

        protected bool HasStartableParallelDosingNodes(IEnumerable<IPWNodeReceiveMaterial> allParallelDosingWFs, DosingSkipMode skipComponentsMode, ACPartslistManager.QrySilosResult possibleSilos, ProdOrderPartslistPosRelation relation)
        {
            if (allParallelDosingWFs != null && allParallelDosingWFs.Any()
                                             && (skipComponentsMode == DosingSkipMode.DifferentWFClasses || (possibleSilos != null && possibleSilos.FilteredResult != null && possibleSilos.FilteredResult.Any())))
            {
                List<IPWNodeReceiveMaterial> otherDosingWFs = allParallelDosingWFs.Where(c => (c as IPWNodeReceiveMaterial).IterationCount.ValueT <= 0
                                                                                        || ((c as IPWNodeReceiveMaterial).ParentPWGroup != null
                                                                                            && (c as IPWNodeReceiveMaterial).ParentPWGroup.CurrentACSubState == (uint)ACSubStateEnum.SMInterDischarging))
                                                                               .ToList();
                // Remove potential WFNodes which are out of the SequenceRange
                if (otherDosingWFs.Any())
                {
                    foreach (var otherDosingWF in otherDosingWFs.ToArray())
                    {
                        if (otherDosingWF.ComponentsSeqFrom > 0 && otherDosingWF.ComponentsSeqTo > 0
                            && (relation.Sequence < otherDosingWF.ComponentsSeqFrom || relation.Sequence > otherDosingWF.ComponentsSeqTo))
                        {
                            otherDosingWFs.Remove(otherDosingWF);
                        }
                    }
                }

                if (otherDosingWFs.Any())
                    return true;
            }

            return false;
        }
    }
}
