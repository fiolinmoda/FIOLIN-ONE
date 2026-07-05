import { useCallback, useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import { Alert, Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, InputAdornment, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import FileDownloadOutlinedIcon from '@mui/icons-material/FileDownloadOutlined'
import RemoveIcon from '@mui/icons-material/Remove'
import SearchIcon from '@mui/icons-material/Search'
import { getPurchaseOrders, getSuppliers } from '../purchasing/api'
import type { PurchaseOrder, Supplier } from '../purchasing/types'
import { consumeFabric, createFabricMovement, createFabricPurchaseArrival, getFabricMovements, getFabrics } from './api'
import type { Fabric, FabricConsumptionInput, FabricMovement, FabricMovementInput, FabricPurchaseArrivalInput } from './types'
import { commonText, dialogContentSx, dialogPaperSx, trStatus } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

const movementTypes = ['Purchase', 'Production Consumption', 'Manual Adjustment', 'Inventory Count', 'Return']

function date(value: string): string {
  return new Date(value).toLocaleDateString('tr-TR')
}

export function StockMovementsPage() {
  const [movements, setMovements] = useState<FabricMovement[]>([])
  const [fabrics, setFabrics] = useState<Fabric[]>([])
  const [suppliers, setSuppliers] = useState<Supplier[]>([])
  const [orders, setOrders] = useState<PurchaseOrder[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [purchaseOpen, setPurchaseOpen] = useState(false)
  const [movementOpen, setMovementOpen] = useState(false)
  const [consumptionOpen, setConsumptionOpen] = useState(false)

  const today = new Date().toISOString().slice(0, 10)
  const [purchaseInput, setPurchaseInput] = useState<FabricPurchaseArrivalInput>({ supplierId: '', purchaseOrderId: null, fabricId: '', colorId: '', batchLot: '', totalWeightKg: 1, unitPrice: 0, warehouse: '', arrivalDate: today, notes: '' })
  const [movementInput, setMovementInput] = useState<FabricMovementInput>({ fabricId: '', movementType: 'Manual Adjustment', quantityKg: 1, unitPrice: 0, supplierId: null, purchaseOrderId: null, batchLot: '', warehouse: '', movementDate: today, notes: '' })
  const [consumptionInput, setConsumptionInput] = useState<FabricConsumptionInput>({ fabricId: '', quantityKg: 1, productionReference: '', consumptionDate: today, notes: '' })

  const loadMovements = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getFabricMovements(search)
      setMovements(data.items)
    } catch (exception) {
      setError(toUserMessage(exception, 'Stok hareketleri yüklenemedi.'))
    } finally {
      setLoading(false)
    }
  }, [search])

  useEffect(() => {
    async function loadLookups() {
      const [fabricResult, supplierResult, orderResult] = await Promise.all([getFabrics(), getSuppliers(), getPurchaseOrders()])
      setFabrics(fabricResult.items)
      setSuppliers(supplierResult.items.filter((supplier) => supplier.active))
      setOrders(orderResult.items)
    }

    void loadLookups().catch(() => setError('Kumaş hareketi seçim listeleri yüklenemedi.'))
  }, [])

  useEffect(() => {
    const handle = window.setTimeout(() => { void loadMovements() }, 250)
    return () => window.clearTimeout(handle)
  }, [loadMovements])

  function selectPurchaseFabric(fabricId: string) {
    const fabric = fabrics.find((item) => item.id === fabricId)
    setPurchaseInput((current) => ({ ...current, fabricId, supplierId: fabric?.supplierId ?? current.supplierId, colorId: fabric?.colorId ?? current.colorId, unitPrice: fabric?.purchasePrice ?? current.unitPrice }))
  }

  async function submitPurchase(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)
    try {
      await createFabricPurchaseArrival({ ...purchaseInput, purchaseOrderId: purchaseInput.purchaseOrderId || null })
      setPurchaseOpen(false)
      await loadMovements()
    } catch (exception) {
      setError(toUserMessage(exception, 'Kumaş alışı kaydedilemedi.'))
    } finally {
      setSaving(false)
    }
  }

  async function submitMovement(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)
    try {
      await createFabricMovement({ ...movementInput, supplierId: movementInput.supplierId || null, purchaseOrderId: movementInput.purchaseOrderId || null })
      setMovementOpen(false)
      await loadMovements()
    } catch (exception) {
      setError(toUserMessage(exception, 'Stok hareketi kaydedilemedi.'))
    } finally {
      setSaving(false)
    }
  }

  async function submitConsumption(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)
    try {
      await consumeFabric(consumptionInput)
      setConsumptionOpen(false)
      await loadMovements()
    } catch (exception) {
      setError(toUserMessage(exception, 'Kumaş tüketimi kaydedilemedi.'))
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo<GridColDef<FabricMovement>[]>(
    () => [
      { field: 'fabricCode', headerName: 'Kumaş Kodu', minWidth: 140, flex: 0.7 },
      { field: 'fabricName', headerName: 'Kumaş', minWidth: 200, flex: 1 },
      { field: 'movementType', headerName: 'Hareket Tipi', minWidth: 180, flex: 0.9, valueFormatter: (value: string) => trStatus(value) },
      { field: 'quantityKg', headerName: 'Kg', type: 'number', minWidth: 110, flex: 0.5 },
      { field: 'warehouse', headerName: 'Depo', minWidth: 150, flex: 0.7 },
      { field: 'supplierName', headerName: 'Tedarikçi', minWidth: 180, flex: 0.9 },
      { field: 'movementDate', headerName: 'Tarih', minWidth: 120, valueFormatter: (value: string) => date(value) },
    ],
    [],
  )

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>Stok Hareketleri</Typography>
          <Typography color="text.secondary">Kumaş stokundaki her giriş, çıkış ve düzeltme burada izlenir.</Typography>
        </Box>
        <Stack direction="row" spacing={1}>
          <Button variant="outlined" startIcon={<FileDownloadOutlinedIcon />}>Dışa Aktar</Button>
          <Button variant="outlined" startIcon={<RemoveIcon />} onClick={() => setConsumptionOpen(true)}>Tüketim</Button>
          <Button variant="outlined" startIcon={<AddIcon />} onClick={() => setMovementOpen(true)}>Hareket</Button>
          <Button variant="contained" startIcon={<AddIcon />} onClick={() => setPurchaseOpen(true)}>Kumaş Alışı</Button>
        </Stack>
      </Stack>
      {error && <Alert severity="error">{error}</Alert>}
      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Stok hareketi ara" size="small" fullWidth slotProps={{ input: { startAdornment: <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment> } }} />
          <Box sx={{ width: '100%', minHeight: 500 }}><DataGrid rows={movements} columns={columns} loading={loading} disableRowSelectionOnClick pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }} /></Box>
        </Stack>
      </Paper>

      <Dialog open={purchaseOpen} onClose={() => setPurchaseOpen(false)} fullWidth maxWidth="md" slotProps={{ paper: { sx: dialogPaperSx } }}>
        <Box component="form" onSubmit={(event) => void submitPurchase(event)}>
          <DialogTitle>Kumaş Alışı</DialogTitle>
          <DialogContent sx={dialogContentSx}><Stack spacing={2.5} sx={{ pt: 1 }}>
            <TextField select label="Kumaş" value={purchaseInput.fabricId} onChange={(event) => selectPurchaseFabric(event.target.value)} required fullWidth>{fabrics.map((fabric) => <MenuItem key={fabric.id} value={fabric.id}>{fabric.fabricCode} - {fabric.fabricName}</MenuItem>)}</TextField>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField select label="Tedarikçi" value={purchaseInput.supplierId} onChange={(event) => setPurchaseInput((current) => ({ ...current, supplierId: event.target.value }))} required fullWidth>{suppliers.map((supplier) => <MenuItem key={supplier.id} value={supplier.id}>{supplier.supplierName}</MenuItem>)}</TextField>
              <TextField select label="Satın Alma Siparişi" value={purchaseInput.purchaseOrderId ?? ''} onChange={(event) => setPurchaseInput((current) => ({ ...current, purchaseOrderId: event.target.value || null }))} fullWidth><MenuItem value="">{commonText.none}</MenuItem>{orders.map((order) => <MenuItem key={order.id} value={order.id}>{order.purchaseNumber}</MenuItem>)}</TextField>
            </Stack>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField label="Parti / Lot" value={purchaseInput.batchLot} onChange={(event) => setPurchaseInput((current) => ({ ...current, batchLot: event.target.value }))} fullWidth />
              <TextField label="Toplam Ağırlık (Kg)" type="number" value={purchaseInput.totalWeightKg} onChange={(event) => setPurchaseInput((current) => ({ ...current, totalWeightKg: Number(event.target.value) }))} required fullWidth />
              <TextField label="Birim Fiyat" type="number" value={purchaseInput.unitPrice} onChange={(event) => setPurchaseInput((current) => ({ ...current, unitPrice: Number(event.target.value) }))} fullWidth />
            </Stack>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField label="Depo" value={purchaseInput.warehouse} onChange={(event) => setPurchaseInput((current) => ({ ...current, warehouse: event.target.value }))} required fullWidth />
              <TextField label="Geliş Tarihi" type="date" value={purchaseInput.arrivalDate} onChange={(event) => setPurchaseInput((current) => ({ ...current, arrivalDate: event.target.value }))} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
            </Stack>
            <TextField label="Notlar" value={purchaseInput.notes} onChange={(event) => setPurchaseInput((current) => ({ ...current, notes: event.target.value }))} multiline minRows={2} fullWidth />
          </Stack></DialogContent>
          <DialogActions><Button onClick={() => setPurchaseOpen(false)}>{commonText.cancel}</Button><Button type="submit" variant="contained" disabled={saving}>{commonText.save}</Button></DialogActions>
        </Box>
      </Dialog>

      <Dialog open={movementOpen} onClose={() => setMovementOpen(false)} fullWidth maxWidth="md">
        <Box component="form" onSubmit={(event) => void submitMovement(event)}>
          <DialogTitle>Manuel Stok Hareketi</DialogTitle>
          <DialogContent><Stack spacing={2.5} sx={{ pt: 1 }}>
            <TextField select label="Kumaş" value={movementInput.fabricId} onChange={(event) => setMovementInput((current) => ({ ...current, fabricId: event.target.value }))} required fullWidth>{fabrics.map((fabric) => <MenuItem key={fabric.id} value={fabric.id}>{fabric.fabricCode} - {fabric.fabricName}</MenuItem>)}</TextField>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField select label="Hareket Tipi" value={movementInput.movementType} onChange={(event) => setMovementInput((current) => ({ ...current, movementType: event.target.value }))} required fullWidth>{movementTypes.map((type) => <MenuItem key={type} value={type}>{trStatus(type)}</MenuItem>)}</TextField>
              <TextField label="Miktar (Kg)" type="number" value={movementInput.quantityKg} onChange={(event) => setMovementInput((current) => ({ ...current, quantityKg: Number(event.target.value) }))} required fullWidth />
            </Stack>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField label="Depo" value={movementInput.warehouse} onChange={(event) => setMovementInput((current) => ({ ...current, warehouse: event.target.value }))} required fullWidth />
              <TextField label="Tarih" type="date" value={movementInput.movementDate} onChange={(event) => setMovementInput((current) => ({ ...current, movementDate: event.target.value }))} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
            </Stack>
            <TextField label="Notlar" value={movementInput.notes} onChange={(event) => setMovementInput((current) => ({ ...current, notes: event.target.value }))} multiline minRows={2} fullWidth />
          </Stack></DialogContent>
          <DialogActions><Button onClick={() => setMovementOpen(false)}>{commonText.cancel}</Button><Button type="submit" variant="contained" disabled={saving}>{commonText.save}</Button></DialogActions>
        </Box>
      </Dialog>

      <Dialog open={consumptionOpen} onClose={() => setConsumptionOpen(false)} fullWidth maxWidth="sm">
        <Box component="form" onSubmit={(event) => void submitConsumption(event)}>
          <DialogTitle>Üretim Tüketimi</DialogTitle>
          <DialogContent><Stack spacing={2.5} sx={{ pt: 1 }}>
            <TextField select label="Kumaş" value={consumptionInput.fabricId} onChange={(event) => setConsumptionInput((current) => ({ ...current, fabricId: event.target.value }))} required fullWidth>{fabrics.map((fabric) => <MenuItem key={fabric.id} value={fabric.id}>{fabric.fabricCode} - {fabric.fabricName}</MenuItem>)}</TextField>
            <TextField label="Miktar (Kg)" type="number" value={consumptionInput.quantityKg} onChange={(event) => setConsumptionInput((current) => ({ ...current, quantityKg: Number(event.target.value) }))} required fullWidth />
            <TextField label="Üretim Referansı" value={consumptionInput.productionReference} onChange={(event) => setConsumptionInput((current) => ({ ...current, productionReference: event.target.value }))} required fullWidth />
            <TextField label="Tüketim Tarihi" type="date" value={consumptionInput.consumptionDate} onChange={(event) => setConsumptionInput((current) => ({ ...current, consumptionDate: event.target.value }))} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
            <TextField label="Notlar" value={consumptionInput.notes} onChange={(event) => setConsumptionInput((current) => ({ ...current, notes: event.target.value }))} multiline minRows={2} fullWidth />
          </Stack></DialogContent>
          <DialogActions><Button onClick={() => setConsumptionOpen(false)}>{commonText.cancel}</Button><Button type="submit" variant="contained" disabled={saving}>{commonText.save}</Button></DialogActions>
        </Box>
      </Dialog>
    </Stack>
  )
}
