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
using gip.mes.datamodel; 
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;

using Microsoft.EntityFrameworkCore;
using static gip.core.datamodel.Global;

namespace gip.bso.logistics
{
    /// <summary>
    /// Partial class of BSOVisitorVoucher that manages the assignment of picking orders (Picking) to a visitor voucher.
    /// This partial adds the filter properties FilterPickingDateFrom/FilterPickingDateTo (delivery date range), FilterPickingMaterialNo and FilterPickingKeyOfExternalSys for the unassigned picking list.
    /// The assigned pickings of the current visitor voucher are provided in the PickingList property; the selected record is held in SelectedPicking/CurrentPicking.
    /// The unassigned pickings (query "PickingUnAssigned", filtered to pickings without visitor voucher of the types ReceiptVehicle/IssueVehicle) are provided in the UnAssignedPickingList property with SelectedUnAssignedPicking/CurrentUnAssignedPicking.
    /// AssignPicking() assigns the selected unassigned picking to the current visitor voucher (via VisitorVoucherManager.AssignPicking); UnassignPicking() removes the selected assigned picking again.
    /// NavigateToAPicking()/NavigateToUPicking() open the picking order dialog for the selected record.
    /// RefreshUnAssignedPickingList() re-queries the unassigned list after a filter change; CleanFilterPicking() resets the filter fields.
    /// </summary>
    public partial class BSOVisitorVoucher : ACBSOvbNav
    {
        #region Properties

        #region Picking Filter
        #region Picking -> Filter -> Properties -> Date

        /// <summary>
        /// Filter for the delivery date (from) of the unassigned picking list. Changing the value refreshes the list.
        /// </summary>
        [ACPropertyInfo(301, nameof(FilterPickingDateFrom), "en{'From'}de{'Von'}", Description = @"Filter for the delivery date (from) of the unassigned picking list. Changing the value refreshes the list.")]
        public DateTime? FilterPickingDateFrom
        {
            get
            {
                string tmp = AccessUnAssignedPicking.NavACQueryDefinition.GetSearchValue<string>(nameof(Picking.DeliveryDateFrom));
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessUnAssignedPicking.NavACQueryDefinition.GetSearchValue<DateTime>(nameof(Picking.DeliveryDateFrom));
            }
            set
            {
                if (value.HasValue)
                {
                    AccessUnAssignedPicking.NavACQueryDefinition.SetSearchValue(nameof(Picking.DeliveryDateFrom), Global.LogicalOperators.greaterThanOrEqual, value.Value);
                    OnPropertyChanged();
                    RefreshUnAssignedPickingList();
                }
                else
                {
                    AccessUnAssignedPicking.NavACQueryDefinition.SetSearchValue(nameof(Picking.DeliveryDateFrom), Global.LogicalOperators.greaterThanOrEqual, "");
                    OnPropertyChanged();
                    RefreshUnAssignedPickingList();
                }
            }
        }

        /// <summary>
        /// Filter for the delivery date (to) of the unassigned picking list. Changing the value refreshes the list.
        /// </summary>
        [ACPropertyInfo(302, nameof(FilterPickingDateTo), "en{'to'}de{'bis'}", Description = @"Filter for the delivery date (to) of the unassigned picking list. Changing the value refreshes the list.")]
        public DateTime? FilterPickingDateTo
        {
            get
            {
                string tmp = AccessUnAssignedPicking.NavACQueryDefinition.GetSearchValue<string>(nameof(Picking.DeliveryDateTo));
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessUnAssignedPicking.NavACQueryDefinition.GetSearchValue<DateTime>(nameof(Picking.DeliveryDateTo));
            }
            set
            {
                if (value.HasValue)
                {
                    AccessUnAssignedPicking.NavACQueryDefinition.SetSearchValue(nameof(Picking.DeliveryDateTo), Global.LogicalOperators.lessThan, value.Value);
                    OnPropertyChanged();
                    RefreshUnAssignedPickingList();
                }
                else
                {
                    AccessUnAssignedPicking.NavACQueryDefinition.SetSearchValue(nameof(Picking.DeliveryDateTo), Global.LogicalOperators.lessThan, "");
                    OnPropertyChanged();
                    RefreshUnAssignedPickingList();
                }
            }
        }

        #endregion

