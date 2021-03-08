using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using iPlus = gip.core.datamodel;

namespace gip.mes.autocomponent
{
    public class VBACClassManager
    {

        public MsgWithDetails DeleteACClass(DatabaseApp databaseApp, iPlus.ACClass aCClass, bool withCheck)
        {
            MsgWithDetails msgWithDetails = null;

            /*
              ==========================================================================================================
            | TABLE_NAME                    | COLUMN_NAME                           | DATA_TYPE        | IS_NULLABLE |
            ==========================================================================================================
            | CompanyPersonRole             | VBiRoleACClassID                      | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | DemandOrderPos                | VBiProgramACClassMethodID             | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | Facility                      | VBiFacilityACClassID                  | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | Facility                      | VBiStackCalculatorACClassID           | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | FacilityBooking               | VBiStackCalculatorACClassID           | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | FacilityBookingCharge         | VBiStackCalculatorACClassID           | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | FacilityReservation           | VBiACClassID                          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | HistoryConfig                 | VBiACClassID                          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | HistoryConfig                 | VBiACClassPropertyRelationID          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | HistoryConfig                 | VBiACClassWFID                        | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | HistoryConfig                 | VBiValueTypeACClassID                 | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | InOrderConfig                 | VBiACClassID                          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | InOrderConfig                 | VBiACClassPropertyRelationID          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | InOrderConfig                 | VBiACClassWFID                        | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | InOrderConfig                 | VBiValueTypeACClassID                 | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | InRequestConfig               | VBiACClassID                          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | InRequestConfig               | VBiACClassPropertyRelationID          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | InRequestConfig               | VBiValueTypeACClassID                 | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | MaintOrder                    | VBiPAACClassID                        | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | MaintOrderProperty            | VBiACClassPropertyID                  | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | Material                      | VBiProgramACClassMethodID             | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | Material                      | VBiStackCalculatorACClassID           | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | MaterialConfig                | VBiACClassID                          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | MaterialConfig                | VBiACClassPropertyRelationID          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | MaterialConfig                | VBiACClassWFID                        | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | MaterialConfig                | VBiValueTypeACClassID                 | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | MaterialWFACClassMethod       | ACClassMethodID                       | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | MaterialWFACClassMethod       | MaterialWFACClassMethodID             | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | MaterialWFACClassMethodConfig | MaterialWFACClassMethodConfigID       | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | MaterialWFACClassMethodConfig | MaterialWFACClassMethodID             | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | MaterialWFACClassMethodConfig | ParentMaterialWFACClassMethodConfigID | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | MaterialWFACClassMethodConfig | VBiACClassID                          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | MaterialWFACClassMethodConfig | VBiACClassPropertyRelationID          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | MaterialWFACClassMethodConfig | VBiACClassWFID                        | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | MaterialWFACClassMethodConfig | VBiValueTypeACClassID                 | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | MaterialWFConnection          | ACClassWFID                           | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | MaterialWFConnection          | MaterialWFACClassMethodID             | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | OutOfferConfig             | VBiACClassID                          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | OutOfferConfig             | VBiACClassPropertyRelationID          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | OutOfferConfig             | VBiValueTypeACClassID                 | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | OutOrderConfig                | VBiACClassID                          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | OutOrderConfig                | VBiACClassPropertyRelationID          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | OutOrderConfig                | VBiACClassWFID                        | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | OutOrderConfig                | VBiValueTypeACClassID                 | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | PartslistConfig               | VBiACClassID                          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | PartslistConfig               | VBiACClassPropertyRelationID          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | PartslistConfig               | VBiACClassWFID                        | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | PartslistConfig               | VBiValueTypeACClassID                 | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | PartslistWF                   | VBiPWACClassID                        | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | PartslistWFEdge               | VBiSourceACClassPropertyID            | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | PartslistWFEdge               | VBiTargetACClassPropertyID            | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | ProdOrderBatchPlan            | VBiACClassWFID                        | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | ProdOrderPartslistConfig      | VBiACClassID                          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | ProdOrderPartslistConfig      | VBiACClassPropertyRelationID          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | ProdOrderPartslistConfig      | VBiACClassWFID                        | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | ProdOrderPartslistConfig      | VBiValueTypeACClassID                 | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | ProdOrderPartslistWF          | VBiPWACClassID                        | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | ProdOrderPartslistWFEdge      | VBiSourceACClassPropertyID            | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | ProdOrderPartslistWFEdge      | VBiTargetACClassPropertyID            | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | TourplanConfig                | VBiACClassID                          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | TourplanConfig                | VBiACClassPropertyRelationID          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | TourplanConfig                | VBiValueTypeACClassID                 | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | VBArchiveDocument             | ApplicationACClassConfigID            | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | VBConfig                      | ACClassID                             | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | VBConfig                      | ACClassPropertyRelationID             | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | VBConfig                      | ValueTypeACClassID                    | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | VBGroupRight                  | ACClassDesignID                       | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | VBGroupRight                  | ACClassID                             | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | VBGroupRight                  | ACClassMethodID                       | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | VBGroupRight                  | ACClassPropertyID                     | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | VBUser                        | MenuACClassDesignID                   | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------
            | VBUserACClassDesign           | ACClassDesignID                       | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | VBUserACClassDesign           | VBUserACClassDesignID                 | uniqueidentifier | NO          |
            ----------------------------------------------------------------------------------------------------------
            | Weighing                      | VBiACClassID                          | uniqueidentifier | YES         |
            ----------------------------------------------------------------------------------------------------------

            */

            DeleteACClassACRelated(databaseApp, aCClass, withCheck);

            //  CompanyPersonRole/VBiRoleACClassID  |NULLABLE 
            List<CompanyPersonRole> companyPersonRoles = databaseApp.CompanyPersonRole.Where(c => c.VBiRoleACClassID == aCClass.ACClassID).ToList();
            foreach (var companyPersonRole in companyPersonRoles)
                companyPersonRole.DeleteACObject(databaseApp, withCheck);

            //  Facility/VBiFacilityACClassID  |Null 
            List<Facility> facilities = databaseApp.Facility.Where(c => c.VBiFacilityACClassID == aCClass.ACClassID).ToList();
            foreach (Facility facility in facilities)
                facility.VBiFacilityACClassID = null;

            //  Facility/VBiStackCalculatorACClassID  |Null 
            List<Facility> calcClassFacilities = databaseApp.Facility.Where(c => (c.VBiStackCalculatorACClassID ?? Guid.Empty) == aCClass.ACClassID).ToList();
            foreach (Facility calcClassFacility in calcClassFacilities)
                calcClassFacility.VBiStackCalculatorACClassID = null;

            //  FacilityBooking/VBiStackCalculatorACClassID  |Null 
            List<FacilityBooking> calcFacilityBookings = databaseApp.FacilityBooking.Where(c => (c.VBiStackCalculatorACClassID ?? Guid.Empty) == aCClass.ACClassID).ToList();
            foreach (FacilityBooking calcFacilityBooking in calcFacilityBookings)
                calcFacilityBooking.VBiStackCalculatorACClassID = null;

            //  FacilityBookingCharge/VBiStackCalculatorACClassID  |Null 
            List<FacilityBookingCharge> stackCalculatorFacilityBookingCharges = databaseApp.FacilityBookingCharge.Where(c => (c.VBiStackCalculatorACClassID ?? Guid.Empty) == aCClass.ACClassID).ToList();
            foreach (FacilityBookingCharge stackCalculatorFacilityBookingCharge in stackCalculatorFacilityBookingCharges)
                stackCalculatorFacilityBookingCharge.VBiStackCalculatorACClassID = null;

            //  FacilityReservation/VBiACClassID  |Null 
            List<FacilityReservation> facilityReservations = databaseApp.FacilityReservation.Where(c => (c.VBiACClassID ?? Guid.Empty) == aCClass.ACClassID).ToList();
            foreach (FacilityReservation facilityReservation in facilityReservations)
                facilityReservation.VBiACClassID = null;

            //  HistoryConfig/VBiACClassID  |Null 
            List<HistoryConfig> historyConfigs = databaseApp.HistoryConfig.Where(c => (c.VBiACClassID ?? Guid.Empty) == aCClass.ACClassID).ToList();
            foreach (HistoryConfig historyConfig in historyConfigs)
                historyConfig.VBiACClassID = null;

            //  HistoryConfig/VBiValueTypeACClassID  |NotNull 
            List<HistoryConfig> valueTypeClHistoryConfigs = databaseApp.HistoryConfig.Where(c => c.VBiValueTypeACClassID == aCClass.ACClassID).ToList();
            foreach (HistoryConfig valueTypeClHistoryConfig in valueTypeClHistoryConfigs)
                valueTypeClHistoryConfig.DeleteACObject(databaseApp, withCheck);


            //  InOrderConfig/VBiACClassID  |Null 
            List<InOrderConfig> inOrderConfigs = databaseApp.InOrderConfig.Where(c => c.VBiValueTypeACClassID == aCClass.ACClassID).ToList();
            foreach (InOrderConfig inOrderConfig in inOrderConfigs)
                inOrderConfig.VBiACClassID = null;


            //  InOrderConfig/VBiValueTypeACClassID  |NotNull 
            List<InOrderConfig> inOrderValueTypes = databaseApp.InOrderConfig.Where(c => c.VBiValueTypeACClassID == aCClass.ACClassID).ToList();
            foreach (InOrderConfig inOrderValueType in inOrderValueTypes)
                inOrderValueType.DeleteACObject(databaseApp, withCheck);

            //  InRequestConfig/VBiACClassID  |Null 
            List<InRequestConfig> inRequestConfigs = databaseApp.InRequestConfig.Where(c => c.VBiValueTypeACClassID == aCClass.ACClassID).ToList();
            foreach (InRequestConfig inRequestConfig in inRequestConfigs)
                inRequestConfig.VBiACClassID = null;

            //  InRequestConfig/VBiValueTypeACClassID  |NotNull 
            List<InRequestConfig> inRequestValueTypeConfigs = databaseApp.InRequestConfig.Where(c => c.VBiValueTypeACClassID == aCClass.ACClassID).ToList();
            foreach (InRequestConfig inRequestConfig in inRequestValueTypeConfigs)
                inRequestConfig.DeleteACObject(databaseApp, withCheck);

            //  MaintOrder/VBiPAACClassID  |NotNull 
            List<MaintOrder> maintOrders = databaseApp.MaintOrder.Where(c => c.VBiPAACClassID == aCClass.ACClassID).ToList();
            List<MaintTask> maintTasks = maintOrders.SelectMany(c => c.MaintTask_MaintOrder).ToList();
            foreach (MaintTask maintTask in maintTasks)
                maintTask.DeleteACObject(databaseApp, withCheck);
            foreach (MaintOrder maintOrder in maintOrders)
                maintOrder.DeleteACObject(databaseApp, withCheck);



            //  Material/VBiStackCalculatorACClassID  |Null 
            List<Material> vbiStackCalculatorMaterials = databaseApp.Material.Where(c => c.VBiStackCalculatorACClassID == aCClass.ACClassID).ToList();
            foreach (Material vbiStackCalculatorMaterial in vbiStackCalculatorMaterials)
                vbiStackCalculatorMaterial.VBiStackCalculatorACClassID = null;

            //  MaterialConfig/VBiACClassID  |Null 
            List<MaterialConfig> materialConfigs = databaseApp.MaterialConfig.Where(c => c.VBiACClassID == aCClass.ACClassID).ToList();
            foreach (MaterialConfig materialConfig in materialConfigs)
                materialConfig.VBiACClassID = null;

            //  MaterialConfig/VBiValueTypeACClassID  |NotNull 
            List<MaterialConfig> valueTypeMaterialConfigs = databaseApp.MaterialConfig.Where(c => c.VBiACClassID == aCClass.ACClassID).ToList();
            foreach (MaterialConfig valueTypeMaterialConfig in valueTypeMaterialConfigs)
                valueTypeMaterialConfig.DeleteACObject(databaseApp, withCheck);


            //  MaterialWFACClassMethodConfig/VBiACClassID  |Null 
            List<MaterialWFACClassMethodConfig> materialWFACClassMethodConfigs = databaseApp.MaterialWFACClassMethodConfig.Where(c => (c.VBiACClassID ?? Guid.Empty) == aCClass.ACClassID).ToList();
            foreach (MaterialWFACClassMethodConfig materialWFACClassMethodConfig in materialWFACClassMethodConfigs)
                materialWFACClassMethodConfig.VBiACClassID = null;

            //  MaterialWFACClassMethodConfig/VBiValueTypeACClassID  |NotNull 
            List<MaterialWFACClassMethodConfig> typeConfigs = databaseApp.MaterialWFACClassMethodConfig.Where(c => c.VBiValueTypeACClassID == aCClass.ACClassID).ToList();
            foreach (MaterialWFACClassMethodConfig typeConfig in typeConfigs)
                typeConfig.DeleteACObject(databaseApp, withCheck);

            //  OutOfferConfig/VBiACClassID  |Null 
            List<OutOfferConfig> OutOfferConfigs = databaseApp.OutOfferConfig.Where(c => (c.VBiACClassID ?? Guid.Empty) == aCClass.ACClassID).ToList();
            foreach (OutOfferConfig OutOfferConfig in OutOfferConfigs)
                OutOfferConfig.VBiACClassID = null;

            //  OutOfferConfig/VBiValueTypeACClassID  |NotNull 
            List<OutOfferConfig> valueTypeCls = databaseApp.OutOfferConfig.Where(c => c.VBiValueTypeACClassID == aCClass.ACClassID).ToList();
            foreach (OutOfferConfig valueTypeCl in valueTypeCls)
                valueTypeCl.DeleteACObject(databaseApp, withCheck);

            //  OutOrderConfig/VBiACClassID  |Null 
            List<OutOrderConfig> outOrderConfigs = databaseApp.OutOrderConfig.Where(c => (c.VBiACClassID ?? Guid.Empty) == aCClass.ACClassID).ToList();
            foreach (OutOrderConfig outOrderConfig in outOrderConfigs)
                outOrderConfig.VBiACClassID = null;

            //  OutOrderConfig/VBiValueTypeACClassID  |NotNull 
            List<OutOrderConfig> valueTypeOrderConfigs = databaseApp.OutOrderConfig.Where(c => c.VBiValueTypeACClassID == aCClass.ACClassID).ToList();
            foreach (OutOrderConfig valueTypeOrderConfig in valueTypeOrderConfigs)
                valueTypeOrderConfig.DeleteACObject(databaseApp, withCheck);

            //  PartslistConfig/VBiACClassID  |Null 
            List<PartslistConfig> partslistConfigs = databaseApp.PartslistConfig.Where(c => (c.VBiACClassID ?? Guid.Empty) == aCClass.ACClassID).ToList();
            foreach (PartslistConfig partslistConfig in partslistConfigs)
                partslistConfig.VBiACClassID = null;

            //  PartslistConfig/VBiValueTypeACClassID  |NotNull 
            List<PartslistConfig> valuePartslistConfigs = databaseApp.PartslistConfig.Where(c => c.VBiValueTypeACClassID == aCClass.ACClassID).ToList();
            foreach (PartslistConfig valuePartslistConfig in valuePartslistConfigs)
                valuePartslistConfig.DeleteACObject(databaseApp, withCheck);

            //  ProdOrderPartslistConfig/VBiACClassID  |Null 
            List<ProdOrderPartslistConfig> prodOrderPartslistConfigs = databaseApp.ProdOrderPartslistConfig.Where(c => (c.VBiACClassID ?? Guid.Empty) == aCClass.ACClassID).ToList();
            foreach (ProdOrderPartslistConfig prodOrderPartslistConfig in prodOrderPartslistConfigs)
                prodOrderPartslistConfig.VBiACClassID = null;

            //  ProdOrderPartslistConfig/VBiValueTypeACClassID  |NotNull 
            List<ProdOrderPartslistConfig> vbiValuePartslistConfigs = databaseApp.ProdOrderPartslistConfig.Where(c => c.VBiValueTypeACClassID == aCClass.ACClassID).ToList();
            foreach (ProdOrderPartslistConfig vbiValuePartslistConfig in vbiValuePartslistConfigs)
                vbiValuePartslistConfig.DeleteACObject(databaseApp, withCheck);

            //  TourplanConfig/VBiACClassID  |Null 
            List<TourplanConfig> tourplanConfigs = databaseApp.TourplanConfig.Where(c => (c.VBiACClassID ?? Guid.Empty) == aCClass.ACClassID).ToList();
            foreach (TourplanConfig tourplanConfig in tourplanConfigs)
                tourplanConfig.VBiACClassID = null;

            //  TourplanConfig/VBiValueTypeACClassID  |NotNull 
            List<TourplanConfig> valueTypeTourplanConfigs = databaseApp.TourplanConfig.Where(c => c.VBiValueTypeACClassID == aCClass.ACClassID).ToList();
            foreach (TourplanConfig valueTypeTourplanConfig in valueTypeTourplanConfigs)
                valueTypeTourplanConfig.DeleteACObject(databaseApp, withCheck);

            //  Weighing/VBiACClassID  |Null 
            List<Weighing> weighings = databaseApp.Weighing.Where(c => (c.VBiACClassID ?? Guid.Empty) == aCClass.ACClassID).ToList();
            foreach (Weighing weighing in weighings)
                weighing.VBiACClassID = null;

            #region Not Solved
            //  HistoryConfig/VBiACClassWFID  |Null 
            //  InOrderConfig/VBiACClassWFID  |Null
            //  MaterialConfig/VBiACClassWFID  |Null 
            //  MaterialWFACClassMethodConfig/VBiACClassWFID  |Null 
            //  MaterialWFConnection/ACClassWFID  |NotNull 
            //  MaterialWFConnection/MaterialWFACClassMethodID  |NotNull 
            //  OutOrderConfig/VBiACClassWFID  |Null 
            //  PartslistConfig/VBiACClassWFID  |Null 
            //  PartslistWF/VBiPWACClassID  |NotNull 
            //  ProdOrderBatchPlan/VBiACClassWFID  |Null
            //  ProdOrderPartslistConfig/VBiACClassWFID  |Null 
            //  ProdOrderPartslistWF/VBiPWACClassID  |NotNull 
            //  VBArchiveDocument/ApplicationACClassConfigID  |NotNull 


            // iplus?
            //  VBConfig/ACClassPropertyRelationID  |Null 
            //  VBConfig/ValueTypeACClassID  |NotNull 
            //  VBGroupRight/ACClassDesignID  |Null 
            //  VBGroupRight/ACClassID  |NotNull 
            //  VBGroupRight/ACClassMethodID  |Null 
            //  VBGroupRight/ACClassPropertyID  |Null 
            //  VBUser/MenuACClassDesignID  |Null 
            //  VBUserACClassDesign/ACClassDesignID  |NotNull 
            //  VBUserACClassDesign/VBUserACClassDesignID  |NotNull 
            #endregion

            return msgWithDetails;
        }

