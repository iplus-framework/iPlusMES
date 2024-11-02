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

namespace gip.bso.logistics
{
    public partial class BSOPicking : ACBSOvbNav
    {
        #region Properties

        #region InOrderPos
        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<InOrderPos> _AccessInOrderPos;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(694, InOrderPos.ClassName)]
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
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(640, InOrderPos.ClassName)]
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
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(641, InOrderPos.ClassName)]
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
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(642, InOrderPos.ClassName)]
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
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(643, "DNoteInOrderPos")]
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
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(644, "DNoteInOrderPos")]
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
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(645, "DNoteInOrderPos")]
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
        /// Assigns the in order pos.
        /// </summary>
        [ACMethodCommand(InOrderPos.ClassName, "en{'Assign'}de{'Zuordnen'}", 640, true, Global.ACKinds.MSMethodPrePost)]
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
                    Messages.Msg(result);
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
        /// Determines whether [is enabled assign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignInOrderPos()
        {
            if (CurrentInOrderPos == null)
                return false;
            return true;
        }

        [ACMethodInteraction(InOrderPos.ClassName, "en{'Filter'}de{'Filter'}", 641, false)]
        public bool FilterDialogInOrderPos()
        {
            if (AccessInOrderPos == null)
                return false;
            bool result = AccessInOrderPos.ShowACQueryDialog();
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
        /// Assigns the in order pos.
        /// </summary>
        [ACMethodCommand("DNoteInOrderPos", "en{'Assign Purchase Delivery Note'}de{'EK-Lieferschein zuordnen'}", 642, true, Global.ACKinds.MSMethodPrePost)]
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
                    Messages.Msg(result);
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
