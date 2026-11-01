# Destination Planning Picking (Target Selector)

> Business object: `BSOPickingTargetSelector` (Assembly `gip.bso.logistics`,
> embedded sub-BSO of the **Picking** app — not a standalone menu entry)

## Definition

The **destination planning** is the part of the picking app in which the user
decides **where** the picking positions will be stored: it shows the possible
**target storage locations** (storage bins) for the current picking position,
their occupancy and material, and allows to define the **route** to the target
and to **start the picking workflow**.

## Concepts

| Term | Meaning |
|---|---|
| **Target** | A possible target storage location (module) for the current picking position |
| **Reservation** | The reservation of a storage bin for a picking position |
| **Route** | The transport route (conveyor/forerunner chain) from the source to the target |
| **Workflow node** | The step of the picking workflow to which this selector belongs |
| **Application manager** | The runtime instance (app manager) that executes the started picking |

## Screen layout

The selector is embedded in the picking workflow (design *Target selection*):

* **Machine header** — the current workflow step.
* **Targets list** — one row per possible target with:
  module name, material (number/name), current stock quantity, available space,
  maximum weight capacity, inward enabled flag, reservation state, sequence,
  **connected orders** (other pickings reserving the same bin) and the current
  **route name**.
* **Filter checkboxes**: *Show selected targets only*, *Show released targets
  only*, *Show with same Material*.
* Buttons: **Start Workflow**, **Set route**, **Open route**.

## Planning the destination (step by step)

1. Open the destination planning step of a picking in the picking app.
   The target list is loaded automatically for the current picking position.
2. Restrict the list with the filter checkboxes if needed.
3. Select a target row. The columns show stock, free space, capacity and whether
   other picking orders (**Connected orders**) already reserve this bin.
4. *(Optional)* Click **Set route** to define the route to the selected target
   in the route selector dialog. The resulting route is assigned to the target.
5. *(Optional)* Click **Open route** to display the current route of the selected
   target (read only).

## Starting the picking

1. Make sure at least one picking position is **ready to load** and a target is selected.
2. Click **Start Workflow**. The batch plan is validated first:
   * On errors the start is aborted with a message.
   * On warnings you are asked whether to start anyway (**Yes** / **No**).
3. If several application managers are available, a selection dialog appears —
   choose the manager and confirm with **OK** (**Cancel** aborts).
4. The picking is started on the selected application manager.

## Field reference (target row)

| Field | Description |
|---|---|
| Module | Name of the target storage location (module) |
| Material No. / Material name | Material currently in the target bin |
| Stock quantity | Current stock of the target bin |
| Available space | Free space of the target bin |
| Max. weight capacity | Maximum weight capacity of the bin |
| Inward enabled | Goods receipt into this bin is allowed |
| Reservation state | State of the reservation of the selected bin |
| Sequence | Sequence of the reservation |
| Connected orders | Other picking orders reserving the same bin |
| Route | Current route of the target |

## For AI agents (MCP)

This BSO is a **sub-BSO** embedded in the picking workflow — it has no own search
list. The class-level `<summary>` of `BSOPickingTargetSelector` contains the usage
recipe: `CurrentPWInfo`/`CurrentACClassWF` (workflow node), `LoadBatchPlan()`,
`RefreshTargets()`, `TargetsList`/`SelectedTarget` (from base class
`VBBSOModulesSelector`), `StartWorkflow()`, `StartBatchPlanValidation()`,
`SetRoute()`, `OpenRoute()`. The button captions above are the translations of
these methods (`ACMethod*` attributes, stored in `ACClassText`).
