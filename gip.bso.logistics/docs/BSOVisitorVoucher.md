# Visitor Voucher (Besucher Beleg)

> Business object: `BSOVisitorVoucher` (Assembly `gip.bso.logistics`, Menu: **Logistics → Visitor Voucher**)

## Definition

A **visitor voucher** documents the visit of a vehicle/person at the plant:
check-in and check-out, the assigned visitor (driver), vehicle and trailer,
assigned delivery notes, tour plans and pickings, and the vehicle weighings
(entry/exit weight with difference). It is the gate & weighing document of the
logistics process.

## Concepts

| Term | Meaning |
|---|---|
| **Visitor Voucher** | The document itself: number, state, check-in/check-out dates, comment |
| **Visitor** | The person (driver) assigned to the voucher, with company and vehicle |
| **Vehicle / Trailer** | Facilities (storage units) of type vehicle/trailer assigned to the visit |
| **Check In / Check Out** | State transitions of the voucher; check-out also processes assigned pickings, delivery notes and reservations |
| **Delivery Note** | Inbound or outbound delivery note assigned to the visit |
| **Tourplan / Picking** | Tour plan or picking order assigned to the visit |
| **Weighing** | Vehicle scale weighing (alibi weight) recorded for the voucher |

## Screen layout

The app is a search list on the left (**Explorer**: voucher no., person, vehicle,
trailer, check-in/out dates, picking/preparation status, state filter) and a
detail form on the right with these tabs:

| Tab | Content |
|---|---|
| **Visitor Details** | Voucher no., state, visitor, vehicle, trailer, check-in/check-out dates, comment |
| **Search Visitor / Create Visitor** | Visitor search fields (no., vehicle no., company no./name, person name, card no.) and **New Visitor** |
| **Deliverynotes** | Assigned delivery notes (**Assigned Delivery Notes**) and unassigned ones (**Open Delivery Notes**) with **Assign** / **Remove** / **Show Deliverynote**; **New Purchase Deliv.Note** / **New Sales Deliv.Note** |
| **Tourplans** | Assigned tour plans (**Assigned Tourplans** / **Open Tourplans**) with **Assign** / **Remove** / **Show Tourplan** |
| **Pickings** | Assigned pickings (**Assigned Pickings** / **Open Pickings**) with **Assign** / **Remove** / **Show Picking Order**, date/material filters |
| **Vehicle** / **Trailer** | Vehicle and trailer of the visit |
| **Weighings** | Scale selection, **Register Weight**, weighing list with state and weight, first/second weighing and difference |

## Creating a visitor voucher (step by step)

1. Click **New** in the toolbar. A new voucher with a new voucher number is
   created and opened.
2. On the **Search Visitor / Create Visitor** tab, search the driver in the
   **Find Visitor** fields (by visitor no., vehicle no., company, person name
   or card no.) and select him/her — or click **New Visitor** to create a new
   visitor in a dialog.
3. Enter vehicle and trailer on the **Vehicle** / **Trailer** tabs.
4. Click **Save**.

## Check-in / Check-out

1. Select the voucher and click **Check In** — the state becomes *checked in*
   and the check-in date is set. (Only possible for vouchers in state *New*.)
2. After loading/unloading click **Check Out** — the state becomes *checked
   out*; assigned pickings, delivery notes and facility reservations are
   processed. (Only possible for vouchers *checked in*.)
3. Click **Save**.

## Assigning delivery notes, tour plans and pickings

On the respective tab:

1. In the **Open …** list, select the unassigned document (use the filters and
   the search button to refresh the list).
2. Click **Assign** — the document is assigned to the voucher.
3. **Remove** un-assigns the selected document again.
4. **Show Deliverynote** / **Show Tourplan** / **Show Picking Order** opens the
   document.
5. **New Purchase Deliv.Note** / **New Sales Deliv.Note** create a new delivery
   note in a dialog and assign it directly.
6. Click **Save**.

## Weighing the vehicle

1. Select the **Vehicle scale** in the combo box.
2. Click **Register Weight** — an alibi weighing is recorded for the voucher
   (an error message appears if the scale is not connected).
3. The **Weighings** list shows all weighings with state and weight; the
   **First weighing**, **Second weighing** and the **Weighing difference**
   (net load) are displayed automatically.

## Toolbar functions

* **New** / **Delete** — create or delete the current voucher (confirm
  **Save** after **Delete**).
* **Save** / **Undo** — store or revert changes.
* **Load** — re-read the voucher if another user changed it in the background.
* **Search** — re-executes the search with the current state filter
  (**Filter visitor vouchers**, default: *checked out*).

## Field reference (Visitor Details)

| Field | Description |
|---|---|
| Visitor Voucher No. | Unique voucher number |
| State | New / Checked in / Checked out |
| Visitor | Assigned driver (no., name, company) |
| Vehicle / Trailer | Vehicle and trailer facility numbers |
| Check-In Date / Check-Out Date | Set by **Check In** / **Check Out** |
| Comment | Free comment |
| Picking Status / Preparation Status | Status info of the assigned pickings |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_type_infos`, `get_method_info`, `execute_acurl_command`). The class-level
`<summary>` of `BSOVisitorVoucher` contains the complete usage recipe
(`SearchWord` → `VisitorVoucherList` → `SelectedVisitorVoucher`/`CurrentVisitorVoucher`
→ edit → `Save()`), including the C# method names (`New`, `Delete`, `Save`,
`UndoSave`, `Load`, `Search`, `CheckIn`, `CheckOut`, `NewVisitor`,
`NewInDeliveryNote`, `NewOutDeliveryNote`, `RegisterWeight`,
`ShowDialogOrder`, ...). The button captions above are the translations of
these methods (`ACMethod*` attributes, stored in `ACClassText`). The assignment
lists (delivery notes, tour plans, pickings) are implemented in the partial
classes `BSOVisitorVoucher_DNote`, `BSOVisitorVoucher_Tour` and
`BSOVisitorVoucher_Picking` (Assign/Remove/Show methods).
