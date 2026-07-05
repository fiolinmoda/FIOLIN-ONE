import { useEffect, useState } from 'react'
import { Alert, Paper, Stack, Typography } from '@mui/material'
import { getProductionDashboard } from './api'
import type { ProductionDashboard } from './types'
import { toUserMessage } from '../common/apiClient'

const emptyDashboard: ProductionDashboard = {
  productionPlanned: 0,
  inCutting: 0,
  atWorkshop: 0,
  ironingPackaging: 0,
  completed: 0,
}

export function ProductionDashboardPage() {
  const [dashboard, setDashboard] = useState<ProductionDashboard>(emptyDashboard)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    void getProductionDashboard()
      .then(setDashboard)
      .catch((exception) => setError(toUserMessage(exception, 'Üretim ana sayfası yüklenemedi.')))
  }, [])

  const cards = [
    ['Planlanan Üretim', dashboard.productionPlanned],
    ['Kesimde', dashboard.inCutting],
    ['Atölyede', dashboard.atWorkshop],
    ['Ütü / Paket', dashboard.ironingPackaging],
    ['Tamamlanan', dashboard.completed],
  ]

  return (
    <Stack spacing={3}>
      <div>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>Üretim Ana Sayfa</Typography>
        <Typography color="text.secondary">Üretim emirlerinin güncel durum özeti.</Typography>
      </div>
      {error && <Alert severity="error">{error}</Alert>}
      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
        {cards.map(([label, value]) => (
          <Paper key={label} variant="outlined" sx={{ p: 2, borderRadius: 1, flex: 1, minWidth: 160 }}>
            <Typography color="text.secondary">{label}</Typography>
            <Typography variant="h4" sx={{ fontWeight: 800 }}>{value}</Typography>
          </Paper>
        ))}
      </Stack>
    </Stack>
  )
}
