using gip.core.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility
{
    public class MaterialPreparationWFNode
    {
        public ACClassWF ACClassWF { get; set; }

        public List<MaterialPreparationConfigNode> ConfigNodes { get; set; } = new List<MaterialPreparationConfigNode>();
    }
}
