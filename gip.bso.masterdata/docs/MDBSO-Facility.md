# Facility Master-Data Value Lists

> Business objects: `MDBSO*` (Assembly `gip.bso.masterdata`, Menu: **Master Data → Facility**)

## What are master-data value lists?

Value lists are small, centrally maintained tables whose entries appear as
selection lists (drop-downs) in other apps — for example the storage type of a
storage bin or the reason of a stock booking. Each entry has a **Name**
(multilingual), a **Sort Index** (display order) and a **Default** flag
(pre-selected for new records). Some lists also store a configuration
(`XMLConfig`) used by the system.

## Standard operation (identical for all lists in this document)

Every value list is maintained in the same way:

1. **Search** — opens/refreshes the list. All existing entries are shown.
2. **New** — creates a new entry. Enter the **Name** (per language) and a
   **Sort Index**; set **Default** if the entry should be pre-selected.
3. **Edit** — select an entry in the list and change its fields.
4. **Save** — persists the new or changed entry. Always save **before**
   navigating to another entry or creating a new one.
5. **Undo** ("Do not save") — discards unsaved changes and restores the last
   saved state.
6. **Delete** — removes the selected entry. This is only possible if no other
   data (e.g. facilities, bookings) references the entry; otherwise the system
   reports a foreign-key error. Call **Save** afterwards to execute the delete.
7. **Load** — reloads the selected entry from the database, e.g. if another
   user has changed it in the background.

> **Note:** The delivered entries are referenced by the system (by an internal
> index, not by name). Do **not** delete or rename delivered entries — add new
> ones instead, unless instructed otherwise.

---

## Balancing Mode (Bilanzierungsmodus)

**Purpose:** Defines whether stock bookings on a facility are *balanced*
(accounted) on inward and/or outward movements.

| Value (EN / DE) | Default | Meaning |
|---|---|---|
| Inward On, Outward On / Einwärts an, Auswärts an | ✔ | Bookings are balanced in both directions |
| Inward On, Outward Off / Einwärts an, Auswärts aus | | Only inward movements are balanced |
| Inward Off, Outward Off / Einwärts aus, Auswärts aus | | No balancing (see note below) |
| Inward Off, Outward Off / Einwärts aus, Auswärts aus | | No balancing |

> **Data note:** The third entry has the internal key `InwardOff_OutwardOn`
> ("inward off, outward **on**") but its caption reads "Outward Off" — the
> caption in the database appears to be incorrect and should be reviewed.

## Posting Not Available Mode (Modus bei Nichverfügbarkeit)

**Purpose:** Defines how the *Not Available* flag of a facility or facility
charge is automatically set and/or reset when bookings are posted on stock
that is not available.

| Value (EN / DE) | Default | Meaning |
|---|---|---|
| Auto Set / Automatisch setzen | | Flag is set automatically |
| AutoReset / Automatisch zurücksetzen | | Flag is reset automatically |
| AutoSetAndReset / Automatisch setzen und zurücksetzen | ✔ | Flag is set and reset automatically |
| Off / Aus | | No automatic handling |

## Inventory Line State (Inventurpositionsstatus)

**Purpose:** Processing state of a **single line** of a stock-taking
(inventory).

| Value (EN / DE) | Default | Meaning |
|---|---|---|
| Not counted / Ungezählt | ✔ | Line created, not yet counted |
| In Counting / In Zählung | | Counting of this line is running |
| Counted / Gezählt | | Line has been counted |
| Posted / Gebucht | | Difference has been posted |

## Inventory Status (Inventurstatus)

**Purpose:** Processing state of a **complete stock-taking run**.

| Value (EN / DE) | Default | Meaning |
|---|---|---|
| New / Neu | ✔ | Stock-taking created |
| In Progress / In Arbeit | | Counting is running |
| Finished / Fertig | | All lines counted |
| Posted / Gesendet | | Differences posted |
| Cancelled / Storniert | | Stock-taking cancelled |

## Stock Management Type (Lagerführungsart)

**Purpose:** Defines whether and how a facility manages stock.

| Value (DE) | Default | Meaning |
|---|---|---|
| Keine Lagerführung (*No facility management*) | | Facility keeps no stock at all |
| Lagerführung ohne Chargen (*stock management without batches*) | ✔ | Stock is managed per facility, no batch split |
| Lagerführung mit Chargen (*stock management with batches*) | | Stock is managed per facility and batch |
| Verwaltung mit Chargenreservierung (*management with lot reservation*) | | Like above, plus lot reservations |

## Storage Type (Lagerart)

**Purpose:** Classifies the **type of a facility** (storage object).

| Value (EN / DE) | Meaning |
|---|---|
| Storage Location / Lagerort | Top level of the storage structure |
| Storage Bin / Lagerplatz | Storage bin — the classic storage place |
| Preparation Bin / Materialbereitstellungsbehälter | Container for providing material to a workplace |
| Tank/Silo/Cell/Scale Container / Tank/Silo/Zelle/Waage-Behältnis | Tank, silo, cell or scale container |
| Vehicle / Fahrzeug | Vehicle (see *Vehicle Type*) |
| Vehicle Container / Fahrzeugbehältnis | Container mounted on a vehicle |
| Machine or Inventory / Maschine oder Inventar | Machine or inventory item treated as facility |

