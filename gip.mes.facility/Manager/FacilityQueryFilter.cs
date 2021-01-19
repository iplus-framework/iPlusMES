using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.facility
{
    public class FacilityQueryFilter
    {
        public DateTime SearchFrom { get; set; }
        public DateTime SearchTo { get; set; }

        public short? FilterFBTypeValue { get; set; }

        public Guid? FacilityID { get; set; }
        public Guid? FacilityLotID { get; set; }
        public Guid? FacilityChargeID { get; set; }
        public Guid? FacilityLocationID { get; set; }

        public Guid? MaterialID { get; set; }
        public Guid? FacilityInventoryPosID { get; set; }

        public bool HasEntityFilterSet
        {
            get
            {
                return FacilityID.HasValue || FacilityLotID.HasValue || FacilityChargeID.HasValue || FacilityLocationID.HasValue || MaterialID.HasValue;
            }
        }
    }
}
