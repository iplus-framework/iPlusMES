# Open Postings (Production Order Positions)

> Business object: `BSOProdOrderOpenPositions` (Assembly `gip.bso.manufacturing`, Menu: **Manufacturing → Open Postings / Offene Buchungen**)

## Definition

The **Open Postings** app finds production order positions whose **bookings are
incomplete** — either bookings are missing entirely or only **pre-bookings** exist so
far. For every position it shows the **outward** side (consumption of input materials)
and the **inward** side (receipt of the produced material) with target/actual quantities
and the number of existing pre-bookings and bookings.

The app is **read-only**: it does not create, change or delete records. It is used to
detect and follow up on missing postings; the actual booking is done in the production
order dialog.

## Concepts

| Term | Meaning |
|---|---|
| **Open position** | Production order position with missing or only pre-booked postings |
| **Outward** | Consumption bookings of input materials (issue) |
| **Inward** | Receipt bookings of the produced material |
| **Pre-booking** | A planned/announced booking that has not yet been posted |
| **Booking** | A posted stock booking (FacilityBooking) |
| **Direction filter** | Restricts the list to a specific kind of open posting |

## Screen layout

The app consists of three docked areas:

| Area | Content |
|---|---|
| **Position form** (main tab) | Search and filter rows, then the **Outward** and **Inward** sections with pre-booking and booking lists |
| **Explorer** (docked) | List of open positions |
| **Messages** (auto-hide) | Messages of the background search (level, identifier, message) |

**Search row:** Order Number, From, to, **Search** button.
**Filter row:** Direction combo box, **Filter** button.

**Explorer columns:** No., Order No., Partslist Sequence, Parent Pos. Sequence, Pos.
Sequence, Batch No., Insert Date, and for Outward and Inward each: Material No.,
Material Name, Target Quantity, Actual Quantity, Pre-Booking Count, Booking Count.

## Step-by-step tasks

### Searching for open positions

1. Enter the **From** / **to** period in which the production orders were created
   (default: last month up to today).
2. *(Optional)* Enter an exact **Order Number** — it is then used instead of the period.
3. Click **Search**. The search runs in the background (may take up to a few minutes for
   large datasets); a progress dialog is shown and the result appears in the position
   list.

### Filtering the result

1. Select a **Direction**, e.g. *Exist Outward FacilityPreBookings*,
   *Exist Inward FacilityPreBookings*, *No Outward Bookings* or *No Inward Bookings*.
2. Click **Filter** — the list shows only positions of the selected kind.

### Inspecting the bookings of a position

1. Select a position in the position list. The **Outward** and **Inward** sections are
   filled with the position's **Pre-Booking** and **Booking** lists.
2. Select a pre-booking or booking row to inspect it (quantities, storage bins, lots,
   charges, movement reason, comment).

### Opening the production order

1. Select a position, pre-booking or booking.
2. Click **View order** — the production order dialog opens with the position or booking
   preselected, where the missing bookings can be executed.

## Field reference (position list)

| Field | Description |
|---|---|
| No. | Sequential number of the result row |
| Order No. | Production order number |
| Partslist / Parent Pos. / Pos. Sequence | Position of the line within the order structure |
| Batch No. | Batch number of the position |
| Insert Date | Booking date (inward) or position creation date |
| Outward Material / Target / Actual | Input material with target and actual quantity |
| Outward Pre-Booking / Booking Count | Number of existing outward pre-bookings / bookings |
| Inward Material / Target / Actual | Produced material with target and actual quantity |
| Inward Pre-Booking / Booking Count | Number of existing inward pre-bookings / bookings |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOProdOrderOpenPositions`
contains the complete usage recipe (filters → `Search()` → `SelectedOpenPosition` →
booking lists, `ShowOrderSelected*()` methods), so an agent can operate the app
remotely, e.g. for user training. The button captions above are the translations of the
`ACMethod*` attributes (stored in `ACClassText`).
