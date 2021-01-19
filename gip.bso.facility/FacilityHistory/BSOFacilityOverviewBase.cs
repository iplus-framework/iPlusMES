// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOFacilityMaterialOverview.cs" company="gip mbh, Oftersheim, Germany">
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
using gip.mes.facility;
using System.Data.Objects;
using System.ComponentModel;
using gip.mes.processapplication;

namespace gip.bso.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Overview Base'}de{'Übersicht Basis'}", Global.ACKinds.TACAbstractClass, Global.ACStorableTypes.NotStorable, true, true)]
    public abstract class BSOFacilityOverviewBase : BSOFacilityBase
    {
        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOFacilityMaterialOverview"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOFacilityOverviewBase(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            SearchTo = DateTime.Now;
            SearchFrom = SearchTo.AddDays(-1);
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
            _showNotAvailable = false;
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {

            CleanMovements();
            this._FilterFBType = null;
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region BSO->ACProperty

        #region Filter/Limitation
        DateTime _SearchFrom;
        [ACPropertyInfo(705, "", "en{'Search from'}de{'Suche von'}")]
        public DateTime SearchFrom
        {
            get
            {
                return _SearchFrom;
            }
            set
            {
                _SearchFrom = value;
                OnPropertyChanged("SearchFrom");
            }
        }

        DateTime _SearchTo;
        [ACPropertyInfo(706, "", "en{'Search to'}de{'Suche bis'}")]
        public DateTime SearchTo
        {
            get
            {
                return _SearchTo;
            }
            set
            {
                _SearchTo = value;
                OnPropertyChanged("SearchTo");
            }
        }

        ACValueItem _FilterFBType;
        [ACPropertyInfo(707, "", "en{'Filter Posting Type'}de{'Filter Buchungsart'}")]
        public ACValueItem FilterFBType
        {
            get
            {
                return _FilterFBType;
            }
            set
            {
                _FilterFBType = value;
                OnPropertyChanged("FilterFBType");
            }
        }

        [ACPropertyInfo(708, "", "en{'Grouped by Material'}de{'Gruppiert nach Material'}")]
        public bool GroupByMaterial
        {
            get;
            set;
        }

        public const string _CNotAvailableProperty = "NotAvailable";
        protected bool? _showNotAvailable;
        [ACPropertyInfo(709, "Filter", "en{'Show not available'}de{'Nicht verfügbare anzeigen'}")]
        public virtual bool? ShowNotAvailable
        {
            get
            {
                return _showNotAvailable;
            }
            set
            {
                if (_showNotAvailable != value)
                {
                    _showNotAvailable = value;
                    OnPropertyChanged("ShowNotAvailable");
                }
            }
        }

        #endregion

        #region FacilityBookingOverview
        private FacilityBookingOverview _SelectedFacilityBookingOverview;

        [ACPropertySelected(701, "FacilityBookingOverview")]

        public FacilityBookingOverview SelectedFacilityBookingOverview
        {
            get
            {
                return _SelectedFacilityBookingOverview;
            }
            set
            {
                if (_SelectedFacilityBookingOverview != value)
                {
                    _SelectedFacilityBookingOverview = value;
                    OnPropertyChanged("SelectedFacilityBookingOverview");
                }
            }
        }

        private List<FacilityBookingOverview> _FacilityBookingOverviewList;
        /// <summary>
        /// Gets the facility booking list.
        /// </summary>
        /// <value>The facility booking list.</value>
        [ACPropertyList(702, "FacilityBookingOverview")]
        public virtual List<FacilityBookingOverview> FacilityBookingOverviewList
        {
            get
            {
                return _FacilityBookingOverviewList;
            }
        }


        #endregion

        #region FacilityBookingChargeOverview

        private FacilityBookingChargeOverview _SelectedFacilityBookingChargeOverview;

        [ACPropertySelected(703, "FacilityBookingChargeOverview")]
        public FacilityBookingChargeOverview SelectedFacilityBookingChargeOverview
        {
            get
            {
                return _SelectedFacilityBookingChargeOverview;
            }
            set
            {
                if (_SelectedFacilityBookingChargeOverview != value)
                {
                    _SelectedFacilityBookingChargeOverview = value;
                    OnPropertyChanged("SelectedFacilityBookingChargeOverview");
                }
            }
        }

        private List<FacilityBookingChargeOverview> _FacilityBookingChargeOverviewList;
        /// <summary>
        /// Gets the facility booking charge list.
        /// </summary>
        /// <value>The facility booking charge list.</value>
        [ACPropertyList(704, "FacilityBookingChargeOverview")]
        public virtual List<FacilityBookingChargeOverview> FacilityBookingChargeOverviewList
        {
            get
            {
                return _FacilityBookingChargeOverviewList;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Methods -> Other methods 


        #endregion

        #region Tracking

        public override ACMenuItemList GetMenu(string vbContent, string vbControl)
        {
            ACMenuItemList aCMenuItems = base.GetMenu(vbContent, vbControl);
            if (vbContent == "SelectedFacilityBookingOverview" && SelectedFacilityBookingOverview != null)
            {
                TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
                FacilityBooking facilityBooking = new FacilityBooking() { FacilityBookingNo = SelectedFacilityBookingOverview.FacilityBookingNo, FacilityBookingID = SelectedFacilityBookingOverview.FacilityBookingID };
                ACMenuItemList trackingAndTracingMenuItems = trackingCommonStart.GetTrackingAndTrackingMenuItems(this, facilityBooking);
                aCMenuItems.AddRange(trackingAndTracingMenuItems);
            }
            if (vbContent == "SelectedFacilityBookingChargeOverview" && SelectedFacilityBookingChargeOverview != null)
            {
                ACMenuItemList menuItemList = new ACMenuItemList();
                if (!string.IsNullOrEmpty(SelectedFacilityBookingChargeOverview.DeliveryNoteNo))
                {
                    ACMenuItem deliveryNotePosMenuItem = new ACMenuItem("en{'Dialog delivery note'}de{'Dialog Lieferschein'}", "SelectedFacilityBookingChargeOverview\\" + PresenterMenuItems.DeliveryNotePos.ToString(), 250, null, null, true);
                    menuItemList.Add(deliveryNotePosMenuItem);
                }
                else if (!string.IsNullOrEmpty(SelectedFacilityBookingChargeOverview.InwardFacilityChargeProdOrderProgramNo))
                {
                    ACMenuItem inOrderPosMenuItem = new ACMenuItem("en{'View order'}de{'Auftrag anschauen'}", "SelectedFacilityBookingChargeOverview\\" + PresenterMenuItems.ProdOrderPartslist.ToString(), 250, null, null, true);
                    menuItemList.Add(inOrderPosMenuItem);
                }
                if (!string.IsNullOrEmpty(SelectedFacilityBookingChargeOverview.InwardFacilityChargeInOrderNo))
                {
                    ACMenuItem inOrderPosMenuItem = new ACMenuItem("en{'Dialog Purchase Order'}de{'Dialog Bestellung'}", "SelectedFacilityBookingChargeOverview\\" + PresenterMenuItems.InOrderPos.ToString(), 250, null, null, true);
                    menuItemList.Add(inOrderPosMenuItem);
                }
                aCMenuItems.AddRange(menuItemList);
            }
            if (vbContent == "SelectedFacilityBookingOverview" && SelectedFacilityBookingOverview != null)
            {
                ACMenuItemList menuItemList = new ACMenuItemList();
                if (!string.IsNullOrEmpty(SelectedFacilityBookingOverview.DeliveryNoteNo))
                {
                    ACMenuItem deliveryNotePosMenuItem = new ACMenuItem("en{'Dialog delivery note'}de{'Dialog Lieferschein'}", "SelectedFacilityBookingOverview\\" + PresenterMenuItems.DeliveryNotePos.ToString(), 250, null, null, true);
                    menuItemList.Add(deliveryNotePosMenuItem);
                }
                else if (!string.IsNullOrEmpty(SelectedFacilityBookingOverview.InwardFacilityChargeProdOrderProgramNo))
                {
                    ACMenuItem inOrderPosMenuItem = new ACMenuItem("en{'View order'}de{'Auftrag anschauen'}", "SelectedFacilityBookingOverview\\" + PresenterMenuItems.ProdOrderPartslist.ToString(), 250, null, null, true);
                    menuItemList.Add(inOrderPosMenuItem);
                }
                if (!string.IsNullOrEmpty(SelectedFacilityBookingOverview.InwardFacilityChargeInOrderNo))
                {
                    ACMenuItem inOrderPosMenuItem = new ACMenuItem("en{'Dialog Purchase Order'}de{'Dialog Bestellung'}", "SelectedFacilityBookingOverview\\" + PresenterMenuItems.InOrderPos.ToString(), 250, null, null, true);
                    menuItemList.Add(inOrderPosMenuItem);
                }
                aCMenuItems.AddRange(menuItemList);
            }
            return aCMenuItems;
        }

        [ACMethodInfo("OnTrackingCall", "en{'OnTrackingCall'}de{'OnTrackingCall'}", 702, false)]
        public void OnTrackingCall(GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter, TrackingEnginesEnum engine)
        {
            TrackingCommonStart trackingCommonStart = new TrackingCommonStart();
            trackingCommonStart.DoTracking(this, direction, itemForTrack, additionalFilter, engine);
        }


        /// <summary>
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public override void ACAction(ACActionArgs actionArgs)
        {
            base.ACAction(actionArgs);

            if (actionArgs.ElementAction == Global.ElementActionType.ACCommand
                && actionArgs.DropObject != null
                && actionArgs.DropObject.ACContentList != null)
            {
                ACCommand acCommand = actionArgs.DropObject.ACContentList.Where(c => c is ACCommand).FirstOrDefault() as ACCommand;
                if (acCommand != null)
                {
                    if (acCommand.ACUrl.StartsWith("SelectedFacilityBooking") && (SelectedFacilityBookingOverview != null || SelectedFacilityBookingChargeOverview != null))
                    {
                        using (DatabaseApp databaseApp = new DatabaseApp())
                        {
                            PAShowDlgManagerVB manager = PAShowDlgManagerVB.GetServiceInstance(this) as PAShowDlgManagerVB;
                            FacilityBooking facilityBooking = null;
                            string menuItemTypeStr = "";
                            if (acCommand.ACUrl.StartsWith("SelectedFacilityBookingOverview\\"))
                            {
                                menuItemTypeStr = acCommand.ACUrl.Replace("SelectedFacilityBookingOverview\\", "");
                                facilityBooking = databaseApp.FacilityBooking.FirstOrDefault(c => c.FacilityBookingNo == SelectedFacilityBookingOverview.FacilityBookingNo);
                            }
                            else if (acCommand.ACUrl.StartsWith("SelectedFacilityBookingChargeOverview\\"))
                            {
                                menuItemTypeStr = acCommand.ACUrl.Replace("SelectedFacilityBookingChargeOverview\\", "");
                                facilityBooking =
                                    databaseApp
                                    .FacilityBookingCharge
                                    .Where(c => c.FacilityBookingChargeNo == SelectedFacilityBookingChargeOverview.FacilityBookingChargeNo)
                                    .Select(c => c.FacilityBooking)
                                    .FirstOrDefault();
                            }
                            PresenterMenuItems menuItemType = PresenterMenuItems.ProdOrderPartslist;
                            if(Enum.TryParse<PresenterMenuItems>(menuItemTypeStr, out menuItemType))
                            {
                                PAOrderInfo pAOrderInfo = GetPAOrderInfo(databaseApp, manager, facilityBooking, menuItemType);
                                if (pAOrderInfo.Entities.Any())
                                    manager.ShowDialogOrder(ParentACObject as ACComponent, pAOrderInfo);
                            }
                        }
                    }
                }
            }
        }

        private PAOrderInfo GetPAOrderInfo(DatabaseApp databaseApp, PAShowDlgManagerVB manager, FacilityBooking facilityBooking, PresenterMenuItems menuItemType)
        {
            PAOrderInfo pAOrderInfo = new PAOrderInfo();
            switch (menuItemType)
            {
                case PresenterMenuItems.ProdOrderPartslist:
                    pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                    {
                        EntityID = facilityBooking.FacilityBookingID,
                        EntityName = FacilityBooking.ClassName
                    });
                    if (facilityBooking.ProdOrderPartslistPosRelationID != null)
                    {
                        pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = facilityBooking.ProdOrderPartslistPosRelationID.Value,
                            EntityName = ProdOrderPartslistPosRelation.ClassName
                        });
                    }
                    if (facilityBooking.ProdOrderPartslistPosID != null)
                    {
                        pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                        {
                            EntityID = facilityBooking.ProdOrderPartslistPosID.Value,
                            EntityName = ProdOrderPartslistPos.ClassName
                        });
                    }
                    break;
                case PresenterMenuItems.InOrderPos:
                    InOrderPos inOrderPos = facilityBooking.InOrderPos;
                    InOrder inOrder = databaseApp.InOrder.FirstOrDefault(c => c.InOrderNo == SelectedFacilityBookingChargeOverview.InwardFacilityChargeInOrderNo);
                    manager.ShowInOrderDialog(inOrderPos.InOrder.InOrderNo, inOrderPos.InOrderPosID);
                    break;
                case PresenterMenuItems.DeliveryNotePos:
                    DeliveryNotePos dns = facilityBooking.InOrderPos.DeliveryNotePos_InOrderPos.FirstOrDefault();
                    pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                    {
                        EntityID = dns.DeliveryNotePosID,
                        EntityName = DeliveryNotePos.ClassName
                    });
                    break;
                default:
                    break;
            }

            return pAOrderInfo;
        }

        #endregion

        #region FacilityBooking(Charge)Overview methods

        #region FacilityBooking(Charge)Overview methods -> precompiled queries

        #endregion

        #region FacilityBooking(Charge)Overview methods -> Executive methods
        public virtual List<FacilityBookingOverview> GroupFacilityBookingOverviewList(IEnumerable<FacilityBookingOverview> query)
        {
            if (ACFacilityManager == null)
                return null;
            return this.ACFacilityManager.GroupFacilityBookingOverviewList(query);
        }

        public virtual Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> GetFacilityOverviewLists(DatabaseApp databaseApp, FacilityQueryFilter filter)
        {
            if (ACFacilityManager == null)
                return null;
            return this.ACFacilityManager.GetFacilityOverviewLists(databaseApp, filter);
        }

        #endregion

        #region FacilityBooking(Charge)Overview methods -> Guiding methods
        public virtual FacilityQueryFilter GetFacilityBookingFilter()
        {
            FacilityQueryFilter filter = new FacilityQueryFilter();
            filter.SearchFrom = SearchFrom;
            filter.SearchTo = SearchTo;
            bool isFilterFBType = FilterFBType != null && FilterFBType.Value != null;
            if (isFilterFBType)
                filter.FilterFBTypeValue = (short)FilterFBType.Value;
            return filter;
        }

        public virtual void CleanMovements()
        {
            _SelectedFacilityBookingOverview = null;
            _SelectedFacilityBookingChargeOverview = null;
            _FacilityBookingOverviewList = null;
            _FacilityBookingChargeOverviewList = null;
            OnPropertyChanged("FacilityBookingOverviewList");
            OnPropertyChanged("FacilityBookingChargeOverviewList");
        }

        [ACMethodCommand(FacilityBooking.ClassName, "en{'Refresh'}de{'Aktualisiere'}", 701)]
        public virtual void RefreshMovements()
        {
            if (!IsEnabledRefreshMovements())
                return;
            if (BackgroundWorker.IsBusy)
                return;
            _FacilityBookingOverviewList = null;
            _FacilityBookingChargeOverviewList = null;
            BackgroundWorker.RunWorkerAsync("DoFacilityBookingSearch");
            ShowDialog(this, DesignNameProgressBar);
        }

        public virtual bool IsEnabledRefreshMovements()
        {
            return !BackgroundWorker.IsBusy && SearchFrom != null && SearchTo != null;
        }

        #endregion

        #endregion

        #endregion

        #region Background-Worker
        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();
            switch (command)
            {
                case "DoFacilityBookingSearch":
                    DoFacilityBookingSearch();
                    break;
            }
        }

        public virtual void DoFacilityBookingSearch()
        {
            FacilityQueryFilter filter = GetFacilityBookingFilter();
            using (DatabaseApp dbApp = new DatabaseApp())
            {
                Dictionary<FacilityBookingOverview, List<FacilityBookingChargeOverview>> fbList = GetFacilityOverviewLists(dbApp, filter);
                _FacilityBookingOverviewList = fbList.Keys.ToList();
                if (GroupByMaterial)
                    _FacilityBookingOverviewList = GroupFacilityBookingOverviewList(_FacilityBookingOverviewList);
                _FacilityBookingChargeOverviewList = fbList.SelectMany(c => c.Value).ToList();
                OnFacilityBookingSearchSum();
            }
        }

        public virtual void OnFacilityBookingSearchSum()
        {
            if (FacilityBookingOverviewList != null)
            {
                double sum = 0.0;
                foreach (var fb in FacilityBookingOverviewList)
                {
                    sum += fb.InwardQuantityUOM - fb.OutwardQuantityUOM;
                    fb.InOutSumUOM = sum;
                }
            }

            if (FacilityBookingChargeOverviewList != null)
            {
                double sum = 0.0;
                foreach (var fb in FacilityBookingChargeOverviewList)
                {
                    sum += fb.InwardQuantityUOM - fb.OutwardQuantityUOM;
                    fb.InOutSumUOM = sum;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            if (e.Error != null)
            {

            }
            else if (e.Cancelled)
            {

            }
            else
            {

            }
            CloseTopDialog();
            string command = worker.EventArgs.Argument.ToString();
            switch (command)
            {
                case "DoFacilityBookingSearch":
                    OnPropertyChanged("FacilityBookingOverviewList");
                    OnPropertyChanged("FacilityBookingChargeOverviewList");
                    break;
            }
        }

        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "RefreshMovements":
                    RefreshMovements();
                    return true;
                case "IsEnabledRefreshMovements":
                    result = IsEnabledRefreshMovements();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }



        #endregion

    }
}
