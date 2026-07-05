export type PagedResult<T> = {
  items: T[]
  page: number
  pageSize: number
  totalItems: number
  totalPages: number
}

export type Supplier = {
  id: string
  supplierCode: string
  supplierName: string
  phone: string | null
  email: string | null
  address: string | null
  taxNumber: string | null
  paymentTerm: string | null
  active: boolean
  createdAt: string
  updatedAt: string | null
}

export type SupplierInput = {
  supplierCode: string
  supplierName: string
  phone: string
  email: string
  address: string
  taxNumber: string
  paymentTerm: string
  active: boolean
}

export type PurchaseOrderItem = {
  id: string
  purchaseOrderId: string
  fabricTypeId: string | null
  fabricType: string | null
  colorId: string | null
  color: string | null
  itemName: string
  quantity: number
  unit: string
  unitPrice: number
  receivedQuantity: number
  remainingQuantity: number
  status: string
}

export type PurchaseOrderItemInput = {
  id: string | null
  fabricTypeId: string | null
  colorId: string | null
  itemName: string
  quantity: number
  unit: string
  unitPrice: number
  receivedQuantity: number
  status: string
}

export type PurchaseOrder = {
  id: string
  purchaseNumber: string
  supplierId: string
  supplierName: string
  orderDate: string
  expectedDate: string | null
  status: string
  notes: string | null
  totalAmount: number
  items: PurchaseOrderItem[]
  createdAt: string
  updatedAt: string | null
}

export type PurchaseOrderInput = {
  purchaseNumber: string
  supplierId: string
  orderDate: string
  expectedDate: string | null
  status: string
  notes: string
  items: PurchaseOrderItemInput[]
}

export type GoodsReceiptItem = {
  id: string
  goodsReceiptId: string
  purchaseOrderItemId: string | null
  itemName: string
  receivedQuantity: number
  unit: string
  acceptance: string
  differenceQuantity: number
}

export type GoodsReceiptItemInput = {
  id: string | null
  purchaseOrderItemId: string | null
  itemName: string
  receivedQuantity: number
  unit: string
  acceptance: string
  differenceQuantity: number
}

export type GoodsReceipt = {
  id: string
  receiptNumber: string
  supplierId: string
  supplierName: string
  purchaseOrderId: string | null
  purchaseNumber: string | null
  receiptDate: string
  warehouse: string
  status: string
  notes: string | null
  items: GoodsReceiptItem[]
  createdAt: string
  updatedAt: string | null
}

export type GoodsReceiptInput = {
  receiptNumber: string
  supplierId: string
  purchaseOrderId: string | null
  receiptDate: string
  warehouse: string
  status: string
  notes: string
  items: GoodsReceiptItemInput[]
}

export type PurchaseInvoiceItem = {
  id: string
  purchaseInvoiceId: string
  purchaseOrderItemId: string | null
  itemName: string
  quantity: number
  unit: string
  unitPrice: number
  totalAmount: number
}

export type PurchaseInvoiceItemInput = {
  id: string | null
  purchaseOrderItemId: string | null
  itemName: string
  quantity: number
  unit: string
  unitPrice: number
  totalAmount: number
}

export type PurchaseInvoice = {
  id: string
  invoiceNumber: string
  invoiceDate: string
  supplierId: string
  supplierName: string
  purchaseOrderId: string | null
  purchaseNumber: string | null
  invoiceAmount: number
  status: string
  notes: string | null
  items: PurchaseInvoiceItem[]
  createdAt: string
  updatedAt: string | null
}

export type PurchaseInvoiceInput = {
  invoiceNumber: string
  invoiceDate: string
  supplierId: string
  purchaseOrderId: string | null
  invoiceAmount: number
  status: string
  notes: string
  items: PurchaseInvoiceItemInput[]
}
