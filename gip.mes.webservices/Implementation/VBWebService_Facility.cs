using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.datamodel;
using gip.mes.facility;
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
                .Where(c => !facilityChargeID.HasValue || c.FacilityChargeID == facilityChargeID.Value)
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
                        StorageLife = c.FacilityLot != null ? c.FacilityLot.StorageLife : (short)0
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
                        MDUnitNameTrans = c.MDUnit.MDUnitNameTrans
                    },
                    MDReleaseState = new gip.mes.webservices.MDReleaseState()
                    {
                        MDReleaseStateID = c.MDReleaseStateID.HasValue ? c.MDReleaseStateID.Value : Guid.Empty,
                        MDNameTrans = c.MDReleaseState != null ? c.MDReleaseState.MDNameTrans : ""
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
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<List<FacilityCharge>>(s_cQry_GetFacilityCharge(dbApp, null).ToList());
                }
                catch (Exception e)
                {
                    PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityCharges(10)", e);
                    return new WSResponse<List<FacilityCharge>>(null, new Msg(eMsgLevel.Exception, e.Message));
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


            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<FacilityCharge>(s_cQry_GetFacilityCharge(dbApp, guid).FirstOrDefault());
                }
                catch (Exception e)
                {
                    PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityCharge(10)", e);
                    return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
            }
        }

        public WSResponse<FacilityCharge> GetFacilityChargeByBarcode(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "barcodeID is empty"));
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    Guid guid = facManager.ResolveFacilityChargeIdFromBarcode(dbApp, barcodeID);
                    if (guid == Guid.Empty)
                        return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Error, "Coudn't resolve barcodeID"));
                    return new WSResponse<FacilityCharge>(s_cQry_GetFacilityCharge(dbApp, guid).FirstOrDefault());
                }
                catch (Exception e)
                {
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityChargeByBarcode(10)", e);
                    return new WSResponse<FacilityCharge>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
            }
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

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

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
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityChargeBookings(10)", e);
                    return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
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
                    StorageLife = c.FacilityLot != null ? c.FacilityLot.StorageLife : (short)0
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
                    MDUnitNameTrans = c.MDUnit.MDUnitNameTrans
                },
                MDReleaseState = new gip.mes.webservices.MDReleaseState()
                {
                    MDReleaseStateID = c.MDReleaseStateID.HasValue ? c.MDReleaseStateID.Value : Guid.Empty,
                    MDNameTrans = c.MDReleaseState != null ? c.MDReleaseState.MDNameTrans : ""
                },
                FillingDate = c.FillingDate,
                StorageLife = c.StorageLife,
                ProductionDate = c.ProductionDate,
                ExpirationDate = c.ExpirationDate,
                Comment = c.Comment,
                NotAvailable = c.NotAvailable
            };
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
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<List<FacilityLot>>(s_cQry_GetFacilityLots(dbApp).ToList());
                }
                catch (Exception e)
                {
                    PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "FacilityLot(10)", e);
                    return new WSResponse<List<FacilityLot>>(null, new Msg(eMsgLevel.Exception, e.Message));
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

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<FacilityLot>(s_cQry_GetFacilityLot(dbApp, guid, null).FirstOrDefault());
                }
                catch (Exception e)
                {
                    PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLot(10)", e);
                    return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
            }
        }


        public WSResponse<List<FacilityLot>> SearchFacilityLot(string term)
        {
            if (string.IsNullOrEmpty(term) || term == CoreWebServiceConst.EmptyParam)
                term = null;

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<List<FacilityLot>>(s_cQry_GetFacilityLot(dbApp, null, term).ToList());
                }
                catch (Exception e)
                {
                    PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "SearchFacilityLot(10)", e);
                    return new WSResponse<List<FacilityLot>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
            }
        }


        public WSResponse<FacilityLot> GetFacilityLotByBarcode(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Error, "barcodeID is empty"));
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    Guid guid = facManager.ResolveFacilityLotIdFromBarcode(dbApp, barcodeID);
                    if (guid == Guid.Empty)
                        return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Error, "Coudn't resolve barcodeID"));
                    return new WSResponse<FacilityLot>(s_cQry_GetFacilityLot(dbApp, guid, null).FirstOrDefault());
                }
                catch (Exception e)
                {
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLotByBarcode(10)", e);
                    return new WSResponse<FacilityLot>(null, new Msg(eMsgLevel.Exception, e.Message));
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

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<FacilityLotSumOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

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
                    overview.FacilityCharges = ConvertFacilityChargeList(facilityChargeList);
                    return new WSResponse<FacilityLotSumOverview>(overview);
                }
                catch (Exception e)
                {
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLotSum(10)", e);
                    return new WSResponse<FacilityLotSumOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
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

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

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
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLotBookings(10)", e);
                    return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
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

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<MaterialSumOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<MaterialSumOverview>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    MaterialSumOverview overview = new MaterialSumOverview();
                    overview.MaterialStock = s_cQry_GetMaterialStock(dbApp, guid).FirstOrDefault();
                    gip.mes.datamodel.FacilityCharge[] facilityChargeList = FacilityManager.s_cQry_MatOverviewFacilityCharge(dbApp, guid, false).ToArray();
                    overview.FacilityLocationSum = facManager.GetFacilityChargeSumLocationHelperList(facilityChargeList, new FacilityQueryFilter() { MaterialID = guid });
                    overview.FacilitySum = facManager.GetFacilityChargeSumFacilityHelperList(facilityChargeList, new FacilityQueryFilter());
                    overview.FacilityLotSum = facManager.GetFacilityChargeSumLotHelperList(facilityChargeList, new FacilityQueryFilter() { MaterialID = guid });
                    overview.FacilityLocationSum = facManager.GetFacilityChargeSumLocationHelperList(facilityChargeList, new FacilityQueryFilter());
                    overview.FacilityCharges = ConvertFacilityChargeList(facilityChargeList);
                    return new WSResponse<MaterialSumOverview>(overview);
                }
                catch (Exception e)
                {
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetMaterialSum(10)", e);
                    return new WSResponse<MaterialSumOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
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

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

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
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetMaterialBookings(10)", e);
                    return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
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

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<FacilitySumOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

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
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilitySum(10)", e);
                    return new WSResponse<FacilitySumOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
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

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

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
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityBookings(10)", e);
                    return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
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

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<FacilityLocationSumOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

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
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLotSum(10)", e);
                    return new WSResponse<FacilityLocationSumOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
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

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

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
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLocationBookings(10)", e);
                    return new WSResponse<PostingOverview>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
            }
        }

        #endregion

        #region Booking
        public WSResponse<MsgWithDetails> BookFacility(ACMethodBooking bpParam)
        {
            //if (bpParam == null)
            //    return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "Booking parameter is null"));
            //if (String.IsNullOrEmpty(bpParam.VirtualMethodName))
            //    return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "VirtualMethodName is empty"));
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            MsgWithDetails msg;

            using (var dbApp = new DatabaseApp())
            {
                 msg = Book(bpParam, dbApp, facManager, myServiceHost);
            }
            return new WSResponse<MsgWithDetails>(msg);
        }

        private MsgWithDetails Book(ACMethodBooking bpParam, DatabaseApp dbApp, FacilityManager facManager, PAJsonServiceHostVB myServiceHost)
        {
            if (bpParam == null)
                return new MsgWithDetails("Booking parameter is null", null, eMsgLevel.Error, "VBWebService", "Book", 1060);

            if (String.IsNullOrEmpty(bpParam.VirtualMethodName))
                return new MsgWithDetails("VirtualMethodName is empty", null, eMsgLevel.Error, "VBWebService", "Book", 1063);

            MsgWithDetails msgWithDetails = null;
            try
            {
                datamodel.PickingPos pickingPos = bpParam.PickingPosID.HasValue ? dbApp.PickingPos.FirstOrDefault(c => c.PickingPosID == bpParam.PickingPosID.Value) : null;
                facility.ACMethodBooking preBookingMethod = null;
                FacilityPreBooking preBooking = null;
                if (pickingPos != null)
                {
                    preBooking = pickingPos.FacilityPreBooking_PickingPos.FirstOrDefault();
                    if (pickingPos.FacilityPreBooking_PickingPos.Count > 1)
                    {
                        preBooking = null;
                        //TODO: warning message
                    }

                    if (preBooking == null && pickingPos.InOrderPos != null)
                    {
                        preBooking = pickingPos.InOrderPos.FacilityPreBooking_InOrderPos.FirstOrDefault();
                        if (pickingPos.InOrderPos.FacilityPreBooking_InOrderPos.Count > 1)
                        {
                            preBooking = null;
                            //TODO: warning message
                        }
                    }

                    if (preBooking == null && pickingPos.OutOrderPos != null)
                    {
                        preBooking = pickingPos.OutOrderPos.FacilityPreBooking_OutOrderPos.FirstOrDefault();
                        if (pickingPos.OutOrderPos.FacilityPreBooking_OutOrderPos.Count > 1)
                        {
                            preBooking = null;
                            //TODO: warning message
                        }
                    }

                    if (preBooking != null)
                    {
                        preBookingMethod = preBooking.ACMethodBooking as facility.ACMethodBooking;
                    }
                }

                // TODO:  Use ACPickingManager InitBookingParamsFromTemplate


                facility.ACMethodBooking acParam = facManager.ACUrlACTypeSignature("!" + bpParam.VirtualMethodName, gip.core.datamodel.Database.GlobalDatabase) as facility.ACMethodBooking;

                if (preBookingMethod != null)
                {
                    acParam = preBookingMethod;
                    if (bpParam.InwardFacilityID.HasValue)
                    {
                        acParam.InwardFacility = dbApp.Facility.Where(c => c.FacilityID == bpParam.InwardFacilityID.Value).FirstOrDefault();

                        //if (pickingPos != null && pickingPos.ToFacilityID.HasValue 
                        //                       && (pickingPos.Picking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.Receipt
                        //                        || pickingPos.Picking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.ReceiptVehicle))
                        //{
                        //    bool isInTargetFacility = acParam.InwardFacility.IsLocatedIn(pickingPos.ToFacilityID.Value);
                        //    if (!isInTargetFacility)
                        //    {
                        //        return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "The scanned or selected facility is not located under "+ pickingPos.ToFacility.ToString()));
                        //    }
                        //}
                    }

                    acParam.OutwardQuantity = bpParam.OutwardQuantity;
                    acParam.InwardQuantity = bpParam.InwardQuantity;
                }
                else
                {
                    //acParam = acParam.Clone() as ACMethodBooking;
                    acParam.ShiftBookingReverse = bpParam.ShiftBookingReverse;
                    //acParam.BookingType = bpParam.BookingType;
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
                    if (bpParam.MovementReasonIndex.HasValue)
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
                        acParam.InwardFacilityCharge = dbApp.FacilityCharge.Where(c => c.FacilityChargeID == bpParam.InwardFacilityChargeID.Value).FirstOrDefault();
                    if (bpParam.InwardFacilityLocationID.HasValue)
                        acParam.InwardFacilityLocation = dbApp.Facility.Where(c => c.FacilityID == bpParam.InwardFacilityLocationID.Value).FirstOrDefault();
                    if (bpParam.InwardPartslistID.HasValue)
                        acParam.InwardPartslist = dbApp.Partslist.Where(c => c.PartslistID == bpParam.InwardPartslistID.Value).FirstOrDefault();
                    if (bpParam.InwardCompanyMaterialID.HasValue)
                        acParam.InwardCompanyMaterial = dbApp.CompanyMaterial.Where(c => c.CompanyMaterialID == bpParam.InwardCompanyMaterialID.Value).FirstOrDefault();
                    acParam.InwardSplitNo = bpParam.InwardSplitNo;
                    acParam.InwardQuantity = bpParam.InwardQuantity;
                    acParam.InwardTargetQuantity = bpParam.InwardTargetQuantity;

                    if (bpParam.OutwardMaterialID.HasValue)
                        acParam.OutwardMaterial = dbApp.Material.Where(c => c.MaterialID == bpParam.OutwardMaterialID.Value).FirstOrDefault();
                    if (bpParam.OutwardFacilityID.HasValue)
                        acParam.OutwardFacility = dbApp.Facility.Where(c => c.FacilityID == bpParam.OutwardFacilityID.Value).FirstOrDefault();
                    if (bpParam.OutwardFacilityLotID.HasValue)
                        acParam.OutwardFacilityLot = dbApp.FacilityLot.Where(c => c.FacilityLotID == bpParam.OutwardFacilityLotID.Value).FirstOrDefault();
                    if (bpParam.OutwardFacilityChargeID.HasValue)
                        acParam.OutwardFacilityCharge = dbApp.FacilityCharge.Where(c => c.FacilityChargeID == bpParam.OutwardFacilityChargeID.Value).FirstOrDefault();
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
                        acParam.PartslistPosRelation = dbApp.ProdOrderPartslistPosRelation.Where(c => c.ProdOrderPartslistPosRelationID == bpParam.PartslistPosRelationID.Value).FirstOrDefault();
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
                }

                if (pickingPos != null)
                {
                    acParam.PickingPos = pickingPos;
                    acParam.InOrderPos = pickingPos.InOrderPos;
                    acParam.OutOrderPos = pickingPos.OutOrderPos;
                }

                var resultBooking = facManager.BookFacilityWithRetry(ref acParam, dbApp);
                if (resultBooking.ResultState == Global.ACMethodResultState.Failed || resultBooking.ResultState == Global.ACMethodResultState.Notpossible)
                {
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "BookFacility(10)", acParam.ValidMessage.InnerMessage);
                    dbApp.ACUndoChanges();
                    msgWithDetails = acParam.ValidMessage;
                    if (msgWithDetails != null)
                        msgWithDetails.AddDetailMessage(new Msg(eMsgLevel.Exception, acParam.ValidMessage.InnerMessage));

                    return msgWithDetails;
                }
                else
                {
                    if (acParam.PickingPos != null)
                    {
                        ACPickingManager pickingManager = ACPickingManager.GetServiceInstance(myServiceHost);
                        if (pickingManager != null)
                        {
                            double postedQuantity = 0;
                            if (acParam.OutwardQuantity.HasValue)
                                postedQuantity = acParam.OutwardQuantity.Value;
                            else if (acParam.InwardQuantity.HasValue)
                                postedQuantity = acParam.InwardQuantity.Value;
                            pickingManager.RecalcAfterPosting(dbApp, acParam.PickingPos, postedQuantity, false, true);
                        }
                        msgWithDetails = dbApp.ACSaveChangesWithRetry();
                    }
                    else if (acParam.PartslistPos != null)
                    {
                        acParam.PartslistPos.RecalcActualQuantity();
                        msgWithDetails = dbApp.ACSaveChangesWithRetry();
                    }
                    else if (acParam.PartslistPosRelation != null)
                    {
                        acParam.PartslistPosRelation.RecalcActualQuantity();
                        msgWithDetails = dbApp.ACSaveChangesWithRetry();
                    }
                }
            }
            catch (Exception e)
            {
                if (myServiceHost != null)
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityLocationBookings(10)", e);
                msgWithDetails = new MsgWithDetails();
                msgWithDetails.AddDetailMessage(new Msg(eMsgLevel.Exception, e.Message));
                return msgWithDetails;
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

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<MsgWithDetails>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            using (DatabaseApp dbApp = new DatabaseApp())
            {

                foreach (ACMethodBooking bpParam in bpParams)
                {
                    MsgWithDetails msg = Book(bpParam, dbApp, facManager, myServiceHost);
                    if (msg != null)
                    {
                        foreach (Msg m in msg.MsgDetails)
                        {
                            msgWithDetails.AddDetailMessage(m);
                        }
                    }
                }
            }
            return new WSResponse<MsgWithDetails>(msgWithDetails);
        }

        #endregion

        #region Inventory

        #region Inventory -> MD
        public WSResponse<List<MDFacilityInventoryState>> GetMDFacilityInventoryStates()
        {
            WSResponse<List<MDFacilityInventoryState>> response = new WSResponse<List<MDFacilityInventoryState>>();
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
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                if (myServiceHost != null)
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetMDFacilityInventoryStates(973)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            Console.WriteLine("web service: GetMDFacilityInventoryStates");

            return response;
        }

        public WSResponse<List<MDFacilityInventoryPosState>> GetMDFacilityInventoryPosStates()
        {
            WSResponse<List<MDFacilityInventoryPosState>> response = new WSResponse<List<MDFacilityInventoryPosState>>();
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
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                if (myServiceHost != null)
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetMDFacilityInventoryStates(973)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            Console.WriteLine("web service: GetMDFacilityInventoryPosStates");
            return response;
        }
        #endregion

        #region Inventory -> Get

        public static readonly Func<DatabaseApp, short?, DateTime, DateTime, IQueryable<FacilityInventory>> s_cQry_GetFacilityInventories =
        CompiledQuery.Compile<DatabaseApp, short?, DateTime, DateTime, IQueryable<FacilityInventory>>(
            (dbApp, inventoryState, dateFrom, dateTo) =>
                dbApp.FacilityInventory
                        .Where(c =>
                             (inventoryState == null || c.MDFacilityInventoryState.MDFacilityInventoryStateIndex == inventoryState)
                             && c.InsertDate >= dateFrom
                             && c.InsertDate < dateTo
                        )
                        .Select(c => new FacilityInventory()
                        {
                            FacilityInventoryID = c.FacilityInventoryID,
                            FacilityInventoryNo = c.FacilityInventoryNo,
                            FacilityInventoryName = c.FacilityInventoryName,
                            InsertName = c.InsertName,
                            InsertDate = c.InsertDate,
                            MDFacilityInventoryState = new gip.mes.webservices.MDFacilityInventoryState()
                            {
                                MDFacilityInventoryStateID = c.MDFacilityInventoryStateID,
                                MDNameTrans = c.MDFacilityInventoryState.MDNameTrans,
                                MDFacilityInventoryStateIndex = c.MDFacilityInventoryState.MDFacilityInventoryStateIndex,
                                SortIndex = c.MDFacilityInventoryState.SortIndex,
                                IsDefault = c.MDFacilityInventoryState.IsDefault
                            }
                        })
        );

        public WSResponse<List<FacilityInventory>> GetFacilityInventories(string inventoryState, string dateFrom, string dateTo)
        {
            WSResponse<List<FacilityInventory>> response = new WSResponse<List<FacilityInventory>>();
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
                    response.Data = s_cQry_GetFacilityInventories(databaseApp, inventoryStateVal, dtFrom, dtTo).ToList();
                }
            }
            catch (Exception e)
            {
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                if (myServiceHost != null)
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityInventories(1067)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            Console.WriteLine("web service: GetFacilityInventories");
            return response;
        }

        #endregion

        #region Inventory -> New
        public WSResponse<string> GetFacilityInventoryNo()
        {
            WSResponse<string> response = new WSResponse<string>();
            try
            {
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                response.Data = facManager.GetNewInventoryNo();
            }
            catch (Exception e)
            {
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                if (myServiceHost != null)
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityInventoryNo(1090)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            return response;
        }

        public WSResponse<bool> NewFacilityInventory(string facilityInventoryNo, string facilityInventoryName)
        {
            WSResponse<bool> response = new WSResponse<bool>();
            try
            {
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                MsgWithDetails msgWithDetails = facManager.InventoryGenerate(facilityInventoryNo, facilityInventoryName, null, null);
                response.Data = msgWithDetails.IsSucceded();
                response.Message = new Msg { MessageLevel = msgWithDetails.MessageLevel, Message = msgWithDetails.Message };
            }
            catch (Exception e)
            {
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                if (myServiceHost != null)
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "NewFacilityInventory(1109)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            return response;
        }
        #endregion

        #region Inventory -> Lifecycle
        public WSResponse<bool> StartFacilityInventory(string facilityInventoryNo)
        {
            WSResponse<bool> response = new WSResponse<bool>();
            try
            {
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    mes.datamodel.MDFacilityInventoryState inProgressState = databaseApp.MDFacilityInventoryState.FirstOrDefault(c => c.MDFacilityInventoryStateIndex == (short)mes.datamodel.MDFacilityInventoryState.FacilityInventoryStates.InProgress);
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
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                if (myServiceHost != null)
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "StartFacilityInventory(1132)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            return response;
        }

        public WSResponse<bool> CloseFacilityInventory(string facilityInventoryNo)
        {
            WSResponse<bool> response = new WSResponse<bool>();
            try
            {
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    mes.datamodel.MDFacilityInventoryState finishedState = databaseApp.MDFacilityInventoryState.FirstOrDefault(c => c.MDFacilityInventoryStateIndex == (short)mes.datamodel.MDFacilityInventoryState.FacilityInventoryStates.Finished);
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
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                if (myServiceHost != null)
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "CloseFacilityInventory(1168)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
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
            WSResponse<List<FacilityInventoryPos>> response = new WSResponse<List<FacilityInventoryPos>>();
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
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                if (myServiceHost != null)
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityInventoryLines(1361)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            Console.WriteLine("web service: GetFacilityInventoryLines");
            return response;
        }

        #endregion

        #region Inventory -> Pos -> Lifecycle
        public WSResponse<bool> UpdateFacilityInventoryPos(FacilityInventoryPos facilityInventoryPos)
        {
            WSResponse<bool> response = new WSResponse<bool>();
            try
            {
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
                    mes.datamodel.FacilityInventoryPos dbFacilityInventoryPos =
                        databaseApp
                        .FacilityInventoryPos
                        .Where(c => c.FacilityInventoryPosID == facilityInventoryPos.FacilityInventoryPosID)
                        .FirstOrDefault();

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
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                if (myServiceHost != null)
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "UpdateFacilityInventoryPos(1430)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
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

            SearchFacilityCharge result = new SearchFacilityCharge();
            result.States = new List<FacilityChargeStateEnum>();
            response.Data = result;
            try
            {
                Guid facilityChargeIDVal = Guid.Empty;
                if(!Guid.TryParse(facilityChargeID, out facilityChargeIDVal))
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
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                if (myServiceHost != null)
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityInventorySearchCharge(1508)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            Console.WriteLine("web service: GetFacilityInventorySearchCharge");
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

            try
            {
                using (DatabaseApp databaseApp = new DatabaseApp())
                {
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
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                if (myServiceHost != null)
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "SetFacilityInventoryChargeAvailables(1540)", e);
                response.Message = new Msg() { MessageLevel = eMsgLevel.Error, Message = e.Message };
            }
            Console.WriteLine("web service: SetFacilityInventoryChargeAvailables");
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

    }
}
