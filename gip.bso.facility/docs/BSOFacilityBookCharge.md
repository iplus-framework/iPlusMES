# Quant Management (Facility Charges / Stock Booking)

> Business object: `BSOFacilityBookCharge` (Assembly `gip.bso.facility`, Module: **Warehouse Management / Facility**)

## Definition

A **quant** (facility charge, `FacilityCharge`) is the stock keeping unit of the warehouse:
the quantity of one **material** in one **storage bin** (facility), optionally assigned to
one **lot**. The *Quant Management* app is used to

* create and maintain quants,
* book **inward** (stock receipt) and **outward** (stock issue) movements,
* perform **stock transfers** (relocations) to other storage bins,
* **lock** and **release** quants,
* mark quants as **not available** (zero stock) and make them available again,
* **split** quants and **reassign** material or lot,
* change the **expiration date** of a lot.

## Concepts

| Term | Meaning |
|---|---|
| **Quant / Facility charge** | Stock of one material in one storage bin, optionally bound to one lot |
| **Storage bin (Facility)** | Physical or logical storage location |
| **Lot** | Batch of a lot-managed material; a lot-managed quant cannot be saved without a lot |
| **Release state** | Free, Locked, absolutely locked/free — controls whether a quant may be booked |
| **Not available (zero stock)** | Flag that removes the quant from availability, e.g. after an inventory difference |
| **Booking parameters** | The values of the current posting (quantity, unit, balancing mode, movement reason, comment) — reset by *New Booking Record* |
| **Mass update** | Applying an availability change to *all* quants in the search result at once |

## Screen layout

The app consists of a **quant form** (left), a **posting functions** area (below), an
**explorer** list (docked right, auto-hide) and a **messages** panel (docked bottom).

**Quant form** (`Quant`):

| Area | Content |
|---|---|
| Quant data form | Material, storage bin, lot, split no., external lot numbers, stock quantity, unit, release state, not-available flag — plus the *New Lot* and navigation buttons |
| **Quant Information** tab | Filling date, sort no., production date, expiration date, storage life, comment, order no., program no., company |
| **Stock** tab | Stock quantity, reserved inward/outward/total quantity, available quantity, not-available flag |

**Posting Functions** — one tab per booking type:

| Tab | Content |
|---|---|
| **Inward Posting** | Quantity + unit, optional: absolute quantity, balancing mode, movement reason, comment → **Post Inward Movement** |
| **Outward Posting** | Quantity + unit, optional: absolute quantity, balancing mode, movement reason, contractual partner → **Post Outward Movement** |
| **Stock Transfer** | Quantity + unit, target storage bin (**Choose facility**), optional parameters → **Post Stock Transfer** |
| **Releasing/Locking** | Release state of the quant and buttons **Lock**, **Lock absolute**, **Unlock** |
| **Availability** | Not-available flag and buttons **Quant not available** / **Quant available** |
| **Reassignment** | Suggested material, new material, quantity, BOM → **Reassign Material** |
| **Reassign Lot** | New lot, quantity, BOM → **Reassign Lot** |
| **Split** | Split quantity, new split no., optional parameters → **Split Quant** |
| **Expiration Date** *(WPF layout only)* | New expiration date → **Change Expiration Date** |

> Note: in the current Avalonia layout the **Expiration Date** tab is not yet embedded;
> the function exists in the business object and in the WPF layout.

**Explorer** (right, auto-hide): **Data filter** (show not available, material, storage,
lot, external lot no. 1/2, stock quantity less than), **Time filter** (expiration date
with **Today**, **Next [0] days**, **Reset**), the quant result list and the
**Mass update** buttons (**All Quants not available**, **All Quants available**).

**Messages** (bottom): messages and warnings of the last booking.

## Creating a new quant (step by step)

1. Click **New** in the toolbar. A new, empty quant is created and opened.
2. Assign the **Material** — the unit is preset from the material's base unit.
3. Assign the **Storage bin**.
4. For lot-managed materials assign the **Lot**, or click **New Lot** to create one in
   the lot dialog. A lot-managed quant without a lot cannot be saved (error 50552).
5. Click **Save**.

The **Material**, **Storage bin**, **Split no.** and lot fields are disabled as soon as
the quant is saved (they can only be set for new quants; the lot is locked once the
quant has had any stock).

## Booking stock movements

### Inward posting (stock receipt)

1. Select the quant in the explorer list.
2. On the **Inward Posting** tab enter the **Quantity** and unit.
3. *(Optional)* Set **Quantity is absolute**, **Balancing mode**, **Movement reason**
   and a **Comment**.
4. Click **Post Inward Movement**. On success the booking parameters are reset;
   messages appear in the **Messages** panel.

### Outward posting (stock issue)

1. Select the quant.
2. On the **Outward Posting** tab enter the **Quantity** and unit.
3. *(Optional)* Set balancing mode, movement reason and contractual partner.
4. Click **Post Outward Movement**.

### Stock transfer (relocation)

1. Select the quant.
2. On the **Stock Transfer** tab enter the **Quantity** and unit.
3. Click **Choose facility** and select the target storage bin.
4. Click **Post Stock Transfer**.
   * If the target bin is connected to a plant/transport system, the system asks
     whether the transport should run in **automatic mode**:
     * **Yes** — a picking order is created and the transport workflow is started
       (if preferred parameters are configured, the corresponding dialog opens first).
     * **No** — the relocation is booked directly as a stock movement.
   * Otherwise the relocation is booked directly.

### Splitting a quant

1. Select the quant.
2. On the **Split** tab enter the **Quantity** to split off and the **Split no.**
   of the new quant.
3. Click **Split Quant**. The quantity is moved to a new quant with the same
   material/bin/lot.

## Locking and releasing

