# Sales Delivery Note (Lieferschein Verkauf)

> Business object: `BSOOutDeliveryNote` (Assembly `gip.bso.sales`, Menu: **Sales → Delivery Note**)

## Definition

A **sales delivery note** documents the goods issue to a customer. It collects
**positions** taken either from **open sales order positions** or from already
**picked order positions**, and posts the **outward stock movements** (bookings)
for each position. From a completed delivery note an **invoice** can be created
and a **picking order** can be generated or displayed.

## Concepts

| Term | Meaning |
|---|---|
| **Delivery Note** | The header: delivery note number, delivery date, delivery state, addresses, weights |
| **Delivery Note Position** | One line of the delivery note, referencing a sales order position (material, quantity, state) |
| **Open Order Position** | A sales order position that is not yet *Completely Assigned* and can be added to the delivery note |
| **Picked Position** | An order position that was already picked (picking type *Issue* / *IssueVehicle*) and is not yet on a delivery note |
| **Pre-Booking** | A planned (not yet posted) stock movement for a position, with its posting parameters (outward storage bin, charge, lot, quantity) |
| **Booking / Booking History** | The already posted stock movements of a position and their charges (read only) |
| **Partial Quantity** | Quantity for a partial delivery when only part of the ordered quantity is delivered |
| **Quant** | A physical stock entry (storage bin + charge/lot + quantity) that can be chosen as posting source |

## Screen layout

The app is a search list on the left (**Explorer**) and the delivery note form on the
right, plus a message area. The delivery note form has two main tabs:

| Tab | Content |
|---|---|
| **Delivery Note** | Header data: delivery note number, delivery date, delivery state, delivery/shipper address, supplier delivery number, weights (empty, net, loss, total, delivery/order/stock in/out) and the buttons **Cancel Delivery Note**, **Create Invoice**, **Show Invoice**, **Create or update picking**, **Show picking order** |
| **Delivery Note Positions** | Nested tabs for the positions (see below) |

The **Delivery Note Positions** tab contains these nested tabs:

| Tab | Content |
|---|---|
| **Position List** | List of the delivery note positions (order, material, target/actual quantity, unit, delivery date, state) with the **Remove** button |
| **Position Form** | Detail form of the selected position (material, quantities, prices, delivery dates, load state, comments) with **New Lab Report** and **Lab Report** |
| **Assign Order Position** | List of open sales order positions with filters (material, delivery date from/to, mode of transport) and the **Assign** button plus **Partial Quantity** field and **Filter** |
| **Assign Picking** | List of picked order positions with the **Assign** button |
| **Booking** | Pre-bookings of the selected position and their posting parameters with **New Posting**, **Cancel Posting**, **Delete Posting**, **Post**, **Post Item**, **Post All**, **Choose quant**, **Show Lot**, **New Lot** and the **Show only bins w. material** checkbox |
| **Booking History** | Posted bookings of the position and their charges (read only) |

The **Choose quant** dialog lists the available quants of the position material
(bin, lot, external lot, split, stock quantity, expiration/filling date, release state)
with **OK** and **Close**.

## Creating a new delivery note (step by step)

1. Click **New** in the toolbar. A new delivery note with a new number is created
   (type *Issue*) and opened.
2. Enter the header data (delivery date, addresses, comment).
3. Click **Save**. If a warning message appears, confirm with **Yes**.

## Adding positions from open order positions

On the **Assign Order Position** tab:

1. Restrict the list with the filters **Material**, **Deliv.date from**,
   **Deliv.date to** and **Filter mode of transport**, or open the extended
   **Filter** dialog. Click **Search** to refresh.
2. Select the order position to deliver.
3. For a partial delivery enter the **Partial Quantity**.
4. Click **Assign** — a new position is added to the delivery note.
5. Click **Save** (confirm warnings with **Yes**).

## Adding positions from picking

On the **Assign Picking** tab:

1. Select a picked order position from the list.
2. Click **Assign** — the position is added to the delivery note.
3. Click **Save**.

## Removing a position

On the **Position List** tab select the position and click **Remove**.
Then click **Save**.

## Posting the stock movements (Booking tab)

For each position the goods issue must be posted:

1. Select the position in the **Position List** tab.
2. On the **Booking** tab click **New Posting** — a pre-booking with default
   posting parameters is created.
