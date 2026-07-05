# Purchasing Management

## Purpose

The Purchasing Management module manages supplier purchasing from purchase order creation until invoice completion. It provides FIOLIN ONE with a controlled process for creating purchase orders, tracking supplier confirmation, receiving goods into inventory, and completing the purchase after supplier invoice entry.

The first purchasing scope focuses on fabric purchasing for production needs. Fabric purchasing is operationally connected to Fabric Management, Warehouse, Production Planning, and Finance. The module must support cases where goods arrive before the supplier invoice, because warehouse inventory must be received and made available without waiting for accounting completion.

For the current phase, the module tracks purchase price only. Freight, customs, additional costs, and landed cost allocation are intentionally ignored until a future phase.

---

## Business Rules

- Fabric purchasing is performed by the Production Manager (Vahap).
- One purchase order can contain multiple different fabrics.
- A purchase order may also contain accessories in the future.
- Goods may arrive before the supplier invoice.
- Inventory can be received before invoice.
- Invoice is entered later.
- Only purchase price is tracked.
- Freight and additional costs are ignored for now.

These rules mean purchasing and inventory receiving must be separated. A purchase order can be operationally received by the warehouse even when the invoice has not arrived. The purchase is not financially complete until the invoice is entered and matched to the purchase order.

---

## Workflow

Purchase Request

↓

Purchase Order

↓

Supplier Confirmation

↓

Goods Receipt

↓

Warehouse Entry

↓

Invoice Entry

↓

Purchase Completed

The workflow begins when production identifies a fabric need. The Production Manager creates or approves the purchase order and sends it to the supplier. After supplier confirmation, the order remains open until goods arrive. Warehouse staff receives the delivered goods against the purchase order, records accepted quantities, captures differences, and enters the goods into warehouse stock. The supplier invoice can be entered later. Once the expected goods and invoice are completed, the purchase order can be closed.

---

## Purchase Order

The Purchase Order is the main purchasing document. It represents the company's commitment to buy fabric, and later accessories, from a supplier.

Fields:

- Purchase Number
- Supplier
- Order Date
- Expected Date
- Status
- Notes

The purchase order header should store the supplier, dates, status, and general notes. Detailed material requirements are stored in purchase order items.

Purchase orders should support partial receiving. A single order may be received across multiple deliveries, and each receipt should update received and remaining quantities.

---

## Purchase Order Item

Purchase Order Items represent the fabrics or future accessories being purchased.

Fields:

- Fabric
- Color
- Quantity
- Unit
- Unit Price
- Received Quantity
- Remaining Quantity
- Status

Each item must track ordered quantity, received quantity, and remaining quantity. Remaining quantity should be calculated from ordered quantity minus accepted received quantity.

Item status should reflect fulfillment independently from the purchase order header. For example, one fabric may be fully received while another fabric in the same purchase order is still pending.

---

## Purchase Status

Supported purchase statuses:

- Draft
- Approved
- Partially Received
- Fully Received
- Waiting Invoice
- Completed
- Cancelled

### Draft

The purchase order is being prepared and has not been approved or sent.

### Approved

The purchase order is approved by the responsible user and can be sent to the supplier.

### Partially Received

At least one item has been received, but one or more items still have remaining quantity.

### Fully Received

All ordered quantities have been received, but the invoice may not yet be entered.

### Waiting Invoice

Goods have been received and warehouse entry is complete, but the supplier invoice has not been entered.

### Completed

Goods receipt and invoice entry are complete.

### Cancelled

The purchase order is cancelled and should not accept additional receipt or invoice actions.

---

## Supplier

Supplier records are shared master/business data used by purchasing, fabric management, and finance.

Fields:

- Supplier Code
- Supplier Name
- Phone
- Email
- Address
- Tax Number
- Payment Term
- Active

Supplier status controls whether the supplier can be selected for new purchase orders. Historical purchase orders should keep their supplier reference even if the supplier later becomes inactive.

---

## Goods Receipt

Goods Receipt records warehouse receiving against a purchase order. Warehouse staff enters:

- Supplier
- Purchase Order
- Received Quantity
- Acceptance
- Difference
- Warehouse
- Date

The receiving process confirms what physically arrived. Warehouse staff should select the purchase order, identify the received fabric and color, enter received quantity, and record whether the goods are accepted.

If received quantity differs from the purchase order item quantity or supplier label, the difference must be recorded. Difference handling should support accepted difference, rejected quantity, supplier claim, or manual review.

