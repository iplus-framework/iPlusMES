// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.bso.iplus;
using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static gip.core.datamodel.Global;

namespace gip.mes.maintenance
{
    /// <summary>
    /// Abstract base class for the maintenance plan and maintenance order business objects (BSOMaintOrder, BSOMaintPlan).
    /// It provides the primary navigation access (AccessPrimary) on the MaintOrder entity, sorted descending by MaintOrderNo, with a default take count of 50 records and optional default filters (NavigationqueryDefaultFilter).
    /// The search result is provided in the MaintOrderList property; the selected record in SelectedMaintOrder and the record to edit in CurrentMaintOrder.
    /// Setting CurrentMaintOrder automatically loads the tasks of the order into MaintOrderTaskList (selected task: SelectedMaintOrderTask) and the assignments into MaintOrderAssignmentList (selected assignment: SelectedMaintOrderAssignment).
    /// The order list can be filtered by facility (CurrentFacilityFilter) or by technical component (CurrentComponentFilter); CurrentObjectFilter returns the caption of the active filter.
    /// Derived classes extend the search in _AccessPrimary_NavSearchExecuting and add the workflow methods (e.g. StartMaintenanceTask, EndMaintenanceTask, Delete).
    /// Changes on CurrentMaintOrder must be persisted with Save(); UndoSave() reverts unsaved changes; Search() re-executes the search; SearchFilter() clears the current record and re-executes the search with the active filters.
    /// With ChooseComponent() a dialog (MaintOrderEntity) is opened in which a facility or a component can be selected as filter; ClearChosenComponent() removes the filter; ChooseComponentOK() closes the dialog.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Base class for Maintenance plans and orders'}de{'Basisklasse für Wartungspläne und Wartungsaufträge'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true,
        Description = @"Abstract base class for the maintenance plan and maintenance order business objects (BSOMaintOrder, BSOMaintPlan).
        It provides the primary navigation access (AccessPrimary) on the MaintOrder entity, sorted descending by MaintOrderNo, with a default take count of 50 records and optional default filters (NavigationqueryDefaultFilter).
        The search result is provided in the MaintOrderList property; the selected record in SelectedMaintOrder and the record to edit in CurrentMaintOrder.
        Setting CurrentMaintOrder automatically loads the tasks of the order into MaintOrderTaskList (selected task: SelectedMaintOrderTask) and the assignments into MaintOrderAssignmentList (selected assignment: SelectedMaintOrderAssignment).
        The order list can be filtered by facility (CurrentFacilityFilter) or by technical component (CurrentComponentFilter); CurrentObjectFilter returns the caption of the active filter.
        Derived classes extend the search in _AccessPrimary_NavSearchExecuting and add the workflow methods (e.g. StartMaintenanceTask, EndMaintenanceTask, Delete).
        Changes on CurrentMaintOrder must be persisted with Save(); UndoSave() reverts unsaved changes; Search() re-executes the search; SearchFilter() clears the current record and re-executes the search with the active filters.
        With ChooseComponent() a dialog (MaintOrderEntity) is opened in which a facility or a component can be selected as filter; ClearChosenComponent() removes the filter; ChooseComponentOK() closes the dialog.")]
    public abstract class BSOMaintOrderBase : ACBSOvbNav
    {
        #region c'tors

        public BSOMaintOrderBase(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

            bool done = await base.ACDeInit(deleteACClassTask);
            //if (done && _BSODatabase != null)
            //{
            //    ACObjectContextManager.DisposeAndRemove(_BSODatabase);
            //    _BSODatabase = null;
            //}

            //if (done && _DatabaseApp != null)
            //{
            //    ACObjectContextManager.DisposeAndRemove(_DatabaseApp);
            //    _DatabaseApp = null;
            //}

            return done;
        }

        #endregion

        #region Properties
        //private Database _BSODatabase = null;
        //public override IACEntityObjectContext Database
        //{
        //    get
        //    {
        //        if (_BSODatabase == null)
        //            _BSODatabase = ACObjectContextManager.GetOrCreateContext<Database>(this.GetACUrl());
        //        return _BSODatabase;
        //    }
        //}

