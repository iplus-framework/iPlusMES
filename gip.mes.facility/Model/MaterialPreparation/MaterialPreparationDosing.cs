using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility
{
    /// <summary>
    ///  Sort dosings by all relevant parameters
    ///  - Config Stores: ProdOrderPartslist, Partslist
    ///  - Dosing positions MaterialWF -> InwardMaterial -> ACClassWF[]
    ///  - Scheduling group
    /// </summary>
    public class MaterialPreparationDosing
    {
        public MDSchedulingGroup MDSchedulingGroup { get; set; }
        public Material InwardMaterial { get; set; }
        public MaterialWF MaterialWF { get; set; }
        public List<gip.core.datamodel.ACClassWF>  ACClassWFs { get; set; }

        public ProdOrderPartslist ProdOrderPartslist { get; set; }
        public Partslist Partslist { get; set; }

        public string PreConfigACUrl { get; set; }

        public List<MaterialPreparationRelation> Dosings { get; set; } = new List<MaterialPreparationRelation>();

        public override string ToString()
        {
            return $"MatNo: {InwardMaterial?.MaterialNo} MatWf: {MaterialWF?.MaterialWFNo} PreConfigACUrl: {PreConfigACUrl} PL: {Partslist?.PartslistNo} Pr: {ProdOrderPartslist?.ToString()}";
        }
    }
}
