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
using System.Threading.Tasks;

namespace gip.bso.sales
{
    /// <summary>
    /// Version 3
    /// Folgende alte Masken sind in diesem BSO enthalten: 
    /// 1. Warenausgang->Bestellungen
    /// 2. Warenausgang->Bestellpositionen
    /// 3. Warenausgang->Bestellpositionen (Druckansicht)
    /// 
    /// Neue Masken:
    /// 1. Auftragsverwaltung
    /// 
    /// TODO: Betroffene Tabellen: OutOrder, OutOrderPos
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Sales Order'}de{'Kundenauftrag'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOrder.ClassName)]
    [ACQueryInfo(Const.PackName_VarioSales, Const.QueryPrefix + "OutOpenContractPos", "en{'Open Contract lines'}de{'Offene Kontraktpositionen'}", typeof(OutOrderPos), OutOrderPos.ClassName, MDDelivPosState.ClassName + "\\MDDelivPosStateIndex", "Material\\MaterialNo,TargetDeliveryDate")]
    public class BSOOutOrder : ACBSOvbNav, IOutOrderPosBSO
    {

        #region private
        private UserSettings CurrentUserSettings { get; set; }

        #endregion

        #region c´tors

        public BSOOutOrder(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
                throw new Exception("InDeliveryNoteManager not configured");

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _ProdOrderManager = ACProdOrderManager.ACRefToServiceInstance(this);
            if (_ProdOrderManager == null)
                throw new Exception("ProdOrderManager not configured");

            CurrentUserSettings = DatabaseApp.UserSettings.Where(c => c.VBUserID == Root.Environment.User.VBUserID).FirstOrDefault();

            Search();

            //IssuerResult issuerResult = OutDeliveryNoteManager.GetIssuer(DatabaseApp, Root.Environment.User.VBUserID);
            //IssuerCompanyAddressMessage = issuerResult.IssuerMessage;
            //_IssuerCompanyPersonList = issuerResult.CompanyPeople;
            //OnPropertyChanged("IssuerCompanyPersonList");
            //IssuerCompanyAddress = issuerResult.IssuerCompanyAddress;
            //SelectedIssuerCompanyPerson = issuerResult.IssuerCompanyPerson;

            if (BSOFacilityReservation_Child != null && BSOFacilityReservation_Child.Value != null)
            {
                BSOFacilityReservation_Child.Value.OnReservationChanged += BSOFacilityRservation_ReservationChanged;
            }

            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            ACOutDeliveryNoteManager.DetachACRefFromServiceInstance(this, _OutDeliveryNoteManager);
            _OutDeliveryNoteManager = null;
            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;
            ACProdOrderManager.DetachACRefFromServiceInstance(this, _ProdOrderManager);
            _ProdOrderManager = null;

            this._AccessOutOrderPos = null;
            this._ChangeTargetQuantity = null;
            this._CurrentCompanyMaterialPickup = null;
            this._CurrentMDUnit = null;
            this._CurrentOutOrderPos = null;
            this._SelectedAvailableCompMaterial = null;
            this._SelectedCompanyMaterialPickup = null;
            this._SelectedOutOrderPos = null;
            this._UnSavedUnAssignedContractPos = null;
            this._PartialQuantity = null;
            _SelectedFilterMaterial = null;

            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

            bool result = await base.ACDeInit(deleteACClassTask);


            if (_AccessOutOrderPos != null)
            {
                await _AccessOutOrderPos.ACDeInit(false);
                _AccessOutOrderPos = null;
            }
            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            if (_AccessOpenContractPos != null)
            {
                await _AccessOpenContractPos.ACDeInit(false);
                _AccessOpenContractPos = null;
            }
            if (_AccessFilterMaterial != null)
            {
                await _AccessFilterMaterial.ACDeInit(false);
                _AccessFilterMaterial = null;
            }

            if (BSOFacilityReservation_Child != null && BSOFacilityReservation_Child.Value != null)
            {
                BSOFacilityReservation_Child.Value.OnReservationChanged -= BSOFacilityRservation_ReservationChanged;
            }

            return result;

        }

        private void BSOFacilityRservation_ReservationChanged()
        {
            if (CurrentOutOrderPos != null)
            {
                CurrentOutOrderPos.OnEntityPropertyChanged(nameof(Material));
            }
        }

        #endregion

        #region ChildBSO

        ACChildItem<BSOFacilityReservation> _BSOFacilityReservation_Child;
        [ACPropertyInfo(600)]
        [ACChildInfo(nameof(BSOFacilityReservation_Child), typeof(BSOFacilityReservation))]
        public ACChildItem<BSOFacilityReservation> BSOFacilityReservation_Child
        {
            get
            {
                if (_BSOFacilityReservation_Child == null)
                    _BSOFacilityReservation_Child = new ACChildItem<BSOFacilityReservation>(this, nameof(BSOFacilityReservation_Child));
                return _BSOFacilityReservation_Child;
            }
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

        public gip.mes.datamodel.MDOutOrderState.OutOrderStates? FilterOrderState
        {
            get
            {
                if (SelectedFilterOrderState == null) return null;
                return (gip.mes.datamodel.MDOutOrderState.OutOrderStates)Enum.Parse(typeof(gip.mes.datamodel.MDOutOrderState.OutOrderStates), SelectedFilterOrderState.Value.ToString());
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
                    _FilterOrderStateList = new ACValueItemList("OutOrderStatesList");
                    _FilterOrderStateList.AddRange(DatabaseApp.MDOutOrderState.ToList().Select(x => new ACValueItem(x.MDOutOrderStateName, x.MDOutOrderStateIndex, null)).ToList());
                }
                return _FilterOrderStateList;
            }
        }

        [ACPropertyInfo(300, nameof(FilterNo), "en{'Order No.'}de{'Auftrag Nr.'}")]
        public string FilterNo
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(OutOrder.NoColumnName);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(OutOrder.NoColumnName);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(OutOrder.NoColumnName, value);
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

