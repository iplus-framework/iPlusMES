# Bin Selection (Behältnis auswählen)

> Business object: `BSOBinSelection` (Assembly `gip.bso.manufacturing`,
> operated at a **work center** — child of the Work Center Selector)

## Definition

**Bin Selection** is a work center dialog for selecting a free **preparation bin**
(container) in which the material of the current production step shall be weighed or
filled. The dialog is opened automatically by the bin selection workflow node
(`PWBinSelection`) of the process module when a free container is required.

The instruction shown to the user is:
*"Please select a free container for weighing or scan the container with your mobile
computer."*

## Concepts

| Term | Meaning |
|---|---|
| **Preparation bin** | Container of facility type *PreparationBin* used at the weighing/filling station |
| **Selection list** | The containers that can be selected, with last inward/outward booking and reserved flag |
| **Reserved** | The container is already reserved by another order and cannot be selected |
| **Bin discharging (free up)** | Function to free a reserved container (see below) |

Depending on the source type of the production step the list shows either

* **storage bins** (source type *FacilityID*): all preparation bins, or
* **facility charges** (source type *FacilityChargeID*): all available charges of the
  batch material without facility lot.

## Screen layout

| Area | Content |
|---|---|
| Header | Work center caption, program no., batch sequence no., material name |
| **Info** | Message area (result of the selection) |
| Container list | Facility no., facility name (and last bookings) |
| **Functions** | **Code** input field with the **Select** button and the **Bin discharging** button |

## Step-by-step tasks

### Selecting a container

1. Click the desired container in the list (double-click) — **or** scan the container
   with the mobile computer — **or** enter the container code (GUID) in the **Code**
   field and click **Select**.
2. The selection is forwarded to the process function and the workflow continues.
   The result is shown in the **Info** area.

### Freeing a reserved container (Bin discharging)

If a container is reserved but its content is no longer needed:

1. Select the reserved container in the list.
2. Click **Bin discharging**.
3. Answer the question whether the container is **used in a production order**:
   * **Yes** — the container stays reserved for the production order.
   * **No** — answer whether the content shall be **scrapped**:
     * **Yes** — the content is written off to the scrap storage bin.
     * **No** — the content is booked **back to stock**.
4. The bookings are executed and the list is reloaded.

### Breaking the selection

Click **Break** to cancel the bin selection; the workflow node is reset.

## Field reference

| Field | Description |
|---|---|
| Facility no. / Facility name | Container in the selection list |
| Code | GUID of the container to select (input field) |
| Info | Message of the last action |
| Reserved | Container is reserved by another order (cannot be selected) |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOBinSelection` contains the
complete usage recipe. Entry points: `SendCode()` (button **Select**),
`DblClickBin()` (list double-click), `BreakBinSelection()` (button **Break**) and
`BinFreeUp()` (button **Bin discharging**). The list is provided in `BinFacilityList`
(`SelectedBinFacility` = selection), the code input is `InputSourceCode`, the current
batch position is `EndBatchPos`.
