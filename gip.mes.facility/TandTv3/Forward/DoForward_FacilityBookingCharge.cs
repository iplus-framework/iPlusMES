using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_FacilityBookingCharge : DoBackward_FacilityBookingCharge
    {

        #region ctor's
        public DoForward_FacilityBookingCharge(DatabaseApp databaseApp, TandTResult result, FacilityBookingCharge item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }
        #endregion

        #region IItemTracking

        public override List<IACObjectEntity> GetNextStepItems()
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();

            FacilityCharge fc = null;
            if (Item.InwardFacilityChargeID != null)
                fc = Item.InwardFacilityCharge;
            if (fc != null && Item.InwardMaterialID != null)
            {
                var nextFbcIds =
                    fc
                    .FacilityLot
                    .FacilityBookingCharge_OutwardFacilityLot
                    .Where(c => c.OutwardMaterialID == Item.InwardMaterialID)
                    .OrderBy(c => c.FacilityBookingChargeNo)
                    .Select(c => new { c.FacilityBookingChargeID, c.ProdOrderPartslistPosID, c.ProdOrderPartslistPosRelationID, c.InOrderPosID })
                    .ToList();

                Guid[] fbcIds = nextFbcIds.Select(c => c.FacilityBookingChargeID).ToArray();
                List<FacilityBookingCharge> nextFbc = DatabaseApp.FacilityBookingCharge.Where(c => fbcIds.Contains(c.FacilityBookingChargeID)).ToList();
                nextStepItems.AddRange(nextFbc);
            }

            return nextStepItems;
        }

        #endregion

    }
}