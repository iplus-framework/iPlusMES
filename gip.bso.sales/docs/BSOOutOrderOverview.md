# Order Summary (Auftragsübersicht)

> Business object: `BSOOutOrderOverview` (Assembly `gip.bso.sales`, Menu: **Sales → Order Summary**)

## Definition

The **Order Summary** app shows an overview of order positions (**OutOrderPos**) with
their order, customer, material and delivery data.

## Concepts

| Term | Meaning |
|---|---|
| **Order position** | A position of a customer order |
| **Target delivery date / priority** | When and how urgently the position shall be delivered |
| **Target / Actual quantity and weight** | Planned vs. produced/delivered amounts |

## Screen layout

The screen is a single grid titled **Orderpositions** showing the order positions with
these columns (read-only columns marked *):

| Column | Editable |
|---|---|
| Order no. * | no |
| Customer * | no |
| Sequence * | no |
| Material / Material name * | no |
| Target delivery date | yes |
| Target delivery priority | yes |
| Target quantity / weight * | no |
| Actual quantity / weight * | no |
| Quantity unit * | no |

## Step-by-step tasks

### Searching order positions

1. Enter a search string in the search field.
2. Click **Search** — the result appears in the grid.

### Working with records

* **Select**: click a row to select an order position.
* **Edit**: change the editable fields (e.g. **Target delivery date**,
  **Target delivery priority**) on the current record, then click **Save**.
* **Undo**: click **Undo** to discard unsaved changes.
* **Load**: click **Load** to re-read the selected record if another user changed it
  in the background.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOOutOrderOverview` contains
the complete usage recipe (search → navigate → edit → save) with the C# method names
(`Search`, `NavigateFirst`, `Save`, `UndoSave`, `Load`), so an agent can operate the
app remotely, e.g. for user training. The button captions above are the translations
of these methods (`ACMethod*` attributes, stored in `ACClassText`).
