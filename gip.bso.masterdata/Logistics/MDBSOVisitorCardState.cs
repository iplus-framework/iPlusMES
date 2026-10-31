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
// <copyright file="MDBSOVisitorCardState.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.mes.autocomponent;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Businessobject or App for maintaining the VisitorCardState value list.
    /// It defines the statuses of a visitor card (Not Used, Active, Blocked) and is used to control whether a card may be used.
    /// To search for records, set the SearchWord of the navigation query and call Search(); the database result is copied to the MDVisitorCardStateList property.
    /// Then call NavigateFirst() to set CurrentMDVisitorCardState to the first record of the list.
    /// CurrentMDVisitorCardState is used to display and edit the currently selected record.
    /// Property changes should always be made on CurrentMDVisitorCardState; call Save() before navigating to another record or creating a new one.
    /// The New() method creates a new record and assigns it to CurrentMDVisitorCardState; fill all required fields before calling Save().
    /// Use Delete() to delete the record provided no foreign key relationships exist from other tables; always call Save() afterwards to execute the delete in the database.
    /// The Load() method refreshes CurrentMDVisitorCardState with fresh database data if another user has changed the record in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOVisitorCardState&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Visitor Card Status'}de{'Besucherausweis-Status'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDVisitorCardState.ClassName,
        Description = @"Businessobject or App for maintaining the VisitorCardState value list.
        It defines the statuses of a visitor card (Not Used, Active, Blocked) and is used to control whether a card may be used.
        To search for records, set the SearchWord of the navigation query and call Search(); the database result is copied to the MDVisitorCardStateList property.
        Then call NavigateFirst() to set CurrentMDVisitorCardState to the first record of the list.
        CurrentMDVisitorCardState is used to display and edit the currently selected record.
        Property changes should always be made on CurrentMDVisitorCardState; call Save() before navigating to another record or creating a new one.
        The New() method creates a new record and assigns it to CurrentMDVisitorCardState; fill all required fields before calling Save().
        Use Delete() to delete the record provided no foreign key relationships exist from other tables; always call Save() afterwards to execute the delete in the database.
        The Load() method refreshes CurrentMDVisitorCardState with fresh database data if another user has changed the record in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOVisitorCardState&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class MDBSOVisitorCardState : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOVisitorCardState"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOVisitorCardState(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        ACAccessNav<MDVisitorCardState> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "MDVisitorCardState", Description = "Primary navigation access to the MDVisitorCardState value list. Used by the BSO to execute the query, searches and navigation.")]
        public ACAccessNav<MDVisitorCardState> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDVisitorCardState>("MDVisitorCardState", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the state of the selected visitor card.
        /// </summary>
        /// <value>The state of the selected visitor card.</value>
        [ACPropertySelected(9999, "MDVisitorCardState", Description = "Gets or sets the MDVisitorCardState record currently selected in the result list.")]
        public MDVisitorCardState SelectedVisitorCardState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedVisitorCardState");
            }
        }

        /// <summary>
        /// Gets or sets the state of the current visitor card.
        /// </summary>
        /// <value>The state of the current visitor card.</value>
        [ACPropertyCurrent(9999, "MDVisitorCardState", Description = "Gets or sets the MDVisitorCardState record currently loaded for display and editing. Apply all changes here and call Save() afterwards.")]
        public MDVisitorCardState CurrentVisitorCardState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentVisitorCardState");
            }
        }

        /// <summary>
        /// Gets the visitor card state list.
        /// </summary>
        /// <value>The visitor card state list.</value>
        [ACPropertyList(9999, "MDVisitorCardState", Description = "Gets the result list of the last search, filled by Search() from the database.")]
        public IEnumerable<MDVisitorCardState> VisitorCardStateList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("MDVisitorCardState", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves all changes made on the current MDVisitorCardState record to the database.")]
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
        [ACMethodCommand("MDVisitorCardState", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards all unsaved changes on the current MDVisitorCardState record and restores the last saved state.")]
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
        [ACMethodInteraction("MDVisitorCardState", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedVisitorCardState", Global.ACKinds.MSMethodPrePost, Description = "Reloads the selected MDVisitorCardState record from the database, discarding local changes. Use if another user has changed the record in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDVisitorCardState>(requery, () => SelectedVisitorCardState, () => CurrentVisitorCardState, c => CurrentVisitorCardState = c,
                        DatabaseApp.MDVisitorCardState
                        .Where(c => c.MDVisitorCardStateID == SelectedVisitorCardState.MDVisitorCardStateID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedVisitorCardState != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("MDVisitorCardState", Const.New, (short)MISort.New, true, "SelectedVisitorCardState", Global.ACKinds.MSMethodPrePost, Description = "Creates a new MDVisitorCardState record, adds it to the list and makes it the current record. Fill all required fields and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentVisitorCardState = MDVisitorCardState.NewACObject(DatabaseApp, null);
            DatabaseApp.MDVisitorCardState.Add(CurrentVisitorCardState);
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
        [ACMethodInteraction("MDVisitorCardState", Const.Delete, (short)MISort.Delete, true, "CurrentVisitorCardState", Global.ACKinds.MSMethodPrePost, Description = "Deletes the current MDVisitorCardState record if no foreign key relationships exist from other tables. Call Save() to execute the delete in the database.")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentVisitorCardState.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentVisitorCardState);
            SelectedVisitorCardState = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(VisitorCardStateList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentVisitorCardState != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("MDVisitorCardState", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Executes the search with the current search word and refreshes the MDVisitorCardStateList property.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(VisitorCardStateList));
        }

        /// <summary>
        /// Gets the properties to observe for IsEnabled bindings.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>Array of property names to observe.</returns>
        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string methodName)
        {
            switch (methodName)
            {
                case nameof(Search):
                    // Search is always enabled
                    return new string[] { nameof(InitState) };
                case nameof(Load):
                    // Load depends on ACState and SelectedVisitorCardState
                    return new string[] { nameof(ACState), nameof(SelectedVisitorCardState) };
                case nameof(Delete):
                    // Delete depends on CurrentVisitorCardState
                    return new string[] { nameof(CurrentVisitorCardState) };
                case nameof(New):
                    // New depends on ACState
                    return new string[] { nameof(ACState) };
                default:
                    return base.GetPropsToObserveForIsEnabled(methodName);
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

        #endregion

    }
}