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
  createPurchaseInvoice,
  deletePurchaseInvoice,
  getPurchaseInvoices,
  getPurchaseOrders,
  getSuppliers,
  updatePurchaseInvoice,
} from './api'
import type { PurchaseInvoice, PurchaseInvoiceInput, PurchaseOrder, Supplier } from './types'
import { commonText, confirmDelete, dialogContentSx, dialogPaperSx, requiredMessage, trStatus } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

const statuses = ['Draft', 'Waiting Payment', 'Completed', 'Cancelled']

const emptyInvoice: PurchaseInvoiceInput = {
  invoiceNumber: '',
  invoiceDate: new Date().toISOString().slice(0, 10),
  supplierId: '',
  purchaseOrderId: null,
  invoiceAmount: 0,
  status: 'Draft',
  notes: '',
  items: [
    {
      id: null,
      purchaseOrderItemId: null,
      itemName: '',
      quantity: 1,
      unit: 'Meter',
      unitPrice: 0,
      totalAmount: 0,
    },
  ],
}

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString('tr-TR')
}

function formatAmount(value: number): string {
  return new Intl.NumberFormat('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value)
}

export function PurchaseInvoicePage() {
  const [invoices, setInvoices] = useState<PurchaseInvoice[]>([])
  const [suppliers, setSuppliers] = useState<Supplier[]>([])
  const [orders, setOrders] = useState<PurchaseOrder[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingInvoice, setEditingInvoice] = useState<PurchaseInvoice | null>(null)
  const [invoiceInput, setInvoiceInput] = useState<PurchaseInvoiceInput>(emptyInvoice)

  const loadInvoices = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getPurchaseInvoices(search)
      setInvoices(data.items)
    } catch (exception) {
      setError(toUserMessage(exception, 'AlÄ±ÅŸ faturalarÄ± yÃ¼klenemedi.'))
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

    void loadLookups().catch(() => setError('SatÄ±n alma seÃ§im listeleri yÃ¼klenemedi.'))
  }, [])

  useEffect(() => {
    const handle = window.setTimeout(() => {
      void loadInvoices()
    }, 250)

    return () => window.clearTimeout(handle)
  }, [loadInvoices])

  function openAddDialog() {
    setEditingInvoice(null)
    setInvoiceInput(emptyInvoice)
    setDialogOpen(true)
  }

  function openEditDialog(invoice: PurchaseInvoice) {
    setEditingInvoice(invoice)
    setInvoiceInput({
      invoiceNumber: invoice.invoiceNumber,
      invoiceDate: invoice.invoiceDate.slice(0, 10),
      supplierId: invoice.supplierId,
      purchaseOrderId: invoice.purchaseOrderId,
      invoiceAmount: invoice.invoiceAmount,
      status: invoice.status,
      notes: invoice.notes ?? '',
      items:
        invoice.items.length > 0
          ? invoice.items.map((item) => ({
              id: item.id,
              purchaseOrderItemId: item.purchaseOrderItemId,
              itemName: item.itemName,
              quantity: item.quantity,
              unit: item.unit,
              unitPrice: item.unitPrice,
              totalAmount: item.totalAmount,
            }))
          : emptyInvoice.items,
    })
    setDialogOpen(true)
  }

  const handleDelete = useCallback(
    async (invoice: PurchaseInvoice) => {
      const confirmed = confirmDelete(invoice.invoiceNumber)

      if (!confirmed) {
        return
      }

      setError(null)

      try {
        await deletePurchaseInvoice(invoice.id)
        await loadInvoices()
      } catch (exception) {
        setError(toUserMessage(exception, 'AlÄ±ÅŸ faturasÄ± silinemedi.'))
      }
    },
    [loadInvoices],
  )

  function updateField(field: keyof PurchaseInvoiceInput, value: string | number | null) {
    setInvoiceInput((current) => ({ ...current, [field]: value }))
  }

  function updateFirstItem(field: keyof PurchaseInvoiceInput['items'][number], value: string | number | null) {
    setInvoiceInput((current) => {
      const item = { ...current.items[0], [field]: value }
      const totalAmount = field === 'quantity' || field === 'unitPrice'
        ? Number(item.quantity) * Number(item.unitPrice)
        : Number(item.totalAmount)

      return {
        ...current,
        invoiceAmount: totalAmount,
        items: [{ ...item, totalAmount }],
      }
    })
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      const payload = {
        ...invoiceInput,
        purchaseOrderId: invoiceInput.purchaseOrderId || null,
        items: invoiceInput.items.filter((item) => item.itemName.trim()),
      }

      if (editingInvoice) {
        await updatePurchaseInvoice(editingInvoice.id, payload)
      } else {
        await createPurchaseInvoice(payload)
      }

      setDialogOpen(false)
      await loadInvoices()
    } catch (exception) {
      setError(toUserMessage(exception, 'AlÄ±ÅŸ faturasÄ± kaydedilemedi.'))
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo<GridColDef<PurchaseInvoice>[]>(
    () => [
      { field: 'invoiceNumber', headerName: 'Fatura No', minWidth: 150, flex: 0.8 },
      { field: 'supplierName', headerName: 'TedarikÃ§i', minWidth: 220, flex: 1.2 },
      { field: 'purchaseNumber', headerName: 'SipariÅŸ No', minWidth: 150, flex: 0.8 },
      {
        field: 'invoiceDate',
        headerName: 'Tarih',
        minWidth: 120,
        flex: 0.6,
        valueFormatter: (value: string) => formatDate(value),
      },
      {
        field: 'invoiceAmount',
        headerName: 'Tutar',
        type: 'number',
        minWidth: 130,
        flex: 0.6,
        valueFormatter: (value: number) => formatAmount(value),
      },
      { field: 'status', headerName: 'Durum', minWidth: 150, flex: 0.7, valueFormatter: (value: string) => trStatus(value) },
      {
        field: 'actions',
        headerName: '',
        sortable: false,
        filterable: false,
        width: 112,
        align: 'right',
        renderCell: ({ row }) => (
          <Stack direction="row" spacing={0.5} sx={{ justifyContent: 'flex-end', width: '100%' }}>
            <Tooltip title="FaturayÄ± dÃ¼zenle">
              <IconButton size="small" onClick={() => openEditDialog(row)}>
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="FaturayÄ± sil">
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
            AlÄ±ÅŸ FaturalarÄ±
          </Typography>
          <Typography color="text.secondary">Mal kabul sonrasÄ± gelen tedarikÃ§i faturalarÄ±nÄ± girin.</Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openAddDialog}>
          Fatura Ekle
        </Button>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField value={search} onChange={(event) => setSearch(event.target.value)} placeholder="AlÄ±ÅŸ faturasÄ± ara" size="small" fullWidth slotProps={{ input: { startAdornment: <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment> } }} />
          <Box sx={{ width: '100%', minHeight: 460 }}>
            <DataGrid rows={invoices} columns={columns} loading={loading} disableRowSelectionOnClick pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }} />
          </Box>
        </Stack>
      </Paper>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} fullWidth maxWidth="md" slotProps={{ paper: { sx: dialogPaperSx } }}>
        <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
          <DialogTitle>{editingInvoice ? 'AlÄ±ÅŸ FaturasÄ± DÃ¼zenle' : 'AlÄ±ÅŸ FaturasÄ± Ekle'}</DialogTitle>
          <DialogContent sx={dialogContentSx}>
            {error && <Alert severity="error" sx={{ mb: 2, whiteSpace: 'pre-line' }}>{error}</Alert>}
            <Stack spacing={2.5} sx={{ pt: 1 }}>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Fatura Numarası" value={editingInvoice ? invoiceInput.invoiceNumber : 'Otomatik oluşturulacaktır'} onChange={(event) => updateField('invoiceNumber', event.target.value)} disabled helperText={editingInvoice ? 'Oluşturulduktan sonra değiştirilemez.' : 'Kaydettiğinizde sistem tarafından verilir.'} fullWidth />
                <TextField label="Fatura Tarihi" type="date" value={invoiceInput.invoiceDate} onChange={(event) => updateField('invoiceDate', event.target.value)} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField select label="TedarikÃ§i" value={invoiceInput.supplierId} onChange={(event) => updateField('supplierId', event.target.value)} required helperText={!invoiceInput.supplierId ? requiredMessage('TedarikÃ§i') : ' '} fullWidth>
                  {suppliers.map((supplier) => <MenuItem key={supplier.id} value={supplier.id}>{supplier.supplierName}</MenuItem>)}
                </TextField>
                <TextField select label="SatÄ±n Alma SipariÅŸi" value={invoiceInput.purchaseOrderId ?? ''} onChange={(event) => updateField('purchaseOrderId', event.target.value || null)} fullWidth>
                  <MenuItem value="">{commonText.none}</MenuItem>
                  {orders.map((order) => <MenuItem key={order.id} value={order.id}>{order.purchaseNumber}</MenuItem>)}
                </TextField>
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Fatura TutarÄ±" type="number" value={invoiceInput.invoiceAmount} onChange={(event) => updateField('invoiceAmount', Number(event.target.value))} required fullWidth />
                <TextField select label="Durum" value={invoiceInput.status} onChange={(event) => updateField('status', event.target.value)} required fullWidth>
                  {statuses.map((status) => <MenuItem key={status} value={status}>{trStatus(status)}</MenuItem>)}
                </TextField>
              </Stack>
              <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>Fatura Kalemi</Typography>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Kalem" value={invoiceInput.items[0].itemName} onChange={(event) => updateFirstItem('itemName', event.target.value)} required fullWidth />
                <TextField label="Miktar" type="number" value={invoiceInput.items[0].quantity} onChange={(event) => updateFirstItem('quantity', Number(event.target.value))} required fullWidth />
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Birim" value={invoiceInput.items[0].unit} onChange={(event) => updateFirstItem('unit', event.target.value)} required fullWidth />
                <TextField label="Birim Fiyat" type="number" value={invoiceInput.items[0].unitPrice} onChange={(event) => updateFirstItem('unitPrice', Number(event.target.value))} required fullWidth />
                <TextField label="SatÄ±r ToplamÄ±" type="number" value={invoiceInput.items[0].totalAmount} onChange={(event) => updateFirstItem('totalAmount', Number(event.target.value))} fullWidth />
              </Stack>
              <TextField label="Notlar" value={invoiceInput.notes} onChange={(event) => updateField('notes', event.target.value)} multiline minRows={2} fullWidth />
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
