# Production Materials Requirement

> Business object: `BSOProdMatRequirement` (Assembly `gip.bso.manufacturing`, Menu: **Planning → Production materials requirement**)

## Definition

The **Production Materials Requirement** app calculates and checks whether
enough material is available for the active production batch plans. It is a
calculation app — nothing is edited or saved; the results are only displayed.

## Concepts

| Term | Meaning |
|---|---|
| **Batch plan** | Planned production batch of a production order position |
| **Material requirement** | Quantity of a material needed by the batch plans |
| **Available quantity** | Stock quantity currently available |
| **Difference quantity** | Shortage (required − available) |

## Screen layout

A form with two lists:

| Element | Content |
|---|---|
| **Active batch plans** | Batch plans in the states *Created*, *Auto start* or *In process* (program no., BOM material, batch size, batch count) |
| **Material requirements** | Result list: material no./name, required, available, actual, remaining and difference quantities |
| Buttons | **Calculate materials requirement**, **Check materials requirement** |

## Checking the material requirement (step by step)

1. Click **Calculate materials requirement** to recalculate the requirement
   over **all active batch plans**. The result appears in the *Material
   requirements* list.
2. Select a batch plan in the *Active batch plans* list and click
   **Check materials requirement** to check **only this batch plan**. The
   result is shown as a message (or *OK* if no shortage was found).
3. Interpret the result list: a negative **difference quantity** means a
   material shortage.

## Field reference (material requirements)

| Field | Description |
|---|---|
| Material No. / Material Name | Required material |
| Program No. | Production order program number of the batch plan |
| Required quantity | Quantity needed by the batch plans |
| Available quantity | Stock quantity available |
| Actual quantity | Quantity actually booked/reserved |
| Remaining quantity | Quantity still to be provided |
| Difference quantity | Shortage (required − available) |
| Batch size / Total size | Batch size of the plan / total planned quantity |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server.
The class-level `<summary>` of `BSOProdMatRequirement` contains the complete
usage recipe: `ProdOrderBatchPlanList` holds the active batch plans
(Created/AutoStart/InProcess), `SelectedProdOrderBatchPlan` the selection;
`CalculateMaterialsRequirement()` fills `MatReqResultList` via the
`ACMatReqManager` service (selection in `SelectedMatReqResult`);
`CheckMaterialRequirement()` checks only the selected plan and shows a message.
This BSO is read-only.
