# Process Control (Live Workflow Monitor)

> Business object: `BSOProcessControlVB` (Assembly `gip.bso.manufacturing`, Menu: **Process Control / Prozesssteuerung**)

## Definition

The **Process Control** app is the live monitor of Variobatch production. It lists all
currently **running workflow tasks** — production orders with their batches as well as
picking workflows — and displays the **workflow** of the selected task in the process
control layout (the same visualization used at the machine terminals).

It extends the base process control (`BSOProcessControl`) with Variobatch-specific
filters, an optimized (simplified) task query and the ability to delete **inactive**
workflows.

## Concepts

| Term | Meaning |
|---|---|
| **Workflow task** | A running instance of a process workflow (production order, batch or picking) |
| **Program No.** | Number of the production order / picking the workflow belongs to |
| **Batch No.** | Batch sequence number within the production order |
| **Occupied modules** | Process modules and scheduling groups currently occupied by the workflow |
| **Inactive workflow** | A workflow task that is no longer running and can be deleted |

## Screen layout

The app consists of a search area (left) and the workflow display (right):

| Area | Content |
|---|---|
| **Search — tab "Application"** | List of application managers (production lines); **Search** button |
| **Search — tab "Programs"** | Program type selection (radio buttons); **Search** button |
| **Filters** | **Auto Refresh** checkbox, **Program No.** and **Material** filter fields |
| **Workflow list** | Columns: Program No., Batch No., Material No., Material Name, Program No. (workflow), Identifier, Insert Date; **Show Workflow** button |
| **Process control** | Header with Program No. / Batch No. / Material / Insert Date of the selected workflow and the workflow visualization |

## Step-by-step tasks

### Searching for running workflows

1. Select the **Application** (production line) or the **Programs** tab and choose a
   program type.
2. *(Optional)* Enter a **Program No.** and/or **Material** filter — the list is
   restricted to workflows whose order number or material matches.
3. Click **Search**. The running workflows appear in the workflow list.
4. *(Optional)* Enable **Auto Refresh** to keep the list up to date automatically.
5. *(Optional)* Enable **Show Occupied Modules** to also see which process modules and
   scheduling groups each workflow occupies.

### Displaying a workflow

1. Select a workflow in the workflow list.
2. Click **Show Workflow** — the workflow of the selected task is displayed in the
   process control area.

### Deleting an inactive workflow

1. Select the inactive workflow in the workflow list.
2. Click **Delete inactive workflow**. The workflow is removed; references from
   production order positions to the deleted workflow are cleared.

## Field reference (workflow list)

| Field | Description |
|---|---|
| Program No. | Number of the production order or picking |
| Batch No. | Batch sequence number (empty for non-batch workflows) |
| Material No. / Material Name | Material of the order position |
| Program No. (workflow) | Program number of the workflow task |
| Identifier | Identifier of the workflow task |
| Insert Date | Start time of the workflow |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOProcessControlVB` contains
the complete usage recipe (filters → `Search()` → `SelectedACTaskVB` → `ShowWorkflow()`,
`DeleteWorkflowVB()`), so an agent can operate the process control remotely, e.g. for
user training. The button captions above are the translations of the `ACMethod*`
attributes (stored in `ACClassText`).
