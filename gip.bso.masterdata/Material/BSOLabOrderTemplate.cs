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


        #region Properties
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
                    _AccessPrimary.NavSearchExecuting += LabOrder_AccessPrimary_NavSearchExecuting;
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

        #region Find and Replace
        public override IQueryable<LabOrder> LabOrder_AccessPrimary_NavSearchExecuting(IQueryable<LabOrder> result)
        {
            result = base.LabOrder_AccessPrimary_NavSearchExecuting(result);
            if (IsEnabledApplyValueFilter())
            {
            }
            return result;
        }

        private double? _UpdateWithValue;
        [ACPropertyInfo(612, "", "en{'Value for mass update'}de{'Massenaktualisierungswert'}")]
        public double? UpdateWithValue
        {
            get
            {
                return _UpdateWithValue;
            }
            set
            {
                _UpdateWithValue = value;
                OnPropertyChanged("UpdateWithValue");
            }
        }

        #endregion
        #endregion


        #region Methods

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

        #region Filter
        [ACMethodCommand("ValueFilterField", "en{'Mass update'}de{'Massenaktualisierung'}", 503, false)]
        public void MassUpdateOnValues()
        {
            if (!IsEnabledMassUpdateOnValues())
                return;
            foreach (var labOrder in AccessPrimary.NavList)
            {
                LabOrderPos posToUpdate = null;
                if (ValueFilterFieldType == LOPosValueFieldEnum.MinMin)
                {
                    posToUpdate = labOrder.LabOrderPos_LabOrder.Where(l => l.MDLabTagID == this.SelectedLabTag.MDLabTagID
                                                                    && l.ValueMinMin >= FilterValueFrom.Value
                                                                    && l.ValueMinMin <= FilterValueTo.Value).FirstOrDefault();
                    if (posToUpdate != null)
                        posToUpdate.ValueMinMin = UpdateWithValue;
                }
                else if (ValueFilterFieldType == LOPosValueFieldEnum.Min)
                {
                    posToUpdate = labOrder.LabOrderPos_LabOrder.Where(l => l.MDLabTagID == this.SelectedLabTag.MDLabTagID
                                                                    && l.ValueMin >= FilterValueFrom.Value
                                                                    && l.ValueMin <= FilterValueTo.Value).FirstOrDefault();
                    if (posToUpdate != null)
                        posToUpdate.ValueMin = UpdateWithValue;
                }
                else if (ValueFilterFieldType == LOPosValueFieldEnum.Max)
                {
                    posToUpdate = labOrder.LabOrderPos_LabOrder.Where(l => l.MDLabTagID == this.SelectedLabTag.MDLabTagID
                                                                    && l.ValueMax >= FilterValueFrom.Value
                                                                    && l.ValueMax <= FilterValueTo.Value).FirstOrDefault();
                    if (posToUpdate != null)
                        posToUpdate.ValueMax = UpdateWithValue;
                }
                else //if (ValueFilterFieldType == LOPosValueFieldEnum.MaxMax)
                    posToUpdate = labOrder.LabOrderPos_LabOrder.Where(l => l.MDLabTagID == this.SelectedLabTag.MDLabTagID
                                                                    && l.ValueMaxMax >= FilterValueFrom.Value
                                                                    && l.ValueMaxMax <= FilterValueTo.Value).FirstOrDefault();
                if (posToUpdate != null)
                    posToUpdate.ValueMaxMax = UpdateWithValue;
            }
        }

        public virtual bool IsEnabledMassUpdateOnValues()
        {
            return FilterValueFrom.HasValue
                && FilterValueTo.HasValue
                && UpdateWithValue.HasValue
                && SelectedValueFilterField != null
                && SelectedLabTag != null;
        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "MassUpdateOnValues":
                    MassUpdateOnValues();
                    return true;
                case "IsEnabledMassUpdateOnValues":
                    result = IsEnabledMassUpdateOnValues();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
        #endregion
    }
}
