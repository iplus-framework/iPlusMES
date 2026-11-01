# Bing Maps (BSOBingMaps)

> Business object: `BSOBingMaps` (Assembly `gip.bso.logistics`, German caption: **Bing Maps**)

> ⚠️ **Status: deactivated (legacy demo).** The complete source file of this business object is commented out; it is not compiled and cannot be started. This manual documents the intended behavior for the case it is reactivated.

## Definition

**Bing Maps** is a simple route calculator between two addresses using the Bing Maps services (GeocodeService and RouteService). It serves as a demonstration/precursor of **Bing Maps Tour** (`BSOTourBingMaps`).

## Usage / Concepts

| Term | Meaning |
|---|---|
| From / To | Start and destination address as free text. |
| Geocoding | Resolving an address text to coordinates via the Bing Maps GeocodeService. |
| Route result | The route returned by the Bing Maps RouteService, including the itinerary. |
| Route popup | Hover window on a route point showing the direction text. |

## Screen layout

- Input fields **From** (start address) and **To** (destination address).
- **Calculate route** button.
- Map with the route and the itinerary; hovering a route point shows the direction text in a popup.

## Step-by-step tasks

### Calculate a route
1. Enter the start address in **From**.
2. Enter the destination address in **To**.
3. Click **Calculate route**. The itinerary steps appear in the **Directions** list and on the map.

## Field reference

| Field (UI) | Property | Description |
|---|---|---|
| From | From | Start address (default: "Meridian, ID"). |
| To | To | Destination address (default: "Boise, ID"). |
| Directions | Directions | Itinerary steps of the calculated route. |
| Route result | RouteResult | Raw Bing Maps route result. |

## For AI agents (MCP)

See the class summary of `BSOBingMaps` (via `get_type_infos`). Workflow: set `From` and `To`, call `CalculateRoute()`, read `RouteResult` / `Directions`. Note: currently commented out in source.
