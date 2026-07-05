import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { Alert, Box, Button, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material'
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined'
import { getFabrics } from '../fabric/api'
import type { Fabric } from '../fabric/types'
import { createCutting, createWarehouseEntry, createWorkshopReturn, createWorkshopShipment, getProductionOrders } from './api'
import type { ProductionOrder } from './types'
import { commonText, requiredMessage } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

type OperationKind = 'cutting' | 'shipment' | 'return' | 'warehouse'

type ProductionOperationPageProps = {
  kind: OperationKind
}

const titles: Record<OperationKind, string> = {
  cutting: 'Kesim',
  shipment: 'Atölyeye Gönderim',
  return: 'Atölye Dönüşü',
  warehouse: 'Depo Girişi',
}

export function ProductionOperationPage({ kind }: ProductionOperationPageProps) {
  const navigate = useNavigate()
  const today = new Date().toISOString().slice(0, 10)
  const [orders, setOrders] = useState<ProductionOrder[]>([])
  const [fabrics, setFabrics] = useState<Fabric[]>([])
  const [error, setError] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)
  const [input, setInput] = useState<Record<string, string | number | null>>({
    productionOrderId: '',
    fabricId: '',
    consumedWeightKg: 1,
    wasteWeightKg: 0,
    cuttingDate: today,
    operatorName: '',
    workshop: '',
    shipmentDate: today,
    expectedReturnDate: null,
    sentQuantity: 1,
    status: 'SENT',
    returnedQuantity: 0,
    extraQuantity: 0,
    missingQuantity: 0,
    returnDate: today,
    actualQuantity: 0,
    warehouseDate: today,
    notes: '',
  })

  useEffect(() => {
    void getProductionOrders().then((data) => setOrders(data.items)).catch(() => setError('Üretim emirleri yüklenemedi.'))
    if (kind === 'cutting') {
      void getFabrics().then((data) => setFabrics(data.items)).catch(() => setError('Kumaşlar yüklenemedi.'))
    }
  }, [kind])

  function update(field: string, value: string | number | null) {
    setInput((current) => ({ ...current, [field]: value }))
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      if (kind === 'cutting') {
        await createCutting({
          productionOrderId: String(input.productionOrderId),
          fabricId: String(input.fabricId),
          consumedWeightKg: Number(input.consumedWeightKg),
          wasteWeightKg: Number(input.wasteWeightKg),
          cuttingDate: String(input.cuttingDate),
          operatorName: String(input.operatorName ?? ''),
          notes: String(input.notes ?? ''),
        })
      } else if (kind === 'shipment') {
        await createWorkshopShipment({
          productionOrderId: String(input.productionOrderId),
          workshop: String(input.workshop),
          shipmentDate: String(input.shipmentDate),
          expectedReturnDate: input.expectedReturnDate ? String(input.expectedReturnDate) : null,
          sentQuantity: Number(input.sentQuantity),
          notes: String(input.notes ?? ''),
          status: String(input.status),
        })
      } else if (kind === 'return') {
        await createWorkshopReturn({
          productionOrderId: String(input.productionOrderId),
          workshopShipmentId: null,
          returnedQuantity: Number(input.returnedQuantity),
          extraQuantity: Number(input.extraQuantity),
          missingQuantity: Number(input.missingQuantity),
          returnDate: String(input.returnDate),
          notes: String(input.notes ?? ''),
        })
      } else {
        await createWarehouseEntry({
          productionOrderId: String(input.productionOrderId),
          actualQuantity: Number(input.actualQuantity),
          warehouseDate: String(input.warehouseDate),
          notes: String(input.notes ?? ''),
        })
      }

      navigate('/production/orders')
    } catch (exception) {
      setError(toUserMessage(exception, `${titles[kind]} kaydedilemedi.`))
    } finally {
      setSaving(false)
    }
  }

  return (
    <Stack spacing={3}>
      <Box>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>{titles[kind]}</Typography>
        <Typography color="text.secondary">Üretim operasyon bilgisini kaydedin.</Typography>
      </Box>
      {error && <Alert severity="error">{error}</Alert>}
      <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 1 }}>
        <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
          <Stack spacing={3}>
            <TextField select label="Üretim Emri" value={input.productionOrderId} onChange={(event) => update('productionOrderId', event.target.value)} required helperText={!input.productionOrderId ? requiredMessage('Üretim emri') : ' '} fullWidth>
              {orders.map((order) => <MenuItem key={order.id} value={order.id}>{order.productionNumber} - {order.productName}</MenuItem>)}
            </TextField>

            {kind === 'cutting' && (
              <>
                <TextField select label="Kumaş" value={input.fabricId} onChange={(event) => update('fabricId', event.target.value)} required helperText={!input.fabricId ? requiredMessage('Kumaş') : ' '} fullWidth>
                  {fabrics.map((fabric) => <MenuItem key={fabric.id} value={fabric.id}>{fabric.fabricCode} - {fabric.fabricName} ({fabric.currentStockKg} Kg)</MenuItem>)}
                </TextField>
                <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                  <TextField label="Tüketilen Kg" type="number" value={input.consumedWeightKg} onChange={(event) => update('consumedWeightKg', Number(event.target.value))} required fullWidth />
                  <TextField label="Fire Kg" type="number" value={input.wasteWeightKg} onChange={(event) => update('wasteWeightKg', Number(event.target.value))} fullWidth />
                </Stack>
                <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                  <TextField label="Tarih" type="date" value={input.cuttingDate} onChange={(event) => update('cuttingDate', event.target.value)} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
                  <TextField label="Operatör" value={input.operatorName} onChange={(event) => update('operatorName', event.target.value)} fullWidth />
                </Stack>
              </>
            )}

            {kind === 'shipment' && (
              <>
                <TextField label="Atölye" value={input.workshop} onChange={(event) => update('workshop', event.target.value)} required helperText={!String(input.workshop).trim() ? requiredMessage('Atölye') : ' '} fullWidth />
                <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                  <TextField label="Gönderim Tarihi" type="date" value={input.shipmentDate} onChange={(event) => update('shipmentDate', event.target.value)} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
                  <TextField label="Beklenen Dönüş" type="date" value={input.expectedReturnDate ?? ''} onChange={(event) => update('expectedReturnDate', event.target.value || null)} fullWidth slotProps={{ inputLabel: { shrink: true } }} />
                </Stack>
                <TextField label="Gönderilen Miktar" type="number" value={input.sentQuantity} onChange={(event) => update('sentQuantity', Number(event.target.value))} required fullWidth />
              </>
            )}

            {kind === 'return' && (
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Dönen Miktar" type="number" value={input.returnedQuantity} onChange={(event) => update('returnedQuantity', Number(event.target.value))} fullWidth />
                <TextField label="Fazla Miktar" type="number" value={input.extraQuantity} onChange={(event) => update('extraQuantity', Number(event.target.value))} fullWidth />
                <TextField label="Eksik Miktar" type="number" value={input.missingQuantity} onChange={(event) => update('missingQuantity', Number(event.target.value))} fullWidth />
              </Stack>
            )}

            {kind === 'warehouse' && (
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Gerçek Miktar" type="number" value={input.actualQuantity} onChange={(event) => update('actualQuantity', Number(event.target.value))} required fullWidth />
                <TextField label="Depo Tarihi" type="date" value={input.warehouseDate} onChange={(event) => update('warehouseDate', event.target.value)} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
              </Stack>
            )}

            {kind === 'return' && <TextField label="Dönüş Tarihi" type="date" value={input.returnDate} onChange={(event) => update('returnDate', event.target.value)} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />}
            <TextField label="Notlar" value={input.notes} onChange={(event) => update('notes', event.target.value)} multiline minRows={3} fullWidth />
            <Stack direction="row" spacing={2} sx={{ justifyContent: 'flex-end' }}>
              <Button onClick={() => navigate('/production/orders')}>{commonText.cancel}</Button>
              <Button type="submit" variant="contained" startIcon={<SaveOutlinedIcon />} disabled={saving}>{commonText.save}</Button>
            </Stack>
          </Stack>
        </Box>
      </Paper>
    </Stack>
  )
}
