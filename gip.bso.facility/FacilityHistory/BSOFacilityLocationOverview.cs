// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOFacilityLocationOverview.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Data.Objects;
using gip.mes.facility;

namespace gip.bso.facility
{
    /// <summary>
    /// Folgende alte Masken sind in diesem BSO enthalten:
    /// 1. Bestandsübersicht von Chargen/Artikeln
    /// 1.1 Artikel Gesamtbestände
    /// 1.2 Artikel Lagerortbestände
    /// 1.3 Artikel Lagerplatzbestände
    /// 1.4 Chargen Gesamtbestände
    /// 1.5 Chargen Lagerortbestände
    /// 1.6 Chargen Lagerplatzbestände
    /// 1.7 Artikelchargen Gesamtbestände
    /// 1.8 Artikelchargen Lagerortbestände
    /// 1.9 Artikelchargen Lagerplatzbestände
    /// Neue Masken:
    /// 1. Bestandsübersicht
    /// ALLE Lagerbuchungen erfolgen immer nur über den FacilityBookingManager.
    /// Dieser ist auch in anderen buchenden Anwendungen zu verwenden.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Storage Location Overview'}de{'Lagerort Übersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Facility.ClassName)]
    public class BSOFacilityLocationOverview : BSOFacilityOverviewBase
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityLocationOverview"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityLocationOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
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
            this._CurrentFacilityCharge = null;
            this._CurrentFacilityChargeSumLotHelper = null;
            this._CurrentFacilityChargeSumMaterialHelper = null;
            this._SelectedFacilityCharge = null;
            this._SelectedFacilityChargeSumLotHelper = null;
            this._SelectedFacilityChargeSumMaterialHelper = null;

            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region BSO->ACProperty

