# Order Inventory Management (Goods Issue / Outward Movement)

> Business object: `BSOFacilityBookOutOrderPos` (Assembly `gip.bso.facility`, Menu: **Warehouse Management → Outorderfacility**)

## Definition

This app posts **goods issues (outward movements)** for sales order positions and
**activates** sales order positions in the warehouse management.

It is a *booking* app: it does not store editable records. Instead the booking data is
collected in a booking parameter set and posted immediately via the facility manager.
Every posting is executed immediately — there is no **Save** button.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Order position** | Position of a sales order (OutOrderPos) for which stock is issued |
| **Booking parameter** | The collected booking data (order, position, material, quantity, weight, source bin) |
| **Outward movement** | Stock issue posting: quantity is taken from a storage bin/charge |
| **Activation** | Releasing an order position so that it can be booked |
| **Release state** | State of the position (e.g. released/blocked) |

## Screen layout

The screen consists of a header area and two tabs. The **Explorer** (search list of
sales order positions) is docked at the bottom edge as an auto-hide window.

**Header area — "Orderposition"**

* **Order** — selection of the sales order
* **Order position** — selection of the order position
* **Material** — selection of the material to be issued

**Tab "Outgoing goods" — "Bookingparameter"**

| Field | Description |
|---|---|
| Quantity | Quantity to be posted |
| Unit | Unit of the quantity (read-only) |
| Weight | Weight to be posted |
| Release state | Release state of the position |
| Storage location | Source storage location (read-only) |
| Storage bin | Source storage bin |

Button: **Outward**

**Tab "Activation" — "Bookingparameter"**

Button: **Activate**

## Step-by-step tasks

### Posting a goods issue

1. Select the **Order**, the **Order position** and the **Material** in the header area.
2. On the **Outgoing goods** tab enter the **Quantity** and the **Weight**.
3. Select the source **Storage bin** from which the stock is taken.
4. *(Optional)* Set the **Release state**.
5. Click **Outward**. The booking is executed immediately.

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
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityBookOutOrderPos`
contains the complete usage recipe. Relevant C# methods: `New` (clears booking data),
`Search`, `OutOrderPosOutwardMovement` (**Outward**),
`OutOrderPosActivate` (**Activate**). Booking data is set on `CurrentBookingParameter`
(`OutOrder`, `OutOrderPos`, `InwardMaterial`, `OutwardQuantity`, `OutwardWeight`,
`OutwardFacilityLocation`, `OutwardFacility`, `ReleaseState`).
