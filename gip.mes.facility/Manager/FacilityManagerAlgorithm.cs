using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;

namespace gip.mes.facility
{
    /// <summary>
    /// Diese partielle Klasse beenhaltet die Kernalgorithmen und Methoden für
    /// verschiedene Buchungsvorfälle
    /// </summary>
    public partial class FacilityManager
    {
        /// <summary>
        /// ------------------------------------------------
        /// -------------  PSEUDO-Code ---------------------
        /// ------------------------------------------------
        /// Definiere LocalMaterial, LocalOutwardMaterial, LocalInwardMaterial und LocalOutwardFacility, LocalInwardFacility
        ///     (Die Lokalen Entitäten dient dazu, damit im nachfolgenden Programmcode damit gearbeitet werden kann
        ///     Die übergebenen Entitäten aus dem Bookingparameter dagegen,
        ///     dienen zur Unterscheidung/Erkennung wie der Buchungsalgorithmus ausgeführt werden muss)
        /// 
        /// ---------------------------------------------
        /// A: Ermittle LocalMaterial (wegen Chargenführungskennzeichen) 
        ///     und führe Booking-Parameter-Check in diesem Zuammenhang durch
        /// ---------------------------------------------
        /// Falls Material übergeben
        ///     Setze LocalMaterial mit Material
        /// Falls Material nicht übergeben
        ///     Falls Facility(Lagerplatz) übergeben, checke ob Facility ein Behältnis (Silo, Tank) ist
        ///         Falls Facility(Lagerplatz) mit Material belegt ist
        ///         Hole LocalMaterial aus Facility(Lagerplatz)
        ///     Falls LocalMaterial nicht gefunden und FacilityCharge übergeben
        ///         Hole LocalMaterial aus FacilityCharge
        ///     Falls LocalMaterial nicht gefunden und FacilityLot übergeben
        ///         Hole LocalMaterial aus FacilityLot
        /// Falls Material nicht gefunden
        ///     Breche ab mit Fehler
        ///     
        /// 
        /// ---------------------------------------------
        /// B: Ermittle LocalFacility wenn möglich
        ///     und führe Booking-Parameter-Check in diesem Zuammenhang durch
        /// ---------------------------------------------
        /// Falls Facility übergeben
        ///     Setze LocalFacility mit Facility
        /// 
        /// 
        /// ------------------------------------------------------------
        /// C: Falls Material [LocalMaterial] NICHT CHARGENGEFÜHRT ist
        ///     führe Booking-Parameter-Check in diesem Zuammenhang durch
        /// ------------------------------------------------------------
        ///     Pseudo-code:
        ///     Check: [FacilityCharge], [FacilityLot] darf nicht übergeben sein
        ///         Breche ab mit Fehler
        /// 
        ///     Es gibt folgende Buchungsfälle:
        ///     [Übergebene Entitäten]:
        ///     ..................................
        ///     [FacilityLocation] oder [Facility]
        ///
        ///     Falls Facility(Lagerplatz) nicht übergeben
        ///         Falls FacilityLocation(Lagerort) nicht übergeben
        ///             Breche ab mit Fehlermeldung
        ///         Sonst FacilityLocation(Lagerort) übergeben
        ///             Gibt es FacilityCharges (Enlagerungen) von diesem Material in diesem Lagerort
        ///                 Dann Stackbuchung
        ///             Sonst
        ///                 Hole Standard-Einlagerplatz aus FacilityLocation(Lagerort)
        ///                     Falls Standard-Einlagerplatz nicht existiert
        ///                     breche ab mit Fehler
        ///                 Setze LocalFacility mit Standard-Einlagerplatz
        ///     Sonst Falls Facility(Lagerplatz) übergeben
        ///         Falls FacilityLocation übergeben
        ///             FacilityLocation mit FacilityLocation aus Facility nicht übereinstimmt
        ///                 Breche ab mit Fehler
        ///         Setze LocalFacility mit Facility(Lagerplatz)
        ///   
        /// 
        /// ------------------------------------------------------------
        /// D: Sonst falls Material [LocalMaterial] CHARGENGEFÜHRT ist
        ///     führe Booking-Parameter-Check in diesem Zuammenhang durch
        /// ------------------------------------------------------------
        /// 
        /// Es gibt folgende Buchungsfälle:
        /// [Übergebene Entitäten]:
        /// ..........................
        /// 1. [FacilityCharge]:                           
        /// Zur Bestandsveränderung oder Umlagerung einer Facility-Charge
        ///     Pseudo-Code:        
        ///     FacilityLot, Facility, Material, FacilityLocation darf nicht übergeben werden
        ///         Breche ab mit Fehler
        /// 
        /// 2. [FacilityLocation]
        /// 
        ///     Pseudo-code:
        ///     Facility, FacilityCharge darf nicht übergeben werden
        ///         Breche ab mit Fehler
        ///     Material oder FacilityLot muss übergeben werden zur weiteren Eingrenzung
        ///         Breche ab mit Fehler
        /// 
        /// 2.1 [FacilityLocation],[Material]:		        
        /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit Material vorhanden sind
        /// 
        ///     Pseudo-code:
        ///     Wenn keine FacilityChargen mit Material auf Lagerort vorhanden
        ///             Hole Standard-Einlagerplatz aus FacilityLocation(Lagerort)
        ///                 Falls Standard-Einlagerplatz nicht existiert
        ///                     breche ab mit Fehler
        ///             Setze LocalFacility mit Standard-Einlagerplatz
        /// 
        /// 2.2 [FacilityLocation],[FacilityLot]:			
        /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit FacilityLot vorhanden sind
        /// 
        ///     Pseudo-code:
        ///     Wenn keine FacilityChargen mit FacilityLot auf Lagerort vorhanden
        ///             Hole Standard-Einlagerplatz aus FacilityLocation(Lagerort)
        ///                 Falls Standard-Einlagerplatz nicht existiert
        ///                     breche ab mit Fehler
        ///             Setze LocalFacility mit Standard-Einlagerplatz
        /// 
        /// 2.3 [FacilityLocation],[FacilityLot],[Material]:
        /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit FacilityLot und Material vorhanden
        /// 
        ///     Pseudo-code:
        ///     Wenn keine FacilityChargen mit Material und FacilityLocation auf Lagerort vorhanden
        ///             Hole Standard-Einlagerplatz aus FacilityLocation(Lagerort)
        ///                 Falls Standard-Einlagerplatz nicht existiert
        ///                     breche ab mit Fehler
        ///             Setze LocalFacility mit Standard-Einlagerplatz
        /// 
        /// 
        /// 3. [Facility]:   
        /// Nur bei Silos möglich zur Silobestandsführung
        /// 
        ///     Pseudo-code:
        ///     FacilityCharge, FacilityLocation darf nicht übergeben werden
        ///         Breche ab mit Fehler
        ///     Setze LocalFacility mit Facility
        /// 
        ///     Wenn nicht [Material] und nicht [FacilityLot] übergeben
        ///         Falls keine FacilityCharge vorhanden
        ///             Wenn keine Silo-Belegung, 
        ///                 dann breche ab mit Fehler
        ///             Sonst
        ///                 dann Neuanlage von Anonymer FacilityCharge 
        ///         Sonst
        ///             Wenn keine Silo-Belegung
        ///                 Dann setze Belegung mit erster FacilityCharge.Material bzw. bereits gesetztes [LocalMaterial]
        ///             Stackbuchung auf FacilityChargen mit Facility.Material (bzw. Material aus Fall 3.1)
        ///     Sonst Fall 3.1 oder Fall 3.2
        /// 
        /// 3.1. [Facility],[Material]:   
        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        /// Und zum anlegen von Anonymen Chargen, weil zu dem Buchungszeitpunkt noch keine Cahrgeninformation da war
        /// 
        ///     Pseudo-code:
        /// Wenn Silo
        ///     Wenn keine Belegung vorgegeben
        ///         Falls FacilityChargen vorhanden sind
        ///             Autokorrektur Belegung
        ///     Wenn Belegung vorgegeben
        ///         dann muss Belegung mit Material stimmen 
        ///     Wenn keine Belegung vorgegeben
        ///         dann Autobelegung mit Material wenn Erlaubt
        /// Wenn keine FacilityCharge mit Material vorhanden
        ///     dann Neuanlage von ANONYMER FacilityCharge 
        /// Sonst wenn FacilityChargen mit Material vorhanden
        ///     Stackbuchung wie Fall 3, jedoch mit Material
        ///         
        /// 3.2. [Facility],[FacilityLot]:  
        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        /// Es werden KEINE Anonynmen Chargen angelegt!
        /// 
        ///     Pseudo-code:
        /// Wenn Silo
        ///     Wenn keine Belegung vorgegeben
        ///         Falls FacilityChargen vorhanden sind
        ///             Autokorrektur Belegung
        ///     Wenn Belegung vorgegeben
        ///         dann muss Belegung mit Material aus FacilityLot stimmen
        ///     Wenn keine Belegung vorgegeben
        ///         dann Autobelegung mit Material aus FacilityLot wenn Erlaubt (entspricht [LocalMaterial])
        ///     Setze LocalMaterial
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
        ///         dann Neuanlage FacilityCharge aus FacilityLot jedoch mit Materialnummer aus FacilityLot (entspricht [LocalMaterial])
        /// 
        /// 3.3. [Facility],[Material],[FacilityLot]:
        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        /// Es werden KEINE Anonynmen Chargen angelegt, aber mit Materialien die dem übergbenen Material entsprechen!
        /// Falls anonyme Chargen vorhanden sind, dann wird der anonyme Status aufgehoben durch Ersetzung mit FacilityLot
        ///
        ///     Pseudo-code:
        /// Setze LocalMaterial mit Material
        /// Wenn Silo
        ///     Wenn keine Belegung vorgegeben
        ///         Falls FacilityChargen vorhanden sind
        ///             Autokorrektur Belegung
        ///     Wenn Belegung vorgegeben
        ///         dann muss Belegung mit übergebenen Material stimmen
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
        /// 
        /// 
        /// 4. [FacilityLot]:                              
        /// Zur Stackbuchung wenn FacilityChargen von FacilityLot vorhanden
        ///     Pseudo-code:
        ///     Falls keine FacilityChargen von FacilityLot vorhanden
        ///         Breche ab mit Fehler
        /// 
        /// 4.1.[FacilityLot],[Material]:			        
        /// Zur Stackbuchung wenn FacilityCharge mit Materialnummer vorhanden
        ///     Pseudo-code:
        ///     Falls keine FacilityChargen von FacilityLot und Material vorhanden
        ///         Breche ab mit Fehler
        /// 
        ///     
        /// Falls LocalFacility gesetzt und Facility(Lagerplatz) ein Behältnis (Silo, Tank) ist
        ///     Falls Facility mit Artikelnummer belegt
        ///         Falls Lagerplatzbelegung mit LocalMaterial nicht übereinstimmt
        ///             Breche ab mit Fehler
        ///         Falls FacilityLot übergeben, dann muss Lagerplatzbelegung mit FacilityLot.Material übereinstimmen
        ///             Breche ab bei Fehler
        ///     Sonst
        ///         Wenn Silobelegung automatisch gesetzt werden darf
        ///             Setze Belegung, entweder mit Material oder mit FacilityLot.Material
        ///         Sonst
        ///             Breche ab mit Fehler
        ///     Falls keine Lagerplatz keine Freigabe hat
        ///         Breche ab mit Fehler
        /// 
        /// Falls Lager nicht von variobatch verwaltet und "Ignoriere Verwaltung"-Kennzeichen nicht gesetzt
        ///     Breche ab mit Fehler
        /// 
        /// Anlage Buchungsdatensatz
        ///     Setze leere FacilityLocation-, Facility-, Material-, FacilityLot, FacilityCharge-Daten aus übergebenen Daten
        /// 
        /// 
        /// ------------------------------------------------------------
        /// E: Sonst falls Material [LocalMaterial] CHARGENGEFÜHRT ist
        ///     führe BUCHUNGEN durch
        /// ------------------------------------------------------------
        /// FALLS 1. [FacilityCharge]:                           
        /// Zur Bestandsveränderung oder Umlagerung einer Facility-Charge
        /// 
        /// SONST FALLS 2. [FacilityLocation]
        ///     FALLS 2.1 [FacilityLocation],[Material]:		        
        ///     Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit Material vorhanden sind
        ///     
        ///     SONST FALLS 2.2 [FacilityLocation],[FacilityLot]:			
        ///     Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit FacilityLot vorhanden sind
        ///     
        ///     SONST FALLS 2.3 [FacilityLocation],[FacilityLot],[Material]:
        ///     Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit FacilityLot und Material vorhanden
        /// SONST FALLS 3. [Facility]:   
        ///     FALLS 3. NICHT [Material] UND NICHT [FacilityLot]
        ///     Nur bei Silos möglich zur Silobestandsführung
        ///     
        ///     SONST FALLS 3.1. [Facility],[Material]:   
        ///     Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        ///     Und zum anlegen von Anonymen Chargen, weil zu dem Buchungszeitpunkt noch keine Cahrgeninformation da war
        ///     
        ///     SONST FALLS 3.2. [Facility],[FacilityLot]:  
        ///     Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        ///     Es werden KEINE Anonynmen Chargen angelegt!
        ///     
        ///     SONST FALLS 3.3. [Facility],[Material],[FacilityLot]:
        ///     Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        ///     Es werden KEINE Anonynmen Chargen angelegt, aber mit Materialien die dem übergbenen Material entsprechen!
        ///     Falls anonyme Chargen vorhanden sind, dann wird der anonyme Status aufgehoben durch Ersetzung mit FacilityLot
        ///     
        /// SONST FALLS 4. [FacilityLot]:    
        ///     FALLS 4. UND NICHT [Material]
        ///     Zur Stackbuchung wenn FacilityChargen von FacilityLot vorhanden
        ///     
        ///     SONST FALLS 4.1.[FacilityLot],[Material]:			        
        ///     Zur Stackbuchung wenn FacilityCharge mit Materialnummer vorhanden
        /// 
        /// 
        /// ------------------------------------------------------------
        /// F: SONST Material [LocalMaterial] NICHT CHARGENGEFÜHRT
        ///     FÜHRE BUCHUNGEN DURCH
        /// ------------------------------------------------------------
        /// (StackBookingModel, IgnoreZeroStock, ShiftBookingReverse, xxxFacilityLot, xxxFacilityCharge werden Ignoriert)
        ///
        ///         Falls Materialreservierung
        ///             Lege Reservierungsdatensatz an
        ///             Addiere Reservierungsmenge auf Stock-Tabellen
        ///                 Falls xxx-Stock-Tabelle nicht vorhanden lege an
        ///         
        ///         Sonst Keine Materialreservierung oder Reservierungsauflösung
        ///             Falls Reservierungsauflösung ohne Buchung
        ///                 - lösche Reservierungsdatensatz
        ///                 - Reduziere Reservierungsmengen in Stock-Tabellen
        ///             Falls Richtige Buchung
        ///                 Falls Reservierungsinformationen mit übergeben
        ///                     Falls Abgangsbuchung
        ///                         - Checke ob es Materialreservierungen auf Material- oder Lagerortebene gibt und die nicht zu der übergebenen Reservierungsreferenz gehören
        ///                           und der verfügbare Bestand ausreicht (negativer Bestand ist in dem Zusammenhang ungültig)
        ///                             - Falls verfügbarer Bestand nicht ausreicht,
        ///                               breche Buchungsvorgang ab
        ///                     Falls Zugangsbuchung (GIBT ES NICHT)
        ///                     Falls Umlagerungsbuchung
        ///                         - Checke ob es Materialreservierungen auf Lagerortebene gibt
        ///                             - Falls verfügbarer Bestands nicht ausreicht,
        ///                               breche Buchungsvorgang ab.
        ///                 
        ///                 Falls LocalFacility gesetzt -> Buche auf Einlagerplatz
        ///                     Falls Zu-/Abgangsbuchung
        ///                         *** 1003, FacilityCharge (Zu-/Abgangsbuchung auf Lagerplatz) >>>
        ///                         Falls FacilityCharge nicht übergeben
        ///                             - Finde FacilityCharge auf LocalFacility
        ///                             - Falls FacilityCharge nicht vorhanden lege an ohne FacilityLot!
        ///                             - Achtung FacilityCharge kann nur einmal vorkommen!
        ///                         
        ///                         Buche auf FacilityCharge
        ///                             *** 1002, 0 >>>
        ///                             - Verändere Bestand
        ///                             >>> 1002, 0 ***
        ///                             *** 1002, 1 Falls keine Umlagerung >>>
        ///                             - Falls vollständige Reservierungsauflösung
        ///                                 - lösche Reservierungsdatensatz
        ///                                 - Reduziere Reservierungsmenge
        ///                             - Sonst falls Teilmenge von Reservierung
        ///                                 - Reduziere Reservierungsmenge
        ///                             >>> 1002, 1 ***
        ///                         
        ///                         Falls Umlagerungsbuchung Materialbestand bleibt gleich und es gibt keine Reservierungsveränderung
        ///                         Sonst
        ///                             Buche auf Material
        ///                             *** 1001, 0 >>>
        ///                             - Falls MaterialStock nicht vorhanden lege an
        ///                             - Berücksichtige Negativen Bestands-Kennzeichen
        ///                             - Aktiviere Materialsperrung-/Freigabe
        ///                             >>> 1001, 0 ***
        ///                             
        ///                             *** 1001, 1 >>>
        ///                             - Falls vollständige Reservierungsauflösung
        ///                                 - lösche Reservierungsdatensatz
        ///                                 - Reduziere Reservierungsmenge
        ///                             - Sonst falls Teilmenge von Reservierung
        ///                                 - Reduziere Reservierungsmenge
        ///                             >>> 1001, 1 ***
        ///                         
        ///                         Buche auf Lagerplatz
        ///                             *** 1000, 0 >>>
        ///                             - Falls FacilityStock nicht vorhanden lege an
        ///                             - Führe Bestansdkorrektur durch immer in MDWeightUnit
        ///                             - Berücksichtige Negativen Bestands-Kennzeichen
        ///                             - Aktiviere Materialsperrung-/Freigabe
        ///                             >>> 1000, 0 ***
        ///                         >>> 1003, FacilityCharge (Zu-/Abgangsbuchung auf Lagerplatz) ****
        ///                         
        ///                     Falls Umlagerung
        ///                         >>> 1003, FacilityCharge, QUELLE (Zu-/Abgangsbuchung auf Lagerplatz) >>>
        ///                         >>> 1003, FacilityCharge, ZIEL (Zu-/Abgangsbuchung auf Lagerplatz) >>>
        ///
        ///                 Sonst Stackbuchung auf Lagerortebene (Falls LocalFacility nicht gesetzt -> Es gibt vorhandene Facility-Chargen)
        ///                     Falls Zu-/Abgangsbuchung
        ///                         Ermittle passenden StackCalculator
        ///                         Hole FacilityCharge-Liste von Stack-Calculator (Abhängig vom übergebenen Lagerort)
        ///                         Durchlaufe FacilityCharge-Liste
        ///                             >>> 1003, FacilityCharge (Zu-/Abgangsbuchung auf Lagerplatz) >>>
        ///
        ///                     Falls Umlagerung
        ///                         Ermittle passenden StackCalculator
        ///                         Hole FacilityCharge-Liste(QUELLE) und FacilityCharge-Liste(ZIEL) von Stack-Calculator (Abhängig vom übergebenen Lagerort)
        ///                         Durchlaufe FacilityCharge-Liste(QUELLE)
        ///                             >>> 1003, FacilityCharge (Zu-/Abgangsbuchung auf Lagerplatz) >>>
        ///                         Durchlaufe FacilityCharge-Liste(ZIEL)
        ///                             >>> 1003, FacilityCharge (Zu-/Abgangsbuchung auf Lagerplatz) >>>
        ///                 
        /// </summary>
        protected Global.ACMethodResultState DoFacilityBooking(ACMethodBooking BP)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;

