using gip.core.media;
using VD = gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.core.autocomponent;

namespace gip.mes.facility
{
    public partial class FacilityManager
    {

        #region const

        public double Const_RangeStockQuantityTolerance = 0.1;
        public const string ProdMatStorage = @"ProdMatStorage";

        #endregion

        #region Material preparation methods

        public MaterialPreparationResult GetMaterialPreparationModel(Database database, VD.DatabaseApp databaseApp,
            ACMediaController mediaController, ConfigManagerIPlus configManager, ACRoutingParameters routingParameters,
            List<VD.ProdOrderBatchPlan> selectedBatchPlans)
        {
            MaterialPreparationResult materialPreparationResult = new MaterialPreparationResult();
            if (selectedBatchPlans.Any())
            {
                GetSearchBatchMaterialModels(database, materialPreparationResult, selectedBatchPlans);

                FetchConfigurationForAllowedInstances(database, configManager, materialPreparationResult);
                FetchAllowedInstances(database, materialPreparationResult);
                FetchFacilityInstances(database, databaseApp, routingParameters, materialPreparationResult);
                DistributeAllowedFacilities(materialPreparationResult);


                SetFacilitiesOnRouteIds(materialPreparationResult);

                materialPreparationResult.PreparedMaterials = GetPreparedMaterials(databaseApp, mediaController, materialPreparationResult.MaterialPreparationBatchModels);
            }

            return materialPreparationResult;
        }


        #endregion


        #region MaterialPreparationModel

        public void GetSearchBatchMaterialModels(Database database, MaterialPreparationResult preparationModel, List<VD.ProdOrderBatchPlan> batchPlans)
        {
            foreach (var batchPlan in batchPlans)
            {
                GetPositionsForBatchMaterialModel(database, preparationModel, batchPlan, batchPlan.ProdOrderPartslistPos, batchPlan.ProdOrderPartslistPos.TargetQuantityUOM);
            }
        }

