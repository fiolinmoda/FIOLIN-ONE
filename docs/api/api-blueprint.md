# FIOLIN ONE API Blueprint

This document becomes the official REST API specification for FIOLIN ONE.

## Purpose

The API Blueprint defines the standard structure, behavior, security model, response format, and endpoint map for all FIOLIN ONE backend services. It is the primary reference for backend development, frontend integration, mobile integrations, marketplace integrations, and future external API consumers.

The API must support a modular ERP and PLM platform for the apparel industry while keeping contracts predictable, secure, versioned, and easy to document through Swagger / OpenAPI.

## API Standards

- API Style: REST API
- Versioning: `/api/v1`
- Data Format: JSON
- Encoding: UTF-8
- Transport Security: HTTPS
- Authentication: JWT Authentication
- Authorization: Role Based Authorization
- Documentation: Swagger / OpenAPI
- Naming Style: Resource-based endpoint names
- Request Body Style: JSON DTOs
- Date Format: ISO 8601 UTC timestamps
- Error Format: Standard response envelope

### Base URL Pattern

```text
/api/v1/{resource}
```

Examples:

```text
GET /api/v1/products
GET /api/v1/products/{id}
POST /api/v1/products
PUT /api/v1/products/{id}
DELETE /api/v1/products/{id}
```

## Response Standard

Every endpoint must return a consistent response object.

### Standard Fields

- `success`
- `message`
- `data`
- `errors`
- `timestamp`

### Example Success Response

```json
{
  "success": true,
  "message": "Product retrieved successfully.",
  "data": {
    "id": "0b5d6a9a-6f5c-4c2f-8c3d-6c43f9a82511",
    "productCode": "MDL-1001",
    "productName": "Basic T-Shirt"
  },
  "errors": [],
  "timestamp": "2026-07-05T10:30:00Z"
}
```

### Example Validation Response

```json
{
  "success": false,
  "message": "Validation failed.",
  "data": null,
  "errors": [
    {
      "field": "productCode",
      "code": "Required",
      "message": "Product code is required."
    }
  ],
  "timestamp": "2026-07-05T10:30:00Z"
}
```

### Example Error Response

```json
{
  "success": false,
  "message": "An unexpected error occurred.",
  "data": null,
  "errors": [
    {
      "code": "InternalServerError",
      "message": "The request could not be completed."
    }
  ],
  "timestamp": "2026-07-05T10:30:00Z"
}
```

## Pagination Standard

List endpoints must support pagination.

### Query Parameters

- `page`
- `pageSize`
- `totalItems`
- `totalPages`

### Request Example

```text
GET /api/v1/products?page=1&pageSize=25
```

### Response Data Example

```json
{
  "items": [],
  "page": 1,
  "pageSize": 25,
  "totalItems": 250,
  "totalPages": 10
}
```

## Filtering

List endpoints should support consistent filtering where applicable.

### Common Query Parameters

- `search`
- `status`
- `dateFrom`
- `dateTo`
- `sortBy`
- `sortDirection`

### Request Example

```text
GET /api/v1/purchase-orders?search=SUP-001&status=Approved&dateFrom=2026-01-01&dateTo=2026-12-31&sortBy=orderDate&sortDirection=desc
```

## Authentication

Authentication APIs manage secure user access and session continuity.

### Login

```text
POST /api/v1/auth/login
```

Purpose: Authenticates the user and returns access and refresh tokens.

### Refresh Token

```text
POST /api/v1/auth/refresh-token
```

Purpose: Issues a new access token by validating an active refresh token.

### Logout

```text
POST /api/v1/auth/logout
```

Purpose: Invalidates the current refresh token or session.

### Current User

```text
GET /api/v1/auth/me
```

Purpose: Returns the authenticated user's profile, roles, and permissions.

### Password Change

```text
POST /api/v1/auth/change-password
```

Purpose: Allows an authenticated user to change their password.

### Forgot Password (Future)

```text
POST /api/v1/auth/forgot-password
POST /api/v1/auth/reset-password
```

Purpose: Supports future password reset flows.

## Authorization

