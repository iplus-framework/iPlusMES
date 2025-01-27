using System.Collections.Generic;

namespace gip.mes.facility
{
    public class MaterialPreparationResult
    {
        public List<MaterialPreparationDosing> BatchDosings { get; set; } = new List<MaterialPreparationDosing>();

        public List<MaterialPreparationWFNode> WFNodes { get; set; } = new List<MaterialPreparationWFNode>();

        public List<MaterialPreparationAllowedInstance> AllowedInstances { get; set; } = new List<MaterialPreparationAllowedInstance>();

        public List<MaterialPreparationModel> PreparedMaterials { get; set; }

    }
}
