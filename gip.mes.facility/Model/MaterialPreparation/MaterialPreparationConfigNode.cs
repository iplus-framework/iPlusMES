using gip.core.datamodel;
using System;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public class MaterialPreparationConfigNode
    {
        public string PreConfigACUrl { get; set; }
        public IACConfig ACConfig { get; set; }

        public List<ACClass> AllowedMachines { get; set; } = new List<ACClass>();

        #region Overrides
        public override string ToString()
        {
            return $"PreConfigACUrl: {PreConfigACUrl}";
        }
        #endregion
    }
}
