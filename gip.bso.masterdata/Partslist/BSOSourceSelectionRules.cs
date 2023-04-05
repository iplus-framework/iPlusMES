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
                if (_CurrentRuleSelection != value)
                {

                    _CurrentRuleSelection = value;
                    OnPropertyChanged(nameof(CurrentRuleSelection));
                }
            }
        }

        #endregion

        #region Others

        private List<RuleGroup> LoadRuleGroupList(Database database, dbMes.DatabaseApp databaseApp, List<IACConfigStore> configStores, Guid acClassWFID, Guid partslistID)
        {
            List<ACClassWF> dischargingItems = GetAllDischargingItems(database, databaseApp, acClassWFID, partslistID);
            List<RuleGroup> grouped = GroupDischargingItems(database, databaseApp, configStores, dischargingItems);
            return grouped;
        }
        public List<ACClassWF> GetAllDischargingItems(Database database, dbMes.DatabaseApp databaseApp, Guid acClassWFID, Guid partslistID)
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

            return filteredDischargingItems;
        }

        public List<RuleGroup> GroupDischargingItems(Database database, dbMes.DatabaseApp databaseApp, List<IACConfigStore> configStores, List<ACClassWF> dischargingItems)
        {
            List<RuleGroup> ruleGroups = new List<RuleGroup>();
            foreach (ACClassWF aCClassWF in dischargingItems)
            {
                RuleGroup ruleGroup = GetRuleGroup(database, ruleGroups, aCClassWF);
                dbMes.ACClassWF mesWf = aCClassWF.FromAppContext<dbMes.ACClassWF>(databaseApp);
                List<dbMes.MaterialWFConnection> connections =
                    mesWf
                    .MaterialWFConnection_ACClassWF
                    .GroupBy(c => c.Material.MaterialNo)
                    .Select(c => c.FirstOrDefault())
                    .OrderBy(c => c.Material.MaterialNo).ToList();

                List<ACClass> modules = RulesCommand.GetProcessModules(aCClassWF, database).ToList();

                foreach (dbMes.MaterialWFConnection connection in connections)
                {
                    RuleSelection ruleSelection = new RuleSelection();
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
                            ruleSelection.AvailableValues = FilterModulesByMaterial(databaseApp, modules, materialNos);
                            ruleSelection.SelectedValues = GetSelectedValues(database, configStores, aCClassWF);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    ruleGroup.RuleSelections.Add(ruleSelection);
                }

                // add for all material
                RuleSelection allMaterialsSelection = new RuleSelection();
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
                allMaterialsSelection.AvailableValues = modules;
                allMaterialsSelection.SelectedValues = GetSelectedValues(database, configStores, aCClassWF);
                ruleGroup.RuleSelections.Add(allMaterialsSelection);
            }
            return ruleGroups;
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

        private List<ACClass> GetSelectedValues(Database database, List<IACConfigStore> configStores, ACClassWF aCClassWF)
        {
            List<ACClass> result = new List<ACClass>();
            ConfigManagerIPlus serviceInstance = ConfigManagerIPlus.GetServiceInstance(this);
            RuleValueList ruleValueList = serviceInstance.GetRuleValueList(configStores, "", aCClassWF.ConfigACUrl + @"\Rules\" + ACClassWFRuleTypes.Allowed_instances.ToString());
            if (ruleValueList != null)
            {
                result = ruleValueList.GetSelectedClasses(ACClassWFRuleTypes.Allowed_instances, database).ToList();
            }
            return result ?? new List<ACClass>();
        }

        private RuleGroup GetRuleGroup(Database database, List<RuleGroup> ruleGroups, ACClassWF aCClassWF)
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

        #endregion
    }
}
