# Reservation Overview

> Business object: `BSOFacilityReservationOverview` (Assembly `gip.bso.masterdata`)
> This app is **not opened directly from the menu**. It is embedded as a child window in the
> **Material** app (stock/lot details) and is operated from there.

## Definition

The Reservation Overview shows **who has reserved the stock** of a facility charge, a
facility lot or a material: which pickings and which production orders hold reservations.

The overview is **read-only** — reservations are created and changed in the order apps
(see *Reservations*), not here.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Picking Reservation** | Reservation made by a picking position; shows the picking number |
| **ProdOrder Reservation** | Reservation made by a production order position; shows the program number |
| **Document No** | Number of the document holding the reservation (picking no. resp. program no.) |

## Screen layout

Two lists side by side (or as tabs, depending on the host layout):

| List | Content |
|---|---|
| **Picking Reservations** | All reservations of picking positions, with document no., material, assigned/total reserved/used/free quantity |
| **ProdOrder Reservations** | All reservations of production order positions, with document no., material, assigned/total reserved/used/free quantity |

## Working with the overview

1. Open the stock or lot details in the **Material** app. The host loads the reservations
   of the selected charge, lot or material (optionally limited to a period) into the two
   lists.
2. Select a row in one of the lists.
3. Click **Show Picking** to open the picking of the selected picking reservation, or
   **Show ProdOrder** to open the production order of the selected production order
   reservation. The buttons are only enabled if the selected row has a valid document
   number.
4. The lists are refreshed by the host app; **Clear** (host) empties both lists when no
   charge/lot is selected.

## Field reference

| Field | Description |
|---|---|
| Document No | Picking number resp. production program number of the reserving document |
| Material | Material of the reservation |
| Assigned Quantity | Quantity assigned to this reservation |
| Total Reserved Quantity | Total quantity reserved by the reservation |
| Used Quantity | Quantity already consumed from the reservation |
| Free Quantity | Quantity still available under this reservation |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityReservationOverview`
contains the complete usage recipe. Important: this is a **read-only child BSO** — the
hosting BSO (e.g. `BSOMaterialDetails` via `BSOFacilityReservationOverview_Child`) calls
`LoadReservation(FacilityCharge)`, `LoadReservation(FacilityLot)`,
`LoadReservation(Material, startTime, endTime)` or `ClearReservation()`; results are split
into `PickingReservationList` and `ProdOrderReservationList`. Navigation methods:
`ReservationNavigateToPicking` ("Show Picking"),
`ReservationNavigateToProdOrder` ("Show ProdOrder"). The button captions above are the
translations of these methods (`ACMethod*` attributes, stored in `ACClassText`).
