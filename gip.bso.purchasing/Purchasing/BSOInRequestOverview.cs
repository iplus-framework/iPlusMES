// ***********************************************************************
// Assembly         : gip.bso.purchasing
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOInRequestOverview.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved..ClassName
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using gip.core.manager;
using gip.mes.datamodel; using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.purchasing
{
    /// <summary>
    /// Businessobject or App for an overview of all requests for quotation (RfQ / purchase inquiries) in the system.
    /// This class is a read-oriented overview: it shows the list of all inquiries and allows navigating, loading and saving changes on the selected inquiry.
    /// To search records call the Search() method (it is executed automatically when the BSO starts).
    /// The database result is copied to the InRequestList property.
    /// Then call NavigateFirst() method to set CurrentInRequest with the first record in the list.
    /// CurrentInRequest is used to display and edit the currently selected record.
    /// Property changes should always be made to CurrentInRequest and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record.
    /// The UndoSave() method discards unsaved changes of the current inquiry.
    /// The Load() method re-reads the selected inquiry (including its positions) from the database into CurrentInRequest, e.g. if another user has changed it in the background.
    /// This BSO does not provide New() or Delete() methods; inquiries are created and deleted in the BSOInRequest app.
    /// Visit https://github.com/search?q=org%3Aiplus-framework%20BSOInRequestOverview&type=code for more information or ask the iplus-framework Copilot.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Inrequestoverview'}de{'Anfragenübersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + InRequest.ClassName,
                 Description = @"Businessobject or App for an overview of all requests for quotation (RfQ / purchase inquiries) in the system.
                                This class is a read-oriented overview: it shows the list of all inquiries and allows navigating, loading and saving changes on the selected inquiry.
                                To search records call the Search() method (it is executed automatically when the BSO starts).
                                The database result is copied to the InRequestList property.
                                Then call NavigateFirst() method to set CurrentInRequest with the first record in the list.
                                CurrentInRequest is used to display and edit the currently selected record.
                                Property changes should always be made to CurrentInRequest and when all field values have been changed, the Save() method should be called to save the changes in the database before navigating to the next record.
                                The UndoSave() method discards unsaved changes of the current inquiry.
                                The Load() method re-reads the selected inquiry (including its positions) from the database into CurrentInRequest, e.g. if another user has changed it in the background.
                                This BSO does not provide New() or Delete() methods; inquiries are created and deleted in the BSOInRequest app.
                                Visit https://github.com/search?q=org%3Aiplus-framework%20BSOInRequestOverview&type=code for more information or ask the iplus-framework Copilot.")]
    public class BSOInRequestOverview : ACBSOvbNav 
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOInRequestOverview"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOInRequestOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
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
            var x3 = DatabaseApp.InRequest.ToList();
            var x4 = DatabaseApp.Company.ToList();

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
        ACAccessNav<InRequest> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, "InRequest", Description = "Primary navigation access to the InRequest entities. Used by the search list and the navigation methods.")]
        public ACAccessNav<InRequest> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<InRequest>("InRequest", this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the current in request.
        /// </summary>
        /// <value>The current in request.</value>
        [ACPropertyCurrent(600, "InRequest", Description = "Gets or sets the currently displayed purchase inquiry (InRequest). Changes must be made on this entity and saved with Save().")]
        public InRequest CurrentInRequest
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentInRequest");
            }
        }

        /// <summary>
        /// Gets the in request list.
        /// </summary>
        /// <value>The in request list.</value>
        [ACPropertyList(601, "InRequest", Description = "Gets the result list of all purchase inquiries after Search() was executed.")]
        public IEnumerable<InRequest> InRequestList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected in request.
        /// </summary>
        /// <value>The selected in request.</value>
        [ACPropertySelected(602, "InRequest", Description = "Gets or sets the inquiry selected in the overview list. Load() loads this inquiry into CurrentInRequest.")]
        public InRequest SelectedInRequest
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedInRequest");
            }
        }

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("InRequest", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description = "Saves all changes of the current inquiry to the database.")]
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
        [ACMethodCommand("InRequest", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description = "Discards all unsaved changes of the current inquiry.")]
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
        [ACMethodInteraction("InRequest", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedInRequest", Global.ACKinds.MSMethodPrePost, Description = "Re-reads the selected inquiry (including its positions) from the database into CurrentInRequest, e.g. if another user has changed it in the background.")]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<InRequest>(requery, () => SelectedInRequest, () => CurrentInRequest, c => CurrentInRequest = c,
                        DatabaseApp.InRequest
                        .Include(c => c.InRequestPos_InRequest)
                        .Where(c => c.InRequestID == SelectedInRequest.InRequestID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedInRequest != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("InRequest", "en{'Search'}de{'Suchen'}", (short)MISort.Search, Description = "Executes the search and refreshes the InRequestList property. Executed automatically when the BSO starts.")]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("InRequestList");
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
                    return new string[] { nameof(ACState), nameof(SelectedInRequest) };
                #endregion
            }
            return base.GetPropsToObserveForIsEnabled(acMethodName);
        }

        #endregion

    }
}
