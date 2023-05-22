using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.webservices;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.webservices
{
    public partial class VBWebService
    {
        #region Material

        public static readonly Func<DatabaseApp, Guid?, string, IEnumerable<Material>> s_cQry_GetMaterial =
        EF.CompileQuery<DatabaseApp, Guid?, string, IEnumerable<Material>>(
            (dbApp, materialID, term) =>
                dbApp.Material
                .Where(c => (!materialID.HasValue || c.MaterialID == materialID)
                            && (term == null || c.MaterialNo.Contains(term) || c.MaterialName1.Contains(term)))
                .Select(c => new gip.mes.webservices.Material()
                {
                    MaterialID = c.MaterialID,
                    MaterialNo = c.MaterialNo,
                    MaterialName1 = c.MaterialName1,
                    OptStockQuantity = c.OptStockQuantity,
                    MinStockQuantity = c.MinStockQuantity,
                    BaseMDUnit = new gip.mes.webservices.MDUnit()
                    {
                        MDUnitID = c.BaseMDUnit.MDUnitID,
                        MDUnitNameTrans = c.BaseMDUnit.MDUnitNameTrans
                    }
                }
             )
        );


        public WSResponse<List<Material>> GetMaterials()
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<List<Material>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetMaterials));
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<List<Material>>(s_cQry_GetMaterial(dbApp, null, null).Take(myServiceHost.Root.Environment.AccessDefaultTakeCount).ToList());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetMaterials) + "(10)", e);
                    return new WSResponse<List<Material>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetMaterials));
                }
            }
        }


        public WSResponse<Material> GetMaterial(string materialID)
        {
            if (string.IsNullOrEmpty(materialID))
                return new WSResponse<Material>(null, new Msg(eMsgLevel.Error, "materialID is empty"));

            Guid guid;
            if (!Guid.TryParse(materialID, out guid))
                return new WSResponse<Material>(null, new Msg(eMsgLevel.Error, "materialID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<Material>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetMaterial));
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<Material>(s_cQry_GetMaterial(dbApp, guid, null).FirstOrDefault());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetMaterial) + "(10)", e);
                    return new WSResponse<Material>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetMaterial));
                }
            }
        }

        public WSResponse<List<Material>> GetSuggestedMaterials(string materialID)
        {
            if (string.IsNullOrEmpty(materialID))
                return new WSResponse<List<Material>>(null, new Msg(eMsgLevel.Error, "materialID is empty"));

            Guid guid;
            if (!Guid.TryParse(materialID, out guid))
                return new WSResponse<List<Material>>(null, new Msg(eMsgLevel.Error, "materialID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<List<Material>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetSuggestedMaterials));

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    FacilityManager facilityManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
                    if (facilityManager == null)
                        return new WSResponse<List<Material>>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

                    datamodel.Material material = dbApp.Material.FirstOrDefault(c => c.MaterialID == guid);
                    IEnumerable<datamodel.Material> suggestedMaterials = facilityManager.GetSuggestedReassignmentMaterials(dbApp, material);

                    if (suggestedMaterials == null || !suggestedMaterials.Any())
                        return new WSResponse<List<Material>>(new List<Material>());

                    return new WSResponse<List<Material>>(suggestedMaterials.Select(c => new gip.mes.webservices.Material()
                                                                                         {
                                                                                            MaterialID = c.MaterialID,
                                                                                            MaterialNo = c.MaterialNo,
                                                                                            MaterialName1 = c.MaterialName1,
                                                                                            OptStockQuantity = c.OptStockQuantity,
                                                                                            MinStockQuantity = c.MinStockQuantity,
                                                                                            BaseMDUnit = new gip.mes.webservices.MDUnit()
                                                                                            {
                                                                                                MDUnitID = c.BaseMDUnit.MDUnitID,
                                                                                                MDUnitNameTrans = c.BaseMDUnit.MDUnitNameTrans
                                                                                            }
                                                                                         }).ToList());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetSuggestedMaterials) + "(10)", e);
                    return new WSResponse<List<Material>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetSuggestedMaterials));
                }
            }
        }


        public WSResponse<List<Material>> SearchMaterial(string term)
        {
            if (string.IsNullOrEmpty(term) || term == CoreWebServiceConst.EmptyParam)
                term = null;

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<List<Material>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(SearchMaterial));
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<List<Material>>(s_cQry_GetMaterial(dbApp, null, term).Take(myServiceHost.Root.Environment.AccessDefaultTakeCount).ToList());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(SearchMaterial) + "(10)", e);
                    return new WSResponse<List<Material>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(SearchMaterial));
                }
            }
        }


        public WSResponse<Material> GetMaterialByBarcode(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return new WSResponse<Material>(null, new Msg(eMsgLevel.Error, "barcodeID is empty"));
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<Material>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<Material>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetMaterialByBarcode));
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    Guid guid = facManager.ResolveMaterialIdFromBarcode(dbApp, barcodeID);
                    if (guid == Guid.Empty)
                        return new WSResponse<Material>(null, new Msg(eMsgLevel.Error, "Coudn't resolve barcodeID"));
                    return new WSResponse<Material>(s_cQry_GetMaterial(dbApp, guid, null).FirstOrDefault());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetMaterialByBarcode) + "(10)", e);
                    return new WSResponse<Material>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetMaterialByBarcode));
                }
            }
        }

        #endregion


        #region Facility

        public static readonly Func<DatabaseApp, Guid?, string, Guid?, short?, IEnumerable<Facility>> s_cQry_GetFacility =
        EF.CompileQuery<DatabaseApp, Guid?, string, Guid?, short?, IEnumerable<Facility>>(
            (dbApp, facilityID, term, parentFacilityID, facilityTypeIndex) =>
                dbApp.Facility
                .Where(c => (!facilityID.HasValue || c.FacilityID == facilityID)
                            && (!c.DisabledForMobile)
                            && (term == null || c.FacilityNo.Contains(term) || c.FacilityName.Contains(term))
                            && (!parentFacilityID.HasValue || c.ParentFacilityID == parentFacilityID)
                            && (!facilityTypeIndex.HasValue || c.MDFacilityType.MDFacilityTypeIndex == facilityTypeIndex))
                .Select(c => new gip.mes.webservices.Facility()
                {
                    FacilityID = c.FacilityID,
                    FacilityNo = c.FacilityNo,
                    FacilityName = c.FacilityName,
                    MDFacilityType = new MDFacilityType()
                    {
                        MDFacilityTypeID = c.MDFacilityTypeID,
                        MDNameTrans = c.MDFacilityType.MDNameTrans,
                        MDFacilityTypeIndex = c.MDFacilityType.MDFacilityTypeIndex
                    },
                    ParentFacilityID = c.ParentFacilityID,
                    SkipPrintQuestion = c.SkipPrintQuestion
                }
             )
        );


        public WSResponse<List<Facility>> GetFacilities()
        {
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilities));
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<List<Facility>>(s_cQry_GetFacility(dbApp, null, null, null, null).Take(myServiceHost.Root.Environment.AccessDefaultTakeCount).ToList());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetFacilities) + "(10)", e);
                    return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilities));
                }
            }
        }

        public WSResponse<Facility> GetFacility(string facilityID)
        {
            if (string.IsNullOrEmpty(facilityID))
                return new WSResponse<Facility>(null, new Msg(eMsgLevel.Error, "facilityID is empty"));

            Guid guid;
            if (!Guid.TryParse(facilityID, out guid))
                return new WSResponse<Facility>(null, new Msg(eMsgLevel.Error, "facilityID is invalid"));

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<Facility>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacility));
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    Facility facility = s_cQry_GetFacility(dbApp, guid, null, null, null).FirstOrDefault();
                    if (facility.ParentFacilityID != null)
                        facility.ParentFacility = s_cQry_GetFacility(dbApp, facility.ParentFacilityID, null, null, null).FirstOrDefault();
                    return new WSResponse<Facility>(facility);
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetFacility) + "(10)", e);
                    return new WSResponse<Facility>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacility));
                }
            }
        }

        public WSResponse<List<Facility>> SearchFacility(string term, string parentFacilityID, string type)
        {
            if (string.IsNullOrEmpty(term) || term == CoreWebServiceConst.EmptyParam)
                term = null;

            Guid? guid = null;
            if (!string.IsNullOrEmpty(parentFacilityID) && parentFacilityID != CoreWebServiceConst.EmptyParam)
            {
                Guid tmpGuid;
                if (!Guid.TryParse(parentFacilityID, out tmpGuid))
                    return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Error, "parentFacilityID is invalid"));
                guid = tmpGuid;
            }

            short? facilityTypeIndex = null;
            if (!string.IsNullOrEmpty(type) && type != CoreWebServiceConst.EmptyParam)
            {
                short tmpIndex = 0;
                if (!short.TryParse(type, out tmpIndex))
                    return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Error, "type is invalid"));
                Type typeofFacType = typeof(FacilityTypesEnum);
                if (!Enum.IsDefined(typeofFacType, tmpIndex))
                    return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Error, "type is invalid"));
                facilityTypeIndex = tmpIndex;
            }

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));
            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(SearchFacility));
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<List<Facility>>(s_cQry_GetFacility(dbApp, null, term, guid, facilityTypeIndex).Take(myServiceHost.Root.Environment.AccessDefaultTakeCount).ToList());
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(SearchFacility) + "(10)", e);
                    return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(SearchFacility));
                }
            }
        }

        public WSResponse<Facility> GetFacilityByBarcode(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return new WSResponse<Facility>(null, new Msg(eMsgLevel.Error, "barcodeID is empty"));
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
            if (myServiceHost == null)
                return new WSResponse<Facility>(null, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            FacilityManager facManager = HelperIFacilityManager.GetServiceInstance(myServiceHost) as FacilityManager;
            if (facManager == null)
                return new WSResponse<Facility>(null, new Msg(eMsgLevel.Error, "FacilityManager not found"));

            PerformanceEvent perfEvent = myServiceHost.OnMethodCalled(nameof(GetFacilityByBarcode));
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    Guid guid = facManager.ResolveFacilityIdFromBarcode(dbApp, barcodeID);
                    if (guid == Guid.Empty)
                        return new WSResponse<Facility>(null, new Msg(eMsgLevel.Error, "Coudn't resolve barcodeID"));
                    Facility facility = s_cQry_GetFacility(dbApp, guid, null, null, null).FirstOrDefault();
                    if (facility != null && facility.ParentFacilityID != null)
                        facility.ParentFacility = s_cQry_GetFacility(dbApp, facility.ParentFacilityID, null, null, null).FirstOrDefault();
                    return new WSResponse<Facility>(facility);
                }
                catch (Exception e)
                {
                    myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), nameof(GetFacilityByBarcode) + "(10)", e);
                    return new WSResponse<Facility>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
                finally
                {
                    myServiceHost.OnMethodReturned(perfEvent, nameof(GetFacilityByBarcode));
                }
            }
        }

        #endregion
    }
}
