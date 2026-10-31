# Operation Log Export (Operation View)

> Business object: `BSOOperationView` (Assembly `gip.bso.manufacturing`)

## Definition

The **Operation Log Export** app exports operation logs (scan bookings) from
the database into an Excel workbook. It is a pure export tool — there is no
record list and nothing can be edited.

The export reads the operation logs of a date range, takes the
*UnregisterEntityOnScan* bookings (end of a scan operation) and resolves for
each booking:

* material (number and name), program number, lot and split of the charge
* start time (matching *RegisterEntityOnScan* booking) and end time
* machine/process module instance and its model type (caption and URL)
* the executed method with all its parameters and results

## Screen layout

A single form with the export settings:

| Element | Content |
|---|---|
| **From / To** | Date range of the export (initialized with the current month) |
| **Export file** | Target Excel file with a **…** browse button |
| **Table mode** | Layout of the workbook (see below) |
| **Export** | Starts the export |

## Exporting operation logs (step by step)

1. Set the **From** and **To** dates of the export range.
2. Click the **…** button next to **Export file** and choose the target
   `.xlsx` file.
3. Select the **table mode**:
   * *Operation log and params in one worksheet (true starschema)* — one sheet,
     one row per parameter/result
   * *Operation log and params in each worksheet* — one sheet with the logs and
     a second sheet with the parameters (linked by an ID column)
4. Click **Export**. The export runs in the background with a progress dialog;
   errors are shown in the message list.

The **Export** button is only active when the date range, the file path and the
table mode are set.

## Field reference (exported columns)

| Column | Description |
|---|---|
| Material No. / Material Name | Material of the charge |
| Program No. | Production order program number |
| Lot No. / Split No. | Lot and split of the charge |
| Instance caption / URL | Process module instance that executed the operation |
| Type model caption / URL | Model type (class) of the instance |
| Method | Caption of the executed method |
| Start time / End time | Register → Unregister time span of the scan |
| Parameter name / Value | Parameters and results of the method |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server.
The class-level `<summary>` of `BSOOperationView` contains the complete usage
recipe: set `FilterStartTime`/`FilterEndTime`, call `SetFilePath()` (result in
`FilePath`), select `SelectedTableMode` from `TableModeList` (1 = one
worksheet, 2 = separate worksheets), then call `Export()` (background worker,
progress dialog, messages in `MsgList`/`CurrentMsg`).
