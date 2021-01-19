using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_InOrder : DoBackward_InOrder
    {

        #region ctor's
        public DoForward_InOrder(DatabaseApp databaseApp, TandTv2Result result, InOrder item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

    }
}
