# Purchase Master Data — Value Lists (MDBSO)

> Business objects: `MDBSO*` (Assembly `gip.bso.masterdata`, Module **Purchase**)

## What are master-data value lists?

Value lists are small, centrally maintained tables whose entries appear as
selections (combo boxes, status columns) in the operational screens of the
Purchase module — for example the status of a purchase order or the type of
an order. Each entry has a **multilingual name**, a **sort index** and an
optional **default flag**; some lists also carry a technical **key** used by
the system.

## Standard operation (identical for all lists in this document)

Every value list is maintained in the same way:

1. **Search** — loads all entries of the list into the table on the left.
   (These simple lists have no search-word filter; the **Search** button
   refreshes the complete list.)
2. **New** — creates a new entry and makes it the current record. Fill in
   all required fields (at least the multilingual **Name**).
3. **Edit** — select an entry in the list and change its fields on the right.
4. **Save** — writes the changes to the database. Always save before
   navigating to another entry or creating a new one; **Undo** discards
   unsaved changes.
5. **Delete** — removes the current entry. This is only possible if no other
   tables reference the entry (foreign key check); confirm with **Save**.
6. **Load** — reloads the selected entry from the database if another user
   has changed it in the background.

> **Caution:** entries are referenced by operational records (purchase
> orders, order positions, requests …). Deleting an entry that is still in
> use is rejected by the system — retire it instead of deleting it if
> possible.

---

## Purchase Order Status (`MDBSOInOrderState`)

Status of a **purchase order** (header); tracks the processing state of the
whole order.

| Value | Meaning |
|---|---|
| Created *(default)* | Purchase order has been created, not yet processed |
| In Process | Purchase order is being processed |
| Completed | Purchase order has been completed |
| Blocked | Purchase order is blocked |
| Cancelled | Purchase order was cancelled |

## Purchase Order Position Status (`MDBSOInOrderPosState`)

Status of a **purchase order position**; tracks the processing state of each
order line.

| Value | Meaning |
|---|---|
| Created *(default)* | Position has been created, not yet processed |
| In Process | Position is being processed |
| Completed | Position has been delivered/completed |
| Blocked | Position is blocked |
| Cancelled | Position was cancelled |

## Purchase Order Type (`MDBSOInOrderType`)

Classifies **purchase orders** by their contractual nature.

| Value | Meaning |
|---|---|
| Order *(default)* | Normal order / purchase order |
| Purchase agreement (Contract) | Framework contract with a supplier |
| Internal Order | Internal order (within the company) |
| Release Order | Call-off against a framework contract |

## Request Status (`MDBSOInRequestState`)

Status of a **purchase request (inquiry/RFQ)** sent to suppliers.

| Value | Meaning |
|---|---|
| New *(default)* | Request has been created, not yet sent |
| Sent | Request has been sent to the supplier |
| Completed | Request has been answered/completed |
| Invalid | Request is invalid/withdrawn |

---

## For AI agents (MCP)

The metadata of these business objects is exposed through the iPlus MCP
Server (`get_thesaurus`, `get_type_infos`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of each `MDBSO*` class
contains the complete usage recipe (Search → list → `Current<Entity>` →
Save/New/Delete/Load), so an agent can operate the value lists remotely, e.g.
for user training or data maintenance.
