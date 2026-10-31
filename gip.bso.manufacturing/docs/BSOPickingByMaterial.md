# Picking by Material (Kommissionierung nach Material)

> Business object: `BSOPickingByMaterial` (Assembly `gip.bso.manufacturing`,
> operated at a **work center** — child of the Work Center Selector)

## Definition

**Picking by Material** is a weighing station app for picking orders. Instead of working
order by order, all open pickings of a date range are **grouped by material**: the user
weighs the required quantity of a material once, and the posting is distributed to the
matching picking lines.

The app is started from a work center with a process module (manual weighing station).
The pickings to process are determined by the configuration of the workflow node
(picking type, source storage bins, date range).

## Concepts

| Term | Meaning |
|---|---|
| **Weighing material** | One entry per material, built from the picking lines of all pickings in the date range |
| **Picking line** | A line of a picking order for this material; the weighed quantity is posted to it |
| **Storage charge** | The charge (container/lot) from which the material is weighed out |
| **Target weight** | Target quantity of the selected picking line, preset on the scale |
| **Auto print on posting** | Configuration of the workflow node: print a label automatically after each posting |

## Screen layout

Top: **From** / **To** date pickers and the **Run pickings by material** button.
Below, three columns:

| Area | Content |
|---|---|
| **Scales** (left, top) | Scale selection, current material, target weight, gross/net weight, container size, quantity with **Add** / **Remove** buttons |
| **Commands** (left, bottom) | **Tare**, **Acknowledge**, **Bin change**, **Lot change**, **Abort** |
| **Components** (middle, top) | Weighing material list (sequence, material no./name, actual/target quantity) with **Print** and **Refresh** buttons |
| **Lot** (middle, bottom) | Storage charge selection (charge no., storage bin, expiration date, stock, unit) with the **Enter lot manually** option and the **Apply lot** button |
| **Available pickings** (right) | Picking lines of the selected material (picking no., delivery address, target/actual quantity) |

Dialogs: **Abort dialog** (with **Abort picking by material**, **Cancel component**,
**Finish order**), progress bar while finishing.

## Step-by-step tasks

### Starting a picking run

1. Set the **From** and **To** date (preset: tomorrow).
2. Click **Run pickings by material**. The station's destination storage is determined
   by routing, all open pickings of the configured picking type within the date range
   whose source storage bin matches the station are read, and the single dosing workflow
   is started. The component list shows one line per material.

### Weighing a material

1. Select the material in the **Components** list. The lot list and the picking lines of
   this material are loaded.
2. Select the picking line in **Available pickings** — the **target weight** is preset.
3. Select the storage charge in the **Lot** list (or scan the charge — the material and
   charge are selected automatically).
4. Place the container on the scale; use **Tare** to tare the scale and **Add** /
   **Remove** to correct the quantity manually.
5. Click **Acknowledge** — the weighed quantity is booked out of the storage charge and
   the picking line is updated. If the stock of the charge becomes negative, confirm
   whether the charge shall be set to *not available*.
6. With **Auto print on posting** a label is printed automatically; **Print** prints the
   label of the last posting manually.
7. **Cancel component** cancels the weighing of the current component and returns to the
   lot selection.

### Finishing or aborting the run

* **Finish order** — after confirmation ("Are you sure that you want complete weighing
  of all components?") all pickings of the run are completed (progress dialog).
* **Abort** / **Abort picking by material** — aborts the function.
* **Bin change** / **Lot change** — change the destination bin or the lot during weighing.

## Field reference

| Field | Description |
|---|---|
| From / To | Delivery date range of the pickings |
| Current material | Material currently being weighed |
| Target weight | Target quantity of the selected picking line |
| Gross weight / Net weight | Scale values |
| Quantity | Manually added/removed quantity |
| Components: sequence, material no./name, actual/target quantity | Weighing material list |
| Lot: charge no., storage bin, expiration date, stock, unit | Storage charge list |
| Available pickings: picking no., delivery address, target/actual quantity | Picking lines of the material |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOPickingByMaterial`
contains the complete usage recipe. This BSO inherits from `BSOManualWeighing`
(`Activate(processModule)`, `Acknowledge()`, `Tare()`, `AddKg()`, `RemoveKg()`,
`Abort()` are defined there); its own entry points are `RunPickingByMaterial()`,
`FinishPickingOrder()`, `CancelCurrentComponent()` and `AbortPickingByMaterial()`.
The button captions above are the translations of the `ACMethodInfo` attributes.
