# Jobentry — Order Entry (Aufgabenerfassung)

> Business object: `BSOOutOrderPos` (Assembly `gip.bso.sales`, Menu: **Sales → Jobentry**)

## Definition

The **Jobentry** app is used to create and maintain order positions (**OutOrderPos**)
with their parent order (**OutOrder**), including customer and address assignment.

## Concepts

| Term | Meaning |
|---|---|
| **Order** | Customer order header |
| **Order position** | A position of the order with material and quantity |
| **Customer** | Company flagged as customer |
| **Billing address** | Address of the customer used for invoicing |
| **Delivery address** | Address of the customer used for delivery |

## Screen layout

The main layout contains the tab **Order acquisition** (header "Order acquisition")
with the order position form, and the **Explorer** (search list) docked at the bottom
(auto-hide).

## Creating a new order position (step by step)

1. Click **New** — a new, empty order position is created and opened.
2. Select the **Customer** from the customer list (companies flagged as customers).
3. The **Billing address** and **Delivery address** lists are filled from the selected
   customer's addresses; choose the desired addresses.
4. Fill in the remaining required fields (material, quantity, dates).
5. Click **Save**.

## Working with records

* **Search**: enter a search string and click **Search**; results appear in the list.
* **Edit**: change fields on the current record, then click **Save** before navigating
  to the next record or creating a new one.
* **Delete**: click **Delete** to remove the current order position — only possible if
  no foreign key relationships exist from other tables. Always click **Save**
  afterwards to execute the delete in the database.
* **Undo**: click **Undo** to discard unsaved changes.
* **Load**: click **Load** to re-read the current record if another user changed it
  in the background.

## Field reference

| Field | Description |
|---|---|
| Customer | Company of the order (selection from customer list) |
| Billing address | Invoicing address of the customer |
| Delivery address | Shipping address of the customer |
| Material / quantity / dates | Position data as configured in the query |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOOutOrderPos` contains the
complete usage recipe (search → navigate → edit → save, `New`, `Delete`) with the C#
method names (`Search`, `NavigateFirst`, `New`, `Save`, `UndoSave`, `Load`, `Delete`,
plus `CompanyList`, `BillingCompanyAddressList`, `DeliveryCompanyAddressList`,
`CurrentBillingCompanyAddress`, `CurrentDeliveryCompanyAddress`), so an agent can
operate the app remotely, e.g. for user training. The button captions above are the
translations of these methods (`ACMethod*` attributes, stored in `ACClassText`).
