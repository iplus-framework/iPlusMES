# Material Details (Stock, Postings, Statistics, Reservations)

> Business object: `BSOMaterialDetails` (Assembly `gip.bso.masterdata`)
> Embedded child app of the **Company** app, tab **Material Overview** — it cannot be started
> standalone and has no menu entry of its own.

## Definition

**Material Details** is a read-only detail view for **one material**. It shows:

* the current **stock** of the material (quants, sums per storage area / storage location / lot),
* the **movement history** (postings) of the material,
* **turnover statistics** (day / week / month / year),
* the **reservations** of the material (picking and production orders).

It is embedded in the Company app: selecting a row in the **Material Overview** list pushes the
selected material into this detail view. There is **no editing, saving or deleting** here —
changes are made in the Material app or via the dialogs opened from here.

## Concepts

| Term | Meaning |
|---|---|
| **Quant** | A physical stock item: material in a storage bin with quantity, unit and lot |
| **Lot / Batch** | Production batch; a quant can belong to a lot with lot number, production and expiration date |
| **Storage bin / Storage location / Storage area** | Hierarchy of the warehouse structure; sums are shown per level |
| **Posting** | A stock movement (goods receipt, issue, transfer) with inward/outward quantity |
| **Reservation** | Planned future movement: material reserved for a picking order or a production order |
| **Material stock** | Aggregated stock record of the material (total, reserved, available, min/opt stock) |

## Screen layout

The detail view has four tabs:

| Tab | Content |
|---|---|
| **Statistic** | Turnover statistics of the material per day, week, month and year: inward/outward actual vs. target, differences (quantity and percent), corrections, current/average month stock, last stock, last usage |
| **Postings** | Movement history: filter (search from/to, posting type, group by material) and two lists — **Posting calls** (postings with running total) and **Postings on quants** (postings per quant with lot, bin, delivery note, order and picking references) |
| **Stock** | Current stock: header with material number/name, stock quantity, min/opt stock and differences, reserved quantities; four sub-tabs (see below) |
| **Reservation** | Reservations of the material with search from/to; sub-tabs **Picking Reservation** and **Prod Order Reservation** |

Sub-tabs of the **Stock** tab:

| Sub-tab | Content |
|---|---|
| **Facility** | Stock sums per storage area |
| **FacilityLocation** | Stock sums per storage location |
| **Lots** | Stock sums per lot (lot number, expiration, storage life) |
| **QuantsStorageBin** | The individual quants: bin, location, lot, split no., quantities (units A/B/C), blocked/free/total sums, production/expiration dates, release state, created by/date |

Filters on the Stock tab: **Show not available** (include blocked/unavailable quants) and
**Facility No.** (only bins whose number starts with this text).

## Usage (step by step)

The detail view follows the selection in the parent app:

1. Open the **Company** app and switch to the **Material Overview** tab.
2. Select a material row — the detail view refreshes automatically for this material
   (stock lists are reloaded, movements and reservations are cleared).

### Viewing the stock

1. Open the **Stock** tab.
2. Switch between **Facility**, **FacilityLocation**, **Lots** and **QuantsStorageBin** to see
   the sums per storage area, storage location and lot, or the individual quants.
3. Enter a text in **Facility No.** to restrict the list to bins whose number starts with it.
4. Enable **Show not available** to include currently unavailable (e.g. blocked) quants.

### Viewing the movement history (Postings)

1. Open the **Postings** tab.
2. Set **Search from** / **Search to** (default: yesterday until today), optionally a
   **Filter Posting Type** and **Grouped by Material**.
3. Click **Refresh** — the posting lists are rebuilt in the background (a progress dialog is
   shown). The upper list shows the postings with a running total, the lower list the postings
   on quants.

### Showing reservations

1. Open the **Reservation** tab.
2. Set **Search from** / **Search to**.
3. Click **Show Facility Reservation** — the picking and production order reservations are
   loaded into the **Picking Reservation** and **Prod Order Reservation** sub-tabs.

### Navigating from a quant

Select a quant in the **QuantsStorageBin** list. The following commands are then available
(context menu / toolbar, each only enabled if the referenced data exists):

* **Manage Material** — opens the material master data.
* **Show Order** — opens the production order that produced this quant.
* **Manage Quant** — opens the quant for editing (quant, bin, material, lot).
* **Manage Lot/Batch** — opens the lot of the quant.
* **Show Lot Stock and History** — stock and history of the lot.
* **Show Bin Stock and History** — stock and history of the storage bin (bin containers only).
* **Manage Stock of Bin** — manage the stock of the bin (bin containers only).

On the **Statistic** tab the button **Manage Material** opens the material master data directly.

## Field reference (Stock tab, header)

| Field | Description |
|---|---|
| Material No / Material Name | Number and name of the material |
| Stock Quantity | Current total stock of the material |
| Available Quantity | Stock available for new reservations |
| Reserved Quantity / Reserved Inward / Reserved Outward | Reserved quantities (total, expected goods receipts, planned issues) |
| Min Stock Quantity / Opt Stock Quantity | Minimum and optimal stock levels |
| Min/Opt Stock Quantity Diff | Deviation of the current stock from min/opt level |

Field reference of the quant list (**QuantsStorageBin**): bin and location number/name, lot
number, external lot no., split no., stock quantity and units A/B/C, blocked/free/total sums,
production date, filling date, expiration date, remaining storage life, release state,
**Not available** flag, comment, created by/date.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOMaterialDetails` contains the
complete usage recipe. In short:

* The BSO is **hosted** by `BSOCompanyMaterialOverview` (child item
  `BSOMaterialDetails_Child`); the parent sets `CurrentMaterialID` and `CurrentMaterialStock`.
* Read-only: there are **no** New/Save/Delete methods.
* `RefreshMovements()` fills `FacilityBookingOverviewList` /
  `FacilityBookingChargeOverviewList` (filter: `FBSearchFrom`, `FBSearchTo`, `FilterFBType`,
  `FBGroupByMaterial`).
* `LoadFacilityReservation()` fills the child BSO `BSOFacilityReservationOverview_Child`
  (requires `CurrentMaterialStock`).
* Stock lists: `FacilityChargeList`, `FacilityChargeSumFacilityHelperList`,
  `FacilityChargeSumLocationHelperList`, `FacilityChargeSumLotHelperList`; filters
  `ShowNotAvailable`, `FilterFacilityNoStart`, `FilterLotNos`.
* Navigation from `SelectedFacilityCharge`: `NavigateToMaterial()`, `NavigateToOrder()`,
  `NavigateToFacilityCharge()`, `NavigateToFacilityLot()`,
  `NavigateToFacilityLotOverview()`, `NavigateToFacilityOverview()`, `NavigateToFacility()`.
