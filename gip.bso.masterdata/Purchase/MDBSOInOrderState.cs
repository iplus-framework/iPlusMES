// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 19.01.2018
// ***********************************************************************
// <copyright file="MDBSOInOrderState.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Businessobject for maintaining the "Purchase Order Status" value list (master data): the status of a purchase order (header): Created, In Process, Completed, Blocked, Cancelled.
    /// Call Search() to fill the InOrderStateList property with all entries of the table MDInOrderState.
    /// Select an entry in the list; it is provided as SelectedInOrderState and CurrentInOrderState for display and editing.
    /// Property changes should always be made to CurrentInOrderState and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns the new entity object to the CurrentInOrderState property. Fill in all required fields (at least the multilingual Name) before saving.
    /// Use the Delete() method to delete the current entry provided there are no foreign key relationships from other tables (purchase orders reference this state).
    /// Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load() method updates the CurrentInOrderState object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOInOrderState&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Purchase Order Status'}de{'Bestellstatus'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDInOrderState.ClassName,
        Description = @"Businessobject for maintaining the ""Purchase Order Status"" value list (master data): the status of a purchase order (header): Created, In Process, Completed, Blocked, Cancelled.
        Call Search() to fill the InOrderStateList property with all entries of the table MDInOrderState.
        Select an entry in the list; it is provided as SelectedInOrderState and CurrentInOrderState for display and editing.
        Property changes should always be made to CurrentInOrderState and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns the new entity object to the CurrentInOrderState property. Fill in all required fields (at least the multilingual Name) before saving.
        Use the Delete() method to delete the current entry provided there are no foreign key relationships from other tables (purchase orders reference this state).
        Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load() method updates the CurrentInOrderState object with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOInOrderState&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class MDBSOInOrderState : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOInOrderState"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOInOrderState(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        ACAccessNav<MDInOrderState> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "MDInOrderState", "en{'Purchase Order Status'}de{'Bestellstatus'}", Description = @"Primary navigation access to the MDInOrderState table; used internally by Search() and list navigation.")]
        public ACAccessNav<MDInOrderState> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDInOrderState>("MDInOrderState", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the state of the selected in order.
        /// </summary>
        /// <value>The state of the selected in order.</value>
        [ACPropertySelected(9999, "MDInOrderState", "en{'Selected Purchase Order Status'}de{'Ausgewählt: Bestellstatus'}", Description = @"The purchase order status currently selected in the list; read-only mirror of the navigation selection.")]
        public MDInOrderState SelectedInOrderState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedInOrderState");
            }
        }

        /// <summary>
        /// Gets or sets the state of the current in order.
        /// </summary>
        /// <value>The state of the current in order.</value>
        [ACPropertyCurrent(9999, "MDInOrderState", "en{'Current Purchase Order Status'}de{'Aktuell: Bestellstatus'}", Description = @"The purchase order status currently being edited. Change its properties and call Save() to persist the changes.")]
        public MDInOrderState CurrentInOrderState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentInOrderState");
            }
        }

        /// <summary>
        /// Gets the in order state list.
        /// </summary>
        /// <value>The in order state list.</value>
        [ACPropertyList(9999, "MDInOrderState", "en{'Purchase Order Statuses'}de{'Bestellstatus'}", Description = @"Result list of the last search; contains all purchase order statuses.")]
        public IEnumerable<MDInOrderState> InOrderStateList
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
        [ACMethodCommand("MDInOrderState", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = @"Saves all changes made to CurrentInOrderState to the database and refreshes the list.")]
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
        [ACMethodCommand("MDInOrderState", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = @"Discards all unsaved changes made to the current purchase order status.")]
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
        [ACMethodInteraction("MDInOrderState", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedInOrderState", Global.ACKinds.MSMethodPrePost, Description = @"Reloads the selected purchase order status from the database, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDInOrderState>(requery, () => SelectedInOrderState, () => CurrentInOrderState, c => CurrentInOrderState = c,
                        DatabaseApp.MDInOrderState
                        .Where(c => c.MDInOrderStateID == SelectedInOrderState.MDInOrderStateID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedInOrderState != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("MDInOrderState", Const.New, (short)MISort.New, true, "SelectedInOrderState", Global.ACKinds.MSMethodPrePost, Description = @"Creates a new purchase order status and assigns it to CurrentInOrderState. Fill in the required fields (at least the multilingual Name) and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentInOrderState = MDInOrderState.NewACObject(DatabaseApp, null);
            DatabaseApp.MDInOrderState.Add(CurrentInOrderState);
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
        [ACMethodInteraction("MDInOrderState", Const.Delete, (short)MISort.Delete, true, "CurrentInOrderState", Global.ACKinds.MSMethodPrePost, Description = @"Deletes the current purchase order status. Only possible if no other records reference it; call Save() afterwards to persist the deletion.")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentInOrderState.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentInOrderState);
            SelectedInOrderState = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(InOrderStateList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentInOrderState != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("MDInOrderState", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = @"Executes the search and fills InOrderStateList with all purchase order statuses.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(InOrderStateList));
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
                // Search has no IsEnabled counterpart → always enabled
                case nameof(Search):
                    return new string[] { nameof(InitState) };
                case nameof(IsEnabledSave):
                    return new string[] { nameof(ACState) };
                case nameof(UndoSave):
                    return new string[] { nameof(ACState) };
                case nameof(IsEnabledUndoSave):
                    return new string[] { nameof(ACState) };
                // Load → ACState + Selected
                case nameof(Load):
                    return new string[] { nameof(ACState), nameof(SelectedInOrderState) };
                case nameof(IsEnabledLoad):
                    return new string[] { nameof(ACState), nameof(SelectedInOrderState) };
                // New → ACState
                case nameof(New):
                    return new string[] { nameof(ACState) };
                case nameof(IsEnabledNew):
                    return new string[] { nameof(ACState) };
                // Delete → Current
                case nameof(Delete):
                    return new string[] { nameof(CurrentInOrderState) };
                case nameof(IsEnabledDelete):
                    return new string[] { nameof(CurrentInOrderState) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            };
        }

        #endregion

    }
}