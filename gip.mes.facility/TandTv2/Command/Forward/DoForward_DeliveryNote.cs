using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;


namespace gip.mes.facility
{
    public class DoForward_DeliveryNote : DoBackward_DeliveryNote
    {

        #region ctor's

        public DoForward_DeliveryNote(DatabaseApp databaseApp, TandTv2Result result, DeliveryNote item, TandTv2Job jobFilter) : base(databaseApp, result, item, jobFilter)
        {

        }

        #endregion

    }
}
