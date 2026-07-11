import { apiRequest } from '../common/apiClient'

export type GoodsReceiptVariant = {
  productId: string
  productVariantId: string
  modelCode: string
  productName: string
  colorId: string
  color: string
  sizeId: string
  size: string
  barcode: string
  stock: number
  lastPurchasePrice: number
  shelf: string | null
  box: string | null
  lastSupplierId: string | null
  lastSupplierName: string | null
}

export type GoodsReceiptOperationInput = {
  supplierId: string
  productVariantId: string
  transactionDate: string
  description: string | null
  purchasePrice: number
  quantity: number
  shelf: string | null
  box: string | null
}

export type GoodsReceiptOperationResult = {
  id: string
  productVariantId: string
  barcode: string
  quantity: number
  stockBefore: number
  stockAfter: number
  purchasePrice: number
  shelf: string | null
  box: string | null
  transactionDate: string
}

export async function findVariantByBarcode(barcode: string): Promise<GoodsReceiptVariant> {
  return apiRequest<GoodsReceiptVariant>(`/api/v2/goods-receipt/barcode/${encodeURIComponent(barcode.trim())}`)
}

export async function getGoodsReceiptVariant(productVariantId: string): Promise<GoodsReceiptVariant> {
  return apiRequest<GoodsReceiptVariant>(`/api/v2/goods-receipt/variants/${productVariantId}`)
}

export async function createGoodsReceiptOperation(
  input: GoodsReceiptOperationInput,
): Promise<GoodsReceiptOperationResult> {
  return apiRequest<GoodsReceiptOperationResult>('/api/v2/goods-receipt', {
    method: 'POST',
    body: JSON.stringify(input),
  })
}
