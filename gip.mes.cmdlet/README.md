# gip.mes.cmdlet

PowerShell cmdlet library (`gip.mes.cmdlet.dll`, .NET Framework 4.8) that exposes iPlus/MES
maintenance and deployment tasks as PowerShell commands.

---

## 1. Setup

### (1) Set the `VBPowerShellSettings.VarioDataDefault` path

### (2) Copy and configure `VBPowerShellSettings.json` into that path

### (3) Import the module

```ps
Import-Module "c:\Devel\iPlusV4\trunk\iPlusMES\bin\Debug\gip.mes.cmdlet.dll" -Force
```

### (4) `Set-HandleExecuteACMethod`

#### Description

Reads a `.cs` file, extracts the `ACClass` name and calls `BSOiPlusStudio` to regenerate the
`HandleExecuteACMethod()` method.

```ps
Set-HandleExecuteACMethodCmdlet -FileName "c:\Devel\kajbum.com\projects\Schaefers\schaefers.solution.project\BSO\BSOPartsListSchaefers.cs"
```

---

## 2. Cmdlet reference

All cmdlets derive from `System.Management.Automation.Cmdlet`. They are grouped below by the
`CmdletAttribute` noun name (`NounName`), i.e. the part after the dash in the command name.

| Noun name | Cmdlets | Folder |
|---|---|---|
| `iPlusResource` | `Clear-`, `Export-`, `Get-`, `Import-`, `Set-` | `ControlScript`, `DesignSync` |
| `iPlusTranslation` | `Get-`, `InsertIntoFile-`, `New-`, `ReadFromFile-`, `Set-` | `Translation` |
| `DBSync` | `Add-`, `Remove-`, `Set-` | `DBSync` |
| `iPlusDataForBarcode` | `Get-` | `Barcode` |
| `iPlusConfig` | `Set-` | `Config` |
| `HandleExecuteACMethodCmdlet` | `Set-` | `HandleExecuteACMethod` |

> **Note:** `InsertIntoFile` and `ReadFromFile` are not approved PowerShell verbs, so
> `InsertIntoFile-iPlusTranslation` and `ReadFromFile-iPlusTranslation` may produce a
> `PSScriptAnalyzer` warning. They are kept as-is for backward compatibility with existing scripts.

> **Note:** `VarioData` is a plain configuration property on most cmdlets and is **not** exposed as
> a cmdlet parameter — only `Get-iPlusResource` and `Set-iPlusResource` declare it as a parameter.
> It is resolved through `FactorySettings.Factory(...)` and defaults to
> `VBPowerShellSettings.VarioDataDefault` (`C:\VarioData`).

---

### 2.1 Noun: `iPlusResource`

Manages iPlus design resources (ACClass, ACClassProperty, ACClassMethod, ACClassText,
ACClassMessage, ACClassDesign, ACClassConfig, ACClassPropertyRelation) between the database,
the file system and `.zip` packages.

#### `Clear-iPlusResource` — `iPlusResourceCleanUpCmdlet`

Calls `ACClassCleanManager` to delete unused ACClasses and ACClass members (methods, properties, etc.).

| Parameter | Type | Mandatory | Description |
|---|---|---|---|
| `-IsLogDisabled` | `bool` | no | Disables clean-up logging. |
| `-SavePath` | `string` | no | Path for saving the clean-up log. Defaults to the configured `VarioData` folder. |

#### `Export-iPlusResource` — `iPlusResourceExportZipCmdlet`

Generates iPlus resources (designs, texts and messages) for the selected ACClasses and saves them
to a `.zip` file.

| Parameter | Type | Mandatory | Description |
|---|---|---|---|
| `-ClassNames` | `string[]` | **yes** | List of class names to export. |
| `-Project` | `ProjectEnum` | no | Project of the ACClasses to export: `Root` (MES library) or `Variolibrary` (iPlus library). Default: `Root`. |
| `-SavePath` | `string` | no | Location for the `.zip` export. Defaults to `VarioDataDefault`. |
| `-IsExportACClass` | `bool` | no | Export the ACClass definition (`ACClass.gip`). |
| `-IsExportACClassProperty` | `bool` | no | Export the ACClassProperty definition (`ACClassProperty.gip`). |
| `-IsExportACClassMethod` | `bool` | no | Export the ACClassMethod definition (`ACClassMethod.gip`). |
| `-IsExportACClassPropertyRelation` | `bool` | no | Export the ACClassPropertyRelation definition (`ACClassPropertyRelation.gip`). |
| `-IsExportACClassConfig` | `bool` | no | Export the ACClassConfig definition (`ACClassConfig.gip`). |
| `-IsExportACClassMessage` | `bool` | no | Export ACClassMessage (`ACClassMessage.gip`). Default: `true`. |
| `-IsExportACClassText` | `bool` | no | Export ACClassText (`ACClassText.gip`). Default: `true`. |
| `-IsExportACClassDesign` | `bool` | no | Export ACClassDesign (`ACClassDesign.gip`). Default: `true`. |
| `-ExportFromTime` | `DateTime?` | no | Time of the last update of any exported resource; only resources changed after this time are exported. |
| `-FromToday` | `SwitchParameter` | no | Only resources changed on this date are exported. |
| `-UserName` | `string` | no | User name; becomes part of the `.zip` file name. Default: `sup`. |

