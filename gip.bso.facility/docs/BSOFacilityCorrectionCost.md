# Cost Rate Adjustment (Correction Cost)

> Business object: `BSOFacilityCorrectionCost` (Assembly `gip.bso.facility`, started via the **Businessobject-Manager / Application Object Manager**)

## Definition

The **Cost Rate Adjustment** app corrects the **cost rates** (valuation prices) of
materials and their stock charges. Cost rates determine the value of the stock; this
app revaluates existing charges by posting booking records — either for **all**
charges/materials or for one **definite material**.

## Concepts

| Term | Meaning |
|---|---|
| **Cost rate** | Valuation price of a material/charge used for stock valuation |
| **Material** | The master data record whose cost rate is corrected |
| **Booking parameters** | The posting parameter set (e.g. the material) that the correction commands act on |

## Screen layout

The app consists of a dockable **Explorer** window (search list, auto-hide at the
bottom) and the main form:

| Area | Content |
|---|---|
| Header | **Correction Costrate** |
| **Material** group | **Material** selection combo box (booking parameter) |
| Tab **Cost rate** | Group **Bookingparameter** with the buttons **Adjustment Costrate** and **Adjustment Costrate definite Material** |

## Step-by-step tasks

### Searching materials

1. Enter a search string in the search field.
2. Click **Search** — the matching materials appear in the Explorer list.
3. Select a row to make it the current record.

### Correcting all cost rates

1. *(Optional)* Select the **Material** in the **Material** group.
2. Click **Adjustment Costrate** — the cost rates of all charges are corrected and
   posted as booking records.

### Correcting the cost rate of one material

1. Select the **Material** whose cost rate should be corrected.
2. Click **Adjustment Costrate definite Material** — only the charges of this material
   are revaluated.

### Resetting the entry mask

* Click **New** to clear all booking parameters and start a fresh entry. This does
  **not** create a database record. (The app does this automatically on start-up.)
* Click **Load** to re-read the current record if another user changed it in the
  background.

## Field reference

| Field | Description |
|---|---|
| Material | Material whose cost rate is corrected (booking parameter) |
| Cost rate values | Part of the booking parameter set; posted by the correction commands |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityCorrectionCost`
contains the complete usage recipe (search → navigate → edit booking parameters →
execute correction command). The button captions above are the translations of the
command methods (`ACMethod*` attributes, stored in `ACClassText`), e.g.
**Adjustment Costrate** → `CorrectionCostRateAll()`,
**Adjustment Costrate definite Material** → `CorrectionCostRateMaterial()`.
