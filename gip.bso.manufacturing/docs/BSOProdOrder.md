# Production Order

> Business object: `BSOProdOrder` (Assembly `gip.bso.manufacturing`, Menu: **Manufacturing → Production Order**)

## Definition

A **production order** is the central document of order-related production. It bundles
one or more **order lines**, each of which is an instance of a **bill of material (BOM)**
with its own target quantity. Every order line contains its **components** (materials to
be consumed), its **intermediates** (products of the material workflow that are produced
and consumed within the order) and, if required, **batches** in which the intermediates
are produced.

The production order app covers the whole life cycle of an order:

* create and maintain orders and order lines,
* plan components, intermediates and batches,
* create and post planned bookings (goods issue / goods receipt),
* monitor open (unfinished) bookings and finish the order.

## Concepts

| Term | Meaning |
|---|---|
| **Production order** | Order header with order number, order state and one or more order lines |
| **Order line** | Instance of a BOM within the order, with sequence and target quantity |
| **Component** | Material position of an order line that is consumed (outward) |
| **Intermediate** | Inward position of an order line — a product of the material workflow that is produced inside the order |
| **Batch** | Production lot of an intermediate; a batch position copies the input relations of the intermediate |
| **Overall lot / partial lot** | Lot number of an intermediate or batch position; partial lots subdivide an overall lot |
| **Planned booking** | Planned goods issue (outward) or goods receipt (inward) that is posted later |
| **Open postings** | Planned bookings that are not yet posted; the order can only be finished when none are left |
| **BOM explosion** | Resolves the BOM of an order line into its subordinate BOMs and adds them as order lines |
| **Supply picking** | Picking order that provides material for a component |

## Screen layout

The app consists of a **search list** (left) and the **order form** (right). The order
form has these tabs:

| Tab | Content |
|---|---|
| **Order header** (first tab) | Order line header: BOM, material, target/actual quantities, statistics, dates, order state; **Change Quantity** |
| **Order lines** | List of order lines (sequence, BOM, material, target/actual quantity, state) with **Add**, **BOM-Expand**, **Connect production order lines** |
| **Components** | Component list of the selected order line with component detail form (component data, alternatives, facility reservations, analysis) |
| **Intermediate** | Intermediates of the selected order line with batch assignment, lots and the input/output booking forms |
| **Batch** | Batch list of the selected order line with batch detail form |
| **Workflow** | Process workflow selection, source selection rules, preferred parameters, supply pickings |

The **Intermediate** tab is itself nested: the input side shows the **input list** and the
**input form** (details, booking history, planned bookings); the output side shows the
**output details** (batch, intermediate, lab report) and the **planned inward bookings**.

The search list offers filters for **production status**, **material no./name**,
**output material**, **MRP planning** and the **time filter mode** with **from/to** dates,
plus a **select all** checkbox for recalculating several orders at once.

## Creating a new production order (step by step)

1. Click **New** in the toolbar. A new production order with a new order number is
   created and opened.
2. Click **Add** on the *Order lines* tab. The **Choose BOM** dialog opens.
3. Select a bill of material, enter the **Sequence** and the **Target Quantity** and
   confirm with **OK**. The BOM must be enabled and within its validity period.
4. Click **Save**.

## Adding and maintaining order lines

* **Add** — opens the *Choose BOM* dialog (see above).
* **Delete** (order line) — removes the selected order line after a confirmation question.
* **Change Quantity** — opens the change-quantity dialog. Enter the **New Target Quantity**;
  with **Recalc all quantities** the component quantities are recalculated according to the BOM.
* **BOM-Expand** — explodes the BOM of the selected order line into a tree. Check the
  subordinate BOMs to add and confirm with **OK**; they are inserted as new order lines and
  connected with **Connect production order lines**.
* **Start** — starts the selected order line (reserved for start checks).

## Managing components (Components tab)

1. Select an order line; its components are listed.
2. **New Component** adds a component line. Assign the **Material** and enter the quantity,
   then **Save**.
3. **Delete Component** removes the selected component.
4. **Change target quantity according to BOM** — recalculates the component quantity from
   the new order quantity and the BOM reference size; confirm with **OK**.
5. **Alternate Component** adds an alternative (substitute) component to the selected
   component; **Delete Alternative Component** removes it.
6. **Create pickings for supply** creates supply pickings for the selected component;
   **Show pickings for supply** lists the existing ones.

## Intermediates, batches and lots (Intermediate tab)

1. Select an order line; its intermediates are listed.
2. **New Batch** opens the batch creation dialog with three sections:
   * **Depth** — select which intermediate depth levels are included.
   * **Automatic** — create batches by **batch size** or **batch count**; **Calculate**
     previews the batch definition, **OK** creates the batches.
   * **Manually** — enter **batch count** and **batch size** and create the batches.
