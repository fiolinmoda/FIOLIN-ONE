import { apiFormRequest, apiRequest } from '../common/apiClient'
import type {
  MissingMasterDataMode,
  ProductImportHistory,
  ProductImportMapping,
  ProductImportPreview,
  ProductImportResult,
} from './importTypes'

type PreviewOptions = {
  mapping: ProductImportMapping | null
  missingMasterDataMode: MissingMasterDataMode
}

type ImportOptions = {
  mapping: ProductImportMapping
  missingMasterDataMode: MissingMasterDataMode
  profileName: string | null
  saveProfile: boolean
}

function toFormData(file: File, options: unknown) {
  const formData = new FormData()
  formData.append('file', file)
  formData.append('options', JSON.stringify(options))
  return formData
}

export function previewProductImport(file: File, options: PreviewOptions) {
  return apiFormRequest<ProductImportPreview>('/api/products/import/preview', toFormData(file, options))
}

export function executeProductImport(file: File, options: ImportOptions) {
  return apiFormRequest<ProductImportResult>('/api/products/import', toFormData(file, options))
}

export function getProductImportHistory() {
  return apiRequest<ProductImportHistory[]>('/api/products/import/history')
}