Example:

```ps
Export-iPlusResource -ClassNames "BSOTandTv3" -Project Root -IsExportACClassDesign $true -IsExportACClassText $true -IsExportACClassMessage $true
```

#### `Get-iPlusResource` — `iPlusResourceGetCmdlet`

Saves resources from ACClass (class, properties, texts, designs, relations) to `.gip` and `.xml`
files on the hard drive.

| Parameter | Type | Mandatory | Description |
|---|---|---|---|
| `-ProjectName` | `string` | **yes** | Project name to export. |
| `-ClassNames` | `string[]` | no | Class names to export. |
| `-VarioData` | `string` | no | Configuration folder. Defaults to `VarioDataDefault`. |
| `-OmitExportACClassDesign` | `SwitchParameter` | no | Skip exporting ACClassDesign (parameter set `ExportOptions`). |
| `-OmitExportACClassText` | `SwitchParameter` | no | Skip exporting ACClassText (parameter set `ExportOptions`). |
| `-OmitExportACClassMessage` | `SwitchParameter` | no | Skip exporting ACClassMessage (parameter set `ExportOptions`). |

#### `Import-iPlusResource` — `iPlusResourceImportCmdlet`

Imports new `.zip` designs and resources into the database. It runs the same import procedure for
the `.zip` file as CTRL load.

No cmdlet parameters.

#### `Set-iPlusResource` — `iPlusResourceSetCmdlet`

Reads `.gip` and `.xml` resources from the hard drive and saves them to the database.

| Parameter | Type | Mandatory | Description |
|---|---|---|---|
| `-ProjectName` | `string` | **yes** | Project name to import into. |
| `-ClassNames` | `string[]` | **yes** | Class names to import. |
| `-ItemNames` | `string[]` | no | Item names to import. |
| `-UserName` | `string` | no | User name recorded as the updater. Default: `SUP`. |
| `-VarioData` | `string` | no | Configuration folder. Defaults to `VarioDataDefault`. |
| `-OmitExportACClassDesign` | `SwitchParameter` | no | Skip importing ACClassDesign (parameter set `ExportOptions`). |
| `-OmitExportACClassText` | `SwitchParameter` | no | Skip importing ACClassText (parameter set `ExportOptions`). |
| `-OmitExportACClassMessage` | `SwitchParameter` | no | Skip importing ACClassMessage (parameter set `ExportOptions`). |

---

### 2.2 Noun: `iPlusTranslation`

Manages translations stored in the `ACCaptionTranslation` field of ACClass, ACClassProperty,
ACClassText, ACClassDesign and ACClassMessage.

#### `Get-iPlusTranslation` — `TranslationGetCmdlet`

Searches for translated resources stored in the `ACCaptionTranslation` field of tables such as
ACClass, ACClassProperty, etc.

| Parameter | Type | Mandatory | Description |
|---|---|---|---|
| `-ClassACIdentifer` | `string` | no | Filter by the owning class identifier. |
| `-ACIdentifer` | `string` | no | Filter by the item identifier. |
| `-SearchText` | `string` | no | Free-text search. |
| `-ItemTypes` | `ItemTypeEnum[]` | no | Filter by item type (`ACClass`, `ACClassProperty`, `ACClassText`, `ACClassDesign`, `ACClassMessage`). |

Output: a list of `TranslationPresentation` objects.

#### `InsertIntoFile-iPlusTranslation` — `TranslationInsertIntoFileCmdlet`

Creates a new translation item and inserts it into a source file at the given line.

| Parameter | Type | Mandatory | Description |
|---|---|---|---|
| `-ProjectName` | `string` | **yes** | Project name. |
| `-ClassACIdentifier` | `string` | **yes** | Owning class identifier. |
| `-FileName` | `string` | **yes** | Source file to insert into. |
| `-Line` | `int` | **yes** | Line number for the insertion. |
| `-MessageType` | `MessageTypeEnum` | **yes** | Kind of translation item. |
| `-EnText` | `string` | no | English text. Default: `-`. |
| `-DeText` | `string` | no | German text. Default: `-`. |
| `-UpdateName` | `string` | no | Updater name. Default: `SUP`. |

#### `New-iPlusTranslation` — `TranslationNewCmdlet`

Adds a new translation item into ACClassText or ACClassMessage.

