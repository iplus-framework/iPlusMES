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
    /// Version 3
    /// Neue Masken:
    /// 1. Bestellübersicht
    /// TODO: Betroffene Tabellen: InOrder, InOrderPos
    /// </summary>
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Inorderoverview'}de{'Bestellübersicht'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + InOrderPos.ClassName)]
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

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._CurrentInOrder = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
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
        [ACPropertyAccessPrimary(690, InOrderPos.ClassName)]
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
        [ACPropertyCurrent(600, InOrderPos.ClassName)]
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
        [ACPropertyList(601, InOrderPos.ClassName)]
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
        [ACPropertySelected(602, InOrderPos.ClassName)]
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
        [ACPropertyCurrent(603, InOrder.ClassName)]
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
        [ACMethodCommand(InOrderPos.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(InOrderPos.ClassName, "en{'undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodInteraction(InOrderPos.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedInOrderPos", Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(InOrderPos.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
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
                case"Save":
                    Save();
                    return true;
                case"IsEnabledSave":
                    result = IsEnabledSave();
                    return true;
                case"UndoSave":
                    UndoSave();
                    return true;
                case"IsEnabledUndoSave":
                    result = IsEnabledUndoSave();
                    return true;
                case"Load":
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case"IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
                case"Search":
                    Search();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}
