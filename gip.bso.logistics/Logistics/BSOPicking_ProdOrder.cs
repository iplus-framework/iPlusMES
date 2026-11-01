// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿// ***********************************************************************
// Assembly         : gip.bso.logistics
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOVisitorVoucher.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;
using gip.bso.sales;
using gip.bso.purchasing;
using gip.mes.facility;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace gip.bso.logistics
{
    /// <summary>
    /// Partial class of BSOPicking that manages the assignment of production order positions (ProdOrderPartslistPos) to a picking order.
    /// This partial adds the open production order positions (query "ProdOrderPartslistPosOpen", filtered to production orders in state NewCreated,
    /// root positions of type OutwardRoot without already assigned picking position) in the ProdOrderPartslistPosList property
    /// with SelectedProdOrderPartslistPos/CurrentProdOrderPartslistPos, sorted by program number and sequence.
    /// AssignProdOrderPartslistPos() assigns the selected production order position to the current picking (via PickingManager.AssignProdOrderPartslistPos,
    /// considering the PartialQuantity for partial call-off).
    /// FilterDialogProdOrderPartslistPos() opens the filter dialog for the open positions list; RefreshProdOrderPartslistPosList() re-queries the list.
    /// </summary>
    public partial class BSOPicking : ACBSOvbNav
    {
        #region Properties

        #region ProdOrderPartslistPos
        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<ProdOrderPartslistPos> _AccessProdOrderPartslistPos;
        /// <summary>
        /// Gets the access to the open production order positions.
        /// </summary>
        /// <value>The access to the open ProdOrderPartslistPos.</value>
        [ACPropertyAccess(696, ProdOrderPartslistPos.ClassName, "en{'Open Production Order Positions'}de{'Offene Fertigungsauftragspositionen'}", Description = @"Navigation access to the open production order positions (query ProdOrderPartslistPosOpen, filtered to production orders in state NewCreated, root positions of type OutwardRoot without assigned picking position, sorted by program number and sequence). Provides the ProdOrderPartslistPosList.")]
        public ACAccessNav<ProdOrderPartslistPos> AccessProdOrderPartslistPos
        {
            get
            {
                if (_AccessProdOrderPartslistPos == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "ProdOrderPartslistPosOpen", ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceColumnsIfDifferent(ProdOrderPartslistPosDefaultFilter, ProdOrderPartslistPosDefaultSort);
                    }

                    _AccessProdOrderPartslistPos = navACQueryDefinition.NewAccessNav<ProdOrderPartslistPos>(ProdOrderPartslistPos.ClassName, this);
                    _AccessProdOrderPartslistPos.AutoSaveOnNavigation = false;
                    _AccessProdOrderPartslistPos.NavSearchExecuting += _AccessProdOrderPartslistPos_NavSearchExecuting;
                }
                return _AccessProdOrderPartslistPos;
            }
        }

        private IQueryable<ProdOrderPartslistPos> _AccessProdOrderPartslistPos_NavSearchExecuting(IQueryable<ProdOrderPartslistPos> result)
        {
            result = (result as IQueryable<ProdOrderPartslistPos>)
                            .Include(c => c.ProdOrderPartslist)
                            .Include(c => c.ProdOrderPartslist.ProdOrder)
                            .Include(c => c.ProdOrderPartslist.Partslist)
                            .Include(c => c.Material)
                            .Include(c => c.Material.BaseMDUnit)
                            .Include(c => c.MDUnit)
                            .Where(c => !c.PickingPosProdOrderPartslistPos_ProdorderPartslistPos.Any());
            return result;
        }

        private List<ACFilterItem> ProdOrderPartslistPosDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)MDProdOrderState.ProdOrderStates.NewCreated).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.filter, "ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)MDProdOrderState.ProdOrderStates.NewCreated).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.filter, "ProdOrderPartslistPos1_ParentProdOrderPartslistPos", Global.LogicalOperators.isNull, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, "MaterialPosTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)GlobalApp.MaterialPosTypes.OutwardRoot).ToString(), true)
                };
            }
        }

        private List<ACSortItem> ProdOrderPartslistPosDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("ProdOrderPartslist.ProdOrder.ProgramNo", Global.SortDirections.ascending, true),
                    new ACSortItem("ProdOrderPartslist.Sequence", Global.SortDirections.ascending, true),
                    new ACSortItem("Sequence", Global.SortDirections.ascending, true),
                };
            }
        }

        /// <summary>
        /// Gets or sets the current production order position (open ProdOrderPartslistPos).
        /// </summary>
        /// <value>The current ProdOrderPartslistPos.</value>
        [ACPropertyCurrent(660, ProdOrderPartslistPos.ClassName, "en{'Production Order Position'}de{'Fertigungsauftragsposition'}", Description = @"Currently selected open production order position. This position is assigned to the current picking with AssignProdOrderPartslistPos().")]
        public ProdOrderPartslistPos CurrentProdOrderPartslistPos
        {
            get
            {
                if (AccessProdOrderPartslistPos == null)
                    return null;
                return AccessProdOrderPartslistPos.Current;
            }
            set
            {
                if (AccessProdOrderPartslistPos == null)
                    return;
                AccessProdOrderPartslistPos.Current = value;
                OnPropertyChanged(nameof(CurrentProdOrderPartslistPos));
            }
        }

        /// <summary>
        /// Gets the list of open production order positions that can be assigned to the current picking.
        /// </summary>
        /// <value>The ProdOrderPartslistPos list.</value>
        [ACPropertyList(661, ProdOrderPartslistPos.ClassName, "en{'Open Production Order Positions'}de{'Offene Fertigungsauftragspositionen'}", Description = @"List of open production order positions (query ProdOrderPartslistPosOpen). Positions already assigned by unsaved picking positions of the current picking are hidden; unsaved unassigned positions are merged in.")]
        public IEnumerable<ProdOrderPartslistPos> ProdOrderPartslistPosList
        {
            get
            {
                if (AccessProdOrderPartslistPos == null)
                    return null;
                if (CurrentPicking != null)
                {
                    IEnumerable<ProdOrderPartslistPos> addedPositions = CurrentPicking.PickingPos_Picking.Where(c => c.EntityState == EntityState.Added
                        && c.PickingPosProdOrderPartslistPos_PickingPos.Any()
                        ).SelectMany(c => c.PickingPosProdOrderPartslistPos_PickingPos).Select(c=>c.ProdorderPartslistPos);
                        //&& c.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos != null
                        //).Select(c => c.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos);
                    if (addedPositions.Any())
                    {
                        if (_UnSavedUnAssignedProdOrderPartslistPos.Any())
                            return AccessProdOrderPartslistPos.NavList.Except(addedPositions).Union(_UnSavedUnAssignedProdOrderPartslistPos);
                        else
                            return AccessProdOrderPartslistPos.NavList.Except(addedPositions);
                    }
                }
                if (_UnSavedUnAssignedProdOrderPartslistPos.Any())
                {
                    return AccessProdOrderPartslistPos.NavList.Union(_UnSavedUnAssignedProdOrderPartslistPos);
                }
                return AccessProdOrderPartslistPos.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected production order position in the open positions list. Setting it also sets CurrentProdOrderPartslistPos.
        /// </summary>
        /// <value>The selected ProdOrderPartslistPos.</value>
        [ACPropertySelected(662, ProdOrderPartslistPos.ClassName, "en{'Production Order Position'}de{'Fertigungsauftragsposition'}", Description = @"Selected production order position in the open positions list. Setting it also sets CurrentProdOrderPartslistPos.")]
        public ProdOrderPartslistPos SelectedProdOrderPartslistPos
        {
            get
            {
                if (AccessProdOrderPartslistPos == null)
                    return null;
                return AccessProdOrderPartslistPos.Selected;
            }
            set
            {
                if (AccessProdOrderPartslistPos == null)
                    return;
                AccessProdOrderPartslistPos.Selected = value;
                OnPropertyChanged(nameof(SelectedProdOrderPartslistPos));
                CurrentProdOrderPartslistPos = value;
            }
        }
        #endregion

        #endregion

        #region Methods

        #region ProdOrderPartslistPos
        /// <summary>
        /// Assigns the selected production order position to the current picking.
        /// </summary>
        [ACMethodCommand("xxx", "en{'Assign'}de{'Zuordnen'}", 660, true, Global.ACKinds.MSMethodPrePost, Description = @"Assigns the production order position selected in the open positions list (CurrentProdOrderPartslistPos) to the current picking via PickingManager.AssignProdOrderPartslistPos. The PartialQuantity property allows a partial call-off; after assignment the position list is refreshed and PartialQuantity is reset.")]
        public void AssignProdOrderPartslistPos()
        {
            if (!PreExecute("AssignProdOrderPartslistPos"))
                return;

            try
            {
                List<object> resultNewEntities = new List<object>();
                Msg result = PickingManager.AssignProdOrderPartslistPos(CurrentPicking, CurrentProdOrderPartslistPos, PartialQuantity, this.DatabaseApp, resultNewEntities);
                if (result != null)
                {
                    Messages.MsgAsync(result);
                    return;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOPicking", "AssignProdOrderPartslistPos", msg);
            }

            if (_UnSavedUnAssignedProdOrderPartslistPos.Contains(CurrentProdOrderPartslistPos))
                _UnSavedUnAssignedProdOrderPartslistPos.Remove(CurrentProdOrderPartslistPos);
            OnPropertyChanged(nameof(PickingPosList));
            RefreshProdOrderPartslistPosList();
            PartialQuantity = null;
        }

        /// <summary>
        /// Determines whether the Assign command is enabled (a production order position is selected).
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignProdOrderPartslistPos()
        {
            if (CurrentProdOrderPartslistPos == null)
                return false;
            return true;
        }

        /// <summary>
        /// Opens the filter dialog for the open production order positions list.
        /// </summary>
        [ACMethodInteraction("xxx", "en{'Filter'}de{'Filter'}", 661, false, Description = @"Opens the query/filter dialog (ShowACQueryDialog) for the open production order positions list and refreshes the list after applying the filter.")]
        public async Task<bool> FilterDialogProdOrderPartslistPos()
        {
            if (AccessProdOrderPartslistPos == null)
                return false;
            bool result = await AccessProdOrderPartslistPos.ShowACQueryDialog();
            if (result)
            {
                RefreshProdOrderPartslistPosList();
            }
            return result;
        }

        public virtual void RefreshProdOrderPartslistPosList(bool forceQueryFromDb = false)
        {
            if (AccessProdOrderPartslistPos == null)
                return;
            if (_ActivateProdOpen || forceQueryFromDb)
                AccessProdOrderPartslistPos.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(ProdOrderPartslistPosList));
        }
        #endregion

        #endregion
    }
}
