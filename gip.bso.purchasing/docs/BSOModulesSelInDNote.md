# Machine- and Container Selection (Goods Intake)

> Business object: `BSOModulesSelInDNote` (Assembly `gip.bso.purchasing`, embedded dialog in **Inward Delivery Note** / `BSOInDeliveryNote`)

## Definition

This dialog is used in the **Inward Delivery Note** app to select the **intake module**
(machine, intake station) and the **target storage** (silo, container) for the selected
delivery note position and to **start the goods intake**.

The dialog always follows the position currently selected in the delivery note app:
when another delivery note position is selected, the module and target lists are
refreshed automatically.

## Concepts

| Term | Meaning |
|---|---|
| **Delivery note position** | One position of the inward delivery note, linked to a purchase order position |
| **Intake module** | Machine/station that receives the goods (from the facility reservations of the order position) |
| **Target** | Storage (silo, container) that can be filled by the selected module |
| **Route** | Transport route from the source to the target, defined with the routing service |
| **Load state** | Status of the order position; **Start intake** sets it to *Ready to load* |
| **Intake workflow** | Process workflow that controls the intake; must be selected if several are available |

## Screen layout

| Area | Content |
|---|---|
| **Machine** list (header: *Machine*) | Available intake modules with the facility of the module (material, material no., stock quantity, available space, capacity, inward enabled) and the reservation state |
| **Targets** list (header: *Targets*) | Possible target storages of the selected module with sequence and route name |
| Filters | Checkboxes **Show selected cells**, **Show enabled cells**, **Show same material cells** |
| Buttons | **Start intake**, **Set route**, **View route** |
| **SelectProcessWorkflow** dialog | Selection list of intake workflows with **OK** / **Cancel** |

## Starting a goods intake (step by step)

1. In the delivery note app select the delivery note position to be received.
   The machine list shows the available intake modules.
2. Select the **module** (machine) that will receive the goods. The target list shows
   the possible storages of this module.
3. Select the **target** storage. Use the filter checkboxes to narrow the list
   (e.g. only storages that already contain the same material).
4. *(Optional)* Click **Set route** to define the transport route to the target with
   the route selector. **View route** shows the existing route (read-only).
5. Click **Start intake**. The order position is set to *Ready to load* and saved.
6. If several intake workflows exist for the module, the **SelectProcessWorkflow**
   dialog appears: select the workflow and confirm with **OK** (or **Cancel** to abort).
7. If several app managers are available, select one and confirm with **OK**.
8. The intake program is created and started on the selected app manager.

**Start intake** is disabled if no module or delivery note position is selected, if the
delivery note is already completed, or if the order position is already ready to load.

## For AI agents (MCP)

The class-level `<summary>` of `BSOModulesSelInDNote` contains the usage recipe
(`CurrentDeliveryNotePos` mirrors the parent `BSOInDeliveryNote.SelectedDeliveryNotePos`;
`ModulesList`/`SelectedModule`, `TargetsList`/`SelectedTarget`; `SetRoute()`,
`OpenRoute()`, `StartIntake()` with the `SelectProcessWorkflow` and `SelectAppManager`
dialogs confirmed via `DialogWorkflowOK`/`DialogOK`). This BSO is a child dialog of
`BSOInDeliveryNote` and cannot be used standalone.
