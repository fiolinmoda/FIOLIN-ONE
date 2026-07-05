# FIOLIN ONE

FIOLIN ONE is the ERP platform foundation for a clothing manufacturer and wholesaler.

This repository contains the project architecture, Product Management, Master Data, Purchasing v1.0, Fabric Management v1.0, and Production v1.0 foundations. Future modules such as Sewing, Warehouse, Barcode System, Dealer B2B, Trendyol Orders, Finance, and Reports will continue to build on this foundation.

## Architecture

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
├── docker-compose.yml
└── .env.example
```

## Backend Foundation

- ASP.NET Core 9 Web API
- Clean Architecture project boundaries
- Entity Framework Core with PostgreSQL provider
- JWT bearer authentication wiring
- Swagger/OpenAPI with bearer token support
- Health check endpoint at `/health`
- Serilog request logging
- FluentValidation registration
- Architecture test project for dependency direction checks

## Product Management

- Product entity and EF Core configuration
- Repository and application service
- CRUD API under `/api/products`
- Request validation
- Swagger response metadata and XML documentation comments
- React Product List and Product Detail pages
- Material UI DataGrid with search, add, edit, and delete actions

## Product Variants

- ProductVariant entity linked to master Color and Size records
- Unique database rules for Product + Color + Size, Barcode, and Trendyol SKU
- Nested CRUD API under `/api/products/{productId}/variants`
- EF Core migration for product and variant tables
- Product Detail Variants tab with Material UI DataGrid and add/edit dialogs

## Master Data

- Brand, Category, Season, Color, Size, and FabricType entities
- Generic CRUD API under `/api/master-data/{type}`
- Normalized product Brand, Category, and Season references
- Normalized variant Color and Size references
- Master Data navigation section with DataGrid pages for each list

## Purchasing v1.0

- Supplier, Purchase Order, Purchase Order Item, Goods Receipt, and Purchase Invoice foundations
- Soft delete and audit-ready database fields for purchasing records
- Secured CRUD APIs under `/api/suppliers`, `/api/purchase-orders`, `/api/goods-receipts`, and `/api/purchase-invoices`
- Pagination, filtering, sorting, FluentValidation, Swagger metadata, and XML controller comments
- Purchasing navigation with Purchase Order List, Purchase Order Detail, Supplier Management, Goods Receipt, and Purchase Invoice screens

## Fabric Management v1.0

- Weight-based Fabric cards linked to suppliers and colors
- Current stock, reserved stock, available stock, minimum stock, and automatic `OUT OF STOCK` status
- Fabric purchase arrival, production consumption, manual adjustment, inventory count, return, and reservation workflows
- Secured APIs under `/api/fabrics`, `/api/fabric-movements`, and `/api/fabric-reservations`
- Fabric List, Fabric Detail, Fabric Stock, Stock Movements, and Reservation List screens

## Production v1.0

- Production orders with product variant distribution and barcode-ready item fields
- Cutting records integrated with fabric consumption and negative-stock protection
- Workshop shipment and return records
- Ironing and packaging status transition
- Finished product warehouse entry that completes production
- Production timeline for created, cutting, shipment, return, and warehouse events
- Production dashboard cards and responsive Material UI screens

## Documentation

Project documentation starts at [docs/README.md](docs/README.md).

## Frontend Foundation

- React
- TypeScript
- Vite
- Material UI
- Responsive application shell for future ERP modules

## Getting Started

1. Copy `.env.example` to `.env`.
2. Update secrets and database credentials in `.env`.
3. Start the stack:

```bash
docker compose up --build
```

Default local endpoints:

- Frontend: `http://localhost:5173`
- API: `http://localhost:5000`
- Swagger: `http://localhost:5000/swagger`
- Health: `http://localhost:5000/health`

## Local Development

Backend:

```bash
cd backend
dotnet restore
dotnet build
dotnet test
```

Frontend:

```bash
cd frontend
npm install
npm run dev
```

## Configuration

The API reads PostgreSQL, JWT, CORS, and Serilog settings from `appsettings.json`, environment variables, and Docker Compose overrides. Keep production secrets outside source control.
