import { useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import {
  Alert,
  Box,
  Button,
  IconButton,
  MenuItem,
  Paper,
  Stack,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined'
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined'
import { getMasterDataItems } from '../masterData/api'
import type { MasterDataItem } from '../masterData/types'
import { createPurchaseOrder, getPurchaseOrder, getSuppliers, updatePurchaseOrder } from './api'
import type { PurchaseOrderInput, PurchaseOrderItemInput, Supplier } from './types'
import { commonText, requiredMessage, trStatus } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

const statuses = ['Draft', 'Approved', 'Partially Received', 'Fully Received', 'Waiting Invoice', 'Completed', 'Cancelled']
const itemStatuses = ['Open', 'Partially Received', 'Received', 'Cancelled']

const emptyItem: PurchaseOrderItemInput = {
  id: null,
  fabricTypeId: null,
  colorId: null,
  itemName: '',
  quantity: 1,
  unit: 'Meter',
  unitPrice: 0,
  receivedQuantity: 0,
  status: 'Open',
}

const emptyOrder: PurchaseOrderInput = {
  purchaseNumber: '',
  supplierId: '',
  orderDate: new Date().toISOString().slice(0, 10),
  expectedDate: null,
  status: 'Draft',
  notes: '',
  items: [{ ...emptyItem }],
}

function toDateInput(value: string | null): string {
  return value ? value.slice(0, 10) : ''
}

export function PurchaseOrderDetailPage() {
  const navigate = useNavigate()
  const { id } = useParams()
  const isNew = id === 'new'
  const [order, setOrder] = useState<PurchaseOrderInput>(emptyOrder)
  const [suppliers, setSuppliers] = useState<Supplier[]>([])
  const [fabricTypes, setFabricTypes] = useState<MasterDataItem[]>([])
  const [colors, setColors] = useState<MasterDataItem[]>([])
  const [loading, setLoading] = useState(!isNew)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    async function loadLookups() {
      const [supplierResult, fabricTypeItems, colorItems] = await Promise.all([
        getSuppliers(),
        getMasterDataItems('fabric-types'),
        getMasterDataItems('colors'),
      ])

      setSuppliers(supplierResult.items.filter((supplier) => supplier.active))
      setFabricTypes(fabricTypeItems.filter((item) => item.isActive))
      setColors(colorItems.filter((item) => item.isActive))
    }

    void loadLookups().catch(() => setError('Satın alma seçim listeleri yüklenemedi.'))
  }, [])

  useEffect(() => {
    if (isNew || !id) {
      return
    }

    async function loadOrder() {
      setLoading(true)
      setError(null)

      try {
        const data = await getPurchaseOrder(id!)
        setOrder({
          purchaseNumber: data.purchaseNumber,
          supplierId: data.supplierId,
          orderDate: toDateInput(data.orderDate),
          expectedDate: toDateInput(data.expectedDate) || null,
          status: data.status,
          notes: data.notes ?? '',
          items: data.items.map((item) => ({
            id: item.id,
            fabricTypeId: item.fabricTypeId,
            colorId: item.colorId,
            itemName: item.itemName,
            quantity: item.quantity,
            unit: item.unit,
            unitPrice: item.unitPrice,
            receivedQuantity: item.receivedQuantity,
            status: item.status,
          })),
        })
      } catch (exception) {
        setError(toUserMessage(exception, 'Satın alma siparişi yüklenemedi.'))
      } finally {
        setLoading(false)
      }
    }

    void loadOrder()
  }, [id, isNew])

  const totalAmount = useMemo(
    () => order.items.reduce((total, item) => total + item.quantity * item.unitPrice, 0),
    [order.items],
  )

  function updateOrderField(field: keyof PurchaseOrderInput, value: string | null) {
    setOrder((current) => ({ ...current, [field]: value }))
  }

  function updateItem(index: number, field: keyof PurchaseOrderItemInput, value: string | number | null) {
    setOrder((current) => ({
      ...current,
      items: current.items.map((item, itemIndex) => (itemIndex === index ? { ...item, [field]: value } : item)),
    }))
  }

  function addItem() {
    setOrder((current) => ({ ...current, items: [...current.items, { ...emptyItem }] }))
  }

  function removeItem(index: number) {
    setOrder((current) => ({
      ...current,
      items: current.items.filter((_, itemIndex) => itemIndex !== index),
    }))
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      const payload = {
        ...order,
        expectedDate: order.expectedDate || null,
        items: order.items.filter((item) => item.itemName.trim()),
      }

      if (isNew) {
        await createPurchaseOrder(payload)
      } else if (id) {
        await updatePurchaseOrder(id, payload)
      }

      navigate('/purchasing/orders')
    } catch (exception) {
      setError(toUserMessage(exception, 'Satın alma siparişi kaydedilemedi.'))
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo<GridColDef<PurchaseOrderItemInput & { rowIndex: number }>[]>(
    () => [
      {
        field: 'itemName',
        headerName: 'Kalem',
        minWidth: 190,
        flex: 1,
        renderCell: ({ row }) => (
          <TextField
            value={row.itemName}
            onChange={(event) => updateItem(row.rowIndex, 'itemName', event.target.value)}
            size="small"
            fullWidth
          />
        ),
      },
      {
        field: 'fabricTypeId',
        headerName: 'Kumaş Tipi',
        minWidth: 160,
        flex: 0.8,
        renderCell: ({ row }) => (
          <TextField
            select
            value={row.fabricTypeId ?? ''}
            onChange={(event) => updateItem(row.rowIndex, 'fabricTypeId', event.target.value || null)}
            size="small"
            fullWidth
          >
            <MenuItem value="">{commonText.none}</MenuItem>
            {fabricTypes.map((item) => (
              <MenuItem key={item.id} value={item.id}>
                {item.name}
              </MenuItem>
            ))}
          </TextField>
        ),
      },
      {
        field: 'colorId',
        headerName: 'Renk',
        minWidth: 140,
        flex: 0.7,
        renderCell: ({ row }) => (
          <TextField
            select
            value={row.colorId ?? ''}
            onChange={(event) => updateItem(row.rowIndex, 'colorId', event.target.value || null)}
            size="small"
            fullWidth
          >
            <MenuItem value="">{commonText.none}</MenuItem>
            {colors.map((item) => (
              <MenuItem key={item.id} value={item.id}>
                {item.name}
              </MenuItem>
            ))}
          </TextField>
        ),
      },
      {
        field: 'quantity',
        headerName: 'Miktar',
        minWidth: 110,
        renderCell: ({ row }) => (
          <TextField
            type="number"
            value={row.quantity}
            onChange={(event) => updateItem(row.rowIndex, 'quantity', Number(event.target.value))}
            size="small"
            fullWidth
          />
        ),
      },
      {
        field: 'unit',
        headerName: 'Birim',
        minWidth: 110,
        renderCell: ({ row }) => (
          <TextField
            value={row.unit}
            onChange={(event) => updateItem(row.rowIndex, 'unit', event.target.value)}
            size="small"
            fullWidth
          />
        ),
      },
      {
        field: 'unitPrice',
        headerName: 'Birim Fiyat',
        minWidth: 120,
        renderCell: ({ row }) => (
          <TextField
            type="number"
            value={row.unitPrice}
            onChange={(event) => updateItem(row.rowIndex, 'unitPrice', Number(event.target.value))}
            size="small"
            fullWidth
          />
        ),
      },
      {
        field: 'receivedQuantity',
        headerName: 'Gelen',
        minWidth: 120,
        renderCell: ({ row }) => (
          <TextField
            type="number"
            value={row.receivedQuantity}
            onChange={(event) => updateItem(row.rowIndex, 'receivedQuantity', Number(event.target.value))}
            size="small"
            fullWidth
          />
        ),
      },
      {
        field: 'status',
        headerName: 'Durum',
        minWidth: 160,
        renderCell: ({ row }) => (
          <TextField
            select
            value={row.status}
            onChange={(event) => updateItem(row.rowIndex, 'status', event.target.value)}
            size="small"
            fullWidth
          >
            {itemStatuses.map((status) => (
              <MenuItem key={status} value={status}>
                {trStatus(status)}
              </MenuItem>
            ))}
          </TextField>
        ),
      },
      {
        field: 'actions',
        headerName: '',
        sortable: false,
        filterable: false,
        width: 64,
        renderCell: ({ row }) => (
          <Tooltip title="Kalemi sil">
            <IconButton size="small" color="error" onClick={() => removeItem(row.rowIndex)}>
              <DeleteOutlinedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        ),
      },
    ],
    [colors, fabricTypes],
  )

  const rows = order.items.map((item, index) => ({ ...item, id: item.id ?? `new-${index}`, rowIndex: index }))

  return (
    <Stack spacing={3}>
      <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/purchasing/orders')}>
          {commonText.back}
        </Button>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>
            {isNew ? 'Satın Alma Siparişi Ekle' : 'Satın Alma Siparişi Düzenle'}
          </Typography>
          <Typography color="text.secondary">Tedarikçi, teslim tarihi ve sipariş kalemlerini yönetin.</Typography>
        </Box>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ borderRadius: 1, p: { xs: 2, md: 3 } }}>
        <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
          <Stack spacing={3}>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField label="Sipariş Numarası" value={isNew ? 'Otomatik oluşturulacaktır' : order.purchaseNumber} onChange={(event) => updateOrderField('purchaseNumber', event.target.value)} disabled helperText={isNew ? 'Kaydettiğinizde sistem tarafından verilir.' : 'Oluşturulduktan sonra değiştirilemez.'} fullWidth />
              <TextField
                select
                label="Tedarikçi"
                value={order.supplierId}
                onChange={(event) => updateOrderField('supplierId', event.target.value)}
                disabled={loading}
                required
                helperText={!order.supplierId ? requiredMessage('Tedarikçi') : ' '}
                fullWidth
              >
                {suppliers.map((supplier) => (
                  <MenuItem key={supplier.id} value={supplier.id}>
                    {supplier.supplierName}
                  </MenuItem>
                ))}
              </TextField>
            </Stack>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField
                label="Sipariş Tarihi"
                type="date"
                value={order.orderDate}
                onChange={(event) => updateOrderField('orderDate', event.target.value)}
                required
                fullWidth
                slotProps={{ inputLabel: { shrink: true } }}
              />
              <TextField
                label="Beklenen Tarih"
                type="date"
                value={order.expectedDate ?? ''}
                onChange={(event) => updateOrderField('expectedDate', event.target.value || null)}
                fullWidth
                slotProps={{ inputLabel: { shrink: true } }}
              />
              <TextField
                select
                label="Durum"
                value={order.status}
                onChange={(event) => updateOrderField('status', event.target.value)}
                required
                fullWidth
              >
                {statuses.map((status) => (
                  <MenuItem key={status} value={status}>
                    {trStatus(status)}
                  </MenuItem>
                ))}
              </TextField>
            </Stack>
            <TextField
              label="Notlar"
              value={order.notes}
              onChange={(event) => updateOrderField('notes', event.target.value)}
              multiline
              minRows={2}
              fullWidth
            />

            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ justifyContent: 'space-between' }}>
              <Box>
                <Typography variant="h6" sx={{ fontWeight: 700 }}>
                  Kalemler
                </Typography>
                <Typography color="text.secondary">Toplam: {totalAmount.toLocaleString('tr-TR')}</Typography>
              </Box>
              <Button startIcon={<AddIcon />} onClick={addItem}>
                Kalem Ekle
              </Button>
            </Stack>

            <Box sx={{ width: '100%', minHeight: 360 }}>
              <DataGrid
                rows={rows}
                columns={columns}
                disableRowSelectionOnClick
                hideFooter
                rowHeight={64}
                sx={{ border: 1, borderColor: 'divider', '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }}
              />
            </Box>

            <Stack direction="row" spacing={2} sx={{ justifyContent: 'flex-end' }}>
              <Button onClick={() => navigate('/purchasing/orders')}>{commonText.cancel}</Button>
              <Button type="submit" variant="contained" startIcon={<SaveOutlinedIcon />} disabled={saving}>
                {commonText.save}
              </Button>
            </Stack>
          </Stack>
        </Box>
      </Paper>
    </Stack>
  )
}
