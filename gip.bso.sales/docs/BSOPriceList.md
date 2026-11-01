# Price List

> Business object: `BSOPriceList` (Assembly `gip.bso.sales`, Menu: **Sales → Price list**)

## Definition

A **Price list** defines the sales price of materials for a validity period
(**Date from** / **Date to**) and a **currency**. Price lists are used to take over
material prices into offers and invoices.

Only **one price list per material and overlapping validity period** is allowed: when
saving, the system checks that no other price list with an overlapping period contains
the same materials.

## Concepts

| Term | Meaning |
|---|---|
| **Price list** | The header: number, name, currency, validity period, comment |
| **Price list position** | One material with its price |
| **Validity period** | Date from / Date to — the list is only valid in this range |
| **Material explorer** | Embedded search dialog for selecting a material on the *MaterialAssign* tab |

## Screen layout

The app is a search list on the left and a detail form on the right.

**Search list:** price list number, date from, date to, comment, update name/date.

**Detail form:**

* **Pricelist** expander (header): number, name, currency, date from/to, comment.
* **PriceListPos** (left): the position list (material no., material name, price) with
  **New Position** and **Delete Position** buttons.
* **PriceListEdit** (right) with two tabs:
  * **MaterialPrice** — material no./name of the selected position, a material combo
    box and the **Price** field.
  * **MaterialAssign** — embedded **material explorer**, the **Price** input field and
    the **New Position** (assign) button.

## Creating a new price list (step by step)

1. Click **New**. A new price list with a new number is created and opened.
2. Enter the **name**, select the **currency** and set the validity period
   (**Date from** / **Date to**).
3. Optionally enter a **comment**.
4. Click **Save**. If a warning message appears, confirm with **Yes**.

## Adding material prices

**Variant 1 — via the position list:**

1. Click **New Position** — a new, empty position is created and selected.
2. On the **MaterialPrice** tab select the **material** in the combo box.
3. Enter the **Price**.
4. Click **Save** (confirm warnings with **Yes**).

**Variant 2 — via the material explorer (MaterialAssign tab):**

1. Search and select the material in the **material explorer**.
2. Enter the **Price** (must be greater than zero).
3. Click **New Position** — the position is created with material and price in one
   step. The button is only active if the material is not yet in the price list.
4. Click **Save** (confirm warnings with **Yes**).

**Delete Position** removes the selected position.

## Validation on save

* **Date to must be after date from** — otherwise an error message is shown.
* **No overlapping price lists with the same materials** — if another price list with
  an overlapping validity period contains one of the same materials, the save is
  rejected with an error naming the conflicting price list and material numbers.

## Toolbar functions

* **Save** — saves the price list with all positions (with the validations above).
* **Undo** — discards all unsaved changes.
* **Load** — re-reads the selected price list with its positions if another user
  changed it in the background.
* **Delete** — deletes the selected price list with all positions and selects the
  first remaining price list.
* **Search** — refreshes the price list list.

## Field reference (header)

| Field | Description |
|---|---|
| PriceListNo | Unique number of the price list |
| PriceListName | Name of the price list |
| Currency | Currency of the prices |
| Date from / Date to | Validity period (date to must be after date from) |
| Comment | Free comment |
| Update name / Update date | Last change |

## Field reference (position)

| Field | Description |
|---|---|
| Material | Material of the position (via combo box or material explorer) |
| Price | Sales price of the material in the list currency |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOPriceList` contains the
complete usage recipe (search → navigate → edit → save) including the C# method names
(`Search`, `NavigateFirst`, `New`, `Save`, `UndoSave`, `Load`, `Delete`,
`NewPriceListMaterial`, `AssignMaterialToPriceList`, `DeletePriceListMaterial`), so an
agent can operate the price list app remotely. The button captions above are the
translations of these methods (`ACMethod*` attributes, stored in `ACClassText`).
