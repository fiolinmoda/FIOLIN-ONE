import { useEffect, useState } from 'react'
import { Alert, Paper, Stack, Typography } from '@mui/material'
import { getProductionDashboard } from './api'
import type { ProductionDashboard } from './types'

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
      .catch((exception) => setError(exception instanceof Error ? exception.message : 'Dashboard could not be loaded.'))
  }, [])

  const cards = [
    ['Production Planned', dashboard.productionPlanned],
    ['In Cutting', dashboard.inCutting],
    ['At Workshop', dashboard.atWorkshop],
    ['Ironing & Packaging', dashboard.ironingPackaging],
    ['Completed', dashboard.completed],
  ]

  return (
    <Stack spacing={3}>
      <div>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>Production Dashboard</Typography>
        <Typography color="text.secondary">Live production order status overview.</Typography>
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
