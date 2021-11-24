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
                                    && (pickingID.HasValue || (c.PickingStateIndex == (short)GlobalApp.PickingState.New || c.PickingStateIndex == (short)GlobalApp.PickingState.InProcess)
                                    && c.PickingPos_Picking.Any(p => p.MDDelivPosLoadState.MDDelivPosLoadStateIndex <= (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive))
                              )
        );

        protected IEnumerable<Picking> ConvertToWSPicking(IQueryable<gip.mes.datamodel.Picking> query)
        {
            return query.AsEnumerable().Select(c => new Picking()
            {
                PickingID = c.PickingID,
                PickingNo = c.PickingNo,
                DeliveryDateFrom = c.DeliveryDateFrom,
                PickingPos_Picking = c.PickingPos_Picking
                        .ToArray()
                        .Where(d => d.PickingMaterialID.HasValue && d.ToFacilityID.HasValue && d.FromFacilityID.HasValue)
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
                            Comment = d.Comment
                        }).ToArray()
            });
        }

        public WSResponse<List<Picking>> GetPickings()
        {
            List<Picking> result = null;
            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    result = ConvertToWSPicking(s_cQry_GetPicking(dbApp, null)).ToList();
                }
            }
            catch (Exception e)
            {
                return new WSResponse<List<Picking>>(null, new Msg( eMsgLevel.Exception, e.Message) );
            }
            return new WSResponse<List<Picking>>(result);
        }


        public WSResponse<List<Picking>> SearchPickings(string pType, string fromFacility, string toFacility, string fromDate, string toDate)
        {
            GlobalApp.PickingType pickingType;
            short? pTypeIndex = null;
            if (pType != CoreWebServiceConst.EmptyParam && Enum.TryParse<GlobalApp.PickingType>(pType, out pickingType))
            {
                pTypeIndex = (short)pickingType;
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
                if (DateTime.TryParse(fromDate, out temp))
                {
                    fromDT = temp;
                }
            }

            DateTime? toDT = null;
            if (toDate != CoreWebServiceConst.EmptyParam)
            {
                DateTime temp;
                if (DateTime.TryParse(toDate, out temp))
                {
                    toDT = temp;
                }
            }

            List<Picking> result;
            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    result = ConvertToWSPicking(s_cQry_SearchPicking(dbApp, pTypeIndex, fromFacilityID, toFacilityID, fromDT, toDT)).ToList();
                }
            }
            catch (Exception e)
            {
                return new WSResponse<List<Picking>>(null, new Msg(eMsgLevel.Exception, e.Message));
            }

            return new WSResponse<List<Picking>>(result);
        }

        public static readonly Func<DatabaseApp, short?, Guid, Guid, DateTime?, DateTime?, IQueryable<gip.mes.datamodel.Picking>> s_cQry_SearchPicking =
        CompiledQuery.Compile<DatabaseApp, short?, Guid, Guid, DateTime?, DateTime?, IQueryable<gip.mes.datamodel.Picking>>(
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
                        .Where(c =>    (pType == null || c.MDPickingType.MDPickingTypeIndex == pType)
                                    && (fromFacility == Guid.Empty || c.PickingPos_Picking.Any(x => x.FromFacilityID == fromFacility || (x.FromFacility.ParentFacilityID.HasValue && x.FromFacility.ParentFacilityID == fromFacility))
                                    && (toFacility == Guid.Empty || c.PickingPos_Picking.Any(x => x.ToFacilityID == toFacility || (x.FromFacility.ParentFacilityID.HasValue && x.FromFacility.ParentFacilityID == fromFacility))
                                    && (fromDate == null || c.DeliveryDateFrom >= fromDate)
                                    && (toDate == null || c.DeliveryDateFrom >= toDate)
                                    && (c.PickingStateIndex == (short)GlobalApp.PickingState.New || c.PickingStateIndex == (short)GlobalApp.PickingState.InProcess)
                                    && c.PickingPos_Picking.Any(p => p.MDDelivPosLoadState.MDDelivPosLoadStateIndex <= (short)MDDelivPosLoadState.DelivPosLoadStates.LoadingActive))
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
                    result = ConvertToWSPicking(s_cQry_GetPicking(dbApp, guid)).FirstOrDefault();
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


        public static readonly Func<DatabaseApp, Guid, IQueryable<IGrouping<string, FacilityBookingCharge>>> s_cQry_FBC_ByPickingPos =
            CompiledQuery.Compile<DatabaseApp, Guid, IQueryable<IGrouping<string, FacilityBookingCharge>>>(
            (ctx, pickingPosID) =>
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
                .Where(fbc => fbc.PickingPosID == pickingPosID)
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
                    Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> fbList = 
                        facManager.ConvertGroupedBookingsToOverviewDictionary(s_cQry_FBC_ByPickingPos(dbApp, guid));
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

    }
}