Authorization must be role-based and permission-aware. Roles define default access, while permissions allow precise control per module and action.

### Administrator

- Full system access
- User and role management
- Permission management
- Master data management
- All operational modules
- System configuration
- Audit visibility

### Production Manager

- PLM read/write access
- Product read/write access
- Fabric reservation access
- Purchasing request and approval access
- Production order management
- Cutting and workshop tracking
- Production reports

### Warehouse

- Warehouse inventory access
- Goods receipt operations
- Fabric and product stock movements
- Location management
- Barcode scanning workflows
- Shipment preparation

### Sales

- Customer and dealer access
- Sales order management
- Marketplace order visibility
- Returns management
- Product and inventory read access
- Sales reports

### Finance

- Supplier and customer financial records
- Purchase invoice access
- Sales invoice access
- Payments and collections
- Expense records
- Financial reports

### Viewer

- Read-only access to permitted modules
- Dashboard visibility
- Report visibility based on permission
- No create, update, or delete access

## Master Data APIs

Master Data APIs follow the same CRUD pattern for stable reference data.

### Standard Operations

For every master data module:

- `GET /api/v1/{resource}`
- `GET /api/v1/{resource}/{id}`
- `POST /api/v1/{resource}`
- `PUT /api/v1/{resource}/{id}`
- `DELETE /api/v1/{resource}/{id}`

### Brands

- `GET /api/v1/brands`
- `GET /api/v1/brands/{id}`
- `POST /api/v1/brands`
- `PUT /api/v1/brands/{id}`
- `DELETE /api/v1/brands/{id}`

### Categories

- `GET /api/v1/categories`
- `GET /api/v1/categories/{id}`
- `POST /api/v1/categories`
- `PUT /api/v1/categories/{id}`
- `DELETE /api/v1/categories/{id}`

### Collections

- `GET /api/v1/collections`
- `GET /api/v1/collections/{id}`
- `POST /api/v1/collections`
- `PUT /api/v1/collections/{id}`
- `DELETE /api/v1/collections/{id}`

### Seasons

- `GET /api/v1/seasons`
- `GET /api/v1/seasons/{id}`
- `POST /api/v1/seasons`
- `PUT /api/v1/seasons/{id}`
- `DELETE /api/v1/seasons/{id}`

### Colors

- `GET /api/v1/colors`
- `GET /api/v1/colors/{id}`
- `POST /api/v1/colors`
- `PUT /api/v1/colors/{id}`
- `DELETE /api/v1/colors/{id}`

### Sizes

- `GET /api/v1/sizes`
- `GET /api/v1/sizes/{id}`
- `POST /api/v1/sizes`
- `PUT /api/v1/sizes/{id}`
- `DELETE /api/v1/sizes/{id}`

### Fabric Types

- `GET /api/v1/fabric-types`
- `GET /api/v1/fabric-types/{id}`
- `POST /api/v1/fabric-types`
- `PUT /api/v1/fabric-types/{id}`
- `DELETE /api/v1/fabric-types/{id}`

### Suppliers

- `GET /api/v1/suppliers`
- `GET /api/v1/suppliers/{id}`
- `POST /api/v1/suppliers`
- `PUT /api/v1/suppliers/{id}`
- `DELETE /api/v1/suppliers/{id}`

### Warehouses

- `GET /api/v1/warehouses`
- `GET /api/v1/warehouses/{id}`
- `POST /api/v1/warehouses`
- `PUT /api/v1/warehouses/{id}`
- `DELETE /api/v1/warehouses/{id}`

### Countries

- `GET /api/v1/countries`
- `GET /api/v1/countries/{id}`
- `POST /api/v1/countries`
- `PUT /api/v1/countries/{id}`
- `DELETE /api/v1/countries/{id}`

### Cities

- `GET /api/v1/cities`
- `GET /api/v1/cities/{id}`
- `POST /api/v1/cities`
- `PUT /api/v1/cities/{id}`
- `DELETE /api/v1/cities/{id}`

### Units

- `GET /api/v1/units`
- `GET /api/v1/units/{id}`
- `POST /api/v1/units`
- `PUT /api/v1/units/{id}`
- `DELETE /api/v1/units/{id}`

