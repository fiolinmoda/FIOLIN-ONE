# FIOLIN ONE Database Blueprint

## Purpose

This document describes the complete PostgreSQL database architecture for FIOLIN ONE. It is the official database design reference for the project and should guide table design, naming, relationships, indexing, auditability, and future expansion.

FIOLIN ONE is an ERP + PLM platform for apparel manufacturing and wholesale operations. The database must support product development, product variants, purchasing, fabric management, production, warehouse operations, sales, finance, and reporting while preserving strong traceability and operational consistency.

This document becomes the official database reference for FIOLIN ONE.

---

## Database Standards

Database: PostgreSQL

Primary Key: UUID (GUID)

All business tables should use UUID identifiers. UUIDs allow stable references across integrations, imports, distributed operations, and future mobile/offline scenarios.

Soft Delete

Business records should generally be soft deleted instead of physically deleted. This protects historical reporting, audit trails, stock movement integrity, invoice history, and approval history.

Audit Fields

Standard audit fields:

- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

### CreatedAt

UTC timestamp when the record was created.

### CreatedBy

User identifier that created the record.

### UpdatedAt

UTC timestamp when the record was last updated.

### UpdatedBy

User identifier that last updated the record.

### DeletedAt

UTC timestamp when the record was soft deleted.

### DeletedBy

User identifier that soft deleted the record.

### RowVersion

Concurrency field used to prevent conflicting updates.

---

## Naming Convention

### Table Names

Table names should be plural and use PascalCase in conceptual documentation. Physical PostgreSQL table names may use snake_case through EF Core configuration.

Examples:

- Products
- ProductVariants
- PurchaseOrders
- FabricMovements

### Column Names

Column names should be clear, business-oriented, and consistent. Foreign key columns should end with `Id`.

Examples:

- ProductCode
- ProductName
- SupplierId
- WarehouseId
- CreatedAt

### Foreign Keys

Foreign key names should describe source and target tables.

Pattern:

- FK_{SourceTable}_{TargetTable}_{ColumnName}

Example:

- FK_ProductVariants_Products_ProductId

### Indexes

Index names should describe the table and column set.

Pattern:

- IX_{Table}_{Column}
- IX_{Table}_{Column1}_{Column2}

Example:

- IX_ProductVariants_ProductId_ColorId_SizeId

### Constraints

Constraint names should be explicit and stable.

Patterns:

- PK_{Table}
- UQ_{Table}_{Column}
- CK_{Table}_{RuleName}

Examples:

- PK_Products
- UQ_ProductVariants_Barcode
- CK_FabricInventory_CurrentQuantity_NonNegative

---

# Identity Module

## Users

Purpose

Stores application users.

Columns

- Id
- UserName
- Email
- PasswordHash
- FullName
- Phone
- IsActive
- LastLoginAt
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Users has many UserRoles
- Users has many LoginHistory records
- Users has many Sessions

Indexes

- Unique index on UserName
- Unique index on Email
- Index on IsActive

## Roles

Purpose

Stores system roles.

Columns

- Id
- Name
- Code
- Description
- IsActive
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Roles has many UserRoles
- Roles has many RolePermissions

Indexes

- Unique index on Code
- Index on IsActive

## Permissions

Purpose

Stores granular permissions for modules and actions.

Columns

- Id
- Module
- Action
- Code
- Description
- IsActive
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Permissions has many RolePermissions

Indexes

- Unique index on Code
- Composite index on Module and Action

## UserRoles

Purpose

Connects users to roles.

Columns

- Id
- UserId
- RoleId
- CreatedAt
- CreatedBy

Relationships

- UserRoles belongs to Users
- UserRoles belongs to Roles

Indexes

- Unique composite index on UserId and RoleId
- Index on RoleId

## RolePermissions

Purpose

Connects roles to permissions.

Columns

- Id
- RoleId
- PermissionId
- CreatedAt
- CreatedBy

Relationships

- RolePermissions belongs to Roles
- RolePermissions belongs to Permissions

Indexes

- Unique composite index on RoleId and PermissionId
- Index on PermissionId

## LoginHistory

Purpose

Stores login attempts and authentication audit data.

Columns

- Id
- UserId
- LoginAt
- IpAddress
- UserAgent
- IsSuccessful
- FailureReason

