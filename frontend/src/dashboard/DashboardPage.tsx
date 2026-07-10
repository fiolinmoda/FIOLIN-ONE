import { useEffect, useMemo, useState } from 'react'
import type { ReactNode } from 'react'
import { useNavigate } from 'react-router-dom'
import { Alert, Box, ButtonBase, Chip, Divider, Paper, Skeleton, Stack, Typography } from '@mui/material'
import AssignmentOutlinedIcon from '@mui/icons-material/AssignmentOutlined'
import FactoryOutlinedIcon from '@mui/icons-material/FactoryOutlined'
import Inventory2OutlinedIcon from '@mui/icons-material/Inventory2Outlined'
import ReceiptLongOutlinedIcon from '@mui/icons-material/ReceiptLongOutlined'
import StorefrontOutlinedIcon from '@mui/icons-material/StorefrontOutlined'
import WarningAmberOutlinedIcon from '@mui/icons-material/WarningAmberOutlined'
import { getDashboardOverview } from './api'
import type { DashboardMetric, DashboardOverview, RecentDocument } from './types'
import { toUserMessage } from '../common/apiClient'
import { trStatus } from '../common/uiText'

const emptyDashboard: DashboardOverview = {
  todaySales: { title: 'Bugünkü Satış', value: 0, unit: 'TL', link: '/sales' },
  totalOrders: { title: 'Toplam Sipariş', value: 0, unit: 'Adet', link: '/sales' },
  openProductionOrders: { title: 'Açık Üretim Emri', value: 0, unit: 'Adet', link: '/production/orders' },
  currentInventory: { title: 'Mevcut Stok', value: 0, unit: 'Adet/Kg', link: '/warehouse/product-stock' },
  criticalProducts: [],
  recentPurchasing: [],
  recentProduction: [],
  recentSales: [],
}

function formatNumber(value: number): string {
  return new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 2 }).format(value)
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value)
}

function formatMetric(metric: DashboardMetric): string {
  return metric.unit === 'TL' ? formatCurrency(metric.value) : formatNumber(metric.value)
}

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString('tr-TR')
}

