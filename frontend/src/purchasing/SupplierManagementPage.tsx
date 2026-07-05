import { useCallback, useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import {
  Alert,
  Box,
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  IconButton,
  InputAdornment,
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
import { createSupplier, deleteSupplier, getSuppliers, updateSupplier } from './api'
import type { Supplier, SupplierInput } from './types'
import { commonText, confirmDelete, dialogContentSx, dialogPaperSx, requiredMessage } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

const emptySupplier: SupplierInput = {
  supplierCode: '',
  supplierName: '',
  phone: '',
  email: '',
  address: '',
  taxNumber: '',
  paymentTerm: '',
  active: true,
}

export function SupplierManagementPage() {
  const [suppliers, setSuppliers] = useState<Supplier[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingSupplier, setEditingSupplier] = useState<Supplier | null>(null)
  const [supplierInput, setSupplierInput] = useState<SupplierInput>(emptySupplier)

  const loadSuppliers = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getSuppliers(search)
      setSuppliers(data.items)
    } catch (exception) {
      setError(toUserMessage(exception, 'Tedarikçiler yüklenemedi.'))
    } finally {
      setLoading(false)
    }
  }, [search])

  useEffect(() => {
    const handle = window.setTimeout(() => {
      void loadSuppliers()
    }, 250)

    return () => window.clearTimeout(handle)
  }, [loadSuppliers])

  function openAddDialog() {
    setEditingSupplier(null)
    setSupplierInput(emptySupplier)
    setDialogOpen(true)
  }

  function openEditDialog(supplier: Supplier) {
    setEditingSupplier(supplier)
    setSupplierInput({
      supplierCode: supplier.supplierCode,
      supplierName: supplier.supplierName,
      phone: supplier.phone ?? '',
      email: supplier.email ?? '',
      address: supplier.address ?? '',
      taxNumber: supplier.taxNumber ?? '',
      paymentTerm: supplier.paymentTerm ?? '',
      active: supplier.active,
    })
    setDialogOpen(true)
  }

  const handleDelete = useCallback(
    async (supplier: Supplier) => {
      const confirmed = confirmDelete(supplier.supplierName)

      if (!confirmed) {
        return
      }

      setError(null)

      try {
        await deleteSupplier(supplier.id)
        await loadSuppliers()
      } catch (exception) {
        setError(toUserMessage(exception, 'Tedarikçi silinemedi.'))
      }
    },
    [loadSuppliers],
  )

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      if (editingSupplier) {
        await updateSupplier(editingSupplier.id, supplierInput)
      } else {
        await createSupplier(supplierInput)
      }

      setDialogOpen(false)
      await loadSuppliers()
    } catch (exception) {
        setError(toUserMessage(exception, 'Tedarikçi kaydedilemedi.'))
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo<GridColDef<Supplier>[]>(
    () => [
      { field: 'supplierCode', headerName: 'Kod', minWidth: 140, flex: 0.7 },
      { field: 'supplierName', headerName: 'Tedarikçi', minWidth: 220, flex: 1.2 },
      { field: 'phone', headerName: 'Telefon', minWidth: 140, flex: 0.7 },
      { field: 'email', headerName: 'E-posta', minWidth: 190, flex: 1 },
      { field: 'taxNumber', headerName: 'Vergi No', minWidth: 130, flex: 0.6 },
      {
        field: 'active',
        headerName: 'Aktif',
        minWidth: 100,
        flex: 0.4,
        valueFormatter: (value: boolean) => (value ? commonText.yes : commonText.no),
      },
      {
        field: 'actions',
        headerName: '',
        sortable: false,
        filterable: false,
        width: 112,
        align: 'right',
        renderCell: ({ row }) => (
          <Stack direction="row" spacing={0.5} sx={{ justifyContent: 'flex-end', width: '100%' }}>
            <Tooltip title="Tedarikçi düzenle">
              <IconButton size="small" onClick={() => openEditDialog(row)}>
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Tedarikçi sil">
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
      <Stack
        direction={{ xs: 'column', md: 'row' }}
        spacing={2}
        sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}
      >
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>
            Tedarikçiler
          </Typography>
          <Typography color="text.secondary">Satın alma sürecinde kullanılan tedarikçi kayıtlarını yönetin.</Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openAddDialog}>
          Tedarikçi Ekle
        </Button>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Tedarikçi ara"
            size="small"
            fullWidth
            slotProps={{
              input: {
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon fontSize="small" />
                  </InputAdornment>
                ),
              },
            }}
          />
          <Box sx={{ width: '100%', minHeight: 460 }}>
            <DataGrid
              rows={suppliers}
              columns={columns}
              loading={loading}
              disableRowSelectionOnClick
              pageSizeOptions={[10, 25, 50]}
              initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
              sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }}
            />
          </Box>
        </Stack>
      </Paper>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} fullWidth maxWidth="md" slotProps={{ paper: { sx: dialogPaperSx } }}>
        <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
          <DialogTitle>{editingSupplier ? 'Tedarikçi Düzenle' : 'Tedarikçi Ekle'}</DialogTitle>
          <DialogContent sx={dialogContentSx}>
            {error && <Alert severity="error" sx={{ mb: 2, whiteSpace: 'pre-line' }}>{error}</Alert>}
            <Stack spacing={2.5} sx={{ pt: 1 }}>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField
                  label="Tedarikçi Kodu"
                  value={supplierInput.supplierCode}
                  onChange={(event) => setSupplierInput((current) => ({ ...current, supplierCode: event.target.value }))}
                  required
                  helperText={!supplierInput.supplierCode.trim() ? requiredMessage('Tedarikçi kodu') : ' '}
                  fullWidth
                />
                <TextField
                  label="Tedarikçi Adı"
                  value={supplierInput.supplierName}
                  onChange={(event) => setSupplierInput((current) => ({ ...current, supplierName: event.target.value }))}
                  required
                  helperText={!supplierInput.supplierName.trim() ? requiredMessage('Tedarikçi adı') : ' '}
                  fullWidth
                />
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField
                  label="Telefon"
                  value={supplierInput.phone}
                  onChange={(event) => setSupplierInput((current) => ({ ...current, phone: event.target.value }))}
                  fullWidth
                />
                <TextField
                  label="E-posta"
                  type="email"
                  value={supplierInput.email}
                  onChange={(event) => setSupplierInput((current) => ({ ...current, email: event.target.value }))}
                  fullWidth
                />
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField
                  label="Vergi Numarası"
                  value={supplierInput.taxNumber}
                  onChange={(event) => setSupplierInput((current) => ({ ...current, taxNumber: event.target.value }))}
                  fullWidth
                />
                <TextField
                  label="Ödeme Vadesi"
                  value={supplierInput.paymentTerm}
                  onChange={(event) => setSupplierInput((current) => ({ ...current, paymentTerm: event.target.value }))}
                  fullWidth
                />
              </Stack>
              <TextField
                label="Adres"
                value={supplierInput.address}
                onChange={(event) => setSupplierInput((current) => ({ ...current, address: event.target.value }))}
                multiline
                minRows={2}
                fullWidth
              />
              <FormControlLabel
                control={
                  <Checkbox
                    checked={supplierInput.active}
                    onChange={(event) => setSupplierInput((current) => ({ ...current, active: event.target.checked }))}
                  />
                }
                label="Aktif"
              />
            </Stack>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setDialogOpen(false)}>{commonText.cancel}</Button>
            <Button type="submit" variant="contained" disabled={saving}>
              {commonText.save}
            </Button>
          </DialogActions>
        </Box>
      </Dialog>
    </Stack>
  )
}
