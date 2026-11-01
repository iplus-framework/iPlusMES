# Picking Order — Sales Order Positions (Tab)

> Business object: `BSOPicking` (Assembly `gip.bso.logistics`, Menu: **Logistics → Picking Order**)
> This manual describes the sales order assignment view of the Picking Order app
> (partial class `BSOPicking_OutOrder`).

## Definition

A **sales order position** (Auftragsposition, `OutOrderPos`) is a position of a sales order that
has to be picked for the customer (goods issue). In the Picking Order app, open sales order
positions are assigned to the picking order so that the warehouse knows which ordered quantities
are picked by this picking.

## Concepts

| Term | Meaning |
|---|---|
| **Open Positions** | Sales order positions not yet completely assigned to pickings (no parent position) |
| **Delivery Notes** | Positions of unpicked sales delivery notes (alternative assignment source) |
| **Partial Quantity** | Optional quantity for a partial call-off of the position |
| **Called Up / Remaining Call Quantity** | Ordered vs. still callable quantity of the position |

## Screen layout

The view has two tabs:

| Tab | Content |
|---|---|
| **OpenPositions** | List of open sales order positions: order no., material no./name, state, target quantity, unit, target delivery date, called up/remaining call/actual quantity. Buttons: **Assign**, **Filter** |
| **DeliveryNotes** | Positions of unpicked sales delivery notes. Button: **Assign Sales Delivery Note** |

## Step-by-step tasks

### Assign a sales order position

1. Open the picking order and switch to the sales order positions view, tab **OpenPositions**.
2. Select a position in the list.
3. *(Optional)* Enter a **Partial Quantity** if only part of the position should be called off now.
4. Click **Assign**. The position is assigned to the picking order, the list is refreshed and the
   partial quantity is reset.
5. **Save** the picking order to persist the new picking position.

### Assign from a sales delivery note

1. Switch to the **DeliveryNotes** tab.
2. Select the position of the unpicked sales delivery note.
3. Click **Assign Sales Delivery Note** and **Save** the picking order.

### Filter the list

Click **Filter** to open the query dialog and narrow the open positions (e.g. by material or
delivery date); the list refreshes after applying.

## Field reference

| Field | Description |
|---|---|
| Order No. | Number of the sales order |
| Material No. / Material Name | Material of the position |
| State | Delivery position state (open … completely assigned) |
| Target Quantity / Unit | Ordered quantity with unit |
| Target Delivery Date | Requested delivery date |
| Called Up Quantity / Remaining Call Quantity | Already called off / still callable quantity |
| Actual Quantity | Quantity actually picked |
| Partial Quantity | Input for a partial call-off |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server. The class-level
`<summary>` of `BSOPicking` (and this partial) contains the usage recipe. Relevant members of this
partial: `OutOrderPosList`, `SelectedOutOrderPos`, `CurrentOutOrderPos`, `DNoteOutOrderPosList`,
`SelectedDNoteOutOrderPos`, `CurrentDNoteOutOrderPos`, `AssignOutOrderPos()`,
`AssignDNoteOutOrderPos()`, `FilterDialogOutOrderPos()`, `RefreshOutOrderPosList()`.
