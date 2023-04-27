using System.Collections.Generic;

namespace gip.mes.facility
{
    public class PartslistValidationInfo
    {
        public List<MapPosToWFConn> MapPosToWFConnections { get; internal set; } = new List<MapPosToWFConn>();
        public bool IsSucceded { get; internal set; }
    }
}
