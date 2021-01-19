using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;

namespace gip.bso.masterdata
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Lab Order Template'}de{'Laborauftrag Vorlage'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + "LabOrderTemplate")]
    [ACQueryInfo(Const.PackName_VarioMaterial, Const.QueryPrefix + "LabOrderTemplate", "en{'Lab Order Template'}de{'Laborauftrag Vorlage'}", typeof(LabOrder), LabOrder.ClassName, "LabOrderTypeIndex", "LabOrderNo")]
    public class BSOLabOrderTemplate : BSOLabOrderBase
    {
        #region c'tors

        public BSOLabOrderTemplate(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<LabOrder> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "LabOrder")]
        public override ACAccessNav<LabOrder> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + LabOrder.ClassName, ACType.ACIdentifier);
                    short loType = (short)GlobalApp.LabOrderType.Template;
                    bool rebuildACQueryDef = false;
                    if (!navACQueryDefinition.ACFilterColumns.Any())
                    {
                        rebuildACQueryDef = true;
                    }
                    else
                    {
                        int countFoundCorrect = 0;
                        foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            if (filterItem.PropertyName == "LabOrderTypeIndex")
                            {
                                if (filterItem.SearchWord == loType.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal)
                                    countFoundCorrect++;
                            }
                            else if (filterItem.PropertyName == "LabOrderNo")
                            {
                                countFoundCorrect++;
                            }
                        }
                        if (countFoundCorrect < 2)
                            rebuildACQueryDef = true;
                    }
                    if (rebuildACQueryDef)
                    {
                        navACQueryDefinition.ClearFilter(true);
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "LabOrderTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, loType.ToString(), true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "LabOrderNo", Global.LogicalOperators.equal, Global.Operators.and, null, true));
                        navACQueryDefinition.SaveConfig(true);
                    }

                    _AccessPrimary = navACQueryDefinition.NewAccessNav<LabOrder>("LabOrder", this);
                }
                return _AccessPrimary;
            }
        }

        public override LabOrder SelectedLabOrder
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                SetCurrentSelected(value);
            }
        }

        public override LabOrder CurrentLabOrder
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                SetCurrentSelected(value);
            }
        }

        public override void SetCurrentSelected(LabOrder value)
        {
            if (AccessPrimary == null)
                return;
            if (value != CurrentLabOrder)
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentLabOrder");
                OnPropertyChanged("LabOrderPosList");
            }
            if (value != SelectedLabOrder)
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedLabOrder");
                OnPropertyChanged("LabOrderPosList");
            }
        }

        public override void New()
        {
            if (!PreExecute("New"))
                return;
            if (AccessPrimary == null)
                return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(LabOrder), LabOrder.NoColumnName, LabOrder.FormatNewNo, this);
            var newLabOrder = LabOrder.NewACObject(DatabaseApp, null, secondaryKey);
            newLabOrder.LabOrderTypeIndex = (short)GlobalApp.LabOrderType.Template;
            newLabOrder.MDLabOrderState = DatabaseApp.MDLabOrderState.FirstOrDefault(c => c.IsDefault);
            DatabaseApp.LabOrder.AddObject(newLabOrder);
            ACState = Const.SMNew;
            AccessPrimary.NavList.Add(newLabOrder);
            CurrentLabOrder = newLabOrder;
            OnPropertyChanged("LabOrderList");
            OnPropertyChanged("LabOrderPosList");
            PostExecute("New");
        }
    }
}
