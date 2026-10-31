# Components List (Batch Plan Components Dialog)

> Business object: `BSOProdOrderBatchComponents` (Assembly `gip.bso.manufacturing`)

## Definition

The **Components List** dialog shows the **components of a batch plan** with
their quantities scaled to the batch size. It is opened by another component
(e.g. from the batch plan processing) and is read-only — the operator can
check which materials are needed for the current batch and in which quantity.

## Concepts

| Term | Meaning |
|---|---|
| **Batch plan** | Planned production batch of a production order position |
| **Batch size** | Quantity of the current batch |
| **Prod. target quantity** | Target quantity of the production position the batch belongs to |
| **Component** | Root outward BOM position of the batch plan |

The component quantities are scaled from the BOM default quantity to the batch
size: `component quantity = BOM quantity × batch size / prod. target quantity`.

## Screen layout

A dialog with three tabs:

| Tab | Content |
|---|---|
| **Components** | Component list (sequence, material no./name, unit, target quantity) with **Batch Size** and **Prod. Target Quantity** headers |
| **Recipe description** | Name, number and comment of the bill of material |
| **Image** | Product image of the BOM material |

## Using the dialog (step by step)

1. The dialog is opened for a batch plan — the component list is loaded with
   the quantities scaled to the **Batch Size**.
2. Switch to the **Recipe description** tab to read the BOM comment.
3. Switch to the **Image** tab to see the product image.
4. Close the dialog.

## Field reference (components tab)

| Field | Description |
|---|---|
| Sequence | Position of the component in the BOM |
| Material No. / Material Name | Component material |
| Unit | Unit of the component quantity |
| Target quantity | Quantity scaled to the batch size |
| Batch Size | Batch size of the batch plan |
| Prod. Target Quantity | Target quantity of the production position |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server.
The class-level `<summary>` of `BSOProdOrderBatchComponents` contains the
complete usage recipe: the dialog is opened via
`ShowDialogComponent(paOrderInfo)` with a `ProdOrderBatchPlan` entity in the
`PAOrderInfo`; the scaled components are in `ComponentList`, the batch plan in
`BatchPlan`, the BOM in `Partslist` and the material image in
`PartslistImage`. This BSO is read-only.
