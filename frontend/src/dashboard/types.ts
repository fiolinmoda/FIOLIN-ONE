export type DashboardMetric = {
  title: string
  value: number
  unit: string
  link: string
}

export type CriticalProduct = {
  productId: string
  variantId: string
  productCode: string
  productName: string
  color: string
  size: string
  stock: number
  link: string
}

export type RecentDocument = {
  id: string
  documentNumber: string
  title: string
  date: string
  status: string
  quantity: number
  amount: number
  link: string
}

export type DashboardOverview = {
  todaySales: DashboardMetric
  totalOrders: DashboardMetric
  openProductionOrders: DashboardMetric
  currentInventory: DashboardMetric
  criticalProducts: CriticalProduct[]
  recentPurchasing: RecentDocument[]
  recentProduction: RecentDocument[]
  recentSales: RecentDocument[]
}
