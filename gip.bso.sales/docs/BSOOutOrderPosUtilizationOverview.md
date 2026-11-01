# Utilization Analysis (Aufwandsauswertung)

> Business object: `BSOOutOrderPosUtilizationOverview` (Assembly `gip.bso.sales`, Menu: **Sales → Utilizationanalysis**)

## Definition

The **Utilization Analysis** app shows an overview of recorded efforts/utilizations
(**OutOrderPosUtilization**) for evaluation purposes.

## Concepts

| Term | Meaning |
|---|---|
| **Utilization record** | One recorded effort on an order position |
| **Analysis** | The list view of all recorded efforts |

## Screen layout

The screen is a single grid titled **Orderpositions** showing the utilization records
in a list.

## Step-by-step tasks

### Searching utilization records

1. Enter a search string in the search field.
2. Click **Search** — the result appears in the grid.

### Working with records

* **Select**: click a row to select a utilization record.
* **Edit**: change fields on the current record, then click **Save** before navigating
  to the next record.
* **Undo**: click **Undo** to discard unsaved changes.
* **Load**: click **Load** to re-read the selected record if another user changed it
  in the background.

## Field reference

The visible columns follow the query definition of the utilization record
(number, order position, effort data — depending on the query layout).

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of
`BSOOutOrderPosUtilizationOverview` contains the complete usage recipe
(search → navigate → edit → save) with the C# method names (`Search`,
`NavigateFirst`, `Save`, `UndoSave`, `Load`), so an agent can operate the app
remotely, e.g. for user training. The button captions above are the translations of
these methods (`ACMethod*` attributes, stored in `ACClassText`).
