# Lot Management

> Business object: `BSOFacilityLot` (Assembly `gip.bso.facility`, Menu: **Warehouse Management → Lot Management**)

## Definition

A **lot** (FacilityLot) is the logistical unit for stock keeping and traceability.
The lot management maintains lots from the logistical point of view: supplier
information and quality-relevant data from production (production date, filling date,
expiration date, storage life).

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Lot number** | Internal, unique number of the lot (generated automatically for new lots) |
| **External lot number** | Lot number assigned by the supplier (two fields available) |
| **Storage life** | Shelf life in days; used to calculate the expiration date |
| **Expiration date** | Date until which the lot may be used |
| **Storage life remaining** | Remaining shelf life, calculated from the expiration date |
| **Lock** | Locks the lot against further bookings |
| **Expiration date helper** | Internal helper that keeps production date, storage life and expiration date consistent |

## Screen layout

The screen is a search list on the left (**Explorer**) and a detail form on the right.

**Explorer (search area)**

| Filter | Description |
|---|---|
| Lot number | Filter on the internal lot number (contains) |
| External lot number | Filter on the first external lot number (contains) |
| Material number | Filter on the material of the lot (contains) |
| External lot number 2 | Filter on the second external lot number (contains) |

The result list is sorted by lot number (newest first). The search starts automatically
when a filter is changed.

**Detail form**

* Header: **Lot number**, **External lot number**, **Material**
* Expander **Properties**:
  * **Filling date**, **Production date**, **Storage life** (days),
    **Expiration date**, **Storage life remaining**, **Lock**, **Comment**
  * Search button (popup): **Show Lot Stock and History**

## Step-by-step tasks

### Searching for a lot

1. Enter the **Lot number**, **External lot number**, **Material number** or
   **External lot number 2** in the search area.
2. The search runs automatically; the result appears in the list.
3. Select a row to edit the lot in the detail form.

### Creating a new lot

1. Click **New** in the toolbar. A new lot is created with a generated lot number.
2. Assign the **Material** and, if required, change the **Lot number** and
   **External lot number**.
3. Click **Save**.

### Editing dates and shelf life

The three fields **Production date**, **Storage life** and **Expiration date** are
kept consistent automatically:

* Entering a **Storage life** (days) recalculates the **Expiration date** from the
  **Production date**.
* Changing the **Production date** recalculates **Expiration date** and **Storage life**.
* Changing the **Expiration date** recalculates **Production date** and **Storage life**.

After editing, click **Save**.

### Deleting a lot

1. Select the lot and click **Delete** in the toolbar.
2. Deletion is only possible if no other data (e.g. storage charges) references the lot.
3. After deletion the first remaining lot is selected automatically.

### Undo and refresh

* **Undo** discards unsaved changes on the current lot.
* **Load** re-reads the current lot if another user changed it in the background.

### Showing lot stock and history

1. Select the lot in the list.
2. Open the search popup in the detail form and click
   **Show Lot Stock and History**. The lot stock and history overview opens for this lot.

## Field reference

| Field | Description |
|---|---|
| Lot number | Unique internal lot number |
| External lot number | Supplier lot number (1st field) |
| External lot number 2 | Supplier lot number (2nd field) |
| Material | Material of the lot |
| Filling date | Date the lot was filled |
| Production date | Production date; basis for the expiration date calculation |
| Storage life | Shelf life in days |
| Expiration date | Calculated/entered expiry date |
| Storage life remaining | Remaining days until expiration (calculated) |
| Lock | Locks the lot against bookings |
| Comment | Free comment |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityLot` contains the
complete usage recipe (filter → search → navigate → edit → save). Relevant C# methods:
`Search`, `New`, `Save`, `UndoSave`, `Load`, `Delete`, `NavigateToFacilityLotOverview`
(**Show Lot Stock and History**), `ShowDialogNewLot`, `ShowDialogOrder`,
`ShowDialogOrderInfo`, `DialogOK`, `DialogCancel`. Filters: `FilterLotNo`,
`FilterExternLotNo`, `FilterExternLotNo2`, `FilterMaterialNo`. The current lot is
`CurrentFacilityLot`; the expiration date logic runs via
`CurrentFacilityLotExpirationDateHelper`.
