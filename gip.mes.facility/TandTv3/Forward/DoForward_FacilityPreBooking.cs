using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.facility.TandTv3
{
    public class DoForward_FacilityPreBooking : DoBackward_FacilityPreBooking
    {

        #region ctor's

        public DoForward_FacilityPreBooking(DatabaseApp databaseApp, TandTResult result, FacilityPreBooking item) : base(databaseApp, result, item)
        {
            TrackingDirection = MDTrackingDirectionEnum.Forward;
        }

        #endregion

        #region IItemTracking

        public override List<IACObjectEntity> GetNextStepItems()
        {
            List<IACObjectEntity> nextStepItems = new List<IACObjectEntity>();
            FacilityCharge fc = null;
            if (Item.InwardFacilityCharge != null)
                fc = Item.InwardFacilityCharge;
            if (fc != null && Item.OutwardMaterial != null)
            {
                var nextFbcIds =
                    fc
                    .FacilityLot
                    .FacilityBookingCharge_OutwardFacilityLot
                    .Where(c => c.OutwardMaterialID == Item.InwardMaterial.MaterialID)
                    .OrderBy(c => c.FacilityBookingChargeNo)
                    .Select(c => new { c.FacilityBookingChargeID, c.ProdOrderPartslistPosID, c.ProdOrderPartslistPosRelationID, c.InOrderPosID })
                    .ToList();

                Guid[] fbcIds = nextFbcIds.Select(c => c.FacilityBookingChargeID).ToArray();
                List<FacilityBookingCharge> nextFbc = 
                    DatabaseApp
                    .FacilityBookingCharge
                    .Include(c => c.InwardFacilityCharge)
                    .Include(c => c.InwardFacilityCharge.FacilityLot)
                    .Include(c => c.ProdOrderPartslistPosRelation)
                    .Include(c => c.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos)
                    .Include(c => c.ProdOrderPartslistPos)
                    .Include(c => c.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos)
                    .Where(c => fbcIds.Contains(c.FacilityBookingChargeID)).ToList();
                nextStepItems.AddRange(nextFbc);
            }

            return nextStepItems;
        }

        #endregion
    }
}