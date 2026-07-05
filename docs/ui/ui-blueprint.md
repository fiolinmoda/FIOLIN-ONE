# FIOLIN ONE UI / UX Blueprint

This document becomes the official user interface and user experience standard for the FIOLIN ONE project.

## Purpose

The UI / UX Blueprint defines how FIOLIN ONE should look, feel, and behave across all ERP and PLM modules. It provides a shared reference for page structure, navigation, tables, forms, dashboards, accessibility, responsiveness, and role-based user experience.

The goal is to create a professional, fast, and easy-to-learn system for a clothing manufacturer and wholesaler.

## Design Philosophy

The system must be:

- Extremely simple
- Fast
- Minimal clicks
- Easy to learn
- Professional
- Modern
- ERP focused
- Keyboard friendly
- Tablet friendly

### Goal

A new employee should be able to use the system after only one day of training.

The interface should support repeated daily work without visual noise. Important actions must be easy to find, frequent tasks must require as few steps as possible, and screens must remain clear even when they contain large amounts of operational data.

## Layout

FIOLIN ONE uses a stable application layout across all modules.

### Top Navigation

The top navigation contains global actions and system-level controls.

Expected content:

- Application name
- Global search
- Notifications
- User menu
- Theme switch
- Language selector when required

### Left Sidebar

The left sidebar is the primary module navigation area.

Expected behavior:

- Collapsible
- Role-aware
- Icon and label based
- Active module highlighting
- Support for grouped menu sections

### Content Area

The content area contains the active page.

Expected behavior:

- Page title
- Primary actions
- Filters
- Data grids
- Forms
- Detail tabs
- Contextual alerts

### Notification Area

The notification area displays operational warnings and user messages.

Examples:

- Low stock alerts
- Delayed workshop alerts
- Pending approval alerts
- Validation feedback
- Save confirmations

### Status Bar

The status bar provides lightweight system context.

Examples:

- Environment name
- Current user role
- Last synchronization time
- Online/offline status for future mobile support

## Left Menu

The left menu must include the following sections as the system grows:

- Dashboard
- Product Development (PLM)
- Products
- Master Data
- Purchasing
- Fabric
- Production
- Cutting
- Workshop
- Warehouse
- Sales
- Marketplace
- Dealers
- Finance
- Reports
- Settings
- Administration

Menus must automatically hide unauthorized pages based on user role and permissions.

## Dashboard

The dashboard is the operational starting point of FIOLIN ONE.

Dashboard should contain cards:

- Today's Production
- Today's Shipments
- Waiting Purchase Orders
- Pending Approvals
- Fabric Critical Stock
- Workshop Delays
- Marketplace Orders
- Dealer Orders
- Revenue
- Expenses
- Charts
- Recent Activities
- Notifications

### Dashboard Behavior

- Cards must be role-aware.
- KPIs must link to their source screens.
- Critical warnings must be visually distinct.
- Users must be able to scan the dashboard quickly.
- Dashboard data should refresh without forcing full page reloads.

## Global Search

Global Search must search everywhere across permitted data.

Searchable areas:

- Products
- Models
- Suppliers
- Orders
- Fabrics
- Invoices
- Users

### Search Behavior

- Results must be grouped by module.
- Unauthorized results must not be shown.
- Results must open the related detail page directly.
- Search must support partial matching.
- Search must remain fast with large datasets.

## DataGrid Standard

Every table must support:

- Search
- Quick Filter
- Advanced Filter
- Sorting
- Multi Sorting
- Column Resize
- Column Reorder
- Column Hide
- Column Freeze
- Export Excel
- Export PDF
- Print
- Pagination
- Row Selection
- Bulk Actions

### DataGrid Behavior

- Default columns must match the daily workflow.
- Important identifiers must stay visible.
- Numeric values must be right aligned.
- Dates must use a consistent display format.
- Status fields must use clear labels and colors.
- Empty states must explain that no records exist.
- Loading states must be clear but lightweight.
- Dangerous bulk actions must require confirmation.

## Form Standard

