# Purchase Order (Bestellverwaltung)

> Business object: `BSOInOrder` (Assembly `gip.bso.purchasing`, Menu: **Purchasing → Purchase Order**)

## Definition

The **Purchase Order** app manages incoming purchase orders (**InOrder**) and their
positions (**InOrderPos**). A purchase order records what a distributor/supplier will
deliver, in which quantities, at which prices and until which delivery date.

A special case is the **contract**: a contract order contains contract lines that are
not delivered directly but **called off** by subsequent release/internal orders. This
app supports creating such call-offs and revising them.

The app replaces the older masks *Goods Acceptance → Orders* and
*Goods Acceptance → Order Positions*.

## Concepts

| Term | Meaning |
|---|---|
| **Purchase Order (InOrder)** | The order header: distributor, delivery/billing address, delivery dates, prices, state |
| **Order Type** | Distinguishes **Contract**, **Release Order**, **Internal Order** etc.; the type cannot be changed once positions exist |
| **Order State** | Status of the order (e.g. Cancelled) |
| **Order Position (InOrderPos)** | One material line of the order with quantity, unit, delivery date, prices and state |
| **Contract line** | Position of a contract order that can be called off by other orders |
| **Call-off** | Assignment of a contract line to a release/internal order position (full or partial quantity) |
| **Pick-up master (CompanyMaterialPickup)** | Loading/pick-up master data attached to an order position for an external material |
| **Tenant** | The client (mandant) the order belongs to, with its invoice address and invoice person |

## Screen layout

The app consists of a dockable **Explorer** window (search list) and the main form
(**InOrderForm**), which contains the order header and the position area:

| Area | Content |
|---|---|
| **Explorer** (search list) | Search fields **Order No.**, **Comment**, **Material** filter, **Order state** filter; result list with order no., date, type, state, delivery dates; buttons **Search** and **Cancel selected** |
| **Order header** | Distributor, distributor order no., delivery address, billing address, terms of payment, delivery type, time range, target delivery date / max. delivery date, prices, comment, based-on request, currency, last changed by/on |
| **Positions** | Position list (position no., material, quantity, unit, state, delivery date) with buttons **New Item** / **Delete Item**, and the position detail form with the sections *Purchase Order Data*, *Pricing*, *Status* and *Comments* |

## Searching purchase orders

1. Enter the **Order No.** and/or **Comment** search text in the Explorer.
2. *(Optional)* Restrict by **Material** and/or **Order state**.
3. Click **Search**. The result list is sorted by order no. (newest first).
   Orders that are only part of an unpublished material requirements proposal are hidden.
4. Select an order to open it in the main form.

## Creating a new purchase order (step by step)

1. Click **New**. A new order with a new order number is created and opened;
   the own delivery address and the invoice address/person from your user settings are pre-assigned.
2. Select the **Order Type** (only possible while the order has no positions).
3. Select the **Distributor** (supplier), the **Delivery Address** and the **Billing Address**.
4. Enter the **Target Delivery Date** (and optionally the latest delivery date), delivery type, terms of payment and prices.
5. Click **Save**. Confirm a warning message with **Yes**.

## Adding positions

1. Click **New Item** — a new position is added to the position list and selected.
2. Assign the **Material**; the unit defaults to the base unit of the material.
3. Enter the **Target Quantity** (with unit) and the position delivery dates.
4. Click **Save** (confirm warnings with **Yes**).
5. **Delete Item** removes the selected position. If the position is a call-off of a
   contract line, the call-off is reverted instead of deleting the position.

## Calling off contract lines

Open contract lines (contract positions that are not yet completely assigned) are
listed with their material and delivery date:

1. Select the contract line in the list.
2. *(Optional)* Enter a **Partial Quantity** to call off only part of the line.
3. Click **Call off contract line** — a new position is created in the current order
   and linked to the contract line. The list refreshes automatically.
4. To revert a call-off, select the call-off position and click **Revise contract**.
5. **Filter** opens the filter dialog for the contract line list;
   **Find contract lines** re-reads the list.

## Cancelling orders

1. In the Explorer list, mark one or more orders as selected (checkbox).
2. Click **Cancel selected** — the marked orders are set to state *Cancelled* and
   their positions to *Completely Assigned*.

## Pick-up master data (per position)

1. Select the position and an entry in **Available Ext. Material**.
2. Click **New Pick Up** to attach a new pick-up master to the position.
3. **Load Pick Up** loads the selected pick-up master into the detail form;
   **Delete Pick Up** removes it.
4. Click **Save**.

## Delete, Undo, Load

* **Delete** — deletes the current purchase order; afterwards the first order of the list is shown.
* **Undo** — discards unsaved changes and refreshes the position and contract line lists.
* **Load** — re-reads the current order if another user changed it in the background.

## Field reference (order header)

| Field | Description |
|---|---|
| Order No. | Unique order number (assigned automatically on **New**) |
| Order Type | Contract / Release Order / Internal Order — locked once positions exist |
| Order State | Status of the order |
| Distributor | Supplier company (only companies flagged as distributor) |
| Distributor Order No. | Order number at the supplier |
| Delivery Address / Billing Address | Own company addresses used for delivery/invoicing |
| Address for Invoice / Person for invoice | Tenant invoice contact |
| Target Delivery Date / Target Delivery Max Date | Requested and latest delivery date |
| Delivery Type / Transport Mode / Time Range | Logistics master data |
| Terms of Payment | Payment conditions |
| Price Net / Price Gross | Order prices |
| Currency | Currency (from the outgoing order context) |
| Comment | Free text |
| Based On In Request | Request the order originates from |
| Update Name / Update Date | Last change (audit) |

Position fields: Position (line no.), Material, Target Quantity, Unit, Actual Quantity,
Called Up Quantity, Extern Quantity, Delivery dates (requested/confirmed/max), Delivery
Priority, Position State, Load State, Country Sales Tax, Prices, Comments.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOInOrder` contains the
complete usage recipe (search → navigate → edit → save) including the C# method names
(`Search`, `NavigateFirst`, `New`, `Save`, `Delete`, `Load`, `NewInOrderPos`,
`DeleteInOrderPos`, `AssignContractPos`, `UnAssignContractPos`,
`FilterDialogContractPos`, `RefreshOpenContractPosList`, `NewCompanyMaterialPickup`,
`LoadCompanyMaterialPickup`, `DeleteCompanyMaterialPickup`, `CancelSelectedInOrders`,
`ShowDialogNewInOrder`, `ShowDialogOrder`, `ShowDialogOrderInfo`, `DialogOK`,
`DialogCancel`), so an agent can operate the app remotely. The button captions above
are the translations of these methods (`ACMethod*` attributes, stored in `ACClassText`).
