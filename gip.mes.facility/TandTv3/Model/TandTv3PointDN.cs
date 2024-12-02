using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    public class TandTv3PointDN : TandTv3Point
    {

        #region override
        public List<DeliveryNotePosPreview> DeliveryNotePosPreviews { get; set; }

        public List<InOrderPosPreview> InOrderPosPreviews { get; set; }
        public List<OutOrderPosPreview> OutOrderPosPreviews { get; set; }
        public List<PickingPosPreview> PickingPosPreviews { get; set; }

        #endregion

        #region Methods
        public override void Finish()
        {
            if (InOrderPositions.Any())
            {
                List<DeliveryNotePos> deliveryNotePositions = InOrderPositions.SelectMany(c => c.DeliveryNotePos_InOrderPos).ToList();
                
                if (deliveryNotePositions.Any())
                    DeliveryNotePosPreviews = deliveryNotePositions.Select(c => new DeliveryNotePosPreview(c)).ToList();

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

            }

            if (OutOrderPositions.Any())
            {
                List<DeliveryNotePos> dnss = OutOrderPositions.SelectMany(c => c.DeliveryNotePos_OutOrderPos).ToList();

                if (dnss.Any())
                    DeliveryNotePosPreviews = dnss.Select(c => new DeliveryNotePosPreview(c)).ToList();

                OutOrderPosPreviews = null;
                OutOrderPosPreviews = OutOrderPositions
                   .Select(c => new
                   {
                       OutOrderNo = c.OutOrder.OutOrderNo,
                       MaterialNo = c.Material.MaterialNo,
                       MaterialName = c.Material.MaterialName1,
                       TargetQuantity = c.TargetQuantity,
                       ID = c.OutOrderPosID
                   })
                   .GroupBy(c => new { c.OutOrderNo, c.MaterialNo, c.MaterialName })
                   .ToList()
                   .Select(c => new OutOrderPosPreview()
                   {
                       OutOrderNo = c.Key.OutOrderNo,
                       MaterialNo = c.Key.MaterialNo,
                       MaterialName = c.Key.MaterialName,
                       TargetQuantity = c.Sum(x => x.TargetQuantity),
                       ID = c.Select(x => x.ID).FirstOrDefault()
                   })
                   .ToList();


            }

            if (PickingPositions != null && PickingPositions.Any())
            {
                PickingPosPreviews =
                    PickingPositions
                    .Select(c =>
                        new
                        {
                            PickingNo = c.Picking.PickingNo,
                            MaterialNo = c.Material.MaterialNo,
                            MaterialName = c.Material.MaterialName1,
                            TargetQuantity = c.TargetQuantityUOM,
                            ID = c.PickingPosID
                        })
                    .GroupBy(c => new { c.PickingNo, c.MaterialNo, c.MaterialName })
                    .ToList()
                    .Select(c => new PickingPosPreview()
                    {
                        PickingNo = c.Key.PickingNo,
                        MaterialNo = c.Key.MaterialNo,
                        MaterialName = c.Key.MaterialName,
                        TargetQuantity = c.Sum(x => x.TargetQuantity),
                        ID = c.Select(x => x.ID).FirstOrDefault()
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
