export type Product = {
  id: string
  productCode: string
  productName: string
  brand: string | null
  category: string
  season: string | null
  status: string
  createdAt: string
  updatedAt: string | null
}

export type ProductInput = {
  productCode: string
  productName: string
  brand: string
  category: string
  season: string
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
  color: string
  size: string
  barcode: string
  trendyolSku: string
  stock: number
  status: string
}
