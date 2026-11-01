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
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gip.bso.logistics
{
    /// <summary>
    /// Partial class of BSOPicking that manages the assignment of sales order positions (OutOrderPos) to a picking order.
    /// This partial adds the open sales order positions (query "OutOrderPosOpen", filtered to positions not completely assigned and without parent position)
    /// in the OutOrderPosList property with SelectedOutOrderPos/CurrentOutOrderPos, and the positions of unpicked sales delivery notes in the DNoteOutOrderPosList property
    /// with SelectedDNoteOutOrderPos/CurrentDNoteOutOrderPos (active when _ActivateOutDNote is set).
    /// AssignOutOrderPos() assigns the selected sales order position to the current picking (via PickingManager.AssignOutOrderPos, considering the PartialQuantity for partial call-off).
    /// AssignDNoteOutOrderPos() assigns the selected position of a sales delivery note to the current picking.
    /// FilterDialogOutOrderPos() opens the filter dialog for the open positions list; RefreshOutOrderPosList() re-queries the list.
    /// </summary>
    public partial class BSOPicking : ACBSOvbNav
    {
        #region Properties

        #region OutOrderPos
        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<OutOrderPos> _AccessOutOrderPos;
        /// <summary>
        /// Gets the access to the open sales order positions.
        /// </summary>
        /// <value>The access to the open OutOrderPos.</value>
        [ACPropertyAccess(695, OutOrderPos.ClassName, "en{'Open Sales Order Positions'}de{'Offene Auftragspositionen'}", Description = @"Navigation access to the open sales order positions (query OutOrderPosOpen, filtered to positions with delivery state less than CompletelyAssigned and without parent position). Provides the OutOrderPosList.")]
        public ACAccessNav<OutOrderPos> AccessOutOrderPos
        {
            get
            {
                if (_AccessOutOrderPos == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "OutOrderPosOpen", ACType.ACIdentifier);
                    bool rebuildACQueryDef = false;
                    short assigned = (short)MDDelivPosState.DelivPosStates.CompletelyAssigned;
                    if (navACQueryDefinition.ACFilterColumns.Count <= 0)
                    {
                        rebuildACQueryDef = true;
                    }
                    else
                    {
                        int countFoundCorrect = 0;
                        foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            if (filterItem.PropertyName == "MDDelivPosState\\MDDelivPosStateIndex")
                            {
                                if (filterItem.SearchWord == assigned.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.lessThan)
                                    countFoundCorrect++;
                            }
                            else if (filterItem.PropertyName == "OutOrderPos1_ParentOutOrderPos")
                            {
                                if (String.IsNullOrEmpty(filterItem.SearchWord) && filterItem.LogicalOperator == Global.LogicalOperators.isNull)
                                    countFoundCorrect++;
                            }
                        }
                        if (countFoundCorrect < 2)
                            rebuildACQueryDef = true;
                    }
                    if (rebuildACQueryDef)
                    {
                        navACQueryDefinition.ClearFilter(true);
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDDelivPosState\\MDDelivPosStateIndex", Global.LogicalOperators.lessThan, Global.Operators.and, assigned.ToString(), true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "OutOrderPos1_ParentOutOrderPos", Global.LogicalOperators.isNull, Global.Operators.and, "", true));
                        navACQueryDefinition.SaveConfig(true);
                    }
                    _AccessOutOrderPos = navACQueryDefinition.NewAccessNav<OutOrderPos>(OutOrderPos.ClassName, this);
                    _AccessOutOrderPos.AutoSaveOnNavigation = false;
                }
                return _AccessOutOrderPos;
            }
        }

        /// <summary>
        /// Gets or sets the current sales order position (open OutOrderPos).
        /// </summary>
        /// <value>The current OutOrderPos.</value>
        [ACPropertyCurrent(650, OutOrderPos.ClassName, "en{'Sales Order Position'}de{'Auftragsposition'}", Description = @"Currently selected open sales order position. This position is assigned to the current picking with AssignOutOrderPos().")]
        public OutOrderPos CurrentOutOrderPos
        {
            get
            {
                return AccessOutOrderPos.Current;
            }
            set
            {
                AccessOutOrderPos.Current = value;
                OnPropertyChanged(nameof(CurrentOutOrderPos));
            }
        }

        /// <summary>
        /// Gets the list of open sales order positions that can be assigned to the current picking.
        /// </summary>
        /// <value>The OutOrderPos list.</value>
        [ACPropertyList(651, OutOrderPos.ClassName, "en{'Open Sales Order Positions'}de{'Offene Auftragspositionen'}", Description = @"List of open sales order positions (query OutOrderPosOpen). Positions already fully assigned by unsaved picking positions of the current picking are hidden; unsaved unassigned positions are merged in.")]
        public IEnumerable<OutOrderPos> OutOrderPosList
        {
            get
            {
                if (AccessOutOrderPos == null)
                    return null;
                if (CurrentPicking != null)
                {
                    IEnumerable<OutOrderPos> addedPositions = CurrentPicking.PickingPos_Picking.Where(c => c.EntityState == Microsoft.EntityFrameworkCore.EntityState.Added
                        && c.OutOrderPos != null
                        && c.OutOrderPos.OutOrderPos1_ParentOutOrderPos != null
                        && c.OutOrderPos.OutOrderPos1_ParentOutOrderPos.MDDelivPosState == StateCompletelyAssigned
                        ).Select(c => c.OutOrderPos.OutOrderPos1_ParentOutOrderPos);
                    if (addedPositions.Any())
                    {
                        if (_UnSavedUnAssignedOutOrderPos.Any())
                            return AccessOutOrderPos.NavList.Except(addedPositions).Union(_UnSavedUnAssignedOutOrderPos);
                        else
                            return AccessOutOrderPos.NavList.Except(addedPositions);
                    }
                }
                if (_UnSavedUnAssignedOutOrderPos.Any())
                {
                    return AccessOutOrderPos.NavList.Union(_UnSavedUnAssignedOutOrderPos);
                }
                return AccessOutOrderPos.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected sales order position in the open positions list. Setting it also sets CurrentOutOrderPos.
        /// </summary>
        /// <value>The selected OutOrderPos.</value>
        [ACPropertySelected(652, OutOrderPos.ClassName, "en{'Sales Order Position'}de{'Auftragsposition'}", Description = @"Selected sales order position in the open positions list. Setting it also sets CurrentOutOrderPos.")]
        public OutOrderPos SelectedOutOrderPos
        {
            get
            {
                if (AccessOutOrderPos == null)
                    return null;
                return AccessOutOrderPos.Selected;
            }
            set
            {
                if (AccessOutOrderPos == null)
                    return;
                AccessOutOrderPos.Selected = value;
                OnPropertyChanged(nameof(SelectedOutOrderPos));
                CurrentOutOrderPos = value;
            }
        }
        #endregion

        #region Unpicked Deliverynotes
        OutOrderPos _CurrentDNoteOutOrderPos = null;
        /// <summary>
        /// Gets or sets the current position of an unpicked sales delivery note.
        /// </summary>
        /// <value>The current DNote OutOrderPos.</value>
        [ACPropertyCurrent(653, "DNoteOutOrderPos", "en{'Sales Delivery Note Position'}de{'VK-Lieferscheinposition'}", Description = @"Currently selected position of an unpicked sales delivery note. This position is assigned to the current picking with AssignDNoteOutOrderPos().")]
        public OutOrderPos CurrentDNoteOutOrderPos
        {
            get
            {
                return _CurrentDNoteOutOrderPos;
            }
            set
            {
                _CurrentDNoteOutOrderPos = value;
                OnPropertyChanged(nameof(CurrentDNoteOutOrderPos));
            }
        }

        /// <summary>
        /// Gets the list of positions of unpicked sales delivery notes (active when the delivery note view is activated).
        /// </summary>
        /// <value>The DNote OutOrderPos list.</value>
        [ACPropertyList(654, "DNoteOutOrderPos", "en{'Sales Delivery Note Positions'}de{'VK-Lieferscheinpositionen'}", Description = @"List of positions of unpicked sales delivery notes (DeliveryNotePos with OutOrderPos that has no parent position). Only filled when the delivery note view (_ActivateOutDNote) is activated.")]
        public IEnumerable<OutOrderPos> DNoteOutOrderPosList
        {
            get
            {
                if (_ActivateOutDNote)
                    return DatabaseApp.DeliveryNotePos.Where(c => c.OutOrderPos != null
                                                    && c.OutOrderPos.MDDelivPosState.MDDelivPosStateIndex <= (short)MDDelivPosState.DelivPosStates.Delivered
                                                    && !c.OutOrderPos.OutOrderPos_ParentOutOrderPos.Any()).Select(c => c.OutOrderPos).ToList();
                return null;
            }
        }

        OutOrderPos _SelectedDNoteOutOrderPos = null;
        /// <summary>
        /// Gets or sets the selected position of an unpicked sales delivery note. Setting it also sets CurrentDNoteOutOrderPos.
        /// </summary>
        /// <value>The selected DNote OutOrderPos.</value>
        [ACPropertySelected(655, "DNoteOutOrderPos", "en{'Sales Delivery Note Position'}de{'VK-Lieferscheinposition'}", Description = @"Selected position of an unpicked sales delivery note. Setting it also sets CurrentDNoteOutOrderPos.")]
        public OutOrderPos SelectedDNoteOutOrderPos
        {
            get
            {
                return _SelectedDNoteOutOrderPos;
            }
            set
            {
                _SelectedDNoteOutOrderPos = value;
                OnPropertyChanged(nameof(SelectedDNoteOutOrderPos));
                CurrentDNoteOutOrderPos = value;
            }
        }
        #endregion

        #endregion

        #region Methods

        #region OutOrderPos
        /// <summary>
        /// Assigns the selected sales order position to the current picking.
        /// </summary>
        [ACMethodCommand("xxx", "en{'Assign'}de{'Zuordnen'}", 650, true, Global.ACKinds.MSMethodPrePost, Description = @"Assigns the sales order position selected in the open positions list (CurrentOutOrderPos) to the current picking via PickingManager.AssignOutOrderPos. The PartialQuantity property allows a partial call-off; after assignment the position list is refreshed and PartialQuantity is reset.")]
        public virtual void AssignOutOrderPos()
        {
            if (!PreExecute("AssignOutOrderPos"))
                return;

            try
            {
                List<object> resultNewEntities = new List<object>();
                Msg result = PickingManager.AssignOutOrderPos(CurrentPicking, CurrentOutOrderPos, PartialQuantity, this.DatabaseApp, resultNewEntities);
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

                Messages.LogException("BSOPicking", "AssignOutOrderPos", msg);
            }

            // TODO: Es muss noch implementiert werden, wenn Teilmenge abgerufen worden dass Auftrags-Position nicht verschwindet
            if (_UnSavedUnAssignedOutOrderPos.Contains(CurrentOutOrderPos))
                _UnSavedUnAssignedOutOrderPos.Remove(CurrentOutOrderPos);
            OnPropertyChanged(nameof(PickingPosList));
            RefreshOutOrderPosList();
            PartialQuantity = null;

            PostExecute("AssignOutOrderPos");
        }

        /// <summary>
        /// Determines whether the Assign command is enabled (a sales order position is selected).
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignOutOrderPos()
        {
            if (CurrentOutOrderPos == null)
                return false;
            return true;
        }

        /// <summary>
        /// Opens the filter dialog for the open sales order positions list.
        /// </summary>
        [ACMethodInteraction("xxx", "en{'Filter'}de{'Filter'}", 651, false, Description = @"Opens the query/filter dialog (ShowACQueryDialog) for the open sales order positions list and refreshes the list after applying the filter.")]
        public async Task<bool> FilterDialogOutOrderPos()
        {
            if (AccessOutOrderPos == null)
                return false;
            bool result = await AccessOutOrderPos.ShowACQueryDialog();
            if (result)
            {
                RefreshOutOrderPosList();
            }
            return result;
        }

        public virtual void RefreshOutOrderPosList(bool forceQueryFromDb = false)
        {
            if ((_ActivateOutOpen || forceQueryFromDb) && AccessOutOrderPos != null)
                AccessOutOrderPos.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(OutOrderPosList));
        }
        #endregion

        #region Unpicked Deliverynotes
        /// <summary>
        /// Assigns the selected sales delivery note position to the current picking.
        /// </summary>
        [ACMethodCommand("DNoteOutOrderPos", "en{'Assign Sales Delivery Note'}de{'VK-Lieferschein zuordnen'}", 652, true, Global.ACKinds.MSMethodPrePost, Description = @"Assigns the position of an unpicked sales delivery note selected in the DNoteOutOrderPosList (CurrentDNoteOutOrderPos) to the current picking via PickingManager.AssignDNoteOutOrderPos. After assignment the position list is refreshed.")]
        public void AssignDNoteOutOrderPos()
        {
            if (!PreExecute("AssignDNoteOutOrderPos"))
                return;

            try
            {
                List<object> resultNewEntities = new List<object>();
                Msg result = PickingManager.AssignDNoteOutOrderPos(CurrentPicking, CurrentDNoteOutOrderPos, null, this.DatabaseApp, resultNewEntities);
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

                Messages.LogException("BSOPicking", "AssignDNoteOutOrderPos", msg);
            }

            if (_UnSavedUnAssignedOutOrderPos.Contains(CurrentDNoteOutOrderPos))
                _UnSavedUnAssignedOutOrderPos.Remove(CurrentDNoteOutOrderPos);
            OnPropertyChanged(nameof(PickingPosList));
            RefreshOutOrderPosList();
            PartialQuantity = null;

            PostExecute("AssignDNoteOutOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled assign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignDNoteOutOrderPos()
        {
            if (CurrentDNoteOutOrderPos == null)
                return false;
            return true;
        }
        #endregion

        #endregion

    }
}
