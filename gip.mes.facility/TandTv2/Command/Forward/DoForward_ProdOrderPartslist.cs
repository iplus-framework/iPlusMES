using System.Collections.Generic;
using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_ProdOrderPartslist : DoBackward_ProdOrderPartslist
    {
        #region ctor's

        public DoForward_ProdOrderPartslist(DatabaseApp databaseApp, TandTv2Result result, ProdOrderPartslist item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

        #region overrides
        public override List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            return new List<IDoItem>() { new DoForward_ProdOrder(databaseApp, result, Item.ProdOrder, jobFilter) };
        }

        #endregion

    }
}
