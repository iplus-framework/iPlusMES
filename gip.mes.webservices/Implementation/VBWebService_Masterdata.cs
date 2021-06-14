using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.webservices;

namespace gip.mes.webservices
{
    public partial class VBWebService
    {
        #region Material

        public static readonly Func<DatabaseApp, Guid?, string, IQueryable<Material>> s_cQry_GetMaterial =
        CompiledQuery.Compile<DatabaseApp, Guid?, string, IQueryable<Material>>(
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
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<List<Material>>(s_cQry_GetMaterial(dbApp, null, null).ToList());
                }
                catch (Exception e)
                {
                    PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetMaterials()", e);
                    return new WSResponse<List<Material>>(null, new Msg(eMsgLevel.Exception, e.Message));
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

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<Material>(s_cQry_GetMaterial(dbApp, guid, null).FirstOrDefault());
                }
                catch (Exception e)
                {
                    PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetMaterial(10)", e);
                    return new WSResponse<Material>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
            }
        }


        public WSResponse<List<Material>> SearchMaterial(string term)
        {
            if (string.IsNullOrEmpty(term) || term == CoreWebServiceConst.EmptyParam)
                term = null;

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<List<Material>>(s_cQry_GetMaterial(dbApp, null, term).ToList());
                }
                catch (Exception e)
                {
                    PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "SearchMaterial(10)", e);
                    return new WSResponse<List<Material>>(null, new Msg(eMsgLevel.Exception, e.Message));
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
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetMaterialByBarcode(10)", e);
                    return new WSResponse<Material>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
            }
        }

        #endregion


        #region Facility

        public static readonly Func<DatabaseApp, Guid?, string, Guid?, short?, IQueryable<Facility>> s_cQry_GetFacility =
        CompiledQuery.Compile<DatabaseApp, Guid?, string, Guid?, short?, IQueryable<Facility>>(
            (dbApp, facilityID, term, parentFacilityID, facilityTypeIndex) =>
                dbApp.Facility
                .Where(c => (!facilityID.HasValue || c.FacilityID == facilityID)
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
                    ParentFacilityID = c.ParentFacilityID
                }
             )
        );


        public WSResponse<List<Facility>> GetFacilities()
        {
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<List<Facility>>(s_cQry_GetFacility(dbApp, null, null, null, null).ToList());
                }
                catch (Exception e)
                {
                    PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilities()", e);
                    return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Exception, e.Message));
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
                    PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacility(10)", e);
                    return new WSResponse<Facility>(null, new Msg(eMsgLevel.Exception, e.Message));
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
                Type typeofFacType = typeof(MDFacilityType.FacilityTypes);
                if (!Enum.IsDefined(typeofFacType, tmpIndex))
                    return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Error, "type is invalid"));
                facilityTypeIndex = tmpIndex;
            }

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    return new WSResponse<List<Facility>>(s_cQry_GetFacility(dbApp, null, term, guid, facilityTypeIndex).ToList());
                }
                catch (Exception e)
                {
                    PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>();
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "SearchFacility(10)", e);
                    return new WSResponse<List<Facility>>(null, new Msg(eMsgLevel.Exception, e.Message));
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

            using (DatabaseApp dbApp = new DatabaseApp())
            {
                try
                {
                    Guid guid = facManager.ResolveFacilityIdFromBarcode(dbApp, barcodeID);
                    if (guid == Guid.Empty)
                        return new WSResponse<Facility>(null, new Msg(eMsgLevel.Error, "Coudn't resolve barcodeID"));
                    Facility facility = s_cQry_GetFacility(dbApp, guid, null, null, null).FirstOrDefault();
                    if (facility.ParentFacilityID != null)
                        facility.ParentFacility = s_cQry_GetFacility(dbApp, facility.ParentFacilityID, null, null, null).FirstOrDefault();
                    return new WSResponse<Facility>(facility);
                }
                catch (Exception e)
                {
                    if (myServiceHost != null)
                        myServiceHost.Messages.LogException(myServiceHost.GetACUrl(), "GetFacilityByBarcode(10)", e);
                    return new WSResponse<Facility>(null, new Msg(eMsgLevel.Exception, e.Message));
                }
            }
        }

        #endregion
    }
}
