# Company (Company Master Data)

> Business object: `BSOCompany` (Assembly `gip.bso.masterdata`, Menu: **Master Data → Company**)

## Definition

The Company app manages the **company master data**: customers, suppliers, partners and
your own companies. A company record bundles

* general data (company number, name, flags such as *Is Customer*, *Is Shipper*, VAT number, web URL, notes),
* **addresses** with **departments**,
* **contact persons** with their roles,
* **plants** (subordinate companies),
* **material mappings** (the customer/supplier material numbers used for this company),
* **pickup relations** and **stock information** of the company materials,
* a **material overview** showing which materials the company supplies or stocks.

## Concepts

| Term | Meaning |
|---|---|
| **Company** | The master record: number, name, role flags, VAT number, notes |
| **House address** | The main address of the company; created automatically with a new company |
| **Address** | Additional address of the company (delivery, billing, factory address, ...) |
| **Department** | Department within an address |
| **Person** | Contact person of the company with a generated person number |
| **Plant / Factory** | A subordinate company below the current company |
| **Material mapping** | Assignment of a material to the company with its own company material number and min./opt. stock quantities |
| **Pickup** | A pickup relation of the company with a validity period |
| **Material overview** | Read-only overview of the materials and stocks of the company (separate tab, own child app) |

## Screen layout

The app shows a search list (Explorer) and a detail area. The detail area has a header
with **Company No** and **Company Name** and these tabs:

| Tab | Content |
|---|---|
| **Company** | General data: role flags (customer, distributor, shipper, tenant, own company, leads), VAT number, web URL, billing/shipping account numbers, internal and external notes, house address (street, postcode/city, country, phone, fax, mobile, e-mail) |
| **Address** | Address list with the address form (names, street, postcode/city, country, phone/fax/mobile, e-mail, delivery type, billing/delivery/factory flags) and the **departments** of the selected address |
| **Person** | Person list with search field and the person form (person number, name, address data, phone/fax/mobile, e-mail) |
| **Company Material** | Material mappings of the company: material, company material number/name, unit, min./opt. stock quantity, blocked flag; search field to find a company by material number |
| **Pickup** | Pickup relations of the company: material, company material number/name, unit, validity from/to, blocked flag; search field to find a company by pickup number |
| **Company Stock** | Read-only stock list: material, company material, stock quantity, min./opt. quantity, difference, day inward/outward |
| **Rating / Complaint** | Rating and complaint management (embedded sub-design) |
| **Material Overview** | Embedded material overview app (see [BSOCompanyMaterialOverview](BSOCompanyMaterialOverview.md)) |

## Creating a new Company (step by step)

1. Click **New** in the toolbar. A new company with a generated company number and an
   empty house address is created and opened.
2. Enter at least the **Company Name**.
3. Fill the further fields on the **Company** tab (role flags, VAT number, web URL, notes)
   and the house address data.
4. Click **Save**.

## Working with records

* **Search**: enter a search string and click **Search**; the results appear in the
  company list.
* **Edit**: change fields of the current company, then click **Save** before navigating
  to the next record or creating a new one. **Save** is only enabled when something was
  changed and the company name is filled.
* **Load**: re-reads the selected company with all addresses, persons, plants and
  material mappings if another user changed it in the background.
* **Undo**: discards all unsaved changes.
* **Delete**: deletes the current company after confirmation. It fails if other tables
  reference the company. Click **Save** afterwards to execute the delete in the database.

## Address tab

1. Select an address in the address list and click **Load Address** to edit it.
2. Click **New Address** to create a new address for the company, fill the form and
   click **Save**.
3. **Delete Address** removes the current address (then **Save**).
4. In the **Department** section: **New Department** adds a department to the current
   address, **Delete Department** removes it. Click **Save** afterwards.

## Person tab

1. Use the **Search Person** field to filter the persons of the company by person
   number or name.
2. Click **New Person** — a new person with a generated person number is created.
3. Fill name, address and contact data in the person form and click **Save**.
4. **Delete Person** removes the current person (then **Save**).

## Company Material tab

1. Click **New Material** — a new material mapping is created.
2. Assign the **Material**, enter the **Company Material No.** / name, unit and the
   **Min Stock Quantity** / **Opt Stock Quantity**.
3. Click **Save**. **Delete Material** removes the selected mapping.
4. Tip: enter a company material number in the **Find company with material no.** field
   to jump directly to the company that uses this number.

## Pickup tab

1. Click **New pickup** — a new pickup relation is created.
2. Assign the **Material**, enter the company material data and the validity period
   (**Valid from** / **Valid to**).
3. Click **Save**. **Delete pickup** removes the selected relation.
4. The **Find company with pickup no.** field loads the company belonging to a pickup number.

## Company Stock tab

Read-only list of the company materials with their stock quantity, minimum/optional
quantity, the difference to the minimum and the day inward/outward quantities.
Changes are made in the **Company Material** tab or in the Material app.

## Material Overview tab

An embedded, read-only overview app (see
[BSOCompanyMaterialOverview](BSOCompanyMaterialOverview.md)). It is refreshed
automatically when another company is loaded. Choose the view type
(**View Supplier stock**, **View Partner stock**, **View Partner stock of unique
Materials**) and select a row to see the material details.

## Field reference (Company tab)

| Field | Description |
|---|---|
| Company No | Unique number, generated by the system |
| Company Name | Name of the company (required for saving) |
| Is Customer / Is Distributor / Is Shipper / Is Tenant / Is Own Company | Role flags of the company |
| Is Sales Lead / Is Distributor Lead | Lead flags |
| VAT Number | VAT identification number |
| Web URL | Website |
| Billing Account No / Use Billing Account No | Billing account and usage flag |
| Shipping Account No / Use Shipping Account No | Shipping account and usage flag |
| Note Internal / Note External | Internal and external notes |
| House address fields | Name, street, postcode/city, country, phone, fax, mobile, post office box |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOCompany` contains the
complete usage recipe (search → navigate → edit → save) including the C# method names
(`New`, `Save`, `Delete`, `Load`, `Search`, `NewCompanyAddress`,
`NewCompanyAddressDepartment`, `NewCompanyPerson`, `NewFactory`, `NewCompanyMaterial`,
`NewCompanyMaterialPickup`, `FindCompanyWithCMaterialNo`, `FindCompanyWithPickupNo`,
...), so an agent can operate the Company app remotely, e.g. for user training.
The button captions above are the translations of these methods (`ACMethod*`
attributes, stored in `ACClassText`).
