using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace gip.mes.facility
{

    public class MapPosToWFConnSubItem
    {
        public core.datamodel.ACClassWF PWNode { get; set; }
        public Dictionary<Material, List<Route>> Mat4DosingAndRoutes { get; set; } = new Dictionary<Material, List<Route>>();
    }

    public class MapPosToWFConn
    {
        public IPartslistPos Pos { get; set; }
        public MaterialWFConnection MatWFConn { get; set; }
        public gip.core.datamodel.ACClassWF PWNode { get; set; }

        private Type _PWObjectType;
        public Type PWObjectType
        {
            get
            {
                if (_PWObjectType != null)
                    return _PWObjectType;
                if (PWNode != null && PWNode.PWACClass != null)
                    _PWObjectType = PWNode.PWACClass.ObjectType;
                return _PWObjectType;
            }
        }

        public List<MapPosToWFConnSubItem> MapPosToWFConnSubItems { get; set; } = new List<MapPosToWFConnSubItem>();

        public double CalcExpectedBatchWeightAtThisIntermPos(DatabaseApp dbApp, double batchScaleFactor)
        {
            double targetQuantityUOM = Pos.TargetQuantityUOM * batchScaleFactor;
            MDUnit weightUnit = MDUnit.GetSIUnit(dbApp, GlobalApp.SIDimensions.Mass);
            // Recalc to weight
            if (weightUnit != null && Pos.Material.BaseMDUnitID != weightUnit.MDUnitID)
            {
                try
                {
                    targetQuantityUOM = Pos.Material.ConvertFromBaseQuantity(targetQuantityUOM, weightUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException("MapPosToWFConn", "CalcExpectedBatchWeightAtThisIntermPos", msg);
                }
            }
            return targetQuantityUOM;
        }
    }

    public class CheckResourcesAndRoutingEventArgs : EventArgs
    {
        public CheckResourcesAndRoutingEventArgs(DatabaseApp dbApp, Database dbIPlus, IPartslist pList,
                                                    List<IACConfigStore> configStores, List<MapPosToWFConn> mapPosToWFConn,
                                                    PARole.ValidationBehaviour validationBehaviour,
                                                    MsgWithDetails detailMessages,
                                                    MapPosToWFConn batchInvoker,
                                                    IEnumerable<ProdOrderBatchPlan> openPlans)
        {
            _db = dbApp;
            _dbIPlus = dbIPlus;
            _pList = pList;
            _configStores = configStores;
            _mapPosToWFConn = mapPosToWFConn;
            _validationBehaviour = validationBehaviour;
            _detailMessages = detailMessages;
            _batchInvoker = batchInvoker;
            _OpenPlans = openPlans;
        }

        DatabaseApp _db;
        public DatabaseApp Db
        {
            get
            {
                return _db;
            }
        }

        Database _dbIPlus;
        public Database DbIPlus
        {
            get
            {
                return _dbIPlus;
            }
        }

        IPartslist _pList;
        public IPartslist PList
        {
            get
            {
                return _pList;
            }
        }

        List<IACConfigStore> _configStores;
        public List<IACConfigStore> ConfigStores
        {
            get
            {
                return _configStores;
            }
        }

        List<MapPosToWFConn> _mapPosToWFConn;
        public List<MapPosToWFConn> MapPosToWFConn
        {
            get
            {
                return _mapPosToWFConn;
            }
        }

        PARole.ValidationBehaviour _validationBehaviour;
        public PARole.ValidationBehaviour ValidationBehaviour
        {
            get
            {
                return _validationBehaviour;
            }
        }

        MsgWithDetails _detailMessages;
        public MsgWithDetails DetailMessages
        {
            get
            {
                return _detailMessages;
            }
        }

        MapPosToWFConn _batchInvoker;
        public MapPosToWFConn BatchInvoker
        {
            get
            {
                return _batchInvoker;
            }
        }

        IEnumerable<ProdOrderBatchPlan> _OpenPlans;
        public IEnumerable<ProdOrderBatchPlan> OpenPlans
        {
            get
            {
                return _OpenPlans;
            }
        }

    }

    public delegate void CheckResourcesAndRoutingEventHandler(object sender, CheckResourcesAndRoutingEventArgs e);


    public partial class ACPartslistManager
    {

        #region events
        public event CheckResourcesAndRoutingEventHandler CheckResourcesAndRoutingEvent;
        #endregion

        #region Helper-Classes
        protected class MapPWGroup2Modules
        {
            public gip.core.datamodel.ACClassWF PWGroup { get; set; }
            public List<gip.core.datamodel.ACClass> ProcessModuleList { get; set; }
        }
        #endregion

        #region Routing-Checks

        public virtual MsgWithDetails ValidateRoutes(DatabaseApp dbApp, Database dbiPlus, Partslist partslist, List<IACConfigStore> configStores, PARole.ValidationBehaviour validationBehaviour)
        {
            MsgWithDetails detailMessages = new MsgWithDetails();

            foreach (PartslistPos pos in partslist.PartslistPos_Partslist.Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot))
            {
                pos.PartslistPosRelation_SourcePartslistPos.AutoRefresh(dbApp);
                pos.PartslistPosRelation_TargetPartslistPos.AutoRefresh(dbApp);
                if (!pos.PartslistPosRelation_SourcePartslistPos.Any())
                {
                    // Stücklistenposition {0} {1} {2} ist keinem Zwischenmaterial zugeordnet.
                    detailMessages.AddDetailMessage(new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Warning,
                        ACIdentifier = "ValidateRoutes(10)",
                        Message = Root.Environment.TranslateMessage(this, "Warning50013", pos.Sequence, pos.Material.MaterialNo, pos.MaterialName)
                    });
                }
            }

            CheckResourcesAndRouting(dbApp, dbiPlus, partslist, configStores, validationBehaviour, detailMessages);

            return detailMessages;
        }

        #region Virtual and protected
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbApp"></param>
        /// <param name="dbiPlus"></param>
        /// <param name="iPartslist"></param>
        /// <param name="configStores"></param>
        /// <param name="validationBehaviour"></param>
        /// <param name="detailMessages"></param>
        /// <param name="checkAll">return all combination if true - otherwise only first finded</param>
        /// <returns></returns>
        public PartslistValidationInfo CheckResourcesAndRouting(
            DatabaseApp dbApp,
            Database dbiPlus,
            IPartslist iPartslist,
            List<IACConfigStore> configStores,
            PARole.ValidationBehaviour validationBehaviour,
            MsgWithDetails detailMessages,
            core.datamodel.ACClassWF invokerPWNode = null,
            bool collectingData = false)
        {
            PartslistValidationInfo validationInfo = new PartslistValidationInfo();
            List<MapPosToWFConn> matWFConnections = new List<MapPosToWFConn>();

            Partslist partslist = iPartslist is ProdOrderPartslist ? (iPartslist as ProdOrderPartslist).Partslist : iPartslist as Partslist;

            if (partslist != null
                && partslist.MaterialWF != null
                && partslist.MaterialWF.MaterialWFACClassMethod_MaterialWF.Any()
                )
            {
                partslist.PartslistACClassMethod_Partslist.AutoRefresh(dbApp);
                if (!partslist.PartslistACClassMethod_Partslist.Any())
                {
                    // Der Stückliste wurde kein Steuerrezept zugeordnet obwohl im Materialflussplan mindestens ein Steuerrezept zugeordnet worden ist.
                    detailMessages.AddDetailMessage(new Msg
                    {
                        Source = GetACUrl(),
                        MessageLevel = eMsgLevel.Error,
                        ACIdentifier = "CheckResourcesAndRouting(10)",
                        Message = Root.Environment.TranslateMessage(this, "Error50100")
                    });
                }
                else
                {
                    foreach (var plACMethod in partslist.PartslistACClassMethod_Partslist)
                    {
                        MaterialWFACClassMethod mwfMethod = plACMethod.MaterialWFACClassMethod;
                        foreach (PartslistPos intermediatePos in partslist.PartslistPos_Partslist.Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern))
                        {
                            var inwardConnections = dbApp.MaterialWFConnection
                                .Include(c => c.ACClassWF)
                                .Include(c => c.ACClassWF.RefPAACClassMethod)
                                .Include(c => c.ACClassWF.PWACClass)
                                .Where(c => c.MaterialWFACClassMethodID == mwfMethod.MaterialWFACClassMethodID && c.MaterialID == intermediatePos.MaterialID).ToArray();

                            //var inwardConnections = intermediatePos.Material.MaterialWFConnection_Material.Where(c => c.MaterialWFACClassMethodID == mwfMethod.MaterialWFACClassMethodID).ToArray();
                            if (inwardConnections == null || !inwardConnections.Any())
                            {
                                // Die Zwischenproduktsposition {0} {1} {2} ist NICHT im Steuerrezept oder seinen Untersteuerrezepten zugeordnet worden.
                                detailMessages.AddDetailMessage(new Msg
                                {
                                    Source = GetACUrl(),
                                    MessageLevel = eMsgLevel.Warning,
                                    ACIdentifier = "CheckResourcesAndRouting(20)",
                                    Message = Root.Environment.TranslateMessage(this, "Warning50014", intermediatePos.Sequence, intermediatePos.MaterialNo, intermediatePos.MaterialName)
                                });
                            }
                            else
                            {
                                IPartslistPos iIntermediatePos = null;
                                ProdOrderPartslist poPL = iPartslist as ProdOrderPartslist;
                                if (poPL != null)
                                    iIntermediatePos = poPL.ProdOrderPartslistPos_ProdOrderPartslist.Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern && c.MaterialID == intermediatePos.MaterialID).FirstOrDefault();
                                if (iIntermediatePos == null)
                                    iIntermediatePos = intermediatePos;
                                matWFConnections.AddRange(inwardConnections.Select(c => new MapPosToWFConn() { Pos = iIntermediatePos, MatWFConn = c }));
                            }
                        }
                    }
                }
            }

            if (detailMessages.IsSucceded())
            {
                validationInfo = CheckResourcesAndRouting(dbApp, dbiPlus, iPartslist, configStores, matWFConnections, validationBehaviour, detailMessages, invokerPWNode, collectingData);
            }
            return validationInfo;
        }


        protected virtual PartslistValidationInfo CheckResourcesAndRouting(DatabaseApp dbApp, Database dbIPlus, IPartslist pList,
                                                    List<IACConfigStore> configStores, List<MapPosToWFConn> mapPosToWFConn,
                                                    PARole.ValidationBehaviour validationBehaviour,
                                                    MsgWithDetails detailMessages,
                                                    core.datamodel.ACClassWF invokerPWNode = null,
                                                    bool collectingData = false)
        {
            PartslistValidationInfo validationInfo = new PartslistValidationInfo();
            if (!mapPosToWFConn.Any() || configStores == null)
            {
                validationInfo.IsSucceded = false;
                return validationInfo;
            }
            foreach (var mapElement in mapPosToWFConn)
            {
                if (mapElement.PWNode == null && mapElement.MatWFConn.ACClassWF != null)
                {
                    //mapElement.PWNode = mapElement.MatWFConn.ACClassWF.FromIPlusContext<gip.core.datamodel.ACClassWF>(dbiPlus);
                    mapElement.PWNode = dbIPlus.ACClassWF.Include(c => c.PWACClass)
                                                        .Include(c => c.ACClassWF1_ParentACClassWF)
                                                        .Where(c => c.ACClassWFID == mapElement.MatWFConn.ACClassWFID)
                                                        .FirstOrDefault();
                }
            }

            Type typeDeliverMat = typeof(IPWNodeDeliverMaterial);
            Type typeReceiveMat = typeof(IPWNodeReceiveMaterialRouteable);
            Type typeCheckWeight = typeof(IPWNodeCheckWeight);

            // 1. Determine which Nodes in workflow are PWNodeProcessWorkflow-Instance which invokes Subworkflows for each Batch
            var connToBatchInvocNodes =
                mapPosToWFConn
                .Where(c =>
                            c.MatWFConn.ACClassWF.RefPAACClassMethod != null
                            && c.MatWFConn.ACClassWF.RefPAACClassMethod.ACKindIndex == (short)Global.ACKinds.MSWorkflow
                            && (invokerPWNode == null || c.MatWFConn.ACClassWF.ACClassWFID == invokerPWNode.ACClassWFID)
                      );
            validationInfo.MapPosToWFConnections = connToBatchInvocNodes.ToList();
            // Loop through Batch-Nodes
            foreach (var mapPosWF in connToBatchInvocNodes)
            {
                // Is there any active Batchplan for this Node?
                IEnumerable<ProdOrderBatchPlan> openPlans = null;
                ProdOrderPartslist prodOrderPartsList = pList as ProdOrderPartslist;
                if (prodOrderPartsList != null)
                {
                    openPlans = prodOrderPartsList.ProdOrderBatchPlan_ProdOrderPartslist.Where(c => c.VBiACClassWFID.HasValue
                                                                     && c.VBiACClassWFID == mapPosWF.MatWFConn.ACClassWFID
                                                                     && c.PlanStateIndex <= (short)GlobalApp.BatchPlanState.InProcess
                                                                     && !c.IsValidated
                                                                     && (c.BatchSize > 0.0001 || c.TotalSize > 0.0001));
                }
                if (openPlans == null || openPlans.Any())
                {
                    gip.mes.datamodel.ACClass[] possibleClassProjects = mapPosWF.MatWFConn.ACClassWF.RefPAACClass.ACClass_BasedOnACClass.ToArray();
                    // 2. Determine on which Application-Projects can subworkflows be started (Line-Check)
                    possibleClassProjects = ApplyRulesOnProjects(dbIPlus, possibleClassProjects, mapPosWF, configStores);
                    if (possibleClassProjects != null && possibleClassProjects.Any())
                    {
                        Guid[] possibleProjectIDs = possibleClassProjects.Select(c => c.ACProjectID).ToArray();
                        // Determine all PWGroups of the subworkflow and determine which ProcessModules could be mapped
                        MapPWGroup2Modules[] mapPWGroup2Modules = dbIPlus.ACClassWF
                                                            .Include(c => c.RefPAACClass.ACClass_BasedOnACClass)
                                                            .Where(c => c.ACClassMethodID == mapPosWF.MatWFConn.ACClassWF.RefPAACClassMethodID
                                                                && c.PWACClass.ACKindIndex == (short)Global.ACKinds.TPWGroup
                                                                && c.RefPAACClassID.HasValue)
                                                          .ToArray()
                                                          .Select(c => new MapPWGroup2Modules()
                                                          {
                                                              PWGroup = c,
                                                              ProcessModuleList = c.RefPAACClass.ACClass_BasedOnACClass
                                                                                  .Where(d => possibleProjectIDs.Contains(d.ACProjectID))
                                                                                  .ToList()
                                                          })
                                                          .ToArray();
                        // Apply rules to filter out the Processmodules which are allowed
                        foreach (var mapPWGroup2Module in mapPWGroup2Modules)
                        {
                            ApplyRulesOnPossibleInstances(dbIPlus, mapPosWF, mapPWGroup2Module, configStores);
                        }


                        // Find Discharging and Checkweighing-Nodes in Subworkflow which are connected to a intermediate Material
                        var nodes2CheckForMaxWeight = mapPosToWFConn.Where(c => c.PWNode != null
                                            && c.PWNode.ACClassMethodID == mapPosWF.MatWFConn.ACClassWF.RefPAACClassMethodID
                                            && c.PWNode.PWACClass != null
                                            && (typeDeliverMat.IsAssignableFrom(c.PWObjectType) || typeCheckWeight.IsAssignableFrom(c.PWObjectType)));
                        if (nodes2CheckForMaxWeight.Any())
                        {
                            foreach (var node2Check in nodes2CheckForMaxWeight)
                            {
                                // Has Parent-PWGroup
                                if (node2Check.PWNode.ACClassWF1_ParentACClassWF != null)
                                {
                                    // Find mappingInfo for PWGroup (which ProcessModules could be used)
                                    var pwGroup2Check = mapPWGroup2Modules.Where(c => c.PWGroup == node2Check.PWNode.ACClassWF1_ParentACClassWF).FirstOrDefault();
                                    if (pwGroup2Check != null)
                                    {
                                        // Determine maximum Weight/Volume in Module
                                        double? minMaxWeight = null;
                                        short? maxRepeatsInterDis = null;
                                        gip.core.datamodel.ACClass acClassWithMinCapacity = null;
                                        foreach (var acClassPM in pwGroup2Check.ProcessModuleList)
                                        {
                                            double maxWeight = 0;
                                            var acClassProperty = acClassPM.GetProperty(nameof(PAProcessModule.MaxWeightCapacity));
                                            if (acClassProperty != null && acClassProperty.Value != null && acClassProperty.Value is string)
                                                maxWeight = (double)ACConvert.ChangeType(acClassProperty.Value as string, typeof(double), true, dbIPlus);
                                            if (maxWeight > 0.001)
                                            {
                                                if (!minMaxWeight.HasValue || maxWeight < minMaxWeight.Value)
                                                {
                                                    minMaxWeight = maxWeight;
                                                    acClassWithMinCapacity = acClassPM;

                                                    acClassProperty = acClassPM.GetProperty(nameof(PAProcessModule.MaxCapacityRepeat));
                                                    if (acClassProperty != null && acClassProperty.Value != null && acClassProperty.Value is string)
                                                        maxRepeatsInterDis = (short)ACConvert.ChangeType(acClassProperty.Value as string, typeof(short), true, dbIPlus);
                                                }
                                            }
                                        }

                                        if (minMaxWeight.HasValue)
                                        {
                                            if (openPlans != null)
                                            {
                                                foreach (var batchPlan in openPlans)
                                                {
                                                    double factor = batchPlan.PlanMode == BatchPlanMode.UseTotalSize ? batchPlan.TotalSize / prodOrderPartsList.TargetQuantity : batchPlan.BatchSize / prodOrderPartsList.TargetQuantity;
                                                    double expectedBatchWeight = node2Check.CalcExpectedBatchWeightAtThisIntermPos(dbApp, factor);
                                                    double totalSumMinMax = minMaxWeight.Value;
                                                    if (maxRepeatsInterDis.HasValue && maxRepeatsInterDis > 0)
                                                        totalSumMinMax = maxRepeatsInterDis.Value * minMaxWeight.Value;
                                                    if (Math.Round(expectedBatchWeight) > Math.Round(totalSumMinMax))
                                                    {
                                                        // Die zu erwartende Batchgröße von {0}kg würde die maximal erlaubte Kapazität von {1}kg im Prozessmodul {2},{3} überschreiten!
                                                        detailMessages.AddDetailMessage(new Msg
                                                        {
                                                            Source = GetACUrl(),
                                                            MessageLevel = validationBehaviour == PARole.ValidationBehaviour.Strict ? eMsgLevel.Error : eMsgLevel.Warning,
                                                            ACIdentifier = "CheckResourcesAndRouting(10)",
                                                            Message = Root.Environment.TranslateMessage(this, "Warning50015",
                                                                            expectedBatchWeight,
                                                                            minMaxWeight.Value,
                                                                            acClassWithMinCapacity.ACCaption,
                                                                            acClassWithMinCapacity.GetACUrlComponent())
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        // Find Dosing-Nodes in Subworkflow which are connected to a intermediate Material
                        IEnumerable<MapPosToWFConn> nodes2CheckForRouting = mapPosToWFConn.Where(c => c.PWNode != null
                                            && c.PWNode.ACClassMethodID == mapPosWF.MatWFConn.ACClassWF.RefPAACClassMethodID
                                            && c.PWNode.PWACClass != null
                                            && typeReceiveMat.IsAssignableFrom(c.PWObjectType));
                        if (nodes2CheckForRouting.Any())
                        {
                            List<IPartslistPosRelation> dosableRelations = new List<IPartslistPosRelation>();
                            List<IPartslistPosRelation> allRelations = new List<IPartslistPosRelation>();

                            foreach (MapPosToWFConn node2Check in nodes2CheckForRouting)
                            {
                                if (!node2Check.Pos.I_PartslistPosRelation_TargetPartslistPos.Any())
                                    continue;
                                // Has Parent-PWGroup
                                if (node2Check.PWNode.ACClassWF1_ParentACClassWF != null)
                                {
                                    IPartslistPosRelation[] materialsToCheck = node2Check.Pos.I_PartslistPosRelation_TargetPartslistPos.ToArray();
                                    foreach (IPartslistPosRelation mat4Dosing in materialsToCheck)
                                    {
                                        if (!allRelations.Contains(mat4Dosing))
                                            allRelations.Add(mat4Dosing);
                                        if (!collectingData && dosableRelations.Contains(mat4Dosing))
                                            continue;
                                        if (!IsRouteValidationNeededForPos(mat4Dosing, dbApp, dbIPlus, pList, configStores, mapPosToWFConn, validationBehaviour, detailMessages))
                                        {
                                            dosableRelations.Add(mat4Dosing);
                                            continue;
                                        }

                                        MapPosToWFConnSubItem subItem = mapPosWF.MapPosToWFConnSubItems.Where(c => c.PWNode.ACClassWFID == node2Check.PWNode.ACClassWFID).FirstOrDefault();
                                        if (subItem == null)
                                        {
                                            subItem = new MapPosToWFConnSubItem() { PWNode = node2Check.PWNode };
                                            mapPosWF.MapPosToWFConnSubItems.Add(subItem);
                                        }

                                        if (!subItem.Mat4DosingAndRoutes.Select(x => x.Key.MaterialNo).Contains(mat4Dosing.I_SourcePartslistPos.MaterialNo))
                                        {
                                            subItem.Mat4DosingAndRoutes.Add(mat4Dosing.I_SourcePartslistPos.Material, new List<Route>());
                                        }
                                        IEnumerable<Route> routes = null;
                                        // Find mappingInfo for PWGroup (which ProcessModules could be used)
                                        MapPWGroup2Modules[] submapPWGroup2Modules = mapPWGroup2Modules.Where(c => c.PWGroup == node2Check.PWNode.ACClassWF1_ParentACClassWF).ToArray();
                                        foreach (var pwGroup2Check in submapPWGroup2Modules)
                                        {
                                            foreach (var acClassPM in pwGroup2Check.ProcessModuleList)
                                            {
                                                facility.ACPartslistManager.QrySilosResult possibleSilos;
                                                facility.ACPartslistManager.QrySilosResult allSilos;
                                                routes = GetRoutes(mat4Dosing, dbApp, dbIPlus, acClassPM, SearchMode.AllSilos, null, out possibleSilos, out allSilos, null, null);
                                                if (routes != null && routes.Any())
                                                {
                                                    dosableRelations.Add(mat4Dosing);

                                                    KeyValuePair<Material, List<Route>> mat4DosingRoute = subItem.Mat4DosingAndRoutes.FirstOrDefault(c => c.Key.MaterialNo == mat4Dosing.I_SourcePartslistPos.MaterialNo);
                                                    mat4DosingRoute.Value.AddRange(routes);
                                                    if (!collectingData)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        //if (routes == null || !routes.Any())
                                        //{
                                        //    // Keine Route gefunden über die Material {0} {1} dosiert werden könnte.
                                        //    detailMessages.AddDetailMessage(new Msg
                                        //    {
                                        //        Source = GetACUrl(),
                                        //        MessageLevel = eMsgLevel.Warning,
                                        //        ACIdentifier = "CheckResourcesAndRouting(20)",
                                        //        Message = Root.Environment.TranslateMessage(this, "Warning50016",
                                        //                        mat4Dosing.I_SourcePartslistPos.Material.MaterialNo,
                                        //                        mat4Dosing.I_SourcePartslistPos.Material.MaterialName1)
                                        //    });
                                        //}
                                    }
                                }
                            }

                            allRelations.RemoveAll(c => dosableRelations.Contains(c));
                            foreach (var unsolvedRelation in allRelations)
                            {
                                detailMessages.AddDetailMessage(new Msg
                                {
                                    Source = GetACUrl(),
                                    MessageLevel = eMsgLevel.Warning,
                                    ACIdentifier = "CheckResourcesAndRouting(20)",
                                    Message = Root.Environment.TranslateMessage(this, "Warning50016",
                                                    unsolvedRelation.I_SourcePartslistPos.Material.MaterialNo,
                                                    unsolvedRelation.I_SourcePartslistPos.Material.MaterialName1)
                                });
                            }
                        }

                        OnBatchInvocNodeChecked(dbApp, dbIPlus, pList, configStores, mapPosToWFConn, validationBehaviour, detailMessages, mapPosWF, openPlans);
                    }
                }
            }

            return validationInfo;
        }

        protected gip.mes.datamodel.ACClass[] ApplyRulesOnProjects(Database dbiPlus, gip.mes.datamodel.ACClass[] possibleProjects, MapPosToWFConn mapPosWF, List<IACConfigStore> configStores)
        {
            if (possibleProjects == null || !possibleProjects.Any())
                return possibleProjects;
            RuleValueList ruleValueList = null;
            gip.core.datamodel.ACClassWF acClassWF = mapPosWF.MatWFConn.ACClassWF.FromIPlusContext<gip.core.datamodel.ACClassWF>(dbiPlus);
            ConfigManagerIPlus serviceInstance = ConfigManagerIPlus.GetServiceInstance(this);
            ruleValueList = serviceInstance.GetRuleValueList(configStores, "", acClassWF.ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Allowed_instances.ToString());
            if (ruleValueList != null && ruleValueList.Items != null)
            {
                var selectedClasses = ruleValueList.GetSelectedClasses(ACClassWFRuleTypes.Allowed_instances, dbiPlus);
                if (selectedClasses != null && selectedClasses.Any())
                {
                    var allowedComponents = selectedClasses.Select(c => c.ACClassID);
                    var filteredList = possibleProjects.Where(c => allowedComponents.Contains(c.ACClassID)).ToArray();
                    return filteredList;
                }
            }
            return possibleProjects;
        }

        protected void ApplyRulesOnPossibleInstances(Database dbiPlus, MapPosToWFConn invoker, MapPWGroup2Modules mapPWGroup2Module, List<IACConfigStore> configStores)
        {
            if (mapPWGroup2Module == null || !mapPWGroup2Module.ProcessModuleList.Any())
                return;
            RuleValueList ruleValueList = null;
            ConfigManagerIPlus serviceInstance = ConfigManagerIPlus.GetServiceInstance(this as ACComponent);
            if (invoker.PWNode == null)
                invoker.PWNode = invoker.MatWFConn.ACClassWF.FromIPlusContext<gip.core.datamodel.ACClassWF>(dbiPlus);

            ruleValueList = serviceInstance.GetRuleValueList(configStores,
                String.IsNullOrEmpty(invoker.PWNode.ConfigACUrl) ? invoker.PWNode.ConfigACUrl : invoker.PWNode.ConfigACUrl + "\\",
                mapPWGroup2Module.PWGroup.ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Allowed_instances.ToString());
            if (ruleValueList != null)
            {
                var selectedClasses = ruleValueList.GetSelectedClasses(ACClassWFRuleTypes.Allowed_instances, dbiPlus);
                if (selectedClasses != null && selectedClasses.Any())
                {
                    var allowedComponents = selectedClasses.Select(c => c.ACClassID);
                    mapPWGroup2Module.ProcessModuleList = mapPWGroup2Module.ProcessModuleList.Where(c => allowedComponents.Contains(c.ACClassID)).ToList();
                }
            }
        }

        protected virtual void OnBatchInvocNodeChecked(DatabaseApp dbApp, Database dbIPlus, IPartslist pList,
                                                    List<IACConfigStore> configStores, List<MapPosToWFConn> mapPosToWFConn,
                                                    PARole.ValidationBehaviour validationBehaviour,
                                                    MsgWithDetails detailMessages,
                                                    MapPosToWFConn batchInvoker,
                                                    IEnumerable<ProdOrderBatchPlan> openPlans)
        {
            if (CheckResourcesAndRoutingEvent != null)
                CheckResourcesAndRoutingEvent(this, new CheckResourcesAndRoutingEventArgs(dbApp, dbIPlus, pList, configStores, mapPosToWFConn, validationBehaviour, detailMessages, batchInvoker, openPlans));
        }

        protected virtual bool IsRouteValidationNeededForPos(IPartslistPosRelation mat4Dosing, DatabaseApp dbApp, Database dbIPlus, IPartslist pList,
                                                    List<IACConfigStore> configStores, List<MapPosToWFConn> mapPosToWFConn,
                                                    PARole.ValidationBehaviour validationBehaviour,
                                                    MsgWithDetails detailMessages)
        {
            ProdOrderPartslistPos poPos = mat4Dosing.I_SourcePartslistPos as ProdOrderPartslistPos;
            return (poPos != null
                    && !mat4Dosing.I_SourcePartslistPos.Material.IsIntermediate
                    && (poPos.MDProdOrderPartslistPosState.ProdOrderPartslistPosState < MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed
                        || poPos.MDProdOrderPartslistPosState.ProdOrderPartslistPosState > MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled))
                || (!mat4Dosing.I_SourcePartslistPos.Material.IsIntermediate);
        }
        #endregion


        #endregion

        #region PartslistValidation

        public MsgWithDetails Validate(Partslist partslist)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();

            // Partslist Validate
            MsgWithDetails partslistValidationMsg = ValidatePartslist(partslist);
            msgWithDetails.AddDetailMessage(partslistValidationMsg);

            // Partslist Components Validate
            (MsgWithDetails componentsValidationMsg, PartslistPos[] components) = ValidatePartslistComponent(partslist);
            msgWithDetails.AddDetailMessage(componentsValidationMsg);

            // Partslist Components Dosing Validate
            MsgWithDetails componentDosingValidationMsg = ValidatePartslistComponentDosing(partslist, components);
            msgWithDetails.AddDetailMessage(componentDosingValidationMsg);

            // Partslist Intermediate Validate
            MsgWithDetails validateIntermediate = ValidateIntermediate(partslist);
            msgWithDetails.AddDetailMessage(validateIntermediate);

            PartslistPos finalIntermediate = partslist
                    .PartslistPos_Partslist
                    .Where(x => x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.InwardIntern
                    &&
                    !x.PartslistPosRelation_SourcePartslistPos.Any()
                    ).FirstOrDefault();

            if (finalIntermediate != null)
            {
                Msg finalIntermedateValidationMsg = ValidatePartslistFinalIntermedateQuantity(partslist, finalIntermediate);
                if (finalIntermedateValidationMsg != null)
                {
                    msgWithDetails.AddDetailMessage(finalIntermedateValidationMsg);
                }
            }

            return msgWithDetails;
        }

        public MsgWithDetails ValidatePartslist(Partslist partslist)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();

            // Partslist.PartslistNo
            // en{'Partslist No field is requiered!'}
            if (string.IsNullOrEmpty(partslist.PartslistNo))
            {
                // Error50704
                // ACPartslistManager
                // Recipe No. is required!
                // Rezept-Nr. wird benötigt!
                Msg msg = new Msg(this, eMsgLevel.Error, nameof(FacilityManager), nameof(ValidatePartslist), 691, "Error50704");
                // msgWithDetails.AddDetailMessage(msg);
            }

            // Partslist.PartslistName
            if (string.IsNullOrEmpty(partslist.PartslistName))
            {
                // Error50705
                // ACPartslistManager
                // Recipe Name is required!
                // Rezeptname wird benötigt!
                Msg msg = new Msg(this, eMsgLevel.Error, nameof(FacilityManager), nameof(ValidatePartslist), 701, "Error50705");
                // msgWithDetails.AddDetailMessage(msg);
            }

            // Partslist.Material
            if (partslist.Material == null)
            {
                // Error50706
                // ACPartslistManager
                // Recipe Material is required!
                // Rezeptmaterial wird benötigt!
                Msg msg = new Msg(this, eMsgLevel.Error, nameof(FacilityManager), nameof(ValidatePartslist), 712, "Error50706");
                // msgWithDetails.AddDetailMessage(msg);
            }

            // Partslist.TargetQuantityUOM
            if (partslist.TargetQuantityUOM <= 0)
            {
                // Warning50078
                // ACPartslistManager
                // No reference size was specified in Bill of Material {0}.
                // In Stückliste {0} wurde keine Bezugsgröße angegeben.
                Msg msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslist), 723, "Warning50078", partslist.PartslistNo);
                msgWithDetails.AddDetailMessage(msg);
            }

            // Partslist.EnabledFrom - Partslist.EnabledTo
            if (partslist.EnabledFrom.HasValue && partslist.EnabledTo.HasValue)
            {
                if (partslist.EnabledFrom >= partslist.EnabledTo)
                {
                    // Error50707
                    // ACPartslistManager
                    // Recipe {0} enabled date range {1}-{2} is not valid!
                    // Der für Rezept {0} aktivierte Datumsbereich {1}-{2} ist ungültig!
                    Msg msg = new Msg(this, eMsgLevel.Error, nameof(FacilityManager), nameof(ValidatePartslist), 736, "Error50707", partslist.PartslistNo, partslist.EnabledFrom, partslist.EnabledTo);
                    // msgWithDetails.AddDetailMessage(msg); 
                }
            }

            if (!partslist.IsEnabled)
            {
                if (partslist.MaterialWF != null && partslist.PartslistACClassMethod_Partslist.Any() && partslist.PartslistPos_Partslist.Any())
                {
                    // Warning50079
                    // ACPartslistManager
                    // The Bill of Material {0} is not enabled.
                    // Die Stückliste {0} ist nicht freigegeben.
                    Msg msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslist), 749, "Warning50079", partslist.PartslistNo);
                    msgWithDetails.AddDetailMessage(msg);
                }
            }

            if (partslist.MaterialWF == null)
            {
                // Warning50080
                // ACPartslistManager
                // No material workflow has been assigned to the bill of materials {0}.
                // Der Stückliste {0} wurde kein Materialworkflow zugewiesen.
                Msg msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslist), 760, "Warning50080", partslist.PartslistNo);
                msgWithDetails.AddDetailMessage(msg);

            }
            else if (partslist.Material != null)
            {
                MaterialWF materialWF = partslist.MaterialWF;
                Material[] allMaterials = materialWF.GetMaterials().ToArray();
                MaterialWFRelation[] allRelations = materialWF.MaterialWFRelation_MaterialWF.ToArray();
                Material finalWFMaterial =
                    allMaterials
                    .Where(c =>
                                !allRelations
                                .Where(x => x.SourceMaterialID == c.MaterialID)
                                .Any()
                    )
                    .FirstOrDefault();
                if (finalWFMaterial != null)
                {
                    MDUnit partslistMDUnit = partslist.MDUnit;
                    if (partslistMDUnit == null)
                    {
                        partslistMDUnit = partslist.Material.BaseMDUnit;
                    }
                    if (!finalWFMaterial.BaseMDUnit.IsConvertableToUnit(partslistMDUnit))
                    {
                        // Warning50081
                        // ACPartslistManager
                        // The unit of measurement {0} of material {1} from BOM {2} is not convertible (or compatible) to the unit of measurement {3} of the last intermediate product in the material workflow {4}.
                        // Die Maßeinheit {0} des Materials {1} von Stückliste {2}  ist nicht konvertierbar (bzw. kompatibel) in die Maßeinheit {3} des letzten Zwischenprodukts im Materialworkflow {4}.
                        Msg msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslist), 782, "Warning50081",
                                partslistMDUnit.Symbol,
                                partslist.Material.MaterialNo + " " +partslist.Material.MaterialNo,
                                partslist.PartslistNo,
                                finalWFMaterial.BaseMDUnit.Symbol,
                                finalWFMaterial.MaterialNo + " " + finalWFMaterial.MaterialName1
                            );
                        msgWithDetails.AddDetailMessage(msg);
                    }
                }
            }

            if (!partslist.PartslistACClassMethod_Partslist.Any())
            {
                // Warning50082
                // ACPartslistManager
                // No process workflow has been assigned to the bill of materials {0}.
                // Der Stückliste {0} wurde kein Prozessworkflow zugewiesen.
                Msg msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslist), 802, "Warning50082", partslist.PartslistNo);
                msgWithDetails.AddDetailMessage(msg);
            }



            return msgWithDetails;
        }

        #region PartslistValidation -> ValidatePartslistComponent

        public (MsgWithDetails msgWithDetails, PartslistPos[] components) ValidatePartslistComponent(Partslist partslist)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();

            PartslistPos[] components =
                partslist
                .PartslistPos_Partslist
                .Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot)
                .ToArray();

            // Without material
            if (components.Where(c => c.Material == null).Any())
            {
                // Warning50083
                // ACPartslistManager
                // In the bill of materials {0} there are lines for which no material is assigned.
                // In der Stückliste {0} gibt es Positionen bei denen kein Material zugewiesen ist.
                Msg msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslistComponent), 830, "Warning50083", partslist.PartslistNo);
                msgWithDetails.AddDetailMessage(msg);
            }

            // Without Quantity
            if (components.Where(c => c.TargetQuantityUOM <= 0).Any())
            {
                // Warning50084
                // ACPartslistManager
                // In the bill of materials {0} there are items for which no target quantity is entered.
                // In der Stückliste {0} gibt es Positionen bei denen keine Sollmenge eingetragen ist.
                Msg msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslistComponent), 841, "Warning50084", partslist.PartslistNo);
                msgWithDetails.AddDetailMessage(msg);
            }

            if(partslist.MaterialWF != null)
            {
                // Components not used
                PartslistPos[] notUsedComponents =
                    components
                    .Where(c => !c.I_PartslistPosRelation_SourcePartslistPos.Any())
                    .ToArray();
                if (notUsedComponents.Any())
                {
                    // Warning50085
                    // ACPartslistManager
                    // In the BOM {0} there are lines {1} that are not assigned to an intermediate product in the material workflow.
                    // In der Stückliste {0} gibt es Positionen {1} die keinem Zwischenprodukt im Materialworkflow zugewiesen sind.
                    string componentsStr = GetComponentStr(notUsedComponents);
                    Msg msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslistComponent), 856, "Warning50085", partslist.PartslistNo, componentsStr);
                    msgWithDetails.AddDetailMessage(msg);
                }
            }

            return (msgWithDetails, components);
        }

        public MsgWithDetails ValidatePartslistComponentDosing(Partslist partslist, PartslistPos[] components)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();

            foreach (PartslistPos pos in components)
            {
                if (pos.PartslistPosRelation_SourcePartslistPos.Any() && pos.TargetQuantityUOM > 0 && pos.Material != null)
                {
                    Msg msgNotReachedQuantity = ValidatePartslistComponentDosingAmountReached(partslist, pos);
                    if (msgNotReachedQuantity != null)
                    {
                        msgWithDetails.AddDetailMessage(msgNotReachedQuantity);
                    }

                    //MsgWithDetails validateComponentDosingUnit = ValidatePartslistComponentDosingTargetUnit(partslist, pos);
                    //msgWithDetails.AddDetailMessage(validateComponentDosingUnit);
                }
            }

            return msgWithDetails;
        }

        public Msg ValidatePartslistComponentDosingAmountReached(Partslist partslist, PartslistPos pos)
        {
            Msg msg = null;

            double relQuantity = pos.PartslistPosRelation_SourcePartslistPos.Sum(c => c.TargetQuantityUOM);
            if (!IsDiffSmallerAsPercent(pos.TargetQuantityUOM, relQuantity, 0.1))
            {
                if(pos.TargetQuantityUOM > relQuantity)
                {
                    // Warning50086
                    // ACPartslistManager
                    // The target quantity in line #{1} {2} {3} was not completely allocated to the intermediate products in BOM {0}.
                    // Die Sollmenge von Zeile #{1} {2} {3} wurde nicht vollständig bei den Zwischenprodukten in Stückliste {0} aufgeteilt.
                    msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslist), 897, "Warning50086",
                        partslist.PartslistNo,
                        pos.Sequence,
                        pos.Material.MaterialNo,
                        pos.Material.MaterialName1
                    );
                }
                else
                {
                    // Warning50091
                    // ACPartslistManager
                    // Too much of the target quantity in line #{1} {2} {3} was allocated to the intermediate products in BOM {0}.
                    // Es wurde zu viel von der Sollmenge in Zeile #{1} {2} {3} zu den Zwischenprodukten in Stückliste {0} zugeteilt.
                    msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslist), 920, "Warning50091",
                        partslist.PartslistNo,
                        pos.Sequence,
                        pos.Material.MaterialNo,
                        pos.Material.MaterialName1
                    );
                }
                
            }

            return msg;
        }

        public MsgWithDetails ValidatePartslistComponentDosingTargetUnit(Partslist partslist, PartslistPos pos)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();

            // Component unit not convertable
            PartslistPosRelation[] compRelations = pos.PartslistPosRelation_SourcePartslistPos.ToArray();
            foreach (PartslistPosRelation relation in compRelations)
            {
                if (relation.TargetPartslistPos != null)
                {
                    Msg msg = ValidateComponentDosingTargetUnit(partslist, pos, relation);
                    if (msg != null)
                    {
                        msgWithDetails.AddDetailMessage(msg);
                    }
                }
            }

            return msgWithDetails;
        }

        private Msg ValidateComponentDosingTargetUnit(Partslist partslist, PartslistPos pos, PartslistPosRelation relation)
        {
            Msg msg = null;

            MDUnit sourceMDUnit = pos.MDUnit != null ? pos.MDUnit : pos.Material.BaseMDUnit;
            MDUnit targetMDUnit = relation.TargetPartslistPos.MDUnit != null ? relation.TargetPartslistPos.MDUnit : relation.TargetPartslistPos.Material.BaseMDUnit;
            if (!sourceMDUnit.IsConvertableToUnit(targetMDUnit))
            {
                // Warning50087
                // ACPartslistManager
                // Recipe {0} dosing #{1} {2} {3} into #{4} {5} {6} have problem with material conversion {7} -> {8}!
                // Rezept {0} Dosierung #{1} {2} {3} in #{4} {5} {6} hat ein Problem mit der Materialumwandlung {7} -> {8}!
                msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslist), 966, "Warning50087",
                    partslist.PartslistNo,
                    pos.Sequence,
                    pos.Material.MaterialNo,
                    pos.Material.MaterialName1,
                    relation.Sequence,
                    relation.TargetPartslistPos.Material.MaterialNo,
                    relation.TargetPartslistPos.Material.MaterialName1,
                    sourceMDUnit.Symbol,
                    targetMDUnit.Symbol
                );
                msg = null; // @aagincic: for now no validation becouse is common case, example: final product: stk, component folie (m)
            }

            return msg;
        }

        #endregion 

        #region PartslistValidation -> ValidateIntermediate

        private MsgWithDetails ValidateIntermediate(Partslist partslist)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            PartslistPos[] intermediates =
                partslist
                .PartslistPos_Partslist
                .Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardIntern)
                .ToArray();

            bool enterTargetQuantity =
                intermediates
                .AsEnumerable()
                .Where(c =>
                            c.TargetQuantityUOM <= 0
                            && c.PartslistPosRelation_TargetPartslistPos.Any()
                            &&
                            (
                                (c.MDUnit != null && c.MDUnit.IsQuantityUnit)
                                || (c.Material.BaseMDUnit.IsQuantityUnit)
                            )
                )
                .Any();

            if (enterTargetQuantity)
            {
                // Warning50088
                // ACPartslistManager
                // For intermediate products with commercial units, the target quantity must be entered manually since no total calculation is possible (BOM {0}).
                // Bei Zwischenprodukten mit kommerziellen Einheiten muss die Sollmenge händisch eingetragen werden da keine Summenberechnung möglich ist (Stückliste {0}).
                Msg msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslist), 1014, "Warning50088", partslist.PartslistNo);
                msgWithDetails.AddDetailMessage(msg);
            }

            bool doSumCalc =
                intermediates
                .Where(c =>
                            c.PartslistPosRelation_TargetPartslistPos.Any()
                            && c.IsIntermediateForRecalculate
                            &&
                            (
                                (c.MDUnit != null && c.MDUnit.ISOCode == "KGM")
                                || (c.Material.BaseMDUnit.ISOCode == "KGM")
                            )
                )
                .Any();

            if (doSumCalc)
            {
                // Warning50089
                // ACPartslistManager
                // BOM {0}: Changes were made to the components assigned to the intermediate products in the material workflow, but no total calculation or change to the target quantity was carried out for the intermediate products.
                // Stückliste {0}: Es wurden Änderungen an den Komponenten durchgeführt, die den Zwischenprodukten im Materialworkflow zugeordnet sind aber es wurde keine Summenberechnung oder Änderung der Sollmenge bei den Zwischenprodukten durchgeführt.
                Msg msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslist), 1037, "Warning50089", partslist.PartslistNo);
                msgWithDetails.AddDetailMessage(msg);
            }

            return msgWithDetails;
        }

        private Msg ValidatePartslistFinalIntermedateQuantity(Partslist partslist, PartslistPos finalIntermediate)
        {
            Msg msg = null;

            if (!IsDiffSmallerAsPercent(partslist.TargetQuantityUOM, finalIntermediate.TargetQuantityUOM, 0.1))
            {
                // Warning50090
                // ACPartslistManager
                // The target quantity of the last intermediate product does not match the reference quantity of the bill of materials {0}.
                // Die Sollmenge des letzten Zwischenprodukts stimmt nicht mit der Bezugröße der Stückliste {0} überein.
                msg = new Msg(this, eMsgLevel.Warning, nameof(FacilityManager), nameof(ValidatePartslistFinalIntermedateQuantity), 1054, "Warning50090", partslist.PartslistNo);
            }

            return msg;
        }

        #endregion

        #region PartslistValidation -> Helpers
        private bool IsDiffSmallerAsPercent(double num1, double num2, double percent)
        {
            return (Math.Abs(num1 - num2) / num1) < percent;
        }

        private string GetComponentStr(PartslistPos[] notUsedComponents)
        {
            return
                string.Join(",",
                notUsedComponents
                .Select(c => $"#{c.Sequence} {c.Material.MaterialNo}")
                .ToList()
                );
        }

        #endregion

        #endregion

    }
}
