# Template Schedule (Planning)

> Business object: `BSOTemplateSchedule` (Assembly `gip.bso.manufacturing`, Menu: **Planning → Template Schedule / Vorlageplan**)

## Definition

A **template schedule** is a reusable planning template. It contains **proposals** that
reference production orders. From a template schedule, real **production orders and
batch plans** are generated for a concrete date ("Add to schedule").

Template schedules are `PlanningMR` records flagged as *Template*. The app embeds the
**Batch Plan Scheduler** which shows and edits the batch plans of the selected template.

## Concepts

| Term | Meaning |
|---|---|
| **Template schedule** | Planning template (`PlanningMR` with Template = true) with a number, name, validity range and comment |
| **Proposal** | A proposal line of the template referencing a production order |
| **Batch Plan Termin** | Target date for which the orders are generated |
| **Clone** | A copy of the template schedule with a new number |
| **Outdated partslist** | A production order partslist whose BOM (formula) has changed since the order was scheduled |

## Screen layout

The app consists of the schedule view (main tab), the template list (Explorer, docked
right) and the embedded **Batch Plan Scheduler**:

| Area | Content |
|---|---|
| **Header** | [PlanningMR No.] Planning Name of the current template schedule |
| **PlanningMR expander** | Fields: PlanningMR No., Planning Name, Range From, Range To, Comment; buttons **Add to schedule** and **Update template schedule orders** |
| **Batch Plan Scheduler** | Embedded child app showing the batch plans of the selected template |
| **Explorer** | Template list with columns: PlanningMR No., Planning Name, Range From, Range To, Template, Comment, Update Date, Update Name |
| **Generate dialog** | Batch Plan Termin date picker with **OK** and **Cancel** buttons |

## Step-by-step tasks

### Creating a template schedule

1. Click **New** — a new template schedule with a new number is created and opened.
2. Enter the **Planning Name**, the validity **Range From** / **Range To** and a
   **Comment**.
3. Click **Save**.

### Generating production orders from a template

1. Select the template schedule in the list. The batch plan scheduler shows its plans.
2. Click **Add to schedule** — the generate dialog opens with the **Batch Plan Termin**
   preset to the current date/time.
3. Adjust the termin if needed and click **OK**.
   - If partslists of the referenced production orders have changed since scheduling, a
     confirmation appears first; confirm with **Yes** to update them.
   - The production orders and batch plans are generated in the background; afterwards
     the new schedule is selected automatically.
4. **Cancel** closes the dialog without generating.

### Updating outdated orders

1. Select the template schedule.
2. Click **Update template schedule orders** — if BOMs of the referenced production
   orders have changed, a confirmation appears; confirm with **Yes** to refresh the
   scheduled orders.

### Cloning a template schedule

1. Select the template schedule.
2. Click **Clone** — a copy with a new number is created in the background and selected.

### Deleting a template schedule

1. Select the template schedule.
2. Click **Delete** and confirm the security question with **Yes**. The template schedule
   is deleted **together with its generated production orders, partslists and batch
   plans** (background operation).

## Toolbar functions

* **New** — create a new template schedule.
* **Save** / **Undo** — persist or discard changes of the current template.
* **Delete** — delete the current template with its generated orders (see above).
* **Load** — re-read the selected template if another user changed it in the background.
* **Search** — refresh the template list.

## Field reference (header)

| Field | Description |
|---|---|
| PlanningMR No. | Unique number of the template schedule |
| Planning Name | Name of the template schedule |
| Range From / Range To | Validity range of the template |
| Comment | Free comment |
| Template | Flag marking the record as template (set automatically) |
| Update Date / Update Name | Last change (system information) |
| Batch Plan Termin | Target date for generation (generate dialog) |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOTemplateSchedule` contains
the complete usage recipe (search → `CurrentPlanningMR` → `New()`/`Save()`/`Delete()`,
`GeneratePlan()`/`GeneratePlanOK()`, `TemplateClone()`,
`CheckIsPartslistChanged()`), so an agent can operate the app remotely, e.g. for user
training. The button captions above are the translations of the `ACMethod*` attributes
(stored in `ACClassText`).