export function DashboardPage() {
  const navigate = useNavigate()
  const [dashboard, setDashboard] = useState<DashboardOverview>(emptyDashboard)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let isMounted = true

    async function loadDashboard() {
      setLoading(true)
      setError(null)

      try {
        const data = await getDashboardOverview()
        if (isMounted) {
          setDashboard(data)
        }
      } catch (exception) {
        if (isMounted) {
          setError(toUserMessage(exception, 'Ana panel yüklenemedi.'))
        }
      } finally {
        if (isMounted) {
          setLoading(false)
        }
      }
    }

    void loadDashboard()

    return () => {
      isMounted = false
    }
  }, [])

  const metricCards = useMemo(
    () => [
      { metric: dashboard.todaySales, icon: <StorefrontOutlinedIcon color="primary" /> },
      { metric: dashboard.totalOrders, icon: <AssignmentOutlinedIcon color="primary" /> },
      { metric: dashboard.openProductionOrders, icon: <FactoryOutlinedIcon color="primary" /> },
      { metric: dashboard.currentInventory, icon: <Inventory2OutlinedIcon color="primary" /> },
    ],
    [dashboard],
  )

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>Ana Panel</Typography>
          <Typography color="text.secondary">Güne başlarken satış, üretim, stok ve son işlemleri tek ekranda görün.</Typography>
        </Box>
        <Chip label="Canlı veritabanı verisi" color="success" variant="outlined" />
      </Stack>

      {error && <Alert severity="error" sx={{ whiteSpace: 'pre-line' }}>{error}</Alert>}

      <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, minmax(0, 1fr))', xl: 'repeat(4, minmax(0, 1fr))' } }}>
        {metricCards.map(({ metric, icon }) => (
          <ButtonBase key={metric.title} onClick={() => navigate(metric.link)} sx={{ display: 'block', textAlign: 'left', borderRadius: 1 }}>
            <Paper variant="outlined" sx={{ p: 2.5, borderRadius: 1, minHeight: 142, width: '100%' }}>
              <Stack spacing={2}>
                <Stack direction="row" sx={{ alignItems: 'center', justifyContent: 'space-between' }}>
                  <Typography variant="body2" color="text.secondary">{metric.title}</Typography>
                  {icon}
                </Stack>
                <Box>
                  <Typography variant="h4" sx={{ fontWeight: 800 }}>
                    {loading ? <Skeleton width={140} /> : formatMetric(metric)}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">{metric.unit}</Typography>
                </Box>
              </Stack>
            </Paper>
          </ButtonBase>
        ))}
      </Box>

      <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', lg: '1.1fr 1fr' } }}>
        <Paper variant="outlined" sx={{ p: 2.5, borderRadius: 1 }}>
          <Stack spacing={2}>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <WarningAmberOutlinedIcon color="warning" />
              <Typography variant="h6" sx={{ fontWeight: 800 }}>Kritik Ürünler</Typography>
            </Stack>
            <Divider />
            {loading ? (
              <Stack spacing={1.5}>{Array.from({ length: 4 }).map((_, index) => <Skeleton key={index} height={42} />)}</Stack>
            ) : dashboard.criticalProducts.length === 0 ? (
              <Typography color="text.secondary">Kritik ürün stoğu bulunmuyor.</Typography>
            ) : (
              <Stack spacing={1}>
                {dashboard.criticalProducts.map((item) => (
                  <ButtonBase key={item.variantId} onClick={() => navigate(item.link)} sx={{ display: 'block', textAlign: 'left', borderRadius: 1 }}>
                    <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5} sx={{ p: 1.25, alignItems: { sm: 'center' }, justifyContent: 'space-between', borderRadius: 1, bgcolor: 'action.hover' }}>
                      <Box>
                        <Typography sx={{ fontWeight: 700 }}>{item.productCode} - {item.productName}</Typography>
                        <Typography variant="body2" color="text.secondary">{item.color} / {item.size}</Typography>
                      </Box>
                      <Chip label={`${item.stock} Adet`} color={item.stock === 0 ? 'error' : 'warning'} size="small" />
                    </Stack>
                  </ButtonBase>
                ))}
              </Stack>
            )}
          </Stack>
        </Paper>

        <RecentDocumentsCard
          title="Son Satın Alma"
          icon={<ReceiptLongOutlinedIcon color="primary" />}
          rows={dashboard.recentPurchasing}
          loading={loading}
          emptyText="Son satın alma kaydı bulunmuyor."
          onOpen={(link) => navigate(link)}
        />
      </Box>

      <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', lg: 'repeat(2, minmax(0, 1fr))' } }}>
        <RecentDocumentsCard
          title="Son Üretim"
          icon={<FactoryOutlinedIcon color="primary" />}
          rows={dashboard.recentProduction}
          loading={loading}
          emptyText="Son üretim kaydı bulunmuyor."
          onOpen={(link) => navigate(link)}
        />
        <RecentDocumentsCard
          title="Son Satış"
          icon={<StorefrontOutlinedIcon color="primary" />}
          rows={dashboard.recentSales}
          loading={loading}
          emptyText="Son satış kaydı bulunmuyor."
          onOpen={(link) => navigate(link)}
        />
      </Box>
    </Stack>
  )
}

function RecentDocumentsCard({
  title,
  icon,
  rows,
  loading,
  emptyText,
  onOpen,
}: {
  title: string
  icon: ReactNode
  rows: RecentDocument[]
  loading: boolean
  emptyText: string
  onOpen: (link: string) => void
}) {
  return (
    <Paper variant="outlined" sx={{ p: 2.5, borderRadius: 1 }}>
      <Stack spacing={2}>
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
          {icon}
          <Typography variant="h6" sx={{ fontWeight: 800 }}>{title}</Typography>
        </Stack>
        <Divider />
        {loading ? (
          <Stack spacing={1.5}>{Array.from({ length: 5 }).map((_, index) => <Skeleton key={index} height={42} />)}</Stack>
        ) : rows.length === 0 ? (
          <Typography color="text.secondary">{emptyText}</Typography>
        ) : (
          <Stack spacing={1}>
            {rows.map((row) => (
              <ButtonBase key={row.id} onClick={() => onOpen(row.link)} sx={{ display: 'block', textAlign: 'left', borderRadius: 1 }}>
                <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5} sx={{ p: 1.25, alignItems: { sm: 'center' }, justifyContent: 'space-between', borderRadius: 1, bgcolor: 'action.hover' }}>
                  <Box sx={{ minWidth: 0 }}>
                    <Typography sx={{ fontWeight: 700 }}>{row.documentNumber}</Typography>
                    <Typography variant="body2" color="text.secondary" noWrap>{row.title}</Typography>
                  </Box>
                  <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap', justifyContent: { xs: 'flex-start', sm: 'flex-end' } }}>
                    <Chip label={trStatus(row.status)} size="small" variant="outlined" />
                    <Typography variant="body2" color="text.secondary">{formatDate(row.date)}</Typography>
                    {row.amount > 0 && <Typography variant="body2" sx={{ fontWeight: 700 }}>{formatCurrency(row.amount)}</Typography>}
                  </Stack>
                </Stack>
              </ButtonBase>
            ))}
          </Stack>
        )}
      </Stack>
    </Paper>
  )
}
