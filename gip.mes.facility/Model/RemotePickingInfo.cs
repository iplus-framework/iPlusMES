using System;

namespace gip.mes.facility
{
    public class RemotePickingInfo
    {
        public long RowNr { get; set; }

        public string PickingNo { get; set; }
        public DateTime DeliveryDateFrom { get; set; }
        public DateTime InsertDate { get; set; }
        public string InsertName { get; set; }
        public DateTime? PosStartUpdate { get; set; }
        public DateTime? PosEndUpdate { get; set; }
        public DateTime? FBStartUpdate { get; set; }
        public DateTime? FBEndUpdate { get; set; }

        public bool? IsSuccessfullyClosed { get; set; }
    }
}
