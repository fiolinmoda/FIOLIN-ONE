# FIOLIN ONE Project Status

## Completed Sprint

### Workspace & Navigation

Status: Completed

Completed work:

- Navigation was reorganized around business workspaces:
  - Ana Panel
  - Ürün Yönetimi
  - Satın Alma
  - Üretim
  - Depo
  - Satış
  - Raporlar
  - Sistem Tanımları
  - Ayarlar
- Related pages were grouped under their business modules.
- Sidebar collapsed state is now remembered locally.
- Last selected page is remembered and reopened from the root route.
- Current module and page are highlighted consistently.
- Header now shows the active module title and breadcrumb.
- Dashboard placeholder layout was added for:
  - Yönetici
  - Satın Alma
  - Depo
  - Üretim
  - Muhasebe
- System definitions were grouped under "Sistem Tanımları" and remain prepared for future role-based visibility.
- Placeholder pages were added for future workspaces so navigation has no broken entries.

Known workspace limitations:

- Dashboard cards are placeholders only; KPI widgets are planned for later sprints.
- Sales, Reports, Settings and some Warehouse pages have prepared layouts but no business transactions yet.
- Mobile navigation uses the same drawer model, but detailed tablet/mobile UX should receive a separate pass.

Next sprint recommendation:

- Add real role-based menu visibility after authentication is implemented.
- Add dashboard KPI widgets using existing transactional data.
- Complete Warehouse product stock, barcode and counting workflows.

### Business Workflow Validation

Status: Completed

Validated workflow:

- Supplier
- Purchase Order
- Goods Receipt
- Fabric Inventory
- Production Order
- Cutting
- Workshop Shipment
- Workshop Return
- Ironing
- Packaging
- Warehouse Entry
- Finished Goods Inventory

Completed work:

- Verified that cutting consumes fabric and creates a fabric stock movement.
- Warehouse entry now completes the production order and increases finished goods stock on product variants.
- Warehouse entry can no longer be duplicated for the same production order.
- Workshop return now updates the related workshop shipment as `PARTIAL_RETURN` or `RETURNED`.
- Production timeline remains populated for production creation, fabric consumption, cutting, workshop shipment, workshop return, ironing/packaging and warehouse entry.
- Barcode-ready fields on production order items were verified and covered by tests.
- Business workflow tests were added for finished goods stock, workshop return status and barcode-ready fields.

End-to-end example verified:

- Example stamp: `20260709230906`
- Purchase Order: `SAT-2026-000001`
- Goods Receipt: `MK-2026-000001`
- Purchase Invoice: `FAT-2026-000001`
- Fabric Code: `KMS-2026-000001`
- Product Code: `URN-2026-000001`
- Production Order: `URT-2026-000001`
- Fabric stock after purchase arrival: `50 Kg`
- Fabric stock after cutting: `38 Kg`
- Finished goods variant stock after warehouse entry: `20`
- Final production status: `COMPLETED`
- Workshop shipment status: `RETURNED`
- Timeline entries: `7`
- Search and completed-status filter verified through API.

Known workflow limitations:

- Goods Receipt and Fabric Inventory are still connected by a manual fabric purchase movement because goods receipt items do not yet reference a Fabric card directly.
- Finished goods inventory currently updates Product Variant stock only; a dedicated warehouse/location inventory ledger is still future work.
- Reports are currently covered by dashboards and list filters, not by a complete reporting module.

Next sprint recommendation:

- Connect Goods Receipt items directly to Fabric cards so accepted fabric can create inventory movement automatically.
- Add a Warehouse Inventory ledger for finished goods, locations and movement history.
- Add end-to-end integration tests using PostgreSQL/Testcontainers or Docker Compose.
- Add workflow reports for purchase waiting, fabric stock, production status and finished goods inventory.

### ERP Usability & Smart Forms

Status: Completed

Completed work:

- Central automatic document numbering foundation added.
- Year-based sequential document numbers implemented for:
  - Product Code
  - Fabric Code
  - Purchase Order Number
  - Goods Receipt Number
  - Purchase Invoice Number
  - Production Order Number
  - Reservation Number
- New create forms no longer require users to manually enter business document numbers.
- Create forms show "Otomatik oluşturulacaktır" for automatic numbers.
- Product, Fabric and Production create screens include "Kaydet ve Yeni".
- Success snackbar feedback added to main smart forms.
- Master Data menu renamed to "Sistem Tanımları".
- Navigation model prepared for future role-based menu visibility.
- Turkish FluentValidation messages added for the most important smart-form validation paths.

## Known Issues

- Frontend bundle is larger than Vite's default recommended chunk size. Code splitting should be planned.
- Some older UI text still needs a full Turkish language QA pass because early files contain mixed encoding artifacts.
- Dialog forms have automatic numbers, but not every dialog has "Kaydet ve Yeni" yet.
- Automated integration tests for document numbering concurrency are not implemented yet.
- `dotnet-ef` is not installed as a local tool; the document sequence migration was added manually.

## Next Sprint

Recommended next sprint: Full Turkish UX Polish & Form Validation Hardening

Priorities:

- Add a reusable form validation helper for field-level backend errors.
- Add "Kaydet ve Yeni" consistently to all create dialogs.
- Add focused integration tests for automatic document numbering.
- Add frontend code splitting for large modules.
- Complete a full Turkish copy and encoding review across all UI files.