Goods receipt creates inventory availability before the invoice is entered. For fabrics, receiving may also create fabric roll records if roll-level tracking is enabled.

---

## Invoice

The Purchase Invoice records the supplier invoice after goods are received or when the invoice arrives.

Fields:

- Invoice Number
- Invoice Date
- Supplier
- Purchase Order
- Invoice Amount
- Status

The invoice should be linked to the supplier and purchase order. In the current phase, only the purchase price is tracked. Additional costs such as freight, customs, service fees, and landed cost allocation are excluded.

Invoice status should allow draft and completed states. Future finance integration may add approval, payment, and accounting transfer statuses.

---

## Database Proposal

Suggested entities:

### PurchaseOrder

Stores purchase order header information such as purchase number, supplier, order date, expected date, status, and notes.

### PurchaseOrderItem

Stores purchased fabric lines, including fabric, color, quantity, unit, unit price, received quantity, remaining quantity, and line status.

### PurchaseInvoice

Stores supplier invoice information linked to purchase order and supplier.

### Supplier

Stores supplier master data including contact, tax, payment, and active status.

### GoodsReceipt

Stores receiving header information such as supplier, purchase order, warehouse, receipt date, and receiving status.

Additional future entities may include GoodsReceiptItem, PurchaseApproval, PurchaseAttachment, PurchaseOrderStatusHistory, and SupplierPriceHistory.

---

## API Proposal

Suggested REST endpoints:

- `GET /api/purchase-orders`
- `GET /api/purchase-orders/{id}`
- `POST /api/purchase-orders`
- `PUT /api/purchase-orders/{id}`
- `DELETE /api/purchase-orders/{id}`
- `POST /api/purchase-orders/{id}/approve`
- `POST /api/purchase-orders/{id}/cancel`
- `GET /api/purchase-orders/{id}/items`
- `POST /api/purchase-orders/{id}/items`
- `PUT /api/purchase-orders/{id}/items/{itemId}`
- `DELETE /api/purchase-orders/{id}/items/{itemId}`
- `GET /api/suppliers`
- `GET /api/suppliers/{id}`
- `POST /api/suppliers`
- `PUT /api/suppliers/{id}`
- `DELETE /api/suppliers/{id}`
- `GET /api/goods-receipts`
- `GET /api/goods-receipts/{id}`
- `POST /api/goods-receipts`
- `POST /api/goods-receipts/{id}/complete`
- `GET /api/purchase-invoices`
- `GET /api/purchase-invoices/{id}`
- `POST /api/purchase-invoices`
- `PUT /api/purchase-invoices/{id}`
- `POST /api/purchase-invoices/{id}/complete`

APIs that receive goods must validate that received quantity does not incorrectly exceed remaining quantity unless an authorized difference handling rule is applied.

---

## UI Proposal

Required screens:

### Dashboard

Shows open purchase orders, pending receipts, waiting invoices, and supplier purchase summaries.

### Purchase Orders

List screen for searching, filtering, and creating purchase orders.

### Purchase Order Detail

Header and item management screen for fabrics, quantities, unit prices, status, notes, and receiving progress.

### Suppliers

Supplier master data screen for supplier records and active status.

### Goods Receipt

Warehouse receiving screen for selecting purchase order, entering received quantities, recording differences, and confirming warehouse entry.

### Invoices

Invoice entry screen for supplier invoice number, invoice date, purchase order link, and invoice amount.

### Reports

Reporting area for pending orders, waiting invoices, supplier purchases, purchase history, and purchase prices.

---

## Reports

### Pending Orders

Shows approved or partially received purchase orders with remaining quantities.

### Waiting Invoice

Shows fully or partially received purchases where the supplier invoice has not yet been entered.

### Supplier Purchases

Shows purchase totals and quantities by supplier and date range.

### Purchase History

Shows all purchase order, receipt, and invoice history for audit and review.

### Purchase Prices

Shows unit purchase prices by fabric, supplier, color, and date. This supports future price comparison and supplier negotiation.

---

## Future Improvements

### Approval Workflow

Introduce approval steps for purchase orders based on amount, supplier, or fabric category.

### PDF Purchase Order

Generate professional purchase order PDFs for supplier communication.

### Email Purchase Order

Send purchase orders directly to suppliers from FIOLIN ONE.

### Supplier Portal

Allow suppliers to confirm orders, upload invoices, and share delivery status.

### Automatic Invoice Matching

Automatically match purchase invoice quantities and amounts with purchase order and goods receipt records.

---

## Status

Draft

## Last Updated

2026-07-04