        private string _FilterPickingMaterialNo;
        /// <summary>
        /// Filter for the material number of the unassigned picking list. Changing the value refreshes the list.
        /// </summary>
        [ACPropertyInfo(304, nameof(FilterPickingMaterialNo), ConstApp.Material, Description = @"Filter for the material number of the unassigned picking list. Changing the value refreshes the list.")]
        public string FilterPickingMaterialNo
        {
            get
            {
                return _FilterPickingMaterialNo;
            }
            set
            {
                if (_FilterPickingMaterialNo != value)
                {
                    _FilterPickingMaterialNo = value;
                    OnPropertyChanged();
                    RefreshUnAssignedPickingList();
                }
            }
        }

        /// <summary>
        /// Filter for the external system key (KeyOfExtSys) of the unassigned picking list. Changing the value refreshes the list.
        /// </summary>
        [ACPropertyInfo(305, nameof(FilterPickingKeyOfExternalSys), ConstApp.EntityTranslateKeyOfExtSys, Description = @"Filter for the external system key (KeyOfExtSys) of the unassigned picking list. Changing the value refreshes the list.")]
        public virtual string FilterPickingKeyOfExternalSys
        {
            get
            {
                string tmp = AccessUnAssignedPicking.NavACQueryDefinition.GetSearchValue<string>(nameof(Picking.KeyOfExtSys));
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessUnAssignedPicking.NavACQueryDefinition.GetSearchValue<string>(nameof(Picking.KeyOfExtSys));
            }
            set
            {
                AccessUnAssignedPicking.NavACQueryDefinition.SetSearchValue(nameof(Picking.KeyOfExtSys), Global.LogicalOperators.contains, value ?? "");
                OnPropertyChanged();
                RefreshUnAssignedPickingList();
            }
        }

        #endregion

        #endregion


        #region BSO->ACProperty

        #region Picking assigned
        /// <summary>
        /// The _ current delivery note pos
        /// </summary>
        Picking _CurrentPicking;
        /// <summary>
        /// Gets or sets the current picking (assigned to the visitor voucher).
        /// </summary>
        /// <value>The current picking.</value>
        [ACPropertyCurrent(630, "Picking", "en{'Picking'}de{'Kommissionierauftrag'}", Description = @"Currently selected picking that is assigned to the current visitor voucher. Displayed and edited via the PickingList/SelectedPicking.")]
        public Picking CurrentPicking
        {
            get
            {
                return _CurrentPicking;
            }
            set
            {
                _CurrentPicking = value;
                OnPropertyChanged("CurrentPicking");
            }

        }

        /// <summary>
        /// Gets the picking list assigned to the current visitor voucher.
        /// </summary>
        /// <value>The picking list.</value>
        [ACPropertyList(631, "Picking", "en{'Picking'}de{'Kommissionierauftrag'}", Description = @"List of pickings assigned to the current visitor voucher (Picking_VisitorVoucher).")]
        public IEnumerable<Picking> PickingList
        {
            get
            {
                if (CurrentVisitorVoucher == null)
                    return null;
                return CurrentVisitorVoucher.Picking_VisitorVoucher;
            }
        }

        /// <summary>
        /// The _ selected delivery note pos
        /// </summary>
        Picking _SelectedPicking;
        /// <summary>
        /// Gets or sets the selected picking in the assigned picking list. Setting it also sets CurrentPicking.
        /// </summary>
        /// <value>The selected picking.</value>
        [ACPropertySelected(632, "Picking", "en{'Picking'}de{'Kommissionierauftrag'}", Description = @"Selected picking in the assigned picking list. Setting it also sets CurrentPicking.")]
        public Picking SelectedPicking
        {
            get
            {
                return _SelectedPicking;
            }
            set
            {
                _SelectedPicking = value;
                OnPropertyChanged("SelectedPicking");
                CurrentPicking = value;
            }
        }
        #endregion

