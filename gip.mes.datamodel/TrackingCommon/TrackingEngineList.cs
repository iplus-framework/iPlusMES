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
            engines.Add(TrackingEnginesEnum.v3, "en{'v3'}de{'v3'}");
            return engines;
        }
    }
}
