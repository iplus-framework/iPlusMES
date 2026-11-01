# Visitor Voucher — Delivery Notes (Tab)

> Business object: `BSOVisitorVoucher` (Assembly `gip.bso.logistics`, Menu: **Logistics → Visitor Voucher**)
> This manual describes the **Deliverynotes** tab of the Visitor Voucher app (partial class `BSOVisitorVoucher_DNote`).

## Definition

A **delivery note** (Lieferschein) documents the goods movement of a shipment. On the
**Deliverynotes** tab of a visitor voucher, delivery notes are assigned to or removed from the
visit. New purchase or sales delivery notes can also be created directly from the voucher.

## Concepts

| Term | Meaning |
|---|---|
| **Delivery Note** | Purchase (EK) or sales (VK) delivery note |
| **Assigned Delivery Notes** | Delivery notes of the current voucher |
| **Open Delivery Notes** | Delivery notes not yet assigned to any voucher, sorted by delivery note number |
| **Tour plan position** | If a delivery note belongs to a tour plan, it is managed via the tour plan, not here |

## Screen layout

The **Deliverynotes** tab shows two lists side by side:

| Area | Content |
|---|---|
| **Assigned Delivery Notes** | Delivery notes of the current voucher |
| **Open Delivery Notes** | Delivery notes without voucher |

Buttons: **Assign** (on the open list), **Remove** (on the assigned list), **Show Deliverynote**.
On the voucher header: **New In Delivery Note** and **New Out Delivery Note** (create and assign a
new purchase/sales delivery note in a dialog).

## Step-by-step tasks

### Assign a delivery note to the voucher

1. Select the visitor voucher and open the **Deliverynotes** tab.
2. Select a delivery note in the **Open Delivery Notes** list.
3. Click **Assign**. The delivery note moves to the **Assigned Delivery Notes** list.
   The button is disabled for delivery notes that already belong to a **tour plan** — assign those
   via the tour plan (see *Tourplans* tab).

### Remove a delivery note from the voucher

1. Select the delivery note in the **Assigned Delivery Notes** list.
2. Click **Remove**. Also disabled for delivery notes with a tour plan position.

### Create a new delivery note for the voucher

1. On the voucher header click **New In Delivery Note** (purchase) or
   **New Out Delivery Note** (sales).
2. Complete the dialog; the new delivery note is created and assigned to the voucher.

### Open a delivery note

1. Select a delivery note in either list and click **Show Deliverynote** — the delivery note
   dialog opens with that document.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server. The class-level
`<summary>` of `BSOVisitorVoucher` (and this partial) contains the usage recipe. Relevant members of
this partial: `DeliveryNoteList`, `SelectedDeliveryNote`, `CurrentDeliveryNote`,
`UnAssignedDeliveryNoteList`, `SelectedUnAssignedDeliveryNote`, `CurrentUnAssignedDeliveryNote`,
`AssignDeliveryNote()`, `UnassignDeliveryNote()`, `NavigateToADeliveryNote()`,
`NavigateToUDeliveryNote()`, `RefreshUnAssignedDeliveryNoteList()`. Creating new delivery notes is
handled in the main class (`NewInDeliveryNote()` / `NewOutDeliveryNote()`).
