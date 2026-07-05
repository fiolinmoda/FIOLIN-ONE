import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import {
  Alert,
  Box,
  Button,
  IconButton,
  InputAdornment,
  Paper,
  Stack,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined'
import EditOutlinedIcon from '@mui/icons-material/EditOutlined'
import SearchIcon from '@mui/icons-material/Search'
import { deletePurchaseOrder, getPurchaseOrders } from './api'
import type { PurchaseOrder } from './types'

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value)
}

function formatDate(value: string | null): string {
  return value ? new Date(value).toLocaleDateString('tr-TR') : ''
}

export function PurchaseOrderListPage() {
  const navigate = useNavigate()
  const [orders, setOrders] = useState<PurchaseOrder[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const loadOrders = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getPurchaseOrders(search)
      setOrders(data.items)
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : 'Purchase orders could not be loaded.')
    } finally {
      setLoading(false)
    }
  }, [search])

  useEffect(() => {
    const handle = window.setTimeout(() => {
      void loadOrders()
    }, 250)

    return () => window.clearTimeout(handle)
  }, [loadOrders])

  const handleDelete = useCallback(
    async (order: PurchaseOrder) => {
      const confirmed = window.confirm(`Delete purchase order ${order.purchaseNumber}?`)

      if (!confirmed) {
        return
      }

      setError(null)

      try {
        await deletePurchaseOrder(order.id)
        await loadOrders()
      } catch (exception) {
        setError(exception instanceof Error ? exception.message : 'Purchase order could not be deleted.')
      }
    },
    [loadOrders],
  )

  const columns = useMemo<GridColDef<PurchaseOrder>[]>(
    () => [
      { field: 'purchaseNumber', headerName: 'Purchase No', minWidth: 150, flex: 0.8 },
      { field: 'supplierName', headerName: 'Supplier', minWidth: 220, flex: 1.2 },
      {
        field: 'orderDate',
        headerName: 'Order Date',
        minWidth: 130,
        flex: 0.6,
        valueFormatter: (value: string) => formatDate(value),
      },
      {
        field: 'expectedDate',
        headerName: 'Expected',
        minWidth: 130,
        flex: 0.6,
        valueFormatter: (value: string | null) => formatDate(value),
      },
      { field: 'status', headerName: 'Status', minWidth: 150, flex: 0.7 },
      {
        field: 'totalAmount',
        headerName: 'Total',
        type: 'number',
        minWidth: 140,
        flex: 0.6,
        valueFormatter: (value: number) => formatCurrency(value),
      },
      {
        field: 'actions',
        headerName: '',
        sortable: false,
        filterable: false,
        width: 112,
        align: 'right',
        renderCell: ({ row }) => (
          <Stack direction="row" spacing={0.5} sx={{ justifyContent: 'flex-end', width: '100%' }}>
            <Tooltip title="Edit purchase order">
              <IconButton size="small" onClick={() => navigate(`/purchasing/orders/${row.id}`)}>
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Delete purchase order">
              <IconButton size="small" color="error" onClick={() => void handleDelete(row)}>
                <DeleteOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          </Stack>
        ),
      },
    ],
    [handleDelete, navigate],
  )

  return (
    <Stack spacing={3}>
      <Stack
        direction={{ xs: 'column', md: 'row' }}
        spacing={2}
        sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}
      >
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>
            Purchase Orders
          </Typography>
          <Typography color="text.secondary">Track supplier purchase orders from draft to invoice.</Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate('/purchasing/orders/new')}>
          Add Purchase Order
        </Button>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search purchase orders"
            size="small"
            fullWidth
            slotProps={{
              input: {
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon fontSize="small" />
                  </InputAdornment>
                ),
              },
            }}
          />
          <Box sx={{ width: '100%', minHeight: 460 }}>
            <DataGrid
              rows={orders}
              columns={columns}
              loading={loading}
              disableRowSelectionOnClick
              pageSizeOptions={[10, 25, 50]}
              initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
              sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }}
            />
          </Box>
        </Stack>
      </Paper>
    </Stack>
  )
}