3. Set the posting parameters: **outward storage bin**, **charge**, **lot**,
   **quantity** and unit. With **Show only bins w. material** the bin list is
   restricted to bins containing the position material.
4. Optionally click **Choose quant** and select a concrete quant in the dialog,
   confirm with **OK** (**Close** cancels).
5. For lot-managed materials assign or create a lot: **Show Lot** displays the
   current lot, **New Lot** creates a new one and assigns it.
6. Click **Post** to book the outward movement. The pre-booking is deleted, the
   actual quantity of the order position is recalculated and its state is set to
   *Completed* (or *Cancelled* for cancellation bookings). The delivery note is
   saved automatically.
7. **Post All** posts all pre-bookings of the position one after the other.
8. **Delete Posting** removes the current pre-booking; **Cancel Posting** cancels
   the pre-bookings of the position (only enabled when the position has no
   pre-bookings).

The **Booking History** tab shows the posted bookings of the position with their
charges (read only). From the booking context menu you can start **Tracking &
Tracing**.

## Finishing, cancelling, invoicing

On the **Delivery Note** tab:

* **Finish** — completes the delivery note (state *Completed*). Enabled only while
  the state is below *Completed*.
* **Cancel Delivery Note** — cancels all pre-bookings and bookings; if all
  assigned positions are cancelled, the delivery note state is set to *Cancelled*
  and saved.
* **Create Invoice** — after a confirmation question, creates an invoice from the
  delivery note (only if every position is assigned to an order position and no
  invoice exists yet).
* **Show Invoice** — opens the invoice created from this delivery note.
* **Create or update picking** — creates or updates a picking order from the
  delivery note, assigns the source storage bin to each picking position and sets
  the state to *Completed*.
* **Show picking order** — opens the picking order of the delivery note.

## Lab orders

On the **Position Form** tab, for the selected position:

* **New Lab Report** — creates a new lab order (only if none exists).
* **Lab Report** — shows the existing lab order.

## Toolbar functions

* **Save** — saves all changes (confirm warnings with **Yes**).
* **Undo** (**Don't save**) — discards unsaved changes.
* **Load** — re-reads the delivery note if another user changed it in the background.
* **New** / **Delete** — create or delete a delivery note.
* **Search** — refreshes the search list (filters: **Delivery state**, **Comment**).

## Field reference (header)

| Field | Description |
|---|---|
| Delivery Note No. | Number of the delivery note (searchable) |
| Delivery Date | Date of delivery |
| Delivery State | Status of the delivery note (e.g. *Cancelled*, *Completed*) |
| Delivery Address / Shipper | Delivery and shipper company addresses |
| Supplier Delivery No. | Delivery note number of the supplier |
| Empty Weight / Net Weight / Loss Weight / Total Weight | Weight fields of the delivery |
| Delivery Weight (Order/Stock/Delivery In/Out) | Weight bookings of the delivery |
| Partial Quantity | Quantity for a partial delivery (Assign Order Position tab) |
| Comment | Free comment (also usable as search filter) |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOOutDeliveryNote`
contains the complete usage recipe (search → navigate → edit → save) including the
C# method names: `Search`, `NavigateFirst`, `New`, `Save`, `Delete`, `Load`,
`AssignOutOrderPos`, `UnassignOutOrderPos`, `AssignPicking`,
`NewFacilityPreBooking`, `DeleteFacilityPreBooking`, `CancelFacilityPreBooking`,
`ShowDlgOutwardAvailableQuants`, `DlgAvailableQuantsOk`,
`BookCurrentACMethodBooking`, `BookAllACMethodBookings`, `NewFacilityLot`,
`ShowFacilityLot`, `CompleteOutDeliveryNote`, `CancelDelivery`, `CreateInvoice`,
`ShowInvoice`, `CreateOrUpdatePicking`, `ShowPickingOrder`,
`ShowDialogNewDeliveryNote`, `ShowDialogOrder`, `ShowDialogOrderInfo`.
The button captions above are the translations of these methods (`ACMethod*`
attributes, stored in `ACClassText`).

Note: `DeliveryNoteReady`, `Delivered` and `BookDeliveryPos` are TODO stubs
(not implemented).
