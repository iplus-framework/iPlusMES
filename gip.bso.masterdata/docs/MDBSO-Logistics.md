# Logistics Master Data — Value Lists (MDBSO)

> Business objects: `MDBSO*` (Assembly `gip.bso.masterdata`, Module **Logistics**)

## What are master-data value lists?

Value lists are small, centrally maintained tables whose entries appear as
selections (combo boxes, status columns) in the operational screens of the
Logistics module — for example the status of a delivery note or the mode of
transport of a delivery. Each entry has a **name** (multilingual), a **sort
index** and an optional **default flag**; some lists also carry a technical
**key** used by the system.

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

> **Caution:** entries are referenced by operational records (deliveries,
> tour plans, picking orders …). Deleting an entry that is still in use is
> rejected by the system — retire it instead of deleting it if possible.

---

## Batchplan Group (`MDBSOBatchPlanGroup`)

Groups batch planning objects so that production orders/batches can be
planned, filtered and scheduled together.

**Values in the sample database:** none — the list is empty and can be filled
by the customer.

**Fields:** Name, Sort index, Is default.

## Delivery Note Status (`MDBSODelivNoteState`)

Status of a **delivery note** as a whole; tracks the processing state of the
document.

| Value | Meaning |
|---|---|
| Created *(default)* | Delivery note has been created, not yet processed |
| In Process | Delivery note is being processed |
| Completed | Delivery note is finished |
| Cancelled | Delivery note was cancelled |

## Loading Status (`MDBSODelivPosLoadState`)

Loading state of a **delivery position**; tracks the physical loading process
from goods provision to the truck.

| Value | Meaning |
|---|---|
| New created *(default)* | Position created, loading not started |
| Ready to load | Goods are staged and ready for loading |
| Batch active | Batch (charging) is active for this position |
| Loading to forecell | Goods are being loaded into the forecell |
| Loading active | Loading is currently running |
| Loading to Truck | Goods are being loaded onto the truck |

## Delivery Status (`MDBSODelivPosState`)

Delivery state of a **delivery position**; tracks how much of the ordered
quantity has been assigned and delivered.

| Value | Meaning |
|---|---|
| Not Planned *(default)* | Position has not been scheduled yet |
| Subset Assigned | Only part of the quantity has been assigned |
| Completely Assigned | Full quantity has been assigned |
| Delivered | Position has been delivered |

## Delivery Type (`MDBSODelivType`)

Classifies **delivery partners** by direction of goods flow.

| Value | Meaning |
|---|---|
| Supplier | Goods are delivered **to** the company |
| Collector *(default)* | Goods are picked **up** by the customer |

## Picking Type (`MDBSOPickingType`)

Classifies **picking orders / goods movements**.

| Value | Meaning |
|---|---|
| Receipt | Goods receipt |
| Issue | Goods issue |
| Production | Consumption/issue for production |
| Receipt with vehicle | Goods receipt directly with a vehicle |
| Issue with vehicle | Goods issue directly with a vehicle |
| Automatic relocation *(default)* | Relocation triggered automatically by the system |
| Internal relocation | Manual relocation inside the warehouse |
| Unplanned receipt | Goods receipt without a preceding order |
| Return receipt | Return of goods received earlier |
| Retail | Retail sale |
| Internal issue | Internal goods issue |
| Return issue | Return of goods issued earlier |

## Tour (`MDBSOTour`)

A **tour** is a delivery route with a tour number and the weekdays on which it
runs; it is used to schedule deliveries in tour planning.

**Fields:** Tour number, Name, and the weekday flags **Monday** … **Sunday**
(the tour runs on the marked days).

**Values in the sample database:** Tour *1000* — runs Monday, Wednesday and
Saturday.

## Tourplan Position Status (`MDBSOTourplanPosState`)

Status of a **tour plan position**; tracks whether a delivery position has
been assigned to a tour plan.

| Value | Meaning |
|---|---|
| Not Planned *(default)* | Position has not been assigned to a tour plan |

## Tour Plan Status (`MDBSOTourplanState`)

Status of a **tour plan**; tracks the tour planning and loading workflow.

| Value | Meaning |
|---|---|
| New Created | Tour plan created |
| Loading Plan Generated | Loading plan has been generated |
| Loading Plan Active | Loading plan is being executed |
| Loaded | Tour is loaded |
| Delivered | Tour has been delivered |
| Blocked | Tour plan is blocked |
| Cancelled | Tour plan was cancelled |

## Transport Mode (`MDBSOTransportMode`)

Classifies **how goods are transported**.

| Value | Meaning |
|---|---|
| Truck | Road transport (truck) |
| Rail | Rail transport |
| Pipeline | Pipeline |
| Storage Tank | Transfer between storage tanks |
| Barge | Inland waterway (ship/barge) |
| Tank Wagon | Tank wagon (rail tanker) |
| Book Transfer | Book transfer (no physical movement) |
| Container | Container transport |

## Visitor Card (`MDBSOVisitorCard`)

Manages **visitor badge cards** (visitor passes). A card is a physical badge
that can be assigned to a visitor (user) and released again for reuse.

**Special function:**

* **Unassign Card** — releases the current card from the visitor it is
  currently assigned to, so the card can be handed out again. The button is
  only active when the card is assigned and there are no unsaved changes.

**Fields:** Card number, Card key, Status (see *Visitor Card Status*),
assigned user.

**Values in the sample database:** none — cards are created during operation.

## Visitor Card Status (`MDBSOVisitorCardState`)

Status of a **visitor card**; controls whether a card may be used.

| Value | Meaning |
|---|---|
| Not Used | Card is free and can be assigned |
| Active | Card is assigned and in use |
| Blocked | Card is blocked and may not be used |

## Visitor Voucher Status (`MDBSOVisitorVoucherState`)

Status of a **visitor voucher**; tracks the visitor check-in/check-out
workflow.

| Value | Meaning |
|---|---|
| New Created | Voucher created, visitor not yet registered |
| Checked In | Visitor has checked in (registered) |
| Checked Out | Visitor has checked out (deregistered) |

---

## For AI agents (MCP)

The metadata of these business objects is exposed through the iPlus MCP
Server (`get_thesaurus`, `get_type_infos`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of each `MDBSO*` class
contains the complete usage recipe (search → list → `Current<Entity>` →
Save/New/Delete/Load), so an agent can operate the value lists remotely, e.g.
for user training or data maintenance.
