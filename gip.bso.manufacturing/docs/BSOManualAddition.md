# Manual Addition

> Business object: `BSOManualAddition` (Assembly `gip.bso.manufacturing`, Work center child app)

## Definition

The **Manual Addition** app is used at a production work center to add material
**manually** to a running process (e.g. dosing by hand into a container on a
scale). It is not a standalone app: it is hosted by the **Work Center
Selector** and activated when the operator selects a process module with a
*manual addition* function.

## Concepts

| Term | Meaning |
|---|---|
| **Manual addition** | Adding material by hand, weighed on the scale of the module |
| **Component** | Material to be added, from the running manual addition workflow node |
| **Quant / Lot** | The storage quantity (charge) from which the material is taken |
| **Add quantity** | Quantity to be added (suggested automatically or entered) |
| **Only acknowledge** | Function setting: the operator only selects the lot and acknowledges — the quantity is suggested automatically |

## Screen layout

| Area | Content |
|---|---|
| **Components** | Materials to be added (from the running workflow node) |
| **Facility charges** | Available quants/lots of the selected component |
| **Scales** | **Target weight**, **actual weight**, **difference weight**, add quantity with unit, **+ 1 kg** / **− 1 kg** buttons |
| **Commands** | **Acknowledge**, **Lot change**, **Abort** |
| **Messages** | Pending messages with **Weigh** button |

## Adding material manually (step by step)

1. Select the process module in the **Work Center Selector** — the app
   connects to its manual addition function and shows the components.
2. Select the **component** to be added (usually preselected by the workflow).
3. Select the **quant/lot** to take the material from. In *Only acknowledge*
   mode the **add quantity** is suggested automatically (quant stock or
   remaining target quantity) and transferred to the process function.
4. Add the material; the **actual weight** and the **difference weight** to the
   target are updated continuously (color of the display shows the tolerance
   state). Fine corrections with **+ 1 kg** / **− 1 kg**.
5. Click **Acknowledge** to complete the addition when the weight is in
   tolerance (pending messages are acknowledged first).
6. **Lot change** changes the quant during the addition. If the target quantity
   has already been reached, the app asks whether to change the lot without
   posting the current quantity.
7. **Abort** opens the abort dialog (abort component, scale other components,
   emptying mode, ...).

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server.
The class-level `<summary>` of `BSOManualAddition` contains the complete usage
recipe. It extends `BSOManualWeighing` (see its summary for the full weighing
workflow: `SelectedWeighingMaterial`, `SelectedFacilityCharge`,
`StartWeighing`, `Weigh`, `Acknowledge`, `Tare`, `LotChange`, `Abort`, ...).
Manual-addition specifics: the added weight is in `ScaleAddActualWeight`
(exposed as `ScaleActualWeight`), the tolerance state is recalculated on every
weight change, `OnlyAcknowledge` is read from the workflow method
(`OnLoadPWConfiguration`), and selecting a quant in this mode auto-suggests
the add quantity and calls `PAFManualAddition.ChangeManuallyAddedQuantity`.
Only components flagged as manual addition are handled
(`OnHandleWeighingComponentInfo`); the workflow nodes are resolved via
`FindWFNodes`.
