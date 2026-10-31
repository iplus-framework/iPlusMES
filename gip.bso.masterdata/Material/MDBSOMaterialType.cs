// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 10.01.2018
// ***********************************************************************
// <copyright file="MDBSOMaterialType.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Value list for material types that further classify materials within a material group (customer-specific classification, e.g. for reporting or filtering).
    /// To search for records, enter the search string in the SearchWord property.
    /// The database result is copied to the MaterialTypeList property.
    /// Then call the NavigateFirst() method to set CurrentMaterialType with the first record in the list.
    /// CurrentMaterialType is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentMaterialType and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns the new entity object to the CurrentMaterialType property. Fill in all required fields before saving.
    /// Use the Delete() method to delete the type provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load() method updates the CurrentMaterialType object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOMaterialType&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Material Type'}de{'Materialart'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDMaterialType.ClassName,
        Description = @"Value list for material types that further classify materials within a material group (customer-specific classification, e.g. for reporting or filtering).
        To search for records, enter the search string in the SearchWord property.
        The database result is copied to the MaterialTypeList property.
        Then call the NavigateFirst() method to set CurrentMaterialType with the first record in the list.
        CurrentMaterialType is used to display and edit the currently selected record.
        Property changes should always be made to CurrentMaterialType and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns the new entity object to the CurrentMaterialType property. Fill in all required fields before saving.
        Use the Delete() method to delete the type provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load() method updates the CurrentMaterialType object with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOMaterialType&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class MDBSOMaterialType : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOMaterialType"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOMaterialType(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            //OnValueChanged += new ValueChangedEventHandler(BSOMaterialType_OnValueChanged);

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

        /// <summary>
        /// Gets the current AC class.
        /// </summary>
        /// <value>The current AC class.</value>
        public gip.core.datamodel.ACClass CurrentACClass
        {
            get
            {
                return DatabaseApp.ACClass.Where(c => c.ACIdentifier == "Root").FirstOrDefault().FromIPlusContext<gip.core.datamodel.ACClass>();
            }
        }

        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<MDMaterialType> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDMaterialType.ClassName, Description = "Gets the primary navigation access to the material type entities.")]
        public ACAccessNav<MDMaterialType> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null
                    && InitState != ACInitState.Destructing && InitState != ACInitState.Destructed && InitState != ACInitState.DisposedToPool && InitState != ACInitState.DisposingToPool)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDMaterialType>(MDMaterialType.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the type of the selected material.
        /// </summary>
        /// <value>The type of the selected material.</value>
        [ACPropertySelected(9999, MDMaterialType.ClassName, Description = "Gets or sets the material type currently selected in the list.")]
        public MDMaterialType SelectedMaterialType
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                CurrentMaterialType = SelectedMaterialType;
                OnPropertyChanged("SelectedMaterialType");
            }
        }

        /// <summary>
        /// Gets or sets the type of the current material.
        /// </summary>
        /// <value>The type of the current material.</value>
        [ACPropertyCurrent(9999, MDMaterialType.ClassName, Description = "Gets or sets the current material type. Property changes are made on this record before calling Save().")]
        public MDMaterialType CurrentMaterialType
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentMaterialType");
            }
        }

        /// <summary>
        /// Gets the material type list.
        /// </summary>
        /// <value>The material type list.</value>
        [ACPropertyList(9999, MDMaterialType.ClassName, Description = "Gets the list of material types resulting from the search.")]
        public IEnumerable<MDMaterialType> MaterialTypeList
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
        [ACMethodCommand(MDMaterialType.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Saves the changes made to the current material type to the database and refreshes the list.
                           Always call Save() before navigating to another record or creating a new one.")]
        public void Save()
        {
            OnSave();
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
        [ACMethodCommand(MDMaterialType.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Discards all unsaved changes made to the current material type and restores the database state.")]
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
        [ACMethodInteraction(MDMaterialType.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMaterialType", Global.ACKinds.MSMethodPrePost, Description =
                         @"Reloads the selected material type from the database with fresh data, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDMaterialType>(requery, () => SelectedMaterialType, () => CurrentMaterialType, c => CurrentMaterialType = c,
                        DatabaseApp.MDMaterialType
                        .Where(c => c.MDMaterialTypeID == SelectedMaterialType.MDMaterialTypeID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedMaterialType != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDMaterialType.ClassName, Const.New, (short)MISort.New, true, "SelectedMaterialType", Global.ACKinds.MSMethodPrePost, Description =
                         @"Creates a new material type and assigns it to the CurrentMaterialType property. Fill in all required fields and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            SelectedMaterialType = MDMaterialType.NewACObject(DatabaseApp, null);
            DatabaseApp.MDMaterialType.Add(SelectedMaterialType);
            AccessPrimary.NavList.Add(SelectedMaterialType);
            ACState = Const.SMNew;
            OnPropertyChanged("MaterialTypeList");
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
        [ACMethodInteraction(MDMaterialType.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentMaterialType", Global.ACKinds.MSMethodPrePost, Description =
                         @"Deletes the current material type, provided no other tables reference it (foreign key check). Confirm the deletion with Save().")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentMaterialType.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentMaterialType);
            SelectedMaterialType = AccessPrimary.NavList.FirstOrDefault();
            Load();
            OnPropertyChanged("MaterialTypeList");
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentMaterialType != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDMaterialType.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description =
                         @"Executes the search and refreshes the MaterialTypeList with the database result.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("MaterialTypeList");
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
                    return new string[] { nameof(ACState), nameof(SelectedMaterialType) };
                case nameof(New):
                    return new string[] { nameof(ACState) };
                case nameof(Delete):
                    return new string[] { nameof(CurrentMaterialType) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }
}