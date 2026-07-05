import type {
  Fabric,
  FabricConsumptionInput,
  FabricInput,
  FabricMovement,
  FabricMovementInput,
  FabricPurchaseArrivalInput,
  FabricReservation,
  FabricReservationInput,
  PagedResult,
} from './types'
import { apiRequest } from '../common/apiClient'

const request = apiRequest

function query(search: string, status = '', pageSize = 100): string {
  const params = new URLSearchParams({ page: '1', pageSize: String(pageSize) })

  if (search.trim()) {
    params.set('search', search.trim())
  }

  if (status.trim()) {
    params.set('status', status.trim())
  }

  return params.toString()
}

export async function getFabrics(search = '', status = ''): Promise<PagedResult<Fabric>> {
  return request<PagedResult<Fabric>>(`/api/fabrics?${query(search, status)}`)
}

export async function getFabric(id: string): Promise<Fabric> {
  return request<Fabric>(`/api/fabrics/${id}`)
}

export async function createFabric(input: FabricInput): Promise<Fabric> {
  return request<Fabric>('/api/fabrics', { method: 'POST', body: JSON.stringify(input) })
}

export async function updateFabric(id: string, input: Omit<FabricInput, 'currentStockKg'>): Promise<Fabric> {
  return request<Fabric>(`/api/fabrics/${id}`, { method: 'PUT', body: JSON.stringify(input) })
}

export async function deleteFabric(id: string): Promise<void> {
  await request<void>(`/api/fabrics/${id}`, { method: 'DELETE' })
}

export async function getFabricMovements(search = '', status = ''): Promise<PagedResult<FabricMovement>> {
  return request<PagedResult<FabricMovement>>(`/api/fabric-movements?${query(search, status)}`)
}

export async function createFabricMovement(input: FabricMovementInput): Promise<FabricMovement> {
  return request<FabricMovement>('/api/fabric-movements', { method: 'POST', body: JSON.stringify(input) })
}

export async function createFabricPurchaseArrival(input: FabricPurchaseArrivalInput): Promise<FabricMovement> {
  return request<FabricMovement>('/api/fabrics/purchase-arrivals', { method: 'POST', body: JSON.stringify(input) })
}

export async function consumeFabric(input: FabricConsumptionInput): Promise<FabricMovement> {
  return request<FabricMovement>('/api/fabrics/consumption', { method: 'POST', body: JSON.stringify(input) })
}

export async function getFabricReservations(search = '', status = ''): Promise<PagedResult<FabricReservation>> {
  return request<PagedResult<FabricReservation>>(`/api/fabric-reservations?${query(search, status)}`)
}

export async function createFabricReservation(input: FabricReservationInput): Promise<FabricReservation> {
  return request<FabricReservation>('/api/fabric-reservations', { method: 'POST', body: JSON.stringify(input) })
}

export async function updateFabricReservation(id: string, input: FabricReservationInput): Promise<FabricReservation> {
  return request<FabricReservation>(`/api/fabric-reservations/${id}`, { method: 'PUT', body: JSON.stringify(input) })
}

export async function deleteFabricReservation(id: string): Promise<void> {
  await request<void>(`/api/fabric-reservations/${id}`, { method: 'DELETE' })
}
