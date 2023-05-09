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
        #region Quantity-Checks
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public MsgWithDetails Validation(Partslist partslist)
        {
            MsgWithDetails msg = null;
            msg = new MsgWithDetails();
            // msg.Message =  Root.Environment.TranslateMessage(this,"Error50042");

            // 1. Null field validation: PartslistNo, PartslistName, MaterialID, TargetQuantity
            // 2. Valid from to date validation
            ValidateInput(partslist, msg);

            // 3. Check is output quantity same as Declared in partslist
            ValidateFinalQuantity(partslist, msg);

            // 4. Check quantity of materials summation
            ValidateMaterialQuantity(partslist, msg);

            // 5. Validate intermediate quantity
            //ValidateIntermediateQuantity(partslist, msg);

            if (!msg.MsgDetails.Any())
                return null;
            return msg;
        }

        private void ValidateInput(Partslist partslist, MsgWithDetails msg)
        {
            //if (string.IsNullOrEmpty(partslist.PartslistNo))
            //    msg.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, "Error50035") });

            //if (string.IsNullOrEmpty(partslist.PartslistName))
            //    msg.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, "Error50036") });

            //if (partslist.MaterialID == Guid.Empty)
            //    msg.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, "Error50037") });

            //if (partslist.TargetQuantity <= 0)
            //    msg.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, "Error50038") });

            // Valid from to date validation
            if (partslist.EnabledFrom.HasValue && partslist.EnabledTo.HasValue)
            {
                if (partslist.EnabledFrom >= partslist.EnabledTo)
                    msg.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = Root.Environment.TranslateMessage(this, "Error50039", partslist.EnabledFrom, partslist.EnabledTo) });
            }
        }

        private void ValidateFinalQuantity(Partslist partslist, MsgWithDetails msg)
        {
            if (partslist.PartslistPos_Partslist.Where(x => x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.InwardIntern).Any())
            {
                var lastIntermediate = partslist
                    .PartslistPos_Partslist
                    .Where(x => x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.InwardIntern
                    &&
                    !x.PartslistPosRelation_SourcePartslistPos.Any()
                    ).FirstOrDefault();
                if (lastIntermediate != null && partslist.TargetQuantityUOM != lastIntermediate.TargetQuantityUOM)
                {
                    string quantityMessage =
                        Root.Environment.TranslateMessage(this, @"Error50040",
                        lastIntermediate.Material.MaterialName1,
                        lastIntermediate.TargetQuantity,
                        (lastIntermediate.MDUnit != null ? lastIntermediate.MDUnit.TechnicalSymbol : @"-"),
                        lastIntermediate.TargetQuantityUOM,
                        lastIntermediate.Material.BaseMDUnit.TechnicalSymbol,

                        partslist.TargetQuantity,
                        (partslist.MDUnit != null ? partslist.MDUnit.TechnicalSymbol : @"-"),
                        partslist.TargetQuantityUOM,
                        partslist.Material.BaseMDUnit.TechnicalSymbol);
                    msg.AddDetailMessage(new Msg() { MessageLevel = eMsgLevel.Warning, Message = quantityMessage });
                }
            }
        }

        private void ValidateMaterialQuantity(Partslist partslist, MsgWithDetails msg)
        {
            var positions = partslist.PartslistPos_Partslist.Where(x => x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.OutwardRoot && x.AlternativePartslistPosID == null);

            var mixures = partslist.PartslistPos_Partslist.Where(x => x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.InwardIntern);

            List<MaterialUsageCheck> listMaterialUsageCheck =
                positions
                .Select(x => x.Material)
                .Distinct()
                .ToList()
                .Select(x => new MaterialUsageCheck() { Material = x }).ToList();
            foreach (var item in listMaterialUsageCheck)
            {
                item.OutwardQuantityUOM = positions.Where(x => x.MaterialID == item.Material.MaterialID).Sum(x => x.TargetQuantityUOM);
                item.InwardQuantityUOM =
                    mixures
                    .ToList()
                    .SelectMany(x => x.PartslistPosRelation_TargetPartslistPos.Where(y => y.SourcePartslistPos.MaterialID == item.Material.MaterialID).Select(z => z.TargetQuantityUOM))
                    .Sum(x => x);
            }
            if (listMaterialUsageCheck.Where(x => !x.IsQuantityValid).Any())
            {
                List<MaterialUsageCheck> listInvalidUsedMaterial = listMaterialUsageCheck.Where(x => !x.IsQuantityValid).ToList();
                listInvalidUsedMaterial.ForEach(x =>
                {
                    Msg lMsg = new Msg();
                    lMsg.MessageLevel = eMsgLevel.Warning;
                    lMsg.Message = Root.Environment.TranslateMessage(this, @"Error50041",
                        x.Material, x.InwardQuantityUOM, x.OutwardQuantityUOM);
                    msg.AddDetailMessage(lMsg);
                });
            }
        }

        // TODO: @aagincic: ACParstslistManager -> ValidateIntermediateQuantity - this shuld be checked - maybe this shuld be reminder becouse editing partslist is not completed on first time
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partslist"></param>
        /// <param name="msg"></param>
        private void ValidateIntermediateQuantity(Partslist partslist, MsgWithDetails msg)
        {
            var mixures = partslist.PartslistPos_Partslist.Where(x => x.MaterialPosTypeIndex == (short)gip.mes.datamodel.GlobalApp.MaterialPosTypes.InwardIntern);
            foreach (var item in mixures)
            {
                double inputQuantity = item.PartslistPosRelation_TargetPartslistPos.Sum(x => x.TargetQuantityUOM);
                if (item.TargetQuantityUOM > inputQuantity)
                {
                    Msg lMsg = new Msg();
                    lMsg.MessageLevel = eMsgLevel.Error;
                    lMsg.Message = Root.Environment.TranslateMessage(this, @"Error50044",
                        item.Sequence, item.MaterialName, item.TargetQuantityUOM, inputQuantity);
                    msg.AddDetailMessage(lMsg);
                }
            }
        }

        #endregion

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
                                            continue;

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
                                                IList<Facility> possibleSilos;
                                                routes = GetRoutes(mat4Dosing, dbApp, dbIPlus, acClassPM, SearchMode.AllSilos, null, out possibleSilos, null, null);
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
    }
}
