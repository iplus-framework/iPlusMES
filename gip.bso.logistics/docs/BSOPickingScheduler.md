# Picking Scheduler (Kommission Zeitplaner)

> Business object: `BSOPickingScheduler` (Assembly `gip.bso.logistics`, Menu: **Logistics → Picking Scheduler**)

## Definition

The **Picking Scheduler** plans pickings (commissioning orders) over time on
**scheduling lines** (scheduling groups). It is the picking counterpart of the
batch plan scheduler: pickings are listed per line, can be re-ordered, moved to
another line and scheduled automatically forwards or backwards.

## Concepts

| Term | Meaning |
|---|---|
| **Line / Scheduling group** | A resource (line) on which pickings are executed; pickings are planned per line |
| **Picking** | A commissioning order with positions to be picked |
| **Scheduled Order** | The sequence position of a picking on its line |
| **Backward scheduling** | Calculates the latest start times from a planned **end date** |
| **Forward scheduling** | Calculates the earliest end times from a planned **start date** |
| **Readiness (Ready to start)** | State that releases a picking for execution |
| **Route check** | Validates the calculated routes of the orders (asynchronous, via message system) |

## Screen layout

The app is a line selector on the left and the plan on the right. The detail
area has these tabs:

| Tab | Content |
|---|---|
| **Picking orders** (Pickings) | Legacy order list (order filters, add/remove orders) — most of its buttons are no longer active in code |
| **Scheduling** (PickingScheduler) | The picking plan of the selected line: filter bar (From/To, picking no., material no.), the picking list (sequence, picking no., state, scheduled/calculated start & end) and the command bar |
| **Timeline** | Timeline presentation of the pickings (start/end, duration, quantity) |

Command bar buttons on the **Scheduling** tab:

| Button | Action |
|---|---|
| **Search** | Loads the pickings of the selected line for the filter period (max. 10 days) |
| **Backward scheduling** | Opens the backward scheduling dialog |
| **Forward scheduling** | Opens the forward scheduling dialog |
| **Switch to Readiness** | Sets the selected pickings to *ready to start* |
| **Route check over orders** | Starts the asynchronous route check |

Explorer toolbar buttons: **Change Mode** (scheduler mode), **Move to other line**,
**Calculate All**.

## Planning pickings (step by step)

1. Select the **Line** in the left tree.
2. Set the period **From** / **To** (maximum 10 days) and optionally the
   **Picking No.** / **Material** filters.
3. Click **Search** — the pickings of the line appear in the plan list.
4. Change the sequence with **Up** / **Down** or by drag & drop
   (**Scheduled Order** is renumbered automatically).
5. Click **Save** (or **Undo** to revert).

## Moving pickings to another line

1. Select one or more pickings in the plan list (checkbox).
2. In the **Move to other line** list, select the target line
   (only lines with matching scheduling groups are offered).
3. Click **Move** — the pickings are re-assigned and the sort order on both
   lines is corrected.

## Scheduling (automatic date calculation)

**Backward scheduling** (from a planned end):

1. Click **Backward scheduling** — the dialog opens.
2. Enter the **Planned End Date**.
3. Click **OK** — the calculation runs in the background; the calculated
   start/end dates are written to the pickings.

**Forward scheduling** (from a planned start): click **Forward scheduling**,
enter the **Planned Start Date** and confirm with **OK**.

**Cancel** closes the dialog and clears the dates.
**Calculate All** recalculates the schedule for all lines.

## Readiness and navigation

* **Switch to Readiness** — selected pickings (state up to *in process*) become
  *ready to start*. **Reset Readiness** sets them back to *in process*.
* **Show Picking Order** (double-click / context menu) opens the picking order
  of the selected picking.
* **Show Visitor voucher** opens the visitor voucher assigned to the selected
  picking (see [BSOVisitorVoucher](BSOVisitorVoucher.md)).

## Route check

Click **Route check over orders** to start the asynchronous route calculation
over the orders. The result is shown in the **Calculated Route** dialog; while
the calculation is running a message asks you to wait and try again.

## Field reference (plan list)

| Field | Description |
|---|---|
| Sequence / Scheduled Order | Position of the picking on the line |
| Picking No. | Number of the picking |
| State | Picking state (created, ready to start, in process, …) |
| Scheduled Start / End | Manually or automatically planned dates |
| Calculated Start / End | Dates calculated by the scheduler |
| Delivery Date From | Requested delivery date |
| Selected | Checkbox for multi-selection (Move, Readiness) |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_type_infos`, `get_method_info`, `execute_acurl_command`). The class-level
`<summary>` of `BSOPickingScheduler` contains the complete usage recipe
(select line → `Search()` → edit `PickingList` → `Save()`), including the C#
method names (`MoveToOtherLine`, `BackwardScheduling`/`BackwardSchedulingOk`,
`ForwardScheduling`/`ForwardSchedulingOk`, `SchedulingCalculateAll`,
`SetBatchStateReadyToStart`, `SetBatchStateCreated`, `NavigateToPicking`,
`NavigateToVoucher`, `ShowPickingsOnTimeline`, `RunPossibleRoutesCheck`, ...).
The button captions above are the translations of these methods
(`ACMethod*` attributes, stored in `ACClassText`).
