# Sales Order (Customer Order)

> Business object: `BSOOutOrder` (Assembly `gip.bso.sales`, Menu: **Sales → Order management**)

## Definition

A **sales order** records what a customer has ordered: an order header (customer, order
number, dates, terms of payment, currency) with one or more **order positions**
(material, quantity, unit price).

The app replaces the old screens *Goods issue → Orders* and *Goods issue → Order
positions*. Besides plain orders it also handles **contracts** and **contract
call-offs**: a contract holds open contract lines which are called off into
subsequent orders.

## Concepts

| Term | Meaning |
|---|---|
| **Sales order** | The order header: customer, dates, terms, totals |
| **Order position** | A line of the order: material, quantity, unit, price, delivery data |
| **Sub position** | A position below a group position (hierarchical positions) |
| **Contract** | An order of type *Contract*; its positions are **contract lines** |
| **Contract call-off** | Taking quantities from an open contract line into a normal order |
| **Partial quantity** | Call-off of only part of the contract line quantity |
| **Pickup master data** | External material of the customer assigned to a position (pickup) |
| **Tenant / Issuer** | The issuing company used for invoicing data |

## Screen layout

The app is a search list on the left (**Explorer**) and a detail form on the right.
The detail form has these areas:

| Tab / area | Content |
|---|---|
| **Order** (header) | Order number, order date, order type, order state, customer, customer order no., delivery type, term of payment, currency, target delivery dates, net/gross price, comment |
| **Positions** | Position list (position, material, quantity, unit, delivery date, state) with the position detail form and the totals (net sum, discount, total) |
| **Position form** | Tabs: *Position info* (material, quantity, unit, states), *Pricing* (net price, gross price, sales tax, price lists), *Delivery dates*, *Comments* |
| **Contract call-off** | Open contract lines with filters (material, contract date from/to) and the call-off action |
| **Pickup** | Pickup master data of the position (external material of the customer) |
| **Tenant** | Issuer company, invoice address and invoice person |

## Creating a new order (step by step)

1. Click **New**. A new order with the next order number is created and opened.
2. Select the **Customer**. Billing and delivery address lists are filled from the
   customer's addresses.
3. Enter the **Order date** and the **Target delivery date** (and if needed the
   latest delivery date).
4. Set the **Order type**, **Delivery type**, **Term of payment** and **Currency**.
5. Click **Save**. If a warning message appears, confirm with **Yes**.

## Working with positions

On the **Positions** area:

1. Click **New Position** — a new line is added to the position list.
   (**New sub Position** adds a sub position below the selected line.)
2. Assign the **Material** on the selected line.
3. Enter the **Target Quantity** and choose the **Unit** (the quantity is converted
   to the selected unit). Alternatively enter a **New Target Quantity**.
4. Enter the **Price net** — optionally pick a valid entry from the **Price lists**
   of the material.
5. Click **Save** (confirm warnings with **Yes**).
6. **Delete Position** removes the selected line; **Position up** / **Position down**
   change the sequence.

## Calling off contract lines

On the **Contract call-off** area:

1. Filter the open contract lines by **Material** and **Contract date from/to** and
   click **Find contract lines** (or **Filter** for the extended filter dialog).
2. Select a contract line. Optionally enter a **Partial Quantity**.
3. Click **Call off contract line** — the quantity is taken into the current order
   as a new position and the contract line state is updated.
4. To revise a call-off, select the generated position and click
   **Revise contract** — the position is deleted and the quantity returned to the
   contract line.
5. Click **Save**.

## Pickup master data (external material)

On the **Pickup** area of a position:

1. Select an entry in **Available Ext. Material**.
2. Click **New Pick Up** to create a pickup record for the position.
3. **Load Pick Up** shows the selected record, **Delete Pick Up** removes it.
4. Click **Save**.

## Invoicing and production

* **Create Invoice** — generates an invoice from the current order. You are asked
  for the **invoice date**. If invoices already exist for this order you can create
  a **correction invoice** linked to the previous one.
* **Create production order** — creates production orders for all positions whose
  material has an enabled bill of material.

## Delete, Undo, Load

Toolbar functions:

* **Delete** — deletes the current order (referenced records are reported as messages).
* **Undo** — reverts all unsaved changes.
* **Load** — re-reads the current order if another user changed it in the background.
* **Search** — re-executes the search with the current filters (order no., comment,
  material, order state).

## Field reference (header)

| Field | Description |
|---|---|
| Order No. | Number of the order (assigned automatically on **New**) |
| Order date | Date of the order |
| Order type | e.g. standard order, release order, contract, internal order |
| Order state | Status of the order |
| Customer | Customer company; determines address and price lists |
| Customer order no. | Customer's own order reference |
| Target delivery date / max. date | Requested and latest delivery date |
| Delivery type / Term of payment / Currency / Time range | Order terms |
| Price net / Price gross | Order totals (recalculated from positions) |
| Comment | Free comment (also usable as search filter) |
| Issuer / Tenant | Issuing company with invoice address and person |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOOutOrder` contains the
complete usage recipe (search → navigate → edit → save) including the C# method
names: `Search`, `New`, `Save`, `UndoSave`, `Load`, `Delete`, `NewOutOrderPos`,
`NewSubOutOrderPos`, `DeleteOutOrderPos`, `OutOrderPosUp`, `OutOrderPosDown`,
`AssignContractPos`, `UnAssignContractPos`, `RefreshOpenContractPosList`,
`FilterDialogContractPos`, `NewCompanyMaterialPickup`, `LoadCompanyMaterialPickup`,
`DeleteCompanyMaterialPickup`, `CreateInvoice`, `CreateProductionOrder`,
`ShowDialogOrder`, `DialogOK`, `DialogCancel`. The button captions above are the
translations of these methods (`ACMethod*` attributes, stored in `ACClassText`).
