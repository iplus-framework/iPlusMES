# Picking Order

> Business object: `BSOPicking` (Assembly `gip.bso.logistics`, Menu: **Logistics → Picking Order**)

## Definition

A **picking order** (commission) controls the physical movement of material in the
warehouse: material is taken from a **source storage bin** (From Facility) and moved to a
**target storage bin** (To Facility). Depending on the **Picking Type** the movement is a

* **Receipt** (goods receipt picking — material is put away),
* **Issue** (goods issue picking — material is picked for a customer),
* **Relocation** (internal transfer between bins).

A picking order consists of one or more **picking positions**. Each position can be
**assigned** to a source document — an open purchase order position, an open sales order
position or a production order position — so the picking quantity is booked against that
document. The physical movement is prepared as a **pre-booking** (planned posting) and is
executed with the **booking** functions; afterwards the order can be **finished**.

The app also supports **facility reservations**, **weighings** (scale or manual),
**lab orders**, **visitor vouchers**, **process workflows with transports**,
**route checking** against production schedules, **mirroring** to remote stores and a
**picking scheduler** for time-planning the orders.

## Concepts

| Term | Meaning |
|---|---|
| **Picking order** | Order header: picking number, picking type, state, delivery address, delivery dates |
| **Picking position** | One material line: material, target/actual quantity, source and target bin, load state |
| **Picking Type** | Receipt, Issue or Relocation — determines the direction of the movement |
| **From Facility / To Facility** | Source resp. target storage bin of the position |
| **Assignment** | Link of a picking position to an open purchase/sales/production order position |
| **Pre-booking** | Planned posting (FacilityPreBooking) with booking parameters; not yet posted |
| **Booking** | Executed stock movement; converts pre-bookings into real facility bookings |
| **Quant** | A stock charge (FacilityCharge) of a material on a bin, possibly lot-managed |
| **Reservation** | Facility reservation that reserves stock for a picking position |
| **Supply picking** | Sub-order ("Create pickings for supply") that provides material for this order |
| **Load state** | Delivery position load state of the assigned order position (e.g. new, picked, loaded) |
| **Process workflow** | Optional workflow assigned to the order; transports can be started for it |
| **Route check** | Validates the order against other pickings and production batch plans for conflicts |
| **Mirroring** | Forwards the order to remote mirrored stores (facilities mirrored across databases) |

## Screen layout

The app consists of a **search list** (left, *Explorer*) and the **order form** (right,
*PickingForm*), plus a **Messages** area. The Explorer offers the filters **Picking-No.**,
**From** / **to** (delivery date), **Picking Type**, **Picking Status**, **Delivery
Address**, **From facility** / **To facility** (with the **Choose facility** dialogs),
**Material No.**, **Lot No.** and **Ext. Lot No.** (booking and reservation), the
**Search** and **Clear** buttons and the result columns (picking no., type, state,
preparation status, delivery date, visitor voucher, created/updated by).

The order form has these tabs:

| Tab | Content |
|---|---|
| **Picking Data** (first tab) | Header data: picking no., picking type, state, delivery address, delivery date from/to, comments, visitor voucher; buttons **Finish order**, **Cancel Picking**, **Create pickings for supply**, **Recalculate Actual Quantity**, **Broadcast picking** |
| **Picking Pos** | Positions of the order — itself nested (see below) |
| **Workflow** | Process workflow of the order: **Add** / **Remove** workflow, **Preferred Param**, **Start Transports**, **Check Routes**, **Create pickings for supply** |

The **Picking Pos** tab contains:

| Section | Content |
|---|---|
| **Position list** | Positions (sequence, material, target/actual quantity, unit, from/to bin) with **Add** and **Remove** |
| **Position form** | Detail of the selected position: material, quantities, from/to bin (**Choose facility**), load state, shipment status of the assigned order, comments; **New Lab Order**, **Lab Report**, **Show delivery note** |
| **Assignments** | Three assignment lists: open purchase order positions (**Assign**, **Assign Purchase Delivery Note**, **Filter**), open sales order positions (**Assign**, **Assign Sales Delivery Note**, **Filter**) and open production order positions (**Assign**, **Filter**) |
| **Booking** | Booking list of the position with **New Posting**, **Cancel Posting**, **Delete Posting**, **Post Item**, **Post**, **Post All**; the booking form switches between **BookingInward**, **BookingOutward** and **BookingRelocation** layouts depending on the picking type |
| **Booking history** | Posted bookings with details (booking no., type, outward/inward bin, lot, quantity, movement reason) |
| **Reservation** | Facility reservations of the position (child component *Facility Reservation*) |
| **Weighings** | Weighings of the position: scale selection, **Register Weight**, **Weight manuell** |

