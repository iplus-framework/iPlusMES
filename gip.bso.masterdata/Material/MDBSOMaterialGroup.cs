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
// <copyright file="MDBSOMaterialGroup.cs" company="gip mbh, Oftersheim, Germany">
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
using System.IO;
using System;
using gip.core.media;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Value list for material groups that classify materials by their role in the value chain (raw material, intermediate/semi-finished/finished product, packaging, service, ...).
    /// To search for records, enter the search string in the SearchWord property.
    /// The database result is copied to the MaterialGroupList property.
    /// Then call the NavigateFirst() method to set CurrentMaterialGroup with the first record in the list.
    /// CurrentMaterialGroup is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentMaterialGroup and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns the new entity object to the CurrentMaterialGroup property. Fill in all required fields before saving.
    /// Use the Delete() method to delete the group provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load() method updates the CurrentMaterialGroup object with fresh database data if another user has made changes in the background.
    /// Special functions: ShowImages configuration switches image display for the groups on or off (media via BSOMedia_Child).
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOMaterialGroup&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Material Group'}de{'Materialgruppe'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDMaterialGroup.ClassName,
        Description = @"Value list for material groups that classify materials by their role in the value chain (raw material, intermediate/semi-finished/finished product, packaging, service, ...).
        To search for records, enter the search string in the SearchWord property.
        The database result is copied to the MaterialGroupList property.
        Then call the NavigateFirst() method to set CurrentMaterialGroup with the first record in the list.
        CurrentMaterialGroup is used to display and edit the currently selected record.
        Property changes should always be made to CurrentMaterialGroup and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns the new entity object to the CurrentMaterialGroup property. Fill in all required fields before saving.
        Use the Delete() method to delete the group provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load() method updates the CurrentMaterialGroup object with fresh database data if another user has made changes in the background.
        Special functions: ShowImages configuration switches image display for the groups on or off (media via BSOMedia_Child).
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOMaterialGroup&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class MDBSOMaterialGroup : ACBSOvbNav
    {
        
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOMaterialGroup"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOMaterialGroup(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
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

            _ShowImages = new ACPropertyConfigValue<bool>(this, nameof(ShowImages), false);

            MediaController = ACMediaController.GetServiceInstance(this);

            if (BSOMedia_Child != null && BSOMedia_Child.Value != null)
                BSOMedia_Child.Value.OnDefaultImageDelete += Value_OnDefaultImageDelete;

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
           
            if (BSOMedia_Child != null && BSOMedia_Child.Value != null)
                BSOMedia_Child.Value.OnDefaultImageDelete -= Value_OnDefaultImageDelete;

            MediaController = null;
            return b;
        }

        private void Value_OnDefaultImageDelete(object sender, EventArgs e)
        {
            OnPropertyChanged("MaterialGroupList");
        }


        #endregion

        #region Managers
        
        public ACMediaController MediaController { get; set; }

        #endregion

        #region Child BSO
        ACChildItem<BSOMedia> _BSOMedia_Child;
        [ACPropertyInfo(9999, Description = "Gets the child BSO for managing media (images) attached to the material groups.")]
        [ACChildInfo("BSOMedia_Child", typeof(BSOMedia))]
        public ACChildItem<BSOMedia> BSOMedia_Child
        {
            get
            {
                if (_BSOMedia_Child == null)
                    _BSOMedia_Child = new ACChildItem<BSOMedia>(this, "BSOMedia_Child");
                return _BSOMedia_Child;
            }
        }
        #endregion

        #region Configuration
        private ACPropertyConfigValue<bool> _ShowImages;
        [ACPropertyConfig("en{'Show images'}de{'Bilder anzeigen'}", Description = "Configuration switch: shows or hides the images of the material groups in the user interface.")]
        public bool ShowImages
        {
            get
            {
                return _ShowImages.ValueT;
            }
            set
            {
                _ShowImages.ValueT = value;
            }
        }
        #endregion

        #region BSO->ACProperty
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<MDMaterialGroup> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDMaterialGroup.ClassName, Description = "Gets the primary navigation access to the material group entities.")]
        public ACAccessNav<MDMaterialGroup> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDMaterialGroup>(MDMaterialGroup.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected material group.
        /// </summary>
        /// <value>The selected material group.</value>
        [ACPropertySelected(9999, MDMaterialGroup.ClassName, Description = "Gets or sets the material group currently selected in the list.")]
        public MDMaterialGroup SelectedMaterialGroup
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                CurrentMaterialGroup = SelectedMaterialGroup;
                OnPropertyChanged("SelectedMaterialGroup");
            }
        }

        /// <summary>
        /// Gets or sets the current material group.
        /// </summary>
        /// <value>The current material group.</value>
        [ACPropertyCurrent(9999, MDMaterialGroup.ClassName, Description = "Gets or sets the current material group. Property changes are made on this record before calling Save().")]
        public MDMaterialGroup CurrentMaterialGroup
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return;
                if (AccessPrimary.Current != value)
                {
                    if (BSOMedia_Child != null && BSOMedia_Child.Value != null)
                    {
                        BSOMedia_Child.Value.LoadMedia(value);
                    }

                    AccessPrimary.Current = value;
                    OnPropertyChanged("CurrentMaterialGroup");
                }
            }
        }

        /// <summary>
        /// Gets the material group list.
        /// </summary>
        /// <value>The material group list.</value>
        [ACPropertyList(9999, MDMaterialGroup.ClassName, Description = "Gets the list of material groups resulting from the search.")]
        public List<MDMaterialGroup> MaterialGroupList
        {
            get
            {
                var groupList = AccessPrimary.NavList.ToList();
                if (ShowImages)
                {
                    foreach (var item in groupList)
                    {
                        MediaController.LoadIImageInfo(item);
                    }
                }
                return groupList;
            }
        }

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(MDMaterialGroup.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Saves the changes made to the current material group to the database and refreshes the list.
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
        [ACMethodCommand(MDMaterialGroup.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Discards all unsaved changes made to the current material group and restores the database state.")]
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
        [ACMethodInteraction(MDMaterialGroup.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedMaterialGroup", Global.ACKinds.MSMethodPrePost, Description =
                         @"Reloads the selected material group from the database with fresh data, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDMaterialGroup>(requery, () => SelectedMaterialGroup, () => CurrentMaterialGroup, c => CurrentMaterialGroup = c,
                        DatabaseApp.MDMaterialGroup
                        .Where(c => c.MDMaterialGroupID == SelectedMaterialGroup.MDMaterialGroupID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedMaterialGroup != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDMaterialGroup.ClassName, Const.New, (short)MISort.New, true, "SelectedMaterialGroup", Global.ACKinds.MSMethodPrePost, Description =
                         @"Creates a new material group and assigns it to the CurrentMaterialGroup property. Fill in all required fields and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            SelectedMaterialGroup = MDMaterialGroup.NewACObject(DatabaseApp, null);
            DatabaseApp.MDMaterialGroup.Add(SelectedMaterialGroup);
            AccessPrimary.NavList.Add(SelectedMaterialGroup);
            ACState = Const.SMNew;
            OnPropertyChanged("MaterialGroupList");
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
        [ACMethodInteraction(MDMaterialGroup.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentMaterialGroup", Global.ACKinds.MSMethodPrePost, Description =
                         @"Deletes the current material group, provided no other tables reference it (foreign key check). Confirm the deletion with Save().")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentMaterialGroup.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentMaterialGroup);
            SelectedMaterialGroup = AccessPrimary.NavList.FirstOrDefault();
            Load();
            OnPropertyChanged("MaterialGroupList");
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentMaterialGroup != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDMaterialGroup.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description =
                         @"Executes the search and refreshes the MaterialGroupList with the database result.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("MaterialGroupList");
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
                    return new string[] { nameof(ACState), nameof(SelectedMaterialGroup) };
                case nameof(Delete):
                    return new string[] { nameof(CurrentMaterialGroup) };
                case nameof(New):
                    return new string[] { nameof(ACState) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }
}