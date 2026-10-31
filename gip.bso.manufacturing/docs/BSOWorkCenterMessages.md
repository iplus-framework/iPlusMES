# Work Center Messages

> Business object: `BSOWorkCenterMessages` (Assembly `gip.bso.manufacturing`,
> child app of **Work center**)

## Definition

**Work center messages** is a function tab of the **Work center** app. It collects
all **messages** and **alarms** of the selected work center in one list so that the
operator can see and acknowledge them without leaving the work center.

Two kinds of entries appear in the list:

* **Alarms** of the process module — raised and cleared automatically by the
  control system. When the alarm state of the module changes, the list is updated
  automatically (entries disappear when the alarm is gone).
* **User acknowledgement messages** — messages of workflow nodes that explicitly
  wait for a user acknowledgement (user acknowledgement and decision nodes of the
  running workflow). They remain in the list until they are acknowledged in the
  workflow.

## Concepts

| Term | Meaning |
|---|---|
| **Message list** | Thread-safe list of all current messages/alarms of the work center |
| **Alarm message** | Alarm of the process module (source, caption, message text); managed automatically |
| **User acknowledgement message** | Message of a workflow node waiting for user acknowledgement |
| **Acknowledged messages** | Messages that have been acknowledged by the user |

## Screen layout

The tab shows the **message list** of the current work center. Each entry shows the
message text (for alarms: source, caption and message). The list is refreshed
automatically — no manual refresh is needed:

* when a workflow node that requires user acknowledgement starts or stops,
* when an alarm is raised or cleared on the process module.

## Working with messages

1. Select the **Work center** app and choose the desired work center — the
   **messages** tab is activated automatically with the work center.
2. Watch the list: alarms appear and disappear automatically; user acknowledgement
   messages stay until they are acknowledged.
3. Acknowledge a selected message with the acknowledge function of the work center
   (provided by the concrete work center configuration).

## Field reference

| Field | Description |
|---|---|
| Message | Message text; for alarms composed of *source: caption message* |
| Acknowledged messages | List of messages already acknowledged by the user |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOWorkCenterMessages`
contains the usage recipe. Key members:

* `MessagesList` — current messages/alarms (`MessagesListSafe` is the thread-safe source),
* `SelectedMessage` — highlighted entry,
* `AckMessageList` / `SelectedAckMessage` — acknowledged messages,
* `AcknowledgeMessage()` — acknowledges the selected message (virtual; implemented
  by derived work center message BSOs),
* `RefreshMessageList()` — re-reads the thread-safe list into the observable collection.

The BSO is activated and deactivated by the `BSOWorkCenterSelector`
(`Activate(processModule)` / `DeActivate()`); an agent normally works on it through
the parent work center BSO.