The **Booking** form offers, depending on direction: **Choose facility** (inward/outward),
**Choose quant**, **New Lot**, **Show Lot**, quantity, split no. and unit.

## Searching and creating picking orders

1. Enter the search word or set the filters in the Explorer and click **Search**.
   **Clear** resets all filters.
2. Click **New** in the toolbar. A new picking order with a new picking number is created;
   the picking type and delivery address are taken from the current filter selection
   (default: Receipt).
3. Fill in the header data on the *Picking Data* tab: **Delivery Address**,
   **Delivery Date From/To**, comments.
4. Click **Save**.

## Adding and assigning positions

1. On the *Picking Pos* tab click **Add** — a new position is created; source and target
   bin are pre-set from the filter selection.
2. Assign the **Material** and enter the **Target Quantity** with unit.
3. Set **From Facility** / **To Facility** with the **Choose facility** buttons in the
   position form.
4. **Remove** deletes the selected position from the order.
5. Click **Save** (confirm warnings with **Yes**).

### Assigning an order document

In the *Assignments* section of the position:

* **Purchase**: select an open purchase order position and click **Assign** — the picking
  position is linked to it and shows its data (state, load state, delivery date).
  Alternatively select a purchase delivery note position and click
  **Assign Purchase Delivery Note**. **Filter** restricts the list of open positions.
* **Sales**: same with **Assign** / **Assign Sales Delivery Note** on open sales order
  positions.
* **Production**: select an open production order position and click **Assign**.
* **Show delivery note** opens the delivery note of the assigned order position.

## Booking material movements

The physical movement is prepared as a **pre-booking** and then posted:

1. Select the position and go to the *Booking* section. Click **New Posting** — a
   pre-booking with booking parameters is created (the booking form shows the
   *BookingInward*, *BookingOutward* or *BookingRelocation* layout depending on the
   picking type).
2. Complete the booking parameters:
   * **Choose facility** — select the inward resp. outward storage bin.
   * **Choose quant** — select a concrete stock charge (quant); the dialog shows bin, lot,
     split, stock quantity, expiration and release state. Confirm with **OK**,
     **Close** cancels.
   * **New Lot** — create a new lot for lot-managed material; **Show Lot** displays the
     lot of the booking.
   * Enter the **Inward/Outward Quantity** and, if needed, the **Split No.**.
3. **Post Item** books the selected pre-booking; **Post** books the current booking;
   **Post All** books all pre-bookings of the position;
   **Post all positions** books all pre-bookings of all positions of the order.
4. **Cancel Posting** cancels the pre-booking of the position (reverts the planned
   movement); **Delete Posting** removes a pre-booking that was not yet posted.
5. The *Booking history* section lists the posted bookings with all details.
6. Click **Save**.

The **Only show source bins with material** / **Only show target bins with material**
options restrict the bin lists to bins that actually contain the material.

## Finishing, cancelling and copying orders

* **Finish order** — completes the picking order: delivery processing, bookings and
  status update are executed by the picking manager. Confirm warnings with **Yes** to
  force completion.
* **Cancel Picking** — cancels the order after confirmation: all pre-bookings are
  cancelled, existing bookings are posted, and the order state is set to *Cancelled* when
  all assigned positions are cancelled.
* **Mirror picking** — creates a copy of the order with a new picking number, including
  all positions and bin assignments.
* **Create pickings for supply** — creates supply pickings that provide the material for
  this order (available on the *Picking Data* tab and in the *Workflow* tab).
* **Recalculate Actual Quantity** — recalculates the actual quantities of all positions
  from the posted bookings.
* **Broadcast picking** — sends the order to remote mirrored stores (for facilities that
  are mirrored across databases).
* **Load** — re-reads the order if another user changed it in the background.

## Workflow, transports and route check (Workflow tab)

1. **Add** opens the workflow selection dialog; select a **Process Workflow** and confirm —
   the workflow is assigned to the order. **Remove** unassigns it.