3. Select a batch in the batch list and click **Assign Batch** to assign it to the
   selected intermediate. **Unassign** removes the batch position again,
   **Unselect** clears the selection.
4. **New overall lot** creates and assigns a new lot number for the selected intermediate
   or batch position (only for lot-managed materials). **New partial lot** /
   **Delete partial lot** manage partial lots of a batch position.
5. **Recalculate Totals** recalculates the sums of the intermediates — the system asks
   whether the calculation should run over the whole material workflow network.
6. **Sum bookings to Actual Quantity** sums the posted bookings to the actual quantity of
   the selected intermediate or batch position.

## Booking material movements (input / output forms)

The input side of an intermediate lists the **inputs** (consumed materials):

* **New Input** / **Delete Input** — add or remove an input line.
* **New Posting** — creates a planned booking (goods issue) for the input.
* **Choose facility** / **Choose quant** — select the storage bin resp. the stock quant
  for the planned booking.
* **Post Item** — posts the selected planned booking; **Post All** posts all planned
  bookings of the list.
* **Cancel Posting** / **Delete Posting** — cancel or remove a planned booking.

The output side works the same way for the **goods receipt** of the intermediate
(**New Posting**, **Post Item**, **Post All**, **Cancel Posting**, **Delete Posting**,
**Choose facility**).

### Correcting a posted booking

In the booking history of an input or output, select the posted booking and its quant
and click **Correct posting**. A new planned booking is created for the stock quant of
the selected booking charge; if the quant stock is fully consumed afterwards, the system
asks whether the quant should be set to *not available*.

### Issuing an alternative material

On the input (outward) booking form, the **Alternative material** list shows the
substitute materials of the input. Selecting one issues this material instead of the
booking material; clearing the selection restores the original material. The facility
list is filtered accordingly.

With the **Only show bins with material** checkbox the facility list is restricted to
storage bin containers that contain the (booking or alternative) material; without it,
all storage bins (and preparation bins on the outward side) are offered.

On the output details you can also create and view lab documents:
**New Lab Order** and **Lab Report View**.

## Open postings and finishing the order

1. **Check for open postings** lists all unfinished planned bookings of the current order.
   If there are none, back- and foreflushed stocks are balanced.
2. **Navigate to open posting** jumps through the tabs directly to the selected open
   booking so it can be completed.
3. **Finish Order** finishes the production order: it checks for open postings, balances
   stocks, recalculates quantities and statistics and sets the order state to *finished*.
   The order can only be finished when no open postings are left.
4. **Recalculate Totals** recalculates all totals of the current order;
   **Recalculate Totals for all** does this for all orders marked in the search list.

## Workflow tab

* Select the **process workflow** of the order line (from the material workflow of the BOM).
* **Select Sources** — opens the source selection rules of the selected workflow node.
* Preferred parameters — opens the preferred parameter dialog of the selected workflow node.
* **Create pickings for supply** — creates supply pickings for the selected component.

## Batch list (Batch tab)

* **Add** — creates a new batch for the selected order line.
* **Delete** — deletes the selected batch with its positions and relations (after
  confirmation).
* **Delete All** — deletes all batches of the selected order line.

## Field reference (order line header)

| Field | Description |
|---|---|
| Program No. | Order number of the production order |
| Order state | Production status of the order (e.g. created, released, finished) |
| Sequence | Position of the order line within the order |
| BOM / BOM No. / Version | Bill of material with number and version the order line is based on |
| Material | Output material of the order line |
| Material workflow | Material workflow of the BOM |
| Target Quantity / Actual Quantity | Planned and produced quantity (with good/scrap split) |
| Difference Quantity | Deviation between target and actual quantity |
| Start date / End date | Planned start and end of the order line |
| Target delivery date | Requested delivery date |
| Valid from / Valid to / Enabled | Validity of the underlying BOM |
| Comment / Loss comment | Free text and loss remarks |
| Statistics columns | Planned/good/scrap/total products, target/actual/difference input (absolute and %) |

## For AI agents (MCP)

The metadata of this business object is exposed through the iPlus MCP Server
(`get_thesaurus`, `get_type_infos`, `get_instance_info`, `get_method_info`,
`execute_acurl_command`). The class-level `<summary>` of `BSOProdOrder` contains the
complete usage recipe (search → navigate → edit → save) including the C# method names
(`Search`, `New`, `Save`, `Delete`, `Load`, `AddPartslist`, `NewProdOrderPartslistPos`,
`ProdOrderIntermediateBatchCreateDlg`, `ProdOrderIntermediateBatchAssign`,
`GenerateLotNumber`, `CheckForOpenPostings`, `FinishOrder`, ...), so an agent can operate
the production order app remotely, e.g. for user training. The button captions above are
the translations of these methods (`ACMethod*` attributes, stored in `ACClassText`).
Booking methods (post/cancel planned bookings) are defined in the partial classes
`BSOProdOrderInward` / `BSOProdOrderOutward`.
