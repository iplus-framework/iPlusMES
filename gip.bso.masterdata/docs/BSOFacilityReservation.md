# Reservations (Stock Reservations of Lots)

> Business object: `BSOFacilityReservation` (Assembly `gip.bso.masterdata`)
> This app is **not opened directly from the menu**. It is embedded as a child window in the
> **Picking**, **Production Order** and **Sales Order (Out Order)** apps and is operated from there.

## Definition

A **reservation** books stock (a lot in a storage bin) for a document position, so that the
quantity is not consumed by another order. The Reservation app manages these bookings for a
single order position:

* Which lots are reserved for the position (list of reservations)
* How the required quantity is distributed over the available lots (lot dialog)

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Reservation** | Booking of a lot quantity for an order position |
| **Owner position** | The document position the reservations belong to (production order input position, picking position or sales order position) |
| **Quantity for reservation** | Quantity that still has to be reserved; preset with the target quantity of the position |
| **Lot dialog** | Dialog in which the available lots of the material are listed and quantities are assigned |
| **Included / Excluded facilities** | Filter lists that restrict the lot search to certain storage bins (including sub-bin levels) |
| **Total Assigned Quantity** | Sum of the quantities assigned to the selected lots in the lot dialog |
| **Diff** | Difference between *Quantity for reservation* and *Total Assigned Quantity* — how much is still missing |

## Screen layout

The app consists of two parts:

1. **Reservation list** (tab of the hosting order app) — the existing reservations of the
   position with lot number, external lot number, storage bins, assigned/reserved/used/free
   quantity and reservation state. Toolbar: **Add**, **Remove**.
2. **Lot dialog** — opened with **Add**. It contains:
   * **Facility selection**: **Included** and **Excluded** facility lists with **Add** /
     **Remove** buttons and a **Search** button to refresh the lot list.
   * **Lot list**: available lots with selection flag, lot number, external lot number,
     oldest charge date, storage bins, reserved/used/free quantity and the assignable
     **Assigned Quantity**.
   * **Quantity fields**: *Quantity for reservation*, *Total Assigned Quantity*, *Diff*.
   * Buttons: **Ok**, **Distribute Quantity**, **Cancel**.

## Reserving stock (step by step)

1. Open the position in the hosting order app (production order, picking or sales order).
   The existing reservations of the position are shown in the reservation list.
2. Click **Add**. The lot dialog opens; the *Quantity for reservation* is preset with the
   target quantity of the position and the available lots of the material are listed.
3. *(Optional)* Restrict the search: add storage bins to **Included** or **Excluded** with
   the **Add**/**Remove** buttons of the respective list, then click **Search** to refresh
   the lot list.
4. Select the lots to reserve (selection flag) and enter the **Assigned Quantity** per lot.
   Selecting a lot automatically proposes up to the missing quantity (*Diff*).
5. Alternatively click **Distribute Quantity**: the missing quantity is distributed
   automatically — oldest charge date first, then by lot number, up to the free quantity
   of each lot.
6. Check the *Total Assigned Quantity* and the *Diff*.
7. Click **Ok** to confirm. An error message is shown if
   * a larger quantity than the free quantity is reserved for a lot, or
   * a larger quantity than required is reserved in total.
   On success the new reservations appear in the reservation list.
   Click **Cancel** to discard all entries instead.
8. Save the order position in the hosting app to persist the reservations.

## Removing a reservation

1. Select the reservation in the reservation list.
2. Click **Remove**. The reservation is deleted.
3. Save the order position in the hosting app.

## Facility filter (Included / Excluded facilities)

The included/excluded facility lists are stored per group in the app configuration and are
restored the next time the dialog is opened. Only charges in included facilities (including
sub-bin levels) are offered; charges in excluded facilities are hidden.

## Behaviour notes

* If the material of the owner position is changed, **all existing reservations of the
  position are deleted**.
* Reservations are written to the database when the hosting order app is saved.
* **Load** / refresh of the reservation list is done automatically by the hosting app when
  the position is (re)loaded.

## Field reference (lot dialog)

| Field | Description |
|---|---|
| Is Selected | Lot is chosen for reservation |
| Lot No / Extern Lot No | Lot identification |
| Oldest Charge Date | Oldest charge date of the lot (filling/production/insert date) — used for FIFO distribution |
| Facility Nos | Storage bins holding the lot |
| Total Reserved Quantity / Used Quantity / Free Quantity | Stock situation of the lot |
| Assigned Quantity | Quantity of this lot to reserve |
| Quantity for reservation | Quantity still to be reserved (preset with target quantity) |
| Total Assigned Quantity | Sum of assigned quantities of selected lots |
| Diff | Missing quantity (Quantity for reservation − Total Assigned Quantity) |
| Reservation State | State of the reservation |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityReservation` contains
the complete usage recipe. Important: this is a **child BSO** — it is controlled by its
hosting BSO (`BSOPicking`, `BSOProdOrder`, `BSOOutOrder` via `BSOFacilityReservation_Child`),
which assigns `FacilityReservationOwner` (a `ProdOrderPartslistPos`, `PickingPos` or
`OutOrderPos`) and persists the changes. Key methods: `AddFacilityReservation`,
`RemoveFacilityReservation`, `ShowLotDlgOk`, `ShowLotDlgCancel`, `DistributeQuantity`,
`SearchLot`, `AddFilterFacilityIncluded`, `RemoveFilterFacilityIncluded`,
`AddFilterFacilityExcluded`, `RemoveFilterFacilityExcluded`. The button captions above are
the translations of these methods (`ACMethod*` attributes, stored in `ACClassText`).
