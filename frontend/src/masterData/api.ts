import type { MasterDataInput, MasterDataItem, MasterDataType } from './types'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    headers: {
      'Content-Type': 'application/json',
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

export async function getMasterDataItems(type: MasterDataType, search = ''): Promise<MasterDataItem[]> {
  const query = search.trim()
  const path = query
    ? `/api/master-data/${type}?search=${encodeURIComponent(query)}`
    : `/api/master-data/${type}`

  return request<MasterDataItem[]>(path)
}

export async function createMasterDataItem(
  type: MasterDataType,
  item: MasterDataInput,
): Promise<MasterDataItem> {
  return request<MasterDataItem>(`/api/master-data/${type}`, {
    method: 'POST',
    body: JSON.stringify(item),
  })
}

export async function updateMasterDataItem(
  type: MasterDataType,
  id: string,
  item: MasterDataInput,
): Promise<MasterDataItem> {
  return request<MasterDataItem>(`/api/master-data/${type}/${id}`, {
    method: 'PUT',
    body: JSON.stringify(item),
  })
}

export async function deleteMasterDataItem(type: MasterDataType, id: string): Promise<void> {
  await request<void>(`/api/master-data/${type}/${id}`, {
    method: 'DELETE',
  })
}
