# FIOLIN ONE System Architecture Blueprint

## Vision

FIOLIN ONE is a complete ERP + PLM platform for the apparel industry. It is designed to manage the entire lifecycle from product idea until customer delivery.

The platform connects product development, master data, purchasing, fabric inventory, production, warehouse operations, sales channels, finance, and reporting into one integrated system. The long-term goal is to provide a single operational source of truth for apparel manufacturers and wholesalers.

FIOLIN ONE should support business teams from the first model idea through design, sampling, approval, purchasing, fabric reservation, cutting, sewing, finished goods warehousing, dealer sales, marketplace orders, shipment, finance, and management reporting.

This document becomes the main architectural reference for the entire FIOLIN ONE project.

---

## Core Principles

### Modular Architecture

The system is organized around business modules. Each module should have clear boundaries, business ownership, data responsibilities, APIs, UI screens, and integration points.

### Clean Architecture

Backend code follows Clean Architecture principles. Domain rules must remain independent from infrastructure, database, API, and UI concerns.

### Domain Driven Design

The model should reflect apparel business language. Terms such as Model, Product, Variant, Fabric Roll, Production Order, Cutting, Warehouse Movement, Dealer Order, and Current Account should be represented consistently.

### REST API

The backend exposes REST APIs for frontend and future integrations. APIs should use predictable routes, clear DTOs, validation, and consistent error responses.

### JWT Authentication

Authentication is based on JWT tokens. The frontend authenticates users and sends bearer tokens to protected API endpoints.

### Responsive Web UI

The frontend is a responsive web application designed for office, warehouse, and production users. Screens should work on desktop and practical tablet widths.

### PostgreSQL

PostgreSQL is the primary relational database. It stores transactional ERP data, master data, audit information, and operational history.

### Docker Deployment

The application is deployable with Docker and Docker Compose. Local development, test, and production environments should use consistent containerized services where possible.

### Audit Logging

Important business actions must be traceable. The system should record who created, updated, approved, cancelled, received, moved, consumed, or completed records.

### Role Based Authorization

Access is controlled by roles and permissions. Users should only see and perform actions appropriate to their job responsibilities.

### File Management

The platform manages business files such as design drawings, technical sheets, pattern files, sample photos, product images, invoices, and documents.

---

## Module Map

### Identity

Identity manages authentication and user access foundations.

#### User Management

Creates and maintains user accounts for office, warehouse, production, finance, and management users.

#### Role Management

Defines business roles such as Administrator, Production Manager, Designer, Warehouse Staff, Purchasing, Finance, Dealer Manager, and Reporting User.

#### Permissions

Controls specific actions such as creating products, approving models, receiving fabric, adjusting stock, viewing finance, and managing master data.

---

### Master Data

Master Data stores shared reference information used across the ERP.

#### Brands

Brands classify products and models.

#### Categories

Categories classify products, models, and reporting groups.

#### Seasons

Seasons organize collections and sales periods.

#### Collections

Collections group models and products for seasonal or commercial planning.

#### Colors

Colors standardize product variants and fabric rolls.

#### Sizes

Sizes standardize sellable variants.

#### Fabric Types

Fabric Types classify materials such as woven, knitted, denim, lining, rib, or interlock.

#### Suppliers

Suppliers provide fabrics, accessories, services, and future purchase items.

#### Warehouses

Warehouses and locations define where stock is physically stored.

---

### PLM

PLM manages product development before production.

#### Ideas

Captures model ideas and early design concepts.

#### Models

Stores model cards with model code, brand, season, category, designer, status, and development information.

#### Design Files

Stores drawings, sketches, mood references, and visual design documents.

#### Technical Sheets

Stores technical PDFs and production specifications.

#### Patterns

Stores pattern files such as Gerber, Lectra, ZIP archives, and related documents.

#### Samples

Tracks sample requests, sample production, sample review, and sample approval.

#### Revisions

Records every model revision with date, reason, changed by, and notes.

#### Approvals

Controls release from development to production-ready status.

---

### Products

Products manage commercial and sellable items.

#### Products

Products represent approved product models used for sales and production planning.

#### Variants

Variants are sellable items created from product, color, and size combinations.

#### Pricing

Pricing stores wholesale, dealer, marketplace, and future price lists.

#### Barcodes

Barcodes identify variants for warehouse, production, and sales operations.

---

### Purchasing

Purchasing controls supplier orders and purchase completion.

