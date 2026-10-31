// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="MDBSOGMPAdditive.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using gip.core.autocomponent;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.mes.autocomponent;
using System.Threading.Tasks;

namespace gip.bso.masterdata
{
    /// <summary>
    /// Value list for GMP additives (GMP/HACCP ingredients such as antibiotics, hormones, bacteria, dioxin) that can be assigned to materials for GMP/HACCP documentation.
    /// To search for records, enter the search string in the SearchWord property.
    /// The database result is copied to the GMPAdditiveList property.
    /// Then call the NavigateFirst() method to set CurrentGMPAdditive with the first record in the list.
    /// CurrentGMPAdditive is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentGMPAdditive and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
    /// The New() method creates a new record and assigns the new entity object to the CurrentGMPAdditive property. Fill in all required fields before saving.
    /// Use the Delete() method to delete the additive provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
    /// The Load() method updates the CurrentGMPAdditive object with fresh database data if another user has made changes in the background.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOGMPAdditive&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'GMP-Additive'}de{'GMP-Zusatzstoff'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDGMPAdditive.ClassName,
        Description = @"Value list for GMP additives (GMP/HACCP ingredients such as antibiotics, hormones, bacteria, dioxin) that can be assigned to materials for GMP/HACCP documentation.
        To search for records, enter the search string in the SearchWord property.
        The database result is copied to the GMPAdditiveList property.
        Then call the NavigateFirst() method to set CurrentGMPAdditive with the first record in the list.
        CurrentGMPAdditive is used to display and edit the currently selected record.
        Property changes should always be made to CurrentGMPAdditive and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record or creating a new record.
        The New() method creates a new record and assigns the new entity object to the CurrentGMPAdditive property. Fill in all required fields before saving.
        Use the Delete() method to delete the additive provided there are no foreign key relationships from other tables. Always call the Save() method after calling Delete() to execute the delete operation in the database.
        The Load() method updates the CurrentGMPAdditive object with fresh database data if another user has made changes in the background.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20MDBSOGMPAdditive&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class MDBSOGMPAdditive : ACBSOvbNav
    {
        #region c´tors


        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOGMPAdditive"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOGMPAdditive(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
        ACAccessNav<MDGMPAdditive> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, "MDGMPAdditive", Description = "Gets the primary navigation access to the GMP additive entities.")]
        public ACAccessNav<MDGMPAdditive> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDGMPAdditive>("MDGMPAdditive", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected GMP additive.
        /// </summary>
        /// <value>The selected GMP additive.</value>
        [ACPropertySelected(9999, "MDGMPAdditive", Description = "Gets or sets the GMP additive currently selected in the list.")]
        public MDGMPAdditive SelectedGMPAdditive
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedGMPAdditive");
            }
        }

        /// <summary>
        /// Gets or sets the current GMP additive.
        /// </summary>
        /// <value>The current GMP additive.</value>
        [ACPropertyCurrent(9999, "MDGMPAdditive", Description = "Gets or sets the current GMP additive. Property changes are made on this record before calling Save().")]
        public MDGMPAdditive CurrentGMPAdditive
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentGMPAdditive");
            }
        }

        /// <summary>
        /// Gets the GMP additive list.
        /// </summary>
        /// <value>The GMP additive list.</value>
        [ACPropertyList(9999, "MDGMPAdditive", Description = "Gets the list of GMP additives resulting from the search.")]
        public IEnumerable<MDGMPAdditive> GMPAdditiveList
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
        [ACMethodCommand("MDGMPAdditive", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Saves the changes made to the current GMP additive to the database and refreshes the list.
                           Always call Save() before navigating to another record or creating a new one.")]
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
        [ACMethodCommand("MDGMPAdditive", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description =
                         @"Discards all unsaved changes made to the current GMP additive and restores the database state.")]
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
        [ACMethodInteraction("MDGMPAdditive", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedGMPAdditive", Global.ACKinds.MSMethodPrePost, Description =
                         @"Reloads the selected GMP additive from the database with fresh data, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDGMPAdditive>(requery, () => SelectedGMPAdditive, () => CurrentGMPAdditive, c => CurrentGMPAdditive = c,
                        DatabaseApp.MDGMPAdditive
                        .Where(c => c.MDGMPAdditiveID == SelectedGMPAdditive.MDGMPAdditiveID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedGMPAdditive != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("MDGMPAdditive", Const.New, (short)MISort.New, true, "SelectedGMPAdditive", Global.ACKinds.MSMethodPrePost, Description =
                         @"Creates a new GMP additive and assigns it to the CurrentGMPAdditive property. Fill in all required fields and call Save().")]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentGMPAdditive = MDGMPAdditive.NewACObject(DatabaseApp, null);
            DatabaseApp.MDGMPAdditive.Add(CurrentGMPAdditive);
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
        [ACMethodInteraction("MDGMPAdditive", Const.Delete, (short)MISort.Delete, true, "CurrentGMPAdditive", Global.ACKinds.MSMethodPrePost, Description =
                         @"Deletes the current GMP additive, provided no other tables reference it (foreign key check). Confirm the deletion with Save().")]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentGMPAdditive.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }

            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentGMPAdditive);
            SelectedGMPAdditive = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
            OnPropertyChanged(nameof(GMPAdditiveList));
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentGMPAdditive != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("MDGMPAdditive", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description =
                         @"Executes the search and refreshes the GMPAdditiveList with the database result.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(GMPAdditiveList));
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
                #region Always Enabled
                case nameof(Search):
                    return new string[] { nameof(InitState) };
                #endregion

                #region Load
                case nameof(Load):
                case nameof(IsEnabledLoad):
                    return new string[] { nameof(ACState), nameof(SelectedGMPAdditive) };
                #endregion

                #region Delete
                case nameof(Delete):
                case nameof(IsEnabledDelete):
                    return new string[] { nameof(CurrentGMPAdditive) };
                #endregion

                #region New
                case nameof(New):
                case nameof(IsEnabledNew):
                    return new string[] { nameof(ACState) };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

    }
}