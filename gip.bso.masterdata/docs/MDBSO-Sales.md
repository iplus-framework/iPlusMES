# Sales Master Data — Value Lists (MDBSO)

> Business objects: `MDBSO*` (Assembly `gip.bso.masterdata`, Module **Sales**)

## What are master-data value lists?

Value lists are small, centrally maintained tables whose entries appear as
selections (combo boxes, status columns) in the operational screens of the
Sales module — for example the status of an invoice or the type of a sales
order. Each entry has a **multilingual name**, a **sort index** and an
optional **default flag**; some lists also carry a technical **key** used by
the system.

## Standard operation (identical for all lists in this document)

Every value list is maintained in the same way:

1. **Search** — loads (or re-filters) the entries into the list on the left.
2. **New** — creates a new entry and makes it the current record. Fill in all
   required fields (at least the **Name**).
3. **Edit** — select an entry in the list and change its fields on the right.
4. **Save** — writes the changes to the database. Always save before
   navigating to another entry or creating a new one; **Undo** discards
   unsaved changes.
5. **Delete** — removes the current entry. This is only possible if no other
   tables reference the entry (foreign key check); confirm with **Save**.
6. **Load** — reloads the selected entry from the database if another user
   has changed it in the background.

> **Caution:** entries are referenced by operational records (offers, sales
> orders, invoices …). Deleting an entry that is still in use is rejected by
> the system — retire it instead of deleting it if possible.

---

## Country Tax (`MDBSOCountrySalesTax`)

Maintains the **VAT rates per country**. A country tax entry defines the VAT
rate (in percent) that applies to a country for a **validity period**
(*Date from* – *Date to*). Below the selected entry, deviating rates can be
defined **per material** and **per material group**; the screen embeds the
material explorer to pick materials.

**Special functions:**

* **Assign material** — assigns the material selected in the embedded
  material explorer to the current tax entry, using the rate entered in the
  **VAT** field. Materials already assigned are no longer offered.
* **New** / **Delete Position** — add or remove a material position.
* **New Position** / **Delete Position** — add or remove a material group
  position.
* **Save** validates the entry: the period must not end before it starts,
  and validity periods of the same country must not overlap.

**Fields:** Country, Name, VAT rate, Date from, Date to, Is default; material
and material group positions with their own VAT rates.

**Values in the sample database:** none — the list is empty and can be filled
by the customer (e.g. *Germany, 19 %, from 01.01.2020*).

## Invoice State (`MDBSOInvoiceState`)

Status of an **invoice**; tracks the processing state of the document.

| Value | Meaning |
|---|---|
| Created *(default)* | Invoice has been created, not yet processed |
| In Process | Invoice is being processed |
| Completed | Invoice is finished |
| Blocked | Invoice is blocked |
| Cancelled | Invoice was cancelled |

## Invoice Type (`MDBSOInvoiceType`)

Classifies the **kind of an invoicing document**.

| Value | Meaning |
|---|---|
| Invoice *(default)* | Regular customer invoice |
| Purchase agreement (Contract) | Framework contract invoicing |
| Internal Invoice | Invoice for internal transactions |
| Release Invoice | Call-off invoice against a contract |

## Offer Status (`MDBSOOutOfferState`)

Status of a **customer offer**; tracks the offer workflow.

| Value | Meaning |
|---|---|
| New *(default)* | Offer created, not yet sent |
| Sended | Offer has been sent to the customer |
| Completed | Offer has been accepted/finished |
| Cancelled | Offer was cancelled |

## Sales Order Plan Status (`MDBSOOutOrderPlanState`)

Planning status of a **sales order**; tracks how far the order has been
planned and produced.

| Value | Meaning |
|---|---|
| Not Planned *(default)* | Order has not been scheduled yet |
| Subset Planned | Only part of the quantity has been planned |
| Completely Planned | Full quantity has been planned |
| Produced | Order has been produced |

## Sales Order Pos. Status (`MDBSOOutOrderPosState`)

Status of a **sales order position** (order line).

| Value | Meaning |
|---|---|
| Created *(default)* | Position created, not yet processed |
| In Process | Position is being processed |
| Completed | Position is finished |
| Blocked | Position is blocked |
| Cancelled | Position was cancelled |

## Sales Order Status (`MDBSOOutOrderState`)

Status of a **sales order** as a whole.

| Value | Meaning |
|---|---|
| Created *(default)* | Order created, not yet processed |
| In Process | Order is being processed |
| Completed | Order is finished |
| Blocked | Order is blocked |
| Cancelled | Order was cancelled |

## Sales Order Type (`MDBSOOutOrderType`)

Classifies the **kind of a sales order**.

| Value | Meaning |
|---|---|
| Order *(default)* | Regular sales order |
| Purchase agreement (Contract) | Framework contract |
| Internal Order | Order for internal transactions |
| Release Order | Call-off order against a contract |

---

## For AI agents (MCP)

The metadata of these business objects is exposed through the iPlus MCP
Server (`get_thesaurus`, `get_type_infos`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of each `MDBSO*` class
contains the complete usage recipe (Search → list → `Current<Entity>` →
Save/New/Delete/Load), so an agent can operate the value lists remotely, e.g.
for user training or data maintenance.
