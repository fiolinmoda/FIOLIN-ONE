# Fabric Management

## Purpose

The Fabric Management module controls the full lifecycle of fabric from supplier purchase through warehouse storage, production reservation, cutting consumption, and remaining fabric tracking. Its goal is to give FIOLIN ONE accurate, roll-level visibility over fabric inventory, quality, availability, and consumption so production planning can rely on real stock instead of approximate totals.

This module must support clothing manufacturing operations where fabric is purchased from suppliers, received in rolls or lots, verified by weight or meter, stored in the warehouse, reserved for production orders, and consumed during cutting. It must also preserve a traceable history of every movement, difference, and adjustment.

---

## Business Process

The complete business flow is:

Supplier

↓

Fabric Purchase

↓

Fabric Arrival

↓

Weight / Meter Verification

↓

Quality Check

↓

Warehouse Placement

↓

Production Reservation

↓

Cutting

↓

Remaining Fabric

The process starts when a supplier provides fabric based on a purchase decision or production need. The purchased fabric arrives at the warehouse and is verified against supplier documents, labels, and expected quantity. After verification and quality control, accepted rolls are placed into warehouse locations. When production orders require fabric, available rolls are reserved. During cutting, fabric is consumed from reserved rolls, and any remaining fabric is returned, adjusted, or kept available depending on business rules.

The system must make the current state of each fabric roll clear at every step: expected, arrived, accepted, rejected, stored, reserved, consumed, partially consumed, or closed.

---

## Master Data

The module depends on stable master data. These records should be maintained before fabric transactions begin.

### Fabric

The primary fabric definition. It represents the reusable fabric card used across purchases, warehouse, and production.

### Fabric Type

Classification such as woven, knitted, denim, lining, rib, interlock, or other company-defined types.

### Supplier

The vendor or producer that provides fabric.

### Color

Standardized color master data used to identify fabric rolls and align them with product requirements.

### Warehouse

Physical warehouse, area, rack, shelf, or location structure where fabric is stored.

### Roll

The individually tracked unit of fabric inventory. A fabric may have many rolls, and each roll can have different quantities, lots, statuses, and movement history.

---

## Fabric Card

The Fabric Card stores the standard definition of a fabric. It should not represent physical stock by itself; physical stock is represented by rolls and stock movements.

Fields:

- Fabric Code
- Fabric Name
- Fabric Type
- Supplier
- Composition
- Width
- Weight (GSM)
- Unit (Kg / Meter)
- Active

Additional future fields may include shrinkage percentage, washing instructions, default warehouse, default color group, minimum stock level, and purchase lead time.

---

## Fabric Roll

Each roll is tracked separately. This is essential because roll quantities, lots, colors, supplier labels, quality results, and warehouse placement may differ even when the fabric card is the same.

Fields:

- Roll Number
- Fabric
- Color
- Lot Number
- Supplier
- Original Quantity
- Current Quantity
- Unit
- Status

Recommended statuses include:

- Expected
- Received
- Accepted
- Rejected
- In Warehouse
- Reserved
- In Cutting
- Partially Consumed
- Consumed
- Closed

The roll record must preserve original quantity and current quantity separately. Original quantity represents the confirmed quantity at receiving. Current quantity changes through reservations, cutting consumption, transfers, adjustments, and returns.

---

## Warehouse Rules

Same fabric rolls should preferably be stored together. This improves warehouse efficiency, reduces picking mistakes, and makes roll counting easier.

If there is no space, different fabrics may be stacked or stored together in the same location. When this happens, FIOLIN ONE must still keep exact roll-level location records so users can identify where each roll is physically placed.

Every movement must be recorded. A movement includes receiving, placement, transfer, reservation, release, adjustment, cutting consumption, return from cutting, and disposal. No roll quantity should change without a movement record.

Warehouse rules should support:

- Roll-level traceability
- Location-level visibility
- Prevention of negative stock
- Clear movement reason codes
- User and timestamp auditability
- Optional approval for manual adjustments

---

## Receiving Process

The receiving process confirms that fabric delivered by the supplier matches the purchase expectation and physical reality.

Arrival

↓

Weighing

↓

Comparison with supplier label

↓

Acceptance

↓

Difference handling

↓

Warehouse placement

When fabric arrives, warehouse staff record the supplier, fabric, color, lot number, roll number, and supplier-declared quantity. The roll is then weighed or measured depending on the unit. The verified quantity is compared with the supplier label and purchase document.

If the difference is within accepted tolerance, the roll can be accepted. If the difference exceeds tolerance, the system should require a difference reason and business decision such as accept with difference, reject, quarantine, or supplier claim.

After acceptance, the roll is assigned to a warehouse location. The placement creates an inbound stock movement and updates current quantity.

---

## Reservation

Production reserves fabric when a production order or cutting plan requires it. Reservation does not consume stock immediately; it blocks available quantity so the same fabric cannot be promised to another order.

Reservation rules should include:

