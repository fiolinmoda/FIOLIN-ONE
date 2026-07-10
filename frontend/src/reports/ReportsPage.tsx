import { useCallback, useEffect, useMemo, useState } from 'react'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import { Alert, Box, Button, Chip, InputAdornment, MenuItem, Paper, Skeleton, Stack, Tab, Tabs, TextField, Typography } from '@mui/material'
import DownloadOutlinedIcon from '@mui/icons-material/DownloadOutlined'
import SearchIcon from '@mui/icons-material/Search'
import { getReportsOverview, type ReportFilters } from './api'
import type {
  InventoryReportRow,
  ProductReportRow,
  ProductionReportRow,
  PurchasingReportRow,
  ReportsOverview,
  SalesReportRow,
  StockConsistencyRow,
} from './types'
import { commonText, trStatus } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

type ReportTab = 'products' | 'inventory' | 'purchasing' | 'production' | 'sales' | 'consistency'

const statusOptions = [
  'Active',
  'OUT OF STOCK',
  'Draft',
  'Approved',
  'Completed',
  'Cancelled',
  'PLANNED',
  'FABRIC_ALLOCATED',
  'CUTTING',
  'AT_WORKSHOP',
  'AT_IRONING_PACKAGING',
  'READY_FOR_WAREHOUSE',
  'COMPLETED',
  'CANCELLED',
]

const emptyOverview: ReportsOverview = {
  summary: {
    productCount: 0,
    productVariantCount: 0,
    productStockQuantity: 0,
    fabricStockKg: 0,
    purchaseOrderAmount: 0,
    purchaseInvoiceAmount: 0,
    plannedProductionQuantity: 0,
    completedProductionQuantity: 0,
    salesAmount: 0,
    soldQuantity: 0,
  },
  products: [],
  inventory: [],
  purchasing: [],
  production: [],
  sales: [],
  stockConsistency: [],
}

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString('tr-TR')
}

function formatNumber(value: number): string {
  return new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 2 }).format(value)
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value)
}