On the **Releasing/Locking** tab (release state of the quant is shown):

* **Lock** — the quant is no longer proposed for bookings.
* **Lock absolute** — hard lock; can only be removed by an absolute release.
* **Unlock** — sets the quant back to free.

## Availability (zero stock)

On the **Availability** tab:

* **Quant not available** — marks the current quant as not available
  (the *Not available* flag in the quant form is set).
* **Quant available** — makes the quant available again. If the quant had a stock
  quantity before it was set to not available, the system asks whether the last
  stock quantity should be **restored**.
* **All Quants not available** / **All Quants available** (explorer, **Mass update**)
  — apply the change to **all quants in the search result** after a confirmation
  question.

## Reassigning material or lot

* **Reassign Material**: on the **Reassignment** tab select the suggested material
  (the list proposes suitable materials) or enter the new material, then click
  **Reassign Material**.
* **Reassign Lot**: on the **Reassign Lot** tab select the new lot, then click
  **Reassign Lot**.
* **Change Expiration Date**: enter the new date and click **Change Expiration Date**
  (WPF layout; see note above).

## Filters and mass functions in the explorer

* Enter search text in **Material**, **Storage**, **Lot**, **External lot no.** /
  **External lot no. 2** or limit the **Stock quantity less than** — each change
  triggers a new search immediately.
* **Show not available** includes quants that are marked as not available.
* **Time filter** (expiration date): **Today**, **Next [0] days** (period is
  configurable) or **Reset**.
* **Search** re-executes the search; **Load** re-reads the selected quant if another
  user changed it in the background; **Undo** discards unsaved changes.

## Navigation from a quant

Buttons on the quant data form (enabled depending on the quant's data):

* **Show Order** — opens the production order of the quant's origin position.
* **Show stock history of quant** — stock history of this quant.
* **Show Material Stock and History** / **Manage Material** — material stock overview / material master data.
* **Show Lot Stock and History** / **Manage Lot/Batch** — lot stock overview / lot master data.
* **Show Bin Stock and History** / **Manage Stock of Bin** — bin stock overview / bin stock management (storage-bin containers only).

## Field reference (quant form)

| Field | Description |
|---|---|
| Material | Material of the quant (only for new quants) |
| Storage bin | Storage location (only for new quants) |
| Lot | Lot of lot-managed materials; locked once the quant has had stock |
| Split no. | Split number of the quant (only for new quants) |
| External lot no. / External lot no. 2 | Customer/supplier lot numbers of the lot |
| Stock quantity / Stock quantity (UOM) | Current stock with unit |
| Reserved inward / outward / reserved | Quantities reserved by open orders |
| Available quantity | Stock minus reservations |
| Release state | Free / Locked / absolutely locked or free |
| Not available | Zero-stock flag |
| Filling date, Sort no. | Quant information |
| Production date, Expiration date, Storage life | Lot dates (from the lot) |
| Order no., Program no. | Originating production order |
| Company no. / Company name | Contractual partner |
| Comment | Free text |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityBookCharge`
contains the complete usage recipe (search → navigate → edit → save, plus all booking
workflows), so an agent can operate the Quant Management app remotely, e.g. for user
training.

Key C# methods and their button captions:

| Method | Button caption (en) |
|---|---|
| `New` / `Save` / `UndoSave` / `Load` / `Search` | New / Save / Undo / Load / Search |
| `ClearBookingData` | New Booking Record |
| `InwardFacilityChargeMovement` | Post Inward Movement |
| `OutwardFacilityChargeMovement` | Post Outward Movement |
| `FacilityChargeRelocation` (+ `ShowDlgFacilityRelocation`) | Post Stock Transfer (+ Choose facility) |
| `LockFacilityCharge` / `LockFacilityChargeAbsolute` / `ReleaseFacilityCharge` / `ReleaseFacilityChargeAbsolute` | Lock / Lock absolute / Unlock / Unlock absolute |
| `NotAvailableFacilityCharge` / `AvailableFacilityCharge` | Quant not available / Quant available |
| `NotAvailableFacilityChargeAll` / `AvailableFacilityChargeAll` | All Quants not available / All Quants available |
| `FacilityReassign` / `FacilityLotReassign` / `FacilityLotChangeExpirationDate` | Reassign Material / Reassign Lot / Change Expiration Date |
| `SplitQuant` | Split Quant |
| `FacilityChargeLotGenerateDlg` | New Lot |
| `NavigateToOrder`, `NavigateToFacilityChargeHistory`, `NavigateToMaterialOverview`, `NavigateToMaterial`, `NavigateToFacilityLotOverview`, `NavigateToFacilityLot`, `NavigateToFacilityOverview`, `NavigateToFacility` | Show Order, Show stock history of quant, Show Material Stock and History, Manage Material, Show Lot Stock and History, Manage Lot/Batch, Show Bin Stock and History, Manage Stock of Bin |
| `SetFilterExpirationDateToday` / `SetFilterExpirationDateNextDays` / `ResetFilterExpirationDate` | Today / Next [0] days / Reset |

Booking quantities are set on the `CurrentBookParam...` properties
(`CurrentBookParamInwardMovement.InwardQuantity`,
`CurrentBookParamOutwardMovement.OutwardQuantity`,
`CurrentBookParamRelocation.OutwardQuantity` + `.InwardFacility`,
`CurrentBookParamSplit.OutwardQuantity` + `.InwardSplitNo`,
`CurrentBookParamReassignMat.InwardMaterial`,
`CurrentBookParamReassignLot.InwardFacilityLot`,
`CurrentBookParamChangeExpirationDate.ExpirationDate`) before calling the
corresponding booking method. `Delete()` is currently a no-op — quants are not
deleted by this BSO.
