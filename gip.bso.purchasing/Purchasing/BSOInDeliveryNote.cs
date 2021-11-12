// ***********************************************************************
// Assembly         : gip.bso.purchasing
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOInDeliveryNote.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;

namespace gip.bso.purchasing
{
    /// <summary>
    /// Version 3
    /// Folgende alte Masken sind in diesem BSO enthalten:
    /// 1. Warenannahme-&gt;Lieferscheine
    /// 2. Warenannahme-&gt;Bestellpositionen zuordnen
    /// 1. Warenannahme-&gt;Warenbegleitscheine
    /// 2. Warenannahme-&gt;Warenbegleitscheine Prüfdaten
    /// 3. Warenannahme-&gt;Annahme verpackte Ware
    /// 4. Warenannahme-&gt;Annahme Loseware
    /// Neue Masken:
    /// 1. Warenannahme
    /// Im Gegensatz zum VarioBatch Classic wird die geplante Warenannahme aufgrund von Bestellungen
    /// nicht in die Vielzahl von Masken verteilt, sondern in einem zentralen und flexiblen BSO
    /// zusammengefaßt. Ziel ist es erstmal für die Version 1 und/oder Messeversion einen
    /// im Basisumfang funktionierenden Einkauf präsentieren zu können, der intuitiver ist,
    /// wie bisher. Später wird es dann wohl auch Branchenspezifische Varianten geben.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioPurchase, "een{'Purchase Delivery Note'}de{'Lieferschein (Einkauf)'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + DeliveryNote.ClassName)]
    [ACQueryInfo(Const.PackName_VarioPurchase, Const.QueryPrefix + "InOrderPosOpen", "en{'Open Purchase Order Pos.'}de{'Offene Bestellposition'}", typeof(InOrderPos), InOrderPos.ClassName, MDDelivPosState.ClassName + "\\MDDelivPosStateIndex", "TargetDeliveryDate,Material\\MaterialNo")]
    [ACQueryInfo(Const.PackName_VarioPurchase, Const.QueryPrefix + "BookingFacility", ConstApp.Facility, typeof(Facility), Facility.ClassName, MDFacilityType.ClassName + "\\MDFacilityTypeIndex", "FacilityNo")]
    public class BSOInDeliveryNote : ACBSOvbNav
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOInDeliveryNote"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOInDeliveryNote(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

            _InDeliveryNoteManager = ACInDeliveryNoteManager.ACRefToServiceInstance(this);
            if (_InDeliveryNoteManager == null)
                throw new Exception("InDeliveryNoteManager not configured");

            _ACFacilityManager = FacilityManager.ACRefToServiceInstance(this);
            if (_ACFacilityManager == null)
                throw new Exception("FacilityManager not configured");

            _LabOrderManager = ACLabOrderManager.ACRefToServiceInstance(this);
            if (_LabOrderManager == null)
                throw new Exception("LabOrderManager not configured");

            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            ACInDeliveryNoteManager.DetachACRefFromServiceInstance(this, _InDeliveryNoteManager);
            _InDeliveryNoteManager = null;
            FacilityManager.DetachACRefFromServiceInstance(this, _ACFacilityManager);
            _ACFacilityManager = null;
            ACLabOrderManager.DetachACRefFromServiceInstance(this, _LabOrderManager);
            _LabOrderManager = null;

