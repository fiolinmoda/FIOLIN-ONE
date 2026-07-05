# Product Development (PLM)

## Purpose

The Product Development (PLM) module manages the complete lifecycle of a clothing model from idea until production approval. It controls all design, technical, sample, revision, file, and approval information before a model is released to production.

The workflow begins before production. Production orders, cutting, sewing, purchasing, and warehouse operations should only rely on models that have completed the PLM approval process and are marked as ready for production.

This module gives FIOLIN ONE a single source of truth for model development decisions, design files, technical sheets, pattern files, sample history, and revision history.

---

## Business Workflow

Idea

↓

Model Creation

↓

Design Drawing

↓

Technical Sheet

↓

Pattern Preparation

↓

Sample Production

↓

Sample Review

↓

Revision

↓

Approval

↓

Ready For Production

The process starts with a product idea. A model card is created to represent the concept. Designers attach drawings and initial notes. Technical teams prepare technical sheets and pattern files. A sample is produced and reviewed. If changes are needed, revisions are recorded and a new sample cycle may begin. Once the sample, technical sheet, and pattern are approved, the model becomes ready for production.

---

## Business Rules

- Every model has its own model code.
- Design drawings are stored and attached to the model.
- Technical sheets are attached as PDF.
- Pattern files (Gerber, Lectra, etc.) are attached.
- Sample photos are attached.
- A model may have multiple revisions.
- Production cannot start before approval.
- Every revision must be recorded.
- Old files are never deleted.

These rules ensure traceability. A model's design history must remain available even after newer revisions are created. Files should be versioned or linked to revisions instead of overwritten. If a design, pattern, or technical sheet changes, the change must create a revision record.

---

## Model Card

The Model Card is the central record for product development. It represents the clothing model before it becomes a production-ready product card.

Fields:

- Model Code
- Model Name
- Brand
- Season
- Collection
- Category
- Designer
- Status
- Description
- Creation Date
- Approval Date

The model code should be unique. The status indicates where the model is in the development lifecycle. Approval date should remain empty until the model is approved for production.

---

## File Management

Each model can contain:

- Design Drawing
- Technical Sheet
- Pattern File
- Measurement Table
- Sample Photos
- Other Documents

Supported file types:

- PDF
- DOCX
- XLSX
- ZIP
- JPG
- PNG

File management must preserve history. Old files are never deleted. If users upload a new version of a technical sheet, pattern file, drawing, or measurement table, FIOLIN ONE should keep the previous file and record who uploaded the new file and when.

Recommended file metadata:

- File Name
- File Type
- File Category
- Revision Number
- Uploaded By
- Uploaded Date
- Description
- Active Version

---

## Sample Management

Sample Management tracks sample requests, production, review, and approval.

Fields:

- Sample Number
- Sample Date
- Revision Number
- Responsible Person
- Notes
- Status

Possible Status:

- Requested
- In Progress
- Completed
- Rejected
- Approved

A model may have multiple samples. Each sample should reference the model and, when applicable, the revision number. Sample photos and review notes should be attached to the sample record.

Sample records help the company understand how many sample cycles were required before approval and where delays occurred.

---

## Revision Management

Each revision stores:

- Revision Number
- Date
- Reason
- Changed By
- Notes

Revision Management records every meaningful change to a model during development. A revision may be triggered by sample rejection, customer feedback, designer changes, pattern correction, measurement correction, fabric change, or production feasibility review.

Each revision should link to relevant files, samples, and approval history. Revision numbers must be sequential per model.

---

## Approval Process

Statuses:

- Draft
- Under Design
- Waiting Sample
- Revision Required
- Approved
- Production Ready
- Archived

### Draft

The model card exists but development work has not formally started.

### Under Design

Design drawings, product details, and early technical information are being prepared.

### Waiting Sample

The model is waiting for sample production or sample completion.

### Revision Required

The sample or technical review found issues that require a new revision.

### Approved

The model has passed sample and technical review.

### Production Ready

The model is approved and released for production use.

### Archived

The model is no longer active for new development or production.

Production cannot start before the model reaches Production Ready status.

---

## Database Proposal

Suggested entities:

### Model

Stores model card information such as model code, model name, brand, season, collection, category, designer, status, description, creation date, and approval date.

### ModelRevision

Stores revision number, date, reason, changed by, and notes for each model revision.

### ModelFile

Stores all model-related files such as design drawings, technical sheets, pattern files, measurement tables, sample photos, and other documents.

### Sample

Stores sample number, sample date, revision number, responsible person, notes, status, and sample-related attachments.

### ApprovalHistory

Stores approval events, status changes, approver, date, and notes.

### Designer

Stores designer master data and performance-related references.

Additional future entities may include MeasurementTable, SampleReview, ModelComment, ModelCollection, and FileVersion.

---

## API Proposal

REST endpoints for:

### Models

- `GET /api/models`
- `GET /api/models/{id}`
- `POST /api/models`
- `PUT /api/models/{id}`
- `DELETE /api/models/{id}`

### Revisions

- `GET /api/models/{modelId}/revisions`
- `GET /api/models/{modelId}/revisions/{revisionId}`
- `POST /api/models/{modelId}/revisions`
- `PUT /api/models/{modelId}/revisions/{revisionId}`

### Files

- `GET /api/models/{modelId}/files`
- `POST /api/models/{modelId}/files`
- `GET /api/models/{modelId}/files/{fileId}`
- `POST /api/models/{modelId}/files/{fileId}/archive`

### Samples

- `GET /api/models/{modelId}/samples`
- `GET /api/models/{modelId}/samples/{sampleId}`
- `POST /api/models/{modelId}/samples`
- `PUT /api/models/{modelId}/samples/{sampleId}`

### Approvals

- `GET /api/models/{modelId}/approvals`
- `POST /api/models/{modelId}/approve`
- `POST /api/models/{modelId}/request-revision`
- `POST /api/models/{modelId}/release-to-production`

APIs should validate status transitions so a model cannot move directly from Draft to Production Ready without required development and approval steps.

---

## UI Proposal

Pages:

### Model List

Search, filter, and manage all model cards by status, brand, season, category, designer, and creation date.

### Model Detail

Central model workspace showing model card fields, status, description, files, samples, revisions, and approval history.

### Files

File management page or tab for design drawings, technical sheets, pattern files, measurement tables, sample photos, and other documents.

### Sample Tracking

Sample request, sample status, sample review notes, sample photos, and approval decision tracking.

### Revision History

Timeline of all model revisions with reason, changed by, files, and notes.

### Approval History

Timeline of status changes, approvals, revision requests, and production release events.

### Dashboard

Overview of models waiting approval, models under design, samples in progress, and revision workload.

---

## Reports

### Models Waiting Approval

Shows models that are ready for approval but not yet approved.

### Approved Models

Shows models approved or released for production by date range, brand, season, and designer.

### Revision History

Shows revision counts and details by model, designer, reason, and date range.

### Sample Status

Shows requested, in progress, completed, rejected, and approved samples.

### Designer Performance

Shows model counts, approval rate, revision count, and average development duration by designer.

---

## Future Improvements

### Digital Signature

Support electronic approval signatures for model approval and production release.

### Version Comparison

Compare technical sheet, measurement table, or pattern file versions between revisions.

### Automatic PDF Generation

Generate standardized model technical sheets and approval packs from system data.

### 3D Sample Integration

Integrate 3D sample files or previews for digital product development.

### AI Image Generation

Generate design concept images or visual variations from prompts and reference images.

---

## Status

Draft

## Last Updated

2026-07-04
