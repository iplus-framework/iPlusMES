# Scheduling Group

> Business object: `MDBSOSchedulingGroup` (Assembly `gip.bso.masterdata`, Module: **Scheduling**)

## Definition

A **scheduling group** bundles storage facilities that are planned and scheduled
together. It also defines which **workflow nodes** are offered for scheduling
within the group.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Scheduling Group** | The group header: key, name, index, sort index |
| **Assigned workflow node** | A workflow node (from a process workflow) assigned to the group; only these nodes are offered when scheduling within this group |
| **Available workflow nodes** | Workflow nodes not yet assigned to the group, filtered by the configured process workflow class and a search string |
| **Connected facility** | A storage bin (Facility) that belongs to the group, with its own picking type |

## Screen layout

A search list on the left and a detail form on the right. The detail form has
these tabs:

| Tab | Content |
|---|---|
| **Details** | Header fields (key, name, index, sort index) and the workflow node assignment (two lists side by side) |
| **Facilities** | List of connected facilities with their picking type and the assignment buttons |

## Step-by-step tasks

### Creating a new scheduling group

1. Click **New** in the toolbar. A new, empty group is created and opened.
2. Enter the **key**, **name**, **index** and **sort index**.
3. Click **Save**.

### Assigning workflow nodes (Details tab)

The left list shows the **Available workflow nodes**, the right list the
**Assigned workflow nodes**.

1. *(Optional)* Enter a text in the search field above the left list to filter the
   available nodes (by identifier, caption or comment).
2. Select a node in the left list and click **>** — the node is moved to the
   assigned list.
3. To remove an assignment, select it in the right list and click **<**.
4. Click **Save**.

### Connecting facilities (Facilities tab)

1. Click **Add** — a new, empty row is added to the facility list and selected.
2. Select a storage bin in the combo box, or click **Choose facility** and pick
   the storage bin in the dialog.
3. Set the **picking type** for the row.
4. Click **Save**.
5. **Delete** removes the selected row.

### Other toolbar functions

* **Search** — refreshes the list with the current search string.
* **Undo** — discards all unsaved changes.
* **Load** — re-reads the selected group if another user changed it in the background.
* **Delete** — deletes the current group (fails with a message if still referenced).

## Field reference

| Field | Description |
|---|---|
| Key | Key of the scheduling group |
| Name | Name of the scheduling group |
| Index | Scheduling group index |
| Sort index | Sort order |
| Available workflow nodes | Identifier, caption, comment and method of each candidate node |
| Assigned workflow nodes | Assigned nodes with identifier, caption, comment, method and last change (user/date) |
| Facility No. / Facility Name | Columns of the connected-facility list |
| Picking type | Picking type of the connected facility |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `MDBSOSchedulingGroup`
contains the complete usage recipe (search → `MDSchedulingGroupList` →
`NavigateFirst()` → `CurrentMDSchedulingGroup` → edit → `Save()`), including the
C# methods `AddMDSchedulingGroupWF`, `DeleteMDSchedulingGroupWF`, `AddFacility`,
`ShowFacility`, `DeleteFacility`, so an agent can operate the app remotely.
The button captions above are the translations of these methods (`ACMethod*`
attributes, stored in `ACClassText`).
