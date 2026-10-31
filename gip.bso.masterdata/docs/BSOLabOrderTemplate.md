# Lab Order Template

> Business object: `BSOLabOrderTemplate` (Assembly `gip.bso.masterdata`, Menu: **Master Data → Lab Order Template**)

## Definition

A **Lab Order Template** is the blueprint for [lab orders](BSOLabOrder.md). It defines
for one **material** which test parameters (**positions** with **laboratory tags** and
limit values) have to be checked. When a lab order is created — manually or
automatically by a workflow node — its positions are copied from such a template.

A template is technically a lab order record of type *Template*; it is maintained in
this separate app so that the quality department can manage the test specifications
independently of the running lab orders.

## Concepts

| Term | Meaning |
|---|---|
| **Lab Order Template** | The template header: template number, template name, material |
| **Position** | One test parameter of the template: laboratory tag, sequence, limit values, reference value |
| **Laboratory Tag** | Master data definition of a measurement tag (what is measured) |
| **Limit values** | Four limits per position: absolute minimum, minimum, maximum, absolute maximum |
| **Reference Value** | Target/nominal value of the test |
| **Mass update** | Changing one limit field of a laboratory tag across all templates in one step |

## Screen layout

The app consists of a search **Explorer** (docked, auto-hide on the right) and the main
**Lab Order Template** form (tab).

**Explorer** (right):

| Area | Content |
|---|---|
| **Search** | Filter fields (order number, sample taking date **From**/**To**, limit value **From**/**To**, **Search in Field**, **Laboratory Tag**) with the buttons **Search over filtered value range** and **Clear search filter**; below it the result list of templates |
| **Mass update** | **Value for mass update** field and the **Mass update** button |

**Lab Order Template** (main tab):

| Area | Content |
|---|---|
| Header (upper grid) | Template number (read-only), **Template Name**, **Material** |
| **Position list** (left) | All test positions with laboratory tag, sequence and limit values; buttons **New Position** and **Delete Position** |
| **Position form** (right/bottom) | Detail form of the selected position: laboratory tag, sequence, limit values |

## Step-by-step tasks

### Creating a new template

1. Click **New**. A new, empty template is created and opened.
2. Enter the **Template Name**.
3. Assign the **Material** the template belongs to. Templates are later offered for lab
   orders of this material (or of its production material).
4. Click **Save**.

### Adding test parameters (positions)

In the position list:

1. Click **New Position** — a new line is added.
2. In the position form, assign the **Laboratory Tag** and the **Sequence**.
3. Enter the limit values: **absolute minimum**, **minimum**, **maximum**,
   **absolute maximum**, and if required the **Reference Value**.
4. Click **Save**.
5. Repeat for every test parameter. **Delete Position** removes the selected line.

### Searching templates

1. Open the **Explorer** and enter the search criteria (order number, sample taking
   date **From**/**To**).
2. Click **Search** and select a template in the result list.

### Searching by limit value range

To find templates whose positions have a certain limit value:

1. In the Explorer, enter **Limit value from** and **Limit value to**.
2. Select the **Search in Field** (which of the four limit fields to search:
   absolute minimum, minimum, maximum or absolute maximum).
3. Select the **Laboratory Tag**.
4. Click **Search over filtered value range**. **Clear search filter** removes the
   filter again.

### Mass update of limit values

To change one limit field of a laboratory tag in many templates at once:

1. In the Explorer, enter the **Limit value from** / **Limit value to** range,
   the **Search in Field** and the **Laboratory Tag** as described above.
2. Enter the new value in **Value for mass update**.
3. Click **Mass update**. All matching positions in the listed templates receive the
   new value in the selected limit field.
4. Click **Save** on the affected templates to persist the change.

### Working with records

* **Save** — stores changes of the current template (always save before navigating to
  another record or creating a new one).
* **Undo** — discards unsaved changes of the current template.
* **Delete** — deletes the current template; confirm with **Save** to execute the
  delete in the database. Templates that are referenced by existing lab orders cannot
  be deleted.
* **Load** — re-reads the current template if another user changed it in the background.

## Field reference (header)

| Field | Description |
|---|---|
| Template No. | Unique number of the template (assigned automatically, read-only) |
| Template Name | Name of the template |
| Material | Material the template is valid for |

## Field reference (position)

| Field | Description |
|---|---|
| Laboratory Tag | Measurement tag to be tested |
| Sequence | Sort order of the position |
| Absolute minimum / Minimum / Maximum / Absolute maximum | The four limit values of the tag |
| Reference Value | Nominal value of the test |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOLabOrderTemplate`
contains the complete usage recipe (search → navigate → edit → save, position
handling, mass update), so an agent can operate the Lab Order Template app remotely,
e.g. for user training. The button captions above are the translations of these
methods (`ACMethod*` attributes, stored in `ACClassText`); the most important mapping:
**New** → `New`, **Save** → `Save`, **New Position** → `NewLabOrderPos`,
**Delete Position** → `DeleteLabOrderPos`, **Search** → `Search`,
**Search over filtered value range** → `ApplyValueFilter`,
**Clear search filter** → `ClearValueFilter`, **Mass update** → `MassUpdateOnValues`.