| Parameter | Type | Mandatory | Description |
|---|---|---|---|
| `-ProjectName` | `string` | **yes** | Project name. |
| `-MessageType` | `MessageTypeEnum` | **yes** | Kind of translation item (`None`, `Info`, `Warning`, `Failure`, `Error`, `Exception`, `Question`, `Status`). |
| `-EnText` | `string` | **yes** | English text. |
| `-DeText` | `string` | **yes** | German text. |
| `-UpdateName` | `string` | no | Updater name. |
| `-ACIdentifier` | `string` | no | Item identifier. |
| `-ClassACIdentifier` | `string` | no | Owning class identifier. |
| `-ACURLComponentCached` | `string` | no | Cached AC URL component. |

Either `-ClassACIdentifier` or `-ACURLComponentCached` must be supplied.

#### `ReadFromFile-iPlusTranslation` — `iPlusTranslationFromSourceFileCmdlet`

Updates translations in the database from a source file.

| Parameter | Type | Mandatory | Description |
|---|---|---|---|
| `-ProjectName` | `string` | **yes** | Project name. |
| `-FileName` | `string` | **yes** | Source file to read translations from. |
| `-UpdateName` | `string` | no | Updater name. |

#### `Set-iPlusTranslation` — `TranslationSetCmdlet`

Sets the translation value in the `ACCaptionTranslation` field of the tables that have it.

| Parameter | Type | Mandatory | Description |
|---|---|---|---|
| `-ItemType` | `ItemTypeEnum` | **yes** | Table/item type to update. |
| `-ACIdentifer` | `string` | **yes** | Item identifier. |
| `-EnText` | `string` | **yes** | English text. |
| `-DeText` | `string` | **yes** | German text. |
| `-UpdateName` | `string` | **yes** | Updater name. |
| `-ID` | `Guid?` | no | Explicit record ID. |

---

### 2.3 Noun: `DBSync`

Runs and maintains the DBSyncer SQL script pipeline.

#### `Add-DBSync` — `DBSyncScriptAddCmdlet`

Executes only one file — the same as CTRL load for a single SQL file.

| Parameter | Type | Mandatory | Description |
|---|---|---|---|
| `-DbSyncerInfoContextID` | `string` | **yes** | DBSyncer context (sub-folder under `DbScripts`). |
| `-FileName` | `string` | **yes** | SQL script file name inside the context folder. |

#### `Remove-DBSync` — `DBSyncScriptRemoveCmdLet`

Deletes a DBSyncer file from the hard drive and from the database.

| Parameter | Type | Mandatory | Description |
|---|---|---|---|
| `-DbSyncerInfoContextID` | `string` | **yes** | DBSyncer context (sub-folder under `DbScripts`). |
| `-FileName` | `string` | **yes** | SQL script file name inside the context folder. |

#### `Set-DBSync` — `DBSyncSetCmdlet`

Calls the DBSyncer update procedure, the same as in CTRL load.

No cmdlet parameters.

---

### 2.4 Noun: `iPlusDataForBarcode`

#### `Get-iPlusDataForBarcode` — `iPlusDataForBarcodeCmdlet`

Returns item IDs used for barcode generation:

1. `BarcodeType = FacilityCharge` — returns `FacilityChargeID` for a quant in a facility.
   Filter with `FacilityNo` (facility where the quant is located), `MaterialNo` and `LotNo`.
2. `BarcodeType = Machine` — returns `ACClassID` for a machine. Filter with `MachineACIdentifier`
   (`ACClass.ACIdentifier`).
3. `BarcodeType = Facility` — returns `FacilityID` for a facility. Filter with `FacilityNo`.

| Parameter | Type | Mandatory | Description |
|---|---|---|---|
| `-BarcodeType` | `BarcodeTypeEnum` | **yes** | Item type: `FacilityCharge`, `Machine` or `Facility`. |
| `-FacilityNo` | `string` | no | Facility number used to filter the returned barcode data. |
| `-MaterialNo` | `string` | no | Material number used to filter the returned barcode data. |
| `-MachineACIdentifier` | `string` | no | Machine name used to filter the returned barcode data. |
| `-LotNo` | `string` | no | Lot number used to filter the returned barcode data. |

Output: a list of `BarcodeResult` objects (`ID`, `ItemNo`, `ItemName`, `ACUrl`).

---

### 2.5 Noun: `iPlusConfig`

#### `Set-iPlusConfig` — `iPlusConfig`

Loads the connection string settings into the cmdlet session.

No cmdlet parameters. Output: the resolved `VBPowerShellSettings` object.

---

### 2.6 Noun: `HandleExecuteACMethodCmdlet`

#### `Set-HandleExecuteACMethodCmdlet` — `HandleExecuteACMethodCmdlet`

Reads a `.cs` file, extracts the `ACClass` name and regenerates the `HandleExecuteACMethod()`
method body via `BSOiPlusStudio`.

| Parameter | Type | Mandatory | Description |
|---|---|---|---|
| `-FileName` | `string` | **yes** | Path to the C# source file to rewrite. |

---