Relationships

- LoginHistory belongs to Users

Indexes

- Index on UserId
- Index on LoginAt
- Composite index on UserId and LoginAt

## Sessions

Purpose

Stores active and historical user sessions.

Columns

- Id
- UserId
- RefreshTokenHash
- StartedAt
- ExpiresAt
- RevokedAt
- IpAddress
- UserAgent
- IsActive

Relationships

- Sessions belongs to Users

Indexes

- Index on UserId
- Index on ExpiresAt
- Index on IsActive

---

# Master Data

Master data tables provide shared reference values across modules. Each master data table should include Id, Name, Code, IsActive, SortOrder, audit fields, and RowVersion.

## Brands

Purpose

Stores product and model brands.

Columns

- Id
- Name
- Code
- IsActive
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Brands has many Products
- Brands has many Models

Indexes

- Unique index on Code
- Index on IsActive
- Index on SortOrder

## Categories

Purpose

Stores product and model categories.

Columns

- Id
- Name
- Code
- IsActive
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Categories has many Products
- Categories has many Models

Indexes

- Unique index on Code
- Index on IsActive

## Collections

Purpose

Stores seasonal or commercial collections.

Columns

- Id
- Name
- Code
- SeasonId
- IsActive
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Collections belongs to Seasons
- Collections has many Models

Indexes

- Unique index on Code
- Index on SeasonId

## Seasons

Purpose

Stores season definitions.

Columns

- Id
- Name
- Code
- IsActive
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Seasons has many Products
- Seasons has many Models
- Seasons has many Collections

Indexes

- Unique index on Code
- Index on IsActive

## Colors

Purpose

Stores standardized colors for variants, fabrics, and models.

Columns

- Id
- Name
- Code
- HexCode
- IsActive
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Colors has many ProductVariants
- Colors has many FabricInventory records

Indexes

- Unique index on Code
- Index on IsActive

## Sizes

Purpose

Stores standardized sizes for product variants.

Columns

- Id
- Name
- Code
- SizeGroup
- IsActive
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Sizes has many ProductVariants

Indexes

- Unique index on Code
- Index on SizeGroup

## FabricTypes

Purpose

Stores fabric classification values.

Columns

- Id
- Name
- Code
- IsActive
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- FabricTypes has many Fabrics

Indexes

- Unique index on Code

## Suppliers

Purpose

Stores supplier master records.

Columns

- Id
- SupplierCode
- SupplierName
- Phone
- Email
- Address
- TaxNumber
- PaymentTerm
- IsActive
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Suppliers has many PurchaseOrders
- Suppliers has many PurchaseInvoices
- Suppliers has many Fabrics

Indexes

- Unique index on SupplierCode
- Index on SupplierName
- Index on TaxNumber

## Warehouses

Purpose

Stores warehouse definitions.

Columns

- Id
- Name
- Code
- Address
- IsActive
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Warehouses has many Locations
- Warehouses has many Inventory records

Indexes

- Unique index on Code

## Countries

Purpose

Stores country master data.

Columns

- Id
- Name
- Code
- IsActive
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Countries has many Cities
- Countries may be referenced by customers and suppliers

Indexes

- Unique index on Code

## Cities

Purpose

Stores city master data.

Columns

- Id
- CountryId
- Name
- Code
- IsActive
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Cities belongs to Countries

Indexes

- Composite unique index on CountryId and Code
- Index on CountryId

## Units

Purpose

Stores measurement units such as piece, kilogram, meter, box, and roll.

Columns

- Id
- Name
- Code
- IsActive
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Units are referenced by purchase items, fabric inventory, and stock movements

Indexes

- Unique index on Code

## Currencies

Purpose

Stores currency master data.

Columns

- Id
- Name
- Code
- Symbol
- IsActive
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Currencies are referenced by prices, invoices, payments, and collections

Indexes

- Unique index on Code

---

# Product Module

## Products

Purpose

Stores approved product models used for production, sales, and reporting.

Columns

- Id
- ProductCode
- ProductName
- BrandId
- CategoryId
- SeasonId
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Products belongs to Brands
- Products belongs to Categories
- Products belongs to Seasons
- Products has many ProductVariants
- Products has many ProductImages
- Products has many ProductPrices