All forms must follow a consistent structure.

Required behavior:

- Tabbed forms
- Auto Save Warning
- Required field indicator
- Validation messages
- Keyboard navigation
- Responsive layout

### Form Rules

- Primary action must be easy to identify.
- Save and Cancel actions must be consistently placed.
- Required fields must be visually marked.
- Validation messages must appear close to the field.
- Long forms must be split into logical tabs.
- Read-only fields must be visually distinct.
- Unsaved changes must trigger a warning before leaving the page.

## Product Screen

The Product screen manages product models and their sellable variants.

### Tabs

- General
- Variants
- Prices
- Images
- Files
- Production
- Warehouse
- Sales
- History

### UX Rules

- Product code and product name must remain visible on the detail page.
- Variants must be easy to add and edit.
- Images must be visible without leaving the product detail.
- History must show important changes and user activity.

## Purchasing Screen

The Purchasing screen manages supplier purchasing from order to invoice.

### Main Areas

- Purchase List
- Purchase Detail
- Supplier
- Items
- Receipt
- Invoice
- History

### UX Rules

- Purchase status must be visible at all times.
- Received and remaining quantities must be easy to compare.
- Invoice status must be clear.
- Users must be able to identify pending supplier actions quickly.

## Fabric Screen

The Fabric screen manages fabric cards, roll-level inventory, reservations, and consumption.

### Main Areas

- Fabric Card
- Inventory
- Movements
- Consumption
- Reservations
- Purchase History

### UX Rules

- Current quantity must be visible immediately.
- Unit must always be shown with quantity.
- Roll details must be accessible from inventory.
- Reservation and consumption records must link to production.

## Production Screen

The Production screen manages the full production lifecycle.

### Main Areas

- Production Orders
- Planning
- Cutting
- Workshop
- Quality
- Finished Goods

### UX Rules

- Production status must be visible on lists and details.
- Delays must be highlighted.
- Cutting, workshop, and quality data must be connected.
- Users must be able to move from order to operation details quickly.

## Warehouse Screen

The Warehouse screen manages stock, locations, transfers, boxes, and shipment operations.

### Main Areas

- Inventory
- Locations
- Transfers
- Barcode
- Boxes
- Shipment

### UX Rules

- Barcode workflows must be fast and keyboard friendly.
- Location changes must be recorded clearly.
- Shipment screens must support warehouse staff on tablets.
- Stock movements must show source, target, quantity, date, and user.

## Sales Screen

The Sales screen manages marketplace and dealer sales.

### Main Areas

- Marketplace Orders
- Dealer Orders
- Returns
- Invoices

### UX Rules

- Order status must be clear.
- Marketplace and dealer orders must be easy to distinguish.
- Returns must show reason and related order.
- Sales users must be able to move quickly from order to shipment.

## Finance Screen

The Finance screen manages financial records and profitability.

### Main Areas

- Current Accounts
- Payments
- Collections
- Expenses
- Profit Analysis

### UX Rules

- Monetary values must display currency.
- Debt and credit positions must be visually clear.
- Payment status must be visible.
- Finance screens must prioritize accuracy and auditability.

## Reports

Reports provide operational and management visibility.

### Report Types

- Dashboard
- Charts
- Tables
- Export
- Print

### Report Rules

- Reports must support filtering.
- Reports must support date ranges.
- Reports must show generation time.
- Exported reports must match on-screen data.
- Printed reports must be clean and readable.

## Notifications

Notifications must alert users to operational issues requiring attention.

### Notification Types

- Low Stock
- Delayed Workshop
- Purchase Waiting
- Invoice Waiting
- Production Delay
- Shipment Delay

### Notification Rules

- Notifications must be role-aware.
- Critical notifications must be visually distinct.
- Users must be able to open the related record.
- Read and unread states must be clear.

## Keyboard Shortcuts

FIOLIN ONE must support common keyboard shortcuts.

- `Ctrl + S` = Save
- `Ctrl + F` = Search
- `Ctrl + N` = New Record
- `Ctrl + P` = Print
- `Ctrl + E` = Export
- `F2` = Edit
- `Delete` = Delete
- `ESC` = Cancel

