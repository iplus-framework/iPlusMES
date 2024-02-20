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

        #region Tourplan assigned
        /// <summary>
        /// The _ current delivery note pos
        /// </summary>
        Tourplan _CurrentTourplan;
        /// <summary>
        /// Gets or sets the current delivery note pos.
        /// </summary>
        /// <value>The current delivery note pos.</value>
        [ACPropertyCurrent(640, Tourplan.ClassName)]
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
        /// Gets the delivery note pos list.
        /// </summary>
        /// <value>The delivery note pos list.</value>
        [ACPropertyList(641, Tourplan.ClassName)]
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
        /// Gets or sets the selected delivery note pos.
        /// </summary>
        /// <value>The selected delivery note pos.</value>
        [ACPropertySelected(642, Tourplan.ClassName)]
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
        [ACPropertyAccess(694, "UnAssignedTourplan")]
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

        [ACPropertyCurrent(643, "UnAssignedTourplan")]
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


        [ACPropertyList(644, "UnAssignedTourplan")]
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

        [ACPropertySelected(645, "UnAssignedTourplan")]
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
        [ACMethodCommand("UnAssignedTourplan", "en{'Assign'}de{'Zuordnen'}", 640, true, Global.ACKinds.MSMethodPrePost)]
        public void AssignTourplan()
        {
            if (!IsEnabledAssignTourplan())
                return;
            if (!PreExecute("AssignTourplan"))
                return;

            // Bei Zurodnung von Tourenplan, führe Check durch ob geplanter LKW bei Anmeldung. Falls nicht, dann MEldung mit Bestätigung, dass Fahrzeugwechsel
            if (CurrentVisitorVoucher.Visitor != null && CurrentVisitorVoucher.Visitor.VehicleFacility != CurrentUnAssignedTourplan.VehicleFacility)
            {
                if (Messages.Question(this, "Question00009", Global.MsgResult.Yes) == Global.MsgResult.No)
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
                    Messages.Msg(result);
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

        [ACMethodCommand(Tourplan.ClassName, "en{'Remove'}de{'Entfernen'}", 641, true, Global.ACKinds.MSMethodPrePost)]
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
                    Messages.Msg(result);
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

        #endregion
    }
}