### Currencies

- `GET /api/v1/currencies`
- `GET /api/v1/currencies/{id}`
- `POST /api/v1/currencies`
- `PUT /api/v1/currencies/{id}`
- `DELETE /api/v1/currencies/{id}`

## Product APIs

Product APIs manage sellable product models, variants, commercial definitions, images, and barcode records.

### Products

- `GET /api/v1/products`
- `GET /api/v1/products/{id}`
- `POST /api/v1/products`
- `PUT /api/v1/products/{id}`
- `DELETE /api/v1/products/{id}`

### Variants

- `GET /api/v1/products/{productId}/variants`
- `GET /api/v1/product-variants/{id}`
- `POST /api/v1/products/{productId}/variants`
- `PUT /api/v1/product-variants/{id}`
- `DELETE /api/v1/product-variants/{id}`

### Prices

- `GET /api/v1/product-prices`
- `GET /api/v1/product-prices/{id}`
- `POST /api/v1/product-prices`
- `PUT /api/v1/product-prices/{id}`
- `DELETE /api/v1/product-prices/{id}`

### Images

- `GET /api/v1/products/{productId}/images`
- `GET /api/v1/product-images/{id}`
- `POST /api/v1/products/{productId}/images`
- `PUT /api/v1/product-images/{id}`
- `DELETE /api/v1/product-images/{id}`

### Barcodes

- `GET /api/v1/barcodes`
- `GET /api/v1/barcodes/{id}`
- `POST /api/v1/barcodes`
- `PUT /api/v1/barcodes/{id}`
- `DELETE /api/v1/barcodes/{id}`

## PLM APIs

PLM APIs manage the lifecycle of a model before production approval.

### Ideas

- `GET /api/v1/plm/ideas`
- `GET /api/v1/plm/ideas/{id}`
- `POST /api/v1/plm/ideas`
- `PUT /api/v1/plm/ideas/{id}`
- `DELETE /api/v1/plm/ideas/{id}`

### Models

- `GET /api/v1/plm/models`
- `GET /api/v1/plm/models/{id}`
- `POST /api/v1/plm/models`
- `PUT /api/v1/plm/models/{id}`
- `DELETE /api/v1/plm/models/{id}`

### Files

- `GET /api/v1/plm/models/{modelId}/files`
- `GET /api/v1/plm/files/{id}`
- `POST /api/v1/plm/models/{modelId}/files`
- `PUT /api/v1/plm/files/{id}`
- `DELETE /api/v1/plm/files/{id}`

### Samples

- `GET /api/v1/plm/models/{modelId}/samples`
- `GET /api/v1/plm/samples/{id}`
- `POST /api/v1/plm/models/{modelId}/samples`
- `PUT /api/v1/plm/samples/{id}`
- `DELETE /api/v1/plm/samples/{id}`

### Revisions

- `GET /api/v1/plm/models/{modelId}/revisions`
- `GET /api/v1/plm/revisions/{id}`
- `POST /api/v1/plm/models/{modelId}/revisions`
- `PUT /api/v1/plm/revisions/{id}`
- `DELETE /api/v1/plm/revisions/{id}`

### Approvals

- `GET /api/v1/plm/models/{modelId}/approvals`
- `GET /api/v1/plm/approvals/{id}`
- `POST /api/v1/plm/models/{modelId}/approvals`
- `PUT /api/v1/plm/approvals/{id}`
- `DELETE /api/v1/plm/approvals/{id}`

## Purchasing APIs

Purchasing APIs manage supplier procurement from purchase order creation through invoice completion.

### Purchase Orders

- `GET /api/v1/purchase-orders`
- `GET /api/v1/purchase-orders/{id}`
- `POST /api/v1/purchase-orders`
- `PUT /api/v1/purchase-orders/{id}`
- `DELETE /api/v1/purchase-orders/{id}`

### Purchase Order Items

- `GET /api/v1/purchase-orders/{purchaseOrderId}/items`
- `GET /api/v1/purchase-order-items/{id}`
- `POST /api/v1/purchase-orders/{purchaseOrderId}/items`
- `PUT /api/v1/purchase-order-items/{id}`
- `DELETE /api/v1/purchase-order-items/{id}`

