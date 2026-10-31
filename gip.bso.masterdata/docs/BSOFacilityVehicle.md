# Vehicle (Vehicles and Chambers)

> Business object: `BSOFacilityVehicle` (Assembly `gip.bso.masterdata`, Menu: **Master Data → Vehicle**)

## Definition

A **vehicle** is a mobile storage object (facility type *Vehicle*) — e.g. a truck, a
tanker or a container vehicle. A vehicle can have **chambers** (facility type
*Vehicle Container*): independent compartments of the vehicle, each of which can hold
stock (facility charges) and be loaded/unloaded separately.

This app manages vehicles and their chambers, the vehicle measurements and the
assignment of the **hauling company** and **driver**.

## Concepts

| Term | Meaning |
|---|---|
| **Vehicle** | Facility of the type *Vehicle*; the record edited in the header of this app |
| **Chamber (Vehicle Container)** | Child facility of a vehicle; an independent compartment with its own stock |
| **Vehicle Type** | Classification of the vehicle (master data *MDFacilityVehicleType*) |
| **Haulier** | The hauling company (Company) assigned to the vehicle |
| **Driver** | The driver (CompanyPerson) assigned to the vehicle |

## Screen layout

The app is a search list (docked **Explorer**) and a detail form (**MainForm**). The
header shows **Number**, **Name**, the **Facility Type** (read-only, always *Vehicle*)
and the **Vehicle Type**. The detail form has these tabs:

| Tab | Content |
|---|---|
| **Properties** | Configuration group (Configuration, Tara, Max. Weight Capacity, Inward Enabled, Outward Enabled) and Measurement group (Fittings Distance Front, Fittings Distance Back) |
| **Haulier** | Company (name, address) and Driver (name, address) of the vehicle |
| **Vehicle Container** | Chamber list (left) with the buttons **Load Chamber**, **New Chamber**, **Delete Chamber**, and the chamber detail form (right) |

**Chamber detail form fields:** Number, Name, Configuration, Facility Type, Inward
Enabled, Outward Enabled, Charge Sort No., and the Measurement group (Max. Weight
Capacity, Max. Volume Capacity, Empty Tolerance).

## Step-by-step tasks

### Searching and editing a vehicle

1. Enter a search word in the search field and click **Search** — the list shows all
   vehicles whose number or name contains the word.
2. Select a vehicle in the list and click **Load** (or navigate) — the record is loaded
   into the header for editing.
3. Change the fields on the **Properties** tab (measurements, capacities, posting
   flags) or assign company/driver on the **Haulier** tab.
4. Click **Save**.
5. Click **Undo** to discard unsaved changes, or **Load** to re-read the record if
   another user changed it in the background.

### Creating a new vehicle

1. Click **Create new record**. A new vehicle is created below the root store for
   vehicles; inward and outward posting are enabled automatically.
2. Enter the **Number** and **Name** and select the **Vehicle Type**.
3. Click **Save**.

### Deleting a vehicle

1. Select the vehicle and click **Delete current record**.
   The delete is rejected if foreign key relationships exist from other tables.
2. On success the first remaining record is selected and loaded.

### Creating a new chamber

1. Select the vehicle on the **Vehicle Container** tab.
2. Click **New Chamber** — a new chamber is created as a child of this vehicle. It
   inherits the vehicle type and has inward and outward posting enabled.
3. Enter the **Number** and **Name** and set the measurement fields.
4. Click **Save**.

### Editing or deleting a chamber

1. Select the chamber in the list and click **Load Chamber** to open it in the detail
   form.
2. Change the fields and click **Save**.
3. **Delete Chamber** deletes the opened chamber.

## Field reference (vehicle header)

| Field | Description |
|---|---|
| Number | Unique number of the vehicle |
| Name | Name of the vehicle |
| Facility Type | Read-only; always *Vehicle* |
| Vehicle Type | Classification of the vehicle |
| Configuration | AC class (configuration) of the vehicle |
| Tara | Tara (unloaded weight) |
| Max. Weight Capacity | Maximum weight capacity |
| Inward Enabled / Outward Enabled | Stock may be booked in/out of the vehicle |
| Fittings Distance Front / Back | Distance of the fittings front/back (for level measurement) |
| Company / Driver (Haulier tab) | Hauling company and driver with their addresses |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityVehicle` contains
the complete usage recipe (search → select → `Load` → edit `CurrentVehicle` → save),
including the C# method names (`Search`, `Load`, `New`, `Delete`, `Save`, `UndoSave`,
`LoadVehicleContainer`, `NewVehicleContainer`, `DeleteVehicleContainer`), so an agent
can operate the app remotely, e.g. for user training. The button captions above are the
translations of these methods (`ACMethod*` attributes, stored in `ACClassText`).
