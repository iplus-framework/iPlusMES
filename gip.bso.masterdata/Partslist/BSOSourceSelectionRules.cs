using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using dbMes = gip.mes.datamodel;
using static gip.mes.datamodel.EntityObjectExtensionApp;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'BSOSourceSelectionRules'}de{'BSOSourceSelectionRules'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOSourceSelectionRules : ACBSOvb
    {

        #region ctor's

        public BSOSourceSelectionRules(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
           base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Method

        [ACMethodInfo("Dialog", "en{'Select Sources'}de{'Quellen auswählen'}", (short)MISort.QueryPreviewDlg)]
        public void ShowDialogSelectSources(Guid acClassWFID, Guid partslistID)
        {
            Console.WriteLine(acClassWFID.ToString());
            BSOPartslist partslist = ParentACComponent as BSOPartslist;
            List<IACConfigStore> configStores = partslist.MandatoryConfigStores;
            _RuleGroupList = LoadRuleGroupList(DatabaseApp.ContextIPlus, DatabaseApp, configStores, acClassWFID, partslistID);
            foreach (RuleGroup ruleGroup in _RuleGroupList)
            {
                ruleGroup.PropertyChanged += RuleGroup_PropertyChanged;
            }
            ShowDialog(this, "DlgSelectSources");
        }

        private void RuleGroup_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RuleGroup.CurrentRuleSelection))
            {
                RuleGroup ruleGroup = sender as RuleGroup;
                if (ruleGroup != null)
                {
                    CurrentRuleSelection = ruleGroup.CurrentRuleSelection;
                }
            }
        }

        /// <summary>
        /// Source Property: DlgSelectSourcesOk
        /// </summary>
        [ACMethodInfo("DlgSelectSourcesOk", Const.Ok, 500)]
        public void DlgSelectSourcesOk()
        {
            if (!IsEnabledDlgSelectSourcesOk())
                return;
            SaveSourceSelectionRules(_RuleGroupList);
            this.CloseTopDialog();
        }



        public bool IsEnabledDlgSelectSourcesOk()
        {
            return true;
        }

        #endregion

        #region RuleGroup

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
                if (_CurrentRuleSelection != null)
                {
                    _CurrentRuleSelection.PropagateRuleSelection = false;
                }
                if (_CurrentRuleSelection != value)
                {
                    _CurrentRuleSelection = value;
                    OnPropertyChanged(nameof(CurrentRuleSelection));
                    _CurrentRuleSelection.PropagateRuleSelection = true;
                }
            }
        }

        #endregion

        #region Others

        private List<RuleGroup> LoadRuleGroupList(Database database, dbMes.DatabaseApp databaseApp, List<IACConfigStore> configStores, Guid acClassWFID, Guid partslistID)
        {
            Tuple<ACClassWF, List<ACClassWF>> dischargingResult = GetAllDischargingItems(database, databaseApp, acClassWFID, partslistID);
            List<RuleGroup> grouped = GroupDischargingItems(database, databaseApp, configStores, dischargingResult.Item1, dischargingResult.Item2);
            return grouped;
        }
        public Tuple<ACClassWF, List<ACClassWF>> GetAllDischargingItems(Database database, dbMes.DatabaseApp databaseApp, Guid acClassWFID, Guid partslistID)
        {
            Type typeReceiveMat = typeof(IPWNodeReceiveMaterialRouteable);
            ACClassWF wf = database.ACClassWF.Where(c => c.ACClassWFID == acClassWFID).FirstOrDefault();
            ACClassMethod refMth = wf.RefPAACClassMethod;

            List<ACClassWF> dischargingItems =
                refMth
                .ACClassWF_ACClassMethod
                .AsEnumerable()
                .Where(c => typeReceiveMat.IsAssignableFrom(c.PWACClass.ObjectType))
                .ToList();

            dbMes.Partslist pll = databaseApp.Partslist.FirstOrDefault(c => c.PartslistID == partslistID);
            Guid[] connectedACClassWFIDs =
                pll
                .MaterialWF
                .MaterialWFACClassMethod_MaterialWF
                .SelectMany(c => c.MaterialWFConnection_MaterialWFACClassMethod)
                .Select(c => c.ACClassWFID)
                .Distinct()
                .ToArray();

            List<ACClassWF> filteredDischargingItems = dischargingItems.Where(c => connectedACClassWFIDs.Contains(c.ACClassWFID)).ToList();

            return new Tuple<ACClassWF, List<ACClassWF>>(wf, filteredDischargingItems);
        }

        public List<RuleGroup> GroupDischargingItems(Database database, dbMes.DatabaseApp databaseApp, List<IACConfigStore> configStores,
            ACClassWF rootWF, List<ACClassWF> dischargingItems)
        {
            List<RuleGroup> ruleGroups = new List<RuleGroup>();
            foreach (ACClassWF aCClassWF in dischargingItems)
            {
                RuleGroup ruleGroup = GetRuleGroup(ruleGroups, aCClassWF);
                dbMes.ACClassWF mesWf = aCClassWF.FromAppContext<dbMes.ACClassWF>(databaseApp);
                List<dbMes.MaterialWFConnection> connections =
                    mesWf
                    .MaterialWFConnection_ACClassWF
                    .GroupBy(c => c.Material.MaterialNo)
                    .Select(c => c.FirstOrDefault())
                    .OrderBy(c => c.Material.MaterialNo).ToList();

                List<ACClass> availableValues = RulesCommand.GetProcessModules(aCClassWF, database).ToList();
                string preConfigACUrl = rootWF.ConfigACUrl;
                if (!preConfigACUrl.EndsWith("\\"))
                {
                    preConfigACUrl = preConfigACUrl + "\\";
                }
                List<ACClass> exludedProcessModules = GetExcludedProcessModules(database, configStores, preConfigACUrl, aCClassWF);
                List<ACClass> selectedValues = InvertSelection(availableValues, exludedProcessModules);
                foreach (dbMes.MaterialWFConnection connection in connections)
                {
                    RuleSelection ruleSelection = new RuleSelection(ruleGroup);
                    ruleSelection.PreConfigACUrl = preConfigACUrl;
                    ruleSelection.WF = new mes.facility.MapPosToWFConn()
                    {
                        PWNode = aCClassWF
                    };
                    ruleSelection.WF.MatWFConn = connection;

                    try
                    {
                        string[] materialNos = new string[] { };
                        dbMes.Partslist partslist = configStores.FirstOrDefault() as dbMes.Partslist;
                        if (partslist != null)
                        {
                            materialNos =
                                partslist
                                .PartslistPos_Partslist.Where(c => c.MaterialID == connection.Material.MaterialID)
                                .SelectMany(c => c.PartslistPosRelation_TargetPartslistPos)
                                .Select(c => c.SourcePartslistPos.Material)
                                .GroupBy(c => c.MaterialNo)
                                .Select(c => c.Key)
                                .ToArray();
                        }
                        if (materialNos.Any())
                        {
                            List<ACClass> filteredAvailableValues = FilterModulesByMaterial(databaseApp, availableValues, materialNos);
                            ruleSelection.Items = GetItems(ruleSelection, filteredAvailableValues, selectedValues);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    ruleGroup.RuleSelections.Add(ruleSelection);
                }

                // add for all material
                RuleSelection allMaterialsSelection = new RuleSelection(ruleGroup);
                allMaterialsSelection.PreConfigACUrl = preConfigACUrl;
                allMaterialsSelection.WF = new mes.facility.MapPosToWFConn()
                {
                    PWNode = aCClassWF,
                    MatWFConn = new dbMes.MaterialWFConnection()
                    {
                        Material = new dbMes.Material()
                        {
                            MaterialNo = "-",
                            MaterialName1 = "-"
                        }
                    }
                };
                allMaterialsSelection.Items = GetItems(allMaterialsSelection, availableValues, selectedValues);
                ruleGroup.RuleSelections.Add(allMaterialsSelection);
            }
            return ruleGroups;
        }

        private List<RuleItem> GetItems(RuleSelection ruleSelection, List<ACClass> availableValues, List<ACClass> selectedValues)
        {
            return
                availableValues
                .Select(c =>
                new RuleItem(ruleSelection)
                {
                    Machine = c,
                    IsSelected = selectedValues != null ? selectedValues.Select(x => x.ACClassID).Contains(c.ACClassID) : false
                })
                .ToList();
        }

        private List<ACClass> FilterModulesByMaterial(dbMes.DatabaseApp databaseApp, List<ACClass> modules, string[] materialNos)
        {
            Guid[] facilityAssociatedWithMaterial =
                databaseApp
                .Facility
                .Where(c =>
                                c.Material != null
                                && materialNos.Contains(c.Material.MaterialNo)
                                && c.VBiFacilityACClassID != null
                     )
                .Select(c => c.VBiFacilityACClassID ?? Guid.Empty)
                .ToArray();

            dbMes.Facility[] faciltiyWithMaterial =
                databaseApp
                .Material
                .Where(c => materialNos.Contains(c.MaterialNo))
                .SelectMany(c => c.FacilityCharge_Material)
                .Where(c => !c.NotAvailable && c.FacilityID != null)
                .Select(c => c.Facility)
                .GroupBy(c => c.FacilityNo)
                .Select(c => c.FirstOrDefault())
                .ToArray();

            Guid[] facilityHaveMaterialCharge =
                faciltiyWithMaterial
                .Where(c =>
                               c.VBiFacilityACClassID != null
                     )
                .Select(c => c.VBiFacilityACClassID ?? Guid.Empty)
                .ToArray();

            List<ACClass> filtered =
                modules
                .Where(c =>
                    facilityAssociatedWithMaterial.Contains(c.ACClassID)
                    || facilityHaveMaterialCharge.Contains(c.ACClassID))
            .ToList();
            return filtered;
        }

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

        private List<ACClass> InvertSelection(List<ACClass> availableValues, List<ACClass> exludedProcessModules)
        {
            return
                availableValues
                .Where(c => exludedProcessModules == null || !exludedProcessModules.Select(x => x.ACClassID).Contains(c.ACClassID))
                .ToList();
        }

        private RuleGroup GetRuleGroup(List<RuleGroup> ruleGroups, ACClassWF aCClassWF)
        {
            RuleGroup ruleGroup = ruleGroups.FirstOrDefault(c => c.RefPAACClass.ACClassID == aCClassWF.RefPAACClassID);
            if (ruleGroup == null)
            {
                ruleGroup = new RuleGroup()
                {
                    RefPAACClass = aCClassWF.RefPAACClass
                };
                ruleGroups.Add(ruleGroup);
            }
            return ruleGroup;
        }

        private void SaveSourceSelectionRules(List<RuleGroup> ruleGroupList)
        {
            if (ruleGroupList != null)
            {
                foreach (RuleGroup ruleGroup in ruleGroupList)
                {
                    foreach (RuleSelection ruleSelection in ruleGroup.RuleSelections)
                    {
                        // use only summary ACClassWF value (not filtered by material)
                        if (ruleSelection.WF.MatWFConn.MaterialWFConnectionID == Guid.Empty)
                        {
                            BSOPartslist partslist = ParentACComponent as BSOPartslist;
                            string configACUrl = ruleSelection.WF.PWNode.ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Excluded_process_modules.ToString();
                            IACConfig currentStoreConfigItem = ACConfigHelper.GetStoreConfiguration(partslist.CurrentConfigStore.ConfigurationEntries, ruleSelection.PreConfigACUrl, configACUrl, false, null);

                            if (!ruleSelection.Items.Any(c => !c.IsSelected))
                            {
                                if(currentStoreConfigItem != null)
                                {
                                    partslist.CurrentConfigStore.RemoveACConfig(currentStoreConfigItem);
                                }
                            }
                            else
                            {
                                if (currentStoreConfigItem == null)
                                {
                                    currentStoreConfigItem = partslist.CurrentConfigStore.NewACConfig(ruleSelection.WF.PWNode);
                                    currentStoreConfigItem.PreConfigACUrl = ruleSelection.PreConfigACUrl;
                                    currentStoreConfigItem.LocalConfigACUrl = configACUrl;
                                }
                                List<object> excludedModules = ruleSelection.Items.Where(c => !c.IsSelected).Select(c => c.Machine as object).ToList();
                                RuleValue ruleValue = RulesCommand.RuleValueFromObjectList(ACClassWFRuleTypes.Excluded_process_modules, excludedModules);
                                RulesCommand.WriteIACConfig(Database, currentStoreConfigItem, new List<RuleValue>() { ruleValue });
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