        //private DatabaseApp _DatabaseApp;
        //public override DatabaseApp DatabaseApp
        //{
        //    get
        //    {
        //        if (_DatabaseApp == null)
        //            _DatabaseApp = ACObjectContextManager.GetOrCreateContext<DatabaseApp>(this.GetACUrl(),"", Database.ContextIPlus);
        //        return _DatabaseApp;
        //    }
        //}

        protected ACQueryDefinition _ACQueryDefinition;

        private ACAccessNav<MaintOrder> _AccessPrimary;
        [ACPropertyAccessPrimary(999, "MaintOrder", Description = "Primary navigation access on the MaintOrder entity, sorted descending by MaintOrderNo, with the default filters of the derived BSO.")]
        public ACAccessNav<MaintOrder> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(this, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _ACQueryDefinition = navACQueryDefinition;
                    navACQueryDefinition.ClearSort(true);
                    navACQueryDefinition.ACSortColumns.Add(new ACSortItem(nameof(MaintOrder.MaintOrderNo), Global.SortDirections.descending, true));
                    navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MaintOrder>("MaintOrder", this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected maintenance order.
        /// </summary>
        /// <value>The selected maintenance order.</value>
        [ACPropertySelected(601, nameof(MaintOrder), Description = "The maintenance order that is currently selected in the order list. Setting it also sets CurrentMaintOrder.")]
        public virtual MaintOrder SelectedMaintOrder
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Selected;
            }
            set
            {
                AccessPrimary.Selected = value;
                if (CurrentMaintOrder != value)
                    CurrentMaintOrder = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current maintenance order.
        /// </summary>
        /// <value>The current maintenance order.</value>
        [ACPropertyCurrent(602, nameof(MaintOrder), Description = "The current maintenance order that is displayed and edited. Setting it loads the tasks into MaintOrderTaskList and the assignments into MaintOrderAssignmentList.")]
        public virtual MaintOrder CurrentMaintOrder
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Current;
            }
            set
            {
                AccessPrimary.Current = value;

                if (value != null)
                {
                    MaintOrderTaskList = value.MaintOrderTask_MaintOrder?.ToList();
                    SelectedMaintOrderTask = MaintOrderTaskList?.FirstOrDefault();
                    MaintOrderAssignmentList = value.MaintOrderAssignment_MaintOrder?.ToList();
                }
                else
                {
                    MaintOrderTaskList = null;
                    MaintOrderAssignmentList = null;
                    SelectedMaintOrderTask = null;
                }

                if (InitState == ACInitState.Destructed)
                    return;

                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Gets the maintenance order list.
        /// </summary>
        /// <value>The maintenance order list.</value>
        [ACPropertyList(603, nameof(MaintOrder), Description = "The list of maintenance orders resulting from the last search (Search / SearchFilter), including the filters of the derived BSO.")]
        public IEnumerable<MaintOrder> MaintOrderList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        public virtual List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get;
        }

        public virtual int NavigationQueryTakeCount
        {
            get
            {
                return 50;
            }
        }

        private MaintOrderAssignment _SelectedMaintOrderAssignment;
        [ACPropertySelected(9999, "Assignment", Description = "The currently selected assignment (MaintOrderAssignment: user or group the order is assigned to).")]
        public MaintOrderAssignment SelectedMaintOrderAssignment
        {
            get
            {
                return _SelectedMaintOrderAssignment;
            }
            set
            {
                _SelectedMaintOrderAssignment = value;
                OnPropertyChanged();
            }
        }

        private List<MaintOrderAssignment> _MaintOrderAssignmentList;
        [ACPropertyList(9999, "Assignment", Description = "List of the assignments (MaintOrderAssignment) of the current maintenance order.")]
        public List<MaintOrderAssignment> MaintOrderAssignmentList
        {
            get
            {
                return _MaintOrderAssignmentList;
            }
            set
            {
                _MaintOrderAssignmentList = value;
                OnPropertyChanged();
            }
        }

        private MaintOrderTask _SelectedMaintOrderTask;
        [ACPropertySelected(9999, "OrderTasks", Description = "The currently selected task (MaintOrderTask) of the current maintenance order. StartMaintenanceTask/EndMaintenanceTask operate on this task.")]
        public virtual MaintOrderTask SelectedMaintOrderTask
        {
            get => _SelectedMaintOrderTask;
            set
            {
                _SelectedMaintOrderTask = value;
                OnPropertyChanged();
            }
        }

        private List<MaintOrderTask> _MaintOrderTaskList;

        [ACPropertyList(9999, "OrderTasks", Description = "List of the tasks (MaintOrderTask) of the current maintenance order.")]
        public List<MaintOrderTask> MaintOrderTaskList
        {
            get => _MaintOrderTaskList;
            set
            {
                _MaintOrderTaskList = value;
                OnPropertyChanged();
            }
        }

        public override IAccessNav AccessNav { get { return AccessPrimary; } }

        private core.datamodel.ACClass _CurrentComponentFilter;
        [ACPropertyInfo(9999, "", "en{'Object'}de{'Objekt'}", Description = "The technical component (ACClass) that is used to filter the order list. Setting it refreshes CurrentObjectFilter.")]
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
        [ACPropertyInfo(9999, "", "en{'Facility'}de{'Anlage'}", Description = "The facility that is used to filter the order list. Setting it refreshes CurrentObjectFilter.")]
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

        [ACPropertyInfo(9999, "", "en{'Object'}de{'Objekt'}", Description = "Caption of the active object filter: the facility caption if a facility filter is set, otherwise the ACUrl of the component filter.")]
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

        #endregion

        #region Methods

        protected virtual IQueryable<MaintOrder> _AccessPrimary_NavSearchExecuting(IQueryable<MaintOrder> result)
        {
            return result;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("MaintOrder", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves the changes on the current maintenance order to the database.")]
        public async Task Save()
        {
            await OnSave();
        }

        /// <summary>
        /// Determines whether [is enabled save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand("MaintOrder", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Reverts the unsaved changes on the current maintenance order.")]
        public void UndoSave()
        {
            OnUndoSave();
        }

        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodCommand("MaintOrder", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Executes the search on the MaintOrder entity and refreshes the MaintOrderList property. Derived classes extend the search with state, facility/component and assignment filters.")]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("MaintOrderList");
        }

        [ACMethodInfo("", "en{'Search'}de{'Suchen'}", 9999, Description = "Clears the current maintenance order and re-executes the search with the active filters (order state, facility/component, assignment).")]
        public void SearchFilter()
        {
            CurrentMaintOrder = null;

            //if (CurrentMaintOrderStateFilter != null && CurrentComponentFilter == null)
            //{
            //    if (_ACQueryDefinition.ACFilterColumns.Count != 1 || _ACQueryDefinition.ACFilterColumns.FirstOrDefault().PropertyName != "MDMaintOrderState\\MDMaintOrderStateIndex"
            //        || _ACQueryDefinition.ACFilterColumns.FirstOrDefault().SearchWord != CurrentMaintOrderStateFilter.MDMaintOrderStateIndex.ToString())
            //    {
            //        _ACQueryDefinition.ClearFilter(true);
            //        _ACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDMaintOrderState\\MDMaintOrderStateIndex", Global.LogicalOperators.equal,
            //            Global.Operators.and, CurrentMaintOrderStateFilter.MDMaintOrderStateIndex.ToString(), true));
            //    }
            //}
            //else if (CurrentMaintOrderStateFilter == null && CurrentComponentFilter != null)
            //{
            //    if (_ACQueryDefinition.ACFilterColumns.Count != 1 || _ACQueryDefinition.ACFilterColumns.FirstOrDefault().PropertyName != "VBiPAACClassID"
            //        || _ACQueryDefinition.ACFilterColumns.FirstOrDefault().SearchWord != CurrentComponentFilter.ACClassID.ToString())
            //    {
            //        _ACQueryDefinition.ClearFilter(true);
            //        _ACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "VBiPAACClassID", Global.LogicalOperators.equal,
            //            Global.Operators.and, CurrentComponentFilter.ACClassID.ToString(), true));
            //    }
            //}
            //else if (CurrentMaintOrderStateFilter != null && CurrentComponentFilter != null)
            //{
            //    bool rebuildACQuery = false;
            //    if (_ACQueryDefinition.ACFilterColumns.Count != 2)
            //        rebuildACQuery = true;
            //    else
            //    {
            //        ACFilterItem state = _ACQueryDefinition.ACFilterColumns.FirstOrDefault(c => c.PropertyName == "MDMaintOrderState\\MDMaintOrderStateIndex");
            //        if (state == null)
            //            rebuildACQuery = true;
            //        else if (state.SearchWord != CurrentMaintOrderStateFilter.MDMaintOrderStateIndex.ToString())
            //            rebuildACQuery = true;
            //        if (!rebuildACQuery)
            //        {
            //            ACFilterItem acClassID = _ACQueryDefinition.ACFilterColumns.FirstOrDefault(c => c.PropertyName == "VBiPAACClassID");
            //            if (acClassID == null)
            //                rebuildACQuery = true;
            //            else if (acClassID.SearchWord != CurrentComponentFilter.ACClassID.ToString())
            //                rebuildACQuery = true;
            //        }
            //    }
            //    if (rebuildACQuery)
            //    {
            //        _ACQueryDefinition.ClearFilter(true);
            //        _ACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "VBiPAACClassID", Global.LogicalOperators.equal,
            //            Global.Operators.and, CurrentComponentFilter.ACClassID.ToString(), true));
            //        _ACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDMaintOrderState\\MDMaintOrderStateIndex", Global.LogicalOperators.equal,
            //            Global.Operators.and, CurrentMaintOrderStateFilter.MDMaintOrderStateIndex.ToString(), true));
            //    }
            //}
            //else
            //{
            //    _ACQueryDefinition.ClearFilter(true);
            //}
            Search();
        }

        [ACMethodInfo("", "en{'Choose Object'}de{'Objekt auswählen'}", 9999, Description = "Opens the MaintOrderEntity dialog in which a facility or a technical component can be selected as filter for the order list. The selection is taken over into CurrentFacilityFilter or CurrentComponentFilter.")]
        public async Task ChooseComponent()
        {
            if (!IsEnabledChooseComponent())
                return;

            await ShowDialogAsync(this, "MaintOrderEntity");

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

        [ACMethodInfo("", "en{'Clear Object'}de{'Objekt löschen'}", 9999, Description = "Removes the facility and component filter from the order list.")]
        public void ClearChosenComponent()
        {
            CurrentFacilityFilter = null;
            CurrentComponentFilter = null;
        }

        [ACMethodInfo("", Const.Ok, 9999, Description = "Closes the object selection dialog (MaintOrderEntity) and applies the selected facility or component filter.")]
        public void ChooseComponentOK()
        {
            CloseTopDialog();
        }

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Save):
                    result = Save();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(SearchFilter):
                    SearchFilter();
                    return true;
                case nameof(ChooseComponent):
                    result = ChooseComponent();
                    return true;
                case nameof(IsEnabledChooseComponent):
                    result = IsEnabledChooseComponent();
                    return true;
                case nameof(ClearChosenComponent):
                    ClearChosenComponent();
                    return true;
                case nameof(ChooseComponentOK):
                    ChooseComponentOK();
                    return true;
                    
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                #region Search
                case nameof(Search):
                    return new string[] { nameof(InitState) };
                #endregion

                #region Component Selection
                case nameof(SearchFilter):
                    return new string[] { nameof(InitState) };
                case nameof(ChooseComponent):
                case nameof(IsEnabledChooseComponent):
                    return new string[] { nameof(InitState) };
                case nameof(ClearChosenComponent):
                    return new string[] { nameof(InitState) };
                case nameof(ChooseComponentOK):
                    return new string[] { nameof(InitState) };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

    }
}
