import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import {
  Alert,
  Box,
  Button,
  Chip,
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
import FileDownloadOutlinedIcon from '@mui/icons-material/FileDownloadOutlined'
import SearchIcon from '@mui/icons-material/Search'
import { deleteFabric, getFabrics } from './api'
import type { Fabric } from './types'

function formatKg(value: number): string {
  return `${value.toLocaleString('tr-TR', { maximumFractionDigits: 2 })} Kg`
}

export function FabricListPage() {
  const navigate = useNavigate()
  const [fabrics, setFabrics] = useState<Fabric[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const loadFabrics = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getFabrics(search)
      setFabrics(data.items)
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : 'Fabrics could not be loaded.')
    } finally {
      setLoading(false)
    }
  }, [search])

  useEffect(() => {
    const handle = window.setTimeout(() => {
      void loadFabrics()
    }, 250)

    return () => window.clearTimeout(handle)
  }, [loadFabrics])

  const handleDelete = useCallback(
    async (fabric: Fabric) => {
      const confirmed = window.confirm(`Delete ${fabric.fabricCode} - ${fabric.fabricName}?`)

      if (!confirmed) {
        return
      }

      setError(null)

      try {
        await deleteFabric(fabric.id)
        await loadFabrics()
      } catch (exception) {
        setError(exception instanceof Error ? exception.message : 'Fabric could not be deleted.')
      }
    },
    [loadFabrics],
  )

  const columns = useMemo<GridColDef<Fabric>[]>(
    () => [
      { field: 'fabricCode', headerName: 'Code', minWidth: 130, flex: 0.6 },
      { field: 'fabricName', headerName: 'Fabric', minWidth: 220, flex: 1.1 },
      { field: 'supplierName', headerName: 'Supplier', minWidth: 190, flex: 1 },
      { field: 'color', headerName: 'Color', minWidth: 130, flex: 0.6 },
      {
        field: 'currentStockKg',
        headerName: 'Stock',
        type: 'number',
        minWidth: 130,
        flex: 0.5,
        valueFormatter: (value: number) => formatKg(value),
      },
      {
        field: 'reservedQuantityKg',
        headerName: 'Reserved',
        type: 'number',
        minWidth: 130,
        flex: 0.5,
        valueFormatter: (value: number) => formatKg(value),
      },
      {
        field: 'status',
        headerName: 'Status',
        minWidth: 140,
        flex: 0.6,
        renderCell: ({ value }) => (
          <Chip
            label={String(value)}
            size="small"
            color={value === 'OUT OF STOCK' ? 'error' : 'success'}
            variant="outlined"
          />
        ),
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
            <Tooltip title="Edit fabric">
              <IconButton size="small" onClick={() => navigate(`/fabric/fabrics/${row.id}`)}>
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Delete fabric">
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
      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>
            Fabric List
          </Typography>
          <Typography color="text.secondary">Manage fabric cards and current weight-based stock.</Typography>
        </Box>
        <Stack direction="row" spacing={1}>
          <Button variant="outlined" startIcon={<FileDownloadOutlinedIcon />}>Export</Button>
          <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate('/fabric/fabrics/new')}>
            Add Fabric
          </Button>
        </Stack>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search fabrics" size="small" fullWidth slotProps={{ input: { startAdornment: <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment> } }} />
          <Box sx={{ width: '100%', minHeight: 500 }}>
            <DataGrid rows={fabrics} columns={columns} loading={loading} disableRowSelectionOnClick pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }} />
          </Box>
        </Stack>
      </Paper>
    </Stack>
  )
}
