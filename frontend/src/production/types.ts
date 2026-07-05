import type { PagedResult } from '../purchasing/types'

export type { PagedResult }

export type ProductionOrderItem = {
  id: string
  productVariantId: string
  variantName: string
  plannedQuantity: number
  barcodeGenerated: boolean
  barcodePrinted: boolean
  barcodeValue: string | null
}

export type ProductionOrder = {
  id: string
  productionNumber: string
  productId: string
  productCode: string
  productName: string
  plannedQuantity: number
  productionReason: string
  notes: string | null
  status: string
  items: ProductionOrderItem[]
  createdAt: string
  updatedAt: string | null
}

export type ProductionOrderInput = {
  productionNumber: string
  productId: string
  plannedQuantity: number
  productionReason: string
  notes: string
  status: string
  items: Array<{ productVariantId: string; plannedQuantity: number }>
}

export type ProductionDashboard = {
  productionPlanned: number
  inCutting: number
  atWorkshop: number
  ironingPackaging: number
  completed: number
}

export type CuttingInput = {
  productionOrderId: string
  fabricId: string
  consumedWeightKg: number
  wasteWeightKg: number
  cuttingDate: string
  operatorName: string
  notes: string
}

export type WorkshopShipmentInput = {
  productionOrderId: string
  workshop: string
  shipmentDate: string
  expectedReturnDate: string | null
  sentQuantity: number
  notes: string
  status: string
}

export type WorkshopReturnInput = {
  productionOrderId: string
  workshopShipmentId: string | null
  returnedQuantity: number
  extraQuantity: number
  missingQuantity: number
  returnDate: string
  notes: string
}

export type WarehouseEntryInput = {
  productionOrderId: string
  actualQuantity: number
  warehouseDate: string
  notes: string
}

export type ProductionTimeline = {
  id: string
  productionOrderId: string
  eventType: string
  description: string
  eventDate: string
  createdAt: string
}
