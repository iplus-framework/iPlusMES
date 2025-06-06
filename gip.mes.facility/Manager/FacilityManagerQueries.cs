// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.facility
{
    /// <summary>
    /// Diese partielle Klasse beenhaltet Methoden zum Geneireren und Manipulieren
    /// von Bewegungssätzen FacilityBooking und FacilityBookingCharge
    /// </summary>
    public partial class FacilityManager
    {
        #region FacilityBooking

        static readonly Func<DatabaseApp, DateTime, DateTime, short?, Guid?, Guid?, Guid?, Guid?, Guid?, IEnumerable<IGrouping<string, FacilityBookingCharge>>> s_cQry_FacilityBookingChargeOverview =
            (ctx, searchFrom, searchTo, filterFBTypeValue, facilityID, facilityLotID, facilityChargeID, facilityLocationID, materialID) =>
                ctx
                .FacilityBookingCharge
                // Booking Include
                .Include(FacilityBooking.ClassName)
                .Include("FacilityBooking.InwardMaterial")
                .Include("FacilityBooking.OutwardMaterial")
                .Include("FacilityBooking.InwardFacility")
                .Include("FacilityBooking.OutwardFacility")
                .Include("FacilityBooking.MDMovementReason")
                .Include("FacilityBooking.InwardFacilityLot")

                // Charge include
                .Include("OutwardMaterial")
                .Include("InwardMaterial")
                .Include("InwardFacility")
                .Include("OutwardFacility")
                .Include("MDMovementReason")
                .Include("InwardFacilityCharge")
                .Include("InwardFacilityCharge.FacilityLot")
                .Include("OutwardFacilityCharge")
                .Include("OutwardFacilityCharge.FacilityLot")
                // Where cause
                .AsEnumerable()
                .Where(fbc =>
                    // filtering by period
                    fbc.FacilityBooking.InsertDate >= searchFrom
                    && fbc.FacilityBooking.InsertDate <= searchTo

                    // filterFBTypeValue
                    && (filterFBTypeValue == null || fbc.FacilityBooking.FacilityBookingTypeIndex == filterFBTypeValue)

                    // facilityID
                    && (
                        facilityID == null ||
                        ((fbc.InwardFacilityID == facilityID) || (fbc.OutwardFacilityID == facilityID))
                    )

                    // facilityLotID
                    && (
                        facilityLotID == null ||
                        (fbc.InwardFacilityLot != null && fbc.InwardFacilityLot.FacilityLotID == facilityLotID) || (fbc.OutwardFacilityLot != null && fbc.OutwardFacilityLot.FacilityLotID == facilityLotID)
                    )

                    // facilityChargeID
                    && (
                        facilityChargeID == null ||
                        ((fbc.InwardFacilityCharge != null && fbc.InwardFacilityCharge.FacilityChargeID == facilityChargeID) || (fbc.OutwardFacilityCharge != null && fbc.OutwardFacilityCharge.FacilityChargeID == facilityChargeID))
                    )

                    // facilityLocationID
                    && (
                        facilityLocationID == null ||
                        ((fbc.InwardFacility != null && fbc.InwardFacility.Facility1_ParentFacility != null && fbc.InwardFacility.Facility1_ParentFacility.FacilityID == facilityLocationID)
                            || (fbc.OutwardFacility != null && fbc.OutwardFacility.Facility1_ParentFacility != null && fbc.OutwardFacility.Facility1_ParentFacility.FacilityID == facilityLocationID))
                    )

                    // materialID
                    && (
                        materialID == null ||
                        ((fbc.InwardMaterial?.MaterialID == materialID) || (fbc.OutwardMaterial?.MaterialID == materialID))
                    )
                )
                .OrderBy(c => c.FacilityBookingChargeNo)
                .GroupBy(c => c.FacilityBooking.FacilityBookingNo)
                .OrderBy(c => c.Key);


        public virtual List<FacilityBookingOverview> GroupFacilityBookingOverviewList(IEnumerable<FacilityBookingOverview> query)
        {
            List<FacilityBookingOverview> groupedList = new List<FacilityBookingOverview>();
            var groupedQuery = query.GroupBy(p => (p.InwardMaterialNo != null) ? p.InwardMaterialNo : p.OutwardMaterialNo);

            foreach (var item in groupedQuery)
            {
                FacilityBookingOverview sumFB = new FacilityBookingOverview();
                sumFB.InwardMaterialNo = item.Select(c => c.InwardMaterialNo).FirstOrDefault();
                sumFB.InwardMaterialName = item.Select(c => c.InwardMaterialName).FirstOrDefault();
                sumFB.OutwardMaterialNo = item.Select(c => c.OutwardMaterialNo).FirstOrDefault();
                sumFB.OutwardMaterialName = item.Select(c => c.OutwardMaterialName).FirstOrDefault();
                sumFB.InsertDate = item.Select(c => c.InsertDate).FirstOrDefault();
                sumFB.FacilityBookingNo = "Ʃ";

                sumFB.InwardQuantityUOM = item.Sum(p => p.InwardQuantityUOM);
                sumFB.OutwardQuantityUOM = item.Sum(p => p.OutwardQuantityUOM);
                groupedList.AddRange(item.ToList());
                groupedList.Add(sumFB);
            }
            return groupedList.ToList();
        }

        public virtual Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> GetFacilityOverviewLists(DatabaseApp databaseApp, FacilityQueryFilter filter)
        {
            return ConvertGroupedBookingsToOverviewDictionary(
                s_cQry_FacilityBookingChargeOverview
                (
                    databaseApp,
                    filter.SearchFrom,
                    filter.SearchTo,
                    filter.FilterFBTypeValue,
                    filter.FacilityID,
                    filter.FacilityLotID,
                    filter.FacilityChargeID,
                    filter.FacilityLocationID,
                    filter.MaterialID
                 )
                 );
        }

        public Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> ConvertGroupedBookingsToOverviewDictionary(IEnumerable<IGrouping<string, FacilityBookingCharge>> query)
        {
            Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> fbList = new Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>>();
            fbList = query.ToDictionary(
                                key => key.Select(fb => new FacilityBookingOverview()
                                {
                                    FacilityBookingID = fb.FacilityBookingID,
                                    FacilityBookingNo = fb.FacilityBooking.FacilityBookingNo,
                                    InsertDate = fb.FacilityBooking.InsertDate,
                                    InsertName = fb.FacilityBooking.InsertName,

                                    OutwardQuantityUOM = fb.FacilityBooking.OutwardQuantity,
                                    OutwardMaterialNo = fb.FacilityBooking.OutwardMaterial != null ? fb.FacilityBooking.OutwardMaterial.MaterialNo : "",
                                    OutwardMaterialName = fb.FacilityBooking.OutwardMaterial != null ? fb.FacilityBooking.OutwardMaterial.MaterialName1 : "",
                                    OutwardFacilityNo = fb.FacilityBooking.OutwardFacility != null ? fb.FacilityBooking.OutwardFacility.FacilityNo : "",
                                    OutwardFacilityName = fb.FacilityBooking.OutwardFacility != null ? fb.FacilityBooking.OutwardFacility.FacilityName : "",

                                    InwardQuantityUOM = fb.FacilityBooking.InwardQuantity,
                                    InwardMaterialNo = fb.FacilityBooking.InwardMaterial != null ? fb.FacilityBooking.InwardMaterial.MaterialNo : "",
                                    InwardMaterialName = fb.FacilityBooking.InwardMaterial != null ? fb.FacilityBooking.InwardMaterial.MaterialName1 : "",
                                    InwardFacilityNo = fb.FacilityBooking.InwardFacility != null ? fb.FacilityBooking.InwardFacility.FacilityNo : "",
                                    InwardFacilityName = fb.FacilityBooking.InwardFacility != null ? fb.FacilityBooking.InwardFacility.FacilityName : "",
                                    FacilityBookingTypeIndex = fb.FacilityBooking.FacilityBookingTypeIndex,
                                    MDMovementReasonName = fb.FacilityBooking.MDMovementReason != null ? fb.FacilityBooking.MDMovementReason.MDMovementReasonName : "",
                                    Comment = fb.FacilityBooking.Comment,

                                    InwardFacilityChargeLotNo = fb.FacilityBooking.InwardFacilityChargeID != null && fb.FacilityBooking.InwardFacilityCharge.FacilityLot != null ? fb.FacilityBooking.InwardFacilityCharge.FacilityLot.LotNo : "",
                                    InwardFacilityChargeExternLotNo = fb.FacilityBooking.InwardFacilityChargeID != null && fb.FacilityBooking.InwardFacilityCharge.FacilityLot != null ? fb.FacilityBooking.InwardFacilityCharge.FacilityLot.ExternLotNo : "",
                                    InwardFacilityChargeExternLotNo2 = fb.FacilityBooking.InwardFacilityChargeID != null && fb.FacilityBooking.InwardFacilityCharge.FacilityLot != null ? fb.FacilityBooking.InwardFacilityCharge.FacilityLot.ExternLotNo2 : "",
                                    InwardFacilityChargeSplitNo = fb.FacilityBooking.InwardFacilityCharge != null ? fb.FacilityBooking.InwardFacilityCharge.SplitNo : 0,

                                    OutwardFacilityChargeLotNo = fb.FacilityBooking.OutwardFacilityCharge != null && fb.FacilityBooking.OutwardFacilityCharge.FacilityLot != null ? fb.FacilityBooking.OutwardFacilityCharge.FacilityLot.LotNo : "",
                                    OutwardFacilityChargeExternLotNo = fb.FacilityBooking.OutwardFacilityCharge != null && fb.FacilityBooking.OutwardFacilityCharge.FacilityLot != null ? fb.FacilityBooking.OutwardFacilityCharge.FacilityLot.ExternLotNo : "",
                                    OutwardFacilityChargeExternLotNo2 = fb.FacilityBooking.OutwardFacilityCharge != null && fb.FacilityBooking.OutwardFacilityCharge.FacilityLot != null ? fb.FacilityBooking.OutwardFacilityCharge.FacilityLot.ExternLotNo2 : "",
                                    OutwardFacilityChargeSplitNo = fb.FacilityBooking.OutwardFacilityCharge != null ? fb.FacilityBooking.OutwardFacilityCharge.SplitNo : 0,

                                    InwardFacilityChargeInOrderNo = fb.InOrderPosID != null ? fb.InOrderPos.InOrder.InOrderNo : "",
                                    InwardFacilityChargeProdOrderProgramNo = fb.ProdOrderPartslistPosID != null ? fb.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo : (fb.ProdOrderPartslistPosRelationID != null ? fb.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo : ""),
                                    DeliveryNoteNo = fb.InOrderPosID != null ? fb.InOrderPos.DeliveryNotePos_InOrderPos.Select(c => c.DeliveryNote.DeliveryNoteNo).FirstOrDefault() : "",
                                    PickingNo = fb.PickingPosID != null  ? fb.PickingPos.Picking.PickingNo : null

                                }).FirstOrDefault(),
                                val => val
                                .Select(fbc => new FacilityBookingChargeOverview()
                                {
                                    FacilityBookingChargeNo = fbc.FacilityBookingChargeNo,
                                    InsertDate = fbc.InsertDate,
                                    InsertName = fbc.InsertName,

                                    OutwardQuantityUOM = fbc.OutwardQuantity,
                                    OutwardMaterialNo = fbc.OutwardMaterial != null ? fbc.OutwardMaterial.MaterialNo : "",
                                    OutwardMaterialName = fbc.OutwardMaterial != null ? fbc.OutwardMaterial.MaterialName1 : "",
                                    OutwardFacilityNo = fbc.OutwardFacility != null ? fbc.OutwardFacility.FacilityNo : "",
                                    OutwardFacilityName = fbc.OutwardFacility != null ? fbc.OutwardFacility.FacilityName : "",
                                    OutwardFacilityChargeID = fbc.OutwardFacilityChargeID,

                                    InwardQuantityUOM = fbc.InwardQuantity,
                                    InwardMaterialNo = fbc.InwardMaterial != null ? fbc.InwardMaterial.MaterialNo : "",
                                    InwardMaterialName = fbc.InwardMaterial != null ? fbc.InwardMaterial.MaterialName1 : "",
                                    InwardFacilityNo = fbc.InwardFacility != null ? fbc.InwardFacility.FacilityNo : "",
                                    InwardFacilityName = fbc.InwardFacility != null ? fbc.InwardFacility.FacilityName : "",
                                    InwardFacilityPostingBehaviour = fbc.InwardFacility != null ? fbc.InwardFacility.PostingBehaviourIndex : (short)0,
                                    InwardFacilityChargeID = fbc.InwardFacilityChargeID,

                                    FacilityBookingTypeIndex = fbc.FacilityBookingTypeIndex,
                                    MDMovementReasonName = fbc.MDMovementReason != null ? fbc.MDMovementReason.MDMovementReasonName : "",
                                    Comment = fbc.Comment,

                                    InwardFacilityChargeLotNo = fbc.InwardFacilityCharge != null && fbc.InwardFacilityCharge.FacilityLot != null ? fbc.InwardFacilityCharge.FacilityLot.LotNo : "",
                                    InwardFacilityChargeExternLotNo = fbc.InwardFacilityCharge != null && fbc.InwardFacilityCharge.FacilityLot != null ? fbc.InwardFacilityCharge.FacilityLot.ExternLotNo : "",
                                    InwardFacilityChargeExternLotNo2 = fbc.InwardFacilityCharge != null && fbc.InwardFacilityCharge.FacilityLot != null ? fbc.InwardFacilityCharge.FacilityLot.ExternLotNo2 : "",
                                    InwardFacilityChargeFillingDate = fbc.InwardFacilityCharge != null && fbc.InwardFacilityCharge.FacilityLot != null ? fbc.InwardFacilityCharge.FacilityLot.FillingDate : null,
                                    InwardFacilityChargeSplitNo = fbc.InwardFacilityCharge != null ? fbc.InwardFacilityCharge.SplitNo : 0,

                                    OutwardFacilityChargeLotNo = fbc.OutwardFacilityCharge != null && fbc.OutwardFacilityCharge.FacilityLot != null ? fbc.OutwardFacilityCharge.FacilityLot.LotNo : "",
                                    OutwardFacilityChargeExternLotNo = fbc.OutwardFacilityCharge != null && fbc.OutwardFacilityCharge.FacilityLot != null ? fbc.OutwardFacilityCharge.FacilityLot.ExternLotNo : "",
                                    OutwardFacilityChargeExternLotNo2 = fbc.OutwardFacilityCharge != null && fbc.OutwardFacilityCharge.FacilityLot != null ? fbc.OutwardFacilityCharge.FacilityLot.ExternLotNo2 : "",
                                    OutwardFacilityChargeFillingDate = fbc.OutwardFacilityCharge != null && fbc.OutwardFacilityCharge.FacilityLot != null ? fbc.OutwardFacilityCharge.FacilityLot.FillingDate : null,
                                    OutwardFacilityChargeSplitNo = fbc.OutwardFacilityCharge != null ? fbc.OutwardFacilityCharge.SplitNo : 0,

                                    InwardFacilityChargeInOrderNo = fbc.InOrderPosID != null ? fbc.InOrderPos.InOrder.InOrderNo : "",
                                    InwardFacilityChargeProdOrderProgramNo = fbc.ProdOrderPartslistPosID != null ? fbc.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo :(fbc.ProdOrderPartslistPosRelationID != null ? fbc.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProgramNo : ""),
                                    DeliveryNoteNo = fbc.InOrderPosID != null ? fbc.InOrderPos.DeliveryNotePos_InOrderPos.Select(c => c.DeliveryNote.DeliveryNoteNo).FirstOrDefault() : "",
                                    PickingNo = fbc.PickingPosID != null ? fbc.PickingPos.Picking.PickingNo : null
                                })
                                .ToList());;
            foreach (var fb in fbList)
            {
                fb.Key.FacilityBookingTypeIndexName = GlobalApp.FacilityBookingTypeList.GetEntryByIndex((short)fb.Key.FacilityBookingTypeIndex).ACCaption;
                foreach (var fbc in fb.Value)
                {
                    fbc.FacilityBookingTypeIndexName = GlobalApp.FacilityBookingTypeList.GetEntryByIndex((short)fbc.FacilityBookingTypeIndex).ACCaption;
                    if (string.IsNullOrEmpty(fbc.InwardFacilityChargeProdOrderProgramNo) && !string.IsNullOrEmpty(fbc.DeliveryNoteNo))
                        fbc.InwardFacilityChargeProdOrderProgramNo = fbc.DeliveryNoteNo;
                }
            }
            return fbList;
        }

        #endregion


        #region MaterialOverview

        public static readonly Func<DatabaseApp, Guid, bool?, IEnumerable<FacilityCharge>> s_cQry_MatOverviewFacilityCharge =
        EF.CompileQuery<DatabaseApp, Guid, bool?, IEnumerable<FacilityCharge>>(
            (ctx, matID, showNotAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include("Facility.Facility1_ParentFacility")
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.MaterialID == matID && (!showNotAvailable.HasValue || c.NotAvailable == showNotAvailable.Value))
        );

        public IEnumerable<FacilityChargeSumMaterialHelper> GetFacilityChargeSumMaterialHelperList(IEnumerable<FacilityCharge> facilityChargeList, FacilityQueryFilter bookingFilter)
        {
            if (facilityChargeList == null || bookingFilter == null)
                return null;

            try
            {
                IEnumerable<FacilityChargeSumMaterialHelper> result =
                       from c in facilityChargeList
                       where c.Facility != null
                                && (!bookingFilter.HasEntityFilterSet
                                    || (bookingFilter.FacilityLocationID.HasValue && c.Facility.ParentFacilityID.HasValue && c.Facility.ParentFacilityID == bookingFilter.FacilityLocationID)
                                    || (bookingFilter.FacilityLotID.HasValue && c.FacilityLotID == bookingFilter.FacilityLotID)
                                    || (bookingFilter.FacilityID.HasValue && c.FacilityID == bookingFilter.FacilityID)
                                    || (bookingFilter.FacilityChargeID.HasValue && c.FacilityChargeID == bookingFilter.FacilityChargeID)
                                    )
                       group c by new { c.Material } into g
                       orderby g.Key.Material.MaterialNo
                       select new FacilityChargeSumMaterialHelper
                       {
                           FacilityChargeSumMaterialHelperID = Guid.NewGuid(),
                           Material = g.Key.Material,
                           SumTotal = g.Sum(o => o.StockQuantity),
                           SumBlocked = g.Sum(o => (o.MDReleaseStateID != null && o.MDReleaseState.ReleaseState == MDReleaseState.ReleaseStates.Locked) ? o.StockQuantityUOM : 0),
                           SumBlockedAbsolute = g.Sum(o => (o.MDReleaseStateID != null && o.MDReleaseState.ReleaseState == MDReleaseState.ReleaseStates.AbsLocked) ? o.StockQuantityUOM : 0),
                           SumFree = g.Sum(o => (o.MDReleaseStateID == null || o.MDReleaseState.ReleaseState <= MDReleaseState.ReleaseStates.AbsFree) ? o.StockQuantityUOM : 0),
                       };
                if (result != null)
                {
                    result = result.ToArray();
                    foreach (var item in result)
                    {
                        item.Unit = item.Material.BaseMDUnit.MDUnitName;
                        Tuple<MDUnit, double> conv = item.Material.ConvertBaseQuantity(item.SumTotal, 0);
                        item.StockUnitA = conv != null ? conv.Item2 : 0;
                        item.UnitA = conv != null ? conv.Item1.MDUnitName : null;
                        conv = item.Material.ConvertBaseQuantity(item.SumTotal, 1);
                        item.StockUnitB = conv != null ? conv.Item2 : 0;
                        item.UnitB = conv != null ? conv.Item1.MDUnitName : null;
                        conv = item.Material.ConvertBaseQuantity(item.SumTotal, 2);
                        item.StockUnitC = conv != null ? conv.Item2 : 0;
                        item.UnitC = conv != null ? conv.Item1.MDUnitName : null;
                    }
                }
                return result;

            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException(this.GetACUrl(), "GetFacilityChargeSumMaterialHelperList", msg);
            }
            return null;
        }

        #endregion


        #region LotOverview

        public static readonly Func<DatabaseApp, Guid, bool?, IEnumerable<FacilityCharge>> s_cQry_LotOverviewFacilityCharge =
            EF.CompileQuery<DatabaseApp, Guid, bool?, IEnumerable<FacilityCharge>>(
                (ctx, lotID, showNotAvailable) => ctx.FacilityCharge
                .Include(FacilityLot.ClassName)
                .Include(Facility.ClassName)
                .Include("Facility.Facility1_ParentFacility")
                .Include(Material.ClassName)
                .Include("Material.BaseMDUnit")
                .Include("Material.MaterialUnit_Material.ToMDUnit")
                .Include(MDReleaseState.ClassName)
                .Include(MDUnit.ClassName)
                .Where(c => c.FacilityLotID.HasValue && c.FacilityLotID == lotID && (!showNotAvailable.HasValue || c.NotAvailable == showNotAvailable.Value))
        );

        public IEnumerable<FacilityChargeSumLotHelper> GetFacilityChargeSumLotHelperList(IEnumerable<FacilityCharge> facilityChargeList, FacilityQueryFilter bookingFilter)
        {
            if (facilityChargeList == null || bookingFilter == null)
                return null;

            try
            {
                return from c in facilityChargeList
                       where c.FacilityLotID.HasValue
                            && c.FacilityLot != null
                            && (!bookingFilter.HasEntityFilterSet
                                || (bookingFilter.MaterialID.HasValue && c.MaterialID == bookingFilter.MaterialID)
                                || (bookingFilter.FacilityLocationID.HasValue && c.Facility != null && c.Facility.ParentFacilityID.HasValue && c.Facility.ParentFacilityID == bookingFilter.FacilityLocationID)
                                || (bookingFilter.FacilityID.HasValue && c.FacilityID == bookingFilter.FacilityID)
                                || (bookingFilter.FacilityChargeID.HasValue && c.FacilityChargeID == bookingFilter.FacilityChargeID)
                                )
                       group c by new { c.FacilityLot } into g
                       orderby g.Key.FacilityLot.LotNo
                       select new FacilityChargeSumLotHelper
                       {
                           FacilityChargeSumLotHelperID = Guid.NewGuid(),
                           FacilityLot = g.Key.FacilityLot,
                           SumTotal = g.Sum(o => o.StockQuantityUOM),
                           SumBlocked = g.Sum(o => (o.MDReleaseStateID != null && o.MDReleaseState.ReleaseState == MDReleaseState.ReleaseStates.Locked) ? o.StockQuantityUOM : 0),
                           SumBlockedAbsolute = g.Sum(o => (o.MDReleaseStateID != null && o.MDReleaseState.ReleaseState == MDReleaseState.ReleaseStates.AbsLocked) ? o.StockQuantityUOM : 0),
                           SumFree = g.Sum(o => (o.MDReleaseStateID == null || o.MDReleaseState.ReleaseState <= MDReleaseState.ReleaseStates.AbsFree) ? o.StockQuantityUOM : 0),
                       };
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException(this.GetACUrl(), "GetFacilityChargeSumLotHelperList", msg);
            }
            return null;

        }

        #endregion


        #region FacilityOverview

        public static readonly Func<DatabaseApp, Guid, bool?, IEnumerable<FacilityCharge>> s_cQry_FacilityOverviewFacilityCharge =
            EF.CompileQuery<DatabaseApp, Guid, bool?, IEnumerable<FacilityCharge>>(
                (ctx, facID, showNotAvailable) => ctx.FacilityCharge
                .Include(FacilityLot.ClassName)
                .Include(Facility.ClassName)
                .Include("Facility.Facility1_ParentFacility")
                .Include(Material.ClassName)
                .Include("Material.BaseMDUnit")
                .Include("Material.MaterialUnit_Material.ToMDUnit")
                .Include(MDReleaseState.ClassName)
                .Include(MDUnit.ClassName)
                .Where(c => c.FacilityID == facID && (!showNotAvailable.HasValue || c.NotAvailable == showNotAvailable.Value))
                .OrderBy(c => c.FacilityChargeSortNo)
        );

        public IEnumerable<FacilityChargeSumFacilityHelper> GetFacilityChargeSumFacilityHelperList(IEnumerable<FacilityCharge> facilityChargeList, FacilityQueryFilter bookingFilter)
        {
            if (facilityChargeList == null || bookingFilter == null)
                return null;

            try
            {
                return from c in facilityChargeList
                       where c.Facility != null
                                && (!bookingFilter.HasEntityFilterSet
                                    || (bookingFilter.MaterialID.HasValue && c.MaterialID == bookingFilter.MaterialID)
                                    || (bookingFilter.FacilityLotID.HasValue && c.FacilityLotID == bookingFilter.FacilityLotID)
                                    || (bookingFilter.FacilityChargeID.HasValue && c.FacilityChargeID == bookingFilter.FacilityChargeID)
                                    )
                       group c by new { c.Facility } into g
                       orderby g.Key.Facility.FacilityNo
                       select new FacilityChargeSumFacilityHelper
                       {
                           FacilityChargeSumFacilityHelperID = Guid.NewGuid(),
                           Facility = g.Key.Facility,
                           SumTotal = g.Sum(o => o.StockQuantityUOM),
                           SumBlocked = g.Sum(o => (o.MDReleaseStateID != null && o.MDReleaseState.ReleaseState == MDReleaseState.ReleaseStates.Locked) ? o.StockQuantityUOM : 0),
                           SumBlockedAbsolute = g.Sum(o => (o.MDReleaseStateID != null && o.MDReleaseState.ReleaseState == MDReleaseState.ReleaseStates.AbsLocked) ? o.StockQuantityUOM : 0),
                           SumFree = g.Sum(o => (o.MDReleaseStateID == null || o.MDReleaseState.ReleaseState <= MDReleaseState.ReleaseStates.AbsFree) ? o.StockQuantityUOM : 0),
                       };
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException(this.GetACUrl(), "GetFacilityChargeSumFacilityHelperList", msg);
            }
            return null;
        }

        #endregion


        #region LocationOverview

        public static readonly Func<DatabaseApp, Guid, bool?, IEnumerable<FacilityCharge>> s_cQry_LocationOverviewFacilityCharge =
            EF.CompileQuery<DatabaseApp, Guid, bool?, IEnumerable<FacilityCharge>>(
                (ctx, facID, showNotAvailable) => ctx.FacilityCharge
                .Include(FacilityLot.ClassName)
                .Include(Facility.ClassName)
                .Include("Facility.Facility1_ParentFacility")
                .Include(Material.ClassName)
                .Include("Material.BaseMDUnit")
                .Include("Material.MaterialUnit_Material.ToMDUnit")
                .Include(MDReleaseState.ClassName)
                .Include(MDUnit.ClassName)
                .Where(c => c.Facility != null
                        && c.Facility.Facility1_ParentFacility != null
                        && c.Facility.Facility1_ParentFacility.FacilityID == facID
                        && (!showNotAvailable.HasValue || c.NotAvailable == showNotAvailable.Value))
        );

        public IEnumerable<FacilityChargeSumLocationHelper> GetFacilityChargeSumLocationHelperList(IEnumerable<FacilityCharge> facilityChargeList, FacilityQueryFilter bookingFilter)
        {
            if (facilityChargeList == null || bookingFilter == null)
                return null;
            try
            {
                return from c in facilityChargeList
                       where c.Facility != null
                                && c.Facility.Facility1_ParentFacility != null
                                && (!bookingFilter.HasEntityFilterSet
                                    || (bookingFilter.MaterialID.HasValue && c.MaterialID == bookingFilter.MaterialID)
                                    || (bookingFilter.FacilityLotID.HasValue && c.FacilityLotID == bookingFilter.FacilityLotID)
                                    || (bookingFilter.FacilityChargeID.HasValue && c.FacilityChargeID == bookingFilter.FacilityChargeID)
                                    )
                       group c by new { c.Facility.Facility1_ParentFacility } into g
                       orderby g.Key.Facility1_ParentFacility.FacilityNo
                       select new FacilityChargeSumLocationHelper
                       {
                           FacilityChargeSumLocationHelperID = Guid.NewGuid(),
                           FacilityLocation = g.Key.Facility1_ParentFacility,
                           SumTotal = g.Sum(o => o.StockQuantityUOM),
                           SumBlocked = g.Sum(o => (o.MDReleaseStateID != null && o.MDReleaseState.ReleaseState == MDReleaseState.ReleaseStates.Locked) ? o.StockQuantityUOM : 0),
                           SumBlockedAbsolute = g.Sum(o => (o.MDReleaseStateID != null && o.MDReleaseState.ReleaseState == MDReleaseState.ReleaseStates.AbsLocked) ? o.StockQuantityUOM : 0),
                           SumFree = g.Sum(o => (o.MDReleaseStateID == null || o.MDReleaseState.ReleaseState <= MDReleaseState.ReleaseStates.AbsFree) ? o.StockQuantityUOM : 0),
                       };
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException(this.GetACUrl(), "GetFacilityChargeSumLocationHelperList", msg);
            }
            return null;
        }

        #endregion


        #region Algorithm
        public static readonly Func<DatabaseApp, Guid, Guid?, Guid, int?, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_Lot_Mat =
        EF.CompileQuery<DatabaseApp, Guid, Guid?, Guid, int?, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, facilityLotID, materialID, splitNo) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityID == facilityID
                                    && (!facilityLotID.HasValue || c.FacilityLotID == facilityLotID)
                                    && c.MaterialID == materialID
                                    && (!splitNo.HasValue || c.SplitNo == splitNo.Value))
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid?, Guid, Guid?, int?, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_Lot_Mat_Pl =
        EF.CompileQuery<DatabaseApp, Guid, Guid?, Guid, Guid?, int?, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, facilityLotID, materialID, partslistID, splitNo) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c =>    c.FacilityID == facilityID 
                                    && (!facilityLotID.HasValue || c.FacilityLotID == facilityLotID)
                                    && c.MaterialID == materialID
                                    && (!partslistID.HasValue || c.PartslistID == partslistID)
                                    && (!splitNo.HasValue || c.SplitNo == splitNo.Value))
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid, Guid?, int?, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_LotNull_Mat_Pl =
        EF.CompileQuery<DatabaseApp, Guid, Guid, Guid?, int?, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, materialID, partslistID, splitNo) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityID == facilityID
                                    && !c.FacilityLotID.HasValue
                                    && c.MaterialID == materialID
                                    && (!partslistID.HasValue || c.PartslistID == partslistID)
                                    && (!splitNo.HasValue || c.SplitNo == splitNo.Value))
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid?, Guid, Guid?, int?, IEnumerable<FacilityCharge>> s_cQry_FCList_FacLoc_Lot_Mat_Pl =
        EF.CompileQuery<DatabaseApp, Guid, Guid?, Guid, Guid?, int?, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, facilityLotID, materialID, partslistID, splitNo) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.Facility.ParentFacilityID.HasValue 
                                    && c.Facility.ParentFacilityID == facilityID
                                    && (!facilityLotID.HasValue || c.FacilityLotID == facilityLotID)
                                    && c.MaterialID == materialID
                                    && (!partslistID.HasValue || c.PartslistID == partslistID)
                                    && (!splitNo.HasValue || c.SplitNo == splitNo.Value))
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_FacLoc_ProdMat_Pl_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, materialID, prodMaterialID, partslistID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.Facility.ParentFacilityID.HasValue
                                    && c.Facility.ParentFacilityID == facilityID
                                    && (c.MaterialID == materialID
                                        || (prodMaterialID.HasValue && c.MaterialID == prodMaterialID.Value)
                                        || (c.Material.ProductionMaterialID != null && (c.Material.ProductionMaterialID == materialID || (prodMaterialID.HasValue && c.Material.ProductionMaterialID == prodMaterialID.Value)))
                                        )
                                    && (!partslistID.HasValue || c.PartslistID == partslistID)
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_FacLoc_Lot_Pl_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, facilityLotID, partslistID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.Facility.ParentFacilityID.HasValue
                                    && c.Facility.ParentFacilityID == facilityID
                                    && (!facilityLotID.HasValue || c.FacilityLotID == facilityLotID)
                                    && (!partslistID.HasValue || c.PartslistID == partslistID)
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid?, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_FacLoc_Lot_ProdMat_Pl_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, Guid?, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, facilityLotID, materialID, prodMaterialID, partslistID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.Facility.ParentFacilityID.HasValue
                                    && c.Facility.ParentFacilityID == facilityID
                                    && (!facilityLotID.HasValue || c.FacilityLotID == facilityLotID)
                                    && (c.MaterialID == materialID
                                        || (prodMaterialID.HasValue && c.MaterialID == prodMaterialID.Value)
                                        || (c.Material.ProductionMaterialID != null && (c.Material.ProductionMaterialID == materialID || (prodMaterialID.HasValue && c.Material.ProductionMaterialID == prodMaterialID.Value)))
                                        )
                                    && (!partslistID.HasValue || c.PartslistID == partslistID)
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityID == facilityID
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid?, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_Lot_ProdMat_Pl_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, Guid?, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, facilityLotID, materialID, prodMaterialID, partslistID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityID == facilityID
                                    && (!facilityLotID.HasValue || c.FacilityLotID == facilityLotID)
                                    && (c.MaterialID == materialID
                                        || (prodMaterialID.HasValue && c.MaterialID == prodMaterialID.Value)
                                        || (c.Material.ProductionMaterialID != null && (c.Material.ProductionMaterialID == materialID || (prodMaterialID.HasValue && c.Material.ProductionMaterialID == prodMaterialID.Value)))
                                        )
                                    && (!partslistID.HasValue || c.PartslistID == partslistID)
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid?, Guid, Guid?, Guid?, bool, int, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_Lot_ProdMat_Pl_NotAvailable_SplitNo =
        EF.CompileQuery<DatabaseApp, Guid, Guid?, Guid, Guid?, Guid?, bool, int, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, facilityLotID, materialID, prodMaterialID, partslistID, notAvailable, splitNo) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityID == facilityID
                                    && (!facilityLotID.HasValue || c.FacilityLotID == facilityLotID)
                                    && (c.MaterialID == materialID
                                        || (prodMaterialID.HasValue && c.MaterialID == prodMaterialID.Value)
                                        || (c.Material.ProductionMaterialID != null && (c.Material.ProductionMaterialID == materialID || (prodMaterialID.HasValue && c.Material.ProductionMaterialID == prodMaterialID.Value)))
                                        )
                                    && (!partslistID.HasValue || c.PartslistID == partslistID)
                                    && c.NotAvailable == notAvailable
                                    && c.SplitNo == splitNo)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );


        public static readonly Func<DatabaseApp, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_Lot_Pl_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, facilityLotID, partslistID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityID == facilityID
                                    && (!facilityLotID.HasValue || c.FacilityLotID == facilityLotID)
                                    && (!partslistID.HasValue || c.PartslistID == partslistID)
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid?, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_Lot_Pl_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, Guid?, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityLotID, partslistID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityLotID == facilityLotID
                                    && (!partslistID.HasValue || c.PartslistID == partslistID)
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid, Guid?, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_Lot_Mat_Pl_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, Guid, Guid?, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityLotID, materialID, partslistID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityLotID == facilityLotID
                                    && c.MaterialID == materialID
                                    && (!partslistID.HasValue || c.PartslistID == partslistID)
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid?, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_Pl_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, Guid?, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, partslistID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityID == facilityID
                                    && (!partslistID.HasValue || c.PartslistID == partslistID)
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
            );


        public static readonly Func<DatabaseApp, Guid, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_ProdMat_Pl_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, materialID, prodMaterialID, partslistID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityID == facilityID
                                    && (   c.MaterialID == materialID
                                        || (prodMaterialID.HasValue && c.MaterialID == prodMaterialID.Value)
                                        || (c.Material.ProductionMaterialID != null && (c.Material.ProductionMaterialID == materialID || (prodMaterialID.HasValue && c.Material.ProductionMaterialID == prodMaterialID.Value)))
                                        )
                                    && (!partslistID.HasValue || c.PartslistID == partslistID)
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_LastAvailable =
        EF.CompileQuery<DatabaseApp, Guid, IEnumerable<FacilityCharge>>(
            (ctx, facilityID) => ctx.FacilityBooking.Include("FacilityBookingCharge_FacilityBooking")
                                                    .Where(c => c.InwardFacilityID == facilityID && c.FacilityBookingTypeIndex == (short)GlobalApp.FacilityBookingType.ZeroStock_Facility_BulkMaterial)
                                                    .OrderByDescending(c => c.InsertDate)
                                                    .FirstOrDefault().FacilityBookingCharge_FacilityBooking.Select(x => x.InwardFacilityCharge).AsQueryable()
        );

        public static readonly Func<DatabaseApp, Guid, Guid, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_Mat_LotNotNull_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, Guid, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, materialID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityID == facilityID
                                    && c.MaterialID == materialID
                                    && c.FacilityLotID.HasValue
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_Mat_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, Guid, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, materialID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityID == facilityID
                                    && c.MaterialID == materialID
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_Lot_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityLotID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityLotID == facilityLotID
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );


        public static readonly Func<DatabaseApp, Guid, Guid, Guid?, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_Lot_ProdMat_NotAvailable =