function exportRows(fileName: string, rows: Record<string, unknown>[]) {
  if (rows.length === 0) {
    return
  }

  const headers = Object.keys(rows[0])
  const escape = (value: unknown) => `"${String(value ?? '').replaceAll('"', '""')}"`
  const csv = [headers.join(';'), ...rows.map((row) => headers.map((header) => escape(row[header])).join(';'))].join('\n')
  const blob = new Blob([`\uFEFF${csv}`], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  link.click()
  URL.revokeObjectURL(url)
}

export function ReportsPage() {
  const [activeTab, setActiveTab] = useState<ReportTab>('products')
  const [filters, setFilters] = useState<ReportFilters>({ search: '', status: '', dateFrom: '', dateTo: '' })
  const [overview, setOverview] = useState<ReportsOverview>(emptyOverview)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const loadReports = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      setOverview(await getReportsOverview(filters))
    } catch (exception) {
      setError(toUserMessage(exception, 'Raporlar yüklenemedi.'))
    } finally {
      setLoading(false)
    }
  }, [filters])

  useEffect(() => {
    const handle = window.setTimeout(() => {
      void loadReports()
    }, 250)

    return () => window.clearTimeout(handle)
  }, [loadReports])

  const productColumns = useMemo<GridColDef<ProductReportRow>[]>(
    () => [
      { field: 'productCode', headerName: 'Ürün Kodu', minWidth: 150, flex: 0.8 },
      { field: 'productName', headerName: 'Ürün Adı', minWidth: 220, flex: 1.3 },
      { field: 'brand', headerName: 'Marka', minWidth: 140, flex: 0.8 },
      { field: 'category', headerName: 'Kategori', minWidth: 140, flex: 0.8 },
      { field: 'season', headerName: 'Sezon', minWidth: 120, flex: 0.7 },
      { field: 'variantCount', headerName: 'Varyant', type: 'number', minWidth: 100, flex: 0.5 },
      { field: 'stockQuantity', headerName: 'Stok', type: 'number', minWidth: 110, flex: 0.5 },
      { field: 'status', headerName: 'Durum', minWidth: 120, flex: 0.6, valueFormatter: (value: string) => trStatus(value) },
    ],
    [],
  )

  const inventoryColumns = useMemo<GridColDef<InventoryReportRow>[]>(
    () => [
      { field: 'inventoryType', headerName: 'Stok Tipi', minWidth: 130, flex: 0.7 },
      { field: 'code', headerName: 'Kod', minWidth: 150, flex: 0.8 },
      { field: 'name', headerName: 'Ad', minWidth: 220, flex: 1.2 },
      { field: 'detail', headerName: 'Detay', minWidth: 240, flex: 1.3 },
      { field: 'quantity', headerName: 'Miktar', type: 'number', minWidth: 120, flex: 0.6, valueFormatter: (value: number) => formatNumber(value) },
      { field: 'unit', headerName: 'Birim', minWidth: 90, flex: 0.4 },
      { field: 'status', headerName: 'Durum', minWidth: 130, flex: 0.6, valueFormatter: (value: string) => trStatus(value) },
    ],
    [],
  )

  const purchasingColumns = useMemo<GridColDef<PurchasingReportRow>[]>(
    () => [
      { field: 'documentNumber', headerName: 'Belge No', minWidth: 160, flex: 0.8 },
      { field: 'supplierName', headerName: 'Tedarikçi', minWidth: 220, flex: 1.1 },
      { field: 'documentDate', headerName: 'Tarih', minWidth: 130, flex: 0.6, valueFormatter: (value: string) => formatDate(value) },
      { field: 'orderedQuantity', headerName: 'Sipariş', type: 'number', minWidth: 110, flex: 0.5 },
      { field: 'receivedQuantity', headerName: 'Kabul', type: 'number', minWidth: 110, flex: 0.5 },
      { field: 'remainingQuantity', headerName: 'Kalan', type: 'number', minWidth: 110, flex: 0.5 },
      { field: 'totalAmount', headerName: 'Sipariş Tutarı', type: 'number', minWidth: 150, flex: 0.7, valueFormatter: (value: number) => formatCurrency(value) },
      { field: 'invoiceAmount', headerName: 'Fatura', type: 'number', minWidth: 140, flex: 0.7, valueFormatter: (value: number) => formatCurrency(value) },
      { field: 'status', headerName: 'Durum', minWidth: 140, flex: 0.6, valueFormatter: (value: string) => trStatus(value) },
    ],
    [],
  )

  const productionColumns = useMemo<GridColDef<ProductionReportRow>[]>(
    () => [
      { field: 'productionNumber', headerName: 'Üretim No', minWidth: 160, flex: 0.8 },
      { field: 'productCode', headerName: 'Ürün Kodu', minWidth: 150, flex: 0.7 },
      { field: 'productName', headerName: 'Ürün', minWidth: 220, flex: 1.2 },
      { field: 'createdAt', headerName: 'Tarih', minWidth: 130, flex: 0.6, valueFormatter: (value: string) => formatDate(value) },
      { field: 'reason', headerName: 'Neden', minWidth: 120, flex: 0.6, valueFormatter: (value: string) => trStatus(value) },
      { field: 'plannedQuantity', headerName: 'Planlanan', type: 'number', minWidth: 130, flex: 0.6 },
      { field: 'warehouseQuantity', headerName: 'Depoya Giren', type: 'number', minWidth: 140, flex: 0.6 },
      { field: 'status', headerName: 'Durum', minWidth: 160, flex: 0.7, valueFormatter: (value: string) => trStatus(value) },
    ],
    [],
  )

  const salesColumns = useMemo<GridColDef<SalesReportRow>[]>(
    () => [
      { field: 'salesOrderNumber', headerName: 'Sipariş No', minWidth: 160, flex: 0.8 },
      { field: 'customerName', headerName: 'Müşteri', minWidth: 220, flex: 1.2 },
      { field: 'orderDate', headerName: 'Tarih', minWidth: 130, flex: 0.6, valueFormatter: (value: string) => formatDate(value) },
      { field: 'quantity', headerName: 'Adet', type: 'number', minWidth: 110, flex: 0.5 },
      { field: 'totalAmount', headerName: 'Tutar', type: 'number', minWidth: 140, flex: 0.7, valueFormatter: (value: number) => formatCurrency(value) },
      { field: 'status', headerName: 'Durum', minWidth: 130, flex: 0.6, valueFormatter: (value: string) => trStatus(value) },
    ],
    [],
  )

  const consistencyColumns = useMemo<GridColDef<StockConsistencyRow>[]>(
    () => [
      { field: 'area', headerName: 'Alan', minWidth: 140, flex: 0.8 },
      { field: 'incomingQuantity', headerName: 'Giriş', type: 'number', minWidth: 140, flex: 0.7, valueFormatter: (value: number) => formatNumber(value) },
      { field: 'outgoingQuantity', headerName: 'Çıkış', type: 'number', minWidth: 140, flex: 0.7, valueFormatter: (value: number) => formatNumber(value) },
      { field: 'currentStock', headerName: 'Mevcut Stok', type: 'number', minWidth: 150, flex: 0.7, valueFormatter: (value: number) => formatNumber(value) },
      { field: 'unit', headerName: 'Birim', minWidth: 100, flex: 0.5 },
      {
        field: 'status',
        headerName: 'Durum',
        minWidth: 140,
        flex: 0.7,
        renderCell: ({ value }) => <Chip label={String(value)} size="small" color={value === 'Tutarlı' ? 'success' : 'warning'} variant="outlined" />,
      },
    ],
    [],
  )

  const currentRows = {
    products: overview.products,
    inventory: overview.inventory,
    purchasing: overview.purchasing,
    production: overview.production,
    sales: overview.sales,
    consistency: overview.stockConsistency.map((row) => ({ id: row.area, ...row })),
  }[activeTab] as Record<string, unknown>[]

  const currentColumns = {
    products: productColumns,
    inventory: inventoryColumns,
    purchasing: purchasingColumns,
    production: productionColumns,
    sales: salesColumns,
    consistency: consistencyColumns,
  }[activeTab] as GridColDef[]

  const currentTitle = {
    products: 'Ürün Raporu',
    inventory: 'Stok Raporu',
    purchasing: 'Satın Alma Raporu',
    production: 'Üretim Raporu',
    sales: 'Satış Raporu',
    consistency: 'Stok Tutarlılığı',
  }[activeTab]

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>Raporlar</Typography>
          <Typography color="text.secondary">Ürün, stok, satın alma, üretim ve satış kayıtlarını tek ekranda kontrol edin.</Typography>
        </Box>
        <Button variant="outlined" startIcon={<DownloadOutlinedIcon />} onClick={() => exportRows(`${currentTitle}.csv`, currentRows)} disabled={currentRows.length === 0}>
          {commonText.export}
        </Button>
      </Stack>

      {error && <Alert severity="error" sx={{ whiteSpace: 'pre-line' }}>{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack direction={{ xs: 'column', lg: 'row' }} spacing={2}>
          <TextField value={filters.search} onChange={(event) => setFilters((current) => ({ ...current, search: event.target.value }))} placeholder="Belge no, ürün, müşteri veya tedarikçi ara" size="small" fullWidth slotProps={{ input: { startAdornment: <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment> } }} />
          <TextField select label="Durum" value={filters.status} onChange={(event) => setFilters((current) => ({ ...current, status: event.target.value }))} size="small" sx={{ minWidth: { xs: '100%', lg: 220 } }}>
            <MenuItem value="">Tüm Durumlar</MenuItem>
            {statusOptions.map((status) => <MenuItem key={status} value={status}>{trStatus(status)}</MenuItem>)}
          </TextField>
          <TextField label="Başlangıç" type="date" value={filters.dateFrom} onChange={(event) => setFilters((current) => ({ ...current, dateFrom: event.target.value }))} size="small" sx={{ minWidth: { xs: '100%', lg: 180 } }} slotProps={{ inputLabel: { shrink: true } }} />
          <TextField label="Bitiş" type="date" value={filters.dateTo} onChange={(event) => setFilters((current) => ({ ...current, dateTo: event.target.value }))} size="small" sx={{ minWidth: { xs: '100%', lg: 180 } }} slotProps={{ inputLabel: { shrink: true } }} />
        </Stack>
      </Paper>

      <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, minmax(0, 1fr))', xl: 'repeat(5, minmax(0, 1fr))' } }}>
        {[
          ['Ürün', overview.summary.productCount],
          ['Ürün Stoğu', `${formatNumber(overview.summary.productStockQuantity)} Adet`],
          ['Kumaş Stoğu', `${formatNumber(overview.summary.fabricStockKg)} Kg`],
          ['Satın Alma', formatCurrency(overview.summary.purchaseOrderAmount)],
          ['Satış', formatCurrency(overview.summary.salesAmount)],
        ].map(([label, value]) => (
          <Paper key={String(label)} variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
            <Typography variant="body2" color="text.secondary">{label}</Typography>
            <Typography variant="h6" sx={{ fontWeight: 800 }}>{loading ? <Skeleton width={120} /> : value}</Typography>
          </Paper>
        ))}
      </Box>

      <Paper variant="outlined" sx={{ borderRadius: 1, overflow: 'hidden' }}>
        <Tabs value={activeTab} onChange={(_, value: ReportTab) => setActiveTab(value)} variant="scrollable" scrollButtons="auto">
          <Tab label="Ürünler" value="products" />
          <Tab label="Stok" value="inventory" />
          <Tab label="Satın Alma" value="purchasing" />
          <Tab label="Üretim" value="production" />
          <Tab label="Satış" value="sales" />
          <Tab label="Tutarlılık" value="consistency" />
        </Tabs>
        <Box sx={{ width: '100%', minHeight: 560, p: 2 }}>
          <DataGrid
            rows={currentRows}
            columns={currentColumns}
            loading={loading}
            disableRowSelectionOnClick
            pageSizeOptions={[10, 25, 50, 100]}
            initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
            localeText={{ noRowsLabel: 'Bu filtrelere uygun rapor kaydı bulunamadı.' }}
            sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }}
          />
        </Box>
      </Paper>
    </Stack>
  )
}
