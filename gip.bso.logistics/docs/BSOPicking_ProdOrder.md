# Picking Order — Production Order Positions (Tab)

> Business object: `BSOPicking` (Assembly `gip.bso.logistics`, Menu: **Logistics → Picking Order**)
> This manual describes the production order assignment view of the Picking Order app
> (partial class `BSOPicking_ProdOrder`).

## Definition

A **production order position** (Fertigungsauftragsposition, `ProdOrderPartslistPos`) is an
outward position of a production order (created from a BOM). In the Picking Order app, open
production order positions are assigned to the picking order so that the material required by the
production order is picked by this picking.

## Concepts

| Term | Meaning |
|---|---|
| **Open Positions** | Root positions (type *OutwardRoot*) of production orders in state *NewCreated* that have no picking position yet |
| **Partial Quantity** | Optional quantity for a partial call-off of the position |
| **Program No.** | Number of the production order the position belongs to |
| **BOM / Partslist** | Bill of materials the production order was created from |

## Screen layout

The view shows the list of open production order positions: program no., BOM name, sequence,
material no./name, target quantity with unit, actual quantity. Buttons: **Assign**, **Filter**.

## Step-by-step tasks

### Assign a production order position

1. Open the picking order and switch to the production order positions view.
2. Select a position in the list (sorted by program no. and sequence).
3. *(Optional)* Enter a **Partial Quantity** if only part of the position should be called off now.
4. Click **Assign**. The position is assigned to the picking order, the list is refreshed and the
   partial quantity is reset.
5. **Save** the picking order to persist the new picking position.

### Filter the list

Click **Filter** to open the query dialog and narrow the open positions; the list refreshes after
applying.

## Field reference

| Field | Description |
|---|---|
| Program No. | Number of the production order |
| BOM (Partslist) | Bill of materials of the production order |
| Sequence | Position sequence within the production order |
| Material No. / Material Name | Material to be picked |
| Target Quantity / Unit | Required quantity with unit |
| Actual Quantity | Quantity actually picked |
| Partial Quantity | Input for a partial call-off |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server. The class-level
`<summary>` of `BSOPicking` (and this partial) contains the usage recipe. Relevant members of this
partial: `ProdOrderPartslistPosList`, `SelectedProdOrderPartslistPos`,
`CurrentProdOrderPartslistPos`, `AssignProdOrderPartslistPos()`,
`FilterDialogProdOrderPartslistPos()`, `RefreshProdOrderPartslistPosList()`.
