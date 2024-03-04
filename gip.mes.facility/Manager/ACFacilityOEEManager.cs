using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.processapplication;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'OEE-Manager'}de{'OEE-Manager'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class ACFacilityOEEManager : PARole
    {
        #region c'tors
         public ACFacilityOEEManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public const string C_DefaultServiceACIdentifier = "FacilityOEEManager";
        public const double C_AvailabilityThreasholdForAutoCalc = 0.990;
        #endregion

        #region Attach / Detach
        public static ACFacilityOEEManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ACFacilityOEEManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<ACFacilityOEEManager> ACRefToServiceInstance(ACComponent requester)
        {
            ACFacilityOEEManager serviceInstance = GetServiceInstance(requester);
            if (serviceInstance != null)
                return new ACRef<ACFacilityOEEManager>(serviceInstance, requester);
            return null;
        }
        #endregion

        #region Methods
        public Msg GenerateTestOEEData(DatabaseApp dbApp, FacilityMaterial facilityMaterial)
        {
            Msg msg = null;
            if (facilityMaterial == null || dbApp == null)
                return msg;
            DateTime endDate = DateTime.Now;
            for (int i = 0; i < 500; i++)
            {
                Random random = new Random();
                int minutesOperation = random.Next(100, 1440);
                int minutesIdle = random.Next(0, 120);
                int minutesStandBy = random.Next(0, 15);
                int minutesScheduledBreak = random.Next(0, 15);
                int minutesUnscheduledBreak = random.Next(0, (int)((double)minutesOperation * 0.2));
                if (random.Next(0, 4) == 0) // 20% probability for a flewless production
                    minutesUnscheduledBreak = 0;
                int minutesRetooling = random.Next(0, 15);
                int minutesMaintenance = random.Next(0, 15);
                DateTime startDate = endDate.AddMinutes((minutesOperation + minutesStandBy + minutesScheduledBreak + minutesUnscheduledBreak + minutesRetooling + minutesMaintenance) * -1);

                FacilityMaterialOEE oeeEntry = FacilityMaterialOEE.NewACObject(dbApp, facilityMaterial);
                oeeEntry.ACProgramLogID = facilityMaterial.FacilityMaterialID;
                dbApp.FacilityMaterialOEE.AddObject(oeeEntry);
                oeeEntry.StartDate = startDate;
                oeeEntry.EndDate = endDate;
                oeeEntry.IdleTime = TimeSpan.FromMinutes(minutesIdle);
                oeeEntry.StandByTime = TimeSpan.FromMinutes(minutesStandBy);
                oeeEntry.OperationTime = TimeSpan.FromMinutes(minutesOperation);
                oeeEntry.ScheduledBreakTime = TimeSpan.FromMinutes(minutesScheduledBreak);
                oeeEntry.UnscheduledBreakTime = TimeSpan.FromMinutes(minutesUnscheduledBreak);
                oeeEntry.RetoolingTime = TimeSpan.FromMinutes(minutesRetooling);
                oeeEntry.MaintenanceTime = TimeSpan.FromMinutes(minutesMaintenance);

                if (facilityMaterial.ThroughputMax.HasValue && facilityMaterial.ThroughputMin.HasValue)
                {
                    int throughPut = random.Next((int)facilityMaterial.ThroughputMin, (int)facilityMaterial.ThroughputMax.Value);
                    oeeEntry.Quantity = throughPut * (minutesOperation / 60);
                    int scrapPerc = random.Next(0, 500);
                    if (scrapPerc > 0)
                        oeeEntry.QuantityScrap = oeeEntry.Quantity * scrapPerc * 0.0001;
                }
                oeeEntry.CalcOEE();
                msg = dbApp.ACSaveChanges();
                if (msg != null)
                    return msg;

                endDate = startDate.AddMinutes(minutesIdle * -1);
            }
            return msg;
        }

        public int DeleteTestOEEData(DatabaseApp dbApp, FacilityMaterial facilityMaterial)
        {
            if (facilityMaterial == null || dbApp == null)
                return -1;
            return dbApp.ExecuteStoreCommand("DELETE FROM FacilityMaterialOEE WHERE ACProgramLogID = {0}", facilityMaterial.FacilityMaterialID);
        }

        public Msg RecalcThroughputAverage(DatabaseApp dbApp, FacilityMaterial facilityMaterial, bool saveChanges = true, int countLatestEntries = 500, double availabilityThreshold = C_AvailabilityThreasholdForAutoCalc)
        {
            Msg msg = null;
            if (facilityMaterial == null || dbApp == null)
                return msg;

            try
            {
                double avgThroughPut =
                    dbApp.FacilityMaterialOEE
                    .Where(c => c.FacilityMaterialID == facilityMaterial.FacilityMaterialID
                                && c.AvailabilityOEE > availabilityThreshold
                                && c.Throughput > 0.00001
                                && (!facilityMaterial.ThroughputMax.HasValue || c.Throughput <= facilityMaterial.ThroughputMax)
                                && (!facilityMaterial.ThroughputMin.HasValue || c.Throughput >= facilityMaterial.ThroughputMin))
                    .OrderByDescending(c => c.EndDate)
                    .Take(countLatestEntries)
                    .Select(c => c.Throughput)
                    .Average();
                //.GroupBy(g => 1)
                //.Select(g => new FacilityOEEAvg()
                //{
                //    AvailabilityOEE = g.Average(c => c.AvailabilityOEE),
                //    PerformanceOEE = g.Average(c => c.PerformanceOEE),
                //    QualityOEE = g.Average(c => c.QualityOEE),
                //    TotalOEE = g.Average(c => c.TotalOEE)
                //})
                //.FirstOrDefault();
                facilityMaterial.Throughput = avgThroughPut;
                if (saveChanges)
                    msg = dbApp.ACSaveChanges();
            }
            catch (Exception e)
            {
                msg = new Msg(eMsgLevel.Exception, e.Message);
                return msg;
            }

            return msg;
        }

        public Msg RecalcThroughputAndOEE(DatabaseApp dbApp, FacilityMaterial facilityMaterial, DateTime? from, DateTime? to)
        {
            Msg msg = null;
            if (facilityMaterial == null || dbApp == null)
                return msg;

            dbApp.FacilityMaterialOEE.Where(c => c.FacilityMaterial.FacilityMaterialID == facilityMaterial.FacilityMaterialID 
                                                && (!from.HasValue || c.EndDate >= from)
                                                && (!to.HasValue || c.EndDate <= to))
                                    .ToList()
                                    .ForEach(c => c.CalcOEE());
            msg = dbApp.ACSaveChanges();
            return msg;
        }

        public FacilityOEEAvg[] GetOEEEntries(DatabaseApp dbApp, Facility facility, DateTime from, DateTime to)
        {
            if (dbApp == null || facility == null)
                return new FacilityOEEAvg[] { };
            FacilityOEEAvg[] resultsInTotalRange =
            dbApp.FacilityMaterialOEE.Where(c => c.FacilityMaterial.FacilityID == facility.FacilityID && c.EndDate >= from && c.EndDate <= to)
                .OrderBy(c => c.EndDate)
                .Select(c => new FacilityOEEAvg()
                {
                    AvailabilityOEE = c.AvailabilityOEE,
                    PerformanceOEE = c.PerformanceOEE,
                    QualityOEE = c.QualityOEE,
                    TotalOEE = c.TotalOEE,
                    EndDate = c.EndDate,
                    StartDate = c.StartDate
                })
                .ToArray();
            return resultsInTotalRange;
        }

        public FacilityOEEAvg GetOEE(DatabaseApp dbApp, Facility facility, DateTime from, DateTime to)
        {
            if (dbApp == null || facility == null)
                return null;

            FacilityOEEAvg[] resultsInTotalRange = GetOEEEntries(dbApp, facility, from, to);
            return new FacilityOEEAvg(resultsInTotalRange) { Facility = facility };
        }


        #region Methods for ACProdOrderManager

        public void CalculateOEE(DatabaseApp databaseApp, ProdOrderPartslist prodOrderPartslist, IEnumerable<ProdOrderPartslistPos> component,
            ProdOrderPartslistPos lastIntermediatePos, ProdOrderPartslist finalPartslist, ProdOrderPartslistPos lastIntermediatePosOfFinalPL,
            List<object> itemsForPostProcessing)
        {
            using (Database database = new core.datamodel.Database())
            {
                Guid[] programIDs = databaseApp.OrderLog
                                            .Where(c => ((c.ProdOrderPartslistPos != null
                                                                && c.ProdOrderPartslistPos.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID)
                                                            || (c.ProdOrderPartslistPosRelation != null
                                                                && c.ProdOrderPartslistPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID)
                                                          )
                                                        && c.VBiACProgramLog.ACProgramLog1_ParentACProgramLog == null)
                                           .Select(x => x.VBiACProgramLog.ACProgramID)
                                           .Distinct()
                                           .ToArray();
                if (programIDs == null || !programIDs.Any())
                    return;
                List<ACPropertyLogSumOfProgram> machineDurations = new List<ACPropertyLogSumOfProgram>();
                foreach (Guid programID in programIDs)
                {
                    IEnumerable<ACPropertyLogSumOfProgram> sumOfProgram = gip.core.datamodel.ACPropertyLog.GetSummarizedDurationsOfProgram(database, programID, new string[] { GlobalProcApp.AvailabilityStatePropName });
                    if (sumOfProgram != null && sumOfProgram.Any())
                    {
                        machineDurations.AddRange(sumOfProgram);
                    }
                }

                List<FacilityMaterialOEE> oeeEntriesOfEntireOrder = new List<FacilityMaterialOEE>();
                foreach (var machine in machineDurations.GroupBy(c => c.ACClass))
                {
                    Guid machineACClassID = machine.Key.ACClassID;
                    Facility facility = databaseApp.Facility.Where(c => c.VBiFacilityACClassID.HasValue && c.VBiFacilityACClassID == machineACClassID).FirstOrDefault();
                    if (facility == null)
                        continue;

                    FacilityMaterial facilityMaterial = databaseApp.FacilityMaterial
                                                                    .Include(c => c.Facility.VBiFacilityACClass)
                                                                    .Where(c => c.FacilityID == facility.FacilityID && c.MaterialID == prodOrderPartslist.Partslist.MaterialID)
                                                                    .FirstOrDefault();
                    if (facilityMaterial == null)
                    {
                        facilityMaterial = FacilityMaterial.NewACObject(databaseApp, facility);
                        // TODO Material could theoretically also be a component instead
                        facilityMaterial.Material = prodOrderPartslist.Partslist.Material;
                        facilityMaterial.Throughput = 0;
                        facilityMaterial.ThroughputAuto = 1;
                        databaseApp.FacilityMaterial.AddObject(facilityMaterial);
                    }

                    var propLogSumsByProgramLog = machine.SelectMany(c => c.Sum)
                                            .Where(c => c.ProgramLog != null)
                                            .GroupBy(c => c.ProgramLog);
                    foreach (var propLogSumByProgramLog in propLogSumsByProgramLog)
                    {
                        FacilityMaterialOEE oeeEntry = null;
                        if (facilityMaterial.EntityState != EntityState.Added)
                            oeeEntry = databaseApp.FacilityMaterialOEE.Where(c => c.ACProgramLogID == propLogSumByProgramLog.Key.ACProgramLogID && c.FacilityMaterialID == facilityMaterial.FacilityMaterialID).FirstOrDefault();
                        if (oeeEntry == null)
                        {
                            oeeEntry = FacilityMaterialOEE.NewACObject(databaseApp, facilityMaterial);
                            oeeEntry.ACProgramLogID = propLogSumByProgramLog.Key.ACProgramLogID;
                            databaseApp.FacilityMaterialOEE.AddObject(oeeEntry);
                        }
                        else
                        {
                            // If Entry already exists, rest to Zero for recalculation
                            oeeEntry.Quantity = 0;
                            oeeEntry.QuantityScrap = 0;
                            oeeEntry.Throughput = 0;
                            oeeEntry.PerformanceOEE = 0;
                            oeeEntry.QualityOEE = 0;
                            oeeEntry.AvailabilityOEE = 0;
                        }
                        oeeEntriesOfEntireOrder.Add(oeeEntry);

                        DateTime minStartDate = propLogSumByProgramLog.Key.StartDateDST.HasValue ? propLogSumByProgramLog.Key.StartDateDST.Value : DateTime.MaxValue;
                        DateTime maxEndDate = propLogSumByProgramLog.Key.EndDateDST.HasValue ? propLogSumByProgramLog.Key.EndDateDST.Value : DateTime.MinValue;
                        TimeSpan idleTime = TimeSpan.Zero;
                        TimeSpan maintenanceTime = TimeSpan.Zero;
                        TimeSpan retoolingTime = TimeSpan.Zero;
                        TimeSpan scheduledBreakTime = TimeSpan.Zero;
                        TimeSpan unscheduledBreakTime = TimeSpan.Zero;
                        TimeSpan standByTime = TimeSpan.Zero;
                        TimeSpan operationTime = TimeSpan.Zero;
                        int countPropertyLogs = 0;
                        foreach (var propLogSum in propLogSumByProgramLog)
                        {
                            countPropertyLogs++;
                            if (propLogSum.StartDate.HasValue && propLogSum.StartDate.Value < minStartDate)
                                minStartDate = propLogSum.StartDate.Value;
                            if (propLogSum.EndDate.HasValue && propLogSum.EndDate.Value > maxEndDate)
                                maxEndDate = propLogSum.EndDate.Value;
                            AvailabilityState availabilityState = (AvailabilityState)propLogSum.PropertyValue;
                            if (availabilityState == AvailabilityState.Idle)
                                idleTime += propLogSum.Duration;
                            else if (availabilityState == AvailabilityState.Standby)
                                standByTime += propLogSum.Duration;
                            else if (availabilityState == AvailabilityState.InOperation)
                                operationTime += propLogSum.Duration;
                            else if (availabilityState == AvailabilityState.ScheduledBreak)
                                scheduledBreakTime += propLogSum.Duration;
                            else if (availabilityState == AvailabilityState.UnscheduledBreak)
                                unscheduledBreakTime += propLogSum.Duration;
                            else if (availabilityState == AvailabilityState.Retooling)
                                retoolingTime += propLogSum.Duration;
                            else if (availabilityState == AvailabilityState.Maintenance)
                                maintenanceTime += propLogSum.Duration;
                        }
                        if (countPropertyLogs == 0
                            || (   idleTime == TimeSpan.Zero
                                && operationTime == TimeSpan.Zero
                                && scheduledBreakTime == TimeSpan.Zero
                                && unscheduledBreakTime == TimeSpan.Zero
                                && retoolingTime == TimeSpan.Zero
                                && maintenanceTime == TimeSpan.Zero))
                        {

                            TimeSpan totalDuration = maxEndDate - minStartDate;
                            if (totalDuration > standByTime)
                                operationTime = totalDuration - standByTime;
                            else
                            {
                                operationTime = standByTime;
                                standByTime = TimeSpan.Zero;
                            }
                        }

                        oeeEntry.StartDate = minStartDate;
                        oeeEntry.EndDate = maxEndDate;
                        oeeEntry.IdleTime = idleTime;
                        oeeEntry.StandByTime = standByTime;
                        oeeEntry.OperationTime = operationTime;
                        oeeEntry.ScheduledBreakTime = scheduledBreakTime;
                        oeeEntry.UnscheduledBreakTime = unscheduledBreakTime;
                        oeeEntry.RetoolingTime = retoolingTime;
                        oeeEntry.MaintenanceTime = maintenanceTime;

                        //#region DEBUG
                        //{
                        //    Random random = new Random();
                        //    PerformanceOEE = ((double)random.Next(1, 100)) / 100.0;
                        //}
                        //#endregion

                        /// Calculation Quantities and Scrap:
                        maxEndDate = maxEndDate.AddSeconds(1);
                        bool quantityFound = false;
                        /// 1. Check whether there is an acquisition booking in the group (relation to "ProdOrderPartslistPos" in "Booking") -> Use these quantities for determination
                        FacilityBookingCharge[] postingsOnThisMachine =
                        databaseApp.FacilityBookingCharge.Include(c => c.MDMovementReason)
                                                    .Where(c => c.ProdOrderPartslistPosID.HasValue
                                                                && c.ProdOrderPartslistPos.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID
                                                                && c.FacilityBookingTypeIndex == (short)GlobalApp.FacilityBookingType.ProdOrderPosInward
                                                                && c.InsertDate >= minStartDate && c.InsertDate <= maxEndDate // Use Posting-Day instead!
                                                                && c.FacilityBooking.PropertyACUrl == machine.Key.ACURLComponentCached)
                                                    .ToArray();
                        if (postingsOnThisMachine != null && postingsOnThisMachine.Any())
                        {
                            quantityFound = true;
                            oeeEntry.Quantity = postingsOnThisMachine.Select(c => c.InwardQuantity)
                                                                    .DefaultIfEmpty()
                                                                    .Sum();
                            oeeEntry.QuantityScrap = postingsOnThisMachine.Where(c => c.MDMovementReason != null && c.MDMovementReason.MDMovementReasonIndex == (short)MovementReasonsEnum.Reject)
                                                 .Select(c => c.InwardQuantity)
                                                 .DefaultIfEmpty()
                                                 .Sum();
                        }

                        /// 2. Check if there are any OperationsLogs. 
                        /// If yes, then take this quant-quantities if they are not Bookings with low quantitites (e.g. rack trolleys with one piece because the put in quantity is unkown)
                        if (!quantityFound)
                        {
                            postingsOnThisMachine = databaseApp.FacilityBookingCharge
                                                    .Include(c => c.MDMovementReason)
                                                    .Where(c => c.ProdOrderPartslistPosID.HasValue
                                                                && c.ProdOrderPartslistPos.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID
                                                                && c.InwardFacilityCharge != null
                                                                && c.InwardFacilityCharge.OperationLog_FacilityCharge.Any(d => d.RefACClass.ParentACClassID == machineACClassID
                                                                                                                            && d.ACProgramLogID.HasValue
                                                                                                                            && d.ACProgramLogID == oeeEntry.ACProgramLogID))
                                                    .ToArray();

                            if (postingsOnThisMachine == null || !postingsOnThisMachine.Any())
                            {
                                postingsOnThisMachine = databaseApp.FacilityBookingCharge
                                                        .Include(c => c.MDMovementReason)
                                                        .Where(c => c.ProdOrderPartslistPosID.HasValue
                                                                    && c.ProdOrderPartslistPos.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID
                                                                    && c.InwardFacilityCharge != null
                                                                    && c.InwardFacilityCharge.OperationLog_FacilityCharge.Any(d => d.RefACClass.ParentACClassID == machineACClassID
                                                                                                                                && c.InsertDate >= minStartDate && c.InsertDate <= maxEndDate))
                                                        .ToArray();

                            }
                            if (postingsOnThisMachine != null && postingsOnThisMachine.Any())
                            {
                                quantityFound = true;
                                oeeEntry.Quantity = postingsOnThisMachine.Select(c => c.InwardQuantity)
                                                                        .DefaultIfEmpty()
                                                                        .Sum();
                                oeeEntry.QuantityScrap = postingsOnThisMachine.Where(c => c.MDMovementReason != null && c.MDMovementReason.MDMovementReasonIndex == (short)MovementReasonsEnum.Reject)
                                                     .Select(c => c.InwardQuantity)
                                                     .DefaultIfEmpty()
                                                     .Sum();
                            }
                        }

                        /// 3. Check whether there are consumption postings and find out the main component and determine about it (semi-finished product from preliminary stage) 
                        /// TODO: The problem here ist, that we dont have any information about scrap
                        if (!quantityFound && Math.Abs(prodOrderPartslist.ActualQuantity) <= FacilityConst.C_ZeroCompare)
                        {
                            var mainConsumptionPos = prodOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.SourceProdOrderPartslistID.HasValue).FirstOrDefault();
                            if (mainConsumptionPos != null && facilityMaterial.Material.BaseMDUnitID == mainConsumptionPos.Material.BaseMDUnitID)
                            {
                                postingsOnThisMachine =
                                    databaseApp.FacilityBookingCharge.Include(c => c.MDMovementReason)
                                                                .Where(c => c.ProdOrderPartslistPosRelationID.HasValue
                                                                            && c.ProdOrderPartslistPosRelation.SourceProdOrderPartslistPosID == mainConsumptionPos.ProdOrderPartslistPosID
                                                                            && c.InsertDate >= minStartDate && c.InsertDate <= maxEndDate // Use Posting-Day instead
                                                                            && c.FacilityBooking.PropertyACUrl == machine.Key.ACURLComponentCached)
                                                                .ToArray();
                                if (postingsOnThisMachine != null && postingsOnThisMachine.Any())
                                {
                                    quantityFound = true;
                                    oeeEntry.Quantity = postingsOnThisMachine.Select(c => c.OutwardQuantity)
                                                                            .DefaultIfEmpty()
                                                                            .Sum();
                                    oeeEntry.QuantityScrap = postingsOnThisMachine.Where(c => c.MDMovementReason != null && c.MDMovementReason.MDMovementReasonIndex == (short)MovementReasonsEnum.Reject)
                                                         .Select(c => c.OutwardQuantity)
                                                         .DefaultIfEmpty()
                                                         .Sum();
                                }
                            }
                        }
                        oeeEntry.CalcOEE();
                    }
                }

                if (prodOrderPartslist.ActualQuantity > FacilityConst.C_ZeroCompare)
                {
                    var oeeGroupedEntriesWithoutQ = oeeEntriesOfEntireOrder.Where(c => c.Quantity <= 0.00001 && c.OperationTimeSec > 0 && c.FacilityMaterial.Facility.VBiFacilityACClass.BasedOnACClassID.HasValue)
                                                                          .GroupBy(c => c.FacilityMaterial.Facility.VBiFacilityACClass.BasedOnACClassID.Value);
                    foreach (var key in oeeGroupedEntriesWithoutQ)
                    {
                        /// 4. Determine the quantity based on the end product and calculate the theoretical throughput based on the mean value
                        var entriesWithThroughPut = key.Where(c => c.FacilityMaterial.Throughput.HasValue && c.FacilityMaterial.Throughput.Value > FacilityConst.C_ZeroCompare)
                                                        .Select(c => new DistributionOEE() { OEEItem = c })
                                                        .ToArray();
                        // Only if all Machines has a defined Throughput, then a distribution of the produced quantity is possible
                        if (entriesWithThroughPut.Count() == key.Count())
                        {
                            double sumThroughPuts = 0;
                            int sumOperationTime = 0;
                            foreach (DistributionOEE d in entriesWithThroughPut)
                            {
                                sumThroughPuts += d.OEEItem.FacilityMaterial.Throughput.Value;
                                sumOperationTime += d.OEEItem.OperationTimeSec;
                            }
                            double sumQuota = 0;
                            foreach (DistributionOEE d in entriesWithThroughPut)
                            {
                                d.PercentualThroughPut = d.OEEItem.FacilityMaterial.Throughput.Value / sumThroughPuts;
                                d.PercentualOperatingTime = (double)d.OEEItem.OperationTimeSec / (double)sumOperationTime;
                                sumQuota += d.Quota;
                            }
                            foreach (DistributionOEE d in entriesWithThroughPut)
                            {
                                d.OEEItem.Quantity = (prodOrderPartslist.ActualQuantity * d.Quota) / sumQuota;
                                d.OEEItem.QuantityScrap = (prodOrderPartslist.ActualQuantityScrapUOM * d.Quota) / sumQuota;
                                d.OEEItem.CalcOEE();
                            }
                        }
                        else if (key.Count() == 1)
                        {
                            FacilityMaterialOEE oeeItem = key.FirstOrDefault();
                            oeeItem.Quantity = prodOrderPartslist.ActualQuantity;
                            oeeItem.QuantityScrap = prodOrderPartslist.ActualQuantityScrapUOM;
                            oeeItem.CalcOEE();
                            // Automatic Throughput-Calculation takes place afterwards OnPostProcessingOEE
                            //if (oeeItem.AvailabilityOEE >= C_AvailabilityThreasholdForAutoCalc
                            //    && (!oeeItem.FacilityMaterial.Throughput.HasValue || oeeItem.FacilityMaterial.Throughput.Value <= FacilityConst.C_ZeroCompare)
                            //    && oeeItem.FacilityMaterial.ThroughputAuto == 1)
                            //{
                            //    oeeItem.FacilityMaterial.Throughput = oeeItem.Throughput;
                            //}
                        }
                    }
                }

                itemsForPostProcessing.Add(oeeEntriesOfEntireOrder);
            }
        }

        internal void OnPostProcessingOEE(DatabaseApp databaseApp, ProdOrder prodOrder, List<object> itemsForPostProcessing)
        {
            IEnumerable<List<FacilityMaterialOEE>> oeeEntriesOfEntireOrder = itemsForPostProcessing.Where(c => c is List<FacilityMaterialOEE>).Select(c => c as List<FacilityMaterialOEE>);
            if (oeeEntriesOfEntireOrder == null)
                return;

            // If Availability 100% und auto correction is set, then recalculate new Average
            foreach (List<FacilityMaterialOEE> facMatOEEs in oeeEntriesOfEntireOrder)
            {
                foreach (FacilityMaterial facMat in facMatOEEs.Where(c => c.FacilityMaterial != null && c.FacilityMaterial.ThroughputAuto == 1 && c.AvailabilityOEE >= C_AvailabilityThreasholdForAutoCalc && c.Quantity >= FacilityConst.C_ZeroCompare)
                            .Select(c => c.FacilityMaterial)
                            .Distinct())
                {
                    var msg = RecalcThroughputAverage(databaseApp, facMat, true);
                    if (msg != null)
                        databaseApp.ACUndoChanges();
                }
            }
        }


        private class DistributionOEE
        {
            public FacilityMaterialOEE OEEItem { get; set; }
            public double PercentualThroughPut { get; set; }
            public double PercentualOperatingTime { get; set; }
            public double Quota { get { return PercentualThroughPut / PercentualOperatingTime; } }
        }

        #endregion

        #endregion

    }
}