### Goods Receipts

- `GET /api/v1/goods-receipts`
- `GET /api/v1/goods-receipts/{id}`
- `POST /api/v1/goods-receipts`
- `PUT /api/v1/goods-receipts/{id}`
- `DELETE /api/v1/goods-receipts/{id}`

### Invoices

- `GET /api/v1/purchase-invoices`
- `GET /api/v1/purchase-invoices/{id}`
- `POST /api/v1/purchase-invoices`
- `PUT /api/v1/purchase-invoices/{id}`
- `DELETE /api/v1/purchase-invoices/{id}`

### Suppliers

- `GET /api/v1/suppliers`
- `GET /api/v1/suppliers/{id}`
- `POST /api/v1/suppliers`
- `PUT /api/v1/suppliers/{id}`
- `DELETE /api/v1/suppliers/{id}`

## Fabric APIs

Fabric APIs manage fabric cards, inventory, movements, production reservations, and consumption.

### Fabric Cards

- `GET /api/v1/fabrics`
- `GET /api/v1/fabrics/{id}`
- `POST /api/v1/fabrics`
- `PUT /api/v1/fabrics/{id}`
- `DELETE /api/v1/fabrics/{id}`

### Inventory

- `GET /api/v1/fabric-inventory`
- `GET /api/v1/fabric-inventory/{id}`
- `POST /api/v1/fabric-inventory`
- `PUT /api/v1/fabric-inventory/{id}`
- `DELETE /api/v1/fabric-inventory/{id}`

### Movements

- `GET /api/v1/fabric-movements`
- `GET /api/v1/fabric-movements/{id}`
- `POST /api/v1/fabric-movements`
- `PUT /api/v1/fabric-movements/{id}`
- `DELETE /api/v1/fabric-movements/{id}`

### Reservations

- `GET /api/v1/fabric-reservations`
- `GET /api/v1/fabric-reservations/{id}`
- `POST /api/v1/fabric-reservations`
- `PUT /api/v1/fabric-reservations/{id}`
- `DELETE /api/v1/fabric-reservations/{id}`

### Consumption

- `GET /api/v1/fabric-consumption`
- `GET /api/v1/fabric-consumption/{id}`
- `POST /api/v1/fabric-consumption`
- `PUT /api/v1/fabric-consumption/{id}`
- `DELETE /api/v1/fabric-consumption/{id}`

## Production APIs

Production APIs manage production orders, planning, cutting, workshop activity, quality control, and finished goods.

### Production Orders

- `GET /api/v1/production-orders`
- `GET /api/v1/production-orders/{id}`
- `POST /api/v1/production-orders`
- `PUT /api/v1/production-orders/{id}`
- `DELETE /api/v1/production-orders/{id}`

### Planning

- `GET /api/v1/production-plans`
- `GET /api/v1/production-plans/{id}`
- `POST /api/v1/production-plans`
- `PUT /api/v1/production-plans/{id}`
- `DELETE /api/v1/production-plans/{id}`

### Cutting

- `GET /api/v1/cutting-plans`
- `GET /api/v1/cutting-plans/{id}`
- `POST /api/v1/cutting-plans`
- `PUT /api/v1/cutting-plans/{id}`
- `DELETE /api/v1/cutting-plans/{id}`

### Workshop

- `GET /api/v1/workshop-orders`
- `GET /api/v1/workshop-orders/{id}`
- `POST /api/v1/workshop-orders`
- `PUT /api/v1/workshop-orders/{id}`
- `DELETE /api/v1/workshop-orders/{id}`

### Quality Control

- `GET /api/v1/quality-control`
- `GET /api/v1/quality-control/{id}`
- `POST /api/v1/quality-control`
- `PUT /api/v1/quality-control/{id}`
- `DELETE /api/v1/quality-control/{id}`

### Finished Goods

- `GET /api/v1/finished-goods`
- `GET /api/v1/finished-goods/{id}`
- `POST /api/v1/finished-goods`
- `PUT /api/v1/finished-goods/{id}`
- `DELETE /api/v1/finished-goods/{id}`

