# Batch Scheduler (Batch Plan Scheduler)

> Business object: `BSOBatchPlanScheduler` (Assembly `gip.bso.manufacturing`, Menu: **Batch scheduler** / *Batch Zeitplaner*)

## Definition

The **Batch Scheduler** is the planning workplace for production orders. It shows all
plannable production orders (*ProdOrders* tab) and the already generated **batch plans**
of a production line, and lets the planner create, change, schedule, move and cancel
batch plans.

A **batch plan** (`ProdOrderBatchPlan`) is one planned production batch of a production
order: it defines how many batches of which size are produced on which line
(*scheduling group*), in which sequence, with which planned/scheduled start and end
times and which target silo/route.

The screen is organized around **production lines** (scheduling groups): the left panel
lists all lines the user is allowed to plan; all lists and buttons always refer to the
line currently selected there.

## Concepts

| Term | Meaning |
|---|---|
| **Production line / Scheduling group** | A planning unit; batch plans are always created and scheduled per line |
| **Batch plan** | One planned batch of a production order (count and/or size, sequence, dates, target) |
| **Plan state** | Lifecycle of a batch plan: *Created → Ready to start → (Auto start) → In process → Completed / Paused / Cancelled* |
| **Prod order** | The production order with its order parts (one order can consist of several parts/BOMs) |
| **Batch plan group** | Optional grouping of batch plans (filter and assignment) |
| **Wizard** | Guided dialog for creating or changing batch plans (material → BOM → explosion → line → batch sizes → targets) |
| **Batch size suggestion** | Proposal how the target quantity is split into batches (min./max./standard batch size, production units) |
| **Route check** | Validation of the target silo routes over the orders |

## Screen layout

The app consists of a **line panel** on the left and a work area with the following
tabs (the *Wizard* replaces the work area while it is active):

| Tab | Content |
|---|---|
| **BatchPlanScheduler** | Batch plans of the selected line: toolbar, search filters (from/to date, program no., material no., batch plan group, connected line) and the batch plan list (sequence, program no., material, batch size/count, planned and scheduled dates, plan state, partial counts) |
| **ProdOrders** | Sub-tabs: **ProdOrders** (order list with planned/unplanned quantities), **FinishedBatches** (produced batches with lot, batch times, workflow) and **Search** (order filters: day, from/to, only completed, department, program no., material no., sort order) |
| **MaterialPlan** | Material preparation for the selected line (search of stock material) |
| **Timeline** (below) | Graphical timeline of the batch plans of all lines, refreshed with **Show** |

**Toolbar of the BatchPlanScheduler tab:** New, Backward scheduling, Forward scheduling,
Up, Down, Switch to Readiness, Reset Readiness, Deactivate and remove, Show components,
Preferred parameters, Select all.

**Wizard phases** (titles shown in the wizard header): *Select material* →
*Select bill of material* → *BOM-Expand* → *Select & Plan Linie* →
*Batch size suggestion* → *Batchplan* (and *Delete batch plans* when deleting).

## Creating batch plans for a new order (wizard)

1. Click **New** in the toolbar. The wizard opens with phase *Select material*.
2. Search and select the **material** to be produced, then click **Forward**.
3. In phase *Select bill of material* select the **BOM** (partslist), adjust the
   **target quantity** and unit if necessary, then click **Forward**.
4. In phase *BOM-Expand* the BOM is exploded into sub-BOMs; click **Forward**.
5. In phase *Select & Plan Linie* every partslist of the order is listed as a planning
   item. For each item select the **line** (scheduling group), the **new target
   quantity** and, if required, the **preferred parameters** (button with the
   parameter caption). Click **Plan** on an item to plan it on the selected line.
   **Remove** deletes the batch plans of an item again.
6. In phase *Batch size suggestion* the system proposes a split of the quantity into
   batches. Use **Calculate** to recalculate the suggestion, **Add** to add a manual
   batch line and **Delete** to remove one. Click **Forward**.
7. In phase *Batchplan* assign the **target silo/route** for each batch
   (route selection with *Open route* / *Set route*). Click **Forward** — the batch
   plans are generated and saved.
8. Repeat from step 5 for further planning items, or click **Close** to leave the
   wizard. **Back** returns to the previous phase at any time.

## Creating batch plans for an existing order

On the **ProdOrders** tab:

1. Set the filters on the **Search** sub-tab (e.g. **Day**, from/to, department,
   program no., material no.) and click **Search for orders**. With
   **Only not planned orders** the list can be reduced to unplanned orders.
