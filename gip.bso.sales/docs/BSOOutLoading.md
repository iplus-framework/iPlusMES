# Shipment (Outgoing Loading)

> Business object: `BSOOutLoading` (Assembly `gip.bso.sales`, Menu: **Sales → Shipment / Verladung**)

## Definition

The **Shipment** app is intended for the outgoing loading area (dispatch) of bulk goods
and packaged goods. The picking/commissioning of prepared delivery notes
(**DeliveryNote**) is meant to take place here.

> **Status:** this business object is currently a **skeleton**. The search and the
> Save/Undo functions work, but the creation, deletion and loading of delivery notes,
> positions and declarations are **disabled placeholders** (the corresponding buttons
> are greyed out and the entity logic is not yet implemented). The productive
> commissioning of delivery notes is currently handled by other apps.

## Concepts

| Term | Meaning |
|---|---|
| **Delivery note** | The shipping document for the goods leaving the plant |
| **Delivery note position** | One item of the delivery note |
| **Declaration (load list)** | Declaration data attached to a delivery note position |

## Screen layout

No screen designs are stored for this business object in the database. The BSO exposes
the standard toolbar functions and a navigation list of delivery notes
(`OutDeliveryNoteList`) with the selected record (`SelectedOutDeliveryNote` /
`CurrentOutDeliveryNote`).

## Functions

* **Search** — loads the delivery notes into the list.
* **Save** — saves changes on the current delivery note.
* **Undo** — discards unsaved changes.
* **New** — switches to new-record state (creation not yet implemented).
* **Load** — disabled.
* **Delete** — disabled.
* **Load Position / New Position / Delete Position** — disabled placeholders for the
  delivery note positions.
* **Load Declaration / New Declaration / Delete Declaration** — disabled placeholders
  for the declaration data of a position.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server. The
class-level `<summary>` of `BSOOutLoading` contains the usage recipe and notes which
methods are currently disabled placeholders (`Load`, `Delete`, `LoadOutDeliveryNotePos`,
`NewOutDeliveryNotePos`, `DeleteOutDeliveryNotePos`,
`LoadOutDeliveryNotePosLoadlist`, `NewOutDeliveryNotePosLoadlist`,
`DeleteOutDeliveryNotePosLoadlist`). The primary navigation access works on entity
`DeliveryNote` (`AccessPrimary`, `Search`, `Save`, `UndoSave`).
