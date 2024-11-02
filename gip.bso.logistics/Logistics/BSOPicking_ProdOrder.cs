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
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;
using gip.bso.sales;
using gip.bso.purchasing;
using gip.mes.facility;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.logistics
{
    public partial class BSOPicking : ACBSOvbNav
    {
        #region Properties

        #region ProdOrderPartslistPos
        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<ProdOrderPartslistPos> _AccessProdOrderPartslistPos;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(696, ProdOrderPartslistPos.ClassName)]
        public ACAccessNav<ProdOrderPartslistPos> AccessProdOrderPartslistPos
        {
            get
            {
                if (_AccessProdOrderPartslistPos == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "ProdOrderPartslistPosOpen", ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceColumnsIfDifferent(ProdOrderPartslistPosDefaultFilter, ProdOrderPartslistPosDefaultSort);
                    }

                    _AccessProdOrderPartslistPos = navACQueryDefinition.NewAccessNav<ProdOrderPartslistPos>(ProdOrderPartslistPos.ClassName, this);
                    _AccessProdOrderPartslistPos.AutoSaveOnNavigation = false;
                    _AccessProdOrderPartslistPos.NavSearchExecuting += _AccessProdOrderPartslistPos_NavSearchExecuting;
                }
                return _AccessProdOrderPartslistPos;
            }
        }

        private IQueryable<ProdOrderPartslistPos> _AccessProdOrderPartslistPos_NavSearchExecuting(IQueryable<ProdOrderPartslistPos> result)
        {
            result = (result as IQueryable<ProdOrderPartslistPos>)
                            .Include(c => c.ProdOrderPartslist)
                            .Include(c => c.ProdOrderPartslist.ProdOrder)
                            .Include(c => c.ProdOrderPartslist.Partslist)
                            .Include(c => c.Material)
                            .Include(c => c.Material.BaseMDUnit)
                            .Include(c => c.MDUnit)
                            .Where(c => !c.PickingPosProdOrderPartslistPos_ProdorderPartslistPos.Any());
            return result;
        }

        private List<ACFilterItem> ProdOrderPartslistPosDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "ProdOrderPartslist.MDProdOrderState.MDProdOrderStateIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)MDProdOrderState.ProdOrderStates.NewCreated).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.filter, "ProdOrderPartslist.ProdOrder.MDProdOrderState.MDProdOrderStateIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)MDProdOrderState.ProdOrderStates.NewCreated).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.filter, "ProdOrderPartslistPos1_ParentProdOrderPartslistPos", Global.LogicalOperators.isNull, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, "MaterialPosTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short)GlobalApp.MaterialPosTypes.OutwardRoot).ToString(), true)
                };
            }
        }

        private List<ACSortItem> ProdOrderPartslistPosDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("ProdOrderPartslist.ProdOrder.ProgramNo", Global.SortDirections.ascending, true),
                    new ACSortItem("ProdOrderPartslist.Sequence", Global.SortDirections.ascending, true),
                    new ACSortItem("Sequence", Global.SortDirections.ascending, true),
                };
            }
        }

        /// <summary>
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(660, ProdOrderPartslistPos.ClassName)]
        public ProdOrderPartslistPos CurrentProdOrderPartslistPos
        {
            get
            {
                if (AccessProdOrderPartslistPos == null)
                    return null;
                return AccessProdOrderPartslistPos.Current;
            }
            set
            {
                if (AccessProdOrderPartslistPos == null)
                    return;
                AccessProdOrderPartslistPos.Current = value;
                OnPropertyChanged(nameof(CurrentProdOrderPartslistPos));
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(661, ProdOrderPartslistPos.ClassName)]
        public IEnumerable<ProdOrderPartslistPos> ProdOrderPartslistPosList
        {
            get
            {
                if (AccessProdOrderPartslistPos == null)
                    return null;
                if (CurrentPicking != null)
                {
                    IEnumerable<ProdOrderPartslistPos> addedPositions = CurrentPicking.PickingPos_Picking.Where(c => c.EntityState == EntityState.Added
                        && c.PickingPosProdOrderPartslistPos_PickingPos.Any()
                        ).SelectMany(c => c.PickingPosProdOrderPartslistPos_PickingPos).Select(c=>c.ProdorderPartslistPos);
                        //&& c.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos != null
                        //).Select(c => c.ProdOrderPartslistPos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos);
                    if (addedPositions.Any())
                    {
                        if (_UnSavedUnAssignedProdOrderPartslistPos.Any())
                            return AccessProdOrderPartslistPos.NavList.Except(addedPositions).Union(_UnSavedUnAssignedProdOrderPartslistPos);
                        else
                            return AccessProdOrderPartslistPos.NavList.Except(addedPositions);
                    }
                }
                if (_UnSavedUnAssignedProdOrderPartslistPos.Any())
                {
                    return AccessProdOrderPartslistPos.NavList.Union(_UnSavedUnAssignedProdOrderPartslistPos);
                }
                return AccessProdOrderPartslistPos.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(662, ProdOrderPartslistPos.ClassName)]
        public ProdOrderPartslistPos SelectedProdOrderPartslistPos
        {
            get
            {
                if (AccessProdOrderPartslistPos == null)
                    return null;
                return AccessProdOrderPartslistPos.Selected;
            }
            set
            {
                if (AccessProdOrderPartslistPos == null)
                    return;
                AccessProdOrderPartslistPos.Selected = value;
                OnPropertyChanged(nameof(SelectedProdOrderPartslistPos));
                CurrentProdOrderPartslistPos = value;
            }
        }
        #endregion

        #endregion

        #region Methods

        #region ProdOrderPartslistPos
        /// <summary>
        /// Assigns the in order pos.
        /// </summary>
        [ACMethodCommand("xxx", "en{'Assign'}de{'Zuordnen'}", 660, true, Global.ACKinds.MSMethodPrePost)]
        public void AssignProdOrderPartslistPos()
        {
            if (!PreExecute("AssignProdOrderPartslistPos"))
                return;

            try
            {
                List<object> resultNewEntities = new List<object>();
                Msg result = PickingManager.AssignProdOrderPartslistPos(CurrentPicking, CurrentProdOrderPartslistPos, PartialQuantity, this.DatabaseApp, resultNewEntities);
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

                Messages.LogException("BSOPicking", "AssignProdOrderPartslistPos", msg);
            }

            if (_UnSavedUnAssignedProdOrderPartslistPos.Contains(CurrentProdOrderPartslistPos))
                _UnSavedUnAssignedProdOrderPartslistPos.Remove(CurrentProdOrderPartslistPos);
            OnPropertyChanged(nameof(PickingPosList));
            RefreshProdOrderPartslistPosList();
            PartialQuantity = null;
        }

        /// <summary>
        /// Determines whether [is enabled assign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignProdOrderPartslistPos()
        {
            if (CurrentProdOrderPartslistPos == null)
                return false;
            return true;
        }

        [ACMethodInteraction("xxx", "en{'Filter'}de{'Filter'}", 661, false)]
        public bool FilterDialogProdOrderPartslistPos()
        {
            if (AccessProdOrderPartslistPos == null)
                return false;
            bool result = AccessProdOrderPartslistPos.ShowACQueryDialog();
            if (result)
            {
                RefreshProdOrderPartslistPosList();
            }
            return result;
        }

        public virtual void RefreshProdOrderPartslistPosList(bool forceQueryFromDb = false)
        {
            if (AccessProdOrderPartslistPos == null)
                return;
            if (_ActivateProdOpen || forceQueryFromDb)
                AccessProdOrderPartslistPos.NavSearch(DatabaseApp);
            OnPropertyChanged(nameof(ProdOrderPartslistPosList));
        }
        #endregion

        #endregion
    }
}
