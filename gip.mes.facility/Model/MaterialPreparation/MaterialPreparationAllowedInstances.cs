using System;
using System.Collections.Generic;
using gip.core.datamodel;
using VD = gip.mes.datamodel;

namespace gip.mes.facility
{
    public class MaterialPreparationAllowedInstances
    {
        public List<Guid> PartslistIds { get; set; } = new List<Guid>();
        public List<VD.ProdOrderPartslist> ProdorderPartslists { get; set; } = new List<VD.ProdOrderPartslist>();

        public IACConfig AllowedInstancesConfig { get; set; }

        public List<ACClass> AllowedInstances { get; set; } = new List<ACClass>();
        public List<VD.Facility> ConnectedFacilities { get; set; } = new List<VD.Facility>();
    }
}
