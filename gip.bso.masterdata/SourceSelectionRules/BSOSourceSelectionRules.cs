using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VD = gip.mes.datamodel;
using static gip.mes.datamodel.EntityObjectExtensionApp;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Businessobject (dialog) for selecting the sources (machines) from which the materials of a production
    /// order are provided (source selection rules).
    /// This BSO is a dialog that is invoked programmatically; it has no search list.
    /// Call the ShowDialogSelectSources(acClassWFID, partslistID, prodOrderPartslistID) method to open the
    /// source selection dialog. The dialog loads the possible source selections for the given workflow and
    /// partslist context into the RuleGroupList property (rule groups with their rule selections), the
    /// ReservationRuleGroupList property (rule groups with reservation), the AllItemsList property (all
    /// machines without rule group) and the NotDosableMaterialsList property (materials that cannot be dosed).
    /// The user selects or deselects the sources with the checkboxes in the dialog (IsSelected / IsSelectedAll).
    /// Confirm the dialog with the DlgSelectSourcesOk() method (OK button). It writes the selection to the
    /// configuration of the workflow nodes (excluded process modules rules), saves the changes in the database
    /// and closes the dialog. Closing the dialog with the cancel button (CloseTopDialog) discards the changes.
    /// The AnyNotDosableMaterial property is true when the NotDosableMaterialsList contains entries; the dialog
    /// highlights the corresponding tab in red in this case.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOSourceSelectionRules&amp;type=code on github
    /// to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Source selection rules'}de{'Quellenauswahlregeln'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true,
        Description = @"Businessobject (dialog) for selecting the sources (machines) from which the materials of a production order are provided (source selection rules).
        This BSO is a dialog that is invoked programmatically; it has no search list.
        Call the ShowDialogSelectSources(acClassWFID, partslistID, prodOrderPartslistID) method to open the source selection dialog. The dialog loads the possible source selections for the given workflow and partslist context into the RuleGroupList property (rule groups with their rule selections), the ReservationRuleGroupList property (rule groups with reservation), the AllItemsList property (all machines without rule group) and the NotDosableMaterialsList property (materials that cannot be dosed).
        The user selects or deselects the sources with the checkboxes in the dialog (IsSelected / IsSelectedAll).
        Confirm the dialog with the DlgSelectSourcesOk() method (OK button). It writes the selection to the configuration of the workflow nodes (excluded process modules rules), saves the changes in the database and closes the dialog. Closing the dialog with the cancel button (CloseTopDialog) discards the changes.
        The AnyNotDosableMaterial property is true when the NotDosableMaterialsList contains entries; the dialog highlights the corresponding tab in red in this case.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOSourceSelectionRules&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.")]
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

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            bool baseACDeinit = await base.ACDeInit(deleteACClassTask);

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

        #region Property -> RuleGroup

        public const string RuleGroup = "RuleGroup";

        private RuleGroup _SelectedRuleGroup;
        /// <summary>
        /// Selected property for EntityType
        /// </summary>
        /// <value>The selected RuleGroup</value>
        [ACPropertySelected(9999, nameof(RuleGroup), "en{'Rule group'}de{'Regelgruppe'}", Description = "Gets or sets the currently selected rule group of the possible source selections. Each rule group contains a list of rule selections (source machine, target, material) with a checkbox for selecting the source.")]
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
                    OnPropertyChanged();
                }
            }
        }


        private List<RuleGroup> _RuleGroupList;
        /// <summary>
        /// List property for EntityType
        /// </summary>
        /// <value>The RuleGroup list</value>
        [ACPropertyList(9999, nameof(RuleGroup), Description = "Gets the rule groups with their rule selections that are offered in the source selection dialog. Contains only rule groups that have at least one rule selection.")]
        public List<RuleGroup> RuleGroupList
        {
            get
            {
                return _RuleGroupList;
            }
        }

        private RuleSelection _CurrentRuleSelection;

        [ACPropertyInfo(102, "", Const.ACGroup, Description = "Gets or sets the currently selected rule selection within a rule group (bound by the nested data grid of the rule group list).")]
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
                    OnPropertyChanged();
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
        [ACPropertySelected(9999, "AllRules", "en{'All sources'}de{'Alle Quellen'}", Description = "Gets or sets the currently selected row of the All Sources tab: a machine with its material and the IsSelectedAll checkbox for machines that do not belong to a rule group.")]
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
                    OnPropertyChanged();
                }
            }
        }

        private List<MachineItem> _AllItemsList;
        /// <summary>
        /// List property for RuleItem
        /// </summary>
        /// <value>The AllItems list</value>
        [ACPropertyList(9999, "AllRules", Description = "Gets all machines with their materials that do not belong to a rule group, ordered by machine identifier (All Sources tab).")]
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
        [ACPropertySelected(9999, "NotDosableMaterials", "en{'Not dosable material'}de{'Nicht dosierbares Material'}", Description = "Gets or sets the currently selected material in the list of materials that cannot be dosed.")]
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
                    OnPropertyChanged();
                }
            }
        }

        private List<VD.Material> _NotDosableMaterialsList;
        /// <summary>
        /// List property for dbMes.Material
        /// </summary>
        /// <value>The NotDosableMaterials list</value>
        [ACPropertyList(9999, "NotDosableMaterials", Description = "Gets the materials of the production order that cannot be dosed. If this list contains entries, AnyNotDosableMaterial is true and the dialog highlights the corresponding tab in red.")]
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

        #region Properties -> ReservationRuleGroup

        public const string ReservationRuleGroup = "ReservationRuleGroup";

        private RuleGroup _SelectedReservationRuleGroup;
        /// <summary>
        /// Selected property for EntityType
        /// </summary>
        /// <value>The selected RuleGroup</value>
        [ACPropertySelected(9999, nameof(ReservationRuleGroup), "en{'Rule group'}de{'Regelgruppe'}", Description = "Gets or sets the currently selected rule group of the possible sources with reservation. Each rule group contains a list of rule selections (source machine, target, material) with a checkbox for selecting the source.")]
        public RuleGroup SelectedReservationRuleGroup
        {
            get
            {
                return _SelectedReservationRuleGroup;
            }
            set
            {
                if (_SelectedReservationRuleGroup != value)
                {
                    _SelectedReservationRuleGroup = value;
                    OnPropertyChanged();
                }
            }
        }


        private List<RuleGroup> _ReservationRuleGroupList;
        /// <summary>
        /// List property for EntityType
        /// </summary>
        /// <value>The RuleGroup list</value>
        [ACPropertyList(9999, nameof(ReservationRuleGroup), Description = "Gets the rule groups with reservation and their rule selections that are offered in the source selection dialog. Contains only rule groups that have at least one rule selection.")]
        public List<RuleGroup> ReservationRuleGroupList
        {
            get
            {
                return _ReservationRuleGroupList;
            }
        }

        #endregion

        #endregion

        #region ACMethod

        [ACMethodInfo("Dialog", "en{'Select Sources'}de{'Quellen auswählen'}", (short)MISort.QueryPreviewDlg, Description = "Opens the source selection dialog for the given workflow and partslist context. Loads the rule groups, the reservation rule groups, the machines without rule group and the not dosable materials from the PartslistManager and shows the dialog.")]
        public async Task ShowDialogSelectSources(Guid acClassWFID, Guid partslistID, Guid? prodOrderPartslistID)
        {
            AnyNotDosableMaterial = false;

            (WFGroupStartData wFGroupStartData, SourceSelectionRulesResult sourceSelectionRulesResult) = PartslistManager.GetSourceSelectionRulesResult(DatabaseApp, iPlusMESConfigManager, acClassWFID, partslistID, prodOrderPartslistID);

            _RuleGroupList = sourceSelectionRulesResult.RuleGroups.Where(c => c.RuleSelectionList.Any()).ToList();
            _NotDosableMaterialsList = sourceSelectionRulesResult.NotDosableMaterials;
            AnyNotDosableMaterial = _NotDosableMaterialsList != null && _NotDosableMaterialsList.Any();
            _AllItemsList = sourceSelectionRulesResult.MachineItems.Where(c => c.RuleGroup == null).OrderBy(c => c.Machine.ACIdentifier).ToList();
            _ReservationRuleGroupList = sourceSelectionRulesResult.ReservationRuleGroups.Where(c => c.RuleSelectionList.Any()).ToList();

            OnPropertyChanged(nameof(RuleGroupList));
            OnPropertyChanged(nameof(NotDosableMaterialsList));
            OnPropertyChanged(nameof(AllItemsList));
            OnPropertyChanged(nameof(ReservationRuleGroupList));

            await ShowDialogAsync(this, "DlgSelectSources");
        }

        /// <summary>
        /// Source Property: DlgSelectSourcesOk
        /// </summary>
        [ACMethodInfo("DlgSelectSourcesOk", Const.Ok, 500, Description = "Saves the source selection: writes the selected and deselected sources to the configuration of the workflow nodes (excluded process modules rules), saves the changes in the database and closes the dialog.")]
        public async Task DlgSelectSourcesOk()
        {
            if (!IsEnabledDlgSelectSourcesOk())
                return;
            await SaveSourceSelectionRules(DatabaseApp.ContextIPlus, DatabaseApp, _RuleGroupList, _AllItemsList);
            this.CloseTopDialog();
        }

        public bool IsEnabledDlgSelectSourcesOk()
        {
            return true;
        }

        #endregion

        #region Methods



        

    
        

        private async Task SaveSourceSelectionRules(Database database, VD.DatabaseApp databaseApp, List<RuleGroup> ruleGroupList, List<MachineItem> allMachineItems)
        {
            if (ruleGroupList != null)
            {
                bool change = false;

                List<ACClassMethod> visitedMethods = new List<ACClassMethod>();
                if (allMachineItems != null)
                {
                    foreach (MachineItem machineItem in allMachineItems)
                    {
                        bool tmpChange = WriteConfigToPWMNode(database, machineItem, visitedMethods);
                        change = change || tmpChange;
                    }
                }

                if (ruleGroupList != null)
                {
                    foreach (RuleGroup ruleGroup in ruleGroupList)
                    {
                        foreach (RuleSelection ruleSelection in ruleGroup.RuleSelectionList)
                        {
                            bool tmpChange = WriteConfigToPWMNode(database, ruleSelection.MachineItem, visitedMethods);
                            change = change || tmpChange;
                        }
                    }
                }


                if (change)
                {
                    MsgWithDetails msgWithDetails = databaseApp.ACSaveChanges();
                    if (msgWithDetails != null && !msgWithDetails.IsSucceded())
                    {
                        await Messages.MsgAsync(msgWithDetails);
                        databaseApp.ACUndoChanges();
                    }
                    else
                    {
                        if (visitedMethods != null && visitedMethods.Any())
                            ConfigManagerIPlus.ReloadConfigOnServerIfChanged(this, visitedMethods, database, true);
                        await OnSave();
                    }
                }
            }
        }

        private bool WriteConfigToPWMNode(Database database, MachineItem machineItem, List<ACClassMethod> visitedMethods)
        {
            bool change = false;
            foreach (ACClassWF aCClassWF in machineItem.PWNodes)
            {
                string localConfigACUrl = machineItem.GetConfigACUrl(aCClassWF);

                IACConfig currentStoreConfigItem = ACConfigHelper.GetStoreConfiguration(machineItem.SourceSelectionRulesResult.CurrentConfigStore.ConfigurationEntries, machineItem.PreConfigACUrl, localConfigACUrl, false, null);
                List<ACClass> excludedModules = PartslistManager.GetExcludedProcessModules(database, new List<IACConfigStore>() { machineItem.SourceSelectionRulesResult.CurrentConfigStore }, machineItem.PreConfigACUrl, aCClassWF);

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
                    if (!visitedMethods.Contains(aCClassWF.ACClassMethod))
                        visitedMethods.Add(aCClassWF.ACClassMethod);
                    RuleValue ruleValue = RulesCommand.RuleValueFromObjectList(ACClassWFRuleTypes.Excluded_process_modules, excludedModules);
                    RulesCommand.WriteIACConfig(database, currentStoreConfigItem, new List<RuleValue>() { ruleValue });
                }
            }

            return change;
        }

        #endregion

        #region Helper methods

       
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(ShowDialogSelectSources):
                    result = ShowDialogSelectSources((System.Guid)acParameter[0], (System.Guid)acParameter[1], (System.Nullable<System.Guid>)acParameter[2]);
                    return true;
                case nameof(DlgSelectSourcesOk):
                    result = DlgSelectSourcesOk();
                    return true;
                case nameof(IsEnabledDlgSelectSourcesOk):
                    result = IsEnabledDlgSelectSourcesOk();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(ShowDialogSelectSources):
                    return new string[] { nameof(InitState) };
                case nameof(DlgSelectSourcesOk):
                    return new string[] { nameof(InitState) };
                case nameof(IsEnabledDlgSelectSourcesOk):
                    return new string[] { nameof(InitState) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion
    }

}