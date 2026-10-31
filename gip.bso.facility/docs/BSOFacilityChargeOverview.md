# Quant Overview

> Business object: `BSOFacilityChargeOverview` (Assembly `gip.bso.facility`, Menu: **Warehouse Management → Quant Overview**)

## Definition

The **Quant Overview** shows the current stock of all **quants** (FacilityCharge).
A **quant** is the smallest stock-keeping unit in the warehouse: one material in one
storage bin belonging to one lot/batch.

This app is a **read-only overview**. Quants are created, changed and consumed
exclusively by **stock bookings** (booking apps such as goods receipt, goods issue,
production bookings). The overview itself cannot create or delete quants — it answers
the questions *what is in stock*, *where*, *for which material and lot*, and *until
when is it usable*.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Quant** | Smallest stock unit: material + storage bin + lot + quantity |
| **Lot / Batch** | Logistical unit for traceability; a quant always belongs to exactly one lot |
| **Storage bin (Facility)** | Physical storage location of the quant |
| **Storage location** | Grouping level above storage bins (rooms/areas) |
| **Split No.** | Split number of the quant within the lot |
| **Not available** | Flag on a quant that excludes it from availability (e.g. blocked, in quality check) |
| **Release state** | Release status of the quant (e.g. released, locked) |
| **Reserved quantity** | Quantity reserved by picking or production order reservations |
| **Available quantity** | Stock quantity minus reserved quantity |
| **Expiration date** | Date until which the quant may be used (from the lot) |

## Screen layout

The screen consists of a search list on the left (**Explorer**) and detail tabs at the
bottom/right:

| Area | Content |
|---|---|
| **Explorer** (left) | Filter fields (Show not available, Material, Storage, Lot No., Extern Lot No., Expiration date) and the quant result list |
| **Stock** (tab) | Quant details: left the quant identification with navigation buttons, right two nested tabs — **Stock** (quantities) and **Quant Information** (dates, order and company data) |
| **FacilityBookingHistory** (tab) | Booking history of the selected quant (inward/outward bookings for the selected period) |
| **PickingReservation** (tab) | Picking reservations referencing the selected quant |
| **ProdOrderReservation** (tab) | Production order reservations referencing the selected quant |

The main layout also embeds the **Tracking & Tracing fast view** which follows the
currently selected quant.

### Explorer filters

| Filter | Effect |
|---|---|
| **Show not available** | Include quants flagged *Not available* (default: hidden) |
| **Material** | Searches material number **or** material name |
| **Storage** | Searches storage bin number **or** name |
| **Lot No.** | Lot number of the quant |
| **Extern Lot No.** | First external (supplier) lot number |
| **Extern Lot No. 2** | Second external lot number |
| **Expiration date less then** | Only quants expiring on or before this date; set via the **Today** / **Next [0] days** / **Reset** buttons (period configurable, default 30 days) |

Every change of a filter immediately re-executes the search.

### Quant list columns

Material No., Material Name, Storage Bin No., Storage Bin Name, Stock Quantity,
Stock Quantity (UOM), Unit, Release State, Lot No., Split No., Extern Lot No.,
Not Available, Expiration Date, Storage Life Remaining, Filling Date, Production
Date, Comment, Insert Date, plus additional unit columns (Stock Unit A/B/C with
their units).

## Step-by-step tasks

### Searching for stock

1. Enter the **Material**, **Storage**, **Lot No.** or **Extern Lot No.** in the
   filter fields of the Explorer — the list refreshes automatically.
2. To find soon-expiring stock, click **Today** or **Next [0] days** (period from the
   configuration, default 30) next to the expiration date filter; **Reset** clears it.
3. Tick **Show not available** to include blocked quants.
4. Select a quant in the list — the detail tabs below are filled.

### Viewing quant details

1. Select the quant in the list.
2. The left part of the **Stock** tab shows material, storage bin, lot number,
   split number, external lot numbers, stock quantity, unit, release state and the
   *Not available* flag.
3. The nested **Stock** tab shows Stock Quantity, Reserved Outward/Inward Quantity,
   Reserved Quantity, Available Quantity.
4. The nested **Quant Information** tab shows Filling Date, Sort No., Production
   Date, Expiration Date, Storage Life, Comment, In-Order No., Program No. and
   Company No./Name.

### Viewing the booking history

1. Select the quant in the list.
2. Open the **FacilityBookingHistory** tab.
3. Set the **Search from** / **Search to** period (and optionally the posting type
   filter) and click **Refresh**.
4. The inward/outward bookings of the quant are listed with running in/out sum.

### Viewing reservations

1. Select the quant in the list.
2. Open the **PickingReservation** or **ProdOrderReservation** tab to see which
   picking orders or production orders have reserved this quant.

### Navigating from a quant

With a quant selected, the following buttons are available in the **Stock** tab:

* **Show Order** — opens the production order that produced this quant (only if the
  quant originates from a production order).
* **Manage Quant** — opens the quant management dialog for stock corrections.
* **Show Lot Stock and History** — jumps to the *Lot Overview* for the lot of this quant.
* **Manage Lot/Batch** — opens the lot management dialog.
* **Show Material Stock and History** — jumps to the material stock overview.
* **Manage Material** — opens the material master data.
* **Show Bin Stock and History** — jumps to the storage bin stock overview.
* **Manage Stock of Bin** — opens the storage bin management (only for storage bin
  containers).

### Editing a quant

The overview is primarily read-only, but fields of the current quant (e.g. comment,
release state) can be corrected:

1. Change the field on the current quant.
2. Click **Save** to write the change to the database, or **Undo** to discard it.
3. **Load** re-reads the current quant if another user changed it in the background.

## Field reference (Stock tab)

| Field | Description |
|---|---|
| Material No. / Material Name | Material of the quant |
| Storage Bin No. / Name | Storage bin where the quant is stored |
| Lot No. | Lot the quant belongs to |
| Split No. | Split number of the quant within the lot |
| Extern Lot No. / Extern Lot No. 2 | Supplier lot numbers |
| Stock Quantity / Stock Quantity (UOM) | Stock in base unit / in the unit of measure |
| Reserved Outward / Inward / Reserved Quantity | Reserved quantities of the quant |
| Available Quantity | Stock minus reservations |
| Not Available | Quant is excluded from availability |
| Release State | Release status of the quant |
| Filling Date / Production Date | Dates from the lot/production |
| Expiration Date / Storage Life | Usable-until date and shelf life |
| Storage Life Remaining | Remaining shelf life |
| In-Order No. / Program No. | Order/program references from production |
| Company No. / Company Name | Company (client) of the quant |
| Comment | Free comment |
| Insert Date | Creation date of the quant |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityChargeOverview`
contains the complete usage recipe (filter → `Search()` → `NavigateFirst()` →
`CurrentFacilityCharge` → `Save()`/`Load()`, expiration date filters
`SetFilterExpirationDateToday`/`SetFilterExpirationDateNextDays`/`ResetFilterExpirationDate`,
aggregation lists `FacilityChargeSum*HelperList`, booking history `RefreshMovements`
and all `NavigateTo*` methods), so an agent can operate the app remotely, e.g. for
user training. The button captions above are the translations of these methods
(`ACMethod*` attributes, stored in `ACClassText`).
