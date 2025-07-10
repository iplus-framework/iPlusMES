using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    int phaseIndex = (int)currentPlanningMR.MRPPlanningPhase;
                    phaseIndex++;
                    MRPPlanningPhaseEnum newPhase = (MRPPlanningPhaseEnum)phaseIndex;
                    currentPlanningMR.MRPPlanningPhase = newPhase;
                }
                switch (currentPlanningMR.MRPPlanningPhase)
                {
                    case MRPPlanningPhaseEnum.PlanDefinition:
                        break;
                    case MRPPlanningPhaseEnum.MaterialSelection:
                        if (increaseIndex)
                        {
                            GetPlanningMRConsumptions(databaseApp, currentPlanningMR);
                        }
                        mRPResult.PlanningPosition = GetPlanningPositions(databaseApp, currentPlanningMR);
                        GetDefaultRecipies(databaseApp, mRPResult.PlanningPosition);
                        break;
                    case MRPPlanningPhaseEnum.ConsumptionBased:
                        if (mRPResult.PlanningPosition == null)
                        {
                            mRPResult.PlanningPosition = GetPlanningPositions(databaseApp, currentPlanningMR);
                        }

                        List<ConsumptionModel> byConsumption =
                               mRPResult
                               .PlanningPosition
                               .Where(c => c.PlanningMRCons.Material.MRPProcedureIndex == (short)MRPProcedure.ConsumptionBased)
                               .ToList();

                        if (increaseIndex)
                        {
                            byConsumption = CalculateByConsumption(databaseApp, mRPResult, byConsumption);
                            List<ConsumptionModel> consByReq = CalculateMaterialsByRequirements(databaseApp, mRPResult, byConsumption);
                            if (consByReq.Any())
                            {
                                CorrectConsumptionFromRequirements(databaseApp, consByReq);
                            }
                        }
                        else
                        {
                            LoadExistingRequirementplanningMRPos(databaseApp, byConsumption);
                        }

                        mRPResult.ConsumptionPlanningPosition = byConsumption;
                        break;
                    case MRPPlanningPhaseEnum.RequirementBased:
                        if (mRPResult.PlanningPosition == null)
                        {
                            mRPResult.PlanningPosition = GetPlanningPositions(databaseApp, currentPlanningMR);
                        }

                        List<ConsumptionModel> byRequirements =
                            mRPResult
                            .PlanningPosition
                            .Where(c => c.PlanningMRCons.Material.MRPProcedureIndex == (short)MRPProcedure.RequirementBased)
                            .ToList();

                        if (increaseIndex)
                        {
                            byRequirements = CalculateMaterialsByRequirements(databaseApp, mRPResult, byRequirements);
                            if (byRequirements.Any())
                            {
                                CorrectConsumptionFromRequirements(databaseApp, byRequirements);
                            }
                        }
                        else
                        {
                            LoadExistingRequirementplanningMRPos(databaseApp, byRequirements);
                        }

                        mRPResult.RequirementPlanningPosition = byRequirements;
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


        public MRPResult PlanningBackward(DatabaseApp databaseApp, PlanningMR currentPlanningMR, MRPResult mRPResult, bool decreaseIndex = true)
        {
            if (currentPlanningMR.PlanningMRPhaseIndex > (short)MRPPlanningPhaseEnum.PlanDefinition)
            {
                if (decreaseIndex)
                {
                    int phaseIndex = (int)currentPlanningMR.MRPPlanningPhase;
                    phaseIndex--;
                    MRPPlanningPhaseEnum newPhase = (MRPPlanningPhaseEnum)phaseIndex;
                    currentPlanningMR.MRPPlanningPhase = newPhase;
                }
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
            DateTime planningMRConsTo = planningMRCons.ConsumptionDate.AddDays(1);
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

        public void GetPlanningMRConsumptions(DatabaseApp databaseApp, PlanningMR planningMR)
        {
            List<Material> materials = databaseApp.Material.Where(c => c.MRPProcedureIndex > (short)MRPProcedure.None).ToList();
            DateTime consumptionDate = planningMR.RangeFrom ?? DateTime.Now.Date;
            foreach (Material material in materials)
            {
                PlanningMRCons planningMRCons = GetPlanningMRCons(databaseApp, planningMR, material.MaterialNo, consumptionDate);
                SetInitialPlanningMRPos(databaseApp, planningMR, planningMRCons);
            }
        }

        public PlanningMRCons GetPlanningMRCons(DatabaseApp databaseApp, PlanningMR planningMR, string materialNo, DateTime consumptionDate)
        {
            PlanningMRCons planningMRCons =
                    planningMR
                    .PlanningMRCons_PlanningMR
                    .Where(c => c.Material.MaterialNo == materialNo && c.ConsumptionDate == consumptionDate)
                    .FirstOrDefault();

            if (planningMRCons == null)
            {
                planningMRCons = PlanningMRCons.NewACObject(databaseApp, planningMR);
                Material material = databaseApp.Material.Where(c => c.MaterialNo == materialNo).FirstOrDefault();
                planningMRCons.Material = material;
                planningMRCons.ConsumptionDate = consumptionDate;
                planningMR.PlanningMRCons_PlanningMR.Add(planningMRCons);
            }

            return planningMRCons;
        }

        public void SetInitialPlanningMRPos(DatabaseApp databaseApp, PlanningMR planningMR, PlanningMRCons planningMRCons)
        {
            PlanningMRPos initalPlanningMRPos = GetInitialPlanningMRPos(databaseApp, planningMRCons, planningMRCons.ConsumptionDate);
            if (initalPlanningMRPos == null)
            {
                initalPlanningMRPos = NewInitialPlanningMRPos(databaseApp, planningMRCons, planningMRCons.ConsumptionDate);
                planningMRCons.PlanningMRPos_PlanningMRCons.Add(initalPlanningMRPos);
            }
        }

        private PlanningMRPos GetInitialPlanningMRPos(DatabaseApp databaseApp, PlanningMRCons planningMRCons, DateTime consumptionDate)
        {
            PlanningMRPos initalPlanningMRPos =
                        planningMRCons
                        .PlanningMRPos_PlanningMRCons
                        .Where(c =>
                                    c.ExpectedBookingDate == consumptionDate
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

        private PlanningMRPos NewInitialPlanningMRPos(DatabaseApp databaseApp, PlanningMRCons planningMRCons, DateTime consumptionDate)
        {
            PlanningMRPos firstPlanningMRPos = PlanningMRPos.NewACObject(databaseApp, planningMRCons);
            firstPlanningMRPos.ExpectedBookingDate = consumptionDate;
            firstPlanningMRPos.StoreQuantityUOM = GetStartStockQuantity(planningMRCons.Material, consumptionDate);
            return firstPlanningMRPos;
        }

        private double GetStartStockQuantity(Material material, DateTime from)
        {
            double startStock = material.MaterialStock_Material.Select(c => c.StockQuantity).FirstOrDefault();
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

        public List<ConsumptionModel> GetPlanningPositions(DatabaseApp databaseApp, PlanningMR planningMR)
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
            return consumptionModels;
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

        public void SetDefaultRecipe(DatabaseApp databaseApp, ConsumptionModel consumptionModel)
        {
            List<Partslist> partslists =
                    databaseApp.Partslist
                    .Where(pl =>
                        pl.Material.MaterialNo == consumptionModel.PlanningMRCons.Material.MaterialNo
                        && pl.IsEnabled
                    )
                    .OrderByDescending(c => c.PartslistVersion)
                    .ToList();
            SetDefaultRecipe(databaseApp, consumptionModel, partslists);
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

        public List<PlanningMRItem<double>> GetConsumptions(DatabaseApp databaseApp, DateTime[] consumptionDates, string[] materialNos)
        {
            List<PlanningMRItem<double>> result = new List<PlanningMRItem<double>>();
            foreach (DateTime consumptionDate in consumptionDates.ToArray())
            {
                foreach (string materialNo in materialNos)
                {
                    double estimatedQuantity = GetEstimatedQuantity(databaseApp, materialNo, consumptionDate, consumptionDate.AddDays(1));
                    if (estimatedQuantity > 0.1)
                    {
                        PlanningMRItem<double> planningMRItem = new PlanningMRItem<double>
                        {
                            ConsumptionDate = consumptionDate,
                            MaterialNo = materialNo,
                            Item = estimatedQuantity
                        };
                        result.Add(planningMRItem);
                    }
                }
            }

            return result;
        }

        private List<ConsumptionModel> CalculateByConsumption(DatabaseApp databaseApp, MRPResult mRPResult, List<ConsumptionModel> byConsumption)
        {
            List<ConsumptionModel> newByConsumption = new List<ConsumptionModel>();
            DateTime[] consumptionDates = GetConsumptionDates(mRPResult.PlanningMR);
            string[] materialNos = byConsumption.Select(c => c.PlanningMRCons.Material.MaterialNo).Distinct().ToArray();
            List<PlanningMRItem<double>> consumptions = GetConsumptions(databaseApp, consumptionDates, materialNos);

            foreach (PlanningMRItem<double> consumption in consumptions)
            {
                ConsumptionModel consumptionModel =
                        byConsumption
                        .Where(c => c.PlanningMRCons.Material.MaterialNo == consumption.MaterialNo && c.PlanningMRCons.ConsumptionDate == consumption.ConsumptionDate)
                        .FirstOrDefault();

                if (consumptionModel == null)
                {
                    consumptionModel = GetConsumptionModel(databaseApp, mRPResult.PlanningMR, consumption.MaterialNo, consumption.ConsumptionDate);
                }
                consumptionModel.PlanningMRCons.EstimatedQuantityUOM = consumption.Item;
                newByConsumption.Add(consumptionModel);
            }

            List<ConsumptionModel> byConsumptionForDelete = 
                byConsumption
                .Where(c => 
                            !newByConsumption.Select(x => x.PlanningMRCons.PlanningMRConsID).Contains(c.PlanningMRCons.PlanningMRConsID)
                        )
                .ToList();
            foreach (var forDelete in byConsumptionForDelete)
            {
                DeleteConsumption(databaseApp, forDelete, mRPResult.PlanningMR);
            }

            newByConsumption = newByConsumption.OrderBy(c => c.PlanningMRCons.Material.MaterialNo).ThenBy(c => c.PlanningMRCons.ConsumptionDate).ToList();

            return newByConsumption;
        }

        public ConsumptionModel GetConsumptionModel(DatabaseApp databaseApp, PlanningMR planningMR, string materialNo, DateTime consumptionDate)
        {
            PlanningMRCons planningMRCons = GetPlanningMRCons(databaseApp, planningMR, materialNo, consumptionDate);
            SetInitialPlanningMRPos(databaseApp, planningMR, planningMRCons);

            ConsumptionModel consumptionModel = new ConsumptionModel
            {
                PlanningMRCons = planningMRCons
            };
            SetDefaultRecipe(databaseApp, consumptionModel);
            return consumptionModel;
        }

        public void DeleteConsumption(DatabaseApp databaseApp, ConsumptionModel consumptionModel, PlanningMR planningMR)
        {
            if (consumptionModel != null && consumptionModel.PlanningMRCons != null)
            {
                foreach (PlanningMRPos planningMRPos in consumptionModel.PlanningMRCons.PlanningMRPos_PlanningMRCons.ToList())
                {
                    planningMRPos.DeleteACObject(databaseApp, false);
                }
                planningMR.PlanningMRCons_PlanningMR.Remove(consumptionModel.PlanningMRCons);
                consumptionModel.PlanningMRCons.DeleteACObject(databaseApp, false);
            }
        }

        private double GetEstimatedQuantity(DatabaseApp databaseApp, string materialNo, DateTime rangeFrom, DateTime rangeTo)
        {
            double estimatedQuantity = 0;

            FacilityBooking lastBooking =
                databaseApp
                .FacilityBooking
                .Where(c => c.OutwardMaterial != null && c.OutwardMaterial.MaterialNo == materialNo)
                .OrderByDescending(c => c.InsertDate)
                .FirstOrDefault();

            if (lastBooking != null)
            {
                DateTime dateTime = lastBooking.InsertDate.Date;
                DateTime rangeFromV1 = GetPreviousWeekday(dateTime, rangeFrom.DayOfWeek);
                DateTime rangeToV1 = rangeFromV1.Add(rangeTo - rangeFrom);

                double estimatedQuantity1 = GetExtimatedConsumption(databaseApp, materialNo, rangeFromV1, rangeToV1);
                double estimatedQuantity2 = GetExtimatedConsumption(databaseApp, materialNo, rangeFromV1.AddMonths(-1), rangeToV1.AddMonths(-1));
                double estimatedQuantity3 = GetExtimatedConsumption(databaseApp, materialNo, rangeFromV1.AddMonths(-2), rangeToV1.AddMonths(-2));
                estimatedQuantity = new double[] { estimatedQuantity1, estimatedQuantity2, estimatedQuantity3 }.Average();
            }

            return estimatedQuantity;
        }

        public static DateTime GetPreviousWeekday(DateTime startDate, DayOfWeek targetDay)
        {
            int daysToAdd = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;
            daysToAdd = daysToAdd == 0 ? 7 : daysToAdd;
            return startDate.AddDays(-daysToAdd);
        }

        private double GetExtimatedConsumption(DatabaseApp databaseApp, string materialNo, DateTime rangeFrom, DateTime rangeTo)
        {
            return
                databaseApp
                .FacilityBookingCharge
                .Where(c =>
                    c.OutwardMaterial != null && c.OutwardMaterial.MaterialNo == materialNo
                    && c.InsertDate >= rangeFrom
                    && c.InsertDate < rangeTo
                    )
                .Select(c => c.OutwardQuantityUOM)
                .DefaultIfEmpty()
                .Sum();
        }

        private void CorrectConsumptionFromRequirements(DatabaseApp databaseApp, List<ConsumptionModel> byConsumption)
        {
            foreach (ConsumptionModel consumption in byConsumption)
            {
                PlanningMRCons planningMRCons = consumption.PlanningMRCons;
                List<PlanningMRPos> planningMRPosList =
                    planningMRCons
                    .PlanningMRPos_PlanningMRCons
                    .Where(c => c.OutOrderPosID != null || c.ProdOrderPartslistPosID != null)
                    .ToList();

                planningMRCons.ReqCorrectionQuantityUOM = 0;
                foreach (PlanningMRPos planningMRPos in planningMRPosList)
                {
                    // DifferenceQuantityUOM is negative when amount is not reached
                    if (planningMRPos.OutOrderPos != null)
                    {
                        planningMRCons.ReqCorrectionQuantityUOM -= planningMRPos.OutOrderPos.DifferenceQuantityUOM;
                    }
                    else if (planningMRPos.ProdOrderPartslistPos != null)
                    {
                        planningMRCons.ReqCorrectionQuantityUOM -= planningMRPos.ProdOrderPartslistPos.DifferenceQuantityUOM;
                    }
                }

                planningMRCons.RequiredQuantityUOM = planningMRCons.EstimatedQuantityUOM - planningMRCons.ReqCorrectionQuantityUOM;
            }
        }

        #endregion

        #region Requirements

        public List<PlanningMRItem<PlanningMRRequirements>> GetRequirements(DatabaseApp databaseApp, PlanningMR planningMR, List<ConsumptionModel> byRequirements)
        {
            List<PlanningMRItem<PlanningMRRequirements>> requirements = new List<PlanningMRItem<PlanningMRRequirements>>();

            (DateTime planningMRFrom, DateTime planningMRTo) = GetPlanningMRPeriod(planningMR);
            string[] materialNos = byRequirements.Select(c => c.PlanningMRCons.Material.MaterialNo).Distinct().ToArray();

            foreach (string materialNo in materialNos)
            {
                Dictionary<DateTime, List<OutOrderPos>> outOrderPositions = GetOutOrderPosInTimeFrame(databaseApp, materialNo, planningMRFrom, planningMRTo);
                Dictionary<DateTime, List<ProdOrderPartslistPos>> components = GetProdOrderComponentsInTimeFrame(databaseApp, materialNo, planningMRFrom, planningMRTo);

                DateTime consumptionDate = planningMRFrom.Date;
                while (consumptionDate < planningMRTo.Date)
                {
                    bool haveOutOrder = outOrderPositions.ContainsKey(consumptionDate) && outOrderPositions[consumptionDate].Count > 0;
                    bool haveComponents = components.ContainsKey(consumptionDate) && components[consumptionDate].Count > 0;

                    if (haveOutOrder || haveComponents)
                    {
                        List<OutOrderPos> outOrderList = outOrderPositions.ContainsKey(consumptionDate) ? outOrderPositions[consumptionDate] : new List<OutOrderPos>();
                        List<ProdOrderPartslistPos> componentList = components.ContainsKey(consumptionDate) ? components[consumptionDate] : new List<ProdOrderPartslistPos>();

                        PlanningMRItem<PlanningMRRequirements> requirement = new PlanningMRItem<PlanningMRRequirements>
                        {
                            ConsumptionDate = consumptionDate,
                            MaterialNo = materialNo,
                            Item = new PlanningMRRequirements
                            {
                                OutOrderPosList = outOrderList,
                                Components = componentList
                            }
                        };

                        requirements.Add(requirement);
                    }
                    consumptionDate = consumptionDate.AddDays(1);
                }
            }

            return requirements;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseApp"></param>
        /// <param name="mRPResult"></param>
        /// <param name="byRequirements"></param>
        /// <param name="isFirstLoad"></param>
        private List<ConsumptionModel> CalculateMaterialsByRequirements(DatabaseApp databaseApp, MRPResult mRPResult, List<ConsumptionModel> byRequirements)
        {
            List<ConsumptionModel> newByRequirements = new List<ConsumptionModel>();

            List<PlanningMRItem<PlanningMRRequirements>> requirements = GetRequirements(databaseApp, mRPResult.PlanningMR, byRequirements);
            foreach (PlanningMRItem<PlanningMRRequirements> requirement in requirements)
            {
                ConsumptionModel consumption =
                            byRequirements
                            .Where(c => c.PlanningMRCons.Material.MaterialNo == requirement.MaterialNo && c.PlanningMRCons.ConsumptionDate == requirement.ConsumptionDate)
                            .FirstOrDefault();

                if (requirement == null)
                {
                    consumption = GetConsumptionModel(databaseApp, mRPResult.PlanningMR, requirement.MaterialNo, requirement.ConsumptionDate);
                }
                newByRequirements.Add(consumption);

                PlanningMRPos initialPlanningMRPos = GetInitialPlanningMRPos(databaseApp, consumption.PlanningMRCons, requirement.ConsumptionDate);

                PlanningMRPos tempPos = initialPlanningMRPos;
                foreach (OutOrderPos outOrderPos in requirement.Item.OutOrderPosList)
                {
                    tempPos = AddPlanningMRPos(databaseApp, consumption.PlanningMRCons, tempPos, outOrderPos, null);
                    consumption.OutOrderPosList.Add(outOrderPos);
                }

                foreach (ProdOrderPartslistPos component in requirement.Item.Components)
                {
                    tempPos = AddPlanningMRPos(databaseApp, consumption.PlanningMRCons, tempPos, null, component);
                    consumption.ComponentList.Add(component);
                }
            }

            List<ConsumptionModel> byRequirementsToDelte =
                byRequirements
                .Where(c =>
                        c.PlanningMRCons.EstimatedQuantityUOM == 0 // not delete consumption based elements
                        && !newByRequirements.Select(x => x.PlanningMRCons.ConsumptionDate).Contains(c.PlanningMRCons.ConsumptionDate))
                .ToList();
            foreach (var forDelete in byRequirementsToDelte)
            {
                DeleteConsumption(databaseApp, forDelete, mRPResult.PlanningMR);
            }


            newByRequirements = newByRequirements.OrderBy(c => c.PlanningMRCons.Material.MaterialNo).ThenBy(c => c.PlanningMRCons.ConsumptionDate).ToList();

            return newByRequirements;
        }

        public void LoadExistingRequirementplanningMRPos(DatabaseApp databaseApp, List<ConsumptionModel> byRequirements)
        {
            foreach (ConsumptionModel requirement in byRequirements)
            {
                LoadExistingRequirementplanningMRPos(databaseApp, requirement);
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

        public Dictionary<DateTime, List<OutOrderPos>> GetOutOrderPosInTimeFrame(DatabaseApp databaseApp, string materialNo, DateTime? from, DateTime? to)
        {
            IEnumerable<OutOrderPos> outOrderLines =
                                 databaseApp
                                 .OutOrderPos
                                 .Where(c =>
                                         c.Material.MaterialNo == materialNo
                                         && c.TargetDeliveryDate != null
                                         && (from == null || c.OutOrder.TargetDeliveryDate >= from)
                                         && (to == null || c.OutOrder.TargetDeliveryDate < to)
                                         && c.OutOrder.MDOutOrderState.MDOutOrderStateIndex < (short)MDInOrderState.InOrderStates.Completed
                                         && c.MDOutOrderPlanState.MDOutOrderPlanStateIndex < (short)MDInOrderPosState.InOrderPosStates.Completed
                                         && (c.ActualQuantityUOM - c.TargetQuantityUOM) < 0
                                     )
                                 .OrderBy(c => c.OutOrder.OutOrderDate)
                                 .ThenBy(c => c.Sequence)
                                 .AsEnumerable();

            return outOrderLines.GroupBy(c => c.OutOrder.OutOrderDate.Date)
                               .ToDictionary(
                                   g => g.Key,
                                   g => g.ToList()
                               );
        }

        public Dictionary<DateTime, List<ProdOrderPartslistPos>> GetProdOrderComponentsInTimeFrame(DatabaseApp databaseApp, string materialNo, DateTime? from, DateTime? to)
        {
            IEnumerable<ProdOrderPartslistPos> components =
                  databaseApp
                  .ProdOrderPartslistPos
                  .Where(c =>
                          c.Material.MaterialNo == materialNo
                          && c.ProdOrderPartslist.TargetDeliveryDate != null
                          && (from == null || c.ProdOrderPartslist.TargetDeliveryDate >= from)
                          && (to == null || c.ProdOrderPartslist.TargetDeliveryDate < to)
                          && c.ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex < (short)MDProdOrderState.ProdOrderStates.ProdFinished
                          && c.ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex < (short)MDProdOrderState.ProdOrderStates.ProdFinished
                          && c.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex < (short)MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                          && (c.ActualQuantityUOM - c.TargetQuantityUOM) < 0
                      )
                  .AsEnumerable();

            return components.GroupBy(c => c.ProdOrderPartslist.TargetDeliveryDate.Value.Date)
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
