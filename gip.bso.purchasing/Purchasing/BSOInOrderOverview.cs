// ***********************************************************************
// Assembly         : gip.bso.purchasing
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOInOrderOverview.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using gip.core.manager;
using gip.mes.datamodel; 
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.purchasing
{
    /// <summary>
    /// Businessobject or App for an overview of all purchase order positions (InOrderPos) in the system.
    /// This class is a read-oriented overview: it shows the list of all purchase order positions and allows navigating, loading and saving changes on the selected position.
    /// To search records call the Search() method (it is executed automatically when the BSO starts).
    /// The database result is copied to the InOrderPosList property.
    /// Then call NavigateFirst() method to set CurrentInOrderPos with the first record in the list.
    /// CurrentInOrderPos is used to display and edit the currently selected record; the parent order of the current position is available in the CurrentInOrder property.
    /// Property changes should always be made to CurrentInOrderPos and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record.
    /// The UndoSave() method discards unsaved changes of the current position.
    /// The Load() method re-reads the selected position from the database into CurrentInOrderPos, e.g. if another user has changed it in the background.
    /// This BSO does not provide New() or Delete() methods; purchase orders are created and managed in the BSOInOrder app.
    /// Visit https://github.com/search?q=org%3Aiplus-framework%20BSOInOrderOverview&type=code for more information or ask the iplus-framework Copilot.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Inorderoverview'}de{'Bestellübersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + InOrderPos.ClassName,
                 Description = @"Businessobject or App for an overview of all purchase order positions (InOrderPos) in the system.
                                This class is a read-oriented overview: it shows the list of all purchase order positions and allows navigating, loading and saving changes on the selected position.
                                To search records call the Search() method (it is executed automatically when the BSO starts).
                                The database result is copied to the InOrderPosList property.
                                Then call NavigateFirst() method to set CurrentInOrderPos with the first record in the list.
                                CurrentInOrderPos is used to display and edit the currently selected record; the parent order of the current position is available in the CurrentInOrder property.
                                Property changes should always be made to CurrentInOrderPos and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record.
                                The UndoSave() method discards unsaved changes of the current position.
                                The Load() method re-reads the selected position from the database into CurrentInOrderPos, e.g. if another user has changed it in the background.
                                This BSO does not provide New() or Delete() methods; purchase orders are created and managed in the BSOInOrder app.
                                Visit https://github.com/search?q=org%3Aiplus-framework%20BSOInOrderOverview&type=code for more information or ask the iplus-framework Copilot.")]
    public class BSOInOrderOverview : ACBSOvbNav 
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOInOrderOverview"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOInOrderOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
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
            var x1 = DatabaseApp.MDUnit.ToList();
            var x2 = DatabaseApp.Material.ToList();
            var x3 = DatabaseApp.InOrder.ToList();
            var x4 = DatabaseApp.Company.ToList();

            Search();
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            this._CurrentInOrder = null;
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
        ACAccessNav<InOrderPos> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, InOrderPos.ClassName, Description = "Primary navigation access to the InOrderPos entities. Used by the search list and the navigation methods.")]
        public  ACAccessNav<InOrderPos> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<InOrderPos>(InOrderPos.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(600, InOrderPos.ClassName, Description = "Gets or sets the currently displayed purchase order position (InOrderPos). Changes must be made on this entity and saved with Save().")]
        public InOrderPos CurrentInOrderPos
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentInOrderPos");
                OnPropertyChanged("CurrentInOrder");
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(601, InOrderPos.ClassName, Description = "Gets the result list of all purchase order positions after Search() was executed.")]
        public IEnumerable<InOrderPos> InOrderPosList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(602, InOrderPos.ClassName, Description = "Gets or sets the purchase order position selected in the overview list. Load() loads this position into CurrentInOrderPos.")]
        public InOrderPos SelectedInOrderPos
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedInOrderPos");
            }
        }

        /// <summary>
        /// The _ current in order
        /// </summary>
        InOrder _CurrentInOrder;
        /// <summary>
        /// Gets or sets the current in order.
        /// </summary>
        /// <value>The current in order.</value>
        [ACPropertyCurrent(603, InOrder.ClassName, Description = "Gets or sets the parent purchase order (InOrder) of the current position. Set automatically when CurrentInOrderPos changes.")]
        public InOrder CurrentInOrder
        {
            get
            {
                return _CurrentInOrder;
            }
            set
            {
                _CurrentInOrder = value;
                OnPropertyChanged("CurrentInOrder");
            }
        }
        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(InOrderPos.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves all changes of the current purchase order position to the database.")]
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
        [ACMethodCommand(InOrderPos.ClassName, "en{'undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards all unsaved changes of the current purchase order position.")]
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
        [ACMethodInteraction(InOrderPos.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedInOrderPos", Global.ACKinds.MSMethodPrePost, Description = "Re-reads the selected purchase order position from the database into CurrentInOrderPos, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            if (SelectedInOrderPos != null
                && (requery
                    || (!requery && CurrentInOrderPos != SelectedInOrderPos)))
            {
                ACSaveOrUndoChanges();
                if (requery)
                {
                    if (DatabaseApp.RecommendedMergeOption == MergeOption.OverwriteChanges)
                    {
                        Debugger.Break();
                        throw new NotImplementedException();
                        //Not implemented in entity framework core
                        //CurrentInOrder.InOrderPos_InOrder.AutoLoad(System.Data.Objects.MergeOption.OverwriteChanges);
                    }
                    CurrentInOrderPos = CurrentInOrder.InOrderPos_InOrder
                        .Where(c => c.InOrderPosID == SelectedInOrderPos.InOrderPosID).FirstOrDefault();

                    // Falls Neu angelegt und nicht gespeichert, dann ist Datenbankabfrage natürlich leer
                    if (CurrentInOrderPos == null)
                        CurrentInOrderPos = SelectedInOrderPos;
                }
                else
                    CurrentInOrderPos = SelectedInOrderPos;
            }
            else if (SelectedInOrderPos == null)
                CurrentInOrderPos = null;
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedInOrderPos != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(InOrderPos.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Executes the search and refreshes the InOrderPosList property. Executed automatically when the BSO starts.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("InOrderPosList");
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
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
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
                    return new string[] { nameof(ACState), nameof(SelectedInOrderPos) };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

    }
}
