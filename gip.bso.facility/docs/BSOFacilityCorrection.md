# Stock Correction (Facility Correction)

> Business object: `BSOFacilityCorrection` (Assembly `gip.bso.facility`, started via the **Businessobject-Manager / Application Object Manager**)

## Definition

The **Stock Correction** app is used to correct stock records (**charges** on storage
bins) by posting booking records. It covers:

* **Stock corrections** — correct the stock of a charge/material to a target quantity
* **Zero-stock postings** — set the stock of a material or a storage bin to zero
* **Change of storage unit** — convert a charge quantity into another unit of measure
* **Adjustments (matching)** — reconcile quantities with weights, and reconcile stock
  for all or definite storage bins and materials
* **Cost rate corrections** — correct the valuation prices (cost rates) of charges

All functions are executed as **facility bookings**: the parameters entered on the
screen are collected in a booking parameter set and posted with a single button click.

## Concepts

| Term | Meaning |
|---|---|
| **Charge (FacilityCharge)** | A stock record: material + lot on a storage bin with a quantity |
| **Storage bin (Facility)** | Physical storage location |
| **Storage bin location (FacilityLocation)** | Grouping level above storage bins |
| **Booking parameters** | The posting parameter set (material, storage bin, charge, quantities) that the booking commands act on |
| **Adjustment / Matching** | Recalculation that reconciles stored quantities with weights or with the stock of other records |

## Screen layout

The app consists of a dockable **Explorer** window (search list, auto-hide on the right)
and the main form with two groups:

| Group | Content |
|---|---|
| **Quant** | Booking parameters: **Material**, **Storage Bin Location** (disabled), **Storage Bin**, **Charge** — each a selection combo box |
| **PostingParameter** | Buttons: **Stock Correction**, **Material Stock = 0**, **Storage Bin Stock = 0** |

The Explorer window contains the result list of charges; selecting a row makes it the
current record.

## Step-by-step tasks

### Searching charges

1. Enter a search string in the search field.
2. Click **Search** — the matching charges appear in the Explorer list.
3. Select a row to make it the current record.

### Correcting stock

1. Select the **Material**, **Storage Bin** and **Charge** in the **Quant** group.
2. Enter the target quantity in the booking parameters.
3. Click **Stock Correction** — the correction is posted as a booking record.

### Setting stock to zero

* Click **Material Stock = 0** to post all charges of the selected material to zero stock.
* Click **Storage Bin Stock = 0** to post the selected charge to zero stock.

### Changing the storage unit

1. Select the charge and enter the new unit/quantity in the booking parameters.
2. Click **Change Unit** — the quantity is converted and posted.

### Adjustments (matching)

The following buttons reconcile stored data (each runs a booking):

* **Completeadjustment Quantity with Weight** — all charges
* **Completeadjustment Quantity with Weight Material** — the selected material only
* **Adjustment Stock all Facilitylocation** — all storage bins
* **Adjustment Stock definite Facilitylocation** — the selected storage bin only
* **Adjustment Stock all Material** — all materials
* **Adjustment Stock definite Material** — the selected material only

### Cost rate corrections

* **Adjustment Costrate** — corrects the cost rates of all charges.
* **Adjustment Costrate definite Material** — corrects the cost rate of the selected
  material only.

### Resetting the entry mask

* Click **New** to clear all booking parameters and start a fresh entry. This does **not**
  create a database record.
* Click **Load** to re-read the current record if another user changed it in the background.

## Field reference (booking parameters)

| Field | Description |
|---|---|
| Material | Material whose stock is corrected |
| Storage Bin Location | Grouping level (read-only on screen) |
| Storage Bin | Storage bin of the charge |
| Charge | The stock record to be corrected |
| Quantity fields | Target quantity / unit of the correction (part of the booking parameter set) |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityCorrection`
contains the complete usage recipe (search → navigate → edit booking parameters →
execute booking command). The button captions above are the translations of the
command methods (`ACMethod*` attributes, stored in `ACClassText`), e.g.
**Stock Correction** → `FacilityStockCorrection()`, **Change Unit** → `ChangeUnit()`,
**Material Stock = 0** → `MaterialZero()`, **Storage Bin Stock = 0** → `FacilityZero()`,
**Adjustment Costrate** → `CorrectionCostRateAll()`.
