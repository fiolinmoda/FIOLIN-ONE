export type ProductImportMapping = {
  modelCode: string | null
  productName: string | null
  brand: string | null
  category: string | null
  season: string | null
  color: string | null
  size: string | null
  fabricType: string | null
  purchasePrice: string | null
  salesPrice: string | null
  stock: string | null
}

export type MissingMasterDataMode = 'Create' | 'Skip' | 'Cancel'

export type ProductImportSummary = {
  total: number
  valid: number
  missingField: number
  error: number
  newProducts: number
  existingProducts: number
  skipped: number
}

export type ProductImportPreviewRow = {
  rowNumber: number
  modelCode: string
  productName: string
  status: string
  errors: string[]
}

export type ProductImportProfile = {
  id: string
  profileName: string
  fileSignature: string
  mapping: ProductImportMapping
  createdAt: string
  updatedAt: string | null
}

export type ProductImportPreview = {
  fileName: string
  fileSignature: string
  headers: string[]
  suggestedMapping: ProductImportMapping
  savedProfile: ProductImportProfile | null
  summary: ProductImportSummary
  rows: ProductImportPreviewRow[]
}

export type ProductImportErrorRow = {
  rowNumber: number
  modelCode: string
  productName: string
  reason: string
}

export type ProductImportResult = {
  summary: ProductImportSummary
  inserted: number
  existing: number
  skipped: number
  error: number
  durationMilliseconds: number
  errorRows: ProductImportErrorRow[]
}

export type ProductImportHistory = {
  id: string
  importedAt: string
  userName: string
  fileName: string
  totalRecords: number
  insertedRecords: number
  existingRecords: number
  skippedRecords: number
  errorRecords: number
  durationMilliseconds: number
  status: string
  notes: string | null
}
