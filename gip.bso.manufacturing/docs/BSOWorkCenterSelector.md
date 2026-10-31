# Work Center (Work Center Selector)

> Business object: `BSOWorkCenterSelector` (Assembly `gip.bso.manufacturing`, Menu: **Manufacturing → Work center**)

## Definition

The **Work Center** app is the operator dashboard of the shop floor. On startup it
searches for all **process modules** (work centers) that have process functions
installed which provide functions for this app (e.g. manual weighing, sample
weighing, manual addition, picking). The operator selects one work center and the
app loads everything that belongs to it:

* the **function tabs** — for every installed process function the configured child
  business objects are started and embedded as a tab (e.g. **Work center messages**,
  **Tasks on hold**, **Manual weighing**, **Manual addition**, **Picking by material**,
  **Bin selection**, **Bin discharging**, **Sample weighing**),
* the **order context** — the batch or picking order that currently occupies the
  work center, with its bill of materials and quantities,
* the **workflow** — the currently executing process workflow,
* the **function monitor** — the process modules currently accessed by the running
  workflow and their alarm commands,
* the **alarms** received by the work center.

Access to work centers can be restricted per user with **work center rules**.

## Concepts

| Term | Meaning |
|---|---|
| **Work center** | A process module (`PAProcessModule`) on which the operator works |
| **Process function** | A function installed under the process module (e.g. weighing); provides the function tabs |
| **Work center item** | One selectable entry in the work center list (process module + its functions) |
| **Child BSO** | A function app embedded as a tab (derived from `BSOWorkCenterChild`), activated per work center |
| **Order context** | Batch or picking order currently occupying the work center (order no., material, batch sequence) |
| **Bill of Materials** | Input components of the current batch/picking with target, actual and difference quantities |
| **Function monitor** | List of process modules currently accessed by the running workflow, with their alarm commands |
| **Extra discharge target** | Storage (silo, parkingspace, intermediate bin) to which the work center can be emptied |
| **Work center rule** | Permission that assigns a work center to a user |
| **Tasks on hold** | Work tasks of the work center that are waiting in the waiting loop (see *Tasks on hold* app) |

## Screen layout

The app consists of a main form and an auto-hide **explorer** window on the left.

**Explorer (left, auto-hide):** list of all available work centers. Each entry shows
the work center name and a badge with the number of **active functions** (red circle).
Entries are highlighted green while an order is running on the work center.

**Main form (top):** header bar with

| Element | Content |
|---|---|
| Module name | Caption of the current work center (process module) |
| **O:** | Order number of the current batch/picking |
| **M:** | Material of the final batch position |
| **B:** | Batch sequence number |
| Clock | Current time (updated every second) |
| Alarm bell | Blinking alarm icon — only visible when the work center has received alarms; opens the alarms dialog |

Below the header the monitored process modules (function monitor visuals) are shown.

**Tabs (main area):**

| Tab | Content |
|---|---|
| *Function tabs* (dynamic) | One tab per installed process function; the tab header shows the function caption and is displayed in red while the function is active |
| **Workflow** | Workflow visualization with the **Show/Refresh workflow** button (refresh icon) |
| **Functions** | Function monitor: modules currently accessed by the workflow and their **Commands** |
| **Bill of Materials** | Sub-tabs **Bill of Materials**, **Description** and **Image** |
| **Visualisation** | Live visualisation of the current process module |

**Bill of Materials tab** — component list of the current batch or picking:

| Column | Content |
|---|---|
| Material No. / Material | Material of the input component |
| Target Quantity | Target quantity with unit |
| Actual Quantity | Actual quantity with unit |
| Difference Quantity | Difference with unit |
| State | Status of the position |

The refresh icon button (**Show/Refresh Bill of Materials**) reloads the list.

**Dialogs:**

* **Configuration** — work center permissions per user
* **Select extra discharging destination** — target selection for emptying
* **Received alarms** — list of received alarms with acknowledge buttons

## Selecting a work center