        #region Primary
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<Facility> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(890, "FacilityLocation")]
        public ACAccessNav<Facility> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    navACQueryDefinition.CheckAndReplaceFilterColumnsIfDifferent(NavigationqueryDefaultFilter);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Facility>("FacilityLocation", this);
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
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityNo", Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityName", Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)FacilityTypesEnum.StorageLocation).ToString(), true),
                };
            }
        }


        /// <summary>
        /// Gets or sets the selected facility location.
        /// </summary>
        /// <value>The selected facility location.</value>
        [ACPropertySelected(801, "FacilityLocation")]
        public Facility SelectedFacilityLocation
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
                OnPropertyChanged("SelectedFacilityLocation");
            }
        }

        /// <summary>
        /// Gets or sets the current facility location.
        /// </summary>
        /// <value>The current facility location.</value>
        [ACPropertyCurrent(802, "FacilityLocation")]
        public Facility CurrentFacilityLocation
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
                OnPropertyChanged("CurrentFacilityLocation");
                CleanMovements();
                RefreshRelatedData();
            }
        }

        public override void OnPropertyChanged(string name)
        {
            base.OnPropertyChanged(name);
            if (name == "ShowNotAvailable")
            {
                RefreshRelatedData();
            }
        }

        public void RefreshRelatedData()
        {
            RefreshFacilityChargeList();
            RefreshFacilityChargeSumMaterialHelperList();
            RefreshFacilityChargeSumLotHelperList();
        }

        /// <summary>
        /// Gets the facility location list.
        /// </summary>
        /// <value>The facility location list.</value>
        [ACPropertyList(803, "FacilityLocation")]
        public IEnumerable<Facility> FacilityLocationList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region FacilityCharge


        /// <summary>
        /// The _ current facility charge
        /// </summary>
        FacilityCharge _CurrentFacilityCharge;
        /// <summary>
        /// Gets or sets the current facility charge.
        /// </summary>
        /// <value>The current facility charge.</value>
        [ACPropertyCurrent(804, FacilityCharge.ClassName)]
        public FacilityCharge CurrentFacilityCharge
        {
            get
            {
                return _CurrentFacilityCharge;
            }
            set
            {
                _CurrentFacilityCharge = value;
                OnPropertyChanged("CurrentFacilityCharge");
            }
        }

        IEnumerable<FacilityCharge> _FacilityChargeList;
        /// <summary>
        /// Gets the facility charge list.
        /// </summary>
        /// <value>The facility charge list.</value>
        [ACPropertyList(805, FacilityCharge.ClassName)]
        public IEnumerable<FacilityCharge> FacilityChargeList
        {
            get
            {
                if (_FacilityChargeList != null)
                    return _FacilityChargeList;
                if (CurrentFacilityLocation == null)
                    return null;
                _FacilityChargeList = FacilityManager.s_cQry_LocationOverviewFacilityCharge(this.DatabaseApp, CurrentFacilityLocation.FacilityID, ShowNotAvailable).ToArray();
                return _FacilityChargeList;
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
        [ACPropertySelected(806, FacilityCharge.ClassName)]
        public FacilityCharge SelectedFacilityCharge
        {
            get
            {
                return _SelectedFacilityCharge;
            }
            set
            {
                _SelectedFacilityCharge = value;
                OnPropertyChanged("SelectedFacilityCharge");
            }
        }

        private void RefreshFacilityChargeList()
        {
            CurrentFacilityCharge = null;
            SelectedFacilityCharge = null;
            _FacilityChargeList = null;
            OnPropertyChanged("FacilityChargeList");
        }

        #endregion

        #region SumHelper
        /// <summary>
        /// The _ current facility charge sum material helper
        /// </summary>
        FacilityChargeSumMaterialHelper _CurrentFacilityChargeSumMaterialHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum material helper.
        /// </summary>
        /// <value>The current facility charge sum material helper.</value>
        [ACPropertyCurrent(807, "FacilityChargeSumMaterialHelper")]
        public FacilityChargeSumMaterialHelper CurrentFacilityChargeSumMaterialHelper
        {
            get
            {
                return _CurrentFacilityChargeSumMaterialHelper;
            }
            set
            {
                _CurrentFacilityChargeSumMaterialHelper = value;
                OnPropertyChanged("CurrentFacilityChargeSumMaterialHelper");
            }
        }

        /// <summary>
        /// Gets the facility charge sum material helper list.
        /// </summary>
        /// <value>The facility charge sum material helper list.</value>
        [ACPropertyList(808, "FacilityChargeSumMaterialHelper")]
        public IEnumerable<FacilityChargeSumMaterialHelper> FacilityChargeSumMaterialHelperList
        {
            get
            {
                if (CurrentFacilityLocation == null)
                    return null;
                return ACFacilityManager.GetFacilityChargeSumMaterialHelperList(FacilityChargeList, new FacilityQueryFilter() { FacilityLocationID = CurrentFacilityLocation.FacilityID });
            }
        }
        /// <summary>
        /// The _ selected facility charge sum material helper
        /// </summary>
        FacilityChargeSumMaterialHelper _SelectedFacilityChargeSumMaterialHelper;
        /// <summary>
        /// Gets or sets the selected facility charge sum material helper.
        /// </summary>
        /// <value>The selected facility charge sum material helper.</value>
        [ACPropertySelected(809, "FacilityChargeSumMaterialHelper")]
        public FacilityChargeSumMaterialHelper SelectedFacilityChargeSumMaterialHelper
        {
            get
            {
                return _SelectedFacilityChargeSumMaterialHelper;
            }
            set
            {
                _SelectedFacilityChargeSumMaterialHelper = value;
                OnPropertyChanged("SelectedFacilityChargeSumMaterialHelper");
            }
        }

        private void RefreshFacilityChargeSumMaterialHelperList()
        {
            CurrentFacilityChargeSumMaterialHelper = null;
            SelectedFacilityChargeSumMaterialHelper = null;
            OnPropertyChanged("FacilityChargeSumMaterialHelperList");
        }


        /// <summary>
        /// The _ current facility charge sum lot helper
        /// </summary>
        FacilityChargeSumLotHelper _CurrentFacilityChargeSumLotHelper;
        /// <summary>
        /// Gets or sets the current facility charge sum lot helper.
        /// </summary>
        /// <value>The current facility charge sum lot helper.</value>
        [ACPropertyCurrent(810, "FacilityChargeSumLotHelper")]
        public FacilityChargeSumLotHelper CurrentFacilityChargeSumLotHelper
        {
            get
            {
                return _CurrentFacilityChargeSumLotHelper;
            }
            set
            {
                _CurrentFacilityChargeSumLotHelper = value;
                OnPropertyChanged("CurrentFacilityChargeSumLotHelper");
            }
        }

        /// <summary>
        /// Gets the facility charge sum lot helper list.
        /// </summary>
        /// <value>The facility charge sum lot helper list.</value>
        [ACPropertyList(811, "FacilityChargeSumLotHelper")]
        public IEnumerable<FacilityChargeSumLotHelper> FacilityChargeSumLotHelperList
        {
            get
            {
                if (CurrentFacilityLocation == null)
                    return null;
                return ACFacilityManager.GetFacilityChargeSumLotHelperList(FacilityChargeList, new FacilityQueryFilter() { FacilityLocationID = CurrentFacilityLocation.FacilityID });
            }
        }
        /// <summary>
        /// The _ selected facility charge sum lot helper
        /// </summary>
        FacilityChargeSumLotHelper _SelectedFacilityChargeSumLotHelper;
        /// <summary>
        /// Gets or sets the selected facility charge sum lot helper.
        /// </summary>
        /// <value>The selected facility charge sum lot helper.</value>
        [ACPropertySelected(812, "FacilityChargeSumLotHelper")]
        public FacilityChargeSumLotHelper SelectedFacilityChargeSumLotHelper
        {
            get
            {
                return _SelectedFacilityChargeSumLotHelper;
            }
            set
            {
                _SelectedFacilityChargeSumLotHelper = value;
                OnPropertyChanged("SelectedFacilityChargeSumLotHelper");
            }
        }

        private void RefreshFacilityChargeSumLotHelperList()
        {
            CurrentFacilityChargeSumLotHelper = null;
            SelectedFacilityChargeSumLotHelper = null;
            OnPropertyChanged("FacilityChargeSumLotHelperList");
        }

        #endregion
        
        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
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
        [ACMethodInteraction(Facility.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacilityLocation")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<Facility>(requery, () => SelectedFacilityLocation, () => CurrentFacilityLocation, c => CurrentFacilityLocation = c,
                        DatabaseApp.Facility
                        .Include("FacilityStock_Facility")
                        .Where(c => c.ParentFacilityID != null && c.FacilityID == SelectedFacilityLocation.FacilityID));
            if (CurrentFacilityLocation != null && CurrentFacilityLocation.CurrentFacilityStock != null)
                CurrentFacilityLocation.CurrentFacilityStock.AutoRefresh(DatabaseApp);
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return true;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("FacilityLocationList");
        }

        IQueryable<Facility> _AccessPrimary_NavSearchExecuting(IQueryable<Facility> result)
        {
            ObjectQuery<Facility> query = result as ObjectQuery<Facility>;
            if (query != null)
            {
                query.Include("FacilityStock_Facility");
            }
            return result;
        }

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"Save":
                    Save();
                    return true;
                case"IsEnabledSave":
                    result = IsEnabledSave();
                    return true;
                case"UndoSave":
                    UndoSave();
                    return true;
                case"IsEnabledUndoSave":
                    result = IsEnabledUndoSave();
                    return true;
                case"Load":
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case"IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
                case"Search":
                    Search();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

        #region FacilityBooking(Charge)Overview methods -> Executive methods overrides

        public override bool IsEnabledRefreshMovements()
        {
            return base.IsEnabledRefreshMovements() && CurrentFacilityLocation != null;
        }

        public override FacilityQueryFilter GetFacilityBookingFilter()
        {
            FacilityQueryFilter filter = base.GetFacilityBookingFilter();
            if (CurrentFacilityLocation != null)
                filter.FacilityLocationID = CurrentFacilityLocation.FacilityID;
            return filter;
        }
        #endregion
    }
}