- Reserve by fabric, color, and required quantity
- Prefer rolls from the same lot when possible
- Reserve full rolls or partial roll quantities
- Track reserved quantity separately from consumed quantity
- Allow reservation release if production changes
- Prevent reservation beyond available quantity

The reservation should reference the production order, product, color, and planned cutting quantity. Users should be able to see available, reserved, and current quantities.

---

## Cutting Consumption

Cutting consumes fabric from reserved rolls. Consumption should be recorded when the cutting operation confirms actual usage.

Cutting consumption should support:

- Selecting reserved rolls
- Recording actual consumed quantity
- Recording remaining quantity
- Returning unused fabric to warehouse availability
- Closing fully consumed rolls
- Capturing waste or cutting loss

If actual consumption differs from planned consumption, FIOLIN ONE should preserve the difference for reporting. This helps identify fabric waste, pattern efficiency, supplier differences, or operational issues.

---

## Stock Movements

Fabric stock must be movement-driven. Current stock should be the result of movement history, not manual editing.

### Inbound

Created when fabric is accepted into stock after receiving.

### Outbound

Created when fabric leaves the warehouse outside production consumption, such as supplier return, disposal, or external transfer.

### Transfer

Created when a roll moves from one warehouse location to another.

### Adjustment

Created when authorized users correct quantity due to counting, damage, measurement error, or administrative correction.

### Consumption

Created when cutting consumes fabric for production.

Each movement should include movement type, roll, source location, destination location, quantity, unit, related document, user, timestamp, and reason.

---

## Database Proposal

Suggested entities:

- Fabric
- FabricType
- Supplier
- FabricRoll
- FabricLot
- Warehouse
- WarehouseLocation
- FabricStockMovement
- FabricReservation
- FabricReservationLine
- FabricReceiving
- FabricReceivingLine
- FabricQualityCheck
- FabricCuttingConsumption
- FabricAdjustmentReason
- FabricMovementReason

Key relationships:

- Fabric has one FabricType
- Fabric can have one default Supplier
- FabricRoll belongs to Fabric, Color, Supplier, and optionally Lot
- FabricRoll has many FabricStockMovements
- FabricReservation references ProductionOrder
- FabricReservationLine references FabricRoll
- FabricCuttingConsumption references ProductionOrder, Cutting operation, and FabricRoll

---

## API Proposal

Suggested endpoints:

- `GET /api/fabrics`
- `GET /api/fabrics/{id}`
- `POST /api/fabrics`
- `PUT /api/fabrics/{id}`
- `DELETE /api/fabrics/{id}`
- `GET /api/fabric-rolls`
- `GET /api/fabric-rolls/{id}`
- `POST /api/fabric-rolls`
- `PUT /api/fabric-rolls/{id}`
- `GET /api/fabric-rolls/{id}/movements`
- `POST /api/fabric-receivings`
- `GET /api/fabric-receivings/{id}`
- `POST /api/fabric-receivings/{id}/accept`
- `POST /api/fabric-receivings/{id}/reject`
- `POST /api/fabric-reservations`
- `GET /api/fabric-reservations/{id}`
- `POST /api/fabric-reservations/{id}/release`
- `POST /api/fabric-cutting-consumptions`
- `POST /api/fabric-stock-movements/transfer`
- `POST /api/fabric-stock-movements/adjustment`

API responses should expose roll-level quantities and movement history clearly. Commands that change stock must validate available quantity and should return the updated roll state.

---

## UI Proposal

Suggested screens:

- Fabric List
- Fabric Detail
- Fabric Roll List
- Fabric Roll Detail
- Fabric Receiving List
- Fabric Receiving Detail
- Fabric Arrival Entry
- Weight / Meter Verification
- Quality Check
- Warehouse Placement
- Fabric Reservation List
- Fabric Reservation Detail
- Production Fabric Reservation
- Cutting Consumption Entry
- Fabric Stock Movement List
- Fabric Transfer
- Fabric Adjustment
- Roll History
- Fabric Inventory Report
- Fabric Consumption Report
- Supplier Difference Report

UI behavior should emphasize scanning, fast data entry, and clear exception handling for warehouse users.

---

## Reports

### Inventory

Shows current fabric stock by fabric, color, lot, supplier, warehouse, location, and roll.

### Fabric Consumption

Shows planned versus actual fabric usage by production order, product, cutting operation, and date range.

### Supplier Differences

Shows differences between supplier-declared quantity and verified received quantity.

### Roll History

Shows the complete lifecycle and movement trail of a roll from receiving through consumption or closure.

---

## Future Improvements

### Barcode

Use barcode labels for roll identification, warehouse movements, and cutting consumption.

### QR Code

Use QR codes to encode richer roll data such as fabric, color, lot, supplier, and quantity.

### RFID

Use RFID for faster warehouse counting and roll tracking in high-volume operations.

### Automatic Scale Integration

Connect digital scales directly to FIOLIN ONE to reduce manual entry errors during receiving and verification.

---

## Status

Draft

## Last Updated

2026-07-04
