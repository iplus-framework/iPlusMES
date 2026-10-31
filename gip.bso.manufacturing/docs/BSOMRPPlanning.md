# MRP Planning (Material Requirements Planning)

> Business object: `BSOMRPPlanning` (Assembly `gip.bso.manufacturing`, Menu: **Manufacturing → MRP Planning**; class caption: *Template schedules / Vorlagepläne*)

## Definition

The **MRP Planning** app plans the material requirements for a freely selectable planning
horizon. It compares demands (sales orders, consumption forecasts) with supplies (stock,
production and purchase orders), explodes the BOM and creates planning proposals which can
be turned into production orders.

A planning is processed **phase by phase in a wizard**:

| Phase | Meaning |
|---|---|
| **Plan definition** | Name and planning range (from/to) |
| **Material selection** | Which materials with MRP procedure are planned |
| **Consumption based** | Planning based on consumption data / forecasts |
| **Requirement based** | Planning based on requirements (BOM explosion) |
| **Fulfillment in production** | How requirements are covered by existing production orders |
| **Fulfillment in order** | How requirements are covered by existing sales orders |
| **Finished** | Planning completed |

## Concepts

| Term | Meaning |
|---|---|
| **Planning (MRP)** | The planning header: number, name, range, current phase |
| **Planning horizon** | Date range (from/to) the calculation covers |
| **Consumption forecast** | Forecast consumption included in the calculation on request |
| **Planning position** | One material requirement of the planning with its components |
| **Not planned material** | Material with MRP procedure not yet contained in the planning |
| **Fulfillment** | How a requirement is covered: by a sales order position or by a production order |
| **Preview only** | Wizard runs without writing data |

## Screen layout

The app has two layouts:

* **Preview** (start): list of the plannings (sorted by planning number, descending) with
  the toolbar functions **Create new record**, **Delete**, **Load**, **Search**, **Save**,
  **Undo** and the MRP functions **Run MRP Calculation**, **Activate Planning**,
  **Refresh Planning Data**. Filter fields: **Planning Horizon From/To**,
  **Include Consumption Forecast**, **Preview Only**.
* **Wizard** (after **Edit**): phase dialog with **Forward** / **Backward** navigation,
  **Cancel** to leave the wizard, and phase-specific lists (planning positions, not planned
  materials, fulfillment lists with material filter and planning day filter).

## Step-by-step tasks

### Creating a planning

1. Set the **Planning Horizon From** / **Planning Horizon To** and decide whether the
   **Include Consumption Forecast** checkbox is set.
2. Click **Create new record** — a new planning with the next number and the name
   *MRP Planning \<date time\>* is created and saved immediately.
3. Click **Run MRP Calculation** to calculate the requirements for the horizon in the
   background (clears existing planning data, analyzes demands/supplies, explodes the BOM,
   consolidates the requirements and saves the results).

### Working through the wizard

1. Select the planning in the list and click **Edit** — the wizard opens at the current phase.
2. Work through the phases with **Forward**; **Backward** returns to the previous phase
   (or to the phase selected in **Viewed phase**).
3. In the **Material selection** phase: add a material from the *not planned materials*
   list with **Add**, remove a planning position with **Delete**.
4. In the **Fulfillment in production** / **Fulfillment in order** phases: use **Search**
   to reload the fulfillment lists filtered by the selected materials and planning days.
   With **Show Order** resp. **Dialog sales Order** you can open the covering production
   resp. sales order of the selected position.
5. Confirm each phase with **Forward**; the planning is validated first — on validation
   errors the messages are shown instead of advancing.
6. **Cancel** discards the wizard changes and returns to the preview. When the last phase
   is finished, the preview is shown again.
7. With **Preview Only** activated, the phases are only previewed without writing data.

### Activating a planning

1. Select the finished planning and click **Activate Planning**.
2. Confirm the question with **Yes** — production orders are created from the planning
   proposals in the background; a message reports the number of created orders.

### Maintaining plannings

* **Save** persists changes on the current planning, **Undo** discards them.
* **Load** reloads the selected planning if another user changed it in the background.
* **Delete** asks for confirmation and removes the planning with all dependent data.
* **Refresh Planning Data** reloads the planning details.

## Field reference (planning header)

| Field | Description |
|---|---|
| Planning No. | Unique number of the planning |
| Planning name | Default: *MRP Planning \<date time\>* |
| Range from / Range to | Planning horizon (disabled after the plan definition phase) |
| Phase | Current wizard phase of the planning |
| Template | Plannings flagged as template are not listed by the search |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOMRPPlanning` contains the
complete usage recipe. Relevant methods: `Search`, `New`, `Delete`, `Save`, `UndoSave`,
`Load`, `RunMRPCalculation`, `ActivatePlanning`, `RefreshPlanningData`, `Edit`,
`CancelEdit`, `WizardForward`, `WizardBackward`, `AddMaterialToPlanning`,
`RemoveMaterialFromPlanning`, `FilterFullfilmentProdOrderPartslist`,
`FilterFullfilmentInOrder`, `ShowOrderDialog`, `ShowSalesOrderDialog`. The current record is
`CurrentPlanningMR`, the list `PlanningMRList`. Long running operations (MRP calculation,
activation, deletion) run in a background worker.
