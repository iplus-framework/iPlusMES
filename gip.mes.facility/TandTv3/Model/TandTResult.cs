using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace gip.mes.facility.TandTv3
{
    public class TandTResult
    {
        #region ctor's

        public TandTResult()
        {
            Ids = new Dictionary<Guid, string>();
            BatchIDs = new List<Guid>();
            Steps = new List<TandTStep>();
            Lots = new List<string>();
            MixPoints = new List<TandTv3Point>();
            DeliveryNotePositions = new List<DeliveryNotePos>();
            FacilityChargeIDs = new List<FacilityChargeIDModel>();
            ProgramNos = new List<string>();
            MixPointRelations = new List<MixPointRelation>();
        }

        #endregion

        #region Success && Tracking time

        public bool Success { get; set; }

        public MsgWithDetails ErrorMsg { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        #endregion

        #region Result content

        public TandTv3FilterTracking Filter { get; set; }

        public Dictionary<Guid, string> Ids { get; set; }
        public List<Guid> BatchIDs { get; set; }
        public List<string> Lots { get; set; }
        public List<TandTStep> Steps { get; set; }
        public List<TandTv3Point> MixPoints { get; set; }

        public List<DeliveryNotePos> DeliveryNotePositions { get; set; }
        public List<FacilityChargeIDModel> FacilityChargeIDs { get; set; }

        public List<DeliveryNotePosPreview> DeliveryNotes { get; set; }
        public List<FacilityChargeModel> FacilityCharges { get; set; }

        public List<string> ProgramNos { get; set; }

        public TandTStep CurrentStep { get; internal set; }

        #endregion

        #region Relations

        public List<MixPointRelation> MixPointRelations { get; set; }

        #endregion

        #region Tracking

        public StreamWriter LogWritter { get; set; }

        #endregion

        #region Machines

        public List<OrderLogRelView> OrderLogRelView { get; set; }
        public List<OrderLogPosMachines> OrderLogPosMachines { get; set; }
        public List<gip.core.datamodel.ACClass> ACClasses { get; set; }

        #endregion

        #region methods

        public void RegisterFacilityChargeID(int stepNo, Guid facilityChargeID)
        {
            if (!FacilityChargeIDs.Any(c => c.FacilityChargeID == facilityChargeID))
            {
                int maxNr = FacilityChargeIDs.Count + 1;
                FacilityChargeIDs.Add(new FacilityChargeIDModel() { FacilityChargeID = facilityChargeID, Nr = maxNr, StepNo = stepNo });
            }
        }

        #endregion

        #region Add Mix Point
        public TandTv3Point GetMixPoint(TandTv3.TandTStep step, ProdOrderPartslistPos pos)
        {
            TandTv3Point mixPoint =
                MixPoints
                .FirstOrDefault(c =>
                        c.IsProductionPoint &&
                        c.ProductionPositions.Select(x => x.ProdOrderPartslistPosID).Contains(pos.ProdOrderPartslistPosID)
                        );
            if (mixPoint == null)
            {
                FacilityLot lot = null;
                if (pos.FacilityBookingCharge_ProdOrderPartslistPos.Where(c => c.InwardFacilityChargeID != null).Any())
                {
                    lot = pos
                        .FacilityBookingCharge_ProdOrderPartslistPos
                        .Where(c => c.InwardFacilityChargeID != null)
                        .Select(c => c.InwardFacilityCharge)
                        .Where(c => c.FacilityLotID != null)
                        .Select(c => c.FacilityLot)
                        .FirstOrDefault();

                }
                if (lot == null)
                    lot = new FacilityLot() { LotNo = TandTv3Command.EmptyLotName };
                mixPoint = AddMixPoint(step, pos, lot);
            }
            return mixPoint;
        }

        public TandTv3Point AddMixPoint(TandTv3.TandTStep step, ProdOrderPartslistPos pos, FacilityLot inwardLot)
        {
            TandTv3Point mixPoint = null;
            Material bookingMaterial = pos.BookingMaterial;
            if (inwardLot != null)
            {
                mixPoint =
                    MixPoints
                    .FirstOrDefault(c =>
                        c.IsProductionPoint &&
                        c.InwardLot.LotNo == inwardLot.LotNo &&
                        c.InwardMaterialNo == bookingMaterial.MaterialNo &&
                        c.InwardLot.LotNo != TandTv3Command.EmptyLotName
                        );
            }
            else
            {
                mixPoint = MixPoints.FirstOrDefault(c => c.ProductionPositions.Select(x => x.ProdOrderPartslistPosID).Contains(pos.ProdOrderPartslistPosID));
            }
            if (mixPoint == null)
            {
                mixPoint =
                    new TandTv3Point()
                    {
                        IsProductionPoint = true,
                        Step = step,
                        PartslistSequence = pos.ProdOrderPartslist.Sequence,
                        ProgramNo = pos.ProdOrderPartslist.ProdOrder.ProgramNo,
                        ProdOrder = pos.ProdOrderPartslist.ProdOrder,
                        InwardMaterialNo = bookingMaterial.MaterialNo,
                        InwardMaterialName = bookingMaterial.MaterialName1,
                        InwardMaterial = bookingMaterial,
                        InwardBatchNo = pos.ProdOrderBatchID != null ? pos.ProdOrderBatch.ProdOrderBatchNo : ""
                    };

                if (inwardLot != null)
                {
                    mixPoint.AddInwardLot(inwardLot);
                }
                if (pos.ProdOrderBatchID != null)
                {
                    mixPoint.BatchNoList.Add(pos.ProdOrderBatch.ProdOrderBatchNo);
                }
                MixPoints.Add(mixPoint);
                CurrentStep.MixingPoints.Add(mixPoint);
            }

            if (!mixPoint.ProductionPositions.Select(x => x.ProdOrderPartslistPosID).Contains(pos.ProdOrderPartslistPosID))
                mixPoint.ProductionPositions.Add(pos);

            return mixPoint;
        }

        public TandTv3Point AddMixPoint(TandTStep step, InOrderPos inOrderPos)
        {
            FacilityLot facilityLot = inOrderPos.FacilityBookingCharge_InOrderPos.Select(c => c.InwardFacilityCharge.FacilityLot).FirstOrDefault();
            TandTv3Point mixPoint = null;
            if (facilityLot != null)
            {
                mixPoint = MixPoints.FirstOrDefault(c => c.IsInputPoint && c.InwardLot.LotNo == facilityLot.LotNo);
            }
            else
            {
                mixPoint = MixPoints.FirstOrDefault(c => c.IsInputPoint && c.InOrderPositions.Select(x => x.InOrderPosID).Contains(inOrderPos.InOrderPosID));
            }
            if (mixPoint == null)
            {
                DeliveryNotePos dns = inOrderPos.DeliveryNotePos_InOrderPos.FirstOrDefault();
                mixPoint =
                    new TandTv3PointDN()
                    {
                        IsInputPoint = true,
                        Step = step,
                        InwardMaterialNo = inOrderPos.Material.MaterialNo,
                        InwardMaterialName = inOrderPos.Material.MaterialName1,
                        InwardMaterial = inOrderPos.Material,
                        DeliveryNo = dns != null ? dns.DeliveryNote.DeliveryNoteNo : ""
                    };
                if (facilityLot != null)
                {
                    mixPoint.AddInwardLot(facilityLot);
                }
                mixPoint.InOrderPositions.Add(inOrderPos);
                MixPoints.Add(mixPoint);
                CurrentStep.MixingPoints.Add(mixPoint);
                mixPoint.IsInputPoint = true;
            }

            return mixPoint;
        }

        public TandTv3Point AddMixPoint(TandTStep step, OutOrderPos outOrderPos)
        {
            FacilityLot facilityLot = outOrderPos.FacilityBookingCharge_OutOrderPos.Select(c => c.OutwardFacilityCharge.FacilityLot).FirstOrDefault();
            TandTv3Point mixPoint =
                MixPoints
                .Where(c =>
                    c is TandTv3PointDN &&
                    c.OutOrderPositions.Select(x => x.OutOrderID).Contains(outOrderPos.OutOrderID) &&
                    c.InwardLot != null &&
                    c.InwardLot.LotNo == facilityLot.LotNo
                )
                .FirstOrDefault();
            if (mixPoint == null)
            {
                mixPoint =
                    new TandTv3PointDN()
                    {
                        IsInputPoint = true,
                        Step = step,
                        InwardMaterialNo = outOrderPos.Material.MaterialNo,
                        InwardMaterialName = outOrderPos.Material.MaterialName1
                    };
                if (facilityLot != null)
                {
                    mixPoint.AddInwardLot(facilityLot);
                }
                mixPoint.OutOrderPositions.Add(outOrderPos);
                MixPoints.Add(mixPoint);
                CurrentStep.MixingPoints.Add(mixPoint);
                mixPoint.IsInputPoint = true;
            }

            return mixPoint;
        }
        #endregion

    }
}