### Shortcut Rules

- Shortcuts must not conflict with browser behavior where avoidable.
- Dangerous actions must still require confirmation.
- Shortcut hints may be shown in tooltips or menus.

## Color Standards

Color usage must be consistent across the system.

### Primary Color

Dark Blue

Purpose:

- Main navigation
- Primary buttons
- Active states
- Brand identity

### Success

Green

Purpose:

- Completed status
- Successful operations
- Positive stock or financial state

### Warning

Orange

Purpose:

- Pending actions
- Delays
- Attention required
- Medium severity alerts

### Danger

Red

Purpose:

- Errors
- Delete actions
- Critical stock
- Failed operations

### Information

Blue

Purpose:

- Informational messages
- Neutral status
- System notices

## Typography

Typography must be:

- Professional
- Readable
- High Contrast

### Typography Rules

- Body text must remain readable during long daily use.
- Tables must use compact but legible text.
- Headings must create clear page hierarchy.
- Labels must be direct and business-friendly.
- Avoid decorative typography.

## Icons

FIOLIN ONE uses Material Icons.

### Icon Rules

- Use consistent icon usage.
- Icons must support meaning, not decoration.
- Destructive actions must use recognizable warning icons.
- Repeated actions must use the same icon across modules.
- Icon-only buttons must provide tooltips.

## Responsive Rules

The UI must support the main working environments of the business.

### Desktop

Primary working mode for office users.

Expected use:

- Administration
- Finance
- Purchasing
- Product management
- Reporting

### Laptop

Fully supported.

Expected use:

- Daily operations
- Management review
- Remote work

### Tablet

Supported for operational workflows.

Expected use:

- Warehouse
- Production floor
- Fabric receiving
- Shipment preparation

### Mobile

Limited support.

Expected use:

- View notifications
- Quick approvals
- Basic lookups
- Future mobile application workflows

## Dark Mode

Dark Mode is supported.

### Dark Mode Rules

- Contrast must remain accessible.
- Status colors must remain recognizable.
- Data grids must remain readable.
- Charts must use dark-mode-compatible palettes.
- Dark mode must not change business meaning of colors.

## Accessibility

The UI must support accessibility requirements from the beginning.

Required support:

- Keyboard navigation
- Focus indicators
- ARIA compatibility
- High contrast mode

### Accessibility Rules

- Interactive elements must be reachable by keyboard.
- Focus state must be visible.
- Forms must have accessible labels.
- Error messages must be associated with fields.
- Color must not be the only indicator of status.

## User Roles UI

Menus and actions must adapt to user roles.

### Administrator

- Full navigation
- Administration menu
- Settings menu
- User and permission management

### Production Manager

- PLM
- Products
- Purchasing
- Fabric
- Production
- Cutting
- Workshop
- Reports

### Warehouse

- Warehouse
- Fabric receiving
- Inventory
- Barcode
- Shipment
- Relevant reports

### Sales

- Sales
- Marketplace
- Dealers
- Products read access
- Shipment visibility
- Sales reports

### Finance

- Finance
- Invoices
- Payments
- Collections
- Expenses
- Financial reports

### Viewer

- Dashboard
- Read-only permitted modules
- Reports based on permission

Menus must automatically hide unauthorized pages.

## Dashboard KPIs

The dashboard must support role-based KPIs.

### KPI List

- Production Efficiency
- Fabric Consumption
- Stock Value
- Today's Sales
- Monthly Sales
- Purchase Costs
- Workshop Performance
- Shipment Performance

### KPI Rules

- KPIs must show clear values.
- KPIs must include trend direction where useful.
- KPI cards must link to source details.
- KPIs must be filtered by user permission.

## Future Improvements

Future UI / UX improvements may include:

- Custom Dashboard
- Widget System
- Drag & Drop Dashboard
- AI Assistant Panel
- Voice Commands

## Status

Draft

## Last Updated

2026-07-05
