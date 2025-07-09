using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, ConstApp.PlanningMR, Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]

    public class MRPPlanningManager : PARole
    {
        #region const
        public const string C_DefaultServiceACIdentifier = "MRPPlanningManager";
        #endregion

        #region c´tors

        public MRPPlanningManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {


            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region static Methods
        public static MRPPlanningManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<MRPPlanningManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<MRPPlanningManager> ACRefToServiceInstance(ACComponent requester)
        {
            MRPPlanningManager serviceInstance = GetServiceInstance(requester) as MRPPlanningManager;
            if (serviceInstance != null)
                return new ACRef<MRPPlanningManager>(serviceInstance, requester);
            return null;
        }

        #endregion

        #region Wizard

        public MRPResult PlanningForward(DatabaseApp databaseApp, PlanningMR currentPlanningMR, MRPResult mRPResult, bool increaseIndex = true)
        {
            if (currentPlanningMR.PlanningMRPhaseIndex < (short)MRPPlanningPhaseEnum.Finished)
            {
                if (increaseIndex)
                {
                    currentPlanningMR.PlanningMRPhaseIndex++;
                }
                switch (currentPlanningMR.MRPPlanningPhase)
                {
                    case MRPPlanningPhaseEnum.PlanDefinition:
                        break;
                    case MRPPlanningPhaseEnum.MaterialSelection:
                        if (increaseIndex)
                        {
                            GetPlanningMRCons(databaseApp, currentPlanningMR);
                        }
                        GetConsumptionModels(databaseApp, currentPlanningMR, mRPResult);
                        GetDefaultRecipies(databaseApp, mRPResult.PlanningPosition);
                        break;
                    case MRPPlanningPhaseEnum.ConsumptionBased:
                        if (mRPResult.PlanningPosition == null)
                        {
                            GetConsumptionModels(databaseApp, currentPlanningMR, mRPResult);
                        }

                        List<ConsumptionModel> byConsumption =
                               mRPResult
                               .PlanningPosition
                               .Where(c => c.PlanningMRCons.Material.MRPProcedureIndex == (short)MRPProcedure.ConsumptionBased)
                               .ToList();

                        if (increaseIndex)
                        {
                            CalculateByConsumption(databaseApp, mRPResult, byConsumption);
                        }

                        CalculateMaterialsByRequirements(databaseApp, mRPResult, byConsumption, increaseIndex);
                        break;
                    case MRPPlanningPhaseEnum.RequirementBased:
                        if (mRPResult.PlanningPosition == null)
                        {
                            GetConsumptionModels(databaseApp, currentPlanningMR, mRPResult);
                        }

                        List<ConsumptionModel> byRequierements =
                            mRPResult
                            .PlanningPosition
                            .Where(c => c.PlanningMRCons.Material.MRPProcedureIndex == (short)MRPProcedure.RequirementBased)
                            .ToList();

                        CalculateMaterialsByRequirements(databaseApp, mRPResult, byRequierements, increaseIndex);
                        break;
                    case MRPPlanningPhaseEnum.Fulfillment:
                        break;
                    case MRPPlanningPhaseEnum.Finished:
                        break;
                    default:
                        break;
                }
                databaseApp.ACSaveChanges();
            }
            return mRPResult;
        }

        public MRPResult PlanningBackward(DatabaseApp databaseApp, PlanningMR currentPlanningMR, MRPResult mRPResult)
        {
            if (currentPlanningMR.PlanningMRPhaseIndex > (short)MRPPlanningPhaseEnum.PlanDefinition)
            {
                currentPlanningMR.PlanningMRPhaseIndex--;
                switch (currentPlanningMR.MRPPlanningPhase)
                {
                    case MRPPlanningPhaseEnum.PlanDefinition:
                        break;
                    case MRPPlanningPhaseEnum.MaterialSelection:
                        break;
                    case MRPPlanningPhaseEnum.ConsumptionBased:
                        break;
                    case MRPPlanningPhaseEnum.RequirementBased:
                        break;
                    case MRPPlanningPhaseEnum.Fulfillment:
                        break;
                    case MRPPlanningPhaseEnum.Finished:
                        break;
                    default:
                        break;
                }
                databaseApp.ACSaveChanges();
            }

            return mRPResult;
        }

        #endregion

        #region Common

        public (DateTime planningMRFrom, DateTime planningMRTo) GetPlanningMRPeriod(PlanningMR planningMR)
        {
            DateTime planningMRFrom = DateTime.Now.Date;
            DateTime planningMRTo = planningMRFrom.AddDays(7);

            if (planningMR.RangeFrom != null)
            {
                planningMRFrom = planningMR.RangeFrom ?? DateTime.Now;
            }

            if (planningMR.RangeTo != null)
            {
                planningMRTo = planningMR.RangeTo ?? DateTime.Now;
            }

            return (planningMRFrom, planningMRTo);
        }

        public (DateTime planningMRConsFrom, DateTime planningMRConsTo) GetPlanningMRConsPeriod(PlanningMRCons planningMRCons)
        {
            DateTime planningMRConsFrom = planningMRCons.ConsumptionDate;
            DateTime planningMRConsTo = planningMRCons.ConsumptionDate.AddDays(7);
            return (planningMRConsFrom, planningMRConsTo);
        }

        public DateTime[] GetConsumptionDates(PlanningMR planningMR)
        {
            List<DateTime> dates = new List<DateTime>();
            (DateTime planningMRFrom, DateTime planningMRTo) = GetPlanningMRPeriod(planningMR);
            planningMRFrom = planningMRFrom.Date;
            while (planningMRFrom < planningMRTo)
            {
                dates.Add(planningMRFrom);
                planningMRFrom = planningMRFrom.AddDays(1);
            }
            return dates.ToArray();
        }

        public List<Material> GetMaterials(DatabaseApp databaseApp, MRPProcedure mRPProcedure)
        {
            List<Material> materials =
                databaseApp
                .Material
                .Where(c => c.MRPProcedureIndex == (short)mRPProcedure)
                .OrderBy(c => c.MaterialNo)
                .ToList();
            return materials;
        }

        public void GetPlanningMRCons(DatabaseApp databaseApp, PlanningMR planningMR)
        {
            List<Material> materials = databaseApp.Material.Where(c => c.MRPProcedureIndex > (short)MRPProcedure.None).ToList();
            DateTime[] consumptionDates = GetConsumptionDates(planningMR);
            foreach (Material material in materials)
            {
                foreach (DateTime consumptionDate in consumptionDates)
                {
                    PlanningMRCons planningMRCons =
                        planningMR
                        .PlanningMRCons_PlanningMR
                        .Where(c => c.MaterialID == material.MaterialID && c.ConsumptionDate == consumptionDate)
                        .FirstOrDefault();
                    if (planningMRCons == null)
                    {
                        planningMRCons = PlanningMRCons.NewACObject(databaseApp, planningMR);
                        planningMRCons.Material = material;
                        planningMRCons.ConsumptionDate = consumptionDate;
                        planningMR.PlanningMRCons_PlanningMR.Add(planningMRCons);
                    }
                    (DateTime planningMRFrom, DateTime planningMRTo) = GetPlanningMRPeriod(planningMR);
                    PlanningMRPos initalPlanningMRPos = GetInitialPlanningMRPos(databaseApp, planningMRCons, planningMRFrom);
                    if (initalPlanningMRPos == null)
                    {
                        initalPlanningMRPos = NewInitialPlanningMRPos(databaseApp, planningMRCons, planningMRFrom);
                        planningMRCons.PlanningMRPos_PlanningMRCons.Add(initalPlanningMRPos);
                    }
                }
            }
        }

        private PlanningMRPos GetInitialPlanningMRPos(DatabaseApp databaseApp, PlanningMRCons planningMRCons, DateTime from)
        {
            PlanningMRPos initalPlanningMRPos =
                        planningMRCons
                        .PlanningMRPos_PlanningMRCons
                        .Where(c =>
                                    c.ExpectedBookingDate == from
                                    && c.OutOrderPosID == null
                                    && c.ProdOrderPartslistPosID == null
                                    && c.PlanningMRProposalID == null
                                    && c.InOrderPosID == null
                                    && c.ProdOrderPartslistID == null
                        )
                        .OrderBy(c => c.ExpectedBookingDate)
                        .FirstOrDefault();
            return initalPlanningMRPos;
        }

        private PlanningMRPos NewInitialPlanningMRPos(DatabaseApp databaseApp, PlanningMRCons planningMRCons, DateTime from)
        {
            PlanningMRPos firstPlanningMRPos = PlanningMRPos.NewACObject(databaseApp, planningMRCons);
            firstPlanningMRPos.ExpectedBookingDate = from;
            firstPlanningMRPos.StoreQuantityUOM = GetStartStockQuantity(planningMRCons.Material, from);
            return firstPlanningMRPos;
        }

        private double GetStartStockQuantity(Material material, DateTime from)
        {
            double startStock = material.CurrentMaterialStock.StockQuantity;
            MaterialHistory materialHistory =
                material
                .MaterialHistory_Material
                .Where(c =>
                    c.History.TimePeriodIndex == (short)GlobalApp.TimePeriods.Day
                    && c.History.BalanceDate <= from
                )
                .OrderByDescending(c => c.History.BalanceDate)
                .FirstOrDefault();

            if (materialHistory != null)
            {
                startStock = materialHistory.StockQuantity;
            }
            return startStock;
        }

        public void GetConsumptionModels(DatabaseApp databaseApp, PlanningMR planningMR, MRPResult mRPResult)
        {
            List<ConsumptionModel> consumptionModels = new List<ConsumptionModel>();

            foreach (PlanningMRCons planningMRCons in planningMR.PlanningMRCons_PlanningMR)
            {
                ConsumptionModel model = new ConsumptionModel
                {
                    PlanningMRCons = planningMRCons
                };
                model.SelectedRecipeSource = planningMRCons.DefaultPartslist;
                consumptionModels.Add(model);
            }

            consumptionModels = consumptionModels.OrderBy(c => c.PlanningMRCons.Material.MaterialNo).ThenBy(c => c.PlanningMRCons.ConsumptionDate).ToList();
            mRPResult.PlanningPosition = consumptionModels;
        }

        public void GetDefaultRecipies(DatabaseApp databaseApp, List<ConsumptionModel> consumptionModels)
        {
            var query = consumptionModels.GroupBy(c => c.PlanningMRCons.Material.MaterialNo);
            foreach (var item in query)
            {
                List<Partslist> partslists =
                    databaseApp.Partslist
                    .Where(pl =>
                        pl.Material.MaterialNo == item.Key
                        && pl.IsEnabled
                    )
                    .OrderByDescending(c => c.PartslistVersion)
                    .ToList();

                foreach (var subItem in item)
                {
                    SetDefaultRecipe(databaseApp, subItem, partslists);
                }
            }
        }

        public void SetDefaultRecipe(DatabaseApp databaseApp, ConsumptionModel consumptionModel, List<Partslist> partslists)
        {
            consumptionModel.RecipeSourceList = partslists;

            if (consumptionModel.PlanningMRCons.DefaultPartslist != null)
            {
                consumptionModel.SelectedRecipeSource = consumptionModel.PlanningMRCons.DefaultPartslist;
                if (!consumptionModel.RecipeSourceList.Contains(consumptionModel.PlanningMRCons.DefaultPartslist))
                {
                    consumptionModel.RecipeSourceList.Add(consumptionModel.PlanningMRCons.DefaultPartslist);
                }
            }
            else
            {
                consumptionModel.SelectedRecipeSource = consumptionModel.RecipeSourceList.FirstOrDefault();
                consumptionModel.PlanningMRCons.DefaultPartslist = consumptionModel.SelectedRecipeSource;
            }

            consumptionModel.MRPProcedure = databaseApp.MRPProcedureList.Where(c => ((short)c.Value) == (short)consumptionModel.PlanningMRCons.Material.MRPProcedure).Select(c => c.ACCaption).FirstOrDefault();
        }

        #endregion

        #region Consumption


        private void CalculateByConsumption(DatabaseApp databaseApp, MRPResult mRPResult, List<ConsumptionModel> byConsumption)
        {
            foreach (ConsumptionModel consumption in byConsumption)
            {
                DateTime from = consumption.PlanningMRCons.ConsumptionDate;
                DateTime to = consumption.PlanningMRCons.ConsumptionDate.AddDays(1);

                double estimatedQuantity = GetEstimatedQuantity(databaseApp, consumption.PlanningMRCons.Material, from, to);
                consumption.PlanningMRCons.EstimatedQuantityUOM = estimatedQuantity;
            }
            mRPResult.ConsumptionPlanningPosition = byConsumption;
        }

        private double GetEstimatedQuantity(DatabaseApp databaseApp, Material material, DateTime? rangeFrom, DateTime? rangeTo)
        {
            double estimatedQuantity = 0;

            FacilityBooking lastBooking =
                databaseApp
                .FacilityBooking
                .Where(c => c.OutwardMaterialID == material.MaterialID)
                .OrderByDescending(c => c.InsertDate)
                .FirstOrDefault();

            if (lastBooking != null)
            {
                double estimatedQuantity1 = GetExtimatedConsumption(databaseApp, material, lastBooking.InsertDate.AddMonths(-1), lastBooking.InsertDate);
                double estimatedQuantity2 = GetExtimatedConsumption(databaseApp, material, lastBooking.InsertDate.AddMonths(-2), lastBooking.InsertDate.AddMonths(-1));
                estimatedQuantity = new double[] { estimatedQuantity1, estimatedQuantity2 }.Average();
            }

            return estimatedQuantity;
        }

        private double GetExtimatedConsumption(DatabaseApp databaseApp, Material material, DateTime rangeFrom, DateTime rangeTo)
        {
            return
                databaseApp
                .FacilityBookingCharge
                .Where(c =>
                    c.OutwardMaterialID == material.MaterialID
                    && c.InsertDate >= rangeFrom
                    && c.InsertDate < rangeTo
                    )
                .Select(c => c.OutwardQuantityUOM)
                .DefaultIfEmpty()
                .Sum();
        }

        #endregion

        #region Requierements


        // TODO: @aagincic - rewrite this in direction for distinct methods for calculation and model loading
        // TODO: @aagincic - name harmonization for this and other methods here
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseApp"></param>
        /// <param name="mRPResult"></param>
        /// <param name="byRequierements"></param>
        /// <param name="isFirstLoad"></param>
        private void CalculateMaterialsByRequirements(DatabaseApp databaseApp, MRPResult mRPResult, List<ConsumptionModel> byRequierements, bool isFirstLoad)
        {
            foreach (ConsumptionModel requirement in byRequierements)
            {
                if (isFirstLoad)
                {
                    AddNewRequirementPlanningMRPos(databaseApp, requirement);
                }
                else
                {
                    LoadExistingRequirementplanningMRPos(databaseApp, requirement);
                }
            }

            mRPResult.RequirementPlanningPosition = byRequierements;
        }

        private void AddNewRequirementPlanningMRPos(DatabaseApp databaseApp, ConsumptionModel requirement)
        {
            List<PlanningMRPos> items = new List<PlanningMRPos>();
            PlanningMRPos initialPlanningMRPos = GetInitialPlanningMRPos(databaseApp, requirement.PlanningMRCons, requirement.PlanningMRCons.ConsumptionDate);
            (DateTime planningMRConsFrom, DateTime planningMRConsTo) = GetPlanningMRConsPeriod(requirement.PlanningMRCons);

            Dictionary<DateTime, List<OutOrderPos>> outOrderPositions = GetOutOrderPosInTimeFrame(databaseApp, requirement.PlanningMRCons.Material.MaterialID, planningMRConsFrom, planningMRConsTo);
            Dictionary<DateTime, List<ProdOrderPartslistPos>> components = GetProdOrderComponentsInTimeFrame(databaseApp, requirement.PlanningMRCons.Material.MaterialID, planningMRConsFrom, planningMRConsTo);

            List<OutOrderPos> outOrderList = outOrderPositions.ContainsKey(planningMRConsFrom) ? outOrderPositions[planningMRConsFrom] : new List<OutOrderPos>();
            List<ProdOrderPartslistPos> componentList = components.ContainsKey(planningMRConsFrom) ? components[planningMRConsFrom] : new List<ProdOrderPartslistPos>();

            PlanningMRPos tempPos = initialPlanningMRPos;
            foreach (OutOrderPos outOrderPos in outOrderList)
            {
                tempPos = AddPlanningMRPos(databaseApp, requirement.PlanningMRCons, tempPos, outOrderPos, null);
                items.Add(tempPos);
                requirement.OutOrderPosList.Add(outOrderPos);
            }

            foreach (ProdOrderPartslistPos component in componentList)
            {
                tempPos = AddPlanningMRPos(databaseApp, requirement.PlanningMRCons, tempPos, null, component);
                items.Add(tempPos);
                requirement.ComponentList.Add(component);
            }
        }

        private void LoadExistingRequirementplanningMRPos(DatabaseApp databaseApp, ConsumptionModel requirement)
        {
            List<PlanningMRPos> items = requirement.PlanningMRCons.PlanningMRPos_PlanningMRCons.ToList();
            foreach (PlanningMRPos item in items)
            {
                if (item.OutOrderPos != null)
                {
                    requirement.OutOrderPosList.Add(item.OutOrderPos);
                }
                else if (item.ProdOrderPartslistPos != null)
                {
                    requirement.ComponentList.Add(item.ProdOrderPartslistPos);
                }
            }
        }

        public Dictionary<DateTime, List<OutOrderPos>> GetOutOrderPosInTimeFrame(DatabaseApp databaseApp, Guid materialID, DateTime? from, DateTime? to)
        {
            IEnumerable<OutOrderPos> outOrderLines =
                                 databaseApp
                                 .OutOrderPos
                                 .Where(c =>
                                         c.MaterialID == materialID
                                         && (from == null || c.OutOrder.OutOrderDate >= from)
                                         && (to == null || c.OutOrder.OutOrderDate < to)
                                         && c.OutOrder.MDOutOrderState.MDOutOrderStateIndex < (short)MDInOrderState.InOrderStates.Completed
                                         && c.MDOutOrderPlanState.MDOutOrderPlanStateIndex < (short)MDInOrderPosState.InOrderPosStates.Completed
                                         && (c.ActualQuantityUOM - c.TargetQuantityUOM) < 0
                                     )
                                 .OrderBy(c => c.OutOrder.OutOrderDate)
                                 .ThenBy(c => c.Sequence);

            return outOrderLines.GroupBy(c => c.OutOrder.OutOrderDate)
                               .ToDictionary(
                                   g => g.Key,
                                   g => g.ToList()
                               );
        }

        public Dictionary<DateTime, List<ProdOrderPartslistPos>> GetProdOrderComponentsInTimeFrame(DatabaseApp databaseApp, Guid materialID, DateTime? from, DateTime? to)
        {
            IEnumerable<ProdOrderPartslistPos> components =
                  databaseApp
                  .ProdOrderPartslistPos
                  .Where(c =>
                          c.MaterialID == materialID
                          && (from == null || c.ProdOrderPartslist.StartDate >= from)
                          && (to == null || c.ProdOrderPartslist.StartDate < to)
                          && c.ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex < (short)MDProdOrderState.ProdOrderStates.ProdFinished
                          && c.ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex < (short)MDProdOrderState.ProdOrderStates.ProdFinished
                          && c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex < (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                          && (c.ActualQuantityUOM - c.TargetQuantityUOM) < 0
                      );

            return components.GroupBy(c => c.ProdOrderPartslist.StartDate ?? DateTime.Now.Date)
                               .ToDictionary(
                                   g => g.Key,
                                   g => g.ToList()
                               );
        }

        public Dictionary<DateTime, List<ProdOrderPartslist>> GetProdOrderPartslistInTimeFrame(DatabaseApp databaseApp, Guid materialID, DateTime? from, DateTime? to)
        {
            IEnumerable<ProdOrderPartslist> components =
                  databaseApp
                  .ProdOrderPartslist
                  .Where(c =>
                          c.Partslist.MaterialID == materialID
                          && (from == null || c.StartDate >= from)
                          && (to == null || c.StartDate < to)
                          && c.ProdOrder.MDProdOrderState.MDProdOrderStateIndex < (short)MDProdOrderState.ProdOrderStates.ProdFinished
                          && c.MDProdOrderState.MDProdOrderStateIndex < (short)MDProdOrderState.ProdOrderStates.ProdFinished
                          && (c.ActualQuantity - c.TargetQuantity) < 0
                      );

            return components.GroupBy(c => c.StartDate ?? DateTime.Now)
                               .ToDictionary(
                                   g => g.Key,
                                   g => g.ToList()
                               );
        }

        private PlanningMRPos AddPlanningMRPos(DatabaseApp databaseApp, PlanningMRCons consumption, PlanningMRPos lastMrPos, OutOrderPos outOrderPos, ProdOrderPartslistPos component)
        {
            PlanningMRPos planningMRPos = PlanningMRPos.NewACObject(databaseApp, consumption);

            if (outOrderPos != null)
            {
                planningMRPos.OutOrderPos = outOrderPos;
                planningMRPos.StoreQuantityUOM = lastMrPos.StoreQuantityUOM + outOrderPos.DifferenceQuantityUOM;
            }
            else if (component != null)
            {
                planningMRPos.ProdOrderPartslistPos = component;
                planningMRPos.StoreQuantityUOM = lastMrPos.StoreQuantityUOM + component.DifferenceQuantityUOM;
            }

            consumption.PlanningMRPos_PlanningMRCons.Add(planningMRPos);

            return planningMRPos;
        }

        #endregion



        #region Validation

        public MsgWithDetails Validate(PlanningMR planningMR)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            switch (planningMR.MRPPlanningPhase)
            {
                case MRPPlanningPhaseEnum.PlanDefinition:
                    msgWithDetails = ValidatePlanDefinition(planningMR);
                    break;
                case MRPPlanningPhaseEnum.MaterialSelection:
                    break;
                case MRPPlanningPhaseEnum.ConsumptionBased:
                    break;
                case MRPPlanningPhaseEnum.RequirementBased:
                    break;
                case MRPPlanningPhaseEnum.Fulfillment:
                    break;
                case MRPPlanningPhaseEnum.Finished:
                    break;
                default:
                    break;
            }
            return msgWithDetails;
        }

        private MsgWithDetails ValidatePlanDefinition(PlanningMR planningMR)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            if (planningMR.RangeFrom == null)
            {
                Msg msg = new Msg
                {
                    MessageLevel = eMsgLevel.Error,
                    Message = "RangeFrom don't have value!"
                };
                msgWithDetails.AddDetailMessage(msg);
            }

            if (planningMR.RangeTo == null)
            {
                Msg msg = new Msg
                {
                    MessageLevel = eMsgLevel.Error,
                    Message = "RangeFrom don't have value!"
                };
                msgWithDetails.AddDetailMessage(msg);
            }

            if (planningMR.RangeFrom != null && planningMR.RangeTo != null && planningMR.RangeFrom > planningMR.RangeTo)
            {
                Msg msg = new Msg
                {
                    MessageLevel = eMsgLevel.Error,
                    Message = "RangeFrom must be before RangeTo!"
                };
                msgWithDetails.AddDetailMessage(msg);
            }

            return msgWithDetails;
        }

        public MsgWithDetails DeletePlanningMR(DatabaseApp databaseApp, PlanningMR currentPlanningMR)
        {
            List<PlanningMRProposal> proposals = databaseApp.PlanningMRProposal.Where(c => c.PlanningMRID == currentPlanningMR.PlanningMRID).ToList();
            List<PlanningMRCons> planningMRCons = databaseApp.PlanningMRCons.Where(c => c.PlanningMRID == currentPlanningMR.PlanningMRID).ToList();
            List<PlanningMRPos> planningMRPos = planningMRCons.SelectMany(c => c.PlanningMRPos_PlanningMRCons).ToList();


            foreach (PlanningMRProposal proposal in proposals)
            {
                proposal.DeleteACObject(databaseApp, false);
            }

            foreach (PlanningMRPos plMrPos in planningMRPos)
            {
                plMrPos.DeleteACObject(databaseApp, false);
            }

            foreach (PlanningMRCons consumption in planningMRCons)
            {
                consumption.DeleteACObject(databaseApp, false);
            }

            databaseApp.PlanningMR.DeleteObject(currentPlanningMR);
            return databaseApp.ACSaveChanges();
        }
        #endregion
    }
}
