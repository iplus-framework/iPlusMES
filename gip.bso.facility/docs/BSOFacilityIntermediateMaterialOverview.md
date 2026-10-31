# Intermediate Material Overview

> Business object: `BSOFacilityIntermediateMaterialOverview` (Assembly `gip.bso.facility`, Menu: **Warehouse Management → Intermediate Material Overview**)

## Definition

This app displays a **read-only overview of intermediate materials in stock**.

For every intermediate material (a target material of a material workflow relation that
is not itself a source material) the overview shows the current stock quantity and the
inward/outward booking quantities of the current day, together with the parent material
of the partslist in which the intermediate is produced.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Intermediate material** | Material that is produced in one production stage and consumed in a later stage |
| **Parent material (VMaterial)** | Material of the partslist in which the intermediate is used/produced |
| **Stock quantity** | Current stock of the intermediate (sum of all storage charges) |
| **Day inward / Day outward** | Sum of the inward/outward bookings of the current day |
| **Sn** | Running row number |

## Screen layout

The screen is a single data grid (the **Explorer** design embedded in the main layout).
There are no tabs, no filter fields and no detail form.

| Column | Description |
|---|---|
| Sn | Row number |
| Material number | Number of the intermediate material |
| Material name | Name of the intermediate material |
| VMaterial number | Number of the parent material (partslist material) |
| VMaterial name | Name of the parent material |
| Stock quantity | Current stock quantity |
| Day inward | Inward bookings of the current day |
| Day outward | Outward bookings of the current day |

The grid supports column summing (sum display is enabled).

## Step-by-step tasks

### Viewing the intermediate material stock

1. Open the app. The list is loaded automatically on start and the first row is selected.
2. Click **Search** (toolbar) to rebuild the list from the database.
3. Click **Load** (toolbar) to refresh; the search is re-executed when the reload
   option is set.

### Notes

* The overview is read-only — there is no **New**, **Save** or **Delete**.
* Storage bins whose facility number is excluded by the configuration are not
  included in the totals.
* The day columns refer to the current calendar day (from midnight to midnight).

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of
`BSOFacilityIntermediateMaterialOverview` contains the complete usage recipe.
Relevant C# methods: `Search` (rebuilds `IntermediateMaterialOverviewList` and selects
the first row into `SelectedIntermediateMaterialOverview`) and `Load(requery)`
(re-executes the search when `requery` is true). The list is built by a precompiled
query over `MaterialWFRelation`, `Partslist` and `FacilityCharge`; the virtual
`ExcludedFacilityBins` property defines the excluded bin number.
