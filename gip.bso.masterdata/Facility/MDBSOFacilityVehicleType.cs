// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using gip.core.autocomponent;
// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 01-19-2018
// ***********************************************************************
// <copyright file="MDBSOFacilityVehicleType.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.mes.autocomponent;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Business object (value list) for maintaining the "Vehicle Type" master data (table MDFacilityVehicleType).
    /// A vehicle type classifies vehicles used for transport, e.g. semi-trailer truck, semi-trailer, full trailer, ballast tractor or calibration vehicle.
    /// To load the list, call Search(); the database result is copied to the FacilityVehicleTypeList property.
    /// Then call NavigateFirst() to set CurrentFacilityVehicleType with the first record of the list.
    /// CurrentFacilityVehicleType is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentFacilityVehicleType; call Save() to persist them before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns it to CurrentFacilityVehicleType; fill all required fields before calling Save().
    /// Use Delete() to delete the current record provided no foreign key relationships exist from other tables; always call Save() afterwards to execute the delete in the database.
    /// The Load() method updates CurrentFacilityVehicleType with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOFacilityVehicleType&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Vehicle Type'}de{'Fahrzeugtyp'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDFacilityVehicleType.ClassName,
        Description = @"Business object (value list) for maintaining the ""Vehicle Type"" master data (table MDFacilityVehicleType).
        A vehicle type classifies vehicles used for transport, e.g. semi-trailer truck, semi-trailer, full trailer, ballast tractor or calibration vehicle.
        To load the list, call Search(); the database result is copied to the FacilityVehicleTypeList property.
        Then call NavigateFirst() to set CurrentFacilityVehicleType with the first record of the list.
        CurrentFacilityVehicleType is used to display and edit the currently selected record.
        Property changes should always be made to CurrentFacilityVehicleType; call Save() to persist them before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns it to CurrentFacilityVehicleType; fill all required fields before calling Save().
        Use Delete() to delete the current record provided no foreign key relationships exist from other tables; always call Save() afterwards to execute the delete in the database.
        The Load() method updates CurrentFacilityVehicleType with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOFacilityVehicleType&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.")]
    public class MDBSOFacilityVehicleType : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOFacilityVehicleType"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOFacilityVehicleType(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        ACAccessNav<MDFacilityVehicleType> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "MDFacilityVehicleType", Description = @"Primary navigation access to the MDFacilityVehicleType table. Provides the query, the loaded list (NavList) and the currently selected record.")]
        public ACAccessNav<MDFacilityVehicleType> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDFacilityVehicleType>("MDFacilityVehicleType", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the type of the selected facility.
        /// </summary>
        /// <value>The type of the selected facility.</value>
        [ACPropertySelected(9999, "MDFacilityVehicleType", Description = @"The Vehicle Type record currently selected in the list.")]
        public MDFacilityVehicleType SelectedFacilityVehicleType
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
                OnPropertyChanged("SelectedFacilityVehicleType");
            }
        }

        /// <summary>
        /// Gets or sets the type of the current facility.
        /// </summary>
        /// <value>The type of the current facility.</value>
        [ACPropertyCurrent(9999, "MDFacilityVehicleType", Description = @"The Vehicle Type record currently displayed and edited. All changes must be made on this object, followed by Save().")]
        public MDFacilityVehicleType CurrentFacilityVehicleType
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
                OnPropertyChanged("CurrentFacilityVehicleType");
            }
        }

        /// <summary>
        /// Gets the facility type list.
        /// </summary>
        /// <value>The facility type list.</value>
        [ACPropertyList(9999, "MDFacilityVehicleType", Description = @"The loaded Vehicle Type value list (result of Search()).")]
        public IEnumerable<MDFacilityVehicleType> FacilityVehicleTypeList
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
        [ACMethodCommand("MDFacilityVehicleType", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = @"Saves all changes made on CurrentFacilityVehicleType to the database.")]
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
        [ACMethodCommand("MDFacilityVehicleType", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = @"Discards all unsaved changes on CurrentFacilityVehicleType and restores the last saved state.")]
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
        [ACMethodInteraction("MDFacilityVehicleType", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedFacilityVehicleType", Global.ACKinds.MSMethodPrePost, Description = @"Reloads the record selected in FacilityVehicleTypeList from the database into CurrentFacilityVehicleType, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDFacilityVehicleType>(requery, () => SelectedFacilityVehicleType, () => CurrentFacilityVehicleType, c => CurrentFacilityVehicleType = c,
                        DatabaseApp.MDFacilityVehicleType
                        .Where(c => c.MDFacilityVehicleTypeID == SelectedFacilityVehicleType.MDFacilityVehicleTypeID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedFacilityVehicleType != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("MDFacilityVehicleType", Const.New, (short)MISort.New, true, "SelectedFacilityVehicleType", Global.ACKinds.MSMethodPrePost, Description = @"Creates a new Vehicle Type record and assigns it to CurrentFacilityVehicleType. Fill all required fields and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentFacilityVehicleType = MDFacilityVehicleType.NewACObject(DatabaseApp, null);
            DatabaseApp.MDFacilityVehicleType.Add(CurrentFacilityVehicleType);
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
        [ACMethodInteraction("MDFacilityVehicleType", Const.Delete, (short)MISort.Delete, true, "CurrentFacilityVehicleType", Global.ACKinds.MSMethodPrePost, Description = @"Deletes the current Vehicle Type record if no other tables reference it (foreign key check). Call Save() afterwards to execute the delete in the database.")]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (AccessPrimary == null)
                return;
            Msg msg = CurrentFacilityVehicleType.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }


            AccessPrimary.NavList.Remove(CurrentFacilityVehicleType);
            SelectedFacilityVehicleType = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(FacilityVehicleTypeList));
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
        [ACMethodCommand("MDFacilityVehicleType", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = @"Reloads the Vehicle Type value list from the database into FacilityVehicleTypeList.")]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(FacilityVehicleTypeList));
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
                    return new string[] { nameof(ACState), nameof(SelectedFacilityVehicleType) };
                case nameof(Delete):
                    return new string[] { nameof(CurrentFacilityVehicleType) };
                case nameof(New):
                    return new string[] { nameof(ACState) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }
}