# Lot Overview

> Business object: `BSOFacilityLotOverview` (Assembly `gip.bso.facility`, Menu: **Warehouse Management → Lot Overview**)

## Definition

The **Lot Overview** shows the current stock of all **lots/batches** (FacilityLot)
and the distribution of each lot over storage bins, storage locations and materials.

This app is a **read-only overview**. Lots are created by production and booking
processes; the overview itself cannot create or delete lots. It answers the questions
*which lots are in stock*, *how much of each lot*, *where is it stored* and *which
bookings were made with it*.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Lot / Batch** | Logistical unit for stock keeping and traceability |
| **Lot number** | Internal, unique number of the lot |
| **External lot number(s)** | Lot numbers assigned by the supplier (two fields) |
| **Quant** | Smallest stock unit; a lot consists of one or more quants in different storage bins |
| **Storage bin (Facility)** | Physical storage location of a quant |
| **Storage location** | Grouping level above storage bins (rooms/areas) |
| **Lock** | Lot is locked and must not be used |
| **Not available** | Quants of the lot flagged as not available |
| **Release state** | Release status of the quants |
| **In/Out sum** | Running sum of inward minus outward bookings of the lot |

## Screen layout

The screen consists of a search list on the left (**Explorer**) and detail tabs:

| Area | Content |
|---|---|
| **Explorer** (left) | Filter fields (Show not available, Lot No., Extern Lot No., Material No., Extern Lot No. 2) and the lot result list (sorted by lot number, newest first) |
| **Stock** (tab) | Lot header data (material, lot number, external lot number, dates, lock, comment) and nested tabs with the stock distribution of the lot |
| **FacilityBookingHistory** (tab) | Booking history of the selected lot for the selected period |

The main layout also embeds the **Tracking & Tracing fast view** which follows the
currently selected lot.

### Stock tab — nested tabs

| Tab | Content |
|---|---|
| **Facility** | Stock of the lot grouped by storage bin (total, free, blocked, blocked absolute) |
| **Facility Location** | Stock of the lot grouped by storage location |
| **Quants in Storage Bins** | All quants of the lot with split no., quantity, unit, storage bin, release state, dates |
| **Material** | Stock of the lot grouped by material |
| **Picking Reservation** | Picking reservations referencing quants of the lot |
| **Prod. Order Reservation** | Production order reservations referencing quants of the lot |

### Explorer filters

| Filter | Effect |
|---|---|
| **Show not available** | Only lots that have quants with the chosen availability flag |
| **Lot No.** | Contains-search on the lot number |
| **Extern Lot No.** | Contains-search on the first external lot number |
| **Extern Lot No. 2** | Contains-search on the second external lot number |
| **Material No.** | Contains-search on the material number of the lot |

Every change of a filter immediately re-executes the search.

### Lot list columns

Lot No., Material No., Material Name, Extern Lot No., Comment, Stock Quantity,
Stock Weight, Expiration Date, Filling Date, Handling Unit, Insert Date, Enabled,
Lock, Release State, Not Available, Production Date, Storage Life,
Storage Life Remaining.

## Step-by-step tasks

### Searching for a lot

1. Enter the **Lot No.**, **Extern Lot No.** or **Material No.** in the filter
   fields of the Explorer — the list refreshes automatically.
2. Tick **Show not available** to include lots whose quants are flagged not available.
3. Select a lot in the list — the **Stock** tab below is filled.

### Viewing the stock distribution of a lot

1. Select the lot in the list.
2. The upper part of the **Stock** tab shows material, lot number, external lot
   number, filling date, production date, storage life, expiration date, remaining
   storage life, the **Lock** flag and the comment.
3. Use the nested tabs to see the stock **by storage bin** (Facility), **by storage
   location** (Facility Location), the individual **quants**, the stock **by
   material**, and the **Picking Reservation** / **Prod. Order Reservation** lists.

### Viewing the booking history of a lot

1. Select the lot in the list.
2. Open the **FacilityBookingHistory** tab.
3. Set the **Search from** / **Search to** period (and optionally the posting type
   filter) and click **Refresh**.
4. The inward/outward bookings of the lot are listed with a running in/out sum per row.

### Editing a lot

The overview is primarily read-only, but lot header fields (e.g. **Lock**, comment)
can be corrected:

1. Change the field on the current lot.
2. Click **Save** to write the change to the database, or **Undo** to discard it.
3. **Load** re-reads the current lot including its stock if another user changed it
   in the background.

### Navigating from a lot

With a quant selected in the **Quants in Storage Bins** list, the following buttons
are available:

* **Show Order** — opens the production order that produced this quant (only if the
  quant originates from a production order).
* **Manage Quant** — opens the quant management dialog for stock corrections.
* **Manage Lot/Batch** — opens the lot management dialog for the selected lot.
* **Show Bin Stock and History** — jumps to the storage bin stock overview.
* **Manage Stock of Bin** — opens the storage bin management (only for storage bin
  containers).
* **Show Material Stock and History** — jumps to the material stock overview.

## Field reference (Stock tab)

| Field | Description |
|---|---|
| Material No. / Material Name | Material of the lot |
| Lot No. | Internal lot number |
| Extern Lot No. | First supplier lot number |
| Filling Date | Date the lot was filled/produced |
| Production Date | Production date of the lot |
| Storage Life | Shelf life in days |
| Expiration Date | Date until which the lot may be used |
| Storage Life Remaining | Remaining shelf life, calculated from the expiration date |
| Lock | Lot is locked and must not be used |
| Comment | Free comment |
| Sum Total / Sum Free / Sum Blocked / Sum Blocked Absolute | Aggregated stock columns of the distribution tabs |
| In/Out Sum | Running sum of inward minus outward bookings in the booking history |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityLotOverview`
contains the complete usage recipe (filter → `Search()` → `NavigateFirst()` →
`CurrentFacilityLot` → `Save()`/`Load()`, quant list `FacilityChargeList`,
aggregation lists `FacilityChargeSum*HelperList`, booking history `RefreshMovements`,
dialog entry points `ShowDialogOrder`/`ShowDialogOrderInfo` and all `NavigateTo*`
methods), so an agent can operate the app remotely, e.g. for user training. The
button captions above are the translations of these methods (`ACMethod*` attributes,
stored in `ACClassText`).
