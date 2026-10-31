// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 19.01.2018
// ***********************************************************************
// <copyright file="MDBSOReleaseState.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Business object (value list) for maintaining the "Release State" master data (table MDReleaseState).
    /// A release state describes the release/blocking state of facility stock: released, blocked, absolutely blocked or release of absolutely locked stock.
    /// To load the list, call Search(); the database result is copied to the ReleaseStateList property.
    /// Then call NavigateFirst() to set CurrentReleaseState with the first record of the list.
    /// CurrentReleaseState is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentReleaseState; call Save() to persist them before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns it to CurrentReleaseState; fill all required fields before calling Save().
    /// Use Delete() to delete the current record provided no foreign key relationships exist from other tables; always call Save() afterwards to execute the delete in the database.
    /// The Load() method updates CurrentReleaseState with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOReleaseState&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.ESReleaseState, Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDReleaseState.ClassName,
        Description = @"Business object (value list) for maintaining the ""Release State"" master data (table MDReleaseState).
        A release state describes the release/blocking state of facility stock: released, blocked, absolutely blocked or release of absolutely locked stock.
        To load the list, call Search(); the database result is copied to the ReleaseStateList property.
        Then call NavigateFirst() to set CurrentReleaseState with the first record of the list.
        CurrentReleaseState is used to display and edit the currently selected record.
        Property changes should always be made to CurrentReleaseState; call Save() to persist them before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns it to CurrentReleaseState; fill all required fields before calling Save().
        Use Delete() to delete the current record provided no foreign key relationships exist from other tables; always call Save() afterwards to execute the delete in the database.
        The Load() method updates CurrentReleaseState with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOReleaseState&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.")]
    public class MDBSOReleaseState : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOReleaseState"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOReleaseState(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            Search();
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }
        #endregion

        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<MDReleaseState> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDReleaseState.ClassName, Description = @"Primary navigation access to the MDReleaseState table. Provides the query, the loaded list (NavList) and the currently selected record.")]
        public ACAccessNav<MDReleaseState> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDReleaseState>(MDReleaseState.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the state of the selected release.
        /// </summary>
        /// <value>The state of the selected release.</value>
        [ACPropertySelected(9999, MDReleaseState.ClassName, Description = @"The Release State record currently selected in the list.")]
        public MDReleaseState SelectedReleaseState
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
                OnPropertyChanged("SelectedReleaseState");
            }
        }

        /// <summary>
        /// Gets or sets the state of the current release.
        /// </summary>
        /// <value>The state of the current release.</value>
        [ACPropertyCurrent(9999, MDReleaseState.ClassName, Description = @"The Release State record currently displayed and edited. All changes must be made on this object, followed by Save().")]
        public MDReleaseState CurrentReleaseState
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
                OnPropertyChanged("CurrentReleaseState");
            }
        }

        /// <summary>
        /// Gets the release state list.
        /// </summary>
        /// <value>The release state list.</value>
        [ACPropertyList(9999, MDReleaseState.ClassName, Description = @"The loaded Release State value list (result of Search()).")]
        public IEnumerable<MDReleaseState> ReleaseStateList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(MDReleaseState.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = @"Saves all changes made on CurrentReleaseState to the database.")]
        public async Task Save()
        {
            if (await OnSave())
                Search();
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
        [ACMethodCommand(MDReleaseState.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = @"Discards all unsaved changes on CurrentReleaseState and restores the last saved state.")]
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
        [ACMethodInteraction(MDReleaseState.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedReleaseState", Global.ACKinds.MSMethodPrePost, Description = @"Reloads the record selected in ReleaseStateList from the database into CurrentReleaseState, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDReleaseState>(requery, () => SelectedReleaseState, () => CurrentReleaseState, c => CurrentReleaseState = c,
                        DatabaseApp.MDReleaseState
                        .Where(c => c.MDReleaseStateID == SelectedReleaseState.MDReleaseStateID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedReleaseState != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDReleaseState.ClassName, Const.New, (short)MISort.New, true, "SelectedReleaseState", Global.ACKinds.MSMethodPrePost, Description = @"Creates a new Release State record and assigns it to CurrentReleaseState. Fill all required fields and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentReleaseState = MDReleaseState.NewACObject(DatabaseApp, null);
            DatabaseApp.MDReleaseState.Add(CurrentReleaseState);
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
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(MDReleaseState.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentReleaseState", Global.ACKinds.MSMethodPrePost, Description = @"Deletes the current Release State record if no other tables reference it (foreign key check). Call Save() afterwards to execute the delete in the database.")]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (AccessPrimary == null)
                return;
            Msg msg = CurrentReleaseState.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            AccessPrimary.NavList.Remove(CurrentReleaseState);
            SelectedReleaseState = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(ReleaseStateList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentReleaseState != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDReleaseState.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = @"Reloads the Release State value list from the database into ReleaseStateList.")]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(ReleaseStateList));
        }

        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Save):
                    _ = Save();
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
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(Search):
                    return new string[] { nameof(InitState) };
                case nameof(Load):
                    return new string[] { nameof(ACState), nameof(SelectedReleaseState) };
                case nameof(Delete):
                    return new string[] { nameof(CurrentReleaseState) };
                case nameof(New):
                    return new string[] { nameof(ACState) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }
}