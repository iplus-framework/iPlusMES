using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.webservices
{
    public partial class VBWebService
    {
        #region FacilityCharge

        public static readonly Func<DatabaseApp, Guid?, IQueryable<FacilityCharge>> s_cQry_GetFacilityCharge =
        CompiledQuery.Compile<DatabaseApp, Guid?, IQueryable<FacilityCharge>>(
            (dbApp, facilityChargeID) =>
                dbApp.FacilityCharge
                //.Include(gip.mes.datamodel.Material.ClassName)
                //.Include(gip.mes.datamodel.FacilityLot.ClassName)
                //.Include(gip.mes.datamodel.Facility.ClassName)
                //.Include(gip.mes.datamodel.MDUnit.ClassName)
                //.Include(gip.mes.datamodel.MDReleaseState.ClassName)
                .Where(c => (!facilityChargeID.HasValue && !c.NotAvailable)
                            || (facilityChargeID.HasValue && c.FacilityChargeID == facilityChargeID.Value))
                .Select(c => new gip.mes.webservices.FacilityCharge()
                {
                    FacilityChargeID = c.FacilityChargeID,
                    Material = new gip.mes.webservices.Material()
                    {
                        MaterialID = c.MaterialID,
                        MaterialNo = c.Material.MaterialNo,
                        MaterialName1 = c.Material.MaterialName1
                    },
                    FacilityLot = new gip.mes.webservices.FacilityLot()
                    {
                        FacilityLotID = c.FacilityLotID.HasValue ? c.FacilityLotID.Value : Guid.Empty,
                        LotNo = c.FacilityLot != null ? c.FacilityLot.LotNo : "",
                        Comment = c.FacilityLot != null ? c.FacilityLot.Comment : "",
                        FillingDate = c.FacilityLot != null ? c.FacilityLot.FillingDate : null,
                        ExpirationDate = c.FacilityLot != null ? c.FacilityLot.ExpirationDate : null,
                        ProductionDate = c.FacilityLot != null ? c.FacilityLot.ProductionDate : null,
                        StorageLife = c.FacilityLot != null ? c.FacilityLot.StorageLife : (short)0,
                        ExternLotNo = c.FacilityLot != null ? c.FacilityLot.ExternLotNo : null
                    },
                    Facility = new gip.mes.webservices.Facility()
                    {
                        FacilityID = c.FacilityID,
                        FacilityName = c.Facility.FacilityName,
                        FacilityNo = c.Facility.FacilityNo
                    },
                    SplitNo = c.SplitNo,
                    StockQuantity = c.StockQuantity,
                    MDUnit = new gip.mes.webservices.MDUnit()
                    {
                        MDUnitID = c.MDUnitID,
                        MDUnitNameTrans = c.MDUnit.MDUnitNameTrans,
                        SymbolTrans = c.MDUnit.SymbolTrans
                    },
                    MDReleaseState = new gip.mes.webservices.MDReleaseState()
                    {
                        MDReleaseStateID = c.MDReleaseStateID.HasValue ? c.MDReleaseStateID.Value : Guid.Empty,
                        MDNameTrans = c.MDReleaseState != null ? c.MDReleaseState.MDNameTrans : "",
                        MDReleaseStateIndex = c.MDReleaseState != null ? c.MDReleaseState.MDReleaseStateIndex : (short)0
                    },
                    FillingDate = c.FillingDate,
                    StorageLife = c.StorageLife,
                    ProductionDate = c.ProductionDate,
                    ExpirationDate = c.ExpirationDate,
                    Comment = c.Comment,
                    NotAvailable = c.NotAvailable
                }
             )
        );

        public static readonly Func<DatabaseApp, Guid, Guid, Guid?, int?, IQueryable<FacilityCharge>> s_cQry_GetFacilityChargeFromFacilityMaterialLot =
                CompiledQuery.Compile<DatabaseApp, Guid, Guid, Guid?, int?, IQueryable<FacilityCharge>>(
                    (dbApp, facilityID, materialID, facilityLotID, splitNo) =>
                        dbApp.FacilityCharge
                        .Include(gip.mes.datamodel.Material.ClassName)
                        .Include(gip.mes.datamodel.FacilityLot.ClassName)
                        .Include(gip.mes.datamodel.Facility.ClassName)
                        .Include(gip.mes.datamodel.MDUnit.ClassName)
                        .Include(gip.mes.datamodel.MDReleaseState.ClassName)
                        .Where(c => c.FacilityID == facilityID
                                 && c.MaterialID == materialID
                                 && (facilityLotID == Guid.Empty || c.FacilityLotID == facilityLotID)
                                 && (splitNo == null || c.SplitNo == splitNo))
                        .Select(c => new gip.mes.webservices.FacilityCharge()
                        {
                            FacilityChargeID = c.FacilityChargeID,
                            Material = new gip.mes.webservices.Material()
                            {
                                MaterialID = c.MaterialID,
                                MaterialNo = c.Material.MaterialNo,
                                MaterialName1 = c.Material.MaterialName1
                            },
                            FacilityLot = new gip.mes.webservices.FacilityLot()
                            {
                                FacilityLotID = c.FacilityLotID.HasValue ? c.FacilityLotID.Value : Guid.Empty,
                                LotNo = c.FacilityLot != null ? c.FacilityLot.LotNo : "",
                                Comment = c.FacilityLot != null ? c.FacilityLot.Comment : "",
                                FillingDate = c.FacilityLot != null ? c.FacilityLot.FillingDate : null,
                                ExpirationDate = c.FacilityLot != null ? c.FacilityLot.ExpirationDate : null,
                                ProductionDate = c.FacilityLot != null ? c.FacilityLot.ProductionDate : null,
                                StorageLife = c.FacilityLot != null ? c.FacilityLot.StorageLife : (short)0,
                                ExternLotNo = c.FacilityLot != null ? c.FacilityLot.ExternLotNo : null
                            },
                            Facility = new gip.mes.webservices.Facility()
                            {
                                FacilityID = c.FacilityID,
                                FacilityName = c.Facility.FacilityName,
                                FacilityNo = c.Facility.FacilityNo
                            },
                            SplitNo = c.SplitNo,
                            StockQuantity = c.StockQuantity,
                            MDUnit = new gip.mes.webservices.MDUnit()
                            {
                                MDUnitID = c.MDUnitID,
                                MDUnitNameTrans = c.MDUnit.MDUnitNameTrans,
                                SymbolTrans = c.MDUnit.SymbolTrans
                            },
                            MDReleaseState = new gip.mes.webservices.MDReleaseState()
                            {
                                MDReleaseStateID = c.MDReleaseStateID.HasValue ? c.MDReleaseStateID.Value : Guid.Empty,
                                MDNameTrans = c.MDReleaseState != null ? c.MDReleaseState.MDNameTrans : "",
                                MDReleaseStateIndex = c.MDReleaseState != null ? c.MDReleaseState.MDReleaseStateIndex : (short)0
                            },
                            FillingDate = c.FillingDate,
                            StorageLife = c.StorageLife,
                            ProductionDate = c.ProductionDate,
                            ExpirationDate = c.ExpirationDate,
                            Comment = c.Comment,
                            NotAvailable = c.NotAvailable
                        }
                        )
                );


        public WSResponse<List<FacilityCharge>> GetFacilityCharges()
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityCharges));
                try
                {
                    return new WSResponse<List<FacilityCharge>>(s_cQry_GetFacilityCharge(dbApp, null).Take(myServiceHost.Root.Environment.AccessDefaultTakeCount).ToList());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityCharges(10)", e);
                    return new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityCharges));
                }
            }
        }

        public WSResponse<FacilityCharge> GetFacilityCharge(string facilityChargeID)
        {
            if (string.IsNullOrEmpty(facilityChargeID))
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "facilityChargeID is empty"));

            Guid guid;
            if (!Guid.TryParse(facilityChargeID, out guid))
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "facilityChargeID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityCharge));
                try
                {
                    return new WSResponse<FacilityCharge>(s_cQry_GetFacilityCharge(dbApp, guid).FirstOrDefault());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityCharge(10)", e);
                    return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityCharge));
                }
            }
        }

        public WSResponse<FacilityCharge> GetFacilityChargeByBarcode(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "barcodeID is empty"));
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityChargeByBarcode));
                try
                {
                    Guid guid = facManager.ResolveFacilityChargeIdFromBarcode(dbApp, barcodeID);
                    if (guid == Guid.Empty)
                        return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "Coudn't resolve barcodeID"));
                    return new WSResponse<FacilityCharge>(s_cQry_GetFacilityCharge(dbApp, guid).FirstOrDefault());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityChargeByBarcode(10)", e);
                    return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityChargeByBarcode));
                }
            }
        }

        public WSResponse<List<FacilityCharge>> GetRegisteredFacilityCharges(string workplaceID)
        {
            if (string.IsNullOrEmpty(workplaceID))
                return new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Error, "workplaceID is empty"));

            Guid workplaceGUID;
            if (!Guid.TryParse(workplaceID, out workplaceGUID))
            {
                return new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Error, "workplaceID is not valid GUID."));
            }

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetRegisteredFacilityCharges));
                try
                {
                    var registeredChargesID = dbApp.MaterialConfig.Where(c => c.VBiACClassID == workplaceGUID).ToArray().Select(x => x.Value as Guid?);

                    if (registeredChargesID != null && registeredChargesID.Any())
                    {
                        var charges = dbApp.FacilityCharge.Where(c => registeredChargesID.Contains(c.FacilityChargeID))
                                                            .ToArray()
                                                            .Select(c => ConvertFacilityCharge(c))
                                                            .ToList();

                        return new WSResponse<List<FacilityCharge>>(charges);
                    }
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetRegisteredFacilityCharges(10)", e);
                    return new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetRegisteredFacilityCharges));
                }
            }

            return new WSResponse<List<FacilityCharge>>(null, null);
        }

        public WSResponse<PostingOverview> GetFacilityChargeBookings(string facilityChargeID, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(facilityChargeID))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "facilityChargeID is empty"));
            Guid? sessonId = WSRestAuthorizationManager.CurrentSessionID;

            Guid guid;
            if (!Guid.TryParse(facilityChargeID, out guid))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "facilityChargeID is invalid"));
            //string dt = dt.ToUniversalTime().ToString("o");
            DateTime dtFrom;
            if (String.IsNullOrWhiteSpace(dateFrom) || !DateTime.TryParseExact(dateFrom, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFrom))
                dtFrom = DateTime.Now.AddDays(-1);

            DateTime dtTo;
            if (String.IsNullOrWhiteSpace(dateTo) || !DateTime.TryParseExact(dateTo, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtTo))
                dtTo = DateTime.Now;

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityChargeBookings));
            try
            {
                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                if (facManager == null)
                    return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    try
                    {
                        FacilityQueryFilter filter = new FacilityQueryFilter();
                        filter.SearchFrom = dtFrom;
                        filter.SearchTo = dtTo;
                        filter.FacilityChargeID = guid;

                        Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> fbList = facManager.GetFacilityOverviewLists(dbApp, filter);
                        PostingOverview po = new PostingOverview();
                        po.Postings = fbList.Keys.ToList();
                        po.PostingsFBC = fbList.SelectMany(c => c.Value).ToList();
                        return new WSResponse<PostingOverview>(po);
                    }
                    catch (Exception e)
                    {
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityChargeBookings(10)", e);
                        return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                    }
                }
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityChargeBookings));
            }
        }

        public WSResponse<FacilityCharge> GetFacilityChargeFromFacilityMaterialLot(string facilityID, string materialID, string facilityLotID, string splitNo)
        {
            if (string.IsNullOrEmpty(facilityID))
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "facilityID is empty"));

            Guid facilityGuidID;
            if (!Guid.TryParse(facilityID, out facilityGuidID))
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "facilityID is invalid"));


            if (string.IsNullOrEmpty(materialID))
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "materialID is empty"));

            Guid materialGuidID;
            if (!Guid.TryParse(materialID, out materialGuidID))
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "materialID is invalid"));



            if (string.IsNullOrEmpty(facilityLotID))
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "facilityLotID is empty"));

            Guid facilityLotGuidID;
            if (!Guid.TryParse(facilityLotID, out facilityLotGuidID))
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "facilityLotID is invalid"));

            int splitNoParsed = 0;
            if (splitNo != CoreWebServiceConst.EmptyParam && !int.TryParse(splitNo, out splitNoParsed))
            {
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "splitNo is invalid"));
            }

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityChargeFromFacilityMaterialLot));
                try
                {
                    if (splitNo == CoreWebServiceConst.EmptyParam)
                    {
                        return new WSResponse<FacilityCharge>(s_cQry_GetFacilityChargeFromFacilityMaterialLot(dbApp, facilityGuidID, materialGuidID,
                                                                                                          facilityLotGuidID, null)
                                                              .OrderByDescending(c => c.SplitNo).FirstOrDefault());
                    }
                    return new WSResponse<FacilityCharge>(s_cQry_GetFacilityChargeFromFacilityMaterialLot(dbApp, facilityGuidID, materialGuidID,
                                                                                                          facilityLotGuidID, splitNoParsed).FirstOrDefault());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityCharge(10)", e);
                    return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityChargeFromFacilityMaterialLot));
                }
            }
        }

        protected IEnumerable<FacilityCharge> ConvertFacilityChargeList(gip.mes.datamodel.FacilityCharge[] facilityChargeList)
        {
            List<FacilityCharge> resultList = new List<FacilityCharge>();
            foreach (gip.mes.datamodel.FacilityCharge vbFacilityCharge in facilityChargeList)
            {
                resultList.Add(ConvertFacilityCharge(vbFacilityCharge));
            }
            return resultList;
        }

        protected FacilityCharge ConvertFacilityCharge(gip.mes.datamodel.FacilityCharge c)
        {
            return new FacilityCharge()
            {
                FacilityChargeID = c.FacilityChargeID,
                Material = new gip.mes.webservices.Material()
                {
                    MaterialID = c.MaterialID,
                    MaterialNo = c.Material.MaterialNo,
                    MaterialName1 = c.Material.MaterialName1
                },
                FacilityLot = new gip.mes.webservices.FacilityLot()
                {
                    FacilityLotID = c.FacilityLotID.HasValue ? c.FacilityLotID.Value : Guid.Empty,
                    LotNo = c.FacilityLot != null ? c.FacilityLot.LotNo : "",
                    Comment = c.FacilityLot != null ? c.FacilityLot.Comment : "",
                    FillingDate = c.FacilityLot != null ? c.FacilityLot.FillingDate : null,
                    ExpirationDate = c.FacilityLot != null ? c.FacilityLot.ExpirationDate : null,
                    ProductionDate = c.FacilityLot != null ? c.FacilityLot.ProductionDate : null,
                    StorageLife = c.FacilityLot != null ? c.FacilityLot.StorageLife : (short)0,
                    ExternLotNo = c.FacilityLot != null ? c.FacilityLot.ExternLotNo : null
                },
                Facility = new gip.mes.webservices.Facility()
                {
                    FacilityID = c.FacilityID,
                    FacilityName = c.Facility.FacilityName,
                    FacilityNo = c.Facility.FacilityNo,
                    ParentFacilityID = c.Facility.ParentFacilityID
                },
                SplitNo = c.SplitNo,
                StockQuantity = c.StockQuantity,
                MDUnit = new gip.mes.webservices.MDUnit()
                {
                    MDUnitID = c.MDUnitID,
                    MDUnitNameTrans = c.MDUnit.MDUnitNameTrans,
                    SymbolTrans = c.MDUnit.SymbolTrans
                },
                MDReleaseState = new gip.mes.webservices.MDReleaseState()
                {
                    MDReleaseStateID = c.MDReleaseStateID.HasValue ? c.MDReleaseStateID.Value : Guid.Empty,
                    MDNameTrans = c.MDReleaseState != null ? c.MDReleaseState.MDNameTrans : "",
                    MDReleaseStateIndex = c.MDReleaseState != null ? c.MDReleaseState.MDReleaseStateIndex : (short)0
                },
                FillingDate = c.FillingDate,
                StorageLife = c.StorageLife,
                ProductionDate = c.ProductionDate,
                ExpirationDate = c.ExpirationDate,
                Comment = c.Comment,
                NotAvailable = c.NotAvailable
            };
        }

        public WSResponse<FacilityCharge> CreateFacilityCharge(FacilityChargeParamItem facilityCharge)
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(CreateFacilityCharge));
            try
            {

                if (facilityCharge == null)
                {
                    return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "parameter facilityCharge is null"));
                }

                if (facilityCharge.Material == null)
                {
                    return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "parameter facilityCharge must have material."));
                }

                if (facilityCharge.Facility == null)
                {
                    return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "parameter facilityCharge must have facility."));
                }

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    try
                    {
                        var response = SetDatabaseUserName<MsgWithDetails>(dbApp, myServiceHost);
                        if (response != null)
                            return new WSResponse<FacilityCharge>(null, response.Message);

                        Msg msg = null;

                        datamodel.Material material = dbApp.Material.FirstOrDefault(c => c.MaterialID == facilityCharge.Material.MaterialID);
                        if (material == null)
                        {
                            return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, String.Format("The material with ID:{0} not exist in database!",
                                                                                                       facilityCharge.Material.MaterialID)));
                        }

                        FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                        if (facManager == null)
                            return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

                        datamodel.FacilityInventory inventory = null;

                        if (facilityCharge.ParamID != Guid.Empty)
                        {
                            inventory = dbApp.FacilityInventory.FirstOrDefault(c => c.FacilityInventoryID == facilityCharge.ParamID);
                        }

                        if (inventory != null && inventory.FacilityID.HasValue)
                        {
                            datamodel.Facility facilityToCheck = dbApp.Facility.Include(c => c.Facility1_ParentFacility)
                                                                               .FirstOrDefault(c => c.FacilityID == facilityCharge.Facility.FacilityID);

                            while (facilityToCheck.FacilityID != inventory.FacilityID)
                            {
                                facilityToCheck = facilityToCheck.Facility1_ParentFacility;
                                if (facilityToCheck == null)
                                {
                                    return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "Quant can not be added to this facility because inventory is for another facility."));
                                }
                            }
                        }

                        datamodel.FacilityLot lot = null;
                        if (material.IsLotManaged && facilityCharge.FacilityLot != null)
                        {
                            string secondaryKey = dbApp.Root().NoManager.GetNewNo(dbApp.ContextIPlus, typeof(FacilityLot), datamodel.FacilityLot.NoColumnName,
                                                                                   datamodel.FacilityLot.FormatNewNo, myServiceHost);

                            lot = datamodel.FacilityLot.NewACObject(dbApp, null, secondaryKey);
                            lot.ExpirationDate = facilityCharge.FacilityLot.ExpirationDate;
                            lot.ExternLotNo = facilityCharge.FacilityLot.ExternLotNo;
                            dbApp.FacilityLot.AddObject(lot);

                            msg = dbApp.ACSaveChangesWithRetry();
                            if (msg != null)
                            {
                                return new WSResponse<FacilityCharge>(null, msg);
                            }
                        }

                        ACMethodBooking acMethodBooking = new ACMethodBooking();
                        acMethodBooking.VirtualMethodName = datamodel.GlobalApp.FBT_InventoryNewQuant;
                        acMethodBooking.InwardFacilityID = facilityCharge.Facility.FacilityID;
                        acMethodBooking.InwardMaterialID = facilityCharge.Material.MaterialID;
                        acMethodBooking.InwardQuantity = facilityCharge.StockQuantity;

                        if (lot != null)
                        {
                            acMethodBooking.InwardFacilityLotID = lot.FacilityLotID;
                        }

                        var bookResponse = Book(acMethodBooking, dbApp, facManager, myServiceHost);
                        if (bookResponse != null)
                        {
                            return new WSResponse<FacilityCharge>(new Msg(bookResponse.MessageLevel, bookResponse.DetailsAsText));
                        }

                        FacilityCharge fc = s_cQry_GetFacilityChargeFromFacilityMaterialLot(dbApp, acMethodBooking.InwardFacilityID.Value,
                                                                        acMethodBooking.InwardMaterialID.Value,
                                                                        lot?.FacilityLotID, acMethodBooking.InwardSplitNo).FirstOrDefault();

                        Msg invMsg = null;
                        if (inventory != null && fc != null)
                        {
                            datamodel.FacilityInventoryPos iPos = datamodel.FacilityInventoryPos.NewACObject(dbApp, inventory);
                            iPos.FacilityChargeID = fc.FacilityChargeID;
                            iPos.StockQuantity = fc.StockQuantity;
                            iPos.NewStockQuantity = fc.StockQuantity;

                            datamodel.MDFacilityInventoryPosState posState = dbApp.MDFacilityInventoryPosState
                                                                                    .FirstOrDefault(c => c.MDFacilityInventoryPosStateIndex == (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished);

                            if (posState != null)
                                iPos.MDFacilityInventoryPosState = posState;

                            invMsg = dbApp.ACSaveChanges();

                        }

                        return new WSResponse<FacilityCharge>(fc, invMsg);

                    }
                    catch (Exception e)
                    {
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(CreateFacilityCharge) + "(10)", e);
                        return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Exception, e.Message));
                    }
                }
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(CreateFacilityCharge));
            }
        }

        public WSResponse<bool> ActivateFacilityCharge(FacilityChargeParamItem activationItem)
        {
            if (activationItem == null)
            {
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "The parameter activationItem is null."));
            }

            if (activationItem.Material == null)
            {
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "In the parameter activationItem material is missing."));
            }

            if (activationItem.ParamID == Guid.Empty)
            {
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "Workplace ID is empty"));
            }

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(ActivateFacilityCharge));
            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    MaterialConfig mConfig = dbApp.MaterialConfig.FirstOrDefault(c => c.KeyACUrl == FacilityChargeParamItem.FacilityChargeActivationKeyACUrl
                                                                                   && c.VBiACClassID == activationItem.ParamID
                                                                                   && c.MaterialID == activationItem.Material.MaterialID);

                    if (mConfig == null)
                    {
                        mes.datamodel.Material material = dbApp.Material.FirstOrDefault(c => c.MaterialID == activationItem.Material.MaterialID);
                        if (material == null)
                        {
                            return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "The material not exists in the database."));
                        }

                        mConfig = MaterialConfig.NewACObject(dbApp, material);
                        mConfig.KeyACUrl = FacilityChargeParamItem.FacilityChargeActivationKeyACUrl;
                        mConfig.VBiACClassID = activationItem.ParamID;
                        mConfig.SetValueTypeACClass(dbApp.ContextIPlus.GetACType("Guid"));

                        dbApp.MaterialConfig.AddObject(mConfig);
                    }

                    mConfig.Value = activationItem.FacilityChargeID;

                    Msg msg = dbApp.ACSaveChanges();
                    if (msg != null)
                    {
                        return new WSResponse<bool>(false, msg);
                    }
                }

                return new WSResponse<bool>(true);
            }
            catch (Exception e)
            {
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(ActivateFacilityCharge));
            }
        }

        public WSResponse<bool> DeactivateFacilityCharge(FacilityChargeParamItem deactivationItem)
        {
            if (deactivationItem == null)
            {
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "The parameter deactivationItem is null"));
            }

            if (deactivationItem.Material == null)
            {
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "In the parameter activationItem material is missing."));
            }

            if (deactivationItem.ParamID == Guid.Empty)
            {
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "Workplace ID is empty"));
            }

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(DeactivateFacilityCharge));
            try
            {
                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    MaterialConfig mConfig = dbApp.MaterialConfig.FirstOrDefault(c => c.KeyACUrl == FacilityChargeParamItem.FacilityChargeActivationKeyACUrl
                                                                                   && c.VBiACClassID == deactivationItem.ParamID
                                                                                   && c.MaterialID == deactivationItem.Material.MaterialID);

                    if (mConfig == null)
                    {
                        return new WSResponse<bool>(true);
                    }

                    dbApp.MaterialConfig.DeleteObject(mConfig);

                    Msg msg = dbApp.ACSaveChanges();
                    if (msg != null)
                    {
                        return new WSResponse<bool>(false, msg);
                    }
                }

                return new WSResponse<bool>(true);
            }
            catch (Exception e)
            {
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Exception, e.Message));
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(DeactivateFacilityCharge));
            }
        }

        public WSResponse<List<FacilityCharge>> GetOperationLogFacilityCharges(string machineID)
        {
            if (string.IsNullOrEmpty(machineID))
                return new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Error, "machineID is empty"));

            Guid workplaceGUID;
            if (!Guid.TryParse(machineID, out workplaceGUID))
            {
                return new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Error, "machineID is not valid GUID."));
            }

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetOperationLogFacilityCharges));
                try
                {
                    var charges = dbApp.OperationLog.Where(c => c.RefACClassID == workplaceGUID && c.OperationState == (short)OperationLogStateEnum.Open && c.FacilityCharge != null)
                                                    .ToArray()
                                                    .Select(c => ConvertFacilityCharge(c.FacilityCharge))
                                                    .ToList();

                    return new WSResponse<List<FacilityCharge>>(charges);
                    
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetOperationLogFacilityCharges)+"(10)", e);
                    return new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetOperationLogFacilityCharges));
                }
            }
        }

        public WSResponse<BarcodeEntity> GetLastPostingOrder(string facilityChargeID)
        {
            if (string.IsNullOrEmpty(facilityChargeID))
                return new WSResponse<BarcodeEntity>(null, new Msg(eMsgLevel.Error, "facilityChargeID is empty"));
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<BarcodeEntity>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<BarcodeEntity>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetLastPostingOrder));
                try
                {
                    Guid guid = facManager.ResolveFacilityChargeIdFromBarcode(dbApp, facilityChargeID);
                    if (guid == Guid.Empty)
                        return new WSResponse<BarcodeEntity>(null, new Msg(eMsgLevel.Error, "Coudn't resolve barcodeID"));

                    gip.mes.datamodel.FacilityBookingCharge fbc = dbApp.FacilityBookingCharge.Where(c => c.OutwardFacilityChargeID == guid && (c.ProdOrderPartslistPosRelation != null || c.PickingPos != null))
                                                                           .OrderByDescending(c => c.InsertDate)
                                                                           .FirstOrDefault();

                    if (fbc == null)
                    {
                        return new WSResponse<BarcodeEntity>(null, new Msg(eMsgLevel.Error, "Not possible!"));
                    }

                    BarcodeEntity result = new BarcodeEntity();

                    if (fbc.ProdOrderPartslistPosRelation != null)
                    {
                        ProdOrderPartslistWFInfo wfInfo = new ProdOrderPartslistWFInfo();
                        wfInfo.Relation = ConvertToWSProdOrderPartslistPosRel(fbc.ProdOrderPartslistPosRelation);
                        wfInfo.IntermediateBatch = ConvertToWSProdOrderPartslistPos(fbc.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos);
                        //wfInfo.Intermediate = ConvertToWSProdOrderPartslistPos(fbc.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.TopParentPartslistPos);
                        wfInfo.ProdOrderPartslist = ConvertToWSProdOrderPartslist(fbc.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos.ProdOrderPartslist);
                        result.SelectedOrderWF = wfInfo;
                    }
                    else if (fbc.PickingPos != null)
                    {
                        result.PickingPos = ConvertToWSPickingPos(fbc.PickingPos, dbApp, facManager);
                    }

                    return new WSResponse<BarcodeEntity>(result);
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetLastPostingOrder(10)", e);
                    return new WSResponse<BarcodeEntity>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetLastPostingOrder));
                }
            }
        }

        #endregion

        #region FacilityLot

        public static readonly Func<DatabaseApp, IQueryable<FacilityLot>> s_cQry_GetFacilityLots =
        CompiledQuery.Compile<DatabaseApp, IQueryable<FacilityLot>>(
            (dbApp) =>
                dbApp.FacilityCharge
                        .Where(c => !c.NotAvailable && c.FacilityLotID.HasValue)
                        .GroupBy(c => new { c.FacilityLot.FacilityLotID, c.FacilityLot.LotNo, c.FillingDate, c.StorageLife, c.ProductionDate, c.ExpirationDate, c.Comment })
                        .Select(c => new gip.mes.webservices.FacilityLot()
                        {
                            FacilityLotID = c.Key.FacilityLotID,
                            LotNo = c.Key.LotNo,
                            FillingDate = c.Key.FillingDate,
                            StorageLife = c.Key.StorageLife,
                            ProductionDate = c.Key.ProductionDate,
                            ExpirationDate = c.Key.ExpirationDate,
                            Comment = c.Key.Comment
                        })
        );

        public static readonly Func<DatabaseApp, Guid, IQueryable<FacilityLotStock>> s_cQry_GetFacilityLotStock =
        CompiledQuery.Compile<DatabaseApp, Guid, IQueryable<FacilityLotStock>>(
            (dbApp, facilityLotID) =>
                dbApp.FacilityLotStock
                        .Where(c => c.FacilityLotID == facilityLotID)
                        .Select(c => new gip.mes.webservices.FacilityLotStock()
                        {
                            FacilityLotStockID = c.FacilityLotStockID,
                            StockQuantity = c.StockQuantity,
                            DayInward = c.DayInward,
                            DayOutward = c.DayOutward,
                            MonthInward = c.MonthInward,
                            MonthOutward = c.MonthOutward,
                            MDReleaseState = new gip.mes.webservices.MDReleaseState()
                            {
                                MDReleaseStateID = c.MDReleaseStateID.HasValue ? c.MDReleaseStateID.Value : Guid.Empty,
                                MDNameTrans = c.MDReleaseState != null ? c.MDReleaseState.MDNameTrans : ""
                            },
                        })
        );

        public WSResponse<List<FacilityLot>> GetFacilityLots()
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<FacilityLot>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityLots));
                try
                {
                    return new WSResponse<List<FacilityLot>>(s_cQry_GetFacilityLots(dbApp).Take(myServiceHost.Root.Environment.AccessDefaultTakeCount).ToList());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "FacilityLot(10)", e);
                    return new WSResponse<List<FacilityLot>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityLots));
                }
            }
        }


        public static readonly Func<DatabaseApp, Guid?, string, IQueryable<FacilityLot>> s_cQry_GetFacilityLot =
        CompiledQuery.Compile<DatabaseApp, Guid?, string, IQueryable<FacilityLot>>(
            (dbApp, facilityLotID, facilityLotNo) =>
                dbApp.FacilityLot
                .Where(c => (!facilityLotID.HasValue || c.FacilityLotID == facilityLotID)
                            && (facilityLotNo == null || c.LotNo.Contains(facilityLotNo)))
                .Select(c => new gip.mes.webservices.FacilityLot()
                {
                    FacilityLotID = c.FacilityLotID,
                    LotNo = c.LotNo,
                    FillingDate = c.FillingDate,
                    StorageLife = c.StorageLife,
                    ProductionDate = c.ProductionDate,
                    ExpirationDate = c.ExpirationDate,
                    ExternLotNo = c.ExternLotNo,
                    Comment = c.Comment
                }
             )
        );

        public static readonly Func<DatabaseApp, string, IQueryable<FacilityLot>> s_cQry_GetFacilityLotByMaterial =
        CompiledQuery.Compile<DatabaseApp, string, IQueryable<FacilityLot>>(
            (dbApp, materialNo) =>
                dbApp.FacilityLot
                .Include(nameof(Material))
                .Where(c => c.Material.MaterialNo == materialNo)
                .Select(c => new gip.mes.webservices.FacilityLot()
                {
                    FacilityLotID = c.FacilityLotID,
                    LotNo = c.LotNo,
                    FillingDate = c.FillingDate,
                    StorageLife = c.StorageLife,
                    ProductionDate = c.ProductionDate,
                    ExpirationDate = c.ExpirationDate,
                    ExternLotNo = c.ExternLotNo,
                    Comment = c.Comment
                }
             )
        );

        public WSResponse<FacilityLot> GetFacilityLot(string facilityLotID)
        {
            if (string.IsNullOrEmpty(facilityLotID))
                return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Error, "facilityLotID is empty"));

            Guid guid;
            if (!Guid.TryParse(facilityLotID, out guid))
                return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Error, "facilityLotID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityLot));
                try
                {
                    return new WSResponse<FacilityLot>(s_cQry_GetFacilityLot(dbApp, guid, null).FirstOrDefault());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLot(10)", e);
                    return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityLot));
                }
            }
        }


        public WSResponse<List<FacilityLot>> SearchFacilityLot(string term)
        {
            if (string.IsNullOrEmpty(term) || term == CoreWebServiceConst.EmptyParam)
                term = null;

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<FacilityLot>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(SearchFacilityLot));
                try
                {
                    return new WSResponse<List<FacilityLot>>(s_cQry_GetFacilityLot(dbApp, null, term).Take(myServiceHost.Root.Environment.AccessDefaultTakeCount).ToList());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(SearchFacilityLot) + "(10)", e);
                    return new WSResponse<List<FacilityLot>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(SearchFacilityLot));
                }
            }
        }

        public WSResponse<List<FacilityLot>> SearchFacilityLotByMaterial(string materialNo)
        {
            if (string.IsNullOrEmpty(materialNo) || materialNo == CoreWebServiceConst.EmptyParam)
                return new WSResponse<List<FacilityLot>>(null, new Msg(eMsgLevel.Exception, "materialNo is empty."));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<FacilityLot>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(SearchFacilityLotByMaterial));
                try
                {
                    return new WSResponse<List<FacilityLot>>(s_cQry_GetFacilityLotByMaterial(dbApp, materialNo).Take(myServiceHost.Root.Environment.AccessDefaultTakeCount).ToList());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(SearchFacilityLotByMaterial) + "(10)", e);
                    return new WSResponse<List<FacilityLot>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(SearchFacilityLotByMaterial));
                }
            }
        }


        public WSResponse<FacilityLot> GetFacilityLotByBarcode(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Error, "barcodeID is empty"));
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityLotByBarcode));
                try
                {
                    Guid guid = facManager.ResolveFacilityLotIdFromBarcode(dbApp, barcodeID);
                    if (guid == Guid.Empty)
                        return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Error, "Coudn't resolve barcodeID"));
                    return new WSResponse<FacilityLot>(s_cQry_GetFacilityLot(dbApp, guid, null).FirstOrDefault());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLotByBarcode(10)", e);
                    return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityLotByBarcode));
                }
            }
        }


        public WSResponse<FacilityLotSumOverview> GetFacilityLotSum(string facilityLotID)
        {
            if (string.IsNullOrEmpty(facilityLotID))
                return new WSResponse<FacilityLotSumOverview>(null, new Msg(eMsgLevel.Error, "facilityLotID is empty"));

            Guid guid;
            if (!Guid.TryParse(facilityLotID, out guid))
                return new WSResponse<FacilityLotSumOverview>(null, new Msg(eMsgLevel.Error, "facilityLotID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<FacilityLotSumOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityLotSum));
            try
            {

                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                if (facManager == null)
                    return new WSResponse<FacilityLotSumOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    try
                    {
                        FacilityLotSumOverview overview = new FacilityLotSumOverview();
                        overview.FacilityLotStock = s_cQry_GetFacilityLotStock(dbApp, guid).FirstOrDefault();
                        gip.mes.datamodel.FacilityCharge[] facilityChargeList = FacilityManager.s_cQry_LotOverviewFacilityCharge(dbApp, guid, false).ToArray();
                        overview.MaterialSum = facManager.GetFacilityChargeSumMaterialHelperList(facilityChargeList, new FacilityQueryFilter() { FacilityLotID = guid });
                        overview.FacilityLocationSum = facManager.GetFacilityChargeSumLocationHelperList(facilityChargeList, new FacilityQueryFilter() { FacilityLotID = guid });
                        overview.FacilitySum = facManager.GetFacilityChargeSumFacilityHelperList(facilityChargeList, new FacilityQueryFilter() { FacilityLotID = guid });
                        if (facilityChargeList != null)
                            facilityChargeList = facilityChargeList.Where(c => c.Facility != null && !c.Facility.DisabledForMobile).ToArray();
                        overview.FacilityCharges = ConvertFacilityChargeList(facilityChargeList);
                        return new WSResponse<FacilityLotSumOverview>(overview);
                    }
                    catch (Exception e)
                    {
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLotSum(10)", e);
                        return new WSResponse<FacilityLotSumOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                    }
                }
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityLotSum));
            }
        }


        public WSResponse<PostingOverview> GetFacilityLotBookings(string facilityLotID, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(facilityLotID))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "facilityLotID is empty"));
            Guid? sessonId = WSRestAuthorizationManager.CurrentSessionID;

            Guid guid;
            if (!Guid.TryParse(facilityLotID, out guid))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "facilityLotID is invalid"));
            //string dt = dt.ToUniversalTime().ToString("o");
            DateTime dtFrom;
            if (String.IsNullOrWhiteSpace(dateFrom) || !DateTime.TryParseExact(dateFrom, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFrom))
                dtFrom = DateTime.Now.AddDays(-1);

            DateTime dtTo;
            if (String.IsNullOrWhiteSpace(dateTo) || !DateTime.TryParseExact(dateTo, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtTo))
                dtTo = DateTime.Now;

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityLotBookings));
            try
            {

                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                if (facManager == null)
                    return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    try
                    {
                        FacilityQueryFilter filter = new FacilityQueryFilter();
                        filter.SearchFrom = dtFrom;
                        filter.SearchTo = dtTo;
                        filter.FacilityLotID = guid;

                        Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> fbList = facManager.GetFacilityOverviewLists(dbApp, filter);
                        PostingOverview po = new PostingOverview();
                        po.Postings = fbList.Keys.ToList();
                        po.PostingsFBC = fbList.SelectMany(c => c.Value).ToList();
                        return new WSResponse<PostingOverview>(po);
                    }
                    catch (Exception e)
                    {
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLotBookings(10)", e);
                        return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                    }
                }
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityLotBookings));
            }
        }

        #endregion

        #region Material

        public static readonly Func<DatabaseApp, Guid, IQueryable<MaterialStock>> s_cQry_GetMaterialStock =
        CompiledQuery.Compile<DatabaseApp, Guid, IQueryable<MaterialStock>>(
            (dbApp, MaterialID) =>
                dbApp.MaterialStock
                        .Where(c => c.MaterialID == MaterialID)
                        .Select(c => new gip.mes.webservices.MaterialStock()
                        {
                            MaterialStockID = c.MaterialStockID,
                            StockQuantity = c.StockQuantity,
                            DayInward = c.DayInward,
                            DayOutward = c.DayOutward,
                            MonthInward = c.MonthInward,
                            MonthOutward = c.MonthOutward,
                            MDReleaseState = new gip.mes.webservices.MDReleaseState()
                            {
                                MDReleaseStateID = c.MDReleaseStateID.HasValue ? c.MDReleaseStateID.Value : Guid.Empty,
                                MDNameTrans = c.MDReleaseState != null ? c.MDReleaseState.MDNameTrans : ""
                            },
                        })
        );


        public WSResponse<MaterialSumOverview> GetMaterialSum(string materialID)
        {
            if (string.IsNullOrEmpty(materialID))
                return new WSResponse<MaterialSumOverview>(null, new Msg(eMsgLevel.Error, "materialID is empty"));

            Guid guid;
            if (!Guid.TryParse(materialID, out guid))
                return new WSResponse<MaterialSumOverview>(null, new Msg(eMsgLevel.Error, "materialID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<MaterialSumOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetMaterialSum));
            try
            {
                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                if (facManager == null)
                    return new WSResponse<MaterialSumOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    try
                    {
                        MaterialSumOverview overview = new MaterialSumOverview();
                        overview.MaterialStock = s_cQry_GetMaterialStock(dbApp, guid).FirstOrDefault();
                        gip.mes.datamodel.FacilityCharge[] facilityChargeList = FacilityManager.s_cQry_MatOverviewFacilityCharge(dbApp, guid, false).OrderBy(c => c.InsertDate).ToArray();
                        overview.FacilityLocationSum = facManager.GetFacilityChargeSumLocationHelperList(facilityChargeList, new FacilityQueryFilter() { MaterialID = guid });
                        overview.FacilitySum = facManager.GetFacilityChargeSumFacilityHelperList(facilityChargeList, new FacilityQueryFilter());
                        overview.FacilityLotSum = facManager.GetFacilityChargeSumLotHelperList(facilityChargeList, new FacilityQueryFilter() { MaterialID = guid });
                        overview.FacilityLocationSum = facManager.GetFacilityChargeSumLocationHelperList(facilityChargeList, new FacilityQueryFilter());
                        if (facilityChargeList != null)
                            facilityChargeList = facilityChargeList.Where(c => c.Facility != null && !c.Facility.DisabledForMobile).ToArray();
                        overview.FacilityCharges = ConvertFacilityChargeList(facilityChargeList);
                        return new WSResponse<MaterialSumOverview>(overview);
                    }
                    catch (Exception e)
                    {
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetMaterialSum(10)", e);
                        return new WSResponse<MaterialSumOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                    }
                }
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetMaterialSum));
            }
        }

        public WSResponse<PostingOverview> GetMaterialBookings(string materialID, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(materialID))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "materialID is empty"));
            Guid? sessonId = WSRestAuthorizationManager.CurrentSessionID;

            Guid guid;
            if (!Guid.TryParse(materialID, out guid))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "materialID is invalid"));
            //string dt = dt.ToUniversalTime().ToString("o");
            DateTime dtFrom;
            if (String.IsNullOrWhiteSpace(dateFrom) || !DateTime.TryParseExact(dateFrom, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFrom))
                dtFrom = DateTime.Now.AddDays(-1);

            DateTime dtTo;
            if (String.IsNullOrWhiteSpace(dateTo) || !DateTime.TryParseExact(dateTo, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtTo))
                dtTo = DateTime.Now;

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetMaterialBookings));
            try
            {
                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                if (facManager == null)
                    return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    try
                    {
                        FacilityQueryFilter filter = new FacilityQueryFilter();
                        filter.SearchFrom = dtFrom;
                        filter.SearchTo = dtTo;
                        filter.MaterialID = guid;

                        Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> fbList = facManager.GetFacilityOverviewLists(dbApp, filter);
                        PostingOverview po = new PostingOverview();
                        po.Postings = fbList.Keys.ToList();
                        po.PostingsFBC = fbList.SelectMany(c => c.Value).ToList();
                        return new WSResponse<PostingOverview>(po);
                    }
                    catch (Exception e)
                    {
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetMaterialBookings(10)", e);
                        return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                    }
                }
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetMaterialBookings));
            }
        }

        #endregion

        #region Facility

        public static readonly Func<DatabaseApp, Guid, IQueryable<FacilityStock>> s_cQry_GetFacilityStock =
        CompiledQuery.Compile<DatabaseApp, Guid, IQueryable<FacilityStock>>(
            (dbApp, FacilityID) =>
                dbApp.FacilityStock
                        .Where(c => c.FacilityID == FacilityID)
                        .Select(c => new gip.mes.webservices.FacilityStock()
                        {
                            FacilityStockID = c.FacilityStockID,
                            StockQuantity = c.StockQuantity,
                            DayInward = c.DayInward,
                            DayOutward = c.DayOutward,
                            MonthInward = c.MonthInward,
                            MonthOutward = c.MonthOutward,
                            MDReleaseState = new gip.mes.webservices.MDReleaseState()
                            {
                                MDReleaseStateID = c.MDReleaseStateID.HasValue ? c.MDReleaseStateID.Value : Guid.Empty,
                                MDNameTrans = c.MDReleaseState != null ? c.MDReleaseState.MDNameTrans : ""
                            },
                        })
        );

        public WSResponse<FacilitySumOverview> GetFacilitySum(string facilityID)
        {
            if (string.IsNullOrEmpty(facilityID))
                return new WSResponse<FacilitySumOverview>(null, new Msg(eMsgLevel.Error, "facilityID is empty"));

            Guid guid;
            if (!Guid.TryParse(facilityID, out guid))
                return new WSResponse<FacilitySumOverview>(null, new Msg(eMsgLevel.Error, "facilityID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<FacilitySumOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilitySum));
            try
            {

                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                if (facManager == null)
                    return new WSResponse<FacilitySumOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    try
                    {
                        FacilitySumOverview overview = new FacilitySumOverview();
                        overview.FacilityStock = s_cQry_GetFacilityStock(dbApp, guid).FirstOrDefault();
                        gip.mes.datamodel.FacilityCharge[] facilityChargeList = FacilityManager.s_cQry_FacilityOverviewFacilityCharge(dbApp, guid, false).ToArray();
                        overview.MaterialSum = facManager.GetFacilityChargeSumMaterialHelperList(facilityChargeList, new FacilityQueryFilter() { FacilityID = guid });
                        overview.FacilityLotSum = facManager.GetFacilityChargeSumLotHelperList(facilityChargeList, new FacilityQueryFilter() { FacilityID = guid });
                        overview.FacilityCharges = ConvertFacilityChargeList(facilityChargeList);
                        return new WSResponse<FacilitySumOverview>(overview);
                    }
                    catch (Exception e)
                    {
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilitySum(10)", e);
                        return new WSResponse<FacilitySumOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                    }
                }
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilitySum));
            }
        }

        public WSResponse<PostingOverview> GetFacilityBookings(string facilityID, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(facilityID))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "facilityID is empty"));
            Guid? sessonId = WSRestAuthorizationManager.CurrentSessionID;

            Guid guid;
            if (!Guid.TryParse(facilityID, out guid))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "facilityID is invalid"));
            //string dt = dt.ToUniversalTime().ToString("o");
            DateTime dtFrom;
            if (String.IsNullOrWhiteSpace(dateFrom) || !DateTime.TryParseExact(dateFrom, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFrom))
                dtFrom = DateTime.Now.AddDays(-1);

            DateTime dtTo;
            if (String.IsNullOrWhiteSpace(dateTo) || !DateTime.TryParseExact(dateTo, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtTo))
                dtTo = DateTime.Now;

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityBookings));
            try
            {

                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                if (facManager == null)
                    return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    try
                    {
                        FacilityQueryFilter filter = new FacilityQueryFilter();
                        filter.SearchFrom = dtFrom;
                        filter.SearchTo = dtTo;
                        filter.FacilityID = guid;

                        Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> fbList = facManager.GetFacilityOverviewLists(dbApp, filter);
                        PostingOverview po = new PostingOverview();
                        po.Postings = fbList.Keys.ToList();
                        po.PostingsFBC = fbList.SelectMany(c => c.Value).ToList();
                        return new WSResponse<PostingOverview>(po);
                    }
                    catch (Exception e)
                    {
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityBookings(10)", e);
                        return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                    }
                }
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityBookings));
            }
        }

        #endregion

        #region FacilityLocation

        public WSResponse<FacilityLocationSumOverview> GetFacilityLocationSum(string facilityID)
        {
            if (string.IsNullOrEmpty(facilityID))
                return new WSResponse<FacilityLocationSumOverview>(null, new Msg(eMsgLevel.Error, "facilityID is empty"));

            Guid guid;
            if (!Guid.TryParse(facilityID, out guid))
                return new WSResponse<FacilityLocationSumOverview>(null, new Msg(eMsgLevel.Error, "facilityID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<FacilityLocationSumOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityLocationSum));
            try
            {

                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                if (facManager == null)
                    return new WSResponse<FacilityLocationSumOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    try
                    {
                        FacilityLocationSumOverview overview = new FacilityLocationSumOverview();
                        gip.mes.datamodel.FacilityCharge[] facilityChargeList = FacilityManager.s_cQry_LocationOverviewFacilityCharge(dbApp, guid, false).ToArray();
                        overview.MaterialSum = facManager.GetFacilityChargeSumMaterialHelperList(facilityChargeList, new FacilityQueryFilter() { FacilityLocationID = guid });
                        overview.FacilityLotSum = facManager.GetFacilityChargeSumLotHelperList(facilityChargeList, new FacilityQueryFilter() { FacilityLocationID = guid });
                        overview.FacilityCharges = ConvertFacilityChargeList(facilityChargeList);
                        return new WSResponse<FacilityLocationSumOverview>(overview);
                    }
                    catch (Exception e)
                    {
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLotSum(10)", e);
                        return new WSResponse<FacilityLocationSumOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                    }
                }
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityLocationSum));
            }
        }


        public WSResponse<PostingOverview> GetFacilityLocationBookings(string facilityID, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(facilityID))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "facilityID is empty"));
            Guid? sessonId = WSRestAuthorizationManager.CurrentSessionID;

            Guid guid;
            if (!Guid.TryParse(facilityID, out guid))
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "facilityID is invalid"));
            //string dt = dt.ToUniversalTime().ToString("o");
            DateTime dtFrom;
            if (String.IsNullOrWhiteSpace(dateFrom) || !DateTime.TryParseExact(dateFrom, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFrom))
                dtFrom = DateTime.Now.AddDays(-1);

            DateTime dtTo;
            if (String.IsNullOrWhiteSpace(dateTo) || !DateTime.TryParseExact(dateTo, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtTo))
                dtTo = DateTime.Now;

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityLocationBookings));
            try
            {

                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                if (facManager == null)
                    return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    try
                    {
                        FacilityQueryFilter filter = new FacilityQueryFilter();
                        filter.SearchFrom = dtFrom;
                        filter.SearchTo = dtTo;
                        filter.FacilityLocationID = guid;

                        Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> fbList = facManager.GetFacilityOverviewLists(dbApp, filter);
                        PostingOverview po = new PostingOverview();
                        po.Postings = fbList.Keys.ToList();
                        po.PostingsFBC = fbList.SelectMany(c => c.Value).ToList();
                        return new WSResponse<PostingOverview>(po);
                    }
                    catch (Exception e)
                    {
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLocationBookings(10)", e);
                        return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                    }
                }
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityLocationBookings));
            }
        }

        #endregion

        #region Booking
        public WSResponse<MsgWithDetails> BookFacility(ACMethodBooking bpParam)
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(BookFacility));
            try
            {

                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                if (facManager == null)
                    return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

                MsgWithDetails msg;

                using (var dbApp = new DatabaseApp())
                {
                    var response = SetDatabaseUserName<MsgWithDetails>(dbApp, myServiceHost);
                    if (response != null)
                        return response;
                    msg = Book(bpParam, dbApp, facManager, myServiceHost);
                }
                return new WSResponse<MsgWithDetails>(msg);
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(BookFacility));
            }
        }

        protected virtual MsgWithDetails Book(ACMethodBooking bpParam, DatabaseApp dbApp, FacilityManager facManager, PAJsonServiceHostVB myServiceHost)
        {
            if (bpParam == null)
            {
                var error = new MsgWithDetails();
                error.AddDetailMessage(new Msg(eMsgLevel.Error, "Error: Booking parameter is null"));
                return error;
            }

            if (String.IsNullOrEmpty(bpParam.VirtualMethodName))
            {
                var error = new MsgWithDetails();
                error.AddDetailMessage(new Msg(eMsgLevel.Error, "Error: VirtualMethodName is empty"));
                return error;
            }

            if (bpParam.InwardQuantity > 10000000 || bpParam.OutwardQuantity > 10000000 || bpParam.InwardQuantity < -10000000 || bpParam.OutwardQuantity < -10000000)
            {
                var error = new MsgWithDetails();
                error.AddDetailMessage(new Msg(eMsgLevel.Error, "Error: Booking quantity is not valid!!!"));
                return error;
            }

            PerformanceEvent perfEvent110 = null;
            PerformanceEvent perfEvent120 = null;
            PerformanceEvent perfEvent130 = null;
            PerformanceEvent perfEvent140 = null;
            PerformanceEvent perfEvent150 = null;
            MsgWithDetails msgWithDetails = null;
            try
            {
                datamodel.PickingPos pickingPos = null;
                if (bpParam.PickingPosID.HasValue)
                {
                    perfEvent110 = myServiceHost.OnMethodCalled(nameof(BookFacility), 110);
                    pickingPos = dbApp.PickingPos
                                        .Include(c => c.InOrderPos)
                                        .Include(c => c.OutOrderPos)
                                        .Include(c => c.Picking)
                                        .FirstOrDefault(c => c.PickingPosID == bpParam.PickingPosID.Value);
                    myServiceHost.OnMethodReturned(perfEvent110, nameof(BookFacility), 110);
                    perfEvent110 = null;
                }
                facility.ACMethodBooking acParam = null;
                FacilityPreBooking preBooking = null;
                if (pickingPos != null)
                {
                    perfEvent120 = myServiceHost.OnMethodCalled(nameof(BookFacility), 120);
                    preBooking = pickingPos.FacilityPreBooking_PickingPos.FirstOrDefault();
                    if (preBooking == null && pickingPos.InOrderPos != null)
                        preBooking = pickingPos.InOrderPos.FacilityPreBooking_InOrderPos.FirstOrDefault();
                    else if (preBooking == null && pickingPos.OutOrderPos != null)
                        preBooking = pickingPos.OutOrderPos.FacilityPreBooking_OutOrderPos.FirstOrDefault();
                    if (preBooking != null)
                        acParam = preBooking.ACMethodBooking.Clone() as facility.ACMethodBooking;
                    myServiceHost.OnMethodReturned(perfEvent120, nameof(BookFacility), 120);
                    perfEvent120 = null;
                }

                if (acParam == null)
                {
                    perfEvent130 = myServiceHost.OnMethodCalled(nameof(BookFacility), 130);
                    acParam = facManager.ACUrlACTypeSignature("!" + bpParam.VirtualMethodName, gip.core.datamodel.Database.GlobalDatabase) as facility.ACMethodBooking;
                    myServiceHost.OnMethodReturned(perfEvent130, nameof(BookFacility), 130);
                    perfEvent130 = null;
                    if (pickingPos != null && facManager != null)
                    {
                        perfEvent140 = myServiceHost.OnMethodCalled(nameof(BookFacility), 140);
                        facManager.InitBookingParamsFromTemplate(acParam, pickingPos, preBooking);
                        myServiceHost.OnMethodReturned(perfEvent140, nameof(BookFacility), 140);
                        perfEvent140 = null;
                    }
                }

                if (acParam != null && bpParam.VirtualMethodName == mes.datamodel.GlobalApp.FBT_PickingInward 
                                    && (bpParam.ExpirationDate != null || !string.IsNullOrEmpty(bpParam.ExternLotNo)))
                {
                    string secondaryKey = Database.Root.NoManager.GetNewNo(dbApp, typeof(mes.datamodel.FacilityLot), mes.datamodel.FacilityLot.NoColumnName, mes.datamodel.FacilityLot.FormatNewNo);
                    mes.datamodel.FacilityLot lot = mes.datamodel.FacilityLot.NewACObject(dbApp, null, secondaryKey);
                    if (bpParam.ExpirationDate != null)
                        lot.ExpirationDate = bpParam.ExpirationDate;
                    else if (bpParam.InwardMaterialID.HasValue)
                    {
                        datamodel.Material material = dbApp.Material.FirstOrDefault(c => c.MaterialID == bpParam.InwardMaterialID.Value);
                        if (material != null)
                            lot.UpdateExpirationInfo(material);
                    }

                    if (!string.IsNullOrEmpty(bpParam.ExternLotNo))
                        lot.ExternLotNo = bpParam.ExternLotNo;
                    else
                        lot.ExternLotNo = DateTime.Now.ToShortDateString();

                    acParam.InwardFacilityLot = lot;
                }

                if (pickingPos != null)
                {
                    acParam.PickingPos = pickingPos;
                    acParam.InOrderPos = pickingPos.InOrderPos;
                    acParam.OutOrderPos = pickingPos.OutOrderPos;
                }

                if (acParam != null && bpParam.VirtualMethodName == mes.datamodel.GlobalApp.FBT_ProdOrderPosInward && bpParam.ProductionDateNewSublot.HasValue)
                {
                    var prodOrderPartslistPos = dbApp.ProdOrderPartslistPos.Include(c => c.FacilityLot)
                                                                           .Include(c => c.ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos)
                                                                           .Where(c => c.ProdOrderPartslistPosID == bpParam.PartslistPosID).FirstOrDefault();
                    if (prodOrderPartslistPos != null)
                    {
                        if (prodOrderPartslistPos.FacilityLot != null && prodOrderPartslistPos.FacilityLot.ProductionDate != null && prodOrderPartslistPos.FacilityLot.ProductionDate.Value.Date != bpParam.ProductionDateNewSublot.Value.Date)
                        {
                            var subLot = prodOrderPartslistPos.ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos.Where(c => c.FacilityLot.ProductionDate != null 
                                                                                                                              && c.FacilityLot.ProductionDate.Value.Date == bpParam.ProductionDateNewSublot.Value.Date)
                                                                                                                     .FirstOrDefault();

                            if (subLot == null)
                            {
                                string secondaryKey = Database.Root.NoManager.GetNewNo(dbApp, typeof(mes.datamodel.FacilityLot), mes.datamodel.FacilityLot.NoColumnName, mes.datamodel.FacilityLot.FormatNewNo);
                                mes.datamodel.FacilityLot lot = mes.datamodel.FacilityLot.NewACObject(dbApp, null, secondaryKey);
                                lot.ProductionDate = bpParam.ProductionDateNewSublot;

                                datamodel.Material mat = prodOrderPartslistPos.BookingMaterial;
                                if (mat == null)
                                    mat = prodOrderPartslistPos.Material;
                                if (mat != null)
                                    lot.UpdateExpirationInfo(mat, lot.ProductionDate);

                                dbApp.FacilityLot.AddObject(lot);

                                subLot = ProdOrderPartslistPosFacilityLot.NewACObject(dbApp, prodOrderPartslistPos);
                                subLot.FacilityLot = lot;

                                Msg msg = dbApp.ACSaveChanges();
                                if (msg != null)
                                {
                                    msgWithDetails = new MsgWithDetails();
                                    msgWithDetails.AddDetailMessage(msg);
                                    return msgWithDetails;
                                }
                            }

                            if (subLot != null)
                            {
                                bpParam.InwardFacilityLotID = subLot.FacilityLotID;
                            }
                        }
                    }
                }

                perfEvent150 = myServiceHost.OnMethodCalled(nameof(BookFacility), 150);
                acParam.ShiftBookingReverse = bpParam.ShiftBookingReverse;
                acParam.DontAllowNegativeStock = bpParam.DontAllowNegativeStock;
                acParam.IgnoreManagement = bpParam.IgnoreManagement;
                acParam.QuantityIsAbsolute = bpParam.QuantityIsAbsolute;
                if (bpParam.BalancingModeIndex.HasValue)
                    acParam.MDBalancingMode = dbApp.MDBalancingMode.Where(c => c.MDBalancingModeIndex == bpParam.BalancingModeIndex).FirstOrDefault();
                if (bpParam.ZeroStockStateIndex.HasValue)
                    acParam.MDZeroStockState = dbApp.MDZeroStockState.Where(c => c.MDZeroStockStateIndex == bpParam.ZeroStockStateIndex).FirstOrDefault();
                if (bpParam.ReleaseStateIndex.HasValue)
                    acParam.MDReleaseState = dbApp.MDReleaseState.Where(c => c.MDReleaseStateIndex == bpParam.ReleaseStateIndex).FirstOrDefault();
                if (bpParam.ReservationModeIndex.HasValue)
                    acParam.MDReservationMode = dbApp.MDReservationMode.Where(c => c.MDReservationModeIndex == bpParam.ReservationModeIndex).FirstOrDefault();

                if (bpParam.MovementReasonID.HasValue)
                    acParam.MDMovementReason = dbApp.MDMovementReason.Where(c => c.MDMovementReasonID == bpParam.MovementReasonID).FirstOrDefault();
                else if (bpParam.MovementReasonIndex.HasValue)
                    acParam.MDMovementReason = dbApp.MDMovementReason.Where(c => c.MDMovementReasonIndex == bpParam.MovementReasonIndex).FirstOrDefault();

                acParam.IgnoreIsEnabled = bpParam.IgnoreIsEnabled;
                acParam.SetCompleted = bpParam.SetCompleted;

                if (bpParam.InwardMaterialID.HasValue)
                    acParam.InwardMaterial = dbApp.Material.Where(c => c.MaterialID == bpParam.InwardMaterialID.Value).FirstOrDefault();
                if (bpParam.InwardFacilityID.HasValue)
                    acParam.InwardFacility = dbApp.Facility.Where(c => c.FacilityID == bpParam.InwardFacilityID.Value).FirstOrDefault();
                if (bpParam.InwardFacilityLotID.HasValue)
                    acParam.InwardFacilityLot = dbApp.FacilityLot.Where(c => c.FacilityLotID == bpParam.InwardFacilityLotID.Value).FirstOrDefault();
                if (bpParam.InwardFacilityChargeID.HasValue)
                {
                    acParam.InwardFacilityCharge = dbApp.FacilityCharge.Include(c => c.Facility).Where(c => c.FacilityChargeID == bpParam.InwardFacilityChargeID.Value).FirstOrDefault();
                    if (acParam.InwardFacilityCharge != null && (acParam.MDZeroStockState == null || acParam.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.Off))
                        acParam.InwardFacility = acParam.InwardFacilityCharge.Facility;
                }
                if (bpParam.InwardFacilityLocationID.HasValue)
                    acParam.InwardFacilityLocation = dbApp.Facility.Where(c => c.FacilityID == bpParam.InwardFacilityLocationID.Value).FirstOrDefault();
                if (bpParam.InwardPartslistID.HasValue)
                    acParam.InwardPartslist = dbApp.Partslist.Where(c => c.PartslistID == bpParam.InwardPartslistID.Value).FirstOrDefault();
                if (bpParam.InwardCompanyMaterialID.HasValue)
                    acParam.InwardCompanyMaterial = dbApp.CompanyMaterial.Where(c => c.CompanyMaterialID == bpParam.InwardCompanyMaterialID.Value).FirstOrDefault();
                acParam.InwardSplitNo = bpParam.InwardSplitNo;
                acParam.InwardQuantity = bpParam.InwardQuantity;
                acParam.InwardTargetQuantity = bpParam.InwardTargetQuantity;

                if (bpParam.InwardAutoSplitQuant.HasValue)
                    acParam.InwardAutoSplitQuant = bpParam.InwardAutoSplitQuant;

                if (bpParam.OutwardMaterialID.HasValue)
                    acParam.OutwardMaterial = dbApp.Material.Where(c => c.MaterialID == bpParam.OutwardMaterialID.Value).FirstOrDefault();
                if (bpParam.OutwardFacilityID.HasValue)
                    acParam.OutwardFacility = dbApp.Facility.Where(c => c.FacilityID == bpParam.OutwardFacilityID.Value).FirstOrDefault();
                if (bpParam.OutwardFacilityLotID.HasValue)
                    acParam.OutwardFacilityLot = dbApp.FacilityLot.Where(c => c.FacilityLotID == bpParam.OutwardFacilityLotID.Value).FirstOrDefault();
                if (bpParam.OutwardFacilityChargeID.HasValue)
                {
                    acParam.OutwardFacilityCharge = dbApp.FacilityCharge.Include(c => c.Facility).Where(c => c.FacilityChargeID == bpParam.OutwardFacilityChargeID.Value).FirstOrDefault();
                    if (acParam.OutwardFacilityCharge != null)
                        acParam.OutwardFacility = acParam.OutwardFacilityCharge.Facility;
                }
                if (bpParam.OutwardFacilityLocationID.HasValue)
                    acParam.OutwardFacilityLocation = dbApp.Facility.Where(c => c.FacilityID == bpParam.OutwardFacilityLocationID.Value).FirstOrDefault();
                if (bpParam.OutwardPartslistID.HasValue)
                    acParam.OutwardPartslist = dbApp.Partslist.Where(c => c.PartslistID == bpParam.OutwardPartslistID.Value).FirstOrDefault();
                if (bpParam.OutwardCompanyMaterialID.HasValue)
                    acParam.OutwardCompanyMaterial = dbApp.CompanyMaterial.Where(c => c.CompanyMaterialID == bpParam.OutwardCompanyMaterialID.Value).FirstOrDefault();
                acParam.OutwardSplitNo = bpParam.OutwardSplitNo;
                acParam.OutwardQuantity = bpParam.OutwardQuantity;
                acParam.OutwardTargetQuantity = bpParam.OutwardTargetQuantity;

                if (bpParam.MDUnitID.HasValue)
                    acParam.MDUnit = dbApp.MDUnit.Where(c => c.MDUnitID == bpParam.MDUnitID.Value).FirstOrDefault();
                if (bpParam.InOrderPosID.HasValue)
                    acParam.InOrderPos = dbApp.InOrderPos.Where(c => c.InOrderPosID == bpParam.InOrderPosID.Value).FirstOrDefault();
                if (bpParam.OutOrderPosID.HasValue)
                    acParam.OutOrderPos = dbApp.OutOrderPos.Where(c => c.OutOrderPosID == bpParam.OutOrderPosID.Value).FirstOrDefault();
                if (bpParam.PartslistPosID.HasValue)
                    acParam.PartslistPos = dbApp.ProdOrderPartslistPos.Where(c => c.ProdOrderPartslistPosID == bpParam.PartslistPosID.Value).FirstOrDefault();

                if (bpParam.PartslistPosRelationID.HasValue)
                    acParam.PartslistPosRelation = dbApp.ProdOrderPartslistPosRelation
                                                        .Include(x => x.SourceProdOrderPartslistPos)
                                                        .Where(c => c.ProdOrderPartslistPosRelationID == bpParam.PartslistPosRelationID.Value).FirstOrDefault();

                if (bpParam.ProdOrderPartslistPosFacilityLotID.HasValue)
                    acParam.ProdOrderPartslistPosFacilityLot = dbApp.ProdOrderPartslistPosFacilityLot.Where(c => c.ProdOrderPartslistPosFacilityLotID == bpParam.ProdOrderPartslistPosFacilityLotID.Value).FirstOrDefault();
                if (bpParam.PickingPosID.HasValue)
                    acParam.PickingPos = dbApp.PickingPos.Where(c => c.PickingPosID == bpParam.PickingPosID.Value).FirstOrDefault();

                acParam.StorageDate = bpParam.StorageDate;
                acParam.ProductionDate = bpParam.ProductionDate;
                acParam.ExpirationDate = bpParam.ExpirationDate;
                acParam.MinimumDurability = bpParam.MinimumDurability;
                acParam.Comment = bpParam.Comment;
                acParam.RecipeOrFactoryInfo = bpParam.RecipeOrFactoryInfo;
                acParam.PropertyACUrl = bpParam.PropertyACUrl;
                myServiceHost.OnMethodReturned(perfEvent150, nameof(BookFacility), 150);
                perfEvent150 = null;

                msgWithDetails = OnBookWithFacilityManager(bpParam, acParam, dbApp, facManager, myServiceHost);
            }
            catch (Exception e)
            {
                if (myServiceHost != null)
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLocationBookings(10)", e);
                msgWithDetails = new MsgWithDetails();
                msgWithDetails.AddDetailMessage(new Msg(eMsgLevel.Exception, e.Message));
                return msgWithDetails;
            }
            finally
            {
                if (perfEvent110 != null)
                    myServiceHost.OnMethodReturned(perfEvent110, nameof(BookFacility), 110);
                if (perfEvent120 != null)
                    myServiceHost.OnMethodReturned(perfEvent110, nameof(BookFacility), 120);
                if (perfEvent130 != null)
                    myServiceHost.OnMethodReturned(perfEvent110, nameof(BookFacility), 130);
                if (perfEvent140 != null)
                    myServiceHost.OnMethodReturned(perfEvent110, nameof(BookFacility), 140);
                if (perfEvent150 != null)
                    myServiceHost.OnMethodReturned(perfEvent110, nameof(BookFacility), 150);
            }
            return msgWithDetails;
        }

        protected virtual MsgWithDetails OnBookWithFacilityManager(ACMethodBooking bpParam, facility.ACMethodBooking acParam, DatabaseApp dbApp, FacilityManager facManager, PAJsonServiceHostVB myServiceHost)
        {
            MsgWithDetails msgWithDetails = null;

            if (acParam.PartslistPosRelation != null && acParam.PartslistPosRelation.SourceProdOrderPartslistPos != null)
            {
                if (!acParam.PartslistPosRelation.SourceProdOrderPartslistPos.TakeMatFromOtherOrder && acParam.OutwardFacilityCharge != null)
                {
                    string programNo = acParam.OutwardFacilityCharge.ProdOrderProgramNo;
                    var prodOrder = acParam.PartslistPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist?.ProdOrder;
                    string currentProgramNo = prodOrder?.ProgramNo;

                    if (!string.IsNullOrEmpty(programNo) && !string.IsNullOrEmpty(currentProgramNo) && programNo != currentProgramNo)
                    {
                        if (prodOrder.ProdOrderPartslist_ProdOrder.Any(c => c.Partslist.MaterialID == acParam.PartslistPosRelation.SourceProdOrderPartslistPos.MaterialID))
                        {
                            var error = new MsgWithDetails();
                            error.AddDetailMessage(new Msg(facManager, eMsgLevel.Warning, nameof(VBWebService), nameof(OnBookWithFacilityManager) + "(10)", 1475, "Warning50053"));
                            return error;
                        }
                    }
                }
            }

            var resultBooking = facManager.BookFacilityWithRetry(ref acParam, dbApp, false);
            if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (myServiceHost != null)
                    myServiceHost.Messages.LogError(myServiceHost.GetACUrl(), "BookFacility(10)", acParam.ValidMessage.InnerMessage);
                dbApp.ACUndoChanges();
                msgWithDetails = acParam.ValidMessage;
                return msgWithDetails;
            }
            else
            {
                if (acParam.PickingPos != null && facManager != null)
                {
                    double postedQuantity = 0;
                    if (acParam.OutwardQuantity.HasValue)
                        postedQuantity = acParam.OutwardQuantity.Value;
                    else if (acParam.InwardQuantity.HasValue)
                        postedQuantity = acParam.InwardQuantity.Value;
                    facManager.RecalcAfterPosting(dbApp, acParam.PickingPos, postedQuantity, false, true);
                    msgWithDetails = dbApp.ACSaveChangesWithRetry();

                    if (msgWithDetails == null)
                    {
                        datamodel.FacilityCharge outwardFC = acParam.OutwardFacilityCharge;
                        if (outwardFC != null)
                        {
                            msgWithDetails = facManager.IsQuantStockConsumed(outwardFC, dbApp);
                            return msgWithDetails;
                        }
                    }
                }
                else if (acParam.PartslistPos != null)
                {
                    acParam.PartslistPos.RecalcActualQuantity();
                    if (acParam.PartslistPos.ProdOrderPartslist != null)
                        acParam.PartslistPos.ProdOrderPartslist.RecalcActualQuantitySP(dbApp);
                    msgWithDetails = dbApp.ACSaveChangesWithRetry();
                }
                else if (acParam.PartslistPosRelation != null)
                {
                    acParam.PartslistPosRelation.RecalcActualQuantity();
                    msgWithDetails = dbApp.ACSaveChangesWithRetry();
                }
            }
            return msgWithDetails;
        }

        private void CompleteBookFacilityPicking(DatabaseApp dbApp, datamodel.PickingPos pickingPos, facility.ACMethodBooking acParam, facility.ACMethodBooking preBookingMethod,
                                                 FacilityPreBooking preBooking)
        {
            bool isCompleted = pickingPos.RemainingDosingQuantityUOM >= 0;
            bool isCancellation = acParam.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel || acParam.BookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel;

            acParam.PickingPos.RecalcActualQuantity();

            if (preBookingMethod != null && preBooking != null && acParam.ValidMessage.IsSucceded())
            {
                if (isCompleted)
                {
                    preBooking.DeleteACObject(dbApp, true);
                    pickingPos.MDDelivPosLoadState = DatabaseApp.s_cQry_GetMDDelivPosLoadState(dbApp, MDDelivPosLoadState.DelivPosLoadStates.LoadToTruck).FirstOrDefault();
                }
            }

            dbApp.ACSaveChanges();
        }

        public WSResponse<MsgWithDetails> BookFacilities(ACMethodBookingList bpParams)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(BookFacilities));
            try
            {

                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                if (facManager == null)
                    return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

                using (DatabaseApp dbApp = new DatabaseApp())
                {
                    var response = SetDatabaseUserName<MsgWithDetails>(dbApp, myServiceHost);
                    if (response != null)
                        return response;
                    foreach (ACMethodBooking bpParam in bpParams)
                    {
                        MsgWithDetails msg = Book(bpParam, dbApp, facManager, myServiceHost);
                        if (msg != null)
                        {
                            Msg message = new Msg(msg.MessageLevel, msg.Message) { MessageButton = msg.MessageButton };
                            msgWithDetails.AddDetailMessage(message);

                            foreach (Msg m in msg.MsgDetails)
                            {
                                message = new Msg(m.MessageLevel, m.Message);
                                msgWithDetails.AddDetailMessage(message);
                            }
                        }
                    }
                }
                return new WSResponse<MsgWithDetails>(msgWithDetails);
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(BookFacilities));
            }
        }

        #endregion

        #region Inventory

        #region Inventory -> MD
        public WSResponse<List<MDFacilityInventoryState>> GetMDFacilityInventoryStates()
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<MDFacilityInventoryState>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            WSResponse<List<MDFacilityInventoryState>> response = new WSResponse<List<MDFacilityInventoryState>>();
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetMDFacilityInventoryStates));
            try
            {
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    List<MDFacilityInventoryState> states =
                        databaseApp
                        .MDFacilityInventoryState
                        .Select(c => new MDFacilityInventoryState()
                        {
                            MDFacilityInventoryStateID = c.MDFacilityInventoryStateID,
                            MDFacilityInventoryStateIndex = c.MDFacilityInventoryStateIndex,
                            MDNameTrans = c.MDNameTrans,
                            IsDefault = c.IsDefault,
                            SortIndex = c.SortIndex
                        })
                        .ToList();
                    response.Data = states;
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetMDFacilityInventoryStates(973)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetMDFacilityInventoryStates));
            }

            return response;
        }

        public WSResponse<List<MDFacilityInventoryPosState>> GetMDFacilityInventoryPosStates()
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<MDFacilityInventoryPosState>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            WSResponse<List<MDFacilityInventoryPosState>> response = new WSResponse<List<MDFacilityInventoryPosState>>();
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetMDFacilityInventoryPosStates));
            try
            {
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    List<MDFacilityInventoryPosState> states =
                        databaseApp
                        .MDFacilityInventoryPosState
                        .Select(c => new MDFacilityInventoryPosState()
                        {
                            MDFacilityInventoryPosStateID = c.MDFacilityInventoryPosStateID,
                            MDFacilityInventoryPosStateIndex = c.MDFacilityInventoryPosStateIndex,
                            MDNameTrans = c.MDNameTrans,
                            IsDefault = c.IsDefault,
                            SortIndex = c.SortIndex
                        })
                        .ToList();
                    response.Data = states;
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetMDFacilityInventoryStates(973)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetMDFacilityInventoryPosStates));
            }
            return response;
        }
        #endregion

        #region Inventory -> Get

        public static readonly Func<DatabaseApp, short?, DateTime, DateTime, IQueryable<datamodel.FacilityInventory>> s_cQry_GetFacilityInventories =
        CompiledQuery.Compile<DatabaseApp, short?, DateTime, DateTime, IQueryable<datamodel.FacilityInventory>>(
            (dbApp, inventoryState, dateFrom, dateTo) =>
                dbApp.FacilityInventory
                        .Where(c => (inventoryState == null || c.MDFacilityInventoryState.MDFacilityInventoryStateIndex == inventoryState)
                                    && c.InsertDate >= dateFrom
                                    && c.InsertDate < dateTo));

        public List<FacilityInventory> ConvtertFacilityInventory(IEnumerable<datamodel.FacilityInventory> inventories)
        {
            return inventories.ToArray()
                            .Select(c => new FacilityInventory()
                            {
                                FacilityInventoryID = c.FacilityInventoryID,
                                FacilityInventoryNo = c.FacilityInventoryNo,
                                FacilityInventoryName = c.FacilityInventoryName,
                                InsertName = c.InsertName,
                                InsertDate = c.InsertDate,
                                SuggestStockQuantity = c.SuggestStockQuantity,
                                MDFacilityInventoryState = new gip.mes.webservices.MDFacilityInventoryState()
                                {
                                    MDFacilityInventoryStateID = c.MDFacilityInventoryStateID,
                                    MDNameTrans = c.MDFacilityInventoryState.MDNameTrans,
                                    MDFacilityInventoryStateIndex = c.MDFacilityInventoryState.MDFacilityInventoryStateIndex,
                                    SortIndex = c.MDFacilityInventoryState.SortIndex,
                                    IsDefault = c.MDFacilityInventoryState.IsDefault
                                },
                                Facility = c.Facility == null ? null :
                                                        new gip.mes.webservices.Facility()
                                                        {
                                                            FacilityID = c.Facility.FacilityID,
                                                            FacilityNo = c.Facility.FacilityNo,
                                                            FacilityName = c.Facility.FacilityName,
                                                            MDFacilityType = new MDFacilityType()
                                                            {
                                                                MDFacilityTypeID = c.Facility.MDFacilityTypeID,
                                                                MDNameTrans = c.Facility.MDFacilityType.MDNameTrans,
                                                                MDFacilityTypeIndex = c.Facility.MDFacilityType.MDFacilityTypeIndex
                                                            },
                                                            ParentFacilityID = c.Facility.ParentFacilityID,
                                                            SkipPrintQuestion = c.Facility.SkipPrintQuestion
                                                        }
                            }).ToList();
        }

        public WSResponse<List<FacilityInventory>> GetFacilityInventories(string inventoryState, string dateFrom, string dateTo)
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<FacilityInventory>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            WSResponse<List<FacilityInventory>> response = new WSResponse<List<FacilityInventory>>();
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityInventories));
            try
            {
                short? inventoryStateVal = null;
                if (!string.IsNullOrEmpty(inventoryState) && inventoryState != CoreWebServiceConst.EmptyParam)
                    inventoryStateVal = short.Parse(inventoryState);

                DateTime dtTo;
                if (String.IsNullOrWhiteSpace(dateTo) || !DateTime.TryParseExact(dateTo, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtTo))
                    dtTo = new DateTime(DateTime.Now.Year + 1, 1, 1);

                DateTime dtFrom;
                if (String.IsNullOrWhiteSpace(dateFrom) || !DateTime.TryParseExact(dateFrom, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtFrom))
                    dtFrom = dtTo.AddYears(-1);

                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    response.Data = ConvtertFacilityInventory(s_cQry_GetFacilityInventories(databaseApp, inventoryStateVal, dtFrom, dtTo).Take(myServiceHost.Root.Environment.AccessDefaultTakeCount));
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityInventories(1067)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityInventories));
            }
            return response;
        }

        #endregion

        #region Inventory -> New
        public WSResponse<string> GetFacilityInventoryNo()
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<string>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityInventoryNo));
            WSResponse<string> response = new WSResponse<string>();
            try
            {
                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                response.Data = facManager.GetNewInventoryNo();
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityInventoryNo(1090)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityInventoryNo));
            }
            return response;
        }

        public WSResponse<bool> NewFacilityInventory(string facilityInventoryNo, string facilityInventoryName)
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(NewFacilityInventory));
            WSResponse<bool> response = new WSResponse<bool>();
            try
            {
                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                MsgWithDetails msgWithDetails = facManager.InventoryGenerate(facilityInventoryNo, facilityInventoryName, null, false, true);
                response.Data = msgWithDetails.IsSucceded();
                response.Message = new Msg { MessageLevel = msgWithDetails.MessageLevel, Message = msgWithDetails.Message };
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "NewFacilityInventory(1109)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(NewFacilityInventory));
            }
            return response;
        }
        #endregion

        #region Inventory -> Lifecycle
        public WSResponse<bool> StartFacilityInventory(string facilityInventoryNo)
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            WSResponse<bool> response = new WSResponse<bool>();
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(StartFacilityInventory));
            try
            {
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    var subResponse = SetDatabaseUserName<bool>(databaseApp);
                    if (subResponse != null)
                        return subResponse;
                    mes.datamodel.MDFacilityInventoryState inProgressState = databaseApp.MDFacilityInventoryState.FirstOrDefault(c => c.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.InProgress);
                    mes.datamodel.FacilityInventory facilityInventory = databaseApp.FacilityInventory.FirstOrDefault(c => c.FacilityInventoryNo == facilityInventoryNo);

                    if (facilityInventory == null)
                    {
                        response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format("Missing Inventory No: {0}", facilityInventoryNo) };
                    }
                    else
                    {
                        facilityInventory.MDFacilityInventoryState = inProgressState;
                        MsgWithDetails saveMsg = databaseApp.ACSaveChanges();
                        if (saveMsg == null || saveMsg.IsSucceded())
                            response.Data = true;
                        else
                            response.Message = saveMsg;
                    }

                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "StartFacilityInventory(1132)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(StartFacilityInventory));
            }
            return response;
        }

        public WSResponse<bool> CloseFacilityInventory(string facilityInventoryNo)
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(CloseFacilityInventory));
            WSResponse<bool> response = new WSResponse<bool>();
            try
            {
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    var subResponse = SetDatabaseUserName<bool>(databaseApp);
                    if (subResponse != null)
                        return subResponse;
                    mes.datamodel.MDFacilityInventoryState finishedState = databaseApp.MDFacilityInventoryState.FirstOrDefault(c => c.MDFacilityInventoryStateIndex == (short)FacilityInventoryStateEnum.Finished);
                    mes.datamodel.FacilityInventory facilityInventory = databaseApp.FacilityInventory.FirstOrDefault(c => c.FacilityInventoryNo == facilityInventoryNo);

                    if (facilityInventory == null)
                    {
                        response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format("Missing Inventory No: {0}", facilityInventoryNo) };
                    }
                    else
                    {
                        facilityInventory.MDFacilityInventoryState = finishedState;
                        MsgWithDetails saveMsg = databaseApp.ACSaveChanges();
                        if (saveMsg == null || saveMsg.IsSucceded())
                            response.Data = true;
                        else
                            response.Message = saveMsg;
                    }

                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "CloseFacilityInventory(1168)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(CloseFacilityInventory));
            }
            return response;
        }
        #endregion

        #region Inventory -> Pos
        #region Inventory -> Pos - Get

        public static readonly Func<DatabaseApp, string, Guid?, string, string, string, string, short?, bool?, bool?, bool, IQueryable<FacilityInventoryPos>> s_cQry_GetFacilityInventoryLines =
        CompiledQuery.Compile<DatabaseApp, string, Guid?, string, string, string, string, short?, bool?, bool?, bool, IQueryable<FacilityInventoryPos>>(
           (dbApp, facilityInventoryNo, inputCodeVal, storageLocationNo, facilityNo, lotNo, materialNo, inventoryPosStateVal, notAvailableVal, zeroStockVal, notProcessedVal) =>
               dbApp.FacilityInventoryPos
                       .Where(c =>
                          c.FacilityInventory.FacilityInventoryNo == facilityInventoryNo
                          && (inputCodeVal == null || c.FacilityChargeID == inputCodeVal)
                           && (
                                   storageLocationNo == null
                                   || (c.FacilityCharge.Facility.Facility1_ParentFacility != null && c.FacilityCharge.Facility.Facility1_ParentFacility.FacilityNo == storageLocationNo)
                           )
                           && (facilityNo == null || c.FacilityCharge.Facility.FacilityNo == facilityNo)
                           && (lotNo == null || (c.FacilityCharge.FacilityLot != null && c.FacilityCharge.FacilityLot.LotNo == lotNo))
                           && (materialNo == null || c.FacilityCharge.Material.MaterialNo == materialNo)
                           && (inventoryPosStateVal == null || c.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex <= inventoryPosStateVal)
                           && (notAvailableVal == null || c.NotAvailable == (notAvailableVal ?? false))
                           && (zeroStockVal == null || (c.FacilityCharge.StockQuantity == 0) == (zeroStockVal ?? false))
                           && (!notProcessedVal || (!c.NotAvailable && c.NewStockQuantity == null))
                       )
                       .Select(c => new FacilityInventoryPos()
                       {
                           FacilityInventoryPosID = c.FacilityInventoryPosID,
                           Sequence = c.Sequence,
                           Comment = c.Comment,
                           LotNo = c.FacilityCharge.FacilityLot.LotNo,
                           MaterialNo = c.FacilityCharge.Material.MaterialNo,
                           MaterialName = c.FacilityCharge.Material.MaterialName1,
                           ParentFacilityNo = c.FacilityCharge.Facility.Facility1_ParentFacility != null ? c.FacilityCharge.Facility.Facility1_ParentFacility.FacilityNo : null,
                           FacilityNo = c.FacilityCharge.Facility.FacilityNo,
                           FacilityName = c.FacilityCharge.Facility.FacilityName,
                           StockQuantity = c.StockQuantity,
                           NewStockQuantity = c.NewStockQuantity,
                           NotAvailable = c.NotAvailable,
                           UpdateDate = c.UpdateDate,
                           UpdateName = c.UpdateName,
                           FacilityChargeID = c.FacilityChargeID,
                           FacilityInventoryNo = c.FacilityInventory.FacilityInventoryNo,
                           MDFacilityInventoryPosStateIndex = c.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex
                       })
        );
        
        public WSResponse<List<FacilityInventoryPos>> GetFacilityInventoryLines(string facilityInventoryNo, string inputCode, string storageLocationNo, string facilityNo,
            string lotNo, string materialNo, string inventoryPosState, string notAvailable, string zeroStock, string notProcessed)
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<FacilityInventoryPos>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            WSResponse<List<FacilityInventoryPos>> response = new WSResponse<List<FacilityInventoryPos>>();
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityInventoryLines));
            try
            {
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    storageLocationNo = storageLocationNo != CoreWebServiceConst.EmptyParam ? storageLocationNo : null;
                    facilityNo = facilityNo != CoreWebServiceConst.EmptyParam ? facilityNo : null;
                    lotNo = lotNo != CoreWebServiceConst.EmptyParam ? lotNo : null;
                    materialNo = materialNo != CoreWebServiceConst.EmptyParam ? materialNo : null;

                    short? inventoryPosStateVal = null;
                    if (!string.IsNullOrEmpty(inventoryPosState) && inventoryPosState != CoreWebServiceConst.EmptyParam)
                        inventoryPosStateVal = short.Parse(inventoryPosState);

                    bool? notAvailableVal = null;
                    if (!string.IsNullOrEmpty(notAvailable) && notAvailable != CoreWebServiceConst.EmptyParam)
                        notAvailableVal = bool.Parse(notAvailable);

                    bool? zeroStockVal = null;
                    if (!string.IsNullOrEmpty(zeroStock) && zeroStock != CoreWebServiceConst.EmptyParam)
                        zeroStockVal = bool.Parse(zeroStock);

                    bool notProcessedVal = false;
                    if (!string.IsNullOrEmpty(notProcessed) && notProcessed != CoreWebServiceConst.EmptyParam)
                        notProcessedVal = bool.Parse(notProcessed);
                    Guid? inputCodeVal = null;
                    if (!string.IsNullOrEmpty(inputCode) && inputCode != CoreWebServiceConst.EmptyParam)
                    {
                        Guid testParseInputCode = Guid.Empty;
                        if (Guid.TryParse(inputCode, out testParseInputCode))
                        {
                            inputCodeVal = testParseInputCode;
                            bool isFacilityCharge = databaseApp.FacilityCharge.Any(c => c.FacilityChargeID == inputCodeVal);
                            if (!isFacilityCharge)
                            {
                                // Facility
                                datamodel.Facility facility = databaseApp.Facility.FirstOrDefault(c => c.FacilityID == inputCodeVal);
                                if (facility != null)
                                {
                                    facilityNo = facility.FacilityNo;
                                    inputCodeVal = null;
                                }
                                else
                                {
                                    // Material
                                    datamodel.Material material = databaseApp.Material.FirstOrDefault(c => c.MaterialID == inputCodeVal);
                                    if (material != null)
                                    {
                                        materialNo = material.MaterialNo;
                                        inputCodeVal = null;
                                    }
                                    else
                                    {
                                        // Lot
                                        datamodel.FacilityLot facilityLot = databaseApp.FacilityLot.FirstOrDefault(c => c.FacilityLotID == inputCodeVal);
                                        if (facilityLot != null)
                                        {
                                            lotNo = facilityLot.LotNo;
                                            inputCodeVal = null;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Facility
                            datamodel.Facility facility = databaseApp.Facility.FirstOrDefault(c => c.FacilityNo == inputCode);
                            if (facility != null)
                                facilityNo = facility.FacilityNo;
                            else
                            {
                                // Material
                                datamodel.Material material = databaseApp.Material.FirstOrDefault(c => c.MaterialNo == inputCode);
                                if (material != null)
                                    materialNo = material.MaterialNo;
                                else
                                {
                                    // Lot
                                    datamodel.FacilityLot facilityLot = databaseApp.FacilityLot.FirstOrDefault(c => c.LotNo == inputCode);
                                    if (facilityLot != null)
                                        lotNo = facilityLot.LotNo;
                                }
                            }
                        }
                    }

                    List<FacilityInventoryPos> items =
                        s_cQry_GetFacilityInventoryLines(
                                databaseApp,
                                facilityInventoryNo,
                                inputCodeVal,
                                storageLocationNo,
                                facilityNo,
                                lotNo,
                                materialNo,
                                inventoryPosStateVal,
                                notAvailableVal,
                                zeroStockVal,
                                notProcessedVal
                        )
                        .OrderBy(c => c.LotNo).ToList();
                    response.Data = items;
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityInventoryLines(1361)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityInventoryLines));
            }
            return response;
        }

        #endregion

        #region Inventory -> Pos -> Lifecycle

        public WSResponse<bool> UpdateFacilityInventoryPos(FacilityInventoryPos facilityInventoryPos)
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            WSResponse<bool> response = new WSResponse<bool>();
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(UpdateFacilityInventoryPos));
            try
            {
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    var subResponse = SetDatabaseUserName<bool>(databaseApp);
                    if (subResponse != null)
                        return subResponse;



                    mes.datamodel.FacilityInventoryPos dbFacilityInventoryPos = null;

                    if (facilityInventoryPos.FacilityInventoryPosID != Guid.Empty)
                    {
                        dbFacilityInventoryPos = databaseApp.FacilityInventoryPos
                                                            .Where(c => c.FacilityInventoryPosID == facilityInventoryPos.FacilityInventoryPosID)
                                                            .FirstOrDefault();
                    }
                    else if (!facilityInventoryPos.NotAvailable)
                    {
                        datamodel.FacilityInventory inv = databaseApp.FacilityInventory.FirstOrDefault(c => c.FacilityInventoryNo == facilityInventoryPos.FacilityInventoryNo);
                        if (inv != null)
                        {
                            dbFacilityInventoryPos = datamodel.FacilityInventoryPos.NewACObject(databaseApp, inv);
                            dbFacilityInventoryPos.FacilityChargeID = facilityInventoryPos.FacilityChargeID;
                        }
                    }


                    if (dbFacilityInventoryPos == null)
                    {
                        Msg errorMissingPosition = new Msg() { MessageLevel = eMsgLevel.Error, Message = "Missing Inventory Linie!" };
                        response.Message = errorMissingPosition;
                    }
                    else
                    {
                        // Check is inventory document active for inventory - InArbeit
                        bool newInventory = dbFacilityInventoryPos.FacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)MDFacilityInventoryState.FacilityInventoryStates.New;
                        bool finishedInventory = dbFacilityInventoryPos.FacilityInventory.MDFacilityInventoryState.MDFacilityInventoryStateIndex == (short)MDFacilityInventoryState.FacilityInventoryStates.Finished;
                        if (newInventory)
                        {
                            Msg errorNewInventory = new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format("Inventory {0} is not started! Fail to update line!", dbFacilityInventoryPos.FacilityInventory.FacilityInventoryNo) };
                            response.Message = errorNewInventory;
                        }
                        else if (finishedInventory)
                        {
                            Msg errorFinishedInventory = new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format("Inventory {0} is finished! Fail to update line!", dbFacilityInventoryPos.FacilityInventory.FacilityInventoryNo) };
                            response.Message = errorFinishedInventory;
                        }
                        if (!newInventory && !finishedInventory)
                        {
                            gip.mes.datamodel.MDFacilityInventoryPosState inProgressState = databaseApp.MDFacilityInventoryPosState.Where(c => c.MDFacilityInventoryPosStateIndex == (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.InProgress).FirstOrDefault();
                            gip.mes.datamodel.MDFacilityInventoryPosState finishedState = databaseApp.MDFacilityInventoryPosState.Where(c => c.MDFacilityInventoryPosStateIndex == (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished).FirstOrDefault();
                            dbFacilityInventoryPos.NewStockQuantity = facilityInventoryPos.NewStockQuantity;
                            dbFacilityInventoryPos.NotAvailable = facilityInventoryPos.NotAvailable;
                            dbFacilityInventoryPos.Comment = facilityInventoryPos.Comment;
                            dbFacilityInventoryPos.UpdateDate = DateTime.Now;
                            dbFacilityInventoryPos.UpdateName = facilityInventoryPos.UpdateName;
                            if (facilityInventoryPos.MDFacilityInventoryPosStateIndex == (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.InProgress)
                                dbFacilityInventoryPos.MDFacilityInventoryPosState = inProgressState;
                            if (facilityInventoryPos.MDFacilityInventoryPosStateIndex == (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished)
                                dbFacilityInventoryPos.MDFacilityInventoryPosState = finishedState;

                            MsgWithDetails saveMsg = databaseApp.ACSaveChanges(true, SaveOptions.AcceptAllChangesAfterSave, false, false);
                            if (saveMsg == null || saveMsg.IsSucceded())
                                response.Data = true;
                            else
                                response.Message = saveMsg;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "UpdateFacilityInventoryPos(1430)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(UpdateFacilityInventoryPos));
            }
            return response;
        }

        public WSResponse<SearchFacilityCharge> GetFacilityInventorySearchCharge(string facilityInventoryNo, string storageLocationNo, string facilityNo, string facilityChargeID)
        {
            WSResponse<SearchFacilityCharge> response = new WSResponse<SearchFacilityCharge>();

            string emptyErrorMessage = "";
            if (string.IsNullOrEmpty(facilityInventoryNo))
                emptyErrorMessage += "facilityInventoryNo is empty";
            if (string.IsNullOrEmpty(facilityChargeID))
                emptyErrorMessage += "facilityChargeID is empty";
            if (!string.IsNullOrEmpty(emptyErrorMessage))
                return new WSResponse<SearchFacilityCharge>(null, new Msg(eMsgLevel.Error, emptyErrorMessage));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<SearchFacilityCharge>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            SearchFacilityCharge result = new SearchFacilityCharge();
            result.States = new List<FacilityChargeStateEnum>();
            response.Data = result;
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityInventorySearchCharge));
            try
            {
                Guid facilityChargeIDVal = Guid.Empty;
                if (!Guid.TryParse(facilityChargeID, out facilityChargeIDVal))
                    throw new FormatException("facilityChargeID not in valid format !");
                storageLocationNo = storageLocationNo != CoreWebServiceConst.EmptyParam ? storageLocationNo : "";
                facilityNo = facilityNo != CoreWebServiceConst.EmptyParam ? facilityNo : "";
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    gip.mes.datamodel.FacilityCharge facilityCharge = databaseApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == facilityChargeIDVal);
                    gip.mes.datamodel.FacilityInventoryPos inventoryPos =
                        databaseApp
                        .FacilityInventoryPos
                        .Where(c =>
                                c.FacilityInventory.FacilityInventoryNo == facilityInventoryNo
                                && c.FacilityChargeID == facilityChargeIDVal)
                        .FirstOrDefault();


                    if (facilityCharge != null)
                    {
                        if (inventoryPos != null)
                        {
                            result.FacilityInventoryPos = GetFacilityInventoryPos(inventoryPos);
                            result.States.Add(FacilityChargeStateEnum.Available);

                            if (inventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex == (short)MDFacilityInventoryPosState.FacilityInventoryPosStates.Finished)
                                result.States.Add(FacilityChargeStateEnum.AlreadyFinished);
                        }
                        else
                            result.States.Add(FacilityChargeStateEnum.QuantNotAvailable);

                        if (!string.IsNullOrEmpty(facilityNo))
                        {
                            if (facilityCharge.Facility.FacilityNo != facilityNo)
                            {
                                result.States.Add(FacilityChargeStateEnum.InDifferentFacility);
                                result.DifferentFacilityNo = facilityCharge.Facility.FacilityNo;
                            }
                        }
                        else if (facilityCharge.Facility.Facility1_ParentFacility != null && facilityCharge.Facility.Facility1_ParentFacility.FacilityNo != storageLocationNo)
                        {
                            result.States.Add(FacilityChargeStateEnum.InDifferentFacility);
                            result.DifferentFacilityNo = facilityCharge.Facility.Facility1_ParentFacility.FacilityNo;
                        }
                    }
                    else
                        result.States.Add(FacilityChargeStateEnum.NotExist);
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityInventorySearchCharge(1508)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityInventorySearchCharge));
            }
            return response;
        }

        public WSResponse<FacilityInventoryPos> SetFacilityInventoryChargeAvailable(string facilityInventoryNo, string facilityChargeID)
        {
            WSResponse<FacilityInventoryPos> response = new WSResponse<FacilityInventoryPos>();

            string emptyErrorMessage = "";
            if (string.IsNullOrEmpty(facilityInventoryNo))
                emptyErrorMessage += "facilityInventoryNo is empty";
            if (string.IsNullOrEmpty(facilityChargeID))
                emptyErrorMessage += "facilityChargeID is empty";
            if (!string.IsNullOrEmpty(emptyErrorMessage))
                return new WSResponse<FacilityInventoryPos>(null, new Msg(eMsgLevel.Error, emptyErrorMessage));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<FacilityInventoryPos>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(SetFacilityInventoryChargeAvailable));
            try
            {
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    var subResponse = SetDatabaseUserName<FacilityInventoryPos>(databaseApp);
                    if (subResponse != null)
                        return subResponse;
                    gip.mes.datamodel.FacilityCharge facilityCharge = databaseApp.FacilityCharge.FirstOrDefault(c => c.FacilityChargeID == new Guid(facilityChargeID));
                    if (facilityCharge != null)
                    {
                        gip.mes.datamodel.FacilityInventory facilityInventory = databaseApp.FacilityInventory.FirstOrDefault(c => c.FacilityInventoryNo == facilityInventoryNo);
                        if (facilityInventory != null)
                        {

                            if (facilityCharge.NotAvailable)
                                facilityCharge.NotAvailable = false;

                            gip.mes.datamodel.FacilityInventoryPos mesPos = gip.mes.datamodel.FacilityInventoryPos.NewACObject(databaseApp, facilityInventory);
                            mesPos.FacilityCharge = facilityCharge;
                            mesPos.StockQuantity = facilityCharge.StockQuantityUOM;
                            mesPos.NotAvailable = facilityCharge.NotAvailable;
                            databaseApp.FacilityInventoryPos.AddObject(mesPos);
                            var rez = databaseApp.ACSaveChanges();

                            response.Data = GetFacilityInventoryPos(mesPos);
                        }
                        else
                            response.Message = new Msg(eMsgLevel.Error, string.Format("Facility inventory with no: {0} not exist!", facilityInventoryNo));
                    }
                    else
                        response.Message = new Msg(eMsgLevel.Error, string.Format("Quant with ID: {0} not exist!", facilityChargeID));
                }
            }
            catch (Exception e)
            {
                myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "SetFacilityInventoryChargeAvailables(1540)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            finally
            {
                myServiceHost.OnMethodReturned(perfEvent, nameof(SetFacilityInventoryChargeAvailable));
            }
            return response;
        }

        private FacilityInventoryPos GetFacilityInventoryPos(gip.mes.datamodel.FacilityInventoryPos inventoryPos)
        {
            return new FacilityInventoryPos()
            {
                FacilityInventoryPosID = inventoryPos.FacilityInventoryPosID,
                FacilityChargeID = inventoryPos.FacilityChargeID,
                Sequence = inventoryPos.Sequence,
                Comment = inventoryPos.Comment,
                LotNo = inventoryPos.FacilityCharge.FacilityLot?.LotNo,
                ParentFacilityNo = inventoryPos.FacilityCharge.Facility.Facility1_ParentFacility?.FacilityNo,
                FacilityNo = inventoryPos.FacilityCharge.Facility.FacilityNo,
                FacilityName = inventoryPos.FacilityCharge.Facility.FacilityName,
                MaterialNo = inventoryPos.FacilityCharge.Material.MaterialNo,
                MaterialName = inventoryPos.FacilityCharge.Material.MaterialName1,
                FacilityInventoryNo = inventoryPos.FacilityInventory.FacilityInventoryNo,
                MDFacilityInventoryPosStateIndex = inventoryPos.MDFacilityInventoryPosState.MDFacilityInventoryPosStateIndex,
                StockQuantity = inventoryPos.StockQuantity,
                NewStockQuantity = inventoryPos.NewStockQuantity,
                NotAvailable = inventoryPos.NotAvailable,
                UpdateName = inventoryPos.UpdateName,
                UpdateDate = inventoryPos.UpdateDate
            };
        }

        #endregion

        #endregion

        #endregion

        #region Movement reason

        public static readonly Func<DatabaseApp, IQueryable<MDMovementReason>> s_cQry_GetMovementReasons =
                CompiledQuery.Compile<DatabaseApp, IQueryable<MDMovementReason>>(
                    (dbApp) =>
                        dbApp.MDMovementReason
                             .OrderBy(x => x.SortIndex)
                             .ThenBy(x => x.MDKey)
                             .Select(c => new MDMovementReason()
                             {
                                 MDMovementReasonID = c.MDMovementReasonID,
                                 MDMovementReasonIndex = c.MDMovementReasonIndex,
                                 MDNameTrans = c.MDNameTrans,
                             })
                );

        public virtual WSResponse<List<MDMovementReason>> GetMovementReasons()
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<MDMovementReason>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetMovementReasons));
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<List<MDMovementReason>>(s_cQry_GetMovementReasons(dbApp).ToList());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetMovementReasons) + "(10)", e);
                    return new WSResponse<List<MDMovementReason>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetMovementReasons));
                }
            }
        }

        #endregion

        #region OEEReason

        

        public WSResponse<List<core.webservices.ACClassMessage>> GetOEEReasons(string acClassID)
        {
            if (string.IsNullOrEmpty(acClassID))
                return new WSResponse<List<core.webservices.ACClassMessage>>(null, new Msg(eMsgLevel.Error, "acClassID is empty"));

            Guid guid;
            if (!Guid.TryParse(acClassID, out guid))
                return new WSResponse<List<core.webservices.ACClassMessage>>(null, new Msg(eMsgLevel.Error, "acClassID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<List<core.webservices.ACClassMessage>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetOEEReasons));
            using (Database db = new Database())
            {
                try
                {
                    List<core.webservices.ACClassMessage> result = null;

                    core.datamodel.ACClass acClass = db.ACClass.Include(c => c.ACClassMessage_ACClass).Where(c => c.ACClassID == guid).FirstOrDefault();
                    if (acClass != null)
                    {
                        result = acClass.Messages.Where(c => c.ACIdentifier.StartsWith(PAFWorkTaskScanBase.OEEReasonPrefix)).ToArray()
                                                                                                    .Select(c => new core.webservices.ACClassMessage() 
                                                                                                                 { ACClassMessageID = c.ACClassMessageID, 
                                                                                                                   ACIdentifier = c.ACIdentifier, 
                                                                                                                   ACCaption = c.ACCaption })
                                                                                                    .ToList();
                    }
                    return new WSResponse<List<core.webservices.ACClassMessage>>(result, null);
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetOEEReasons) + "(10)", e);
                    return new WSResponse<List<core.webservices.ACClassMessage>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetOEEReasons));
                }
            }
        }


        #endregion
    }
}
