import { useCallback, useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import {
  Alert,
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  InputAdornment,
  MenuItem,
  Paper,
  Stack,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined'
import EditOutlinedIcon from '@mui/icons-material/EditOutlined'
import SearchIcon from '@mui/icons-material/Search'
import {
  createGoodsReceipt,
  deleteGoodsReceipt,
  getGoodsReceipts,
  getPurchaseOrders,
  getSuppliers,
  updateGoodsReceipt,
} from './api'
import type { GoodsReceipt, GoodsReceiptInput, PurchaseOrder, Supplier } from './types'
import { commonText, confirmDelete, dialogContentSx, dialogPaperSx, requiredMessage, trStatus } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

const statuses = ['Draft', 'Accepted', 'Difference', 'Completed', 'Cancelled']

const emptyReceipt: GoodsReceiptInput = {
  receiptNumber: '',
  supplierId: '',
  purchaseOrderId: null,
  receiptDate: new Date().toISOString().slice(0, 10),
  warehouse: '',
  status: 'Draft',
  notes: '',
  items: [
    {
      id: null,
      purchaseOrderItemId: null,
      itemName: '',
      receivedQuantity: 1,
      unit: 'Meter',
      acceptance: 'Accepted',
      differenceQuantity: 0,
    },
  ],
}

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString('tr-TR')
}

export function GoodsReceiptPage() {
  const [receipts, setReceipts] = useState<GoodsReceipt[]>([])
  const [suppliers, setSuppliers] = useState<Supplier[]>([])
  const [orders, setOrders] = useState<PurchaseOrder[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingReceipt, setEditingReceipt] = useState<GoodsReceipt | null>(null)
  const [receiptInput, setReceiptInput] = useState<GoodsReceiptInput>(emptyReceipt)

  const loadReceipts = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getGoodsReceipts(search)
      setReceipts(data.items)
    } catch (exception) {
      setError(toUserMessage(exception, 'Mal kabul kayıtları yüklenemedi.'))
    } finally {
      setLoading(false)
    }
  }, [search])

  useEffect(() => {
    async function loadLookups() {
      const [supplierResult, orderResult] = await Promise.all([getSuppliers(), getPurchaseOrders()])

      setSuppliers(supplierResult.items.filter((supplier) => supplier.active))
      setOrders(orderResult.items)
    }

    void loadLookups().catch(() => setError('Satın alma seçim listeleri yüklenemedi.'))
  }, [])

  useEffect(() => {
    const handle = window.setTimeout(() => {
      void loadReceipts()
    }, 250)

    return () => window.clearTimeout(handle)
  }, [loadReceipts])

  function openAddDialog() {
    setEditingReceipt(null)
    setReceiptInput(emptyReceipt)
    setDialogOpen(true)
  }

  function openEditDialog(receipt: GoodsReceipt) {
    setEditingReceipt(receipt)
    setReceiptInput({
      receiptNumber: receipt.receiptNumber,
      supplierId: receipt.supplierId,
      purchaseOrderId: receipt.purchaseOrderId,
      receiptDate: receipt.receiptDate.slice(0, 10),
      warehouse: receipt.warehouse,
      status: receipt.status,
      notes: receipt.notes ?? '',
      items:
        receipt.items.length > 0
          ? receipt.items.map((item) => ({
              id: item.id,
              purchaseOrderItemId: item.purchaseOrderItemId,
              itemName: item.itemName,
              receivedQuantity: item.receivedQuantity,
              unit: item.unit,
              acceptance: item.acceptance,
              differenceQuantity: item.differenceQuantity,
            }))
          : emptyReceipt.items,
    })
    setDialogOpen(true)
  }

  const handleDelete = useCallback(
    async (receipt: GoodsReceipt) => {
      const confirmed = confirmDelete(receipt.receiptNumber)

      if (!confirmed) {
        return
      }

      setError(null)

      try {
        await deleteGoodsReceipt(receipt.id)
        await loadReceipts()
      } catch (exception) {
        setError(toUserMessage(exception, 'Mal kabul kaydı silinemedi.'))
      }
    },
    [loadReceipts],
  )

  function updateField(field: keyof GoodsReceiptInput, value: string | null) {
    setReceiptInput((current) => ({ ...current, [field]: value }))
  }

  function updateFirstItem(field: keyof GoodsReceiptInput['items'][number], value: string | number | null) {
    setReceiptInput((current) => ({
      ...current,
      items: [{ ...current.items[0], [field]: value }],
    }))
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      const payload = {
        ...receiptInput,
        purchaseOrderId: receiptInput.purchaseOrderId || null,
        items: receiptInput.items.filter((item) => item.itemName.trim()),
      }

      if (editingReceipt) {
        await updateGoodsReceipt(editingReceipt.id, payload)
      } else {
        await createGoodsReceipt(payload)
      }

      setDialogOpen(false)
      await loadReceipts()
    } catch (exception) {
      setError(toUserMessage(exception, 'Mal kabul kaydı kaydedilemedi.'))
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo<GridColDef<GoodsReceipt>[]>(
    () => [
      { field: 'receiptNumber', headerName: 'Kabul No', minWidth: 150, flex: 0.8 },
      { field: 'supplierName', headerName: 'Tedarikçi', minWidth: 220, flex: 1.2 },
      { field: 'purchaseNumber', headerName: 'Sipariş No', minWidth: 150, flex: 0.8 },
      {
        field: 'receiptDate',
        headerName: 'Tarih',
        minWidth: 120,
        flex: 0.6,
        valueFormatter: (value: string) => formatDate(value),
      },
      { field: 'warehouse', headerName: 'Depo', minWidth: 160, flex: 0.8 },
      { field: 'status', headerName: 'Durum', minWidth: 130, flex: 0.6, valueFormatter: (value: string) => trStatus(value) },
      {
        field: 'actions',
        headerName: '',
        sortable: false,
        filterable: false,
        width: 112,
        align: 'right',
        renderCell: ({ row }) => (
          <Stack direction="row" spacing={0.5} sx={{ justifyContent: 'flex-end', width: '100%' }}>
            <Tooltip title="Mal kabul düzenle">
              <IconButton size="small" onClick={() => openEditDialog(row)}>
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Mal kabul sil">
              <IconButton size="small" color="error" onClick={() => void handleDelete(row)}>
                <DeleteOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          </Stack>
        ),
      },
    ],
    [handleDelete],
  )

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>
            Mal Kabul
          </Typography>
          <Typography color="text.secondary">Tedarikçiden gelen malları ve depo kabulünü kaydedin.</Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openAddDialog}>
          Mal Kabul Ekle
        </Button>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Mal kabul ara"
            size="small"
            fullWidth
            slotProps={{ input: { startAdornment: <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment> } }}
          />
          <Box sx={{ width: '100%', minHeight: 460 }}>
            <DataGrid rows={receipts} columns={columns} loading={loading} disableRowSelectionOnClick pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }} />
          </Box>
        </Stack>
      </Paper>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} fullWidth maxWidth="md" slotProps={{ paper: { sx: dialogPaperSx } }}>
        <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
          <DialogTitle>{editingReceipt ? 'Mal Kabul Düzenle' : 'Mal Kabul Ekle'}</DialogTitle>
          <DialogContent sx={dialogContentSx}>
            {error && <Alert severity="error" sx={{ mb: 2, whiteSpace: 'pre-line' }}>{error}</Alert>}
            <Stack spacing={2.5} sx={{ pt: 1 }}>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Kabul Numarası" value={receiptInput.receiptNumber} onChange={(event) => updateField('receiptNumber', event.target.value)} required helperText={!receiptInput.receiptNumber.trim() ? requiredMessage('Kabul numarası') : ' '} fullWidth />
                <TextField select label="Tedarikçi" value={receiptInput.supplierId} onChange={(event) => updateField('supplierId', event.target.value)} required helperText={!receiptInput.supplierId ? requiredMessage('Tedarikçi') : ' '} fullWidth>
                  {suppliers.map((supplier) => <MenuItem key={supplier.id} value={supplier.id}>{supplier.supplierName}</MenuItem>)}
                </TextField>
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField select label="Satın Alma Siparişi" value={receiptInput.purchaseOrderId ?? ''} onChange={(event) => updateField('purchaseOrderId', event.target.value || null)} fullWidth>
                  <MenuItem value="">{commonText.none}</MenuItem>
                  {orders.map((order) => <MenuItem key={order.id} value={order.id}>{order.purchaseNumber}</MenuItem>)}
                </TextField>
                <TextField label="Kabul Tarihi" type="date" value={receiptInput.receiptDate} onChange={(event) => updateField('receiptDate', event.target.value)} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Depo" value={receiptInput.warehouse} onChange={(event) => updateField('warehouse', event.target.value)} required helperText={!receiptInput.warehouse.trim() ? requiredMessage('Depo') : ' '} fullWidth />
                <TextField select label="Durum" value={receiptInput.status} onChange={(event) => updateField('status', event.target.value)} required fullWidth>
                  {statuses.map((status) => <MenuItem key={status} value={status}>{trStatus(status)}</MenuItem>)}
                </TextField>
              </Stack>
              <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>Kabul Kalemi</Typography>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Kalem" value={receiptInput.items[0].itemName} onChange={(event) => updateFirstItem('itemName', event.target.value)} required fullWidth />
                <TextField label="Miktar" type="number" value={receiptInput.items[0].receivedQuantity} onChange={(event) => updateFirstItem('receivedQuantity', Number(event.target.value))} required fullWidth />
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Birim" value={receiptInput.items[0].unit} onChange={(event) => updateFirstItem('unit', event.target.value)} required fullWidth />
                <TextField label="Kabul Durumu" value={receiptInput.items[0].acceptance} onChange={(event) => updateFirstItem('acceptance', event.target.value)} required fullWidth />
                <TextField label="Fark" type="number" value={receiptInput.items[0].differenceQuantity} onChange={(event) => updateFirstItem('differenceQuantity', Number(event.target.value))} fullWidth />
              </Stack>
              <TextField label="Notlar" value={receiptInput.notes} onChange={(event) => updateField('notes', event.target.value)} multiline minRows={2} fullWidth />
            </Stack>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setDialogOpen(false)}>{commonText.cancel}</Button>
            <Button type="submit" variant="contained" disabled={saving}>{commonText.save}</Button>
          </DialogActions>
        </Box>
      </Dialog>
    </Stack>
  )
}
