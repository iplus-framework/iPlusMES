# Storage Bin Overview

> Business object: `BSOFacilityOverview` (Assembly `gip.bso.facility`, Menu: **Warehouse Management → Storage Bin Overview**)

## Definition

This app shows the **current stock of every storage bin** (container) and its
movements. It replaces the legacy stock overview masks (total stocks, storage
location stocks and storage bin stocks of materials, charges and material charges).

The overview is **read-only**: all stock bookings are always executed only via the
*FacilityBookingManager*. This app does not create, change or delete stock.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Storage bin** | A facility of type *Storage bin/container* that physically holds quants |
| **Storage location** | Parent facility of type *Storage location*; groups several storage bins |
| **Quant** | A stored charge (material + lot + quantity) in a storage bin |
| **Totals per material / per lot** | Aggregated stock of the bin grouped by material or by lot |
| **Movements** | Inward/outward bookings of the bin in a selected period |
| **Not available** | Quants that are blocked/reserved and not available; shown only on request |

## Screen layout

The screen consists of a search list (left) and a detail area (right) with these tabs:

| Tab | Content |
|---|---|
| **Explorer** (list) | Storage bins with stock figures: bin number/name, material, stock quantity, available/reserved/ordered quantity, minimum/optimum stock with difference, day and month inward/outward figures, capacities (volume/weight, available space), inward/outward enabled, first/last quant and expiration dates |
| **Stock** | Three sections: **Quants** (quants stored in the bin), **Material** (totals per material: total/free/blocked), **Lots** (totals per lot with lot number, external lot no., production/filling/expiration date, remaining storage life) |
| **Statistic** | Day/week/month/year balance of the bin: inward/outward actual vs. target, differences, corrections, last stock, average month stock, last usage |
| **Movements** (Facility Booking History) | Inward/outward bookings of the bin in the period *Search from*–*Search to*, with running sum per row; embedded preview of the selected booking |
| **Filter** | Storage location dropdown and **Search** button |

The same Stock/Statistic/Movements tabs are also available in a **dialog** variant
(e.g. opened from other apps for a single bin) with **OK** / **Cancel** buttons.

## Step-by-step tasks

### Searching storage bins

1. Enter a search string (bin number or name) in the search field.
2. *(Optional)* In the **Filter** section select a **Storage location** — the list is
   then restricted to the bins of this location.
3. Click **Search**.

### Viewing the stock of a bin

1. Select the bin in the list. The header shows bin number, name and stock totals.
2. Open the **Stock** tab: the **Quants** section lists every quant in the bin
   (material, quantity, unit, split no., production/expiration date, release state).
   The **Material** and **Lots** sections show the aggregated totals.
3. Enable **Show not available** to include blocked/unavailable quants.
4. Click **Load** to re-read the selected bin from the database (e.g. after another
   user booked in the background).

### Viewing the movements of a bin

1. In the **Movements** tab set **Search from** and **Search to**.
2. *(Optional)* Set the **posting type filter** or enable **Group by material**.
3. Click **Refresh** — the movements are loaded in the background; the running sum
   is shown per row.

### Navigating from a quant

Select a quant in the **Stock** tab and use the context menu:

* **Show Order** — opens the production order that produced the quant.
* **Manage Quant** — opens the quant management for the selected quant.
* **Manage Stock of Bin** — opens the stock management of the bin (from the
  Statistic tab).
* **Show Lot Stock and History** — opens the lot stock/history overview.
* **Manage Lot/Batch** — opens the lot/batch management.
* **Show Material Stock and History** — opens the material stock/history overview.

### Saving and discarding

* **Save** stores changes of the current record.
* **Undo** discards unsaved changes.
* The overview itself has **no New** and **no Delete** — stock is only changed by
  bookings (FacilityBookingManager).

## Field reference (Stock tab)

| Field | Description |
|---|---|
| Stock quantity / Available / Reserved / Ordered | Stock figures of the bin |
| Min stock / Opt stock (+ difference, exceeded) | Minimum and optimum stock monitoring |
| Total / Free / Blocked | Totals per material or per lot |
| Lot no. / External lot no. | Lot identification |
| Production date / Filling date / Expiration date | Lot dates |
| Remaining storage life | Days until the expiration date |
| Split no. | Split number of the quant |
| Release state | Release state of the quant |
| Inward enabled / Outward enabled | Whether bookings into/out of the bin are allowed |
| Max volume / Max weight capacity, Available space | Physical capacity of the bin |
| Search from / Search to | Period of the movement evaluation |
| Group by material | Groups the movement rows by material |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityOverview`
contains the complete usage recipe: `SearchWord` → `FacilityList` → `NavigateFirst()`
→ `CurrentFacility`; quants in `FacilityChargeList`, totals in
`FacilityChargeSumMaterialHelperList` / `FacilityChargeSumLotHelperList`; movements
via `RefreshMovements()` into `FacilityBookingOverviewList` /
`FacilityBookingChargeOverviewList`; dialog entry point `ShowDialogFacility(facilityNo,
from, to)` closed with `DialogOK()` / `DialogCancel()`; navigation methods
`NavigateToOrder`, `NavigateToFacilityCharge`, `NavigateToFacility`,
`NavigateToFacilityLotOverview`, `NavigateToFacilityLot`, `NavigateToMaterialOverview`.
The button captions above are the translations of these methods (`ACMethod*`
attributes, stored in `ACClassText`).
