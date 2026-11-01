# Visitor Voucher — Tour Plans (Tab)

> Business object: `BSOVisitorVoucher` (Assembly `gip.bso.logistics`, Menu: **Logistics → Visitor Voucher**)
> This manual describes the **Tourplans** tab of the Visitor Voucher app (partial class `BSOVisitorVoucher_Tour`).

## Definition

A **tour plan** (Tourenplan) bundles delivery notes and pickings into one vehicle tour. On the
**Tourplans** tab of a visitor voucher, a tour plan is assigned to the visit — the vehicle of the
tour plan should match the vehicle of the checked-in visitor.

## Concepts

| Term | Meaning |
|---|---|
| **Tourplan** | A planned vehicle tour with tour positions (delivery notes, pickings) |
| **Assigned Tourplans** | The tour plan assigned to the current voucher (max. one) |
| **Open Tourplans** | Tour plans not yet assigned to any voucher, sorted by tour plan number |

## Screen layout

The **Tourplans** tab shows two lists side by side:

| Area | Content |
|---|---|
| **Assigned Tourplans** | Tour plan of the current voucher |
| **Open Tourplans** | Tour plans without voucher |

Buttons: **Assign** (on the open list), **Remove** (on the assigned list), **Show Tourplan**.

## Step-by-step tasks

### Assign a tour plan to the voucher

1. Select the visitor voucher and open the **Tourplans** tab.
2. Select a tour plan in the **Open Tourplans** list.
3. Click **Assign**.
   - If the tour plan's vehicle differs from the visitor's vehicle, a confirmation message about
     the **vehicle change** appears — confirm with **Yes** to proceed or **No** to cancel.
4. The tour plan moves to the **Assigned Tourplans** list. Only **one** tour plan can be assigned
   per voucher; the **Assign** button is disabled once one is assigned.

### Remove the tour plan

1. Select the tour plan in the **Assigned Tourplans** list.
2. Click **Remove**.

### Open a tour plan

1. Select a tour plan in either list and click **Show Tourplan** — the tour plan dialog opens
   with that tour plan.

## Notes

* Delivery notes that already belong to a tour plan are assigned/removed **via the tour plan**,
  not on the Delivery Notes tab (the **Assign**/**Remove** buttons are disabled there for such
  delivery notes).

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server. The class-level
`<summary>` of `BSOVisitorVoucher` (and this partial) contains the usage recipe. Relevant members of
this partial: `TourplanList`, `SelectedTourplan`, `CurrentTourplan`, `UnAssignedTourplanList`,
`SelectedUnAssignedTourplan`, `CurrentUnAssignedTourplan`, `AssignTourplan()`,
`UnassignTourplan()`, `NavigateToATourplan()`, `NavigateToUTourplan()`,
`RefreshUnAssignedTourplanList()`.
