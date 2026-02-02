// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿// ***********************************************************************
// Assembly         : gip.bso.logistics
// Author           : APinter
// Created          : 07.08.2018
//
// ***********************************************************************
// <copyright file="BSOWeighing.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.mes.autocomponent;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.logistics.Logistics
{

    /// <summary>
    /// Wägung
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Weighing'}de{'Wägung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + "Weighing")]
    public class BSOWeighing : ACBSOvbNav
    {
        #region c´tors
        public BSOWeighing(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            Search();
            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            //CurrentWeighing.InOrderPos.DeliveryNotePos_InOrderPos.FirstOrDefault().DeliveryNote
            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;
           
            bool result = await base.ACDeInit(deleteACClassTask);

            if (_AccessPrimary != null)
            {
                await _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return result;
        }

        #endregion

        #region BSO->ACProperty

        #region Weighing
        public override IAccessNav AccessNav { get { return AccessPrimary; } }

        ACAccessNav<Weighing> _AccessPrimary;

        /// <summary>
        /// Gets the access primary
        /// </summary>
        /// <value>Access primary</value>
        [ACPropertyAccessPrimary(690, "Weighing")]
        public ACAccessNav<Weighing> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        navACQueryDefinition.CheckAndReplaceColumnsIfDifferent(NavigationqueryDefaultFilter, NavigationqueryDefaultSort);
                    }
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Weighing>(nameof(Weighing), this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessPrimary;
            }
        }

        protected virtual List<ACFilterItem> NavigationqueryDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, Weighing.NoColumnName, Global.LogicalOperators.contains, Global.Operators.or, null, true, true),
                };
            }
        }

        protected virtual List<ACSortItem> NavigationqueryDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem(Weighing.NoColumnName, Global.SortDirections.descending, true)
                };
            }
        }

        /// <summary>
        /// Gets or sets the selected weighing object.
        /// </summary>
        /// <value>The selected weighing object</value>
        [ACPropertySelected(600, "Weighing")]
        public Weighing SelectedWeighing
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.SelectedNavObject = value;
                OnPropertyChanged("SelectedWeighing");
                OnPropertyChanged("DeliveryNote");
            }
        }

        /// <summary>
        /// Gets or sets the selected weighing object
        /// </summary>
        /// <value>The current weighing object</value>
        [ACPropertyCurrent(601, "Weighing")]
        public Weighing CurrentWeighing
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null)
                    return;
                AccessPrimary.CurrentNavObject = value;
                OnPropertyChanged("CurrentWeighing");
                OnPropertyChanged("DeliveryNote");
            }
        }

        /// <summary>
        /// Gets the weighing list
        /// </summary>
        /// <value>The weighing list</value>
        [ACPropertyList(602, "Weighing")]
        public IEnumerable<Weighing> WeighingList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }

        private IQueryable<Weighing> _AccessPrimary_NavSearchExecuting(IQueryable<Weighing> result)
        {

            if (result != null)
            {
                result.Include(c => c.VisitorVoucher)
                    .Include("InOrderPos.DeliveryNotePos_InOrderPos.DeliveryNote")
                    .Include("OutOrderPos.DeliveryNotePos_OutOrderPos.DeliveryNote")
                    .Include("PickingPos.Picking")
                    .Include("LabOrderPos.LabOrder");
            }
            return result;        }

        #endregion

        #region DeliveryNote

        [ACPropertyInfo(603, DeliveryNote.ClassName, "en{'Delivery Note'}de{'Lieferschein'}")]
        public DeliveryNote DeliveryNote
        {
            get
            {
                if (SelectedWeighing != null)
                {
                    return SelectedWeighing.InOrderPos?.DeliveryNotePos_InOrderPos.FirstOrDefault().DeliveryNote;
                }
                return null;
            }
        }

        [ACPropertyInfo(604, DeliveryNote.ClassName, "en{'Delivery Note'}de{'Lieferschein'}")]
        public DeliveryNote DeliveryNote2
        {
            get
            {
                if (SelectedWeighing != null)
                {
                    return SelectedWeighing.OutOrderPos?.DeliveryNotePos_OutOrderPos.FirstOrDefault().DeliveryNote;
                }
                return null;
            }
        }
        #endregion


        #endregion

        #region BSO->ACMethod

        [ACMethodCommand("Weighing", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary != null)
                AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("WeighingList");
            OnPropertyChanged("DeliveryNote");
            OnPropertyChanged("DeliveryNot2");
        }

        /// <summary>
        /// Loads this instance
        /// </summary>
        [ACMethodInteraction("Weighing", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedWeighing", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<Weighing>(requery, () => SelectedWeighing, () => CurrentWeighing, c => CurrentWeighing = c,
                DatabaseApp.Weighing
                .Include("InOrderPos.DeliveryNotePos_InOrderPos.DeliveryNote")
                .Where(c => c.WeighingID == SelectedWeighing.WeighingID));
            PostExecute("Weighing");
        }

        public bool IsEnabledLoad()
        {
            return SelectedWeighing != null;
        }
        #endregion

        #region ControlMode
        /// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        /// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        /// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        /// <returns>ControlModesInfo</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            //if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            //return Global.ControlModes.Disabled;
        }
        #endregion


    }
}
