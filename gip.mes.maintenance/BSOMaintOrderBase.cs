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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'BSOMaintOrderBase'}de{'BSOMaintOrderBase'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public abstract class BSOMaintOrderBase : ACBSOvbNav
    {
        public BSOMaintOrderBase(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        private ACQueryDefinition _ACQueryDefinition;

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
                }
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

        public List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                ACFilterItem basedOnFilter = new ACFilterItem(Global.FilterTypes.filter, nameof(MaintOrder.MaintOrder1_BasedOnMaintOrder), Global.LogicalOperators.equal, Global.Operators.and, null, true);
                aCFilterItems.Add(basedOnFilter);

                ACFilterItem maintOrderNoFilter = new ACFilterItem(FilterTypes.filter, nameof(MaintOrder.MaintOrderNo), LogicalOperators.contains, Operators.and, null, true, true);
                aCFilterItems.Add(maintOrderNoFilter);

                return aCFilterItems;
            }
        }

        public int NavigationQueryTakeCount
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
        public MaintOrderTask SelectedMaintOrderTask
        {
            get => _SelectedMaintOrderTask;
            set
            {
                _SelectedMaintOrderTask = value;
                OnPropertyChanged();
            }
        }

        private List<MaintOrderTask> _MaintOrderTaskList;

        public override IAccessNav AccessNav { get { return AccessPrimary; } }

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




        #region Methods

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

        #endregion

    }
}
