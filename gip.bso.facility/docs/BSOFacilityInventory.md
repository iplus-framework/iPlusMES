# Warehouse Inventory (Stock-taking)

> Business object: `BSOFacilityInventory` (Assembly `gip.bso.facility`, Menu: **Warehouse Management → Warehouse Inventory**)

## Definition

A **Warehouse Inventory** (stock-taking) records the physical stock of a storage bin
(**Facility**) and compares it with the book inventory. The result is a list of
**inventory lines** — one per **quant** (stock unit / FacilityCharge) — in which the
**counted quantity** is entered. When the inventory is posted, the system books the
differences so that the book stock matches the physical stock.

An inventory runs through these states:

| State | Meaning |
|---|---|
| **New** | Inventory created, lines may still be generated; counting not started |
| **In progress** | Counting active — lines can be edited |
| **Finished** | Counting completed, ready for posting |
| **Posted** | Posted / sent to ERP — read-only |
| **Canceled** | Inventory canceled, all its bookings were reverted |

Inventory lines have their own states (**New**, **In progress**, **Finished**,
**Posted**). A line is *counted* when a counted quantity was entered or it was marked
**Not available**.

## Concepts

| Term | Meaning |
|---|---|
| **Inventory** | The stock-taking header: number, name, storage bin, state |
| **Inventory line** | One line per quant: material, storage bin, lot, stock quantity, counted quantity |
| **Quant** | A stock unit (FacilityCharge) in a storage bin — the object being counted |
| **Counted quantity** | The physically counted quantity (**New Stock Quantity**) |
| **Not available** | Marks a quant as not available (e.g. empty or blocked); no counted quantity is entered |
| **Generate positions** | Creates one inventory line per quant of the storage bin |
| **Omit silo quants** | Excludes silo quants from line generation |
| **Booking** | The stock movement created when the inventory is posted (visible per line and per inventory) |

## Screen layout

The app shows the **search list** on the left (filter: **from** / **to** date and
**Inventory Status**, buttons **Search** and **Reset**) and the **inventory form** on
the right with these tabs:

| Tab | Content |
|---|---|
| **Inventory Lines** | Filter bar (barcode, material, storage bin, lot, status, open lines, …), the line list with checkboxes, and the detail area of the selected line (edit + its bookings) |
| **Inventory Bookings** | All bookings created by this inventory, with filters for material, storage bin and lot |
| **Unused Quants** | Quants of the storage bin that are not yet in the inventory; they can be added to the inventory |

Dialogs: **New Inventory** (number, name, storage bin, generation options),
**Generate Positions**, and a message list.

## Creating a new inventory (step by step)

1. Click **New** in the toolbar. The **New Inventory** dialog opens with a suggested
   **Inventory No.**
2. Enter the **Name** of the inventory.
3. *(Optional)* Click **Choose facility** and select the storage bin to be counted.
   If no storage bin is chosen, the inventory covers all storage bins (the system asks
   for confirmation).
4. Set the options **Generate positions** (create one line per quant) and
   **Omit silo quants** (skip silo quants).
5. Click **New** (OK) in the dialog. The inventory is created in the background —
   a progress window is shown. **Close** cancels the dialog.
6. If **Generate positions** was active, the inventory lines are created immediately;
   otherwise they can be generated later (see below).

## Counting workflow (1 – 2 – 3)

The three lifecycle buttons are numbered in the toolbar:

### 1.) Start inventory

Click **1.) Start inventory**. The inventory state changes to **In progress** and the
lines become editable. If the inventory has no lines yet, the **Generate Positions**
dialog opens first — set the options and confirm with **Ok**.

### 2.) Count the lines (Inventory Lines tab)

1. Find the line: use the **Barcode** field (scan a quant barcode), or filter by
   **Material**, **Storage Bin**, **Lot No.**, **Inventory Line Status**,
   **View open lines** or **Lines whose inventory has changed**. The list reloads
   automatically; **Clear filter** resets all filters. **Limit Record Count** caps the
   number of loaded lines (default 500).
2. Select the line and enter the physically counted quantity in
   **New Stock Quantity** — or tick **Not Available** if the quant is not available
   (the counted quantity is then cleared).
   **Takeover stock** copies the book stock quantity into the counted quantity.
3. Optionally use the line status buttons: **Start** (line → *In progress*) and
   **Close filtered inventory lines** (line → *Finished*).
4. For mass processing tick the checkboxes in the list (or **Select all**) and use:
   * **2.) Set selected lines as counted** — marks all selected lines as counted
     (*Finished*).
   * **Copy quantity from stock** — takes over the book stock as counted quantity for
     all selected lines (after confirmation).
   * **Set not available** — marks all selected lines as not available (after
     confirmation).
   * **Change status for selected** — sets the status chosen next to it for all
     selected lines.
5. To add quants that are missing in the inventory, open the **Unused Quants** tab,
   click **Search**, select a quant and click **Restore quant** — or use
   **Add new quant to inventory list** to create a completely new quant via dialog.
6. Click **Save** (confirm warnings with **Yes**). **Undo** discards unsaved changes,
   **Load** re-reads the inventory if another user changed it in the meantime.

### 3.) Post and complete inventory

Click **3.) Post and complete inventory**. If lines are still open (not counted), the
system filters the list to the open lines and shows a warning. Otherwise, after
confirmation, the differences are booked in the background and the inventory is
completed. The created bookings are visible on the **Inventory Lines** tab (detail
area of the selected line) and on the **Inventory Bookings** tab.

## After posting

* **Send to ERP** — marks a finished or canceled inventory as posted (transfer to ERP).
* **Cancel Inventory** — cancels the inventory after confirmation; **all bookings made
  by this inventory are reverted** and the quants return to their previous state.
* **Delete** — deletes the inventory with all its lines (only while *New* or
  *In progress*).
* **Change facility** — changes the storage bin of the inventory (only while it has no
  lines).

## Field reference (header)

| Field | Description |
|---|---|
| Inventory No. | Unique number of the inventory (suggested automatically) |
| Name | Free name of the inventory |
| Storage Bin | Storage bin (Facility) being counted |
| Inventory Status | New / In progress / Finished / Posted / Canceled |
| Suggest Stock Quantity | If set, the book stock is preassigned as counted-quantity suggestion |
| from / to | Date-range filter of the search list (insert date) |
| Insert / Update | User and date of creation and last change |

## Field reference (inventory line)

| Field | Description |
|---|---|
| Material | Material number and name of the quant |
| Storage Bin | Storage bin of the quant |
| Lot No. | Lot number of the quant |
| Stock Quantity | Book stock at the time the line was generated |
| New Stock Quantity | Physically counted quantity |
| Not Available | Quant is not available (no counted quantity) |
| Status | New / In progress / Finished / Posted |
| Comment | Free comment on the line |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityInventory`
contains the complete usage recipe (filter → `Search` → `NavigateFirst` → edit
`SelectedFacilityInventory` → `Save`) including all C# method names
(`New`, `NewDlgOk`, `StartInventory`, `SearchPos`, `SelectPos`, `CopyQuantityFromStock`,
`CloseAllPositions`, `ClosingInventory`, `SendToERP`, `CancelInventory`,
`AddFacilityCharge`, `NewFacilityCharge`, `GeneratePositions`, `Delete`, `Load`, ...),
so an agent can operate the inventory app remotely. The button captions above are the
translations of these methods (`ACMethod*` attributes, stored in `ACClassText`).
