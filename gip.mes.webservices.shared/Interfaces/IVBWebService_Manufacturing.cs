using gip.core.autocomponent;
using gip.core.datamodel;
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

        #region ProdOrderPartslist
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderPartslists, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<ProdOrderPartslist>> GetProdOrderPartslists();
#elif NETSTANDARD
        Task<WSResponse<List<ProdOrderPartslist>>> GetProdOrderPartslistsAsync();
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderPartslistID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<ProdOrderPartslist> GetProdOrderPartslist(string prodOrderPartslistID);
#elif NETSTANDARD
        Task<WSResponse<ProdOrderPartslist>> GetProdOrderPartslistAsync(string prodOrderPartslistID);
#endif
        #endregion

        #region Intermediate
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderPLIntermediates, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<ProdOrderPartslistPos>> GetProdOrderPLIntermediates(string prodOrderPartslistID);
#elif NETSTANDARD
        Task<WSResponse<List<ProdOrderPartslistPos>>> GetProdOrderPLIntermediatesAsync(string prodOrderPartslistID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderPLIntermediateID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<ProdOrderPartslistPos> GetProdOrderPLIntermediate(string intermediateID);
#elif NETSTANDARD
        Task<WSResponse<ProdOrderPartslistPos>> GetProdOrderPLIntermediateAsync(string intermediateID);
#endif

        #endregion

        #region Intermediate-Batch
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderIntermBatches, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<ProdOrderPartslistPos>> GetProdOrderIntermBatches(string parentIntermediateID);
#elif NETSTANDARD
        Task<WSResponse<List<ProdOrderPartslistPos>>> GetProdOrderIntermBatchesAsync(string parentIntermediateID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderIntermBatchID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<ProdOrderPartslistPos> GetProdOrderIntermBatch(string intermediateID);
#elif NETSTANDARD
        Task<WSResponse<ProdOrderPartslistPos>> GetProdOrderIntermBatchAsync(string intermediateID);
#endif
        #endregion


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderInputMaterials, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<ProdOrderPartslistPosRelation>> GetProdOrderInputMaterials(string targetPOPLPosID);
#elif NETSTANDARD
        Task<WSResponse<List<ProdOrderPartslistPosRelation>>> GetProdOrderInputMaterialsAsync(string targetPOPLPosID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderPosRelFacilityBooking, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<PostingOverview> GetProdOrderPosRelFacilityBooking(string POPLPosRelID);
#elif NETSTANDARD
        Task<WSResponse<PostingOverview>> GetProdOrderPosRelFacilityBookingAsync(string POPLPosRelID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderPosFacilityBooking, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<PostingOverview> GetProdOrderPosFacilityBooking(string POPLPosID);
#elif NETSTANDARD
        Task<WSResponse<PostingOverview>> GetProdOrderPosFacilityBookingAsync(string POPLPosID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderIntermOrIntermBatchByMachine, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<ProdOrderPartslistPos> GetProdOrderIntermOrIntermBatchByMachine(string machineID);
#elif NETSTANDARD
        Task<WSResponse<ProdOrderPartslistPos>> GetProdOrderIntermOrIntermBatchByMachineAsync(string machineID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderPLPos, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<ProdOrderPartslistPos> GetProdOrderPartslistPos(string POPLPosID);
#elif NETSTANDARD
        Task<WSResponse<ProdOrderPartslistPos>> GetProdOrderPartslistPosAsync(string POPLPosID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderPLPosRel, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<ProdOrderPartslistPosRelation> GetProdOrderPartslistPosRel(string POPLPosRelID);
#elif NETSTANDARD
        Task<WSResponse<ProdOrderPartslistPosRelation>> GetProdOrderPartslistPosRelAsync(string POPLPosRelID);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderBatchTargetFacilities, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<Facility>> GetPOBatchTargetFacilities(string intermBatchID);
#elif NETSTANDARD
        Task<WSResponse<List<Facility>>> GetPOBatchTargetFacilitiesAsync(string intermBatchID);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriNFBatchTargetFacility, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<Facility> GetNFBatchTargetFacility(string machineFunctionID);
#elif NETSTANDARD
        Task<WSResponse<Facility>> GetNFBatchTargetFacilityAsync(string machineFunctionID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = VBWebServiceConst.UriProdOrderVerOrderPostings, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<Msg> VerifyOrderPostingsOnRelease(BarcodeEntity entity);
#elif NETSTANDARD
        Task<WSResponse<Msg>> VerifyOrderPostingsOnReleaseAsync(BarcodeEntity entity);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriProdOrderAvailableFC, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<FacilityCharge>> GetPOAvaialbleFC(string machineFunctionID, string POPLPosRelID);
#elif NETSTANDARD
        Task<WSResponse<List<FacilityCharge>>> GetPOAvaialbleFCAsync(string machineFunctionID, string POPLPosRelID);
#endif
    }
}   
