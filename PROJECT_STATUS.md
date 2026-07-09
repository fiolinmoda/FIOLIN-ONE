# FIOLIN ONE Project Status

## Completed Sprint

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

