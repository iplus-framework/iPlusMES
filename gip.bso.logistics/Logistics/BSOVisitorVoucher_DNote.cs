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
using gip.mes.datamodel; using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;
using gip.bso.sales;
using gip.bso.purchasing;
using gip.mes.facility;

namespace gip.bso.logistics
{
    /// <summary>
    /// Partial class of BSOVisitorVoucher that manages the assignment of delivery notes (DeliveryNote) to a visitor voucher.
    /// This partial adds the assigned delivery note list (DeliveryNoteList with SelectedDeliveryNote/CurrentDeliveryNote) and the unassigned delivery note list
    /// (UnAssignedDeliveryNoteList with SelectedUnAssignedDeliveryNote/CurrentUnAssignedDeliveryNote, query "DeliveryNoteUnAssigned" filtered to delivery notes without visitor voucher, sorted by DeliveryNoteNo).
    /// AssignDeliveryNote() assigns the selected unassigned delivery note to the current visitor voucher via VisitorVoucherManager.AssignDeliveryNote.
    /// A delivery note that already belongs to a tour plan (TourplanPos set) cannot be assigned or removed here — this must be done via the tour plan assignment (AssignTourplan).
    /// UnassignDeliveryNote() removes the assigned delivery note again.
    /// NavigateToADeliveryNote()/NavigateToUDeliveryNote() open the delivery note dialog for the selected record.
    /// RefreshUnAssignedDeliveryNoteList() re-queries the unassigned list.
    /// </summary>
    public partial class BSOVisitorVoucher : ACBSOvbNav
    {
        #region BSO->ACProperty
        
        #region Deliverynotes assigned
        /// <summary>
        /// The _ current delivery note pos
        /// </summary>
        DeliveryNote _CurrentDeliveryNote;
        /// <summary>
        /// Gets or sets the current delivery note (assigned to the visitor voucher).
        /// </summary>
        /// <value>The current delivery note.</value>
        [ACPropertyCurrent(620, DeliveryNote.ClassName, "en{'Delivery Note'}de{'Lieferschein'}", Description = @"Currently selected delivery note that is assigned to the current visitor voucher. Displayed via the DeliveryNoteList/SelectedDeliveryNote.")]
        public DeliveryNote CurrentDeliveryNote
        {
            get
            {
                return _CurrentDeliveryNote;
            }
            set
            {
                _CurrentDeliveryNote = value;
                OnPropertyChanged("CurrentDeliveryNote");
            }

        }

        /// <summary>
        /// Gets the delivery note list assigned to the current visitor voucher.
        /// </summary>
        /// <value>The delivery note list.</value>
        [ACPropertyList(621, DeliveryNote.ClassName, "en{'Delivery Note'}de{'Lieferschein'}", Description = @"List of delivery notes assigned to the current visitor voucher (DeliveryNote_VisitorVoucher).")]
        public IEnumerable<DeliveryNote> DeliveryNoteList
        {
            get
            {
                if (CurrentVisitorVoucher == null)
                    return null;
                return CurrentVisitorVoucher.DeliveryNote_VisitorVoucher;
            }
        }

        /// <summary>
        /// The _ selected delivery note pos
        /// </summary>
        DeliveryNote _SelectedDeliveryNote;
        /// <summary>
        /// Gets or sets the selected delivery note in the assigned delivery note list. Setting it also sets CurrentDeliveryNote.
        /// </summary>
        /// <value>The selected delivery note.</value>
        [ACPropertySelected(622, DeliveryNote.ClassName, "en{'Delivery Note'}de{'Lieferschein'}", Description = @"Selected delivery note in the assigned delivery note list. Setting it also sets CurrentDeliveryNote.")]
        public DeliveryNote SelectedDeliveryNote
        {
            get
            {
                return _SelectedDeliveryNote;
            }
            set
            {
                _SelectedDeliveryNote = value;
                OnPropertyChanged("SelectedDeliveryNote");
                CurrentDeliveryNote = value;
            }
        }
        #endregion

