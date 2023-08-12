// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 19.01.2018
// ***********************************************************************
// <copyright file="MDBSOTimeRange.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Allgemeine Stammdatenmaske für MDTimeRange
    /// Bei den einfachen MD-Tabellen wird bewußt auf die Managerklassen verzichtet.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioScheduling, "en{'Shift'}de{'Schichten'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDTimeRange.ClassName)]
    class MDBSOTimeRange : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOTimeRange"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOTimeRange(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

            AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "IsShiftModel", Global.LogicalOperators.equal, Global.Operators.and, "true", true));

            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._AccessTimeRange = null;
            this._CurrentTimeRange = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessTimeRange != null)
            {
                _AccessTimeRange.ACDeInit(false);
                _AccessTimeRange = null;
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
        #region 1. MDShiftModel
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<MDTimeRange> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "MDTimeRangeModel")]
        public ACAccessNav<MDTimeRange> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDTimeRange>(MDTimeRange.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected time range model.
        /// </summary>
        /// <value>The selected time range model.</value>
        [ACPropertySelected(9999, "MDTimeRangeModel")]
        public MDTimeRange SelectedTimeRangeModel
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedTimeRangeModel");
            }
        }

        /// <summary>
        /// Gets or sets the current time range model.
        /// </summary>
        /// <value>The current time range model.</value>
        [ACPropertyCurrent(9999, "MDTimeRangeModel")]
        public MDTimeRange CurrentTimeRangeModel
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentTimeRangeModel");
                OnPropertyChanged("TimeRangeList");
            }
        }

        /// <summary>
        /// Gets the time range model list.
        /// </summary>
        /// <value>The time range model list.</value>
        [ACPropertyList(9999, "MDTimeRangeModel")]
        public IEnumerable<MDTimeRange> TimeRangeModelList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region 1.1 MDTimeRange
        /// <summary>
        /// The _ access time range
        /// </summary>
        ACAccess<MDTimeRange> _AccessTimeRange;
        /// <summary>
        /// Gets the access time range.
        /// </summary>
        /// <value>The access time range.</value>
        [ACPropertyAccess(9999, MDTimeRange.ClassName)]
        public ACAccess<MDTimeRange> AccessTimeRange
        {
            get
            {
                if (_AccessTimeRange == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + "MDTimeRangePart") as ACQueryDefinition;
                    _AccessTimeRange = acQueryDefinition.NewAccess<MDTimeRange>(MDTimeRange.ClassName, this);
                }
                return _AccessTimeRange;
            }
        }

        /// <summary>
        /// The _ current time range
        /// </summary>
        MDTimeRange _CurrentTimeRange;
        /// <summary>
        /// Gets or sets the current time range.
        /// </summary>
        /// <value>The current time range.</value>
        [ACPropertyCurrent(9999, MDTimeRange.ClassName)]
        public MDTimeRange CurrentTimeRange
        {
            get
            {
                return _CurrentTimeRange;
            }
            set
            {
                if (_CurrentTimeRange != value)
                {
                    _CurrentTimeRange = value;
                    OnPropertyChanged("CurrentTimeRange");
                }
            }
        }

        /// <summary>
        /// Gets the time range list.
        /// </summary>
        /// <value>The time range list.</value>
        [ACPropertyList(9999, MDTimeRange.ClassName)]
        public IEnumerable<MDTimeRange> TimeRangeList
        {
            get
            {
                if (CurrentTimeRangeModel == null)
                    return null;
                return CurrentTimeRangeModel.MDTimeRange_ParentMDTimeRange;
            }
        }
        #endregion

        #endregion

        #region BSO->ACMethod
        #region 1. MDTimeRangeModel
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("MDTimeRangeModel", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand("MDTimeRangeModel", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction("MDTimeRangeModel", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedTimeRangeModel", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDTimeRange>(requery, () => SelectedTimeRangeModel, () => CurrentTimeRangeModel, c => CurrentTimeRangeModel = c,
                        DatabaseApp.MDTimeRange
                        .Where(c => c.MDTimeRangeID == SelectedTimeRangeModel.MDTimeRangeID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedTimeRangeModel != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("MDTimeRangeModel", "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedTimeRange", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentTimeRangeModel = MDTimeRange.NewACObject(DatabaseApp, null);
            DatabaseApp.MDTimeRange.Add(CurrentTimeRangeModel);
            ACState = Const.SMNew;
            PostExecute("Neu");

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
        [ACMethodInteraction("MDTimeRangeModel", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentTimeRange", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentTimeRangeModel.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentTimeRange);
            CurrentTimeRange = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether this instance is enabled.
        /// </summary>
        /// <returns><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</returns>
        public bool IsEnabled()
        {
            return CurrentTimeRangeModel != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("MDTimeRangeModel", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("TimeRangeModelList");
        }
        #endregion

        #region 1.1 MDTimeRange
        /// <summary>
        /// News the time range.
        /// </summary>
        [ACMethodInteraction(MDTimeRange.ClassName, "en{'New Schift'}de{'Neue Schicht'}", (short)MISort.New, true, "CurrentTimeRange", Global.ACKinds.MSMethodPrePost)]
        public void NewTimeRange()
        {
            if (!PreExecute("NewTimeRange")) return;
            CurrentTimeRange = MDTimeRange.NewACObject(DatabaseApp, CurrentTimeRangeModel);
            CurrentTimeRange.MDTimeRange1_ParentMDTimeRange = CurrentTimeRangeModel;
            PostExecute("NewTimeRange");
            OnPropertyChanged("TimeRangeList");
        }

        /// <summary>
        /// Determines whether [is enabled new time range].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new time range]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewTimeRange()
        {
            return CurrentTimeRangeModel != null;
        }

        /// <summary>
        /// Deletes the time range.
        /// </summary>
        [ACMethodInteraction(MDTimeRange.ClassName, "en{'Delete Schift'}de{'Schicht löschen'}", (short)MISort.Delete, true, "CurrentTimeRange", Global.ACKinds.MSMethodPrePost)]
        public void DeleteTimeRange()
        {
            if (CurrentTimeRange != null)
            {
                if (!PreExecute("DeleteTimeRange")) return;
                CurrentTimeRangeModel.MDTimeRange_ParentMDTimeRange.Remove(CurrentTimeRange);
                PostExecute("DeleteTimeRange");
            }
        }

        /// <summary>
        /// Determines whether [is enabled delete time range].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete time range]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteTimeRange()
        {
            return CurrentTimeRangeModel != null && CurrentTimeRange != null;
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
                case nameof(IsEnabled):
                    result = IsEnabled();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(NewTimeRange):
                    NewTimeRange();
                    return true;
                case nameof(IsEnabledNewTimeRange):
                    result = IsEnabledNewTimeRange();
                    return true;
                case nameof(DeleteTimeRange):
                    DeleteTimeRange();
                    return true;
                case nameof(IsEnabledDeleteTimeRange):
                    result = IsEnabledDeleteTimeRange();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}