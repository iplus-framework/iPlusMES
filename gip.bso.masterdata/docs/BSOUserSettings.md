# User Settings

> Business object: `BSOUserSettings` (Assembly `gip.bso.masterdata`, Module: **System**)

## Definition

The **User Settings** app manages the personal settings of the currently
logged-in user: the **tenant** (company) the user works for and the **address**
and **person** used for invoicing.

## Usage / Concepts

| Term | Meaning |
|---|---|
| **Tenant** | Company flagged as tenant; the user's home company |
| **Address for Invoice** | Company address of the tenant used on invoices |
| **Person for Invoice** | Company person of the tenant used on invoices |

There is no search or record navigation: the app always works on the settings of
the current user. On opening, the saved settings are preselected.

## Screen layout

A single form with three combo boxes and two buttons (no tabs):

| Control | Content |
|---|---|
| Combo box | Tenant |
| Combo box | Address for Invoice |
| Combo box | Person for Invoice |
| Button | **Set** |
| Button | **Remove** |

Toolbar: **Save**, **Undo**.

## Step-by-step tasks

### Changing your settings

1. Select the **Tenant** in the first combo box. The address and person lists
   reload for the selected tenant.
2. Select the **Address for Invoice**.
3. Select the **Person for Invoice**.
4. Click **Set** — this applies the selections to your user settings (the record
   is created if you have none yet).
5. Click **Save** to write the changes to the database.

### Removing your settings

1. Click **Remove** — your user settings record is deleted and the selection is
   cleared.
2. Click **Save** to execute the delete in the database.

### Other toolbar functions

* **Undo** — discards all unsaved changes.

## Field reference

| Field | Description |
|---|---|
| Tenant | Company with *IsTenant* flag; only tenants are listed |
| Address for Invoice | Invoice address of the selected tenant |
| Person for Invoice | Invoice person of the selected tenant |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOUserSettings`
contains the complete usage recipe: select `SelectedCompany`,
`SelectedCompanyAddress`, `SelectedCompanyPerson` → `SetUserSettings()` →
`Save()`; `ResetUserSettings()` deletes the record. There is no search/navigation.
