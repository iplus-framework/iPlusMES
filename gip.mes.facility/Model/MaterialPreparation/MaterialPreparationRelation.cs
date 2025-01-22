using gip.mes.datamodel;

namespace gip.mes.facility
{
    public class MaterialPreparationRelation
    {
        public MaterialPreparationRelation(MaterialPreparationDosing materialPreparationDosing)
        {
            MaterialPreparationDosing = materialPreparationDosing;
        }
        public MaterialPreparationDosing MaterialPreparationDosing { get;set; }

        public ProdOrderPartslistPosRelation Relation { get; set; }
        public double Factor { get; set; }

        public override string ToString()
        {
            return Relation?.ToString();
        }
    }
}
