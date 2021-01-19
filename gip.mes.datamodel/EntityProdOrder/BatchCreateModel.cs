using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    public class BatchCreateModel
    {
        public BatchCreateModel()
        {
            Batches = new List<ProdOrderBatch>();
            BatchRelations = new List<ProdOrderPartslistPosRelation>();
            BatchPositions = new List<ProdOrderPartslistPos>();
        }

        public Dictionary<int, double> BatchDefinition { get; set; }
        public List<ProdOrderBatch> Batches { get; set; }

        public List<ProdOrderPartslistPosRelation> BatchRelations { get; set; }
        public List<ProdOrderPartslistPos> BatchPositions { get; set; }
    }
}
 
