// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.purchasing
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOPicking.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.bso.masterdata;
using gip.bso.purchasing;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.manager;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using gip.mes.processapplication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using static gip.core.datamodel.Global;
using static gip.mes.datamodel.GlobalApp;
using gipCoreData = gip.core.datamodel;

namespace gip.bso.logistics
{
    /// <summary>
    /// Business Service Object (BSO) for managing picking operations in warehouse logistics.
    /// Handles picking order creation, modification, and execution including facility reservations,
    /// booking processes, and workflow management for material movements between facilities.
    /// Supports various picking types (receipt, issue, relocation) and provides comprehensive
    /// filtering, assignment, and tracking capabilities for order positions and facility bookings.
    /// To search for records, enter the search string in the SearchWord property or use the filter in the Explorer.
    /// The database result is copied to the PickingList property. 
    /// Then call the NavigateFirst() method to set CurrentPicking with the first record in the list. 
    /// CurrentPicking is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentPicking and when all field values ​​have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record. 
    /// The New() method creates a new record and assigns the new entity object to the CurrentPicking property.
    /// Fill in all required fields like DateFrom, DateTo, DeliveryCompanyAddress and Comment before saving.
    /// The method AddPickingPos() creates a new PickingPos record and adds it to the CurrentPicking.
    /// In the PickingPos is defined properties like material, quantites and status of the position.
    /// These properties can be related to another entites InOrderPos or OutOrderPos, if they related then shows the values from these related entities.
    /// Also PickingPos defines from which facility material moves to which facility, properties FromFacility and ToFacility.
    /// To remove or unassign PickingPos use the method UnassignPickingPos().
    /// Use the Delete() method to delete the material provided there are no foreign key relationships from other tables. 
    /// Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load method updates the CurrentPicking object with fresh database data if another user has made changes in the background.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Picking Order'}de{'Kommissionierauftrag'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Picking.ClassName,
                 Description = @"Business Service Object (BSO) for managing picking operations in warehouse logistics.
                                 Handles picking order creation, modification, and execution including facility reservations,
                                 booking processes, and workflow management for material movements between facilities.
                                 Supports various picking types (receipt, issue, relocation) and provides comprehensive
                                 filtering, assignment, and tracking capabilities for order positions and facility bookings.
                                 To search for records, enter the search string in the SearchWord property or use the filter in the Explorer.
                                 The database result is copied to the PickingList property. 
                                 Then call the NavigateFirst() method to set CurrentPicking with the first record in the list. 
                                 CurrentPicking is used to display and edit the currently selected record.
                                 Property changes should always be made to CurrentPicking and when all field values ​​have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record. 
                                 The New() method creates a new record and assigns the new entity object to the CurrentPicking property.
                                 Fill in all required fields like DateFrom, DateTo, DeliveryCompanyAddress and Comment before saving.
                                 The method AddPickingPos() creates a new PickingPos record and adds it to the CurrentPicking.
                                 In the PickingPos is defined properties like material, quantites and status of the position.
                                 These properties can be related to another entites InOrderPos or OutOrderPos, if they related then shows the values from these related entities.
                                 Also PickingPos defines from which facility material moves to which facility, properties FromFacility and ToFacility.
                                To remove or unassign PickingPos use the method UnassignPickingPos().
                                Use the Delete() method to delete the material provided there are no foreign key relationships from other tables. 
                                Always call the Save() method after calling Delete() to execute the delete operation in the database.
                                The Load method updates the CurrentPicking object with fresh database data if another user has made changes in the background.")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "InOrderPosOpen", "en{'Open Purchase Order Pos.'}de{'Offene Bestellposition'}", typeof(InOrderPos), InOrderPos.ClassName, "MDDelivPosState\\MDDelivPosStateIndex", "TargetDeliveryDate,Material\\MaterialNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "OutOrderPosOpen", "en{'Open Sales Order Pos.'}de{'Offene Auftragsposition'}", typeof(OutOrderPos), OutOrderPos.ClassName, "MDDelivPosState\\MDDelivPosStateIndex", "TargetDeliveryDate,Material\\MaterialNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "ProdOrderPartslistPosOpen", "en{'Open Prod. Order Pos.'}de{'Offene Prod.-auftragsposition'}", typeof(ProdOrderPartslistPos), ProdOrderPartslistPos.ClassName, "ProdOrderPartslistPos1_ParentProdOrderPartslistPos", "Material\\MaterialNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "BookingFacility", ConstApp.Facility, typeof(Facility), Facility.ClassName, MDFacilityType.ClassName + "\\MDFacilityTypeIndex", "FacilityNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "PositionFacilityFrom", ConstApp.Facility, typeof(Facility), Facility.ClassName, MDFacilityType.ClassName + "\\MDFacilityTypeIndex", "FacilityNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "BookingFacilityLot", ConstApp.Lot, typeof(FacilityLot), FacilityLot.ClassName, "LotNo", "LotNo")]
    [ACClassConstructorInfo(
       new object[]
       {
            new object[] { "PickingType", Global.ParamOption.Optional, typeof(string) },
            new object[] { "FromFacility", Global.ParamOption.Optional, typeof(string) },
            new object[] { "ToFacility", Global.ParamOption.Optional, typeof(string) },
            new object[] { "CompanyNo", Global.ParamOption.Optional, typeof(string) },
            new object[] { "PickingStateIndex", Global.ParamOption.Optional, typeof(short) }
       }
   )]

    public partial class BSOPicking : ACBSOvbNav, IACBSOConfigStoreSelection, IACBSOACProgramProvider
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOPicking"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOPicking(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
            _ForwardToRemoteStores = new ACPropertyConfigValue<bool>(this, nameof(ForwardToRemoteStores), false);
            _NavigateOnGenRelated = new ACPropertyConfigValue<bool>(this, nameof(NavigateOnGenRelated), false);
            _DefaultReservationState = new ACPropertyConfigValue<ReservationState>(this, nameof(DefaultReservationState), ReservationState.New);
            _RMISubscr = new ACPointAsyncRMISubscr(this, "RMISubscr", 1);
        }

        /// <summary>
        /// Initializes the BSOPicking component by setting up required service managers, 
        /// configuration properties, and UI components. Performs initial search operations
        /// and configures facility reservation functionality if startup search is enabled.
        /// </summary>
        /// <param name="startChildMode">Specifies how child components should be started</param>
        /// <returns>True if initialization succeeds, false if base initialization fails</returns>
        /// <exception cref="Exception">Thrown when required managers (InDeliveryNoteManager, OutDeliveryNoteManager, PickingManager, or FacilityManager) are not configured</exception>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _InDeliveryNoteManager = ACInDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_InDeliveryNoteManager == null)
                throw new Exception("InDeliveryNoteManager not configured");

            _OutDeliveryNoteManager = ACOutDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_OutDeliveryNoteManager == null)
                throw new Exception("InDeliveryNoteManager not configured");

            _PickingManager = ACPickingManager.ACRefToServiceInstance(this);
            if (_PickingManager == null)
                throw new Exception("PickingManager not configured");

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");


            _ = ForwardToRemoteStores;
            _ = NavigateOnGenRelated;
            _ = DefaultReservationState;

            bool skipSearchOnStart = ParameterValueT<bool>(Const.SkipSearchOnStart);
            if (!skipSearchOnStart)
            {
                AccessFilterFromFacility.NavSearch();
                AccessFilterToFacility.NavSearch();

                if (Parameters != null && Parameters.Any())
                    InitParams();

                Search(false);
            }

            if (BSOFacilityReservation_Child != null && BSOFacilityReservation_Child.Value != null)
            {
                BSOFacilityReservation_Child.Value.DefaultReservationState = GetDefaultReservationState();
                BSOFacilityReservation_Child.Value.OnReservationChanged += BSOFacilityReservation_Changed;

                if (SelectedFilterMDPickingType != null)
                {
                    BSOFacilityReservation_Child.Value.LoadFilterFacilityLists(SelectedFilterMDPickingType.MDKey);
                }
                else
                {
                    BSOFacilityReservation_Child.Value.LoadFilterFacilityLists(null);
                }
            }

            _MainSyncContext = SynchronizationContext.Current;

            return true;
        }

        /// <summary>
        /// Initializes filter parameters from the constructor parameters passed to the BSOPicking component.
        /// Sets up default filter values for picking type, source/target facilities, delivery address, and picking state
        /// based on the parameter values provided during component instantiation.
        /// Parameters processed:
        /// - PickingType: Sets the selected picking type filter
        /// - FromFacility: Sets the source facility filter and adds to navigation list if not present
        /// - ToFacility: Sets the target facility filter and adds to navigation list if not present  
        /// - CompanyNo: Configures delivery address filter by company number and performs search
        /// - PickingStateIndex: Sets the picking state filter based on state index value
        /// </summary>
        public void InitParams()
        {
            object pickingTypeOb = Parameters["PickingType"];
            if (pickingTypeOb != null)
            {
                string pickingType = pickingTypeOb.ToString();
                SelectedFilterMDPickingType = FilterMDPickingTypeList.FirstOrDefault(c => c.MDKey == pickingType);
            }

            object fromFacilityOb = Parameters["FromFacility"];
            if (fromFacilityOb != null)
            {
                string fromFacilityNo = fromFacilityOb.ToString();
                Facility fromFacility = DatabaseApp.Facility.FirstOrDefault(c => c.FacilityNo == fromFacilityNo);
                if (!AccessFilterFromFacility.NavList.Contains(fromFacility))
                    AccessFilterFromFacility.NavList.Add(fromFacility);
                SelectedFilterFromFacility = fromFacility;
            }

            object toFacilityOb = Parameters["ToFacility"];
            if (toFacilityOb != null)
            {
                string toFacilityNo = toFacilityOb.ToString();
                Facility toFacility = DatabaseApp.Facility.FirstOrDefault(c => c.FacilityNo == toFacilityNo);
                if (!AccessFilterToFacility.NavList.Contains(toFacility))
                    AccessFilterToFacility.NavList.Add(toFacility);
                SelectedFilterToFacility = toFacility;
            }

            object companyNoOb = Parameters["CompanyNo"];
            string companyNo = null;
            if (companyNoOb != null)
            {
                companyNo = companyNoOb.ToString();
                AccessFilterDeliveryAddress.NavACQueryDefinition.SetSearchValue("Company\\CompanyNo", Global.LogicalOperators.equal, companyNo);
                AccessFilterDeliveryAddress.NavSearch();
                SelectedFilterDeliveryAddress = FilterDeliveryAddressList.FirstOrDefault();
            }

            object stateIndex = Parameters["PickingStateIndex"];
            if (stateIndex != null)
                SelectedFilterPickingState = FilterPickingStateList.FirstOrDefault(c => (short)c.Value == (short)stateIndex);
        }

        /// <summary>
        /// Performs cleanup and deinitialization of the BSOPicking component.
        /// Detaches service references, clears access objects, resets navigation properties,
        /// unsubscribes from events, and calls base deinitialization.
        /// This method ensures proper resource cleanup and prevents memory leaks when the component is being destroyed.
        /// </summary>
        /// <param name="deleteACClassTask">If true, removes the component from the persistable application tree</param>
        /// <returns>True if deinitialization was successful, false otherwise</returns>
        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_PickingManager != null)
                ACPickingManager.DetachACRefFromServiceInstance(this, _PickingManager);
            _PickingManager = null;

            if (_InDeliveryNoteManager != null)
                ACInDeliveryNoteManager.DetachACRefFromServiceInstance(this, _InDeliveryNoteManager);
            _InDeliveryNoteManager = null;

            if (_OutDeliveryNoteManager != null)
                ACOutDeliveryNoteManager.DetachACRefFromServiceInstance(this, _OutDeliveryNoteManager);
            _OutDeliveryNoteManager = null;

            if (_ACFacilityManager != null)
                FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            this._AccessBookingFacility = null;
            this._AccessInOrderPos = null;
            this._AccessOutOrderPos = null;
            this._AccessPickingPos = null;
            this._AccessProdOrderPartslistPos = null;
            this._CurrentACMethodBookingDummy = null;
            this._CurrentDNoteInOrderPos = null;
            this._CurrentDNoteOutOrderPos = null;
            this._CurrentFacilityBooking = null;
            this._CurrentFacilityPreBooking = null;
            this._CurrentPickingPos = null;
            this._SelectedDNoteInOrderPos = null;
            this._SelectedDNoteOutOrderPos = null;
            this._SelectedFacilityBooking = null;
            this._SelectedFacilityBookingCharge = null;
            this._SelectedFacilityPreBooking = null;
            this._SelectedPickingPos = null;
            this._BookingFacilityLotNo = null;
            this._PartialQuantity = null;
            this._StateCompletelyAssigned = null;
            this._UnSavedUnAssignedInOrderPos = null;
            this._UnSavedUnAssignedOutOrderPos = null;
            this._UnSavedUnAssignedProdOrderPartslistPos = null;
            _PreBookingAvailableQuantsList = null;
            _SelectedPreBookingAvailableQuants = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPickingPos != null)
            {
                _AccessPickingPos.ACDeInit(false);
                _AccessPickingPos = null;
            }
            if (_AccessBookingFacility != null)
            {
                _AccessBookingFacility.ACDeInit(false);
                _AccessBookingFacility = null;
            }
            if (_AccessInOrderPos != null)
            {
                _AccessInOrderPos.ACDeInit(false);
                _AccessInOrderPos = null;
            }
            if (_AccessOutOrderPos != null)
            {
                _AccessOutOrderPos.ACDeInit(false);
                _AccessOutOrderPos = null;
            }
            if (_AccessProdOrderPartslistPos != null)
            {
                _AccessProdOrderPartslistPos.ACDeInit(false);
                _AccessProdOrderPartslistPos = null;
            }
            if (_AccessPrimary != null)
            {
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }

            if (_AccessBookingFacilityLot != null)
            {
                _AccessBookingFacilityLot.NavSearchExecuted -= _AccessBookingFacilityLot_NavSearchExecuted;
                _AccessBookingFacilityLot.ACDeInit(false);
                _AccessBookingFacilityLot = null;
            }

            if (_AccessBookingFacilityTarget != null)
            {
                _AccessBookingFacilityTarget.ACDeInit(false);
                _AccessBookingFacilityTarget = null;
            }

            if (_AccessFilterFromFacility != null)
            {
                //_AccessFilterFromFacility.NavSearchExecuted -= _AccessBookingFacilityLot_NavSearchExecuted;
                _AccessFilterFromFacility.ACDeInit(false);
                _AccessFilterFromFacility = null;
            }

            if (_AccessFilterToFacility != null)
            {
                //_AccessFilterToFacility.NavSearchExecuted -= _AccessBookingFacilityLot_NavSearchExecuted;
                _AccessFilterToFacility.ACDeInit(false);
                _AccessFilterToFacility = null;
            }

            _IsInward = false;
            _QuantDialogMaterial = null;


            if (BSOFacilityReservation_Child != null && BSOFacilityReservation_Child.Value != null)
            {
                BSOFacilityReservation_Child.Value.OnReservationChanged -= BSOFacilityReservation_Changed;
            }

            _MainSyncContext = null;

            return b;
        }

        /// <summary>
        /// Creates a clone of the current BSOPicking instance.
        /// Copies facility booking related properties from the current instance to the cloned instance.
        /// This method calls the base Clone() method and then transfers specific facility booking state
        /// including the selected facility booking and selected facility booking charge.
        /// </summary>
        /// <returns>A cloned BSOPicking object with copied facility booking properties, or the base clone if casting fails</returns>
        public override object Clone()
        {
            BSOPicking clone = base.Clone() as BSOPicking;
            if (clone != null)
            {
                clone._SelectedFacilityBooking = this._SelectedFacilityBooking;
                clone._SelectedFacilityBookingCharge = this._SelectedFacilityBookingCharge;
                clone._ReportFacilityCharge = this._ReportFacilityCharge;
            }
            return clone;
        }


        private void BSOFacilityReservation_Changed()
        {
            if (CurrentPickingPos != null)
            {
                CurrentPickingPos.OnEntityPropertyChanged(nameof(PickingPos.PickingMaterial));
            }
        }

        #endregion

        #region ChildBSO

        ACChildItem<BSOFacilityExplorer> _BSOFacilityExplorer_Child;
        /// <summary>
        /// Child component for facility exploration functionality.
        /// Provides access to the BSOFacilityExplorer which allows users to browse and select facilities
        /// within the picking order management interface. Used for facility selection dialogs
        /// and facility-related operations in the picking workflow.
        /// </summary>
        [ACPropertyInfo(600, Description = @"Child component for facility exploration functionality.
                                             Provides access to the BSOFacilityExplorer which allows users to browse and select facilities
                                             within the picking order management interface. Used for facility selection dialogs
                                             and facility-related operations in the picking workflow.")]
        [ACChildInfo("BSOFacilityExplorer_Child", typeof(BSOFacilityExplorer))]
        public ACChildItem<BSOFacilityExplorer> BSOFacilityExplorer_Child
        {
            get
            {
                if (_BSOFacilityExplorer_Child == null)
                    _BSOFacilityExplorer_Child = new ACChildItem<BSOFacilityExplorer>(this, "BSOFacilityExplorer_Child");
                return _BSOFacilityExplorer_Child;
            }
        }


        ACChildItem<BSOFacilityReservation> _BSOFacilityReservation_Child;
        /// <summary>
        /// Child component for facility reservation management functionality.
        /// Provides access to the BSOFacilityReservation which handles material reservation operations,
        /// batch planning, and facility allocation for picking positions within the warehouse logistics workflow.
        /// This component manages reservation states, available quantities, and facility selection for efficient
        /// inventory management and material planning in picking operations.
        /// </summary>
        [ACPropertyInfo(600, Description = @"Child component for facility reservation management functionality.
                                             Provides access to the BSOFacilityReservation which handles material reservation operations,
                                             batch planning, and facility allocation for picking positions within the warehouse logistics workflow.
                                             This component manages reservation states, available quantities, and facility selection for efficient
                                             inventory management and material planning in picking operations.")]
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

        ACChildItem<BSOPreferredParameters> _BSOPreferredParameters;
        /// <summary>
        /// Child component for preferred parameters management functionality.
        /// Provides access to the BSOPreferredParameters which handles configuration of preferred parameter values
        /// for workflow methods associated with picking operations. This component enables users to view, modify,
        /// and manage parameter preferences that control the behavior of picking workflows and related processes.
        /// </summary>
        [ACPropertyInfo(603, Description = @"Child component for preferred parameters management functionality.
                                             Provides access to the BSOPreferredParameters which handles configuration of preferred parameter values
                                             for workflow methods associated with picking operations. This component enables users to view, modify,
                                             and manage parameter preferences that control the behavior of picking workflows and related processes.")]
        [ACChildInfo(nameof(BSOPreferredParameters_Child), typeof(BSOPreferredParameters))]
        public ACChildItem<BSOPreferredParameters> BSOPreferredParameters_Child
        {
            get
            {
                if (_BSOPreferredParameters == null)
                    _BSOPreferredParameters = new ACChildItem<BSOPreferredParameters>(this, nameof(BSOPreferredParameters_Child));
                return _BSOPreferredParameters;
            }
        }

        #endregion

        #region BSO->ACProperty

        /// <summary>
        /// Gets or sets the index of the currently selected tab in the user interface.
        /// </summary>
        public int SelectedTab
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the preferred parameters view is currently being displayed in the user interface.
        /// This property is used to track the state of the preferred parameters tab or section in the picking order dialog,
        /// allowing the UI to maintain the correct display mode when showing workflow parameter configurations.
        /// </summary>
        public bool IsShowingPreferredParams
        {
            get;
            set;
        }

        #region Picking -> Filter

        #region Picking -> Filter -> Default filters

        /// <summary>
        /// Gets the default filter configuration for the primary navigation query.
        /// Defines the standard filter criteria including picking number, picking type, delivery dates,
        /// picking state, and delivery company address that are applied when searching for picking orders.
        /// The filters support various logical operators and can be configured as default parameters.
        /// </summary>
        public List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                ACFilterItem acFilterPickingNo = new ACFilterItem(Global.FilterTypes.filter, "PickingNo", Global.LogicalOperators.contains, Global.Operators.and, null, true, true);
                aCFilterItems.Add(acFilterPickingNo);

                ACFilterItem acFilterPickingIndex = new ACFilterItem(Global.FilterTypes.filter, "MDPickingType\\MDKey", Global.LogicalOperators.equal, Global.Operators.and, null, true);
                aCFilterItems.Add(acFilterPickingIndex);

                ACFilterItem phOpen = new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true);
                aCFilterItems.Add(phOpen);

                ACFilterItem acFilterDeliveryDateFrom = new ACFilterItem(FilterTypes.filter, "DeliveryDateFrom", LogicalOperators.greaterThanOrEqual, Operators.and, "", true, false);
                aCFilterItems.Add(acFilterDeliveryDateFrom);

                ACFilterItem acFilterDeliveryDateTo = new ACFilterItem(FilterTypes.filter, "DeliveryDateTo", LogicalOperators.lessThan, Operators.and, "", true, false);
                aCFilterItems.Add(acFilterDeliveryDateTo);

                ACFilterItem phClose = new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true);
                aCFilterItems.Add(phClose);

                ACFilterItem acFilterPickingStateIndex = new ACFilterItem(FilterTypes.filter, "PickingStateIndex", LogicalOperators.equal, Operators.and, "", true);
                aCFilterItems.Add(acFilterPickingStateIndex);

                ACFilterItem acFilterCompanyNo = new ACFilterItem(FilterTypes.filter, "DeliveryCompanyAddress\\CompanyNo", LogicalOperators.equal, Operators.and, "", true);
                aCFilterItems.Add(acFilterCompanyNo);


                return aCFilterItems;
            }
        }

        private List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem acSortPickingNo = new ACSortItem("PickingNo", SortDirections.descending, true);
                acSortItems.Add(acSortPickingNo);

                return acSortItems;
            }
        }

        #endregion

        #region Picking -> Filter -> Properties

        /// <summary>
        /// Indicates whether the filter settings are currently being changed to prevent recursive updates.
        /// This flag is used to avoid triggering property change notifications and related logic
        /// when programmatically updating filter values, preventing infinite loops and 
        /// unwanted side effects during filter synchronization operations.
        /// </summary>
        public bool InFilterChange { get; set; }

        #region Picking -> Filter -> Properties -> FacilityLot

        private string _FilterLotNoFB;
        /// <summary>
        /// Filter property for facility booking lot number.
        /// Used to filter picking orders by lot numbers found in facility booking charges.
        /// Supports filtering on both inward and outward facility lots as well as facility charges.
        /// </summary>
        [ACPropertyInfo(9999, nameof(FilterLotNoFB), ConstApp.LotNo,
                        Description = @"Filter property for facility booking lot number.
                                        Used to filter picking orders by lot numbers found in facility booking charges.
                                        Supports filtering on both inward and outward facility lots as well as facility charges.")]
        public string FilterLotNoFB
        {
            get
            {
                return _FilterLotNoFB;
            }
            set
            {
                if (_FilterLotNoFB != value)
                {
                    _FilterLotNoFB = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _FilterExternLotNoFB;
        /// <summary>
        /// Filter property for external facility booking lot number.
        /// Used to filter picking orders by external lot numbers found in facility booking charges.
        /// Supports filtering on both inward and outward facility lots as well as facility charges.
        /// </summary>
        [ACPropertyInfo(9999, nameof(FilterExternLotNoFB), ConstApp.ExternLotNo,
                        Description = @"Filter property for external facility booking lot number.
                                        Used to filter picking orders by external lot numbers found in facility booking charges.
                                        Supports filtering on both inward and outward facility lots as well as facility charges.")]
        public string FilterExternLotNoFB
        {
            get
            {
                return _FilterExternLotNoFB;
            }
            set
            {
                if (_FilterExternLotNoFB != value)
                {
                    _FilterExternLotNoFB = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _FilterLotNoFR;
        /// <summary>
        /// Filter property for facility reservation lot number.
        /// Used to filter picking orders by lot numbers found in facility reservations.
        /// Filters on FacilityReservation records associated with picking positions,
        /// matching against the FacilityLot.LotNo property.
        /// </summary>
        [ACPropertyInfo(9999, nameof(FilterLotNoFR), ConstApp.LotNo,
                        Description = @"Filter property for facility reservation lot number.
                                        Used to filter picking orders by lot numbers found in facility reservations.
                                        Filters on FacilityReservation records associated with picking positions,
                                        matching against the FacilityLot.LotNo property.")]
        public string FilterLotNoFR
        {
            get
            {
                return _FilterLotNoFR;
            }
            set
            {
                if (_FilterLotNoFR != value)
                {
                    _FilterLotNoFR = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _FilterExternLotNoFR;
        /// <summary>
        /// Filter property for external facility reservation lot number.
        /// Used to filter picking orders by external lot numbers found in facility reservations.
        /// Filters on FacilityReservation records associated with picking positions,
        /// matching against the FacilityLot.ExternLotNo property.
        /// </summary>
        [ACPropertyInfo(9999, nameof(FilterExternLotNoFR), ConstApp.ExternLotNo,
                        Description = @"Filter property for external facility reservation lot number.
                                        Used to filter picking orders by external lot numbers found in facility reservations.
                                        Filters on FacilityReservation records associated with picking positions,
                                        matching against the FacilityLot.ExternLotNo property.")]
        public string FilterExternLotNoFR
        {
            get
            {
                return _FilterExternLotNoFR;
            }
            set
            {
                if (_FilterExternLotNoFR != value)
                {
                    _FilterExternLotNoFR = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        /// <summary>
        /// Filter property for picking order number search.
        /// Allows filtering of picking orders by their picking number using a contains search operation.
        /// This property is bound to the primary navigation query definition and triggers search updates
        /// when changed, enabling users to filter the picking list based on partial picking number matches.
        /// </summary>
        [ACPropertyInfo(300, "FilterPickingPickingNo", "en{'Picking-No.'}de{'Kommissions-Nr.'}",
                        Description = @"Filter property for picking order number search.
                                        Allows filtering of picking orders by their picking number using a contains search operation.
                                        This property is bound to the primary navigation query definition and triggers search updates
                                        when changed, enabling users to filter the picking list based on partial picking number matches.")]
        public string FilterPickingPickingNo
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("PickingNo");
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("PickingNo");
                if (tmp != value)
                {
                    InFilterChange = true;

                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>("PickingNo", value);
                    OnPropertyChanged(nameof(FilterPickingPickingNo));

                    InFilterChange = false;
                }
            }
        }

        #region Picking -> Filter -> Properties -> Date

        /// <summary>
        /// Filter property for delivery date "from" criteria.
        /// Allows filtering of picking orders by their delivery date using a "greater than or equal" comparison.
        /// This property is bound to the primary navigation query definition and triggers search updates
        /// when changed, enabling users to filter the picking list based on delivery date ranges.
        /// The property handles both string and DateTime representations internally for query compatibility.
        /// </summary>
        [ACPropertyInfo(301, "FilterDateFrom", "en{'From'}de{'Von'}",
                        Description = @"Filter property for delivery date ""from"" criteria.
                                        Allows filtering of picking orders by their delivery date using a ""greater than or equal"" comparison.
                                        This property is bound to the primary navigation query definition and triggers search updates
                                        when changed, enabling users to filter the picking list based on delivery date ranges.
                                        The property handles both string and DateTime representations internally for query compatibility.")]
        public DateTime? FilterDateFrom
        {
            get
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("DeliveryDateFrom");
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<DateTime>("DeliveryDateFrom");
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("DeliveryDateFrom", LogicalOperators.greaterThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                {
                    if (value.HasValue)
                    {
                        AccessPrimary.NavACQueryDefinition.SetSearchValue("DeliveryDateFrom", Global.LogicalOperators.greaterThanOrEqual, value.Value);
                        OnPropertyChanged(nameof(FilterDateFrom));
                    }
                }
                else
                {
                    if (value.HasValue)
                    {
                        DateTime tmpdt = AccessInOrderPos.NavACQueryDefinition.GetSearchValue<DateTime>("DeliveryDateFrom", Global.LogicalOperators.greaterThanOrEqual);
                        if (tmpdt != value)
                        {
                            AccessPrimary.NavACQueryDefinition.SetSearchValue("DeliveryDateFrom", Global.LogicalOperators.greaterThanOrEqual, value.Value);
                            OnPropertyChanged(nameof(FilterDateFrom));
                        }
                    }
                    else
                    {
                        AccessPrimary.NavACQueryDefinition.SetSearchValue("DeliveryDateFrom", Global.LogicalOperators.greaterThanOrEqual, "");
                        OnPropertyChanged(nameof(FilterDateFrom));
                    }
                }
            }
        }

        /// <summary>
        /// Filter property for delivery date "to" criteria.
        /// Allows filtering of picking orders by their delivery date using a "less than" comparison operation.
        /// This property is bound to the primary navigation query definition and triggers search updates
        /// when changed, enabling users to filter the picking list based on delivery date ranges.
        /// The property handles both string and DateTime representations internally for query compatibility
        /// and works in conjunction with FilterDateFrom to define date range filters.
        /// </summary>
        [ACPropertyInfo(302, "FilterDateTo", "en{'to'}de{'bis'}",
                        Description = @"Filter property for delivery date ""to"" criteria.
                                        Allows filtering of picking orders by their delivery date using a ""less than"" comparison operation.
                                        This property is bound to the primary navigation query definition and triggers search updates
                                        when changed, enabling users to filter the picking list based on delivery date ranges.
                                        The property handles both string and DateTime representations internally for query compatibility
                                        and works in conjunction with FilterDateFrom to define date range filters.")]
        public DateTime? FilterDateTo
        {
            get
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("DeliveryDateTo");
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<DateTime>("DeliveryDateTo");
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("DeliveryDateTo", LogicalOperators.lessThan);
                if (String.IsNullOrEmpty(tmp))
                {
                    if (value.HasValue)
                    {
                        AccessPrimary.NavACQueryDefinition.SetSearchValue("DeliveryDateTo", Global.LogicalOperators.lessThan, value.Value);
                        OnPropertyChanged(nameof(FilterDateTo));
                    }
                }
                else
                {
                    if (value.HasValue)
                    {
                        DateTime tmpdt = AccessInOrderPos.NavACQueryDefinition.GetSearchValue<DateTime>("DeliveryDateTo", Global.LogicalOperators.lessThan);
                        if (tmpdt != value)
                        {
                            AccessPrimary.NavACQueryDefinition.SetSearchValue("DeliveryDateTo", Global.LogicalOperators.lessThan, value.Value);
                            OnPropertyChanged(nameof(FilterDateTo));
                        }
                    }
                    else
                    {
                        AccessPrimary.NavACQueryDefinition.SetSearchValue("DeliveryDateTo", Global.LogicalOperators.lessThan, "");
                        OnPropertyChanged(nameof(FilterDateTo));
                    }
                }
            }
        }

        #endregion

        #region Picking -> Filter -> Properties -> FilterPickingTypeIndex -> FilterMDPickingType
        private MDPickingType _SelectedFilterMDPickingType;
        /// <summary>
        /// Selected filter property for MDPickingType used to filter picking orders by their picking type.
        /// When changed, automatically updates the primary navigation query definition to apply the filter
        /// on the MDPickingType relationship through the MDKey property. This enables filtering the picking
        /// order list to show only orders of a specific picking type (Receipt, Issue, Relocation, etc.).
        /// The property synchronization with the query filter is protected by the InFilterChange flag
        /// to prevent recursive updates during programmatic filter modifications.
        /// </summary>
        [ACPropertySelected(303, "FilterMDPickingType", "en{'Picking Type'}de{'Kommissioniertyp'}",
                            Description = @"Selected filter property for MDPickingType used to filter picking orders by their picking type.
                                            When changed, automatically updates the primary navigation query definition to apply the filter
                                            on the MDPickingType relationship through the MDKey property. This enables filtering the picking
                                            order list to show only orders of a specific picking type (Receipt, Issue, Relocation, etc.).
                                            The property synchronization with the query filter is protected by the InFilterChange flag
                                            to prevent recursive updates during programmatic filter modifications.")]
        public MDPickingType SelectedFilterMDPickingType
        {
            get
            {
                return _SelectedFilterMDPickingType;
            }
            set
            {
                if (_SelectedFilterMDPickingType != value)
                {
                    _SelectedFilterMDPickingType = value;
                    OnPropertyChanged(nameof(SelectedFilterMDPickingType));

                    string filterPickingKey = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>("MDPickingType\\MDKey");
                    string newPickingKey = null;
                    if (value != null)
                        newPickingKey = value.MDKey;
                    if (newPickingKey != filterPickingKey)
                    {
                        InFilterChange = true;

                        AccessPrimary.NavACQueryDefinition.SetSearchValue<string>("MDPickingType\\MDKey", newPickingKey);
                        OnPropertyChanged(nameof(SelectedFilterMDPickingType));

                        InFilterChange = false;
                    }
                }
            }
        }


        private List<MDPickingType> _FilterMDPickingTypeList;

        /// <summary>
        /// Gets the list of available MDPickingType filter options for the picking orders search.
        /// This property provides a dropdown list of picking types (Receipt, Issue, Relocation, etc.) 
        /// that can be used to filter the picking order results. The list is loaded from the database
        /// and cached for performance. When accessed, it also attempts to set the SelectedFilterMDPickingType
        /// based on any existing PickingTypeIndex filter value in the navigation query definition.
        /// </summary>
        [ACPropertyList(304, "FilterMDPickingType",
                        Description = @"Gets the list of available MDPickingType filter options for the picking orders search.
                                        This property provides a dropdown list of picking types (Receipt, Issue, Relocation, etc.) 
                                        that can be used to filter the picking order results. The list is loaded from the database
                                        and cached for performance. When accessed, it also attempts to set the SelectedFilterMDPickingType
                                        based on any existing PickingTypeIndex filter value in the navigation query definition.")]
        public List<MDPickingType> FilterMDPickingTypeList
        {
            get
            {
                if (_FilterMDPickingTypeList == null)
                    _FilterMDPickingTypeList = LoadFilterMDPickingTypeList();
                short? filterPickingTypeIndex = AccessPrimary.NavACQueryDefinition.GetSearchValue<short?>("PickingTypeIndex");
                if (filterPickingTypeIndex != null)
                    SelectedFilterMDPickingType = FilterMDPickingTypeList.FirstOrDefault(c => c.MDPickingTypeIndex == (short)filterPickingTypeIndex.Value);
                return _FilterMDPickingTypeList;
            }
        }

        private List<MDPickingType> LoadFilterMDPickingTypeList()
        {
            return DatabaseApp.MDPickingType.OrderBy(c => c.SortIndex).ToList();

        }
        #endregion

        #region Picking -> Filter -> Properties -> FilterPickingState
        private ACValueItem _SelectedFilterPickingState;
        /// <summary>
        /// Selected filter property for picking state used to filter picking orders by their current state.
        /// When changed, automatically updates the primary navigation query definition to apply the filter
        /// on the PickingStateIndex property. This enables filtering the picking order list to show only
        /// orders in a specific state (New, InProcess, Finished, Cancelled, etc.).
        /// The property synchronization with the query filter is protected by the InFilterChange flag
        /// to prevent recursive updates during programmatic filter modifications.
        /// </summary>
        [ACPropertySelected(305, "FilterPickingState", "en{'Picking Status'}de{'Status'}",
                            Description = @"Selected filter property for picking state used to filter picking orders by their current state.
                                            When changed, automatically updates the primary navigation query definition to apply the filter
                                            on the PickingStateIndex property. This enables filtering the picking order list to show only
                                            orders in a specific state (New, InProcess, Finished, Cancelled, etc.).
                                            The property synchronization with the query filter is protected by the InFilterChange flag
                                            to prevent recursive updates during programmatic filter modifications.")]
        public ACValueItem SelectedFilterPickingState
        {
            get
            {
                return _SelectedFilterPickingState;
            }
            set
            {
                if (_SelectedFilterPickingState != value)
                {
                    _SelectedFilterPickingState = value;
                    OnPropertyChanged(nameof(SelectedFilterPickingState));

                    short? filterPickingStateIndex = AccessPrimary.NavACQueryDefinition.GetSearchValue<short?>("PickingStateIndex");
                    short? newPickingStateIndex = null;
                    if (value != null)
                        newPickingStateIndex = (short)(value.Value);
                    if (newPickingStateIndex != filterPickingStateIndex)
                    {
                        InFilterChange = true;

                        AccessPrimary.NavACQueryDefinition.SetSearchValue<short?>("PickingStateIndex", newPickingStateIndex);
                        OnPropertyChanged(nameof(SelectedFilterPickingState));

                        InFilterChange = false;
                    }
                }
            }
        }

        private List<ACValueItem> _FilterPickingStateList;
        /// <summary>
        /// Gets the list of available picking state filter options for the picking orders search.
        /// This property provides a dropdown list of picking states (New, InProcess, Finished, Cancelled, etc.) 
        /// that can be used to filter the picking order results. The list is loaded from the database
        /// and cached for performance. When accessed, it also attempts to set the SelectedFilterPickingState
        /// based on any existing PickingStateIndex filter value in the navigation query definition.
        /// </summary>
        [ACPropertyList(306, "FilterPickingState",
                        Description = @"Gets the list of available picking state filter options for the picking orders search.
                                        This property provides a dropdown list of picking states (New, InProcess, Finished, Cancelled, etc.) 
                                        that can be used to filter the picking order results. The list is loaded from the database
                                        and cached for performance. When accessed, it also attempts to set the SelectedFilterPickingState
                                        based on any existing PickingStateIndex filter value in the navigation query definition.")]
        public List<ACValueItem> FilterPickingStateList
        {
            get
            {
                if (_FilterPickingStateList == null)
                    _FilterPickingStateList = LoadFilterPickingStateList();
                short? filterPickingStateIndex = AccessPrimary.NavACQueryDefinition.GetSearchValue<short?>("PickingStateIndex");
                if (filterPickingStateIndex != null)
                {
                    SelectedFilterPickingState = _FilterPickingStateList.FirstOrDefault(c => (short)c.Value == (short)filterPickingStateIndex.Value);
                }
                return _FilterPickingStateList;
            }
        }

        private List<ACValueItem> LoadFilterPickingStateList()
        {
            return DatabaseApp.PickingStateList;
        }

        #endregion

        #region Picking -> Filter -> Properties -> FilterDeliveryAddress

        /// <summary>
        /// Gets the default filter configuration for delivery address navigation query.
        /// Defines filter criteria for filtering delivery address records by company number,
        /// supporting exact match filtering on the related company's CompanyNo property.
        /// Used to configure default filter settings for the delivery address selection interface.
        /// </summary>
        public List<ACFilterItem> FilterDeliveryAddressDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                ACFilterItem acFilterCompanyNo = new ACFilterItem(Global.FilterTypes.filter, "Company\\CompanyNo", Global.LogicalOperators.equal, Global.Operators.and, null, true, true);
                aCFilterItems.Add(acFilterCompanyNo);

                return aCFilterItems;
            }
        }

        private List<ACSortItem> FilterDeliveryAddressDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem acSortPickingNo = new ACSortItem("Company\\CompanyNo", SortDirections.descending, true);
                acSortItems.Add(acSortPickingNo);

                return acSortItems;
            }
        }

        ACAccess<CompanyAddress> _AccessFilterDeliveryAddress;
        /// <summary>
        /// Gets the access object for filtering delivery addresses in the picking order interface.
        /// Provides navigation and query functionality for CompanyAddress entities used as delivery addresses.
        /// The access object is configured with default filter and sort columns for company number filtering
        /// and automatically performs a search when first accessed to populate the available delivery addresses.
        /// </summary>
        [ACPropertyAccess(9999, "FilterDeliveryAddress",
                          Description = @"Gets the access object for filtering delivery addresses in the picking order interface.
                                          Provides navigation and query functionality for CompanyAddress entities used as delivery addresses.
                                          The access object is configured with default filter and sort columns for company number filtering
                                          and automatically performs a search when first accessed to populate the available delivery addresses.")]
        public ACAccess<CompanyAddress> AccessFilterDeliveryAddress
        {
            get
            {
                if (_AccessFilterDeliveryAddress == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + CompanyAddress.ClassName, ACType.ACIdentifier);

                    acQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(FilterDeliveryAddressDefaultSort);
                    acQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(FilterDeliveryAddressDefaultFilter);

                    _AccessFilterDeliveryAddress = acQueryDefinition.NewAccessNav<CompanyAddress>("FilterDeliveryAddress", this);
                    _AccessFilterDeliveryAddress.NavSearch();
                }
                return _AccessFilterDeliveryAddress;
            }
        }

        /// <summary>
        /// Gets the list of available company addresses for delivery address filtering.
        /// This property provides access to the navigation list of CompanyAddress entities
        /// that can be used as delivery addresses in picking order searches and operations.
        /// The list is populated through the AccessFilterDeliveryAddress navigation object
        /// and is filtered based on the delivery address search criteria configured in the
        /// FilterDeliveryAddressDefaultFilter property.
        /// </summary>
        [ACPropertyInfo(9999, "FilterDeliveryAddress",
                        Description = @"Gets the list of available company addresses for delivery address filtering.
                                        This property provides access to the navigation list of CompanyAddress entities
                                        that can be used as delivery addresses in picking order searches and operations.
                                        The list is populated through the AccessFilterDeliveryAddress navigation object
                                        and is filtered based on the delivery address search criteria configured in the
                                        FilterDeliveryAddressDefaultFilter property.")]
        public IEnumerable<CompanyAddress> FilterDeliveryAddressList
        {
            get
            {
                return AccessFilterDeliveryAddress.NavList;
            }
        }

        private CompanyAddress _SelectedFilterDeliveryAddress;
        /// <summary>
        /// Selected filter property for delivery address used to filter picking orders by their delivery company address.
        /// When changed, updates the selected delivery address for filtering purposes. This property allows users to 
        /// filter the picking order list to show only orders associated with a specific delivery address/company.
        /// The property is used in conjunction with the navigation query to apply delivery address filtering
        /// on the picking orders displayed in the user interface.
        /// </summary>
        [ACPropertySelected(9999, "FilterDeliveryAddress", "en{'Delivery Address'}de{'Lieferadresse'}",
                            Description = @"Selected filter property for delivery address used to filter picking orders by their delivery company address.
                                            When changed, updates the selected delivery address for filtering purposes. This property allows users to 
                                            filter the picking order list to show only orders associated with a specific delivery address/company.
                                            The property is used in conjunction with the navigation query to apply delivery address filtering
                                            on the picking orders displayed in the user interface.")]
        public CompanyAddress SelectedFilterDeliveryAddress
        {
            get
            {
                return _SelectedFilterDeliveryAddress;
            }
            set
            {
                if (_SelectedFilterDeliveryAddress != value)
                {
                    _SelectedFilterDeliveryAddress = value;

                    OnPropertyChanged(nameof(SelectedFilterDeliveryAddress));
                }
            }
        }


        #endregion

        #region Filter -> From(To)Facility

        /// <summary>
        /// Gets the default filter configuration for facility filtering in navigation queries.
        /// Returns an empty list of ACFilterItem objects that can be used as a base for facility filtering.
        /// This property provides the default filter structure for facility-related navigation components
        /// without applying any specific filter criteria, allowing derived classes or methods to add
        /// custom filters as needed for facility selection and filtering operations.
        /// </summary>
        public List<ACFilterItem> FilterFacilityNavigationqueryDefaultFilter
        {
            get
            {
                List<ACFilterItem> aCFilterItems = new List<ACFilterItem>();

                return aCFilterItems;
            }
        }

        private List<ACSortItem> FilterFacilityNavigationqueryDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem acSortPickingNo = new ACSortItem("FacilityNo", SortDirections.ascending, true);
                acSortItems.Add(acSortPickingNo);

                return acSortItems;
            }
        }

        ACAccess<Facility> _AccessFilterFromFacility;
        /// <summary>
        /// Gets the access object for filtering source facilities in the picking order interface.
        /// Provides navigation and query functionality for Facility entities used as source locations.
        /// The access object is configured with default filter and sort columns for facility filtering
        /// and automatically performs a search when first accessed to populate the available source facilities.
        /// </summary>
        [ACPropertyAccess(9999, "FilterFromFacility",
                          Description = @"Gets the access object for filtering source facilities in the picking order interface.
                                          Provides navigation and query functionality for Facility entities used as source locations.
                                          The access object is configured with default filter and sort columns for facility filtering
                                          and automatically performs a search when first accessed to populate the available source facilities.")]
        public ACAccess<Facility> AccessFilterFromFacility
        {
            get
            {
                if (_AccessFilterFromFacility == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Facility.ClassName, ACType.ACIdentifier);

                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(FilterFacilityNavigationqueryDefaultSort);
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(FilterFacilityNavigationqueryDefaultFilter);

                        foreach (ACFilterItem aCFilterItem in navACQueryDefinition.ACFilterColumns)
                            aCFilterItem.PropertyChanged += ACFilterItem_PropertyChanged;
                    }

                    _AccessFilterFromFacility = navACQueryDefinition.NewAccessNav<Facility>("FilterFromFacility", this);
                    //_AccessFilterFromFacility.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessFilterFromFacility;
            }
        }

        /// <summary>
        /// Gets the list of available source facilities for filtering purposes.
        /// This property provides access to the navigation list of Facility entities
        /// that can be used as source facilities in picking order searches and operations.
        /// The list is populated through the AccessFilterFromFacility navigation object
        /// and is filtered based on the facility search criteria configured in the
        /// FilterFacilityNavigationqueryDefaultFilter property.
        /// </summary>
        [ACPropertyInfo(9999, "FilterFromFacility",
                        Description = @"Gets the list of available source facilities for filtering purposes.
                                        This property provides access to the navigation list of Facility entities
                                        that can be used as source facilities in picking order searches and operations.
                                        The list is populated through the AccessFilterFromFacility navigation object
                                        and is filtered based on the facility search criteria configured in the
                                        FilterFacilityNavigationqueryDefaultFilter property.")]
        public IEnumerable<Facility> FilterFromFacilityList
        {
            get
            {
                return AccessFilterFromFacility.NavList;
            }
        }

        private Facility _SelectedFilterFromFacility;
        /// <summary>
        /// Selected filter property for source facility used to filter picking orders by their source location.
        /// When changed, updates the selected source facility for filtering purposes. This property allows users to 
        /// filter the picking order list to show only orders that have positions with materials being moved from 
        /// a specific source facility. The property is used in conjunction with the navigation query to apply 
        /// source facility filtering on the picking orders displayed in the user interface.
        /// </summary>
        [ACPropertySelected(9999, "FilterFromFacility", "en{'From facility'}de{'Von Lagerplatz'}",
                            Description = @"Selected filter property for source facility used to filter picking orders by their source location.
                                            When changed, updates the selected source facility for filtering purposes. This property allows users to 
                                            filter the picking order list to show only orders that have positions with materials being moved from 
                                            a specific source facility. The property is used in conjunction with the navigation query to apply 
                                            source facility filtering on the picking orders displayed in the user interface.")]
        public Facility SelectedFilterFromFacility
        {
            get
            {
                return _SelectedFilterFromFacility;
            }
            set
            {
                if (_SelectedFilterFromFacility != value)
                {
                    _SelectedFilterFromFacility = value;
                    OnPropertyChanged(nameof(SelectedFilterFromFacility));
                }
            }
        }

        ACAccess<Facility> _AccessFilterToFacility;
        /// <summary>
        /// Gets the access object for filtering target facilities in the picking order interface.
        /// Provides navigation and query functionality for Facility entities used as target locations.
        /// The access object is configured with default filter and sort columns for facility filtering
        /// and automatically performs a search when first accessed to populate the available target facilities.
        /// </summary>
        [ACPropertyAccess(9999, "FilterToFacility",
                          Description = @"Gets the access object for filtering target facilities in the picking order interface.
                                          Provides navigation and query functionality for Facility entities used as target locations.
                                          The access object is configured with default filter and sort columns for facility filtering
                                          and automatically performs a search when first accessed to populate the available target facilities.")]
        public ACAccess<Facility> AccessFilterToFacility
        {
            get
            {
                if (_AccessFilterToFacility == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Facility.ClassName, ACType.ACIdentifier);

                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(FilterFacilityNavigationqueryDefaultSort);
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(FilterFacilityNavigationqueryDefaultFilter);

                        foreach (ACFilterItem aCFilterItem in navACQueryDefinition.ACFilterColumns)
                            aCFilterItem.PropertyChanged += ACFilterItem_PropertyChanged;
                    }

                    _AccessFilterToFacility = navACQueryDefinition.NewAccessNav<Facility>("FilterToFacility", this);
                    //_AccessFilterToFacility.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessFilterToFacility;
            }
        }

        /// <summary>
        /// Gets the list of available target facilities for filtering purposes.
        /// This property provides access to the navigation list of Facility entities
        /// that can be used as target facilities in picking order searches and operations.
        /// The list is populated through the AccessFilterToFacility navigation object
        /// and is filtered based on the facility search criteria configured in the
        /// FilterFacilityNavigationqueryDefaultFilter property.
        /// </summary>
        [ACPropertyInfo(9999, "FilterToFacility",
                        Description = @"Gets the list of available target facilities for filtering purposes.
                                        This property provides access to the navigation list of Facility entities
                                        that can be used as target facilities in picking order searches and operations.
                                        The list is populated through the AccessFilterToFacility navigation object
                                        and is filtered based on the facility search criteria configured in the
                                        FilterFacilityNavigationqueryDefaultFilter property.")]
        public IEnumerable<Facility> FilterToFacilityList
        {
            get
            {
                return AccessFilterToFacility.NavList;
            }
        }

        private Facility _SelectedFilterToFacility;
        /// <summary>
        /// Selected filter property for target facility used to filter picking orders by their target location.
        /// When changed, updates the selected target facility for filtering purposes. This property allows users to 
        /// filter the picking order list to show only orders that have positions with materials being moved to 
        /// a specific target facility. The property is used in conjunction with the navigation query to apply 
        /// target facility filtering on the picking orders displayed in the user interface.
        /// </summary>
        [ACPropertySelected(9999, "FilterToFacility", "en{'To facility'}de{'Zur Lagerplatz'}",
                            Description = @"Selected filter property for target facility used to filter picking orders by their target location.
                                            When changed, updates the selected target facility for filtering purposes. This property allows users to 
                                            filter the picking order list to show only orders that have positions with materials being moved to 
                                            a specific target facility. The property is used in conjunction with the navigation query to apply 
                                            target facility filtering on the picking orders displayed in the user interface.")]
        public Facility SelectedFilterToFacility
        {
            get
            {
                return _SelectedFilterToFacility;
            }
            set
            {
                if (_SelectedFilterToFacility != value)
                {
                    _SelectedFilterToFacility = value;
                    OnPropertyChanged(nameof(SelectedFilterToFacility));
                }
            }
        }

        #endregion

        private string _FilterMaterialNo;
        /// <summary>
        /// Filter property for material number search.
        /// Allows filtering of picking orders by material numbers found in picking positions.
        /// Supports filtering on materials assigned to InOrderPos, OutOrderPos, or directly to PickingPos,
        /// matching against both MaterialNo and MaterialName1 properties using a contains search operation.
        /// </summary>
        [ACPropertyInfo(9999, "FilterMaterialNo", ConstApp.Material,
                        Description = @"Filter property for material number search.
                                        Allows filtering of picking orders by material numbers found in picking positions.
                                        Supports filtering on materials assigned to InOrderPos, OutOrderPos, or directly to PickingPos,
                                        matching against both MaterialNo and MaterialName1 properties using a contains search operation.")]
        public string FilterMaterialNo
        {
            get
            {
                return _FilterMaterialNo;
            }
            set
            {
                if (_FilterMaterialNo != value)
                {
                    _FilterMaterialNo = value;
                    OnPropertyChanged(nameof(FilterMaterialNo));
                }
            }
        }

        #endregion

        #endregion

        #region Picking

        /// <summary>
        /// The AccessNav property provides access to the primary navigation interface for picking orders.
        /// This property returns the AccessPrimary navigation object which handles data access, filtering,
        /// and navigation operations for the Picking entity. It serves as the main entry point for
        /// interacting with picking order data in the business service object.
        /// </summary>
        public override IAccessNav AccessNav { get { return AccessPrimary; } }

        ACAccessNav<Picking> _AccessPrimary;

        /// <summary>
        /// Gets the primary navigation access object for picking order data management.
        /// This property provides the main data access interface for navigating, filtering, and querying
        /// Picking entities in the business logic layer. It initializes the navigation query definition
        /// with default filter and sort columns, sets up property change event handlers for filter items,
        /// and configures the search execution event handler for enhanced query processing.
        /// The AccessPrimary serves as the central data gateway for all picking order operations
        /// including CRUD operations, filtering, and navigation functionality.
        /// </summary>
        [ACPropertyAccessPrimary(690, "Picking",
                                Description = @"Gets the primary navigation access object for picking order data management.
                                                This property provides the main data access interface for navigating, filtering, and querying
                                                Picking entities in the business logic layer. It initializes the navigation query definition
                                                with default filter and sort columns, sets up property change event handlers for filter items,
                                                and configures the search execution event handler for enhanced query processing.
                                                The AccessPrimary serves as the central data gateway for all picking order operations
                                                including CRUD operations, filtering, and navigation functionality.")]
        public virtual ACAccessNav<Picking> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);

                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(NavigationqueryDefaultSort);
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);

                        foreach (ACFilterItem aCFilterItem in navACQueryDefinition.ACFilterColumns)
                            aCFilterItem.PropertyChanged += ACFilterItem_PropertyChanged;
                    }

                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Picking>("Picking", this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;

                }
                return _AccessPrimary;
            }
        }

        private void ACFilterItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ACFilterItem aCFilterItem = sender as ACFilterItem;
            if (e.PropertyName == "SearchWord" && !InFilterChange)
            {
                switch (aCFilterItem.PropertyName)
                {
                    case nameof(Picking.PickingNo):
                        OnPropertyChanged(nameof(FilterPickingPickingNo));
                        break;
                    case nameof(Picking.DeliveryDateFrom):
                        OnPropertyChanged(nameof(FilterDateFrom));
                        OnPropertyChanged(nameof(FilterDateTo));
                        break;
                    case nameof(MDPickingType.MDPickingTypeIndex):
                        OnPropertyChanged(nameof(SelectedFilterMDPickingType));
                        break;
                    case nameof(Picking.PickingStateIndex):
                        OnPropertyChanged(nameof(SelectedFilterPickingState));
                        break;
                }
            }
        }

        protected IQueryable<Picking> _AccessPrimary_NavSearchExecuting(IQueryable<Picking> result)
        {
            IQueryable<Picking> query = result as IQueryable<Picking>;
            if (query != null)
            {
                query.Include(c => c.VisitorVoucher).Include(c => c.MDPickingType);
            }

            if (SelectedFilterDeliveryAddress != null)
                result = result.Where(c => c.DeliveryCompanyAddressID == SelectedFilterDeliveryAddress.CompanyAddressID);

            if (SelectedFilterFromFacility != null)
            {
                result =
                    result
                    .Where(c =>
                                !c.PickingPos_Picking.Any()// show picking without positions
                                || c.PickingPos_Picking
                                .Any(x =>
                                            x.FromFacility != null
                                            && x.FromFacility.FacilityID == SelectedFilterFromFacility.FacilityID
                                    )
                          );
            }

            if (SelectedFilterToFacility != null)
            {
                result =
                    result
                    .Where(c =>
                                !c.PickingPos_Picking.Any() // show picking without positions
                                || c.PickingPos_Picking
                                .Any(x =>
                                            x.ToFacility != null
                                            && x.ToFacility.FacilityID == SelectedFilterToFacility.FacilityID
                                    )
                            );
            }

            if (!string.IsNullOrEmpty(FilterMaterialNo))
                result =
                    result
                    .Where(c =>
                                c.PickingPos_Picking
                                .Any(x =>
                                            (
                                                x.InOrderPos != null
                                                && (x.InOrderPos.Material.MaterialNo.Contains(FilterMaterialNo) || x.InOrderPos.Material.MaterialName1.Contains(FilterMaterialNo))
                                            )
                                            || (
                                                x.OutOrderPos != null
                                                && (x.OutOrderPos.Material.MaterialNo.Contains(FilterMaterialNo) || x.OutOrderPos.Material.MaterialName1.Contains(FilterMaterialNo))
                                            ) || (
                                                x.PickingMaterial != null
                                                && (x.PickingMaterial.MaterialNo.Contains(FilterMaterialNo) || x.PickingMaterial.MaterialName1.Contains(FilterMaterialNo))
                                            )

                                )
                           );

            // FilterLotNoFB
            if (!string.IsNullOrEmpty(FilterLotNoFB))
            {
                result =
                    result
                    .Where(c =>
                            c.PickingPos_Picking
                            .SelectMany(x => x.FacilityBookingCharge_PickingPos)
                            .Where(x =>
                                        (
                                            x.InwardFacilityLot != null
                                            && x.InwardFacilityLot.LotNo.Contains(FilterLotNoFB)
                                        )
                                        ||
                                        (
                                            x.InwardFacilityCharge != null
                                            && x.InwardFacilityCharge.FacilityLot != null
                                            && x.InwardFacilityCharge.FacilityLot.LotNo.Contains(FilterLotNoFB)
                                        )
                                        ||
                                        (
                                            x.OutwardFacilityLot != null
                                            && x.OutwardFacilityLot.LotNo.Contains(FilterLotNoFB)
                                        )
                                        ||
                                        (
                                            x.OutwardFacilityCharge != null
                                            && x.OutwardFacilityCharge.FacilityLot != null
                                            && x.OutwardFacilityCharge.FacilityLot.LotNo.Contains(FilterLotNoFB)
                                        )
                                    )
                            .Any()
                        );
            }

            // FilterExternLotNoFB
            if (!string.IsNullOrEmpty(FilterExternLotNoFB))
            {
                result =
                    result
                    .Where(c =>
                            c.PickingPos_Picking
                            .SelectMany(x => x.FacilityBookingCharge_PickingPos)
                            .Where(x =>
                                        (
                                            x.InwardFacilityLot != null
                                            && !string.IsNullOrEmpty(x.InwardFacilityLot.ExternLotNo)
                                            && x.InwardFacilityLot.ExternLotNo.Contains(FilterExternLotNoFB)
                                        )
                                        ||
                                        (
                                            x.InwardFacilityCharge != null
                                            && x.InwardFacilityCharge.FacilityLot != null
                                            && !string.IsNullOrEmpty(x.InwardFacilityCharge.FacilityLot.ExternLotNo)
                                            && x.InwardFacilityCharge.FacilityLot.ExternLotNo.Contains(FilterExternLotNoFB)
                                        )
                                        ||
                                        (
                                            x.OutwardFacilityLot != null
                                            && !string.IsNullOrEmpty(x.OutwardFacilityLot.ExternLotNo)
                                            && x.OutwardFacilityLot.ExternLotNo.Contains(FilterExternLotNoFB)
                                        )
                                        ||
                                        (
                                            x.OutwardFacilityCharge != null
                                            && x.OutwardFacilityCharge.FacilityLot != null
                                            && !string.IsNullOrEmpty(x.OutwardFacilityCharge.FacilityLot.ExternLotNo)
                                            && x.OutwardFacilityCharge.FacilityLot.ExternLotNo.Contains(FilterExternLotNoFB)
                                        )
                                    )
                            .Any()
                        );
            }


            // FilterLotNoFR
            if (!string.IsNullOrEmpty(FilterLotNoFR))
            {
                result =
                    result
                    .Where(c =>
                            c.PickingPos_Picking
                            .SelectMany(x => x.FacilityReservation_PickingPos)
                            .Where(x =>
                                        x.FacilityLot != null
                                        && x.FacilityLot.LotNo.Contains(FilterLotNoFR)
                                    )
                            .Any()
                        );
            }

            // FilterExternLotNoFR
            if (!string.IsNullOrEmpty(FilterExternLotNoFR))
            {
                result =
                    result
                    .Where(c =>
                            c.PickingPos_Picking
                            .SelectMany(x => x.FacilityReservation_PickingPos)
                            .Where(x =>
                                        x.FacilityLot != null
                                        && !string.IsNullOrEmpty(x.FacilityLot.ExternLotNo)
                                        && x.FacilityLot.LotNo.Contains(FilterExternLotNoFR)
                                    )
                            .Any()
                        );
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the currently selected picking order for display and editing operations.
        /// This property serves as the primary interface for accessing and modifying the active picking order
        /// within the picking management workflow. When set, it triggers various update operations including
        /// loading workflow configurations, refreshing related position lists, and managing remote store forwarding.
        /// The setter performs the following operations:
        /// - Updates the primary navigation access current record
        /// - Loads and configures process workflow settings
        /// - Refreshes picking positions and related order position lists
        /// - Manages visited pickings collection for remote store forwarding
        /// - Triggers property change notifications for UI updates
        /// During loading operations (_InLoad = true), the picker positions are automatically refreshed
        /// from the database to ensure data consistency. If the current picking positions become empty,
        /// any existing CurrentPickingPos reference is cleared to maintain data integrity.
        /// </summary>
        [ACPropertyCurrent(601, "Picking",
                           Description = @" Gets or sets the currently selected picking order for display and editing operations.
                                            This property serves as the primary interface for accessing and modifying the active picking order
                                            within the picking management workflow. When set, it triggers various update operations including
                                            loading workflow configurations, refreshing related position lists, and managing remote store forwarding.
                                            The setter performs the following operations:
                                            - Updates the primary navigation access current record
                                            - Loads and configures process workflow settings
                                            - Refreshes picking positions and related order position lists
                                            - Manages visited pickings collection for remote store forwarding
                                            - Triggers property change notifications for UI updates
                                            During loading operations (_InLoad = true), the picker positions are automatically refreshed
                                            from the database to ensure data consistency. If the current picking positions become empty,
                                            any existing CurrentPickingPos reference is cleared to maintain data integrity.")]
        public Picking CurrentPicking
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

                LoadSelectedProcessWorkflow(CurrentPicking);
                LoadProcessWorkflowPresenter(CurrentPicking);

                OnPropertyChanged(nameof(CurrentPicking));
                if (value != null && _InLoad)
                    value.PickingPos_Picking.AutoRefresh(value.PickingPos_PickingReference, value);
                OnPropertyChanged(nameof(PickingPosList));
                if (value != null && CurrentPickingPos != null)
                {
                    if (!value.PickingPos_Picking.Any())
                        CurrentPickingPos = null;
                }
                OnPropertyChanged(nameof(CurrentPickingPos));
                RefreshInOrderPosList();
                RefreshOutOrderPosList();
                RefreshProdOrderPartslistPosList();
                if (value != null
                    && ForwardToRemoteStores)
                {
                    if (_VisitedPickings == null)
                        _VisitedPickings = new List<Picking>();
                    if (!_VisitedPickings.Contains(value))
                        _VisitedPickings.Add(value);
                }
            }
        }

        private List<Picking> _PickingList;
        /// <summary>
        /// Gets or sets the list of picking orders for display in the user interface.
        /// This property contains the filtered and sorted collection of Picking entities
        /// that match the current search criteria. The list is populated when the Search()
        /// method is executed and serves as the data source for navigation controls.
        /// When set, triggers property change notification to update bound UI controls.
        /// </summary>
        [ACPropertyList(602, "Picking",
                        Description = @"Gets or sets the list of picking orders for display in the user interface.
                                        This property contains the filtered and sorted collection of Picking entities
                                        that match the current search criteria. The list is populated when the Search()
                                        method is executed and serves as the data source for navigation controls.
                                        When set, triggers property change notification to update bound UI controls.")]
        public List<Picking> PickingList
        {
            get
            {
                return _PickingList;
            }
            set
            {
                _PickingList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected picking order from the list for UI selection purposes.
        /// This property represents the picking order that is highlighted or selected in the user interface,
        /// typically in a list or grid control. When set, it updates the primary navigation access object
        /// and triggers property change notifications to update bound UI controls.
        /// This property is used for selection tracking and differs from CurrentPicking which represents
        /// the active picking order being edited or displayed in detail views.
        /// </summary>
        [ACPropertySelected(603, "Picking",
                            Description = @"Gets or sets the currently selected picking order from the list for UI selection purposes.
                                            This property represents the picking order that is highlighted or selected in the user interface,
                                            typically in a list or grid control. When set, it updates the primary navigation access object
                                            and triggers property change notifications to update bound UI controls.
                                            This property is used for selection tracking and differs from CurrentPicking which represents
                                            the active picking order being edited or displayed in detail views.")]
        public Picking SelectedPicking
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
                OnPropertyChanged(nameof(SelectedPicking));
            }
        }

        /// <summary>
        /// Finishes a picking order by completing all related delivery processes and finalizing the order status.
        /// Invokes the PickingManager to perform order completion which handles delivery note processing,
        /// facility bookings, and final status updates. If warnings or errors occur during the finish process,
        /// prompts the user for confirmation before forcing completion. Updates the UI to reflect any changes
        /// made to the current picking order after the finish operation completes.
        /// </summary>
        [ACMethodInfo("", "en{'Finish order'}de{'Auftrag beenden'}", 650, true,
                      Description = @"Finishes a picking order by completing all related delivery processes and finalizing the order status.
                                     Invokes the PickingManager to perform order completion which handles delivery note processing,
                                     facility bookings, and final status updates. If warnings or errors occur during the finish process,
                                     prompts the user for confirmation before forcing completion. Updates the UI to reflect any changes
                                     made to the current picking order after the finish operation completes.")]
        public virtual void FinishOrder()
        {
            if (PickingManager == null)
                return;

            MsgWithDetails msgWithDetails = PickingManager.FinishOrder(DatabaseApp, CurrentPicking, InDeliveryNoteManager, OutDeliveryNoteManager, ACFacilityManager);
            if (msgWithDetails != null)
            {
                if (Messages.Msg(msgWithDetails, MsgResult.No, eMsgButton.YesNo) == MsgResult.Yes)
                {
                    msgWithDetails = PickingManager.FinishOrder(DatabaseApp, CurrentPicking, InDeliveryNoteManager, OutDeliveryNoteManager, ACFacilityManager, true);
                    if (msgWithDetails != null)
                    {
                        Messages.Msg(msgWithDetails);
                    }
                }
            }

            OnPropertyChanged(nameof(CurrentPicking));
        }

        /// <summary>
        /// Check if the current picking order can be completed(finished).
        /// </summary>
        /// <returns>True if the picking order can be finished, false otherwise.</returns>
        public virtual bool IsEnabledFinishOrder()
        {
            return CurrentPicking != null && (CurrentPicking.PickingState < PickingStateEnum.Finished || CurrentPicking.PickingState == PickingStateEnum.WaitOnManualClosing);
        }

        #endregion

        #region PickingPos

        #region PickingPos -> PositionFacilityFrom

        private bool? _FilterPositionFacilityFrom = null;
        /// <summary>
        /// Gets or sets a filter option for controlling the display of source facilities in the picking position interface.
        /// When set to true, only shows source facilities that currently contain material (non-empty facilities).
        /// When set to false, shows all source facilities regardless of material content.
        /// When set to null, no specific filtering is applied based on material content.
        /// This filter helps users focus on relevant facilities during picking operations by hiding empty storage locations.
        /// </summary>
        [ACPropertyInfo(814, "", "en{'Only show source bins with material'}de{'Zeige Quelle-Lagerplätze mit Material'}",
                        Description = @"Gets or sets a filter option for controlling the display of source facilities in the picking position interface.
                                        When set to true, only shows source facilities that currently contain material (non-empty facilities).
                                        When set to false, shows all source facilities regardless of material content.
                                        When set to null, no specific filtering is applied based on material content.
                                        This filter helps users focus on relevant facilities during picking operations by hiding empty storage locations.")]
        public bool? FilterPositionFacilityFrom
        {
            get
            {
                return _FilterPositionFacilityFrom;
            }
            set
            {
                if (_FilterPositionFacilityFrom != value)
                {
                    _FilterPositionFacilityFrom = value;
                    OnPropertyChanged();
                    RefreshFilterFacilityAccess(AccessPositionFacilityFrom, value);
                    OnPropertyChanged(nameof(PositionFacilityFromList));
                }
            }
        }

        ACAccessNav<Facility> _AccessPositionFacilityFrom;
        /// <summary>
        /// Gets the access object for filtering source facilities in the picking order interface when selecting position facilities.
        /// Provides navigation and query functionality for Facility entities used as source locations for picking positions.
        /// The access object is configured with default filter and sort columns for facility filtering
        /// and automatically performs a search when first accessed to populate the available source facilities.
        /// Used in conjunction with the PositionFacilityFromList property to provide facility selection capabilities
        /// for picking position source locations.
        /// </summary>
        [ACPropertyAccess(815, "PositionFacilityFrom",
                          Description = @"Gets the access object for filtering source facilities in the picking order interface when selecting position facilities.
                                          Provides navigation and query functionality for Facility entities used as source locations for picking positions.
                                          The access object is configured with default filter and sort columns for facility filtering
                                          and automatically performs a search when first accessed to populate the available source facilities.
                                          Used in conjunction with the PositionFacilityFromList property to provide facility selection capabilities
                                          for picking position source locations.")]
        public ACAccessNav<Facility> AccessPositionFacilityFrom
        {
            get
            {
                if (_AccessPositionFacilityFrom == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(this, Const.QueryPrefix + "PositionFacilityFrom", ACType.ACIdentifier);
                    _AccessPositionFacilityFrom = navACQueryDefinition.NewAccessNav<Facility>("PositionFacilityFrom", this);
                    _AccessPositionFacilityFrom.AutoSaveOnNavigation = false;
                    RefreshFilterFacilityAccess(_AccessPositionFacilityFrom, FilterPositionFacilityFrom);
                }
                return _AccessBookingFacilityTarget;
            }
        }

        /// <summary>
        /// Gets the list of available facilities for use as source locations in picking positions.
        /// This property manages the list of facilities that can be selected as "FromFacility" for picking operations.
        /// The property performs the following operations:
        /// - Ensures the current picking position's FromFacility is included in the list even if filtered out
        /// - Automatically includes all child facilities of facilities in the navigation list
        /// - Returns the complete navigation list from AccessPositionFacilityFrom for facility selection
        /// The list is filtered based on the FilterPositionFacilityFrom setting which can show only facilities
        /// with material content or all available facilities depending on the filter configuration.
        /// </summary>
        [ACPropertyList(817, "PositionFacilityFrom",
                        Description = @"Gets the list of available facilities for use as source locations in picking positions.
                                        This property manages the list of facilities that can be selected as ""FromFacility"" for picking operations.
                                        The property performs the following operations:
                                        - Ensures the current picking position's FromFacility is included in the list even if filtered out
                                        - Automatically includes all child facilities of facilities in the navigation list
                                        - Returns the complete navigation list from AccessPositionFacilityFrom for facility selection
                                        The list is filtered based on the FilterPositionFacilityFrom setting which can show only facilities
                                        with material content or all available facilities depending on the filter configuration.")]
        public IList<Facility> PositionFacilityFromList
        {
            get
            {
                // Manage case if CurrentPickingPos.FromFacility is not in filtered set:
                // if not add to list to display value and prevent delete initial value
                if (CurrentPickingPos != null && CurrentPickingPos.FromFacility != null)
                {
                    if (!AccessPositionFacilityFrom?.NavList.Contains(CurrentPickingPos.FromFacility) ?? false)
                    {
                        AccessPositionFacilityFrom?.NavList.Add(CurrentPickingPos.FromFacility);
                    }
                }

                if (AccessPositionFacilityFrom != null && AccessPositionFacilityFrom.NavList != null)
                {
                    // add child facilities
                    foreach (Facility facility in AccessPositionFacilityFrom?.NavList)
                    {
                        AddChidFacilityToPosFacilityFormList(facility);
                    }
                }

                // Order list

                return AccessPositionFacilityFrom?.NavList;
            }
        }

        private void AddChidFacilityToPosFacilityFormList(Facility facility)
        {
            if (!AccessPositionFacilityFrom?.NavList.Contains(facility) ?? false)
            {
                AccessPositionFacilityFrom?.NavList.Add(facility);
            }
            if (facility.Facility_ParentFacility.Any())
            {
                foreach (Facility childFacility in facility.Facility_ParentFacility)
                {
                    AddChidFacilityToPosFacilityFormList(childFacility);
                }
            }
        }

        #endregion

        ACAccess<PickingPos> _AccessPickingPos;
        /// <summary>
        /// Gets the access object for picking position data management.
        /// This property provides data access functionality for PickingPos entities, enabling
        /// CRUD operations and query execution for picking position records. The access object
        /// is initialized on first use with an ACQueryDefinition derived from the primary
        /// navigation query definition, creating a non-navigational access instance for
        /// picking position data manipulation within the picking order management workflow.
        /// </summary>
        [ACPropertyAccess(691, "PickingPos",
                          Description = @"Gets the access object for picking position data management.
                                          This property provides data access functionality for PickingPos entities, enabling
                                          CRUD operations and query execution for picking position records. The access object
                                          is initialized on first use with an ACQueryDefinition derived from the primary
                                          navigation query definition, creating a non-navigational access instance for
                                          picking position data manipulation within the picking order management workflow.")]
        public ACAccess<PickingPos> AccessPickingPos
        {
            get
            {
                if (_AccessPickingPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + "PickingPos") as ACQueryDefinition;
                    _AccessPickingPos = acQueryDefinition.NewAccess<PickingPos>("PickingPos", this);
                }
                return _AccessPickingPos;
            }
        }

        PickingPos _CurrentPickingPos;
        /// <summary>
        /// Gets or sets the currently selected picking position for editing and display operations.
        /// This property serves as the primary interface for accessing and modifying the active picking position
        /// within the picking order workflow. When set, it triggers various operations including refreshing
        /// facility booking data, updating related entity collections, and configuring UI state.
        /// The setter performs the following operations when a new picking position is assigned:
        /// - Refreshes picking position data from the database for existing entities
        /// - Auto-loads and refreshes related InOrderPos/OutOrderPos facility booking and pre-booking collections
        /// - Auto-loads and refreshes picking position facility booking and pre-booking collections  
        /// - Updates various property change notifications for UI synchronization
        /// - Configures facility reservation settings and filter access objects
        /// - Sets up property change event handlers for the new picking position
        /// - Updates the current measurement unit based on the picking position's unit
        /// - Assigns the picking position as the facility reservation owner for batch planning
        /// This property is essential for coordinating facility operations, booking processes, and UI state
        /// management throughout the picking position lifecycle.
        /// </summary>
        [ACPropertyCurrent(604, "PickingPos",
                           Description = @"Gets or sets the currently selected picking position for editing and display operations.
                                           This property serves as the primary interface for accessing and modifying the active picking position
                                           within the picking order workflow. When set, it triggers various operations including refreshing
                                           facility booking data, updating related entity collections, and configuring UI state.
                                           The setter performs the following operations when a new picking position is assigned:
                                           - Refreshes picking position data from the database for existing entities
                                           - Auto-loads and refreshes related InOrderPos/OutOrderPos facility booking and pre-booking collections
                                           - Auto-loads and refreshes picking position facility booking and pre-booking collections  
                                           - Updates various property change notifications for UI synchronization
                                           - Configures facility reservation settings and filter access objects
                                           - Sets up property change event handlers for the new picking position
                                           - Updates the current measurement unit based on the picking position's unit
                                           - Assigns the picking position as the facility reservation owner for batch planning
                                           This property is essential for coordinating facility operations, booking processes, and UI state
                                           management throughout the picking position lifecycle.")]
        public PickingPos CurrentPickingPos
        {
            get
            {
                return _CurrentPickingPos;
            }
            set
            {
                if (_CurrentPickingPos != value)
                {
                    _CurrentPickingPos = value;
                    if (_CurrentPickingPos != null && _CurrentPickingPos.EntityState != EntityState.Added)
                    {
                        _CurrentPickingPos.AutoRefresh();
                        if (_CurrentPickingPos.InOrderPos != null)
                        {
                            _CurrentPickingPos.InOrderPos.AutoRefresh();
                            _CurrentPickingPos.InOrderPos.FacilityPreBooking_InOrderPos.AutoLoad(_CurrentPickingPos.InOrderPos.FacilityPreBooking_InOrderPosReference, _CurrentPickingPos);
                        }
                        else if (_CurrentPickingPos.OutOrderPos != null)
                        {
                            _CurrentPickingPos.OutOrderPos.AutoRefresh();
                            _CurrentPickingPos.OutOrderPos.FacilityPreBooking_OutOrderPos.AutoLoad(_CurrentPickingPos.OutOrderPos.FacilityPreBooking_OutOrderPosReference, _CurrentPickingPos);
                        }
                        if (_CurrentPickingPos.FacilityPreBooking_PickingPos != null && _CurrentPickingPos.FacilityPreBooking_PickingPos.Any())
                            _CurrentPickingPos.FacilityPreBooking_PickingPos.AutoLoad(_CurrentPickingPos.FacilityPreBooking_PickingPosReference, _CurrentPickingPos);

                        if (_CurrentPickingPos.FacilityBooking_PickingPos != null)
                            _CurrentPickingPos.FacilityBooking_PickingPos.AutoLoad(_CurrentPickingPos.FacilityBooking_PickingPosReference, _CurrentPickingPos);
                    }
                    OnPropertyChanged(nameof(CurrentPickingPos));
                    OnPropertyChanged(nameof(FacilityPreBookingList));
                    OnPropertyChanged(nameof(FacilityBookingList));
                    if (FacilityPreBookingList != null && FacilityPreBookingList.Any())
                        SelectedFacilityPreBooking = FacilityPreBookingList.First();
                    else
                        SelectedFacilityPreBooking = null;
                    RefreshBookingFilterFacilityAccess(AccessBookingFacility, BookingFilterMaterial);
                    RefreshBookingFilterFacilityAccess(AccessBookingFacilityTarget, BookingFilterMaterialTarget);

                    RefreshFilterFacilityAccess(AccessPositionFacilityFrom, FilterPositionFacilityFrom);
                    OnPropertyChanged(nameof(PositionFacilityFromList));

                    if (AccessBookingFacilityLot != null)
                        RefreshFilterFacilityLotAccess(_AccessBookingFacilityLot);
                    OnPropertyChanged(nameof(BookingFacilityList));
                    OnPropertyChanged(nameof(BookingFacilityListTarget));

                    if (CurrentPickingPos != null)
                        CurrentPickingPos.PropertyChanged += CurrentPickingPos_PropertyChanged;

                    CurrentMDUnit = CurrentPickingPos?.MDUnit;

                    if (BSOFacilityReservation_Child != null && BSOFacilityReservation_Child.Value != null)
                    {
                        BSOFacilityReservation_Child.Value.FacilityReservationOwner = value;
                    }
                }
            }
        }

        private void CurrentPickingPos_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentPickingPos.PickingMaterialID):
                    OnPropertyChanged(nameof(MDUnitList));
                    RefreshFilterFacilityAccess(AccessPositionFacilityFrom, FilterPositionFacilityFrom);
                    OnPropertyChanged(nameof(PositionFacilityFromList));
                    break;
            }
        }

        /// <summary>
        /// Gets the list of picking positions (lines) for the current picking order.
        /// Returns an ordered collection of PickingPos entities sorted by their sequence number.
        /// Returns null if no current picking order is selected.
        /// This property provides access to all line items within the picking order,
        /// including their materials, quantities, source/target facilities, and status information.
        /// </summary>
        [ACPropertyList(605, "PickingPos",
                        Description = @"Gets the list of picking positions (lines) for the current picking order.
                                        Returns an ordered collection of PickingPos entities sorted by their sequence number.
                                        Returns null if no current picking order is selected.
                                        This property provides access to all line items within the picking order,
                                        including their materials, quantities, source/target facilities, and status information.")]
        public IEnumerable<PickingPos> PickingPosList
        {
            get
            {
                if (CurrentPicking == null)
                    return null;
                //return AccessPickingPos.NavList;
                return CurrentPicking.PickingPos_Picking.OrderBy(c => c.Sequence);
                //return DatabaseApp.PickingPos.Include(c => c.InOrderPos)
                //    .Include(c => c.OutOrderPos)
                //    .Include(c => c.ProdOrderPartslistPos)
                //    .Where(c => c.PickingID == CurrentPicking.PickingID).AutoMergeOption();
            }
        }

        PickingPos _SelectedPickingPos;
        /// <summary>
        /// Gets or sets the currently selected picking position from the list for UI selection and editing operations.
        /// This property represents the picking position that is highlighted or selected in the user interface,
        /// typically in a list or grid control. When set, it triggers property change notifications, refreshes
        /// the weighing list, and automatically updates the CurrentPickingPos property to maintain synchronization
        /// between the selected and current picking positions for consistent UI behavior and data management.
        /// </summary>
        [ACPropertySelected(606, "PickingPos",
                            Description = @"Gets or sets the currently selected picking position from the list for UI selection and editing operations.
                                            This property represents the picking position that is highlighted or selected in the user interface,
                                            typically in a list or grid control. When set, it triggers property change notifications, refreshes
                                            the weighing list, and automatically updates the CurrentPickingPos property to maintain synchronization
                                            between the selected and current picking positions for consistent UI behavior and data management.")]
        public PickingPos SelectedPickingPos
        {
            get
            {
                return _SelectedPickingPos;
            }
            set
            {
                bool changed = _SelectedPickingPos != value;
                _SelectedPickingPos = value;
                if (changed)
                {
                    OnPropertyChanged(nameof(SelectedPickingPos));
                    RefreshWeighingList(true);
                }

                CurrentPickingPos = value;
            }
        }


        #region Properties -> InOrderPos -> MDUnit

        /// <summary>
        /// Gets the list of available measurement units for the current picking position material.
        /// Returns the material unit list based on the material associated with the current picking position,
        /// falling back to related order positions if no direct material is assigned.
        /// This property provides unit selection options for quantity input and conversion operations.
        /// </summary>
        [ACPropertyList(603, MDUnit.ClassName,
                        Description = @"Gets the list of available measurement units for the current picking position material.
                                        Returns the material unit list based on the material associated with the current picking position,
                                        falling back to related order positions if no direct material is assigned.
                                        This property provides unit selection options for quantity input and conversion operations.")]
        public IEnumerable<MDUnit> MDUnitList
        {
            get
            {
                if (CurrentPickingPos == null)
                    return null;
                if (CurrentPickingPos.Material != null)
                    return CurrentPickingPos.Material.MDUnitList;
                if (CurrentInOrderPos != null && CurrentInOrderPos.Material != null)
                    return CurrentInOrderPos.Material.MDUnitList;
                if (CurrentOutOrderPos != null && CurrentOutOrderPos.Material != null)
                    return CurrentOutOrderPos.Material.MDUnitList;
                if (CurrentProdOrderPartslistPos != null && CurrentProdOrderPartslistPos.Material != null)
                    return CurrentProdOrderPartslistPos.Material.MDUnitList;
                return null;
            }
        }

        MDUnit _CurrentMDUnit;
        /// <summary>
        /// Gets or sets the current measurement unit for the picking position.
        /// This property represents the unit of measurement (UOM) that is currently selected
        /// for the picking position material quantities. When set, it automatically updates
        /// the MDUnit property of the current picking position if they differ, ensuring
        /// consistency between the UI selection and the data model.
        /// </summary>
        [ACPropertyCurrent(604, MDUnit.ClassName, "en{'New Unit'}de{'Neue Einheit'}",
                           Description = @"Gets or sets the current measurement unit for the picking position.
                                           This property represents the unit of measurement (UOM) that is currently selected
                                           for the picking position material quantities. When set, it automatically updates
                                           the MDUnit property of the current picking position if they differ, ensuring
                                           consistency between the UI selection and the data model.")]
        public MDUnit CurrentMDUnit
        {
            get
            {
                return _CurrentMDUnit;
            }
            set
            {
                _CurrentMDUnit = value;
                if (CurrentPickingPos != null && CurrentPickingPos.MDUnit != value)
                {
                    CurrentPickingPos.MDUnit = value;
                    OnPropertyChanged(nameof(CurrentPickingPos));
                }
                OnPropertyChanged(nameof(CurrentMDUnit));
            }
        }

        #endregion
        #endregion

        #region FacilityPreBooking

        /// <summary>
        /// Enumeration defining the location context for facility selection operations.
        /// Used to specify which type of facility operation is being performed when
        /// showing facility selection dialogs, enabling proper context-aware handling
        /// of facility assignments in picking and pre-booking workflows.
        /// </summary>
        public FacilitySelectLoctation FacilitySelectLoctation { get; set; }


        #region FacilityPreBooking -> PreBooking
        FacilityPreBooking _CurrentFacilityPreBooking;
        /// <summary>
        /// Gets or sets the currently selected facility pre-booking for editing and display operations.
        /// This property serves as the primary interface for accessing and modifying the active facility pre-booking
        /// within the picking order workflow. When set, it triggers various operations including refreshing
        /// facility booking data, updating related entity collections, and configuring UI state.
        /// The setter performs the following operations when a new facility pre-booking is assigned:
        /// - Detaches property change event handlers from the previous pre-booking's ACMethodBooking
        /// - Updates the current facility pre-booking reference
        /// - Triggers property change notifications for related UI elements
        /// - Refreshes facility booking filter access objects for both source and target facilities
        /// - Updates facility lot filter access if available
        /// - Attaches property change event handlers to the new pre-booking's ACMethodBooking
        /// - Updates facility booking list properties for UI synchronization
        /// This property is essential for coordinating facility pre-booking operations, managing ACMethodBooking
        /// configurations, and ensuring proper UI state management throughout the facility pre-booking lifecycle.
        /// </summary>
        [ACPropertyCurrent(607, "FacilityPreBooking",
                           Description = @"Gets or sets the currently selected facility pre-booking for editing and display operations.
                                           This property serves as the primary interface for accessing and modifying the active facility pre-booking
                                           within the picking order workflow. When set, it triggers various operations including refreshing
                                           facility booking data, updating related entity collections, and configuring UI state.
                                           The setter performs the following operations when a new facility pre-booking is assigned:
                                           - Detaches property change event handlers from the previous pre-booking's ACMethodBooking
                                           - Updates the current facility pre-booking reference
                                           - Triggers property change notifications for related UI elements
                                           - Refreshes facility booking filter access objects for both source and target facilities
                                           - Updates facility lot filter access if available
                                           - Attaches property change event handlers to the new pre-booking's ACMethodBooking
                                           - Updates facility booking list properties for UI synchronization
                                           This property is essential for coordinating facility pre-booking operations, managing ACMethodBooking
                                           configurations, and ensuring proper UI state management throughout the facility pre-booking lifecycle.")]
        public FacilityPreBooking CurrentFacilityPreBooking
        {
            get
            {
                return _CurrentFacilityPreBooking;
            }
            set
            {
                if (_CurrentFacilityPreBooking != value)
                {
                    if (_CurrentFacilityPreBooking != null && _CurrentFacilityPreBooking.ACMethodBooking != null)
                        _CurrentFacilityPreBooking.ACMethodBooking.PropertyChanged -= ACMethodBooking_PropertyChanged;

                    _CurrentFacilityPreBooking = value;
                    OnPropertyChanged(nameof(CurrentFacilityPreBooking));
                    OnPropertyChanged(nameof(CurrentACMethodBooking));
                    OnPropertyChanged(nameof(CurrentACMethodBookingLayout));
                    //OnPropertyChanged(nameof(BookableFacilityLots));
                    RefreshBookingFilterFacilityAccess(AccessBookingFacility, BookingFilterMaterial);
                    RefreshBookingFilterFacilityAccess(AccessBookingFacilityTarget, BookingFilterMaterialTarget);
                    if (AccessBookingFacilityLot != null)
                        RefreshFilterFacilityLotAccess(_AccessBookingFacilityLot);

                    if (_CurrentFacilityPreBooking != null && _CurrentFacilityPreBooking.ACMethodBooking != null)
                        _CurrentFacilityPreBooking.ACMethodBooking.PropertyChanged += ACMethodBooking_PropertyChanged;

                    OnPropertyChanged(nameof(BookingFacilityList));
                    OnPropertyChanged(nameof(BookingFacilityListTarget));
                }
            }
        }

        /// <summary>
        /// Gets the list of facility pre-bookings associated with the current picking position.
        /// Returns different collections based on the picking type and position context:
        /// - For Receipt/ReceiptVehicle types: Returns pre-bookings from the associated InOrderPos
        /// - For Issue/IssueVehicle types: Returns pre-bookings from the associated OutOrderPos  
        /// - For other types: Returns pre-bookings directly from the PickingPos
        /// Returns null if no current picking position or picking type is available.
        /// </summary>
        [ACPropertyList(608, "FacilityPreBooking",
                        Description = @"Gets the list of facility pre-bookings associated with the current picking position.
                                        Returns different collections based on the picking type and position context:
                                        - For Receipt/ReceiptVehicle types: Returns pre-bookings from the associated InOrderPos
                                        - For Issue/IssueVehicle types: Returns pre-bookings from the associated OutOrderPos  
                                        - For other types: Returns pre-bookings directly from the PickingPos
                                        Returns null if no current picking position or picking type is available.")]
        public IEnumerable<FacilityPreBooking> FacilityPreBookingList
        {
            get
            {
                if (CurrentPickingPos == null || CurrentPicking.MDPickingType == null)
                    return null;
                if ((CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.Receipt
                  || CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.ReceiptVehicle)
                    && CurrentPickingPos.InOrderPos != null)
                {
                    return CurrentPickingPos.InOrderPos.FacilityPreBooking_InOrderPos.ToList();
                }
                else if ((CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.Issue
                       || CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.IssueVehicle)
                    && (CurrentPickingPos.OutOrderPos != null))
                {
                    return CurrentPickingPos.OutOrderPos.FacilityPreBooking_OutOrderPos.ToList();
                }
                else if (CurrentPickingPos.FacilityPreBooking_PickingPos != null)
                {
                    return CurrentPickingPos.FacilityPreBooking_PickingPos.ToList();
                    //if (CurrentPickingPos.ProdOrderPartslistPos == null)
                    //    return null;
                    //return CurrentPickingPos.ProdOrderPartslistPos.FacilityPreBooking_ProdOrderPartslistPos.ToList();
                }
                else
                {
                    return null;
                }
                //return null;
            }
        }

        FacilityPreBooking _SelectedFacilityPreBooking;
        /// <summary>
        /// Gets or sets the currently selected facility pre-booking for user interface selection purposes.
        /// This property represents the facility pre-booking that is highlighted or selected in the user interface,
        /// typically in a list or grid control. When set, it triggers various operations including:
        /// - Setting the CurrentFacilityPreBooking to the selected value for detailed editing
        /// - Refreshing facility booking lists for both source and target facilities
        /// - Updating outward and inward facility charge lists based on the new selection
        /// - Performing a navigation search on facility lots if the AccessBookingFacilityLot is available
        /// This property works in conjunction with CurrentFacilityPreBooking to maintain synchronization
        /// between the selected item in lists and the active item being edited or displayed in detail views.
        /// </summary>
        [ACPropertySelected(609, "FacilityPreBooking",
                            Description = @"Gets or sets the currently selected facility pre-booking for user interface selection purposes.
                                            This property represents the facility pre-booking that is highlighted or selected in the user interface,
                                            typically in a list or grid control. When set, it triggers various operations including:
                                            - Setting the CurrentFacilityPreBooking to the selected value for detailed editing
                                            - Refreshing facility booking lists for both source and target facilities
                                            - Updating outward and inward facility charge lists based on the new selection
                                            - Performing a navigation search on facility lots if the AccessBookingFacilityLot is available
                                            This property works in conjunction with CurrentFacilityPreBooking to maintain synchronization
                                            between the selected item in lists and the active item being edited or displayed in detail views.")]
        public FacilityPreBooking SelectedFacilityPreBooking
        {
            get
            {
                return _SelectedFacilityPreBooking;
            }
            set
            {
                _SelectedFacilityPreBooking = value;
                OnPropertyChanged(nameof(SelectedFacilityPreBooking));
                CurrentFacilityPreBooking = value;
                OnPropertyChanged(nameof(BookingFacilityList));
                OnPropertyChanged(nameof(OutwardFacilityChargeList));
                OnPropertyChanged(nameof(InwardFacilityChargeList));
                //OnPropertyChanged(nameof(BookableFacilityLots));
                if (AccessBookingFacilityLot != null)
                    AccessBookingFacilityLot.NavSearch(DatabaseApp);
            }
        }

        ACMethodBooking _CurrentACMethodBookingDummy = null; // Dummy-Parameter notwendig, damit Bindung an Oberfläche erfolgen kann, da abgeleitete Klasse

        /// <summary>
        /// Gets or sets the current ACMethodBooking for facility booking operations.
        /// This property manages the booking parameters for facility transactions within the picking workflow.
        /// When no CurrentFacilityPreBooking is available, returns a dummy ACMethodBooking instance
        /// created from the PickingManager's inward movement template for UI binding purposes.
        /// When CurrentFacilityPreBooking exists, returns and manages its associated ACMethodBooking,
        /// including automatic event handler attachment/detachment for property change notifications
        /// to keep the UI synchronized with booking parameter changes.
        /// The setter handles the association of booking parameters with facility pre-bookings and
        /// ensures proper event handler management for real-time UI updates when booking properties change.
        /// </summary>
        [ACPropertyInfo(610, "", "en{'Posting Parameter'}de{'Buchungsparameter'}",
                        Description = @"Gets or sets the current ACMethodBooking for facility booking operations.
                                        This property manages the booking parameters for facility transactions within the picking workflow.
                                        When no CurrentFacilityPreBooking is available, returns a dummy ACMethodBooking instance
                                        created from the PickingManager's inward movement template for UI binding purposes.
                                        When CurrentFacilityPreBooking exists, returns and manages its associated ACMethodBooking,
                                        including automatic event handler attachment/detachment for property change notifications
                                        to keep the UI synchronized with booking parameter changes.
                                        The setter handles the association of booking parameters with facility pre-bookings and
                                        ensures proper event handler management for real-time UI updates when booking properties change.")]
        public ACMethodBooking CurrentACMethodBooking
        {
            get
            {
                if (CurrentFacilityPreBooking == null)
                {
                    if (_CurrentACMethodBookingDummy != null)
                        return _CurrentACMethodBookingDummy;
                    if (this.PickingManager == null)
                        return null;
                    ACMethodBooking acMethodClone = ACFacilityManager.BookParamInOrderPosInwardMovementClone(DatabaseApp);
                    if (acMethodClone != null)
                        _CurrentACMethodBookingDummy = acMethodClone.Clone() as ACMethodBooking;
                    return _CurrentACMethodBookingDummy;
                }
                _CurrentACMethodBookingDummy = null;
                return CurrentFacilityPreBooking?.ACMethodBooking as ACMethodBooking;
            }
            set
            {
                if (CurrentFacilityPreBooking != null)
                {
                    if (CurrentFacilityPreBooking.ACMethodBooking != null)
                        CurrentFacilityPreBooking.ACMethodBooking.PropertyChanged -= ACMethodBooking_PropertyChanged;
                    CurrentFacilityPreBooking.ACMethodBooking = value;
                    if (CurrentFacilityPreBooking.ACMethodBooking != null)
                        CurrentFacilityPreBooking.ACMethodBooking.PropertyChanged += ACMethodBooking_PropertyChanged; ;
                }
                else
                    _CurrentACMethodBookingDummy = null;
                OnPropertyChanged(nameof(CurrentACMethodBooking));
                OnPropertyChanged(nameof(CurrentACMethodBookingLayout));
            }
        }

        bool _UpdatingControlModeBooking = false;
        private void ACMethodBooking_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_UpdatingControlModeBooking)
                return;
            try
            {
                if (e.PropertyName == nameof(ACMethodBooking.OutwardFacility))
                {
                    _UpdatingControlModeBooking = true;
                    OnPropertyChanged(nameof(OutwardFacilityChargeList));
                    OnPropertyChanged(nameof(BookableFacilityLots));
                    if (CurrentACMethodBooking != null)
                    {
                        CurrentACMethodBooking.OnEntityPropertyChanged(nameof(ACMethodBooking.OutwardFacility));
                        CurrentACMethodBooking.OnEntityPropertyChanged(nameof(ACMethodBooking.OutwardFacilityCharge));
                        CurrentACMethodBooking.OnEntityPropertyChanged(nameof(ACMethodBooking.OutwardFacilityLot));
                    }
                }
                else if (e.PropertyName == nameof(ACMethodBooking.InwardFacility))
                {
                    _UpdatingControlModeBooking = true;
                    OnPropertyChanged(nameof(InwardFacilityChargeList));
                    OnPropertyChanged(nameof(BookableFacilityLots));
                    if (CurrentACMethodBooking != null)
                    {
                        CurrentACMethodBooking.OnEntityPropertyChanged(nameof(ACMethodBooking.InwardFacility));
                        CurrentACMethodBooking.OnEntityPropertyChanged(nameof(ACMethodBooking.InwardFacilityCharge));
                        CurrentACMethodBooking.OnEntityPropertyChanged(nameof(ACMethodBooking.InwardFacilityLot));
                    }
                }
            }
            finally
            {
                _UpdatingControlModeBooking = false;
            }
        }

        /// <summary>
        /// Gets the current ACMethodBooking layout XAML for display in the user interface.
        /// Determines the appropriate design layout based on the booking type and order context:
        /// - For inward bookings (receipts, picking inward): Returns "BookingInward" layout
        /// - For outward bookings (issues, picking outward): Returns "BookingOutward" layout  
        /// - For other booking types (relocations): Returns "BookingRelocation" layout
        /// If no specific design is found, returns a default VBDockPanel XAML structure.
        /// </summary>
        [ACPropertyInfo(611, "",
                        Description = @"Gets the current ACMethodBooking layout XAML for display in the user interface.
                                        Determines the appropriate design layout based on the booking type and order context:
                                        - For inward bookings (receipts, picking inward): Returns ""BookingInward"" layout
                                        - For outward bookings (issues, picking outward): Returns ""BookingOutward"" layout  
                                        - For other booking types (relocations): Returns ""BookingRelocation"" layout
                                        If no specific design is found, returns a default VBDockPanel XAML structure.")]
        public string CurrentACMethodBookingLayout
        {
            get
            {
                gip.core.datamodel.ACClassDesign acClassDesign = null;
                if (ACType != null)
                {
                    if ((CurrentACMethodBooking != null)
                        && (CurrentACMethodBooking.InOrderPos != null
                            || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosInwardMovement
                            || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel
                            || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.PickingInward
                            ))
                        acClassDesign = ACType.GetDesign(this, Global.ACUsages.DULayout, Global.ACKinds.DSDesignLayout, "BookingInward");
                    else if ((CurrentACMethodBooking != null)
                        && (CurrentACMethodBooking.OutOrderPos != null
                        || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.OutOrderPosOutwardMovement
                        || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel
                        || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.PickingOutward
                        ))
                        acClassDesign = ACType.GetDesign(this, Global.ACUsages.DULayout, Global.ACKinds.DSDesignLayout, "BookingOutward");
                    else
                        acClassDesign = ACType.GetDesign(this, Global.ACUsages.DULayout, Global.ACKinds.DSDesignLayout, "BookingRelocation");
                }

                string layoutXAML = "<vb:VBDockPanel></vb:VBDockPanel>";
                if (acClassDesign != null)
                    layoutXAML = acClassDesign.XMLDesign;

                return layoutXAML;
            }
        }

        private bool _BookingFilterMaterial = false;
        /// <summary>
        /// Filter property for controlling the display of source facilities with material content in the booking interface.
        /// When set to true, only shows facilities that currently contain material (non-empty facilities) for booking operations.
        /// When set to false, shows all facilities regardless of material content.
        /// This filter helps users focus on relevant facilities during facility booking operations by hiding empty storage locations,
        /// improving the user experience when selecting source facilities for material movements and bookings.
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Only show source bins with material'}de{'Zeige Quell-Lagerplätze mit Material'}",
                        Description = @"Filter property for controlling the display of source facilities with material content in the booking interface.
                                        When set to true, only shows facilities that currently contain material (non-empty facilities) for booking operations.
                                        When set to false, shows all facilities regardless of material content.
                                        This filter helps users focus on relevant facilities during facility booking operations by hiding empty storage locations,
                                        improving the user experience when selecting source facilities for material movements and bookings.")]
        public bool BookingFilterMaterial
        {
            get
            {
                return _BookingFilterMaterial;
            }
            set
            {
                _BookingFilterMaterial = value;
                OnPropertyChanged(nameof(BookingFilterMaterial));
                RefreshBookingFilterFacilityAccess(AccessBookingFacility, value);
                OnPropertyChanged(nameof(BookingFacilityList));
                OnPropertyChanged(nameof(OutwardFacilityChargeList));
                OnPropertyChanged(nameof(InwardFacilityChargeList));
                //OnPropertyChanged(nameof(BookableFacilityLots));
            }
        }

        /// <summary>
        /// Gets a list of outward facility charges for the current picking position.
        /// Returns facility charges from the outward facility that match the current picking position's material
        /// and are available (not marked as NotAvailable). The results are ordered by insertion date in descending order.
        /// Used for selecting specific facility charges when performing outward material movements in picking operations.
        /// </summary>
        [ACPropertyList(612, "OutwardFacilityCharge",
                        Description = @"Gets a list of outward facility charges for the current picking position.
                                        Returns facility charges from the outward facility that match the current picking position's material
                                        and are available (not marked as NotAvailable). The results are ordered by insertion date in descending order.
                                        Used for selecting specific facility charges when performing outward material movements in picking operations.")]
        public IEnumerable<FacilityCharge> OutwardFacilityChargeList
        {
            get
            {
                if (CurrentACMethodBooking == null || CurrentACMethodBooking.OutwardFacility == null || CurrentPickingPos.Material == null)
                    return null;
                return CurrentACMethodBooking.OutwardFacility.FacilityCharge_Facility
                    .Where(x => x.MaterialID == CurrentPickingPos.Material.MaterialID && !x.NotAvailable).OrderByDescending(x => x.InsertDate);
            }
        }

        /// <summary>
        /// Gets a list of inward facility charges for the current picking position.
        /// Returns facility charges from the inward facility that match the current picking position's material
        /// and are available (not marked as NotAvailable). The results are ordered by insertion date in descending order.
        /// Used for selecting specific facility charges when performing inward material movements in picking operations.
        /// </summary>
        [ACPropertyList(612, "InwardFacilityCharge")]
        public IEnumerable<FacilityCharge> InwardFacilityChargeList
        {
            get
            {
                if (CurrentACMethodBooking == null || CurrentACMethodBooking.InwardFacility == null || CurrentPickingPos == null || CurrentPickingPos.Material == null)
                    return null;
                return CurrentACMethodBooking.InwardFacility.FacilityCharge_Facility
                    .Where(x => x.MaterialID == CurrentPickingPos.Material.MaterialID && !x.NotAvailable).OrderByDescending(x => x.InsertDate);
            }
        }

        ACAccessNav<Facility> _AccessBookingFacility;
        /// <summary>
        /// Gets the navigation access object for booking facilities in the picking order interface.
        /// Provides data access functionality for Facility entities used in booking operations,
        /// enabling CRUD operations and query execution for facility records within the picking workflow.
        /// The access object is initialized on first use with an ACQueryDefinition for "BookingFacility" queries,
        /// configured with AutoSaveOnNavigation disabled to prevent automatic saving during navigation.
        /// Automatically applies booking filter settings based on the BookingFilterMaterial property
        /// to show relevant facilities for the current booking context.
        /// </summary>
        [ACPropertyAccess(613, "BookingFacility",
                          Description = @"Gets the navigation access object for booking facilities in the picking order interface.
                                          Provides data access functionality for Facility entities used in booking operations,
                                          enabling CRUD operations and query execution for facility records within the picking workflow.
                                          The access object is initialized on first use with an ACQueryDefinition for ""BookingFacility"" queries,
                                          configured with AutoSaveOnNavigation disabled to prevent automatic saving during navigation.
                                          Automatically applies booking filter settings based on the BookingFilterMaterial property
                                          to show relevant facilities for the current booking context.")]
        public ACAccessNav<Facility> AccessBookingFacility
        {
            get
            {
                if (_AccessBookingFacility == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "BookingFacility", ACType.ACIdentifier);
                    _AccessBookingFacility = navACQueryDefinition.NewAccessNav<Facility>("BookingFacility", this);
                    _AccessBookingFacility.AutoSaveOnNavigation = false;
                    RefreshBookingFilterFacilityAccess(_AccessBookingFacility, BookingFilterMaterial);
                }
                return _AccessBookingFacility;
            }
        }

        private bool _BookingFilterMaterialTarget = true;
        /// <summary>
        /// Filter property for controlling the display of target facilities with material content in the booking interface.
        /// When set to true, only shows target facilities that currently contain material (non-empty facilities) for booking operations.
        /// When set to false, shows all target facilities regardless of material content.
        /// This filter helps users focus on relevant facilities during facility booking operations by hiding empty storage locations,
        /// improving the user experience when selecting target facilities for material movements and bookings.
        /// </summary>
        [ACPropertyInfo(614, "", "en{'Only show target bins with material'}de{'Zeige Ziel-Lagerplätze mit Material'}",
                        Description = @"Filter property for controlling the display of target facilities with material content in the booking interface.
                                        When set to true, only shows target facilities that currently contain material (non-empty facilities) for booking operations.
                                        When set to false, shows all target facilities regardless of material content.
                                        This filter helps users focus on relevant facilities during facility booking operations by hiding empty storage locations,
                                        improving the user experience when selecting target facilities for material movements and bookings.")]
        public bool BookingFilterMaterialTarget
        {
            get
            {
                return _BookingFilterMaterialTarget;
            }
            set
            {
                _BookingFilterMaterialTarget = value;
                OnPropertyChanged(nameof(BookingFilterMaterialTarget));
                RefreshBookingFilterFacilityAccess(AccessBookingFacilityTarget, value);
                OnPropertyChanged(nameof(BookingFacilityListTarget));
            }
        }

        ACAccessNav<Facility> _AccessBookingFacilityTarget;
        /// <summary>
        /// Gets the navigation access object for target booking facilities in the picking order interface.
        /// The access object is initialized on first use with an ACQueryDefinition for "BookingFacility" queries,
        /// configured with AutoSaveOnNavigation disabled to prevent automatic saving during navigation.
        /// Automatically applies booking filter settings based on the BookingFilterMaterialTarget property
        /// to show relevant target facilities for the current booking context.
        /// </summary>
        [ACPropertyAccess(615, "BookingFacilityTarget",
                          Description = @"Gets the navigation access object for target booking facilities in the picking order interface.
                                          The access object is initialized on first use with an ACQueryDefinition for ""BookingFacility"" queries,
                                          configured with AutoSaveOnNavigation disabled to prevent automatic saving during navigation.
                                          Automatically applies booking filter settings based on the BookingFilterMaterialTarget property
                                          to show relevant target facilities for the current booking context.")]
        public ACAccessNav<Facility> AccessBookingFacilityTarget
        {
            get
            {
                if (_AccessBookingFacilityTarget == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "BookingFacility", ACType.ACIdentifier);
                    _AccessBookingFacilityTarget = navACQueryDefinition.NewAccessNav<Facility>("BookingFacility", this);
                    _AccessBookingFacilityTarget.AutoSaveOnNavigation = false;
                    RefreshBookingFilterFacilityAccess(_AccessBookingFacilityTarget, BookingFilterMaterialTarget);
                }
                return _AccessBookingFacilityTarget;
            }
        }


        private List<ACFilterItem> AccessBookingFacilityDefaultFilter_Material
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)FacilityTypesEnum.StorageBinContainer).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, "Material\\MaterialNo", Global.LogicalOperators.equal, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, "Material\\MaterialName1", Global.LogicalOperators.equal, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true)
                };
            }
        }

        private List<ACFilterItem> AccessBookingFacilityDefaultFilter_StorageBin
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityNo", Global.LogicalOperators.equal, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityName", Global.LogicalOperators.contains, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)FacilityTypesEnum.StorageBin).ToString(), true)
                };
            }
        }

        private List<ACFilterItem> AccessBookingFacilityDefaultFilter_StorageAll
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityNo", Global.LogicalOperators.equal, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityName", Global.LogicalOperators.contains, Global.Operators.and, "", true),
                };
            }
        }

        private List<ACSortItem> AccessBookingFacilityDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("FacilityNo", Global.SortDirections.ascending, true),
                };
            }
        }

        /// <summary>
        /// Gets the list of available facilities for booking operations.
        /// This property provides access to the navigation list of Facility entities
        /// that can be used as source facilities in facility booking operations within the picking workflow.
        /// The list is populated through the AccessBookingFacility navigation object
        /// and is filtered based on the BookingFilterMaterial property to show relevant facilities
        /// for the current booking context. Returns null if no AccessBookingFacility is available.
        /// </summary>
        [ACPropertyList(616, "BookingFacility",
                        Description = @"Gets the list of available facilities for booking operations.
                                        This property provides access to the navigation list of Facility entities
                                        that can be used as source facilities in facility booking operations within the picking workflow.
                                        The list is populated through the AccessBookingFacility navigation object
                                        and is filtered based on the BookingFilterMaterial property to show relevant facilities
                                        for the current booking context. Returns null if no AccessBookingFacility is available.")]
        public IList<Facility> BookingFacilityList
        {
            get
            {
                return AccessBookingFacility?.NavList;
            }
        }

        /// <summary>
        /// Gets the list of available target facilities for booking operations.
        /// This property provides access to the navigation list of Facility entities
        /// that can be used as target facilities in facility booking operations within the picking workflow.
        /// The list is populated through the AccessBookingFacilityTarget navigation object
        /// and is filtered based on the BookingFilterMaterialTarget property to show relevant facilities
        /// for the current booking context. Returns null if no AccessBookingFacilityTarget is available.
        /// </summary>
        [ACPropertyList(617, "BookingFacilityTarget",
                        Description = @"Gets the list of available target facilities for booking operations.
                                        This property provides access to the navigation list of Facility entities
                                        that can be used as target facilities in facility booking operations within the picking workflow.
                                        The list is populated through the AccessBookingFacilityTarget navigation object
                                        and is filtered based on the BookingFilterMaterialTarget property to show relevant facilities
                                        for the current booking context. Returns null if no AccessBookingFacilityTarget is available.")]
        public IList<Facility> BookingFacilityListTarget
        {
            get
            {
                return AccessBookingFacilityTarget?.NavList;
            }
        }

        private void RefreshBookingFilterFacilityAccess(ACAccessNav<Facility> accessNavFacility, bool bookingFilterMaterial)
        {
            if (accessNavFacility == null
                || accessNavFacility.NavACQueryDefinition == null
                || CurrentFacilityPreBooking == null
                || FacilityPreBookingList == null
                || !FacilityPreBookingList.Any())
                return;
            RefreshFilterFacilityAccess(accessNavFacility, bookingFilterMaterial);
        }

        private void RefreshFilterFacilityAccess(ACAccessNav<Facility> accessNavFacility, bool? filterMaterial)
        {
            if (!filterMaterial.HasValue)
            {
                accessNavFacility.NavACQueryDefinition.CheckAndReplaceColumnsIfDifferent(AccessBookingFacilityDefaultFilter_StorageAll, AccessBookingFacilityDefaultSort);
            }
            else if (filterMaterial.Value)
            {
                accessNavFacility.NavACQueryDefinition.CheckAndReplaceColumnsIfDifferent(AccessBookingFacilityDefaultFilter_Material, AccessBookingFacilityDefaultSort);
                var acFilter = accessNavFacility.NavACQueryDefinition.ACFilterColumns.Where(c => c.ACIdentifier == "Material\\MaterialNo").FirstOrDefault();

                if (CurrentPickingPos != null && CurrentPickingPos.Material != null)
                {
                    if (acFilter != null)
                        acFilter.SearchWord = CurrentPickingPos.Material.MaterialNo;
                    var acFilter2 = accessNavFacility.NavACQueryDefinition.ACFilterColumns.Where(c => c.ACIdentifier == "Material\\MaterialNo" && c != acFilter).FirstOrDefault();
                    if (acFilter2 != null)
                        acFilter2.SearchWord = CurrentPickingPos.Material.Material1_ProductionMaterial != null ? CurrentPickingPos.Material.Material1_ProductionMaterial.MaterialNo : "";
                }
            }
            else
            {
                accessNavFacility.NavACQueryDefinition.CheckAndReplaceColumnsIfDifferent(AccessBookingFacilityDefaultFilter_StorageBin, AccessBookingFacilityDefaultSort);
            }
            accessNavFacility.NavSearch(this.DatabaseApp);
        }

        /// <summary>
        /// Gets the collection of facility lots that are available for booking operations.
        /// Returns the navigation list from the AccessBookingFacilityLot access object, which contains
        /// FacilityLot entities that can be selected during facility booking processes within the picking workflow.
        /// This property provides access to lot-managed inventory items for precise material tracking and booking operations.
        /// Returns null if the AccessBookingFacilityLot is not available or has not been initialized.
        /// </summary>
        [ACPropertyList(618, "FacilityLots",
                        Description = @"Gets the collection of facility lots that are available for booking operations.
                                        Returns the navigation list from the AccessBookingFacilityLot access object, which contains
                                        FacilityLot entities that can be selected during facility booking processes within the picking workflow.
                                        This property provides access to lot-managed inventory items for precise material tracking and booking operations.
                                        Returns null if the AccessBookingFacilityLot is not available or has not been initialized.")]
        public IEnumerable<FacilityLot> BookableFacilityLots
        {
            get
            {
                if (AccessBookingFacilityLot == null)
                    return null;

                return AccessBookingFacilityLot.NavList;
            }
        }

        ACAccessNav<FacilityLot> _AccessBookingFacilityLot;
        /// <summary>
        /// Gets the navigation access object for booking facility lots in the picking order interface.
        /// The access object is initialized on first use with an ACQueryDefinition for "BookingFacilityLot" queries,
        /// configured with a take count limit of 20 records, AutoSaveOnNavigation disabled to prevent automatic saving during navigation,
        /// and includes a search executed event handler to enhance facility lot results with bookable lots from pre-bookings and bookings.
        /// Automatically applies facility lot filter settings to show relevant lots for the current booking context.
        /// </summary>
        [ACPropertyAccess(613, "FacilityLots",
                          Description = @"Gets the navigation access object for booking facility lots in the picking order interface.
                                          The access object is initialized on first use with an ACQueryDefinition for ""BookingFacilityLot"" queries,
                                          configured with a take count limit of 20 records, AutoSaveOnNavigation disabled to prevent automatic saving during navigation,
                                          and includes a search executed event handler to enhance facility lot results with bookable lots from pre-bookings and bookings.
                                          Automatically applies facility lot filter settings to show relevant lots for the current booking context.")]
        public ACAccessNav<FacilityLot> AccessBookingFacilityLot
        {
            get
            {
                if (_AccessBookingFacilityLot == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "BookingFacilityLot", ACType.ACIdentifier);
                    navACQueryDefinition.TakeCount = 20;
                    _AccessBookingFacilityLot = navACQueryDefinition.NewAccessNav<FacilityLot>("BookingFacilityLot", this);
                    _AccessBookingFacilityLot.AutoSaveOnNavigation = false;
                    _AccessBookingFacilityLot.NavSearchExecuted += _AccessBookingFacilityLot_NavSearchExecuted;
                    RefreshFilterFacilityLotAccess(_AccessBookingFacilityLot);
                }
                return _AccessBookingFacilityLot;
            }
        }

        private void _AccessBookingFacilityLot_NavSearchExecuted(object sender, IList<FacilityLot> result)
        {
            List<FacilityLot> bookableFacilityLots = null;
            if (FacilityPreBookingList != null && FacilityPreBookingList.Any())
            {
                if (FacilityPreBookingList.Where(c => c.InwardFacilityLot != null).Any())
                    bookableFacilityLots = FacilityPreBookingList.Where(c => c.InwardFacilityLot != null).Select(c => c.InwardFacilityLot).Distinct().ToList();
                else
                    bookableFacilityLots = FacilityPreBookingList.Where(c => c.OutwardFacilityLot != null).Select(c => c.OutwardFacilityLot).Distinct().ToList();
            }

            if (FacilityBookingList != null && FacilityBookingList.Any())
            {
                IEnumerable<FacilityLot> query2 = null;
                if (FacilityBookingList.Where(c => c.InwardFacilityLot != null).Any())
                    query2 = FacilityBookingList.Where(c => c.InwardFacilityLot != null).Select(c => c.InwardFacilityLot).Distinct();
                else
                    query2 = FacilityBookingList.Where(c => c.OutwardFacilityLot != null).Select(c => c.OutwardFacilityLot).Distinct();
                if (bookableFacilityLots == null)
                    bookableFacilityLots = query2.ToList();
                else
                {
                    var query3 = bookableFacilityLots.Union(query2);
                    if ((query3 != null) && (query3.Any()))
                        bookableFacilityLots = query3.ToList();
                }
            }

            if (bookableFacilityLots != null && bookableFacilityLots.Any())
            {
                foreach (var lot in bookableFacilityLots)
                {
                    result.Insert(0, lot);
                }
            }
        }

        private List<ACFilterItem> AccessBookingFacilityLotDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "LotNo", Global.LogicalOperators.equal, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, "Material\\MaterialNo", Global.LogicalOperators.equal, Global.Operators.or, "", true),
                };
            }
        }

        private List<ACSortItem> AccessBookingFacilityLotDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("LotNo", Global.SortDirections.ascending, true),
                };
            }
        }


        private void RefreshFilterFacilityLotAccess(ACAccessNav<FacilityLot> accessNavLot)
        {
            if (accessNavLot == null
                || accessNavLot.NavACQueryDefinition == null
                || CurrentPickingPos == null)
                return;
            accessNavLot.NavACQueryDefinition.CheckAndReplaceColumnsIfDifferent(AccessBookingFacilityLotDefaultFilter, AccessBookingFacilityLotDefaultSort);

            var acFilter = accessNavLot.NavACQueryDefinition.ACFilterColumns.Where(c => c.ACIdentifier == "Material\\MaterialNo").FirstOrDefault();
            if (acFilter != null && CurrentPickingPos.Material != null)
                acFilter.SearchWord = CurrentPickingPos.Material.MaterialNo;

            accessNavLot.NavSearch(this.DatabaseApp);
        }
        #endregion

        #region  FacilityPreBooking -> Available quants

        private bool _IsInward;
        private Material _QuantDialogMaterial;

        private FacilityCharge _SelectedPreBookingAvailableQuants;
        /// <summary>
        /// Gets or sets the currently selected facility charge from the available pre-booking quantities list.
        /// This property is used to track which facility charge the user has selected when choosing
        /// quantities for pre-booking operations. The selected facility charge contains information
        /// about the material batch, quantity, and facility location that will be used for the booking.
        /// When changed, triggers property change notification to update bound UI controls.
        /// </summary>
        [ACPropertySelected(500, "PropertyGroupName", "en{'TODO: PreBookingAvailableQuants'}de{'TODO: PreBookingAvailableQuants'}",
                            Description = @"Gets or sets the currently selected facility charge from the available pre-booking quantities list.
                                            This property is used to track which facility charge the user has selected when choosing
                                            quantities for pre-booking operations. The selected facility charge contains information
                                            about the material batch, quantity, and facility location that will be used for the booking.
                                            When changed, triggers property change notification to update bound UI controls.")]
        public FacilityCharge SelectedPreBookingAvailableQuants
        {
            get
            {
                return _SelectedPreBookingAvailableQuants;
            }
            set
            {
                if (_SelectedPreBookingAvailableQuants != value)
                {
                    _SelectedPreBookingAvailableQuants = value;
                    OnPropertyChanged(nameof(SelectedPreBookingAvailableQuants));
                }
            }
        }

        private List<FacilityCharge> _PreBookingAvailableQuantsList;
        /// <summary>
        /// Gets the list of available facility charges for pre-booking operations.
        /// Returns facility charges from the database that match the current dialog material
        /// and are available (not marked as NotAvailable). The list is ordered by expiration date
        /// and filling date to prioritize older stock for FIFO inventory management.
        /// Used in facility selection dialogs to provide users with available quantities
        /// for material movements and booking operations.
        /// </summary>
        [ACPropertyList(501, "PropertyGroupName",
                        Description = @"Gets the list of available facility charges for pre-booking operations.
                                        Returns facility charges from the database that match the current dialog material
                                        and are available (not marked as NotAvailable). The list is ordered by expiration date
                                        and filling date to prioritize older stock for FIFO inventory management.
                                        Used in facility selection dialogs to provide users with available quantities
                                        for material movements and booking operations.")]
        public List<FacilityCharge> PreBookingAvailableQuantsList
        {
            get
            {
                if (_PreBookingAvailableQuantsList == null)
                    _PreBookingAvailableQuantsList = LoadPreBookingAvailableQuantsList();
                return _PreBookingAvailableQuantsList;
            }
        }

        private List<FacilityCharge> LoadPreBookingAvailableQuantsList()
        {
            if (_QuantDialogMaterial == null)
                return new List<FacilityCharge>();
            return
                DatabaseApp.FacilityCharge
                .Include(c => c.FacilityLot)
                .Include(c => c.Material)
                .Include(c => c.MDUnit)
                .Include(c => c.Facility)
                .Where(c => c.MaterialID == _QuantDialogMaterial.MaterialID && !c.NotAvailable)
                .OrderBy(c => c.ExpirationDate)
                .ThenBy(c => c.FillingDate)
                .Take(Root.Environment.AccessDefaultTakeCount)
                .ToList();
        }

        private void LoadPreBookingAvailableQuants()
        {
            _PreBookingAvailableQuantsList = null;
            OnPropertyChanged(nameof(PreBookingAvailableQuantsList));
        }

        #endregion

        #endregion

        #region FacilityBooking
        FacilityBooking _CurrentFacilityBooking;
        /// <summary>
        /// Gets or sets the currently selected facility booking for display and editing operations.
        /// This property represents the facility booking that has been selected from the FacilityBookingList
        /// for detailed viewing or modification. When set, it updates the current facility booking reference
        /// and triggers property change notifications to update bound UI controls.
        /// </summary>
        [ACPropertyCurrent(619, FacilityBooking.ClassName,
                           Description = @"Gets or sets the currently selected facility booking for display and editing operations.
                                           This property represents the facility booking that has been selected from the FacilityBookingList
                                           for detailed viewing or modification. When set, it updates the current facility booking reference
                                           and triggers property change notifications to update bound UI controls.")]
        public FacilityBooking CurrentFacilityBooking
        {
            get
            {
                return _CurrentFacilityBooking;
            }
            set
            {
                _CurrentFacilityBooking = value;
                OnPropertyChanged(nameof(CurrentFacilityBooking));
            }
        }

        /// <summary>
        /// Gets the list of facility bookings associated with the current picking position.
        /// Returns different collections based on the picking type and position context:
        /// - For Receipt/ReceiptVehicle types: Returns bookings from the associated InOrderPos
        /// - For Issue/IssueVehicle types: Returns bookings from the associated OutOrderPos  
        /// - For production-related positions: Returns bookings from associated production order positions
        /// - For other types: Returns bookings directly from the PickingPos
        /// The bookings are automatically loaded and refreshed from the database and ordered by facility booking number.
        /// Returns null if no current picking position, picking order, or if the position is in an invalid state.
        /// </summary>
        [ACPropertyList(620, FacilityBooking.ClassName,
                        Description = @"Gets the list of facility bookings associated with the current picking position.
                                        Returns different collections based on the picking type and position context:
                                        - For Receipt/ReceiptVehicle types: Returns bookings from the associated InOrderPos
                                        - For Issue/IssueVehicle types: Returns bookings from the associated OutOrderPos  
                                        - For production-related positions: Returns bookings from associated production order positions
                                        - For other types: Returns bookings directly from the PickingPos
                                        The bookings are automatically loaded and refreshed from the database and ordered by facility booking number.
                                        Returns null if no current picking position, picking order, or if the position is in an invalid state.")]
        public IEnumerable<FacilityBooking> FacilityBookingList
        {
            get
            {
                if (CurrentPickingPos == null
                    || CurrentPicking == null
                    || CurrentPickingPos.EntityState == EntityState.Added
                    || CurrentPickingPos.EntityState == EntityState.Deleted
                    || CurrentPickingPos.EntityState == EntityState.Detached)
                    return null;
                if ((CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.Receipt
                  || CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.ReceiptVehicle) && CurrentPickingPos.InOrderPos != null)
                {
                    CurrentPickingPos.InOrderPos.FacilityBooking_InOrderPos.AutoLoad(CurrentPickingPos.InOrderPos.FacilityBooking_InOrderPosReference, CurrentPickingPos);
                    return CurrentPickingPos.InOrderPos.FacilityBooking_InOrderPos.OrderBy(c => c.FacilityBookingNo).ToList();
                }
                else if ((CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.Issue
                       || CurrentPicking.MDPickingType.MDPickingTypeIndex == (short)GlobalApp.PickingType.IssueVehicle) && CurrentPickingPos.OutOrderPos != null)
                {
                    CurrentPickingPos.OutOrderPos.FacilityBooking_OutOrderPos.AutoLoad(CurrentPickingPos.OutOrderPos.FacilityBooking_OutOrderPosReference, CurrentPickingPos);
                    return CurrentPickingPos.OutOrderPos.FacilityBooking_OutOrderPos.OrderBy(c => c.FacilityBookingNo).ToList();
                }
                List<FacilityBooking> bookingList = null;
                if (CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any())
                {
                    foreach (var pickingPosRef in CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos)
                        pickingPosRef.ProdorderPartslistPos.FacilityBooking_ProdOrderPartslistPos.AutoRefresh(pickingPosRef.ProdorderPartslistPos.FacilityBooking_ProdOrderPartslistPosReference, pickingPosRef);
                    bookingList = CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.SelectMany(c => c.ProdorderPartslistPos.FacilityBooking_ProdOrderPartslistPos).OrderBy(c => c.FacilityBookingNo).ToList();
                }

                if (bookingList == null || !bookingList.Any())
                {
                    CurrentPickingPos.FacilityBooking_PickingPos.AutoLoad(CurrentPickingPos.FacilityBooking_PickingPosReference, CurrentPickingPos);
                    bookingList = CurrentPickingPos.FacilityBooking_PickingPos.OrderBy(c => c.FacilityBookingNo).ToList();
                }
                return bookingList;
            }
        }

        FacilityBooking _SelectedFacilityBooking;
        /// <summary>
        /// Gets or sets the currently selected facility booking from the list for UI selection purposes.
        /// This property represents the facility booking that is highlighted or selected in the user interface,
        /// typically in a list or grid control. When set, it triggers property change notifications, 
        /// updates the CurrentFacilityBooking property for detailed editing, and refreshes the 
        /// FacilityBookingChargeList to display related booking charge records.
        /// </summary>
        [ACPropertySelected(621, FacilityBooking.ClassName,
                            Description = @"Gets or sets the currently selected facility booking from the list for UI selection purposes.
                                            This property represents the facility booking that is highlighted or selected in the user interface,
                                            typically in a list or grid control. When set, it triggers property change notifications, 
                                            updates the CurrentFacilityBooking property for detailed editing, and refreshes the 
                                            FacilityBookingChargeList to display related booking charge records.")]
        public FacilityBooking SelectedFacilityBooking
        {
            get
            {
                return _SelectedFacilityBooking;
            }
            set
            {
                _SelectedFacilityBooking = value;
                OnPropertyChanged(nameof(SelectedFacilityBooking));
                CurrentFacilityBooking = value;
                OnPropertyChanged(nameof(FacilityBookingChargeList));
            }
        }
        #endregion

        #region FacilityBookingCharge

        /// <summary>
        /// Gets the list of facility booking charges associated with the current facility booking.
        /// Returns facility booking charges for the selected facility booking, ordered by their charge number.
        /// The charges are automatically refreshed from the database to ensure data consistency.
        /// Returns null if no current facility booking is selected.
        /// </summary>
        [ACPropertyList(623, "FacilityBookingCharge",
                        Description = @"Gets the list of facility booking charges associated with the current facility booking.
                                        Returns facility booking charges for the selected facility booking, ordered by their charge number.
                                        The charges are automatically refreshed from the database to ensure data consistency.
                                        Returns null if no current facility booking is selected.")]
        public IEnumerable<FacilityBookingCharge> FacilityBookingChargeList
        {
            get
            {
                if (CurrentFacilityBooking != null)
                {
                    CurrentFacilityBooking.FacilityBookingCharge_FacilityBooking.AutoRefresh(CurrentFacilityBooking.FacilityBookingCharge_FacilityBookingReference, CurrentFacilityBooking);
                    return CurrentFacilityBooking.FacilityBookingCharge_FacilityBooking.OrderBy(c => c.FacilityBookingChargeNo).ToList();
                }
                return null;
            }
        }

        private FacilityBookingCharge _SelectedFacilityBookingCharge;
        /// <summary>
        /// Gets or sets the currently selected facility booking charge from the list for UI selection purposes.
        /// This property represents the facility booking charge that is highlighted or selected in the user interface,
        /// typically in a list or grid control. When set, it updates the selected facility booking charge reference
        /// and triggers property change notifications to update bound UI controls.
        /// </summary>
        [ACPropertySelected(624, "FacilityBookingCharge",
                            Description = @"Gets or sets the currently selected facility booking charge from the list for UI selection purposes.
                                            This property represents the facility booking charge that is highlighted or selected in the user interface,
                                            typically in a list or grid control. When set, it updates the selected facility booking charge reference
                                            and triggers property change notifications to update bound UI controls.")]
        public FacilityBookingCharge SelectedFacilityBookingCharge
        {
            get
            {
                return _SelectedFacilityBookingCharge;
            }
            set
            {
                _SelectedFacilityBookingCharge = value;
                if (_SelectedFacilityBookingCharge != null)
                {
                    ReportFacilityCharge = _SelectedFacilityBookingCharge.InwardFacilityCharge != null ? _SelectedFacilityBookingCharge.InwardFacilityCharge : _SelectedFacilityBookingCharge.OutwardFacilityCharge;
                    ReportFacilityCharge.FBCTargetQuantityUOM = _SelectedFacilityBookingCharge.InwardTargetQuantityUOM > 0 ? _SelectedFacilityBookingCharge.InwardTargetQuantityUOM : _SelectedFacilityBookingCharge.OutwardTargetQuantityUOM;
                }
                else
                {
                    ReportFacilityCharge = null;
                }
            }
        }


        /// <summary>
        /// Source Property: 
        /// </summary>
        private FacilityCharge _ReportFacilityCharge;
        [ACPropertyInfo(999, nameof(ReportFacilityCharge), "en{'TODO:_ReportFacilityCharge'}de{'TODO:_ReportFacilityCharge'}")]
        public FacilityCharge ReportFacilityCharge
        {
            get
            {
                return _ReportFacilityCharge;
            }
            set
            {
                if (_ReportFacilityCharge != value)
                {
                    _ReportFacilityCharge = value;
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        #region Local Properties

        protected ACPropertyConfigValue<bool> _ForwardToRemoteStores;
        /// <summary>
        /// Configuration property that controls whether picking order changes should be forwarded to remote mirrored stores.
        /// When enabled, changes made to picking positions involving facilities that are mirrored on other databases
        /// will automatically broadcast those changes to the remote locations during the save operation.
        /// This ensures synchronization of picking data across distributed warehouse systems.
        /// Default value is false, meaning changes are only saved locally unless explicitly enabled.
        /// </summary>
        [ACPropertyConfig("en{'Forward to remote stores'}de{'An entfernte Läger weiterleiten'}",
                          Description = @"Configuration property that controls whether picking order changes should be forwarded to remote mirrored stores.
                                          When enabled, changes made to picking positions involving facilities that are mirrored on other databases
                                          will automatically broadcast those changes to the remote locations during the save operation.
                                          This ensures synchronization of picking data across distributed warehouse systems.
                                          Default value is false, meaning changes are only saved locally unless explicitly enabled.")]
        public bool ForwardToRemoteStores
        {
            get
            {
                return _ForwardToRemoteStores.ValueT;
            }
            set
            {
                _ForwardToRemoteStores.ValueT = value;
            }
        }

        protected ACPropertyConfigValue<bool> _NavigateOnGenRelated;
        /// <summary>
        /// Configuration property that controls whether the user interface should automatically navigate to a newly generated picking order.
        /// When enabled (true), the system will automatically switch the current view to display the newly created picking order
        /// after generation operations such as mirroring or creating related picking orders.
        /// When disabled (false), the system remains on the current picking order view after generation operations.
        /// Default value is false, meaning no automatic navigation occurs unless explicitly enabled.
        /// </summary>
        [ACPropertyConfig("en{'Navigate to ne generated picking order'}de{'Navigiere zu generiertem Kommissionierauftrag'}",
                          Description = @"Configuration property that controls whether the user interface should automatically navigate to a newly generated picking order.
                                          When enabled (true), the system will automatically switch the current view to display the newly created picking order
                                          after generation operations such as mirroring or creating related picking orders.
                                          When disabled (false), the system remains on the current picking order view after generation operations.
                                          Default value is false, meaning no automatic navigation occurs unless explicitly enabled.")]
        public bool NavigateOnGenRelated
        {
            get
            {
                return _NavigateOnGenRelated.ValueT;
            }
            set
            {
                _NavigateOnGenRelated.ValueT = value;
            }
        }

        protected ACPropertyConfigValue<ReservationState> _DefaultReservationState;
        /// <summary>
        /// Configuration property that defines the default reservation state for facility reservations in picking operations.
        /// This property determines the initial state assigned to new facility reservations when they are created
        /// during the picking workflow. The reservation state controls how materials are allocated and managed
        /// in the warehouse, affecting availability calculations and batch planning operations.
        /// Possible values include:
        /// - New: Newly created reservation without allocation
        /// - Reserved: Material quantity is allocated and reserved
        /// - Released: Reservation is confirmed and ready for processing
        /// - Completed: Reservation has been fulfilled
        /// This setting is used by the BSOFacilityReservation child component to initialize reservation states
        /// and ensure consistent behavior across picking position management and facility allocation processes.
        /// Default value is ReservationState.New.
        /// </summary>
        [ACPropertyConfig("en{'Def. Batch plannning state'}de{'Def. Reservierungssstatus'}",
                          Description = @"Configuration property that defines the default reservation state for facility reservations in picking operations.
                                          This property determines the initial state assigned to new facility reservations when they are created
                                          during the picking workflow. The reservation state controls how materials are allocated and managed
                                          in the warehouse, affecting availability calculations and batch planning operations.
                                          Possible values include:
                                          - New: Newly created reservation without allocation
                                          - Reserved: Material quantity is allocated and reserved
                                          - Released: Reservation is confirmed and ready for processing
                                          - Completed: Reservation has been fulfilled
                                          This setting is used by the BSOFacilityReservation child component to initialize reservation states
                                          and ensure consistent behavior across picking position management and facility allocation processes.
                                          Default value is ReservationState.New.")]
        public ReservationState DefaultReservationState
        {
            get
            {
                return _DefaultReservationState.ValueT;
            }
            set
            {
                _DefaultReservationState.ValueT = value;
            }
        }

        protected ACRef<ACInDeliveryNoteManager> _InDeliveryNoteManager = null;
        /// <summary>
        /// Gets the InDeliveryNoteManager service instance used for managing inbound delivery note operations.
        /// This property provides access to the ACInDeliveryNoteManager which handles the processing, validation,
        /// and management of incoming delivery notes within the picking workflow. The manager is initialized
        /// during component startup and is essential for receipt-type picking operations that involve
        /// creating and updating delivery notes for incoming materials and goods.
        /// Returns null if the manager reference is not initialized or the service is unavailable.
        /// </summary>
        public ACInDeliveryNoteManager InDeliveryNoteManager
        {
            get
            {
                if (_InDeliveryNoteManager == null)
                    return null;
                return _InDeliveryNoteManager.ValueT;
            }
        }

        protected ACRef<ACOutDeliveryNoteManager> _OutDeliveryNoteManager = null;
        /// <summary>
        /// Gets the OutDeliveryNoteManager service instance used for managing outbound delivery note operations.
        /// This property provides access to the ACOutDeliveryNoteManager which handles the processing, validation,
        /// and management of outgoing delivery notes within the picking workflow. The manager is initialized
        /// during component startup and is essential for issue-type picking operations that involve
        /// creating and updating delivery notes for outgoing materials and goods shipments.
        /// Returns null if the manager reference is not initialized or the service is unavailable.
        /// </summary>
        public ACOutDeliveryNoteManager OutDeliveryNoteManager
        {
            get
            {
                if (_OutDeliveryNoteManager == null)
                    return null;
                return _OutDeliveryNoteManager.ValueT;
            }
        }

        protected ACRef<ACPickingManager> _PickingManager = null;
        /// <summary>
        /// Gets the PickingManager service instance used for managing picking operations and workflows.
        /// This property provides access to the ACPickingManager which handles the core business logic
        /// for picking order processing, validation, assignment, unassignment, and state management.
        /// The manager is initialized during component startup and is essential for all picking-related
        /// operations including order creation, position management, facility bookings, and workflow execution.
        /// Returns null if the manager reference is not initialized or the service is unavailable.
        /// </summary>
        public ACPickingManager PickingManager
        {
            get
            {
                if (_PickingManager == null)
                    return null;
                return _PickingManager.ValueT;
            }
        }

        protected ACRef<ACComponent> _ACFacilityManager = null;
        /// <summary>
        /// Gets the FacilityManager service instance used for managing facility operations and bookings.
        /// This property provides access to the ACFacilityManager which handles all facility-related operations
        /// including material movements, stock management, booking processes, and facility reservations.
        /// The manager is initialized during component startup and is essential for all facility operations
        /// within the picking workflow including pre-bookings, bookings, and facility validations.
        /// Returns null if the manager reference is not initialized or the service is unavailable.
        /// </summary>
        public FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }

        protected ACMethodBooking BookParamInwardMovementClone
        {
            get
            {
                return ACFacilityManager.BookParamInOrderPosInwardMovementClone(DatabaseApp);
            }
        }

        protected ACMethodBooking BookParamOutwardMovementClone
        {
            get
            {
                return ACFacilityManager.BookParamOutOrderPosOutwardMovementClone(DatabaseApp);
            }
        }

        protected ACMethodBooking BookParamInCancelClone
        {
            get
            {
                return ACFacilityManager.BookParamInCancelClone(DatabaseApp);
            }
        }

        protected ACMethodBooking BookParamOutCancelClone
        {
            get
            {
                return ACFacilityManager.BookParamOutCancelClone(DatabaseApp);
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
                if (queryDelivStateAssigned.Any())
                    _StateCompletelyAssigned = queryDelivStateAssigned.First();
                return _StateCompletelyAssigned;
            }
        }

        protected List<InOrderPos> _UnSavedUnAssignedInOrderPos = new List<InOrderPos>();
        protected List<OutOrderPos> _UnSavedUnAssignedOutOrderPos = new List<OutOrderPos>();
        protected List<ProdOrderPartslistPos> _UnSavedUnAssignedProdOrderPartslistPos = new List<ProdOrderPartslistPos>();

        Nullable<double> _PartialQuantity;
        /// <summary>
        /// Gets or sets the partial quantity for picking operations.
        /// This property represents a portion of the total quantity that can be used
        /// for partial fulfillment of picking positions when the full target quantity
        /// is not available or when splitting quantities across multiple operations.
        /// Used in scenarios where picking positions need to be partially processed
        /// or when handling partial deliveries and split operations.
        /// </summary>
        [ACPropertyInfo(625, "", "en{'Partial Quantity'}de{'Teilmenge'}",
                        Description = @"Gets or sets the partial quantity for picking operations.
                                        This property represents a portion of the total quantity that can be used
                                        for partial fulfillment of picking positions when the full target quantity
                                        is not available or when splitting quantities across multiple operations.
                                        Used in scenarios where picking positions need to be partially processed
                                        or when handling partial deliveries and split operations.")]
        public Nullable<double> PartialQuantity
        {
            get
            {
                return _PartialQuantity;
            }
            set
            {
                _PartialQuantity = value;
                OnPropertyChanged(nameof(PartialQuantity));
            }
        }

        private string _BookingFacilityLotNo;
        /// <summary>
        /// Gets or sets the facility lot number used for booking operations.
        /// This property stores the lot number that can be used to filter or identify
        /// specific facility lots during booking processes within the picking workflow.
        /// When set, triggers property change notification to update bound UI controls.
        /// </summary>
        public string BookingFacilityLotNo
        {
            get
            {
                return _BookingFacilityLotNo;
            }
            set
            {
                _BookingFacilityLotNo = value;
                OnPropertyChanged(nameof(BookingFacilityLotNo));
            }
        }

        #endregion

        #region Message

        private MsgWithDetails _BSOMsg = new MsgWithDetails();
        /// <summary>
        /// Gets or sets the message with details for the picking business service object.
        /// This property holds comprehensive error, warning, and information messages
        /// that can contain multiple message details for complex business operations.
        /// When set, triggers property change notification to update bound UI controls
        /// that display message information to users during picking order processing.
        /// </summary>
        [ACPropertyInfo(626, "Message",
                        Description = @"Gets or sets the message with details for the picking business service object.
                                        This property holds comprehensive error, warning, and information messages
                                        that can contain multiple message details for complex business operations.
                                        When set, triggers property change notification to update bound UI controls
                                        that display message information to users during picking order processing.")]
        public MsgWithDetails BSOMsg
        {
            get
            {
                return _BSOMsg;
            }
            set
            {
                _BSOMsg = value;
                OnPropertyChanged(nameof(BSOMsg));
            }
        }

        #endregion

        #region Forward
        private class ForwardToMirroredStore
        {
            public ForwardToMirroredStore(Facility store)
            {
                MirroredStore = store;
                Pickings = new List<Picking>();
            }
            public Facility MirroredStore { get; private set; }
            public List<Picking> Pickings { get; private set; }
        }
        private List<ForwardToMirroredStore> _ForwardPlan;
        private List<Picking> _VisitedPickings = null;
        #endregion

        #region Properties => RMI and Routing

        ACPointAsyncRMISubscr _RMISubscr;
        /// <summary>
        /// Gets the RMI (Remote Method Invocation) subscription point for asynchronous communication.
        /// This property provides access to the ACPointAsyncRMISubscr which is used for subscribing to
        /// and receiving callbacks from remote method invocations. It enables asynchronous communication
        /// between components in distributed scenarios, allowing this BSOPicking instance to receive
        /// notifications and results from remote operations without blocking the UI thread.
        /// The RMI subscription point is configured with a maximum capacity of 1 and uses the
        /// "RMICallback" method as the callback handler for processing asynchronous responses.
        /// </summary>
        [ACPropertyAsyncMethodPointSubscr(9999, false, 0, "RMICallback",
                                          Description = @"Gets the RMI (Remote Method Invocation) subscription point for asynchronous communication.
                                                          This property provides access to the ACPointAsyncRMISubscr which is used for subscribing to
                                                          and receiving callbacks from remote method invocations. It enables asynchronous communication
                                                          between components in distributed scenarios, allowing this BSOPicking instance to receive
                                                          notifications and results from remote operations without blocking the UI thread.
                                                          The RMI subscription point is configured with a maximum capacity of 1 and uses the
                                                          ""RMICallback"" method as the callback handler for processing asynchronous responses.")]
        public ACPointAsyncRMISubscr RMISubscr
        {
            get
            {
                return _RMISubscr;
            }
        }

        private string _CalculateRouteResult;
        /// <summary>
        /// Gets or sets the result of the route calculation operation.
        /// This property stores the outcome message from the asynchronous route calculation process
        /// that checks for possible route conflicts between picking orders and production batch plans.
        /// The result indicates whether route conflicts were found or if the routes are clear for execution.
        /// </summary>
        [ACPropertyInfo(9999, "", "",
                        Description = @"Gets or sets the result of the route calculation operation.
                                        This property stores the outcome message from the asynchronous route calculation process
                                        that checks for possible route conflicts between picking orders and production batch plans.
                                        The result indicates whether route conflicts were found or if the routes are clear for execution.")]
        public string CalculateRouteResult
        {
            get => _CalculateRouteResult;
            set
            {
                _CalculateRouteResult = value;
                OnPropertyChanged();
            }
        }

        private SynchronizationContext _MainSyncContext;

        #endregion

        #endregion

        #region BSO->ACMethod

        #region ControlMode

        /// <summary>
        /// Determines the control mode for WPF controls bound to picking-related properties.
        /// This method evaluates various business rules and conditions to determine whether controls
        /// should be enabled, disabled, or have other presentation modes based on the current state
        /// of picking operations, material properties, and booking parameters.
        /// Key control mode evaluations include:
        /// - InwardFacilityLot: Disabled if material is not lot-managed
        /// - PickingMaterial: Disabled if picking position has order references or reservations
        /// - Quantity fields: Disabled if picking position is linked to order positions
        /// - ACMethodBooking properties: Delegates to the booking method's control mode logic
        /// This method is called by the WPF binding system whenever bound property values change
        /// to update the visual state and user interaction capabilities of UI controls.
        /// </summary>
        /// <param name="vbControl">The WPF control implementing IVBContent that requires control mode evaluation</param>
        /// <returns>The appropriate control mode (Enabled, Disabled, etc.) for the specified control</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            switch (vbControl.VBContent)
            {
                case "CurrentACMethodBooking\\InwardFacilityLot":
                    {
                        if (CurrentACMethodBooking != null && CurrentPickingPos != null)
                        {
                            if (CurrentPickingPos.Material == null
                                || !CurrentPickingPos.Material.IsLotManaged)
                                return Global.ControlModes.Disabled;
                        }
                        break;
                    }
                case "CurrentPickingPos\\PickingMaterial":
                    if (CurrentPickingPos == null
                       || CurrentPickingPos.InOrderPosID.HasValue
                       || CurrentPickingPos.OutOrderPosID.HasValue
                       || CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any()
                       || CurrentPickingPos.FacilityReservation_PickingPos.Any())
                        return Global.ControlModes.Disabled;
                    break;
                case "CurrentPickingPos\\PickingQuantityUOM":
                case "CurrentPickingPos\\TargetQuantity":
                case "CurrentPickingPos\\TargetQuantityUOM":
                    if (CurrentPickingPos == null
                        || CurrentPickingPos.InOrderPosID.HasValue
                        || CurrentPickingPos.OutOrderPosID.HasValue
                        || CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos != null && CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any())
                        return Global.ControlModes.Disabled;
                    break;
            }

            if (!String.IsNullOrEmpty(vbControl.VBContent) && vbControl.VBContent.StartsWith("CurrentACMethodBooking"))
            {
                if (CurrentACMethodBooking == null)
                    return Global.ControlModes.Disabled;
                int pos = vbControl.VBContent.IndexOf('\\');
                if (pos > 0)
                {
                    string methodId = vbControl.VBContent.Substring(0, pos);
                    string acValueId = vbControl.VBContent.Substring(pos + 1);
                    if (!String.IsNullOrEmpty(methodId) && !String.IsNullOrEmpty(acValueId))
                    {
                        ACMethodBooking acMethod = ACUrlCommand(methodId) as ACMethodBooking;
                        if (acMethod != null)
                        {
                            Global.ControlModesInfo subResult = acMethod.OnGetControlModes(vbControl, acValueId);
                            if (subResult.Mode != result)
                            {
                                result = subResult.Mode;
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        #region Picking

        /// <summary>
        /// Rebuilds the primary access navigation query definition by clearing existing filters and adding default filters.
        /// This method reinitializes the navigation query with standard filter criteria for picking operations:
        /// - Adds a filter for picking number (PickingNo) using contains operator for partial matching
        /// - Adds a filter for picking type index (PickingTypeIndex) using greater than or equal operator with Receipt type as minimum
        /// The method ensures the AccessPrimary navigation object is properly configured with basic filtering capabilities
        /// for picking order searches. It performs validation checks to ensure the necessary objects are available before execution.
        /// </summary>
        public virtual void RebuildAccessPrimary()
        {
            if (_AccessPrimary == null || _AccessPrimary.NavACQueryDefinition == null || ACType == null)
                return;
            String checkedOut = ((short)MDVisitorVoucherState.VisitorVoucherStates.CheckedIn).ToString();
            _AccessPrimary.NavACQueryDefinition.ClearFilter(true);
            short filterDelivType = (short)GlobalApp.PickingType.Receipt;
            _AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "PickingNo", Global.LogicalOperators.contains, Global.Operators.and, "", true));
            _AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "PickingTypeIndex", Global.LogicalOperators.greaterThanOrEqual, Global.Operators.and, filterDelivType.ToString(), true));
        }

        /// <summary>
        /// Its invoked from a WPF-Itemscontrol that wants to refresh its CollectionView because the user has changed the LINQ-Expressiontree in the ACQueryDefinition-Property of IAccess. 
        /// The BSO should execute the query on the database first, to get the new results for refreshing the CollectionView of the control.
        /// If the bso don't want to handle this request or manipulate the ACQueryDefinition it returns false. The WPF-control invokes then the IAccess.NavSearch()-Method itself.  
        /// </summary>
        /// <param name="acAccess">Reference to IAccess that contains the changed query (Property NavACQueryDefinition)</param>
        /// <returns>True if the bso has handled this request and queried the database context. Otherwise it returns false.</returns>
        public override bool ExecuteNavSearch(IAccess acAccess)
        {
            if (acAccess == _AccessBookingFacility)
            {
                _AccessBookingFacility.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(BookingFacilityList));
                return true;
            }
            else if (acAccess == _AccessBookingFacilityTarget)
            {
                _AccessBookingFacility.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(BookingFacilityListTarget));
                return true;
            }
            else if (acAccess == _AccessPickingPos)
            {
                _AccessPickingPos.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(PickingPosList));
                return true;
            }
            else if (acAccess == _AccessInOrderPos)
            {
                _AccessInOrderPos.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(InOrderPosList));
                return true;
            }
            else if (acAccess == _AccessOutOrderPos)
            {
                _AccessOutOrderPos.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(OutOrderPosList));
                return true;
            }
            else if (acAccess == _AccessProdOrderPartslistPos)
            {
                _AccessProdOrderPartslistPos.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(ProdOrderPartslistPosList));
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }

        /// <summary>
        /// Saves changes made to the picking order and its positions to the database.
        /// This method invokes the base class OnSave() method which handles the actual save operation,
        /// including validation, transaction management, and database persistence.
        /// The save operation performs the following:
        /// - Validates picking order data and positions before saving
        /// - Handles facility reservation updates and material movements
        /// - Manages delivery note synchronization for inbound/outbound operations
        /// - Processes facility booking confirmations and pre-booking cancellations
        /// - Updates picking order status and position states
        /// - Forwards changes to remote mirrored stores if configured
        /// - Refreshes related order position lists after successful save
        /// Call this method after making changes to CurrentPicking, picking positions,
        /// facility bookings, or any related entities to persist changes to the database.
        /// Always ensure data validation is complete before calling Save().
        /// </summary>
        [ACMethodCommand("Picking", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost,
                         Description = @"Saves changes made to the picking order and its positions to the database.
                                        This method invokes the base class OnSave() method which handles the actual save operation,
                                        including validation, transaction management, and database persistence.
                                        The save operation performs the following:
                                        - Validates picking order data and positions before saving
                                        - Handles facility reservation updates and material movements
                                        - Manages delivery note synchronization for inbound/outbound operations
                                        - Processes facility booking confirmations and pre-booking cancellations
                                        - Updates picking order status and position states
                                        - Forwards changes to remote mirrored stores if configured
                                        - Refreshes related order position lists after successful save
                                        Call this method after making changes to CurrentPicking, picking positions,
                                        facility bookings, or any related entities to persist changes to the database.
                                        Always ensure data validation is complete before calling Save().")]
        public void Save()
        {
            OnSave();
        }

        protected override Msg OnPreSave()
        {
            if (this.PickingManager != null)
            {
                this.PickingManager.ValidateOnSave(DatabaseApp, CurrentPicking);
            }

            _ForwardPlan = null;
            if (ForwardToRemoteStores
                && _VisitedPickings != null
                && _VisitedPickings.Any())
            {
                _ForwardPlan = new List<ForwardToMirroredStore>();
                IList<PickingPos> changedPositionsInThisBSO = DatabaseApp.GetChangedEntities<PickingPos>(c => c.Picking != null && _VisitedPickings.Contains(c.Picking));
                foreach (PickingPos changedPos in changedPositionsInThisBSO)
                {
                    if (changedPos.FromFacility != null && changedPos.FromFacility.IsMirroredOnMoreDatabases)
                    {
                        BroadcastFacility(changedPos.FromFacility, changedPos.Picking);
                    }
                    if (changedPos.ToFacility != null && changedPos.ToFacility.IsMirroredOnMoreDatabases)
                    {
                        BroadcastFacility(changedPos.ToFacility, changedPos.Picking);
                    }
                }

                // scenario for delete
                IList<PickingPos> deletedItems = DatabaseApp.GetDeletedEntities<PickingPos>(null);
                if (deletedItems.Any())
                {
                    foreach (PickingPos deletedItem in deletedItems)
                    {
                        PropertyValues originalItem = DatabaseApp.ChangeTracker.Entries().Where(c => c.Entity == deletedItem).FirstOrDefault().OriginalValues;
                        //DbDataRecord originalItem = DatabaseApp.GetOriginalValues(deletedItem.EntityKey);
                        Guid pickingID = (Guid)originalItem["PickingID"];
                        Picking picking = DatabaseApp.Picking.FirstOrDefault(c => c.PickingID == pickingID);
                        if (!string.IsNullOrEmpty(originalItem["FromFacilityID"].ToString()))
                        {
                            Guid fromFacilityID = (Guid)originalItem["FromFacilityID"];
                            Facility facility = DatabaseApp.Facility.FirstOrDefault(c => c.FacilityID == fromFacilityID);
                            if (facility.IsMirroredOnMoreDatabases)
                                BroadcastFacility(facility, picking);
                        }
                        if (!string.IsNullOrEmpty(originalItem["ToFacilityID"].ToString()))
                        {
                            Guid fromFacilityID = (Guid)originalItem["ToFacilityID"];
                            Facility facility = DatabaseApp.Facility.FirstOrDefault(c => c.FacilityID == fromFacilityID);
                            if (facility.IsMirroredOnMoreDatabases)
                                BroadcastFacility(facility, picking);
                        }
                    }
                }
            }
            return base.OnPreSave();
        }

        private void BroadcastFacility(Facility facility, Picking picking)
        {
            ForwardToMirroredStore broadcastEntry = _ForwardPlan.Where(c => c.MirroredStore == facility).FirstOrDefault();
            if (broadcastEntry == null)
            {
                broadcastEntry = new ForwardToMirroredStore(facility);
                _ForwardPlan.Add(broadcastEntry);
            }
            if (!broadcastEntry.Pickings.Contains(picking))
                broadcastEntry.Pickings.Add(picking);
        }

        protected override void OnPostSave()
        {
            _UnSavedUnAssignedInOrderPos = new List<InOrderPos>();
            _UnSavedUnAssignedOutOrderPos = new List<OutOrderPos>();
            _UnSavedUnAssignedProdOrderPartslistPos = new List<ProdOrderPartslistPos>();
            if (_ForwardPlan != null && _ForwardPlan.Any())
            {
                foreach (var broadcastEntry in _ForwardPlan)
                {
                    foreach (var picking in broadcastEntry.Pickings)
                    {
                        broadcastEntry.MirroredStore.CallSendPicking(false, picking.PickingID);
                    }
                }
            }
            if (ForwardToRemoteStores
                && CurrentPicking != null)
            {
                _VisitedPickings = new List<Picking>();
                _VisitedPickings.Add(CurrentPicking);
            }
            else
                _VisitedPickings = null;

            _ForwardPlan = null;

            RefreshInOrderPosList(false);
            RefreshOutOrderPosList(false);
            RefreshProdOrderPartslistPosList(false);

            LoadProcessWorkflowPresenter(CurrentPicking);
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
        /// Reverts changes made to the picking order and related entities without saving them to the database.
        /// This method calls the base OnUndoSave() method which handles the actual revert operation,
        /// including restoring entities to their original state, discarding unsaved changes, and
        /// refreshing data from the database if necessary. Use this method when you want to cancel
        /// modifications and return to the last saved state of the picking order and its positions.
        /// After calling UndoSave(), all pending changes will be lost and the picking order data
        /// will be restored to its previous saved state.
        /// </summary>
        [ACMethodCommand("Picking", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost,
                         Description = @"Reverts changes made to the picking order and related entities without saving them to the database.
                                         This method calls the base OnUndoSave() method which handles the actual revert operation,
                                         including restoring entities to their original state, discarding unsaved changes, and
                                         refreshing data from the database if necessary. Use this method when you want to cancel
                                         modifications and return to the last saved state of the picking order and its positions.
                                         After calling UndoSave(), all pending changes will be lost and the picking order data
                                         will be restored to its previous saved state.")]
        public void UndoSave()
        {
            OnUndoSave();
        }

        protected override void OnPostUndoSave()
        {
            _UnSavedUnAssignedInOrderPos = new List<InOrderPos>();
            _UnSavedUnAssignedOutOrderPos = new List<OutOrderPos>();
            _UnSavedUnAssignedProdOrderPartslistPos = new List<ProdOrderPartslistPos>();
            if (CurrentPicking.EntityState == EntityState.Modified)
            {
                RefreshInOrderPosList(true);
                RefreshOutOrderPosList(true);
                RefreshProdOrderPartslistPosList(true);
	            if (CurrentPicking != null && CurrentPicking.EntityState != EntityState.Detached)
	                CurrentPicking.PickingPos_Picking.AutoLoad(CurrentPicking.PickingPos_PickingReference, CurrentPicking);                OnPropertyChanged(nameof(PickingPosList));
            }
            else
            {
                Search();
            }
            base.OnPostUndoSave();        }


        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        private bool _InLoad = false;

        /// <summary>
        /// Loads the selected picking order from the database with all related entities and refreshes the UI state.
        /// This method performs a complete reload of the picking order with its positions, order references, facility bookings,
        /// and related data to ensure all information is current and synchronized with the database.
        /// The load operation includes:
        /// - Loading the picking order with all related picking positions and their dependencies
        /// - Including related order positions (InOrderPos/OutOrderPos) with their facility bookings and pre-bookings
        /// - Including facility references (FromFacility/ToFacility) and picking materials
        /// - Including delivery position load states and visitor voucher information
        /// - Refreshing ACProperties for the loaded picking order and visitor voucher
        /// - Setting up the first picking position as current if positions exist
        /// - Updating weighing list if requerying the currently selected position
        /// Use this method when you need to refresh the picking order data from the database,
        /// especially after external changes or when ensuring data consistency is critical.
        /// Always call this method before performing operations that depend on current database state.
        /// </summary>
        /// <param name="requery">If true, forces a complete refresh from database even if data appears current</param>
        [ACMethodInteraction("Picking", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedPicking", Global.ACKinds.MSMethodPrePost,
                             Description = @"Loads the selected picking order from the database with all related entities and refreshes the UI state.
                                             This method performs a complete reload of the picking order with its positions, order references, facility bookings,
                                             and related data to ensure all information is current and synchronized with the database.
                                             The load operation includes:
                                             - Loading the picking order with all related picking positions and their dependencies
                                             - Including related order positions (InOrderPos/OutOrderPos) with their facility bookings and pre-bookings
                                             - Including facility references (FromFacility/ToFacility) and picking materials
                                             - Including delivery position load states and visitor voucher information
                                             - Refreshing ACProperties for the loaded picking order and visitor voucher
                                             - Setting up the first picking position as current if positions exist
                                             - Updating weighing list if requerying the currently selected position
                                             Use this method when you need to refresh the picking order data from the database,
                                             especially after external changes or when ensuring data consistency is critical.
                                             Always call this method before performing operations that depend on current database state.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            _InLoad = true;
            LoadEntity<Picking>(requery, () => SelectedPicking, () => CurrentPicking, c => CurrentPicking = c,
                        DatabaseApp.Picking
                        .Include(c => c.PickingPos_Picking)
                        .Include("PickingPos_Picking.OutOrderPos")
                        .Include("PickingPos_Picking.OutOrderPos.FacilityBooking_OutOrderPos")
                        .Include("PickingPos_Picking.OutOrderPos.FacilityPreBooking_OutOrderPos")
                        .Include("PickingPos_Picking.InOrderPos")
                        .Include("PickingPos_Picking.InOrderPos.FacilityBooking_InOrderPos")
                        .Include("PickingPos_Picking.InOrderPos.FacilityPreBooking_InOrderPos")
                        .Include("PickingPos_Picking.ToFacility")
                        .Include("PickingPos_Picking.FromFacility")
                        .Include("PickingPos_Picking.PickingMaterial")
                        .Include("PickingPos_Picking.MDDelivPosLoadState")
                        //.Include("PickingPos_Picking.ProdOrderPartslistPos")
                        //.Include("PickingPos_Picking.ProdOrderPartslistPos.MDProdOrderPartslistPosState")
                        .Include(c => c.VisitorVoucher)
                        .Include(c => c.MDPickingType)
                        .Where(c => c.PickingID == SelectedPicking.PickingID));

            _InLoad = false;
            if (CurrentPicking != null)
            {
                CurrentPicking.ACProperties.Refresh();
                if (CurrentPicking.VisitorVoucher != null)
                    CurrentPicking.VisitorVoucher.ACProperties.Refresh();

                CurrentPicking.PickingPos_Picking.AutoRefresh(CurrentPicking.PickingPos_PickingReference, CurrentPicking);
            }
            PostExecute("Load");
            OnPropertyChanged(nameof(PickingPosList));
            if (PickingPosList != null && PickingPosList.Any())
            {
                _CurrentPickingPos = null;
                CurrentPickingPos = PickingPosList.FirstOrDefault();
                bool refreshWeighing = requery && SelectedPickingPos == CurrentPickingPos;
                SelectedPickingPos = CurrentPickingPos;
                if (refreshWeighing)
                    RefreshWeighingList(true);
            }
            else
            {
                CurrentPickingPos = null;
                SelectedPickingPos = null;
            }
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedPicking != null;
        }

        /// <summary>
        /// Creates a new picking order with default settings and initializes it for editing.
        /// This method generates a new picking order entity with the following operations:
        /// - Generates a unique picking number using the configured number generator
        /// - Sets default picking type to Receipt if no filter is applied
        /// - Applies currently selected filter values for picking type and delivery address
        /// - Adds the new picking order to the database context and UI list
        /// - Sets the new picking order as current and selected for immediate editing
        /// - Calls virtual OnNewPicking method for customization by derived classes
        /// - Sets the component state to "New" to indicate creation mode
        /// This method is typically called when users click the "New" button in the picking order interface.
        /// After calling this method, users can modify the picking order properties before saving.
        /// </summary>
        [ACMethodInteraction("Picking", Const.New, (short)MISort.New, true, "SelectedPicking", Global.ACKinds.MSMethodPrePost,
                             Description = @"Creates a new picking order with default settings and initializes it for editing.
                                             This method generates a new picking order entity with the following operations:
                                             - Generates a unique picking number using the configured number generator
                                             - Sets default picking type to Receipt if no filter is applied
                                             - Applies currently selected filter values for picking type and delivery address
                                             - Adds the new picking order to the database context and UI list
                                             - Sets the new picking order as current and selected for immediate editing
                                             - Calls virtual OnNewPicking method for customization by derived classes
                                             - Sets the component state to ""New"" to indicate creation mode
                                             This method is typically called when users click the ""New"" button in the picking order interface.
                                             After calling this method, users can modify the picking order properties before saving.")]
        public void New()
        {
            if (!PreExecute("New"))
                return;
            if (AccessPrimary == null)
                return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Picking), Picking.NoColumnName, Picking.FormatNewNo, this);
            Picking newPicking = Picking.NewACObject(DatabaseApp, null, secondaryKey);
            newPicking.MDPickingType = DatabaseApp.MDPickingType.FirstOrDefault(c => c.MDPickingTypeIndex == (short)GlobalApp.PickingType.Receipt);
            DatabaseApp.Picking.Add(newPicking);
            if (SelectedFilterMDPickingType != null)
                newPicking.MDPickingType = SelectedFilterMDPickingType;
            if (SelectedFilterDeliveryAddress != null)
                newPicking.DeliveryCompanyAddress = SelectedFilterDeliveryAddress;
            CurrentPicking = newPicking;
            OnNewPicking(newPicking);
            if (PickingList == null)
            {
                PickingList = new List<Picking>();
            }
            PickingList.Insert(0, newPicking);
            SelectedPicking = newPicking;
            ACState = Const.SMNew;
            PostExecute("New");
        }

        protected virtual void OnNewPicking(Picking newPicking)
        {
        }

        /// <summary>
        /// Determines whether [is enabled new delivery note].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new delivery note]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNew()
        {
            return true;
        }

        /// <summary>
        /// Deletes the current picking order from the database after unassigning all picking positions.
        /// This method performs the following operations in sequence:
        /// 1. Validates preconditions through PreExecute and AccessPrimary availability checks
        /// 2. Unassigns all picking positions associated with the current picking order using PickingManager
        /// 3. Deletes the picking order entity from the database
        /// 4. Updates the UI by removing the deleted picking from navigation lists and collections
        /// 5. Sets the first available picking order as the new current/selected picking
        /// 6. Refreshes the picking list display and preparation status information
        /// The deletion process includes proper error handling - if unassignment or deletion fails,
        /// error messages are displayed to the user and the operation is terminated without proceeding.
        /// This ensures data integrity by preventing orphaned picking positions and maintaining
        /// consistent state between the database and user interface.
        /// Call this method only when you want to permanently remove a picking order and all its
        /// associated positions. The operation cannot be undone once committed to the database.
        /// </summary>
        [ACMethodInteraction("Picking", Const.Delete, (short)MISort.Delete, true, "CurrentPicking", Global.ACKinds.MSMethodPrePost,
                             Description = @"Deletes the current picking order from the database after unassigning all picking positions.
                                             This method performs the following operations in sequence:
                                             1. Validates preconditions through PreExecute and AccessPrimary availability checks
                                             2. Unassigns all picking positions associated with the current picking order using PickingManager
                                             3. Deletes the picking order entity from the database
                                             4. Updates the UI by removing the deleted picking from navigation lists and collections
                                             5. Sets the first available picking order as the new current/selected picking
                                             6. Refreshes the picking list display and preparation status information
                                             The deletion process includes proper error handling - if unassignment or deletion fails,
                                             error messages are displayed to the user and the operation is terminated without proceeding.
                                             This ensures data integrity by preventing orphaned picking positions and maintaining
                                             consistent state between the database and user interface.
                                             Call this method only when you want to permanently remove a picking order and all its
                                             associated positions. The operation cannot be undone once committed to the database.")]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (AccessPrimary == null)
                return;
            Msg msg = PickingManager.UnassignAllPickingPos(CurrentPicking, DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            msg = CurrentPicking.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            AccessPrimary.NavList.Remove(CurrentPicking);
            _PickingList = AccessPrimary.NavList.ToList();
            SetPreparationStatusInList();
            Picking firstPicking = AccessPrimary.NavList.FirstOrDefault();
            OnPropertyChanged(nameof(PickingList));
            CurrentPicking = firstPicking;
            SelectedPicking = firstPicking;
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete delivery note].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete delivery note]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return true;
        }

        /// <summary>
        /// Executes a search operation to retrieve picking orders from the database based on the current filter criteria.
        /// This method performs the following operations:
        /// - Clears the current picking list and resets it to null
        /// - Executes a navigation search using the primary access object with OverwriteChanges merge option
        /// - Converts the navigation results to a list and assigns it to the PickingList property
        /// - Updates preparation status information for all retrieved picking orders
        /// - Triggers property change notification for the PickingList to update bound UI controls
        /// - Optionally refreshes related order position lists (InOrder, OutOrder, ProdOrder) if refreshList parameter is true
        /// The search respects all active filter criteria including picking number, date ranges, picking type,
        /// picking state, delivery address, material filters, and lot number filters that have been configured
        /// through the various filter properties and navigation query definition.
        /// </summary>
        /// <param name="refreshList">If true, also refreshes the related order position lists after the search completes</param>
        [ACMethodCommand("Picking", "en{'Search'}de{'Suchen'}", (short)MISort.Search,
                         Description = @"Executes a search operation to retrieve picking orders from the database based on the current filter criteria.
                                         This method performs the following operations:
                                         - Clears the current picking list and resets it to null
                                         - Executes a navigation search using the primary access object with OverwriteChanges merge option
                                         - Converts the navigation results to a list and assigns it to the PickingList property
                                         - Updates preparation status information for all retrieved picking orders
                                         - Triggers property change notification for the PickingList to update bound UI controls
                                         - Optionally refreshes related order position lists (InOrder, OutOrder, ProdOrder) if refreshList parameter is true
                                         The search respects all active filter criteria including picking number, date ranges, picking type,
                                         picking state, delivery address, material filters, and lot number filters that have been configured
                                         through the various filter properties and navigation query definition.")]
        public virtual void Search(bool refreshList = true)
        {
            if (AccessPrimary == null)
                return;

            _PickingList = null;
            if (AccessPrimary != null)
            {
                AccessPrimary.NavSearch(DatabaseApp, MergeOption.OverwriteChanges);
                _PickingList = AccessPrimary.NavList.ToList();
                SetPreparationStatusInList();
            }

            OnPropertyChanged(nameof(PickingList));

            if (refreshList)
            {
                RefreshInOrderPosList();
                RefreshOutOrderPosList();
                RefreshProdOrderPartslistPosList();
            }
        }

        /// <summary>
        /// Cancels the current picking order by performing the following operations:
        /// 1. Prompts the user for confirmation to cancel the picking order
        /// 2. Cancels all facility pre-bookings associated with the picking positions
        /// 3. Books all ACMethod bookings for positions that have pre-bookings
        /// 4. Counts assigned and cancelled picking positions to determine overall status
        /// 5. Sets the picking order state to Cancelled if all assigned positions are cancelled
        /// 6. Saves changes and refreshes the UI display
        /// This method handles the complete cancellation workflow including facility management,
        /// booking operations, and status updates while maintaining data consistency.
        /// </summary>
        [ACMethodCommand("Picking", "en{'Cancel Picking'}de{'Storniere Kommission'}", (short)MISort.Cancel, true, Global.ACKinds.MSMethodPrePost,
                         Description = @"Cancels the current picking order by performing the following operations:
                                         1. Prompts the user for confirmation to cancel the picking order
                                         2. Cancels all facility pre-bookings associated with the picking positions
                                         3. Books all ACMethod bookings for positions that have pre-bookings
                                         4. Counts assigned and cancelled picking positions to determine overall status
                                         5. Sets the picking order state to Cancelled if all assigned positions are cancelled
                                         6. Saves changes and refreshes the UI display
                                         This method handles the complete cancellation workflow including facility management,
                                         booking operations, and status updates while maintaining data consistency.")]
        public virtual void CancelPicking()
        {
            if (!PreExecute("CancelPicking"))
                return;
            if (!IsEnabledCancelPicking())
                return;
            // "Möchten Sie die Kommission stornieren"
            Msg msgForAll = new Msg
            {
                Source = GetACUrl(),
                MessageLevel = eMsgLevel.Info,
                ACIdentifier = "CancelPicking(0)",
                Message = Root.Environment.TranslateMessage(this, "Question50017")
            };
            Global.MsgResult msgResult = Messages.Msg(msgForAll, Global.MsgResult.No, eMsgButton.YesNo);
            if (msgResult == Global.MsgResult.No)
                return;
            var result = ACFacilityManager.CancelFacilityPreBooking(DatabaseApp, CurrentPicking);
            if (result != null && result.Any())
            {
                foreach (PickingPos pickingPos in PickingPosList.ToList())
                {
                    SelectedPickingPos = pickingPos;
                    if (CurrentPickingPos == pickingPos)
                    {
                        BookAllACMethodBookings();
                    }
                }
            }

            int countCancelled = 0;
            int countAssigned = 0;
            foreach (PickingPos pickingPos in PickingPosList.ToList())
            {
                if (pickingPos.InOrderPos != null || pickingPos.OutOrderPos != null)
                    countAssigned++;
                if ((pickingPos.InOrderPos != null)
                    && (pickingPos.InOrderPos.MDInOrderPosState.InOrderPosState == MDInOrderPosState.InOrderPosStates.Cancelled
                        || !pickingPos.InOrderPos.FacilityBooking_InOrderPos.Any()))
                {
                    countCancelled++;
                }
                else if ((pickingPos.OutOrderPos != null)
                    && (pickingPos.OutOrderPos.MDOutOrderPosState.OutOrderPosState == MDOutOrderPosState.OutOrderPosStates.Cancelled
                        || !pickingPos.OutOrderPos.FacilityBooking_OutOrderPos.Any()))
                {
                    countCancelled++;
                }
            }

            if (countCancelled == countAssigned
                && CurrentPicking.PickingState != PickingStateEnum.Cancelled)
            {
                CurrentPicking.PickingState = PickingStateEnum.Cancelled;
                Save();
            }

            OnPropertyChanged(nameof(PickingList));
            CurrentPicking = CurrentPicking;

            PostExecute("CancelPicking");
        }

        /// <summary>
        /// Determines whether the CancelPicking method can be executed.
        /// Checks if there is a current picking order selected, if it's not already cancelled,
        /// and if it has any picking positions to process.
        /// </summary>
        /// <returns>True if the picking order can be cancelled; otherwise, false.</returns>
        public bool IsEnabledCancelPicking()
        {
            if (CurrentPicking == null)
                return false;
            if (CurrentPicking.PickingState == PickingStateEnum.Cancelled)
                return false;
            if (!PickingPosList.Any())
                return false;
            return true;
        }

        /// <summary>
        /// Creates a mirrored copy of the current picking order with the same structure and positions.
        /// This method uses the PickingManager to create an exact duplicate of the current picking order,
        /// including all picking positions, materials, quantities, and facility assignments. The mirrored
        /// picking maintains the same configuration but receives a new picking number and can be processed
        /// independently of the original order.
        /// The method performs the following operations:
        /// - Validates that mirroring is enabled through IsEnabledMirrorPicking()
        /// - Preserves the current picking reference for potential restoration
        /// - Invokes PickingManager.MirrorPicking() to create the mirrored copy
        /// - Adds the new mirrored picking to the navigation list for UI display
        /// - Conditionally navigates to the new picking based on NavigateOnGenRelated configuration
        /// - Updates the picking list property to reflect the changes in the UI
        /// Use this method when you need to create duplicate picking orders for scenarios such as:
        /// - Creating backup or alternative picking orders
        /// - Generating picking orders for different shifts or time periods
        /// - Duplicating complex picking configurations for similar operations
        /// - Creating template-based picking orders with the same structure
        /// The NavigateOnGenRelated configuration determines whether the UI automatically switches
        /// to display the newly created mirrored picking order or remains on the original order.
        /// </summary>
        [ACMethodInfo("MirrorPicking", "en{'Mirror picking'}de{'Kommissionierung spiegeln'}", 100,
                      Description = @"Creates a mirrored copy of the current picking order with the same structure and positions.
                                      This method uses the PickingManager to create an exact duplicate of the current picking order,
                                      including all picking positions, materials, quantities, and facility assignments. The mirrored
                                      picking maintains the same configuration but receives a new picking number and can be processed
                                      independently of the original order.
                                      The method performs the following operations:
                                      - Validates that mirroring is enabled through IsEnabledMirrorPicking()
                                      - Preserves the current picking reference for potential restoration
                                      - Invokes PickingManager.MirrorPicking() to create the mirrored copy
                                      - Adds the new mirrored picking to the navigation list for UI display
                                      - Conditionally navigates to the new picking based on NavigateOnGenRelated configuration
                                      - Updates the picking list property to reflect the changes in the UI
                                      Use this method when you need to create duplicate picking orders for scenarios such as:
                                      - Creating backup or alternative picking orders
                                      - Generating picking orders for different shifts or time periods
                                      - Duplicating complex picking configurations for similar operations
                                      - Creating template-based picking orders with the same structure
                                      The NavigateOnGenRelated configuration determines whether the UI automatically switches
                                      to display the newly created mirrored picking order or remains on the original order.")]
        public void MirrorPicking()
        {
            if (!IsEnabledMirrorPicking())
                return;
            Picking currentPicking = CurrentPicking;
            Picking mirroredPicking = PickingManager.MirrorPicking(DatabaseApp, CurrentPicking);
            AccessPrimary.NavList.Add(mirroredPicking);
            OnPropertyChanged(nameof(PickingList));
            if (NavigateOnGenRelated && mirroredPicking != null)
            {
                CurrentPicking = mirroredPicking;
                SelectedPicking = mirroredPicking;
            }
            else
            {
                CurrentPicking = currentPicking;
                CurrentPicking = currentPicking;
            }
        }

        /// <summary>
        /// Determines whether the current picking order can be mirrored.
        /// Returns true if there is a current picking order selected and it contains at least one picking position.
        /// This method is used to enable/disable the mirror picking functionality in the user interface.
        /// </summary>
        /// <returns>True if the picking order can be mirrored; otherwise, false.</returns>
        public bool IsEnabledMirrorPicking()
        {
            return CurrentPicking != null && CurrentPicking.PickingPos_Picking.Any();
        }

        /// <summary>
        /// Creates supply pickings for the current picking order to support material provisioning workflows.
        /// This method generates additional picking orders that are used to supply materials from storage locations
        /// to production or intermediate staging areas, enabling automated material flow in warehouse operations.
        /// The method performs the following operations:
        /// - Validates that supply picking generation is enabled through IsEnabledGenerateSubPickingsForSupply()
        /// - Preserves the current picking reference for potential restoration after generation
        /// - Invokes PickingManager.CreateSupplyPickings() to generate the supply picking orders based on the current picking
        /// - Uses the selected process workflow node (SelectedPWNodeProcessWorkflow) to determine routing and workflow context
        /// - Adds all generated supply pickings to both the navigation list and picking list for UI display
        /// - Conditionally navigates to the first generated picking based on NavigateOnGenRelated configuration
        /// - Updates the picking list property to reflect the new supply pickings in the user interface
        /// Supply pickings are typically used in scenarios such as:
        /// - Automated material replenishment from main storage to production line storage
        /// - Pre-positioning materials at intermediate locations before final picking
        /// - Supporting multi-stage picking workflows where materials need to be staged
        /// - Enabling just-in-time material provisioning for production orders
        /// The NavigateOnGenRelated configuration determines whether the UI automatically switches
        /// to display the newly created supply picking order or remains on the original order.
        /// </summary>
        [ACMethodInfo("MirrorPicking", "en{'Create pickings for supply'}de{'Erstelle Bereitstellungsaufträge'}", 101,
                      Description = @"Creates supply pickings for the current picking order to support material provisioning workflows.
                                      This method generates additional picking orders that are used to supply materials from storage locations
                                      to production or intermediate staging areas, enabling automated material flow in warehouse operations.
                                      The method performs the following operations:
                                      - Validates that supply picking generation is enabled through IsEnabledGenerateSubPickingsForSupply()
                                      - Preserves the current picking reference for potential restoration after generation
                                      - Invokes PickingManager.CreateSupplyPickings() to generate the supply picking orders based on the current picking
                                      - Uses the selected process workflow node (SelectedPWNodeProcessWorkflow) to determine routing and workflow context
                                      - Adds all generated supply pickings to both the navigation list and picking list for UI display
                                      - Conditionally navigates to the first generated picking based on NavigateOnGenRelated configuration
                                      - Updates the picking list property to reflect the new supply pickings in the user interface
                                      Supply pickings are typically used in scenarios such as:
                                      - Automated material replenishment from main storage to production line storage
                                      - Pre-positioning materials at intermediate locations before final picking
                                      - Supporting multi-stage picking workflows where materials need to be staged
                                      - Enabling just-in-time material provisioning for production orders
                                      The NavigateOnGenRelated configuration determines whether the UI automatically switches
                                      to display the newly created supply picking order or remains on the original order.")]
        public virtual void GenerateSubPickingsForSupply()
        {
            if (!IsEnabledGenerateSubPickingsForSupply())
                return;
            Picking mirroredPicking = null;
            Picking currentPicking = CurrentPicking;
            IEnumerable<Picking> mirroredPickings = PickingManager.CreateSupplyPickings(DatabaseApp, currentPicking, this.SelectedPWNodeProcessWorkflow?.ACClassWFID, this);
            if (mirroredPickings != null && mirroredPickings.Any())
            {
                foreach (var picking in mirroredPickings)
                {
                    if (mirroredPicking == null)
                    {
                        mirroredPicking = picking;
                    }

                    if (!AccessPrimary.NavList.Contains(picking))
                    {
                        AccessPrimary.NavList.Add(picking);
                    }
                    if (!PickingList.Contains(picking))
                    {
                        PickingList.Add(picking);
                    }
                }
            }

            OnPropertyChanged(nameof(PickingList));
            if (NavigateOnGenRelated && mirroredPicking != null)
            {
                CurrentPicking = mirroredPicking;
                SelectedPicking = mirroredPicking;
            }
            else
            {
                CurrentPicking = currentPicking;
                CurrentPicking = currentPicking;
            }
        }

        /// <summary>
        /// Determines whether supply pickings can be generated for the current picking order.
        /// Returns true if there is a current picking order that is not finished and is not of type Receipt or ReceiptVehicle.
        /// Supply pickings are used to create automated material provisioning orders that support the main picking workflow
        /// by pre-positioning materials from storage locations to production or intermediate staging areas.
        /// </summary>
        /// <returns>True if supply pickings can be generated; otherwise, false.</returns>
        public virtual bool IsEnabledGenerateSubPickingsForSupply()
        {
            return CurrentPicking != null
                && CurrentPicking.PickingState < PickingStateEnum.Finished
                //&& CurrentPicking.MDPickingType?.PickingType != PickingType.AutomaticRelocation
                //&& CurrentPicking.MDPickingType?.PickingType != PickingType.InternalRelocation
                && CurrentPicking.MDPickingType?.PickingType != PickingType.Receipt
                && CurrentPicking.MDPickingType?.PickingType != PickingType.ReceiptVehicle;
        }

        /// <summary>
        /// Recalculates the actual quantity for all picking positions in the selected picking order.
        /// This method iterates through all picking positions associated with the selected picking order
        /// and calls the RecalcActualQuantity() method on each position to update their actual quantities
        /// based on the current facility bookings and movements. This is useful when actual quantities
        /// need to be refreshed after booking operations or data synchronization processes.
        /// </summary>
        [ACMethodInteraction(nameof(RecalcActualQuantity), "en{'Recalculate Actual Quantity'}de{'Istmenge neu berechnen'}", (short)MISort.New, true, nameof(SelectedPicking),
                             Description = @"Recalculates the actual quantity for all picking positions in the selected picking order.
                                             This method iterates through all picking positions associated with the selected picking order
                                             and calls the RecalcActualQuantity() method on each position to update their actual quantities
                                             based on the current facility bookings and movements. This is useful when actual quantities
                                             need to be refreshed after booking operations or data synchronization processes.")]
        public void RecalcActualQuantity()
        {
            if (!IsEnabledRecalcActualQuantity())
            {
                return;
            }

            PickingPos[] pickingPositions = SelectedPicking.PickingPos_Picking.ToArray();
            foreach (PickingPos pos in pickingPositions)
            {
                pos.RecalcActualQuantity();
            }
        }

        /// <summary>
        /// Determines whether the RecalcActualQuantity method can be executed.
        /// Returns true if there is a currently selected picking order that contains at least one picking position.
        /// This method is used to enable/disable the recalculate actual quantity functionality in the user interface.
        /// The recalculation updates the actual quantities of all picking positions based on current facility bookings and movements.
        /// </summary>
        /// <returns>True if the picking order has positions and can have actual quantities recalculated; otherwise, false.</returns>
        public bool IsEnabledRecalcActualQuantity()
        {
            return
                SelectedPicking != null
                && SelectedPicking.PickingPos_Picking.Any();
        }

        /// <summary>
        /// Gets the class name of the root workflow process method for picking operations.
        /// Returns the configured PWMethodRelocClass from the FacilityManager if available,
        /// otherwise returns the default PWMethodRelocation class name. This method is used
        /// to determine which workflow class should be used as the root process for picking
        /// order execution and routing operations.
        /// </summary>
        /// <returns>The class name of the root PWMethod workflow class to use for picking operations</returns>
        public virtual string GetPWClassNameOfRoot()
        {
            if (this.ACFacilityManager != null)
                return this.ACFacilityManager.C_PWMethodRelocClass;
            else
                return nameof(PWMethodRelocation);
        }

        #endregion

        #region Picking-Pos

        /// <summary>
        /// Unassigns the current picking position from its associated order position and removes it from the picking order.
        /// This method handles different types of order positions including InOrderPos, OutOrderPos, and production order positions.
        /// The method performs the following operations:
        /// 1. Checks if the picking position is associated with an InOrderPos and handles parent-child relationships
        /// 2. Checks if the picking position is associated with an OutOrderPos and handles parent-child relationships  
        /// 3. Checks if the picking position is associated with production order parts list positions
        /// 4. For positions without order associations, performs direct unassignment
        /// 5. Tracks unassigned parent positions in temporary collections for UI refresh purposes
        /// 6. Refreshes related order position lists and clears partial quantity values
        /// For each order type, the method:
        /// - Navigates to the top-level parent order position (1st level)
        /// - Calls the PickingManager to perform the actual unassignment operation
        /// - Handles any errors that occur during unassignment with proper logging
        /// - Adds successfully unassigned parent positions to tracking collections
        /// - Refreshes the corresponding order position lists in the UI
        /// Error handling includes logging exceptions with specific identifiers for troubleshooting.
        /// The method ensures that the UI is properly updated after unassignment operations complete.
        /// </summary>
        [ACMethodCommand("PickingPos", "en{'Remove'}de{'Entfernen'}", 601, true, Global.ACKinds.MSMethodPrePost,
                         Description = @"Unassigns the current picking position from its associated order position and removes it from the picking order.
                                         This method handles different types of order positions including InOrderPos, OutOrderPos, and production order positions.
                                         The method performs the following operations:
                                         1. Checks if the picking position is associated with an InOrderPos and handles parent-child relationships
                                         2. Checks if the picking position is associated with an OutOrderPos and handles parent-child relationships  
                                         3. Checks if the picking position is associated with production order parts list positions
                                         4. For positions without order associations, performs direct unassignment
                                         5. Tracks unassigned parent positions in temporary collections for UI refresh purposes
                                         6. Refreshes related order position lists and clears partial quantity values
                                         For each order type, the method:
                                         - Navigates to the top-level parent order position (1st level)
                                         - Calls the PickingManager to perform the actual unassignment operation
                                         - Handles any errors that occur during unassignment with proper logging
                                         - Adds successfully unassigned parent positions to tracking collections
                                         - Refreshes the corresponding order position lists in the UI
                                         Error handling includes logging exceptions with specific identifiers for troubleshooting.
                                         The method ensures that the UI is properly updated after unassignment operations complete.")]
        public virtual void UnassignPickingPos()
        {
            if (!PreExecute(nameof(UnassignPickingPos)))
                return;

            // TODO: Es muss noch implementiert werden, wenn Teilmenge abgerufen worden ist bei den mehrere Kommissionierpositionen entstehen
            if (CurrentPickingPos.InOrderPos != null && CurrentPickingPos.InOrderPos.InOrderPos1_ParentInOrderPos != null)
            {
                InOrderPos parentInOrderPos = CurrentPickingPos.InOrderPos.InOrderPos1_ParentInOrderPos; // 2. Ebene
                parentInOrderPos = parentInOrderPos.InOrderPos1_ParentInOrderPos; // 1. Ebene

                Msg result = null;
                try
                {
                    result = PickingManager.UnassignPickingPos(CurrentPickingPos, this.DatabaseApp);
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

                    Messages.LogException("BSOPicking", "UnassignPickingPos", msg);
                }

                if (result == null && parentInOrderPos != null)
                {
                    if (!_UnSavedUnAssignedInOrderPos.Contains(parentInOrderPos))
                        _UnSavedUnAssignedInOrderPos.Add(parentInOrderPos);
                }
                RefreshInOrderPosList();
            }
            else if (CurrentPickingPos.OutOrderPos != null && CurrentPickingPos.OutOrderPos.OutOrderPos1_ParentOutOrderPos != null)
            {
                OutOrderPos parentOutOrderPos = CurrentPickingPos.OutOrderPos.OutOrderPos1_ParentOutOrderPos;
                parentOutOrderPos = parentOutOrderPos.OutOrderPos1_ParentOutOrderPos; // 1. Ebene

                Msg result = null;
                try
                {
                    result = PickingManager.UnassignPickingPos(CurrentPickingPos, this.DatabaseApp);
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

                    Messages.LogException("BSOPicking", "UnassignPickingPos(10)", msg);
                }

                if (result == null && parentOutOrderPos != null)
                {
                    if (!_UnSavedUnAssignedOutOrderPos.Contains(parentOutOrderPos))
                        _UnSavedUnAssignedOutOrderPos.Add(parentOutOrderPos);
                }
                RefreshOutOrderPosList();
            }
            else if (CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any())
            {
                List<ProdOrderPartslistPos> parentProdOrderPartslistPositions =
                    CurrentPickingPos
                    .PickingPosProdOrderPartslistPos_PickingPos
                    .Select(c => c.ProdorderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos).ToList();

                Msg result = null;
                try
                {
                    result = PickingManager.UnassignPickingPos(CurrentPickingPos, this.DatabaseApp);
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

                    Messages.LogException("BSOPicking", "UnassignPickingPos(20)", msg);
                }

                if (result == null && parentProdOrderPartslistPositions != null && parentProdOrderPartslistPositions.Any())
                {
                    foreach (var parentProdOrderPartslistPos in parentProdOrderPartslistPositions)
                        if (!_UnSavedUnAssignedProdOrderPartslistPos.Contains(parentProdOrderPartslistPos))
                            _UnSavedUnAssignedProdOrderPartslistPos.Add(parentProdOrderPartslistPos);
                }
                RefreshProdOrderPartslistPosList();
            }
            else
            {
                Msg result = null;
                try
                {
                    result = PickingManager.UnassignPickingPos(CurrentPickingPos, this.DatabaseApp);
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

                    Messages.LogException("BSOPicking", "UnassignPickingPos(30)", msg);
                }
            }

            OnPropertyChanged(nameof(PickingPosList));
            PartialQuantity = null;
        }

        /// <summary>
        /// Determines whether [is enabled unassign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled unassign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUnassignPickingPos()
        {
            if (CurrentPickingPos == null)
                return false;
            if (CurrentPickingPos.Picking == null || CurrentPickingPos.Picking.VisitorVoucher != null || CurrentPickingPos.Picking.Tourplan != null)
                return false;
            return true;
        }

        /// <summary>
        /// Creates a new picking position (line item) and adds it to the current picking order.
        /// This method generates a new PickingPos entity with default settings and configuration:
        /// - Creates a new PickingPos object using the database context and current picking order
        /// - Adds the position to the current picking order's position collection
        /// - Sets default source and target facilities from current filter selections
        /// - Initializes the delivery position load state to "NewCreated" if not already set
        /// - Sets the new position as both current and selected for immediate editing
        /// - Triggers UI property change notifications to refresh the picking positions list
        /// This method is used when users need to manually add line items to picking orders
        /// for materials that are not associated with existing purchase/sales orders.
        /// </summary>
        [ACMethodCommand("PickingPos", "en{'Add'}de{'Hinzufügen'}", 602, true,
                         Description = @"Creates a new picking position (line item) and adds it to the current picking order.
                                         This method generates a new PickingPos entity with default settings and configuration:
                                         - Creates a new PickingPos object using the database context and current picking order
                                         - Adds the position to the current picking order's position collection
                                         - Sets default source and target facilities from current filter selections
                                         - Initializes the delivery position load state to ""NewCreated"" if not already set
                                         - Sets the new position as both current and selected for immediate editing
                                         - Triggers UI property change notifications to refresh the picking positions list
                                         This method is used when users need to manually add line items to picking orders
                                         for materials that are not associated with existing purchase/sales orders.")]
        public virtual void AddPickingPos()
        {
            if (!IsEnabledAddPickingPos())
                return;
            PickingPos pickingPos = PickingPos.NewACObject(DatabaseApp, CurrentPicking);
            if (pickingPos != null)
                CurrentPicking.PickingPos_Picking.Add(pickingPos);
            OnPropertyChanged(nameof(PickingPosList));
            pickingPos.FromFacility = SelectedFilterFromFacility;
            pickingPos.ToFacility = SelectedFilterToFacility;
            if (pickingPos.MDDelivPosLoadState == null)
                pickingPos.MDDelivPosLoadState = DatabaseApp.MDDelivPosLoadState.FirstOrDefault(c => c.MDDelivPosLoadStateIndex == (short)MDDelivPosLoadState.DelivPosLoadStates.NewCreated);
            CurrentPickingPos = pickingPos;
            SelectedPickingPos = pickingPos;
        }

        /// <summary>
        /// Determines whether [is enabled AddPickingPos in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled AddPickingPos in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAddPickingPos()
        {
            if (CurrentPicking == null)
                return false;
            return true;
        }

        /// <summary>
        /// Opens a dialog to select a source facility (FromFacility) for the current picking position.
        /// Uses the BSOFacilityExplorer child component to display a facility selection dialog with the current
        /// FromFacility as the pre-selected value. If the user confirms the selection (OK), the chosen facility
        /// is assigned to the CurrentPickingPos.FromFacility property and a property change notification is triggered
        /// to update bound UI controls.
        /// </summary>
        [ACMethodInfo("ShowDlgFromFacility", "en{'Choose facility'}de{'Lager auswählen'}", 9999,
                      Description = @"Opens a dialog to select a source facility (FromFacility) for the current picking position.
                                     Uses the BSOFacilityExplorer child component to display a facility selection dialog with the current
                                     FromFacility as the pre-selected value. If the user confirms the selection (OK), the chosen facility
                                     is assigned to the CurrentPickingPos.FromFacility property and a property change notification is triggered
                                     to update bound UI controls.")]
        public void ShowDlgFromFacility()
        {
            if (!IsEnabledShowDlgFromFacility())
                return;

            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(CurrentPickingPos.FromFacility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                CurrentPickingPos.FromFacility = facility;
                OnPropertyChanged(nameof(CurrentPickingPos));
            }
        }

        /// <summary>
        /// Determines whether the source facility selection dialog can be displayed for the current picking position.
        /// Returns true if there is a current picking position available that requires source facility assignment.
        /// This method is used to enable/disable the source facility selection functionality in the user interface,
        /// allowing users to choose and assign source facilities (FromFacility) for picking position operations.
        /// </summary>
        /// <returns>True if there is a current picking position available for source facility selection; otherwise, false.</returns>
        public bool IsEnabledShowDlgFromFacility()
        {
            return CurrentPickingPos != null;
        }

        /// <summary>
        /// Opens a dialog to select a target facility (ToFacility) for the current picking position.
        /// Uses the BSOFacilityExplorer child component to display a facility selection dialog with the current
        /// ToFacility as the pre-selected value. If the user confirms the selection (OK), the chosen facility
        /// is assigned to the CurrentPickingPos.ToFacility property and a property change notification is triggered
        /// to update bound UI controls.
        /// </summary>
        [ACMethodInfo("ShowDlgToFacility", "en{'Choose facility'}de{'Lager auswählen'}", 9999,
                      Description = @"Opens a dialog to select a target facility (ToFacility) for the current picking position.
                                      Uses the BSOFacilityExplorer child component to display a facility selection dialog with the current
                                      ToFacility as the pre-selected value. If the user confirms the selection (OK), the chosen facility
                                      is assigned to the CurrentPickingPos.ToFacility property and a property change notification is triggered
                                      to update bound UI controls.")]
        public void ShowDlgToFacility()
        {
            if (!IsEnabledShowDlgToFacility())
                return;

            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(CurrentPickingPos.ToFacility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                CurrentPickingPos.ToFacility = facility;
                OnPropertyChanged(nameof(CurrentPickingPos));
            }
        }

        /// <summary>
        /// Determines whether the target facility selection dialog can be displayed for the current picking position.
        /// Returns true if there is a current picking position available that requires target facility assignment.
        /// This method is used to enable/disable the target facility selection functionality in the user interface,
        /// allowing users to choose and assign target facilities (ToFacility) for picking position operations.
        /// </summary>
        /// <returns>True if there is a current picking position available for target facility selection; otherwise, false.</returns>
        public bool IsEnabledShowDlgToFacility()
        {
            return CurrentPickingPos != null;
        }

        #endregion

        #region FacilityPreBooking

        #region FacilityPreBooking ->  PreBooking

        /// <summary>
        /// Creates a new facility pre-booking for the current picking position.
        /// This method generates a new FacilityPreBooking entity that serves as a planned posting
        /// for material movements within the picking workflow. The pre-booking contains booking
        /// parameters that define how materials will be moved between facilities during execution.
        /// The method performs the following operations:
        /// - Validates that pre-booking creation is allowed through IsEnabledNewFacilityPreBooking()
        /// - Creates a new FacilityPreBooking using the ACFacilityManager for the current picking position
        /// - Initializes booking parameters from templates using the picking position context
        /// - Updates UI property notifications to reflect the new pre-booking in related lists and controls
        /// Pre-bookings are essential for material planning and serve as templates for actual facility
        /// bookings that will be executed during the picking process. They define source/target facilities,
        /// quantities, materials, and other booking parameters required for proper inventory management.
        /// This method is typically called when users need to manually plan material movements for
        /// picking positions that require specific facility booking configurations before execution.
        /// </summary>
        [ACMethodInteraction("FacilityPreBooking", "en{'New Posting'}de{'Neue Buchung'}", (short)MISort.New, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost,
                             Description = @"Creates a new facility pre-booking for the current picking position.
                                             This method generates a new FacilityPreBooking entity that serves as a planned posting
                                             for material movements within the picking workflow. The pre-booking contains booking
                                             parameters that define how materials will be moved between facilities during execution.
                                             The method performs the following operations:
                                             - Validates that pre-booking creation is allowed through IsEnabledNewFacilityPreBooking()
                                             - Creates a new FacilityPreBooking using the ACFacilityManager for the current picking position
                                             - Initializes booking parameters from templates using the picking position context
                                             - Updates UI property notifications to reflect the new pre-booking in related lists and controls
                                             Pre-bookings are essential for material planning and serve as templates for actual facility
                                             bookings that will be executed during the picking process. They define source/target facilities,
                                             quantities, materials, and other booking parameters required for proper inventory management.
                                             This method is typically called when users need to manually plan material movements for
                                             picking positions that require specific facility booking configurations before execution.")]
        public void NewFacilityPreBooking()
        {
            if (!IsEnabledNewFacilityPreBooking())
                return;
            if (!PreExecute("NewFacilityPreBooking"))
                return;
            CurrentFacilityPreBooking = ACFacilityManager.NewFacilityPreBooking(DatabaseApp, CurrentPickingPos);
            if (CurrentFacilityPreBooking != null)
                ACFacilityManager.InitBookingParamsFromTemplate(CurrentFacilityPreBooking.ACMethodBooking as ACMethodBooking, CurrentPickingPos, CurrentFacilityPreBooking);

            OnPropertyChanged(nameof(CurrentACMethodBooking));
            OnPropertyChanged(nameof(CurrentACMethodBookingLayout));
            OnPropertyChanged(nameof(FacilityPreBookingList));
        }

        /// <summary>
        /// Determines whether a new facility pre-booking can be created for the current picking position.
        /// This method validates various conditions to ensure that pre-booking creation is appropriate:
        /// - Validates that a current picking position exists
        /// - For InOrderPos: Checks that the position is not cancelled or completed
        /// - For OutOrderPos: Checks that the position is not cancelled or completed  
        /// - For production order positions: Ensures they are in Created state
        /// - For standalone positions: Requires either FromFacility or ToFacility to be set
        /// Pre-bookings serve as planned postings that define material movement parameters before execution.
        /// </summary>
        /// <returns>True if a new facility pre-booking can be created; otherwise, false.</returns>
        public bool IsEnabledNewFacilityPreBooking()
        {
            if (CurrentPickingPos == null)
                //|| (CurrentPickingPos.InOrderPos == null && CurrentPickingPos.OutOrderPos == null && !CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any()))
                return false;
            if (CurrentPickingPos.InOrderPos != null
                && (CurrentPickingPos.InOrderPos.MDInOrderPosState.InOrderPosState == MDInOrderPosState.InOrderPosStates.Cancelled
                 || CurrentPickingPos.InOrderPos.MDInOrderPosState.InOrderPosState == MDInOrderPosState.InOrderPosStates.Completed))
                return false;
            else if (CurrentPickingPos.OutOrderPos != null
                && (CurrentPickingPos.OutOrderPos.MDOutOrderPosState.OutOrderPosState == MDOutOrderPosState.OutOrderPosStates.Cancelled
                 || CurrentPickingPos.OutOrderPos.MDOutOrderPosState.OutOrderPosState == MDOutOrderPosState.OutOrderPosStates.Completed))
                return false;
            else if (CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any())
            {
                if (
                        CurrentPickingPos.PickingPosProdOrderPartslistPos_PickingPos.Any()
                        &&
                        CurrentPickingPos
                         .PickingPosProdOrderPartslistPos_PickingPos
                         .Select(c => c.ProdorderPartslistPos)
                         .Any(c => c.MDProdOrderPartslistPosState.ProdOrderPartslistPosState != MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Created)
                    )
                    return false;
            }
            else
            {
                if (CurrentPickingPos.FromFacility == null && CurrentPickingPos.ToFacility == null)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Cancels facility pre-booking for the current picking position.
        /// This method invokes the FacilityManager to cancel existing pre-bookings associated with the current picking position,
        /// which reverts planned material movements that haven't been executed yet. If pre-bookings are successfully cancelled,
        /// the method updates the current facility pre-booking reference and refreshes related UI properties including
        /// the booking parameters layout and pre-booking list to reflect the cancellation in the user interface.
        /// </summary>
        [ACMethodInteraction("FacilityPreBooking", "en{'Cancel Posting'}de{'Buchung abbrechen'}", (short)MISort.Cancel, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost,
                             Description = @"Cancels facility pre-booking for the current picking position.
                                             This method invokes the FacilityManager to cancel existing pre-bookings associated with the current picking position,
                                             which reverts planned material movements that haven't been executed yet. If pre-bookings are successfully cancelled,
                                             the method updates the current facility pre-booking reference and refreshes related UI properties including
                                             the booking parameters layout and pre-booking list to reflect the cancellation in the user interface.")]
        public void CancelFacilityPreBooking()
        {
            if (!IsEnabledCancelFacilityPreBooking())
                return;
            if (!PreExecute("CancelFacilityPreBooking"))
                return;
            var result = ACFacilityManager.CancelFacilityPreBooking(DatabaseApp, CurrentPickingPos);
            if (result != null && result.Any())
            {
                CurrentFacilityPreBooking = result.FirstOrDefault();
                OnPropertyChanged(nameof(CurrentACMethodBooking));
                OnPropertyChanged(nameof(CurrentACMethodBookingLayout));
                OnPropertyChanged(nameof(FacilityPreBookingList));
            }
        }

        /// <summary>
        /// Determines whether facility pre-booking cancellation is enabled for the current picking position.
        /// Returns false if the current picking position is null, has no associated order positions 
        /// (InOrderPos or OutOrderPos), has existing facility pre-bookings, or if the associated 
        /// order positions are in a cancelled state.
        /// </summary>
        /// <returns>
        /// True if facility pre-booking cancellation is enabled; otherwise, false.
        /// </returns>
        public bool IsEnabledCancelFacilityPreBooking()
        {
            if (CurrentPickingPos == null
                || (CurrentPickingPos.InOrderPos == null && CurrentPickingPos.OutOrderPos == null)
                || (FacilityPreBookingList != null && FacilityPreBookingList.Any()))
                return false;
            if (CurrentPickingPos == null || (CurrentPickingPos.InOrderPos == null && CurrentPickingPos.OutOrderPos == null))
                return false;
            if (CurrentPickingPos.InOrderPos != null
                && CurrentPickingPos.InOrderPos.MDInOrderPosState.InOrderPosState == MDInOrderPosState.InOrderPosStates.Cancelled)
                return false;
            else if (CurrentPickingPos.OutOrderPos != null
                && CurrentPickingPos.OutOrderPos.MDOutOrderPosState.OutOrderPosState == MDOutOrderPosState.OutOrderPosStates.Cancelled)
                return false;
            return true;
        }

        /// <summary>
        /// Deletes the current facility pre-booking from the database after validation.
        /// This method performs the following operations:
        /// - Validates that deletion is allowed through IsEnabledDeleteFacilityPreBooking()
        /// - Executes pre-processing checks via PreExecute()
        /// - Calls the overloaded DeleteFacilityPreBooking method with the current facility pre-booking
        /// - Handles the deletion of planned facility booking parameters and updates related UI properties
        /// Use this method to remove unwanted or incorrect facility pre-bookings before they are executed.
        /// The deletion only affects pre-bookings that haven't been converted to actual facility bookings yet.
        /// </summary>
        [ACMethodInteraction("FacilityPreBooking", "en{'Delete Posting'}de{'Lösche Buchung'}", (short)MISort.Delete, true, "SelectedPickingPos", Global.ACKinds.MSMethodPrePost,
                             Description = @"Deletes the current facility pre-booking from the database after validation.
                                             This method performs the following operations:
                                             - Validates that deletion is allowed through IsEnabledDeleteFacilityPreBooking()
                                             - Executes pre-processing checks via PreExecute()
                                             - Calls the overloaded DeleteFacilityPreBooking method with the current facility pre-booking
                                             - Handles the deletion of planned facility booking parameters and updates related UI properties
                                             Use this method to remove unwanted or incorrect facility pre-bookings before they are executed.
                                             The deletion only affects pre-bookings that haven't been converted to actual facility bookings yet.")]
        public void DeleteFacilityPreBooking()
        {
            if (!IsEnabledDeleteFacilityPreBooking())
                return;
            if (!PreExecute(nameof(DeleteFacilityPreBooking)))
                return;
            DeleteFacilityPreBooking(CurrentFacilityPreBooking);
        }

        /// <summary>
        /// Deletes the specified facility pre-booking from the database after performing validation checks.
        /// This method handles the complete deletion workflow including error handling and UI property updates.
        /// The method performs the following operations:
        /// - Attempts to delete the facility pre-booking entity using DeleteACObject with validation enabled
        /// - Displays any error messages that occur during the deletion process
        /// - Sets the CurrentFacilityPreBooking to null upon successful deletion
        /// - Triggers property change notification to update the FacilityPreBookingList in the UI
        /// This method is used to remove unwanted or incorrect facility pre-bookings before they are executed,
        /// ensuring proper cleanup of planned material movements that are no longer needed.
        /// </summary>
        /// <param name="CurrentFacilityPreBooking">The facility pre-booking entity to be deleted from the database</param>
        public void DeleteFacilityPreBooking(FacilityPreBooking CurrentFacilityPreBooking)
        {
            Msg msg = CurrentFacilityPreBooking.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            else
            {
                CurrentFacilityPreBooking = null;
                OnPropertyChanged(nameof(FacilityPreBookingList));
            }
        }

        /// <summary>
        /// Determines whether the current facility pre-booking can be deleted.
        /// Returns true if there is a current facility pre-booking available for deletion.
        /// This method is used to enable/disable the delete facility pre-booking functionality in the user interface.
        /// The deletion removes planned facility booking parameters that haven't been converted to actual facility bookings yet.
        /// </summary>
        /// <returns>True if there is a current facility pre-booking that can be deleted; otherwise, false.</returns>
        public bool IsEnabledDeleteFacilityPreBooking()
        {
            return CurrentFacilityPreBooking != null;
        }

        /// <summary>
        /// Posts/books the current delivery position item for processing.
        /// This method handles the booking of delivery position items within the picking workflow.
        /// Executes pre and post processing steps to ensure proper booking validation and completion.
        /// Used for individual position-level booking operations that require specific handling
        /// separate from bulk booking processes.
        /// </summary>
        [ACMethodInteraction("FacilityPreBooking", "en{'Post Item'}de{'Buche Position'}", (short)MISort.New, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost,
                             Description = @"Posts/books the current delivery position item for processing.
                                             This method handles the booking of delivery position items within the picking workflow.
                                             Executes pre and post processing steps to ensure proper booking validation and completion.
                                             Used for individual position-level booking operations that require specific handling
                                             separate from bulk booking processes.")]
        public void BookDeliveryPos()
        {
            if (!PreExecute(nameof(BookDeliveryPos))) return;
            PostExecute(nameof(BookDeliveryPos));
        }

        /// <summary>
        /// Determines whether posting/booking of delivery positions is enabled.
        /// This method validates if the current picking position and related booking parameters
        /// are in a valid state to allow posting operations. Returns true to indicate that
        /// delivery position posting is currently enabled for the picking workflow.
        /// </summary>
        /// <returns>True if delivery position posting is enabled; otherwise, false.</returns>
        public bool IsEnabledBookDeliveryPos()
        {
            return true;
        }

        /// <summary>
        /// Books/posts the current ACMethodBooking for facility operations within the picking workflow.
        /// This method executes facility booking operations for the current picking position using the associated
        /// ACMethodBooking parameters and FacilityPreBooking configuration. It validates that booking is enabled,
        /// performs the actual facility booking through the BookACMethodBooking method, and handles post-execution
        /// cleanup and status updates.
        /// The booking process includes:
        /// - Validation of booking prerequisites through IsEnabledBookCurrentACMethodBooking()
        /// - Execution of facility booking with automatic quantity-to-zero checking for consumed stock
        /// - Post-processing operations including facility pre-booking cleanup and status updates
        /// - Integration with the picking workflow execution lifecycle
        /// This method is typically called when users confirm facility booking operations in the picking interface,
        /// converting planned facility movements (pre-bookings) into actual facility transactions.
        /// </summary>
        [ACMethodInteraction("FacilityPreBooking", "en{'Post'}de{'Buchen'}", (short)MISort.New, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost,
                             Description = @"Books/posts the current ACMethodBooking for facility operations within the picking workflow.
                                             This method executes facility booking operations for the current picking position using the associated
                                             ACMethodBooking parameters and FacilityPreBooking configuration. It validates that booking is enabled,
                                             performs the actual facility booking through the BookACMethodBooking method, and handles post-execution
                                             cleanup and status updates.
                                             The booking process includes:
                                             - Validation of booking prerequisites through IsEnabledBookCurrentACMethodBooking()
                                             - Execution of facility booking with automatic quantity-to-zero checking for consumed stock
                                             - Post-processing operations including facility pre-booking cleanup and status updates
                                             - Integration with the picking workflow execution lifecycle
                                             This method is typically called when users confirm facility booking operations in the picking interface,
                                             converting planned facility movements (pre-bookings) into actual facility transactions.")]
        public void BookCurrentACMethodBooking()
        {
            if (!IsEnabledBookCurrentACMethodBooking())
                return;

            BookACMethodBooking(CurrentPickingPos, CurrentACMethodBooking, CurrentFacilityPreBooking, SetQuantToZeroMode.CheckIfZeroAskUser);
            PostExecute(nameof(BookCurrentACMethodBooking));
        }

        public enum SetQuantToZeroMode
        {
            Off = 0,
            CheckIfZeroAskUser = 1,
            Force = 2
        }

        /// <summary>
        /// Books/executes an ACMethodBooking for facility operations within the picking workflow.
        /// This method performs the core facility booking operation that converts planned facility movements 
        /// (pre-bookings) into actual facility transactions with comprehensive validation and error handling.
        /// The method handles the following operations:
        /// - Synchronizes booking parameters with picking position order references (InOrderPos/OutOrderPos)
        /// - Synchronizes facility booking entity properties with ACMethodBooking parameters
        /// - Validates booking parameters and saves pending changes before execution
        /// - Executes the facility booking through FacilityManager with result validation
        /// - Handles successful bookings by cleaning up pre-bookings and recalculating quantities
        /// - Optionally manages zero-stock scenarios with user confirmation or forced execution
        /// - Performs zero-stock facility charge bookings when stock is fully consumed
        /// Zero-stock handling modes:
        /// - Off: No automatic zero-stock processing
        /// - CheckIfZeroAskUser: Prompts user when stock would be consumed completely
        /// - Force: Automatically sets consumed facility charges to not available
        /// </summary>
        /// <param name="currentPickingPos">The picking position being processed for booking</param>
        /// <param name="currentACMethodBooking">The booking method containing all booking parameters and validation logic</param>
        /// <param name="currentFacilityPreBooking">The pre-booking to be converted into an actual booking</param>
        /// <param name="autoSetQuantToZero">Controls automatic zero-stock handling behavior</param>
        /// <returns>True if booking was successful and all related operations completed; false if booking failed or was cancelled</returns>
        public virtual bool BookACMethodBooking(PickingPos currentPickingPos, ACMethodBooking currentACMethodBooking, FacilityPreBooking currentFacilityPreBooking, SetQuantToZeroMode autoSetQuantToZero)
        {
            if (currentPickingPos.OutOrderPos != null)
            {
                if (currentACMethodBooking.OutOrderPos != currentPickingPos.OutOrderPos)
                    currentACMethodBooking.OutOrderPos = currentPickingPos.OutOrderPos;
                if (currentPickingPos.OutOrderPos.OutOrder.CPartnerCompany != null && currentACMethodBooking.CPartnerCompany != currentPickingPos.OutOrderPos.OutOrder.CPartnerCompany)
                    currentACMethodBooking.CPartnerCompany = currentPickingPos.OutOrderPos.OutOrder.CPartnerCompany;
            }
            else if (currentPickingPos.InOrderPos != null)
            {
                if (currentACMethodBooking.InOrderPos != currentPickingPos.InOrderPos)
                    currentACMethodBooking.InOrderPos = currentPickingPos.InOrderPos;
                if (currentPickingPos.InOrderPos.InOrder.CPartnerCompany != null && currentACMethodBooking.CPartnerCompany != currentPickingPos.InOrderPos.InOrder.CPartnerCompany)
                    currentACMethodBooking.CPartnerCompany = currentPickingPos.InOrderPos.InOrder.CPartnerCompany;
            }

            if (currentACMethodBooking.FacilityBooking != null)
            {
                if (currentACMethodBooking.FacilityBooking.OutOrderPos != currentACMethodBooking.OutOrderPos)
                    currentACMethodBooking.FacilityBooking.OutOrderPos = currentACMethodBooking.OutOrderPos;

                if (currentACMethodBooking.FacilityBooking.InOrderPos != currentACMethodBooking.InOrderPos)
                    currentACMethodBooking.FacilityBooking.InOrderPos = currentACMethodBooking.InOrderPos;

                if (currentACMethodBooking.FacilityBooking.PickingPos != currentACMethodBooking.PickingPos)
                    currentACMethodBooking.FacilityBooking.PickingPos = currentACMethodBooking.PickingPos;

                if (currentACMethodBooking.FacilityBooking.InwardMaterial != currentACMethodBooking.InwardMaterial)
                    currentACMethodBooking.FacilityBooking.InwardMaterial = currentACMethodBooking.InwardMaterial;

                if (currentACMethodBooking.FacilityBooking.OutwardMaterial != currentACMethodBooking.OutwardMaterial)
                    currentACMethodBooking.FacilityBooking.OutwardMaterial = currentACMethodBooking.OutwardMaterial;

                if (currentACMethodBooking.FacilityBooking.OutwardFacility != currentACMethodBooking.OutwardFacility)
                    currentACMethodBooking.FacilityBooking.OutwardFacility = currentACMethodBooking.OutwardFacility;

                if (currentACMethodBooking.FacilityBooking.InwardFacility != currentACMethodBooking.InwardFacility)
                    currentACMethodBooking.FacilityBooking.InwardFacility = currentACMethodBooking.InwardFacility;
            }

            OnValidateBookingParams(currentACMethodBooking);

            bool isCancellation = currentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel || currentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel;

            Save();
            if (DatabaseApp.IsChanged)
                return false;
            if (!PreExecute(nameof(BookCurrentACMethodBooking)))
                return false;

            currentACMethodBooking.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(currentACMethodBooking, this.DatabaseApp) as ACMethodEventArgs;
            if (!currentACMethodBooking.ValidMessage.IsSucceded() || currentACMethodBooking.ValidMessage.HasWarnings())
            {
                Messages.Msg(currentACMethodBooking.ValidMessage);
                return false;
            }
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
                OnPropertyChanged(nameof(FacilityBookingList));
                return false;
            }
            else
            {
                double changedQuantity = 0;
                FacilityCharge outwardFC = null;

                if (currentACMethodBooking != null)
                {
                    if (currentACMethodBooking.OutwardQuantity.HasValue)
                        changedQuantity = currentACMethodBooking.OutwardQuantity.Value;
                    else if (currentACMethodBooking.InwardQuantity.HasValue)
                        changedQuantity = currentACMethodBooking.InwardQuantity.Value;

                    outwardFC = currentACMethodBooking.OutwardFacilityCharge;
                }

                DeleteFacilityPreBooking(currentFacilityPreBooking);
                OnPropertyChanged(nameof(FacilityBookingList));
                ACFacilityManager.RecalcAfterPosting(DatabaseApp, currentPickingPos, changedQuantity, isCancellation, true);
                Save();

                Msg msg = null;
                if (autoSetQuantToZero == SetQuantToZeroMode.CheckIfZeroAskUser)
                    msg = ACFacilityManager.IsQuantStockConsumed(outwardFC, DatabaseApp);
                if (autoSetQuantToZero == SetQuantToZeroMode.Force || msg != null)
                {
                    MsgResult msgResult = MsgResult.No;
                    if (msg != null)
                        msgResult = Messages.Question(this, msg.Message, MsgResult.No, true);
                    if (autoSetQuantToZero == SetQuantToZeroMode.Force || msgResult == MsgResult.Yes)
                    {
                        if (ACFacilityManager == null)
                            return false;

                        ACMethodBooking fbtZeroBooking = PickingManager.BookParamZeroStockFacilityChargeClone(ACFacilityManager, DatabaseApp);
                        ACMethodBooking fbtZeroBookingClone = fbtZeroBooking.Clone() as ACMethodBooking;

                        fbtZeroBookingClone.InwardFacilityCharge = outwardFC;
                        fbtZeroBookingClone.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);

                        fbtZeroBookingClone.AutoRefresh = true;
                        ACMethodEventArgs resultZeroBook = ACFacilityManager.BookFacility(fbtZeroBookingClone, this.DatabaseApp);
                        if (!fbtZeroBookingClone.ValidMessage.IsSucceded() || fbtZeroBookingClone.ValidMessage.HasWarnings())
                        {
                            Messages.Msg(currentACMethodBooking.ValidMessage);
                            return false;
                        }
                        else if (resultZeroBook.ResultState == Global.ACMethodResultState.Failed || resultZeroBook.ResultState == Global.ACMethodResultState.Notpossible)
                        {
                            if (String.IsNullOrEmpty(result.ValidMessage.Message))
                                result.ValidMessage.Message = result.ResultState.ToString();
                            Messages.Msg(result.ValidMessage);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether the current ACMethodBooking can be executed for facility booking operations.
        /// This method validates the booking parameters and ensures that all required conditions are met
        /// before allowing the booking to proceed. It performs the following checks:
        /// - Verifies that no dummy booking method is being used
        /// - Validates booking parameters through OnValidateBookingParams
        /// - Checks if the ACMethodBooking is enabled for execution
        /// - Updates the BSO message display with any validation results
        /// Returns false if validation fails or if essential booking components are missing.
        /// </summary>
        /// <returns>True if the current ACMethodBooking can be executed; otherwise, false.</returns>
        public bool IsEnabledBookCurrentACMethodBooking()
        {
            if (_CurrentACMethodBookingDummy != null)
                return false;
            bool bRetVal = false;
            if (CurrentACMethodBooking != null)
            {
                OnValidateBookingParams(CurrentACMethodBooking);
                bRetVal = CurrentACMethodBooking.IsEnabled();
            }
            else
                return false;
            UpdateBSOMsg();
            return bRetVal;
        }

        protected virtual void OnValidateBookingParams(ACMethodBooking currentACMethodBooking)
        {
            if (currentACMethodBooking == null)
                return;
            if (currentACMethodBooking.BookingType < GlobalApp.FacilityBookingType.InOrderPosInwardMovement
                || currentACMethodBooking.BookingType > GlobalApp.FacilityBookingType.PickingOutwardCancel)
            {
                if (currentACMethodBooking.InwardQuantity.HasValue && !currentACMethodBooking.OutwardQuantity.HasValue)
                    currentACMethodBooking.OutwardQuantity = currentACMethodBooking.InwardQuantity;
                else if (!currentACMethodBooking.InwardQuantity.HasValue && currentACMethodBooking.OutwardQuantity.HasValue)
                    currentACMethodBooking.InwardQuantity = currentACMethodBooking.OutwardQuantity;
                else if (currentACMethodBooking.InwardQuantity != currentACMethodBooking.OutwardQuantity)
                    currentACMethodBooking.InwardQuantity = currentACMethodBooking.OutwardQuantity;
            }
        }

        /// <summary>
        /// Books all ACMethodBookings associated with the current picking position's pre-bookings.
        /// This method iterates through all facility pre-bookings in the FacilityPreBookingList and executes
        /// the booking operation for each one by calling BookCurrentACMethodBooking().
        /// The method performs the following operations:
        /// - Validates that booking is enabled through IsEnabledBookAllACMethodBookings()
        /// - Iterates through each FacilityPreBooking in the current picking position's pre-booking list
        /// - Sets each pre-booking as the selected/current pre-booking for processing
        /// - Calls BookCurrentACMethodBooking() to execute the actual facility booking operation
        /// This provides a batch processing capability to convert all planned facility movements (pre-bookings)
        /// into actual facility transactions in a single operation, streamlining the picking workflow
        /// when multiple bookings need to be processed simultaneously for a picking position.
        /// Use this method when you want to execute all prepared facility bookings for the current
        /// picking position at once, rather than booking them individually.
        /// </summary>
        [ACMethodInfo(nameof(BookAllACMethodBookings), "en{'Post All'}de{'Buche alle'}", 101, true,
                      Description = @"Books all ACMethodBookings associated with the current picking position's pre-bookings.
                                      This method iterates through all facility pre-bookings in the FacilityPreBookingList and executes
                                      the booking operation for each one by calling BookCurrentACMethodBooking().
                                      The method performs the following operations:
                                      - Validates that booking is enabled through IsEnabledBookAllACMethodBookings()
                                      - Iterates through each FacilityPreBooking in the current picking position's pre-booking list
                                      - Sets each pre-booking as the selected/current pre-booking for processing
                                      - Calls BookCurrentACMethodBooking() to execute the actual facility booking operation
                                      This provides a batch processing capability to convert all planned facility movements (pre-bookings)
                                      into actual facility transactions in a single operation, streamlining the picking workflow
                                      when multiple bookings need to be processed simultaneously for a picking position.
                                      Use this method when you want to execute all prepared facility bookings for the current
                                      picking position at once, rather than booking them individually.")]
        public void BookAllACMethodBookings()
        {
            if (!IsEnabledBookAllACMethodBookings())
                return;
            foreach (FacilityPreBooking facilityPreBooking in FacilityPreBookingList.ToList())
            {
                SelectedFacilityPreBooking = facilityPreBooking;
                if (CurrentFacilityPreBooking == facilityPreBooking)
                    BookCurrentACMethodBooking();
            }
        }

        /// <summary>
        /// Determines whether all facility pre-bookings can be executed for booking operations.
        /// This method validates if the current picking position exists, has facility pre-bookings available,
        /// and if all pre-bookings are in a valid state for batch booking execution.
        /// Returns false if there is no current picking position or if the facility pre-booking list
        /// is null or empty, preventing batch booking operations from proceeding.
        /// </summary>
        /// <returns>True if all facility pre-bookings can be executed; otherwise, false.</returns>
        public bool IsEnabledBookAllACMethodBookings()
        {
            if (CurrentPickingPos == null || FacilityPreBookingList == null || !FacilityPreBookingList.Any())
                return false;
            return true;
        }

        /// <summary>
        /// Books all ACMethodBookings for all picking positions in the current picking order.
        /// This method iterates through all picking positions in the PickingPosList and processes
        /// any available facility pre-bookings for each position by calling BookAllACMethodBookings().
        /// The method performs batch booking operations across all positions in the picking order,
        /// converting planned facility movements (pre-bookings) into actual facility transactions.
        /// This provides a comprehensive booking capability to execute all prepared facility bookings
        /// for the entire picking order in a single operation, streamlining the picking workflow
        /// when multiple positions need to be processed simultaneously.
        /// Use this method when you want to execute all prepared facility bookings for all
        /// picking positions at once, rather than booking them individually per position.
        /// </summary>
        [ACMethodInfo(nameof(BookAllPositions), "en{'Post all positions'}de{'Buche alle line'}", 102, true,
                      Description = @"Books all ACMethodBookings for all picking positions in the current picking order.
                                     This method iterates through all picking positions in the PickingPosList and processes
                                     any available facility pre-bookings for each position by calling BookAllACMethodBookings().
                                     The method performs batch booking operations across all positions in the picking order,
                                     converting planned facility movements (pre-bookings) into actual facility transactions.
                                     This provides a comprehensive booking capability to execute all prepared facility bookings
                                     for the entire picking order in a single operation, streamlining the picking workflow
                                     when multiple positions need to be processed simultaneously.
                                     Use this method when you want to execute all prepared facility bookings for all
                                     picking positions at once, rather than booking them individually per position.")]
        public void BookAllPositions()
        {
            if (!IsEnabledBookAllPositions())
                return;
            PickingPos[] lines = PickingPosList.ToArray();
            foreach (PickingPos pos in lines)
            {
                CurrentPickingPos = pos;
                if (IsEnabledBookAllACMethodBookings())
                {
                    BookAllACMethodBookings();
                }
            }
        }

        /// <summary>
        /// Determines whether all picking positions can be executed for booking operations.
        /// This method validates if there are picking positions available in the current picking order
        /// that can be processed for facility booking operations. It checks for the existence of
        /// PickingPosList and validates that the PickingList contains at least one entry.
        /// Used to enable/disable the "Post all positions" functionality in the user interface.
        /// </summary>
        /// <returns>True if there are picking positions that can be booked; otherwise, false.</returns>
        public bool IsEnabledBookAllPositions()
        {
            return PickingPosList != null && PickingList.Any();
        }

        /// <summary>
        /// Creates a new facility lot (batch/charge) for material management in the picking workflow.
        /// This method opens a facility lot dialog that allows users to create new lot records for lot-managed materials
        /// during picking operations. The dialog is modal and handles the complete lot creation process including
        /// validation and assignment to the current picking position material context.
        /// The method performs the following operations:
        /// - Validates that lot creation is enabled through IsEnabledNewFacilityLot()
        /// - Starts the FacilityLotDialog child component if not already running
        /// - Opens the ShowDialogNewLot dialog with the current picking position material as context
        /// - Processes the dialog result through dlgResult_OnDialogResult callback method
        /// - Automatically stops the dialog component after completion
        /// This functionality is essential for lot-managed materials where specific batch information
        /// needs to be tracked throughout the picking and material movement processes. The created
        /// facility lot can then be assigned to booking operations for proper traceability and
        /// inventory management in warehouse operations.
        /// The dialog provides user interface for entering lot-specific information such as:
        /// - Lot number and external lot number
        /// - Production and expiration dates
        /// - Quality and storage information
        /// - Other lot-specific attributes required for material tracking
        /// </summary>
        [ACMethodInfo("Dialog", "en{'New Lot'}de{'Neues Los'}", (short)MISort.New,
                      Description = @"Creates a new facility lot (batch/charge) for material management in the picking workflow.
                                     This method opens a facility lot dialog that allows users to create new lot records for lot-managed materials
                                     during picking operations. The dialog is modal and handles the complete lot creation process including
                                     validation and assignment to the current picking position material context.
                                     The method performs the following operations:
                                     - Validates that lot creation is enabled through IsEnabledNewFacilityLot()
                                     - Starts the FacilityLotDialog child component if not already running
                                     - Opens the ShowDialogNewLot dialog with the current picking position material as context
                                     - Processes the dialog result through dlgResult_OnDialogResult callback method
                                     - Automatically stops the dialog component after completion
                                     This functionality is essential for lot-managed materials where specific batch information
                                     needs to be tracked throughout the picking and material movement processes. The created
                                     facility lot can then be assigned to booking operations for proper traceability and
                                     inventory management in warehouse operations.
                                     The dialog provides user interface for entering lot-specific information such as:
                                     - Lot number and external lot number
                                     - Production and expiration dates
                                     - Quality and storage information
                                     - Other lot-specific attributes required for material tracking")]
        public void NewFacilityLot()
        {
            if (!IsEnabledNewFacilityLot())
                return;
            ACComponent childBSO = ACUrlCommand("?FacilityLotDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("FacilityLotDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!ShowDialogNewLot", "", CurrentPickingPos.Material);
            dlgResult_OnDialogResult(dlgResult);
            childBSO.Stop();
        }

        void dlgResult_OnDialogResult(VBDialogResult dlgResult)
        {
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                FacilityLot result = dlgResult.ReturnValue as FacilityLot;
                if (result != null)
                {
                    Save();
                    CurrentACMethodBooking.InwardFacilityLot = result;
                    OnNewCreatedFacilityLot(result);
                    if (AccessBookingFacilityLot != null)
                        AccessBookingFacilityLot.NavSearch(DatabaseApp);
                    Save();
                }
            }
        }


        /// <summary>
        /// Virtual method called when a new facility lot is successfully created through the facility lot dialog.
        /// This method serves as an extension point for derived classes to implement custom logic
        /// that should be executed after a new facility lot has been created and assigned to the current booking.
        /// The method is called in the dlgResult_OnDialogResult callback after:
        /// - The facility lot dialog returns with OK result
        /// - The facility lot is saved to the database  
        /// - The lot is assigned to CurrentACMethodBooking.InwardFacilityLot
        /// - The facility lot access navigation is refreshed
        /// - Changes are saved to the database 
        /// Common use cases for overriding this method include:
        /// - Logging facility lot creation events
        /// - Triggering additional business logic workflows
        /// - Updating related entities or calculations
        /// - Sending notifications about new lot creation
        /// - Performing validation or compliance checks
        /// - Initializing lot-specific configurations
        /// </summary>
        /// <param name="lot">The newly created facility lot that was added to the system</param>
        public virtual void OnNewCreatedFacilityLot(FacilityLot lot)
        {
        }

        /// <summary>
        /// Determines whether a new facility lot can be created for the current picking position.
        /// Returns true if the current booking is for an inward movement, no lot is already assigned,
        /// the picking position and its material exist, and the material is lot-managed.
        /// Only enabled for receipt operations where lot tracking is required.
        /// </summary>
        public bool IsEnabledNewFacilityLot()
        {
            // Nur bei Wareneingängen kann Charge ausgewählt werden
            return CurrentACMethodBooking != null
                && CurrentACMethodBooking.InwardFacilityLot == null
                && CurrentPickingPos != null
                && (CurrentACMethodBooking.InOrderPos != null
                    || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosInwardMovement
                    || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel
                    || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.PickingInward)
                && CurrentPickingPos.Material != null
                && CurrentPickingPos.Material.IsLotManaged;
        }

        /// <summary>
        /// Opens a dialog to display details of the facility lot (batch/charge) associated with the current booking.
        /// If an outward or inward facility lot is assigned in the current booking, this method launches the FacilityLotDialog
        /// to show the lot information. The dialog is only shown if a lot is present; otherwise, the method returns without action.
        /// </summary>
        [ACMethodInfo("Dialog", "en{'Show Lot'}de{'Los anzeigen'}", (short)MISort.New + 1,
                      Description = @"Opens a dialog to display details of the facility lot (batch/charge) associated with the current booking.
                                      If an outward or inward facility lot is assigned in the current booking, this method launches the FacilityLotDialog
                                      to show the lot information. The dialog is only shown if a lot is present; otherwise, the method returns without action.")]
        public void ShowFacilityLot()
        {
            if (!IsEnabledShowFacilityLot())
                return;
            ACComponent childBSO = ACUrlCommand("?FacilityLotDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("FacilityLotDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            string lotNo = null;
            if (CurrentACMethodBooking.OutwardFacilityLot != null)
                lotNo = CurrentACMethodBooking.OutwardFacilityLot.LotNo;
            else if (CurrentACMethodBooking.InwardFacilityLot != null)
                lotNo = CurrentACMethodBooking.InwardFacilityLot.LotNo;
            else
                return;
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!ShowDialogOrder", lotNo);
            childBSO.Stop();
        }

        /// <summary>
        /// Determines whether the facility lot dialog can be shown for the current booking.
        /// Returns true if the current booking has either an inward or outward facility lot assigned.
        /// Used to enable/disable the "Show Lot" dialog functionality in the UI.
        public bool IsEnabledShowFacilityLot()
        {
            return CurrentACMethodBooking != null
                    && (CurrentACMethodBooking.InwardFacilityLot != null
                        || CurrentACMethodBooking.OutwardFacilityLot != null);
        }

        #endregion

        #region FacilityPreBooking -> Available quants

        /// <summary>
        /// Opens a dialog to select an available quant (FacilityCharge) for inward pre-booking.
        /// This method checks if the dialog can be shown, sets the context to inward, loads the material for pre-booking,
        /// resets the available quants list, and displays the "DlgAvailableQuants" dialog for user selection.
        /// </summary>
        [ACMethodInfo("ShowDlgInwardAvailableQuants", "en{'Choose quant'}de{'Quant auswählen'}", 9999,
                      Description = @"Opens a dialog to select an available quant (FacilityCharge) for inward pre-booking.
                                      This method checks if the dialog can be shown, sets the context to inward, loads the material for pre-booking,
                                      resets the available quants list, and displays the ""DlgAvailableQuants"" dialog for user selection.")]
        public void ShowDlgInwardAvailableQuants()
        {
            if (!IsEnabledShowDlgInwardAvailableQuants())
                return;
            _IsInward = true;
            _QuantDialogMaterial = GetPreBookingInwardMaterial();
            _PreBookingAvailableQuantsList = null;
            ShowDialog(this, "DlgAvailableQuants");
        }

        /// <summary>
        /// Determines whether the inward available quants dialog can be displayed for the current booking.
        /// Returns true if the current ACMethodBooking exists and the inward material for pre-booking is available,
        /// allowing users to select facility charges for inward material movements in the picking workflow.
        /// </summary>
        /// <returns>True if the inward available quants dialog can be shown; otherwise, false.</returns>
        public bool IsEnabledShowDlgInwardAvailableQuants()
        {
            return CurrentACMethodBooking != null && GetPreBookingInwardMaterial() != null;
        }

        /// <summary>
        /// Opens a dialog to select an available quant (FacilityCharge) for outward pre-booking.
        /// This method checks if the dialog can be shown, sets the context to outward, loads the material for pre-booking,
        /// resets the available quants list, and displays the "DlgAvailableQuants" dialog for user selection.
        /// </summary>
        [ACMethodInfo("ShowDlgOutwardAvailableQuants", "en{'Choose quant'}de{'Quant auswählen'}", 9999,
                      Description = @"Opens a dialog to select an available quant (FacilityCharge) for outward pre-booking.
                                      This method checks if the dialog can be shown, sets the context to outward, loads the material for pre-booking,
                                      resets the available quants list, and displays the ""DlgAvailableQuants"" dialog for user selection.")]
        public void ShowDlgOutwardAvailableQuants()
        {
            if (!IsEnabledShowDlgOutwardAvailableQuants())
                return;
            _IsInward = false;
            _QuantDialogMaterial = GetPreBookingOutwardMaterial();
            _PreBookingAvailableQuantsList = null;
            ShowDialog(this, "DlgAvailableQuants");
        }

        /// <summary>
        /// Determines whether the outward available quants dialog can be displayed for the current booking.
        /// Returns true if the current ACMethodBooking exists and the outward material for pre-booking is available,
        /// allowing users to select facility charges for outward material movements in the picking workflow.
        /// </summary>
        /// <returns>True if the outward available quants dialog can be shown; otherwise, false.</returns>
        public bool IsEnabledShowDlgOutwardAvailableQuants()
        {
            return CurrentACMethodBooking != null && GetPreBookingOutwardMaterial() != null;
        }

        /// <summary>
        /// Confirms the selection of an available facility charge (quant) from the pre-booking available quants dialog.
        /// Updates the current ACMethodBooking with the selected facility charge details, setting either inward or outward
        /// facility and charge based on the dialog context (_IsInward flag). Clears associated material and lot references
        /// to ensure the booking uses the selected quant. Triggers property change notification and closes the dialog.
        /// </summary>
        [ACMethodInfo("DlgAvailableQuantsOk", Const.Ok, 9999,
                      Description = @"Confirms the selection of an available facility charge (quant) from the pre-booking available quants dialog.
                                      Updates the current ACMethodBooking with the selected facility charge details, setting either inward or outward
                                      facility and charge based on the dialog context (_IsInward flag). Clears associated material and lot references
                                      to ensure the booking uses the selected quant. Triggers property change notification and closes the dialog.")]
        public void DlgAvailableQuantsOk()
        {
            if (!IsEnabledDlgAvailableQuantsOk())
                return;
            if (_IsInward)
            {
                CurrentACMethodBooking.InwardFacility = SelectedPreBookingAvailableQuants.Facility;
                CurrentACMethodBooking.InwardFacilityCharge = SelectedPreBookingAvailableQuants;
                CurrentACMethodBooking.InwardMaterial = null;
                CurrentACMethodBooking.InwardFacilityLot = null;
            }
            else
            {
                CurrentACMethodBooking.OutwardFacility = SelectedPreBookingAvailableQuants.Facility;
                CurrentACMethodBooking.OutwardFacilityCharge = SelectedPreBookingAvailableQuants;
                CurrentACMethodBooking.OutwardMaterial = null;
                CurrentACMethodBooking.OutwardFacilityLot = null;
            }
            OnPropertyChanged(nameof(CurrentACMethodBooking));
            CloseTopDialog();
        }

        /// <summary>
        /// Determines whether the "OK" button in the available quants dialog can be enabled.
        /// Returns true if a facility charge (quant) has been selected from the available pre-booking quantities list.
        /// </summary>
        /// <returns>True if a quant is selected; otherwise, false.</returns>
        public bool IsEnabledDlgAvailableQuantsOk()
        {
            return SelectedPreBookingAvailableQuants != null;
        }

        /// <summary>
        /// Cancels the selection of available facility charges (quants) and closes the dialog without applying any changes.
        /// This method is called when the user chooses to cancel the quant selection process in the available quants dialog,
        /// dismissing the dialog and returning to the previous state without updating any facility booking parameters.
        /// </summary>
        [ACMethodInfo("DlgAvailableQuantsCancel", "en{'Close'}de{'Schließen'}", 9999,
                      Description = @"Cancels the selection of available facility charges (quants) and closes the dialog without applying any changes.
                                      This method is called when the user chooses to cancel the quant selection process in the available quants dialog,
                                      dismissing the dialog and returning to the previous state without updating any facility booking parameters.")]
        public void DlgAvailableQuantsCancel()
        {
            CloseTopDialog();
        }

        private Material GetPreBookingOutwardMaterial()
        {
            if (CurrentACMethodBooking != null && CurrentPickingPos != null && CurrentPickingPos.Material != null)
                return CurrentPickingPos.Material;
            return null;
        }

        private Material GetPreBookingInwardMaterial()
        {
            if (CurrentACMethodBooking != null && CurrentPickingPos != null && CurrentPickingPos.Material != null)
                return CurrentPickingPos.Material;
            return null;
        }

        #endregion

        #region FacilityPreBooking -> Select Facility

        /// <summary>
        /// Opens a dialog to select the inward facility for facility pre-booking operations.
        /// This method sets the facility selection context to inward pre-booking and displays
        /// the facility explorer dialog with the current inward facility pre-selected.
        /// If the user confirms the selection, the chosen facility is assigned to the
        /// CurrentACMethodBooking.InwardFacility property for subsequent booking operations.
        /// </summary>
        [ACMethodInfo("ShowDlgInwardFacility", "en{'Choose facility'}de{'Lager auswählen'}", 9999,
                      Description = @"Opens a dialog to select the inward facility for facility pre-booking operations.
                                      This method sets the facility selection context to inward pre-booking and displays
                                      the facility explorer dialog with the current inward facility pre-selected.
                                      If the user confirms the selection, the chosen facility is assigned to the
                                      CurrentACMethodBooking.InwardFacility property for subsequent booking operations.")]
        public void ShowDlgInwardFacility()
        {
            if (!IsEnabledShowDlgInwardFacility())
                return;
            FacilitySelectLoctation = FacilitySelectLoctation.PrebookingInward;
            ShowDlgFacility(CurrentACMethodBooking.InwardFacility);
        }

        /// <summary>
        /// Determines whether the inward facility selection dialog can be displayed for the current booking.
        /// Returns true if the current ACMethodBooking exists, allowing users to select inward facilities
        /// for facility booking operations within the picking workflow.
        /// </summary>
        /// <returns>True if the inward facility selection dialog can be shown; otherwise, false.</returns>
        public bool IsEnabledShowDlgInwardFacility()
        {
            return CurrentACMethodBooking != null;
        }


        /// <summary>
        /// Opens a dialog to select a target facility (OutwardFacility) for facility pre-booking operations.
        /// This method sets the facility selection context to outward pre-booking and displays
        /// the facility explorer dialog with the current OutwardFacility as the pre-selected value.
        /// If the user confirms the selection (OK), the chosen facility is assigned to the
        /// CurrentACMethodBooking.OutwardFacility property and a property change notification is triggered
        /// to update bound UI controls.
        /// </summary>
        [ACMethodInfo("ShowDlgInwardFacility", "en{'Choose facility'}de{'Lager auswählen'}", 9999,
                      Description = @"Opens a dialog to select a target facility (OutwardFacility) for facility pre-booking operations.
                                      This method sets the facility selection context to outward pre-booking and displays
                                      the facility explorer dialog with the current OutwardFacility as the pre-selected value.
                                      If the user confirms the selection (OK), the chosen facility is assigned to the
                                      CurrentACMethodBooking.OutwardFacility property and a property change notification is triggered
                                      to update bound UI controls.")]
        public void ShowDlgOutwardFacility()
        {
            if (!IsEnabledShowDlgOutwardFacility())
                return;
            FacilitySelectLoctation = FacilitySelectLoctation.PrebookingOutward;
            ShowDlgFacility(CurrentACMethodBooking.OutwardFacility);
        }

        /// <summary>
        /// Determines whether the outward facility selection dialog can be displayed for the current booking.
        /// Returns true if the current ACMethodBooking exists, allowing users to select outward facilities
        /// for facility booking operations within the picking workflow.
        /// </summary>
        /// <returns>True if the outward facility selection dialog can be shown; otherwise, false.</returns>
        public bool IsEnabledShowDlgOutwardFacility()
        {
            return CurrentACMethodBooking != null;
        }

        /// <summary>
        /// Opens a dialog to select a facility for filtering "from" facilities in the picking order interface.
        /// Uses the BSOFacilityExplorer child component to display a facility selection dialog with the current
        /// SelectedFilterFromFacility as the pre-selected value. If the user confirms the selection (OK), the chosen facility
        /// is added to the AccessFilterFromFacility navigation list if not already present, and the SelectedFilterFromFacility
        /// property is updated to reflect the new selection. This enables users to choose and assign source facilities
        /// for filtering picking orders by their "from" location.
        /// </summary>
        [ACMethodInfo("ShowDlgFilterFromFacility", "en{'Choose facility'}de{'Lager auswählen'}", 9999,
                      Description = @"Opens a dialog to select a facility for filtering ""from"" facilities in the picking order interface.
                                      Uses the BSOFacilityExplorer child component to display a facility selection dialog with the current
                                      SelectedFilterFromFacility as the pre-selected value. If the user confirms the selection (OK), the chosen facility
                                      is added to the AccessFilterFromFacility navigation list if not already present, and the SelectedFilterFromFacility
                                      property is updated to reflect the new selection. This enables users to choose and assign source facilities
                                      for filtering picking orders by their ""from"" location.")]
        public void ShowDlgFilterFromFacility()
        {
            if (!IsEnabledShowDlgFilterFromFacility())
                return;
            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(SelectedFilterFromFacility != null ? SelectedFilterFromFacility : null);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                if (facility != null)
                    if (!AccessFilterFromFacility.NavList.Contains(facility))
                        AccessFilterFromFacility.NavList.Add(facility);
                SelectedFilterFromFacility = facility;
            }
        }

        /// <summary>
        /// Determines whether the filter dialog for selecting source facilities can be displayed.
        /// This method always returns true, indicating that the facility filter selection dialog
        /// is always available for use in the picking order interface, allowing users to choose
        /// and assign source facilities for filtering picking orders by their "from" location.
        /// </summary>
        /// <returns>Always returns true, as the filter dialog is unconditionally enabled.</returns>
        public bool IsEnabledShowDlgFilterFromFacility()
        {
            return true;
        }

        /// <summary>
        /// Opens a dialog to select a target facility (ToFacility) for filtering picking orders in the picking order interface.
        /// Uses the BSOFacilityExplorer child component to display a facility selection dialog with the current
        /// SelectedFilterToFacility as the pre-selected value. If the user confirms the selection (OK), the chosen facility
        /// is added to the AccessFilterToFacility navigation list if not already present, and the SelectedFilterToFacility
        /// property is updated to reflect the new selection. This enables users to choose and assign target facilities
        /// for filtering picking orders by their "to" location.
        /// </summary>
        [ACMethodInfo("ShowDlgFilterToFacility", "en{'Choose facility'}de{'Lager auswählen'}", 9999,
                      Description = @"Opens a dialog to select a target facility (ToFacility) for filtering picking orders in the picking order interface.
                                      Uses the BSOFacilityExplorer child component to display a facility selection dialog with the current
                                      SelectedFilterToFacility as the pre-selected value. If the user confirms the selection (OK), the chosen facility
                                      is added to the AccessFilterToFacility navigation list if not already present, and the SelectedFilterToFacility
                                      property is updated to reflect the new selection. This enables users to choose and assign target facilities
                                      for filtering picking orders by their ""to"" location.")]
        public void ShowDlgFilterToFacility()
        {
            if (!IsEnabledShowDlgFilterToFacility())
                return;
            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(SelectedFilterToFacility != null ? SelectedFilterToFacility : null);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                if (facility != null)
                    if (!AccessFilterToFacility.NavList.Contains(facility))
                        AccessFilterToFacility.NavList.Add(facility);
                SelectedFilterToFacility = facility;
            }
        }

        /// <summary>
        /// Determines whether the filter dialog for selecting target facilities can be displayed.
        /// This method always returns true, indicating that the facility filter selection dialog
        /// is always available for use in the picking order interface, allowing users to choose
        /// and assign target facilities for filtering picking orders by their "to" location.
        /// </summary>
        /// <returns>Always returns true, as the filter dialog is unconditionally enabled.</returns>
        public bool IsEnabledShowDlgFilterToFacility()
        {
            return true;
        }

        private void ShowDlgFacility(Facility preselectedFacility)
        {
            VBDialogResult dlgResult = BSOFacilityExplorer_Child.Value.ShowDialog(preselectedFacility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                switch (FacilitySelectLoctation)
                {
                    case FacilitySelectLoctation.PrebookingInward:
                        CurrentACMethodBooking.InwardFacility = facility;
                        break;
                    case FacilitySelectLoctation.PrebookingOutward:
                        CurrentACMethodBooking.OutwardFacility = facility;
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #endregion

        #region Message
        protected virtual void UpdateBSOMsg()
        {
            if (CurrentACMethodBooking == null)
                return;
            if (!CurrentACMethodBooking.ValidMessage.IsSucceded() || CurrentACMethodBooking.ValidMessage.HasWarnings())
            {
                if (!BSOMsg.IsEqual(CurrentACMethodBooking.ValidMessage))
                {
                    BSOMsg.UpdateFrom(CurrentACMethodBooking.ValidMessage);
                }
            }
            else
            {
                if (BSOMsg.MsgDetailsCount > 0)
                    BSOMsg.ClearMsgDetails();
            }
        }
        #endregion

        #region Activation

        /// <summary>
        /// Handles interaction events from WPF controls, such as tab item activation.
        /// When a tab item is activated, delegates to the OnActivate method with the VBContent.
        /// For other interaction events, calls the base class implementation.
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
        protected bool _ActivateInDNote = false;
        protected bool _ActivateOutOpen = false;
        protected bool _ActivateOutDNote = false;
        protected bool _ActivateProdOpen = false;

        /// <summary>
        /// Handles activation of different tabs or sections in the picking order interface.
        /// When a tab is activated for the first time, it refreshes the corresponding order position lists
        /// (InOrderPos, OutOrderPos, or ProdOrderPartslistPos) to ensure data is loaded and displayed.
        /// This prevents unnecessary refreshes on subsequent activations of the same tab.
        /// </summary>
        /// <param name="page">The page or tab identifier being activated (e.g., "ActivateInOpen", "ActivateOutOpen").</param>
        [ACMethodInfo("Picking", "en{'Activate'}de{'Aktivieren'}", 602, true, Global.ACKinds.MSMethodPrePost,
                      Description = @"Handles activation of different tabs or sections in the picking order interface.
                                      When a tab is activated for the first time, it refreshes the corresponding order position lists
                                      (InOrderPos, OutOrderPos, or ProdOrderPartslistPos) to ensure data is loaded and displayed.
                                      This prevents unnecessary refreshes on subsequent activations of the same tab.")]
        public void OnActivate(string page)
        {
            if (!PreExecute(nameof(OnActivate)))
                return;
            switch (page)
            {
                case "*AssignInOrderPos":
                case "ActivateInOpen":
                    if (!_ActivateInOpen)
                    {
                        _ActivateInOpen = true;
                        RefreshInOrderPosList();
                    }
                    break;
                case "ActivateInDNote":
                    if (!_ActivateInDNote)
                    {
                        _ActivateInDNote = true;
                        OnPropertyChanged(nameof(DNoteInOrderPosList));
                    }
                    break;
                case "*AssignOutOrderPos":
                case "ActivateOutOpen":
                    if (!_ActivateOutOpen)
                    {
                        _ActivateOutOpen = true;
                        RefreshOutOrderPosList();
                    }
                    break;
                case "ActivateOutDNote":
                    if (!_ActivateOutDNote)
                    {
                        _ActivateOutDNote = true;
                        OnPropertyChanged(nameof(DNoteOutOrderPosList));
                    }
                    break;
                case "*AssignProdOrderPos":
                    if (!_ActivateProdOpen)
                    {
                        _ActivateProdOpen = true;
                        RefreshProdOrderPartslistPosList();
                    }
                    break;
                //case "*PickingPos":
                //    OnPropertyChanged(nameof(CurrentPickingPos));
                //    break;
                default:
                    break;
            }
            PostExecute(nameof(OnActivate));
        }
        #endregion

        #region Validation

        /// <summary>
        /// Validates the routes for the current picking order by checking for potential conflicts with other picking orders and production batch plans.
        /// This method saves any pending changes, performs route validation if enabled, and displays results or warnings to the user.
        /// Route validation ensures that the picking order's material movements do not conflict with scheduled production operations or other logistics workflows.
        /// </summary>
        [ACMethodCommand("", "en{'Check Routes'}de{'Routenprüfung'}", (short)MISort.Cancel,
                         Description = @"Validates the routes for the current picking order by checking for potential conflicts with other picking orders and production batch plans.
                                         This method saves any pending changes, performs route validation if enabled, and displays results or warnings to the user.
                                         Route validation ensures that the picking order's material movements do not conflict with scheduled production operations or other logistics workflows.")]
        public void ValidateRoutes()
        {
            ACSaveChanges();
            if (!IsEnabledValidateRoutes())
                return;
            ValidateRoutesInternal();
        }

        protected MsgWithDetails ValidateRoutesInternal()
        {
            MsgWithDetails msg = null;
            using (var dbIPlus = new Database())
            {
                msg = this.PickingManager.ValidateRoutes(this.DatabaseApp, dbIPlus, CurrentPicking,
                                                            MandatoryConfigStores,
                                                            PARole.ValidationBehaviour.Laxly);
                if (msg != null)
                {
                    if (!msg.IsSucceded())
                    {
                        if (String.IsNullOrEmpty(msg.Message))
                        {
                            // Die Stückliste wäre nicht produzierbar weil:
                            msg.Message = Root.Environment.TranslateMessage(this, "Info50020");
                        }
                        Messages.Msg(msg, Global.MsgResult.OK, eMsgButton.OK);
                        return msg;
                    }
                    else if (msg.HasWarnings())
                    {
                        if (String.IsNullOrEmpty(msg.Message))
                        {
                            // Es gäbe folgende Probleme wenn Sie einen Auftrag anlegen und starten würden:
                            msg.Message = Root.Environment.TranslateMessage(this, "Info50021");
                        }
                        Messages.Msg(msg, Global.MsgResult.OK, eMsgButton.OK);
                        return msg;
                    }
                }
                // Die Routenprüfung war erflogreich. Die Stückliste ist produzierbar.
                Messages.Info(this, "Info50022");
                return msg;
            }
        }

        /// <summary>
        /// Determines whether route validation can be performed for the current picking order.
        /// Returns true if there is a current picking order and a picking manager is available; otherwise, false.
        /// </summary>
        /// <returns>True if route validation is enabled; otherwise, false.</returns>
        public bool IsEnabledValidateRoutes()
        {
            if (CurrentPicking == null || this.PickingManager == null)
                return false;
            return true;
        }
        #endregion

        #region Start Workflow

        /// <summary>
        /// Starts the transport workflow for the current picking order by validating prerequisites, 
        /// checking for route conflicts or warnings, selecting an appropriate application manager, 
        /// and initiating the picking process through the PickingManager. This method handles 
        /// the complete workflow startup including user confirmations for warnings, app manager 
        /// selection dialogs, and error handling for workflow execution failures.
        /// </summary>
        [ACMethodCommand("Start", "en{'Start Transports'}de{'Transporte starten'}", 604, true,
                         Description = @"Starts the transport workflow for the current picking order by validating prerequisites, 
                                         checking for route conflicts or warnings, selecting an appropriate application manager, 
                                         and initiating the picking process through the PickingManager. This method handles 
                                         the complete workflow startup including user confirmations for warnings, app manager 
                                         selection dialogs, and error handling for workflow execution failures.")]
        public void StartWorkflow()
        {
            //_IsStartingWF = true;
            try
            {
                if (!IsEnabledStartWorkflow())
                    return;

                MsgWithDetails msg = null;
                using (var dbIPlus = new Database())
                {
                    msg = this.PickingManager.ValidateStart(this.DatabaseApp, dbIPlus, CurrentPicking,
                                                                MandatoryConfigStores,
                                                                PARole.ValidationBehaviour.Strict,
                                                                SelectedPWNodeProcessWorkflow, true);
                    if (msg != null)
                    {
                        if (!msg.IsSucceded())
                        {
                            if (String.IsNullOrEmpty(msg.Message))
                            {
                                // Der Auftrag kann nicht gestartet werden weil:
                                msg.Message = Root.Environment.TranslateMessage(this, "Error50642");
                            }
                            Messages.Msg(msg, Global.MsgResult.OK, eMsgButton.OK);
                            return;
                        }
                        else if (msg.HasWarnings())
                        {
                            if (String.IsNullOrEmpty(msg.Message))
                            {
                                //Möchten Sie den Auftrag wirklich starten? Es gibt nämlich folgende Probleme:
                                msg.Message = Root.Environment.TranslateMessage(this, "Question50107");
                            }
                            var userResult = Messages.Msg(msg, Global.MsgResult.No, eMsgButton.YesNo);
                            if (userResult == Global.MsgResult.No || userResult == Global.MsgResult.Cancel)
                                return;
                        }
                    }
                }

                gip.core.datamodel.ACClassMethod acClassMethod = CurrentPicking.ACClassMethod.FromIPlusContext<gip.core.datamodel.ACClassMethod>(this.DatabaseApp.ContextIPlus);
                if (acClassMethod == null)
                    return;
                gip.core.datamodel.ACProject project = acClassMethod.ACClass.ACProject as gip.core.datamodel.ACProject;

                AppManagersList = this.Root.FindChildComponents(project.RootClass, 1);
                if (AppManagersList.Count > 1)
                {
                    DialogResult = null;
                    ShowDialog(this, "SelectAppManager");
                    if (DialogResult == null || DialogResult.SelectedCommand != eMsgButton.OK)
                        return;
                }
                else
                    SelectedAppManager = AppManagersList.FirstOrDefault();

                ACComponent pAppManager = SelectedAppManager as ACComponent;
                if (pAppManager == null)
                    return;
                if (pAppManager.IsProxy && pAppManager.ConnectionState == ACObjectConnectionState.DisConnected)
                {
                    // TODO: Message
                    return;
                }

                msg = this.PickingManager.StartPicking(this.DatabaseApp, pAppManager, CurrentPicking, acClassMethod, SelectedPWNodeProcessWorkflow, true);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOPicking", "StartWorkflow", msg);
            }
            finally
            {
                //_IsStartingWF = false;
            }

        }

        /// <summary>
        /// Determines whether the transport workflow can be started for the current picking order.
        /// This method validates multiple prerequisites before allowing workflow execution:
        /// - Verifies that a current picking order exists and has an associated ACClassMethod (workflow definition)
        /// - Ensures the picking order is not already in progress or completed (state must be <= InProcess)
        /// - Checks that picking positions exist and at least one position is in ReadyToLoad state
        /// - Confirms that the PickingManager service is available for workflow operations
        /// - If a ProcessWorkflowPresenter is available, validates that a process workflow node is selected
        /// Returns true if all conditions are met and the workflow can be initiated; otherwise, returns false.
        /// </summary>
        /// <returns>True if the picking order workflow can be started; otherwise, false.</returns>
        public bool IsEnabledStartWorkflow()
        {
            if (this.CurrentPicking == null
                || CurrentPicking.ACClassMethod == null
                || this.CurrentPicking.PickingState > PickingStateEnum.InProcess
                || PickingPosList == null
                || !PickingPosList.Any()
                || !PickingPosList.Where(c => c.MDDelivPosLoadState != null && c.MDDelivPosLoadState.DelivPosLoadState == MDDelivPosLoadState.DelivPosLoadStates.ReadyToLoad).Any()
                || PickingManager == null)
                return false;

            if (ProcessWorkflowPresenter != null)
                return SelectedPWNodeProcessWorkflow != null;

            return true;
        }


        protected gip.core.datamodel.ACClassWF SelectedPWNodeProcessWorkflow
        {
            get
            {
                if (ProcessWorkflowPresenter == null
                    || ProcessWorkflowPresenter.SelectedWFNode == null
                    || ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF == null
                    || ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF.PWACClass == null)
                    return null;
                if (typeof(PWNodeProcessWorkflow).IsAssignableFrom(ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF.PWACClass.ObjectType))
                    return ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF;
                return null;
            }
        }

        protected IACComponent _SelectedAppManager;
        /// <summary>
        /// Gets or sets the currently selected application manager component for workflow execution.
        /// This property represents the IACComponent that has been chosen from the AppManagersList
        /// to handle the execution of picking workflows. When multiple application managers are available,
        /// this property is set through a user selection dialog in the StartWorkflow method.
        /// The selected manager is used to initiate and control the transport workflow for the current picking order.
        /// </summary>
        [ACPropertySelected(630, "AppManagers",
                            Description = @"Gets or sets the currently selected application manager component for workflow execution.
                                            This property represents the IACComponent that has been chosen from the AppManagersList
                                            to handle the execution of picking workflows. When multiple application managers are available,
                                            this property is set through a user selection dialog in the StartWorkflow method.
                                            The selected manager is used to initiate and control the transport workflow for the current picking order.")]
        public IACComponent SelectedAppManager
        {
            get
            {
                return _SelectedAppManager;
            }
            set
            {
                _SelectedAppManager = value;
                OnPropertyChanged(nameof(SelectedAppManager));
            }
        }

        protected List<IACComponent> _AppManagersList;
        /// <summary>
        /// Gets or sets the list of available application manager components for workflow execution.
        /// This property contains IACComponent instances representing application managers that can handle
        /// the execution of picking workflows. The list is populated dynamically when starting a workflow
        /// by searching for child components under the project's root class. If multiple managers are found,
        /// a selection dialog is presented to the user. The selected manager is used to initiate and control
        /// the transport workflow for the current picking order.
        /// </summary>
        [ACPropertyList(631, "AppManagers",
                        Description = @"Gets or sets the list of available application manager components for workflow execution.
                                        This property contains IACComponent instances representing application managers that can handle
                                        the execution of picking workflows. The list is populated dynamically when starting a workflow
                                        by searching for child components under the project's root class. If multiple managers are found,
                                        a selection dialog is presented to the user. The selected manager is used to initiate and control
                                        the transport workflow for the current picking order.")]
        public List<IACComponent> AppManagersList
        {
            get
            {
                return _AppManagersList;
            }
            set
            {
                _AppManagersList = value;
                OnPropertyChanged(nameof(AppManagersList));
            }
        }

        public VBDialogResult DialogResult { get; set; }

        /// <summary>
        /// Confirms the app manager selection in the dialog by setting the dialog result to OK,
        /// preserving the currently selected app manager, and closing the top dialog.
        /// This method is used in the workflow startup process to finalize the selection
        /// of an application manager for executing picking operations.
        /// </summary>
        [ACMethodCommand("Dialog", Const.Ok, (short)MISort.Okay,
                         Description = @"Confirms the app manager selection in the dialog by setting the dialog result to OK,
                                         preserving the currently selected app manager, and closing the top dialog.
                                         This method is used in the workflow startup process to finalize the selection
                                         of an application manager for executing picking operations.")]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            var selected = SelectedAppManager;
            CloseTopDialog();
            SelectedAppManager = selected;
        }

        /// <summary>
        /// Cancels the app manager selection dialog by setting the dialog result to cancel,
        /// preserving the currently selected app manager, closing the dialog, and restoring the selection.
        /// This method ensures that the SelectedAppManager state is maintained during the cancel operation.
        /// </summary>
        [ACMethodCommand("Dialog", Const.Cancel, (short)MISort.Cancel,
                         Description = @"Cancels the app manager selection dialog by setting the dialog result to cancel,
                                         preserving the currently selected app manager, closing the dialog, and restoring the selection.
                                         This method ensures that the SelectedAppManager state is maintained during the cancel operation.")]
        public void DialogCancel()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            var selected = SelectedAppManager;
            CloseTopDialog();
            SelectedAppManager = selected;
        }

        /// <summary>
        /// Gets the picking position that is currently selected for workflow target selection operations.
        /// This method returns the SelectedPickingPos property, which represents the picking position
        /// that has been chosen by the user or system for workflow-related targeting and processing.
        /// It is used by workflow engines to determine which picking position should be the target
        /// for automated operations, routing decisions, or further processing steps within the
        /// picking order management workflow.
        /// </summary>
        /// <returns>The currently selected PickingPos instance, or null if no position is selected.</returns>

        public virtual PickingPos GetPickingPosForWFTargetSelector()
        {
            return SelectedPickingPos;
        }
        #endregion

        #region Show order dialog

        /// <summary>
        /// Opens a dialog to display the picking order details based on the provided picking number and optional picking position ID.
        /// This method configures the primary navigation query to filter by the specified picking number, performs a search to load the relevant picking orders,
        /// sets the current picking order and optionally selects a specific picking position if provided.
        /// If showPreferredParam is true, it switches to the preferred parameters tab in the dialog.
        /// Finally, it displays the "DisplayOrderDialog" and stops the current component while enabling AC program functionality.
        /// </summary>
        /// <param name="pickingNo">The picking order number to filter and display.</param>
        /// <param name="pickingPosID">The optional picking position ID to select within the picking order.</param>
        /// <param name="showPreferredParam">If true, switches to the preferred parameters tab in the dialog.</param>
        [ACMethodInfo("Dialog", "en{'Dialog Picking Order'}de{'Dialog Kommissionierauftrag'}", (short)MISort.QueryPrintDlg,
                      Description = @"Opens a dialog to display the picking order details based on the provided picking number and optional picking position ID.
                                     This method configures the primary navigation query to filter by the specified picking number, performs a search to load the relevant picking orders,
                                     sets the current picking order and optionally selects a specific picking position if provided.
                                     If showPreferredParam is true, it switches to the preferred parameters tab in the dialog.
                                     Finally, it displays the ""DisplayOrderDialog"" and stops the current component while enabling AC program functionality.")]
        public void ShowDialogOrder(string pickingNo, Guid pickingPosID, bool showPreferredParam = false)
        {
            if (AccessPrimary == null)
                return;
            //AccessPrimary.NavACQueryDefinition.SearchWord = facilityNo;
            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "PickingNo").FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, "PickingNo", Global.LogicalOperators.contains, Global.Operators.and, pickingNo, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            else
                filterItem.SearchWord = pickingNo;

            this.Search();
            if (PickingList != null && PickingList.Count() > 1)
                CurrentPicking = PickingList.FirstOrDefault(c => c.PickingNo == pickingNo);
            if (CurrentPicking != null && pickingPosID != Guid.Empty)
            {
                if (this.PickingPosList != null && this.PickingPosList.Any())
                {
                    var pickingPos = this.PickingPosList.Where(c => c.PickingPosID == pickingPosID).FirstOrDefault();
                    if (pickingPos != null)
                        SelectedPickingPos = pickingPos;
                }
            }

            if (showPreferredParam && ProcessWorkflowPresenter != null)
            {
                SelectedTab = 2;
                IsShowingPreferredParams = true;
            }

            ShowDialog(this, "DisplayOrderDialog");

            this.ParentACComponent.StopComponent(this);
            _IsEnabledACProgram = true;
        }

        /// <summary>
        /// Opens a dialog to display picking order information based on the provided PAOrderInfo.
        /// Processes the entities in PAOrderInfo to retrieve the associated Picking and PickingPos objects from the database.
        /// Handles different entity types including Picking, PickingPos, OrderLog, and BSOPreferredParameters.
        /// Sets flags such as _IsEnabledACProgram and showPreferredParams based on the entity types found.
        /// Calls ShowDialogOrder to display the picking order dialog with the retrieved information.
        /// Sets the DialogResult on the PAOrderInfo object.
        /// </summary>
        /// <param name="paOrderInfo">The PAOrderInfo containing entities to process for displaying the picking order dialog.</param>
        [ACMethodInfo("Dialog", "en{'Dialog Picking Order'}de{'Dialog Kommissionierauftrag'}", (short)MISort.QueryPrintDlg + 1,
                      Description = @"Opens a dialog to display picking order information based on the provided PAOrderInfo.
                                      Processes the entities in PAOrderInfo to retrieve the associated Picking and PickingPos objects from the database.
                                      Handles different entity types including Picking, PickingPos, OrderLog, and BSOPreferredParameters.
                                      Sets flags such as _IsEnabledACProgram and showPreferredParams based on the entity types found.
                                      Calls ShowDialogOrder to display the picking order dialog with the retrieved information.
                                      Sets the DialogResult on the PAOrderInfo object.")]
        public void ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;

            // Falls Produktionsauftrag
            PickingPos pickingPos = null;
            Picking picking = null;
            bool showPreferredParams = false;
            foreach (var entry in paOrderInfo.Entities)
            {
                if (entry.EntityName == Picking.ClassName)
                {
                    picking = this.DatabaseApp.Picking
                        .Where(c => c.PickingID == entry.EntityID)
                        .FirstOrDefault();
                }
                else if (entry.EntityName == PickingPos.ClassName)
                {
                    pickingPos = this.DatabaseApp.PickingPos
                        .Include(c => c.Picking)
                        .Where(c => c.PickingPosID == entry.EntityID)
                        .FirstOrDefault();
                    if (pickingPos != null)
                        picking = pickingPos.Picking;
                }
                else if (entry.EntityName == OrderLog.ClassName)
                {
                    _IsEnabledACProgram = false;
                    OrderLog currentOrderLog = DatabaseApp.OrderLog.FirstOrDefault(c => c.VBiACProgramLogID == entry.EntityID);
                    if (currentOrderLog == null || currentOrderLog.PickingPos == null)
                        return;
                    pickingPos = currentOrderLog.PickingPos;
                    picking = pickingPos.Picking;
                }
                else if (entry.EntityName == nameof(BSOPreferredParameters))
                {
                    showPreferredParams = true;
                }
            }


            if (picking == null)
                return;

            ShowDialogOrder(picking.PickingNo, pickingPos != null ? pickingPos.PickingPosID : Guid.Empty, showPreferredParams);
            paOrderInfo.DialogResult = this.DialogResult;
        }
        #endregion

        #region Context menu => Tracking

        /// <summary>
        /// Builds and returns a context menu list for the specified WPF control, extending the base menu with tracking and tracing options for facility bookings.
        /// When the context is related to a selected facility booking, adds menu items for tracking and tracing functionality using the TrackingCommonStart service.
        /// This allows users to access tracking operations directly from the facility booking context menu in the picking order interface.
        /// </summary>
        /// <param name="vbContent">The VBContent identifier of the WPF control requesting the menu</param>
        /// <param name="vbControl">The full type name of the WPF control requesting the menu</param>
        /// <returns>An ACMenuItemList containing the base menu items plus any additional tracking/tracing items for facility bookings</returns>
        public override ACMenuItemList GetMenu(string vbContent, string vbControl)
        {
            ACMenuItemList aCMenuItems = base.GetMenu(vbContent, vbControl);
            if (vbContent == nameof(SelectedFacilityBooking) && SelectedFacilityBooking != null)
            {
                TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
                ACMenuItemList trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, SelectedFacilityBooking);
                aCMenuItems.AddRange(trackingAndTracingMenuItems);
            }
            return aCMenuItems;
        }

        /// <summary>
        /// Handles tracking and tracing operations for facility bookings by delegating to the TrackingCommonStart service.
        /// This method is invoked from context menus to perform tracking operations on selected facility booking items,
        /// allowing users to trace material movements, batch histories, and related logistics data through the system.
        /// The method creates a TrackingCommonStart instance and executes the appropriate tracking operation based on
        /// the specified direction, item to track, additional filters, and tracking engine configuration.
        /// </summary>
        /// <param name="direction">The direction of the tracking search (forward/backward in the supply chain)</param>
        /// <param name="itemForTrack">The IACObject item (typically a facility booking) to perform tracking on</param>
        /// <param name="additionalFilter">Optional additional filter criteria for refining the tracking results</param>
        /// <param name="engine">The specific tracking engine to use for the operation</param>
        [ACMethodInfo("OnTrackingCall", "en{'OnTrackingCall'}de{'OnTrackingCall'}", 600, false,
                      Description = @"Handles tracking and tracing operations for facility bookings by delegating to the TrackingCommonStart service.
                                      This method is invoked from context menus to perform tracking operations on selected facility booking items,
                                      allowing users to trace material movements, batch histories, and related logistics data through the system.
                                      The method creates a TrackingCommonStart instance and executes the appropriate tracking operation based on
                                      the specified direction, item to track, additional filters, and tracking engine configuration.")]
        public void OnTrackingCall(GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter, TrackingEnginesEnum engine)
        {
            TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
            trackingCommonStart.DoTracking(this, direction, itemForTrack, additionalFilter, engine);
        }

        #endregion

        #region ACMethods -> Filter

        /// <summary>
        /// Clears all filter properties used for searching picking orders, resetting them to their default null or unselected state.
        /// This method resets the following filters: picking number, date range (from/to), material number, lot numbers (facility booking and reservation, including external),
        /// selected picking type, picking state, and delivery address. After clearing, the search results will reflect no active filters.
        /// </summary>
        [ACMethodInfo("FilterClear", "en{'Clear'}de{'Löschen'}", 307,
                      Description = @"Clears all filter properties used for searching picking orders, resetting them to their default null or unselected state.
                                      This method resets the following filters: picking number, date range (from/to), material number, lot numbers (facility booking and reservation, including external),
                                      selected picking type, picking state, and delivery address. After clearing, the search results will reflect no active filters.")]
        public void FilterClear()
        {
            if (!IsEnabledFilterClear())
                return;

            FilterPickingPickingNo = null;
            FilterDateFrom = null;
            FilterDateTo = null;
            FilterMaterialNo = null;
            FilterLotNoFB = null;
            FilterExternLotNoFB = null;
            FilterLotNoFR = null;
            FilterExternLotNoFR = null;
            SelectedFilterMDPickingType = null;
            SelectedFilterPickingState = null;
            SelectedFilterDeliveryAddress = null;
        }

        /// <summary>
        /// Determines whether the filter clear functionality is enabled.
        /// This method always returns true, indicating that the filter clear operation is unconditionally available.
        /// </summary>
        /// <returns>True, as the filter clear functionality is always enabled.</returns>
        public bool IsEnabledFilterClear()
        {
            return true;
        }

        #endregion

        #region BroadCasting

        /// <summary>
        /// Broadcasts the current picking order to remote mirrored stores by identifying facilities involved in picking positions and facility bookings that are mirrored across multiple databases.
        /// For each mirrored facility found in picking positions, sends the picking order details using CallSendPicking.
        /// Additionally, for each mirrored facility found in facility bookings, refreshes the facility data for all related bookings using CallRefreshFacility.
        /// This ensures synchronization of picking order changes across distributed warehouse systems where facilities are mirrored on remote databases.
        /// </summary>
        [ACMethodCommand("BroadCastPicking", "en{'Broadcast picking'}de{'Sende Kommissionierauftrag'}", 800, true,
                         Description = @"Broadcasts the current picking order to remote mirrored stores by identifying facilities involved in picking positions and facility bookings that are mirrored across multiple databases.
                                         For each mirrored facility found in picking positions, sends the picking order details using CallSendPicking.
                                         Additionally, for each mirrored facility found in facility bookings, refreshes the facility data for all related bookings using CallRefreshFacility.
                                         This ensures synchronization of picking order changes across distributed warehouse systems where facilities are mirrored on remote databases.")]
        public void BroadCastPicking()
        {
            if (!IsEnabledBroadCastPicking())
                return;
            PickingPos[] pickingPositions = CurrentPicking.PickingPos_Picking.ToArray();

            List<Facility> facilities = new List<Facility>();
            foreach (PickingPos pickingPos in pickingPositions)
            {
                if (pickingPos.FromFacility != null && pickingPos.FromFacility.IsMirroredOnMoreDatabases && !facilities.Contains(pickingPos.FromFacility))
                {
                    facilities.Add(pickingPos.FromFacility);
                }
                if (pickingPos.ToFacility != null && pickingPos.ToFacility.IsMirroredOnMoreDatabases && !facilities.Contains(pickingPos.ToFacility))
                {
                    facilities.Add(pickingPos.ToFacility);
                }
            }

            foreach (Facility facility in facilities)
            {
                facility.CallSendPicking(false, CurrentPicking.PickingID);
            }

            facilities = new List<Facility>();
            FacilityBooking[] facilityBookings = CurrentPicking.PickingPos_Picking.SelectMany(c => c.FacilityBooking_PickingPos).ToArray();

            foreach (FacilityBooking facilityBooking in facilityBookings)
            {
                if (facilityBooking.InwardFacility != null && facilityBooking.InwardFacility.IsMirroredOnMoreDatabases && !facilities.Contains(facilityBooking.InwardFacility))
                {
                    facilities.Add(facilityBooking.InwardFacility);
                }
                if (facilityBooking.OutwardFacility != null && facilityBooking.OutwardFacility.IsMirroredOnMoreDatabases && !facilities.Contains(facilityBooking.OutwardFacility))
                {
                    facilities.Add(facilityBooking.OutwardFacility);
                }
            }

            foreach (Facility facility in facilities)
            {
                foreach (FacilityBooking facilityBooking in facilityBookings)
                {
                    facility.CallRefreshFacility(true, facilityBooking.FacilityBookingID);
                }
            }

        }

        /// <summary>
        /// Determines whether broadcasting the current picking order to remote mirrored stores is enabled.
        /// Returns true if there is a current picking order selected and it contains at least one picking position.
        /// Broadcasting allows synchronization of picking order changes across distributed warehouse systems
        /// where facilities are mirrored on remote databases.
        /// </summary>
        /// <returns>True if broadcasting is enabled; otherwise, false.</returns>
        public bool IsEnabledBroadCastPicking()
        {
            return CurrentPicking != null && CurrentPicking.PickingPos_Picking.Any();
        }

        #endregion

        #region LabOrder

        /// <summary>
        /// Creates a new lab order for the selected picking position by opening a lab order dialog.
        /// Saves the current picking order state first, then checks if a picking position is selected.
        /// If valid, starts or retrieves the LabOrderDialog component and invokes the new lab order dialog
        /// with the selected picking position as a parameter. The dialog allows users to create and configure
        /// a new lab order associated with the picking position for quality control or testing purposes.
        /// </summary>
        [ACMethodInteraction("Dialog", "en{'New Lab Order'}de{'Neuer Laborauftrag'}", 605, false, "CreateNewLabOrder", Global.ACKinds.MSMethodPrePost,
                             Description = @"Creates a new lab order for the selected picking position by opening a lab order dialog.
                                             Saves the current picking order state first, then checks if a picking position is selected.
                                             If valid, starts or retrieves the LabOrderDialog component and invokes the new lab order dialog
                                             with the selected picking position as a parameter. The dialog allows users to create and configure
                                             a new lab order associated with the picking position for quality control or testing purposes.")]
        public virtual void CreateNewLabOrder()
        {
            Save();
            if (this.DatabaseApp.IsChanged || SelectedPickingPos == null)
                return;
            ACComponent childBSO = ACUrlCommand("?LabOrderDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("LabOrderDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            childBSO.ACUrlCommand("!" + nameof(BSOLabOrder.NewLabOrderDialog), null, null, null, null, SelectedPickingPos);
            childBSO.Stop();
        }

        /// <summary>
        /// Determines whether creating a new lab order is enabled for the current picking position.
        /// Returns true if a picking position is currently selected, allowing users to create
        /// lab orders for quality control or testing purposes associated with the selected position.
        /// </summary>
        /// <returns>True if a picking position is selected and lab order creation is enabled; otherwise, false.</returns>
        public bool IsEnabledCreateNewLabOrder()
        {
            return SelectedPickingPos != null;
        }

        /// <summary>
        /// Opens a dialog to display lab order information for the currently selected picking position.
        /// This method checks if the database has unsaved changes or if no picking position is selected,
        /// then starts or retrieves the LabOrderDialog component to show the lab order view dialog.
        /// The dialog allows users to view and manage lab reports associated with the selected picking position.
        /// After displaying the dialog, the LabOrderDialog component is stopped to clean up resources.
        /// </summary>
        [ACMethodInfo("Dialog", "en{'Lab Report'}de{'Laborbericht'}", 606,
                      Description = @"Opens a dialog to display lab order information for the currently selected picking position.
                                      This method checks if the database has unsaved changes or if no picking position is selected,
                                      then starts or retrieves the LabOrderDialog component to show the lab order view dialog.
                                      The dialog allows users to view and manage lab reports associated with the selected picking position.
                                      After displaying the dialog, the LabOrderDialog component is stopped to clean up resources.")]
        public void ShowLabOrder()
        {
            if (this.DatabaseApp.IsChanged || SelectedPickingPos == null)
                return;
            ACComponent childBSO = ACUrlCommand("?LabOrderDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("LabOrderDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            childBSO.ACUrlCommand("!" + nameof(BSOLabOrder.ShowLabOrderViewDialog), null, null, null, null, SelectedPickingPos, null, true, null);
            childBSO.Stop();
        }

        /// <summary>
        /// Determines whether the lab order dialog can be displayed for the currently selected picking position.
        /// Returns true if no picking position is selected, or if a picking position is selected and it has associated lab orders.
        /// Returns false only if a picking position is selected but has no associated lab orders.
        /// </summary>
        /// <returns>True if the lab order dialog can be shown; otherwise, false.</returns>
        public bool IsEnabledShowLabOrder()
        {
            if (SelectedPickingPos != null)
            {
                if (!SelectedPickingPos.LabOrder_PickingPos.Any())
                    return false;
            }

            return true;
        }

        #endregion

        #region Navigation

        /// <summary>
        /// Navigates to and displays the visitor voucher associated with the currently selected picking order.
        /// This method retrieves the PAShowDlgManagerBase service instance and creates a PAOrderInfo object
        /// containing the visitor voucher's entity information. It then invokes the service to show the
        /// visitor voucher dialog, allowing users to view or interact with the visitor voucher details
        /// related to the selected picking order. If no visitor voucher is associated or the service
        /// is unavailable, the method returns without action.
        /// </summary>
        [ACMethodInteraction("Picking", "en{'Show Visitor voucher'}de{'Besucherbeleg anzeigen'}", 633, false, nameof(SelectedPicking),
                             Description = @"Navigates to and displays the visitor voucher associated with the currently selected picking order.
                                             This method retrieves the PAShowDlgManagerBase service instance and creates a PAOrderInfo object
                                             containing the visitor voucher's entity information. It then invokes the service to show the
                                             visitor voucher dialog, allowing users to view or interact with the visitor voucher details
                                             related to the selected picking order. If no visitor voucher is associated or the service
                                             is unavailable, the method returns without action.")]
        public void NavigateToVisitorVoucher()
        {
            if (!IsEnabledNavigateToVisitorVoucher())
                return;
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = SelectedPicking.VisitorVoucher.VisitorVoucherID,
                    EntityName = VisitorVoucher.ClassName
                });
                service.ShowDialogOrder(this, info);
            }
        }

        /// <summary>
        /// Determines whether navigation to the visitor voucher is enabled for the currently selected picking order.
        /// Returns true if a picking order is selected and it has an associated visitor voucher.
        /// </summary>
        /// <returns>True if navigation to visitor voucher is enabled; otherwise, false.</returns>
        public bool IsEnabledNavigateToVisitorVoucher()
        {
            return SelectedPicking != null && SelectedPicking.VisitorVoucher != null;
        }

        #endregion

        #region ShowParamDialog

        /// <summary>
        /// Opens the preferred parameters dialog for the current picking order's workflow node.
        /// This method displays a dialog allowing users to view and modify preferred parameter values
        /// associated with the selected workflow node in the process workflow presenter.
        /// The dialog is configured with the current picking order's ID and workflow node information
        /// to enable context-specific parameter management for picking operations
        /// </summary>
        [ACMethodCommand(nameof(ShowParamDialog), ConstApp.PrefParam, 656, true,
                         Description = @"Opens the preferred parameters dialog for the current picking order's workflow node.
                                         This method displays a dialog allowing users to view and modify preferred parameter values
                                         associated with the selected workflow node in the process workflow presenter.
                                         The dialog is configured with the current picking order's ID and workflow node information
                                         to enable context-specific parameter management for picking operations")]
        public void ShowParamDialog()
        {
            if (!IsEnabledShowParamDialog())
                return;

            BSOPreferredParameters_Child.Value.ShowParamDialog(
                ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF.ACClassWFID,
                null,
                null,
                CurrentPicking.PickingID);
        }

        /// <summary>
        /// Determines whether the preferred parameters dialog can be displayed for the current picking order's workflow node.
        /// Returns true if the ProcessWorkflowPresenter exists, has a selected workflow node, and the node contains valid workflow content.
        /// </summary>
        /// <returns>True if the preferred parameters dialog can be shown; otherwise, false.</returns>
        public bool IsEnabledShowParamDialog()
        {
            return
                ProcessWorkflowPresenter != null
                && ProcessWorkflowPresenter.SelectedWFNode != null
                && ProcessWorkflowPresenter.SelectedWFNode.ContentACClassWF != null;
        }

        #endregion

        #region Facility Reservation

        /// <summary>
        /// Gets the default reservation state for facility reservations in picking operations.
        /// This method returns the configured default reservation state that is used when creating new facility reservations
        /// during the picking workflow. The reservation state controls how materials are allocated and managed in the warehouse,
        /// affecting availability calculations and batch planning operations. This virtual method can be overridden in derived classes
        /// to provide custom logic for determining the default reservation state based on specific business requirements.
        /// </summary>
        /// <returns>The default ReservationState value configured for the picking operations.</returns>
        public virtual ReservationState GetDefaultReservationState()
        {
            return DefaultReservationState;
        }

        #endregion

        #region Methods => Routing

        /// <summary>
        /// Initiates an asynchronous route calculation check across picking orders and production batch plans to identify potential conflicts.
        /// This method clears any existing messages, resets the calculation result, sets the progress indicator to indeterminate state,
        /// and attempts to invoke the route calculation asynchronously. If the calculation is already in progress, it informs the user
        /// to wait and try again. Otherwise, it displays a dialog showing the calculated route results, which include conflicts
        /// between scheduled pickings, production orders, and facility reservations that may affect material flow and logistics planning.
        /// </summary>
        [ACMethodInfo("", "en{'Route check over orders'}de{'Routenprüfung über Aufträge'}", 9999, true,
                      Description = @"Initiates an asynchronous route calculation check across picking orders and production batch plans to identify potential conflicts.
                                      This method clears any existing messages, resets the calculation result, sets the progress indicator to indeterminate state,
                                      and attempts to invoke the route calculation asynchronously. If the calculation is already in progress, it informs the user
                                      to wait and try again. Otherwise, it displays a dialog showing the calculated route results, which include conflicts
                                      between scheduled pickings, production orders, and facility reservations that may affect material flow and logistics planning.")]
        public void RunPossibleRoutesCheck()
        {
            MsgList.Clear();
            CalculateRouteResult = null;
            CurrentProgressInfo.ProgressInfoIsIndeterminate = true;
            bool invoked = InvokeCalculateRoutesAsync();
            if (!invoked)
            {
                Messages.Info(this, "The calculation is in progress, please wait and try again!");
                return;
            }

            ShowDialog(this, "CalculatedRouteDialog");
        }

        /// <summary>
        /// Determines whether route validation can be performed for the current picking order.
        /// Returns true if there is a PickingList that is not null and contains at least one picking order.
        /// </summary>
        /// <returns>True if route validation is enabled; otherwise, false.</returns>
        public bool IsEnabledPossibleRoutesCheck()
        {
            return PickingList != null && PickingList.Any();
        }

        /// <summary>
        /// Handles asynchronous RMI (Remote Method Invocation) callback responses for route calculation operations.
        /// This method processes incoming callback events from asynchronous method invocations, specifically checking
        /// for completed route calculation tasks. When a task with the matching request ID is detected as deleted
        /// (indicating completion), it triggers the OnCalculateRoutesCallback method to process the results.
        /// Additionally, it logs whether the response corresponds to the expected asynchronous request for debugging purposes.
        /// </summary>
        /// <param name="sender">The IACPointNetBase instance that sent the callback event</param>
        /// <param name="e">The ACEventArgs containing event data, potentially including ACMethodEventArgs for method results</param>
        /// <param name="wrapObject">The wrapped IACObject containing task information, typically an IACTask or ACPointAsyncRMIWrap</param>
        [ACMethodInfo("Function", "en{'RMICallback'}de{'RMICallback'}", 9999,
                      Description = @"Handles asynchronous RMI (Remote Method Invocation) callback responses for route calculation operations.
                                      This method processes incoming callback events from asynchronous method invocations, specifically checking
                                      for completed route calculation tasks. When a task with the matching request ID is detected as deleted
                                      (indicating completion), it triggers the OnCalculateRoutesCallback method to process the results.
                                      Additionally, it logs whether the response corresponds to the expected asynchronous request for debugging purposes.")]
        public void RMICallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            // The callback-method can be called
            if (e != null)
            {
                IACTask taskEntry = wrapObject as IACTask;
                ACPointAsyncRMIWrap<ACComponent> taskEntryMoreConcrete = wrapObject as ACPointAsyncRMIWrap<ACComponent>;
                ACMethodEventArgs eM = e as ACMethodEventArgs;
                if (taskEntry.State == PointProcessingState.Deleted)
                {
                    // Compare RequestID to identify your asynchronus invocation
                    if (taskEntry.RequestID == myTestRequestID)
                    {
                        OnCalculateRoutesCallback();
                    }
                }
                if (taskEntryMoreConcrete.Result.ResultState == Global.ACMethodResultState.Succeeded)
                {
                    bool wasMyAsynchronousRequest = false;
                    if (myRequestEntryA != null && myRequestEntryA.CompareTo(taskEntryMoreConcrete) == 0)
                        wasMyAsynchronousRequest = true;
                    System.Diagnostics.Trace.WriteLine(wasMyAsynchronousRequest.ToString());
                }
            }
        }

        Guid myTestRequestID;
        ACPointAsyncRMISubscrWrap<ACComponent> myRequestEntryA;
        /// <summary>
        /// Initiates an asynchronous route calculation operation by locating the workflow scheduler component,
        /// preparing the route calculation method with required parameters, and invoking it asynchronously.
        /// This method searches for the PABatchPlanScheduler component in the application hierarchy,
        /// constructs an ACMethod for route calculation, sets the RouteCalculation parameter to true,
        /// and executes the method asynchronously using RMI subscription. The operation returns true if
        /// the asynchronous invocation is successfully initiated, allowing for non-blocking route validation
        /// that checks for potential conflicts between picking orders and production batch plans.
        /// </summary>
        /// <returns>True if the asynchronous route calculation was successfully invoked; otherwise, false.</returns>
        public bool InvokeCalculateRoutesAsync()
        {
            ACComponent paWorkflowScheduler = null;
            if (paWorkflowScheduler == null)
            {
                string acUrl = @"\Planning\PickingScheduler";
                using (ACMonitor.Lock(DatabaseApp.ContextIPlus.QueryLock_1X000))
                {
                    core.datamodel.ACClass paClass = DatabaseApp.ContextIPlus.ACClass.FirstOrDefault(c => c.ACIdentifier == nameof(PABatchPlanScheduler) && !c.ACProject.IsProduction);
                    while (paClass != null)
                    {
                        acUrl = paClass.ACURLComponentCached;
                        paClass = paClass.ACClass_BasedOnACClass.Where(c => c.ACProject.IsProduction).FirstOrDefault();
                    }
                }

                paWorkflowScheduler = Root.ACUrlCommand(acUrl) as ACComponent;
            }

            if (paWorkflowScheduler == null)
            {
                Messages.Msg(new Msg(eMsgLevel.Error, "Workflow scheduler is not installed or you have not rights"));
                return false;
            }

            // 1. Invoke ACUrlACTypeSignature for getting a default-ACMethod-Instance
            ACMethod acMethod = paWorkflowScheduler.ACUrlACTypeSignature("!RunRouteCalculation", gip.core.datamodel.Database.GlobalDatabase);

            // 2. Fill out all important parameters
            acMethod.ParameterValueList.GetACValue("RouteCalculation").Value = true;



            myRequestEntryA = RMISubscr.InvokeAsyncMethod(paWorkflowScheduler, "RMIPoint", acMethod, RMICallback);
            if (myRequestEntryA != null)
                myTestRequestID = myRequestEntryA.RequestID;
            return myRequestEntryA != null;
        }

        /// <summary>
        /// Handles the asynchronous callback for route calculation operations, checking for potential conflicts between the current picking order and other scheduled pickings or production batch plans.
        /// Retrieves scheduled pickings and production batch plans, collects their facility reservations, and analyzes calculated routes to identify overlapping facility usage.
        /// Groups conflicts by picking orders and batch plans, generates detailed messages for each conflict including material information and facility components, and updates the UI with results.
        /// Sets the calculation result message indicating whether conflicts were found and completes the progress indication.
        /// </summary>
        public void OnCalculateRoutesCallback()
        {
            try
            {
                var pickings = PickingManager.GetScheduledPickings(DatabaseApp, PickingStateEnum.WaitOnManualClosing, PickingStateEnum.InProcess, null, null, null, null).ToArray();

                List<FacilityReservation> reservations = new List<FacilityReservation>();

                foreach (Picking picking in pickings)
                {
                    //if (PickingList.Where(c => c.PickingID == picking.PickingID).Any())
                    //    continue;

                    foreach (PickingPos pPos in picking.PickingPos_Picking)
                    {
                        reservations.AddRange(pPos.FacilityReservation_PickingPos);
                    }
                }

                var myReservations = SelectedPicking.PickingPos_Picking.SelectMany(c => c.FacilityReservation_PickingPos).ToArray();

                ACProdOrderManager prodOrderManager = ACProdOrderManager.GetServiceInstance(this);
                var prodOrderBatchPlans = prodOrderManager.GetProductionLinieBatchPlansWithPWNode(DatabaseApp, GlobalApp.BatchPlanState.Created, GlobalApp.BatchPlanState.Paused,
                                                                                                         null, null, null, null, null, null, null);

                reservations.AddRange(prodOrderBatchPlans.SelectMany(c => c.FacilityReservation_ProdOrderBatchPlan.Where(x => x.FacilityID.HasValue)));

                List<Tuple<FacilityReservation, string>> result = new List<Tuple<FacilityReservation, string>>();

                foreach (FacilityReservation reservation in myReservations)
                {
                    if (reservation.CalculatedRoute != null)
                    {
                        string[] splitedRoute = reservation.CalculatedRoute.Split(new char[] { ',' });

                        foreach (string guid in splitedRoute)
                        {
                            if (string.IsNullOrEmpty(guid))
                                continue;

                            IEnumerable<Tuple<FacilityReservation, string>> items = reservations.Where(c => c.CalculatedRoute != null && c.CalculatedRoute.Contains(guid)).Select(c => new Tuple<FacilityReservation, string>(c, guid));
                            if (items.Any())
                                result.AddRange(items);
                        }
                    }
                }

                if (result.Any())
                {
                    var groupedByPickings = result.Where(c => c.Item1.PickingPos != null).GroupBy(x => x.Item1.PickingPos.Picking);
                    var groupedByBatchPlan = result.Where(c => c.Item1.ProdOrderBatchPlan != null).GroupBy(x => x.Item1.ProdOrderBatchPlan);

                    List<core.datamodel.ACClass> tempList = new List<core.datamodel.ACClass>();
                    //List<Msg> msgs = new List<Msg>();

                    foreach (var pickingItem in groupedByPickings)
                    {
                        string message = string.Format("{0} ({1}) - {2}", pickingItem.Key.PickingNo, pickingItem.Key.MDPickingType.ACCaption, pickingItem.Key.InsertDate);

                        var groupByReservation = pickingItem.GroupBy(c => c.Item1);

                        foreach (var reservationItem in groupByReservation)
                        {
                            message += System.Environment.NewLine;
                            message += "    ";
                            message += reservationItem.Key.PickingPos.Material.MaterialName1;
                            message += " (";

                            foreach (var routeItem in reservationItem)
                            {
                                Guid acClassID = Guid.Empty;
                                if (Guid.TryParse(routeItem.Item2, out acClassID))
                                {
                                    core.datamodel.ACClass acComp = tempList.FirstOrDefault(c => c.ACClassID == acClassID);
                                    if (acComp == null)
                                    {
                                        acComp = DatabaseApp.ContextIPlus.ACClass.Where(c => c.ACClassID == acClassID).FirstOrDefault();
                                        if (acComp == null)
                                            continue;

                                        tempList.Add(acComp);
                                    }

                                    message += acComp.ACIdentifier + ", ";
                                }
                            }

                            message = message.TrimEnd(new char[] { ',', ' ' });
                            message += ")";
                        }

                        _MainSyncContext?.Send((object state) =>
                        {
                            MsgList.Add(new Msg(eMsgLevel.Info, message));
                        }, new object());
                    }

                    foreach (var batchPlan in groupedByBatchPlan)
                    {
                        string message = string.Format("{0} ({1}) - {2}", batchPlan.Key.ProdOrderPartslist.ProdOrder.ProgramNo, batchPlan.Key.ProdOrderPartslist.InsertDate, batchPlan.Key.ProdOrderPartslist.Partslist.Material.MaterialName1);

                        var groupByReservation = batchPlan.GroupBy(c => c.Item1);

                        foreach (var reservationItem in groupByReservation)
                        {
                            message += System.Environment.NewLine;
                            message += "    (";

                            foreach (var routeItem in reservationItem)
                            {
                                Guid acClassID = Guid.Empty;
                                if (Guid.TryParse(routeItem.Item2, out acClassID))
                                {
                                    core.datamodel.ACClass acComp = tempList.FirstOrDefault(c => c.ACClassID == acClassID);
                                    if (acComp == null)
                                    {
                                        acComp = DatabaseApp.ContextIPlus.ACClass.Where(c => c.ACClassID == acClassID).FirstOrDefault();
                                        if (acComp == null)
                                            continue;

                                        tempList.Add(acComp);
                                    }

                                    message += acComp.ACIdentifier + ", ";
                                }
                            }

                            message = message.TrimEnd(new char[] { ',', ' ' });
                            message += ")";
                        }

                        _MainSyncContext?.Send((object state) =>
                        {
                            MsgList.Add(new Msg(eMsgLevel.Info, message));
                        }, new object());
                    }

                    CalculateRouteResult = Root.Environment.TranslateMessage(this, "Info50099");
                }
                else
                    CalculateRouteResult = Root.Environment.TranslateMessage(this, "Info50098");

                CurrentProgressInfo.ProgressInfoIsIndeterminate = false;
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), nameof(OnCalculateRoutesCallback), e);
            }
        }

        #endregion

        #region Methods -> Other

        /// <summary>
        /// Updates the preparation status and status name for all picking orders in the current picking list.
        /// Iterates through each picking in the _PickingList and sets the PreparationStatus property by calling
        /// PickingManager.GetPickingPreparationStatus(), and sets the PreparationStatusName property by calling
        /// PickingManager.GetPickingPreparationStatusName() with the retrieved status. This ensures that the
        /// preparation status information is synchronized and up-to-date for display in the user interface.
        /// </summary>
        public void SetPreparationStatusInList()
        {
            if (_PickingList != null)
            {
                foreach (Picking picking in _PickingList)
                {
                    picking.PreparationStatus = PickingManager.GetPickingPreparationStatus(DatabaseApp, picking);
                    picking.PreparationStatusName = PickingManager.GetPickingPreparationStatusName(DatabaseApp, picking.PreparationStatus);
                }
            }
        }

        #endregion

        #region Methods => DeliveryNote

        /// <summary>
        /// Opens a dialog to display the delivery note associated with the current picking position.
        /// This method retrieves the delivery note by navigating through the order position hierarchy
        /// (InOrderPos or OutOrderPos) to find the parent order position and its related delivery note.
        /// If a delivery note is found, it uses the PAShowDlgManagerBase service to display the delivery note dialog.
        /// </summary>
        [ACMethodCommand("", "en{'Show delivery note'}de{'Lieferschein anzeigen'}", 9999, true,
                         Description = @"Opens a dialog to display the delivery note associated with the current picking position.
                                         This method retrieves the delivery note by navigating through the order position hierarchy
                                         (InOrderPos or OutOrderPos) to find the parent order position and its related delivery note.
                                         If a delivery note is found, it uses the PAShowDlgManagerBase service to display the delivery note dialog.")]
        public void ShowDeliveryNote()
        {
            if (CurrentPicking == null && CurrentPickingPos == null)
                return;

            DeliveryNote dn = null;

            if (CurrentPickingPos.InOrderPos != null)
            {
                InOrderPos parentPos = CurrentPickingPos.InOrderPos.InOrderPos1_ParentInOrderPos;
                if (parentPos != null)
                {
                    dn = parentPos.DeliveryNotePos_InOrderPos.FirstOrDefault()?.DeliveryNote;
                }

            }
            else if (CurrentPickingPos.OutOrderPos != null)
            {
                OutOrderPos parentPos = CurrentPickingPos.OutOrderPos.OutOrderPos1_ParentOutOrderPos;
                if (parentPos != null)
                {
                    dn = parentPos.DeliveryNotePos_OutOrderPos.FirstOrDefault()?.DeliveryNote;
                }
            }

            if (dn != null)
            {
                PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
                if (service != null)
                {
                    PAOrderInfo info = new PAOrderInfo();
                    info.Entities.Add(new PAOrderInfoEntry(nameof(DeliveryNote), dn.DeliveryNoteID));
                    service.ShowDialogOrder(this, info);
                }
            }
        }

        /// <summary>
        /// Determines whether the delivery note dialog can be displayed for the current picking position.
        /// Returns true if there is a current picking order, a current picking position, and the picking position
        /// is associated with either an InOrderPos (purchase order position) or OutOrderPos (sales order position).
        /// This enables the "Show Delivery Note" functionality when the picking position has a valid order reference
        /// that includes delivery note information.
        /// </summary>
        /// <returns>True if the delivery note can be shown; otherwise, false.</returns>
        public bool IsEnabledShowDeliveryNote()
        {
            return CurrentPicking != null && CurrentPickingPos != null && (CurrentPickingPos.InOrderPos != null || CurrentPickingPos.OutOrderPos != null);
        }

        #endregion

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(AssignProdOrderPartslistPos):
                    AssignProdOrderPartslistPos();
                    return true;
                case nameof(IsEnabledAssignProdOrderPartslistPos):
                    result = IsEnabledAssignProdOrderPartslistPos();
                    return true;
                case nameof(FilterDialogProdOrderPartslistPos):
                    result = FilterDialogProdOrderPartslistPos();
                    return true;
                case nameof(ShowDlgInwardFacility):
                    ShowDlgInwardFacility();
                    return true;
                case nameof(IsEnabledShowDlgInwardFacility):
                    result = IsEnabledShowDlgInwardFacility();
                    return true;
                case nameof(ShowDlgOutwardFacility):
                    ShowDlgOutwardFacility();
                    return true;
                case nameof(IsEnabledShowDlgOutwardFacility):
                    result = IsEnabledShowDlgOutwardFacility();
                    return true;
                case nameof(ShowDlgFilterFromFacility):
                    ShowDlgFilterFromFacility();
                    return true;
                case nameof(IsEnabledShowDlgFilterFromFacility):
                    result = IsEnabledShowDlgFilterFromFacility();
                    return true;
                case nameof(ShowDlgFilterToFacility):
                    ShowDlgFilterToFacility();
                    return true;
                case nameof(IsEnabledShowDlgFilterToFacility):
                    result = IsEnabledShowDlgFilterToFacility();
                    return true;
                case nameof(OnActivate):
                    OnActivate((System.String)acParameter[0]);
                    return true;
                case nameof(ValidateRoutes):
                    ValidateRoutes();
                    return true;
                case nameof(IsEnabledValidateRoutes):
                    result = IsEnabledValidateRoutes();
                    return true;
                case nameof(StartWorkflow):
                    StartWorkflow();
                    return true;
                case nameof(IsEnabledStartWorkflow):
                    result = IsEnabledStartWorkflow();
                    return true;
                case nameof(DialogOK):
                    DialogOK();
                    return true;
                case nameof(DialogCancel):
                    DialogCancel();
                    return true;
                case nameof(ShowDialogOrder):
                    bool showPreferredParams = false;
                    if (acParameter.Length == 3)
                    {
                        showPreferredParams = (bool)acParameter[2];
                    }
                    ShowDialogOrder((System.String)acParameter[0], (System.Guid)acParameter[1], showPreferredParams);
                    return true;
                case nameof(ShowDialogOrderInfo):
                    ShowDialogOrderInfo((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(OnTrackingCall):
                    OnTrackingCall((TrackingAndTracingSearchModel)acParameter[0], (gip.core.datamodel.IACObject)acParameter[1], (System.Object)acParameter[2], (TrackingEnginesEnum)acParameter[3]);
                    return true;
                case nameof(FilterClear):
                    FilterClear();
                    return true;
                case nameof(IsEnabledFilterClear):
                    result = IsEnabledFilterClear();
                    return true;
                case nameof(ProcessWorkflowAssign):
                    ProcessWorkflowAssign();
                    return true;
                case nameof(IsEnabledProcessWorkflowAssign):
                    result = IsEnabledProcessWorkflowAssign();
                    return true;
                case nameof(ProcessWorkflowCancel):
                    ProcessWorkflowCancel();
                    return true;
                case nameof(IsEnabledProcessWorkflowCancel):
                    result = IsEnabledProcessWorkflowCancel();
                    return true;
                case nameof(AssignInOrderPos):
                    AssignInOrderPos();
                    return true;
                case nameof(IsEnabledAssignInOrderPos):
                    result = IsEnabledAssignInOrderPos();
                    return true;
                case nameof(FilterDialogInOrderPos):
                    result = FilterDialogInOrderPos();
                    return true;
                case nameof(AssignDNoteInOrderPos):
                    AssignDNoteInOrderPos();
                    return true;
                case nameof(IsEnabledAssignDNoteInOrderPos):
                    result = IsEnabledAssignDNoteInOrderPos();
                    return true;
                case nameof(AssignOutOrderPos):
                    AssignOutOrderPos();
                    return true;
                case nameof(IsEnabledAssignOutOrderPos):
                    result = IsEnabledAssignOutOrderPos();
                    return true;
                case nameof(FilterDialogOutOrderPos):
                    result = FilterDialogOutOrderPos();
                    return true;
                case nameof(AssignDNoteOutOrderPos):
                    AssignDNoteOutOrderPos();
                    return true;
                case nameof(IsEnabledAssignDNoteOutOrderPos):
                    result = IsEnabledAssignDNoteOutOrderPos();
                    return true;
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
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
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
                case nameof(CancelPicking):
                    CancelPicking();
                    return true;
                case nameof(IsEnabledCancelPicking):
                    result = IsEnabledCancelPicking();
                    return true;
                case nameof(MirrorPicking):
                    MirrorPicking();
                    return true;
                case nameof(IsEnabledMirrorPicking):
                    result = IsEnabledMirrorPicking();
                    return true;
                case nameof(GenerateSubPickingsForSupply):
                    GenerateSubPickingsForSupply();
                    return true;
                case nameof(IsEnabledGenerateSubPickingsForSupply):
                    result = IsEnabledGenerateSubPickingsForSupply();
                    return true;
                case nameof(UnassignPickingPos):
                    UnassignPickingPos();
                    return true;
                case nameof(IsEnabledUnassignPickingPos):
                    result = IsEnabledUnassignPickingPos();
                    return true;
                case nameof(AddPickingPos):
                    AddPickingPos();
                    return true;
                case nameof(IsEnabledAddPickingPos):
                    result = IsEnabledAddPickingPos();
                    return true;
                case nameof(ShowDlgFromFacility):
                    ShowDlgFromFacility();
                    return true;
                case nameof(IsEnabledShowDlgFromFacility):
                    result = IsEnabledShowDlgFromFacility();
                    return true;
                case nameof(ShowDlgToFacility):
                    ShowDlgToFacility();
                    return true;
                case nameof(IsEnabledShowDlgToFacility):
                    result = IsEnabledShowDlgToFacility();
                    return true;
                case nameof(NewFacilityPreBooking):
                    NewFacilityPreBooking();
                    return true;
                case nameof(IsEnabledNewFacilityPreBooking):
                    result = IsEnabledNewFacilityPreBooking();
                    return true;
                case nameof(CancelFacilityPreBooking):
                    CancelFacilityPreBooking();
                    return true;
                case nameof(IsEnabledCancelFacilityPreBooking):
                    result = IsEnabledCancelFacilityPreBooking();
                    return true;
                case nameof(DeleteFacilityPreBooking):
                    DeleteFacilityPreBooking();
                    return true;
                case nameof(IsEnabledDeleteFacilityPreBooking):
                    result = IsEnabledDeleteFacilityPreBooking();
                    return true;
                case nameof(BookDeliveryPos):
                    BookDeliveryPos();
                    return true;
                case nameof(IsEnabledBookDeliveryPos):
                    result = IsEnabledBookDeliveryPos();
                    return true;
                case nameof(BookCurrentACMethodBooking):
                    BookCurrentACMethodBooking();
                    return true;
                case nameof(IsEnabledBookCurrentACMethodBooking):
                    result = IsEnabledBookCurrentACMethodBooking();
                    return true;
                case nameof(BookAllACMethodBookings):
                    BookAllACMethodBookings();
                    return true;
                case nameof(IsEnabledBookAllACMethodBookings):
                    result = IsEnabledBookAllACMethodBookings();
                    return true;
                case nameof(NewFacilityLot):
                    NewFacilityLot();
                    return true;
                case nameof(IsEnabledNewFacilityLot):
                    result = IsEnabledNewFacilityLot();
                    return true;
                case nameof(ShowFacilityLot):
                    ShowFacilityLot();
                    return true;
                case nameof(IsEnabledShowFacilityLot):
                    result = IsEnabledShowFacilityLot();
                    return true;
                case nameof(ShowDlgInwardAvailableQuants):
                    ShowDlgInwardAvailableQuants();
                    return true;
                case nameof(IsEnabledShowDlgInwardAvailableQuants):
                    result = IsEnabledShowDlgInwardAvailableQuants();
                    return true;
                case nameof(ShowDlgOutwardAvailableQuants):
                    ShowDlgOutwardAvailableQuants();
                    return true;
                case nameof(IsEnabledShowDlgOutwardAvailableQuants):
                    result = IsEnabledShowDlgOutwardAvailableQuants();
                    return true;
                case nameof(DlgAvailableQuantsOk):
                    DlgAvailableQuantsOk();
                    return true;
                case nameof(IsEnabledDlgAvailableQuantsOk):
                    result = IsEnabledDlgAvailableQuantsOk();
                    return true;
                case nameof(DlgAvailableQuantsCancel):
                    DlgAvailableQuantsCancel();
                    return true;
                case nameof(FinishOrder):
                    FinishOrder();
                    return true;
                case nameof(IsEnabledFinishOrder):
                    result = IsEnabledFinishOrder();
                    return true;
                case nameof(RegisterWeight):
                    RegisterWeight();
                    return true;
                case nameof(IsEnabledRegisterWeight):
                    result = IsEnabledRegisterWeight();
                    return true;
                case nameof(BroadCastPicking):
                    BroadCastPicking();
                    return true;
                case nameof(IsEnabledBroadCastPicking):
                    result = IsEnabledBroadCastPicking();
                    return true;
                case nameof(CreateNewLabOrder):
                    CreateNewLabOrder();
                    return true;
                case nameof(IsEnabledCreateNewLabOrder):
                    result = IsEnabledCreateNewLabOrder();
                    return true;
                case nameof(NavigateToVisitorVoucher):
                    NavigateToVisitorVoucher();
                    return true;
                case nameof(IsEnabledNavigateToVisitorVoucher):
                    result = IsEnabledNavigateToVisitorVoucher();
                    return true;
                case nameof(RecalcActualQuantity):
                    RecalcActualQuantity();
                    return true;
                case nameof(IsEnabledRecalcActualQuantity):
                    result = IsEnabledRecalcActualQuantity();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region Workflow

        #region Workflow -> private 
        private VBPresenterMethod _presenter = null;
        #endregion

        #region Workflow -> Properties
        private bool _ProcessWorkflowShowAllWorkflows = true;
        /// <summary>
        /// Gets or sets a value indicating whether to display all available workflows in the process workflow list.
        /// When set to true, the ProcessWorkflowList is reset to null, triggering a refresh of the workflow collection
        /// to include all workflows regardless of filtering criteria. This property is used to control the visibility
        /// of workflows in the user interface, allowing users to toggle between filtered and complete workflow lists.
        /// </summary>
        [ACPropertyInfo(627, "InputMaterials", "en{'All Workflows'}de{'Alle Steuerrezepte'}",
                        Description = @"Gets or sets a value indicating whether to display all available workflows in the process workflow list.
                                        When set to true, the ProcessWorkflowList is reset to null, triggering a refresh of the workflow collection
                                        to include all workflows regardless of filtering criteria. This property is used to control the visibility
                                        of workflows in the user interface, allowing users to toggle between filtered and complete workflow lists.")]
        public bool ProcessWorkflowShowAllWorkflows
        {
            get
            {
                return _ProcessWorkflowShowAllWorkflows;
            }
            set
            {
                if (_ProcessWorkflowShowAllWorkflows = value)
                {
                    _ProcessWorkflowShowAllWorkflows = value;
                    OnPropertyChanged(nameof(ProcessWorkflowShowAllWorkflows));
                    _ProcessWorkflowList = null;
                    OnPropertyChanged(nameof(ProcessWorkflowList));
                }
            }
        }

        bool _PresenterRightsChecked = false;
        /// <summary>
        /// Gets the VBPresenterMethod instance for displaying and managing process workflows in the picking order interface.
        /// This property lazily initializes a VBPresenterMethod component using ACUrlCommand to load the current design's workflow presenter.
        /// If the presenter cannot be loaded and user rights have not been checked yet, an error message is displayed indicating insufficient rights
        /// for viewing workflows, prompting the user to assign appropriate rights in the group management.
        /// The presenter is used to visualize and interact with workflow definitions associated with picking operations.
        /// </summary>
        public VBPresenterMethod ProcessWorkflowPresenter
        {
            get
            {
                if (_presenter == null)
                {
                    _presenter = this.ACUrlCommand("VBPresenterMethod(CurrentDesign)") as VBPresenterMethod;
                    if (_presenter == null && !_PresenterRightsChecked)
                        Messages.Error(this, "This user has no rights for viewing workflows. Assign rights for VBPresenterMethod in the group management!", true);
                    _PresenterRightsChecked = true;
                }
                return _presenter;
            }
        }

        private gipCoreData.ACClassMethod _CurrentProcessWorkflow;
        /// <summary>
        /// Gets or sets the current process workflow method for the picking order.
        /// This property represents the currently active workflow method that defines the process steps and execution logic for the picking operation.
        /// It is synchronized with the SelectedProcessWorkflow property and triggers property change notifications to update bound UI controls when modified.
        /// </summary>
        [ACPropertySelected(628, "", "en{'Process Workflow'}de{'Prozess-Workflow'}",
                            Description = @"Gets or sets the current process workflow method for the picking order.
                                            This property represents the currently active workflow method that defines the process steps and execution logic for the picking operation.
                                            It is synchronized with the SelectedProcessWorkflow property and triggers property change notifications to update bound UI controls when modified.")]
        public gipCoreData.ACClassMethod CurrentProcessWorkflow
        {
            get => _CurrentProcessWorkflow;
            set
            {
                _CurrentProcessWorkflow = value;
                OnPropertyChanged();
            }
        }

        private gipCoreData.ACClassMethod _SelectedProcessWorkflow;
        /// <summary>
        /// Gets or sets the currently selected process workflow method for the picking order.
        /// This property represents the selected workflow method that defines the process steps, execution logic,
        /// and routing for the picking operation. When changed, it automatically updates the process workflow presenter
        /// to reflect the new workflow configuration and triggers property change notifications to update bound UI controls.
        /// The selected workflow method is used to determine how the picking order will be processed, including
        /// material movements, facility bookings, and workflow execution steps within the logistics system.
        /// </summary>
        [ACPropertySelected(628, "ProcessWorkflow", "en{'Process Workflow'}de{'Prozess-Workflow'}",
                            Description = @"Gets or sets the currently selected process workflow method for the picking order.
                                            This property represents the selected workflow method that defines the process steps, execution logic,
                                            and routing for the picking operation. When changed, it automatically updates the process workflow presenter
                                            to reflect the new workflow configuration and triggers property change notifications to update bound UI controls.
                                            The selected workflow method is used to determine how the picking order will be processed, including
                                            material movements, facility bookings, and workflow execution steps within the logistics system.")]
        public gipCoreData.ACClassMethod SelectedProcessWorkflow
        {
            get
            {
                return _SelectedProcessWorkflow;
            }
            set
            {
                if (_SelectedProcessWorkflow != value)
                {
                    _SelectedProcessWorkflow = value;
                    LoadProcessWorkflowPresenter(CurrentPicking);
                    OnPropertyChanged(nameof(SelectedProcessWorkflow));
                }
            }
        }

        private List<gipCoreData.ACClassMethod> _ProcessWorkflowList;
        /// <summary>
        /// Gets the list of available process workflow methods for picking operations.
        /// This property provides a collection of ACClassMethod entities that define workflow processes
        /// applicable to picking orders, including transport and logistics workflows. The list is loaded
        /// lazily from the database and cached for performance, filtering workflows based on their
        /// association with picking-related materials and workflow classes. Workflows are ordered by
        /// caption and identifier for consistent presentation in user interfaces.
        /// </summary>
        [ACPropertyList(629, "ProcessWorkflow",
                        Description = @"Gets the list of available process workflow methods for picking operations.
                                        This property provides a collection of ACClassMethod entities that define workflow processes
                                        applicable to picking orders, including transport and logistics workflows. The list is loaded
                                        lazily from the database and cached for performance, filtering workflows based on their
                                        association with picking-related materials and workflow classes. Workflows are ordered by
                                        caption and identifier for consistent presentation in user interfaces.")]
        public List<gipCoreData.ACClassMethod> ProcessWorkflowList
        {
            get
            {
                if (_ProcessWorkflowList == null)
                    _ProcessWorkflowList = LoadProcessWorkflowList();
                return _ProcessWorkflowList;
            }
        }

        private List<gipCoreData.ACClassMethod> LoadProcessWorkflowList()
        {
            string pwClassNameOfRoot = nameof(PWMethodTransportBase);

            List<gipCoreData.ACClassMethod> tempList = DatabaseApp.ContextIPlus.ACClassMethod
                                                                  .Where(c => c.ACKindIndex == (Int16)Global.ACKinds.MSWorkflow
                                                                           && c.ACClassWF_ACClassMethod
                                                                               .Any(wf => !wf.ParentACClassWFID.HasValue
                                                                                       && (wf.PWACClass.ACIdentifier == pwClassNameOfRoot
                                                                                       || (wf.PWACClass.BasedOnACClassID.HasValue
                                                                                       && (wf.PWACClass.ACClass1_BasedOnACClass.ACIdentifier == pwClassNameOfRoot
                                                                                       || (wf.PWACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                        && wf.PWACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pwClassNameOfRoot))))
                                                                                       && !wf.ACClassMethod.IsSubMethod))
                                                                  .ToArray()
                                                                  .OrderBy(c => c.ACCaption)
                                                                  .ThenBy(c => c.ACIdentifier)
                                                                  .ToList();

            MDMaterialGroup pickingGroup = DatabaseApp.MDMaterialGroup.FirstOrDefault(c => c.MDMaterialGroupIndex == (short)MDMaterialGroup.MaterialGroupTypes.Picking);

            MaterialWF materialWf = DatabaseApp.MaterialWF.Include(c => c.MaterialWFACClassMethod_MaterialWF)
                                                          .FirstOrDefault(c => c.MaterialWFRelation_MaterialWF
                                                                                .Any(x => x.SourceMaterial.MDMaterialGroupID == pickingGroup.MDMaterialGroupID
                                                                                       || x.TargetMaterial.MDMaterialGroupID == pickingGroup.MDMaterialGroupID));

            if (materialWf != null)
            {
                List<Guid> relatedACClassMethods = materialWf.MaterialWFACClassMethod_MaterialWF.OrderBy(x => x.ACClassMethod.ACCaption)
                                                                                                .ThenBy(x => x.ACClassMethod.ACIdentifier)
                                                                                                .Select(c => c.ACClassMethodID).ToList();
                if (relatedACClassMethods.Any())
                {
                    tempList = tempList.OrderBy(c =>
                                        {
                                            int index = relatedACClassMethods.IndexOf(c.ACClassMethodID);
                                            if (index == -1)
                                                index = tempList.IndexOf(c) + 10000;
                                            return index;
                                        }).ToList();
                }
            }

            return tempList;
        }

        #endregion

        #region Workflow -> Methods

        #region Workflow -> Methods -> Interaction

        /// <summary>
        /// Opens a dialog to assign a process workflow to the current picking order.
        /// This method checks if workflow assignment is enabled and displays the "SelectWorkflowDialog"
        /// to allow users to choose and assign a workflow method for processing the picking order.
        /// </summary>
        [ACMethodInteraction("ProcessWorkflow", "en{'Add'}de{'Hinzufügen'}", (short)MISort.New, true, "CurrentProcessWorkflow",
                             Description = @"Opens a dialog to assign a process workflow to the current picking order.
                                             This method checks if workflow assignment is enabled and displays the ""SelectWorkflowDialog""
                                             to allow users to choose and assign a workflow method for processing the picking order.")]
        public void ProcessWorkflowAssign()
        {
            if (!IsEnabledProcessWorkflowAssign())
                return;

            ShowDialog(this, "SelectWorkflowDialog");
        }

        /// <summary>
        /// Determines whether a process workflow can be assigned to the current picking order.
        /// Returns true if there is a current picking order selected, allowing users to assign
        /// a workflow method for processing the picking operation.
        /// </summary>
        /// <returns>True if a picking order is selected and workflow assignment is enabled; otherwise, false.</returns>
        public bool IsEnabledProcessWorkflowAssign()
        {
            return CurrentPicking != null;
        }

        /// <summary>
        /// Confirms the selection of a process workflow and assigns it to the current picking order.
        /// Updates the picking order's ACClassMethod, refreshes the workflow presenter, and closes the selection dialog.
        /// Triggers property change notifications to update the enabled state of workflow assignment and cancellation buttons.
        /// </summary>
        [ACMethodInteraction("ProcessWorkflow", "en{'Add'}de{'Hinzufügen'}", (short)MISort.New, true, "CurrentProcessWorkflow",
                             Description = @"Confirms the selection of a process workflow and assigns it to the current picking order.
                                             Updates the picking order's ACClassMethod, refreshes the workflow presenter, and closes the selection dialog.
                                             Triggers property change notifications to update the enabled state of workflow assignment and cancellation buttons.")]
        public void ProcessWorkflowAssignOk()
        {
            CurrentPicking.ACClassMethod = SelectedProcessWorkflow.FromAppContext<gip.mes.datamodel.ACClassMethod>(DatabaseApp);
            CurrentProcessWorkflow = SelectedProcessWorkflow;
            OnPropertyChanged(nameof(IsEnabledProcessWorkflowAssign));
            OnPropertyChanged(nameof(IsEnabledProcessWorkflowCancel));

            LoadProcessWorkflowPresenter(CurrentPicking);
            CloseTopDialog();
        }

        private void LoadSelectedProcessWorkflow(Picking picking)
        {
            if (ProcessWorkflowList != null && ProcessWorkflowList.Any() && picking != null && picking.ACClassMethodID != null)
            {
                _SelectedProcessWorkflow = ProcessWorkflowList.FirstOrDefault(p => p.ACClassMethodID == picking.ACClassMethodID);
            }
            else
            {
                _SelectedProcessWorkflow = null;
            }
            OnPropertyChanged(nameof(SelectedProcessWorkflow));
            CurrentProcessWorkflow = SelectedProcessWorkflow;
        }

        private void LoadProcessWorkflowPresenter(Picking picking)
        {
            if (ProcessWorkflowPresenter != null)
            {
                gipCoreData.ACClassMethod method = (picking != null && picking.ACClassMethod != null) ? picking.ACClassMethod.FromIPlusContext<gipCoreData.ACClassMethod>(DatabaseApp.ContextIPlus) : null;
                ProcessWorkflowPresenter.Load(method);
            }
        }

        /// <summary>
        /// Determines whether the workflow assignment can be confirmed in the selection dialog.
        /// Returns true if a process workflow has been selected; otherwise, false.
        /// This method is used to enable/disable the OK button in the workflow assignment dialog.
        /// </summary>
        /// <returns>True if a process workflow is selected and assignment can proceed; otherwise, false.</returns>
        public bool IsEnabledProcessWorkflowAssignOk()
        {
            return SelectedProcessWorkflow != null;
        }


        /// <summary>
        /// Cancels the assignment of the process workflow to the current picking order.
        /// This method removes the workflow association by clearing the ACClassMethod reference,
        /// resets the selected and current process workflow properties to null, and triggers
        /// property change notifications to update the UI state for workflow assignment and cancellation buttons.
        /// </summary>
        [ACMethodInteraction("ProcessWorkflow", "en{'Remove'}de{'Entfernen'}", (short)MISort.Delete, true, "CurrentProcessWorkflow",
                             Description = @"Cancels the assignment of the process workflow to the current picking order.
                                             This method removes the workflow association by clearing the ACClassMethod reference,
                                             resets the selected and current process workflow properties to null, and triggers
                                             property change notifications to update the UI state for workflow assignment and cancellation buttons.")]
        public void ProcessWorkflowCancel()
        {
            if (!IsEnabledProcessWorkflowCancel()) return;
            CurrentPicking.ACClassMethod = null;
            CurrentPicking.ACClassMethodID = null;

            if (CurrentProcessWorkflow != SelectedProcessWorkflow)
                SelectedProcessWorkflow = CurrentProcessWorkflow;

            SelectedProcessWorkflow = null;
            CurrentProcessWorkflow = null;
            OnPropertyChanged(nameof(IsEnabledProcessWorkflowAssign));
            OnPropertyChanged(nameof(IsEnabledProcessWorkflowCancel));
        }

        /// <summary>
        /// Determines whether the process workflow can be cancelled for the current picking order.
        /// Returns true if there is a current picking order and it has an associated workflow method (ACClassMethodID is not null).
        /// </summary>
        /// <returns>True if the process workflow can be cancelled; otherwise, false.</returns>
        public bool IsEnabledProcessWorkflowCancel()
        {
            return CurrentPicking != null && CurrentPicking.ACClassMethodID != null;
        }

        #endregion

        #endregion

        #endregion

        #region IACBSOConfigStoreSelection

        /// <summary>
        /// Gets the list of mandatory configuration stores required for the current picking order context.
        /// This property returns a list containing the current picking order (CurrentPicking) if it exists,
        /// which serves as the primary configuration store for workflow and parameter management.
        /// The list is used by the IACBSOConfigStoreSelection interface to determine which entities
        /// must be included in configuration operations and parameter validation.
        /// Returns an empty list if no current picking order is selected.
        /// </summary>
        public List<IACConfigStore> MandatoryConfigStores
        {
            get
            {
                List<IACConfigStore> listOfSelectedStores = new List<IACConfigStore>();
                if (CurrentPicking != null)
                {
                    listOfSelectedStores.Add(CurrentPicking);
                }
                return listOfSelectedStores;
            }
        }

        /// <summary>
        /// Gets the current configuration store for the picking order interface.
        /// This property returns the currently selected Picking entity as the configuration store,
        /// which serves as the primary entity for storing and retrieving configuration data
        /// related to picking operations, workflow parameters, and material movements.
        /// Returns null if no current picking order is selected.
        /// This property is part of the IACBSOConfigStoreSelection interface and is used
        /// by configuration management components to access the active picking order's
        /// configuration settings and parameters.
        /// </summary>
        public IACConfigStore CurrentConfigStore
        {
            get
            {
                if (CurrentPicking == null) return null;
                return CurrentPicking;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the configuration store is read-only.
        /// Always returns false, indicating that the picking order configuration can be modified.
        /// </summary>
        public bool IsReadonly
        {
            get
            {
                return false;
            }
        }

        private List<core.datamodel.ACClassMethod> _VisitedMethods;
        /// <summary>
        /// Gets or sets the list of visited workflow methods during navigation.
        /// This property tracks the ACClassMethod instances that represent workflow methods
        /// accessed by the user in subworkflows, helping to determine navigation history
        /// and support features like backtracking or workflow state management.
        /// </summary>
        public List<core.datamodel.ACClassMethod> VisitedMethods
        {
            get
            {
                if (_VisitedMethods == null)
                    _VisitedMethods = new List<core.datamodel.ACClassMethod>();
                return _VisitedMethods;
            }
            set
            {
                _VisitedMethods = value;
                OnPropertyChanged(nameof(VisitedMethods));
            }
        }

        /// <summary>
        /// Adds the specified workflow method to the list of visited methods if it is not already present.
        /// This method is used to track which subworkflows the user has navigated to, helping to determine
        /// the current navigation state within the workflow hierarchy for configuration and UI purposes.
        /// </summary>
        /// <param name="acClassMethod">The workflow method (ACClassMethod) that has been visited/navigated to.</param>
        public void AddVisitedMethods(core.datamodel.ACClassMethod acClassMethod)
        {
            if (!VisitedMethods.Contains(acClassMethod))
                VisitedMethods.Add(acClassMethod);
        }

        #endregion

        #region IACBSOACProgramProvider

        private bool _IsEnabledACProgram = true;

        /// <summary>
        /// Gets or sets a value indicating whether AC program functionality is enabled for this business service object.
        /// When enabled, allows access to AC program-related operations and workflows within the picking order management interface.
        /// </summary>
        public bool IsEnabledACProgram
        {
            get
            {
                return _IsEnabledACProgram;
            }
            set
            {
                _IsEnabledACProgram = value;
            }
        }

        /// <summary>
        /// Gets the ACUrl of the currently selected workflow node relative to the root workflow node.
        /// Returns null if the ProcessWorkflowPresenter is null or if the selected root workflow node's ACIdentifier
        /// matches the selected ACUrl, indicating no relative path is needed.
        /// This property is used to provide navigation context within workflow hierarchies for program execution and tracking.
        /// </summary>
        public string WorkflowACUrl
        {
            get
            {
                if (ProcessWorkflowPresenter == null)
                    return null;
                if (ProcessWorkflowPresenter.SelectedRootWFNode.ACIdentifier == ProcessWorkflowPresenter.SelectedACUrl)
                    return null;
                else
                    return ProcessWorkflowPresenter.SelectedWFNode.GetACUrl(ProcessWorkflowPresenter.SelectedRootWFNode);
            }
        }

        /// <summary>
        /// Gets the collection of ACProgram instances associated with the current picking order.
        /// This method retrieves distinct ACProgram entities by querying OrderLog entries linked to picking positions
        /// of the current picking order, converting them from the IPlus context to the application context.
        /// Used by the IACBSOACProgramProvider interface to provide program information for workflow execution and tracking.
        /// </summary>
        /// <returns>An IEnumerable of distinct ACProgram instances related to the current picking order's order logs.</returns>
        public IEnumerable<gipCoreData.ACProgram> GetACProgram()
        {
            return
                DatabaseApp
                .OrderLog
                .Where(c => c.PickingPos.PickingID == CurrentPicking.PickingID)
                .ToList()
                .Select(x => x.VBiACProgramLog.ACProgram.FromIPlusContext<gip.core.datamodel.ACProgram>(DatabaseApp.ContextIPlus))
                .Distinct();
        }

        /// <summary>
        /// Gets the program number for the production order associated with the current picking order.
        /// This method returns the translated caption (ACCaption) of the current picking order, which serves as the program number
        /// for workflow execution and tracking purposes within the IACBSOACProgramProvider interface.
        /// The program number is typically used to identify and reference the picking order in automated processes,
        /// logging, and program management systems.
        /// </summary>
        /// <returns>The program number as a string, representing the caption of the current picking order.</returns>
        public string GetProdOrderProgramNo()
        {
            return CurrentPicking.ACCaption;
        }

        /// <summary>
        /// Gets the insert date of the production order associated with the current picking order.
        /// This method returns the InsertDate property of the CurrentPicking entity, which represents
        /// when the picking order was initially created in the database. It is used by the IACBSOACProgramProvider
        /// interface for workflow execution and tracking purposes, providing the creation timestamp
        /// of the picking order that serves as the production order in this context.
        /// </summary>
        /// <returns>The DateTime when the current picking order was inserted into the database.</returns>
        public DateTime GetProdOrderInsertDate()
        {
            return CurrentPicking.InsertDate;
        }

        #endregion

        #region Messages

        /// <summary>
        /// Sends a message by adding it to the message list and notifying property change for UI updates.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="msg">The message to send.</param>
        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged(nameof(MsgList));
        }

        #region Messages -> Properties

        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the currently selected message for display and editing operations.
        /// This property represents the active message from the MsgList that is being viewed or modified
        /// in the user interface. When set, it updates the current message reference and triggers
        /// property change notifications to update bound UI controls. The message can contain
        /// error, warning, or information details related to picking operations and workflows.
        /// </summary>
        [ACPropertyCurrent(9999, "Message", "en{'Message'}de{'Meldung'}",
                           Description = @"Gets or sets the currently selected message for display and editing operations.
                                           This property represents the active message from the MsgList that is being viewed or modified
                                           in the user interface. When set, it updates the current message reference and triggers
                                           property change notifications to update bound UI controls. The message can contain
                                           error, warning, or information details related to picking operations and workflows.")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged(nameof(CurrentMsg));
            }
        }

        private ObservableCollection<Msg> msgList;
        /// <summary>
        /// Gets the message list containing error, warning, and information messages related to picking operations.
        /// This property provides an observable collection of Msg objects that can be used to display notifications
        /// to users during picking order processing, facility bookings, and workflow executions. Messages are added
        /// dynamically during operations such as validation, booking attempts, or route calculations, and are
        /// automatically updated in bound UI controls due to the ObservableCollection implementation.
        /// The collection is lazily initialized on first access to ensure efficient memory usage.
        /// </summary>
        [ACPropertyList(9999, "Message", "en{'Messagelist'}de{'Meldungsliste'}",
                        Description = @"Gets the message list containing error, warning, and information messages related to picking operations.
                                        This property provides an observable collection of Msg objects that can be used to display notifications
                                        to users during picking order processing, facility bookings, and workflow executions. Messages are added
                                        dynamically during operations such as validation, booking attempts, or route calculations, and are
                                        automatically updated in bound UI controls due to the ObservableCollection implementation.
                                        The collection is lazily initialized on first access to ensure efficient memory usage.")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        #endregion

        #endregion

        #region Overrides

        #region Overrides -> PrintOrderInfo & GS1
        public override PAOrderInfo GetOrderInfo()
        {
            if (SelectedFacilityBookingCharge != null)
            {
                FacilityCharge facilityCharge = SelectedFacilityBookingCharge.InwardFacilityCharge != null ? SelectedFacilityBookingCharge.InwardFacilityCharge : SelectedFacilityBookingCharge.OutwardFacilityCharge;
                if (facilityCharge != null && CurrentPickingPos != null)
                {
                    PAOrderInfo info = new PAOrderInfo();
                    info.Add(nameof(FacilityCharge), facilityCharge.FacilityChargeID);
                    info.Add(nameof(Picking), CurrentPickingPos.PickingID);
                    info.Add(nameof(FacilityBookingCharge), SelectedFacilityBookingCharge.FacilityBookingChargeID);
                    return info;

                }
            }
            return base.GetOrderInfo();
        }

        public override Msg FilterByOrderInfo(PAOrderInfo paOrderInfo)
        {
            Msg msg =  base.FilterByOrderInfo(paOrderInfo);
            if(paOrderInfo != null)
            {
                PAOrderInfoEntry fcEntry = paOrderInfo.Entities.Where(c=>c.EntityName == nameof(FacilityCharge)).FirstOrDefault();
                if(fcEntry != null)
                {
                    ReportFacilityCharge = DatabaseApp.FacilityCharge.Where(c=>c.FacilityChargeID == fcEntry.EntityID).FirstOrDefault();
                    if(ReportFacilityCharge != null)
                    {
                        ReportFacilityCharge.FBCTargetQuantityUOM = ACFacilityManager.GetFacilityBookingQuantityUOM(DatabaseApp, paOrderInfo);
                    }
                }
            }

            return msg;
        }

        #endregion

        #endregion

    }
}
