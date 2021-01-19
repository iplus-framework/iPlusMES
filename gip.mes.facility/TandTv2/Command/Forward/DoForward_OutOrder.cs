using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;

namespace gip.mes.facility
{
    public class DoForward_OutOrder : DoBackward_OutOrder
    {
        #region ctor's

        public DoForward_OutOrder(DatabaseApp databaseApp, TandTv2Result result, OutOrder item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion
    }
}
