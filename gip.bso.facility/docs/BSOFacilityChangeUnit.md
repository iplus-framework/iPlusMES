# Change Facility Units (Change Storage Unit)

> Business object: `BSOFacilityChangeUnit` (Assembly `gip.bso.facility`, started via the **Businessobject-Manager / Application Object Manager**)

## Definition

The **Change Facility Units** app converts the **unit of measure** of existing stock
charges. A charge quantity stored in one unit (e.g. kg) can be converted into another
storage unit (e.g. pcs) by posting a "Change storage unit" booking. The conversion is
executed as a facility booking with the parameters entered on the screen.

## Concepts

| Term | Meaning |
|---|---|
| **Charge (FacilityCharge)** | A stock record: material + lot on a storage bin with a quantity |
| **Storage unit** | Unit of measure in which the charge quantity is managed |
| **Booking parameters** | The posting parameter set (e.g. the material) that the **Change Unit** command acts on |

## Screen layout

The app consists of a dockable **Explorer** window (search list, auto-hide at the
bottom) and the main form:

| Area | Content |
|---|---|
| Header | **Change Facility Units** |
| **Material** group | **Material** selection combo box (booking parameter) |
| Tab **Facility units** | Group **Bookingparameter** with the button **Change Unit** |

## Step-by-step tasks

### Searching materials

1. Enter a search string in the search field.
2. Click **Search** — the matching materials appear in the Explorer list.
3. Select a row to make it the current record.

### Changing the storage unit of a charge

1. Select the **Material** in the **Material** group.
2. Fill the remaining booking parameters (charge, target unit/quantity) in the
   **Bookingparameter** group.
3. Click **Change Unit** — the quantity of the charge is converted into the new storage
   unit and posted as a booking record.

### Resetting the entry mask

* Click **New** to clear all booking parameters and start a fresh entry. This does
  **not** create a database record. (The app does this automatically on start-up.)
* Click **Load** to re-read the current record if another user changed it in the
  background.

## Field reference

| Field | Description |
|---|---|
| Material | Material of the charge whose unit is changed (booking parameter) |
| Charge / target unit / quantity | Part of the booking parameter set; posted by **Change Unit** |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityChangeUnit`
contains the complete usage recipe (search → navigate → edit booking parameters →
execute booking command). The button captions above are the translations of the
command methods (`ACMethod*` attributes, stored in `ACClassText`), e.g.
**Change Unit** → `ChangeUnit()`.