Indexes

- Unique index on ProductCode
- Index on BrandId
- Index on CategoryId
- Index on SeasonId
- Index on Status

## ProductVariants

Purpose

Stores sellable product variants based on product, color, and size.

Columns

- Id
- ProductId
- ColorId
- SizeId
- Barcode
- TrendyolSku
- Stock
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- ProductVariants belongs to Products
- ProductVariants belongs to Colors
- ProductVariants belongs to Sizes
- ProductVariants has many Barcodes
- ProductVariants has many Inventory records

Indexes

- Unique composite index on ProductId, ColorId, SizeId
- Unique index on Barcode
- Unique index on TrendyolSku
- Index on ProductId

## ProductImages

Purpose

Stores product and variant image metadata.

Columns

- Id
- ProductId
- ProductVariantId
- FileName
- FilePath
- IsPrimary
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- ProductImages belongs to Products
- ProductImages optionally belongs to ProductVariants

Indexes

- Index on ProductId
- Index on ProductVariantId

## ProductPrices

Purpose

Stores product or variant price definitions.

Columns

- Id
- ProductId
- ProductVariantId
- CurrencyId
- PriceType
- Price
- ValidFrom
- ValidTo
- IsActive
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- ProductPrices belongs to Products
- ProductPrices optionally belongs to ProductVariants
- ProductPrices belongs to Currencies

Indexes

- Index on ProductId
- Index on ProductVariantId
- Composite index on PriceType and IsActive

## Barcodes

Purpose

Stores barcode records for product variants, boxes, and warehouse operations.

Columns

- Id
- BarcodeValue
- ProductVariantId
- BoxId
- BarcodeType
- IsActive
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Barcodes may belong to ProductVariants
- Barcodes may belong to Boxes

Indexes

- Unique index on BarcodeValue
- Index on ProductVariantId

---

# Product Development (PLM)

## Ideas

Purpose

Stores early product ideas before model creation.

Columns

- Id
- Title
- Description
- BrandId
- SeasonId
- DesignerId
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Ideas may become Models
- Ideas belongs to Designers

Indexes

- Index on Status
- Index on DesignerId

## Models

Purpose

Stores PLM model cards before production approval.

Columns

- Id
- ModelCode
- ModelName
- BrandId
- SeasonId
- CollectionId
- CategoryId
- DesignerId
- Status
- Description
- CreationDate
- ApprovalDate
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Models belongs to Brands, Seasons, Collections, Categories, and Designers
- Models has many ModelFiles
- Models has many ModelRevisions
- Models has many Samples
- Models has many ApprovalHistory records

Indexes

- Unique index on ModelCode
- Index on Status
- Index on DesignerId

## ModelFiles

Purpose

Stores metadata for drawings, technical sheets, pattern files, photos, and documents.

Columns

- Id
- ModelId
- RevisionId
- FileCategory
- FileName
- FilePath
- FileType
- VersionNumber
- IsActiveVersion
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- ModelFiles belongs to Models
- ModelFiles optionally belongs to ModelRevisions

Indexes

- Index on ModelId
- Index on RevisionId
- Composite index on ModelId and FileCategory

## ModelRevisions

Purpose

Stores revision history for models.

Columns

- Id
- ModelId
- RevisionNumber
- RevisionDate
- Reason
- ChangedBy
- Notes
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- ModelRevisions belongs to Models
- ModelRevisions has many ModelFiles

Indexes

- Unique composite index on ModelId and RevisionNumber

## Samples

Purpose

Stores sample tracking records.

Columns

- Id
- ModelId
- RevisionId
- SampleNumber
- SampleDate
- ResponsiblePerson
- Notes
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Samples belongs to Models
- Samples optionally belongs to ModelRevisions

Indexes

- Index on ModelId
- Index on Status

## ApprovalHistory

Purpose

Stores model approval and status transition history.

Columns

- Id
- ModelId
- FromStatus
- ToStatus
- ApprovedBy
- ApprovalDate
- Notes
- CreatedAt
- CreatedBy

Relationships

- ApprovalHistory belongs to Models

Indexes

- Index on ModelId
- Index on ApprovalDate

## Designers

Purpose

Stores designer master records.

Columns

