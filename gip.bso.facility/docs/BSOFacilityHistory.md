# Balance Sheet History

> Business object: `BSOFacilityHistory` (Assembly `gip.bso.facility`, Menu: **Warehouse Management → Balance Sheet History**)

## Definition

This app displays the **balance sheet history**: stock balances per period
(day, week, month, year) for materials, company materials and storage bins.

Balance sheets are created by the system (period closing). The app is **read-only**:
no new balance sheets can be created and **Delete** is disabled.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Balance sheet** | One balance record for a period with a balance date and previous balance date |
| **Period no.** | Number of the period within the balance sheet |
| **Material balance** | Stock, inward and outward quantities per material |
| **Company material balance** | Stock, inward and outward quantities per company material |
| **Storage bin balance** | Stock, inward and outward quantities per storage bin |
| **Time period** | Aggregation level of the balance sheet (day, week, month, year, ...) |

## Screen layout

The screen consists of a search list (left) and a detail area (right):

| Tab / Section | Content |
|---|---|
| **Explorer** (list) | Balance sheets with balance date, previous balance date and period number |
| **Balance sheet details** | Sub-lists of the selected balance sheet: **Material**, **Company material** and **Facility** (storage bin) balances with stock quantity, inward and outward quantities, last stock quantity |
| **Filter** | Time period, period number, balance date from/to |

The header of the detail area shows the balance date, previous balance date and
period number of the current balance sheet.

## Step-by-step tasks

### Filtering balance sheets

1. Select the **Time Period** (day, week, month, year, ...).
2. *(Optional)* Enter a **Period no.**
3. *(Optional)* Restrict the **Balance date from** / **Balance date to** range.
4. The list is re-searched automatically after each filter change; **Search**
   re-executes it manually.

### Viewing the balances of a period

1. Select a balance sheet in the list. The detail area shows its balance date,
   previous balance date and period number.
2. In the **Material** list: stock quantity, inward and outward per material.
3. In the **Company material** list: the same per company material. Use the
   **Filter by Company** dropdown to restrict the list to one company.
4. In the **Facility** list: the same per storage bin.
5. Click **Load** to re-read the selected balance sheet with all detail lists
   (e.g. after another user changed data in the background).

### Saving and discarding

* **Save** stores changes of the current record.
* **Undo** discards unsaved changes and reloads.
* **Delete** is disabled — balance sheets are created by the period closing and
  cannot be deleted here.

## Field reference

| Field | Description |
|---|---|
| Balance date | Date of the balance sheet |
| Balance date (previous) | Balance date of the preceding period |
| Period no. | Period number |
| Time period | Aggregation level (day/week/month/year) |
| Stock quantity | Stock at the balance date |
| Inward / Outward | Sum of the inward/outward bookings in the period |
| Last stock quantity | Stock of the preceding balance |
| Filter by Company | Restricts the company material balances to one company |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityHistory`
contains the complete usage recipe: `SearchWord` → `HistoryList` → `NavigateFirst()`
→ `CurrentHistory`; filters `FilterSelectedTimePeriod`, `FilterPeriodNo`,
`FilterBalanceDateFrom` / `FilterBalanceDateTo` (each re-executes the search);
detail lists `MaterialHistoryList`, `CompanyMaterialHistoryList` (filter
`SelectedCompanyFilter`) and `FacilityHistoryList`. `Delete()` exists but is always
disabled (`IsEnabledDelete()` returns `false`).
