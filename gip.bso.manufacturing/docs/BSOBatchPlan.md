# Batch Planning

> Business object: `BSOBatchPlan` (Assembly `gip.bso.manufacturing`; sub-BSO embedded in the production order dialog, the workflow method presenter and the batch plan scheduler)

## Definition

The **Batch Planning** sub-BSO plans the **batches** in which an **intermediate product**
of a production order is produced. For the intermediate of the current workflow step it
manages the batch plan entries (number of batches, batch size, batch numbers, schedule)
and starts the batch execution.

It is **not a standalone app**: it is embedded in the production order dialog
(*Batch planning* area), in the workflow method presenter or in the batch plan scheduler
and takes over the selected production order partslist from its parent.

## Concepts

| Term | Meaning |
|---|---|
| **Intermediate** | Position of the production order whose material is produced in this workflow step |
| **Batch plan entry** | One planned batch (or batch range) of the intermediate, with plan mode, quantities and schedule |
| **Plan mode** | How the batch is planned: **From/To** (batch numbers), **Batch count** (number of batches) or **Total size** (total quantity) |
| **Production line** | Scheduling group (plant/line) the workflow step is assigned to |
| **Target** | The process module (e.g. reactor) with its facility reservation that executes the batch |
| **Route** | Path of storages/modules the material takes; can be defined, displayed and checked |
| **Preferred parameters** | Workflow parameters required on production order level before starting |

## Screen layout

The layout consists of two areas:

* **Batch plan list** (docked left): production line combo, list of the batch plan entries
  (plan state, target/actual batch count, batch size, total size, unit, sequence, planned
  start) and the buttons **New Batchplan entry** / **Delete Batchplan entry**.
* **Batch plan form** (tab): details of the selected entry — plan mode, batch numbers
  (From/To), target batch count, batch size, total size, yield fields, schedule
  (planned/calculated start and end), the intermediate's target/actual/difference quantity,
  the planned totals (**Planned total**, **Missing quantity of plan**, **Missing target
  batch count**, **Missing actual batch count**), the target with its facility stock and
  reservation state, route fields and the buttons **Start Batch**, **Calculate production
  time**, **Set route**, **Open route**, **Route check over orders**.

## Step-by-step tasks

### Planning batches for an intermediate

1. Open the production order and select the partslist/workflow step — the batch plan list
   shows the existing batch plan entries of the intermediate.
2. Click **New Batchplan entry** — a new entry is created; the plan mode configured at the
   BOM is taken over.
3. Fill the fields depending on the plan mode:
   * **From/To**: enter the first and last batch number.
   * **Batch count**: enter the number of batches (and the batch size).
   * **Total size**: enter the total quantity.
4. Check the totals: **Planned total**, **Missing quantity of plan**, **Missing target
   batch count** and **Missing actual batch count** show what is still missing to reach
   the target quantity of the intermediate.
5. Click **Calculate production time** to fill the calculated start/end of the schedule
   from the production duration of the workflow method.
6. Save the production order (the batch plan is part of it).

**Delete Batchplan entry** removes the selected entry.

### Starting a batch

1. Select the batch plan entry and click **Start Batch**.
2. The order is validated first; warnings must be confirmed. If the workflow requires
   parameters on production order level and they are not defined, the preferred parameters
   dialog opens — define them or cancel.
3. If several application managers exist, select one in the dialog and confirm with **Ok**.
4. The order state is activated into production and the batch is started.

### Routes

* **Set route** — opens the route selector with the possible source modules for the
  selected target; the chosen route is assigned.
* **Open route** — displays the current route of the selected target.
* **Route check over orders** — checks in the background whether the route of this batch
  conflicts with reservations of other orders and pickings; the result is shown in an
  info dialog.

## Field reference (batch plan entry, excerpt)

| Field | Description |
|---|---|
| Plan state | Created, released, in production, ... |
| Plan mode | From/To, Batch count or Total size |
| Batch no. from / to | First/last batch number (plan mode From/To) |
| Target batch count | Planned number of batches |
| Actual batch count | Batches executed so far |
| Batch size | Quantity per batch |
| Total size | Total planned quantity of the entry |
| Yield % / Yield batch size / Yield total size | Yield expectations of the plan |
| Scheduled start / end | Manually scheduled times |
| Calculated start / end | Times calculated from the production duration |
| Sequence | Order of the entry within the intermediate |
| Scheduled order | Sort order for the scheduler |
| Connected orders | Other orders which reserve the same module |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOBatchPlan` contains the
complete usage recipe. Relevant methods: `LoadBatchPlan`, `NewBatchPlanForIntermediate`,
`DeleteBatchPlanForIntermediate`, `GetCalculatedDates`, `StartBatchPlan`,
`StartBatchPlanValidation`, `ValidatePreferredParams`, `SetRoute`, `OpenRoute`,
`RunPossibleRoutesCheck`. The current entries are `SelectedIntermediate`,
`SelectedBatchPlanForIntermediate` (list: `BatchPlanForIntermediateList`) and
`SelectedTarget` (list: `TargetsList`). This sub-BSO has no own `Search`/`Save` — it works
on the production order partslist of its parent BSO and saves via `ACSaveChanges()` resp.
the parent `BSOProdOrder`.
