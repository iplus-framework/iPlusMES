# OEE Dashboard (Facility OEE View)

> Business object: `BSOFacilityOEEView` (Assembly `gip.bso.manufacturing`, Menu: **Manufacturing → OEE Dashboard / OEE Übersichtsseite**)

## Definition

The **OEE Dashboard** shows the **Overall Equipment Effectiveness** of a machine or
storage facility. For the selected facility it displays the averages of
**Availability**, **Performance**, **Quality** and **Total OEE** for three freely
definable evaluation periods, both as key figures and as a graph.

## Concepts

| Term | Meaning |
|---|---|
| **OEE** | Overall Equipment Effectiveness = Availability × Performance × Quality |
| **Facility** | Machine or inventory facility (only facilities of type *Machine or Inventory* are listed) |
| **Period** | One of three evaluation periods (From/To), each with its own OEE average |
| **Availability details** | Property log of the machine module for a period — the source data of the availability |

## Screen layout

The app consists of a facility list (Explorer, docked right) and the dashboard page:

| Area | Content |
|---|---|
| **Explorer** | Facility list with columns **Facility No.** and **Facility Name** |
| **Header** | Facility No. and Facility Name of the current facility |
| **Period 1 / Period 2 / Period 3** (expanders) | From / To date pickers, **Availability details** and **Refresh OEE** buttons, OEE key figure control and OEE graph |

## Step-by-step tasks

### Selecting a facility

1. Enter a search string in the search field of the facility list (searches Facility No.
   and Facility Name) and start the search.
2. Select a facility — the three periods are **set automatically** from the most recent
   OEE entries of the facility (e.g. last day, last week, last month) and the averages
   are refreshed.

### Adjusting the evaluation periods

1. Open the expander of the desired period (**Period 1**, **Period 2** or **Period 3**).
2. Change **From** and/or **To**.
3. Click **Refresh OEE** — the OEE averages of all three periods are recalculated.

### Viewing availability details

1. Select a facility and make sure the period is set.
2. Click **Availability details** in the desired period — the property log viewer of the
   machine module opens for this period.

## Field reference

| Field | Description |
|---|---|
| Facility No. / Facility Name | Identification of the machine or storage facility |
| From / To (per period) | Evaluation period of the OEE average |
| OEE Period 1/2/3 | Average values: Availability OEE, Performance OEE, Quality OEE, Total OEE |
| OEE graph | Graphical course of the OEE values within the period |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityOEEView` contains
the complete usage recipe (search → `CurrentFacility` → automatic periods →
`RefreshOEEAvgs()`, `ShowAvailablityDetails1/2/3()`), so an agent can operate the
dashboard remotely, e.g. for user training. The button captions above are the
translations of the `ACMethod*` attributes (stored in `ACClassText`).
