using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.facility;
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
                if (_CurrentRuleSelection != null)
                {
                    _CurrentRuleSelection.PropagateRuleSelection = false;
                }
                if (_CurrentRuleSelection != value)
                {
                    _CurrentRuleSelection = value;
                    OnPropertyChanged(nameof(CurrentRuleSelection));
                    if (_CurrentRuleSelection != null)
                    {
                        _CurrentRuleSelection.PropagateRuleSelection = true;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region ACMethod

        [ACMethodInfo("Dialog", "en{'Select Sources'}de{'Quellen auswählen'}", (short)MISort.QueryPreviewDlg)]
        public void ShowDialogSelectSources(Guid acClassWFID, Guid acClassMethodID, Guid partslistID, Guid? prodOrderPartslistID)
        {
            Tuple<List<RuleGroup>, IACConfigStore> result = DoShowDialogSelectSources(DatabaseApp, acClassWFID, acClassMethodID, partslistID, prodOrderPartslistID);

            _RuleGroupList = result.Item1;
            OnPropertyChanged(nameof(RuleGroupList));
            CurrentConfigStore = result.Item2;
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
            SaveSourceSelectionRules(DatabaseApp.ContextIPlus, DatabaseApp, _RuleGroupList);
            this.CloseTopDialog();
        }

        public bool IsEnabledDlgSelectSourcesOk()
        {
            return true;
        }

        #endregion

        #region Methods

        private Tuple<List<RuleGroup>, IACConfigStore> DoShowDialogSelectSources(dbMes.DatabaseApp databaseApp, Guid acClassWFID, Guid acClassMethodID, Guid partslistID, Guid? prodOrderPartslistID)
        {
            List<IACConfigStore> configStores = new List<IACConfigStore>();
            IACConfigStore currentConfigStore = null;
            dbMes.Partslist partslist = databaseApp.Partslist.FirstOrDefault(c => c.PartslistID == partslistID);
            if (prodOrderPartslistID != null)
            {
                dbMes.ProdOrderPartslist prodOrderPartslist = databaseApp.ProdOrderPartslist.FirstOrDefault(c => c.ProdOrderPartslistID == prodOrderPartslistID);
                configStores.Add(prodOrderPartslist);
                currentConfigStore = prodOrderPartslist;
            }
            else
            {
                configStores.Add(partslist);
                currentConfigStore = partslist;
            }

            ACClassMethod method = databaseApp.ContextIPlus.ACClassMethod.FirstOrDefault(c => c.ACClassMethodID == acClassMethodID);
            configStores.Add(method);

            configStores = iPlusMESConfigManager.GetACConfigStores(configStores);


            List<RuleGroup> ruleGroups = LoadRuleGroupList(databaseApp.ContextIPlus, DatabaseApp, configStores, acClassWFID, partslist);
            return new Tuple<List<RuleGroup>, IACConfigStore>(ruleGroups, currentConfigStore);
        }


        private List<RuleGroup> LoadRuleGroupList(Database database, dbMes.DatabaseApp databaseApp, List<IACConfigStore> configStores,
            Guid acClassWFID, dbMes.Partslist partslist)
        {

            ACClassWF inwokeWF = database.ACClassWF.Where(c => c.ACClassWFID == acClassWFID).FirstOrDefault();

            MsgWithDetails validationMessage = new MsgWithDetails();
            PartslistValidationInfo partslistValidationInfo = PartslistManager.CheckResourcesAndRouting(databaseApp, Database.ContextIPlus, partslist, configStores, PARole.ValidationBehaviour.Laxly, validationMessage, inwokeWF);


            List<RuleGroup> grouped = GroupDosingItems(database, configStores, inwokeWF, partslist, partslistValidationInfo);
            return grouped;
        }

        private Tuple<ACClassWF, List<ACClassWF>> GetAllDosingItems(Database database, Guid acClassWFID, dbMes.Partslist partslist)
        {
            Type typeReceiveMat = typeof(IPWNodeReceiveMaterialRouteable);
            ACClassWF wf = database.ACClassWF.Where(c => c.ACClassWFID == acClassWFID).FirstOrDefault();
            ACClassMethod refMth = wf.RefPAACClassMethod;

            List<ACClassWF> dosingItems =
                refMth
                .ACClassWF_ACClassMethod
                .AsEnumerable()
                .Where(c => typeReceiveMat.IsAssignableFrom(c.PWACClass.ObjectType))
                .ToList();

            Guid[] connectedACClassWFIDs =
                partslist
                .MaterialWF
                .MaterialWFACClassMethod_MaterialWF
                .SelectMany(c => c.MaterialWFConnection_MaterialWFACClassMethod)
                .Select(c => c.ACClassWFID)
                .Distinct()
                .ToArray();

            List<ACClassWF> filteredDoisingItems = dosingItems.Where(c => connectedACClassWFIDs.Contains(c.ACClassWFID)).ToList();

            return new Tuple<ACClassWF, List<ACClassWF>>(wf, filteredDoisingItems);
        }

        private List<RuleGroup> GroupDosingItems(Database database, List<IACConfigStore> configStores, ACClassWF rootWF, dbMes.Partslist partslist, PartslistValidationInfo partslistValidationInfo)
        {
            List<RuleGroup> ruleGroups = new List<RuleGroup>();
            MapPosToWFConn mapPosToWFConn = partslistValidationInfo.MapPosToWFConnections.FirstOrDefault();
            if (mapPosToWFConn != null)
            {
                var groupDosingItems = mapPosToWFConn.MapPosToWFConnSubItems.GroupBy(c => c.PWNode.RefPAACClass);

                foreach (var groupItem in groupDosingItems)
                {
                    List<MapPosToWFConnSubItem> mapPosToWFConnSubs = groupItem.ToList();
                    if (
                        !mapPosToWFConnSubs.Any()
                        || !mapPosToWFConnSubs.SelectMany(c => c.Mat4DosingAndRoutes.Keys).Any()
                        || !mapPosToWFConnSubs.SelectMany(c => c.Mat4DosingAndRoutes.Values).SelectMany(c => c).Any()
                        )
                    {
                        continue;
                    }

                    RuleGroup ruleGroup = new RuleGroup()
                    {
                        RefPAACClass = groupItem.Key
                    };
                    ruleGroups.Add(ruleGroup);


                    foreach (MapPosToWFConnSubItem mapPosToWFConnSub in mapPosToWFConnSubs)
                    {
                        string preConfigACUrl = rootWF.ConfigACUrl;
                        if (!preConfigACUrl.EndsWith("\\"))
                        {
                            preConfigACUrl = preConfigACUrl + "\\";
                        }

                        List<ACClass> excludedProcessModules = GetExcludedProcessModules(database, configStores, preConfigACUrl, mapPosToWFConnSub.PWNode);


                        foreach (KeyValuePair<dbMes.Material, List<Route>> mat4DosingAndRoutes in mapPosToWFConnSub.Mat4DosingAndRoutes)
                        {
                            foreach (Route route in mat4DosingAndRoutes.Value)
                            {
                                if (route != null)
                                {
                                    RouteItem source = route.GetRouteSource();
                                    RouteItem target = route.GetRouteTarget();

                                    RuleSelection ruleSelection = ruleGroup.AddRuleSelection(mapPosToWFConnSub.PWNode, mat4DosingAndRoutes.Key, source.Source, target.Target, preConfigACUrl);
                                    ruleSelection.IsSelected = ruleSelection.IsSelected || !excludedProcessModules.Contains(source.Source);
                                }
                            }
                        }
                    }
                }
            }

            foreach (RuleGroup ruleGroup in ruleGroups)
            {
                ruleGroup.RuleSelectionList = ruleGroup.RuleSelectionList.OrderBy(c => c.Source.ACIdentifier).ThenBy(c => c.Target.ACIdentifier).ToList();
            }

            return ruleGroups;
        }

        private void SaveSourceSelectionRules(Database database, dbMes.DatabaseApp databaseApp, List<RuleGroup> ruleGroupList)
        {
            if (ruleGroupList != null)
            {
                bool change = false;

                foreach (RuleGroup ruleGroup in ruleGroupList)
                {
                    foreach (RuleSelection ruleSelection in ruleGroup.RuleSelectionList)
                    {
                        foreach (ACClassWF aCClassWF in ruleSelection.PWNodes)
                        {
                            IACConfig currentStoreConfigItem = ACConfigHelper.GetStoreConfiguration(CurrentConfigStore.ConfigurationEntries, ruleSelection.PreConfigACUrl, ruleSelection.GetConfigACUrl(aCClassWF), false, null);
                            List<ACClass> excludedModules = GetExcludedProcessModules(database, new List<IACConfigStore>() { CurrentConfigStore }, ruleSelection.PreConfigACUrl, aCClassWF);

                            if (ruleSelection.IsSelected)
                            {
                                if (excludedModules.Contains(ruleSelection.Source))
                                {
                                    excludedModules.Remove(ruleSelection.Source);
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
                                var test = CurrentConfigStore.ConfigurationEntries.ToList();

                                if (currentStoreConfigItem == null)
                                {
                                    currentStoreConfigItem = CurrentConfigStore.NewACConfig(aCClassWF);
                                    currentStoreConfigItem.PreConfigACUrl = ruleSelection.PreConfigACUrl;
                                    currentStoreConfigItem.LocalConfigACUrl = ruleSelection.GetConfigACUrl(aCClassWF);
                                }

                                if (!excludedModules.Contains(ruleSelection.Source))
                                {
                                    excludedModules.Add(ruleSelection.Source);
                                }

                                RuleValue ruleValue = RulesCommand.RuleValueFromObjectList(ACClassWFRuleTypes.Excluded_process_modules, excludedModules);
                                RulesCommand.WriteIACConfig(database, currentStoreConfigItem, new List<RuleValue>() { ruleValue });
                                change = true;
                            }
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

        #endregion

        #region Helper methods
        //private List<ACClass> FilterModulesByMaterial(dbMes.DatabaseApp databaseApp, List<ACClass> modules, string[] materialNos)
        //{
        //    Guid[] facilityAssociatedWithMaterial =
        //        databaseApp
        //        .Facility
        //        .Where(c =>
        //                        c.Material != null
        //                        && materialNos.Contains(c.Material.MaterialNo)
        //                        && c.VBiFacilityACClassID != null
        //             )
        //        .Select(c => c.VBiFacilityACClassID ?? Guid.Empty)
        //        .ToArray();

        //    dbMes.Facility[] faciltiyWithMaterial =
        //        databaseApp
        //        .Material
        //        .Where(c => materialNos.Contains(c.MaterialNo))
        //        .SelectMany(c => c.FacilityCharge_Material)
        //        .Where(c => !c.NotAvailable && c.FacilityID != null)
        //        .Select(c => c.Facility)
        //        .GroupBy(c => c.FacilityNo)
        //        .Select(c => c.FirstOrDefault())
        //        .ToArray();

        //    Guid[] facilityHaveMaterialCharge =
        //        faciltiyWithMaterial
        //        .Where(c =>
        //                       c.VBiFacilityACClassID != null
        //             )
        //        .Select(c => c.VBiFacilityACClassID ?? Guid.Empty)
        //        .ToArray();

        //    List<ACClass> filtered =
        //        modules
        //        .Where(c =>
        //            facilityAssociatedWithMaterial.Contains(c.ACClassID)
        //            || facilityHaveMaterialCharge.Contains(c.ACClassID))
        //    .ToList();
        //    return filtered;
        //}

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
