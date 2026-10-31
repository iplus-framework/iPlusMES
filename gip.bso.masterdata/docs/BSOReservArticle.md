# Material Reservation

> Business object: `BSOReservArticle` (Assembly `gip.bso.masterdata`, Module: **Facility / Logistics**)

## Definition

The **Material Reservation** app shows the **reservations of stock for a
material**: which storage bins have reserved which quantity of the material.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Reservation** | A reserved quantity of a material on a storage bin |
| **Material filter** | The material whose reservations are searched |

The app is **read-only** — reservations are created and changed by the system
(e.g. by orders), not in this app.

## Screen layout

A search area with the material filter and a **Search** button, and a result
list of the reservations. (The reservations tab design is currently a
placeholder.)

## Step-by-step tasks

### Searching reservations of a material

1. Select the **material** in the filter.
2. Click **Search** — the reservation list shows all reservations for this
   material. If the result contains reservations of exactly one storage bin,
   that bin is shown as the current facility.

## Field reference

| Field | Description |
|---|---|
| Material filter | Material whose reservations are shown |
| Reservation list | All reservations found for the material |
| Current facility | Storage bin of the result (only if the result is limited to one bin) |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOReservArticle`
contains the complete usage recipe: set the filter on
`CurrentFacilityReservationData` (`MaterialFilter`) and call `Search()`; the
result is in `CurrentFacReservationList`, the single-bin result in
`CurrentFacility`. The BSO is read-only.
