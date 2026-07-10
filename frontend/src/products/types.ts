export type Product = {
  id: string
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
  createdAt: string
  updatedAt: string | null
}

export type ProductInput = {
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