## Vehicle Type (Fahrzeugtyp)

**Purpose:** Classifies **vehicles** used for transport.

| Value (EN / DE) | Meaning |
|---|---|
| Calibration Vehicle / Eichfahrzeug | Calibrated vehicle (for calibrated deliveries) |
| Semi Trailer Truck / Sattelzugmaschine | Semi-trailer truck (tractor unit) |
| Semi Trailer / Auflieger für Sattelzug | Semi-trailer |
| Ballast tractor / Schwertansporter | Ballast tractor |
| Full Trailer / Mehrachsiger Anhänger mit gelenkter Vorderachse | Full trailer (multi-axle with steered front axle) |

## Inventory Mgmnt. Type (Bestandsführungsart)

**Purpose:** Defines **how stock levels are managed** on a facility.

| Value (EN / DE) | Default | Meaning |
|---|---|---|
| Manual Inventory Management / Manuelle Bestandsführung | ✔ | Stock is managed manually |
| Signal Stock / Signalbestand | | A signal (reorder) level triggers messages |
| Infinite Stock / Unendlicher Bestand | | Stock is treated as unlimited (no depletion) |

## Movement Reason (Bewegungsgrund)

**Purpose:** Reason code assigned to **facility bookings** — documents *why* a
stock movement happened.

| Value (EN / DE) | Meaning |
|---|---|
| Goods Receipt / Wareneingang | Receipt of goods |
| Goods Issue / Warenausgang | Issue of goods |
| Consumption / Verbrauch | Material consumed (with balancing) |
| Consumption Without Balance / Verbrauch ohne Bilanz | Consumption without balancing |
| Production / Herstellung | Production output (with balancing) |
| Production Without Balance / Herstellung ohne Bilanz | Production output without balancing |
| Adjustment / Korrektur | Stock correction |
| Zero Stock / Nullbestand | Booking to zero stock |
| Relocation / Warenumlagerung | Stock relocation |
| Enabling / Freigabe | Release of blocked stock |
| Blocking / Sperrung | Blocking of stock |
| Inventory / Inventur | Stock-taking difference |
| Correction from ERP / Korrektur aus ERP | Correction received from the ERP system |

## Release State (Freigabestatus)

**Purpose:** Release/blocking **state of facility stock**.

| Value (EN / DE) | Default | Meaning |
|---|---|---|
| Released Stock / Freigegebener Bestand | ✔ | Stock may be used normally |
| Blocked Stock / Gesperrter Bestand | | Stock is blocked |
| Release absolutely locked stock / Absolut gesperrten Bestand freigeben | | Releases absolutely locked stock |
| Absolute Blocked Stock / Absolut gesperrter Bestand | | Stock is absolutely locked (no booking at all) |

## Reservation Mode (Reservierungsmodus)

**Purpose:** Defines the **reservation behavior** of a booking.

| Value (EN / DE) | Default | Meaning |
|---|---|---|
| No Reservation / Keine Reservierung | ✔ | No reservation is made |
| Reservation / Reservierung | | Quantity is reserved |
| Resolve Reservation for Partial Quantity / Reservierung für Teilmenge auflösen | | Reservation is resolved for the partial quantity |
| Completely Resolve Reservation / Reservierung vollständig auflösen | | Reservation is resolved completely |

## Zero Stock Status (Nullbestand-Status)

**Purpose:** Defines **what happens when the stock of a facility reaches
zero**.

| Value (EN / DE) | Default | Meaning |
|---|---|---|
| Off / Aus | ✔ | No special handling |
| Book to Zero Stock / Buchen auf Nullbestand | | Book the stock down to zero |
| Set Not Available / Auf nicht verfügbar setzen | | Set the facility to *not available* |
| Reset If Not Available / Zurücksetzen, wenn nicht verfügbar | | Reset the *not available* flag if stock is not available |
| Reset If Not Available on facility / Zurücksetzen, wenn nicht verfügbar auf Anlage | | Reset the flag on the facility itself |
| Restore quantity if not available / Menge wiederherstellen, wenn nicht verfügbar | | Restore the quantity if stock is not available |

---

## For AI agents (MCP)

Each business object above (`MDBSOBalancingMode`, `MDBSOBookingNotAvailableMode`,
`MDBSOFacilityInventoryPosState`, `MDBSOFacilityInventoryState`,
`MDBSOFacilityManagementType`, `MDBSOFacilityType`, `MDBSOFacilityVehicleType`,
`MDBSOInventoryManagementType`, `MDBSOMovementReason`, `MDBSOReleaseState`,
`MDBSOReservationMode`, `MDBSOZeroStockState`) exposes its metadata — including a
complete usage recipe in the class-level `<summary>` and per-method
descriptions — through the iPlus MCP Server (`get_thesaurus`, `get_type_infos`,
`get_method_info`, `execute_acurl_command`). All twelve share the same recipe:
`Search()` → `<Entity>List` → `NavigateFirst()` → edit `Current<Entity>` →
`Save()`; `New()` / `Delete()` / `Load()` as described above.
