using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
#if NETFRAMEWORK
using System.ServiceModel;
using System.ServiceModel.Web;
#elif NETSTANDARD
using System.Threading.Tasks;
#endif

namespace gip.mes.webservices
{
    partial interface IVBWebService
    {
        #region FacilityCharge

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacilityCharge, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<FacilityCharge>> GetFacilityCharges();
#elif NETSTANDARD
        Task<WSResponse<List<FacilityCharge>>> GetFacilityChargesAsync();
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacilityChargeID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<FacilityCharge> GetFacilityCharge(string facilityChargeID);
#elif NETSTANDARD
        Task<WSResponse<FacilityCharge>> GetFacilityChargeAsync(string facilityChargeID);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacilityCharge_BarcodeID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<FacilityCharge> GetFacilityChargeByBarcode(string barcodeID);
#elif NETSTANDARD
        Task<WSResponse<FacilityCharge>> GetFacilityChargeByBarcodeAsync(string barcodeID);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacilityCharge_Bookings, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<PostingOverview> GetFacilityChargeBookings(string facilityChargeID, string dateFrom, string dateTo);
#elif NETSTANDARD
        Task<WSResponse<PostingOverview>> GetFacilityChargeBookingsAsync(string facilityChargeID, string dateFrom, string dateTo);
#endif

        #endregion


        #region FacilityLot

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacilityLot, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<FacilityLot>> GetFacilityLots();
#elif NETSTANDARD
        Task<WSResponse<List<FacilityLot>>> GetFacilityLotsAsync();
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacilityLotID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<FacilityLot> GetFacilityLot(string facilityLotID);
#elif NETSTANDARD
        Task<WSResponse<FacilityLot>> GetFacilityLotAsync(string facilityLotID);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacilityLot_Search, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<FacilityLot>> SearchFacilityLot(string term);
#elif NETSTANDARD
        Task<WSResponse<List<FacilityLot>>> SearchFacilityLotAsync(string term);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacilityLot_BarcodeID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<FacilityLot> GetFacilityLotByBarcode(string barcodeID);
#elif NETSTANDARD
        Task<WSResponse<FacilityLot>> GetFacilityLotByBarcodeAsync(string barcodeID);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacilityLot_SumID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<FacilityLotSumOverview> GetFacilityLotSum(string facilityLotID);
#elif NETSTANDARD
        Task<WSResponse<FacilityLotSumOverview>> GetFacilityLotSumAsync(string facilityLotID);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacilityLot_Bookings, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<PostingOverview> GetFacilityLotBookings(string facilityLotID, string dateFrom, string dateTo);
#elif NETSTANDARD
        Task<WSResponse<PostingOverview>> GetFacilityLotBookingsAsync(string facilityLotID, string dateFrom, string dateTo);
#endif

        #endregion


        #region Material

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriMaterial_SumID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<MaterialSumOverview> GetMaterialSum(string materialID);
#elif NETSTANDARD
        Task<WSResponse<MaterialSumOverview>> GetMaterialSumAsync(string materialID);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriMaterial_Bookings, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<PostingOverview> GetMaterialBookings(string materialID, string dateFrom, string dateTo);
#elif NETSTANDARD
        Task<WSResponse<PostingOverview>> GetMaterialBookingsAsync(string materialID, string dateFrom, string dateTo);
#endif

        #endregion


        #region Facility

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacility_SumID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<FacilitySumOverview> GetFacilitySum(string facilityID);
#elif NETSTANDARD
        Task<WSResponse<FacilitySumOverview>> GetFacilitySumAsync(string facilityID);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacility_Bookings, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<PostingOverview> GetFacilityBookings(string facilityID, string dateFrom, string dateTo);
#elif NETSTANDARD
        Task<WSResponse<PostingOverview>> GetFacilityBookingsAsync(string facilityID, string dateFrom, string dateTo);
#endif

        #endregion


        #region FacilityLocation

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacilityLocation_SumID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<FacilityLocationSumOverview> GetFacilityLocationSum(string facilityID);
#elif NETSTANDARD
        Task<WSResponse<FacilityLocationSumOverview>> GetFacilityLocationSumAsync(string facilityID);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacilityLocation_Bookings, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<PostingOverview> GetFacilityLocationBookings(string facilityID, string dateFrom, string dateTo);
#elif NETSTANDARD
        Task<WSResponse<PostingOverview>> GetFacilityLocationBookingsAsync(string facilityID, string dateFrom, string dateTo);
#endif

