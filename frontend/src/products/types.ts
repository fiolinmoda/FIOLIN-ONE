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
