// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 19.10.2018
// ***********************************************************************
// <copyright file="MDBSOToleranceState.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Businessobject for maintaining the "Tolerance Status" value list (master data): the result of the tolerance check of a production order/batch: No Tolerance Overruns or Tolerance Overruns Exists.
    /// Call Search() to fill the ToleranceStateList property with all entries of the table MDToleranceState.
    /// Select an entry in the list; it is provided as SelectedToleranceState and CurrentToleranceState for display and editing.
    /// Property changes should always be made to CurrentToleranceState and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns the new entity object to the CurrentToleranceState property. Fill in all required fields (at least the multilingual Name) before saving.
    /// Use the Delete() method to delete the current entry provided there are no foreign key relationships from other tables (production orders and batches reference this state).
    /// Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load() method updates the CurrentToleranceState object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOToleranceState&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Tolerance Status'}de{'Toleranz Status'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDToleranceState.ClassName,
        Description = @"Businessobject for maintaining the ""Tolerance Status"" value list (master data): the result of the tolerance check of a production order/batch: No Tolerance Overruns or Tolerance Overruns Exists.
        Call Search() to fill the ToleranceStateList property with all entries of the table MDToleranceState.
        Select an entry in the list; it is provided as SelectedToleranceState and CurrentToleranceState for display and editing.
        Property changes should always be made to CurrentToleranceState and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns the new entity object to the CurrentToleranceState property. Fill in all required fields (at least the multilingual Name) before saving.
        Use the Delete() method to delete the current entry provided there are no foreign key relationships from other tables (production orders and batches reference this state).
        Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load() method updates the CurrentToleranceState object with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOToleranceState&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class MDBSOToleranceState : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOToleranceState"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOToleranceState(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        ACAccessNav<MDToleranceState> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDToleranceState.ClassName, "en{'Tolerance Status'}de{'Toleranz Status'}", Description = @"Primary navigation access to the MDToleranceState table; used internally by Search() and list navigation.")]
        public ACAccessNav<MDToleranceState> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDToleranceState>(MDToleranceState.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the state of the selected tolerance.
        /// </summary>
        /// <value>The state of the selected tolerance.</value>
        [ACPropertySelected(9999, MDToleranceState.ClassName, "en{'Selected Tolerance Status'}de{'Ausgewählt: Toleranz Status'}", Description = @"The tolerance status currently selected in the list; read-only mirror of the navigation selection.")]
        public MDToleranceState SelectedToleranceState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedToleranceState");
            }
        }

        /// <summary>
        /// Gets or sets the state of the current tolerance.
        /// </summary>
        /// <value>The state of the current tolerance.</value>
        [ACPropertyCurrent(9999, MDToleranceState.ClassName, "en{'Current Tolerance Status'}de{'Aktuell: Toleranz Status'}", Description = @"The tolerance status currently being edited. Change its properties and call Save() to persist the changes.")]
        public MDToleranceState CurrentToleranceState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentToleranceState");
            }
        }

        /// <summary>
        /// Gets the tolerance state list.
        /// </summary>
        /// <value>The tolerance state list.</value>
        [ACPropertyList(9999, MDToleranceState.ClassName, "en{'Tolerance Statuses'}de{'Toleranz Status'}", Description = @"Result list of the last search; contains all tolerance statuses.")]
        public IEnumerable<MDToleranceState> ToleranceStateList
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
        [ACMethodCommand(MDToleranceState.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = @"Saves all changes made to CurrentToleranceState to the database and refreshes the list.")]
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
        [ACMethodCommand(MDToleranceState.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = @"Discards all unsaved changes made to the current tolerance status.")]
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
        [ACMethodInteraction(MDToleranceState.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedToleranceState", Global.ACKinds.MSMethodPrePost, Description = @"Reloads the selected tolerance status from the database, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDToleranceState>(requery, () => SelectedToleranceState, () => CurrentToleranceState, c => CurrentToleranceState = c,
                        DatabaseApp.MDToleranceState
                        .Where(c => c.MDToleranceStateID == SelectedToleranceState.MDToleranceStateID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedToleranceState != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDToleranceState.ClassName, Const.New, (short)MISort.New, true, "SelectedToleranceState", Global.ACKinds.MSMethodPrePost, Description = @"Creates a new tolerance status and assigns it to CurrentToleranceState. Fill in the required fields (at least the multilingual Name) and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentToleranceState = MDToleranceState.NewACObject(DatabaseApp, null);
            DatabaseApp.MDToleranceState.Add(CurrentToleranceState);
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
        [ACMethodInteraction(MDToleranceState.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentToleranceState", Global.ACKinds.MSMethodPrePost, Description = @"Deletes the current tolerance status. Only possible if no other records reference it; call Save() afterwards to persist the deletion.")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentToleranceState.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentToleranceState);
            SelectedToleranceState = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(ToleranceStateList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentToleranceState != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDToleranceState.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = @"Executes the search and fills ToleranceStateList with all tolerance statuses.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(ToleranceStateList));
        }

        /// <summary>
        /// Gets the properties to observe for IsEnabled changes for a given method name.
        /// </summary>
        /// <param name="methodName">The method name.</param>
        /// <returns>Array of property names to observe.</returns>
        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string methodName)
        {
            return methodName switch
            {
                nameof(Search) => new string[] { nameof(InitState) },
                nameof(Load) => new string[] { nameof(ACState), nameof(SelectedToleranceState) },
                nameof(New) => new string[] { nameof(ACState) },
                nameof(Delete) => new string[] { nameof(CurrentToleranceState) },
                _ => base.GetPropsToObserveForIsEnabled(methodName),
            };
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

        #endregion

    }
}