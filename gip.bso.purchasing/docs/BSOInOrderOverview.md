# Purchase Order Overview

> Business object: `BSOInOrderOverview` (Assembly `gip.bso.purchasing`, Menu: **Purchasing → Order Overview**)

## Definition

The **purchase order overview** shows all **purchase order positions** of the system in
a single list. It is a read-oriented overview app: purchase orders are created and
managed in the order app (`BSOInOrder`); here positions can be searched, opened, edited
and saved.

## Concepts

| Term | Meaning |
|---|---|
| **Purchase order (InOrder)** | Order header at a supplier |
| **Order position (InOrderPos)** | One ordered material with target/actual quantity and weight |
| **Overview list** | All order positions matching the current search criteria |

## Screen layout

| Tab / Area | Content |
|---|---|
| **Overview** (tab) | Data grid of all order positions (header caption: **Purchase order positions**) with columns: order no., supplier, sequence, material, material name, target quantity, target weight, actual quantity, actual weight, quantity unit. Delivery date and priority columns are read-only. |

Toolbar functions: **Search**, **Load**, **Save**, **Undo**.

## Working with the overview (step by step)

1. The list is filled automatically when the app starts; click **Search** to refresh it
   with the current search criteria.
2. Select an order position in the list — it becomes the current position; its parent
   order is loaded with it.
3. Click **Load** to re-read the selected position from the database, e.g. if another
   user changed it in the background.
4. Edit the fields of the current position.
5. Click **Save** to store the changes, or **Undo** to discard them.

## Field reference

| Field | Description |
|---|---|
| Order No. | Number of the parent purchase order |
| Supplier | Distributor company of the order |
| Sequence | Position sequence |
| Material / Material name | Ordered material |
| Target Quantity / Target Weight | Ordered quantity and weight |
| Actual Quantity / Actual Weight | Received quantity and weight |
| Quantity Unit | Unit of the quantities |
| Target delivery date / Priority | Read-only in the overview grid |

## For AI agents (MCP)

The class-level `<summary>` of `BSOInOrderOverview` contains the usage recipe
(`Search` → `InOrderPosList` → `NavigateFirst()` → `CurrentInOrderPos` (parent order in
`CurrentInOrder`) → `Save()` / `UndoSave()` / `Load()`). This BSO has **no**
`New()`/`Delete()` methods — create and manage purchase orders via `BSOInOrder`.
