using System.Configuration;

namespace gip.mes.datamodel
{
    public class TrackingConfiguration : ConfigurationSection
    {

        [ConfigurationProperty(nameof(WorkingModel), DefaultValue = TrackingWorkingModelEnum.Multiple)]
        public TrackingWorkingModelEnum WorkingModel
        {
            get
            {
                return (TrackingWorkingModelEnum)this[nameof(WorkingModel)];
            }
            set
            {
                this[nameof(WorkingModel)] = value;
            }
        }

        [ConfigurationProperty(nameof(DefaultTrackingEngine), DefaultValue = TrackingEnginesEnum.v3)]
        public TrackingEnginesEnum DefaultTrackingEngine
        {
            get
            {
                return (TrackingEnginesEnum)this[nameof(DefaultTrackingEngine)];
            }
            set
            {
                this[nameof(DefaultTrackingEngine)] = value;
            }
        }

        [ConfigurationProperty(nameof(TandTWriteDiagnosticLog), DefaultValue = false)]
        public bool TandTWriteDiagnosticLog
        {
            get
            {
                return (bool)this[nameof(TandTWriteDiagnosticLog)];
            }
            set
            {
                this[nameof(TandTWriteDiagnosticLog)] = value;
            }

        }

        [ConfigurationProperty(nameof(UseMDFile), DefaultValue = false)]
        public bool UseMDFile
        {
            get
            {
                return (bool)this[nameof(UseMDFile)];
            }
            set
            {
                this[nameof(UseMDFile)] = value;
            }

        }

        [ConfigurationProperty(nameof(RootLogFolder), DefaultValue = "")]
        public string RootLogFolder
        {
            get
            {
                return (string)this[nameof(RootLogFolder)];
            }
            set
            {
                this[nameof(RootLogFolder)] = value;
            }

        }

        [ConfigurationProperty(nameof(RootLogFolder), DefaultValue = "")]
        public double? PercentageToFollow
        {
            get
            {
                if(this[nameof(PercentageToFollow)] == null)
                {
                    return null;
                }
                return (double)this[nameof(PercentageToFollow)];
            }
            set
            {
                this[nameof(PercentageToFollow)] = value;
            }

        }

        public static TrackingConfiguration FactoryDefaultConfiguration()
        {
            TrackingConfiguration trackingConfiguration= new TrackingConfiguration();
            trackingConfiguration.WorkingModel = TrackingWorkingModelEnum.Multiple;
            trackingConfiguration.DefaultTrackingEngine = TrackingEnginesEnum.v3;
            trackingConfiguration.TandTWriteDiagnosticLog = false;
            return trackingConfiguration;
        }
    }

    
}
