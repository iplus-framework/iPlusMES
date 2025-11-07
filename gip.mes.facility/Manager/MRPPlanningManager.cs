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

        public MRPResult DoPlanningForward(DatabaseApp databaseApp, PlanningMR currentPlanningMR, MRPResult mRPResult, MRPPlanningPhaseEnum planningPhase, bool increaseIndex = true)
        {
            if (increaseIndex && currentPlanningMR.MRPPlanningPhase == planningPhase)
            {
                planningPhase = IncreaseIndex(planningPhase);
            }
            if (increaseIndex)
            {
                currentPlanningMR.MRPPlanningPhase = planningPhase;
            }

            switch (planningPhase)
            {
                case MRPPlanningPhaseEnum.PlanDefinition:
                    break;
                case MRPPlanningPhaseEnum.MaterialSelection:
                    if (increaseIndex)
                    {
                        CreatePlanningMRConsumptions(databaseApp, currentPlanningMR);
                        if (mRPResult.PlanningPosition == null)
                        {
                            mRPResult.PlanningPosition = GetPlanningPositions(databaseApp, currentPlanningMR);
                        }
                        GetDefaultRecipies(databaseApp, mRPResult.PlanningPosition);
                    }
                    break;
                case MRPPlanningPhaseEnum.ConsumptionBased:
                    mRPResult.PlanningPosition = 
                            mRPResult
                           .PlanningPosition
                           .Where(c => c.PlanningMRCons.Material?.MRPProcedureIndex == (short)MRPProcedure.ConsumptionBased)
                           .ToList();
                    if (increaseIndex)
                    {
                        mRPResult.PlanningPosition = CalculateByConsumption(databaseApp, mRPResult, mRPResult.PlanningPosition);
                        List<ConsumptionModel> consByReq = CalculateMaterialsByRequirements(databaseApp, mRPResult, mRPResult.PlanningPosition);
                        if (consByReq.Any())
                        {
                            CorrectConsumptionFromRequirements(databaseApp, consByReq);
                        }
                    }
                    break;
                case MRPPlanningPhaseEnum.RequirementBased:
                    mRPResult.PlanningPosition =
                           mRPResult
                           .PlanningPosition
                           .Where(c => c.PlanningMRCons.Material != null && c.PlanningMRCons.Material.MRPProcedureIndex == (short)MRPProcedure.RequirementBased)
                           .ToList();
                    if (increaseIndex)
                    {
                        mRPResult.PlanningPosition = CalculateMaterialsByRequirements(databaseApp, mRPResult, mRPResult.PlanningPosition);
                        if (mRPResult.PlanningPosition.Any())
                        {
                            CorrectConsumptionFromRequirements(databaseApp, mRPResult.PlanningPosition);
                        }
                    }
                    break;
                case MRPPlanningPhaseEnum.FulfillmentProduction:
                    break;
                case MRPPlanningPhaseEnum.FulfillmentInOrder:
                    break;
                case MRPPlanningPhaseEnum.Finished:
                    break;
                default:
                    break;
            }

            databaseApp.ACSaveChanges();

            mRPResult = DoStepLoad(databaseApp, currentPlanningMR, mRPResult, planningPhase);


            return mRPResult;
        }

        public MRPResult DoPlanningBackward(DatabaseApp databaseApp, PlanningMR currentPlanningMR, MRPResult mRPResult, MRPPlanningPhaseEnum planningPhase, bool decreaseIndex = true)
        {
            if (decreaseIndex && currentPlanningMR.MRPPlanningPhase == planningPhase)
            {
                planningPhase = DecreaseIndex(planningPhase);
                currentPlanningMR.MRPPlanningPhase = planningPhase;
            }

            if (planningPhase < currentPlanningMR.MRPPlanningPhase)
            {
                mRPResult.PlanningPosition = GetPlanningPositions(databaseApp, currentPlanningMR);
            }
            switch (planningPhase)
            {
                case MRPPlanningPhaseEnum.PlanDefinition:
                    break;
                case MRPPlanningPhaseEnum.MaterialSelection:
                    break;
                case MRPPlanningPhaseEnum.ConsumptionBased:
                    mRPResult.PlanningPosition = mRPResult
                       .PlanningPosition
                       .Where(c => c.PlanningMRCons.Material?.MRPProcedureIndex == (short)MRPProcedure.ConsumptionBased)
                       .ToList();
                    break;
                case MRPPlanningPhaseEnum.RequirementBased:
                    mRPResult.PlanningPosition =
                        mRPResult
                        .PlanningPosition
                        .Where(c => c.PlanningMRCons.Material != null && c.PlanningMRCons.Material.MRPProcedureIndex == (short)MRPProcedure.RequirementBased)
                        .ToList();
                    break;
                case MRPPlanningPhaseEnum.FulfillmentProduction:
                    break;
                case MRPPlanningPhaseEnum.FulfillmentInOrder:
                    break;
                case MRPPlanningPhaseEnum.Finished:
                    break;
                default:
                    break;
            }

            databaseApp.ACSaveChanges();

            mRPResult = DoStepLoad(databaseApp, currentPlanningMR, mRPResult, planningPhase);


            return mRPResult;
        }

        public MRPResult DoStepLoad(DatabaseApp databaseApp, PlanningMR currentPlanningMR, MRPResult mRPResult, MRPPlanningPhaseEnum planningPhase)
        {
            if (planningPhase > MRPPlanningPhaseEnum.MaterialSelection || (mRPResult.PlanningPosition == null == planningPhase > MRPPlanningPhaseEnum.MaterialSelection))
            {
                mRPResult.PlanningPosition = GetPlanningPositions(databaseApp, currentPlanningMR);
            }
            switch (planningPhase)
            {
                case MRPPlanningPhaseEnum.PlanDefinition:
                    break;
                case MRPPlanningPhaseEnum.MaterialSelection:
                    break;
                case MRPPlanningPhaseEnum.ConsumptionBased:
                    LoadExistingRequirementplanningMRPos(databaseApp, mRPResult.PlanningPosition);
                    break;
                case MRPPlanningPhaseEnum.RequirementBased:
                    LoadExistingRequirementplanningMRPos(databaseApp, mRPResult.PlanningPosition);
                    break;
                case MRPPlanningPhaseEnum.FulfillmentProduction:
                    break;
                case MRPPlanningPhaseEnum.FulfillmentInOrder:
                    break;
                case MRPPlanningPhaseEnum.Finished:
                    break;
                default:
                    break;
            }

            mRPResult.CurrentPlanningPhase = planningPhase;
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


        public void CreatePlanningMRConsumptions(DatabaseApp databaseApp, PlanningMR planningMR)
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

        private PlanningMRPos GetLastPlanningMRPos(DatabaseApp databaseApp, PlanningMRCons planningMRCons, DateTime consumptionDate)
        {
            PlanningMRPos initalPlanningMRPos =
                        planningMRCons
                        .PlanningMRPos_PlanningMRCons
                        .Where(c =>
                                    c.ExpectedBookingDate == consumptionDate
                        )
                        .OrderByDescending(c => c.InsertDate)
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

        public MRPPlanningPhaseEnum IncreaseIndex(MRPPlanningPhaseEnum phase)
        {
            MRPPlanningPhaseEnum newPhase = phase;
            if (newPhase < MRPPlanningPhaseEnum.Finished)
            {
                int phaseIndex = (int)newPhase;
                phaseIndex++;
                newPhase = (MRPPlanningPhaseEnum)phaseIndex;
            }
            return newPhase;
        }

        public MRPPlanningPhaseEnum DecreaseIndex(MRPPlanningPhaseEnum phase)
        {
            MRPPlanningPhaseEnum newPhase = phase;
            if (newPhase > MRPPlanningPhaseEnum.PlanDefinition)
            {
                int phaseIndex = (int)newPhase;
                phaseIndex--;
                newPhase = (MRPPlanningPhaseEnum)phaseIndex;
            }
            return newPhase;
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

        public DateTime GetPreviousWeekday(DateTime startDate, DayOfWeek targetDay)
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
                }

                foreach (ProdOrderPartslistPos component in requirement.Item.Components)
                {
                    tempPos = AddPlanningMRPos(databaseApp, consumption.PlanningMRCons, tempPos, null, component);
                }
            }

            List<ConsumptionModel> byRequierementsToDelete =
                byRequirements
                .Where(c =>
                        c.PlanningMRCons.EstimatedQuantityUOM == 0 // not delete consumption based elements
                        && !newByRequirements.Select(x => x.PlanningMRCons.ConsumptionDate).Contains(c.PlanningMRCons.ConsumptionDate))
                .ToList();
            foreach (var forDelete in byRequierementsToDelete)
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

        #region Fulfillment

        public MRPResult DoFulfillmentProduction(DatabaseApp databaseApp,ACProdOrderManager prodOrderManager, PlanningMR currentPlanningMR, MRPResult mRPResult)
        {
            mRPResult.PlanningPosition = GetPlanningPositions(databaseApp, currentPlanningMR);

            List<ConsumptionModel> itemsForProduction = mRPResult.PlanningPosition
                .Where(c => c.SelectedRecipeSource != null && c.SelectedRecipeSource.IsEnabled)
                .OrderBy(c => c.PlanningMRCons.ConsumptionDate)
                .ToList();

            foreach(ConsumptionModel consumptionModel in itemsForProduction)
            {
                string prodOrderNo = Root.NoManager.GetNewNo(Database, typeof(ProdOrder), ProdOrder.NoColumnName, ProdOrder.FormatNewNo, this);
                ProdOrder prodOrder = ProdOrder.NewACObject(databaseApp,null, prodOrderNo);

                ProdOrderPartslist prodOrderPartslist = null;
                Msg msg = prodOrderManager.PartslistAdd(databaseApp, prodOrder, consumptionModel.PlanningMRCons.DefaultPartslist, 1, consumptionModel.PlanningMRCons.RequiredQuantityUOM, out prodOrderPartslist);

                if(msg == null || msg.IsSucceded())
                {
                    prodOrderPartslist.TargetDeliveryDate = consumptionModel.PlanningMRCons.ConsumptionDate;
                    AddPartslistProposalAndPos(databaseApp, prodOrderPartslist, consumptionModel.PlanningMRCons);

                    List<ProdOrderPartslist> expandedPartslists = ExpandProdOrderPartslist(databaseApp, prodOrderManager, prodOrderPartslist);
                    foreach(ProdOrderPartslist expandedPartslist in expandedPartslists)
                    {
                        AddPartslistProposalAndPos(databaseApp, expandedPartslist, consumptionModel.PlanningMRCons);
                    }
                }
                else
                {
                    mRPResult.SaveMessage.AddDetailMessage(msg);    
                }

                MsgWithDetails saveResult = databaseApp.ACSaveChanges();
                if(saveResult != null && !saveResult.IsSucceded())
                {
                    mRPResult.SaveMessage.AddDetailMessage(saveResult);
                }
            }

            return mRPResult;
        }

        private List<ProdOrderPartslist> ExpandProdOrderPartslist(DatabaseApp databaseApp, ACProdOrderManager prodOrderManager, ProdOrderPartslist prodOrderPartslist)
        {
            List<ProdOrderPartslist> expandedPartslists = new List<ProdOrderPartslist>();

            ProdOrder prodOrder = prodOrderPartslist.ProdOrder;
            // 1.0 make BOM - create partslists
            double treeQuantityRatio = prodOrderPartslist.TargetQuantity / prodOrderPartslist.Partslist.TargetQuantityUOM;
            PartslistExpand rootPartslistExpand = new PartslistExpand(prodOrderPartslist.Partslist, 1, treeQuantityRatio);
            rootPartslistExpand.IsChecked = true;
            rootPartslistExpand.LoadTree();

            // 2.0 Extract suggestion
            List<ExpandResult> treeResult = rootPartslistExpand.BuildTreeList();
            treeResult =
                treeResult
                .Where(x =>
                    x.Item.IsChecked
                    && x.Item.IsEnabled)
                .OrderByDescending(x => x.TreeVersion)
                .ToList();

            int sn = 0;
            foreach (ExpandResult expand in treeResult)
            {
                sn++;
                PartslistExpand partslistExpand = expand.Item as PartslistExpand;
                ProdOrderPartslist pl = prodOrder.ProdOrderPartslist_ProdOrder.FirstOrDefault(c => c.PartslistID == partslistExpand.Partslist.PartslistID);
                if (pl == null)
                {
                    prodOrderManager.PartslistAdd(databaseApp, prodOrder, partslistExpand.Partslist, sn, partslistExpand.TargetQuantityUOM, out pl);
                    if (pl != null)
                    {
                        pl.Sequence = sn;
                        pl.TargetDeliveryDate = prodOrderPartslist.TargetDeliveryDate;
                        expandedPartslists.Add(pl);
                    }
                }
            }
            prodOrderManager.ConnectSourceProdOrderPartslist(prodOrder);
            prodOrderManager.CorrectSortOrder(prodOrder);

            return expandedPartslists;
        }


        private void AddPartslistProposalAndPos(DatabaseApp databaseApp, ProdOrderPartslist prodOrderPartslist, PlanningMRCons planningMRCons)
        {
            PlanningMRProposal planningMRProposal = PlanningMRProposal.NewACObject(databaseApp, null);
            planningMRProposal.PlanningMR = planningMRCons.PlanningMR;
            planningMRProposal.ProdOrder = prodOrderPartslist.ProdOrder;
            planningMRProposal.ProdOrderPartslist = prodOrderPartslist;
            planningMRCons.PlanningMR.PlanningMRProposal_PlanningMR.Add(planningMRProposal);

            PlanningMRPos lastPlanningPos = GetLastPlanningMRPos(databaseApp, planningMRCons, planningMRCons.ConsumptionDate);
            if (lastPlanningPos == null)
            {
                lastPlanningPos = NewInitialPlanningMRPos(databaseApp, planningMRCons, planningMRCons.ConsumptionDate);
                planningMRCons.PlanningMRPos_PlanningMRCons.Add(lastPlanningPos);
            }

            PlanningMRPos planningMRPos = PlanningMRPos.NewACObject(databaseApp, null);
            planningMRPos.PlanningMRCons = planningMRCons;
            
            planningMRPos.StoreQuantityUOM = lastPlanningPos.StoreQuantityUOM + prodOrderPartslist.TargetQuantity;
            planningMRPos.PlanningMRProposal = planningMRProposal;
            planningMRPos.ProdOrderPartslist = prodOrderPartslist;
            planningMRPos.ExpectedBookingDate = prodOrderPartslist.TargetDeliveryDate;
            planningMRCons.PlanningMRPos_PlanningMRCons.Add(planningMRPos);
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
                case MRPPlanningPhaseEnum.FulfillmentProduction:
                    break;
                case MRPPlanningPhaseEnum.FulfillmentInOrder:
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
