# Source Selection Rules

> Business object: `BSOSourceSelectionRules` (Assembly `gip.bso.masterdata`, Menu: **dialog — opened from the ordering process**)

## Definition

The **Select Sources** dialog shows, for a production order, from which **sources (machines)**
the required materials can be provided, and lets the user **select or deselect** these sources.
The selection is stored as **source selection rules** (excluded process modules) in the
configuration of the workflow nodes.

Like *Preferred Parameters* this app is **not opened from a menu with a search list**. It is a
dialog that the system opens automatically during the ordering process or that other programs
open programmatically.

## Concepts

| Term | Meaning |
|---|---|
| **Source** | A machine that can provide a material for the production order |
| **Rule group** | A group of rule selections belonging to one workflow function; shown with its function caption |
| **Rule selection** | One row: source machine, target, material and a checkbox for selecting the source |
| **All sources** | Machines that do not belong to any rule group; selected individually per machine |
| **Reservation** | Sources that provide the material from stock reservations |
| **Not dosable material** | A material of the order that cannot be dosed; the dialog highlights this tab in red when such materials exist |

## Screen layout

The dialog has four tabs and two buttons at the bottom:

| Tab | Content |
|---|---|
| **Possible sources** (`tabPossibleSources`) | Rule groups (red function caption) with a grid of their rule selections: **Source**, **Target**, **Material number**, **Name** and a selection checkbox |
| **All sources** (`tabAllSources`) | Grid of all machines without rule group: **Machine**, **Material number**, **Name** and a selection checkbox |
| **Not dosable materials** (`tabNotDosableMaterials`) | Read-only list of the materials that cannot be dosed (material number and name); the tab caption is shown in red when the list is not empty |
| **Possible sources with reservation** (`tabReservationSources`) | Same layout as *Possible sources*, but for sources with stock reservation |

Buttons: **OK** (saves and closes) and **Cancel/Close** (closes without saving).

## Selecting sources (step by step)

1. Open the dialog (the system does this automatically during the ordering process).
2. On the **Possible sources** tab, tick the checkbox of every source machine that should
   provide the material; untick the checkbox to exclude a source.
3. Repeat on the **Possible sources with reservation** tab for reserved sources.
4. On the **All sources** tab, tick the machines that do not belong to a rule group.
5. Check the **Not dosable materials** tab: if it is highlighted in red, the order contains
   materials that cannot be dosed — resolve this before continuing.
6. Click **OK**. The selection is written to the workflow configuration and saved in the
   database, and the dialog closes.

**Note:** Closing the dialog with **Cancel/Close** discards all changes.

## Field reference

| Field | Description |
|---|---|
| Source | Machine that can provide the material |
| Target | Target of the rule selection (workflow function) |
| Material number | Number of the material provided by the source |
| Name | Name of the material |
| Checkbox | Ticked = source is selected for the order; unticked = source is excluded |
| Machine | Machine (All sources tab) |
| Material number / Name | Material of the machine row (All sources tab) |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOSourceSelectionRules` contains the
complete usage recipe. The C# method names behind the buttons are:

| Button | C# method |
|---|---|
| **OK** | `DlgSelectSourcesOk()` |
| **Cancel/Close** | `CloseTopDialog()` (base class) |

Programmatic opening: `ShowDialogSelectSources(acClassWFID, partslistID, prodOrderPartslistID)`.
The dialog content is exposed as `RuleGroupList`, `ReservationRuleGroupList`, `AllItemsList` and
`NotDosableMaterialsList`; `AnyNotDosableMaterial` is `true` when not dosable materials exist.
The selection is stored per workflow node as *excluded process modules* rules in the current
config store.
