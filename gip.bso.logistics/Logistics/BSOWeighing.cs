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

        #region BSO->ACProperty-> Filter

        /// <summary>
        /// Source Property: 
        /// </summary>
        private bool _FilterSelectAll;
        [ACPropertyInfo(999, nameof(FilterSelectAll), ConstApp.SelectAll)]
        public bool FilterSelectAll
        {
            get
            {
                return _FilterSelectAll;
            }
            set
            {
                if (_FilterSelectAll != value)
                {
                    _FilterSelectAll = value;
                    OnPropertyChanged(nameof(FilterSelectAll));

                    foreach (var item in WeighingList)
                        item.IsSelected = value;

                    OnPropertyChanged(nameof(WeighingList));
                }
            }
        }

        public const string cEWeighingNoProperty = nameof(Weighing.WeighingNo);
        [ACPropertyInfo(999, "Filter", ConstApp.Weighing)]
        public string FilterWeighingNo
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(cEWeighingNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(cEWeighingNoProperty);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(cEWeighingNoProperty, value);
                    OnPropertyChanged();
                }
            }
        }

        #region BSO->ACProperty-> Filter -> Picking
        //public const string cEWeighingPickingPosProperty = nameof(Weighing.PickingPosID);
        private bool? _FilterIsPicking;
        [ACPropertyInfo(999, "Filter", ConstApp.Picking)]
        public bool? FilterIsPicking
        {
            get
            {
                return _FilterIsPicking;
            }
            set
            {
                if (_FilterIsPicking != value)
                {
                    _FilterIsPicking = value;
                    OnPropertyChanged();
                }
            }
        }

        public const string cEWeighingPickingNoProperty = nameof(PickingPos) + "\\" + nameof(Picking) + "\\" + nameof(Picking.PickingNo);
        [ACPropertyInfo(999, "Filter", ConstApp.PickingNo)]
        public string FilterPickingNo
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(cEWeighingPickingNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(cEWeighingPickingNoProperty);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(cEWeighingPickingNoProperty, value);
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region BSO->ACProperty-> Filter -> InOrder

        //public const string cEWeighingInOrderPosProperty = nameof(Weighing) + "\\" + nameof(Weighing.InOrderPosID);
        private bool? _FilterIsInOrder;
        [ACPropertyInfo(999, "Filter", "en{'Purchase Order'}de{'Bestellung'}")]
        public bool? FilterIsInOrder
        {
            get
            {
                return _FilterIsInOrder;
            }
            set
            {
                if (_FilterIsInOrder != value)
                {
                    _FilterIsInOrder = value;
                    OnPropertyChanged();
                }
            }
        }

        public const string cEWeighingInOrderNoProperty = nameof(InOrderPos) + "\\" + nameof(InOrder) + "\\" + nameof(InOrder.InOrderNo);
        [ACPropertyInfo(999, "Filter", "en{'Purchase Order Number'}de{'Bestellnummer'}")]
        public string FilterInOrderNo
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(cEWeighingInOrderNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(cEWeighingInOrderNoProperty);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(cEWeighingInOrderNoProperty, value);
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region BSO->ACProperty-> Filter -> OutOrder
        //public const string cEWeighingOutOrderPosProperty = nameof(Weighing) + "\\" + nameof(Weighing.OutOrderPosID);
        private bool? _FilterIsOutOrder;
        [ACPropertyInfo(999, "Filter", "en{'Sales Order'}de{'Kundenauftrag'}")]
        public bool? FilterIsOutOrder
        {
            get
            {
                return _FilterIsOutOrder;
            }
            set
            {
                if (_FilterIsOutOrder != value)
                {
                    _FilterIsOutOrder = value;
                    OnPropertyChanged();
                }
            }
        }
        public const string cEWeighingOutOrderNoProperty = nameof(OutOrderPos) + "\\" + nameof(OutOrder) + "\\" + nameof(OutOrder.OutOrderNo);
        [ACPropertyInfo(999, "Filter", "en{'Sales Order Number'}de{'Kundennummer'}")]
        public string FilterOutOrderNo
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(cEWeighingOutOrderNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(cEWeighingOutOrderNoProperty);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(cEWeighingOutOrderNoProperty, value);
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region BSO->ACProperty-> Filter -> LabOrder
        //public const string cEWeighingLabOrderPosProperty = nameof(Weighing) + "\\" + nameof(Weighing.LabOrderPosID);
        private bool? _FilterIsLabOrder;
        [ACPropertyInfo(999, "Filter", "en{'Lab Report'}de{'Laborauftrag'}")]
        public bool? FilterIsLabOrder
        {
            get
            {
                return _FilterIsLabOrder;
            }
            set
            {
                if (_FilterIsLabOrder != value)
                {
                    _FilterIsLabOrder = value;
                    OnPropertyChanged();
                }
            }
        }

        public const string cEWeighingLabOrderNoProperty = nameof(LabOrderPos) + "\\" + nameof(LabOrder) + "\\" + nameof(LabOrder.LabOrderNo);
        [ACPropertyInfo(999, "Filter", "en{'Lab Report Number'}de{'Laborauftrag Nummer'}")]
        public string FilterLabOrderNo
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(cEWeighingLabOrderNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(cEWeighingLabOrderNoProperty);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(cEWeighingLabOrderNoProperty, value);
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region BSO->ACProperty-> Filter -> VisitorVoucher
        // public const string cEWeighingVisitorVoucherProperty = nameof(Weighing) + "\\" + nameof(Weighing.VisitorVoucherID);
        private bool? _FilterIsVisitorVoucher;
        [ACPropertyInfo(999, "Filter", "en{'Visitor Voucher'}de{'Besucher Beleg'}")]
        public bool? FilterIsVisitorVoucher
        {
            get
            {
                return _FilterIsVisitorVoucher;
            }
            set
            {
                if (_FilterIsVisitorVoucher != value)
                {
                    _FilterIsVisitorVoucher = value;
                    OnPropertyChanged();
                }
            }
        }

        public const string cEWeighingVisitorVoucherNoProperty = nameof(VisitorVoucher) + "\\" + nameof(VisitorVoucher.VisitorVoucherNo);
        [ACPropertyInfo(999, "Filter", "en{'Visitor Voucher No.'}de{'Besucherbeleg-Nr.'}")]
        public string FilterVisitorVoucherNo
        {
            get
            {
                return AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(cEWeighingVisitorVoucherNoProperty);
            }
            set
            {
                string tmp = AccessPrimary.NavACQueryDefinition.GetSearchValue<string>(cEWeighingVisitorVoucherNoProperty);
                if (tmp != value)
                {
                    AccessPrimary.NavACQueryDefinition.SetSearchValue<string>(cEWeighingVisitorVoucherNoProperty, value);
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region BSO->ACProperty-> Filter -> FilterWeighingState
        public const string FilterWeighingState = "FilterWeighingState";

        private ACValueItem _SelectedFilterWeighingState;
        /// <summary>
        /// Selected property for ACValueItem
        /// </summary>
        /// <value>The selected FilterWeighingState</value>
        [ACPropertySelected(9999, nameof(FilterWeighingState), "en{'Status'}de{'Status'}")]
        public ACValueItem SelectedFilterWeighingState
        {
            get
            {
                return _SelectedFilterWeighingState;
            }
            set
            {
                if (_SelectedFilterWeighingState != value)
                {
                    _SelectedFilterWeighingState = value;
                    WeighingStateEnum? weigingState = null;
                    if (value != null)
                    {
                        weigingState = (WeighingStateEnum?)value.Value;
                    }
                    ACFilterItem filterItem = AccessPrimary.NavACQueryDefinition.ACFilterColumns.Where(c => c.PropertyName == nameof(Weighing.StateIndex)).FirstOrDefault();
                    if (filterItem != null)
                    {
                        if (value == null)
                        {
                            filterItem.SearchWord = null;
                        }
                        else
                        {
                            filterItem.SetSearchValue<short>((short)weigingState);
                        }
                    }
                    OnPropertyChanged(nameof(SelectedFilterWeighingState));
                }
            }
        }


        private List<ACValueItem> _FilterWeighingStateList;
        /// <summary>
        /// List property for ACValueItem
        /// </summary>
        /// <value>The FilterWeighingState list</value>
        [ACPropertyList(9999, nameof(FilterWeighingState))]
        public List<ACValueItem> FilterWeighingStateList
        {
            get
            {
                if (_FilterWeighingStateList == null)
                    _FilterWeighingStateList = LoadFilterWeighingStateList();
                return _FilterWeighingStateList;
            }
        }

        private List<ACValueItem> LoadFilterWeighingStateList()
        {
            ACValueItemList list = null;
            gip.core.datamodel.ACClass enumClass = Database.ContextIPlus.GetACType(typeof(WeighingStateEnum));
            if (enumClass != null && enumClass.ACValueListForEnum != null)
                list = enumClass.ACValueListForEnum;
            else
                list = new ACValueListWeighingStateEnum();
            return list;

        }
        #endregion

        #region BSO->ACProperty-> Filter -> ToWeighingState
        public const string ToWeighingState = "ToWeighingState";

        private ACValueItem _SelectedToWeighingState;
        /// <summary>
        /// Selected property for ACValueItem
        /// </summary>
        /// <value>The selected ToWeighingState</value>
        [ACPropertySelected(9999, nameof(ToWeighingState), "en{'Status'}de{'Status'}")]
        public ACValueItem SelectedToWeighingState
        {
            get
            {
                return _SelectedToWeighingState;
            }
            set
            {
                if (_SelectedToWeighingState != value)
                {
                    _SelectedToWeighingState = value;
                    OnPropertyChanged(nameof(SelectedToWeighingState));
                }
            }
        }


        private List<ACValueItem> _ToWeighingStateList;
        /// <summary>
        /// List property for ACValueItem
        /// </summary>
        /// <value>The ToWeighingState list</value>
        [ACPropertyList(9999, nameof(ToWeighingState))]
        public List<ACValueItem> ToWeighingStateList
        {
            get
            {
                if (_ToWeighingStateList == null)
                    _ToWeighingStateList = LoadToWeighingStateList();
                return _ToWeighingStateList;
            }
        }

        private List<ACValueItem> LoadToWeighingStateList()
        {
            ACValueItemList list = null;
            gip.core.datamodel.ACClass enumClass = Database.ContextIPlus.GetACType(typeof(WeighingStateEnum));
            if (enumClass != null && enumClass.ACValueListForEnum != null)
                list = enumClass.ACValueListForEnum;
            else
                list = new ACValueListWeighingStateEnum();
            return list;

        }
        #endregion

        #endregion

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
                    new ACFilterItem(Global.FilterTypes.filter, Weighing.NoColumnName, Global.LogicalOperators.contains, Global.Operators.and, null, true, true),
                    new ACFilterItem(Global.FilterTypes.filter, nameof(Weighing.StateIndex), Global.LogicalOperators.equal, Global.Operators.and, null, true),

                    // PickingPos
                    new ACFilterItem(Global.FilterTypes.filter, cEWeighingPickingNoProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),

                    // InOrderPos
                    new ACFilterItem(Global.FilterTypes.filter, cEWeighingInOrderNoProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),

                    // OutOrderPos
                    new ACFilterItem(Global.FilterTypes.filter, cEWeighingOutOrderNoProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),

                    // LabOrderPos
                    new ACFilterItem(Global.FilterTypes.filter, cEWeighingLabOrderNoProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),

                    // VisitorVoucher
                    new ACFilterItem(Global.FilterTypes.filter, cEWeighingVisitorVoucherNoProperty, Global.LogicalOperators.contains, Global.Operators.or, "", true),
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
                    .Include("VisitorVoucherPos.DeliveryNotePos_VisitorVoucherPos.DeliveryNote")
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
                    return SelectedWeighing.VisitorVoucher?.DeliveryNote_VisitorVoucher.FirstOrDefault();
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


        /// <summary>
        /// Source Property: ChangeWeighingState
        /// </summary>
        [ACMethodInfo(nameof(ChangeWeighingState), "en{'Change Weighing state'}de{'Wägezustand ändern'}", 999)]
        public void ChangeWeighingState()
        {
            if (!IsEnabledChangeWeighingState())
                return;

            var weighingsToChange = WeighingList.Where(c => c.IsSelected).ToList();
            foreach (Weighing weighing in weighingsToChange)
            {
                weighing.WeighingState = (WeighingStateEnum)SelectedToWeighingState.Value;
            }

            OnPropertyChanged(nameof(WeighingList));
        }

        public bool IsEnabledChangeWeighingState()
        {
            return
                WeighingList != null
                && WeighingList.Any(c => c.IsSelected)
                && SelectedToWeighingState != null;
        }

        #region Methods -> ACMetod Save

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(gip.core.datamodel.ACClass.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost, Description =
                         "Saves this instance.")]
        public void Save()
        {
            if (!PreExecute()) return;
            OnSave();
            PostExecute();
        }

        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand("Partslist", "en{'Undo'}de{'Rückgängig'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost, Description =
                         "Undoes the save.")]
        public void UndoSave()
        {
            if (!PreExecute()) return;
            OnUndoSave();
            PostExecute();
        }

        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        #endregion



        #region Methods -> Override
        public override void OnPropertyChanged([CallerMemberName] string name = "")
        {
            base.OnPropertyChanged(name);
            string[] propertiesNKGSearchAgain = new string[]
            {
                nameof(FilterWeighingNo),
                nameof(FilterIsPicking),
                nameof(FilterPickingNo),
                nameof(FilterIsInOrder),
                nameof(FilterInOrderNo),
                 nameof(FilterIsOutOrder),
                nameof(FilterOutOrderNo),
                nameof(FilterIsLabOrder),
                nameof(FilterLabOrderNo),
                nameof(FilterIsVisitorVoucher),
                nameof(FilterVisitorVoucherNo),
                nameof(SelectedFilterWeighingState)
            };

            if (
                    propertiesNKGSearchAgain.Contains(name)

                )
            {
                Search();
            }
        }
        #endregion

        #region Methods -> Navigate

        [ACMethodInteraction("", ConstApp.ShowPicking, 781, true, nameof(SelectedWeighing))]
        public void NavigateToPicking()
        {
            if (!IsEnabledNavigateToPicking())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(PickingPos), SelectedWeighing.PickingPosID ?? Guid.Empty));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToPicking()
        {
            if (SelectedWeighing != null && SelectedWeighing.PickingPos != null)
                return true;
            return false;
        }


        [ACMethodInteraction("", ConstApp.ShowInOrder, 781, true, nameof(SelectedWeighing))]
        public void NavigateToInOrder()
        {
            if (!IsEnabledNavigateToInOrder())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(InOrderPos), SelectedWeighing.InOrderPosID ?? Guid.Empty));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToInOrder()
        {
            if (SelectedWeighing != null && SelectedWeighing.InOrderPos != null)
                return true;
            return false;
        }

        [ACMethodInteraction("", ConstApp.ShowOutOrder, 781, true, nameof(SelectedWeighing))]
        public void NavigateToOutOrder()
        {
            if (!IsEnabledNavigateToOutOrder())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(OutOrderPos), SelectedWeighing.OutOrderPosID ?? Guid.Empty));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToOutOrder()
        {
            if (SelectedWeighing != null && SelectedWeighing.OutOrderPos != null)
                return true;
            return false;
        }

        [ACMethodInteraction("", ConstApp.ShowLabOrder, 781, true, nameof(SelectedWeighing))]
        public void NavigateToLabOrder()
        {
            if (!IsEnabledNavigateToLabOrder())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(LabOrderPos), SelectedWeighing.LabOrderPosID ?? Guid.Empty));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToLabOrder()
        {
            if (SelectedWeighing != null && SelectedWeighing.LabOrderPos != null)
                return true;
            return false;
        }

        [ACMethodInteraction("", ConstApp.ShowVisitorVoucher, 781, true, nameof(SelectedWeighing))]
        public void NavigateToVisitorVoucher()
        {
            if (!IsEnabledNavigateToVisitorVoucher())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(VisitorVoucher), SelectedWeighing.VisitorVoucherID ?? Guid.Empty));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToVisitorVoucher()
        {
            if (SelectedWeighing != null && SelectedWeighing.VisitorVoucher != null)
                return true;
            return false;
        }

        #endregion


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
