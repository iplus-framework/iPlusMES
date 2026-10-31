# Material (Material Master Data)

> Business object: `BSOMaterial` (Assembly `gip.bso.masterdata`, Menu: **Master Data → Material**)

## Definition

The material master data contains all required information for a material in one
data record.

## Usage

You can enter and change most of the data contained in a material master record.
There is also information that is created only by the system (created/updated by,
dates).

## Structure — classification of materials

**By intended use**

* Raw materials
* Intermediate material (not completely finished; later processed into finished products)
* Finished products
* *Mixed* materials exist only for a limited time during production as an intermediate
  stage on the way to the final product

**By warehouse management**

* **Without stock management** — e.g. water: only quantity matters, no input orders or lots
* **With stock management but without lot** — lot management not required
* **Stock management and lot** — most common case: stock keeping with lot tracking;
  the lot is part of the final product lot composition

## Creating a new Material

1. Click **New** in the toolbar.
2. Enter a **Material Number** and a **Material Description**.
   **Note:** material numbers must be unique.
3. *(Optional)* Further entries and general conditions can be freely selected and assigned.
4. **Save** your entries.

## Working with records

* **Search**: enter a search string in the search field; results appear in the material list.
* **Edit**: change fields on the current record, then **Save** before navigating to the
  next record or creating a new one.
* **Delete**: use **Delete** only if no foreign key relationships exist from other tables;
  always call **Save** afterwards to execute the delete in the database.
* **Load**: refreshes the current record if another user changed it in the background.

## Field reference

| Field | Type | Description |
|---|---|---|
| Material number | Text | Unique number of the material |
| Material description | Text | Description (several language columns) |
| Material group | Selection | Grouping of similar materials |
| SI Unit | Selection | SI unit of the material |
| Net weight | Number | Total weight excluding packaging |
| Gross weight | Number | Total weight including packaging |
| Density | Number | Mass density (weight per volume) |
| Comment | Text | User comment |
| IsEnabled | Boolean | Material is active and can be used |
| IsIntermediate | Boolean | Product that is not completely finished and is later processed into finished products |
| Created/Updated by, dates | Text/Date | System information |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOMaterial` contains the
complete usage recipe, so an agent can operate the Material app remotely, e.g. for
user training.
