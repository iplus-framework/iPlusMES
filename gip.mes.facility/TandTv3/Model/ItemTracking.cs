using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Text;

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
            toString += "| " + GetItemNo();
            if (SameStepParent != null)
            {
                toString += Environment.NewLine;
                toString += string.Format(@"| SameStepParent: {0}", SameStepParent);
            }
            if (NextStepParent != null)
            {
                toString += Environment.NewLine;
                toString += string.Format(@"| NextStepParent: {0}", NextStepParent);
            }
            return toString;
        }

        public string ToMDString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetMDString());
            if (SameStepParent != null)
            {
                sb.AppendLine($"ParentSameStep: " + SameStepParent.GetMDLink());
            }
            if (NextStepParent != null)
            {
                sb.AppendLine($"ParentPrevousStep:" + NextStepParent.GetMDLink());
            }
            return sb.ToString();
        }

        public string GetMDString()
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine($"<h4>{GetItemNo()}</h4>");
            content.AppendLine($"<div>{Item}</h4>");
            return $"<div id=\"{GetItemID()}\">{content.ToString()}</div>";
        }

        public string GetMDLink()
        {
            return $"<a href=\"#{GetItemID()}\">{GetItemNo()}</a>";
        }

        public string GetItemNo()
        {
            string itemNo = "";

            if (Item is FacilityBookingCharge)
            {
                FacilityBookingCharge tempItem = Item as FacilityBookingCharge;
                itemNo = tempItem.FacilityBookingChargeNo;
            }

            if (Item is FacilityBooking)
            {
                FacilityBooking tempItem = Item as FacilityBooking;
                itemNo = tempItem.FacilityBookingNo;
            }

            if (Item is FacilityPreBooking)
            {
                FacilityPreBooking tempItem = Item as FacilityPreBooking;
                itemNo = tempItem.FacilityPreBookingNo;
            }

            if (Item is InOrderPos)
            {
                InOrderPos tempItem = Item as InOrderPos;
                itemNo = tempItem.InOrder.InOrderNo + " #" + tempItem.Sequence;
            }

            if (Item is ProdOrderPartslistPos)
            {
                ProdOrderPartslistPos tempItem = Item as ProdOrderPartslistPos;
                itemNo = nameof(ProdOrderPartslistPos);
            }

            if (Item is ProdOrderPartslistPosRelation)
            {
                ProdOrderPartslistPosRelation tempItem = Item as ProdOrderPartslistPosRelation;
                itemNo = nameof(ProdOrderPartslistPosRelation);
            }

            if (Item is FacilityCharge)
            {
                FacilityCharge tempItem = Item as FacilityCharge;
                itemNo = tempItem.FacilityLot?.LotNo + " | " + tempItem.Facility?.FacilityNo;
            }

            if (Item is ProdOrder)
            {
                ProdOrder tempItem = Item as ProdOrder;
                itemNo = tempItem.ProgramNo;
            }

            if (Item is ProdOrderPartslist)
            {
                ProdOrderPartslist tempItem = Item as ProdOrderPartslist;
                itemNo = tempItem.ProdOrder.ProgramNo + " #" + tempItem.Sequence;
            }

            if (Item is OutOrder)
            {
                OutOrder tempItem = Item as OutOrder;
                itemNo = tempItem.OutOrderNo;

            }

            if (Item is OutOrderPos)
            {
                OutOrderPos tempItem = Item as OutOrderPos;
                itemNo = tempItem.OutOrder.OutOrderNo + " #" + tempItem.Sequence;
            }

            if (Item is Picking)
            {
                Picking tempItem = Item as Picking;
                itemNo = tempItem.PickingNo;
            }

            if (Item is PickingPos)
            {
                PickingPos tempItem = Item as PickingPos;
                itemNo = tempItem.Picking.PickingNo + " #" + tempItem.Sequence;
            }

            if (Item is FacilityLot)
            {
                FacilityLot tempItem = Item as FacilityLot;
                itemNo = tempItem.LotNo;
            }

            if (Item is InOrder)
            {
                InOrder tempItem = Item as InOrder;
                itemNo = tempItem.InOrderNo;
            }

            if (Item is DeliveryNotePos)
            {
                DeliveryNotePos tempItem = Item as DeliveryNotePos;
                itemNo = tempItem.DeliveryNote.DeliveryNoteNo + " #" + tempItem.Sequence;
            }

            if (Item is Facility)
            {
                Facility tempItem = Item as Facility;
                itemNo = tempItem.FacilityNo;
            }

            if (Item is gip.mes.datamodel.ACClass)
            {
                gip.mes.datamodel.ACClass tempItem = Item as gip.mes.datamodel.ACClass;
                itemNo = tempItem.ACIdentifier;
            }

            if (Item is DeliveryNote)
            {
                DeliveryNote tempItem = Item as DeliveryNote;
                itemNo = tempItem.DeliveryNoteNo;
            }
            return itemNo;
        }

        public Guid GetItemID()
        {
            Guid itemID = Guid.Empty;

            if (Item is FacilityBookingCharge)
            {
                FacilityBookingCharge tempItem = Item as FacilityBookingCharge;
                itemID = tempItem.FacilityBookingChargeID;

            }
            if (Item is FacilityBooking)
            {
                FacilityBooking tempItem = Item as FacilityBooking;
                itemID = tempItem.FacilityBookingID;

            }
            if (Item is FacilityPreBooking)
            {
                FacilityPreBooking tempItem = Item as FacilityPreBooking;
                itemID = tempItem.FacilityPreBookingID;

            }
            if (Item is InOrderPos)
            {
                InOrderPos tempItem = Item as InOrderPos;
                itemID = tempItem.InOrderPosID;

            }
            if (Item is ProdOrderPartslistPos)
            {
                ProdOrderPartslistPos tempItem = Item as ProdOrderPartslistPos;
                itemID = tempItem.ProdOrderPartslistPosID;

            }
            if (Item is ProdOrderPartslistPosRelation)
            {
                ProdOrderPartslistPosRelation tempItem = Item as ProdOrderPartslistPosRelation;
                itemID = tempItem.ProdOrderPartslistPosRelationID;

            }
            if (Item is FacilityCharge)
            {
                FacilityCharge tempItem = Item as FacilityCharge;
                itemID = tempItem.FacilityChargeID;

            }
            if (Item is ProdOrder)
            {
                ProdOrder tempItem = Item as ProdOrder;
                itemID = tempItem.ProdOrderID;

            }
            if (Item is ProdOrderPartslist)
            {
                ProdOrderPartslist tempItem = Item as ProdOrderPartslist;
                itemID = tempItem.ProdOrderPartslistID;

            }
            if (Item is OutOrder)
            {
                OutOrder tempItem = Item as OutOrder;
                itemID = tempItem.OutOrderID;

            }
            if (Item is OutOrderPos)
            {
                OutOrderPos tempItem = Item as OutOrderPos;
                itemID = tempItem.OutOrderPosID;

            }
            if (Item is Picking)
            {
                Picking tempItem = Item as Picking;
                itemID = tempItem.PickingID;

            }
            if (Item is PickingPos)
            {
                PickingPos tempItem = Item as PickingPos;
                itemID = tempItem.PickingPosID;

            }
            if (Item is FacilityLot)
            {
                FacilityLot tempItem = Item as FacilityLot;
                itemID = tempItem.FacilityLotID;

            }
            if (Item is InOrder)
            {
                InOrder tempItem = Item as InOrder;
                itemID = tempItem.InOrderID;

            }
            if (Item is DeliveryNotePos)
            {
                DeliveryNotePos tempItem = Item as DeliveryNotePos;
                itemID = tempItem.DeliveryNotePosID;

            }
            if (Item is Facility)
            {
                Facility tempItem = Item as Facility;
                itemID = tempItem.FacilityID;

            }
            if (Item is gip.mes.datamodel.ACClass)
            {
                gip.mes.datamodel.ACClass tempItem = Item as gip.mes.datamodel.ACClass;
                itemID = tempItem.ACClassID;

            }
            if (Item is DeliveryNote)
            {
                DeliveryNote tempItem = Item as DeliveryNote;
                itemID = tempItem.DeliveryNoteID;

            }

            return itemID;
        }

        #endregion

        #endregion

        #region private methods


        #endregion

        #region Tree

        public IItemTracking<IACObjectEntity> SameStepParent { get; set; }
        public IItemTracking<IACObjectEntity> NextStepParent { get; set; }

        #endregion

    }
}
