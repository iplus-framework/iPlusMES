using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    public class TandTv3PointDN : TandTv3Point
    {

        #region override
        public DeliveryNotePosPreview DeliveryPreview { get; set; }

        public List<DeliveryNotePosPreview> OtherDeliveryPreviews { get; set; }

        public List<InOrderPosPreview> InOrderPosPreviews { get; set; }
        public List<OutOrderPosPreview> OutOrderPosPreviews { get; set; }
        public List<PickingPosPreview> PickingPosPreviews { get; set; }

        #endregion

        #region Methods
        public override void Finish()
        {
            List<PickingPos> pickingPositions = null;
            if (InOrderPositions.Any())
            {
                List<DeliveryNotePos> deliveryNotePositions = InOrderPositions.SelectMany(c => c.DeliveryNotePos_InOrderPos).ToList();
                if (deliveryNotePositions.Any())
                {
                    OtherDeliveryPreviews = deliveryNotePositions.Select(c => new DeliveryNotePosPreview(c)).ToList();
                    DeliveryPreview = OtherDeliveryPreviews.FirstOrDefault();
                }

                InOrderPosPreviews = InOrderPositions
                    .Select(c => new
                    {
                        InOrderNo = c.InOrder.InOrderNo,
                        MaterialNo = c.Material.MaterialNo,
                        MaterialName = c.Material.MaterialName1,
                        TargetQuantity = c.TargetQuantity,
                        ID = c.InOrderPosID
                    })
                    .GroupBy(c => new { c.InOrderNo, c.MaterialNo, c.MaterialName })
                    .ToList()
                    .Select(c => new InOrderPosPreview()
                    {
                        InOrderNo = c.Key.InOrderNo,
                        MaterialNo = c.Key.MaterialNo,
                        MaterialName = c.Key.MaterialName,
                        TargetQuantity = c.Sum(x => x.TargetQuantity),
                        ID = c.Select(x => x.ID).FirstOrDefault()
                    })
                    .ToList();

                pickingPositions = InOrderPositions.SelectMany(c => c.PickingPos_InOrderPos).ToList();
            }


            if (OutOrderPositions.Any())
            {
                List<DeliveryNotePos> dnss = OutOrderPositions.SelectMany(c => c.DeliveryNotePos_OutOrderPos).ToList();
                if (dnss.Any())
                {
                    OtherDeliveryPreviews = dnss.Select(c => new DeliveryNotePosPreview(c)).ToList();
                    DeliveryPreview = OtherDeliveryPreviews.FirstOrDefault();
                }

                OutOrderPosPreviews = null;
                OutOrderPosPreviews = OutOrderPositions
                   .Select(c => new
                   {
                       OutOrderNo = c.OutOrder.OutOrderNo,
                       MaterialNo = c.Material.MaterialNo,
                       MaterialName = c.Material.MaterialName1,
                       TargetQuantity = c.TargetQuantity
                   })
                   .GroupBy(c => new { c.OutOrderNo, c.MaterialNo, c.MaterialName })
                   .ToList()
                   .Select(c => new OutOrderPosPreview()
                   {
                       OutOrderNo = c.Key.OutOrderNo,
                       MaterialNo = c.Key.MaterialNo,
                       MaterialName = c.Key.MaterialName,
                       TargetQuantity = c.Sum(x => x.TargetQuantity),
                       ID = Guid.NewGuid()
                   })
                   .ToList();

                pickingPositions = OutOrderPositions.SelectMany(c => c.PickingPos_OutOrderPos).ToList();

            }

            if (pickingPositions != null && pickingPositions.Any())
            {
                PickingPosPreviews =
                    pickingPositions
                    .Select(c =>
                        new
                        {
                            PickingNo = c.Picking.PickingNo,
                            MaterialNo = c.Material.MaterialNo,
                            MaterialName = c.Material.MaterialName1,
                            TargetQuantity = c.TargetQuantityUOM
                        })
                    .GroupBy(c => new { c.PickingNo, c.MaterialNo, c.MaterialName })
                    .ToList()
                    .Select(c => new PickingPosPreview()
                    {
                        PickingNo = c.Key.PickingNo,
                        MaterialNo = c.Key.MaterialNo,
                        MaterialName = c.Key.MaterialName,
                        TargetQuantity = c.Sum(x => x.TargetQuantity),
                        ID = Guid.NewGuid()
                    })
                    .ToList();
            }

        }

        public override string ToString()
        {
            string inwardLotNo = "-";
            if (InwardLot != null)
                inwardLotNo = InwardLot.LotNo;
            return string.Format(@"MixPointDN: {0} [{1}] {2}", inwardLotNo, InwardMaterialNo, InwardMaterialName);
        }
        #endregion

    }
}