EF.CompileQuery<DatabaseApp, Guid, Guid, Guid?, bool, IEnumerable<FacilityCharge>>(
    (ctx, facilityLotID, materialID, prodMaterialID, notAvailable) => ctx.FacilityCharge
                .Include(FacilityLot.ClassName)
                .Include(Facility.ClassName)
                .Include(Material.ClassName)
                .Include("Material.BaseMDUnit")
                .Include("Material.MaterialUnit_Material.ToMDUnit")
                .Include(MDReleaseState.ClassName)
                .Include(MDUnit.ClassName)
                .Where(c => (c.MaterialID == materialID
                                        || (prodMaterialID.HasValue && c.MaterialID == prodMaterialID.Value)
                                        || (c.Material.ProductionMaterialID != null && (c.Material.ProductionMaterialID == materialID || (prodMaterialID.HasValue && c.Material.ProductionMaterialID == prodMaterialID.Value)))
                                        )
                            && c.FacilityLotID == facilityLotID
                            && c.NotAvailable == notAvailable)
                .OrderByDescending(c => c.FillingDate)
                .ThenByDescending(c => c.FacilityChargeSortNo)
        );


        public static readonly Func<DatabaseApp, Guid, Guid, Guid?, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_FacLoc_ProdMat_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, Guid, Guid?, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, materialID, prodMaterialID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.Facility.ParentFacilityID.HasValue
                                    && c.Facility.ParentFacilityID == facilityID
                                    && (c.MaterialID == materialID
                                        || (prodMaterialID.HasValue && c.MaterialID == prodMaterialID.Value)
                                        || (c.Material.ProductionMaterialID != null && (c.Material.ProductionMaterialID == materialID || (prodMaterialID.HasValue && c.Material.ProductionMaterialID == prodMaterialID.Value)))
                                        )
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_ProdMat_NotAvailable_Retro =
        EF.CompileQuery<DatabaseApp, Guid, Guid?, Guid?, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, materialID, prodMaterialID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityID == facilityID
                                    && (   c.MaterialID == materialID
                                        || (prodMaterialID.HasValue && c.MaterialID == prodMaterialID.Value)
                                        || (c.Material.ProductionMaterialID != null && (c.Material.ProductionMaterialID == materialID || (prodMaterialID.HasValue && c.Material.ProductionMaterialID == prodMaterialID.Value)))
                                        )
                                    //&& c.IsEnabled
                                    && c.NotAvailable == notAvailable
                                    && (!c.MDReleaseStateID.HasValue || c.MDReleaseState.MDReleaseStateIndex <= (short)MDReleaseState.ReleaseStates.AbsFree)
                                    && (!c.FacilityLotID.HasValue || !c.FacilityLot.MDReleaseStateID.HasValue || c.FacilityLot.MDReleaseState.MDReleaseStateIndex <= (short)MDReleaseState.ReleaseStates.AbsFree))
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );

        public static readonly Func<DatabaseApp, Guid, Guid, bool, IEnumerable<FacilityCharge>> s_cQry_FCList_Fac_OtherPL_NotAvailable =
        EF.CompileQuery<DatabaseApp, Guid, Guid, bool, IEnumerable<FacilityCharge>>(
            (ctx, facilityID, partslistID, notAvailable) => ctx.FacilityCharge
                        .Include(FacilityLot.ClassName)
                        .Include(Facility.ClassName)
                        .Include(Material.ClassName)
                        .Include("Material.BaseMDUnit")
                        .Include("Material.MaterialUnit_Material.ToMDUnit")
                        .Include(MDReleaseState.ClassName)
                        .Include(MDUnit.ClassName)
                        .Where(c => c.FacilityID == facilityID
                                    && (   !c.PartslistID.HasValue
                                        || c.Partslist.PartslistID != partslistID)
                                    && c.NotAvailable == notAvailable)
                        .OrderByDescending(c => c.FillingDate)
                        .ThenByDescending(c => c.FacilityChargeSortNo)
        );
        #endregion

        #region MaterialReassignment

        public virtual IEnumerable<Material> GetSuggestedReassignmentMaterials(DatabaseApp dbApp, Material forMaterial)
        {
            if (dbApp == null)
                return null;

            return dbApp.Material.Where(c => c.MDMaterialGroup.MDMaterialGroupIndex == (short)MDMaterialGroup.MaterialGroupTypes.Rework).ToList();
        }

        public virtual Msg IsAllowedReassignMaterial(DatabaseApp dbApp, Material currentMaterial, Material newMaterial)
        {
            return null;
        }

        public virtual Msg IsAllowedReassignLot(DatabaseApp dbApp, FacilityLot currentLot, FacilityLot newLot)
        {
            return null;
        }

        #endregion


        #region FacilityCharges

        public virtual Func<DatabaseApp, IEnumerable<Guid>, Guid?, IEnumerable<FacilityCharge>> ManualWeighingFacilityChargeListQuery
        {
            get
            {
                return (dbApp, facility, matID) => dbApp
                                                        .FacilityCharge
                                                        .Include(c => c.FacilityLot)
                                                        .Include(c => c.Material)
                                                        .Include(c => c.Facility)
                                                        .Where(x => !x.NotAvailable && (matID == null || x.MaterialID == matID || x.Material.ProductionMaterialID == matID)
                                                                                    && facility.Contains(x.FacilityID)
                                                                                    && (x.MDReleaseStateID == null || x.MDReleaseState.MDReleaseStateIndex <= (short)MDReleaseState.ReleaseStates.AbsFree))
                                                        .ToArray()
                                                        .OrderBy(o => o.ExpirationDate);



                //return (dbApp, facility, matID) => facility.SelectMany(c => c.FacilityCharge_Facility)
                //                                    .Where(x => !x.NotAvailable && (matID == null || x.MaterialID == matID)
                //                                                                && (x.MDReleaseStateID == null || x.MDReleaseState.MDReleaseStateIndex <= (short)MDReleaseState.ReleaseStates.AbsFree))
                //                                    .ToArray().OrderBy(o => o.ExpirationDate);
            }
        }


        #endregion

    }
}
