using gip.mes.datamodel;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.autocomponent;
using System.Data.Objects;
using gip.mes.facility;
using gip.core.webservices;
using System.Globalization;

namespace gip.mes.webservices
{
    public partial class VBWebService
    {
        public virtual WSResponse<List<MDPickingType>> GetPickingTypes()
        {
            List<MDPickingType> result = null;
            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    result = ConvertToWSPickingType(dbApp.MDPickingType.AsQueryable()).ToList();
                }
            }
            catch (Exception e)
            {
                return new WSResponse<List<MDPickingType>>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            return new WSResponse<List<MDPickingType>>(result);
        }

        protected IEnumerable<MDPickingType> ConvertToWSPickingType(IQueryable<gip.mes.datamodel.MDPickingType> query)
        {
            return query.AsEnumerable().Select(c => new MDPickingType()
            {
                MDPickingTypeID = c.MDPickingTypeID,
                MDPickingTypeTrans = c.MDNameTrans
            });
        }



        public static readonly Func<DatabaseApp, Guid?, IQueryable<gip.mes.datamodel.Picking>> s_cQry_GetPicking =
        CompiledQuery.Compile<DatabaseApp, Guid?, IQueryable<gip.mes.datamodel.Picking>>(
            (dbApp, pickingID) =>
                dbApp.Picking
                        .Include("PickingPos_Picking")
                        .Include("PickingPos_Picking.FromFacility")
                        .Include("PickingPos_Picking.ToFacility")
                        .Include("PickingPos_Picking.InOrderPos")
                        .Include("PickingPos_Picking.InOrderPos.Material")
                        .Include("PickingPos_Picking.InOrderPos.MDUnit")
                        .Include("PickingPos_Picking.OutOrderPos")
                        .Include("PickingPos_Picking.OutOrderPos.Material")
                        .Include("PickingPos_Picking.OutOrderPos.MDUnit")
                        //.Include("PickingPos_Picking.ProdOrderPartslistPos")
                        //.Include("PickingPos_Picking.ProdOrderPartslistPos.Material")
                        //.Include("PickingPos_Picking.ProdOrderPartslistPos.MDUnit")
                        .Include("PickingPos_Picking.PickingMaterial")
                        .Include("PickingPos_Picking.PickingMaterial.BaseMDUnit")
                        .Where(c => (!pickingID.HasValue || c.PickingID == pickingID)
                                    && (pickingID.HasValue || (c.PickingStateIndex == (short)GlobalApp.PickingState.New || c.PickingStateIndex == (short)GlobalApp.PickingState.InProcess))
                              )
        );

        protected IEnumerable<Picking> ConvertToWSPicking(DatabaseApp dbApp, IQueryable<gip.mes.datamodel.Picking> query)
        {
            ACPickingManager pickingManger = null;
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost != null)
                pickingManger = ACPickingManager.GetServiceInstance(myServiceHost) as ACPickingManager;

