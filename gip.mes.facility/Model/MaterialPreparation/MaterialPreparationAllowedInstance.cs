using gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public class MaterialPreparationAllowedInstance
    {
        public gip.core.datamodel.ACClass Machine { get; set; }
        public List<Facility> ConnectedFacilities { get;set; } = new List<Facility>();

        #region Overrides
        public override string ToString()
        {
            return Machine?.ACIdentifier;
        }
        #endregion
    }
}
