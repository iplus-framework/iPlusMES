using gip.core.datamodel;
using System.Runtime.Serialization;

namespace gip.mes.datamodel
{
#if NETFRAMEWORK
    [ACSerializeableInfo]
#else
    [DataContract]
#endif
    public class GlobalApp
    {
        #region ProcOrUnit
        /// <summary>
        /// Enum für das Feld ProcOrUnitIndex
        /// </summary>

#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'ProcOrUnit'}de{'ProcOrUnit'}", Global.ACKinds.TACEnum)]
#else
       [DataContract]
#endif
        public enum ProcOrUnit : short
        {
            Kg = 1, //kg
            Percent = 2, //%
        }
        #endregion

        #region Month
        /// <summary>
        /// Enum für das Feld PeriodIndex
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'MonthRange'}de{'MonthRange'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif
        public enum MonthRange : short
        {
            Janurary = 1,   // Januar
            February = 2,   // Februar
            March = 3,      // März
            April = 4,      // April
            Mai = 5,        // Mai
            June = 6,       // Juni
            July = 7,       // Juli
            August = 8,     // August
            September = 9,  // September
            October = 10,   // Oktober
            November = 11,  // November
            December = 12,  // Dezember
        }
        #endregion

        #region PrognosisProcedure
        /// <summary>
        /// Enum für das Feld PeriodIndex
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'PrognosisProcedure'}de{'PrognosisProcedure'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif
        public enum PrognosisProcedure : short
        {
            MovingAverage = 1,          // Gleitender Mittelwert
            ExponentialSmoothing1 = 2,  // Exponentielle Glättung erster Ordnung
            ExponentialSmoothing2 = 3   // Exponentielle Glättung zweiter Ordnung
        }
        #endregion

        #region ShiftState
        /// <summary>
        /// Enum für das Feld ShiftStateIndex
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'ShiftStates'}de{'ShiftStates'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif
        public enum ShiftStates : short
        {
            Activ = 1,
            Ill = 2,
            Holiday = 3,
            Other = 4,
        }
        #endregion

        #region TimePeriods
        /// <summary>
        /// Enum für das Feld MDTimePeriodIndex
        /// </summary>
#if NETFRAMEWORK
        [ACClassInfo(Const.PackName_VarioSystem, "en{'TimePeriods'}de{'TimePeriods'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum TimePeriods : short
        {
            Day = 1, //Tag
            Week = 2, //Woche
            Month = 3, //Monat
            Year = 4, //Jahr
        }

#if NETFRAMEWORK
        static ACValueItemList _TimePeriodsList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList TimePeriodsList
        {
            get
            {
                if (GlobalApp._TimePeriodsList == null)
                {

                    GlobalApp._TimePeriodsList = new ACValueItemList("TimePeriodIndex");

                    GlobalApp._TimePeriodsList.AddEntry((short)TimePeriods.Day, "en{'Day'}de{'Tag'}");
                    GlobalApp._TimePeriodsList.AddEntry((short)TimePeriods.Week, "en{'Week'}de{'Woche'}");
                    GlobalApp._TimePeriodsList.AddEntry((short)TimePeriods.Month, "en{'Month'}de{'Monat'}");
                    GlobalApp._TimePeriodsList.AddEntry((short)TimePeriods.Year, "en{'Year'}de{'Jahr'}");
                }
                return GlobalApp._TimePeriodsList;
            }
        }
#endif
        #endregion

        #region OrderTypes
        /// <summary>
        /// Enum für das Feld OrderTypeIndex
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'OrderTypes'}de{'OrderTypes'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum OrderTypes : short
        {
            /// <summary>
            /// Order
            /// Auftrag/Bestellung
            /// </summary>
            Order = 1,
            
            /// <summary>
            /// Purchase agreement (Contract) 
            /// Rahmenvertrag (Kontrakt)
            /// </summary>
            Contract = 2,  

            /// <summary>
            /// Internal Order
            /// Interner Auftrag
            /// </summary>
            InternalOrder = 3,

            /// <summary>
            /// Release Order
            /// Kontraktabruf
            /// </summary>
            ReleaseOrder = 4,
        }

#if NETFRAMEWORK
        static ACValueItemList _OrderTypesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList OrderTypesList
        {
            get
            {
                if (GlobalApp._OrderTypesList == null)
                {
                    GlobalApp._OrderTypesList = new ACValueItemList("OrderTypeIndex");
                    GlobalApp._OrderTypesList.AddEntry((short)OrderTypes.Order, "en{'Order'}de{'Auftrag/Bestellung'}");
                    GlobalApp._OrderTypesList.AddEntry((short)OrderTypes.Contract, "en{'Purchase agreement (Contract)'}de{'Rahmenvertrag (Kontrakt)'}");
                    GlobalApp._OrderTypesList.AddEntry((short)OrderTypes.InternalOrder, "en{'Internal Order'}de{'Interner Auftrag'}");
                    GlobalApp._OrderTypesList.AddEntry((short)OrderTypes.ReleaseOrder, "en{'Release Order'}de{'Kontraktabruf'}");
                }
                return GlobalApp._OrderTypesList;
            }
        }
#endif
        #endregion


         #region InvoiceTypes
        /// <summary>
        /// Enum für das Feld OrderTypeIndex
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'InvoiceTypes'}de{'InvoiceTypes'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum InvoiceTypes : short
        {
            /// <summary>
            /// Order
            /// Auftrag/Bestellung
            /// </summary>
            Invoice = 1,
            
            /// <summary>
            /// Purchase agreement (Contract) 
            /// Rahmenvertrag (Kontrakt)
            /// </summary>
            Contract = 2,  

            /// <summary>
            /// Internal Order
            /// Interner Auftrag
            /// </summary>
            InternalInvoice = 3,

            /// <summary>
            /// Release Order
            /// Kontraktabruf
            /// </summary>
            ReleaseInvoice = 4,
        }

#if NETFRAMEWORK
        static ACValueItemList _InvoiceTypesList = null;
        /// <summary>
        /// Gibt eine Liste mit Übersetzungen an die GUI zurück
        /// </summary>
        public static ACValueItemList InvoiceTypesList
        {
            get
            {
                if (GlobalApp._InvoiceTypesList == null)
                {
                    GlobalApp._InvoiceTypesList = new ACValueItemList("InvoiceTypeIndex");
                    GlobalApp._InvoiceTypesList.AddEntry((short)InvoiceTypes.Invoice, "en{'Invoice'}de{'Rechnung'}");
                    GlobalApp._InvoiceTypesList.AddEntry((short)InvoiceTypes.Contract, "en{'Purchase agreement (Contract)'}de{'Rahmenvertrag (Kontrakt)'}");
                    GlobalApp._InvoiceTypesList.AddEntry((short)InvoiceTypes.InternalInvoice, "en{'Internal Invoice'}de{'Interner Rechnung'}");
                    GlobalApp._InvoiceTypesList.AddEntry((short)InvoiceTypes.ReleaseInvoice, "en{'Release Invoice'}de{'Rechnungbruf'}");
                }
                return GlobalApp._InvoiceTypesList;
            }
        }
#endif
        #endregion

        #region MaterialPosTypes
        /// <summary>
        /// Enum für Positionstypen
        /// Nummernlogik:
        /// 1.Stelle: 1=Zugang, 2=Verbrauch
        /// 2.Stelle: Ebene / Tiefe duch Parentverweise
        /// 3.Stelle: 1=Externe Buchung vom/ins Lager, 2=Interne Buchung 
        /// 4.Stelle: Reserve
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'MaterialPosTypes'}de{'MaterialPosTypes'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum MaterialPosTypes : short
        {
            /// <summary>
            /// "1ΣH"
            /// Positionstyp für Positionen, bei denen eine Zugangsbuchung ins Lager erfolgen soll (Material wird eingelagert)
            /// Ist Wurzelposition ohne Verweis zur Elternposition
            /// Beispiele:
            /// 1. Das zu bestellende Produkt (InOrderPos)
            /// 2. Oder bei der Produktion das herzustellende Produkt (ProdOrderPartslistPos).
            /// </summary>
            InwardRoot = 1110,

            /// <summary>
            /// "2ΣH"
            /// Positionstyp für Positionen, bei denen eine Zugangsbuchung ins Lager erfolgen soll (Material wird eingelagert)
            /// Ist Kindposition mit Verweis zur Elternposition
            /// Beispiele:
            /// 1. Lieferscheinposition: Teilabruf von eines Kundenauftrags (InOrderPos, Hat Parentverweis zu "1ΣH"/InwardRoot) 
            /// 2. Kommissionierposition: Teilmenge einer Lieferscheinposition (InOrderPos, Hat Parentverweis zu "2ΣH"/OutwardPart) 
            ///                             oder Teilmenge einer Stücklistenposition (ProdOrderPartslistPos, Hat Parentverweis zu "1ΣH"/InwardRoot) die entweder durch einen Batchdurchlauf entsteht oder durch einen Kommssionierauftrag
            /// </summary>
            InwardPart = 1210,

            /// <summary>
            /// "1Σ"
            /// Positionstyp für Positionen, bei denen eine INTERNE Zugangsbuchung erfolgen soll (Zwischenprodukt oder HandlingUnit)
            /// Ist Wurzelposition ohne Verweis zur Elternposition
            /// Beispiele:
            /// 1. Summenposition von "1K"/OutwardRoot-Positionen (ProdOrderPartslistPos).
            /// </summary>
            InwardIntern = 1120,

            /// <summary>
            /// "2Σ"
            /// Positionstyp für Positionen, bei denen eine INTERNE Zugangsbuchung erfolgen soll (Zwischenprodukt oder HandlingUnit)
            /// Ist Kindposition mit Verweis zur Elternposition
            /// Beispiele:
            /// 1. Hergestellte Teilmenge die bei entweder bei einem Batchdurchlauf entsteht (ProdOrderPartslistPos, Hat Parentverweis zu "1Σ"/InwardIntern).
            /// </summary>
            InwardPartIntern = 1220,

            /// <summary>
            /// "BZ"
            /// Positionstyp für Positionen, bei denen eine INTERNE Abgangsbuchung erfolgt hat und gleichzeitig eine Zugangsbuchung ins Lager erfolgen soll (Endprodukt)
            /// Ist Kindposition mit Verweis zur Elternposition "2ΣH"/InwardPart
            /// Beispiele:
            /// 1. Entnomme Teilmenge einer Zwischenproduktcharge die gelichzeitig auch das hergestellte Endprodukt ist (ProdOrderPartslistPos, Hat Parentverweis zu "2ΣH"/InwardPart und Verbrauchsverweis zu "2Σ"/InwardPartIntern).
            /// </summary>
            OutwardInternInwardExtern = 1310,

            /// <summary>
            /// "1K"
            /// Positionstyp für Positionen, bei denen eine Abgangsbuchung aus dem Lager erfolgen soll (Material wird ausgelagert)
            /// Ist Wurzelposition ohne Verweis zur Elternposition
            /// Beispiele:
            /// 1. Das zu liefernde Produkt bei einem Kundenauftrag (OutOrderPos)
            /// 2. Oder bei der Produktion das verbrauchte Material (ProdOrderPartslistPos).
            /// </summary>
            OutwardRoot = 2110,

            /// <summary>
            /// "2K"
            /// Positionstyp für Positionen, bei denen eine Abgangsbuchung aus dem Lager erfolgen soll (Material wird ausgelagert)
            /// Ist Kindposition mit Verweis zur Elternposition 
            /// Beispiele:
            /// 1. Lieferscheinposition: Teilabruf von einer Bestellung (OutOrderPos, Hat Parentverweis zu "1K"/OutwardRoot) 
            /// 2. Kommissionierposition: Teilmenge einer Lieferscheinposition (OutOrderPos, Hat Parentverweis zu "2K"/OutwardPart) 
            ///                             oder Teilmenge einer Stücklistenposition (ProdOrderPartslistPos, Hat Parentverweis zu "1K"/OutwardRoot) die entweder durch einen Batchdurchlauf entsteht oder durch einen Kommssionierauftrag
            /// </summary>
            OutwardPart = 2210,

            /// <summary>
            /// "1ΣK"
            /// Positionstyp für Positionen, bei denen eine INTERNE Abgangsbuchung erfolgen soll (von Zwischenprodukt oder HandlingUnit)
            /// Dieser Positionstyp entsteht immer durch eine Edge bzw. entspricht einer Edge
            /// Ist Wurzelposition ohne Verweis zur Elternposition und hat Verweis zu einer
            /// Beispiele:
            /// 1. Stücklistenposition die den Teil- oder Gesamtmenge eines Zwischenproduktes als Verbrauchspsotion darstellt (ProdOrderPartslistPos).
            /// </summary>
            OutwardIntern = 2120,

            /// <summary>
            /// "2ΣK"
            /// Positionstyp für Positionen, bei denen eine INTERNE Abgangsbuchung erfolgen soll (Zwischenprodukt oder HandlingUnit)
            /// Ist Kindposition mit Verweis zur Elternposition "1ΣK"
            /// Beispiele:
            /// 1. Entnomme Teilmenge eines Zwischenproduktes die bei einem Batchdurchlauf entsteht (ProdOrderPartslistPos, Hat Parentverweis zu "1ΣK"/OutwardIntern).
            /// </summary>
            OutwardPartIntern = 2220,

            /// <summary>
            /// "VBI"
            /// Positionstyp für Positionen, bei denen eine INTERNE Abgangsbuchung erfolgt hat (Zwischenprodukt oder HandlingUnit)
            /// Ist Kindposition mit Verweis zur Elternposition "2ΣK"/OutwardPartIntern
            /// Beispiele:
            /// 1. Entnomme Teilmenge einer Zwischenproduktcharge (ProdOrderPartslistPos, Hat Parentverweis zu "2ΣK"/OutwardPartIntern und Verbrauchsverweis zu "2Σ"/InwardPartIntern).
            /// </summary>
            OutwardPartInternBooking = 2320,
        }
        #endregion

        #region FacitlityBookingType
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'FacilityBookingType'}de{'FacilityBookingType'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum FacilityBookingType : short
        {
            // Lagerzugang auf Lagercharge (Ungeplant) (Bestand+)
            InwardMovement_FacilityCharge = 101,

            // Lagerabgang auf Lagercharge (Ungeplant) (Bestand-)
            OutwardMovement_FacilityCharge = 102,

            // Umlagerung zwischen zwei Lagerchargen (Ungeplant)
            Relocation_FacilityCharge = 103,

            // Umlagerung von einer Lagercharge auf einen Lagerplatz (Ungeplant)
            Relocation_FacilityCharge_Facility = 104,

            // Umlagerung von einer Lagercharge auf einen Lagerort (Ungeplant)
            Relocation_FacilityCharge_FacilityLocation = 105,

            // Nullbestandsbuchung auf Lagercharge entsprechend ZeroStockState
            ZeroStock_FacilityCharge = 106,

            // Freigabe und Sperrung auf Lagercharge entsprechend ReleaseState
            ReleaseState_FacilityCharge = 107,

            // Lagerzugang auf ein Silo/Tank/Container von chargen- und nicht chargengeführtem losem Material (Ungeplant) (Bestand+)
            InwardMovement_Facility_BulkMaterial = 201,

            // Lagerabgang von einen Silo/Tank/Container von chargen- und nicht chargengeführtem losem Material (Ungeplant) (Bestand-)
            OutwardMovement_Facility_BulkMaterial = 202,

            // Umlagerung zwischen zwei Silos/Tanks/Container von chargen- und nicht chargengeführtem losem Material (Ungeplant)
            Relocation_Facility_BulkMaterial = 203,

            // Lagerzugang auf einen Lagerplatz von einem bestimmten verpackten Material oder Los (Ungeplant) (Bestand+)
            //InwardMovement_Facility_PackagedMaterial = 204,

            // Lagerabgang von einem Lagerplatz von einem bestimmten verpackten Material oder Los (Ungeplant) (Bestand-)
            //OutwardMovement_Facility_PackagedMaterial = 205,

            // Umlagerung zwischen zwei Lagerplätzen von einem bestimmten verpackten Material oder Los (Ungeplant)
            //Relocation_Facility_PackagedMaterial = 206,

            // Nullbestandsbuchung auf einem Silo/Tank/Container
            ZeroStock_Facility_BulkMaterial = 207,

            // Freigabe und Sperrung des Materials von einem Silo/Tank/Container
            ReleaseState_Facility_BulkMaterial = 208,


            // Umbuchung des Materials auf eine andere Materialnummer
            Reassign_FacilityCharge = 300,

            // Umbuchung des Materials auf eine andere Materialnummer von einem Silo/Tank/Container
            Reassign_Facility_BulkMaterial = 301,


            // Abgleich StockQuantityUOM mit StockQuantity von übergebener FacilityCharge
            MatchingFacilityChargeQuantities = 900,

            // Abgleich über alle FacilityCharges im Lager
            MatchingFacilityChargeQuantitiesAll = 901,

            // Abgleich Stock- und Reserved-Felder mit Summe aus FacilityCharges von dem Material
            MatchingMaterialStock = 902,

            // Abgleich über alle Materialien im Lager
            MatchingMaterialStockAll = 903,

            // Abgleich Stock- und Reserved-Felder mit Summe aus FacilityCharges von der Facility
            MatchingFacilityStock = 904,

            // Abgleich über alle Facilites im Lager
            MatchingFacilityStockAll = 905,

            // Abgleich Stock- und Reserved-Felder mit Summe aus FacilityCharges von der FacilityLot
            MatchingFacilityLotStock = 906,

            // Abgleich über alle FacilityLots im Lager
            MatchingFacilityLotStockAll = 907,

            // Abgleich Stock- und Reserved-Felder mit Summe aus FacilityCharges von der Stückliste
            MatchingPartslistStock = 908,

            // Abgleich über alle Stücklisten im Lager
            MatchingPartslistStockAll = 909,

            // Abgleich von allen Entitäten
            MatchingStockAll = 910,

            // Abgleich Firmenbestand
            MatchingCompanyMaterialStock = 911,

            // Abgleich Firmenbestand über alle Materialien im Lager
            MatchingCompanyMaterialStockAll = 912,

            // Abschluss
            ClosingDay = 920,
            ClosingWeek = 921,
            ClosingMonth = 922,
            ClosingYear = 923,

            // Inventur
            InventoryNew = 930,
            InventoryStockCorrection = 931,
            InventoryNewQuant = 932,
            InventoryClose = 939,

            InOrderPosInwardMovement = 21,			    // Wareneingang für Bestellposition (Bestand+, Bestellter Bestand-)
            InOrderPosCancel = 22,               // Stornierung einer Bestellposition (Bestand-, Reservierter Bestand+)
            InOrderPosActivate = 26,                    // Aktivierung einer Bestellposition (Bestellter Bestand+)

            OutOrderPosOutwardMovement = 32,               // Warenausgang für Auftragsposition (Bestand-, Reservierter Bestand-)
            OutOrderPosCancel = 33,               // Stornierung einer Auftragsposition (Bestand+, Reservierter Bestand+)
            OutOrderPosActivate = 36,               // Aktivierung einer Auftragsposition (Reservierter Bestand+)

            // TODO:
            ProdOrderPosInward = 41,               // Wareneingang für Auftragsposition (Bestand+, Reservierter Bestand-)
            ProdOrderPosInwardCancel = 42,               // Wareneingang für Auftragsposition (Bestand+, Reservierter Bestand-)
            ProdOrderPosOutward = 51,              // Warenausgang für Auftragsposition (Bestand-, Reservierter Bestand+)
            ProdOrderPosOutwardCancel = 52,              // Warenausgang für Auftragsposition (Bestand-, Reservierter Bestand+)
            ProdOrderPosOutwardOnEmptyingFacility = 53,   // // Warenausgang für Auftragsposition (Bestand-, Reservierter Bestand+)
            //ProdOrderInwardActivate = 16,               // Aktivierung einer geplanten Herstellmenge (Reservierter Bestand+)
            //ProdOrderOutwardActivate = 16,              // Aktivierung eines geplanten Materialabgangs (Reservierter Bestand-)

            ChangeStorageUnit = 111,                    // Lagereinheiten ändern

            StockCorrection = 121,                    // Bestandskorrektur 

            CorrectionCostRateAll = 159,
            CorrectionCostRateMaterial = 160,

            MatchingCostRateMaterialInwardFacilityChargeAll = 161,             // Korrektur Kostensatz
            MatchingCostRateMaterialInwardFacilityCharge = 162,        // Korrektur Kostensatz für bestimmtes Material
        }

        public const string FBT_InwardMovement_FacilityCharge = "InwardMovement_FacilityCharge";
        public const string FBT_OutwardMovement_FacilityCharge = "OutwardMovement_FacilityCharge";
        public const string FBT_Relocation_FacilityCharge = "Relocation_FacilityCharge";
        public const string FBT_Relocation_FacilityCharge_Facility = "Relocation_FacilityCharge_Facility";
        public const string FBT_Relocation_FacilityCharge_FacilityLocation = "Relocation_FacilityCharge_FacilityLocation";
        public const string FBT_ZeroStock_FacilityCharge = "ZeroStock_FacilityCharge";
        public const string FBT_ReleaseState_FacilityCharge = "ReleaseState_FacilityCharge";
        public const string FBT_InwardMovement_Facility_BulkMaterial = "InwardMovement_Facility_BulkMaterial";
        public const string FBT_OutwardMovement_Facility_BulkMaterial = "OutwardMovement_Facility_BulkMaterial";
        public const string FBT_Relocation_Facility_BulkMaterial = "Relocation_Facility_BulkMaterial";
        //public const string FBT_InwardMovement_Facility_PackagedMaterial = "InwardMovement_Facility_PackagedMaterial";
        //public const string FBT_OutwardMovement_Facility_PackagedMaterial = "OutwardMovement_Facility_PackagedMaterial";
        //public const string FBT_Relocation_Facility_PackagedMaterial = "Relocation_Facility_PackagedMaterial";
        public const string FBT_ZeroStock_Facility_BulkMaterial = "ZeroStock_Facility_BulkMaterial";
        public const string FBT_ReleaseState_Facility_BulkMaterial = "ReleaseState_Facility_BulkMaterial";
        public const string FBT_Reassign_FacilityCharge = "Reassign_FacilityCharge";
        public const string FBT_Reassign_Facility_BulkMaterial = "Reassign_Facility_BulkMaterial";
        public const string FBT_MatchingFacilityChargeQuantities = "MatchingFacilityChargeQuantities";
        public const string FBT_MatchingFacilityChargeQuantitiesAll = "MatchingFacilityChargeQuantitiesAll";
        public const string FBT_MatchingMaterialStock = "MatchingMaterialStock";
        public const string FBT_MatchingMaterialStockAll = "MatchingMaterialStockAll";
        public const string FBT_MatchingFacilityStock = "MatchingFacilityStock";
        public const string FBT_MatchingFacilityStockAll = "MatchingFacilityStockAll";
        public const string FBT_MatchingFacilityLotStock = "MatchingFacilityLotStock";
        public const string FBT_MatchingFacilityLotStockAll = "MatchingFacilityLotStockAll";
        public const string FBT_MatchingPartslistStock = "MatchingPartslistStock";
        public const string FBT_MatchingPartslistStockAll = "MatchingPartslistStockAll";
        public const string FBT_MatchingStockAll = "MatchingStockAll";
        public const string FBT_ClosingDay = "ClosingDay";
        public const string FBT_ClosingWeek = "ClosingWeek";
        public const string FBT_ClosingMonth = "ClosingMonth";
        public const string FBT_ClosingYear = "ClosingYear";
        public const string FBT_InventoryNew = "InventoryNew";
        public const string FBT_InventoryNewQuant = "InventoryNewQuant";
        public const string FBT_InventoryStockCorrection = "InventoryStockCorrection";
        public const string FBT_InventoryClose = "InventoryClose";
        public const string FBT_InOrderPosInwardMovement = "InOrderPosInwardMovement";
        public const string FBT_InOrderPosCancel = "InOrderPosCancel";
        public const string FBT_InOrderPosActivate = "InOrderPosActivate";
        public const string FBT_OutOrderPosOutwardMovement = "OutOrderPosOutwardMovement";
        public const string FBT_OutOrderPosCancel = "OutOrderPosCancel";
        public const string FBT_OutOrderPosActivate = "OutOrderPosActivate";
        public const string FBT_ChangeStorageUnit = "ChangeStorageUnit";
        public const string FBT_StockCorrection = "StockCorrection";
        public const string FBT_CorrectionCostRateAll = "CorrectionCostRateAll";
        public const string FBT_CorrectionCostRateMaterial = "CorrectionCostRateMaterial";
        public const string FBT_MatchingCostRateMaterialInwardFacilityChargeAll = "MatchingCostRateMaterialInwardFacilityChargeAll";
        public const string FBT_MatchingCostRateMaterialInwardFacilityCharge = "MatchingCostRateMaterialInwardFacilityCharge";
        public const string FBT_ProdOrderPosInward = "ProdOrderPosInward";
        public const string FBT_ProdOrderPosInwardCancel = "ProdOrderPosInwardCancel";
        public const string FBT_ProdOrderPosOutward = "ProdOrderPosOutward";
        public const string FBT_ProdOrderPosOutwardCancel = "ProdOrderPosOutwardCancel";
        public const string FBT_ProdOrderPosOutwardOnEmptyingFacility = "ProdOrderPosOutwardOnEmptyingFacility";

#if NETFRAMEWORK
        static ACValueItemList _FacilityBookingTypeList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList FacilityBookingTypeList
        {
            get
            {
                if (GlobalApp._FacilityBookingTypeList == null)
                {

                    GlobalApp._FacilityBookingTypeList = new ACValueItemList("FacilityBookingTypeIndex");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.InwardMovement_FacilityCharge, "en{'Inward Movement quant'}de{'Lagerzugang auf einem Quant (Ungeplant) (Bestand+)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.OutwardMovement_FacilityCharge, "en{'Outward Movement quant'}de{'Lagerabgang auf einem Quant (Ungeplant) (Bestand-)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.Relocation_FacilityCharge, "en{'Relocation quant'}de{'Umlagerung zwischen zwei Quanten (Ungeplant)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.Relocation_FacilityCharge_Facility, "en{'Relocation quant on Facility'}de{'Umlagerung eines Quants auf einen Lagerplatz (Ungeplant)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.Relocation_FacilityCharge_FacilityLocation, "en{'Relocation quant - Facility Location'}de{'Umlagerung von eines Quants auf einen Lagerort (Ungeplant)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ZeroStock_FacilityCharge, "en{'Zero Stock on quant'}de{'Nullbestandsbuchung auf Quant'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ReleaseState_FacilityCharge, "en{'Release State on quant'}de{'Freigabe und Sperrung eines Quants'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.InwardMovement_Facility_BulkMaterial, "en{'Inward Movement Facility Bulk Material'}de{'Lagerzugang auf ein Silo/Tank/Container von chargen- und nicht chargengeführtem losem Material (Ungeplant) (Bestand+)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.OutwardMovement_Facility_BulkMaterial, "en{'Outward Movement Facility Bulk Material'}de{'Lagerabgang von einen Silo/Tank/Container von chargen- und nicht chargengeführtem losem Material (Ungeplant) (Bestand-)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.Relocation_Facility_BulkMaterial, "en{'Relocation Facility Bulk Material'}de{'Umlagerung zwischen zwei Silos/Tanks/Container von chargen- und nicht chargengeführtem losem Material (Ungeplant)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ZeroStock_Facility_BulkMaterial, "en{'Zero Stock on bulk facility'}de{'Nullbestandsbuchung Lagerplatz'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ReleaseState_Facility_BulkMaterial, "en{'Release State Facility Bulk Material'}de{'Freigabe und Sperrung des Materials von einem Silo/Tank/Container'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.Reassign_FacilityCharge, "en{'Reassign materialnumber on quant'}de{'Umbuchung Materialnummer auf einem Quant'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.Reassign_Facility_BulkMaterial, "en{'Reassign materialnumber Facility Bulk Material'}de{'Umbuchung Materialnummer von einem Silo/Tank/Container'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingFacilityChargeQuantities, "en{'Matching Quantites of quant'}de{'Abgleich StockQuantityUOM mit StockQuantity des Quants'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingFacilityChargeQuantitiesAll, "en{'Matching over all Quants'}de{'Abgleich über alle Quanten im Lager'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingMaterialStock, "en{'Matching Material Stock'}de{'Abgleich Stock- und Reserved-Felder mit Summe aus Quanten von dem Material'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingMaterialStockAll, "en{'Matching Material Stock All'}de{'Abgleich über alle Materialien im Lager'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingFacilityStock, "en{'Matching Facility Stock'}de{'Abgleich Stock- und Reserved-Felder mit Summe der Quanten auf dem Lagerpatz'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingFacilityStockAll, "en{'Matching Facility Stock All'}de{'Abgleich über alle Quanten im Lager'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingFacilityLotStock, "en{'Matching Facility Lot Stock'}de{'Abgleich Stock- und Reserved-Felder mit Summe aus Quanten von der FacilityLot'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingFacilityLotStockAll, "en{'Matching Facility Lot Stock All'}de{'Abgleich über alle Lose im Lager'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingPartslistStock, "en{'Matching Bill of Materials Stock'}de{'Abgleich Stock- und Reserved-Felder mit Summe aus Quanten von der Stückliste'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingPartslistStockAll, "en{'Matching Bill of Materials Stock All'}de{'Abgleich über alle Stücklisten im Lager'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingStockAll, "en{'Matching Stock All'}de{'Abgleich von allen Entitäten'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingCompanyMaterialStock, "en{'Matching Company Material Stock'}de{'Abgleich Firmenbestand'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingCompanyMaterialStockAll, "en{'Matching Company Material Stock All'}de{'Abgleich über Firmenbestand alle Materialien im Lager'}");

                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ClosingDay, "en{'Closing Day'}de{'Tagesabschluss'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ClosingWeek, "en{'Closing Week'}de{'Wochenabschluss'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ClosingMonth, "en{'Closing Month'}de{'Monatsabschluss'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ClosingYear, "en{'Closing Year'}de{'Jahersabschluss'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.InventoryNew, "en{'Inventory New'}de{'Neue Inventur'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.InventoryNewQuant, "en{'Inventory new quant'}de{'Neuer quant bei Inventur'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.InventoryStockCorrection, "en{'Inventory Stock Correction'}de{'Korrektur per Inventurbestand'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.InventoryClose, "en{'Inventory Close'}de{'Inventur beenden'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.InOrderPosInwardMovement, "en{'In Order Pos Inward Movement'}de{'Wareneingang für Bestellposition (Bestand+, Bestellter Bestand-)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.InOrderPosCancel, "en{'Cancel orderline'}de{'Stornierung einer Bestellposition (Bestand-)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.InOrderPosActivate, "en{'In Order Pos Activate'}de{'Aktivierung einer Bestellposition (Bestellter Bestand+)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.OutOrderPosOutwardMovement, "en{'Out Order Pos Outward Movement'}de{'Warenausgang für Auftragsposition (Bestand-, Reservierter Bestand-)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.OutOrderPosCancel, "en{'Cancel orderline'}de{'Stornierung einer Auftragsposition (Bestand+)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.OutOrderPosActivate, "en{'Out Order Pos Activate'}de{'Aktivierung einer Auftragsposition (Reservierter Bestand+)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ChangeStorageUnit, "en{'Change Storage Unit'}de{'Lagereinheiten ändern'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.StockCorrection, "en{'Stock Correction'}de{'Bestandskorrektur'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.CorrectionCostRateAll, "en{'Correction Cost Rate All'}de{'CorrectionCostRateAll'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.CorrectionCostRateMaterial, "en{'Correction Cost Rate Material'}de{'CorrectionCostRateMaterial'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingCostRateMaterialInwardFacilityChargeAll, "en{'Matching Cost Rate of quant'}de{'Korrektur Kostensatz'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.MatchingCostRateMaterialInwardFacilityCharge, "en{'Matching Cost Rate Material'}de{'Korrektur Kostensatz für bestimmtes Material'}");

                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ProdOrderPosInward, "en{'Receipts from production (Stock+, Ordered Stock-)'}de{'Zugang von Produktion (Bestand+, Bestellter Bestand-)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ProdOrderPosInwardCancel, "en{'Cancellation of production receipt (Stock-)'}de{'Stornierung Produktionszugang (Bestand-)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ProdOrderPosOutward, "en{'Consumption of production (Stock-, Reserved Stock-)'}de{'Verbrauch von Produktion (Bestand-, Reservierter Bestand-)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ProdOrderPosOutwardCancel, "en{'Cancellation of production consumption (Stock+)'}de{'Stornierung Produktionsverbrauch (Bestand+)'}");
                    GlobalApp._FacilityBookingTypeList.AddEntry((short)FacilityBookingType.ProdOrderPosOutwardOnEmptyingFacility, "en{'Consumption of production on emptying facility (Stock-, Reserved Stock-)'}de{'Verbrauch von Produktion an Entleerungseinrichtung (Bestand-, Reservierter Bestand-)'}");
                }
                return GlobalApp._FacilityBookingTypeList;
            }
        }
#endif

        #endregion

        #region MaterialProcessState
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'FacilityBookingType'}de{'FacilityBookingType'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum MaterialProcessState : short
        {
            // Neue Buchung die unbewertet ist und vorerst als gut befunden ist
            New = 0,

            // Dosierte Menge war in Ordnung und ist Bestandteil des herzustellen Produkts
            Processed = 1,

            // Ist Ausschuss und ist nicht im hergestellten Produkt entahlten
            Discarded = 2,
        }

#if NETFRAMEWORK
        static ACValueItemList _MaterialProcessStateList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList MaterialProcessStateList
        {
            get
            {
                if (GlobalApp._MaterialProcessStateList == null)
                {

                    GlobalApp._MaterialProcessStateList = new ACValueItemList("MaterialProcessStateIndex");
                    GlobalApp._MaterialProcessStateList.AddEntry((short)MaterialProcessState.New, "en{'New'}de{'Neu'}");
                    GlobalApp._MaterialProcessStateList.AddEntry((short)MaterialProcessState.Processed, "en{'Is processed in product/batch'}de{'Eingearbeitet im Produkt/Batch'}");
                    GlobalApp._MaterialProcessStateList.AddEntry((short)MaterialProcessState.Discarded, "en{'Discarded/Is waste'}de{'Verworfen/Ausschuss'}");
                }
                return GlobalApp._MaterialProcessStateList;
            }
        }
#endif

        #endregion

        #region SIDimensions
        /// <summary>
        /// Enum SIDimensions
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'SIDimensions'}de{'SIDimensions'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum SIDimensions : short
        {
            /// <summary>
            /// Dimensionless
            /// </summary>
            None = 0,

            /// <summary>
            /// Base dimension: Length, Symbol: L, Unitname: meter, Unit-Symbol: m
            /// </summary>
            Length = 1,

            /// <summary>
            /// Base dimension: Mass, Symbol: M, Unitname: kilogram, Unit-Symbol: kg
            /// </summary>
            Mass = 2,

            /// <summary>
            /// Base dimension: Time, Symbol: T, Unitname: second, Unit-Symbol: s
            /// </summary>
            Time = 3,

            /// <summary>
            /// Base dimension: electric current, Symbol: I, Unitname: ampere, Unit-Symbol: A
            /// </summary>
            ElectricCurrent = 4,

            /// <summary>
            /// Base dimension: Thermodynamic temperature, Symbol: Θ, Unitname: kelvin, Unit-Symbol: K
            /// </summary>
            ThermodynamicTemp = 5,

            /// <summary>
            /// Base dimension: Amount of substance, Symbol: N, Unitname: mole, Unit-Symbol: mol
            /// </summary>
            AmountOfSubstance = 6,

            /// <summary>
            /// Base dimension: Luminous intensity, Symbol: J, Unitname: candela, Unit-Symbol: cd
            /// </summary>
            LuminousIntensity = 7,



            /// <summary>
            /// Derived dimension: frequency, Symbol: T−1, Unitname: hertz, Unit-Symbol: Hz
            /// </summary>
            Frequency = 100,

            /// <summary>
            /// Derived dimension: Angle, Symbol: dimensionless, Unitname: radian, Unit-Symbol: rad
            /// </summary>
            Angle = 101,

            /// <summary>
            /// Derived dimension: SolidAngle, Symbol: dimensionless, Unitname: steradian, Unit-Symbol: sr
            /// </summary>
            SolidAngle = 102,

            /// <summary>
            /// Derived dimension: Force, Symbol: M⋅L⋅T−2 , Unitname: newton, Unit-Symbol: N
            /// </summary>
            Force = 103,

            /// <summary>
            /// Derived dimension: Pressure, Symbol: M⋅L−1⋅T−2, Unitname: pascal, Unit-Symbol: Pa
            /// </summary>
            Pressure = 104,

            /// <summary>
            /// Derived dimension: Work, Symbol: M⋅L2⋅T−2, Unitname: joule, Unit-Symbol: J
            /// </summary>
            Work = 105,

            /// <summary>
            /// Derived dimension: Power, Symbol: M⋅L2⋅T−3, Unitname: watt, Unit-Symbol: W
            /// </summary>
            Power = 106,

            /// <summary>
            /// Derived dimension: Electric charge, Symbol: T⋅I, Unitname: coulomb, Unit-Symbol: C
            /// </summary>
            ElectricCharge = 107,

            /// <summary>
            /// Derived dimension: Voltage, Symbol: M⋅L2⋅T−3⋅I−1, Unitname: volt, Unit-Symbol: V
            /// </summary>
            Voltage = 108,

            /// <summary>
            /// Derived dimension: Electrical capacitance, Symbol: M−1⋅L−2⋅T4⋅I2, Unitname: farad, Unit-Symbol: F
            /// </summary>
            ElectricCapacitance = 109,

            /// <summary>
            /// Derived dimension: Electrical resistance, Symbol: M⋅L2⋅T−3⋅I−2, Unitname: ohm, Unit-Symbol: Ω
            /// </summary>
            ElectricResistance = 110,

            /// <summary>
            /// Derived dimension: Electrical conductance, Symbol: M−1⋅L−2⋅T3⋅I2, Unitname: siemens, Unit-Symbol: S
            /// </summary>
            ElectricalConductance = 111,

            /// <summary>
            /// Derived dimension: Magnetic flux, Symbol: M⋅L2⋅T−2⋅I−1, Unitname: weber, Unit-Symbol: Wb
            /// </summary>
            MagneticFlux = 112,

            /// <summary>
            /// Derived dimension: Magnetic field, Symbol: M⋅T−2⋅I−1, Unitname: tesla, Unit-Symbol: T
            /// </summary>
            MagneticField = 113,

            /// <summary>
            /// Derived dimension: Inductance, Symbol: M⋅L2⋅T−2⋅I−2, Unitname: henry, Unit-Symbol: H
            /// </summary>
            Inductance = 114,

            /// <summary>
            /// Derived dimension: Temperature, Symbol: Θ, Unitname: degree Celsius, Unit-Symbol: °C
            /// </summary>
            Temperature = 115,

            /// <summary>
            /// Derived dimension: Luminous flux, Symbol: J, Unitname: lumen, Unit-Symbol: lm
            /// </summary>
            LuminousFlux = 116,

            /// <summary>
            /// Derived dimension: Illuminance, Symbol: L−2⋅J, Unitname: lux, Unit-Symbol: lx
            /// </summary>
            Illuminance = 117,

            /// <summary>
            /// Derived dimension: Radioactivity, Symbol: T−1, Unitname: becquerel, Unit-Symbol: Bq
            /// </summary>
            Radioactivity = 118,

            /// <summary>
            /// Derived dimension: Absorbed dose, Symbol: L2⋅T−2, Unitname: gray, Unit-Symbol: Gy
            /// </summary>
            AbsorbedDose = 119,

            /// <summary>
            /// Derived dimension: Equivalent dose, Symbol: L²⋅T−², Unitname: sievert, Unit-Symbol: Sv
            /// </summary>
            EquivalentDose = 120,

            /// <summary>
            /// Derived dimension: Catalytic activity, Symbol: T−1⋅N, Unitname: katal, Unit-Symbol: kat
            /// </summary>
            CatalyticActivity = 121,


            /// <summary>
            /// Non-SI dimension: Area, Symbol: T−1⋅N, Unitname: hectar, Unit-Symbol: ha
            /// </summary>
            Area = 201,

            /// <summary>
            /// Non-SI dimension: Volume, Symbol: , Unitname: litre, Unit-Symbol: L
            /// </summary>
            Volume = 202,

        }

#if NETFRAMEWORK
        static ACValueItemList _SIDimensionList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList SIDimensionList
        {
            get
            {
                if (GlobalApp._SIDimensionList == null)
                {

                    GlobalApp._SIDimensionList = new ACValueItemList("SIDimensionIndex");

                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.None, "en{'Dimensionless'}de{'Dimensionslos'}");

                    // Base SI-Units:
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Length, "en{'Length (m, metre)'}de{'Länge (m, Meter)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Mass, "en{'Mass (kg, kilogram)'}de{'Masse (kg, Kilogramm)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Time, "en{'Time (s, second)'}de{'Zeit (s, Sekunde)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.ElectricCurrent, "en{'Electric Current (A, ampere)'}de{'Elektrischer Strom (A, Ampere)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.ThermodynamicTemp, "en{'Thermodynamic temperature (K, kelvin)'}de{'Thermodynamische Temperatur (K, Kelvin)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.AmountOfSubstance, "en{'Amount of substance (mol, mole)'}de{'Stoffmenge (mol, Mol)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.LuminousIntensity, "en{'Luminous intensity (cd, candela)'}de{'Lichtstärke (cd, Candela)'}");

                    // Derived SI-Units:
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Frequency, "en{'Frequency (Hz, hertz)'}de{'Frequenz (Hz, Hertz)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Angle, "en{'Angle (rad, radian)'}de{'Winkel (rad, Radiant)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.SolidAngle, "en{'Solid angle (sr, steradian)'}de{'Raumwinkel (sr, Steradiant)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Force, "en{'Force (N, newton)'}de{'Kraft (N, Newton)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Pressure, "en{'Pressure (Pa, pascal)'}de{'Druck (Pa, Pascal)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Work, "en{'Work (J, joule)'}de{'Arbeit (J, Joule)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Power, "en{'Power (W, watt)'}de{'Leistung (W, Watt)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.ElectricCharge, "en{'Electric charge (C, coulomb)'}de{'Elektrische Ladung (C, Coulomb)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Voltage, "en{'Voltage (V, volt)'}de{'Elektrische Spannung (V, Volt)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.ElectricCapacitance, "en{'Electrical capacitance (F, farad)'}de{'Elektrische Kapzität (F, Farad)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.ElectricResistance, "en{'Electrical resistance (Ω, ohm)'}de{'Elektrischer Widerstand (Ω, Ohm)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.ElectricalConductance, "en{'Electrical conductance (S, siemens)'}de{'Elektrische Leitwert (S, Siemens)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.MagneticFlux, "en{'Magnetic flux (Wb, weber)'}de{'Magnetischer Fluss (Wb, Weber)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.MagneticField, "en{'Magnetic field (T, tesla)'}de{'Magnetische Flussdichte (T, Tesla)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Inductance, "en{'Inductance (H, henry)'}de{'Induktivität (H, Henry)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Temperature, "en{'Temperature (°C, degree Celsius)'}de{'Temperatur (°C, Grad Celsius)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.LuminousFlux, "en{'Luminous flux (lm, lumen)'}de{'Lichtstrom (lm, Lumen)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Illuminance, "en{'Illuminance (lx, lux)'}de{'Beleuchtungsstärke (lx, Lux)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Radioactivity, "en{'Radioactivity (Bq, becquerel)'}de{'Radioaktivität (Bq, Becquerel)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.AbsorbedDose, "en{'Absorbed dose (Gy, gray)'}de{'Energiedosis (Gy, gray)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.EquivalentDose, "en{'Equivalent dose (Sv, sievert)'}de{'Äquivalentdosis (Sv, sievert)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.CatalyticActivity, "en{'Catalytic activity (kat, katal)'}de{'Katalytische Aktivität (kat, Katal)'}");

                    // Non SI-Units:
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Area, "en{'Area (ha, hectar)'}de{'Fläche (ha, Hektar)'}");
                    GlobalApp._SIDimensionList.AddEntry((short)SIDimensions.Volume, "en{'Volume (L, litre)'}de{'Volumen (L, Liter)'}");
                }
                return GlobalApp._SIDimensionList;
            }
        }
#endif
        #endregion

        #region PetroleumGroups
        /// <summary>
        /// Enum PetroleumGroups
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'PetroleumGroups'}de{'PetroleumGroups'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum PetroleumGroups : short
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,

            /// <summary>
            /// Raw oil, Rohöle A
            /// </summary>
            A = 10,

            /// <summary>
            /// Petrol, Benzin B.1
            /// </summary>
            B1 = 21,

            /// <summary>
            /// Naphtha, Waschbenzin B.2
            /// </summary>
            B2 = 22,

            /// <summary>
            /// Jet-A-1 Kerosene, Kerosin B.3
            /// </summary>
            B3 = 23,

            /// <summary>
            /// Fuel oil, Heizöl B.4
            /// </summary>
            B4 = 24,

            /// <summary>
            /// Spezialprodukte
            /// </summary>
            C = 25,

            /// <summary>
            /// Schmierprodukte
            /// </summary>
            D = 26,

            /// <summary>
            /// Liquid gases, Flüssiggase X-G1
            /// </summary>
            XG1 = 31,

            /// <summary>
            /// Liquid gases, Flüssiggase X-G2
            /// </summary>
            XG2 = 32,

            /// <summary>
            /// Liquid gases, Flüssiggase X-G3
            /// </summary>
            XG3 = 33,

            /// <summary>
            /// Liquid gases, Flüssiggase X-G4
            /// </summary>
            XG4 = 34,

            /// <summary>
            /// Bitumen X-B1
            /// </summary>
            XB1 = 41,

            /// <summary>
            /// Bitumen X-B2
            /// </summary>
            XB2 = 42,

            /// <summary>
            /// Bitumen X-B3
            /// </summary>
            XB3 = 43,
        }

#if NETFRAMEWORK
        static ACValueItemList _PetroleumGroupList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList PetroleumGroupList
        {
            get
            {
                if (GlobalApp._PetroleumGroupList == null)
                {

                    GlobalApp._PetroleumGroupList = new ACValueItemList("PetroleumGroupIndex");

                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.None, "en{'None'}de{'Keine'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.A, "en{'Raw oil group A'}de{'Rohöle Gruppe A'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.B1, "en{'Fuels group B.1'}de{'Benzin Gruppe B.1'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.B2, "en{'Naphtha group B.2'}de{'Waschbenzin Gruppe B.2'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.B3, "en{'Jet-A-1 Kerosene group B.3'}de{'Kerosin Gruppe B.3'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.B4, "en{'Fuel oil group B.4'}de{'Heizöl Gruppe B.4'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.C, "en{'Special products C'}de{'Spezialprodukte Gruppe C'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.D, "en{'Lubricants group D'}de{'Schmieröle Gruppe D'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.XG1, "en{'Liquid gases group X-G1'}de{'Flüssiggase Gruppe X-G1'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.XG2, "en{'Liquid gases group X-G2'}de{'Flüssiggase Gruppe X-G2'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.XG3, "en{'Liquid gases group X-G3'}de{'Flüssiggase Gruppe X-G3'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.XG4, "en{'Liquid gases group X-G4'}de{'Flüssiggase Gruppe X-G4'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.XB1, "en{'Bitumen group X-B1'}de{'Bitumen Gruppe X-B1'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.XB2, "en{'Bitumen group X-B2'}de{'Bitumen Gruppe X-B2'}");
                    GlobalApp._PetroleumGroupList.AddEntry((short)PetroleumGroups.XB3, "en{'Bitumen group X-B3'}de{'Bitumen Gruppe X-B3'}");
                }
                return GlobalApp._PetroleumGroupList;
            }
        }
#endif
        #endregion

        #region CompanyMaterialTypes
        /// <summary>
        /// Enum CompanyMaterialTypes
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'CompanyMaterialTypes'}de{'CompanyMaterialTypes'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum CompanyMaterialTypes : short
        {
            /// <summary>
            /// Material-Mapping
            /// </summary>
            MaterialMapping = 0,

            /// <summary>
            /// Pickup Masterdata, Abholstamm
            /// </summary>
            Pickup = 1,

        }

#if NETFRAMEWORK
        static ACValueItemList _CompanyMaterialTypeList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList CompanyMaterialTypeList
        {
            get
            {
                if (GlobalApp._CompanyMaterialTypeList == null)
                {

                    GlobalApp._CompanyMaterialTypeList = new ACValueItemList("CompanyMaterialTypeIndex");

                    GlobalApp._CompanyMaterialTypeList.AddEntry((short)CompanyMaterialTypes.MaterialMapping, "en{'Material Mapping'}de{'Material-Abbildung'}");
                    GlobalApp._CompanyMaterialTypeList.AddEntry((short)CompanyMaterialTypes.Pickup, "en{'Pickup'}de{'Abholung'}");
                }
                return GlobalApp._CompanyMaterialTypeList;
            }
        }
#endif
        #endregion

        #region DeliveryNoteType
        /// <summary>
        /// Enum DeliveryNoteType
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'Deliverynote type'}de{'Lieferscheintyp'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum DeliveryNoteType : short
        {
            Receipt = 0,
            Issue = 1,
        }

#if NETFRAMEWORK
        static ACValueItemList _DeliveryNoteTypeList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList DeliveryNoteTypeList
        {
            get
            {
                if (GlobalApp._DeliveryNoteTypeList == null)
                {

                    GlobalApp._DeliveryNoteTypeList = new ACValueItemList("DeliveryNoteTypeIndex");
                    GlobalApp._DeliveryNoteTypeList.AddEntry((short)DeliveryNoteType.Receipt, "en{'Receipt'}de{'Eingang'}");
                    GlobalApp._DeliveryNoteTypeList.AddEntry((short)DeliveryNoteType.Issue, "en{'Issue'}de{'Ausgang'}");
                }
                return GlobalApp._DeliveryNoteTypeList;
            }
        }
#endif
        #endregion

        #region PickingType
        /// <summary>
        /// Enum PickingType
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'Picking type'}de{'Kommissioniertyp'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum PickingType : short
        {
            Receipt = 0,
            Issue = 1,
            Production = 2,
            ReceiptVehicle = 3,
            IssueVehicle = 4,
        }

#if NETFRAMEWORK
        static ACValueItemList _PickingTypeList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList PickingTypeList
        {
            get
            {
                if (GlobalApp._PickingTypeList == null)
                {

                    GlobalApp._PickingTypeList = new ACValueItemList("PickingTypeIndex");
                    GlobalApp._PickingTypeList.AddEntry((short)PickingType.Receipt, "en{'Receipt'}de{'Eingang'}");
                    GlobalApp._PickingTypeList.AddEntry((short)PickingType.Issue, "en{'Issue'}de{'Ausgang'}");
                    GlobalApp._PickingTypeList.AddEntry((short)PickingType.Production, "en{'Production'}de{'Produktion'}");
                    GlobalApp._PickingTypeList.AddEntry((short)PickingType.ReceiptVehicle, "en{'Receipt with vehicle'}de{'Eingang mit Fahrzeug'}");
                    GlobalApp._PickingTypeList.AddEntry((short)PickingType.IssueVehicle, "en{'Issue with vehicle'}de{'Ausgang mit Fahrzeug'}");
                }
                return GlobalApp._PickingTypeList;
            }
        }
#endif
        #endregion

        #region PickingState
        /// <summary>
        /// Enum PickingState
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'Picking state'}de{'Kommissionsstatus'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum PickingState : short
        {
            New = 0,
            InProcess = 1,
            Finished = 2,
            Cancelled = 3,
        }

#if NETFRAMEWORK
        static ACValueItemList _PickingStateList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList PickingStateList
        {
            get
            {
                if (GlobalApp._PickingStateList == null)
                {

                    GlobalApp._PickingStateList = new ACValueItemList("PickingStateIndex");
                    GlobalApp._PickingStateList.AddEntry((short)PickingState.New, "en{'New'}de{'Neu'}");
                    GlobalApp._PickingStateList.AddEntry((short)PickingState.InProcess, "en{'In process'}de{'In Bearbeitung'}");
                    GlobalApp._PickingStateList.AddEntry((short)PickingState.Finished, "en{'Finshed'}de{'Fertiggestellt'}");
                    GlobalApp._PickingStateList.AddEntry((short)PickingState.Cancelled, "en{'Cancelled'}de{'Storniert'}");
                }
                return GlobalApp._PickingStateList;
            }
        }
#endif
        #endregion

        #region BatchPlanMode
        /// <summary>
        /// Enum BatchPlanMode
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioAutomation, "en{'Batch planning mode'}de{'Batch Planmodus'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum BatchPlanMode : short
        {
            UseFromTo = 0,
            UseBatchCount = 1,
            UseTotalSize = 2,
        }

#if NETFRAMEWORK
        static ACValueItemList _BatchPlanModeList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList BatchPlanModeList
        {
            get
            {
                if (GlobalApp._BatchPlanModeList == null)
                {

                    GlobalApp._BatchPlanModeList = new ACValueItemList("BatchPlanModeIndex");
                    GlobalApp._BatchPlanModeList.AddEntry((short)BatchPlanMode.UseFromTo, "en{'Use from/to values'}de{'Nach Von/Bis-Batch-Nr.'}");
                    GlobalApp._BatchPlanModeList.AddEntry((short)BatchPlanMode.UseBatchCount, "en{'Use target batch count'}de{'Nach Soll-Batchzahl'}");
                    GlobalApp._BatchPlanModeList.AddEntry((short)BatchPlanMode.UseTotalSize, "en{'Use total size'}de{'Nach Gesamtgröße'}");
                }
                return GlobalApp._BatchPlanModeList;
            }
        }
#endif
        #endregion

        #region BatchPlanState
        /// <summary>
        /// Enum BatchPlanState
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'Batch plannning state'}de{'Batchplanungsstatus'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum BatchPlanState : short
        {
            Created = 0,
            ReadyToStart = 1,
            AutoStart = 2,
            InProcess = 3,
            Paused = 4,
            Completed = 5,
            Cancelled = 6,
        }

#if NETFRAMEWORK
        static ACValueItemList _BatchPlanStateList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList BatchPlanStateList
        {
            get
            {
                if (GlobalApp._BatchPlanStateList == null)
                {

                    GlobalApp._BatchPlanStateList = new ACValueItemList("BatchPlanStateIndex");
                    GlobalApp._BatchPlanStateList.AddEntry((short)BatchPlanState.Created, "en{'New'}de{'Neu'}");
                    GlobalApp._BatchPlanStateList.AddEntry((short)BatchPlanState.ReadyToStart, "en{'Ready to start'}de{'Startbereit'}");
                    GlobalApp._BatchPlanStateList.AddEntry((short)BatchPlanState.AutoStart, "en{'Automatic start'}de{'Automatik start'}");
                    GlobalApp._BatchPlanStateList.AddEntry((short)BatchPlanState.InProcess, "en{'In process'}de{'Prozess aktiv'}");
                    GlobalApp._BatchPlanStateList.AddEntry((short)BatchPlanState.Completed, "en{'Completed'}de{'Fertiggestellt'}");
                    GlobalApp._BatchPlanStateList.AddEntry((short)BatchPlanState.Cancelled, "en{'Cancelled'}de{'Deaktiviert/Abgebrochen'}");
                    GlobalApp._BatchPlanStateList.AddEntry((short)BatchPlanState.Paused, "en{'Paused'}de{'Pausiert'}");
                }
                return GlobalApp._BatchPlanStateList;
            }
        }
#endif
        #endregion

        #region BatchPlanStartMode

#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'BatchPlanStartModes'}de{'BatchPlanStartModes'}", Global.ACKinds.TACEnum)]
#else
       [DataContract]
#endif
        public enum BatchPlanStartModeEnum : short
        {
            Off = 0,
            AutoSequential = 1,
            AutoTime = 2,
            AutoTimeAndSequential = 3,
            SemiAutomatic = 4
        }

#if NETFRAMEWORK
        static ACValueItemList _BatchPlanStartModeEnumList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList BatchPlanStartModeEnumList
        {
            get
            {
                if (GlobalApp._BatchPlanStartModeEnumList == null)
                {
                    GlobalApp._BatchPlanStartModeEnumList = new ACValueItemList("BatchPlanStartModeEnumList");
                    GlobalApp._BatchPlanStartModeEnumList.AddEntry((short)BatchPlanStartModeEnum.Off, "en{'Off'}de{'Aus'}");
                    GlobalApp._BatchPlanStartModeEnumList.AddEntry((short)BatchPlanStartModeEnum.AutoSequential, "en{'Sequential'}de{'Sequenziell'}");
                    GlobalApp._BatchPlanStartModeEnumList.AddEntry((short)BatchPlanStartModeEnum.AutoTime, "en{'Scheduling'}de{'Nach Zeitplan'}");
                    GlobalApp._BatchPlanStartModeEnumList.AddEntry((short)BatchPlanStartModeEnum.AutoTimeAndSequential, "en{'Scheduling and sequential'}de{'Nach Zeitplan und Sequenziell'}}");
                    GlobalApp._BatchPlanStartModeEnumList.AddEntry((short)BatchPlanStartModeEnum.SemiAutomatic, "en{'Partial quantity'}de{'Nach Teilmenge'}}");
                }
                return GlobalApp._BatchPlanStartModeEnumList;
            }
        }
#endif
        #endregion

        #region ReservationState
        /// <summary>
        /// Enum ReservationState
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'Batch plannning state'}de{'Reservierungssstatus'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum ReservationState : short
        {
            New = 0,
            Active = 1,
            Finished = 2,
        }

#if NETFRAMEWORK
        static ACValueItemList _ReservationStateList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList ReservationStateList
        {
            get
            {
                if (GlobalApp._ReservationStateList == null)
                {

                    GlobalApp._ReservationStateList = new ACValueItemList("ReservationStateIndex");
                    GlobalApp._ReservationStateList.AddEntry((short)ReservationState.New, "en{'New'}de{'Neu'}");
                    GlobalApp._ReservationStateList.AddEntry((short)ReservationState.Active, "en{'Active'}de{'Aktiv'}");
                    GlobalApp._ReservationStateList.AddEntry((short)ReservationState.Finished, "en{'Finshed'}de{'Fertiggestellt'}");
                }
                return GlobalApp._ReservationStateList;
            }
        }
#endif
        #endregion

        #region ReservationState
        /// <summary>
        /// Enum ReservationState
        /// </summary>
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'Type of lab order'}de{'Laborauftragstyp'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum LabOrderType : short
        {
            Template = 0,
            Order = 1,
        }

#if NETFRAMEWORK
        static ACValueItemList _LabOrderTypeList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList LabOrderTypeList
        {
            get
            {
                if (GlobalApp._LabOrderTypeList == null)
                {

                    GlobalApp._LabOrderTypeList = new ACValueItemList("LabOrderTypeIndex");
                    GlobalApp._LabOrderTypeList.AddEntry((short)LabOrderType.Template, "en{'Template'}de{'Vorlage'}");
                    GlobalApp._LabOrderTypeList.AddEntry((short)LabOrderType.Order, "en{'Order'}de{'Auftrag'}");
                }
                return GlobalApp._LabOrderTypeList;
            }
        }
#endif
        #endregion

        #region TrackingAndTracing
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'TrackingAndTracingSearchModel'}de{'TrackingAndTracingSearchModel'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum TrackingAndTracingSearchModel
        {
            Backward = 1,
            Forward = 2
        }


#if NETFRAMEWORK
        static ACValueItemList _TrackAndTracingSearchModelList = null;
        /// <summary>
        /// Gibt eine Liste aller Enums zurück, damit die Gui
        /// damit arbeiten kann.
        /// </summary>
        static public ACValueItemList TrackingAndTracingSearchModelSearchModelList
        {
            get
            {
                if (GlobalApp._TrackAndTracingSearchModelList == null)
                {
                    GlobalApp._TrackAndTracingSearchModelList = new ACValueItemList("TrackAndTracingSearchModel");
                    GlobalApp._TrackAndTracingSearchModelList.AddEntry((short)TrackingAndTracingSearchModel.Backward, "en{'Backward'}de{'Zurück '}");
                    GlobalApp._TrackAndTracingSearchModelList.AddEntry((short)TrackingAndTracingSearchModel.Forward, "en{'Forward'}de{'Vorwärts'}");
                }
                return GlobalApp._TrackAndTracingSearchModelList;
            }
        }
#endif

        #endregion

        #region LabOrder
#if NETFRAMEWORK
        [ACSerializeableInfo]
        [ACClassInfo(Const.PackName_VarioSystem, "en{'From Position Type'}de{'Aus Positionsart'}", Global.ACKinds.TACEnum)]
#else
        [DataContract]
#endif

        public enum LabOrderMaterialState : short
        {
            InOrderPos = 0,
            OutOrderPos = 1,
            PartslistPos = 2,
            LotCharge = 3,
        }

#if NETFRAMEWORK
        static ACValueItemList _LabOrderMaterialStateList = null;
        public static ACValueItemList LabOrderMaterialStateList
        {
            get
            {
                if (GlobalApp._LabOrderMaterialStateList == null)
                {
                    GlobalApp._LabOrderMaterialStateList = new ACValueItemList("LabOrderMaterialState");
                    GlobalApp._LabOrderMaterialStateList.AddEntry(LabOrderMaterialState.InOrderPos.ToString(), "en{'Purchase Order Pos.'}de{'Bestellposition'}");
                    GlobalApp._LabOrderMaterialStateList.AddEntry(LabOrderMaterialState.OutOrderPos.ToString(), "en{'Sales Order Pos.'}de{'Auftragsposition'}");
                    GlobalApp._LabOrderMaterialStateList.AddEntry(LabOrderMaterialState.PartslistPos.ToString(), "en{'Bill of Materials Position'}de{'Stücklistenposition'}");
                    GlobalApp._LabOrderMaterialStateList.AddEntry(LabOrderMaterialState.LotCharge.ToString(), "en{'Lot/Charge'}de{'Los/Charge'}");
                }
                return GlobalApp._LabOrderMaterialStateList;
            }
        }
#endif

        #endregion

    }
}
