export type ReportSummary = {
  productCount: number
  productVariantCount: number
  productStockQuantity: number
  fabricStockKg: number
  purchaseOrderAmount: number
  purchaseInvoiceAmount: number
  plannedProductionQuantity: number
  completedProductionQuantity: number
  salesAmount: number
  soldQuantity: number
}

export type ProductReportRow = {
  id: string
  productCode: string
  productName: string
  brand: string
  category: string
  season: string
  status: string
  variantCount: number
  stockQuantity: number
}

export type InventoryReportRow = {
  id: string
  inventoryType: string
  code: string
  name: string
  detail: string
  quantity: number
  unit: string
  status: string
}

export type PurchasingReportRow = {
  id: string
  documentNumber: string
  supplierName: string
  documentDate: string
  status: string
  orderedQuantity: number
  receivedQuantity: number
  remainingQuantity: number
  totalAmount: number
  invoiceAmount: number
}

export type ProductionReportRow = {
  id: string
  productionNumber: string
  productCode: string
  productName: string
  createdAt: string
  reason: string
  status: string
  plannedQuantity: number
  warehouseQuantity: number
}

export type SalesReportRow = {
  id: string
  salesOrderNumber: string
  customerName: string
  orderDate: string
  status: string
  quantity: number
  totalAmount: number
}

export type StockConsistencyRow = {
  area: string
  incomingQuantity: number
  outgoingQuantity: number
  currentStock: number
  unit: string
  status: string
}

export type ReportsOverview = {
  summary: ReportSummary
  products: ProductReportRow[]
  inventory: InventoryReportRow[]
  purchasing: PurchasingReportRow[]
  production: ProductionReportRow[]
  sales: SalesReportRow[]
  stockConsistency: StockConsistencyRow[]
}
