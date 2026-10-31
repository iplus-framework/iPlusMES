# Operation Logs

> Business object: `BSOOperationLog` (Assembly `gip.bso.manufacturing`, Menu: **Manufacturing → Operation logs**)

## Definition

The **Operation Logs** app shows the **open** operation logs (scan bookings) of
the process functions and allows closing them. An operation log is created when
a charge is scanned at a process function (*RegisterEntityOnScan*) and closed
when the operation ends (*UnregisterEntityOnScan*). Logs that remain open —
e.g. because the operator forgot to scan out — are listed here.

## Concepts

| Term | Meaning |
|---|---|
| **Process function** | The function of a process module at which the scan happened |
| **Operation log** | One scan booking: charge, material, program, lot, time |
| **Open** | The log was registered but not yet unregistered |
| **Grouping** | Summed view of the open logs by material or by lot |

## Screen layout

The app is a search list on the left and a detail area on the right.

| Area | Content |
|---|---|
| **Process function** | Selection list of the process functions (left) |
| **Operation logs** | List of open logs: time, material no./name, program no., lot, split, stock quantity with unit variants A/B/C |
| **Tabs: All / Material / LOT** | Grouped view: sums per material or per lot with stock quantity and unit variants |
| **Search text** | Filters the log list |
| Buttons | **Close operation**, **Navigate to Quantmanagement** |

## Working with open operation logs (step by step)

1. Select the **process function** in the left list — the open logs of this
   function are loaded.
2. Use the **search text** to filter by material number, material name,
   program number or lot number.
3. Switch between the **All**, **Material** and **LOT** tabs for a summed view
   per material or per lot.
4. Select a log and click **Close operation** to close it (only possible for
   open scan registrations). The list is reloaded afterwards.
5. Click **Navigate to Quantmanagement** to open the quant management dialog
   for the charge of the selected log.

## Field reference

| Field | Description |
|---|---|
| Operation time | Time of the scan registration |
| Material No. / Material Name | Material of the charge |
| Program No. | Production order program number |
| Lot No. / Split No. | Lot and split of the charge |
| Stock quantity | Quantity of the charge with unit variants A/B/C |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server.
The class-level `<summary>` of `BSOOperationLog` contains the complete usage
recipe: select `SelectedProcessFunction` (from `ProcessFunctionsList`) → logs
load into `OperationLogList` → `SelectedOperationLog` is the target of
`CloseSelectedOperationLog()` and `NavigateToQuantManagement()`. Grouping is
via `OpLogGroupItemList`/`SelectedOpLogGroupItem` (tabs All/Material/LOT),
filtering via `SearchText`. There are no New/Save/Delete entity workflows.
