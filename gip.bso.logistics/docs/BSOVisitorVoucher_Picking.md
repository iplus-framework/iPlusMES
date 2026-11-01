# Visitor Voucher — Picking Orders (Tab)

> Business object: `BSOVisitorVoucher` (Assembly `gip.bso.logistics`, Menu: **Logistics → Visitor Voucher**)
> This manual describes the **Pickings** tab of the Visitor Voucher app (partial class `BSOVisitorVoucher_Picking`).

## Definition

A **picking order** (Kommissionierauftrag) is a warehouse order that moves material from one storage
location to another. On the **Pickings** tab of a visitor voucher, picking orders are assigned to or
removed from the visitor's visit — typically vehicle loading/unloading at the gate.

## Concepts

| Term | Meaning |
|---|---|
| **Visitor Voucher** | The visit record of a vehicle/driver at the gate (check-in/check-out) |
| **Picking** | A picking order that can be assigned to the voucher |
| **Assigned Pickings** | Pickings already assigned to the current voucher |
| **Open Pickings** | Pickings not yet assigned to any voucher (types *Receipt Vehicle* / *Issue Vehicle*) |

## Screen layout

The **Pickings** tab shows two lists side by side:

| Area | Content |
|---|---|
| **Assigned Pickings** | Pickings of the current voucher: picking number, picking state, preparation status, picking type, delivery date, delivery company, comment |
| **Open Pickings** | Pickings without voucher, with filter fields: **From** / **to** (delivery date), **Material**, **Key of external system** |

Buttons: **Assign** (on the open list), **Remove** (on the assigned list), **Show Picking Order**, **Search** (refresh).

## Step-by-step tasks

### Assign a picking order to the voucher

1. Select the visitor voucher and open the **Pickings** tab.
2. In the **Open Pickings** list, optionally narrow the results with the **From**/**to** date,
   **Material** or **Key of external system** filters — the list refreshes automatically
   (or press **Search**).
3. Select a picking and click **Assign**. The picking moves to the **Assigned Pickings** list.
   The button is only active if the voucher has a visitor and the picking is unassigned.

### Remove a picking order from the voucher

1. Select the picking in the **Assigned Pickings** list.
2. Click **Remove**.

### Open a picking order

1. Select a picking in either list and click **Show Picking Order** — the picking order app opens
   with that order.

## Field reference (Open Pickings filter)

| Field | Description |
|---|---|
| From / to | Delivery date range of the pickings |
| Material | Material number filter |
| Key of external system | Contains-search in the external system key (e.g. ERP order number) |

Assigned list columns: Picking No., Picking State, Preparation Status, Picking Type, Delivery Date,
Delivery Company, Comment.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server. The class-level
`<summary>` of `BSOVisitorVoucher` (and this partial) contains the usage recipe. Relevant members of
this partial: `FilterPickingDateFrom`, `FilterPickingDateTo`, `FilterPickingMaterialNo`,
`FilterPickingKeyOfExternalSys`, `PickingList`, `SelectedPicking`, `CurrentPicking`,
`UnAssignedPickingList`, `SelectedUnAssignedPicking`, `CurrentUnAssignedPicking`,
`AssignPicking()`, `UnassignPicking()`, `NavigateToAPicking()`, `NavigateToUPicking()`,
`RefreshUnAssignedPickingList()`, `CleanFilterPicking()`.