        protected ACRef<ACOutDeliveryNoteManager> _OutDeliveryNoteManager = null;
        public ACOutDeliveryNoteManager OutDeliveryNoteManager
        {
            get
            {
                if (_OutDeliveryNoteManager == null)
                    return null;
                return _OutDeliveryNoteManager.ValueT;
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

        #region 1. OutOrder
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        ACAccessNav<OutOrder> _AccessPrimary;
        [ACPropertyAccessPrimary(690, OutOrder.ClassName)]
        public ACAccessNav<OutOrder> AccessPrimary
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
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<OutOrder>(OutOrder.ClassName, this);
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
                    new ACFilterItem(Global.FilterTypes.filter, OutOrder.NoColumnName, Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
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
                    new ACSortItem(OutOrder.NoColumnName, Global.SortDirections.descending, true)
                };
            }
        }

        private IQueryable<OutOrder> _AccessPrimary_NavSearchExecuting(IQueryable<OutOrder> result)
        {
            IQueryable<OutOrder> query = result as IQueryable<OutOrder>;
            if (query != null)
            {
                query.Include(c => c.MDOutOrderType)
                     .Include(c => c.MDOutOrderState)
                     .Include(c => c.CPartnerCompany)
                     .Include(c => c.CustomerCompany)
                     .Include(c => c.BillingCompanyAddress)
                     .Include(c => c.DeliveryCompanyAddress)
                     .Include(c => c.MDTermOfPayment)
                     .Include(c => c.MDDelivType)
                    .Include("OutOrderPos_InOrder")
                    .Include("OutOrderPos_InOrder.Material");
            }
            if (SelectedFilterMaterial != null)
                result = result.Where(c => c.OutOrderPos_OutOrder.Any(mt => mt.MaterialID == SelectedFilterMaterial.MaterialID));
            if (FilterOrderState.HasValue)
                result = result.Where(x => x.MDOutOrderState.MDOutOrderStateIndex == (short)FilterOrderState.Value);
            return result;
        }

