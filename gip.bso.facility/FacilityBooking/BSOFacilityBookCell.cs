// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 11-08-2012
// ***********************************************************************
// <copyright file="BSOFacilityBookCell.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading;

namespace gip.bso.facility
{
    /// <summary>
    /// BSOFacilityBookCell dient zur Einlagerung, Umlagerung und Ausbuchung von Chargen
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Bin Management'}de{'Behälterverwaltung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Facility.ClassName)]
    [ACQueryInfo(Const.PackName_VarioFacility, Const.QueryPrefix + "BookingFacility", "en{'Storage Bin'}de{'Lagerplatz'}", typeof(Facility), Facility.ClassName, MDFacilityType.ClassName + "\\MDFacilityTypeIndex", "FacilityNo")]
    public class BSOFacilityBookCell : BSOFacilityBase
    {
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

        /// <summary>
        /// The _ act booking param
        /// </summary>
        ACMethodBooking _ActBookingParam;

        /// <summary>
        /// The _ booking parameter matching
        /// </summary>
        ACMethodBooking _BookParamMatching;
        ACMethodBooking _BookParamMatchingClone;

        ACMethodBooking _BookParamReassignMat;
        ACMethodBooking _BookParamReassignMatClone;

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityBookCell"/> class.
        /// </summary>
        /// <param name="typeACClass">The type AC class.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityBookCell(gip.core.datamodel.ACClass typeACClass, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(typeACClass, content, parentACObject, parameter)
        {
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
            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._SelectedFacilityCharge = null;
            this._CurrentFacilityCharge = null;
            this._BookParamInwardMovement = null;
            this._BookParamInwardMovementClone = null;
            this._BookParamMatching = null;
            this._BookParamMatchingClone = null;
            this._BookParamNotAvailable = null;
            this._BookParamNotAvailableClone = null;
            this._BookParamOutwardMovement = null;
            this._BookParamOutwardMovementClone = null;
            this._BookParamReassignMat = null;
            this._BookParamReassignMatClone = null;
            this._BookParamReleaseAndLock = null;
            this._BookParamReleaseAndLockClone = null;
            this._BookParamRelocation = null;
            this._BookParamRelocationClone = null;
            this._AccessBookingFacility = null;

            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessBookingFacility != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }
        #endregion

        #region BSO->ACProperty
        #region BSO->ACProperty->BookingParam
        /// <summary>
        /// Gets the current book param.
        /// </summary>
        /// <value>The current book param.</value>
        [ACPropertyCurrent(701, "BookParam")]
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
        [ACPropertyCurrent(702, "BookParamInwardMovement")]
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
        [ACPropertyCurrent(703, "BookParamOutwardMovement")]
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
        [ACPropertyCurrent(704, "BookParamRelocation")]
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
        [ACPropertyCurrent(705, "BookParamReleaseAndLock")]
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
        [ACPropertyCurrent(706, "BookParamNotAvailable")]
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
        [ACPropertyCurrent(707, "BookParamMatching")]
        public ACMethodBooking CurrentBookParamMatching
        {
            get
            {
                return _BookParamMatching;
            }
            protected set
            {
                _BookParamMatching = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current book param matching.
        /// </summary>
        /// <value>The current book param matching.</value>
        [ACPropertyCurrent(708, "CurrentBookParamReassignMat")]
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


        private bool _BookingFilterMaterial = true;
        [ACPropertyInfo(709, "", "en{'Only show bins with material'}de{'Zeige Lagerpätze mit Material'}")]
        public bool BookingFilterMaterial
        {
            get
            {
                return _BookingFilterMaterial;
            }
            set
            {
                _BookingFilterMaterial = value;
                OnPropertyChanged();
                RefreshFilterFacilityAccess();
                OnPropertyChanged(nameof(BookingFacilityList));
            }
        }


        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<Facility> _AccessBookingFacility;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(710, "BookingFacility")]
        public ACAccessNav<Facility> AccessBookingFacility
        {
            get
            {
                if (_AccessBookingFacility == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "BookingFacility", ACType.ACIdentifier);
                    _AccessBookingFacility = navACQueryDefinition.NewAccessNav<Facility>("BookingFacility", this);
                    _AccessBookingFacility.AutoSaveOnNavigation = false;
                    RefreshFilterFacilityAccess();
                }
                return _AccessBookingFacility;
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
            if (acAccess == _AccessBookingFacility)
            {
                _AccessBookingFacility.NavSearch(this.DatabaseApp);
                OnPropertyChanged(nameof(BookingFacilityList));
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }

        IQueryable<Facility> _AccessPrimary_NavSearchExecuting(IQueryable<Facility> result)
        {
            ObjectQuery<Facility> query = result as ObjectQuery<Facility>;
            if (query != null)
            {
                query.Include(c => c.FacilityStock_Facility)
                        .Include(c => c.Material)
                        .Include(c => c.FacilityCharge_Facility);
            }
            return result;
        }


        [ACPropertyList(711, "BookingFacility")]
        public IList<Facility> BookingFacilityList
        {
            get
            {
                if (AccessBookingFacility == null)
                    return null;
                return AccessBookingFacility.NavList;
            }
        }

        private void RefreshFilterFacilityAccess()
        {
            if (AccessBookingFacility == null || CurrentFacility == null)
                return;
            bool rebuildACQueryDef = false;
            short fcTypeContainer = (short)FacilityTypesEnum.StorageBinContainer;
            short fcTypeBin = (short)FacilityTypesEnum.StorageBin;
            if (AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Count <= 0)
            {
                rebuildACQueryDef = true;
            }
            else
            {
                int countFoundCorrect = 0;
                foreach (ACFilterItem filterItem in AccessBookingFacility.NavACQueryDefinition.ACFilterColumns)
                {
                    if (filterItem.FilterType != Global.FilterTypes.filter)
                        continue;
                    if (filterItem.PropertyName == nameof(MDFacilityType) + "\\" + nameof(MDFacilityType.MDFacilityTypeIndex))
                    {
                        if ((BookingFilterMaterial && filterItem.SearchWord == fcTypeContainer.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal)
                            || (!BookingFilterMaterial && filterItem.SearchWord == fcTypeBin.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal))
                            countFoundCorrect++;
                    }
                    else if (BookingFilterMaterial && filterItem.PropertyName == nameof(Material) + "\\" + nameof(Material.MaterialNo) 
                                                   && CurrentFacility != null && CurrentFacility.Material != null)
                    {
                        if (filterItem.SearchWord == CurrentFacility.Material.MaterialNo)
                            countFoundCorrect++;
                    }
                }
                if (BookingFilterMaterial && countFoundCorrect < 2)
                    rebuildACQueryDef = true;
                else if (!BookingFilterMaterial && countFoundCorrect < 1)
                    rebuildACQueryDef = true;
            }
            if (rebuildACQueryDef)
            {
                AccessBookingFacility.NavACQueryDefinition.ClearFilter(true);
                if (BookingFilterMaterial)
                {
                    AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, nameof(MDFacilityType) + "\\" + nameof(MDFacilityType.MDFacilityTypeIndex), Global.LogicalOperators.equal, Global.Operators.and, fcTypeContainer.ToString(), true));
                    if (CurrentFacility != null && CurrentFacility.Material != null)
                    {
                        AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, false));
                        AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, nameof(Material) + "\\" + nameof(Material.MaterialNo), Global.LogicalOperators.equal, Global.Operators.or, CurrentFacility.Material.MaterialNo, false));
                        if (CurrentFacility.Material.Material1_ProductionMaterial != null)
                            AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, nameof(Material) + "\\" + nameof(Material.MaterialNo), Global.LogicalOperators.equal, Global.Operators.or, CurrentFacility.Material.Material1_ProductionMaterial.MaterialNo, false));
                        AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, false));
                    }
                }
                else
                {
                    AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, nameof(MDFacilityType) + "\\" + nameof(MDFacilityType.MDFacilityTypeIndex), Global.LogicalOperators.equal, Global.Operators.and, fcTypeBin.ToString(), true));
                }
                AccessBookingFacility.NavACQueryDefinition.SaveConfig(false);
            }
            AccessBookingFacility.NavSearch(this.DatabaseApp);
        }

        #endregion

        #region BSO->ACProperty->Facility
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<Facility> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(790, Facility.ClassName)]
        public ACAccessNav<Facility> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Facility>(Facility.ClassName, this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessPrimary;
            }
        }

        private List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, nameof(Facility.FacilityNo), Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.filter, nameof(Facility.FacilityName), Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, nameof(MDFacilityType) + "\\" + nameof(MDFacilityType.MDFacilityTypeIndex), Global.LogicalOperators.equal, Global.Operators.and, ((Int16)(int)FacilityTypesEnum.StorageBinContainer).ToString(), true)
                };
            }
        }

        /// <summary>
        /// Gets or sets the selected facility.
        /// </summary>
        /// <value>The selected facility.</value>
        [ACPropertySelected(712, Facility.ClassName)]
        public Facility SelectedFacility
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
        /// Gets or sets the current facility.
        /// </summary>
        /// <value>The current facility.</value>
        [ACPropertyCurrent(713, Facility.ClassName)]
        public Facility CurrentFacility
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
                if (CurrentFacility != null)
                    CurrentFacility.PropertyChanged -= CurrentFacility_PropertyChanged;
                AccessPrimary.CurrentNavObject = value;

                if (value != null)
                    value.PropertyChanged += CurrentFacility_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(FacilityLotList));
                OnPropertyChanged(nameof(FacilityChargeList));
                OnPropertyChanged(nameof(ContractualPartnerList));
                ClearBookingData();
            }
        }

        void CurrentFacility_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Material.MaterialID))
            {
                OnPropertyChanged(nameof(StorageUnitTestList));
                OnPropertyChanged(nameof(ContractualPartnerList));
                if (CurrentFacility != null)
                    CurrentFacility.OnEntityPropertyChanged(nameof(Material));
            }
        }


        /// <summary>
        /// Gets the facility list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(714, Facility.ClassName)]
        public IEnumerable<Facility> FacilityList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region BSO->ACProperty->FacilityCharge
        /// <summary>
        /// The _ current facility charge
        /// </summary>
        FacilityCharge _CurrentFacilityCharge;
        /// <summary>
        /// Gets or sets the current facility charge.
        /// </summary>
        /// <value>The current facility charge.</value>
        [ACPropertyCurrent(715, FacilityCharge.ClassName)]
        public FacilityCharge CurrentFacilityCharge
        {
            get
            {
                return _CurrentFacilityCharge;
            }
            set
            {
                _CurrentFacilityCharge = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the facility charge list.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(716, FacilityCharge.ClassName)]
        public IEnumerable<FacilityCharge> FacilityChargeList
        {
            get
            {
                if (CurrentFacility == null)
                    return null;
                CurrentFacility.FacilityCharge_Facility.AutoLoad(this.DatabaseApp);
                return CurrentFacility.FacilityCharge_Facility.Where(c => !c.NotAvailable).OrderBy(c => c.FacilityChargeSortNo).ToList();
            }
        }

        /// <summary>
        /// The _ selected facility charge
        /// </summary>
        FacilityCharge _SelectedFacilityCharge;
        /// <summary>
        /// Gets or sets the selected facility charge.
        /// </summary>
        /// <value>The selected facility charge.</value>
        [ACPropertySelected(717, FacilityCharge.ClassName)]
        public FacilityCharge SelectedFacilityCharge
        {
            get
            {
                return _SelectedFacilityCharge;
            }
            set
            {
                _SelectedFacilityCharge = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region BSO->ACProperty->HelperLists
        /// <summary>
        /// Gets the partslist list.
        /// </summary>
        /// <value>The partslist list.</value>
        [ACPropertyList(718, "Partslist")]
        public IEnumerable<Partslist> PartslistList
        {
            get
            {
                if (CurrentFacility == null)
                    return null;
                if (CurrentFacility.Material == null)
                    return null;
                return CurrentFacility.Material.Partslist_Material.AsEnumerable();
            }
        }

        /// <summary>
        /// Gets the facility lot list.
        /// </summary>
        /// <value>The facility lot list.</value>
        [ACPropertyList(719, FacilityLot.ClassName)]
        public IEnumerable<FacilityLot> FacilityLotList
        {
            get
            {
                if (CurrentFacility == null)
                    return null;
                if (CurrentFacility.Material == null)
                    return null;
                return CurrentFacility.Material.FacilityLot_Material.AsEnumerable();
            }
        }
        #endregion
         
        #region Units
        [ACPropertyList(791, "StorageUnit")]
        public IEnumerable<MDUnit> StorageUnitTestList
        {
            get
            {
                if (CurrentFacility == null)
                    return null;
                if (CurrentFacility.Material == null)
                    return null;
                return CurrentFacility.Material.MDUnitList;
            }
        }

        [ACPropertyList(792, "")]
        public IEnumerable<Company> ContractualPartnerList
        {
            get
            {
                if (CurrentFacility == null)
                    return null;
                if (CurrentFacility.Material == null)
                    return null;
                return CurrentFacility.Material.CompanyMaterial_Material.Where(c => c.Company.IsTenant).Select(c => c.Company);
            }
        }

        #endregion


        #region Message

        private MsgWithDetails _BSOMsg = new MsgWithDetails();
        [ACPropertyInfo(793, "Message")]
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
        #endregion

        #region BSO->ACMethod
        #region Allgemein

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(Facility.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(Facility.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacility", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute())
                return;
            LoadEntity<Facility>(requery, () => SelectedFacility, () => CurrentFacility, c => CurrentFacility = c,
                        DatabaseApp.Facility
                                    .Include(c => c.FacilityStock_Facility)
                                    .Include(c => c.Material)
                                    .Include(c => c.FacilityCharge_Facility)
                        .Where(c => c.FacilityID == SelectedFacility.FacilityID));
            if (CurrentFacility != null && CurrentFacility.CurrentFacilityStock != null)
                CurrentFacility.CurrentFacilityStock.AutoRefresh(DatabaseApp);
            PostExecute();
            ClearBookingData();
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedFacility != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp, MergeOption.OverwriteChanges);
            OnPropertyChanged(nameof(FacilityList));
        }

        /// <summary>
        /// Clears the booking data.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'New Bookingrecord'}de{'Neuer Buchungssatz'}", 701, true)]
        public virtual void ClearBookingData()
        {
            _CheckIsDischargingActiveToFacility = null;
            if (_BookParamInwardMovementClone == null)
                _BookParamInwardMovementClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_InwardMovement_Facility_BulkMaterial, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            ACMethodBooking clone = _BookParamInwardMovementClone.Clone() as ACMethodBooking;
            if (CurrentFacility != null)
            {
                if (CurrentFacility.Material != null)
                    clone.MDUnit = CurrentFacility.Material.BaseMDUnit;
                clone.InwardFacility = CurrentFacility;
            }
            CurrentBookParamInwardMovement = clone;

            _BookParamInwardMovementClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_StockCorrection, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            _BookParamInwardMovementClone.BookingType = GlobalApp.FacilityBookingType.StockCorrection;
            _BookParamInwardMovementClone.MDBalancingMode = DatabaseApp.s_cQry_GetMDBalancingMode(this.DatabaseApp, MDBalancingMode.BalancingModes.InwardOff_OutwardOff).FirstOrDefault();


            if (_BookParamOutwardMovementClone == null)
                _BookParamOutwardMovementClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_OutwardMovement_Facility_BulkMaterial, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamOutwardMovementClone.Clone() as ACMethodBooking;
            if (CurrentFacility != null)
            {
                if (CurrentFacility.Material != null)
                    clone.MDUnit = CurrentFacility.Material.BaseMDUnit;
                clone.OutwardFacility = CurrentFacility;
            }
            CurrentBookParamOutwardMovement = clone;

            if (_BookParamRelocationClone == null)
                _BookParamRelocationClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_Relocation_Facility_BulkMaterial, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamRelocationClone.Clone() as ACMethodBooking;
            if (CurrentFacility != null)
            {
                if (CurrentFacility.Material != null)
                    clone.MDUnit = CurrentFacility.Material.BaseMDUnit;
                clone.OutwardFacility = CurrentFacility;
            }
            CurrentBookParamRelocation = clone;

            if (_BookParamReleaseAndLockClone == null)
                _BookParamReleaseAndLockClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ReleaseState_Facility_BulkMaterial, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamReleaseAndLockClone.Clone() as ACMethodBooking;
            if (CurrentFacility != null)
            {
                clone.InwardFacility = CurrentFacility;
            }
            CurrentBookParamReleaseAndLock = clone;

            if (_BookParamNotAvailableClone == null)
                _BookParamNotAvailableClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_Facility_BulkMaterial, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamNotAvailableClone.Clone() as ACMethodBooking;
            if (CurrentFacility != null)
            {
                clone.InwardFacility = CurrentFacility;
            }
            CurrentBookParamNotAvailable = clone;

            if (_BookParamMatchingClone == null)
                _BookParamMatchingClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_MatchingFacilityStock, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamMatchingClone.Clone() as ACMethodBooking;
            if (CurrentFacility != null)
            {
                clone.InwardFacility = CurrentFacility;
            }
            CurrentBookParamMatching = clone;

            if (_BookParamReassignMatClone == null)
                _BookParamReassignMatClone = ACFacilityManager.ACUrlACTypeSignature("!" + GlobalApp.FBT_Reassign_Facility_BulkMaterial, gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            clone = _BookParamReassignMatClone.Clone() as ACMethodBooking;
            if (CurrentFacility != null)
            {
                clone.OutwardFacility = CurrentFacility;
            }
            CurrentBookParamReassignMat = clone;
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
                case "CurrentFacility\\Material":
                    return IsEnabledMaterial ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
                case "CurrentFacility\\Partslist":
                    return IsEnabledPartslist ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
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
        /// Inwards the facility movement.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Post Inward Movement'}de{'Buche Lagerzugang'}", 702, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void InwardFacilityMovement()
        {
            if (!PreExecute()) return;
            if (CurrentFacility.Partslist != null)
            {
                CurrentBookParamInwardMovement.InwardPartslist = CurrentFacility.Partslist;
                CurrentBookParamInwardMovement.InwardMaterial = CurrentFacility.Material;
            }

            if (IsPhysicalTransportPossible)
            {
                Global.MsgResult userQuestionAutomatic = Global.MsgResult.No;
                // Question50035: Do you want to run this relocation/transport on the plant in automatic mode?
                userQuestionAutomatic = Messages.YesNoCancel(this, "Question50035");
                if (userQuestionAutomatic == Global.MsgResult.Yes)
                {
                    gip.core.datamodel.ACClassMethod acClassMethod = null;
                    bool wfRunsBatches = false;
                    if (!PrepareStartWorkflow(CurrentBookParamInwardMovement, out acClassMethod, out wfRunsBatches))
                    {
                        ClearBookingData();
                        return;
                    }

                    ACMethodBooking booking = CurrentBookParamInwardMovement;
                    // If Workflow doesn't contain a PWNodeProcessWorkflow => Start relocation directly
                    if (!wfRunsBatches)
                    {
                        if (booking.InwardQuantity.HasValue)
                            booking.InwardQuantity = 0.000001;
                        if (booking.OutwardQuantity.HasValue)
                            booking.OutwardQuantity = 0.000001;
                        BookInwardFacilityMovement();
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
                            Messages.Msg(msgDetails);
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
                        Save();
                        msgDetails = ACPickingManager.ValidateStart(this.DatabaseApp, this.DatabaseApp.ContextIPlus, picking, null, PARole.ValidationBehaviour.Strict);
                        if (msgDetails != null && msgDetails.MsgDetailsCount > 0)
                        {
                            Messages.Msg(msgDetails);
                            ClearBookingData();
                            return;
                        }
                        StartWorkflow(acClassMethod, picking);
                    }

                }
                else if (userQuestionAutomatic == Global.MsgResult.No)
                    BookInwardFacilityMovement();
            }
            else
                BookInwardFacilityMovement();

            PostExecute();
        }

        protected virtual bool BookInwardFacilityMovement()
        {
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamInwardMovement, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamInwardMovement.ValidMessage.IsSucceded() || CurrentBookParamInwardMovement.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamInwardMovement.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacility));
                OnPropertyChanged(nameof(FacilityChargeList));
            }

            return true;
        }


        /// <summary>
        /// Determines whether [is enabled inward facility movement].
        /// </summary>
        /// <returns><c>true</c> if [is enabled inward facility movement]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledInwardFacilityMovement()
        {
            bool bRetVal = CurrentBookParamInwardMovement != null && CurrentBookParamInwardMovement.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }

        [ACMethodCommand(Facility.ClassName, "en{'New Lot'}de{'Neues Los'}", 703, true, Global.ACKinds.MSMethodPrePost)]
        public void InwardFacilityLotGenerateDlg()
        {
            if (!IsEnabledInwardFacilityLotGenerateDlg()) return;
            ACComponent childBSO = ACUrlCommand("?" + ConstApp.BSOFacilityLot_ChildName) as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent(ConstApp.BSOFacilityLot_ChildName, null, new object[] { }) as ACComponent;
            if (childBSO == null) return;
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!" + ConstApp.BSOFacilityLot_Dialog_ShowDialogNewLot, "", CurrentFacility.Material);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                FacilityLot lot = dlgResult.ReturnValue as FacilityLot;
                CurrentFacility.Material.FacilityLot_Material.Add(lot);
                CurrentBookParamInwardMovement.InwardFacilityLot = lot;
                OnPropertyChanged(nameof(FacilityLotList));
            }
            if (childBSO != null)
                childBSO.Stop();
        }

        private bool IsEnabledInwardFacilityLotGenerateDlg()
        {
            return CurrentFacility != null && CurrentFacility.Material != null && !IsEnabledSave();
        }
        #endregion

        #region Buchung Abgang(OutwardMovementUnscheduled)
        /// <summary>
        /// Outwards the facility movement.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Post Outward Movement'}de{'Buche Lagerabgang'}", 704, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void OutwardFacilityMovement()
        {
            if (!PreExecute()) return;
            if (CurrentFacility.Partslist != null)
            {
                CurrentBookParamOutwardMovement.OutwardPartslist = CurrentFacility.Partslist;
                CurrentBookParamOutwardMovement.OutwardMaterial = CurrentFacility.Material;
            }

            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamOutwardMovement, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamOutwardMovement.ValidMessage.IsSucceded() || CurrentBookParamOutwardMovement.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamOutwardMovement.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacility));
                OnPropertyChanged(nameof(FacilityChargeList));
            }
            PostExecute();
        }
        /// <summary>
        /// Determines whether [is enabled outward facility movement].
        /// </summary>
        /// <returns><c>true</c> if [is enabled outward facility movement]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledOutwardFacilityMovement()
        {
            bool bRetVal = CurrentBookParamOutwardMovement.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }
        #endregion

        #region Umlagerung (Relocation)
        /// <summary>
        /// Facilities the relocation.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Post Stock Transfer'}de{'Buche Umlagerung'}", 705, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void FacilityRelocation()
        {
            if (!PreExecute()) return;

            if (IsPhysicalTransportPossible)
            {
                Global.MsgResult userQuestionAutomatic = Global.MsgResult.No;
                // Question50035: Do you want to run this relocation/transport on the plant in automatic mode?
                userQuestionAutomatic = Messages.YesNoCancel(this, "Question50035");
                if (userQuestionAutomatic == Global.MsgResult.Yes)
                {
                    gip.core.datamodel.ACClassMethod acClassMethod = null;
                    bool wfRunsBatches = false;
                    if (!PrepareStartWorkflow(CurrentBookParamRelocation, out acClassMethod, out wfRunsBatches))
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
                            Messages.Msg(msgDetails);
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
                        Save();
                        msgDetails = ACPickingManager.ValidateStart(this.DatabaseApp, this.DatabaseApp.ContextIPlus, picking, null, PARole.ValidationBehaviour.Strict);
                        if (msgDetails != null && msgDetails.MsgDetailsCount > 0)
                        {
                            Messages.Msg(msgDetails);
                            ClearBookingData();
                            return;
                        }
                        StartWorkflow(acClassMethod, picking);
                    }

                }
                else if (userQuestionAutomatic == Global.MsgResult.No)
                    BookRelocation();
            }
            else
                BookRelocation();

            PostExecute();
        }
        /// <summary>
        /// Determines whether [is enabled facility relocation].
        /// </summary>
        /// <returns><c>true</c> if [is enabled facility relocation]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledFacilityRelocation()
        {
            CurrentBookParamRelocation.InwardQuantity = CurrentBookParamRelocation.OutwardQuantity;
            bool bRetVal = CurrentBookParamRelocation.IsEnabled();
            UpdateBSOMsg();
            return bRetVal;
        }

        protected override bool BookRelocation()
        {
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamRelocation, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamRelocation.ValidMessage.IsSucceded() || CurrentBookParamRelocation.ValidMessage.HasWarnings())
            {
                Messages.Msg(CurrentBookParamRelocation.ValidMessage);
                return false;
            }
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
                return false;
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacility));
                OnPropertyChanged(nameof(FacilityChargeList));
            }
            return true;
        }


        public override bool IsPhysicalTransportPossible
        {
            get
            {
                return
                    ((CurrentBookParamRelocation != null
                        && CurrentBookParamRelocation.OutwardFacility != null
                        && CurrentBookParamRelocation.OutwardFacility.VBiFacilityACClassID.HasValue
                        && CurrentBookParamRelocation.InwardFacility != null
                        && CurrentBookParamRelocation.InwardFacility.VBiFacilityACClassID.HasValue)
                    || (CurrentBookParamInwardMovement != null
                        && CurrentBookParamInwardMovement.InwardFacility != null
                        && CurrentBookParamInwardMovement.InwardFacility.VBiFacilityACClassID.HasValue)
                  )
                  && HasRightsForPhysicalTransport;
            }
        }

        #endregion

        #region Freigabestatus (ReleaseStateFacility, ReleaseStateMaterial)
        /// <summary>
        /// Locks the facility.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Lock'}de{'Sperren'}", 706, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void LockFacility()
        {
            if (!PreExecute("LockFacility")) return;
            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Locked);
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReleaseAndLock, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReleaseAndLock.ValidMessage.IsSucceded() || CurrentBookParamReleaseAndLock.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamReleaseAndLock.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged("CurrentFacility");
                OnPropertyChanged("FacilityChargeList");
            }
            PostExecute("LockFacility");
        }
        /// <summary>
        /// Determines whether [is enabled lock facility].
        /// </summary>
        /// <returns><c>true</c> if [is enabled lock facility]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLockFacility()
        {
            if (CurrentFacility == null)
                return false;
            if (CurrentFacility.CurrentFacilityStock == null)
                return false;
            if (CurrentFacility.CurrentFacilityStock.MDReleaseState != null)
            {
                if (CurrentFacility.CurrentFacilityStock.MDReleaseState.MDReleaseStateIndex == (int)MDReleaseState.ReleaseStates.Locked)
                    return false;
            }

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Locked);
            bool bRetVal = CurrentBookParamReleaseAndLock.IsEnabled();
            CurrentBookParamReleaseAndLock.MDReleaseState = null;
            UpdateBSOMsg();
            return bRetVal;
        }

        /// <summary>
        /// Locks the facility absolute.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Lock Absolute'}de{'Absolut sperren'}", 707, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void LockFacilityAbsolute()
        {
            if (!PreExecute("LockFacilityAbsolute")) return;
            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsLocked);
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReleaseAndLock, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReleaseAndLock.ValidMessage.IsSucceded() || CurrentBookParamReleaseAndLock.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamReleaseAndLock.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged("CurrentFacility");
                OnPropertyChanged("FacilityChargeList");
            }
            PostExecute("LockFacilityAbsolute");
        }

        /// <summary>
        /// Determines whether [is enabled lock facility absolute].
        /// </summary>
        /// <returns><c>true</c> if [is enabled lock facility absolute]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLockFacilityAbsolute()
        {
            if (CurrentFacility == null)
                return false;
            if (CurrentFacility.CurrentFacilityStock == null)
                return false;
            if (CurrentFacility.CurrentFacilityStock.MDReleaseState != null)
            {
                if (CurrentFacility.CurrentFacilityStock.MDReleaseState.MDReleaseStateIndex == (int)MDReleaseState.ReleaseStates.AbsLocked)
                    return false;
            }

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsLocked);
            bool isEnabled = CurrentBookParamReleaseAndLock.IsEnabled();
            CurrentBookParamReleaseAndLock.MDReleaseState = null;
            UpdateBSOMsg();
            return isEnabled;
        }

        /// <summary>
        /// Releases the facility.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Unlock'}de{'Freigeben'}", 708, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void ReleaseFacility()
        {
            if (!PreExecute("ReleaseFacility")) return;
            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Free);
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReleaseAndLock, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReleaseAndLock.ValidMessage.IsSucceded() || CurrentBookParamReleaseAndLock.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamReleaseAndLock.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged("CurrentFacility");
                OnPropertyChanged("FacilityChargeList");
            }
            PostExecute("ReleaseFacility");
        }

        /// <summary>
        /// Determines whether [is enabled release facility].
        /// </summary>
        /// <returns><c>true</c> if [is enabled release facility]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledReleaseFacility()
        {
            if (CurrentFacility == null)
                return false;
            if (CurrentFacility.CurrentFacilityStock == null)
                return false;
            if (CurrentFacility.CurrentFacilityStock.MDReleaseState != null)
            {
                if (CurrentFacility.CurrentFacilityStock.MDReleaseState.MDReleaseStateIndex == (int)MDReleaseState.ReleaseStates.Free)
                    return false;
            }

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.Free);
            bool isEnabled = CurrentBookParamReleaseAndLock.IsEnabled();
            CurrentBookParamReleaseAndLock.MDReleaseState = null;
            UpdateBSOMsg();
            return isEnabled;
        }

        /// <summary>
        /// Releases the facility absolute.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Release Absolute'}de{'Absolut freigeben'}", 709, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void ReleaseFacilityAbsolute()
        {
            if (!PreExecute("ReleaseFacilityAbsolute")) return;
            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsFree);
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReleaseAndLock, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReleaseAndLock.ValidMessage.IsSucceded() || CurrentBookParamReleaseAndLock.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamReleaseAndLock.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged("CurrentFacility");
                OnPropertyChanged("FacilityChargeList");
            }
            PostExecute("ReleaseFacilityAbsolute");
        }
        /// <summary>
        /// Determines whether [is enabled release facility absolute].
        /// </summary>
        /// <returns><c>true</c> if [is enabled release facility absolute]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledReleaseFacilityAbsolute()
        {
            if (CurrentFacility == null)
                return false;
            if (CurrentFacility.CurrentFacilityStock == null)
                return false;
            if (CurrentFacility.CurrentFacilityStock.MDReleaseState != null)
            {
                if (CurrentFacility.CurrentFacilityStock.MDReleaseState.MDReleaseStateIndex == (int)MDReleaseState.ReleaseStates.AbsFree)
                    return false;
            }

            CurrentBookParamReleaseAndLock.MDReleaseState = MDReleaseState.DefaultMDReleaseState(DatabaseApp, MDReleaseState.ReleaseStates.AbsFree);
            bool isEnabled = CurrentBookParamReleaseAndLock.IsEnabled();
            CurrentBookParamReleaseAndLock.MDReleaseState = null;
            UpdateBSOMsg();
            return isEnabled;
        }
        #endregion

        #region Nicht verfügbar (NotAvailable, Available, NotAvailableMaterial, AvailableMaterial)
        /// <summary>
        /// Nots the available facility.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Silo empty'}de{'Silo leer'}", 710, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void NotAvailableFacility()
        {
            if (!PreExecute()) return;
            CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamNotAvailable, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamNotAvailable.ValidMessage.IsSucceded() || CurrentBookParamNotAvailable.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamNotAvailable.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacility));
                OnPropertyChanged(nameof(FacilityChargeList));
            }
            PostExecute();
        }

        /// <summary>
        /// Determines whether [is enabled not available facility].
        /// </summary>
        /// <returns><c>true</c> if [is enabled not available facility]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNotAvailableFacility()
        {
            if (CurrentFacility == null)
                return false;
            bool isEnabled = !CurrentFacility.NotAvailable;
            if (isEnabled)
            {
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
                isEnabled = CurrentBookParamNotAvailable.IsEnabled();
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.Off);
                UpdateBSOMsg();
            }
            return isEnabled;
        }

        /// <summary>
        /// Availables the facility.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Silo not empty'}de{'Silo nicht leer'}", 711, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void AvailableFacility()
        {
            if (!PreExecute()) return;

            //Question50097: Do you want to restore a last stock?
            if (Messages.Question(this, "Question50098") == Global.MsgResult.Yes)
            {
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.RestoreQuantityIfNotAvailable);
            }
            else
            {
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.ResetIfNotAvailableFacility);
            }
            
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamNotAvailable, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamNotAvailable.ValidMessage.IsSucceded() || CurrentBookParamNotAvailable.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamNotAvailable.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacility));
                OnPropertyChanged(nameof(FacilityChargeList));
            }
            PostExecute();
        }

        /// <summary>
        /// Determines whether [is enabled available facility].
        /// </summary>
        /// <returns><c>true</c> if [is enabled available facility]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAvailableFacility()
        {
            if (CurrentFacility == null)
                return false;
            bool isEnabled = CurrentFacility.NotAvailable;
            if (isEnabled)
            {
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.ResetIfNotAvailableFacility);
                isEnabled = CurrentBookParamNotAvailable.IsEnabled();
                CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.Off);
                UpdateBSOMsg();
            }
            return isEnabled;
        }

        //[ACMethodCommand(Facility.ClassName, "en{'Restore last quants'}de{'Letzte Quanten wiederherstellen'}", 711, true, Global.ACKinds.MSMethodPrePost)]
        //public virtual void AvailableLastQuants()
        //{
        //    CurrentBookParamNotAvailable.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp, MDZeroStockState.ZeroStockStates.ResetIfNotAvailableFacility);

        //    ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamNotAvailable, this.DatabaseApp) as ACMethodEventArgs;

        //    ClearBookingData();

        //    //FacilityBooking fb = DatabaseApp.FacilityBooking.Include(c => c.FacilityBookingCharge_FacilityBooking)
        //    //                                                .Where(c => c.InwardFacilityID == CurrentFacility.FacilityID
        //    //                                                         && c.FacilityBookingTypeIndex == (short)GlobalApp.FacilityBookingType.ZeroStock_Facility_BulkMaterial)
        //    //                                                .OrderByDescending(c => c.InsertDate)
        //    //                                                .FirstOrDefault();
        //    //if (fb == null)
        //    //    return;

        //    //FacilityBookingCharge[] charges = fb.FacilityBookingCharge_FacilityBooking.ToArray();

        //    //bool bookLastQuantity = false;

        //    ////Question50097: Do you want to restore a last stock?
        //    //if (Messages.Question(this, "Question50097") == Global.MsgResult.Yes)
        //    //{
        //    //    bookLastQuantity = true;
        //    //}

        //    //foreach (FacilityBookingCharge fbc in charges)
        //    //{
        //    //    FacilityCharge fc = fbc.InwardFacilityCharge;
        //    //    if (fc == null)
        //    //        continue;

        //    //    if (!bookLastQuantity)
        //    //    {
        //    //        fc.NotAvailable = false;
        //    //    }
        //    //    else
        //    //    {
        //    //        ClearBookingData();

        //    //        CurrentBookParamInwardMovement.InwardFacilityCharge = fc;
        //    //        CurrentBookParamInwardMovement.InwardQuantity = fbc.InwardQuantity * -1;

        //    //        BookInwardFacilityMovement();
        //    //    }
        //    //}

        //    //Save();

        //    OnPropertyChanged(nameof(CurrentFacility));
        //    OnPropertyChanged(nameof(FacilityChargeList));
        //}

        //public bool IsEnabledAvailableLastQuants()
        //{
        //    if (CurrentFacility == null)
        //        return false;
        //    bool isEnabled = CurrentFacility.NotAvailable;

        //    return isEnabled;
        //}

        #endregion

        #region Abgleich Lagerplatz
        /// <summary>
        /// Starts the facility adjust.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Adjust Storage Bin'}de{'Abgleich Lagerplatz'}", 712, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void StartFacilityAdjust()
        {
            // Starte Hintergrundthread
            if (!PreExecute()) return;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamMatching, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamMatching.ValidMessage.IsSucceded() || CurrentBookParamMatching.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamMatching.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacility));
                OnPropertyChanged(nameof(FacilityChargeList));
            }
            PostExecute();
        }

        /// <summary>
        /// Determines whether [is enabled start facility adjust].
        /// </summary>
        /// <returns><c>true</c> if [is enabled start facility adjust]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledStartFacilityAdjust()
        {
            if (CurrentFacility == null)
                return false;
            return CurrentBookParamMatching.IsEnabled();
        }

        #endregion

        #region Materialneuzuordnung (Reassignment)
        /// <summary>
        /// Facilities the relocation.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Reassign Material'}de{'Material neu zuordnen'}", 713, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void FacilityReassign()
        {
            if (!PreExecute()) return;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentBookParamReassignMat, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentBookParamReassignMat.ValidMessage.IsSucceded() || CurrentBookParamReassignMat.ValidMessage.HasWarnings())
                Messages.Msg(CurrentBookParamReassignMat.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
            }
            else
            {
                ClearBookingData();
                OnPropertyChanged(nameof(CurrentFacility));
                OnPropertyChanged(nameof(FacilityChargeList));
            }
            PostExecute();
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

        /// <summary>
        /// Updates the BSO MSG.
        /// </summary>
        private void UpdateBSOMsg()
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

        #region Dialog
        public bool DialogResult
        {
            get;
            set;
        }

        [ACMethodInfo("Dialog", "en{'Dialog Facility'}de{'Dialog Zelle'}", (short)MISort.QueryPrintDlg)]
        public void ShowDialogFacility(string facilityNo)
        {
            if (AccessPrimary == null)
                return;
            //AccessPrimary.NavACQueryDefinition.SearchWord = facilityNo;
            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "FacilityNo").FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, nameof(Facility.FacilityNo), Global.LogicalOperators.contains, Global.Operators.and, facilityNo, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            else
                filterItem.SearchWord = facilityNo;
            
            this.Search();
            ShowDialog(this, "FacilityBookDialog");
            this.ParentACComponent.StopComponent(this);
        }

        [ACMethodCommand("Dialog", "en{'OK'}de{'OK'}", (short)MISort.Okay)]
        public void DialogOK()
        {
            DialogResult = true;
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", "en{'Cancel'}de{'Abbrechen'}", (short)MISort.Cancel)]
        public void DialogCancel()
        {
            DialogResult = false;
            CloseTopDialog();
        }
        #endregion


        #endregion

        #region Eventhandling
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
        [ACMethodInfo(Facility.ClassName, "en{'Activate'}de{'Aktivieren'}", 9999, true, Global.ACKinds.MSMethodPrePost)]
        public void OnActivate(string page)
        {
            if (!PreExecute())
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
            PostExecute();
        }

        /// <summary>
        /// BSOs the facility book cell_ on value changed.
        /// </summary>
        /// <param name="dataContent">Content of the data.</param>
        void BSOFacilityBookCell_OnValueChanged(string dataContent)
        {
            if (dataContent == nameof(CurrentFacility) + "." + nameof(CurrentFacility.Material))
            {
                //CurrentFacility.Partslist = null;
                //OnPropertyChanged("PartslistList");
                //OnPropertyChanged("FacilityLotList");
            }
        }

        bool? _CheckIsDischargingActiveToFacility = null;
        /// <summary>
        /// Gets a value indicating whether this instance is enabled material.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled material; otherwise, <c>false</c>.</value>
        public bool IsEnabledMaterial
        {
            get
            {
                if (FacilityChargeList == null)
                    return true;
                if (FacilityChargeList.Any())
                    return false;
                CheckDischarging();
                return !_CheckIsDischargingActiveToFacility.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is enabled partslist.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled partslist; otherwise, <c>false</c>.</value>
        public bool IsEnabledPartslist
        {
            get
            {
                if (CurrentFacility == null)
                    return false;
                if (CurrentFacility.Material == null)
                    return false;
                CheckDischarging();
                return !_CheckIsDischargingActiveToFacility.Value;
            }
        }

        private void CheckDischarging()
        {
            if (!_CheckIsDischargingActiveToFacility.HasValue)
            {
                _CheckIsDischargingActiveToFacility = false;
                if (CurrentFacility != null && CurrentFacility.FacilityACClass != null)
                {
                    string url = CurrentFacility.FacilityACClass.GetACUrlComponent();
                    if (!String.IsNullOrEmpty(url))
                    {
                        object result = ACUrlCommand(url + "!" + nameof(mes.processapplication.PAMSilo.IsDischargingToThisSiloActive), null);
                        if (   result != null 
                            && result is string[]
                            && (result as string[]).Any())
                        {
                            _CheckIsDischargingActiveToFacility = true;
                        }
                    }
                }
            }
        }

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
                case nameof(Search):
                    Search();
                    return true;
                case nameof(ClearBookingData):
                    ClearBookingData();
                    return true;
                case nameof(IsEnabledClearBookingData):
                    result = IsEnabledClearBookingData();
                    return true;
                case nameof(InwardFacilityMovement):
                    InwardFacilityMovement();
                    return true;
                case nameof(IsEnabledInwardFacilityMovement):
                    result = IsEnabledInwardFacilityMovement();
                    return true;
                case nameof(InwardFacilityLotGenerateDlg):
                    InwardFacilityLotGenerateDlg();
                    return true;
                case nameof(OutwardFacilityMovement):
                    OutwardFacilityMovement();
                    return true;
                case nameof(IsEnabledOutwardFacilityMovement):
                    result = IsEnabledOutwardFacilityMovement();
                    return true;
                case nameof(FacilityRelocation):
                    FacilityRelocation();
                    return true;
                case nameof(IsEnabledFacilityRelocation):
                    result = IsEnabledFacilityRelocation();
                    return true;
                case nameof(LockFacility):
                    LockFacility();
                    return true;
                case nameof(IsEnabledLockFacility):
                    result = IsEnabledLockFacility();
                    return true;
                case nameof(LockFacilityAbsolute):
                    LockFacilityAbsolute();
                    return true;
                case nameof(IsEnabledLockFacilityAbsolute):
                    result = IsEnabledLockFacilityAbsolute();
                    return true;
                case nameof(ReleaseFacility):
                    ReleaseFacility();
                    return true;
                case nameof(IsEnabledReleaseFacility):
                    result = IsEnabledReleaseFacility();
                    return true;
                case nameof(ReleaseFacilityAbsolute):
                    ReleaseFacilityAbsolute();
                    return true;
                case nameof(IsEnabledReleaseFacilityAbsolute):
                    result = IsEnabledReleaseFacilityAbsolute();
                    return true;
                case nameof(NotAvailableFacility):
                    NotAvailableFacility();
                    return true;
                case nameof(IsEnabledNotAvailableFacility):
                    result = IsEnabledNotAvailableFacility();
                    return true;
                case nameof(AvailableFacility):
                    AvailableFacility();
                    return true;
                case nameof(IsEnabledAvailableFacility):
                    result = IsEnabledAvailableFacility();
                    return true;
                case nameof(StartFacilityAdjust):
                    StartFacilityAdjust();
                    return true;
                case nameof(IsEnabledStartFacilityAdjust):
                    result = IsEnabledStartFacilityAdjust();
                    return true;
                case nameof(FacilityReassign):
                    FacilityReassign();
                    return true;
                case nameof(IsEnabledFacilityReassign):
                    result = IsEnabledFacilityReassign();
                    return true;
                case nameof(ShowDialogFacility):
                    ShowDialogFacility((String)acParameter[0]);
                    return true;
                case nameof(DialogOK):
                    DialogOK();
                    return true;
                case nameof(DialogCancel):
                    DialogCancel();
                    return true;
                case nameof(OnActivate):
                    OnActivate((String)acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }

}
