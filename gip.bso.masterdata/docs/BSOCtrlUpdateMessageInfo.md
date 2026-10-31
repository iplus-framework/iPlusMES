# Update Info

> Business object: `BSOCtrlUpdateMessageInfo` (Assembly `gip.bso.masterdata`, Module: **Development**)

## Definition

The **Update Info** app shows the log messages that the **CTRL load** (metadata
update) has presented. After the load and the closing of the masterpage this log
is no longer present; with this app the messages can be shown again and exported.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Message** | One log entry of the CTRL load: level, time, message key, text and source |
| **Export folder** | Local folder where the log file is written |

The messages are passed to the app when it is opened — it does not read the
database and is **read-only**.

## Screen layout

The app is docked as a tab at the bottom of the window. It consists of:

| Area | Content |
|---|---|
| **Update info** (tab) | Buttons **Copy all to clipboard** and **Save to file**, the **Export folder** field and the detail of the selected message (level, occurred, message key, message, source, content — read-only) |
| **Explorer** (auto-hide window at the bottom) | Message list with columns **Level**, **Occurred**, **Message key**, **Message**, **Source** |

## Step-by-step tasks

### Viewing a message

1. Open the **Explorer** (auto-hide at the bottom) and select a message row.
   The detail is shown in the **Update info** tab.

### Exporting all messages

1. Enter the target directory in the **Export folder** field (the folder must
   already exist).
2. Click **Save to file** — all messages are written as tab-separated lines to
   the file `CtrlLoadMessage.txt` in that folder.
3. Alternatively click **Copy all to clipboard** to copy all messages as
   tab-separated lines to the clipboard.

> **Note:** *Copy all to clipboard* is currently a placeholder — the clipboard
> call is not yet implemented. Use **Save to file** instead.

## Field reference

| Field | Description |
|---|---|
| Export folder | Folder for the `CtrlLoadMessage.txt` export file |
| Level | Message level (Info/Warning/Error, ...) |
| Occurred | Timestamp of the message |
| Message key | Identifier of the component that raised the message |
| Message | Message text |
| Source | Source of the message |
| Content | Additional XML content of the message (read-only) |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of
`BSOCtrlUpdateMessageInfo` contains the complete usage recipe: the messages
arrive via the `DataList` constructor parameter into `MsgList`; `Search()`
selects the first message; `CopyAllToClipboard()` (placeholder) and
`SaveAllToTxtFile()` export `DataStringList` to `ExportFolder`. The BSO is
read-only (no New/Save/Delete).
