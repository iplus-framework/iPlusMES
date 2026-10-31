# Material Workflow (Mixing Recipe)

> Business object: `BSOMaterialWF` (Assembly `gip.bso.masterdata`, Menu: **Master Data → Material Workflow**)

## Definition

A **Material Workflow** defines *how* materials are mixed together in production:
which **intermediate products** are produced from which **input materials**, and in
which sequence (the **mixing level** of each intermediate).

A material workflow is the counterpart to the **Process Workflow** (control recipe):

* The **Process Workflow** describes *how the production process is controlled*
  (its nodes are the process steps).
* The **Material Workflow** describes *which materials are mixed* and connects each
  intermediate product with the process steps in which it is used.

A Material Workflow is assigned to a **Bill of Materials** (BOM) — see
[BSOPartslist.md](BSOPartslist.md). A workflow that is still used by a BOM is
**protected**: materials, mixing parts and deletion are locked until the BOM
assignment is removed.

## Concepts

| Term | Meaning |
|---|---|
| **Material Workflow** | The mixing recipe: intermediate products, input materials and their relations |
| **Intermediate product** | A material flagged as *intermediate* that is produced within the workflow; appears as a node in the workflow designer |
| **Input material** | A material that flows into an intermediate product |
| **Mixing part** | One input relation of an intermediate product: source material + sequence |
| **Mixing level** | Calculated level (1, 2, 3, ...) of an intermediate in the mixing hierarchy |
| **Process Workflow** | Control recipe attached to the material workflow; its nodes are the process steps |
| **Connection** | Link between an intermediate product and a process step — defines *in which step* the material is used |
| **Related BOM** | A bill of material that uses this workflow |

## Screen layout

The app consists of a search list (left) and a detail area (right). The header shows
the **Material Workflow No.** and the **Name** of the current workflow.

The detail area has these tabs:

| Tab | Content |
|---|---|
| **Workflows** | The graphical **workflow designer**: nodes of the material workflow with the mixing hierarchy, and the connections to the process steps |
| **Materials** | List of all intermediate products of the workflow (with mixing level and unit) on the left; the **mixing parts** of the selected material (sequence, source material) on the right |
| **Partslists** | List of the bills of material that use this workflow (number, version, name, material, target quantity, validity, enabled flag) |
| **Process Workflow** (right side) | Combo box with the attached control recipes, **Add workflow** / **Remove workflow** buttons, **Is Default** checkbox, and the graphical process workflow presenter |

## Creating a new Material Workflow (step by step)

1. Click **Create new record** in the toolbar. A new workflow with an automatically
   generated number is created, opened in the designer and saved immediately.
   If saving fails, discard the empty record with **Undo**.
2. Enter the **Name** of the workflow.
3. Click **Save**.

## Adding intermediate products and input materials

On the **Materials** tab:

1. Click **New intermediate product** — a dialog with all available intermediate
   materials opens.
2. Select the desired input material and click **Ok**.
   * If the workflow has no material yet, the chosen material becomes the **root**
     (target) material of the workflow.
   * If the workflow has exactly one root material, the chosen material becomes its
     **source**.
   * Otherwise a new **input relation** from the chosen material to the material
     selected in the list is created.
3. Click **Cancel** to close the dialog without changes.
4. Click **Save**.

The **mixing level** of every intermediate is recalculated and shown in the list.

## Editing mixing parts

On the **Materials** tab, select an intermediate product. The lower form shows its
mixing parts (source material and sequence):

* Click **New part** to add an empty mixing part, then set the **Source Material**
  and the **Sequence**.
* Click **Delete part** to remove the selected mixing part (confirm with **Yes**).
* Click **Save**.

Both buttons are disabled while the workflow is still used by a bill of material.

## Attaching a Process Workflow (control recipe)

In the **Process Workflow** area on the right:

1. Click **Add workflow** — a dialog lists all control recipes not yet attached.
2. Select one and click **Ok** (**Cancel** aborts).
3. Click **Remove workflow** to detach the selected control recipe again. This also
   removes its BOM assignments, batch plan references and material connections.
4. Set **Is Default** to mark the default control recipe of this workflow.
5. Click **Save**.

## Connecting intermediate products with process steps

On the **Workflows** tab (designer) and the **Process Workflow** presenter:

* **Drag & drop** a process step (workflow node) from the process workflow onto an
  intermediate product in the material workflow designer — this creates a
  **connection** ("this material is used in this step"). A material can be connected
  to a step only once.
* Selecting a material highlights its connected process step, and selecting a process
  step selects the connected material.
* Click **Remove WF connection** to delete connections of the selected intermediate:
  answer **Yes** to remove only the connection of the currently displayed process
  step, or **No** to remove all connections of the material.
* Click **Save**.

## Cloning a Material Workflow

1. Select the workflow to copy and click **Clone** in the toolbar.
2. The dialog proposes a new workflow number (existing number suffixed with
   `_001`, `_002`, ...). Adjust it if needed — the number must be a valid unique name.
3. Click **Ok**. The workflow is copied with all mixing relations, control recipes,
   parameter configurations and connections (a progress bar is shown), and the copy
   is selected afterwards.
4. Click **Cancel** to abort.

## Deleting a Material Workflow

1. Select the workflow and click **Delete** in the toolbar.
2. If the workflow is still assigned to bills of material, deletion is **refused**
   with an error message listing the BOMs. Remove the assignment in the BOM app first.
3. If control recipes are attached, confirm with **Yes** to delete them together with
   all their connections.
4. Click **Save** to execute the deletion in the database.

## Working with records

* **Search**: enter a search string and click **Search**; the first result is opened.
* **Load**: re-reads the current workflow if another user changed it in the background.
* **Undo**: reverts all unsaved changes of the current workflow.
* **Show bill of material**: on the **Partslists** tab, select a BOM and open it
  (navigates to the BOM app).

## Field reference

| Field | Description |
|---|---|
| Material Workflow No. | Unique number of the workflow; editable only in designer mode (renaming also updates the designer layout) |
| Name | Name of the workflow |
| Materials list | Intermediate products with material number, name, base unit and mixing level |
| Sequence | Position of a mixing part within the intermediate product |
| Source Material | Input material of a mixing part |
| Is Default | Marks the default control recipe of the workflow |
| Partslists tab | Number, version, name, material, target quantity, comment, enabled flag and validity of the related BOMs |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOMaterialWF` contains the
complete usage recipe (search → navigate → edit → save) including the C# method names
(`New`, `Save`, `Delete`, `Load`, `UndoSave`, `CloneMaterialWF`, `CloneMaterialWFOK`,
`AddMaterialDlg`, `AddMaterialOK`, `NewMaterialWFRelation`,
`DeleteMaterialWFRelation`, `AddProcessWorkflow`, `NewProcessWorkflowOk`,
`RemoveProcessWorkflow`, `RemoveMaterialConnection`, `NavigateToPartslist`, ...), so
an agent can operate the Material Workflow app remotely, e.g. for user training. The
button captions above are the translations of these methods (`ACMethod*` attributes,
stored in `ACClassText`).
