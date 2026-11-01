# Tour Management (Tour Planning Board)

> Business object: `BSOTourManagement` (Assembly `gip.bso.logistics`, Menu: **Logistics → Tourmanagement**)

## Definition

The **Tour Management** app is a planning overview (tour planning board) for tour plans
(**Tourplans**) within a selectable week. It shows which tours are planned in the displayed date
range.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Tourplan** | A planned vehicle tour with tour positions (delivery notes, pickings) |
| **From date / To date** | The displayed date range (week) |
| **Tour planning board** | The graphical overview of the tours in the date range |

## Screen layout

* Date range selection with **From date** and **To date** and the navigation buttons **-**
  (previous week) and **+** (next week).
* The tour plans of the date range are displayed in the connected tour planning board design.

## Step-by-step tasks

### Change the displayed week

1. Click **-** to move the date range one week back, or **+** to move it one week forward.
2. The board shows the tour plans of the new date range.

### Edit a tour plan

Detailed editing of tour plans and their positions is done in the **tour plan dialog** — open it
from the Visitor Voucher (**Show Tourplan**) or the tour plan app. This overview app is read-only
for planning purposes.

## Field reference

| Field | Description |
|---|---|
| From date | Start of the displayed date range |
| To date | End of the displayed date range |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_type_infos`, `get_method_info`, `execute_acurl_command`). The class-level `<summary>` of
`BSOTourManagement` contains the usage recipe. Relevant members: `CurrentFromDate`,
`CurrentToDate`, `MovePrevWeek()`, `MoveNextWeek()`. Note that `MovePrevWeek`/`MoveNextWeek` are
currently placeholders without implementation.
