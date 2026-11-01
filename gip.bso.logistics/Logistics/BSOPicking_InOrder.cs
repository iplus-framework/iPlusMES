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
    /// Partial class of BSOPicking that manages the assignment of purchase order positions (InOrderPos) to a picking order.
    /// This partial adds the open purchase order positions (query "InOrderPosOpen", filtered to positions not completely assigned and without parent position)
    /// in the InOrderPosList property with SelectedInOrderPos/CurrentInOrderPos, and the positions of unpicked purchase delivery notes in the DNoteInOrderPosList property
    /// with SelectedDNoteInOrderPos/CurrentDNoteInOrderPos (active when _ActivateInDNote is set).
    /// AssignInOrderPos() assigns the selected purchase order position to the current picking (via PickingManager.AssignInOrderPos, considering the PartialQuantity for partial call-off).
    /// AssignDNoteInOrderPos() assigns the selected position of a purchase delivery note to the current picking.
    /// FilterDialogInOrderPos() opens the filter dialog for the open positions list; RefreshInOrderPosList() re-queries the list.
    /// </summary>
    public partial class BSOPicking : ACBSOvbNav
    {
        #region Properties

        #region InOrderPos
        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<InOrderPos> _AccessInOrderPos;
        /// <summary>
        /// Gets the access to the open purchase order positions.
        /// </summary>
        /// <value>The access to the open InOrderPos.</value>
        [ACPropertyAccess(694, InOrderPos.ClassName, "en{'Open Purchase Order Positions'}de{'Offene Bestellpositionen'}", Description = @"Navigation access to the open purchase order positions (query InOrderPosOpen, filtered to positions with delivery state less than CompletelyAssigned and without parent position). Provides the InOrderPosList.")]
        public ACAccessNav<InOrderPos> AccessInOrderPos
        {
            get
            {
                if (_AccessInOrderPos == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "InOrderPosOpen", ACType.ACIdentifier);
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
                            else if (filterItem.PropertyName == "InOrderPos1_ParentInOrderPos")
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
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "InOrderPos1_ParentInOrderPos", Global.LogicalOperators.isNull, Global.Operators.and, "", true));
                        navACQueryDefinition.SaveConfig(true);
                    }
                    _AccessInOrderPos = navACQueryDefinition.NewAccessNav<InOrderPos>(InOrderPos.ClassName, this);
                    _AccessInOrderPos.AutoSaveOnNavigation = false;
                }
                return _AccessInOrderPos;
            }
        }

        /// <summary>
        /// Gets or sets the current purchase order position (open InOrderPos).
        /// </summary>
        /// <value>The current InOrderPos.</value>
        [ACPropertyCurrent(640, InOrderPos.ClassName, "en{'Purchase Order Position'}de{'Bestellposition'}", Description = @"Currently selected open purchase order position. This position is assigned to the current picking with AssignInOrderPos().")]
        public InOrderPos CurrentInOrderPos
        {
            get
            {
                return AccessInOrderPos.Current;
            }
            set
            {
                AccessInOrderPos.Current = value;
                OnPropertyChanged(nameof(CurrentInOrderPos));
            }
        }

        /// <summary>
        /// Gets the list of open purchase order positions that can be assigned to the current picking.
        /// </summary>
        /// <value>The InOrderPos list.</value>
        [ACPropertyList(641, InOrderPos.ClassName, "en{'Open Purchase Order Positions'}de{'Offene Bestellpositionen'}", Description = @"List of open purchase order positions (query InOrderPosOpen). Positions already fully assigned by unsaved picking positions of the current picking are hidden; unsaved unassigned positions are merged in.")]
        public IEnumerable<InOrderPos> InOrderPosList
        {
            get
            {
                if (AccessInOrderPos == null)
                    return null;
                if (CurrentPicking != null)
                {
                    IEnumerable<InOrderPos> addedPositions = CurrentPicking.PickingPos_Picking.Where(c => c.EntityState == Microsoft.EntityFrameworkCore.EntityState.Added
                        && c.InOrderPos != null
                        && c.InOrderPos.InOrderPos1_ParentInOrderPos != null
                        && c.InOrderPos.InOrderPos1_ParentInOrderPos.MDDelivPosState == StateCompletelyAssigned
                        ).Select(c => c.InOrderPos.InOrderPos1_ParentInOrderPos);
                    if (addedPositions.Any())
                    {
                        if (_UnSavedUnAssignedInOrderPos.Any())
                            return AccessInOrderPos.NavList.Except(addedPositions).Union(_UnSavedUnAssignedInOrderPos);
                        else
                            return AccessInOrderPos.NavList.Except(addedPositions);
                    }
                }
                if (_UnSavedUnAssignedInOrderPos.Any())
                {
                    return AccessInOrderPos.NavList.Union(_UnSavedUnAssignedInOrderPos);
                }
                return AccessInOrderPos.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected purchase order position in the open positions list. Setting it also sets CurrentInOrderPos.
        /// </summary>
        /// <value>The selected InOrderPos.</value>
        [ACPropertySelected(642, InOrderPos.ClassName, "en{'Purchase Order Position'}de{'Bestellposition'}", Description = @"Selected purchase order position in the open positions list. Setting it also sets CurrentInOrderPos.")]
        public InOrderPos SelectedInOrderPos
        {
            get
            {
                if (AccessInOrderPos == null)
                    return null;
                return AccessInOrderPos.Selected;
            }
            set
            {
                if (AccessInOrderPos == null)
                    return;
                AccessInOrderPos.Selected = value;
                OnPropertyChanged(nameof(SelectedInOrderPos));
                CurrentInOrderPos = value;
            }
        }
        #endregion

        #region Unpicked Deliverynotes
        InOrderPos _CurrentDNoteInOrderPos = null;
        /// <summary>
        /// Gets or sets the current position of an unpicked purchase delivery note.
        /// </summary>
        /// <value>The current DNote InOrderPos.</value>
        [ACPropertyCurrent(643, "DNoteInOrderPos", "en{'Purchase Delivery Note Position'}de{'EK-Lieferscheinposition'}", Description = @"Currently selected position of an unpicked purchase delivery note. This position is assigned to the current picking with AssignDNoteInOrderPos().")]
        public InOrderPos CurrentDNoteInOrderPos
        {
            get
            {
                return _CurrentDNoteInOrderPos;
            }
            set
            {
                _CurrentDNoteInOrderPos = value;
                OnPropertyChanged(nameof(CurrentDNoteInOrderPos));
            }
        }

        /// <summary>
        /// Gets the list of positions of unpicked purchase delivery notes (active when the delivery note view is activated).
        /// </summary>
        /// <value>The DNote InOrderPos list.</value>
        [ACPropertyList(644, "DNoteInOrderPos", "en{'Purchase Delivery Note Positions'}de{'EK-Lieferscheinpositionen'}", Description = @"List of positions of unpicked purchase delivery notes (DeliveryNotePos with InOrderPos that has no parent position). Only filled when the delivery note view (_ActivateInDNote) is activated.")]
        public IEnumerable<InOrderPos> DNoteInOrderPosList
        {
            get
            {
                if (_ActivateInDNote)
                    return DatabaseApp.DeliveryNotePos.Where(c => c.InOrderPos != null
                                                    && c.InOrderPos.MDDelivPosState.MDDelivPosStateIndex <= (short)MDDelivPosState.DelivPosStates.Delivered
                                                    && !c.InOrderPos.InOrderPos_ParentInOrderPos.Any()).Select(c => c.InOrderPos).ToList();
                return null;
            }
        }

        InOrderPos _SelectedDNoteInOrderPos = null;
        /// <summary>
        /// Gets or sets the selected position of an unpicked purchase delivery note. Setting it also sets CurrentDNoteInOrderPos.
        /// </summary>
        /// <value>The selected DNote InOrderPos.</value>
        [ACPropertySelected(645, "DNoteInOrderPos", "en{'Purchase Delivery Note Position'}de{'EK-Lieferscheinposition'}", Description = @"Selected position of an unpicked purchase delivery note. Setting it also sets CurrentDNoteInOrderPos.")]
        public InOrderPos SelectedDNoteInOrderPos
        {
            get
            {
                return _SelectedDNoteInOrderPos;
            }
            set
            {
                _SelectedDNoteInOrderPos = value;
                OnPropertyChanged(nameof(SelectedDNoteInOrderPos));
                CurrentDNoteInOrderPos = value;
            }
        }
        #endregion

        #endregion

        #region Methods

        #region InOrderPos
        /// <summary>
        /// Assigns the selected purchase order position to the current picking.
        /// </summary>
        [ACMethodCommand(InOrderPos.ClassName, "en{'Assign'}de{'Zuordnen'}", 640, true, Global.ACKinds.MSMethodPrePost, Description = @"Assigns the purchase order position selected in the open positions list (CurrentInOrderPos) to the current picking via PickingManager.AssignInOrderPos. The PartialQuantity property allows a partial call-off; after assignment the position list is refreshed and PartialQuantity is reset.")]
        public void AssignInOrderPos()
        {
            if (!PreExecute("AssignInOrderPos"))
                return;

            try
            {
                List<object> resultNewEntities = new List<object>();
                Msg result = PickingManager.AssignInOrderPos(CurrentPicking, CurrentInOrderPos, PartialQuantity, this.DatabaseApp, resultNewEntities);
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

                Messages.LogException(nameof(BSOPicking), nameof(AssignInOrderPos), msg);
            }

            // TODO: Es muss noch implementiert werden, wenn Teilmenge abgerufen worden dass Auftrags-Position nicht verschwindet
            if (_UnSavedUnAssignedInOrderPos.Contains(CurrentInOrderPos))
                _UnSavedUnAssignedInOrderPos.Remove(CurrentInOrderPos);
            OnPropertyChanged(nameof(PickingPosList));
            RefreshInOrderPosList();
            PartialQuantity = null;
        }

        /// <summary>
        /// Determines whether the Assign command is enabled (a purchase order position is selected).
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignInOrderPos()
        {
            if (CurrentInOrderPos == null)
                return false;
            return true;
        }

        /// <summary>
        /// Opens the filter dialog for the open purchase order positions list.
        /// </summary>
        [ACMethodInteraction(InOrderPos.ClassName, "en{'Filter'}de{'Filter'}", 641, false, Description = @"Opens the query/filter dialog (ShowACQueryDialog) for the open purchase order positions list and refreshes the list after applying the filter.")]
        public async Task<bool> FilterDialogInOrderPos()
        {
            if (AccessInOrderPos == null)
                return false;
            bool result = await AccessInOrderPos.ShowACQueryDialog();
            if (result)
            {
                RefreshInOrderPosList();
            }
            return result;
        }

        public virtual void RefreshInOrderPosList(bool forceQueryFromDb = false)
        {
            if (AccessInOrderPos == null)
                return;
            if (_ActivateInOpen || forceQueryFromDb)
                AccessInOrderPos.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(InOrderPosList));
        }
        #endregion

        #region Unpicked Deliverynotes
        /// <summary>
        /// Assigns the selected purchase delivery note position to the current picking.
        /// </summary>
        [ACMethodCommand("DNoteInOrderPos", "en{'Assign Purchase Delivery Note'}de{'EK-Lieferschein zuordnen'}", 642, true, Global.ACKinds.MSMethodPrePost, Description = @"Assigns the position of an unpicked purchase delivery note selected in the DNoteInOrderPosList (CurrentDNoteInOrderPos) to the current picking via PickingManager.AssignDNoteInOrderPos. After assignment the position list is refreshed.")]
        public void AssignDNoteInOrderPos()
        {
            if (!PreExecute("AssignDNoteInOrderPos"))
                return;

            try
            {
                List<object> resultNewEntities = new List<object>();
                Msg result = PickingManager.AssignDNoteInOrderPos(CurrentPicking, CurrentDNoteInOrderPos, null, this.DatabaseApp, resultNewEntities);
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

                Messages.LogException("BSOPicking", "AssignDNoteInOrderPos", msg);
            }

            if (_UnSavedUnAssignedInOrderPos.Contains(CurrentDNoteInOrderPos))
                _UnSavedUnAssignedInOrderPos.Remove(CurrentDNoteInOrderPos);
            OnPropertyChanged(nameof(PickingPosList));
            RefreshInOrderPosList();
            PartialQuantity = null;
        }

        /// <summary>
        /// Determines whether [is enabled assign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignDNoteInOrderPos()
        {
            if (CurrentDNoteInOrderPos == null)
                return false;
            return true;
        }
        #endregion
        #endregion
    }
}
