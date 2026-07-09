import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { Alert, Box, Button, IconButton, MenuItem, Paper, Snackbar, Stack, TextField, Typography } from '@mui/material'
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined'
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined'
import { createSalesOrder, getSalesOrder, updateSalesOrder } from './api'
import type { SalesOrderInput, SalesOrderItemInput } from './types'
import { getProductVariants, getProducts } from '../products/api'
import type { Product, ProductVariant } from '../products/types'
import { commonText, requiredMessage, trStatus } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

const statuses = ['Draft', 'Approved', 'Completed', 'Cancelled']

type VariantOption = ProductVariant & {
  productCode: string
  productName: string
}

const emptyOrder: SalesOrderInput = {
  salesOrderNumber: '',
  customerName: '',
  orderDate: new Date().toISOString().slice(0, 10),
  status: 'Draft',
  notes: '',
  items: [{ id: null, productVariantId: '', quantity: 1, unitPrice: 0 }],
}

function toDateInput(value: string): string {
  return value.slice(0, 10)
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value)
}

export function SalesOrderDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const isNew = !id || id === 'new'
  const [order, setOrder] = useState<SalesOrderInput>(emptyOrder)
  const [variants, setVariants] = useState<VariantOption[]>([])
  const [loading, setLoading] = useState(false)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  useEffect(() => {
    async function loadVariants() {
      const products = await getProducts('')
      const variantGroups = await Promise.all(
        products.map(async (product: Product) => ({
          product,
          variants: await getProductVariants(product.id),
        })),
      )

      setVariants(
        variantGroups.flatMap(({ product, variants: productVariants }) =>
          productVariants.map((variant) => ({
            ...variant,
            productCode: product.productCode,
            productName: product.productName,
          })),
        ),
      )
    }

    void loadVariants().catch(() => setError('Ürün varyantları yüklenemedi.'))
  }, [])

  const loadOrder = useCallback(async () => {
    if (isNew || !id) {
      setOrder(emptyOrder)
      return
    }

    setLoading(true)
    setError(null)

    try {
      const data = await getSalesOrder(id)
      setOrder({
        salesOrderNumber: data.salesOrderNumber,
        customerName: data.customerName,
        orderDate: toDateInput(data.orderDate),
        status: data.status,
        notes: data.notes ?? '',
        items: data.items.map((item) => ({
          id: item.id,
          productVariantId: item.productVariantId,
          quantity: item.quantity,
          unitPrice: item.unitPrice,
        })),
      })
    } catch (exception) {
      setError(toUserMessage(exception, 'Satış siparişi yüklenemedi.'))
    } finally {
      setLoading(false)
    }
  }, [id, isNew])

  useEffect(() => {
    void loadOrder()
  }, [loadOrder])

  const totalAmount = useMemo(
    () => order.items.reduce((total, item) => total + item.quantity * item.unitPrice, 0),
    [order.items],
  )

  function updateField<K extends keyof SalesOrderInput>(field: K, value: SalesOrderInput[K]) {
    setOrder((current) => ({ ...current, [field]: value }))
  }

  function updateItem(index: number, patch: Partial<SalesOrderItemInput>) {
    setOrder((current) => ({
      ...current,
      items: current.items.map((item, itemIndex) => (itemIndex === index ? { ...item, ...patch } : item)),
    }))
  }

  function addItem() {
    setOrder((current) => ({
      ...current,
      items: [...current.items, { id: null, productVariantId: '', quantity: 1, unitPrice: 0 }],
    }))
  }

  function removeItem(index: number) {
    setOrder((current) => ({
      ...current,
      items: current.items.length === 1 ? current.items : current.items.filter((_, itemIndex) => itemIndex !== index),
    }))
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      const saved = isNew ? await createSalesOrder(order) : await updateSalesOrder(id!, order)
      setSuccess(isNew ? 'Satış siparişi oluşturuldu.' : 'Satış siparişi kaydedildi.')

      if (isNew) {
        navigate(`/sales/orders/${saved.id}`, { replace: true })
      } else {
        await loadOrder()
      }
    } catch (exception) {
      setError(toUserMessage(exception, 'Satış siparişi kaydedilemedi.'))
    } finally {
      setSaving(false)
    }
  }

  return (
    <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
      <Stack spacing={3}>
        <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}>
          <Box>
            <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>{isNew ? 'Satış Siparişi Ekle' : 'Satış Siparişi Düzenle'}</Typography>
            <Typography color="text.secondary">Stok kontrolü tamamlanma sırasında otomatik yapılır.</Typography>
          </Box>
          <Stack direction="row" spacing={1}>
            <Button onClick={() => navigate('/sales')}>{commonText.back}</Button>
            <Button type="submit" variant="contained" startIcon={<SaveOutlinedIcon />} disabled={saving || loading}>{commonText.save}</Button>
          </Stack>
        </Stack>

        {error && <Alert severity="error" sx={{ whiteSpace: 'pre-line' }}>{error}</Alert>}

        <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 1 }}>
          <Stack spacing={2.5}>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField label="Sipariş Numarası" value={isNew ? 'Otomatik oluşturulacaktır' : order.salesOrderNumber} disabled helperText={isNew ? 'Kaydettiğinizde sistem tarafından verilir.' : 'Oluşturulduktan sonra değiştirilemez.'} fullWidth />
              <TextField label="Müşteri" value={order.customerName} onChange={(event) => updateField('customerName', event.target.value)} required helperText={!order.customerName.trim() ? requiredMessage('Müşteri') : ' '} fullWidth />
            </Stack>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField label="Sipariş Tarihi" type="date" value={order.orderDate} onChange={(event) => updateField('orderDate', event.target.value)} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
              <TextField select label="Durum" value={order.status} onChange={(event) => updateField('status', event.target.value)} required fullWidth>
                {statuses.map((status) => <MenuItem key={status} value={status}>{trStatus(status)}</MenuItem>)}
              </TextField>
            </Stack>
            <TextField label="Notlar" value={order.notes} onChange={(event) => updateField('notes', event.target.value)} multiline minRows={2} fullWidth />
          </Stack>
        </Paper>

        <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 1 }}>
          <Stack spacing={2.5}>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}>
              <Typography variant="h6" sx={{ fontWeight: 800 }}>Ürünler</Typography>
              <Button variant="outlined" onClick={addItem}>Satır Ekle</Button>
            </Stack>
            {order.items.map((item, index) => {
              const variant = variants.find((option) => option.id === item.productVariantId)
              const lineTotal = item.quantity * item.unitPrice

              return (
                <Stack key={`${item.id ?? 'new'}-${index}`} direction={{ xs: 'column', lg: 'row' }} spacing={2} sx={{ alignItems: { lg: 'flex-start' } }}>
                  <TextField select label="Ürün / Varyant" value={item.productVariantId} onChange={(event) => updateItem(index, { productVariantId: event.target.value })} required helperText={variant ? `Stok: ${variant.stock}` : requiredMessage('Ürün')} fullWidth sx={{ flex: 2 }}>
                    {variants.map((option) => (
                      <MenuItem key={option.id} value={option.id}>
                        {option.productCode} - {option.productName} / {option.color} / {option.size} - Stok: {option.stock}
                      </MenuItem>
                    ))}
                  </TextField>
                  <TextField label="Miktar" type="number" value={item.quantity} onChange={(event) => updateItem(index, { quantity: Number(event.target.value) })} required fullWidth sx={{ flex: 0.7 }} />
                  <TextField label="Birim Fiyat" type="number" value={item.unitPrice} onChange={(event) => updateItem(index, { unitPrice: Number(event.target.value) })} required fullWidth sx={{ flex: 0.8 }} />
                  <TextField label="Tutar" value={formatCurrency(lineTotal)} disabled fullWidth sx={{ flex: 0.8 }} />
                  <IconButton color="error" onClick={() => removeItem(index)} disabled={order.items.length === 1} aria-label="Satırı sil">
                    <DeleteOutlinedIcon />
                  </IconButton>
                </Stack>
              )
            })}
            <Stack direction="row" sx={{ justifyContent: 'flex-end' }}>
              <Typography variant="h6" sx={{ fontWeight: 800 }}>Toplam: {formatCurrency(totalAmount)}</Typography>
            </Stack>
          </Stack>
        </Paper>
      </Stack>
      <Snackbar open={!!success} autoHideDuration={3000} onClose={() => setSuccess(null)} message={success} />
    </Box>
  )
}
