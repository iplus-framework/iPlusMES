# Effort Recording (Aufwandserfassung)

> Business object: `BSOOutOrderPosUtilization` (Assembly `gip.bso.sales`, Menu: **Sales → Outorderposutilization**)

## Definition

The **Effort Recording** app is used to record efforts/utilizations
(**OutOrderPosUtilization**) on order positions — e.g. work time or resources spent
for an order position.

## Concepts

| Term | Meaning |
|---|---|
| **Utilization record** | One recorded effort with its own number |
| **Order position** | The order position the effort belongs to |

## Screen layout

The main layout contains the utilization form tab and the **Explorer** (search list)
docked at the bottom (auto-hide).

## Creating a new utilization record (step by step)

1. Click **New** — a new utilization record with a new number is created and opened.
2. Fill in the required fields (order position, effort data).
3. Save your entries (see note below).

## Working with records

* **Search**: enter a search string and click **Search**; results appear in the list.
* **Edit**: change fields on the current record.
* **Delete**: click **Delete** to remove the current utilization record — only
  possible if no foreign key relationships exist from other tables.
* **Load**: click **Load** to re-read the current record if another user changed it
  in the background.

> **Note:** This app has no separate **Save** button. New records are added to the
> database context by **New**; persisting/undo behaviour follows the base framework
> (see the class summary for details).

## Field reference

The visible columns follow the query definition of the utilization record
(number, order position, effort data — depending on the query layout).

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOOutOrderPosUtilization`
contains the complete usage recipe with the C# method names (`Search`,
`NavigateFirst`, `New`, `Load`, `Delete`), so an agent can operate the app remotely,
e.g. for user training. The button captions above are the translations of these
methods (`ACMethod*` attributes, stored in `ACClassText`).
