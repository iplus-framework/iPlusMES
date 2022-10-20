using gip.mes.datamodel;

namespace gip.mes.facility
{
    public class ActualSalesTax
    {
        public MDCountrySalesTax MDCountrySalesTax { get; set; }
        public MDCountrySalesTaxMaterial MDCountrySalesTaxMaterial { get; set; }
        public MDCountrySalesTaxMDMaterialGroup MDCountrySalesTaxMDMaterialGroup { get; set; }
    }
}
