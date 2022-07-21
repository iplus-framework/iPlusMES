using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Open postings'}de{'Offene Buchungen'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOProdOrderOpenPositions : ACBSOvb
    {
        #region constants
        public const int QueryTimeOutMinutes = 4;
        public const string BGWorkerMehtod_DoSearchOpenPosition = @"DoSearchOpenPosition";

        public virtual string BSOProdOrderName
        {
            get
            {
                return Const.BusinessobjectsACUrl + ACUrlHelper.Delimiter_Start + nameof(BSOProdOrder);
            }
        }

        public virtual List<string> FilterMaterialBlackListConfig
        {
            get
            {
                return null;
            }
        }
        #endregion

        #region ctor's
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSOProdOrderOpenPositions(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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
            if (!base.ACInit(startChildMode)) return false;

            _FilterEndDate = DateTime.Now.Date.AddDays(1).AddMinutes(-1);
            _FilterStartDate = _FilterEndDate.AddMonths(-1);
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        public override object Clone()
        {
            BSOProdOrderOpenPositions clone = base.Clone() as BSOProdOrderOpenPositions;
            return clone;
        }

        #endregion

        #region Properties

        private DateTime BGWorkerStartTime { get; set; }

        #region Properties -> Filter 
        private DateTime _FilterStartDate;
        [ACPropertyInfo(511, "FilterStartDate", "en{'From'}de{'Von'}")]
        public DateTime FilterStartDate
        {
            get
            {
                return _FilterStartDate;
            }
            set
            {
                if (_FilterStartDate != value)
                {
                    _FilterStartDate = value;
                    OnPropertyChanged("FilterStartDate");
                }
            }
        }

        private DateTime _FilterEndDate;
        [ACPropertyInfo(512, "FilterEndDate", "en{'to'}de{'bis'}")]
        public DateTime FilterEndDate
        {
            get
            {
                return _FilterEndDate;
            }
            set
            {
                if (_FilterEndDate != value)
                {
                    _FilterEndDate = value;
                    OnPropertyChanged("FilterEndDate");
                }
            }
        }

        private string _FilterProgramNo;
        [ACPropertyInfo(513, "FilterProgramNo", "en{'Ordernumber'}de{'Auftragsnummer'}")]
        public string FilterProgramNo
        {
            get
            {
                return _FilterProgramNo;
            }
            set
            {
                if (_FilterProgramNo != value)
                {
                    _FilterProgramNo = value;
                    OnPropertyChanged("FilterProgramNo");
                }
            }
        }

        #endregion

        #region Filter -> OpenPositionType

        ACValueItem _SelectedOpenPositionType;
        [ACPropertySelected(514, "OpenPositionType", "en{'Direction'}de{'Richtung'}")]
        public ACValueItem SelectedOpenPositionType
        {
            get
            {
                return _SelectedOpenPositionType;
            }
            set
            {
                if (_SelectedOpenPositionType != value)
                {
                    _SelectedOpenPositionType = value;
                    OnPropertyChanged("SelectedOpenPositionType");
                }
            }
        }

        public OpenPositionTypeEnum? FilterOpenPositionType
        {
            get
            {
                if (SelectedOpenPositionType == null) return null;
                return (OpenPositionTypeEnum)(short)SelectedOpenPositionType.Value;
            }
        }

        private ACValueItemList _OpenPositionTypeList;
        [ACPropertyList(515, "OpenPositionType")]
        public IEnumerable<ACValueItem> OpenPositionTypeList
        {
            get
            {
                if (_OpenPositionTypeList == null)
                {
                    _OpenPositionTypeList = new ACValueItemList("OpenPositionTypeList");
                    _OpenPositionTypeList.AddEntry((short)OpenPositionTypeEnum.ExistOutwardPreBooking, "en{'Exist Outward FacilityPreBookings'}de{'Es gibt offene Einsatzbuchungen'}");
                    _OpenPositionTypeList.AddEntry((short)OpenPositionTypeEnum.ExistInwardPreBooking, "en{'Exist Outward FacilityPreBookings'}de{'Es gibt offene Ergebnisbuchungen'}");
                    _OpenPositionTypeList.AddEntry((short)OpenPositionTypeEnum.MissingOutwardBooking, "en{'No Outward Bookings'}de{'Keine Einsatzbuchung'}");
                    _OpenPositionTypeList.AddEntry((short)OpenPositionTypeEnum.MissingInwardBooking, "en{'No Inward Bookings'}de{'Keine Ergebnisbuchung'}");
                }
                return _OpenPositionTypeList;
            }
        }

        #endregion

        #region Properties -> OpenPosition
        private ProductionOpenPosition _SelectedOpenPosition;
        /// <summary>
        /// Selected property for ProductionOpenPositions
        /// </summary>
        /// <value>The selected OpenPosition</value>
        [ACPropertySelected(500, "OpenPosition", "en{'TODO: OpenPosition'}de{'TODO: OpenPosition'}")]
        public ProductionOpenPosition SelectedOpenPosition
        {
            get
            {
                return _SelectedOpenPosition;
            }
            set
            {
                if (_SelectedOpenPosition != value)
                {
                    _SelectedOpenPosition = value;
                    OnPropertyChanged("SelectedOpenPosition");

                    CleanBookings();
                    if (value != null)
                        LoadBookings(value.ProdOrderPartslistPosID);
                }
            }
        }


        private List<ProductionOpenPosition> _OpenPositionList;
        /// <summary>
        /// List property for ProductionOpenPositions
        /// </summary>
        /// <value>The OpenPosition list</value>
        [ACPropertyList(501, "OpenPosition")]
        public List<ProductionOpenPosition> OpenPositionList
        {
            get
            {
                if (_OpenPositionList == null)
                    _OpenPositionList = new List<ProductionOpenPosition>();
                var openPositionList = _OpenPositionList.ToList();
                if (FilterOpenPositionType != null)
                {
                    switch (FilterOpenPositionType.Value)
                    {
                        case OpenPositionTypeEnum.ExistOutwardPreBooking:
                            openPositionList = openPositionList.Where(c => c.OutwardPreBookingCount > 0).ToList();
                            break;
                        case OpenPositionTypeEnum.ExistInwardPreBooking:
                            openPositionList = openPositionList.Where(c => c.InwardPreBookingCount > 0).ToList();
                            break;
                        case OpenPositionTypeEnum.MissingOutwardBooking:
                            openPositionList = openPositionList.Where(c => c.OutwardPreBookingCount == 0 && c.OutwardBookingCount == 0).ToList();
                            break;
                        case OpenPositionTypeEnum.MissingInwardBooking:
                            openPositionList = openPositionList.Where(c => c.InwardPreBookingCount == 0 && c.InwardBookingCount == 0).ToList();
                            break;
                        default:
                            break;
                    }
                }
                return openPositionList;
            }
        }

        #endregion

        #region Properties -> OpenPosition -> pos

        private ProdOrderPartslistPos _Pos;

        [ACPropertyInfo(502, "Pos")]
        public ProdOrderPartslistPos Pos
        {
            get
            {
                return _Pos;
            }
            set
            {
                if (_Pos != value)
                {
                    _Pos = value;
                    OnPropertyChanged("Pos");
                }
            }
        }

        #endregion

        #region Properties -> OpenPosition-> Bookings

        #region Properties -> OpenPosition-> Bookings -> Inward

        #region Properties -> OpenPosition-> Bookings -> Inward -> InwardPreBooking

        private FacilityPreBooking _SelectedInwardPreBooking;
        /// <summary>
        /// Selected property for FacilityPreBooking
        /// </summary>
        /// <value>The selected InwardPreBooking</value>
        [ACPropertySelected(503, "InwardPreBooking", "en{'TODO: InwardPreBooking'}de{'TODO: InwardPreBooking'}")]
        public FacilityPreBooking SelectedInwardPreBooking
        {
            get
            {
                return _SelectedInwardPreBooking;
            }
            set
            {
                if (_SelectedInwardPreBooking != value)
                {
                    _SelectedInwardPreBooking = value;
                    OnPropertyChanged("SelectedInwardPreBooking");
                }
            }
        }

        private List<FacilityPreBooking> _InwardPreBookingList;
        /// <summary>
        /// List property for FacilityPreBooking
        /// </summary>
        /// <value>The InwardPreBooking list</value>
        [ACPropertyList(504, "InwardPreBooking")]
        public List<FacilityPreBooking> InwardPreBookingList
        {
            get
            {
                if (_InwardPreBookingList == null)
                    _InwardPreBookingList = new List<FacilityPreBooking>();
                return _InwardPreBookingList;
            }
        }

        #endregion

        #region Properties -> OpenPosition-> Bookings -> Inward -> InwardBooking
        private FacilityBooking _SelectedInwardBooking;
        /// <summary>
        /// Selected property for FacilityBooking
        /// </summary>
        /// <value>The selected InwardBooking</value>
        [ACPropertySelected(505, "InwardBooking", "en{'TODO: InwardBooking'}de{'TODO: InwardBooking'}")]
        public FacilityBooking SelectedInwardBooking
        {
            get
            {
                return _SelectedInwardBooking;
            }
            set
            {
                if (_SelectedInwardBooking != value)
                {
                    _SelectedInwardBooking = value;
                    OnPropertyChanged("SelectedInwardBooking");
                }
            }
        }

        private List<FacilityBooking> _InwardBookingList;
        /// <summary>
        /// List property for FacilityBooking
        /// </summary>
        /// <value>The InwardBooking list</value>
        [ACPropertyList(506, "InwardBooking")]
        public List<FacilityBooking> InwardBookingList
        {
            get
            {
                if (_InwardBookingList == null)
                    _InwardBookingList = new List<FacilityBooking>();
                return _InwardBookingList;
            }
        }

        #endregion

        #endregion

        #region Properties -> OpenPosition-> Bookings -> Outward

        #region  Properties -> OpenPosition-> Bookings -> Outward -> OutwardPreBooking
        private FacilityPreBooking _SelectedOutwardPreBooking;
        /// <summary>
        /// Selected property for FacilityPreBooking
        /// </summary>
        /// <value>The selected OutwardPreBooking</value>
        [ACPropertySelected(507, "OutwardPreBooking", "en{'TODO: OutwardPreBooking'}de{'TODO: OutwardPreBooking'}")]
        public FacilityPreBooking SelectedOutwardPreBooking
        {
            get
            {
                return _SelectedOutwardPreBooking;
            }
            set
            {
                if (_SelectedOutwardPreBooking != value)
                {
                    _SelectedOutwardPreBooking = value;
                    OnPropertyChanged("SelectedOutwardPreBooking");
                }
            }
        }

        private List<FacilityPreBooking> _OutwardPreBookingList;
        /// <summary>
        /// List property for FacilityPreBooking
        /// </summary>
        /// <value>The OutwardPreBooking list</value>
        [ACPropertyList(508, "OutwardPreBooking")]
        public List<FacilityPreBooking> OutwardPreBookingList
        {
            get
            {
                if (_OutwardPreBookingList == null)
                    _OutwardPreBookingList = new List<FacilityPreBooking>();
                return _OutwardPreBookingList;
            }
        }

        #endregion

        #region Properties -> OpenPosition-> Bookings -> Outward -> OutwardBooking
        private FacilityBooking _SelectedOutwardBooking;
        /// <summary>
        /// Selected property for FacilityBooking
        /// </summary>
        /// <value>The selected OutwardBooking</value>
        [ACPropertySelected(509, "OutwardBooking", "en{'TODO: OutwardBooking'}de{'TODO: OutwardBooking'}")]
        public FacilityBooking SelectedOutwardBooking
        {
            get
            {
                return _SelectedOutwardBooking;
            }
            set
            {
                if (_SelectedOutwardBooking != value)
                {
                    _SelectedOutwardBooking = value;
                    OnPropertyChanged("SelectedOutwardBooking");
                }
            }
        }

        private List<FacilityBooking> _OutwardBookingList;
        /// <summary>
        /// List property for FacilityBooking
        /// </summary>
        /// <value>The OutwardBooking list</value>
        [ACPropertyList(510, "OutwardBooking")]
        public List<FacilityBooking> OutwardBookingList
        {
            get
            {
                if (_OutwardBookingList == null)
                    _OutwardBookingList = new List<FacilityBooking>();
                return _OutwardBookingList;
            }
        }

        #endregion

        #endregion

        #endregion

        #region Properties -> Messages
        public void SendMessage(Msg msg)
        {
            MsgList.Add(msg);
            OnPropertyChanged("MsgList");
        }

        /// <summary>
        /// The _ current MSG
        /// </summary>
        Msg _CurrentMsg;
        /// <summary>
        /// Gets or sets the current MSG.
        /// </summary>
        /// <value>The current MSG.</value>
        [ACPropertyCurrent(516, "Message", "en{'Message'}de{'Meldung'}")]
        public Msg CurrentMsg
        {
            get
            {
                return _CurrentMsg;
            }
            set
            {
                _CurrentMsg = value;
                OnPropertyChanged("CurrentMsg");
            }
        }

        private ObservableCollection<Msg> msgList;
        /// <summary>
        /// Gets the MSG list.
        /// </summary>
        /// <value>The MSG list.</value>
        [ACPropertyList(517, "Message", "en{'Messagelist'}de{'Meldungsliste'}")]
        public ObservableCollection<Msg> MsgList
        {
            get
            {
                if (msgList == null)
                    msgList = new ObservableCollection<Msg>();
                return msgList;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Methods -> ACMehtod
        /// <summary>
        /// Searches the delivery note.
        /// </summary>
        [ACMethodInfo("Search", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (!IsEnabledSearch()) return;
            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_DoSearchOpenPosition);
            MsgList.Clear();
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledSearch()
        {
            return !BackgroundWorker.IsBusy;
        }

        [ACMethodInfo("Search", "en{'Filter'}de{'Filter'}", (short)MISort.Search)]
        public void Filter()
        {
            if (!IsEnabledFilter()) return;
            OnPropertyChanged("OpenPositionList");
        }

        public bool IsEnabledFilter()
        {
            return _OpenPositionList != null && _OpenPositionList.Any();
        }

        #endregion

        #region Methods -> Bookings

        public void CleanBookings()
        {
            _Pos = null;

            _SelectedInwardPreBooking = null;
            _InwardPreBookingList = null;
            _SelectedInwardBooking = null;
            _InwardBookingList = null;

            _SelectedOutwardPreBooking = null;
            _OutwardPreBookingList = null;
            _SelectedOutwardBooking = null;
            _OutwardBookingList = null;

            OnPropertyChanged("Pos");

            OnPropertyChanged("InwardPreBookingList");
            OnPropertyChanged("InwardBookingList");
            OnPropertyChanged("OutwardPreBookingList");
            OnPropertyChanged("OutwardBookingList");
        }

        public void LoadBookings(Guid prodOrderPartslistPosID)
        {
            Pos = DatabaseApp.ProdOrderPartslistPos.FirstOrDefault(c => c.ProdOrderPartslistPosID == prodOrderPartslistPosID);
            if (Pos != null)
            {
                _InwardPreBookingList = Pos.FacilityPreBooking_ProdOrderPartslistPos.OrderBy(c => c.FacilityPreBookingNo).ToList();
                SelectedInwardPreBooking = _InwardPreBookingList.FirstOrDefault();

                _InwardBookingList = Pos.FacilityBooking_ProdOrderPartslistPos.OrderBy(c => c.FacilityBookingNo).ToList();
                SelectedInwardBooking = _InwardBookingList.FirstOrDefault();

                _OutwardPreBookingList = Pos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.SelectMany(c => c.FacilityPreBooking_ProdOrderPartslistPosRelation).OrderBy(c => c.FacilityPreBookingNo).ToList();
                SelectedOutwardPreBooking = _OutwardPreBookingList.FirstOrDefault();

                _OutwardBookingList = Pos.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.SelectMany(c => c.FacilityBooking_ProdOrderPartslistPosRelation).OrderBy(c => c.FacilityBookingNo).ToList();
                SelectedOutwardBooking = _OutwardBookingList.FirstOrDefault();

                OnPropertyChanged("InwardPreBookingList");
                OnPropertyChanged("InwardBookingList");
                OnPropertyChanged("OutwardPreBookingList");
                OnPropertyChanged("OutwardBookingList");
            }
        }

        #endregion

        #region Methods -> ShowOrder

        // SelectedOpenPosition
        [ACMethodInteraction("ShowOrder", "en{'View order'}de{'Auftrag anzeigen'}", 501, true, "SelectedOpenPosition")]
        public void ShowOrderSelectedOpenPosition()
        {
            if (!IsEnabledShowOrderSelectedOpenPosition()) return;
            CallBSOProdOrder(
                Pos.ProdOrderPartslist.ProdOrder.ProgramNo,
                Pos.ProdOrderPartslist.ProdOrderPartslistID,
                Pos.ParentProdOrderPartslistPosID,
                Pos.ProdOrderPartslistPosID,
                null,
                null);
        }

        public bool IsEnabledShowOrderSelectedOpenPosition()
        {
            return Pos != null && SelectedOpenPosition != null;
        }

        // SelectedInwardPreBooking
        [ACMethodInteraction("ShowOrder", "en{'View order'}de{'Auftrag anzeigen'}", 502, true, "SelectedInwardPreBooking")]
        public void ShowOrderSelectedInwardPreBooking()
        {
            if (!IsEnabledShowOrderSelectedInwardPreBooking()) return;
            CallBSOProdOrder(
                Pos.ProdOrderPartslist.ProdOrder.ProgramNo,
                Pos.ProdOrderPartslist.ProdOrderPartslistID,
                Pos.ParentProdOrderPartslistPosID,
                Pos.ProdOrderPartslistPosID,
                 SelectedInwardPreBooking.FacilityPreBookingID,
                null);
        }

        public bool IsEnabledShowOrderSelectedInwardPreBooking()
        {
            return SelectedInwardPreBooking != null;
        }

        // SelectedInwardBooking
        [ACMethodInteraction("ShowOrder", "en{'View order'}de{'Auftrag anzeigen'}", 503, true, "SelectedInwardBooking")]
        public void ShowOrderSelectedInwardBooking()
        {
            if (!IsEnabledShowOrderSelectedInwardBooking()) return;
            CallBSOProdOrder(
                Pos.ProdOrderPartslist.ProdOrder.ProgramNo,
                Pos.ProdOrderPartslist.ProdOrderPartslistID,
                Pos.ParentProdOrderPartslistPosID,
                Pos.ProdOrderPartslistPosID,
                null,
                SelectedInwardBooking.FacilityBookingID);
        }

        public bool IsEnabledShowOrderSelectedInwardBooking()
        {
            return SelectedInwardBooking != null;
        }

        // SelectedOutwardPreBooking
        [ACMethodInteraction("ShowOrder", "en{'View order'}de{'Auftrag anzeigen'}", 504, true, "SelectedOutwardPreBooking")]
        public void ShowOrderSelectedOutwardPreBooking()
        {
            if (!IsEnabledShowOrderSelectedOutwardPreBooking()) return;
            CallBSOProdOrder(
                Pos.ProdOrderPartslist.ProdOrder.ProgramNo,
                Pos.ProdOrderPartslist.ProdOrderPartslistID,
                Pos.ParentProdOrderPartslistPosID,
                Pos.ProdOrderPartslistPosID,
                 SelectedOutwardPreBooking.FacilityPreBookingID,
                null);
        }

        public bool IsEnabledShowOrderSelectedOutwardPreBooking()
        {
            return SelectedOutwardPreBooking != null;
        }

        // SelectedOutwardBooking
        [ACMethodInteraction("ShowOrder", "en{'View order'}de{'Auftrag anzeigen'}", 505, true, "SelectedOutwardBooking")]
        public void ShowOrderSelectedOutwardBooking()
        {
            if (!IsEnabledShowOrderSelectedOutwardBooking()) return;
            CallBSOProdOrder(
                Pos.ProdOrderPartslist.ProdOrder.ProgramNo,
                Pos.ProdOrderPartslist.ProdOrderPartslistID,
                Pos.ParentProdOrderPartslistPosID,
                Pos.ProdOrderPartslistPosID,
                null,
                SelectedOutwardBooking.FacilityBookingID);
        }

        public bool IsEnabledShowOrderSelectedOutwardBooking()
        {
            return SelectedOutwardBooking != null;
        }

        #endregion

        #endregion

        #region BackgroundWorker

        #region BackgroundWorker -> BackgroundWorker
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
            worker.ProgressInfo.AddSubTask(command, 0, 100);
            worker.ProgressInfo.ReportProgress(null, null, string.Format("Running {0}...", command));
            worker.ProgressInfo.ReportProgress(command, 10, null);
            BGWorkerStartTime = DateTime.Now;
            switch (command)
            {
                case BGWorkerMehtod_DoSearchOpenPosition:
                    e.Result = DoSearchOpenPosition(worker, FilterStartDate, FilterEndDate, FilterProgramNo);
                    break;
            }
        }

        public override void BgWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ACBackgroundWorker worker = sender as ACBackgroundWorker;
            base.BgWorkerCompleted(sender, e);
            CloseWindow(this, DesignNameProgressBar);
            string command = worker.EventArgs.Argument.ToString();
            if (e.Cancelled)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Info, Message = string.Format(@"Operation {0} canceled by user!", command) });
            }
            else if (e.Error != null)
            {
                SendMessage(new Msg() { MessageLevel = eMsgLevel.Error, Message = string.Format(@"Error by running {0}! Message:{1}", command, e.Error.Message) });
            }
            else
            {
                switch (command)
                {
                    case BGWorkerMehtod_DoSearchOpenPosition:
                        List<ProductionOpenPosition> productionOpenPositions = e.Result as List<ProductionOpenPosition>;
                        DoSearchOpenPositionFinish(productionOpenPositions);
                        TimeSpan ts = DateTime.Now - BGWorkerStartTime;
                        Msg msgInfo = new Msg()
                        {
                            MessageLevel = eMsgLevel.Info,
                            ACIdentifier = BGWorkerMehtod_DoSearchOpenPosition,
                            Message = string.Format("Operation {0} completed for {1:%m}:{1:%s} ({2}ms)!", BGWorkerMehtod_DoSearchOpenPosition, ts, ts.TotalMilliseconds.ToString("#0.00"))
                        };
                        SendMessage(msgInfo);
                        break;
                }
            }
        }

        #endregion

        #region BackgroundWorker -> BGWorker mehtods -> Methods for call

        private List<ProductionOpenPosition> DoSearchOpenPosition(ACBackgroundWorker worker, DateTime startTime, DateTime endTime, string programNo)
        {
            List<ProductionOpenPosition> productionOpenPositions = new List<ProductionOpenPosition>();
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                databaseApp.CommandTimeout = QueryTimeOutMinutes * 60;
                productionOpenPositions = QueryPositionList(databaseApp, startTime, endTime, programNo).ToList();
                int sn = 0;
                foreach (ProductionOpenPosition position in productionOpenPositions)
                    position.Sn = ++sn;
                worker.ProgressInfo.ReportProgress(null, null, string.Format("Running {0} ... query complete!", BGWorkerMehtod_DoSearchOpenPosition));
                worker.ProgressInfo.ReportProgress(BGWorkerMehtod_DoSearchOpenPosition, 60);
            }
            return productionOpenPositions;
        }

        private void DoSearchOpenPositionFinish(List<ProductionOpenPosition> productionOpenPositions)
        {
            _OpenPositionList = productionOpenPositions;
            OnPropertyChanged("OpenPositionList");

            OnPropertyChanged("PositionMissingBookingList");
            OnPropertyChanged("PositionExistPreBookingList");
            OnPropertyChanged("PositionNoBookingList");
        }

        #endregion

        #region Methods -> call BSOProdOrder

        public void CallBSOProdOrder(string programNo, Guid prodOrderPartslistID, Guid? parentProdOrderPartslistPosID, Guid prodOrderPartslistPosID, Guid? facilityPreBookingID, Guid? facilityBookingID)
        {

            PAShowDlgManagerVB manager = PAShowDlgManagerVB.GetServiceInstance(Root as ACComponent) as PAShowDlgManagerVB;
            PAOrderInfo pAOrderInfo = new PAOrderInfo();

            // programNo
            ProdOrder prodOrder = DatabaseApp.ProdOrder.FirstOrDefault(c => c.ProgramNo == programNo);
            pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
            {
                EntityName = ProdOrder.ClassName,
                EntityID = prodOrder.ProdOrderID
            });

            // prodOrderPartslistID
            pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
            {
                EntityName = ProdOrderPartslist.ClassName,
                EntityID = prodOrderPartslistID
            });

            // prodOrderPartslistPosID
            pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
            {
                EntityName = ProdOrderPartslistPos.ClassName,
                EntityID = prodOrderPartslistPosID
            });

            // facilityPreBookingID
            if (facilityPreBookingID != null)
            {
                pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                {
                    EntityName = FacilityPreBooking.ClassName,
                    EntityID = facilityPreBookingID ?? Guid.Empty
                });
            }

            // facilityBookingID
            if (facilityBookingID != null)
            {
                pAOrderInfo.Entities.Add(new PAOrderInfoEntry()
                {
                    EntityName = FacilityBooking.ClassName,
                    EntityID = facilityBookingID ?? Guid.Empty
                });
            }

            manager.ShowDialogOrder(this, pAOrderInfo);
        }
        #endregion

        #endregion

        #region Query

        private IQueryable<ProductionOpenPosition> QueryPositionList(DatabaseApp databaseApp, DateTime startTime, DateTime endTime, string programNo)
        {
            string[] materialBlackList = new string[] { };
            if (FilterMaterialBlackListConfig != null && FilterMaterialBlackListConfig.Any())
                materialBlackList = FilterMaterialBlackListConfig.ToArray();

            if (string.IsNullOrEmpty(programNo))
                programNo = null;
            var queryRelation =
                databaseApp
                .ProdOrderPartslistPosRelation
                .GroupJoin(databaseApp.FacilityBooking, rel => rel.ProdOrderPartslistPosRelationID, outwardFb => outwardFb.ProdOrderPartslistPosRelationID, (rel, outwardFb) => new { rel, outwardFb })
                .GroupJoin(databaseApp.FacilityPreBooking, tmp => tmp.rel.ProdOrderPartslistPosRelationID, preFb => preFb.ProdOrderPartslistPosRelationID, (tmp, preFb) => new { tmp.rel, tmp.outwardFb, preFb });

            var query =
                databaseApp
                .ProdOrder
                .Where(c =>
                        (programNo != null && c.ProgramNo == programNo)
                        ||
                        (programNo == null && (c.InsertDate >= startTime && c.InsertDate < endTime))
                    )
                .Join(databaseApp.ProdOrderPartslist, po => po.ProdOrderID, pl => pl.ProdOrderID, (po, pl) => new { po, pl })
                .Join(databaseApp.ProdOrderPartslistPos, tmp => tmp.pl.ProdOrderPartslistID, pos => pos.ProdOrderPartslistID, (tmp, pos) => new { tmp.po, tmp.pl, pos })
                .GroupJoin(queryRelation, tmp => tmp.pos.ProdOrderPartslistPosID, rel => rel.rel.TargetProdOrderPartslistPosID, (tmp, rel) => new { tmp.po, tmp.pl, tmp.pos, rel })
                .GroupJoin(databaseApp.FacilityBooking, tmp => tmp.pos.ProdOrderPartslistPosID, inwardFb => inwardFb.ProdOrderPartslistPosID, (tmp, inwardFb) => new { tmp.po, tmp.pl, tmp.pos, tmp.rel, inwardFb })
                .GroupJoin(databaseApp.FacilityPreBooking, tmp => tmp.pos.ProdOrderPartslistPosID, inwardPreFb => inwardPreFb.ProdOrderPartslistPosID, (tmp, inwardPreFb) => new { tmp.po, tmp.pl, tmp.pos, tmp.rel, tmp.inwardFb, inwardPreFb })
                .Where(c =>
                        c.pos.ParentProdOrderPartslistPosID != null
                    && (!materialBlackList.Any() || !materialBlackList.Contains(c.pos.Material.MaterialNo))
                    &&

                        (
                            c.inwardPreFb.Any()
                            || c.rel.SelectMany(x => x.preFb).Any()
                            || !c.inwardFb.Any()
                            || !c.rel.SelectMany(x => x.outwardFb).Any()
                        )
                 )
                .Select(c => new ProductionOpenPosition()
                {
                    // Position
                    Sn = 0,
                    ProdOrderPartslistPosID = c.pos.ProdOrderPartslistPosID,
                    BatchNo = c.pos.ProdOrderBatch.ProdOrderBatchNo,
                    InsertDate = (c.inwardFb.Any() ? c.inwardFb.Select(x => x.InsertDate).DefaultIfEmpty().FirstOrDefault() : c.pos.InsertDate),
                    PosSequence = c.pos.Sequence,
                    ParentPosSequence = c.pos.ProdOrderPartslistPos1_ParentProdOrderPartslistPos.Sequence,
                    PartslistSequence = c.pl.Sequence,
                    ProgramNo = c.po.ProgramNo,

                    // Outward
                    OutwardMaterialNo = c.rel.Select(x => x.rel.SourceProdOrderPartslistPos).Where(x => x.BasedOnPartslistPosID != null).Select(x => x.Material.MaterialNo).DefaultIfEmpty().FirstOrDefault(),
                    OutwardMaterialName = c.rel.Select(x => x.rel.SourceProdOrderPartslistPos).Where(x => x.BasedOnPartslistPosID != null).Select(x => x.Material.MaterialName1).DefaultIfEmpty().FirstOrDefault(),
                    OutwardTargetQuantityUOM = c.rel.Select(x => x.rel.TargetQuantityUOM).DefaultIfEmpty().Sum(),
                    OutwardActualQuantityUOM = c.rel.Select(x => x.rel.ActualQuantityUOM).DefaultIfEmpty().Sum(),
                    OutwardPreBookingCount = (c.rel.SelectMany(x => x.preFb).Any() ? c.rel.SelectMany(x => x.preFb).Count() : 0),
                    OutwardBookingCount = (c.rel.SelectMany(x => x.outwardFb).Any() ? c.rel.SelectMany(x => x.outwardFb).Count() : 0),

                    // Inward
                    InwardMaterialNo = c.pos.Material.MaterialNo,
                    InwardMaterialName = c.pos.Material.MaterialName1,
                    InwardTargetQuantityUOM = c.pos.TargetQuantityUOM,
                    InwardActualQuantityUOM = c.pos.ActualQuantityUOM,
                    InwardPreBookingCount = (c.inwardPreFb.Any() ? c.inwardPreFb.Count() : 0),
                    InwardBookingCount = (c.inwardFb.Any() ? c.inwardFb.Count() : 0)

                })
                .OrderBy(c => c.ProgramNo)
                .ThenBy(c => c.ParentPosSequence)
                .ThenBy(c => c.PosSequence);
            return query;
        }

        #endregion
    }

    public enum OpenPositionTypeEnum
    {
        ExistOutwardPreBooking,
        ExistInwardPreBooking,
        MissingOutwardBooking,
        MissingInwardBooking
    }
}
