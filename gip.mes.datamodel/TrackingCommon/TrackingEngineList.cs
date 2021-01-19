using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    public static class TrackingEngineList
    {
        private static Dictionary<TrackingEnginesEnum, string> _TrackingEngines;
        public static Dictionary<TrackingEnginesEnum, string> TrackingEngines
        {
            get
            {
                if (_TrackingEngines == null)
                    _TrackingEngines = LoadTrackingEngines();
                return _TrackingEngines;
            }
        }

        private static Dictionary<TrackingEnginesEnum, string> LoadTrackingEngines()
        {
            Dictionary<TrackingEnginesEnum, string> engines = new Dictionary<TrackingEnginesEnum, string>();
            engines.Add(TrackingEnginesEnum.v1, "en{'v1'}de{'v1'}");
            engines.Add(TrackingEnginesEnum.v2, "en{'v2'}de{'v2'}");
            engines.Add(TrackingEnginesEnum.v3, "en{'v3'}de{'v3'}");
            return engines;
        }
    }
}
