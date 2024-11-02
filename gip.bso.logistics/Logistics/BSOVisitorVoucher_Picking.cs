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
    public partial class BSOVisitorVoucher : ACBSOvbNav
    {
        #region Properties

        #region Picking Filter
        #region Picking -> Filter -> Properties -> Date

        [ACPropertyInfo(301, nameof(FilterPickingDateFrom), "en{'From'}de{'Von'}")]
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

        [ACPropertyInfo(302, nameof(FilterPickingDateTo), "en{'to'}de{'bis'}")]
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
        [ACPropertyInfo(304, nameof(FilterPickingMaterialNo), ConstApp.Material)]
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

        [ACPropertyInfo(305, nameof(FilterPickingKeyOfExternalSys), ConstApp.EntityTranslateKeyOfExtSys)]
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
        /// Gets or sets the current delivery note pos.
        /// </summary>
        /// <value>The current delivery note pos.</value>
        [ACPropertyCurrent(630, "Picking")]
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
        /// Gets the delivery note pos list.
        /// </summary>
        /// <value>The delivery note pos list.</value>
        [ACPropertyList(631, "Picking")]
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
        /// Gets or sets the selected delivery note pos.
        /// </summary>
        /// <value>The selected delivery note pos.</value>
        [ACPropertySelected(632, "Picking")]
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
        [ACPropertyAccess(693, "UnAssignedPicking")]
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

        [ACPropertyCurrent(634, "UnAssignedPicking")]
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


        [ACPropertyList(635, "UnAssignedPicking")]
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

        [ACPropertySelected(636, "UnAssignedPicking")]
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
        [ACMethodCommand("UnAssignedPicking", "en{'Assign'}de{'Zuordnen'}", 630, true, Global.ACKinds.MSMethodPrePost)]
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
                    Messages.Msg(result);
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

        [ACMethodCommand("Picking", "en{'Remove'}de{'Entfernen'}", 631, true, Global.ACKinds.MSMethodPrePost)]
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
                    Messages.Msg(result);
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

        [ACMethodInfo("", ConstApp.Search, 650, true)]
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
        [ACMethodInteraction("Picking", "en{'Show Picking Order'}de{'Kommissionierauftrag anzeigen'}", 632, false, "SelectedPicking")]
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

        [ACMethodInteraction("UnAssignedPicking", "en{'Show Picking Order'}de{'Kommissionierauftrag anzeigen'}", 633, false, "SelectedUnAssignedPicking")]
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
