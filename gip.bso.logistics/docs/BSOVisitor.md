# Visitor Registration

> Business object: `BSOVisitor` (Assembly `gip.bso.logistics`, Menu: **Logistics → Visitor Registration**)

## Definition

The visitor registration manages **visitors** on the company premises: who visits
which company, in which time range, with which vehicle and which visitor card.
Each visitor can have **visitor vouchers** (documents, e.g. weighings or delivery
notes belonging to the visit).

## Concepts

| Term | Meaning |
|---|---|
| **Visitor** | The visit record: number, visited company, person, time range, vehicle, card |
| **Visitor voucher** | A document (position) belonging to the visit |
| **Visitor card** | Physical card (master data `MDVisitorCard`) assigned to the visitor for the duration of the visit |
| **Vehicle / Trailer** | Registered vehicles (facilities of the vehicle type) assigned to the visit |
| **Visited company** | The customer company being visited, with its house and delivery addresses |

## Screen layout

The app consists of a search list (left) and a detail form (right) with these
sections/tabs:

| Tab | Content |
|---|---|
| **Person and Company** | Visitor number, visited company, visitor person |
| **Time Range** | Scheduled from / scheduled to, finished flag |
| **Vehicle and Card** | Vehicle, trailer, assigned visitor card |
| **Visits** | Visitor vouchers of the visitor |

Dialogs: **Assign Visitor Card** (card selection with **OK** / **Cancel**) and the
**New Visitor** quick-registration dialog (with **OK** / **Cancel**).

## Registering a new visitor (step by step)

1. Click **New** — a new visitor record with a new visitor number is created.
2. Select the **visited company** and the **visitor person**.
3. Enter the **scheduled time range** (from/to).
4. *(Optional)* Assign the **vehicle** and **trailer**.
5. Click **Save**.

## Assigning a visitor card

1. Select the visitor.
2. Click **Assign Card** — a dialog opens with all **free visitor cards**.
3. Select a card and click **OK** (the card is assigned to the visitor).
   **Cancel** closes the dialog without assigning.
4. Click **Save** to persist the assignment.
5. **Unassign Card** releases the assigned card and saves immediately
   (only enabled when the visitor has a card and there are no unsaved changes).

## Working with visitor vouchers

On the **Visits** tab:

1. Select a voucher in the voucher list.
2. Click **Load Position** to load it as the current voucher.

## Quick registration dialog

The **New Visitor** dialog (callable from other apps) creates a new visitor,
shows the visitor form and returns the result:

* **OK** — returns the new visitor to the caller.
* **Cancel** — deletes the newly created visitor.

## Working with records

* **Search** — refreshes the visitor list and the vehicle list.
* **Load** — reloads the selected visitor if another user changed it in the background.
* **Save** — persists the changes of the current visitor.
* **Undo** ("Don't save") — discards unsaved changes.
* **Delete** — deletes the current visitor (only if no other records reference it).

## Field reference

| Field | Description |
|---|---|
| Visitor No. | Unique number of the visit (generated automatically) |
| Visited company | Customer company being visited |
| Visitor person | Contact person of the visited company |
| Scheduled from / Scheduled to | Planned time range of the visit |
| Finished | The visit is completed |
| Vehicle / Trailer | Assigned vehicle and trailer |
| Visitor card | Assigned visitor card |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOVisitor` contains the
complete usage recipe (search → navigate → edit → save, `New`, `Delete`,
`AssignVisitorCard`, `UnassignVisitorCard`, `ShowDialogNewVisitor`, ...), so an
agent can operate the visitor registration remotely, e.g. for user training.
