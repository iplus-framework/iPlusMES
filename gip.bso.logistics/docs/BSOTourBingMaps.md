# Bing Maps Tour (BSOTourBingMaps)

> Business object: `BSOTourBingMaps` (Assembly `gip.bso.logistics`, German caption: **Bing Maps Tour**)

> ⚠️ **Status: deactivated.** The complete source file of this business object is commented out; it is not compiled and cannot be started. This manual documents the intended behavior for the case it is reactivated.

## Definition

**Bing Maps Tour** displays the stops of a vehicle route on a Bing Maps map. It is a child component of **Vehicle Route** (`BSOTourplan`) and reacts automatically to the tour planning screen: every delivery address of the open and assigned order positions is geocoded and shown as a pin on the map.

## Usage / Concepts

| Term | Meaning |
|---|---|
| Pin | A map marker for one delivery address. Several order positions can share one pin. |
| Geocoding | Resolving a company address to map coordinates via the Bing Maps GeocodeService; results are cached per address. |
| Route | The driving route over the own company and all addresses assigned to the tour, calculated via the Bing Maps RouteService. |
| Pin popup | Hover window on a pin showing material, quantity, delivery date and address of the order positions at this stop. |

## Screen layout

- Map area with pins for the open and assigned purchase/sales order positions.
- **Auto Route Calculation** checkbox – recalculates the route automatically when the tour changes.
- **Calculate Route** button – calculates the route manually.
- Pin popup with the order positions of the hovered address; the context menu of a pin offers each order position.

## Step-by-step tasks

### Show the tour on the map
1. Open the tour planning screen (**Vehicle Route**). The map child component loads automatically.
2. Each delivery address of the listed order positions appears as a pin. Hover a pin to see its order positions in the popup.

### Calculate the route
1. Assign the desired positions in the tour planning screen.
2. Either enable **Auto Route Calculation**, or click **Calculate Route**. The route starts at the own company and covers all assigned addresses; the itinerary is shown on the map.

### Assign/unassign via the map
1. Right-click a pin. The context menu lists the order positions of this address.
2. Choose a position: if it is not yet on the tour it is assigned (like **>** in the tour planning screen); if it is already on the tour it is unassigned (like **<**).

## Field reference

| Field (UI) | Property | Description |
|---|---|---|
| Auto Route Calculation | AutoCalculateRoute | Recalculate route on tour changes. |
| Directions (open/assigned, purchase/sales) | DirectionsInOrder, DirectionsInOrderTour, DirectionsOutOrder, DirectionsOutOrderTour | Geocoded addresses per list. |
| Pin popup list | TourPinPopupList | Order positions of the hovered pin. |

## For AI agents (MCP)

See the class summary of `BSOTourBingMaps` (via `get_type_infos`). The BSO observes the parent `BSOTourplan` properties `InOrderPosList`, `InOrderTourPosList`, `OutOrderPosList`, `OutOrderTourPosList`. `UpdateRoute()` calculates the route; `OnPinContextMenuClicked(id)` delegates assignment to the parent BSO. Note: currently commented out in source.
