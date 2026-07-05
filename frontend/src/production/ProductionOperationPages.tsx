import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { Alert, Box, Button, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material'
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined'
import { getFabrics } from '../fabric/api'
import type { Fabric } from '../fabric/types'
import { createCutting, createWarehouseEntry, createWorkshopReturn, createWorkshopShipment, getProductionOrders } from './api'
import type { ProductionOrder } from './types'

type OperationKind = 'cutting' | 'shipment' | 'return' | 'warehouse'

type ProductionOperationPageProps = {
  kind: OperationKind
}

const titles: Record<OperationKind, string> = {
  cutting: 'Cutting',
  shipment: 'Workshop Shipment',
  return: 'Workshop Return',
  warehouse: 'Warehouse Entry',
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
    void getProductionOrders().then((data) => setOrders(data.items)).catch(() => setError('Production orders could not be loaded.'))
    if (kind === 'cutting') {
      void getFabrics().then((data) => setFabrics(data.items)).catch(() => setError('Fabrics could not be loaded.'))
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
      setError(exception instanceof Error ? exception.message : `${titles[kind]} could not be saved.`)
    } finally {
      setSaving(false)
    }
  }

  return (
    <Stack spacing={3}>
      <Box>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>{titles[kind]}</Typography>
        <Typography color="text.secondary">Record production operation details.</Typography>
      </Box>
      {error && <Alert severity="error">{error}</Alert>}
      <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 1 }}>
        <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
          <Stack spacing={3}>
            <TextField select label="Production Order" value={input.productionOrderId} onChange={(event) => update('productionOrderId', event.target.value)} required fullWidth>
              {orders.map((order) => <MenuItem key={order.id} value={order.id}>{order.productionNumber} - {order.productName}</MenuItem>)}
            </TextField>

            {kind === 'cutting' && (
              <>
                <TextField select label="Fabric" value={input.fabricId} onChange={(event) => update('fabricId', event.target.value)} required fullWidth>
                  {fabrics.map((fabric) => <MenuItem key={fabric.id} value={fabric.id}>{fabric.fabricCode} - {fabric.fabricName} ({fabric.currentStockKg} Kg)</MenuItem>)}
                </TextField>
                <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                  <TextField label="Consumed Weight Kg" type="number" value={input.consumedWeightKg} onChange={(event) => update('consumedWeightKg', Number(event.target.value))} required fullWidth />
                  <TextField label="Waste Weight Kg" type="number" value={input.wasteWeightKg} onChange={(event) => update('wasteWeightKg', Number(event.target.value))} fullWidth />
                </Stack>
                <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                  <TextField label="Date" type="date" value={input.cuttingDate} onChange={(event) => update('cuttingDate', event.target.value)} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
                  <TextField label="Operator" value={input.operatorName} onChange={(event) => update('operatorName', event.target.value)} fullWidth />
                </Stack>
              </>
            )}

            {kind === 'shipment' && (
              <>
                <TextField label="Workshop" value={input.workshop} onChange={(event) => update('workshop', event.target.value)} required fullWidth />
                <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                  <TextField label="Shipment Date" type="date" value={input.shipmentDate} onChange={(event) => update('shipmentDate', event.target.value)} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
                  <TextField label="Expected Return Date" type="date" value={input.expectedReturnDate ?? ''} onChange={(event) => update('expectedReturnDate', event.target.value || null)} fullWidth slotProps={{ inputLabel: { shrink: true } }} />
                </Stack>
                <TextField label="Sent Quantity" type="number" value={input.sentQuantity} onChange={(event) => update('sentQuantity', Number(event.target.value))} required fullWidth />
              </>
            )}

            {kind === 'return' && (
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Returned Quantity" type="number" value={input.returnedQuantity} onChange={(event) => update('returnedQuantity', Number(event.target.value))} fullWidth />
                <TextField label="Extra Quantity" type="number" value={input.extraQuantity} onChange={(event) => update('extraQuantity', Number(event.target.value))} fullWidth />
                <TextField label="Missing Quantity" type="number" value={input.missingQuantity} onChange={(event) => update('missingQuantity', Number(event.target.value))} fullWidth />
              </Stack>
            )}

            {kind === 'warehouse' && (
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Actual Quantity" type="number" value={input.actualQuantity} onChange={(event) => update('actualQuantity', Number(event.target.value))} required fullWidth />
                <TextField label="Warehouse Date" type="date" value={input.warehouseDate} onChange={(event) => update('warehouseDate', event.target.value)} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
              </Stack>
            )}

            {kind === 'return' && <TextField label="Return Date" type="date" value={input.returnDate} onChange={(event) => update('returnDate', event.target.value)} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />}
            <TextField label="Notes" value={input.notes} onChange={(event) => update('notes', event.target.value)} multiline minRows={3} fullWidth />
            <Stack direction="row" spacing={2} sx={{ justifyContent: 'flex-end' }}>
              <Button onClick={() => navigate('/production/orders')}>Cancel</Button>
              <Button type="submit" variant="contained" startIcon={<SaveOutlinedIcon />} disabled={saving}>Save</Button>
            </Stack>
          </Stack>
        </Box>
      </Paper>
    </Stack>
  )
}