- Id
- DesignerCode
- DesignerName
- Email
- Phone
- IsActive
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Designers has many Models
- Designers has many Ideas

Indexes

- Unique index on DesignerCode
- Index on IsActive

---

# Purchasing

## PurchaseOrders

Purpose

Stores purchase order headers.

Columns

- Id
- PurchaseNumber
- SupplierId
- OrderDate
- ExpectedDate
- Status
- Notes
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- PurchaseOrders belongs to Suppliers
- PurchaseOrders has many PurchaseOrderItems
- PurchaseOrders has many GoodsReceipts
- PurchaseOrders has many PurchaseInvoices

Indexes

- Unique index on PurchaseNumber
- Index on SupplierId
- Index on Status

## PurchaseOrderItems

Purpose

Stores purchase order lines.

Columns

- Id
- PurchaseOrderId
- FabricId
- ColorId
- Quantity
- UnitId
- UnitPrice
- ReceivedQuantity
- RemainingQuantity
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- PurchaseOrderItems belongs to PurchaseOrders
- PurchaseOrderItems belongs to Fabrics
- PurchaseOrderItems belongs to Colors
- PurchaseOrderItems belongs to Units

Indexes

- Index on PurchaseOrderId
- Index on FabricId
- Index on Status

## GoodsReceipts

Purpose

Stores goods receipt headers.

Columns

- Id
- ReceiptNumber
- SupplierId
- PurchaseOrderId
- WarehouseId
- ReceiptDate
- Status
- Notes
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- GoodsReceipts belongs to Suppliers
- GoodsReceipts belongs to PurchaseOrders
- GoodsReceipts belongs to Warehouses
- GoodsReceipts has many GoodsReceiptItems

Indexes

- Unique index on ReceiptNumber
- Index on PurchaseOrderId
- Index on ReceiptDate

## GoodsReceiptItems

Purpose

Stores received quantities by purchase item.

Columns

- Id
- GoodsReceiptId
- PurchaseOrderItemId
- ReceivedQuantity
- AcceptedQuantity
- DifferenceQuantity
- AcceptanceStatus
- Notes
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- GoodsReceiptItems belongs to GoodsReceipts
- GoodsReceiptItems belongs to PurchaseOrderItems

Indexes

- Index on GoodsReceiptId
- Index on PurchaseOrderItemId

## PurchaseInvoices

Purpose

Stores purchase invoice headers.

Columns

- Id
- InvoiceNumber
- InvoiceDate
- SupplierId
- PurchaseOrderId
- InvoiceAmount
- CurrencyId
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- PurchaseInvoices belongs to Suppliers
- PurchaseInvoices belongs to PurchaseOrders
- PurchaseInvoices belongs to Currencies
- PurchaseInvoices has many PurchaseInvoiceItems

Indexes

- Unique composite index on SupplierId and InvoiceNumber
- Index on PurchaseOrderId
- Index on Status

## PurchaseInvoiceItems

Purpose

Stores purchase invoice line details.

Columns

- Id
- PurchaseInvoiceId
- PurchaseOrderItemId
- Quantity
- UnitPrice
- LineTotal
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- PurchaseInvoiceItems belongs to PurchaseInvoices
- PurchaseInvoiceItems may reference PurchaseOrderItems

Indexes

- Index on PurchaseInvoiceId

---

# Fabric Management

## Fabrics

Purpose

Stores fabric card information.

Columns

- Id
- FabricCode
- FabricName
- FabricTypeId
- SupplierId
- Composition
- Width
- Gsm
- UnitId
- IsActive
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Fabrics belongs to FabricTypes
- Fabrics belongs to Suppliers
- Fabrics belongs to Units
- Fabrics has many FabricInventory records

Indexes

- Unique index on FabricCode
- Index on FabricTypeId
- Index on SupplierId

## FabricInventory

Purpose

Stores roll-level fabric stock.

Columns

- Id
- FabricId
- ColorId
- SupplierId
- WarehouseId
- LocationId
- RollNumber
- LotNumber
- OriginalQuantity
- CurrentQuantity
- UnitId
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- FabricInventory belongs to Fabrics
- FabricInventory belongs to Colors
- FabricInventory belongs to Warehouses
- FabricInventory belongs to Locations
- FabricInventory has many FabricMovements

