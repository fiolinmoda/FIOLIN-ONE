import { apiRequest } from '../common/apiClient'
import type { ReportsOverview } from './types'

export type ReportFilters = {
  search: string
  status: string
  dateFrom: string
  dateTo: string
}

export async function getReportsOverview(filters: ReportFilters): Promise<ReportsOverview> {
  const params = new URLSearchParams()

  if (filters.search.trim()) {
    params.set('search', filters.search.trim())
  }

  if (filters.status.trim()) {
    params.set('status', filters.status.trim())
  }

  if (filters.dateFrom) {
    params.set('dateFrom', filters.dateFrom)
  }

  if (filters.dateTo) {
    params.set('dateTo', filters.dateTo)
  }

  return apiRequest<ReportsOverview>(`/api/reports/overview?${params.toString()}`)
}
