using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'OEE-Log'}de{'OEE-Log'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, nameof(FacilityMaterial), ConstApp.FacilityMaterial, Const.ContextDatabase + "\\" + nameof(FacilityMaterial) + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, "ACProgramLogID", "en{'Program Log'}de{'Program Log'}", "", "", true)]
    [ACPropertyEntity(3, "StartDate", "en{'Production Start'}de{'Produktionsstart'}", "", "", true)]
    [ACPropertyEntity(4, "EndDate", "en{'Production End'}de{'Produktionsende'}", "", "", true)]
    [ACPropertyEntity(5, "IdleTimeSec", "en{'Idle [sec]'}de{'Inaktiv [sec]'}", "", "", true)]
    [ACPropertyEntity(6, "StandByTimeSec", "en{'Stand by [sec]'}de{'Still stehend [sec]'}", "", "", true)]
    [ACPropertyEntity(7, "OperationTimeSec", "en{'Operationtime [sec]'}de{'Betriebszeit [sec]'}", "", "", true)]
    [ACPropertyEntity(8, "ScheduledBreakTimeSec", "en{'Schedule break time [sec]'}de{'Geplante Unterbrechung [sec]'}", "", "", true)]
    [ACPropertyEntity(9, "UnscheduledBreakTimeSec", "en{'Unscheduled break time [sec]'}de{'Ungeplante Unterbrechung [sec]'}", "", "", true)]
    [ACPropertyEntity(10, "RetoolingTimeSec", "en{'Retooling [sec]'}de{'Rüstzeit [sec]'}", "", "", true)]
    [ACPropertyEntity(11, "MaintenanceTimeSec", "en{'Maintenance [sec]'}de{'Wartung [sec]'}", "", "", true)]
    [ACPropertyEntity(12, "AvailabilityOEE", "en{'Availability'}de{'Verfügbarkeit'}", "", "", true)]
    [ACPropertyEntity(13, "Quantity", "en{'Total quantity [UOM]'}de{'Gesamtmenge [BME]'}", "", "", true)]
    [ACPropertyEntity(14, "Throughput", "en{'Throughput [UOM/h]'}de{'Durchsatz [BME/h]'}", "", "", true)]
    [ACPropertyEntity(15, "PerformanceOEE", "en{'Performance'}de{'Leistung'}", "", "", true)]
    [ACPropertyEntity(16, "QuantityScrap", "en{'Scrap quantity [UOM]'}de{'Ausschuss [UOM]'}", "", "", true)]
    [ACPropertyEntity(17, "QualityOEE", "en{'Quality'}de{'Qualität'}", "", "", true)]
    [ACPropertyEntity(18, "TotalOEE", "en{'OEE'}de{'OEE'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    public partial class FacilityMaterialOEE
    {
        #region New/Delete

        public static FacilityMaterialOEE NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            FacilityMaterialOEE entity = new FacilityMaterialOEE();
            entity.FacilityMaterialOEEID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.FacilityMaterial = parentACObject as FacilityMaterial;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            return base.EntityCheckAdded(user, context);
        }

        #endregion

        #region PropertyChanging

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(IdleTime):
                    _IdleTime = null;
                    break;
                case nameof(MaintenanceTime):
                    _MaintenanceTime = null;
                    break;
                case nameof(RetoolingTime):
                    _RetoolingTime = null;
                    break;
                case nameof(ScheduledBreakTime):
                    _ScheduledBreakTime = null;
                    break;
                case nameof(UnscheduledBreakTime):
                    _UnscheduledBreakTime = null;
                    break;
                case nameof(OperationTime):
                    _OperationTime = null;
                    break;
                case nameof(StandByTime):
                    _StandByTime = null;
                    break;
            }
            base.OnPropertyChanged(propertyName);
        }

        #endregion

        #region Properties

        private TimeSpan? _IdleTime;
        [ACPropertyInfo(100, "", "en{'Idle'}de{'Inaktiv'}")]
        public TimeSpan IdleTime
        {
            get
            {
                if (!_IdleTime.HasValue)
                    _IdleTime = TimeSpan.FromSeconds(IdleTimeSec);
                return _IdleTime.Value;
            }
            set
            {
                IdleTimeSec = (int)value.TotalSeconds;
                OnPropertyChanged(nameof(IdleTime));
            }
        }

        private TimeSpan? _MaintenanceTime;
        [ACPropertyInfo(101, "", "en{'Maintenance'}de{'Wartung'}")]
        public TimeSpan MaintenanceTime
        {
            get
            {
                if (!_MaintenanceTime.HasValue)
                    _MaintenanceTime = TimeSpan.FromSeconds(MaintenanceTimeSec);
                return _MaintenanceTime.Value;
            }
            set
            {
                MaintenanceTimeSec = (int)value.TotalSeconds;
                OnPropertyChanged(nameof(MaintenanceTime));
            }
        }

        private TimeSpan? _RetoolingTime;
        [ACPropertyInfo(102, "", "en{'Retooling'}de{'Rüstzeit'}")]
        public TimeSpan RetoolingTime
        {
            get
            {
                if (!_RetoolingTime.HasValue)
                    _RetoolingTime = TimeSpan.FromSeconds(RetoolingTimeSec);
                return _RetoolingTime.Value;
            }
            set
            {
                RetoolingTimeSec = (int)value.TotalSeconds;
                OnPropertyChanged(nameof(RetoolingTime));
            }
        }

        private TimeSpan? _ScheduledBreakTime;
        [ACPropertyInfo(103, "", "en{'Schedule break time'}de{'Geplante Unterbrechung'}")]
        public TimeSpan ScheduledBreakTime
        {
            get
            {
                if (!_ScheduledBreakTime.HasValue)
                    _ScheduledBreakTime = TimeSpan.FromSeconds(ScheduledBreakTimeSec);
                return _ScheduledBreakTime.Value;
            }
            set
            {
                ScheduledBreakTimeSec = (int)value.TotalSeconds;
                OnPropertyChanged(nameof(ScheduledBreakTime));
            }
        }

        private TimeSpan? _UnscheduledBreakTime;
        [ACPropertyInfo(104, "", "en{'Unscheduled break time'}de{'Ungeplante Unterbrechung'}")]
        public TimeSpan UnscheduledBreakTime
        {
            get
            {
                if (!_UnscheduledBreakTime.HasValue)
                    _UnscheduledBreakTime = TimeSpan.FromSeconds(UnscheduledBreakTimeSec);
                return _UnscheduledBreakTime.Value;
            }
            set
            {
                UnscheduledBreakTimeSec = (int)value.TotalSeconds;
                OnPropertyChanged(nameof(UnscheduledBreakTime));
            }
        }

        private TimeSpan? _OperationTime;
        [ACPropertyInfo(105, "", "en{'Operationtime'}de{'Betriebszeit'}")]
        public TimeSpan OperationTime
        {
            get
            {
                if (!_OperationTime.HasValue)
                    _OperationTime = TimeSpan.FromSeconds(OperationTimeSec);
                return _OperationTime.Value;
            }
            set
            {
                OperationTimeSec = (int)value.TotalSeconds;
                OnPropertyChanged(nameof(OperationTime));
            }
        }

        private TimeSpan? _StandByTime;
        [ACPropertyInfo(105, "", "en{'Stand by'}de{'Still stehend'}")]
        public TimeSpan StandByTime
        {
            get
            {
                if (!_StandByTime.HasValue)
                    _StandByTime = TimeSpan.FromSeconds(StandByTimeSec);
                return _StandByTime.Value;
            }
            set
            {
                StandByTimeSec = (int)value.TotalSeconds;
                OnPropertyChanged(nameof(StandByTime));
            }
        }

        [ACPropertyInfo(110, "", "en{'Availability [%]'}de{'Verfügbarkeit [%]'}")]
        public double AvailabilityOEEPer
        {
            get
            {
                return AvailabilityOEE * 100;
            }
        }

        [ACPropertyInfo(111, "", "en{'Performance [%]'}de{'Leistung [%]'}")]
        public double PerformanceOEEPer
        {
            get
            {
                return PerformanceOEE * 100;
            }
        }

        [ACPropertyInfo(112, "", "en{'Quality [%]'}de{'Qualität [%]'}")]
        public double QualityOEEPer
        {
            get
            {
                return QualityOEE * 100;
            }
        }

        [ACPropertyInfo(113, "", "en{'OEE [%]'}de{'OEE [%]'}")]
        public double TotalOEEPer
        {
            get
            {
                return TotalOEE * 100;
            }
        }

        public void CalcAvailabilityOEE()
        {
            if (OperationTimeSec > 0)
            {
                int plannedTimeSec = OperationTimeSec + UnscheduledBreakTimeSec;
                AvailabilityOEE = (double)OperationTimeSec / (double)plannedTimeSec;
            }
            else
                AvailabilityOEE = 1;
        }


        public void CalcPerformanceOEE()
        {
            PerformanceOEE = 1;
            if (Quantity > double.Epsilon && OperationTimeSec > 0)
            {
                Throughput = Quantity * 3600.0 / (double)OperationTimeSec;
                if (   this.FacilityMaterial.Throughput.HasValue
                    && this.FacilityMaterial.Throughput.Value > double.Epsilon)
                {
                    PerformanceOEE = Throughput / this.FacilityMaterial.Throughput.Value;
                    if (PerformanceOEE > 1)
                        PerformanceOEE = 1;
                }
            }
        }

        public void CalcQualityOEE()
        {
            QualityOEE = 1;
            if (Quantity > double.Epsilon)
            {
                if (QuantityScrap > double.Epsilon)
                    QualityOEE = (Quantity - QuantityScrap) / Quantity;
                if (QualityOEE > 1)
                    QualityOEE = 1;
            }
        }

        public void CalcOEE()
        {
            CalcAvailabilityOEE();
            CalcPerformanceOEE();
            CalcQualityOEE();
            this.TotalOEE = (AvailabilityOEE > double.Epsilon ? AvailabilityOEE : 1) * (PerformanceOEE > double.Epsilon ? PerformanceOEE : 1) * (QualityOEE > double.Epsilon ? QualityOEE : 1);
        }

        #endregion
    }
}
