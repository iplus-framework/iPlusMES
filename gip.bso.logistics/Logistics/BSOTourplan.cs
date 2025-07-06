// ***********************************************************************
// Assembly         : gip.bso.logistics
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 26.12.2018
// ***********************************************************************
// <copyright file="BSOTourplan.cs" company="gip mbh, Oftersheim, Germany">
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
using gip.bso.sales;
using gip.bso.purchasing;
using gip.mes.facility;

namespace gip.bso.logistics
{
    /// <summary>
    /// Class BSOTourplan
    /// </summary>
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Vehicle Route'}de{'Tour'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Tourplan.ClassName)]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "OutOrderPosOpen", "en{'Open Sales Order Item'}de{'Offene Auftragsposition'}", typeof(OutOrderPos), OutOrderPos.ClassName, "MDDelivPosState\\MDDelivPosStateIndex", "TargetDeliveryDate,Material\\MaterialNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "InOrderPosOpen", "en{'Open Purchase Order Item'}de{'Offene Bestellposition'}", typeof(InOrderPos), InOrderPos.ClassName, "MDDelivPosState\\MDDelivPosStateIndex", "TargetDeliveryDate,Material\\MaterialNo")]
    public class BSOTourplan : ACBSOvbNav
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="BSOTourplan"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOTourplan(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
        }

        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            _OutDeliveryNoteManager = ACOutDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_OutDeliveryNoteManager == null)
                throw new Exception("OutDeliveryNoteManager not configured");

            _InDeliveryNoteManager = ACInDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_InDeliveryNoteManager == null)
                throw new Exception("InDeliveryNoteManager not configured");

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACOutDeliveryNoteManager.DetachACRefFromServiceInstance(this, _OutDeliveryNoteManager);
            _OutDeliveryNoteManager = null;
            ACInDeliveryNoteManager.DetachACRefFromServiceInstance(this, _InDeliveryNoteManager);
            _InDeliveryNoteManager = null;
            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            this._AccessInOrderPos = null;
            this._AccessOutOrderPos = null;
            this._CurrentInOrderTourPos = null;
            this._CurrentOutOrderTourPos = null;
            this._SelectedInOrderTourPos = null;
            this._SelectedOutOrderTourPos = null;
            this._AddedUnsavedDeliveryNotes = null;
            this._BingMaps = null;
            this._StateCompletelyAssigned = null;
            this._UnSavedAssignedPickingInOrderPos = null;
            this._UnSavedAssignedPickingOutOrderPos = null;
            this._UnSavedUnAssignedInOrderPos = null;
            this._UnSavedUnAssignedOutOrderPos = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessInOrderPos != null)
            {
                _AccessInOrderPos.ACDeInit(false);
                _AccessInOrderPos = null;
            }
            if (_AccessOutOrderPos != null)
            {
                _AccessOutOrderPos.ACDeInit(false);
                _AccessOutOrderPos = null;
            }
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region BSO->ACProperty

        #region local
        protected ACRef<ACOutDeliveryNoteManager> _OutDeliveryNoteManager = null;
        protected ACOutDeliveryNoteManager OutDeliveryNoteManager
        {
            get
            {
                if (_OutDeliveryNoteManager == null)
                    return null;
                return _OutDeliveryNoteManager.ValueT;
            }
        }

        protected ACRef<ACInDeliveryNoteManager> _InDeliveryNoteManager = null;
        protected ACInDeliveryNoteManager InDeliveryNoteManager
        {
            get
            {
                if (_InDeliveryNoteManager == null)
                    return null;
                return _InDeliveryNoteManager.ValueT;
            }
        }

        protected ACRef<ACComponent> _ACFacilityManager = null;
        protected ACComponent ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT;
            }
        }
        #endregion

        #region Tourplan
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<Tourplan> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, Tourplan.ClassName)]
        public ACAccessNav<Tourplan> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    if (navACQueryDefinition != null)
                    {
                        ACSortItem sortItem = navACQueryDefinition.ACSortColumns.Where(c => c.ACIdentifier == "TourplanNo").FirstOrDefault();
                        if (sortItem != null && sortItem.IsConfiguration)
                        {
                            sortItem.SortDirection = Global.SortDirections.descending;
                        }
                    }

                    _AccessPrimary = navACQueryDefinition.NewAccessNav<Tourplan>(Tourplan.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the current tourplan.
        /// </summary>
        /// <value>The current tourplan.</value>
        [ACPropertyCurrent(601, Tourplan.ClassName)]
        public Tourplan CurrentTourplan
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
                AccessPrimary.Current = value;
                OnPropertyChanged("CurrentTourplan");
                RefreshOutOrderPosList();
                OnPropertyChanged("OutOrderTourPosList");
                RefreshInOrderPosList();
                OnPropertyChanged("InOrderTourPosList");
            }
        }

        /// <summary>
        /// Gets the tourplan list.
        /// </summary>
        /// <value>The tourplan list.</value>
        [ACPropertyList(602, Tourplan.ClassName)]
        public IEnumerable<Tourplan> TourplanList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected tourplan.
        /// </summary>
        /// <value>The selected tourplan.</value>
        [ACPropertySelected(603, Tourplan.ClassName)]
        public Tourplan SelectedTourplan
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
                AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedTourplan");
            }
        }
        #endregion

        #region InOrderTour
        /// <summary>
        /// The _ current in order tour pos
        /// </summary>
        InOrderPos _CurrentInOrderTourPos;
        /// <summary>
        /// Gets or sets the current in order tour pos.
        /// </summary>
        /// <value>The current in order tour pos.</value>
        [ACPropertyCurrent(604, "InOrderTourPos")]
        public InOrderPos CurrentInOrderTourPos
        {
            get
            {
                return _CurrentInOrderTourPos;
            }
            set
            {
                _CurrentInOrderTourPos = value;
                OnPropertyChanged("CurrentInOrderTourPos");
            }
        }

        /// <summary>
        /// The _ added unsaved delivery notes
        /// </summary>
        private List<DeliveryNote> _AddedUnsavedDeliveryNotes = new List<DeliveryNote>();
        /// <summary>
        /// Gets the in order tour pos list.
        /// </summary>
        /// <value>The in order tour pos list.</value>
        [ACPropertyList(605, "InOrderTourPos")]
        public IEnumerable<InOrderPos> InOrderTourPosList
        {
            get
            {
                if (CurrentTourplan == null || AssignedInDeliveryNotePosList == null)
                    return null;
                return AssignedInDeliveryNotePosList.Select(c => c.InOrderPos);
            }
        }

        /// <summary>
        /// The _ selected in order tour pos
        /// </summary>
        InOrderPos _SelectedInOrderTourPos;
        /// <summary>
        /// Gets or sets the selected in order tour pos.
        /// </summary>
        /// <value>The selected in order tour pos.</value>
        [ACPropertySelected(606, "InOrderTourPos")]
        public InOrderPos SelectedInOrderTourPos
        {
            get
            {
                return _SelectedInOrderTourPos;
            }
            set
            {
                _SelectedInOrderTourPos = value;
                OnPropertyChanged("SelectedInOrderTourPos");
            }
        }

        public IEnumerable<DeliveryNotePos> AssignedInDeliveryNotePosList
        {
            get
            {
                if (CurrentTourplan == null)
                    return null;
                if (CurrentTourplan == null)
                    return null;
                List<DeliveryNotePos> tourPosList = new List<DeliveryNotePos>();
                foreach (DeliveryNote deliveryNote in AssignedDeliveryNotes)
                {
                    foreach (DeliveryNotePos deliveryNotePos in deliveryNote.DeliveryNotePos_DeliveryNote)
                    {
                        if (deliveryNotePos.InOrderPos != null)
                            tourPosList.Add(deliveryNotePos);
                    }
                }
                return tourPosList;
            }
        }
        #endregion

        #region OutOrderTour
        /// <summary>
        /// The _ current out order tour pos
        /// </summary>
        OutOrderPos _CurrentOutOrderTourPos;
        /// <summary>
        /// Gets or sets the current out order tour pos.
        /// </summary>
        /// <value>The current out order tour pos.</value>
        [ACPropertyCurrent(607, "OutOrderTourPos")]
        public OutOrderPos CurrentOutOrderTourPos
        {
            get
            {
                return _CurrentOutOrderTourPos;
            }
            set
            {
                _CurrentOutOrderTourPos = value;
                OnPropertyChanged("CurrentOutOrderTourPos");
            }
        }

        /// <summary>
        /// Gets the out order tour pos list.
        /// </summary>
        /// <value>The out order tour pos list.</value>
        [ACPropertyList(608, "OutOrderTourPos")]
        public IEnumerable<OutOrderPos> OutOrderTourPosList
        {
            get
            {
                if (CurrentTourplan == null || AssignedOutDeliveryNotePosList == null)
                    return null;
                return AssignedOutDeliveryNotePosList.Select(c => c.OutOrderPos);
            }
        }

        /// <summary>
        /// The _ selected out order tour pos
        /// </summary>
        OutOrderPos _SelectedOutOrderTourPos;
        /// <summary>
        /// Gets or sets the selected out order tour pos.
        /// </summary>
        /// <value>The selected out order tour pos.</value>
        [ACPropertySelected(609, "OutOrderTourPos")]
        public OutOrderPos SelectedOutOrderTourPos
        {
            get
            {
                return _SelectedOutOrderTourPos;
            }
            set
            {
                _SelectedOutOrderTourPos = value;
                OnPropertyChanged("SelectedOutOrderTourPos");
            }
        }

        public IEnumerable<DeliveryNotePos> AssignedOutDeliveryNotePosList
        {
            get
            {
                if (CurrentTourplan == null)
                    return null;
                if (CurrentTourplan == null)
                    return null;
                List<DeliveryNotePos> tourPosList = new List<DeliveryNotePos>();
                foreach (DeliveryNote deliveryNote in AssignedDeliveryNotes)
                {
                    foreach (DeliveryNotePos deliveryNotePos in deliveryNote.DeliveryNotePos_DeliveryNote)
                    {
                        if (deliveryNotePos.OutOrderPos != null)
                            tourPosList.Add(deliveryNotePos);
                    }
                }
                return tourPosList;
            }
        }

        #endregion


        #region InOrder
        private List<InOrderPos> _UnSavedUnAssignedInOrderPos = new List<InOrderPos>();
        private List<InOrderPos> _UnSavedAssignedPickingInOrderPos = new List<InOrderPos>();


        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<InOrderPos> _AccessInOrderPos;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(691, InOrderPos.ClassName)]
        public ACAccessNav<InOrderPos> AccessInOrderPos
        {
            get
            {
                if (_AccessInOrderPos == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "InOrderPosOpen", ACType.ACIdentifier);
                    bool rebuildACQueryDef = false;
                    short assigned = (short)MDDelivPosState.DelivPosStates.CompletelyAssigned;
                    if (!navACQueryDefinition.ACFilterColumns.Any())
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
        [ACPropertyCurrent(610, InOrderPos.ClassName)]
        public InOrderPos CurrentInOrderPos
        {
            get
            {
                if (AccessInOrderPos == null)
                    return null;
                return AccessInOrderPos.Current;
            }
            set
            {
                if (AccessInOrderPos == null)
                    return;
                AccessInOrderPos.Current = value;
                OnPropertyChanged("CurrentInOrderPos");
            }
        }

        MDDelivPosState _StateCompletelyAssigned = null;
        MDDelivPosState StateCompletelyAssigned
        {
            get
            {
                if (_StateCompletelyAssigned != null)
                    return _StateCompletelyAssigned;
                var queryDelivStateAssigned = DatabaseApp.MDDelivPosState.Where(c => c.MDDelivPosStateIndex == (Int16)MDDelivPosState.DelivPosStates.CompletelyAssigned);
                _StateCompletelyAssigned = queryDelivStateAssigned.FirstOrDefault();
                return _StateCompletelyAssigned;
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(611, InOrderPos.ClassName)]
        public IEnumerable<InOrderPos> InOrderPosList
        {
            get
            {
                if (AccessInOrderPos == null)
                    return null;
                if (CurrentTourplan != null)
                {
                    List<InOrderPos> addedPositions = new List<InOrderPos>();
                    foreach (TourplanPos tourPos in CurrentTourplan.TourplanPos_Tourplan.Where(c => c.EntityState == System.Data.EntityState.Added))
                    {
                        foreach (DeliveryNote addedDeliveryNote in tourPos.DeliveryNote_TourplanPos)
                        {
                            addedPositions.AddRange(addedDeliveryNote.DeliveryNotePos_DeliveryNote.Where(c => c.EntityState == System.Data.EntityState.Added
                                && c.InOrderPos != null
                                && c.InOrderPos.InOrderPos1_ParentInOrderPos != null
                                && c.InOrderPos.InOrderPos1_ParentInOrderPos.MDDelivPosState == StateCompletelyAssigned
                                ).Select(c => c.InOrderPos.InOrderPos1_ParentInOrderPos));
                        }
                    }

                    if (addedPositions.Any())
                    {
                        if (_UnSavedUnAssignedInOrderPos.Any())
                            return AccessInOrderPos.NavList.Except(addedPositions).Union(_UnSavedUnAssignedInOrderPos);
                        else
                            return AccessInOrderPos.NavList.Except(addedPositions);
                    }
                    else if (_UnSavedUnAssignedInOrderPos.Any())
                    {
                        return AccessInOrderPos.NavList.Union(_UnSavedUnAssignedInOrderPos);
                    }
                }
                return AccessInOrderPos.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(612, InOrderPos.ClassName)]
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
                OnPropertyChanged("SelectedInOrderPos");
                CurrentInOrderPos = value;
            }
        }

        #endregion

        #region OutOrder
        private List<OutOrderPos> _UnSavedUnAssignedOutOrderPos = new List<OutOrderPos>();
        private List<OutOrderPos> _UnSavedAssignedPickingOutOrderPos = new List<OutOrderPos>();

        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<OutOrderPos> _AccessOutOrderPos;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(692, OutOrderPos.ClassName)]
        public ACAccessNav<OutOrderPos> AccessOutOrderPos
        {
            get
            {
                if (_AccessOutOrderPos == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "OutOrderPosOpen", ACType.ACIdentifier);
                    bool rebuildACQueryDef = false;
                    short assigned = (short)MDDelivPosState.DelivPosStates.CompletelyAssigned;
                    if (!navACQueryDefinition.ACFilterColumns.Any())
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
        /// Its invoked from a WPF-Itemscontrol that wants to refresh its CollectionView because the user has changed the LINQ-Expressiontree in the ACQueryDefinition-Property of IAccess. 
        /// The BSO should execute the query on the database first, to get the new results for refreshing the CollectionView of the control.
        /// If the bso don't want to handle this request or manipulate the ACQueryDefinition it returns false. The WPF-control invokes then the IAccess.NavSearch()-Method itself.  
        /// </summary>
        /// <param name="acAccess">Reference to IAccess that contains the changed query (Property NavACQueryDefinition)</param>
        /// <returns>True if the bso has handled this request and queried the database context. Otherwise it returns false.</returns>
        public override bool ExecuteNavSearch(IAccess acAccess)
        {
            if (acAccess == _AccessInOrderPos)
            {
                _AccessInOrderPos.NavSearch(this.DatabaseApp);
                OnPropertyChanged("InOrderPosList");
                return true;
            }
            else if (acAccess == _AccessOutOrderPos)
            {
                _AccessOutOrderPos.NavSearch(this.DatabaseApp);
                OnPropertyChanged("OutOrderPosList");
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }


        /// <summary>
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(613, OutOrderPos.ClassName)]
        public OutOrderPos CurrentOutOrderPos
        {
            get
            {
                if (AccessOutOrderPos == null)
                    return null;
                return AccessOutOrderPos.Current;
            }
            set
            {
                if (AccessOutOrderPos == null)
                    return;
                AccessOutOrderPos.Current = value;
                OnPropertyChanged("CurrentOutOrderPos");
            }
        }


        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(614, OutOrderPos.ClassName)]
        public IEnumerable<OutOrderPos> OutOrderPosList
        {
            get
            {
                if (AccessOutOrderPos == null)
                    return null;
                if (CurrentTourplan != null)
                {
                    List<OutOrderPos> addedPositions = new List<OutOrderPos>();
                    foreach (TourplanPos tourPos in CurrentTourplan.TourplanPos_Tourplan.Where(c => c.EntityState == System.Data.EntityState.Added))
                    {
                        foreach (DeliveryNote addedDeliveryNote in tourPos.DeliveryNote_TourplanPos)
                        {
                            addedPositions.AddRange(addedDeliveryNote.DeliveryNotePos_DeliveryNote.Where(c => c.EntityState == System.Data.EntityState.Added
                                && c.OutOrderPos != null
                                && c.OutOrderPos.OutOrderPos1_ParentOutOrderPos != null
                                && c.OutOrderPos.OutOrderPos1_ParentOutOrderPos.MDDelivPosState == StateCompletelyAssigned
                                ).Select(c => c.OutOrderPos.OutOrderPos1_ParentOutOrderPos));
                        }
                    }

                    if (addedPositions.Any())
                    {
                        if (_UnSavedUnAssignedOutOrderPos.Any())
                            return AccessOutOrderPos.NavList.Except(addedPositions).Union(_UnSavedUnAssignedOutOrderPos);
                        else
                            return AccessOutOrderPos.NavList.Except(addedPositions);
                    }
                    else if (_UnSavedUnAssignedOutOrderPos.Any())
                    {
                        return AccessOutOrderPos.NavList.Union(_UnSavedUnAssignedOutOrderPos);
                    }
                }
                return AccessOutOrderPos.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(615, OutOrderPos.ClassName)]
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

        #region Unassigned DeliveryNotes
        // TODO: Lieferscheine die in BSOOutDelivNote oder BSOInDeliveryNote angelegt worden sind,
        // Nun aber eine Tour zugeorndet werden sollen
        #endregion

        #region Assigned DeliveryNotes
        public List<DeliveryNote> AssignedDeliveryNotes
        {
            get
            {
                if (CurrentTourplan == null)
                    return new List<DeliveryNote>();
                List<DeliveryNote> deliveryNotes = _AddedUnsavedDeliveryNotes.ToList();
                var queryDeliveryNotes = DatabaseApp.DeliveryNote.Where(c => c.TourplanPos.TourplanID == CurrentTourplan.TourplanID);
                if (queryDeliveryNotes.Any())
                    deliveryNotes.AddRange(queryDeliveryNotes);
                return deliveryNotes;
            }
        }
        #endregion

        #region Map
        /// <summary>
        /// The _ bing maps
        /// </summary>
        BSOTourBingMaps _BingMaps;
        /// <summary>
        /// Gets the bing maps.
        /// </summary>
        /// <value>The bing maps.</value>
        [ACPropertyInfo(616)]
        public BSOTourBingMaps BingMaps
        {
            get
            {
                if (_BingMaps == null)
                {
                    _BingMaps = StartComponent("BSOBingMaps", null, null) as BSOTourBingMaps;
                }
                return _BingMaps;
            }
        }
        #endregion

        #endregion

        #region BSO->ACMethod
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Tourplan.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        protected override void OnPostSave()
        {
            _AddedUnsavedDeliveryNotes = new List<DeliveryNote>();
            _UnSavedUnAssignedInOrderPos = new List<InOrderPos>();
            _UnSavedAssignedPickingInOrderPos = new List<InOrderPos>();
            _UnSavedUnAssignedOutOrderPos = new List<OutOrderPos>();
            _UnSavedAssignedPickingOutOrderPos = new List<OutOrderPos>();
            RefreshInOrderPosList(true);
            RefreshOutOrderPosList(true);
            base.OnPostSave();
        }


        /// <summary>
        /// Determines whether [is enabled save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand(Tourplan.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        protected override void OnPostUndoSave()
        {
            _AddedUnsavedDeliveryNotes = new List<DeliveryNote>();
            _UnSavedUnAssignedInOrderPos = new List<InOrderPos>();
            _UnSavedAssignedPickingInOrderPos = new List<InOrderPos>();
            _UnSavedUnAssignedOutOrderPos = new List<OutOrderPos>();
            _UnSavedAssignedPickingOutOrderPos = new List<OutOrderPos>();
            RefreshInOrderPosList(true);
            RefreshOutOrderPosList(true);
            base.OnPostUndoSave();
        }

        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        [ACMethodInteraction(Tourplan.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedTourplan", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<Tourplan>(requery, () => SelectedTourplan, () => CurrentTourplan, c => CurrentTourplan = c,
                        DatabaseApp.Tourplan
                        .Include(c => c.TourplanPos_Tourplan)
                        .Where(c => c.TourplanID == SelectedTourplan.TourplanID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedTourplan != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(Tourplan.ClassName, Const.New, (short)MISort.New, true, "SelectedTourplan", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(Tourplan), Tourplan.NoColumnName, Tourplan.FormatNewNo, this);
            CurrentTourplan = Tourplan.NewACObject(DatabaseApp, null, secondaryKey);
            //Database.Tourplan.AddObject(CurrentTourplan);
            DatabaseApp.Tourplan.AddObject(CurrentTourplan);
            ACState = Const.SMNew;
            PostExecute("New");

        }

        /// <summary>
        /// Determines whether [is enabled new].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNew()
        {
            return true;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(Tourplan.ClassName, Const.Delete, (short)MISort.Delete, true, "CurrentTourplan", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (AccessPrimary == null)
                return;
            Msg msg = CurrentTourplan.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            AccessPrimary.NavList.Remove(CurrentTourplan);
            SelectedTourplan = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentTourplan != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Tourplan.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null)
                return;
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("TourplanList");
            RefreshInOrderPosList();
            RefreshOutOrderPosList();
        }
        #endregion

        #region InOrder-Assignment
        /// <summary>
        /// Assigns the in order pos.
        /// </summary>
        [ACMethodInteraction(InOrderPos.ClassName, "en{'>'}de{'>'}", 600, true, "SelectedInOrderPos")]
        public void AssignInOrderPos()
        {
            if (!IsEnabledAssignInOrderPos())
                return;

            try
            {
                DeliveryNote deliveryNote = AssignedDeliveryNotes.Where(c => c.DeliveryCompanyAddressID == SelectedInOrderPos.InOrder.DeliveryCompanyAddressID).FirstOrDefault();
                if (deliveryNote == null)
                {
                    string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(DeliveryNote), DeliveryNote.NoColumnName, DeliveryNote.FormatNewNo, this);
                    deliveryNote = DeliveryNote.NewACObject(DatabaseApp, null, secondaryKey);
                    DatabaseApp.DeliveryNote.AddObject(deliveryNote);
                    _AddedUnsavedDeliveryNotes.Add(deliveryNote);
                    deliveryNote.DeliveryCompanyAddress = SelectedInOrderPos.InOrder.DeliveryCompanyAddress;
                    deliveryNote.ShipperCompanyAddress = SelectedInOrderPos.InOrder.DeliveryCompanyAddress;
                }

                List<object> resultNewEntities = new List<object>();
                Msg result = InDeliveryNoteManager.AssignInOrderPos(CurrentInOrderPos, deliveryNote, null, DatabaseApp, ACFacilityManager, resultNewEntities);
                if (result != null)
                {
                    Messages.Msg(result);
                    return;
                }

                //DeliveryNotePos newDeliveryNotePos = DeliveryNotePos.NewACObject(DatabaseApp, newDeliveryNote);
                //newDeliveryNotePos.InOrderPos = SelectedInOrderPos;
                //SelectedInOrderPos.MDDelivPosState = DatabaseApp.MDDelivPosState.Where(c => c.MDDelivPosStateIndex == (short)MDDelivPosState.DelivPosStates.CompletelyAssigned).First();
                //newDeliveryNote.DeliveryNotePos_DeliveryNote.Add(newDeliveryNotePos);


                TourplanPos tourplanPos = CurrentTourplan.TourplanPos_Tourplan.ToList().Where(c => c.CompanyAddressID == SelectedInOrderPos.InOrder.DeliveryCompanyAddressID).FirstOrDefault();
                if (tourplanPos == null)
                {
                    tourplanPos = TourplanPos.NewACObject(DatabaseApp, CurrentTourplan);
                    tourplanPos.CompanyAddress = SelectedInOrderPos.InOrder.DeliveryCompanyAddress;
                    tourplanPos.Company = SelectedInOrderPos.InOrder.DistributorCompany;
                    CurrentTourplan.TourplanPos_Tourplan.Add(tourplanPos);
                }
                deliveryNote.TourplanPos = tourplanPos;

                if (_UnSavedUnAssignedInOrderPos.Contains(CurrentInOrderPos))
                    _UnSavedUnAssignedInOrderPos.Remove(CurrentInOrderPos);
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOTourPlan", "AssignInOrderPos", msg);
                return;
            }

            //SelectedInOrderPos.Tourplan = CurrentTourplan;
            RefreshInOrderPosList();
            OnPropertyChanged("InOrderTourPosList");
        }

        /// <summary>
        /// Determines whether [is enabled assign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignInOrderPos()
        {
            return CurrentTourplan != null && SelectedInOrderPos != null;
        }

        /// <summary>
        /// Unassigns the in order pos.
        /// </summary>
        [ACMethodInteraction(InOrderPos.ClassName, "en{'<'}de{'<'}", 601, true, "SelectedInOrderTourPos")]
        public void UnassignInOrderPos()
        {
            if (!IsEnabledUnassignInOrderPos())
                return;

            DeliveryNotePos deliveryNotePos = AssignedInDeliveryNotePosList.Where(c => c.InOrderPosID == SelectedInOrderTourPos.InOrderPosID).FirstOrDefault();
            if (deliveryNotePos == null)
                return;
            DeliveryNote deliveryNote = deliveryNotePos.DeliveryNote;

            InOrderPos parentInOrderPos = null;
            InOrderPos currentInOrderPos = deliveryNotePos.InOrderPos;
            parentInOrderPos = deliveryNotePos.InOrderPos.InOrderPos1_ParentInOrderPos;

            Msg result = null;
            try
            {
                result = InDeliveryNoteManager.UnassignInOrderPos(deliveryNotePos, DatabaseApp);
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

                Messages.LogException("BSOTourPlan", "UnassignInOrderPos", msg);
                return;
            }

            if (result == null && parentInOrderPos != null)
            {
                if (!_UnSavedUnAssignedInOrderPos.Contains(parentInOrderPos))
                    _UnSavedUnAssignedInOrderPos.Add(parentInOrderPos);

                if (!deliveryNote.DeliveryNotePos_DeliveryNote.Any())
                {
                    deliveryNote.TourplanPos = null;
                    _AddedUnsavedDeliveryNotes.Remove(deliveryNote);
                    if (deliveryNote.EntityState != System.Data.EntityState.Added)
                        deliveryNote.DeleteACObject(DatabaseApp, true);
                }
            }

            //bool inCache = false;
            //DeliveryNotePos deliveryNotePos = null;
            //DeliveryNote deliveryNote = null;
            //var queryDeliveryNotePos = DatabaseApp.DeliveryNotePos.Where(c => c.InOrderPosID == SelectedInOrderTourPos.InOrderPosID);
            //if (!queryDeliveryNotePos.Any())
            //{
            //    foreach (DeliveryNote deliveryNote2 in _AddedUnsavedDeliveryNotes)
            //    {
            //        foreach (DeliveryNotePos deliveryNotePos2 in deliveryNote2.DeliveryNotePos_DeliveryNote)
            //        {
            //            if ((deliveryNotePos2.InOrderPos != null) && (deliveryNotePos2.InOrderPos == SelectedInOrderTourPos))
            //            {
            //                inCache = true;
            //                deliveryNotePos = deliveryNotePos2;
            //                deliveryNote = deliveryNote2;
            //                break;
            //            }
            //        }
            //    }
            //    if (!inCache)
            //        return;
            //}
            //else
            //{
            //    deliveryNotePos = queryDeliveryNotePos.First();
            //    deliveryNote = deliveryNotePos.DeliveryNote;
            //}


            //deliveryNote.TourplanPos = null;
            //deliveryNotePos.InOrderPos.MDDelivPosState = DatabaseApp.MDDelivPosState.Where(c => c.MDDelivPosStateIndex == (short)MDDelivPosState.DelivPosStates.NotPlanned).First();
            //deliveryNotePos.InOrderPos = null;
            //deliveryNote.DeliveryNotePos_DeliveryNote.Remove(deliveryNotePos);
            //if (!inCache)
            //    deliveryNotePos.DeleteACObject(DatabaseApp, true);
            //_AddedUnsavedDeliveryNotes.Remove(deliveryNote);
            //if (!inCache)
            //    deliveryNote.DeleteACObject(DatabaseApp, true);

            RefreshInOrderPosList();
            OnPropertyChanged("InOrderTourPosList");
        }

        /// <summary>
        /// Determines whether [is enabled unassign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled unassign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUnassignInOrderPos()
        {
            return CurrentTourplan != null && SelectedInOrderTourPos != null;
        }

        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Filter In-Order'}de{'Filter Bestellungen'}", 602, false)]
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

        #endregion

        #region OutOrder-Assignment
        /// <summary>
        /// Assigns the out order pos.
        /// </summary>
        [ACMethodInteraction(OutOrderPos.ClassName, "en{'>'}de{'>'}", 603, true, "SelectedOutOrderPos")]
        public void AssignOutOrderPos()
        {
            if (!IsEnabledAssignOutOrderPos())
                return;

            try
            {
                DeliveryNote deliveryNote = AssignedDeliveryNotes.Where(c => c.DeliveryCompanyAddressID == SelectedInOrderPos.InOrder.DeliveryCompanyAddressID).FirstOrDefault();
                if (deliveryNote == null)
                {
                    string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(DeliveryNote), DeliveryNote.NoColumnName, DeliveryNote.FormatNewNo, this);
                    deliveryNote = DeliveryNote.NewACObject(DatabaseApp, null, secondaryKey);
                    DatabaseApp.DeliveryNote.AddObject(deliveryNote);
                    _AddedUnsavedDeliveryNotes.Add(deliveryNote);
                    deliveryNote.DeliveryCompanyAddress = SelectedOutOrderPos.OutOrder.DeliveryCompanyAddress;
                    deliveryNote.ShipperCompanyAddress = SelectedOutOrderPos.OutOrder.DeliveryCompanyAddress;
                }

                List<object> resultNewEntities = new List<object>();
                Msg result = OutDeliveryNoteManager.AssignOutOrderPos(CurrentOutOrderPos, deliveryNote, null, DatabaseApp, null, resultNewEntities);
                if (result != null)
                {
                    Messages.Msg(result);
                    return;
                }

                //DeliveryNotePos newDeliveryNotePos = DeliveryNotePos.NewACObject(DatabaseApp, newDeliveryNote);
                //newDeliveryNotePos.OutOrderPos = SelectedOutOrderPos;
                //SelectedOutOrderPos.MDDelivPosState = DatabaseApp.MDDelivPosState.Where(c => c.MDDelivPosStateIndex == (short)MDDelivPosState.DelivPosStates.CompletelyAssigned).First();
                //newDeliveryNote.DeliveryNotePos_DeliveryNote.Add(newDeliveryNotePos);

                TourplanPos tourplanPos = CurrentTourplan.TourplanPos_Tourplan.ToList().Where(c => c.CompanyAddressID == SelectedOutOrderPos.OutOrder.DeliveryCompanyAddressID).FirstOrDefault();
                if (tourplanPos == null)
                {
                    tourplanPos = TourplanPos.NewACObject(DatabaseApp, CurrentTourplan);
                    tourplanPos.CompanyAddress = SelectedOutOrderPos.OutOrder.DeliveryCompanyAddress;
                    tourplanPos.Company = SelectedOutOrderPos.OutOrder.CustomerCompany;
                    CurrentTourplan.TourplanPos_Tourplan.Add(tourplanPos);
                }
                deliveryNote.TourplanPos = tourplanPos;

                if (_UnSavedUnAssignedOutOrderPos.Contains(CurrentOutOrderPos))
                    _UnSavedUnAssignedOutOrderPos.Remove(CurrentOutOrderPos);

            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.Message != null)
                    msg += " Inner:" + e.InnerException.Message;

                Messages.LogException("BSOTourPlan", "AssignOutOrderPos", msg);
                return;
            }

            RefreshOutOrderPosList();
            OnPropertyChanged("OutOrderTourPosList");
        }

        /// <summary>
        /// Determines whether [is enabled assign out order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign out order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignOutOrderPos()
        {
            return CurrentTourplan != null && SelectedOutOrderPos != null;
        }

        /// <summary>
        /// Unassigns the out order pos.
        /// </summary>
        [ACMethodInteraction(OutOrderPos.ClassName, "en{'<'}de{'<'}", 604, true, "SelectedOutOrderTourPos")]
        public void UnassignOutOrderPos()
        {
            if (!IsEnabledUnassignOutOrderPos())
                return;
            DeliveryNotePos deliveryNotePos = AssignedOutDeliveryNotePosList.Where(c => c.OutOrderPosID == SelectedOutOrderTourPos.OutOrderPosID).FirstOrDefault();
            if (deliveryNotePos == null)
                return;
            DeliveryNote deliveryNote = deliveryNotePos.DeliveryNote;

            OutOrderPos parentOutOrderPos = null;
            OutOrderPos currentOutOrderPos = deliveryNotePos.OutOrderPos;
            parentOutOrderPos = deliveryNotePos.OutOrderPos.OutOrderPos1_ParentOutOrderPos;

            Msg result = null;
            try
            {
                result = OutDeliveryNoteManager.UnassignOutOrderPos(deliveryNotePos, DatabaseApp);
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

                Messages.LogException("BSOTourPlan", "UnassignOutOrderPos", msg);
                return;
            }

            if (result == null && parentOutOrderPos != null)
            {
                if (!_UnSavedUnAssignedOutOrderPos.Contains(parentOutOrderPos))
                    _UnSavedUnAssignedOutOrderPos.Add(parentOutOrderPos);

                if (!deliveryNote.DeliveryNotePos_DeliveryNote.Any())
                {
                    deliveryNote.TourplanPos = null;
                    _AddedUnsavedDeliveryNotes.Remove(deliveryNote);
                    if (deliveryNote.EntityState != System.Data.EntityState.Added)
                        deliveryNote.DeleteACObject(DatabaseApp, true);
                }
            }

            //SelectedOutOrderTourPos.Tourplan = null;
            //bool inCache = false;
            //var queryDeliveryNotePos = DatabaseApp.DeliveryNotePos.Where(c => c.OutOrderPosID == SelectedOutOrderTourPos.OutOrderPosID);
            //if (!queryDeliveryNotePos.Any())
            //{
            //    foreach (DeliveryNote deliveryNote2 in _AddedUnsavedDeliveryNotes)
            //    {
            //        foreach (DeliveryNotePos deliveryNotePos2 in deliveryNote2.DeliveryNotePos_DeliveryNote)
            //        {
            //            if ((deliveryNotePos2.OutOrderPos != null) && (deliveryNotePos2.OutOrderPos == SelectedOutOrderTourPos))
            //            {
            //                inCache = true;
            //                deliveryNotePos = deliveryNotePos2;
            //                deliveryNote = deliveryNote2;
            //                break;
            //            }
            //        }
            //    }
            //    if (!inCache)
            //        return;
            //}
            //else
            //{
            //    deliveryNotePos = queryDeliveryNotePos.First();
            //    deliveryNote = deliveryNotePos.DeliveryNote;
            //}

            //deliveryNote.TourplanPos = null;
            //deliveryNotePos.OutOrderPos.MDDelivPosState = DatabaseApp.MDDelivPosState.Where(c => c.MDDelivPosStateIndex == (short)MDDelivPosState.DelivPosStates.NotPlanned).First();
            //deliveryNotePos.OutOrderPos = null;
            //deliveryNote.DeliveryNotePos_DeliveryNote.Remove(deliveryNotePos);
            //if (!inCache)
            //    deliveryNotePos.DeleteACObject(DatabaseApp, true);
            //_AddedUnsavedDeliveryNotes.Remove(deliveryNote);
            //if (!inCache)
            //    deliveryNote.DeleteACObject(DatabaseApp, true);
            RefreshOutOrderPosList();
            OnPropertyChanged("OutOrderTourPosList");
        }

        /// <summary>
        /// Determines whether [is enabled unassign out order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled unassign out order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUnassignOutOrderPos()
        {
            return CurrentTourplan != null && SelectedOutOrderTourPos != null;
        }


        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Filter Sales-Order'}de{'Filter Aufträge'}", 605, false)]
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


        public void RefreshInOrderPosList(bool forceQueryFromDb = false)
        {
            if (AccessInOrderPos == null)
                return;
            AccessInOrderPos.NavSearch(DatabaseApp);
            OnPropertyChanged("InOrderPosList");
        }

        public void RefreshOutOrderPosList(bool forceQueryFromDb = false)
        {
            if (AccessOutOrderPos == null)
                return;
            AccessOutOrderPos.NavSearch(DatabaseApp);
            OnPropertyChanged("OutOrderPosList");
        }

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "Save":
                    Save();
                    return true;
                case "IsEnabledSave":
                    result = IsEnabledSave();
                    return true;
                case "UndoSave":
                    UndoSave();
                    return true;
                case "IsEnabledUndoSave":
                    result = IsEnabledUndoSave();
                    return true;
                case "Load":
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case "IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
                case "New":
                    New();
                    return true;
                case "IsEnabledNew":
                    result = IsEnabledNew();
                    return true;
                case "Delete":
                    Delete();
                    return true;
                case "IsEnabledDelete":
                    result = IsEnabledDelete();
                    return true;
                case "Search":
                    Search();
                    return true;
                case "AssignInOrderPos":
                    AssignInOrderPos();
                    return true;
                case "IsEnabledAssignInOrderPos":
                    result = IsEnabledAssignInOrderPos();
                    return true;
                case "UnassignInOrderPos":
                    UnassignInOrderPos();
                    return true;
                case "IsEnabledUnassignInOrderPos":
                    result = IsEnabledUnassignInOrderPos();
                    return true;
                case "FilterDialogInOrderPos":
                    result = FilterDialogInOrderPos();
                    return true;
                case "AssignOutOrderPos":
                    AssignOutOrderPos();
                    return true;
                case "IsEnabledAssignOutOrderPos":
                    result = IsEnabledAssignOutOrderPos();
                    return true;
                case "UnassignOutOrderPos":
                    UnassignOutOrderPos();
                    return true;
                case "IsEnabledUnassignOutOrderPos":
                    result = IsEnabledUnassignOutOrderPos();
                    return true;
                case "FilterDialogOutOrderPos":
                    result = FilterDialogOutOrderPos();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }

    /// <summary>
    /// Class InOrderPosEqualityComparer
    /// </summary>
    public class InOrderPosEqualityComparer : IEqualityComparer<InOrderPos>
    {
        /// <summary>
        /// Equalses the specified val1.
        /// </summary>
        /// <param name="val1">The val1.</param>
        /// <param name="val2">The val2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool Equals(InOrderPos val1, InOrderPos val2)
        {
            if (val1.InOrderPosID == val2.InOrderPosID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(InOrderPos val)
        {
            return val.InOrderPosID.GetHashCode();
        }
    }

    /// <summary>
    /// Class OutOrderPosEqualityComparer
    /// </summary>
    public class OutOrderPosEqualityComparer : IEqualityComparer<OutOrderPos>
    {
        /// <summary>
        /// Equalses the specified val1.
        /// </summary>
        /// <param name="val1">The val1.</param>
        /// <param name="val2">The val2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool Equals(OutOrderPos val1, OutOrderPos val2)
        {
            if (val1.OutOrderPosID == val2.OutOrderPosID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(OutOrderPos val)
        {
            return val.OutOrderPosID.GetHashCode();
        }
    }
}
