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

        #region Property -> RuleGroup

        public const string RuleGroup = "RuleGroup";

        private RuleGroup _SelectedRuleGroup;
        /// <summary>
        /// Selected property for EntityType
        /// </summary>
        /// <value>The selected RuleGroup</value>
        [ACPropertySelected(9999, nameof(RuleGroup), "en{'TODO: RuleGroup'}de{'TODO: RuleGroup'}")]
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
        [ACPropertyList(9999, nameof(RuleGroup))]
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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

        #region Properties -> ReservationRuleGroup

        public const string ReservationRuleGroup = "ReservationRuleGroup";

        private RuleGroup _SelectedReservationRuleGroup;
        /// <summary>
        /// Selected property for EntityType
        /// </summary>
        /// <value>The selected RuleGroup</value>
        [ACPropertySelected(9999, nameof(ReservationRuleGroup), "en{'TODO: RuleGroup'}de{'TODO: RuleGroup'}")]
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
        [ACPropertyList(9999, nameof(ReservationRuleGroup))]
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

        [ACMethodInfo("Dialog", "en{'Select Sources'}de{'Quellen auswählen'}", (short)MISort.QueryPreviewDlg)]
        public void ShowDialogSelectSources(Guid acClassWFID, Guid partslistID, Guid? prodOrderPartslistID)
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



        

    
        

        private void SaveSourceSelectionRules(Database database, VD.DatabaseApp databaseApp, List<RuleGroup> ruleGroupList, List<MachineItem> allMachineItems)
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
                        Messages.Msg(msgWithDetails);
                        databaseApp.ACUndoChanges();
                    }
                    else
                    {
                        if (visitedMethods != null && visitedMethods.Any())
                            ConfigManagerIPlus.ReloadConfigOnServerIfChanged(this, visitedMethods, database, true);
                        OnSave();
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