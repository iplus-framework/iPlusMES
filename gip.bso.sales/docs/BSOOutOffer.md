# Offer (Quotation)

> Business object: `BSOOutOffer` (Assembly `gip.bso.sales`, Menu: **Sales → Offer management**)

## Definition

An **offer** (quotation) is a priced proposal to a customer: an offer header
(customer, offer number, dates, terms) with one or more **offer positions**
(material, quantity, unit price). From an accepted offer a **sales order** can be
generated with one click.

Offers are **versioned**: a new version copies the whole offer (header and all
positions) and increments the version number.

## Concepts

| Term | Meaning |
|---|---|
| **Offer** | The offer header: customer, dates, terms, totals |
| **Offer position** | A line of the offer: material, quantity, unit, price |
| **Sub position** | A position below a group position (hierarchical positions) |
| **Offer version** | A numbered copy of an offer (see *New version*) |
| **Tenant / Issuer** | The issuing company used for invoice data |

## Screen layout

The app is a search list on the left (**Explorer**) and a detail form on the right.
The detail form has these areas:

| Tab / area | Content |
|---|---|
| **Offer** (header) | Offer number, offer version, offer date, order type, offer state, customer, customer request no., delivery type, term of payment, currency, target delivery dates, net/gross price, comment |
| **Positions** | Position list (position, material, net price, total price) with the position detail form and the totals (net sum, discount, total) |
| **Position form** | Tabs: *Position info* (material, quantity, unit), *Pricing* (net price, gross price, sales tax, price lists), *Delivery dates*, *Comments* |
| **Tenant** | Issuer company, invoice address and invoice person |

## Creating a new offer (step by step)

1. Click **New**. A new offer with the next offer number and the default print
   templates is created and opened.
2. Select the **Customer**. Billing and delivery address lists are filled from the
   customer's addresses.
3. Enter the **Offer date** and the **Target delivery date** (and if needed the
   latest delivery date).
4. Set the **Order type**, **Delivery type**, **Term of payment** and **Currency**.
5. Click **Save**. If a warning message appears, confirm with **Yes**.

## Working with positions

On the **Positions** area:

1. Click **New Position** — a new line is added to the position list.
   (**New sub Position** adds a sub position below the selected line.)
2. Assign the **Material** on the selected line.
3. Enter the **Target Quantity** and choose the **Unit** (the quantity is converted
   to the selected unit).
4. Enter the **Price net** — optionally pick a valid entry from the **Price lists**
   of the material.
5. Click **Save** (confirm warnings with **Yes**).
6. **Delete Position** removes the selected line; **Position up** / **Position down**
   change the sequence; **Load Position** re-reads the selected line.

## Creating a new version

Click **New version** — the current offer is copied completely (header and all
positions, keeping the group hierarchy), the **version number** is incremented and
the new version is opened. Click **Save** if required.

## Generating the order

Click **Create Order** — a sales order is generated from the current offer. The
button is only active as long as no order based on this offer exists.

## Delete, Undo, Load

Toolbar functions:

* **Delete** — deletes the current offer (referenced records are reported as messages).
* **Undo** — reverts all unsaved changes.
* **Load** — re-reads the current offer if another user changed it in the background.
* **Search** — re-executes the search with the current filters (offer no., comment,
  offer state).

## Field reference (header)

| Field | Description |
|---|---|
| Offer No. | Number of the offer (assigned automatically on **New**) |
| Offer version | Version number (see *New version*) |
| Offer date | Date of the offer |
| Order type | Order type of the resulting order |
| Offer state | Status of the offer |
| Customer | Customer company; determines address and price lists |
| Customer request no. | Customer's own request reference |
| Target delivery date / max. date | Requested and latest delivery date |
| Delivery type / Term of payment / Currency / Time range | Offer terms |
| Price net / Price gross | Offer totals (recalculated from positions) |
| Comment | Free comment (also usable as search filter) |
| Issuer / Tenant | Issuing company with invoice address and person |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOOutOffer` contains the
complete usage recipe (search → navigate → edit → save) including the C# method
names: `Search`, `New`, `NewVersion`, `Save`, `UndoSave`, `Load`, `Delete`,
`LoadOutOfferPos`, `NewOutOfferPos`, `NewSubOutOfferPos`, `DeleteOutOfferPos`,
`OutOrderPosUp`, `OutOrderPosDown`, `CreateOutOrder`. The button captions above are
the translations of these methods (`ACMethod*` attributes, stored in `ACClassText`).
