using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using dbMes = gip.mes.datamodel;
using static gip.mes.datamodel.EntityObjectExtensionApp;
using System.Windows.Media.Media3D;
using System.ComponentModel.Design.Serialization;

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
        public IACConfigStore CurrentConfigStore { get; set; }

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

        private dbMes.Material _SelectedNotDosableMaterials;
        /// <summary>
        /// Selected property for dbMes.Material
        /// </summary>
        /// <value>The selected NotDosableMaterials</value>
        [ACPropertySelected(9999, "NotDosableMaterials", "en{'TODO: NotDosableMaterials'}de{'TODO: NotDosableMaterials'}")]
        public dbMes.Material SelectedNotDosableMaterials
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

        private List<dbMes.Material> _NotDosableMaterialsList;
        /// <summary>
        /// List property for dbMes.Material
        /// </summary>
        /// <value>The NotDosableMaterials list</value>
        [ACPropertyList(9999, "NotDosableMaterials")]
        public List<dbMes.Material> NotDosableMaterialsList
        {
            get
            {
                if (_NotDosableMaterialsList == null)
                    _NotDosableMaterialsList = new List<dbMes.Material>();
                return _NotDosableMaterialsList;
            }
        }

        #endregion

        #endregion

        #region ACMethod

        [ACMethodInfo("Dialog", "en{'Select Sources'}de{'Quellen auswählen'}", (short)MISort.QueryPreviewDlg)]
        public void ShowDialogSelectSources(Guid acClassWFID, Guid acClassMethodID, Guid partslistID, Guid? prodOrderPartslistID)
        {
            AnyNotDosableMaterial = false;

            (IACConfigStore currentConfigStore, SourceSelectionRulesResult result) = DoShowDialogSelectSources(DatabaseApp, acClassWFID, acClassMethodID, partslistID, prodOrderPartslistID);

            _RuleGroupList = result.RuleGroups.Where(c => c.RuleSelectionList.Any()).ToList();
            OnPropertyChanged(nameof(RuleGroupList));

            _NotDosableMaterialsList = result.NotDosableMaterials;
            OnPropertyChanged(nameof(NotDosableMaterialsList));
            AnyNotDosableMaterial = _NotDosableMaterialsList != null && _NotDosableMaterialsList.Any();

            _AllItemsList = result.MachineItems.Where(c => c.RuleGroup == null).OrderBy(c => c.Machine.ACIdentifier).ToList();
            OnPropertyChanged(nameof(AllItemsList));

            CurrentConfigStore = currentConfigStore;
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

        private Tuple<IACConfigStore, SourceSelectionRulesResult> DoShowDialogSelectSources(dbMes.DatabaseApp databaseApp, Guid acClassWFID, Guid acClassMethodID, Guid partslistID, Guid? prodOrderPartslistID)
        {
            ACClassMethod method = databaseApp.ContextIPlus.ACClassMethod.FirstOrDefault(c => c.ACClassMethodID == acClassMethodID);
            dbMes.Partslist partslist = databaseApp.Partslist.FirstOrDefault(c => c.PartslistID == partslistID);
            dbMes.ProdOrderPartslist prodOrderPartslist = null;
            ACClassWF invokerPWNode = databaseApp.ContextIPlus.ACClassWF.Where(c => c.ACClassWFID == acClassWFID).FirstOrDefault();
            if (prodOrderPartslistID != null)
            {
                prodOrderPartslist = databaseApp.ProdOrderPartslist.FirstOrDefault(c => c.ProdOrderPartslistID == prodOrderPartslistID);
            }

            (IACConfigStore currentConfigStore, List<IACConfigStore> configStores) = GetConfigStores(new ACClassMethod[] { method, invokerPWNode.RefPAACClassMethod }, partslist, prodOrderPartslist);

            SourceSelectionRulesResult sourceSelectionRulesResult = LoadRuleGroupList(databaseApp.ContextIPlus, DatabaseApp, configStores, invokerPWNode, partslist);
            return new Tuple<IACConfigStore, SourceSelectionRulesResult>(currentConfigStore, sourceSelectionRulesResult);
        }

        private Tuple<IACConfigStore, List<IACConfigStore>> GetConfigStores(ACClassMethod[] aCClassMethods, dbMes.Partslist partslist, dbMes.ProdOrderPartslist prodOrderPartslist)
        {
            List<IACConfigStore> configStores = new List<IACConfigStore>();
            IACConfigStore currentConfigStore = partslist;

            if (prodOrderPartslist != null)
            {
                configStores.Add(prodOrderPartslist);
                currentConfigStore = prodOrderPartslist;
            }
            else
            {
                configStores.Add(partslist);
            }

            configStores.AddRange(aCClassMethods);

            configStores = iPlusMESConfigManager.GetACConfigStores(configStores);

            return new Tuple<IACConfigStore, List<IACConfigStore>>(currentConfigStore, configStores);
        }

        private SourceSelectionRulesResult LoadRuleGroupList(Database database, dbMes.DatabaseApp databaseApp, List<IACConfigStore> configStores, ACClassWF invokerPWNode, dbMes.Partslist partslist)
        {
            SourceSelectionRulesResult result = new SourceSelectionRulesResult();

            MsgWithDetails validationMessage = new MsgWithDetails();
            PartslistValidationInfo partslistValidationInfo = PartslistManager.CheckResourcesAndRouting(databaseApp, Database.ContextIPlus, partslist, configStores, PARole.ValidationBehaviour.Laxly, validationMessage, invokerPWNode, true);

            MapPosToWFConn mapPosToWFConn = partslistValidationInfo.MapPosToWFConnections.FirstOrDefault();
            if (mapPosToWFConn != null)
            {
                var groupDosingItems = mapPosToWFConn.MapPosToWFConnSubItems.GroupBy(c => c.PWNode.RefPAACClass);

                foreach (var groupItem in groupDosingItems)
                {
                    List<MapPosToWFConnSubItem> mapPosToWFConnSubs = groupItem.ToList();

                    RuleGroup ruleGroup = new RuleGroup(result, groupItem.Key);

                    foreach (MapPosToWFConnSubItem mapPosToWFConnSub in mapPosToWFConnSubs)
                    {
                        string preConfigACUrl = invokerPWNode.ConfigACUrl;
                        if (!preConfigACUrl.EndsWith("\\"))
                        {
                            preConfigACUrl = preConfigACUrl + "\\";
                        }


                        List<ACClass> excludedProcessModules = GetExcludedProcessModules(database, configStores, preConfigACUrl, mapPosToWFConnSub.PWNode);

                        foreach (KeyValuePair<dbMes.Material, List<Route>> mat4DosingAndRoutes in mapPosToWFConnSub.Mat4DosingAndRoutes)
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
                                        ruleSelection.MachineItem.IsSelected = ruleSelection.MachineItem.IsSelected || !excludedProcessModules.Contains(source.Source);
                                    }
                                }
                            }
                            else
                            {
                                result.AddNotDosableMaterial(mat4DosingAndRoutes.Key);
                            }
                        }

                        bool? help = false;
                        List<ACClass> allProcessModules = RulesCommand.GetProcessModules(mapPosToWFConnSub.PWNode, database, out help)?.Item1?.ToList();
                        result.FillMachineItems(allProcessModules, excludedProcessModules, preConfigACUrl, mapPosToWFConnSub.PWNode);
                    }
                }
            }

            foreach (RuleGroup ruleGroup in result.RuleGroups)
            {
                ruleGroup.RuleSelectionList = ruleGroup.RuleSelectionList.OrderBy(c => c.MachineItem.Machine.ACIdentifier).ThenBy(c => c.Target.ACIdentifier).ToList();
            }

            List<dbMes.Facility> facilities = databaseApp.Facility.Where(c => c.MaterialID != null && c.VBiFacilityACClassID != null).ToList();
            foreach (dbMes.Facility facility in facilities)
            {
                List<MachineItem> matchedMachineItems = result.MachineItems.Where(c => c.Machine.ACClassID == facility.VBiFacilityACClassID).ToList();
                foreach (MachineItem machineItem in matchedMachineItems)
                {
                    machineItem.Material = facility.Material;
                }
            }

            result.NotDosableMaterials = result.NotDosableMaterials.Where(c => !result.DosableMaterials.Select(x => x.MaterialNo).Contains(c.MaterialNo)).ToList();

            return result;
        }

        private void SaveSourceSelectionRules(Database database, dbMes.DatabaseApp databaseApp, List<RuleGroup> ruleGroupList, List<MachineItem> allMachineItems)
        {
            if (ruleGroupList != null)
            {
                bool change = false;

                if (allMachineItems != null)
                {
                    foreach (MachineItem machineItem in allMachineItems)
                    {
                        change = change || WriteConfigToPWMNode(database, machineItem);
                    }
                }

                if (ruleGroupList != null)
                {
                    foreach (RuleGroup ruleGroup in ruleGroupList)
                    {
                        foreach (RuleSelection ruleSelection in ruleGroup.RuleSelectionList)
                        {
                            change = change || WriteConfigToPWMNode(database, ruleSelection.MachineItem);
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
                }
            }
        }

        private bool WriteConfigToPWMNode(Database database, MachineItem machineItem)
        {
            bool change = false;
            foreach (ACClassWF aCClassWF in machineItem.PWNodes)
            {
                IACConfig currentStoreConfigItem = ACConfigHelper.GetStoreConfiguration(CurrentConfigStore.ConfigurationEntries, machineItem.PreConfigACUrl, machineItem.GetConfigACUrl(aCClassWF), false, null);
                List<ACClass> excludedModules = GetExcludedProcessModules(database, new List<IACConfigStore>() { CurrentConfigStore }, machineItem.PreConfigACUrl, aCClassWF);

                if (machineItem.IsSelected)
                {

                    if (excludedModules.Contains(machineItem.Machine))
                    {
                        excludedModules.Remove(machineItem.Machine);
                        if (excludedModules.Any())
                        {
                            RuleValue ruleValue = RulesCommand.RuleValueFromObjectList(ACClassWFRuleTypes.Excluded_process_modules, excludedModules);
                            RulesCommand.WriteIACConfig(database, currentStoreConfigItem, new List<RuleValue>() { ruleValue });
                        }
                    }

                    if (currentStoreConfigItem != null && !excludedModules.Any())
                    {
                        CurrentConfigStore.RemoveACConfig(currentStoreConfigItem);
                        change = true;
                    }

                }
                else
                {

                    if (currentStoreConfigItem == null)
                    {
                        currentStoreConfigItem = CurrentConfigStore.NewACConfig(aCClassWF);
                        currentStoreConfigItem.PreConfigACUrl = machineItem.PreConfigACUrl;
                        currentStoreConfigItem.LocalConfigACUrl = machineItem.GetConfigACUrl(aCClassWF);
                        change = true;
                    }

                    if (!excludedModules.Contains(machineItem.Machine))
                    {
                        excludedModules.Add(machineItem.Machine);
                        change = true;
                    }

                    if (change)
                    {
                        RuleValue ruleValue = RulesCommand.RuleValueFromObjectList(ACClassWFRuleTypes.Excluded_process_modules, excludedModules);
                        RulesCommand.WriteIACConfig(database, currentStoreConfigItem, new List<RuleValue>() { ruleValue });
                    }

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
                    ShowDialogSelectSources((System.Guid)acParameter[0], (System.Guid)acParameter[1], (System.Guid)acParameter[2], (System.Nullable<System.Guid>)acParameter[3]);
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
