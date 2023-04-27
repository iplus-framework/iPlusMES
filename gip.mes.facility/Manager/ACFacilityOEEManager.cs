using gip.core.autocomponent;
using gip.core.datamodel;
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

        public Msg RecalcThroughputAverage(DatabaseApp dbApp, FacilityMaterial facilityMaterial, bool saveChanges = true, int countLatestEntries = 500, double availabilityThreshold = 0.990)
        {
            Msg msg = null;
            if (facilityMaterial == null || dbApp == null)
                return msg;

            try
            {
                double avgThroughPut =
                    dbApp.FacilityMaterialOEE
                    .Where(c => c.FacilityMaterial.FacilityID == facilityMaterial.FacilityID
                                && c.AvailabilityOEE > availabilityThreshold
                                && c.Throughput > 0.00001
                                && (!facilityMaterial.ThroughputMax.HasValue || c.Throughput <= facilityMaterial.ThroughputMax)
                                && (!facilityMaterial.ThroughputMin.HasValue || c.Throughput >= facilityMaterial.ThroughputMin))
                    .OrderByDescending(c => c.EndDate)
                    .Take(countLatestEntries)
                    .Average(c => c.Throughput);
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

        #endregion

    }
}
