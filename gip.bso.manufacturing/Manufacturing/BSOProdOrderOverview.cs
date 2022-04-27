using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Prodorder overview'}de{'Auftrag Überblick'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOProdOrderOverview : ACBSOvb
    {
        #region const

        public const string BGWorkerMehtod_Search = "Search";

        #endregion

        #region ctor's

        public BSOProdOrderOverview(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }


        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            // Default filter values
            FilterEndDate = DateTime.Now.Date.AddDays(1);
            FilterStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month ,1);

            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            var b = base.ACDeInit(deleteACClassTask);


            return b;
        }

        #endregion

        #region Properties

        #region Properties -> Filter

        /// <summary>
        /// Source Property: 
        /// </summary>
        private DateTime? _FilterStartDate;
        [ACPropertySelected(999, "FilterStartDate", "en{'From'}de{'Von'}")]
        public DateTime? FilterStartDate
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
                    OnPropertyChanged(nameof(FilterStartDate));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private DateTime? _FilterEndDate;
        [ACPropertySelected(999, "FilterEndDate", "en{'to'}de{'Bis'}")]
        public DateTime? FilterEndDate
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
                    OnPropertyChanged(nameof(FilterEndDate));
                }
            }
        }


        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterProgramNo;
        [ACPropertySelected(999, "FilterProgramNo", "en{'Program No.'}de{'AuftragNr.'}")]
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
                    OnPropertyChanged(nameof(FilterProgramNo));
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _FilterMaterialNo;
        [ACPropertySelected(999, "FilterMaterialNo", "en{'Material'}de{'Material'}")]
        public string FilterMaterialNo
        {
            get
            {
                return _FilterMaterialNo;
            }
            set
            {
                if (_FilterMaterialNo != value)
                {
                    _FilterMaterialNo = value;
                    OnPropertyChanged("FilterMaterialNo");
                }
            }
        }

        #region Properties -> Filter -> FilterTimeFilterType (TimeFilterTypeEnum)


        public TimeFilterTypeEnum? FilterTimeFilterType
        {
            get
            {
                if (SelectedFilterTimeFilterType == null)
                    return null;
                return (TimeFilterTypeEnum)SelectedFilterTimeFilterType.Value;
            }
        }


        private ACValueItem _SelectedFilterTimeFilterType;
        /// <summary>
        /// Selected property for ACValueItem
        /// </summary>
        /// <value>The selected TimeFilterType</value>
        [ACPropertySelected(305, "FilterTimeFilterType", "en{'Time filter'}de{'Zeitfilter'}")]
        public ACValueItem SelectedFilterTimeFilterType
        {
            get
            {
                return _SelectedFilterTimeFilterType;
            }
            set
            {
                if (_SelectedFilterTimeFilterType != value)
                {
                    _SelectedFilterTimeFilterType = value;
                    OnPropertyChanged(nameof(SelectedFilterTimeFilterType));
                }
            }
        }

        private List<ACValueItem> _FilterTimeFilterTypeList;
        /// <summary>
        /// List property for ACValueItem
        /// </summary>
        /// <value>The FilterPickingState list</value>
        [ACPropertyList(306, "FilterTimeFilterType")]
        public List<ACValueItem> FilterTimeFilterTypeList
        {
            get
            {
                if (_FilterTimeFilterTypeList == null)
                    _FilterTimeFilterTypeList = LoadFilterTimeFilterTypeList();
                return _FilterTimeFilterTypeList;
            }
        }

        public ACValueItemList LoadFilterTimeFilterTypeList()
        {
            ACValueItemList list = null;
            gip.core.datamodel.ACClass enumClass = Database.ContextIPlus.GetACType(typeof(TimeFilterTypeEnum));
            if (enumClass != null && enumClass.ACValueListForEnum != null)
                list = enumClass.ACValueListForEnum;
            else
                list = new ACValueListTimeFilterTypeEnum();
            return list;
        }

        #endregion

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

        #region Properties -> OverviewProdOrderPartslist

        private OverviewProdOrderPartslist _SelectedOverviewProdOrderPartslist;
        /// <summary>
        /// Selected property for OverviewProdOrderPartslist
        /// </summary>
        /// <value>The selected OverviewProdOrderPartslist</value>
        [ACPropertySelected(9999, "OverviewProdOrderPartslist", "en{'TODO: OverviewProdOrderPartslist'}de{'TODO: OverviewProdOrderPartslist'}")]
        public OverviewProdOrderPartslist SelectedOverviewProdOrderPartslist
        {
            get
            {
                return _SelectedOverviewProdOrderPartslist;
            }
            set
            {
                if (_SelectedOverviewProdOrderPartslist != value)
                {
                    _SelectedOverviewProdOrderPartslist = value;
                    OnPropertyChanged(nameof(SelectedOverviewProdOrderPartslist));
                }
            }
        }

        private List<OverviewProdOrderPartslist> _OverviewProdOrderPartslistList;
        /// <summary>
        /// List property for OverviewProdOrderPartslist
        /// </summary>
        /// <value>The OverviewProdOrderPartslist list</value>
        [ACPropertyList(9999, "OverviewProdOrderPartslist")]
        public List<OverviewProdOrderPartslist> OverviewProdOrderPartslistList
        {
            get
            {
                return _OverviewProdOrderPartslistList;
            }
        }

        private List<OverviewProdOrderPartslist> LoadOverviewProdOrderPartslistList(DatabaseApp databaseApp, DateTime? filterProdStartDate, DateTime? filterProdEndDate,
            DateTime? filterStartBookingDate, DateTime? filterEndBookingDate, string filterProgramNo, string filterMaterialNo)
        {

            List<OverviewProdOrderPartslist> list = s_cQry_OverviewProdOrderPartslist(databaseApp, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo).ToList();
            foreach (OverviewProdOrderPartslist item in list)
                item.RestQuantityUOM = item.TargetActualQuantityUOM - item.SumComponentsActualQuantity;
            return list;
        }

        #endregion

        #region Properties -> OverviewMaterial

        private OverviewMaterial _SelectedOverviewMaterial;
        /// <summary>
        /// Selected property for OverviewMaterial
        /// </summary>
        /// <value>The selected OverviewMaterial</value>
        [ACPropertySelected(9999, "OverviewMaterial", "en{'TODO: OverviewMaterial'}de{'TODO: OverviewMaterial'}")]
        public OverviewMaterial SelectedOverviewMaterial
        {
            get
            {
                return _SelectedOverviewMaterial;
            }
            set
            {
                if (_SelectedOverviewMaterial != value)
                {
                    _SelectedOverviewMaterial = value;
                    OnPropertyChanged(nameof(SelectedOverviewMaterial));
                }
            }
        }

        private List<OverviewMaterial> _OverviewMaterialList;
        /// <summary>
        /// List property for OverviewMaterial
        /// </summary>
        /// <value>The OverviewMaterial list</value>
        [ACPropertyList(9999, "OverviewMaterial")]
        public List<OverviewMaterial> OverviewMaterialList
        {
            get
            {
                return _OverviewMaterialList;
            }
        }

        private List<OverviewMaterial> LoadOverviewMaterialList(DatabaseApp databaseApp, DateTime? filterProdStartDate, DateTime? filterProdEndDate,
            DateTime? filterStartBookingDate, DateTime? filterEndBookingDate, string filterProgramNo, string filterMaterialNo)
        {
            List<OverviewMaterial> list = s_cQry_OverviewMaterial(databaseApp, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo).ToList();
            foreach (OverviewMaterial item in list)
            {
                item.DifferenceOutwardQuantityUOM = item.SumOutwardActualQuantityUOM - item.SumOutwardTargetQuantityUOM;
                item.DifferenceInwardQuantityUOM = item.SumInwardActualQuantityUOM - item.SumInwardTargetQuantityUOM;
                item.RestQuantityUOM = item.SumInwardActualQuantityUOM - item.SumOutwardActualQuantityUOM;
            }
            return list;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Source Property: Search
        /// </summary>
        [ACMethodInfo("Search", "en{'Search'}de{'Suchen'}", 999)]
        public void Search()
        {
            if (!IsEnabledSearch())
                return;

            BackgroundWorker.RunWorkerAsync(BGWorkerMehtod_Search);
            ShowDialog(this, DesignNameProgressBar);
        }

        public bool IsEnabledSearch()
        {
            return FilterStartDate != null && FilterEndDate != null;
        }

        public Tuple<List<OverviewProdOrderPartslist>, List<OverviewMaterial>> GetSearch()
        {
            DateTime? startProdTime = FilterStartDate;
            DateTime? endProdTime = FilterEndDate;

            DateTime? startBookingTime = null;
            DateTime? endBookingTime = null;

            if (FilterTimeFilterType != null && FilterTimeFilterType == TimeFilterTypeEnum.BookingTime)
            {
                startBookingTime = FilterStartDate;
                endBookingTime = FilterEndDate;

                startProdTime = null;
                endProdTime = null;
            }

            List<OverviewProdOrderPartslist> item1 = LoadOverviewProdOrderPartslistList(DatabaseApp, startProdTime, endProdTime, startBookingTime, endBookingTime,
                FilterProgramNo, FilterMaterialNo);
            OnPropertyChanged(nameof(OverviewProdOrderPartslistList));

            List<OverviewMaterial> item2 = LoadOverviewMaterialList(DatabaseApp, startProdTime, endProdTime, startBookingTime, endBookingTime,
               FilterProgramNo, FilterMaterialNo);

            return new Tuple<List<OverviewProdOrderPartslist>, List<OverviewMaterial>>(item1, item2);
        }

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
                case BGWorkerMehtod_Search:
                    e.Result = GetSearch();
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
                switch (command)
                {
                    case BGWorkerMehtod_Search:
                        Tuple<List<OverviewProdOrderPartslist>, List<OverviewMaterial>> result = e.Result as Tuple<List<OverviewProdOrderPartslist>, List<OverviewMaterial>>;
                        if (result != null)
                        {
                            _OverviewProdOrderPartslistList = result.Item1;
                            _OverviewMaterialList = result.Item2;

                            OnPropertyChanged(nameof(OverviewProdOrderPartslistList));
                            OnPropertyChanged(nameof(OverviewMaterialList));
                        }
                        break;
                }
            }
        }

        #endregion

        #region Precompiled queries

        protected static readonly Func<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, IQueryable<OverviewProdOrderPartslist>> s_cQry_OverviewProdOrderPartslist =
        CompiledQuery.Compile<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, IQueryable<OverviewProdOrderPartslist>>(
           (ctx, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo) =>
               ctx
               .ProdOrderPartslist
               .Include("Partslist")
               .Include("Partslist.Material")
               .Include("ProdOrderPartslistPos_ProdOrderPartslist")

               .Where(c =>
                        (filterProdStartDate == null || c.StartDate >= filterProdStartDate)
                        && (filterProdEndDate == null || c.StartDate < filterProdEndDate)
                        && (filterStartBookingDate == null || c.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos).Where(x => x.InsertDate >= filterStartBookingDate).Any())
                        && (filterEndBookingDate == null || c.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos).Where(x => x.InsertDate < filterEndBookingDate).Any())
                        && (string.IsNullOrEmpty(filterProgramNo) || c.ProdOrder.ProgramNo.Contains(filterProgramNo))
                        && (string.IsNullOrEmpty(filterMaterialNo) || c.Partslist.Material.MaterialNo.Contains(filterMaterialNo) || c.Partslist.Material.MaterialName1.Contains(filterMaterialNo))
               )

               .Select(c => new OverviewProdOrderPartslist()
               {
                   OrderNo = c.ProdOrder.ProgramNo,
                   MaterialNo = c.Partslist.Material.MaterialNo,
                   MaterialName = c.Partslist.Material.MaterialName1,
                   TargetInwardQuantityUOM = c.TargetQuantity,
                   TargetActualQuantityUOM = c.ActualQuantity,
                   DifferenceQuantityUOM = c.ActualQuantity - c.TargetQuantity,
                   SumComponentsActualQuantity = c.ProdOrderPartslistPos_ProdOrderPartslist.Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot).Select(x => x.ActualQuantityUOM).DefaultIfEmpty().Sum(),
                   RestQuantityUOM = 0
               })
               .OrderBy(c => c.OrderNo)
            );

        protected static readonly Func<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, IQueryable<OverviewMaterial>> s_cQry_OverviewMaterial =
        CompiledQuery.Compile<DatabaseApp, DateTime?, DateTime?, DateTime?, DateTime?, string, string, IQueryable<OverviewMaterial>>(
            (ctx, filterProdStartDate, filterProdEndDate, filterStartBookingDate, filterEndBookingDate, filterProgramNo, filterMaterialNo) =>
                ctx
                .ProdOrderPartslist
                .Include("Partslist")
                .Include("Partslist.Material")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.FacilityBooking_ProdOrderPartslistPos")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos")
                .Include("ProdOrderPartslistPos_ProdOrderPartslist.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos.FacilityBooking_ProdOrderPartslistPosRelation")

                .Where(c =>
                    (filterProdStartDate == null || c.StartDate >= filterProdStartDate)
                    && (filterProdEndDate == null || c.StartDate < filterProdEndDate)
                    && (filterStartBookingDate == null || c.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos).Where(x => x.InsertDate >= filterStartBookingDate).Any())
                    && (filterEndBookingDate == null || c.ProdOrderPartslistPos_ProdOrderPartslist.SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos).Where(x => x.InsertDate < filterEndBookingDate).Any())
                    && (string.IsNullOrEmpty(filterProgramNo) || c.ProdOrder.ProgramNo.Contains(filterProgramNo))
                    && (string.IsNullOrEmpty(filterMaterialNo) || c.Partslist.Material.MaterialNo.Contains(filterMaterialNo) || c.Partslist.Material.MaterialName1.Contains(filterMaterialNo))
                )

                .GroupBy(c => new { c.Partslist.Material.MaterialNo, c.Partslist.Material.MaterialName1 })
                .Select(c => new OverviewMaterial()
                {
                    MaterialNo = c.Key.MaterialNo,
                    MaterialName = c.Key.MaterialName1,
                    SumOutwardTargetQuantityUOM =
                                    c.SelectMany(x => x.ProdOrderPartslistPos_ProdOrderPartslist)
                                    .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                                    .Select(x => x.TargetQuantityUOM)
                                    .DefaultIfEmpty()
                                    .Sum(),
                    SumOutwardActualQuantityUOM =
                                    c.SelectMany(x => x.ProdOrderPartslistPos_ProdOrderPartslist)
                                    .SelectMany(x => x.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                                    .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPosRelation)
                                    .Select(x => x.OutwardQuantity)
                                    .DefaultIfEmpty()
                                    .Sum(),
                    DifferenceOutwardQuantityUOM = 0,
                    SumInwardTargetQuantityUOM =
                                    c.SelectMany(x => x.ProdOrderPartslistPos_ProdOrderPartslist)
                                    .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern)
                                    .Select(x => x.TargetQuantityUOM)
                                    .DefaultIfEmpty()
                                    .Sum(),
                    SumInwardActualQuantityUOM =
                                    c.SelectMany(x => x.ProdOrderPartslistPos_ProdOrderPartslist)
                                    .Where(x => x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.InwardPartIntern)
                                    .SelectMany(x => x.FacilityBooking_ProdOrderPartslistPos)
                                    .Select(x => x.InwardQuantity)
                                    .DefaultIfEmpty()
                                    .Sum(),
                    DifferenceInwardQuantityUOM = 0,
                    RestQuantityUOM = 0
                })
                .OrderBy(c => c.MaterialNo)
        );
        #endregion
    }
}
