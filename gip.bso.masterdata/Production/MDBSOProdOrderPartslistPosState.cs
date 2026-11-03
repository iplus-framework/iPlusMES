// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 19.01.2018
// ***********************************************************************
// <copyright file="MDBSOProdOrderPartslistPosState.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Businessobject for maintaining the "Prod.Order BOM Pos. Status" value list (master data): the status of a production order BOM (partslist) position: Created, In Progress, Completed, Partially Completed, Blocked, Cancelled, Auto Start.
    /// Call Search() to fill the ProdOrderPartslistPosStateList property with all entries of the table MDProdOrderPartslistPosState.
    /// Select an entry in the list; it is provided as SelectedProdOrderPartslistPosState and CurrentProdOrderPartslistPosState for display and editing.
    /// Property changes should always be made to CurrentProdOrderPartslistPosState and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns the new entity object to the CurrentProdOrderPartslistPosState property. Fill in all required fields (at least the multilingual Name) before saving.
    /// Use the Delete() method to delete the current entry provided there are no foreign key relationships from other tables (production order partslist positions reference this state).
    /// Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load() method updates the CurrentProdOrderPartslistPosState object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOProdOrderPartslistPosState&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Prod.Order BOM Pos. Status'}de{'Prod.auftrag Stückl. Pos.-Status'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDProdOrderPartslistPosState.ClassName,
        Description = @"Businessobject for maintaining the ""Prod.Order BOM Pos. Status"" value list (master data): the status of a production order BOM (partslist) position: Created, In Progress, Completed, Partially Completed, Blocked, Cancelled, Auto Start.
        Call Search() to fill the ProdOrderPartslistPosStateList property with all entries of the table MDProdOrderPartslistPosState.
        Select an entry in the list; it is provided as SelectedProdOrderPartslistPosState and CurrentProdOrderPartslistPosState for display and editing.
        Property changes should always be made to CurrentProdOrderPartslistPosState and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns the new entity object to the CurrentProdOrderPartslistPosState property. Fill in all required fields (at least the multilingual Name) before saving.
        Use the Delete() method to delete the current entry provided there are no foreign key relationships from other tables (production order partslist positions reference this state).
        Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load() method updates the CurrentProdOrderPartslistPosState object with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOProdOrderPartslistPosState&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class MDBSOProdOrderPartslistPosState : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOProdOrderPartslistPosState"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOProdOrderPartslistPosState(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        ACAccessNav<MDProdOrderPartslistPosState> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDProdOrderPartslistPosState.ClassName, "en{'Prod.Order BOM Pos. Status'}de{'Prod.auftrag Stückl. Pos.-Status'}", Description = @"Primary navigation access to the MDProdOrderPartslistPosState table; used internally by Search() and list navigation.")]
        public ACAccessNav<MDProdOrderPartslistPosState> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDProdOrderPartslistPosState>(MDProdOrderPartslistPosState.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the state of the selected out order pos.
        /// </summary>
        /// <value>The state of the selected out order pos.</value>
        [ACPropertySelected(9999, MDProdOrderPartslistPosState.ClassName, "en{'Selected Prod.Order BOM Pos. Status'}de{'Ausgewählt: Prod.auftrag Stückl. Pos.-Status'}", Description = @"The prod.order bom pos. status currently selected in the list; read-only mirror of the navigation selection.")]
        public MDProdOrderPartslistPosState SelectedProdOrderPartslistPosState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedProdOrderPartslistPosState");
            }
        }

        /// <summary>
        /// Gets or sets the state of the current out order pos.
        /// </summary>
        /// <value>The state of the current out order pos.</value>
        [ACPropertyCurrent(9999, MDProdOrderPartslistPosState.ClassName, "en{'Current Prod.Order BOM Pos. Status'}de{'Aktuell: Prod.auftrag Stückl. Pos.-Status'}", Description = @"The prod.order bom pos. status currently being edited. Change its properties and call Save() to persist the changes.")]
        public MDProdOrderPartslistPosState CurrentProdOrderPartslistPosState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentProdOrderPartslistPosState");
            }
        }

        /// <summary>
        /// Gets the out order pos state list.
        /// </summary>
        /// <value>The out order pos state list.</value>
        [ACPropertyList(9999, MDProdOrderPartslistPosState.ClassName, "en{'Prod.Order BOM Pos. Statuses'}de{'Prod.auftrag Stückl. Pos.-Status'}", Description = @"Result list of the last search; contains all prod.order bom pos. statuses.")]
        public IEnumerable<MDProdOrderPartslistPosState> ProdOrderPartslistPosStateList
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
        [ACMethodCommand(MDProdOrderPartslistPosState.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = @"Saves all changes made to CurrentProdOrderPartslistPosState to the database and refreshes the list.")]
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
        [ACMethodCommand(MDProdOrderPartslistPosState.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = @"Discards all unsaved changes made to the current prod.order bom pos. status.")]
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
        [ACMethodInteraction(MDProdOrderPartslistPosState.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedProdOrderPartslistPosState", Global.ACKinds.MSMethodPrePost, Description = @"Reloads the selected prod.order bom pos. status from the database, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDProdOrderPartslistPosState>(requery, () => SelectedProdOrderPartslistPosState, () => CurrentProdOrderPartslistPosState, c => CurrentProdOrderPartslistPosState = c,
                        DatabaseApp.MDProdOrderPartslistPosState
                        .Where(c => c.MDProdOrderPartslistPosStateID == SelectedProdOrderPartslistPosState.MDProdOrderPartslistPosStateID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedProdOrderPartslistPosState != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDProdOrderPartslistPosState.ClassName, Const.New, (short)MISort.New, true, "SelectedProdOrderPartslistPosState", Global.ACKinds.MSMethodPrePost, Description = @"Creates a new prod.order bom pos. status and assigns it to CurrentProdOrderPartslistPosState. Fill in the required fields (at least the multilingual Name) and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentProdOrderPartslistPosState = MDProdOrderPartslistPosState.NewACObject(DatabaseApp, null);
            DatabaseApp.MDProdOrderPartslistPosState.Add(CurrentProdOrderPartslistPosState);
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
        [ACMethodInteraction(MDProdOrderPartslistPosState.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentProdOrderPartslistPosState", Global.ACKinds.MSMethodPrePost, Description = @"Deletes the current prod.order bom pos. status. Only possible if no other records reference it; call Save() afterwards to persist the deletion.")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentProdOrderPartslistPosState.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentProdOrderPartslistPosState);
            SelectedProdOrderPartslistPosState = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(ProdOrderPartslistPosStateList));
        }

        /// <summary>
        /// Determines whether [is enabled delete out order pos state].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete out order pos state]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteProdOrderPartslistPosState()
        {
            return CurrentProdOrderPartslistPosState != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDProdOrderPartslistPosState.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = @"Executes the search and fills ProdOrderPartslistPosStateList with all prod.order bom pos. statuses.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(ProdOrderPartslistPosStateList));
        }

        /// <summary>
        /// Gets the properties to observe for IsEnabled changes.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>Array of property names to observe.</returns>
        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string methodName)
        {
            return methodName switch
            {
                nameof(New) => new string[] { nameof(ACState) },
                nameof(Load) => new string[] { nameof(ACState), nameof(SelectedProdOrderPartslistPosState) },
                nameof(Delete) => new string[] { nameof(CurrentProdOrderPartslistPosState) },
                _ => base.GetPropsToObserveForIsEnabled(methodName)
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
                case nameof(IsEnabledDeleteProdOrderPartslistPosState):
                    result = IsEnabledDeleteProdOrderPartslistPosState();
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