            IEnumerable<Picking> result = query.AsEnumerable().Select(c => new Picking()
            {
                PickingID = c.PickingID,
                PickingNo = c.PickingNo,
                DeliveryDateFrom = c.DeliveryDateFrom,
                PickingType = new MDPickingType()
                {
                    MDPickingTypeID = c.MDPickingType.MDPickingTypeID,
                    MDKey = c.MDPickingType.MDKey,
                    MDPickingTypeIndex = c.MDPickingType.MDPickingTypeIndex,
                    MDPickingTypeTrans = c.MDPickingType.MDNameTrans
                },
                DeliveryCompanyAddress = new CompanyAddress()
                {
                    CompanyAddressID = c.DeliveryCompanyAddress != null ? c.DeliveryCompanyAddress.CompanyAddressID : Guid.Empty,
                    Name1 = c.DeliveryCompanyAddress != null ? c.DeliveryCompanyAddress.Name1 : "",
                    Street = c.DeliveryCompanyAddress != null ? c.DeliveryCompanyAddress.Street : "",
                    PostCode = c.DeliveryCompanyAddress != null ? c.DeliveryCompanyAddress.Postcode : ""
                },
                PickingPos_Picking = c.PickingPos_Picking
                        .ToArray()
                        .Where(d => d.Material != null && (d.ToFacilityID.HasValue || d.FromFacilityID.HasValue))
                        .Select(d => new PickingPos()
                        {
                            PickingPosID = d.PickingPosID,
                            LineNumber = d.LineNumber,
                            Material = new gip.mes.webservices.Material()
                            {

                                MaterialID = d.Material.MaterialID,
                                MaterialNo = d.Material.MaterialNo,
                                MaterialName1 = d.Material.MaterialName1
                            },
                            FromFacility = new gip.mes.webservices.Facility()
                            {
                                FacilityID = d.FromFacilityID.HasValue ? d.FromFacilityID.Value : Guid.Empty,
                                FacilityNo = d.FromFacility != null ? d.FromFacility.FacilityNo : "",
                                FacilityName = d.FromFacility != null ? d.FromFacility.FacilityName : ""
                            },
                            ToFacility = new gip.mes.webservices.Facility()
                            {
                                FacilityID = d.ToFacilityID.HasValue ? d.ToFacilityID.Value : Guid.Empty,
                                FacilityNo = d.ToFacility != null ? d.ToFacility.FacilityNo : "",
                                FacilityName = d.ToFacility != null ? d.ToFacility.FacilityName : ""
                            },
                            MDUnit = new gip.mes.webservices.MDUnit()
                            {
                                MDUnitID = d.MDUnit != null ? d.MDUnit.MDUnitID : Guid.Empty,
                                MDUnitNameTrans = d.MDUnit != null ? d.MDUnit.MDUnitNameTrans : "",
                            },
                            TargetQuantity = d.TargetQuantity,
                            TargetQuantityUOM = d.TargetQuantityUOM,
                            ActualQuantity = d.ActualQuantity,
                            ActualQuantityUOM = d.ActualQuantityUOM,
                            PostingType = DeterminePostingType(dbApp, pickingManger, d, c),
                            Comment = d.Comment
                        }).ToArray()
            });