        #region Deliverynotes unassigned
        ACAccessNav<DeliveryNote> _AccessUnAssignedDeliveryNote;
        /// <summary>
        /// Navigation access to the unassigned delivery notes (query "DeliveryNoteUnAssigned").
        /// </summary>
        [ACPropertyAccess(692, "UnAssignedDeliveryNote", "en{'Unassigned Delivery Notes'}de{'Nicht zugeordnete Lieferscheine'}", Description = @"Navigation access to the unassigned delivery notes (query DeliveryNoteUnAssigned, filtered to delivery notes without visitor voucher, sorted by DeliveryNoteNo). Provides the UnAssignedDeliveryNoteList.")]
        public ACAccessNav<DeliveryNote> AccessUnAssignedDeliveryNote
        {
            get
            {
                if (_AccessUnAssignedDeliveryNote == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "DeliveryNoteUnAssigned", ACType.ACIdentifier);
                    //bool rebuildACQueryDef = false;
                    //if (navACQueryDefinition.ACFilterColumns.Count <= 0)
                    //{
                    //    rebuildACQueryDef = true;
                    //}
                    //else
                    //{
                    //    int countFoundCorrect = 0;
                    //    foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                    //    {
                    //        if (filterItem.PropertyName == "VisitorVoucherID")
                    //        {
                    //            if (String.IsNullOrEmpty(filterItem.SearchWord) && filterItem.LogicalOperator == Global.LogicalOperators.isNull)
                    //                countFoundCorrect++;
                    //        }
                    //    }
                    //    if (countFoundCorrect < 1)
                    //        rebuildACQueryDef = true;
                    //}
                    //if (rebuildACQueryDef)
                    //{
                    //    navACQueryDefinition.ClearFilter(true);
                    //    navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "VisitorVoucherID", Global.LogicalOperators.isNull, Global.Operators.and, "", true));
                    //    navACQueryDefinition.SaveConfig(true);
                    //}
                    _AccessUnAssignedDeliveryNote = navACQueryDefinition.NewAccessNav<DeliveryNote>("UnAssignedDeliveryNote", this);
                    _AccessUnAssignedDeliveryNote.AutoSaveOnNavigation = false;
                    _AccessUnAssignedDeliveryNote.NavACQueryDefinition.CheckAndReplaceColumnsIfDifferent(AccessUnAssignedDeliveryNote_DefaultFilter, AccessUnAssignedDeliveryNote_DefaultSort);
                }
                return _AccessUnAssignedDeliveryNote;
            }
        }

