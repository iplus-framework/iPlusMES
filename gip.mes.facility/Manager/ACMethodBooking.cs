using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using System.Runtime.Serialization;

namespace gip.mes.facility
{
    [ACSerializeableInfo]
    [DataContract]
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Posting Parameters'}de{'Buchungsfunktionen'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, false)]
    public class ACMethodBooking : ACMethod
    {
        #region C´tors
        public ACMethodBooking()
        {
        }

        public ACMethodBooking(IRoot root, Database database)
        {
            _Root = root as ACRoot;
            Database = database;

            ClearBookingData(null);
        }

        protected ACMethodBooking(ACMethodBooking parent)
        {
            if (parent != null)
            {
                _Root = parent.Root;
                Database = parent.Database;
            }
            ClearBookingData(parent);
        }

        private static gip.core.datamodel.ACClass _ReflectedACType = null;
        /// <summary>
        /// Metadata (iPlus-Type) of this instance. ATTENTION: IACType are EF-Objects. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>  iPlus-Type (EF-Object from ACClass*-Tables)</value>
        public override IACType ACType
        {
            get
            {
                if (_ReflectedACType == null)
                    _ReflectedACType = this.ReflectACType() as gip.core.datamodel.ACClass;
                return _ReflectedACType;
            }
        }


        public void OnPropertyChangedAll()
        {
            OnPropertyChanged(MDMovementReason.ClassName);
        }

        protected override void ResultValueList_ListChanged(object sender, ListChangedEventArgs e)
        {
            base.ResultValueList_ListChanged(sender, e);
            ACValue acValue = ResultValueList[e.NewIndex];
            if (acValue != null)
            {
                OnPropertyChanged(acValue.ACIdentifier);
            }
        }

        protected override void ParameterValueList_ListChanged(object sender, ListChangedEventArgs e)
        {
            base.ParameterValueList_ListChanged(sender, e);

            ACValue acValue = ParameterValueList[e.NewIndex];
            if (acValue != null)
            {
                OnPropertyChanged(acValue.ACIdentifier);
            }
        }

        public void ClearBookingData()
        {
            ClearBookingData(null);
        }

        protected virtual ACMethodBooking NewACMethodBookingClone()
        {
            return new ACMethodBooking(this);
        }

        protected virtual void ClearBookingData(ACMethodBooking parent)
        {
            // Inernal and private members:
            if (parent == null)
                ParamsAdjusted = NewACMethodBookingClone();
            else
                ParamsAdjusted = null;
            Parent = parent;

            EmptyCellsWithLotManagedBookings();
            // Konfigurationsparameter:
            // BookingType = null;
            MDBookingNotAvailableMode = MDBookingNotAvailableMode.DefaultMDBookingNotAvailableMode(DatabaseApp);
            DontAllowNegativeStock = null;
            IgnoreManagement = null;
            QuantityIsAbsolute = null;
            MDBalancingMode = MDBalancingMode.DefaultMDBalancingMode(DatabaseApp);

            // Buchungsparameter
            ShiftBookingReverse = false;
            MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp);
            MDReleaseState = null;

            IgnoreIsEnabled = false;
            MDReservationMode = MDReservationMode.DefaultMDReservationMode(DatabaseApp);
            MDMovementReason = null;

            InwardMaterial = null;
            InwardFacility = null;
            InwardFacilityLot = null;
            InwardFacilityCharge = null;
            InwardFacilityLocation = null;
            InwardSplitNo = null;
            InwardStackBookingModel = null;
            InwardPartslist = null;
            InwardHandlingUnit = null;

            OutwardMaterial = null;
            OutwardFacility = null;
            OutwardFacilityLot = null;
            OutwardFacilityCharge = null;
            OutwardFacilityLocation = null;
            OutwardSplitNo = null;
            OutwardStackBookingModel = null;
            OutwardPartslist = null;
            OutwardHandlingUnit = null;

            InwardQuantity = null;
            InwardTargetQuantity = null;
            OutwardQuantity = null;
            OutwardTargetQuantity = null;
            MDUnit = null;

            ACProgram = null;

            //InDeliveryNotePosLot = null;
            InOrderPos = null;

            //OutDeliveryNotePosLot = null;
            OutOrderPos = null;

            StorageDate = null;
            ProductionDate = null;
            ExpirationDate = null;
            MinimumDurability = null;
            Comment = "";
            RecipeOrFactoryInfo = "";
            XMLConfig = "";
        }


        private void CopyThisToAdjustedParams()
        {
            ParamsAdjusted = Clone() as ACMethodBooking;
        }

        public override void CopyFrom(ACMethodDescriptor from)
        {
            if (from == null)
                return;
            base.CopyFrom(from);
            ACMethodBooking acMethod = from as ACMethodBooking;
            if (acMethod == null)
                return;
            OutwardFacilityChargeList = acMethod.OutwardFacilityChargeList;
        }

        public override object Clone()
        {
            ACMethodBooking clone = new ACMethodBooking();
            clone.CopyFrom(this);
            return clone;
        }

        public virtual void InitRequiredDefaultParameterIfNotSet()
        {
            _FacilityBookings = null;
            EmptyCellsWithLotManagedBookings();
            if (MDBookingNotAvailableMode == null)
                MDBookingNotAvailableMode = MDBookingNotAvailableMode.DefaultMDBookingNotAvailableMode(DatabaseApp);
            if (MDBalancingMode == null)
                MDBalancingMode = MDBalancingMode.DefaultMDBalancingMode(DatabaseApp);
            if (MDZeroStockState == null)
                MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(DatabaseApp);
            if (MDReservationMode == null)
                MDReservationMode = MDReservationMode.DefaultMDReservationMode(DatabaseApp);
            //if (!StorageDate.HasValue || StorageDate.Value <= DateTime.MinValue)
            //    StorageDate = DateTime.Today;
        }


        protected static IACComponent _FacilityManager = null;
        protected static IACComponent CurrentFacilityManager
        {
            get
            {
                if (_FacilityManager == null)
                {
                    // Falls als lokaler Dienst konfiguriert
                    _FacilityManager = gip.core.datamodel.Database.Root.ACUrlCommand("LocalServiceObjects\\FacilityManager") as IACComponent;
                    if (_FacilityManager == null)
                    {
                        // Falls als Service Konfiguriert
                        _FacilityManager = gip.core.datamodel.Database.Root.ACUrlCommand("Service\\FacilityManager") as IACComponent;
                    }
                }

                return _FacilityManager;
            }
        }

        #endregion

        #region Internal and Private Properties
        public ACMethodBooking Parent
        {
            get;
            protected set;
        }

        /// <summary>
        /// Der Adjusted-BookingParameter ist eine Kopie vom eigentlichen Aufruf,
        /// jedoch mit fehler korrigierten Werten.
        /// Durch die Fehlertoleranz ist es möglich Buchungsaufrufe von außen zu machen
        /// auch wenn kleine Fehler gemacht wurden.
        /// Mit dem Adjusted arbeitet der FacilityBookingManager.
        /// </summary>
        public ACMethodBooking ParamsAdjusted
        {
            get;
            protected set;
        }

        /// <summary>
        /// Intern von FacilityBookingManager verwendete Objekte 
        /// </summary>
        public FacilityBooking FacilityBooking
        {
            get;
            internal set;
        }

        internal List<Facility> InwardCellsWithLotManagedBookings
        {
            get;
            set;
        }

        internal List<Facility> OutwardCellsWithLotManagedBookings
        {
            get;
            set;
        }

        internal void EmptyCellsWithLotManagedBookings()
        {
            InwardCellsWithLotManagedBookings = new List<Facility>();
            OutwardCellsWithLotManagedBookings = new List<Facility>();
        }
        #endregion

        #region Public Properties
        private ACRoot _Root = null;
        public ACRoot Root
        {
            get
            {
                if (_Root == null)
                    _Root = gip.core.datamodel.Database.Root as ACRoot;
                return _Root;
            }
        }

        /// <summary>
        /// Falls FacilityBookingManager aufgerufen wurde über einen BackgroundWorker-Thread,
        /// dann muss dieses Property gesetzt werden, damit der FacilityBookingManager den aktuellen
        /// Fortschritt über die ProgressInfo-Klasse mittels der ReportProgress()-Methode rückmelden kann.
        /// </summary>
        public IVBProgress VBProgress
        {
            get;
            set;
        }

        public override IACEntityObjectContext Database
        {
            get
            {
                if (_Database == null)
                {
                    if (InwardMaterial != null)
                        _Database = InwardMaterial.GetObjectContext<DatabaseApp>();
                    else if (InwardFacility != null)
                        _Database = InwardFacility.GetObjectContext<DatabaseApp>();
                    else if (InwardFacilityCharge != null)
                        _Database = InwardFacilityCharge.GetObjectContext<DatabaseApp>();
                    else if (InwardFacilityLocation != null)
                        _Database = InwardFacilityLocation.GetObjectContext<DatabaseApp>();
                    else if (InwardFacilityLot != null)
                        _Database = InwardFacilityLot.GetObjectContext<DatabaseApp>();
                    else if (OutwardMaterial != null)
                        _Database = OutwardMaterial.GetObjectContext<DatabaseApp>();
                    else if (OutwardFacility != null)
                        _Database = OutwardFacility.GetObjectContext<DatabaseApp>();
                    else if (OutwardFacilityCharge != null)
                        _Database = OutwardFacilityCharge.GetObjectContext<DatabaseApp>();
                    else if (OutwardFacilityLocation != null)
                        _Database = OutwardFacilityLocation.GetObjectContext<DatabaseApp>();
                    else if (OutwardFacilityLot != null)
                        _Database = OutwardFacilityLot.GetObjectContext<DatabaseApp>();
                }
                return _Database;
            }
            set
            {
                _Database = value;
            }
        }

        public DatabaseApp DatabaseApp
        {
            get
            {
                return Database as DatabaseApp;
            }
        }


        #endregion

        #region Configuration parameters
        //private MDFacilityBookingType _BookingType = null;
        //[ACPropertyInfo(9999, "", "", Const.ContextDatabase + "\\MDFacilityBookingType")]
        //public MDFacilityBookingType BookingType
        //{
        //    get
        //    {
        //        return _BookingType;
        //    }
        //    set
        //    {
        //        _BookingType = value;
        //        if (_BookingType != null)
        //        {
        //            InitParametersFromBookingType();
        //        }
        //    }
        //}

        [ACPropertyInfo(9999, "", "en{'Posting Type'}de{'Buchungstyp'}")]
        public GlobalApp.FacilityBookingType BookingType
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("BookingType");
                if (acValue == null)
                    return GlobalApp.FacilityBookingType.StockCorrection;
                return (GlobalApp.FacilityBookingType)acValue.Value;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("BookingType");
                if (acValue == null)
                {
                    acValue = new ACValue("BookingType", typeof(GlobalApp.FacilityBookingType), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Gibt an wie Buchungsklasse mit dem Nullbestandskennzeichen umgehen soll.
        /// Welche Modi möglich sind, sind im Global.BookingNotAvailableMode-Enumerator definert
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Mode at zero stock'}de{'Modus bei Nullbestand'}", Const.ContextDatabase + "\\" + MDBookingNotAvailableMode.ClassName)]
        public MDBookingNotAvailableMode MDBookingNotAvailableMode
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue(MDBookingNotAvailableMode.ClassName);
                if (acValue == null)
                    return null;
                return acValue.Value as MDBookingNotAvailableMode;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue(MDBookingNotAvailableMode.ClassName);
                if (acValue == null)
                {
                    acValue = new ACValue(MDBookingNotAvailableMode.ClassName, typeof(MDBookingNotAvailableMode), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Wenn FALSE: Bestände auf Lagerplätzen, Chargen, Artikel dürfen negativ werden.
        /// Wenn Parameter TRUE: Dann wird nur noch der maximal mögliche Wert abgebucht,
        /// der dem kleinsten Wert des Bestandes aus der Lagerplatz, Artikel, FacilityLot-Tabelle entspricht.
        /// Falls Parameter nicht gesetzt ist, dann wird in anderen Einstellungen (Global, Lagerort oder Artikel) gesucht.
        /// Falls dort auch nicht gesetzt ist, dann wird FALSE angenommen.
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'No negative stocks'}de{'Keine negativen Bestände'}")]
        public Nullable<Boolean> DontAllowNegativeStock
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("DontAllowNegativeStock");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Boolean>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("DontAllowNegativeStock");
                if (acValue == null)
                {
                    acValue = new ACValue("DontAllowNegativeStock", typeof(Boolean), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Falls ein Lager von einem externen ERP-System verwaltet wird, dann erfolgt intern keine Buchung
        /// Soll dies trotzdem erfolgen, kann dies mit diesem Parameter forciert werden
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Ignore Management'}de{'Ignoriere Verwaltungskennzeichen'}")]
        public Nullable<Boolean> IgnoreManagement
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("IgnoreManagement");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Boolean>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("IgnoreManagement");
                if (acValue == null)
                {
                    acValue = new ACValue("IgnoreManagement", typeof(Nullable<Boolean>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Falls ein Lager von einem externen ERP-System verwaltet wird und die absoluten Bestände
        /// übertragen werden und keine Differenzbuchungen gemacht werden müssen, dann muss dieser
        /// Parameter auf true gesetzt werden.
        /// Die übergebenen Qunatity-Felder werden als absolute Menge betrachtet
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Quantity is absolute'}de{'Menge ist absolut'}")]
        public Nullable<Boolean> QuantityIsAbsolute
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("QuantityIsAbsolute");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Boolean>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("QuantityIsAbsolute");
                if (acValue == null)
                {
                    acValue = new ACValue("QuantityIsAbsolute", typeof(Nullable<Boolean>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Kennzeichen ob auf Zugang- oder Abgangsfeldern gebucht werden soll
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Balancing Mode'}de{'Bilanzierungsmodus'}", Const.ContextDatabase + "\\" + MDBalancingMode.ClassName)]
        public MDBalancingMode MDBalancingMode
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue(MDBalancingMode.ClassName);
                if (acValue == null)
                    return null;
                return acValue.Value as MDBalancingMode;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue(MDBalancingMode.ClassName);
                if (acValue == null)
                {
                    acValue = new ACValue(MDBalancingMode.ClassName, typeof(MDBalancingMode), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }
        #endregion

        #region Booking parameters
        /// <summary>
        /// Parameter für ausgewählten Stack-Calculator ob das StackBookingModel rückwärts gerechnet werden soll.
        /// In diesem fall gibt der Stack-Calculator ein umgekehrt sortierte Liste zurück
        /// Wird z.B. gebraucht, falls eine ursprüngliche Umlagerungsbuchung (FIFO) von einem Silo in das andere Silo
        /// wieder rückgängig gemacht werden soll. In diesem Fall müsste nach LIFO wieder zurückgebucht werden.
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Reverse Posting'}de{'Rückbuchung'}")]
        public bool ShiftBookingReverse
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("ShiftBookingReverse");
                if (acValue == null)
                    return false;
                return (bool)acValue.Value;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("ShiftBookingReverse");
                if (acValue == null)
                {
                    acValue = new ACValue("ShiftBookingReverse", typeof(bool), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }


        /// <summary>
        /// Wenn SetNotAvailable = NotAvailableState.Set, dann muss Facility(Silo), FacilityCharge oder FacilityLot auf Nullbestand gesetzt werdern
        /// Wenn SetNotAvailable = NotAvailableState.Reset, dann muss FacilityCharge zurückgesetzt werden
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Set to zero Stock'}de{'Auf Nullbestand setzen'}", Const.ContextDatabase + "\\" + MDZeroStockState.ClassName)]
        public MDZeroStockState MDZeroStockState
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue(MDZeroStockState.ClassName);
                if (acValue == null)
                    return null;
                return acValue.Value as MDZeroStockState;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue(MDZeroStockState.ClassName);
                if (acValue == null)
                {
                    acValue = new ACValue(MDZeroStockState.ClassName, typeof(MDZeroStockState), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }


        /// <summary>
        /// gibt an ob ein Material/FacilityCharge/Facility/FacilityLot gesperrt oder freigegeben werden soll
        /// Wenn Release-State nicht gesetzt wird, dann bleibt der Zustand der FacilityChargen unberührt
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Release/Block'}de{'Freigeben/Sperren'}", Const.ContextDatabase + "\\" + MDReleaseState.ClassName)]
        public MDReleaseState MDReleaseState
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue(MDReleaseState.ClassName);
                if (acValue == null)
                    return null;
                return acValue.Value as MDReleaseState;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue(MDReleaseState.ClassName);
                if (acValue == null)
                {
                    acValue = new ACValue(MDReleaseState.ClassName, typeof(MDReleaseState), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Ignoriert den IsEnbaled-Status eines Lagerplatzes. Die Buchung wir trotzdem durchgeführt auf wenn eine Einlagerungs oder Auslagerungssperre gesetzt ist.
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Ignore in-/outward lock'}de{'Ignoriere Ein-/Ausgangssperre'}")]
        public bool IgnoreIsEnabled
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("IgnoreIsEnabled");
                if (acValue == null)
                    return false;
                return (bool)acValue.Value;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("IgnoreIsEnabled");
                if (acValue == null)
                {
                    acValue = new ACValue("IgnoreIsEnabled", typeof(bool), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Ignoriert den IsEnbaled-Status eines Lagerplatzes. Die Buchung wir trotzdem durchgeführt auf wenn eine Einlagerungs oder Auslagerungssperre gesetzt ist.
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Lot consumed'}de{'Charge aufgebraucht'}")]
        public bool SetCompleted
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("SetCompleted");
                if (acValue == null)
                    return false;
                return (bool)acValue.Value;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("SetCompleted");
                if (acValue == null)
                {
                    acValue = new ACValue("SetCompleted", typeof(bool), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Falls Referenz zu WE, WA, Prod-Tabelle gesetzt, 
        /// dann kann über das Reservierungskennzeichen bestimmt werden ob Reservierungsmengen eingetragen oder aufgelöst werden
        /// Abhängig davon, ob das Inward oder Outward-Mengenfeld gesetzt wird, erfolgt eine Zu- oder Abgangsreservierung
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Reservation Mode'}de{'Reservierungsmodus'}", Const.ContextDatabase + "\\" + MDReservationMode.ClassName)]
        public MDReservationMode MDReservationMode
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue(MDReservationMode.ClassName);
                if (acValue == null)
                    return null;
                return acValue.Value as MDReservationMode;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue(MDReservationMode.ClassName);
                if (acValue == null)
                {
                    acValue = new ACValue(MDReservationMode.ClassName, typeof(MDReservationMode), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }


        /// <summary>
        /// Infoparameter, der nur abgespeichert wird, aber sich keine Verarbeitungslogik dahinter befindet
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Movement Reason'}de{'Buchungsgrund'}", Const.ContextDatabase + "\\" + MDMovementReason.ClassName)]
        public MDMovementReason MDMovementReason
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue(MDMovementReason.ClassName);
                if (acValue == null)
                    return null;
                return acValue.Value as MDMovementReason;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue(MDMovementReason.ClassName);
                if (acValue == null)
                {
                    acValue = new ACValue(MDMovementReason.ClassName, typeof(MDMovementReason), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Prevent broadcast to remote store'}de{'Verhindere Benachrichtigung an entfernten Lagerort'}")]
        public bool PreventSendToRemoteStore
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("PreventSendToRemoteStore");
                if (acValue == null)
                    return false;
                return (bool)acValue.Value;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("PreventSendToRemoteStore");
                if (acValue == null)
                {
                    acValue = new ACValue("PreventSendToRemoteStore", typeof(bool), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        #region Material und Lagerplatz

        #region Entitäten die sich auf die ZUGANGsfelder (INward) auswirken
        /// <summary>
        /// Entitäten die sich auf die ZUGANGsfelder (INward) auswirken
        /// </summary>

        [ACPropertyInfo(9999, "", "en{'Material (Inward Posting)'}de{'Material (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + Material.ClassName)]
        public Material InwardMaterial
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardMaterial");
                if (acValue == null)
                    return null;
                return acValue.Value as Material;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardMaterial");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardMaterial", typeof(Material), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Storage Bin (Inward Posting)'}de{'Lagerplatz (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + Facility.ClassName)]
        public Facility InwardFacility
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardFacility");
                if (acValue == null)
                    return null;
                return acValue.Value as Facility;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardFacility");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardFacility", typeof(Facility), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Lot (Inward Posting)'}de{'Los (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + FacilityLot.ClassName)]
        public FacilityLot InwardFacilityLot
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardFacilityLot");
                if (acValue == null)
                    return null;
                return acValue.Value as FacilityLot;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardFacilityLot");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardFacilityLot", typeof(FacilityLot), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Quant (Inward Posting)'}de{'Quant (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + FacilityCharge.ClassName)]
        public FacilityCharge InwardFacilityCharge
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardFacilityCharge");
                if (acValue == null)
                    return null;
                return acValue.Value as FacilityCharge;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardFacilityCharge");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardFacilityCharge", typeof(FacilityCharge), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Storage Location (Inward Posting)'}de{'Lagerort (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + Facility.ClassName)]
        public Facility InwardFacilityLocation
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardFacilityLocation");
                if (acValue == null)
                    return null;
                return acValue.Value as Facility;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardFacilityLocation");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardFacilityLocation", typeof(Facility), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Split-No. (Inward Posting)'}de{'Split-Nr. (Zugangsbuchung)'}")]
        public Nullable<Int32> InwardSplitNo
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardSplitNo");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Int32>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardSplitNo");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardSplitNo", typeof(Nullable<Int32>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Posting Algorithm (Inward Post)'}de{'Buchungsalgorithmus (Zugangsbuchung)'}", Const.ContextDatabase + "\\MDStackCalculatorTypeList")]
        public gip.core.datamodel.ACClass InwardStackBookingModel
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardStackBookingModel");
                if (acValue == null)
                    return null;
                return acValue.Value as gip.core.datamodel.ACClass;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardStackBookingModel");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardStackBookingModel", typeof(gip.core.datamodel.ACClass), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Bill of Materials (Inward Posting)'}de{'Stückliste (Zugangsbuchung)'}", Const.ContextDatabase + "\\" + Partslist.ClassName)]
        public Partslist InwardPartslist
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardPartslist");
                if (acValue == null)
                    return null;
                return acValue.Value as Partslist;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardPartslist");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardPartslist", typeof(Partslist), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Material Manufact. (Inward)'}de{'Materialhersteller (Zugang)'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName)]
        public CompanyMaterial InwardCompanyMaterial
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardCompanyMaterial");
                if (acValue == null)
                    return null;
                return acValue.Value as CompanyMaterial;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardCompanyMaterial");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardCompanyMaterial", typeof(CompanyMaterial), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999)]
        public HandlingUnit InwardHandlingUnit
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardHandlingUnit");
                if (acValue == null)
                    return null;
                return acValue.Value as HandlingUnit;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardHandlingUnit");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardHandlingUnit", typeof(HandlingUnit), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        #endregion

        #region Entitäten die ABGANGsfelder (OUTward) auswirken
        /// <summary>
        /// Entitäten die sich auf die ABGANGsfelder (OUTward) auswirken
        /// </summary>

        [ACPropertyInfo(9999, "", "en{'Material (Outward Posting)'}de{'Material (Abgangsbuchung)'}", Const.ContextDatabase + "\\" + Material.ClassName)]
        public Material OutwardMaterial
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardMaterial");
                if (acValue == null)
                    return null;
                return acValue.Value as Material;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardMaterial");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardMaterial", typeof(Material), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Storage Bin (Outward Posting)'}de{'Lagerplatz (Abgangsbuchung)'}", Const.ContextDatabase + "\\" + Facility.ClassName)]
        public Facility OutwardFacility
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardFacility");
                if (acValue == null)
                    return null;
                return acValue.Value as Facility;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardFacility");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardFacility", typeof(Facility), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);

                }
                else if (acValue.Value != value)
                {
                    acValue.Value = value;
                }
            }
        }

        [ACPropertyInfo(9999, "", "en{'Lot (Outward Posting)'}de{'Los (Abgangsbuchung)'}", Const.ContextDatabase + "\\" + FacilityLot.ClassName)]
        public FacilityLot OutwardFacilityLot
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardFacilityLot");
                if (acValue == null)
                    return null;
                return acValue.Value as FacilityLot;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardFacilityLot");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardFacilityLot", typeof(FacilityLot), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Quant (Outward Posting)'}de{'Quant (Abgangsbuchung)'}", Const.ContextDatabase + "\\" + FacilityCharge.ClassName)]
        public FacilityCharge OutwardFacilityCharge
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardFacilityCharge");
                if (acValue == null)
                    return null;
                return acValue.Value as FacilityCharge;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardFacilityCharge");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardFacilityCharge", typeof(FacilityCharge), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        public IList<FacilityCharge> OutwardFacilityChargeList
        {
            get;
            set;
        }

        [ACPropertyInfo(9999, "", "en{'Storage Location (Outward Posting)'}de{'Lagerort (Abgangsbuchung)'}", Const.ContextDatabase + "\\" + Facility.ClassName)]
        public Facility OutwardFacilityLocation
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardFacilityLocation");
                if (acValue == null)
                    return null;
                return acValue.Value as Facility;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardFacilityLocation");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardFacilityLocation", typeof(Facility), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Split-No. (Outward Posting)'}de{'Split-Nr. (Abgangsbuchung)'}")]
        public Nullable<Int32> OutwardSplitNo
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardSplitNo");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Int32>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardSplitNo");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardSplitNo", typeof(Nullable<Int32>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Posting Algorithm (Outward Posting)'}de{'Buchungsalgorithmus (Abgangsbuchung)'}", Const.ContextDatabase + "\\MDStackCalculatorTypeList")]
        public gip.core.datamodel.ACClass OutwardStackBookingModel
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardStackBookingModel");
                if (acValue == null)
                    return null;
                return acValue.Value as gip.core.datamodel.ACClass;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardStackBookingModel");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardStackBookingModel", typeof(gip.core.datamodel.ACClass), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Bill of Materials (Outward Posting)'}de{'Stückliste (Abgangsbuchung)'}", Const.ContextDatabase + "\\" + Partslist.ClassName)]
        public Partslist OutwardPartslist
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardPartslist");
                if (acValue == null)
                    return null;
                return acValue.Value as Partslist;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardPartslist");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardPartslist", typeof(Partslist), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Material manufact. (Outward)'}de{'Materialhersteller (Abgang)'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName)]
        public CompanyMaterial OutwardCompanyMaterial
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardCompanyMaterial");
                if (acValue == null)
                    return null;
                return acValue.Value as CompanyMaterial;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardCompanyMaterial");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardCompanyMaterial", typeof(CompanyMaterial), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999)]
        public HandlingUnit OutwardHandlingUnit
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardHandlingUnit");
                if (acValue == null)
                    return null;
                return acValue.Value as HandlingUnit;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardHandlingUnit");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardHandlingUnit", typeof(HandlingUnit), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }
        #endregion
        #endregion

        #region Mengen und Einheiten

        #region Mengen die sich auf die ZUGANGsfelder (INward) auswirken
        /// <summary>
        /// Ist-Menge die gebucht werden soll. 
        /// Diese Menge muss immer dann angegeben werden, wenn sie gedanklich im Zusammenhang mit einem Zugang steht,
        /// und somit als Zugangswert in den Artikel/Lagerplatz/Chargen-Statistiken auftauchen soll
        /// Also bei:
        /// Bei Hergestellten Mengen die aus einem Produktionsauftrag resultieren: Die Menge enstpricht dem Istwert aus der Produktionscharge/Erzeugnis (z.B. Silobefüllung, Abfüllung, Absackung), 
        /// Bei Wareneingängen: Die Menge aus entspricht der Istmenge aus der WE-Lieferscheinposition/Warenavis
        /// Bei Umlagerungen bezieht sich die Menge auf die Charge aus dem Ziellager
        /// Bei Korrektur- oder Inventurbuchungen bezieht sich die Menge auf die Charge im Lager
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Inward Quantity'}de{'Zugangsmenge'}")]
        public Nullable<Double> InwardQuantity
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardQuantity");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Double>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardQuantity");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardQuantity", typeof(Nullable<Double>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }


        /// <summary>
        /// Wenn geplante Warenzugänge gemacht werden (wie z.B. Herstellungen, Wareneingänge...) bei denen die Istwerte
        /// von der geplanten Sollmenge abweichen und diese protokolliert werden sollen, damit später Genauigkeitsstatisitken
        /// aufgestellt werden können, kann hier dieser Soll-Parameter zusätzlich übergeben werden
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Inward Target Quantity'}de{'Eingansgmenge Soll'}")]
        public Nullable<Double> InwardTargetQuantity
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardTargetQuantity");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Double>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardTargetQuantity");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardTargetQuantity", typeof(Nullable<Double>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Ist-Menge ambient (Umgebungstemperatur)
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Inward Quantity Ambient'}de{'Zugangsmenge ambient'}")]
        public Nullable<Double> InwardQuantityAmb
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardQuantityAmb");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Double>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardQuantityAmb");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardQuantityAmb", typeof(Nullable<Double>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }


        /// <summary>
        /// Sollmenge ambient
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Inward Target Qty Ambient'}de{'Zugangsmenge Soll ambient'}")]
        public Nullable<Double> InwardTargetQuantityAmb
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardTargetQuantityAmb");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Double>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardTargetQuantityAmb");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardTargetQuantityAmb", typeof(Nullable<Double>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }
        #endregion


        #region Mengen die sich auf die ABGANGsfelder (OUTward) auswirken
        /// <summary>
        /// Ist-Menge die gebucht werden soll. 
        /// Diese Menge muss immer dann angegeben werden, wenn sie gedanklich im Zusammenhang mit einer Entnahme steht,
        /// und somit als Verbrauchswert in den Artikel/Lagerplatz/Chargen-Statistiken auftauchen soll
        /// Ansonsten darf die Menge nicht gesetzt werden!
        /// Also bei:
        /// Bei Entnahmen über Produktionsaufträgen: Die Menge enstpricht dem Istwert aus der Produktionschargenbezogenen Stückliste (z.B. Dosierung), 
        /// Bei Entnahmen im Warenausgang: Die Menge aus entspricht der Istmenge aus der Kommissionier-/Verladeposition (Verladene Istmenge auf LKW)
        /// Bei Umlagerungen bezieht sich die Menge auf die Charge aus dem Quelllager
        /// Bei Korrektur- oder Inventurbuchungen bezieht sich die Menge auf die Charge im Lager
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Outward Qty'}de{'Abgangsmenge'}")]
        public Nullable<Double> OutwardQuantity
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardQuantity");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Double>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardQuantity");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardQuantity", typeof(Nullable<Double>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                    OnPropertyChanged("OutwardQuantity");
                }
                else
                {
                    acValue.Value = value;
                    OnPropertyChanged("OutwardQuantity");
                }
            }
        }