            this._AccessBookingFacility = null;
            this._AccessDeliveryNotePos = null;
            this._AccessInOrderPos = null;
            this._BookingFacilityLotNo = null;
            this._BSOMsg = null;
            this._CurrentACMethodBookingDummy = null;
            this._CurrentDeliveryNotePos = null;
            this._CurrentFacilityBooking = null;
            this._CurrentFacilityPreBooking = null;
            this._CurrentInOrderPosFromPicking = null;
            this._PartialQuantity = null;
            this._SelectedDeliveryNotePos = null;
            this._SelectedFacilityBooking = null;
            this._SelectedFacilityBookingCharge = null;
            this._SelectedFacilityPreBooking = null;
            this._SelectedInOrderPosFromPicking = null;
            this._StateCompletelyAssigned = null;
            this._UnSavedAssignedPickingInOrderPos = null;
            this._UnSavedUnAssignedInOrderPos = null;
            if (_AccessPrimary != null)
                _AccessPrimary.NavSearchExecuting -= _AccessPrimary_NavSearchExecuting;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessDeliveryNotePos != null)
            {
                _AccessDeliveryNotePos.ACDeInit(false);
                _AccessDeliveryNotePos = null;
            }
            if (_AccessInOrderPos != null)
            {
                _AccessInOrderPos.ACDeInit(false);
                _AccessInOrderPos = null;
            }
            if (_AccessBookingFacility != null)
            {
                _AccessBookingFacility.ACDeInit(false);
                _AccessBookingFacility = null;
            }
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        public override object Clone()
        {
            BSOInDeliveryNote clone = base.Clone() as BSOInDeliveryNote;

            if (clone.AccessInOrderPos != null)
            {
                clone.AccessInOrderPos.NavACQueryDefinition.CopyFrom(this.AccessInOrderPos.NavACQueryDefinition, true, true, true);
                clone.AccessInOrderPos.NavSearch(DatabaseApp);
            }

            return clone;
        }
        #endregion

        #region Managers

        protected ACRef<ACInDeliveryNoteManager> _InDeliveryNoteManager = null;
        public ACInDeliveryNoteManager InDeliveryNoteManager
        {
            get
            {
                if (_InDeliveryNoteManager == null)
                    return null;
                return _InDeliveryNoteManager.ValueT;
            }
        }

        protected ACRef<ACComponent> _ACFacilityManager = null;
        public FacilityManager ACFacilityManager
        {
            get
            {
                if (_ACFacilityManager == null)
                    return null;
                return _ACFacilityManager.ValueT as FacilityManager;
            }
        }

        protected ACRef<ACLabOrderManager> _LabOrderManager = null;
        public ACLabOrderManager LabOrderManager
        {
            get
            {
                if (_LabOrderManager == null)
                    return null;
                return _LabOrderManager.ValueT;
            }
        }

        #endregion

        #region BSO->ACProperty

        #region DeliveryNote

        protected IQueryable<DeliveryNote> _AccessPrimary_NavSearchExecuting(IQueryable<DeliveryNote> result)
        {
            ObjectQuery<DeliveryNote> query = result as ObjectQuery<DeliveryNote>;
            if (query != null)
            {
                query.Include(c => c.MDDelivNoteState);
                query.Include(c => c.DeliveryNotePos_DeliveryNote);
                query.Include("DeliveryNotePos_DeliveryNote");
                query.Include("DeliveryNotePos_DeliveryNote.InOrderPos");
                query.Include("DeliveryNotePos_DeliveryNote.InOrderPos.Material");
            }
            return result;
        }


        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<DeliveryNote> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(690, DeliveryNote.ClassName)]
        public ACAccessNav<DeliveryNote> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);

                    if (navACQueryDefinition != null)
                    {
                        ACSortItem sortItem = navACQueryDefinition.ACSortColumns.Where(c => c.ACIdentifier == "DeliveryNoteNo").FirstOrDefault();
                        if (sortItem != null && sortItem.IsConfiguration)
                            sortItem.SortDirection = Global.SortDirections.descending;
                        if (navACQueryDefinition.TakeCount == 0)
                            navACQueryDefinition.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                    }

                    bool rebuildACQueryDef = false;
                    short filterDelivType = (short)GlobalApp.DeliveryNoteType.Receipt;
                    if (!navACQueryDefinition.ACFilterColumns.Any())
                    {
                        rebuildACQueryDef = true;
                    }
                    else
                    {
                        int countFoundCorrect = 0;
                        foreach (ACFilterItem filterItem in navACQueryDefinition.ACFilterColumns)
                        {
                            if (filterItem.PropertyName == "DeliveryNoteTypeIndex")
                            {
                                if (filterItem.SearchWord == filterDelivType.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal)
                                    countFoundCorrect++;
                            }
                        }
                        if (countFoundCorrect < 1)
                            rebuildACQueryDef = true;
                    }
                    if (rebuildACQueryDef)
                    {
                        navACQueryDefinition.ClearFilter(true);
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "DeliveryNoteNo", Global.LogicalOperators.contains, Global.Operators.and, "", true));
                        navACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "DeliveryNoteTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, filterDelivType.ToString(), true));
                        navACQueryDefinition.SaveConfig(true);
                    }

                    _AccessPrimary = navACQueryDefinition.NewAccessNav<DeliveryNote>(DeliveryNote.ClassName, this);
                    _AccessPrimary.NavSearchExecuting += _AccessPrimary_NavSearchExecuting;
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the current delivery note.
        /// </summary>
        /// <value>The current delivery note.</value>
        [ACPropertyCurrent(600, DeliveryNote.ClassName)]
        public DeliveryNote CurrentDeliveryNote
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Current = value;
                OnPropertyChanged("CurrentDeliveryNote");
                OnPropertyChanged("DeliveryNotePosList");
                RatingOnPropertyChanged();
                RefreshInOrderPosList();
                SelectedDeliveryNotePos = DeliveryNotePosList != null && DeliveryNotePosList.Any() ? DeliveryNotePosList.FirstOrDefault() : null;
            }
        }

        /// <summary>
        /// Gets the delivery note list.
        /// </summary>
        /// <value>The delivery note list.</value>
        [ACPropertyList(601, DeliveryNote.ClassName)]
        public IEnumerable<DeliveryNote> DeliveryNoteList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }

        /// <summary>
        /// Gets or sets the selected delivery note.
        /// </summary>
        /// <value>The selected delivery note.</value>
        [ACPropertySelected(602, DeliveryNote.ClassName)]
        public DeliveryNote SelectedDeliveryNote
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedDeliveryNote");
            }
        }

        #region DeliveryNote -> Rating

        [ACPropertyInfo(603, DeliveryNote.ClassName)]
        public decimal DeliveryCompanyRating
        {
            get
            {
                decimal rating = 0;
                if (CurrentDeliveryNote != null && CurrentDeliveryNote.DeliveryCompanyAddressID != null)
                {
                    Rating item = CurrentDeliveryNote.Rating_DeliveryNote.Where(x => x.CompanyID == CurrentDeliveryNote.DeliveryCompanyAddress.CompanyID).FirstOrDefault();
                    if (item != null)
                        rating = item.Score;
                }
                return rating;
            }
            set
            {
                if (CurrentDeliveryNote != null && CurrentDeliveryNote.DeliveryCompanyAddressID != null)
                {
                    Rating item = CurrentDeliveryNote.Rating_DeliveryNote.Where(x => x.CompanyID == CurrentDeliveryNote.DeliveryCompanyAddress.CompanyID).FirstOrDefault();
                    if (item == null)
                    {
                        item = Rating.NewACObject(DatabaseApp, null);
                        item.CompanyID = CurrentDeliveryNote.DeliveryCompanyAddress.Company.CompanyID;
                        item.DeliveryNoteID = CurrentDeliveryNote.DeliveryNoteID;
                        DatabaseApp.Rating.AddObject(item);
                        CurrentDeliveryNote.Rating_DeliveryNote.Add(item);
                    }
                    else if (value == 0)
                    {
                        DatabaseApp.Rating.DeleteObject(item);
                        CurrentDeliveryNote.Rating_DeliveryNote.Remove(item);
                    }
                    item.Score = value;
                    if (value > 0)
                        OnPropertyChanged("IsEnabledShowDeliveryCompanyRatingComplaint");
                }
            }
        }

        [ACPropertyInfo(604, DeliveryNote.ClassName)]
        public bool IsEnabledDeliveryCompanyRating
        {
            get
            {
                return CurrentDeliveryNote != null && CurrentDeliveryNote.DeliveryCompanyAddressID != null;
            }
        }

        [ACPropertyInfo(605, DeliveryNote.ClassName)]
        public decimal ShipperCompanyRating
        {
            get
            {
                decimal rating = 0;
                if (CurrentDeliveryNote != null && CurrentDeliveryNote.ShipperCompanyAddress != null && CurrentDeliveryNote.ShipperCompanyAddressID != null)
                {
                    Rating item = CurrentDeliveryNote.Rating_DeliveryNote.Where(x => x.CompanyID == CurrentDeliveryNote.ShipperCompanyAddress.CompanyID).FirstOrDefault();
                    if (item != null)
                        rating = item.Score;
                }
                return rating;
            }
            set
            {
                if (CurrentDeliveryNote != null && CurrentDeliveryNote.ShipperCompanyAddressID != null)
                {
                    Rating item = CurrentDeliveryNote.Rating_DeliveryNote.Where(x => x.CompanyID == CurrentDeliveryNote.ShipperCompanyAddress.CompanyID).FirstOrDefault();
                    if (item == null)
                    {
                        item = Rating.NewACObject(DatabaseApp, null);
                        item.CompanyID = CurrentDeliveryNote.ShipperCompanyAddress.Company.CompanyID;
                        item.DeliveryNoteID = CurrentDeliveryNote.DeliveryNoteID;
                        DatabaseApp.Rating.AddObject(item);
                        CurrentDeliveryNote.Rating_DeliveryNote.Add(item);
                    }
                    else if (value == 0)
                    {
                        DatabaseApp.Rating.DeleteObject(item);
                        CurrentDeliveryNote.Rating_DeliveryNote.Remove(item);
                    }
                    item.Score = value;
                    if (value > 0)
                        OnPropertyChanged("IsEnabledShowShipperCompanyRatingComplaint");
                }
            }
        }

        [ACPropertyInfo(606, DeliveryNote.ClassName)]
        public bool IsEnabledShipperCompanyRating
        {
            get
            {
                return CurrentDeliveryNote != null && CurrentDeliveryNote.ShipperCompanyAddressID != null;
            }
        }

        [ACPropertyInfo(607, DeliveryNote.ClassName)]
        public decimal FinisherCompanyRating
        {
            get
            {
                decimal rating = 0;
                if (CurrentDeliveryNote != null && CurrentDeliveryNote.Delivery2CompanyAddress != null && CurrentDeliveryNote.Delivery2CompanyAddressID != null)
                {
                    Rating item = CurrentDeliveryNote.Rating_DeliveryNote.Where(x => x.CompanyID == CurrentDeliveryNote.Delivery2CompanyAddress.CompanyID).FirstOrDefault();
                    if (item != null)
                        rating = item.Score;
                }
                return rating;
            }
            set
            {
                if (CurrentDeliveryNote != null && CurrentDeliveryNote.Delivery2CompanyAddressID != null)
                {
                    Rating item = CurrentDeliveryNote.Rating_DeliveryNote.Where(x => x.CompanyID == CurrentDeliveryNote.Delivery2CompanyAddress.CompanyID).FirstOrDefault();
                    if (item == null)
                    {
                        item = Rating.NewACObject(DatabaseApp, null);
                        item.CompanyID = CurrentDeliveryNote.Delivery2CompanyAddress.Company.CompanyID;
                        item.DeliveryNoteID = CurrentDeliveryNote.DeliveryNoteID;
                        DatabaseApp.Rating.AddObject(item);
                        CurrentDeliveryNote.Rating_DeliveryNote.Add(item);
                    }
                    else if (value == 0)
                    {
                        DatabaseApp.Rating.DeleteObject(item);
                        CurrentDeliveryNote.Rating_DeliveryNote.Remove(item);
                    }
                    item.Score = value;
                    if (value > 0)
                        OnPropertyChanged("IsEnabledFinisherCompanyRating");
                }
            }
        }

        [ACPropertyInfo(608, DeliveryNote.ClassName)]
        public bool IsEnabledFinisherCompanyRating
        {
            get
            {
                return CurrentDeliveryNote != null && CurrentDeliveryNote.Delivery2CompanyAddressID != null;
            }
        }

        private void RatingOnPropertyChanged()
        {
            OnPropertyChanged("DeliveryCompanyRating");
            OnPropertyChanged("ShipperCompanyRating");
            OnPropertyChanged("FinisherCompanyRating");
            OnPropertyChanged("IsEnabledDeliveryCompanyRating");
            OnPropertyChanged("IsEnabledShipperCompanyRating");
            OnPropertyChanged("IsEnabledFinisherCompanyRating");
            OnPropertyChanged("IsEnabledShowDeliveryCompanyRatingComplaint");
            OnPropertyChanged("IsEnabledShowShipperCompanyRatingComplaint");
            OnPropertyChanged("IsEnabledShowFinisherCompanyRatingComplaint");
        }

        #region DeliveryNote -> Rating -> Rating Complaint

        [ACMethodInfo(DeliveryNote.ClassName, "en{'Rating Complaint'}de{'Beanstandung'}", 999)]
        public void ShowDeliveryCompanyRatingComplaint()
        {
            Rating item = CurrentDeliveryNote.Rating_DeliveryNote.Where(x => x.CompanyID == CurrentDeliveryNote.DeliveryCompanyAddress.CompanyID).FirstOrDefault();
            ShowRatingComplaintBSO(item);
        }

        [ACMethodInfo(DeliveryNote.ClassName, "en{'Rating Complaint'}de{'Beanstandung'}", 999)]
        public void ShowShipperCompanyRatingComplaint()
        {
            Rating item = CurrentDeliveryNote.Rating_DeliveryNote.Where(x => x.CompanyID == CurrentDeliveryNote.ShipperCompanyAddress.CompanyID).FirstOrDefault();
            ShowRatingComplaintBSO(item);
        }

        [ACMethodInfo(DeliveryNote.ClassName, "en{'Rating Complaint'}de{'Beanstandung'}", 999)]
        public void ShowFinisherCompanyRatingComplaint()
        {
            Rating item = CurrentDeliveryNote.Rating_DeliveryNote.Where(x => x.CompanyID == CurrentDeliveryNote.Delivery2CompanyAddress.CompanyID).FirstOrDefault();
            ShowRatingComplaintBSO(item);
        }

        private void ShowRatingComplaintBSO(Rating item)
        {
            ACComponent childBSO = ACUrlCommand("?BSORatingComplaint_Child") as ACComponent;
            if (childBSO == null)
            {
                childBSO = StartComponent("BSORatingComplaint_Child", null, new object[] { }) as ACComponent;
            }
            if (childBSO != null)
            {
                childBSO.ACUrlCommand("!OpenAsModal", item);
            }
        }

        public bool IsEnabledShowDeliveryCompanyRatingComplaint()
        {
            return DeliveryCompanyRating > 0;
        }
        public bool IsEnabledShowShipperCompanyRatingComplaint()
        {
            return ShipperCompanyRating > 0;
        }

        public bool IsEnabledShowFinisherCompanyRating()
        {
            return FinisherCompanyRating > 0;
        }

        #endregion
        #endregion

        #endregion

        #region DeliveryNotePos

        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccess<DeliveryNotePos> _AccessDeliveryNotePos;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(691, "DeliveryNotePos")]
        public ACAccess<DeliveryNotePos> AccessDeliveryNotePos
        {
            get
            {
                if (_AccessDeliveryNotePos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + DeliveryNotePos.ClassName) as ACQueryDefinition;
                    _AccessDeliveryNotePos = acQueryDefinition.NewAccess<DeliveryNotePos>("DeliveryNotePos", this);
                }
                return _AccessDeliveryNotePos;
            }
        }

        /// <summary>
        /// The _ current delivery note pos
        /// </summary>
        DeliveryNotePos _CurrentDeliveryNotePos;
        /// <summary>
        /// Gets or sets the current delivery note pos.
        /// </summary>
        /// <value>The current delivery note pos.</value>
        [ACPropertyCurrent(610, "DeliveryNotePos")]
        public DeliveryNotePos CurrentDeliveryNotePos
        {
            get
            {
                return _CurrentDeliveryNotePos;
            }
            set
            {
                _CurrentDeliveryNotePos = value;
                OnPropertyChanged("CurrentDeliveryNotePos");
                OnPropertyChanged("FacilityPreBookingList");
                if (FacilityPreBookingList != null && FacilityPreBookingList.Any())
                    SelectedFacilityPreBooking = FacilityPreBookingList.First();
                RefreshFilterFacilityAccess();
                OnPropertyChanged("BookingFacilityList");
                OnPropertyChanged("FacilityBookingList");
            }
        }

        /// <summary>
        /// Gets the delivery note pos list.
        /// </summary>
        /// <value>The delivery note pos list.</value>
        [ACPropertyList(611, "DeliveryNotePos")]
        public IEnumerable<DeliveryNotePos> DeliveryNotePosList
        {
            get
            {
                if (CurrentDeliveryNote == null)
                    return null;

                return CurrentDeliveryNote.DeliveryNotePos_DeliveryNote.ToList();
            }
        }

        /// <summary>
        /// The _ selected delivery note pos
        /// </summary>
        DeliveryNotePos _SelectedDeliveryNotePos;
        /// <summary>
        /// Gets or sets the selected delivery note pos.
        /// </summary>
        /// <value>The selected delivery note pos.</value>
        [ACPropertySelected(612, "DeliveryNotePos")]
        public DeliveryNotePos SelectedDeliveryNotePos
        {
            get
            {
                return _SelectedDeliveryNotePos;
            }
            set
            {
                if (_SelectedDeliveryNotePos != value)
                {
                    _SelectedDeliveryNotePos = value;
                    if (
                        _SelectedDeliveryNotePos != null &&
                        _SelectedDeliveryNotePos.InOrderPos != null
                        && (_SelectedDeliveryNotePos as EntityObject).EntityState == System.Data.EntityState.Unchanged)
                        _SelectedDeliveryNotePos.InOrderPos.LabOrder_InOrderPos.AutoLoad(this.DatabaseApp);
                    OnPropertyChanged("SelectedDeliveryNotePos");
                }
                CurrentDeliveryNotePos = value;
            }
        }

        #endregion

        #region Open Order lines
        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<InOrderPos> _AccessInOrderPos;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(692, InOrderPos.ClassName)]
        public ACAccessNav<InOrderPos> AccessInOrderPos
        {
            get
            {
                if (_AccessInOrderPos == null && ACType != null)
                {
                    ACQueryDefinition acQueryDef = Root.Queries.CreateQuery(null, Const.QueryPrefix + "InOrderPosOpen", ACType.ACIdentifier);
                    if (acQueryDef != null)
                    {
                        acQueryDef.CheckAndReplaceColumnsIfDifferent(AccessInOrderPosDefaultFilter, AccessInOrderPosDefaultSort, true, true);
                        if (acQueryDef.TakeCount == 0)
                            acQueryDef.TakeCount = ACQueryDefinition.C_DefaultTakeCount;
                    }

                    _AccessInOrderPos = acQueryDef.NewAccessNav<InOrderPos>(InOrderPos.ClassName, this);
                    _AccessInOrderPos.AutoSaveOnNavigation = false;
                    _AccessInOrderPos.NavSearch(DatabaseApp);
                }
                return _AccessInOrderPos;
            }
        }

        protected virtual List<ACFilterItem> AccessInOrderPosDefaultFilter
        {
            get
            {
                return new List<ACFilterItem>()
                {
                    new ACFilterItem(Global.FilterTypes.filter, "MDDelivPosState\\MDDelivPosStateIndex", Global.LogicalOperators.lessThan, Global.Operators.and, ((short) MDDelivPosState.DelivPosStates.CompletelyAssigned).ToString(), true),
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                        new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.or, null, true),
                            new ACFilterItem(Global.FilterTypes.filter, "InOrderPos1_ParentInOrderPos", Global.LogicalOperators.isNull, Global.Operators.and, "", true),
                            new ACFilterItem(Global.FilterTypes.filter, InOrder.ClassName + "\\" + MDInOrderType.ClassName + "\\OrderTypeIndex", Global.LogicalOperators.notEqual, Global.Operators.and, ((short) GlobalApp.OrderTypes.Contract).ToString(), true),
                        new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.or, null, true),
                        new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.or, null, true),
                            new ACFilterItem(Global.FilterTypes.filter, "InOrderPos1_ParentInOrderPos", Global.LogicalOperators.isNotNull, Global.Operators.and, "", true),
                            new ACFilterItem(Global.FilterTypes.filter, "InOrderPos1_ParentInOrderPos\\" + InOrder.ClassName + "\\" + MDInOrderType.ClassName + "\\OrderTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, ((short) GlobalApp.OrderTypes.Contract).ToString(), true),
                        new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.or, null, true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, _CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CTransportModeProperty, Global.LogicalOperators.equal, Global.Operators.and, "", true),
                    new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
                    new ACFilterItem(Global.FilterTypes.filter, _CMaterialNoProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.filter, _CMaterialNameProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
                    new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, true),
            };
            }
        }

        protected virtual List<ACSortItem> AccessInOrderPosDefaultSort
        {
            get
            {
                return new List<ACSortItem>()
                {
                    new ACSortItem("TargetDeliveryDate", Global.SortDirections.ascending, true),
                    new ACSortItem("Material\\MaterialNo", Global.SortDirections.ascending, true),
                };
            }
        }

        /// <summary>
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(613, InOrderPos.ClassName)]
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
                if (AccessInOrderPos != null)
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
        [ACPropertyList(614, InOrderPos.ClassName)]
        public IEnumerable<InOrderPos> InOrderPosList
        {
            get
            {
                if (AccessInOrderPos == null)
                    return null;
                if (CurrentDeliveryNote != null)
                {
                    IEnumerable<InOrderPos> addedPositions = CurrentDeliveryNote.DeliveryNotePos_DeliveryNote.Where(c => c.EntityState == System.Data.EntityState.Added
                        && c.InOrderPos != null
                        && c.InOrderPos.InOrderPos1_ParentInOrderPos != null
                        && c.InOrderPos.InOrderPos1_ParentInOrderPos.MDDelivPosState == StateCompletelyAssigned
                        ).Select(c => c.InOrderPos.InOrderPos1_ParentInOrderPos);
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
        [ACPropertySelected(615, InOrderPos.ClassName)]
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
                if (AccessInOrderPos != null)
                    AccessInOrderPos.Selected = value;
                OnPropertyChanged("SelectedInOrderPos");
                CurrentInOrderPos = value;
            }
        }


        #region Filter
        public const string _CMaterialNoProperty = Material.ClassName + "\\MaterialNo";
        public const string _CMaterialNameProperty = Material.ClassName + "\\MaterialName1";
        [ACPropertyInfo(713, "Filter", "en{'Material'}de{'Material'}")]
        public string FilterMaterial
        {
            get
            {
                return AccessInOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CMaterialNoProperty);
            }
            set
            {
                string tmp = AccessInOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CMaterialNoProperty);
                if (tmp != value)
                {
                    AccessInOrderPos.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNoProperty, value);
                    AccessInOrderPos.NavACQueryDefinition.SetSearchValue<string>(_CMaterialNameProperty, value);
                    OnPropertyChanged("FilterMaterial");
                }
            }
        }


        [ACPropertyList(616, "TransportMode")]
        public IEnumerable<MDTransportMode> MDTransportModeList
        {
            get
            {
                return DatabaseApp.MDTransportMode;
            }
        }


        public const string _CTransportModeProperty = "MDTransportMode\\MDKey";
        [ACPropertySelected(617, "TransportMode", "en{'Filter mode of transport'}de{'Filter Transportart'}")]
        public MDTransportMode FilterTransportMode
        {
            get
            {
                string tmp = AccessInOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CTransportModeProperty, Global.LogicalOperators.equal);
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return DatabaseApp.MDTransportMode.Where(c => c.MDKey == tmp).FirstOrDefault();
            }

            set
            {
                string tmp = AccessInOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CTransportModeProperty, Global.LogicalOperators.equal);
                if (value == null)
                {
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        AccessInOrderPos.NavACQueryDefinition.SetSearchValue(_CTransportModeProperty, Global.LogicalOperators.equal, "");
                        OnPropertyChanged("FilterTransportMode");
                    }
                }
                else
                {
                    if (tmp != value.MDKey)
                    {
                        AccessInOrderPos.NavACQueryDefinition.SetSearchValue(_CTransportModeProperty, Global.LogicalOperators.equal, value.MDKey);
                        OnPropertyChanged("FilterTransportMode");
                    }
                }
            }
        }


        public const string _CTargetDeliveryDateProperty = "TargetDeliveryDate";

        [ACPropertyInfo(618, "", "en{'Deliv.date from'}de{'Lieferdatum von'}")]
        public DateTime? FilterDelivDateFrom
        {
            get
            {
                string tmp = AccessInOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessInOrderPos.NavACQueryDefinition.GetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual);
            }
            set
            {
                string tmp = AccessInOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                {
                    if (value.HasValue)
                    {
                        AccessInOrderPos.NavACQueryDefinition.SetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual, value.Value);
                        OnPropertyChanged("FilterDelivDateFrom");
                    }
                }
                else
                {
                    if (value.HasValue)
                    {
                        DateTime tmpdt = AccessInOrderPos.NavACQueryDefinition.GetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual);
                        if (tmpdt != value.Value)
                        {
                            AccessInOrderPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual, value.Value);
                            OnPropertyChanged("FilterDelivDateFrom");
                        }
                    }
                    else
                    {
                        AccessInOrderPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.greaterThanOrEqual, "");
                        OnPropertyChanged("FilterDelivDateFrom");
                    }
                }
            }
        }

        [ACPropertyInfo(619, "", "en{'Deliv.date to'}de{'Lieferdatum bis'}")]
        public DateTime? FilterDelivDateTo
        {
            get
            {
                string tmp = AccessInOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                    return null;
                return AccessInOrderPos.NavACQueryDefinition.GetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual);
            }
            set
            {
                string tmp = AccessInOrderPos.NavACQueryDefinition.GetSearchValue<string>(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual);
                if (String.IsNullOrEmpty(tmp))
                {
                    if (value.HasValue)
                    {
                        AccessInOrderPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual, value.Value);
                        OnPropertyChanged("FilterDelivDateTo");
                    }
                }
                else
                {
                    if (value.HasValue)
                    {
                        DateTime tmpdt = AccessInOrderPos.NavACQueryDefinition.GetSearchValue<DateTime>(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual);
                        if (tmpdt != value)
                        {
                            AccessInOrderPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual, value.Value);
                            OnPropertyChanged("FilterDelivDateTo");
                        }
                    }
                    else
                    {
                        AccessInOrderPos.NavACQueryDefinition.SetSearchValue(_CTargetDeliveryDateProperty, Global.LogicalOperators.lessThanOrEqual, "");
                        OnPropertyChanged("FilterDelivDateTo");
                    }
                }
            }
        }
        #endregion

        #endregion

        #region InOrderPos from Picking
        InOrderPos _CurrentInOrderPosFromPicking;
        /// <summary>
        /// Gets or sets the current in order pos.
        /// </summary>
        /// <value>The current in order pos.</value>
        [ACPropertyCurrent(620, "InOrderPosFromPicking")]
        public InOrderPos CurrentInOrderPosFromPicking
        {
            get
            {
                return _CurrentInOrderPosFromPicking;
            }
            set
            {
                _CurrentInOrderPosFromPicking = value;
                OnPropertyChanged("CurrentInOrderPosFromPicking");
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(622, "InOrderPosFromPicking")]
        public IEnumerable<InOrderPos> InOrderPosFromPickingList
        {
            get
            {
                var query = DatabaseApp.PickingPos.Where(c => (c.Picking.PickingType == GlobalApp.PickingType.ReceiptVehicle || c.Picking.PickingType == GlobalApp.PickingType.ReceiptVehicle)
                                                          && c.InOrderPos != null
                                                          && !c.InOrderPos.InOrderPos1_ParentInOrderPos.DeliveryNotePos_InOrderPos.Any())
                                             .Select(c => c.InOrderPos.InOrderPos1_ParentInOrderPos)
                                             .ToList().Distinct(); // Distinct auf Clientseite ausführen lassen (nach ToList), weil SQL-Server Abfrage nicht auswerten kann wenn Distinct vorher aufgerufen wird (= Serverseitig ausgeführt werden soll)
                if (query.Any())
                {
                    if (_UnSavedAssignedPickingInOrderPos.Count <= 0)
                        return query;
                    else
                        return query.Except(_UnSavedAssignedPickingInOrderPos);
                }
                else
                    return _UnSavedAssignedPickingInOrderPos;
            }
        }

        InOrderPos _SelectedInOrderPosFromPicking;
        /// <summary>
        /// Gets or sets the selected in order pos.
        /// </summary>
        /// <value>The selected in order pos.</value>
        [ACPropertySelected(623, "InOrderPosFromPicking")]
        public InOrderPos SelectedInOrderPosFromPicking
        {
            get
            {
                return _SelectedInOrderPosFromPicking;
            }
            set
            {
                _SelectedInOrderPosFromPicking = value;
                OnPropertyChanged("SelectedInOrderPosFromPicking");
            }
        }
        #endregion

        #region FacilityPreBooking
        FacilityPreBooking _CurrentFacilityPreBooking;
        [ACPropertyCurrent(624, "FacilityPreBooking")]
        public FacilityPreBooking CurrentFacilityPreBooking
        {
            get
            {
                return _CurrentFacilityPreBooking;
            }
            set
            {
                _CurrentFacilityPreBooking = value;
                OnPropertyChanged("CurrentFacilityPreBooking");
                OnPropertyChanged("CurrentACMethodBooking");
                OnPropertyChanged("BookableFacilityLots");
                RefreshFilterFacilityAccess();
                OnPropertyChanged("BookingFacilityList");
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(625, "FacilityPreBooking")]
        public IEnumerable<FacilityPreBooking> FacilityPreBookingList
        {
            get
            {
                if ((CurrentDeliveryNotePos == null) || (CurrentDeliveryNotePos.InOrderPos == null))
                    return null;
                return CurrentDeliveryNotePos.InOrderPos.FacilityPreBooking_InOrderPos.ToList();
            }
        }

        FacilityPreBooking _SelectedFacilityPreBooking;
        [ACPropertySelected(626, "FacilityPreBooking")]
        public FacilityPreBooking SelectedFacilityPreBooking
        {
            get
            {
                return _SelectedFacilityPreBooking;
            }
            set
            {
                _SelectedFacilityPreBooking = value;
                OnPropertyChanged("SelectedFacilityPreBooking");
                CurrentFacilityPreBooking = value;
            }
        }

        ACMethodBooking _CurrentACMethodBookingDummy = null; // Dummy-Parameter notwendig, damit Bindung an Oberfläche erfolgen kann, da abgeleitete Klasse
        [ACPropertyInfo(627, "", "en{'Posting parameter'}de{'Buchungsparameter'}")]
        public ACMethodBooking CurrentACMethodBooking
        {
            get
            {
                if (CurrentFacilityPreBooking == null)
                {
                    if (_CurrentACMethodBookingDummy != null)
                        return _CurrentACMethodBookingDummy;
                    if (InDeliveryNoteManager != null)
                    {
                        ACMethodBooking acMethodClone = InDeliveryNoteManager.BookParamInwardMovementClone(this.ACFacilityManager, this.DatabaseApp);
                        if (acMethodClone != null)
                            _CurrentACMethodBookingDummy = acMethodClone.Clone() as ACMethodBooking;
                        return _CurrentACMethodBookingDummy;
                    }
                }
                _CurrentACMethodBookingDummy = null;
                return CurrentFacilityPreBooking.ACMethodBooking as ACMethodBooking;
            }
            set
            {
                if (CurrentFacilityPreBooking != null)
                    CurrentFacilityPreBooking.ACMethodBooking = value;
                else
                    _CurrentACMethodBookingDummy = null;
                OnPropertyChanged("CurrentACMethodBooking");
            }
        }

        private bool _BookingFilterMaterial = true;
        [ACPropertyInfo(628, "", "en{'Only show bins with material'}de{'Zeige Lagerpätze mit Material'}")]
        public bool BookingFilterMaterial
        {
            get
            {
                return _BookingFilterMaterial;
            }
            set
            {
                _BookingFilterMaterial = value;
                OnPropertyChanged("BookingFilterMaterial");
                RefreshFilterFacilityAccess();
                OnPropertyChanged("BookingFacilityList");
            }
        }


        /// <summary>
        /// The _ access delivery note pos
        /// </summary>
        ACAccessNav<Facility> _AccessBookingFacility;
        /// <summary>
        /// Gets the access delivery note pos.
        /// </summary>
        /// <value>The access delivery note pos.</value>
        [ACPropertyAccess(629, "BookingFacility")]
        public ACAccessNav<Facility> AccessBookingFacility
        {
            get
            {
                if (_AccessBookingFacility == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQuery(null, Const.QueryPrefix + "BookingFacility", ACType.ACIdentifier);
                    _AccessBookingFacility = navACQueryDefinition.NewAccessNav<Facility>("BookingFacility", this);
                    _AccessBookingFacility.AutoSaveOnNavigation = false;
                    RefreshFilterFacilityAccess();
                }
                return _AccessBookingFacility;
            }
        }


        [ACPropertyList(630, "BookingFacility")]
        public IList<Facility> BookingFacilityList
        {
            get
            {
                if (AccessBookingFacility == null)
                    return null;
                return AccessBookingFacility.NavList;
            }
        }

        private void RefreshFilterFacilityAccess()
        {
            if (AccessBookingFacility == null || CurrentFacilityPreBooking == null || FacilityPreBookingList == null || !FacilityPreBookingList.Any())
                return;
            bool rebuildACQueryDef = false;
            short fcTypeContainer = (short)MDFacilityType.FacilityTypes.StorageBinContainer;
            short fcTypeBin = (short)MDFacilityType.FacilityTypes.StorageBin;
            if (AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Count <= 0)
            {
                rebuildACQueryDef = true;
            }
            else
            {
                int countFoundCorrect = 0;
                foreach (ACFilterItem filterItem in AccessBookingFacility.NavACQueryDefinition.ACFilterColumns)
                {
                    if (filterItem.FilterType != Global.FilterTypes.filter)
                        continue;
                    if (filterItem.PropertyName == "MDFacilityType\\MDFacilityTypeIndex")
                    {
                        if ((BookingFilterMaterial && filterItem.SearchWord == fcTypeContainer.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal)
                            || (!BookingFilterMaterial && filterItem.SearchWord == fcTypeBin.ToString() && filterItem.LogicalOperator == Global.LogicalOperators.equal))
                            countFoundCorrect++;
                    }
                    else if (BookingFilterMaterial && filterItem.PropertyName == "Material\\MaterialNo" && CurrentDeliveryNotePos != null && CurrentDeliveryNotePos.Material != null)
                    {
                        if (filterItem.SearchWord == CurrentDeliveryNotePos.Material.MaterialNo)
                            countFoundCorrect++;
                    }
                }
                if (BookingFilterMaterial && countFoundCorrect < 2)
                    rebuildACQueryDef = true;
                else if (!BookingFilterMaterial && countFoundCorrect < 1)
                    rebuildACQueryDef = true;
            }
            if (rebuildACQueryDef)
            {
                AccessBookingFacility.NavACQueryDefinition.ClearFilter(true);
                if (BookingFilterMaterial)
                {
                    AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, fcTypeContainer.ToString(), true));
                    if (CurrentDeliveryNotePos != null && CurrentDeliveryNotePos.Material != null)
                    {
                        AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisOpen, null, Global.LogicalOperators.none, Global.Operators.and, null, false));
                        AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "Material\\MaterialNo", Global.LogicalOperators.equal, Global.Operators.or, CurrentDeliveryNotePos.Material.MaterialNo, false));
                        if (CurrentDeliveryNotePos.Material.Material1_ProductionMaterial != null)
                            AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "Material\\MaterialNo", Global.LogicalOperators.equal, Global.Operators.or, CurrentDeliveryNotePos.Material.Material1_ProductionMaterial.MaterialNo, false));
                        AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.parenthesisClose, null, Global.LogicalOperators.none, Global.Operators.and, null, false));
                    }
                }
                else
                {
                    AccessBookingFacility.NavACQueryDefinition.ACFilterColumns.Add(new ACFilterItem(Global.FilterTypes.filter, "MDFacilityType\\MDFacilityTypeIndex", Global.LogicalOperators.equal, Global.Operators.and, fcTypeBin.ToString(), true));
                }
                AccessBookingFacility.NavACQueryDefinition.SaveConfig(false);
            }
            AccessBookingFacility.NavSearch(this.DatabaseApp);
        }


        [ACPropertyList(631, "FacilityLots")]
        public IEnumerable<FacilityLot> BookableFacilityLots
        {
            get
            {
                if ((CurrentDeliveryNotePos == null) || (CurrentDeliveryNotePos.InOrderPos == null) || (CurrentACMethodBooking == null))
                    return null;

                List<FacilityLot> bookableFacilityLots = null;
                if (FacilityPreBookingList != null && FacilityPreBookingList.Any())
                    bookableFacilityLots = FacilityPreBookingList.Where(c => c.InwardFacilityLot != null).Select(c => c.InwardFacilityLot).Distinct().ToList();

                if (FacilityBookingList.Any())
                {
                    var query2 = FacilityBookingList.Where(c => c.InwardFacilityLot != null).Select(c => c.InwardFacilityLot).Distinct();
                    if (bookableFacilityLots == null)
                        bookableFacilityLots = query2.ToList();
                    else
                    {
                        var query3 = bookableFacilityLots.Union(query2);
                        if ((query3 != null) && (query3.Any()))
                            bookableFacilityLots = query3.ToList();
                    }
                }
                var queryFromDB = DatabaseApp.FacilityLot.Where(c => c.Material != null && c.MaterialID == CurrentDeliveryNotePos.InOrderPos.MaterialID);
                if (bookableFacilityLots == null)
                    bookableFacilityLots = queryFromDB.ToList();
                else
                {
                    var query3 = bookableFacilityLots.Union(queryFromDB);
                    if ((query3 != null) && (query3.Any()))
                        bookableFacilityLots = query3.ToList();
                }
                return bookableFacilityLots;
            }
        }
        #endregion

        #region FacilityBooking
        FacilityBooking _CurrentFacilityBooking;
        [ACPropertyCurrent(632, FacilityBooking.ClassName)]
        public FacilityBooking CurrentFacilityBooking
        {
            get
            {
                return _CurrentFacilityBooking;
            }
            set
            {
                _CurrentFacilityBooking = value;
                OnPropertyChanged("CurrentFacilityBooking");
            }
        }

        /// <summary>
        /// Gets the in order pos list.
        /// </summary>
        /// <value>The in order pos list.</value>
        [ACPropertyList(633, FacilityBooking.ClassName)]
        public IEnumerable<FacilityBooking> FacilityBookingList
        {
            get
            {
                if ((CurrentDeliveryNotePos == null) || (CurrentDeliveryNotePos.InOrderPos == null))
                    return null;
                return CurrentDeliveryNotePos.InOrderPos.FacilityBooking_InOrderPos.OrderBy(c => c.FacilityBookingNo).ToList();
            }
        }

        FacilityBooking _SelectedFacilityBooking;
        [ACPropertySelected(634, FacilityBooking.ClassName)]
        public FacilityBooking SelectedFacilityBooking
        {
            get
            {
                return _SelectedFacilityBooking;
            }
            set
            {
                _SelectedFacilityBooking = value;
                OnPropertyChanged("SelectedFacilityBooking");
                CurrentFacilityBooking = value;
                OnPropertyChanged("FacilityBookingChargeList");
            }
        }
        #endregion

        #region FacilityBookingCharge

        [ACPropertyList(635, "FacilityBookingCharge")]
        public IEnumerable<FacilityBookingCharge> FacilityBookingChargeList
        {
            get
            {
                if (SelectedFacilityBooking != null)
                    return CurrentFacilityBooking.FacilityBookingCharge_FacilityBooking.ToList();
                return null;
            }
        }

        private FacilityBookingCharge _SelectedFacilityBookingCharge;
        [ACPropertySelected(636, "FacilityBookingCharge")]
        public FacilityBookingCharge SelectedFacilityBookingCharge
        {
            get
            {
                return _SelectedFacilityBookingCharge;
            }
            set
            {
                _SelectedFacilityBookingCharge = value;
            }
        }

        #endregion

        #region Local Properties

        protected ACMethodBooking BookParamInwardMovementClone
        {
            get
            {
                return InDeliveryNoteManager.BookParamInwardMovementClone(ACFacilityManager, this.DatabaseApp);
            }
        }

        protected ACMethodBooking BookParamInCancelClone
        {
            get
            {
                return InDeliveryNoteManager.BookParamInCancelClone(ACFacilityManager, this.DatabaseApp);
            }
        }

        protected List<InOrderPos> _UnSavedUnAssignedInOrderPos = new List<InOrderPos>();
        protected List<InOrderPos> _UnSavedAssignedPickingInOrderPos = new List<InOrderPos>();

        Nullable<double> _PartialQuantity;
        [ACPropertyInfo(637, "", "en{'Partial Quantity'}de{'Teilmenge'}")]
        public Nullable<double> PartialQuantity
        {
            get
            {
                return _PartialQuantity;
            }
            set
            {
                _PartialQuantity = value;
                OnPropertyChanged("PartialQuantity");
            }
        }

        private string _BookingFacilityLotNo;
        public string BookingFacilityLotNo
        {
            get
            {
                return _BookingFacilityLotNo;
            }
            set
            {
                _BookingFacilityLotNo = value;
                OnPropertyChanged("BookingFacilityLotNo");
            }
        }

        #endregion

        #region Message

        private MsgWithDetails _BSOMsg = new MsgWithDetails();
        [ACPropertyInfo(638, "Message")]
        public MsgWithDetails BSOMsg
        {
            get
            {
                return _BSOMsg;
            }
            set
            {
                _BSOMsg = value;
                OnPropertyChanged("BSOMsg");
            }
        }

        #endregion

        #endregion

        #region BSO->ACMethod

        #region BSO->ACMethod->ControlMode
        /// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        /// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        /// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        /// <returns>ControlModesInfo</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            switch (vbControl.VBContent)
            {
                case "CurrentACMethodBooking\\InwardFacilityLot":
                    {
                        if (CurrentACMethodBooking != null && CurrentDeliveryNotePos != null)
                        {
                            if (!CurrentDeliveryNotePos.InOrderPos.Material.IsLotManaged)
                                return Global.ControlModes.Disabled;
                        }
                        break;
                    }
            }

            if (!String.IsNullOrEmpty(vbControl.VBContent) && vbControl.VBContent.StartsWith("CurrentACMethodBooking"))
            {
                if (CurrentACMethodBooking == null)
                    return Global.ControlModes.Disabled;
                int pos = vbControl.VBContent.IndexOf('\\');
                if (pos > 0)
                {
                    string methodId = vbControl.VBContent.Substring(0, pos);
                    string acValueId = vbControl.VBContent.Substring(pos + 1);
                    if (!String.IsNullOrEmpty(methodId) && !String.IsNullOrEmpty(acValueId))
                    {
                        ACMethodBooking acMethod = ACUrlCommand(methodId) as ACMethodBooking;
                        if (acMethod != null)
                        {
                            Global.ControlModesInfo subResult = acMethod.OnGetControlModes(vbControl, acValueId);
                            if (subResult.Mode != result)
                            {
                                result = subResult.Mode;
                            }
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        #region BSO->ACMethod->DeliveryNote

        #region BSO -> ACMethod -> DeliveryNote -> Search

        /// <summary>
        /// Loads this instance.
        /// </summary>
        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedDeliveryNote", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            if (SelectedDeliveryNote != null && requery)
                SelectedDeliveryNote.DeliveryNotePos_DeliveryNote.AutoLoad(this.DatabaseApp);
            LoadEntity<DeliveryNote>(requery, () => SelectedDeliveryNote, () => CurrentDeliveryNote, c => CurrentDeliveryNote = c,
                        DatabaseApp.DeliveryNote
                        .Include(c => c.DeliveryNotePos_DeliveryNote)
                        .Include("DeliveryNotePos_DeliveryNote.InOrderPos.FacilityBooking_InOrderPos")
                        .Where(c => c.DeliveryNoteID == SelectedDeliveryNote.DeliveryNoteID));
            OnPropertyChanged("SelectedDeliveryNote");
            if (requery)
                OnPropertyChanged("SetupCurrentDeliveryNotePos");
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedDeliveryNote != null;
        }


        /// <summary>
        /// Searches the delivery note.
        /// </summary>
        [ACMethodCommand(DeliveryNote.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) 
                return; 
            AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("DeliveryNoteList");
            RefreshInOrderPosList();
            //Load();
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
            else if (acAccess == _AccessBookingFacility)
            {
                _AccessBookingFacility.NavSearch(this.DatabaseApp);
                OnPropertyChanged("BookingFacilityList");
                return true;
            }
            return base.ExecuteNavSearch(acAccess);
        }

        #endregion

        #region BSO-> ACMethod -> DeliveryNote -> Manipulate(Edit, Delete, Delivered etc)

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(DeliveryNote.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        /// <summary>
        /// Determines whether [is enabled save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        protected override Msg OnPreSave()
        {
            if (DeliveryNotePosList != null)
            {
                foreach (var dpos in DeliveryNotePosList.ToArray())
                {
                    if (dpos.InOrderPos != null)
                    {
                        dpos.InOrderPos.TopParentInOrderPos.RecalcDeliveryStates();
                    }
                }
            }
            return base.OnPreSave();
        }

        protected override void OnPostSave()
        {
            _UnSavedUnAssignedInOrderPos = new List<InOrderPos>();
            _UnSavedAssignedPickingInOrderPos = new List<InOrderPos>();
            RefreshInOrderPosList();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand(DeliveryNote.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        protected override void OnPostUndoSave()
        {
            _UnSavedUnAssignedInOrderPos = new List<InOrderPos>();
            _UnSavedAssignedPickingInOrderPos = new List<InOrderPos>();
            RefreshInOrderPosList();
            if (CurrentDeliveryNote != null && CurrentDeliveryNote.EntityState != System.Data.EntityState.Added)
                CurrentDeliveryNote.DeliveryNotePos_DeliveryNote.Load();
            OnPropertyChanged("DeliveryNotePosList");
            base.OnPostUndoSave();
        }


        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(DeliveryNote.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedDeliveryNote", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            string secondaryKey = Root.NoManager.GetNewNo(Database, typeof(DeliveryNote), DeliveryNote.NoColumnName, DeliveryNote.FormatNewNo, this);
            CurrentDeliveryNote = DeliveryNote.NewACObject(DatabaseApp, null, secondaryKey);
            CurrentDeliveryNote.DeliveryNoteType = GlobalApp.DeliveryNoteType.Receipt;
            DatabaseApp.DeliveryNote.AddObject(CurrentDeliveryNote);
            SelectedDeliveryNote = CurrentDeliveryNote;
            if (AccessPrimary != null)
                AccessPrimary.NavList.Add(CurrentDeliveryNote);
            ACState = Const.SMNew;
            PostExecute("New");
        }

        /// <summary>
        /// Determines whether [is enabled new delivery note].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new delivery note]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNew()
        {
            return true;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentDeliveryNote", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentDeliveryNote.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentDeliveryNote);
            SelectedDeliveryNote = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete delivery note].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete delivery note]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return true;
        }

        /// <summary>
        /// Deliveries the note ready.
        /// </summary>
        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Ready'}de{'Fertig'}", 600, false, "", Global.ACKinds.MSMethodPrePost)]
        public void DeliveryNoteReady()
        {
            if (!PreExecute("DeliveryNoteReady")) return;
            // TODO: 
            PostExecute("DeliveryNoteReady");
        }

        /// <summary>
        /// Determines whether [is enabled delivery note ready].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delivery note ready]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeliveryNoteReady()
        {
            // TODO: 
            return true;
        }

        /// <summary>
        /// Delivereds this instance.
        /// </summary>
        [ACMethodCommand(DeliveryNote.ClassName, "en{'Set Deliverd'}de{'Geliefert setzen'}", 601, true, Global.ACKinds.MSMethodPrePost)]
        public void Delivered()
        {
            if (!PreExecute("Delivered")) return;
            // TODO: 
            PostExecute("Delivered");
        }

        /// <summary>
        /// Determines whether [is enabled delivered].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delivered]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelivered()
        {
            // TODO: 
            return true;
        }

        [ACMethodCommand(DeliveryNote.ClassName, "en{'Cancel Delivery Note'}de{'Storniere Lieferschein'}", (short)MISort.Cancel)]
        public void CancelDelivery()
        {
            if (!PreExecute("CancelDelivery"))
                return;
            if (!IsEnabledCancelDelivery())
                return;
            var result = InDeliveryNoteManager.CancelFacilityPreBooking(ACFacilityManager, DatabaseApp, CurrentDeliveryNote);
            if (result != null && result.Any())
            {
                foreach (DeliveryNotePos deliveryNotePos in DeliveryNotePosList.ToList())
                {
                    SelectedDeliveryNotePos = deliveryNotePos;
                    if (CurrentDeliveryNotePos == deliveryNotePos)
                    {
                        BookAllACMethodBookings();
                    }
                }
                int countCancelled = 0;
                int countAssigned = 0;
                foreach (DeliveryNotePos deliveryNotePos in DeliveryNotePosList.ToList())
                {
                    if (deliveryNotePos.InOrderPos != null)
                        countAssigned++;
                    if ((deliveryNotePos.InOrderPos != null)
                        && (deliveryNotePos.InOrderPos.MDInOrderPosState.InOrderPosState == MDInOrderPosState.InOrderPosStates.Cancelled
                            || !deliveryNotePos.InOrderPos.FacilityBooking_InOrderPos.Any()))
                    {
                        countCancelled++;
                    }
                }
                if (countCancelled == countAssigned)
                {
                    MDDelivNoteState state = DatabaseApp.MDDelivNoteState.Where(c => c.MDDelivNoteStateIndex == (short)MDDelivNoteState.DelivNoteStates.Cancelled).FirstOrDefault();
                    if (state != null)
                    {
                        CurrentDeliveryNote.MDDelivNoteState = state;
                        Save();
                    }
                }
                OnPropertyChanged("DeliveryNotePosList");
            }
            PostExecute("CancelDelivery");
        }

        public bool IsEnabledCancelDelivery()
        {
            if (CurrentDeliveryNote == null || CurrentDeliveryNote.MDDelivNoteState == null)
                return false;
            if (CurrentDeliveryNote.MDDelivNoteState.DelivNoteState == MDDelivNoteState.DelivNoteStates.Cancelled)
                return false;
            if (!DeliveryNotePosList.Any())
                return false;
            return true;
        }

        #endregion

        #endregion

        #region BSO -> ACMethod -> DeliveryNotePos

        [ACMethodInteraction(DeliveryNote.ClassName, "en{'Filter'}de{'Filter'}", 602, false)]
        public bool FilterDialogInOrderPos()
        {
            bool result = AccessInOrderPos.ShowACQueryDialog();
            if (result)
            {
                RefreshInOrderPosList();
            }
            return result;
        }

        #region BSO -> ACMethod -> DeliveryNotePos -> Assign / Unassign

        /// <summary>
        /// Assigns the in order pos.
        /// </summary>
        [ACMethodCommand(DeliveryNote.ClassName, "en{'Assign'}de{'Zuordnen'}", 603, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void AssignInOrderPos()
        {
            if (!PreExecute("AssignInOrderPos"))
                return;

            if (!IsEnabledAssignInOrderPos())
                return;
            List<object> resultNewEntities = new List<object>();
            try
            {

                Msg result = InDeliveryNoteManager.AssignInOrderPos(CurrentInOrderPos, CurrentDeliveryNote, PartialQuantity, DatabaseApp, ACFacilityManager, resultNewEntities);
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

                Messages.LogException("BSOInDeliveryNote", "AssignInOrderPos", msg);

                return;
            }

            if (_UnSavedUnAssignedInOrderPos.Contains(CurrentInOrderPos))
                _UnSavedUnAssignedInOrderPos.Remove(CurrentInOrderPos);
            OnPropertyChanged("DeliveryNotePosList");

            RefreshInOrderPosList();
            PartialQuantity = null;
            foreach (object item in resultNewEntities)
            {
                if (item is DeliveryNotePos)
                {
                    SelectedDeliveryNotePos = item as DeliveryNotePos;
                    break;
                }
            }
            PostExecute("AssignInOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled assign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignInOrderPos()
        {
            if (CurrentInOrderPos == null)
                return false;
            return true;
        }

        /// <summary>
        /// Unassigns the in order pos.
        /// </summary>
        [ACMethodCommand(DeliveryNote.ClassName, "en{'Remove'}de{'Entfernen'}", 604, true, Global.ACKinds.MSMethodPrePost)]
        public void UnassignInOrderPos()
        {
            if (!IsEnabledUnassignInOrderPos())
                return;

            InOrderPos parentInOrderPos = null;
            InOrderPos currentInOrderPos = CurrentDeliveryNotePos.InOrderPos;
            parentInOrderPos = CurrentDeliveryNotePos.InOrderPos.InOrderPos1_ParentInOrderPos;

            Msg result = null;
            try
            {
                result = InDeliveryNoteManager.UnassignInOrderPos(CurrentDeliveryNotePos, DatabaseApp);
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

                Messages.LogException("BSOInDeliveryNote", "UnassignInOrderPos", msg);
                return;
            }

            if (result == null && parentInOrderPos != null)
            {
                if (!_UnSavedUnAssignedInOrderPos.Contains(parentInOrderPos))
                    _UnSavedUnAssignedInOrderPos.Add(parentInOrderPos);
            }

            if (_UnSavedAssignedPickingInOrderPos.Contains(currentInOrderPos))
            {
                _UnSavedAssignedPickingInOrderPos.Remove(currentInOrderPos);
                OnPropertyChanged("InOrderPosFromPickingList");
            }

            OnPropertyChanged("DeliveryNotePosList");
            RefreshInOrderPosList();
            PartialQuantity = null;
            PostExecute("UnassignInOrderPos");
        }

        /// <summary>
        /// Determines whether [is enabled unassign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled unassign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUnassignInOrderPos()
        {
            if (CurrentDeliveryNotePos == null)
                return false;
            return true;
        }

        #endregion

        #region BSO -> ACMethod -> DeliveryNotePos -> LabOrder

        [ACMethodInteraction("Dialog", "en{'New Lab Order'}de{'Neuer Laborauftrag'}", 605, false, "CreateNewLabOrderFromInOrder", Global.ACKinds.MSMethodPrePost)]
        public virtual void CreateNewLabOrderFromInOrder()
        {
            Save();
            if (this.DatabaseApp.IsChanged)
                return;

            ACComponent childBSO = ACUrlCommand("?LabOrderDialog") as ACComponent;
            if (childBSO == null && SelectedDeliveryNotePos != null)
            {
                childBSO = StartComponent("LabOrderDialog", null, new object[] { }) as ACComponent;
                childBSO.ACUrlCommand("!NewLabOrderDialog", SelectedDeliveryNotePos, null, null, null);
            }
            if (childBSO == null)
            {
                return;
            }
        }

        public bool IsEnabledCreateNewLabOrderFromInOrder()
        {
            if (SelectedDeliveryNotePos != null)
            {
                if (SelectedDeliveryNotePos.InOrderPos.LabOrder_InOrderPos.Any())
                    return false;
            }

            return true;
        }

        [ACMethodInfo("Dialog", "en{'Lab Report'}de{'Laborbericht'}", 606)]
        public void ShowLabOrderFromInOrder()
        {
            ACComponent childBSO = ACUrlCommand("?LabOrderViewDialog") as ACComponent;
            if (childBSO == null && SelectedDeliveryNotePos != null)
            {
                childBSO = StartComponent("LabOrderViewDialog", null, new object[] { }) as ACComponent;
                childBSO.ACUrlCommand("!ShowLabOrderViewDialog", SelectedDeliveryNotePos.InOrderPos, null, null, null, null, true, null);
                childBSO.Stop();
            }
        }

        public bool IsEnabledShowLabOrderFromInOrder()
        {
            if (SelectedDeliveryNotePos != null)
            {
                if (!SelectedDeliveryNotePos.InOrderPos.LabOrder_InOrderPos.Any())
                    return false;
            }

            return true;
        }

        #endregion

        #endregion

        #region Picking

        [ACMethodCommand(DeliveryNote.ClassName, "en{'Assign'}de{'Zuordnen'}", 607, true, Global.ACKinds.MSMethodPrePost)]
        public void AssignPicking()
        {
            if (!PreExecute("AssignPicking"))
                return;

            try
            {
                List<object> resultNewEntities = new List<object>();
                Msg result = InDeliveryNoteManager.NewDeliveryNotePos(CurrentInOrderPosFromPicking, CurrentDeliveryNote, DatabaseApp, resultNewEntities);
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

            if (!_UnSavedAssignedPickingInOrderPos.Contains(CurrentInOrderPosFromPicking))
                _UnSavedAssignedPickingInOrderPos.Add(CurrentInOrderPosFromPicking);
            OnPropertyChanged("InOrderPosFromPickingList");
            RefreshInOrderPosList();
            PartialQuantity = null;

            PostExecute("AssignPicking");
        }

        /// <summary>
        /// Determines whether [is enabled assign in order pos].
        /// </summary>
        /// <returns><c>true</c> if [is enabled assign in order pos]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledAssignPicking()
        {
            if (this.CurrentInOrderPosFromPicking == null || CurrentDeliveryNote == null)
                return false;
            return true;
        }

        #endregion

        #region InOrderPos
        [ACMethodInfo(DeliveryNote.ClassName, "en{'Find open order lines'}de{'Offene Bestellpos. suchen'}", 602, false)]
        public void RefreshInOrderPosList()
        {
            if (_ActivateInOpen && AccessInOrderPos != null)
                AccessInOrderPos.NavSearch(DatabaseApp);
            OnPropertyChanged("InOrderPosList");
        }

        #endregion

        #region FacilityPreBooking

        [ACMethodInteraction("FacilityPreBooking", "en{'New Posting'}de{'Neue Buchung'}", (short)MISort.New, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void NewFacilityPreBooking()
        {
            if (!IsEnabledNewFacilityPreBooking())
                return;
            if (!PreExecute("NewFacilityPreBooking"))
                return;
            CurrentFacilityPreBooking = InDeliveryNoteManager.NewFacilityPreBooking(ACFacilityManager, DatabaseApp, CurrentDeliveryNotePos);
            OnPropertyChanged("CurrentACMethodBooking");
            OnPropertyChanged("FacilityPreBookingList");
            PostExecute("NewFacilityPreBooking");
        }

        public bool IsEnabledNewFacilityPreBooking()
        {
            if (CurrentDeliveryNotePos == null || CurrentDeliveryNotePos.InOrderPos == null)
                return false;
            return true;
        }

        [ACMethodInteraction("FacilityPreBooking", "en{'Cancel Posting'}de{'Buchung abbrechen'}", (short)MISort.Cancel, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void CancelFacilityPreBooking()
        {
            if (!IsEnabledCancelFacilityPreBooking())
                return;
            if (!PreExecute("CancelFacilityPreBooking"))
                return;
            var result = InDeliveryNoteManager.CancelFacilityPreBooking(ACFacilityManager, DatabaseApp, CurrentDeliveryNotePos);
            if (result != null && result.Any())
            {
                CurrentFacilityPreBooking = result.FirstOrDefault();
                OnPropertyChanged("CurrentACMethodBooking");
                OnPropertyChanged("FacilityPreBookingList");
            }
            PostExecute("CancelFacilityPreBooking");
        }

        public bool IsEnabledCancelFacilityPreBooking()
        {
            if (CurrentDeliveryNotePos == null
                || CurrentDeliveryNotePos.InOrderPos == null
                || FacilityPreBookingList.Any())
                return false;
            return true;
        }

        [ACMethodInteraction("FacilityPreBooking", "en{'Delete Posting'}de{'Lösche Buchung'}", (short)MISort.Delete, true, "SelectedDeliveryNotePos", Global.ACKinds.MSMethodPrePost)]
        public void DeleteFacilityPreBooking()
        {
            if (!IsEnabledDeleteFacilityPreBooking())
                return;
            if (!PreExecute("DeleteFacilityPreBooking"))
                return;
            Msg msg = CurrentFacilityPreBooking.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            else
            {
                CurrentFacilityPreBooking = null;
                OnPropertyChanged("FacilityPreBookingList");
            }
            PostExecute("DeleteFacilityPreBooking");
        }

        public bool IsEnabledDeleteFacilityPreBooking()
        {
            return CurrentFacilityPreBooking != null;
        }

        [ACMethodInteraction("FacilityPreBooking", "en{'Post Item'}de{'Buche Position'}", (short)MISort.New, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void BookDeliveryPos()
        {
            if (!PreExecute("BookDeliveryPos")) return;
            PostExecute("BookDeliveryPos");
        }

        public bool IsEnabledBookDeliveryPos()
        {
            return true;
        }

        [ACMethodInteraction("FacilityPreBooking", "en{'Post'}de{'Buchen'}", (short)MISort.New, true, "SelectedFacilityPreBooking", Global.ACKinds.MSMethodPrePost)]
        public void BookCurrentACMethodBooking()
        {
            if (!IsEnabledBookCurrentACMethodBooking())
                return;

            if (CurrentACMethodBooking.InOrderPos != CurrentDeliveryNotePos.InOrderPos)
                CurrentACMethodBooking.InOrderPos = CurrentDeliveryNotePos.InOrderPos;
            if (CurrentDeliveryNotePos.InOrderPos.InOrder.CPartnerCompany != null && CurrentACMethodBooking.CPartnerCompany != CurrentDeliveryNotePos.InOrderPos.InOrder.CPartnerCompany)
                CurrentACMethodBooking.CPartnerCompany = CurrentDeliveryNotePos.InOrderPos.InOrder.CPartnerCompany;

            bool isCancellation = CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel || CurrentACMethodBooking.BookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel;

            Save();
            if (DatabaseApp.IsChanged)
                return;
            if (!PreExecute("BookCurrentACMethodBooking"))
                return;
            ACMethodEventArgs result = ACFacilityManager.BookFacility(CurrentACMethodBooking, this.DatabaseApp) as ACMethodEventArgs;
            if (!CurrentACMethodBooking.ValidMessage.IsSucceded() || CurrentACMethodBooking.ValidMessage.HasWarnings())
                Messages.Msg(CurrentACMethodBooking.ValidMessage);
            else if (result.ResultState == Global.ACMethodResultState.Failed || result.ResultState == Global.ACMethodResultState.Notpossible)
            {
                if (String.IsNullOrEmpty(result.ValidMessage.Message))
                    result.ValidMessage.Message = result.ResultState.ToString();
                Messages.Msg(result.ValidMessage);
                OnPropertyChanged("FacilityBookingList");
            }
            else
            {
                DeleteFacilityPreBooking();
                OnPropertyChanged("FacilityBookingList");
                CurrentDeliveryNotePos.InOrderPos.TopParentInOrderPos.RecalcActualQuantity();
                if (isCancellation)
                {
                    MDInOrderPosState state = DatabaseApp.MDInOrderPosState.Where(c => c.MDInOrderPosStateIndex == (short)MDInOrderPosState.InOrderPosStates.Cancelled).FirstOrDefault();
                    if (state != null)
                        CurrentDeliveryNotePos.InOrderPos.MDInOrderPosState = state;
                }
                else
                {
                    MDInOrderPosState state = DatabaseApp.MDInOrderPosState.Where(c => c.MDInOrderPosStateIndex == (short)MDInOrderPosState.InOrderPosStates.Completed).FirstOrDefault();
                    if (state != null)
                        CurrentDeliveryNotePos.InOrderPos.MDInOrderPosState = state;
                }
                Save();
            }
            PostExecute("BookCurrentACMethodBooking");
        }

        public bool IsEnabledBookCurrentACMethodBooking()
        {
            if (_CurrentACMethodBookingDummy != null)
                return false;
            bool bRetVal = false;
            if (CurrentACMethodBooking != null)
                bRetVal = CurrentACMethodBooking.IsEnabled();
            else
                return false;
            UpdateBSOMsg();
            return bRetVal;
        }

        [ACMethodCommand("DeliveryNotePos", "en{'Post all'}de{'Buche alle'}", (short)MISort.Cancel)]
        public void BookAllACMethodBookings()
        {
            if (!IsEnabledBookAllACMethodBookings())
                return;
            foreach (FacilityPreBooking facilityPreBooking in FacilityPreBookingList.ToList())
            {
                SelectedFacilityPreBooking = facilityPreBooking;
                if (CurrentFacilityPreBooking == facilityPreBooking)
                    BookCurrentACMethodBooking();
            }
        }

        public bool IsEnabledBookAllACMethodBookings()
        {
            if (CurrentDeliveryNotePos == null || FacilityPreBookingList == null || !FacilityPreBookingList.Any())
                return false;
            return true;
        }

        [ACMethodInfo("Dialog", "en{'New Lot'}de{'Neue Charge'}", (short)MISort.New)]
        public void NewFacilityLot()
        {
            if (!IsEnabledNewFacilityLot())
                return;
            ACComponent childBSO = ACUrlCommand("?FacilityLotDialog") as ACComponent;
            if (childBSO == null)
                childBSO = StartComponent("FacilityLotDialog", null, new object[] { }) as ACComponent;
            if (childBSO == null)
                return;
            VBDialogResult dlgResult = (VBDialogResult)childBSO.ACUrlCommand("!ShowDialogNewLot", "", CurrentDeliveryNotePos.InOrderPos.Material);
            if (dlgResult.SelectedCommand == eMsgButton.OK)
            {
                FacilityLot result = dlgResult.ReturnValue as FacilityLot;
                if (result != null)
                {
                    Save();
                    CurrentACMethodBooking.InwardFacilityLot = result;
                    OnNewCreatedFacilityLot(result);
                    OnPropertyChanged("BookableFacilityLots");
                    Save();
                }
            }
            childBSO.Stop();
        }

        public bool IsEnabledNewFacilityLot()
        {
            return CurrentACMethodBooking != null
                && CurrentDeliveryNotePos != null
                && CurrentDeliveryNotePos.InOrderPos != null
                && CurrentDeliveryNotePos.InOrderPos.Material != null
                && CurrentDeliveryNotePos.InOrderPos.Material.IsLotManaged;
        }

        public virtual void OnNewCreatedFacilityLot(FacilityLot lot)
        {
        }

        #endregion

        #region Message
        private void UpdateBSOMsg()
        {
            // TODO: Bei BSOFacilityBookCharge die Methode UpdateBSOMsg implementieren
            if (CurrentACMethodBooking == null)
                return;
            if (!CurrentACMethodBooking.ValidMessage.IsSucceded() || CurrentACMethodBooking.ValidMessage.HasWarnings())
            {
                if (!BSOMsg.IsEqual(CurrentACMethodBooking.ValidMessage))
                {
                    BSOMsg.UpdateFrom(CurrentACMethodBooking.ValidMessage);
                }
            }
            else
            {
                if (BSOMsg.MsgDetailsCount > 0)
                    BSOMsg.ClearMsgDetails();
            }
        }
        #endregion

        #region Report
        //[ACMethodCommand(DeliveryNote.ClassName, "en{'Print'}de{'Drucken'}", (short)MISort.QueryPrint, false, Global.ACKinds.MSMethodPrePost)]
        //public void ReportPrint()
        //{
        //    if (!PreExecute("ReportPrint")) return;
        //    // TODO: Drucken
        //    PostExecute("ReportPrint");
        //}
        #endregion

        #region Dialog New DeliveryNote
        public VBDialogResult DialogResult { get; set; }

        [ACMethodInfo("Dialog", "en{'New Delivery Note'}de{'Neuer Lieferschein'}", (short)MISort.QueryPrintDlg)]
        public VBDialogResult ShowDialogNewDeliveryNote(string DeliveryNoteNo = "")
        {
            New();
            if (!String.IsNullOrEmpty(DeliveryNoteNo))
                CurrentDeliveryNote.DeliveryNoteNo = DeliveryNoteNo;
            ShowDialog(this, "DeliveryNoteDialog");
            this.ParentACComponent.StopComponent(this);
            return DialogResult;
        }

        [ACMethodCommand("Dialog", "en{'OK'}de{'OK'}", (short)MISort.Okay)]
        public void DialogOK()
        {
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.OK;
            DialogResult.ReturnValue = CurrentDeliveryNote;
            CloseTopDialog();
        }

        [ACMethodCommand("Dialog", "en{'Cancel'}de{'Abbrechen'}", (short)MISort.Cancel)]
        public void DialogCancel()
        {
            if (CurrentDeliveryNote != null && CurrentDeliveryNote.EntityState == System.Data.EntityState.Added)
                Delete();
            DialogResult = new VBDialogResult();
            DialogResult.SelectedCommand = eMsgButton.Cancel;
            CloseTopDialog();
        }

        [ACMethodInfo("Dialog", "en{'Dialog Delivery Note'}de{'Dialog Lieferschein'}", (short)MISort.QueryPrintDlg)]
        public void ShowDialogNote(string deliveryNoteNo, Guid dNotePosID)
        {
            //AccessPrimary.NavACQueryDefinition.SearchWord = facilityNo;
            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "DeliveryNoteNo").FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, "DeliveryNoteNo", Global.LogicalOperators.contains, Global.Operators.and, deliveryNoteNo, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            else
                filterItem.SearchWord = deliveryNoteNo;

            this.Search();
            if (CurrentDeliveryNote != null && DeliveryNotePosList != null && dNotePosID != Guid.Empty)
            {
                DeliveryNotePos dnPos = DeliveryNotePosList.Where(c => c.DeliveryNotePosID == dNotePosID).FirstOrDefault();
                CurrentDeliveryNotePos = dnPos;
                SelectedDeliveryNotePos = dnPos;
            }
            ShowDialog(this, "DisplayNoteDialog");
            this.ParentACComponent.StopComponent(this);
        }

        #endregion

        #region Activation
        /// <summary>
        /// ACAction is called when one IACInteractiveObject (Source) wants to inform another IACInteractiveObject (Target) about an relevant interaction-event.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public override void ACAction(ACActionArgs actionArgs)
        {
            if (actionArgs.ElementAction == Global.ElementActionType.TabItemActivated)
            {
                OnActivate(actionArgs.DropObject.VBContent);
            }
            else
                base.ACAction(actionArgs);
        }

        protected bool _ActivateInOpen = false;
        [ACMethodInfo("Picking", "en{'Activate'}de{'Aktivieren'}", 608, true, Global.ACKinds.MSMethodPrePost)]
        public void OnActivate(string page)
        {
            if (!PreExecute("OnActivate"))
                return;
            switch (page)
            {
                case "*AssignInOrderPos":
                case "AssignInOrderPos":
                    if (!_ActivateInOpen)
                    {
                        _ActivateInOpen = true;
                        RefreshInOrderPosList();
                    }
                    break;
                default:
                    break;
            }
            PostExecute("OnActivate");
        }
        #endregion

        #region Show order dialog

        private bool _IsEnabledACProgram = true;
        public bool IsEnabledACProgram
        {
            get
            {
                return _IsEnabledACProgram;
            }
            set
            {
                _IsEnabledACProgram = value;
            }
        }


        [ACMethodInfo("Dialog", "en{'Dialog delivery note'}de{'Dialog Lieferschein'}", (short)MISort.QueryPrintDlg)]
        public void ShowDialogOrder(string deliveryNoteNo, Guid deliveryNotePosID)
        {
            if (AccessPrimary == null)
                return;
            //AccessPrimary.NavACQueryDefinition.SearchWord = facilityNo;
            ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == "DeliveryNoteNo").FirstOrDefault();
            if (filterItem == null)
            {
                filterItem = new ACFilterItem(Global.FilterTypes.filter, "DeliveryNoteNo", Global.LogicalOperators.contains, Global.Operators.and, deliveryNoteNo, false);
                AccessPrimary.NavACQueryDefinition.ACFilterColumns.Insert(0, filterItem);
            }
            else
                filterItem.SearchWord = deliveryNoteNo;

            this.Search();
            if (CurrentDeliveryNote != null && deliveryNotePosID != Guid.Empty)
            {
                if (this.DeliveryNotePosList != null && this.DeliveryNotePosList.Any())
                {
                    var deliveryNotePos = this.DeliveryNotePosList.Where(c => c.DeliveryNotePosID == deliveryNotePosID).FirstOrDefault();
                    if (deliveryNotePos != null)
                        SelectedDeliveryNotePos = deliveryNotePos;
                }
            }
            ShowDialog(this, "DisplayOrderDialog");
            this.ParentACComponent.StopComponent(this);
            _IsEnabledACProgram = true;
        }

        [ACMethodInfo("Dialog", "en{'Dialog delivery note'}de{'Dialog Lieferschein'}", (short)MISort.QueryPrintDlg + 1)]
        public void ShowDialogOrderInfo(PAOrderInfo paOrderInfo)
        {
            if (AccessPrimary == null || paOrderInfo == null)
                return;

            // Falls Produktionsauftrag
            DeliveryNotePos dnPos = null;
            DeliveryNote dn = null;
            foreach (var entry in paOrderInfo.Entities)
            {
                if (entry.EntityName == DeliveryNote.ClassName)
                {
                    dn = this.DatabaseApp.DeliveryNote
                        .Where(c => c.DeliveryNoteID == entry.EntityID)
                        .FirstOrDefault();
                }
                else if (entry.EntityName == DeliveryNotePos.ClassName)
                {
                    dnPos = this.DatabaseApp.DeliveryNotePos
                        .Include(c => c.DeliveryNote)
                        .Where(c => c.DeliveryNotePosID == entry.EntityID)
                        .FirstOrDefault();
                    if (dnPos != null)
                        dn = dnPos.DeliveryNote;
                }
                else if (entry.EntityName == OrderLog.ClassName)
                {
                    _IsEnabledACProgram = false;
                    OrderLog currentOrderLog = DatabaseApp.OrderLog.FirstOrDefault(c => c.VBiACProgramLogID == entry.EntityID);
                    if (currentOrderLog == null || currentOrderLog.DeliveryNotePos == null)
                        return;
                    dnPos = currentOrderLog.DeliveryNotePos;
                    dn = dnPos.DeliveryNote;
                }
            }

            if (dn == null)
                return;

            ShowDialogOrder(dn.DeliveryNoteNo, dnPos != null ? dnPos.DeliveryNotePosID : Guid.Empty);
        }
        #endregion

        #region Tracking

        public override ACMenuItemList GetMenu(string vbContent, string vbControl)
        {
            ACMenuItemList aCMenuItems = base.GetMenu(vbContent, vbControl);
            new BSOInDeliveryNoteTrackingMenuBuilder(this, aCMenuItems, vbContent, vbControl);
            return aCMenuItems;
        }

        [ACMethodInfo("OnTrackingCall", "en{'OnTrackingCall'}de{'OnTrackingCall'}", 609, false)]
        public void OnTrackingCall(GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter, TrackingEnginesEnum engine)
        {
            TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
            trackingCommonStart.DoTracking(this, direction, itemForTrack, additionalFilter, engine);
        }


        #endregion

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "BookAllACMethodBookings":
                    BookAllACMethodBookings();
                    return true;
                case "IsEnabledBookAllACMethodBookings":
                    result = IsEnabledBookAllACMethodBookings();
                    return true;
                case "NewFacilityLot":
                    NewFacilityLot();
                    return true;
                case "IsEnabledNewFacilityLot":
                    result = IsEnabledNewFacilityLot();
                    return true;
                case "ShowDialogNewDeliveryNote":
                    result = ShowDialogNewDeliveryNote(acParameter.Count() == 1 ? (String)acParameter[0] : "");
                    return true;
                case "DialogOK":
                    DialogOK();
                    return true;
                case "DialogCancel":
                    DialogCancel();
                    return true;
                case "ShowDialogNote":
                    ShowDialogNote((String)acParameter[0], (Guid)acParameter[1]);
                    return true;
                case "OnActivate":
                    OnActivate((String)acParameter[0]);
                    return true;
                case "ShowDeliveryCompanyRatingComplaint":
                    ShowDeliveryCompanyRatingComplaint();
                    return true;
                case "ShowShipperCompanyRatingComplaint":
                    ShowShipperCompanyRatingComplaint();
                    return true;
                case "IsEnabledShowDeliveryCompanyRatingComplaint":
                    result = IsEnabledShowDeliveryCompanyRatingComplaint();
                    return true;
                case "IsEnabledShowShipperCompanyRatingComplaint":
                    result = IsEnabledShowShipperCompanyRatingComplaint();
                    return true;
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
                case "DeliveryNoteReady":
                    DeliveryNoteReady();
                    return true;
                case "IsEnabledDeliveryNoteReady":
                    result = IsEnabledDeliveryNoteReady();
                    return true;
                case "Delivered":
                    Delivered();
                    return true;
                case "IsEnabledDelivered":
                    result = IsEnabledDelivered();
                    return true;
                case "CancelDelivery":
                    CancelDelivery();
                    return true;
                case "IsEnabledCancelDelivery":
                    result = IsEnabledCancelDelivery();
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
                case "RefreshInOrderPosList":
                    RefreshInOrderPosList();
                    return true;
                case "CreateNewLabOrderFromInOrder":
                    CreateNewLabOrderFromInOrder();
                    return true;
                case "IsEnabledCreateNewLabOrderFromInOrder":
                    result = IsEnabledCreateNewLabOrderFromInOrder();
                    return true;
                case "ShowLabOrderFromInOrder":
                    ShowLabOrderFromInOrder();
                    return true;
                case "IsEnabledShowLabOrderFromInOrder":
                    result = IsEnabledShowLabOrderFromInOrder();
                    return true;
                case "AssignPicking":
                    AssignPicking();
                    return true;
                case "IsEnabledAssignPicking":
                    result = IsEnabledAssignPicking();
                    return true;
                case "NewFacilityPreBooking":
                    NewFacilityPreBooking();
                    return true;
                case "IsEnabledNewFacilityPreBooking":
                    result = IsEnabledNewFacilityPreBooking();
                    return true;
                case "CancelFacilityPreBooking":
                    CancelFacilityPreBooking();
                    return true;
                case "IsEnabledCancelFacilityPreBooking":
                    result = IsEnabledCancelFacilityPreBooking();
                    return true;
                case "DeleteFacilityPreBooking":
                    DeleteFacilityPreBooking();
                    return true;
                case "IsEnabledDeleteFacilityPreBooking":
                    result = IsEnabledDeleteFacilityPreBooking();
                    return true;
                case "BookDeliveryPos":
                    BookDeliveryPos();
                    return true;
                case "IsEnabledBookDeliveryPos":
                    result = IsEnabledBookDeliveryPos();
                    return true;
                case "BookCurrentACMethodBooking":
                    BookCurrentACMethodBooking();
                    return true;
                case "IsEnabledBookCurrentACMethodBooking":
                    result = IsEnabledBookCurrentACMethodBooking();
                    return true;
                case "ShowDialogOrder":
                    ShowDialogOrder((String)acParameter[0], (Guid)acParameter[1]);
                    return true;
                case "ShowDialogOrderInfo":
                    ShowDialogOrderInfo((PAOrderInfo)acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }
}
