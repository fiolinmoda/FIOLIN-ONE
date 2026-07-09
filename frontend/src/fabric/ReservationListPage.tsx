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
import FileDownloadOutlinedIcon from '@mui/icons-material/FileDownloadOutlined'
import SearchIcon from '@mui/icons-material/Search'
import { createFabricReservation, deleteFabricReservation, getFabricReservations, getFabrics, updateFabricReservation } from './api'
import type { Fabric, FabricReservation, FabricReservationInput } from './types'
import { commonText, confirmDelete, dialogContentSx, dialogPaperSx, requiredMessage, trStatus } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

const statuses = ['Active', 'Completed', 'Cancelled']

const emptyReservation: FabricReservationInput = {
  fabricId: '',
  reservationNumber: '',
  productionReference: '',
  reservedQuantityKg: 1,
  reservationDate: new Date().toISOString().slice(0, 10),
  status: 'Active',
  notes: '',
}

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString('tr-TR')
}

export function ReservationListPage() {
  const [reservations, setReservations] = useState<FabricReservation[]>([])
  const [fabrics, setFabrics] = useState<Fabric[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingReservation, setEditingReservation] = useState<FabricReservation | null>(null)
  const [reservationInput, setReservationInput] = useState<FabricReservationInput>(emptyReservation)

  const loadReservations = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getFabricReservations(search)
      setReservations(data.items)
    } catch (exception) {
      setError(toUserMessage(exception, 'Rezervasyonlar yÃ¼klenemedi.'))
    } finally {
      setLoading(false)
    }
  }, [search])

  useEffect(() => {
    async function loadFabrics() {
      const data = await getFabrics()
      setFabrics(data.items)
    }

    void loadFabrics().catch(() => setError('KumaÅŸ listesi yÃ¼klenemedi.'))
  }, [])

  useEffect(() => {
    const handle = window.setTimeout(() => { void loadReservations() }, 250)
    return () => window.clearTimeout(handle)
  }, [loadReservations])

  function openAddDialog() {
    setEditingReservation(null)
    setReservationInput(emptyReservation)
    setDialogOpen(true)
  }

  function openEditDialog(reservation: FabricReservation) {
    setEditingReservation(reservation)
    setReservationInput({
      fabricId: reservation.fabricId,
      reservationNumber: reservation.reservationNumber,
      productionReference: reservation.productionReference,
      reservedQuantityKg: reservation.reservedQuantityKg,
      reservationDate: reservation.reservationDate.slice(0, 10),
      status: reservation.status,
      notes: reservation.notes ?? '',
    })
    setDialogOpen(true)
  }

  const handleDelete = useCallback(
    async (reservation: FabricReservation) => {
      const confirmed = confirmDelete(reservation.reservationNumber)

      if (!confirmed) {
        return
      }

      setError(null)

      try {
        await deleteFabricReservation(reservation.id)
        await loadReservations()
      } catch (exception) {
        setError(toUserMessage(exception, 'Rezervasyon silinemedi.'))
      }
    },
    [loadReservations],
  )

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      if (editingReservation) {
        await updateFabricReservation(editingReservation.id, reservationInput)
      } else {
        await createFabricReservation(reservationInput)
      }

      setDialogOpen(false)
      await loadReservations()
    } catch (exception) {
      setError(toUserMessage(exception, 'Rezervasyon kaydedilemedi.'))
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo<GridColDef<FabricReservation>[]>(
    () => [
      { field: 'reservationNumber', headerName: 'Rezervasyon No', minWidth: 160, flex: 0.8 },
      { field: 'fabricCode', headerName: 'KumaÅŸ Kodu', minWidth: 140, flex: 0.7 },
      { field: 'fabricName', headerName: 'KumaÅŸ', minWidth: 220, flex: 1.1 },
      { field: 'productionReference', headerName: 'Ãœretim Ref.', minWidth: 170, flex: 0.8 },
      { field: 'reservedQuantityKg', headerName: 'Rezerve Kg', type: 'number', minWidth: 130, flex: 0.6 },
      { field: 'reservationDate', headerName: 'Tarih', minWidth: 120, valueFormatter: (value: string) => formatDate(value) },
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
            <Tooltip title="Rezervasyonu dÃ¼zenle"><IconButton size="small" onClick={() => openEditDialog(row)}><EditOutlinedIcon fontSize="small" /></IconButton></Tooltip>
            <Tooltip title="Rezervasyonu sil"><IconButton size="small" color="error" onClick={() => void handleDelete(row)}><DeleteOutlinedIcon fontSize="small" /></IconButton></Tooltip>
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
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>KumaÅŸ RezervasyonlarÄ±</Typography>
          <Typography color="text.secondary">Ãœretim iÃ§in kullanÄ±labilir kumaÅŸ miktarÄ±nÄ± rezerve edin.</Typography>
        </Box>
        <Stack direction="row" spacing={1}>
          <Button variant="outlined" startIcon={<FileDownloadOutlinedIcon />}>DÄ±ÅŸa Aktar</Button>
          <Button variant="contained" startIcon={<AddIcon />} onClick={openAddDialog}>Rezervasyon Ekle</Button>
        </Stack>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Rezervasyon ara" size="small" fullWidth slotProps={{ input: { startAdornment: <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment> } }} />
          <Box sx={{ width: '100%', minHeight: 500 }}>
            <DataGrid rows={reservations} columns={columns} loading={loading} disableRowSelectionOnClick pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }} />
          </Box>
        </Stack>
      </Paper>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} fullWidth maxWidth="sm" slotProps={{ paper: { sx: dialogPaperSx } }}>
        <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
          <DialogTitle>{editingReservation ? 'Rezervasyon DÃ¼zenle' : 'Rezervasyon Ekle'}</DialogTitle>
          <DialogContent sx={dialogContentSx}>
            {error && <Alert severity="error" sx={{ mb: 2, whiteSpace: 'pre-line' }}>{error}</Alert>}
            <Stack spacing={2.5} sx={{ pt: 1 }}>
              <TextField select label="KumaÅŸ" value={reservationInput.fabricId} onChange={(event) => setReservationInput((current) => ({ ...current, fabricId: event.target.value }))} required helperText={!reservationInput.fabricId ? requiredMessage('KumaÅŸ') : ' '} fullWidth>
                {fabrics.map((fabric) => <MenuItem key={fabric.id} value={fabric.id}>{fabric.fabricCode} - {fabric.fabricName} ({fabric.availableStockKg} Kg kullanÄ±labilir)</MenuItem>)}
              </TextField>
              <TextField label="Rezervasyon Numarası" value={editingReservation ? reservationInput.reservationNumber : 'Otomatik oluşturulacaktır'} onChange={(event) => setReservationInput((current) => ({ ...current, reservationNumber: event.target.value }))} disabled helperText={editingReservation ? 'Oluşturulduktan sonra değiştirilemez.' : 'Kaydettiğinizde sistem tarafından verilir.'} fullWidth />
              <TextField label="Ãœretim ReferansÄ±" value={reservationInput.productionReference} onChange={(event) => setReservationInput((current) => ({ ...current, productionReference: event.target.value }))} required fullWidth />
              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
                <TextField label="Rezerve Miktar (Kg)" type="number" value={reservationInput.reservedQuantityKg} onChange={(event) => setReservationInput((current) => ({ ...current, reservedQuantityKg: Number(event.target.value) }))} required fullWidth />
                <TextField label="Rezervasyon Tarihi" type="date" value={reservationInput.reservationDate} onChange={(event) => setReservationInput((current) => ({ ...current, reservationDate: event.target.value }))} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
              </Stack>
              <TextField select label="Durum" value={reservationInput.status} onChange={(event) => setReservationInput((current) => ({ ...current, status: event.target.value }))} required fullWidth>
                {statuses.map((status) => <MenuItem key={status} value={status}>{trStatus(status)}</MenuItem>)}
              </TextField>
              <TextField label="Notlar" value={reservationInput.notes} onChange={(event) => setReservationInput((current) => ({ ...current, notes: event.target.value }))} multiline minRows={2} fullWidth />
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
