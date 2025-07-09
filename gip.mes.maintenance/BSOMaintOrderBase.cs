using gip.bso.iplus;
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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Base class for Maintenance plans and orders'}de{'Basisklasse für Wartungspläne und Wartungsaufträge'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public abstract class BSOMaintOrderBase : ACBSOvbNav
    {
        #region c'tors

        public BSOMaintOrderBase(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

            bool done = base.ACDeInit(deleteACClassTask);
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
        [ACPropertyAccessPrimary(999, "MaintOrder")]
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
        [ACPropertySelected(601, nameof(MaintOrder))]
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
        [ACPropertyCurrent(602, nameof(MaintOrder))]
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
                    MaintOrderTaskList = value.MaintOrderTask_MaintOrder.ToList();
                    SelectedMaintOrderTask = MaintOrderTaskList?.FirstOrDefault();
                    MaintOrderAssignmentList = value.MaintOrderAssignment_MaintOrder.ToList();
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
        [ACPropertyList(603, nameof(MaintOrder))]
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
        [ACPropertySelected(9999, "Assignment")]
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
        [ACPropertyList(9999, "Assignment")]
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
        [ACPropertySelected(9999, "OrderTasks")]
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

        [ACPropertyList(9999, "OrderTasks")]
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

        #endregion

        #region Methods

        protected virtual IQueryable<MaintOrder> _AccessPrimary_NavSearchExecuting(IQueryable<MaintOrder> result)
        {
            return result;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("MaintOrder", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
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
        [ACMethodCommand("MaintOrder", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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

        [ACMethodCommand("MaintOrder", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("MaintOrderList");
        }

        [ACMethodInfo("", "en{'Search'}de{'Suchen'}", 9999)]
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

        [ACMethodInfo("", "", 9999)]
        public void ClearChosenComponent()
        {
            CurrentFacilityFilter = null;
            CurrentComponentFilter = null;
        }

        [ACMethodInfo("", Const.Ok, 9999)]
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
                    Save();
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
                    ChooseComponent();
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

        #endregion

    }
}
