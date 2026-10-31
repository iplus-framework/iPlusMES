# Storage Location (Storage Locations and Storage Bins)

> Business object: `BSOFacilityLocation` (Assembly `gip.bso.masterdata`, Menu: **Master Data → Storage Location**)

## Definition

A **storage location** is a top-level storage object (facility type *Storage Location*)
that holds **storage bins** (facility type *Storage Bin*) as child facilities. This app
manages storage locations and — on the *Storage Bin* tab — the storage bins belonging
to the selected storage location.

Only storage locations **without a parent facility** are listed; the storage bins are
managed per storage location on their own tab.

## Concepts

| Term | Meaning |
|---|---|
| **Storage Location** | Top-level facility of the type *Storage Location*; the record edited in the header of this app |
| **Storage Bin** | Child facility of a storage location; the physical place where facility charges (stock) are stored |
| **Reservations** | System-maintained reserved and ordered quantities of the storage location |
| **Posting behaviour** | How stock postings are handled for this facility |

## Screen layout

The app is a search list on the left (**Explorer**) and a detail form on the right
(**MainForm**). The header shows **Number**, **Name** and the **Facility Type**
(read-only, always *Storage Location*). The detail form has these tabs:

| Tab | Content |
|---|---|
| **Properties** | Configuration group (Configuration, Locked Facility, Incoming Facility, Outgoing Facility) and Reservations group (Reserved Quantity, Ordered Quantity, Posting behaviour) |
| **Storage Bin** | Storage bin list (left) with the buttons **Load Storage Bin**, **New Storage Bin**, **Delete Storage Bin**, and the storage bin detail form (right) |

**Storage bin detail form fields:** Number, Name, Configuration, Facility Type,
Stack Calculator, Inward Enabled, Outward Enabled, Min. Stock Quantity, Opt. Stock
Quantity, Max. Volume Capacity, Max. Weight Capacity, Distance Front, Distance Back,
Fill level validation flags, Post remaining quantity to order on emptying, Disabled for
mobile application, Skip print question, Leave material occupation, Posting behaviour,
Key of ext. system, Classification code.

## Step-by-step tasks

### Searching and editing a storage location

1. Enter a search word in the search field and click **Search** — the list shows all
   storage locations whose number or name contains the word.
2. Select a storage location in the list and click **Load** (or navigate) — the record
   is loaded into the header for editing.
3. Change the fields on the **Properties** tab.
4. Click **Save**.
5. Click **Undo** to discard unsaved changes, or **Load** to re-read the record if
   another user changed it in the background.

### Creating a new storage location

1. Click **Create new record**. A new storage location is created and opened.
2. Enter the **Number** and **Name**.
3. Click **Save**.

### Deleting a storage location

1. Select the storage location and click **Delete current record**.
   The delete is rejected if foreign key relationships exist from other tables.
2. On success the first remaining record is selected and loaded.

### Creating a new storage bin

1. Select the storage location on the **Storage Bin** tab.
2. Click **New Storage Bin** — a new storage bin is created as a child of this storage
   location and opened in the detail form.
3. Enter the **Number** and **Name** and set the capacities/flags as needed.
4. Click **Save**.

### Editing or deleting a storage bin

1. Select the storage bin in the list and click **Load Storage Bin** to open it in the
   detail form.
2. Change the fields and click **Save**.
3. **Delete Storage Bin** deletes the opened storage bin. If foreign key references
   exist, a confirmation question appears — answering **Yes** deletes the record anyway.

## Field reference (storage location header)

| Field | Description |
|---|---|
| Number | Unique number of the storage location |
| Name | Name of the storage location |
| Facility Type | Read-only; always *Storage Location* |
| Configuration | AC class (configuration) of the facility |
| Locked Facility | Reference to a locked facility |
| Incoming Facility | Default incoming facility for postings |
| Outgoing Facility | Default outgoing facility for postings |
| Reserved Quantity | System-maintained reserved quantity (read-only) |
| Ordered Quantity | System-maintained ordered quantity (read-only) |
| Posting behaviour | How postings are handled |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityLocation` contains
the complete usage recipe (search → select → `Load` → edit `CurrentStorageLocation` →
save), including the C# method names (`Search`, `Load`, `New`, `Delete`, `Save`,
`UndoSave`, `LoadStorageBin`, `NewStorageBin`, `DeleteStorageBin`), so an agent can
operate the app remotely, e.g. for user training. The button captions above are the
translations of these methods (`ACMethod*` attributes, stored in `ACClassText`).
