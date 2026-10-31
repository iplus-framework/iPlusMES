// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 19.01.2018
// ***********************************************************************
// <copyright file="MDBSOOutOfferState.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Business object for maintaining the offer state (table MDOutOfferState). Offer states track the processing status of a customer offer (New, Sended, Completed, Cancelled).
    /// To load the records, call the Search() method; the database result is copied to the MDOutOfferStateList property.
    /// Then call the NavigateFirst() method to set CurrentMDOutOfferState with the first record in the list.
    /// CurrentMDOutOfferState is used to display and edit the currently selected record; SelectedMDOutOfferState reflects the selection in the list.
    /// Property changes should always be made to CurrentMDOutOfferState and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns the new entity object to the CurrentMDOutOfferState property. Fill in all required fields (at least the multilingual name) before saving.
    /// Use the Delete() method to delete the record provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load() method updates the CurrentMDOutOfferState object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOOutOfferState&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSales, "en{'Offering Status'}de{'Angebotsstatus'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDOutOfferState.ClassName,
        Description = @"Business object for maintaining the offer state (table MDOutOfferState). Offer states track the processing status of a customer offer (New, Sended, Completed, Cancelled).
        To load the records, call the Search() method; the database result is copied to the MDOutOfferStateList property.
        Then call the NavigateFirst() method to set CurrentMDOutOfferState with the first record in the list.
        CurrentMDOutOfferState is used to display and edit the currently selected record; SelectedMDOutOfferState reflects the selection in the list.
        Property changes should always be made to CurrentMDOutOfferState and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns the new entity object to the CurrentMDOutOfferState property. Fill in all required fields (at least the multilingual name) before saving.
        Use the Delete() method to delete the record provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load() method updates the CurrentMDOutOfferState object with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOOutOfferState&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.")]
    public class MDBSOOutOfferState : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOOutOfferState"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOOutOfferState(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        ACAccessNav<MDOutOfferState> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "MDOutOfferState", Description = "Navigation access to the offer state value list. NavSearch() fills the list; NavigateFirst() sets CurrentMDOutOfferState.")]
        public ACAccessNav<MDOutOfferState> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDOutOfferState>("MDOutOfferState", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the state of the selected out offering.
        /// </summary>
        /// <value>The state of the selected out offering.</value>
        [ACPropertySelected(9999, "MDOutOfferState", Description = "Selected offer state record in the list; changing it updates the detail view.")]
        public MDOutOfferState SelectedOutOfferState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedOutOfferState");
            }
        }

        /// <summary>
        /// Gets or sets the state of the current out offering.
        /// </summary>
        /// <value>The state of the current out offering.</value>
        [ACPropertyCurrent(9999, "MDOutOfferState", Description = "Current offer state record that is displayed and edited; call Save() after changing its fields.")]
        public MDOutOfferState CurrentOutOfferState
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentOutOfferState");
            }
        }

        /// <summary>
        /// Gets the out offering state list.
        /// </summary>
        /// <value>The out offering state list.</value>
        [ACPropertyList(9999, "MDOutOfferState", Description = "Result list of the offer state search.")]
        public IEnumerable<MDOutOfferState> OutOfferStateList
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
        [ACMethodCommand("MDOutOfferState", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand("MDOutOfferState", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction("MDOutOfferState", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedOutOfferState", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDOutOfferState>(requery, () => SelectedOutOfferState, () => CurrentOutOfferState, c => CurrentOutOfferState = c,
                        DatabaseApp.MDOutOfferState
                        .Where(c => c.MDOutOfferStateID == SelectedOutOfferState.MDOutOfferStateID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedOutOfferState != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("MDOutOfferState", Const.New, (short)MISort.New, true, "SelectedOutOfferState", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentOutOfferState = MDOutOfferState.NewACObject(DatabaseApp, null);
            DatabaseApp.MDOutOfferState.Add(CurrentOutOfferState);
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
        [ACMethodInteraction("MDOutOfferState", Const.Delete, (short)MISort.Delete, true, "CurrentOutOfferState")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentOutOfferState.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentOutOfferState);
            SelectedOutOfferState = AccessPrimary.NavList.FirstOrDefault();
            Load();

            PostExecute("Delete");
            OnPropertyChanged(nameof(OutOfferStateList));
        }

        /// <summary>
        /// Determines whether [is enabled delete out offering state].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete out offering state]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteOutOfferState()
        {
            return CurrentOutOfferState != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("MDOutOfferState", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(OutOfferStateList));
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
                case nameof(IsEnabledDeleteOutOfferState):
                    result = IsEnabledDeleteOutOfferState();
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
                #region Always Enabled
                case nameof(Search):
                    return new string[] { nameof(InitState) };
                #endregion

                #region Load
                case nameof(Load):
                case nameof(IsEnabledLoad):
                    return new string[] { nameof(ACState), nameof(SelectedOutOfferState) };
                #endregion

                #region New
                case nameof(New):
                case nameof(IsEnabledNew):
                    return new string[] { nameof(ACState) };
                #endregion

                #region Delete
                case nameof(Delete):
                case nameof(IsEnabledDeleteOutOfferState):
                    return new string[] { nameof(CurrentOutOfferState) };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

    }
}