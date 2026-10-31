# Preferred Parameters

> Business object: `BSOPreferredParameters` (Assembly `gip.bso.masterdata`, Menu: **dialog — opened from the ordering/production process**)

## Definition

The **Preferred Parameters** dialog shows all parameters of a workflow — the parameters of the
**workflow nodes** (PWNode) and of the **process functions** (PAFunction) — for a concrete context
(a workflow, a partslist, a production order partslist or a picking) and allows the user to
**override** individual parameter values for this context, optionally per **machine**.

Unlike most business objects this app is **not opened from a menu with a search list**. It is a
dialog that the system opens automatically (e.g. during order creation) or that other programs
open programmatically.

## Concepts

| Term | Meaning |
|---|---|
| **Workflow node (PWNode)** | A node of the process workflow; has its own parameters |
| **Process function (PAFunction)** | A function module within the workflow; has its own parameters |
| **Override** | A new value for a parameter that is stored in the current config store and takes precedence over the default value |
| **Machine** | An override can be restricted to a single machine; the machine list shows only machines attached to the workflow of the parameter |
| **Configuration history** | The previously defined configurations of a parameter (config store, expression, value, comment) |
| **Short dialog** | Compact variant of the dialog (config flag *Use short dialog*) that shows only the editable parameters in one list |

## Screen layout

The dialog exists in two variants:

**Full dialog** (default) — two tabs:

| Tab | Content |
|---|---|
| **Konfiguration** (Configuration) | Parameter list of the **workflow nodes** with value, config store, expression and comment; machine selection; **Override parameter** button; configuration history of the selected parameter with **Remove overridden parameter** |
| **Methode** (Method) | Same layout for the parameters of the **process functions** |

**Short dialog** (*Use short dialog* enabled) — a single list of the editable parameters
(expression, comment, value) with a comment field and the **OK** / **Cancel** buttons.

## Overriding a parameter (step by step)

1. Open the dialog (the system does this automatically, e.g. during order creation).
2. Select the parameter in the parameter list.
3. *(Optional)* To override the parameter only for one machine, select the **Machine** in the
   combo box. Only machines that are attached to the workflow of the parameter are offered.
4. Click **Override parameter**. A new configuration entry is created for the current context
   and appears at the top of the history.
5. Enter the new **Value** (and optionally an **Expression** and a **Comment**).
6. Click **OK** to save all changes and close the dialog — or **Cancel** to discard them.

## Removing an override

1. Select the parameter whose override you want to remove.
2. In the configuration history select the override entry of the current context.
3. Click **Remove overridden parameter**. The entry is deleted and the change is saved
   immediately.
4. Click **OK** to close the dialog.

**Note:** **Remove overridden parameter** is only enabled when the selected parameter actually
has an override in the current context.

## Field reference

| Field | Description |
|---|---|
| Parameter list | All parameters of the workflow nodes / process functions with caption, identifier and URLs |
| Value | Current value of the parameter override |
| Expression | Formula/expression of the configuration (validated while typing; invalid expressions are disabled) |
| Comment | Free comment for the configuration |
| Config store | Config store in which the override is stored (shows the level of the override) |
| Machine | Machine for which the override applies (optional) |
| Configuration history | Previous configurations of the selected parameter with config store, expression, value and comment |
| Use short dialog | Config flag: switches to the compact short dialog |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOPreferredParameters` contains the
complete usage recipe. The C# method names behind the buttons are:

| Button | C# method |
|---|---|
| **Override parameter** | `CreatePWNodeValue()` / `CreatePAFunctionValue()` |
| **Remove overridden parameter** | `DeletePWNodeParamValue()` / `DeletePAFunctionValue()` |
| **OK** | `DialogOK()` |
| **Cancel** | `DialogCancel()` |

Programmatic opening: `ShowParamDialog(acClassWFID, partslistID, prodOrderPartslistID, pickingID)`
or `ShowParamDialogResult(...)` (returns `true` when confirmed with OK). The parameter lists are
exposed as `PWNodeParamValueList`, `PAFunctionParamValueList` and `QuickParamList` (short dialog);
the selected parameter as `SelectedPWNodeParamValue` / `SelectedPAFunctionParamValue`.
