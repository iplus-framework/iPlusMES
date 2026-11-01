# Rating Complaints (Beanstandungen)

> Business object: `BSORatingComplaint` (Assembly `gip.bso.purchasing`, Menu: **Purchasing → Company / Rating**)

## Definition

The **Rating Complaints** app manages complaints (**RatingComplaint**) that belong to a
**Rating** of a company — typically a supplier evaluation per delivery note. It is
usually embedded in the **Company** app (rating tab) or opened as a modal dialog for a
single rating.

A complaint consists of a **complaint type** (master data), a **score** and a free-text
**comment**. The app also shows aggregated statistics: average score per complaint type,
score distribution and the overall average score of the company.

## Concepts

| Term | Meaning |
|---|---|
| **Rating** | A score for a company, usually per delivery note |
| **Rating Complaint** | A single complaint belonging to a rating: type, score, comment |
| **Complaint Type** | Master data category of the complaint (e.g. late delivery, wrong quantity) |
| **Score** | Numeric rating value; displayed on a scale of **Rating Star Count** stars (default 3) |
| **Company preview** | Unfiltered view over all ratings of a company with filters for period and complaint type |
| **Rating editing** | View for one specific rating with its complaint list and edit form |

## Screen layout

The app has two main layouts, chosen automatically:

| Layout | When | Content |
|---|---|---|
| **Company preview** (MainCompanyPreview) | No single rating selected | Overall average score (stars), filter fields **From** / **To** and **Rating Complaint Type**, button **Search**, complaint type averages, score distribution, ratings per delivery note (with delivery note preview) and the complaint list |
| **Rating editing** (MainEditing) | A specific rating is opened | Rating header (company, delivery note no., date), complaint list with buttons **New** / **Delete**, and the complaint edit form (complaint type, score, comment) with a close button |

## Viewing complaints of a company

1. Open the rating tab of a company (or the app with a company parameter).
2. *(Optional)* Set the period **From** / **To** and the **Rating Complaint Type** filter.
3. Click **Search** — the complaint list, the ratings per delivery note and all
   averages are refreshed.
4. The **Score** (stars) shows the average score of the company; the distribution list
   shows how many ratings fall into each score value.

## Creating a complaint (step by step)

1. Open a specific rating (in the rating editing view).
2. Click **New** — a new complaint is created for this rating and selected.
3. Select the **Complaint Type**.
4. Set the **Score**.
5. Enter a text in **Comment**.
6. Click **Save**. Confirm a warning message with **Yes**.

## Deleting a complaint

1. Select the complaint in the complaint list.
2. Click **Delete** and confirm.

## Delete, Undo, Search

* **Undo** — discards unsaved changes of the current complaint.
* **Search** — refreshes all lists after changing filters.
* Complaints are deleted only via **Delete** (see above); there is no separate
  order-level delete.

## Field reference

| Field | Description |
|---|---|
| Complaint Type | Category of the complaint (master data) |
| Score | Numeric score of the complaint (scale: star count) |
| Comment | Free-text description of the complaint |
| Insert Date | Date the complaint/rating was recorded |
| Delivery Note No. | Delivery note the rating belongs to |
| Company | Rated company |
| From / To | Filter period for the company preview |
| Rating Complaint Type (filter) | Restricts the list to one complaint type |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSORatingComplaint` contains
the complete usage recipe including the C# method names (`Search`, `New`, `Save`,
`Delete`, `UndoSave`, `OpenAsModal`) and the list properties (`RatingComplaintList`,
`RatingComplaintAvgList`, `RatingList`, `RatingAVGList`, `RatingCompleteAVGList`,
`RatingCompanyScoreAvg`), so an agent can operate the app remotely. The button captions
above are the translations of these methods (`ACMethod*` attributes, stored in
`ACClassText`).
