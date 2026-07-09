import type { PagedResult } from '../purchasing/types'

export type { PagedResult }

export type SalesOrderItem = {
  id: string
  productVariantId: string
  productCode: string
  productName: string
  color: string
  size: string
  barcode: string
  quantity: number
  unitPrice: number
  totalAmount: number
  availableStock: number
}

export type SalesOrder = {
  id: string
  salesOrderNumber: string
  customerName: string
  orderDate: string
  status: string
  totalAmount: number
  notes: string | null
  items: SalesOrderItem[]
  createdAt: string
  updatedAt: string | null
}

export type SalesOrderItemInput = {
  id: string | null
  productVariantId: string
  quantity: number
  unitPrice: number
}

export type SalesOrderInput = {
  salesOrderNumber: string
  customerName: string
  orderDate: string
  status: string
  notes: string
  items: SalesOrderItemInput[]
}