        private void GetPositionsForBatchMaterialModel(Database database, MaterialPreparationResult preparationModel, VD.ProdOrderBatchPlan batchPlan, VD.ProdOrderPartslistPos prodOrderPartslistPos, double posTargetQuantityUOM)
        {
            foreach (VD.ProdOrderPartslistPosRelation prodOrderPartslistPosRelation in prodOrderPartslistPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
            {
                if (prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.MaterialPosType == VD.GlobalApp.MaterialPosTypes.OutwardRoot)
                {
                    MaterialPreparationBatchModel searchBatchMaterialModel = GetRelationForBatchMaterialModel(batchPlan, prodOrderPartslistPosRelation, posTargetQuantityUOM);
                    preparationModel.MaterialPreparationBatchModels.Add(searchBatchMaterialModel);
                    BuildAllowedInstances(database, preparationModel, batchPlan, prodOrderPartslistPosRelation);
                }
                else
                {
                    double factor = prodOrderPartslistPosRelation.TargetQuantityUOM / posTargetQuantityUOM;
                    double subPosTargetQuantity = posTargetQuantityUOM * factor;
                    GetPositionsForBatchMaterialModel(database, preparationModel, batchPlan, prodOrderPartslistPosRelation.SourceProdOrderPartslistPos, subPosTargetQuantity);
                }
            }
        }


        private MaterialPreparationBatchModel GetRelationForBatchMaterialModel(VD.ProdOrderBatchPlan batchPlan, VD.ProdOrderPartslistPosRelation prodOrderPartslistPosRelation, double posTargetQuantityUOM)
        {
            MaterialPreparationBatchModel searchBatchMaterialModel = new MaterialPreparationBatchModel();
            searchBatchMaterialModel.MaterialNo = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.Material.MaterialNo;
            searchBatchMaterialModel.MaterialID = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.MaterialID.Value;
            searchBatchMaterialModel.ProdOrderBatchPlanID = batchPlan.ProdOrderBatchPlanID;
            searchBatchMaterialModel.SourceProdOrderPartslistPos = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos;
            searchBatchMaterialModel.TargetQuantityUOM = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.TargetQuantityUOM * (batchPlan.TotalSize / batchPlan.ProdOrderPartslist.TargetQuantity);

            if (batchPlan.VBiACClassWF != null && batchPlan.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Any())
            {
                searchBatchMaterialModel.MDSchedulingGroupID = batchPlan.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Select(c => c.MDSchedulingGroupID).FirstOrDefault();
            }
            if (batchPlan.IplusVBiACClassWF != null)
            {
                searchBatchMaterialModel.PreConfigACUrl = batchPlan.IplusVBiACClassWF.LocalConfigACUrl;
            }


            return searchBatchMaterialModel;
        }


        private void SetFacilitiesOnRouteIds(MaterialPreparationResult materialPreparationResult)
        {
            foreach (MaterialPreparationBatchModel batchModel in materialPreparationResult.MaterialPreparationBatchModels)
            {
                MaterialPreparationWFGroup wfGroup =
                    materialPreparationResult
                    .MaterialPreparationWFGroup
                    .Where(c => c.OutwardMaterials.Contains(batchModel.MaterialNo))
                    .Where(c => c.AllowedInstances.SelectMany(x => x.ProdorderPartslists).Select(x => x.ProdOrderPartslistID).Contains(batchModel.SourceProdOrderPartslistPos.ProdOrderPartslistID))
                    .FirstOrDefault();

                if (wfGroup != null)
                {
                    List<MaterialPreparationAllowedInstances> allowedInstances =
                        wfGroup
                        .AllowedInstances
                        .Where(c => c.ProdorderPartslists.Select(x => x.ProdOrderPartslistID).Contains(batchModel.SourceProdOrderPartslistPos.ProdOrderPartslistID))
                        .ToList();

                    List<Guid> facilityIds = new List<Guid>();
                    foreach(MaterialPreparationAllowedInstances allowedInstance in allowedInstances)
                    {
                        if (allowedInstance != null && allowedInstance.ConnectedFacilities != null)
                        {
                            facilityIds.AddRange(allowedInstance.ConnectedFacilities.Select(c => c.FacilityID).ToArray());
                        }
                    }

                    batchModel.FacilitiesOnRouteIds = facilityIds.ToArray();
                }
            }
        }


        #endregion

        #region PreparedMaterial
        public List<MaterialPreparationModel> GetPreparedMaterials(VD.DatabaseApp databaseApp, ACMediaController mediaController, List<MaterialPreparationBatchModel> researchedFacilities)
        {
            List<MaterialPreparationModel> preparedMaterials = new List<MaterialPreparationModel>();
            var queryResearchedFacilities = researchedFacilities.GroupBy(c => c.MaterialNo);
            string[] materialNos = queryResearchedFacilities.Select(c => c.Key).ToArray();
            List<VD.Material> materials = databaseApp.Material.Where(c => materialNos.Contains(c.MaterialNo)).ToList();
            int nr = 0;
            foreach (var item in queryResearchedFacilities)
            {
                string materialNo = item.Key;
                VD.Material material = materials.FirstOrDefault(c => c.MaterialNo == materialNo);
                mediaController.LoadIImageInfo(material);
                nr++;
                MaterialPreparationModel preparedMaterial = new MaterialPreparationModel() { Sn = nr, PickingRelationType = PickingRelationTypeEnum.ProductionLine };
                preparedMaterial.Material = material;
                preparedMaterial.MaterialNo = material.MaterialNo;
                preparedMaterial.MaterialName = material.MaterialName1;

                preparedMaterial.DefaultThumbImage = material.DefaultThumbImage;
                preparedMaterial.TargetQuantityUOM = item.Sum(x => x.TargetQuantityUOM);
                preparedMaterial.RelatedIDs = item.Select(c => c.SourceProdOrderPartslistPos.ProdOrderPartslistPosID).Distinct().ToArray();

                double availableQuantity =
                    databaseApp
                    .FacilityCharge
                    .Where(c =>
                        !c.NotAvailable
                        && c.Material.MaterialNo == materialNo
                       )
                    .Select(c => c.StockQuantityUOM)
                    .DefaultIfEmpty()
                    .Sum(c => c);
                preparedMaterial.AvailableQuantityUOM = availableQuantity;

                double pickingPosQuantityUOM =
                    databaseApp
                    .Picking
                    .Where(c => c.PickingStateIndex < (short)VD.PickingStateEnum.Finished)
                    .SelectMany(c => c.PickingPos_Picking)
                    .Where(c => c.PickingMaterial.MaterialNo == materialNo && c.PickingPosProdOrderPartslistPos_PickingPos.Any(x => preparedMaterial.RelatedIDs.Contains(x.ProdorderPartslistPosID)))
                    .Select(c => c.PickingQuantityUOM ?? 0)
                    .DefaultIfEmpty()
                    .Sum(c => c);

                double inOrderQuantityUOM =
                    databaseApp
                    .InOrder
                    .Where(c => c.MDInOrderState.MDInOrderStateIndex <= (short)VD.MDInOrderState.InOrderStates.InProcess)
                    .SelectMany(c => c.InOrderPos_InOrder)
                    .Where(c => c.Material.MaterialNo == materialNo)
                    .Select(c => c.TargetQuantityUOM)
                    .DefaultIfEmpty()
                    .Sum(c => c);

                double prodOrderQuantityUOM =
                    databaseApp
                    .ProdOrderPartslist
                    .Where(c => c.MDProdOrderState.MDProdOrderStateIndex <= (short)VD.MDProdOrderState.ProdOrderStates.InProduction)
                    .Where(c => c.Partslist.Material.MaterialNo == materialNo)
                    .Select(c => c.TargetQuantity)
                    .DefaultIfEmpty()
                    .Sum(c => c);

                preparedMaterial.PickingPosQuantityUOM = pickingPosQuantityUOM + inOrderQuantityUOM + prodOrderQuantityUOM;

                preparedMaterial.MissingQuantityUOM = preparedMaterial.TargetQuantityUOM - preparedMaterial.PickingPosQuantityUOM;

                preparedMaterial.MDSchedulingGroupIDs =
                    item
                    .Where(c => c.MDSchedulingGroupID != null)
                    .Select(c => c.MDSchedulingGroupID ?? Guid.Empty)
                    .Distinct()
                    .ToArray();

                preparedMaterial.FacilityIDsOnRoute = item.Where(c => c.FacilitiesOnRouteIds != null).SelectMany(c => c.FacilitiesOnRouteIds).ToArray();

                preparedMaterials.Add(preparedMaterial);
            }
            return preparedMaterials;
        }

        #endregion

        #region Allowed Instances

        private void BuildAllowedInstances(Database database, MaterialPreparationResult materialPreparationResult, VD.ProdOrderBatchPlan batchPlan, VD.ProdOrderPartslistPosRelation prodOrderPartslistPosRelation)
        {
            VD.ProdOrderPartslistPos inwardIntern = prodOrderPartslistPosRelation.TargetProdOrderPartslistPos;
            VD.ProdOrderPartslist prodOrderPartslist = inwardIntern.ProdOrderPartslist;
            VD.Partslist partslist = inwardIntern.ProdOrderPartslist.Partslist;
            VD.MaterialWF materialWF = inwardIntern.ProdOrderPartslist.Partslist.MaterialWF;
            VD.Material outwardMaterial = prodOrderPartslistPosRelation.SourceProdOrderPartslistPos.Material;
            VD.Material inwardMaterial = inwardIntern.Material;

            MaterialPreparationWFGroup wfGroup =
                materialPreparationResult
                .MaterialPreparationWFGroup
                .Where(c => c.MaterialWFID == materialWF.MaterialWFID && c.IntermediateMaterialNo == inwardIntern.Material.MaterialNo)
                .FirstOrDefault();

            if (wfGroup == null)
            {
                wfGroup = new MaterialPreparationWFGroup() { MaterialWFID = materialWF.MaterialWFID, IntermediateMaterialNo = inwardIntern.Material.MaterialNo };

                materialPreparationResult.MaterialPreparationWFGroup.Add(wfGroup);
            }

            VD.ACClassWF[] wfs =
                partslist
                .MaterialWF
                .MaterialWFACClassMethod_MaterialWF
                .SelectMany(c => c.MaterialWFConnection_MaterialWFACClassMethod)
                .Where(c => c.MaterialID == inwardMaterial.MaterialID)
                .Select(c => c.ACClassWF)
                .ToArray();

            foreach (VD.ACClassWF wf in wfs)
            {
                ACClassWF tmp = wf.FromIPlusContext<ACClassWF>(database);
                if (tmp != null && tmp.WFGroup != null)
                {
                    ACClassWF groupWf = tmp.WFGroup as ACClassWF;
                    if (groupWf != null)
                    {
                        if (!wfGroup.ACClassWFs.Select(c => c.ACClassWFID).Contains(groupWf.ACClassWFID))
                        {
                            wfGroup.ACClassWFs.Add(groupWf);
                        }
                    }
                }
            }

            MaterialPreparationAllowedInstances allowedInstance =
                wfGroup
                .AllowedInstances
                .Where(c => c.PartslistIds.Contains(partslist.PartslistID))
                .FirstOrDefault();

            if (allowedInstance == null)
            {
                allowedInstance = new MaterialPreparationAllowedInstances();
                allowedInstance.PartslistIds.Add(partslist.PartslistID);
                wfGroup.AllowedInstances.Add(allowedInstance);
            }

            if (!allowedInstance.ProdorderPartslists.Select(c => c.ProdOrderPartslistID).Contains(prodOrderPartslist.ProdOrderPartslistID))
            {
                allowedInstance.ProdorderPartslists.Add(prodOrderPartslist);
            }

            if (!wfGroup.OutwardMaterials.Contains(outwardMaterial.MaterialNo))
            {
                wfGroup.OutwardMaterials.Add(outwardMaterial.MaterialNo);
            }
        }

        private void FetchConfigurationForAllowedInstances(Database database, ConfigManagerIPlus configManager, MaterialPreparationResult materialPreparationResult)
        {
            foreach (MaterialPreparationWFGroup matWFGroup in materialPreparationResult.MaterialPreparationWFGroup)
            {
                List<MaterialPreparationAllowedInstances> allowedFromOverridedConfig = new List<MaterialPreparationAllowedInstances>();

                foreach (ACClassWF aCClassWF in matWFGroup.ACClassWFs)
                {
                    foreach (MaterialPreparationAllowedInstances allowInstances in matWFGroup.AllowedInstances)
                    {
                        foreach (VD.ProdOrderPartslist pl in allowInstances.ProdorderPartslists)
                        {
                            var materials = materialPreparationResult.MaterialPreparationBatchModels.Where(c => matWFGroup.OutwardMaterials.Any(x => x == c.MaterialNo));

                            foreach (var preparationMaterial in materials)
                            {
                                VD.PartslistACClassMethod mth = pl.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
                                ACClassMethod plMth = mth.MaterialWFACClassMethod.ACClassMethod.FromIPlusContext<ACClassMethod>(database);
                                List<IACConfigStore> mandatoryConfigStores =
                                    configManager.GetACConfigStores(
                                            new List<IACConfigStore>()
                                            {
                                            pl,
                                            pl.Partslist.MaterialWF,
                                            plMth,
                                            aCClassWF.ACClassMethod
                                            });
                                int priorityLevel = 0;
                                IACConfig allowedInstancesOnRouteConfig =
                                    configManager.GetConfiguration(
                                        mandatoryConfigStores,
                                        preparationMaterial.PreConfigACUrl+"\\",
                                        aCClassWF.ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Allowed_instances.ToString(),
                                        null,
                                        out priorityLevel);

                                if (allowedInstancesOnRouteConfig != null)
                                {
                                    MaterialPreparationAllowedInstances withOverridedConfig = null;
                                    if (allowedInstancesOnRouteConfig is VD.ProdOrderPartslistConfig)
                                    {
                                        withOverridedConfig =
                                            allowedFromOverridedConfig
                                            .Where(c =>
                                                        c.ProdorderPartslists
                                                        .Select(x => x.ProdOrderPartslistID)
                                                        .Contains(pl.ProdOrderPartslistID)
                                                    )
                                            .FirstOrDefault();
                                        if (withOverridedConfig == null)
                                        {
                                            withOverridedConfig = new MaterialPreparationAllowedInstances();
                                            withOverridedConfig.PartslistIds.Add(pl.PartslistID ?? Guid.Empty);
                                            withOverridedConfig.ProdorderPartslists.Add(pl);
                                        }
                                    }
                                    else if (allowedInstancesOnRouteConfig is VD.PartslistConfig)
                                    {
                                        withOverridedConfig =
                                            allowedFromOverridedConfig
                                            .Where(c =>
                                                        c.PartslistIds
                                                        .Contains(pl.PartslistID ?? Guid.Empty)
                                                    )
                                            .FirstOrDefault();
                                        if (withOverridedConfig == null)
                                        {
                                            withOverridedConfig = new MaterialPreparationAllowedInstances();
                                            withOverridedConfig.PartslistIds.Add(pl.PartslistID ?? Guid.Empty);
                                            withOverridedConfig.ProdorderPartslists.Add(pl);
                                        }
                                    }

                                    if (withOverridedConfig != null)
                                    {
                                        withOverridedConfig.AllowedInstancesConfig = allowedInstancesOnRouteConfig;
                                        allowedFromOverridedConfig.Add(withOverridedConfig);
                                    }
                                    else
                                    {
                                        if (allowInstances.AllowedInstancesConfig == null)
                                        {
                                            allowInstances.AllowedInstancesConfig = allowedInstancesOnRouteConfig;
                                        }
                                        //else
                                        //{
                                        //    if (allowInstances.AllowedInstancesConfig != allowedInstancesOnRouteConfig)
                                        //    {
                                        //        throw new Exception("Not expected scenario!");
                                        //    }
                                        //}
                                    }
                                }

                            }
                        }
                    }
                }


                matWFGroup.AllowedInstances.AddRange(allowedFromOverridedConfig);
            }
        }

        public void FetchAllowedInstances(Database database, MaterialPreparationResult materialPreparationResult)
        {
            foreach (MaterialPreparationWFGroup matWFGroup in materialPreparationResult.MaterialPreparationWFGroup)
            {
                foreach (ACClassWF aCClassWF in matWFGroup.ACClassWFs)
                {
                    foreach (MaterialPreparationAllowedInstances allowInstances in matWFGroup.AllowedInstances)
                    {
                        if (allowInstances.AllowedInstancesConfig != null)
                        {
                            List<RuleValue> allowedInstancesRuleValueList = RulesCommand.ReadIACConfig(allowInstances.AllowedInstancesConfig);
                            if (allowedInstancesRuleValueList != null && allowedInstancesRuleValueList.Any())
                            {
                                List<string> classes = allowedInstancesRuleValueList.SelectMany(c => c.ACClassACUrl).Distinct().ToList();
                                ACClass[] cls = database.ACClass.Where(c => classes.Contains(c.ACURLCached)).ToArray();
                                foreach (ACClass cl in cls)
                                {
                                    if (!allowInstances.AllowedInstances.Contains(cl))
                                    {
                                        allowInstances.AllowedInstances.Add(cl);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void FetchFacilityInstances(Database database, VD.DatabaseApp databaseApp, ACRoutingParameters routingParameters, MaterialPreparationResult materialPreparationResult)
        {
            ACClass[] cls =
                materialPreparationResult
                .MaterialPreparationWFGroup
                .SelectMany(c => c.AllowedInstances)
                .Where(c => c.AllowedInstances != null)
                .SelectMany(c => c.AllowedInstances)
                .GroupBy(key => key.ACClassID)
                .Select(c => c.FirstOrDefault())
                .ToArray();

            foreach (ACClass cl in cls)
            {
                RoutingResult rResult = ACRoutingService.FindSuccessors(cl, routingParameters);
                if (rResult.Routes != null)
                {
                    Guid[] sourceACClassIDs =
                    rResult.Routes
                    .SelectMany(c => c.Items)
                    .Where(c => c.Source != null)
                    .Select(c => c.Source.ACClassID)
                    .ToArray();

                    List<VD.Facility> facilities =
                        databaseApp
                        .Facility
                        .Where(c =>
                                c.VBiFacilityACClassID != null
                                && sourceACClassIDs.Contains(c.VBiFacilityACClassID ?? Guid.Empty)
                        ).ToList();

                    materialPreparationResult.RoutingResult.Add(cl.ACClassID, facilities);
                }
            }
        }

        public void DistributeAllowedFacilities(MaterialPreparationResult materialPreparationResult)
        {
            foreach (MaterialPreparationWFGroup matWFGroup in materialPreparationResult.MaterialPreparationWFGroup)
            {
                foreach (ACClassWF aCClassWF in matWFGroup.ACClassWFs)
                {
                    foreach (MaterialPreparationAllowedInstances allowInstances in matWFGroup.AllowedInstances)
                    {
                        if (allowInstances.AllowedInstances != null)
                        {
                            foreach (ACClass cls in allowInstances.AllowedInstances)
                            {
                                if (materialPreparationResult.RoutingResult.Keys.Contains(cls.ACClassID))
                                {
                                    List<VD.Facility> facilities = materialPreparationResult.RoutingResult[cls.ACClassID];
                                    foreach (VD.Facility facility in facilities)
                                    {
                                        if (!allowInstances.ConnectedFacilities.Contains(facility))
                                        {
                                            allowInstances.ConnectedFacilities.Add(facility);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Storage bins
        public List<PlanningTargetStockPreview> LoadTargetStorageBins(VD.DatabaseApp databaseApp, MaterialPreparationModel preparedMaterial)
        {
            List<PlanningTargetStockPreview> list = new List<PlanningTargetStockPreview>();

            List<VD.FacilityMDSchedulingGroup> schGroupFacility =
                databaseApp
                .MDSchedulingGroup
                .Where(c => preparedMaterial.MDSchedulingGroupIDs.Contains(c.MDSchedulingGroupID))
                .SelectMany(c => c.FacilityMDSchedulingGroup_MDSchedulingGroup)
                .AsEnumerable()
                .Where(c => preparedMaterial.FacilityIDsOnRoute == null || !preparedMaterial.FacilityIDsOnRoute.Any() || preparedMaterial.FacilityIDsOnRoute.Contains(c.FacilityID))
                .ToList();

            foreach (VD.FacilityMDSchedulingGroup schGroup in schGroupFacility)
            {
                PlanningTargetStockPreview item = list.FirstOrDefault(c => c.FacilityNo == schGroup.Facility.FacilityNo);
                if (item == null)
                {
                    item = new PlanningTargetStockPreview();
                    item.Facility = schGroup.Facility;
                    item.FacilityNo = schGroup.Facility.FacilityNo;
                    item.FacilityName = schGroup.Facility.FacilityName;
                    item.MDPickingType = schGroup.MDPickingType;
                    if (preparedMaterial.Material.FacilityMaterial_Material.Any())
                    {
                        item.OptStockQuantity =
                            preparedMaterial
                            .Material
                            .FacilityMaterial_Material
                            .Where(c => c.FacilityID == schGroup.Facility.FacilityID)
                            .Select(c => c.OptStockQuantity)
                            .FirstOrDefault();
                    }
                    list.Add(item);
                }
            }

            var testConistentQuery =
                schGroupFacility
                .Where(c => c.MDPickingTypeID != null)
                .Select(c => new { c.Facility.FacilityNo, c.Facility.FacilityName, SchedulingGroup_MDKey = c.MDSchedulingGroup.MDKey, c.MDPickingType.MDKey })
                .GroupBy(c => new { c.FacilityNo, c.FacilityName, c.SchedulingGroup_MDKey });

            if (testConistentQuery.Any(c => c.Count() > 1))
            {
                string pickingTypes = string.Join(",", testConistentQuery.Select(c => c));
                Messages.Warning(this, "Warning50054", false, pickingTypes);
            }
            else
            {
                foreach (PlanningTargetStockPreview item in list)
                {
                    item.ActualStockQuantity =
                        databaseApp
                        .FacilityCharge
                        .Where(c => c.MaterialID == preparedMaterial.Material.MaterialID && !c.NotAvailable && c.Facility.FacilityNo == item.FacilityNo)
                        .Select(c => c.StockQuantity)
                        .DefaultIfEmpty()
                        .Sum();

                    item.OrderedQuantity =
                        databaseApp
                        .PickingPos
                        .Where(c =>
                                c.PickingMaterialID == preparedMaterial.Material.MaterialID
                                && (c.Picking.PickingStateIndex < (short)VD.PickingStateEnum.Finished)
                               )
                        .AsEnumerable()
                        .Select(c => c.TargetQuantityUOM - c.ActualQuantityUOM)
                        .DefaultIfEmpty()
                        .Sum();
                }
            }

            foreach (var item in list)
            {
                if (item.MDPickingType != null)
                {
                    item.NewPlannedStockQuantity = 0;
                    if (preparedMaterial.TargetQuantityUOM > item.ActualStockQuantity)
                        item.NewPlannedStockQuantity = preparedMaterial.TargetQuantityUOM - item.ActualStockQuantity;
                    if (item.OptStockQuantity != null && (item.OptStockQuantity ?? 0) > item.ActualStockQuantity)
                        item.NewPlannedStockQuantity += (item.OptStockQuantity ?? 0) - item.ActualStockQuantity;
                    if (item.OptStockQuantity != null)
                    {
                        if (item.NewPlannedStockQuantity > Const_RangeStockQuantityTolerance)
                        {
                            item.IsInRange = -1;
                        }
                        else if (item.NewPlannedStockQuantity == 0)
                        {
                            item.IsInRange = 1;
                        }
                    }
                }
            }

            return list;
        }

        public List<FacilityChargeSumFacilityHelper> LoadSourceStorageBins(VD.DatabaseApp databaseApp, MaterialPreparationModel preparedMaterial, string[] targetsNotShownAsSource)
        {
            VD.FacilityCharge[] facilityCharges =
                FacilityManager
                .s_cQry_MatOverviewFacilityCharge(databaseApp, preparedMaterial.Material.MaterialID, false)
                .Where(c => c.Facility != null && c.Facility.FacilityNo != ProdMatStorage)
                .ToArray();
            List<FacilityChargeSumFacilityHelper> list = GetFacilityChargeSumFacilityHelperList(facilityCharges, new FacilityQueryFilter()).ToList();

            list = list.Where(c => !targetsNotShownAsSource.Contains(c.FacilityNo)).ToList();

            foreach (FacilityChargeSumFacilityHelper item in list)
            {
                item.NewPlannedStock = databaseApp
                        .PickingPos
                        .Where(c =>
                                c.PickingMaterialID == preparedMaterial.Material.MaterialID
                                && (c.Picking.PickingStateIndex < (short)VD.PickingStateEnum.Finished)
                                && c.FromFacility.FacilityNo == item.FacilityNo
                               )
                        .Select(c => c.PickingQuantityUOM ?? 0)
                        .DefaultIfEmpty()
                        .Sum();
            }
            return list;
        }

        #endregion
    }
}
