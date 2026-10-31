# Sample Weighing (Weight Check)

> Business object: `BSOSampleWeighing` (Assembly `gip.bso.manufacturing`, Work center child app)

## Definition

The **Sample Weighing** app is used at a production work center to check the
weight of a sample against a target with tolerances (weight check). It is not
a standalone app: it is hosted by the **Work Center Selector** and activated
when the operator selects a process module with a *sample weighing* function.

## Concepts

| Term | Meaning |
|---|---|
| **Sample weighing** | Checking a sample weight against a setpoint with +/- tolerance |
| **Target weight** | Setpoint from the current method of the process function |
| **Tolerance + / −** | Allowed deviation above/below the setpoint |
| **In tolerance** | The sample weight is within the tolerance — the sample can be registered |
| **Acknowledge in tolerance** | Function setting: the sample may only be registered when the weight is in tolerance |

## Screen layout

| Element | Content |
|---|---|
| **Gross weight** | Current weight of the scale |
| **Target weight** | Setpoint of the sample |
| **Register sample weight** | Button to register the sample |
| **Info** | Prompt, e.g. *"Please, take a sample weighing now!"* |

## Taking a sample (step by step)

1. Select the process module in the **Work Center Selector** — the app
   connects to its sample weighing function and the scales.
2. Take a sample and put it on the scale. The **gross weight** is displayed and
   compared with the **target weight** and tolerances (color of the display).
3. Click **Register sample weight** to register the sample at the process
   function.
   * If the function requires acknowledgement in tolerance, the button is only
     active when the weight is **in tolerance**.
4. When the function is reset, the target weight is cleared for the next
   sample.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server.
The class-level `<summary>` of `BSOSampleWeighing` contains the complete usage
recipe: the app is activated by the Work Center Selector via
`Activate(selectedProcessModule)`; target and tolerances come from the current
method of the process function (`TargetWeight`, `TolerancePlus`,
`ToleranceMinus`), the weight is in `ScaleActualWeight` and the tolerance
state in `ScaleBckgrState`. `RegisterSampleWeight()` registers the sample
(enabled only in function state `SMRunning`, and — with `AckInTol` — only in
tolerance).
