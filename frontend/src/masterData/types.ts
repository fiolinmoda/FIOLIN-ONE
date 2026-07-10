export type MasterDataType = 'brands' | 'categories' | 'seasons' | 'colors' | 'sizes' | 'fabric-types'

export type MasterDataItem = {
  id: string
  name: string
  code: string
  isActive: boolean
  sortOrder: number
  createdAt: string
  updatedAt: string | null
}

export type MasterDataInput = {
  name: string
  isActive: boolean
}

export const masterDataLabels: Record<MasterDataType, string> = {
  brands: 'Markalar',
  categories: 'Kategoriler',
  seasons: 'Sezonlar',
  colors: 'Renkler',
  sizes: 'Bedenler',
  'fabric-types': 'Kumaş Tipleri',
}

export const masterDataSingularLabels: Record<MasterDataType, string> = {
  brands: 'Marka',
  categories: 'Kategori',
  seasons: 'Sezon',
  colors: 'Renk',
  sizes: 'Beden',
  'fabric-types': 'Kumaş Tipi',
}
