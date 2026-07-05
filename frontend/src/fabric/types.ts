import type { PagedResult } from '../purchasing/types'

export type { PagedResult }

export type Fabric = {
  id: string
  fabricCode: string
  fabricName: string
  supplierId: string
  supplierName: string
  colorId: string
  color: string
  composition: string | null
  width: number
  weightGsm: number
  unit: string
  purchasePrice: number
  currentStockKg: number
  minimumStock: number
  reservedQuantityKg: number
  availableStockKg: number
  status: string
  notes: string | null
  createdAt: string
  updatedAt: string | null
}

export type FabricInput = {
  fabricCode: string
  fabricName: string
  supplierId: string
  colorId: string
  composition: string
  width: number
  weightGsm: number
  unit: string
  purchasePrice: number
  currentStockKg: number
  minimumStock: number
  status: string
  notes: string
}

export type FabricMovement = {
  id: string
  fabricId: string
  fabricCode: string
  fabricName: string
  movementType: string
  quantityKg: number
  unitPrice: number
  supplierId: string | null
  supplierName: string | null
  purchaseOrderId: string | null
  purchaseNumber: string | null
  batchLot: string | null
  warehouse: string
  movementDate: string
  notes: string | null
  createdAt: string
}

export type FabricPurchaseArrivalInput = {
  supplierId: string
  purchaseOrderId: string | null
  fabricId: string
  colorId: string
  batchLot: string
  totalWeightKg: number
  unitPrice: number
  warehouse: string
  arrivalDate: string
  notes: string
}

export type FabricMovementInput = {
  fabricId: string
  movementType: string
  quantityKg: number
  unitPrice: number
  supplierId: string | null
  purchaseOrderId: string | null
  batchLot: string
  warehouse: string
  movementDate: string
  notes: string
}

export type FabricConsumptionInput = {
  fabricId: string
  quantityKg: number
  productionReference: string
  consumptionDate: string
  notes: string
}

export type FabricReservation = {
  id: string
  fabricId: string
  fabricCode: string
  fabricName: string
  reservationNumber: string
  productionReference: string
  reservedQuantityKg: number
  reservationDate: string
  status: string
  notes: string | null
  createdAt: string
  updatedAt: string | null
}

export type FabricReservationInput = {
  fabricId: string
  reservationNumber: string
  productionReference: string
  reservedQuantityKg: number
  reservationDate: string
  status: string
  notes: string
}