        #endregion


        #region Booking
#if NETFRAMEWORK
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = VBWebServiceConst.UriBookFacility, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<MsgWithDetails> BookFacility(ACMethodBooking bpParam);
#elif NETSTANDARD
        Task<WSResponse<MsgWithDetails>> BookFacilityAsync(ACMethodBooking bpParam);
#endif
        #endregion

        #region Inventory


        // *** Template ***
        //#if NETFRAMEWORK
        //        [OperationContract]
        //        [WebGet(UriTemplate = VBWebServiceConst.{URL}, ResponseFormat = WebMessageFormat.Json)]
        //        WSResponse<{ResponseType}> {MethodName}({params});
        //#elif NETSTANDARD
        //        Task<WSResponse<{ResponseType}>> {MethodName}Async({params});
        //#endif

        #region Inventory-> MD
        //public const string UrlInventory_MDFacilityInventoryState = "FacilityInventory/Get/MDFacilityInventoryState";
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UrlInventory_MDFacilityInventoryState, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<MDFacilityInventoryState>> GetMDFacilityInventoryStates();
#elif NETSTANDARD
        Task<WSResponse<List<MDFacilityInventoryState>>> GetMDFacilityInventoryStatesAsync();
#endif

        //public const string UrlInventory_MDFacilityInventoryPosState = "FacilityInventory/Get/MDFacilityInventoryPosState";
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UrlInventory_MDFacilityInventoryPosState, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<MDFacilityInventoryPosState>> GetMDFacilityInventoryPosStates();
#elif NETSTANDARD
        Task<WSResponse<List<MDFacilityInventoryPosState>>> GetMDFacilityInventoryPosStatesAsync();
#endif
        #endregion

        #region Inventory -> Get

        // WSResponse<List<FacilityInventory>> GetFacilityInventories (short? inventoryState, DateTime dateFrom, DateTime dateTo)
        //public const string UrlInventory_Inventories = "FacilityInventory/FacilityInventories/InventoryState/{inventoryState}/from/{dateFrom}/to/{dateTo}";
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UrlInventory_Inventories, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<FacilityInventory>> GetFacilityInventories(string inventoryState, string dateFrom, string dateTo);
#elif NETSTANDARD
        Task<WSResponse<List<FacilityInventory>>> GetFacilityInventoriesAsync(string inventoryState, string dateFrom, string dateTo);
#endif

        #endregion

        #region Inventory -> New
        // WSResponse<string> GetFacilityInventoryNo ()
        //public const string UrlInventory_GetInventoryNo = "FacilityInventory/Get/FacilityInventoryNo";
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UrlInventory_GetInventoryNo, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<string> GetFacilityInventoryNo();
#elif NETSTANDARD
       Task<WSResponse<string>> GetFacilityInventoryNoAsync();
#endif

        // WSResponse NewFacilityInventory (string facilityInventoryNo, string facilityInventoryName)
        // public const string UrlInventory_New = "FacilityInventory/New/FacilityInventoryNo/{facilityInventoryNo}/FacilityInventoryName/{facilityInventoryName}";
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UrlInventory_New, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<bool> NewFacilityInventory(string facilityInventoryNo, string facilityInventoryName);
#elif NETSTANDARD
       Task<WSResponse<bool>> NewFacilityInventoryAsync(string facilityInventoryNo, string facilityInventoryName);
#endif
        #endregion

        #region Inventory -> Lifecycle
        // WSResponse<bool> StartFacilityInventory (string facilityInventoryNo)
        //public const string UrlInventory_Start = "FacilityInventory/Start/FacilityInventoryNo/{facilityInventoryNo}";
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UrlInventory_Start, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<bool> StartFacilityInventory(string facilityInventoryNo);
#elif NETSTANDARD
       Task<WSResponse<bool>> StartFacilityInventoryAsync(string facilityInventoryNo);
#endif

        // WSResponse<bool> CloseFacilityInventory (string facilityInventoryNo)
        //public const string UrlInventory_Close = "FacilityInventory/Close/FacilityInventoryNo/{facilityInventoryNo}";
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UrlInventory_Close, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<bool> CloseFacilityInventory(string facilityInventoryNo);
#elif NETSTANDARD
       Task<WSResponse<bool>> CloseFacilityInventoryAsync(string facilityInventoryNo);
#endif
        #endregion

