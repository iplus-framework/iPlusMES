# Tasks on Hold (Waiting Loop)

> Business object: `BSOWorkTaskOnHold` (Assembly `gip.bso.manufacturing`,
> child app of **Work center**, Menu caption: **Tasks on hold** / *Warteschleife*)

## Definition

**Tasks on hold** is a function tab of the **Work center** app. It lists all work
tasks of the current work center that are currently **on hold** (waiting loop) and
lets the operator release them again.

A work task is a step of a production order (batch or recipe step) that was
interrupted or is waiting. The list is read from the work task scan component
(`PAFWorkTaskScan`) of the process module and shows for every task the order, the
partslist, the sequence, the planned start date and the affected materials.

## Concepts

| Term | Meaning |
|---|---|
| **Work task on hold** | A workflow step of a production order that is currently waiting |
| **For release** | Flag of a task that is ready to be released/completed |
| **Sort** | Sort order of the list (batch sequence ascending/descending, workflow start date ascending/descending) |
| **Components** | Component list of the batch plan belonging to the selected task |

## Screen layout

The tab shows the **Tasks** list with these columns:

| Column | Content |
|---|---|
| Order | Program number of the production order |
| Partslist No. / Partslist | Number and name of the partslist (recipe) of the step |
| Sequence | Sequence of the step within the partslist |
| Start date | Planned start of the workflow method |
| Intermediate material | Material produced by the step |
| Final partslist No. / Final partslist | Number and name of the final partslist of the order |

Above the list the **Sort** selection determines the ordering. Buttons:
**Start work task**, **Complete work task**, **Show components** and the refresh
button (**Refresh work tasks**).

## Releasing a task (step by step)

1. Select the task in the **Tasks** list.
2. Click **Start work task** — the work center is occupied/released for this task
   so that it can be continued. The button is only enabled for tasks that are **not
   yet marked for release**.
3. For tasks that are **marked for release**, click **Complete work task** to
   complete the release.
4. The list is refreshed automatically after both actions.

## Sorting and refreshing

* Select an entry in **Sort** to reorder the list: by batch sequence (ascending or
  descending) or by workflow start date (ascending or descending).
* Click **Refresh work tasks** (refresh icon) to re-read the tasks from the work
  center at any time.

## Showing the components of a task

1. Select a task that belongs to a batch.
2. Click **Show components** — the components dialog of the batch plan opens.
   The button is only enabled when the task has a batch plan.

## Field reference

| Field | Description |
|---|---|
| Order (POProgramNo) | Program number of the production order |
| Partslist No. / Partslist | Partslist (recipe) of the step |
| Sequence | Step number within the partslist |
| Start date | Planned start date of the workflow method |
| Intermediate material | Material of the step |
| Final partslist No. / Final partslist | Final partslist of the order (final product) |
| Sort | Sort order: batch sequence ↑/↓, workflow start date ↑/↓ |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOWorkTaskOnHold`
contains the usage recipe. Key members:

* `WorkTaskOnHoldList` / `SelectedWorkTaskOnHold` — task list and highlighted entry,
* `WorkTaskSortItems` / `SelectedWorkTaskSort` — sort orders (`WorkTaskOnHoldSortEnum`),
* `RefreshWorkTasks()` — re-reads the tasks from the `PAFWorkTaskScan` component,
* `StartWorkTask()` / `CompleteWorkTask()` — release the selected task via
  `OccupyReleaseProcessModule` (enabled depending on the *For release* flag),
* `ShowComponents()` — opens the components dialog of the batch plan.

The BSO is activated and deactivated by the `BSOWorkCenterSelector`
(`Activate(processModule)` / `DeActivate()`); an agent normally works on it through
the parent work center BSO.
