using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility.TandTv3
{
    public interface ITandTv3Process
    {
        IItemTracking<IACObjectEntity> FactoryBacwardItem(IACObjectEntity item);
        IItemTracking<IACObjectEntity> FactoryForwardItem(IACObjectEntity item);

        List<IItemTracking<IACObjectEntity>> OperateSameStepItems(List<IACObjectEntity> sameStepItems, MDTrackingDirectionEnum trackingDirection, IItemTracking<IACObjectEntity> callerItem);
        List<IItemTracking<IACObjectEntity>> OperateNextStepItems(List<IACObjectEntity> nextStepItems, MDTrackingDirectionEnum trackingDirection, IItemTracking<IACObjectEntity> callerItem);


    }
}