        private List<ACFilterItem> AccessUnAssignedDeliveryNote_DefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "VisitorVoucherID", Global.LogicalOperators.isNull, Global.Operators.and, "", true),
                };
            }
        }

        private List<ACSortItem> AccessUnAssignedDeliveryNote_DefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("DeliveryNoteNo", Global.SortDirections.ascending, true),
                };
            }
        }

        /// <summary>
        /// Gets or sets the current delivery note in the unassigned delivery note list.
        /// </summary>
        [ACPropertyCurrent(623, "UnAssignedDeliveryNote", "en{'Unassigned Delivery Note'}de{'Nicht zugeordneter Lieferschein'}", Description = @"Current record of the unassigned delivery note list. This delivery note is assigned to the visitor voucher with AssignDeliveryNote().")]
        public DeliveryNote CurrentUnAssignedDeliveryNote
        {
            get
            {
                if (AccessUnAssignedDeliveryNote == null)
                    return null;
                return AccessUnAssignedDeliveryNote.Current;
            }
            set
            {
                if (AccessUnAssignedDeliveryNote == null)
                    return;
                AccessUnAssignedDeliveryNote.Current = value;
                OnPropertyChanged("CurrentUnAssignedDeliveryNote");
            }
        }


        /// <summary>
        /// Gets the list of delivery notes that are not yet assigned to a visitor voucher.
        /// </summary>
        [ACPropertyList(624, "UnAssignedDeliveryNote", "en{'Unassigned Delivery Notes'}de{'Nicht zugeordnete Lieferscheine'}", Description = @"List of delivery notes that are not yet assigned to a visitor voucher (result of the query DeliveryNoteUnAssigned, minus the already assigned delivery notes of the current voucher).")]
        public IEnumerable<DeliveryNote> UnAssignedDeliveryNoteList
        {
            get
            {
                if (AccessUnAssignedDeliveryNote == null)
                    return null;
                if (CurrentVisitorVoucher != null && CurrentVisitorVoucher.DeliveryNote_VisitorVoucher.Count > 0)
                {
                    return AccessUnAssignedDeliveryNote.NavList.Except(CurrentVisitorVoucher.DeliveryNote_VisitorVoucher);
                }
                return AccessUnAssignedDeliveryNote.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected delivery note in the unassigned delivery note list. Setting it also sets CurrentUnAssignedDeliveryNote.
        /// </summary>
        [ACPropertySelected(625, "UnAssignedDeliveryNote", "en{'Unassigned Delivery Note'}de{'Nicht zugeordneter Lieferschein'}", Description = @"Selected delivery note in the unassigned delivery note list. Setting it also sets CurrentUnAssignedDeliveryNote.")]
        public DeliveryNote SelectedUnAssignedDeliveryNote
        {
            get
            {
                if (AccessUnAssignedDeliveryNote == null)
                    return null;
                return AccessUnAssignedDeliveryNote.Selected;
            }
            set
            {
                if (AccessUnAssignedDeliveryNote == null)
                    return;
                AccessUnAssignedDeliveryNote.Selected = value;
                OnPropertyChanged("SelectedUnAssignedDeliveryNote");
                CurrentUnAssignedDeliveryNote = value;
            }
        }
        #endregion
        
        #endregion

        #region BSO->ACMethod

        #region Un-/Assign DeliveryNote
        /// <summary>
        /// Assigns the selected unassigned delivery note to the current visitor voucher.
        /// </summary>
        [ACMethodCommand("UnAssignedDeliveryNote", "en{'Assign'}de{'Zuordnen'}", 620, true, Global.ACKinds.MSMethodPrePost, Description = @"Assigns the delivery note selected in the unassigned delivery note list (CurrentUnAssignedDeliveryNote) to the current visitor voucher via VisitorVoucherManager.AssignDeliveryNote. Disabled for delivery notes that already belong to a tour plan (TourplanPos set) — these are assigned via the tour plan.")]
        public void AssignDeliveryNote()
        {
            if (!IsEnabledAssignDeliveryNote())
                return;
            if (!PreExecute("AssignDeliveryNote"))
                return;

            Msg result = null;
            try
            {
                result = VisitorVoucherManager.AssignDeliveryNote(CurrentVisitorVoucher, CurrentUnAssignedDeliveryNote, this.DatabaseApp);
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

                Messages.LogException("BSOVisitorVoucher", "AssignDeliveryNote", msg);
                return;
            }

            RefreshLists();
            PostExecute("AssignDeliveryNote");
        }

        public bool IsEnabledAssignDeliveryNote()
        {
            if ((CurrentUnAssignedDeliveryNote == null) || (CurrentVisitorVoucher == null))
                return false;
            // Falls Delivery-Note bereits einer Tour zugeordnet, dann muss die Zuordnung über die Methode AssignTourplan() erfolgen
            if (CurrentUnAssignedDeliveryNote.TourplanPos != null)
                return false;
            return true;
        }

        /// <summary>
        /// Removes the assigned delivery note from the current visitor voucher.
        /// </summary>
        [ACMethodCommand(DeliveryNote.ClassName, "en{'Remove'}de{'Entfernen'}", 621, true, Global.ACKinds.MSMethodPrePost, Description = @"Removes the delivery note selected in the assigned delivery note list (CurrentDeliveryNote) from the current visitor voucher via VisitorVoucherManager.UnassignDeliveryNote. Disabled for delivery notes that belong to a tour plan (TourplanPos set).")]
        public void UnassignDeliveryNote()
        {
            if (!IsEnabledUnassignDeliveryNote())
                return;
            if (!PreExecute("UnassignDeliveryNote"))
                return;


            Msg result = null;
            try
            {
                result = VisitorVoucherManager.UnassignDeliveryNote(CurrentVisitorVoucher, CurrentDeliveryNote, this.DatabaseApp);
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

                Messages.LogException("BSOVisitorVoucher", "UnassignDeliveryNote", msg);
                return;
            }

            RefreshLists();
            PostExecute("UnassignDeliveryNote");
        }

        public bool IsEnabledUnassignDeliveryNote()
        {
            if ((CurrentDeliveryNote == null) || (CurrentVisitorVoucher == null))
                return false;
            // Abwählen einer zugeordneten DeliveryNotePos-Position darf nicht gehen, wenn Tourenplan dahinter steckt
            if (CurrentDeliveryNote.TourplanPos != null)
                return false;
            return true;
        }
        #endregion

        #region Refresh Lists

        public void RefreshUnAssignedDeliveryNoteList(bool forceQueryFromDb = false)
        {
            if (AccessUnAssignedDeliveryNote == null)
                return;
            AccessUnAssignedDeliveryNote.NavSearch(DatabaseApp);
            OnPropertyChanged("UnAssignedDeliveryNoteList");
        }

        #endregion

        #region Navigation
        /// <summary>
        /// Opens the delivery note dialog for the delivery note selected in the assigned list.
        /// </summary>
        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Show Deliverynote'}de{'Lieferschein anzeigen'}", 622, false, "SelectedDeliveryNote", Description = @"Opens the delivery note dialog (ShowDialogOrder) for the delivery note selected in the assigned delivery note list.")]
        public void NavigateToADeliveryNote()
        {
            if (!IsEnabledNavigateToADeliveryNote())
                return;
            NavigateToDeliveryNote(SelectedDeliveryNote);
        }

        public bool IsEnabledNavigateToADeliveryNote()
        {
            return SelectedDeliveryNote != null;
        }

        /// <summary>
        /// Opens the delivery note dialog for the delivery note selected in the unassigned list.
        /// </summary>
        [ACMethodInteraction("UnAssignedDeliveryNote", "en{'Show Deliverynote'}de{'Lieferschein anzeigen'}", 623, false, "SelectedUnAssignedDeliveryNote", Description = @"Opens the delivery note dialog (ShowDialogOrder) for the delivery note selected in the unassigned delivery note list.")]
        public void NavigateToUDeliveryNote()
        {
            if (!IsEnabledNavigateToUDeliveryNote())
                return;
            NavigateToDeliveryNote(SelectedUnAssignedDeliveryNote);
        }

        public bool IsEnabledNavigateToUDeliveryNote()
        {
            return SelectedUnAssignedPicking != null;
        }

        private void NavigateToDeliveryNote(DeliveryNote deliveryNote)
        {
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = deliveryNote.DeliveryNoteID,
                    EntityName = DeliveryNote.ClassName
                });
                service.ShowDialogOrder(this, info);
            }
        }
        #endregion

        #endregion
    }
}
