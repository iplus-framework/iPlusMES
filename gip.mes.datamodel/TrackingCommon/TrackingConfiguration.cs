using System;
using System.Collections.Generic;
using System.Configuration;

namespace gip.mes.datamodel
{
    public class TrackingConfiguration : ConfigurationSection
    {

        [ConfigurationProperty("WorkingModel", DefaultValue = TrackingWorkingModelEnum.Multiple)]
        public TrackingWorkingModelEnum WorkingModel
        {
            get
            {
                return (TrackingWorkingModelEnum)this["WorkingModel"];
            }
            set
            {
                this["WorkingModel"] = value;
            }
        }

        [ConfigurationProperty("DefaultTrackingEngine", DefaultValue = TrackingEnginesEnum.v3)]
        public TrackingEnginesEnum DefaultTrackingEngine
        {
            get
            {
                return (TrackingEnginesEnum)this["DefaultTrackingEngine"];
            }
            set
            {
                this["DefaultTrackingEngine"] = value;
            }
        }

        [ConfigurationProperty("TandTWriteDiagnosticLog", DefaultValue = false)]
        public bool TandTWriteDiagnosticLog
        {
            get
            {
                return (bool)this["TandTWriteDiagnosticLog"];
            }
            set
            {
                this["TandTWriteDiagnosticLog"] = value;
            }

        }

        [ConfigurationProperty("RootLogFolder", DefaultValue = "")]
        public string RootLogFolder
        {
            get
            {
                return (string)this["RootLogFolder"];
            }
            set
            {
                this["RootLogFolder"] = value;
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