        private MsgWithDetails DeleteACClassACRelated(DatabaseApp databaseApp, iPlus.ACClass aCClass, bool withCheck)
        {
            MsgWithDetails msgWithDetails = null;
            List<iPlus.ACClassMethod> classMethods = aCClass.ACClassMethod_ACClass.ToList();
            foreach (iPlus.ACClassMethod aCClassMethod in classMethods)
                DeleteACClassMethod(databaseApp, aCClassMethod, withCheck);

            List<iPlus.ACClassProperty> classProperties = aCClass.Properties.ToList();
            foreach (iPlus.ACClassProperty acClassProperty in classProperties)
                DeleteACClassProperty(databaseApp, acClassProperty, withCheck);

            return msgWithDetails;
        }

        public MsgWithDetails DeleteACClassProperty(DatabaseApp databaseApp, iPlus.ACClassProperty acClassProperty, bool withCheck)
        {
            MsgWithDetails msgWithDetails = null;

            Guid[] sourceRelations = acClassProperty.ACClassPropertyRelation_SourceACClassProperty.Select(c => c.ACClassPropertyRelationID).ToArray();
            Guid[] targetRelations = acClassProperty.ACClassPropertyRelation_SourceACClassProperty.Select(c => c.ACClassPropertyRelationID).ToArray();
            Guid[] relations = sourceRelations.Union(targetRelations).Distinct().ToArray();

            //  HistoryConfig/VBiACClassPropertyRelationID  |Null 
            List<HistoryConfig> historyConfigs = databaseApp.HistoryConfig.Where(c => relations.Contains(c.VBiACClassPropertyRelationID ?? Guid.Empty)).ToList();
            foreach (HistoryConfig historyConfig in historyConfigs)
                historyConfig.VBiACClassPropertyRelationID = null;

            //  InOrderConfig/VBiACClassPropertyRelationID  |Null 
            List<InOrderConfig> inOrderConfigs = databaseApp.InOrderConfig.Where(c => relations.Contains(c.VBiACClassPropertyRelationID ?? Guid.Empty)).ToList();
            foreach (InOrderConfig inOrderConfig in inOrderConfigs)
                inOrderConfig.VBiACClassPropertyRelationID = null;

            //  InRequestConfig/VBiACClassPropertyRelationID  |Null 
            List<InRequestConfig> inRequestConfigs = databaseApp.InRequestConfig.Where(c => relations.Contains(c.VBiACClassPropertyRelationID ?? Guid.Empty)).ToList();
            foreach (InRequestConfig inRequestCOnfig in inRequestConfigs)
                inRequestCOnfig.VBiACClassPropertyRelationID = null;

            //  MaintOrderProperty/VBiACClassPropertyID  |NotNull 
            //List<MaintOrderProperty> maintOrderProperties = databaseApp.MaintOrderProperty.Where(c => c.vbiacclas == acClassProperty.ACClassPropertyID).ToList();
            //foreach (MaintOrder maintOrder in maintOrders)
            //    maintOrder.DeleteACObject(databaseApp, withCheck);

            //  MaterialConfig/VBiACClassPropertyRelationID  |Null 
            List<MaterialConfig> materialConfigs = databaseApp.MaterialConfig.Where(c => relations.Contains(c.VBiACClassPropertyRelationID ?? Guid.Empty)).ToList();
            foreach (MaterialConfig materialConfig in materialConfigs)
                materialConfig.VBiACClassPropertyRelationID = null;

            //  MaterialWFACClassMethodConfig/VBiACClassPropertyRelationID  |Null 
            List<MaterialWFACClassMethodConfig> materialWFACClassMethodConfigs = databaseApp.MaterialWFACClassMethodConfig.Where(c => relations.Contains(c.VBiACClassPropertyRelationID ?? Guid.Empty)).ToList();
            foreach (MaterialWFACClassMethodConfig materialWFACClassMethodConfig in materialWFACClassMethodConfigs)
                materialWFACClassMethodConfig.VBiACClassPropertyRelationID = null;

            //  OutOfferConfig/VBiACClassPropertyRelationID  |Null 
            List<OutOfferConfig> OutOfferConfigs = databaseApp.OutOfferConfig.Where(c => relations.Contains(c.VBiACClassPropertyRelationID ?? Guid.Empty)).ToList();
            foreach (OutOfferConfig OutOfferConfig in OutOfferConfigs)
                OutOfferConfig.VBiACClassPropertyRelationID = null;

            //  OutOrderConfig/VBiACClassPropertyRelationID  |Null 
            List<OutOrderConfig> outOrderConfigs = databaseApp.OutOrderConfig.Where(c => relations.Contains(c.VBiACClassPropertyRelationID ?? Guid.Empty)).ToList();
            foreach (OutOrderConfig outOrderConfig in outOrderConfigs)
                outOrderConfig.VBiACClassID = null;

            //  PartslistConfig/VBiACClassPropertyRelationID  |Null 
            List<PartslistConfig> partslistConfigs = databaseApp.PartslistConfig.Where(c => relations.Contains(c.VBiACClassPropertyRelationID ?? Guid.Empty)).ToList();
            foreach (PartslistConfig partslistConfig in partslistConfigs)
                partslistConfig.DeleteACObject(databaseApp, withCheck);

            //  PartslistWFEdge/VBiSourceACClassPropertyID  |NotNull 
            //List<PartslistWFEdge> sourceEdges = databaseApp.edge
            //  PartslistWFEdge/VBiTargetACClassPropertyID  |NotNull 

            //  ProdOrderPartslistConfig/VBiACClassPropertyRelationID  |Null 
            //  PartslistConfig/VBiACClassPropertyRelationID  |Null 
            List<ProdOrderPartslistConfig> prodOrderPartslistConfigs = databaseApp.ProdOrderPartslistConfig.Where(c => relations.Contains(c.VBiACClassPropertyRelationID ?? Guid.Empty)).ToList();
            foreach (ProdOrderPartslistConfig prodOrderPartslistConfig in prodOrderPartslistConfigs)
                prodOrderPartslistConfig.DeleteACObject(databaseApp, withCheck);

            //  ProdOrderPartslistWFEdge/VBiSourceACClassPropertyID  |NotNull 
            //  ProdOrderPartslistWFEdge/VBiTargetACClassPropertyID  |NotNull 

            //  TourplanConfig/VBiACClassPropertyRelationID  |Null 
            List<TourplanConfig> tourplanConfigs = databaseApp.TourplanConfig.Where(c => relations.Contains(c.VBiACClassPropertyRelationID ?? Guid.Empty)).ToList();
            foreach (TourplanConfig tourplanConfig in tourplanConfigs)
                tourplanConfig.DeleteACObject(databaseApp, withCheck);

            return msgWithDetails;
        }

