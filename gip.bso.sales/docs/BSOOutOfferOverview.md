# Offer Overview (Angebotsübersicht)

> Business object: `BSOOutOfferOverview` (Assembly `gip.bso.sales`, Menu: **Sales → Offer Overview**)

## Definition

The **Offer Overview** app shows customer offers (**OutOffer**) together with their
offer positions for viewing and editing.

## Concepts

| Term | Meaning |
|---|---|
| **Offer** | A quotation to a customer (header record) |
| **Offer position** | A line of the offer with material and quantity |

## Screen layout

The screen shows the offer list; the current offer is displayed in the detail area.
Loading an offer also loads its offer positions.

## Step-by-step tasks

### Searching offers

1. Enter a search string in the search field.
2. Click **Search** — the result appears in the offer list.

### Working with records

* **Select**: click a row in the list to select an offer.
* **Edit**: change fields on the current offer, then click **Save** before navigating
  to the next record.
* **Undo**: click **Undo** to discard unsaved changes.
* **Load**: click **Load** to re-read the selected offer **including its offer
  positions** if another user changed it in the background.

## Field reference

The visible columns follow the query definition of the offer (offer number, customer,
dates, positions — depending on the query layout).

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOOutOfferOverview` contains
the complete usage recipe (search → navigate → edit → save) with the C# method names
(`Search`, `NavigateFirst`, `Save`, `UndoSave`, `Load`), so an agent can operate the
app remotely, e.g. for user training. The button captions above are the translations
of these methods (`ACMethod*` attributes, stored in `ACClassText`).
