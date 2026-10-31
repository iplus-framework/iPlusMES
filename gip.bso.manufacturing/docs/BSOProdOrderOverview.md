# Production Order Overview

> Business object: `BSOProdOrderOverview` (Assembly `gip.bso.manufacturing`, Menu: **Manufacturing → Prodorder overview**)

## Definition

The **Production Order Overview** is a read-only evaluation of production orders and their
material inputs within a freely selectable period. It aggregates planned, target and actual
quantities, differences, good/scrap quantities and input statistics.

The app **does not create, change or delete any records** — it is purely for analysis.
From here you can navigate to the underlying production orders.

## Concepts

| Term | Meaning |
|---|---|
| **Production order** | Order produced from a BOM (partslist); identified by its order number |
| **Planned quantity** | Quantity planned for the order (target of the order header) |
| **Target quantity** | Sum of the target quantities of the order positions |
| **Actual quantity** | Quantity actually produced (incl. good and scrap portions) |
| **Input** | Material consumed by the orders (outward postings) |
| **Final product input** | Input quantity grouped by the final product material it is used for |
| **Zero posting** | Stock zero postings (inventory corrections) included in the input statistics |
| **Time filter** | Whether the period applies to the **production time** or the **booking time** |

## Screen layout

Top area: filter fields — **From**, **to**, **Time filter**, **Program No.**, **Material**,
**Department** — and the **Recalculate Statistics** button.

Below are four tabs:

| Tab | Content |
|---|---|
| **ProdOrder** | One row per production order partslist: order no., material, department, planned/target/actual output quantities, differences, good/scrap shares, usage quantities. Button: **Show Orders & Materials** |
| **Material** | The same data grouped and summed by material. Button: **Show Orders & Materials** |
| **Inputs** | Summed material inputs of all orders in the period, with detailed input statistics. Buttons: facility combo + **Choose facility**, **Sum material inputs** |
| **FinalInputs** | Inputs grouped by final product material. Buttons: **Show all**, facility combo + **Choose facility**, **Filter** |

Messages of the background operations (progress, execution time, errors) are shown in a
message area.

## Step-by-step tasks

### Evaluating orders and materials

1. Enter the period (**From** / **to**) — the period must not be longer than one year.
2. *(Optional)* Restrict by **Program No.**, **Material** or **Department**, and choose in
   **Time filter** whether the period refers to the production time or the booking time.
3. On the **ProdOrder** or **Material** tab click **Show Orders & Materials**.
   The evaluation is built in the background (progress dialog).
4. Select a row to see its details; on the **ProdOrder** tab you can open the underlying
   production order (**Show Order**).

### Summing material inputs

1. Switch to the **Inputs** tab and click **Sum material inputs**.
2. *(Optional)* Select a storage bin in the facility combo or click **Choose facility** to
   pick one from the facility explorer, then click **Filter** to reduce the list to the
   bookings of this storage bin.
3. Select an input row and use **Show Order Inputs** to see all order positions which
   consume this material; from there you can jump to the order (**Show Order**).

### Complete evaluation at once

Click **Show all** (FinalInputs tab): this loads the order overview, the material overview,
the summed inputs and the final product inputs in one run.

### Recalculating statistics

Click **Recalculate Statistics** to recalculate the statistics of all orders in the period
in the background (e.g. after booking corrections).

## Field reference (order/material tabs, excerpt)

| Field | Description |
|---|---|
| Program No. | Production order number |
| Material No. / Material Name | Produced material |
| Unit | Technical symbol of the unit |
| Department | Department user of the order |
| Planned / Target / Actual quantity | Output quantities of the order |
| Difference quantity / % | Deviation actual vs. target (absolute and percent) |
| Good / Scrap quantity and shares | Actual quantity split into good and scrap |
| Usage target / actual quantity | Consumed input quantities of the order |
| Input statistics (Inputs tabs) | Input quantity related to actual, good, scrap and final output, each with difference and percent |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOProdOrderOverview` contains the
complete usage recipe. Relevant methods: `Search`, `Search2`, `SearchInputs`,
`FilterFacilityInputs`, `FilterFacilityFinalInputs`, `ShowDlgInputFilterFacility`,
`ShowDlgFinalInputFilterFacility`, `ShowOrderPositionsForInput`, `NavigateToProdOrder`,
`NavigateToProdOrder1`, `RecalculateAllStats`. Filter via `FilterStartDate`, `FilterEndDate`,
`FilterProgramNo`, `FilterMaterialNo`, `FilterDepartmentName`,
`SelectedFilterTimeFilterType`. Results: `OverviewProdOrderPartslistList`,
`OverviewMaterialList`, `InputList`, `FinalInputList`,
`OrderPositionsForInputList`. This BSO has no `New`/`Save`/`Delete` — it is read-only.
