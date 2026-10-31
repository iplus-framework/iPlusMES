# Bill of Materials (BOM / Partslist)

> Business object: `BSOPartslist` (Assembly `gip.bso.masterdata`, Menu: **Master Data → Bill of Materials**)

## Definition

A **BOM** (Bill of Materials) is a structured arrangement of parts or assemblies needed to
produce a parent material. In process industries the BOM is also known as the **formula**,
**recipe**, or **ingredients list**.

Technically, a BOM consists of a list of materials (**Input**) with quantities that are used
to produce a new material (**Output**).

* A BOM in Variobatch represents only **one production stage** at a time. If several
  production stages are necessary, create several BOMs: the result material of the
  subordinate BOM appears as a component in the parent BOM.
* BOMs can be **versioned**: several BOMs for the same material, tagged with a unique
  version number. A versioned BOM can reference another versioned BOM instead of a
  single material (BOM reference in the material list).
* A BOM requires a **Material Workflow** (defines *how* materials are mixed).
* Each component can have **alternative components** (substitutes if the original
  material is unavailable).

## Concepts

| Term | Meaning |
|---|---|
| **BOM / Partslist** | The BOM header: output material, target quantity, validity, version |
| **Component** | A component line: input material + quantity |
| **Material Workflow** | Defines how materials are mixed (must be assigned to the BOM) |
| **Intermediate** | Intermediate product from the Material Workflow; becomes a component of the BOM |
| **Input** | Assignment of a component to an intermediate, with sequence and quantity — defines *at which process step* the material is used |
| **Process Workflow** | Defines how the production process is controlled |
| **Alternative** | Substitute component for a component line |

## Screen layout

The BOM app is a search list on the left and a detail form on the right. The detail form
has these tabs:

| Tab | Content |
|---|---|
| **BOM** (first tab) | Header data: output material, target quantity, validity, active flag, production details, comment |
| **Components** | Component list (sequence, material, quantity, unit) with the detail form of the selected component, including its alternatives |
| **Intermediate** | Intermediates of the Material Workflow and their assigned inputs |
| **Workflow** | Process Workflow selection and workflow parameter transfer |
| **Details** | Free-text description |
| **BOM** (tree) | BOM explosion: tree of subordinate BOMs with their components |

## Creating a new BOM (step by step)

1. Click **New** in the toolbar. A new, empty BOM is created and opened.
2. Enter the **Name** of the bill of material.
3. Assign the **Material** that will be produced with this BOM.
4. Enter the default production quantity (**Target Quantity**) with its unit.
5. *(Optional)* Set the validity period (**Valid from** / **Valid to**) and the
   **Enabled** flag.
6. Click **Save**. If a warning message appears, confirm with **Yes**.

## Assigning the Material Workflow

1. On the first tab, select the desired workflow in the **Select Workflow** list.
2. Click **Assign Materialworkflow** — this assigns the Material Workflow to the BOM
   and loads its **intermediates** into the *Intermediate* tab.
   (**Remove Materialworkflow** unassigns it again.)
3. After changing the Material Workflow you can synchronize component data with
   **Update from Materialworkflow** / **Update all from Materialworkflow**.
4. Click **Save**.

## Adding components (materials)

On the **Components** tab:

1. Click **New Component** — a new line is added to the component list.
2. Assign the **Material** on the selected line.
3. Enter the required quantity (**Target Quantity**) and unit.
4. Click **Save** (confirm warnings with **Yes**).
5. Repeat for every material. **Delete Component** removes the selected line.

### Alternative components

In the component detail form (lower section **Alternative**):

1. Click **New alternative component**.
2. Assign the substitute material and quantity.
3. Click **Save**. **Delete alternative component** removes it.

## Linking components to process steps (Intermediates)

The intermediates from the Material Workflow are connected with the nodes of the
Process Workflow. By assigning inputs to intermediates you tell the system *on which
step of the production process* a material is used. One material can be used in
multiple intermediates.

On the **Intermediate** tab:

1. Select the intermediate product in the upper list.
2. In the lower list (**Inputs**), click **New Input**.
3. Select the component from the component list and assign it as the
   **source** of the new input.
4. Set the **Sequence** (position in the process step) and the
   **Target Quantity** (quantity used in this step).
5. Click **Save** (confirm warnings with **Yes**).
6. **Delete Input** removes an assignment; **Recalculate Totals** recalculates
   the summed quantities of the intermediate.

## Process Workflow (Workflow tab)

1. Select the Process Workflow in the combo box at the top and click **Add**
   (**Remove** deletes it again).
2. **Copy WF-Parameter from:** — select an existing BOM and click **Next** to
   transfer its workflow parameter configuration.
3. **Show parameters** opens the parameter dialog; **Init machine parameters**
   initializes the standard machine parameters.
4. Click **Save**.

## Versioning, Delete, Undo

Toolbar functions:

* **New BOM version** — creates a new version of the current BOM; the current data is
  copied to the new version and the view is refreshed.
* **Delete** — deletes the current BOM (soft delete where supported).
* **Restore** / **Undo** — revert unsaved or deleted state.
* **Load** — re-reads the current BOM if another user changed it in the background.

## BOM explosion (tree tab)

The **BOM** tree tab shows the hierarchy of subordinate BOMs (left) and the components
of the selected node (right). With the **Show intermediates** checkbox the intermediate
levels are included in the explosion. Use **Check Routes** (Workflow tab) to validate
the routing of the BOM.

## Field reference (header)

| Field | Description |
|---|---|
| Name | Unique name of the BOM |
| Material | Output material produced by this BOM |
| Target Quantity | Default production quantity (with unit) |
| Valid from / Valid to | Optional validity period |
| Enabled | Active flag — disabled BOMs are not proposed for production orders |
| Is Default | Marks the default BOM of the material |
| Production Units / Production Weight | Production details |
| Comment | Free comment |
| Details tab | Free-text description |
| Version no. | Version identifier (see *New BOM version*) |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOPartslist` contains the
complete usage recipe (search → navigate → edit → save) including the C# method names
(`New`, `Save`, `NewPartslistPos`, `SetMaterialWF`, `NewIntermediateParts`,
`AddProcessWorkflow`, ...), so an agent can operate the BOM app remotely, e.g. for
user training. The button captions above are the translations of these methods
(`ACMethod*` attributes, stored in `ACClassText`).