1. Open the **explorer** on the left edge (auto-hide button).
2. Click the desired work center. The app loads its functions as tabs and the
   current order context into the header.
3. The last selected work center is restored automatically on the next start
   (if enabled in the configuration).

## Working with the current order

As soon as a batch or picking order occupies the work center, the header shows
**O:** (order), **M:** (material) and **B:** (batch sequence). On the
**Bill of Materials** tab the input components with target, actual and difference
quantities are listed. Use the refresh icon (**Show/Refresh Bill of Materials**)
to reload the list. On the **Workflow** tab, **Show/Refresh workflow** loads the
currently executing workflow into the presenter.

## Function monitor (Functions tab)

The **Functions** tab lists all process modules that are currently accessed by the
running workflow. Selecting a module shows its **Commands** — the alarm commands of
the selected function, which can be executed directly.

## Acknowledging alarms

When the work center receives alarms, the blinking alarm bell appears in the header.

1. Click the **alarm bell** — the *Received alarms* dialog opens with the columns
   Caption, Message, Time occurred and Source.
2. Select an alarm and click **Acknowledge alarm**, or click
   **Acknowledge all alarms** to acknowledge everything.
3. **Abort** closes the dialog.

## Switching to emptying mode

1. Start the target search (extra discharge targets are searched via the routing
   service — silos, parkingspaces and intermediate bins reachable from the work center).
2. In the dialog **Select extra discharging destination** select a target.
3. Click **Switch to emptying** to switch the current workflow group to emptying
   mode with this target, or **Abort all and switch to emptying** to abort all
   running programs first.
4. **Abort** closes the dialog without changes.

## Configuring work center permissions

1. Click **Configure work center** — the configuration dialog opens with the
   currently assigned rules.
2. Select a **User** and an entry under **Available work centers**.
3. Click **Grant permission** to assign the work center to the user, or select an
   entry under **Assigned process modules** and click **Remove permission**.
4. **Replace username** opens a dialog to replace a user name in all rules
   (**Replace** confirms); **Copy user rules** copies the rules of one user to
   another (**Copy** confirms).
5. Click **Apply rules and close** to save the rules and close the dialog.
6. Use **Save** / **Undo** in the toolbar to persist or discard pending changes.

## Field reference (header)

| Field | Description |
|---|---|
| Module | Caption of the current work center (process module) |
| O: (Order number) | Program number of the current production order / picking caption |
| M: (Material) | Material number and name of the final batch position |
| B: (Batch) | Batch sequence number of the current batch |
| Clock | Current time |
| Active functions (explorer badge) | Number of currently active functions of the work center |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOWorkCenterSelector`
contains the complete usage recipe. Typical agent sequence:

1. Read `WorkCenterItems`, set `SelectedWorkCenterItem` (or call `SelectWorkCenterItem()`)
   to load a work center — this starts and activates the child BSOs.
2. Read the order context (`CurrentBatch`, `CurrentPicking`, `EndBatchPos`,
   `ProdOrderProgramNo`, `BatchSeqNo`, `EBPMaterialName`, `InputComponentList`,
   `SelectedInputComponent`).
3. `ShowWorkflow()`, `LoadPartslist()`, `ShowReceivedAlarmsDialog()`,
   `AckReceivedAlarm()`, `AckAllReceivedAlarms()`,
   `SelectExtraDisTargetOnPWGroup()` + `SwitchPWGroupToEmptyingMode()` /
   `AbortAllAndSwitchPWGroupToEmptyingMode()`.
4. Permissions: `ConfigureBSO()`, `AddRule()`, `RemoveRule()`, `ApplyRulesAndClose()`,
   `Save()` / `UndoSave()`.

The button captions above are the translations of these methods (`ACMethod*`
attributes, stored in `ACClassText`). The child BSOs
(`BSOWorkCenterMessages`, `BSOWorkTaskOnHold`, `BSOManualWeighing`,
`BSOManualAddition`, `BSOPickingByMaterial`, `BSOBinSelection`,
`BSOBinDischarging`, `BSOSampleWeighing`) have their own metadata and manuals.
