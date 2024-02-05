using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    public partial class FacilityManager 
    {
        protected virtual void CreateConfigParams()
        {
            _DayClosingDays = new ACPropertyConfigValue<int>(this, "DayClosingDays", 0);
            _DayClosingPeriodDayrange = new ACPropertyConfigValue<int>(this, "DayClosingPeriodDayrange", 14);
            _DayClosingLastPeriodDayrange = new ACPropertyConfigValue<int>(this, "DayClosingLastPeriodDayrange", 0);
            _DayClosingLastDayClosing = new ACPropertyConfigValue<DateTime>(this, "DayClosingLastDayClosing", DateTime.MinValue);
            _WeekClosingPeriodWeekrange = new ACPropertyConfigValue<int>(this, "WeekClosingPeriodWeekrange", 5);
            _WeekClosingLastPeriodWeekrange = new ACPropertyConfigValue<int>(this, "WeekClosingLastPeriodWeekrange", 0);
            _WeekClosingLastWeekClosing = new ACPropertyConfigValue<DateTime>(this, "WeekClosingLastWeekClosing", DateTime.MinValue);
            _MonthClosingPeriodMonthrange = new ACPropertyConfigValue<int>(this, "MonthClosingPeriodMonthrange", 12);
            _MonthClosingLastPeriodMonthrange = new ACPropertyConfigValue<int>(this, "MonthClosingLastPeriodMonthrange", 0);
            _MonthClosingLastMonthClosing = new ACPropertyConfigValue<DateTime>(this, "MonthClosingLastMonthClosing", DateTime.MinValue);
            _YearClosingPeriodYearrange = new ACPropertyConfigValue<int>(this, "YearClosingPeriodYearrange", 20);
            _YearClosingLastPeriodYearrange = new ACPropertyConfigValue<int>(this, "YearClosingLastPeriodYearrange", 0);
            _YearClosingLastYearClosing = new ACPropertyConfigValue<DateTime>(this, "YearClosingLastYearClosing", DateTime.MinValue);
            _BookingParameterStackBookingModel = new ACPropertyConfigValue<string>(this, "BookingParameterStackBookingModel", "");
            _GenerateHistoryWhenZero = new ACPropertyConfigValue<bool>(this, "GenerateHistoryWhenZero", false);
            _CreateStockOnMaterialMatching = new ACPropertyConfigValue<bool>(this, "CreateStockOnMaterialMatching", false);
            _CreateStockOnFacilityMatching = new ACPropertyConfigValue<bool>(this, "CreateStockOnFacilityMatching", false);
            _CreateStockOnFacilityLotMatching = new ACPropertyConfigValue<bool>(this, "CreateStockOnFacilityLotMatching", false);
            _CreateStockOnCompanyMaterialMatching = new ACPropertyConfigValue<bool>(this, "CreateStockOnCompanyMaterialMatching", false);
            _CreateStockOnPartslistMatching = new ACPropertyConfigValue<bool>(this, "CreateStockOnPartslistMatching", false);
            _BookingParameterNotAvailableMode = new ACPropertyConfigValue<int>(this, "BookingParameterNotAvailableMode", (int)MDBookingNotAvailableMode.BookingNotAvailableModes.AutoSet);
            _BookingParameterDontAllowNegativeStock = new ACPropertyConfigValue<bool>(this, "BookingParameterDontAllowNegativeStock", false);
            _BookingParameterIgnoreManagement = new ACPropertyConfigValue<bool>(this, "BookingParameterIgnoreManagement", false);
            _BookingParameterQuantityIsAbsolute = new ACPropertyConfigValue<bool>(this, "BookingParameterQuantityIsAbsolute", false);
            _BookingParameterBalancingMode = new ACPropertyConfigValue<int>(this, "BookingParameterBalancingMode", (int)MDBalancingMode.BalancingModes.InwardOn_OutwardOn);
            _RootStoreForVehicles = new ACPropertyConfigValue<string>(this, "RootStoreForVehicles", null);
            CreateModuleConstants();
        }

        protected virtual void InitConfigParams()
        {
            InitModuleConstants();
        }

        private ACPropertyConfigValue<int> _DayClosingDays;
        [ACPropertyConfig("en{'DayClosingDays'}de{'Tagesabschluss Anzahl Tage'}", DefaultValue = (int)0)]
        public int DayClosingDays
        {
            get
            {
                ResetConfigValuesCache();
                return _DayClosingDays.ValueT;
            }
            set
            {
                _DayClosingDays.ValueT = value;
            }
        }

        private ACPropertyConfigValue<int> _DayClosingPeriodDayrange;
        [ACPropertyConfig("en{'DayClosingPeriodDayrange'}de{'Periodendauer Tagesbereich'}", DefaultValue = (int)14)]
        public int DayClosingPeriodDayrange
        {
            get
            {
                return _DayClosingPeriodDayrange.ValueT;
            }
            set
            {
                _DayClosingPeriodDayrange.ValueT = value;
            }
        }

        private ACPropertyConfigValue<int> _DayClosingLastPeriodDayrange;
        [ACPropertyConfig("en{'DayClosingLastPeriodDayrange'}de{'Letzte Periode Tagesbereich'}", DefaultValue = (int)0)]
        public int DayClosingLastPeriodDayrange
        {
            get
            {
                ResetConfigValuesCache();
                return _DayClosingLastPeriodDayrange.ValueT;
            }
            set
            {
                _DayClosingLastPeriodDayrange.ValueT = value;
            }
        }

        private ACPropertyConfigValue<DateTime> _DayClosingLastDayClosing;
        [ACPropertyConfig("Letzter Tagesabschluss")]
        public DateTime DayClosingLastDayClosing
        {
            get
            {
                ResetConfigValuesCache();
                return _DayClosingLastDayClosing.ValueT;
            }
            set
            {
                _DayClosingLastDayClosing.ValueT = value;
            }
        }


        private ACPropertyConfigValue<int> _WeekClosingPeriodWeekrange;
        [ACPropertyConfig("Periodendauer Wochenbereich", DefaultValue = (int)5)]
        public int WeekClosingPeriodWeekrange
        {
            get
            {
                return _WeekClosingPeriodWeekrange.ValueT;
            }
            set
            {
                _WeekClosingPeriodWeekrange.ValueT = value;
            }
        }

        private ACPropertyConfigValue<int> _WeekClosingLastPeriodWeekrange;
        [ACPropertyConfig("Letzte Periode Wochenbereich", DefaultValue = (int)0)]
        public int WeekClosingLastPeriodWeekrange
        {
            get
            {
                ResetConfigValuesCache();
                return _WeekClosingLastPeriodWeekrange.ValueT;
            }
            set
            {
                _WeekClosingLastPeriodWeekrange.ValueT = value;
            }
        }

        private ACPropertyConfigValue<DateTime> _WeekClosingLastWeekClosing;
        [ACPropertyConfig("Letzter Wochenabschluss")]
        public DateTime WeekClosingLastWeekClosing
        {
            get
            {
                ResetConfigValuesCache();
                return _WeekClosingLastWeekClosing.ValueT;
            }
            set
            {
                _WeekClosingLastWeekClosing.ValueT = value;
            }
        }


        private ACPropertyConfigValue<int> _MonthClosingPeriodMonthrange;
        [ACPropertyConfig("Periodendauer Monatsbereich", DefaultValue = (int)12)]
        public int MonthClosingPeriodMonthrange
        {
            get
            {
                return _MonthClosingPeriodMonthrange.ValueT;
            }
            set
            {
                _MonthClosingPeriodMonthrange.ValueT = value;
            }
        }

        private ACPropertyConfigValue<int> _MonthClosingLastPeriodMonthrange;
        [ACPropertyConfig("Letzte Periode Monatsbereich", DefaultValue = (int)0)]
        public int MonthClosingLastPeriodMonthrange
        {
            get
            {
                ResetConfigValuesCache();
                return _MonthClosingLastPeriodMonthrange.ValueT;
            }
            set
            {
                _MonthClosingLastPeriodMonthrange.ValueT = value;
            }
        }

        private ACPropertyConfigValue<DateTime> _MonthClosingLastMonthClosing;
        [ACPropertyConfig("Letzter Monatsabschluss")]
        public DateTime MonthClosingLastMonthClosing
        {
            get
            {
                ResetConfigValuesCache();
                return _MonthClosingLastMonthClosing.ValueT;
            }
            set
            {
                _MonthClosingLastMonthClosing.ValueT = value;
            }
        }


        private ACPropertyConfigValue<int> _YearClosingPeriodYearrange;
        [ACPropertyConfig("Periodendauer Jahresbereich", DefaultValue = (int)20)]
        public int YearClosingPeriodYearrange
        {
            get
            {
                return _YearClosingPeriodYearrange.ValueT;
            }
            set
            {
                _YearClosingPeriodYearrange.ValueT = value;
            }
        }

        private ACPropertyConfigValue<int> _YearClosingLastPeriodYearrange;
        [ACPropertyConfig("Letzte Periode Jahresbereich", DefaultValue = (int)0)]
        public int YearClosingLastPeriodYearrange
        {
            get
            {
                ResetConfigValuesCache();
                return _YearClosingLastPeriodYearrange.ValueT;
            }
            set
            {
                _YearClosingLastPeriodYearrange.ValueT = value;
            }
        }

        private ACPropertyConfigValue<DateTime> _YearClosingLastYearClosing;
        [ACPropertyConfig("Letzter Jahresabschluss")]
        public DateTime YearClosingLastYearClosing
        {
            get
            {
                ResetConfigValuesCache();
                return _YearClosingLastYearClosing.ValueT;
            }
            set
            {
                _YearClosingLastYearClosing.ValueT = value;
            }
        }

        // BookingParamter Configuration Parameters

        private ACPropertyConfigValue<string> _BookingParameterStackBookingModel;
        [ACPropertyConfig("Standard Schichtenmodell")]
        public string BookingParameterStackBookingModel
        {
            get { return _BookingParameterStackBookingModel.ValueT; }
            set { _BookingParameterStackBookingModel.ValueT = value; }
        }

        private ACPropertyConfigValue<bool> _GenerateHistoryWhenZero;
        [ACPropertyConfig("en{'GenerateHistoryWhenZero'}de{'Erzeuge Bilanzhistorieneintrag bei Nullbestand'}")]
        public virtual bool GenerateHistoryWhenZero
        {
            get
            {
                return _GenerateHistoryWhenZero.ValueT;
            }
            set
            {
                _GenerateHistoryWhenZero.ValueT = value;
            }
        }


        private ACPropertyConfigValue<bool> _CreateStockOnMaterialMatching;
        [ACPropertyConfig("en{'Create Stock-entity on material matching'}de{'Erzeuge Bestandseintrag bei Materialabgleich'}")]
        public virtual bool CreateStockOnMaterialMatching
        {
            get
            {
                return _CreateStockOnMaterialMatching.ValueT;
            }
            set
            {
                _CreateStockOnMaterialMatching.ValueT = value;
            }
        }

        private ACPropertyConfigValue<bool> _CreateStockOnFacilityMatching;
        [ACPropertyConfig("en{'Create Stock-entity on facility matching'}de{'Erzeuge Bestandseintrag bei Lagerplatzabgleich'}")]
        public virtual bool CreateStockOnFacilityMatching
        {
            get
            {
                return _CreateStockOnFacilityMatching.ValueT;
            }
            set
            {
                _CreateStockOnFacilityMatching.ValueT = value;
            }
        }

        private ACPropertyConfigValue<bool> _CreateStockOnFacilityLotMatching;
        [ACPropertyConfig("en{'Create Stock-entity on lot matching'}de{'Erzeuge Bestandseintrag bei Chargenabgleich'}")]
        public virtual bool CreateStockOnFacilityLotMatching
        {
            get
            {
                return _CreateStockOnFacilityLotMatching.ValueT;
            }
            set
            {
                _CreateStockOnFacilityLotMatching.ValueT = value;
            }
        }

        private ACPropertyConfigValue<bool> _CreateStockOnCompanyMaterialMatching;
        [ACPropertyConfig("en{'Create Stock-entity on company material matching'}de{'Erzeuge Bestandseintrag bei Firmenmaterialabgleich'}")]
        public virtual bool CreateStockOnCompanyMaterialMatching
        {
            get
            {
                return _CreateStockOnCompanyMaterialMatching.ValueT;
            }
            set
            {
                _CreateStockOnCompanyMaterialMatching.ValueT = value;
            }
        }

        private ACPropertyConfigValue<bool> _CreateStockOnPartslistMatching;
        [ACPropertyConfig("en{'Create Stock-entity on bill of material matching'}de{'Erzeuge Bestandseintrag bei Stücklistenabgleich'}")]
        public virtual bool CreateStockOnPartslistMatching
        {
            get
            {
                return _CreateStockOnPartslistMatching.ValueT;
            }
            set
            {
                _CreateStockOnPartslistMatching.ValueT = value;
            }
        }

        private ACPropertyConfigValue<int> _BookingParameterNotAvailableMode;
        [ACPropertyConfig("Modus zum Umgang mit Nullbestandskennzeichen", DefaultValue = (int)MDBookingNotAvailableMode.BookingNotAvailableModes.AutoSet)]
        public int BookingParameterNotAvailableMode
        {
            get { return _BookingParameterNotAvailableMode.ValueT; }
            set { _BookingParameterNotAvailableMode.ValueT = value; }
        }

        private ACPropertyConfigValue<bool> _BookingParameterDontAllowNegativeStock;
        [ACPropertyConfig("Keine negativen Bestände", DefaultValue = false)]
        public bool BookingParameterDontAllowNegativeStock
        {
            get { return _BookingParameterDontAllowNegativeStock.ValueT; }
            set { _BookingParameterDontAllowNegativeStock.ValueT = value; }
        }

        private ACPropertyConfigValue<bool> _BookingParameterIgnoreManagement;
        [ACPropertyConfig("Buchung auch wenn Lager von ext. System verwaltet", DefaultValue = false)]
        public bool BookingParameterIgnoreManagement
        {
            get { return _BookingParameterIgnoreManagement.ValueT; }
            set { _BookingParameterIgnoreManagement.ValueT = value; }
        }

        private ACPropertyConfigValue<bool> _BookingParameterQuantityIsAbsolute;
        [ACPropertyConfig("Keine Differenzbuchuchungen", DefaultValue = false)]
        public bool BookingParameterQuantityIsAbsolute
        {
            get { return _BookingParameterQuantityIsAbsolute.ValueT; }
            set { _BookingParameterQuantityIsAbsolute.ValueT = value; }
        }

        private ACPropertyConfigValue<int> _BookingParameterBalancingMode;
        [ACPropertyConfig("Bilanzierung auf Zu- und Abgangsfeldern", DefaultValue = (int)MDBalancingMode.BalancingModes.InwardOn_OutwardOn)]
        public int BookingParameterBalancingMode
        {
            get { return _BookingParameterBalancingMode.ValueT; }
            set { _BookingParameterBalancingMode.ValueT = value; }
        }

        private ACPropertyConfigValue<string> _RootStoreForVehicles;
        [ACPropertyConfig("en{'Store for vehicles'}de{'Lagerort für Fahrzeuge'}")]
        public string RootStoreForVehicles
        {
            get { return _RootStoreForVehicles.ValueT; }
            set { _RootStoreForVehicles.ValueT = value; }
        }


        public Facility GetRootStoreForVehicles(DatabaseApp dbApp)
        {
            string rootStore = RootStoreForVehicles;
            if (String.IsNullOrEmpty(rootStore))
                return null;
            return dbApp.Facility.Where(c => c.FacilityNo == rootStore).FirstOrDefault();
        }
    }
}


