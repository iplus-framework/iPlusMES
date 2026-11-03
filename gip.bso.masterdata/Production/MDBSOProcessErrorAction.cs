// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 19.01.2018
// ***********************************************************************
// <copyright file="MDBSOProcessErrorAction.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Businessobject for maintaining the "Process Error Action" value list (master data): the reaction to a process error: Warning Only or Block Process.
    /// Call Search() to fill the ProcessErrorActionList property with all entries of the table MDProcessErrorAction.
    /// Select an entry in the list; it is provided as SelectedProcessErrorAction and CurrentProcessErrorAction for display and editing.
    /// Property changes should always be made to CurrentProcessErrorAction and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns the new entity object to the CurrentProcessErrorAction property. Fill in all required fields (at least the multilingual Name) before saving.
    /// Use the Delete() method to delete the current entry provided there are no foreign key relationships from other tables (process modules and error handlers reference this action).
    /// Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load() method updates the CurrentProcessErrorAction object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOProcessErrorAction&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Process Error Action'}de{'Prozessfehleraktion'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDProcessErrorAction.ClassName,
        Description = @"Businessobject for maintaining the ""Process Error Action"" value list (master data): the reaction to a process error: Warning Only or Block Process.
        Call Search() to fill the ProcessErrorActionList property with all entries of the table MDProcessErrorAction.
        Select an entry in the list; it is provided as SelectedProcessErrorAction and CurrentProcessErrorAction for display and editing.
        Property changes should always be made to CurrentProcessErrorAction and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns the new entity object to the CurrentProcessErrorAction property. Fill in all required fields (at least the multilingual Name) before saving.
        Use the Delete() method to delete the current entry provided there are no foreign key relationships from other tables (process modules and error handlers reference this action).
        Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load() method updates the CurrentProcessErrorAction object with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOProcessErrorAction&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    ,
                ")]
    public class MDBSOProcessErrorAction : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOProcessErrorAction"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOProcessErrorAction(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        ACAccessNav<MDProcessErrorAction> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "MDProcessErrorAction", "en{'Process Error Action'}de{'Prozessfehleraktion'}", Description = @"Primary navigation access to the MDProcessErrorAction table; used internally by Search() and list navigation.")]
        public ACAccessNav<MDProcessErrorAction> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDProcessErrorAction>("MDProcessErrorAction", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected process error action.
        /// </summary>
        /// <value>The selected process error action.</value>
        [ACPropertySelected(9999, "MDProcessErrorAction", "en{'Selected Process Error Action'}de{'Ausgewählt: Prozessfehleraktion'}", Description = @"The process error action currently selected in the list; read-only mirror of the navigation selection.")]
        public MDProcessErrorAction SelectedProcessErrorAction
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedProcessErrorAction");
            }
        }

        /// <summary>
        /// Gets or sets the current process error action.
        /// </summary>
        /// <value>The current process error action.</value>
        [ACPropertyCurrent(9999, "MDProcessErrorAction", "en{'Current Process Error Action'}de{'Aktuell: Prozessfehleraktion'}", Description = @"The process error action currently being edited. Change its properties and call Save() to persist the changes.")]
        public MDProcessErrorAction CurrentProcessErrorAction
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentProcessErrorAction");
            }
        }

        /// <summary>
        /// Gets the process error action list.
        /// </summary>
        /// <value>The process error action list.</value>
        [ACPropertyList(9999, "MDProcessErrorAction", "en{'Process Error Actions'}de{'Prozessfehleraktionen'}", Description = @"Result list of the last search; contains all process error actions.")]
        public IEnumerable<MDProcessErrorAction> ProcessErrorActionList
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
        [ACMethodCommand("MDProcessErrorAction", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = @"Saves all changes made to CurrentProcessErrorAction to the database and refreshes the list.")]
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
        [ACMethodCommand("MDProcessErrorAction", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = @"Discards all unsaved changes made to the current process error action.")]
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
        [ACMethodInteraction("MDProcessErrorAction", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedProcessErrorAction", Global.ACKinds.MSMethodPrePost, Description = @"Reloads the selected process error action from the database, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDProcessErrorAction>(requery, () => SelectedProcessErrorAction, () => CurrentProcessErrorAction, c => CurrentProcessErrorAction = c,
                        DatabaseApp.MDProcessErrorAction
                        .Where(c => c.MDProcessErrorActionID == SelectedProcessErrorAction.MDProcessErrorActionID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedProcessErrorAction != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("MDProcessErrorAction", Const.New, (short)MISort.New, true, "SelectedProcessErrorAction", Global.ACKinds.MSMethodPrePost, Description = @"Creates a new process error action and assigns it to CurrentProcessErrorAction. Fill in the required fields (at least the multilingual Name) and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentProcessErrorAction = MDProcessErrorAction.NewACObject(DatabaseApp, null);
            DatabaseApp.MDProcessErrorAction.Add(CurrentProcessErrorAction);
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
        [ACMethodInteraction("MDProcessErrorAction", Const.Delete, (short)MISort.Delete, true, "CurrentProcessErrorAction", Global.ACKinds.MSMethodPrePost, Description = @"Deletes the current process error action. Only possible if no other records reference it; call Save() afterwards to persist the deletion.")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentProcessErrorAction.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentProcessErrorAction);
            SelectedProcessErrorAction = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(ProcessErrorActionList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentProcessErrorAction != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("MDProcessErrorAction", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = @"Executes the search and fills ProcessErrorActionList with all process error actions.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(ProcessErrorActionList));
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
                case nameof(Load):
                case nameof(IsEnabledLoad):
                    return new string[] { nameof(SelectedProcessErrorAction) };
                case nameof(New):
                case nameof(IsEnabledNew):
                    return new string[] { nameof(InitState) };
                case nameof(Delete):
                case nameof(IsEnabledDelete):
                    return new string[] { nameof(CurrentProcessErrorAction) };
                case nameof(Search):
                    return new string[] { nameof(InitState) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }
}