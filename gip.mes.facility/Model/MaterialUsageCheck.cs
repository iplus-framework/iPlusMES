using gip.mes.datamodel;

namespace gip.mes.facility
{
    public class MaterialUsageCheck
    {
        public Material Material { get; set; }
        public double OutwardQuantityUOM { get; set; }
        public double InwardQuantityUOM { get; set; }

        public bool IsQuantityValid
        {
            get
            {
                return OutwardQuantityUOM >= InwardQuantityUOM;
            }
        }
    }
}
