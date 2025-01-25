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

        public MaterialPreparationResult GetMaterialPreparationModel1(
           Database database,
           VD.DatabaseApp databaseApp,
           ACMediaController mediaController,
           ConfigManagerIPlus configManager,
           ACRoutingParameters routingParameters,
           List<VD.ProdOrderBatchPlan> selectedBatchPlans)
        {
            MaterialPreparationResult materialPreparationResult = new MaterialPreparationResult();

            if (selectedBatchPlans.Any())
            {
                FetchBatchDosings(database, materialPreparationResult, selectedBatchPlans);
                BuildWFNodeList(database, configManager, materialPreparationResult);
                FetchAllowedMachines(database, materialPreparationResult, routingParameters);
                FetchAllowedInstances(databaseApp, routingParameters, materialPreparationResult);

                materialPreparationResult.PreparedMaterials = GetPreparedMaterials(databaseApp, materialPreparationResult.BatchDosings);
                foreach (MaterialPreparationModel preparedMaterial in materialPreparationResult.PreparedMaterials)
                {
                    FillMaterialPreparationModel(databaseApp, preparedMaterial);
                    preparedMaterial.OnRouteFacilityNos = GetOnRouteFacilityNos(preparedMaterial, materialPreparationResult.WFNodes, materialPreparationResult.AllowedInstances);


                    var schedulingGroupFacilities =
                         databaseApp
                        .MDSchedulingGroup
                        .Where(c => preparedMaterial.MDSchedulingGroupIDs.Contains(c.MDSchedulingGroupID))
                        .SelectMany(c => c.FacilityMDSchedulingGroup_MDSchedulingGroup)
                        .ToList();

                    preparedMaterial.FacilityScheduligGroups =
                        schedulingGroupFacilities
                        .Where(c => preparedMaterial.OnRouteFacilityNos == null || !preparedMaterial.OnRouteFacilityNos.Any() || preparedMaterial.OnRouteFacilityNos.Contains(c.Facility.FacilityNo))
                        .ToList();
                }
            }

            return materialPreparationResult;
        }

        #endregion


        #region Storage bins
        public List<PlanningTargetStockPreview> LoadTargetStorageBins(VD.DatabaseApp databaseApp, MaterialPreparationModel preparedMaterial)
        {
            List<PlanningTargetStockPreview> list = new List<PlanningTargetStockPreview>();

            foreach (VD.FacilityMDSchedulingGroup schGroup in preparedMaterial.FacilityScheduligGroups)
            {
                PlanningTargetStockPreview planningTargetStockPreview = GetPlanningTargetStockPreview(databaseApp, schGroup, preparedMaterial);
                list.Add(planningTargetStockPreview);
            }

            return list;
        }

        public PlanningTargetStockPreview GetPlanningTargetStockPreview(VD.DatabaseApp databaseApp, VD.FacilityMDSchedulingGroup schGroup, MaterialPreparationModel preparedMaterial)
        {
            PlanningTargetStockPreview item = new PlanningTargetStockPreview();
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

            return item;
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

        #region Material Preparation 2

        #region Material Preparation 2 -> FetchBatchDosings
        private void FetchBatchDosings(Database database, MaterialPreparationResult materialPreparationResult, List<VD.ProdOrderBatchPlan> selectedBatchPlans)
        {
            List<MaterialPreparationDosing> list = new List<MaterialPreparationDosing>();
            foreach (VD.ProdOrderBatchPlan selectedBatchPlan in selectedBatchPlans)
            {
                FetchBatchDosings(database, materialPreparationResult, selectedBatchPlan);
            }
        }
        private void FetchBatchDosings(Database database, MaterialPreparationResult materialPreparationResult, VD.ProdOrderBatchPlan selectedBatchPlan)
        {
            VD.ProdOrderPartslistPos batchPos = selectedBatchPlan.ProdOrderPartslistPos;
            double factor = selectedBatchPlan.TotalSize / selectedBatchPlan.ProdOrderPartslist.TargetQuantity;
            FetchBatchDosings(database, materialPreparationResult, selectedBatchPlan, batchPos, factor);

        }

        private void FetchBatchDosings(Database database, MaterialPreparationResult materialPreparationResult, VD.ProdOrderBatchPlan batchPlan, VD.ProdOrderPartslistPos batchPos, double factor)
        {
            VD.ProdOrderPartslistPosRelation[] relations = batchPos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.ToArray();
            foreach (VD.ProdOrderPartslistPosRelation relation in relations)
            {
                FetchBatchDosings(database, materialPreparationResult, batchPlan, relation, factor);
            }
        }

        private void FetchBatchDosings(Database database, MaterialPreparationResult materialPreparationResult, VD.ProdOrderBatchPlan batchPlan, VD.ProdOrderPartslistPosRelation relation, double factor)
        {
            if (relation.SourceProdOrderPartslistPos.MaterialPosType == VD.GlobalApp.MaterialPosTypes.OutwardRoot)
            {
                FetchMaterialPreparationDosing(database, materialPreparationResult, batchPlan, relation, factor);
            }
            else
            {
                FetchBatchDosings(database, materialPreparationResult, batchPlan, relation.SourceProdOrderPartslistPos, factor);
            }
        }

        private void FetchMaterialPreparationDosing(Database database, MaterialPreparationResult materialPreparationResult, VD.ProdOrderBatchPlan batchPlan, VD.ProdOrderPartslistPosRelation relation, double factor)
        {
            // prepare for dosing data
            VD.MDSchedulingGroup mDSchedulingGroup = batchPlan.VBiACClassWF.MDSchedulingGroupWF_VBiACClassWF.Select(c => c.MDSchedulingGroup).FirstOrDefault();
            VD.Material inwardMaterial = relation.TargetProdOrderPartslistPos.Material;
            VD.MaterialWF materialWF = batchPlan.ProdOrderPartslist.Partslist.MaterialWF;
            VD.ProdOrderPartslist prodOrderPartslist = batchPlan.ProdOrderPartslist;
            VD.Partslist partslist = batchPlan.ProdOrderPartslist.Partslist;

            MaterialPreparationDosing materialPreparationDosing =
                materialPreparationResult
                .BatchDosings
                .Where(c =>
                        c.MDSchedulingGroup.MDSchedulingGroupID == mDSchedulingGroup.MDSchedulingGroupID
                        && c.InwardMaterial.MaterialNo == inwardMaterial.MaterialNo
                        && c.MaterialWF.MaterialWFID == materialWF.MaterialWFID
                        && c.Partslist.PartslistID == partslist.PartslistID
                        && c.ProdOrderPartslist.ProdOrderPartslistID == prodOrderPartslist.ProdOrderPartslistID

                )
                .FirstOrDefault();

            if (materialPreparationDosing == null)
            {
                materialPreparationDosing = new MaterialPreparationDosing();
                materialPreparationDosing.MDSchedulingGroup = mDSchedulingGroup;
                materialPreparationDosing.InwardMaterial = inwardMaterial;
                materialPreparationDosing.MaterialWF = materialWF;
                materialPreparationDosing.ProdOrderPartslist = prodOrderPartslist;
                materialPreparationDosing.Partslist = partslist;
                materialPreparationDosing.PreConfigACUrl = batchPlan.IplusVBiACClassWF.LocalConfigACUrl;
                materialPreparationDosing.ACClassWFs = GetWFNodes(database, inwardMaterial, partslist);

                materialPreparationResult.BatchDosings.Add(materialPreparationDosing);
            }

            if (!materialPreparationDosing.Dosings.Select(c => c.Relation.ParentProdOrderPartslistPosRelationID).Contains(relation.ProdOrderPartslistPosRelationID))
            {
                MaterialPreparationRelation materialPreparationRelation = new MaterialPreparationRelation(materialPreparationDosing);
                materialPreparationRelation.Relation = relation;
                materialPreparationRelation.Factor = factor;
                materialPreparationDosing.Dosings.Add(materialPreparationRelation);
            }
        }

        private List<ACClassWF> GetWFNodes(Database database, VD.Material inwardMaterial, VD.Partslist partslist)
        {
            List<ACClassWF> wfs = new List<ACClassWF>();
            var acclassWfs =
                               partslist
                            .MaterialWF
                            .MaterialWFACClassMethod_MaterialWF
                            .SelectMany(c => c.MaterialWFConnection_MaterialWFACClassMethod)
                            .Where(c => c.MaterialID == inwardMaterial.MaterialID)
                            .Select(c => c.ACClassWF)
                            .ToList();

            foreach (VD.ACClassWF wf in acclassWfs)
            {
                ACClassWF tmp = wf.FromIPlusContext<ACClassWF>(database);
                if (tmp != null && tmp.WFGroup != null)
                {
                    ACClassWF groupWf = tmp.WFGroup as ACClassWF;
                    if (!wfs.Select(c => c.ACClassWFID).Contains(groupWf.ACClassWFID))
                    {
                        wfs.Add(groupWf);
                    }
                }
            }

            return wfs;
        }

        #endregion

        #region Material Preparation 2 -> Build WFnodelist
        public void BuildWFNodeList(Database database, ConfigManagerIPlus configManager, MaterialPreparationResult materialPreparationResult)
        {
            foreach (MaterialPreparationDosing materialPreparationDosing in materialPreparationResult.BatchDosings)
            {
                FetchMaterialPreparationConfig(database, configManager, materialPreparationResult, materialPreparationDosing);
            }
        }

        private void FetchMaterialPreparationConfig(Database database, ConfigManagerIPlus configManager, MaterialPreparationResult materialPreparationResult, MaterialPreparationDosing materialPreparationDosing)
        {
            VD.PartslistACClassMethod mth = materialPreparationDosing.Partslist.PartslistACClassMethod_Partslist.FirstOrDefault();
            ACClassMethod plMth = mth.MaterialWFACClassMethod.ACClassMethod.FromIPlusContext<ACClassMethod>(database);

            foreach (ACClassWF aCClassWF in materialPreparationDosing.ACClassWFs)
            {
                MaterialPreparationWFNode materialPreparationWFNode =
                    materialPreparationResult
                    .WFNodes
                    .Where(c => c.ACClassWF.ACClassWFID == aCClassWF.ACClassWFID)
                    .FirstOrDefault();

                if (materialPreparationWFNode == null)
                {
                    materialPreparationWFNode = new MaterialPreparationWFNode();
                    materialPreparationWFNode.ACClassWF = aCClassWF;
                    materialPreparationResult.WFNodes.Add(materialPreparationWFNode);
                }

                List<IACConfigStore> mandatoryConfigStores =
                            configManager.GetACConfigStores(
                                    new List<IACConfigStore>()
                                    {
                            materialPreparationDosing.ProdOrderPartslist,
                            materialPreparationDosing.Partslist,
                            materialPreparationDosing.Partslist.MaterialWF,
                            plMth,
                            aCClassWF.ACClassMethod
                                    });

                // Fetch group sub wfs
                List<ACClassWF> allSubWf = aCClassWF.ACClassMethod.ACClassWF_ACClassMethod.Where(c => c.PWACClass.ACKind == Global.ACKinds.TPWGroup).ToList();

                FillConfigNodes(configManager, mandatoryConfigStores, materialPreparationWFNode, aCClassWF, materialPreparationDosing.PreConfigACUrl);
                foreach (ACClassWF subWf in allSubWf)
                {
                   FillConfigNodes(configManager, mandatoryConfigStores, materialPreparationWFNode, subWf, materialPreparationDosing.PreConfigACUrl);
                }
            }
        }

        private void FillConfigNodes(ConfigManagerIPlus configManager, List<IACConfigStore> mandatoryConfigStores,
            MaterialPreparationWFNode materialPreparationWFNode, ACClassWF aCClassWF, string preConfigACUrl)
        {
            int priorityLevel = 0;

            IACConfig aCConfig =
                configManager.GetConfiguration(
                    mandatoryConfigStores,
                     preConfigACUrl + "\\",
                    aCClassWF.ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Allowed_instances.ToString(),
                    null,
                    out priorityLevel);

#if DEBUG
            var allowedInstancesConfigs = 
                mandatoryConfigStores
                .SelectMany(c => c.ConfigurationEntries)
                .Where(c => c.LocalConfigACUrl != null && c.LocalConfigACUrl.Contains("Allowed_instances"))
                .ToArray();
            var allowedInstancesPreview = allowedInstancesConfigs.Select(c => new { c.PreConfigACUrl, c.LocalConfigACUrl }).ToArray();
#endif

            if (aCConfig != null)
            {
                MaterialPreparationConfigNode configNode =
                    materialPreparationWFNode
                    .ConfigNodes
                    .Where(c => c.ACConfig == aCConfig && c.PreConfigACUrl == preConfigACUrl)
                    .FirstOrDefault();

                if (configNode == null)
                {
                    configNode = new MaterialPreparationConfigNode();
                    configNode.PreConfigACUrl = preConfigACUrl;
                    configNode.ACConfig = aCConfig;
                    materialPreparationWFNode.ConfigNodes.Add(configNode);
                }
            }
        }

        #endregion

        #region Material Preparation 2 -> FetchAllowedMachines

        private void FetchAllowedMachines(Database database, MaterialPreparationResult materialPreparationResult, ACRoutingParameters routingParameters)
        {
            foreach (MaterialPreparationWFNode matPrepWFNode in materialPreparationResult.WFNodes)
            {
                FetchAllowedMachines(database, routingParameters, matPrepWFNode);
            }
        }

        private void FetchAllowedMachines(Database database, ACRoutingParameters routingParameters, MaterialPreparationWFNode matPrepWFNode)
        {
            foreach (MaterialPreparationConfigNode configNode in matPrepWFNode.ConfigNodes)
            {
                FetchAllowedMachines(database, routingParameters, configNode);
            }
        }

        private void FetchAllowedMachines(Database database, ACRoutingParameters routingParameters, MaterialPreparationConfigNode configNode)
        {
            List<RuleValue> allowedInstancesRuleValueList = RulesCommand.ReadIACConfig(configNode.ACConfig);
            if (allowedInstancesRuleValueList != null && allowedInstancesRuleValueList.Any())
            {
                List<string> classes = allowedInstancesRuleValueList.SelectMany(c => c.ACClassACUrl).Distinct().ToList();
                ACClass[] cls = database.ACClass.Where(c => classes.Contains(c.ACURLCached)).ToArray();
                foreach (ACClass cl in cls)
                {
                    if (!configNode.AllowedMachines.Select(c => c.ACClassID).Contains(cl.ACClassID))
                    {
                        configNode.AllowedMachines.Add(cl);
                    }
                }
            }
        }

        #endregion

        #region Material Preparation 2 = > FetchAllowedInstances

        private void FetchAllowedInstances(VD.DatabaseApp databaseApp, ACRoutingParameters routingParameters, MaterialPreparationResult materialPreparationResult)
        {
            ACClass[] cls =
                materialPreparationResult
                .WFNodes
                .SelectMany(c => c.ConfigNodes)
                .SelectMany(c => c.AllowedMachines)
                .GroupBy(c => c.ACClassID)
                .Select(c => c.FirstOrDefault())
                .ToArray();


            materialPreparationResult.AllowedInstances.Clear();
            foreach (ACClass cl in cls)
            {
                MaterialPreparationAllowedInstance allowedInstance = new MaterialPreparationAllowedInstance();
                allowedInstance.Machine = cl;
                allowedInstance.ConnectedFacilities = GetConnectedFacilities(databaseApp, routingParameters, cl);
                materialPreparationResult.AllowedInstances.Add(allowedInstance);
            }

            List<VD.Facility> testList = databaseApp.Facility.AsEnumerable().Where(c => c.VBiFacilityACClassID != null
            && cls.Select(x => x.ACClassID).Contains(c.VBiFacilityACClassID ?? Guid.Empty)).ToList();
        }

        private List<VD.Facility> GetConnectedFacilities(VD.DatabaseApp databaseApp, ACRoutingParameters routingParameters, ACClass cl)
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

                return
                    databaseApp
                    .Facility
                    .Where(c =>
                            c.VBiFacilityACClassID != null
                            && sourceACClassIDs.Contains(c.VBiFacilityACClassID ?? Guid.Empty)
                    ).ToList();

            }

            return new List<VD.Facility>();
        }


        #endregion

        #region Material Preparation 2 -> PreparedMaterials

        private List<MaterialPreparationModel> GetPreparedMaterials(VD.DatabaseApp databaseApp, List<MaterialPreparationDosing> batchDosings)
        {
            Guid[] materialIDs =
                batchDosings
                .SelectMany(c => c.Dosings)
                .Select(c => c.Relation.SourceProdOrderPartslistPos.MaterialID ?? Guid.Empty)
                .Distinct()
                .ToArray();

            List<VD.Material> materials =
                databaseApp
                .Material
                .Where(c => materialIDs.Contains(c.MaterialID))
                .OrderBy(c => c.MaterialNo)
                .ToList();

            List<MaterialPreparationModel> preparedMaterials =
                materials
                .Select(c => new MaterialPreparationModel()
                {
                    Material = c,
                    MaterialNo = c.MaterialNo,
                    MaterialName = c.MaterialName1
                })
                .ToList();

            foreach (MaterialPreparationModel preparedMaterial in preparedMaterials)
            {
                preparedMaterial.Dosings =
                     batchDosings
                    .SelectMany(c => c.Dosings)
                    .Where(c => c.Relation.SourceProdOrderPartslistPos.Material.MaterialNo == preparedMaterial.MaterialNo)
                    .ToList();
            }

            return preparedMaterials;
        }

        private void FillMaterialPreparationModel(VD.DatabaseApp databaseApp, MaterialPreparationModel preparedMaterial)
        {
            preparedMaterial.DefaultThumbImage = preparedMaterial.Material.DefaultThumbImage;
            preparedMaterial.TargetQuantityUOM =
                preparedMaterial
                .Dosings
                .Select(c => c.Relation.TargetQuantityUOM * c.Factor)
                .DefaultIfEmpty()
                .Sum();

            preparedMaterial.RelatedOutwardPosIDs =
                preparedMaterial
                .Dosings
                .Select(c => c.Relation.SourceProdOrderPartslistPosID)
                .Distinct()
                .ToArray();

            double availableQuantity =
                databaseApp
                .FacilityCharge
                .Where(c =>
                    !c.NotAvailable
                    && c.Material.MaterialNo == preparedMaterial.MaterialNo
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
                .Where(c => c.PickingMaterial.MaterialNo == preparedMaterial.MaterialNo && c.PickingPosProdOrderPartslistPos_PickingPos.Any(x => preparedMaterial.RelatedOutwardPosIDs.Contains(x.ProdorderPartslistPosID)))
                .Select(c => c.PickingQuantityUOM ?? 0)
                .DefaultIfEmpty()
                .Sum(c => c);

            double inOrderQuantityUOM =
                databaseApp
                .InOrder
                .Where(c => c.MDInOrderState.MDInOrderStateIndex <= (short)VD.MDInOrderState.InOrderStates.InProcess)
                .SelectMany(c => c.InOrderPos_InOrder)
                .Where(c => c.Material.MaterialNo == preparedMaterial.MaterialNo)
                .Select(c => c.TargetQuantityUOM)
                .DefaultIfEmpty()
                .Sum(c => c);

            double prodOrderQuantityUOM =
                databaseApp
                .ProdOrderPartslist
                .Where(c => c.MDProdOrderState.MDProdOrderStateIndex <= (short)VD.MDProdOrderState.ProdOrderStates.InProduction)
                .Where(c => c.Partslist.Material.MaterialNo == preparedMaterial.MaterialNo)
                .Select(c => c.TargetQuantity)
                .DefaultIfEmpty()
                .Sum(c => c);

            preparedMaterial.PickingPosQuantityUOM = pickingPosQuantityUOM + inOrderQuantityUOM + prodOrderQuantityUOM;

            preparedMaterial.MissingQuantityUOM = preparedMaterial.TargetQuantityUOM - preparedMaterial.PickingPosQuantityUOM;

            preparedMaterial.MDSchedulingGroupIDs =
                preparedMaterial
                .Dosings
                .Select(c => c.MaterialPreparationDosing)
                .Where(c => c.Dosings.Any(x => x.Relation.SourceProdOrderPartslistPos.Material.MaterialNo == preparedMaterial.MaterialNo))
                .Select(c => c.MDSchedulingGroup.MDSchedulingGroupID)
                .Distinct()
                .ToArray();
        }

        private string[] GetOnRouteFacilityNos(MaterialPreparationModel preparedMaterial, List<MaterialPreparationWFNode> wfNodes, List<MaterialPreparationAllowedInstance> allowedInstances)
        {
            List<ACClass> allowedMachines = GetMaterialPreparationAllowedMachines(preparedMaterial, wfNodes);
            return GetMachineFacilities(allowedInstances, allowedMachines);
        }

        private List<ACClass> GetMaterialPreparationAllowedMachines(MaterialPreparationModel preparedMaterial, List<MaterialPreparationWFNode> wfNodes)
        {
            List<ACClass> machines = new List<ACClass>();

            foreach (MaterialPreparationRelation dosing in preparedMaterial.Dosings)
            {
                foreach (ACClassWF wf in dosing.MaterialPreparationDosing.ACClassWFs)
                {
                    MaterialPreparationWFNode matPrepWFNode = wfNodes.Where(c => c.ACClassWF.ACClassWFID == wf.ACClassWFID).FirstOrDefault();
                    if (matPrepWFNode != null)
                    {
                        MaterialPreparationConfigNode configNode = matPrepWFNode.ConfigNodes.Where(c => c.PreConfigACUrl == dosing.MaterialPreparationDosing.PreConfigACUrl).FirstOrDefault();
                        if (configNode != null)
                        {
                            foreach (ACClass machine in configNode.AllowedMachines)
                            {
                                if (!machines.Select(c => c.ACClassID).Contains(machine.ACClassID))
                                {
                                    machines.Add(machine);
                                }
                            }
                        }
                    }
                }
            }

            return machines;
        }

        private string[] GetMachineFacilities(List<MaterialPreparationAllowedInstance> allowedInstances, List<ACClass> allowedMachines)
        {
            return
                allowedInstances
                .Where(c => allowedMachines.Select(x => x.ACClassID).Contains(c.Machine.ACClassID))
                .SelectMany(c => c.ConnectedFacilities)
                .Select(c => c.FacilityNo)
                .Distinct()
                .ToArray();
        }

        #endregion

        #endregion
    }
}