            if (BP.IsCallForBooking)
            {
                #region if BP.IsLotManaged
                if (BP.ParamsAdjusted.IsLotManaged)
                {
                    /// Es gibt folgende Buchungsfälle:
                    /// [Übergebene Entitäten]:
                    /// ..........................
                    /// 1. [FacilityCharge]:                           
                    /// Zur Bestandsveränderung oder Umlagerung einer Facility-Charge
                    if ((BP.OutwardFacilityCharge != null) || (BP.InwardFacilityCharge != null))
                    {
                        bookingResult = BookingOn_FacilityCharge(BP);
                    }

                    /// 2. [FacilityLocation]
                    else if ((BP.OutwardFacilityLocation != null) || (BP.InwardFacilityLocation != null))
                    {
                        if ((BP.OutwardMaterial != null) || (BP.InwardMaterial != null))
                        {
                            /// 2.1 [FacilityLocation],[Material]:		        
                            /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit Material vorhanden sind
                            if ((BP.OutwardFacilityLot == null) && (BP.InwardFacilityLot == null))
                            {
                                bookingResult = BookingOn_FacilityLocation_Material(BP);
                            }
                            else
                            {
                                /// 2.3 [FacilityLocation],[FacilityLot],[Material]:
                                /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit FacilityLot und Material vorhanden
                                bookingResult = BookingOn_FacilityLocation_FacilityLot_Material(BP);
                            }
                        }
                        /// 2.2 [FacilityLocation],[FacilityLot]:			
                        /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit FacilityLot vorhanden sind
                        else if ((BP.OutwardFacilityLot != null) || (BP.InwardFacilityLot != null))
                        {
                            bookingResult = BookingOn_FacilityLocation_FacilityLot(BP);
                        }
                        else
                        {
                            /// 2. [FacilityLocation]
                            bookingResult = BookingOn_FacilityLocation(BP);
                        }
                    }

                    /// 3. [Facility]:   
                    else if ((BP.OutwardFacility != null) || (BP.InwardFacility != null))
                    {
                        if ((BP.OutwardMaterial != null) || (BP.InwardMaterial != null))
                        {
                            /// 3.1. [Facility],[Material]:   
                            /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
                            /// Und zum anlegen von Anonymen Chargen, weil zu dem Buchungszeitpunkt noch keine Cahrgeninformation da war
                            if ((BP.OutwardFacilityLot == null) && (BP.InwardFacilityLot == null))
                            {
                                bookingResult = BookingOn_Facility_Material(BP);
                            }
                            else
                            {
                                /// 3.3. [Facility],[Material],[FacilityLot]:
                                /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
                                /// Es werden KEINE Anonynmen Chargen angelegt, aber mit Materialien die dem übergbenen Material entsprechen!
                                /// Falls anonyme Chargen vorhanden sind, dann wird der anonyme Status aufgehoben durch Ersetzung mit FacilityLot
                                bookingResult = BookingOn_Facility_FacilityLot_Material(BP);
                            }
                        }
                        /// 3.2. [Facility],[FacilityLot]:  
                        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
                        /// Es werden KEINE Anonynmen Chargen angelegt!
                        else if ((BP.OutwardFacilityLot != null) || (BP.InwardFacilityLot != null))
                        {
                            bookingResult = BookingOn_Facility_FacilityLot(BP);
                        }
                        /// 3. [Facility] Buchung ohne Angabe von Material und FacilityLot -> Silobuchung
                        else
                        {
                            bookingResult = BookingOn_Facility(BP);
                        }
                    }

                    /// 4. [FacilityLot]:                              
                    else if ((BP.OutwardFacilityLot != null) || (BP.InwardFacilityLot != null))
                    {
                        /// 4.1.[FacilityLot],[Material]:			        
                        /// Zur Stackbuchung wenn FacilityCharge mit Materialnummer vorhanden
                        if ((BP.OutwardMaterial != null) || (BP.InwardMaterial != null))
                        {
                            bookingResult = BookingOn_FacilityLot_Material(BP);
                        }
                        /// 4. [FacilityLot] Zur Stackbuchung wenn FacilityChargen von FacilityLot vorhanden
                        else
                        {
                            bookingResult = BookingOn_FacilityLot(BP);
                        }
                    }

                    if (bookingResult == Global.ACMethodResultState.Succeeded)
                        bookingResult = ReOrganizeCells(BP);
                }
                #endregion
                #region else !BP.IsLotManaged
                // Sonst nicht chargengeführtes Material
                else // if (!BP.IsLotManaged)
                {
                    /// Es gibt folgende Buchungsfälle:
                    /// [Übergebene Entitäten]:
                    /// ..........................
                    /// 1. [FacilityCharge]:                           
                    /// Zur Bestandsveränderung oder Umlagerung einer Facility-Charge
                    if ((BP.OutwardFacilityCharge != null) || (BP.InwardFacilityCharge != null))
                    {
                        bookingResult = BookingOn_FacilityCharge_NotLotManaged(BP);
                    }
                    /// 
                    ///     Es gibt folgende Buchungsfälle:
                    ///     [Übergebene Entitäten]:
                    ///     ..................................
                    ///     [FacilityLocation] oder [Facility]
                    ///
                    /// 5. [FacilityLocation]
                    else if ((BP.OutwardFacilityLocation != null) || (BP.InwardFacilityLocation != null))
                    {
                        /// Stackbuchung auf Lagerortebene
                        bookingResult = BookingOn_FacilityLocation_Material_NotLotManaged(BP);
                    }
                    /// 6. [Facility]:   
                    else if ((BP.OutwardFacility != null) || (BP.InwardFacility != null))
                    {
                        if ((BP.OutwardMaterial != null) || (BP.InwardMaterial != null))
                        {
                            /// 6.1. [Facility],[Material]:   
                            /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
                            /// Und zum anlegen von Anonymen Chargen
                            bookingResult = BookingOn_Facility_Material_NotLotManaged(BP);
                        }
                        // Buchung ohne Angabe von Material und FacilityLot -> Silobuchung
                        else
                        {
                            bookingResult = BookingOn_Facility_NotLotManaged(BP);
                        }
                    }
                    // else Zweig kann nie eintreten
                }
                #endregion

                // If relocation posting and target should be set to blocked state for new quants, set quant to blocked
                if (BP.ParamsAdjusted.PostingBehaviour == PostingBehaviourEnum.ZeroStockOnRelocation)
                {
                    List<FacilityCharge> quantsForZeroBooking = new List<FacilityCharge>();
                    if (BP.FacilityBookings != null)
                    {
                        foreach (ACMethodBooking booking in BP.FacilityBookings.ToArray())
                        {
                            foreach (var postings in booking.CreatedPostings)
                            {
                                if (postings.InwardFacilityCharge != null && !postings.InwardFacilityCharge.NotAvailable)
                                {
                                    if (!quantsForZeroBooking.Contains(postings.InwardFacilityCharge))
                                        quantsForZeroBooking.Add(postings.InwardFacilityCharge);
                                }
                            }
                        }
                    }
                    foreach (FacilityCharge fc in quantsForZeroBooking)
                    {
                        ACMethodBooking method = NewBookParamZeroStock(BP, fc);
                        method.FacilityBooking = BP.FacilityBooking;
                        Global.ACMethodResultState bookingSubResult = BookingOn_FacilityCharge(method);
                        if ((bookingSubResult == Global.ACMethodResultState.Failed) || (bookingSubResult == Global.ACMethodResultState.Notpossible))
                        {
                            BP.Merge(method.ValidMessage);
                            return bookingSubResult;
                        }
                        else
                        {
                            BP.FacilityBookings.Add(method);
                        }
                    }
                }
            }
            else if (BP.IsCallForMatching)
            {
                bookingResult = DoMatching(BP);
            }
            else if (BP.IsCallForClosing)
            {
                bookingResult = DoClosing(BP);
            }
            else if (BP.IsCallForInventory)
            {
                bookingResult = DoInventory(BP);
            }

