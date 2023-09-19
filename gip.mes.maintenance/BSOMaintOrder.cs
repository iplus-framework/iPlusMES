using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.autocomponent;
using gip.core.autocomponent;
using gip.bso.iplus;
using System.Runtime.Remoting.Messaging;
using gip.bso.masterdata;

namespace gip.mes.maintenance
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance Order'}de{'Wartungsauftrag'}", Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MaintOrder.ClassName)]
    public class BSOMaintOrder : BSOMaintOrderBase
    {
        #region c'tors
        public BSOMaintOrder(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _CN_BSOProcessControl = new ACPropertyConfigValue<string>(this, "CN_BSOProcessControl", "");
            _CN_BSOVisualisation = new ACPropertyConfigValue<string>(this, "CN_BSOVisualisation", "");
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool init = base.ACInit(startChildMode);
            //_BSOComponentSelector = ACComponentChilds.FirstOrDefault(c => c.ACIdentifier == "BSOComponentSelector_Child");
            //if (_BSOComponentSelector == null)
            //    return false;
            Search();
            return init;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            CurrentMaintOrder = null;
            CurrentWarningComponent = null;
            SelectedMaintOrder = null;
            //SelectedMaintOrderHistory = null;
            SelectedMaintOrderProperty = null;
            //SelectedMaintOrderPropertyHistory = null;
            //SelectedMaintTask = null;
            //SelectedMaintTaskHistory = null;
            //MaintOrderHistoryList = null;
            //MaintTaskList = null;
            //MaintOrderHistoryList = null;
            return base.ACDeInit(deleteACClassTask);
        }

        public const string ClassName = nameof(BSOMaintOrder);

        public IACComponent ComponentSelector
        {
            get
            {
                BSOComponentSelector componentSelector = this.FindChildComponents<BSOComponentSelector>(c => c is BSOComponentSelector).FirstOrDefault();
                if (componentSelector != null)
                    return componentSelector;
                gip.core.datamodel.ACClass acClassOfSelector = typeof(BSOComponentSelector).GetACType() as gip.core.datamodel.ACClass;
                componentSelector = StartComponent(acClassOfSelector, acClassOfSelector, null) as BSOComponentSelector;
                return componentSelector;
            }
        }
        #endregion

        #region BSO -> ACProperty

        #region Config
        private ACPropertyConfigValue<string> _CN_BSOProcessControl;
        [ACPropertyConfig("en{'Classname BSOProcessControl'}de{'Klassenname BSOProcessControl'}")]
        public string CN_BSOProcessControl
        {
            get
            {
                if (!String.IsNullOrEmpty(_CN_BSOProcessControl.ValueT))
                    return _CN_BSOProcessControl.ValueT;
                gip.core.datamodel.ACClass classOfBso = typeof(BSOProcessControl).GetACType() as gip.core.datamodel.ACClass;
                if (classOfBso != null)
                {
                    var derivation = gip.core.datamodel.Database.GlobalDatabase.ACClass
                                                .Where(c => c.BasedOnACClassID == classOfBso.ACClassID
                                                        && !String.IsNullOrEmpty(c.AssemblyQualifiedName)
                                                        && c.AssemblyQualifiedName != classOfBso.AssemblyQualifiedName).FirstOrDefault();
                    if (derivation != null)
                        _CN_BSOProcessControl.ValueT = derivation.ACIdentifier;
                    else
                        _CN_BSOProcessControl.ValueT = classOfBso.ACIdentifier;
                    return _CN_BSOProcessControl.ValueT;
                }
                return BSOProcessControl.BSOClassName;
            }
            set { _CN_BSOProcessControl.ValueT = value; }
        }

        private ACPropertyConfigValue<string> _CN_BSOVisualisation;
        [ACPropertyConfig("en{'Classname BSOVisualisation'}de{'Klassenname BSOVisualisation'}")]
        public string CN_BSOVisualisation
        {
            get
            {
                if (!String.IsNullOrEmpty(_CN_BSOVisualisation.ValueT))
                    return _CN_BSOVisualisation.ValueT;
                gip.core.datamodel.ACClass classOfBso = typeof(BSOVisualisationStudio).GetACType() as gip.core.datamodel.ACClass;
                if (classOfBso != null)
                {
                    var derivation = gip.core.datamodel.Database.GlobalDatabase.ACClass
                                                .Where(c => c.BasedOnACClassID == classOfBso.ACClassID
                                                        && !String.IsNullOrEmpty(c.AssemblyQualifiedName)
                                                        && c.AssemblyQualifiedName != classOfBso.AssemblyQualifiedName).FirstOrDefault();
                    if (derivation != null)
                        _CN_BSOVisualisation.ValueT = derivation.ACIdentifier;
                    else
                        _CN_BSOVisualisation.ValueT = classOfBso.ACIdentifier;
                    return _CN_BSOVisualisation.ValueT;
                }
                return BSOVisualisationStudio.BSOClassName;
            }
            set { _CN_BSOVisualisation.ValueT = value; }
        }

        #endregion

        #region ACProperty => MaintOrder

        public override IAccessNav AccessNav => AccessPrimary;

        private string _MaintOrderTask;
        [ACPropertyInfo(999, "", "en{'Tasks'}de{'Aufgaben'}")]
        public string MaintOrderTask
        {
            get
            {
                return _MaintOrderTask;
            }
            set
            {
                _MaintOrderTask = value;
                OnPropertyChanged("MaintOrderTask");
            }
        }

        #endregion

        #region ACProperty => MaintOrderProperty

        private MaintOrderProperty _SelectedMaintOrderProperty;
        [ACPropertySelected(999, "MaintOrderProperty")]
        public MaintOrderProperty SelectedMaintOrderProperty
        {
            get
            {
                return _SelectedMaintOrderProperty;
            }
            set
            {
                _SelectedMaintOrderProperty = value;
                //if (SelectedMaintTask != null & _SelectedMaintOrderProperty != null)
                //{
                //    var task = _SelectedMaintOrderProperty.MaintACClassProperty.MaintACClassVBGroup_MaintACClassProperty.FirstOrDefault(c => c.VBGroupID == SelectedMaintTask.MaintACClassVBGroup.VBGroupID);
                //    if (task != null)
                //        MaintOrderPropertyTask = task.Comment;
                //    else
                //        MaintOrderPropertyTask = null;
                //}

                OnPropertyChanged();
            }
        }

        private List<MaintOrderProperty> _MaintOrderPropertyList;
        [ACPropertyList(999, "MaintOrderProperty")]
        public List<MaintOrderProperty> MaintOrderPropertyList
        {
            get
            {
                return _MaintOrderPropertyList;
            }
            set
            {
                _MaintOrderPropertyList = value;
                OnPropertyChanged("MaintOrderPropertyList");
            }
        }

        private string _MaintOrderPropertyTask;
        [ACPropertyInfo(999, "", "en{'Property task'}de{'Aufgabe nach Instandhaltungseigenschaft'}")]
        public string MaintOrderPropertyTask
        {
            get
            {
                return _MaintOrderPropertyTask;
            }
            set
            {
                _MaintOrderPropertyTask = value;
                OnPropertyChanged("MaintOrderPropertyTask");
            }
        }

        #endregion

        [ACPropertySelected(999, "Warning")]
        public ACMaintWarning CurrentWarningComponent
        {
            get;
            set;
        }

        [ACPropertyList(999, "Warning")]
        public List<ACMaintWarning> ComponentsWarningList
        {
            get;
            set;
        }

        #region ACProperty => Filter

        [ACPropertyCurrent(9999, "MaintOrderStateFilter", "en{'Order state'}de{'Auftragsstatus'}")]
        public MDMaintOrderState CurrentMaintOrderStateFilter
        {
            get;
            set;
        }

        private IEnumerable<MDMaintOrderState> _MaintOrderStateFilterList;
        [ACPropertyList(9999, "MaintOrderStateFilter")]
        public IEnumerable<MDMaintOrderState> MaintOrderStateFilterList
        {
            get
            {
                if (_MaintOrderStateFilterList == null)
                {
                    _MaintOrderStateFilterList = DatabaseApp.MDMaintOrderState;
                    CurrentMaintOrderStateFilter = _MaintOrderStateFilterList.FirstOrDefault(c => c.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceNeeded);
                    SearchFilter();
                }
                return _MaintOrderStateFilterList;
            }
        }

        private core.datamodel.ACClass _CurrentComponentFilter;
        [ACPropertyInfo(9999, "", "en{'Object'}de{'Objekt'}")]
        public core.datamodel.ACClass CurrentComponentFilter
        {
            get
            {
                return _CurrentComponentFilter;
            }
            set
            {
                _CurrentComponentFilter = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentObjectFilter));
            }
        }

        private Facility _CurrentFacilityFilter;
        [ACPropertyInfo(9999, "", "en{'Facility'}de{'Anlage'}")]
        public Facility CurrentFacilityFilter
        {
            get => _CurrentFacilityFilter;
            set
            {
                _CurrentFacilityFilter = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentObjectFilter));
            }
        }

        [ACPropertyInfo(9999, "", "en{'Object'}de{'Objekt'}")]
        public string CurrentObjectFilter
        {
            get
            {
                if (CurrentFacilityFilter != null)
                    return CurrentFacilityFilter.ACCaption;
                else if (CurrentComponentFilter != null)
                    return CurrentComponentFilter.ACUrlComponent;

                return null;
            }
        }

        private List<Type> _AssignableTypes = new List<Type> { typeof(PAClassAlarmingBase), typeof(ApplicationManager) };
        [ACPropertyInfo(9999)]
        public ACClassInfoWithItems.VisibilityFilters ComponentTypeFilter
        {
            get
            {
                return new ACClassInfoWithItems.VisibilityFilters() { IncludeTypes = _AssignableTypes };
            }
        }

        private bool _IsFiliterVisible = true;
        [ACPropertyInfo(9999)]
        public bool IsFilterVisible
        {
            get
            {
                return _IsFiliterVisible;
            }
            set
            {
                _IsFiliterVisible = value;
                OnPropertyChanged("IsFilterVisible");
            }
        }

        [ACPropertyInfo(9999, "", "en{'Order date'}de{'Auftragsdatum'}")]
        public DateTime? MaintOrderDateTime
        {
            get;
            set;
        }

        private core.datamodel.ACClassDesign _IconClear;
        [ACPropertyInfo(9999)]
        public core.datamodel.ACClassDesign IconClear
        {
            get
            {
                if (_IconClear == null)
                    _IconClear = this.ComponentClass.Designs.FirstOrDefault(c => c.ACIdentifier == "IconClear");
                return _IconClear;
            }
        }


        private bool _ShowMyTasks;
        [ACPropertyInfo(9999, "", "en{'My tasks'}de{'Meine Aufgaben'}")]
        public bool ShowMyTasks
        {
            get => _ShowMyTasks;
            set
            {
                _ShowMyTasks = value;
                OnPropertyChanged();
            }
        }

        private bool _ShowMyDefaultTasks;
        [ACPropertyInfo(9999, "", "en{'My default tasks'}de{'Meine Standardaufgaben'}")]
        public bool ShowMyDefaultTasks
        {
            get => _ShowMyDefaultTasks;
            set
            {
                _ShowMyDefaultTasks = value;
                OnPropertyChanged();
            }
        }

        private bool _ShowOutsourcedTasks;
        [ACPropertyInfo(9999, "", "en{'Outsourced tasks'}de{'Ausgelagerte Aufgaben'}")]
        public bool ShowOutsourcedTasks
        {
            get => _ShowOutsourcedTasks;
            set
            {
                _ShowOutsourcedTasks = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region BSO -> ACMethod

        protected override IQueryable<MaintOrder> _AccessPrimary_NavSearchExecuting(IQueryable<MaintOrder> result)
        {
            result = result.Where(c => c.BasedOnMaintOrderID.HasValue);

            if (ShowMyTasks)
            {
                result = result.Where(c => c.MaintOrderAssignment_MaintOrder.Any(x => x.VBUserID == Root.Environment.User.VBUserID && x.IsActive)
                                        || c.MaintOrderAssignment_MaintOrder.Any(x => x.IsActive && Root.Environment.User.VBUserGroup_VBUser.Any(t => t.VBGroupID == x.VBGroupID)));
            }
            else if (ShowMyDefaultTasks)
            {
                result = result.Where(c => c.MaintOrderAssignment_MaintOrder.Any(x => x.VBUserID == Root.Environment.User.VBUserID && x.IsDefault && x.IsActive));
            }
            else if (ShowOutsourcedTasks)
            {
                result = result.Where(c => c.MaintOrderAssignment_MaintOrder.Any(x => x.IsActive && x.CompanyID.HasValue));
            }

            return result;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodCommand("MaintOrder", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;

            if (AccessPrimary == null)
                return;

            foreach (MaintOrderProperty moProp in CurrentMaintOrder.MaintOrderProperty_MaintOrder.ToArray())
            {
                Msg msgProp = moProp.DeleteACObject(DatabaseApp, true);
                if (msgProp != null)
                {
                    Messages.Msg(msgProp);
                    return;
                }
            }

            foreach (MaintOrderTask maintTask in CurrentMaintOrder.MaintOrderTask_MaintOrder.ToArray())
            {
                Msg msgTask = maintTask.DeleteACObject(DatabaseApp, true);
                if (msgTask != null)
                {
                    Messages.Msg(msgTask);
                    return;
                }
            }

            foreach (MaintOrderAssignment assignment in CurrentMaintOrder.MaintOrderAssignment_MaintOrder.ToArray())
            {
                Msg msgAssignment = assignment.DeleteACObject(DatabaseApp, true);
                if (msgAssignment != null)
                {
                    Messages.Msg(msgAssignment);
                    return;
                }
            }

            Msg msg = CurrentMaintOrder.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentMaintOrder);
            SelectedMaintOrder = AccessPrimary.NavList.FirstOrDefault();

            PostExecute("Delete");
        }

        public bool IsEnabledDelete()
        {
            return SelectedMaintOrder != null;
        }

        [ACMethodInfo("", "en{'Start Worktask'}de{'Arbeitsauftrag beginnen'}", 999)]
        public void StartMaintenanceTask()
        {
            SelectedMaintOrderTask.MDMaintTaskState = DatabaseApp.MDMaintTaskState.FirstOrDefault(c => c.MDMaintTaskStateIndex == (short)MaintTaskState.TaskInProcess);
            SelectedMaintOrderTask.StartDate = DateTime.Now;
            OnPropertyChanged(nameof(SelectedMaintOrderTask));
            if (CurrentMaintOrder != null && CurrentMaintOrder.MDMaintOrderState.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceNeeded)
            {
                CurrentMaintOrder.MDMaintOrderState = DatabaseApp.MDMaintOrderState.FirstOrDefault(c => c.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceInProcess);
                OnPropertyChanged(nameof(CurrentMaintOrder));
            }
            Save();
        }

        public bool IsEnabledStartMaintenanceTask()
        {
            return SelectedMaintOrderTask != null && SelectedMaintOrderTask.MDMaintTaskState != null
                                                  && SelectedMaintOrderTask.MDMaintTaskState.MDMaintTaskStateIndex == (short)MaintTaskState.UnfinishedTask;
        }

        [ACMethodInfo("", "en{'Worktask completed'}de{'Arbeitsauftrag erledigt'}", 999)]
        public void EndMaintenanceTask()
        {
            SelectedMaintOrderTask.MDMaintTaskState = DatabaseApp.MDMaintTaskState.FirstOrDefault(c => c.MDMaintTaskStateIndex == (short)MaintTaskState.TaskCompleted);
            SelectedMaintOrderTask.EndDate = DateTime.Now;
            OnPropertyChanged(nameof(SelectedMaintOrderTask));
            MaintOrderTaskList = MaintOrderTaskList.ToList();
            if (!MaintOrderTaskList.Any(c => c.MDMaintTaskState.MDMaintTaskStateIndex != (short)MaintTaskState.TaskCompleted))
            {
                CurrentMaintOrder.MDMaintOrderStateID = DatabaseApp.MDMaintOrderState.FirstOrDefault(c => c.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceCompleted).MDMaintOrderStateID;
                CurrentMaintOrder.StartDate = MaintOrderTaskList.Min(c => c.StartDate);
                CurrentMaintOrder.EndDate = MaintOrderTaskList.Max(c => c.EndDate);
                
                Save();
                OnPropertyChanged(nameof(CurrentMaintOrder));
            }
            Save();
        }

        public bool IsEnabledEndMaintenanceTask()
        {
            return SelectedMaintOrderTask != null && SelectedMaintOrderTask.MDMaintTaskState != null
                                                 && SelectedMaintOrderTask.MDMaintTaskState.MDMaintTaskStateIndex == (short)MaintTaskState.TaskInProcess;
        }

        [ACMethodInfo("", "", 999)]
        public void ShowMaintenance(IACComponent acComponent)
        {
            CurrentMaintOrder = null;
            CurrentComponentFilter = acComponent.ComponentClass;
            CurrentMaintOrderStateFilter = MaintOrderStateFilterList.FirstOrDefault(c => c.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceNeeded);
            SearchFilter();
            if (CurrentMaintOrder == null)
            {
                CurrentMaintOrderStateFilter = MaintOrderStateFilterList.FirstOrDefault(c => c.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceInProcess);
                SearchFilter();
            }

            if (CurrentMaintOrder != null)
                ShowDialog(this, "MaintOrderDialog");
            else
                Messages.Info(this, "en{'There is no maintenance order for this component.'}de{'Für diese Komponente existiert keine Wartungsaufgabe.'}");
        }

        [ACMethodInfo("", "", 999)]
        public void ShowMaintenanceHistory(IACComponent acComponent)
        {
            CurrentMaintOrder = null;
            IsFilterVisible = false;
            CurrentComponentFilter = acComponent.ComponentClass;
            CurrentMaintOrderStateFilter = MaintOrderStateFilterList.FirstOrDefault(c => c.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceCompleted);
            SearchFilter();

            if (CurrentMaintOrder != null)
                ShowDialog(this, "Mainlayout");
            else
                Messages.Info(this, "en{'There is no maintenance history for this component.'}de{'Für diese Komponente existiert keine Wartungshistorie.'}");
            IsFilterVisible = true;
        }

        [ACMethodInfo("", "", 999)]
        public bool ShowMaintenanceWarning(List<ACMaintWarning> components)
        {
            ComponentsWarningList = components;
            CurrentMaintOrder = null;
            CurrentMaintOrderStateFilter = MaintOrderStateFilterList.FirstOrDefault(c => c.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceNeeded);
            SearchFilter();
            ShowDialog(this, "MaintWarningDialog");
            Search();
            if (MaintOrderList.Any() || (components != null && components.Any()))
                return true;
            return false;
        }

        [ACMethodInfo("", "en{'Show Maintenance Order'}de{'Wartungsauftrag anzeigen'}", 999)]
        public void ShowMaintenaceOrder()
        {
            if (CurrentMaintOrder != null)
            {
                CloseTopDialog();
                ShowDialog(this, "MaintOrderDialog");
            }
        }

        [ACMethodInfo("", "en{'Search'}de{'Suchen'}", 9999)]
        public void SearchFilter()
        {
            CurrentMaintOrder = null;




            if (CurrentMaintOrderStateFilter != null && CurrentComponentFilter == null)
            {
                if (_ACQueryDefinition.ACFilterColumns.Count != 1 || _ACQueryDefinition.ACFilterColumns.FirstOrDefault().PropertyName != "MDMaintOrderState\\MDMaintOrderStateIndex"
                    || _ACQueryDefinition.ACFilterColumns.FirstOrDefault().SearchWord != CurrentMaintOrderStateFilter.MDMaintOrderStateIndex.ToString())
                {
                    _ACQueryDefinition.ClearFilter(true);
                    _ACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDMaintOrderState\\MDMaintOrderStateIndex", Global.LogicalOperators.equal,
                        Global.Operators.and, CurrentMaintOrderStateFilter.MDMaintOrderStateIndex.ToString(), true));
                }
            }
            else if (CurrentMaintOrderStateFilter == null && CurrentComponentFilter != null)
            {
                if (_ACQueryDefinition.ACFilterColumns.Count != 1 || _ACQueryDefinition.ACFilterColumns.FirstOrDefault().PropertyName != "VBiPAACClassID"
                    || _ACQueryDefinition.ACFilterColumns.FirstOrDefault().SearchWord != CurrentComponentFilter.ACClassID.ToString())
                {
                    _ACQueryDefinition.ClearFilter(true);
                    _ACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "VBiPAACClassID", Global.LogicalOperators.equal,
                        Global.Operators.and, CurrentComponentFilter.ACClassID.ToString(), true));
                }
            }
            else if (CurrentMaintOrderStateFilter != null && CurrentComponentFilter != null)
            {
                bool rebuildACQuery = false;
                if (_ACQueryDefinition.ACFilterColumns.Count != 2)
                    rebuildACQuery = true;
                else
                {
                    ACFilterItem state = _ACQueryDefinition.ACFilterColumns.FirstOrDefault(c => c.PropertyName == "MDMaintOrderState\\MDMaintOrderStateIndex");
                    if (state == null)
                        rebuildACQuery = true;
                    else if (state.SearchWord != CurrentMaintOrderStateFilter.MDMaintOrderStateIndex.ToString())
                        rebuildACQuery = true;
                    if (!rebuildACQuery)
                    {
                        ACFilterItem acClassID = _ACQueryDefinition.ACFilterColumns.FirstOrDefault(c => c.PropertyName == "VBiPAACClassID");
                        if (acClassID == null)
                            rebuildACQuery = true;
                        else if (acClassID.SearchWord != CurrentComponentFilter.ACClassID.ToString())
                            rebuildACQuery = true;
                    }
                }
                if (rebuildACQuery)
                {
                    _ACQueryDefinition.ClearFilter(true);
                    _ACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "VBiPAACClassID", Global.LogicalOperators.equal,
                        Global.Operators.and, CurrentComponentFilter.ACClassID.ToString(), true));
                    _ACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDMaintOrderState\\MDMaintOrderStateIndex", Global.LogicalOperators.equal,
                        Global.Operators.and, CurrentMaintOrderStateFilter.MDMaintOrderStateIndex.ToString(), true));
                }
            }
            else
            {
                _ACQueryDefinition.ClearFilter(true);
            }
            Search();
        }

        [ACMethodInfo("", "en{'Choose Object'}de{'Objekt auswählen'}", 9999)]
        public void ChooseComponent()
        {
            if (!IsEnabledChooseComponent())
                return;

            ShowDialog(this, "MaintOrderEntity");

            BSOFacilityExplorer facilityExpl = FindChildComponents<BSOFacilityExplorer>(c => c is BSOFacilityExplorer).FirstOrDefault();
            if (facilityExpl != null && facilityExpl.SelectedFacility != null)
            {
                CurrentFacilityFilter = facilityExpl.SelectedFacility;
            }
            else
            {
                BSOComponentSelector compExpl = FindChildComponents<BSOComponentSelector>(c => c is BSOComponentSelector).FirstOrDefault();
                if (compExpl != null && compExpl.CurrentProjectItemCS != null)
                {
                    CurrentComponentFilter = compExpl.CurrentProjectItemCS.ValueT;
                }
            }
        }

        public bool IsEnabledChooseComponent()
        {
            return true;
        }

        [ACMethodInfo("", "", 999)]
        public void ClearChosenComponent()
        {
            CurrentFacilityFilter = null;
            CurrentComponentFilter = null;
        }

        [ACMethodInfo("", "en{'Documentation'}de{'Dokumentation'}", 9999)]
        public void OpenDocumentation()
        {

        }

        /// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        /// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        /// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        /// <returns>ControlModesInfo</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            var cm = base.OnGetControlModes(vbControl);
            if (vbControl != null && !string.IsNullOrEmpty(vbControl.VBContent))
            {
                switch (vbControl.VBContent)
                {
                    case nameof(IsFilterVisible):
                        if (IsFilterVisible)
                            cm = Global.ControlModes.Enabled;
                        else
                            cm = Global.ControlModes.Collapsed;
                        break;
                    case "CurrentMaintOrder\\MaintActStartDate":
                        if (CurrentMaintOrder != null && CurrentMaintOrder.MDMaintOrderState.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceCompleted)
                            cm = Global.ControlModes.Disabled;
                        else
                            cm = Global.ControlModes.Enabled;
                        break;
                    case "CurrentMaintOrder\\MaintActEndDate":
                        if (CurrentMaintOrder != null && CurrentMaintOrder.MDMaintOrderState.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceCompleted)
                            cm = Global.ControlModes.Disabled;
                        else
                            cm = Global.ControlModes.Enabled;
                        break;
                    case "CurrentMaintOrder\\Comment":
                        if (CurrentMaintOrder != null && CurrentMaintOrder.MDMaintOrderState.MDMaintOrderStateIndex == (short)MDMaintOrderState.MaintOrderStates.MaintenanceCompleted)
                            cm = Global.ControlModes.Disabled;
                        else
                            cm = Global.ControlModes.Enabled;
                        break;
                    //case "SelectedMaintTask\\IsRepair":
                    //    if (SelectedMaintTask != null && SelectedMaintTask.MDMaintTaskState.MDMaintTaskStateIndex == (short)MaintTaskState.TaskCompleted)
                    //        cm = Global.ControlModes.Disabled;
                    //    else
                    //        cm = Global.ControlModes.Enabled;
                    //    break;
                    //case "SelectedMaintTask\\StartTaskDate":
                    //    if (SelectedMaintTask != null && SelectedMaintTask.MDMaintTaskState.MDMaintTaskStateIndex == (short)MaintTaskState.TaskCompleted)
                    //        cm = Global.ControlModes.Disabled;
                    //    else
                    //        cm = Global.ControlModes.Enabled;
                    //    break;
                    //case "SelectedMaintTask\\EndTaskDate":
                    //    if (SelectedMaintTask != null && SelectedMaintTask.MDMaintTaskState.MDMaintTaskStateIndex == (short)MaintTaskState.TaskCompleted)
                    //        cm = Global.ControlModes.Disabled;
                    //    else
                    //        cm = Global.ControlModes.Enabled;
                    //    break;
                    //case "SelectedMaintTask\\Comment":
                    //    if (SelectedMaintTask != null && SelectedMaintTask.MDMaintTaskState.MDMaintTaskStateIndex == (short)MaintTaskState.TaskCompleted)
                    //        cm = Global.ControlModes.Disabled;
                    //    else
                    //        cm = Global.ControlModes.Enabled;
                    //    break;
                }
            }
            return cm;
        }

        #region Navigation
        IACBSOAlarmPresenter _BSOAlarmPresenter = null;
        MaintOrder _DelegatedMaintOrder = null;
        FocusBSOResult? _DelegateFocus = null;

        //[ACMethodInteraction("MaintOrder", "en{'Start Maintenance and Navigate'}de{'Starte Wartung und Navigiere'}", 311, true, "CurrentMaintOrder")]
        //public void SwitchToMaintenanceAndShow()
        //{
        //}

        [ACMethodInteraction("MaintOrder", "en{'Navigate to visualisation'}de{'Navigiere zur Visualisierung'}", 310, true, "CurrentMaintOrder")]
        public void NavigateToVisualisation()
        {
            //bool startNewBSO = false;
            //if (_BSOAlarmPresenter != null)
            //    return;
            //_DelegateFocus = null;
            //_DelegatedMaintOrder = null;

            //IACBSOAlarmPresenter alarmPresenter = null;
            //if (ACUrlHelper.IsUrlDynamicInstance(SelectedMaintOrder.ComponentACUrl))
            //{
            //    alarmPresenter = Root.Businessobjects.FindChildComponents<BSOProcessControl>(c => c is BSOProcessControl, null, 1).FirstOrDefault();
            //    startNewBSO = alarmPresenter == null;
            //    if (startNewBSO)
            //        this.Root.RootPageWPF.StartBusinessobject(Const.BusinessobjectsACUrl + ACUrlHelper.Delimiter_Start + CN_BSOProcessControl, null);
            //    alarmPresenter = Root.Businessobjects.FindChildComponents<BSOProcessControl>(c => c is BSOProcessControl, null, 1).FirstOrDefault();
            //}
            //else
            //{
            //    alarmPresenter = Root.Businessobjects.FindChildComponents<BSOVisualisationStudio>(c => c is BSOVisualisationStudio, null, 1).FirstOrDefault();
            //    startNewBSO = alarmPresenter == null;
            //    if (startNewBSO)
            //        this.Root.RootPageWPF.StartBusinessobject(Const.BusinessobjectsACUrl + ACUrlHelper.Delimiter_Start + CN_BSOVisualisation, null);
            //    alarmPresenter = Root.Businessobjects.FindChildComponents<BSOVisualisationStudio>(c => c is BSOVisualisationStudio, null, 1).FirstOrDefault();
            //}
            //if (alarmPresenter == null)
            //    return;
            //if (startNewBSO)
            //{
            //    _BSOAlarmPresenter = alarmPresenter;
            //    _DelegatedMaintOrder = SelectedMaintOrder;
            //    // Wait until view is loaded
            //    _BSOAlarmPresenter.ACActionEvent += AlarmPresenter_ACActionEvent;
            //}
            //else
            //    SwitchToViewOnAlarm(alarmPresenter, SelectedMaintOrder);
        }

        public bool IsEnabledNavigateToVisualisation()
        {
            return SelectedMaintOrder != null;
        }

        private void AlarmPresenter_ACActionEvent(object sender, ACActionArgs e)
        {
            if (e.ElementAction == Global.ElementActionType.VBDesignLoaded)
            {
                if (_BSOAlarmPresenter != null)
                {
                    FocusBSOResult focusBSOResult = FocusBSOResult.NotFocusable;
                    try
                    {
                        _BSOAlarmPresenter.ACActionEvent -= AlarmPresenter_ACActionEvent;
                        // Invoked first time after new bso loaded
                        if (_DelegateFocus == null)
                            focusBSOResult = SwitchToViewOnAlarm(_BSOAlarmPresenter, _DelegatedMaintOrder);
                        // Invoked second time after Tabitem with already running bso was switched and loaded to view
                        else
                        {
                            _DelegateFocus = null;
                            _BSOAlarmPresenter.SwitchToViewOnAlarm(CreateMsgFromMaintOrder(_DelegatedMaintOrder));
                        }
                    }
                    catch (Exception ex)
                    {
                        Messages.LogException(this.GetACUrl(), "AlarmPresenter_ACActionEvent", ex.Message);
                    }
                    finally
                    {
                        if (focusBSOResult != FocusBSOResult.SelectionSwitched)
                        {
                            _BSOAlarmPresenter = null;
                            _DelegatedMaintOrder = null;
                        }
                    }
                }
            }
        }

        private FocusBSOResult SwitchToViewOnAlarm(IACBSOAlarmPresenter alarmPresenter, MaintOrder currentACMsgAlarm)
        {
            FocusBSOResult focusResult = this.Root.RootPageWPF.FocusBSO(alarmPresenter);
            if (focusResult == FocusBSOResult.AlreadyFocused)
                alarmPresenter.SwitchToViewOnAlarm(CreateMsgFromMaintOrder(SelectedMaintOrder));
            else if (focusResult == FocusBSOResult.SelectionSwitched)
            {
                _BSOAlarmPresenter = alarmPresenter;
                _DelegatedMaintOrder = currentACMsgAlarm;
                _DelegateFocus = focusResult;
                // Wait until view is loaded
                _BSOAlarmPresenter.ACActionEvent += AlarmPresenter_ACActionEvent;
            }
            return focusResult;
        }

        private Msg CreateMsgFromMaintOrder(MaintOrder maintOrder)
        {
            //if (maintOrder != null)
            //{
            //    IACComponent instance = this.ACUrlCommand(maintOrder.ComponentACUrl) as IACComponent;
            //    if (instance != null)
            //    {
            //        //instance.ACUrlCommand(ACUrlHelper.Delimiter_InvokeMethod + nameof(PAClassPhysicalBase.SwitchToManual));
            //        //instance.ACUrlCommand(ACUrlHelper.Delimiter_InvokeMethod + nameof(PAClassPhysicalBase.SwitchToMaintenance));
            //        return new Msg("Maintenance", instance, eMsgLevel.Info, "", "", 0);
            //    }
            //}
            return new Msg("Maintenance", this, eMsgLevel.Info, "", "", 0);
        }

        #endregion

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "Save":
                    Save();
                    return true;
                case "IsEnabledSave":
                    result = IsEnabledSave();
                    return true;
                case "UndoSave":
                    UndoSave();
                    return true;
                case "IsEnabledUndoSave":
                    result = IsEnabledUndoSave();
                    return true;
                case "Search":
                    Search();
                    return true;
                case "StartMaintenanceTask":
                    StartMaintenanceTask();
                    return true;
                case "IsEnabledStartMaintenanceTask":
                    result = IsEnabledStartMaintenanceTask();
                    return true;
                case "EndMaintenanceTask":
                    EndMaintenanceTask();
                    return true;
                case "IsEnabledEndMaintenanceTask":
                    result = IsEnabledEndMaintenanceTask();
                    return true;
                case "ShowMaintenance":
                    ShowMaintenance((IACComponent)acParameter[0]);
                    return true;
                case "ShowMaintenanceHistory":
                    ShowMaintenanceHistory((IACComponent)acParameter[0]);
                    return true;
                case "ShowMaintenanceWarning":
                    result = ShowMaintenanceWarning((List<gip.mes.maintenance.ACMaintWarning>)acParameter[0]);
                    return true;
                case "ShowMaintenaceOrder":
                    ShowMaintenaceOrder();
                    return true;
                case "SearchFilter":
                    SearchFilter();
                    return true;
                case "ChooseComponent":
                    ChooseComponent();
                    return true;
                case "IsEnabledChooseComponent":
                    result = IsEnabledChooseComponent();
                    return true;
                case "ClearChosenComponent":
                    ClearChosenComponent();
                    return true;
                case "Delete":
                    Delete();
                    return true;
                case "IsEnabledDelete":
                    result = IsEnabledDelete();
                    return true;
                case "NavigateToVisualisation":
                    NavigateToVisualisation();
                    return true;
                case "IsEnabledNavigateToVisualisation":
                    result = IsEnabledNavigateToVisualisation();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}

