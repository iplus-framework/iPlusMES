using gip.core.datamodel;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_ProdOrderPartslistPosRelation : DoBackward_ProdOrderPartslistPosRelation
    {

        #region ctor's

        public DoForward_ProdOrderPartslistPosRelation(DatabaseApp databaseApp, TandTResult result, ProdOrderPartslistPosRelation item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion

        #region IItemTracking

        #region IItemtracking -> SameStepItem

        public override List<IACObjectEntity> GetSameStepItems()
        {
            List<IACObjectEntity> sameStepItems = new List<IACObjectEntity>();
            sameStepItems.Add(Item.TargetProdOrderPartslistPos);
            return sameStepItems;
        }

        #endregion

        #region IItemTracking -> NextStepItems

        public override List<IACObjectEntity> GetNextStepItems()
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();
            return nextStepItems;
        }

        #endregion
        #endregion
    }
}