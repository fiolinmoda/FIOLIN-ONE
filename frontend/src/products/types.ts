export type Product = {
  id: string
  modelCode: string
  productCode: string
  productName: string
  brandId: string | null
  brand: string | null
  categoryId: string | null
  category: string | null
  seasonId: string | null
  season: string | null
  status: string
  imageUrl: string | null
  colorCount: number
  sizeCount: number
  variantCount: number
  totalStock: number
  colorGroups: ProductColorGroup[]
  createdAt: string
  updatedAt: string | null
}

export type ProductColorGroup = {
  colorId: string
  color: string
  totalStock: number
  sizes: ProductSizeVariant[]
}

export type ProductSizeVariant = {
  variantId: string
  sizeId: string
  size: string
  barcode: string
  stock: number
  purchasePrice: number
  salesPrice: number
  shelf: string | null
  box: string | null
}

export type ProductInput = {
  modelCode: string
  productCode: string
  productName: string
  brandId: string | null
  categoryId: string | null
  seasonId: string | null
  status: string
}

export type ProductVariant = {
  id: string
  productId: string
  colorId: string
  color: string
  sizeId: string
  size: string
  barcode: string
  trendyolSku: string | null
  stock: number
  purchasePrice: number
  salesPrice: number
  status: string
  createdAt: string
  updatedAt: string | null
}

export type ProductVariantInput = {
  colorId: string
  sizeId: string
  barcode: string
  trendyolSku: string
  stock: number
  status: string
}
