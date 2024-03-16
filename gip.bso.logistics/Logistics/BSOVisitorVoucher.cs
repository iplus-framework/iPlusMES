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
using gip.mes.datamodel;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.autocomponent;
using gip.mes.facility;
using System.Data.Objects;
using System.Data;

namespace gip.bso.logistics
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Visitor Voucher'}de{'Besucher Beleg'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + VisitorVoucher.ClassName)]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "VisitorVoucherOpen", "en{'Visitor Voucher'}de{'Besucherbeleg'}", typeof(VisitorVoucher), VisitorVoucher.ClassName, "VisitorVoucherNo,MDVisitorVoucherState\\MDVisitorVoucherStateIndex", "VisitorVoucherNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "VisitorAssign", "en{'Visitor Assignment'}de{'Besucherzuordnung'}", typeof(Visitor), Visitor.ClassName, "VisitorNo,VehicleFacility\\FacilityNo,VisitorCompany\\CompanyNo,VisitorCompany\\CompanyName,VisitorCompanyPerson\\Name1,MDVisitorCard\\MDVisitorCardNo", "VisitorNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "DeliveryNoteUnAssigned", "en{'Delivery Note Assignment'}de{'Lieferscheinzuordnung'}", typeof(DeliveryNote), DeliveryNote.ClassName, "VisitorVoucherID", "DeliveryNoteNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "TourplanUnAssigned", "en{'Tourplan Assignment'}de{'Tourzuordnung'}", typeof(Tourplan), Tourplan.ClassName, "VisitorVoucherID", "TourplanNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "PickingUnAssigned", "en{'Picking Assignment'}de{'Tourzuordnung'}", typeof(Picking), Picking.ClassName, "PickingTypeIndex,VisitorVoucherID", "PickingNo")]
    [ACQueryInfo(Const.PackName_VarioLogistics, Const.QueryPrefix + "VisitorVoucherPrint", "en{'Print'}de{'Laufzettel'}", typeof(VisitorVoucher), VisitorVoucher.ClassName, "VisitorVoucherNo", "VisitorVoucherNo", new object[]
        {
            new object[] {Const.QueryPrefix + Picking.ClassName, "en{'Picking'}de{'Kommissionierauftrag'}", typeof(Picking), Picking.ClassName + "_" + VisitorVoucher.ClassName, "PickingNo", "PickingNo", new object[]
                {
                    new object[] {Const.QueryPrefix + PickingPos.ClassName, "en{'Picking Item'}de{'Kommissionierposition'}", typeof(PickingPos), PickingPos.ClassName + "_" + Picking.ClassName, "Sequence", "Sequence"}
                }
            }
        })
    ]
    public partial class BSOVisitorVoucher : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOVisitorVoucher"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOVisitorVoucher(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            _InDeliveryNoteManager = ACInDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_InDeliveryNoteManager == null)
                throw new Exception("InDeliveryNoteManager not configured");

            _OutDeliveryNoteManager = ACOutDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_OutDeliveryNoteManager == null)
                throw new Exception("OutDeliveryNoteManager not configured");

            _VisitorVoucherManager = ACVisitorVoucherManager.ACRefToServiceInstance(this);
            if (_VisitorVoucherManager == null)
                throw new Exception("VisitorVoucherManager not configured");

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            if (!base.ACInit(startChildMode))
                return false;

            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACOutDeliveryNoteManager.DetachACRefFromServiceInstance(this, _OutDeliveryNoteManager);
            _OutDeliveryNoteManager = null;
            ACInDeliveryNoteManager.DetachACRefFromServiceInstance(this, _InDeliveryNoteManager);
            _InDeliveryNoteManager = null;
            ACVisitorVoucherManager.DetachACRefFromServiceInstance(this, _VisitorVoucherManager);
            _VisitorVoucherManager = null;
            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;

            this._AccessUnAssignedDeliveryNote = null;
            this._AccessUnAssignedPicking = null;
            this._AccessUnAssignedTourplan = null;
            this._AccessVisitor = null;
            this._CurrentDeliveryNote = null;
            this._CurrentPicking = null;
            this._CurrentTourplan = null;
            this._SelectedDeliveryNote = null;
            this._SelectedPicking = null;
            this._SelectedTourplan = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessVisitor != null)
            {
                _AccessVisitor.ACDeInit(false);
                _AccessVisitor = null;
            }
            if (_AccessUnAssignedDeliveryNote != null)
            {
                _AccessUnAssignedDeliveryNote.ACDeInit(false);
                _AccessUnAssignedDeliveryNote = null;
            }
            if (_AccessUnAssignedPicking != null)
            {
                _AccessUnAssignedPicking.NavSearchExecuting -= _Picking_NavSearchExecuting;
                _AccessUnAssignedPicking.ACDeInit(false);
                _AccessUnAssignedPicking = null;
            }
            if (_AccessUnAssignedTourplan != null)
            {
                _AccessUnAssignedTourplan.ACDeInit(false);
                _AccessUnAssignedTourplan = null;
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

        #region Local Properties
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

        protected ACRef<ACVisitorVoucherManager> _VisitorVoucherManager = null;
        protected ACVisitorVoucherManager VisitorVoucherManager
        {
            get
            {
                if (_VisitorVoucherManager == null)
                    return null;
                return _VisitorVoucherManager.ValueT;
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

        #region VisitorVoucher

        [ACPropertySelected(604, "FilterVisitor", "en{'Filter visitor vouchers'}de{'Filter Besucherbelege'}")]
        public MDVisitorVoucherState FilterVisitorVouchers
        {
            get
            {
                if (AccessPrimary != null)
                {
                    ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "MDVisitorVoucherState\\MDVisitorVoucherStateIndex").FirstOrDefault();
                    if (filterItem == null)
                    {
                        RebuildAccessPrimary();
                        filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "MDVisitorVoucherState\\MDVisitorVoucherStateIndex").FirstOrDefault();
                    }
                    if (filterItem != null)
                    {
                        if (String.IsNullOrEmpty(filterItem.SearchWord))
                        {
                            AccessPrimary.NavACQueryDefinition.SaveToACConfigOff = true;
                            filterItem.SearchWord = ((short)MDVisitorVoucherState.VisitorVoucherStates.CheckedOut).ToString();
                            AccessPrimary.NavACQueryDefinition.SaveToACConfigOff = false;
                        }
                        MDVisitorVoucherState.VisitorVoucherStates enumState = (MDVisitorVoucherState.VisitorVoucherStates)System.Convert.ToInt16(filterItem.SearchWord);
                        MDVisitorVoucherState mdState = MDVisitorVoucherState.FromEnum(DatabaseApp, enumState);
                        return mdState;
                    }
                }
                return null;
            }

            set
            {
                if (value == null)
                    return;
                if (AccessPrimary != null)
                {
                    ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "MDVisitorVoucherState\\MDVisitorVoucherStateIndex").FirstOrDefault();
                    if (filterItem == null)
                    {
                        RebuildAccessPrimary();
                        filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "MDVisitorVoucherState\\MDVisitorVoucherStateIndex").FirstOrDefault();
                    }
                    if (filterItem != null)
                    {
                        string filterValue = value.MDVisitorVoucherStateIndex.ToString();
                        if (filterValue != filterItem.SearchWord)
                        {
                            AccessPrimary.NavACQueryDefinition.SaveToACConfigOff = true;
                            filterItem.SearchWord = value.MDVisitorVoucherStateIndex.ToString();
                            AccessPrimary.NavACQueryDefinition.SaveToACConfigOff = false;
                            Search();
                        }
                    }
                }
                OnPropertyChanged("FilterVisitorVouchers");
            }
        }

        [ACPropertyList(605, "FilterVisitor")]
        public IEnumerable<MDVisitorVoucherState> FilterVisitorVouchersList
        {
            get
            {
                return DatabaseApp.MDVisitorVoucherState.ToList();
            }
        }


        public void RebuildAccessPrimary()
        {
            if (_AccessPrimary == null || _AccessPrimary.NavACQueryDefinition == null || ACType == null)
                return;
            String checkedOut = ((short)MDVisitorVoucherState.VisitorVoucherStates.CheckedIn).ToString();
            _AccessPrimary.NavACQueryDefinition.ClearFilter(true);
            _AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "VisitorVoucherNo", Global.LogicalOperators.equal, Global.Operators.and, "", true));
            _AccessPrimary.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDVisitorVoucherState\\MDVisitorVoucherStateIndex", Global.LogicalOperators.lessThanOrEqual, Global.Operators.and, checkedOut, true));
        }

        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<VisitorVoucher> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, VisitorVoucher.ClassName)]
        public ACAccessNav<VisitorVoucher> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition2 = Root.Queries.CreateQuery(null, Const.QueryPrefix + "VisitorVoucherPrint", ACType.ACIdentifier);
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "VisitorVoucherOpen", ACType.ACIdentifier);

                    if (navACQueryDefinition != null)
                    {
                        ACSortItem sortItem = navACQueryDefinition.ACSortColumns.Where(c => c.ACIdentifier == "VisitorVoucherNo").FirstOrDefault();
                        if (sortItem != null && sortItem.IsConfiguration)
                            sortItem.SortDirection = Global.SortDirections.descending;
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                    }

                    _AccessPrimary = navACQueryDefinition.NewAccessNav<VisitorVoucher>(VisitorVoucher.ClassName, this);

                    bool rebuildACQueryDef = false;
                    //String checkedOut = ((short)MDVisitorVoucherState.VisitorVoucherStates.CheckedOut).ToString();
                    if (!navACQueryDefinition.ACFilterColumns.Any())
                    {
                        rebuildACQueryDef = true;
                    }
                    else
                    {
                        int countFoundCorrect = 0;
                        foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            if (filterItem.PropertyName == "VisitorVoucherNo")
                            {
                                if (filterItem.Operator == Global.Operators.and)
                                    countFoundCorrect++;
                            }
                            else if (filterItem.PropertyName == "MDVisitorVoucherState\\MDVisitorVoucherStateIndex")
                            {
                                if (filterItem.LogicalOperator == Global.LogicalOperators.lessThanOrEqual)
                                    countFoundCorrect++;
                            }
                        }
                        if (countFoundCorrect < 2)
                            rebuildACQueryDef = true;
                    }
                    if (rebuildACQueryDef)
                    {
                        RebuildAccessPrimary();
                        navACQueryDefinition.SaveConfig(true);
                    }
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the current visitor.
        /// </summary>
        /// <value>The current visitor.</value>
        [ACPropertyCurrent(601, VisitorVoucher.ClassName)]
        public VisitorVoucher CurrentVisitorVoucher
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
                TempNewVisitor = null;
                AccessPrimary.Current = value;
                OnPropertyChanged("CurrentVisitorVoucher");
                EmptyVisitorSearch();
                OnPropertyChanged("CurrentVisitor");

                RefreshLists(true);
            }
        }

        /// <summary>
        /// Gets the visitor list.
        /// </summary>
        /// <value>The visitor list.</value>
        [ACPropertyList(602, VisitorVoucher.ClassName)]
        public IEnumerable<VisitorVoucher> VisitorVoucherList
        {
            get
            {
                if (AccessPrimary == null)
                    return null;
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected visitor.
        /// </summary>
        /// <value>The selected visitor.</value>
        [ACPropertySelected(603, VisitorVoucher.ClassName)]
        public VisitorVoucher SelectedVisitorVoucher
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
                OnPropertyChanged("SelectedVisitorVoucher");
            }
        }
        #endregion

        #region Visitor Assignment

        protected Visitor TempNewVisitor
        {
            get;
            set;
        }

        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<Visitor> _AccessVisitor;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(691, "VisitorAssign")]
        public ACAccessNav<Visitor> AccessVisitor
        {
            get
            {
                if (_AccessVisitor == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "VisitorAssign", ACType.ACIdentifier);
                    bool rebuildACQueryDef = false;
                    if (!navACQueryDefinition.ACFilterColumns.Any())
                    {
                        rebuildACQueryDef = true;
                    }
                    else
                    {
                        int countFoundCorrect = 0;
                        foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            if (filterItem.PropertyName == "VisitorNo")
                            {
                                //if (filterItem.SearchWord == assigned.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.lessThan)
                                countFoundCorrect++;
                            }
                            else if (filterItem.PropertyName == "VehicleFacility\\FacilityNo")
                                countFoundCorrect++;
                            else if (filterItem.PropertyName == "VisitorCompany\\CompanyNo")
                                countFoundCorrect++;
                            else if (filterItem.PropertyName == "VisitorCompany\\CompanyName")
                                countFoundCorrect++;
                            else if (filterItem.PropertyName == "VisitorCompanyPerson\\Name1")
                                countFoundCorrect++;
                            else if (filterItem.PropertyName == "MDVisitorCard\\MDVisitorCardNo")
                                countFoundCorrect++;
                        }
                        if (countFoundCorrect < 6)
                            rebuildACQueryDef = true;
                    }
                    if (rebuildACQueryDef)
                    {
                        navACQueryDefinition.ClearFilter(true);
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "VisitorNo", Global.LogicalOperators.contains, Global.Operators.or, null, true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "VehicleFacility\\FacilityNo", Global.LogicalOperators.contains, Global.Operators.or, null, true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "VisitorCompany\\CompanyNo", Global.LogicalOperators.contains, Global.Operators.or, null, true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "VisitorCompany\\CompanyName", Global.LogicalOperators.contains, Global.Operators.or, null, true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "VisitorCompanyPerson\\Name1", Global.LogicalOperators.contains, Global.Operators.or, null, true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDVisitorCard\\MDVisitorCardNo", Global.LogicalOperators.contains, Global.Operators.or, null, true));
                        navACQueryDefinition.SaveConfig(true);
                    }
                    _AccessVisitor = navACQueryDefinition.NewAccessNav<Visitor>("VisitorAssign", this);
                    _AccessVisitor.AutoSaveOnNavigation = false;
                }
                return _AccessVisitor;
            }
        }

        /// <summary>
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(606, "VisitorAssign", "en{'Select Visitor'}de{'Auswahl Besucher'}")]
        public Visitor CurrentVisitor
        {
            get
            {
                if (AccessVisitor == null)
                    return null;
                return AccessVisitor.Current;
            }
            set
            {
                if (AccessVisitor == null)
                    return;
                AccessVisitor.Current = value;
                if (CurrentVisitorVoucher != null)
                {
                    CurrentVisitorVoucher.Visitor = value;
                    OnPropertyChanged("CurrentVisitorVoucher");
                }
                OnPropertyChanged("CurrentVisitor");
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(607, "VisitorAssign")]
        public IEnumerable<Visitor> VisitorList
        {
            get
            {
                return AccessVisitor.NavList;
            }
        }


        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(608, "AllVisitor")]
        public IEnumerable<Visitor> AllVisitorList
        {
            get
            {
                List<Visitor> visitorList = DatabaseApp.Visitor.ToList();
                IList<Visitor> visitorListAdded = DatabaseApp.GetAddedEntities<Visitor>();
                if (!visitorListAdded.Any())
                    visitorList.AddRange(visitorListAdded);
                return visitorList;
            }
        }
        #endregion

        #region Visitor Assignment Search-Properties

        [ACPropertyInfo(609, "", "en{'Find Visitor'}de{'Suche Besucher'}")]
        public string FindVisitor
        {
            get
            {
                if (AccessVisitor != null)
                    return AccessVisitor.NavACQueryDefinition.SearchWord;
                return null;
            }
            set
            {
                if (TempNewVisitor != null)
                    return;
                if (AccessVisitor != null)
                {
                    AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = true;
                    AccessVisitor.NavACQueryDefinition.SearchWord = value;
                    AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = false;
                    AccessVisitor.NavSearch(this.DatabaseApp);
                    OnPropertyChanged("VisitorList");
                    CurrentVisitor = VisitorList.FirstOrDefault();
                }
                OnPropertyChanged("FindVisitor");
            }
        }

        [ACPropertyInfo(610, "", "en{'Find by Visitor-No.'}de{'Suche nach Besucher-Nr.'}")]
        public string FindVisitorNo
        {
            get
            {
                if (AccessVisitor != null)
                {
                    ACFilterItem filterItem = AccessVisitor.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "VisitorNo").FirstOrDefault();
                    if (filterItem != null)
                        return filterItem.SearchWord;
                }
                return null;
            }

            set
            {
                if (TempNewVisitor != null)
                    return;
                if (AccessVisitor != null)
                {
                    ACFilterItem filterItem = AccessVisitor.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "VisitorNo").FirstOrDefault();
                    if (filterItem != null)
                    {
                        AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = true;
                        filterItem.SearchWord = value;
                        AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = false;
                        AccessVisitor.NavSearch(this.DatabaseApp);
                        OnPropertyChanged("VisitorList");
                        CurrentVisitor = VisitorList.FirstOrDefault();
                    }
                }
                OnPropertyChanged("FindVisitorNo");
            }
        }

        [ACPropertyInfo(611, "", "en{'Find by Vehicle-No.'}de{'Suche nach Fahrzeug-Nr.'}")]
        public string FindVisitorVehicleNo
        {
            get
            {
                if (AccessVisitor != null)
                {
                    ACFilterItem filterItem = AccessVisitor.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "VehicleFacility\\FacilityNo").FirstOrDefault();
                    if (filterItem != null)
                        return filterItem.SearchWord;
                }
                return null;
            }

            set
            {
                if (TempNewVisitor != null)
                    return;
                if (AccessVisitor != null)
                {
                    ACFilterItem filterItem = AccessVisitor.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "VehicleFacility\\FacilityNo").FirstOrDefault();
                    if (filterItem != null)
                    {
                        AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = true;
                        filterItem.SearchWord = value;
                        AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = false;
                        AccessVisitor.NavSearch(this.DatabaseApp);
                        OnPropertyChanged("VisitorList");
                        CurrentVisitor = VisitorList.FirstOrDefault();
                    }
                }
                OnPropertyChanged("FindVisitorVehicleNo");
            }
        }

        [ACPropertyInfo(612, "", "en{'Find by Company-No.'}de{'Suche nach Firmen-Nr.'}")]
        public string FindVisitorCompanyNo
        {
            get
            {
                if (AccessVisitor != null)
                {
                    ACFilterItem filterItem = AccessVisitor.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "VisitorCompany\\CompanyNo").FirstOrDefault();
                    if (filterItem != null)
                        return filterItem.SearchWord;
                }
                return null;
            }

            set
            {
                if (TempNewVisitor != null)
                    return;
                if (AccessVisitor != null)
                {
                    ACFilterItem filterItem = AccessVisitor.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "VisitorCompany\\CompanyNo").FirstOrDefault();
                    if (filterItem != null)
                    {
                        AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = true;
                        filterItem.SearchWord = value;
                        AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = false;
                        AccessVisitor.NavSearch(this.DatabaseApp);
                        OnPropertyChanged("VisitorList");
                        CurrentVisitor = VisitorList.FirstOrDefault();
                    }
                }
                OnPropertyChanged("FindVisitorCompanyNo");
            }
        }

        [ACPropertyInfo(613, "", "en{'Find by name of company'}de{'Suche nach Firmenname'}")]
        public string FindVisitorCompanyName
        {
            get
            {
                if (AccessVisitor != null)
                {
                    ACFilterItem filterItem = AccessVisitor.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "VisitorCompany\\CompanyName").FirstOrDefault();
                    if (filterItem != null)
                        return filterItem.SearchWord;
                }
                return null;
            }

            set
            {
                if (TempNewVisitor != null)
                    return;
                if (AccessVisitor != null)
                {
                    ACFilterItem filterItem = AccessVisitor.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "VisitorCompany\\CompanyName").FirstOrDefault();
                    if (filterItem != null)
                    {
                        AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = true;
                        filterItem.SearchWord = value;
                        AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = false;
                        AccessVisitor.NavSearch(this.DatabaseApp);
                        OnPropertyChanged("VisitorList");
                        CurrentVisitor = VisitorList.FirstOrDefault();
                    }
                }
                OnPropertyChanged("FindVisitorCompanyName");
            }
        }

        [ACPropertyInfo(614, "", "en{'Find by name of person'}de{'Suche nach Personenname'}")]
        public string FindVisitorPersonName
        {
            get
            {
                if (AccessVisitor != null)
                {
                    ACFilterItem filterItem = AccessVisitor.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "VisitorCompanyPerson\\Name1").FirstOrDefault();
                    if (filterItem != null)
                        return filterItem.SearchWord;
                }
                return null;
            }

            set
            {
                if (TempNewVisitor != null)
                    return;
                if (AccessVisitor != null)
                {
                    ACFilterItem filterItem = AccessVisitor.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "VisitorCompanyPerson\\Name1").FirstOrDefault();
                    if (filterItem != null)
                    {
                        AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = true;
                        filterItem.SearchWord = value;
                        AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = false;
                        AccessVisitor.NavSearch(this.DatabaseApp);
                        OnPropertyChanged("VisitorList");
                        CurrentVisitor = VisitorList.FirstOrDefault();
                    }
                }
                OnPropertyChanged("FindVisitorPersonName");
            }
        }

        [ACPropertyInfo(615, "", "en{'Find by Card-No.'}de{'Suche nach Karten-Nr.'}")]
        public string FindVisitorCardNo
        {
            get
            {
                if (AccessVisitor != null)
                {
                    ACFilterItem filterItem = AccessVisitor.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "MDVisitorCard\\MDVisitorCardNo").FirstOrDefault();
                    if (filterItem != null)
                        return filterItem.SearchWord;
                }
                return null;
            }

            set
            {
                if (TempNewVisitor != null)
                    return;
                if (AccessVisitor != null)
                {
                    ACFilterItem filterItem = AccessVisitor.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "MDVisitorCard\\MDVisitorCardNo").FirstOrDefault();
                    if (filterItem != null)
                    {
                        AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = true;
                        filterItem.SearchWord = value;
                        AccessVisitor.NavACQueryDefinition.SaveToACConfigOff = false;
                        AccessVisitor.NavSearch(this.DatabaseApp);
                        OnPropertyChanged("VisitorList");
                        CurrentVisitor = VisitorList.FirstOrDefault();
                    }
                }
                OnPropertyChanged("FindVisitorCardNo");
            }
        }

        #endregion

        #region Weighing
        private core.datamodel.ACClass _SelectedScale;
        [ACPropertySelected(611, "Scales", "en{'Vehicle scale'}de{'Fahrzeugwaage'}")]
        public core.datamodel.ACClass SelectedScale
        {
            get
            {
                return _SelectedScale;
            }
            set
            {
                if (_SelectedScale != value)
                {
                    _SelectedScale = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<core.datamodel.ACClass> _ScalesList;
        [ACPropertyList(610, "Scales")]
        public List<core.datamodel.ACClass> ScalesList
        {
            get
            {
                if (_ScalesList != null)
                    return _ScalesList;
                _ScalesList = core.datamodel.Database.s_cQry_FindInstancesOfClass(DatabaseApp.ContextIPlus, "PAEScaleCalibratable").ToList();
                return _ScalesList;
            }
            set
            {
                _ScalesList = value;
                OnPropertyChanged();
            }
        }


        Weighing _SelectedWeighing;
        [ACPropertySelected(658, "Weighings")]
        public Weighing SelectedWeighing
        {
            get
            {
                return _SelectedWeighing;
            }
            set
            {
                if (_SelectedWeighing != value)
                {
                    _SelectedWeighing = value;
                    OnPropertyChanged();
                }
            }
        }

        IEnumerable<Weighing> _WeighingList = null;

        [ACPropertyList(659, "Weighings")]
        public IEnumerable<Weighing> WeighingList
        {
            get
            {
                if (   this.SelectedVisitorVoucher == null
                    || this.SelectedVisitorVoucher.EntityState == EntityState.Added
                    || this.SelectedVisitorVoucher.EntityState == EntityState.Detached)
                    return null;
                if (_WeighingList != null)
                    return _WeighingList;
                _WeighingList = SelectedVisitorVoucher.Weighing_VisitorVoucher.OrderBy(c => c.StartDate).ToList();
                return _WeighingList;
            }
        }

        protected void RefreshWeighingList(bool forceRefresh = false)
        {
            if (forceRefresh && SelectedVisitorVoucher !=  null)
            {
                SelectedVisitorVoucher.Weighing_VisitorVoucher.AutoLoad();
                SelectedVisitorVoucher.Weighing_VisitorVoucher.AutoRefresh();
            }
            _WeighingList = null;
            OnPropertyChanged(nameof(WeighingList));
        }

        #endregion

        #endregion

        #region BSO->ACMethod

        #region VisitorVoucher
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(Visitor.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
            TempNewVisitor = null;
        }

        protected override void OnPostSave()
        {
            RefreshLists(true);
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
        [ACMethodCommand(Visitor.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
            Load();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand(Visitor.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        protected override void OnPostUndoSave()
        {
            RefreshLists(true);
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
        [ACMethodInteraction("SelectedVisitorVoucher", "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedVisitorVoucher", Global.ACKinds.MSMethodPrePost)]
        public virtual void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<VisitorVoucher>(requery, () => SelectedVisitorVoucher, () => CurrentVisitorVoucher, c => CurrentVisitorVoucher = c,
                        DatabaseApp.VisitorVoucher
                        .Include(c => c.DeliveryNote_VisitorVoucher)
                        .Include(c => c.Weighing_VisitorVoucher)
                        .Where(c => c.VisitorVoucherID == SelectedVisitorVoucher.VisitorVoucherID));
            if (CurrentVisitorVoucher != null)
            {
                CurrentVisitorVoucher.ACProperties.Refresh();
            }
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedVisitorVoucher != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("SelectedVisitorVoucher", "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedVisitorVoucher", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New"))
                return;
            if (AccessPrimary == null)
                return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(VisitorVoucher), VisitorVoucher.NoColumnName, VisitorVoucher.FormatNewNo, this);
            VisitorVoucher voucher = VisitorVoucher.NewACObject(DatabaseApp, null, secondaryKey);
            DatabaseApp.VisitorVoucher.AddObject(voucher);
            TempNewVisitor = null;
            AccessPrimary.NavList.Add(voucher);
            OnPropertyChanged("VisitorVoucherList");
            SelectedVisitorVoucher = voucher;
            CurrentVisitorVoucher = voucher;

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
        [ACMethodInteraction("SelectedVisitorVoucher", "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "SelectedVisitorVoucher", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete"))
                return;
            if (AccessPrimary == null)
                return;
            Msg msg = CurrentVisitorVoucher.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            AccessPrimary.NavList.Remove(CurrentVisitorVoucher);
            SelectedVisitorVoucher = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return true;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(Visitor.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public virtual void Search()
        {
            if (AccessPrimary == null)
                return;
            bool isSearchable = true;
            if (DatabaseApp.HasAddedEntities<VisitorVoucher>())
            {
                isSearchable = false;
                isSearchable = ACSaveOrUndoChanges();
            }
            if (isSearchable)
                AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("VisitorVoucherList");
            RefreshLists(true);
        }

        public virtual bool IsEnabledSearch()
        {
            return !DatabaseApp.HasAddedEntities<VisitorVoucher>();
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
            if (acAccess == _AccessVisitor)
            {
                _AccessVisitor.NavSearch(this.DatabaseApp);
                OnPropertyChanged("VisitorList");
                return true;
            }
            else if (acAccess == _AccessUnAssignedDeliveryNote)
            {
                _AccessUnAssignedDeliveryNote.NavSearch(this.DatabaseApp);
                OnPropertyChanged("UnAssignedDeliveryNoteList");
                return true;
            }
            else if (acAccess == _AccessUnAssignedPicking)
            {
                _AccessUnAssignedPicking.NavSearch(this.DatabaseApp);
                OnPropertyChanged("UnAssignedPickingList");
                return true;
            }
            else if (acAccess == _AccessUnAssignedTourplan)
            {
                _AccessUnAssignedTourplan.NavSearch(this.DatabaseApp);
                OnPropertyChanged("UnAssignedTourplanList");
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }

        #endregion

        #region New Deliverynote's
        [ACMethodInfo("Dialog", "en{'New Purchase Deliv.Note'}de{'Neuer Kauf.-Lieferschein'}", (short)MISort.New)]
        public void NewInDeliveryNote()
        {
            if (!IsEnabledNewInDeliveryNote())
                return;
            ACComponent childBSO = ACUrlCommand("?InDeliveryNoteDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("InDeliveryNoteDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!ShowDialogNewDeliveryNote", "");
            InDeliveryNoteDialog_OnDialogResult(dlgResult);
            childBSO.Stop();
        }

        void InDeliveryNoteDialog_OnDialogResult(VBDialogResult dlgResult)
        {
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                DeliveryNote result = dlgResult.ReturnValue as DeliveryNote;
                if (result != null)
                {
                    result.VisitorVoucher = CurrentVisitorVoucher;
                    OnPropertyChanged("DeliveryNoteList");
                }
            }
        }

        public bool IsEnabledNewInDeliveryNote()
        {
            return CurrentVisitorVoucher != null;
        }

        [ACMethodInfo("Dialog", "en{'New Sales Deliv.Note'}de{'Neuer Verkaufs.-Lieferschein'}", (short)MISort.New)]
        public void NewOutDeliveryNote()
        {
            if (!IsEnabledNewOutDeliveryNote()) return;
            ACComponent childBSO = ACUrlCommand("?OutDeliveryNoteDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("OutDeliveryNoteDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null) return;
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!ShowDialogNewDeliveryNote", "");
            InDeliveryNoteDialog_OnDialogResult(dlgResult);
            childBSO.Stop();
        }

        public bool IsEnabledNewOutDeliveryNote()
        {
            return CurrentVisitorVoucher != null;
        }
        #endregion

        #region New Visitor
        [ACMethodInfo("Dialog", "en{'New Visitor'}de{'Neuer Besucher'}", (short)MISort.New)]
        public void NewVisitor()
        {
            if (!IsEnabledNewVisitor())
                return;
            ACComponent childBSO = ACUrlCommand("?VisitorDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("VisitorDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!ShowDialogNewVisitor");
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                TempNewVisitor = dlgResult.ReturnValue as Visitor;
                if (TempNewVisitor != null)
                {
                    TempNewVisitor.VisitorVoucher_Visitor.Add(CurrentVisitorVoucher);
                    OnPropertyChanged("AllVisitorList");
                    CurrentVisitorVoucher.Visitor = TempNewVisitor;
                    OnPropertyChanged("CurrentVisitorVoucher");
                }
            }
            childBSO.Stop();
        }

        public bool IsEnabledNewVisitor()
        {
            return CurrentVisitorVoucher != null && TempNewVisitor == null;
        }
        #endregion

        #region Check In/Out
        /// <summary>
        /// Checks the in.
        /// </summary>
        [ACMethodCommand(Visitor.ClassName, "en{'Check In'}de{'Anmelden'}", (short)600, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void CheckIn()
        {
            if (!PreExecute("CheckIn")) return;
            MDVisitorVoucherState state = DatabaseApp.MDVisitorVoucherState.Where(c => c.MDVisitorVoucherStateIndex == (short)MDVisitorVoucherState.VisitorVoucherStates.CheckedIn).FirstOrDefault();
            if (state != null)
            {
                CurrentVisitorVoucher.MDVisitorVoucherState = state;
                CurrentVisitorVoucher.CheckInDate = DateTime.Now;
            }
            PostExecute("CheckIn");
        }

        /// <summary>
        /// Determines whether [is enabled check in].
        /// </summary>
        /// <returns><c>true</c> if [is enabled check in]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledCheckIn()
        {
            if (CurrentVisitorVoucher == null)
                return false;
            if (CurrentVisitorVoucher.MDVisitorVoucherState == null)
                return false;
            if (CurrentVisitorVoucher.MDVisitorVoucherState.VisitorVoucherState != MDVisitorVoucherState.VisitorVoucherStates.New)
                return false;
            return true;
        }

        /// <summary>
        /// Checks the out.
        /// </summary>
        [ACMethodCommand(Visitor.ClassName, "en{'Check Out'}de{'Abmelden'}", (short)601, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void CheckOut()
        {
            if (!PreExecute("CheckOut")) 
                return;
            if (this.VisitorVoucherManager == null)
                return;
            this.VisitorVoucherManager.CheckOut(CurrentVisitorVoucher, DatabaseApp);
            PostExecute("CheckOut");
        }

        /// <summary>
        /// Determines whether [is enabled check out].
        /// </summary>
        /// <returns><c>true</c> if [is enabled check out]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledCheckOut()
        {
            if (CurrentVisitorVoucher == null || VisitorVoucherManager == null)
                return false;
            if (CurrentVisitorVoucher.MDVisitorVoucherState == null)
                return false;
            if (CurrentVisitorVoucher.MDVisitorVoucherState.VisitorVoucherState != MDVisitorVoucherState.VisitorVoucherStates.CheckedIn)
                return false;
            return true;
        }
        #endregion

        #region Find-Visitor
        private void EmptyVisitorSearch()
        {
            if (!String.IsNullOrEmpty(FindVisitor))
                FindVisitor = "";
            if (!String.IsNullOrEmpty(FindVisitorNo))
                FindVisitorNo = "";
            if (!String.IsNullOrEmpty(FindVisitorVehicleNo))
                FindVisitorVehicleNo = "";
            if (!String.IsNullOrEmpty(FindVisitorCompanyNo))
                FindVisitorCompanyNo = "";
            if (!String.IsNullOrEmpty(FindVisitorCompanyName))
                FindVisitorCompanyName = "";
            if (!String.IsNullOrEmpty(FindVisitorPersonName))
                FindVisitorPersonName = "";
            if (!String.IsNullOrEmpty(FindVisitorCardNo))
                FindVisitorCardNo = "";
        }
        #endregion

        #region Refresh Lists

        public void RefreshLists(bool forceQueryFromDb = false)
        {
            RefreshUnAssignedDeliveryNoteList(forceQueryFromDb);
            OnPropertyChanged(nameof(DeliveryNoteList));

            RefreshUnAssignedTourplanList(forceQueryFromDb);
            OnPropertyChanged(nameof(TourplanList));

            RefreshUnAssignedPickingList(forceQueryFromDb);
            OnPropertyChanged(nameof(PickingList));

            RefreshWeighingList();
        }

        #endregion

        #region Weighing
        [ACMethodInfo("Dialog", "en{'Register Weight'}de{'Registriere Gewicht'}", (short)500)]
        public void RegisterWeight()
        {
            if (!IsEnabledRegisterWeight())
                return;
            string acUrl = SelectedScale.GetACUrlComponent();
            if (String.IsNullOrEmpty(acUrl))
                return;
            ACComponent scaleComp = Root.ACUrlCommand(acUrl) as ACComponent;
            if (scaleComp == null || scaleComp.ConnectionState == ACObjectConnectionState.DisConnected)
            {
                // TODO Message
                Messages.Error(this, "No connection", true);
                return;
            }
            Msg result = scaleComp.ACUrlCommand("!RegisterAlibiWeightEntity", new PAOrderInfoEntry() { EntityName = nameof(VisitorVoucher), EntityID = CurrentVisitorVoucher.VisitorVoucherID }) as Msg;
            if (result == null)
                return;
            if (result.MessageLevel > eMsgLevel.Info)
            {
                Messages.Msg(result);
                return;
            }

            RefreshWeighingList(true);
        }

        public bool IsEnabledRegisterWeight()
        {
            return SelectedScale != null;
        }
        #endregion

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Save):
                    Save();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(IsEnabledSearch):
                    result = IsEnabledSearch();
                    return true;
                case nameof(NewInDeliveryNote):
                    NewInDeliveryNote();
                    return true;
                case nameof(IsEnabledNewInDeliveryNote):
                    result = IsEnabledNewInDeliveryNote();
                    return true;
                case nameof(NewOutDeliveryNote):
                    NewOutDeliveryNote();
                    return true;
                case nameof(IsEnabledNewOutDeliveryNote):
                    result = IsEnabledNewOutDeliveryNote();
                    return true;
                case nameof(NewVisitor):
                    NewVisitor();
                    return true;
                case nameof(IsEnabledNewVisitor):
                    result = IsEnabledNewVisitor();
                    return true;
                case nameof(CheckIn):
                    CheckIn();
                    return true;
                case nameof(IsEnabledCheckIn):
                    result = IsEnabledCheckIn();
                    return true;
                case nameof(CheckOut):
                    CheckOut();
                    return true;
                case nameof(IsEnabledCheckOut):
                    result = IsEnabledCheckOut();
                    return true;
                case nameof(AssignDeliveryNote):
                    AssignDeliveryNote();
                    return true;
                case nameof(IsEnabledAssignDeliveryNote):
                    result = IsEnabledAssignDeliveryNote();
                    return true;
                case nameof(UnassignDeliveryNote):
                    UnassignDeliveryNote();
                    return true;
                case nameof(IsEnabledUnassignDeliveryNote):
                    result = IsEnabledUnassignDeliveryNote();
                    return true;
                case nameof(AssignTourplan):
                    AssignTourplan();
                    return true;
                case nameof(IsEnabledAssignTourplan):
                    result = IsEnabledAssignTourplan();
                    return true;
                case nameof(UnassignTourplan):
                    UnassignTourplan();
                    return true;
                case nameof(IsEnabledUnassignTourplan):
                    result = IsEnabledUnassignTourplan();
                    return true;
                case nameof(AssignPicking):
                    AssignPicking();
                    return true;
                case nameof(IsEnabledAssignPicking):
                    result = IsEnabledAssignPicking();
                    return true;
                case nameof(UnassignPicking):
                    UnassignPicking();
                    return true;
                case nameof(IsEnabledUnassignPicking):
                    result = IsEnabledUnassignPicking();
                    return true;
                case nameof(RegisterWeight):
                    RegisterWeight();
                    return true;
                case nameof(IsEnabledRegisterWeight):
                    result = IsEnabledRegisterWeight();
                    return true;
                case nameof(NavigateToAPicking):
                    NavigateToAPicking();
                    return true;
                case nameof(IsEnabledNavigateToAPicking):
                    result = IsEnabledNavigateToAPicking();
                    return true;
                case nameof(NavigateToUPicking):
                    NavigateToUPicking();
                    return true;
                case nameof(IsEnabledNavigateToUPicking):
                    result = IsEnabledNavigateToUPicking();
                    return true;
                case nameof(NavigateToADeliveryNote):
                    NavigateToADeliveryNote();
                    return true;
                case nameof(IsEnabledNavigateToADeliveryNote):
                    result = IsEnabledNavigateToADeliveryNote();
                    return true;
                case nameof(NavigateToUDeliveryNote):
                    NavigateToUDeliveryNote();
                    return true;
                case nameof(IsEnabledNavigateToUDeliveryNote):
                    result = IsEnabledNavigateToUDeliveryNote();
                    return true;
                case nameof(NavigateToATourplan):
                    NavigateToATourplan();
                    return true;
                case nameof(IsEnabledNavigateToATourplan):
                    result = IsEnabledNavigateToATourplan();
                    return true;
                case nameof(NavigateToUTourplan):
                    NavigateToUTourplan();
                    return true;
                case nameof(IsEnabledNavigateToUTourplan):
                    result = IsEnabledNavigateToUTourplan();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