        public MsgWithDetails DeleteACClassMethod(DatabaseApp databaseApp, iPlus.ACClassMethod aCClassMethod, bool withCheck)
        {
            MsgWithDetails msgWithDetails = null;
            //  DemandOrderPos/VBiProgramACClassMethodID  |NotNull 
            List<DemandOrderPos> demandOrderPoses = databaseApp.DemandOrderPos.Where(c => c.VBiProgramACClassMethodID == aCClassMethod.ACClassMethodID).ToList();
            foreach (DemandOrderPos demandOrderPos in demandOrderPoses)
                demandOrderPos.DeleteACObject(databaseApp, withCheck);

            //  Material/VBiProgramACClassMethodID  |Null 
            List<Material> materials = databaseApp.Material.Where(c => (c.VBiProgramACClassMethodID ?? Guid.Empty) == aCClassMethod.ACClassMethodID).ToList();
            foreach (Material material in materials)
                material.VBiProgramACClassMethodID = null;

            //  MaterialWFACClassMethod/ACClassMethodID  |NotNull 
            List<MaterialWFACClassMethod> materialWFACClassMethods = databaseApp.MaterialWFACClassMethod.Where(c => c.ACClassMethodID == aCClassMethod.ACClassMethodID).ToList();
            foreach (MaterialWFACClassMethod materialWFACClassMethod in materialWFACClassMethods)
                materialWFACClassMethod.DeleteACObject(databaseApp, withCheck);

            //  MaterialWFACClassMethod/MaterialWFACClassMethodID  |NotNull 
            //  MaterialWFACClassMethodConfig/MaterialWFACClassMethodConfigID  |NotNull 
            //  MaterialWFACClassMethodConfig/MaterialWFACClassMethodID  |Null 
            //  MaterialWFACClassMethodConfig/ParentMaterialWFACClassMethodConfigID  |Null 

            return msgWithDetails;
        }
    }
}
