using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using gip.mes.facility;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.sales
{
    /// <summary>
    /// Version 3
    /// 
    /// Neue Masken:
    /// 1. Angebotsverwaltung
    /// 
    /// TODO: Betroffene Tabellen: OutOffer, OutOfferPos
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Offer'}de{'Angebot'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOffer.ClassName)]
    public class BSOOutOffer : ACBSOvbNav, IOutOrderPosBSO
    {
        #region private
        private UserSettings CurrentUserSettings { get; set; }

        #endregion

        #region c´tors

        public BSOOutOffer(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
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

            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

            this._AccessOutOfferPos = null;
            this._CurrentOutOfferPos = null;
            this._SelectedOutOfferPos = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessOutOfferPos != null)
            {
                _AccessOutOfferPos.ACDeInit(false);
                _AccessOutOfferPos = null;
            }
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
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
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        ACAccessNav<OutOffer> _AccessPrimary;
        [ACPropertyAccessPrimary(690, OutOffer.ClassName)]
        public ACAccessNav<OutOffer> AccessPrimary
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
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<OutOffer>(OutOffer.ClassName, this);
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
                    new ACFilterItem(Global.FilterTypes.filter, OutOffer.NoColumnName, Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
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
                    new ACSortItem(OutOffer.NoColumnName, Global.SortDirections.descending, true)
                };
            }
        }

        private IQueryable<OutOffer> _AccessPrimary_NavSearchExecuting(IQueryable<OutOffer> result)
        {
            IQueryable<OutOffer> query = result as IQueryable<OutOffer>;
            if (query != null)
            {
                query.Include(c => c.MDOutOfferState)
                     .Include(c => c.CustomerCompany)
                     .Include(c => c.MDOutOrderType)
                     .Include(c => c.BillingCompanyAddress)
                     .Include(c => c.DeliveryCompanyAddress);
            }
            if (FilterOfferState.HasValue)
                result = result.Where(x => x.MDOutOfferState.MDOutOfferStateIndex == (short)FilterOfferState.Value);
            return result;
        }

        [ACPropertyCurrent(600, OutOffer.ClassName)]
        public OutOffer CurrentOutOffer
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (CurrentOutOffer != null)
                    CurrentOutOffer.PropertyChanged -= CurrentOutOffer_PropertyChanged;
                if (AccessPrimary == null)
                    return;

                //if (AccessPrimary.Current != value)
                //{
                AccessPrimary.Current = value;

                if (CurrentOutOffer != null)
                {
                    CurrentOutOffer.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentOutOffer_PropertyChanged);
                }
                CurrentOutOfferPos = null;
                CurrentOutOfferPos = OutOfferPosList?.FirstOrDefault();
                OnPropertyChanged("CurrentOutOffer");
                OnPropertyChanged("OutOfferPosList");
                OnPropertyChanged("CompanyList");
                OnPropertyChanged("BillingCompanyAddressList");
                OnPropertyChanged("DeliveryCompanyAddressList");
                OnPropertyChanged("CurrentBillingCompanyAddress");
                OnPropertyChanged("CurrentDeliveryCompanyAddress");

                ResetAccessTenantCompanyFilter(value);
                //}
            }
        }

        void CurrentOutOffer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (OutDeliveryNoteManager != null)
                OutDeliveryNoteManager.HandleIOrderPropertyChange(e.PropertyName, CurrentOutOffer);
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

        [ACPropertyList(601, OutOffer.ClassName)]
        public IEnumerable<OutOffer> OutOfferList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, OutOffer.ClassName)]
        public OutOffer SelectedOutOffer
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedOutOffer");
            }
        }

        ACAccess<OutOfferPos> _AccessOutOfferPos;
        [ACPropertyAccess(603, "OutOfferPos")]
        public ACAccess<OutOfferPos> AccessOutOfferPos
        {
            get
            {
                if (_AccessOutOfferPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + OutOfferPos.ClassName) as ACQueryDefinition;
                    _AccessOutOfferPos = acQueryDefinition.NewAccess<OutOfferPos>("OutOfferPos", this);
                }
                return _AccessOutOfferPos;
            }
        }

        OutOfferPos _CurrentOutOfferPos;
        [ACPropertyCurrent(604, "OutOfferPos")]
        public OutOfferPos CurrentOutOfferPos
        {
            get
            {
                return _CurrentOutOfferPos;
            }
            set
            {
                if (_CurrentOutOfferPos != null)
                    _CurrentOutOfferPos.PropertyChanged -= CurrentOutOfferPos_PropertyChanged;
                _CurrentOutOfferPos = value;
                if (_CurrentOutOfferPos != null)
                {
                    _CurrentOutOfferPos.PropertyChanged += CurrentOutOfferPos_PropertyChanged;
                    if (_CurrentOutOfferPos.Material != null)
                        PriceListMaterialItems = DatabaseApp.PriceListMaterial.Where(c => c.MaterialID == _CurrentOutOfferPos.MaterialID && c.PriceList.DateFrom < DateTime.Now
                                                                                     && (!c.PriceList.DateTo.HasValue || c.PriceList.DateTo > DateTime.Now)).ToList();
                }
                else
                    PriceListMaterialItems = null;
                OnPropertyChanged("MDUnitList");
                OnPropertyChanged("CurrentOutOfferPos");
                OnPropertyChanged("CurrentMDUnit");
            }
        }

        void CurrentOutOfferPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (OutDeliveryNoteManager != null)
            {
                OutDeliveryNoteManager.HandleIOrderPosPropertyChange(DatabaseApp, this, e.PropertyName,
                    CurrentOutOffer, CurrentOutOfferPos, CurrentOutOffer.OutOfferPos_OutOffer.Select(c => (IOutOrderPos)c).ToList(), CurrentOutOffer?.BillingCompanyAddress);
            }
        }

        /// <summary>
        /// Handle price changes
        /// </summary>
        public void OnPricePropertyChanged()
        {
            if (CurrentOutOffer != null)
                CurrentOutOffer.OnPricePropertyChanged();
        }

        /// <summary>
        /// OutOffer list
        /// </summary>
        [ACPropertyList(605, "OutOfferPos")]
        public IEnumerable<OutOfferPos> OutOfferPosList
        {
            get
            {
                return CurrentOutOffer?.OutOfferPos_OutOffer.OrderBy(c => c.Position);
            }
        }

        OutOfferPos _SelectedOutOfferPos;
        /// <summary>
        /// 
        /// </summary>
        [ACPropertySelected(606, "OutOfferPos")]
        public OutOfferPos SelectedOutOfferPos
        {
            get
            {
                return _SelectedOutOfferPos;
            }
            set
            {
                _SelectedOutOfferPos = value;
                OnPropertyChanged("SelectedOutOfferPos");
            }
        }


        #region Properties -> FilterOrderState

        public gip.mes.datamodel.MDOutOfferState.OutOfferStates? FilterOfferState
        {
            get
            {
                if (SelectedFilterOfferState == null) return null;
                return (gip.mes.datamodel.MDOutOfferState.OutOfferStates)Enum.Parse(typeof(gip.mes.datamodel.MDOutOfferState.OutOfferStates), SelectedFilterOfferState.Value.ToString());
            }
        }


        private ACValueItem _SelectedFilterOfferState;
        [ACPropertySelected(9999, "FilterOfferState", "en{'Offer state'}de{'Angebotsstatus'}")]
        public ACValueItem SelectedFilterOfferState
        {
            get
            {
                return _SelectedFilterOfferState;
            }
            set
            {
                if (_SelectedFilterOfferState != value)
                {
                    _SelectedFilterOfferState = value;
                    OnPropertyChanged("SelectedFilterOfferState");
                }
            }
        }


        private ACValueItemList _FilterOfferStateList;
        [ACPropertyList(9999, "FilterOfferState")]
        public ACValueItemList FilterOfferStateList
        {
            get
            {
                if (_FilterOfferStateList == null)
                {
                    _FilterOfferStateList = new ACValueItemList("OutOfferStatesList");
                    _FilterOfferStateList.AddRange(DatabaseApp.MDOutOfferState.ToList().Select(x => new ACValueItem(x.MDOutOfferStateName, x.MDOutOfferStateIndex, null)).ToList());
                }
                return _FilterOfferStateList;
            }
        }

        [ACPropertyInfo(300, nameof(FilterNo), "en{'Offering No.'}de{'Angebot Nr.'}")]
        public string FilterNo
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(OutOffer.NoColumnName);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(OutOffer.NoColumnName);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(OutOffer.NoColumnName, value);
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

        #region Property -> OutOfferPos -> MDUnit

        /// <summary>
        /// Unit list
        /// </summary>
        [ACPropertyList(607, MDUnit.ClassName)]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                if (CurrentOutOfferPos == null || CurrentOutOfferPos.Material == null)
                    return null;
                return CurrentOutOfferPos.Material.MDUnitList;
            }
        }

        private MDUnit _CurrentMDUnit;
        /// <summary>
        /// Current MDUnit
        /// </summary>
        [ACPropertyCurrent(608, MDUnit.ClassName)]
        public MDUnit CurrentMDUnit
        {
            get
            {
                return _CurrentMDUnit;
            }
            set
            {
                _CurrentMDUnit = value;
                if (CurrentOutOfferPos != null && CurrentOutOfferPos.MDUnit != value)
                {
                    CurrentOutOfferPos.MDUnit = value;
                    if (CurrentOutOfferPos.MDUnit != null)
                    {
                        CurrentOutOfferPos.TargetQuantity = CurrentOutOfferPos.Material.ConvertQuantity(CurrentOutOfferPos.TargetQuantityUOM, CurrentOutOfferPos.Material.BaseMDUnit, CurrentOutOfferPos.MDUnit);
                        OnPropertyChanged("CurrentOutOfferPos");
                    }
                }
                OnPropertyChanged("CurrentMDUnit");
            }
        }

        #endregion

        /// <summary>
        /// Liste aller Unternehmen, die Lieferanten sind
        /// </summary>
        [ACPropertyList(609, Company.ClassName)]
        public IEnumerable<Company> CompanyList
        {
            get
            {
                return DatabaseApp.Company.Where(c => c.IsCustomer).OrderBy(c => c.CompanyName).AsEnumerable();
            }
        }

        [ACPropertyList(610, "BillingCompanyAddress")]
        public IEnumerable<CompanyAddress> BillingCompanyAddressList
        {
            get
            {
                if (CurrentOutOffer == null || CurrentOutOffer.CustomerCompany == null)
                    return null;
                if (!CurrentOutOffer.CustomerCompany.CompanyAddress_Company_IsLoaded)
                    CurrentOutOffer.CustomerCompany.CompanyAddress_Company.AutoLoad(CurrentOutOffer.CustomerCompany.CompanyAddress_CompanyReference, CurrentOutOffer);
                return CurrentOutOffer.CustomerCompany.CompanyAddress_Company.Where(c => c.IsHouseCompanyAddress).OrderBy(c => c.Name1).AsEnumerable();
            }
        }

        [ACPropertyList(611, "DeliveryCompanyAddress")]
        public IEnumerable<CompanyAddress> DeliveryCompanyAddressList
        {
            get
            {
                if (CurrentOutOffer == null || CurrentOutOffer.CustomerCompany == null)
                    return null;
                if (!CurrentOutOffer.CustomerCompany.CompanyAddress_Company_IsLoaded)
                    CurrentOutOffer.CustomerCompany.CompanyAddress_Company.AutoLoad(CurrentOutOffer.CustomerCompany.CompanyAddress_CompanyReference, CurrentOutOffer);
                return CurrentOutOffer.CustomerCompany.CompanyAddress_Company.Where(c => c.IsDeliveryCompanyAddress).OrderBy(c => c.Name1).AsEnumerable();
            }
        }

        [ACPropertyCurrent(612, "BillingCompanyAddress", ConstApp.BillingCompanyAddress)]
        public CompanyAddress CurrentBillingCompanyAddress
        {
            get
            {
                if (CurrentOutOffer == null)
                    return null;
                return CurrentOutOffer.BillingCompanyAddress;
            }
            //set
            //{
            //    if (CurrentOutOffer == null || value == null)
            //        return;
            //    CurrentOutOffer.BillingCompanyAddress = value;
            //    OnPropertyChanged("CurrentBillingCompanyAddress");
            //    if (CurrentOutOffer.MDCurrency == null && CurrentOutOffer.BillingCompanyAddress != null)
            //        CurrentOutOffer.MDCurrency = CurrentOutOffer.BillingCompanyAddress.MDCountry.MDCurrency;
            //}
        }

        [ACPropertyCurrent(613, "DeliveryCompanyAddress", ConstApp.DeliveryCompanyAddress)]
        public CompanyAddress CurrentDeliveryCompanyAddress
        {
            get
            {
                if (CurrentOutOffer == null)
                    return null;
                return CurrentOutOffer.DeliveryCompanyAddress;
            }
            set
            {
                if (CurrentOutOffer == null)
                    return;
                CurrentOutOffer.DeliveryCompanyAddress = value;
                OnPropertyChanged("CurrentDeliveryCompanyAddress");
            }
        }

        #region Properties => Report

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

        [ACPropertyInfo(650)]
        public List<OutOfferPos> OutOfferPosDataList
        {
            get;
            set;
        }

        [ACPropertyInfo(651)]
        public List<OutOfferPos> OutOfferPosDiscountList
        {
            get;
            set;
        }

        [ACPropertyInfo(652)]
        public List<MDCountrySalesTax> TaxOverviewList
        {
            get;
            set;
        }

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
                    CurrentOutOfferPos.PriceNet = _SelectedPriceListMaterial.Price;
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

        #region Issuer

        #region Issuer -> IssuerCompanyAddress

        private CompanyAddress _IssuerCompanyAddress;
        [ACPropertyInfo(999, "IssuerCompanyAddress", "en{'Issuer Company Address'}de{'Emittentenfirmaadresse'}")]
        public CompanyAddress IssuerCompanyAddress
        {
            get
            {
                return _IssuerCompanyAddress;
            }
            set
            {
                if (_IssuerCompanyAddress != value)
                {
                    _IssuerCompanyAddress = value;
                    OnPropertyChanged("IssuerCompanyAddress");
                }
            }
        }

        private string _IssuerCompanyAddressMessage;
        [ACPropertyInfo(999, "IssuerCompanyAddressMessage", "en{'Issuer status'}de{'Emittentenstatus'}")]
        public string IssuerCompanyAddressMessage

        {
            get
            {
                return _IssuerCompanyAddressMessage;
            }
            set
            {
                if (_IssuerCompanyAddressMessage != value)
                {
                    _IssuerCompanyAddressMessage = value;
                    OnPropertyChanged("IssuerCompanyAddressMessage");
                }
            }
        }

        #endregion

        #region Issuer -> IssuerCompanyPerson

        private CompanyPerson _SelectedIssuerCompanyPerson;
        /// <summary>
        /// Selected property for CompanyPerson
        /// </summary>
        /// <value>The selected IssuerCompanyPerson</value>
        [ACPropertySelected(9999, "IssuerCompanyPerson", "en{'TODO: IssuerCompanyPerson'}de{'TODO: IssuerCompanyPerson'}")]
        public CompanyPerson SelectedIssuerCompanyPerson
        {
            get
            {
                return _SelectedIssuerCompanyPerson;
            }
            set
            {
                if (_SelectedIssuerCompanyPerson != value)
                {
                    _SelectedIssuerCompanyPerson = value;
                    OnPropertyChanged("SelectedIssuerCompanyPerson");
                }
            }
        }

        private List<CompanyPerson> _IssuerCompanyPersonList = null;
        /// <summary>
        /// List property for CompanyPerson
        /// </summary>
        /// <value>The IssuerCompanyPerson list</value>
        [ACPropertyList(9999, "IssuerCompanyPerson")]
        public List<CompanyPerson> IssuerCompanyPersonList
        {
            get
            {
                return _IssuerCompanyPersonList;
            }
        }

        #endregion

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

        #endregion

        #region BSO->ACMethod

        [ACMethodCommand(OutOffer.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(OutOffer.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        [ACMethodInteraction(OutOffer.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedOutOffer", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<OutOffer>(requery, () => SelectedOutOffer, () => CurrentOutOffer, c => CurrentOutOffer = c,
                        DatabaseApp.OutOffer
                        .Include(c => c.OutOfferPos_OutOffer)
                        .Where(c => c.OutOfferID == SelectedOutOffer.OutOfferID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedOutOffer != null;
        }

        [ACMethodInteraction(OutOffer.ClassName, Const.New, (short)MISort.New, true, "SelectedOutOffer", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(OutOffer), OutOffer.NoColumnName, OutOffer.FormatNewNo, this);
            OutOffer outOffer = OutOffer.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.OutOffer.Add(outOffer);
            if (CurrentUserSettings != null)
            {
                outOffer.IssuerCompanyAddress = CurrentUserSettings.InvoiceCompanyAddress;
                outOffer.IssuerCompanyPerson = CurrentUserSettings.InvoiceCompanyPerson;
            }
            SelectedOutOffer = outOffer;
            CurrentOutOffer = outOffer;

            if (AccessPrimary != null)
                AccessPrimary.NavList.Add(CurrentOutOffer);

            ACState = Const.SMNew;
            PostExecute("New");
        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction(OutOffer.ClassName, "en{'New version'}de{'Neue Version'}", (short)MISort.New + 1, true, "", Global.ACKinds.MSMethodPrePost)]
        public void NewVersion()
        {
            OutOffer newOfferVersion = CurrentOutOffer.Clone() as OutOffer;
            if (newOfferVersion == null)
                return;

            newOfferVersion.OutOfferID = Guid.NewGuid();
            newOfferVersion.OutOfferVersion = DatabaseApp.OutOffer.Where(c => c.OutOfferNo == CurrentOutOffer.OutOfferNo).Max(v => v.OutOfferVersion) + 1;

            DatabaseApp.OutOffer.Add(newOfferVersion);

            foreach (OutOfferPos pos in CurrentOutOffer.OutOfferPos_OutOffer.Where(c => !c.GroupOutOfferPosID.HasValue))
            {
                NewVersionPos(newOfferVersion, pos, null);
            }

            DatabaseApp.ACSaveChanges();
            Search();
            SelectedOutOffer = newOfferVersion;
        }

        public bool IsEnabledNewVersion()
        {
            return CurrentOutOffer != null;
        }

        private void NewVersionPos(OutOffer outOffer, OutOfferPos pos, OutOfferPos groupPos)
        {
            OutOfferPos newPos = pos.Clone() as OutOfferPos;
            if (newPos == null)
                return;

            newPos.OutOfferPosID = Guid.NewGuid();
            newPos.OutOfferID = outOffer.OutOfferID;
            newPos.GroupOutOfferPosID = groupPos?.OutOfferPosID;

            outOffer.OutOfferPos_OutOffer.Add(newPos);
            DatabaseApp.OutOfferPos.Add(newPos);

            foreach (OutOfferPos subPos in pos.OutOfferPos_GroupOutOfferPos)
            {
                NewVersionPos(outOffer, subPos, newPos);
            }
        }

        [ACMethodInteraction(OutOffer.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentOutOffer", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentOutOffer.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentOutOffer);
            SelectedOutOffer = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        public bool IsEnabledDelete()
        {
            return true;
        }

        [ACMethodCommand(OutOffer.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("OutOfferList");
        }

        [ACMethodInteraction("OutOfferPos", "en{'Load Position'}de{'Position laden'}", (short)MISort.Load, false, "SelectedOutOfferPos", Global.ACKinds.MSMethodPrePost)]
        public void LoadOutOfferPos()
        {
            if (!IsEnabledLoadOutOfferPos())
                return;
            if (!PreExecute("LoadOutOfferPos")) return;
            // Laden des aktuell selektierten OutOfferPos 
            CurrentOutOfferPos = CurrentOutOffer.OutOfferPos_OutOffer.Where(c => c.OutOfferPosID == SelectedOutOfferPos.OutOfferPosID).FirstOrDefault();
            PostExecute("LoadOutOfferPos");
        }

        public bool IsEnabledLoadOutOfferPos()
        {
            return SelectedOutOfferPos != null && CurrentOutOffer != null;
        }

        [ACMethodInteraction("OutOfferPos", "en{'New Position'}de{'Neue Position'}", (short)MISort.New, true, "SelectedOutOfferPos", Global.ACKinds.MSMethodPrePost)]
        public void NewOutOfferPos()
        {
            if (!PreExecute("NewOutOfferPos")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            OutOfferPos groupPos = CurrentOutOfferPos?.OutOfferPos1_GroupOutOfferPos;
            CurrentOutOfferPos = OutOfferPos.NewACObject(DatabaseApp, CurrentOutOffer, groupPos);
            CurrentOutOfferPos.OutOffer = CurrentOutOffer;
            CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos = groupPos;
            CurrentOutOffer.OutOfferPos_OutOffer.Add(CurrentOutOfferPos);
            OnPropertyChanged("OutOfferPosList");
            PostExecute("NewOutOfferPos");
        }

        public bool IsEnabledNewOutOfferPos()
        {
            return CurrentOutOffer != null;
        }

        [ACMethodInteraction("OutOfferPos", "en{'New sub Position'}de{'Neue sub Position'}", (short)MISort.New, true, "SelectedOutOfferPos", Global.ACKinds.MSMethodPrePost)]
        public void NewSubOutOfferPos()
        {
            if (!PreExecute("NewSubOutOfferPos")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            OutOfferPos subOutOfferPos = OutOfferPos.NewACObject(DatabaseApp, CurrentOutOffer, CurrentOutOfferPos);
            subOutOfferPos.OutOfferPos1_GroupOutOfferPos = CurrentOutOfferPos;
            subOutOfferPos.OutOffer = CurrentOutOffer;
            CurrentOutOffer.OutOfferPos_OutOffer.Add(subOutOfferPos);
            OnPropertyChanged("OutOfferPosList");
            CurrentOutOfferPos = subOutOfferPos;
            PostExecute("NewSubOutOfferPos");
        }

        public bool IsEnabledSubOutOfferPos()
        {
            return SelectedOutOfferPos != null;
        }

        [ACMethodInteraction("OutOfferPos", "en{'Delete Position'}de{'Position löschen'}", (short)MISort.Delete, true, "CurrentOutOfferPos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteOutOfferPos()
        {
            if (!PreExecute("DeleteOutOfferPos")) return;
            Msg msg = CurrentOutOfferPos.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            PostExecute("DeleteOutOfferPos");
            OnPropertyChanged("OutOfferPosList");
            if (CurrentOutOffer != null)
            {
                CurrentOutOffer.OnPricePropertyChanged();
                OutDeliveryNoteManager.CalculateTaxOverview(this, CurrentOutOffer, CurrentOutOffer.OutOfferPos_OutOffer.Select(c => (IOutOrderPos)c).ToList());
            }
        }

        public bool IsEnabledDeleteOutOfferPos()
        {
            return CurrentOutOffer != null && CurrentOutOfferPos != null;
        }

        [ACMethodInteraction("OutOfferPos", "en{'Position up'}de{'Position oben'}", 10, true, "CurrentOutOfferPos", Global.ACKinds.MSMethodPrePost)]
        public void OutOrderPosUp()
        {
            int sequencePre = 0;
            if (CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos != null)
            {
                var posPre = CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos.OutOfferPos_GroupOutOfferPos
                                                 .Where(c => c.Sequence < CurrentOutOfferPos.Sequence).OrderByDescending(x => x.Sequence)
                                                 .FirstOrDefault();

                if (posPre == null)
                    return;

                sequencePre = posPre.Sequence;

                posPre.Sequence = CurrentOutOfferPos.Sequence;
                CurrentOutOfferPos.Sequence = sequencePre;
            }
            else
            {
                var posPre = OutOfferPosList.Where(c => c.OutOfferPos1_GroupOutOfferPos == null && c.Sequence < CurrentOutOfferPos.Sequence)
                                            .OrderByDescending(x => x.Sequence)
                                            .FirstOrDefault();

                if (posPre == null)
                    return;

                sequencePre = posPre.Sequence;
                posPre.Sequence = CurrentOutOfferPos.Sequence;
                CurrentOutOfferPos.Sequence = sequencePre;
            }
            OnPropertyChanged("OutOfferPosList");
        }

        public bool IsEnabledOutOrderPosUp()
        {
            return CurrentOutOfferPos != null && CurrentOutOfferPos.Sequence > 1;
        }

        [ACMethodInteraction("OutOfferPos", "en{'Position down'}de{'Position unten'}", 11, true, "CurrentOutOfferPos", Global.ACKinds.MSMethodPrePost)]
        public void OutOrderPosDown()
        {
            int sequencePre = 0;
            if (CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos != null)
            {
                var posPre = CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos.OutOfferPos_GroupOutOfferPos
                                                 .Where(c => c.Sequence > CurrentOutOfferPos.Sequence)
                                                 .OrderBy(x => x.Sequence)
                                                 .FirstOrDefault();

                if (posPre == null)
                    return;

                sequencePre = posPre.Sequence;

                posPre.Sequence = CurrentOutOfferPos.Sequence;
                CurrentOutOfferPos.Sequence = sequencePre;
            }
            else
            {
                var posPre = OutOfferPosList.Where(c => c.OutOfferPos1_GroupOutOfferPos == null && c.Sequence > CurrentOutOfferPos.Sequence)
                                            .OrderBy(x => x.Sequence)
                                            .FirstOrDefault();

                if (posPre == null)
                    return;

                sequencePre = posPre.Sequence;
                posPre.Sequence = CurrentOutOfferPos.Sequence;
                CurrentOutOfferPos.Sequence = sequencePre;
            }
            OnPropertyChanged("OutOfferPosList");
        }

        public bool IsEnabledOutOrderPosDown()
        {
            if (CurrentOutOfferPos != null)
            {
                if (CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos == null)
                {
                    return CurrentOutOfferPos.Sequence < OutOfferPosList.Where(x => x.OutOfferPos1_GroupOutOfferPos == null).Max(c => c.Sequence);
                }
                else if (CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos != null)
                {
                    return CurrentOutOfferPos.Sequence < CurrentOutOfferPos.OutOfferPos1_GroupOutOfferPos.OutOfferPos_GroupOutOfferPos.Max(x => x.Sequence);
                }

            }

            return false;
        }

        #region Properties => Report

        private ACComponent _BSOOutOfferReportHandler;
        public ACComponent BSOOutOfferReportHandler
        {
            get
            {
                return _BSOOutOfferReportHandler;
            }
        }

        #endregion

        #region Methods => Report

        public void BuildOutOfferPosData(string langCode)
        {
            if (CurrentOutOffer == null)
                return;

            List<OutOfferPos> posData = new List<OutOfferPos>();

            foreach (var outOfferPos in CurrentOutOffer.OutOfferPos_OutOffer.Where(c => c.GroupOutOfferPosID == null && c.PriceNet >= 0).OrderBy(p => p.Position))
            {
                posData.Add(outOfferPos);
                BuildOutOfferPosDataRecursive(posData, outOfferPos.Items);
                if (outOfferPos.GroupSum)
                {
                    OutOfferPos sumPos = new OutOfferPos();
                    sumPos.TotalPricePrinted = outOfferPos.Items.Sum(c => c.TotalPrice).ToString("N");
                    sumPos.MaterialNo = Root.Environment.TranslateMessageLC(this, "Info50063", langCode) + outOfferPos.Material.MaterialNo; // Info50063.
                    sumPos.Sequence = outOfferPos.Sequence;
                    sumPos.GroupSum = outOfferPos.GroupSum;
                    posData.Add(sumPos);
                }
            }

            OutOfferPosDataList = posData;
            OutOfferPosDiscountList = CurrentOutOffer.OutOfferPos_OutOffer.Where(c => c.PriceNet < 0).OrderBy(s => s.Sequence).ToList();
            if (OutOfferPosDiscountList != null && OutOfferPosDiscountList.Any())
            {
                //OutOfferPosDiscountList.Add(new OutOfferPos() { Comment = "Rabatt in Summe:", PriceNet = (decimal)CurrentOutOffer.PosPriceNetDiscount });
                OutOfferPosDiscountList.Add(new OutOfferPos() { Comment = Root.Environment.TranslateMessageLC(this, "Info50064", langCode), PriceNet = (decimal)CurrentOutOffer.PosPriceNetTotal }); //Info50064.
            }

            OutDeliveryNoteManager.CalculateTaxOverview(this, CurrentOutOffer, CurrentOutOffer.OutOfferPos_OutOffer.Select(c => (IOutOrderPos)c).ToList());
        }

        private void BuildOutOfferPosDataRecursive(List<OutOfferPos> posDataList, IEnumerable<OutOfferPos> outOfferPosList)
        {
            foreach (var outOfferPos in outOfferPosList.Where(c => c.PriceNet >= 0).OrderBy(p => p.Position))
            {
                posDataList.Add(outOfferPos);
                BuildOutOfferPosDataRecursive(posDataList, outOfferPos.Items);
            }
        }

        public override void OnPrintingPhase(object reportEngine, ACPrintingPhase printingPhase)
        {
            ACComponent childBSO = ACUrlCommand("BSOOutOfferReportHandler_Child") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("BSOOutOfferReportHandler_Child", null, new object[] { }) as ACComponent;
            _BSOOutOfferReportHandler = childBSO;

            if (BSOOutOfferReportHandler != null)
                BSOOutOfferReportHandler.OnPrintingPhase(reportEngine, printingPhase);
        }

        #endregion

        #region Methods -> Invoice

        [ACMethodCommand(OutOffer.ClassName, "en{'Create Order'}de{'Auftrag generieren'}", (short)MISort.Cancel)]
        public void CreateOutOrder()
        {
            if (!IsEnabledCreateOutOrder())
                return;
            Msg msg = OutDeliveryNoteManager.NewOutOrderFromOutOffer(DatabaseApp, CurrentOutOffer);
            if (msg != null)
                Messages.Msg(msg);
        }

        public bool IsEnabledCreateOutOrder()
        {
            return CurrentOutOffer != null && !CurrentOutOffer.OutOrder_BasedOnOutOffer.Any();
        }

        #endregion

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
                case "LoadOutOfferPos":
                    LoadOutOfferPos();
                    return true;
                case "IsEnabledLoadOutOfferPos":
                    result = IsEnabledLoadOutOfferPos();
                    return true;
                case "NewOutOfferPos":
                    NewOutOfferPos();
                    return true;
                case "IsEnabledNewOutOfferPos":
                    result = IsEnabledNewOutOfferPos();
                    return true;
                case "DeleteOutOfferPos":
                    DeleteOutOfferPos();
                    return true;
                case "IsEnabledDeleteOutOfferPos":
                    result = IsEnabledDeleteOutOfferPos();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}