            return result;
        }

        private PostingTypeEnum DeterminePostingType(DatabaseApp dbApp, ACPickingManager pickingManger, gip.mes.datamodel.PickingPos pos, gip.mes.datamodel.Picking picking)
        {
            if (pickingManger != null)
                return pickingManger.DeterminePostingType(dbApp, pos, picking);
            else
            {
                PostingTypeEnum postingTypeEnum = PostingTypeEnum.Relocation;
                if (picking.PickingType == GlobalApp.PickingType.Receipt
                     || picking.PickingType == GlobalApp.PickingType.ReceiptVehicle)
                    postingTypeEnum = PostingTypeEnum.Inward;
                else if (picking.PickingType == GlobalApp.PickingType.Issue)
                    //|| picking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.IssueVehicle))
                    postingTypeEnum = PostingTypeEnum.Outward;
                return postingTypeEnum;
            }
        }

        public WSResponse<List<Picking>> GetPickings()
        {
            List<Picking> result = null;
            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    result = ConvertToWSPicking(dbApp, s_cQry_GetPicking(dbApp, null)).ToList();
                }
            }
            catch (Exception e)
            {
                return new WSResponse<List<Picking>>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            return new WSResponse<List<Picking>>(result);
        }


        public WSResponse<List<Picking>> SearchPickings(string pType, string fromFacility, string toFacility, string fromDate, string toDate)
        {
            Guid mdPickingType = Guid.Empty;
            if (pType != CoreWebServiceConst.EmptyParam)
            {
                Guid.TryParse(pType, out mdPickingType);
            }

            Guid fromFacilityID = Guid.Empty;
            if (fromFacility != CoreWebServiceConst.EmptyParam)
            {
                Guid.TryParse(fromFacility, out fromFacilityID);
            }

            Guid toFacilityID = Guid.Empty;
            if (toFacility != CoreWebServiceConst.EmptyParam)
            {
                Guid.TryParse(toFacility, out toFacilityID);
            }

            DateTime? fromDT = null;
            if (fromDate != CoreWebServiceConst.EmptyParam)
            {
                DateTime temp;
                if (DateTime.TryParseExact(fromDate, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out temp))
                {
                    fromDT = temp;
                }
            }

            DateTime? toDT = null;
            if (toDate != CoreWebServiceConst.EmptyParam)
            {
                DateTime temp;
                if (DateTime.TryParseExact(toDate, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out temp))
                {
                    toDT = temp;
                }
            }

            List<Picking> result;
            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    result = ConvertToWSPicking(dbApp, s_cQry_SearchPicking(dbApp, mdPickingType, fromFacilityID, toFacilityID, fromDT, toDT)).ToList();
                }
            }
            catch (Exception e)
            {
                return new WSResponse<List<Picking>>(null, new Msg(eMsgLevel.Exception, e.Message));
            }

            return new WSResponse<List<Picking>>(result);
        }

        public static readonly Func<DatabaseApp, Guid, Guid, Guid, DateTime?, DateTime?, IQueryable<gip.mes.datamodel.Picking>> s_cQry_SearchPicking =
        CompiledQuery.Compile<DatabaseApp, Guid, Guid, Guid, DateTime?, DateTime?, IQueryable<gip.mes.datamodel.Picking>>(
            (dbApp, pType, fromFacility, toFacility, fromDate, toDate) =>
                dbApp.Picking
                        .Include("PickingPos_Picking")
                        .Include("PickingPos_Picking.FromFacility")
                        .Include("PickingPos_Picking.ToFacility")
                        .Include("PickingPos_Picking.InOrderPos")
                        .Include("PickingPos_Picking.InOrderPos.Material")
                        .Include("PickingPos_Picking.InOrderPos.MDUnit")
                        .Include("PickingPos_Picking.OutOrderPos")
                        .Include("PickingPos_Picking.OutOrderPos.Material")
                        .Include("PickingPos_Picking.OutOrderPos.MDUnit")
                        //.Include("PickingPos_Picking.ProdOrderPartslistPos")
                        //.Include("PickingPos_Picking.ProdOrderPartslistPos.Material")
                        //.Include("PickingPos_Picking.ProdOrderPartslistPos.MDUnit")
                        .Include("PickingPos_Picking.PickingMaterial")
                        .Include("PickingPos_Picking.PickingMaterial.BaseMDUnit")
                        .Where(c => (pType == Guid.Empty || c.MDPickingTypeID == pType)
                                    && ((fromFacility == Guid.Empty || (c.PickingPos_Picking.Any(x => x.FromFacilityID == fromFacility || (x.FromFacility.ParentFacilityID.HasValue && x.FromFacility.ParentFacilityID == fromFacility))))
                                    && ((toFacility == Guid.Empty || (c.PickingPos_Picking.Any(x => x.ToFacilityID == toFacility || (x.ToFacility.ParentFacilityID.HasValue && x.ToFacility.ParentFacilityID == toFacility))))
                                    && (fromDate == null || c.DeliveryDateFrom >= fromDate)
                                    && (toDate == null || c.DeliveryDateTo <= toDate)
                                    && (c.PickingStateIndex == (short)GlobalApp.PickingState.New || c.PickingStateIndex == (short)GlobalApp.PickingState.InProcess))
                              )
        ));


        public WSResponse<Picking> GetPicking(string pickingID)
        {
            if (string.IsNullOrEmpty(pickingID))
                return new WSResponse<Picking>(null, new Msg(eMsgLevel.Error, "pickingID is empty"));

            Guid guid;
            if (!Guid.TryParse(pickingID, out guid))
                return new WSResponse<Picking>(null, new Msg(eMsgLevel.Error, "pickingID is invalid"));

            Picking result = null;
            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    result = ConvertToWSPicking(dbApp, s_cQry_GetPicking(dbApp, guid)).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                return new WSResponse<Picking>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            return new WSResponse<Picking>(result);
        }


        public WSResponse<bool> InsertPicking(Picking item)
        {
            return new WSResponse<bool>(true);
        }


        public WSResponse<bool> UpdatePicking(Picking item)
        {
            if (item == null || item.PickingID == Guid.Empty)
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "item is null"));
            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    var query = dbApp.Picking.Where(c => c.PickingID == item.PickingID).FirstOrDefault();
                    // TODO:
                    return new WSResponse<bool>(query != null);
                }
            }
            catch (Exception e)
            {
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Exception, e.Message));
            }
        }

        public WSResponse<bool> DeletePicking(string pickingID)
        {
            return new WSResponse<bool>(true);
        }

        public static readonly Func<DatabaseApp, Guid, IQueryable<gip.mes.datamodel.PickingPos>> s_cQry_GetPickingPos =
        CompiledQuery.Compile<DatabaseApp, Guid, IQueryable<gip.mes.datamodel.PickingPos>>(
            (dbApp, pickingPosID) =>
                dbApp.PickingPos
                        .Include("FromFacility")
                        .Include("ToFacility")
                        .Include("InOrderPos")
                        .Include("InOrderPos.Material")
                        .Include("InOrderPos.MDUnit")
                        .Include("OutOrderPos")
                        .Include("OutOrderPos.Material")
                        .Include("OutOrderPos.MDUnit")
                        .Include("PickingPosProdOrderPartslistPos_PickingPos")
                        //.Include("ProdOrderPartslistPos.Material")
                        //.Include("ProdOrderPartslistPos.MDUnit")
                        .Include("PickingMaterial")
                        .Include("PickingMaterial.BaseMDUnit")
                        .Where(c => c.PickingPosID == pickingPosID)

        );

        protected IEnumerable<PickingPos> ConvertToWSPickingPos(IQueryable<gip.mes.datamodel.PickingPos> query)
        {
            return query.AsEnumerable().Select(d => new PickingPos()
            {
                PickingPosID = d.PickingPosID,
                LineNumber = d.LineNumber,
                Material = new gip.mes.webservices.Material()
                {
                    MaterialID = d.Material.MaterialID,
                    MaterialNo = d.Material.MaterialNo,
                    MaterialName1 = d.Material.MaterialName1
                },
                FromFacility = new gip.mes.webservices.Facility()
                {
                    FacilityID = d.FromFacilityID.HasValue ? d.FromFacilityID.Value : Guid.Empty,
                    FacilityNo = d.FromFacility != null ? d.FromFacility.FacilityNo : "",
                    FacilityName = d.FromFacility != null ? d.FromFacility.FacilityName : ""
                },
                ToFacility = new gip.mes.webservices.Facility()
                {
                    FacilityID = d.ToFacilityID.HasValue ? d.ToFacilityID.Value : Guid.Empty,
                    FacilityNo = d.ToFacility != null ? d.ToFacility.FacilityNo : "",
                    FacilityName = d.ToFacility != null ? d.ToFacility.FacilityName : ""
                },
                MDUnit = new gip.mes.webservices.MDUnit()
                {
                    MDUnitID = d.MDUnit.MDUnitID,
                    MDUnitNameTrans = d.MDUnit.MDUnitNameTrans,
                },
                TargetQuantity = d.TargetQuantity,
                TargetQuantityUOM = d.TargetQuantityUOM,
                ActualQuantity = d.ActualQuantity,
                ActualQuantityUOM = d.ActualQuantityUOM,
                Comment = d.Comment
            }).ToArray();
        }

        public WSResponse<PickingPos> GetPickingPos(string pickingPosID)
        {
            if (string.IsNullOrEmpty(pickingPosID))
                return new WSResponse<PickingPos>(null, new Msg(eMsgLevel.Error, "pickingPosID is empty"));

            Guid guid;
            if (!Guid.TryParse(pickingPosID, out guid))
                return new WSResponse<PickingPos>(null, new Msg(eMsgLevel.Error, "pickingPosID is invalid"));

            PickingPos result = null;
            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    result = ConvertToWSPickingPos(s_cQry_GetPickingPos(dbApp, guid)).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                return new WSResponse<PickingPos>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            return new WSResponse<PickingPos>(result);
        }


        public static readonly Func<DatabaseApp, datamodel.PickingPos, IQueryable<IGrouping<string, FacilityBookingCharge>>> s_cQry_FBC_ByPickingPos =
            CompiledQuery.Compile<DatabaseApp, datamodel.PickingPos, IQueryable<IGrouping<string, FacilityBookingCharge>>>(
            (ctx, pickingPos) =>
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
                .Where(fbc => (fbc.PickingPosID == pickingPos.PickingPosID || fbc.FacilityBooking.PickingPosID == pickingPos.PickingPosID)
                            || (pickingPos.InOrderPosID.HasValue && fbc.InOrderPosID == pickingPos.InOrderPosID)
                            || (pickingPos.OutOrderPosID.HasValue && fbc.OutOrderPosID == pickingPos.OutOrderPosID))
                .OrderBy(c => c.FacilityBookingChargeNo)
                .GroupBy(c => c.FacilityBooking.FacilityBookingNo)
                .OrderBy(c => c.Key)
            );


        public WSResponse<PostingOverview> GetPickingPostingsPos(string pickingPosID)
        {
            if (string.IsNullOrEmpty(pickingPosID))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "pickingPosID is empty"));

            Guid guid;
            if (!Guid.TryParse(pickingPosID, out guid))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "pickingPosID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            PostingOverview result = new PostingOverview();
            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    datamodel.PickingPos pPos = dbApp.PickingPos.FirstOrDefault(c => c.PickingPosID == guid);
                    if (pPos == null)
                    {
                        return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "The picking position with ID: can not found in the database."));
                    }

                    Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> fbList =
                        facManager.ConvertGroupedBookingsToOverviewDictionary(s_cQry_FBC_ByPickingPos(dbApp, pPos));
                    result.Postings = fbList.Keys.ToList();
                    result.PostingsFBC = fbList.SelectMany(c => c.Value).ToList();
                }
            }
            catch (Exception e)
            {
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            return new WSResponse<PostingOverview>(result);
        }


        public WSResponse<PickingPosList> GetPickingPosByMaterial(PickingPosList pickingPos)
        {
            if (pickingPos == null || !pickingPos.Any())
            {
                return new WSResponse<PickingPosList>(null, new Msg(eMsgLevel.Exception, "pickingPos is null or empty!"));
            }

            PickingPosList result;

            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    Guid[] pickIDs = pickingPos.Select(c => c.PickingPosID).ToArray();
                    result = new PickingPosList(ConvertToWSPickingPosOnlyActQuantity(dbApp.PickingPos.Where(c => pickIDs.Contains(c.PickingPosID))));
                }
            }
            catch (Exception e)
            {
                return new WSResponse<PickingPosList>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            return new WSResponse<PickingPosList>(result);
        }

        protected IEnumerable<PickingPos> ConvertToWSPickingPosOnlyActQuantity(IQueryable<gip.mes.datamodel.PickingPos> query)
        {
            return query.AsEnumerable().Select(d => new PickingPos()
            {
                PickingPosID = d.PickingPosID,
                ActualQuantity = d.ActualQuantity,
                ActualQuantityUOM = d.ActualQuantityUOM,
            }).ToArray();
        }

        public WSResponse<MsgWithDetails> FinishPickingOrder(Guid pickingID)
        {
            if (pickingID == Guid.Empty)
            {
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "pickingID is empty."));
            }

            MsgWithDetails result = null;

            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    datamodel.Picking picking = dbApp.Picking.FirstOrDefault(c => c.PickingID == pickingID);
                    if (picking == null)
                    {
                        //TODO: message
                        return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "Picking with ID: not exists in the database."));
                    }

                    PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                    ACInDeliveryNoteManager inDeliveryNoteManager = ACInDeliveryNoteManager.GetServiceInstance(myServiceHost);
                    ACOutDeliveryNoteManager outDeliveryNoteManager = ACOutDeliveryNoteManager.GetServiceInstance(myServiceHost);
                    ACPickingManager pickingManger = ACPickingManager.GetServiceInstance(myServiceHost) as ACPickingManager;
                    if (pickingManger == null)
                        return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "Picking manager instance is null!"));


                    DeliveryNote deliveryNote = null;
                    InOrder inOrder = null;
                    OutOrder outOrder = null;
                    result = pickingManger.FinishOrder(dbApp, picking, inDeliveryNoteManager, outDeliveryNoteManager, out deliveryNote, out inOrder, out outOrder, false);
                }
            }
            catch (Exception e)
            {
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Exception, e.Message));
            }

            return new WSResponse<MsgWithDetails>(result);
        }

        public WSResponse<MsgWithDetails> FinishPickingOrderWithoutCheck(Guid pickingID)
        {
            if (pickingID == Guid.Empty)
            {
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "pickingID is empty."));
            }

            MsgWithDetails result = null;

            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    datamodel.Picking picking = dbApp.Picking.FirstOrDefault(c => c.PickingID == pickingID);
                    if (picking == null)
                    {
                        //TODO: message
                        return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "Picking with ID: not exists in the database."));
                    }

                    PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                    ACInDeliveryNoteManager inDeliveryNoteManager = ACInDeliveryNoteManager.GetServiceInstance(myServiceHost);
                    ACOutDeliveryNoteManager outDeliveryNoteManager = ACOutDeliveryNoteManager.GetServiceInstance(myServiceHost);
                    ACPickingManager pickingManger = ACPickingManager.GetServiceInstance(myServiceHost) as ACPickingManager;
                    if (pickingManger == null)
                        return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "Picking manager instance is null!"));

                    DeliveryNote deliveryNote = null;
                    InOrder inOrder = null;
                    OutOrder outOrder = null;
                    result = pickingManger.FinishOrder(dbApp, picking, inDeliveryNoteManager, outDeliveryNoteManager, out deliveryNote, out inOrder, out outOrder, true);
                }
            }
            catch (Exception e)
            {
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Exception, e.Message));
            }

            return new WSResponse<MsgWithDetails>(result);
        }
    }
}
