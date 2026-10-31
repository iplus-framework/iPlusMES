# Fast View (Track & Trace Fast View for Charge/Lot)

> Business object: `BSOTandTFastView` (Assembly `gip.bso.facility`; embedded child of **Charge Overview** (`BSOFacilityChargeOverview`) and **Lot Overview** (`BSOFacilityLotOverview`))

## Definition

The **Fast View** is a lightweight track & trace view for a **single charge or lot**.
Instead of the full tracking graph it answers one question with one query:

> *In which production orders was this charge/lot consumed, how much of it was used, and which final material came out?*

It is a **read-only** view — no data is created or changed.

## Concepts

| Term | Meaning |
|---|---|
| **Charge** (Facility charge) | A physical stock quantity of a material on a storage bin |
| **Lot** (Facility lot) | Lot under which charges are managed |
| **Fast view row** | One production order in which the traced charge/lot was consumed |
| **Consumed quantity** | Sum of the outward bookings of the traced charge/lot in that order |

## Screen layout

The app is a single tab (docking layout `Mainlayout` → `TandTFastView`):

| Area | Content |
|---|---|
| Toolbar | **Search** button |
| Result grid | Columns: **Production date**, **Order no.**, **Consumed quantity** (with unit), **Material no.**, **Material name**, **Produced quantity** (with unit), **Final material no.**, **Final material name**, **Final quantity** (with unit) |

## How it is used

1. Open the **Charge Overview** or **Lot Overview** app and select a charge or lot.
2. The Fast View is automatically fed with the selected record (the parent app sets the filter).
3. Click **Search** — all production orders in which the charge/lot was consumed are listed.
4. Select a row and click **Show ProdOrder** to open the production order in the order dialog.

## Field reference (result grid)

| Field | Description |
|---|---|
| Production date | Start date of the production order (fallback: creation date) |
| Order no. | Program number of the production order |
| Consumed quantity / unit | Sum of the outward quantities of the traced charge/lot in this order |
| Material no. / Material name | Material produced by the order stage |
| Produced quantity / unit | Actual quantity of that stage |
| Final material no. / Final material name | Material of the last stage of the order (the final product) |
| Final quantity / unit | Target quantity of the final stage |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOTandTFastView` contains the
complete usage recipe. Key members:

* Filter: `FilterFacilityCharge` / `FilterFacilityLot` (exactly one is set; set via `SetFaciltiyCharge()` / `SetFaciltiyLot()` by the parent BSO)
* Methods: `Search()` (fills `FastViewList` in a background worker), `NavigateToProdOrder()` (opens the order of `SelectedFastView`)
* Result: `FastViewList` (list of `TandTFastViewModel`), `SelectedFastView`

Button captions above are the translations of these methods (`ACMethod*` attributes,
stored in `ACClassText`).
