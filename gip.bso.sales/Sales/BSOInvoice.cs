﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.reporthandler.Flowdoc;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;
using static gip.core.datamodel.Global;

namespace gip.bso.sales
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'Invoice'}de{'Rechunungen'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Invoice.ClassName)]
    public class BSOInvoice : ACBSOvbNav, IOutOrderPosBSO
    {

        #region private
        private UserSettings CurrentUserSettings { get; set; }

        #endregion

        #region c´tors

        public BSOInvoice(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            TempReportData = new ReportData();
            _OutDeliveryNoteManager = ACOutDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_OutDeliveryNoteManager == null)
                throw new Exception("OutDeliveryNoteManager not configured");
            CurrentUserSettings = DatabaseApp.UserSettings.Where(c => c.VBUserID == Root.Environment.User.VBUserID).FirstOrDefault();
            Search();
            SetSelectedPos();

            //IssuerResult issuerResult = OutDeliveryNoteManager.GetIssuer(DatabaseApp, Root.Environment.User.VBUserID);
            //IssuerCompanyAddressMessage = issuerResult.IssuerMessage;
            //_IssuerCompanyPersonList = issuerResult.CompanyPeople;
            //OnPropertyChanged("IssuerCompanyPersonList");
            //IssuerCompanyAddress = issuerResult.IssuerCompanyAddress;
            //SelectedIssuerCompanyPerson = issuerResult.IssuerCompanyPerson;

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_OutDeliveryNoteManager != null)
            {
                ACOutDeliveryNoteManager.DetachACRefFromServiceInstance(this, _OutDeliveryNoteManager);
                _OutDeliveryNoteManager = null;
            }

            this._CurrentMDUnit = null;
            this._CurrentInvoicePos = null;
            this._SelectedInvoicePos = null;
            this._UnSavedUnAssignedContractPos = null;
            this._PartialQuantity = null;
            _SelectedFilterMaterial = null;

            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

            bool result = base.ACDeInit(deleteACClassTask);

            if (_AccessInvoicePos != null)
            {
                _AccessInvoicePos.ACDeInit(false);
                _AccessInvoicePos = null;
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

        #endregion

        #region ControlMode
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            //CurrentInvoice.MDCurrencyExchange
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            if (!String.IsNullOrEmpty(vbControl.VBContent))
            {
                if (vbControl.VBContent.StartsWith("CurrentInvoice\\MDCurrencyExchange"))
                {
                    if (CurrentInvoice == null || !CurrentInvoice.IsExchangeRateValid)
                        return Global.ControlModes.EnabledWrong;
                    else
                        return Global.ControlModes.Disabled;
                }
            }
            //switch (vbControl.VBContent)
            //{
            //    case "CurrentInvoice\\MDCurrencyExchange":
            //        {
            //            if (CurrentInvoice == null || !CurrentInvoice.IsExchangeRateValid)
            //                return Global.ControlModes.EnabledWrong;
            //            break;
            //        }
            //}

            return result;
        }
        #endregion

        #region Managers

        protected ACRef<ACOutDeliveryNoteManager> _OutDeliveryNoteManager = null;
        protected ACOutDeliveryNoteManager OutDeliveryNoteManager
        {
            get
            {
                if (_OutDeliveryNoteManager == null)
                    return null;
                return _OutDeliveryNoteManager.ValueT;
            }
        }

        #endregion

        #region BSO->ACProperty
        #region 1. Invoice
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        ACAccessNav<Invoice> _AccessPrimary;
        [ACPropertyAccessPrimary(100, Invoice.ClassName)]
        public ACAccessNav<Invoice> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(NavigationqueryDefaultSort);
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Invoice>(Invoice.ClassName, this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessPrimary;
            }
        }

        private IQueryable<Invoice> _AccessPrimary_NavSearchExecuting(IQueryable<Invoice> result)
        {
            if (SelectedFilterMaterial != null)
            {
                result = result.Where(c => c.InvoicePos_Invoice.Any(mt => mt.MaterialID == SelectedFilterMaterial.MaterialID));
            }
            return result;
        }

        public List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                ACFilterItem acFilterInvoiceNo = new ACFilterItem(Global.FilterTypes.filter, "InvoiceNo", Global.LogicalOperators.contains, Global.Operators.and, null, true, true);
                aCFilterItems.Add(acFilterInvoiceNo);

                return aCFilterItems;
            }
        }

        private List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem acSortInvoiceDate = new ACSortItem("InvoiceDate", SortDirections.descending, true);
                acSortItems.Add(acSortInvoiceDate);

                return acSortItems;
            }
        }


        [ACPropertyCurrent(101, Invoice.ClassName)]
        public Invoice CurrentInvoice
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Current;
            }
            set
            {
                if (CurrentInvoice != null)
                {
                    CurrentInvoice.PropertyChanged -= CurrentInvoice_PropertyChanged;
                    if (string.IsNullOrEmpty(CurrentInvoice.XMLDesignStart))
                        CurrentInvoice.XMLDesignStart = Invoice.Const_XMLDesign;
                    if (string.IsNullOrEmpty(CurrentInvoice.XMLDesignEnd))
                        CurrentInvoice.XMLDesignEnd = Invoice.Const_XMLDesign;
                }

                if (AccessPrimary == null)
                    return;
                if (AccessPrimary.Current != value)
                {
                    AccessPrimary.Current = value;

                    if (CurrentInvoice != null)
                        CurrentInvoice.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentInvoice_PropertyChanged);

                    OnPropertyChanged("CurrentInvoice");
                    OnPropertyChanged("InvoicePosList");
                    OnPropertyChanged("CompanyList");
                    OnPropertyChanged("BillingCompanyAddressList");
                    OnPropertyChanged("DeliveryCompanyAddressList");
                    OnPropertyChanged("CurrentBillingCompanyAddress");
                    OnPropertyChanged("CurrentDeliveryCompanyAddress");

                    SetSelectedPos();

                    ResetAccessTenantCompanyFilter(value);
                }

            }
        }

        public void SetSelectedPos()
        {
            InvoicePos pos = null;
            if (CurrentInvoice != null)
                pos = CurrentInvoice.InvoicePos_Invoice.OrderBy(c => c.Sequence).FirstOrDefault();
            CurrentInvoicePos = pos;
            SelectedInvoicePos = pos;
        }

        void CurrentInvoice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (OutDeliveryNoteManager != null)
                OutDeliveryNoteManager.HandleIOrderPropertyChange(e.PropertyName, CurrentInvoice);

            switch (e.PropertyName)
            {
                case "CustomerCompanyID":

                    OnPropertyChanged("BillingCompanyAddressList");
                    OnPropertyChanged("DeliveryCompanyAddressList");
                    OnPropertyChanged("CurrentBillingCompanyAddress");
                    OnPropertyChanged("CurrentDeliveryCompanyAddress");

                    break;
            }
        }

        [ACPropertyList(103, Invoice.ClassName)]
        public IEnumerable<Invoice> InvoiceList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(102, Invoice.ClassName)]
        public Invoice SelectedInvoice
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
                OnPropertyChanged("SelectedInvoice");
            }
        }
        #endregion

        #region 1.1 InvoicePos
        ACAccess<InvoicePos> _AccessInvoicePos;
        [ACPropertyAccess(200, InvoicePos.ClassName)]
        public ACAccess<InvoicePos> AccessInvoicePos
        {
            get
            {
                if (_AccessInvoicePos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + InvoicePos.ClassName) as ACQueryDefinition;
                    _AccessInvoicePos = acQueryDefinition.NewAccess<InvoicePos>(InvoicePos.ClassName, this);
                }
                return _AccessInvoicePos;
            }
        }

        InvoicePos _CurrentInvoicePos;
        [ACPropertyCurrent(201, InvoicePos.ClassName)]
        public InvoicePos CurrentInvoicePos
        {
            get
            {
                return _CurrentInvoicePos;
            }
            set
            {
                if (_CurrentInvoicePos != value)
                {
                    if (_CurrentInvoicePos != null)
                    {
                        _CurrentInvoicePos.PropertyChanged -= CurrentInvoicePos_PropertyChanged;
                        if (string.IsNullOrEmpty(CurrentInvoicePos.XMLDesign))
                            CurrentInvoicePos.XMLDesign = Invoice.Const_XMLDesign;
                    }

                    _CurrentInvoicePos = value;
                    if (_CurrentInvoicePos != null)
                        _CurrentInvoicePos.PropertyChanged += CurrentInvoicePos_PropertyChanged;
                    CurrentMDUnit = CurrentInvoicePos?.MDUnit;
                    OnPropertyChanged("CurrentInvoicePos");
                    OnPropertyChanged("MDUnitList");
                    OnPropertyChanged("AvailableCompMaterialList");
                }
            }
        }

        void CurrentInvoicePos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (OutDeliveryNoteManager != null)
                OutDeliveryNoteManager.HandleIOrderPosPropertyChange(DatabaseApp, this, e.PropertyName,
                    CurrentInvoice, CurrentInvoicePos, CurrentInvoice.InvoicePos_Invoice.Select(c => (IOutOrderPos)c).ToList(), CurrentInvoice?.BillingCompanyAddress);
        }

        public void OnPricePropertyChanged()
        {
            if (CurrentInvoice != null)
            {
                CurrentInvoice.OnPricePropertyChanged();
            }
        }

        [ACPropertyList(203, InvoicePos.ClassName)]
        public IEnumerable<InvoicePos> InvoicePosList
        {
            get
            {
                if (CurrentInvoice == null)
                    return null;
                return CurrentInvoice.InvoicePos_Invoice.Where(c => c.EntityState != System.Data.EntityState.Deleted).OrderBy(c => c.Sequence);
            }
        }

        InvoicePos _SelectedInvoicePos;
        [ACPropertySelected(202, InvoicePos.ClassName)]
        public InvoicePos SelectedInvoicePos
        {
            get
            {
                return _SelectedInvoicePos;
            }
            set
            {
                _SelectedInvoicePos = value;
            }
        }

        /// <summary>
        /// Gets the MU quantity unit list.
        /// </summary>
        /// <value>The MU quantity unit list.</value>
        [ACPropertyList(205, MDUnit.ClassName)]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                if (CurrentInvoicePos == null || CurrentInvoicePos.Material == null)
                    return null;
                return CurrentInvoicePos.Material.MDUnitList;
            }
        }

        #region Property -> InvoicePos -> MDUnit

        MDUnit _CurrentMDUnit;
        /// <summary>
        /// Gets or sets the current MU quantity unit.
        /// </summary>
        /// <value>The current MU quantity unit.</value>
        [ACPropertyCurrent(206, MDUnit.ClassName, "en{'New Unit'}de{'Neue Einheit'}")]
        public MDUnit CurrentMDUnit
        {
            get
            {
                return _CurrentMDUnit;
            }
            set
            {
                _CurrentMDUnit = value;
                if (CurrentInvoicePos != null && CurrentInvoicePos.MDUnit != value)
                {
                    CurrentInvoicePos.MDUnit = value;
                    if (CurrentInvoicePos.MDUnit != null)
                        CurrentInvoicePos.TargetQuantity = CurrentInvoicePos.Material.ConvertQuantity(CurrentInvoicePos.TargetQuantityUOM, CurrentInvoicePos.Material.BaseMDUnit, CurrentInvoicePos.MDUnit);
                    OnPropertyChanged("CurrentInvoicePos");
                }
                OnPropertyChanged("CurrentMDUnit");
            }
        }

        #endregion

        Nullable<double> _ChangeTargetQuantity = null;
        [ACPropertyInfo(608, "", "en{'New Target Quantity'}de{'Neue Sollmenge'}")]
        public Nullable<double> ChangeTargetQuantity
        {
            get
            {
                return _ChangeTargetQuantity;
            }
            set
            {
                _ChangeTargetQuantity = value;
                if (_ChangeTargetQuantity.HasValue && (_ChangeTargetQuantity.Value > 0) && CurrentInvoicePos != null && CurrentInvoicePos.Material != null)
                {
                    CurrentInvoicePos.TargetQuantity = _ChangeTargetQuantity.Value;
                }
                _ChangeTargetQuantity = null;
                OnPropertyChanged("ChangeTargetQuantity");
            }
        }

        #endregion

        #region 1.2 Pos extended

        #region 1.2 Pos extended -> InvoicePosData
        private InvoicePos _SelectedInvoicePosData;
        /// <summary>
        /// Selected property for InvoicePos
        /// </summary>
        /// <value>The selected InvoicePosData</value>
        [ACPropertySelected(9999, "InvoicePosData", "en{'TODO: InvoicePosData'}de{'TODO: InvoicePosData'}")]
        public InvoicePos SelectedInvoicePosData
        {
            get
            {
                return _SelectedInvoicePosData;
            }
            set
            {
                if (_SelectedInvoicePosData != value)
                {
                    _SelectedInvoicePosData = value;
                    OnPropertyChanged("SelectedInvoicePosData");
                }
            }
        }


        /// <summary>
        /// List property for InvoicePos
        /// </summary>
        /// <value>The InvoicePosData list</value>
        [ACPropertyList(9999, "InvoicePosData")]
        public List<InvoicePos> InvoicePosDataList
        {
            get
            {
                if (InvoicePosList == null)
                    return null;
                return InvoicePosList.Where(c => c.Sequence < 1000).OrderBy(c => c.Sequence).ToList();
            }
        }

        #endregion


        #region 1.2 Pos extended -> InvoicePosDiscount
        private InvoicePos _SelectedInvoicePosDiscount;
        /// <summary>
        /// Selected property for InvoicePos
        /// </summary>
        /// <value>The selected InvoicePosDiscount</value>
        [ACPropertySelected(9999, "InvoicePosDiscount", "en{'TODO: InvoicePosDiscount'}de{'TODO: InvoicePosDiscount'}")]
        public InvoicePos SelectedInvoicePosDiscount
        {
            get
            {
                return _SelectedInvoicePosDiscount;
            }
            set
            {
                if (_SelectedInvoicePosDiscount != value)
                {
                    _SelectedInvoicePosDiscount = value;
                    OnPropertyChanged("SelectedInvoicePosDiscount");
                }
            }
        }


        /// <summary>
        /// List property for InvoicePos
        /// </summary>
        /// <value>The InvoicePosDiscount list</value>
        [ACPropertyList(9999, "InvoicePosDiscount")]
        public List<InvoicePos> InvoicePosDiscountList
        {
            get
            {
                if (InvoicePosList == null)
                    return null;
                return InvoicePosList.Where(c => c.PriceNet < 0 && c.Sequence >= 1000).OrderBy(c => c.Sequence).ToList();
            }
        }

        #endregion

        #endregion

        #endregion

        #region Company
        /// <summary>
        /// Liste aller Unternehmen, die Lieferanten sind
        /// </summary>
        [ACPropertyList(300, Company.ClassName)]
        public IEnumerable<Company> CompanyList
        {
            get
            {
                return DatabaseApp.Company
                    .Where(c => c.IsCustomer)
                    .OrderBy(c => c.CompanyName);
            }
        }

        [ACPropertyList(301, "BillingCompanyAddress")]
        public IEnumerable<CompanyAddress> BillingCompanyAddressList
        {
            get
            {
                if (CurrentInvoice == null || CurrentInvoice.CustomerCompany == null)
                    return null;
                if (!CurrentInvoice.CustomerCompany.CompanyAddress_Company.IsLoaded)
                    CurrentInvoice.CustomerCompany.CompanyAddress_Company.Load();
                return CurrentInvoice.CustomerCompany.CompanyAddress_Company
                    .Where(c => c.IsBillingCompanyAddress)
                    .OrderBy(c => c.Name1);
            }
        }

        [ACPropertyList(302, "DeliveryCompanyAddress")]
        public IEnumerable<CompanyAddress> DeliveryCompanyAddressList
        {
            get
            {
                if (CurrentInvoice == null || CurrentInvoice.CustomerCompany == null)
                    return null;
                if (!CurrentInvoice.CustomerCompany.CompanyAddress_Company.IsLoaded)
                    CurrentInvoice.CustomerCompany.CompanyAddress_Company.Load();
                return CurrentInvoice.CustomerCompany.CompanyAddress_Company
                    .Where(c => c.IsDeliveryCompanyAddress)
                    .OrderBy(c => c.Name1);
            }
        }

        [ACPropertyCurrent(303, "BillingCompanyAddress", ConstApp.BillingCompanyAddress)]
        public CompanyAddress CurrentBillingCompanyAddress
        {
            get
            {
                if (CurrentInvoice == null)
                    return null;
                return CurrentInvoice.BillingCompanyAddress;
            }
            set
            {
                if (CurrentInvoice != null && value != null)
                {
                    CurrentInvoice.BillingCompanyAddress = value;
                    OnPropertyChanged("CurrentBillingCompanyAddress");
                }
            }
        }

        [ACPropertyCurrent(304, "DeliveryCompanyAddress", ConstApp.DeliveryCompanyAddress)]
        public CompanyAddress CurrentDeliveryCompanyAddress
        {
            get
            {
                if (CurrentInvoice == null)
                    return null;
                return CurrentInvoice.DeliveryCompanyAddress;
            }
            set
            {
                if (CurrentInvoice != null && value != null)
                {
                    CurrentInvoice.DeliveryCompanyAddress = value;
                    OnPropertyChanged("CurrentDeliveryCompanyAddress");
                }
            }
        }
        #endregion

        #region 1.2 OpenContractPos
        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<OutOrderPos> _AccessOpenContractPos;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(400, "OpenContractPos")]
        public ACAccessNav<OutOrderPos> AccessOpenContractPos
        {
            get
            {
                if (_AccessOpenContractPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDef = Root.Queries.CreateQuery(null, Const.QueryPrefix + "OutOpenContractPos", ACType.ACIdentifier);
                    if (acQueryDef != null)
                    {
                        acQueryDef.CheckAndReplaceColumnsIfDifferent(OpenContractPosDefaultFilter, OpenContractPosDefaultSort, true, true);
                        if (acQueryDef.TakeCount == 0)
                            acQueryDef.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                    }

                    _AccessOpenContractPos = acQueryDef.NewAccessNav<OutOrderPos>(OutOrderPos.ClassName, this);
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
                    new ACFilterItem(Global.FilterTypes.filter, "OutOrderPos1_ParentOutOrderPos", Global.LogicalOperators.isNull, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, OutOrder.ClassName + "\\" + MDOutOrderType.ClassName + "\\OrderTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short) GlobalApp.OrderTypes.Contract).ToString(), true),
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
        [ACPropertyInfo(500, "FilterMaterial", "en{'Material'}de{'Material'}")]
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

        public const string _CTargetDeliveryDateProperty = OutOrder.ClassName + "\\" + "TargetDeliveryDate";

        [ACPropertyInfo(501, "FilterDelivDateFrom", "en{'Contract date from'}de{'Kontraktdatum von'}")]
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

        public const string _CTargetDeliveryMaxDateProperty = OutOrder.ClassName + "\\" + "TargetDeliveryMaxDate";
        [ACPropertyInfo(502, "FilterDelivDateTo", "en{'Contract date to'}de{'Kontraktdatum bis'}")]
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
        [ACPropertyInfo(503, "PartialQuantity", "en{'Partial Quantity'}de{'Teilmenge'}")]
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
        [ACPropertyCurrent(401, "OpenContractPos")]
        public OutOrderPos CurrentOpenContractPos
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

        protected List<OutOrderPos> _UnSavedUnAssignedContractPos = new List<OutOrderPos>();

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(403, "OpenContractPos")]
        public IEnumerable<OutOrderPos> OpenContractPosList
        {
            get
            {
                if (AccessOpenContractPos == null)
                    return null;
                if (CurrentInvoicePos != null)
                {
                    Guid[] outOrderPosIDs = CurrentInvoice.InvoicePos_Invoice.Select(c => c.OutOrderPos?.ParentOutOrderPosID ?? Guid.Empty).ToArray();
                    IEnumerable<OutOrderPos> addedPositions =
                        DatabaseApp
                        .OutOrderPos
                        .Where(c => outOrderPosIDs.Contains(c.OutOrderPosID));
                    if (addedPositions != null && addedPositions.Any())
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
        [ACPropertySelected(402, "OpenContractPos")]
        public OutOrderPos SelectedOpenContractPos
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


        #endregion

        #region Report
        private ReportData _TempReportData;
        [ACPropertyInfo(9999)]
        public ReportData TempReportData
        {
            get
            {
                return _TempReportData;
            }
            set
            {
                _TempReportData = value;
                OnPropertyChanged("TempReportData");
            }
        }

        [ACPropertyInfo(652)]
        public List<MDCountrySalesTax> TaxOverviewList
        {
            get;
            set;
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

        public void ResetAccessTenantCompanyFilter(IOutOrder outOrder)
        {
            ResetAccessTenantCompanyFilter();
            Company tenantCompany = null;
            if (outOrder != null)
            {
                if (outOrder.IssuerCompanyAddress != null)
                    tenantCompany = outOrder.IssuerCompanyAddress.Company;
                else if (outOrder.IssuerCompanyPerson != null)
                    tenantCompany = outOrder.IssuerCompanyPerson.Company;

                if (tenantCompany != null)
                    SelectedTenantCompany = tenantCompany;

                SelectedInvoiceCompanyAddress = outOrder.IssuerCompanyAddress;
                SelectedInvoiceCompanyPerson = outOrder.IssuerCompanyPerson;
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

        #region Properties => Pricelist

        private PriceListMaterial _SelectedPriceListMaterial;
        [ACPropertySelected(671, "PriceListMaterial", "en{'Price lists'}de{'Preisliste'}")]
        public PriceListMaterial SelectedPriceListMaterial
        {
            get => _SelectedPriceListMaterial;
            set
            {
                _SelectedPriceListMaterial = value;
                if (_SelectedPriceListMaterial != null)
                {
                    CurrentInvoicePos.PriceNet = _SelectedPriceListMaterial.Price;
                }
                _SelectedPriceListMaterial = null;
                OnPropertyChanged("SelectedPriceListMaterial");
            }
        }

        private List<PriceListMaterial> _PriceListMaterialItems;
        [ACPropertyList(671, "PriceListMaterial")]
        public List<PriceListMaterial> PriceListMaterialItems
        {
            get
            {
                return _PriceListMaterialItems;
            }
            set
            {
                _PriceListMaterialItems = value;
                OnPropertyChanged("PriceListMaterialItems");
            }
        }

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
        [ACMethodInfo(Invoice.ClassName, "en{'Activate'}de{'Aktivieren'}", 700, true, Global.ACKinds.MSMethodPrePost)]
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

        #endregion

        #region Invoice
        [ACMethodCommand(Invoice.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(Invoice.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        protected override void OnPostSave()
        {
            _UnSavedUnAssignedContractPos = new List<OutOrderPos>();
        }

        protected override void OnPostUndoSave()
        {
            _UnSavedUnAssignedContractPos = new List<OutOrderPos>();
            RefreshOpenContractPosList();
            if (CurrentInvoice != null && CurrentInvoice.EntityState != System.Data.EntityState.Added)
                CurrentInvoice.InvoicePos_Invoice.Load();
            OnPropertyChanged("InvoicePosList");
            base.OnPostUndoSave();
        }


        [ACMethodInteraction(Invoice.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedInvoice", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<Invoice>(requery, () => SelectedInvoice, () => CurrentInvoice, c => CurrentInvoice = c,
                        DatabaseApp.Invoice
                        .Include(c => c.InvoicePos_Invoice)
                        .Where(c => c.InvoiceID == SelectedInvoice.InvoiceID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedInvoice != null;
        }

        [ACMethodInteraction(Invoice.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedOutOrder", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Invoice), Invoice.NoColumnName, Invoice.FormatNewNo, this);
            Invoice newInvoice = Invoice.NewACObject(DatabaseApp, null, secondaryKey);
            if (CurrentUserSettings != null)
            {
                newInvoice.IssuerCompanyAddress = CurrentUserSettings.InvoiceCompanyAddress;
                newInvoice.IssuerCompanyPerson = CurrentUserSettings.InvoiceCompanyPerson;
            }
            DatabaseApp.Invoice.AddObject(newInvoice);
            if (AccessPrimary != null)
                AccessPrimary.NavList.Add(newInvoice);
            CurrentInvoice = newInvoice;
        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction(Invoice.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentOutOrder", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (Root.Messages.Question(this, "Question50061", Global.MsgResult.Yes, false, CurrentInvoice.InvoiceNo) == Global.MsgResult.Yes)
            {
                List<InvoicePos> items = CurrentInvoice.InvoicePos_Invoice.ToList();
                foreach (var item in items)
                    item.DeleteACObject(DatabaseApp, false);
                Msg msg = CurrentInvoice.DeleteACObject(DatabaseApp, true);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }
                if (AccessPrimary == null)
                    return;
                AccessPrimary.NavList.Remove(CurrentInvoice);
                SelectedInvoice = AccessPrimary.NavList.FirstOrDefault();
                Load();
            }
            PostExecute("Delete");

        }

        public bool IsEnabledDelete()
        {
            return CurrentInvoice != null;
        }

        [ACMethodCommand(Invoice.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("InvoiceList");
        }

        [ACMethodCommand("ExchangeRate", "en{'Update Exchange Rate'}de{'Wechselkurs aktualisieren'}", 850, true)]
        public void UpdateExchangeRate()
        {
            if (!IsEnabledUpdateExchangeRate())
                return;
            CurrentInvoice.UpdateExchangeRate();
            if (!CurrentInvoice.IsExchangeRateValid)
            {
                this.Messages.Warning(this, "Please open the businesss object for exchange rates and add a exchange rate for this invoice date", true);
            }
        }

        public bool IsEnabledUpdateExchangeRate()
        {
            if (CurrentInvoice == null)
                return false;
            return !CurrentInvoice.IsExchangeRateValid;
        }
        #endregion

        #region InvoicePos


        [ACMethodInteraction(InvoicePos.ClassName, "en{'New Item'}de{'Neue Position'}", (short)MISort.New, true, "SelectedInvoicePos", Global.ACKinds.MSMethodPrePost)]
        public void NewInvoicePos()
        {
            if (!PreExecute("NewInvoicePos"))
                return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            var invoicePos = InvoicePos.NewACObject(DatabaseApp, CurrentInvoice);
            OnPropertyChanged("InvoicePosList");
            SelectedInvoicePos = invoicePos;
            CurrentInvoicePos = invoicePos;
            PostExecute("NewInvoicePos");
        }

        public bool IsEnabledNewInvoicePos()
        {
            return CurrentInvoice != null;
        }

        [ACMethodInteraction(InvoicePos.ClassName, "en{'Delete Item'}de{'Position löschen'}", (short)MISort.Delete, true, "CurrentInvoicePos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteInvoicePos()
        {
            if (!PreExecute("DeleteInvoicePos"))
                return;
            if (IsEnabledUnAssignContractPos())
            {
                UnAssignContractPos();
            }
            else
            {
                Msg msg = CurrentInvoicePos.DeleteACObject(DatabaseApp, true);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }
                else
                    CurrentInvoice.InvoicePos_Invoice.Remove(CurrentInvoicePos);
                CurrentInvoice.RenumberSequence(1);
                OnPropertyChanged("InvoicePosList");
                SelectedInvoicePos = CurrentInvoice.InvoicePos_Invoice.FirstOrDefault();
                CurrentInvoicePos = SelectedInvoicePos;
            }
            PostExecute("DeleteInvoicePos");
        }

        public bool IsEnabledDeleteInvoicePos()
        {
            return CurrentInvoice != null && CurrentInvoicePos != null;
        }
        #endregion

        #region Assign / Unassign Contract lines

        [ACMethodInteraction("OpenContractPos", "en{'Filter'}de{'Filter'}", 800, false)]
        public bool FilterDialogContractPos()
        {
            bool result = AccessOpenContractPos.ShowACQueryDialog();
            if (result)
            {
                RefreshOpenContractPosList();
            }
            return result;
        }


        [ACMethodInfo("OpenContractPos", "en{'Find contract lines'}de{'Kontraktpos. suchen'}", 801, false)]
        public void RefreshOpenContractPosList()
        {
            if (_ActivateInOpen && AccessOpenContractPos != null)
                AccessOpenContractPos.NavSearch(DatabaseApp);
            OnPropertyChanged("OpenContractPosList");
        }

        /// <summary>
        /// Assigns the in order pos.
        /// </summary>
        [ACMethodCommand("OpenContractPos", "en{'Call off contract line'}de{'Kontraktposition abrufen'}", 802, true)]
        public virtual void AssignContractPos()
        {
            if (!IsEnabledAssignContractPos())
                return;

            double quantity = PartialQuantity != null ? (PartialQuantity ?? 0) : CurrentOpenContractPos.TargetQuantityUOM;
            InvoicePos pos = null;
            if (AssignToCurrentPosition)
            {
                pos = CurrentInvoicePos;
                pos.TargetQuantityUOM += quantity;
            }
            else
            {
                pos = InvoicePos.NewACObject(DatabaseApp, CurrentInvoice);
                pos.Material = CurrentOpenContractPos.Material;
                pos.TargetQuantityUOM = quantity;
                CurrentInvoice.RenumberSequence(1);
            }

            if (_UnSavedUnAssignedContractPos.Contains(CurrentOpenContractPos))
                _UnSavedUnAssignedContractPos.Remove(CurrentOpenContractPos);

            OutOrderPos childPos = OutOrderPos.NewACObject(DatabaseApp, CurrentOpenContractPos);
            childPos.TargetQuantityUOM = quantity;
            pos.OutOrderPos = childPos;

            OnPropertyChanged("InvoicePosList");
            RefreshOpenContractPosList();
            PartialQuantity = null;
        }

        public bool IsEnabledAssignContractPos()
        {
            return
                 CurrentOpenContractPos != null
                 && IsCurrentContractPosNotAssigned
                 && CurrentInvoice != null
                 &&
                 (
                    !AssignToCurrentPosition
                    || (CurrentInvoicePos != null && CurrentInvoicePos.OutOrderPos == null && CurrentInvoicePos.MaterialID == CurrentOpenContractPos.MaterialID)
                 );
        }

        public bool IsCurrentContractPosNotAssigned
        {
            get
            {
                if (CurrentOpenContractPos == null) return false;
                Guid[] outOrderPosIDs = CurrentInvoice.InvoicePos_Invoice.Select(c => c.OutOrderPos?.ParentOutOrderPosID ?? Guid.Empty).ToArray();
                return !outOrderPosIDs.Contains(CurrentOpenContractPos.OutOrderPosID);
            }
        }

        private bool _AssignToCurrentPosition;

        [ACPropertyInfo(805, "AssignToCurrentPosition", "en{'Assign to current position'}de{'Der aktuellen Position zuweisen'}")]
        public bool AssignToCurrentPosition
        {
            get
            {
                return _AssignToCurrentPosition;
            }
            set
            {
                if (_AssignToCurrentPosition != value)
                {
                    _AssignToCurrentPosition = value;
                    OnPropertyChanged("AssignToCurrentPosition");
                }
            }
        }

        /// <summary>
        /// Unassigns the in order pos.
        /// </summary>
        [ACMethodCommand("OpenContractPos", "en{'Revise contract'}de{'Kontraktabruf revidieren'}", 803, true)]
        public void UnAssignContractPos()
        {
            if (!IsEnabledUnAssignContractPos())
                return;
            OutOrderPos parentOutOrderPos = null;
            Msg result = null;
            try
            {
                parentOutOrderPos = CurrentInvoicePos.OutOrderPos.OutOrderPos1_ParentOutOrderPos;
                result = CurrentInvoicePos.OutOrderPos.DeleteACObject(DatabaseApp, false);
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

                Messages.LogException("BSOInvoice", "UnAssignContractPos", msg);
                return;
            }

            if (result == null && parentOutOrderPos != null)
            {
                CurrentInvoicePos.OutOrderPos = null;
                CurrentInvoicePos.OutOrderPos = null;
                if (!_UnSavedUnAssignedContractPos.Contains(parentOutOrderPos))
                    _UnSavedUnAssignedContractPos.Add(parentOutOrderPos);
            }

            OnPropertyChanged("OutOrderPosList");
            RefreshOpenContractPosList();
            PartialQuantity = null;
        }

        /// <summary>
        /// Determines whether [is enabled unassign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled unassign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUnAssignContractPos()
        {
            return CurrentInvoicePos != null && CurrentInvoicePos.OutOrderPosID != null;
        }

        #endregion

        #region Methods => Report

        [ACPropertyInfo(650)]
        public List<InvoicePos> InvoiceDataList
        {
            get;
            set;
        }

        [ACPropertyInfo(651)]
        public List<InvoicePos> InvoiceDiscountList
        {
            get;
            set;
        }

        private void BuildInvoicePosData(string langCode)
        {
            if (CurrentInvoice == null)
                return;

            List<InvoicePos> posData = new List<InvoicePos>();

            //foreach (var invoicePos in CurrentInvoice.InvoicePos_Invoice.Where(c => c.GroupInvoicePosID == null && c.PriceNet >= 0).OrderBy(p => p.Position))
            foreach (var invoicePos in CurrentInvoice.InvoicePos_Invoice.Where(c => c.Sequence < 1000).OrderBy(p => p.Position))
            {
                posData.Add(invoicePos);
                //BuildInvoicePosDataRecursive(posData, invoicePos.Items);
                //if (invoicePos.GroupSum)
                //{
                //    InvoicePos sumPos = new InvoicePos();
                //    sumPos.TotalPricePrinted = invoicePos.Items.Sum(c => c.TotalPrice).ToString("N");
                //    sumPos.MaterialNo = Root.Environment.TranslateMessageLC(this, "Info50063", langCode) + invoicePos.Material.MaterialNo; // Info50063.
                //    sumPos.Sequence = invoicePos.Sequence;
                //    sumPos.GroupSum = invoicePos.GroupSum;
                //    posData.Add(sumPos);
                //}
            }

            InvoiceDataList = posData;
            InvoiceDiscountList = CurrentInvoice.InvoicePos_Invoice.Where(c => c.PriceNet < 0 && c.Sequence >= 1000).OrderBy(s => s.Sequence).ToList();
            if (InvoiceDiscountList != null && InvoiceDiscountList.Any())
            {
                //OutOfferPosDiscountList.Add(new OutOfferPos() { Comment = "Rabatt in Summe:", PriceNet = (decimal)CurrentInvoice.PosPriceNetDiscount });
                InvoiceDiscountList.Add(new InvoicePos() { Comment = Root.Environment.TranslateMessageLC(this, "Info50064", langCode), PriceNet = (decimal)CurrentInvoice.PosPriceNetTotal }); //Info50064.
            }

            OutDeliveryNoteManager.CalculateTaxOverview(this, CurrentInvoice, CurrentInvoice.InvoicePos_Invoice.Select(c => (IOutOrderPos)c).ToList());
        }

        private void BuildInvoicePosDataRecursive(List<InvoicePos> posDataList, IEnumerable<InvoicePos> invoicePosList)
        {
            foreach (var invoicePos in invoicePosList.Where(c => c.Sequence < 1000).OrderBy(p => p.Position))
            {
                posDataList.Add(invoicePos);
                //BuildInvoicePosDataRecursive(posDataList, invoicePos.Items);
            }
        }

        public override void OnPrintingPhase(object reportEngine, ACPrintingPhase printingPhase)
        {
            if (printingPhase == ACPrintingPhase.Started)
            {
                ReportDocument doc = reportEngine as ReportDocument;
                if (doc != null && doc.ReportData != null && doc.ReportData.Any(c => c.ACClassDesign != null
                                                                                 && (c.ACClassDesign.ACIdentifier.EndsWith("De")) || c.ACClassDesign.ACIdentifier.EndsWith("En") || c.ACClassDesign.ACIdentifier.EndsWith("Hr")))
                {
                    doc.SetFlowDocObjValue += Doc_SetFlowDocObjValue;
                    gip.core.datamodel.ACClassDesign design = doc.ReportData.Select(c => c.ACClassDesign).FirstOrDefault();
                    string langCode = "de";
                    if (design != null)
                    {
                        if (design.ACIdentifier.EndsWith("Hr"))
                            langCode = "hr";
                        if (design.ACIdentifier.EndsWith("En"))
                            langCode = "en";
                    }
                    BuildInvoicePosData(langCode);
                }
            }
            else
            {
                ReportDocument doc = reportEngine as ReportDocument;
                if (doc != null)
                {
                    doc.SetFlowDocObjValue -= Doc_SetFlowDocObjValue;
                }
            }

            base.OnPrintingPhase(reportEngine, printingPhase);
        }

        private void Doc_SetFlowDocObjValue(object sender, PaginatorOnSetValueEventArgs e)
        {
            InvoicePos pos = e.ParentDataRow as InvoicePos;
            if (e.FlowDocObj != null
                && (e.FlowDocObj.VBContent == "CurrentInvoice\\IsReverseCharge"
                || e.FlowDocObj.VBContent == "CurrentInvoice\\NotIsReverseCharge"))
            {
                if (CurrentInvoice != null
                    && ((!CurrentInvoice.IsReverseCharge && e.FlowDocObj.VBContent == "CurrentInvoice\\IsReverseCharge")
                        || (CurrentInvoice.IsReverseCharge && e.FlowDocObj.VBContent == "CurrentInvoice\\NotIsReverseCharge")))
                {
                    var inlineCell = e.FlowDocObj as InlineContextValue;
                    if (inlineCell != null)
                    {
                        var tableCell = (inlineCell.Parent as Paragraph)?.Parent as TableCell;
                        if (tableCell != null)
                        {
                            TableRow tableRow = tableCell.Parent as TableRow;
                            if (tableRow != null)
                                tableRow.Cells.Remove(tableCell);
                        }
                    }
                }
            }
            if (e.FlowDocObj != null && e.FlowDocObj.VBContent == "CurrentInvoice\\MDCurrencyExchange\\ExchangeNo")
            {
                if (CurrentInvoice != null && CurrentInvoice.MDCurrencyExchange == null)
                {
                    var inlineCell = e.FlowDocObj as InlineContextValue;
                    if (inlineCell != null)
                    {
                        var tableCell = (inlineCell.Parent as Paragraph)?.Parent as TableCell;
                        if (tableCell != null)
                        {
                            TableRow tableRow = tableCell.Parent as TableRow;
                            if (tableRow != null)
                                tableRow.Cells.Remove(tableCell);
                        }
                    }
                }
            }
            //if (pos != null && pos.GroupSum && pos.OutOfferPosID == new Guid())
            //{
            //    var inlineCell = e.FlowDocObj as InlineTableCellValue;
            //    if (inlineCell != null)
            //    {
            //        var tableCell = (inlineCell.Parent as Paragraph)?.Parent as TableCell;
            //        if (tableCell != null)
            //        {
            //            if (inlineCell.VBContent == "MaterialNo")
            //            {
            //                TableRow tableRow = tableCell.Parent as TableRow;
            //                if (tableRow != null && tableRow.Cells.Count > 6)
            //                {
            //                    tableRow.Cells.RemoveAt(2);
            //                    tableRow.Cells.RemoveAt(2);
            //                    tableRow.Cells.RemoveAt(2);
            //                    tableRow.Cells.RemoveAt(2);
            //                }
            //                tableCell.ColumnSpan = 2;
            //            }

            //            else if (inlineCell.VBContent == "TotalPricePrinted")
            //            {
            //                tableCell.ColumnSpan = 4;
            //                tableCell.BorderBrush = Brushes.Black;
            //                tableCell.BorderThickness = new System.Windows.Thickness(0, 1, 0, 1);
            //                tableCell.TextAlignment = System.Windows.TextAlignment.Right;
            //            }
            //            tableCell.FontWeight = System.Windows.FontWeights.Bold;
            //        }
            //    }
            //}
        }

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
                case "Load":
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case "IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
                case "New":
                    New();
                    return true;
                case "IsEnabledNew":
                    result = IsEnabledNew();
                    return true;
                case "Delete":
                    Delete();
                    return true;
                case "IsEnabledDelete":
                    result = IsEnabledDelete();
                    return true;
                case "Search":
                    Search();
                    return true;
                case "AssignContractPos":
                    AssignContractPos();
                    return true;
                case "IsEnabledAssignContractPos":
                    result = IsEnabledAssignContractPos();
                    return true;
                case "UnAssignContractPos":
                    UnAssignContractPos();
                    return true;
                case "IsEnabledUnAssignContractPos":
                    result = IsEnabledUnAssignContractPos();
                    return true;
                case "RefreshOpenContractPosList":
                    RefreshOpenContractPosList();
                    return true;
                case "FilterDialogContractPos":
                    FilterDialogContractPos();
                    return true;
                case "UpdateExchangeRate":
                    UpdateExchangeRate();
                    return true;
                case "IsEnabledUpdateExchangeRate":
                    result = IsEnabledUpdateExchangeRate();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #endregion
    }
}