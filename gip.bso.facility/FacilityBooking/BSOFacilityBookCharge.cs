// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 11-08-2012
// ***********************************************************************
// <copyright file="BSOFacilityBookCharge.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static gip.core.datamodel.Global;

namespace gip.bso.facility
{
    /// <summary>
    /// Businessobject or App for managing the facility charges (quants) in the warehouse:
    /// booking inward (goods receipt), outward (goods issue), stock transfers (relocation),
    /// releasing/locking, availability (zero stock) states, splitting quants and reassigning material or lot.
    /// This class extends the <see cref="BSOFacilityBase"/> and provides methods for booking quantities on facility charges.
    /// To search records enter the search string in the SearchWord property.
    /// The database result is copied to the FacilityChargeList property.
    /// Then call the NavigateFirst() method to set CurrentFacilityCharge with the first record in the list.
    /// CurrentFacilityCharge is used to display and edit the currently selected quant.
    /// Property changes should always be made to CurrentFacilityCharge and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new quant (FacilityCharge) and assigns the new entity object to the CurrentFacilityCharge property.
    /// Assign the material (CurrentFacilityCharge.Material), the storage bin (CurrentFacilityCharge.Facility) and - for lot-managed materials - the lot (CurrentFacilityCharge.FacilityLot, or create a new one with FacilityChargeLotGenerateDlg()).
    /// A lot-managed quant without a lot cannot be saved (error 50552). Then call the Save() method to save the changes in the database.
    /// The Delete() method is currently not implemented (no-op): quants are not deleted by this BSO, use the booking functions instead.
    /// The Load() method updates the CurrentFacilityCharge object with fresh database data if another user has made changes in the background.
    /// Every booking is executed with booking parameters (ACMethodBooking) which are held in the CurrentBookParam... properties.
    /// The ClearBookingData() method ("New Booking Record") resets all booking parameters and pre-assigns the current quant to them. It is called automatically after each successful booking.
    /// Inward posting: enter the quantity in CurrentBookParamInwardMovement.InwardQuantity and call the InwardFacilityChargeMovement() method to book a stock receipt on the current quant.
    /// Outward posting: enter the quantity in CurrentBookParamOutwardMovement.OutwardQuantity and call the OutwardFacilityChargeMovement() method to book a stock issue from the current quant.
    /// Stock transfer: enter the quantity in CurrentBookParamRelocation.OutwardQuantity and the target storage bin in CurrentBookParamRelocation.InwardFacility (selectable via the ShowDlgFacilityRelocation() dialog) and call the FacilityChargeRelocation() method.
    /// If the target bin is connected to a plant/transport system, the user is asked whether the transport should run in automatic mode; in automatic mode a picking order is created and a workflow started, otherwise the relocation is booked directly with BookRelocation().
    /// Releasing/locking: the LockFacilityCharge(), LockFacilityChargeAbsolute(), ReleaseFacilityCharge() and ReleaseFacilityChargeAbsolute() methods change the release state (MDReleaseState) of the current quant.
    /// Availability: the NotAvailableFacilityCharge() method marks the current quant as not available (zero stock), the AvailableFacilityCharge() method makes it available again (optionally restoring the last stock after confirmation).
    /// NotAvailableFacilityChargeAll() and AvailableFacilityChargeAll() apply this to all quants in the explorer list after a confirmation question.
    /// Reassignment: select the suggested material in the SelectedRassignmentMaterial property (or set CurrentBookParamReassignMat.InwardMaterial) and call the FacilityReassign() method to reassign the material of the quant.
    /// The FacilityLotReassign() method reassigns the lot of the quant (CurrentBookParamReassignLot.InwardFacilityLot) and the FacilityLotChangeExpirationDate() method changes the expiration date of the lot (CurrentBookParamChangeExpirationDate.ExpirationDate).
    /// Splitting: enter the split quantity in CurrentBookParamSplit.OutwardQuantity and the new split number in CurrentBookParamSplit.InwardSplitNo and call the SplitQuant() method to split the current quant.
    /// After a successful booking the booking data is cleared; messages and warnings of the booking are shown via the BSOMsg property.
    /// The filters of the explorer list are provided by the ShowNotAvailable, FilterMaterial, FilterFacility, FilterLot, FilterExternLot, FilterExternLot2, StockQuantityLessThan and FilterExpirationDate properties; changing a filter triggers a new search automatically.
    /// The SetFilterExpirationDateToday(), SetFilterExpirationDateNextDays() and ResetFilterExpirationDate() methods set or reset the expiration date filter.
    /// The navigation methods NavigateToOrder(), NavigateToFacilityChargeHistory(), NavigateToMaterialOverview(), NavigateToMaterial(), NavigateToFacilityLotOverview(), NavigateToFacilityLot(), NavigateToFacilityOverview() and NavigateToFacility() open the corresponding dialogs for the selected quant.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityBookCharge&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Quant Management'}de{'Quantenverwaltung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + FacilityCharge.ClassName,
                 Description = @"Businessobject or App for managing the facility charges (quants) in the warehouse: booking inward (goods receipt), outward (goods issue), stock transfers (relocation), releasing/locking, availability (zero stock) states, splitting quants and reassigning material or lot.
                                This class extends the <see cref=""BSOFacilityBase""/> and provides methods for booking quantities on facility charges.
                                To search records enter the search string in the SearchWord property.
                                The database result is copied to the FacilityChargeList property.
                                Then call the NavigateFirst() method to set CurrentFacilityCharge with the first record in the list.
                                CurrentFacilityCharge is used to display and edit the currently selected quant.
                                Property changes should always be made to CurrentFacilityCharge and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
                                The New() method creates a new quant (FacilityCharge) and assigns the new entity object to the CurrentFacilityCharge property.
                                Assign the material (CurrentFacilityCharge.Material), the storage bin (CurrentFacilityCharge.Facility) and - for lot-managed materials - the lot (CurrentFacilityCharge.FacilityLot, or create a new one with FacilityChargeLotGenerateDlg()).
                                A lot-managed quant without a lot cannot be saved (error 50552). Then call the Save() method to save the changes in the database.
                                The Delete() method is currently not implemented (no-op): quants are not deleted by this BSO, use the booking functions instead.
                                The Load() method updates the CurrentFacilityCharge object with fresh database data if another user has made changes in the background.
                                Every booking is executed with booking parameters (ACMethodBooking) which are held in the CurrentBookParam... properties.
                                The ClearBookingData() method (""New Booking Record"") resets all booking parameters and pre-assigns the current quant to them. It is called automatically after each successful booking.
                                Inward posting: enter the quantity in CurrentBookParamInwardMovement.InwardQuantity and call the InwardFacilityChargeMovement() method to book a stock receipt on the current quant.
                                Outward posting: enter the quantity in CurrentBookParamOutwardMovement.OutwardQuantity and call the OutwardFacilityChargeMovement() method to book a stock issue from the current quant.
                                Stock transfer: enter the quantity in CurrentBookParamRelocation.OutwardQuantity and the target storage bin in CurrentBookParamRelocation.InwardFacility (selectable via the ShowDlgFacilityRelocation() dialog) and call the FacilityChargeRelocation() method.
                                If the target bin is connected to a plant/transport system, the user is asked whether the transport should run in automatic mode; in automatic mode a picking order is created and a workflow started, otherwise the relocation is booked directly with BookRelocation().
                                Releasing/locking: the LockFacilityCharge(), LockFacilityChargeAbsolute(), ReleaseFacilityCharge() and ReleaseFacilityChargeAbsolute() methods change the release state (MDReleaseState) of the current quant.
                                Availability: the NotAvailableFacilityCharge() method marks the current quant as not available (zero stock), the AvailableFacilityCharge() method makes it available again (optionally restoring the last stock after confirmation).
                                NotAvailableFacilityChargeAll() and AvailableFacilityChargeAll() apply this to all quants in the explorer list after a confirmation question.
                                Reassignment: select the suggested material in the SelectedRassignmentMaterial property (or set CurrentBookParamReassignMat.InwardMaterial) and call the FacilityReassign() method to reassign the material of the quant.
                                The FacilityLotReassign() method reassigns the lot of the quant (CurrentBookParamReassignLot.InwardFacilityLot) and the FacilityLotChangeExpirationDate() method changes the expiration date of the lot (CurrentBookParamChangeExpirationDate.ExpirationDate).
                                Splitting: enter the split quantity in CurrentBookParamSplit.OutwardQuantity and the new split number in CurrentBookParamSplit.InwardSplitNo and call the SplitQuant() method to split the current quant.
                                After a successful booking the booking data is cleared; messages and warnings of the booking are shown via the BSOMsg property.
                                The filters of the explorer list are provided by the ShowNotAvailable, FilterMaterial, FilterFacility, FilterLot, FilterExternLot, FilterExternLot2, StockQuantityLessThan and FilterExpirationDate properties; changing a filter triggers a new search automatically.
                                The SetFilterExpirationDateToday(), SetFilterExpirationDateNextDays() and ResetFilterExpirationDate() methods set or reset the expiration date filter.
                                The navigation methods NavigateToOrder(), NavigateToFacilityChargeHistory(), NavigateToMaterialOverview(), NavigateToMaterial(), NavigateToFacilityLotOverview(), NavigateToFacilityLot(), NavigateToFacilityOverview() and NavigateToFacility() open the corresponding dialogs for the selected quant.
                                Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOFacilityBookCharge&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
                                ")]
    [ACQueryInfo(Const.PackName_VarioFacility, Const.QueryPrefix + FacilityLot.ClassName, "en{'Quant Management'}de{'Quantenverwaltung'}", typeof(FacilityLot), FacilityLot.ClassName, "LotNo", "LotNo")]
    public class BSOFacilityBookCharge : BSOFacilityBase
    {

        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityBookCharge"/> class.
        /// </summary>
        /// <param name="typeACClass">The type AC class.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityBookCharge(gip.core.datamodel.ACClass typeACClass, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(typeACClass, content, parentACObject, parameter)
        {
            _ExpirationDateDayPeriod = new ACPropertyConfigValue<int>(this, nameof(ExpirationDateDayPeriod), 30);
            _IsCheckPreferredParamsActive = new ACPropertyConfigValue<bool>(this, nameof(IsCheckPreferredParamsActive), true);
            _IsIncludeDisabled = new ACPropertyConfigValue<bool>(this, nameof(IsIncludeDisabled), false);
            //_AutoSearchAfterMassPosting = new ACPropertyConfigValue<bool>(this, nameof(AutoSearchAfterMassPosting), true);
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
            bool skipSearchOnStart = ParameterValueT<bool>(Const.SkipSearchOnStart);
            if (!skipSearchOnStart)
                Search();

            _ = ExpirationDateDayPeriod;
            _ = IsCheckPreferredParamsActive;
            _ = IsIncludeDisabled;
            //_ = AutoSearchAfterMassPosting

            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {

            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;
            this._BookParamInwardMovement = null;
            this._BookParamInwardMovementClone = null;
            this._BookParamNotAvailable = null;
            this._BookParamNotAvailableClone = null;
            this._BookParamOutwardMovement = null;
            this._BookParamOutwardMovementClone = null;
            this._BookParamReassignMat = null;
            this._BookParamReassignMatClone = null;
            this._BookParamReassignLot = null;
            this._BookParamReassignLotClone = null;
            this._BookParamReleaseAndLock = null;
            this._BookParamReleaseAndLockClone = null;
            this._BookParamRelocation = null;
            this._BookParamRelocationClone = null;
            this._BookParamSplit = null;
            this._BookParamSplitClone = null;
            this._AccessFacilityLot = null;
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessFacilityLot != null)
            {
                await _AccessFacilityLot.ACDeInit(false);
                _AccessFacilityLot = null;
            }
            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }
        #endregion

        #region Configuration

        private ACPropertyConfigValue<int> _ExpirationDateDayPeriod;
        [ACPropertyConfig("en{'Expiration date filter period'}de{'Ablaufdatum Filterperiode'}")]
        public int ExpirationDateDayPeriod
        {
            get
            {
                return _ExpirationDateDayPeriod.ValueT;
            }
            set
            {
                if (_ExpirationDateDayPeriod.ValueT != value)
                {
                    _ExpirationDateDayPeriod.ValueT = value;
                    OnPropertyChanged();
                }
            }
        }


        private ACPropertyConfigValue<bool> _IsCheckPreferredParamsActive;
        [ACPropertyConfig("en{'Active check preferred params'}de{'Überprüfung von bevorzugten Parameter aktiv'}")]
        public bool IsCheckPreferredParamsActive
        {
            get
            {
                return _IsCheckPreferredParamsActive.ValueT;
            }
            set
            {
                if (_IsCheckPreferredParamsActive.ValueT != value)
                {
                    _IsCheckPreferredParamsActive.ValueT = value;
                    OnPropertyChanged();
                }
            }
        }

        private ACPropertyConfigValue<bool> _IsIncludeDisabled;
        [ACPropertyConfig("en{'Disable include'}de{'Include deaktivieren'}")]
        public bool IsIncludeDisabled
        {
            get
            {
                return _IsIncludeDisabled.ValueT;
            }
            set
            {
                if (_IsIncludeDisabled.ValueT != value)
                {
                    _IsIncludeDisabled.ValueT = value;
                    OnPropertyChanged();
                }
            }
        }

        //private ACPropertyConfigValue<bool> _AutoSearchAfterMassPosting;
        //[ACPropertyConfig("en{'Disable include'}de{'Include deaktivieren'}", DefaultValue = true)]
        //public bool AutoSearchAfterMassPosting
        //{
        //    get
        //    {
        //        return _AutoSearchAfterMassPosting.ValueT;
        //    }
        //    set
        //    {
        //        if (_AutoSearchAfterMassPosting.ValueT != value)
        //        {
        //            _AutoSearchAfterMassPosting.ValueT = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        #endregion

        #region BSO->ACProperty

        #region ChildBSO

        ACChildItem<BSOFacilityExplorer> _BSOFacilityExplorer_Child;
        [ACPropertyInfo(600, Description = "Child BSO instance of the BSOFacilityExplorer, used e.g. by the relocation dialog to select the target storage bin.")]
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

        #endregion

