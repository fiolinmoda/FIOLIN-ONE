import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import { Alert, Box, Button, IconButton, InputAdornment, MenuItem, Paper, Snackbar, Stack, TextField, Tooltip, Typography } from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined'
import EditOutlinedIcon from '@mui/icons-material/EditOutlined'
import SearchIcon from '@mui/icons-material/Search'
import { deleteSalesOrder, getSalesOrders } from './api'
import type { SalesOrder } from './types'
import { confirmDelete, trStatus } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

const statuses = ['Draft', 'Approved', 'Completed', 'Cancelled']

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString('tr-TR')
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value)
}

export function SalesOrderListPage() {
  const navigate = useNavigate()
  const [orders, setOrders] = useState<SalesOrder[]>([])
  const [search, setSearch] = useState('')
  const [status, setStatus] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  const loadOrders = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getSalesOrders(search, status)
      setOrders(data.items)
    } catch (exception) {
      setError(toUserMessage(exception, 'Satış siparişleri yüklenemedi.'))
    } finally {
      setLoading(false)
    }
  }, [search, status])

  useEffect(() => {
    const handle = window.setTimeout(() => {
      void loadOrders()
    }, 250)

    return () => window.clearTimeout(handle)
  }, [loadOrders])

  const handleDelete = useCallback(
    async (order: SalesOrder) => {
      if (!confirmDelete(order.salesOrderNumber)) {
        return
      }

      setError(null)

      try {
        await deleteSalesOrder(order.id)
        setSuccess('Satış siparişi silindi.')
        await loadOrders()
      } catch (exception) {
        setError(toUserMessage(exception, 'Satış siparişi silinemedi.'))
      }
    },
    [loadOrders],
  )

  const columns = useMemo<GridColDef<SalesOrder>[]>(
    () => [
      { field: 'salesOrderNumber', headerName: 'Sipariş No', minWidth: 160, flex: 0.8 },
      { field: 'customerName', headerName: 'Müşteri', minWidth: 220, flex: 1.2 },
      { field: 'orderDate', headerName: 'Sipariş Tarihi', minWidth: 140, flex: 0.7, valueFormatter: (value: string) => formatDate(value) },
      { field: 'status', headerName: 'Durum', minWidth: 140, flex: 0.7, valueFormatter: (value: string) => trStatus(value) },
      { field: 'totalAmount', headerName: 'Toplam', type: 'number', minWidth: 140, flex: 0.7, valueFormatter: (value: number) => formatCurrency(value) },
      {
        field: 'actions',
        headerName: '',
        sortable: false,
        filterable: false,
        width: 112,
        align: 'right',
        renderCell: ({ row }) => (
          <Stack direction="row" spacing={0.5} sx={{ justifyContent: 'flex-end', width: '100%' }}>
            <Tooltip title="Satış siparişini düzenle">
              <IconButton size="small" onClick={() => navigate(`/sales/orders/${row.id}`)}>
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Satış siparişini sil">
              <span>
                <IconButton size="small" color="error" onClick={() => void handleDelete(row)} disabled={row.status === 'Completed'}>
                  <DeleteOutlinedIcon fontSize="small" />
                </IconButton>
              </span>
            </Tooltip>
          </Stack>
        ),
      },
    ],
    [handleDelete, navigate],
  )

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>Satış Siparişleri</Typography>
          <Typography color="text.secondary">Müşteri siparişlerini stok uygunluğu ve toplam tutarlarıyla takip edin.</Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate('/sales/orders/new')}>Sipariş Ekle</Button>
      </Stack>

      {error && <Alert severity="error" sx={{ whiteSpace: 'pre-line' }}>{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
            <TextField value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Sipariş no, müşteri, ürün veya barkod ara" size="small" fullWidth slotProps={{ input: { startAdornment: <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment> } }} />
            <TextField select label="Durum" value={status} onChange={(event) => setStatus(event.target.value)} size="small" sx={{ minWidth: { xs: '100%', md: 220 } }}>
              <MenuItem value="">Tüm Durumlar</MenuItem>
              {statuses.map((statusOption) => <MenuItem key={statusOption} value={statusOption}>{trStatus(statusOption)}</MenuItem>)}
            </TextField>
          </Stack>
          <Box sx={{ width: '100%', minHeight: 500 }}>
            <DataGrid rows={orders} columns={columns} loading={loading} disableRowSelectionOnClick pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} localeText={{ noRowsLabel: search.trim() ? 'Aramanıza uygun satış siparişi bulunamadı.' : 'Henüz satış siparişi yok.' }} sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }} />
          </Box>
        </Stack>
      </Paper>

      <Snackbar open={!!success} autoHideDuration={3000} onClose={() => setSuccess(null)} message={success} />
    </Stack>
  )
}
