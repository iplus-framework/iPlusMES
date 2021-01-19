// ***********************************************************************
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

        #region OutOrderPos
        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<OutOrderPos> _AccessOutOrderPos;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(695, OutOrderPos.ClassName)]
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
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(650, OutOrderPos.ClassName)]
        public OutOrderPos CurrentOutOrderPos
        {
            get
            {
                return AccessOutOrderPos.Current;
            }
            set
            {
                AccessOutOrderPos.Current = value;
                OnPropertyChanged("CurrentOutOrderPos");
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(651, OutOrderPos.ClassName)]
        public IEnumerable<OutOrderPos> OutOrderPosList
        {
            get
            {
                if (AccessOutOrderPos == null)
                    return null;
                if (CurrentPicking != null)
                {
                    IEnumerable<OutOrderPos> addedPositions = CurrentPicking.PickingPos_Picking.Where(c => c.EntityState == System.Data.EntityState.Added
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
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(652, OutOrderPos.ClassName)]
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
                OnPropertyChanged("SelectedOutOrderPos");
                CurrentOutOrderPos = value;
            }
        }
        #endregion

        #region Unpicked Deliverynotes
        OutOrderPos _CurrentDNoteOutOrderPos = null;
        /// <summary>
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(653, "DNoteOutOrderPos")]
        public OutOrderPos CurrentDNoteOutOrderPos
        {
            get
            {
                return _CurrentDNoteOutOrderPos;
            }
            set
            {
                _CurrentDNoteOutOrderPos = value;
                OnPropertyChanged("CurrentDNoteOutOrderPos");
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(654, "DNoteOutOrderPos")]
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
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(655, "DNoteOutOrderPos")]
        public OutOrderPos SelectedDNoteOutOrderPos
        {
            get
            {
                return _SelectedDNoteOutOrderPos;
            }
            set
            {
                _SelectedDNoteOutOrderPos = value;
                OnPropertyChanged("SelectedDNoteOutOrderPos");
                CurrentDNoteOutOrderPos = value;
            }
        }
        #endregion

        #endregion

        #region Methods

        #region OutOrderPos
        /// <summary>
        /// Assigns the in order pos.
        /// </summary>
        [ACMethodCommand("xxx", "en{'Assign'}de{'Zuordnen'}", 650, true, Global.ACKinds.MSMethodPrePost)]
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
                    Messages.Msg(result);
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
            OnPropertyChanged("PickingPosList");
            RefreshOutOrderPosList();
            PartialQuantity = null;

            PostExecute("AssignOutOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled assign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignOutOrderPos()
        {
            if (CurrentOutOrderPos == null)
                return false;
            return true;
        }

        [ACMethodInteraction("xxx", "en{'Filter'}de{'Filter'}", 651, false)]
        public bool FilterDialogOutOrderPos()
        {
            if (AccessOutOrderPos == null)
                return false;
            bool result = AccessOutOrderPos.ShowACQueryDialog();
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
            OnPropertyChanged("OutOrderPosList");
        }
        #endregion

        #region Unpicked Deliverynotes
        /// <summary>
        /// Assigns the in order pos.
        /// </summary>
        [ACMethodCommand("DNoteOutOrderPos", "en{'Assign Sales Delivery Note'}de{'VK-Lieferschein zuordnen'}", 652, true, Global.ACKinds.MSMethodPrePost)]
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
                    Messages.Msg(result);
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
            OnPropertyChanged("PickingPosList");
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
