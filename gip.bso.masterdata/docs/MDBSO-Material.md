# Material Master Data — Value Lists (MDBSO)

> Business objects: `MDBSO*` (Assembly `gip.bso.masterdata`, Module **Material**)

## What are master-data value lists?

Value lists are small, centrally maintained tables whose entries appear as
selections (combo boxes, status columns, classification fields) in the
operational screens of the Material module — for example the group or type of
a material, the status of a laboratory order or the unit of a quantity. Each
entry has a **name** (multilingual), a **sort index** and an optional
**default flag**; some lists also carry a technical **key** used by the system.

## Standard operation (identical for all lists in this document)

Every value list is maintained in the same way:

1. **Search** — enter a search word in the search field; the result appears in
   the list on the left.
2. **New** — creates a new entry and makes it the current record. Fill in all
   required fields (at least the **Name**).
3. **Edit** — select an entry in the list and change its fields on the right.
4. **Save** — writes the changes to the database. Always save before
   navigating to another entry or creating a new one; **Undo** discards
   unsaved changes.
5. **Delete** — removes the current entry. This is only possible if no other
   tables reference the entry (foreign key check); confirm with **Save**.
6. **Load** — reloads the selected entry from the database if another user has
   changed it in the background.

> **Caution:** entries are referenced by materials, lab orders and other
> operational records. Deleting an entry that is still in use is rejected by
> the system — retire it instead of deleting it if possible.

---

## GMP Additive (`MDBSOGMPAdditive`)

Value list of **GMP/HACCP ingredients** (additives) that can be assigned to
materials for GMP/HACCP documentation. Each additive carries a **safety
factor** and a **process error action** that define how the system reacts when
the additive limit is exceeded, plus a **delete level**.

**Values in the sample database:**

| Value | Meaning |
|---|---|
| Dioxin | Contaminant dioxin |
| Bacteria | Bacteria (microbiological load) |
| Antibiotics | Antibiotics residue |
| Hormone | Hormones residue |

**Fields:** Number, Name, Delete level, Quantity unit, Safety factor, Process
error action.

## GMP Material Group (`MDBSOGMPMaterialGroup`)

Value list of **GMP material groups** — animal species groups used to group
materials for GMP/HACCP documentation. Each group can contain a list of
**ingredients** that apply to all materials of the group.

**Special functions:**

* **New Ingredient** — adds an ingredient position to the current group.
* **Delete Ingredient** — removes the selected ingredient position from the
  group.
* **Load Ingredient** — loads the selected ingredient position for editing.

**Values in the sample database:**

| Value | Meaning |
|---|---|
| Milchvieh | Dairy cattle |
| Schweine | Pigs |
| Geflügel | Poultry |
| Pferde | Horses |

**Fields:** Number, Name, Wear facility number.

## Laboratory Order Pos. Status (`MDBSOLabOrderPosState`)

Status of a **laboratory order position**; tracks the processing state of a
single lab test position.

**Values in the sample database:** none — the list is empty and can be filled
by the customer.

**Fields:** Status index, Name, Sort index, Is default.

## Laboratory Order Status (`MDBSOLabOrderState`)

Status of a **laboratory order** as a whole; tracks the processing state of
the lab order.

| Value | Meaning |
|---|---|
| New *(default)* | Lab order created, not yet processed |
| In Process | Lab order is being processed |
| Finished | Lab order is finished |
| Declined | Lab order was declined |

## Laboratory Tag (`MDBSOLabTag`)

Value list of **laboratory tags** — lab test characteristics that can be
assigned to lab orders and their positions.

| Value | Meaning |
|---|---|
| pH | pH value measurement |
| Color *(default)* | Color inspection |
| Sample weight | Stored sample weight |

**Fields:** Tag index, Name, Sort index, Is default.

## Material Group (`MDBSOMaterialGroup`)

Value list of **material groups** that classify materials by their role in the
value chain. The group of a material controls e.g. procurement, production and
sales behaviour.

**Special function:** the **Show images** configuration switches the display
of group images on or off.

**Values in the sample database:**

| Value | Meaning |
|---|---|
| Raw Material | Raw material (input for production) |
| Intermediate Product | Intermediate product |
| Semi Finished Product | Semi-finished product |
| Finished Product | Finished product |
| Packaging Means | Packaging material |
| Means of Transport | Transport equipment (pallets, containers …) |
| Misc. Goods | Miscellaneous goods |
| Merchandise | Goods purchased for resale |
| Service | Service (no physical stock) |
| Text Article | Text-only article |
| GIP-Licence *(default)* | Licence article |
| Rework | Rework material |
| Picking | Picking material |

**Fields:** Group index, Name, "Is not percentage" flag, Sort index, Is
default.

## Material Type (`MDBSOMaterialType`)

Value list of **material types** that further classify materials within a
material group (customer-specific classification, e.g. for reporting or
filtering).

**Values in the sample database:** only demo entries (*Test 1*, *demo*) — the
list is intended to be filled by the customer.

**Fields:** Type index, Name, Sort index, Is default.

## Units of Measurement (`MDBSOUnit`)

Value list of **units of measurement** used wherever quantities are entered or
displayed: SI units (mass, length, volume, time), packaging units and
percent. Each unit has a **symbol**, a **technical symbol**, an **ISO code**
and a **rounding** setting. Units can have **conversion rules** to other units
(multiplier/divisor).

**Special functions:**

* **New Unit Conversion** — opens a dialog to define a conversion rule from
  the current unit to another unit (target unit, multiplier, divisor);
  confirm with **OK**, discard with **Cancel**.
* **Delete Unit Conversion** — removes the selected conversion rule.
* **Convert** — conversion test: converts a sample **Input** value from the
  current unit to the selected **To Unit** and shows the **Output**.

**Values in the sample database (excerpt):**

| Unit | Symbol | SI dimension | Notes |
|---|---|---|---|
| kilogram | kg | Mass | SI unit |
| ton | t | Mass | |
| gram | g | Mass | |
| metre | m | Length | SI unit |
| centimetre | cm | Length | |
| Kilometre | km | Length | |
| Litre | l | Volume | SI unit |
| Hectare | ha | Area | |
| Second / Minute / Hour | s / min / h | Time | |
| percent | % | — | |
| Sack 25kg / Sack 50kg | SAK25 / SAK50 | — | Packaging units with ISO code SA(25)/SA(50) |
| piece | PCE | — | |
| Package, Bag, Parcel, carton, Big bag, Can, Bottle, Bucket, Drum, Container, Coil, Box, Pallet | … | — | Packaging units |

**Fields:** Name, Symbol, Technical symbol, SI dimension, Is SI unit,
Rounding, Is quantity unit, ISO code, Sort index.

---

## For AI agents (MCP)

The metadata of these business objects is exposed through the iPlus MCP
Server (`get_thesaurus`, `get_type_infos`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of each `MDBSO*` class
contains the complete usage recipe (search → list → `Current<Entity>` →
Save/New/Delete/Load), so an agent can operate the value lists remotely, e.g.
for user training or data maintenance.
