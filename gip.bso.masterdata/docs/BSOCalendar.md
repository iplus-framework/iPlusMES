# Calendar

> Business object: `BSOCalendar` (Assembly `gip.bso.masterdata`, Module: **Scheduling**)

## Definition

The **Calendar** app manages calendar master data. A calendar record represents a
calendar date and is the base for **calendar holidays** and **calendar shifts**,
which are maintained in their own apps. Calendar records are referenced e.g. by
primary demands for capacity planning.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Calendar** | One record per calendar date |
| **Calendar Holiday** | Holiday definition assigned to a calendar date (own app) |
| **Calendar Shift** | Shift definition assigned to a calendar date (own app) |

## Screen layout

A standard search list on the left and a detail form on the right. There are no
nested tabs.

## Step-by-step tasks

### Creating a new calendar date

1. Click **New** in the toolbar. A new, empty record is created and opened.
2. Check the **Calendar Date** of the new record.
3. Click **Save**.

### Editing a calendar date

1. Search: enter a search string and click **Search**; results appear in the list.
2. Select a record in the list (or use **First record / Previous record / Next record / Last record**).
3. Change the fields.
4. Click **Save** before navigating to another record or creating a new one.

### Deleting a calendar date

1. Select the record.
2. Click **Delete**. The delete fails with a message if holidays, shifts or demands
   still reference the record.
3. Click **Save** to execute the delete in the database.

### Other toolbar functions

* **Undo** — discards all unsaved changes.
* **Load** — re-reads the selected record if another user changed it in the background.

## Field reference

| Field | Type | Description |
|---|---|---|
| Calendar Date | Date | The calendar date of the record |
| Content | Text | Free configuration/comment (XML) |
| Created/Updated by, dates | Text/Date | System information |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOCalendar` contains the
complete usage recipe (search → `CalendarList` → `NavigateFirst()` → `CurrentCalendar`
→ edit → `Save()`; `New()`, `Delete()`, `Load()`), so an agent can operate the
Calendar app remotely, e.g. for user training.
