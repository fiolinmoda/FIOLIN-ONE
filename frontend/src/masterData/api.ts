import type { MasterDataInput, MasterDataItem, MasterDataType } from './types'
import { apiRequest } from '../common/apiClient'

const request = apiRequest

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