#### Purchase Orders

Purchase Orders contain supplier, order date, expected date, status, and item lines.

#### Goods Receipt

Goods Receipt records physical arrival and warehouse acceptance.

#### Invoices

Invoices are entered after goods receipt when supplier invoice documents arrive.

#### Suppliers

Supplier records support purchase orders, invoices, and supplier reporting.

---

### Fabric

Fabric manages material inventory for production.

#### Fabric Cards

Fabric Cards define fabric code, name, type, supplier, composition, width, GSM, unit, and active status.

#### Fabric Inventory

Fabric Inventory tracks roll-level stock quantities.

#### Fabric Movements

Fabric Movements record inbound, outbound, transfer, adjustment, and consumption events.

#### Reservations

Reservations block fabric quantities for production orders before consumption.

#### Consumption

Consumption records actual fabric usage during cutting.

---

### Production

Production manages manufacturing execution.

#### Production Orders

Production Orders define what will be produced, in which variants, and in what quantities.

#### Planning

Planning organizes production dates, material needs, and capacity.

#### Cutting

Cutting consumes fabric and prepares cut pieces for sewing.

#### Workshop Orders

Workshop Orders assign work to sewing workshops or internal production units.

#### Quality Control

Quality Control records inspection results, defects, rework, and approvals.

#### Finished Goods

Finished Goods represent completed sellable stock ready for warehouse and shipment.

---

### Warehouse

Warehouse manages physical stock operations.

#### Warehouse Stock

Tracks product, variant, fabric, and future accessory stock.

#### Locations

Represents physical storage areas, racks, shelves, bins, and staging zones.

#### Boxes

Boxes support packed goods, shipment preparation, and barcode scanning.

#### Barcode

Barcode scanning accelerates receiving, transfers, picking, shipment, and counting.

#### Transfers

Transfers move stock between warehouses or locations.

#### Shipment

Shipment prepares goods for dealers, Trendyol, or other sales channels.

---

### Sales

Sales manages outgoing demand and customer delivery.

#### Trendyol

Trendyol integration imports marketplace orders and maps them to product variants.

#### Dealer Orders

Dealer Orders support B2B wholesale customers.

#### Returns

Returns record products coming back from dealers, marketplaces, or customers.

#### Shipping

Shipping handles dispatch, carrier handoff, and shipment tracking.

---

### Finance

Finance tracks commercial accounting information.

#### Current Accounts

Current Accounts track supplier, dealer, and customer balances.

#### Payments

Payments record outgoing supplier payments and other financial transactions.

#### Collections

Collections record incoming customer or dealer payments.

#### Profit Analysis

Profit Analysis compares sales, purchases, production cost, and operational profitability.

---

### Reporting

Reporting provides operational and management visibility.

#### Dashboard

Dashboard summarizes key business indicators.

#### Sales Reports

Sales Reports analyze sales by customer, dealer, marketplace, product, variant, date, and season.

#### Inventory Reports

Inventory Reports show product, variant, fabric, roll, warehouse, and location stock.

#### Production Reports

Production Reports show production progress, consumption, efficiency, defects, and completion.

#### Financial Reports

Financial Reports show current accounts, payments, collections, and profitability.

---

## Module Relationships

The primary business flow is:

PLM

↓

Product

↓

Purchase

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

PLM creates approved models and technical definitions. Product Management turns approved models into products and sellable variants. Purchasing orders the materials needed for production. Fabric Management receives and controls material inventory. Production consumes fabric and creates finished goods. Warehouse receives, stores, transfers, and ships stock. Sales creates demand through dealers and marketplaces. Finance records commercial outcomes, payments, collections, and profitability.

Modules should communicate through clear application services and APIs. Shared master data must be referenced by identifiers instead of duplicated text fields. Transactional modules should not silently modify another module's data; changes should happen through explicit workflows and recorded movements.

---

## Technology Stack

### Backend

- ASP.NET Core 9
- Entity Framework Core
- PostgreSQL
- JWT
- Serilog
- FluentValidation
- Swagger

### Frontend

- React
- TypeScript
- Material UI
- React Router
- Axios

### Docker

- Docker
- Docker Compose

The backend provides REST APIs. The frontend consumes APIs and renders responsive ERP screens. PostgreSQL stores application data. Docker Compose coordinates local and deployable services.

---

## Folder Structure

Current repository layout:

```text
FIOLIN-ONE
├── backend
│   ├── src
│   │   ├── FiolinOne.Api
│   │   ├── FiolinOne.Application
│   │   ├── FiolinOne.Domain
│   │   └── FiolinOne.Infrastructure
│   └── tests
│       └── FiolinOne.ArchitectureTests
├── frontend
│   └── React + TypeScript + Vite + Material UI
├── docs
│   ├── api
│   ├── architecture
│   ├── business
│   ├── database
│   ├── decisions
│   ├── deployment
│   ├── images
│   ├── ui
│   └── workflows
├── docker-compose.yml
└── .env.example
```

Backend modules should place domain entities in `FiolinOne.Domain`, interfaces and business services in `FiolinOne.Application`, persistence and external infrastructure in `FiolinOne.Infrastructure`, and HTTP controllers in `FiolinOne.Api`.

---

## Database Strategy

### One PostgreSQL Database

FIOLIN ONE uses one PostgreSQL database for the ERP. Modular boundaries are enforced in code and schema organization rather than separate databases.

### Migration Strategy

Entity Framework Core migrations manage schema changes. Each business module should add migrations that are reviewed before deployment.

### Soft Delete

Soft delete should be used for business records that must remain historically traceable. Physical deletion should be limited to safe reference data or development-only scenarios.

### Audit Fields

Business entities should include audit fields such as created date, updated date, created by, updated by, and future status history where needed.

### GUID Identifiers

Entities use GUID identifiers to support distributed creation, integration readiness, and stable references across modules.

---

## Security

### Authentication

Users authenticate through the FIOLIN ONE identity process and receive JWT tokens.

### Authorization

API and UI actions must be protected by authorization rules.

### Password Hashing

Passwords must never be stored in plain text. Password hashing should use proven framework-supported algorithms.

### JWT

JWT tokens carry authenticated user identity and authorization claims.

### Role Based Access

Role based access controls what modules, pages, and actions a user can access.

---

## File Storage Strategy

FIOLIN ONE must manage many business files:

- Model drawings
- Technical sheets
- Pattern files
- Sample photos
- Product images
- Invoices
- Documents

Files should not be stored directly as large binary data in transactional tables. The recommended strategy is to store files in a managed file storage location and keep metadata in the database.

File metadata should include file name, file type, module, entity reference, uploaded by, uploaded date, version, status, and storage path. Old files should not be deleted when newer versions are uploaded. This is especially important for PLM, technical sheets, pattern files, invoices, and approval history.

In production, file storage may use a mounted volume, object storage, or cloud storage provider. The database should store references and metadata, not the file storage implementation details.

---

## Coding Standards

### Naming

Names should use clear business language. Backend classes use PascalCase. Database tables and columns use snake_case.

### Folder Organization

Files should be organized by layer and business responsibility. Shared concepts should be placed in reusable folders only when they are truly shared.

### DTO Usage

API responses and requests should use DTOs instead of exposing domain entities directly.

### Repository Pattern

Repositories handle data access and persistence details. Application services depend on repository interfaces.

### Service Layer

Application services coordinate business operations, validation, uniqueness checks, and workflow decisions.

### Dependency Injection

Services, repositories, validators, and infrastructure dependencies should be registered through dependency injection.

---

## Deployment

### Development

Development uses local source code, local environment settings, Docker Compose services, and developer tooling.

### Test

Test environments should run production-like containers and database migrations with controlled test data.

### Production

Production should run stable Docker images, secure environment variables, database backups, logging, and monitoring.

### Docker Deployment

Docker Compose is used to coordinate API, frontend, and PostgreSQL services. Future production deployment may use a container orchestrator, managed database, and external file storage.

---

## Roadmap

### Phase 1

Foundation

### Phase 2

PLM

### Phase 3

Purchasing

### Phase 4

Fabric

### Phase 5

Production

### Phase 6

Warehouse

### Phase 7

Sales

### Phase 8

Finance

The roadmap should be refined as business priorities, dependencies, and implementation risk become clearer.

---

## Future Vision

FIOLIN ONE can grow beyond operational ERP into intelligent planning and decision support.

Future capabilities may include:

- AI assisted demand forecasting
- Automatic purchasing suggestions
- Production planning optimization
- Supplier portal
- Dealer portal
- Mobile warehouse application
- Business Intelligence Dashboard

These improvements should build on accurate operational data, clean master data, and reliable transaction history.

---

## Status

Draft

## Last Updated

2026-07-04
