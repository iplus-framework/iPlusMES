# Delivery Proposal (Liefervorschlag)

> Business object: `BSOOutDelivProposal` (Assembly `gip.bso.sales`, Menu: **Sales → Outdeliveryproposal**)

## Definition

The **Delivery Proposal** app prepares out-deliveries to customers. It shows a list of
order positions that are candidates for delivery, from which one or more positions can
be selected for further processing.

## Concepts

| Term | Meaning |
|---|---|
| **Order position** | A position of a customer order that can be delivered |
| **Delivery proposal** | The list of order positions prepared for delivery |

## Screen layout

The screen is a search list (Explorer) with the order positions grid. The Explorer is
docked at the bottom of the main layout and can be auto-hidden.

## Step-by-step tasks

### Searching order positions

1. Enter a search string in the search field.
2. Click **Search** — the result appears in the order position list.

### Working with records

* **Select**: click a row in the list to select an order position.
* **Edit**: change fields on the current record, then click **Save** before navigating
  to the next record.
* **Undo**: click **Undo** to discard unsaved changes.
* **Load**: click **Load** to re-read the selected record if another user changed it
  in the background.

## Field reference

The grid shows the order position columns as configured in the query definition
(order, customer, material, quantities, delivery data — depending on the query layout).

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOOutDelivProposal` contains
the complete usage recipe (search → navigate → edit → save) with the C# method names
(`Search`, `NavigateFirst`, `Save`, `UndoSave`, `Load`), so an agent can operate the
app remotely, e.g. for user training. The button captions above are the translations
of these methods (`ACMethod*` attributes, stored in `ACClassText`).
