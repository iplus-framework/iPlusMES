# Lab Order

> Business object: `BSOLabOrder` (Assembly `gip.bso.masterdata`, Menu: **Master Data → Lab Order**)

## Definition

A **Lab Order** documents a laboratory test (quality control) in the manufacturing
process. Every lab order is linked to exactly one business object that was sampled:

* an **inbound order position** (goods receipt inspection),
* an **outbound order position** (goods issue inspection),
* a **production order position** (batch/product inspection),
* a **facility lot** (lot/charge inspection), or
* a **picking position**.

The test parameters to be checked are stored as **positions** of the lab order. Each
position refers to a **Laboratory Tag** (measurement tag) with its limit values; the
measured result is entered as the **Actual Value**.

Lab orders are usually created **automatically** — by a workflow node or another module
(e.g. the process module). They can also be created **manually** in this app, always
based on a **Lab Order Template** (see [BSOLabOrderTemplate.md](BSOLabOrderTemplate.md)).

## Concepts

| Term | Meaning |
|---|---|
| **Lab Order** | The test header: order number, reference to the sampled object, material, sample taking date, test date, status |
| **Lab Order Template** | Blueprint from which the positions of a new lab order are copied |
| **Position** | One test parameter: laboratory tag, limit values, reference value, actual value |
| **Laboratory Tag** | Master data definition of a measurement tag (what is measured) |
| **Limit values** | Four limits per position: absolute minimum, minimum, maximum, absolute maximum |
| **Reference Value** | Target/nominal value of the test |
| **Actual Value** | The measured test result entered by the user |
| **Lab Order State** | Status of the whole order (e.g. *New*); **Position State** is the status of a single position |
| **Average Values** | Automatic statistics the BSO calculates over the laboratory tags |

## Screen layout

The app consists of a search **Explorer** (docked, auto-hide on the right) and the main
**Lab Order** form (tab).

**Explorer** (right):

| Area | Content |
|---|---|
| **Search** | Filter fields (**Order No.**, sample taking date **From**/**To**, **Has Delivery Note**, **Material Group**, **Distributor**) and the **Search** button; below it the result list of lab orders |
| **Average Values** | Statistics over laboratory tags: average values per position and per position item |

**Lab Order** (main tab):

| Area | Content |
|---|---|
| Header (upper grid) | Order number, reference to the sampled object (inbound/outbound/production order position or facility lot), the order itself, **Material**, **Template No.**/**Template Name** (read-only, shows the template the order was created from), **Sample Taking Date**, **Test Date**, **Lab Order State**, **Template Name** |
| **Position list** (left) | All test positions with laboratory tag, sequence, limit values and actual value; buttons **New Position** and **Delete Position** |
| **Position form** (right, *Details*) | Detail form of the selected position: laboratory tag, sequence, limit values, **Reference Value**, **Actual Value**, position state, line number, comment |

## Step-by-step tasks

### Searching lab orders

1. Open the **Explorer** (right edge of the window).
2. Enter the **Order No.** and/or restrict the sample taking date with **From**/**To**.
   Additional filters: **Has Delivery Note** (only lab orders connected to an inbound
   delivery note), **Material Group**, **Distributor**.
3. Click **Search** and select a record in the result list.

### Creating a new lab order from a template

1. Click **New**. The **New Lab Order** dialog opens.
2. Select the **position type** the lab order refers to (from position type: inbound
   order position, outbound order position, production order position, facility lot or
   picking position).
3. Select the concrete order/position of that type. The **Material** is taken over
   automatically from the selected object.
4. Select the **Template**. The list offers all lab order templates that match the
   material — exactly one template must be selected.
5. Enter the **Sample Taking Date** and the **Test Date**.
6. Check the **Lab Order State** (usually *New*).
7. Click **Create**. The positions of the template are copied into the new lab order.
   **Cancel** aborts the creation and discards the new record.
8. Click **Save** to store the lab order in the database.

### Recording test results

1. Select the position in the **position list** — the position form shows its details.
2. Enter the measured result in **Actual Value** (the field is focused automatically
   when a position is selected).
3. If necessary, correct the limit values, the **Reference Value**, the position state
   or the comment.
4. Click **Save**.

### Adding and removing positions

In the position list:

* Click **New Position** to add a test parameter. Then assign the **Laboratory Tag**
  and enter the limit values (absolute minimum, minimum, maximum, absolute maximum)
  and the **Reference Value**.
* Select a position and click **Delete Position** to remove it.
* Click **Save** afterwards.

### Viewing and printing a lab order

1. Click **View Lab Order**. A dialog shows the order with its position list and form.
2. Use **Print** to print the lab order.
3. Click **Close** to return.

### Working with records

* **Save** — stores changes of the current lab order (always save before navigating to
  another record or creating a new one).
* **Undo** — discards unsaved changes of the current lab order.
* **Delete** — deletes the current lab order; confirm with **Save** to execute the
  delete in the database. Note that lab orders may be referenced by other records.
* **Load** — re-reads the current lab order if another user changed it in the background.

### Average values

On the **Average Values** tab of the Explorer the system shows the statistics it
calculates over the laboratory tags (average values per position and per position
item) for the selected lab order.

## Field reference (header)

| Field | Description |
|---|---|
| Lab Order No. | Unique number of the lab order (assigned automatically) |
| Inbound/Outbound/Production order position, Facility lot | Reference to the sampled business object (only the matching field is visible/editable) |
| Material | Material being tested (taken over from the referenced object) |
| Template No. / Template Name | Template the lab order was created from (read-only) |
| Sample Taking Date | Date when the sample was taken |
| Test Date | Date of the test |
| Lab Order State | Status of the lab order (e.g. *New*) |
| Template Name | Name of the template used (on the header grid) |

## Field reference (position)

| Field | Description |
|---|---|
| Laboratory Tag | Measurement tag to be tested |
| Sequence | Sort order of the position |
| Absolute minimum / Minimum / Maximum / Absolute maximum | The four limit values of the tag |
| Reference Value | Nominal value of the test |
| Actual Value | Measured result |
| Position State | Status of the position |
| Line Number | Line number (e.g. from the source document) |
| Comment | Free comment |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOLabOrder` contains the
complete usage recipe (search → navigate → edit → save, creation from template via
dialog, position handling), so an agent can operate the Lab Order app remotely, e.g.
for user training. The button captions above are the translations of these methods
(`ACMethod*` attributes, stored in `ACClassText`); the most important mapping:
**New** → `New` (opens the creation dialog `NewLabOrderDialog`), **Create** →
`DialogCreatePos`, **Cancel** → `DialogCancelPos`, **View Lab Order** →
`ShowLabOrderViewDialog`, **Close** → `CloseLabOrderViewDialog`, **New Position** →
`NewLabOrderPos`, **Delete Position** → `DeleteLabOrderPos`, **Search** → `Search`,
**Save** → `Save`.
