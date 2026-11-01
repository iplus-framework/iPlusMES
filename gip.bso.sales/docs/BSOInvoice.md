# Invoice

> Business object: `BSOInvoice` (Assembly `gip.bso.sales`, Menu: **Sales → Invoice**)

## Definition

An **Invoice** is the billing document issued to a customer for delivered goods and
services. The invoice app manages the invoice header (customer, billing/delivery
address, dates, currency, terms of payment, state), the invoice positions with prices
and taxes, the call-off of **contract lines**, alternative currencies/exchange rates,
and the printing of the invoice (including a tax overview).

Invoices can be cancelled with a **credit note** (storno) and optionally reissued as a
corrected invoice referencing the original one.

## Concepts

| Term | Meaning |
|---|---|
| **Invoice** | The invoice header: number, date, customer, state, totals |
| **Invoice position** | One billed item: material, quantity, unit, price, tax |
| **Contract line** | An open line of a customer contract (order type *Contract*) that can be called off into an invoice |
| **Call-off** | Taking a (partial) quantity from a contract line; creates an invoice position and a child order position under the contract line |
| **Storno / Credit note** | Cancellation document (type 381) reversing an incorrect invoice |
| **Correction invoice** | New corrected invoice (type 384) referencing the original invoice |
| **Alternative currency** | A second currency with an exchange rate valid at the invoice date |
| **Tenant** | The issuing company (issuer address/person printed on the invoice) |
| **Price list** | Source for taking over material prices into invoice positions |

## Screen layout

The app is a search list on the left and a detail form on the right.

**Search list (Explorer):** invoice number, date, customer, billing/delivery address,
state, reference invoice, comment — with filters for **Invoice No.**, **Comment**,
**Invoice state** and **Material** and a **Search** button.

**Invoice tab** — nested tabs:

| Tab | Content |
|---|---|
| **InvoiceForm** (header) | Customer data, billing/delivery address, issuer (tenant), dates, currency, terms of payment, state, totals, comment, reference invoice |
| **InvoicePos** | Position list and position form, plus the contract call-off area |

**PositionForm** sub-tabs: **PositionInfo** (material, quantity, unit, prices, tax),
**Pricing**, **Comments**, **Details**.

**AssignContractPos** (contract call-off) area: filter fields (Material, Contract date
from/to), the list of open contract lines, **Partial Quantity**, the
**Assign to current position** checkbox and the call-off buttons.

**AssignAlternativeCurrency** dialog: selection list of alternative exchange rates.

## Creating a new invoice (step by step)

1. Click **New**. A new invoice with a new invoice number is created and opened
   (issuer address/person are taken from your user settings).
2. Select the **Customer**. The billing and delivery address lists are reloaded.
3. Select the **Billing address** and the **Delivery address**.
   When the billing address with country is set, the invoice number is regenerated
   with a country code suffix.
4. Enter the invoice **date**, **terms of payment** and other header data.
5. Click **Save**.

## Adding invoice positions

1. On the position tab click **New Item** — a new position is created and selected.
2. Assign the **Material** and enter the **Target Quantity** with its unit
   (changing the unit converts the quantity automatically).
3. Enter the **PriceNet** — or select a price list position to take over the price.
4. Click **Save**.
5. **Delete Item** removes the selected position (a contract call-off is revoked
   first) and renumbers the remaining positions.

## Calling off contract lines

On the contract call-off area:

1. Enter the **Material** and/or the **Contract date from/to** filter and click
   **Find contract lines** (or **Filter** for the extended filter dialog).
2. Select an open contract line in the list.
3. Optionally enter a **Partial Quantity** (otherwise the full remaining quantity is
   called off).
4. To add the quantity to the currently selected invoice position instead of creating
   a new one, enable **Assign to current position**.
5. Click **Call off contract line** — an invoice position with material and quantity
   is created and the contract line is marked as called off.
6. Click **Save**.
7. **Revise contract** revokes the call-off of the selected invoice position and makes
   the contract line available again.

## Storno / Correction

1. Select the invoice to be cancelled.
2. Click **Storno**. Answer the question whether an additional correction invoice
   should be created (**Yes**) or only a credit note (**No**).
3. A credit note (type 381) reversing the original invoice is created — and, if
   confirmed, a corrected reissue invoice (type 384) referencing the original.
4. Check the new documents and click **Save** if needed.

## Alternative currency

* If the invoice currency has alternative exchange rates valid at the invoice date,
  click **Alternative Currency**, select the exchange rate and confirm with **OK**
  (**Cancel** closes without changes).
* **Update Exchange Rate** refreshes the exchange rate of the current invoice; a
  warning appears if no valid rate exists for the invoice date.

## Toolbar functions

* **Save** — saves the invoice with all positions.
* **Undo** — discards all unsaved changes and refreshes the contract line list.
* **Load** — re-reads the selected invoice if another user changed it in the background.
* **Delete** — deletes the current invoice with all positions after confirmation.
* **Search** — refreshes the invoice list with the current filters.

## Field reference (header)

| Field | Description |
|---|---|
| Invoice No. | Unique invoice number (with country suffix from the billing address) |
| Invoice date | Date of the invoice (default sort of the list, descending) |
| Customer | Customer company |
| Billing / Delivery address | Addresses of the customer |
| Issuer (Tenant / Address / Person) | Issuing company data printed on the invoice |
| Invoice state | Document state (filterable) |
| Invoice type | e.g. invoice, credit note (381), correction (384) |
| Currency / Exchange rate | Invoice currency and optional alternative currency |
| Terms of payment | Payment conditions |
| Due date | Payment due date |
| PriceNet / PriceGross | Net/gross totals of the invoice |
| Reference Invoice | Referenced invoice (for corrections) |
| Comment | Free comment |
| Update name / Update date | Last change |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOInvoice` contains the
complete usage recipe (search → navigate → edit → save) including the C# method names
(`Search`, `NavigateFirst`, `New`, `Save`, `Delete`, `StornoInvoice`, `NewInvoicePos`,
`DeleteInvoicePos`, `AssignContractPos`, `UnAssignContractPos`,
`AssignAlternativeCurrency`, `UpdateExchangeRate`, `ShowDialogOrder`, ...), so an agent
can operate the invoice app remotely. The button captions above are the translations of
these methods (`ACMethod*` attributes, stored in `ACClassText`).
