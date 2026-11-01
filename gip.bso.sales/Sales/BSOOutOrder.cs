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
    /// Businessobject (App) for managing sales orders (customer orders) in the system.
    /// It replaces the old screens "Goods issue -> Orders" and "Goods issue -> Order positions" with the new screen "Order management".
    /// To search records enter the search string in the SearchWord property (or set the filters FilterNo, FilterComment, SelectedFilterMaterial, SelectedFilterOrderState) and call the Search() method.
    /// The database result is copied to the OutOrderList property.
    /// Then call NavigateFirst() method to set CurrentOutOrder with the first record in the list.
    /// CurrentOutOrder is used to display and edit the currently selected order.
    /// Property changes should always be made to CurrentOutOrder and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new OutOrder with the next number from the number manager and assigns the new entity object to the CurrentOutOrder property.
    /// Assign the customer to the CurrentOutOrder.CustomerCompany property. The billing and delivery addresses are offered in CurrentBillingCompanyAddress/CurrentDeliveryCompanyAddress (lists BillingCompanyAddressList/DeliveryCompanyAddressList of the customer).
    /// Enter order date, target delivery dates and terms (delivery type, term of payment, currency, time range) on CurrentOutOrder.
    /// The OutOrderPosList property contains the order lines of the current order.
    /// Example how to add a position: call NewOutOrderPos() which adds a new OutOrderPos to the OutOrderPosList and assigns it to CurrentOutOrderPos. NewSubOutOrderPos() adds a sub position below the selected position, DeleteOutOrderPos() deletes the current position. OutOrderPosUp()/OutOrderPosDown() change the sequence of the current position.
    /// Assign the material to the CurrentOutOrderPos.Material property, enter the quantity in CurrentOutOrderPos.TargetQuantity (unit via CurrentMDUnit or ChangeTargetQuantity) and the net price in CurrentOutOrderPos.PriceNet (optionally pick a price from PriceListMaterialItems via SelectedPriceListMaterial).
    /// Then call the Save() method to save the changes in the database. If comes warning message you can ignore it with the Yes button.
    /// Contract call-offs: the OpenContractPosList property contains open contract lines (query "OutOpenContractPos"). Filter them with FilterMaterial, FilterDelivDateFrom, FilterDelivDateTo and call RefreshOpenContractPosList().
    /// Call AssignContractPos() to call off the selected contract line (CurrentOpenContractPos) into the current order - optionally with PartialQuantity for a partial quantity. UnAssignContractPos() revises a call-off: it deletes the generated position and returns the quantity to the contract line.
    /// Pickup master data (external material of the customer) on a position is managed with CompanyMaterialPickupList/AvailableCompMaterialList and the methods NewCompanyMaterialPickup(), LoadCompanyMaterialPickup(), DeleteCompanyMaterialPickup().
    /// CreateInvoice() generates an invoice from the current order (asks for the invoice date; if invoices already exist it can create a correction invoice linked to the previous one).
    /// CreateProductionOrder() creates production orders for all positions whose material has an enabled bill of material.
    /// ShowDialogOrder()/ShowDialogOrderInfo() open the order dialog for a given order number (e.g. called from production), confirmed with DialogOK()/DialogCancel().
    /// The Delete() method deletes the current order (foreign key constraints are checked and reported as messages). Then call Save().
    /// The Load() method re-reads the current order if another user changed it in the background. UndoSave() reverts unsaved changes.
    /// Visit https://github.com/search?q=org%3Aiplus-framework%20BSOOutOrder&type=code for the source code.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Sales Order'}de{'Kundenauftrag'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + OutOrder.ClassName,
                 Description = @"Businessobject (App) for managing sales orders (customer orders) in the system.
                                It replaces the old screens ""Goods issue -> Orders"" and ""Goods issue -> Order positions"" with the new screen ""Order management"".
                                To search records enter the search string in the SearchWord property (or set the filters FilterNo, FilterComment, SelectedFilterMaterial, SelectedFilterOrderState) and call the Search() method.
                                The database result is copied to the OutOrderList property.
                                Then call NavigateFirst() method to set CurrentOutOrder with the first record in the list.
                                CurrentOutOrder is used to display and edit the currently selected order.
                                Property changes should always be made to CurrentOutOrder and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
                                The New() method creates a new OutOrder with the next number from the number manager and assigns the new entity object to the CurrentOutOrder property.
                                Assign the customer to the CurrentOutOrder.CustomerCompany property. The billing and delivery addresses are offered in CurrentBillingCompanyAddress/CurrentDeliveryCompanyAddress (lists BillingCompanyAddressList/DeliveryCompanyAddressList of the customer).
                                Enter order date, target delivery dates and terms (delivery type, term of payment, currency, time range) on CurrentOutOrder.
                                The OutOrderPosList property contains the order lines of the current order.
                                Example how to add a position: call NewOutOrderPos() which adds a new OutOrderPos to the OutOrderPosList and assigns it to CurrentOutOrderPos. NewSubOutOrderPos() adds a sub position below the selected position, DeleteOutOrderPos() deletes the current position. OutOrderPosUp()/OutOrderPosDown() change the sequence of the current position.
                                Assign the material to the CurrentOutOrderPos.Material property, enter the quantity in CurrentOutOrderPos.TargetQuantity (unit via CurrentMDUnit or ChangeTargetQuantity) and the net price in CurrentOutOrderPos.PriceNet (optionally pick a price from PriceListMaterialItems via SelectedPriceListMaterial).
                                Then call the Save() method to save the changes in the database. If comes warning message you can ignore it with the Yes button.
                                Contract call-offs: the OpenContractPosList property contains open contract lines (query ""OutOpenContractPos""). Filter them with FilterMaterial, FilterDelivDateFrom, FilterDelivDateTo and call RefreshOpenContractPosList().
                                Call AssignContractPos() to call off the selected contract line (CurrentOpenContractPos) into the current order - optionally with PartialQuantity for a partial quantity. UnAssignContractPos() revises a call-off: it deletes the generated position and returns the quantity to the contract line.
                                Pickup master data (external material of the customer) on a position is managed with CompanyMaterialPickupList/AvailableCompMaterialList and the methods NewCompanyMaterialPickup(), LoadCompanyMaterialPickup(), DeleteCompanyMaterialPickup().
                                CreateInvoice() generates an invoice from the current order (asks for the invoice date; if invoices already exist it can create a correction invoice linked to the previous one).
                                CreateProductionOrder() creates production orders for all positions whose material has an enabled bill of material.
                                ShowDialogOrder()/ShowDialogOrderInfo() open the order dialog for a given order number (e.g. called from production), confirmed with DialogOK()/DialogCancel().
                                The Delete() method deletes the current order (foreign key constraints are checked and reported as messages). Then call Save().
                                The Load() method re-reads the current order if another user changed it in the background. UndoSave() reverts unsaved changes.
                                Visit https://github.com/search?q=org%3Aiplus-framework%20BSOOutOrder&type=code for the source code.")]
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
        [ACPropertyInfo(600, Description = "en{'Child BSO for facility reservations of the current order position'}de{'Child BSO für Lagerreservierungen der aktuellen Auftragsposition'}")]
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
        [ACPropertyAccess(9999, "FilterMaterial", Description = "en{'Access for the material filter of the order search'}de{'Zugriff auf den Materialfilter der Auftragssuche'}")]
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

        [ACPropertyInfo(9999, "FilterMaterial", Description = "en{'List of materials for the order filter'}de{'Liste der Materialien für den Auftragsfilter'}")]
        public IEnumerable<Material> FilterMaterialList
        {
            get
            {
                return AccessFilterMaterial.NavList;
            }
        }

        private Material _SelectedFilterMaterial;
        [ACPropertySelected(9999, "FilterMaterial", "en{'Material'}de{'Material'}", Description = "en{'Material filter of the order search'}de{'Materialfilter der Auftragssuche'}")]
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
        [ACPropertySelected(9999, "FilterOrderState", "en{'Order state'}de{'Auftragsstatus'}", Description = "en{'Order state filter of the order search'}de{'Auftragsstatusfilter der Auftragssuche'}")]
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
        [ACPropertyList(9999, "FilterOrderState", Description = "en{'Available order states for the filter'}de{'Verfügbare Auftragsstatus für den Filter'}")]
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

        [ACPropertyInfo(300, nameof(FilterNo), "en{'Order No.'}de{'Auftrag Nr.'}", Description = "en{'Filters the order search by order number'}de{'Filtert die Auftragssuche nach Auftragsnummer'}")]
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


        [ACPropertyInfo(301, nameof(FilterComment), ConstApp.Comment, Description = "en{'Filters the order search by comment'}de{'Filtert die Auftragssuche nach Kommentar'}")]
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
        [ACPropertyAccessPrimary(690, OutOrder.ClassName, Description = "en{'Primary navigation access to the sales orders'}de{'Primärer Navigationszugriff auf die Kundenaufträge'}")]
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

        [ACPropertyCurrent(600, OutOrder.ClassName, Description = "en{'Currently selected and edited sales order'}de{'Aktuell ausgewählter und bearbeiteter Kundenauftrag'}")]
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

        [ACPropertyList(601, OutOrder.ClassName, Description = "en{'Result list of the order search'}de{'Ergebnisliste der Auftragssuche'}")]
        public IEnumerable<OutOrder> OutOrderList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        [ACPropertySelected(602, OutOrder.ClassName, Description = "en{'Order selected in the search list'}de{'In der Suchliste ausgewählter Auftrag'}")]
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
        [ACPropertyAccess(691, OutOrderPos.ClassName, Description = "en{'Access to the order positions of the current order'}de{'Zugriff auf die Auftragspositionen des aktuellen Auftrags'}")]
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
        [ACPropertyCurrent(603, OutOrderPos.ClassName, Description = "en{'Currently selected and edited order position'}de{'Aktuell ausgewählte und bearbeitete Auftragsposition'}")]
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
        [ACPropertyList(604, OutOrderPos.ClassName, Description = "en{'Order positions of the current order (for release orders only open outward parts)'}de{'Auftragspositionen des aktuellen Auftrags (bei Freigabeaufträgen nur offene outward parts)'}")]
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
        [ACPropertySelected(605, OutOrderPos.ClassName, Description = "en{'Order position selected in the position list'}de{'In der Positionsliste ausgewählte Auftragsposition'}")]
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
        [ACPropertyList(606, MDUnit.ClassName, Description = "en{'Units of the material of the current order position'}de{'Einheiten des Materials der aktuellen Auftragsposition'}")]
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
        [ACPropertyCurrent(607, MDUnit.ClassName, "en{'New Unit'}de{'Neue Einheit'}", Description = "en{'Unit for the quantity of the current offer position (converts the target quantity)'}de{'Einheit für die Menge der aktuellen Angebotsposition (konvertiert die Sollmenge)'}")]
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
        [ACPropertyInfo(608, "", "en{'New Target Quantity'}de{'Neue Sollmenge'}", Description = "en{'New target quantity that is written to the current order position'}de{'Neue Sollmenge, die in die aktuelle Auftragsposition geschrieben wird'}")]
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
        [ACPropertyCurrent(609, "CompanyMaterialPickup", Description = "en{'Currently edited pickup master data of the order position'}de{'Aktuell bearbeiteter Verladestamm der Auftragsposition'}")]
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
        [ACPropertySelected(610, "CompanyMaterialPickup", Description = "en{'Pickup master data selected in the list'}de{'In der Liste ausgewählter Verladestamm'}")]
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

        [ACPropertyList(611, "CompanyMaterialPickup", Description = "en{'Pickup master data assigned to the current order position'}de{'Verladestämme der aktuellen Auftragsposition'}")]
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
        [ACPropertySelected(612, "AvailableCompMatPickups", "en{'Available Ext. Material'}de{'Verfügbares externes Material'}", Description = "en{'Available external pickup material of the customer selected for a new pickup record'}de{'Verfügbares externes Verladematerial des Kunden, das für einen neuen Verladestamm ausgewählt wurde'}")]
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

        [ACPropertyList(613, "AvailableCompMatPickups", Description = "en{'Pickup company materials of the customer for the material of the current position'}de{'Verladestämme des Kunden zum Material der aktuellen Position'}")]
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
        [ACPropertyList(614, Company.ClassName, Description = "en{'All companies flagged as customers'}de{'Alle als Kunde gekennzeichneten Unternehmen'}")]
        public IEnumerable<Company> CompanyList
        {
            get
            {
                return DatabaseApp.Company
                    .Where(c => c.IsCustomer)
                    .OrderBy(c => c.CompanyName);
            }
        }

        [ACPropertyList(615, "BillingCompanyAddress", Description = "en{'Billing addresses of the customer of the current order'}de{'Rechnungsadressen des Kunden des aktuellen Auftrags'}")]
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

        [ACPropertyList(616, "DeliveryCompanyAddress", Description = "en{'Delivery addresses of the customer of the current order'}de{'Lieferadressen des Kunden des aktuellen Auftrags'}")]
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

        [ACPropertyCurrent(617, "BillingCompanyAddress", ConstApp.BillingCompanyAddress, Description = "en{'Billing address of the current offer'}de{'Rechnungsadresse des aktuellen Angebots'}")]
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

        [ACPropertyCurrent(618, "DeliveryCompanyAddress", ConstApp.DeliveryCompanyAddress, Description = "en{'Delivery address of the current offer'}de{'Lieferadresse des aktuellen Angebots'}")]
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
        [ACPropertyAccess(692, "OpenContractPos", Description = "en{'Navigation access to open contract lines for call-offs'}de{'Navigationszugriff auf offene Kontraktpositionen für Abrufe'}")]
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
        [ACPropertyInfo(713, "Filter", "en{'Material'}de{'Material'}", Description = "en{'Filters the open contract lines by material number or name'}de{'Filtert die offenen Kontraktpositionen nach Materialnummer oder -name'}")]
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

        [ACPropertyInfo(618, "", "en{'Contract date from'}de{'Kontraktdatum von'}", Description = "en{'Filters the open contract lines by target delivery date (from)'}de{'Filtert die offenen Kontraktpositionen nach Soll-Lieferdatum (von)'}")]
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
        [ACPropertyInfo(619, "", "en{'Contract date to'}de{'Kontraktdatum bis'}", Description = "en{'Filters the open contract lines by target delivery date (to)'}de{'Filtert die offenen Kontraktpositionen nach Soll-Lieferdatum (bis)'}")]
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
        [ACPropertyInfo(637, "", "en{'Partial Quantity'}de{'Teilmenge'}", Description = "en{'Partial quantity for calling off a contract line'}de{'Teilmenge für den Abruf einer Kontraktposition'}")]
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
        [ACPropertyCurrent(613, "OpenContractPos", Description = "en{'Contract line currently selected for call-off'}de{'Kontraktposition, die aktuell für den Abruf ausgewählt ist'}")]
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
        [ACPropertyList(614, "OpenContractPos", Description = "en{'Open contract lines that can be called off'}de{'Offene Kontraktpositionen, die abgerufen werden können'}")]
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
        [ACPropertySelected(615, "OpenContractPos", Description = "en{'Contract line selected in the open contract lines list'}de{'In der Liste der offenen Kontraktpositionen ausgewählte Position'}")]
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


        [ACPropertyList(616, "TransportMode", Description = "en{'Transport modes for the order position'}de{'Transportarten für die Auftragsposition'}")]
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
        [ACPropertySelected(671, "PriceListMaterial", "en{'Price lists'}de{'Preisliste'}", Description = "en{'Selected price list entry whose price is copied to the position net price'}de{'Ausgewählter Preislisten Eintrag, dessen Preis in den Positionsnettopreis übernommen wird'}")]
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
        [ACPropertyList(671, "PriceListMaterial", Description = "en{'Valid price list entries for the material of the current position'}de{'Gültige Preislisteneinträge zum Material der aktuellen Position'}")]
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
        [ACPropertyInfo(999, "IssuerCompanyAddress", "en{'Issuer Company Address'}de{'Emittentenfirmaadresse'}", Description = "en{'Issuer company address (set from user settings on new offers)'}de{'Emittentenfirmaadresse (bei neuen Angeboten aus den Benutzereinstellungen)'}")]
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
        [ACPropertyInfo(999, "IssuerCompanyAddressMessage", "en{'Issuer status'}de{'Emittentenstatus'}", Description = "en{'Status message of the issuer determination'}de{'Statusmeldung der Emittentenermittlung'}")]
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
        [ACPropertySelected(9999, "IssuerCompanyPerson", "en{'TODO: IssuerCompanyPerson'}de{'TODO: IssuerCompanyPerson'}", Description = "en{'Issuer company person'}de{'Emittentenfirmenperson'}")]
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
        [ACPropertyList(9999, "IssuerCompanyPerson", Description = "en{'List of issuer company persons'}de{'Liste der Emittentenfirmenpersonen'}")]
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
        [ACPropertyInfo(9999, Description = "en{'Report data for printing the order'}de{'Berichtsdaten zum Drucken des Auftrags'}")]
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

        [ACPropertyInfo(650, Description = "en{'Flattened order positions for the print output'}de{'Aufbereitete Auftragspositionen für den Druck'}")]
        public List<OutOrderPos> OutOrderPosDataList
        {
            get;
            set;
        }

        [ACPropertyInfo(651, Description = "en{'Discount positions of the order for the print output'}de{'Rabattpositionen des Auftrags für den Druck'}")]
        public List<OutOrderPos> OutOrderPosDiscountList
        {
            get;
            set;
        }

        [ACPropertyInfo(652, Description = "en{'Tax overview of the current order'}de{'Steuerübersicht des aktuellen Auftrags'}")]
        public List<MDCountrySalesTax> TaxOverviewList
        {
            get;
            set;
        }

        #endregion

        #region Tenant

        #region Tenant -> TenantCompany

        ACAccessNav<Company> _AccessTenantCompany;
        [ACPropertyAccess(100, "TenantCompany", Description = "en{'Access to tenant companies (issuers) for invoice data'}de{'Zugriff auf Mandantenunternehmen (Emittenten) für Rechnungsdaten'}")]
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
        [ACPropertySelected(101, "TenantCompany", "en{'Tenant'}de{'Mandant'}", Description = "en{'Tenant company (issuer) selected for invoice address/person'}de{'Mandantenunternehmen (Emittent) für Rechnungsadresse/Person'}")]
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
        [ACPropertyList(102, "TenantCompany", Description = "en{'List of tenant companies (issuers)'}de{'Liste der Mandantenunternehmen (Emittenten)'}")]
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
        [ACPropertySelected(9999, "InvoiceCompanyAddress", "en{'Address for Invoice'}de{'Adresse zur Rechnungstellung'}", Description = "en{'Company address used for invoicing'}de{'Firmenadresse, die für die Rechnungsstellung verwendet wird'}")]
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
        [ACPropertyList(9999, "InvoiceCompanyAddress", Description = "en{'Addresses of the selected tenant company'}de{'Adressen des ausgewählten Mandantenunternehmens'}")]
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
        [ACPropertySelected(9999, "InvoiceCompanyPerson", "en{'Person for invoice'}de{'Person zur Rechnungstellung'}", Description = "en{'Company person used for invoicing'}de{'Firmenperson, die für die Rechnungsstellung verwendet wird'}")]
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
        [ACPropertyList(9999, "InvoiceCompanyPerson", Description = "en{'Persons of the selected tenant company'}de{'Personen des ausgewählten Mandantenunternehmens'}")]
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
        [ACMethodInfo("Picking", "en{'Activate'}de{'Aktivieren'}", 608, true, Global.ACKinds.MSMethodPrePost, Description = "en{'Called when a tab item is activated; refreshes the open contract lines when the contract call-off tab is opened'}de{'Wird beim Aktivieren eines Tabitems aufgerufen; aktualisiert die offenen Kontraktpositionen beim Öffnen des Kontraktabruf-Tabs'}")]
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
        [ACMethodCommand(OutOrder.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "en{'Saves all changes of the current order and its positions to the database'}de{'Speichert alle Änderungen des aktuellen Auftrags und seiner Positionen in der Datenbank'}")]
        public async Task Save()
        {
            await OnSave();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        [ACMethodCommand(OutOrder.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "en{'Reverts all unsaved changes of the current order'}de{'Verwirft alle ungespeicherten Änderungen des aktuellen Auftrags'}")]
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


        [ACMethodInteraction(OutOrder.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedOutOrder", Global.ACKinds.MSMethodPrePost, Description = "en{'Re-reads the selected order from the database (e.g. after changes by another user)'}de{'Liest den ausgewählten Auftrag erneut aus der Datenbank (z.B. nach Änderungen durch einen anderen Benutzer)'}")]
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

        [ACMethodInteraction(OutOrder.ClassName, Const.New, (short)MISort.New, true, "SelectedOutOrder", Global.ACKinds.MSMethodPrePost, Description = "en{'Creates a new sales order with the next order number and assigns it to CurrentOutOrder'}de{'Erstellt einen neuen Kundenauftrag mit der nächsten Auftragsnummer und weist ihn CurrentOutOrder zu'}")]
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

        [ACMethodInteraction(OutOrder.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentOutOrder", Global.ACKinds.MSMethodPrePost, Description = "en{'Deletes the current sales order (foreign key constraints are reported as messages)'}de{'Löscht den aktuellen Kundenauftrag (Fremdschlüsselverletzungen werden als Meldung ausgegeben)'}")]
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

        [ACMethodCommand(OutOrder.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "en{'Executes the order search with the current filters and fills OutOrderList'}de{'Führt die Auftragssuche mit den aktuellen Filtern aus und füllt OutOrderList'}")]
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


        [ACMethodInteraction("OutOrderPos", "en{'New Position'}de{'Neue Position'}", (short)MISort.New, true, "SelectedOutOrderPos", Global.ACKinds.MSMethodPrePost, Description = "en{'Adds a new order position (in the group of the current position) and assigns it to CurrentOutOrderPos'}de{'Fügt eine neue Auftragsposition (in der Gruppe der aktuellen Position) hinzu und weist sie CurrentOutOrderPos zu'}")]
        public void NewOutOrderPos()
        {
            if (!PreExecute("NewOutOrderPos")) return;
            // Einfügen einer neuen Eigenschaft und der aktuellen Eigenschaft zuweisen
            OutOrderPos groupPos = CurrentOutOrderPos?.OutOrderPos1_GroupOutOrderPos;
            OutOrderPos newOutOrderPos = OutOrderPos.NewACObject(DatabaseApp, CurrentOutOrder, groupPos);
            newOutOrderPos.OutOrder = CurrentOutOrder;
            newOutOrderPos.OutOrderPos1_GroupOutOrderPos = groupPos;
            // Add to collection BEFORE assigning CurrentOutOrderPos, otherwise the DataGrid
            // coerces SelectedItem back to null (item not yet in ItemsSource) and writes
            // null back into CurrentOutOrderPos via OnSelectionChanged.
            CurrentOutOrder.OutOrderPos_OutOrder.Add(newOutOrderPos);
            OnPropertyChanged("OutOrderPosList");
            CurrentOutOrderPos = newOutOrderPos;
            PostExecute("NewOutOrderPos");
        }

        public bool IsEnabledNewOutOrderPos()
        {
            return CurrentOutOrder != null;
        }

        [ACMethodInteraction("OutOrderPos", "en{'New sub Position'}de{'Neue sub Position'}", (short)MISort.New, true, "SelectedOutOrderPos", Global.ACKinds.MSMethodPrePost, Description = "en{'Adds a new sub position below the selected order position'}de{'Fügt eine neue Unterposition unterhalb der ausgewählten Auftragsposition hinzu'}")]
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

        [ACMethodInteraction("OutOrderPos", "en{'Delete Position'}de{'Position löschen'}", (short)MISort.Delete, true, "CurrentOutOrderPos", Global.ACKinds.MSMethodPrePost, Description = "en{'Deletes the current order position and recalculates prices and tax overview'}de{'Löscht die aktuelle Auftragsposition und berechnet Preise und Steuerübersicht neu'}")]
        public void DeleteOutOrderPos()
        {
            if (!PreExecute("DeleteOutOrderPos")) return;
            OutOrderPos posToDelete = CurrentOutOrderPos;
            Msg msg = posToDelete.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            // EF Core does not remove the entity from navigation collections automatically
            // (unlike EF4). Remove it manually, otherwise the DataGrid still shows the
            // deleted row after OnPropertyChanged("OutOrderPosList").
            CurrentOutOrder?.OutOrderPos_OutOrder.Remove(posToDelete);
            // Clear Current/Selected BEFORE raising the list notification, so the DataGrid
            // doesn't try to restore a selection on the deleted entity.
            CurrentOutOrderPos = null;
            SelectedOutOrderPos = null;

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

        [ACMethodInteraction("OutOrderPos", "en{'Position up'}de{'Position oben'}", 10, true, "CurrentOutOrderPos", Global.ACKinds.MSMethodPrePost, Description = "en{'Moves the current order position one sequence up'}de{'Verschiebt die aktuelle Auftragsposition eine Sequenz nach oben'}")]
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

        [ACMethodInteraction("OutOrderPos", "en{'Position down'}de{'Position unten'}", 11, true, "CurrentOutOrderPos", Global.ACKinds.MSMethodPrePost, Description = "en{'Moves the current order position one sequence down'}de{'Verschiebt die aktuelle Auftragsposition eine Sequenz nach unten'}")]
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
        [ACMethodInteraction("CompanyMaterialPickup", "en{'Load Pick Up'}de{'Verladestamm laden'}", (short)MISort.Load, false, "SelectedCompanyMaterialPickup", Global.ACKinds.MSMethodPrePost, Description = "en{'Loads the selected pickup master data into CurrentCompanyMaterialPickup'}de{'Lädt den ausgewählten Verladestamm in CurrentCompanyMaterialPickup'}")]
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

        [ACMethodInteraction("CompanyMaterialPickup", "en{'New Pick Up'}de{'Neuer Verladestamm'}", (short)MISort.New, true, "SelectedCompanyMaterialPickup", Global.ACKinds.MSMethodPrePost, Description = "en{'Creates a new pickup master data record for the current position from the selected available external material'}de{'Erstellt einen neuen Verladestamm für die aktuelle Position aus dem ausgewählten verfügbaren externen Material'}")]
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

        [ACMethodInteraction("CompanyMaterialPickup", "en{'Delete Pick Up'}de{'Verladestamm löschen'}", (short)MISort.Delete, true, "CurrentCompanyMaterialPickup", Global.ACKinds.MSMethodPrePost, Description = "en{'Deletes the current pickup master data record'}de{'Löscht den aktuellen Verladestamm'}")]
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

        [ACMethodInfo("Dialog", "en{'Dialog sales Order'}de{'Dialog Auftrag'}", (short)MISort.QueryPrintDlg + 1, Description = "en{'Opens the order dialog for the order/position given in the PAOrderInfo (e.g. called from production)'}de{'Öffnet den Auftragsdialog für den in PAOrderInfo angegebenen Auftrag/Position (z.B. aus der Produktion aufgerufen)'}")]
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


        [ACMethodInfo("Dialog", "en{'Dialog Purchase Order'}de{'Dialog Bestellung'}", (short)MISort.QueryPrintDlg, Description = "en{'Searches the order by number, selects the given position and opens the order dialog'}de{'Sucht den Auftrag nach Nummer, wählt die angegebene Position und öffnet den Auftragsdialog'}")]
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


        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay, Description = "en{'Closes the order dialog with OK and returns the current order'}de{'Schließt den Auftragsdialog mit OK und gibt den aktuellen Auftrag zurück'}")]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            DialogResult.ReturnValue = CurrentOutOrder;
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel, Description = "en{'Closes the order dialog with Cancel (a newly created order is deleted)'}de{'Schließt den Auftragsdialog mit Abbrechen (ein neu angelegter Auftrag wird gelöscht)'}")]
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

        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Filter'}de{'Filter'}", 602, false, Description = "en{'Opens the filter dialog for the open contract lines and refreshes the list'}de{'Öffnet den Filterdialog für die offenen Kontraktpositionen und aktualisiert die Liste'}")]
        public async Task<bool> FilterDialogContractPos()
        {
            bool result = await AccessOpenContractPos.ShowACQueryDialog();
            if (result)
            {
                RefreshOpenContractPosList();
            }
            return result;
        }


        [ACMethodInfo("OpenContractPos", "en{'Find contract lines'}de{'Kontraktpos. suchen'}", 602, false, Description = "en{'Re-searches the open contract lines with the current filters'}de{'Sucht die offenen Kontraktpositionen mit den aktuellen Filtern neu'}")]
        public void RefreshOpenContractPosList()
        {
            if (_ActivateInOpen && AccessOpenContractPos != null)
                AccessOpenContractPos.NavSearch(DatabaseApp);
            OnPropertyChanged("OpenContractPosList");
        }

        /// <summary>
        /// Assigns the in order pos.
        /// </summary>
        [ACMethodCommand("OpenContractPos", "en{'Call off contract line'}de{'Kontraktposition abrufen'}", 603, true, Description = "en{'Calls off the selected contract line into the current order (optionally with PartialQuantity)'}de{'Ruft die ausgewählte Kontraktposition in den aktuellen Auftrag ab (optional mit Teilmenge)'}")]
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
        [ACMethodCommand("OpenContractPos", "en{'Revise contract'}de{'Kontraktabruf revidieren'}", 604, true, Description = "en{'Revises the contract call-off of the current position: deletes it and returns the quantity to the contract line'}de{'Revidiert den Kontraktabruf der aktuellen Position: löscht sie und gibt die Menge auf die Kontraktposition zurück'}")]
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
            string childName = "BSOOutOrderReportHandler_Child";
            ACComponent childBSO = ACUrlCommand(childName) as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent(childName, null, new object[] { }) as ACComponent;
            _BSOOutOrderReportHandler = childBSO;

            if (BSOOutOrderReportHandler != null)
                BSOOutOrderReportHandler.OnPrintingPhase(reportEngine, printingPhase);
        }

        #endregion

        #region Invoice

        [ACMethodCommand(DeliveryNote.ClassName, "en{'Create Invoice'}de{'Rechnung erstellen'}", (short)MISort.Cancel, Description = "en{'Generates an invoice from the current order (asks for the invoice date; optionally creates a correction invoice)'}de{'Erstellt aus dem aktuellen Auftrag eine Rechnung (fragt nach dem Rechnungsdatum; erstellt optional eine Korrekturrechnung)'}")]
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

                Invoice referenceInvoice = CurrentOutOrder.Invoice_OutOrder.OrderByDescending(c => c.InvoiceDate).FirstOrDefault();
                if (referenceInvoice != null)
                {
                    // You have already generated one or more invoices for this order. Would you like to create a correction invoice so that the new invoice is linked to the previous one?
                    if (await Messages.QuestionAsync(this, "Question50124", Global.MsgResult.No, false, CurrentOutOrder.OutOrderNo) == Global.MsgResult.No)
                        referenceInvoice = null;
                }
                Msg msg = OutDeliveryNoteManager.NewInvoiceFromOutOrder(DatabaseApp, CurrentOutOrder, referenceInvoice, invoiceDate);
                if (msg != null)
                   await Messages.MsgAsync(msg);
            }
            PostExecute("CreateInvoice");
        }

        public bool IsEnabledCreateInvoice()
        {
            return CurrentOutOrder != null && OutDeliveryNoteManager != null;
        }

        [ACMethodCommand("", "en{'Create production order'}de{'Produktionsauftrag erstellen'}", 800, true, Description = "en{'Creates production orders for all positions whose material has an enabled bill of material'}de{'Erstellt Produktionsaufträge für alle Positionen, deren Material eine freigegebene Stückliste hat'}")]
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
                    result = Save();
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
                    result = FilterDialogContractPos();
                    return true;
                case nameof(ShowDialogOrder):
                    result = ShowDialogOrder(acParameter[0] as string, acParameter.Count() >= 2 ? (Guid?)acParameter[1] : null);
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
                    result = ShowDialogOrderInfo((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                #region Always Enabled
                case nameof(Search):
                    return new string[] { nameof(InitState) };
                case nameof(IsEnabledNew):
                    return new string[] { nameof(InitState) };
                #endregion

                #region Load
                case nameof(Load):
                case nameof(IsEnabledLoad):
                    return new string[] { nameof(ACState), nameof(SelectedOutOrder) };
                #endregion

                #region Delete
                case nameof(Delete):
                case nameof(IsEnabledDelete):
                    return new string[] { nameof(CurrentOutOrder) };
                #endregion

                #region OutOrderPos operations
                case nameof(NewOutOrderPos):
                case nameof(IsEnabledNewOutOrderPos):
                    return new string[] { nameof(InitState) };
                case nameof(DeleteOutOrderPos):
                case nameof(IsEnabledDeleteOutOrderPos):
                    return new string[] { nameof(CurrentOutOrderPos) };
                #endregion

                #region CompanyMaterialPickup operations
                case nameof(LoadCompanyMaterialPickup):
                case nameof(IsEnabledLoadCompanyMaterialPickup):
                    return new string[] { nameof(ACState), nameof(SelectedOutOrder), nameof(SelectedCompanyMaterialPickup) };
                case nameof(NewCompanyMaterialPickup):
                case nameof(IsEnabledNewCompanyMaterialPickup):
                    return new string[] { nameof(ACState), nameof(SelectedOutOrder) };
                case nameof(DeleteCompanyMaterialPickup):
                case nameof(IsEnabledDeleteCompanyMaterialPickup):
                    return new string[] { nameof(CurrentCompanyMaterialPickup) };
                #endregion

                #region ContractPos operations
                case nameof(AssignContractPos):
                case nameof(IsEnabledAssignContractPos):
                case nameof(UnAssignContractPos):
                case nameof(IsEnabledUnAssignContractPos):
                case nameof(RefreshOpenContractPosList):
                case nameof(FilterDialogContractPos):
                    return new string[] { nameof(ACState), nameof(SelectedOutOrder), nameof(SelectedOpenContractPos) };
                #endregion

                #region Dialog / Order
                case nameof(ShowDialogOrder):
                    return new string[] { nameof(ACState), nameof(SelectedOutOrder) };
                case nameof(ShowDialogOrderInfo):
                    return new string[] { nameof(ACState), nameof(SelectedOutOrder) };
                #endregion

                #region Create operations
                case nameof(CreateInvoice):
                case nameof(IsEnabledCreateInvoice):
                    return new string[] { nameof(ACState), nameof(SelectedOutOrder), nameof(SelectedOutOrderPos) };
                case nameof(CreateProductionOrder):
                case nameof(IsEnabledCreateProductionOrder):
                    return new string[] { nameof(ACState), nameof(SelectedOutOrder), nameof(SelectedOutOrderPos) };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

    }
}
