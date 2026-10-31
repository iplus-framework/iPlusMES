# Facility Reservation

> Business object: `BSOReservFacility` (Assembly `gip.bso.masterdata`, Module: **Facility / Logistics**)

## Definition

The **Facility Reservation** app shows the **reservations of a storage bin**:
which materials are reserved on this bin and in which quantities.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Reservation** | A reserved quantity of a material on a storage bin |
| **Storage bin filter** | The storage bin whose reservations are searched |

The app is **read-only** — reservations are created and changed by the system
(e.g. by orders), not in this app.

## Screen layout

A search area with the storage bin filter and a **Search** button, and a result
list of the reservations. (The balance tab design is currently a placeholder.)

## Step-by-step tasks

### Searching reservations of a storage bin

1. Select the **storage bin** in the filter.
2. Click **Search** — the reservation list shows all reservations for this bin.
   If the result contains reservations of exactly one storage bin, that bin is
   shown as the current facility.

## Field reference

| Field | Description |
|---|---|
| Storage bin filter | Storage bin whose reservations are shown |
| Reservation list | All reservations found for the storage bin |
| Current facility | Storage bin of the result (only if the result is limited to one bin) |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOReservFacility`
contains the complete usage recipe: set the filter on
`CurrentFacilityReservationData` (`FacilityFilter`) and call `Search()`; the
result is in `CurrentFacReservationList`, the single-bin result in
`CurrentFacility`. `New()` only sets the state — reservation creation is not
implemented. The BSO is otherwise read-only.
