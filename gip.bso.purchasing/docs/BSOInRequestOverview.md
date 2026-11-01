# Request Overview (Inquiry Overview)

> Business object: `BSOInRequestOverview` (Assembly `gip.bso.purchasing`, Menu: **Purchasing → Request Overview**)

## Definition

The **request overview** shows all purchase inquiries (requests for quotation) of the
system in a single list. It is a read-oriented overview app: inquiries are created and
deleted in the **Quotation** app (`BSOInRequest`), here they can be searched, opened,
edited and saved.

## Concepts

| Term | Meaning |
|---|---|
| **Inquiry (InRequest)** | Purchase inquiry header: number, date, state, supplier, prices |
| **Overview list** | All inquiries matching the current search criteria |

## Screen layout

| Tab / Area | Content |
|---|---|
| **Overview** (tab) | Data grid of all inquiries (header caption: **Request**), bound to the selected inquiry |

Toolbar functions: **Search**, **Load**, **Save**, **Undo**.

## Working with the overview (step by step)

1. The list is filled automatically when the app starts; click **Search** to refresh it
   with the current search criteria.
2. Select an inquiry in the list — it becomes the current inquiry.
3. Click **Load** to re-read the selected inquiry (including its positions) from the
   database, e.g. if another user changed it in the background.
4. Edit the fields of the current inquiry.
5. Click **Save** to store the changes, or **Undo** to discard them.

## Field reference

The columns of the overview grid correspond to the inquiry header fields (inquiry
number, date, version, state, supplier, delivery dates, prices, ...). See the
**Quotation** manual for the full field reference.

## For AI agents (MCP)

The class-level `<summary>` of `BSOInRequestOverview` contains the usage recipe
(`Search` → `InRequestList` → `NavigateFirst()` → `CurrentInRequest` → `Save()` /
`UndoSave()` / `Load()`). This BSO has **no** `New()`/`Delete()` methods — create and
delete inquiries via `BSOInRequest`.
