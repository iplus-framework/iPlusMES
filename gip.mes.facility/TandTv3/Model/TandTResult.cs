using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
                DeliveryNotePos dns =
                    inOrderPos
                    .InOrder
                    .InOrderPos_InOrder
                    .SelectMany(c => c.DeliveryNotePos_InOrderPos)
                    .FirstOrDefault();
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
                        InwardMaterial = outOrderPos.Material,
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

        public TandTv3Point AddMixPoint(TandTStep step, PickingPos pickingPos)
        {

            FacilityLot inwardLot =
                    pickingPos
                    .FacilityBooking_PickingPos
                    .Where(c => c.InwardFacilityCharge != null && c.InwardFacilityCharge.FacilityLot != null)
                    .Select(c => c.InwardFacilityCharge.FacilityLot)
                    .FirstOrDefault();
            if (inwardLot == null)
                inwardLot =
                    pickingPos
                    .FacilityBooking_PickingPos
                    .Where(c => c.InwardFacilityLot != null)
                    .Select(c => c.InwardFacilityLot)
                    .FirstOrDefault();

            TandTv3Point inputMixPoint =
                MixPoints
                .Where(c =>
                    c is TandTv3PointDN
                    && c.PickingPositions.Select(x => x.PickingPosID).Contains(pickingPos.PickingPosID)
                    && c.InwardMaterialNo == pickingPos.PickingMaterial.MaterialNo
                    && ((c.InwardLot == null && inwardLot == null) || (c.InwardLot.LotNo == inwardLot.LotNo))
                )
                .FirstOrDefault();

            if (inputMixPoint == null)
            {
                inputMixPoint =
                    new TandTv3PointDN()
                    {
                        IsInputPoint = true,
                        Step = step
                    };
                inputMixPoint.InwardMaterial = pickingPos.PickingMaterial;
                inputMixPoint.InwardMaterialNo = pickingPos.PickingMaterial.MaterialNo;
                inputMixPoint.InwardMaterialName = pickingPos.PickingMaterial.MaterialName1;

                inputMixPoint.PickingPositions.Add(pickingPos);
                //MixPoints.Add(inputMixPoint);
                //CurrentStep.MixingPoints.Add(inputMixPoint);
                inputMixPoint.IsInputPoint = true;
            }

            FacilityLot outwardLot =
               pickingPos
               .FacilityBooking_PickingPos
               .Where(c => c.OutwardFacilityCharge != null && c.OutwardFacilityCharge.FacilityLot != null)
               .Select(c => c.OutwardFacilityCharge.FacilityLot)
               .FirstOrDefault();
            if (outwardLot == null)
                outwardLot =
                pickingPos
                .FacilityBooking_PickingPos
                .Where(c => c.OutwardFacilityLot != null)
                .Select(c => c.OutwardFacilityLot)
                .FirstOrDefault();

            TandTv3Point outputMixPoint =
                MixPoints
                .Where(c =>
                    c is TandTv3PointDN &&
                    c.PickingPositions.Select(x => x.PickingPosID).Contains(pickingPos.PickingPosID)
                    && c.OutwardMaterials.Select(x => x.MaterialNo).Contains(pickingPos.PickingMaterial.MaterialNo)
                    && ((!c.OutwardLotsNos.Any() && outwardLot == null) || (c.OutwardLotsNos.Contains(outwardLot.LotNo)))
                )
                .FirstOrDefault();

            if (outputMixPoint == null)
            {
                outputMixPoint.OutwardMaterials.Add(pickingPos.PickingMaterial);
                outputMixPoint.PickingPositions.Add(pickingPos);
                //MixPoints.Add(outputMixPoint);
                //CurrentStep.MixingPoints.Add(outputMixPoint);
                inputMixPoint.IsInputPoint = true;


                //MixPointRelations.Add(new MixPointRelation() { SourceMixPoint = inputMixPoint, TargetMixPoint = outputMixPoint }); 
            }


            return null;
        }
        #endregion

        #region Order Depth


        private List<string> _OrderConnections;
        public List<string> OrderConnections
        {
            get
            {
                if (_OrderConnections == null)
                    _OrderConnections = new List<string>();
                return _OrderConnections;
            }
        }

        public void AddOrderConnection(string programNo1, string programNo2)
        {
            string connection = string.Format(@"\{0}\{1}", programNo1, programNo2);
            if (OrderConnections.Contains(connection))
            {
                string programNo1Start = string.Format(@"\{0}", programNo1);
                string prevConnection = OrderConnections.FirstOrDefault(c => c.Contains(programNo1Start));
                if (!string.IsNullOrEmpty(prevConnection) && prevConnection != connection)
                {
                    prevConnection = prevConnection.Substring(0, prevConnection.IndexOf(programNo1Start));
                    prevConnection += connection;
                    OrderConnections.Add(prevConnection);
                }
            }
            else
                OrderConnections.Add(connection);
        }

        public void AddOrderConnection(FacilityBookingCharge source, FacilityBookingCharge target)
        {
            if (target.ProdOrderPartslistPosRelationID != null)
            {
                if (source.ProdOrderPartslistPosID != null)
                {
                    string programNo1 = source.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                    string programNo2 = target.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo;
                    AddOrderConnection(programNo1, programNo2);
                }
            }
        }

        public int GetOrderMaxDepth()
        {
            return
                OrderConnections
                .Select(c => Regex.Matches(c, Regex.Escape(@"\")).Count)
                .DefaultIfEmpty()
                .Max();
        }

        public bool IsOrderTrackingActive()
        {
            return
                Filter.OrderDepth == null
                || GetOrderMaxDepth() < (Filter.OrderDepth ?? 0);
        }
        #endregion

    }
}