        #region Picking unassigned
        ACAccessNav<Picking> _AccessUnAssignedPicking;
        /// <summary>
        /// Navigation access to the unassigned pickings (query "PickingUnAssigned").
        /// </summary>
        [ACPropertyAccess(693, "UnAssignedPicking", "en{'Unassigned Pickings'}de{'Nicht zugeordnete Kommissionieraufträge'}", Description = @"Navigation access to the unassigned pickings (query PickingUnAssigned, filtered to pickings without visitor voucher of the types ReceiptVehicle/IssueVehicle). Provides the UnAssignedPickingList.")]
        public ACAccessNav<Picking> AccessUnAssignedPicking
        {
            get
            {
                if (_AccessUnAssignedPicking == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "PickingUnAssigned", ACType.ACIdentifier);
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
                    //        if (filterItem.PropertyName == "PickingTypeIndex")
                    //        {
                    //            string pickingTypeNo = System.Convert.ToString((short)GlobalApp.PickingType.ReceiptVehicle);
                    //            if ((filterItem.SearchWord == pickingTypeNo) && filterItem.LogicalOperator == Global.LogicalOperators.greaterThanOrEqual)
                    //                countFoundCorrect++;
                    //        }
                    //        //else if (filterItem.PropertyName == "PickingStateIndex")
                    //        //{
                    //        //    string pickingState = System.Convert.ToString((short)GlobalApp.PickingState.New);
                    //        //    if ((filterItem.SearchWord == pickingState) && filterItem.LogicalOperator == Global.LogicalOperators.greaterThan)
                    //        //        countFoundCorrect++;
                    //        //}
                    //        else if (filterItem.PropertyName == "VisitorVoucherID")
                    //        {
                    //            if (String.IsNullOrEmpty(filterItem.SearchWord) && filterItem.LogicalOperator == Global.LogicalOperators.isNull)
                    //                countFoundCorrect++;
                    //        }
                    //    }
                    //    if (countFoundCorrect < 2)
                    //        rebuildACQueryDef = true;
                    //}
                    //if (rebuildACQueryDef)
                    //{
                    //    navACQueryDefinition.ClearFilter(true);
                    //    string pickingType = System.Convert.ToString((short)GlobalApp.PickingType.ReceiptVehicle);
                    //    navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "PickingTypeIndex", Global.LogicalOperators.greaterThanOrEqual, Global.Operators.and, pickingType, true));
                    //    //string pickingState = System.Convert.ToString((short)GlobalApp.PickingState.New);
                    //    //navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "PickingStateIndex", Global.LogicalOperators.isNull, Global.Operators.and, pickingState, true));
                    //    navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "VisitorVoucherID", Global.LogicalOperators.isNull, Global.Operators.and, "", true));
                    //    navACQueryDefinition.SaveConfig(true);
                    //}
                    _AccessUnAssignedPicking = navACQueryDefinition.NewAccessNav<Picking>("UnAssignedPicking", this);
                    _AccessUnAssignedPicking.NavSearchExecuting += _Picking_NavSearchExecuting;
                    _AccessUnAssignedPicking.AutoSaveOnNavigation = false;
                    _AccessUnAssignedPicking.NavACQueryDefinition.CheckAndReplaceColumnsIfDifferent(AccessUnAssignedPicking_DefaultFilter, AccessUnAssignedPicking_DefaultSort);
                }
                return _AccessUnAssignedPicking;
            }
        }

        private IQueryable<Picking> _Picking_NavSearchExecuting(IQueryable<Picking> result)
        {
            IQueryable<Picking> query = result as IQueryable<Picking>;
            if (query != null)
            {
                query.Include(c => c.DeliveryCompanyAddress.Company).Include(c => c.MDPickingType);

                if (!string.IsNullOrEmpty(FilterPickingMaterialNo))
                {
                    result =
                        result
                        .Where(c =>
                            c.PickingPos_Picking
                            .Any(x =>
                                        (
                                            x.InOrderPos != null
                                            && (x.InOrderPos.Material.MaterialNo.Contains(FilterPickingMaterialNo) || x.InOrderPos.Material.MaterialName1.Contains(FilterPickingMaterialNo))
                                        )
                                        || (
                                            x.OutOrderPos != null
                                            && (x.OutOrderPos.Material.MaterialNo.Contains(FilterPickingMaterialNo) || x.OutOrderPos.Material.MaterialName1.Contains(FilterPickingMaterialNo))
                                        ) || (
                                            x.PickingMaterial != null
                                            && (x.PickingMaterial.MaterialNo.Contains(FilterPickingMaterialNo) || x.PickingMaterial.MaterialName1.Contains(FilterPickingMaterialNo))
                                        )

                            )
                        );
                }


            }
            return result;
        }

