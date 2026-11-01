# Purchase Delivery Note (Goods Receipt)

> Business object: `BSOInDeliveryNote` (Assembly `gip.bso.purchasing`, Menu: **Purchasing → Goods Receipt / Delivery Note**)

## Definition

The **Purchase Delivery Note** app (German: *Warenannahme* — "Goods Receipt") covers the complete
incoming goods process: it manages **delivery notes of type *Receipt*** together with their
**positions**, assigns **open purchase order positions** to the delivery note and **books the
incoming material into stock** (stock inward movements with lot/quant handling).

It replaces the scattered classic VarioBatch goods receipt screens with one central, flexible app.

## Concepts

| Term | Meaning |
|---|---|
| **Delivery Note** | The goods receipt header: delivery note number, delivery date, supplier, shipper, weights, state |
| **Delivery Note Position** | One line of the delivery note; created by assigning an open purchase order position |
| **Open Order Lines** | Purchase order positions that are not yet completely assigned (filterable by material, transport mode, delivery date) |
| **Partial Quantity** | Optional quantity to assign only part of an open purchase order position |
| **Pre-Booking** | A prepared (not yet posted) stock booking for a position; its parameters (bin, quant, lot, quantity) are edited before posting |
| **Quant** | A stock container (facility charge) holding a material quantity in a bin |
| **Lot** | Batch number of a lot-managed material |
| **Booking History** | The posted bookings (with charge lines) of a position |
| **Rating / Complaint** | Supplier/shipper/finisher score of the delivery, with a complaint dialog |

## Screen layout

The app consists of a **search list** (left) and the **delivery note form** (right):

| Area / Tab | Content |
|---|---|
| **Search list** | Delivery note number, delivery date, supplier, shipper, state; filters: search word, **Delivery state**, **Comment**; **Search** button |
| **Delivery Note** (header tab) | Delivery note data: delivery date, supplier/shipper address, supplier delivery no., weights (empty, net, gross, loss, total, order/delivery/stock in/out), rating, **Cancel Delivery Note**, **Create or update picking**, **Show picking order** |
| **Position List** | Positions of the delivery note (sequence, order no., material, state, target/actual quantity, unit, delivery date) with **Remove** |
| **Assign Order Positions** | Open purchase order lines with filters (material, transport mode, delivery date from/to, partial quantity) and **Assign** / **Filter** / **Find open order lines** |
| **Assign Picking** | Open picking positions with **Assign** |
| **Posting** | Pre-bookings of the selected position: booking parameters (inward bin, quant, lot, quantity, split no.), **New Posting**, **Cancel Posting**, **Delete Posting**, **Post**, **Post all**, **Post Item**, **New Lot**, **Show Lot**, **Choose quant** |
| **Booking History** | Posted bookings of the position with charge lines (booking no., type, material, lot, bin, split, quantity, movement reason, user, date) |
| **Order Data** (position form) | Purchase order data of the selected position: material, order no., quantities, prices, delivery dates/priority, load state, country tax, comments; **Show purchase order**, **New Lab Order**, **Lab Report** |
| **Machines** | Machine-related sub-layout |

## Creating a goods receipt (step by step)

1. Click **New** — a new delivery note with a new number is created and opened.
2. Enter the header data on the **Delivery Note** tab (delivery date, supplier, shipper, weights).
3. Click **Save**.

## Assigning purchase order positions

On the **Assign Order Positions** tab:

1. *(Optional)* Set the filters **Material**, **Filter mode of transport**,
   **Deliv.date from** / **Deliv.date to** and click **Filter** or
   **Find open order lines** to refresh the list.
2. Select the open order line. For a partial delivery enter the **Partial Quantity** first.
3. Click **Assign** — a delivery note position is created and selected.
4. Click **Save** (confirm warnings with **Yes**).

To take back an assignment select the position in the **Position List** and click **Remove**.

### Positions from picking orders

On the **Assign Picking** tab select a picking position and click **Assign**.

## Booking the goods receipt into stock

On the **Posting** tab, with a position selected:

1. Click **New Posting** — a pre-booking is created and its **Posting parameter** form opens.
2. Fill the booking parameters: **Inward Facility** (target bin/container),
   **Inward Facility Charge** (existing quant) or a new lot, **Inward Quantity**, split no.
   * Use **Choose quant** to pick an existing stock quant (confirm with **OK**,
     abort with **Close**).
   * For lot-managed materials use **New Lot** to create a lot, **Show Lot** to view it.
   * The checkbox **Only show bins with material** restricts the bin list to bins that
     already contain the material.
3. Click **Post** to post this pre-booking (stock inward movement), or **Post all** to post
   all pre-bookings of the position. **Post Item** is a placeholder without function.
4. **Delete Posting** removes the current pre-booking; **Cancel Posting** prepares a
   cancellation booking.
5. Click **Save** — the order position state is updated automatically.

Posted bookings appear on the **Booking History** tab.

## Finishing, cancelling

* **Finish** — completes the delivery note (state *Completed*).
* **Cancel Delivery Note** — cancels the whole delivery note: all pre-bookings are cancelled,
  cancellation bookings are posted and the state is set to *Cancelled* when all assigned
  positions are cancelled.
* **Load** — re-reads the delivery note if another user changed it in the background.
* **Undo** — discards unsaved changes.

## Picking

* **Create or update picking** — creates or updates a picking order from the delivery note
  and completes it.
* **Show picking order** — opens the linked picking order.

## Lab orders and purchase order

On the **Order Data** tab of a position:

* **Show purchase order** — opens the purchase order of the position.
* **New Lab Order** — creates a lab order for the position (only if none exists).
* **Lab Report** — shows the existing lab order.

## Supplier rating

On the **Delivery Note** tab the **Delivery Company Rating**, **Shipper Company Rating** and
**Finisher Company Rating** can be entered. With a rating > 0 the **Rating Complaint** buttons
open the complaint dialog for the respective company.

## Field reference (header)

| Field | Description |
|---|---|
| Delivery Note No. | Number of the delivery note (assigned automatically on **New**) |
| Delivery Date | Date of the delivery |
| Supplier Delivery No. | Delivery note number of the supplier |
| Delivery Company / Shipper / Delivery 2 | Supplier, forwarder and finisher address |
| Delivery State | State of the delivery note (also used as list filter) |
| Empty Weight / Net Weight / Loss Weight / Total Weight | Weighing data |
| Order In/Out, Delivery In/Out, Stock In/Out | Weight bookkeeping values |
| Comment | Free comment (also usable as search filter) |
| Insert/Update Name/Date | Audit fields |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOInDeliveryNote` contains the
complete usage recipe (search → navigate → edit → save) including the C# method names
(`Search`, `NavigateFirst`, `New`, `Save`, `Delete`, `Load`, `AssignInOrderPos`,
`UnassignInOrderPos`, `AssignPicking`, `NewFacilityPreBooking`, `BookCurrentACMethodBooking`,
`BookAllACMethodBookings`, `CancelDelivery`, `CompleteInDeliveryNote`,
`CreateOrUpdatePicking`, `ShowDlgInwardAvailableQuants`, `NewFacilityLot`, ...), so an agent
can operate the goods receipt app remotely. The button captions above are the translations of
these methods (`ACMethod*` attributes, stored in `ACClassText`).
