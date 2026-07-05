import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import { Alert, Box, Button, IconButton, InputAdornment, Paper, Stack, TextField, Tooltip, Typography } from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined'
import EditOutlinedIcon from '@mui/icons-material/EditOutlined'
import SearchIcon from '@mui/icons-material/Search'
import { deleteProductionOrder, getProductionOrders } from './api'
import type { ProductionOrder } from './types'

export function ProductionListPage() {
  const navigate = useNavigate()
  const [orders, setOrders] = useState<ProductionOrder[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const loadOrders = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const data = await getProductionOrders(search)
      setOrders(data.items)
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : 'Production orders could not be loaded.')
    } finally {
      setLoading(false)
    }
  }, [search])

  useEffect(() => {
    const handle = window.setTimeout(() => { void loadOrders() }, 250)
    return () => window.clearTimeout(handle)
  }, [loadOrders])

  const handleDelete = useCallback(async (order: ProductionOrder) => {
    if (!window.confirm(`Delete production order ${order.productionNumber}?`)) return
    await deleteProductionOrder(order.id)
    await loadOrders()
  }, [loadOrders])

  const columns = useMemo<GridColDef<ProductionOrder>[]>(() => [
    { field: 'productionNumber', headerName: 'Production No', minWidth: 160, flex: 0.8 },
    { field: 'productCode', headerName: 'Product Code', minWidth: 140, flex: 0.7 },
    { field: 'productName', headerName: 'Product', minWidth: 220, flex: 1.2 },
    { field: 'plannedQuantity', headerName: 'Planned', type: 'number', minWidth: 120, flex: 0.5 },
    { field: 'productionReason', headerName: 'Reason', minWidth: 130, flex: 0.6 },
    { field: 'status', headerName: 'Status', minWidth: 190, flex: 0.9 },
    {
      field: 'actions',
      headerName: '',
      sortable: false,
      filterable: false,
      width: 112,
      align: 'right',
      renderCell: ({ row }) => (
        <Stack direction="row" spacing={0.5} sx={{ justifyContent: 'flex-end', width: '100%' }}>
          <Tooltip title="Edit"><IconButton size="small" onClick={() => navigate(`/production/orders/${row.id}`)}><EditOutlinedIcon fontSize="small" /></IconButton></Tooltip>
          <Tooltip title="Delete"><IconButton size="small" color="error" onClick={() => void handleDelete(row)}><DeleteOutlinedIcon fontSize="small" /></IconButton></Tooltip>
        </Stack>
      ),
    },
  ], [handleDelete, navigate])

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>Production Orders</Typography>
          <Typography color="text.secondary">Manual production orders created by Vahap.</Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate('/production/orders/new')}>Create Production</Button>
      </Stack>
      {error && <Alert severity="error">{error}</Alert>}
      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search production orders" size="small" fullWidth slotProps={{ input: { startAdornment: <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment> } }} />
          <Box sx={{ width: '100%', minHeight: 500 }}><DataGrid rows={orders} columns={columns} loading={loading} disableRowSelectionOnClick pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }} /></Box>
        </Stack>
      </Paper>
    </Stack>
  )
}
