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
    public partial class BSOVisitorVoucher : ACBSOvbNav
    {
        #region BSO->ACProperty
        
        #region Deliverynotes assigned
        /// <summary>
        /// The _ current delivery note pos
        /// </summary>
        DeliveryNote _CurrentDeliveryNote;
        /// <summary>
        /// Gets or sets the current delivery note pos.
        /// </summary>
        /// <value>The current delivery note pos.</value>
        [ACPropertyCurrent(620, DeliveryNote.ClassName)]
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
        /// Gets the delivery note pos list.
        /// </summary>
        /// <value>The delivery note pos list.</value>
        [ACPropertyList(621, DeliveryNote.ClassName)]
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
        /// Gets or sets the selected delivery note pos.
        /// </summary>
        /// <value>The selected delivery note pos.</value>
        [ACPropertySelected(622, DeliveryNote.ClassName)]
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
        [ACPropertyAccess(692, "UnAssignedDeliveryNote")]
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

        [ACPropertyCurrent(623, "UnAssignedDeliveryNote")]
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


        [ACPropertyList(624, "UnAssignedDeliveryNote")]
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

        [ACPropertySelected(625, "UnAssignedDeliveryNote")]
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
        [ACMethodCommand("UnAssignedDeliveryNote", "en{'Assign'}de{'Zuordnen'}", 620, true, Global.ACKinds.MSMethodPrePost)]
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
                    Messages.Msg(result);
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

        [ACMethodCommand(DeliveryNote.ClassName, "en{'Remove'}de{'Entfernen'}", 621, true, Global.ACKinds.MSMethodPrePost)]
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
                    Messages.Msg(result);
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
        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Show Deliverynote'}de{'Lieferschein anzeigen'}", 622, false, "SelectedDeliveryNote")]
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

        [ACMethodInteraction("UnAssignedDeliveryNote", "en{'Show Deliverynote'}de{'Lieferschein anzeigen'}", 623, false, "SelectedUnAssignedDeliveryNote")]
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