2. Select one or more orders in the **ProdOrders** list.
3. Click **Add Batchplan** to plan the selected order with the wizard (see above), or
   **Generate batch plans** to generate batch plans for all selected orders
   automatically in the background (confirm the question with **Yes**).
4. **Merge prodorders** merges several selected orders into one.
5. **Deactivate and remove** removes the selected orders (batch plans in state
   *Created* are deleted, the order parts are cancelled or removed depending on
   configuration).
6. **Show production order** navigates to the order; **Select all** selects all rows.

## Working with batch plans (BatchPlanScheduler tab)

1. Select the **line** on the left, set **From**/**To** (and optionally program no.,
   material no., batch plan group or a connected line) and click **Search**.
2. Select one or more batch plans in the list (checkbox column; **Select all**
   selects all).
3. **Change** reopens the wizard for the selected batch plan (only in state *Created*
   and if nothing was produced yet). **Delete** deletes the selected batch plan.
4. **Switch to Readiness** sets the selected plans to *Ready to start*. The system
   checks for each plan: destination (target) assigned, BOM enabled, required
   parameters defined and — depending on configuration — plausibility of the recipe
   (duplicate components, quantity ratios, component sum). Problems are shown as
   messages.
5. **Reset Readiness** sets selected plans back to *Created*.
6. **Deactivate and remove** cancels/removes the selected plans. If the order has
   further deletable plans, the system asks whether all plans of the order should be
   deleted at once (silently, via wizard or only the current line).
7. **Up** / **Down** move the selected plan in the scheduling sequence; the sequence
   can also be changed by drag & drop. The sequence is stored in the *Scheduled
   Order* column.
8. **Show components** opens the component list of the selected plan,
   **Show bill of material** opens the BOM explosion dialog (confirm with **OK**),
   the **preferred parameters** button opens the parameter dialog of the plan and
   **Show production order** navigates to the order.
9. Edit **Scheduled Start Date** / **Scheduled End Date** and — for semi-automatic
   lines — the **Partial Target Count** directly in the toolbar area; changes are
   saved automatically.

## Rescheduling a whole line

* **Backward scheduling** — opens a dialog; enter the date all batch plans should
  **end** at and confirm with **OK**. All plans of the line are rescheduled backwards
  in the background.
* **Forward scheduling** — opens a dialog; enter the date all batch plans should
  **start** at and confirm with **OK**.
* **Cancel** closes the dialog without changes.
* **Calculate All** (line panel) recalculates the durations of all batch plans.

## Moving a batch plan to another line

1. Select the batch plan in the list.
2. In the line panel select the target line in the **Move to other line** list.
3. Click **Move**. Enter the number of batches or the quantity to move (the system
   validates minimum batch size and remaining quantity).
   * If everything is moved, the plan is reassigned to the target line.
   * If a part is moved, the plan is split: the rest stays on the current line, the
     moved part becomes a new batch plan on the target line.

## Timeline

Click **Show** above the timeline to display the batch plans of all lines as
timeline bars (program no., material, quantity, start/end). The list on the left
groups the bars per line.

## Route check

With a batch plan selected, start the **Route check over orders**. The result is
shown in a dialog; the messages are also written to the message list.

## Field reference (batch plan list)

| Field | Description |
|---|---|
| Scheduled Order | Sequence of the batch plan on the line (editable via Up/Down/drag) |
| Program No. | Number of the production order |
| Material No. / Name | Material produced by the batch |
| Batch Size / Batch Target Count | Size and number of batches of the plan |
| Total Size | Total quantity (batch size × batch count) |
| Planned Start Date / Calculated dates | Dates calculated by the system |
| Scheduled Start Date / Scheduled End Date | Dates fixed by the planner |
| Plan State | Lifecycle state of the plan |
| Partial Target Count / Partial Actual Count | Partial production quantity (semi-automatic lines) |
| Batch Plan Group | Grouping of the batch plan |
| Actual Quantity / Batch Actual Count | Already produced quantity/count |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOBatchPlanScheduler`
contains the complete usage recipe (search → select line → edit → save) including the
C# method names (`Search`, `New`, `WizardForward`, `AddBatchPlan`, `GenerateBatchPlans`,
`SetBatchStateReadyToStart`, `BackwardScheduling`, `MoveToOtherLine`, ...), so an agent
can operate the Batch Scheduler remotely, e.g. for user training. The button captions
above are the translations of these methods (`ACMethod*` attributes, stored in
`ACClassText`).
