# Bin Management (Storage Bin Booking)

> Business object: `BSOFacilityBookCell` (Assembly `gip.bso.facility`, Caption: **Bin Management** / *Behälterverwaltung*)

## Definition

**Bin Management** is used to book stock movements on a **single storage bin** (*Lagerplatz*):

* **Inward posting** (goods receipt) — stock enters the bin
* **Outward posting** (goods issue) — stock leaves the bin
* **Stock transfer** (relocation) — stock moves from this bin to another bin
* **Releasing / Locking** — lock or release the stock of the bin
* **Availability** — mark a bin as "not available" (e.g. a silo that is physically empty) and reset it
* **Adjust Storage Bin** — stock adjustment (matching) of the bin
* **Reassignment** — assign another material (and BOM) to the stock of the bin

In addition the app shows the **stock** of the bin and its **quants** (facility charges /
lots) and offers navigation to the quant, lot, material and production order overviews.

The app is also used as a **dialog** by other modules (`ShowDialogFacility`): it is then
embedded in a dialog with **OK** / **Cancel** buttons.

## Concepts

| Term | Meaning |
|---|---|
| **Bin / Storage bin (Facility)** | A storage location. This app edits one bin at a time (the *current bin*). |
| **Quant (FacilityCharge)** | A stock entry in a bin: material, lot, quantity, release state. A bin can contain several quants. |
| **Lot (FacilityLot)** | Batch/lot under which the stock is managed (with external lot number and filling date). |
| **Booking parameters (ACMethodBooking)** | The input mask of a booking type: quantity, unit, lot, balancing mode, movement reason, contractual partner, comment. Each posting tab has its own parameter set. |
| **Physical transport** | If the bin is connected to a plant component (e.g. a silo with a conveyor), a movement can run in *automatic mode*: a picking order is created and the plant workflow is started instead of a direct booking. |
| **Charge reservation** | If the material requires lot reservation, the relocation asks which quants (and which quantity per quant) to move. |
| **Release state** | *Free*, *Locked*, *Absolutely locked* — controls whether other bookings may use the stock. |
| **Not available** | Flag for bins whose booked stock does not reflect reality (e.g. silo empty). |

## Screen layout

The app consists of a **search list** (Explorer, docked right) with the columns bin
number, bin name, inward/outward enabled, material and stock quantity, a **Messages**
panel (docked bottom) and the main form. The main form has two sections:

**Header "Bin"** — bin number, bin name, material (with material name) and BOM
(partslist) of the current bin, with these tabs:

| Tab | Content |
|---|---|
| **Stock** | Stock quantity, reserved outward, reserved inward, reserved, available quantity (each with unit) and the *Not available* flag |
| **Miscellaneous** | Release state (read-only), tolerance, inward enabled, outward enabled, stack calculator, comment |
| **Quants** | Grid of the quants of the bin: filling date, created by, stock quantity, release state, material number/name, external lot no., lot no., split no., comment, order no., program no. |

**Section "Posting Functions"** — one tab per booking type:

| Tab | Fields | Buttons |
|---|---|---|
| **Inward Posting** | Quantity + unit, shift booking reverse; *Optional*: lot (with **New Lot**), quantity is absolute, balancing mode, movement reason, contractual partner, comment | **Post Inward Movement** |
| **Outward Posting** | Quantity + unit, shift booking reverse; *Optional*: lot, quantity is absolute, balancing mode, movement reason, contractual partner, comment | **Post Outward Movement** |
| **Relocation** | Quantity + unit, target bin, shift booking reverse; *Optional*: lot, quantity is absolute, balancing mode, movement reason, comment | **Post Stock Transfer** |
| **Releasing/Locking** | Release state (read-only) | **Lock**, **Lock Absolute**, **Unlock**, **Adjust Storage Bin** |
| **Availability** | Not available (read-only), company | **Silo empty**, **Silo not empty** |
| **Reassignment** | New material, new BOM, comment | **Reassign Material** |

The booking parameters of the **active tab** are the ones used by its button; switching
tabs switches the active parameter set automatically.

**Quant selection dialog** (shown for relocations with charge reservation): list of the
quants with a *select all* checkbox, *Is selected* flag and *relocation quantity* per
quant, plus **OK** / **Cancel**.

## Working with the bin (step by step)

1. Enter a search string in the search field and click **Search** — the bin list is refreshed.
2. Select a bin in the list and click **Load** (or navigate to it) — it becomes the
   *current bin*; stock, quants and booking parameters are loaded.
3. Edit the bin data (material, BOM, tolerance, inward/outward enabled, comment) and
   click **Save**. **Undo** discards the changes.
4. **Load** re-reads the bin if another user changed it in the background.
5. **New Bookingrecord** resets all booking masks to a fresh booking record.

> Note: this app does **not** create or delete bins — it books on existing bins and
> edits bin data. Bin master data itself is maintained in the facility master data.

## Posting an inward movement (goods receipt)

On the **Inward Posting** tab:

1. Enter the **Quantity** and, if needed, change the **unit**.
2. *(Optional)* Assign a **lot** — either select an existing one or click **New Lot**
   to create a new lot for the material of the bin.
3. *(Optional)* Set *quantity is absolute*, **balancing mode**, **movement reason**,
   **contractual partner** and a **comment**.
4. Click **Post Inward Movement**.
   * If the bin is connected to a plant component, the system asks whether the movement
     should run in **automatic mode**:
     * **Yes** — a picking order may be created and the plant workflow started
       (the system may ask to save the selected process workflow as default for the bin).
     * **No** — the movement is booked directly.
5. After a successful booking the booking mask is reset and the quant list is refreshed.
   Errors and warnings appear in the **Messages** panel.

## Posting an outward movement (goods issue)

