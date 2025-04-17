using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    public class ProdOrderPlOverview : MaterialOverview
    {
        public ProdOrderPlOverview(ProdOrderPartslist pl)
        {
            ProgramNo = pl.ProdOrder.ProgramNo;
            MaterialNo = pl.Partslist.Material.MaterialNo;
            MaterialName = pl.Partslist.Material.MaterialName1;
            ActualQuantity = pl.ActualQuantity;

            Inputs =
                pl
                .ProdOrderPartslistPos_ProdOrderPartslist
                .SelectMany(c => c.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                .SelectMany(c => c.FacilityBooking_ProdOrderPartslistPosRelation)
                .GroupBy(c => new { c.OutwardMaterial.MaterialNo, c.OutwardMaterial.MaterialName1 })
                .Select(c => new MaterialOverview()
                {
                    MaterialNo = c.Key.MaterialNo,
                    MaterialName = c.Key.MaterialName1,
                    ActualQuantity = c.Select(x => x.OutwardQuantity).Sum()
                })
                .ToList();

            if (ActualQuantity > 0)
            {
                foreach (MaterialOverview pPMaterialOverview in Inputs)
                {
                    pPMaterialOverview.DosingRatio = pPMaterialOverview.ActualQuantity / ActualQuantity;
                }
            }

        }

        public string ProgramNo { get; set; }

        public List<MaterialOverview> Inputs { get; set; }
    }
}
