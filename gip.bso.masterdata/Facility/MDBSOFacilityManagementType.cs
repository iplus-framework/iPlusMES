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
// <copyright file="MDBSOFacilityManagementType.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Business object (value list) for maintaining the "Stock Management Type" master data (table MDFacilityManagementType).
    /// A stock management type defines whether and how a facility manages stock: no management, management with or without batches, or management with lot reservation.
    /// To load the list, call Search(); the database result is copied to the FacilityManagementTypeList property.
    /// Then call NavigateFirst() to set CurrentFacilityManagementType with the first record of the list.
    /// CurrentFacilityManagementType is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentFacilityManagementType; call Save() to persist them before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns it to CurrentFacilityManagementType; fill all required fields before calling Save().
    /// Use Delete() to delete the current record provided no foreign key relationships exist from other tables; always call Save() afterwards to execute the delete in the database.
    /// The Load() method updates CurrentFacilityManagementType with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOFacilityManagementType&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Stock Management Type'}de{'Lagerführungsart'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDFacilityManagementType.ClassName,
        Description = @"Business object (value list) for maintaining the ""Stock Management Type"" master data (table MDFacilityManagementType).
        A stock management type defines whether and how a facility manages stock: no management, management with or without batches, or management with lot reservation.
        To load the list, call Search(); the database result is copied to the FacilityManagementTypeList property.
        Then call NavigateFirst() to set CurrentFacilityManagementType with the first record of the list.
        CurrentFacilityManagementType is used to display and edit the currently selected record.
        Property changes should always be made to CurrentFacilityManagementType; call Save() to persist them before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns it to CurrentFacilityManagementType; fill all required fields before calling Save().
        Use Delete() to delete the current record provided no foreign key relationships exist from other tables; always call Save() afterwards to execute the delete in the database.
        The Load() method updates CurrentFacilityManagementType with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOFacilityManagementType&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.")]
    public class MDBSOFacilityManagementType : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOFacilityManagementType"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOFacilityManagementType(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        ACAccessNav<MDFacilityManagementType> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "MDFacilityManagementType", Description = @"Primary navigation access to the MDFacilityManagementType table. Provides the query, the loaded list (NavList) and the currently selected record.")]
        public ACAccessNav<MDFacilityManagementType> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDFacilityManagementType>("MDFacilityManagementType", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the type of the selected facility management.
        /// </summary>
        /// <value>The type of the selected facility management.</value>
        [ACPropertySelected(9999, "MDFacilityManagementType", Description = @"The Stock Management Type record currently selected in the list.")]
        public MDFacilityManagementType SelectedFacilityManagementType
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
                OnPropertyChanged("SelectedFacilityManagementType");
            }
        }

        /// <summary>
        /// Gets or sets the type of the current facility management.
        /// </summary>
        /// <value>The type of the current facility management.</value>
        [ACPropertyCurrent(9999, "MDFacilityManagementType", Description = @"The Stock Management Type record currently displayed and edited. All changes must be made on this object, followed by Save().")]
        public MDFacilityManagementType CurrentFacilityManagementType
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
                OnPropertyChanged("CurrentFacilityManagementType");
            }
        }

        /// <summary>
        /// Gets the facility management type list.
        /// </summary>
        /// <value>The facility management type list.</value>
        [ACPropertyList(9999, "MDFacilityManagementType", Description = @"The loaded Stock Management Type value list (result of Search()).")]
        public IEnumerable<MDFacilityManagementType> FacilityManagementTypeList
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
        [ACMethodCommand("MDFacilityManagementType", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = @"Saves all changes made on CurrentFacilityManagementType to the database.")]
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
        [ACMethodCommand("MDFacilityManagementType", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = @"Discards all unsaved changes on CurrentFacilityManagementType and restores the last saved state.")]
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
        [ACMethodInteraction("MDFacilityManagementType", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacilityManagementType", Global.ACKinds.MSMethodPrePost, Description = @"Reloads the record selected in FacilityManagementTypeList from the database into CurrentFacilityManagementType, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDFacilityManagementType>(requery, () => SelectedFacilityManagementType, () => CurrentFacilityManagementType, c => CurrentFacilityManagementType = c,
                        DatabaseApp.MDFacilityManagementType
                        .Where(c => c.MDFacilityManagementTypeID == SelectedFacilityManagementType.MDFacilityManagementTypeID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedFacilityManagementType != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("MDFacilityManagementType", Const.New, (short)MISort.New, true, "SelectedFacilityManagementType", Global.ACKinds.MSMethodPrePost, Description = @"Creates a new Stock Management Type record and assigns it to CurrentFacilityManagementType. Fill all required fields and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentFacilityManagementType = MDFacilityManagementType.NewACObject(DatabaseApp, null);
            DatabaseApp.MDFacilityManagementType.Add(CurrentFacilityManagementType);
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
        [ACMethodInteraction("MDFacilityManagementType", Const.Delete, (short)MISort.Delete, true, "CurrentFacilityManagementType", Global.ACKinds.MSMethodPrePost, Description = @"Deletes the current Stock Management Type record if no other tables reference it (foreign key check). Call Save() afterwards to execute the delete in the database.")]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (AccessPrimary == null)
                return;
            Msg msg = CurrentFacilityManagementType.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            AccessPrimary.NavList.Remove(CurrentFacilityManagementType);
            SelectedFacilityManagementType = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(FacilityManagementTypeList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return true;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("MDFacilityManagementType", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = @"Reloads the Stock Management Type value list from the database into FacilityManagementTypeList.")]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(FacilityManagementTypeList));
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
                    return new string[] { nameof(ACState), nameof(SelectedFacilityManagementType) };
                case nameof(New):
                    return new string[] { nameof(ACState) };
                case nameof(Delete):
                    return new string[] { nameof(CurrentFacilityManagementType) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }
}