2. **Preferred Param** opens the preferred parameters dialog of the workflow node.
3. **Start Transports** starts the transport workflow for the order: warnings are
   confirmed, an application manager is selected in a dialog (**OK** / **Cancel**) and the
   picking process is started.
4. **Check Routes** validates the order against other pickings and production batch
   plans; **Route check over orders** runs the check across several orders and shows the
   result dialog (**Close**).
5. **Create pickings for supply** creates supply pickings for the order.

## Weighings (Weighings section)

1. Select the **Scale** and the picking position.
2. **Register Weight** records a weight from the connected scale (an error message
   appears if the scale is not connected).
3. **Weight manuell** — enter the weight manually and record it as a weighing of the
   position.

## Lab orders and visitor voucher

* **New Lab Order** — creates a lab order for the selected position (quality control).
* **Lab Report** — shows the lab report of the selected position.
* **Show Visitor voucher** — displays the visitor voucher assigned to the picking order.

## Picking scheduler

The **Picking scheduler** (*Kommission Zeitplaner*) time-plans picking orders on a
timeline: **Backward scheduling** calculates from a planned end date, **Forward
scheduling** from a planned start date, **Calculate All** recalculates all lines,
**Switch to Readiness** / **Reset Readiness** set or reset the *ready to start* state of
selected pickings, and **Show** displays the scheduled orders in the timeline. The
scheduler also offers the **Route check over orders**. From the timeline (and from
tracking) the **Dialog Picking Order** opens the order of a scheduled line directly.

## Field reference (header)

| Field | Description |
|---|---|
| Picking No. | Unique number of the picking order |
| Picking Type | Receipt, Issue or Relocation |
| Picking State | State of the order (e.g. new, in process, cancelled, finished) |
| Delivery Address | Customer/supplier address the picking is for |
| Delivery Date From / to | Requested delivery period |
| Key of Ext. Sys. | External system key |
| Comment / Comment 2 | Free comments |
| Visitor Voucher | Assigned visitor voucher |
| Preparation Status | Preparation status shown in the result list |

Field reference (position): **Sequence**, **Material** (No./Name), **Target Quantity**,
**Actual Quantity**, **Unit**, **From Facility**, **To Facility**, **Picking Material**,
**Load State** (own and of the assigned order position), **Shipment Status** of the
assigned sales order, **Comment**.

Field reference (booking): **Inward/Outward Facility**, **Inward/Outward Facility
Charge** (quant), **Inward/Outward Facility Lot**, **Inward/Outward Quantity**,
**Split No.**, **Unit**, **Movement Reason** (history).

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOPicking` contains the
complete usage recipe (search → navigate → edit → save) including the C# method names:
`Search`, `New`, `Save`, `Delete`, `Load`, `AddPickingPos`, `UnassignPickingPos`,
`ShowDlgFromFacility`, `ShowDlgToFacility`, `NewFacilityPreBooking`,
`CancelFacilityPreBooking`, `DeleteFacilityPreBooking`, `BookDeliveryPos`,
`BookCurrentACMethodBooking`, `BookAllACMethodBookings`, `BookAllPositions`,
`NewFacilityLot`, `ShowFacilityLot`, `ShowDlgInwardAvailableQuants`,
`ShowDlgOutwardAvailableQuants`, `FinishOrder`, `CancelPicking`, `MirrorPicking`,
`GenerateSubPickingsForSupply`, `RecalcActualQuantity`, `ProcessWorkflowAssign`,
`StartWorkflow`, `ValidateRoutes`, `RunPossibleRoutesCheck`, `BroadCastPicking`,
`CreateNewLabOrder`, `ShowLabOrder`, `ShowDeliveryNote`, `NavigateToVisitorVoucher`,
`RegisterWeight`, `AddManualWeighing`, `FilterClear`, `ShowDialogOrder` /
`ShowDialogOrderInfo` (dialog for scheduler/tracking calls). Assignment of order documents
is implemented in the partial classes `BSOPicking_InOrder` (`AssignInOrderPos`,
`AssignDNoteInOrderPos`), `BSOPicking_OutOrder` (`AssignOutOrderPos`,
`AssignDNoteOutOrderPos`) and `BSOPicking_ProdOrder` (`AssignProdOrderPartslistPos`).
The button captions above are the translations of these methods (`ACMethod*` attributes,
stored in `ACClassText`). Time planning is done by `BSOPickingScheduler`, target/destination
planning by `BSOPickingTargetSelector`.
