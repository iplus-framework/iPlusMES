# Warehouse overview by Barcode

> Business object: `BSOBarcodeFacilityOverview` (Assembly `gip.bso.facility`, registered in the **Businessobject-Manager**)

## Definition

**Warehouse overview by Barcode** (*Lagerübersicht per Barcode*) is intended to show the
warehouse stock by scanning/entering a barcode.

## Current status

This business object is currently an **empty placeholder**:

* It derives only from the BSO base class and defines **no properties, no methods,
  no queries and no screen designs**.
* There is no user interface and no workflow — opening it shows nothing usable.

Until the functionality is implemented, use one of the following apps instead:

| App | Purpose |
|---|---|
| **Facility Overview** (`BSOFacilityOverview`) | Warehouse stock overview per storage bin |
| **Charge Overview** (`BSOFacilityChargeOverview`) | Stock of charges, with Fast View tracking |
| **Lot Overview** (`BSOFacilityLotOverview`) | Stock of lots, with Fast View tracking |

## For AI agents (MCP)

The class-level `<summary>` of `BSOBarcodeFacilityOverview` documents the current state:
the class is a stub with no operable members. There are no `ACProperty*`/`ACMethod*`
attributes to call via the iPlus MCP Server, and no designs exist in `ACClassDesign`.
Do not attempt to automate this business object.
