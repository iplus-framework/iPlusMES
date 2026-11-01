# Picking Order — Weighings (Tab)

> Business object: `BSOPicking` (Assembly `gip.bso.logistics`, Menu: **Logistics → Picking Order**)
> This manual describes the **Weighings** view of the Picking Order app (partial class `BSOPicking_Weighing`).

## Definition

A **weighing** (Wägung) records a weight determined for a picking position — either automatically
from a connected **vehicle scale** (alibi weight) or entered manually. Weighings are typically used
for vehicle loading/unloading (gross/tare determination).

## Concepts

| Term | Meaning |
|---|---|
| **Vehicle Scale** | A scale instance of the iPlus class *PAEScaleCalibratable* connected to the system |
| **Alibi Weight** | Weight value stored by the scale with a unique identification number |
| **Manual Weighing** | Weight entered by hand, stored as a weighing record |

## Screen layout

The **Weighings** view shows:

| Area | Content |
|---|---|
| Scale selection | Combo box with the available vehicle scales (**Vehicle scale**) |
| Weighing list | Weighings of the selected picking position: weighing no., weight, weighing state, start/end date, identification no., created/updated by |
| Manual weight | Input field **Weight manually** with button **Weight manuell** |

Buttons: **Register Weight**, **Weight manuell**.

## Step-by-step tasks

### Register a weight from the scale

1. Select the picking position whose weight should be recorded.
2. Select the **Vehicle scale** in the combo box.
3. Click **Register Weight**. The alibi weight is read from the scale and stored as a weighing
   record for the position; the weighing list refreshes.
   The button is only active if a scale and a **saved** picking position are selected and the
   scale is connected.

### Enter a weight manually

1. Select the picking position.
2. Enter the weight in the **Weight manually** field.
3. Click **Weight manuell**. A weighing record with this weight is created for the position and
   saved immediately. The input field is reset to zero.

## Field reference

| Field | Description |
|---|---|
| Vehicle scale | Selected scale (instances of *PAEScaleCalibratable*) |
| Weighing No. | Number of the weighing record |
| Weight | Recorded weight value |
| Weighing State | State of the weighing |
| Start Date / End Date | Start and end of the weighing |
| Ident No. | Identification number of the alibi weight (manual weighings: timestamp) |
| Weight manually | Input field for a manual weight |
| Created/Updated by, dates | System information |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server. The class-level
`<summary>` of `BSOPicking` (and this partial) contains the usage recipe. Relevant members of this
partial: `ScalesList`, `SelectedScale`, `WeighingList`, `SelectedWeighing`, `WeighingValue`,
`RegisterWeight()`, `AddManualWeighing()`, `RefreshWeighingList()`.
