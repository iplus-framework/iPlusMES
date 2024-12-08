// ***********************************************************************
// Assembly         : gip.bso.purchasing
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 29.01.2018
// ***********************************************************************
// <copyright file="BSOInOrder.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.purchasing
{
    /// <summary>
    /// Version 3
    /// Folgende alte Masken sind in diesem BSO enthalten:
    /// 1. Warenannahme-&gt;Bestellungen
    /// 2. Warenannahme-&gt;Bestellpositionen
    /// Neue Masken:
    /// 1. Bestellverwaltung
    /// TODO: Betroffene Tabellen: InOrder, InOrderPos
    /// </summary>
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Purchase Order'}de{'Bestellung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + InOrder.ClassName)]
    [ACQueryInfo(Const.PackName_VarioPurchase, Const.QueryPrefix + "InOpenContractPos", "en{'Open Contract lines'}de{'Offene Kontraktpositionen'}", typeof(InOrderPos), InOrderPos.ClassName, MDDelivPosState.ClassName + "\\MDDelivPosStateIndex", "Material\\MaterialNo,TargetDeliveryDate")]
    public class BSOInOrder : ACBSOvbNav
    {
        #region private
        private UserSettings CurrentUserSettings { get; set; }
        #endregion

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOInOrder"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOInOrder(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
        }

        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _InDeliveryNoteManager = ACInDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_InDeliveryNoteManager == null)
                throw new Exception("InDeliveryNoteManager not configured");

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            CurrentUserSettings = DatabaseApp.UserSettings.Where(c => c.VBUserID == Root.Environment.User.VBUserID).FirstOrDefault();

            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACInDeliveryNoteManager.DetachACRefFromServiceInstance(this, _InDeliveryNoteManager);
            _InDeliveryNoteManager = null;
            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            this._AccessInOrderPos = null;
            this._ChangeTargetQuantity = null;
            this._CurrentCompanyMaterialPickup = null;
            this._CurrentInOrderPos = null;
            this._CurrentMDUnit = null;
            this._SelectedAvailableCompMaterial = null;
            this._SelectedCompanyMaterialPickup = null;
            this._SelectedInOrderPos = null;
            this._UnSavedUnAssignedContractPos = null;
            this._PartialQuantity = null;
            _SelectedFilterMaterial = null;

            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

            bool result = base.ACDeInit(deleteACClassTask);

            if (_AccessInOrderPos != null)
            {
                _AccessInOrderPos.ACDeInit(false);
                _AccessInOrderPos = null;
            }
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            if (_AccessOpenContractPos != null)
            {
                _AccessOpenContractPos.ACDeInit(false);
                _AccessOpenContractPos = null;
            }
            if (_AccessFilterMaterial != null)
            {
                _AccessFilterMaterial.ACDeInit(false);
                _AccessFilterMaterial = null;
            }

            return result;
        }

        #endregion

        #region Filters

        #region Properties - > FilterMaterial

        ACAccess<Material> _AccessFilterMaterial;
        [ACPropertyAccess(9999, "FilterMaterial")]
        public ACAccess<Material> AccessFilterMaterial
        {
            get
            {
                if (_AccessFilterMaterial == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Material.ClassName, ACType.ACIdentifier);
                    acQueryDefinition.TakeCount = 2000;
                    acQueryDefinition.ClearFilter();
                    _AccessFilterMaterial = acQueryDefinition.NewAccess<Material>("FilterMaterial", this);
                    BuildInOrderMaterialFilterColumns(_AccessFilterMaterial.NavACQueryDefinition);
                    _AccessFilterMaterial.NavSearch();
                }
                return _AccessFilterMaterial;
            }
        }

        private List<string> materialGroups;

        public virtual void BuildInOrderMaterialFilterColumns(ACQueryDefinition acQueryDefinition)
        {
            if (materialGroups == null)
                materialGroups = DatabaseApp.InOrderPos.Select(c => c.Material.MDMaterialGroup.MDKey).Distinct().OrderBy(c => c).ToList();
            string filterPropertyName = @"MDMaterialGroup\MDKey";
            if (!acQueryDefinition.ACFilterColumns.Any(c => c.PropertyName == filterPropertyName))
            {
                acQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true));
                foreach (var materialGroupName in materialGroups)
                {
                    acQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, filterPropertyName, Global.LogicalOperators.equal, Global.Operators.or, materialGroupName, true));
                }
                acQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true));
            }
        }

        [ACPropertyInfo(9999, "FilterMaterial")]
        public IEnumerable<Material> FilterMaterialList
        {
            get
            {
                return AccessFilterMaterial.NavList;
            }
        }

        private Material _SelectedFilterMaterial;
        [ACPropertySelected(9999, "FilterMaterial", "en{'Material'}de{'Material'}")]
        public Material SelectedFilterMaterial
        {
            get
            {
                return _SelectedFilterMaterial;
            }
            set
            {
                if (_SelectedFilterMaterial != value)
                {
                    _SelectedFilterMaterial = value;
                    OnPropertyChanged("SelectedFilterMaterial");
                }
            }
        }

        #endregion

        #region Properties -> FilterOrderState

        public gip.mes.datamodel.MDInOrderState.InOrderStates? FilterOrderState
        {
            get
            {
                if (SelectedFilterOrderState == null) return null;
                return (gip.mes.datamodel.MDInOrderState.InOrderStates)Enum.Parse(typeof(gip.mes.datamodel.MDInOrderState.InOrderStates), SelectedFilterOrderState.Value.ToString());
            }
        }


        private ACValueItem _SelectedFilterOrderState;
        [ACPropertySelected(9999, "FilterOrderState", "en{'Order state'}de{'Auftragsstatus'}")]
        public ACValueItem SelectedFilterOrderState
        {
            get
            {
                return _SelectedFilterOrderState;
            }
            set
            {
                if (_SelectedFilterOrderState != value)
                {
                    _SelectedFilterOrderState = value;
                    OnPropertyChanged("SelectedFilterOrderState");
                }
            }
        }


        private ACValueItemList _FilterOrderStateList;
        [ACPropertyList(9999, "FilterOrderState")]
        public ACValueItemList FilterOrderStateList
        {
            get
            {
                if (_FilterOrderStateList == null)
                {
                    _FilterOrderStateList = new ACValueItemList("InOrderStatesList");
                    _FilterOrderStateList.AddRange(DatabaseApp.MDInOrderState.ToList().Select(x => new ACValueItem(x.MDInOrderStateName, x.MDInOrderStateIndex, null)).ToList());
                }
                return _FilterOrderStateList;
            }
        }

        [ACPropertyInfo(300, nameof(FilterNo), "en{'Order No.'}de{'Auftrag Nr.'}")]
        public string FilterNo
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(InOrder.NoColumnName);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(InOrder.NoColumnName);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(InOrder.NoColumnName, value);
                    OnPropertyChanged();
                }
            }
        }


        [ACPropertyInfo(301, nameof(FilterComment), ConstApp.Comment)]
        public string FilterComment
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("Comment");
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("Comment");
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>("Comment", value);
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #endregion

        #region Managers

        protected ACRef<ACInDeliveryNoteManager> _InDeliveryNoteManager = null;
        public ACInDeliveryNoteManager InDeliveryNoteManager
        {
            get
            {
                if (_InDeliveryNoteManager == null)
                    return null;
                return _InDeliveryNoteManager.ValueT;
            }
        }

        protected ACRef<ACComponent> _ACFacilityManager = null;
        public FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }

        #endregion

        #region BSO->ACProperty

        #region 1. InOrder
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<InOrder> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, InOrder.ClassName)]
        public ACAccessNav<InOrder> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceColumnsIfDifferent(NavigationqueryDefaultFilter, NavigationqueryDefaultSort);
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<InOrder>(InOrder.ClassName, this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessPrimary;
            }
        }

        protected virtual List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, InOrder.NoColumnName, Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.filter, "Comment", Global.LogicalOperators.contains, Global.Operators.and, "", false, false)
                };
            }
        }

        protected virtual List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem(InOrder.NoColumnName, Global.SortDirections.descending, true)
                };
            }
        }

        private IQueryable<InOrder> _AccessPrimary_NavSearchExecuting(IQueryable<InOrder> result)
        {
            IQueryable<InOrder> query = result as IQueryable<InOrder>;
            if (query != null)
            {
                query.Include(c => c.MDInOrderType)
                     .Include(c => c.MDInOrderState)
                     .Include(c => c.DistributorCompany)
                     .Include(c => c.BillingCompanyAddress)
                     .Include(c => c.DeliveryCompanyAddress)
                     .Include(c => c.MDTermOfPayment)
                     .Include(c => c.MDDelivType)
                    .Include("InOrderPos_InOrder")
                    .Include("InOrderPos_InOrder.Material");
            }
            if (SelectedFilterMaterial != null)
                result = result.Where(c => c.InOrderPos_InOrder.Any(mt => mt.MaterialID == SelectedFilterMaterial.MaterialID));
            if (FilterOrderState.HasValue)
                result = result.Where(x => x.MDInOrderState.MDInOrderStateIndex == (short)FilterOrderState.Value);
            return result;
        }

        /// <summary>
        /// Gets or sets the current in order.
        /// </summary>
        /// <value>The current in order.</value>
        [ACPropertyCurrent(600, InOrder.ClassName, "en{'Inorder'}de{'Bestellung'}")]
        public InOrder CurrentInOrder
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Current = value;
                CurrentInOrderPos = null;
                OnPropertyChanged("CurrentInOrder");
                OnPropertyChanged("InOrderPosList");
                OnPropertyChanged("DeliveryCompanyAddressList");
                OnPropertyChanged("DistributorCompanyList");
                OnPropertyChanged("ContractualCompanyList");
                OnCurrentInOrderChanged();
                ResetAccessTenantCompanyFilter(value);
            }
        }

        /// <summary>
        /// Gets the in order list.
        /// </summary>
        /// <value>The in order list.</value>
        [ACPropertyList(601, InOrder.ClassName)]
        public IEnumerable<InOrder> InOrderList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected in order.
        /// </summary>
        /// <value>The selected in order.</value>
        [ACPropertySelected(602, InOrder.ClassName, "en{'Inorder'}de{'Bestellung'}")]
        public InOrder SelectedInOrder
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedInOrder");
            }
        }

        #region Properties -> InOrderPos -> MDUnit

        /// <summary>
        /// Gets the MU quantity unit list.
        /// </summary>
        /// <value>The MU quantity unit list.</value>
        [ACPropertyList(603, MDUnit.ClassName)]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                if (CurrentInOrderPos == null || CurrentInOrderPos.Material == null)
                    return null;
                return CurrentInOrderPos.Material.MDUnitList;
            }
        }

        MDUnit _CurrentMDUnit;
        /// <summary>
        /// Gets or sets the current MU quantity unit.
        /// </summary>
        /// <value>The current MU quantity unit.</value>
        [ACPropertyCurrent(604, MDUnit.ClassName, "en{'New Unit'}de{'Neue Einheit'}")]
        public MDUnit CurrentMDUnit
        {
            get
            {
                return _CurrentMDUnit;
            }
            set
            {
                _CurrentMDUnit = value;
                if (CurrentInOrderPos != null && CurrentInOrderPos.MDUnit != value)
                {
                    CurrentInOrderPos.MDUnit = value;
                    OnPropertyChanged("CurrentInOrderPos");
                }
                OnPropertyChanged("CurrentMDUnit");
            }
        }

        #endregion

        Nullable<double> _ChangeTargetQuantity = null;
        [ACPropertyInfo(605, "", "en{'New Target Quantity'}de{'Neue Sollmenge'}")]
        public Nullable<double> ChangeTargetQuantity
        {
            get
            {
                return _ChangeTargetQuantity;
            }
            set
            {
                _ChangeTargetQuantity = value;
                if (_ChangeTargetQuantity.HasValue && (_ChangeTargetQuantity.Value > 0) && CurrentInOrderPos != null && CurrentInOrderPos.Material != null)
                {
                    //CurrentInOrderPos.TargetQuantityUOM = CurrentInOrderPos.Material.ConvertToBaseQuantity(_ChangeTargetQuantity.Value, CurrentInOrderPos.MDUnit);
                    //CurrentInOrderPos.TargetQuantity = CurrentInOrderPos.Material.ConvertQuantity(CurrentInOrderPos.TargetQuantityUOM, CurrentInOrderPos.Material.BaseMDUnit, CurrentInOrderPos.MDUnit);
                    CurrentInOrderPos.TargetQuantity = _ChangeTargetQuantity.Value;
                }
                _ChangeTargetQuantity = null;
                OnPropertyChanged("ChangeTargetQuantity");
            }
        }

        #region DistributorCompany
        /// <summary>
        /// Gets the current distrbutor company address.
        /// </summary>
        /// <value>The current distrbutor company address.</value>
        [ACPropertyCurrent(606, "DistrbutorCompanyAddress", "en{'Adress'}de{'Adresse'}")]
        public CompanyAddress CurrentDistrbutorCompanyAddress
        {
            get
            {
                if (CurrentInOrder == null)
                    return null;
                try
                {
                    if (!CurrentInOrder.DistributorCompanyReference.IsLoaded)
                        CurrentInOrder.DistributorCompanyReference.Load();
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOInOrder", "CurrentDistributorCompanyAddress", msg);

                    if (CurrentInOrder.DistributorCompany == null)
                        return null;
                }
                try
                {
                    if (!CurrentInOrder.DistributorCompany.CompanyAddress_Company_IsLoaded)
                        CurrentInOrder.DistributorCompany.CompanyAddress_Company.AutoLoad(CurrentInOrder.DistributorCompany.CompanyAddress_CompanyReference, CurrentInOrder);
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOInOrder", "CurrentDistributorCompanyAddress(10)", msg);
                    return null;
                }

                try
                {
                    return CurrentInOrder.DistributorCompany.CompanyAddress_Company.Where(c => c.IsHouseCompanyAddress).FirstOrDefault();
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOInOrder", "CurrentDistributorCompanyAddress(20)", msg);
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the distributor company list.
        /// </summary>
        /// <value>The distributor company list.</value>
        [ACPropertyList(607, "DistributorCompany")]
        public IEnumerable<Company> DistributorCompanyList
        {
            get
            {
                return DatabaseApp.Company
                    .Where(c => c.IsDistributor)
                    .OrderBy(c => c.CompanyName);
            }
        }
        #endregion

        #region DeliveryCompany
        /// <summary>
        /// Gets the current delivery company address.
        /// </summary>
        /// <value>The current delivery company address.</value>
        [ACPropertyCurrent(608, "DeliveryCompanyAddress", ConstApp.DeliveryCompanyAddress)]
        public CompanyAddress CurrentDeliveryCompanyAddress
        {
            get
            {
                if (CurrentInOrder == null)
                    return null;
                return CurrentInOrder.DeliveryCompanyAddress;
            }
        }

        /// <summary>
        /// Gets the delivery company address list.
        /// </summary>
        /// <value>The delivery company address list.</value>
        [ACPropertyList(609, "DeliveryCompanyAddress")]
        public IEnumerable<CompanyAddress> DeliveryCompanyAddressList
        {
            get
            {
                return DatabaseApp.CompanyAddress
                    .Where(c => c.Company.IsOwnCompany && c.IsDeliveryCompanyAddress)
                    .OrderBy(c => c.Name1);
            }
        }
        #endregion

        #region BillingCompany
        /// <summary>
        /// Gets the current billing company address.
        /// </summary>
        /// <value>The current billing company address.</value>
        [ACPropertyCurrent(610, "BillingCompanyAddress", "en{'Billing Address'}de{'Rechnungsadresse'}")]
        public CompanyAddress CurrentBillingCompanyAddress
        {
            get
            {
                if (CurrentInOrder == null)
                    return null;
                try
                {
                    if (!CurrentInOrder.DistributorCompanyReference.IsLoaded)
                        CurrentInOrder.DistributorCompanyReference.Load();
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOInOrder", "CurrentBillingCompanyAddress", msg);

                    if (CurrentInOrder.DistributorCompany == null)
                        return null;
                }
                try
                {
                    if (!CurrentInOrder.DistributorCompany.CompanyAddress_Company_IsLoaded)
                        CurrentInOrder.DistributorCompany.CompanyAddress_Company.AutoLoad(CurrentInOrder.DistributorCompany.CompanyAddress_CompanyReference, CurrentInOrder);
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOInOrder", "CurrentBillingCompanyAddress(10)", msg);
                    return null;
                }

                try
                {
                    return CurrentInOrder.DistributorCompany.CompanyAddress_Company.Where(c => c.IsHouseCompanyAddress).FirstOrDefault();
                }
                catch (Exception e)
                {
                    string msg = e.Message;
                    if (e.InnerException != null && e.InnerException.Message != null)
                        msg += " Inner:" + e.InnerException.Message;

                    Messages.LogException("BSOInOrder", "CurrentBillingCompanyAddress(20)", msg);
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the billing company address list.
        /// </summary>
        /// <value>The billing company address list.</value>
        [ACPropertyList(611, "BillingCompanyAddress")]
        public IEnumerable<CompanyAddress> BillingCompanyAddressList
        {
            get
            {
                return DatabaseApp.CompanyAddress
                    .Where(c => c.Company.IsOwnCompany && c.IsBillingCompanyAddress)
                    .OrderBy(c => c.Name1);
            }
        }
        #endregion
        #endregion

        #region 1.1 InOrderPos
        /// <summary>
        /// The _ access in order pos
        /// </summary>
        ACAccess<InOrderPos> _AccessInOrderPos;
        /// <summary>
        /// Gets the access in order pos.
        /// </summary>
        /// <value>The access in order pos.</value>
        [ACPropertyAccess(691, InOrderPos.ClassName)]
        public ACAccess<InOrderPos> AccessInOrderPos
        {
            get
            {
                if (_AccessInOrderPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + InOrderPos.ClassName) as ACQueryDefinition;
                    _AccessInOrderPos = acQueryDefinition.NewAccess<InOrderPos>(InOrderPos.ClassName, this);
                }
                return _AccessInOrderPos;
            }
        }

        /// <summary>
        /// The _ current in order pos
        /// </summary>
        InOrderPos _CurrentInOrderPos;
        /// <summary>
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(612, InOrderPos.ClassName, "en{'Purchase Order Pos.'}de{'Bestellposition'}")]
        public InOrderPos CurrentInOrderPos
        {
            get
            {
                return _CurrentInOrderPos;
            }
            set
            {
                if (_CurrentInOrderPos != null)
                    _CurrentInOrderPos.PropertyChanged -= CurrentInOrderPos_PropertyChanged;
                _CurrentInOrderPos = value;
                if (_CurrentInOrderPos != null)
                    _CurrentInOrderPos.PropertyChanged += CurrentInOrderPos_PropertyChanged;
                CurrentMDUnit = CurrentInOrderPos?.MDUnit;
                OnPropertyChanged("CurrentInOrderPos");
                OnPropertyChanged("MDUnitList");
                OnPropertyChanged("CompanyMaterialPickupList");
                OnPropertyChanged("AvailableCompMaterialList");
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the CurrentInOrderPos control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void CurrentInOrderPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentInOrderPos.MaterialID):
                    {
                        if (CurrentInOrderPos.Material != null && CurrentInOrderPos.Material.BaseMDUnit != null)
                            CurrentInOrderPos.MDUnit = CurrentInOrderPos.Material.BaseMDUnit;
                        OnPropertyChanged(nameof(CurrentInOrderPos));
                        OnPropertyChanged(nameof(MDUnitList));
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(613, InOrderPos.ClassName)]
        public IEnumerable<InOrderPos> InOrderPosList
        {
            get
            {
                if (CurrentInOrder == null)
                    return null;
                if (CurrentInOrder.MDInOrderType.OrderType == GlobalApp.OrderTypes.ReleaseOrder)
                {
                    return CurrentInOrder.InOrderPos_InOrder
                        .Where(c => c.MaterialPosTypeIndex == (int)GlobalApp.MaterialPosTypes.InwardPart && !c.DeliveryNotePos_InOrderPos.Any())
                        .OrderBy(c => c.Sequence);
                }
                else
                {
                    return CurrentInOrder.InOrderPos_InOrder
                        .Where(c => !c.ParentInOrderPosID.HasValue)
                        .OrderBy(c => c.Sequence);
                }
            }
        }

        /// <summary>
        /// The _ selected in order pos
        /// </summary>
        InOrderPos _SelectedInOrderPos;
        /// <summary>
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(614, InOrderPos.ClassName)]
        public InOrderPos SelectedInOrderPos
        {
            get
            {
                return _SelectedInOrderPos;
            }
            set
            {
                _SelectedInOrderPos = value;
                OnPropertyChanged("SelectedInOrderPos");
            }
        }
        #endregion

        #region 1.1.1 CompanyMaterialPickup
        CompanyMaterialPickup _CurrentCompanyMaterialPickup;
        [ACPropertyCurrent(615, "CompanyMaterialPickup")]
        public CompanyMaterialPickup CurrentCompanyMaterialPickup
        {
            get
            {
                return _CurrentCompanyMaterialPickup;
            }
            set
            {
                if (_CurrentCompanyMaterialPickup != value)
                {
                    _CurrentCompanyMaterialPickup = value;
                    OnPropertyChanged("CurrentCompanyMaterialPickup");
                }
            }
        }

        CompanyMaterialPickup _SelectedCompanyMaterialPickup;
        [ACPropertySelected(616, "CompanyMaterialPickup", "en{'Pick-up Master'}de{'Abhol-/Verladestamm'}")]
        public CompanyMaterialPickup SelectedCompanyMaterialPickup
        {
            get
            {
                return _SelectedCompanyMaterialPickup;
            }
            set
            {
                _SelectedCompanyMaterialPickup = value;
                OnPropertyChanged("SelectedCompanyMaterialPickup");
            }
        }

        [ACPropertyList(617, "CompanyMaterialPickup")]
        public IEnumerable<CompanyMaterialPickup> CompanyMaterialPickupList
        {
            get
            {
                if (CurrentInOrderPos == null)
                    return null;
                return CurrentInOrderPos.CompanyMaterialPickup_InOrderPos;
            }
        }

        CompanyMaterial _SelectedAvailableCompMaterial;
        [ACPropertySelected(618, "AvailableCompMatPickups", "en{'Available Ext. Material'}de{'Verfügbares externes Material'}")] //[ACPropertyList(9999, "AvailableCompMatPickups")]
        public CompanyMaterial SelectedAvailableCompMaterial
        {
            get
            {
                return _SelectedAvailableCompMaterial;
            }
            set
            {
                _SelectedAvailableCompMaterial = value;
            }
        }

        [ACPropertyList(619, "AvailableCompMatPickups")]
        public IEnumerable<CompanyMaterial> AvailableCompMaterialList
        {
            get
            {
                if (CurrentInOrder == null || CurrentInOrderPos == null || CurrentInOrder.DeliveryCompanyAddress == null)
                    return null;
                return DatabaseApp.CompanyMaterial.Where(c => c.CompanyID == CurrentInOrder.DeliveryCompanyAddressID
                                                        && c.MaterialID == CurrentInOrderPos.MaterialID
                                                        && c.CMTypeIndex == (short)GlobalApp.CompanyMaterialTypes.Pickup);
            }
        }
        #endregion

        #region 1.2 OpenContractPos
        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<InOrderPos> _AccessOpenContractPos;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(692, "OpenContractPos")]
        public ACAccessNav<InOrderPos> AccessOpenContractPos
        {
            get
            {
                if (_AccessOpenContractPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDef = Root.Queries.CreateQuery(null, Const.QueryPrefix + "InOpenContractPos", ACType.ACIdentifier);
                    if (acQueryDef != null)
                    {
                        acQueryDef.CheckAndReplaceColumnsIfDifferent(OpenContractPosDefaultFilter, OpenContractPosDefaultSort, true, true);
                    }

                    _AccessOpenContractPos = acQueryDef.NewAccessNav<InOrderPos>(InOrderPos.ClassName, this);
                    _AccessOpenContractPos.AutoSaveOnNavigation = false;
                    _AccessOpenContractPos.NavSearch(DatabaseApp);
                }
                return _AccessOpenContractPos;
            }
        }

        protected virtual List<ACFilterItem> OpenContractPosDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "MDDelivPosState\\MDDelivPosStateIndex", Global.LogicalOperators.lessThan, Global.Operators.and, ((short) MDDelivPosState.DelivPosStates.CompletelyAssigned).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.filter, "InOrderPos1_ParentInOrderPos", Global.LogicalOperators.isNull, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, InOrder.ClassName + "\\" + MDInOrderType.ClassName + "\\OrderTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short) GlobalApp.OrderTypes.Contract).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.filter, _CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CTargetDeliveryMaxDateProperty, Global.LogicalOperators.lessThanOrEqual, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, _CMaterialNoProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CMaterialNameProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                };
            }
        }

        protected virtual List<ACSortItem> OpenContractPosDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("Material\\MaterialNo", Global.SortDirections.ascending, true),
                    new ACSortItem("TargetDeliveryDate", Global.SortDirections.ascending, true),
                };
            }
        }

        #region Filter
        public const string _CMaterialNoProperty = Material.ClassName + "\\MaterialNo";
        public const string _CMaterialNameProperty = Material.ClassName + "\\MaterialName1";
        [ACPropertyInfo(713, "Filter", "en{'Material'}de{'Material'}")]
        public string FilterMaterial
        {
            get
            {
                return AccessOpenContractPos.NavACQueryDefinition.GetSearchValue<string>(_CMaterialNoProperty);
            }
            set
            {
                string tmp = AccessOpenContractPos.NavACQueryDefinition.GetSearchValue<string>(_CMaterialNoProperty);
                if (tmp != value)
                {
                    AccessOpenContractPos.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNoProperty, value);
                    AccessOpenContractPos.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNameProperty, value);
                    OnPropertyChanged("FilterMaterial");
                }
            }
        }

        public const string _CTargetDeliveryDateProperty = InOrder.ClassName + "\\" + "TargetDeliveryDate";

        [ACPropertyInfo(618, "", "en{'Contract date from'}de{'Kontraktdatum von'}")]
        public DateTime? FilterDelivDateFrom
        {
            get
            {
                string tmp = AccessOpenContractPos.NavACQueryDefinition.GetSearchValue<string>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessOpenContractPos.NavACQueryDefinition.GetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual);
            }
            set
            {
                string tmp = AccessOpenContractPos.NavACQueryDefinition.GetSearchValue<string>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                {
                    if (value.HasValue)
                    {
                        AccessOpenContractPos.NavACQueryDefinition.SetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual, value.Value);
                        OnPropertyChanged("FilterDelivDateFrom");
                    }
                }
                else
                {
                    if (value.HasValue)
                    {
                        DateTime tmpdt = AccessOpenContractPos.NavACQueryDefinition.GetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual);
                        if (tmpdt != value.Value)
                        {
                            AccessOpenContractPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual, value.Value);
                            OnPropertyChanged("FilterDelivDateFrom");
                        }
                    }
                    else
                    {
                        AccessOpenContractPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual, "");
                        OnPropertyChanged("FilterDelivDateFrom");
                    }
                }
            }
        }

        public const string _CTargetDeliveryMaxDateProperty = InOrder.ClassName + "\\" + "TargetDeliveryMaxDate";
        [ACPropertyInfo(619, "", "en{'Contract date to'}de{'Kontraktdatum bis'}")]
        public DateTime? FilterDelivDateTo
        {
            get
            {
                string tmp = AccessOpenContractPos.NavACQueryDefinition.GetSearchValue<string>(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessOpenContractPos.NavACQueryDefinition.GetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual);
            }
            set
            {
                string tmp = AccessOpenContractPos.NavACQueryDefinition.GetSearchValue<string>(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                {
                    if (value.HasValue)
                    {
                        AccessOpenContractPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual, value.Value);
                        OnPropertyChanged("FilterDelivDateTo");
                    }
                }
                else
                {
                    if (value.HasValue)
                    {
                        DateTime tmpdt = AccessOpenContractPos.NavACQueryDefinition.GetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual);
                        if (tmpdt != value)
                        {
                            AccessOpenContractPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual, value.Value);
                            OnPropertyChanged("FilterDelivDateTo");
                        }
                    }
                    else
                    {
                        AccessOpenContractPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual, "");
                        OnPropertyChanged("FilterDelivDateTo");
                    }
                }
            }
        }
        #endregion



        Nullable<double> _PartialQuantity;
        [ACPropertyInfo(637, "", "en{'Partial Quantity'}de{'Teilmenge'}")]
        public Nullable<double> PartialQuantity
        {
            get
            {
                return _PartialQuantity;
            }
            set
            {
                _PartialQuantity = value;
                OnPropertyChanged("PartialQuantity");
            }
        }


        /// <summary>
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(613, "OpenContractPos")]
        public InOrderPos CurrentOpenContractPos
        {
            get
            {
                if (AccessOpenContractPos == null)
                    return null;
                return AccessOpenContractPos.Current;
            }
            set
            {
                if (AccessOpenContractPos != null)
                    AccessOpenContractPos.Current = value;
                OnPropertyChanged("CurrentOpenContractPos");
            }
        }

        MDDelivPosState _StateCompletelyAssigned = null;
        MDDelivPosState StateCompletelyAssigned
        {
            get
            {
                if (_StateCompletelyAssigned != null)
                    return _StateCompletelyAssigned;
                var queryDelivStateAssigned = DatabaseApp.MDDelivPosState.Where(c => c.MDDelivPosStateIndex == (Int16)MDDelivPosState.DelivPosStates.CompletelyAssigned);
                _StateCompletelyAssigned = queryDelivStateAssigned.FirstOrDefault();
                return _StateCompletelyAssigned;
            }
        }

        protected List<InOrderPos> _UnSavedUnAssignedContractPos = new List<InOrderPos>();

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(614, "OpenContractPos")]
        public IEnumerable<InOrderPos> OpenContractPosList
        {
            get
            {
                if (AccessOpenContractPos == null)
                    return null;
                if (CurrentInOrderPos != null)
                {
                    IEnumerable<InOrderPos> addedPositions = CurrentInOrderPos.InOrderPos_ParentInOrderPos.Where(c => c.EntityState == EntityState.Added
                        && c != null
                        && c.InOrderPos1_ParentInOrderPos != null
                        && c.InOrderPos1_ParentInOrderPos.MDDelivPosState == StateCompletelyAssigned
                        ).Select(c => c.InOrderPos1_ParentInOrderPos);
                    if (addedPositions.Any())
                    {
                        if (_UnSavedUnAssignedContractPos.Any())
                            return AccessOpenContractPos.NavList.Except(addedPositions).Union(_UnSavedUnAssignedContractPos);
                        else
                            return AccessOpenContractPos.NavList.Except(addedPositions);
                    }
                    else if (_UnSavedUnAssignedContractPos.Any())
                    {
                        return AccessOpenContractPos.NavList.Union(_UnSavedUnAssignedContractPos);
                    }
                }
                return AccessOpenContractPos.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(615, "OpenContractPos")]
        public InOrderPos SelectedOpenContractPos
        {
            get
            {
                if (AccessOpenContractPos == null)
                    return null;
                return AccessOpenContractPos.Selected;
            }
            set
            {
                if (AccessOpenContractPos != null)
                    AccessOpenContractPos.Selected = value;
                OnPropertyChanged("SelectedOpenContractPos");
                CurrentOpenContractPos = value;
            }
        }


        [ACPropertyList(616, "TransportMode")]
        public IEnumerable<MDTransportMode> MDTransportModeList
        {
            get
            {
                return DatabaseApp.MDTransportMode.ToList();
            }
        }

        #endregion

        #region Tenant

        #region Tenant -> TenantCompany

        ACAccessNav<Company> _AccessTenantCompany;
        [ACPropertyAccess(100, "TenantCompany")]
        public ACAccessNav<Company> AccessTenantCompany
        {
            get
            {
                if (_AccessTenantCompany == null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "Company", ACType.ACIdentifier);
                    _AccessTenantCompany = navACQueryDefinition.NewAccessNav<Company>("Company", this);
                    SetAccessTenantCompanyFilter(navACQueryDefinition);
                    _AccessTenantCompany.AutoSaveOnNavigation = false;
                }
                return _AccessTenantCompany;
            }
        }

        public void SetAccessTenantCompanyFilter(ACQueryDefinition navACQueryDefinition)
        {
            ACFilterItem filter = navACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "IsTenant").FirstOrDefault();
            if (filter == null)
            {
                filter = new ACFilterItem(Global.FilterTypes.filter, "IsTenant", Global.LogicalOperators.equal, Global.Operators.and, "true", true);
                navACQueryDefinition.ACFilterColumns.Add(filter);
            }
        }

        public void ResetAccessTenantCompanyFilter()
        {
            AccessTenantCompany.NavACQueryDefinition.ACFilterColumns.Clear();
            SetAccessTenantCompanyFilter(_AccessTenantCompany.NavACQueryDefinition);
            AccessTenantCompany.NavSearch();
        }

        public virtual void OnCurrentInOrderChanged()
        {

        }

        public void ResetAccessTenantCompanyFilter(InOrder inOrder)
        {
            ResetAccessTenantCompanyFilter();
            Company tenantCompany = null;
            if (inOrder != null)
            {
                if (inOrder.BillingCompanyAddress != null)
                    tenantCompany = inOrder.BillingCompanyAddress.Company;
                else if (inOrder.IssuerCompanyPerson != null)
                    tenantCompany = inOrder.IssuerCompanyPerson.Company;

                if (tenantCompany != null)
                    SelectedTenantCompany = tenantCompany;

                SelectedInvoiceCompanyAddress = inOrder.BillingCompanyAddress;
                SelectedInvoiceCompanyPerson = inOrder.IssuerCompanyPerson;
            }
        }

        /// <summary>
        /// Gets or sets the selected Company.
        /// </summary>
        /// <value>The selected Company.</value>
        [ACPropertySelected(101, "TenantCompany", "en{'Tenant'}de{'Mandant'}")]
        public Company SelectedTenantCompany
        {
            get
            {
                if (AccessTenantCompany == null)
                    return null;
                return AccessTenantCompany.Selected;
            }
            set
            {
                if (AccessTenantCompany == null)
                    return;
                if (AccessTenantCompany.Selected != value)
                {
                    AccessTenantCompany.Selected = value;
                    _InvoiceCompanyAddressList = null;
                    _InvoiceCompanyPersonList = null;
                    OnPropertyChanged("InvoiceCompanyAddressList");
                    OnPropertyChanged("InvoiceCompanyPersonList");
                }
                OnPropertyChanged("SelectedTenantCompany");
            }
        }

        /// <summary>
        /// Gets the Company list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(102, "TenantCompany")]
        public IEnumerable<Company> TenantCompanyList
        {
            get
            {
                if (AccessTenantCompany == null)
                    return null;
                return AccessTenantCompany.NavList;
            }
        }

        #endregion


        #region Tenant -> InvoiceCompanyAddress
        private CompanyAddress _SelectedInvoiceCompanyAddress;
        /// <summary>
        /// Selected property for CompanyAddress
        /// </summary>
        /// <value>The selected CompanyAddress</value>
        [ACPropertySelected(9999, "InvoiceCompanyAddress", "en{'Address for Invoice'}de{'Adresse zur Rechnungstellung'}")]
        public CompanyAddress SelectedInvoiceCompanyAddress
        {
            get
            {
                return _SelectedInvoiceCompanyAddress;
            }
            set
            {
                if (_SelectedInvoiceCompanyAddress != value)
                {
                    _SelectedInvoiceCompanyAddress = value;
                    OnPropertyChanged("SelectedInvoiceCompanyAddress");
                }
            }
        }


        private List<CompanyAddress> _InvoiceCompanyAddressList;
        /// <summary>
        /// List property for CompanyAddress
        /// </summary>
        /// <value>The CompanyAddress list</value>
        [ACPropertyList(9999, "InvoiceCompanyAddress")]
        public List<CompanyAddress> InvoiceCompanyAddressList
        {
            get
            {
                if (_InvoiceCompanyAddressList == null)
                    _InvoiceCompanyAddressList = LoadInvoiceCompanyAddressList();
                return _InvoiceCompanyAddressList;
            }
        }

        private List<CompanyAddress> LoadInvoiceCompanyAddressList()
        {
            if (SelectedTenantCompany == null) return null;
            return SelectedTenantCompany.CompanyAddress_Company.OrderBy(c => c.Name1).ToList();

        }
        #endregion

        #region Tenant -> InvoiceCompanyPerson
        private CompanyPerson _SelectedInvoiceCompanyPerson;
        /// <summary>
        /// Selected property for CompanyPerson
        /// </summary>
        /// <value>The selected CompanyPerson</value>
        [ACPropertySelected(9999, "InvoiceCompanyPerson", "en{'Person for invoice'}de{'Person zur Rechnungstellung'}")]
        public CompanyPerson SelectedInvoiceCompanyPerson
        {
            get
            {
                return _SelectedInvoiceCompanyPerson;
            }
            set
            {
                if (_SelectedInvoiceCompanyPerson != value)
                {
                    _SelectedInvoiceCompanyPerson = value;
                    OnPropertyChanged("SelectedInvoiceCompanyPerson");
                }
            }
        }


        private List<CompanyPerson> _InvoiceCompanyPersonList;
        /// <summary>
        /// List property for CompanyPerson
        /// </summary>
        /// <value>The CompanyPerson list</value>
        [ACPropertyList(9999, "InvoiceCompanyPerson")]
        public List<CompanyPerson> InvoiceCompanyPersonList
        {
            get
            {
                if (_InvoiceCompanyPersonList == null)
                    _InvoiceCompanyPersonList = LoadInvoiceCompanyPersonList();
                return _InvoiceCompanyPersonList;
            }
        }

        private List<CompanyPerson> LoadInvoiceCompanyPersonList()
        {
            if (SelectedTenantCompany == null) return null;
            return SelectedTenantCompany.CompanyPerson_Company.OrderBy(c => c.Name1).ThenBy(c => c.Name2).ToList();

        }
        #endregion

        #endregion


        //#region Issuer -> IssuerCompanyPerson

        //private CompanyPerson _SelectedIssuerCompanyPerson;
        ///// <summary>
        ///// Selected property for CompanyPerson
        ///// </summary>
        ///// <value>The selected IssuerCompanyPerson</value>
        //[ACPropertySelected(9999, "IssuerCompanyPerson", "en{'TODO: IssuerCompanyPerson'}de{'TODO: IssuerCompanyPerson'}")]
        //public CompanyPerson SelectedIssuerCompanyPerson
        //{
        //    get
        //    {
        //        return _SelectedIssuerCompanyPerson;
        //    }
        //    set
        //    {
        //        if (_SelectedIssuerCompanyPerson != value)
        //        {
        //            _SelectedIssuerCompanyPerson = value;
        //            OnPropertyChanged("SelectedIssuerCompanyPerson");
        //        }
        //    }
        //}

        //private List<CompanyPerson> _IssuerCompanyPersonList = null;
        ///// <summary>
        ///// List property for CompanyPerson
        ///// </summary>
        ///// <value>The IssuerCompanyPerson list</value>
        //[ACPropertyList(9999, "IssuerCompanyPerson")]
        //public List<CompanyPerson> IssuerCompanyPersonList
        //{
        //    get
        //    {
        //        return _IssuerCompanyPersonList;
        //    }
        //}

        //#endregion


        #endregion

        #region BSO->ACMethod

        #region Activation
        /// <summary>
        /// ACAction is called when one IACInteractiveObject (Source) wants to inform another IACInteractiveObject (Target) about an relevant interaction-event.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public override void ACAction(ACActionArgs actionArgs)
        {
            if (actionArgs.ElementAction == Global.ElementActionType.TabItemActivated)
            {
                OnActivate(actionArgs.DropObject.VBContent);
            }
            else
                base.ACAction(actionArgs);
        }

        protected bool _ActivateInOpen = false;
        [ACMethodInfo("Picking", "en{'Activate'}de{'Aktivieren'}", 608, true, Global.ACKinds.MSMethodPrePost)]
        public void OnActivate(string page)
        {
            if (!PreExecute("OnActivate"))
                return;
            switch (page)
            {
                case "*AssignContractPos":
                case "AssignContractPos":
                    if (!_ActivateInOpen)
                    {
                        _ActivateInOpen = true;
                        RefreshOpenContractPosList();
                    }
                    break;
                default:
                    break;
            }
            PostExecute("OnActivate");
        }

        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            switch (vbControl.VBContent)
            {
                case "CurrentInOrder\\MDInOrderType":
                    {
                        if (InOrderPosList != null && InOrderPosList.Any())
                            return Global.ControlModes.Disabled;
                        break;
                    }
            }

            return result;
        }

        #endregion

        #region 1. InOrder
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(InOrder.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(InOrder.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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

        protected override void OnPostSave()
        {
            _UnSavedUnAssignedContractPos = new List<InOrderPos>();
        }

        protected override void OnPostUndoSave()
        {
            _UnSavedUnAssignedContractPos = new List<InOrderPos>();
            RefreshOpenContractPosList();
            if (CurrentInOrder != null && CurrentInOrder.EntityState != EntityState.Added && CurrentInOrder.EntityState != EntityState.Detached)
                CurrentInOrder.InOrderPos_InOrder.AutoLoad(CurrentInOrder.InOrderPos_InOrderReference, CurrentInOrder);
            OnPropertyChanged("InOrderPosList");
            base.OnPostUndoSave();
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        [ACMethodInteraction(InOrder.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedInOrder", Global.ACKinds.MSMethodPrePost)]
        public virtual void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<InOrder>(requery, () => SelectedInOrder, () => CurrentInOrder, c => CurrentInOrder = c,
                        DatabaseApp.InOrder
                        .Include(c => c.InOrderPos_InOrder)
                        .Where(c => c.InOrderID == SelectedInOrder.InOrderID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedInOrder != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(InOrder.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedInOrder", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New"))
                return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(InOrder), InOrder.NoColumnName, InOrder.FormatNewNo, this);
            CurrentInOrder = InOrder.NewACObject(DatabaseApp, null, secondaryKey);

            // Vorbelegung mit der eigenen Anschrift
            try
            {
                CurrentInOrder.DeliveryCompanyAddress =
                    DatabaseApp.CompanyAddress.Where(c => c.Company.IsOwnCompany && c.IsDeliveryCompanyAddress)
                                              .OrderBy(c => c.Name1).FirstOrDefault();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOInOrder", "New", msg);
            }

            DatabaseApp.InOrder.Add(CurrentInOrder);
            if (CurrentUserSettings != null)
            {
                CurrentInOrder.BillingCompanyAddress = CurrentUserSettings.InvoiceCompanyAddress;
                CurrentInOrder.IssuerCompanyPerson = CurrentUserSettings.InvoiceCompanyPerson;
            }

            SelectedInOrder = CurrentInOrder;
            if (AccessPrimary != null)
                AccessPrimary.NavList.Add(CurrentInOrder);
            ACState = Const.SMNew;
            PostExecute("New");

        }

        /// <summary>
        /// Determines whether [is enabled new].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNew()
        {
            return true;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(InOrder.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentInOrder", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            Msg msg = CurrentInOrder.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavList.Remove(CurrentInOrder);
            SelectedInOrder = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return true;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(InOrder.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("InOrderList");
        }

        #endregion

        #region InOrderPos

        #region InOrderPos -> Manipulate (New, Edit, Delete)

        /// <summary>
        /// News the in order pos.
        /// </summary>
        [ACMethodInteraction(InOrderPos.ClassName, "en{'New Item'}de{'Neue Position'}", (short)MISort.New, true, "SelectedInOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void NewInOrderPos()
        {
            if (!PreExecute("NewInOrderPos"))
                return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            var inOrderPos = InOrderPos.NewACObject(DatabaseApp, CurrentInOrder);
            inOrderPos.MaterialPosType = GlobalApp.MaterialPosTypes.InwardRoot;
            OnPropertyChanged("");
            CurrentInOrderPos = inOrderPos;
            SelectedInOrderPos = inOrderPos;
            PostExecute("NewInOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled new in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewInOrderPos()
        {
            return true;
        }

        /// <summary>
        /// Deletes the in order pos.
        /// </summary>
        [ACMethodInteraction(InOrderPos.ClassName, "en{'Delete Item'}de{'Position löschen'}", (short)MISort.Delete, true, "CurrentInOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteInOrderPos()
        {
            if (!PreExecute("DeleteInOrderPos"))
                return;
            if (IsEnabledUnAssignContractPos())
            {
                UnAssignContractPos();
            }
            else
            {
                Msg msg = CurrentInOrderPos.DeleteACObject(DatabaseApp, true);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }
                OnPropertyChanged("InOrderPosList");
            }
            PostExecute("DeleteInOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled delete in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteInOrderPos()
        {
            return CurrentInOrder != null && CurrentInOrderPos != null;
        }

        #endregion

        #endregion

        #region CompanyMaterialPickup
        [ACMethodInteraction("CompanyMaterialPickup", "en{'Load Pick Up'}de{'Verladestamm laden'}", (short)MISort.Load, false, "SelectedCompanyMaterialPickup", Global.ACKinds.MSMethodPrePost)]
        public void LoadCompanyMaterialPickup()
        {
            if (!IsEnabledLoadCompanyMaterialPickup())
                return;
            if (!PreExecute("LoadCompanyMaterialPickup"))
                return;
            // Laden des aktuell selektierten CompanyMaterialPickup 
            CurrentCompanyMaterialPickup = CurrentInOrderPos.CompanyMaterialPickup_InOrderPos
                                            .Where(c => c.CompanyMaterialPickupID == SelectedCompanyMaterialPickup.CompanyMaterialPickupID)
                                            .FirstOrDefault();
            PostExecute("LoadCompanyMaterialPickup");
        }

        public bool IsEnabledLoadCompanyMaterialPickup()
        {
            return SelectedCompanyMaterialPickup != null && CurrentInOrderPos != null;
        }

        [ACMethodInteraction("CompanyMaterialPickup", "en{'New Pick Up'}de{'Neuer Verladestamm'}", (short)MISort.New, true, "SelectedCompanyMaterialPickup", Global.ACKinds.MSMethodPrePost)]
        public void NewCompanyMaterialPickup()
        {
            if (!PreExecute("NewCompanyMaterialPickup"))
                return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            var companyMaterialPickup = CompanyMaterialPickup.NewACObject(DatabaseApp, CurrentInOrderPos);
            companyMaterialPickup.CompanyMaterial = SelectedAvailableCompMaterial;
            OnPropertyChanged("CompanyMaterialPickupList");
            CurrentCompanyMaterialPickup = companyMaterialPickup;
            SelectedCompanyMaterialPickup = companyMaterialPickup;
            PostExecute("NewCompanyMaterialPickup");
        }

        public bool IsEnabledNewCompanyMaterialPickup()
        {
            if (SelectedAvailableCompMaterial == null || CurrentInOrderPos == null || SelectedAvailableCompMaterial.MaterialID != CurrentInOrderPos.MaterialID)
                return false;
            return true;
        }

        [ACMethodInteraction("CompanyMaterialPickup", "en{'Delete Pick Up'}de{'Verladestamm löschen'}", (short)MISort.Delete, true, "CurrentCompanyMaterialPickup", Global.ACKinds.MSMethodPrePost)]
        public void DeleteCompanyMaterialPickup()
        {
            if (!PreExecute("DeleteCompanyMaterialPickup")) return;
            Msg msg = CurrentCompanyMaterialPickup.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            OnPropertyChanged("CompanyMaterialPickupList");

            PostExecute("DeleteCompanyMaterialPickup");
        }

        public bool IsEnabledDeleteCompanyMaterialPickup()
        {
            return CurrentInOrderPos != null && CurrentCompanyMaterialPickup != null;
        }
        #endregion

        #region OrderDialog
        public VBDialogResult DialogResult { get; set; }

        [ACMethodInfo("Dialog", "en{'Dialog purchase Order'}de{'Dialog Bestellung'}", (short)MISort.QueryPrintDlg + 1)]
        public void ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;

            InOrderPos inOrderPos = null;
            InOrder inOrder = null;
            foreach (var entry in paOrderInfo.Entities)
            {
                if (entry.EntityName == InOrder.ClassName)
                {
                    inOrder = this.DatabaseApp.InOrder
                        .Where(c => c.InOrderID == entry.EntityID)
                        .FirstOrDefault();
                }
                else if (entry.EntityName == InOrderPos.ClassName)
                {
                    inOrderPos = this.DatabaseApp.InOrderPos
                        .Include(c => c.InOrder)
                        .Where(c => c.InOrderPosID == entry.EntityID)
                        .FirstOrDefault();
                    if (inOrderPos != null)
                        inOrder = inOrderPos.InOrder;
                }
            }

            if (inOrder == null)
                return;

            ShowDialogOrder(inOrder.InOrderNo, inOrderPos != null ? inOrderPos.InOrderPosID : (Guid?) null);
            paOrderInfo.DialogResult = this.DialogResult;
        }

        [ACMethodInfo("Dialog", "en{'Dialog Purchase Order'}de{'Dialog Bestellung'}", (short)MISort.QueryPrintDlg)]
        public void ShowDialogOrder(string inOrderNo, Guid? inOrderPosID)
        {
            if (AccessPrimary == null)
                return;
            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "InOrderNo").FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, "InOrderNo", Global.LogicalOperators.contains, Global.Operators.and, inOrderNo, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            else
                filterItem.SearchWord = inOrderNo;

            this.Search();
            if (CurrentInOrder != null && inOrderPosID != null)
            {
                OnPropertyChanged("InOrderPosList");
                if (InOrderPosList != null && InOrderPosList.Any())
                {
                    InOrderPos dbInOrderPos = DatabaseApp.InOrderPos.FirstOrDefault(c => c.InOrderPosID == (inOrderPosID ?? Guid.Empty));
                    if (dbInOrderPos != null)
                    {
                        var inOrderPos = InOrderPosList.Where(c => c.InOrderPosID == dbInOrderPos.InOrderPosID ||
                        c.InOrderPosID == dbInOrderPos.ParentInOrderPosID).FirstOrDefault();
                        if (inOrderPos != null)
                            SelectedInOrderPos = inOrderPos;
                    }
                }
            }
            ShowDialog(this, "DisplayOrderDialog");
            this.ParentACComponent.StopComponent(this);
        }

        [ACMethodInfo("Dialog", "en{'New purchase order'}de{'Neue Bestellung'}", (short)MISort.QueryPrintDlg)]
        public VBDialogResult ShowDialogNewInOrder(Material material = null, double? targetQuantity = null)
        {
            if (DialogResult == null)
                DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            New();
            if (material != null)
            {
                NewInOrderPos();
                if (CurrentInOrderPos != null)
                {
                    CurrentInOrderPos.Material = material;
                    if (targetQuantity.HasValue)
                    {
                        CurrentInOrderPos.TargetQuantityUOM = targetQuantity.Value;
                    }
                }
            }
            ShowDialog(this, "DisplayOrderDialog");
            this.ParentACComponent.StopComponent(this);
            return DialogResult;
        }

        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay)]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            DialogResult.ReturnValue = CurrentInOrder;
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel)]
        public void DialogCancel()
        {
            if (CurrentInOrder != null && CurrentInOrder.EntityState == EntityState.Added)
                Delete();
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            CloseTopDialog();
        }
        #endregion

        #region Assign / Unassign Contract lines

        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Filter'}de{'Filter'}", 602, false)]
        public bool FilterDialogContractPos()
        {
            bool result = AccessOpenContractPos.ShowACQueryDialog();
            if (result)
            {
                RefreshOpenContractPosList();
            }
            return result;
        }


        [ACMethodInfo("OpenContractPos", "en{'Find contract lines'}de{'Kontraktpos. suchen'}", 602, false)]
        public void RefreshOpenContractPosList()
        {
            if (_ActivateInOpen && AccessOpenContractPos != null)
                AccessOpenContractPos.NavSearch(DatabaseApp);
            OnPropertyChanged("OpenContractPosList");
        }

        /// <summary>
        /// Assigns the in order pos.
        /// </summary>
        [ACMethodCommand("OpenContractPos", "en{'Call off contract line'}de{'Kontraktposition abrufen'}", 603, true)]
        public virtual void AssignContractPos()
        {
            if (!IsEnabledAssignContractPos())
                return;
            List<object> resultNewEntities = new List<object>();
            try
            {

                Msg result = InDeliveryNoteManager.AssignContractInOrderPos(CurrentOpenContractPos, CurrentInOrder, PartialQuantity, DatabaseApp, ACFacilityManager, resultNewEntities);
                if (result != null)
                {
                    Messages.Msg(result);
                    return;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOInOrder", "AssignContractPos", msg);

                return;
            }

            if (_UnSavedUnAssignedContractPos.Contains(CurrentOpenContractPos))
                _UnSavedUnAssignedContractPos.Remove(CurrentOpenContractPos);
            OnPropertyChanged("InOrderPosList");

            RefreshOpenContractPosList();
            PartialQuantity = null;
            foreach (object item in resultNewEntities)
            {
                if (item is InOrderPos)
                {
                    SelectedInOrderPos = item as InOrderPos;
                    break;
                }
            }
        }

        /// <summary>
        /// Determines whether [is enabled assign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignContractPos()
        {
            if (CurrentOpenContractPos == null
                || CurrentInOrder == null
                || CurrentInOrder.MDInOrderType == null
                || CurrentInOrder.MDInOrderType.OrderType == GlobalApp.OrderTypes.Contract
                || CurrentInOrder.MDInOrderType.OrderType == GlobalApp.OrderTypes.InternalOrder)
                return false;
            return true;
        }

        /// <summary>
        /// Unassigns the in order pos.
        /// </summary>
        [ACMethodCommand("OpenContractPos", "en{'Revise contract'}de{'Kontraktabruf revidieren'}", 604, true)]
        public void UnAssignContractPos()
        {
            if (!IsEnabledUnAssignContractPos())
                return;

            InOrderPos parentInOrderPos = null;
            InOrderPos currentInOrderPos = CurrentInOrderPos;
            parentInOrderPos = CurrentInOrderPos.InOrderPos1_ParentInOrderPos;

            Msg result = null;
            try
            {
                result = InDeliveryNoteManager.UnassignContractInOrderPos(currentInOrderPos, DatabaseApp);
                if (result != null)
                {
                    Messages.Msg(result);
                    return;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOInOrder", "UnAssignContractPos", msg);
                return;
            }

            if (result == null && parentInOrderPos != null)
            {
                if (!_UnSavedUnAssignedContractPos.Contains(parentInOrderPos))
                    _UnSavedUnAssignedContractPos.Add(parentInOrderPos);
            }

            OnPropertyChanged("InOrderPosList");
            RefreshOpenContractPosList();
            PartialQuantity = null;
        }

        /// <summary>
        /// Determines whether [is enabled unassign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled unassign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUnAssignContractPos()
        {
            if (CurrentInOrderPos == null
                || CurrentInOrderPos.InOrderPos1_ParentInOrderPos == null
                || CurrentInOrderPos.InOrderPos1_ParentInOrderPos.InOrder.MDInOrderType.OrderType != GlobalApp.OrderTypes.Contract)
                return false;
            return true;
        }

        #endregion

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
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(NewInOrderPos):
                    NewInOrderPos();
                    return true;
                case nameof(IsEnabledNewInOrderPos):
                    result = IsEnabledNewInOrderPos();
                    return true;
                case nameof(DeleteInOrderPos):
                    DeleteInOrderPos();
                    return true;
                case nameof(IsEnabledDeleteInOrderPos):
                    result = IsEnabledDeleteInOrderPos();
                    return true;
                case nameof(LoadCompanyMaterialPickup):
                    LoadCompanyMaterialPickup();
                    return true;
                case nameof(IsEnabledLoadCompanyMaterialPickup):
                    result = IsEnabledLoadCompanyMaterialPickup();
                    return true;
                case nameof(NewCompanyMaterialPickup):
                    NewCompanyMaterialPickup();
                    return true;
                case nameof(IsEnabledNewCompanyMaterialPickup):
                    result = IsEnabledNewCompanyMaterialPickup();
                    return true;
                case nameof(DeleteCompanyMaterialPickup):
                    DeleteCompanyMaterialPickup();
                    return true;
                case nameof(IsEnabledDeleteCompanyMaterialPickup):
                    result = IsEnabledDeleteCompanyMaterialPickup();
                    return true;
                case nameof(AssignContractPos):
                    AssignContractPos();
                    return true;
                case nameof(IsEnabledAssignContractPos):
                    result = IsEnabledAssignContractPos();
                    return true;
                case nameof(UnAssignContractPos):
                    UnAssignContractPos();
                    return true;
                case nameof(IsEnabledUnAssignContractPos):
                    result = IsEnabledUnAssignContractPos();
                    return true;
                case nameof(RefreshOpenContractPosList):
                    RefreshOpenContractPosList();
                    return true;
                case nameof(FilterDialogContractPos):
                    FilterDialogContractPos();
                    return true;
                case nameof(ShowDialogOrder):
                    ShowDialogOrder(acParameter[0] as string, acParameter.Count() >= 2 ? (Guid?)acParameter[1] : null);
                    return true;
                case nameof(ShowDialogNewInOrder):
                    result = ShowDialogNewInOrder(acParameter.Count() >= 1 ? (Material)acParameter[0] : null, acParameter.Count() >= 2 ? (double?)acParameter[1] : null);
                    return true;
                case nameof(ShowDialogOrderInfo):
                    ShowDialogOrderInfo((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}
