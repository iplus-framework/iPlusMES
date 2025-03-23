// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.facility
{
    public partial class ACProdOrderManager
    {
        #region Methods

        #region Public

        #region Public -> Partslist validation
        public MsgWithDetails ValidateProdOrderPartslist(DatabaseApp dbApp, ACPartslistManager partsListManager, ProdOrderPartslist poList)
        {
            MsgWithDetails detailMessages = new MsgWithDetails();
            IEnumerable<ProdOrderPartslistPos> rootPosList = dbApp.ProdOrderPartslistPos
                .Include(c => c.Material.MDFacilityManagementType)
                .Include("ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos")
                .Where(c => c.ProdOrderPartslistID == poList.ProdOrderPartslistID && c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot)
                .AsEnumerable();
            foreach (ProdOrderPartslistPos pos in rootPosList)
            {
                //pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.AutoRefresh(pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPosReference, pos);
                //pos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.AutoRefresh(pos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosReference, pos);
                if (!pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any())
                {
                    // Stücklistenposition {0} {1} {2} ist keinem Zwischenmaterial zugeordnet.
                    detailMessages.AddDetailMessage(new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Warning,
                        ACIdentifier = "ValidateProdOrderPartslist(1)",
                        Message = Root.Environment.TranslateMessage(partsListManager, "Warning50013", pos.Sequence, pos.Material.MaterialNo, pos.MaterialName)
                    });
                }
                if (   pos.Material != null 
                    && pos.Material.IsLotReservationNeeded 
                    && !pos.FacilityReservation_ProdOrderPartslistPos.Where(c => !c.VBiACClassID.HasValue).Any())
                {
                    // Error50635: The material {0} {1} at position {2} requires reservation and no batch has been reserved.
                    detailMessages.AddDetailMessage(new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "ValidateProdOrderPartslist(20)",
                        Message = Root.Environment.TranslateMessage(this, "Error50634", pos.Material.MaterialNo, pos.Material.MaterialName1, pos.Sequence)
                    });
                }
            }
            return detailMessages;
        }

        #endregion

        #region Public ->Batch

        #region Public -> Batch -> Validation
        public MsgWithDetails ValidateBatchPlan(DatabaseApp dbApp, ProdOrderBatchPlan prodOrderBatchPlan)
        {
            MsgWithDetails detailMessages = new MsgWithDetails();
            bool hasBatchPlansWithoutDestination = !prodOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan.Any();
            if (hasBatchPlansWithoutDestination)
            {
                //Bei einem oder mehreren Batchplänen wurde kein Ziel eingetragen. Wollen Sie dennoch starten?
                detailMessages.AddDetailMessage(new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Warning,
                    ACIdentifier = "ValidateBatchPlan(10)",
                    Message = Root.Environment.TranslateMessage(this, "Warning50008")
                });
            }

            if (!prodOrderBatchPlan.IsValidated)
            {

            }
            return detailMessages;
        }

        public virtual MsgWithDetails ValidateStart(DatabaseApp dbApp, Database dbiPlus,
            ACComponent invoker,
            ProdOrderPartslist poList, List<IACConfigStore> configStores,
            PARole.ValidationBehaviour validationBehaviour,
            ACPartslistManager partsListManager, ACMatReqManager matReqManager)
        {
            MsgWithDetails detailMessages = new MsgWithDetails();
            if (!poList.ProdOrderBatchPlan_ProdOrderPartslist.Any())
                return detailMessages;

            if (!poList.Partslist.IsEnabled)
            {
                // Error50298: Bill of Material is not released.
                detailMessages.AddDetailMessage(new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "ValidateStart(10)",
                    Message = Root.Environment.TranslateMessage(this, "Error50298")
                });
            }

            if (   (poList.Partslist.EnabledFrom.HasValue && DateTime.Now < poList.Partslist.EnabledFrom.Value)
                || (poList.Partslist.EnabledTo.HasValue && DateTime.Now > poList.Partslist.EnabledTo.Value))
            {
                // Error50299: The validity period of the BOM has been exceeded.
                detailMessages.AddDetailMessage(new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Error,
                    ACIdentifier = "ValidateStart(20)",
                    Message = Root.Environment.TranslateMessage(this, "Error50299")
                });
            }

            bool hasBatchPlansWithoutDestination = poList.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => !c.FacilityReservation_ProdOrderBatchPlan.Any()).Any();
            if (hasBatchPlansWithoutDestination)
            {
                //Bei einem oder mehreren Batchplänen wurde kein Ziel eingetragen. Wollen Sie dennoch starten?
                detailMessages.AddDetailMessage(new Msg
                {
                    Source = GetACUrl(),
                    MessageLevel = eMsgLevel.Warning,
                    ACIdentifier = "ValidateStart(30)",
                    Message = Root.Environment.TranslateMessage(this, "Warning50008")
                });
            }

            IEnumerable<ProdOrderPartslistPos> rootPosList = dbApp.ProdOrderPartslistPos
                .Include(c => c.Material.MDFacilityManagementType)
                .Include("ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos")
                .Where(c => c.ProdOrderPartslistID == poList.ProdOrderPartslistID && c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot)
                .AsEnumerable();
            foreach (ProdOrderPartslistPos pos in rootPosList) //poList.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot))
            {
                //pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.AutoRefresh(pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPosReference, pos);
                //pos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.AutoRefresh(pos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosReference, pos);
                if (!pos.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos.Any())
                {
                    // Stücklistenposition {0} {1} {2} ist keinem Zwischenmaterial zugeordnet.
                    detailMessages.AddDetailMessage(new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Warning,
                        ACIdentifier = "ValidateStart(40)",
                        Message = Root.Environment.TranslateMessage(partsListManager, "Warning50013", pos.Sequence, pos.Material != null ? pos.Material.MaterialNo : "", pos.MaterialName)
                    });
                }
                if (pos.Material != null
                    && pos.Material.IsLotReservationNeeded
                    && !pos.FacilityReservation_ProdOrderPartslistPos.Where(c => !c.VBiACClassID.HasValue).Any())
                {
                    // Error50635: The material {0} {1} at position {2} requires reservation and no batch has been reserved.
                    detailMessages.AddDetailMessage(new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "ValidateStart(41)",
                        Message = Root.Environment.TranslateMessage(this, "Error50634", pos.Material.MaterialNo, pos.Material.MaterialName1, pos.Sequence)
                    });
                }
            }

            bool hasNotValidatedPos = poList.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => !c.IsValidated).Any();

            if (hasNotValidatedPos)
            {
                try
                {
                    partsListManager.CheckResourcesAndRoutingEvent += PartsListManager_CheckResourcesAndRoutingEvent;
                    partsListManager.CheckResourcesAndRouting(dbApp, dbiPlus, poList, configStores, validationBehaviour, detailMessages);
                }
                catch (Exception ec)
                {
                    Msg errMsg = new Msg() { MessageLevel = eMsgLevel.Error, Message = ec.Message };
                    detailMessages.AddDetailMessage(errMsg);
                }
                finally
                {
                    partsListManager.CheckResourcesAndRoutingEvent -= PartsListManager_CheckResourcesAndRoutingEvent;
                }
            }

            if (matReqManager != null && IsActiveMatReqCheck)
                CheckMaterialReq(dbApp, poList, matReqManager, ref detailMessages);

            return detailMessages;
        }

        protected virtual void PartsListManager_CheckResourcesAndRoutingEvent(object sender, CheckResourcesAndRoutingEventArgs e)
        {
        }

        public int SetBatchPlanValidated(DatabaseApp dbApp, ProdOrderPartslist poList)
        {
            int changedEntries = 0;
            var notValidatedEntries = poList.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => !c.IsValidated);
            foreach (var notValidatedPos in notValidatedEntries)
            {
                notValidatedPos.IsValidated = true;
                changedEntries++;
            }
            return changedEntries;
        }

        public static bool IsEnabledStartBatchPlan(ProdOrderBatchPlan prodOrderBatchPlan, ProdOrderPartslist prodOrderPartslist, ProdOrderPartslistPos intermediate)
        {

            bool isBatchNotValid = true;
            if(prodOrderBatchPlan != null && prodOrderPartslist != null && intermediate != null && prodOrderBatchPlan != null)
            {
                bool prodOrderorPartslistStarted =
                    prodOrderPartslist.ProdOrder.MDProdOrderState.ProdOrderState >= MDProdOrderState.ProdOrderStates.ProdFinished
                    || prodOrderPartslist.MDProdOrderState.ProdOrderState >= MDProdOrderState.ProdOrderStates.ProdFinished;

                bool batchCompletedOrInStartOrder = 
                    (
                        intermediate.MDProdOrderPartslistPosState != null
                        && (
                                intermediate.MDProdOrderPartslistPosState.ProdOrderPartslistPosState >= MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                                && intermediate.MDProdOrderPartslistPosState.ProdOrderPartslistPosState < MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.AutoStart
                            )
                    )
                    || prodOrderBatchPlan.PlanState >= GlobalApp.BatchPlanState.AutoStart;


                bool batchPlanInvalidOrWithoutReservation = !prodOrderBatchPlan.FacilityReservation_ProdOrderBatchPlan.Any() || !prodOrderBatchPlan.IsValid;

                isBatchNotValid = prodOrderorPartslistStarted || batchCompletedOrInStartOrder || batchPlanInvalidOrWithoutReservation;
            }
            return !isBatchNotValid;
        }

        #endregion

        #endregion
        #endregion

        private void CheckMaterialReq(DatabaseApp dbApp, ProdOrderPartslist poList, ACMatReqManager matReqManager, ref MsgWithDetails msg)
        {
            if (matReqManager == null)
                return;

            var result = matReqManager.CheckMaterialsRequirement(dbApp, poList.ProdOrderBatchPlan_ProdOrderPartslist);
            foreach (var item in result)
            {
                msg.AddDetailMessage(item);
            }

            if (ValidateBatchOverplan && poList != null && poList.ProdOrderBatchPlan_ProdOrderPartslist.Any())
            {
                double totalPlanned = poList.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => c.PlanStateIndex >= (short)GlobalApp.BatchPlanState.Created).Sum(c => c.TotalSize);
                double diff = totalPlanned - poList.TargetQuantity;
                if (diff > FacilityConst.C_ZeroCompare)
                {
                    // Warning50092: The sum of the planned Batchsizes exceeds the Orderquantity by {0}. Are you sure that you want to produce this overplanned quantities?
                    msg.AddDetailMessage(new Msg()
                    {
                        ACIdentifier = "Material requirement",
                        MessageLevel = eMsgLevel.Warning,
                        Message = Root.Environment.TranslateMessage(this, "Warning50092", diff.ToString("N2", CultureInfo.CurrentCulture))
                    });
                }
            }

        }

        private DateTime _LastRunPossibleRoutesCheck = DateTime.Now;

        public IEnumerable<FacilityReservationRoutes> CalculatePossibleRoutes(DatabaseApp dbApp, Database dbiPlus, ProdOrderBatchPlan batchPlan, List<IACConfigStore> configStores, ConfigManagerIPlus varioConfigManager, MsgWithDetails detailMessages)
        {
            TimeSpan ts = DateTime.Now - _LastRunPossibleRoutesCheck;
            if (ts.TotalSeconds < 5)
                return null;

            if (batchPlan == null)
                return null;

            List<FacilityReservationRoutes> result = new List<FacilityReservationRoutes>();

            foreach (FacilityReservation fr in batchPlan.FacilityReservation_ProdOrderBatchPlan)
            {
                if (!fr.FacilityID.HasValue)
                    continue;

                string targetCompACUrl = fr.FacilityACClass.ACUrlComponent;

                ACRoutingParameters routingParameters = new ACRoutingParameters()
                {
                    RoutingService = this.RoutingService,
                    Database = this.Database.ContextIPlus,
                    SelectionRuleID = PAProcessModule.SelRuleID_ProcessModule,
                    Direction = RouteDirections.Backwards,
                    MaxRouteAlternativesInLoop = 1,
                    IncludeReserved = true,
                    IncludeAllocated = true
                };


                var dischargingClass = GetACClassWFDischarging(dbApp, batchPlan.ProdOrderPartslist, batchPlan.VBiACClassWF, batchPlan.ProdOrderPartslistPos);

                var sources = dischargingClass.ParentACClass.DerivedClassesInProjects;
                if (sources == null)
                {
                    Messages.Info(this, string.Format("Successors are not found for the component with ACUrl {0}!", targetCompACUrl));
                    return null;
                }

                List<core.datamodel.ACClass> possibleSources = sources.ToList();
                core.datamodel.ACClass start = null;

                if (batchPlan.IplusVBiACClassWF != null)
                    start = ConfigManagerIPlus.FilterByAllowedInstances(dischargingClass.ACClassWF1_ParentACClassWF, configStores, varioConfigManager, possibleSources).FirstOrDefault();

                if (start == null)
                    start = sources.FirstOrDefault();

                routingParameters.SelectionRuleID = "PAMSilo.Deselector";
                routingParameters.Direction = RouteDirections.Forwards;

                RoutingResult routes = ACRoutingService.SelectRoutes(start, fr.FacilityACClass, routingParameters);

                if (result != null)
                {
                    fr.CalculatedRoute = routes.Routes.FirstOrDefault().GetRouteItemsGuid();
                    result.Add(new FacilityReservationRoutes() { Reservation = fr, Routes = new RoutingResult(routes.Routes, false, null) });
                }
            }

            Msg msgSave = dbApp.ACSaveChanges();
            if (msgSave != null)
                detailMessages.AddDetailMessage(msgSave);

            return result;
        }



        #endregion

    }
}