            return bookingResult;
        }

        
        #region Booking on Lot-Managed Materials / Entity-Combinations
        /// <summary>
        /// 1. [FacilityCharge]:                           
        /// Zur Bestandsveränderung oder Umlagerung einer Facility-Charge
        /// </summary>
        private Global.ACMethodResultState BookingOn_FacilityCharge(ACMethodBooking BP)
        {
            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;
            FacilityBooking FB = BP.FacilityBooking != null ? BP.FacilityBooking : NewFacilityBooking(BP);
            ///     Pseudo-Code:        
            // Falls Umlagerungsbuchung von FacilityCharge nach FacilityCharge
            // FacitlityBookingType.Relocation_FacilityCharge:
            if ((BP.OutwardFacilityCharge != null) && (BP.InwardFacilityCharge != null))
            {
                // Falls Quellcharge nicht anonym und Zielcharge anonym, dann mache eine nicht anonyme daraus
                if ((BP.OutwardFacilityCharge.FacilityLot != null) && (BP.InwardFacilityCharge.FacilityLot == null) && BP.IsLotManaged)
                {
                    BP.ParamsAdjusted.InwardFacilityCharge.FacilityLot = BP.ParamsAdjusted.OutwardFacilityCharge.FacilityLot;
                }

                // Falls Umlagerung innerhalb des gleichen Lagerplatzes
                if (BP.OutwardFacilityCharge.Facility == BP.InwardFacilityCharge.Facility)
                {
                    // Split muss unterschiedlich sein, 
                    // darf aber nicht vorkommen, weil sie die selbe GUID dann haben müssen
                    if (BP.OutwardFacilityCharge.SplitNo == BP.InwardFacilityCharge.SplitNo)
                    {
                    }
                }
                else
                {
                    // Split kann unterschiedlich sein
                }

                // 1. Auslagerungsbuchung von Quellcharge
                FacilityBookingCharge FBC = NewFacilityBookingCharge(BP, false);
                bookingResult = InitFacilityBookingCharge_FromBookingParameter_Outward(BP, FBC);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;

                bookingResult = BookFacilityBookingChargeOut(BP, FBC, null);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;

                // 2. Einlagerungsbuchung auf Zielcharge
                FBC = NewFacilityBookingCharge(BP, false);
                bookingResult = InitFacilityBookingCharge_FromBookingParameter_Inward(BP, FBC);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;

                bookingResult = BookFacilityBookingChargeIn(BP, FBC, null);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;
            }
            // Buchung von OutwardFacilityCharge
            else if (BP.OutwardFacilityCharge != null)
            {
                FacilityCharge inwardFacilityCharge = null;
                // Umlagerungbuchung auf ein Silo/Tank... 
                // FacitlityBookingType.Relocation_FacilityCharge_Facility:
                if (BP.ParamsAdjusted.InwardFacility != null)
                {
                    int splitNo = BP.ParamsAdjusted.OutwardFacilityCharge.SplitNo;
                    if (BP.BookingType == GlobalApp.FacilityBookingType.Split_FacilityCharge)
                    {
                        if (BP.InwardSplitNo.HasValue && BP.OutwardFacilityCharge.SplitNo != BP.InwardSplitNo.Value)
                            splitNo = BP.InwardSplitNo.Value;
                        else
                        {
                            if (BP.ParamsAdjusted.IsLotManaged)
                                splitNo = BP.DatabaseApp.FacilityCharge.Where(c => c.FacilityLotID == BP.OutwardFacilityCharge.FacilityLotID).Max(c => c.SplitNo);
                            else
                                splitNo = BP.DatabaseApp.FacilityCharge.Where(c => c.MaterialID == BP.OutwardFacilityCharge.MaterialID && c.FacilityID == BP.OutwardFacilityCharge.FacilityID && !c.NotAvailable).Max(c => c.SplitNo);
                            splitNo++;
                        }
                        BP.InwardQuantity = BP.OutwardQuantity;
                        BP.ParamsAdjusted.InwardQuantity = BP.OutwardQuantity;
                    }
                    // Für Umlagerungen kann OutwardFacility oder OutwardFacilityLocation anstatt OutwardFacilityCharge gesetzt sein -> Neuanlage auf Ziellagerplatz
                    // Gibt es eine FacilityCharge von derselben FacilityLot und Split-Nummer?
                    FacilityChargeList facilityChargeList = null;
                    if (BP.IsLotManaged)
                    {
                        if (BP.ParamsAdjusted.OutwardFacilityCharge.Partslist != null)
                        {
                            facilityChargeList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                        where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                            && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityCharge.FacilityLot.FacilityLotID)
                                                                            && (c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.MaterialID)
                                                                            //   || (BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID))
                                                                            && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.OutwardFacilityCharge.Partslist.PartslistID))
                                                                            && (c.SplitNo == splitNo)
                                                                        select c);
                        }
                        else
                        {
                            facilityChargeList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                        where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                            && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityCharge.FacilityLot.FacilityLotID)
                                                                            && (c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.MaterialID)
                                                                            //   || (BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID))
                                                                            && (c.Partslist == null)
                                                                            && (c.SplitNo == splitNo)
                                                                        select c);
                        }
                    }
                    else
                    {
                        if (BP.ParamsAdjusted.OutwardFacilityCharge.Partslist != null)
                        {
                            facilityChargeList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                        where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                            && (c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.MaterialID)
                                                                            //   || (BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID))
                                                                            && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.OutwardFacilityCharge.Partslist.PartslistID))
                                                                            && (c.SplitNo == splitNo)
                                                                        select c);
                        }
                        else
                        {
                            facilityChargeList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                        where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                            && (c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.MaterialID)
                                                                            //   || (BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID))
                                                                            && (c.Partslist == null)
                                                                            && (c.SplitNo == splitNo)
                                                                        select c);
                        }
                    }
                    // Falls ja, dann buche darauf
                    if (facilityChargeList.Any())
                    {
                        inwardFacilityCharge = facilityChargeList.First();
                        // Falls kein Rücksetzen erlaubt und neue Charge angelegt werden muss
                        if (inwardFacilityCharge.NotAvailable && BP.IsLotManaged && !BP.IsAutoResetNotAvailable)
                            inwardFacilityCharge = null;
                    }
                    // Falls nicht,
                    if (inwardFacilityCharge == null)
                    {
                        if (BP.IsLotManaged)
                        {
                            // Gibt es eine anonyme Charge auf dieser Facility?
                            if (BP.ParamsAdjusted.OutwardFacilityCharge.Partslist != null)
                            {
                                facilityChargeList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                            where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                                && (c.FacilityLot == null)
                                                                                && (c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.MaterialID)
                                                                                //   || (BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID))
                                                                                && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.OutwardFacilityCharge.Partslist.PartslistID))
                                                                            select c);
                            }
                            else
                            {
                                facilityChargeList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                            where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                                && (c.FacilityLot == null)
                                                                                && (c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.MaterialID)
                                                                                //   || (BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID))
                                                                                && (c.Partslist == null)
                                                                            select c);
                            }
                        }
                        // Falls ja, dann mache daraus eine nicht anonyme (Was passiert mit Chargenverfolgung?) und buche
                        if (facilityChargeList.Any())
                        {
                            inwardFacilityCharge = facilityChargeList.First();
                            if (inwardFacilityCharge.NotAvailable && BP.IsLotManaged && !BP.IsAutoResetNotAvailable)
                                inwardFacilityCharge = null;
                            else
                            {
                                if (BP.IsLotManaged)
                                    inwardFacilityCharge.FacilityLot = BP.ParamsAdjusted.OutwardFacilityCharge.FacilityLot;
                                inwardFacilityCharge.SplitNo = splitNo;
                            }
                        }
                        // Sonst, lege eine neue an und buche
                        if (inwardFacilityCharge == null)
                        {
                            inwardFacilityCharge = FacilityCharge.NewACObject(BP.DatabaseApp, null);
                            inwardFacilityCharge.CloneFrom(BP.ParamsAdjusted.OutwardFacilityCharge, false);
                            if (!BP.IsLotManaged && inwardFacilityCharge.FacilityLot != null)
                                inwardFacilityCharge.FacilityLot = null;
                            inwardFacilityCharge.Facility = BP.ParamsAdjusted.InwardFacility;
                            inwardFacilityCharge.NotAvailable = false;
                            inwardFacilityCharge.AddToParentsList();
                            // Einlagerdatum + Eindeutige Reihenfolgennumer der Einlagerung
                            inwardFacilityCharge.FillingDate = DateTime.Now;
                            inwardFacilityCharge.FacilityChargeSortNo = inwardFacilityCharge.Facility.GetNextFCSortNo(BP.DatabaseApp);
                            inwardFacilityCharge.SplitNo = splitNo;
                        }
                    }

                    if (inwardFacilityCharge == null)
                    {
                        BP.AddBookingMessage(ACMethodBooking.eResultCodes.WrongStateOfEntity, Root.Environment.TranslateMessage(this, "Error00052",
                                            BP.ParamsAdjusted.InwardFacility.FacilityNo, BP.ParamsAdjusted.InwardFacility.FacilityName));
                        return Global.ACMethodResultState.Notpossible;
                    }
                    else
                    {
                        // 1. Auslagerungsbuchung von Quellcharge
                        FacilityBookingCharge FBC = NewFacilityBookingCharge(BP, false);
                        bookingResult = InitFacilityBookingCharge_FromBookingParameter_Outward(BP, FBC);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        bookingResult = BookFacilityBookingChargeOut(BP, FBC, facilityChargeList);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        // 2. Einlagerungsbuchung auf Zielcharge
                        FBC = NewFacilityBookingCharge(BP, false);
                        bookingResult = InitFacilityBookingCharge_FromBookingParameter_Inward(BP, FBC, inwardFacilityCharge);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        bookingResult = BookFacilityBookingChargeIn(BP, FBC, facilityChargeList);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;
                    }
                }
                // Umlagerungsbuchung auf ein Lagerort, ist erlaubt
                // FacitlityBookingType.Relocation_FacilityCharge_FacilityLocation:
                else if (BP.InwardFacilityLocation != null)
                {
                    // Suche ob es eine FacilityCharge derselben FacilityLot und Split-Nummer auf dem Lagerort gibt
                    FacilityChargeList facilityChargeList = null;
                    if (BP.IsLotManaged)
                    {
                        if (BP.ParamsAdjusted.OutwardFacilityCharge.Partslist != null)
                        {
                            facilityChargeList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                        where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.InwardFacilityLocation.FacilityID)
                                                                                               && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityCharge.FacilityLot.FacilityLotID)
                                                                                               && (c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.MaterialID)
                                                                                                //   || (BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID))
                                                                                               && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.OutwardFacilityCharge.Partslist.PartslistID))
                                                                                               && (c.SplitNo == BP.ParamsAdjusted.OutwardFacilityCharge.SplitNo)
                                                                        select c);
                        }
                        else
                        {
                            facilityChargeList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                        where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.InwardFacilityLocation.FacilityID)
                                                                                                && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityCharge.FacilityLot.FacilityLotID)
                                                                                                && (c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.MaterialID)
                                                                                                //   || (BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID))
                                                                                                && (c.Partslist == null)
                                                                                                && (c.SplitNo == BP.ParamsAdjusted.OutwardFacilityCharge.SplitNo)
                                                                        select c);
                        }
                    }
                    // Falls ja,
                    if (facilityChargeList.Count > 0)
                    {
                        // buche darauf
                        inwardFacilityCharge = facilityChargeList.First();
                        // Falls kein Rücksetzen erlaubt und neue Charge angelegt werden muss
                        if (inwardFacilityCharge.NotAvailable && !BP.IsAutoResetNotAvailable)
                            inwardFacilityCharge = null;
                    }
                    // Sonst falls nein,
                    if (inwardFacilityCharge == null)
                    {
                        // Suche im Einlagerplatz, ob es dort eine anonyme Charge gibt
                        if (BP.ParamsAdjusted.OutwardFacilityCharge.Partslist != null)
                        {
                            facilityChargeList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                        where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacilityLocation.Facility1_IncomingFacility.FacilityID)
                                                                            && (c.FacilityLot == null)
                                                                            && (c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.MaterialID)
                                                                            //   || (BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID))
                                                                            && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.OutwardFacilityCharge.Partslist.PartslistID))
                                                                        select c);
                        }
                        else
                        {
                            facilityChargeList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                        where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacilityLocation.Facility1_IncomingFacility.FacilityID)
                                                                            && (c.FacilityLot == null)
                                                                            && (c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.MaterialID)
                                                                            //   || (BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardFacilityCharge.Material.ProductionMaterialID))
                                                                            && (c.Partslist == null)
                                                                        select c);
                        }
                        // Falls ja, dann mache daraus eine nicht anonyme und buche
                        if (facilityChargeList.Count > 0)
                        {
                            inwardFacilityCharge = facilityChargeList.First();
                            if (inwardFacilityCharge.NotAvailable && !BP.IsAutoResetNotAvailable)
                                inwardFacilityCharge = null;
                            else
                            {
                                if (BP.IsLotManaged)
                                    inwardFacilityCharge.FacilityLot = BP.ParamsAdjusted.OutwardFacilityCharge.FacilityLot;
                                inwardFacilityCharge.SplitNo = BP.ParamsAdjusted.OutwardFacilityCharge.SplitNo;
                            }
                        }
                        // Sonst, lege eine neue an und buche
                        if (inwardFacilityCharge == null)
                        {
                            inwardFacilityCharge = FacilityCharge.NewACObject(BP.DatabaseApp, null);
                            inwardFacilityCharge.CloneFrom(BP.ParamsAdjusted.OutwardFacilityCharge, false);
                            if (!BP.IsLotManaged && inwardFacilityCharge.FacilityLot != null)
                                inwardFacilityCharge.FacilityLot = null;
                            inwardFacilityCharge.Facility = BP.ParamsAdjusted.InwardFacilityLocation.Facility1_IncomingFacility;
                            inwardFacilityCharge.NotAvailable = false;
                            inwardFacilityCharge.AddToParentsList();
                            // Einlagerdatum + Eindeutige Reihenfolgennumer der Einlagerung
                            inwardFacilityCharge.FillingDate = DateTime.Now;
                            inwardFacilityCharge.FacilityChargeSortNo = inwardFacilityCharge.Facility.GetNextFCSortNo(BP.DatabaseApp);
                        }
                    }

                    if (inwardFacilityCharge == null)
                    {
                        BP.AddBookingMessage(ACMethodBooking.eResultCodes.WrongStateOfEntity, Root.Environment.TranslateMessage(this, "Error00052",
                                            BP.ParamsAdjusted.InwardFacility.FacilityNo, BP.ParamsAdjusted.InwardFacility.FacilityName));
                        return Global.ACMethodResultState.Notpossible;
                    }
                    else
                    {
                        // 1. Auslagerungsbuchung von Quellcharge
                        FacilityBookingCharge FBC = NewFacilityBookingCharge(BP, false);
                        bookingResult = InitFacilityBookingCharge_FromBookingParameter_Outward(BP, FBC);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        bookingResult = BookFacilityBookingChargeOut(BP, FBC, facilityChargeList);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        // 2. Einlagerungsbuchung auf Zielcharge
                        FBC = NewFacilityBookingCharge(BP, false);
                        bookingResult = InitFacilityBookingCharge_FromBookingParameter_Inward(BP, FBC, inwardFacilityCharge);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        bookingResult = BookFacilityBookingChargeIn(BP, FBC, facilityChargeList);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;
                    }
                }
                // Umbuchung auf eine andere Materialnummer
                else if (BP.InwardMaterial != null && BP.BookingType == GlobalApp.FacilityBookingType.Reassign_FacilityCharge)
                {
                    // Auslagerungsbuchung auf altem Material
                    FacilityBookingCharge FBC = NewFacilityBookingCharge(BP, false);

                    BP.ParamsAdjusted.MDBookingNotAvailableMode = DatabaseApp.s_cQry_GetMDBookingNotAvailableMode(BP.DatabaseApp, MDBookingNotAvailableMode.BookingNotAvailableModes.AutoSetAndReset).FirstOrDefault();
                    double quantityToBook = BP.OutwardFacilityCharge.StockQuantity;
                    bookingResult = InitFacilityBookingCharge_FromFacilityCharge_Outward(BP, FBC, BP.OutwardFacilityCharge,
                                       false,
                                       quantityToBook, quantityToBook, BP.OutwardFacilityCharge.MDUnit);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    bookingResult = BookFacilityBookingChargeOut(BP, FBC, null);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;


                    // Einlagerungsbuchung auf neues Material, Suche ob Quant im Lager vorhanden sonst lege neues Quant an
                    FBC = NewFacilityBookingCharge(BP, false);

                    inwardFacilityCharge = BP.DatabaseApp.FacilityCharge.Where(c => c.FacilityID == BP.OutwardFacilityCharge.FacilityID
                                                    && c.MaterialID == BP.InwardMaterial.MaterialID
                                                    && c.FacilityLotID == BP.OutwardFacilityCharge.FacilityLotID
                                                    && c.SplitNo == BP.OutwardFacilityCharge.SplitNo).FirstOrDefault();
                    if (inwardFacilityCharge == null)
                    {
                        inwardFacilityCharge = FacilityCharge.NewACObject(BP.DatabaseApp, null);
                        inwardFacilityCharge.CloneFrom(BP.ParamsAdjusted.OutwardFacilityCharge, false);
                        inwardFacilityCharge.Material = BP.InwardMaterial;
                        inwardFacilityCharge.NotAvailable = false;
                        if (BP.InwardPartslist != null)
                            inwardFacilityCharge.Partslist = BP.InwardPartslist;
                        inwardFacilityCharge.AddToParentsList();
                    }
                    else if (inwardFacilityCharge.NotAvailable)
                        inwardFacilityCharge.NotAvailable = false;
                    if (BP.InwardPartslist != null)
                        inwardFacilityCharge.Partslist = BP.InwardPartslist;
                    else
                        inwardFacilityCharge.Partslist = null;

                    bookingResult = InitFacilityBookingCharge_FromFacilityCharge_Inward(BP, FBC, inwardFacilityCharge,
                        false,
                        quantityToBook, quantityToBook, BP.OutwardFacilityCharge.MDUnit);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    bookingResult = BookFacilityBookingChargeIn(BP, FBC, null);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;
                }
                // Sonst normale Auslagerungsbuchung
                // FacitlityBookingType.OutwardMovement_FacilityCharge:
                else
                {
                    FacilityBookingCharge FBC = NewFacilityBookingCharge(BP, false);
                    bookingResult = InitFacilityBookingCharge_FromBookingParameter_Outward(BP, FBC);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    bookingResult = BookFacilityBookingChargeOut(BP, FBC, null);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;
                }
            }
            // Sonst Buchung auf InwardFacilityCharge
            else // (InwardFacilityCharge != null)
            {
                // Einlagerungsbuchung
                FacilityBookingCharge FBC = NewFacilityBookingCharge(BP, false);
                bookingResult = InitFacilityBookingCharge_FromBookingParameter_Inward(BP, FBC);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;

                bookingResult = BookFacilityBookingChargeIn(BP, FBC, null);
                if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                    return bookingResult;
            }

            return bookingResult;
        }

        /// <summary>
        /// 2. [FacilityLocation]
        /// </summary>
        private Global.ACMethodResultState BookingOn_FacilityLocation(ACMethodBooking BP)
        {
            ///     Material oder FacilityLot muss übergeben werden zur weiteren Eingrenzung
            return Global.ACMethodResultState.Notpossible;
        }

        /// <summary>
        /// 2.1 [FacilityLocation],[Material]:		        
        /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit Material vorhanden sind
        /// </summary>
        private Global.ACMethodResultState BookingOn_FacilityLocation_Material(ACMethodBooking BP)
        {
            ///     Pseudo-code:
            ///     Wenn keine FacilityChargen mit Material auf Lagerort vorhanden
            ///             Hole Standard-Einlagerplatz aus FacilityLocation(Lagerort)
            ///                 Falls Standard-Einlagerplatz nicht existiert
            ///                     breche ab mit Fehler
            ///             Setze LocalFacility mit Standard-Einlagerplatz
            /// Einlagerungsbuchung: 
            if ((BP.OutwardFacilityLocation == null) && (BP.InwardFacilityLocation != null))
            {
                /// Wenn keine FacilityChargen mit Material auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Einlagerplatz angelegt werden
                /// Local.InwardMaterial ist garantiert gesetzt aufgrund des Aufrufs von CheckAndAdjustMaterial() zuvor
                int nCount = 0;
                if (BP.ParamsAdjusted.InwardPartslist != null)
                {
                    nCount = (from c in BP.DatabaseApp.FacilityCharge
                                                                            where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.InwardFacilityLocation.FacilityID)
                                                                               && ((c.MaterialID == BP.ParamsAdjusted.InwardMaterial.MaterialID)
                                                                                  || (BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID))
                                                                               && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.InwardPartslist.PartslistID))
                                                                               && (c.NotAvailable == false)
                                                                            select c).Count();
                }
                else
                {
                    nCount = (from c in BP.DatabaseApp.FacilityCharge
                                                                            where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.InwardFacilityLocation.FacilityID)
                                                                               && ((c.MaterialID == BP.ParamsAdjusted.InwardMaterial.MaterialID)                                                                                
                                                                                  || (BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID))
                                                                               && (c.NotAvailable == false)
                                                                            select c).Count();
                }                
                if (nCount <= 0)
                {
                    /// Falls Standard-Einlagerplatz nicht existiert, breche ab
                    if (BP.ParamsAdjusted.InwardFacilityLocation.Facility1_IncomingFacility == null)
                    {
                        //return BookingResult.Failed;
                    }
                    else
                    {
                        //Local.InwardFacility = InwardFacilityLocation.Facility1_IncomingFacility;
                    }
                }
            }
            /// Auslagerungsbuchung: 
            else if ((BP.OutwardFacilityLocation != null) && (BP.InwardFacilityLocation == null))
            {
                /// Wenn keine FacilityChargen mit Material auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Auslagerplatz angelegt werden
                /// Local.OutwardMaterial ist garantiert gesetzt aufgrund des Aufrufs von CheckAndAdjustMaterial() zuvor
                int nCount = 0;
                if (BP.ParamsAdjusted.OutwardPartslist != null)
                {
                    nCount = (from c in BP.DatabaseApp.FacilityCharge
                                                                            where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.OutwardFacilityLocation.FacilityID)
                                                                               && ((c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.MaterialID)
                                                                                  || (BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID))
                                                                               && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.OutwardPartslist.PartslistID))
                                                                               && (c.NotAvailable == false)
                                                                            select c).Count();
                }
                else
                {
                    nCount = (from c in BP.DatabaseApp.FacilityCharge
                                                                            where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.OutwardFacilityLocation.FacilityID)
                                                                               && ((c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.MaterialID)
                                                                                  || (BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID))
                                                                               && (c.NotAvailable == false)
                                                                            select c).Count();
                }
                if (nCount <= 0)
                {
                    /// Falls Standard-Auslagerplatz nicht existiert, breche ab
                    if (BP.ParamsAdjusted.OutwardFacilityLocation.Facility1_OutgoingFacility == null)
                    {
                        //return BookingResult.Failed;
                    }
                    else
                    {
                        //Local.OutwardFacility = OutwardFacilityLocation.Facility1_OutgoingFacility;
                    }
                }
            }
            /// Umlagerungsbuchung: 
            else // if ((BP.OutwardFacilityLocation != null) && (BP.InwardFacilityLocation != null))
            {
            }
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!

            return Global.ACMethodResultState.Succeeded;
        }

        /// <summary>
        /// 2.2 [FacilityLocation],[FacilityLot]:			
        /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit FacilityLot vorhanden sind
        /// </summary>
        private Global.ACMethodResultState BookingOn_FacilityLocation_FacilityLot(ACMethodBooking BP)
        {
            ///     Pseudo-code:
            ///     Wenn keine FacilityChargen mit FacilityLot auf Lagerort vorhanden
            ///             Hole Standard-Einlagerplatz aus FacilityLocation(Lagerort)
            ///                 Falls Standard-Einlagerplatz nicht existiert
            ///                     breche ab mit Fehler
            ///             Setze LocalFacility mit Standard-Einlagerplatz
            if (BP.OutwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit FacilityLot auf Lagerort vorhanden
                /// Dann muss eine FacilityCharge auf dem Standard-Auslagerplatz angelegt werden
                int nCount = 0;
                if (BP.ParamsAdjusted.OutwardPartslist != null)
                {
                    nCount = (from c in BP.DatabaseApp.FacilityCharge
                                                                            where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.OutwardFacilityLocation.FacilityID)
                                                                                && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.OutwardPartslist.PartslistID))
                                                                               && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityLot.FacilityLotID)
                                                                               && (c.NotAvailable == false)
                                                                            select c).Count();
                }
                else
                {
                    nCount = (from c in BP.DatabaseApp.FacilityCharge
                                                                            where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.OutwardFacilityLocation.FacilityID)
                                                                               && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityLot.FacilityLotID)
                                                                               && (c.NotAvailable == false)
                                                                            select c).Count();
                }
                if (nCount <= 0)
                {
                    /// Falls Standard-Auslagerplatz nicht existiert, breche ab
                    if (BP.ParamsAdjusted.OutwardFacilityLocation.Facility1_OutgoingFacility == null)
                    {
                        return Global.ACMethodResultState.Failed;
                    }
                    else
                    {
                        //BP.Local.OutwardFacility = BP.OutwardFacilityLocation.Facility1_OutgoingFacility;
                    }
                }
            }
            if (BP.InwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit FacilityLot auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Einlagerplatz angelegt werden
                int nCount = 0;
                if (BP.ParamsAdjusted.InwardPartslist != null)
                {
                    nCount = (from c in BP.DatabaseApp.FacilityCharge
                                                                            where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.InwardFacilityLocation.FacilityID)
                                                                                && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.InwardPartslist.PartslistID))
                                                                               && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.InwardFacilityLot.FacilityLotID)
                                                                               && (c.NotAvailable == false)
                                                                            select c).Count();
                }
                else
                {
                    nCount = (from c in BP.DatabaseApp.FacilityCharge
                                                                            where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.InwardFacilityLocation.FacilityID)
                                                                               && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.InwardFacilityLot.FacilityLotID)
                                                                               && (c.NotAvailable == false)
                                                                            select c).Count();
                }
                if (nCount <= 0)
                {
                    /// Falls Standard-Einlagerplatz nicht existiert, breche ab
                    if (BP.ParamsAdjusted.InwardFacilityLocation.Facility1_IncomingFacility == null)
                    {
                        return Global.ACMethodResultState.Failed;
                    }
                    else
                    {
                        //Local.InwardFacility = InwardFacilityLocation.Facility1_IncomingFacility;
                    }
                }
            }
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!
            return Global.ACMethodResultState.Succeeded;
        }

        /// <summary>
        /// 2.3 [FacilityLocation],[FacilityLot],[Material]:
        /// Zur Stackbuchung auf Lagerortebene wenn dort FacilityChargen mit FacilityLot und Material vorhanden
        /// </summary>
        private Global.ACMethodResultState BookingOn_FacilityLocation_FacilityLot_Material(ACMethodBooking BP)
        {
            ///     Pseudo-code:
            ///     Wenn keine FacilityChargen mit FacilityLot und Material und FacilityLocation auf Lagerort vorhanden
            ///             Hole Standard-Einlagerplatz aus FacilityLocation(Lagerort)
            ///                 Falls Standard-Einlagerplatz nicht existiert
            ///                     breche ab mit Fehler
            ///             Setze LocalFacility mit Standard-Einlagerplatz
            if ((BP.ParamsAdjusted.OutwardFacilityLocation == null) || (BP.ParamsAdjusted.InwardFacilityLocation == null))
                return Global.ACMethodResultState.Failed;
            if ((BP.ParamsAdjusted.OutwardMaterial == null) && (BP.ParamsAdjusted.InwardMaterial == null))
                return Global.ACMethodResultState.Failed;
            if ((BP.ParamsAdjusted.OutwardFacilityLot == null) && (BP.ParamsAdjusted.InwardFacilityLot == null))
                return Global.ACMethodResultState.Failed;

            if (BP.OutwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit FacilityLot und Material auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Auslagerplatz angelegt werden
                /// Local.OutwardMaterial ist garantiert gesetzt aufgrund des Aufrufs von CheckAndAdjustMaterial() zuvor
                int nCount = 0;
                if (BP.ParamsAdjusted.OutwardPartslist != null)
                {
                    nCount = (from c in BP.DatabaseApp.FacilityCharge
                                                                            where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.OutwardFacilityLocation.FacilityID)
                                                                                && ((c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.MaterialID)
                                                                                  || (BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID))
                                                                                && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.OutwardPartslist.PartslistID))
                                                                                && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityLot.FacilityLotID)
                                                                                && (c.NotAvailable == false)
                                                                            select c).Count();
                }
                else
                {
                    nCount = (from c in BP.DatabaseApp.FacilityCharge
                                                                            where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.OutwardFacilityLocation.FacilityID)
                                                                               && ((c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.MaterialID)
                                                                                  || (BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID))
                                                                               && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityLot.FacilityLotID)
                                                                               && (c.NotAvailable == false)
                                                                            select c).Count();
                }
                if (nCount <= 0)
                {
                    /// Falls Standard-Auslagerplatz nicht existiert, breche ab
                    if (BP.ParamsAdjusted.OutwardFacilityLocation.Facility1_OutgoingFacility == null)
                    {
                        //return BookingResult.Failed;
                    }
                    else
                    {
                    }
                }
            }
            if (BP.InwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit FacilityLot und Material auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Einlagerplatz angelegt werden
                /// Local.InwardMaterial ist garantiert gesetzt aufgrund des Aufrufs von CheckAndAdjustMaterial() zuvor
                int nCount = 0;
                if (BP.ParamsAdjusted.InwardPartslist != null)
                {
                    nCount = (from c in BP.DatabaseApp.FacilityCharge
                                                                            where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.InwardFacilityLocation.FacilityID)
                                                                               && ((c.MaterialID == BP.ParamsAdjusted.InwardMaterial.MaterialID)
                                                                                  || (BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID))
                                                                               && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.InwardPartslist.PartslistID))
                                                                               && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.InwardFacilityLot.FacilityLotID)
                                                                               && (c.NotAvailable == false)
                                                                            select c).Count();
                }
                else
                {
                    nCount = (from c in BP.DatabaseApp.FacilityCharge
                                                                            where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.InwardFacilityLocation.FacilityID)
                                                                               && ((c.MaterialID == BP.ParamsAdjusted.InwardMaterial.MaterialID)
                                                                                  || (BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID))
                                                                               && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.InwardFacilityLot.FacilityLotID)
                                                                               && (c.NotAvailable == false)
                                                                            select c).Count();
                }
                if (nCount <= 0)
                {
                    /// Falls Standard-Einlagerplatz nicht existiert, breche ab
                    if (BP.ParamsAdjusted.InwardFacilityLocation.Facility1_IncomingFacility == null)
                    {
                        //return BookingResult.Failed;
                    }
                    else
                    {
                        //Local.InwardFacility = InwardFacilityLocation.Facility1_IncomingFacility;
                    }
                }
            }
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!
            return Global.ACMethodResultState.Succeeded;
        }

        /// <summary>
        /// Fall 3. [Facility]:   
        /// Nur bei Silos möglich zur Silobestandsführung
        /// </summary>
        private Global.ACMethodResultState BookingOn_Facility(ACMethodBooking BP)
        {
            return BookingOn_Facility_Common(BP);
        }

        /// <summary>
        /// Fall 3.1. [Facility],[Material]:   
        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        /// Und zum anlegen von Anonymen Chargen, weil zu dem Buchungszeitpunkt noch keine Cahrgeninformation da war
        /// </summary>
        private Global.ACMethodResultState BookingOn_Facility_Material(ACMethodBooking BP)
        {
            return BookingOn_Facility_Common(BP);
        }

        /// <summary>
        /// Fall 3.2. [Facility],[FacilityLot]:  
        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        /// Es werden KEINE Anonynmen Chargen angelegt!
        /// </summary>
        private Global.ACMethodResultState BookingOn_Facility_FacilityLot(ACMethodBooking BP)
        {
            return BookingOn_Facility_Common(BP);
        }

        /// <summary>
        /// Fall 3.3. [Facility],[Material],[FacilityLot]:
        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        /// Es werden KEINE Anonynmen Chargen angelegt, aber mit Materialien die dem übergbenen Material entsprechen!
        /// Falls anonyme Chargen vorhanden sind, dann wird der anonyme Status aufgehoben durch Ersetzung mit FacilityLot
        /// </summary>
        private Global.ACMethodResultState BookingOn_Facility_FacilityLot_Material(ACMethodBooking BP)
        {
            return BookingOn_Facility_Common(BP);
        }


        private Global.ACMethodResultState BookingOn_Facility_Common(ACMethodBooking BP)
        {
            /// TODO: ZeroStock ist erlaubt wenn Silo. Beachte bei Buchungen !!!
            /// 
            /// 
            /// Überprüfe für Quelle und Ziel:
            /// Wenn Silo
            ///     Wenn keine Belegung vorgegeben
            ///         Falls FacilityChargen vorhanden sind
            ///             Autokorrektur Belegung mit Local-Material wenn Erlaubt (Aus Konfig-Parameter und FacilityChargen-Plausibilität)
            ///     Wenn Belegung vorgegeben
            ///         dann muss Belegung mit Local-Material (Local MAterial wird aus FacilityLot gebildet, wenn übergeben) stimmen (Check wurde bereits durch Aufruf von CheckAndAdjustFacility() gemacht!)
            ///     Wenn keine Belegung vorgegeben
            ///         dann Autobelegung mit übergebenen Local-Material wenn Erlaubt
            ///     Wenn noch immer keine Belegung gesetzt
            ///         springe raus
            ///         
            /// Falls FacilityLot übergeben
            ///     Falls kein Material übergeben
            ///         (X):
            ///         Falls Abgang oder Zugang:
            ///             Wenn keine FacilityCharge mit FacilityLot vorhanden
            ///                 dann Neuanlage FacilityCharge aus FacilityLot und local-Material(B)
            ///             Sonst
            ///                 Übergebe Liste an Stackbucher, der die entsprechende Buchungs-Aufteilung zurückgibt
            ///                 Mache Stackbuchung(A1)
            ///         Sonst Umlagerung:
            ///             Wenn keine FacilityCharge mit FacilityLot auf beiden Lagerplätzen vorhanden
            ///                 dann Neuanlage FacilityCharge aus FacilityLot und local-Material(B) auf beiden Plätzen
            ///             Sonst wenn keine FacilityCharge mit FacilityLot auf Quelle vorhanden aber auf Ziel
            ///                 dann Neuanlage FacilityCharge aus FacilityLot und local-Material(B) auf Quelle
            ///                 Mache Umlagerbuchung von Quell-FacilityCharge
            ///             Sonst wenn keine FacilityCharge mit FacilityLot auf Ziel vorhanden aber auf Quelle
            ///                 Übergebe Liste von Quelle an Stackbucher, der die entsprechende Buchungs-Aufteilung zurückgibt
            ///                 Mache Stack-Umlagerbuchungen (A2) mit neuanlage auf Ziel
            ///             Sonst auf beiden vorhanden
            ///                 Übergebe Liste von Quelle an Stackbucher, der die entsprechende Buchungs-Aufteilung zurückgibt
            ///                 Mache Stack-Umlagerbuchungen (A2)
            ///     Sonst
            ///         Mache dasselbe wie oben (X), jedoch gefiltert mit übergebenen Material
            /// Sonst Material übergeben oder nicht übergeben
            ///     Mache dasselbe wie oben (X), jedoch gefiltert mit übergebenen Material ohne FacilityLot

            ///     (Das würde folgendem Algorithmus entsprechen:
            ///     Falls Abgang oder Zugang:
            ///         Wenn keine FacilityCharge mit Local-Material vorhanden
            ///             dann Neuanlage von ANONYMER FacilityCharge  
            ///         Sonst wenn FacilityChargen mit Local-Material vorhanden
            ///             Übergebe Liste an Stackbucher, der die entsprechende Buchungs-Aufteilung zurückgibt
            ///             Mache Stackbuchung(A1)
            ///     Sonst Umlagerung:
            ///          Wenn keine FacilityCharge mit Local-Material auf beiden Lagerplätzen vorhanden
            ///              dann Neuanlage von ANONYMER FacilityCharge aus local-Material(B) auf beiden Plätzen
            ///          Sonst wenn keine FacilityCharge mit Local-Material auf Quelle vorhanden aber auf Ziel
            ///              dann Neuanlage ANONYMER FacilityCharge aus local-Material(B) auf Quelle
            ///              Mache Umlagerbuchung von Quell-FacilityCharge
            ///          Sonst wenn keine FacilityCharge auf Ziel vorhanden aber auf Quelle
            ///              Übergebe Liste von Quelle an Stackbucher, der die entsprechende Buchungs-Aufteilung zurückgibt
            ///              Mache Stack-Umlagerbuchungen (A2) mit neuanlage auf Ziel
            ///          Sonst auf beiden vorhanden
            ///              Übergebe Liste von Quelle an Stackbucher, der die entsprechende Buchungs-Aufteilung zurückgibt
            ///              Mache Stack-Umlagerbuchungen (A2)
            ///    )
            ///    

            Global.ACMethodResultState bookingResult = Global.ACMethodResultState.Succeeded;

            // Liste von FacilityChargen, die nach buchungsparameter FacilityLot und Material gefiltert sind,
            // um herauszufinden ob FacilityCharge überhaupt existiert und evtl. angelegt werden muss.
            FacilityChargeList facilityInwardChargeSubList = null;
            FacilityChargeList facilityOutwardChargeSubList = null;
            // Liste von allen FacilityChargen/Schichten im Silo
            FacilityChargeList cellInwardChargeList = null;
            FacilityChargeList cellOutwardChargeList = null;

            // Überprüfe für Quelle und Ziel:
            if (BP.ParamsAdjusted.InwardFacility != null)
            {
                // Wenn Silo
                if (BP.ParamsAdjusted.InwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                {
                    // Wenn keine Belegung vorgegeben
                    if (BP.ParamsAdjusted.InwardFacility.Material == null)
                    {
                        // Falls FacilityChargen vorhanden sind
                        // Autokorrektur Belegung mit Local-Material wenn Erlaubt (Aus Konfig-Parameter und FacilityChargen-Plausibilität)
                        bookingResult = SetMaterialAssignmentOnFacility(BP, BP.ParamsAdjusted.InwardFacility, BP.ParamsAdjusted.InwardMaterial, 
                                                                        BP.ParamsAdjusted.InwardPartslist);
                        if (bookingResult != Global.ACMethodResultState.Succeeded)
                            return bookingResult;
                    }
                    // Wenn noch immer keine Belegung gesetzt springe raus
                    if (BP.ParamsAdjusted.InwardFacility.Material == null)
                    {
                        BP.AddBookingMessage(ACMethodBooking.eResultCodes.ProhibitedBooking,
                            Root.Environment.TranslateMessage(this, "Error00091",
                                            BP.ParamsAdjusted.InwardFacility.FacilityNo, BP.ParamsAdjusted.InwardFacility.FacilityName,
                                            BP.ParamsAdjusted.InwardMaterial.MaterialNo, BP.ParamsAdjusted.InwardMaterial.MaterialName1));
                        return Global.ACMethodResultState.Notpossible;
                    }
                    // Belegung mit Local-Material (Local MAterial wird aus FacilityLot gebildet, wenn übergeben) stimmen (Check wurde bereits durch Aufruf von CheckAndAdjustFacility() gemacht!)
                    if (!Material.IsMaterialEqual(BP.ParamsAdjusted.InwardFacility.Material,BP.ParamsAdjusted.InwardMaterial))
                    {
                        BP.AddBookingMessage(ACMethodBooking.eResultCodes.ProhibitedBooking,
                            Root.Environment.TranslateMessage(this, "Error00090",
                                            BP.ParamsAdjusted.InwardFacility.FacilityNo, BP.ParamsAdjusted.InwardFacility.FacilityName,
                                            BP.ParamsAdjusted.InwardFacility.Material.MaterialNo, BP.ParamsAdjusted.InwardFacility.Material.MaterialName1,
                                            BP.ParamsAdjusted.InwardMaterial.MaterialNo, BP.ParamsAdjusted.InwardMaterial.MaterialName1));
                        return Global.ACMethodResultState.Notpossible;
                    }

                    // Ermittle Schichten im Silo
                    cellInwardChargeList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                  where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                && (c.NotAvailable == false)
                                            select c);
                }

                // Ermittle FacilityCharge's auf Ziel-lagerplatz 
                if (BP.ParamsAdjusted.IsLotManaged)
                {
                    if ((BP.InwardFacilityLot != null) && (BP.InwardMaterial != null))
                    {
                        if (BP.ParamsAdjusted.InwardPartslist != null)
                        {
                            facilityInwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge
                                                                                 where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                                     && ((c.MaterialID == BP.ParamsAdjusted.InwardMaterial.MaterialID)
                                                                                        || (BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID))
                                                                                     && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.InwardPartslist.PartslistID))
                                                                                     && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.InwardFacilityLot.FacilityLotID)
                                                                                     && (c.NotAvailable == false)
                                                                                 select c);
                        }
                        else
                        {
                            facilityInwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge
                                                                                 where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                                     && ((c.MaterialID == BP.ParamsAdjusted.InwardMaterial.MaterialID)
                                                                                        || (BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID))
                                                                                     && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.InwardFacilityLot.FacilityLotID)
                                                                                     && (c.NotAvailable == false)
                                                                                 select c);
                        }
                    }
                    else if (BP.InwardFacilityLot != null)
                    {
                        if (BP.ParamsAdjusted.InwardPartslist != null)
                        {
                            facilityInwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge
                                                                                 where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                                     && ((c.Partslist != null) && (c.Partslist.PartslistID == BP.ParamsAdjusted.InwardPartslist.PartslistID))
                                                                                     && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.InwardFacilityLot.FacilityLotID)
                                                                                     && (c.NotAvailable == false)
                                                                                 select c);
                        }
                        else
                        {
                            facilityInwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge
                                                                                 where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                                     && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.InwardFacilityLot.FacilityLotID)
                                                                                     && (c.NotAvailable == false)
                                                                                 select c);
                        }
                    }
                    // Keine Materialnummer vorgegeben
                    else
                    {
                        if (BP.ParamsAdjusted.InwardPartslist != null)
                        {
                            facilityInwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge
                                                                                 where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                                     && ((c.Partslist != null) && (c.Partslist.PartslistID == BP.ParamsAdjusted.InwardPartslist.PartslistID))
                                                                                     && (c.NotAvailable == false)
                                                                                 select c);
                        }
                        else
                        {
                            facilityInwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge
                                                                                 where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                                     && (c.NotAvailable == false)
                                                                                 select c);
                        }
                    }
                }
                else
                {
                    // Ermittle FacilityCharge's auf Ziel-lagerplatz 
                    if (BP.ParamsAdjusted.InwardMaterial != null)
                    {
                        if (BP.ParamsAdjusted.InwardPartslist != null)
                        {
                            facilityInwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                 where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                                     && ((c.MaterialID == BP.ParamsAdjusted.InwardMaterial.MaterialID)
                                                                                        || (BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID))
                                                                                     && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.InwardPartslist.PartslistID))
                                                                                     && (c.NotAvailable == false)
                                                                                 select c);
                        }
                        else
                        {
                            facilityInwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                 where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                                     && ((c.MaterialID == BP.ParamsAdjusted.InwardMaterial.MaterialID)
                                                                                        || (BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID))
                                                                                     && (c.NotAvailable == false)
                                                                                 select c);
                        }
                    }
                    // Keine Materialnummer vorgegeben
                    else
                    {
                        if (BP.ParamsAdjusted.InwardPartslist != null)
                        {
                            facilityInwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                 where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                                     && ((c.Partslist != null) && (c.Partslist.PartslistID == BP.ParamsAdjusted.InwardPartslist.PartslistID))
                                                                                     && (c.NotAvailable == false)
                                                                                 select c);
                        }
                        else
                        {
                            facilityInwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                 where (c.Facility.FacilityID == BP.ParamsAdjusted.InwardFacility.FacilityID)
                                                                                     && (c.NotAvailable == false)
                                                                                 select c);
                        }
                    }
                }
            }

            if (BP.ParamsAdjusted.OutwardFacility != null)
            {
                // Wenn Silo
                if (BP.ParamsAdjusted.OutwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                {
                    // Wenn keine Belegung vorgegeben
                    if (BP.ParamsAdjusted.OutwardFacility.Material == null)
                    {
                        // Falls FacilityChargen vorhanden sind
                        // Autokorrektur Belegung mit Local-Material wenn Erlaubt (Aus Konfig-Parameter und FacilityChargen-Plausibilität)
                        bookingResult = SetMaterialAssignmentOnFacility(BP, BP.ParamsAdjusted.OutwardFacility, BP.ParamsAdjusted.OutwardMaterial, 
                                                                        BP.ParamsAdjusted.OutwardPartslist);
                        if (bookingResult != Global.ACMethodResultState.Succeeded)
                            return bookingResult;
                    }
                    // Wenn noch immer keine Belegung gesetzt springe raus
                    if (BP.ParamsAdjusted.OutwardFacility.Material == null)
                    {
                        BP.AddBookingMessage(ACMethodBooking.eResultCodes.ProhibitedBooking,
                            Root.Environment.TranslateMessage(this, "Error00105",
                                            BP.ParamsAdjusted.OutwardFacility.FacilityNo, BP.ParamsAdjusted.OutwardFacility.FacilityName,
                                            BP.ParamsAdjusted.OutwardMaterial.MaterialNo, BP.ParamsAdjusted.OutwardMaterial.MaterialName1));
                        return Global.ACMethodResultState.Notpossible;
                    }
                    // Belegung mit Local-Material (Local MAterial wird aus FacilityLot gebildet, wenn übergeben) stimmen (Check wurde bereits durch Aufruf von CheckAndAdjustFacility() gemacht!)
                    if (!Material.IsMaterialEqual(BP.ParamsAdjusted.OutwardFacility.Material,BP.ParamsAdjusted.OutwardMaterial))
                    {
                        BP.AddBookingMessage(ACMethodBooking.eResultCodes.ProhibitedBooking,
                            Root.Environment.TranslateMessage(this, "Error00104",
                                            BP.ParamsAdjusted.OutwardFacility.FacilityNo, BP.ParamsAdjusted.OutwardFacility.FacilityName,
                                            BP.ParamsAdjusted.OutwardFacility.Material.MaterialNo, BP.ParamsAdjusted.OutwardFacility.Material.MaterialName1,
                                            BP.ParamsAdjusted.OutwardMaterial.MaterialNo, BP.ParamsAdjusted.OutwardMaterial.MaterialName1));
                        return Global.ACMethodResultState.Notpossible;
                    }

                    // Ermittle Schichten im Silo
                    cellOutwardChargeList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                   where (c.Facility.FacilityID == BP.ParamsAdjusted.OutwardFacility.FacilityID)
                                                 && (c.NotAvailable == false)
                                             select c);
                }

                if (BP.ParamsAdjusted.IsLotManaged)
                {
                    // Ermittle FacilityCharge's auf Quelllagerplatz 
                    if ((BP.OutwardFacilityLot != null) && (BP.OutwardMaterial != null))
                    {
                        if (BP.ParamsAdjusted.OutwardPartslist != null)
                        {
                            facilityOutwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                  where (c.Facility.FacilityID == BP.ParamsAdjusted.OutwardFacility.FacilityID)
                                                                                      && ((c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.MaterialID)
                                                                                       || (BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID))
                                                                                      && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.OutwardPartslist.PartslistID))
                                                                                      && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityLot.FacilityLotID)
                                                                                      && (c.NotAvailable == false)
                                                                                  select c);
                        }
                        else
                        {
                            facilityOutwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                  where (c.Facility.FacilityID == BP.ParamsAdjusted.OutwardFacility.FacilityID)
                                                                                      && ((c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.MaterialID)
                                                                                        || (BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID))
                                                                                      && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityLot.FacilityLotID)
                                                                                      && (c.NotAvailable == false)
                                                                                  select c);
                        }
                    }
                    else if (BP.OutwardFacilityLot != null)
                    {
                        if (BP.ParamsAdjusted.OutwardPartslist != null)
                        {
                            facilityOutwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                  where (c.Facility.FacilityID == BP.ParamsAdjusted.OutwardFacility.FacilityID)
                                                                                      && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityLot.FacilityLotID)
                                                                                      && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.OutwardPartslist.PartslistID))
                                                                                      && (c.NotAvailable == false)
                                                                                  select c);
                        }
                        else
                        {
                            facilityOutwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                  where (c.Facility.FacilityID == BP.ParamsAdjusted.OutwardFacility.FacilityID)
                                                                                      && (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityLot.FacilityLotID)
                                                                                      && (c.NotAvailable == false)
                                                                                  select c);
                        }
                    }
                    // Kein Material vorgegeben
                    else
                    {
                        if (BP.ParamsAdjusted.OutwardPartslist != null)
                        {
                            facilityOutwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                  where (c.Facility.FacilityID == BP.ParamsAdjusted.OutwardFacility.FacilityID)
                                                                                      && ((c.Partslist != null) && (c.Partslist.PartslistID == BP.ParamsAdjusted.OutwardPartslist.PartslistID))
                                                                                      && (c.NotAvailable == false)
                                                                                  select c);
                        }
                        else
                        {
                            facilityOutwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                  where (c.Facility.FacilityID == BP.ParamsAdjusted.OutwardFacility.FacilityID)
                                                                                      && (c.NotAvailable == false)
                                                                                  select c);
                        }
                    }
                }
                else
                {
                    // Ermittle FacilityCharge's auf Quelllagerplatz 
                    if (BP.ParamsAdjusted.OutwardMaterial != null)
                    {
                        if (BP.ParamsAdjusted.OutwardPartslist != null)
                        {
                            facilityOutwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                  where (c.Facility.FacilityID == BP.ParamsAdjusted.OutwardFacility.FacilityID)
                                                                                      && ((c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.MaterialID)
                                                                                         || (BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID))
                                                                                      && ((c.Partslist != null) && (c.PartslistID == BP.ParamsAdjusted.OutwardPartslist.PartslistID))
                                                                                      && (c.NotAvailable == false)
                                                                                  select c);
                        }
                        else
                        {
                            facilityOutwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                  where (c.Facility.FacilityID == BP.ParamsAdjusted.OutwardFacility.FacilityID)
                                                                                      && ((c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.MaterialID)
                                                                                        || (BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID))
                                                                                      && (c.NotAvailable == false)
                                                                                  select c);
                        }
                    }
                    // Kein Material vorgegeben
                    else
                    {
                        if (BP.ParamsAdjusted.OutwardPartslist != null)
                        {
                            facilityOutwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                  where (c.Facility.FacilityID == BP.ParamsAdjusted.OutwardFacility.FacilityID)
                                                                                      && ((c.Partslist != null) && (c.Partslist.PartslistID == BP.ParamsAdjusted.OutwardPartslist.PartslistID))
                                                                                      && (c.NotAvailable == false)
                                                                                  select c);
                        }
                        else
                        {
                            facilityOutwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot)
                                                                                  where (c.Facility.FacilityID == BP.ParamsAdjusted.OutwardFacility.FacilityID)
                                                                                      && (c.NotAvailable == false)
                                                                                  select c);
                        }
                    }
                }
            }

            // Generiere Stack-Calculator
            if (BP.StackCalculatorInward(this) == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.WrongConfigurationInMaterialManagement,
                                                                    Root.Environment.TranslateMessage(this, "Error00014"));
                return Global.ACMethodResultState.Notpossible;
            }

            if (BP.StackCalculatorOutward(this) == null)
            {
                BP.AddBookingMessage(ACMethodBooking.eResultCodes.WrongConfigurationInMaterialManagement,
                                                                    Root.Environment.TranslateMessage(this, "Error00014"));
                return Global.ACMethodResultState.Notpossible;
            }

            FacilityBooking FB = BP.FacilityBooking != null ? BP.FacilityBooking : NewFacilityBooking(BP);

            // Umbuchung auf eine andere Materialnummer
            if (BP.OutwardFacility != null && BP.InwardMaterial != null && BP.BookingType == GlobalApp.FacilityBookingType.Reassign_Facility_BulkMaterial)
            {
                foreach (FacilityCharge outwardFacilityCharge in cellOutwardChargeList)
                {
                    // Auslagerungsbuchung auf altem Material
                    FacilityBookingCharge FBC = NewFacilityBookingCharge(BP, false);

                    BP.ParamsAdjusted.MDBookingNotAvailableMode = DatabaseApp.s_cQry_GetMDBookingNotAvailableMode(BP.DatabaseApp, MDBookingNotAvailableMode.BookingNotAvailableModes.AutoSetAndReset).FirstOrDefault();
                    double quantityToBook = outwardFacilityCharge.StockQuantity;
                    bookingResult = InitFacilityBookingCharge_FromFacilityCharge_Outward(BP, FBC, outwardFacilityCharge,
                                        false,
                                        quantityToBook, quantityToBook, outwardFacilityCharge.MDUnit);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    bookingResult = BookFacilityBookingChargeOut(BP, FBC, cellOutwardChargeList);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;


                    // Einlagerungsbuchung auf neues Material, Suche ob Quant im Lager vorhanden sonst lege neues Quant an
                    FBC = NewFacilityBookingCharge(BP, false);

                    FacilityCharge inwardFacilityCharge = BP.DatabaseApp.FacilityCharge.Where(c => c.FacilityID == outwardFacilityCharge.FacilityID
                                                    && c.MaterialID == BP.InwardMaterial.MaterialID
                                                    && c.FacilityLotID == outwardFacilityCharge.FacilityLotID
                                                    && c.SplitNo == outwardFacilityCharge.SplitNo).FirstOrDefault();
                    if (inwardFacilityCharge == null)
                    {
                        inwardFacilityCharge = FacilityCharge.NewACObject(BP.DatabaseApp, null);
                        inwardFacilityCharge.CloneFrom(outwardFacilityCharge, false);
                        inwardFacilityCharge.Material = BP.InwardMaterial;
                        inwardFacilityCharge.NotAvailable = false;
                        if (BP.InwardPartslist != null)
                            inwardFacilityCharge.Partslist = BP.InwardPartslist;
                        inwardFacilityCharge.AddToParentsList();
                    }
                    else if (inwardFacilityCharge.NotAvailable)
                        inwardFacilityCharge.NotAvailable = false;
                    if (BP.InwardPartslist != null)
                        inwardFacilityCharge.Partslist = BP.InwardPartslist;
                    else
                        inwardFacilityCharge.Partslist = null;

                    bookingResult = InitFacilityBookingCharge_FromFacilityCharge_Inward(BP, FBC, inwardFacilityCharge,
                        false,
                        quantityToBook, quantityToBook, outwardFacilityCharge.MDUnit);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    bookingResult = BookFacilityBookingChargeIn(BP, FBC, cellInwardChargeList);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;
                }

                BP.OutwardFacility.Material = BP.InwardMaterial;
                BP.OutwardFacility.Partslist = BP.InwardPartslist;
            }
            // Falls Abgang oder Zugang:
            else if ((BP.InwardFacility == null) || (BP.OutwardFacility == null))
            {
                // Falls Zugang
                if (BP.ParamsAdjusted.InwardFacility != null)
                {
                    // TODO: Untersuchung ob Silo oder Lagerpatz, dementsprechend volle oder teilliste dem Stackbucher übergeben
                    // Wenn keine FacilityCharge mit FacilityLot vorhanden
                    if (!facilityInwardChargeSubList.Any())
                    {
                        FacilityCharge InwardFacilityCharge = null;
                        // Falls alte FacilityCharge rückgesetzt werden kann, wenn im Lager als Nichtvorhanden vorliegt
                        if (BP.IsAutoResetNotAvailable || !BP.ParamsAdjusted.IsLotManaged)
                            InwardFacilityCharge = TryReactivateInwardFacilityCharge(BP);
                        // dann Neuanlage FacilityCharge aus local-Material(B) entweder mit oder ohne FacilityLot (AnonymeCharge)
                        if (InwardFacilityCharge == null)
                            InwardFacilityCharge = FacilityCharge.NewACObject(BP.DatabaseApp, null);
                        InwardFacilityCharge.CloneFrom(BP.DatabaseApp, BP.ParamsAdjusted.InwardMaterial, BP.ParamsAdjusted.InwardFacility, BP.ParamsAdjusted.InwardFacilityLot, BP.ParamsAdjusted.InwardPartslist, true);
                        InwardFacilityCharge.NotAvailable = false;
                        InwardFacilityCharge.Partslist = BP.ParamsAdjusted.InwardPartslist;
                        // Einlagerdatum + Eindeutige Reihenfolgennumer der Einlagerung
                        InwardFacilityCharge.FillingDate = DateTime.Now;
                        InwardFacilityCharge.FacilityChargeSortNo = InwardFacilityCharge.Facility.GetNextFCSortNo(BP.DatabaseApp);

                        FacilityBookingCharge FBC = NewFacilityBookingCharge(BP, false);
                        bookingResult = InitFacilityBookingCharge_FromBookingParameter_Inward(BP, FBC, InwardFacilityCharge);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        bookingResult = BookFacilityBookingChargeIn(BP, FBC, cellInwardChargeList);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        facilityInwardChargeSubList.Add(InwardFacilityCharge);
                        if (cellInwardChargeList != null)
                            cellInwardChargeList.Add(InwardFacilityCharge);
                    }
                    // FacilityChargen vorhanden
                    else
                    {
                        Double stackQuantity = 0;

                        bookingResult = ConvertQuantityToMaterialBaseUnit(BP, BP.ParamsAdjusted.InwardQuantity, BP.ParamsAdjusted.MDUnit, BP.ParamsAdjusted.InwardMaterial, ref stackQuantity);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        // Ermittle Buchungsmenge
                        if ((BP.ParamsAdjusted.QuantityIsAbsolute == true) && (BP.ParamsAdjusted.InwardFacility.CurrentFacilityStock != null))
                        {
                            stackQuantity = stackQuantity - BP.ParamsAdjusted.InwardFacility.CurrentFacilityStock.StockQuantity;
                        }

                        StackItemList stackItemListInOut;
                        MsgBooking msgBookingInOut;
                        // Booking always on ChargeSubList, depends on parameter which were passed over ACMethodBooking
                        //if (BP.ParamsAdjusted.InwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                        //{
                        //    // Übergebe komplette Liste mit Schichten an Stackbucher, der die entsprechende Buchungs-Aufteilung zurückgibt
                        //    bookingResult = StackCalculatorInward.CalculateInOut(true,
                        //                                            BP.ParamsAdjusted.ShiftBookingReverse,
                        //                                            BP.ParamsAdjusted.DontAllowNegativeStock == true ? false : true,
                        //                                            stackQuantity, BP.ParamsAdjusted.InwardMaterial.BaseMDUnit,
                        //                                            cellInwardChargeList, BP);
                        //}
                        //else
                        {
                            // Übergebe gefilterte Liste an Stackbucher, der die entsprechende Buchungs-Aufteilung zurückgibt
                            bookingResult = BP.StackCalculatorInward(this).CalculateInOut(true,
                                                                    BP.ParamsAdjusted.ShiftBookingReverse,
                                                                    BP.ParamsAdjusted.DontAllowNegativeStock == true ? false : true,
                                                                    stackQuantity, BP.ParamsAdjusted.InwardMaterial.BaseMDUnit,
                                                                    facilityInwardChargeSubList, BP, out stackItemListInOut, out msgBookingInOut);
                        }
                        BP.Merge(msgBookingInOut);
                        if (bookingResult != Global.ACMethodResultState.Succeeded)
                        {
                            return Global.ACMethodResultState.Notpossible;
                        }

                        // Mache Stackbuchung
                        bookingResult = DoInwardStackBooking(BP, stackItemListInOut, facilityInwardChargeSubList);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        // Falls Nullbestand und die Summe der Facility-Charge-Abbuchungen nicht dazu geführt haben, dass der Facility-Bestand 0 ergibt,
                        // setze Facility-Bestand auf 0
                        if ((BP.ParamsAdjusted.MDZeroStockState != null)
                                && ((BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.BookToZeroStock)
                                    || (BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.SetNotAvailable)))
                        {
                            if (BP.ParamsAdjusted.InwardFacility.CurrentFacilityStock != null)
                            {
                                if (Math.Abs(0 - BP.ParamsAdjusted.InwardFacility.CurrentFacilityStock.StockQuantity) > Double.Epsilon)
                                    BP.ParamsAdjusted.InwardFacility.CurrentFacilityStock.StockQuantity = 0;
                            }
                        }

                    }
                }
                // Sonst Abgang
                else
                {
                    // Wenn keine FacilityCharge mit FacilityLot vorhanden
                    if (!facilityOutwardChargeSubList.Any())
                    {
                        FacilityCharge OutwardFacilityCharge = null;
                        // Falls alte FacilityCharge rückgesetzt werden kann, wenn im Lager als Nichtvorhanden vorliegt
                        if (BP.IsAutoResetNotAvailable || !BP.ParamsAdjusted.IsLotManaged)
                            OutwardFacilityCharge = TryReactivateOutwardFacilityCharge(BP);
                        // dann Neuanlage FacilityCharge aus local-Material(B) entweder mit oder ohne FacilityLot (AnonymeCharge)
                        if (OutwardFacilityCharge == null)
                            OutwardFacilityCharge = FacilityCharge.NewACObject(BP.DatabaseApp, null);
                        OutwardFacilityCharge.CloneFrom(BP.DatabaseApp, BP.ParamsAdjusted.OutwardMaterial, BP.ParamsAdjusted.OutwardFacility, BP.ParamsAdjusted.OutwardFacilityLot, BP.ParamsAdjusted.OutwardPartslist, true);
                        OutwardFacilityCharge.NotAvailable = false;
                        OutwardFacilityCharge.Partslist = BP.ParamsAdjusted.OutwardPartslist;
                        // Einlagerdatum + Eindeutige Reihenfolgennumer der Einlagerung
                        OutwardFacilityCharge.FillingDate = DateTime.Now;
                        OutwardFacilityCharge.FacilityChargeSortNo = OutwardFacilityCharge.Facility.GetNextFCSortNo(BP.DatabaseApp);

                        FacilityBookingCharge FBC = NewFacilityBookingCharge(BP, false);
                        bookingResult = InitFacilityBookingCharge_FromBookingParameter_Outward(BP, FBC, OutwardFacilityCharge);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        bookingResult = BookFacilityBookingChargeOut(BP, FBC, cellOutwardChargeList);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        if (facilityOutwardChargeSubList != null)
                            facilityOutwardChargeSubList.Add(OutwardFacilityCharge);
                        if (cellOutwardChargeList != null)
                            cellOutwardChargeList.Add(OutwardFacilityCharge);
                    }
                    // FacilityChargen vorhanden
                    else
                    {
                        Double stackQuantity = 0;

                        bookingResult = ConvertQuantityToMaterialBaseUnit(BP, FB.OutwardQuantity, BP.ParamsAdjusted.MDUnit, BP.ParamsAdjusted.OutwardMaterial, ref stackQuantity);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        // Ermittle Buchungsmenge
                        if ((BP.ParamsAdjusted.QuantityIsAbsolute == true) && (BP.ParamsAdjusted.OutwardFacility.CurrentFacilityStock != null))
                        {
                            stackQuantity = BP.ParamsAdjusted.OutwardFacility.CurrentFacilityStock.StockQuantity - stackQuantity;
                        }

                        StackItemList stackItemListInOut;
                        MsgBooking msgBookingInOut;
                        // Booking always on ChargeSubList, depends on parameter which were passed over ACMethodBooking
                        //if (BP.ParamsAdjusted.OutwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                        //{
                        //    // Übergebe komplette Liste mit Schichten an Stackbucher, der die entsprechende Buchungs-Aufteilung zurückgibt
                        //    bookingResult = StackCalculatorOutward.CalculateInOut(false,
                        //                                                            BP.ParamsAdjusted.ShiftBookingReverse,
                        //                                                            BP.ParamsAdjusted.DontAllowNegativeStock == true ? false : true,
                        //                                                            stackQuantity, BP.ParamsAdjusted.OutwardMaterial.BaseMDUnit,
                        //                                                            cellOutwardChargeList, BP);
                        //}
                        //else
                        {
                            // Übergebe Liste an Stackbucher, der die entsprechende Buchungs-Aufteilung zurückgibt
                            bookingResult = BP.StackCalculatorOutward(this).CalculateInOut(false,
                                                                                    BP.ParamsAdjusted.ShiftBookingReverse,
                                                                                    BP.ParamsAdjusted.DontAllowNegativeStock == true ? false : true,
                                                                                    stackQuantity, BP.ParamsAdjusted.OutwardMaterial.BaseMDUnit,
                                                                                    facilityOutwardChargeSubList, BP, out stackItemListInOut, out msgBookingInOut);
                        }
                        BP.Merge(msgBookingInOut);
                        if (bookingResult != Global.ACMethodResultState.Succeeded)
                        {
                            return Global.ACMethodResultState.Notpossible;
                        }

                        // Mache Stackbuchung
                        bookingResult = DoOutwardStackBooking(BP, stackItemListInOut, facilityOutwardChargeSubList);
                        if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                            return bookingResult;

                        // Falls Nullbestand und die Summe der Facility-Charge-Abbuchungen nicht dazu geführt haben, dass der Facility-Bestand 0 ergibt,
                        // setze Facility-Bestand auf 0
                        if ((BP.ParamsAdjusted.MDZeroStockState != null)
                                && ((BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.BookToZeroStock)
                                    || (BP.ParamsAdjusted.MDZeroStockState.ZeroStockState == MDZeroStockState.ZeroStockStates.SetNotAvailable)))
                        {
                            if (BP.ParamsAdjusted.OutwardFacility.CurrentFacilityStock != null)
                            {
                                if (Math.Abs(0 - BP.ParamsAdjusted.OutwardFacility.CurrentFacilityStock.StockQuantity) > Double.Epsilon)
                                    BP.ParamsAdjusted.OutwardFacility.CurrentFacilityStock.StockQuantity = 0;
                            }
                        }

                    }
                }
            }
            // Sonst Umlagerung
            else
            {
                FacilityCharge outwardFacilityCharge = null;
                FacilityCharge inwardFacilityCharge = null;
                bool outwardFCListWasEmpty = !facilityOutwardChargeSubList.Any();
                bool inwardFCListWasEmpty = !facilityInwardChargeSubList.Any();
                if (outwardFCListWasEmpty)
                {
                    // Falls alte FacilityCharge rückgesetzt werden kann, wenn im Lager als Nichtvorhanden vorliegt
                    if (BP.IsAutoResetNotAvailable || !BP.ParamsAdjusted.IsLotManaged)
                        outwardFacilityCharge = TryReactivateOutwardFacilityCharge(BP);
                    // dann Neuanlage FacilityCharge aus local-Material(B) entweder mit oder ohne FacilityLot (AnonymeCharge)
                    if (outwardFacilityCharge == null)
                        outwardFacilityCharge = FacilityCharge.NewACObject(BP.DatabaseApp, null);
                    outwardFacilityCharge.CloneFrom(BP.DatabaseApp, BP.ParamsAdjusted.OutwardMaterial, BP.ParamsAdjusted.OutwardFacility, BP.ParamsAdjusted.OutwardFacilityLot, BP.ParamsAdjusted.OutwardPartslist, true);
                    outwardFacilityCharge.NotAvailable = false;
                    outwardFacilityCharge.Partslist = BP.ParamsAdjusted.OutwardPartslist;
                    // Einlagerdatum + Eindeutige Reihenfolgennumer der Einlagerung
                    outwardFacilityCharge.FillingDate = DateTime.Now;
                    outwardFacilityCharge.FacilityChargeSortNo = outwardFacilityCharge.Facility.GetNextFCSortNo(BP.DatabaseApp);
                    facilityOutwardChargeSubList.Add(outwardFacilityCharge);
                }
                if (inwardFCListWasEmpty && !BP.ParamsAdjusted.IsLotManaged)

                {
                    // Falls alte FacilityCharge rückgesetzt werden kann, wenn im Lager als Nichtvorhanden vorliegt
                    if (BP.IsAutoResetNotAvailable || !BP.ParamsAdjusted.IsLotManaged)
                        inwardFacilityCharge = TryReactivateInwardFacilityCharge(BP);
                    // dann Neuanlage FacilityCharge aus FacilityLot und local-Material(B) auf beiden Plätzen
                    // dann Neuanlage FacilityCharge aus local-Material(B) entweder mit oder ohne FacilityLot (AnonymeCharge)
                    if (inwardFacilityCharge == null)
                        inwardFacilityCharge = FacilityCharge.NewACObject(BP.DatabaseApp, null);
                    inwardFacilityCharge.CloneFrom(BP.DatabaseApp, BP.ParamsAdjusted.InwardMaterial, BP.ParamsAdjusted.InwardFacility, BP.ParamsAdjusted.InwardFacilityLot, BP.ParamsAdjusted.InwardPartslist, true);
                    inwardFacilityCharge.NotAvailable = false;
                    inwardFacilityCharge.Partslist = BP.ParamsAdjusted.InwardPartslist;
                    // Einlagerdatum + Eindeutige Reihenfolgennumer der Einlagerung
                    inwardFacilityCharge.FillingDate = DateTime.Now;
                    inwardFacilityCharge.FacilityChargeSortNo = inwardFacilityCharge.Facility.GetNextFCSortNo(BP.DatabaseApp);
                    facilityInwardChargeSubList.Add(inwardFacilityCharge);
                }

                // If only one quant has to be relocated AND on destination is no quant or is not lot-managed
                // then only quant has to be posted on both sides
                if (outwardFCListWasEmpty && (inwardFCListWasEmpty || !BP.ParamsAdjusted.IsLotManaged))
                {
                    if (inwardFacilityCharge == null)
                        inwardFacilityCharge = facilityInwardChargeSubList.FirstOrDefault();
                    // Abgangsbuchung
                    FacilityBookingCharge FBC = NewFacilityBookingCharge(BP, false);
                    bookingResult = InitFacilityBookingCharge_FromBookingParameter_Outward(BP, FBC, outwardFacilityCharge);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    bookingResult = BookFacilityBookingChargeOut(BP, FBC, cellOutwardChargeList);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    // Zugangsbuchung
                    FBC = NewFacilityBookingCharge(BP, false);
                    bookingResult = InitFacilityBookingCharge_FromBookingParameter_Inward(BP, FBC, inwardFacilityCharge);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    bookingResult = BookFacilityBookingChargeIn(BP, FBC, cellInwardChargeList);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    //if (cellInwardChargeList != null)
                    //    cellInwardChargeList.Add(inwardFacilityCharge);
                }
                else
                {
                    if (cellOutwardChargeList != null && outwardFacilityCharge != null)
                        cellOutwardChargeList.Add(outwardFacilityCharge);
                    if (cellInwardChargeList != null && inwardFacilityCharge != null)
                        cellInwardChargeList.Add(inwardFacilityCharge);

                    Double stackQuantity = 0;

                    bookingResult = ConvertQuantityToMaterialBaseUnit(BP, FB.OutwardQuantity, BP.ParamsAdjusted.MDUnit, BP.ParamsAdjusted.OutwardMaterial, ref stackQuantity);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    // Ermittle Buchungsmenge
                    if ((BP.ParamsAdjusted.QuantityIsAbsolute == true) && (BP.ParamsAdjusted.OutwardFacility.CurrentFacilityStock != null))
                    {
                        stackQuantity = BP.ParamsAdjusted.OutwardFacility.CurrentFacilityStock.StockQuantity - stackQuantity;
                    }

                    StackItemList stackItemListInOut;
                    MsgBooking msgBookingInOut;
                    if (BP.ParamsAdjusted.OutwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                    {
                        // Übergebe komplette Liste mit Schichten an Stackbucher, der die entsprechende Buchungs-Aufteilung zurückgibt
                        bookingResult = BP.StackCalculatorOutward(this).CalculateInOut(false,
                                                                                BP.ParamsAdjusted.ShiftBookingReverse,
                                                                                BP.ParamsAdjusted.DontAllowNegativeStock == true ? false : true,
                                                                                stackQuantity, BP.ParamsAdjusted.OutwardMaterial.BaseMDUnit,
                                                                                cellOutwardChargeList, BP, out stackItemListInOut, out msgBookingInOut);
                    }
                    else
                    {
                        // Übergebe Liste an Stackbucher, der die entsprechende Buchungs-Aufteilung zurückgibt
                        bookingResult = BP.StackCalculatorOutward(this).CalculateInOut(false,
                                                                                BP.ParamsAdjusted.ShiftBookingReverse,
                                                                                BP.ParamsAdjusted.DontAllowNegativeStock == true ? false : true,
                                                                                stackQuantity, BP.ParamsAdjusted.OutwardMaterial.BaseMDUnit,
                                                                                facilityOutwardChargeSubList, BP, out stackItemListInOut, out msgBookingInOut);
                    }

                    BP.Merge(msgBookingInOut);
                    if (bookingResult != Global.ACMethodResultState.Succeeded)
                    {
                        return Global.ACMethodResultState.Notpossible;
                    }

                    // Übergebe Ergebnis-Liste an Stackbucher zur Umlagerung
                    bookingResult = CheckOutwardStackBooking(BP, stackItemListInOut);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    StackItemList stackItemListRelocation;
                    MsgBooking msgBookingRelocation;
                    if (BP.ParamsAdjusted.InwardFacility.MDFacilityType.FacilityType == FacilityTypesEnum.StorageBinContainer)
                    {
                        // Übergebe komplette Liste mit Schichten an Stackbucher, der die entsprechende Buchungs-Aufteilung zurückgibt
                        bookingResult = BP.StackCalculatorInward(this).CalculateRelocation(BP.ParamsAdjusted.ShiftBookingReverse,
                                                                                BP.ParamsAdjusted.DontAllowNegativeStock == true ? false : true,
                                                                                stackItemListInOut,
                                                                                cellInwardChargeList, BP, out stackItemListRelocation, out msgBookingRelocation);
                    }
                    else
                    {
                        bookingResult = BP.StackCalculatorInward(this).CalculateRelocation(BP.ParamsAdjusted.ShiftBookingReverse,
                                                                                BP.ParamsAdjusted.DontAllowNegativeStock == true ? false : true,
                                                                                stackItemListInOut,
                                                                                facilityInwardChargeSubList, BP, out stackItemListRelocation, out msgBookingRelocation);
                    }

                    BP.Merge(msgBookingRelocation);
                    if (bookingResult != Global.ACMethodResultState.Succeeded)
                    {
                        return Global.ACMethodResultState.Notpossible;
                    }

                    bookingResult = DoOutwardStackBooking(BP, stackItemListInOut, facilityOutwardChargeSubList);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;

                    bookingResult = DoInwardStackBooking(BP, stackItemListRelocation, facilityOutwardChargeSubList);
                    if ((bookingResult == Global.ACMethodResultState.Failed) || (bookingResult == Global.ACMethodResultState.Notpossible))
                        return bookingResult;
                }
            }

            return bookingResult;
        }

        private FacilityCharge TryReactivateOutwardFacilityCharge(ACMethodBooking BP)
        {
            return TryReactivateFacilityCharge(BP, BP.ParamsAdjusted.OutwardMaterial, BP.ParamsAdjusted.OutwardFacility, BP.ParamsAdjusted.OutwardFacilityLot, BP.ParamsAdjusted.OutwardPartslist);
        }

        private FacilityCharge TryReactivateInwardFacilityCharge(ACMethodBooking BP)
        {
            return TryReactivateFacilityCharge(BP, BP.ParamsAdjusted.InwardMaterial, BP.ParamsAdjusted.InwardFacility, BP.ParamsAdjusted.InwardFacilityLot, BP.ParamsAdjusted.InwardPartslist);
        }

        private FacilityCharge TryReactivateFacilityCharge(ACMethodBooking BP, Material material, Facility facility, FacilityLot lot, Partslist partsList)
        {
            FacilityCharge InwardFacilityCharge = null;
            IQueryable<FacilityCharge> queryExistOld = null;
            if (lot != null && partsList != null)
            {
                queryExistOld = from c in BP.DatabaseApp.FacilityCharge
                                where
                                (c.MaterialID == material.MaterialID)
                                && (c.Facility.FacilityID == facility.FacilityID)
                                && (c.FacilityLot != null && c.FacilityLotID == lot.FacilityLotID)
                                && (c.Partslist != null && (c.PartslistID == partsList.PartslistID))
                                && (c.NotAvailable == true)
                                orderby c.FillingDate descending
                                orderby c.FacilityChargeSortNo descending
                                select c;
            }
            else if (lot != null && partsList == null)
            {
                queryExistOld = from c in BP.DatabaseApp.FacilityCharge
                                where
                                (c.MaterialID == material.MaterialID)
                                && (c.Facility.FacilityID == facility.FacilityID)
                                && (c.FacilityLot != null && c.FacilityLotID == lot.FacilityLotID)
                                && (c.Partslist == null)
                                && (c.NotAvailable == true)
                                orderby c.FillingDate descending
                                orderby c.FacilityChargeSortNo descending
                                select c;
            }
            else if (lot == null && partsList != null)
            {
                queryExistOld = from c in BP.DatabaseApp.FacilityCharge
                                where
                                (c.MaterialID == material.MaterialID)
                                && (c.Facility.FacilityID == facility.FacilityID)
                                && (c.FacilityLot == null)
                                && (c.Partslist != null && (c.PartslistID == partsList.PartslistID))
                                && (c.NotAvailable == true)
                                orderby c.FillingDate descending
                                orderby c.FacilityChargeSortNo descending
                                select c;
            }
            else
            {
                if (material.IsLotManaged)
                {
                    queryExistOld = from c in BP.DatabaseApp.FacilityCharge
                                    where
                                    (c.MaterialID == material.MaterialID)
                                    && (c.Facility.FacilityID == facility.FacilityID)
                                    && (c.FacilityLot != null)
                                    //&& (c.Partslist == null)
                                    && (c.NotAvailable == true)
                                    orderby c.FillingDate descending
                                    orderby c.FacilityChargeSortNo descending
                                    select c;
                }
                else
                {
                    queryExistOld = from c in BP.DatabaseApp.FacilityCharge
                                    where
                                    (c.MaterialID == material.MaterialID)
                                    && (c.Facility.FacilityID == facility.FacilityID)
                                    //&& (c.FacilityLot == null)
                                    //&& (c.Partslist == null)
                                    && (c.NotAvailable == true)
                                    orderby c.FillingDate descending
                                    orderby c.FacilityChargeSortNo descending
                                    select c;
                }
            }
            InwardFacilityCharge = queryExistOld.FirstOrDefault();
            return InwardFacilityCharge;
        }


        /// <summary>
        /// 4. [FacilityLot]:                              
        /// Zur Stackbuchung wenn FacilityChargen von FacilityLot vorhanden
        /// </summary>
        private Global.ACMethodResultState BookingOn_FacilityLot(ACMethodBooking BP)
        {
            ///     Pseudo-code:
            ///     Falls keine FacilityChargen von FacilityLot vorhanden
            ///         Breche ab mit Fehler, weil anonyme Chargen nicht angelegt werden können weil keine Lagerinfo vorhanden ist
            if ((BP.ParamsAdjusted.OutwardFacilityLot == null) && (BP.ParamsAdjusted.InwardFacilityLot == null))
                return Global.ACMethodResultState.Failed;
            if ((BP.ParamsAdjusted.OutwardFacilityLot != null) && (BP.ParamsAdjusted.InwardFacilityLot != null))
            {
                return Global.ACMethodResultState.Failed;
            }

            if (BP.OutwardFacilityLot != null)
            {
                if (!(from c in BP.DatabaseApp.FacilityCharge
                     where (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityLot.FacilityLotID)
                          && (c.NotAvailable == false)
                     select c).Any())
                {
                    return Global.ACMethodResultState.Failed;
                }
            }
            if (BP.InwardFacilityLot != null)
            {
                if (!(from c in BP.DatabaseApp.FacilityCharge
                     where (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.InwardFacilityLot.FacilityLotID)
                            && (c.NotAvailable == false)
                     select c).Any())
                {
                    return Global.ACMethodResultState.Failed;
                }
            }
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!
            return Global.ACMethodResultState.Succeeded;
        }

        /// <summary>
        /// 4.1.[FacilityLot],[Material]:			        
        /// Zur Stackbuchung wenn FacilityCharge mit Materialnummer vorhanden
        /// </summary>
        private Global.ACMethodResultState BookingOn_FacilityLot_Material(ACMethodBooking BP)
        {
            ///     Pseudo-code:
            ///     Falls keine FacilityChargen von FacilityLot und Material vorhanden
            ///         Breche ab mit Fehler, weil anonyme Chargen nicht angelegt werden können weil keine Lagerinfo vorhanden ist
            if ((BP.ParamsAdjusted.OutwardMaterial == null) && (BP.ParamsAdjusted.InwardMaterial == null))
                return Global.ACMethodResultState.Failed;
            if (BP.OutwardFacilityLot != null)
            {
                if (!(from c in BP.DatabaseApp.FacilityCharge
                     where (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.OutwardFacilityLot.FacilityLotID)
                        && ((c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.MaterialID)
                           || (BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID))
                        && (c.NotAvailable == false)
                     select c).Any())
                {
                    return Global.ACMethodResultState.Failed;
                }
            }
            if (BP.InwardFacilityLot != null)
            {
                if (!(from c in BP.DatabaseApp.FacilityCharge
                     where (c.FacilityLot != null && c.FacilityLotID == BP.ParamsAdjusted.InwardFacilityLot.FacilityLotID)
                        && ((c.MaterialID == BP.ParamsAdjusted.InwardMaterial.MaterialID)
                            || (BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID))
                        && (c.NotAvailable == false)
                     select c).Any())
                {
                    return Global.ACMethodResultState.Failed;
                }
            }
            return Global.ACMethodResultState.Succeeded;
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!
        }
        #endregion

        #region Booking on NOT Lot-Managed Materials

        private Global.ACMethodResultState BookingOn_FacilityCharge_NotLotManaged(ACMethodBooking BP)
        {
            return BookingOn_FacilityCharge(BP);
        }

        /// <summary>
        /// 5. [FacilityLocation]
        /// </summary>
        private Global.ACMethodResultState BookingOn_FacilityLocation_Material_NotLotManaged(ACMethodBooking BP)
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
            ///                 Setze LocalFacility mit Standard-Einlagerplatz
            if ((BP.ParamsAdjusted.OutwardFacilityLocation == null) && (BP.ParamsAdjusted.InwardFacilityLocation == null))
                return Global.ACMethodResultState.Failed;

            if ((BP.ParamsAdjusted.OutwardMaterial == null) || (BP.ParamsAdjusted.InwardMaterial == null))
                return Global.ACMethodResultState.Failed;

            //if (!CheckAndAdjustFacilityLocation())
            //return BookingResult.Failed;

            if ((BP.ParamsAdjusted.OutwardFacility != null) || (BP.ParamsAdjusted.InwardFacility != null))
            {
                return Global.ACMethodResultState.Failed;
            }

            if (BP.OutwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit Material auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Auslagerplatz angelegt werden
                /// Local.OutwardMaterial ist garantiert gesetzt aufgrund des Aufrufs von CheckAndAdjustMaterial() zuvor
                if (!(from c in BP.DatabaseApp.FacilityCharge
                     where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.OutwardFacilityLocation.FacilityID)
                        && ((c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.MaterialID)
                            || (BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.OutwardMaterial.ProductionMaterialID))
                        && (c.NotAvailable == false)
                     select c).Any())
                {
                    /// Falls Standard-Auslagerplatz nicht existiert, breche ab
                    if (BP.ParamsAdjusted.OutwardFacilityLocation.Facility1_OutgoingFacility == null)
                    {
                        return Global.ACMethodResultState.Failed;
                    }
                    else
                    {
                        BP.ParamsAdjusted.OutwardFacility = BP.ParamsAdjusted.OutwardFacilityLocation.Facility1_OutgoingFacility;
                    }
                }
            }
            if (BP.InwardFacilityLocation != null)
            {
                /// Wenn keine FacilityChargen mit Material auf Lagerort vorhanden
                /// Dann muss eine anonyme Charge auf dem Standard-Einlagerplatz angelegt werden
                /// Local.InwardMaterial ist garantiert gesetzt aufgrund des Aufrufs von CheckAndAdjustMaterial() zuvor
                if (!(from c in BP.DatabaseApp.FacilityCharge
                     where (c.Facility.Facility1_ParentFacility.FacilityID == BP.ParamsAdjusted.InwardFacilityLocation.FacilityID)
                        && ((c.MaterialID == BP.ParamsAdjusted.InwardMaterial.MaterialID)
                            || (BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID.HasValue && c.MaterialID == BP.ParamsAdjusted.InwardMaterial.ProductionMaterialID))
                        && (c.NotAvailable == false)
                     select c).Any())
                {
                    /// Falls Standard-Einlagerplatz nicht existiert, breche ab
                    if (BP.ParamsAdjusted.InwardFacilityLocation.Facility1_IncomingFacility == null)
                    {
                        return Global.ACMethodResultState.Failed;
                    }
                    else
                    {
                        BP.ParamsAdjusted.InwardFacility = BP.ParamsAdjusted.InwardFacilityLocation.Facility1_IncomingFacility;
                    }
                }
            }
            /// Einlagerungsbuchung: ((OutwardFacilityLocation == null) && (InwardFacilityLocation != null))
            /// Auslagerungsbuchung: ((OutwardFacilityLocation != null) && (InwardFacilityLocation == null))
            /// Umlagerungsbuchung: ((OutwardFacilityLocation != null) && (InwardFacilityLocation != null))
            /// TODO: NotAvailableState.Set ist erlaubt. Beachte bei Buchungen !!!

            return Global.ACMethodResultState.Succeeded;
        }

        /// <summary>
        /// Fall 6. [Facility]:   
        /// Nur bei Silos möglich zur Silobestandsführung
        /// </summary>
        private Global.ACMethodResultState BookingOn_Facility_NotLotManaged(ACMethodBooking BP)
        {
            return BookingOn_Facility_Common(BP);
        }

        /// <summary>
        /// Fall 6.1. [Facility],[Material]:   
        /// Zur Silobestandsführung oder Lagerplatzverwaltung von gemischten Materialien, die ähnlich wie ein Silo funktionieren
        /// Und zum anlegen von Anonymen Chargen, weil zu dem Buchungszeitpunkt noch keine Chargeninformation da war
        /// </summary>
        private Global.ACMethodResultState BookingOn_Facility_Material_NotLotManaged(ACMethodBooking BP)
        {
            return BookingOn_Facility_Common(BP);
        }
        #endregion

        #region Retrograde Posting
        private Global.ACMethodResultState PerformRetrogradePosting(ACMethodBooking BP, FacilityBookingCharge FBC, List<FacilityCharge> facilityCharges)
        {
            if (BP.BookingType != GlobalApp.FacilityBookingType.ProdOrderPosInward
                || BP.PartslistPos == null
                || !BP.PartslistPos.Backflushing
                || !BP.PartslistPos.ProdOrderBatchID.HasValue
                || Math.Abs(FBC.InwardQuantityUOM - 0) <= Double.Epsilon
                || PartslistManager == null)
                return Global.ACMethodResultState.Succeeded;

            var queryRelationsForRetrogPosting = BP.DatabaseApp.ProdOrderPartslistPosRelation
                .Include(c => c.SourceProdOrderPartslistPos)
                .Include(c => c.SourceProdOrderPartslistPos.Material)
                .Include(c => c.SourceProdOrderPartslistPos.BasedOnPartslistPos)
                .Include(c => c.SourceProdOrderPartslistPos.BasedOnPartslistPos.Material)
                .Where(c => c.ProdOrderBatchID == BP.PartslistPos.ProdOrderBatchID.Value
                            && ((c.SourceProdOrderPartslistPos.Material.RetrogradeFIFO.HasValue && c.SourceProdOrderPartslistPos.Material.RetrogradeFIFO.Value)
                                || (c.SourceProdOrderPartslistPos.BasedOnPartslistPosID.HasValue && c.SourceProdOrderPartslistPos.BasedOnPartslistPos.RetrogradeFIFO.HasValue && c.SourceProdOrderPartslistPos.BasedOnPartslistPos.RetrogradeFIFO.Value)
                                || (c.SourceProdOrderPartslistPos.RetrogradeFIFO.HasValue && c.SourceProdOrderPartslistPos.RetrogradeFIFO.Value)
                                || (c.RetrogradeFIFO.HasValue && c.RetrogradeFIFO.Value)
                                || (c.ParentProdOrderPartslistPosRelationID.HasValue && c.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.RetrogradeFIFO.HasValue && c.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation.RetrogradeFIFO.Value))
                            );

            if (queryRelationsForRetrogPosting == null || !queryRelationsForRetrogPosting.Any())
                return Global.ACMethodResultState.Succeeded;
            
            ACComponent workplace = null;
            core.datamodel.ACClass workplaceClass = null;
            if (!String.IsNullOrEmpty(BP.PropertyACUrl))
            {
                workplace = this.ACUrlCommand(BP.PropertyACUrl) as ACComponent;
                if (workplace != null)
                    workplaceClass = workplace.ComponentClass;
                else
                    workplaceClass = core.datamodel.Database.GlobalDatabase.GetACTypeByACUrlComp(BP.PropertyACUrl);
            }
            if (workplaceClass == null && BP.InwardFacility != null)
                workplaceClass = BP.InwardFacility.FacilityACClass;


            if (workplaceClass != null)
            {
                using (core.datamodel.Database usingPlusDB = new Database())
                {
                    core.datamodel.Database iPlusDB = usingPlusDB;
                    if (BP.DatabaseApp.IsSeparateIPlusContext)
                        iPlusDB = BP.DatabaseApp.ContextIPlus;
                    if (workplaceClass.Database != iPlusDB)
                        workplaceClass = workplaceClass.FromIPlusContext<core.datamodel.ACClass>(iPlusDB);

                    double factor = FBC.InwardQuantityUOM / BP.PartslistPos.TargetQuantityUOM;
                    foreach (var relationForRPost in queryRelationsForRetrogPosting)
                    {
                        if (!relationForRPost.Backflushing)
                            continue;
                        double postingQuantity = relationForRPost.TargetQuantityUOM * factor;
                        IList<Facility> possibleSourceFacilities;
                        IEnumerable<Route> routes = PartslistManager.GetRoutes(relationForRPost, BP.DatabaseApp, iPlusDB, workplaceClass, ACPartslistManager.SearchMode.SilosWithOutwardEnabled, null, out possibleSourceFacilities, null, null, null, false);
                        if (routes != null && routes.Any())
                        {
                            Route dosingRoute = null;
                            Facility storeForRetrogradePosting = null;
                            foreach (var prioSilo in possibleSourceFacilities.OrderBy(c => c.MDFacilityType.MDFacilityTypeIndex)) // First storage place then silo!
                            {
                                if (!prioSilo.VBiFacilityACClassID.HasValue)
                                    continue;
                                dosingRoute = routes.Where(c => c.FirstOrDefault().Source.ACClassID == prioSilo.VBiFacilityACClassID).FirstOrDefault();
                                if (dosingRoute != null)
                                {
                                    storeForRetrogradePosting = prioSilo;
                                    break;
                                }
                            }

                            if (storeForRetrogradePosting == null)
                            {
                                // Warning50040: Backflushing for Material {0} / {1} is not possible, because no route from {2} (Target) to a storage container (Source) could be found.
                                BP.AddBookingMessage(ACMethodBooking.eResultCodes.NoFacilityFoundForRetrotragePosting, Root.Environment.TranslateMessage(this, "Warning50040", relationForRPost.SourceProdOrderPartslistPos.MaterialNo, relationForRPost.SourceProdOrderPartslistPos.MaterialName, workplaceClass.ACURLComponentCached));
                                continue;
                            }

                            Global.ACMethodResultState bookingSubResult = Global.ACMethodResultState.Succeeded;
                            FacilityChargeList facilityOutwardChargeSubList = new FacilityChargeList(from c in BP.DatabaseApp.FacilityCharge.Include(d => d.FacilityLot).Include(d => d.Facility)
                                                                                                     where (c.Facility.FacilityID == storeForRetrogradePosting.FacilityID)
                                                                                                            && (   (c.MaterialID == relationForRPost.SourceProdOrderPartslistPos.MaterialID)
                                                                                                                || (relationForRPost.SourceProdOrderPartslistPos.Material.ProductionMaterialID.HasValue && c.MaterialID == relationForRPost.SourceProdOrderPartslistPos.Material.ProductionMaterialID))
                                                                                                            && (c.NotAvailable == false)
                                                                                                            && (c.IsEnabled)
                                                                                                            && (!c.MDReleaseStateID.HasValue || c.MDReleaseState.MDReleaseStateIndex <= (short)MDReleaseState.ReleaseStates.AbsFree)
                                                                                                            && (!c.FacilityLotID.HasValue || !c.FacilityLot.MDReleaseStateID.HasValue || c.FacilityLot.MDReleaseState.MDReleaseStateIndex <= (short)MDReleaseState.ReleaseStates.AbsFree)
                                                                                                     select c);

                            StackItemList stackItemListInOut;
                            MsgBooking msgBookingInOut;
                            bookingSubResult = BP.StackCalculatorOutward(this).CalculateInOut(false,
                                                false,
                                                BP.ParamsAdjusted.DontAllowNegativeStock == true ? false : true,
                                                postingQuantity, relationForRPost.SourceProdOrderPartslistPos.Material.BaseMDUnit,
                                                facilityOutwardChargeSubList, BP, out stackItemListInOut, out msgBookingInOut, true);
                            BP.Merge(msgBookingInOut);
                            if (bookingSubResult == Global.ACMethodResultState.Succeeded)
                            {
                                ACMethodBooking outwardMethod = NewBookParamOutwardMovement(BP, relationForRPost, postingQuantity);
                                outwardMethod.OutwardFacility = facilityOutwardChargeSubList.Select(c => c.Facility).FirstOrDefault();
                                FacilityBooking fbOutward = NewFacilityBooking(outwardMethod);

                                // Mache Stackbuchung
                                bookingSubResult = DoOutwardStackBooking(outwardMethod, stackItemListInOut, facilityOutwardChargeSubList);
                                if ((bookingSubResult == Global.ACMethodResultState.Failed) || (bookingSubResult == Global.ACMethodResultState.Notpossible))
                                {
                                    BP.Merge(outwardMethod.ValidMessage);
                                    return bookingSubResult;
                                }
                                else
                                {
                                    BP.FacilityBookings.Add(outwardMethod);
                                }
                            }
                        }
                    }
                }
            }

            return Global.ACMethodResultState.Succeeded;
        }

        public ACMethodBooking NewBookParamOutwardMovement(ACMethodBooking BP, ProdOrderPartslistPosRelation partsListPosRelation, double postingQuantity)
        {
            ACMethodBooking outwardMethod = GetBookParamOutwardMovementClone();
            outwardMethod.OutwardMaterial = partsListPosRelation.SourceProdOrderPartslistPos.Material;
            outwardMethod.PartslistPosRelation = partsListPosRelation;
            //acMethodBooking.InwardQuantity = deliveryNotePos.InOrderPos.TargetQuantityUOM;
            if (partsListPosRelation.SourceProdOrderPartslistPos.MDUnit != null)
            {
                outwardMethod.OutwardQuantity = partsListPosRelation.SourceProdOrderPartslistPos.Material.ConvertQuantity(postingQuantity, partsListPosRelation.SourceProdOrderPartslistPos.Material.BaseMDUnit, partsListPosRelation.SourceProdOrderPartslistPos.MDUnit);
                outwardMethod.MDUnit = partsListPosRelation.SourceProdOrderPartslistPos.MDUnit;
            }
            else
            {
                outwardMethod.OutwardQuantity = postingQuantity;
            }
            if (partsListPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.CPartnerCompany != null)
                outwardMethod.CPartnerCompany = partsListPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.CPartnerCompany;
            outwardMethod.Database = BP.DatabaseApp;
            outwardMethod.CheckAndAdjustPropertiesForBooking(BP.DatabaseApp);
            return outwardMethod;
        }

        ACMethodBooking _BookParamOutwardMovementClone;
        public ACMethodBooking GetBookParamOutwardMovementClone()
        {
            using (ACMonitor.Lock(_40010_ValueLock))
            {
                if (_BookParamOutwardMovementClone != null)
                    return _BookParamOutwardMovementClone.Clone() as ACMethodBooking;
            }
            var clone = ACUrlACTypeSignature("!" + FacilityManager.MN_ProdOrderPosOutward.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            using (ACMonitor.Lock(_40010_ValueLock))
            {
                _BookParamOutwardMovementClone = clone;
                return _BookParamOutwardMovementClone.Clone() as ACMethodBooking;
            }
        }

        public ACMethodBooking NewBookParamZeroStock(ACMethodBooking BP, FacilityCharge fcToBookZero)
        {
            ACMethodBooking method = GetBookParamNotAvailableClone();
            method.InwardFacilityCharge = fcToBookZero;
            method.Database = BP.DatabaseApp;
            method.MDZeroStockState = MDZeroStockState.DefaultMDZeroStockState(BP.DatabaseApp, MDZeroStockState.ZeroStockStates.SetNotAvailable);
            method.CheckAndAdjustPropertiesForBooking(BP.DatabaseApp);
            return method;
        }

        ACMethodBooking _BookParamNotAvailableClone;
        public ACMethodBooking GetBookParamNotAvailableClone()
        {
            using (ACMonitor.Lock(_40010_ValueLock))
            {
                if (_BookParamNotAvailableClone != null)
                    return _BookParamNotAvailableClone.Clone() as ACMethodBooking;
            }
            var clone = ACUrlACTypeSignature("!" + GlobalApp.FBT_ZeroStock_FacilityCharge.ToString(), gip.core.datamodel.Database.GlobalDatabase) as ACMethodBooking; // Immer Globalen context um Deadlock zu vermeiden 
            using (ACMonitor.Lock(_40010_ValueLock))
            {
                _BookParamNotAvailableClone = clone;
                return _BookParamNotAvailableClone.Clone() as ACMethodBooking;
            }
        }


        #endregion
    }
}