        [ACPropertyCurrent(600, OutOrder.ClassName)]
        public OutOrder CurrentOutOrder
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Current;
            }
            set
            {
                if (CurrentOutOrder != null)
                    CurrentOutOrder.PropertyChanged -= CurrentOutOrder_PropertyChanged;
                if (AccessPrimary == null)
                    return;
                if (AccessPrimary.Current != value)
                {
                    AccessPrimary.Current = value;

                    if (CurrentOutOrder != null)
                        CurrentOutOrder.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CurrentOutOrder_PropertyChanged);
                    CurrentOutOrderPos = null;
                    OnPropertyChanged("CurrentOutOrder");
                    OnPropertyChanged("OutOrderPosList");
                    OnPropertyChanged("CompanyList");
                    OnPropertyChanged("BillingCompanyAddressList");
                    OnPropertyChanged("DeliveryCompanyAddressList");
                    OnPropertyChanged("CurrentBillingCompanyAddress");
                    OnPropertyChanged("CurrentDeliveryCompanyAddress");
                    OnCurrentOutOrderChanged();
                    ResetAccessTenantCompanyFilter(value);
                }
            }
        }

        void CurrentOutOrder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (OutDeliveryNoteManager != null)
                OutDeliveryNoteManager.HandleIOrderPropertyChange(e.PropertyName, CurrentOutOrder);
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

        public void OnPricePropertyChanged()
        {
            if (CurrentOutOrder != null)
            {
                CurrentOutOrder.OnPricePropertyChanged();
            }
        }

        [ACPropertyList(601, OutOrder.ClassName)]
        public IEnumerable<OutOrder> OutOrderList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, OutOrder.ClassName)]
        public OutOrder SelectedOutOrder
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
                OnPropertyChanged("SelectedOutOrder");
            }
        }
        #endregion

        #region 1.1 OutOrderPos
        ACAccess<OutOrderPos> _AccessOutOrderPos;
        [ACPropertyAccess(691, OutOrderPos.ClassName)]
        public ACAccess<OutOrderPos> AccessOutOrderPos
        {
            get
            {
                if (_AccessOutOrderPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + OutOrderPos.ClassName) as ACQueryDefinition;
                    _AccessOutOrderPos = acQueryDefinition.NewAccess<OutOrderPos>(OutOrderPos.ClassName, this);
                }
                return _AccessOutOrderPos;
            }
        }

        OutOrderPos _CurrentOutOrderPos;
        [ACPropertyCurrent(603, OutOrderPos.ClassName)]
        public OutOrderPos CurrentOutOrderPos
        {
            get
            {
                return _CurrentOutOrderPos;
            }
            set
            {
                if (_CurrentOutOrderPos != value)
                {
                    if (_CurrentOutOrderPos != null)
                        _CurrentOutOrderPos.PropertyChanged -= CurrentOutOrderPos_PropertyChanged;
                    _CurrentOutOrderPos = value;
                    if (_CurrentOutOrderPos != null)
                        _CurrentOutOrderPos.PropertyChanged += CurrentOutOrderPos_PropertyChanged;
                    OnPropertyChanged("CurrentOutOrderPos");
                    OnPropertyChanged("MDUnitList");
                    OnPropertyChanged("CompanyMaterialPickupList");
                    OnPropertyChanged("AvailableCompMaterialList");

                    if (BSOFacilityReservation_Child != null && BSOFacilityReservation_Child.Value != null)
                    {
                        BSOFacilityReservation_Child.Value.FacilityReservationOwner = value;
                    }
                }
            }
        }

        void CurrentOutOrderPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (OutDeliveryNoteManager != null)
                OutDeliveryNoteManager.HandleIOrderPosPropertyChange(DatabaseApp, this, e.PropertyName,
                    CurrentOutOrder, CurrentOutOrderPos, CurrentOutOrder.OutOrderPos_OutOrder.Select(c => (IOutOrderPos)c).ToList(), CurrentOutOrder?.BillingCompanyAddress);
        }

        /// <summary>
        /// OutOrderPos list of lines
        /// </summary>
        [ACPropertyList(604, OutOrderPos.ClassName)]
        public IEnumerable<OutOrderPos> OutOrderPosList
        {
            get
            {
                if (CurrentOutOrder == null || CurrentOutOrder.MDOutOrderType == null)
                    return null;
                if (CurrentOutOrder.MDOutOrderType.OrderType == GlobalApp.OrderTypes.ReleaseOrder)
                {
                    return CurrentOutOrder.OutOrderPos_OutOrder
                        .Where(c => c.MaterialPosTypeIndex == (int)GlobalApp.MaterialPosTypes.OutwardPart && !c.DeliveryNotePos_OutOrderPos.Any())
                        .OrderBy(c => c.Position);
                }
                else
                {
                    return CurrentOutOrder.OutOrderPos_OutOrder
                        .Where(c => !c.ParentOutOrderPosID.HasValue)
                        .OrderBy(c => c.Position);
                }
            }
        }

        OutOrderPos _SelectedOutOrderPos;
        /// <summary>
        /// Selected OutOrderPos
        /// </summary>
        [ACPropertySelected(605, OutOrderPos.ClassName)]
        public OutOrderPos SelectedOutOrderPos
        {
            get
            {
                return _SelectedOutOrderPos;
            }
            set
            {
                _SelectedOutOrderPos = value;
            }
        }

        #region Property -> OutOrderPos -> MDUnit

        /// <summary>
        /// Gets the MU quantity unit list.
        /// </summary>
        /// <value>The MU quantity unit list.</value>
        [ACPropertyList(606, MDUnit.ClassName)]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                if (CurrentOutOrderPos == null || CurrentOutOrderPos.Material == null)
                    return null;
                return CurrentOutOrderPos.Material.MDUnitList;
            }
        }

        MDUnit _CurrentMDUnit;
        /// <summary>
        /// Gets or sets the current MU quantity unit.
        /// </summary>
        /// <value>The current MU quantity unit.</value>
        [ACPropertyCurrent(607, MDUnit.ClassName, "en{'New Unit'}de{'Neue Einheit'}")]
        public MDUnit CurrentMDUnit
        {
            get
            {
                return _CurrentMDUnit;
            }
            set
            {
                _CurrentMDUnit = value;
                if (CurrentOutOrderPos != null && CurrentOutOrderPos.MDUnit != value)
                {
                    CurrentOutOrderPos.MDUnit = value;
                    if (CurrentOutOrderPos.MDUnit != null)
                    {
                        CurrentOutOrderPos.TargetQuantity = CurrentOutOrderPos.Material.ConvertQuantity(CurrentOutOrderPos.TargetQuantityUOM, CurrentOutOrderPos.Material.BaseMDUnit, CurrentOutOrderPos.MDUnit);
                        CurrentOutOrderPos.ActualQuantity = CurrentOutOrderPos.Material.ConvertQuantity(CurrentOutOrderPos.ActualQuantityUOM, CurrentOutOrderPos.Material.BaseMDUnit, CurrentOutOrderPos.MDUnit);
                        CurrentOutOrderPos.CalledUpQuantity = CurrentOutOrderPos.Material.ConvertQuantity(CurrentOutOrderPos.CalledUpQuantityUOM, CurrentOutOrderPos.Material.BaseMDUnit, CurrentOutOrderPos.MDUnit);
                        CurrentOutOrderPos.ExternQuantity = CurrentOutOrderPos.Material.ConvertQuantity(CurrentOutOrderPos.ExternQuantityUOM, CurrentOutOrderPos.Material.BaseMDUnit, CurrentOutOrderPos.MDUnit);
                    }
                    OnPropertyChanged("CurrentOutOrderPos");
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
                if (_ChangeTargetQuantity.HasValue && (_ChangeTargetQuantity.Value > 0) && CurrentOutOrderPos != null && CurrentOutOrderPos.Material != null)
                {
                    //CurrentOutOrderPos.TargetQuantityUOM = CurrentOutOrderPos.Material.ConvertToBaseQuantity(_ChangeTargetQuantity.Value, CurrentOutOrderPos.MDUnit);
                    //CurrentOutOrderPos.TargetQuantity = CurrentOutOrderPos.Material.ConvertQuantity(CurrentOutOrderPos.TargetQuantityUOM, CurrentOutOrderPos.Material.BaseMDUnit, CurrentOutOrderPos.MDUnit);
                    CurrentOutOrderPos.TargetQuantity = _ChangeTargetQuantity.Value;
                }
                _ChangeTargetQuantity = null;
                OnPropertyChanged("ChangeTargetQuantity");
            }
        }
        #endregion

        #region 1.1.1 CompanyMaterialPickup
        CompanyMaterialPickup _CurrentCompanyMaterialPickup;
        [ACPropertyCurrent(609, "CompanyMaterialPickup")]
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
        [ACPropertySelected(610, "CompanyMaterialPickup")]
        public CompanyMaterialPickup SelectedCompanyMaterialPickup
        {
            get
            {
                return _SelectedCompanyMaterialPickup;
            }
            set
            {
                _SelectedCompanyMaterialPickup = value;
            }
        }

        [ACPropertyList(611, "CompanyMaterialPickup")]
        public IEnumerable<CompanyMaterialPickup> CompanyMaterialPickupList
        {
            get
            {
                if (CurrentOutOrderPos == null)
                    return null;
                return CurrentOutOrderPos.CompanyMaterialPickup_OutOrderPos;
            }
        }

        CompanyMaterial _SelectedAvailableCompMaterial;
        [ACPropertySelected(612, "AvailableCompMatPickups", "en{'Available Ext. Material'}de{'Verfügbares externes Material'}")]
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

        [ACPropertyList(613, "AvailableCompMatPickups")]
        public IEnumerable<CompanyMaterial> AvailableCompMaterialList
        {
            get
            {
                if (CurrentOutOrder == null || CurrentOutOrderPos == null || CurrentOutOrder.CustomerCompany == null)
                    return null;
                return DatabaseApp.CompanyMaterial.Where(c => c.CompanyID == CurrentOutOrder.CustomerCompanyID
                                                        && c.MaterialID == CurrentOutOrderPos.MaterialID
                                                        && c.CMTypeIndex == (short)GlobalApp.CompanyMaterialTypes.Pickup);
            }
        }
        #endregion

        #region Company
        /// <summary>
        /// Liste aller Unternehmen, die Lieferanten sind
        /// </summary>
        [ACPropertyList(614, Company.ClassName)]
        public IEnumerable<Company> CompanyList
        {
            get
            {
                return DatabaseApp.Company
                    .Where(c => c.IsCustomer)
                    .OrderBy(c => c.CompanyName);
            }
        }

        [ACPropertyList(615, "BillingCompanyAddress")]
        public IEnumerable<CompanyAddress> BillingCompanyAddressList
        {
            get
            {
                if (CurrentOutOrder == null || CurrentOutOrder.CustomerCompany == null)
                    return null;
                if (!CurrentOutOrder.CustomerCompany.CompanyAddress_Company_IsLoaded)
                    CurrentOutOrder.CustomerCompany.CompanyAddress_Company.AutoLoad(CurrentOutOrder.CustomerCompany.CompanyAddress_CompanyReference, CurrentOutOrder);
                return CurrentOutOrder.CustomerCompany.CompanyAddress_Company
                    .Where(c => c.IsBillingCompanyAddress)
                    .OrderBy(c => c.Name1);
            }
        }

        [ACPropertyList(616, "DeliveryCompanyAddress")]
        public IEnumerable<CompanyAddress> DeliveryCompanyAddressList
        {
            get
            {
                if (CurrentOutOrder == null || CurrentOutOrder.CustomerCompany == null)
                    return null;
                if (!CurrentOutOrder.CustomerCompany.CompanyAddress_Company_IsLoaded)
                    CurrentOutOrder.CustomerCompany.CompanyAddress_Company.AutoLoad(CurrentOutOrder.CustomerCompany.CompanyAddress_CompanyReference, CurrentOutOrder);
                return CurrentOutOrder.CustomerCompany.CompanyAddress_Company
                    .Where(c => c.IsDeliveryCompanyAddress)
                    .OrderBy(c => c.Name1);
            }
        }

        [ACPropertyCurrent(617, "BillingCompanyAddress", ConstApp.BillingCompanyAddress)]
        public CompanyAddress CurrentBillingCompanyAddress
        {
            get
            {
                if (CurrentOutOrder == null)
                    return null;
                return CurrentOutOrder.BillingCompanyAddress;
            }
            //set
            //{
            //    if (CurrentOutOrder == null || value == null)
            //        return;
            //    CurrentOutOrder.BillingCompanyAddress = value;
            //    OnPropertyChanged("CurrentBillingCompanyAddress");
            //    if (CurrentOutOrder.MDCurrency == null && CurrentOutOrder.BillingCompanyAddress != null)
            //        CurrentOutOrder.MDCurrency = CurrentOutOrder.BillingCompanyAddress.MDCountry.MDCurrency;
            //}
        }

        [ACPropertyCurrent(618, "DeliveryCompanyAddress", ConstApp.DeliveryCompanyAddress)]
        public CompanyAddress CurrentDeliveryCompanyAddress
        {
            get
            {
                if (CurrentOutOrder == null)
                    return null;
                return CurrentOutOrder.DeliveryCompanyAddress;
            }
            set
            {
                if (CurrentOutOrder != null && value != null)
                {
                    CurrentOutOrder.DeliveryCompanyAddress = value;
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
        [ACPropertyAccess(692, "OpenContractPos")]
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

        public const string _CTargetDeliveryDateProperty = OutOrder.ClassName + "\\" + "TargetDeliveryDate";

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

        public const string _CTargetDeliveryMaxDateProperty = OutOrder.ClassName + "\\" + "TargetDeliveryMaxDate";
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
        [ACPropertyList(614, "OpenContractPos")]
        public IEnumerable<OutOrderPos> OpenContractPosList
        {
            get
            {
                if (AccessOpenContractPos == null)
                    return null;
                if (CurrentOutOrderPos != null)
                {
                    IEnumerable<OutOrderPos> addedPositions = CurrentOutOrderPos.OutOrderPos_ParentOutOrderPos.Where(c => c.EntityState == EntityState.Added
                        && c != null
                        && c.OutOrderPos1_ParentOutOrderPos != null
                        && c.OutOrderPos1_ParentOutOrderPos.MDDelivPosState == StateCompletelyAssigned
                        ).Select(c => c.OutOrderPos1_ParentOutOrderPos);
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


        [ACPropertyList(616, "TransportMode")]
        public IEnumerable<MDTransportMode> MDTransportModeList
        {
            get
            {
                return DatabaseApp.MDTransportMode.ToList();
            }
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
                    CurrentOutOrderPos.PriceNet = _SelectedPriceListMaterial.Price;
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

        protected ACRef<ACProdOrderManager> _ProdOrderManager = null;
        protected ACProdOrderManager ProdOrderManager
        {
            get
            {
                if (_ProdOrderManager == null)
                    return null;
                return _ProdOrderManager.ValueT;
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
        public List<OutOrderPos> OutOrderPosDataList
        {
            get;
            set;
        }

        [ACPropertyInfo(651)]
        public List<OutOrderPos> OutOrderPosDiscountList
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
                case nameof(CurrentOutOrder) + "\\" + nameof(MDOutOrderType):
                    {
                        if (OutOrderPosList != null && OutOrderPosList.Any())
                            return Global.ControlModes.Disabled;
                        break;
                    }
                case nameof(CurrentOutOrderPos) + "\\" + nameof(Material):
                    {
                        if (CurrentOutOrderPos != null && CurrentOutOrderPos.FacilityReservation_OutOrderPos.Any())
                            return Global.ControlModes.Disabled;
                        break;
                    }
            }

            return result;
        }

        #endregion

        #region OutOrder
        [ACMethodCommand(OutOrder.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public async Task Save()
        {
            await OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(OutOrder.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
            if (CurrentOutOrder != null
                && CurrentOutOrder.EntityState != EntityState.Added
                && CurrentOutOrder.EntityState != EntityState.Detached)
                CurrentOutOrder.OutOrderPos_OutOrder.AutoLoad(CurrentOutOrder.OutOrderPos_OutOrderReference, CurrentOutOrder);
            OnPropertyChanged("OutOrderPosList");
            base.OnPostUndoSave();
        }


        [ACMethodInteraction(OutOrder.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedOutOrder", Global.ACKinds.MSMethodPrePost)]
        public virtual void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<OutOrder>(requery, () => SelectedOutOrder, () => CurrentOutOrder, c => CurrentOutOrder = c,
                        DatabaseApp.OutOrder
                        .Include(c => c.OutOrderPos_OutOrder)
                        .Where(c => c.OutOrderID == SelectedOutOrder.OutOrderID));
            PostExecute("Load");
        }

        public bool IsEnabledLoad()
        {
            return SelectedOutOrder != null;
        }

        [ACMethodInteraction(OutOrder.ClassName, Const.New, (short)MISort.New, true, "SelectedOutOrder", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New"))
                return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(OutOrder), OutOrder.NoColumnName, OutOrder.FormatNewNo, this);
            OutOrder outOrder = OutOrder.NewACObject(DatabaseApp, null, secondaryKey);
            if (CurrentUserSettings != null)
            {
                outOrder.IssuerCompanyAddress = CurrentUserSettings.InvoiceCompanyAddress;
                outOrder.IssuerCompanyPerson = CurrentUserSettings.InvoiceCompanyPerson;
            }
            DatabaseApp.OutOrder.Add(outOrder);
            if (AccessPrimary != null)
                AccessPrimary.NavList.Add(CurrentOutOrder);
            CurrentOutOrder = outOrder;
            ACState = Const.SMNew;
            PostExecute("New");

        }

        public bool IsEnabledNew()
        {
            return true;
        }

        [ACMethodInteraction(OutOrder.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentOutOrder", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            Msg msg = CurrentOutOrder.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavList.Remove(CurrentOutOrder);
            SelectedOutOrder = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");

        }

        public bool IsEnabledDelete()
        {
            return CurrentOutOrder != null;
        }

        [ACMethodCommand(OutOrder.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("OutOrderList");
        }

        public virtual void OnCurrentOutOrderChanged()
        {

        }
        #endregion

        #region OutOrderPos

        #region OutOrderPos -> Manipulate (New, Edit, Delete ...)
        //[ACMethodInteraction(OutOrderPos.ClassName, "en{'New Item'}de{'Neue Position'}", (short)MISort.New, true, "SelectedOutOrderPos", Global.ACKinds.MSMethodPrePost)]
        //public void NewOutOrderPos()
        //{
        //    if (!PreExecute("NewOutOrderPos"))
        //        return;
        //    // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
        //    var outOrderPos = OutOrderPos.NewACObject(DatabaseApp, CurrentOutOrder);
        //    OnPropertyChanged("OutOrderPosList");
        //    CurrentOutOrderPos = outOrderPos;
        //    SelectedOutOrderPos = outOrderPos;
        //    PostExecute("NewOutOrderPos");
        //}

        //public bool IsEnabledNewOutOrderPos()
        //{
        //    return true;
        //}

        //[ACMethodInteraction(OutOrderPos.ClassName, "en{'Delete Item'}de{'Position löschen'}", (short)MISort.Delete, true, "CurrentOutOrderPos", Global.ACKinds.MSMethodPrePost)]
        //public void DeleteOutOrderPos()
        //{
        //    if (!PreExecute("DeleteOutOrderPos"))
        //        return;
        //    if (IsEnabledUnAssignContractPos())
        //    {
        //        UnAssignContractPos();
        //    }
        //    else
        //    {
        //        Msg msg = CurrentOutOrderPos.DeleteACObject(DatabaseApp, true);
        //        if (msg != null)
        //        {
        //            Messages.MsgAsync(msg);
        //            return;
        //        }
        //        OnPropertyChanged("OutOrderPosList");
        //    }
        //    PostExecute("DeleteOutOrderPos");
        //}

        //public bool IsEnabledDeleteOutOrderPos()
        //{
        //    return CurrentOutOrder != null && CurrentOutOrderPos != null;
        //}


        [ACMethodInteraction("OutOrderPos", "en{'New Position'}de{'Neue Position'}", (short)MISort.New, true, "SelectedOutOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void NewOutOrderPos()
        {
            if (!PreExecute("NewOutOrderPos")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            OutOrderPos groupPos = CurrentOutOrderPos?.OutOrderPos1_GroupOutOrderPos;
            CurrentOutOrderPos = OutOrderPos.NewACObject(DatabaseApp, CurrentOutOrder, groupPos);
            CurrentOutOrderPos.OutOrder = CurrentOutOrder;
            CurrentOutOrderPos.OutOrderPos1_GroupOutOrderPos = groupPos;
            CurrentOutOrder.OutOrderPos_OutOrder.Add(CurrentOutOrderPos);
            OnPropertyChanged("OutOrderPosList");
            PostExecute("NewOutOrderPos");
        }

        public bool IsEnabledNewOutOrderPos()
        {
            return CurrentOutOrder != null;
        }

        [ACMethodInteraction("OutOrderPos", "en{'New sub Position'}de{'Neue sub Position'}", (short)MISort.New, true, "SelectedOutOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void NewSubOutOrderPos()
        {
            if (!PreExecute("NewSubOutOrderPos")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            OutOrderPos subOutOrderPos = OutOrderPos.NewACObject(DatabaseApp, CurrentOutOrder, CurrentOutOrderPos);
            subOutOrderPos.OutOrderPos1_GroupOutOrderPos = CurrentOutOrderPos;
            subOutOrderPos.OutOrder = CurrentOutOrder;
            CurrentOutOrder.OutOrderPos_OutOrder.Add(subOutOrderPos);
            OnPropertyChanged("OutOrderPosList");
            CurrentOutOrderPos = subOutOrderPos;
            PostExecute("NewSubOutOrderPos");
        }

        public bool IsEnabledSubOutOrderPos()
        {
            return SelectedOutOrderPos != null;
        }

        [ACMethodInteraction("OutOrderPos", "en{'Delete Position'}de{'Position löschen'}", (short)MISort.Delete, true, "CurrentOutOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteOutOrderPos()
        {
            if (!PreExecute("DeleteOutOrderPos")) return;
            Msg msg = CurrentOutOrderPos.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            PostExecute("DeleteOutOrderPos");
            OnPropertyChanged("OutOrderPosList");
            if (CurrentOutOrder != null)
            {
                CurrentOutOrder.OnPricePropertyChanged();
                OutDeliveryNoteManager.CalculateTaxOverview(this, CurrentOutOrder, CurrentOutOrder.OutOrderPos_OutOrder.Select(c => (IOutOrderPos)c).ToList());
            }
        }

        public bool IsEnabledDeleteOutOrderPos()
        {
            return CurrentOutOrder != null && CurrentOutOrderPos != null;
        }

        [ACMethodInteraction("OutOrderPos", "en{'Position up'}de{'Position oben'}", 10, true, "CurrentOutOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void OutOrderPosUp()
        {
            int sequencePre = 0;
            if (CurrentOutOrderPos.OutOrderPos1_GroupOutOrderPos != null)
            {
                var posPre = CurrentOutOrderPos.OutOrderPos1_GroupOutOrderPos.OutOrderPos_GroupOutOrderPos
                                                 .Where(c => c.Sequence < CurrentOutOrderPos.Sequence).OrderByDescending(x => x.Sequence)
                                                 .FirstOrDefault();

                if (posPre == null)
                    return;

                sequencePre = posPre.Sequence;

                posPre.Sequence = CurrentOutOrderPos.Sequence;
                CurrentOutOrderPos.Sequence = sequencePre;
            }
            else
            {
                var posPre = OutOrderPosList.Where(c => c.OutOrderPos1_GroupOutOrderPos == null && c.Sequence < CurrentOutOrderPos.Sequence)
                                            .OrderByDescending(x => x.Sequence)
                                            .FirstOrDefault();

                if (posPre == null)
                    return;

                sequencePre = posPre.Sequence;
                posPre.Sequence = CurrentOutOrderPos.Sequence;
                CurrentOutOrderPos.Sequence = sequencePre;
            }
            OnPropertyChanged("OutOrderPosList");
        }

        public bool IsEnabledOutOrderPosUp()
        {
            return CurrentOutOrderPos != null && CurrentOutOrderPos.Sequence > 1;
        }

        [ACMethodInteraction("OutOrderPos", "en{'Position down'}de{'Position unten'}", 11, true, "CurrentOutOrderPos", Global.ACKinds.MSMethodPrePost)]
        public void OutOrderPosDown()
        {
            int sequencePre = 0;
            if (CurrentOutOrderPos.OutOrderPos1_GroupOutOrderPos != null)
            {
                var posPre = CurrentOutOrderPos.OutOrderPos1_GroupOutOrderPos.OutOrderPos_GroupOutOrderPos
                                                 .Where(c => c.Sequence > CurrentOutOrderPos.Sequence)
                                                 .OrderBy(x => x.Sequence)
                                                 .FirstOrDefault();

                if (posPre == null)
                    return;

                sequencePre = posPre.Sequence;

                posPre.Sequence = CurrentOutOrderPos.Sequence;
                CurrentOutOrderPos.Sequence = sequencePre;
            }
            else
            {
                var posPre = OutOrderPosList.Where(c => c.OutOrderPos1_GroupOutOrderPos == null && c.Sequence > CurrentOutOrderPos.Sequence)
                                            .OrderBy(x => x.Sequence)
                                            .FirstOrDefault();

                if (posPre == null)
                    return;

                sequencePre = posPre.Sequence;
                posPre.Sequence = CurrentOutOrderPos.Sequence;
                CurrentOutOrderPos.Sequence = sequencePre;
            }
            OnPropertyChanged("OutOrderPosList");
        }

        public bool IsEnabledOutOrderPosDown()
        {
            if (CurrentOutOrderPos != null)
            {
                if (CurrentOutOrderPos.OutOrderPos1_GroupOutOrderPos == null)
                {
                    return CurrentOutOrderPos.Sequence < OutOrderPosList.Where(x => x.OutOrderPos1_GroupOutOrderPos == null).Max(c => c.Sequence);
                }
                else if (CurrentOutOrderPos.OutOrderPos1_GroupOutOrderPos != null)
                {
                    return CurrentOutOrderPos.Sequence < CurrentOutOrderPos.OutOrderPos1_GroupOutOrderPos.OutOrderPos_GroupOutOrderPos.Max(x => x.Sequence);
                }

            }

            return false;
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
            CurrentCompanyMaterialPickup = CurrentOutOrderPos.CompanyMaterialPickup_OutOrderPos
                                           .Where(c => c.CompanyMaterialPickupID == SelectedCompanyMaterialPickup.CompanyMaterialPickupID)
                                           .FirstOrDefault();
            PostExecute("LoadCompanyMaterialPickup");
        }

        public bool IsEnabledLoadCompanyMaterialPickup()
        {
            return SelectedCompanyMaterialPickup != null && CurrentOutOrderPos != null;
        }

        [ACMethodInteraction("CompanyMaterialPickup", "en{'New Pick Up'}de{'Neuer Verladestamm'}", (short)MISort.New, true, "SelectedCompanyMaterialPickup", Global.ACKinds.MSMethodPrePost)]
        public void NewCompanyMaterialPickup()
        {
            if (!PreExecute("NewCompanyMaterialPickup"))
                return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            var companyMaterialPickup = CompanyMaterialPickup.NewACObject(DatabaseApp, CurrentOutOrderPos);
            companyMaterialPickup.CompanyMaterial = SelectedAvailableCompMaterial;
            OnPropertyChanged("CompanyMaterialPickupList");
            CurrentCompanyMaterialPickup = companyMaterialPickup;
            SelectedCompanyMaterialPickup = companyMaterialPickup;
            PostExecute("NewCompanyMaterialPickup");
        }

        public bool IsEnabledNewCompanyMaterialPickup()
        {
            if (SelectedAvailableCompMaterial == null || CurrentOutOrderPos == null || SelectedAvailableCompMaterial.MaterialID != CurrentOutOrderPos.MaterialID)
                return false;
            return true;
        }

        [ACMethodInteraction("CompanyMaterialPickup", "en{'Delete Pick Up'}de{'Verladestamm löschen'}", (short)MISort.Delete, true, "CurrentCompanyMaterialPickup", Global.ACKinds.MSMethodPrePost)]
        public void DeleteCompanyMaterialPickup()
        {
            if (!PreExecute("DeleteCompanyMaterialPickup"))
                return;
            Msg msg = CurrentCompanyMaterialPickup.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            OnPropertyChanged("CompanyMaterialPickupList");

            PostExecute("DeleteCompanyMaterialPickup");
        }

        public bool IsEnabledDeleteCompanyMaterialPickup()
        {
            return CurrentOutOrderPos != null && CurrentCompanyMaterialPickup != null;
        }
        #endregion

        #region OrderDialog
        public VBDialogResult DialogResult { get; set; }

        [ACMethodInfo("Dialog", "en{'Dialog sales Order'}de{'Dialog Auftrag'}", (short)MISort.QueryPrintDlg + 1)]
        public async Task ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;

            OutOrderPos OutOrderPos = null;
            OutOrder OutOrder = null;
            foreach (var entry in paOrderInfo.Entities)
            {
                if (entry.EntityName == OutOrder.ClassName)
                {
                    OutOrder = this.DatabaseApp.OutOrder
                        .Where(c => c.OutOrderID == entry.EntityID)
                        .FirstOrDefault();
                }
                else if (entry.EntityName == OutOrderPos.ClassName)
                {
                    OutOrderPos = this.DatabaseApp.OutOrderPos
                        .Include(c => c.OutOrder)
                        .Where(c => c.OutOrderPosID == entry.EntityID)
                        .FirstOrDefault();
                    if (OutOrderPos != null)
                        OutOrder = OutOrderPos.OutOrder;
                }
            }

            if (OutOrder == null)
                return;

            await ShowDialogOrder(OutOrder.OutOrderNo, OutOrderPos != null ? OutOrderPos.OutOrderPosID : (Guid?)null);
            paOrderInfo.DialogResult = this.DialogResult;
        }


        [ACMethodInfo("Dialog", "en{'Dialog Purchase Order'}de{'Dialog Bestellung'}", (short)MISort.QueryPrintDlg)]
        public async Task ShowDialogOrder(string OutOrderNo, Guid? OutOrderPosID)
        {
            if (AccessPrimary == null)
                return;
            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "OutOrderNo").FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, "OutOrderNo", Global.LogicalOperators.contains, Global.Operators.and, OutOrderNo, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            else
                filterItem.SearchWord = OutOrderNo;

            this.Search();
            if (CurrentOutOrder != null && OutOrderPosID != null)
            {
                OnPropertyChanged("OutOrderPosList");
                if (OutOrderPosList != null && OutOrderPosList.Any())
                {
                    OutOrderPos dbOutOrderPos = DatabaseApp.OutOrderPos.FirstOrDefault(c => c.OutOrderPosID == (OutOrderPosID ?? Guid.Empty));
                    if (dbOutOrderPos != null)
                    {
                        var OutOrderPos = OutOrderPosList.Where(c => c.OutOrderPosID == dbOutOrderPos.OutOrderPosID ||
                        c.OutOrderPosID == dbOutOrderPos.ParentOutOrderPosID).FirstOrDefault();
                        if (OutOrderPos != null)
                            SelectedOutOrderPos = OutOrderPos;
                    }
                }
            }
            await ShowDialogAsync(this, "DisplayOrderDialog");
            await this.ParentACComponent.StopComponent(this);
        }


        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay)]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            DialogResult.ReturnValue = CurrentOutOrder;
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel)]
        public void DialogCancel()
        {
            if (CurrentOutOrder != null && CurrentOutOrder.EntityState == EntityState.Added)
                Delete();
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            CloseTopDialog();
        }
        #endregion

        #region Assign / Unassign Contract lines

        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Filter'}de{'Filter'}", 602, false)]
        public async Task<bool> FilterDialogContractPos()
        {
            bool result = await AccessOpenContractPos.ShowACQueryDialog();
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

                Msg result = OutDeliveryNoteManager.AssignContractOutOrderPos(CurrentOpenContractPos, CurrentOutOrder, PartialQuantity, DatabaseApp, ACFacilityManager, resultNewEntities);
                if (result != null)
                {
                    Messages.MsgAsync(result);
                    return;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOOutOrder", "AssignContractPos", msg);

                return;
            }

            if (_UnSavedUnAssignedContractPos.Contains(CurrentOpenContractPos))
                _UnSavedUnAssignedContractPos.Remove(CurrentOpenContractPos);
            OnPropertyChanged("OutOrderPosList");

            RefreshOpenContractPosList();
            PartialQuantity = null;
            foreach (object item in resultNewEntities)
            {
                if (item is OutOrderPos)
                {
                    SelectedOutOrderPos = item as OutOrderPos;
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
                || CurrentOutOrder == null
                || CurrentOutOrder.MDOutOrderType == null
                || CurrentOutOrder.MDOutOrderType.OrderType == GlobalApp.OrderTypes.Contract
                || CurrentOutOrder.MDOutOrderType.OrderType == GlobalApp.OrderTypes.InternalOrder)
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

            OutOrderPos parentOutOrderPos = null;
            OutOrderPos currentOutOrderPos = CurrentOutOrderPos;
            parentOutOrderPos = CurrentOutOrderPos.OutOrderPos1_ParentOutOrderPos;

            Msg result = null;
            try
            {
                result = OutDeliveryNoteManager.UnassignContractOutOrderPos(currentOutOrderPos, DatabaseApp);
                if (result != null)
                {
                    Messages.MsgAsync(result);
                    return;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOOutOrder", "UnAssignContractPos", msg);
                return;
            }

            if (result == null && parentOutOrderPos != null)
            {
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
            if (CurrentOutOrderPos == null
                || CurrentOutOrderPos.OutOrderPos1_ParentOutOrderPos == null
                || CurrentOutOrderPos.OutOrderPos1_ParentOutOrderPos.OutOrder.MDOutOrderType.OrderType != GlobalApp.OrderTypes.Contract)
                return false;
            return true;
        }

        #endregion

        #region Properties => Report

        private ACComponent _BSOOutOrderReportHandler;
        public ACComponent BSOOutOrderReportHandler
        {
            get
            {
                return _BSOOutOrderReportHandler;
            }
        }

        #endregion

        #region Methods => Report

        public void BuildOutOrderPosData(string langCode)
        {
            if (CurrentOutOrder == null)
                return;

            List<OutOrderPos> posData = new List<OutOrderPos>();

            foreach (var outOrderPos in CurrentOutOrder.OutOrderPos_OutOrder.Where(c => c.GroupOutOrderPosID == null && c.PriceNet >= 0).OrderBy(p => p.Position))
            {
                posData.Add(outOrderPos);
                BuildOutOrderPosDataRecursive(posData, outOrderPos.Items);
                if (outOrderPos.GroupSum)
                {
                    OutOrderPos sumPos = new OutOrderPos();
                    sumPos.TotalPricePrinted = outOrderPos.Items.Sum(c => c.TotalPrice).ToString("N");
                    sumPos.MaterialNo = Root.Environment.TranslateMessageLC(this, "Info50076", langCode) + outOrderPos.Material.MaterialNo; // Info50063.
                    sumPos.Sequence = outOrderPos.Sequence;
                    sumPos.GroupSum = outOrderPos.GroupSum;
                    posData.Add(sumPos);
                }
            }

            OutOrderPosDataList = posData;
            OutOrderPosDiscountList = CurrentOutOrder.OutOrderPos_OutOrder.Where(c => c.PriceNet < 0).OrderBy(s => s.Sequence).ToList();
            if (OutOrderPosDiscountList != null && OutOrderPosDiscountList.Any())
            {
                //OutOfferPosDiscountList.Add(new OutOfferPos() { Comment = "Rabatt in Summe:", PriceNet = (decimal)CurrentOutOffer.PosPriceNetDiscount });
                OutOrderPosDiscountList.Add(new OutOrderPos() { Comment = Root.Environment.TranslateMessageLC(this, "Info50077", langCode), PriceNet = (decimal)CurrentOutOrder.PosPriceNetTotal }); //Info50064.
            }

            OutDeliveryNoteManager.CalculateTaxOverview(this, CurrentOutOrder, CurrentOutOrder.OutOrderPos_OutOrder.Select(c => (IOutOrderPos)c).ToList());
        }

        private void BuildOutOrderPosDataRecursive(List<OutOrderPos> posDataList, IEnumerable<OutOrderPos> outOfferPosList)
        {
            foreach (var outOfferPos in outOfferPosList.Where(c => c.PriceNet >= 0).OrderBy(p => p.Position))
            {
                posDataList.Add(outOfferPos);
                BuildOutOrderPosDataRecursive(posDataList, outOfferPos.Items);
            }
        }

        public override void OnPrintingPhase(object reportEngine, ACPrintingPhase printingPhase)
        {
            ACComponent childBSO = ACUrlCommand("BSOOutOrderReportHandler_Child") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("BSOOutOrderReportHandler_Child", null, new object[] { }) as ACComponent;
            _BSOOutOrderReportHandler = childBSO;

            if (BSOOutOrderReportHandler != null)
                BSOOutOrderReportHandler.OnPrintingPhase(reportEngine, printingPhase);
        }

        #endregion

        #region Invoice

        [ACMethodCommand(DeliveryNote.ClassName, "en{'Create Invoice'}de{'Rechnung erstellen'}", (short)MISort.Cancel)]
        public async void CreateInvoice()
        {
            if (!PreExecute("CreateInvoice"))
                return;
            if (await Messages.QuestionAsync(this, "Question50060", Global.MsgResult.Yes, false, CurrentOutOrder.OutOrderNo) == Global.MsgResult.Yes)
            {
                DateTime invoiceDate = DateTime.Now;
                object[] valueList = new object[] { DateTime.Now };
                string[] captionList = new string[] { "Date" };
                object[] resultList = Messages.InputBoxValues("Invoice-Date", valueList, captionList);

                if (resultList != null)
                    invoiceDate = (DateTime)resultList[0];

                Msg msg = OutDeliveryNoteManager.NewInvoiceFromOutOrder(DatabaseApp, CurrentOutOrder, invoiceDate);
                if (msg != null)
                   await Messages.MsgAsync(msg);
            }
            PostExecute("CreateInvoice");
        }

        public bool IsEnabledCreateInvoice()
        {
            return CurrentOutOrder != null && OutDeliveryNoteManager != null;
        }

        [ACMethodCommand("", "en{'Create production order'}de{'Produktionsauftrag erstellen'}", 800, true)]
        public void CreateProductionOrder()
        {
            foreach (OutOrderPos pos in this.OutOrderPosList)
            {
                Partslist partsList = pos.Material.Partslist_Material.Where(c => c.IsEnabled || (c.IsInEnabledPeriod != null && c.IsInEnabledPeriod.Value)).FirstOrDefault();
                if (partsList == null)
                    continue;

                string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(ProdOrder), ProdOrder.NoColumnName, ProdOrder.FormatNewNo, this);
                ProdOrder prodOrder = ProdOrder.NewACObject(DatabaseApp, null, secondaryKey);

                ProdOrderPartslist prodOrderPartslist;
                Msg msg = ProdOrderManager.PartslistAdd(DatabaseApp, prodOrder, partsList, 1, pos.TargetQuantity, out prodOrderPartslist);
                if (msg != null)
                {
                    Messages.MsgAsync(msg);
                    continue;
                }
            }
            ACSaveChanges();
        }

        public bool IsEnabledCreateProductionOrder()
        {
            return CurrentOutOrder != null && ProdOrderManager != null;
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
                    _= Save();
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
                case nameof(NewOutOrderPos):
                    NewOutOrderPos();
                    return true;
                case nameof(IsEnabledNewOutOrderPos):
                    result = IsEnabledNewOutOrderPos();
                    return true;
                case nameof(DeleteOutOrderPos):
                    DeleteOutOrderPos();
                    return true;
                case nameof(IsEnabledDeleteOutOrderPos):
                    result = IsEnabledDeleteOutOrderPos();
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
                    _= FilterDialogContractPos();
                    return true;
                case nameof(ShowDialogOrder):
                    _= ShowDialogOrder(acParameter[0] as string, acParameter.Count() >= 2 ? (Guid?)acParameter[1] : null);
                    return true;
                case nameof(CreateInvoice):
                    CreateInvoice();
                    return true;
                case nameof(IsEnabledCreateInvoice):
                    result = IsEnabledCreateInvoice();
                    return true;
                case nameof(CreateProductionOrder):
                    CreateProductionOrder();
                    return true;
                case nameof(IsEnabledCreateProductionOrder):
                    result = IsEnabledCreateProductionOrder();
                    return true;
                case nameof(ShowDialogOrderInfo):
                    _= ShowDialogOrderInfo((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}
