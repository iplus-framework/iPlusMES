using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_ProdOrder : DoBackward_ProdOrder
    {

        #region ctor's

        public DoForward_ProdOrder(DatabaseApp databaseApp, TandTv2Result result, ProdOrder item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

    }
}
