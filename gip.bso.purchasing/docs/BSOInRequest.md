# Quotation (Request for Quotation / Purchase Inquiry)

> Business object: `BSOInRequest` (Assembly `gip.bso.purchasing`, Menu: **Purchasing → Quotation**)

## Definition

A **purchase inquiry** (request for quotation, RfQ) is a request sent to a supplier
(**distributor**) asking for an offer for one or more **materials** with quantities,
desired delivery dates and prices.

An inquiry consists of a **header** (inquiry number, date, version, supplier, delivery
type, payment terms, prices) and a list of **positions** (requested material, quantity,
unit, delivery dates, prices, comment).

## Concepts

| Term | Meaning |
|---|---|
| **Inquiry (InRequest)** | The inquiry header: number, date, version, state, supplier, prices |
| **Position (InRequestPos)** | One requested material with quantity, unit, delivery dates and prices |
| **Distributor** | The supplier company the inquiry is sent to |
| **Contractual company** | The contractual partner of the inquiry |
| **Delivery company address** | Delivery address of the own company (pre-assigned on new inquiries) |
| **Inquiry state** | Status of the inquiry (e.g. created, offered, completed) |

## Screen layout

The app shows the search list (**Explorer**, docked right) and the detail form. The
detail form has these areas:

| Tab / Area | Content |
|---|---|
| Header (top) | Inquiry number, version, inquiry date, order type, inquiry state |
| **InRequest** (tab) | Supplier data, delivery address, delivery type, payment terms, time range, target delivery dates, net/gross price, comment, last change (user/date) |
| **InRequestPos** (tab) | Position list (left) with **New Position**, **Load Position**, **Delete Position** buttons and the position form (right) |
| Position form | Position number, material, target quantity/weight, delivery dates, priority, time range, sales tax, net/gross price, comments |

## Creating a new inquiry (step by step)

1. Click **New** in the toolbar. A new inquiry is created with an automatically
   generated inquiry number and the delivery address of the own company.
2. Enter the **Inquiry date** and select the **order type** and the **inquiry state**.
3. On the **InRequest** tab select the **Distributor** (supplier), the contractual
   partner and the **delivery type**.
4. *(Optional)* Set the **target delivery date** / **maximum delivery date**, the
   **payment terms**, the **time range** and the **net/gross price**.
5. Click **Save**.

## Adding positions (requested materials)

On the **InRequestPos** tab:

1. Click **New Position** — a new line is added to the position list and opened in the
   position form.
2. Assign the requested **Material**. The unit list is filled with the units of the
   material and the base unit is pre-assigned.
3. Enter the requested quantity (**Target Quantity**) with its unit. The base quantity
   and the weight are calculated automatically from the unit.
4. *(Optional)* Set the **delivery dates**, the **priority**, the **time range**, the
   **sales tax**, the **net/gross price** and the **comments**.
5. Click **Save**.
6. Repeat for every material. **Delete Position** removes the selected position;
   **Load Position** re-reads the selected position.

## Delete, Undo, Load

Toolbar functions:

* **Delete** — deletes the current inquiry from the database and selects the first
  remaining inquiry.
* **Undo** — discards all unsaved changes of the inquiry and its positions.
* **Load** — re-reads the current inquiry (including its positions) if another user
  changed it in the background.
* **Search** — refreshes the inquiry list with the current search criteria.

## Field reference (header)

| Field | Description |
|---|---|
| Inquiry No. | Unique inquiry number (generated automatically) |
| Version | Version of the inquiry |
| Inquiry date | Date of the inquiry |
| Order type | Type of the purchase order the inquiry belongs to |
| Inquiry state | Status of the inquiry |
| Distributor | Supplier company |
| Delivery company address | Delivery address of the own company |
| Billing company address | Billing address |
| Delivery type | Requested delivery type |
| Payment terms | Terms of payment |
| Time range | Requested delivery time range |
| Target delivery date / max date | Desired delivery dates |
| Net price / Gross price | Prices of the inquiry |
| Distributor offering no. | Supplier's own offer number |
| Comment | Free comment |
| Update name / Update date | Last change (user/date) |

## Field reference (position)

| Field | Description |
|---|---|
| Position | Position number |
| Material | Requested material |
| Target Quantity / Unit | Requested quantity with unit |
| Target Weight | Weight calculated from the quantity |
| Target delivery date / max date | Desired delivery dates of the position |
| Priority | Delivery priority |
| Time range | Requested delivery time range |
| Sales tax | Country sales tax |
| Net price / Gross price | Position prices |
| Comment / Comment 2 | Free comments |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOInRequest` contains the
complete usage recipe (search → navigate → edit → save) including the C# method names
(`New`, `Save`, `Delete`, `Load`, `Search`, `NewInRequestPos`, `DeleteInRequestPos`,
`LoadInRequestPos`), so an agent can operate the inquiry app remotely. The button
captions above are the translations of these methods (`ACMethod*` attributes, stored
in `ACClassText`).
