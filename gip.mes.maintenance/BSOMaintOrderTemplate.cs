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
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance templates'}de{'Wartungsvorlagen'}", Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, false, true)]
    public class BSOMaintOrderTemplate : ACBSOvbNav
    {
        #region c'tors

        public BSOMaintOrderTemplate(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        public const string Const_FacilityExplorer = "BSOFacilityExplorer_Child";
        public const string Const_ComponentSelector = "BSOComponentSelector_Child";

        #endregion

        #region Properties

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
        public virtual MaintOrder SelectedMaintOrderOrder
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

        #region Properties => Facility

        private ACChildItem<BSOFacilityExplorer> _BSOFacilityExplorer_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo("BSOFacilityExplorer_Child", typeof(BSOFacilityExplorer))]
        public ACChildItem<BSOFacilityExplorer> BSOFacilityExplorer_Child
        {
            get
            {
                if (_BSOFacilityExplorer_Child == null)
                    _BSOFacilityExplorer_Child = new ACChildItem<BSOFacilityExplorer>(this, Const_FacilityExplorer);
                return _BSOFacilityExplorer_Child;
            }
        }


        #endregion

        #region Properties => MaintACClass

        #region ComponentSelector

        private ACChildItem<BSOComponentSelector> _BSOComponentSelector_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo("BSOComponentSelector_Child", typeof(BSOComponentSelector))]
        public ACChildItem<BSOComponentSelector> BSOComponentSelector_Child
        {
            get
            {
                if (_BSOComponentSelector_Child == null)
                    _BSOComponentSelector_Child = new ACChildItem<BSOComponentSelector>(this, Const_ComponentSelector);
                return _BSOComponentSelector_Child;
            }
        }

        private ACClassInfoWithItems.VisibilityFilters _ComponentTypeFilter;
        [ACPropertyInfo(999)]
        public ACClassInfoWithItems.VisibilityFilters ComponentTypeFilter
        {
            get
            {
                if (_ComponentTypeFilter == null)
                    _ComponentTypeFilter = new ACClassInfoWithItems.VisibilityFilters() { IncludeTypes = new List<Type> { typeof(PAClassAlarmingBase), typeof(ApplicationManager) } };
                return _ComponentTypeFilter;
            }
        }

        public override IAccessNav AccessNav => throw new NotImplementedException();

        #endregion

        #endregion



        #endregion

        #region Methods

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(nameof(MaintOrder), "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedMaintOrder", Global.ACKinds.MSMethodPrePost)]
        public virtual void New()
        {
            if (!PreExecute("New"))
                return;
            if (AccessPrimary == null)
                return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(MaintOrder), nameof(MaintOrder.MaintOrderNo), MaintOrder.FormatNewNo, this);
            var newMaintOrder = MaintOrder.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.MaintOrder.Add(newMaintOrder);
            ACState = Const.SMNew;
            AccessPrimary.NavList.Add(newMaintOrder);
            //CurrentLabOrder = newMaintOrder;
            OnPropertyChanged(nameof(MaintOrderList));
            PostExecute("New");
        }

        /// <summary>
        /// Determines whether [is enabled new].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new]; otherwise, <c>false</c>.</returns>
        public virtual bool IsEnabledNew()
        {
            return true;
        }




        #endregion
    }
}
