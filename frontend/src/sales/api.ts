import { apiRequest } from '../common/apiClient'
import type { PagedResult, SalesOrder, SalesOrderInput } from './types'

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

export async function getSalesOrders(search = '', status = ''): Promise<PagedResult<SalesOrder>> {
  return request<PagedResult<SalesOrder>>(`/api/sales-orders?${query(search, status)}`)
}

export async function getSalesOrder(id: string): Promise<SalesOrder> {
  return request<SalesOrder>(`/api/sales-orders/${id}`)
}

export async function createSalesOrder(input: SalesOrderInput): Promise<SalesOrder> {
  return request<SalesOrder>('/api/sales-orders', { method: 'POST', body: JSON.stringify(input) })
}

export async function updateSalesOrder(id: string, input: SalesOrderInput): Promise<SalesOrder> {
  return request<SalesOrder>(`/api/sales-orders/${id}`, { method: 'PUT', body: JSON.stringify(input) })
}

export async function deleteSalesOrder(id: string): Promise<void> {
  await request<void>(`/api/sales-orders/${id}`, { method: 'DELETE' })
}
