# Tracking and Tracing (Track & Trace / Lot Tracking)

> Business object: `BSOTandTv3` (Assembly `gip.bso.facility`, registered in the **Businessobject-Manager**; typically opened from the facility charge/lot apps or as report)

## Definition

**Tracking and Tracing** answers two questions for every lot (batch):

* **Backward tracking** (*Rückverfolgung*): which lots, deliveries and bookings were **used** to produce a given lot?
* **Forward tracking** (*Verfolgung*): in which lots was a given lot **used** later?

The result is displayed as a **graph** (nodes = storage places, containers, receivals/issues, productions, machines, lots) and in **data tabs** (lot formation, deliveries, lots, production orders, lab reports).

Every executed tracking is stored as a **tracking filter** record, so a tracking can be re-opened later without recalculating it.

## Concepts

| Term | Meaning |
|---|---|
| **Tracking direction** | **Backward** = which inputs were used; **Forward** = where the lot was used |
| **Tracing item** | The start object type of the tracking (e.g. stock booking / pre-booking) |
| **Number** | The system number of the start object (e.g. booking number) |
| **Tracking filter** | Saved tracking run (start item, direction, period, recalculation flag) |
| **Lot formation (Mix point)** | A production step in which input lots are mixed into output lots |
| **Display type** | Which node groups are shown in the graph (storage places, receival/issue/production, machines, compressed/detail view, materials) |
| **Lab report** | Laboratory order attached to a material of a lot formation step |

## Screen layout

The app consists of three areas (docking layout):

| Area | Content |
|---|---|
| **Search** (top, collapsible) | Filter bar: **Direction**, **Tracing Item**, **Number**, **From**, **To**, **Recalc** and the **Start tracking** button |
| **Tracking** (main tab) | Contains the **Filter** bar and two tabs: **Data** and **Graph** |
| **Data** tab | Nested tabs: **Delivery note**, **Lots**, **Production order**, **Receival**, **Laboratory** |
| **Graph** tab | Graphic tracking tree with display type filter, zoom and edge routing |
| **Explorer** (auto-hide, right) | List of saved trackings with **Delete all results** |
| **Messages** (auto-hide, bottom) | Message list of the last tracking run |

### Data tab — nested views

| Nested tab | Content |
|---|---|
| **Delivery note** | Delivery note positions of the tracking result (delivery note no., date, material, quantities, shipper, delivery address, contract no., semitrailer) plus indirect delivery notes |
| **Lots** | Lots of the result: lot no., external lot no., material, facility, batch no., dosed quantity, production order, comment |
| **Production order** | Recapitulation of production orders with machines and inputs (target/actual quantities inward/outward) |
| **Receival** | Goods receival delivery notes (same columns as *Delivery note*) |
| **Laboratory** | Lot formation steps with lab reports: material with lab report, lab report (no., sample taking date, template) and lab order lines (tag, actual value, min/max limits, state) |

## Starting a tracking (step by step)

1. Open the **Search** bar at the top.
2. Select the **Direction** (**Backward** is the default).
3. Select the **Tracing Item** (e.g. stock booking).
4. Enter the **Number** of the start object.
5. *(Optional)* Restrict the period with **From** / **To**.
6. *(Optional)* Tick **Recalc** to force a new calculation instead of using the cached result.
7. Click **Start tracking**. A progress dialog is shown while the tracking runs.
8. The result appears in the **Graph** tab and in the tabs under **Data**.

## Working with saved trackings

* Selecting a row in the **Explorer** list re-executes that tracking (from cache where possible).
* **Search** refreshes the list of saved trackings and re-runs the current one.
* **Load** repeats the last tracking with recalculation.
* **Save** stores changes of the current tracking filter.
* **Delete** removes the result of the selected tracking; the next tracking is shown afterwards.
* **Delete all results** (in the Explorer) deletes **all** cached tracking results after a confirmation question.

## Working with the graph

On the **Graph** tab:

1. In the **Filter** area, tick/untick the **display types** (storage places and container, receival/issue/production, machines and work centers, compressed view, detail view, used and manufactured). Note: *Used and manufactured* excludes the lot views and storage places.
2. Click **Calculate and update** to redraw the graph with the selected display types.
3. Tick **Crossing avoidance** and click **Recalc. edges** to recalculate the edge routing with fewer crossings.
4. Click on a node and use **Show details** (context menu) to open the detail dialog of that node.
5. Use the **Zoom %** control to scale the graph.

## Laboratory reports

On the **Laboratory** tab:

1. Select a **lot formation** step in the upper list (only steps with lab reports qualify).
2. The middle list shows the **material with lab report**.
3. Select a **lab report** (no., sample taking date, template).
4. The lower list shows the **laboratory order lines** with tag, actual value, min/max limits and state.

## Field reference (search bar)

| Field | Description |
|---|---|
| Direction | Backward or Forward tracking |
| Tracing Item | Start object type of the tracking |
| Number | System number of the start object (e.g. booking number) |
| From / To | Optional period the tracking considers |
| Recalc | Force recalculation instead of cached result |

## Field reference (Lots tab)

| Field | Description |
|---|---|
| Lot no. / External lot no. / External lot no. 2 | Identification of the lot |
| Material no. / Material name | Material of the lot |
| Facility no. / Facility name | Storage bin of the lot |
| Batch no. | Batch number |
| Dosed quantity | Quantity dosed in the step |
| Production order | Program number of the production order |
| Insert date | Creation date |
| Comment | Free comment |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOTandTv3` contains the
complete usage recipe. Key members:

* Navigation: `FilterList` (saved trackings) → `SelectedFilter` (assigning re-runs the tracking)
* Filter fields: `SelectedItemType`, `FilterSearchNo`, `SelectedTrackingStyle`, `FilterDateFrom`, `FilterDateTo`, `FilterRecalcAgain`
* Methods: `SearchFilter()` (start tracking), `Search()`, `Filter()` (redraw graph), `SwitchDisplayType()`, `Load()`, `Save()`, `Delete()`, `DeleteAllCacheDlg()`, `ShowDetails(TandTPointPresenter)`, `RecalcEdgesRoute()`
* Results: `Result` (TandTResult), `TandTMixPointList`, `DeliveryNoteList`, `FacilityChargeList`, `AvailableTandTEdges`, `DisplayTypeList`

Button captions above are the translations of these methods (`ACMethod*` attributes,
stored in `ACClassText`).
