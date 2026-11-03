// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 19.01.2018
// ***********************************************************************
// <copyright file="MDBSOInRequestState.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
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
    /// Businessobject for maintaining the "Request Status" value list (master data): the status of a purchase request (inquiry/RFQ): New, Sent, Completed, Invalid.
    /// Call Search() to fill the InRequestStateList property with all entries of the table MDInRequestState.
    /// Select an entry in the list; it is provided as SelectedInRequestState and CurrentInRequestState for display and editing.
    /// Property changes should always be made to CurrentInRequestState and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns the new entity object to the CurrentInRequestState property. Fill in all required fields (at least the multilingual Name) before saving.
    /// Use the Delete() method to delete the current entry provided there are no foreign key relationships from other tables (purchase requests reference this state).
    /// Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load() method updates the CurrentInRequestState object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOInRequestState&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Request Status'}de{'Anfragestatus'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDInRequestState.ClassName,
        Description = @"Businessobject for maintaining the ""Request Status"" value list (master data): the status of a purchase request (inquiry/RFQ): New, Sent, Completed, Invalid.
        Call Search() to fill the InRequestStateList property with all entries of the table MDInRequestState.
        Select an entry in the list; it is provided as SelectedInRequestState and CurrentInRequestState for display and editing.
        Property changes should always be made to CurrentInRequestState and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns the new entity object to the CurrentInRequestState property. Fill in all required fields (at least the multilingual Name) before saving.
        Use the Delete() method to delete the current entry provided there are no foreign key relationships from other tables (purchase requests reference this state).
        Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load() method updates the CurrentInRequestState object with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOInRequestState&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class MDBSOInRequestState : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOInRequestState"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOInRequestState(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        ACAccessNav<MDInRequestState> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "MDInRequestState", "en{'Request Status'}de{'Anfragestatus'}", Description = @"Primary navigation access to the MDInRequestState table; used internally by Search() and list navigation.")]
        public ACAccessNav<MDInRequestState> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDInRequestState>("MDInRequestState", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the state of the selected in request.
        /// </summary>
        /// <value>The state of the selected in request.</value>
        [ACPropertySelected(9999, "MDInRequestState", "en{'Selected Request Status'}de{'Ausgewählt: Anfragestatus'}", Description = @"The request status currently selected in the list; read-only mirror of the navigation selection.")]
        public MDInRequestState SelectedInRequestState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedInRequestState");
            }
        }

        /// <summary>
        /// Gets or sets the state of the current in request.
        /// </summary>
        /// <value>The state of the current in request.</value>
        [ACPropertyCurrent(9999, "MDInRequestState", "en{'Current Request Status'}de{'Aktuell: Anfragestatus'}", Description = @"The request status currently being edited. Change its properties and call Save() to persist the changes.")]
        public MDInRequestState CurrentInRequestState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentInRequestState");
            }
        }

        /// <summary>
        /// Gets the in request state list.
        /// </summary>
        /// <value>The in request state list.</value>
        [ACPropertyList(9999, "MDInRequestState", "en{'Request Statuses'}de{'Anfragestatus'}", Description = @"Result list of the last search; contains all request statuses.")]
        public IEnumerable<MDInRequestState> InRequestStateList
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
        [ACMethodCommand("MDInRequestState", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = @"Saves all changes made to CurrentInRequestState to the database and refreshes the list.")]
        public async void Save()
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
        [ACMethodCommand("MDInRequestState", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = @"Discards all unsaved changes made to the current request status.")]
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
        [ACMethodInteraction("MDInRequestState", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedInRequestState", Global.ACKinds.MSMethodPrePost, Description = @"Reloads the selected request status from the database, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDInRequestState>(requery, () => SelectedInRequestState, () => CurrentInRequestState, c => CurrentInRequestState = c,
                        DatabaseApp.MDInRequestState
                        .Where(c => c.MDInRequestStateID == SelectedInRequestState.MDInRequestStateID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedInRequestState != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("MDInRequestState", Const.New, (short)MISort.New, true, "SelectedInRequestState", Global.ACKinds.MSMethodPrePost, Description = @"Creates a new request status and assigns it to CurrentInRequestState. Fill in the required fields (at least the multilingual Name) and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentInRequestState = MDInRequestState.NewACObject(DatabaseApp, null);
            DatabaseApp.MDInRequestState.Add(CurrentInRequestState);
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
        [ACMethodInteraction("MDInRequestState", Const.Delete, (short)MISort.Delete, true, "CurrentInRequestState", Global.ACKinds.MSMethodPrePost, Description = @"Deletes the current request status. Only possible if no other records reference it; call Save() afterwards to persist the deletion.")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentInRequestState.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentInRequestState);
            SelectedInRequestState = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(InRequestStateList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentInRequestState != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("MDInRequestState", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = @"Executes the search and fills InRequestStateList with all request statuses.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(InRequestStateList));
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
                    return new string[] { nameof(ACState), nameof(SelectedInRequestState) };
                case nameof(Delete):
                    return new string[] { nameof(CurrentInRequestState) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }
}