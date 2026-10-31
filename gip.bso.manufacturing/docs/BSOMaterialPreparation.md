# Material Preparation (Bereitstellung)

> Business object: `BSOMaterialPreparation` (Assembly `gip.bso.manufacturing`, Menu: **Manufacturing → Material Preparation**)

## Definition

The **Material Preparation** app shows which materials have to be supplied to which
storage bins of a production area and lets the user cover the missing quantities by

* **picking orders** (material is taken from a source storage bin),
* **purchase orders** (material is ordered from a supplier), or
* **production orders** (material is produced internally).

The demand list is calculated by a routing run over the storage bins connected to the
production area (source and destination storage bins, reserved and allocated stock
included).

## Concepts

| Term | Meaning |
|---|---|
| **Materials to supply** | The demand list: one line per material with target, available, already planned and missing quantity |
| **Source storage bin** | Storage bin from which the material is taken (e.g. for a picking line) |
| **Destination storage bin** | Storage bin to which the material shall be supplied; has a **picking type** that determines which picking orders are used |
| **Picking order / picking line** | Order to move the material from the source to the destination storage bin |
| **Purchase order / order line** | Order to buy the missing material |
| **Production order** | Order to produce the missing material with the BOM of the material |

## Screen layout

The screen consists of an **explorer** (left) and the **preparation form** (right).
The preparation form has three areas:

| Area | Content |
|---|---|
| **Materials to supply** | Demand list (image, material number, material name, target quantity, available quantity, available quantity at destination, planned quantity, missing quantity) with the **Target store** filter combo box above and the **Calculate demands** button below |
| **Source / Destination storage bins** | Two grids: source storage bins (storage bin, total, free, blocked, newly planned stock) and destination storage bins (storage bin, actual, optimal, ordered, newly planned stock, picking type) |
| **Details** (tabs at the bottom) | **Picking** (TabPicking), **Purchase** (TabPurchase), **Production** (TabProduction) |

### Picking tab

Left: **Picking orders** (picking no., delivery date, picking type) with buttons
**New picking order**, **Delete picking order**, **Show picking order**.
Right: **Picking lines** (material, target/actual quantity, unit, from/to storage bin,
update date) with the **Show of all picking orders** checkbox, the **Picking quantity**
field and the buttons **Take over into picking order** and **Delete picking line**.

### Purchase tab

Left: **Purchase Order** (order no., type, state, order date, delivery dates) with buttons
**New purchase order**, **Delete purchase order**, **Show purchase order**.
Right: **Purchase line** (material, target/actual quantity, unit, update date, delivery
date, line state) with the **Show of all Purchase orders** checkbox, the **Purchase
quantity** field and the buttons **Take over into purchase order** and
**Delete order line**.

### Production tab

**Production order** grid (program no., sequence, BOM no./name/version, material, state,
target/actual quantity, created by/date) with the **Production order size** field and the
buttons **Create production order**, **Show production order**,
**Delete production order**.

## Step-by-step tasks

### Calculating the demand list

1. Click **Calculate demands**. The demand list of all materials to supply is loaded
   (the calculation is performed by the hosting application via routing).
2. *(Optional)* Select a storage bin in the **Target store** filter to show only the
   materials of this destination storage bin ("All storage locations" shows all).

### Supplying by picking order

1. Select the material in the **Materials to supply** list. The source and destination
   storage bins and the existing picking orders are loaded.
2. Select the **destination storage bin**. The picking type and the **Picking quantity**
   are preset (new planned stock quantity of the bin).
3. Select a **source storage bin**.
4. Click **New picking order** — a new picking order is created and selected.
5. Click **Take over into picking order** — the picking quantity is booked as a picking
   line; the planned and missing quantities in the demand list and the storage bin grids
   are updated.
6. Repeat steps 4–5 for further quantities or materials.
7. **Delete picking line** removes the selected picking line, **Delete picking order**
   removes the whole picking order, **Show picking order** opens the picking order dialog.

### Supplying by purchase order

1. Select the material in the demand list and enter the **Purchase quantity**
   (preset with the missing quantity).
2. Click **New purchase order** — the purchase order dialog opens; confirm with **OK**
   to create the order.
3. Select the new purchase order and click **Take over into purchase order** — the
   quantity is added as an order line and the missing quantity is updated.
4. **Delete order line** removes the selected order line, **Delete purchase order**
   deletes the whole order (only while it is in state *Created*),
   **Show purchase order** opens the order dialog.

### Supplying by production order

1. Select the material in the demand list and enter the **Production order size**
   (preset with the missing quantity).
2. Click **Create production order** — a production order is created with the first
   enabled BOM of the material. If the material has no enabled BOM an error message is
   shown.
3. **Show production order** opens the order dialog, **Delete production order** removes
   the order again (only while it is in state *NewCreated*).

## Field reference (demand list)

| Field | Description |
|---|---|
| Material number / Material name | Material to be supplied |
| Target quantity | Quantity that shall be available at the destination |
| Available quantity | Quantity currently available (source side) |
| Available quantity at destination | Quantity currently available at the destination |
| Planned quantity | Quantity already covered by picking lines / orders |
| Missing quantity | Target quantity minus planned quantity |

Source storage bins: storage bin, total, free, blocked, newly planned stock.
Destination storage bins: storage bin, actual, optimal, ordered, newly planned stock,
picking type.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOMaterialPreparation`
contains the complete usage recipe. Important: this BSO has **no** SearchWord/
NavigateFirst pattern — the demand list is calculated by `SearchStockMaterial()`
(raising `OnSearchStockMaterial`) and filled by the host via
`DoSearchStockMaterial()`/`LoadMaterialPreparationResult()`. The button captions above
are the translations of the `ACMethodCommand`/`ACMethodInteraction` attributes
(`NewPicking`, `NewPickingPos`, `NewInOrder`, `NewInOrderPos`,
`GenerateProductionOrder`, `ShowPicking`, `ShowInOrder`, `ShowProductionOrder`, ...).
