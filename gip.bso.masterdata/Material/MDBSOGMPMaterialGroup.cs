// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 01-11-2018
// ***********************************************************************
// <copyright file="MDBSOGMPMaterialGroup.cs" company="gip mbh, Oftersheim, Germany">
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
    /// Value list for GMP material groups (animal species groups such as dairy cattle, pigs, poultry, horses) used to group materials for GMP/HACCP documentation.
    /// Each group can contain a list of ingredients (GMPMaterialGroupPos) that apply to the materials of this group.
    /// To search for records, enter the search string in the SearchWord property.
    /// The database result is copied to the GMPMaterialGroupList property.
    /// Then call the NavigateFirst() method to set CurrentGMPMaterialGroup with the first record in the list.
    /// CurrentGMPMaterialGroup is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentGMPMaterialGroup and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns the new entity object to the CurrentGMPMaterialGroup property. Fill in all required fields before saving.
    /// Use the Delete() method to delete the group provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load() method updates the CurrentGMPMaterialGroup object with fresh database data if another user has made changes in the background.
    /// Special functions: New Ingredient / Delete Ingredient maintain the ingredient positions (GMPMaterialGroupPos) of the current group.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOGMPMaterialGroup&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'GMP-Material Group'}de{'GMP-Materialgruppe'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDGMPMaterialGroup.ClassName,
        Description = @"Value list for GMP material groups (animal species groups such as dairy cattle, pigs, poultry, horses) used to group materials for GMP/HACCP documentation.
        Each group can contain a list of ingredients (GMPMaterialGroupPos) that apply to the materials of this group.
        To search for records, enter the search string in the SearchWord property.
        The database result is copied to the GMPMaterialGroupList property.
        Then call the NavigateFirst() method to set CurrentGMPMaterialGroup with the first record in the list.
        CurrentGMPMaterialGroup is used to display and edit the currently selected record.
        Property changes should always be made to CurrentGMPMaterialGroup and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns the new entity object to the CurrentGMPMaterialGroup property. Fill in all required fields before saving.
        Use the Delete() method to delete the group provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load() method updates the CurrentGMPMaterialGroup object with fresh database data if another user has made changes in the background.
        Special functions: New Ingredient / Delete Ingredient maintain the ingredient positions (GMPMaterialGroupPos) of the current group.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOGMPMaterialGroup&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class MDBSOGMPMaterialGroup : ACBSOvbNav
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOGMPMaterialGroup"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOGMPMaterialGroup(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            // Workaround
            var xx = DatabaseApp.MDGMPAdditive.ToList();
            Search();
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._AccessMDGMPMaterialGroupPos = null;
            this._CurrentGMPMaterialGroupPos = null;
            this._SelectedGMPMaterialGroupPos = null;
            var b = await base.ACDeInit(deleteACClassTask);
            if (_AccessMDGMPMaterialGroupPos != null)
            {
                await _AccessMDGMPMaterialGroupPos.ACDeInit(false);
                _AccessMDGMPMaterialGroupPos = null;
            }
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
        ACAccessNav<MDGMPMaterialGroup> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDGMPMaterialGroup.ClassName, Description = "Gets the primary navigation access to the GMP material group entities.")]
        public ACAccessNav<MDGMPMaterialGroup> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDGMPMaterialGroup>(MDGMPMaterialGroup.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the current GMP material group.
        /// </summary>
        /// <value>The current GMP material group.</value>
        [ACPropertyCurrent(9999, MDGMPMaterialGroup.ClassName, Description = "Gets or sets the current GMP material group. Property changes are made on this record before calling Save().")]
        public MDGMPMaterialGroup CurrentGMPMaterialGroup
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentGMPMaterialGroup");
            }
        }

        /// <summary>
        /// Gets or sets the selected GMP material group.
        /// </summary>
        /// <value>The selected GMP material group.</value>
        [ACPropertySelected(9999, MDGMPMaterialGroup.ClassName, Description = "Gets or sets the GMP material group currently selected in the list.")]
        public MDGMPMaterialGroup SelectedGMPMaterialGroup
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedGMPMaterialGroup");
            }
        }

        /// <summary>
        /// Gets the GMP material group list.
        /// </summary>
        /// <value>The GMP material group list.</value>
        [ACPropertyList(9999, MDGMPMaterialGroup.ClassName, Description = "Gets the list of GMP material groups resulting from the search.")]
        public IEnumerable<MDGMPMaterialGroup> GMPMaterialGroupList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// The _ access MDGMP material group pos
        /// </summary>
        ACAccess<MDGMPMaterialGroupPos> _AccessMDGMPMaterialGroupPos;
        /// <summary>
        /// Gets the access GMP material group pos.
        /// </summary>
        /// <value>The access GMP material group pos.</value>
        [ACPropertyAccessPrimary(9999, "MDGMPMaterialGroupPos", Description = "Gets the access to the ingredient positions (MDGMPMaterialGroupPos) of the current GMP material group.")]
        public ACAccess<MDGMPMaterialGroupPos> AccessGMPMaterialGroupPos
        {
            get
            {
                if (_AccessMDGMPMaterialGroupPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + "MDGMPMaterialGroupPos") as ACQueryDefinition;
                    _AccessMDGMPMaterialGroupPos = acQueryDefinition.NewAccess<MDGMPMaterialGroupPos>("MDGMPMaterialGroupPos", this);
                }
                return _AccessMDGMPMaterialGroupPos;
            }
        }

        /// <summary>
        /// The _ selected GMP material group pos
        /// </summary>
        MDGMPMaterialGroupPos _SelectedGMPMaterialGroupPos;
        /// <summary>
        /// Gets or sets the selected GMP material group pos.
        /// </summary>
        /// <value>The selected GMP material group pos.</value>
        [ACPropertySelected(9999, "MDGMPMaterialGroupPos", Description = "Gets or sets the ingredient position currently selected in the ingredient list of the current group.")]
        public MDGMPMaterialGroupPos SelectedGMPMaterialGroupPos
        {
            get
            {
                return _SelectedGMPMaterialGroupPos;
            }
            set
            {
                _SelectedGMPMaterialGroupPos = value;
                OnPropertyChanged("SelectedGMPMaterialGroupPos");
            }
        }

        /// <summary>
        /// The _ current GMP material group pos
        /// </summary>
        MDGMPMaterialGroupPos _CurrentGMPMaterialGroupPos;
        /// <summary>
        /// Gets or sets the current GMP material group pos.
        /// </summary>
        /// <value>The current GMP material group pos.</value>
        [ACPropertyCurrent(9999, "MDGMPMaterialGroupPos", Description = "Gets or sets the current ingredient position of the current GMP material group.")]
        public MDGMPMaterialGroupPos CurrentGMPMaterialGroupPos
        {
            get
            {
                return _CurrentGMPMaterialGroupPos;
            }
            set
            {
                _CurrentGMPMaterialGroupPos = value;
                OnPropertyChanged("CurrentGMPMaterialGroupPos");
            }
        }

        /// <summary>
        /// Gets the GMP material group pos list.
        /// </summary>
        /// <value>The GMP material group pos list.</value>
        [ACPropertyList(9999, "MDGMPMaterialGroupPos", Description = "Gets the ingredient positions (MDGMPMaterialGroupPos) of the current GMP material group.")]
        public IEnumerable<MDGMPMaterialGroupPos> GMPMaterialGroupPosList
        {
            get
            {
                if (CurrentGMPMaterialGroup == null)
                    return null;
                return CurrentGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup;
            }
        }

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(MDGMPMaterialGroup.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Saves the changes made to the current GMP material group and its ingredient positions to the database and refreshes the list.
                           Always call Save() before navigating to another record or creating a new one.")]
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
        [ACMethodCommand(MDGMPMaterialGroup.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Discards all unsaved changes made to the current GMP material group and restores the database state.")]
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
        [ACMethodInteraction(MDGMPMaterialGroup.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedGMPMaterialGroup", Global.ACKinds.MSMethodPrePost, Description =
                         @"Reloads the selected GMP material group from the database with fresh data, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDGMPMaterialGroup>(requery, () => SelectedGMPMaterialGroup, () => CurrentGMPMaterialGroup, c => CurrentGMPMaterialGroup = c,
                        DatabaseApp.MDGMPMaterialGroup
                        .Where(c => c.MDGMPMaterialGroupID == SelectedGMPMaterialGroup.MDGMPMaterialGroupID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedGMPMaterialGroup != null;
        }


        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDGMPMaterialGroup.ClassName, Const.New, (short)MISort.New, true, "SelectedGMPMaterialGroup", Global.ACKinds.MSMethodPrePost, Description =
                         @"Creates a new GMP material group and assigns it to the CurrentGMPMaterialGroup property. Fill in all required fields and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentGMPMaterialGroup = MDGMPMaterialGroup.NewACObject(DatabaseApp, null);
            DatabaseApp.MDGMPMaterialGroup.Add(CurrentGMPMaterialGroup);
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
        [ACMethodInteraction(MDGMPMaterialGroup.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentGMPMaterialGroup", Global.ACKinds.MSMethodPrePost, Description =
                         @"Deletes the current GMP material group, provided no other tables reference it (foreign key check). Confirm the deletion with Save().")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentGMPMaterialGroup.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentGMPMaterialGroup);
            SelectedGMPMaterialGroup = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(GMPMaterialGroupList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentGMPMaterialGroup != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDGMPMaterialGroup.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description =
                         @"Executes the search and refreshes the GMPMaterialGroupList with the database result.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(GMPMaterialGroupList));
        }

        /// <summary>
        /// Loads the GMP material group pos.
        /// </summary>
        [ACMethodInteraction("MDGMPMaterialGroupPos", "en{'Load Ingredient'}de{'Inhaltsstoff laden'}", 9999, false, "SelectedGMPMaterialGroupPos", Global.ACKinds.MSMethodPrePost, Description =
                         @"Loads the selected ingredient position of the current GMP material group into the CurrentGMPMaterialGroupPos property.")]
        public void LoadGMPMaterialGroupPos()
        {
            if (!IsEnabledLoadGMPMaterialGroupPos())
            {
                if (!PreExecute("LoadGMPMaterialGroupPos")) return;
                if (SelectedGMPMaterialGroupPos != null)
                    CurrentGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup.Where(c => c.MDGMPMaterialGroupPosID == SelectedGMPMaterialGroupPos.MDGMPMaterialGroupPosID).FirstOrDefault();
                else
                    CurrentGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup.FirstOrDefault();
                PostExecute("LoadGMPMaterialGroupPos");

            }
        }

        /// <summary>
        /// Determines whether [is enabled load GMP material group pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load GMP material group pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoadGMPMaterialGroupPos()
        {
            return SelectedGMPMaterialGroupPos != null && CurrentGMPMaterialGroup != null;
        }


        /// <summary>
        /// Deletes the GMP material group pos.
        /// </summary>
        [ACMethodInteraction("MDGMPMaterialGroupPos", "en{'Delete Ingredient'}de{'Inhaltsstoff löschen'}", (short)MISort.Delete, true, "CurrentGMPMaterialGroupPos", Global.ACKinds.MSMethodPrePost, Description =
                         @"Deletes the current ingredient position from the current GMP material group. Confirm the deletion with Save().")]
        public void DeleteGMPMaterialGroupPos()
        {
            if (CurrentGMPMaterialGroupPos != null)
            {
                if (!PreExecute("DeleteGMPMaterialGroupPos")) return;
                Msg msg = CurrentGMPMaterialGroupPos.DeleteACObject(DatabaseApp, true);
                if (msg != null)
                {
                    Messages.MsgAsync(msg);
                    return;
                }

                PostExecute("DeleteGMPMaterialGroupPos");
            }
        }

        /// <summary>
        /// Determines whether [is enabled delete GMP material group pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete GMP material group pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteGMPMaterialGroupPos()
        {
            return CurrentGMPMaterialGroup != null && CurrentGMPMaterialGroupPos != null;
        }

        /// <summary>
        /// News the GMP material group pos.
        /// </summary>
        [ACMethodInteraction("MDGMPMaterialGroupPos", "en{'New Ingredient'}de{'Neuer Inhaltsstoff'}", (short)MISort.New, true, "SelectedGMPMaterialGroupPos", Global.ACKinds.MSMethodPrePost, Description =
                         @"Creates a new ingredient position and adds it to the ingredient list of the current GMP material group. Fill in all required fields and call Save().")]
        public void NewGMPMaterialGroupPos()
        {
            if (!PreExecute("NewGMPMaterialGroupPos")) return;
            CurrentGMPMaterialGroupPos = MDGMPMaterialGroupPos.NewACObject(DatabaseApp, CurrentGMPMaterialGroup);
            CurrentGMPMaterialGroupPos.MDGMPMaterialGroup = CurrentGMPMaterialGroup;
            CurrentGMPMaterialGroup.MDGMPMaterialGroupPos_MDGMPMaterialGroup.Add(CurrentGMPMaterialGroupPos);
            PostExecute("NewGMPMaterialGroupPos");
            LoadGMPMaterialGroupPos();
        }

        /// <summary>
        /// Determines whether [is enabled new GMP material group pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new GMP material group pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewGMPMaterialGroupPos()
        {
            return CurrentGMPMaterialGroup != null;
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
                case nameof(LoadGMPMaterialGroupPos):
                    LoadGMPMaterialGroupPos();
                    return true;
                case nameof(IsEnabledLoadGMPMaterialGroupPos):
                    result = IsEnabledLoadGMPMaterialGroupPos();
                    return true;
                case nameof(DeleteGMPMaterialGroupPos):
                    DeleteGMPMaterialGroupPos();
                    return true;
                case nameof(IsEnabledDeleteGMPMaterialGroupPos):
                    result = IsEnabledDeleteGMPMaterialGroupPos();
                    return true;
                case nameof(NewGMPMaterialGroupPos):
                    NewGMPMaterialGroupPos();
                    return true;
                case nameof(IsEnabledNewGMPMaterialGroupPos):
                    result = IsEnabledNewGMPMaterialGroupPos();
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
                    return new string[] { nameof(ACState), nameof(SelectedGMPMaterialGroup) };
                case nameof(New):
                    return new string[] { nameof(ACState) };
                case nameof(Delete):
                    return new string[] { nameof(CurrentGMPMaterialGroup) };
                default:
                    return base.GetPropsToObserveForIsEnabled(acMethodName);
            }
        }

        #endregion

    }
}