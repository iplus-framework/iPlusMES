using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;

namespace gip.mes.facility.TandTv3
{
    public interface IItemTracking<out T> where T : IACObjectEntity
    {
        T Item { get; }
        DatabaseApp DatabaseApp { get; }
        TandTResult Result { get; }

        TandTStep Step { get; set; }

        void AssignItemToMixPoint(List<IACObjectEntity> sameStepItems);

        List<IACObjectEntity> GetSameStepItems();
        List<IACObjectEntity> GetNextStepItems();

        string GetItemNo();

        #region Tree

        IItemTracking<IACObjectEntity> SameStepParent { get; set; }
        IItemTracking<IACObjectEntity> NextStepParent { get; set; }

        string GetMDLink();
        string GetMDString();
        string ToMDString();

        #endregion
    }
}
