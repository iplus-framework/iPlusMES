# Closings and Inventory Synchronization

> Business object: `BSOFacilityEndClosing` (Assembly `gip.bso.facility`, Menu: **Warehouse Management → Closings and Inventory Sync.**)

## Definition

This app executes **stock closings** (daily, weekly, monthly, year-end) and the
**stock balancing** (cell/material adjustment) in the warehouse management.

All facility bookings are always executed only via the facility manager
(StoreBookingManager). This app only starts the corresponding booking jobs in the
background and shows a progress dialog. It is a pure command dialog — there is no
search list and no editable data.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Closing** | Periodic stock closing run (day/week/month/year) |
| **Balancing / Adjustment** | Cell/material adjustment: synchronizes stock quantities with the physical stock |
| **Background job** | The closing runs in a background worker; the app shows a progress dialog meanwhile |
| **Progress dialog** | Shows total and sub progress and the **Cancel Closing / Adjustment** button |

## Screen layout

The screen is a tab control with one tab per job type. Each tab contains an
explanatory text and one button:

| Tab | Button |
|---|---|
| **Balancing** | **Cell / Material Adjustment** |
| **DailyClosing** | **Daily Closing** |
| **WeeklyClosing** | **Weekly Closing** |
| **MonthlyClosing** | **Monthly Closing** |
| **YearEndClosing** | **Year-End Closing** |

While a job is running, the **ClosingProgress** dialog is shown with:

* Total progress bar and progress text
* Sub progress bar and sub progress text
* Button **Cancel Closing / Adjustment**

## Step-by-step tasks

### Running a closing or adjustment

1. Switch to the tab of the desired job type
   (**Balancing**, **DailyClosing**, **WeeklyClosing**, **MonthlyClosing** or
   **YearEndClosing**).
2. Click the button in the tab (e.g. **Daily Closing**).
3. The job starts in the background and the progress dialog opens.
4. Wait until the job finishes. Result and warning messages are displayed afterwards.

### Cancelling a running job

1. In the progress dialog click **Cancel Closing / Adjustment**.
2. The background job is cancelled. The button is only enabled while a job is running.

### Notes

* Only **one** background job can run at a time — all start buttons are disabled while
  a job is running.
* Closings post bookings; run them outside production hours where possible.

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOFacilityEndClosing`
contains the complete usage recipe. Relevant C# methods: `StartMatching`
(**Cell / Material Adjustment**, booking method `!MatchingStockAll`),
`StartDayClosing` (**Daily Closing**, `!ClosingDay`), `StartWeekClosing`
(**Weekly Closing**, `!ClosingWeek`), `StartMonthClosing` (**Monthly Closing**,
`!ClosingMonth`), `StartYearClosing` (**Year-End Closing**, `!ClosingYear`) and
`Cancel` (**Cancel Closing / Adjustment**). All start methods are disabled while
`BackgroundWorker.IsBusy` is true; the running booking parameters are exposed via
`CurrentBookingParameter`.