Indexes

- Unique index on RollNumber
- Index on FabricId
- Index on WarehouseId and LocationId
- Index on Status

## FabricMovements

Purpose

Stores every fabric stock movement.

Columns

- Id
- FabricInventoryId
- MovementType
- SourceWarehouseId
- SourceLocationId
- DestinationWarehouseId
- DestinationLocationId
- Quantity
- UnitId
- RelatedDocumentType
- RelatedDocumentId
- Reason
- MovementDate
- CreatedAt
- CreatedBy

Relationships

- FabricMovements belongs to FabricInventory
- FabricMovements references Warehouses and Locations

Indexes

- Index on FabricInventoryId
- Index on MovementDate
- Index on MovementType

## FabricReservations

Purpose

Stores fabric reservations for production.

Columns

- Id
- ProductionOrderId
- FabricInventoryId
- ReservedQuantity
- ConsumedQuantity
- ReleasedQuantity
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- FabricReservations belongs to ProductionOrders
- FabricReservations belongs to FabricInventory

Indexes

- Index on ProductionOrderId
- Index on FabricInventoryId
- Index on Status

## FabricConsumption

Purpose

Stores fabric consumption during cutting.

Columns

- Id
- ProductionOrderId
- CuttingPlanId
- FabricInventoryId
- ConsumedQuantity
- WasteQuantity
- RemainingQuantity
- ConsumptionDate
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- FabricConsumption belongs to ProductionOrders
- FabricConsumption belongs to CuttingPlans
- FabricConsumption belongs to FabricInventory

Indexes

- Index on ProductionOrderId
- Index on FabricInventoryId
- Index on ConsumptionDate

---

# Production

## ProductionOrders

Purpose

Stores production order headers.

Columns

- Id
- ProductionNumber
- ProductId
- PlannedStartDate
- PlannedEndDate
- Status
- Notes
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- ProductionOrders belongs to Products
- ProductionOrders has many ProductionOrderItems
- ProductionOrders has many CuttingPlans
- ProductionOrders has many WorkshopOrders

Indexes

- Unique index on ProductionNumber
- Index on ProductId
- Index on Status

## ProductionOrderItems

Purpose

Stores production quantities by variant.

Columns

- Id
- ProductionOrderId
- ProductVariantId
- PlannedQuantity
- ProducedQuantity
- RemainingQuantity
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- ProductionOrderItems belongs to ProductionOrders
- ProductionOrderItems belongs to ProductVariants

Indexes

- Index on ProductionOrderId
- Index on ProductVariantId

## CuttingPlans

Purpose

Stores cutting plan headers.

Columns

- Id
- ProductionOrderId
- CuttingNumber
- PlannedDate
- Status
- Notes
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- CuttingPlans belongs to ProductionOrders
- CuttingPlans has many CuttingPlanItems

Indexes

- Unique index on CuttingNumber
- Index on ProductionOrderId

## CuttingPlanItems

Purpose

Stores cutting quantities by variant.

Columns

- Id
- CuttingPlanId
- ProductVariantId
- PlannedQuantity
- CutQuantity
- DefectQuantity
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- CuttingPlanItems belongs to CuttingPlans
- CuttingPlanItems belongs to ProductVariants

Indexes

- Index on CuttingPlanId
- Index on ProductVariantId

## WorkshopOrders

Purpose

Stores sewing workshop orders.

Columns

- Id
- ProductionOrderId
- WorkshopName
- StartDate
- EndDate
- Status
- Notes
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- WorkshopOrders belongs to ProductionOrders
- WorkshopOrders has many WorkshopOperations

Indexes

- Index on ProductionOrderId
- Index on Status

## WorkshopOperations

Purpose

Stores workshop operation tracking.

Columns

- Id
- WorkshopOrderId
- OperationName
- PlannedQuantity
- CompletedQuantity
- DefectQuantity
- OperationDate
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- WorkshopOperations belongs to WorkshopOrders

Indexes

- Index on WorkshopOrderId
- Index on OperationDate

## QualityControl

Purpose

Stores quality control inspections.

Columns

- Id
- ProductionOrderId
- ProductVariantId
- InspectionDate
- CheckedQuantity
- PassedQuantity
- FailedQuantity
- DefectReason
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- QualityControl belongs to ProductionOrders
- QualityControl belongs to ProductVariants

