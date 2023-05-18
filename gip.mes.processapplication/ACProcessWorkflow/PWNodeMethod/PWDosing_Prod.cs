using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System.Threading;


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
            out MaterialWFConnection matWFConnection, out ProdOrderBatch batch, out ProdOrderBatchPlan batchPlan)
        {
            intermediateChildPos = null;
            matWFConnection = null;
            batch = null;
            batchPlan = null;
            intermediatePosition = null;

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
                matWFConnection = dbApp.MaterialWFConnection
                                        .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan2.MaterialWFACClassMethodID.Value
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
                        bool posFound = GetRelatedProdOrderPosForWFNode(this, dbIPlus, dbApp, pwMethodProduction, out intermediateChildPos, out intermediatePos, 
                            out endBatchPos, out matWFConnection, out batch, out batchPlan);
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

                                IList<Facility> possibleSilos;

                                PAProcessModule module = ParentPWGroup.AccessedProcessModule != null ? ParentPWGroup.AccessedProcessModule : ParentPWGroup.FirstAvailableProcessModule;
                                if (module == null && ParentPWGroup.ProcessModuleList != null) // If all occupied, then use first that is generally possible 
                                    module = ParentPWGroup.ProcessModuleList.FirstOrDefault();
                                RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.StartDosing,
                                    OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                    null, null, ExcludedSilos);
                                IEnumerable<Route> routes = GetRoutes(relation, dbApp, dbIPlus, queryParams, module, out possibleSilos);

                                // #SKIPMALZERS
                                // TODO: This is a temporary solution (Malzers) to prevent skipping dosings and afterwards it will be detected that the other node cannot dose the material.
                                // See also code below #SKIPMALZERS
                                if (   (routes == null || !routes.Any())
                                    && skipComponentsMode == DosingSkipMode.DifferentWFClasses)
                                {
                                    RouteQueryParams queryParams2 = new RouteQueryParams(RouteQueryPurpose.StartDosing, ACPartslistManager.SearchMode.AllSilos, null, null, ExcludedSilos);
                                    IList<Facility> possibleSilos2;
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
                    bool posFound = GetRelatedProdOrderPosForWFNode(this, dbIPlus, dbApp, pwMethodProduction, 
                        out intermediateChildPos, out intermediatePosition, out endBatchPos, out matWFConnection, out batch, out batchPlan);
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

                    bool hasOpenDosings = false;
                    bool isAnyCompDosableFromAnyRoutableSilo = false;
                    bool componentsSkippable = ComponentsSkippable;
                    DosingSkipMode skipComponentsMode = SkipComponentsMode;
                    StartNextCompResult openDosingsResult = StartNextCompResult.Done;
                    // Falls noch Dosierungen anstehen, dann dosiere nächste Komponente
                    if (queryOpenDosings != null && queryOpenDosings.Any())
                    {
                        queryOpenDosings = OnSortOpenDosings(queryOpenDosings, dbIPlus, dbApp);
                        foreach (ProdOrderPartslistPosRelation relation in queryOpenDosings)
                        {
                            if (!relation.SourceProdOrderPartslistPos.Material.UsageACProgram)
                                continue;
                            double dosingWeight = relation.RemainingDosingWeight;
                            bool interDischargingNeeded = false;
                            IPAMContScale scale = ParentPWGroup != null ? ParentPWGroup.AccessedProcessModule as IPAMContScale : null;
                            ScaleBoundaries scaleBoundaries = null;
                            if (scale != null)
                                scaleBoundaries = OnGetScaleBoundariesForDosing(scale, dbApp, queryOpenDosings, intermediateChildPos, intermediatePosition, matWFConnection, batch, batchPlan, endBatchPos);
                            if (scaleBoundaries != null)
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
                                        ParentPWGroup.CurrentACSubState = (uint) ACSubStateEnum.SMInterDischarging;
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
                                            ParentPWGroup.CurrentACSubState = (uint) ACSubStateEnum.SMInterDischarging;
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
                                return StartNextCompResult.CycleWait;
                            }

                            MDProdOrderPartslistPosState posState;
                            IList<Facility> possibleSilos;

                            RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.StartDosing,
                                OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                null, null, ExcludedSilos);
                            IEnumerable<Route> routes = GetRoutes(relation, dbApp, dbIPlus, queryParams, null, out possibleSilos);

                            if (routes != null && routes.Any())
                            {
                                List<Route> routesList = routes.ToList();
                                module.GetACStateOfFunction(acMethod.ACIdentifier, out responsibleFunc);
                                if (responsibleFunc == null)
                                {
                                    //Error50327: Responsible dosingfunction for ACMethod {0} not found. Please check your logical brige from the InPoints of the processmodule to the InPoint of the dosingfunction.
                                    msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(9b)", 1121, "Error50327", acMethod.ACIdentifier);
                                    OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                    return StartNextCompResult.CycleWait;
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

                            if (routes == null || !routes.Any())
                            {
                                bool hasOtherStartableDosingNodes = false;
                                // Falls Komponente überspringbar und es weitere Dosierschritte gibt, die diese Komponente dosieren könnten, und vom gleichen Dosierschritttyp sind,
                                // dann gehe zur nächsten Komponente 
                                Guid[] otherDosingNodes = null;
                                Guid thisACClassID = ComponentClass.ACClassID;
                                core.datamodel.ACClassWF thisContentACClassWF = ContentACClassWF;
                                if (batchPlan != null && batchPlan.MaterialWFACClassMethodID.HasValue)
                                {
                                    otherDosingNodes = intermediatePosition.Material.MaterialWFConnection_Material
                                    .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == batchPlan.MaterialWFACClassMethodID.Value
                                                && c.ACClassWFID != thisContentACClassWF.ACClassWFID
                                                && c.ACClassWF.ACClassMethodID == thisContentACClassWF.ACClassMethodID
                                                && (skipComponentsMode == DosingSkipMode.DifferentWFClasses || c.ACClassWF.PWACClassID == thisACClassID))
                                    .Select(c => c.ACClassWFID)
                                    .ToArray();
                                }
                                else
                                {
                                    PartslistACClassMethod plMethod = intermediatePosition.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                                    if (plMethod != null)
                                    {
                                        otherDosingNodes = intermediatePosition.Material.MaterialWFConnection_Material
                                                                .Where(c => c.MaterialWFACClassMethod.MaterialWFACClassMethodID == plMethod.MaterialWFACClassMethodID
                                                                && c.ACClassWFID != thisContentACClassWF.ACClassWFID
                                                                && c.ACClassWF.ACClassMethodID == thisContentACClassWF.ACClassMethodID
                                                                && (skipComponentsMode == DosingSkipMode.DifferentWFClasses || c.ACClassWF.PWACClassID == thisACClassID))
                                                .Select(c => c.ACClassWFID)
                                                .ToArray();
                                    }
                                    else
                                    {
                                        otherDosingNodes = intermediatePosition.Material.MaterialWFConnection_Material
                                            .Where(c => c.MaterialWFACClassMethod.PartslistACClassMethod_MaterialWFACClassMethod
                                                            .Where(d => d.PartslistID == endBatchPos.ProdOrderPartslist.PartslistID).Any()
                                                        && c.MaterialWFACClassMethod.MaterialWFID == endBatchPos.ProdOrderPartslist.Partslist.MaterialWFID
                                                        && c.ACClassWFID != thisContentACClassWF.ACClassWFID
                                                        && c.ACClassWF.ACClassMethodID == thisContentACClassWF.ACClassMethodID
                                                        && (skipComponentsMode == DosingSkipMode.DifferentWFClasses || c.ACClassWF.PWACClassID == thisACClassID))
                                            .Select(c => c.ACClassWFID)
                                            .ToArray();
                                    }
                                }

                                if (   otherDosingNodes != null 
                                    && otherDosingNodes.Any() 
                                    && (skipComponentsMode == DosingSkipMode.DifferentWFClasses || (possibleSilos != null && possibleSilos.Any())))
                                {
                                    List<IPWNodeReceiveMaterial> otherDosingWFs = this.RootPW.FindChildComponents<IPWNodeReceiveMaterial>(c => c is IPWNodeReceiveMaterial
                                                                                                && (c as IPWNodeReceiveMaterial).ContentACClassWF != null
                                                                                                && otherDosingNodes.Contains((c as IPWNodeReceiveMaterial).ContentACClassWF.ACClassWFID)
                                                                                                && (   (c as IPWNodeReceiveMaterial).IterationCount.ValueT <= 0 
                                                                                                    || (   (c as IPWNodeReceiveMaterial).ParentPWGroup != null 
                                                                                                        && (c as IPWNodeReceiveMaterial).ParentPWGroup.CurrentACSubState == (uint)ACSubStateEnum.SMInterDischarging))
                                                                                                /*&& (c.CurrentACState == PABaseState.SMIdle || c.CurrentACState == PABaseState.SMBreakPoint)*/);
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
                                    if (hasOtherStartableDosingNodes && skipComponentsMode == DosingSkipMode.DifferentWFClasses)
                                    {
                                        RouteQueryParams queryParams2 = new RouteQueryParams(RouteQueryPurpose.StartDosing, ACPartslistManager.SearchMode.AllSilos, null, null, ExcludedSilos);
                                        IList<Facility> possibleSilos2;
                                        IEnumerable<Route> routes2 = GetRoutes(relation, dbApp, dbIPlus, queryParams2, null, out possibleSilos2);
                                        if (routes2 != null && routes2.Any())
                                            hasOtherStartableDosingNodes = false;
                                    }
                                }

                                // Versuche nächste Komponente, wenn es noch andere Dosierschritte gibt
                                if (hasOtherStartableDosingNodes)
                                {
                                    queryParams = new RouteQueryParams(RouteQueryPurpose.StartDosing, ACPartslistManager.SearchMode.AllSilos, null, null, ExcludedSilos);
                                    routes = GetRoutes(relation, dbApp, dbIPlus, queryParams, null, out possibleSilos);
                                    if (routes != null && routes.Any())
                                        isAnyCompDosableFromAnyRoutableSilo = true;
                                    hasOpenDosings = true;
                                    continue;
                                }

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
                                        return StartNextCompResult.CycleWait;
                                    }
                                    relation.MDProdOrderPartslistPosState = posState;
                                    dbApp.ACSaveChanges();
                                    hasOpenDosings = true;
                                    continue; // Gehe zur nächsten Komponente
                                }

                                HandleNoSourceFoundForDosing(relation, dbApp, dbIPlus, queryParams, possibleSilos);
                                return StartNextCompResult.CycleWait;
                            }
                            else if (NoSourceFoundForDosing.ValueT == 1)
                            {
                                NoSourceFoundForDosing.ValueT = 0;
                                AcknowledgeAlarms();
                            }

                            // 3. Finde die Route mit der höchsten Siloprioriät
                            Route dosingRoute = null;
                            foreach (var prioSilo in possibleSilos)
                            {
                                if (!prioSilo.VBiFacilityACClassID.HasValue)
                                    continue;
                                dosingRoute = routes.Where(c => c.FirstOrDefault().Source.ACClassID == prioSilo.VBiFacilityACClassID).FirstOrDefault();
                                if (dosingRoute != null)
                                    break;
                            }
                            if (dosingRoute == null || double.IsNaN(relation.RemainingDosingWeight))
                            {
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
                                        return StartNextCompResult.CycleWait;
                                    }
                                    relation.MDProdOrderPartslistPosState = posState;
                                    dbApp.ACSaveChanges();
                                    hasOpenDosings = true;
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
                                return StartNextCompResult.CycleWait;
                            }

                            responsibleFunc = CanStartProcessFunc(module, acMethod, dbApp, relation, endBatchPos, intermediatePosition, batch, sourceSilo);
                            if (responsibleFunc == null)
                                return StartNextCompResult.CycleWait;

                            if (relation != null && double.IsNaN(relation.RemainingDosingWeight))
                            {
                                NoSourceFoundForDosing.ValueT = 1;
                                //Error50597: Dosing error on the component {0} {1}, {2};
                                string error = relation.RemainingDosingWeightError;
                                msg = new Msg(this, eMsgLevel.Error, PWClassName, "StartNextProdComponent(9a)", 1111, "Error50597", relation.SourceProdOrderPartslistPos.MaterialNo, relation.SourceProdOrderPartslistPos.MaterialName, error);
                                OnNewAlarmOccurred(ProcessAlarm, msg, true);
                                return StartNextCompResult.CycleWait;
                            }

                            if (!(bool)ExecuteMethod(nameof(GetConfigForACMethod), acMethod, true, dbApp, relation, endBatchPos, intermediatePosition, batch, sourceSilo))
                                return StartNextCompResult.CycleWait;

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
                            acMethod["Route"] = dosingRoute != null ? dosingRoute.Clone() as Route : null;
                            acMethod["Source"] = sourceSilo.RouteItemIDAsNum;
                            acMethod["TargetQuantity"] = Math.Abs(dosingWeight);
                            acMethod[Material.ClassName] = relation.SourceProdOrderPartslistPos.Material.MaterialName1;
                            if (relation.SourceProdOrderPartslistPos.Material.Density > 0.00001)
                                acMethod["Density"] = relation.SourceProdOrderPartslistPos.Material.Density;
                            if (dosingRoute != null)
                                dosingRoute.Detach(true);

                            if (!(bool)ExecuteMethod(nameof(AfterConfigForACMethodIsSet), acMethod, true, dbApp, relation, endBatchPos, intermediatePosition, batch, sourceSilo))
                                return StartNextCompResult.CycleWait;

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
                                return StartNextCompResult.CycleWait;
                            }

                            RecalcTimeInfo(true);
                            CurrentDosingPos.ValueT = relation.ProdOrderPartslistPosRelationID;
                            if (CreateNewProgramLog(acMethod, _NewAddedProgramLog == null) <= CreateNewProgramLogResult.ErrorNoProgramFound)
                                return StartNextCompResult.CycleWait;
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
                                return StartNextCompResult.CycleWait;
                            }
                            UpdateCurrentACMethod();

                            if (interDischargingNeeded)
                                ParentPWGroup.CurrentACSubState = (uint) ACSubStateEnum.SMInterDischarging;

                            CachedEmptySiloHandlingOption = null;
                            relation.MDProdOrderPartslistPosState = posState;
                            MsgWithDetails msg2 = dbApp.ACSaveChanges();
                            if (msg2 != null)
                            {
                                Messages.LogException(this.GetACUrl(), "StartNextProdComponent(5)", msg2.InnerMessage);
                                OnNewAlarmOccurred(ProcessAlarm, new Msg(msg2.InnerMessage, this, eMsgLevel.Exception, PWClassName, "StartNextProdComponent", 1150), true);
                                return StartNextCompResult.CycleWait;
                            }
                            AcknowledgeAlarms();
                            ExecuteMethod(nameof(OnACMethodSended), acMethod, true, dbApp, relation, endBatchPos, intermediatePosition, batch, sourceSilo, responsibleFunc);
                            return StartNextCompResult.NextCompStarted;
                            #endregion
                        }
                    }


                    if ((hasOpenDosings && componentsSkippable) || !hasOpenDosings)
                        openDosingsResult = StartNextCompResult.Done;
                    else
                        openDosingsResult = StartNextCompResult.CycleWait;


                    //if (openDosingsResult == StartNextCompResult.CycleWait
                    //    || queryOpenDosings == null || !queryOpenDosings.Any())
                    {
                        // Überprüfe ob es noch parallele Schritte gibt, welche noch die letzte Komponete dosieren.
                        // Warte noch darauf dass diese erledigt werden, weil sonst kein Waagenwechsel mehr möglich wäre
                        currentParallelPWDosings = CurrentParallelPWDosings;
                        if (currentParallelPWDosings == null
                            || currentParallelPWDosings.Where(c => c.CurrentACState != ACStateEnum.SMIdle).Any())
                        {
                            // Reduziere zyklische Datenbankabfragen über Zeitstempel
                            if (NextCheckIfPWDosingsFinished.HasValue && DateTime.Now < NextCheckIfPWDosingsFinished)
                                return StartNextCompResult.CycleWait;

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
                                    ParentPWGroup.CurrentACSubState = (uint) ACSubStateEnum.SMIdle;
                                return StartNextCompResult.Done;
                            }


                            // Finde die anderen Knoten die zur Zeit dosieren
                            if (queryActiveDosings != null && queryActiveDosings.Any())
                            {
                                // Prüfe ob die parallelen aktiven Dosierungen noch auf dieser Waage dosiert werden könnten für einen potentiellen Waagenwechsel
                                if (!isAnyCompDosableFromAnyRoutableSilo)
                                {
                                    foreach (var activeDosing in queryActiveDosings)
                                    {
                                        IList<Facility> possibleSilos;
                                        RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.StartDosing, ACPartslistManager.SearchMode.AllSilos, null, null, ExcludedSilos);
                                        var routes = GetRoutes(activeDosing, dbApp, dbIPlus, queryParams, null, out possibleSilos);
                                        if (routes != null && routes.Any())
                                            isAnyCompDosableFromAnyRoutableSilo = true;
                                    }
                                }

                                var openRelations = queryActiveDosings.Select(c => c.ProdOrderPartslistPosRelationID);
                                CurrentParallelPWDosings = RootPW.FindChildComponents<PWDosing>(c => c is PWDosing
                                                                        && c != this
                                                                        && (c as PWDosing).CurrentDosingPos.ValueT != Guid.Empty
                                                                        && openRelations.Contains((c as PWDosing).CurrentDosingPos.ValueT))
                                                                        .ToList();
                            }

                            currentParallelPWDosings = CurrentParallelPWDosings;
                            if (   !isAnyCompDosableFromAnyRoutableSilo 
                                || ((currentParallelPWDosings == null || !currentParallelPWDosings.Any()) && openDosingsResult == StartNextCompResult.Done))
                            {
                                NextCheckIfPWDosingsFinished = null;
                                CurrentParallelPWDosings = null;
                                if (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging))
                                    ParentPWGroup.CurrentACSubState = (uint) ACSubStateEnum.SMIdle;
                                return StartNextCompResult.Done;
                            }

                            // Nächste Datenbankprüfung in 20 Sekunden
                            NextCheckIfPWDosingsFinished = DateTime.Now.AddSeconds(20);
                            return StartNextCompResult.CycleWait;
                        }

                        NextCheckIfPWDosingsFinished = null;
                        CurrentParallelPWDosings = null;
                        if (((ACSubStateEnum)ParentPWGroup.CurrentACSubState).HasFlag(ACSubStateEnum.SMInterDischarging))
                            ParentPWGroup.CurrentACSubState = (uint) ACSubStateEnum.SMIdle;
                        return openDosingsResult;
                        //return StartNextCompResult.Done;
                    }
                    //return openDosingsResult;
                }
            }
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

                    IList<Facility> possibleSilos;
                    RouteQueryParams queryParams = new RouteQueryParams(RouteQueryPurpose.HandleEmptySilo,
                        OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                        null, silo.Facility.ValueT.ValueT.FacilityID, ExcludedSilos);
                    IEnumerable<Route> routes = GetRoutes(dosingPosRelation, dbApp, dbIPlus, queryParams, null,out possibleSilos);
                    if (routes == null || !routes.Any())
                    {
                        if (AutoChangeScale && possibleSilos != null && possibleSilos.Any())
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
                                    IList<Facility> alternativeSilos;
                                    RouteQueryParams queryParams2 = new RouteQueryParams(RouteQueryPurpose.HandleEmptySilo,
                                        OldestSilo ? ACPartslistManager.SearchMode.OnlyEnabledOldestSilo : ACPartslistManager.SearchMode.SilosWithOutwardEnabled,
                                        null, silo.Facility.ValueT.ValueT.FacilityID, ExcludedSilos);
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

                if (dosingPosRelation.SourceProdOrderPartslistPos.MaterialID != outwardFacility.MaterialID.Value)
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
                                                    && activeDosingFunct.IsTransportActive
                                                    && activeDosingFunct.DosingAbortReason.ValueT == PADosingAbortReason.NotSet)
                                                {
                                                    anyOtherFunctionActiveFromThisSilo = true;
                                                    activeDosingFunct.SetAbortReasonEmptyForced();
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!anyOtherFunctionActiveFromThisSilo)
                                {
                                    bool hasQuants = outwardFacility.FacilityCharge_Facility.Where(c => c.NotAvailable == false).Any();

                                    bool zeroBookSucceeded = false;
                                    if (hasQuants)
                                    {
                                        if (outwardFacility.OrderPostingOnEmptying)
                                        {
                                            OnBookingToOrderOnEmptying(collectedMessages, reEvaluatePosState, sender, e, wrapObject, dbApp, dosingPosRelation, outwardFacility,
                                                                        dosingFuncResultState, dosing, dis2SpecialDest, actualQuantity, tolerancePlus, toleranceMinus, targetQuantity,
                                                                        isEndlessDosing, thisDosingIsInTol, msg, posState, ref changePosState);
                                        }

                                        zeroBookSucceeded = true;
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
                                            zeroBookSucceeded = false;
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
                                            else
                                                zeroBookSucceeded = true;
                                        }
                                    }

                                    if (!hasQuants || zeroBookSucceeded)
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
                                        if (!disChargingActive
                                            && (sourceSilo == null || !sourceSilo.LeaveMaterialOccupation))
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
            if (actualQuantity > 0.00001 || actualQuantity < -0.00001)
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
                dbApp.FacilityCharge.AddObject(newQuant);

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
                                        IList<Facility> possibleSilos)
        {
        }
    }
}
