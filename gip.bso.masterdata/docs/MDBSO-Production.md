# Production Master Data — Value Lists (MDBSO)

> Business objects: `MDBSO*` (Assembly `gip.bso.masterdata`, Module **Production**)

## What are master-data value lists?

Value lists are small, centrally maintained tables whose entries appear as
selections (combo boxes, status columns) in the operational screens of the
Production module — for example the status of a production order or the
reaction to a process error. Each entry has a **multilingual name**, a
**sort index** and an optional **default flag**; some lists also carry a
technical **key** used by the system.

## Standard operation (identical for all lists in this document)

Every value list is maintained in the same way:

1. **Search** — loads all entries of the list into the table on the left.
   (These simple lists have no search-word filter; the **Search** button
   refreshes the complete list.)
2. **New** — creates a new entry and makes it the current record. Fill in
   all required fields (at least the multilingual **Name**).
3. **Edit** — select an entry in the list and change its fields on the right.
4. **Save** — writes the changes to the database. Always save before
   navigating to another entry or creating a new one; **Undo** discards
   unsaved changes.
5. **Delete** — removes the current entry. This is only possible if no other
   tables reference the entry (foreign key check); confirm with **Save**.
6. **Load** — reloads the selected entry from the database if another user
   has changed it in the background.

> **Caution:** entries are referenced by operational records (production
> orders, batches, BOM positions …). Deleting an entry that is still in use
> is rejected by the system — retire it instead of deleting it if possible.

---

## Demand Order State (`MDBSODemandOrderState`)

Status of a **purchase requisition (demand order)**; tracks whether a
material requirement is still a proposal or already being processed.

| Value | Meaning |
|---|---|
| Proposal | Requirement is only a proposal, not yet confirmed |
| Active | Requirement is active and being processed |
| Inactive | Requirement is parked/inactive |
| Finished | Requirement has been completed |

## Process Error Action (`MDBSOProcessErrorAction`)

Defines **what happens when a process error occurs** during production
(used by process modules and error handlers).

| Value | Meaning |
|---|---|
| Warning Only *(default)* | The error is only reported as a warning; the process continues |
| Block Process | The process is blocked until the error is cleared |

## Prod. Order BOM Position Status (`MDBSOProdOrderPartslistPosState`)

Status of a **BOM (partslist) position of a production order**; tracks the
production progress of each component line.

| Value | Meaning |
|---|---|
| Created | Position has been created, not yet started |
| In Progress | Position is currently being produced |
| Completed | Position has been produced completely |
| Partially Completed | Position has been produced partially (German caption only) |
| Blocked | Position is blocked |
| Auto Start | Position is started automatically (German caption only) |
| Cancelled | Position was cancelled |

## Production Order Status (`MDBSOProdOrderState`)

Status of a **production order** as a whole; tracks the classic production
workflow from creation to rating.

| Value | Meaning |
|---|---|
| New Created *(default)* | Production order has been created |
| Prepare Batch | Batch preparation is running |
| Batch Prepared | Batches are prepared, production can start |
| In Production | Production is currently running |
| Production Finished | Production has finished |
| Rated | Order has been rated/settled |
| Cancelled | Production order was cancelled |

## Tolerance Status (`MDBSOToleranceState`)

Result of the **tolerance check** of a production order/batch (target vs.
actual quantities).

| Value | Meaning |
|---|---|
| No Tolerance Overruns *(default)* | All quantities are within tolerance |
| Tolerance Overruns Exists | At least one tolerance was exceeded |

---

## For AI agents (MCP)

The metadata of these business objects is exposed through the iPlus MCP
Server (`get_thesaurus`, `get_type_infos`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of each `MDBSO*` class
contains the complete usage recipe (Search → list → `Current<Entity>` →
Save/New/Delete/Load), so an agent can operate the value lists remotely, e.g.
for user training or data maintenance.
