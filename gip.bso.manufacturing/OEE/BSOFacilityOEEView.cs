// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOFacilityOEEView.cs" company="gip mbh, Oftersheim, Germany">
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
using System.ComponentModel;
using gip.mes.facility;
using System.Runtime.CompilerServices;
using gip.bso.facility;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'OEE Dashboard'}de{'OEE Übersichtsseite'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Facility.ClassName)]
    public class BSOFacilityOEEView : BSOFacilityOverviewBase
    {
        #region Constants
        #endregion

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityOEEView"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityOEEView(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            _FacilityOEEManager = ACFacilityOEEManager.ACRefToServiceInstance(this);
            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;

            ACFacilityOEEManager.DetachACRefFromServiceInstance(this, _FacilityOEEManager);
            _FacilityOEEManager = null;

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


        #region Managers

        protected ACRef<ACFacilityOEEManager> _FacilityOEEManager = null;
        public ACFacilityOEEManager FacilityOEEManager
        {
            get
            {
                if (_FacilityOEEManager == null)
                    return null;
                return _FacilityOEEManager.ValueT;
            }
        }

        #endregion

        #region Access-Primary
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<Facility> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(890, Facility.ClassName)]
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
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityNo", Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.filter, "FacilityName", Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)FacilityTypesEnum.MachineOrInventory).ToString(), true),
                };
            }
        }

        /// <summary>
        /// Gets or sets the selected facility.
        /// </summary>
        /// <value>The selected facility.</value>
        [ACPropertySelected(801, Facility.ClassName)]
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
                bool changed = AccessPrimary.Selected != value;
                AccessPrimary.Selected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current facility.
        /// </summary>
        /// <value>The current facility.</value>
        [ACPropertyCurrent(802, Facility.ClassName)]
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

                bool changed = AccessPrimary.Current != value;
                AccessPrimary.Current = value;
                OnPropertyChanged();
                if (changed)
                    AutoSetPeriodsAndRefresh();
                CleanMovements();
                RefreshRelatedData();
            }
        }

        public override void OnPropertyChanged([CallerMemberName] string name = "")
        {
            base.OnPropertyChanged(name);
            if (name == "ShowNotAvailable")
            {
                RefreshRelatedData();
            }
        }


        public void RefreshRelatedData()
        {
        }

        /// <summary>
        /// Gets the facility list.
        /// </summary>
        /// <value>The facility list.</value>
        [ACPropertyList(803, Facility.ClassName)]
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

        #region OEE-Data Sum

        private static FacilityOEEAvg _NullOEEAvg = new FacilityOEEAvg() { AvailabilityOEE = 100, QualityOEE = 100, PerformanceOEE = 100, TotalOEE = 100 };
        FacilityOEEAvg _OEEAvgPeriod1 = _NullOEEAvg;
        [ACPropertyInfo(400, "", "en{'OEE Period 1'}de{'OEE Periode 1'}")]
        public FacilityOEEAvg OEEAvgPeriod1
        {
            get
            {
                return _OEEAvgPeriod1;
            }
            set
            {
                _OEEAvgPeriod1 = value;
                OnPropertyChanged();
            }
        }

        DateTime? _Period1From;
        [ACPropertyInfo(401, "", "en{'From'}de{'Von'}")]
        public DateTime? Period1From
        {
            get
            {
                return _Period1From;
            }
            set
            {
                _Period1From = value; 
                OnPropertyChanged();
            }
        }

        DateTime? _Period1To;
        [ACPropertyInfo(402, "", "en{'To'}de{'Bis'}")]
        public DateTime? Period1To
        {
            get
            {
                return _Period1To;
            }
            set
            {
                _Period1To = value;
                OnPropertyChanged();
            }
        }

        FacilityOEEAvg _OEEAvgPeriod2 = _NullOEEAvg;
        [ACPropertyInfo(400, "", "en{'OEE Period 2'}de{'OEE Periode 2'}")]
        public FacilityOEEAvg OEEAvgPeriod2
        {
            get
            {
                return _OEEAvgPeriod2;
            }
            set
            {
                _OEEAvgPeriod2 = value;
                OnPropertyChanged();
            }
        }

        DateTime? _Period2From;
        [ACPropertyInfo(402, "", "en{'From'}de{'Von'}")]
        public DateTime? Period2From
        {
            get
            {
                return _Period2From;
            }
            set
            {
                _Period2From = value;
                OnPropertyChanged();
            }
        }

        DateTime? _Period2To;
        [ACPropertyInfo(402, "", "en{'To'}de{'Bis'}")]
        public DateTime? Period2To
        {
            get
            {
                return _Period2To;
            }
            set
            {
                _Period2To = value;
                OnPropertyChanged();
            }
        }

        FacilityOEEAvg _OEEAvgPeriod3 = _NullOEEAvg;
        [ACPropertyInfo(400, "", "en{'OEE Period 3'}de{'OEE Periode 3'}")]
        public FacilityOEEAvg OEEAvgPeriod3
        {
            get
            {
                return _OEEAvgPeriod3;
            }
            set
            {
                _OEEAvgPeriod3 = value;
                OnPropertyChanged();
            }
        }


        DateTime? _Period3From;
        [ACPropertyInfo(403, "", "en{'From'}de{'Von'}")]
        public DateTime? Period3From
        {
            get
            {
                return _Period3From;
            }
            set
            {
                _Period3From = value;
                OnPropertyChanged();
            }
        }

        DateTime? _Period3To;
        [ACPropertyInfo(402, "", "en{'To'}de{'Bis'}")]
        public DateTime? Period3To
        {
            get
            {
                return _Period3To;
            }
            set
            {
                _Period3To = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region BSO->ACMethod

        #region Saving
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
        [ACMethodInteraction(Facility.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacility")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<Facility>(requery, () => SelectedFacility, () => CurrentFacility, c => CurrentFacility = c,
                        DatabaseApp.Facility
                        .Where(c => c.FacilityID == SelectedFacility.FacilityID));
            PostExecute("Load");
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Facility.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public virtual void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("FacilityList");
        }

        IQueryable<Facility> _AccessPrimary_NavSearchExecuting(IQueryable<Facility> result)
        {
            return result;
        }
        #endregion

        #region OEE-Calculation
        public void AutoSetPeriodsAndRefresh()
        {
            bool periodsSet = false;
            //FacilityOEEAvg fOEEAvg = null;
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                if (CurrentFacility != null)
                {
                    //fOEEAvg = 
                    //dbApp.FacilityMaterialOEE.Where(c => c.FacilityMaterial.FacilityID == CurrentFacility.FacilityID)
                    //    .GroupBy(g => 1)
                    //    .Select(g => new FacilityOEEAvg()
                    //    {
                    //        AvailabilityOEE = g.Average(c => c.AvailabilityOEE),
                    //        PerformanceOEE = g.Average(c => c.PerformanceOEE),
                    //        QualityOEE = g.Average(c => c.QualityOEE),
                    //        TotalOEE = g.Average(c => c.TotalOEE)
                    //    })
                    //    .FirstOrDefault();

                    FacilityMaterialOEE[] recentEntries = dbApp.FacilityMaterialOEE
                        .Where(c => c.FacilityMaterial.FacilityID == CurrentFacility.FacilityID)
                        .OrderByDescending(c => c.EndDate).Take(2).ToArray();
                    if (recentEntries != null && recentEntries.Any())
                    {
                        FacilityMaterialOEE latestEntry = recentEntries[0];
                        DateTime lastEntryOneDayInPast = latestEntry.EndDate.AddDays(-1);
                        bool smallestPeriodIsDay = false;
                        if (recentEntries.Count() > 1)
                        {
                            if (recentEntries[1].EndDate > lastEntryOneDayInPast)
                                smallestPeriodIsDay = true;
                        }

                        periodsSet = true;
                        Period1To = latestEntry.EndDate.AddSeconds(1);
                        Period2To = Period1To;
                        Period3To = Period1To;
                        if (smallestPeriodIsDay)
                        {
                            Period1From = lastEntryOneDayInPast;
                            Period2From = Period1To.Value.AddDays(-7);
                            Period3From = Period1From.Value.AddMonths(-1);
                        }
                        else
                        {
                            Period1From = Period1To.Value.AddDays(-7);
                            Period2From = Period1From.Value.AddMonths(-1);
                            Period3From = Period1From.Value.AddYears(-1);
                        }
                    }
                }
                if (!periodsSet)
                {
                    Period1To = null;
                    Period2To = null;
                    Period3To = null;
                    Period1From = null;
                    Period2From = null;
                    Period3From = null;
                }
                RefreshOEEAvgs(dbApp);
            }
        }

        [ACMethodCommand("", "en{'Refresh OEE'}de{'OEE Aktualisieren'}", 201)]
        public void RefreshOEEAvgs()
        {
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                RefreshOEEAvgs(dbApp);
            }
        }

        public void RefreshOEEAvgs(DatabaseApp dbApp)
        {
            DateTime? totalRangeFrom = Period1From;
            if (   !totalRangeFrom.HasValue
                || (Period2From.HasValue && Period2From.Value < totalRangeFrom.Value))
                totalRangeFrom = Period2From;
            if (!totalRangeFrom.HasValue
                || (Period3From.HasValue && Period3From.Value < totalRangeFrom.Value))
                totalRangeFrom = Period3From;
            
            DateTime? totalRangeTo = Period1To;
            if (!totalRangeTo.HasValue
                || (Period2To.HasValue && Period2To.Value < totalRangeTo.Value))
                totalRangeTo = Period2To;
            if (!totalRangeTo.HasValue
                || (Period3To.HasValue && Period3To.Value < totalRangeTo.Value))
                totalRangeTo = Period3To;

            if (   CurrentFacility == null 
                || !totalRangeFrom.HasValue 
                || !totalRangeTo.HasValue
                || FacilityOEEManager == null)
            {
                OEEAvgPeriod1 = _NullOEEAvg;
                OEEAvgPeriod2 = _NullOEEAvg;
                OEEAvgPeriod3 = _NullOEEAvg;
                return;
            }

            FacilityOEEAvg[] resultsInTotalRange = FacilityOEEManager.GetOEEEntries(dbApp, CurrentFacility, totalRangeFrom.Value, totalRangeTo.Value);
            List<FacilityOEEAvg> resultsInPeriod1 = new List<FacilityOEEAvg>();
            List<FacilityOEEAvg> resultsInPeriod2 = new List<FacilityOEEAvg>();
            List<FacilityOEEAvg> resultsInPeriod3 = new List<FacilityOEEAvg>();
            foreach (FacilityOEEAvg entry in resultsInTotalRange)
            {
                if (Period1From.HasValue && Period1To.HasValue && entry.EndDate >= Period1From.Value && entry.EndDate <= Period1To.Value)
                    resultsInPeriod1.Add(entry);
                if (Period2From.HasValue && Period2To.HasValue && entry.EndDate >= Period2From.Value && entry.EndDate <= Period2To.Value)
                    resultsInPeriod2.Add(entry);
                if (Period3From.HasValue && Period3To.HasValue && entry.EndDate >= Period3From.Value && entry.EndDate <= Period3To.Value)
                    resultsInPeriod3.Add(entry);
            }

            if (!resultsInPeriod1.Any())
                OEEAvgPeriod1 = _NullOEEAvg;
            else
                OEEAvgPeriod1 = new FacilityOEEAvg(resultsInPeriod1) { Facility = CurrentFacility };

            if (!resultsInPeriod2.Any())
                OEEAvgPeriod2 = _NullOEEAvg;
            else
                OEEAvgPeriod2 = new FacilityOEEAvg(resultsInPeriod2) { Facility = CurrentFacility };

            if (!resultsInPeriod3.Any())
                OEEAvgPeriod3 = _NullOEEAvg;
            else
                OEEAvgPeriod3 = new FacilityOEEAvg(resultsInPeriod3) { Facility = CurrentFacility };
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
                case nameof(Search):
                    Search();
                    return true;
                case nameof(RefreshOEEAvgs):
                    RefreshOEEAvgs();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}


