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
      setError(exception instanceof Error ? exception.message : 'Purchase invoices could not be loaded.')
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

    void loadLookups().catch(() => setError('Purchasing lookups could not be loaded.'))
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
      const confirmed = window.confirm(`Delete purchase invoice ${invoice.invoiceNumber}?`)

      if (!confirmed) {
        return
      }

      setError(null)

      try {
        await deletePurchaseInvoice(invoice.id)
        await loadInvoices()
      } catch (exception) {
        setError(exception instanceof Error ? exception.message : 'Purchase invoice could not be deleted.')
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
      setError(exception instanceof Error ? exception.message : 'Purchase invoice could not be saved.')
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo<GridColDef<PurchaseInvoice>[]>(
    () => [
      { field: 'invoiceNumber', headerName: 'Invoice No', minWidth: 150, flex: 0.8 },
      { field: 'supplierName', headerName: 'Supplier', minWidth: 220, flex: 1.2 },
      { field: 'purchaseNumber', headerName: 'Purchase No', minWidth: 150, flex: 0.8 },
      {
        field: 'invoiceDate',
        headerName: 'Date',
        minWidth: 120,
        flex: 0.6,
        valueFormatter: (value: string) => formatDate(value),
      },
      {
        field: 'invoiceAmount',
        headerName: 'Amount',
        type: 'number',
        minWidth: 130,
        flex: 0.6,
        valueFormatter: (value: number) => formatAmount(value),
      },
      { field: 'status', headerName: 'Status', minWidth: 150, flex: 0.7 },
      {
        field: 'actions',
        headerName: '',
        sortable: false,
        filterable: false,
        width: 112,
        align: 'right',
        renderCell: ({ row }) => (
          <Stack direction="row" spacing={0.5} sx={{ justifyContent: 'flex-end', width: '100%' }}>
            <Tooltip title="Edit invoice">
              <IconButton size="small" onClick={() => openEditDialog(row)}>
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Delete invoice">
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
            Purchase Invoices
          </Typography>
          <Typography color="text.secondary">Enter supplier invoices after goods are received.</Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openAddDialog}>
          Add Invoice
        </Button>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search purchase invoices" size="small" fullWidth slotProps={{ input: { startAdornment: <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment> } }} />
          <Box sx={{ width: '100%', minHeight: 460 }}>
            <DataGrid rows={invoices} columns={columns} loading={loading} disableRowSelectionOnClick pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }} />
          </Box>
        </Stack>
      </Paper>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} fullWidth maxWidth="md">
        <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
          <DialogTitle>{editingInvoice ? 'Edit Purchase Invoice' : 'Add Purchase Invoice'}</DialogTitle>
          <DialogContent>
            <Stack spacing={2.5} sx={{ pt: 1 }}>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Invoice Number" value={invoiceInput.invoiceNumber} onChange={(event) => updateField('invoiceNumber', event.target.value)} required fullWidth />
                <TextField label="Invoice Date" type="date" value={invoiceInput.invoiceDate} onChange={(event) => updateField('invoiceDate', event.target.value)} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField select label="Supplier" value={invoiceInput.supplierId} onChange={(event) => updateField('supplierId', event.target.value)} required fullWidth>
                  {suppliers.map((supplier) => <MenuItem key={supplier.id} value={supplier.id}>{supplier.supplierName}</MenuItem>)}
                </TextField>
                <TextField select label="Purchase Order" value={invoiceInput.purchaseOrderId ?? ''} onChange={(event) => updateField('purchaseOrderId', event.target.value || null)} fullWidth>
                  <MenuItem value="">None</MenuItem>
                  {orders.map((order) => <MenuItem key={order.id} value={order.id}>{order.purchaseNumber}</MenuItem>)}
                </TextField>
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Invoice Amount" type="number" value={invoiceInput.invoiceAmount} onChange={(event) => updateField('invoiceAmount', Number(event.target.value))} required fullWidth />
                <TextField select label="Status" value={invoiceInput.status} onChange={(event) => updateField('status', event.target.value)} required fullWidth>
                  {statuses.map((status) => <MenuItem key={status} value={status}>{status}</MenuItem>)}
                </TextField>
              </Stack>
              <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>Invoice Line</Typography>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Item" value={invoiceInput.items[0].itemName} onChange={(event) => updateFirstItem('itemName', event.target.value)} required fullWidth />
                <TextField label="Quantity" type="number" value={invoiceInput.items[0].quantity} onChange={(event) => updateFirstItem('quantity', Number(event.target.value))} required fullWidth />
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Unit" value={invoiceInput.items[0].unit} onChange={(event) => updateFirstItem('unit', event.target.value)} required fullWidth />
                <TextField label="Unit Price" type="number" value={invoiceInput.items[0].unitPrice} onChange={(event) => updateFirstItem('unitPrice', Number(event.target.value))} required fullWidth />
                <TextField label="Line Total" type="number" value={invoiceInput.items[0].totalAmount} onChange={(event) => updateFirstItem('totalAmount', Number(event.target.value))} fullWidth />
              </Stack>
              <TextField label="Notes" value={invoiceInput.notes} onChange={(event) => updateField('notes', event.target.value)} multiline minRows={2} fullWidth />
            </Stack>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
            <Button type="submit" variant="contained" disabled={saving}>Save</Button>
          </DialogActions>
        </Box>
      </Dialog>
    </Stack>
  )
}