## Warehouse APIs

Warehouse APIs manage physical stock, locations, transfers, shipments, boxes, and barcode-based workflows.

### Inventory

- `GET /api/v1/warehouse-inventory`
- `GET /api/v1/warehouse-inventory/{id}`
- `POST /api/v1/warehouse-inventory`
- `PUT /api/v1/warehouse-inventory/{id}`
- `DELETE /api/v1/warehouse-inventory/{id}`

### Locations

- `GET /api/v1/locations`
- `GET /api/v1/locations/{id}`
- `POST /api/v1/locations`
- `PUT /api/v1/locations/{id}`
- `DELETE /api/v1/locations/{id}`

### Transfers

- `GET /api/v1/transfers`
- `GET /api/v1/transfers/{id}`
- `POST /api/v1/transfers`
- `PUT /api/v1/transfers/{id}`
- `DELETE /api/v1/transfers/{id}`

### Shipment

- `GET /api/v1/shipments`
- `GET /api/v1/shipments/{id}`
- `POST /api/v1/shipments`
- `PUT /api/v1/shipments/{id}`
- `DELETE /api/v1/shipments/{id}`

### Boxes

- `GET /api/v1/boxes`
- `GET /api/v1/boxes/{id}`
- `POST /api/v1/boxes`
- `PUT /api/v1/boxes/{id}`
- `DELETE /api/v1/boxes/{id}`

### Barcode

- `GET /api/v1/warehouse-barcodes`
- `GET /api/v1/warehouse-barcodes/{id}`
- `POST /api/v1/warehouse-barcodes/scan`
- `PUT /api/v1/warehouse-barcodes/{id}`
- `DELETE /api/v1/warehouse-barcodes/{id}`

## Sales APIs

Sales APIs manage marketplace integrations, dealer sales, orders, shipping flow, and returns.

### Marketplace

- `GET /api/v1/marketplace-orders`
- `GET /api/v1/marketplace-orders/{id}`
- `POST /api/v1/marketplace-orders/sync`
- `PUT /api/v1/marketplace-orders/{id}`
- `DELETE /api/v1/marketplace-orders/{id}`

### Dealers

- `GET /api/v1/dealers`
- `GET /api/v1/dealers/{id}`
- `POST /api/v1/dealers`
- `PUT /api/v1/dealers/{id}`
- `DELETE /api/v1/dealers/{id}`

### Orders

- `GET /api/v1/sales-orders`
- `GET /api/v1/sales-orders/{id}`
- `POST /api/v1/sales-orders`
- `PUT /api/v1/sales-orders/{id}`
- `DELETE /api/v1/sales-orders/{id}`

### Returns

- `GET /api/v1/returns`
- `GET /api/v1/returns/{id}`
- `POST /api/v1/returns`
- `PUT /api/v1/returns/{id}`
- `DELETE /api/v1/returns/{id}`

## Finance APIs

Finance APIs manage current accounts, payments, collections, expenses, and financial references.

### Current Accounts

- `GET /api/v1/current-accounts`
- `GET /api/v1/current-accounts/{id}`
- `POST /api/v1/current-accounts`
- `PUT /api/v1/current-accounts/{id}`
- `DELETE /api/v1/current-accounts/{id}`

### Payments

- `GET /api/v1/payments`
- `GET /api/v1/payments/{id}`
- `POST /api/v1/payments`
- `PUT /api/v1/payments/{id}`
- `DELETE /api/v1/payments/{id}`

### Collections

- `GET /api/v1/collections`
- `GET /api/v1/collections/{id}`
- `POST /api/v1/collections`
- `PUT /api/v1/collections/{id}`
- `DELETE /api/v1/collections/{id}`

### Expenses

- `GET /api/v1/expenses`
- `GET /api/v1/expenses/{id}`
- `POST /api/v1/expenses`
- `PUT /api/v1/expenses/{id}`
- `DELETE /api/v1/expenses/{id}`

## Dashboard APIs

Dashboard APIs provide aggregated operational data for the web application.

### Statistics

