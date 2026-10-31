# Manual Weighing (Handverwiegung)

> Business object: `BSOManualWeighing` (Assembly `gip.bso.manufacturing`)
> This app is **not opened from the menu** — it is a *work-center child app* that is hosted
> inside the **Work center** screen (`BSOWorkCenterSelector`). It becomes active as soon as
> the operator selects the weighing process module of the work center.

## Definition

**Manual Weighing** is the operator screen for manually weighing batch components at a
production work center (e.g. manual addition of sacks, drums or hand-dosed materials).

The app connects to:

* the **process module** of the work center and its **weighing function**
  (`PAFManualWeighing`) — this provides the scales and the current target data,
* the **workflow node** of the running batch (`PWManualWeighing`) — this provides the
  components to be weighed, their states and the booking logic,
* the **warehouse** — the quants/lots that may be used for the current component.

All weighings are booked directly on the batch (production order partslist positions or
picking positions). There is no separate "save" — every completed weighing is booked
immediately.

## Concepts

| Term | Meaning |
|---|---|
| **Work center selector** | The hosting app (`BSOWorkCenterSelector`). Selecting a process module here activates this weighing app. |
| **Process module** | The technical module of the work center (scales, PLC function). Its *Order info* tells the app which batch is running. |
| **Weighing function** | Child component `PAFManualWeighing` of the process module; provides scales, target quantity, tolerances and material. |
| **Workflow node** | The `PWManualWeighing` node of the running batch workflow; executes start/complete/abort/lot change of a weighing. |
| **Component (material)** | One line of the batch to be weighed (material, target quantity, actual quantity, state). |
| **Quant / Lot** | A stock unit (facility charge) of the component material; selecting it assigns the lot to the weighing. |
| **Net / Gross** | Net = weighed content, Gross = raw scale value including container. |
| **Target / Difference** | Target quantity of the component and difference actual − target. |
| **Tolerance state** | The scale display changes color according to target and tolerances; near the target it switches to precision (fine) mode. |
| **Diff weighing** | Mode where only the *difference* is weighed/booked (button **Weigh on start** / **Weigh on end**). |
| **Lot change** | Replacing the quant during an ongoing weighing (e.g. sack is empty). |
| **Bin change** | Changing the container (bin) on the scale during the batch. |
| **Interdischarging** | Emptying the container during weighing and weighing the remaining amount afterwards. |
| **Emptying mode** | Work-center mode to drain remaining material when the batch is finished/aborted. |
| **Single dosing** | Dosing of a material *outside* of a batch order into a target storage. |
| **Rework** | Adding rework material to the batch. |
| **Last used lots** | Suggestion list of recently used lots per material and work center (settings dialog). |

## Screen layout

The screen is a panel layout (no classic tabs). The main layout embeds the
**Manual weighing** panel, which consists of these parts:

| Part | Content |
|---|---|
| **Commands** | Buttons: **Tare**, **Lot change**, **Bin change**, **Abort**, **Acknowledge** |
| **Materials** | List of the batch components (sequence, material no./name, target/actual quantity) with **Refresh**, **Restart weighing** and **Print last quant** |
| **Lots** | Lot selection panel: charge number input, **Apply charge/lot**, quant list with **Quants** details and the **All quants** switch; hint text *"Select a lot from the warehouse."* |
| **Scale** | Scale selection, weight display (**Net**, **Gross**, **Target**, **Difference**), container size, quantity input with **+ 1 kg** / **− 1 kg** |
| **Messages** | Message list of the work center with the weighing buttons **Weigh on start** / **Weigh on end**, **Single dosing** and **Rework** |

When the work center screen embeds the *Manual operations* layout, the app appears in the
tab **Manual weighing** (next to **Manual addition**).

### Dialogs

| Dialog | Purpose |
|---|---|
| **Abort dialog** | Options for aborting: **Yes ->** (abort component), **No ->**, **Abort and scale other comp.**, **Abort and emptying mode**, **Activate emptying mode**, **Interdischarge**, **Interdischarging complete**, **Close** |
| **Acknowledge dialog** | Lists pending messages with **Acknowledge**, **Yes ->**, **No ->** |
| **Settings** | Last used lot suggestions with **Remove last used lot suggestion** |
| **Quant details** | Details of a quant (facility, charge no., external lot no., split no., stock quantity, expiration date, comment) with **Quant not available** |
| **Single dosing** | Material list, **Target weight**, **Number of repetitions**, **Dosing start** |
| **Select target** | Target storage selection with **Select** |
| **Rework** | Rework quantities per batch/order with **Add rework material** |

## Step-by-step tasks

### Weighing a component

1. Select the work center and the weighing process module in the **Work center** screen —
   the Manual Weighing app activates automatically and shows the components of the
   running batch.
2. Select the **component** in the *Materials* list (usually preselected by the workflow).
3. Select the **lot** in the *Lots* list.
   * If the workflow allows manual lot entry, type the charge number and press
     **Apply charge/lot**.
   * With **All quants** switched on, all quants of the material are offered instead of
     only the reserved ones.
4. The weighing starts (or press **Weigh on start** / **Weigh on end** in diff weighing
   mode).
5. Watch the scale display: **Net**, **Target** and **Difference**; the display switches to
   precision mode near the target.
