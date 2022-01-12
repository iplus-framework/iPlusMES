using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace gip.mes.datamodel
{
    // FacilityStock (Lagerplatz-Bestand)
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.FacilityStock, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, Facility.ClassName, ConstApp.Facility, Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(2, "MinStockQuantity", ConstApp.MinStockQuantity, "", "", true)]
    [ACPropertyEntity(3, "OptStockQuantity", ConstApp.OptStockQuantity, "", "", true)]
    [ACPropertyEntity(4, MDReleaseState.ClassName, ConstApp.ESReleaseState, Const.ContextDatabase + "\\" + MDReleaseState.ClassName, "", true)]
    [ACPropertyEntity(5, "StockQuantity", ConstApp.StockQuantity, "", "", true)]
    [ACPropertyEntity(6, "StockWeight", ConstApp.StockWeight, "", "", true)]

    [ACPropertyEntity(5, "DayInward", ConstApp.DayInward, "", "", true)]
    [ACPropertyEntity(6, "DayTargetInward", ConstApp.DayTargetInward, "", "", true)]
    // 7 DayInwardDiff
    // 8 DayInwardDiffPercent
    [ACPropertyEntity(9, "DayOutward", ConstApp.DayOutward, "", "", true)]
    [ACPropertyEntity(10, "DayTargetOutward", ConstApp.DayTargetOutward, "", "", true)]
    // 11 DayOutwardDiff
    // 12 DayOutwardDiffPercent
    [ACPropertyEntity(13, "WeekInward", ConstApp.WeekInward, "", "", true)]
    [ACPropertyEntity(14, "WeekTargetInward", ConstApp.WeekTargetInward, "", "", true)]
    // 15 WeekInwardDiff
    // 16 WeekInwardDiffPercent
    [ACPropertyEntity(17, "WeekOutward", ConstApp.WeekOutward, "", "", true)]
    [ACPropertyEntity(18, "WeekTargetOutward", ConstApp.WeekTargetOutward, "", "", true)]
    // 19 WeekOutwardDiff
    // 20 WeekOutwardDiffPercent
    [ACPropertyEntity(21, "MonthInward", ConstApp.MonthInward, "", "", true)]
    [ACPropertyEntity(22, "MonthTargetInward", ConstApp.MonthTargetInward, "", "", true)]
    // 23 MonthInwardDiff
    // 24 MonthInwardDiffPercent
    [ACPropertyEntity(25, "MonthOutward", ConstApp.MonthOutward, "", "", true)]
    [ACPropertyEntity(26, "MonthTargetOutward", ConstApp.MonthTargetOutward, "", "", true)]
    // 27 MonthOutwardDiff
    // 28 MonthOutwardDiffPercent
    [ACPropertyEntity(29, "YearInward", ConstApp.YearInward, "", "", true)]
    [ACPropertyEntity(30, "YearTargetInward", ConstApp.YearTargetInward, "", "", true)]
    // 31 YearInwardDiff
    // 32 YearInwardDiffPercent
    [ACPropertyEntity(33, "YearOutward", ConstApp.YearOutward, "", "", true)]
    [ACPropertyEntity(34, "YearTargetOutward", ConstApp.YearTargetOutward, "", "", true)]
    // 39 YearOutwardDiff
    // 40 YearOutwardDiffPercent
    [ACPropertyEntity(37, "DayAdjustment", ConstApp.DayAdjustment, "", "", true)]
    [ACPropertyEntity(38, "DayBalanceDate", ConstApp.DayBalanceDate, "", "", true)]
    [ACPropertyEntity(40, "DayLastOutward", ConstApp.DayLastOutward, "", "", true)]
    [ACPropertyEntity(41, "DayLastStock", ConstApp.DayLastStock, "", "", true)]
    [ACPropertyEntity(42, "WeekAdjustment", ConstApp.WeekAdjustment, "", "", true)]
    [ACPropertyEntity(43, "WeekBalanceDate", ConstApp.WeekBalanceDate, "", "", true)]
    [ACPropertyEntity(44, "MonthActStock", ConstApp.MonthActStock, "", "", true)]
    [ACPropertyEntity(45, "MonthAdjustment", ConstApp.MonthAdjustment, "", "", true)]
    [ACPropertyEntity(46, "MonthAverageStock", ConstApp.MonthAverageStock, "", "", true)]
    [ACPropertyEntity(47, "MonthBalanceDate", ConstApp.MonthBalanceDate, "", "", true)]
    [ACPropertyEntity(49, "MonthLastOutward", ConstApp.MonthLastOutward, "", "", true)]
    [ACPropertyEntity(50, "MonthLastStock", ConstApp.MonthLastStock, "", "", true)]
    [ACPropertyEntity(51, "YearAdjustment", ConstApp.YearAdjustment, "", "", true)]
    [ACPropertyEntity(52, "YearBalanceDate",ConstApp.YearBalanceDate, "", "", true)]
    [ACPropertyEntity(53, "ReservedInwardQuantity", ConstApp.ReservedInwardQuantity, "", "", true)]
    [ACPropertyEntity(54, "ReservedOutwardQuantity", ConstApp.ReservedOutwardQuantity, "", "", true)]
    [ACPropertyEntity(57, "ToleranceMinus", ConstApp.ToleranceMinus, "", "", true)]
    [ACPropertyEntity(58, "TolerancePlus", ConstApp.TolerancePlus, "", "", true)]
    // 60 ReservedQuantity
    // 61 AvailableQuantity

    [ACPropertyEntity(62, "StockQuantityAmb", ConstApp.StockQuantityAmb, "", "", true)]
    [ACPropertyEntity(63, "DayInwardAmb", ConstApp.DayInwardAmb, "", "", true)]
    [ACPropertyEntity(64, "DayTargetInwardAmb", ConstApp.DayTargetInwardAmb, "", "", true)]
    [ACPropertyEntity(65, "DayOutwardAmb", ConstApp.DayTargetOutwardAmb, "", "", true)]
    [ACPropertyEntity(66, "DayTargetOutwardAmb", ConstApp.DayTargetOutwardAmb, "", "", true)]
    [ACPropertyEntity(67, "WeekInwardAmb", ConstApp.WeekInwardAmb, "", "", true)]
    [ACPropertyEntity(68, "WeekTargetInwardAmb", ConstApp.WeekTargetInwardAmb, "", "", true)]
    [ACPropertyEntity(69, "WeekOutwardAmb", ConstApp.WeekOutwardAmb, "", "", true)]
    [ACPropertyEntity(70, "WeekTargetOutwardAmb", ConstApp.WeekTargetOutwardAmb, "", "", true)]
    [ACPropertyEntity(71, "MonthInwardAmb",ConstApp.MonthInwardAmb, "", "", true)]
    [ACPropertyEntity(72, "MonthTargetInwardAmb", ConstApp.MonthTargetInwardAmb, "", "", true)]
    [ACPropertyEntity(73, "MonthOutwardAmb", ConstApp.MonthOutwardAmb, "", "", true)]
    [ACPropertyEntity(74, "MonthTargetOutwardAmb", ConstApp.MonthTargetOutwardAmb, "", "", true)]
    [ACPropertyEntity(75, "YearInwardAmb", ConstApp.YearInwardAmb, "", "", true)]
    [ACPropertyEntity(76, "YearTargetInwardAmb", ConstApp.YearTargetInwardAmb, "", "", true)]
    [ACPropertyEntity(77, "YearOutwardAmb", ConstApp.YearOutwardAmb, "", "", true)]
    [ACPropertyEntity(78, "YearTargetOutwardAmb", ConstApp.YearTargetOutwardAmb, "", "", true)]
    [ACPropertyEntity(79, "DayAdjustmentAmb", ConstApp.DayAdjustmentAmb, "", "", true)]
    [ACPropertyEntity(80, "DayLastOutwardAmb", ConstApp.DayLastOutwardAmb, "", "", true)]
    [ACPropertyEntity(81, "DayLastStockAmb", ConstApp.DayLastStockAmb, "", "", true)]
    [ACPropertyEntity(82, "WeekAdjustmentAmb", ConstApp.WeekAdjustmentAmb, "", "", true)]
    [ACPropertyEntity(83, "MonthActStockAmb", ConstApp.MonthActStockAmb, "", "", true)]
    [ACPropertyEntity(84, "MonthAdjustmentAmb", ConstApp.MonthAdjustmentAmb, "", "", true)]
    [ACPropertyEntity(85, "MonthAverageStockAmb", ConstApp.MonthAverageStockAmb, "", "", true)]
    [ACPropertyEntity(86, "MonthLastOutwardAmb", ConstApp.MonthLastOutwardAmb, "", "", true)]
    [ACPropertyEntity(87, "MonthLastStockAmb", ConstApp.MonthLastStockAmb, "", "", true)]
    [ACPropertyEntity(88, "YearAdjustmentAmb", ConstApp.YearAdjustmentAmb, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]

    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + FacilityStock.ClassName, ConstApp.FacilityStock, typeof(FacilityStock), FacilityStock.ClassName, "Facility\\FacilityNo", "Facility\\FacilityNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<FacilityStock>) })]
    public partial class FacilityStock
    {
        public const string ClassName = "FacilityStock";

        #region New/Delete
        public static FacilityStock NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            FacilityStock entity = new FacilityStock();
            entity.FacilityStockID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDReleaseState = MDReleaseState.DefaultMDReleaseState(dbApp);
            if (parentACObject is Facility)
            {
                Facility facility = parentACObject as Facility;
                entity.Facility = facility;
                entity.Facility.FacilityStock_Facility.Add(entity);
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            return ACCaption;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                if (Facility == null)
                    return "";
                return Facility.ACCaption;
            }
        }

        /// <summary>
        /// Returns Facility
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Facility</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return Facility;
            }
        }

        #endregion

        #region IACObjectEntity Members
        /// <summary>
        /// Method for validating values and references in this EF-Object.
        /// Is called from Change-Tracking before changes will be saved for new unsaved entity-objects.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">Entity-Framework databasecontext</param>
        /// <returns>NULL if sucessful otherwise a Message-List</returns>
        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            if (Facility == null)
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = Facility.ClassName,
                    Message = "Facility is null",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", Facility.ClassName), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }

        //private bool _ContextEventSubscr = false;
        /// <summary>
        /// Method for validating values and references in this EF-Object.
        /// Is called from Change-Tracking before changes will be saved for changed entity-objects.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">Entity-Framework databasecontext</param>
        /// <returns>NULL if sucessful otherwise a Message-List</returns>
        public override IList<Msg> EntityCheckModified(string user, IACEntityObjectContext context)
        {
            return base.EntityCheckModified(user, context);
            //if (!_ContextEventSubscr)
            //{
            //    context.ACChangesExecuted += new ACChangesEventHandler(OnContextACChangesExecuted);
            //    _ContextEventSubscr = true;
            //}
            //return null;
        }

        //void OnContextACChangesExecuted(object sender, ACChangesEventArgs e)
        //{
        //    if (e.Succeeded && e.ChangeType == ACChangesEventArgs.ACChangesType.ACSaveChanges)
        //    {
        //        this.Facility.CallRefreshFacility(false, null);
        //    }
        //    (sender as IACEntityObjectContext).ACChangesExecuted -= new ACChangesEventHandler(OnContextACChangesExecuted);
        //    _ContextEventSubscr = false;
        //}

        static public string KeyACIdentifier
        {
            get
            {
                return "Facility\\FacilityNo";
            }
        }
        #endregion

        #region IEntityProperty Members

        bool bRefreshConfig = false;
        partial void OnXMLConfigChanging(global::System.String value)
        {
            bRefreshConfig = false;
            if (this.EntityState != System.Data.EntityState.Detached && (!(String.IsNullOrEmpty(value) && String.IsNullOrEmpty(XMLConfig)) && value != XMLConfig))
                bRefreshConfig = true;
        }

        partial void OnXMLConfigChanged()
        {
            if (bRefreshConfig)
                ACProperties.Refresh();
        }

        #endregion

        /*
         * Onlinehelp: Definition von zusätzlichen Properties bei Entitäten im Datenbankmodell
         * In der Region "additional properties" können zusätzliche Properties bereit 
         * gestellt werden, die nicht in der Datenbank gespeichert werden.
         * 
         * Wenn die Events vom Set-Property (z.B. OnDayInward) abgefangen werden, 
         * kann dadurch eine automatische Berechnung der zusätzlichen Properties erfolgen.
         * 
         * TODO: Wie erfolgt die erste Berechnung nach dem Laden aus der Datenbank
         * TODO: Automatische Berechnung implementieren
         */
        #region additional properties
        /// <summary>
        /// There are no comments for Property DayInwardDiff in the schema.
        /// </summary>
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(11, "", "en{'Day Inward Qty Difference'}de{'Tageszugang Differenz'}")]
        public float DayInwardDiff
        {
            get
            {
                return this._DayInwardDiff;
            }
            set
            {
                this.OnDayInwardDiffChanging(value);
                this.ReportPropertyChanging("DayInwardDiff");
                this._DayInwardDiff = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("DayInwardDiff");
                this.OnDayInwardDiffChanged();
            }
        }
        private float _DayInwardDiff;
        partial void OnDayInwardDiffChanging(float value);
        partial void OnDayInwardDiffChanged();

        /// <summary>
        /// There are no comments for Property DayInwardDiffPercent in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(12, "", "en{'Day Inward Qty Difference in %'}de{'Tageszugang Differenz in %'}")]
        public global::System.Nullable<float> DayInwardDiffPercent
        {
            get
            {
                return this._DayInwardDiffPercent;
            }
            set
            {
                this.OnDayInwardDiffPercentChanging(value);
                this.ReportPropertyChanging("DayInwardDiffPercent");
                this._DayInwardDiffPercent = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("DayInwardDiffPercent");
                this.OnDayInwardDiffPercentChanged();
            }
        }
        private global::System.Nullable<float> _DayInwardDiffPercent;
        partial void OnDayInwardDiffPercentChanging(global::System.Nullable<float> value);
        partial void OnDayInwardDiffPercentChanged();

        /// <summary>
        /// There are no comments for Property DayOutwardDiff in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(15, "", "en{'Day Outward Qty Difference'}de{'Tagesabgang Differenz'}")]
        public global::System.Nullable<float> DayOutwardDiff
        {
            get
            {
                return this._DayOutwardDiff;
            }
            set
            {
                this.OnDayOutwardDiffChanging(value);
                this.ReportPropertyChanging("DayOutwardDiff");
                this._DayOutwardDiff = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("DayOutwardDiff");
                this.OnDayOutwardDiffChanged();
            }
        }
        private global::System.Nullable<float> _DayOutwardDiff;
        partial void OnDayOutwardDiffChanging(global::System.Nullable<float> value);
        partial void OnDayOutwardDiffChanged();

        /// <summary>
        /// There are no comments for Property DayOutwardDiffPercent in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(16, "", "en{'Day Outward Qty Difference in %'}de{'Tagesabgang Differenz in %'}")]
        public global::System.Nullable<float> DayOutwardDiffPercent
        {
            get
            {
                return this._DayOutwardDiffPercent;
            }
            set
            {
                this.OnDayOutwardDiffPercentChanging(value);
                this.ReportPropertyChanging("DayOutwardDiffPercent");
                this._DayOutwardDiffPercent = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("DayOutwardDiffPercent");
                this.OnDayOutwardDiffPercentChanged();
            }
        }
        private global::System.Nullable<float> _DayOutwardDiffPercent;
        partial void OnDayOutwardDiffPercentChanging(global::System.Nullable<float> value);
        partial void OnDayOutwardDiffPercentChanged();

        /// <summary>
        /// There are no comments for Property WeekInwardDiff in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(19, "", "en{'Week Inward Qty Difference'}de{'Wochenzugang Differenz'}")]
        public global::System.Nullable<float> WeekInwardDiff
        {
            get
            {
                return this._WeekInwardDiff;
            }
            set
            {
                this.OnWeekInwardDiffChanging(value);
                this.ReportPropertyChanging("WeekInwardDiff");
                this._WeekInwardDiff = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("WeekInwardDiff");
                this.OnWeekInwardDiffChanged();
            }
        }
        private global::System.Nullable<float> _WeekInwardDiff;
        partial void OnWeekInwardDiffChanging(global::System.Nullable<float> value);
        partial void OnWeekInwardDiffChanged();

        /// <summary>
        /// There are no comments for Property WeekInwardDiffPercent in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(20, "", "en{'Week Inward Qty Difference in %'}de{'Wochenzugang Differenz in %'}")]
        public global::System.Nullable<float> WeekInwardDiffPercent
        {
            get
            {
                return this._WeekInwardDiffPercent;
            }
            set
            {
                this.OnWeekInwardDiffPercentChanging(value);
                this.ReportPropertyChanging("WeekInwardDiffPercent");
                this._WeekInwardDiffPercent = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("WeekInwardDiffPercent");
                this.OnWeekInwardDiffPercentChanged();
            }
        }
        private global::System.Nullable<float> _WeekInwardDiffPercent;
        partial void OnWeekInwardDiffPercentChanging(global::System.Nullable<float> value);
        partial void OnWeekInwardDiffPercentChanged();


        /// <summary>
        /// There are no comments for Property WeekOutwardDiff in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(23, "", "en{'Week Outward Qty Difference'}de{'Wochenabgang Differenz'}")]
        public global::System.Nullable<float> WeekOutwardDiff
        {
            get
            {
                return this._WeekOutwardDiff;
            }
            set
            {
                this.OnWeekOutwardDiffChanging(value);
                this.ReportPropertyChanging("WeekOutwardDiff");
                this._WeekOutwardDiff = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("WeekOutwardDiff");
                this.OnWeekOutwardDiffChanged();
            }
        }
        private global::System.Nullable<float> _WeekOutwardDiff;
        partial void OnWeekOutwardDiffChanging(global::System.Nullable<float> value);
        partial void OnWeekOutwardDiffChanged();

        /// <summary>
        /// There are no comments for Property WeekOutwardDiffPercent in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(24, "", "en{'Week Outward Qty Difference in %'}de{'Wochenabgang Differenz in %'}")]
        public global::System.Nullable<float> WeekOutwardDiffPercent
        {
            get
            {
                return this._WeekOutwardDiffPercent;
            }
            set
            {
                this.OnWeekOutwardDiffPercentChanging(value);
                this.ReportPropertyChanging("WeekOutwardDiffPercent");
                this._WeekOutwardDiffPercent = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("WeekOutwardDiffPercent");
                this.OnWeekOutwardDiffPercentChanged();
            }
        }
        private global::System.Nullable<float> _WeekOutwardDiffPercent;
        partial void OnWeekOutwardDiffPercentChanging(global::System.Nullable<float> value);
        partial void OnWeekOutwardDiffPercentChanged();

        /// <summary>
        /// There are no comments for Property MonthInwardDiff in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(27, "", "en{'Month Inward Qty Difference'}de{'Monatszugang Differenz'}")]
        public global::System.Nullable<float> MonthInwardDiff
        {
            get
            {
                return this._MonthInwardDiff;
            }
            set
            {
                this.OnMonthInwardDiffChanging(value);
                this.ReportPropertyChanging("MonthInwardDiff");
                this._MonthInwardDiff = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("MonthInwardDiff");
                this.OnMonthInwardDiffChanged();
            }
        }
        private global::System.Nullable<float> _MonthInwardDiff;
        partial void OnMonthInwardDiffChanging(global::System.Nullable<float> value);
        partial void OnMonthInwardDiffChanged();

        /// <summary>
        /// There are no comments for Property MonthInwardDiffPercent in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(28, "", "en{'Month Inward Qty Difference in %'}de{'Monatszugang Differenz in %'}")]
        public global::System.Nullable<float> MonthInwardDiffPercent
        {
            get
            {
                return this._MonthInwardDiffPercent;
            }
            set
            {
                this.OnMonthInwardDiffPercentChanging(value);
                this.ReportPropertyChanging("MonthInwardDiffPercent");
                this._MonthInwardDiffPercent = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("MonthInwardDiffPercent");
                this.OnMonthInwardDiffPercentChanged();
            }
        }
        private global::System.Nullable<float> _MonthInwardDiffPercent;
        partial void OnMonthInwardDiffPercentChanging(global::System.Nullable<float> value);
        partial void OnMonthInwardDiffPercentChanged();


        /// <summary>
        /// There are no comments for Property MonthOutwardDiff in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(31, "", "en{'Month Outward Qty Difference'}de{'Monatsabgang Differenz in %'}")]
        public global::System.Nullable<float> MonthOutwardDiff
        {
            get
            {
                return this._MonthOutwardDiff;
            }
            set
            {
                this.OnMonthOutwardDiffChanging(value);
                this.ReportPropertyChanging("MonthOutwardDiff");
                this._MonthOutwardDiff = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("MonthOutwardDiff");
                this.OnMonthOutwardDiffChanged();
            }
        }
        private global::System.Nullable<float> _MonthOutwardDiff;
        partial void OnMonthOutwardDiffChanging(global::System.Nullable<float> value);
        partial void OnMonthOutwardDiffChanged();

        /// <summary>
        /// There are no comments for Property MonthOutwardDiffPercent in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(32, "", "en{'Month Outward Qty Difference in %'}de{'Monatsabgang Differenz in %'}")]
        public global::System.Nullable<float> MonthOutwardDiffPercent
        {
            get
            {
                return this._MonthOutwardDiffPercent;
            }
            set
            {
                this.OnMonthOutwardDiffPercentChanging(value);
                this.ReportPropertyChanging("MonthOutwardDiffPercent");
                this._MonthOutwardDiffPercent = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("MonthOutwardDiffPercent");
                this.OnMonthOutwardDiffPercentChanged();
            }
        }
        private global::System.Nullable<float> _MonthOutwardDiffPercent;
        partial void OnMonthOutwardDiffPercentChanging(global::System.Nullable<float> value);
        partial void OnMonthOutwardDiffPercentChanged();

        /// <summary>
        /// There are no comments for Property YearInwardDiff in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(35, "", "en{'Year Inward Qty Difference'}de{'Jahreseingang Differenz'}")]
        public global::System.Nullable<float> YearInwardDiff
        {
            get
            {
                return this._YearInwardDiff;
            }
            set
            {
                this.OnYearInwardDiffChanging(value);
                this.ReportPropertyChanging("YearInwardDiff");
                this._YearInwardDiff = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("YearInwardDiff");
                this.OnYearInwardDiffChanged();
            }
        }
        private global::System.Nullable<float> _YearInwardDiff;
        partial void OnYearInwardDiffChanging(global::System.Nullable<float> value);
        partial void OnYearInwardDiffChanged();

        /// <summary>
        /// There are no comments for Property YearInwardDiffPercent in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(36, "", "en{'Year Inward Qty Difference in %'}de{'Jahreseingang Differenz in %'}")]
        public global::System.Nullable<float> YearInwardDiffPercent
        {
            get
            {
                return this._YearInwardDiffPercent;
            }
            set
            {
                this.OnYearInwardDiffPercentChanging(value);
                this.ReportPropertyChanging("YearInwardDiffPercent");
                this._YearInwardDiffPercent = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("YearInwardDiffPercent");
                this.OnYearInwardDiffPercentChanged();
            }
        }
        private global::System.Nullable<float> _YearInwardDiffPercent;
        partial void OnYearInwardDiffPercentChanging(global::System.Nullable<float> value);
        partial void OnYearInwardDiffPercentChanged();

        /// <summary>
        /// There are no comments for Property YearOutwardDiff in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(39, "", "en{'Year Outward Qty Difference'}de{'Jahresabgang Differenz'}")]
        public global::System.Nullable<float> YearOutwardDiff
        {
            get
            {
                return this._YearOutwardDiff;
            }
            set
            {
                this.OnYearOutwardDiffChanging(value);
                this.ReportPropertyChanging("YearOutwardDiff");
                this._YearOutwardDiff = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("YearOutwardDiff");
                this.OnYearOutwardDiffChanged();
            }
        }
        private global::System.Nullable<float> _YearOutwardDiff;
        partial void OnYearOutwardDiffChanging(global::System.Nullable<float> value);
        partial void OnYearOutwardDiffChanged();

        /// <summary>
        /// There are no comments for Property YearOutwardDiffPercent in the schema.
        /// </summary>

        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [ACPropertyInfo(40, "", "en{'Year Outward Qty Difference in %'}de{'Jahresabgang Differenz in %'}")]
        public global::System.Nullable<float> YearOutwardDiffPercent
        {
            get
            {
                return this._YearOutwardDiffPercent;
            }
            set
            {
                this.OnYearOutwardDiffPercentChanging(value);
                this.ReportPropertyChanging("YearOutwardDiffPercent");
                this._YearOutwardDiffPercent = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("YearOutwardDiffPercent");
                this.OnYearOutwardDiffPercentChanged();
            }
        }
        private global::System.Nullable<float> _YearOutwardDiffPercent;
        partial void OnYearOutwardDiffPercentChanging(global::System.Nullable<float> value);
        partial void OnYearOutwardDiffPercentChanged();


        /// <summary>
        /// There are no comments for Property ReservedInwardQuantity in the schema.
        /// </summary>
        public void RecalcReservedInwardQuantity()
        {
            if ((this.Facility == null) || (this.Facility.Material == null))
                return;

            this.ReservedInwardQuantity = 0;
            // TODO: OR-Klausel einfügen für Produktionsaufträge
            List<FacilityReservation> facilityReservationList = (from c in this.Facility.FacilityReservation_Facility where c.InOrderPos != null select c).ToList();
            foreach (FacilityReservation facilityReservation in facilityReservationList)
            {
                // Die ActualQuantity gibt an, wieviel bereits auf dem Material gebucht worden ist
                // Die TargetQuantity gibt an, wieviel Reserviert ist
                // Die Differenz gibt an, wieviel noch geliefert wird
                // TODO: Zugänge von Produktionsaufträge mit einrechnen
                this.ReservedInwardQuantity +=
                        this.Facility.Material.ConvertQuantity(facilityReservation.InOrderPos.TargetQuantity,
                                                    facilityReservation.InOrderPos.Material.BaseMDUnit,
                                                    this.Facility.Material.BaseMDUnit)
                        - this.Facility.Material.ConvertQuantity(facilityReservation.InOrderPos.ActualQuantity,
                                                    facilityReservation.InOrderPos.Material.BaseMDUnit,
                                                    this.Facility.Material.BaseMDUnit);

            }
        }


        /// <summary>
        /// There are no comments for Property ReservedOutwardQuantity in the schema.
        /// </summary>
        public void RecalcReservedOutwardQuantity()
        {
            if ((this.Facility == null) || (this.Facility.Material == null))
                return;

            this.ReservedOutwardQuantity = 0;
            // TODO: OR-Klausel einfügen für Produktionsaufträge
            List<FacilityReservation> facilityReservationList = (from c in this.Facility.FacilityReservation_Facility where c.OutOrderPos != null select c).ToList();
            foreach (FacilityReservation facilityReservation in facilityReservationList)
            {
                // Die ActualQuantity gibt an, wieviel bereits auf dem Material gebucht worden ist
                // Die TargetQuantity gibt an, wieviel Reserviert ist
                // Die Differenz gibt an, wieviel noch abgebucht wird
                // TODO: Abgänge von Produktionsaufträgen mit einrechnen
                this.ReservedOutwardQuantity +=
                        this.Facility.Material.ConvertQuantity(facilityReservation.OutOrderPos.TargetQuantity,
                                                    facilityReservation.OutOrderPos.Material.BaseMDUnit,
                                                    this.Facility.Material.BaseMDUnit)
                        - this.Facility.Material.ConvertQuantity(facilityReservation.OutOrderPos.ActualQuantity,
                                                    facilityReservation.OutOrderPos.Material.BaseMDUnit,
                                                    this.Facility.Material.BaseMDUnit);

            }
        }



        /// <summary>
        /// There are no comments for Property ReservedQuantity in the schema.
        /// </summary>
        [ACPropertyInfo(90, "", "en{'Reserved Quantity'}de{'Reservierte Menge'}")]
        public Double ReservedQuantity
        {
            get
            {
                return ReservedOutwardQuantity - ReservedInwardQuantity;
            }
        }


        /// <summary>
        /// There are no comments for Property AvailableQuantity in the schema.
        /// </summary>
        [ACPropertyInfo(91, "", "en{'Available Quantity'}de{'Verfügbare Menge'}")]
        public Double AvailableQuantity
        {
            get
            {
                return StockQuantity - ReservedQuantity;
            }
        }

        /// <summary>
        /// Get weight. It's calculated over Density and StockQuantity - only if Dimension of BaseMDUnit is Volume
        /// </summary>
        [ACPropertyInfo(92, "", "en{'Weight 15°C'}de{'Gewicht 15°C'}")]
        public Double Weight15FromVolume
        {
            get
            {
                if (this.Facility.Material == null || this.Facility.Material.BaseMDUnit == null || this.Facility.Material.BaseMDUnit.SIDimension != GlobalApp.SIDimensions.Volume || this.Facility.Material.BaseMDUnit.SIUnit == null)
                    return 0;
                if (Math.Abs(this.Facility.Density - 0) <= double.Epsilon)
                    return 0;
                double quantity = this.StockQuantity;
                if (this.Facility.Material.BaseMDUnit != this.Facility.Material.BaseMDUnit.SIUnit)
                    quantity = this.Facility.Material.BaseMDUnit.ConvertToUnit(this.StockQuantity, this.Facility.Material.BaseMDUnit.SIUnit);
                return quantity * this.Facility.Density;
            }
        }

        /// <summary>
        /// Get weight. It's calculated over Density and ambien5t StockQuantity - only if Dimension of BaseMDUnit is Volume
        /// </summary>
        [ACPropertyInfo(93, "", "en{'Weight Ambient'}de{'Gewicht Ambient'}")]
        public Double WeightAmbFromVolume
        {
            get
            {
                if (this.Facility.Material == null || this.Facility.Material.BaseMDUnit == null || this.Facility.Material.BaseMDUnit.SIDimension != GlobalApp.SIDimensions.Volume || this.Facility.Material.BaseMDUnit.SIUnit == null)
                    return 0;
                if (Math.Abs(this.Facility.DensityAmb - 0) <= double.Epsilon)
                    return 0;
                double quantity = this.StockQuantityAmb;
                if (this.Facility.Material.BaseMDUnit != this.Facility.Material.BaseMDUnit.SIUnit)
                    quantity = this.Facility.Material.BaseMDUnit.ConvertToUnit(this.StockQuantityAmb, this.Facility.Material.BaseMDUnit.SIUnit);
                return quantity * this.Facility.DensityAmb;
            }
        }

        partial void OnReservedOutwardQuantityChanged()
        {
            RefreshMinOptFields();
        }

        partial void OnReservedInwardQuantityChanged()
        {
            RefreshMinOptFields();
        }

        partial void OnStockQuantityChanged()
        {
            RefreshMinOptFields();
        }

        public void RefreshMinOptFields()
        {
            OnPropertyChanged("AvailableQuantity");
            OnPropertyChanged("ReservedQuantity");
            OnPropertyChanged("MinStockQuantityDiff");
            OnPropertyChanged("MinStockQuantityExceeded");
            OnPropertyChanged("OptStockQuantityDiff");
            OnPropertyChanged("OptStockQuantityExceeded");
            if (this.Facility != null)
                this.Facility.OnEntityPropertyChanged(null);
        }

        [ACPropertyInfo(94, "", "en{'Min. Stock Diff.'}de{'Min. Bestand Diff.'}")]
        public Double? MinStockQuantityDiff
        {
            get
            {
                if (this.Facility == null)
                    return null;
                if (!this.Facility.MinStockQuantity.HasValue)
                    return null;
                return AvailableQuantity - Facility.MinStockQuantity;
            }
        }

        [ACPropertyInfo(95, "", "en{'Min. Stock Exceeded'}de{'Min. Bestand überschritten'}")]
        public bool MinStockQuantityExceeded
        {
            get
            {
                if (!MinStockQuantityDiff.HasValue || MinStockQuantityDiff.Value > 0)
                    return false;
                return true;
            }
        }

        [ACPropertyInfo(96, "", "en{'Opt. Stock Diff.'}de{'Opt. Bestand Diff.'}")]
        public Double? OptStockQuantityDiff
        {
            get
            {
                if (this.Facility == null)
                    return null;
                if (!this.Facility.OptStockQuantity.HasValue)
                    return null;
                return AvailableQuantity - Facility.OptStockQuantity;
            }
        }

        [ACPropertyInfo(7, "", "en{'Opt. Stock Exceeded'}de{'Opt. Bestand überschritten'}")]
        public bool OptStockQuantityExceeded
        {
            get
            {
                if (!OptStockQuantityDiff.HasValue || OptStockQuantityDiff.Value > 0)
                    return false;
                return true;
            }
        }

        #endregion

    }
}