Indexes

- Index on ProductionOrderId
- Index on ProductVariantId
- Index on InspectionDate

## FinishedGoods

Purpose

Stores finished goods output before or during warehouse receipt.

Columns

- Id
- ProductionOrderId
- ProductVariantId
- Quantity
- WarehouseId
- LocationId
- Status
- CompletionDate
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- FinishedGoods belongs to ProductionOrders
- FinishedGoods belongs to ProductVariants
- FinishedGoods references Warehouses and Locations

Indexes

- Index on ProductionOrderId
- Index on ProductVariantId
- Index on Status

---

# Warehouse

## Warehouses

Purpose

Stores warehouse definitions.

Columns

- Id
- Code
- Name
- Address
- IsActive
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Warehouses has many Locations
- Warehouses has many Inventory records

Indexes

- Unique index on Code

## Locations

Purpose

Stores physical warehouse locations.

Columns

- Id
- WarehouseId
- Code
- Name
- LocationType
- IsActive
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Locations belongs to Warehouses
- Locations has many Inventory records

Indexes

- Unique composite index on WarehouseId and Code

## Inventory

Purpose

Stores product variant stock by warehouse and location.

Columns

- Id
- ProductVariantId
- WarehouseId
- LocationId
- Quantity
- ReservedQuantity
- AvailableQuantity
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Inventory belongs to ProductVariants
- Inventory belongs to Warehouses
- Inventory belongs to Locations

Indexes

- Unique composite index on ProductVariantId, WarehouseId, LocationId
- Index on WarehouseId

## InventoryMovements

Purpose

Stores product inventory movements.

Columns

- Id
- ProductVariantId
- MovementType
- SourceWarehouseId
- SourceLocationId
- DestinationWarehouseId
- DestinationLocationId
- Quantity
- RelatedDocumentType
- RelatedDocumentId
- MovementDate
- Reason
- CreatedAt
- CreatedBy

Relationships

- InventoryMovements belongs to ProductVariants
- InventoryMovements references Warehouses and Locations

Indexes

- Index on ProductVariantId
- Index on MovementDate
- Index on MovementType

## Boxes

Purpose

Stores packed box information.

Columns

- Id
- BoxNumber
- WarehouseId
- LocationId
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Boxes belongs to Warehouses
- Boxes belongs to Locations
- Boxes has many Barcodes

Indexes

- Unique index on BoxNumber
- Index on Status

## Barcodes

Purpose

Stores warehouse barcode records.

Columns

- Id
- BarcodeValue
- ProductVariantId
- BoxId
- BarcodeType
- IsActive
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Barcodes may belong to ProductVariants
- Barcodes may belong to Boxes

Indexes

- Unique index on BarcodeValue

## Transfers

Purpose

Stores warehouse transfer headers.

Columns

- Id
- TransferNumber
- SourceWarehouseId
- DestinationWarehouseId
- Status
- TransferDate
- Notes
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Transfers references source and destination Warehouses

Indexes

- Unique index on TransferNumber
- Index on Status

## Shipments

Purpose

Stores shipment headers.

Columns

- Id
- ShipmentNumber
- CustomerId
- DealerId
- SalesOrderId
- WarehouseId
- ShipmentDate
- Status
- Carrier
- TrackingNumber
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Shipments belongs to SalesOrders
- Shipments has many ShipmentItems
- Shipments belongs to Warehouses

Indexes

- Unique index on ShipmentNumber
- Index on SalesOrderId
- Index on Status

## ShipmentItems

Purpose

Stores shipment lines.

Columns

- Id
- ShipmentId
- ProductVariantId
- Quantity
- BoxId
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- ShipmentItems belongs to Shipments
- ShipmentItems belongs to ProductVariants
- ShipmentItems may belong to Boxes

Indexes

- Index on ShipmentId
- Index on ProductVariantId

---

# Sales

## Customers

Purpose

Stores customer master records.

Columns

- Id
- CustomerCode
- CustomerName
- Phone
- Email
- Address
- TaxNumber
- IsActive
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Customers has many SalesOrders

Indexes

- Unique index on CustomerCode
- Index on CustomerName

## Dealers

Purpose

