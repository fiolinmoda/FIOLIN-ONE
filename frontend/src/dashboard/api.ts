import { apiRequest } from '../common/apiClient'
import type { DashboardOverview } from './types'

export async function getDashboardOverview(): Promise<DashboardOverview> {
  return apiRequest<DashboardOverview>('/api/dashboard/overview')
}
