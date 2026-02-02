// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System.Linq;
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using static gip.core.datamodel.Global;
using System.Collections.Generic;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Business object for managing laboratory order templates.
    /// This class provides functionality to create, manage, and perform mass updates on laboratory order templates,
    /// which serve as blueprints for creating actual laboratory orders.
    /// Usually contains data about laboratory order like a material and positions with measurement tags.
    /// To search records enter the search string in the SearchWord property.
    /// The database result is copied to the LabOrderList property.
    /// Then call NavigateFirst() method to set CurrentLabOrder with the first record in the list.
    /// The New() method creates a new record and assigns the new entity object to the CurrentLabOrder property..
    /// CurrentLabOrder is used to display and edit the currently selected record.
    /// The template name should be written to the property CurrentLabOrder.TemplateName.
    /// Then the material of the template must be selected in the property CurrentLabOrder.Material.
    /// Laboratory order template test parameters and results are presented in the positions list (LabOrderPosList).
    /// The property CurrentLabOrderPos is used to display and the edit currently selected labotory order template position.
    /// To add a new position call the method NewLabOrderPos().
    /// After a new position was added, the properties as MDLabTag, ValueMinMin, ValueMin, ValueMax, ValueMaxMax, ReferenceValue needs to be setted.
    /// To remove position, select the target position and then call the method DeleteLabOrderPos().
    /// After all call the Save() method to save changes to the database.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Lab Order Template'}de{'Laborauftrag Vorlage'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + "LabOrderTemplate",
                Description = @"Business object for managing laboratory order templates.
                                This class provides functionality to create, manage, and perform mass updates on laboratory order templates,
                                which serve as blueprints for creating actual laboratory orders.
                                Usually contains data about laboratory order like a material and positions with measurement tags.
                                To search records enter the search string in the SearchWord property.
                                The database result is copied to the LabOrderList property.
                                Then call NavigateFirst() method to set CurrentLabOrder with the first record in the list.
                                The New() method creates a new record and assigns the new entity object to the CurrentLabOrder property..
                                CurrentLabOrder is used to display and edit the currently selected record.
                                The template name should be written to the property CurrentLabOrder.TemplateName.
                                Then the material of the template must be selected in the property CurrentLabOrder.Material.
                                Laboratory order template test parameters and results are presented in the positions list (LabOrderPosList).
                                The property CurrentLabOrderPos is used to display and the edit currently selected labotory order template position.
                                To add a new position call the method NewLabOrderPos().
                                After a new position was added, the properties as MDLabTag, ValueMinMin, ValueMin, ValueMax, ValueMaxMax, ReferenceValue needs to be setted.
                                To remove position, select the target position and then call the method DeleteLabOrderPos().
                                After all call the Save() method to save changes to the database.")]
    [ACQueryInfo(Const.PackName_VarioMaterial, Const.QueryPrefix + "LabOrderTemplate", "en{'Lab Order Template'}de{'Laborauftrag Vorlage'}", typeof(LabOrder), LabOrder.ClassName, "LabOrderTypeIndex", "LabOrderNo")]
    public class BSOLabOrderTemplate : BSOLabOrderBase
    {
        #region c'tors

        public BSOLabOrderTemplate(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            var b = await base.ACDeInit(deleteACClassTask);
            return b;
        }

        #endregion

        #region Properties
        public override IAccessNav AccessNav { get { return AccessPrimary; } }


        #region Properties -> AccessPrimary

        /// <summary>
        /// Gets the type of laboratory order that this business object manages.
        /// Returns GlobalApp.LabOrderType.Template to indicate this BSO specifically handles laboratory order templates.
        /// This filter ensures that only template-type laboratory orders are processed by this business object.
        /// </summary>
        public override GlobalApp.LabOrderType FilterLabOrderType
        {
            get
            {
                return GlobalApp.LabOrderType.Template;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// <summary>
        /// Gets or sets the selected laboratory order template.
        /// This property represents the currently selected template from the navigation list.
        /// Used for UI binding and interaction with the selected template record.
        /// </summary>
        /// </summary>
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

        /// <summary>
        /// Gets or sets the current laboratory order template that is being viewed or edited.
        /// This property represents the currently active template from the navigation list and is used 
        /// for UI binding and interaction with the selected template record. When a new template is 
        /// created via the New() method, this property is automatically set to the newly created instance.
        /// The current template's properties such as TemplateName and Material should be configured 
        /// through this property. Laboratory order template positions are accessible through the 
        /// LabOrderPosList property when a template is set as current.
        /// </summary>
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

        /// <summary>
        /// Executes custom filtering logic during the primary navigation search for laboratory order templates.
        /// This method is called during the search process to apply additional filtering criteria to the LabOrder query.
        /// It first calls the base implementation to apply standard filtering, then applies value-based filtering 
        /// if enabled through the IsEnabledApplyValueFilter() method. This allows for filtering templates based 
        /// on specific value ranges or criteria beyond the standard search functionality.
        /// </summary>
        public override IQueryable<LabOrder> LabOrder_AccessPrimary_NavSearchExecuting(IQueryable<LabOrder> result)
        {
            result = base.LabOrder_AccessPrimary_NavSearchExecuting(result);
            if (IsEnabledApplyValueFilter())
            {
            }
            return result;
        }

        private double? _UpdateWithValue;
        /// <summary>
        /// Gets or sets the value used for mass updates on laboratory order template positions.
        /// This property holds the new value that will be applied to matching positions across all
        /// templates during mass update operations. The value is used in conjunction with filtering
        /// criteria (FilterValueFrom, FilterValueTo, SelectedLabTag, ValueFilterFieldType) to
        /// determine which positions should be updated with this value.
        /// </summary>
        [ACPropertyInfo(612, "", "en{'Value for mass update'}de{'Massenaktualisierungswert'}", 
                        Description = @"Gets or sets the value used for mass updates on laboratory order template positions.
                                        This property holds the new value that will be applied to matching positions across all
                                        templates during mass update operations. The value is used in conjunction with filtering
                                        criteria (FilterValueFrom, FilterValueTo, SelectedLabTag, ValueFilterFieldType) to
                                        determine which positions should be updated with this value.")]
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

        /// <summary>
        /// Creates a new laboratory order template record and initializes it with default values.
        /// This method generates a unique order number, sets the template type, assigns the default state,
        /// and adds the new template to the navigation list. The newly created template becomes the current
        /// selection for immediate editing. Properties like TemplateName and Material should be configured
        /// after calling this method, and positions can be added using NewLabOrderPos().
        /// </summary>
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

        /// <summary>
        /// Performs mass updates on laboratory order template positions based on filtering criteria.
        /// This method iterates through all laboratory order templates in the navigation list and updates
        /// positions that match the selected laboratory tag and fall within the specified value range
        /// (FilterValueFrom to FilterValueTo). The value field to be updated is determined by the
        /// ValueFilterFieldType property (MinMin, Min, Max, or MaxMax), and all matching positions
        /// are updated with the value specified in UpdateWithValue property.
        /// </summary>
        [ACMethodCommand("ValueFilterField", "en{'Mass update'}de{'Massenaktualisierung'}", 503, false,
                          Description = @"Performs mass updates on laboratory order template positions based on filtering criteria.
                                          This method iterates through all laboratory order templates in the navigation list and updates
                                          positions that match the selected laboratory tag and fall within the specified value range
                                          (FilterValueFrom to FilterValueTo). The value field to be updated is determined by the
                                          ValueFilterFieldType property (MinMin, Min, Max, or MaxMax), and all matching positions
                                          are updated with the value specified in UpdateWithValue property.")]
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

        /// <summary>
        /// Determines whether mass update operations on laboratory order template values are enabled.
        /// Returns true when all required conditions are met: filter value range is specified (FilterValueFrom and FilterValueTo),
        /// an update value is provided (UpdateWithValue), a value field type is selected (SelectedValueFilterField),
        /// and a laboratory tag is selected (SelectedLabTag). This method is used to validate input before executing
        /// the MassUpdateOnValues() method to ensure all necessary parameters are available for the mass update operation.
        /// </summary>
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