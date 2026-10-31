# Printer Settings MES

> Business object: `BSOPrinterSettingsMES` (Assembly `gip.bso.masterdata`)
> Extends `BSOPrinterSettings` (Assembly `gip.core.reporthandler`).

> ⚠️ **Status: deactivated.** The whole class is currently commented out in the source code
> (`Facility/BSOPrinterSettingsMES.cs`), so the app is **not registered** in the iPlus metadata,
> has no menu entry and cannot be started or automated. This page documents the intended
> behavior as found in the code.

## Definition

**Printer Settings MES** is a variant of the generic **Printer Settings** app for the MES
context. While the generic app assigns printers to machines and users, this variant additionally
assigns printers to **facilities / storage locations**: a printer is addressed via the storage
location (ACUrl) or directly via the facility, so that labels and documents are printed at the
printer of the warehouse area where the material is located.

## Concepts

| Term | Meaning |
|---|---|
| **Configured printer** | A printer assignment: system printer or printer component + target (machine, facility, location ACUrl or user) |
| **Windows printer** | A printer of the operating system (selected by name) |
| **Printer component / Print server** | An iPlus printer component addressed via its ACUrl |
| **Facility tree** | Tree of storage areas/bins used to pick the print target location |
| **Location name (ACUrl)** | Alternative target: a component URL instead of a facility |

## Screen layout

The app is a docking layout with two tabs:

| Tab | Content |
|---|---|
| **PrintMES** | Selection of the print target and the printer, with the **Add printer** button |
| **Explorer** | List of the configured printers with the **Remove printer** button |

The **PrintMES** tab contains:

| Area | Content |
|---|---|
| **Facility** (tree) | Facility tree to choose the print target location; selecting a facility fills the location name |
| **Machine** (list) | Machines as alternative print target; selecting a machine clears the facility selection |
| **Location name** | Free ACUrl of the target location (alternative to the facility tree) |
| **User** (list) | Users as additional assignment target, with **Deselect user** |
| **Windows printer** (list) | System printers to choose from |
| **Printer component** (list) | Printer components (print servers) with name and ACUrl |
| **Add printer** | Creates the printer assignment from the current selection |

The **Explorer** tab lists the configured printers (printer name, printer ACUrl, facility no.,
machine ACUrl, user, default flag) with the **Remove printer** button.

## Usage (step by step) — intended behavior

1. On the **PrintMES** tab choose the print target: a **facility** in the facility tree, a
   **machine** in the machine list, or a **location name** (ACUrl). Optionally select a **user**.
2. Choose the printer: a **Windows printer** or a **printer component**.
3. Click **Add printer** — the assignment is created and appears in the **Explorer** tab.
   The button is only enabled if a target and a printer are selected and this exact assignment
   does not exist yet.
4. On the **Explorer** tab select an assignment and click **Remove printer** to delete it.

## For AI agents (MCP)

This BSO is **currently deactivated** (class commented out) and therefore **not exposed** through
the iPlus MCP Server — `get_type_infos` will not return it. The generic
`BSOPrinterSettings` (gip.core.reporthandler) remains available. If the class is reactivated,
its metadata will provide: `CurrentFacilityRoot`/`CurrentFacility` (facility tree),
`SelectedFacility`, `LocationName`, the inherited printer selections
(`SelectedWindowsPrinter`, `SelectedPrintServer`, `SelectedVBUser`,
`SelectedConfiguredPrinter`) and the commands **Add printer** (`AddPrinter`),
**Remove printer** (`RemovePrinter`) and **Deselect user** (`DeselectVBUser`).
