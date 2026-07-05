import type {
  CuttingInput,
  PagedResult,
  ProductionDashboard,
  ProductionOrder,
  ProductionOrderInput,
  ProductionTimeline,
  WarehouseEntryInput,
  WorkshopReturnInput,
  WorkshopShipmentInput,
} from './types'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const token = window.localStorage.getItem('fiolin-one-token')
  const response = await fetch(`${apiBaseUrl}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...init?.headers,
    },
    ...init,
  })

  if (!response.ok) {
    const body = await response.json().catch(() => null)
    throw new Error(body?.message ?? 'Request failed.')
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json()
}

function query(search: string, status = '', pageSize = 100): string {
  const params = new URLSearchParams({ page: '1', pageSize: String(pageSize) })
  if (search.trim()) params.set('search', search.trim())
  if (status.trim()) params.set('status', status.trim())
  return params.toString()
}

export async function getProductionDashboard(): Promise<ProductionDashboard> {
  return request<ProductionDashboard>('/api/production-orders/dashboard')
}

export async function getProductionOrders(search = '', status = ''): Promise<PagedResult<ProductionOrder>> {
  return request<PagedResult<ProductionOrder>>(`/api/production-orders?${query(search, status)}`)
}

export async function getProductionOrder(id: string): Promise<ProductionOrder> {
  return request<ProductionOrder>(`/api/production-orders/${id}`)
}

export async function createProductionOrder(input: ProductionOrderInput): Promise<ProductionOrder> {
  return request<ProductionOrder>('/api/production-orders', { method: 'POST', body: JSON.stringify(input) })
}

export async function updateProductionOrder(id: string, input: ProductionOrderInput): Promise<ProductionOrder> {
  return request<ProductionOrder>(`/api/production-orders/${id}`, { method: 'PUT', body: JSON.stringify(input) })
}

export async function deleteProductionOrder(id: string): Promise<void> {
  await request<void>(`/api/production-orders/${id}`, { method: 'DELETE' })
}

export async function sendToIroningPackaging(id: string): Promise<ProductionOrder> {
  return request<ProductionOrder>(`/api/production-orders/${id}/ironing-packaging`, { method: 'POST' })
}

export async function createCutting(input: CuttingInput): Promise<unknown> {
  return request('/api/production-cutting', { method: 'POST', body: JSON.stringify(input) })
}

export async function createWorkshopShipment(input: WorkshopShipmentInput): Promise<unknown> {
  return request('/api/workshop-shipments', { method: 'POST', body: JSON.stringify(input) })
}

export async function createWorkshopReturn(input: WorkshopReturnInput): Promise<unknown> {
  return request('/api/workshop-returns', { method: 'POST', body: JSON.stringify(input) })
}

export async function createWarehouseEntry(input: WarehouseEntryInput): Promise<unknown> {
  return request('/api/production-warehouse-entries', { method: 'POST', body: JSON.stringify(input) })
}

export async function getProductionTimeline(productionOrderId: string): Promise<ProductionTimeline[]> {
  return request<ProductionTimeline[]>(`/api/production-orders/${productionOrderId}/timeline`)
}
