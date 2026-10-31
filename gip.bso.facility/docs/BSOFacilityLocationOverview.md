# Storage Location Overview

> Business object: `BSOFacilityLocationOverview` (Assembly `gip.bso.facility`, Menu: **Warehouse Management → Storage Location Overview**)

## Definition

This app shows the **current stock of every storage location** — aggregated across
all storage bins belonging to the location — and its movements. It replaces the
legacy stock overview masks for storage location stocks.

The overview is **read-only**: all stock bookings are always executed only via the
*FacilityBookingManager*. This app does not create, change or delete stock.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Storage location** | A facility of type *Storage location*; parent of one or more storage bins |
| **Quant** | A stored charge (material + lot + quantity) within the location |
| **Totals per material / per lot** | Aggregated stock of the whole location grouped by material or by lot |
| **Movements** | Inward/outward bookings of the location in a selected period |
| **Not available** | Quants that are blocked/reserved; shown only on request |

## Screen layout

The screen consists of a search list (left) and a detail area (right) with these tabs:

| Tab | Content |
|---|---|
| **Explorer** (list) | Storage locations with number and name |
| **Stock** | Three sections: **Quants** (all quants stored in the location), **Material** (totals per material: total/free/blocked), **Lots** (totals per lot with lot number, external lot no., production/filling/expiration date, remaining storage life) |
| **Movements** (Facility Booking History) | Inward/outward bookings of the location in the period *Search from*–*Search to*, with running sum per row; embedded preview of the selected booking |

## Step-by-step tasks

### Searching storage locations

1. Enter a search string (location number or name) in the search field.
2. Click **Search**. The list is loaded automatically on start.

### Viewing the stock of a location

1. Select the storage location in the list.
2. Open the **Stock** tab: the **Quants** section lists every quant stored anywhere
   in the location (material, quantity, unit, split no., production/expiration date,
   release state). The **Material** and **Lots** sections show the aggregated totals.
3. Enable **Show not available** to include blocked/unavailable quants.
4. Click **Load** to re-read the selected location from the database.

### Viewing the movements of a location

1. In the **Movements** tab set **Search from** and **Search to**.
2. *(Optional)* Set the **posting type filter** or enable **Group by material**.
3. Click **Refresh** — the movements are loaded in the background; the running sum
   is shown per row.

### Saving and discarding

* **Save** stores changes of the current record.
* **Undo** discards unsaved changes.
* The overview itself has **no New** and **no Delete** — stock is only changed by
  bookings (FacilityBookingManager).

## Field reference (Stock tab)

| Field | Description |
|---|---|
| Stock quantity / unit | Quantity of the quant or total |
| Total / Free / Blocked | Totals per material or per lot |
| Lot no. / External lot no. | Lot identification |
| Production date / Filling date / Expiration date | Lot dates |
| Remaining storage life | Days until the expiration date |
| Split no. | Split number of the quant |
| Release state | Release state of the quant |
| Search from / Search to | Period of the movement evaluation |
| Group by material | Groups the movement rows by material |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityLocationOverview`
contains the complete usage recipe: `SearchWord` → `FacilityLocationList` →
`NavigateFirst()` → `CurrentFacilityLocation`; quants in `FacilityChargeList`, totals
in `FacilityChargeSumMaterialHelperList` / `FacilityChargeSumLotHelperList`;
movements via `RefreshMovements()` into `FacilityBookingOverviewList` /
`FacilityBookingChargeOverviewList`. Only facilities of type
`FacilityTypesEnum.StorageLocation` are listed.
