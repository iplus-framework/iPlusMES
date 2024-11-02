// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using System;
using System.Collections.Generic;
#if NETFRAMEWORK
using CoreWCF;
using CoreWCF.Web;
//using System.ServiceModel;
#elif NETSTANDARD
using System.Threading.Tasks;
#endif

namespace gip.mes.webservices
{
    partial interface IVBWebService
    {
        #region Material

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriMaterial, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<Material>> GetMaterials();
#elif NETSTANDARD
        Task<WSResponse<List<Material>>> GetMaterialsAsync();
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriMaterialID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<Material> GetMaterial(string materialID);
#elif NETSTANDARD
        Task<WSResponse<Material>> GetMaterialAsync(string materialID);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriMaterial_Search, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<Material>> SearchMaterial(string term);
#elif NETSTANDARD
        Task<WSResponse<List<Material>>> SearchMaterialAsync(string term);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriMaterial_BarcodeID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<Material> GetMaterialByBarcode(string barcodeID);
#elif NETSTANDARD
        Task<WSResponse<Material>> GetMaterialByBarcodeAsync(string barcodeID);
#endif

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriSuggestedMaterialsID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<Material>> GetSuggestedMaterials(string materialID);
#elif NETSTANDARD
        Task<WSResponse<List<Material>>> GetSuggestedMaterialsAsync(string materialID);
#endif

        #endregion


        #region Facility

#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacility, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<Facility>> GetFacilities();
#elif NETSTANDARD
        Task<WSResponse<List<Facility>>> GetFacilitiesAsync();
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacilityID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<Facility> GetFacility(string facilityID);
#elif NETSTANDARD
        Task<WSResponse<Facility>> GetFacilityAsync(string facilityID);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacility_Search, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<List<Facility>> SearchFacility(string term, string parentFacilityID, string type);
#elif NETSTANDARD
        Task<WSResponse<List<Facility>>> SearchFacilityAsync(string term, string parentFacilityID, string type);
#endif


#if NETFRAMEWORK
        [OperationContract]
        [WebGet(UriTemplate = VBWebServiceConst.UriFacility_BarcodeID, ResponseFormat = WebMessageFormat.Json)]
        WSResponse<Facility> GetFacilityByBarcode(string barcodeID);
#elif NETSTANDARD
        Task<WSResponse<Facility>> GetFacilityByBarcodeAsync(string barcodeID);
#endif

        #endregion
    }
}
