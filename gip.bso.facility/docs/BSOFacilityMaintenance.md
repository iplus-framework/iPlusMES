# Facility Maintenance Functions

> Business object: `BSOFacilityMaintenance` (Assembly `gip.bso.facility`, started via the **Businessobject-Manager / Application Object Manager**)

## Definition

The **Facility Maintenance** app provides **maintenance functions for stock records**
(charges on storage bins). It reconciles ("matches") stored data:

* **Quantity/weight adjustments** — reconcile the stored quantities of charges with
  their weights
* **Stock adjustments** — reconcile the stock of storage bins and materials with the
  posted charges

All functions are executed as **facility bookings**: the parameters entered on the
screen are collected in a booking parameter set and posted with a single button click.

## Concepts

| Term | Meaning |
|---|---|
| **Charge (FacilityCharge)** | A stock record: material + lot on a storage bin with a quantity |
| **Storage bin (Facility)** | Physical storage location |
| **Storage bin location (FacilityLocation)** | Grouping level above storage bins |
| **Adjustment / Matching** | Recalculation that reconciles stored quantities with weights or with the stock of other records |
| **Booking parameters** | The posting parameter set (material, storage bin, charge, quantities) that the maintenance commands act on |

## Screen layout

The app consists of a dockable **Explorer** window (search list, auto-hide at the
bottom) and the main form:

| Area | Content |
|---|---|
| Header | **Facility maintenance functions** |
| **Charge** group | Booking parameters: **Material**, **Storage Bin Location**, **Storage Bin**, **Charge** — each a selection combo box |
| Tab **Maintenance functions** | Group **Bookingparameter** with the six adjustment buttons |

## Step-by-step tasks

### Searching charges

1. Enter a search string in the search field.
2. Click **Search** — the matching charges appear in the Explorer list.
3. Select a row to make it the current record.

### Quantity/weight adjustments

* Click **Completeadjustment Quantity with Weight** — reconciles the quantities with
  the weights of **all** charges.
* Click **Adjustment Quantity with Weight** — reconciles quantities with weights for
  the **selected material** only.

### Stock adjustments

* Click **Adjustment all Facilitylocation** — reconciles the stock of **all** storage bins.
* Click **Adjustment definite Facilitylocation** — reconciles the stock of the
  **selected storage bin** only.
* Click **Adjustment Stock** — reconciles the stock of **all** materials.
* Click **Adjustment Stock definite Material** — reconciles the stock of the
  **selected material** only.

### Resetting the entry mask

* Click **New** to clear all booking parameters and start a fresh entry. This does
  **not** create a database record. (The app does this automatically on start-up.)
* Click **Load** to re-read the current record if another user changed it in the
  background.

## Field reference (booking parameters)

| Field | Description |
|---|---|
| Material | Material of the charge to be maintained |
| Storage Bin Location | Grouping level above storage bins |
| Storage Bin | Storage bin of the charge |
| Charge | The stock record to be maintained |
| Quantity fields | Part of the booking parameter set; posted by the adjustment commands |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityMaintenance`
contains the complete usage recipe (search → navigate → edit booking parameters →
execute maintenance command). The button captions above are the translations of the
command methods (`ACMethod*` attributes, stored in `ACClassText`), e.g.
**Completeadjustment Quantity with Weight** → `MatchingWeightWithQuantityAll()`,
**Adjustment all Facilitylocation** → `MatchingStockAll()`,
**Adjustment Stock definite Material** → `MatchingStockMaterialMaterial()`.