Stores dealer records for B2B sales.

Columns

- Id
- DealerCode
- DealerName
- CustomerId
- Phone
- Email
- Address
- IsActive
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Dealers may belong to Customers
- Dealers has many SalesOrders

Indexes

- Unique index on DealerCode

## SalesOrders

Purpose

Stores sales order headers.

Columns

- Id
- SalesOrderNumber
- CustomerId
- DealerId
- OrderDate
- Status
- TotalAmount
- CurrencyId
- Notes
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- SalesOrders belongs to Customers
- SalesOrders may belong to Dealers
- SalesOrders has many SalesOrderItems
- SalesOrders has many Shipments

Indexes

- Unique index on SalesOrderNumber
- Index on CustomerId
- Index on DealerId
- Index on Status

## SalesOrderItems

Purpose

Stores sales order lines.

Columns

- Id
- SalesOrderId
- ProductVariantId
- Quantity
- UnitPrice
- LineTotal
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- SalesOrderItems belongs to SalesOrders
- SalesOrderItems belongs to ProductVariants

Indexes

- Index on SalesOrderId
- Index on ProductVariantId

## MarketplaceOrders

Purpose

Stores marketplace order headers such as Trendyol orders.

Columns

- Id
- Marketplace
- MarketplaceOrderNumber
- OrderDate
- CustomerName
- Status
- TotalAmount
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- MarketplaceOrders has many MarketplaceOrderItems

Indexes

- Unique composite index on Marketplace and MarketplaceOrderNumber
- Index on Status

## MarketplaceOrderItems

Purpose

Stores marketplace order lines.

Columns

- Id
- MarketplaceOrderId
- ProductVariantId
- MarketplaceSku
- Quantity
- UnitPrice
- LineTotal
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- MarketplaceOrderItems belongs to MarketplaceOrders
- MarketplaceOrderItems belongs to ProductVariants

Indexes

- Index on MarketplaceOrderId
- Index on ProductVariantId
- Index on MarketplaceSku

## Returns

Purpose

Stores sales return records.

Columns

- Id
- ReturnNumber
- SalesOrderId
- MarketplaceOrderId
- CustomerId
- ReturnDate
- Status
- Reason
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Returns may belong to SalesOrders
- Returns may belong to MarketplaceOrders
- Returns belongs to Customers

Indexes

- Unique index on ReturnNumber
- Index on Status
- Index on ReturnDate

---

# Finance

## CurrentAccounts

Purpose

Stores financial account records for customers, dealers, and suppliers.

Columns

- Id
- AccountCode
- AccountName
- AccountType
- RelatedEntityId
- CurrencyId
- Balance
- IsActive
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- CurrentAccounts belongs to Currencies
- CurrentAccounts may reference Customers, Dealers, or Suppliers

Indexes

- Unique index on AccountCode
- Index on AccountType

## Invoices

Purpose

Stores sales and finance invoice headers.

Columns

- Id
- InvoiceNumber
- InvoiceType
- CurrentAccountId
- InvoiceDate
- DueDate
- TotalAmount
- CurrencyId
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Invoices belongs to CurrentAccounts
- Invoices belongs to Currencies

Indexes

- Unique index on InvoiceNumber
- Index on CurrentAccountId
- Index on Status

## Payments

Purpose

Stores outgoing payment records.

Columns

- Id
- PaymentNumber
- CurrentAccountId
- PaymentDate
- Amount
- CurrencyId
- PaymentMethod
- Status
- Notes
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Payments belongs to CurrentAccounts
- Payments belongs to Currencies

Indexes

- Unique index on PaymentNumber
- Index on CurrentAccountId
- Index on PaymentDate

## Collections

Purpose

Stores incoming collection records.

Columns

- Id
- CollectionNumber
- CurrentAccountId
- CollectionDate
- Amount
- CurrencyId
- CollectionMethod
- Status
- Notes
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Collections belongs to CurrentAccounts
- Collections belongs to Currencies

Indexes

- Unique index on CollectionNumber
- Index on CurrentAccountId
- Index on CollectionDate

## ExpenseCategories

Purpose

Stores expense category master records.

Columns

- Id
- Name
- Code
- IsActive
- SortOrder
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- ExpenseCategories has many Expenses

Indexes

