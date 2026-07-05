import { useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import { Alert, Box, Button, Paper, Stack, Typography } from '@mui/material'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import { getProductionTimeline } from './api'
import type { ProductionTimeline } from './types'
import { commonText } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

export function ProductionTimelinePage() {
  const navigate = useNavigate()
  const { id } = useParams()
  const [timeline, setTimeline] = useState<ProductionTimeline[]>([])
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!id) return
    void getProductionTimeline(id).then(setTimeline).catch((exception) => setError(toUserMessage(exception, 'Zaman çizelgesi yüklenemedi.')))
  }, [id])

  const columns = useMemo<GridColDef<ProductionTimeline>[]>(() => [
    { field: 'eventType', headerName: 'Olay', minWidth: 190, flex: 0.8 },
    { field: 'description', headerName: 'Açıklama', minWidth: 320, flex: 1.5 },
    { field: 'eventDate', headerName: 'Tarih', minWidth: 170, flex: 0.7, valueFormatter: (value: string) => new Date(value).toLocaleString('tr-TR') },
  ], [])

  return (
    <Stack spacing={3}>
      <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate(`/production/orders/${id}`)}>{commonText.back}</Button>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>Üretim Zaman Çizelgesi</Typography>
          <Typography color="text.secondary">Bu üretim emri için önemli olaylar.</Typography>
        </Box>
      </Stack>
      {error && <Alert severity="error">{error}</Alert>}
      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Box sx={{ width: '100%', minHeight: 440 }}><DataGrid rows={timeline} columns={columns} disableRowSelectionOnClick pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }} /></Box>
      </Paper>
    </Stack>
  )
}
