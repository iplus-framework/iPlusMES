# Receipt Weighing (Vehicle Scale)

> Business object: `BSOVehicleWeighing` (Assembly `gip.bso.masterdata`, Query: **Weighing**)

> ⚠️ **Status: work in progress.** This app is a skeleton (see the TODO remarks in the source
> code): **New**, **Delete** and **Reset Scale** have no effect yet, **Load** is permanently
> disabled, and the weighing form fields are not connected. Only **Search** and the list
> navigation currently work.

## Definition

The **Receipt Weighing** app is used to weigh incoming deliveries on a **vehicle scale**
(Fahrzeugwaage): a truck is weighed before and after unloading, and the system calculates the
**net weight** of the received goods and assigns it to an incoming delivery note position.

## Concepts

| Term | Meaning |
|---|---|
| **Weighing** | One weighing record for an incoming delivery (table `InDeliveryNotePosInWeighing`) |
| **First weighing / Second weighing** | Truck weight before and after unloading (with identity numbers) |
| **Net weight** | Difference between first and second weighing, minus **loss** |
| **Incoming delivery note position** | The delivery line the weighed quantity is posted to |
| **Negative remittance advice** | Flag on the weighing record |

## Screen layout

The app is a search list on the left (**Explorer**) and the weighing form on the right
(**Wheiging**, header "Vehicle scale") with these fields:

| Field | Description |
|---|---|
| Weighing No | Number of the weighing record |
| Start Date / End Date | Start and end of the weighing procedure |
| First Weighing / Second Weighing | The two weighings with their identity numbers |
| Net Weight | Calculated net weight |
| Loss | Deducted loss |
| Actual Weight Inventory | Current scale weight |
| Shipper Company | Supplier / shipper |
| Incoming Delivery Note Position | Delivery line the quantity is assigned to |
| Material | Material of the delivery line |
| Area / Storage Bin / Vehicle | Storage area, storage bin and vehicle of the delivery |
| Comment | Free comment |
| Negative Remittance Advice | Flag |

## Usage (step by step)

1. Click **Search** to load the weighing records into the list.
2. Select a record in the list to show it in the weighing form.
3. **Save** stores changes to the current record; **Undo** discards them.

> **Note:** **New**, **Delete**, **Load** and **Reset Scale** are currently without function
> (see status warning above).

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOVehicleWeighing` contains the
complete usage recipe **including the current limitations**: `New()`, `Delete()` and
`ResetWheigingMachine()` are stubs (entity operations commented out), `IsEnabledLoad()`
returns `false`, and the form properties (`CurrentInDeliveryNotePosInWeighing`) are commented
out. Functionally available: `Search()` (fills the navigation list of `AccessPrimary`, query
`Weighing` over `InDeliveryNotePosInWeighing`), the navigation methods of the base class
(`NavigateFirst()`, `NavigateNext()`, ... via `AccessPrimary.CurrentNavObject`) and
`Save()`/`UndoSave()` (delegating to `OnSave()`/`OnUndoSave()`).
