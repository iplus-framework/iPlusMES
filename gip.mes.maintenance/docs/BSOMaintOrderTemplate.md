# Maintenance Plan Templates (BSOMaintOrderTemplate)

> Business object: `BSOMaintOrderTemplate` (Assembly `gip.mes.maintenance`, Menu: **Maintenance → Maintenance plan**)

## Definition

A **maintenance plan template** defines a recurring maintenance task for a **facility**
(plant) or a **component**. A template is a maintenance order that is *not based on
another maintenance order* — it is the master definition from which concrete maintenance
orders are later derived.

A template consists of:

* the **header** (order number, facility or component, interval data),
* the **period/event settings** (maintenance interval, last/next maintenance term,
  warning difference),
* the **monitored properties** of the component (with warning and maximum values),
* the **tasks** (work steps with description and planned duration, optionally with
  media documentation),
* the **performers** (a user, a user group or a company).

## Concepts

| Term | Meaning |
|---|---|
| **Template** | Maintenance order with no parent order (`BasedOnMaintOrderID` empty); the list only shows templates |
| **Facility** | The plant the maintenance applies to (alternative to a component) |
| **Component** | The machine/component the maintenance applies to; a component configuration is created for it |
| **Maintenance Property** | A component property monitored by the maintenance rule, with warning/maximum value |
| **Task** | A work step of the maintenance, with description and planned duration |
| **Performer** | User, group or company responsible for executing the maintenance |
| **Wizard** | The **New** dialog guides through the steps *Facility/Component → Period and event → Task → Performer* |

## Screen layout

The app is a search list on the left (**Explorer**: order number, state, component,
insert/update user and date, search filter with **Search**) and a detail area on the
right (**Maintenance plan**). The detail area shows the header of the current template
(order number, component) and embeds the wizard pages:

| Tab / page | Content |
|---|---|
| **Facility / Components** (first wizard step) | Facility explorer or component selector to choose what is maintained |
| **Period and event** | Interval data (maintenance interval, last/next maintenance term, warning difference) and the component property lists: *Available properties* and *Assigned properties* with warning/maximum values |
| **Task** | Task list with task name, description and planned duration; **Task documentation** (media) |
| **Performer** | Assignment list (name, type, active, default) and tabs **User** / **Group** / **Company** |

The wizard dialog contains the buttons **Next**, **Back** and **Close**.

## Creating a new template (step by step)

1. Click **New** in the toolbar. The template wizard opens.
2. **Step 1 – Facility or Component:** select a **Facility** in the facility explorer
   *or* a **Component** in the component tree. If a component is chosen, a component
   configuration is created for it automatically.
3. Click **Next**.
4. **Step 2 – Period and event:** enter the **Maintenance Interval**, the
   **Last Maintenance Term**, the **Next Maintenance Term** and the **Warning
   Difference**.
   Optionally, monitor component properties: enter a **Search Keyword**, choose the
   **Filter Mode** (*By Group* or *By Property Name*), click **Search**, select a
   property in *Available properties* and click **Add property**. Assigned properties
   can be removed again with **Remove property**; the selected property's
   **Warning value at** and **Maximum value** can be set.
5. Click **Next**. If no task exists yet, an empty task is created automatically.
6. **Step 3 – Task:** enter the **Task Name**, the **Task description** and the
   **Planned Duration**. **Add task** adds further tasks, **Remove task** deletes the
   selected one, **Task documentation** opens the media documentation for the selected
   task.
7. Click **Next**.
8. **Step 4 – Performer:** select a **User**, **Group** or **Company** tab and pick an
   entry, then click **Add performer**. **Remove performer** deletes the selected
   assignment, **Unselect** clears the selection.
9. Click **Next** — the template is saved and appears in the list.
   **Back** returns to the previous step; **Close** cancels the wizard without saving.

## Editing and deleting

* Select a template in the list (or navigate with **First record** / **Previous
  record** / **Next record** / **Last record**); the template is loaded into the
  detail area.
* Change the fields and click **Save**. **Undo save** discards the changes.
* **Delete** removes the selected template *together with all its tasks and
  assignments* — the deletion is executed immediately in the database.
* When a component is selected in the project tree, the list automatically shows all
  templates of this component and of its class hierarchy.
* After each save the maintenance cache of the runtime services is rebuilt, so the
  changed rules take effect immediately.

## Field reference (Period and event tab)

| Field | Meaning |
|---|---|
| **Maintenance Interval** | Interval between two maintenances |
| **Last Maintenance Term** | When the maintenance was last performed |
| **Next Maintenance Term** | When the next maintenance is due |
| **Warning Difference** | Time span before the next term at which a warning is raised |
| **Search Keyword** | Filter for the property search (group name or property name) |
| **Filter Mode** | *By Group* or *By Property Name* |
| **Warning value at** | Value of the property at which a warning is raised |
| **Maximum value** | Maximum allowed value of the property |

## For AI agents (MCP)

See the class summary of `BSOMaintOrderTemplate` (via `get_type_infos`). Key methods:
`SearchWord` → `MaintOrderList` → `NavigateFirst()` → `CurrentMaintOrder`; wizard:
`New()`, `WizzardNext()`, `WizzardBack()`, `CloseWizzard()`; properties:
`SearchProperties()`, `AddPropertyToMaintACClass()`, `RemovePropertyFromMaintACClass()`;
tasks: `AddNewTask()`, `RemoveTask()`, `OpenTaskDocumentation()`; performers:
`AddNewMaintAssignment()`, `RemoveMaintAssignment()`, `UnselectPerformer()`;
`Save()`, `Delete()`, `Load()`.
