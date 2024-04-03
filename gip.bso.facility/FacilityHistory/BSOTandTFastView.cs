using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace gip.bso.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'BSOTandTFastView'}de{'TBSOTandTFastView'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOTandTFastView : ACBSOvb
    {
        #region const

        public const string BGWorkerMehtod_SearchFacilityCharge = "SearchFacilityCharge";
        public const string BGWorkerMehtod_SearchFacilityLot = "SearchFacilityLot";

        #endregion

        #region c´tors

        /// <summary>
        /// Konstruktor für ACComponent
        /// (Gleiche Signatur, wie beim ACGenericObject)
        /// </summary>
        /// <param name="acType">ACType anhand dessen die Methoden, Properties und Designs initialisiert werden</param>
        /// <param name="content">Inhalt
        /// Bei Model- oder BSO immer gleich ACClass
        /// Bei WF immer WorkOrderWF</param>
        /// <param name="parentACObject">Lebende ACComponent-Instanz</param>
        /// <param name="parameter">Parameter je nach Ableitungsimplementierung</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOTandTFastView(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
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

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Properties

        #region Properties -> Filter

        /// <summary>
        /// Source Property: 
        /// </summary>
        private FacilityCharge _FilterFacilityCharge;
        [ACPropertyInfo(999, "FilterFacilityCharge", "en{'TODO:FilterFacilityCharge'}de{'TODO:FilterFacilityCharge'}")]
        public FacilityCharge FilterFacilityCharge
        {
            get
            {
                return _FilterFacilityCharge;
            }
            set
            {
                if (_FilterFacilityCharge != value)
                {
                    _FilterFacilityCharge = value;
                    OnPropertyChanged(nameof(FilterFacilityCharge));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private FacilityLot _FilterFacilityLot;
        [ACPropertyInfo(999, "FilterFacilityLot", "en{'TODO:FilterFacilityLot'}de{'TODO:FilterFacilityLot'}")]
        public FacilityLot FilterFacilityLot
        {
            get
            {
                return _FilterFacilityLot;
            }
            set
            {
                if (_FilterFacilityLot != value)
                {
                    _FilterFacilityLot = value;
                    OnPropertyChanged(nameof(FilterFacilityLot));
                }
            }
        }

        #endregion

        #region Properties -> Messages

        public void SendMessage(object result)
        {
            Msg msg = result as Msg;
            if (msg != null)
            {
                SendMessage(msg);
            }
        }

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(528, "Message", "en{'Message'}de{'Meldung'}")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged(nameof(CurrentMsg));
            }
        }

        private ObservableCollection<Msg> msgList;
        /// <summary>
        /// Gets the MSG list.
        /// </summary>
        /// <value>The MSG list.</value>
        [ACPropertyList(529, "Message", "en{'Messagelist'}de{'Meldungsliste'}")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged(nameof(MsgList));
        }

        public void ClearMessages()
        {
            MsgList.Clear();
            OnPropertyChanged(nameof(MsgList));
        }
        #endregion

        #region Properties -> FastView (TandTFastViewModel)


        private TandTFastViewModel _SelectedFastView;
        /// <summary>
        /// Selected property for TandTFastViewModel
        /// </summary>
        /// <value>The selected FastView</value>
        [ACPropertySelected(9999, "PropertyGroupName", "en{'TODO: FastView'}de{'TODO: FastView'}")]
        public TandTFastViewModel SelectedFastView
        {
            get
            {
                return _SelectedFastView;
            }
            set
            {
                if (_SelectedFastView != value)
                {
                    _SelectedFastView = value;
                    OnPropertyChanged(nameof(SelectedFastView));
                }
            }
        }

        private List<TandTFastViewModel> _FastViewList;
        /// <summary>
        /// List property for TandTFastViewModel
        /// </summary>
        /// <value>The FastView list</value>
        [ACPropertyList(9999, "PropertyGroupName")]
        public List<TandTFastViewModel> FastViewList
        {
            get
            {
                //if (_FastViewList == null)
                //    _FastViewList = LoadFastViewList();
                return _FastViewList;
            }
        }

        #endregion

        #endregion

        #region Methods

        public void SetFaciltiyCharge(FacilityCharge facilityCharge)
        {
            FilterFacilityCharge = facilityCharge;
            FilterFacilityLot = null;
            _FastViewList = null;
            OnPropertyChanged(nameof(FastViewList));
        }

        public void SetFaciltiyLot(FacilityLot facilityLot)
        {
            FilterFacilityLot = facilityLot;
            FilterFacilityCharge = null;
            _FastViewList = null;
            OnPropertyChanged(nameof(FastViewList));
        }

        /// <summary>
        /// Source Property: Search
        /// </summary>
        [ACMethodInfo("Search", "en{'Search'}de{'Suchen'}", 999)]
        public void Search()
        {
            if (!IsEnabledSearch())
                return;

            if (FilterFacilityCharge != null)
            {
                BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_SearchFacilityCharge);
                ShowDialog(this, DesignNameProgressBar);
            }
            else if (FilterFacilityLot != null)
            {
                BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_SearchFacilityLot);
                ShowDialog(this, DesignNameProgressBar);
            }
        }

        public bool IsEnabledSearch()
        {
            return FilterFacilityCharge != null || FilterFacilityLot != null;
        }

        [ACMethodInteraction(nameof(NavigateToProdOrder), "en{'Show ProdOrder'}de{'Zum Produktionauftrag'}", 110, true, nameof(SelectedFastView))]
        public void NavigateToProdOrder()
        {
            if (!IsEnabledNavigateToProdOrder())
                return;

            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo();
                info.Entities.Add(new PAOrderInfoEntry(nameof(ProdOrderPartslist),
                    SelectedFastView.ProdOrderPartslistID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToProdOrder()
        {
            return SelectedFastView != null;
        }

        #region Methods -> Load

        private object DoSearchFacilityCharge(Guid facilityChargeID)
        {
            return
                s_cQry_FastViewFC(DatabaseApp, facilityChargeID)
                .ToList();
        }

        private object DoSearchFacilityLot(Guid facilityLotID)
        {
            return
                s_cQry_FastViewFL(DatabaseApp, facilityLotID)
                .ToList();
        }

        #endregion

        #endregion

        #region Background worker

        /// <summary>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DoWorkEventArgs"/> instance containing the event data.</param>
        public override void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            base.BgWorkerDoWork(sender, e);
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = e.Argument.ToString();

            worker.ProgressInfo.OnlyTotalProgress = true;
            worker.ProgressInfo.AddSubTask(command, 0, 9);
            string message = Translator.GetTranslation("en{'Running {0}...'}de{'{0} läuft...'}");
            worker.ProgressInfo.ReportProgress(command, 0, string.Format(message, command));

            string updateName = Root.Environment.User.Initials;
            switch (command)
            {
                case BGWorkerMehtod_SearchFacilityCharge:
                    e.Result = DoSearchFacilityCharge(FilterFacilityCharge.FacilityChargeID);
                    break;
                case BGWorkerMehtod_SearchFacilityLot:
                    e.Result = DoSearchFacilityLot(FilterFacilityLot.FacilityLotID);
                    break;
            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);
            ClearMessages();
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            string command = worker.EventArgs.Argument.ToString();

            if (e.Cancelled)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Info, Message = string.Format(@"Operation {0} canceled by user!", command) });
            }
            if (e.Error != null)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format(@"Error by doing {0}! Message:{1}", command, e.Error.Message) });
            }
            else
            {
                _FastViewList = e.Result as List<TandTFastViewModel>;
                OnPropertyChanged(nameof(FastViewList));
            }
        }

        #endregion

        #region precompiled queries
        protected static readonly Func<DatabaseApp, Guid, IEnumerable<TandTFastViewModel>> s_cQry_FastViewFC =
        EF.CompileQuery<DatabaseApp, Guid, IEnumerable<TandTFastViewModel>>(
        (ctx, facilityChargeID) =>
                ctx
                .ProdOrderPartslist

                .Include("ProdOrder")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.FacilityBookingCharge_ProdOrderPartslistPosRelation")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.FacilityBookingCharge_ProdOrderPartslistPosRelation.OutwardFacilityCharge")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.FacilityBookingCharge_ProdOrderPartslistPosRelation.OutwardFacilityCharge.FacilityLot")

                .Where(c =>
                    c.ProdOrderPartslistPos_ProdOrderPartslist
                    .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                    .SelectMany(x => x.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                    .Where(x => x.OutwardFacilityChargeID == facilityChargeID)
                    .Any()
                )
                .Select(c => new TandTFastViewModel()
                {
                    OrderNo =
                        c.ProdOrder.ProgramNo,
                    ConsumptionActualQuantity = c.ProdOrderPartslistPos_ProdOrderPartslist
                        .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                        .SelectMany(x => x.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                        .Where(x => x.OutwardFacilityChargeID == facilityChargeID)
                        .Select(x => x.OutwardQuantityUOM)
                        .DefaultIfEmpty()
                        .Sum(x => x),

                    ConsMDUnit = c.ProdOrderPartslistPos_ProdOrderPartslist
                        .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                        .SelectMany(x => x.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                        .Where(x => x.OutwardFacilityChargeID == facilityChargeID)
                        .Select(x => x.MDUnit)
                        .DefaultIfEmpty()
                        .FirstOrDefault(),

                    ProdOrderPartslistID = c.ProdOrderPartslistID,
                    ProductionDate = (c.StartDate != null ? c.StartDate.Value : c.InsertDate),
                    MaterialNo = c.Partslist.Material.MaterialNo,
                    MaterialName = c.Partslist.Material.MaterialName1,
                    TargetActualQuantityUOM = c.ActualQuantity,
                    MDUnit = c.Partslist.MDUnit,

                    FinalMaterialNo =
                                    c.ProdOrder
                                    .ProdOrderPartslist_ProdOrder
                                    .OrderByDescending(x => x.Sequence)
                                    .Select(x => x.Partslist.Material.MaterialNo)
                                    .DefaultIfEmpty()
                                    .FirstOrDefault(),
                    FinalMaterialName = c.ProdOrder
                                    .ProdOrderPartslist_ProdOrder
                                    .OrderByDescending(x => x.Sequence)
                                    .Select(x => x.Partslist.Material.MaterialName1)
                                    .DefaultIfEmpty()
                                    .FirstOrDefault(),
                    FinalMDUnit = c.ProdOrder
                                    .ProdOrderPartslist_ProdOrder
                                    .OrderByDescending(x => x.Sequence)
                                    .Select(x => x.Partslist.MDUnit)
                                    .DefaultIfEmpty()
                                    .FirstOrDefault(),
                    FinalTargetActualQuantityUOM = c.ProdOrder
                                    .ProdOrderPartslist_ProdOrder
                                    .OrderByDescending(x => x.Sequence)
                                    .Select(x => x.TargetQuantity)
                                    .DefaultIfEmpty()
                                    .FirstOrDefault()
                })

                .OrderBy(c => c.OrderNo)
        );

        protected static readonly Func<DatabaseApp, Guid, IEnumerable<TandTFastViewModel>> s_cQry_FastViewFL =
        EF.CompileQuery<DatabaseApp, Guid, IEnumerable<TandTFastViewModel>>(
        (ctx, facilityLotID) =>
                ctx
                .ProdOrderPartslist

                .Include("ProdOrder")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.FacilityBookingCharge_ProdOrderPartslistPosRelation")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.FacilityBookingCharge_ProdOrderPartslistPosRelation.OutwardFacilityCharge")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.FacilityBookingCharge_ProdOrderPartslistPosRelation.OutwardFacilityCharge.FacilityLot")


                .Where(c =>
                    c.ProdOrderPartslistPos_ProdOrderPartslist
                    .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                    .SelectMany(x => x.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                    .Where(x =>
                                x.OutwardFacilityCharge != null
                                && x.OutwardFacilityCharge.FacilityLotID != null
                                && x.OutwardFacilityCharge.FacilityLotID == facilityLotID
                    )
                    .Any()
                )

                .Select(c => new TandTFastViewModel()
                {
                    OrderNo =
                        c.ProdOrder.ProgramNo,
                    ConsumptionActualQuantity = c.ProdOrderPartslistPos_ProdOrderPartslist
                        .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                        .SelectMany(x => x.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                        .Where(x => x.OutwardFacilityChargeID != null && x.OutwardFacilityCharge.FacilityID != null && x.OutwardFacilityCharge.FacilityLotID == facilityLotID)
                        .Select(x => x.OutwardQuantityUOM)
                        .DefaultIfEmpty()
                        .Sum(x => x),

                    ConsMDUnit = c.ProdOrderPartslistPos_ProdOrderPartslist
                        .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                        .SelectMany(x => x.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                        .Where(x => x.OutwardFacilityChargeID != null && x.OutwardFacilityCharge.FacilityID != null && x.OutwardFacilityCharge.FacilityLotID == facilityLotID)
                        .Select(x => x.MDUnit)
                        .DefaultIfEmpty()
                        .FirstOrDefault(),

                    MaterialNo = c.Partslist.Material.MaterialNo,
                    MaterialName = c.Partslist.Material.MaterialName1,
                    TargetActualQuantityUOM = c.ActualQuantity,
                    MDUnit = c.Partslist.MDUnit,

                    FinalMaterialNo =
                                    c.ProdOrder
                                    .ProdOrderPartslist_ProdOrder
                                    .OrderByDescending(x => x.Sequence)
                                    .Select(x => x.Partslist.Material.MaterialNo)
                                    .DefaultIfEmpty()
                                    .FirstOrDefault(),
                    FinalMaterialName = c.ProdOrder
                                    .ProdOrderPartslist_ProdOrder
                                    .OrderByDescending(x => x.Sequence)
                                    .Select(x => x.Partslist.Material.MaterialName1)
                                    .DefaultIfEmpty()
                                    .FirstOrDefault(),
                    FinalMDUnit = c.ProdOrder
                                    .ProdOrderPartslist_ProdOrder
                                    .OrderByDescending(x => x.Sequence)
                                    .Select(x => x.Partslist.MDUnit)
                                    .DefaultIfEmpty()
                                    .FirstOrDefault(),
                    FinalTargetActualQuantityUOM = c.ProdOrder
                                    .ProdOrderPartslist_ProdOrder
                                    .OrderByDescending(x => x.Sequence)
                                    .Select(x => x.TargetQuantity)
                                    .DefaultIfEmpty()
                                    .FirstOrDefault()
                })
                .OrderBy(c => c.OrderNo)
        );

        #endregion
    }
}
