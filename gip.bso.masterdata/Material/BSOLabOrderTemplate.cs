// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using static gip.core.datamodel.Global;
using System.Collections.Generic;

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
            return b;
        }

        #endregion


        #region Properties
        public override IAccessNav AccessNav { get { return AccessPrimary; } }


        #region Properties -> AccessPrimary

        public override GlobalApp.LabOrderType FilterLabOrderType
        {
            get
            {
                return GlobalApp.LabOrderType.Template;
            }
        }

        #endregion

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
            if (!PreExecute(nameof(New)))
                return;
            if (AccessPrimary == null)
                return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(LabOrder), LabOrder.NoColumnName, LabOrder.FormatNewNo, this);
            var newLabOrder = LabOrder.NewACObject(DatabaseApp, null, secondaryKey);
            newLabOrder.LabOrderTypeIndex = (short)GlobalApp.LabOrderType.Template;
            newLabOrder.MDLabOrderState = DatabaseApp.MDLabOrderState.FirstOrDefault(c => c.IsDefault);
            DatabaseApp.LabOrder.Add(newLabOrder);
            ACState = Const.SMNew;
            AccessPrimary.NavList.Add(newLabOrder);
            CurrentLabOrder = newLabOrder;
            OnPropertyChanged(nameof(LabOrderList));
            OnPropertyChanged(nameof(LabOrderPosList));
            PostExecute(nameof(New));
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

        //public bool IsEnabledUpdatePos()
        //{
        //    return SelectedLabOrderPos != null;
        //}

        //public void UpdatePos()
        //{
        //    OnPropertyChanged(nameof(SelectedLabOrderPos));
        //}

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(MassUpdateOnValues):
                    MassUpdateOnValues();
                    return true;
                case nameof(IsEnabledMassUpdateOnValues):
                    result = IsEnabledMassUpdateOnValues();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #endregion
    }
}