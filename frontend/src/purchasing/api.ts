import type {
  GoodsReceipt,
  GoodsReceiptInput,
  PagedResult,
  PurchaseInvoice,
  PurchaseInvoiceInput,
  PurchaseOrder,
  PurchaseOrderInput,
  Supplier,
  SupplierInput,
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
    const message = body?.message ?? 'Request failed.'

    throw new Error(message)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json()
}

function query(search: string, pageSize = 100): string {
  const params = new URLSearchParams({ page: '1', pageSize: String(pageSize) })
  const term = search.trim()

  if (term) {
    params.set('search', term)
  }

  return params.toString()
}

export async function getSuppliers(search = ''): Promise<PagedResult<Supplier>> {
  return request<PagedResult<Supplier>>(`/api/suppliers?${query(search)}`)
}

export async function getSupplier(id: string): Promise<Supplier> {
  return request<Supplier>(`/api/suppliers/${id}`)
}

export async function createSupplier(input: SupplierInput): Promise<Supplier> {
  return request<Supplier>('/api/suppliers', {
    method: 'POST',
    body: JSON.stringify(input),
  })
}

export async function updateSupplier(id: string, input: SupplierInput): Promise<Supplier> {
  return request<Supplier>(`/api/suppliers/${id}`, {
    method: 'PUT',
    body: JSON.stringify(input),
  })
}

export async function deleteSupplier(id: string): Promise<void> {
  await request<void>(`/api/suppliers/${id}`, { method: 'DELETE' })
}

export async function getPurchaseOrders(search = ''): Promise<PagedResult<PurchaseOrder>> {
  return request<PagedResult<PurchaseOrder>>(`/api/purchase-orders?${query(search)}`)
}

export async function getPurchaseOrder(id: string): Promise<PurchaseOrder> {
  return request<PurchaseOrder>(`/api/purchase-orders/${id}`)
}

export async function createPurchaseOrder(input: PurchaseOrderInput): Promise<PurchaseOrder> {
  return request<PurchaseOrder>('/api/purchase-orders', {
    method: 'POST',
    body: JSON.stringify(input),
  })
}

export async function updatePurchaseOrder(id: string, input: PurchaseOrderInput): Promise<PurchaseOrder> {
  return request<PurchaseOrder>(`/api/purchase-orders/${id}`, {
    method: 'PUT',
    body: JSON.stringify(input),
  })
}

export async function deletePurchaseOrder(id: string): Promise<void> {
  await request<void>(`/api/purchase-orders/${id}`, { method: 'DELETE' })
}

export async function getGoodsReceipts(search = ''): Promise<PagedResult<GoodsReceipt>> {
  return request<PagedResult<GoodsReceipt>>(`/api/goods-receipts?${query(search)}`)
}

export async function createGoodsReceipt(input: GoodsReceiptInput): Promise<GoodsReceipt> {
  return request<GoodsReceipt>('/api/goods-receipts', {
    method: 'POST',
    body: JSON.stringify(input),
  })
}

export async function updateGoodsReceipt(id: string, input: GoodsReceiptInput): Promise<GoodsReceipt> {
  return request<GoodsReceipt>(`/api/goods-receipts/${id}`, {
    method: 'PUT',
    body: JSON.stringify(input),
  })
}

export async function deleteGoodsReceipt(id: string): Promise<void> {
  await request<void>(`/api/goods-receipts/${id}`, { method: 'DELETE' })
}

export async function getPurchaseInvoices(search = ''): Promise<PagedResult<PurchaseInvoice>> {
  return request<PagedResult<PurchaseInvoice>>(`/api/purchase-invoices?${query(search)}`)
}

export async function createPurchaseInvoice(input: PurchaseInvoiceInput): Promise<PurchaseInvoice> {
  return request<PurchaseInvoice>('/api/purchase-invoices', {
    method: 'POST',
    body: JSON.stringify(input),
  })
}

export async function updatePurchaseInvoice(id: string, input: PurchaseInvoiceInput): Promise<PurchaseInvoice> {
  return request<PurchaseInvoice>(`/api/purchase-invoices/${id}`, {
    method: 'PUT',
    body: JSON.stringify(input),
  })
}

export async function deletePurchaseInvoice(id: string): Promise<void> {
  await request<void>(`/api/purchase-invoices/${id}`, { method: 'DELETE' })
}
