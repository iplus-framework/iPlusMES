# Weighing

> Business object: `BSOWeighing` (Assembly `gip.bso.logistics`, Menu: **Logistics → Weighing**)

## Definition

A **weighing** is a weight capture event. Each weighing record stores the weighed
weight, the time range and an identifier (e.g. truck/license plate or container
number) and can be assigned to one of the following documents:

* **Picking** (picking order)
* **Purchase Order** (goods receipt)
* **Sales Order** (goods issue)
* **Lab Order**
* **Visitor Voucher**

The weighing app is used to search, display and reclassify weighings — in
particular to change the **weighing state** of one or more weighings in bulk.

## Concepts

| Term | Meaning |
|---|---|
| **Weighing** | A single weight capture event with number, state, weight and time range |
| **Weighing state** | Lifecycle status of the weighing (e.g. created, in process, finished) |
| **Assigned document** | The Picking / Purchase Order / Sales Order / Lab Order / Visitor Voucher the weighing belongs to |
| **Delivery note** | Delivery note of the purchase order or of the visitor voucher of the weighing |

## Screen layout

The app consists of a search list (left) and a detail form (right).

**Search list (Explorer)** — filter area and result grid:

| Filter | Meaning |
|---|---|
| **Weighing** | Weighing number (search word) |
| **Picking** / **Picking-No.** | Only weighings of a picking order / its number |
| **Purchase Order** / **Purchase Order Number** | Only weighings of a purchase order / its number |
| **Sales Order** / **Sales Order Number** | Only weighings of a sales order / its number |
| **Lab Report** / **Lab Report Number** | Only weighings of a lab order / its number |
| **Visitor Voucher** / **Visitor Voucher No.** | Only weighings of a visitor voucher / its number |
| **Status** (filter) | Weighing state |
| **Select all** | Selects/deselects all rows in the result grid |

Result grid columns: selection checkbox, weighing number, state, weight, start/end
date, identifiers and the numbers of the assigned documents.

**Detail form (Weighing)** — data of the selected weighing: weighing number,
state, weight, start/end date, identifiers, the assigned document numbers and the
delivery note.

## Changing the weighing state (bulk)

1. Set the filters so that the relevant weighings appear in the list
   (each filter change refreshes the list automatically).
2. Mark the rows with the selection checkbox, or use **Select all**.
3. Choose the desired target state in the **Status** selection (target state).
4. Click **Change Weighing state** — the state of all marked rows is set.
5. Click **Save** to persist the changes, or **Undo** to discard them.

## Working with records

* **Search** — executes the search with the current filter values.
* **Load** — reloads the selected weighing if another user changed it in the background.
* **Save** / **Undo** — persist or discard changes.

## Navigating to the assigned document

With a weighing selected, one of the following buttons opens the assigned
document (only enabled if the weighing has such an assignment):

* **Show Picking**
* **Show In Order** (purchase order)
* **Show Out Order** (sales order)
* **Show Lab Order**
* **Show Visitor Voucher**

## Field reference (detail form)

| Field | Description |
|---|---|
| Weighing No. | Number of the weighing |
| Weighing state | Lifecycle status |
| Weight | Captured weight |
| Start date / End date | Time range of the weighing |
| Ident number / Ident number 2 | Free identifiers (e.g. truck, container) |
| Picking-No. | Assigned picking order |
| Purchase Order Number | Assigned purchase order |
| Sales Order Number | Assigned sales order |
| Lab Order Number | Assigned lab order |
| Visitor Voucher No. | Assigned visitor voucher |
| Delivery Note | Delivery note of the purchase order / visitor voucher |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOWeighing` contains the
complete usage recipe (filters → `Search` → `NavigateFirst` → `CurrentWeighing`,
`ChangeWeighingState`, `Save`, `Load`, `NavigateToPicking`, ...), so an agent can
operate the weighing app remotely, e.g. for user training. The button captions above
are the translations of these methods (`ACMethod*` attributes, stored in `ACClassText`).
