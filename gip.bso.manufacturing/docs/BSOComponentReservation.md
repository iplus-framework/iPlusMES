# Module Reservations (Component Reservation Dialog)

> Business object: `BSOComponentReservation` (Assembly `gip.bso.manufacturing`)

## Definition

The **Module Reservations** dialog shows the **open facility reservations** of
a module or component (e.g. a process module or a work center). A reservation
links a storage quantity (charge) to an order position — an incoming order
position, a production order position or a batch plan. The dialog is opened by
another component (e.g. a process module) and is read-only.

## Concepts

| Term | Meaning |
|---|---|
| **Facility reservation** | Booking of a charge for a specific order position |
| **Open reservation** | Reservation whose order position is not yet delivered/finished/cancelled |
| **InOrderPos** | Position of an incoming delivery note |
| **ProdOrderPartslistPos** | Position of a production order (BOM position) |
| **ProdOrderBatchPlan** | Batch plan of a production order |

## Screen layout

A dialog with the reservation list and buttons:

| Element | Content |
|---|---|
| **Reservation list** | Reservations with incoming order (no., material), production order (program no., material, position state) or batch plan (plan mode) |
| **View order** | Opens the order behind the selected reservation |
| **OK** / **Cancel** | Close the dialog |

## Working with reservations (step by step)

1. The dialog is opened for a module/component — the open reservations of this
   component are listed.
2. Select a reservation.
3. Click **View order** to open the order behind it:
   * reservations on an **incoming order position** open the incoming delivery
     note dialog,
   * reservations on a **production order position** or **batch plan** open the
     production order dialog.
4. Click **OK** or **Cancel** to close the dialog.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server.
The class-level `<summary>` of `BSOComponentReservation` contains the complete
usage recipe: the dialog is opened via `ShowReservationDialog(component)`; the
open reservations are in `FacilityReservationList`, the selection in
`SelectedFacilityReservation`; `ShowOrder()` navigates to the order dialog
(name resolvable via `GetBSONameForShowOrder()`); `DialogOK()`/`DialogCancel()`
close the dialog (result in `DialogResult`). This BSO is read-only.