        #region Filters
        public const string _CNotAvailableProperty = "NotAvailable";
        [ACPropertyInfo(712, "Filter", "en{'Show not available'}de{'Nicht verfügbare anzeigen'}", Description = "Filter: when set, quants that are marked as not available are also shown in the explorer list. Changing this filter triggers a new search.")]
        public virtual bool? ShowNotAvailable
        {
            get
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CNotAvailableProperty);
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<bool>(_CNotAvailableProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CNotAvailableProperty);
                if (!value.HasValue)
                {
                    if (String.IsNullOrEmpty(tmp))
                        return;
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CNotAvailableProperty, "");
                    OnPropertyChanged();
                    return;
                }
                if (String.IsNullOrEmpty(tmp)
                    || AccessPrimary.NavACQueryDefinition.GetSearchValue<bool>(_CNotAvailableProperty) != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<bool>(_CNotAvailableProperty, value.Value);
                    OnPropertyChanged();
                    return;
                }
            }
        }


        public const string _CMaterialNoProperty = Material.ClassName + "\\MaterialNo";
        public const string _CMaterialNameProperty = Material.ClassName + "\\MaterialName1";
        [ACPropertyInfo(713, "Filter", "en{'Material'}de{'Material'}", Description = "Filter: search text for the material number or material name of the quant. Changing this filter triggers a new search.")]
        public string FilterMaterial
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CMaterialNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CMaterialNoProperty);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNoProperty, value);
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNameProperty, value);
                    OnPropertyChanged();
                }
            }
        }

        public const string _CFacilityNoProperty = Facility.ClassName + "\\FacilityNo";
        public const string _CFacilityNameProperty = Facility.ClassName + "\\FacilityName";
        [ACPropertyInfo(714, "Filter", "en{'Storage'}de{'Lagerplatz'}", Description = "Filter: search text for the storage bin number or name of the quant. Changing this filter triggers a new search.")]
        public string FilterFacility
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CFacilityNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CFacilityNoProperty);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CFacilityNoProperty, value);
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CFacilityNameProperty, value);
                    OnPropertyChanged();
                }
            }
        }


        public const string _CLotNoProperty = FacilityLot.ClassName + "\\LotNo";
        [ACPropertyInfo(715, "Filter", ConstApp.LotNo, Description = "Filter: search text for the internal lot number of the quant. Changing this filter triggers a new search.")]
        public string FilterLot
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CLotNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CLotNoProperty);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CLotNoProperty, value);
                    OnPropertyChanged();
                }
            }
        }

        public const string _CExternLotNoProperty = FacilityLot.ClassName + "\\" + nameof(FacilityLot.ExternLotNo);
        [ACPropertyInfo(715, "Filter", ConstApp.ExternLotNo, Description = "Filter: search text for the first external lot number of the quant. Changing this filter triggers a new search.")]
        public string FilterExternLot
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CExternLotNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CExternLotNoProperty);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CExternLotNoProperty, value);
                    OnPropertyChanged();
                }
            }
        }

        public const string _CExternLotNo2Property = FacilityLot.ClassName + "\\" + nameof(FacilityLot.ExternLotNo2);
        [ACPropertyInfo(715, "Filter", ConstApp.ExternLotNo2, Description = "Filter: search text for the second external lot number of the quant. Changing this filter triggers a new search.")]
        public string FilterExternLot2
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CExternLotNo2Property);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(_CExternLotNo2Property);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CExternLotNo2Property, value);
                    OnPropertyChanged();
                }
            }
        }

        [ACPropertyInfo(716, "Filter", "en{'Stock quantity less than'}de{'Lagermenge weniger als'}", Description = "Filter: shows only quants whose stock quantity is less than this value. Changing this filter triggers a new search.")]
        public double StockQuantityLessThan
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<double>(nameof(FacilityCharge.StockQuantityUOM));
            }
            set
            {
                double tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<double>(nameof(FacilityCharge.StockQuantityUOM));
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<double>(nameof(FacilityCharge.StockQuantityUOM), value);
                    OnPropertyChanged();
                }
            }
        }

        [ACPropertyInfo(717, "Filter", "en{'Expiration date less then'}de{'Mindesthaltbarkeitsdatum dann'}", Description = "Filter: shows only quants whose expiration date is on or before this date. Changing this filter triggers a new search.")]
        public DateTime? FilterExpirationDate
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<DateTime?>(nameof(FacilityCharge.ExpirationDate));
            }
            set
            {
                DateTime? tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<DateTime?>(nameof(FacilityCharge.ExpirationDate));
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<DateTime?>(nameof(FacilityCharge.ExpirationDate), value);
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _SetFilterExpirationDateNextDaysCaption;
        [ACPropertyInfo(999, "SetFilterExpirationDateNextDaysCaption", "en{'TODO:FilterExpirationDateDayPeriod'}de{'TODO:FilterExpirationDateDayPeriod'}", Description = "Computed caption of the 'Next [0] days' button, filled with the configured ExpirationDateDayPeriod.")]
        public string SetFilterExpirationDateNextDaysCaption
        {
            get
            {
                if (_SetFilterExpirationDateNextDaysCaption == null)
                {

                    gip.core.datamodel.ACClassMethod method = this.ACType.ValueTypeACClass.Methods.Where(c => c.ACIdentifier == nameof(SetFilterExpirationDateNextDays)).FirstOrDefault();
                    if (method != null)
                    {
                        _SetFilterExpirationDateNextDaysCaption = method.ACCaption;
                        _SetFilterExpirationDateNextDaysCaption = _SetFilterExpirationDateNextDaysCaption.Replace("[0]", ExpirationDateDayPeriod.ToString());
                    }
                }
                return _SetFilterExpirationDateNextDaysCaption;
            }
        }

        #endregion

        #region BSO->ACProperty->BookingParam
        /// <summary>
        /// Gets the current book param.
        /// </summary>
        /// <value>The current book param.</value>
        [ACPropertyCurrent(701, "BookParam", Description = "Booking parameters of the currently active booking tab; set automatically when a posting tab is activated.")]
        public ACMethodBooking CurrentBookParam
        {
            get
            {
                return _ActBookingParam;
            }
            protected set
            {
                _ActBookingParam = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param inward movement.
        /// </summary>
        /// <value>The current book param inward movement.</value>
        [ACPropertyCurrent(702, "BookParamInwardMovement", Description = "Booking parameters for the inward posting (stock receipt) of the current quant; the quantity is entered in InwardQuantity.")]
        public ACMethodBooking CurrentBookParamInwardMovement
        {
            get
            {
                return _BookParamInwardMovement;
            }
            protected set
            {
                _BookParamInwardMovement = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param outward movement.
        /// </summary>
        /// <value>The current book param outward movement.</value>
        [ACPropertyCurrent(703, "BookParamOutwardMovement", Description = "Booking parameters for the outward posting (stock issue) of the current quant; the quantity is entered in OutwardQuantity.")]
        public ACMethodBooking CurrentBookParamOutwardMovement
        {
            get
            {
                return _BookParamOutwardMovement;
            }
            protected set
            {
                _BookParamOutwardMovement = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param relocation.
        /// </summary>
        /// <value>The current book param relocation.</value>
        [ACPropertyCurrent(704, "BookParamRelocation", Description = "Booking parameters for the stock transfer (relocation) of the current quant; the target storage bin is set in InwardFacility.")]
        public ACMethodBooking CurrentBookParamRelocation
        {
            get
            {
                return _BookParamRelocation;
            }
            protected set
            {
                _BookParamRelocation = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param release and lock.
        /// </summary>
        /// <value>The current book param release and lock.</value>
        [ACPropertyCurrent(705, "BookParamReleaseAndLock", Description = "Booking parameters for changing the release state (lock/unlock) of the current quant.")]
        public ACMethodBooking CurrentBookParamReleaseAndLock
        {
            get
            {
                return _BookParamReleaseAndLock;
            }
            protected set
            {
                _BookParamReleaseAndLock = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param not available.
        /// </summary>
        /// <value>The current book param not available.</value>
        [ACPropertyCurrent(706, "BookParamNotAvailable", Description = "Booking parameters for changing the availability (zero stock) state of the current quant.")]
        public ACMethodBooking CurrentBookParamNotAvailable
        {
            get
            {
                return _BookParamNotAvailable;
            }
            protected set
            {
                _BookParamNotAvailable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param matching.
        /// </summary>
        /// <value>The current book param matching.</value>
        [ACPropertyCurrent(707, "CurrentBookParamReassignMat", Description = "Booking parameters for reassigning the material of the current quant; the new material is set in InwardMaterial.")]
        public ACMethodBooking CurrentBookParamReassignMat
        {
            get
            {
                return _BookParamReassignMat;
            }
            protected set
            {
                _BookParamReassignMat = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param matching.
        /// </summary>
        /// <value>The current book param matching.</value>
        [ACPropertyCurrent(707, "CurrentBookParamSplit", Description = "Booking parameters for splitting the current quant; the split quantity is entered in OutwardQuantity and the new split number in InwardSplitNo.")]
        public ACMethodBooking CurrentBookParamSplit
        {
            get
            {
                return _BookParamSplit;
            }
            protected set
            {
                _BookParamSplit = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Gets the current book param matching.
        /// </summary>
        /// <value>The current book param matching.</value>
        [ACPropertyCurrent(708, "CurrentBookParamReassignLot", Description = "Booking parameters for reassigning the lot of the current quant; the new lot is set in InwardFacilityLot.")]
        public ACMethodBooking CurrentBookParamReassignLot
        {
            get
            {
                return _BookParamReassignLot;
            }
            protected set
            {
                _BookParamReassignLot = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Gets the current book param matching.
        /// </summary>
        /// <value>The current book param matching.</value>
        [ACPropertyCurrent(709, "BookParamChangeExpirationDate", Description = "Booking parameters for changing the expiration date of the lot of the current quant; the new date is set in ExpirationDate.")]
        public ACMethodBooking CurrentBookParamChangeExpirationDate
        {
            get
            {
                return _BookParamChangeExpirationDate;
            }
            protected set
            {
                _BookParamChangeExpirationDate = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region BSO->ACProperty->FacilityCharge
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<FacilityCharge> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(790, FacilityCharge.ClassName, Description = "Primary navigation access to the FacilityCharge (quant) records shown in the explorer list.")]
        public ACAccessNav<FacilityCharge> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter, true, true);
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<FacilityCharge>(FacilityCharge.ClassName, this);
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
                    new ACFilterItem(Global.FilterTypes.filter, _CNotAvailableProperty, Global.LogicalOperators.equal, Global.Operators.and, "false", true),

                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, _CMaterialNoProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CMaterialNameProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),

                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, _CFacilityNoProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CFacilityNameProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),

                    new ACFilterItem(Global.FilterTypes.filter, _CLotNoProperty, Global.LogicalOperators.contains, Global.Operators.and, "", true, true),
                    new ACFilterItem(Global.FilterTypes.filter, _CExternLotNoProperty, Global.LogicalOperators.contains, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CExternLotNo2Property, Global.LogicalOperators.contains, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, nameof(FacilityCharge.StockQuantityUOM), Global.LogicalOperators.lessThan, Global.Operators.and, "", true),

                    new ACFilterItem(Global.FilterTypes.filter, nameof(FacilityCharge.ExpirationDate), Global.LogicalOperators.lessThanOrEqual, Global.Operators.and, null, true)
                };
            }
        }

        protected virtual IQueryable<FacilityCharge> _AccessPrimary_NavSearchExecuting(IQueryable<FacilityCharge> result)
        {
            IQueryable<FacilityCharge> query = result as IQueryable<FacilityCharge>;
            if (query != null && !IsIncludeDisabled)
            {
                query =
                query
                .Include(c => c.Facility)
                .Include(c => c.FacilityLot)
                .Include(c => c.Material)
                .Include(c => c.MDUnit)
                .Include(c => c.MDReleaseState)
                .Include(c => c.CompanyMaterial)
                .Include(c => c.CPartnerCompanyMaterial)
                .Include("Material.MaterialUnit_Material.ToMDUnit")
                //.Include(c => c.FacilityBooking_InwardFacilityCharge)
                //.Include(c => c.FacilityBookingCharge_InwardFacilityCharge)
                ;
            }

            return query;
        }


        /// <summary>
        /// Gets or sets the selected facility charge.
        /// </summary>
        /// <value>The selected facility charge.</value>
        [ACPropertySelected(708, FacilityCharge.ClassName, "en{'Facilitycharge'}de{'Chargenplatz'}", Description = "The quant currently selected in the explorer list.")]
        public FacilityCharge SelectedFacilityCharge
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
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current facility charge.
        /// </summary>
        /// <value>The current facility charge.</value>
        [ACPropertyCurrent(709, FacilityCharge.ClassName, "en{'Facilitycharge'}de{'Chargenplatz'}", Description = "The currently edited quant. All field changes and bookings refer to this record; save with Save() before navigating or creating a new record.")]
        public FacilityCharge CurrentFacilityCharge
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
                if (AccessPrimary.CurrentNavObject != value)
                {

                    RefreshFilterFacilityLotAccess();

                    if (value != null && value.FacilityLot != null)
                    {
                        if (!AccessFacilityLot.NavList.Any(c => c.FacilityLotID == value.FacilityLotID))
                        {
                            AccessFacilityLot.NavList.Add(value.FacilityLot);
                            OnPropertyChanged(nameof(FacilityLotList));
                        }
                        if (AccessPrimary.Current != null)
                        {
                            AccessPrimary.Current.PropertyChanged -= CurrentFacilityCharge_PropertyChanged;
                        }
                    }

                    AccessPrimary.CurrentNavObject = value;

                    if (AccessPrimary.Current != null)
                    {
                        AccessPrimary.Current.PropertyChanged += CurrentFacilityCharge_PropertyChanged;
                    }

                    OnPropertyChanged_CurrentFacilityCharge();
                    OnPropertyChanged(nameof(CurrentFacilityCharge));
                    OnPropertyChanged(nameof(ContractualPartnerList));
                    OnPropertyChanged(nameof(StorageUnitTestList));
                    OnPropertyChanged(nameof(CurrentFacilityLot));
                    CorrectCurrentFacilityChargeFacilityList();
                    ClearBookingData();
                    OnPropertyChanged();
                }
            }
        }

        private void CorrectCurrentFacilityChargeFacilityList()
        {
            AccessQuantFacilityFilter.NavSearch();

            if (CurrentFacilityCharge != null && CurrentFacilityCharge.Facility != null && !AccessQuantFacilityFilter.NavList.Contains(CurrentFacilityCharge.Facility))
            {
                AccessQuantFacilityFilter.NavList.Add(CurrentFacilityCharge.Facility);

            }
            if (CurrentFacilityCharge != null)
            {
                SelectedQuantFacilityFilter = CurrentFacilityCharge.Facility;
            }
            else
            {
                SelectedQuantFacilityFilter = null;
            }
            OnPropertyChanged(nameof(QuantFacilityFilterList));
        }

        public virtual void CurrentFacilityCharge_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FacilityCharge.Material))
            {
                OnPropertyChanged(nameof(StorageUnitTestList));
                OnPropertyChanged(nameof(ContractualPartnerList));
                if (CurrentFacilityCharge.MDUnit == null)
                {
                    CurrentFacilityCharge.MDUnit = CurrentFacilityCharge.Material.BaseMDUnit;
                }
            }

            if (new string[] { nameof(FacilityCharge.MaterialID), nameof(FacilityCharge.FacilityID), nameof(FacilityCharge.FacilityLotID) }.Contains(e.PropertyName))
            {
                OnPropertyChanged(nameof(CurrentFacilityCharge));
            }
        }

        public virtual void OnPropertyChanged_CurrentFacilityCharge()
        {
        }

        private List<FacilityCharge> _FacilityChargeList;
        /// <summary>
        /// Gets or sets the facility charge list.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(710, FacilityCharge.ClassName, Description = "The result list of the last search; contains the quants shown in the explorer list.")]
        public List<FacilityCharge> FacilityChargeList
        {
            get
            {
                return _FacilityChargeList;
            }
            set
            {
                _FacilityChargeList = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<FacilityLot> _AccessFacilityLot;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(791, nameof(FacilityLot), Description = "Access to the FacilityLot records; provides the lot list of the current quant.")]
        public ACAccessNav<FacilityLot> AccessFacilityLot
        {
            get
            {
                if (_AccessFacilityLot == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + FacilityLot.ClassName, ACType.ACIdentifier);
                    _AccessFacilityLot = navACQueryDefinition.NewAccessNav<FacilityLot>(Const.QueryPrefix + FacilityLot.ClassName, this);
                    _AccessFacilityLot.AutoSaveOnNavigation = false;
                    _AccessFacilityLot.NavSearch();
                }
                return _AccessFacilityLot;
            }
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
            if (acAccess == _AccessFacilityLot)
            {
                _AccessFacilityLot.NavSearch();
                OnPropertyChanged(nameof(FacilityLotList));
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }


        /// <summary>
        /// Gets the facility lot list.
        /// </summary>
        /// <value>The facility lot list.</value>
        [ACPropertyList(200, nameof(FacilityLot), ConstApp.LotNo, Description = "List of lots available for the current quant.")]
        public IEnumerable<FacilityLot> FacilityLotList
        {
            get
            {
                if (AccessFacilityLot == null)
                    return null;
                return AccessFacilityLot.NavList;
            }
        }

        [ACPropertyCurrent(201, nameof(FacilityLot), ConstApp.LotNo, Description = "The lot currently assigned to the current quant. Disabled once the quant has had any stock.")]
        public FacilityLot CurrentFacilityLot
        {
            get
            {
                if (CurrentFacilityCharge == null)
                    return null;
                return CurrentFacilityCharge.FacilityLot;
            }
            set
            {
                if (CurrentFacilityCharge != null && CurrentFacilityCharge.FacilityLot != value)
                {
                    CurrentFacilityCharge.FacilityLot = value;
                    OnPropertyChanged();
                }
            }
        }

        public virtual void RefreshFilterFacilityLotAccess()
        {
            if (_AccessFacilityLot != null)
                _AccessFacilityLot.NavSearch();
            OnPropertyChanged(nameof(FacilityLotList));
        }

        private double _FBCTargetQuantityUOM;
        /// <summary>
        /// Value for another quantity for quant print
        /// for example from last or selected booking
        /// Default behavior is to use the current facility charge stock quantity
        /// Transfer property to current quant
        /// </summary>
        [ACPropertyInfo(999, nameof(FBCTargetQuantityUOM), ConstApp.TargetQuantityUOM, Description = "Alternative quantity for the quant printout (e.g. from the last or selected booking); by default the stock quantity of the current quant is used.")]
        public double FBCTargetQuantityUOM
        {
            get
            {
                return _FBCTargetQuantityUOM;
            }
            set
            {
                if (_FBCTargetQuantityUOM != value)
                {
                    _FBCTargetQuantityUOM = value;
                }
            }
        }

        private string _RefDocumentNo;
        /// <summary>
        /// if last booking is connected to one external document: picking, production
        /// this field will be populated and used in report
        /// </summary>
        [ACPropertyInfo(999, nameof(RefDocumentNo), "en{'Ref. Document No.'}de{'Ref. DokummentNr.'}", Description = "Number of an external document (e.g. picking or production order) connected to the last booking; used in reports.")]
        public string RefDocumentNo
        {
            get
            {
                return _RefDocumentNo;
            }
            set
            {
                if (_RefDocumentNo != value)
                {
                    _RefDocumentNo = value;
                }
            }
        }

        #endregion

        #region Units
        [ACPropertyList(792, "StorageUnit", Description = "List of units of the material of the current quant.")]
        public IEnumerable<MDUnit> StorageUnitTestList
        {
            get
            {
                if (CurrentFacilityCharge == null)
                    return null;
                if (CurrentFacilityCharge.Material == null)
                    return null;
                return CurrentFacilityCharge.Material.MaterialUnitList;
            }
        }

        [ACPropertyList(793, "", Description = "List of contractual partner companies of the material of the current quant.")]
        public IEnumerable<Company> ContractualPartnerList
        {
            get
            {
                if (CurrentFacilityCharge == null)
                    return null;
                if (CurrentFacilityCharge.Material == null)
                    return null;
                return CurrentFacilityCharge.Material.CompanyMaterial_Material.Select(c => c.Company);
            }
        }

        #endregion

        #region Message

        private MsgWithDetails _BSOMsg = new MsgWithDetails();
        [ACPropertyInfo(794, "Message", Description = "Messages and warnings of the last booking operation, shown in the Messages panel.")]
        public MsgWithDetails BSOMsg
        {
            get
            {
                return _BSOMsg;
            }
            set
            {
                _BSOMsg = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Material reassignment

        private Material _SelectedRassignmentMaterial;
        [ACPropertySelected(9999, "ReassignmentMaterial", "en{'Suggested material'}de{'Empfohlenes Material'}", Description = "The material selected for the material reassignment; assigning it also sets CurrentBookParamReassignMat.InwardMaterial.")]
        public Material SelectedRassignmentMaterial
        {
            get => _SelectedRassignmentMaterial;
            set
            {
                _SelectedRassignmentMaterial = value;
                CurrentBookParamReassignMat.InwardMaterial = _SelectedRassignmentMaterial;
                OnPropertyChanged();
            }
        }

        private IEnumerable<Material> _ReassignemntMaterials;
        [ACPropertyList(9999, "ReassignmentMaterial", Description = "List of suggested materials for the material reassignment of the current quant.")]
        public IEnumerable<Material> ReassignemntMaterials
        {
            get
            {
                if (_ReassignemntMaterials == null && CurrentFacilityCharge != null)
                {
                    _ReassignemntMaterials = ACFacilityManager.GetSuggestedReassignmentMaterials(DatabaseApp, CurrentFacilityCharge.Material);
                }
                return _ReassignemntMaterials;
            }
            set
            {
                _ReassignemntMaterials = value;
            }
        }

        #endregion

        #region AccessNav -> QuantFacilityFilter 

        public const string QuantFacilityFilter = "QuantFacilityFilter";

        ACAccess<Facility> _AccessQuantFacilityFilter;
        [ACPropertyAccess(9999, nameof(QuantFacilityFilter), Description = "Access to the Facility records used by the storage bin filter of the current quant.")]
        public ACAccess<Facility> AccessQuantFacilityFilter
        {
            get
            {
                if (_AccessQuantFacilityFilter == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + Facility.ClassName, ACType.ACIdentifier);

                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceSortColumnsIfDifferent(QuantFacilityFilterDefaultSort);
                        navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(QuantFacilityFilterDefaultFilter);

                        foreach (ACFilterItem aCFilterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            aCFilterItem.PropertyChanged += QuantFacilityFilterDefaultSort_PropertyChanged;
                        }
                    }

                    _AccessQuantFacilityFilter = navACQueryDefinition.NewAccessNav<Facility>(nameof(QuantFacilityFilter), this);
                    _AccessQuantFacilityFilter.NavSearchExecuting += _AccessQuantFacilityFilter_NavSearchExecuting;
                }
                return _AccessQuantFacilityFilter;
            }
        }

        private Facility FilterParentFacility;

        private IQueryable<Facility> _AccessQuantFacilityFilter_NavSearchExecuting(IQueryable<Facility> result)
        {
            if (FilterParentFacility != null)
            {
                result =
                    result
                    .Where(c =>
                            c.FacilityID == FilterParentFacility.FacilityID
                            || c.Facility1_ParentFacility.FacilityID == FilterParentFacility.FacilityID
                            || c.Facility1_ParentFacility.Facility1_ParentFacility.FacilityID == FilterParentFacility.FacilityID
                            || c.Facility1_ParentFacility.Facility1_ParentFacility.Facility1_ParentFacility.FacilityID == FilterParentFacility.FacilityID
                    );
            }
            return result;
        }

        public virtual List<ACFilterItem> QuantFacilityFilterDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityNo", Global.LogicalOperators.equal, Global.Operators.and, "", true, true),
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityName", Global.LogicalOperators.contains, Global.Operators.and, "", true, true),
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.or, ((short)FacilityTypesEnum.StorageBin).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.or, ((short)FacilityTypesEnum.StorageBinContainer).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.or, ((short)FacilityTypesEnum.PreparationBin).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                };
            }
        }

        public virtual List<ACSortItem> QuantFacilityFilterDefaultSort
        {
            get
            {
                List<ACSortItem> acSortItems = new List<ACSortItem>();

                ACSortItem acSortPickingNo = new ACSortItem("FacilityNo", SortDirections.ascending, true);
                acSortItems.Add(acSortPickingNo);

                return acSortItems;
            }
        }

        public virtual void QuantFacilityFilterDefaultSort_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ACFilterItem aCFilterItem = sender as ACFilterItem;

        }

        [ACPropertyInfo(9999, nameof(QuantFacilityFilter), Description = "List of storage bins offered by the storage bin filter; changing the selection updates the Facility of the current quant.")]
        public IEnumerable<Facility> QuantFacilityFilterList
        {
            get
            {
                return AccessQuantFacilityFilter.NavList;
            }
        }

        private Facility _SelectedQuantFacilityFilter;
        [ACPropertySelected(9999, nameof(QuantFacilityFilter), ConstApp.FacilityNo, Description = "The storage bin selected in the storage bin filter; sets the Facility of the current quant.")]
        public Facility SelectedQuantFacilityFilter
        {
            get
            {
                return _SelectedQuantFacilityFilter;
            }
            set
            {
                if (_SelectedQuantFacilityFilter != value)
                {
                    _SelectedQuantFacilityFilter = value;
                    if (
                        CurrentFacilityCharge != null
                        &&
                            (
                               (CurrentFacilityCharge.Facility == null && value != null)
                               || (CurrentFacilityCharge.Facility != null && CurrentFacilityCharge.Facility != value)
                            )
                        )
                    {
                        CurrentFacilityCharge.Facility = value;
                    }
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #endregion

        #region BSO->ACMethod
        #region Allgemein
        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(FacilityCharge.ClassName, Const.New, (short)MISort.New, true, "SelectedFecilityCharge", Global.ACKinds.MSMethodPrePost, Description =
                         @"Creates a new, empty quant (FacilityCharge), adds it to the database context and assigns it to the CurrentFacilityCharge property.
                           Assign the material, the storage bin and - for lot-managed materials - a lot before saving with Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentFacilityCharge = FacilityCharge.NewACObject(DatabaseApp, null);
            DatabaseApp.FacilityCharge.Add(CurrentFacilityCharge);
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
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Saves all changes of the current quant to the database.
                           A lot-managed quant without a lot cannot be saved (error 50552). After saving, the state returns to search mode.")]
        public async Task Save()
        {
            await OnSave();
        }

        protected override Msg OnPreSave()
        {
            Msg msg = base.OnPreSave();
            if (msg != null)
                return msg;

            if (CurrentFacilityCharge != null && CurrentFacilityCharge.Material != null && CurrentFacilityCharge.Material.IsLotManaged
                                              && CurrentFacilityCharge.FacilityLot == null)
            {
                if (CurrentFacilityCharge.EntityState != EntityState.Added)
                    return msg;

                //Error50552: Lot managed quants must have a lot assigned!!
                return new Msg(this, eMsgLevel.Error, nameof(BSOFacilityBookCharge), nameof(OnPreSave), 693, "Error50552");
            }

            return msg;
        }

        protected override void OnPostSave()
        {
            ACState = Const.SMSearch;
            PostExecute("Save");
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
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Discards all unsaved changes of the current quant and restores the last saved state.")]
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

        /// <summary>
        /// Loads this instance.
        /// </summary>
        [ACMethodInteraction(FacilityCharge.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacilityCharge", Global.ACKinds.MSMethodPrePost, Description =
                         @"Reloads the current quant with fresh data from the database, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            bool isNewDialog = CurrentFacilityCharge == null;
            if (requery)
                CurrentFacilityCharge?.ResetCachedValues();
            LoadEntity<FacilityCharge>(requery, () => SelectedFacilityCharge, () => CurrentFacilityCharge, c => CurrentFacilityCharge = c,
                        DatabaseApp.FacilityCharge
                        .Where(c => c.FacilityChargeID == SelectedFacilityCharge.FacilityChargeID));
            if (isNewDialog)
                CurrentFacilityCharge?.ResetCachedValues();
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedFacilityCharge != null;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(FacilityCharge.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentFacilityCharge", Global.ACKinds.MSMethodPrePost, Description =
                         @"Currently not implemented (no-op): quants are not deleted by this BSO. Use the booking functions (e.g. zero stock / not available) instead.")]
        public void Delete()
        {
            return;

            //if (!PreExecute("Delete")) return;
            //if (CurrentFacilityCharge == null)
            //    return;
            //if (AccessPrimary == null)
            //    return;

            //FacilityManager manager = ACFacilityManager as FacilityManager;
            //Global.ACMethodResultState result = manager.DeleteFacilityCharge(CurrentBookParamNotAvailable, CurrentFacilityCharge, false, this.DatabaseApp);
            //if (result <= Global.ACMethodResultState.Succeeded)
            //{
            //    //(manager as ACMethod).ValidMessage;
            //    // BSOMsg.AddDetailMessage((manager as ACMethod).ValidMessage);
            //}

            //ClearBookingData();
            //AccessPrimary.NavigateFirst();
            //PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return SelectedFacilityCharge != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description =
                         @"Executes the search with the current filter settings and fills the FacilityChargeList property with the result.")]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            DateTime startTime = DateTime.Now;
            _FacilityChargeList = null;
            if (AccessPrimary != null)
            {
                AccessPrimary.NavSearch(DatabaseApp, MergeOption.OverwriteChanges);
                _FacilityChargeList = AccessPrimary.NavList.ToList();

                // Lazy-Loading is sometimes faster:
                if (IsIncludeDisabled && _FacilityChargeList != null)
                {
                    foreach (var charge in _FacilityChargeList)
                    {
                        _ = charge.Facility;
                        _ = charge.FacilityLot;
                        _ = charge.Material.MaterialUnit_Material;
                        _ = charge.MDUnit;
                        _ = charge.MDReleaseState;
                        _ = charge.CompanyMaterial;
                        _ = charge.CPartnerCompanyMaterial;
                        //DatabaseApp.LoadProperty(charge, "Material.MaterialUnit_Material.ToMDUnit");
                    }
                }
            }
            DateTime endTime = DateTime.Now;
            Console.WriteLine("Duration: " + (endTime - startTime).TotalSeconds);
            OnPropertyChanged(nameof(FacilityChargeList));
        }

        private bool _SearchFacilityChargeListInProgress;
        [ACMethodInteraction("Deposit", "en{'Filter'}de{'Filtern'}", (short)MISort.Search, true, "FacilityChargeList", Global.ACKinds.MSMethod, Description =
                         @"Re-executes the search of the explorer list with the current filter settings and refreshes the FacilityChargeList property.")]
        public virtual void SearchFacilityChargeList()
        {
            _SearchFacilityChargeListInProgress = true;
            AccessNav.NavSearch(MergeOption.OverwriteChanges);
            OnPropertyChanged(nameof(FacilityChargeList));
            _SearchFacilityChargeListInProgress = false;
        }


        public bool IsEnabledSearchFacilityChargeList()
        {
            return !_SearchFacilityChargeListInProgress;
        }


        /// <summary>
        /// Clears the booking data.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'New Booking Record'}de{'Neuer Buchungssatz'}", 701, true, Description =
                         @"Resets all booking parameters (inward, outward, relocation, release/lock, availability, reassignment, split, expiration date) and pre-assigns the current quant to them.
                           Called automatically when the current quant changes and after each successful booking.")]
        public virtual void ClearBookingData()
        {
            if (_BookParamInwardMovementClone == null)
                _BookParamInwardMovementClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_InwardMovement_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            ACMethodBooking clone = _BookParamInwardMovementClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.MDUnit = CurrentFacilityCharge.MDUnit;
                clone.InwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamInwardMovement = clone;

            if (_BookParamOutwardMovementClone == null)
                _BookParamOutwardMovementClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_OutwardMovement_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamOutwardMovementClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.MDUnit = CurrentFacilityCharge.MDUnit;
                clone.OutwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamOutwardMovement = clone;

            if (_BookParamRelocationClone == null)
                _BookParamRelocationClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_Relocation_FacilityCharge_Facility.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamRelocationClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.MDUnit = CurrentFacilityCharge.MDUnit;
                clone.OutwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamRelocation = clone;

            if (_BookParamReleaseAndLockClone == null)
                _BookParamReleaseAndLockClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ReleaseState_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamReleaseAndLockClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.InwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamReleaseAndLock = clone;

            if (_BookParamNotAvailableClone == null)
                _BookParamNotAvailableClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamNotAvailableClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.InwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamNotAvailable = clone;

            if (_BookParamReassignMatClone == null)
                _BookParamReassignMatClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_Reassign_FacilityCharge, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamReassignMatClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.OutwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamReassignMat = clone;
            SelectedRassignmentMaterial = null;
            ReassignemntMaterials = null;

            if (_BookParamSplitClone == null)
                _BookParamSplitClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_Split_FacilityCharge, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamSplitClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.OutwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamSplit = clone;


            if (_BookParamReassignLotClone == null)
                _BookParamReassignLotClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_Reassign_FacilityChargeLot, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamReassignLotClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.OutwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamReassignLot = clone;

            if (_BookParamChangeExpirationDateClone == null)
                _BookParamChangeExpirationDateClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_Change_ExpirationDate, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamChangeExpirationDateClone.Clone() as ACMethodBooking;
            if (CurrentFacilityCharge != null)
            {
                clone.InwardFacilityLot = CurrentFacilityCharge.FacilityLot;
                clone.OutwardFacilityCharge = CurrentFacilityCharge;
            }
            CurrentBookParamChangeExpirationDate = clone;

        }

        /// <summary>
        /// Determines whether [is enabled clear booking data].
        /// </summary>
        /// <returns><c>true</c> if [is enabled clear booking data]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledClearBookingData()
        {
            return true;
        }
        #endregion

        #region ControlMode
        /// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        /// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        /// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        /// <returns>ControlModesInfo</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            switch (vbControl.VBContent)
            {
                case "CurrentFacilityCharge\\MDUnit":
                case "CurrentFacilityCharge\\Material":
                case "CurrentFacilityCharge\\Facility":
                case "CurrentFacilityCharge\\SplitNo":
                case nameof(SelectedQuantFacilityFilter):
                    if (CurrentFacilityCharge == null || CurrentFacilityCharge.EntityState != EntityState.Added)
                        return Global.ControlModes.Disabled;
                    break;
                case "CurrentFacilityCharge\\FacilityLot":
                    if (HasFCHadAnyStock)
                        return Global.ControlModes.Disabled;
                    break;
                case nameof(CurrentFacilityLot):
                    if (HasFCHadAnyStock)
                        return Global.ControlModes.Disabled;
                    break;

            }

            if (!String.IsNullOrEmpty(vbControl.VBContent) && vbControl.VBContent.StartsWith("CurrentBookParam"))
            {
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

        #region Buchung Zugang (InwardMovementUnscheduled)
        /// <summary>
        /// Inwards the facility charge movement.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Post Inward Movement'}de{'Buche Lagerzugang'}", 702, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Books a stock receipt (inward movement) on the current quant with the quantity from CurrentBookParamInwardMovement.InwardQuantity.
                           On success the booking data is cleared; messages and warnings are shown via BSOMsg.")]
        public void InwardFacilityChargeMovement()
        {
            if (!PreExecute(nameof(InwardFacilityChargeMovement)))
                return;

            CurrentBookParamInwardMovement.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamInwardMovement, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamInwardMovement.ValidMessage.IsSucceded() || CurrentBookParamInwardMovement.ValidMessage.HasWarnings())
                Messages.MsgAsync(CurrentBookParamInwardMovement.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.MsgAsync(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
            }

            PostExecute(nameof(InwardFacilityChargeMovement));
        }
        /// <summary>
        /// Determines whether [is enabled inward facility charge movement].
        /// </summary>
        /// <returns><c>true</c> if [is enabled inward facility charge movement]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledInwardFacilityChargeMovement()
        {
            if (CurrentBookParamInwardMovement == null)
                return false;
            bool bRetVal = CurrentBookParamInwardMovement.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }
        #endregion

        #region Buchung Abgang(OutwardMovementUnscheduled)
        /// <summary>
        /// Outwards the facility charge movement.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Post Outward Movement'}de{'Buche Lagerabgang'}", 703, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Books a stock issue (outward movement) from the current quant with the quantity from CurrentBookParamOutwardMovement.OutwardQuantity.
                           On success the booking data is cleared; messages and warnings are shown via BSOMsg.")]
        public void OutwardFacilityChargeMovement()
        {
            if (!PreExecute(nameof(OutwardFacilityChargeMovement)))
                return;

            CurrentBookParamOutwardMovement.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamOutwardMovement, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamOutwardMovement.ValidMessage.IsSucceded() || CurrentBookParamOutwardMovement.ValidMessage.HasWarnings())
                Messages.MsgAsync(CurrentBookParamOutwardMovement.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.MsgAsync(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
            }

            PostExecute(nameof(OutwardFacilityChargeMovement));
        }
        /// <summary>
        /// Determines whether [is enabled outward facility charge movement].
        /// </summary>
        /// <returns><c>true</c> if [is enabled outward facility charge movement]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledOutwardFacilityChargeMovement()
        {
            bool bRetVal = CurrentBookParamOutwardMovement.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }
        #endregion

        #region Umlagerung (Relocation)
        /// <summary>
        /// Facilities the charge relocation.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Post Stock Transfer'}de{'Buche Umlagerung'}", 704, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Books a stock transfer (relocation) of the current quant to the target storage bin CurrentBookParamRelocation.InwardFacility.
                           If the target bin is connected to a plant/transport system, the user is asked whether the transport should run in automatic mode; in automatic mode a picking order is created and a workflow started, otherwise the relocation is booked directly.")]
        public virtual async Task FacilityChargeRelocation()
        {
            if (!PreExecute("FacilityChargeRelocation")) return;

            if (IsPhysicalTransportPossible)
            {
                Global.MsgResult userQuestionAutomatic = Global.MsgResult.No;
                // Question50035: Do you want to run this relocation/transport on the plant in automatic mode?
                userQuestionAutomatic = await Messages.YesNoCancelAsync(this, "Question50035");
                if (userQuestionAutomatic == Global.MsgResult.Yes)
                {
                    WorkflowStartData workflowStartData = await PrepareStartWorkflow(CurrentBookParamInwardMovement);
                    bool wfRunsBatches = workflowStartData.WfRunsBatches;
                    gip.core.datamodel.ACClassMethod acClassMethod = workflowStartData.ACClassMethod;

                    if (workflowStartData.Result == false)
                    {
                        ClearBookingData();
                        return;
                    }

                    ACMethodBooking booking = CurrentBookParamRelocation;
                    // If Workflow doesn't contain a PWNodeProcessWorkflow => Start relocation directly
                    if (!wfRunsBatches)
                    {
                        if (booking.InwardQuantity.HasValue)
                            booking.InwardQuantity = 0.000001;
                        if (booking.OutwardQuantity.HasValue)
                            booking.OutwardQuantity = 0.000001;
                        BookRelocation();
                        if (booking.FacilityBooking != null)
                        {
                            StartWorkflow(acClassMethod, booking.FacilityBooking);
                        }
                    }
                    // Workflow contains a PWNodeProcessWorkflow => It runs batches => create Picking
                    else
                    {
                        if (ACPickingManager == null)
                        {
                            ClearBookingData();
                            return;
                        }
                        Picking picking = null;
                        MsgWithDetails msgDetails = ACPickingManager.CreateNewPicking(booking, acClassMethod, this.DatabaseApp, this.DatabaseApp.ContextIPlus, true, out picking);
                        if (msgDetails != null && msgDetails.MsgDetailsCount > 0)
                        {
                            await Messages.MsgAsync(msgDetails);
                            ClearBookingData();
                            ACUndoChanges();
                            return;
                        }
                        if (picking == null)
                        {
                            UndoSave();
                            ClearBookingData();
                            return;
                        }
                        await Save();


                        msgDetails = ACPickingManager.ValidateStart(this.DatabaseApp, this.DatabaseApp.ContextIPlus, picking, null, PARole.ValidationBehaviour.Strict, null, true);
                        if (!msgDetails.IsSucceded())
                        {
                            if (String.IsNullOrEmpty(msgDetails.Message))
                            {
                                // Der Auftrag kann nicht gestartet werden weil:
                                msgDetails.Message = Root.Environment.TranslateMessage(this, "Error50643");
                            }
                            await Messages.MsgAsync(msgDetails, Global.MsgResult.OK, eMsgButton.OK);
                            ClearBookingData();
                            return;
                        }
                        else if (msgDetails.HasWarnings())
                        {
                            if (String.IsNullOrEmpty(msgDetails.Message))
                            {
                                //Möchten Sie den Auftrag wirklich starten? Es gibt nämlich folgende Probleme:
                                msgDetails.Message = Root.Environment.TranslateMessage(this, "Question50108");
                            }
                            var userResult = await Messages.MsgAsync(msgDetails, Global.MsgResult.No, eMsgButton.YesNo);
                            if (userResult == Global.MsgResult.No || userResult == Global.MsgResult.Cancel)
                                return;
                        }

                        Global.MsgResult openPicking = Global.MsgResult.No;
                        bool checkPrefferedParams = IsCheckPreferredParamsActive && (picking?.ACClassMethod.HasRequiredParams ?? false);
                        if (checkPrefferedParams)
                        {
                            openPicking = Global.MsgResult.Yes;
                        }
                        else
                        {
                            if (OpenPickingBeforeStart)
                            {
                                // Question50106: Do you want to open the picking order before starting the workflow?
                                openPicking = await Messages.QuestionAsync(this, "Question50106");
                            }
                        }

                        bool startWorkflow = true;
                        if (openPicking == Global.MsgResult.Yes)
                        {
                            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
                            if (service != null)
                            {
                                PAOrderInfo info = new PAOrderInfo();
                                info.Entities.Add(
                                new PAOrderInfoEntry()
                                {
                                    EntityID = picking.PickingID,
                                    EntityName = Picking.ClassName
                                });

                                if (checkPrefferedParams)
                                {
                                    info.Entities.Add(
                                        new PAOrderInfoEntry()
                                        {
                                            EntityID = Guid.Empty,
                                            EntityName = nameof(BSOPreferredParameters)
                                        });
                                }

                                await service.ShowDialogOrder(this, info);
                                startWorkflow =
                                    info.DialogResult != null
                                    && info.DialogResult.SelectedCommand == eMsgButton.OK
                                    && picking.PickingState != PickingStateEnum.WFActive;
                            }
                        }
                        if (startWorkflow)
                        {
                            bool startWFSuccess = StartWorkflow(acClassMethod, picking);
                            if (startWFSuccess)
                            {
                                ClearBookingData();
                            }
                        }
                    }
                }
                else if (userQuestionAutomatic == Global.MsgResult.No)
                    BookRelocation();
            }
            else
                BookRelocation();

            PostExecute("FacilityChargeRelocation");
        }

        /// <summary>
        /// Determines whether [is enabled facility charge relocation].
        /// </summary>
        /// <returns><c>true</c> if [is enabled facility charge relocation]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledFacilityChargeRelocation()
        {
            CurrentBookParamRelocation.InwardQuantity = CurrentBookParamRelocation.OutwardQuantity;
            bool bRetVal = CurrentBookParamRelocation.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }

        protected override bool BookRelocation()
        {
            if (CurrentBookParamRelocation.FacilityBooking != null)
            {
                if (CurrentBookParamRelocation.FacilityBooking.OutwardMaterial != CurrentBookParamRelocation.OutwardMaterial)
                    CurrentBookParamRelocation.FacilityBooking.OutwardMaterial = CurrentBookParamRelocation.OutwardMaterial;

                if (CurrentBookParamRelocation.FacilityBooking.OutwardFacility != CurrentBookParamRelocation.OutwardFacility)
                    CurrentBookParamRelocation.FacilityBooking.OutwardFacility = CurrentBookParamRelocation.OutwardFacility;

                if (CurrentBookParamRelocation.FacilityBooking.OutwardFacilityLot != CurrentBookParamRelocation.OutwardFacilityLot)
                    CurrentBookParamRelocation.FacilityBooking.OutwardFacilityLot = CurrentBookParamRelocation.OutwardFacilityLot;

                if (CurrentBookParamRelocation.FacilityBooking.InwardMaterial != CurrentBookParamRelocation.InwardMaterial)
                    CurrentBookParamRelocation.FacilityBooking.InwardMaterial = CurrentBookParamRelocation.InwardMaterial;

                if (CurrentBookParamRelocation.FacilityBooking.InwardFacility != CurrentBookParamRelocation.InwardFacility)
                    CurrentBookParamRelocation.FacilityBooking.InwardFacility = CurrentBookParamRelocation.InwardFacility;

                if (CurrentBookParamRelocation.FacilityBooking.InwardFacilityLot != CurrentBookParamRelocation.InwardFacilityLot)
                    CurrentBookParamRelocation.FacilityBooking.InwardFacilityLot = CurrentBookParamRelocation.InwardFacilityLot;
            }

            CurrentBookParamRelocation.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamRelocation, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamRelocation.ValidMessage.IsSucceded() || CurrentBookParamRelocation.ValidMessage.HasWarnings())
            {
                Messages.MsgAsync(CurrentBookParamRelocation.ValidMessage);
                return false;
            }
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.MsgAsync(result.ValidMessage);
                return false;
            }
            else
            {
                ClearBookingData();
            }

            return true;
        }

        /// <summary>
        /// Source Property: ShowDlgInwardFacility
        /// </summary>
        [ACMethodInfo("ShowDlgToFacility", "en{'Choose facility'}de{'Lager auswählen'}", 999, Description =
                         @"Opens a dialog to select the target storage bin for the stock transfer and assigns it to CurrentBookParamRelocation.InwardFacility.")]
        public async Task ShowDlgFacilityRelocation()
        {
            if (!IsEnabledShowDlgFacilityRelocation())
                return;

            VBDialogResult dlgResult = await BSOFacilityExplorer_Child.Value.ShowDialog(CurrentBookParamRelocation.InwardFacility);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                Facility facility = dlgResult.ReturnValue as Facility;
                CurrentBookParamRelocation.InwardFacility = facility;
                OnPropertyChanged(nameof(CurrentBookParamRelocation));
            }
        }

        public bool IsEnabledShowDlgFacilityRelocation()
        {
            return CurrentBookParamRelocation != null;
        }

        #endregion

        #region Freigabestatus (ReleaseStateFacility, ReleaseStateMaterial)
        /// <summary>
        /// Locks the facility charge.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Lock'}de{'Sperren'}", 705, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Sets the release state of the current quant to Locked, so that it is no longer proposed for bookings.")]
        public void LockFacilityCharge()
        {
            if (!PreExecute(nameof(LockFacilityCharge)))
                return;

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Locked);

            CurrentBookParamReleaseAndLock.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReleaseAndLock, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReleaseAndLock.ValidMessage.IsSucceded() || CurrentBookParamReleaseAndLock.ValidMessage.HasWarnings())
                Messages.MsgAsync(CurrentBookParamReleaseAndLock.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.MsgAsync(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacilityCharge));
            }
            PostExecute(nameof(LockFacilityCharge));
        }
        /// <summary>
        /// Determines whether [is enabled lock facility charge].
        /// </summary>
        /// <returns><c>true</c> if [is enabled lock facility charge]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLockFacilityCharge()
        {
            if (CurrentFacilityCharge == null)
                return false;
            if (CurrentFacilityCharge.MDReleaseState != null)
            {
                if (CurrentFacilityCharge.MDReleaseState.MDReleaseStateIndex == (int)MDReleaseState.ReleaseStates.Locked)
                    return false;
            }

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Locked);
            bool bRetVal = CurrentBookParamReleaseAndLock.IsEnabled();
            CurrentBookParamReleaseAndLock.MDReleaseState = null;
            UpdateBSOMsg();
            return bRetVal;
        }

        /// <summary>
        /// Locks the facility charge absolute.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Lock absolute'}de{'Absolut sperren'}", 706, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Sets the release state of the current quant to absolutely locked; an absolute lock can only be removed by an absolute release.")]
        public void LockFacilityChargeAbsolute()
        {
            if (!PreExecute(nameof(LockFacilityChargeAbsolute)))
                return;


            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsLocked);
            CurrentBookParamReleaseAndLock.AutoRefresh = true;

            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReleaseAndLock, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReleaseAndLock.ValidMessage.IsSucceded() || CurrentBookParamReleaseAndLock.ValidMessage.HasWarnings())
                Messages.MsgAsync(CurrentBookParamReleaseAndLock.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.MsgAsync(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacilityCharge));
            }

            PostExecute(nameof(LockFacilityChargeAbsolute));
        }

        /// <summary>
        /// Determines whether [is enabled lock facility charge absolute].
        /// </summary>
        /// <returns><c>true</c> if [is enabled lock facility charge absolute]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLockFacilityChargeAbsolute()
        {
            if (CurrentFacilityCharge == null)
                return false;
            if (CurrentFacilityCharge.MDReleaseState != null)
            {
                if (CurrentFacilityCharge.MDReleaseState.MDReleaseStateIndex == (int)MDReleaseState.ReleaseStates.AbsLocked)
                    return false;
            }

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsLocked);
            bool isEnabled = CurrentBookParamReleaseAndLock.IsEnabled();
            CurrentBookParamReleaseAndLock.MDReleaseState = null;
            UpdateBSOMsg();
            return isEnabled;
        }

        /// <summary>
        /// Releases the facility charge.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Unlock'}de{'Freigeben'}", 707, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Sets the release state of the current quant to Free, so that it is available for bookings again.")]
        public void ReleaseFacilityCharge()
        {
            if (!PreExecute(nameof(ReleaseFacilityCharge)))
                return;


            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Free);
            CurrentBookParamReleaseAndLock.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReleaseAndLock, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReleaseAndLock.ValidMessage.IsSucceded() || CurrentBookParamReleaseAndLock.ValidMessage.HasWarnings())
                Messages.MsgAsync(CurrentBookParamReleaseAndLock.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.MsgAsync(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacilityCharge));
            }

            PostExecute(nameof(ReleaseFacilityCharge));
        }

        /// <summary>
        /// Determines whether [is enabled release facility charge].
        /// </summary>
        /// <returns><c>true</c> if [is enabled release facility charge]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledReleaseFacilityCharge()
        {
            if (CurrentFacilityCharge == null)
                return false;
            if (CurrentFacilityCharge.MDReleaseState != null)
            {
                if (CurrentFacilityCharge.MDReleaseState.MDReleaseStateIndex == (int)MDReleaseState.ReleaseStates.Free)
                    return false;
            }

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Free);
            bool isEnabled = CurrentBookParamReleaseAndLock.IsEnabled();
            CurrentBookParamReleaseAndLock.MDReleaseState = null;
            UpdateBSOMsg();
            return isEnabled;
        }

        /// <summary>
        /// Releases the facility charge absolute.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Unlock absolute'}de{'Absolut freigeben'}", 708, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Sets the release state of the current quant to absolutely free, removing an absolute lock.")]
        public void ReleaseFacilityChargeAbsolute()
        {
            if (!PreExecute(nameof(ReleaseFacilityChargeAbsolute)))
                return;

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsFree);
            CurrentBookParamReleaseAndLock.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReleaseAndLock, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReleaseAndLock.ValidMessage.IsSucceded() || CurrentBookParamReleaseAndLock.ValidMessage.HasWarnings())
                Messages.MsgAsync(CurrentBookParamReleaseAndLock.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.MsgAsync(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacilityCharge));
            }

            PostExecute(nameof(ReleaseFacilityChargeAbsolute));
        }
        /// <summary>
        /// Determines whether [is enabled release facility charge absolute].
        /// </summary>
        /// <returns><c>true</c> if [is enabled release facility charge absolute]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledReleaseFacilityChargeAbsolute()
        {
            if (CurrentFacilityCharge == null)
                return false;
            if (CurrentFacilityCharge.MDReleaseState != null)
            {
                if (CurrentFacilityCharge.MDReleaseState.MDReleaseStateIndex == (int)MDReleaseState.ReleaseStates.AbsFree)
                    return false;
            }

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsFree);
            bool isEnabled = CurrentBookParamReleaseAndLock.IsEnabled();
            CurrentBookParamReleaseAndLock.MDReleaseState = null;
            UpdateBSOMsg();
            return isEnabled;
        }
        #endregion

        #region Nicht verfügbar (NotAvailableCharge, AvailableCharge, NotAvailableMaterial, AvailableMaterial)
        /// <summary>
        /// Nots the available facility charge.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Quant not available'}de{'Quant nicht verfügbar'}", 709, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Marks the current quant as not available (zero stock state SetNotAvailable), e.g. for an inventory difference.")]
        public void NotAvailableFacilityCharge()
        {
            if (!PreExecute(nameof(NotAvailableFacilityCharge)))
                return;
            BookNotAvailableFacilityCharge(false);
            PostExecute(nameof(NotAvailableFacilityCharge));
        }

        public bool BookNotAvailableFacilityCharge(bool withRefresh)
        {
            CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
            CurrentBookParamNotAvailable.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamNotAvailable, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamNotAvailable.ValidMessage.IsSucceded() || CurrentBookParamNotAvailable.ValidMessage.HasWarnings())
                Messages.MsgAsync(CurrentBookParamNotAvailable.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.MsgAsync(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                if (withRefresh)
                {
                    AccessNav.NavSearch();
                    OnPropertyChanged(nameof(FacilityChargeList));
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// Determines whether [is enabled not available facility charge].
        /// </summary>
        /// <returns><c>true</c> if [is enabled not available facility charge]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNotAvailableFacilityCharge()
        {
            if (CurrentFacilityCharge == null)
                return false;
            bool isEnabled = !CurrentFacilityCharge.NotAvailable;
            if (isEnabled)
            {
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
                isEnabled = CurrentBookParamNotAvailable.IsEnabled();
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.Off);
                UpdateBSOMsg();
            }
            return isEnabled;
        }

        [ACMethodCommand(FacilityCharge.ClassName, "en{'All Quants not available'}de{'Alle Quanten nicht verfügbar'}", 711, true, Global.ACKinds.MSMethod, Description =
                         @"Marks all quants in the explorer list as not available after a confirmation question and refreshes the list.")]
        public async void NotAvailableFacilityChargeAll()
        {
            //Question50087: Are you sure that you want to change the zero stock status for all the quants listed in the Explorer?
            Global.MsgResult qResult = await Messages.QuestionAsync(this, "Question50087", Global.MsgResult.No);
            if (qResult != Global.MsgResult.Yes)
                return;
            if (FacilityChargeList == null)
                return;

            FacilityCharge[] facilityCharges = FacilityChargeList.ToArray();

            ACMethodBooking[] bookingMethods = GetMethodBookingsForNotAvailable(facilityCharges);
            Dictionary<ACMethodBooking, ACMethodEventArgs> results = ACFacilityManager.BookFacilities(bookingMethods, this.DatabaseApp);

            MsgWithDetails msgWithDetails = GetBookingMessages(results);

            if (msgWithDetails.MessageLevel >= eMsgLevel.Warning)
            {
                await Messages.MsgAsync(msgWithDetails);
            }

            Search();
        }

        public bool IsEnabledNotAvailableFacilityChargeAll()
        {
            return CurrentFacilityCharge != null && FacilityChargeList != null && FacilityChargeList.Any();
        }

        /// <summary>
        /// Availables the facility charge.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'Quant available'}de{'Quant verfügbar'}", 710, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Makes the current quant available again (zero stock state ResetIfNotAvailable); if a previous stock quantity exists, the user is asked whether it should be restored.")]
        public async void AvailableFacilityCharge()
        {
            if (!PreExecute("AvailableFacilityCharge")) return;
            await BookAvailableFacilityCharge(false);
            PostExecute("AvailableFacilityCharge");
        }

        public async Task<bool> BookAvailableFacilityCharge(bool withRefresh)
        {
            FacilityBookingCharge fbc = CurrentFacilityCharge?.FacilityBookingCharge_InwardFacilityCharge
                                                              .Where(c => c.FacilityBookingTypeIndex == (short)GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge
                                                                       && Math.Abs(c.InwardQuantity) >= double.Epsilon)
                                                              .OrderByDescending(c => c.InsertDate)
                                                              .FirstOrDefault();

            //Question50098: Do you want to restore a last stock?
            if (fbc != null && await Messages.QuestionAsync(this, "Question50098") == Global.MsgResult.Yes)
            {
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.RestoreQuantityIfNotAvailable);
            }
            else
            {
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.ResetIfNotAvailable);
            }

            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamNotAvailable, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamNotAvailable.ValidMessage.IsSucceded() || CurrentBookParamNotAvailable.ValidMessage.HasWarnings())
                await Messages.MsgAsync(CurrentBookParamNotAvailable.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                await Messages.MsgAsync(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                if (withRefresh)
                {
                    AccessNav.NavSearch();
                    OnPropertyChanged(nameof(FacilityChargeList));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is enabled available facility charge].
        /// </summary>
        /// <returns><c>true</c> if [is enabled available facility charge]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAvailableFacilityCharge()
        {
            if (CurrentFacilityCharge == null)
                return false;
            bool isEnabled = CurrentFacilityCharge.NotAvailable;
            if (isEnabled)
            {
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.ResetIfNotAvailable);
                isEnabled = CurrentBookParamNotAvailable.IsEnabled();
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.Off);
                UpdateBSOMsg();
            }
            return isEnabled;
        }


        [ACMethodCommand(FacilityCharge.ClassName, "en{'All Quants available'}de{'Alle Quanten verfügbar'}", 712, true, Global.ACKinds.MSMethod, Description =
                         @"Makes all quants in the explorer list available again after a confirmation question (optionally restoring the last stock) and refreshes the list.")]
        public async void AvailableFacilityChargeAll()
        {
            //Question50087: Are you sure that you want to change the zero stock status for all the quants listed in the Explorer?
            Global.MsgResult qResult = await Messages.QuestionAsync(this, "Question50087", Global.MsgResult.No);
            if (qResult != Global.MsgResult.Yes)
                return;
            if (FacilityChargeList == null)
                return;

            //Question50098: Do you want to restore a last stock?
            bool restoreLastStock = await Messages.QuestionAsync(this, "Question50098") == Global.MsgResult.Yes;
            FacilityCharge[] facilityCharges = FacilityChargeList.ToArray();

            ACMethodBooking[] bookingMethods = GetMethodBookingsForAvailable(facilityCharges, restoreLastStock);
            Dictionary<ACMethodBooking, ACMethodEventArgs> results = ACFacilityManager.BookFacilities(bookingMethods, this.DatabaseApp);

            MsgWithDetails msgWithDetails = GetBookingMessages(results);

            if (msgWithDetails.MessageLevel >= eMsgLevel.Warning)
            {
                await Messages.MsgAsync(msgWithDetails);
            }

            Search();
        }

        public bool IsEnabledAvailableFacilityChargeAll()
        {
            return CurrentFacilityCharge != null && FacilityChargeList != null && FacilityChargeList.Any();
        }

        #endregion

        #region Materialneuzuordnung (Reassignment)
        /// <summary>
        /// Facilities the relocation.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Reassign Material'}de{'Material neu zuordnen'}", 811, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Reassigns the material of the current quant to the material from CurrentBookParamReassignMat.InwardMaterial (selectable via the SelectedRassignmentMaterial property).")]
        public virtual void FacilityReassign()
        {
            if (!PreExecute(nameof(FacilityReassign)))
                return;

            CurrentBookParamReassignMat.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReassignMat, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReassignMat.ValidMessage.IsSucceded() || CurrentBookParamReassignMat.ValidMessage.HasWarnings())
                Messages.MsgAsync(CurrentBookParamReassignMat.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.MsgAsync(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
            }

            PostExecute(nameof(FacilityReassign));
        }

        /// <summary>
        /// Determines whether [is enabled facility relocation].
        /// </summary>
        /// <returns><c>true</c> if [is enabled facility relocation]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledFacilityReassign()
        {
            bool bRetVal = CurrentBookParamReassignMat.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }
        #endregion

        #region Lostneuzuordnung (Reassignment Lot)
        /// <summary>
        /// Facilities the relocation.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Reassign Lot'}de{'Los neu zuordnen'}", 812, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Reassigns the lot of the current quant to the lot from CurrentBookParamReassignLot.InwardFacilityLot.")]
        public virtual void FacilityLotReassign()
        {
            if (!PreExecute(nameof(FacilityLotReassign)))
                return;

            CurrentBookParamReassignLot.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReassignLot, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReassignLot.ValidMessage.IsSucceded() || CurrentBookParamReassignLot.ValidMessage.HasWarnings())
                Messages.MsgAsync(CurrentBookParamReassignLot.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.MsgAsync(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
            }

            PostExecute(nameof(FacilityLotReassign));
        }

        /// <summary>
        /// Determines whether [is enabled facility relocation].
        /// </summary>
        /// <returns><c>true</c> if [is enabled facility relocation]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledFacilityLotReassign()
        {
            bool bRetVal = CurrentBookParamReassignLot.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }
        #endregion

        #region Ablaufdatum ändern (Change Expiration Date)
        /// <summary>
        /// Facilities the relocation.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Change Expiration Date'}de{'Ablaufdatum ändern'}", 813, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Changes the expiration date of the lot of the current quant to the date from CurrentBookParamChangeExpirationDate.ExpirationDate.")]
        public virtual async void FacilityLotChangeExpirationDate()
        {
            if (!PreExecute(nameof(FacilityLotChangeExpirationDate)))
                return;

            CurrentBookParamChangeExpirationDate.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamChangeExpirationDate, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamChangeExpirationDate.ValidMessage.IsSucceded() || CurrentBookParamChangeExpirationDate.ValidMessage.HasWarnings())
                await Messages.MsgAsync(CurrentBookParamChangeExpirationDate.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                await Messages.MsgAsync(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
            }
        }

        /// <summary>
        /// Determines whether [is enabled facility relocation].
        /// </summary>
        /// <returns><c>true</c> if [is enabled facility relocation]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledFacilityLotChangeExpirationDate()
        {
            bool bRetVal = CurrentBookParamChangeExpirationDate.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }
        #endregion

        #region SplitQuant
        /// <summary>
        /// Facilities the relocation.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Split Quant'}de{'Quant Splitten'}", 712, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Splits the current quant: the quantity from CurrentBookParamSplit.OutwardQuantity is moved to a new quant with the split number from CurrentBookParamSplit.InwardSplitNo.")]
        public virtual void SplitQuant()
        {
            if (!PreExecute(nameof(SplitQuant)))
                return;


            CurrentBookParamSplit.AutoRefresh = true;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamSplit, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamSplit.ValidMessage.IsSucceded() || CurrentBookParamSplit.ValidMessage.HasWarnings())
                Messages.MsgAsync(CurrentBookParamSplit.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.MsgAsync(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
            }

            PostExecute(nameof(SplitQuant));
        }

        /// <summary>
        /// Determines whether [is enabled facility relocation].
        /// </summary>
        /// <returns><c>true</c> if [is enabled facility relocation]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSplitQuant()
        {
            bool bRetVal = CurrentBookParamSplit.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }
        #endregion

        #region Lot-Handling
        /// <summary>
        /// Updates the BSO MSG.
        /// </summary>
        protected void UpdateBSOMsg()
        {
            // TODO: Bei BSOFacilityBookCharge die Methode UpdateBSOMsg implementieren
            if (CurrentBookParam == null)
                return;
            if (!CurrentBookParam.ValidMessage.IsSucceded() || CurrentBookParam.ValidMessage.HasWarnings())
            {
                if (!BSOMsg.IsEqual(CurrentBookParam.ValidMessage))
                {
                    BSOMsg.UpdateFrom(CurrentBookParam.ValidMessage);
                }
            }
            else
            {
                if (BSOMsg.MsgDetailsCount > 0)
                    BSOMsg.ClearMsgDetails();
            }
        }

        /// <summary>
        /// News the charge no.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'New Charge No.'}de{'Neue Chargennr.'}", 712, true, Description =
                         @"Reserved for generating a new charge number; currently not implemented (empty stub).")]
        public void NewChargeNo()
        {
            // TODO:
        }

        /// <summary>
        /// Determines whether [is enabled new charge no].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new charge no]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewChargeNo()
        {
            return true;
        }

        /// <summary>
        /// News the split charge no.
        /// </summary>
        [ACMethodCommand(FacilityCharge.ClassName, "en{'New Split No.'}de{'Neue Splitnr.'}", 740, true, Description =
                         @"Reserved for generating a new split number for the current quant; currently not implemented (body commented out).")]
        public void NewSplitChargeNo()
        {
            //CurrentBookParamReleaseAndLock.InwardFacilityCharge.FacilityLot.LotNo = FacilityManager.GenerateSplitChargeNo(CurrentBookParamReleaseAndLock.InwardFacilityCharge);
        }

        /// <summary>
        /// Determines whether [is enabled new split charge no].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new split charge no]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewSplitChargeNo()
        {
            return CurrentBookParamReleaseAndLock.InwardFacilityCharge != null;
        }


        [ACMethodCommand(FacilityCharge.ClassName, "en{'New Lot'}de{'Neues Los'}", 714, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Opens the dialog of the child BSO BSOFacilityLot to create a new lot for the material of the current quant and assigns it to the quant. Only available as long as the quant has never had any stock.")]
        public async Task FacilityChargeLotGenerateDlg()
        {
            if (!IsEnabledFacilityChargeLotGenerateDlg())
                return;
            if (CurrentFacilityCharge.EntityState != EntityState.Added && !ACSaveChanges())
                return;
            ACComponent childBSO = ACUrlCommand("?" + ConstApp.BSOFacilityLot_ChildName) as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent(ConstApp.BSOFacilityLot_ChildName, null, new object[] { }) as ACComponent;
            if (childBSO == null) return;

            var dlgResultAsync = childBSO.ACUrlCommand("!" + ConstApp.BSOFacilityLot_Dialog_ShowDialogNewLot, "", CurrentFacilityCharge.Material) as Task<VBDialogResult>;
            VBDialogResult dlgResult = await dlgResultAsync;
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                FacilityLot lot = dlgResult.ReturnValue as FacilityLot;
                _AccessFacilityLot.NavList.Add(lot);
                OnPropertyChanged(nameof(FacilityLotList));
                CurrentFacilityLot = lot;
            }
            if (childBSO != null)
                await childBSO.Stop();
        }

        public bool IsEnabledFacilityChargeLotGenerateDlg()
        {
            return CurrentFacilityCharge != null && CurrentFacilityCharge.Material != null && !HasFCHadAnyStock;
        }


        public bool HasFCHadAnyStock
        {
            get
            {
                return CurrentFacilityCharge == null || Math.Abs(CurrentFacilityCharge.StockQuantityUOM) >= double.Epsilon || CurrentFacilityCharge.NotAvailable;
            }
        }
        #endregion

        #region Automatic/Workflow
        public override bool IsPhysicalTransportPossible
        {
            get
            {
                if (CurrentBookParamRelocation == null)
                    return false;
                return CurrentBookParamRelocation.OutwardFacilityCharge != null
                        && CurrentBookParamRelocation.OutwardFacilityCharge.Facility != null
                        && CurrentBookParamRelocation.InwardFacility != null
                        && CurrentBookParamRelocation.InwardFacility.VBiFacilityACClassID.HasValue
                        && HasRightsForPhysicalTransport;
            }
        }
        #endregion

        #region Dialog Navigate

        [ACMethodInteraction("", ConstApp.ShowProdOrder, 781, true, nameof(SelectedFacilityCharge), Description =
                         @"Opens the production order dialog for the order position from which the selected quant originates.")]
        public async Task NavigateToOrder()
        {
            if (!IsEnabledNavigateToOrder())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(ProdOrderPartslistPos), SelectedFacilityCharge.FinalPositionFromFbc.ProdOrderPartslistPosID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToOrder()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.FinalPositionFromFbc != null)
                return true;
            return false;
        }


        [ACMethodInteraction(nameof(NavigateToFacilityChargeHistory), "en{'Show stock history of quant'}de{'Bestandshistorie des Quants anzeigen'}", 782, true, nameof(SelectedFacilityCharge), Description =
                         @"Opens the stock history dialog for the selected quant.")]
        public async Task  NavigateToFacilityChargeHistory()
        {
            if (!IsEnabledNavigateToFacilityChargeHistory())
            {
                return;
            }

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityCharge), SelectedFacilityCharge.FacilityChargeID));
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacilityCharge.FacilityID));
                info.Entities.Add(new PAOrderInfoEntry(nameof(Material), SelectedFacilityCharge.MaterialID));

                if (SelectedFacilityCharge.FacilityLotID != null)
                {
                    info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityCharge.FacilityLotID ?? Guid.Empty));
                }

                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityChargeHistory()
        {
            return SelectedFacilityCharge != null;
        }


        [ACMethodInteraction("", "en{'Show Material Stock and History'}de{'Zeige Materialbestand und Historie'}", 783, true, nameof(SelectedFacilityCharge), Description =
                         @"Opens the material stock overview with the stock and history of the material of the selected quant.")]
        public async Task NavigateToMaterialOverview()
        {
            if (!IsEnabledNavigateToMaterialOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(Material), SelectedFacilityCharge.MaterialID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToMaterialOverview()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Material != null)
                return true;
            return false;
        }


        [ACMethodInteraction("", "en{'Manage Material'}de{'Verwalte Material'}", 784, true, nameof(SelectedFacilityCharge), Description =
                         @"Opens the material master data app for the material of the selected quant.")]
        public async Task NavigateToMaterial()
        {
            if (!IsEnabledNavigateToMaterial())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(Material), SelectedFacilityCharge.MaterialID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToMaterial()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Material != null)
                return true;
            return false;
        }


        [ACMethodInteraction("", "en{'Show Lot Stock and History'}de{'Zeige Losbestand und Historie'}", 785, true, nameof(SelectedFacilityCharge), Description =
                         @"Opens the lot stock overview with the stock and history of the lot of the selected quant.")]
        public async Task NavigateToFacilityLotOverview()
        {
            if (!IsEnabledNavigateToFacilityLotOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityCharge.FacilityLotID ?? Guid.Empty));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityLotOverview()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.FacilityLot != null)
                return true;
            return false;
        }


        [ACMethodInteraction("", "en{'Manage Lot/Batch'}de{'Verwalte Los/Charge'}", 786, true, nameof(SelectedFacilityCharge), Description =
                         @"Opens the lot master data app for the lot of the selected quant.")]
        public async Task NavigateToFacilityLot()
        {
            if (!IsEnabledNavigateToFacilityLot())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityLot), SelectedFacilityCharge.FacilityLotID ?? Guid.Empty));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityLot()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.FacilityLot != null)
                return true;
            return false;
        }


        [ACMethodInteraction("", "en{'Show Bin Stock and History'}de{'Zeige Behälterbestand und Historie'}", 787, true, nameof(SelectedFacilityCharge), Description =
                         @"Opens the storage bin stock overview for the bin/container of the selected quant.")]
        public async Task NavigateToFacilityOverview()
        {
            if (!IsEnabledNavigateToFacilityOverview())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 1 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacilityCharge.FacilityID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacilityOverview()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Facility != null && SelectedFacilityCharge.Facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                return true;
            return false;
        }


        [ACMethodInteraction("", "en{'Manage Stock of Bin'}de{'Verwalte Behälterbestand'}", 788, true, nameof(SelectedFacilityCharge), Description =
                         @"Opens the stock management app for the bin/container of the selected quant.")]
        public async Task NavigateToFacility()
        {
            if (!IsEnabledNavigateToFacility())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(Facility), SelectedFacilityCharge.FacilityID));
                await service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToFacility()
        {
            if (SelectedFacilityCharge != null && SelectedFacilityCharge.Facility != null && SelectedFacilityCharge.Facility.MDFacilityType != null && SelectedFacilityCharge.Facility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                return true;
            return false;
        }

        #endregion

        #region BSO->ACMethod Filter ExpirationDate

        /// <summary>
        /// Source Property: SetFilterExpirationDateToday
        /// </summary>
        [ACMethodInfo("SetFilterExpirationDate", "en{'Today'}de{'Heute'}", 999, Description =
                         @"Sets the expiration date filter to today's date, so only quants expiring today or earlier are shown.")]
        public void SetFilterExpirationDateToday()
        {
            if (!IsEnabledSetFilterExpirationDateToday())
                return;
            FilterExpirationDate = DateTime.Now.Date;
        }

        public bool IsEnabledSetFilterExpirationDateToday()
        {
            return true;
        }

        /// <summary>
        /// Source Property: SetFilterExpirationDateNextDays
        /// </summary>
        [ACMethodInfo("SetFilterExpirationDate", "en{'Next [0] days'}de{'Nächste [0] Tage'}", 999, Description =
                         @"Sets the expiration date filter to the date in the configured number of days (ExpirationDateDayPeriod), so only quants expiring within this period are shown.")]
        public void SetFilterExpirationDateNextDays()
        {
            if (!IsEnabledSetFilterExpirationDateNextDays())
                return;
            FilterExpirationDate = DateTime.Now.Date.AddDays(ExpirationDateDayPeriod);
        }

        public bool IsEnabledSetFilterExpirationDateNextDays()
        {
            return true;
        }

        /// <summary>
        /// Source Property: ResetFilterExpirationDate
        /// </summary>
        [ACMethodInfo("SetFilterExpirationDate", "en{'Reset'}de{'Zurücksetzen'}", 999, Description =
                         @"Resets the expiration date filter, so quants are no longer filtered by expiration date.")]
        public void ResetFilterExpirationDate()
        {
            if (!IsEnabledResetFilterExpirationDate())
                return;
            FilterExpirationDate = null;
        }

        public bool IsEnabledResetFilterExpirationDate()
        {
            return true;
        }

        #endregion

        #region Method -> ShowDialogOrderInfo

        private FacilityCharge dialogAddedFacilityCharge;

        [ACMethodInfo("Dialog", "en{'Dialog lot overview'}de{'Dialog Losübersicht'}", (short)MISort.QueryPrintDlg + 1, Description =
                         @"Opens this BSO as a dialog (lot/quant overview) for other applications: filters and preselects the requested quant, or creates a new quant (DialogSelectInfo = 2) and returns it to the caller.")]
        public virtual async Task ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;
            if (paOrderInfo.DialogSelectInfo == 0)
            {
                ShowDialogOrderInfoExistingCharge(paOrderInfo);
            }
            else if (paOrderInfo.DialogSelectInfo == 2)
            {
                ShowDialogOrderInfoNewCharge(paOrderInfo);
            }

            await ShowDialogAsync(this, "ShowDialogOrderInfoDlg");
            await this.ParentACComponent.StopComponent(this);

            if (
                    paOrderInfo.DialogSelectInfo == 2
                    && dialogAddedFacilityCharge != null
                    && dialogAddedFacilityCharge.EntityState != EntityState.Detached
                    && dialogAddedFacilityCharge.EntityState != EntityState.Deleted
               )
            {
                paOrderInfo.DialogResult = new VBDialogResult();
                paOrderInfo.DialogResult.ReturnValue = dialogAddedFacilityCharge;
            }

            FilterParentFacility = null;
            dialogAddedFacilityCharge = null;
        }

        #region Method -> ShowDialogOrderInfo -> Existing FacilityCharge

        public virtual void ShowDialogOrderInfoExistingCharge(PAOrderInfo paOrderInfo)
        {
            DialogOrderInfoPrepareFilter(paOrderInfo);
            Search();
            DialogOrderInfoPreSelectCharge(paOrderInfo);
        }

        public virtual void DialogOrderInfoPrepareFilter(PAOrderInfo paOrderInfo)
        {
            PAOrderInfoEntry materialEntry = paOrderInfo.Entities.Where(c => c.EntityName == nameof(Material)).FirstOrDefault();
            PAOrderInfoEntry facilityLotEntry = paOrderInfo.Entities.Where(c => c.EntityName == nameof(FacilityLot)).FirstOrDefault();

            if (materialEntry != null)
            {
                Material material = DatabaseApp.Material.Where(c => c.MaterialID == materialEntry.EntityID).FirstOrDefault();
                AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNoProperty, material.MaterialNo);
            }

            if (facilityLotEntry != null)
            {
                FacilityLot facilityLot = DatabaseApp.FacilityLot.Where(c => c.FacilityLotID == facilityLotEntry.EntityID).FirstOrDefault();
                AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(_CLotNoProperty, facilityLot.LotNo);
            }
        }

        public virtual void DialogOrderInfoPreSelectCharge(PAOrderInfo paOrderInfo)
        {
            PAOrderInfoEntry facilityChargeEntry = paOrderInfo.Entities.Where(c => c.EntityName == nameof(FacilityCharge)).FirstOrDefault();

            if (facilityChargeEntry != null)
            {
                FacilityCharge facilityCharge = DatabaseApp.FacilityCharge.Where(c => c.FacilityChargeID == facilityChargeEntry.EntityID).FirstOrDefault();
                if (FacilityChargeList == null)
                {
                    FacilityChargeList = new List<FacilityCharge>();
                }

                if (!FacilityChargeList.Contains(facilityCharge))
                {
                    FacilityChargeList.Add(facilityCharge);
                }

                SelectedFacilityCharge = facilityCharge;
                CurrentFacilityCharge = facilityCharge;

            }
        }

        #endregion

        #region Method -> ShowDialogOrderInfo -> New FacilityCharge

        public virtual void ShowDialogOrderInfoNewCharge(PAOrderInfo paOrderInfo)
        {
            PAOrderInfoEntry facilityEntry = paOrderInfo.Entities.Where(c => c.EntityName == nameof(Facility)).FirstOrDefault();
            FilterParentFacility = DatabaseApp.Facility.Where(c => c.FacilityID == facilityEntry.EntityID).FirstOrDefault();
            New();
        }

        #endregion

        /// <summary>
        /// Source Property: CloseDialogOrderInfoDlg
        /// </summary>
        [ACMethodInfo(nameof(CloseDialogOrderInfoDlg), Const.Close, 999, Description =
                         @"Closes the lot overview dialog and returns the current quant to the caller; only enabled if the current quant has a valid material, storage bin, unit and - for lot-managed materials - a lot.")]
        public void CloseDialogOrderInfoDlg()
        {
            if (!IsEnabledCloseDialogOrderInfoDlg())
                return;
            dialogAddedFacilityCharge = CurrentFacilityCharge;
            CloseTopDialog();
        }

        public bool IsEnabledCloseDialogOrderInfoDlg()
        {
            bool isCurrentChargeValid =
                CurrentFacilityCharge != null
                && CurrentFacilityCharge.Material != null
                && CurrentFacilityCharge.Facility != null
                && CurrentFacilityCharge.MDUnit != null;

            bool isLotValid =
                isCurrentChargeValid
                && (!CurrentFacilityCharge.Material.IsLotManaged || CurrentFacilityCharge.FacilityLot != null);

            return isCurrentChargeValid && isLotValid;
        }


        #endregion

        #region Eventhandling
        public override void OnPropertyChanged([CallerMemberName] string name = "")
        {
            base.OnPropertyChanged(name);
            if (name == nameof(ShowNotAvailable)
                || name == nameof(FilterMaterial)
                || name == nameof(FilterFacility)
                || name == nameof(FilterLot)
                || name == nameof(FilterExternLot)
                || name == nameof(FilterExternLot2)
                || name == nameof(FilterExpirationDate))
            {
                Search();
            }
        }

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

        /// <summary>
        /// Called when [activate].
        /// </summary>
        /// <param name="page">The page.</param>
        [ACMethodInfo(FacilityCharge.ClassName, "en{'Activate'}de{'Aktiviere'}", 9999, true, Global.ACKinds.MSMethodPrePost, Description =
                         @"Called when a posting tab is activated; sets CurrentBookParam to the booking parameters of the activated tab (inward, outward, relocation, release/lock, availability, reassignment, split, expiration date).")]
        public virtual void OnActivate(string page)
        {
            if (!PreExecute("OnActivate"))
                return;
            if (page == "ActivateInwardMovement" || page == "InwardBookingTab" || page == "*InwardBookingTab")
            {
                CurrentBookParam = CurrentBookParamInwardMovement;
            }
            else if (page == "ActivateOutwardMovement" || page == "OutwardBookingTab" || page == "*OutwardBookingTab")
            {
                CurrentBookParam = CurrentBookParamOutwardMovement;
            }
            else if (page == "ActivateRelocation" || page == "RelocationTab" || page == "*RelocationTab")
            {
                CurrentBookParam = CurrentBookParamRelocation;
            }
            else if (page == "ActivateReleaseAndLock" || page == "ReleaseAndLockTab" || page == "*ReleaseAndLockTab")
            {
                CurrentBookParam = CurrentBookParamReleaseAndLock;
            }
            else if (page == "ActivateNotAvailable" || page == "AvailabilityTab" || page == "*AvailabilityTab")
            {
                CurrentBookParam = CurrentBookParamNotAvailable;
            }
            else if (page == "ActivateReassignMat" || page == "ReassignTab" || page == "*ReassignTab")
            {
                CurrentBookParam = CurrentBookParamReassignMat;
            }
            else if (page == "ActivateSplitQuant" || page == "SplitQuantTab" || page == "*SplitQuantTab")
            {
                CurrentBookParam = CurrentBookParamSplit;
            }
            else if (page == "ActivateReassignLot" || page == "ReassignLotTab" || page == "*ReassignLotTab")
            {
                CurrentBookParam = CurrentBookParamReassignLot;
            }
            else if (page == "ChangeExpirationDate" || page == "ChangeExpirationDateTab" || page == "*ChangeExpirationDateTab")
            {
                CurrentBookParam = CurrentBookParamChangeExpirationDate;
            }
            PostExecute("OnActivate");

        }

        /// <summary>
        /// BSOs the facility book charge_ on value changed.
        /// </summary>
        /// <param name="dataContent">Content of the data.</param>
        void BSOFacilityBookCharge_OnValueChanged(string dataContent)
        {
            switch (dataContent)
            {
                case "CurrentFacilityCharge.Material":
                    if ((CurrentFacilityCharge.Material != null) && DatabaseApp.IsChanged)
                    {
                        if (CurrentFacilityCharge.Material.BaseMDUnit != null)
                            CurrentFacilityCharge.MDUnit = CurrentFacilityCharge.Material.BaseMDUnit;
                        else
                            CurrentFacilityCharge.MDUnit = null;
                    }
                    break;
            }
        }

        #endregion

        #region private members

        /// <summary>
        /// The _ book param inward movement
        /// </summary>
        ACMethodBooking _BookParamInwardMovement;
        ACMethodBooking _BookParamInwardMovementClone;
        /// <summary>
        /// The _ book param outward movement
        /// </summary>
        ACMethodBooking _BookParamOutwardMovement;
        ACMethodBooking _BookParamOutwardMovementClone;
        /// <summary>
        /// The _ book param relocation
        /// </summary>
        ACMethodBooking _BookParamRelocation;
        ACMethodBooking _BookParamRelocationClone;
        /// <summary>
        /// The _ book param release and lock
        /// </summary>
        ACMethodBooking _BookParamReleaseAndLock;
        ACMethodBooking _BookParamReleaseAndLockClone;
        /// <summary>
        /// The _ book param not available
        /// </summary>
        ACMethodBooking _BookParamNotAvailable;
        ACMethodBooking _BookParamNotAvailableClone;

        ACMethodBooking _BookParamReassignMat;
        ACMethodBooking _BookParamReassignMatClone;

        ACMethodBooking _BookParamSplit;
        ACMethodBooking _BookParamSplitClone;

        ACMethodBooking _BookParamReassignLot;
        ACMethodBooking _BookParamReassignLotClone;
        ACMethodBooking _BookParamChangeExpirationDate;
        ACMethodBooking _BookParamChangeExpirationDateClone;

        /// <summary>
        /// The _ act booking param
        /// </summary>
        ACMethodBooking _ActBookingParam;

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(New):
                    New();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
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
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(ClearBookingData):
                    ClearBookingData();
                    return true;
                case nameof(IsEnabledClearBookingData):
                    result = IsEnabledClearBookingData();
                    return true;
                case nameof(InwardFacilityChargeMovement):
                    InwardFacilityChargeMovement();
                    return true;
                case nameof(IsEnabledInwardFacilityChargeMovement):
                    result = IsEnabledInwardFacilityChargeMovement();
                    return true;
                case nameof(OutwardFacilityChargeMovement):
                    OutwardFacilityChargeMovement();
                    return true;
                case nameof(IsEnabledOutwardFacilityChargeMovement):
                    result = IsEnabledOutwardFacilityChargeMovement();
                    return true;
                case nameof(FacilityChargeRelocation):
                    result = FacilityChargeRelocation();
                    return true;
                case nameof(IsEnabledFacilityChargeRelocation):
                    result = IsEnabledFacilityChargeRelocation();
                    return true;
                case nameof(LockFacilityCharge):
                    LockFacilityCharge();
                    return true;
                case nameof(IsEnabledLockFacilityCharge):
                    result = IsEnabledLockFacilityCharge();
                    return true;
                case nameof(LockFacilityChargeAbsolute):
                    LockFacilityChargeAbsolute();
                    return true;
                case nameof(IsEnabledLockFacilityChargeAbsolute):
                    result = IsEnabledLockFacilityChargeAbsolute();
                    return true;
                case nameof(ReleaseFacilityCharge):
                    ReleaseFacilityCharge();
                    return true;
                case nameof(IsEnabledReleaseFacilityCharge):
                    result = IsEnabledReleaseFacilityCharge();
                    return true;
                case nameof(ReleaseFacilityChargeAbsolute):
                    ReleaseFacilityChargeAbsolute();
                    return true;
                case nameof(IsEnabledReleaseFacilityChargeAbsolute):
                    result = IsEnabledReleaseFacilityChargeAbsolute();
                    return true;
                case nameof(NotAvailableFacilityCharge):
                    NotAvailableFacilityCharge();
                    return true;
                case nameof(IsEnabledNotAvailableFacilityCharge):
                    result = IsEnabledNotAvailableFacilityCharge();
                    return true;
                case nameof(AvailableFacilityCharge):
                    AvailableFacilityCharge();
                    return true;
                case nameof(IsEnabledAvailableFacilityCharge):
                    result = IsEnabledAvailableFacilityCharge();
                    return true;
                case nameof(FacilityReassign):
                    FacilityReassign();
                    return true;
                case nameof(IsEnabledFacilityReassign):
                    result = IsEnabledFacilityReassign();
                    return true;
                case nameof(FacilityLotReassign):
                    FacilityLotReassign();
                    return true;
                case nameof(IsEnabledFacilityLotReassign):
                    result = IsEnabledFacilityLotReassign();
                    return true;
                case nameof(SplitQuant):
                    SplitQuant();
                    return true;
                case nameof(IsEnabledSplitQuant):
                    result = IsEnabledSplitQuant();
                    return true;
                case nameof(NewChargeNo):
                    NewChargeNo();
                    return true;
                case nameof(IsEnabledNewChargeNo):
                    result = IsEnabledNewChargeNo();
                    return true;
                case nameof(NewSplitChargeNo):
                    NewSplitChargeNo();
                    return true;
                case nameof(IsEnabledNewSplitChargeNo):
                    result = IsEnabledNewSplitChargeNo();
                    return true;
                case nameof(FacilityChargeLotGenerateDlg):
                    result = FacilityChargeLotGenerateDlg();
                    return true;
                case nameof(IsEnabledFacilityChargeLotGenerateDlg):
                    result = IsEnabledFacilityChargeLotGenerateDlg();
                    return true;
                case nameof(OnActivate):
                    OnActivate((String)acParameter[0]);
                    return true;
                case nameof(SearchFacilityChargeList):
                    SearchFacilityChargeList();
                    return true;
                case nameof(IsEnabledSearchFacilityChargeList):
                    result = IsEnabledSearchFacilityChargeList();
                    return true;
                case nameof(NotAvailableFacilityChargeAll):
                    NotAvailableFacilityChargeAll();
                    return true;
                case nameof(IsEnabledNotAvailableFacilityChargeAll):
                    result = IsEnabledNotAvailableFacilityChargeAll();
                    return true;
                case nameof(AvailableFacilityChargeAll):
                    AvailableFacilityChargeAll();
                    return true;
                case nameof(IsEnabledAvailableFacilityChargeAll):
                    result = IsEnabledAvailableFacilityChargeAll();
                    return true;
                case nameof(NavigateToOrder):
                    result = NavigateToOrder();
                    return true;
                case nameof(IsEnabledNavigateToOrder):
                    result = IsEnabledNavigateToOrder();
                    return true;
                case nameof(NavigateToFacilityLot):
                    result = NavigateToFacilityLot();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLot):
                    result = IsEnabledNavigateToFacilityLot();
                    return true;
                case nameof(NavigateToFacilityLotOverview):
                    result = NavigateToFacilityLotOverview();
                    return true;
                case nameof(IsEnabledNavigateToFacilityLotOverview):
                    result = IsEnabledNavigateToFacilityLotOverview();
                    return true;
                case nameof(NavigateToFacilityChargeHistory):
                    result = NavigateToFacilityChargeHistory();
                    return true;
                case nameof(IsEnabledNavigateToFacilityChargeHistory):
                    result = IsEnabledNavigateToFacilityChargeHistory();
                    return true;
                case nameof(ShowDialogOrderInfo):
                    result = ShowDialogOrderInfo((gip.core.autocomponent.PAOrderInfo)acParameter[0]);
                    return true;
                case nameof(NavigateToFacility):
                    result = NavigateToFacility();
                    return true;
                case nameof(IsEnabledNavigateToFacility):
                    result = IsEnabledNavigateToFacility();
                    return true;
                case nameof(NavigateToFacilityOverview):
                    result = NavigateToFacilityOverview();
                    return true;
                case nameof(IsEnabledNavigateToFacilityOverview):
                    result = IsEnabledNavigateToFacilityOverview();
                    return true;
                case nameof(NavigateToMaterial):
                    result = NavigateToMaterial();
                    return true;
                case nameof(IsEnabledNavigateToMaterial):
                    result = IsEnabledNavigateToMaterial();
                    return true;
                case nameof(NavigateToMaterialOverview):
                    result = NavigateToMaterialOverview();
                    return true;
                case nameof(IsEnabledNavigateToMaterialOverview):
                    result = IsEnabledNavigateToMaterialOverview();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                // General operations
                case nameof(New):
                case nameof(IsEnabledNew):
                case nameof(Save):
                case nameof(IsEnabledSave):
                case nameof(UndoSave):
                case nameof(IsEnabledUndoSave):
                case nameof(Load):
                case nameof(IsEnabledLoad):
                case nameof(Delete):
                case nameof(IsEnabledDelete):
                case nameof(Search):
                case nameof(ClearBookingData):
                case nameof(IsEnabledClearBookingData):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };

                // Booking operations
                case nameof(InwardFacilityChargeMovement):
                case nameof(IsEnabledInwardFacilityChargeMovement):
                    return new[] { "CurrentBookParamInwardMovement" };
                case nameof(OutwardFacilityChargeMovement):
                case nameof(IsEnabledOutwardFacilityChargeMovement):
                    return new[] { "CurrentBookParamOutwardMovement" };
                case nameof(FacilityChargeRelocation):
                case nameof(IsEnabledFacilityChargeRelocation):
                    return new[] { "CurrentBookParamRelocation" };
                case nameof(LockFacilityCharge):
                case nameof(IsEnabledLockFacilityCharge):
                case nameof(LockFacilityChargeAbsolute):
                case nameof(IsEnabledLockFacilityChargeAbsolute):
                case nameof(ReleaseFacilityCharge):
                case nameof(IsEnabledReleaseFacilityCharge):
                case nameof(ReleaseFacilityChargeAbsolute):
                case nameof(IsEnabledReleaseFacilityChargeAbsolute):
                    return new[] { "CurrentFacilityCharge", "CurrentBookParamReleaseAndLock" };
                case nameof(NotAvailableFacilityCharge):
                case nameof(IsEnabledNotAvailableFacilityCharge):
                case nameof(AvailableFacilityCharge):
                case nameof(IsEnabledAvailableFacilityCharge):
                    return new[] { "CurrentFacilityCharge", "CurrentBookParamNotAvailable" };
                case nameof(FacilityReassign):
                case nameof(IsEnabledFacilityReassign):
                case nameof(FacilityLotReassign):
                case nameof(IsEnabledFacilityLotReassign):
                    return new[] { "CurrentFacilityCharge", "SelectedRassignmentMaterial" };
                case nameof(SplitQuant):
                case nameof(IsEnabledSplitQuant):
                    return new[] { "CurrentFacilityCharge", "FBCTargetQuantityUOM" };

                // New charge/split operations
                case nameof(NewChargeNo):
                case nameof(IsEnabledNewChargeNo):
                case nameof(NewSplitChargeNo):
                case nameof(IsEnabledNewSplitChargeNo):
                    return new[] { "CurrentFacilityCharge" };

                // All availability operations
                case nameof(NotAvailableFacilityChargeAll):
                case nameof(IsEnabledNotAvailableFacilityChargeAll):
                case nameof(AvailableFacilityChargeAll):
                case nameof(IsEnabledAvailableFacilityChargeAll):
                    return new[] { "FacilityChargeList" };

                // Navigation methods
                case nameof(NavigateToOrder):
                case nameof(IsEnabledNavigateToOrder):
                case nameof(NavigateToFacilityLot):
                case nameof(IsEnabledNavigateToFacilityLot):
                case nameof(NavigateToFacilityLotOverview):
                case nameof(IsEnabledNavigateToFacilityLotOverview):
                case nameof(NavigateToFacilityChargeHistory):
                case nameof(IsEnabledNavigateToFacilityChargeHistory):
                case nameof(NavigateToFacility):
                case nameof(IsEnabledNavigateToFacility):
                case nameof(NavigateToFacilityOverview):
                case nameof(IsEnabledNavigateToFacilityOverview):
                case nameof(NavigateToMaterial):
                case nameof(IsEnabledNavigateToMaterial):
                case nameof(NavigateToMaterialOverview):
                case nameof(IsEnabledNavigateToMaterialOverview):
                    return new[] { "SelectedFacilityCharge" };

                // Dialog/other
                case nameof(ShowDialogOrderInfo):
                    return new[] { "SelectedFacilityCharge" };
                case nameof(FacilityChargeLotGenerateDlg):
                case nameof(IsEnabledFacilityChargeLotGenerateDlg):
                    return new[] { "CurrentFacilityCharge" };
                case nameof(SearchFacilityChargeList):
                case nameof(IsEnabledSearchFacilityChargeList):
                    return new[] { "FilterMaterial", "FilterFacility", "FilterLot", "FilterExternLot", "FilterExternLot2", "StockQuantityLessThan", "FilterExpirationDate", "ShowNotAvailable" };
                case nameof(OnActivate):
                    return new[] { "CurrentFacilityCharge", "SelectedFacilityCharge" };

                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

        #region IACPrintPrepare

        public override Msg FilterByOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (paOrderInfo != null)
            {
                PAOrderInfoEntry entry = paOrderInfo.Entities.Where(c => c.EntityName == FacilityCharge.ClassName).FirstOrDefault();
                CurrentFacilityCharge =
                    DatabaseApp
                    .FacilityCharge
                    .Include(c => c.Facility)
                    .Include(c => c.FacilityLot)
                    .Include(c => c.Material)
                    .FirstOrDefault(c => c.FacilityChargeID == entry.EntityID);
                return null;
            }
            return new Msg() { MessageLevel = eMsgLevel.Error, Message = "" };
        }

        protected override IEnumerable<core.datamodel.ACClassDesign> OnGetDefaultPrintDesigns(string printerName, PAOrderInfo paOrderInfo = null)
        {
            return base.OnGetDefaultPrintDesigns(printerName, paOrderInfo);
        }
        #endregion

        #region Overrides


        #region Overrides -> PrintOrderInfo & GS1
        public override PAOrderInfo GetOrderInfo()
        {
            PAOrderInfo pAOrderInfo = new PAOrderInfo();
            if (SelectedFacilityCharge != null)
                pAOrderInfo.Add(FacilityCharge.ClassName, SelectedFacilityCharge.FacilityChargeID);
            return pAOrderInfo;
        }

        /// <summary>
        /// Handle additional params by printing via PAOrderInfo
        /// when in params FacilityBookingCharge is present, set FBCTargetQuantityUOM
        /// </summary>
        /// <param name="paOrderInfo"></param>
        /// <param name="printerName"></param>
        /// <param name="numberOfCopies"></param>
        /// <param name="designName"></param>
        /// <param name="maxPrintJobsInSpooler"></param>
        /// <param name="preventClone"></param>
        /// <returns></returns>
        public override Msg PrintByOrderInfo(PAOrderInfo paOrderInfo, string printerName, short numberOfCopies, string designName = null, int maxPrintJobsInSpooler = 0, bool preventClone = true)
        {
            if (paOrderInfo != null)
            {
                ExtractAdditionalDataFromPAOrderInfo(paOrderInfo);
            }
            return base.PrintByOrderInfo(paOrderInfo, printerName, numberOfCopies, designName, maxPrintJobsInSpooler, preventClone);
        }

        public override core.datamodel.ACClassDesign GetDesignForPrinting(string printerName, string designName = null, PAOrderInfo paOrderInfo = null)
        {
            if (paOrderInfo != null)
            {
                ExtractAdditionalDataFromPAOrderInfo(paOrderInfo);
            }
            return base.GetDesignForPrinting(printerName, designName, paOrderInfo);
        }

        public virtual void ExtractAdditionalDataFromPAOrderInfo(PAOrderInfo paOrderInfo)
        {
            PAOrderInfoEntry fbcPaorderEntry = paOrderInfo.Entities.Where(c => c.EntityName == nameof(FacilityBookingCharge)).FirstOrDefault();
            if (fbcPaorderEntry != null)
            {
                FacilityBookingCharge facilityBookingCharge = DatabaseApp.FacilityBookingCharge.FirstOrDefault(c => c.FacilityBookingChargeID == fbcPaorderEntry.EntityID);
                if (facilityBookingCharge != null)
                {
                    if (facilityBookingCharge.InwardTargetQuantityUOM > 0)
                    {
                        FBCTargetQuantityUOM = facilityBookingCharge.InwardTargetQuantityUOM;
                        if(CurrentFacilityCharge != null)
                        {
                            CurrentFacilityCharge.FBCTargetQuantityUOM = facilityBookingCharge.InwardTargetQuantityUOM;
                        }
                    }
                    if (facilityBookingCharge.OutwardTargetQuantityUOM > 0)
                    {
                        FBCTargetQuantityUOM = facilityBookingCharge.OutwardTargetQuantityUOM;
                        if(CurrentFacilityCharge != null)
                        {
                            CurrentFacilityCharge.FBCTargetQuantityUOM = facilityBookingCharge.OutwardTargetQuantityUOM;
                        }
                    }
                }

                if (facilityBookingCharge.PickingPos != null)
                {
                    RefDocumentNo = facilityBookingCharge.PickingPos.Picking.PickingNo;
                }
            }
        }


        /// <summary>
        /// Forward FBCTargetQuantityUOM into FacilityCharge.FBCTargetQuantityUOM for availability in reports
        /// </summary>
        /// <param name="design"></param>
        /// <param name="printerName"></param>
        /// <param name="numberOfCopies"></param>
        /// <param name="withDialog"></param>
        /// <param name="selectMode"></param>
        /// <param name="queryDefinition"></param>
        /// <param name="maxPrintJobsInSpooler"></param>
        /// <param name="preventClone"></param>
        /// <returns></returns>
        public override Msg PrintDesign(gip.core.datamodel.ACClassDesign design, string printerName, int numberOfCopies, bool withDialog, Global.CurrentOrList selectMode = Global.CurrentOrList.Current, ACQueryDefinition queryDefinition = null, int maxPrintJobsInSpooler = 0, bool preventClone = false)
        {
            if (CurrentFacilityCharge != null)
            {
                CurrentFacilityCharge.FBCTargetQuantityUOM = FBCTargetQuantityUOM;
            }
            return base.PrintDesign(design, printerName, numberOfCopies, withDialog, selectMode, queryDefinition, maxPrintJobsInSpooler, preventClone);
        }

        public override object Clone()
        {
            BSOFacilityBookCharge fc = base.Clone() as BSOFacilityBookCharge;
            if (fc != null)
            {
                fc.CurrentFacilityCharge = CurrentFacilityCharge;
                fc.FBCTargetQuantityUOM = FBCTargetQuantityUOM;
                fc.RefDocumentNo = RefDocumentNo;
            }
            return fc;
        }

        #endregion

        //public override bool IsPoolable
        //{
        //    get
        //    {
        //        return true;
        //    }
        //}
        #endregion

        #region All FacilityCharge availability -> Not available

        private ACMethodBooking[] GetMethodBookingsForNotAvailable(FacilityCharge[] facilityCharges)
        {
            List<ACMethodBooking> result = new List<ACMethodBooking>();
            foreach (FacilityCharge facilityCharge in facilityCharges)
            {
                if (!facilityCharge.NotAvailable)
                {
                    ACMethodBooking aCMethodBooking = GetMethodBookingForNotAvailable(facilityCharge);
                    result.Add(aCMethodBooking);
                }
            }
            return result.ToArray();
        }

        private ACMethodBooking GetMethodBookingForNotAvailable(FacilityCharge facilityCharge)
        {
            ACMethodBooking bookParamNotAvailable = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            bookParamNotAvailable.InwardFacilityCharge = facilityCharge;
            bookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
            bookParamNotAvailable.AutoRefresh = true;
            return bookParamNotAvailable;
        }

        #endregion
        #region All FacilityCharge availability -> Available

        private ACMethodBooking[] GetMethodBookingsForAvailable(FacilityCharge[] facilityCharges, bool restoreLastStock)
        {
            List<ACMethodBooking> result = new List<ACMethodBooking>();
            foreach (FacilityCharge facilityCharge in facilityCharges)
            {
                if (facilityCharge.NotAvailable)
                {
                    ACMethodBooking aCMethodBooking = GetMethodBookingForAvailable(facilityCharge, restoreLastStock);
                    result.Add(aCMethodBooking);
                }
            }
            return result.ToArray();
        }

        private ACMethodBooking GetMethodBookingForAvailable(FacilityCharge facilityCharge, bool restoreLastStock)
        {
            FacilityBookingCharge fbc =
                facilityCharge?.FacilityBookingCharge_InwardFacilityCharge
                                .Where(c => c.FacilityBookingTypeIndex == (short)GlobalApp.FacilityBookingType.ZeroStock_Facility_BulkMaterial)
                                .OrderByDescending(c => c.InsertDate)
                                .FirstOrDefault();

            ACMethodBooking bookParamNotAvailable = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            bookParamNotAvailable.InwardFacilityCharge = facilityCharge;
            bookParamNotAvailable.AutoRefresh = true;

            if (fbc != null && restoreLastStock)
            {
                bookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.RestoreQuantityIfNotAvailable);
            }
            else
            {
                bookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.ResetIfNotAvailable);
            }

            return bookParamNotAvailable;
        }


        #endregion

        #region All FacilityCharge availability -> Messages

        private MsgWithDetails GetBookingMessages(Dictionary<ACMethodBooking, ACMethodEventArgs> results)
        {
            MsgWithDetails msgWithDetails = new MsgWithDetails();
            foreach (KeyValuePair<ACMethodBooking, ACMethodEventArgs> item in results)
            {
                MsgWithDetails msg = GetBookingMessage(item.Key, item.Value);
                if (msg != null)
                {
                    msgWithDetails.AddDetailMessage(msg);
                }
            }
            return msgWithDetails;
        }

        private MsgWithDetails GetBookingMessage(ACMethodBooking aCMethodBooking, ACMethodEventArgs result)
        {
            MsgWithDetails msgWithDetails = null;
            if (!aCMethodBooking.ValidMessage.IsSucceded() || aCMethodBooking.ValidMessage.HasWarnings())
            {
                msgWithDetails = aCMethodBooking.ValidMessage;
            }
            else if (result != null && (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible))
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                {
                    result.ValidMessage.Message = result.ResultState.ToString();
                }
                msgWithDetails = result.ValidMessage;
            }

            return msgWithDetails;
        }

        #endregion

        #endregion
    }
}
