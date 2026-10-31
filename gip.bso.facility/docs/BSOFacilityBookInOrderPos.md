# Inventory Management Purchase Order (Goods Receipt / Inward Movement)

> Business object: `BSOFacilityBookInOrderPos` (Assembly `gip.bso.facility`, Menu: **Warehouse Management → Inorderfacility**)

## Definition

This app posts **goods receipts (inward movements)** for purchase order positions and
**activates** purchase order positions in the warehouse management.

It is a *booking* app: it does not store editable records. Instead the booking data is
collected in a booking parameter set and posted immediately via the facility manager.
Every posting is executed immediately — there is no **Save** button.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Purchase order position** | Position of a purchase order (InOrderPos) for which stock is received |
| **Booking parameter** | The collected booking data (order, position, material, quantity, weight, target bin) |
| **Inward movement** | Stock receipt posting: quantity is added to a storage bin/charge |
| **Activation** | Releasing an order position so that it can be booked |
| **Release state** | State of the position (e.g. released/blocked) |

## Screen layout

The screen consists of a header area and two tabs. The **Explorer** (search list of
purchase order positions) is docked at the bottom edge as an auto-hide window.

**Header area — "Purchase order position"**

* **Order** — selection of the purchase order
* **Order position** — selection of the order position
* **Material** — selection of the material to be stored

**Tab "Incomming goods" — "Bookingparameter"**

| Field | Description |
|---|---|
| Quantity | Quantity to be posted |
| Unit | Unit of the quantity (read-only) |
| Weight | Weight to be posted |
| Storage location | Target storage location (read-only) |
| Storage bin | Target storage bin |
| Storage charge | Target storage charge (lot) |
| Release state | Release state of the position |

Button: **Post Inward Movement**

**Tab "Activation" — "Bookingparameter"**

Button: **Activate**

## Step-by-step tasks

### Posting a goods receipt

1. Select the **Order**, the **Order position** and the **Material** in the header area.
2. On the **Incomming goods** tab enter the **Quantity** and the **Weight**.
3. Select the target **Storage bin** and, if required, the **Storage charge**.
4. *(Optional)* Set the **Release state**.
5. Click **Post Inward Movement**. The booking is executed immediately.

### Activating an order position

1. Select the **Order** and the **Order position** in the header area.
2. Switch to the **Activation** tab.
3. Click **Activate**. The position is released for booking.

### Notes

* The app starts with an empty booking parameter set; use **New** (toolbar) to clear
  all booking parameters and start a fresh entry.
* Buttons are only enabled when the booking parameters are complete.
* Postings cannot be undone in this app — check the data before posting.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityBookInOrderPos`
contains the complete usage recipe. Relevant C# methods: `New` (clears booking data),
`Search`, `InOrderPosInwardMovement` (**Post Inward Movement**),
`InOrderPosActivate` (**Activate**). Booking data is set on `CurrentBookingParameter`
(`InOrder`, `InOrderPos`, `InwardMaterial`, `OutwardQuantity`, `OutwardWeight`,
`OutwardFacilityLocation`, `OutwardFacility`, `OutwardFacilityCharge`, `ReleaseState`).