- `GET /api/v1/dashboard/statistics`

Purpose: Returns high-level counts and operational summaries.

### Charts

- `GET /api/v1/dashboard/charts`

Purpose: Returns chart-ready datasets for sales, stock, purchasing, production, and finance.

### KPIs

- `GET /api/v1/dashboard/kpis`

Purpose: Returns key performance indicators by role and module.

### Notifications

- `GET /api/v1/dashboard/notifications`
- `PUT /api/v1/dashboard/notifications/{id}/read`

Purpose: Returns system notifications and supports marking notifications as read.

## Error Codes

### 400 Bad Request

The request is malformed or contains invalid parameters.

### 401 Unauthorized

Authentication is missing, expired, or invalid.

### 403 Forbidden

The authenticated user does not have permission for the requested action.

### 404 Not Found

The requested resource does not exist or is not accessible.

### 409 Conflict

The request conflicts with an existing business rule or unique constraint.

### 422 Unprocessable Entity

The request is syntactically correct but fails validation or business rule checks.

### 500 Internal Server Error

An unexpected server-side error occurred.

## Validation Rules

Validation must be implemented consistently across all write operations.

### FluentValidation

- Request DTOs must have dedicated validators.
- Required fields must be explicitly validated.
- Maximum lengths must be validated.
- Numeric fields must validate valid ranges.
- Date fields must validate logical order.
- Foreign key references must be validated where required.

### Business Rules

- Duplicate business keys must be blocked.
- Status transitions must follow module workflows.
- Deleted or inactive master data must not be used in new transactions.
- Quantity values must not become negative unless the workflow explicitly allows it.
- Financial totals must be calculated consistently.

## Performance

The API must support operational ERP workloads without loading unnecessary data.

### Pagination

- All list endpoints must support pagination.
- Default page size should be defined centrally.
- Maximum page size should prevent expensive responses.

### Filtering

- Filtering must be handled server-side.
- Frequently filtered fields should be indexed.
- Date range filters must use UTC-compatible comparisons.

### Sorting

- Sorting must be explicit.
- Default sorting should be stable.
- Invalid sort fields must be rejected or ignored consistently.

### Async

- Database and file operations must use asynchronous APIs.
- External integration calls must use asynchronous APIs.

### CancellationToken

- API handlers and service methods should accept `CancellationToken`.
- Long-running operations must respect cancellation.

## Logging

Logging must support diagnostics, traceability, and audit requirements.

### Serilog

- Structured logging must be used.
- Logs must include request correlation identifiers.
- Sensitive data must not be logged.

### Request Logging

- Request path
- HTTP method
- Response status code
- Duration
- User identifier when authenticated
- Correlation identifier

### Audit Logging

- User actions
- Create, update, delete operations
- Business-critical status changes
- Authentication events
- Financial events
- Inventory movements

## Security

Security rules apply to every module.

### JWT

- Access tokens must be short-lived.
- Claims must include user identifier, roles, and permissions.
- Expired tokens must be rejected.

### Refresh Token

- Refresh tokens must be securely stored.
- Refresh tokens must be revocable.
- Token reuse should be detected where possible.

### Role Authorization

- Every endpoint must declare required authorization.
- Administrative endpoints must require administrator privileges.
- Module-specific write actions must require module permissions.

### Rate Limiting (Future)

- Login endpoints should be protected against brute-force attempts.
- Public or integration endpoints should have rate limits.
- Marketplace synchronization endpoints should define safe execution windows.

## Future APIs

Future APIs will extend FIOLIN ONE without breaking existing contracts.

### AI

- AI-assisted product insights
- AI-assisted demand analysis
- AI-assisted production planning

### Forecast

- Sales forecasting
- Purchasing suggestions
- Inventory risk forecasting

### Supplier Portal

- Supplier order confirmation
- Supplier shipment notices
- Supplier invoice upload

### Dealer Portal

- Dealer order entry
- Dealer stock visibility
- Dealer invoice visibility

### Mobile Application

- Warehouse scanning
- Fabric receiving
- Stock counting
- Shipment preparation

## Status

Draft

## Last Updated

2026-07-05
