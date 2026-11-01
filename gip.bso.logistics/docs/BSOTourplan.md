# Vehicle Route (Tourplan)

> Business object: `BSOTourplan` (Assembly `gip.bso.logistics`, German caption: **Tour**)

## Definition

**Vehicle Route** is used to plan delivery and pickup tours. A *tour* (Tourplan) groups order positions that shall be delivered or collected together. When an order position is assigned to a tour, the system automatically creates a **delivery note** (DeliveryNote) with a delivery note position for the delivery address of the order and links it to a *tour position* (TourplanPos) of that address. The tour positions therefore represent the stops of the tour.

## Usage / Concepts

| Term | Meaning |
|---|---|
| Tour (Tourplan) | The route plan with a tour number; contains tour positions (stops). |
| Tour position (TourplanPos) | One stop of the tour, per delivery address/company. Created automatically on the first assignment for an address. |
| Open purchase order position | An InOrderPos that is not yet completely assigned to a delivery note; can be planned on a tour (goods to be collected). |
| Open sales order position | An OutOrderPos that is not yet completely assigned to a delivery note; can be planned on a tour (goods to be delivered). |
| Delivery note (DeliveryNote) | Created automatically per delivery address when positions are assigned; carries the tour position reference. |
| Delivery position state | Only positions that are not yet *completely assigned* appear in the open lists. |

## Screen layout

The screen shows the tour list on the left and the detail area of the current tour on the right. The detail area contains (based on the bound properties; the design database was not reachable during documentation, so exact tab captions could not be verified):

- **Tour** – master data of the current tour (tour number etc.).
- **Open purchase order positions** – list of InOrderPos that can be assigned, with **>** (assign) and **<** (unassign) buttons and a **Filter In-Order** button.
- **Assigned purchase order positions** – the InOrderTourPos list of positions already on the tour.
- **Open sales order positions** – list of OutOrderPos that can be assigned, with **>** (assign) and **<** (unassign) buttons and a **Filter Sales-Order** button.
- **Assigned sales order positions** – the OutOrderTourPos list of positions already on the tour.

## Step-by-step tasks

### Create a tour
1. Click **New**. A new tour with a new tour number is created and becomes the current tour.
2. Fill in the required fields of the tour.
3. Click **Save**.

### Assign purchase order positions (goods to collect)
1. Select the tour in the tour list (or search first with **Search**).
2. Select a position in the *open purchase order positions* list. Use **Filter In-Order** to narrow the list.
3. Click **>**. The position moves to the *assigned purchase order positions* list; a delivery note and a tour stop for the delivery address are created (unsaved).
4. Repeat for further positions, then click **Save**. If a warning message appears, confirm with **Yes**.

### Remove positions from the tour
1. Select a position in the *assigned purchase order positions* list.
2. Click **<**. The position is unassigned; if its delivery note becomes empty, the delivery note is deleted (unsaved).
3. Click **Save**.

### Assign / remove sales order positions (goods to deliver)
Same as above, using the *open sales order positions* list with **>** / **<** and **Filter Sales-Order**.

### Correct mistakes before saving
- Click **Undo** to discard all unsaved assignments and newly created delivery notes.

### Delete a tour
1. Select the tour and click **Delete**.
2. Click **Save** to execute the deletion in the database. Deletion fails if other records reference the tour.

## Field reference

| Field (UI) | Property | Description |
|---|---|---|
| Tour no. | Tourplan.TourplanNo | Number of the tour (sorted descending in the list). |
| Open purchase order positions | InOrderPosList | Positions not yet on the tour. |
| Assigned purchase order positions | InOrderTourPosList | Positions assigned to the tour. |
| Open sales order positions | OutOrderPosList | Positions not yet on the tour. |
| Assigned sales order positions | OutOrderTourPosList | Positions assigned to the tour. |

## For AI agents (MCP)

See the class summary of `BSOTourplan` (via `get_type_infos`). Workflow: `SearchWord` → `TourplanList` → `NavigateFirst()` → `CurrentTourplan`; edit only on `CurrentTourplan`, then `Save()`. Assignment: set `SelectedInOrderPos` and call `AssignInOrderPos()` / set `SelectedInOrderTourPos` and call `UnassignInOrderPos()` (analogous for OutOrderPos). `FilterDialogInOrderPos()` / `FilterDialogOutOrderPos()` open query dialogs. Unsaved assignments are held in memory until `Save()`; `UndoSave()` discards them.
