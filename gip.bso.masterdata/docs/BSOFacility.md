# Storage Locations / Facility Master Data (Tree)

> Business object: `BSOFacility` (Assembly `gip.bso.masterdata`, Menu: **Master Data → Storage Location**)

## Definition

The facility master data manages **all storage objects** of the system — storage
locations, storage bins, vehicles and their sub-structures — as a **hierarchy tree**.
Every storage object is a *Facility* record with a number, a name and a **facility type**
(storage location, storage bin, vehicle, …). Child facilities are nested below their
parent, so a storage location contains its storage bins, and a vehicle contains its
chambers.

This app is the **tree view** over the whole facility hierarchy. For a flat list of
storage locations see [BSOFacilityLocation](BSOFacilityLocation.md), for vehicles see
[BSOFacilityVehicle](BSOFacilityVehicle.md).

## Concepts

| Term | Meaning |
|---|---|
| **Facility** | Any storage object (storage location, storage bin, vehicle, chamber, …) |
| **Facility type** | Classification of the facility (storage location, storage bin, vehicle, …); determines the behaviour in warehousing |
| **Facility Material** | Assignment of a material to a facility — defines which materials may be stored/produced at this facility |
| **Process Workflow** | Workflow that can be assigned to a facility (e.g. for controlling/automation of this storage object) |
| **OEE** | Overall Equipment Effectiveness — throughput and availability figures kept per facility material |

## Screen layout

The app consists of a docked **Explorer** window on the left (facility tree with a
search field) and the **Facility** tab with the detail form of the selected node.

**Explorer (left):** search field and the facility tree. Typing in the search field
filters the tree live by **number, name and key of external system** and preselects the
first match.

**Facility tab (detail form of the selected node):**

| Section | Content |
|---|---|
| Header fields | Number, Name, Configuration (AC class), Facility Type, Stack Calculator, Inward Enabled, Outward Enabled, Min./Opt. Stock Quantity, Max. Volume/Weight Capacity, Distance Front/Back, Fill level validation flags, Post remaining quantity to order on emptying, Disabled for mobile application, Skip print question, Leave material occupation, Posting behaviour, Key of ext. system, Classification code, Driver Name, assigned Process Workflow (read-only) |
| **Configuration** (expander) | Locked Facility, Incoming Facility, Outgoing Facility |
| **Reservations** (expander) | Reserved Quantity, Ordered Quantity (read-only), Posting behaviour |
| Buttons | **Assign process workflow**, **Remove process workflow** |

**Workflow assignment dialog:** list/combo of available process workflows with the
buttons **Assign process workflow** (OK) and **Close** (cancel).

## Step-by-step tasks

### Searching and navigating

1. Enter a search word in the **Search** field of the Explorer — the tree is filtered
   immediately and the first match is selected.
2. Click **Search** to reload the complete tree.
3. Select a node in the tree — the detail form shows its data.

### Editing a facility

1. Select the facility in the tree.
2. Change the fields in the detail form (open the **Configuration** and
   **Reservations** expanders if needed).
3. Click **Save**.
4. Click **Undo** to discard unsaved changes, or **Load** to re-read the record if
   another user changed it in the background.

### Creating a new (child) facility

1. Select the parent node in the tree under which the new facility shall be created.
2. Click **Create new record**. A new facility is inserted below the selected node with
   a generated number (`NewFacility1`, `NewFacility2`, …) and the default facility type.
3. Enter the **Number** and **Name** and adjust the facility type and other fields.
4. Click **Save**.

### Deleting a facility

1. Select the facility in the tree.
2. Click **Delete current record** — this is only possible if the facility has **no
   child facilities**.
3. Click **Save** to execute the delete in the database.

### Assigning a Process Workflow

1. Select the facility in the tree.
2. Click **Assign process workflow** — the workflow selection dialog opens.
3. Select the desired workflow and click **Assign process workflow** (OK) in the dialog.
4. Click **Save**.
5. **Remove process workflow** deletes the assignment after a confirmation question
   (**Yes**).

### Facility materials

The materials assigned to the selected facility are managed via the facility material
list of the selected node:

1. Select the facility in the tree.
2. Click **Add** — a new facility material is created for this facility and selected.
3. Click **Delete** to remove the selected facility material from the facility.
4. Click **Save**.

### OEE test data and throughput figures

With a facility material selected, the toolbar of the facility material area offers:

* **Generate OEE test data** — creates test data for the selected facility material.
* **Delete OEE test data** — removes the test data.
* **Recalc average throughput** — recalculates the average throughput.
* **Correct throuhputs and OEE** — recalculates throughputs and OEE values.

## Field reference (detail form)

| Field | Description |
|---|---|
| Number | Unique number of the facility (generated as `NewFacility{n}` until saved) |
| Name | Name of the facility |
| Configuration | AC class (configuration) of the facility |
| Facility Type | Storage location, storage bin, vehicle, … |
| Stack Calculator | Stack calculator class for stock calculation |
| Inward Enabled / Outward Enabled | Goods may be booked in/out of this facility |
| Min. / Opt. Stock Quantity | Minimum and optimal stock for stock warnings |
| Max. Volume Capacity / Max. Weight Capacity | Capacity limits |
| Distance Front / Distance Back | Distances (e.g. for level measurement) |
| Fill level validation flags | Flags for fill level validation |
| Post remaining quantity to order on emptying | Posting behaviour on emptying |
| Disabled for mobile application | Facility is hidden in mobile apps |
| Skip print question | Skips the print question in workflows |
| Leave material occupation | Keeps material occupation on … |
| Posting behaviour | How postings are handled |
| Key of ext. system | Key of the facility in an external system (also searched) |
| Classification code | Classification code |
| Driver Name | Name of the driver (for vehicles) |
| Locked / Incoming / Outgoing Facility | Configuration expander: locked facility and incoming/outgoing facility references |
| Reserved / Ordered Quantity | Reservations expander: system-maintained quantities (read-only) |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacility` contains the
complete usage recipe (search → select tree node → edit `SelectedFacility` → save),
including the C# method names (`Search`, `AddFacility`, `DeleteFacility`, `Save`,
`Load`, `UndoSave`, `AddFacilityMaterial`, `DeleteFacilityMaterial`,
`SetProcessWorkflow`, `SetWFOk`, `RemoveProcessWorkflow`, `GenerateTestOEEData`,
`DeleteTestOEEData`, `RecalcThroughputAverage`, `RecalcThroughputAndOEE`), so an agent
can operate the app remotely, e.g. for user training. The button captions above are the
translations of these methods (`ACMethod*` attributes, stored in `ACClassText`).
