# Bin Discharging (Gebinde entleeren)

> Business object: `BSOBinDischarging` (Assembly `gip.bso.manufacturing`,
> operated at a **work center** — child of the Work Center Selector)

## Definition

**Bin Discharging** is a work center dialog for **emptying preparation bins**. The
containers that must be emptied for the current production step are listed; the user
empties them one after the other and confirms each discharge. The dialog is driven by
the bin discharging workflow node (`PAFBinDischarging`) of the process module.

The instruction shown to the user is:
*"The containers shown below must be emptied. Please scan the containers in order and,
if confirmed positively on the display, empty the container. If you do not have a
scanner available, click on the container that you have emptied."*

## Concepts

| Term | Meaning |
|---|---|
| **Discharging item** | One container to be emptied, with lot, inward booking (*Filled*) and outward booking (*Discharged*) |
| **Filled** | Quantity booked into the container (inward booking) |
| **Discharged** | Quantity already booked out of the container (outward booking) |
| **Tolerance check weighing** | Optional check weighing after discharging; on failure the tolerance dialog is shown |

## Screen layout

| Area | Content |
|---|---|
| Header | Work center caption, program no., batch sequence no., material name |
| **Info** | Instruction text and message area |
| Container list | Item no., lot no., facility (no./name), material (no./name), **Filled** (quantity, booking no., time), **Discharged** (quantity, booking no., time), discharged flag |
| **Code** | Input field with the **Select** button |

Dialog **Tolerance check weighing alarm**: tolerance message with the buttons
**Check weight again**, **Accept tolerance error** and **Cancel**.

## Step-by-step tasks

### Emptying a container

1. Take the next container from the list that is not yet discharged.
2. Scan the container — **or** click it in the list (double-click) — **or** enter its
   code (GUID) in the **Code** field and click **Select**.
3. The discharge of the container is started; when the process function waits for the
   confirmation, the container is preselected automatically.
4. Empty the container and click **Confirm discharge** (or the confirmation on the
   display). The outward booking is written and the container is marked as discharged.

### Resolving a tolerance alarm

If the tolerance check weighing fails, the **Tolerance check weighing alarm** dialog
appears:

* **Check weight again** — weigh again and re-check.
* **Accept tolerance error** — accept the deviation and continue.
* **Cancel** — close the dialog without accepting.

## Field reference

| Field | Description |
|---|---|
| Lot no. | Lot of the container content |
| Facility no. / Facility name | Container (storage bin) |
| Material no. / Material name | Material in the container |
| Filled: quantity, booking no., time | Inward booking (what was filled in) |
| Discharged: quantity, booking no., time | Outward booking (what was taken out) |
| Is discharged | Container is fully discharged |
| Code | GUID of the container to select (input field) |
| Tolerance message | Message of the tolerance check weighing |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOBinDischarging` contains
the complete usage recipe. Entry points: `SendCode()` (button **Select**),
`SelectForDiscarge()` (list double-click), `ConfirmDischarge()` (button
**Confirm discharge**), `CheckWeightAgain()` and `AcceptToleranceError()` (tolerance
dialog) and `Cancel()`. The container list is `DischargingItemList`
(`SelectedDischargingItem` = selection), the code input is `InputSourceCode`, the
tolerance message is `ToleranceMsg`.
