# Company Material Overview

> Business object: `BSOCompanyMaterialOverview` (Assembly `gip.bso.masterdata`,
> embedded in the Company app, tab **Material Overview**)

## Definition

The **Material Overview** shows which materials belong to a company and which stock is
currently available for them. It is a **read-only** overview: there is no editing,
saving or deleting here. Changes are made in the Material app or in the
**Company Material** tab of the Company app (see
[BSOCompany](BSOCompany.md)).

## Concepts

| Term | Meaning |
|---|---|
| **Company material** | A material in the context of a company, with its own company material number and name |
| **Supplier stock** | Stock of the company's own company materials, taken from the facility charges (lot-managed stock) |
| **Partner stock** | Stock that the company holds for you: for lot-managed materials from the facility charges of the partner company materials, for materials without lot management from the stock records |
| **Unique materials** | All company materials of the company with their current material stock |
| **Material details** | Detail view of the material of the selected row (embedded child app) |

## Screen layout

| Area | Content |
|---|---|
| **View type** (radio buttons at the top) | Selection of the overview type: **View Supplier stock**, **View Partner stock**, **View Partner stock of unique Materials** |
| **Overview list** | Columns: sequence number, **Material No**, **Material Name**, **Company Material No**, **Company Material Name** |
| **Material details** (right/below) | Details and stock of the material of the selected row |

## Usage (step by step)

1. Open the Company app and load the desired company — the Material Overview tab is
   refreshed automatically for the current company.
2. Switch to the **Material Overview** tab.
3. Select the desired view type with the radio buttons:
   * **View Supplier stock** — stock of the company's own company materials from the
     facility charges.
   * **View Partner stock** — stock the partner company holds (lot-managed and
     not lot-managed materials combined, ordered by material number).
   * **View Partner stock of unique Materials** — all company materials of the company
     with their current material stock.
   The list is rebuilt immediately when the view type changes.
4. Select a row in the list — the **material details** show the material and its
   current stock.

There are no New/Save/Delete functions in this overview; it only displays data.

## Field reference (list columns)

| Column | Description |
|---|---|
| Sn | Sequence number of the row |
| Material No | Number of the material |
| Material Name | Name of the material |
| Company Material No | Company-specific material number |
| Company Material Name | Company-specific material name |

The current material stock of each material is passed to the material details view.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOCompanyMaterialOverview`
contains the complete usage recipe. The BSO is operated via `CurrentCompany` (set by
the parent `BSOCompany`), the view type (`FilterCompanyMaterialOverview` /
`SelectedFilterCompanyMaterialOverviewType`), `Search()` to fill
`MaterialOverviewList`, `SelectedMaterialOverview` to push the selected material to the
`BSOMaterialDetails_Child`, and `Clear()`. It is read-only — there are no
New/Save/Delete methods.
