# Facility Location Reservation

> Business object: `BSOReservFacilityLocation` (Assembly `gip.bso.masterdata`, Module: **Facility / Logistics**)

## Definition

The **Facility Location Reservation** app shows the **reservations of a storage
location**: which materials are reserved within this storage location and in
which quantities.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Reservation** | A reserved quantity of a material on a storage bin |
| **Storage location filter** | The storage location whose reservations are searched |

The app is **read-only** — reservations are created and changed by the system
(e.g. by orders), not in this app.

## Screen layout

A search area with the storage location filter and a **Search** button, and a
result list of the reservations. (The reservations tab design is currently a
placeholder.)

## Step-by-step tasks

### Searching reservations of a storage location

1. Select the **storage location** in the filter.
2. Click **Search** — the reservation list shows all reservations within this
   storage location. If the result contains reservations of exactly one storage
   bin, that bin is shown as the current facility.

## Field reference

| Field | Description |
|---|---|
| Storage location filter | Storage location whose reservations are shown |
| Reservation list | All reservations found for the storage location |
| Current facility | Storage bin of the result (only if the result is limited to one bin) |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of
`BSOReservFacilityLocation` contains the complete usage recipe: set the filter
on `CurrentFacilityReservationData` (`FacilityLocationFilter`) and call
`Search()`; the result is in `CurrentFacReservationList`, the single-bin result
in `CurrentFacility`. `New()` only sets the state — reservation creation is not
implemented. The BSO is otherwise read-only.