6. If needed: **Tare** the scale, or correct manually added material with **+ 1 kg** /
   **− 1 kg**.
7. When the weight is in tolerance, press **Acknowledge** — the component is booked and
   the next component is selected. Pending messages must be acknowledged first
   (**Acknowledge** / **Yes ->** / **No ->** in the acknowledge dialog).

### Changing the lot during weighing

1. Press **Lot change** while the component is in weighing.
2. The app asks whether the current lot is used up (considering the zero-booking
   tolerance) — answer **Yes ->** / **No ->**.
3. Select the new lot; the weighing continues with the new quant.

### Changing the container (bin)

* Press **Bin change** (only available if the workflow node supports bin change).

### Aborting a component

1. Press **Abort**. The abort dialog offers:
   * **Yes ->** — abort the component and continue with the next one.
   * **Abort and scale other comp.** — abort and re-scale the remaining components
     (only offered when configured).
   * **Abort and emptying mode** — abort and switch the work center to emptying mode.
   * **Activate emptying mode** — switch to emptying mode without aborting.
   * **Interdischarge** — empty the container and weigh the remaining amount
     (finish with **Interdischarging complete** after putting the container back).
   * **Close** — close the dialog without aborting.
2. After aborting, the app may ask whether the material should still be weighed in the
   following batches — answer **Yes ->** / **No ->**.

### Working with quants

* **Quants** — open the quant details of the selected component.
* In the quant details: **Quant not available** books the selected quant to zero stock
  ("not available"), e.g. when a quant is actually empty.

### Settings — last used lots

1. Open **Settings** (in the message/command area).
2. The dialog lists the last used lots per material of this work center.
3. Select a row and press **Remove last used lot suggestion** to remove the suggestion.

### Single dosing (dosing without batch)

1. Press **Single dosing**. The app searches possible target storages and lists all
   dosable materials of the module.
2. Select the **material**, enter the **Target weight** and — if needed — the
   **Number of repetitions**.
3. Press **Dosing start**. If several target storages are possible, the
   **Select target** dialog appears — choose the storage and press **Select**.
4. The dosing workflow books the quantity into the target storage.

### Rework

1. Press **Rework** (only available when rework is enabled for the module).
2. The dialog shows the rework quantities per batch and per order.
3. Press **Add rework material** to activate rework for the batch.

### Miscellaneous

* **Refresh** — reload components and lots (e.g. after another user changed data).
* **Restart weighing** — restart the weighing of the selected component.
* **Print last quant** — print the label of the last booked quant again.

## Field reference

| Field | Part | Description |
|---|---|---|
| Sequence | Materials | Position of the component in the batch |
| Material No. / Material | Materials | Component material |
| Target quantity / Actual quantity | Materials | Target and booked quantity of the component |
| Scale | Scale | Selection of the scale (if the module has several) |
| Net | Scale | Net weight (scale + manually added) |
| Gross | Scale | Gross weight (raw scale value) |
| Target | Scale | Target quantity of the current component |
| Difference | Scale | Actual − target |
| Quantity (kg) | Scale | Manually added quantity (with **+ 1 kg** / **− 1 kg**) |
| Container size | Scale | Container size of the selected component |
| Charge: | Lots | Manually entered charge/lot number |
| Facility / Charge No. / Stock quantity / Expiration date | Lots | Columns of the quant list |
| All quants | Lots | Show all quants instead of only reserved lots |
| Messages | Messages | Message list of the work center |
| Target weight | Single dosing | Quantity to dose |
| Number of repetitions | Single dosing | Repeat the dosing n times |
| Destination | Select target | Target storage of the single dosing |
| Rework quantity per batch / per order | Rework | Rework quantities |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOManualWeighing` contains the
complete usage recipe. Key points for agents:

* The app is a **work-center child BSO**: it is activated by `BSOWorkCenterSelector`
  via `Activate(selectedProcessModule)` → `CurrentProcessModule` →
  `ActivateManualWeighingModel()`. It cannot operate without a selected process module.
* State is driven by the workflow: the running `PWManualWeighing` node becomes
  `CurrentComponentPWNode`; its components are in `WeighingMaterialList` /
  `SelectedWeighingMaterial`, its quants in `FacilityChargeList` /
  `SelectedFacilityCharge`.
* Main methods: `StartWeighing(forceSetFC_F)`, `Weigh()` (diff weighing),
  `Acknowledge()`, `Tare()`, `LotChange()`, `ApplyLot()`, `BinChange()`, `AddKg()`,
  `RemoveKg()`, `RestartWeighing()`, `Abort()` (+ dialog methods `AbortComponent`,
  `AbortComponentScaleOther`, `AbortComponentEmptyingMode`, `SwitchEmptyingMode`,
  `Interdischarge`, `CompleteInterdischarging`, `CloseAbortDialog`),
  `RefreshMaterialOrFC_F()`, `ShowQuantDetails()`, `NotAvailableFacilityCharge()`,
  `OpenSettings()`, `RemoveLastUsedLot()`, `ShowSingleDosingDialog()`,
  `SingleDosingStart()`, `SelectTargetStorage()`, `OpenReworkDialog()`,
  `AddReworkMaterial()`, `PrintLastQuant()`.
* The button captions above are the translations of these methods (`ACMethod*`
  attributes, stored in `ACClassText`).
