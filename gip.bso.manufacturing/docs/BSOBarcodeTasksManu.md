# Tasks by Barcode in Manufacturing

> Business object: `BSOBarcodeTasksManu` (Assembly `gip.bso.manufacturing`)

## Definition

**Tasks by Barcode in Manufacturing** is the anchor business object for the
barcode task screens of the manufacturing module. Currently it is an empty
placeholder: the class adds no properties or methods of its own. The actual
barcode task logic is implemented in the process modules and functions that
are bound to the designs of this business object.

## Usage

Scanning a barcode at a production work center opens the task screen bound to
this business object; the scanned code is processed by the underlying process
module/function (e.g. starting, acknowledging or completing a task).

## For AI agents (MCP)

The class-level `<summary>` of `BSOBarcodeTasksManu` describes the current
state: an empty `ACBSOvb` placeholder without own workflow methods. There is
nothing to operate remotely yet; use the process modules/functions bound to
its designs instead.