        #region Inventory -> Pos
        #region Inventory -> Pos - Get
        // WSResponse<List<FacilityInventoryPos>> GetFacilityInventoryPoses (string facilityInventoryNo, string inputCode, string facilityNo, string lotNo, string materialNo, string inventoryPosState, string notAvailable, string zeroStock);
        //public const string UrlInventory_InventoryPoses = "FacilityInventory/FacilityInventoryNo/{facilityInventoryNo}/InputCode/{inputCode}/FacilityNo/{facilityNo}/LotNo/{lotNo}/MaterialNo/{materialNo}/InventoryPosState/{inventoryPosState}/NotAvailable/{notAvailable}/ZeroStock/{zeroStock}";
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UrlInventory_InventoryPoses, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<FacilityInventoryPos>> GetFacilityInventoryPoses(string facilityInventoryNo, string inputCode, string facilityNo, string lotNo, string materialNo, string inventoryPosState, string notAvailable, string zeroStock, string notProcessed);
#elif NETSTANDARD
        Task<WSResponse<List<FacilityInventoryPos>>> GetFacilityInventoryPosesAsync(string facilityInventoryNo, string inputCode, string facilityNo, string lotNo, string materialNo, string inventoryPosState, string notAvailable, string zeroStock, string notProcessed);
#endif

        #endregion

        #region Inventory -> Pos -> Lifecycle
        // WSResponse<bool> UpdateFacilityInventoryPos (FacilityInventoryPos facilityInventoryPos)
        //public const string UrlInventory_InventoryPos_Update = "FacilityInventoryPos/Update";
#if NETFRAMEWORK
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = VBWebServiceConst.UrlInventory_InventoryPos_Update,RequestFormat =WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<bool> UpdateFacilityInventoryPos(FacilityInventoryPos facilityInventoryPos);
#elif NETSTANDARD
        Task<WSResponse<bool>> UpdateFacilityInventoryPosAsync(FacilityInventoryPos facilityInventoryPos);
#endif

        // WSResponse<bool> StartFacilityInventoryPos (string facilityInventoryNo, Guid facilityChargeID)
        //public const string UrlInventory_InventoryPos_Start = "FacilityInventoryPos/Start/FacilityInventoryNo/{facilityInventoryNo}/FacilityChargeID/{facilityChargeID}";
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UrlInventory_InventoryPos_Start, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<bool> StartFacilityInventoryPos(string facilityInventoryNo, string facilityChargeID);
#elif NETSTANDARD
       Task<WSResponse<bool>> StartFacilityInventoryPosAsync(string facilityInventoryNo, string facilityChargeID);
#endif

        // WSResponse<bool> CloseFacilityInventoryPos (string facilityInventoryNo, Guid facilityChargeID)
        //public const string UrlInventory_InventoryPos_Close = "FacilityInventoryPos/Close/FacilityInventoryNo/{facilityInventoryNo}/FacilityChargeID/{facilityChargeID}";
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UrlInventory_InventoryPos_Close, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<bool> CloseFacilityInventoryPos(string facilityInventoryNo, string facilityChargeID);
#elif NETSTANDARD
       Task<WSResponse<bool>> CloseFacilityInventoryPosAsync(string facilityInventoryNo, string facilityChargeID);
#endif
        #endregion

        #region Inventory -> Pos -> Booings
        // WSResponse<PostingOverview> GetFacilityInventoryPosBookings(string facilityInventoryNo, Guid facilityChargeID)
        //public const string UrlInventory_InventoryPos_Bookings = "FacilityInventoryPos/FacilityInventoryNo/{facilityInventoryNo}/FacilityChargeID/{facilityChargeID}/Bookings";
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UrlInventory_InventoryPos_Bookings, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<PostingOverview> GetFacilityInventoryPosBookings(string facilityInventoryNo, string facilityChargeID);
#elif NETSTANDARD
       Task<WSResponse<PostingOverview>> GetFacilityInventoryPosBookingsAsync(string facilityInventoryNo, string facilityChargeID);
#endif
        #endregion

        #endregion

        #endregion
    }
}
