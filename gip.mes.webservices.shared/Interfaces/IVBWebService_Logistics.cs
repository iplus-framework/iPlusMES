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
#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriPickingTypes, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<MDPickingType>> GetPickingTypes();
#elif NETSTANDARD
        Task<WSResponse<List<MDPickingType>>> GetPickingTypesAsync();
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriPicking, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<Picking>> GetPickings();
#elif NETSTANDARD
        Task<WSResponse<List<Picking>>> GetPickingsAsync();
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriSearchPicking, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<Picking>> SearchPickings(string pType, string fromFacility, string toFacility, string fromDate, string toDate);
#elif NETSTANDARD
        Task<WSResponse<List<Picking>>> SearchPickingsAsync(string pType, string fromFacility, string toFacility, string fromDate, string toDate);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriPickingID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<Picking> GetPicking(string pickingID);
#elif NETSTANDARD
        Task<WSResponse<Picking>> GetPickingAsync(string pickingID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = VBWebServiceConst.UriPicking, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<bool> InsertPicking(Picking item);
#elif NETSTANDARD
        Task<WSResponse<bool>> InsertPickingAsync(Picking item);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = VBWebServiceConst.UriPicking, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<bool> UpdatePicking(Picking item);
#elif NETSTANDARD
        Task<WSResponse<bool>> UpdatePickingAsync(Picking item);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = VBWebServiceConst.UriPickingID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<bool> DeletePicking(string pickingID);
#elif NETSTANDARD
        Task<WSResponse<bool>> DeletePickingAsync(string pickingID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriPickingPosID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<PickingPos> GetPickingPos(string pickingPosID);
#elif NETSTANDARD
        Task<WSResponse<PickingPos>> GetPickingPosAsync(string pickingPosID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriPickingPos_Postings, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<PostingOverview> GetPickingPostingsPos(string pickingPosID);
#elif NETSTANDARD
        Task<WSResponse<PostingOverview>> GetPickingPosPostingsAsync(string pickingPosID);
#endif

        //TODO:Replace with GET method
#if NETFRAMEWORK
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = VBWebServiceConst.UriPickingPos_Material, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<PickingPosList> GetPickingPosByMaterial(PickingPosList pickingPos);
#elif NETSTANDARD
        Task<WSResponse<PickingPosList>> GetPickingPosByMaterialAsync(PickingPosList pickingPos);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = VBWebServiceConst.UriPicking_Finish, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<MsgWithDetails> FinishPickingOrder(Guid pickingID);
#elif NETSTANDARD
        Task<WSResponse<MsgWithDetails>> FinishPickingOrderAsync(Guid pickingID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = VBWebServiceConst.UriPicking_FinishWithoutCheck, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<MsgWithDetails> FinishPickingOrderWithoutCheck(Guid pickingID);
#elif NETSTANDARD
        Task<WSResponse<MsgWithDetails>> FinishPickingOrderWithoutCheckAsync(Guid pickingID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = VBWebServiceConst.UriPicking_FinishAndBook, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<MsgWithDetails> BookAndFinishPickingOrder(PickingWorkplace pickingWorkplace);
#elif NETSTANDARD
        Task<WSResponse<MsgWithDetails>> BookAndFinishPickingOrderAsync(PickingWorkplace pickingWorkplace);
#endif

    }
}
