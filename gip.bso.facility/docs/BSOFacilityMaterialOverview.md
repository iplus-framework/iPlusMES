# Material Overview

> Business object: `BSOFacilityMaterialOverview` (Assembly `gip.bso.facility`, Menu: **Warehouse Management → Material Overview**)

## Definition

This app shows the **current stock of every material** (material stock records) and
provides the material details. It replaces the legacy stock overview masks for total
material stocks.

The overview is **read-only**: all stock bookings are always executed only via the
*FacilityBookingManager*. This app does not create, change or delete stock.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Material stock** | Stock record of a material: total stock, available/reserved quantities, day/month statistics |
| **Material group** | Classification of materials; used as a filter here |
| **Material details** | Embedded detail view of the selected material (child app *Material Details*) |
| **Lot dialog** | Selection dialog that returns a lot of a material (used by other apps) |

## Screen layout

The screen consists of a search list (left) and a detail area (right):

| Tab / Section | Content |
|---|---|
| **Explorer** (list) | Materials with stock figures: material number/name, unit, stock quantity, available/reserved quantities, minimum/optimum stock with difference, day and month inward/outward figures, weight, release state |
| **Material details** | Embedded *Material Details* app showing the details of the selected material |
| **Filter** | **Material Group Filter** dropdown and **Filter** button |

A **Statistic** view (day/week/month/year balances of the current material) and a
**Lot dialog** (lot selection with **OK** / **Cancel**) are available as designs of
this app.

## Step-by-step tasks

### Searching materials

1. Enter a search string (material number or name) in the search field.
2. Click **Search**. The list is loaded automatically on start (unless the app is
   started with the *AutoFilter* parameter set to *false*).

### Filtering by material group

1. In the **Filter** section select a **Material Group**.
2. Click **Filter** — the list is restricted to materials of this group.
   Selecting no group and clicking **Filter** removes the restriction again.

### Viewing the details of a material

1. Select the material in the list. The **Material details** tab shows the details
   of the selected material.
2. Click **Load** to re-read the selected material stock from the database.

### Selecting a lot (lot dialog)

1. The lot dialog is opened by other apps (e.g. for booking) for a given material
   number and shows the lots of this material.
2. Select a lot and confirm with **OK** — the selected lot is returned to the caller.
3. **Cancel** closes the dialog without a selection.

### Saving and discarding

* **Save** stores changes of the current record.
* **Undo** discards unsaved changes.
* The overview itself has **no New** and **no Delete** — stock is only changed by
  bookings (FacilityBookingManager).

## Field reference (list)

| Field | Description |
|---|---|
| Material number / Material name | Identification of the material |
| Unit | Base unit of the material |
| Stock quantity / Stock unit A/B/C | Stock in the base unit and alternative units |
| Available quantity | Stock available for booking |
| Reserved quantity (inward/outward) | Reserved incoming/outgoing quantities |
| Min stock / Opt stock (+ difference, exceeded) | Minimum and optimum stock monitoring |
| Day inward / Day outward (+ target, difference) | Bookings of the current day |
| Month inward / Month outward (+ target, difference) | Bookings of the current month |
| Average production quantity / Current month stock / Average month stock | Statistical stock figures |
| Weight (from volume) | Weight of the stock |
| Release state | Release state of the material |
| Material Group Filter | Restricts the list to one material group |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityMaterialOverview`
contains the complete usage recipe: `SearchWord` → `MaterialStockList` →
`NavigateFirst()` → `CurrentMaterialStock`; `MaterialGroupFilter` + `Filter()` for the
material group restriction; the selected material is forwarded to the child BSO
`BSOMaterialDetails_Child` (`CurrentMaterialID`, `CurrentMaterialStock`);
`ShowLotDlg(materialNo, lotNos)` opens the lot selection dialog and returns the
selected lot via `DialogResult` (closed with `ShowLotDlgOk()` / `ShowLotDlgCancel()`).
