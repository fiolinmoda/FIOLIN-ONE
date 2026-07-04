# FIOLIN ONE

FIOLIN ONE is the ERP platform foundation for a clothing manufacturer and wholesaler.

This repository contains the project architecture and the first Product Management module. Future modules such as Fabric Management, Production Orders, Cutting, Sewing, Warehouse, Barcode System, Dealer B2B, Trendyol Orders, Finance, and Reports are intentionally not implemented yet.

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

- ProductColor, ProductSize, and ProductVariant entities
- Unique database rules for Product + Color + Size, Barcode, and Trendyol SKU
- Nested CRUD API under `/api/products/{productId}/variants`
- EF Core migration for product and variant tables
- Product Detail Variants tab with Material UI DataGrid and add/edit dialogs

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
