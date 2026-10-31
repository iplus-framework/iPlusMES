# Shift Models (MDBSOTimeRange)

> Business object: `MDBSOTimeRange` (Assembly `gip.bso.masterdata`, Menu: **Scheduling → Shift**)

## Definition

A **shift model** is a master-data container that groups **shift times** (from/to
entries). Shift models are used by scheduling to define when production resources
operate. Technically both the model and its shift times are stored as `MDTimeRange`
records (`IsShiftModel = true` for models).

## Screen layout

Standard explorer layout: search list on the left, detail form on the right.
The detail form shows the shift model header and the list of its shift times.

## Working with shift models

1. **Search**: enter a search string; results appear in the shift model list.
2. **New**: creates a new shift model; fill in number and name.
3. **Edit**: change fields on the current record, then **Save** before navigating
   to the next record or creating a new one.
4. **Delete**: removes the current shift model (only if no other data references it);
   call **Save** afterwards to execute the delete.
5. **Undo**: discards unsaved changes.
6. **Load**: refreshes the current record if another user changed it in the background.

## Working with shift times

1. Select the shift model.
2. **New Shift** adds a new shift time entry to the model.
3. Enter the **from/to** times.
4. **Save** to persist. **Delete Shift** removes the selected shift time.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `MDBSOTimeRange` contains the
complete usage recipe including the C# method names (`New`, `Save`, `Delete`, `Load`,
`NewTimeRange`, `DeleteTimeRange`), so an agent can operate the app remotely, e.g. for
user training. Button captions: Save, Undo, Load, New, Delete, Search, New Shift,
Delete Shift.
