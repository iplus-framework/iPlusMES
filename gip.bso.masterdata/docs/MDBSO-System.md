# System Master Data — Value Lists (MDBSO)

> Business objects: `MDBSO*` (Assembly `gip.bso.masterdata`, Module **System**)

## What are master-data value lists?

Value lists are small, centrally maintained tables whose entries appear as
selections (combo boxes, status columns) in the operational screens of the
system — for example the country of a business partner, the currency of a
price or the terms of payment of an invoice. Each entry has a **multilingual
name**, a **sort index** and an optional **default flag**; some lists also
carry a technical **key** or **code** used by the system.

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

> **Caution:** entries are referenced by operational records (companies,
> prices, invoices …). Deleting an entry that is still in use is rejected by
> the system — retire it instead of deleting it if possible.

---

## Cost Center (`MDBSOCostCenter`)

Cost centers are organizational units used to allocate internal costs
(e.g. to evaluate revenues and expenses per department).

**Fields:** Cost center number, Name, Is enabled, Is default.

**Values in the sample database:** none — the list is empty and can be filled
by the customer.

## Country (`MDBSOCountry`)

Maintains the **countries** of business partners and addresses. A country
entry consists of the **country code**, a **name**, the **EU membership**
flag and an assignment to a **default currency**.

**Special functions:**

* **New Federal State** — adds a federal state (with code and name) to the
  current country.
* **Delete Federal State** — removes the selected federal state.

**Fields:** Country code, Name, Is EU member, Currency, Is default; federal
states with code and name.

**Values in the sample database:**

| Code | Name | EU member | Currency |
|---|---|---|---|
| 049 DE *(default)* | Germany | Yes | EUR |
| 033 FR | France | Yes | EUR |
| 031 NL | Netherlands | Yes | EUR |
| 032 BE | Belgium | Yes | EUR |
| 048 PL | Poland | Yes | EUR |
| 046 SE | Sweden | Yes | EUR |
| 045 DK | Denmark | Yes | EUR |
| 041 CH | Switzerland | Yes | EUR |
| 044 GB | United Kingdom | Yes | EUR |
| 044 GB2 | United Kingdom (2nd code) | Yes | EUR |

For **Germany (049)** the federal states are maintained (Baden-Württemberg,
Bayern, Berlin, Brandenburg, Bremen, Hamburg, Hessen, Niedersachsen,
Nordrhein-Westfalen, Rheinland-Pfalz, Saarland, Sachsen,
Sachsen-Anhalt, Schleswig-Holstein, …).

## Currency (`MDBSOCurrency`)

Maintains the **currencies** used for prices and invoices, together with
their **exchange rates** to other currencies.

**Special functions:**

* **New Exchange rate** — opens a dialog to enter a new exchange rate for
  the current currency (target currency and rate); confirm with **OK**,
  abort with **Cancel**.
* **Delete Exchange rate** — removes the selected exchange rate.

**Fields:** Short name (e.g. `EUR`), Name, Is default; exchange rates with
target currency and rate.

**Values in the sample database:**

| Short name | Name | Default |
|---|---|---|
| EUR | Euro | Yes |
| USD | US Dollar | No |

Exchange rates: **EUR → USD** with rate **0.9**.

## Complaint Type (`MDBSORatingComplaintType`)

Complaint types classify **customer complaints** (for ratings and quality
management) and are offered as selections in the complaint screens.

**Fields:** Key, Name, Sort index.

**Values in the sample database:** none — the list is empty and can be filled
by the customer.

## Terms of Payment (`MDBSOTermOfPayment`)

Terms of payment define the **payment target in days** and optional
**discount conditions** (skonto) offered to customers.

**Fields:** Name, Description, Payment days, Is default.

**Values in the sample database:**

| Name | Payment days |
|---|---|
| Immediately (8 Days Net) | 8 |
| Immediately, 3 % Skonto | 5 |
| 10 Days Net | 10 |
| 10 Days 1.0 % Discount, 30 Days Net | 11 |
| 15 Days Net | 15 |
| 20 Days Net | 20 |
| 21 days +1 % Discount | 21 |
| 30 Days Net | 30 |
| 50 Days Net | 50 |
| 60 Days Net | 60 |

---

## For AI agents (MCP)

The metadata of these business objects is exposed through the iPlus MCP
Server (`get_thesaurus`, `get_type_infos`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of each `MDBSO*` class
contains the complete usage recipe (Search → list → `Current<Entity>` →
Save/New/Delete/Load), so an agent can operate the value lists remotely, e.g.
for user training or data maintenance.
