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
  code: string
  isActive: boolean
  sortOrder: number
}

export const masterDataLabels: Record<MasterDataType, string> = {
  brands: 'Brands',
  categories: 'Categories',
  seasons: 'Seasons',
  colors: 'Colors',
  sizes: 'Sizes',
  'fabric-types': 'Fabric Types',
}
