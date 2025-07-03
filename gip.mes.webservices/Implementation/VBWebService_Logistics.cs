using gip.mes.datamodel;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return query.OrderBy(c => c.SortIndex).AsEnumerable().Select(c => new MDPickingType()
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
                                    && (pickingID.HasValue || c.PickingStateIndex <= (short)PickingStateEnum.InProcess || c.PickingStateIndex >= (short)PickingStateEnum.WFReadyToStart)
                              )
        );

        protected virtual IEnumerable<Picking> ConvertToWSPicking(DatabaseApp dbApp, IQueryable<gip.mes.datamodel.Picking> query)
        {
            FacilityManager facilityManager = null;
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost != null)
                facilityManager = FacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;

            IEnumerable<Picking> result = query.AsEnumerable().Select(c => new Picking()
            {
                PickingID = c.PickingID,
                PickingNo = c.PickingNo,
                DeliveryDateFrom = c.DeliveryDateFrom,
                Comment = c.Comment,
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
                        .OrderBy(d => d.Sequence)
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
                            PostingType = DeterminePostingType(dbApp, facilityManager, d, c),
                            Sequence = d.Sequence,
                            Comment = d.Comment
                        }).ToArray()
            });

            return result;
        }

        protected PostingTypeEnum DeterminePostingType(DatabaseApp dbApp, FacilityManager facilityManager, gip.mes.datamodel.PickingPos pos, gip.mes.datamodel.Picking picking)
        {
            if (facilityManager != null)
                return facilityManager.DeterminePostingType(dbApp, pos, picking);
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
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<Picking>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            List<Picking> result = null;
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetPickings));
            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    result = ConvertToWSPicking(dbApp, s_cQry_GetPicking(dbApp, null).Take(myServiceHost.Root.Environment.AccessDefaultTakeCount)).ToList();
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetPickings) + "(10)", e);
                return new WSResponse<List<Picking>>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetPickings));
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

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<Picking>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(SearchPickings));
            List<Picking> result;
            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    result = ConvertToWSPicking(dbApp, s_cQry_SearchPicking(dbApp, mdPickingType, fromFacilityID, toFacilityID, fromDT, toDT).Take(myServiceHost.Root.Environment.AccessDefaultTakeCount)).ToList();
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(SearchPickings) + "(10)", e);
                return new WSResponse<List<Picking>>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(SearchPickings));
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
                                    && (c.PickingStateIndex <= (short)PickingStateEnum.InProcess || c.PickingStateIndex >= (short)PickingStateEnum.WFReadyToStart))
                              )
        ));

        public WSResponse<Picking> GetPicking(string pickingID)
        {
            if (string.IsNullOrEmpty(pickingID))
                return new WSResponse<Picking>(null, new Msg(eMsgLevel.Error, "pickingID is empty"));

            Guid guid;
            if (!Guid.TryParse(pickingID, out guid))
                return new WSResponse<Picking>(null, new Msg(eMsgLevel.Error, "pickingID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<Picking>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetPicking));
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
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetPicking) + "(10)", e);
                return new WSResponse<Picking>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetPicking));
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
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(UpdatePicking));
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
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(UpdatePicking) + "(10)", e);
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(UpdatePicking));
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

        protected virtual IEnumerable<PickingPos> ConvertToWSPickingPos(IQueryable<gip.mes.datamodel.PickingPos> query, DatabaseApp dbApp, FacilityManager facilityManager)
        {
            return query.AsEnumerable().Select(d => ConvertToWSPickingPos(d, dbApp, facilityManager)).ToArray();
        }

        public PickingPos ConvertToWSPickingPos(datamodel.PickingPos pickingPos, DatabaseApp dbApp, FacilityManager facilityManager)
        {
            return new PickingPos()
            {
                PickingPosID = pickingPos.PickingPosID,
                LineNumber = pickingPos.LineNumber,
                Material = new gip.mes.webservices.Material()
                {
                    MaterialID = pickingPos.Material.MaterialID,
                    MaterialNo = pickingPos.Material.MaterialNo,
                    MaterialName1 = pickingPos.Material.MaterialName1
                },
                FromFacility = new gip.mes.webservices.Facility()
                {
                    FacilityID = pickingPos.FromFacilityID.HasValue ? pickingPos.FromFacilityID.Value : Guid.Empty,
                    FacilityNo = pickingPos.FromFacility != null ? pickingPos.FromFacility.FacilityNo : "",
                    FacilityName = pickingPos.FromFacility != null ? pickingPos.FromFacility.FacilityName : ""
                },
                ToFacility = new gip.mes.webservices.Facility()
                {
                    FacilityID = pickingPos.ToFacilityID.HasValue ? pickingPos.ToFacilityID.Value : Guid.Empty,
                    FacilityNo = pickingPos.ToFacility != null ? pickingPos.ToFacility.FacilityNo : "",
                    FacilityName = pickingPos.ToFacility != null ? pickingPos.ToFacility.FacilityName : ""
                },
                MDUnit = new gip.mes.webservices.MDUnit()
                {
                    MDUnitID = pickingPos.MDUnit.MDUnitID,
                    MDUnitNameTrans = pickingPos.MDUnit.MDUnitNameTrans,
                },
                TargetQuantity = pickingPos.TargetQuantity,
                TargetQuantityUOM = pickingPos.TargetQuantityUOM,
                ActualQuantity = pickingPos.ActualQuantity,
                ActualQuantityUOM = pickingPos.ActualQuantityUOM,
                PostingType = DeterminePostingType(dbApp, facilityManager, pickingPos, pickingPos.Picking),
                Comment = pickingPos.Comment
            };
        }

        public WSResponse<PickingPos> GetPickingPos(string pickingPosID)
        {
            if (string.IsNullOrEmpty(pickingPosID))
                return new WSResponse<PickingPos>(null, new Msg(eMsgLevel.Error, "pickingPosID is empty"));

            Guid guid;
            if (!Guid.TryParse(pickingPosID, out guid))
                return new WSResponse<PickingPos>(null, new Msg(eMsgLevel.Error, "pickingPosID is invalid"));

            FacilityManager facilityManager = null;
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<PickingPos>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            facilityManager = FacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(ConvertToWSPickingPos));
            PickingPos result = null;
            try
            {
                using (var dbApp = new DatabaseApp())
                {
                    result = ConvertToWSPickingPos(s_cQry_GetPickingPos(dbApp, guid), dbApp, facilityManager).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(ConvertToWSPickingPos) + "(10)", e);
                return new WSResponse<PickingPos>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(ConvertToWSPickingPos));
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
                .Where(fbc =>
                            fbc.PickingPosID == pickingPos.PickingPosID
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

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetPickingPostingsPos));
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
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetPickingPostingsPos) + "(10)", e);
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetPickingPostingsPos));
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
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<PickingPosList>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetPickingPosByMaterial));
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
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetPickingPosByMaterial) + "(10)", e);
                return new WSResponse<PickingPosList>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetPickingPosByMaterial));
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
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(ConvertToWSPickingPosOnlyActQuantity));

            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    var subResponse = SetDatabaseUserName<MsgWithDetails>(dbApp);
                    if (subResponse != null)
                        return subResponse;
                    datamodel.Picking picking = dbApp.Picking.FirstOrDefault(c => c.PickingID == pickingID);
                    if (picking == null)
                    {
                        //TODO: message
                        return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "Picking with ID: not exists in the database."));
                    }

                    ACInDeliveryNoteManager inDeliveryNoteManager = ACInDeliveryNoteManager.GetServiceInstance(myServiceHost);
                    ACOutDeliveryNoteManager outDeliveryNoteManager = ACOutDeliveryNoteManager.GetServiceInstance(myServiceHost);
                    FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                    ACPickingManager pickingManger = ACPickingManager.GetServiceInstance(myServiceHost) as ACPickingManager;
                    if (pickingManger == null || inDeliveryNoteManager == null || outDeliveryNoteManager == null || facManager == null)
                        return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "ACPickingManager == null || ACInDeliveryNoteManager == null || ACOutDeliveryNoteManager == null || FacilityManager == null"));


                    result = pickingManger.FinishOrder(dbApp, picking, inDeliveryNoteManager, outDeliveryNoteManager, facManager, false);
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(ConvertToWSPickingPosOnlyActQuantity) + "(10)", e);
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(ConvertToWSPickingPosOnlyActQuantity));
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
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(FinishPickingOrderWithoutCheck));

            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    var subResponse = SetDatabaseUserName<MsgWithDetails>(dbApp);
                    if (subResponse != null)
                        return subResponse;
                    datamodel.Picking picking = dbApp.Picking.FirstOrDefault(c => c.PickingID == pickingID);
                    if (picking == null)
                    {
                        //TODO: message
                        return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "Picking with ID: not exists in the database."));
                    }

                    ACInDeliveryNoteManager inDeliveryNoteManager = ACInDeliveryNoteManager.GetServiceInstance(myServiceHost);
                    ACOutDeliveryNoteManager outDeliveryNoteManager = ACOutDeliveryNoteManager.GetServiceInstance(myServiceHost);
                    FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                    ACPickingManager pickingManger = ACPickingManager.GetServiceInstance(myServiceHost) as ACPickingManager;
                    if (pickingManger == null || inDeliveryNoteManager == null || outDeliveryNoteManager == null || facManager == null)
                        return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "ACPickingManager == null || ACInDeliveryNoteManager == null || ACOutDeliveryNoteManager == null || FacilityManager == null"));

                    result = pickingManger.FinishOrder(dbApp, picking, inDeliveryNoteManager, outDeliveryNoteManager, facManager, true);
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(FinishPickingOrderWithoutCheck) + "(10)", e);
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(FinishPickingOrderWithoutCheck));
            }

            return new WSResponse<MsgWithDetails>(result);
        }

        public WSResponse<MsgWithDetails> FinishPickingOrdersByMaterial(BarcodeSequence pickingOrders)
        {
            if (pickingOrders == null)
            {
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "The parameter pickingOrders is null."));
            }

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(FinishPickingOrdersByMaterial));
            try
            {
                Guid[] pickingIDs = pickingOrders.Sequence.Where(c => c.Picking != null).Select(x => x.Picking.PickingID).ToArray();

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    var pickings = dbApp.Picking.Where(c => pickingIDs.Any(x => c.PickingID == x)).ToArray();

                    MsgWithDetails mainResult = new MsgWithDetails();

                    ACInDeliveryNoteManager inDeliveryNoteManager = ACInDeliveryNoteManager.GetServiceInstance(myServiceHost);
                    ACOutDeliveryNoteManager outDeliveryNoteManager = ACOutDeliveryNoteManager.GetServiceInstance(myServiceHost);
                    FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                    ACPickingManager pickingManger = ACPickingManager.GetServiceInstance(myServiceHost) as ACPickingManager;
                    if (pickingManger == null || inDeliveryNoteManager == null || outDeliveryNoteManager == null || facManager == null)
                        return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "ACPickingManager == null || ACInDeliveryNoteManager == null || ACOutDeliveryNoteManager == null || FacilityManager == null"));

                    foreach (datamodel.Picking picking in pickings)
                    {
                        MsgWithDetails msgWithDetails = pickingManger.FinishOrder(dbApp, picking, inDeliveryNoteManager, outDeliveryNoteManager, facManager, false);

                        if (msgWithDetails != null && msgWithDetails.MsgDetailsCount > 0)
                        {
                            mainResult.AddDetailMessage(new Msg(eMsgLevel.Error, msgWithDetails.DetailsAsText));
                        }
                    }

                    if (mainResult != null && mainResult.MsgDetailsCount > 0)
                    {
                        return new WSResponse<MsgWithDetails>(mainResult);
                    }

                    return new WSResponse<MsgWithDetails>(null, null);
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(FinishPickingOrdersByMaterial) + "(10)", e);
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(FinishPickingOrdersByMaterial));
            }
        }

        public WSResponse<MsgWithDetails> BookAndFinishPickingOrder(PickingWorkplace pickingWorkplace)
        {
            if (pickingWorkplace == null)
            {
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "pickingWorkplace is null."));
            }

            MsgWithDetails result = new MsgWithDetails();
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(BookAndFinishPickingOrder));

            try
            {
                using (Database db = new Database())
                using (DatabaseApp dbApp = new DatabaseApp(db))
                {
                    var subResponse = SetDatabaseUserName<MsgWithDetails>(dbApp);
                    if (subResponse != null)
                        return subResponse;
                    datamodel.Picking picking = dbApp.Picking.FirstOrDefault(c => c.PickingID == pickingWorkplace.PickingID);
                    if (picking == null)
                    {
                        return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "Picking with ID: not exists in the database."));
                    }

                    ACInDeliveryNoteManager inDeliveryNoteManager = ACInDeliveryNoteManager.GetServiceInstance(myServiceHost);
                    ACOutDeliveryNoteManager outDeliveryNoteManager = ACOutDeliveryNoteManager.GetServiceInstance(myServiceHost);
                    FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                    ACPickingManager pickingManger = ACPickingManager.GetServiceInstance(myServiceHost) as ACPickingManager;
                    if (pickingManger == null || inDeliveryNoteManager == null || outDeliveryNoteManager == null || facManager == null)
                        return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "ACPickingManager == null || ACInDeliveryNoteManager == null || ACOutDeliveryNoteManager == null || FacilityManager == null"));

                    if (picking.PickingType != GlobalApp.PickingType.Issue)
                    {
                        //Error50546 :This picking option is only available for a issue.
                        var msg = new Msg(pickingManger, eMsgLevel.Error, nameof(ACPickingManager), nameof(BookAndFinishPickingOrder) + "(10)", 688, "Error50546");
                        return new WSResponse<MsgWithDetails>(null, msg);
                    }

                    core.datamodel.ACClass workplace = db.ACClass.FirstOrDefault(c => c.ACClassID == pickingWorkplace.WorkplaceID);
                    if (workplace == null)
                    {
                        return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "Workplace with ID: not exists in the database."));
                    }

                    var materialConfigs = dbApp.MaterialConfig.Where(c => c.VBiACClassID == pickingWorkplace.WorkplaceID).ToArray();
                    if (materialConfigs == null || !materialConfigs.Any())
                    {
                        //Error50547: Activated quants are not found, please check are you activate all quants!
                        var msg = new Msg(pickingManger, eMsgLevel.Error, nameof(ACPickingManager), nameof(BookAndFinishPickingOrder) + "(20)", 688, "Error50547");
                        return new WSResponse<MsgWithDetails>(null, msg);
                    }

                    foreach (datamodel.PickingPos pPos in picking.PickingPos_Picking)
                    {
                        if (pPos.MDDelivPosLoadState.DelivPosLoadState < MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck)
                        {
                            datamodel.FacilityCharge fc = null;
                            Msg msg = pickingManger.GetActivatedFacilityChargeForBooking(dbApp, pPos, materialConfigs, out fc);
                            if (msg != null)
                            {
                                result.AddDetailMessage(msg);
                                continue;
                            }

                            if (pPos.RemainingDosingQuantityUOM > 0)
                                continue;

                            double reqQuantity = Math.Abs(pPos.RemainingDosingQuantityUOM);
                            if (reqQuantity < 0.000001)
                                continue;

                            Msg msgQ = pickingManger.ValidateQuantityForBooking(reqQuantity, fc.AvailableQuantity, pPos.Material.MaterialName1);

                            if (msgQ != null)
                            {
                                result.AddDetailMessage(msgQ);
                                continue;
                            }

                            ACMethodBooking aCMethodBooking = new ACMethodBooking();
                            aCMethodBooking.VirtualMethodName = gip.mes.datamodel.GlobalApp.FBT_PickingOutward;
                            aCMethodBooking.PickingPosID = pPos.PickingPosID;
                            aCMethodBooking.OutwardQuantity = Math.Abs(pPos.RemainingDosingQuantityUOM);
                            aCMethodBooking.OutwardFacilityID = fc.Facility.FacilityID;
                            aCMethodBooking.OutwardFacilityChargeID = fc.FacilityChargeID;
                            aCMethodBooking.InwardQuantity = Math.Abs(pPos.RemainingDosingQuantityUOM);

                            MsgWithDetails resultMsg = Book(aCMethodBooking, dbApp, facManager, myServiceHost);
                            if (resultMsg != null && resultMsg.MsgDetailsCount > 0)
                            {
                                resultMsg.AddDetailMessage(new Msg(eMsgLevel.Error, resultMsg.DetailsAsText));
                            }
                        }
                    }

                    if (result != null && result.MsgDetailsCount > 0)
                    {
                        return new WSResponse<MsgWithDetails>(result);
                    }

                    result = pickingManger.FinishOrder(dbApp, picking, inDeliveryNoteManager, outDeliveryNoteManager, facManager, false);
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(BookAndFinishPickingOrder) + "(10)", e);
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(BookAndFinishPickingOrder));
            }

            return new WSResponse<MsgWithDetails>(result);
        }
    }
}
