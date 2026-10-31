// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 01-18-2018
// ***********************************************************************
// <copyright file="MDBSOBookingNotAvailableMode.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Business object (value list) for maintaining the "Posting Not Available Mode" master data (table MDBookingNotAvailableMode).
    /// A posting-not-available mode defines how the NotAvailable flag of a facility or facility charge is automatically set and/or reset when bookings are posted on unavailable stock.
    /// To load the list, call Search(); the database result is copied to the BookingNotAvailableModeList property.
    /// Then call NavigateFirst() to set CurrentBookingNotAvailableMode with the first record of the list.
    /// CurrentBookingNotAvailableMode is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentBookingNotAvailableMode; call Save() to persist them before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns it to CurrentBookingNotAvailableMode; fill all required fields before calling Save().
    /// Use Delete() to delete the current record provided no foreign key relationships exist from other tables; always call Save() afterwards to execute the delete in the database.
    /// The Load() method updates CurrentBookingNotAvailableMode with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOBookingNotAvailableMode&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Posting Not Available Mode'}de{'Modus bei Nichverfügbarkeit'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDBookingNotAvailableMode.ClassName,
        Description = @"Business object (value list) for maintaining the ""Posting Not Available Mode"" master data (table MDBookingNotAvailableMode).
        A posting-not-available mode defines how the NotAvailable flag of a facility or facility charge is automatically set and/or reset when bookings are posted on unavailable stock.
        To load the list, call Search(); the database result is copied to the BookingNotAvailableModeList property.
        Then call NavigateFirst() to set CurrentBookingNotAvailableMode with the first record of the list.
        CurrentBookingNotAvailableMode is used to display and edit the currently selected record.
        Property changes should always be made to CurrentBookingNotAvailableMode; call Save() to persist them before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns it to CurrentBookingNotAvailableMode; fill all required fields before calling Save().
        Use Delete() to delete the current record provided no foreign key relationships exist from other tables; always call Save() afterwards to execute the delete in the database.
        The Load() method updates CurrentBookingNotAvailableMode with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOBookingNotAvailableMode&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.")]
    public class MDBSOBookingNotAvailableMode : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOBookingNotAvailableMode"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOBookingNotAvailableMode(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        ACAccessNav<MDBookingNotAvailableMode> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDBookingNotAvailableMode.ClassName, Description = @"Primary navigation access to the MDBookingNotAvailableMode table. Provides the query, the loaded list (NavList) and the currently selected record.")]
        public ACAccessNav<MDBookingNotAvailableMode> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDBookingNotAvailableMode>(MDBookingNotAvailableMode.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected booking not available mode.
        /// </summary>
        /// <value>The selected booking not available mode.</value>
        [ACPropertySelected(9999, MDBookingNotAvailableMode.ClassName, Description = @"The Posting Not Available Mode record currently selected in the list.")]
        public MDBookingNotAvailableMode SelectedBookingNotAvailableMode
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
                OnPropertyChanged("SelectedBookingNotAvailableMode");
            }
        }

        /// <summary>
        /// Gets or sets the current booking not available mode.
        /// </summary>
        /// <value>The current booking not available mode.</value>
        [ACPropertyCurrent(9999, MDBookingNotAvailableMode.ClassName, Description = @"The Posting Not Available Mode record currently displayed and edited. All changes must be made on this object, followed by Save().")]
        public MDBookingNotAvailableMode CurrentBookingNotAvailableMode
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
                OnPropertyChanged("CurrentBookingNotAvailableMode");
            }
        }

        /// <summary>
        /// Gets the booking not available mode list.
        /// </summary>
        /// <value>The booking not available mode list.</value>
        [ACPropertyList(9999, MDBookingNotAvailableMode.ClassName, Description = @"The loaded Posting Not Available Mode value list (result of Search()).")]
        public IEnumerable<MDBookingNotAvailableMode> BookingNotAvailableModeList
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
        [ACMethodCommand(MDBookingNotAvailableMode.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = @"Saves all changes made on CurrentBookingNotAvailableMode to the database.")]
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
        [ACMethodCommand(MDBookingNotAvailableMode.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = @"Discards all unsaved changes on CurrentBookingNotAvailableMode and restores the last saved state.")]
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
        [ACMethodInteraction(MDBookingNotAvailableMode.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedBookingNotAvailableMode", Global.ACKinds.MSMethodPrePost, Description = @"Reloads the record selected in BookingNotAvailableModeList from the database into CurrentBookingNotAvailableMode, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDBookingNotAvailableMode>(requery, () => SelectedBookingNotAvailableMode, () => CurrentBookingNotAvailableMode, c => CurrentBookingNotAvailableMode = c,
                        DatabaseApp.MDBookingNotAvailableMode
                        .Where(c => c.MDBookingNotAvailableModeID == SelectedBookingNotAvailableMode.MDBookingNotAvailableModeID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedBookingNotAvailableMode != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDBookingNotAvailableMode.ClassName, Const.New, (short)MISort.New, true, "SelectedBookingNotAvailableMode", Global.ACKinds.MSMethodPrePost, Description = @"Creates a new Posting Not Available Mode record and assigns it to CurrentBookingNotAvailableMode. Fill all required fields and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentBookingNotAvailableMode = MDBookingNotAvailableMode.NewACObject(DatabaseApp, null);
            DatabaseApp.MDBookingNotAvailableMode.Add(CurrentBookingNotAvailableMode);
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
        [ACMethodInteraction(MDBookingNotAvailableMode.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentBookingNotAvailableMode", Global.ACKinds.MSMethodPrePost, Description = @"Deletes the current Posting Not Available Mode record if no other tables reference it (foreign key check). Call Save() afterwards to execute the delete in the database.")]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (AccessPrimary == null)
                return;
            Msg msg = CurrentBookingNotAvailableMode.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            AccessPrimary.NavList.Remove(CurrentBookingNotAvailableMode);
            SelectedBookingNotAvailableMode = AccessPrimary.NavList.FirstOrDefault();
            Load();

            PostExecute("Delete");
            OnPropertyChanged(nameof(BookingNotAvailableModeList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentBookingNotAvailableMode != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDBookingNotAvailableMode.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = @"Reloads the Posting Not Available Mode value list from the database into BookingNotAvailableModeList.")]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(BookingNotAvailableModeList));
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

        public override IEnumerable<string> GetPropsToObserveForIsEnabled(string acMethodName)
        {
            switch (acMethodName)
            {
                case nameof(Search):
                    return new string[] { nameof(InitState) };
                case nameof(Load):
                    return new string[] { nameof(ACState), nameof(SelectedBookingNotAvailableMode) };
                case nameof(New):
                    return new string[] { nameof(ACState) };
                case nameof(Delete):
                    return new string[] { nameof(CurrentBookingNotAvailableMode) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }
}