        protected virtual List<ACFilterItem> AccessUnAssignedPicking_DefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, nameof(Picking.VisitorVoucherID), Global.LogicalOperators.isNull, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, nameof(Picking.DeliveryDateFrom), Global.LogicalOperators.greaterThanOrEqual, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, nameof(Picking.DeliveryDateTo), Global.LogicalOperators.lessThan, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, nameof(Picking.KeyOfExtSys), Global.LogicalOperators.contains, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDPickingType\\MDPickingTypeIndex", Global.LogicalOperators.equal, Global.Operators.or, System.Convert.ToString((short)GlobalApp.PickingType.ReceiptVehicle), true),
                    new ACFilterItem(Global.FilterTypes.filter, "MDPickingType\\MDPickingTypeIndex", Global.LogicalOperators.equal, Global.Operators.or, System.Convert.ToString((short)GlobalApp.PickingType.IssueVehicle), true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true)
                };
            }
        }

        protected virtual List<ACSortItem> AccessUnAssignedPicking_DefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("PickingNo", Global.SortDirections.descending, true),
                };
            }
        }

        /// <summary>
        /// Gets or sets the current picking in the unassigned picking list.
        /// </summary>
        [ACPropertyCurrent(634, "UnAssignedPicking", "en{'Unassigned Picking'}de{'Nicht zugeordneter Kommissionierauftrag'}", Description = @"Current record of the unassigned picking list. This picking is assigned to the visitor voucher with AssignPicking().")]
        public Picking CurrentUnAssignedPicking
        {
            get
            {
                if (AccessUnAssignedPicking == null)
                    return null;
                return AccessUnAssignedPicking.Current;
            }
            set
            {
                if (AccessUnAssignedPicking == null)
                    return;
                AccessUnAssignedPicking.Current = value;
                OnPropertyChanged("CurrentUnAssignedPicking");
            }
        }


        /// <summary>
        /// Gets the list of pickings that are not yet assigned to a visitor voucher.
        /// </summary>
        [ACPropertyList(635, "UnAssignedPicking", "en{'Unassigned Pickings'}de{'Nicht zugeordnete Kommissionieraufträge'}", Description = @"List of pickings that are not yet assigned to a visitor voucher (result of the query PickingUnAssigned, minus the already assigned pickings of the current voucher).")]
        public IEnumerable<Picking> UnAssignedPickingList
        {
            get
            {
                if (AccessUnAssignedPicking == null)
                    return null;
                if (CurrentVisitorVoucher != null && CurrentVisitorVoucher.Picking_VisitorVoucher.Count > 0)
                {
                    return AccessUnAssignedPicking.NavList.Except(CurrentVisitorVoucher.Picking_VisitorVoucher);
                }
                return AccessUnAssignedPicking.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected picking in the unassigned picking list. Setting it also sets CurrentUnAssignedPicking.
        /// </summary>
        [ACPropertySelected(636, "UnAssignedPicking", "en{'Unassigned Picking'}de{'Nicht zugeordneter Kommissionierauftrag'}", Description = @"Selected picking in the unassigned picking list. Setting it also sets CurrentUnAssignedPicking.")]
        public Picking SelectedUnAssignedPicking
        {
            get
            {
                if (AccessUnAssignedPicking == null)
                    return null;
                return AccessUnAssignedPicking.Selected;
            }
            set
            {
                if (AccessUnAssignedPicking == null)
                    return;
                AccessUnAssignedPicking.Selected = value;
                OnPropertyChanged("SelectedUnAssignedPicking");
                CurrentUnAssignedPicking = value;
            }
        }
        #endregion

        #endregion

        #region BSO->ACMethod

        #region Un-/Assign Picking
        /// <summary>
        /// Assigns the selected unassigned picking to the current visitor voucher.
        /// </summary>
        [ACMethodCommand("UnAssignedPicking", "en{'Assign'}de{'Zuordnen'}", 630, true, Global.ACKinds.MSMethodPrePost, Description = @"Assigns the picking selected in the unassigned picking list (CurrentUnAssignedPicking) to the current visitor voucher via VisitorVoucherManager.AssignPicking. Enabled only if a voucher with visitor and an unassigned picking are selected.")]
        public void AssignPicking()
        {
            if (!IsEnabledAssignPicking())
                return;
            if (!PreExecute("AssignPicking"))
                return;


            Msg result = null;
            try
            {
                result = VisitorVoucherManager.AssignPicking(CurrentVisitorVoucher, CurrentUnAssignedPicking, this.DatabaseApp, this.ACFacilityManager, this.InDeliveryNoteManager, this.OutDeliveryNoteManager);
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

                Messages.LogException("BSOVisitorVoucher", "AssignPicking", msg);
                return;
            }

            RefreshLists();

            PostExecute("AssignPicking");
        }

        public bool IsEnabledAssignPicking()
        {
            if ((CurrentUnAssignedPicking == null) || (CurrentVisitorVoucher == null))
                return false;
            if (CurrentUnAssignedPicking.VisitorVoucher != null)
                return false;
            //if (CurrentVisitorVoucher.Picking_VisitorVoucher.Count > 0)
            //return false;
            if (CurrentVisitorVoucher.Visitor == null)
                return false;
            return true;
        }

        /// <summary>
        /// Removes the selected assigned picking from the current visitor voucher.
        /// </summary>
        [ACMethodCommand("Picking", "en{'Remove'}de{'Entfernen'}", 631, true, Global.ACKinds.MSMethodPrePost, Description = @"Removes the picking selected in the assigned picking list (CurrentPicking) from the current visitor voucher via VisitorVoucherManager.UnassignPicking.")]
        public void UnassignPicking()
        {
            if (!IsEnabledUnassignPicking())
                return;
            if (!PreExecute("UnassignPicking"))
                return;

            UnassignPickingInternal();

            PostExecute("UnassignPicking");
        }

        protected Msg UnassignPickingInternal()
        {
            Msg result = null;
            try
            {
                result = VisitorVoucherManager.UnassignPicking(CurrentPicking, CurrentVisitorVoucher, this.DatabaseApp);
                if (result != null)
                {
                    Messages.MsgAsync(result);
                    return result;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOVisitorVoucher", "UnassignPickingInternal", msg);
                return result;
            }

            RefreshLists();
            return result;
        }

        public bool IsEnabledUnassignPicking()
        {
            if (CurrentVisitorVoucher == null || CurrentPicking == null)
                return false;
            if (CurrentVisitorVoucher.Visitor == null)
                return false;
            return true;
        }
        #endregion

        #region Refresh Lists

        /// <summary>
        /// Re-queries the unassigned picking list (e.g. after a filter change).
        /// </summary>
        [ACMethodInfo("", ConstApp.Search, 650, true, Description = @"Re-queries the unassigned picking list (AccessUnAssignedPicking.NavSearch) and refreshes the UnAssignedPickingList property. Called automatically when one of the FilterPicking* properties changes.")]
        public void RefreshUnAssignedPickingList(bool forceQueryFromDb = false)
        {
            if (AccessUnAssignedPicking == null)
                return;
            AccessUnAssignedPicking.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(UnAssignedPickingList));
        }

        public void CleanFilterPicking()
        {
            FilterPickingMaterialNo = null;
            FilterPickingDateFrom = null;
            FilterPickingDateTo = null;

        }

        public void LoadPickingRelatedData(IEnumerable<Picking> pickings)
        {
            if (pickings != null)
            {
                foreach (Picking picking in pickings)
                {
                    LoadPickingRelatedData(picking);
                }
            }
        }

        public void LoadPickingRelatedData(Picking picking)
        {
            picking.PreparationStatus = PickingManager.GetPickingPreparationStatus(DatabaseApp, picking);
            picking.PreparationStatusName = PickingManager.GetPickingPreparationStatusName(DatabaseApp, picking.PreparationStatus);
        }

        #endregion

        #region Navigation
        /// <summary>
        /// Opens the picking order dialog for the picking selected in the assigned list.
        /// </summary>
        [ACMethodInteraction("Picking", "en{'Show Picking Order'}de{'Kommissionierauftrag anzeigen'}", 632, false, "SelectedPicking", Description = @"Opens the picking order dialog (ShowDialogOrder) for the picking selected in the assigned picking list.")]
        public void NavigateToAPicking()
        {
            if (!IsEnabledNavigateToAPicking())
                return;
            NavigateToPicking(SelectedPicking);
        }

        public bool IsEnabledNavigateToAPicking()
        {
            return SelectedPicking != null;
        }

        /// <summary>
        /// Opens the picking order dialog for the picking selected in the unassigned list.
        /// </summary>
        [ACMethodInteraction("UnAssignedPicking", "en{'Show Picking Order'}de{'Kommissionierauftrag anzeigen'}", 633, false, "SelectedUnAssignedPicking", Description = @"Opens the picking order dialog (ShowDialogOrder) for the picking selected in the unassigned picking list.")]
        public void NavigateToUPicking()
        {
            if (!IsEnabledNavigateToUPicking())
                return;
            NavigateToPicking(SelectedUnAssignedPicking);
        }

        public bool IsEnabledNavigateToUPicking()
        {
            return SelectedUnAssignedPicking != null;
        }

        private void NavigateToPicking(Picking picking)
        {
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(
                new PAOrderInfoEntry()
                {
                    EntityID = picking.PickingID,
                    EntityName = Picking.ClassName
                });
                service.ShowDialogOrder(this, info);
            }
        }
        #endregion

        #endregion
    }
}
