using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using VD = gip.mes.datamodel;
using static gip.mes.datamodel.EntityObjectExtensionApp;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'BSOSourceSelectionRules'}de{'BSOSourceSelectionRules'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOSourceSelectionRules : ACBSOvb
    {
        #region ctor's
        public const string ACURL = @"\Businessobjects\BSOSourceSelectionRules";
        #endregion

        #region ctor's

        public BSOSourceSelectionRules(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
           base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseACInit = base.ACInit(startChildMode);

            _PartslistManager = ACPartslistManager.ACRefToServiceInstance(this);
            if (_PartslistManager == null)
                throw new Exception("PartslistManager not configured");

            _iPlusMESConfigManager = ConfigManagerIPlus.ACRefToServiceInstance(this);

            return baseACInit;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            bool baseACDeinit = base.ACDeInit(deleteACClassTask);

            if (_PartslistManager != null)
                ACPartslistManager.DetachACRefFromServiceInstance(this, _PartslistManager);
            _PartslistManager = null;

            if (_iPlusMESConfigManager != null)
                ConfigManagerIPlus.DetachACRefFromServiceInstance(this, _iPlusMESConfigManager);
            _iPlusMESConfigManager = null;

            return baseACDeinit;
        }
        #endregion

        #region Managers

        protected ACRef<ACPartslistManager> _PartslistManager = null;
        protected ACPartslistManager PartslistManager
        {
            get
            {
                if (_PartslistManager == null)
                    return null;
                return _PartslistManager.ValueT;
            }
        }

        protected ACRef<ConfigManagerIPlus> _iPlusMESConfigManager = null;
        public ConfigManagerIPlus iPlusMESConfigManager
        {
            get
            {
                if (_iPlusMESConfigManager == null)
                    return null;
                return _iPlusMESConfigManager.ValueT;
            }
        }

        #endregion

        #region Property

        public bool AnyNotDosableMaterial { get; set; } = false;

        private Type _TypeOfPWNodeProcessWorkflow;
        protected Type TypeOfPWNodeProcessWorkflow
        {
            get
            {
                if (_TypeOfPWNodeProcessWorkflow == null)
                    _TypeOfPWNodeProcessWorkflow = typeof(PWNodeProcessWorkflow);
                return _TypeOfPWNodeProcessWorkflow;
            }
        }

        #region Property -> RuleGroup

        private RuleGroup _SelectedRuleGroup;
        /// <summary>
        /// Selected property for EntityType
        /// </summary>
        /// <value>The selected RuleGroup</value>
        [ACPropertySelected(9999, "RuleGroup", "en{'TODO: RuleGroup'}de{'TODO: RuleGroup'}")]
        public RuleGroup SelectedRuleGroup
        {
            get
            {
                return _SelectedRuleGroup;
            }
            set
            {
                if (_SelectedRuleGroup != value)
                {
                    _SelectedRuleGroup = value;
                    OnPropertyChanged(nameof(SelectedRuleGroup));
                }
            }
        }


        private List<RuleGroup> _RuleGroupList;
        /// <summary>
        /// List property for EntityType
        /// </summary>
        /// <value>The RuleGroup list</value>
        [ACPropertyList(9999, "RuleGroup")]
        public List<RuleGroup> RuleGroupList
        {
            get
            {
                return _RuleGroupList;
            }
        }

        private RuleSelection _CurrentRuleSelection;

        [ACPropertyInfo(102, "", Const.ACGroup)]
        public RuleSelection CurrentRuleSelection

        {
            get
            {
                return _CurrentRuleSelection;
            }
            set
            {
                if (_CurrentRuleSelection != value)
                {
                    _CurrentRuleSelection = value;
                    OnPropertyChanged(nameof(CurrentRuleSelection));
                }
            }
        }

        #endregion

        #region Property -> AllRules

        private MachineItem _SelectedAllItems;
        /// <summary>
        /// Selected property for RuleItem
        /// </summary>
        /// <value>The selected AllItems</value>
        [ACPropertySelected(9999, "AllRules", "en{'TODO: AllItems'}de{'TODO: AllItems'}")]
        public MachineItem SelectedAllItems
        {
            get
            {
                return _SelectedAllItems;
            }
            set
            {
                if (_SelectedAllItems != value)
                {
                    _SelectedAllItems = value;
                    OnPropertyChanged(nameof(SelectedAllItems));
                }
            }
        }

        private List<MachineItem> _AllItemsList;
        /// <summary>
        /// List property for RuleItem
        /// </summary>
        /// <value>The AllItems list</value>
        [ACPropertyList(9999, "AllRules")]
        public List<MachineItem> AllItemsList
        {
            get
            {
                if (_AllItemsList == null)
                    _AllItemsList = new List<MachineItem>();
                return _AllItemsList;
            }
        }

        #endregion

        #region Properties -> NotDosableMaterials

        private VD.Material _SelectedNotDosableMaterials;
        /// <summary>
        /// Selected property for dbMes.Material
        /// </summary>
        /// <value>The selected NotDosableMaterials</value>
        [ACPropertySelected(9999, "NotDosableMaterials", "en{'TODO: NotDosableMaterials'}de{'TODO: NotDosableMaterials'}")]
        public VD.Material SelectedNotDosableMaterials
        {
            get
            {
                return _SelectedNotDosableMaterials;
            }
            set
            {
                if (_SelectedNotDosableMaterials != value)
                {
                    _SelectedNotDosableMaterials = value;
                    OnPropertyChanged(nameof(SelectedNotDosableMaterials));
                }
            }
        }

        private List<VD.Material> _NotDosableMaterialsList;
        /// <summary>
        /// List property for dbMes.Material
        /// </summary>
        /// <value>The NotDosableMaterials list</value>
        [ACPropertyList(9999, "NotDosableMaterials")]
        public List<VD.Material> NotDosableMaterialsList
        {
            get
            {
                if (_NotDosableMaterialsList == null)
                    _NotDosableMaterialsList = new List<VD.Material>();
                return _NotDosableMaterialsList;
            }
        }

        #endregion

        #endregion

        #region ACMethod

        [ACMethodInfo("Dialog", "en{'Select Sources'}de{'Quellen auswählen'}", (short)MISort.QueryPreviewDlg)]
        public void ShowDialogSelectSources(Guid acClassWFID, Guid partslistID, Guid? prodOrderPartslistID)
        {
            AnyNotDosableMaterial = false;

            SourceSelectionRulesResult result = DoShowDialogSelectSources(DatabaseApp, acClassWFID, partslistID, prodOrderPartslistID);

            _RuleGroupList = result.RuleGroups.Where(c => c.RuleSelectionList.Any()).ToList();
            _NotDosableMaterialsList = result.NotDosableMaterials;
            AnyNotDosableMaterial = _NotDosableMaterialsList != null && _NotDosableMaterialsList.Any();
            _AllItemsList = result.MachineItems.Where(c => c.RuleGroup == null).OrderBy(c => c.Machine.ACIdentifier).ToList();

            OnPropertyChanged(nameof(RuleGroupList));
            OnPropertyChanged(nameof(NotDosableMaterialsList));
            OnPropertyChanged(nameof(AllItemsList));

            ShowDialog(this, "DlgSelectSources");
        }

        /// <summary>
        /// Source Property: DlgSelectSourcesOk
        /// </summary>
        [ACMethodInfo("DlgSelectSourcesOk", Const.Ok, 500)]
        public void DlgSelectSourcesOk()
        {
            if (!IsEnabledDlgSelectSourcesOk())
                return;
            SaveSourceSelectionRules(DatabaseApp.ContextIPlus, DatabaseApp, _RuleGroupList, _AllItemsList);
            this.CloseTopDialog();
        }

        public bool IsEnabledDlgSelectSourcesOk()
        {
            return true;
        }

        #endregion

        #region Methods

        

        private SourceSelectionRulesResult DoShowDialogSelectSources(VD.DatabaseApp databaseApp, Guid acClassWFID, Guid partslistID, Guid? prodOrderPartslistID)
        {
            WFGroupStartData wFGroupStartData = new WFGroupStartData(databaseApp, iPlusMESConfigManager, acClassWFID, partslistID, prodOrderPartslistID, null);
            
            SourceSelectionRulesResult sourceSelectionRulesResult = new SourceSelectionRulesResult();
            LoadRuleGroupList(databaseApp.ContextIPlus, DatabaseApp, sourceSelectionRulesResult, wFGroupStartData.ConfigStores, wFGroupStartData.Partslist, wFGroupStartData.InvokerPWNode);
            sourceSelectionRulesResult.CurrentConfigStore = GetCurrentConfigStore(wFGroupStartData.Partslist, wFGroupStartData.ProdOrderPartslist, null);

            // Load Subs (level 1)
            List<ACClassWF> allSubWf = wFGroupStartData.InvokerPWNode.RefPAACClassMethod.ACClassWF_ACClassMethod.ToList();
            List<ACClassWF> filteredSubs = new List<ACClassWF>();
            foreach (ACClassWF subWf in allSubWf)
            {
                Type nodeType = subWf.PWACClass?.ObjectType;
                if (nodeType != null && TypeOfPWNodeProcessWorkflow.IsAssignableFrom(nodeType))
                    filteredSubs.Add(subWf);
            }

            if (filteredSubs.Any())
            {
                string preConfigACUrl = wFGroupStartData.InvokerPWNode.ConfigACUrl;
                if (!preConfigACUrl.EndsWith("\\"))
                {
                    preConfigACUrl = preConfigACUrl + "\\";
                }
                foreach (ACClassWF subWf in filteredSubs)
                {
                    List<IACConfigStore> subConfigStores = GetConfigStores(new ACClassMethod[] { wFGroupStartData.Method, subWf.RefPAACClassMethod }, wFGroupStartData.Partslist, wFGroupStartData.ProdOrderPartslist);
                    LoadRuleGroupList(databaseApp.ContextIPlus, DatabaseApp, sourceSelectionRulesResult, subConfigStores, wFGroupStartData.Partslist, subWf, preConfigACUrl);
                }
            }

            return sourceSelectionRulesResult;
        }

        private List<IACConfigStore> GetConfigStores(ACClassMethod[] aCClassMethods, VD.Partslist partslist, VD.ProdOrderPartslist prodOrderPartslist)
        {
            List<IACConfigStore> configStores = new List<IACConfigStore>();

            if (prodOrderPartslist != null)
            {
                configStores.Add(prodOrderPartslist);
            }
            else
            {
                configStores.Add(partslist);
            }

            configStores.AddRange(aCClassMethods);

            configStores = iPlusMESConfigManager.GetACConfigStores(configStores);

            foreach (IACConfigStore configStore in configStores)
            {
                configStore.ClearCacheOfConfigurationEntries();
            }

            return configStores;
        }

        private IACConfigStore GetCurrentConfigStore(VD.Partslist partslist, VD.ProdOrderPartslist prodOrderPartslist, VD.Picking picking)
        {
            IACConfigStore currentConfigStore = partslist;

            if (prodOrderPartslist != null)
            {
                currentConfigStore = prodOrderPartslist;
            }

            if (picking != null)
            {
                currentConfigStore = picking;
            }

            return currentConfigStore;
        }

        private void LoadRuleGroupList(Database database, VD.DatabaseApp databaseApp, SourceSelectionRulesResult result, List<IACConfigStore> configStores, VD.Partslist partslist, ACClassWF invokerPWNode, string outPreConfigACUrl = null)
        {

            MsgWithDetails validationMessage = new MsgWithDetails();
            PartslistValidationInfo partslistValidationInfo = PartslistManager.CheckResourcesAndRouting(databaseApp, Database.ContextIPlus, partslist, configStores, PARole.ValidationBehaviour.Laxly, validationMessage, invokerPWNode, true);

            MapPosToWFConn mapPosToWFConn = partslistValidationInfo.MapPosToWFConnections.FirstOrDefault();
            if (mapPosToWFConn != null)
            {
                var groupDosingItems = mapPosToWFConn.MapPosToWFConnSubItems.GroupBy(c => c.PWNode.RefPAACClass);

                foreach (var groupItem in groupDosingItems)
                {
                    List<MapPosToWFConnSubItem> mapPosToWFConnSubs = groupItem.ToList();
                    RuleGroup ruleGroup = result.RuleGroups.FirstOrDefault(c => c.RefPAACClass.ACClassID == groupItem.Key.ACClassID);
                    if (ruleGroup == null)
                    {
                        ruleGroup = new RuleGroup(result, groupItem.Key);
                    }

                    string preConfigACUrl = invokerPWNode.ConfigACUrl;
                    if (!preConfigACUrl.EndsWith("\\"))
                    {
                        preConfigACUrl = preConfigACUrl + "\\";
                    }
                    if (!string.IsNullOrEmpty(outPreConfigACUrl))
                    {
                        preConfigACUrl = outPreConfigACUrl + preConfigACUrl;
                    }

                    foreach (MapPosToWFConnSubItem mapPosToWFConnSub in mapPosToWFConnSubs)
                    {

                        List<ACClass> excludedProcessModules = GetExcludedProcessModules(database, configStores, preConfigACUrl, mapPosToWFConnSub.PWNode);

                        foreach (KeyValuePair<VD.Material, List<Route>> mat4DosingAndRoutes in mapPosToWFConnSub.Mat4DosingAndRoutes)
                        {
                            if (mat4DosingAndRoutes.Value.Any())
                            {
                                result.AddDosableMaterial(mat4DosingAndRoutes.Key);
                                foreach (Route route in mat4DosingAndRoutes.Value)
                                {
                                    if (route != null)
                                    {
                                        RouteItem source = route.GetRouteSource();
                                        RouteItem target = route.GetRouteTarget();

                                        RuleSelection ruleSelection = ruleGroup.AddRuleSelection(mapPosToWFConnSub.PWNode, mat4DosingAndRoutes.Key, source.Source, target.Target, preConfigACUrl);
                                        // IsSelected == true - if auf any PWNode is not in excludedProcessModules
                                        ruleSelection.MachineItem._IsSelected = !excludedProcessModules.Select(c => c.ACClassID).Contains(source.Source.ACClassID) && ruleSelection.MachineItem.IsSelected;
                                    }
                                }
                            }
                            else
                            {
                                result.AddNotDosableMaterial(mat4DosingAndRoutes.Key);
                            }
                        }

                        bool? help = false;
                        List<ACClass> allProcessModules = RulesCommand.GetProcessModules(mapPosToWFConnSub.PWNode, database, out help, null, RouteResultMode.ShortRoute)?.Item1?.ToList();
                        result.FillMachineItems(allProcessModules, excludedProcessModules, preConfigACUrl, mapPosToWFConnSub.PWNode);
                    }
                }
            }

            foreach (RuleGroup ruleGroup in result.RuleGroups)
            {
                ruleGroup.RuleSelectionList = ruleGroup.RuleSelectionList.OrderBy(c => c.MachineItem.Machine.ACIdentifier).ThenBy(c => c.Target.ACIdentifier).ToList();
            }

            List<VD.Facility> facilities = databaseApp.Facility.Where(c => c.MaterialID != null && c.VBiFacilityACClassID != null).ToList();
            foreach (VD.Facility facility in facilities)
            {
                List<MachineItem> matchedMachineItems = result.MachineItems.Where(c => c.Machine.ACClassID == facility.VBiFacilityACClassID).ToList();
                foreach (MachineItem machineItem in matchedMachineItems)
                {
                    machineItem.Material = facility.Material;
                }
            }

            result.NotDosableMaterials = result.NotDosableMaterials.Where(c => !result.DosableMaterials.Select(x => x.MaterialNo).Contains(c.MaterialNo)).ToList();

        }

        private void SaveSourceSelectionRules(Database database, VD.DatabaseApp databaseApp, List<RuleGroup> ruleGroupList, List<MachineItem> allMachineItems)
        {
            if (ruleGroupList != null)
            {
                bool change = false;

                if (allMachineItems != null)
                {
                    foreach (MachineItem machineItem in allMachineItems)
                    {
                        bool tmpChange = WriteConfigToPWMNode(database, machineItem);
                        change = change || tmpChange;
                    }
                }

                if (ruleGroupList != null)
                {
                    foreach (RuleGroup ruleGroup in ruleGroupList)
                    {
                        foreach (RuleSelection ruleSelection in ruleGroup.RuleSelectionList)
                        {
                            bool tmpChange = WriteConfigToPWMNode(database, ruleSelection.MachineItem);
                            change = change || tmpChange;
                        }
                    }
                }


                if (change)
                {
                    MsgWithDetails msgWithDetails = databaseApp.ACSaveChanges();
                    if (msgWithDetails != null && !msgWithDetails.IsSucceded())
                    {
                        Messages.Msg(msgWithDetails);
                        databaseApp.ACUndoChanges();
                    }
                    else
                    {
                        OnSave();
                    }
                }
            }
        }

        private bool WriteConfigToPWMNode(Database database, MachineItem machineItem)
        {
            bool change = false;
            foreach (ACClassWF aCClassWF in machineItem.PWNodes)
            {
                string localConfigACUrl = machineItem.GetConfigACUrl(aCClassWF);

                IACConfig currentStoreConfigItem = ACConfigHelper.GetStoreConfiguration(machineItem.SourceSelectionRulesResult.CurrentConfigStore.ConfigurationEntries, machineItem.PreConfigACUrl, localConfigACUrl, false, null);
                List<ACClass> excludedModules = GetExcludedProcessModules(database, new List<IACConfigStore>() { machineItem.SourceSelectionRulesResult.CurrentConfigStore }, machineItem.PreConfigACUrl, aCClassWF);

                bool? isSelected = null;
                if (machineItem.RuleGroup == null)
                {
                    isSelected = machineItem.IsSelectedAll;
                }
                else
                {
                    isSelected = machineItem.IsSelected;
                }

                if (isSelected == null)
                {
                    // do nothing
                }
                else if (isSelected ?? false)
                {

                    if (excludedModules.Select(c => c.ACClassID).Contains(machineItem.Machine.ACClassID))
                    {
                        excludedModules.RemoveAll(c => c.ACClassID == machineItem.Machine.ACClassID);
                        if (excludedModules.Any())
                        {
                            change = true;
                        }
                    }

                    if (currentStoreConfigItem != null && !excludedModules.Any())
                    {
                        machineItem.SourceSelectionRulesResult.CurrentConfigStore.RemoveACConfig(currentStoreConfigItem);
                        change = true;
                    }
                }
                else
                {
                    if (currentStoreConfigItem == null)
                    {
                        currentStoreConfigItem = machineItem.SourceSelectionRulesResult.CurrentConfigStore.NewACConfig(aCClassWF);
                        currentStoreConfigItem.PreConfigACUrl = machineItem.PreConfigACUrl;
                        currentStoreConfigItem.LocalConfigACUrl = localConfigACUrl;
                        change = true;
                    }

                    if (!excludedModules.Select(c => c.ACClassID).Contains(machineItem.Machine.ACClassID))
                    {
                        excludedModules.Add(machineItem.Machine);
                        change = true;
                    }
                }

                if (change && excludedModules.Any())
                {
                    RuleValue ruleValue = RulesCommand.RuleValueFromObjectList(ACClassWFRuleTypes.Excluded_process_modules, excludedModules);
                    RulesCommand.WriteIACConfig(database, currentStoreConfigItem, new List<RuleValue>() { ruleValue });
                }
            }

            return change;
        }

        #endregion

        #region Helper methods

        private List<ACClass> GetExcludedProcessModules(Database database, List<IACConfigStore> configStores, string preConfigACUrl, ACClassWF aCClassWF)
        {
            List<ACClass> result = new List<ACClass>();
            ConfigManagerIPlus serviceInstance = ConfigManagerIPlus.GetServiceInstance(this);
            string configACUrl = aCClassWF.ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Excluded_process_modules.ToString();
            RuleValueList ruleValueList = serviceInstance.GetRuleValueList(configStores, preConfigACUrl, configACUrl);
            if (ruleValueList != null)
            {
                result = ruleValueList.GetSelectedClasses(ACClassWFRuleTypes.Excluded_process_modules, database).ToList();
            }
            return result ?? new List<ACClass>();
        }
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(ShowDialogSelectSources):
                    ShowDialogSelectSources((System.Guid)acParameter[0], (System.Guid)acParameter[1], (System.Nullable<System.Guid>)acParameter[2]);
                    return true;
                case nameof(DlgSelectSourcesOk):
                    DlgSelectSourcesOk();
                    return true;
                case nameof(IsEnabledDlgSelectSourcesOk):
                    result = IsEnabledDlgSelectSourcesOk();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }

}