- Unique index on Code

## Expenses

Purpose

Stores operational expense records.

Columns

- Id
- ExpenseCategoryId
- ExpenseDate
- Amount
- CurrencyId
- Description
- Status
- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- RowVersion

Relationships

- Expenses belongs to ExpenseCategories
- Expenses belongs to Currencies

Indexes

- Index on ExpenseCategoryId
- Index on ExpenseDate

---

# Reporting

No report tables.

Reports must be generated from transactional data.

Reporting should use optimized queries, database views, materialized views, or read models only when performance requires them. These read structures should not become the source of truth.

---

## Relationships

The primary module relationship chain is:

PLM

↓

Products

↓

Purchasing

↓

Fabric

↓

Production

↓

Warehouse

↓

Sales

↓

Finance

PLM creates approved models. Products convert approved models into sellable product variants. Purchasing orders fabrics and future accessories. Fabric Management receives, reserves, and consumes material. Production transforms material into finished goods. Warehouse stores and ships stock. Sales creates outgoing demand. Finance records invoices, payments, collections, expenses, and profitability.

Shared master data connects these modules through stable foreign keys.

---

## Enumerations

### Status

- Active
- Passive
- Draft
- Archived
- Cancelled

### Order Status

- Draft
- Approved
- Partially Received
- Fully Received
- Waiting Invoice
- Completed
- Cancelled

### Approval Status

- Draft
- Waiting Approval
- Approved
- Rejected
- Revision Required

### Payment Status

- Draft
- Pending
- Partially Paid
- Paid
- Cancelled

### Warehouse Status

- Available
- Reserved
- In Transfer
- Shipped
- Damaged
- Lost

### Production Status

- Draft
- Planned
- In Cutting
- In Sewing
- In Quality Control
- Completed
- Cancelled

### Shipment Status

- Draft
- Picking
- Packed
- Shipped
- Delivered
- Returned
- Cancelled

---

## Index Strategy

### Search Indexes

Create indexes for frequently searched business identifiers and names:

- ProductCode
- ProductName
- ModelCode
- PurchaseNumber
- SupplierName
- BarcodeValue
- SalesOrderNumber
- InvoiceNumber

### Unique Indexes

Use unique indexes for business keys:

- ProductCode
- ProductVariant Barcode
- ProductVariant TrendyolSku
- ModelCode
- SupplierCode
- PurchaseNumber
- SalesOrderNumber
- InvoiceNumber

### Composite Indexes

Use composite indexes for common lookup patterns:

- ProductId, ColorId, SizeId
- ProductVariantId, WarehouseId, LocationId
- PurchaseOrderId, FabricId
- SalesOrderId, ProductVariantId
- WarehouseId, LocationCode
- Marketplace, MarketplaceOrderNumber

---

## Performance Strategy

### Pagination

All list endpoints must support pagination before production-scale usage.

### Filtering

APIs should support filtering by status, date range, master data references, warehouse, supplier, customer, and product.

### Sorting

List endpoints should provide predictable default sorting and optional user sorting.

### Read Optimization

Heavy reports should be optimized through indexed queries, projections, views, materialized views, or separate read models when needed.

Transactional tables remain the source of truth.

---

## Backup Strategy

### Daily Backup

Daily backups should protect recent operational changes.

### Weekly Backup

Weekly backups should be retained longer than daily backups and used for broader recovery windows.

### Monthly Backup

Monthly backups should be retained for long-term audit and disaster recovery needs.

Backup verification is required. A backup is not reliable unless it can be restored and validated.

---

## Future Database Expansion

### Forecasting

Future forecasting tables may support demand planning, season analysis, and purchasing suggestions.

### AI

AI features may require prompt history, recommendation results, model metadata, and user feedback records.

### Machine Learning

Machine learning may require historical feature stores, training snapshots, and prediction result tables.

### Supplier Portal

Supplier portal expansion may require supplier users, supplier confirmations, document uploads, and portal audit history.

### Dealer Portal

Dealer portal expansion may require dealer users, online order carts, dealer-specific prices, and availability snapshots.

### Mobile App

Mobile warehouse and production applications may require device registration, offline sync tracking, scan logs, and conflict resolution records.

---

## Status

Draft

## Last Updated

2026-07-04
