using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_ProdOrderPartslistPosRelation : DoBackward_ProdOrderPartslistPosRelation
    {

        #region ctor's

        public DoForward_ProdOrderPartslistPosRelation(DatabaseApp databaseApp, TandTv2Result result, ProdOrderPartslistPosRelation item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion


        #region overriden methods
        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            IEnumerable<ACClass> foundedRelatedMachines = FindRelatedMachine(databaseApp);
            if (foundedRelatedMachines == null || !foundedRelatedMachines.Any()) return null;
            return
                foundedRelatedMachines
               .Select(c =>
               new DoForward_ACClass(databaseApp, result, c, jobFilter) as IDoItem).ToList();
        }

        public override List<IDoItem> SearchRelatedNextStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            List<IDoItem> related = new List<IDoItem>();
            related.Add(new DoForward_ProdOrderPartslistPos(databaseApp, result, Item.TargetProdOrderPartslistPos, jobFilter));
            return related;
        }

        #endregion

    }
}
