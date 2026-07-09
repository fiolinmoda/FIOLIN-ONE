import { useCallback, useEffect, useMemo, useState } from 'react'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import { Alert, Box, Button, Chip, InputAdornment, Paper, Stack, TextField, Typography } from '@mui/material'
import FileDownloadOutlinedIcon from '@mui/icons-material/FileDownloadOutlined'
import SearchIcon from '@mui/icons-material/Search'
import { getFabrics } from './api'
import type { Fabric } from './types'
import { trStatus } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

function kg(value: number): string {
  return `${value.toLocaleString('tr-TR', { maximumFractionDigits: 2 })} Kg`
}

export function FabricStockPage() {
  const [fabrics, setFabrics] = useState<Fabric[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const loadStock = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getFabrics(search)
      setFabrics(data.items)
    } catch (exception) {
      setError(toUserMessage(exception, 'Kumaş stoku yüklenemedi.'))
    } finally {
      setLoading(false)
    }
  }, [search])

  useEffect(() => {
    const handle = window.setTimeout(() => {
      void loadStock()
    }, 250)

    return () => window.clearTimeout(handle)
  }, [loadStock])

  const columns = useMemo<GridColDef<Fabric>[]>(
    () => [
      { field: 'fabricCode', headerName: 'Kod', minWidth: 130, flex: 0.6 },
      { field: 'fabricName', headerName: 'Kumaş', minWidth: 220, flex: 1.1 },
      { field: 'supplierName', headerName: 'Tedarikçi', minWidth: 190, flex: 1 },
      { field: 'color', headerName: 'Renk', minWidth: 130, flex: 0.6 },
      { field: 'currentStockKg', headerName: 'Mevcut', type: 'number', minWidth: 130, valueFormatter: (value: number) => kg(value) },
      { field: 'reservedQuantityKg', headerName: 'Rezerve', type: 'number', minWidth: 130, valueFormatter: (value: number) => kg(value) },
      { field: 'availableStockKg', headerName: 'Kullanılabilir', type: 'number', minWidth: 130, valueFormatter: (value: number) => kg(value) },
      { field: 'minimumStock', headerName: 'Minimum', type: 'number', minWidth: 130, valueFormatter: (value: number) => kg(value) },
      {
        field: 'status',
        headerName: 'Durum',
        minWidth: 140,
        renderCell: ({ value }) => <Chip label={trStatus(String(value))} size="small" color={value === 'OUT OF STOCK' ? 'error' : 'success'} variant="outlined" />,
      },
    ],
    [],
  )

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>Kumaş Stok</Typography>
          <Typography color="text.secondary">Mevcut, rezerve ve kullanılabilir kumaş miktarları.</Typography>
        </Box>
        <Button variant="outlined" startIcon={<FileDownloadOutlinedIcon />}>Dışa Aktar</Button>
      </Stack>
      {error && <Alert severity="error">{error}</Alert>}
      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Kumaş stoku ara" size="small" fullWidth slotProps={{ input: { startAdornment: <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment> } }} />
          <Box sx={{ width: '100%', minHeight: 500 }}>
            <DataGrid
              rows={fabrics}
              columns={columns}
              loading={loading}
              disableRowSelectionOnClick
              pageSizeOptions={[10, 25, 50]}
              initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
              localeText={{ noRowsLabel: search.trim() ? 'Aramanıza uygun kumaş stoğu bulunamadı.' : 'Henüz kumaş stoğu yok.' }}
              sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }}
            />
          </Box>
        </Stack>
      </Paper>
    </Stack>
  )
}
