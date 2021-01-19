using System;
using gip.core.autocomponent;
#if NETFRAMEWORK
using System.ServiceModel;
using System.ServiceModel.Web;
#elif NETSTANDARD
using System.Threading.Tasks;
#endif
using gip.core.webservices;

namespace gip.mes.webservices
{
#if NETFRAMEWORK
    [ServiceContract]
#endif
    public partial interface IVBWebService : gip.core.webservices.ICoreWebService
    {
        #region General

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriBarcodeEntity_BarcodeID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<BarcodeEntity> GetBarcodeEntity(string barcodeID);
#elif NETSTANDARD
        Task<WSResponse<BarcodeEntity>> GetBarcodeEntityAsync(string barcodeID);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = VBWebServiceConst.UriBarcodeSequence, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<BarcodeSequence> InvokeBarcodeSequence(BarcodeSequence sequence);
#elif NETSTANDARD
        Task<WSResponse<BarcodeSequence>> InvokeBarcodeSequenceAsync(BarcodeSequence sequence);
#endif

        #endregion
    }
}
