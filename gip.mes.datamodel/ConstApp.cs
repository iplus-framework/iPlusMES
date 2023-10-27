namespace gip.mes.datamodel
{
    /// <summary>
    /// Definition der grundsätzlichen Ansichtsarten, welche das Standardframework unterstützt
    /// 
    /// Use capital letters for the Englisch localization.
    /// </summary>

    public static partial class ConstApp
    {
        #region Entity Strings
        public const string ESBalancingMode = "en{'Balancing Mode'}de{'Bilanzierungsmodus'}";
        public const string ESBookingNotAvailableMode = "en{'Not Available Mode'}de{'Modus bei Nichverfügbarkeit'}";
        public const string ESCostCenter = "en{'Cost Center'}de{'Kostenstelle'}";
        public const string ESCountry = "en{'Country'}de{'Land'}";
        public const string ESCountryLand = "en{'Federal State'}de{'Bundesland'}";
        public const string ESCountrySalesTax = "en{'VAT'}de{'Mehrwertsteuer'}";
        public const string ESCountrySalesTaxMaterial = "en{'Tax Material'}de{'MwSt Material'}";
        public const string ESCountrySalesTaxMDMaterialGroup = "en{'Tax Material Group'}de{'MwSt Materialgruppe'}";

        public const string ESCurrency = "en{'Currency'}de{'Währung'}";
        public const string ESCurrencyExchange = "en{'Exchange Rate'}de{'Wechselkurs'}";
        public const string ESDelivNoteState = "en{'Delivery Note Status'}de{'Lieferscheinstatus'}";
        public const string ESDelivPosLoadState = "en{'Loading Status'}de{'Beladungszustand'}";
        public const string ESDelivPosState = "en{'Delivery Status'}de{'Lieferstatus'}";
        public const string ESDelivType = "en{'Delivery Type'}de{'Lieferart'}";
        public const string ESDemandOrderState = "en{'Demand Order State'}de{'Bestellanforderung-Status'}";
        public const string ESFacilityInventoryPosState = "en{'Inventory Position Status'}de{'Lagerbestandsstatus'}";
        public const string ESFacilityInventoryState = "en{'Inventory Status'}de{'Inventurstatus'}";
        public const string ESFacilityManagementType = "en{'Stock Management Type'}de{'Lagerführungsart'}";
        public const string ESFacilityType = "en{'Storage Type'}de{'Lagerart'}";
        public const string ESFacilityVehicleType = "en{'Vehicle Type'}de{'Fahrzeugtyp'}";
        public const string ESGMPAdditives = "en{'GMP-Additives'}de{'GMP-Zusatzstoffe'}";
        public const string ESGMPMaterialGroup = "en{'GMP-Material Group'}de{'GMP-Materialgruppe'}";
        public const string ESGMPMaterialGroupPos = "en{'GMP-Material Group Pos.'}de{'GMP-Materialgruppenpos.'}";
        public const string ESInOrderPosState = "en{'Purchase Order Pos. Status'}de{'Bestellposition-Status'}";
        public const string ESInOrderState = "en{'Purchase Order Status'}de{'Bestellstatus'}";
        public const string ESInOrderType = "en{'Purchase Order Type'}de{'Bestellart'}";
        public const string ESInRequestState = "en{'Request Status'}de{'Anfragestatus'}";
        public const string ESInventoryManagementType = "en{'Inventory Management Type'}de{'Bestandsführungsart'}";
        public const string ESLabOrderPosState = "en{'Laboratory Order Pos. Status'}de{'Laborauftrag Positionsstatus'}";
        public const string ESLabOrderState = "en{'Laboratory Order Status'}de{'Laborauftragsstatus'}";
        public const string ESLabTag = "en{'Laboratory Tag'}de{'Laborkennzeichen'}";
        public const string ESMaintMode = "en{'Maintenance Mode'}de{'Wartungsmodus'}";
        public const string ESMaintOrderPropertyState = "en{'Maint. Order Prop. Status'}de{'Wartungsauftrag Eigenschaft-Status'}";
        public const string ESMaintOrderState = "en{'Maintenance Order Status'}de{'Wartungsauftrag-Status'}";
        public const string ESMaintTaskState = "en{'Maintenance Task Status'}de{'Wartungsmaßnahme-Status'}";
        public const string ESMaterialGroup = "en{'Material Group'}de{'Material Gruppe'}";
        public const string ESMaterialType = "en{'Material Type'}de{'Materialart'}";
        public const string ESMovementReason = "en{'Movement Reason'}de{'Bewegungsgrund'}";
        public const string ESOutOfferState = "en{'Offering Status'}de{'Angebotsstatus'}";
        public const string ESOutOrderPlanState = "en{'Sales Order Plan Status'}de{'Verkaufsauftragsplan Status'}";
        public const string ESOutOrderPosState = "en{'Sales Order Pos. Status'}de{'Verkaufsauftrag Positionstatus'}";
        public const string ESInvoiceType = "en{'Invoice Type'}de{'Rechnungsart'}";
        public const string ESInvoiceState = "en{'Invoice Status'}de{'Rechnungsstatus'}";
        public const string ESInvoice = "en{'Invoice'}de{'Rechnung'}";
        public const string ESInvoicePos = "en{'Invoice linie'}de{'Rechnungposition'}";
        public const string ESOutOrderState = "en{'Sales Order Status'}de{'Verkaufsauftragstatus'}";
        public const string ESOutOrderType = "en{'Sales Order Type'}de{'Verkaufsauftragsart'}";
        public const string ESProcessErrorAction = "en{'Process Error Action'}de{'Prozessfehleraktion'}";
        public const string ESProdOrderPartslistPosState = "en{'Prod.Order BOM Pos. Status'}de{'Prod.auftrag Stückl. Pos.-Status'}";
        public const string ESMDBatchPlanMode = "en{'Batch planning mode'}de{'Batch Planmodus'}";
        public const string ESMDBatchPlanStartMode = "en{'Batch plan mode'}de{'Batch plan mode'}";
        public const string ESProdOrderState = "en{'Production Order Status'}de{'Produktionsauftragsstatus'}";
        public const string ESRatingComplaintType = "en{'Complaint Type'}de{'Reklamationsart'}";
        public const string ESReleaseState = "en{'Release Status'}de{'Freigabestatus'}";
        public const string ESReservationMode = "en{'Reservation Mode'}de{'Reservierungsmodus'}";
        public const string ESTermOfPayment = "en{'Terms of Payment'}de{'Zahlungsbedingungen'}";
        public const string ESTimeRange = "en{'Shift'}de{'Schicht'}";
        public const string ESToleranceState = "en{'Tolerance Status'}de{'Toleranzstatus'}";
        public const string ESTour = "en{'Tour'}de{'Tour'}";
        public const string ESTourplanPosState = "en{'Tour Plan Pos. Status'}de{'Tourplan Positionstatus'}";
        public const string ESTourplanState = "en{'Tour Plan Status'}de{'Tourplanstatus'}";
        public const string ESTransportMode = "en{'Transport Mode'}de{'Transportart'}";
        public const string ESUnit = "en{'Unit of Measurement'}de{'Maßeinheiten'}";
        public const string ESUnitConversion = "en{'Unit Conversion'}de{'Einheitenumrechnung'}";
        public const string ESVisitorCard = "en{'Visitor Card'}de{'Besucherausweis'}";
        public const string ESVisitorCardState = "en{'Visitor Card Status'}de{'Besucherausweisstatus'}";
        public const string ESVisitorVoucherState = "en{'Visitor Voucher Status'}de{'Besucherbelegstatus'}";
        public const string ESZeroStockState = "en{'Zero Stock Status'}de{'Nullbestand-Status'}";
        public const string ESPriceList = "en{'Price list'}de{'Preisliste'}";
        public const string ESPriceListMaterial = "en{'Price list material'}de{'Preislistematerial'}";
        public const string ESUserSettings = "en{'User settings'}de{'Benutzereinstellungen'}";

        // Issuer
        public const string IssuerCompanyAddress = "IssuerCompanyAddress";
        public const string IssuerCompanyPerson = "IssuerCompanyPerson";

        public const string IssuerCompanyAddress_ACCaption = "en{'Issuer Address'}de{'Emittentenadresse'}";
        public const string IssuerCompanyPerson_ACCaption = "en{'Issuer Person'}de{'Emittentenperson'}";

        public const string MDSchedulingGroup = "en{'Scheduling Group'}de{'Planungsgruppe'}";

        #endregion

        #region Labels
        // Quantity
        public const string InwardQuantity = "en{'Inward Quantity'}de{'Zugangsmenge'}";
        public const string InwardTargetQuantity = "en{'Target Inward Qty.'}de{'Zugangsmenge Soll'}";
        public const string ReservedInwardQuantity = "en{'Reserved Inward Qty.'}de{'Reservierte Zugangsmenge'}";
        public const string ReservedQuantity = "en{'Reserved Qty.'}de{'Reservierte Menge'}";

        public const string OutwardQuantity = "en{'Outward Quantity'}de{'Abgangsmenge'}";
        public const string OutwardTargetQuantity = "en{'Target Outward Qty.'}de{'Abgangsmenge Soll'}";
        public const string ReservedOutwardQuantity = "en{'Reserved Outward Qty.'}de{'Reservierte Abgangsmenge'}";

        public const string TargetQuantity = "en{'Target Quantity'}de{'Sollmenge'}";
        public const string TargetQuantityUOM = "en{'Target Quantity (UOM)'}de{'Sollmenge (BME)'}";
        public const string ActualQuantity = "en{'Actual Quantity'}de{'Istmenge'}";
        public const string ActualQuantityUOM = "en{'Actual Quantity (UOM)'}de{'Istmenge (BME)'}";
        public const string RejectQuantityUOM = "en{'Reject Quantity (UOM)'}de{'Menge ablehnen (BME)'}";
        public const string StockQuantity = "en{'Stock Quantity'}de{'Lagermenge'}";
        public const string StockQuantityAmb = "en{'Stock Quantity Ambient'}de{'Lagermenge ambient'}";
        public const string StockWeight = "en{'Stock Weight'}de{'Lagergewicht'}";
        public const string MinStockQuantity = "en{'Min.Stock Quantity'}de{'Min.Lagermenge'}";
        public const string MaxStockQuantity = "en{'Max.Stock Quantity'}de{'Max.Lagermenge'}";
        public const string OptStockQuantity = "en{'Opt.Stock Quantity'}de{'Opt.Lagermenge'}";

        public const string TotalQuantity = "en{'Total Quantity'}de{'Gesamtmenge'}";
        public const string BlockedQuantity = "en{'Blocked Quantity'}de{'Gesperrte Menge'}";
        public const string AbsoluteBlockedQuantity = "en{'Absolute Blocked Quantity'}de{'Absolut gesperrte Menge'}";
        public const string FreeQuantity = "en{'Free Quantity'}de{'Freie Menge'}";
        public const string NewPlannedStock = "en{'New Planned Stock'}de{'Neuer Planbestand'}";

        public const string DayInward = "en{'Day Inward Qty.'}de{'Tageszugang'}";
        public const string DayInwardAmb = "en{'Day Inward Qty Ambient'}de{'Tageszugang ambient'}";
        public const string DayTargetInward = "en{'Day Inward Target Qty.'}de{'Tageszugang Soll'}";
        public const string DayTargetInwardAmb = "en{'Day Inward Target Qty. Ambient'}de{'Tageszugang Soll ambient'}";
        public const string DayOutward = "en{'Day Outward Qty.'}de{'Tagesabgang'}";
        public const string DayOutwardAmb = "en{'Day Outward Qty. Ambient'}de{'Tagesabgang ambient'}";
        public const string DayTargetOutward = "en{'Day Outward Target Qty.'}de{'Tagesabgang Soll'}";
        public const string DayTargetOutwardAmb = "en{'Day Outward Target Qty. Ambient'}de{'Tagesabgang Soll ambient'}";
        public const string DayAdjustment = "en{'Day Adjustment'}de{'Tageskorrektur'}";
        public const string DayAdjustmentAmb = "en{'Day Adjustment Ambient'}de{'Tageskorrektur ambient'}";
        public const string DayBalanceDate = "en{'Day Balance Date'}de{'Tagesbilanz Datum'}";
        public const string DayLastOutward = "en{'Last Day Outward Qty.'}de{'Letzter Tagesabgang'}";
        public const string DayLastOutwardAmb = "en{'Last Day Outward Qty. Ambient'}de{'Letzter Tagesabgang ambient'}";
        public const string DayLastStock = "en{'Last Daily Stock'}de{'Letzter Tagesbestand'}";
        public const string DayLastStockAmb = "en{'Last Daily Stock Ambient'}de{'Letzter Tagesbestand ambient'}";

        public const string WeekInward = "en{'Week Inward Qty.'}de{'Wochenzugang'}";
        public const string WeekInwardAmb = "en{'Week Inward Qty. Ambient'}de{'Wochenzugang ambient'}";
        public const string WeekTargetInward = "en{'Week Inward Target Qty.'}de{'Wochenzugang Soll'}";
        public const string WeekTargetInwardAmb = "en{'Week Inward Target Qty. Ambient'}de{'Wochenzugang Soll ambient'}";
        public const string WeekOutward = "en{'Week Outward Qty.'}de{'Wochenabgang'}";
        public const string WeekOutwardAmb = "en{'Week Outward Qty. Ambient'}de{'Wochenabgang ambient'}";
        public const string WeekTargetOutward = "en{'Week Outward Target Qty.'}de{'Wochenabgang Soll'}";
        public const string WeekTargetOutwardAmb = "en{'Week Outward Target Qty. Ambient'}de{'Wochenabgang Soll ambient'}";
        public const string WeekAdjustment = "en{'Week Adjustment'}de{'Wochenkorrektur'}";
        public const string WeekAdjustmentAmb = "en{'Week Adjustment Ambient'}de{'Wochenkorrektur ambient'}";
        public const string WeekBalanceDate = "en{'Weekly Balance Date'}de{'Wochenbilanz Datum'}";

        public const string MonthInward = "en{'Month Inward Qty.'}de{'Monatszugang'}";
        public const string MonthInwardAmb = "en{'Month Inward Qty. Ambient'}de{'Monatszugang ambient'}";
        public const string MonthTargetInward = "en{'Month Inward Target Qty.'}de{'Monatszugang Soll'}";
        public const string MonthTargetInwardAmb = "en{'Month Inward Target Qty.Ambient'}de{'Monatszugang Soll ambient'}";
        public const string MonthOutward = "en{'Month Outward Qty.'}de{'Monatsabgang'}";
        public const string MonthOutwardAmb = "en{'Month Outward Qty. Ambient'}de{'Monatsabgang ambient'}";
        public const string MonthTargetOutward = "en{'Month Outward Target Qty.'}de{'Monatsabgang Soll'}";
        public const string MonthTargetOutwardAmb = "en{'Month Outward Target Qty. Ambient'}de{'Monatsabgang Soll ambient'}";
        public const string MonthActStock = "en{'Current Monthly Stock'}de{'Aktueller Monatsbestand'}";
        public const string MonthActStockAmb = "en{'Current Monthly Stock Ambient'}de{'Aktueller Monatsbestand ambient'}";
        public const string MonthAdjustment = "en{'Month Adjustment'}de{'Monatskorrektur'}";
        public const string MonthAdjustmentAmb = "en{'Month Adjustment Ambient'}de{'Monatskorrektur ambient'}";
        public const string MonthAverageStock = "en{'Month Average Stock'}de{'Monatlicher Durchschnittsbestand'}";
        public const string MonthAverageStockAmb = "en{'Month Average Stock Ambient'}de{'Monatlicher Durchschnittsbestand ambient'}";
        public const string MonthBalanceDate = "en{'Month Balance Date'}de{'Monatsbilanz Datum'}";
        public const string MonthLastOutward = "en{'Last Month Outward Qty.'}de{'Letzter Monatsabgang'}";
        public const string MonthLastOutwardAmb = "en{'Last Month Outward Qty. Ambient'}de{'Letzter Monatsabgang ambient'}";
        public const string MonthLastStock = "en{'Last Monthly Stock'}de{'Letzter Monatsbestand'}";
        public const string MonthLastStockAmb = "en{'Last Monthly Stock Ambient'}de{'Letzter Monatsbestand ambient'}";

        public const string YearInward = "en{'Year Inward Qty.'}de{'Jahreszugang'}";
        public const string YearInwardAmb = "en{'Year Inward Qty. Ambient'}de{'Jahreszugang ambient'}";
        public const string YearTargetInward = "en{'Year Inward Target Qty.'}de{'Jahreszugang Soll'}";
        public const string YearTargetInwardAmb = "en{'Year Inward Target Qty. Ambient'}de{'Jahreszugang Soll ambient'}";
        public const string YearOutward = "en{'Year Outward Qty.'}de{'Jahresabgang'}";
        public const string YearOutwardAmb = "en{'Year Outward Qty. Ambient'}de{'Jahresabgang ambient'}";
        public const string YearTargetOutward = "en{'Year Outward Target Qty.'}de{'Jahresabgang Soll'}";
        public const string YearTargetOutwardAmb = "en{'Year Outward Target Qty. Ambient'}de{'Jahresabgang Soll ambient'}";
        public const string YearAdjustment = "en{'Year Adjustment'}de{'Jahreskorrektur'}";
        public const string YearAdjustmentAmb = "en{'Year Adjustment Ambient'}de{'Jahreskorrektur ambient'}";
        public const string YearBalanceDate = "en{'Year Balance Date'}de{'Jahresbilanz Datum'}";

        public const string MDUnit = "en{'Unit of Measure'}de{'Maßeinheit'}";
        public const string TechnicalSymbol = "en{'Symbol Tech.'}de{'Zeichen tech.'}";

        // Name, Description
        public const string Name = "en{'Name'}de{'Bezeichnung'}";
        public const string Number = "en{'No.'}de{'Nr.'}";

        public const string Facility = "en{'Storage Bin'}de{'Lagerplatz'}";
        public const string FacilityMaterial = "en{'Storage Bin Material'}de{'Lagerplatz Material'}";
        public const string FacilityMDSchedulingGroup = "en{'Storage Bin Scheduling Group'}de{'Lagerplatz Scheduling Group'}";
        public const string FacilityNo = "en{'Storage Bin No.'}de{'Lagerplatznummer'}";
        public const string FacilityStock = "en{'Stock'}de{'Lagerbestand'}";
        public const string FacilityReservation = "en{'Reservation'}de{'Reservierung'}";

        public const string Material = "en{'Material'}de{'Material'}";
        public const string MaterialNo = "en{'Material No.'}de{'Material-Nr.'}";
        public const string MaterialStock = "en{'Material Stock'}de{'Materialbestand'}";
        public const string ExternLotNo = "en{'Extern Lot No.'}de{'Externe Losnr.'}";
        public const string ExternLotNo2 = "en{'Extern Lot No.2'}de{'Externe Losnr.2'}";

        public const string Lot = "en{'Lot'}de{'Los'}";
        public const string LotNo = "en{'Lot No.'}de{'Los-Nr.'}";
        public const string SplitNo = "en{'Split No.'}de{'Splitnr'}";

        public const string Company = "en{'Company'}de{'Unternehmen'}";

        public const string BOM = "en{'Bill of Materials'}de{'Stückliste'}";

        public const string FinalIntermediate = "en{'Final intermediate'}de{'Finales Zwischenprodukt'}";

        public const string OperationLog = "en{'Operation log'}de{'Betriebsprotokoll'}";

        // Date
        public const string ExpirationDate = "en{'Expiration Date'}de{'Ablaufdatum'}";
        public const string FillingDate = "en{'Fill Date'}de{'Fülldatum'}";
        public const string ProductionDate = "en{'Production Date'}de{'Produktionsdatum'}";
        public const string StorageLife = "en{'Storage Life'}de{'Haltbarkeit'}";
        public const string StorageDate = "en{'Storage Date'}de{'Einlagerungsdatum'}";
        public const string ValidFromDate = "en{'Valid From'}de{'Gültig von'}";
        public const string ValidToDate = "en{'Valid To'}de{'Gültig bis'}";
        public const string TargetDeliveryDate = "en{'Delivery Date'}de{'Lieferdatum'}";
        public const string TargetDeliveryMaxDate = "en{'Delivery latest by'}de{'Lieferung bis spätestens'}";

        // Cost
        public const string CostFix = "en{'Fixed Costs'}de{'Fixkosten'}";
        public const string CostLoss = "en{'Loss'}de{'Verlust'}";
        public const string CostVar = "en{'Variable Costs'}de{'Variable Kosten'}";
        public const string CostMat = "en{'Material Cost'}de{'Materialkosten'}";
        public const string CostPack = "en{'Packaging Costs'}de{'Verpackungskosten'}";
        public const string CostGeneral = "en{'Total Cost'}de{'Gesamtkosten'}";
        public const string PriceNet = "en{'Net Price'}de{'Nettopreis'}";
        public const string PriceGross = "en{'Gross Price'}de{'Bruttopreis'}";
        public const string PriceNetTotal = "en{'Net Total'}de{'Netto Gesamt'}";
        public const string PriceGrossTotal = "en{'Gross total'}de{'Brutto Gesamt'}";
        public const string VATPerUnit = "en{'VAT per unit'}de{'MwSt. pro Einheit'}";
        public const string VATTotal = "en{'VAT total'}de{'MwSt. gesamt'}";
        public const string ForeignPriceNet = "en{'Net Price in f. currency'}de{'Nettopreis in Fremdwährung'}";
        public const string ForeignPriceGross = "en{'Gross Price in f. currency'}de{'Bruttopreis in Fremdwährung'}";
        public const string ForeignTotalPrice = "en{'Net Total in f. currency'}de{'Netto Gesamt in Fremdwährung'}";
        public const string ForeignPriceGrossTotal = "en{'Gross Total in f. currency'}de{'Gross Gesamt in Fremdwährung'}";

        // Addition
        public const string Addition1Percent = "en{'Percent 1'}de{'Prozent 1'}";
        public const string Addition2Percent = "en{'Percent 2'}de{'Prozent 2'}";
        public const string Addition3Percent = "en{'Percent 3'}de{'Prozent 3'}";
        public const string Addition4Percent = "en{'Percent 4'}de{'Prozent 4'}";
        public const string Addition5Percent = "en{'Percent 5'}de{'Prozent 5'}";

        public const string Addition1Material = "en{'Material 1'}de{'Material 1'}";
        public const string Addition2Material = "en{'Material 2'}de{'Material 2'}";
        public const string Addition3Material = "en{'Material 3'}de{'Material 3'}";
        public const string Addition4Material = "en{'Material 4'}de{'Material 4'}";
        public const string Addition5Material = "en{'Material 5'}de{'Material 5'}";

        public const string MaterialName1 = "en{'Material Desc. 1'}de{'Materialbez. 1'}";
        public const string MaterialName2 = "en{'Material Desc. 2'}de{'Materialbez. 2'}";
        public const string MaterialName3 = "en{'Material Desc. 3'}de{'Materialbez. 3'}";

        // Orders
        public const string ProdOrder = "en{'Production Order'}de{'Produktionsauftrag'}";
        public const string OrderNo = "en{'Order No.'}de{'Order Nr.'}";
        public const string ProdOrderProgramNo = "en{'Order Number'}de{'Auftragsnummer'}";
        public const string ProdOrderPartslist = "en{'Prod. Order BOM'}de{'Prod.auftrag Stückliste'}";
        public const string ProductionStart = "en{'Production Start'}de{'Produktionsstart'}";
        public const string ProductionEnd = "en{'Production End'}de{'Produktionsende'}";
        public const string DepartmentUserName = "en{'Ended by User'}de{'Beendet von Bediener'}";

        // Batches
        public const string BatchNo = "en{'Batch No.'}de{'Batchnummer'}";

        // Other
        public const string Comment = "en{'Comment'}de{'Bemerkung'}";
        public const string Comment2 = "en{'Comment2'}de{'Bemerkung2'}";
        public const string XMLComment = "en{'Long text'}de{'Langtext'}";
        public const string SetCompleted = "en{'Set to completed'}de{'Auf Fertiggestellt setzen'}";
        public const string NotAvailable = "en{'Not Available'}de{'Nicht verfügbar'}";
        public const string Lock = "en{'Locked'}de{'Gesperrt'}";
        public const string IsActive = "en{'Active'}de{'Aktiv'}";
        public const string ToleranceMinus = "en{'Tolerance Minus'}de{'Toleranz minus'}";
        public const string TolerancePlus = "en{'Tolerance Plus'}de{'Toleranz plus'}";
        public const string TermsOfPayment = "en{'Terms of Payment'}de{'Zahlungsbedingungen'}";
        public const string BillingCompanyAddress = "en{'Billing Address'}de{'Rechnungsadresse'}";
        public const string DeliveryCompanyAddress = "en{'Delivery Address'}de{'Lieferadresse'}";
        public const string PlanningMR = "en{'Template schedules'}de{'Vorlagepläne'}";

        public const string Search = "en{'Search'}de{'Suchen'}";
        public const string Select = "en{'Select'}de{'Auswahl'}";
        public const string SelectAll = "en{'Select all'}de{'Alles auswählen'}";
        public const string PickingType = "en{'Picking type'}de{'Kommissionierung Typ'}";
        public const string PickingNo = "en{'Picking-No.'}de{'Kommissions-Nr.'}";

        public const string Backward = "en{'Backward'}de{'Zurück '}";
        public const string Forward = "en{'Forward'}de{'Vorwärts'}";


        public const string KeyOfExtSys = "KeyOfExtSys";
        public const string EntityTranslateKeyOfExtSys = "en{'Key of ext. system'}de{'Schlüssel von ext. System'}";

        public const string DeliveryNotePos = "en{'Indeliverynotepos'}de{'Eingangslieferscheinposition'}";
        #endregion


        public const string ACMethodBooking_ClassName = "ACMethodBooking";

        public const string UOM_ISOCode_g = "GRM";
        public const string UOM_ISOCode_kg = "KGM";
        public const string UOM_ISOCode_t = "TNE";

        #region BSO Dialog Names

        #region BSOFacilityLot
        public const string BSOFacilityLot_ChildName = "BSOFacilityLot_Child";
        public const string BSOFacilityLot_Dialog_ShowDialogNewLot = "ShowDialogNewLot";
        #endregion

        #region VBBSOConfigTransfer

        public const string VBBSOConfigTransfer_ChildName = "VBBSOConfigTransfer_Child";
        public const string VBBSOConfigTransfer_ChildName_LoadPartslistInfo = "LoadPartslistInfo";

        #endregion

        #region BSOTrackingAndTracing
        public const string BSOTrackingAndTracing_ChildName = "BSOTrackingAndTracing_Child";
        public const string BSOTrackingAndTracing_Name = "BSOTrackingAndTracing";
        public const string BSOTrackingAndTracing_Dialog_TrackingAndTracingOpenDlg = "TrackingAndTracingOpenDlg";
        public const string BSOTrackingAndTracing_Dialog_TrackingAndTracingDialogOk = "TrackingAndTracingDialogOk";
        #endregion

        #region BSORatingComplaint
        public const string BSORatingComplaint_ChildName = "BSORatingComplaint_Child";
        public const string BSORatingComplaint_Dialog = "OpenAsModal";
        #endregion

        #endregion

        #region OrderLotItem

        public const string TimeEntered = "en{'Time Entered'}de{'Eingabezeit'}";
        public const string MinDuration = "en{'Minimum duration'}de{'Minimum duration'}";
        public const string MaxDuration = "en{'Maximum duration'}de{'Maximum duration'}";
        public const string Duration = "en{'Duration'}de{'Dauer'}";
        public const string HintDuration = "en{'Hint Duration'}de{'Hinweisdauer'}";
        public const string ElapsedTime = "en{'Elapsed Time'}de{'Verstrichene Zeit'}";
        public const string FinishTime = "en{'Finish Time'}de{'Endzeit'}";
        public const string RestTime = "en{'Rest Time'}de{'Verbleibende Zeit'}";
        public const string OperationitemTimeStatus = "en{'Operation item status'}de{'Vorgangspositionsstatus'}";

        #endregion


    }

}
