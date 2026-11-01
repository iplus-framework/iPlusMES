# Maintenance Rule (BSOMaintConfig)

> Business object: `BSOMaintConfig` (Assembly `gip.mes.maintenance`, Menu: **Maintenance → Maintenance Rule**)

## Definition

The **Maintenance Rule** business object is intended for configuring maintenance rules
per component and per component property: which properties of a component are monitored,
from which value a warning is raised, and which times and tasks apply.

> ⚠️ **Status:** In the current version this business object is an **empty placeholder**.
> Its complete implementation (properties and methods) is commented out in the source
> code and therefore not active. The stored screen designs (*MaintConfig*,
> *MaintConfigDetails*) reference functions such as *Generate New Maintenance
> Configuration*, *Apply Changes*, *Search*, *Add property* and *Remove property*,
> which are not available in the active code.

## Where the functionality lives today

The active maintenance rule configuration is done in the **Maintenance plan**
(`BSOMaintOrderTemplate`) app: its wizard creates a component configuration
(*Maintenance Rule*) for the selected component and manages the monitored properties
(*Period and event* step), tasks and performers. See
[BSOMaintOrderTemplate.md](BSOMaintOrderTemplate.md).

## Screen layout (stored designs, currently inactive)

| Design | Content |
|---|---|
| **Mainlayout** | Embeds the *MaintConfigCS* component selector |
| **MaintConfig** | Level selection (*Select Level*) in the component hierarchy and the embedded *MaintConfigDetails* |
| **MaintConfigDetails** | Role selection, work task description, times (interval, last/next term, warning difference), events, tasks and the property lists *Available Properties* / *Assigned Properties for Maintenance* with warning and maximum values |

## For AI agents (MCP)

The class summary of `BSOMaintConfig` (via `get_type_infos`) states that the BSO is a
placeholder: it exposes no active properties or methods. Do not attempt to invoke
methods on it; use `BSOMaintOrderTemplate` instead.