        /// <summary>
        /// Wenn geplante Warenentnahmen gemacht werden (wie z.B. Dosierungen, Verladungen...) bei denen die Istwerte
        /// von der geplanten Sollmenge abweichen und diese protokolliert werden sollen, damit später Genauigkeitsstatisitken
        /// aufgestellt werden können, kann hier dieser Soll-Parameter zusätzlich übergeben werden
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Outward Target Quantity'}de{'Abgangsmenge Soll'}")]
        public Nullable<Double> OutwardTargetQuantity
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardTargetQuantity");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Double>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardTargetQuantity");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardTargetQuantity", typeof(Nullable<Double>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }


        /// <summary>
        /// Ist-Menge Ambient (Umgebungstemperatur)
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Outward Quantity Ambient'}de{'Abgangsmenge ambient'}")]
        public Nullable<Double> OutwardQuantityAmb
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardQuantityAmb");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Double>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardQuantityAmb");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardQuantityAmb", typeof(Nullable<Double>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Soll-Menge Ambient (Umgebungstemperatur)
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Outward Target Qty Ambient'}de{'Abgangsmenge Soll ambient'}")]
        public Nullable<Double> OutwardTargetQuantityAmb
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardTargetQuantityAmb");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<Double>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("OutwardTargetQuantityAmb");
                if (acValue == null)
                {
                    acValue = new ACValue("OutwardTargetQuantityAmb", typeof(Nullable<Double>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        #endregion


        [ACPropertyInfo(9999, "", "en{'Quantity Unit'}de{'Mengeneinheit'}", Const.ContextDatabase + "\\MDUnitList")]
        public MDUnit MDUnit
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue(MDUnit.ClassName);
                if (acValue == null)
                    return null;
                return acValue.Value as MDUnit;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue(MDUnit.ClassName);
                if (acValue == null)
                {
                    acValue = new ACValue(MDUnit.ClassName, typeof(MDUnit), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        #endregion

        #region Auftrags-/Bestellungsbeziehung
        [ACPropertyInfo(9999, "", "", Const.ContextDatabaseIPlus + "\\ACProgram")]
        public gip.core.datamodel.ACProgram ACProgram
        {
            get;
            set;
        }

        [ACPropertyInfo(9999, "", "", Const.ContextDatabase + "\\" + InOrderPos.ClassName)]
        public InOrderPos InOrderPos
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue(InOrderPos.ClassName);
                if (acValue == null)
                    return null;
                return acValue.Value as InOrderPos;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue(InOrderPos.ClassName);
                if (acValue == null)
                {
                    acValue = new ACValue(InOrderPos.ClassName, typeof(InOrderPos), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        //[ACPropertyInfo(9999, "", "", Const.ContextDatabase + "\\" + InOrder.ClassName)]
        //public InDeliveryNotePosLot InDeliveryNotePosLot
        //{
        //    get
        //    {
        //        return (InDeliveryNotePosLot)ParameterValueList["InDeliveryNotePosLot"];
        //    }
        //    set
        //    {
        //        ParameterValueList["InDeliveryNotePosLot"] = value;
        //    }
        //}

        [ACPropertyInfo(9999, "", "", Const.ContextDatabase + "\\" + OutOrderPos.ClassName)]
        public OutOrderPos OutOrderPos
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue(OutOrderPos.ClassName);
                if (acValue == null)
                    return null;
                return acValue.Value as OutOrderPos;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue(OutOrderPos.ClassName);
                if (acValue == null)
                {
                    acValue = new ACValue(OutOrderPos.ClassName, typeof(OutOrderPos), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        //[ACPropertyInfo(9999, "", "", Const.ContextDatabase + "\\" + OutOrder.ClassName)]
        //public OutDeliveryNotePosLot OutDeliveryNotePosLot
        //{
        //    get
        //    {
        //        return (OutDeliveryNotePosLot)ParameterValueList["OutDeliveryNotePosLot"];
        //    }
        //    set
        //    {
        //        ParameterValueList["OutDeliveryNotePosLot"] = value;
        //    }
        //}


        /// <summary>
        /// Reference to a ProdOrderPartslistPos which represents a intermediate-Material or the final material
        /// "INward"
        /// </summary>
        [ACPropertyInfo(9999, "", "", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName)]
        public ProdOrderPartslistPos PartslistPos
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("PartslistPos");
                if (acValue == null)
                    return null;
                return acValue.Value as ProdOrderPartslistPos;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("PartslistPos");
                if (acValue == null)
                {
                    acValue = new ACValue("PartslistPos", typeof(ProdOrderPartslistPos), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Reference to a ProdOrderPartslistPosRelation which represents a material which is consumed from an Order
        /// "OUTward"
        /// </summary>
        [ACPropertyInfo(9999, "", "", Const.ContextDatabase + "\\" + ProdOrderPartslistPosRelation.ClassName)]
        public ProdOrderPartslistPosRelation PartslistPosRelation
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("PartslistPosRelation");
                if (acValue == null)
                    return null;
                return acValue.Value as ProdOrderPartslistPosRelation;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("PartslistPosRelation");
                if (acValue == null)
                {
                    acValue = new ACValue("PartslistPosRelation", typeof(ProdOrderPartslistPosRelation), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Reference to a ProdOrderPartslistPosRelation which represents a material which is consumed from an Order
        /// "OUTward"
        /// </summary>
        [ACPropertyInfo(9999, "", "", Const.ContextDatabase + "\\" + FacilityInventoryPos.ClassName)]
        public FacilityInventoryPos FacilityInventoryPos
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("FacilityInventoryPos");
                if (acValue == null)
                    return null;
                return acValue.Value as FacilityInventoryPos;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("FacilityInventoryPos");
                if (acValue == null)
                {
                    acValue = new ACValue("FacilityInventoryPos", typeof(FacilityInventoryPos), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Reference to a ProdOrderPartslistPosRelation which represents a material which is consumed from an Order
        /// "OUTward"
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Partial Lot'}de{'Teillos'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPosFacilityLot.ClassName)]
        public ProdOrderPartslistPosFacilityLot ProdOrderPartslistPosFacilityLot
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("ProdOrderPartslistPosFacilityLot");
                if (acValue == null)
                    return null;
                return acValue.Value as ProdOrderPartslistPosFacilityLot;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("ProdOrderPartslistPosFacilityLot");
                if (acValue == null)
                {
                    acValue = new ACValue("ProdOrderPartslistPosFacilityLot", typeof(ProdOrderPartslistPosFacilityLot), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Contractual Partner'}de{'Vertragspartner'}", Const.ContextDatabase + "\\CPartnerCompanyList")]
        public Company CPartnerCompany
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("CPartnerCompany");
                if (acValue == null)
                    return null;
                return acValue.Value as Company;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("CPartnerCompany");
                if (acValue == null)
                {
                    acValue = new ACValue("CPartnerCompany", typeof(Company), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "", Const.ContextDatabase + "\\" + PickingPos.ClassName)]
        public PickingPos PickingPos
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("PickingPos");
                if (acValue == null)
                    return null;
                return acValue.Value as PickingPos;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("PickingPos");
                if (acValue == null)
                {
                    acValue = new ACValue("PickingPos", typeof(PickingPos), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        #endregion

        #region Einlagerungsinformationen
        [ACPropertyInfo(9999)]
        public Nullable<DateTime> StorageDate
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("StorageDate");
                if (acValue == null)
                    return null;
                Nullable<DateTime> dateTime = acValue.Value as Nullable<DateTime>;
                if (dateTime.HasValue && dateTime.Value <= DateTime.MinValue)
                    return null;
                return dateTime;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("StorageDate");
                if (acValue == null)
                {
                    acValue = new ACValue("StorageDate", typeof(Nullable<DateTime>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999)]
        public Nullable<DateTime> ProductionDate
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("ProductionDate");
                if (acValue == null)
                    return null;
                Nullable<DateTime> dateTime = acValue.Value as Nullable<DateTime>;
                if (dateTime.HasValue && dateTime.Value <= DateTime.MinValue)
                    return null;
                return dateTime;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("ProductionDate");
                if (acValue == null)
                {
                    acValue = new ACValue("ProductionDate", typeof(Nullable<DateTime>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999)]
        public Nullable<DateTime> ExpirationDate
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("ExpirationDate");
                if (acValue == null)
                    return null;
                Nullable<DateTime> dateTime = acValue.Value as Nullable<DateTime>;
                if (dateTime.HasValue && dateTime.Value <= DateTime.MinValue)
                    return null;
                return dateTime;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("ExpirationDate");
                if (acValue == null)
                {
                    acValue = new ACValue("ExpirationDate", typeof(Nullable<DateTime>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999)]
        public Nullable<int> MinimumDurability
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("MinimumDurability");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<int>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("MinimumDurability");
                if (acValue == null)
                {
                    acValue = new ACValue("MinimumDurability", typeof(Nullable<int>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999, "", "en{'Comment'}de{'Bemerkung'}")]
        public string Comment
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("Comment");
                if (acValue == null)
                    return null;
                return acValue.Value as string;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("Comment");
                if (acValue == null)
                {
                    acValue = new ACValue("Comment", typeof(string), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999)]
        public string RecipeOrFactoryInfo
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("RecipeOrFactoryInfo");
                if (acValue == null)
                    return null;
                return acValue.Value as string;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("RecipeOrFactoryInfo");
                if (acValue == null)
                {
                    acValue = new ACValue("RecipeOrFactoryInfo", typeof(string), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Machine/Workplace where the posting takes place
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Process Module'}de{'Prozessmodul'}")]
        public string PropertyACUrl
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("PropertyACUrl");
                if (acValue == null)
                    return null;
                return acValue.Value as string;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("PropertyACUrl");
                if (acValue == null)
                {
                    acValue = new ACValue("PropertyACUrl", typeof(string), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        [ACPropertyInfo(9999)]
        public override string XMLConfig
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("XMLConfig");
                if (acValue == null)
                    return null;
                return acValue.Value as string;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("XMLConfig");
                if (acValue == null)
                {
                    acValue = new ACValue("XMLConfig", typeof(string), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }
        #endregion

        #endregion

        #region Calculated Properties
        public bool IsOnlyReservationBooking
        {
            get
            {
                if (MDReservationMode == null)
                    return false;
                if (MDReservationMode.ReservationMode == MDReservationMode.ReservationModes.Off)
                    return false;
                Double inwardQuantity = 0;
                if (InwardQuantity.HasValue)
                    inwardQuantity = InwardQuantity.Value;
                Double outwardQuantity = 0;
                if (OutwardQuantity.HasValue)
                    outwardQuantity = OutwardQuantity.Value;
                if ((MDReservationMode.ReservationMode == MDReservationMode.ReservationModes.Reserve)
                    || ((MDReservationMode.ReservationMode == MDReservationMode.ReservationModes.CancelComplete)
                          && (Math.Abs(inwardQuantity - 0) <= Double.Epsilon)
                          && (Math.Abs(outwardQuantity - 0) <= Double.Epsilon)))
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsReservationBooking
        {
            get
            {
                if (MDReservationMode == null)
                    return false;
                if (MDReservationMode.ReservationMode == MDReservationMode.ReservationModes.Off)
                    return false;
                return true;
            }
        }

        public bool IsOnlyPhysicalBooking
        {
            get
            {
                return !IsReservationBooking;
            }
        }

        public bool IsPhysicalBooking
        {
            get
            {
                return !IsOnlyReservationBooking && MDReleaseState == null;
            }
        }

        public bool IsLotManaged
        {
            get
            {
                Material material = DetermineMaterial();
                if (material != null)
                {
                    return material.IsLotManaged;
                }
                ACMethodBooking BP = this;
                if (BP.InwardMaterial != null)
                    return BP.InwardMaterial.IsLotManaged;
                else if (BP.OutwardMaterial != null)
                    return BP.OutwardMaterial.IsLotManaged;
                else if (BP.InwardMaterial == null && BP.ParamsAdjusted != null && BP.ParamsAdjusted.InwardMaterial != null)
                    return BP.ParamsAdjusted.InwardMaterial.IsLotManaged;
                else if (BP.OutwardMaterial == null && BP.ParamsAdjusted != null && BP.ParamsAdjusted.OutwardMaterial != null)
                    return BP.ParamsAdjusted.OutwardMaterial.IsLotManaged;
                return false;
            }
        }

        public bool IsAutoResetNotAvailable
        {
            get
            {
                ACMethodBooking BP = this;
                if (BP.MDBookingNotAvailableMode == null && BP.ParamsAdjusted != null)
                    BP = BP.ParamsAdjusted;
                if (BP.MDBookingNotAvailableMode == null)
                    return true;
                return (!((BP.MDBookingNotAvailableMode.BookingNotAvailableMode == MDBookingNotAvailableMode.BookingNotAvailableModes.Off)
                            || (BP.MDBookingNotAvailableMode.BookingNotAvailableMode == MDBookingNotAvailableMode.BookingNotAvailableModes.AutoSet)
                            || (BP.MDBookingNotAvailableMode.BookingNotAvailableMode == MDBookingNotAvailableMode.BookingNotAvailableModes._null)));
            }
        }

        public int? InwardAutoSplitQuant
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardAutoSplitQuant");
                if (acValue == null)
                    return null;
                return acValue.Value as Nullable<int>;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("InwardAutoSplitQuant");
                if (acValue == null)
                {
                    acValue = new ACValue("InwardAutoSplitQuant", typeof(Nullable<int>), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }

        /// <summary>
        /// Ignoriert den IsEnbaled-Status eines Lagerplatzes. Die Buchung wir trotzdem durchgeführt auf wenn eine Einlagerungs oder Auslagerungssperre gesetzt ist.
        /// </summary>
        [ACPropertyInfo(9999, "", "en{'Posting behaviour'}de{'Buchungsverhalten'}")]
        public PostingBehaviourEnum PostingBehaviour
        {
            get
            {
                ACValue acValue = ParameterValueList.GetACValue("PostingBehaviour");
                if (acValue == null)
                    return DefaultPostingBehaviour;
                return (PostingBehaviourEnum)acValue.Value;
            }
            set
            {
                ACValue acValue = ParameterValueList.GetACValue("PostingBehaviour");
                if (acValue == null)
                {
                    acValue = new ACValue("PostingBehaviour", typeof(PostingBehaviourEnum), value, Global.ParamOption.Optional);
                    ParameterValueList.Add(acValue);
                }
                acValue.Value = value;
            }
        }


        public PostingBehaviourEnum DefaultPostingBehaviour
        {
            get
            {
                bool isRelocation = BookingType == GlobalApp.FacilityBookingType.Relocation_FacilityCharge
                        || BookingType == GlobalApp.FacilityBookingType.Relocation_FacilityCharge_Facility
                        || BookingType == GlobalApp.FacilityBookingType.Relocation_FacilityCharge_FacilityLocation
                        || BookingType == GlobalApp.FacilityBookingType.Relocation_Facility_BulkMaterial
                        || BookingType == GlobalApp.FacilityBookingType.PickingRelocation
                        || BookingType == GlobalApp.FacilityBookingType.PickingInward
                        || BookingType == GlobalApp.FacilityBookingType.PickingInwardCancel;
                bool isProdInward = BookingType == GlobalApp.FacilityBookingType.ProdOrderPosInward
                        || BookingType == GlobalApp.FacilityBookingType.ProdOrderPosInwardCancel;

                if (!isRelocation && !isProdInward)
                    return PostingBehaviourEnum.DoNothing;

                PostingBehaviourEnum behaviour = PostingBehaviourEnum.DoNothing;
                if (InwardFacility == null)
                    return behaviour;
                else
                {
                    var facility2Check = InwardFacility;
                    while (facility2Check != null)
                    {
                        if (facility2Check.PostingBehaviour > PostingBehaviourEnum.DoNothing)
                        {
                            behaviour = facility2Check.PostingBehaviour;
                            break;
                        }
                        facility2Check = facility2Check.Facility1_ParentFacility;
                    }

                    if (    (behaviour == PostingBehaviourEnum.ZeroStockAlways)
                         || (isRelocation && (behaviour == PostingBehaviourEnum.BlockOnRelocation || behaviour == PostingBehaviourEnum.ZeroStockOnRelocation))
                         || (isProdInward && behaviour == PostingBehaviourEnum.ZeroStockOnProduction))
                        return behaviour;
                }
                return PostingBehaviourEnum.DoNothing;
            }
        }

        /// <summary>
        /// Falls kein ZeroStockState oder ReleaseState gesetzt, dann ist Buchung immer mengenbezogen
        /// </summary>
        public bool IsQuantityNeededForBooking
        {
            get
            {
                if (!AreQuantityParamsNeeded)
                    return false;
                if (MDZeroStockState != null && MDZeroStockState.ZeroStockState != MDZeroStockState.ZeroStockStates.Off)
                    return false;
                if (MDReleaseState != null)
                    return false;
                return true;
            }
        }


        gip.core.datamodel.ACClass _InwardStackCalculatorType = null;
        public gip.core.datamodel.ACClass InwardStackCalculatorType
        {
            get
            {
                if (InwardStackBookingModel != null)
                    return InwardStackBookingModel;
                if (_InwardStackCalculatorType != null)
                    return _InwardStackCalculatorType;
                if (InwardFacility != null)
                {
                    if (InwardFacility.StackCalculatorACClass != null)
                    {
                        _InwardStackCalculatorType = InwardFacility.StackCalculatorACClass.FromIPlusContext<gip.core.datamodel.ACClass>();
                        return _InwardStackCalculatorType;
                    }
                }
                if (InwardFacilityCharge != null)
                {
                    if (InwardFacilityCharge.Facility != null)
                    {
                        if (InwardFacilityCharge.Facility.StackCalculatorACClass != null)
                        {
                            _InwardStackCalculatorType = InwardFacilityCharge.Facility.StackCalculatorACClass.FromIPlusContext<gip.core.datamodel.ACClass>();
                            return _InwardStackCalculatorType;
                        }
                    }
                }
                if (InwardMaterial != null)
                {
                    if (InwardMaterial.StackCalculatorACClass != null)
                    {
                        _InwardStackCalculatorType = InwardMaterial.StackCalculatorACClass.FromIPlusContext<gip.core.datamodel.ACClass>();
                        return _InwardStackCalculatorType;
                    }
                }

                if (DatabaseApp != null)
                {
                    if (StackCalculatorACClassList != null)
                    {
                        _InwardStackCalculatorType = StackCalculatorACClassList.ToArray().FirstOrDefault(c => c.ACIdentifier == StandardCalculator.ClassName);
                        if (_InwardStackCalculatorType == null)
                            _InwardStackCalculatorType = StackCalculatorACClassList.FirstOrDefault();
                        return _InwardStackCalculatorType;
                    }
                }

                return null;
            }
        }

        gip.core.datamodel.ACClass _OutwardStackCalculatorType = null;
        public gip.core.datamodel.ACClass OutwardStackCalculatorType
        {
            get
            {
                if (OutwardStackBookingModel != null)
                    return OutwardStackBookingModel;
                if (_OutwardStackCalculatorType != null)
                    return _OutwardStackCalculatorType;
                if (OutwardFacility != null)
                {
                    if (OutwardFacility.StackCalculatorACClass != null)
                    {
                        _OutwardStackCalculatorType = OutwardFacility.StackCalculatorACClass.FromIPlusContext<gip.core.datamodel.ACClass>();
                        return _OutwardStackCalculatorType;
                    }
                }
                if (OutwardFacilityCharge != null)
                {
                    if (OutwardFacilityCharge.Facility != null)
                    {
                        if (OutwardFacilityCharge.Facility.StackCalculatorACClass != null)
                        {
                            _OutwardStackCalculatorType = OutwardFacilityCharge.Facility.StackCalculatorACClass.FromIPlusContext<gip.core.datamodel.ACClass>();
                            return _OutwardStackCalculatorType;
                        }
                    }
                }
                if (OutwardMaterial != null)
                {
                    if (OutwardMaterial.StackCalculatorACClass != null)
                    {
                        _OutwardStackCalculatorType = OutwardMaterial.StackCalculatorACClass.FromIPlusContext<gip.core.datamodel.ACClass>();
                        return _OutwardStackCalculatorType;
                    }
                }

                if (DatabaseApp != null)
                {
                    if (StackCalculatorACClassList != null)
                    {
                        _OutwardStackCalculatorType = StackCalculatorACClassList.ToArray().FirstOrDefault(c => c.ACIdentifier == StandardCalculator.ClassName);
                        if (_OutwardStackCalculatorType == null)
                            _OutwardStackCalculatorType = StackCalculatorACClassList.FirstOrDefault();
                        return _OutwardStackCalculatorType;
                    }
                }

                return null;
            }
        }

        [ACPropertyInfo(9999)]
        public IQueryable<gip.core.datamodel.ACClass> StackCalculatorACClassList
        {
            get
            {
                if (DatabaseApp == null)
                    return null;
                using (ACMonitor.Lock(DatabaseApp.ContextIPlus.QueryLock_1X000))
                {
                    return s_cQry_StackCalculator(DatabaseApp.ContextIPlus);
                }
            }
        }

        public static readonly Func<Database, IQueryable<gip.core.datamodel.ACClass>> s_cQry_StackCalculator =
            CompiledQuery.Compile<Database, IQueryable<gip.core.datamodel.ACClass>>(
                (db) =>
                    db.ACClass
                    .Where(c => (c.BasedOnACClassID.HasValue
                                    && !c.IsAbstract
                                    && (c.ACClass1_BasedOnACClass.ACIdentifier == ACStackCalculatorBase.ClassName // 1. Ableitungsstufe
                                        || (c.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == ACStackCalculatorBase.ClassName // 2. Ableitungsstufe
                                                    || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == ACStackCalculatorBase.ClassName // 3. Ableitungsstufe
                                                                    || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                        && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == ACStackCalculatorBase.ClassName) // 4. Ableitungsstufe
                                                                    )
                                                        )
                                                    )
                                            )
                                        )
                                    )
                    )
            );

        public Double FactorInwardTargetQuantityToQuantity
        {
            get
            {
                double factor = 1;
                if (InwardTargetQuantity.HasValue && InwardQuantity.HasValue)
                {
                    factor = (InwardTargetQuantity.Value / InwardQuantity.Value);
                    if (Double.IsNaN(factor))
                        factor = 1;
                }
                return factor;
            }
        }

        public Double FactorOutwardTargetQuantityToQuantity
        {
            get
            {
                double factor = 1;
                if (OutwardTargetQuantity.HasValue && OutwardQuantity.HasValue)
                {
                    factor = (OutwardTargetQuantity.Value / OutwardQuantity.Value);
                    if (Double.IsNaN(factor))
                        factor = 1;
                }
                return factor;
            }
        }

        public Double FactorInwardAmbient
        {
            get
            {
                double factor = 1;
                if (InwardQuantity.HasValue && InwardQuantityAmb.HasValue)
                {
                    factor = (InwardQuantityAmb.Value / InwardQuantity.Value);
                    if (Double.IsNaN(factor))
                        factor = 1;
                }

                return factor;
            }
        }

        public Double FactorOutwardAmbient
        {
            get
            {
                double factor = 1;
                if (OutwardQuantity.HasValue && OutwardQuantityAmb.HasValue)
                {
                    factor = (OutwardQuantityAmb.Value / OutwardQuantity.Value);
                    if (Double.IsNaN(factor))
                        factor = 1;
                }
                return factor;
            }
        }

        public Double FactorInwardTargetAmbient
        {
            get
            {
                double factor = 1;
                if (InwardTargetQuantity.HasValue && InwardTargetQuantityAmb.HasValue)
                {
                    factor = (InwardTargetQuantityAmb.Value / InwardTargetQuantity.Value);
                    if (Double.IsNaN(factor))
                        factor = 1;
                }
                return factor;
            }
        }

        public Double FactorOutwardTargetAmbient
        {
            get
            {
                double factor = 1;
                if (OutwardTargetQuantity.HasValue && OutwardTargetQuantityAmb.HasValue)
                {
                    factor = (OutwardTargetQuantityAmb.Value / OutwardTargetQuantity.Value);
                    if (Double.IsNaN(factor))
                        factor = 1;
                }
                return factor;
            }
        }

        private CompanyMaterial _CPartnerCompanyMaterial = null;
        public CompanyMaterial CPartnerCompanyMaterial
        {
            get
            {
                return _CPartnerCompanyMaterial;
            }
            set
            {
                _CPartnerCompanyMaterial = value;
            }
        }

        #endregion

        #region public methods
        /// <summary>
        /// Diese Funktion kann für die IsEnabled-Funktion der Commands eingesetzt werden.
        /// Es gibt Dialoge, bei denen mit den BookingParamer über verschiedene Buttons,
        /// unterschiedliche Buchungenaufgerufen werden können.
        /// </summary>
        public bool IsEnabled()
        {
            return IsValid();
        }

        public Global.ControlModesInfo OnGetControlModes(IVBContent vbControl, string acValueId)
        {
            Global.ControlModesInfo subResult = Global.ControlModesInfo.Enabled;
            ACValue acValue = ParameterValueList.GetACValue(acValueId) as ACValue;
            if (acValue != null)
            {
                if (acValue.Option == Global.ParamOption.Required)
                {
                    subResult = IACObjectReflectionExtension.CheckPropertyMinMax(vbControl.VBContentPropertyInfo, acValue.Value, null, acValue.ObjectFullType, false,
                                                                    vbControl.VBContentPropertyInfo.MinLength, vbControl.VBContentPropertyInfo.MaxLength,
                                                                    vbControl.VBContentPropertyInfo.MinValue, vbControl.VBContentPropertyInfo.MaxValue,
                                                                    gip.core.datamodel.Database.GlobalDatabase);
                }
                else if (acValue.Option == Global.ParamOption.Optional)
                {
                    if (acValueId == "CPartnerCompany" || acValueId == "OutwardQuantityAmb" || acValueId == "InwardQuantityAmb")
                    {
                        Material material = DetermineMaterial();
                        if (material != null)
                        {
                            if (acValueId == "CPartnerCompany" && material.ContractorStock)
                            {
                                if ((this.CPartnerCompany == null) &&
                                    ((this.BookingType == GlobalApp.FacilityBookingType.InwardMovement_Facility_BulkMaterial)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.OutwardMovement_Facility_BulkMaterial)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.InwardMovement_FacilityCharge)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.OutwardMovement_FacilityCharge)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.ZeroStock_Facility_BulkMaterial)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.StockCorrection)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.InOrderPosInwardMovement)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.InOrderPosActivate)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.OutOrderPosOutwardMovement)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.OutOrderPosActivate)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosInward)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosInwardCancel)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutward)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutwardCancel)
                                    || (this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutwardOnEmptyingFacility)
                                    || this.BookingType == GlobalApp.FacilityBookingType.InventoryStockCorrection
                                    ))
                                {
                                    subResult = Global.ControlModesInfo.EnabledRequired;
                                }
                            }
                            else if ((acValueId == "OutwardQuantityAmb" || acValueId == "InwardQuantityAmb") && material.PetroleumGroup != GlobalApp.PetroleumGroups.None)
                            {
                                if (!this.InwardQuantityAmb.HasValue
                                    && (this.BookingType == GlobalApp.FacilityBookingType.InwardMovement_Facility_BulkMaterial
                                    || this.BookingType == GlobalApp.FacilityBookingType.InwardMovement_FacilityCharge
                                    || this.BookingType == GlobalApp.FacilityBookingType.InOrderPosInwardMovement
                                    || this.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel
                                    || this.BookingType == GlobalApp.FacilityBookingType.InOrderPosActivate
                                    || this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosInward
                                    || this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosInwardCancel
                                    || this.BookingType == GlobalApp.FacilityBookingType.InventoryStockCorrection
                                    ))
                                    subResult = Global.ControlModesInfo.EnabledRequired;
                                else if (!this.OutwardQuantityAmb.HasValue
                                    && (this.BookingType == GlobalApp.FacilityBookingType.OutwardMovement_Facility_BulkMaterial
                                    || this.BookingType == GlobalApp.FacilityBookingType.OutwardMovement_FacilityCharge
                                    || this.BookingType == GlobalApp.FacilityBookingType.OutOrderPosOutwardMovement
                                    || this.BookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel
                                    || this.BookingType == GlobalApp.FacilityBookingType.OutOrderPosActivate
                                    || this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutward
                                    || this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutwardCancel
                                    || (this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutwardOnEmptyingFacility)
                                    || this.BookingType == GlobalApp.FacilityBookingType.InventoryStockCorrection
                                    ))
                                    subResult = Global.ControlModesInfo.EnabledRequired;
                            }
                        }
                    }
                    else if (acValueId == "InwardFacilityLot")
                    {
                        if (this.BookingType == GlobalApp.FacilityBookingType.InOrderPosInwardMovement
                            && this.InwardFacilityLot == null
                            && this.IsLotManaged)
                        {
                            subResult = Global.ControlModesInfo.EnabledRequired;
                        }
                    }
                }
            }
            return subResult;
        }

        /// <summary>
        /// Überprüfen auf Gültigkeit der übergebenen Parameter
        /// Die Prüfung findet in zwei Schritten statt:
        /// 1. Prüfung auf die hartcodierten Eigenschaften im BookingTypeInfo
        /// 2. Prüfung auf die MDFacilityBookingType in der Datenbank gespeicherten Informationen
        /// TODO: Sinnvollen Fehlertext zusammenbauen
        /// </summary>
        public override bool IsValid()
        {
            //this.ValidMessage.ClearMsgDetails();
            this._ValidMessage = new MsgWithDetails();

            bool bValid = true;
            if (!base.IsValid())
                bValid = false;
            if (!CheckSyntaxOfProperties())
                bValid = false;
            return bValid;
        }
        #endregion

        #region private or internal methods
        /// <summary>
        /// Stufe 2 Prüfung: Syntaktische Prüfung
        /// Check ob voneinander abhängige Parameter, die unabhängig vom Buchungstyp, richtig gesetzt sind
        /// </summary>
        internal bool CheckSyntaxOfProperties()
        {
            if (Parent != null)
                return false;

            // *************************
            // ZUSÄTZLICHE SICHERHEITS-ÜBERPRÜFUNGEN FALLS neue MDFacilityBookingType definiert werden und diese Fehlerhaft sind
            // *************************

            // Mindestens eine Entität muss gesetzt werden
            if (AreFacilityEntitiesNeeded &&
                (InwardMaterial == null) && (InwardFacility == null) && (InwardFacilityCharge == null) && (InwardFacilityLot == null) && (InwardFacilityLocation == null) &&
                (OutwardMaterial == null) && (OutwardFacility == null) && (OutwardFacilityCharge == null) && (OutwardFacilityLot == null) && (OutwardFacilityLocation == null))
                AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00056"));

            // Falls Material, FacilityCharge, FacilityLot übergeben und keine Mengeneinheiten, 
            // dann können die Mengeneinheiten aus Entitäten abgeleitet werden
            if (AreQuantityParamsNeeded
                && (MDUnit == null)
                && (InwardMaterial == null) && (InwardFacilityCharge == null) && (InwardFacilityLot == null)
                && (OutwardMaterial == null) && (OutwardFacilityCharge == null) && (OutwardFacilityLot == null)
                && InOrderPos == null && OutOrderPos == null && PickingPos == null && PartslistPosRelation == null && this.PartslistPos == null)
                AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Info00013"));

            // Zusätzliche Merkmale eines Materials zur Materialindentifikation, müssen immer im Zusammenhang mit dem Material angegeben werden
            if (((InwardPartslist != null) && (InwardMaterial == null))
                || ((OutwardPartslist != null) && (OutwardMaterial == null)))
                AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00094"));

            // Falls irgendeine Inward-entity gesetzt ist, dann muss auch mindestens die Einlagermenge gesetzt werden
            if (((InwardMaterial != null) || (InwardFacility != null) || (InwardFacilityCharge != null) || (InwardFacilityLot != null) || (InwardFacilityLocation != null))
                && (InwardQuantity == null) && IsQuantityNeededForBooking)
                AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Info00005"));
            // Falls irgendeine Outward-entity gesetzt ist, dann muss auch mindestens die Auslagermenge gesetzt werden
            if (((OutwardMaterial != null) || (OutwardFacility != null) || (OutwardFacilityCharge != null) || (OutwardFacilityLot != null) || (OutwardFacilityLocation != null))
                && (OutwardQuantity == null) && IsQuantityNeededForBooking)
                AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Info00008"));

            // Angaben in Mengeneinheiten müssen immer gesetzt werden, wenn Gewichtswerte übergeben,
            // damit ungekoppelt gebucht werden kann
            //if ((InwardQuantity == null) && (OutwardQuantity == null))
            //AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Info00010"));

            // Falls Soll-Buchungswerte gesetzt dann müssen Buchungswerte auch gesetzt sein
            if ((OutwardTargetQuantity != null) && (OutwardQuantity == null))
                AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Info00009"));
            if ((InwardTargetQuantity != null) && (InwardQuantity == null))
                AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Info00003"));

            // Überprüfe ob Nullbestandsbuchungen erlaubt ist in Abhängigkeit von den übergebenen Parametern
            if (MDZeroStockState != null && MDZeroStockState.ZeroStockState >= MDZeroStockState.ZeroStockStates.BookToZeroStock)
            {
                if (((InwardMaterial != null) || (InwardFacility != null) || (InwardFacilityCharge != null) || (InwardFacilityLot != null) || (InwardFacilityLocation != null))
                   && ((OutwardMaterial != null) || (OutwardFacility != null) || (OutwardFacilityCharge != null) || (OutwardFacilityLot != null) || (OutwardFacilityLocation != null)))
                {
                    AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00081"));
                }
                else if (MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.ResetIfNotAvailable)
                {
                    if ((InwardMaterial != null) || (InwardFacility != null) || (InwardFacilityLot != null) || (InwardFacilityLocation != null)
                       || (OutwardMaterial != null) || (OutwardFacility != null) || (OutwardFacilityLot != null) || (OutwardFacilityLocation != null))
                        AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00022"));
                    else if ((InwardFacilityCharge != null) && (OutwardFacilityCharge != null))
                        AddBookingMessage(eResultCodes.RequiredParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00023"));
                }
                else // if (ZeroStock <= Global.ZeroStockState.SetNotAvailable)
                {
                }
            }

            // Reservierungsbuchungen können bei Umlagerungsbuchungen nicht durchgeführt werden
            // Überprüfe ob Inward- und Outward-Felder gesetzt sind.
            if (MDReservationMode != null && MDReservationMode.ReservationMode != MDReservationMode.ReservationModes.Off)
            {
                if (((InwardMaterial != null) || (InwardFacility != null) || (InwardFacilityCharge != null) || (InwardFacilityLot != null) || (InwardFacilityLocation != null))
                   && ((OutwardMaterial != null) || (OutwardFacility != null) || (OutwardFacilityCharge != null) || (OutwardFacilityLot != null) || (OutwardFacilityLocation != null)))
                {
                    AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00080"));
                }
            }

            if (MDReservationMode != null)
            {
                // Falls keine vollständig aufzulösende Reservierung, dann müssen Mengen übergeben werden damit aus dem Lager gebucht werden kann
                // Umgekehrt, wenn eine Reservierung gemacht werden soll, müssen Mengen übergeben werden
                if (MDReservationMode != null && MDReservationMode.ReservationMode != MDReservationMode.ReservationModes.CancelComplete)
                {
                    if (((InwardMaterial != null) || (InwardFacility != null) || (InwardFacilityCharge != null) || (InwardFacilityLot != null) || (InwardFacilityLocation != null))
                        && (InwardQuantity == null) && IsQuantityNeededForBooking)
                        AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Info00006"));
                    if (((OutwardMaterial != null) || (OutwardFacility != null) || (OutwardFacilityCharge != null) || (OutwardFacilityLot != null) || (OutwardFacilityLocation != null))
                        && (OutwardQuantity == null) && IsQuantityNeededForBooking)
                        AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Info00011"));
                }

                // Check ob bei Reservierung eine Entität übergeben
                if (MDReservationMode.ReservationMode == MDReservationMode.ReservationModes.Reserve || MDReservationMode.ReservationMode == MDReservationMode.ReservationModes.CancelComplete)
                {
                    if ((InOrderPos == null) && (OutOrderPos == null))
                        AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00037"));
                    // TODO: Hier müssen künftige Entitäten ebenfalls eingetragen werden
                }
                else if (MDReservationMode.ReservationMode == MDReservationMode.ReservationModes.CancelSubset)
                {
                    //if ((InDeliveryNotePosLot == null) && (OutDeliveryNotePosLot == null))
                    //    AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00037"));
                    // TODO: Hier müssen künftige Entitäten ebenfalls eingetragen werden
                }
            }
            // Falls Buchungen auf einen Wareneingang und der Wareneingangsbeleg übergeben wird, dann muss auch die Bestellposition mit übergeben werden
            //if ((InDeliveryNotePosLot != null) && (InOrderPos == null))
            //    AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00038"));
            //// Falls Buchungen auf einen Warenausgang und der Warenausgangsbeleg übergeben wird, dann muss auch die Bestellposition mit übergeben werden
            //if ((OutDeliveryNotePosLot != null) && (OutOrderPos == null))
            //    AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00039"));
            // TODO: Weitere künftige Entitäten


            //if (/*(InDeliveryNotePosLot != null) && */(InOrderPos != null))
            //{
            //    if ((OutwardMaterial != null) || (OutwardFacility != null) || (OutwardFacilityCharge != null) || (OutwardFacilityLot != null) || (OutwardFacilityLocation != null)
            //        || (OutwardQuantity != null))
            //        AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00057"));
            //}

            //if (/*(OutDeliveryNotePosLot != null) &&*/ (OutOrderPos != null))
            //{
            //    if ((InwardMaterial != null) || (InwardFacility != null) || (InwardFacilityCharge != null) || (InwardFacilityLot != null) || (InwardFacilityLocation != null)
            //        || (InwardQuantity != null))
            //        AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00042"));
            //}
            if ((this.BookingType == GlobalApp.FacilityBookingType.Reassign_FacilityCharge
                    || this.BookingType == GlobalApp.FacilityBookingType.Reassign_Facility_BulkMaterial)
                    && !this.IsLotManaged)
                AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error50056"));

            Material material = DetermineMaterial();
            if ((material != null) && (material.ContractorStock))
            {
                if ((this.CPartnerCompany == null)
                       && ((this.BookingType == GlobalApp.FacilityBookingType.InwardMovement_Facility_BulkMaterial)
                        || (this.BookingType == GlobalApp.FacilityBookingType.OutwardMovement_Facility_BulkMaterial)
                        || (this.BookingType == GlobalApp.FacilityBookingType.InwardMovement_FacilityCharge)
                        || (this.BookingType == GlobalApp.FacilityBookingType.OutwardMovement_FacilityCharge)
                        || (this.BookingType == GlobalApp.FacilityBookingType.ZeroStock_FacilityCharge)
                        || (this.BookingType == GlobalApp.FacilityBookingType.ZeroStock_Facility_BulkMaterial)
                        || (this.BookingType == GlobalApp.FacilityBookingType.StockCorrection)
                        || (this.BookingType == GlobalApp.FacilityBookingType.InOrderPosInwardMovement)
                        || (this.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel)
                        || (this.BookingType == GlobalApp.FacilityBookingType.InOrderPosActivate)
                        || (this.BookingType == GlobalApp.FacilityBookingType.OutOrderPosOutwardMovement)
                        || (this.BookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel)
                        || (this.BookingType == GlobalApp.FacilityBookingType.OutOrderPosActivate)
                        || (this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosInward)
                        || (this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosInwardCancel)
                        || (this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutward)
                        || (this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutwardCancel)
                        || (this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutwardOnEmptyingFacility)
                        || this.BookingType == GlobalApp.FacilityBookingType.InventoryStockCorrection
                    ))
                {
                    AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Info50001"));
                }
                else if (material.PetroleumGroup != GlobalApp.PetroleumGroups.None)
                {
                    if (!this.InwardQuantityAmb.HasValue
                        && (this.BookingType == GlobalApp.FacilityBookingType.InwardMovement_Facility_BulkMaterial
                        || this.BookingType == GlobalApp.FacilityBookingType.InwardMovement_FacilityCharge
                        || this.BookingType == GlobalApp.FacilityBookingType.InOrderPosInwardMovement
                        || this.BookingType == GlobalApp.FacilityBookingType.InOrderPosCancel
                        || this.BookingType == GlobalApp.FacilityBookingType.InOrderPosActivate
                        || this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosInward
                        || this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosInwardCancel
                        || this.BookingType == GlobalApp.FacilityBookingType.InventoryStockCorrection
                        ))
                        AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Info50010"));
                    else if (!this.OutwardQuantityAmb.HasValue
                        && (this.BookingType == GlobalApp.FacilityBookingType.OutwardMovement_Facility_BulkMaterial
                        || this.BookingType == GlobalApp.FacilityBookingType.OutwardMovement_FacilityCharge
                        || this.BookingType == GlobalApp.FacilityBookingType.OutOrderPosOutwardMovement
                        || this.BookingType == GlobalApp.FacilityBookingType.OutOrderPosCancel
                        || this.BookingType == GlobalApp.FacilityBookingType.OutOrderPosActivate
                        || this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutward
                        || this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutwardCancel
                        || (this.BookingType == GlobalApp.FacilityBookingType.ProdOrderPosOutwardOnEmptyingFacility)
                        || this.BookingType == GlobalApp.FacilityBookingType.InventoryStockCorrection
                        ))
                        AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Info50010"));
                }
            }
            else if (material != null 
                        && material.MDFacilityManagementType != null 
                        && (    material.MDFacilityManagementType.FacilityManagementType == MDFacilityManagementType.FacilityManagementTypes.NoFacility))
                            //|| (BookingType == GlobalApp.FacilityBookingType.Split_FacilityCharge && material.MDFacilityManagementType.FacilityManagementType != MDFacilityManagementType.FacilityManagementTypes.FacilityCharge)))
                AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Info50011"));
            else if (this.BookingType == GlobalApp.FacilityBookingType.InOrderPosInwardMovement
                && (this.InwardFacilityLot == null && this.InwardFacilityCharge == null)
                && this.IsLotManaged)
            {
                AddBookingMessage(eResultCodes.DependingParamsNotSet, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error50049"));
            }

            SetMessageLevelOfHead();
            return this.ValidMessage.IsSucceded();
        }

        /// <summary>
        /// Stufe 3 Prüfung: Logische und inhaltliche Prüfung
        /// Darf nur nach Aufruf von CheckSyntaxOfProperties() aufgerufen werden!
        /// - Prüfung ob in den Buchungsparametern übergebene Inhalte schlüssig sind
        /// z.B. Passt Material mit Silobelegung zusammen...
        /// - Falls Parameter inhaltlich nicht sauber zusammenpassen führe Korrekturen durch (kleine Fehler)
        ///   Falls schwerwiegende Fehler, dann Abbruch
        /// - Initialisiere Lokale Buchungsparameter, die zur Buchungsverarbeitung genutzt werden
        /// </summary>
        public bool CheckAndAdjustPropertiesForBooking(DatabaseApp dbApp)
        {
            if (dbApp == null)
                return false;
            Database = dbApp;

            InitRequiredDefaultParameterIfNotSet();

            CopyThisToAdjustedParams();

            // TODO: Freigabe/Sperrung ReleaseState auswerten. Freigeben und Sperren von Chargen muss getrennt aufgerufen werden

            // 1. Setze Soll-Buchungswerte mit Buchungswerten, falls diese von exteren nicht belegt worden sind
            CheckAndAdjustTargetQuantityProperties();

            // 2. Ermittle Material (wegen Chargenführungskennzeichen) auf dem die Buchungen später ausgeführt werden
            if (!CheckAndAdjustMaterial())
                return false;

            // 3. Checke Vertragspartnerinformationen
            if (!CheckAndAdjustCompanyMaterial())
                return false;

            // 4. Aktualisiere Mengeneinheit, falls diese von ausserhalb nicht gesetzt worden ist
            if (!CheckAndAdjustQuantityAndUnit())
                return false;

            // 5. Ermittle Facility und ob Buchungen darauf erlaubt sind
            if (!CheckAndAdjustFacility())
                return false;

            // 6. Überprüfe, dass Partslist bei Inward und Outward gleich übergeben ist
            if (!CheckAndAdjustPartslist())
                return false;

            // 7. Überprüfe abhängig ob Material Chargengeführt ist oder nicht ob richtige LagerEntitäts-Kombinationen gesetzt sind
            if (!CheckAndAdjustEntityCombinations())
                return false;

            return true;
        }

        /// <summary>
        /// Funktion setzt Soll-Buchungswerte mit Buchungswerten, falls diese von exteren nicht belegt worden sind
        /// </summary>
        private void CheckAndAdjustTargetQuantityProperties()
        {
            if ((ParamsAdjusted.OutwardTargetQuantity == null) && (ParamsAdjusted.OutwardQuantity != null))
                ParamsAdjusted.OutwardTargetQuantity = ParamsAdjusted.OutwardQuantity;
            if ((ParamsAdjusted.InwardTargetQuantity == null) && (ParamsAdjusted.InwardQuantity != null))
                ParamsAdjusted.InwardTargetQuantity = ParamsAdjusted.InwardQuantity;
            if ((ParamsAdjusted.OutwardTargetQuantityAmb == null) && (ParamsAdjusted.OutwardQuantityAmb != null))
                ParamsAdjusted.OutwardTargetQuantityAmb = ParamsAdjusted.OutwardQuantityAmb;
            if ((ParamsAdjusted.InwardTargetQuantityAmb == null) && (ParamsAdjusted.InwardQuantityAmb != null))
                ParamsAdjusted.InwardTargetQuantityAmb = ParamsAdjusted.InwardQuantityAmb;
        }

        /// <summary>
        /// Funktion Aktualisiert Mengeneinheit, falls diese von ausserhalb nicht gesetzt worden ist
        /// </summary>
        private bool CheckAndAdjustQuantityAndUnit()
        {
            // Falls QuantityUnit übergeben, checke 
            // 1. ob übergebene Einheit in die Basiseinheit des Material konvertiert werden kannn
            // 2. ob übergebene  Einheit als Lagereinheit verwaltet werden kann
            if (ParamsAdjusted.MDUnit != null)
            {
                // 1. ob übergebene Einheit in die Basiseinheit des Material konvertiert werden kannn
                if (ParamsAdjusted.InwardMaterial == null && ParamsAdjusted.OutwardMaterial == null)
                    return false; // Darf nicht vorkommen, da zuvor mit CheckAndAdjustMaterial() überprüft
                if (ParamsAdjusted.InwardMaterial != null)
                {
                    if (ParamsAdjusted.InwardMaterial.BaseMDUnit.MDUnitID != ParamsAdjusted.MDUnit.MDUnitID)
                    {
                        if (!ParamsAdjusted.InwardMaterial.IsConvertableToBaseUnit(ParamsAdjusted.MDUnit))
                        {
                            AddBookingMessage(eResultCodes.DependingParamsHasWrongValue, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00053"));
                            return false;
                        }
                    }
                }
                else
                {
                    if (ParamsAdjusted.OutwardMaterial.BaseMDUnit.MDUnitID != ParamsAdjusted.MDUnit.MDUnitID)
                    {
                        if (!ParamsAdjusted.OutwardMaterial.IsConvertableToBaseUnit(ParamsAdjusted.MDUnit))
                        {
                            AddBookingMessage(eResultCodes.DependingParamsHasWrongValue, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00053"));
                            return false;
                        }
                    }
                }

                // 2. ob übergebene Einheit als Lagereinheit verwaltet werden kann
                if (ParamsAdjusted.InwardFacilityCharge != null && ParamsAdjusted.InwardMaterial != null)
                {
                    if (ParamsAdjusted.InwardFacilityCharge.MDUnit != null && ParamsAdjusted.MDUnit.MDUnitID != ParamsAdjusted.InwardFacilityCharge.MDUnit.MDUnitID)
                    {
                        if (!ParamsAdjusted.InwardMaterial.IsConvertableToUnit(ParamsAdjusted.MDUnit, ParamsAdjusted.InwardFacilityCharge.MDUnit))
                        {
                            AddBookingMessage(eResultCodes.DependingParamsHasWrongValue, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00053"));
                            return false;
                        }
                    }
                }
                if (ParamsAdjusted.OutwardFacilityCharge != null && ParamsAdjusted.OutwardMaterial != null)
                {
                    if (ParamsAdjusted.OutwardFacilityCharge.MDUnit != null && ParamsAdjusted.MDUnit.MDUnitID != ParamsAdjusted.OutwardFacilityCharge.MDUnit.MDUnitID)
                    {
                        if (!ParamsAdjusted.OutwardMaterial.IsConvertableToUnit(ParamsAdjusted.MDUnit, ParamsAdjusted.OutwardFacilityCharge.MDUnit))
                        {
                            AddBookingMessage(eResultCodes.DependingParamsHasWrongValue, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00053"));
                            return false;
                        }
                    }
                }
            }
            // Falls keine Einheit übergeben, dann ist Menge 
            // 1. entweder in Lagereinheiten verwaltet
            // 2. oder in Baismengeneinheiten verwaltet
            else
            {
                if (ParamsAdjusted.MDUnit == null && ParamsAdjusted.InwardFacilityCharge != null)
                {
                    if (ParamsAdjusted.InwardFacilityCharge.MDUnit != null)
                        ParamsAdjusted.MDUnit = ParamsAdjusted.InwardFacilityCharge.MDUnit;
                    else if (ParamsAdjusted.InwardFacilityCharge.Material != null)
                        ParamsAdjusted.MDUnit = ParamsAdjusted.InwardFacilityCharge.Material.BaseMDUnit;
                }
                if (ParamsAdjusted.MDUnit == null && ParamsAdjusted.OutwardFacilityCharge != null)
                {
                    if (ParamsAdjusted.OutwardFacilityCharge.MDUnit != null)
                        ParamsAdjusted.MDUnit = ParamsAdjusted.OutwardFacilityCharge.MDUnit;
                    else if (ParamsAdjusted.OutwardFacilityCharge.Material != null)
                        ParamsAdjusted.MDUnit = ParamsAdjusted.OutwardFacilityCharge.Material.BaseMDUnit;
                }

                if (ParamsAdjusted.MDUnit == null && ParamsAdjusted.InwardMaterial != null)
                    ParamsAdjusted.MDUnit = ParamsAdjusted.InwardMaterial.BaseMDUnit;
                if (ParamsAdjusted.MDUnit == null && ParamsAdjusted.OutwardMaterial != null)
                    ParamsAdjusted.MDUnit = ParamsAdjusted.OutwardMaterial.BaseMDUnit;

                if (ParamsAdjusted.MDUnit == null && ParamsAdjusted.InwardFacility != null && ParamsAdjusted.InwardFacility.Material != null)
                    ParamsAdjusted.MDUnit = ParamsAdjusted.InwardFacility.Material.BaseMDUnit;
                if (ParamsAdjusted.MDUnit == null && ParamsAdjusted.OutwardFacility != null && ParamsAdjusted.OutwardFacility.Material != null)
                    ParamsAdjusted.MDUnit = ParamsAdjusted.OutwardFacility.Material.BaseMDUnit;
            }

            if (ParamsAdjusted.MDUnit == null)
            {
                if (!AreQuantityParamsNeeded)
                    return true;
            }

            if (this.BookingType == GlobalApp.FacilityBookingType.Reassign_FacilityCharge)
            {
                if (ParamsAdjusted.OutwardFacilityCharge != null)
                {
                    if (   ParamsAdjusted.InwardMaterial != null 
                        && ParamsAdjusted.InwardMaterial.BaseMDUnit != ParamsAdjusted.OutwardFacilityCharge.MDUnit
                        && !ParamsAdjusted.OutwardFacilityCharge.Material.IsConvertableToUnit(ParamsAdjusted.OutwardFacilityCharge.MDUnit, ParamsAdjusted.InwardMaterial.BaseMDUnit))
                    {
                        AddBookingMessage(eResultCodes.DependingParamsHasWrongValue, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00053"));
                        return false;
                    }
                }
            }


            // Runde Zahlen ab. Nötig wenn z.B. Sack als Einheit und Nachkommastellen = 0 sein sollen und eine Menge mit Nachkommastellen übergeben worden ist
            if (ParamsAdjusted.MDUnit != null && ParamsAdjusted.MDUnit.Rounding >= 0)
            {
                if (ParamsAdjusted.InwardQuantity.HasValue)
                    ParamsAdjusted.InwardQuantity = ParamsAdjusted.MDUnit.GetRoundedValue(ParamsAdjusted.InwardQuantity.Value);
                if (ParamsAdjusted.OutwardQuantity.HasValue)
                    ParamsAdjusted.OutwardQuantity = ParamsAdjusted.MDUnit.GetRoundedValue(ParamsAdjusted.OutwardQuantity.Value);
                if (ParamsAdjusted.InwardTargetQuantity.HasValue)
                    ParamsAdjusted.InwardTargetQuantity = ParamsAdjusted.MDUnit.GetRoundedValue(ParamsAdjusted.InwardTargetQuantity.Value);
                if (ParamsAdjusted.OutwardTargetQuantity.HasValue)
                    ParamsAdjusted.OutwardTargetQuantity = ParamsAdjusted.MDUnit.GetRoundedValue(ParamsAdjusted.OutwardTargetQuantity.Value);
            }


            //if (ParamsAdjusted.MaterialUnit == null)
            //    AddBookingMessage(eResultCodes.EmptyParameterCouldNotBeDerived, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00053"));
            //else if (ParamsAdjusted.MDUnit == null)
            //    ParamsAdjusted.MDUnit = ParamsAdjusted.MaterialUnit.MDUnit;
            if (ParamsAdjusted.MDUnit == null)
                AddBookingMessage(eResultCodes.EmptyParameterCouldNotBeDerived, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00054"));

            return ParamsAdjusted.MDUnit != null;
        }

        /// <summary>
        /// Funktion Ermittelt Material (wegen Chargenführungskennzeichen) auf dem die Buchungen später ausgeführt werden
        /// Wenn Material ermittelt, dann werden Propeties im lokalen BookingParameter gesetzt, dass unterschieden werden kann
        /// was externe Funktion gesetzt hat und was der interne Buchungsalgorithmus
        /// </summary>
        private bool CheckAndAdjustMaterial()
        {
            /// Pseudo-Code:
            /// Falls Material übergeben
            ///     Setze AdjustedMaterial mit Material
            /// Falls Material nicht übergeben
            ///     Falls Facility(Lagerplatz) übergeben, checke ob Facility ein Behältnis (Silo, Tank) ist
            ///         Falls Facility(Lagerplatz) mit Material belegt ist
            ///         Hole AdjustedMaterial aus Facility(Lagerplatz)
            ///     Falls AdjustedMaterial nicht gefunden und FacilityCharge übergeben
            ///         Hole AdjustedMaterial aus FacilityCharge
            ///     Falls AdjustedMaterial nicht gefunden und FacilityLot übergeben
            ///         Hole AdjustedMaterial aus FacilityLot
            /// Falls Material nicht gefunden
            ///     Breche ab mit Fehler

            if ((ParamsAdjusted.OutwardMaterial != null) && (ParamsAdjusted.InwardMaterial != null) && (ParamsAdjusted.OutwardMaterial != ParamsAdjusted.InwardMaterial))
            {
                AddBookingMessage(eResultCodes.DependingParamsHasWrongValue,
                    Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00013", "InwardMaterial", "OutwardMaterial"));
                return false;
            }

            if (ParamsAdjusted.OutwardFacility != null || ParamsAdjusted.InwardFacility != null)
            {
                if (ParamsAdjusted.OutwardFacility != null
                    && ParamsAdjusted.OutwardFacility.MDFacilityType != null
                    && ParamsAdjusted.OutwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer
                    && ParamsAdjusted.OutwardFacility.Material != null
                    && ((ParamsAdjusted.OutwardMaterial == null)
                                || (ParamsAdjusted.OutwardMaterial == ParamsAdjusted.OutwardFacility.Material)
                                || (ParamsAdjusted.OutwardMaterial.ProductionMaterialID.HasValue && ParamsAdjusted.OutwardMaterial.ProductionMaterialID == ParamsAdjusted.OutwardFacility.MaterialID)))
                {
                    ParamsAdjusted.OutwardMaterial = ParamsAdjusted.OutwardFacility.Material;
                }
                if (ParamsAdjusted.InwardFacility != null
                    && ParamsAdjusted.InwardFacility.MDFacilityType != null
                    && ParamsAdjusted.InwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer
                    && ParamsAdjusted.InwardFacility.Material != null
                    && ((ParamsAdjusted.InwardMaterial == null)
                                || (ParamsAdjusted.InwardMaterial == ParamsAdjusted.InwardFacility.Material)
                                || (ParamsAdjusted.InwardMaterial.ProductionMaterialID.HasValue && ParamsAdjusted.InwardMaterial.ProductionMaterialID == ParamsAdjusted.InwardFacility.MaterialID)))
                {
                    ParamsAdjusted.InwardMaterial = ParamsAdjusted.InwardFacility.Material;
                }

                if (ParamsAdjusted.OutwardMaterial != null && ParamsAdjusted.InwardMaterial == null)
                    ParamsAdjusted.InwardMaterial = ParamsAdjusted.OutwardMaterial;
                else if (ParamsAdjusted.OutwardMaterial == null && ParamsAdjusted.InwardMaterial != null)
                    ParamsAdjusted.OutwardMaterial = ParamsAdjusted.InwardMaterial;

                if (ParamsAdjusted.InwardMaterial != null || ParamsAdjusted.OutwardMaterial != null)
                    return true;
            }

            if (ParamsAdjusted.OutwardFacilityCharge != null || ParamsAdjusted.InwardFacilityCharge != null)
            {
                if (ParamsAdjusted.OutwardMaterial == null
                    && ParamsAdjusted.OutwardFacilityCharge != null
                    && ParamsAdjusted.OutwardFacilityCharge.Material != null)
                {
                    ParamsAdjusted.OutwardMaterial = ParamsAdjusted.OutwardFacilityCharge.Material;
                }
                if (ParamsAdjusted.InwardMaterial == null
                    && ParamsAdjusted.InwardFacilityCharge != null
                    && ParamsAdjusted.InwardFacilityCharge.Material != null)
                {
                    ParamsAdjusted.InwardMaterial = ParamsAdjusted.InwardFacilityCharge.Material;
                }
                if (ParamsAdjusted.OutwardMaterial != null && ParamsAdjusted.InwardMaterial == null)
                    ParamsAdjusted.InwardMaterial = ParamsAdjusted.OutwardMaterial;
                else if (ParamsAdjusted.OutwardMaterial == null && ParamsAdjusted.InwardMaterial != null)
                    ParamsAdjusted.OutwardMaterial = ParamsAdjusted.InwardMaterial;

                if (ParamsAdjusted.InwardMaterial != null || ParamsAdjusted.OutwardMaterial != null)
                    return true;
            }

            if (ParamsAdjusted.OutwardFacilityLot != null || ParamsAdjusted.InwardFacilityLot != null)
            {
                if (ParamsAdjusted.OutwardFacilityLot != null
                    && ParamsAdjusted.OutwardFacilityLot.Material != null
                    && ParamsAdjusted.OutwardMaterial == null)
                {
                    ParamsAdjusted.OutwardMaterial = ParamsAdjusted.OutwardFacilityLot.Material;
                }
                if (ParamsAdjusted.InwardFacilityLot != null
                    && ParamsAdjusted.InwardFacilityLot.Material != null
                    && ParamsAdjusted.InwardMaterial == null)
                {
                    ParamsAdjusted.InwardMaterial = ParamsAdjusted.InwardFacilityLot.Material;
                }
                if (ParamsAdjusted.OutwardMaterial != null && ParamsAdjusted.InwardMaterial == null)
                    ParamsAdjusted.InwardMaterial = ParamsAdjusted.OutwardMaterial;
                else if (ParamsAdjusted.OutwardMaterial == null && ParamsAdjusted.InwardMaterial != null)
                    ParamsAdjusted.OutwardMaterial = ParamsAdjusted.InwardMaterial;

                if (ParamsAdjusted.InwardMaterial != null || ParamsAdjusted.OutwardMaterial != null)
                    return true;
            }

            if (ParamsAdjusted.OutwardFacility != null || ParamsAdjusted.InwardFacility != null)
            {
                if (ParamsAdjusted.OutwardFacility != null
                    && ParamsAdjusted.OutwardFacility.MDFacilityType != null
                    && ParamsAdjusted.OutwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer
                    && ParamsAdjusted.OutwardFacility.Material == null
                    && ParamsAdjusted.OutwardFacility.FacilityCharge_Facility.Any())
                {
                    ParamsAdjusted.OutwardMaterial = ParamsAdjusted.OutwardFacility.FacilityCharge_Facility.First().Material;
                }
                if (ParamsAdjusted.InwardFacility != null
                    && ParamsAdjusted.InwardFacility.MDFacilityType != null
                    && ParamsAdjusted.InwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer
                    && ParamsAdjusted.InwardFacility.Material == null
                    && ParamsAdjusted.InwardFacility.FacilityCharge_Facility.Any())
                {
                    ParamsAdjusted.InwardMaterial = ParamsAdjusted.OutwardMaterial;
                }
                if (ParamsAdjusted.OutwardMaterial != null && ParamsAdjusted.InwardMaterial == null)
                    ParamsAdjusted.InwardMaterial = ParamsAdjusted.OutwardMaterial;
                else if (ParamsAdjusted.OutwardMaterial == null && ParamsAdjusted.InwardMaterial != null)
                    ParamsAdjusted.OutwardMaterial = ParamsAdjusted.InwardMaterial;

                if (ParamsAdjusted.InwardMaterial != null || ParamsAdjusted.OutwardMaterial != null)
                    return true;
            }

            if (ParamsAdjusted.OutwardMaterial != null && ParamsAdjusted.InwardMaterial == null)
            {
                ParamsAdjusted.InwardMaterial = ParamsAdjusted.OutwardMaterial;
                return true;
            }
            else if (ParamsAdjusted.OutwardMaterial == null && ParamsAdjusted.InwardMaterial != null)
            {
                ParamsAdjusted.OutwardMaterial = ParamsAdjusted.InwardMaterial;
                return true;
            }

            if (ParamsAdjusted.PickingPos != null)
            {
                if (ParamsAdjusted.InwardFacility != null)
                    ParamsAdjusted.InwardMaterial = ParamsAdjusted.PickingPos.Material;
                if (ParamsAdjusted.OutwardFacility != null)
                    ParamsAdjusted.OutwardMaterial = ParamsAdjusted.PickingPos.Material;
                if (ParamsAdjusted.InwardMaterial != null || ParamsAdjusted.OutwardMaterial != null)
                    return true;
            }
            else if (ParamsAdjusted.InOrderPos != null)
            {
                if (ParamsAdjusted.InwardFacility != null)
                {
                    ParamsAdjusted.InwardMaterial = ParamsAdjusted.InOrderPos.Material;
                    return true;
                }
            }
            else if (ParamsAdjusted.OutOrderPos != null)
            {
                if (ParamsAdjusted.OutwardFacility != null)
                { 
                    ParamsAdjusted.OutwardMaterial = ParamsAdjusted.OutOrderPos.Material;
                    return true;
                }
            }
            else if (ParamsAdjusted.PartslistPosRelation != null)
            {
                if (ParamsAdjusted.OutwardFacility != null)
                { 
                    ParamsAdjusted.OutwardMaterial = ParamsAdjusted.PartslistPosRelation.SourceProdOrderPartslistPos.Material;
                    return true;
                }
            }
            else if (ParamsAdjusted.PartslistPos != null)
            {
                if (ParamsAdjusted.InwardFacility != null)
                { 
                    ParamsAdjusted.InwardMaterial = ParamsAdjusted.PartslistPos.Material;
                    return true;
                }
            }

            if (!AreFacilityEntitiesNeeded || !IsCallForBooking)
                return true;

            AddBookingMessage(eResultCodes.EmptyParameterCouldNotBeDerived, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00049"));
            return false;
        }

        protected Material DetermineMaterial()
        {
            if (InwardMaterial != null)
                return InwardMaterial;
            if (OutwardMaterial != null)
                return OutwardMaterial;
            if (OutwardFacility != null && OutwardFacility.Material != null)
                return OutwardFacility.Material;
            if (InwardFacility != null && InwardFacility.Material != null)
                return InwardFacility.Material;
            if (OutwardFacilityCharge != null && OutwardFacilityCharge.Material != null)
                return OutwardFacilityCharge.Material;
            if (InwardFacilityCharge != null && InwardFacilityCharge.Material != null)
                return InwardFacilityCharge.Material;
            if (OutwardFacilityLot != null && OutwardFacilityLot.Material != null)
                return OutwardFacilityLot.Material;
            if (InwardFacilityLot != null && InwardFacilityLot.Material != null)
                return InwardFacilityLot.Material;
            return null;
        }

        /// <summary>
        /// Falls Vertragspartner(Company) übergeben wurde, ermittelt die Funktion auf welchem CompanyMaterial
        /// der Vertragspartner-Bestand (Konto) gebucht wird
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckAndAdjustCompanyMaterial()
        {
            if (ParamsAdjusted.CPartnerCompany == null)
                return true;
            Material material = null;
            if (ParamsAdjusted.InwardMaterial != null)
                material = ParamsAdjusted.InwardMaterial;
            else if (ParamsAdjusted.OutwardMaterial != null)
                material = ParamsAdjusted.OutwardMaterial;
            if (material == null)
                return true;
            if (!material.ContractorStock)
                return true;
            var query = material.CompanyMaterial_Material.Where(c => c.CompanyID == ParamsAdjusted.CPartnerCompany.CompanyID);
            if (!query.Any())
            {
                AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                        Root.Environment.TranslateMessage(CurrentFacilityManager, "Error50006",
                                    material.MaterialNo,
                                    ParamsAdjusted.CPartnerCompany.CompanyName));
                return false;
            }
            ParamsAdjusted.CPartnerCompanyMaterial = query.First();
            return true;
        }

        /// <summary>
        /// Funktion Ermittelt Facility auf der gebucht werden soll
        /// Überprüft bei Umlagerungen, ob Silos den mit dem selben Artikel belegt sind
        /// Überprüft ob Ein- oder Auslagerungen freigegeben sind.
        /// </summary>
        protected virtual bool CheckAndAdjustFacility()
        {
            /// Pseudo-Code:
            /// Falls Facility übergeben
            ///     Setze AdjustedFacility mit Facility
            ///     
            if (BookingType == GlobalApp.FacilityBookingType.Split_FacilityCharge
                && ParamsAdjusted.OutwardFacilityCharge != null)
            {
                if (ParamsAdjusted.InwardFacility != ParamsAdjusted.OutwardFacilityCharge.Facility)
                    ParamsAdjusted.InwardFacility = ParamsAdjusted.OutwardFacilityCharge.Facility;
            }

            // Überprüfe Konfiguration auf Quelllagerplatz
            if (ParamsAdjusted.OutwardFacility != null)
            {
                if (ParamsAdjusted.OutwardFacility.MDFacilityType == null)
                {
                    AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                            Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00087",
                                        ParamsAdjusted.OutwardFacility.FacilityNo,
                                        ParamsAdjusted.OutwardFacility.FacilityName));
                    return false;
                }

                if (ParamsAdjusted.IsPhysicalBooking
                    && (ParamsAdjusted.OutwardFacility.OutwardEnabled == false && !ParamsAdjusted.IgnoreIsEnabled)
                    && (MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.Off)
                    && (ParamsAdjusted.MDMovementReason == null
                        || ParamsAdjusted.MDMovementReason.MovementReason != MovementReasonsEnum.Inventory)) // Keine Nullbestandsbuchung
                {
                    AddBookingMessage(eResultCodes.ProhibitedBooking,
                            Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00025",
                                        ParamsAdjusted.OutwardFacility.FacilityNo,
                                        ParamsAdjusted.OutwardFacility.FacilityName));
                    return false;
                }
            }

            // Überprüfe Konfiguration auf Ziellagerplatz
            if (ParamsAdjusted.InwardFacility != null)
            {
                if (ParamsAdjusted.InwardFacility.MDFacilityType == null)
                {
                    AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                            Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00087",
                                        ParamsAdjusted.InwardFacility.FacilityNo,
                                        ParamsAdjusted.InwardFacility.FacilityName));
                    return false;
                }

                if (ParamsAdjusted.IsPhysicalBooking
                    && (ParamsAdjusted.InwardFacility.InwardEnabled == false && !ParamsAdjusted.IgnoreIsEnabled)
                    && (MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.Off)
                    && (ParamsAdjusted.MDMovementReason == null
                        || ParamsAdjusted.MDMovementReason.MovementReason != MovementReasonsEnum.Inventory)) // Keine Nullbestandsbuchung
                {
                    AddBookingMessage(eResultCodes.ProhibitedBooking,
                            Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00024",
                                    ParamsAdjusted.InwardFacility.FacilityNo,
                                    ParamsAdjusted.InwardFacility.FacilityName));
                    return false;
                }
            }
            // Falls Lagerplatz-Umlagerung, 
            // Überprüfe Unterschiede zwischen Material, Rezept und Materialindentifikation auf Quelle und Ziel
            if ((ParamsAdjusted.OutwardFacility != null) && (ParamsAdjusted.InwardFacility != null))
            {
                if (   (ParamsAdjusted.OutwardFacility.MDFacilityType != null) 
                    && (ParamsAdjusted.InwardFacility.MDFacilityType != null))
                {
                    // Lagerplatztypen müssen auf beiden Seiten gleich sein
                    if (    ParamsAdjusted.OutwardFacility.MDFacilityType != ParamsAdjusted.InwardFacility.MDFacilityType
                        &&  ParamsAdjusted.InwardFacility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBin 
                        &&  ParamsAdjusted.InwardFacility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBinContainer)
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00060"));
                        return false;
                    }

                    if (ParamsAdjusted.InwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                    {
                        Material checkMaterial = ParamsAdjusted.OutwardMaterial;
                        if (ParamsAdjusted.OutwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                            checkMaterial = ParamsAdjusted.OutwardFacility.Material;

                        // Überprüfe ob Material auf Quelle und Ziel gleich sind
                        if ((checkMaterial != null) && (ParamsAdjusted.InwardFacility.Material != null)
                            && !Material.IsMaterialEqual(checkMaterial, ParamsAdjusted.InwardFacility.Material))
                        {
                            AddBookingMessage(eResultCodes.ProhibitedBooking,
                                    Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00068",
                                                ParamsAdjusted.OutwardFacility.FacilityNo, ParamsAdjusted.OutwardFacility.FacilityName, checkMaterial.MaterialNo, checkMaterial.MaterialName1,
                                                ParamsAdjusted.InwardFacility.FacilityNo, ParamsAdjusted.InwardFacility.FacilityName, ParamsAdjusted.InwardFacility.Material.MaterialNo, ParamsAdjusted.InwardFacility.Material.MaterialName1));
                            AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00062"));
                            return false;
                        }
                        // Materialbelegung muss auf Quelle vorhanden sein, wenn auf Ziel vorhanden
                        else if ((checkMaterial == null) && (ParamsAdjusted.InwardFacility.Material != null))
                        {
                            AddBookingMessage(eResultCodes.ProhibitedBooking,
                                    Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00066",
                                                ParamsAdjusted.OutwardFacility.FacilityNo, ParamsAdjusted.OutwardFacility.FacilityName,
                                                ParamsAdjusted.InwardFacility.FacilityNo, ParamsAdjusted.InwardFacility.FacilityName, ParamsAdjusted.InwardFacility.Material.MaterialNo, ParamsAdjusted.InwardFacility.Material.MaterialName1));
                            AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00064"));
                            return false;
                        }

                        // Überprüfe ob Partslist auf Quelle und Ziel gleich sind
                        if ((ParamsAdjusted.OutwardFacility.Partslist != null) && (ParamsAdjusted.InwardFacility.Partslist != null)
                            && (ParamsAdjusted.OutwardFacility.Partslist != ParamsAdjusted.InwardFacility.Partslist))
                        {
                            AddBookingMessage(eResultCodes.ProhibitedBooking,
                                    Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00070",
                                                ParamsAdjusted.OutwardFacility.FacilityNo, ParamsAdjusted.OutwardFacility.FacilityName, ParamsAdjusted.OutwardFacility.Partslist.PartslistNo, ParamsAdjusted.OutwardFacility.Partslist.PartslistName,
                                                ParamsAdjusted.InwardFacility.FacilityNo, ParamsAdjusted.InwardFacility.FacilityName, ParamsAdjusted.InwardFacility.Partslist.PartslistNo, ParamsAdjusted.InwardFacility.Partslist.PartslistName));
                            AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00063"));
                            return false;
                        }
                        // Rezeptzuordnung muss auf Quelle vorhanden sein, wenn auf Ziel vorhanden
                        else if ((ParamsAdjusted.OutwardFacility.Partslist == null) && (ParamsAdjusted.InwardFacility.Partslist != null))
                        {
                            AddBookingMessage(eResultCodes.ProhibitedBooking,
                                    Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00067",
                                                ParamsAdjusted.OutwardFacility.FacilityNo, ParamsAdjusted.OutwardFacility.FacilityName,
                                                ParamsAdjusted.InwardFacility.FacilityNo, ParamsAdjusted.InwardFacility.FacilityName, ParamsAdjusted.InwardFacility.Partslist.PartslistNo, ParamsAdjusted.InwardFacility.Partslist.PartslistName));
                            AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00065"));
                            return false;
                        }
                    }
                }
            }

            // Überprüfe Material, Rezept und Materialindentifikation auf Quelle
            if (   ParamsAdjusted.OutwardFacility != null
                && ParamsAdjusted.OutwardFacility.MDFacilityType != null)
            { 
                if (    ParamsAdjusted.OutwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer
                    && (ParamsAdjusted.InwardFacility == null || ParamsAdjusted.InwardFacility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBin))
                {
                    // Check ob Zellenbelegung mit übergebenen Material übereinstimmt
                    if (ParamsAdjusted.IsPhysicalBooking
                        && (ParamsAdjusted.OutwardFacility.Material != null)
                        && !Material.IsMaterialEqual(ParamsAdjusted.OutwardFacility.Material, ParamsAdjusted.OutwardMaterial))
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00031",
                                            ParamsAdjusted.OutwardFacility.FacilityNo, ParamsAdjusted.OutwardFacility.FacilityName, ParamsAdjusted.OutwardFacility.Material.MaterialNo, ParamsAdjusted.OutwardFacility.Material.MaterialName1,
                                            ParamsAdjusted.OutwardMaterial.MaterialNo, ParamsAdjusted.OutwardMaterial.MaterialName1));
                        return false;
                    }

                    // Check ob Zellenbelegung mit übergebenen Partslist übereinstimmt
                    if (ParamsAdjusted.IsPhysicalBooking
                        && (ParamsAdjusted.OutwardFacility.Partslist != null)
                        && (ParamsAdjusted.OutwardPartslist != null)
                        && (ParamsAdjusted.OutwardFacility.Partslist != ParamsAdjusted.OutwardPartslist))
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00033",
                                            ParamsAdjusted.OutwardFacility.FacilityNo, ParamsAdjusted.OutwardFacility.FacilityName, ParamsAdjusted.OutwardFacility.Partslist.PartslistNo, ParamsAdjusted.OutwardFacility.Partslist.PartslistName,
                                            ParamsAdjusted.OutwardPartslist.PartslistNo, ParamsAdjusted.OutwardPartslist.PartslistName));
                        return false;
                    }
                }
            }

            // Überprüfe Material, Rezept und Materialindentifikation auf Ziel
           if (    ParamsAdjusted.InwardFacility != null
                && ParamsAdjusted.InwardFacility.MDFacilityType != null)
            {
                if (ParamsAdjusted.InwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                {
                    // Check ob Zellenbelegung mit übergebenen Material übereinstimmt
                    if (ParamsAdjusted.IsPhysicalBooking
                        && (ParamsAdjusted.InwardFacility.Material != null)
                        && !Material.IsMaterialEqual(ParamsAdjusted.InwardFacility.Material, ParamsAdjusted.InwardMaterial))
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00031",
                                            ParamsAdjusted.InwardFacility.FacilityNo, ParamsAdjusted.InwardFacility.FacilityName, ParamsAdjusted.InwardFacility.Material.MaterialNo, ParamsAdjusted.InwardFacility.Material.MaterialName1,
                                            ParamsAdjusted.InwardMaterial.MaterialNo, ParamsAdjusted.InwardMaterial.MaterialName1));
                        return false;
                    }

                    // Check ob Zellenbelegung mit übergebenen Partslist übereinstimmt
                    if (ParamsAdjusted.IsPhysicalBooking
                        && (ParamsAdjusted.InwardFacility.Partslist != null)
                        && (ParamsAdjusted.InwardPartslist != null)
                        && (ParamsAdjusted.InwardFacility.Partslist != ParamsAdjusted.InwardPartslist))
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00033",
                                            ParamsAdjusted.InwardFacility.FacilityNo, ParamsAdjusted.InwardFacility.FacilityName, ParamsAdjusted.InwardFacility.Partslist.PartslistNo, ParamsAdjusted.InwardFacility.Partslist.PartslistName,
                                            ParamsAdjusted.InwardPartslist.PartslistNo, ParamsAdjusted.InwardPartslist.PartslistName));
                        return false;
                    }
                }
            }

            //if ((ParamsAdjusted.OutwardFacility != null) && (ParamsAdjusted.InwardFacility != null) && (ParamsAdjusted.InwardFacility == ParamsAdjusted.OutwardFacility))
            //{
            //    AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00041"));
            //    return false;
            //}

            return true;
        }


        /// <summary>
        /// Funktion überprüft ob OutwardFacilityLot und InwardFacilityLot ungleich sind, was nicht vorkommen darf
        /// Funktion setzt Adjusted.OutwardFacilityLot und Adjusted.InwardFacilityLot aus einer von beiden gültigen Entität
        /// </summary>
        protected virtual bool CheckAndAdjustFacilityLot()
        {
            if ((ParamsAdjusted.OutwardFacilityLot == null) && (ParamsAdjusted.InwardFacilityLot == null))
                return true;
            if (ParamsAdjusted.OutwardFacilityLot == null)
            {
                ParamsAdjusted.OutwardFacilityLot = ParamsAdjusted.InwardFacilityLot;
            }
            else if (ParamsAdjusted.InwardFacilityLot == null)
            {
                ParamsAdjusted.InwardFacilityLot = ParamsAdjusted.OutwardFacilityLot;
            }
            else if (ParamsAdjusted.OutwardFacilityLot != ParamsAdjusted.InwardFacilityLot)
            {
                AddBookingMessage(eResultCodes.DependingParamsHasWrongValue, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00013", "InwardFacilityLot", "OutwardFacilityLot"));
                return false;
            }
            return true;
        }


        /// <summary>
        /// Funktion überprüft ob OutwardPartslist und InwardPartslist ungleich sind, was nicht vorkommen darf
        /// Funktion setzt Adjusted.OutwardPartslist und Adjusted.InwardPartslist aus einer von beiden gültigen Entität
        /// </summary>
        protected virtual bool CheckAndAdjustPartslist()
        {
            if ((ParamsAdjusted.OutwardPartslist == null) && (ParamsAdjusted.InwardPartslist == null))
                return true;
            if (ParamsAdjusted.OutwardPartslist == null)
            {
                ParamsAdjusted.OutwardPartslist = ParamsAdjusted.InwardPartslist;
            }
            else if (ParamsAdjusted.InwardPartslist == null)
            {
                ParamsAdjusted.InwardPartslist = ParamsAdjusted.OutwardPartslist;
            }
            else if (ParamsAdjusted.OutwardPartslist != ParamsAdjusted.InwardPartslist)
            {
                AddBookingMessage(eResultCodes.DependingParamsHasWrongValue, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00013", "InwardPartslist", "OutwardPartslist"));
                return false;
            }
            return true;
        }


        /// <summary>
        /// Funktion überprüft ob OutwardFacilityLocation und InwardFacilityLocation gleich sind, was nicht vorkommen darf 
        /// Funktion setzt Adjusted.OutwardFacilityLocation und Adjusted.InwardFacilityLocation aus einer von beiden gültigen Entität
        /// </summary>
        protected virtual bool CheckAndAdjustFacilityLocation()
        {
            if ((ParamsAdjusted.OutwardFacilityLocation == null) && (ParamsAdjusted.InwardFacilityLocation == null))
                return true;
            /*if (Adjusted.OutwardFacilityLocation == null)
            {
                Adjusted.OutwardFacilityLocation = Adjusted.InwardFacilityLocation;
            }
            else if (Adjusted.InwardFacilityLocation == null)
            {
                Adjusted.InwardFacilityLocation = Adjusted.OutwardFacilityLocation;
            }
            else */
            if (ParamsAdjusted.OutwardFacilityLocation == ParamsAdjusted.InwardFacilityLocation)
            {
                AddBookingMessage(eResultCodes.DependingParamsHasWrongValue, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00012", "InwardFacilityLocation", "OutwardFacilityLocation"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Funktion die überprüft ob die Kombinationen aus 
        /// [Facility]-[FacilityLocation]-[FacilityCharge]-[FacilityLot]-[Material]
        /// gültig sind im Zusammenhang mit dem Chargenführungskennzeichen des Materials
        /// </summary>
        protected virtual bool CheckAndAdjustEntityCombinations()
        {
            #region if Adjusted.InwardMaterial.LotManaged
            // Falls Material Chargengeführt ist
            if (ParamsAdjusted.IsLotManaged)
            {
                /// Es gibt folgende Buchungsfälle:
                /// [Übergebene Entitäten]:
                /// ..........................
                /// 1. [FacilityCharge]:                           
                /// Zur Bestandsveränderung oder Umlagerung einer Facility-Charge
                if ((OutwardFacilityCharge != null) || (InwardFacilityCharge != null))
                {
                    if (!CAABookingOn_FacilityCharge())
                        return false;
                }

                /// 2. [FacilityLocation]
                else if ((OutwardFacilityLocation != null) || (InwardFacilityLocation != null))
                {
                    if (!CAABookingOn_FacilityLocation())
                        return false;
                    if ((OutwardMaterial != null) || (InwardMaterial != null))
                    {
                        /// 2.1 [FacilityLocation],[Material]:		        
                        /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit Material vorhanden sind
                        if ((OutwardFacilityLot == null) && (InwardFacilityLot == null))
                        {
                            if (!CAABookingOn_FacilityLocation_Material())
                                return false;
                        }
                        else
                        {
                            /// 2.3 [FacilityLocation],[FacilityLot],[Material]:
                            /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit FacilityLot und Material vorhanden
                            if (!CAABookingOn_FacilityLocation_FacilityLot_Material())
                                return false;
                        }
                    }
                    /// 2.2 [FacilityLocation],[FacilityLot]:			
                    /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit FacilityLot vorhanden sind
                    else if ((OutwardFacilityLot != null) || (InwardFacilityLot != null))
                    {
                        if (!CAABookingOn_FacilityLocation_FacilityLot())
                            return false;
                    }
                }

                /// 3. [Facility]:   
                else if ((OutwardFacility != null) || (InwardFacility != null))
                {
                    // Buchung ohne Angabe von Material und FacilityLot -> Silobuchung
                    if (!CAABookingOn_Facility())
                        return false;
                    if ((OutwardMaterial != null) || (InwardMaterial != null))
                    {
                        /// 3.1. [Facility],[Material]:   
                        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
                        /// Und zum anlegen von Anonymen Chargen, weil zu dem Buchungszeitpunkt noch keine Cahrgeninformation da war
                        if ((OutwardFacilityLot == null) && (InwardFacilityLot == null))
                        {
                            if (!CAABookingOn_Facility_Material())
                                return false;
                        }
                        else
                        {
                            /// 3.3. [Facility],[Material],[FacilityLot]:
                            /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
                            /// Es werden KEINE Anonynmen Chargen angelegt, aber mit Materialien die dem übergbenen Material entsprechen!
                            /// Falls anonyme Chargen vorhanden sind, dann wird der anonyme Status aufgehoben durch Ersetzung mit FacilityLot
                            if (!CAABookingOn_Facility_FacilityLot_Material())
                                return false;
                        }
                    }
                    /// 3.2. [Facility],[FacilityLot]:  
                    /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
                    /// Es werden KEINE Anonynmen Chargen angelegt!
                    else if ((OutwardFacilityLot != null) || (InwardFacilityLot != null))
                    {
                        if (!CAABookingOn_Facility_FacilityLot())
                            return false;
                    }
                    /// Buchung auf Lagerplätzen oder Umbuchung zwischen Lagerplätzen ohne Materialinformation ist nicht erlaubt
                    /// Wurde bereits oben gecheckt
                    else
                    {
                        //MsgBookingResult.AddBookingMessage(MsgBooking.eResultCodes.WrongParameterCombinations, "Bookings on Facility without passing informations about the [Material] are not possible.");
                        //return false;
                    }
                }

                /// 4. [FacilityLot]:                              
                else if ((OutwardFacilityLot != null) || (InwardFacilityLot != null))
                {
                    /// Zur Stackbuchung wenn FacilityChargen von FacilityLot vorhanden
                    if (!CAABookingOn_FacilityLot())
                        return false;
                    /// 4.1.[FacilityLot],[Material]:			        
                    /// Zur Stackbuchung wenn FacilityCharge mit Materialnummer vorhanden
                    if ((OutwardMaterial != null) || (InwardMaterial != null))
                    {
                        if (!CAABookingOn_FacilityLot_Material())
                            return false;
                    }
                    // else gibt es nicht, weil alle Fälle bereits vorher abgefangen worden sind
                }
            }
            #endregion
            #region else !Adjusted.InwardMaterial.LotManaged
            // Sonst nicht chargengeführtes Material
            else // if (!ParamsAdjusted.IsLotManaged)
            {
                ///     Pseudo-code:
                ///     Check: [FacilityCharge], [FacilityLot] darf nicht übergeben sein
                ///         Breche ab mit Fehler
                if ((InwardFacilityLot != null) || (OutwardFacilityLot != null))
                {
                    AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00116"));
                    return false;
                }

                /// Es gibt folgende Buchungsfälle:
                /// [Übergebene Entitäten]:
                /// ..........................
                /// 1. [FacilityCharge]:                           
                /// Zur Bestandsveränderung oder Umlagerung einer Facility-Charge
                if ((OutwardFacilityCharge != null) || (InwardFacilityCharge != null))
                {
                    if (!CAABookingOn_FacilityCharge_NotLotManaged())
                        return false;
                }

                /// 
                ///     Es gibt folgende Buchungsfälle:
                ///     [Übergebene Entitäten]:
                ///     ..................................
                ///     [FacilityLocation] oder [Facility]
                ///
                /// 5. [FacilityLocation]
                else if ((OutwardFacilityLocation != null) || (InwardFacilityLocation != null))
                {
                    /// Stackbuchung auf Lagerortebene
                    if (!CAABookingOn_FacilityLocation_Material_NotLotManaged())
                        return false;
                }
                /// 6. [Facility]:   
                else if ((OutwardFacility != null) || (InwardFacility != null))
                {
                    // Buchung ohne Angabe von Material und FacilityLot -> Silobuchung
                    if (!CAABookingOn_Facility_NotLotManaged())
                        return false;
                    if ((OutwardMaterial != null) || (InwardMaterial != null))
                    {
                        /// 6.1. [Facility],[Material]:   
                        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
                        /// Und zum anlegen von Anonymen Chargen
                        if (!CAABookingOn_Facility_Material_NotLotManaged())
                            return false;
                    }
                }
                // else Zweig kann nie eintreten
            }
            #endregion
            return true;
        }

        #region Checks for different Booking-Types on Lot-Managed Materials / Entity-Combinations
        /// <summary>
        /// 1. [FacilityCharge]:                           
        /// Zur Bestandsveränderung oder Umlagerung einer Facility-Charge
        /// </summary>
        protected virtual bool CAABookingOn_FacilityCharge()
        {
            if (!AreFacilityEntitiesNeeded)
                return true;

            ///     Pseudo-Code:        
            ///     FacilityLot, Facility, Material, FacilityLocation darf nicht übergeben werden
            ///         Breche ab mit Fehler
            if ((OutwardFacilityCharge == null) && (InwardFacilityCharge == null))
                return false;
            if (OutwardFacilityCharge != null)
            {
                if (((OutwardFacility != null && (OutwardFacility != OutwardFacilityCharge.Facility)) && OutwardMaterial != null && (OutwardFacilityLot != null)) || (OutwardFacilityLocation != null))
                {
                    AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00111"));
                    return false;
                }
                if (MDZeroStockState.ZeroStockState != MDZeroStockState.ZeroStockStates.Off)
                {
                    AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00118"));
                    return false;
                }

                if (OutwardFacilityCharge.Facility != null)
                {
                    if (OutwardFacilityCharge.Facility.OutwardEnabled == false && !ParamsAdjusted.IgnoreIsEnabled)
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00103",
                                            OutwardFacilityCharge.Facility.FacilityNo, OutwardFacilityCharge.Facility.FacilityName, OutwardFacilityCharge.Material.MaterialNo, OutwardFacilityCharge.Material.MaterialName1,
                                            OutwardFacilityCharge.Facility.FacilityNo, OutwardFacilityCharge.Facility.FacilityName));
                        return false;
                    }
                }

                // Überprüfe, ob Rezept unterschiedlich
                if ((ParamsAdjusted.OutwardPartslist != null) && (ParamsAdjusted.OutwardPartslist != OutwardFacilityCharge.Partslist))
                {
                    AddBookingMessage(eResultCodes.ProhibitedBooking,
                            Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00030",
                                        ParamsAdjusted.OutwardPartslist.PartslistNo, ParamsAdjusted.OutwardPartslist.PartslistName));
                    return false;
                }
            }
            if (InwardFacilityCharge != null)
            {
                if (   (InwardFacility != null && InwardFacility.FacilityID != InwardFacilityCharge.FacilityID) 
                    || (InwardFacilityLocation != null && InwardFacilityLocation != InwardFacilityCharge.Facility.GetFirstParentOfType(FacilityTypesEnum.StorageLocation)) 
                    || (InwardFacilityLot != null && InwardFacilityLot.FacilityLotID != InwardFacilityCharge.FacilityLotID) 
                    || (InwardMaterial != null && InwardMaterial.MaterialID != InwardFacilityCharge.MaterialID))
                {
                    AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00111"));
                    return false;
                }
                if (InwardFacilityCharge.Facility != null)
                {
                    if (InwardFacilityCharge.Facility.InwardEnabled == false && !ParamsAdjusted.IgnoreIsEnabled)
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00089",
                                            InwardFacilityCharge.Facility.FacilityNo, InwardFacilityCharge.Facility.FacilityName, InwardFacilityCharge.Material.MaterialNo, InwardFacilityCharge.Material.MaterialName1,
                                            InwardFacilityCharge.Facility.FacilityNo, InwardFacilityCharge.Facility.FacilityName));
                        return false;
                    }
                }

                // Überprüfe, ob Rezept unterschiedlich
                if ((ParamsAdjusted.InwardPartslist != null) && (ParamsAdjusted.InwardPartslist != InwardFacilityCharge.Partslist))
                {
                    AddBookingMessage(eResultCodes.ProhibitedBooking,
                            Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00028",
                                        ParamsAdjusted.InwardPartslist.PartslistNo, ParamsAdjusted.InwardPartslist.PartslistName));
                    return false;
                }
            }
            // Falls Umlagerungsbuchung von FacilityCharge nach FacilityCharge
            if ((OutwardFacilityCharge != null) && (InwardFacilityCharge != null))
            {
                if (MDZeroStockState.ZeroStockState != MDZeroStockState.ZeroStockStates.Off)
                {
                    AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00118"));
                    return false;
                }

                if (OutwardFacilityCharge == InwardFacilityCharge)
                {
                    AddBookingMessage(eResultCodes.DependingParamsHasWrongValue, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00012", "OutwardFacilityCharge", "InwardFacilityCharge"));
                    return false;
                }
                else
                {
                    if (OutwardFacilityCharge.Material != InwardFacilityCharge.Material)
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00018",
                                    OutwardFacilityCharge.Material.MaterialNo, OutwardFacilityCharge.Material.MaterialName1,
                                    InwardFacilityCharge.Material.MaterialNo, InwardFacilityCharge.Material.MaterialName1));
                        return false;
                    }

                    if (OutwardFacilityCharge.Partslist != InwardFacilityCharge.Partslist)
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00120",
                                    OutwardFacilityCharge.Partslist.PartslistNo, OutwardFacilityCharge.Partslist.PartslistName,
                                    InwardFacilityCharge.Partslist.PartslistNo, InwardFacilityCharge.Partslist.PartslistName));
                        return false;
                    }


                    // Falls Chargen unterschiedlich sind
                    // dann darf keine Umbuchung erfolgen
                    if ((OutwardFacilityCharge.FacilityLot != null) && (InwardFacilityCharge.FacilityLot != null))
                    {
                        if (OutwardFacilityCharge.FacilityLot != InwardFacilityCharge.FacilityLot)
                        {
                            AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00107"));
                            return false;
                        }
                    }
                    // Falls die Quellcharge anonym ist (FacilityLot = null) die andere nicht dann darf nicht umgebucht werden!
                    else if ((OutwardFacilityCharge.FacilityLot == null) && (InwardFacilityCharge.FacilityLot != null))
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00109"));
                        return false;
                    }
                    // ELSE: Anonyme auf Anonyme Chargen können gebucht werden und echte Chargen auf anonyme ist ebenfalls erlaubt,
                    // da die anonyme Charge zur echten Charge wird

                    if (OutwardFacilityCharge.NotAvailable)
                    {
                        if (!IsAutoResetNotAvailable)
                        {
                            AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00083"));
                            return false;
                        }
                        else //if ((NotAvailableMode == Global.BookingNotAvailableMode.AutoReset) || (NotAvailableMode == Global.BookingNotAvailableMode.AutoSetAndReset))
                        {
                            // Automatisches Rücksetzen von NotAvailable
                        }
                    }
                    else if (InwardFacilityCharge.NotAvailable)
                    {
                        if (!IsAutoResetNotAvailable)
                        {
                            AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00082"));
                            return false;
                        }
                        else //if ((NotAvailableMode == Global.BookingNotAvailableMode.AutoReset) || (NotAvailableMode == Global.BookingNotAvailableMode.AutoSetAndReset))
                        {
                            // Automatisches Rücksetzen von NotAvailable
                        }
                    }

                    // Falls Umlagerung innerhalb des gleichen Lagerplatzes
                    if (OutwardFacilityCharge.Facility == InwardFacilityCharge.Facility)
                    {
                        // Split muss unterschiedlich sein, 
                        // darf aber nicht vorkommen, weil sie die selbe GUID dann haben müssen
                        if (OutwardFacilityCharge.SplitNo == InwardFacilityCharge.SplitNo)
                        {
                            AddBookingMessage(eResultCodes.DependingParamsHasWrongValue, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00012", "OutwardFacilityCharge", "InwardFacilityCharge"));
                            return false;
                        }
                    }
                    else
                    {
                        // Split kann unterschiedlich sein
                    }
                }
            }
            // Buchung auf OutwardFacilityCharge
            else if (OutwardFacilityCharge != null)
            {
                if (OutwardFacilityCharge.NotAvailable)
                {
                    if (!IsAutoResetNotAvailable)
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00083"));
                        return false;
                    }
                    else //if ((NotAvailableMode == Global.BookingNotAvailableMode.AutoReset) || (NotAvailableMode == Global.BookingNotAvailableMode.AutoSetAndReset))
                    {
                        // Automatisches Rücksetzen von NotAvailable
                    }
                }

                // Umlagerungsbuchung von OutwardFacilityCharge nach Facility oder FacilityLocation
                if ((InwardFacilityLot != null) || (InwardMaterial != null))
                {
                    AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00079"));
                    return false;
                }
                // Umlagerungbuchung auf ein Silo/Tank...
                else if (InwardFacility != null)
                {
                    // Kommentar für Buchungsprogrammierung:
                    // Für Umlagerungen kann OutwardFacility oder OutwardFacilityLocation anstatt OutwardFacilityCharge gesetzt sein -> Neuanlage auf Ziellagerort
                    // Was passiert mit anonymen Chargen?
                    if (InwardFacilityLocation != null)
                    {
                        AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00110"));
                        return false;
                    }
                    if (QuantityIsAbsolute == true)
                    {
                        AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00020"));
                        return false;
                    }
                    // Umlagerungsbuchungen auf den selben Lagerplatz ist nicht möglich
                    if (OutwardFacilityCharge.Facility == ParamsAdjusted.InwardFacility)
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00071",
                                            OutwardFacilityCharge.Facility.FacilityNo, OutwardFacilityCharge.Facility.FacilityName,
                                            ParamsAdjusted.InwardFacility.FacilityNo, ParamsAdjusted.InwardFacility.FacilityName));
                        AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00021"));
                        return false;
                    }

                    // Check Belegung
                    if ((ParamsAdjusted.InwardFacility.MDFacilityType != null)
                        && (ParamsAdjusted.InwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer))
                    {
                        // Überprüfe Materialunterschied zwischen FacilityCharge und Facility
                        if ((OutwardFacilityCharge.Material != null) && (ParamsAdjusted.InwardFacility.Material != null)
                            && !Material.IsMaterialEqual(OutwardFacilityCharge.Material, ParamsAdjusted.InwardFacility.Material))
                        {
                            AddBookingMessage(eResultCodes.ProhibitedBooking,
                                    Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00073",
                                                OutwardFacilityCharge.Facility.FacilityNo, OutwardFacilityCharge.Facility.FacilityName, OutwardFacilityCharge.Material.MaterialNo, OutwardFacilityCharge.Material.MaterialName1,
                                                ParamsAdjusted.InwardFacility.FacilityNo, ParamsAdjusted.InwardFacility.FacilityName, ParamsAdjusted.InwardFacility.Material.MaterialNo, ParamsAdjusted.InwardFacility.Material.MaterialName1));
                            AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00077"));
                            return false;
                        }

                        // Überprüfe Rezeptunterschied zwischen FacilityCharge und Facility
                        if ((OutwardFacilityCharge.Partslist != null) && (ParamsAdjusted.InwardFacility.Partslist != null)
                            && (OutwardFacilityCharge.Partslist != ParamsAdjusted.InwardFacility.Partslist))
                        {
                            AddBookingMessage(eResultCodes.ProhibitedBooking,
                                    Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00076",
                                                OutwardFacilityCharge.Facility.FacilityNo, OutwardFacilityCharge.Facility.FacilityName, OutwardFacilityCharge.Partslist.PartslistNo, OutwardFacilityCharge.Partslist.PartslistName,
                                                ParamsAdjusted.InwardFacility.FacilityNo, ParamsAdjusted.InwardFacility.FacilityName, ParamsAdjusted.InwardFacility.Partslist.PartslistNo, ParamsAdjusted.InwardFacility.Partslist.PartslistName));
                            AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00078"));
                            return false;
                        }
                    }
                    // Check ob Einlagerung überhaupt erlaubt ist
                    if (!ParamsAdjusted.InwardFacility.InwardEnabled && !ParamsAdjusted.IgnoreIsEnabled)
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00074",
                                            OutwardFacilityCharge.Facility.FacilityNo, OutwardFacilityCharge.Facility.FacilityName, OutwardFacilityCharge.Material.MaterialNo, OutwardFacilityCharge.Material.MaterialName1,
                                            ParamsAdjusted.InwardFacility.FacilityNo, ParamsAdjusted.InwardFacility.FacilityName));
                        return false;
                    }
                }
                // Umlagerungsbuchung auf ein Lagerort, ist erlaubt
                else if (InwardFacilityLocation != null)
                {
                    if (QuantityIsAbsolute == true)
                    {
                        AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00020"));
                        return false;
                    }
                    // Standard-einlagerplatz muss gesetzt sein!
                    if (InwardFacilityLocation.Facility1_IncomingFacility == null)
                    {
                        AddBookingMessage(eResultCodes.EntityPropertyNotSet,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00040",
                                            InwardFacilityLocation.FacilityNo, InwardFacilityLocation.FacilityName));
                        return false;
                    }
                    // Lagerplatz-Typ muss gesetzt sein
                    if (InwardFacilityLocation.Facility1_IncomingFacility.MDFacilityType == null)
                    {
                        AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00088",
                                InwardFacilityLocation.Facility1_IncomingFacility.FacilityNo, InwardFacilityLocation.Facility1_IncomingFacility.FacilityName,
                                InwardFacilityLocation.FacilityNo, InwardFacilityLocation.FacilityName));
                    }

                    // Umlagerungsbuchungen auf den selben Lagerplatz ist nicht möglich
                    if (OutwardFacilityCharge.Facility == InwardFacilityLocation.Facility1_IncomingFacility)
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00072",
                                            OutwardFacilityCharge.Facility.FacilityNo, OutwardFacilityCharge.Facility.FacilityName,
                                            InwardFacilityLocation.FacilityNo, InwardFacilityLocation.FacilityName,
                                            InwardFacilityLocation.Facility1_IncomingFacility.FacilityNo, InwardFacilityLocation.Facility1_IncomingFacility.FacilityName));
                        AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00021"));
                        return false;
                    }

                    // Check Belegung
                    if ((InwardFacilityLocation.Facility1_IncomingFacility.MDFacilityType != null)
                        && (InwardFacilityLocation.Facility1_IncomingFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer))
                    {
                        // Überprüfe Materialunterschied zwischen FacilityCharge und Standard-Einlagerpatz von Lagerort
                        if ((OutwardFacilityCharge.Material != null) && (InwardFacilityLocation.Facility1_IncomingFacility.Material != null)
                            && !Material.IsMaterialEqual(OutwardFacilityCharge.Material, InwardFacilityLocation.Facility1_IncomingFacility.Material))
                        {
                            AddBookingMessage(eResultCodes.ProhibitedBooking,
                                    Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00073",
                                                OutwardFacilityCharge.Facility.FacilityNo, OutwardFacilityCharge.Facility.FacilityName, OutwardFacilityCharge.Material.MaterialNo, OutwardFacilityCharge.Material.MaterialName1,
                                                InwardFacilityLocation.Facility1_IncomingFacility.FacilityNo, InwardFacilityLocation.Facility1_IncomingFacility.FacilityName, InwardFacilityLocation.Facility1_IncomingFacility.Material.MaterialNo, InwardFacilityLocation.Facility1_IncomingFacility.Material.MaterialName1));
                            AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00077"));
                            return false;
                        }

                        // Überprüfe Partslistunterschied zwischen FacilityCharge und Standard-Einlagerpatz von Lagerort
                        if ((OutwardFacilityCharge.Partslist != null) && (InwardFacilityLocation.Facility1_IncomingFacility.Partslist != null)
                            && (OutwardFacilityCharge.Partslist != InwardFacilityLocation.Facility1_IncomingFacility.Partslist))
                        {
                            AddBookingMessage(eResultCodes.ProhibitedBooking,
                                    Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00076",
                                                OutwardFacilityCharge.Facility.FacilityNo, OutwardFacilityCharge.Facility.FacilityName, OutwardFacilityCharge.Partslist.PartslistNo, OutwardFacilityCharge.Partslist.PartslistName,
                                                InwardFacilityLocation.Facility1_IncomingFacility.FacilityNo, InwardFacilityLocation.Facility1_IncomingFacility.FacilityName, InwardFacilityLocation.Facility1_IncomingFacility.Partslist.PartslistNo, InwardFacilityLocation.Facility1_IncomingFacility.Partslist.PartslistName));
                            AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00078"));
                            return false;
                        }
                    }
                    // Check ob Einlagerung überhaupt erlaubt ist
                    if (!InwardFacilityLocation.Facility1_IncomingFacility.InwardEnabled && !ParamsAdjusted.IgnoreIsEnabled)
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00074",
                                            OutwardFacilityCharge.Facility.FacilityNo, OutwardFacilityCharge.Facility.FacilityName, OutwardFacilityCharge.Material.MaterialNo, OutwardFacilityCharge.Material.MaterialName1,
                                            InwardFacilityLocation.Facility1_IncomingFacility.FacilityNo, InwardFacilityLocation.Facility1_IncomingFacility.FacilityName));
                        return false;
                    }
                }
                // Sonst normale Auslagerungsbuchung
                else
                {
                    //(ZeroStock == NotAvailableState.Reset)
                }
            }
            // Sonst Buchung auf InwardFacilityCharge
            else // (InwardFacilityCharge != null)
            {
                if (InwardFacilityCharge.NotAvailable)
                {
                    if (!IsAutoResetNotAvailable
                        && (MDZeroStockState.ZeroStockState != MDZeroStockState.ZeroStockStates.ResetIfNotAvailable))
                    {
                        AddBookingMessage(eResultCodes.ProhibitedBooking, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00082"));
                        return false;
                    }
                    else // if ((NotAvailableMode == Global.BookingNotAvailableMode.AutoReset) || (NotAvailableMode == Global.BookingNotAvailableMode.AutoSetAndReset))
                    {
                        // Automatisches Rücksetzen von NotAvailable
                    }
                }

                // Umlagerungsbuchung von einem Lagerplatz oder Lagerort auf eine FacilityCharge ist nicht erlaubt
                if ((OutwardFacility != null) || (OutwardFacilityLocation != null) || (OutwardFacilityLot != null) || (OutwardMaterial != null))
                {
                    AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00117"));
                    return false;
                }
                // Sonst normale Einlagerungsbuchung
                else
                {
                }
            }
            return true;
        }

        /// <summary>
        /// 2. [FacilityLocation]
        /// </summary>
        protected virtual bool CAABookingOn_FacilityLocation()
        {
            ///     Pseudo-code:
            ///     Facility, FacilityCharge darf nicht übergeben werden
            ///         Breche ab mit Fehler
            ///     Material oder FacilityLot muss übergeben werden zur weiteren Eingrenzung
            ///         Breche ab mit Fehler
            if ((OutwardFacilityLocation == null) && (InwardFacilityLocation == null))
                return false;
            if (!CheckAndAdjustFacilityLocation())
                return false;

            if (MDZeroStockState.ZeroStockState > MDZeroStockState.ZeroStockStates.Off) // Reset wurde bereits vorher abgefangen
            {
                AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00084"));
                return false;
            }

            ///     Facility, FacilityCharge darf nicht übergeben werden
            ///         Breche ab mit Fehler
            if (OutwardFacilityLocation != null)
            {
                if ((OutwardFacility != null) || (OutwardFacilityCharge != null))
                {
                    AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00115"));
                    return false;
                }
            }
            if (InwardFacilityLocation != null)
            {
                if ((InwardFacility != null) || (InwardFacilityCharge != null))
                {
                    AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00115"));
                    return false;
                }
            }

            ///     Material oder FacilityLot muss übergeben werden zur weiteren Eingrenzung
            ///         Breche ab mit Fehler
            // Buchung auf Lagerorten oder Umbuchung zwischen Lagerorten ohne Materialinformation ist nicht erlaubt
            if ((OutwardMaterial == null) && (InwardMaterial == null) && (OutwardFacilityLot == null) && (InwardFacilityLot == null))
            {
                AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00034"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 2.1 [FacilityLocation],[Material]:		        
        /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit Material vorhanden sind
        /// </summary>
        protected virtual bool CAABookingOn_FacilityLocation_Material()
        {
            ///     Pseudo-code:
            ///     Wenn keine FacilityChargen mit Material auf Lagerort vorhanden
            ///             Hole Standard-Einlagerplatz aus FacilityLocation(Lagerort)
            ///                 Falls Standard-Einlagerplatz nicht existiert
            ///                     breche ab mit Fehler
            ///             Setze AdjustedFacility mit Standard-Einlagerplatz
            if ((OutwardFacilityLocation == null) && (InwardFacilityLocation == null))
                return false;
            if (!CheckAndAdjustFacilityLocation())
                return false;
            if ((OutwardMaterial == null) && (InwardMaterial == null))
                return false;
            if (OutwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit Material auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Auslagerplatz angelegt werden
                /// Adjusted.OutwardMaterial ist garantiert gesetzt aufgrund des Aufrufs von CheckAndAdjustMaterial() zuvor
                Guid? guidNull = null;
                if (!FacilityManager.s_cQry_FCList_FacLoc_ProdMat_Pl_NotAvailable(DatabaseApp,
                    OutwardFacilityLocation.FacilityID,
                    ParamsAdjusted.OutwardMaterial.MaterialID,
                    null,
                    ParamsAdjusted.OutwardPartslist != null ? ParamsAdjusted.OutwardPartslist.PartslistID : guidNull,
                    false).Any())
                {
                    /// Falls Standard-Auslagerplatz nicht existiert, breche ab
                    if (OutwardFacilityLocation.Facility1_OutgoingFacility == null)
                    {
                        AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00102",
                                            OutwardFacilityLocation.FacilityNo, OutwardFacilityLocation.FacilityName));
                        return false;
                    }
                    else
                    {
                        ParamsAdjusted.OutwardFacility = OutwardFacilityLocation.Facility1_OutgoingFacility;
                    }
                }
            }
            if (InwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit Material auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Einlagerplatz angelegt werden
                /// Adjusted.InwardMaterial ist garantiert gesetzt aufgrund des Aufrufs von CheckAndAdjustMaterial() zuvor
                Guid? guidNull = null;
                if (!FacilityManager.s_cQry_FCList_FacLoc_ProdMat_Pl_NotAvailable(DatabaseApp,
                    InwardFacilityLocation.FacilityID,
                    ParamsAdjusted.InwardMaterial.MaterialID,
                    null,
                    ParamsAdjusted.InwardPartslist != null ? ParamsAdjusted.InwardPartslist.PartslistID : guidNull,
                    false).Any())
                {
                    /// Falls Standard-Einlagerplatz nicht existiert, breche ab
                    if (InwardFacilityLocation.Facility1_IncomingFacility == null)
                    {
                        AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00086",
                                            InwardFacilityLocation.FacilityNo, InwardFacilityLocation.FacilityName));
                        return false;
                    }
                    else
                    {
                        ParamsAdjusted.InwardFacility = InwardFacilityLocation.Facility1_IncomingFacility;
                    }
                }
            }
            /// Einlagerungsbuchung: ((OutwardFacilityLocation == null) && (InwardFacilityLocation != null))
            /// Auslagerungsbuchung: ((OutwardFacilityLocation != null) && (InwardFacilityLocation == null))
            /// Umlagerungsbuchung: ((OutwardFacilityLocation != null) && (InwardFacilityLocation != null))
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!

            return true;
        }

        /// <summary>
        /// 2.2 [FacilityLocation],[FacilityLot]:			
        /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit FacilityLot vorhanden sind
        /// </summary>
        protected virtual bool CAABookingOn_FacilityLocation_FacilityLot()
        {
            ///     Pseudo-code:
            ///     Wenn keine FacilityChargen mit FacilityLot auf Lagerort vorhanden
            ///             Hole Standard-Einlagerplatz aus FacilityLocation(Lagerort)
            ///                 Falls Standard-Einlagerplatz nicht existiert
            ///                     breche ab mit Fehler
            ///             Setze AdjustedFacility mit Standard-Einlagerplatz
            if ((OutwardFacilityLocation == null) && (InwardFacilityLocation == null))
                return false;
            if (!CheckAndAdjustFacilityLocation())
                return false;
            if ((OutwardFacilityLot == null) && (InwardFacilityLot == null))
                return false;
            if (!CheckAndAdjustFacilityLot())
                return false;

            if (OutwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit FacilityLot auf Lagerort vorhanden
                /// Dann muss eine FacilityCharge auf dem Standard-Auslagerplatz angelegt werden
                Guid? guidNull = null;
                if (!FacilityManager.s_cQry_FCList_FacLoc_Lot_Pl_NotAvailable(DatabaseApp,
                    OutwardFacilityLocation.FacilityID,
                    ParamsAdjusted.OutwardFacilityLot.FacilityLotID,
                    ParamsAdjusted.OutwardPartslist != null ? ParamsAdjusted.OutwardPartslist.PartslistID : guidNull,
                    false).Any())
                {
                    /// Falls Standard-Auslagerplatz nicht existiert, breche ab
                    if (OutwardFacilityLocation.Facility1_OutgoingFacility == null)
                    {
                        AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00102",
                                            OutwardFacilityLocation.FacilityNo, OutwardFacilityLocation.FacilityName));
                        return false;
                    }
                    else
                    {
                        ParamsAdjusted.OutwardFacility = OutwardFacilityLocation.Facility1_OutgoingFacility;
                    }
                }
            }
            if (InwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit FacilityLot auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Einlagerplatz angelegt werden
                Guid? guidNull = null;
                if (!FacilityManager.s_cQry_FCList_FacLoc_Lot_Pl_NotAvailable(DatabaseApp,
                    InwardFacilityLocation.FacilityID,
                    ParamsAdjusted.InwardFacilityLot.FacilityLotID,
                    ParamsAdjusted.InwardPartslist != null ? ParamsAdjusted.InwardPartslist.PartslistID : guidNull,
                    false).Any())
                {
                    /// Falls Standard-Einlagerplatz nicht existiert, breche ab
                    if (InwardFacilityLocation.Facility1_IncomingFacility == null)
                    {
                        AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00086",
                                            InwardFacilityLocation.FacilityNo, InwardFacilityLocation.FacilityName));
                        return false;
                    }
                    else
                    {
                        ParamsAdjusted.InwardFacility = InwardFacilityLocation.Facility1_IncomingFacility;
                    }
                }
            }
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!
            return true;
        }

        /// <summary>
        /// 2.3 [FacilityLocation],[FacilityLot],[Material]:
        /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit FacilityLot und Material vorhanden
        /// </summary>
        protected virtual bool CAABookingOn_FacilityLocation_FacilityLot_Material()
        {
            ///     Pseudo-code:
            ///     Wenn keine FacilityChargen mit FacilityLot und Material und FacilityLocation auf Lagerort vorhanden
            ///             Hole Standard-Einlagerplatz aus FacilityLocation(Lagerort)
            ///                 Falls Standard-Einlagerplatz nicht existiert
            ///                     breche ab mit Fehler
            ///             Setze AdjustedFacility mit Standard-Einlagerplatz
            if ((OutwardFacilityLocation == null) || (InwardFacilityLocation == null))
                return false;
            if (!CheckAndAdjustFacilityLocation())
                return false;
            if ((OutwardMaterial == null) && (InwardMaterial == null))
                return false;
            if ((OutwardFacilityLot == null) && (InwardFacilityLot == null))
                return false;
            if (!CheckAndAdjustFacilityLot())
                return false;

            if (OutwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit FacilityLot und Material auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Auslagerplatz angelegt werden
                /// Adjusted.OutwardMaterial ist garantiert gesetzt aufgrund des Aufrufs von CheckAndAdjustMaterial() zuvor
                Guid? guidNull = null;
                if (!FacilityManager.s_cQry_FCList_FacLoc_Lot_ProdMat_Pl_NotAvailable(DatabaseApp,
                    OutwardFacilityLocation.FacilityID,
                    ParamsAdjusted.OutwardFacilityLot.FacilityLotID,
                    ParamsAdjusted.OutwardMaterial.MaterialID,
                    null,
                    ParamsAdjusted.OutwardPartslist != null ? ParamsAdjusted.OutwardPartslist.PartslistID : guidNull,
                    false).Any())
                {
                    /// Falls Standard-Auslagerplatz nicht existiert, breche ab
                    if (OutwardFacilityLocation.Facility1_OutgoingFacility == null)
                    {
                        AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00102",
                                            OutwardFacilityLocation.FacilityNo, OutwardFacilityLocation.FacilityName));
                        return false;
                    }
                    else
                    {
                        ParamsAdjusted.OutwardFacility = OutwardFacilityLocation.Facility1_OutgoingFacility;
                    }
                }
            }
            if (InwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit FacilityLot und Material auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Einlagerplatz angelegt werden
                /// Adjusted.InwardMaterial ist garantiert gesetzt aufgrund des Aufrufs von CheckAndAdjustMaterial() zuvor
                Guid? guidNull = null;
                if (!FacilityManager.s_cQry_FCList_FacLoc_Lot_ProdMat_Pl_NotAvailable(DatabaseApp,
                    InwardFacilityLocation.FacilityID,
                    ParamsAdjusted.InwardFacilityLot.FacilityLotID,
                    ParamsAdjusted.InwardMaterial.MaterialID,
                    null,
                    ParamsAdjusted.InwardPartslist != null ? ParamsAdjusted.InwardPartslist.PartslistID : guidNull,
                    false).Any())
                {
                    /// Falls Standard-Einlagerplatz nicht existiert, breche ab
                    if (InwardFacilityLocation.Facility1_IncomingFacility == null)
                    {
                        AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00086",
                                            InwardFacilityLocation.FacilityNo, InwardFacilityLocation.FacilityName));
                        return false;
                    }
                    else
                    {
                        ParamsAdjusted.InwardFacility = InwardFacilityLocation.Facility1_IncomingFacility;
                    }
                }
            }
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!
            return true;
        }

        /// <summary>
        /// Fall 3. [Facility]:   
        /// Nur bei Silos möglich zur Silobestandsführung
        /// </summary>
        protected virtual bool CAABookingOn_Facility()
        {
            ///     Pseudo-code:
            ///     FacilityCharge, FacilityLocation darf nicht übergeben werden
            ///         Breche ab mit Fehler
            ///     Setze AdjustedFacility mit Facility
            /// 
            ///     Wenn nicht [Material] und nicht [FacilityLot] übergeben
            ///         Falls keine FacilityCharge vorhanden
            ///             Wenn keine Silo-Belegung, 
            ///                 dann breche ab mit Fehler
            ///             Sonst
            ///                 dann Neuanlage von Anonymer FacilityCharge 
            ///         Sonst
            ///             Wenn keine Silo-Belegung
            ///                 Dann setze Belegung mit erster FacilityCharge.Material bzw. bereits gesetztes [AdjustedMaterial] wenn Erlaubt
            ///             Stackbuchung auf FacilityChargen mit Facility.Material (bzw. Material aus Fall 3.1)
            ///     Sonst Fall 3.1 oder Fall 3.2
            if ((OutwardFacility == null) && (InwardFacility == null))
                return false;

            if (OutwardFacility != null)
            {
                if ((OutwardFacilityLocation != null) || (OutwardFacilityCharge != null))
                {
                    AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00113"));
                    return false;
                }
                if (MDZeroStockState.ZeroStockState > MDZeroStockState.ZeroStockStates.Off) // Reset wurde bereits vorher abgefangen
                {
                    if ((OutwardFacility.MDFacilityType != null) && (OutwardFacility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBinContainer))
                    {
                        AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00017"));
                        return false;
                    }
                }
                if (InwardFacility == null)
                {
                    if (IsLotManaged 
                        && OutwardFacilityChargeList == null
                        && OutwardFacilityLot == null
                        && (!OutwardFacility.FacilityCharge_Facility.Where(c => c.NotAvailable == false).Any()
                            || OutwardFacility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBinContainer)
                        )
                    {
                        AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error50049"));
                        return false;
                    }
                }
            }
            if (InwardFacility != null)
            {
                if ((InwardFacilityLocation != null) || (InwardFacilityCharge != null))
                {
                    AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00113"));
                    return false;
                }
                if (MDZeroStockState.ZeroStockState > MDZeroStockState.ZeroStockStates.Off) // Reset wurde bereits vorher abgefangen
                {
                    if ((InwardFacility.MDFacilityType != null) && (InwardFacility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBinContainer))
                    {
                        AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00017"));
                        return false;
                    }
                }
                if (OutwardFacility == null && MDZeroStockState.ZeroStockState == datamodel.MDZeroStockState.ZeroStockStates.Off)
                {
                    if (IsLotManaged && InwardFacilityLot == null
                        && (!InwardFacility.FacilityCharge_Facility.Where(c => c.NotAvailable == false).Any()
                            || InwardFacility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBinContainer)
                        )
                    {
                        AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error50049"));
                        return false;
                    }
                }
            }

            // Falls Umlagerung, dann müssen Lagerplätze vom selben Typ sein!
            if ((OutwardFacility != null) && (InwardFacility != null))
            {
                if (InwardFacility.MDFacilityType.MDFacilityTypeIndex != OutwardFacility.MDFacilityType.MDFacilityTypeIndex
                    && InwardFacility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBin)
                {
                    AddBookingMessage(eResultCodes.ProhibitedBooking,
                            Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00019",
                                        InwardFacility.FacilityNo, InwardFacility.FacilityName,
                                        OutwardFacility.FacilityNo, OutwardFacility.FacilityName));
                    return false;
                }
            }

            if (OutwardFacility != null)
            {
                if ((OutwardMaterial == null) && (OutwardFacilityLot == null))
                {
                    if ((OutwardFacility.MDFacilityType != null) && (OutwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer))
                    {
                        if (OutwardFacility.Material == null)
                        {
                            // Setze Silobelegung im Buchungsprogramm mit Adjusted.OutwardMaterial, das gesetzt sein muss
                        }
                    }
                }
            }

            if (InwardFacility != null)
            {
                if ((InwardMaterial == null) && (InwardFacilityLot == null))
                {
                    if ((InwardFacility.MDFacilityType != null) && (InwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer))
                    {
                        if (InwardFacility.Material == null)
                        {
                            // Setze Silobelegung im Buchungsprogramm mit Adjusted.InwardMaterial, das gesetzt sein muss
                        }
                    }
                }
            }

            /// TODO: NotAvailableState.Set ist erlaubt wenn Silo. Beachte bei Buchungen !!!
            return true;
        }

        /// <summary>
        /// Fall 3.1. [Facility],[Material]:   
        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        /// Und zum anlegen von Anonymen Chargen, weil zu dem Buchungszeitpunkt noch keine Cahrgeninformation da war
        /// </summary>
        protected virtual bool CAABookingOn_Facility_Material()
        {
            ///     Pseudo-code:
            /// Wenn Silo
            ///     Wenn keine Belegung vorgegeben
            ///         Falls FacilityChargen vorhanden sind
            ///             Autokorrektur Belegung
            ///     Wenn Belegung vorgegeben
            ///         dann muss Belegung mit Material stimmen (Check wurde bereits durch Aufruf von CheckAndAdjustFacility() gemacht!)
            ///     Wenn keine Belegung vorgegeben
            ///         dann Autobelegung mit Material wenn Erlaubt
            /// Wenn keine FacilityCharge mit Material vorhanden
            ///     dann Neuanlage von ANONYMER FacilityCharge 
            /// Sonst wenn FacilityChargen mit Material vorhanden
            ///     Stackbuchung wie Fall 3, jedoch mit Material
            if ((OutwardFacility == null) && (InwardFacility == null))
                return false;
            if ((OutwardMaterial == null) && (InwardMaterial == null))
                return false;
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!
            return true;
        }

        /// <summary>
        /// Fall 3.2. [Facility],[FacilityLot]:  
        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        /// Es werden KEINE Anonynmen Chargen angelegt!
        /// </summary>
        protected virtual bool CAABookingOn_Facility_FacilityLot()
        {
            ///     Pseudo-code:
            /// Wenn Silo
            ///     Wenn keine Belegung vorgegeben
            ///         Falls FacilityChargen vorhanden sind
            ///             Autokorrektur Belegung
            ///     Wenn Belegung vorgegeben
            ///         dann muss Belegung mit Material aus FacilityLot stimmen (Check wurde bereits durch Aufruf von CheckAndAdjustFacility() gemacht!)
            ///     Wenn keine Belegung vorgegeben
            ///         dann Autobelegung mit Material aus FacilityLot wenn Erlaubt (entspricht [AdjustedMaterial])
            ///     Setze AdjustedMaterial
            ///     Wenn keine FacilityCharge mit FacilityLot vorhanden
            ///         dann Neuanlage FacilityCharge aus FacilityLot jedoch mit Materialnummer aus Facility.Material
            ///     Sonst wenn FacilityChargen vorhanden (nicht unbedingt von FacilityLot)
            ///         Stackbuchung über FacilityChargen
            ///         Falls anonyme Chargen vorhanden dann Ersetzung durch FacilityLot
            /// Wenn Lagerplatz
            ///     Wenn FacilityChargen mit FacilityLot vorhanden sind
            ///         dann Stackbuchung über FacilityChargen von FacilityLot
            ///         Falls anonyme Chargen vorhanden dann Ersetzung durch FacilityLot
            ///     Sonst wenn FacilityChargen mit FacilityLot nicht angelegt ist
            ///         dann Neuanlage FacilityCharge aus FacilityLot jedoch mit Materialnummer aus FacilityLot (entspricht [AdjustedMaterial])
            if ((OutwardFacility == null) && (InwardFacility == null))
                return false;
            if ((OutwardFacilityLot == null) && (InwardFacilityLot == null))
                return false;
            if (!CheckAndAdjustFacilityLot())
                return false;

            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!
            return true;
        }

        /// <summary>
        /// Fall 3.3. [Facility],[Material],[FacilityLot]:
        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        /// Es werden KEINE Anonynmen Chargen angelegt, aber mit Materialien die dem übergbenen Material entsprechen!
        /// Falls anonyme Chargen vorhanden sind, dann wird der anonyme Status aufgehoben durch Ersetzung mit FacilityLot
        /// </summary>
        protected virtual bool CAABookingOn_Facility_FacilityLot_Material()
        {
            ///     Pseudo-code:
            /// Setze AdjustedMaterial mit Material
            /// Wenn Silo
            ///     Wenn keine Belegung vorgegeben
            ///         Falls FacilityChargen vorhanden sind
            ///             Autokorrektur Belegung
            ///     Wenn Belegung vorgegeben
            ///         dann muss Belegung mit übergebenen Material stimmen (Check wurde bereits durch Aufruf von CheckAndAdjustFacility() gemacht!)
            ///     Wenn keine Belegung vorgegeben
            ///         dann Autobelegung mit übergebenen Material wenn Erlaubt
            ///     Wenn keine FacilityCharge mit FacilityLot vorhanden
            ///         dann Neuanlage FacilityCharge aus FacilityLot jedoch mit Materialnummer aus übergebenen Material
            ///     Sonst wenn FacilityChargen vorhanden (nicht unbedingt von FacilityLot)
            ///         Stackbuchung über FacilityChargen
            ///         Falls anonyme Chargen vorhanden dann Ersetzung durch FacilityLot
            /// Wenn Lagerplatz
            ///     Wenn FacilityChargen mit FacilityLot und Material vorhanden sind
            ///         dann Stackbuchung über FacilityChargen von FacilityLot und Material
            ///         Falls anonyme Chargen vorhanden dann Ersetzung durch FacilityLot
            ///     Sonst wenn FacilityChargen mit FacilityLot und Material nicht angelegt ist
            ///         dann Neuanlage FacilityCharge aus FacilityLot und Material jedoch mit Materialnummer aus Material
            if ((OutwardFacility == null) && (InwardFacility == null))
                return false;
            if ((OutwardMaterial == null) && (InwardMaterial == null))
                return false;
            if ((OutwardFacilityLot == null) && (InwardFacilityLot == null))
                return false;
            if (!CheckAndAdjustFacilityLot())
                return false;

            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!
            return true;
        }

        /// <summary>
        /// 4. [FacilityLot]:                              
        /// Zur Stackbuchung wenn FacilityChargen von FacilityLot vorhanden
        /// </summary>
        protected virtual bool CAABookingOn_FacilityLot()
        {
            ///     Pseudo-code:
            ///     Falls keine FacilityChargen von FacilityLot vorhanden
            ///         Breche ab mit Fehler, weil anonyme Chargen nicht angelegt werden können weil keine Lagerinfo vorhanden ist
            if ((OutwardFacilityLot == null) && (InwardFacilityLot == null))
                return false;
            if (!CheckAndAdjustFacilityLot())
                return false;
            if ((OutwardFacilityLot != null) && (InwardFacilityLot != null))
            {
                AddBookingMessage(eResultCodes.WrongParameterCombinations,
                    Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00061"));
                return false;
            }

            if (OutwardFacilityLot != null)
            {
                Guid? guidNull = null;
                if (!FacilityManager.s_cQry_FCList_Lot_Pl_NotAvailable(DatabaseApp,
                    ParamsAdjusted.OutwardFacilityLot.FacilityLotID,
                    ParamsAdjusted.OutwardPartslist != null ? ParamsAdjusted.OutwardPartslist.PartslistID : guidNull,
                    false).Any())
                {
                    AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                        Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00015", "OutwardFacilityLot.LotNo"));
                    return false;
                }
            }
            if (InwardFacilityLot != null)
            {
                Guid? guidNull = null;
                if (!FacilityManager.s_cQry_FCList_Lot_Pl_NotAvailable(DatabaseApp,
                    ParamsAdjusted.InwardFacilityLot.FacilityLotID,
                    ParamsAdjusted.InwardPartslist != null ? ParamsAdjusted.InwardPartslist.PartslistID : guidNull,
                    false).Any())
                {
                    AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                        Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00015", InwardFacilityLot.LotNo));
                    return false;
                }
            }
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!
            return true;
        }

        /// <summary>
        /// 4.1.[FacilityLot],[Material]:			        
        /// Zur Stackbuchung wenn FacilityCharge mit Materialnummer vorhanden
        /// </summary>
        protected virtual bool CAABookingOn_FacilityLot_Material()
        {
            ///     Pseudo-code:
            ///     Falls keine FacilityChargen von FacilityLot und Material vorhanden
            ///         Breche ab mit Fehler, weil anonyme Chargen nicht angelegt werden können weil keine Lagerinfo vorhanden ist
            if ((OutwardMaterial == null) && (InwardMaterial == null))
                return false;
            if (OutwardFacilityLot != null)
            {
                Guid? guidNull = null;
                if (!FacilityManager.s_cQry_FCList_Lot_Mat_Pl_NotAvailable(DatabaseApp,
                    ParamsAdjusted.OutwardFacilityLot.FacilityLotID,
                    ParamsAdjusted.OutwardMaterial.MaterialID,
                    ParamsAdjusted.OutwardPartslist != null ? ParamsAdjusted.OutwardPartslist.PartslistID : guidNull,
                    false).Any())
                {
                    AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                            Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00016", OutwardFacilityLot.LotNo, ParamsAdjusted.OutwardMaterial.MaterialNo, ParamsAdjusted.OutwardMaterial.MaterialName1));
                    return false;
                }
            }

            if (InwardFacilityLot != null)
            {
                Guid? guidNull = null;
                if (!FacilityManager.s_cQry_FCList_Lot_Mat_Pl_NotAvailable(DatabaseApp,
                    ParamsAdjusted.InwardFacilityLot.FacilityLotID,
                    ParamsAdjusted.InwardMaterial.MaterialID,
                    ParamsAdjusted.InwardPartslist != null ? ParamsAdjusted.InwardPartslist.PartslistID : guidNull,
                    false).Any())
                {
                    AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                        Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00016", InwardFacilityLot.LotNo, ParamsAdjusted.InwardMaterial.MaterialNo, ParamsAdjusted.InwardMaterial.MaterialName1));
                    return false;
                }
            }
            return true;
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!
        }
        #endregion

        #region Checks for different Booking-Types on NOT Lot-Managed Materials

        protected virtual bool CAABookingOn_FacilityCharge_NotLotManaged()
        {
            return CAABookingOn_FacilityCharge();
        }

        /// <summary>
        /// 5. [FacilityLocation]
        /// </summary>
        protected virtual bool CAABookingOn_FacilityLocation_Material_NotLotManaged()
        {
            ///     Pseudo-code:
            ///     Facility, FacilityCharge darf nicht übergeben werden
            ///         Breche ab mit Fehler
            ///             Gibt es FacilityCharges (Enlagerungen) von diesem Material in diesem Lagerort
            ///                 Dann Stackbuchung
            ///             Sonst
            ///                 Hole Standard-Einlagerplatz aus FacilityLocation(Lagerort)
            ///                     Falls Standard-Einlagerplatz nicht existiert
            ///                     breche ab mit Fehler
            ///                 Setze AdjustedFacility mit Standard-Einlagerplatz
            if ((OutwardFacilityLocation == null) && (InwardFacilityLocation == null))
                return false;

            if ((OutwardMaterial == null) && (InwardMaterial == null))
                return false;

            if (!CheckAndAdjustFacilityLocation())
                return false;

            if ((OutwardFacility != null) || (InwardFacility != null))
            {
                AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00114"));
                return false;
            }

            if (OutwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit Material auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Auslagerplatz angelegt werden
                /// Adjusted.OutwardMaterial ist garantiert gesetzt aufgrund des Aufrufs von CheckAndAdjustMaterial() zuvor
                Guid? guidNull = null;
                if (!FacilityManager.s_cQry_FCList_FacLoc_ProdMat_Pl_NotAvailable(DatabaseApp,
                    OutwardFacilityLocation.FacilityID,
                    ParamsAdjusted.OutwardMaterial.MaterialID,
                    null,
                    ParamsAdjusted.OutwardPartslist != null ? ParamsAdjusted.OutwardPartslist.PartslistID : guidNull,
                    false).Any())
                {
                    /// Falls Standard-Auslagerplatz nicht existiert, breche ab
                    if (OutwardFacilityLocation.Facility1_OutgoingFacility == null)
                    {
                        AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                                Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00102",
                                            OutwardFacilityLocation.FacilityNo, OutwardFacilityLocation.FacilityName));
                        return false;
                    }
                    else
                    {
                        ParamsAdjusted.OutwardFacility = OutwardFacilityLocation.Facility1_OutgoingFacility;
                    }
                }
            }
            if (InwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit Material auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Einlagerplatz angelegt werden
                /// Adjusted.InwardMaterial ist garantiert gesetzt aufgrund des Aufrufs von CheckAndAdjustMaterial() zuvor
                Guid? guidNull = null;
                if (!FacilityManager.s_cQry_FCList_FacLoc_ProdMat_Pl_NotAvailable(DatabaseApp,
                    InwardFacilityLocation.FacilityID,
                    ParamsAdjusted.InwardMaterial.MaterialID,
                    null,
                    ParamsAdjusted.InwardPartslist != null ? ParamsAdjusted.InwardPartslist.PartslistID : guidNull,
                    false).Any())
                {
                    /// Falls Standard-Einlagerplatz nicht existiert, breche ab
                    if (InwardFacilityLocation.Facility1_IncomingFacility == null)
                    {
                        AddBookingMessage(eResultCodes.WrongConfigurationInMaterialManagement,
                                gip.core.datamodel.Database.Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00086",
                                            InwardFacilityLocation.FacilityNo, InwardFacilityLocation.FacilityName));
                        return false;
                    }
                    else
                    {
                        ParamsAdjusted.InwardFacility = InwardFacilityLocation.Facility1_IncomingFacility;
                    }
                }
            }
            /// Einlagerungsbuchung: ((OutwardFacilityLocation == null) && (InwardFacilityLocation != null))
            /// Auslagerungsbuchung: ((OutwardFacilityLocation != null) && (InwardFacilityLocation == null))
            /// Umlagerungsbuchung: ((OutwardFacilityLocation != null) && (InwardFacilityLocation != null))
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!

            return true;
        }

        /// <summary>
        /// Fall 6. [Facility]:   
        /// Nur bei Silos möglich zur Silobestandsführung
        /// </summary>
        protected virtual bool CAABookingOn_Facility_NotLotManaged()
        {
            ///     Pseudo-code:
            if ((OutwardFacility == null) && (InwardFacility == null))
                return false;

            if ((OutwardFacilityLocation != null) || (InwardFacilityLocation != null))
            {
                AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00112"));
                return false;
            }

            if (OutwardFacility != null)
            {
                if (MDZeroStockState.ZeroStockState > MDZeroStockState.ZeroStockStates.Off) // Reset wurde bereits vorher abgefangen
                {
                    if ((OutwardFacility.MDFacilityType != null) && (OutwardFacility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBinContainer))
                    {
                        AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00017"));
                        return false;
                    }
                }
                if (OutwardMaterial == null)
                {
                    if ((OutwardFacility.MDFacilityType != null) && (OutwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer))
                    {
                        if (OutwardFacility.Material == null)
                        {
                            // Setze Silobelegung im Buchungsprogramm mit Adjusted.OutwardMaterial, das gesetzt sein muss
                        }
                    }
                }
            }

            if (InwardFacility != null)
            {
                if (MDZeroStockState.ZeroStockState > MDZeroStockState.ZeroStockStates.Off) // Reset wurde bereits vorher abgefangen
                {
                    if ((InwardFacility.MDFacilityType != null) && (InwardFacility.MDFacilityType.FacilityType != FacilityTypesEnum.StorageBinContainer))
                    {
                        AddBookingMessage(eResultCodes.WrongParameterCombinations, Root.Environment.TranslateMessage(CurrentFacilityManager, "Error00017"));
                        return false;
                    }
                }
                if (InwardMaterial == null)
                {
                    if ((InwardFacility.MDFacilityType != null) && (InwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer))
                    {
                        if (InwardFacility.Material == null)
                        {
                            // Setze Silobelegung im Buchungsprogramm mit Adjusted.InwardMaterial, das gesetzt sein muss
                        }
                    }
                }
            }

            /// TODO: NotAvailableState.Set ist erlaubt wenn Silo. Beachte bei Buchungen !!!
            return true;
        }

        /// <summary>
        /// Fall 6.1. [Facility],[Material]:   
        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        /// Und zum anlegen von Anonymen Chargen, weil zu dem Buchungszeitpunkt noch keine Chargeninformation da war
        /// </summary>
        protected virtual bool CAABookingOn_Facility_Material_NotLotManaged()
        {
            ///     Pseudo-code:
            if ((OutwardFacility == null) && (InwardFacility == null))
                return false;
            if ((OutwardMaterial == null) && (InwardMaterial == null))
                return false;
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!
            return true;
        }
        #endregion

        #endregion

        #region Hilfsmethoden
        public bool AreQuantityParamsNeeded
        {
            get
            {
                var quantityParamsNeeded = this.ParameterValueList["QuantityParamsNeeded"];
                if (!(quantityParamsNeeded is Boolean))
                    return false;
                return (Boolean)quantityParamsNeeded;
            }
        }

        public bool AreFacilityEntitiesNeeded
        {
            get
            {
                if (AreInwardFacilityEntitiesNeeded)
                    return true;
                if (AreOutwardFacilityEntitiesNeeded)
                    return true;
                return false;
            }
        }


        public bool AreInwardFacilityEntitiesNeeded
        {
            get
            {
                var inwardFacilityEntitiesNeeded = this.ParameterValueList["InwardFacilityEntitiesNeeded"];
                if (!(inwardFacilityEntitiesNeeded is Boolean))
                    return false;
                return (Boolean)inwardFacilityEntitiesNeeded;
            }
        }

        public bool AreOutwardFacilityEntitiesNeeded
        {
            get
            {
                var outwardFacilityEntitiesNeeded = this.ParameterValueList["OutwardFacilityEntitiesNeeded"];
                if (!(outwardFacilityEntitiesNeeded is Boolean))
                    return false;
                return (Boolean)outwardFacilityEntitiesNeeded;
            }
        }


        #endregion

        #region Msg
        public enum eResultCodes : short
        {
            Suceeded = 0,

            // Warning  (10000 - 19999)
            NoFacilityFoundForRetrotragePosting = 10000,
            NoFacilityChargeFoundForRetrotragePosting = 10001,

            // Failure  (20000 - 29999)

            // Error    (30000 - 39999)
            NoBookingType = 30000,
            NoBookingTypeInfo = 30001,
            RequiredParamsNotSet = 30002,
            DependingParamsNotSet = 30003,
            WrongParameterCombinations = 30004,
            EmptyParameterCouldNotBeDerived = 30005,
            DependingParamsHasWrongValue = 30006,
            ProhibitedBooking = 30007,
            WrongConfigurationInMaterialManagement = 30008,
            WrongStateOfEntity = 30009,
            QuantityConversionError = 30010,
            EntityPropertyNotSet = 30011,
            WrongImplementation = 30012,
            TransactionError = 31000,
        }

        public void AddBookingMessage(eResultCodes messageNo, string message, string xmlData = null)
        {
            ValidMessage.AddDetailMessage(new Msg { Source = "BookingResultMessage", MessageLevel = GetMessageLevel(messageNo), ACIdentifier = messageNo.ToString(), Message = message });
            SetMessageLevelOfHead();
        }

        public void Merge(MsgWithDetails msgToIntegrate)
        {
            if (msgToIntegrate == null)
                return;
            if (msgToIntegrate.MsgDetails == null)
                return;
            foreach (Msg msg in msgToIntegrate.MsgDetails)
            {
                ValidMessage.AddDetailMessage(msg);
            }
            SetMessageLevelOfHead();
        }

        private eMsgLevel GetMessageLevel(eResultCodes messageNo)
        {
            eMsgLevel messageLevel = eMsgLevel.Info;
            if ((int)messageNo >= 10000 && (int)messageNo <= 19999)
            {
                messageLevel = eMsgLevel.Warning;
            }
            else if ((int)messageNo >= 20000 && (int)messageNo <= 29999)
            {
                messageLevel = eMsgLevel.Failure;
            }
            else if ((int)messageNo >= 30000 && (int)messageNo <= 39999)
            {
                messageLevel = eMsgLevel.Error;
            }
            return messageLevel;
        }

        private void SetMessageLevelOfHead()
        {
            eMsgLevel levelLocal = eMsgLevel.Default;
            foreach (Msg msg in ValidMessage.MsgDetails)
            {
                if (msg.MessageLevel == eMsgLevel.Exception)
                {
                    levelLocal = msg.MessageLevel;
                    break;
                }
                else if (msg.MessageLevel == eMsgLevel.Error)
                {
                    if ((levelLocal == eMsgLevel.Failure)
                        || (levelLocal == eMsgLevel.Warning)
                        || (levelLocal == eMsgLevel.Info)
                        || (levelLocal == eMsgLevel.Debug)
                        || (levelLocal == eMsgLevel.Default))
                    {
                        levelLocal = msg.MessageLevel;
                    }
                }
                else if (msg.MessageLevel == eMsgLevel.Failure)
                {
                    if ((levelLocal == eMsgLevel.Warning)
                        || (levelLocal == eMsgLevel.Info)
                        || (levelLocal == eMsgLevel.Debug)
                        || (levelLocal == eMsgLevel.Default))
                    {
                        levelLocal = msg.MessageLevel;
                    }
                }
                else if (msg.MessageLevel == eMsgLevel.Warning)
                {
                    if ((levelLocal == eMsgLevel.Info)
                        || (levelLocal == eMsgLevel.Debug)
                        || (levelLocal == eMsgLevel.Default))
                    {
                        levelLocal = msg.MessageLevel;
                    }
                }
                else if (msg.MessageLevel == eMsgLevel.Info)
                {
                    if ((levelLocal == eMsgLevel.Debug)
                        || (levelLocal == eMsgLevel.Default))
                    {
                        levelLocal = msg.MessageLevel;
                    }
                }
            }
            ValidMessage.MessageLevel = levelLocal;
        }
        #endregion

        #region Check BookingType
        /// <summary>
        /// Gibt an, ob es sich um einen normalen Buchungsaufruf handelt
        /// </summary>
        public bool IsCallForBooking
        {
            get
            {
                if (IsCallForMatching == true)
                    return false;
                if (IsCallForClosing == true)
                    return false;
                if (IsCallForInventory == true)
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Gibt an, ob es sich um einen Anbgleichaufruf handelt
        /// </summary>
        public bool IsCallForMatching
        {
            get
            {
                if ((BookingType >= GlobalApp.FacilityBookingType.MatchingFacilityChargeQuantities)
                    && (BookingType <= GlobalApp.FacilityBookingType.MatchingStockAll))
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Gibt an, ob es sich um einen Abschluss handelt
        /// </summary>
        public bool IsCallForClosing
        {
            get
            {
                if ((BookingType >= GlobalApp.FacilityBookingType.ClosingDay)
                    && (BookingType <= GlobalApp.FacilityBookingType.ClosingYear))
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Gibt an, ob es sich um eine Inventuranlage oder Inventurende handelt
        /// </summary>
        public bool IsCallForInventory
        {
            get
            {
                if ((BookingType == GlobalApp.FacilityBookingType.InventoryNew)
                    || (BookingType == GlobalApp.FacilityBookingType.InventoryClose))
                    return true;
                return false;
            }
        }
        #endregion

        #region FacilityManager
        [IgnoreDataMember]
        private List<ACMethodBooking> _FacilityBookings;
        [IgnoreDataMember]
        public List<ACMethodBooking> FacilityBookings
        {
            get
            {
                if (_FacilityBookings == null)
                    _FacilityBookings = new List<ACMethodBooking>();
                return _FacilityBookings;
            }
            set
            {
                _FacilityBookings = value;
            }
        }

        [IgnoreDataMember]
        private List<FacilityBookingCharge> _CreatedPostings;
        [IgnoreDataMember]
        public List<FacilityBookingCharge> CreatedPostings
        {
            get
            {
                if (_CreatedPostings == null)
                    _CreatedPostings = new List<FacilityBookingCharge>();
                return _CreatedPostings;
            }
            set
            {
                _CreatedPostings = value;
            }
        }

        [IgnoreDataMember]
        private ACStackCalculatorBase _stackCalculatorInward = null;
        public ACStackCalculatorBase StackCalculatorInward(FacilityManager manager)
        {
            if (_stackCalculatorInward != null && _stackCalculatorInward.ACType == ParamsAdjusted.InwardStackCalculatorType)
                return _stackCalculatorInward;

            else if (_stackCalculatorOutward != null)
            {
                if (ParamsAdjusted.InwardStackCalculatorType == ParamsAdjusted.OutwardStackCalculatorType)
                {
                    _stackCalculatorInward = _stackCalculatorOutward;
                    return _stackCalculatorInward;
                }
            }

            if (ParamsAdjusted.InwardStackCalculatorType != null)
            {
                _stackCalculatorInward = manager.FindChildComponents(ParamsAdjusted.InwardStackCalculatorType, 1).FirstOrDefault() as ACStackCalculatorBase;
                if (_stackCalculatorInward == null)
                    _stackCalculatorInward = manager.StartComponent(ParamsAdjusted.InwardStackCalculatorType, ParamsAdjusted.InwardStackCalculatorType, null) as ACStackCalculatorBase;
            }
            else if (ParamsAdjusted.OutwardStackCalculatorType != null)
            {
                _stackCalculatorInward = StackCalculatorOutward(manager);
            }

            return _stackCalculatorInward;
        }

        [IgnoreDataMember]
        private ACStackCalculatorBase _stackCalculatorOutward = null;
        public ACStackCalculatorBase StackCalculatorOutward(FacilityManager manager)
        {
            if (_stackCalculatorOutward != null && _stackCalculatorOutward.ACType == ParamsAdjusted.OutwardStackCalculatorType)
                return _stackCalculatorOutward;
            if (_stackCalculatorInward != null)
            {
                if (ParamsAdjusted.OutwardStackCalculatorType == ParamsAdjusted.InwardStackCalculatorType)
                {
                    _stackCalculatorOutward = _stackCalculatorInward;
                    return _stackCalculatorOutward;
                }
            }
            if (ParamsAdjusted.OutwardStackCalculatorType != null)
            {
                _stackCalculatorOutward = manager.FindChildComponents(ParamsAdjusted.OutwardStackCalculatorType, 1).FirstOrDefault() as ACStackCalculatorBase;
                if (_stackCalculatorOutward == null)
                    _stackCalculatorOutward = manager.StartComponent(ParamsAdjusted.OutwardStackCalculatorType, ParamsAdjusted.OutwardStackCalculatorType, null) as ACStackCalculatorBase;
            }
            else if (ParamsAdjusted.InwardStackCalculatorType != null)
            {
                _stackCalculatorOutward = StackCalculatorInward(manager);
            }
            return _stackCalculatorOutward;
        }

        #endregion
    }
}
