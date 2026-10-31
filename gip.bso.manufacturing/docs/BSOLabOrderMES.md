# Laboratory Order MES

> Business object: `BSOLabOrderMES` (Assembly `gip.bso.manufacturing`, Menu: **Manufacturing → Laboratory Order MES**)

## Definition

The **Laboratory Order MES** app manages laboratory orders (quality tests) in the
manufacturing execution system. It extends the base *Lab Order* app with two
MES-specific features:

* **Export to Excel** of selected laboratory orders
* **Weighing statistics** (sample weighing charts) for positions with a sample
  weighing tag

A laboratory order consists of a header (material, order/lot reference, sample
taking date, test date, state) and positions (test parameters with min/max
limits and the actual test value).

## Concepts

| Term | Meaning |
|---|---|
| **Laboratory order** | Quality test for a material, usually created automatically by a workflow node or a process module |
| **Position** | One test parameter of the order, with limits (min/max) and the actual value |
| **Lab tag** | Measurement tag of the position (e.g. sample weighing tag) |
| **Weighing statistics** | Recorded sample values of a position with setpoint and tolerance limits |

## Screen layout

The app is a search list on the left and a detail form on the right.

| Area | Content |
|---|---|
| **Search list** | Filter (order no., sample taking date from/to, select all), lab order list with order no., material, state, sample taking date; **Export to Excel** button |
| **Order (Details)** | Header data: order no., material, lot, incoming/outgoing order, production order position, template, sample taking date, test date, state |
| **Positions** | Position list and position detail form: line no., lab tag, limits (min, min-min, max, max-max), setpoint, actual value, comment, state |
| **Weighing** | Sample weighing chart of the selected position: recorded values with **Setpoint**, **Min**, **Max** series and tolerance counters |

## Exporting laboratory orders to Excel (step by step)

1. In the search list, tick the orders to export (column **IsSelected**) or use
   the **Select all** checkbox.
2. Click **Export to Excel**. A file dialog opens — choose the target `.xlsx`
   file.
3. The export runs in the background with a progress dialog. Only orders with a
   production order position are exported; errors are shown in the message list.

## Working with laboratory orders

* **Search**: enter a search string and click **Search**; results appear in the
  lab order list.
* **Edit**: change fields on the current order/position, then **Save** before
  navigating to the next record.
* **New**: laboratory orders are usually created automatically. Manually, use
  **New Lab Order**, select the entity type, the template, sample taking and
  test date, then click **Create**.
* **Load**: refreshes the current order if another user changed it in the
  background.

## Weighing statistics

When a position with a **sample weighing tag** is selected, the app
automatically rebuilds the statistics: the recorded sample values are shown as
a chart together with the **Setpoint**, **Min** and **Max** series, and the
counters (valid, invalid, in tolerance, above, below) are displayed. If the
position has limits but no stored tolerances, the tolerances are recalculated
from the limits.

## Field reference (position detail)

| Field | Description |
|---|---|
| Line number | Position number within the order |
| Lab tag | Measurement tag of the test parameter |
| Min / Min-Min / Max / Max-Max | Tolerance limits of the test |
| Setpoint (Reference value) | Target value of the test |
| Actual value | Recorded test value |
| Comment | Free comment |
| State | State of the position |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOLabOrderMES`
contains the complete usage recipe (search → navigate → edit → save, plus
`ExportToExcel()` and the weighing statistics properties `PiStats`,
`SamplePiStatsList`, `SamplePiMaxList`, `SamplePiMinList`,
`SamplePiSetPointList`), so an agent can operate the app remotely, e.g. for
user training.
