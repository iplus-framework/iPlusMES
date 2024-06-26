using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'SchedulingForecastManager'}de{'SchedulingForecastManager'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class SchedulingForecastManager : PARole
    {
        #region constants
        public const string ClassName = "SchedulingForecastManager";

        public const string Const_UpdateBatchPlanSQLName = @"SchedulingForecastManager-UpdateBatchPlan.sql";
        public const double Const_BatchQuantityFactor = 0.2;

        public const string C_DefaultServiceACIdentifier = "SchedulingForecastManager";
        #endregion

        #region Configuration
        protected ACPropertyConfigValue<SchedulingForecastModeEnum> _SchedulingForecastMode;
        [ACPropertyConfig("en{'Forecast Mode'}de{'Forecast Mode'}")]
        public virtual SchedulingForecastModeEnum SchedulingForecastMode
        {
            get
            {
                return _SchedulingForecastMode.ValueT;
            }
            set
            {
                _SchedulingForecastMode.ValueT = value;
            }
        }
        #endregion

        #region c´tors
        public SchedulingForecastManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _SchedulingForecastMode = new ACPropertyConfigValue<SchedulingForecastModeEnum>(this, "SchedulingForecastMode", SchedulingForecastModeEnum.AverageWithoutDeviateValues);
        }
        #endregion

        #region Manager instancing static methods
        public static SchedulingForecastManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<SchedulingForecastManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<SchedulingForecastManager> ACRefToServiceInstance(ACComponent requester)
        {
            SchedulingForecastManager serviceInstance = GetServiceInstance(requester);
            if (serviceInstance != null)
                return new ACRef<SchedulingForecastManager>(serviceInstance, requester);
            return null;
        }
        #endregion

        #region Scheduling
        [ACMethodInfo("BackwardScheduling", "en{'BackwardScheduling'}de{'BackwardScheduling'}", 9999)]
        public MsgWithDetails BackwardScheduling(DatabaseApp databaseApp, Guid mdSchedulingGroupID, string updateName, DateTime endTime)
        {
            return Scheduling(databaseApp, mdSchedulingGroupID, updateName, endTime, true);
        }

        [ACMethodInfo("ForwardScheduling", "en{'ForwardScheduling'}de{'ForwardScheduling'}", 9999)]

        public MsgWithDetails ForwardScheduling(DatabaseApp databaseApp, Guid mdSchedulingGroupID, string updateName, DateTime startTime)
        {
            return Scheduling(databaseApp, mdSchedulingGroupID, updateName, startTime, false);
        }

        private MsgWithDetails Scheduling(DatabaseApp databaseApp, Guid mdSchedulingGroupID, string updateName, DateTime beginTime, bool isBackward)
        {
            MsgWithDetails message = new MsgWithDetails();
            try
            {
                ACProdOrderManager aCProdOrderManager = ACProdOrderManager.GetServiceInstance(this);
                List<ProdOrderBatchPlan> batchPlans = aCProdOrderManager.GetProductionLinieBatchPlans(
                    databaseApp,
                    mdSchedulingGroupID,
                    GlobalApp.BatchPlanState.Created,
                    GlobalApp.BatchPlanState.Paused,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null).ToList();
                batchPlans = batchPlans.Where(c => c.IsSelected).ToList();
                if (isBackward)
                    batchPlans = batchPlans.OrderByDescending(c => c.ScheduledOrder).ToList();

                ProdOrderBatchPlan firstBatchPlan = null;
                ProdOrderBatchPlan prevBatchPlan = null;
                foreach (ProdOrderBatchPlan batchPlan in batchPlans)
                {
                    string testPartslistNo = batchPlan.ProdOrderPartslist.Partslist.PartslistNo;
                    SchedulingDurationAVG duration = GetBPDuration(databaseApp, mdSchedulingGroupID, batchPlan.ProdOrderPartslist.PartslistID ?? Guid.Empty, batchPlan.BatchTargetCount, batchPlan.BatchSize);
                    if ((duration.DurationSecAVG ?? 0) == 0)
                    {
                        IACConfig defaultDurationSecAVGConfig = GetConfig(ProdOrderBatchPlan.C_DurationSecAVG,
                            batchPlan.IplusVBiACClassWF,
                            batchPlan.VBiACClassWF,
                            batchPlan.MaterialWFACClassMethod.MaterialWFID,
                            batchPlan.ProdOrderPartslist.Partslist,
                            batchPlan.ProdOrderPartslist);
                        if (defaultDurationSecAVGConfig != null)
                            duration.DurationSecAVG = (int)defaultDurationSecAVGConfig.Value;
                    }

                    if ((duration.StartOffsetSecAVG ?? 0) == 0)
                    {
                        IACConfig defaultStartOffsetSecAVGConfig = GetConfig(ProdOrderBatchPlan.C_StartOffsetSecAVG,
                              batchPlan.IplusVBiACClassWF,
                            batchPlan.VBiACClassWF,
                            batchPlan.MaterialWFACClassMethod.MaterialWFID,
                            batchPlan.ProdOrderPartslist.Partslist,
                            batchPlan.ProdOrderPartslist);
                        if (defaultStartOffsetSecAVGConfig != null)
                            duration.StartOffsetSecAVG = (int)defaultStartOffsetSecAVGConfig.Value;
                    }

                    bool durationIsValid = duration.DurationSecAVG > 0;
                    if (!durationIsValid)
                    {
                        var method = batchPlan.ProdOrderPartslist.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                        gip.mes.datamodel.ACClassMethod mth = databaseApp.ACClassMethod.FirstOrDefault(c => c.ACClassMethodID == method.MaterialWFACClassMethod.ACClassMethodID);
                        Msg msgInvalidDuration = new Msg(this, eMsgLevel.Error, ClassName, "Scheduling", 168, "Error50351",
                            batchPlan.ProdOrderPartslist.ProdOrder.ProgramNo,
                            batchPlan.Sequence,
                            batchPlan.ProdOrderPartslistPos.Material.MaterialNo,
                            batchPlan.ProdOrderPartslistPos.Material.MaterialName1,

                            batchPlan.ProdOrderPartslist.Partslist.PartslistNo,
                            batchPlan.ProdOrderPartslist.Partslist.PartslistName,
                            mth != null ? mth.ACIdentifier : "");
                        message.AddDetailMessage(msgInvalidDuration);
                    }
                    if (firstBatchPlan == null)
                    {
                        firstBatchPlan = batchPlan;
                        if (isBackward)
                            batchPlan.ScheduledEndDate = beginTime;
                        else
                            batchPlan.ScheduledStartDate = beginTime;
                    }
                    else
                    {
                        if (isBackward)
                            batchPlan.ScheduledEndDate = prevBatchPlan.ScheduledStartDate.Value;
                        else
                            batchPlan.ScheduledStartDate = prevBatchPlan.ScheduledEndDate.Value;


                        if (duration.StartOffsetSecAVG != null)
                            if (isBackward)
                                batchPlan.ScheduledEndDate = batchPlan.ScheduledEndDate.Value.AddSeconds(duration.StartOffsetSecAVG.Value);
                            else
                                batchPlan.ScheduledStartDate = batchPlan.ScheduledStartDate.Value.AddSeconds(-duration.StartOffsetSecAVG.Value);
                    }
                    double durationValue = 0;
                    if (durationIsValid)
                        durationValue = duration.DurationSecAVG.Value;
                    if (isBackward)
                        batchPlan.ScheduledStartDate = batchPlan.ScheduledEndDate.Value.AddSeconds(-durationValue);
                    else
                        batchPlan.ScheduledEndDate = batchPlan.ScheduledStartDate.Value.AddSeconds(durationValue);

                    prevBatchPlan = batchPlan;
                    batchPlan.UpdateName = updateName;
                    batchPlan.UpdateDate = DateTime.Now;
                }
                Msg saveMessage = databaseApp.ACSaveChanges();
                message.MessageLevel = eMsgLevel.Info;
                if (saveMessage != null)
                    message.AddDetailMessage(saveMessage);
            }
            catch (Exception e)
            {
                string methodName = isBackward ? "BackwardScheduling" : "ForwardScheduling";
                message = new MsgWithDetails() { Message = e.Message, Source = this.GetACUrl(), MessageLevel = eMsgLevel.Exception };
                Root.Messages.LogException(this.GetACUrl(), string.Format("PABatchPlanScheduler.{0}()", methodName), e.Message);
            }
            return message;
        }

        #endregion

        #region Calculation

        public SchedulingDurationAVG GetBPDuration(DatabaseApp databaseApp, Guid mdSchedulingGroupID, Guid partslistID, int batchTargetCount, double batchSize)
        {
            List<SchedulingBatchPlanDuration> statistic = new List<SchedulingBatchPlanDuration>();

            switch (SchedulingForecastMode)
            {
                case SchedulingForecastModeEnum.LinearAverage:
                    statistic = GetBPStatistic_LinearAverage(databaseApp, mdSchedulingGroupID, partslistID, batchTargetCount, batchSize);
                    break;
                case SchedulingForecastModeEnum.AverageWithoutDeviateValues:
                    statistic = GetBPStatistic_AverageWithoutDeviateValues(databaseApp, mdSchedulingGroupID, partslistID, batchTargetCount, batchSize);
                    break;
                default:
                    break;
            }

            double? startOffsetSecAVG = null;
            double? durationSecAVG = null;


            if (statistic.Any(c => c.IncludeInCalc))
            {
                startOffsetSecAVG = statistic.Average(x => x.StartOffsetSecAVGPerUnit) * batchSize;
                durationSecAVG = statistic.Average(x => x.DurationSecAVGPerUnit) * batchSize;
            }

            return new SchedulingDurationAVG() { DurationSecAVG = durationSecAVG, StartOffsetSecAVG = startOffsetSecAVG };
        }


        #region Calculation -> LinearAverage
        public List<SchedulingBatchPlanDuration> GetBPStatistic_LinearAverage(DatabaseApp databaseApp, Guid mdSchedulingGroupID, Guid partslistID, int batchTargetCount, double batchSize)
        {
            List<ProdOrderBatchPlan> bpStatistic = new List<ProdOrderBatchPlan>();

            double minBatchSize = batchSize - batchSize * Const_BatchQuantityFactor;
            double maxBatchSize = batchSize + batchSize * Const_BatchQuantityFactor;

            // Try 1: Similar batch size x1 & this year
            bpStatistic.AddRange(QueryProdOrderBatchPlan(databaseApp, 1, minBatchSize, maxBatchSize, mdSchedulingGroupID, partslistID));
            if (!bpStatistic.Any() || bpStatistic.Count() < 10)
            {
                minBatchSize = batchSize - batchSize * (Const_BatchQuantityFactor * 2);
                maxBatchSize = batchSize + batchSize * (Const_BatchQuantityFactor * 2);
                bpStatistic.Clear();
                // Try 2: Similar batch size x2 & entire past year
                bpStatistic.AddRange(QueryProdOrderBatchPlan(databaseApp, 1, minBatchSize, maxBatchSize, mdSchedulingGroupID, partslistID));
            }

            if (!bpStatistic.Any() || bpStatistic.Count() < 10)
            {
                minBatchSize = batchSize - batchSize * (Const_BatchQuantityFactor);
                maxBatchSize = batchSize + batchSize * (Const_BatchQuantityFactor);
                // Try 2: Similar batch size x1 & entire past two years
                bpStatistic.AddRange(QueryProdOrderBatchPlan(databaseApp, 2, minBatchSize, maxBatchSize, mdSchedulingGroupID, partslistID));
            }

            if (!bpStatistic.Any() || bpStatistic.Count() < 10)
            {
                minBatchSize = batchSize - batchSize * (Const_BatchQuantityFactor * 1);
                maxBatchSize = batchSize + batchSize * (Const_BatchQuantityFactor * 1);
                // Try 2: Similar batch size x2 & entire past two years
                bpStatistic.AddRange(QueryProdOrderBatchPlan(databaseApp, 2, minBatchSize, maxBatchSize, mdSchedulingGroupID, partslistID));
            }
            if (!bpStatistic.Any())
            {
                bpStatistic.AddRange(QueryProdOrderBatchPlan(databaseApp, 2, null, null, mdSchedulingGroupID, partslistID));
            }
            // Build list
            List<SchedulingBatchPlanDuration> statistic =
                    bpStatistic
                    .Select(c =>
                                new SchedulingBatchPlanDuration()
                                {
                                    ProdOrderBatchPlanID = c.ProdOrderBatchPlanID,
                                    BatchTargetCount = c.BatchTargetCount,
                                    BatchSize = c.BatchSize,
                                    StartOffsetSecAVG = c.StartOffsetSecAVG,
                                    DurationSecAVG = c.DurationSecAVG
                                })
                    .ToList();

            // StartOffsetSecAVG and DurationSecAVG as average per batch
            foreach (var item in statistic)
            {
                if (item.StartOffsetSecAVG != null)
                    item.StartOffsetSecAVGPerUnit = item.StartOffsetSecAVG / item.BatchSize;

                if (item.DurationSecAVG != null)
                    item.DurationSecAVGPerUnit = item.DurationSecAVG / item.BatchSize;

                item.IncludeInCalc = true;
            }
            return statistic;
        }

        #endregion


        #region Calculation -> AverageWithoutDeviateValues

        public List<SchedulingBatchPlanDuration> GetBPStatistic_AverageWithoutDeviateValues(DatabaseApp databaseApp, Guid mdSchedulingGroupID, Guid partslistID, int batchTargetCount, double batchSize)
        {
            List<SchedulingBatchPlanDuration> statistic = GetBPStatistic_LinearAverage(databaseApp, mdSchedulingGroupID, partslistID, batchTargetCount, batchSize);
            if (statistic.Any())
            {
                statistic.ForEach(x => x.IncludeInCalc = false);

                List<double> includedDurationSecAVGPerUnitValues = statistic.Where(c => c.DurationSecAVGPerUnit != null).Select(c => c.DurationSecAVGPerUnit.Value).Distinct().ToList();
                List<double> includedStartOffsetSecAVGPerUnitValues = statistic.Where(c => c.StartOffsetSecAVGPerUnit != null).Select(c => c.StartOffsetSecAVGPerUnit.Value).Distinct().ToList();

                if (includedDurationSecAVGPerUnitValues.Any() && includedDurationSecAVGPerUnitValues.Count() > 1)
                    includedDurationSecAVGPerUnitValues = FilterList(includedDurationSecAVGPerUnitValues);
                if (includedStartOffsetSecAVGPerUnitValues.Any() && includedStartOffsetSecAVGPerUnitValues.Count() > 1)
                    includedStartOffsetSecAVGPerUnitValues = FilterList(includedStartOffsetSecAVGPerUnitValues);

                foreach (var item in statistic)
                {
                    bool validDuration = item.DurationSecAVGPerUnit != null && includedDurationSecAVGPerUnitValues.Contains(item.DurationSecAVGPerUnit.Value);
                    bool validOffset = item.StartOffsetSecAVGPerUnit != null && includedStartOffsetSecAVGPerUnitValues.Contains(item.StartOffsetSecAVGPerUnit.Value);
                    item.IncludeInCalc = validDuration;
                }
            }
            return statistic;
        }

        public List<double> FilterList(List<double> inputList)
        {
            double mean = inputList.Average();
            double sd = StdDev(inputList, true);
            List<double> finalList = inputList.Where(c => c > (mean - 2 * sd)).ToList();
            finalList = finalList.Where(c => c < (mean + 2 * sd)).ToList();
            return finalList;
        }

        public double StdDev(IEnumerable<double> values, bool as_sample)
        {
            double mean = values.Sum() / values.Count();
            List<double> squares_query = values.Select(value => (value - mean) * (value - mean)).ToList();
            double sum_of_squares = squares_query.Sum();

            if (as_sample)
                return Math.Sqrt(sum_of_squares / (values.Count() - 1));
            else
                return Math.Sqrt(sum_of_squares / values.Count());
        }

        #endregion

        public IQueryable<ProdOrderBatchPlan> QueryProdOrderBatchPlan(DatabaseApp databaseApp, int yearShift, double? minBatchSize, double? maxBatchSize, Guid mdSchedulingGroupID, Guid partslistID)
        {
            DateTime endTime = DateTime.Now;
            DateTime startTime = endTime.AddYears(-yearShift);

            var queryTest = databaseApp
                .ProdOrderBatchPlan
                .Where(c =>
                    c.BatchSize > 0
                    && c.BatchTargetCount > 0
                    //&& c.DurationSecAVG > 0
                    && c.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Any(x => x.MDSchedulingGroupID == mdSchedulingGroupID)
                    //&& c.ProdOrderBatch_ProdOrderBatchPlan.Any(x => x.InsertDate >= startTime && x.InsertDate < endTime)
                    && c.ProdOrderPartslist.PartslistID == partslistID
                    //&& c.BatchSize >= minBatchSize
                    //&& c.BatchSize <= maxBatchSize
                    )
                .Select(c => new
                {
                    PlMaterialNo = c.ProdOrderPartslist.Partslist.Material.MaterialNo,
                    PlMaterialName = c.ProdOrderPartslist.Partslist.Material.MaterialName1,
                    c.BatchSize,
                    c.BatchTargetCount,
                    c.DurationSecAVG,
                    HavingBatch = c.ProdOrderBatch_ProdOrderBatchPlan.Any(),
                    HavingBatchInTime = c.ProdOrderBatch_ProdOrderBatchPlan.Any(x => x.InsertDate >= startTime && x.InsertDate < endTime),
                    c.ProdOrderPartslist.PartslistID
                })
                .ToArray();

            var testSasa = queryTest;

            return databaseApp
                .ProdOrderBatchPlan
                .Where(c =>
                    c.BatchSize > 0
                    && c.BatchTargetCount > 0
                    //&& c.DurationSecAVG > 0
                    && c.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Any(x => x.MDSchedulingGroupID == mdSchedulingGroupID)
                    && c.ProdOrderBatch_ProdOrderBatchPlan.Any(x => x.InsertDate >= startTime && x.InsertDate < endTime)
                    && c.ProdOrderPartslist.PartslistID == partslistID
                    && (minBatchSize == null || c.BatchSize >= minBatchSize)
                    && (maxBatchSize == null || c.BatchSize <= maxBatchSize));
        }

        #endregion

        #region Update BatchPlan Durations

        public void UpdateAllBatchPlanDurations(string username)
        {
            UpdateBatchPlanDuration(null, null, null, null, username);
        }

        public void UpdateBatchPlanDuration(Guid? prodOrderPartslistID, Guid? prodOrderBatchPlanID, string username)
        {
            UpdateBatchPlanDuration(null, null, prodOrderPartslistID, prodOrderBatchPlanID, username);
        }

        public void UpdateBatchPlanDuration(string linie, string partslistNo, Guid? prodOrderPartslistID, Guid? prodOrderBatchPlanID, string username)
        {
            try
            {
                //declare @linie varchar(20);
                //declare @prodOrderBatchPlanID uniqueidentifier;
                //declare @prodOrderPartslistID uniqueidentifier;
                //declare @partslistNo varchar(20);
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    SqlParameter linieParam = new SqlParameter();
                    linieParam.ParameterName = "linie";
                    if (linie != null)
                        linieParam.Value = linie;
                    else
                        linieParam.Value = DBNull.Value;

                    SqlParameter prodOrderBatchPlanIDParam = new SqlParameter();
                    prodOrderBatchPlanIDParam.ParameterName = "prodOrderBatchPlanID";
                    if (prodOrderBatchPlanID != null)
                        prodOrderBatchPlanIDParam.Value = prodOrderBatchPlanID;
                    else
                        prodOrderBatchPlanIDParam.Value = DBNull.Value;


                    SqlParameter prodOrderPartslistIDParam = new SqlParameter();
                    prodOrderPartslistIDParam.ParameterName = "prodOrderPartslistID";
                    if (prodOrderPartslistID != null)
                        prodOrderPartslistIDParam.Value = prodOrderPartslistID;
                    else
                        prodOrderPartslistIDParam.Value = DBNull.Value;


                    SqlParameter partslistNoParam = new SqlParameter();
                    partslistNoParam.ParameterName = "partslistNo";
                    if (partslistNo != null)
                        partslistNoParam.Value = partslistNo;
                    else
                        partslistNoParam.Value = DBNull.Value;

                    SqlParameter usernameParam = new SqlParameter();
                    usernameParam.ParameterName = "username";
                    usernameParam.Value = username;

                    string updateBatchPlanDurationSQL = GetUpdateDurationSQL();
                    int cnt = databaseApp.ExecuteStoreCommand(updateBatchPlanDurationSQL, linieParam, partslistNoParam, prodOrderPartslistIDParam, prodOrderBatchPlanIDParam, usernameParam);
                }
            }
            catch (Exception ec)
            {
                Root.Messages.LogException(this.GetACUrl(), "UpdateBatchPlanDuration()", ec);
            }
        }

        public string GetUpdateDurationSQL()
        {
            return ReadResource(Const_UpdateBatchPlanSQLName);
        }

        public string ReadResource(string name)
        {
            // Determine path
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion

        #region Helpers
        private IACConfig GetConfig(string propertyName, gip.core.datamodel.ACClassWF currentACClassWF, gip.mes.datamodel.ACClassWF vbCurrentACClassWF, Guid? materialWFID, Partslist partslist, ProdOrderPartslist prodOrderPartslist)
        {
            IACConfig config = null;
            var configStores = GetCurrentConfigStores(currentACClassWF, vbCurrentACClassWF, materialWFID, partslist, prodOrderPartslist);
            foreach (var configStore in configStores)
            {
                foreach (var item in configStore.ConfigurationEntries)
                {
                    if (item.ConfigACUrl != null && item.ConfigACUrl.EndsWith(propertyName))
                    {
                        config = item;
                        break;
                    }
                }
                if (config != null)
                    break;
            }
            return config;
        }

        private List<IACConfigStore> GetCurrentConfigStores(gip.core.datamodel.ACClassWF currentACClassWF, gip.mes.datamodel.ACClassWF vbCurrentACClassWF, Guid? materialWFID, Partslist partslist, ProdOrderPartslist prodOrderPartslist)
        {
            List<IACConfigStore> configStores = new List<IACConfigStore>();
            if (prodOrderPartslist != null)
                configStores.Add(prodOrderPartslist);
            if (partslist != null)
                configStores.Add(partslist);
            ACProdOrderManager poManager = ACProdOrderManager.GetServiceInstance(this);
            MaterialWFConnection matWFConnection = poManager.GetMaterialWFConnection(vbCurrentACClassWF, materialWFID);
            configStores.Add(matWFConnection.MaterialWFACClassMethod);
            configStores.Add(currentACClassWF.ACClassMethod);
            if (currentACClassWF.RefPAACClassMethod != null)
                configStores.Add(currentACClassWF.RefPAACClassMethod);
            return configStores;
        }

        #endregion
    }
}
