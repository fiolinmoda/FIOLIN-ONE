# FIOLIN ONE

FIOLIN ONE is the ERP platform foundation for a clothing manufacturer and wholesaler.

This repository currently contains only the project architecture. Business modules such as Product Cards, Fabric Management, Production Orders, Cutting, Sewing, Warehouse, Barcode System, Dealer B2B, Trendyol Orders, Finance, and Reports are intentionally not implemented yet.

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
