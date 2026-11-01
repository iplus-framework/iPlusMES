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
    /// Partial class of BSOVisitorVoucher that manages the assignment of tour plans (Tourplan) to a visitor voucher.
    /// This partial adds the assigned tour plan list (TourplanList with SelectedTourplan/CurrentTourplan) and the unassigned tour plan list
    /// (UnAssignedTourplanList with SelectedUnAssignedTourplan/CurrentUnAssignedTourplan, query "TourplanUnAssigned" filtered to tour plans without visitor voucher, sorted by TourplanNo).
    /// AssignTourplan() assigns the selected unassigned tour plan to the current visitor voucher via VisitorVoucherManager.AssignTourplan.
    /// If the vehicle of the tour plan differs from the vehicle of the checked-in visitor, a confirmation message (vehicle change) is shown first.
    /// Only one tour plan can be assigned per visitor voucher (IsEnabledAssignTourplan returns false if one is already assigned).
    /// UnassignTourplan() removes the assigned tour plan again.
    /// NavigateToATourplan()/NavigateToUTourplan() open the tour plan dialog for the selected record.
    /// RefreshUnAssignedTourplanList() re-queries the unassigned list.
    /// </summary>
    public partial class BSOVisitorVoucher : ACBSOvbNav
    {
        #region BSO->ACProperty

        #region Tourplan assigned
        /// <summary>
        /// The _ current delivery note pos
        /// </summary>
        Tourplan _CurrentTourplan;
        /// <summary>
        /// Gets or sets the current tour plan (assigned to the visitor voucher).
        /// </summary>
        /// <value>The current tour plan.</value>
        [ACPropertyCurrent(640, Tourplan.ClassName, "en{'Tourplan'}de{'Tourenplan'}", Description = @"Currently selected tour plan that is assigned to the current visitor voucher. Displayed via the TourplanList/SelectedTourplan.")]
        public Tourplan CurrentTourplan
        {
            get
            {
                return _CurrentTourplan;
            }
            set
            {
                _CurrentTourplan = value;
                OnPropertyChanged("CurrentTourplan");
            }

        }

        /// <summary>
        /// Gets the tour plan list assigned to the current visitor voucher.
        /// </summary>
        /// <value>The tour plan list.</value>
        [ACPropertyList(641, Tourplan.ClassName, "en{'Tourplan'}de{'Tourenplan'}", Description = @"List of tour plans assigned to the current visitor voucher (Tourplan_VisitorVoucher).")]
        public IEnumerable<Tourplan> TourplanList
        {
            get
            {
                if (CurrentVisitorVoucher == null)
                    return null;
                return CurrentVisitorVoucher.Tourplan_VisitorVoucher;
            }
        }

        /// <summary>
        /// The _ selected delivery note pos
        /// </summary>
        Tourplan _SelectedTourplan;
        /// <summary>
        /// Gets or sets the selected tour plan in the assigned tour plan list. Setting it also sets CurrentTourplan.
        /// </summary>
        /// <value>The selected tour plan.</value>
        [ACPropertySelected(642, Tourplan.ClassName, "en{'Tourplan'}de{'Tourenplan'}", Description = @"Selected tour plan in the assigned tour plan list. Setting it also sets CurrentTourplan.")]
        public Tourplan SelectedTourplan
        {
            get
            {
                return _SelectedTourplan;
            }
            set
            {
                _SelectedTourplan = value;
                OnPropertyChanged("SelectedTourplan");
                CurrentTourplan = value;
            }
        }
        #endregion

        #region Tourplan unassigned
        ACAccessNav<Tourplan> _AccessUnAssignedTourplan;
        /// <summary>
        /// Navigation access to the unassigned tour plans (query "TourplanUnAssigned").
        /// </summary>
        [ACPropertyAccess(694, "UnAssignedTourplan", "en{'Unassigned Tourplans'}de{'Nicht zugeordnete Tourenpläne'}", Description = @"Navigation access to the unassigned tour plans (query TourplanUnAssigned, filtered to tour plans without visitor voucher, sorted by TourplanNo). Provides the UnAssignedTourplanList.")]
        public ACAccessNav<Tourplan> AccessUnAssignedTourplan
        {
            get
            {
                if (_AccessUnAssignedTourplan == null && ACType != null)
                {
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
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "TourplanUnAssigned", ACType.ACIdentifier);
                    _AccessUnAssignedTourplan = navACQueryDefinition.NewAccessNav<Tourplan>("UnAssignedTourplan", this);
                    _AccessUnAssignedTourplan.AutoSaveOnNavigation = false;
                    _AccessUnAssignedTourplan.NavACQueryDefinition.CheckAndReplaceColumnsIfDifferent(AccessUnAssignedTourplan_DefaultFilter, AccessUnAssignedTourplan_DefaultSort);
                }
                return _AccessUnAssignedTourplan;
            }
        }

        private List<ACFilterItem> AccessUnAssignedTourplan_DefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "VisitorVoucherID", Global.LogicalOperators.isNull, Global.Operators.and, "", true),
                };
            }
        }

        private List<ACSortItem> AccessUnAssignedTourplan_DefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("TourplanNo", Global.SortDirections.ascending, true),
                };
            }
        }

        /// <summary>
        /// Gets or sets the current tour plan in the unassigned tour plan list.
        /// </summary>
        [ACPropertyCurrent(643, "UnAssignedTourplan", "en{'Unassigned Tourplan'}de{'Nicht zugeordneter Tourenplan'}", Description = @"Current record of the unassigned tour plan list. This tour plan is assigned to the visitor voucher with AssignTourplan().")]
        public Tourplan CurrentUnAssignedTourplan
        {
            get
            {
                if (AccessUnAssignedTourplan == null)
                    return null;
                return AccessUnAssignedTourplan.Current;
            }
            set
            {
                if (AccessUnAssignedTourplan == null)
                    return;
                AccessUnAssignedTourplan.Current = value;
                OnPropertyChanged("CurrentUnAssignedTourplan");
            }
        }


        /// <summary>
        /// Gets the list of tour plans that are not yet assigned to a visitor voucher.
        /// </summary>
        [ACPropertyList(644, "UnAssignedTourplan", "en{'Unassigned Tourplans'}de{'Nicht zugeordnete Tourenpläne'}", Description = @"List of tour plans that are not yet assigned to a visitor voucher (result of the query TourplanUnAssigned, minus the already assigned tour plans of the current voucher).")]
        public IEnumerable<Tourplan> UnAssignedTourplanList
        {
            get
            {
                if (AccessUnAssignedTourplan == null)
                    return null;
                if (CurrentVisitorVoucher != null && CurrentVisitorVoucher.Tourplan_VisitorVoucher.Count > 0)
                {
                    return AccessUnAssignedTourplan.NavList.Except(CurrentVisitorVoucher.Tourplan_VisitorVoucher);
                }
                return AccessUnAssignedTourplan.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected tour plan in the unassigned tour plan list. Setting it also sets CurrentUnAssignedTourplan.
        /// </summary>
        [ACPropertySelected(645, "UnAssignedTourplan", "en{'Unassigned Tourplan'}de{'Nicht zugeordneter Tourenplan'}", Description = @"Selected tour plan in the unassigned tour plan list. Setting it also sets CurrentUnAssignedTourplan.")]
        public Tourplan SelectedUnAssignedTourplan
        {
            get
            {
                if (AccessUnAssignedTourplan == null)
                    return null;
                return AccessUnAssignedTourplan.Selected;
            }
            set
            {
                AccessUnAssignedTourplan.Selected = value;
                OnPropertyChanged("SelectedUnAssignedTourplan");
                CurrentUnAssignedTourplan = value;
            }
        }
        #endregion

        #endregion

        #region BSO->ACMethod

        #region Un-/Assign Tourplan
        /// <summary>
        /// Assigns the selected unassigned tour plan to the current visitor voucher.
        /// </summary>
        [ACMethodCommand("UnAssignedTourplan", "en{'Assign'}de{'Zuordnen'}", 640, true, Global.ACKinds.MSMethodPrePost, Description = @"Assigns the tour plan selected in the unassigned tour plan list (CurrentUnAssignedTourplan) to the current visitor voucher via VisitorVoucherManager.AssignTourplan. If the tour plan vehicle differs from the visitor's vehicle, a confirmation message (vehicle change) is shown first. Enabled only if the voucher has no tour plan assigned yet.")]
        public async void AssignTourplan()
        {
            if (!IsEnabledAssignTourplan())
                return;
            if (!PreExecute("AssignTourplan"))
                return;

            // Bei Zurodnung von Tourenplan, führe Check durch ob geplanter LKW bei Anmeldung. Falls nicht, dann MEldung mit Bestätigung, dass Fahrzeugwechsel
            if (CurrentVisitorVoucher.Visitor != null && CurrentVisitorVoucher.Visitor.VehicleFacility != CurrentUnAssignedTourplan.VehicleFacility)
            {
                if (await Messages.QuestionAsync(this, "Question00009", Global.MsgResult.Yes) == Global.MsgResult.No)
                {
                    return;
                }
            }

            Msg result = null;
            try
            {
                result = VisitorVoucherManager.AssignTourplan(CurrentVisitorVoucher, CurrentUnAssignedTourplan, this.DatabaseApp);
                if (result != null)
                {
                    await Messages.MsgAsync(result);
                    return;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOVisitorVoucher", "AssignTourplan", msg);
                return;
            }

            RefreshLists();

            PostExecute("AssignTourplan");
        }

        public bool IsEnabledAssignTourplan()
        {
            if ((CurrentUnAssignedTourplan == null) || (CurrentVisitorVoucher == null))
                return false;
            if (CurrentVisitorVoucher.Tourplan_VisitorVoucher.Count > 0)
                return false;
            return true;
        }

        /// <summary>
        /// Removes the assigned tour plan from the current visitor voucher.
        /// </summary>
        [ACMethodCommand(Tourplan.ClassName, "en{'Remove'}de{'Entfernen'}", 641, true, Global.ACKinds.MSMethodPrePost, Description = @"Removes the tour plan selected in the assigned tour plan list (CurrentTourplan) from the current visitor voucher via VisitorVoucherManager.UnassignTourplan.")]
        public void UnassignTourplan()
        {
            if (!IsEnabledUnassignTourplan())
                return;
            if (!PreExecute("UnassignTourplan"))
                return;

            Msg result = null;
            try
            {
                result = VisitorVoucherManager.UnassignTourplan(CurrentVisitorVoucher, CurrentTourplan, this.DatabaseApp);
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

                Messages.LogException("BSOVisitorVoucher", "UnassignTourplan", msg);
                return;
            }

            RefreshLists();

            PostExecute("UnassignTourplan");
        }

        public bool IsEnabledUnassignTourplan()
        {
            if (CurrentVisitorVoucher == null || CurrentTourplan == null)
                return false;
            return true;
        }
        #endregion

        #region Refresh Lists

        public void RefreshUnAssignedTourplanList(bool forceQueryFromDb = false)
        {
            AccessUnAssignedTourplan.NavSearch(DatabaseApp);
            OnPropertyChanged("UnAssignedTourplanList");
        }

        #endregion

        #region Navigation
        /// <summary>
        /// Opens the tour plan dialog for the tour plan selected in the assigned list.
        /// </summary>
        [ACMethodInteraction(Tourplan.ClassName, "en{'Show Tourplan'}de{'Lieferschein anzeigen'}", 642, false, "SelectedTourplan", Description = @"Opens the tour plan dialog (ShowDialogOrder) for the tour plan selected in the assigned tour plan list.")]
        public void NavigateToATourplan()
        {
            if (!IsEnabledNavigateToATourplan())
                return; 
            NavigateToTourplan(SelectedTourplan);
        }

        public bool IsEnabledNavigateToATourplan()
        {
            return SelectedTourplan != null;
        }

        /// <summary>
        /// Opens the tour plan dialog for the tour plan selected in the unassigned list.
        /// </summary>
        [ACMethodInteraction("UnAssignedTourplan", "en{'Show Tourplan'}de{'Lieferschein anzeigen'}", 643, false, "SelectedUnAssignedTourplan", Description = @"Opens the tour plan dialog (ShowDialogOrder) for the tour plan selected in the unassigned tour plan list.")]
        public void NavigateToUTourplan()
        {
            if (!IsEnabledNavigateToUTourplan())
                return; 
            NavigateToTourplan(SelectedUnAssignedTourplan);
        }

        public bool IsEnabledNavigateToUTourplan()
        {
            return SelectedUnAssignedPicking != null;
        }

        private void NavigateToTourplan(Tourplan Tourplan)
        {
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = Tourplan.TourplanID,
                    EntityName = Tourplan.ClassName
                });
                service.ShowDialogOrder(this, info);
            }
        }
        #endregion

        #endregion
    }
}
