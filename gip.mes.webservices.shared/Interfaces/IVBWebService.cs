// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using gip.core.autocomponent;
#if NETFRAMEWORK
using CoreWCF;
using CoreWCF.Web;
//using System.ServiceModel;
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
        //[WebGet(UriTemplate = VBWebServiceConst.UriBarcodeEntity_BarcodeID, ResponseFormat = WebMessageFormat.Json)]
        [WebInvoke(Method = "POST", UriTemplate = VBWebServiceConst.UriBarcodeEntity, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
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

        #region Print & PrintSettings

#if NETFRAMEWORK
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = VBWebServiceConst.UriPrint, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<bool> Print(PrintEntity printEntity);
#elif NETSTANDARD
        Task<WSResponse<bool>> Print(PrintEntity printEntity);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriGetAssignedPrinter, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<string> GetAssignedPrinter();
#elif NETSTANDARD
        Task<WSResponse<string>> GetAssignedPrinterAsync();
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriGetScannedPrinter, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<string> GetScannedPrinter(string printerID);
#elif NETSTANDARD
        Task<WSResponse<string>> GetScannedPrinterAsync(string printerID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = VBWebServiceConst.UriAssignPrinter, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<string> AssignPrinter(string printerID);
#elif NETSTANDARD
        Task<WSResponse<string>> AssignPrinterAsync(string printerID);
#endif

        #endregion
    }
}