On the **Outward Posting** tab:

1. Enter the **Quantity** and unit, plus optional values (lot, balancing mode,
   movement reason, contractual partner, comment).
2. Click **Post Outward Movement**. The stock is booked out of the bin.

## Relocating stock (stock transfer)

On the **Relocation** tab:

1. Enter the **Quantity** to move.
2. Select the **target bin** (the list can be restricted with *Only show bins with material*).
3. *(Optional)* Set lot, balancing mode, movement reason, comment.
4. Click **Post Stock Transfer**.
   * With a connected plant component the transport can again run in **automatic mode**
     (picking order / workflow).
   * If the material requires **charge reservation**, the **quant selection dialog**
     opens: select the quants and adjust the **relocation quantity** per quant
     (the entered total quantity is pre-distributed to the available quants), then
     confirm with **OK**.
5. To move the **complete stock** of the bin instead, use **Fully relocate source bin**
   (available in the command menu) and confirm the question with **Yes**.

## Locking and releasing stock

On the **Releasing/Locking** tab (the current release state is shown read-only):

* **Lock** — locks the stock (no further bookings on it).
* **Lock Absolute** — absolute lock; can only be undone explicitly.
* **Unlock** — releases the stock again.
* **Release Absolute** — releases an absolute lock.
* **Adjust Storage Bin** — starts the stock adjustment (matching) of the bin.

## Availability (silo functions)

On the **Availability** tab:

* **Silo empty** — marks the bin as *not available* (e.g. the silo is physically empty
  although the booked stock is not zero).
* **Silo not empty** — resets the state. The system asks whether the **last stock**
  should be restored (**Yes**) or only the flag reset (**No**).

## Reassigning the material

On the **Reassignment** tab:

1. Select the **new material** and, if required, the **new BOM**.
2. Enter a **comment** if needed.
3. Click **Reassign Material** — the stock of the bin is booked to the new material/BOM.

## Navigating from a quant

Select a quant in the **Quants** tab (or in the quant selection dialog) and use the
context menu / command menu:

* **Manage Quant** — opens the quant management dialog.
* **Manage Lot/Batch** — opens the lot management for the lot of the quant.
* **Show Lot Stock and History** — lot stock and history overview.
* **Show Material Stock and History** — material stock and history overview.
* **Show Bin Stock and History** — bin stock and history overview (for the selected bin).
* **Show production order** — opens the production order the quant belongs to
  (enabled only if the quant originates from an order).

## Field reference (header)

| Field | Description |
|---|---|
| Bin number / Bin name | Identification of the current bin (read-only) |
| Material | Material of the bin; editable only while the bin has no quants and nothing is being discharged into it |
| BOM (Partslist) | BOM of the material used for production bookings |
| Stock quantity / Reserved / Available | Stock figures of the bin (with unit) |
| Not available | Bin is marked as not available (see *Availability*) |
| Release state | Current release state of the stock (read-only) |
| Tolerance | Tolerance of the bin |
| Inward enabled / Outward enabled | Booking allowed in/out of this bin |
| Stack calculator | Assigned stack calculator component |
| Comment | Free text on the bin |

**Booking mask fields:** Quantity (with unit), Lot, Quantity is absolute, Balancing
mode, Movement reason, Contractual partner, Comment, Shift booking reverse.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityBookCell` contains
the complete usage recipe (search → navigate → edit → save, plus all booking workflows),
so an agent can operate the app remotely, e.g. for user training.

Method ↔ button caption mapping (from the `ACMethod*` attributes):

| C# method | Button caption (en / de) |
|---|---|
| `Search()` | Search / Suchen |
| `Load()` | Load / Laden |
| `Save()` | Save / Speichern |
| `UndoSave()` | Undo / Nicht speichern |
| `ClearBookingData()` | New Bookingrecord / Neuer Buchungssatz |
| `InwardFacilityMovement()` | Post Inward Movement / Buche Lagerzugang |
| `InwardFacilityLotGenerateDlg()` | New Lot / Neues Los |
| `OutwardFacilityMovement()` | Post Outward Movement / Buche Lagerabgang |
| `FacilityRelocation()` | Post Stock Transfer / Buche Umlagerung |
| `CompleteFacilityRelocation()` | Fully relocate source bin / Quellzelle vollständig umlagern |
| `LockFacility()` | Lock / Sperren |
| `LockFacilityAbsolute()` | Lock Absolute / Absolut sperren |
| `ReleaseFacility()` | Unlock / Freigeben |
| `ReleaseFacilityAbsolute()` | Release Absolute / Absolut freigeben |
| `NotAvailableFacility()` | Silo empty / Silo leer |
| `AvailableFacility()` | Silo not empty / Silo nicht leer |
| `StartFacilityAdjust()` | Adjust Storage Bin / Abgleich Lagerplatz |
| `FacilityReassign()` | Reassign Material / Material neu zuordnen |
| `DlgSelectChargeOk()` / `DlgSelectChargeCancel()` | OK / Cancel (quant selection dialog) |
| `DialogOK()` / `DialogCancel()` | OK / Cancel (facility dialog) |
| `NavigateToFacilityCharge()` | Manage Quant / Quant verwalten |
| `NavigateToFacilityLot()` | Manage Lot/Batch / Verwalte Los/Charge |
| `NavigateToFacilityOverview()` | Show Bin Stock and History / Zeige Behälterbestand und Historie |
| `NavigateToFacilityLotOverview()` | Show Lot Stock and History / Zeige Losbestand und Historie |
| `NavigateToMaterialOverview()` | Show Material Stock and History / Zeige Materialbestand und Historie |
| `NavigateToOrder()` | Show production order (ConstApp.ShowProdOrder) |
| `OnActivate(page)` | Activate / Aktivieren (tab switching) |
