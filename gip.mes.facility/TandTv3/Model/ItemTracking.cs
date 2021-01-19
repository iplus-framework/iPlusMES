using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using gip.mes.facility;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public abstract class ItemTracking<T> : IItemTracking<T> where T : IACObjectEntity
    {

        #region ctor's
        public ItemTracking(DatabaseApp databaseApp, TandTResult result, T item)
        {
            DatabaseApp = databaseApp;
            Result = result;
            Item = item;

            TrackingDirection = MDTrackingDirectionEnum.Backward;
        }

        #endregion

        #region IACObjectEntity


        #region Properties

        public MDTrackingDirectionEnum TrackingDirection { get; set; }
        public string ItemTypeName { get; set; }

        public T Item { get; private set; }

        public DatabaseApp DatabaseApp { get; private set; }
        public TandTResult Result { get; private set; }

        public TandTStep Step { get; set; }

        #endregion

        #region Methods

        public virtual void AssignItemToMixPoint(List<IACObjectEntity> sameStepItems)
        {
            // Do nothing
        }

        public virtual List<IACObjectEntity> GetNextStepItems()
        {
            return null;
        }

        public virtual List<IACObjectEntity> GetSameStepItems()
        {
            return null;
        }

        public override string ToString()
        {
            string toString = TrackingDirection.ToString() + "|";
            toString += "[" + Item.ToString() + "]";
            if(!string.IsNullOrEmpty(SameStepParent))
            {
                toString += Environment.NewLine;
                toString += string.Format(@"| SameStepParent: {0}", SameStepParent);
            }
            if (!string.IsNullOrEmpty(NextStepParent))
            {
                toString += Environment.NewLine;
                toString += string.Format(@"| NextStepParent: {0}", NextStepParent);
            }
            return toString;
        }

        public string GetItemNo()
        {
            string itemNo = "";
            if (Item is ProdOrder)
            {
                ProdOrder prodOrder = Item as ProdOrder;
                itemNo = prodOrder.ProgramNo;
            }
            if (Item is FacilityBookingCharge)
            {
                FacilityBookingCharge fbc = Item as FacilityBookingCharge;
                itemNo = fbc.FacilityBookingChargeNo;
            }
            if (Item is FacilityBooking)
            {
                FacilityBooking fb = Item as FacilityBooking;
                itemNo = fb.FacilityBookingNo;
            }
            if (Item is ProdOrderPartslistPos)
            {
                ProdOrderPartslistPos pos = Item as ProdOrderPartslistPos;
                itemNo = pos.ToString();
            }
            if (Item is FacilityLot)
            {
                FacilityLot fl = Item as FacilityLot;
                itemNo = fl.LotNo;
            }
            if (Item is ProdOrderPartslistPosRelation)
            {
                ProdOrderPartslistPosRelation rel = Item as ProdOrderPartslistPosRelation;
                itemNo = rel.ToString();
            }
            return itemNo;
        }

        #endregion

        #endregion

        #region private methods

        
        #endregion

        #region Tree

        public string SameStepParent { get; set; }
        public string NextStepParent { get; set; }

        #endregion

    }
}
