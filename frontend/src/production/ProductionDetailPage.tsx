import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { Alert, Box, Button, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined'
import TimelineOutlinedIcon from '@mui/icons-material/TimelineOutlined'
import { getProducts } from '../products/api'
import type { Product, ProductVariant } from '../products/types'
import { getProductVariants } from '../products/api'
import { createProductionOrder, getProductionOrder, sendToIroningPackaging, updateProductionOrder } from './api'
import type { ProductionOrderInput } from './types'

const statuses = ['PLANNED', 'FABRIC_ALLOCATED', 'CUTTING', 'AT_WORKSHOP', 'AT_IRONING_PACKAGING', 'READY_FOR_WAREHOUSE', 'COMPLETED', 'CANCELLED']
const reasons = ['Stock', 'Dealer', 'Trendyol', 'Custom']

const emptyOrder: ProductionOrderInput = {
  productionNumber: '',
  productId: '',
  plannedQuantity: 1,
  productionReason: 'Stock',
  notes: '',
  status: 'PLANNED',
  items: [{ productVariantId: '', plannedQuantity: 1 }],
}

export function ProductionDetailPage() {
  const navigate = useNavigate()
  const { id } = useParams()
  const isNew = id === 'new'
  const [order, setOrder] = useState<ProductionOrderInput>(emptyOrder)
  const [products, setProducts] = useState<Product[]>([])
  const [variants, setVariants] = useState<ProductVariant[]>([])
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    void getProducts('').then(setProducts).catch(() => setError('Products could not be loaded.'))
  }, [])

  useEffect(() => {
    if (!order.productId) return
    void getProductVariants(order.productId).then(setVariants).catch(() => setError('Product variants could not be loaded.'))
  }, [order.productId])

  useEffect(() => {
    if (isNew || !id) return
    void getProductionOrder(id)
      .then((data) => {
        setOrder({
          productionNumber: data.productionNumber,
          productId: data.productId,
          plannedQuantity: data.plannedQuantity,
          productionReason: data.productionReason,
          notes: data.notes ?? '',
          status: data.status,
          items: data.items.length > 0 ? data.items.map((item) => ({ productVariantId: item.productVariantId, plannedQuantity: item.plannedQuantity })) : emptyOrder.items,
        })
      })
      .catch((exception) => setError(exception instanceof Error ? exception.message : 'Production order could not be loaded.'))
  }, [id, isNew])

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)
    try {
      if (isNew) {
        await createProductionOrder(order)
      } else if (id) {
        await updateProductionOrder(id, order)
      }
      navigate('/production/orders')
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : 'Production order could not be saved.')
    } finally {
      setSaving(false)
    }
  }

  async function handleIroningPackaging() {
    if (!id || isNew) return
    await sendToIroningPackaging(id)
    navigate('/production/orders')
  }

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}>
        <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
          <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/production/orders')}>Back</Button>
          <Box>
            <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>{isNew ? 'Create Production' : 'Production Detail'}</Typography>
            <Typography color="text.secondary">Production order, product, variant distribution, and status.</Typography>
          </Box>
        </Stack>
        {!isNew && <Button startIcon={<TimelineOutlinedIcon />} onClick={() => navigate(`/production/orders/${id}/timeline`)}>Timeline</Button>}
      </Stack>
      {error && <Alert severity="error">{error}</Alert>}
      <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 1 }}>
        <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
          <Stack spacing={3}>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField label="Production Number" value={order.productionNumber} onChange={(event) => setOrder((current) => ({ ...current, productionNumber: event.target.value }))} required fullWidth />
              <TextField select label="Product" value={order.productId} onChange={(event) => setOrder((current) => ({ ...current, productId: event.target.value, items: [{ productVariantId: '', plannedQuantity: current.plannedQuantity }] }))} required fullWidth>
                {products.map((product) => <MenuItem key={product.id} value={product.id}>{product.productCode} - {product.productName}</MenuItem>)}
              </TextField>
            </Stack>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField label="Planned Quantity" type="number" value={order.plannedQuantity} onChange={(event) => setOrder((current) => ({ ...current, plannedQuantity: Number(event.target.value), items: [{ ...current.items[0], plannedQuantity: Number(event.target.value) }] }))} required fullWidth />
              <TextField select label="Production Reason" value={order.productionReason} onChange={(event) => setOrder((current) => ({ ...current, productionReason: event.target.value }))} required fullWidth>{reasons.map((reason) => <MenuItem key={reason} value={reason}>{reason}</MenuItem>)}</TextField>
              <TextField select label="Status" value={order.status} onChange={(event) => setOrder((current) => ({ ...current, status: event.target.value }))} required fullWidth>{statuses.map((status) => <MenuItem key={status} value={status}>{status}</MenuItem>)}</TextField>
            </Stack>
            <TextField select label="Variant (Color / Size)" value={order.items[0]?.productVariantId ?? ''} onChange={(event) => setOrder((current) => ({ ...current, items: [{ ...current.items[0], productVariantId: event.target.value }] }))} required fullWidth>
              {variants.map((variant) => <MenuItem key={variant.id} value={variant.id}>{variant.color} / {variant.size}</MenuItem>)}
            </TextField>
            <TextField label="Notes" value={order.notes} onChange={(event) => setOrder((current) => ({ ...current, notes: event.target.value }))} multiline minRows={3} fullWidth />
            <Stack direction="row" spacing={2} sx={{ justifyContent: 'flex-end' }}>
              {!isNew && <Button onClick={() => void handleIroningPackaging()}>Send to Ironing & Packaging</Button>}
              <Button onClick={() => navigate('/production/orders')}>Cancel</Button>
              <Button type="submit" variant="contained" startIcon={<SaveOutlinedIcon />} disabled={saving}>Save</Button>
            </Stack>
          </Stack>
        </Box>
      </Paper>
    </Stack>
  )
}
