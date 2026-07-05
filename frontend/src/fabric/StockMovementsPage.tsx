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
      setError(exception instanceof Error ? exception.message : 'Stock movements could not be loaded.')
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

    void loadLookups().catch(() => setError('Fabric movement lookups could not be loaded.'))
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
      setError(exception instanceof Error ? exception.message : 'Purchase arrival could not be saved.')
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
      setError(exception instanceof Error ? exception.message : 'Movement could not be saved.')
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
      setError(exception instanceof Error ? exception.message : 'Consumption could not be saved.')
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo<GridColDef<FabricMovement>[]>(
    () => [
      { field: 'fabricCode', headerName: 'Fabric Code', minWidth: 140, flex: 0.7 },
      { field: 'fabricName', headerName: 'Fabric', minWidth: 200, flex: 1 },
      { field: 'movementType', headerName: 'Type', minWidth: 180, flex: 0.9 },
      { field: 'quantityKg', headerName: 'Kg', type: 'number', minWidth: 110, flex: 0.5 },
      { field: 'warehouse', headerName: 'Warehouse', minWidth: 150, flex: 0.7 },
      { field: 'supplierName', headerName: 'Supplier', minWidth: 180, flex: 0.9 },
      { field: 'movementDate', headerName: 'Date', minWidth: 120, valueFormatter: (value: string) => date(value) },
    ],
    [],
  )

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>Stock Movements</Typography>
          <Typography color="text.secondary">Every fabric stock change is recorded here.</Typography>
        </Box>
        <Stack direction="row" spacing={1}>
          <Button variant="outlined" startIcon={<FileDownloadOutlinedIcon />}>Export</Button>
          <Button variant="outlined" startIcon={<RemoveIcon />} onClick={() => setConsumptionOpen(true)}>Consume</Button>
          <Button variant="outlined" startIcon={<AddIcon />} onClick={() => setMovementOpen(true)}>Movement</Button>
          <Button variant="contained" startIcon={<AddIcon />} onClick={() => setPurchaseOpen(true)}>Purchase Arrival</Button>
        </Stack>
      </Stack>
      {error && <Alert severity="error">{error}</Alert>}
      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search movements" size="small" fullWidth slotProps={{ input: { startAdornment: <InputAdornment position="start"><SearchIcon fontSize="small" /></InputAdornment> } }} />
          <Box sx={{ width: '100%', minHeight: 500 }}><DataGrid rows={movements} columns={columns} loading={loading} disableRowSelectionOnClick pageSizeOptions={[10, 25, 50]} initialState={{ pagination: { paginationModel: { pageSize: 10 } } }} sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }} /></Box>
        </Stack>
      </Paper>

      <Dialog open={purchaseOpen} onClose={() => setPurchaseOpen(false)} fullWidth maxWidth="md">
        <Box component="form" onSubmit={(event) => void submitPurchase(event)}>
          <DialogTitle>Fabric Purchase Arrival</DialogTitle>
          <DialogContent><Stack spacing={2.5} sx={{ pt: 1 }}>
            <TextField select label="Fabric" value={purchaseInput.fabricId} onChange={(event) => selectPurchaseFabric(event.target.value)} required fullWidth>{fabrics.map((fabric) => <MenuItem key={fabric.id} value={fabric.id}>{fabric.fabricCode} - {fabric.fabricName}</MenuItem>)}</TextField>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField select label="Supplier" value={purchaseInput.supplierId} onChange={(event) => setPurchaseInput((current) => ({ ...current, supplierId: event.target.value }))} required fullWidth>{suppliers.map((supplier) => <MenuItem key={supplier.id} value={supplier.id}>{supplier.supplierName}</MenuItem>)}</TextField>
              <TextField select label="Purchase Order" value={purchaseInput.purchaseOrderId ?? ''} onChange={(event) => setPurchaseInput((current) => ({ ...current, purchaseOrderId: event.target.value || null }))} fullWidth><MenuItem value="">None</MenuItem>{orders.map((order) => <MenuItem key={order.id} value={order.id}>{order.purchaseNumber}</MenuItem>)}</TextField>
            </Stack>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField label="Batch / Lot" value={purchaseInput.batchLot} onChange={(event) => setPurchaseInput((current) => ({ ...current, batchLot: event.target.value }))} fullWidth />
              <TextField label="Total Weight (Kg)" type="number" value={purchaseInput.totalWeightKg} onChange={(event) => setPurchaseInput((current) => ({ ...current, totalWeightKg: Number(event.target.value) }))} required fullWidth />
              <TextField label="Unit Price" type="number" value={purchaseInput.unitPrice} onChange={(event) => setPurchaseInput((current) => ({ ...current, unitPrice: Number(event.target.value) }))} fullWidth />
            </Stack>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField label="Warehouse" value={purchaseInput.warehouse} onChange={(event) => setPurchaseInput((current) => ({ ...current, warehouse: event.target.value }))} required fullWidth />
              <TextField label="Arrival Date" type="date" value={purchaseInput.arrivalDate} onChange={(event) => setPurchaseInput((current) => ({ ...current, arrivalDate: event.target.value }))} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
            </Stack>
            <TextField label="Notes" value={purchaseInput.notes} onChange={(event) => setPurchaseInput((current) => ({ ...current, notes: event.target.value }))} multiline minRows={2} fullWidth />
          </Stack></DialogContent>
          <DialogActions><Button onClick={() => setPurchaseOpen(false)}>Cancel</Button><Button type="submit" variant="contained" disabled={saving}>Save</Button></DialogActions>
        </Box>
      </Dialog>

      <Dialog open={movementOpen} onClose={() => setMovementOpen(false)} fullWidth maxWidth="md">
        <Box component="form" onSubmit={(event) => void submitMovement(event)}>
          <DialogTitle>Manual Stock Movement</DialogTitle>
          <DialogContent><Stack spacing={2.5} sx={{ pt: 1 }}>
            <TextField select label="Fabric" value={movementInput.fabricId} onChange={(event) => setMovementInput((current) => ({ ...current, fabricId: event.target.value }))} required fullWidth>{fabrics.map((fabric) => <MenuItem key={fabric.id} value={fabric.id}>{fabric.fabricCode} - {fabric.fabricName}</MenuItem>)}</TextField>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField select label="Movement Type" value={movementInput.movementType} onChange={(event) => setMovementInput((current) => ({ ...current, movementType: event.target.value }))} required fullWidth>{movementTypes.map((type) => <MenuItem key={type} value={type}>{type}</MenuItem>)}</TextField>
              <TextField label="Quantity (Kg)" type="number" value={movementInput.quantityKg} onChange={(event) => setMovementInput((current) => ({ ...current, quantityKg: Number(event.target.value) }))} required fullWidth />
            </Stack>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <TextField label="Warehouse" value={movementInput.warehouse} onChange={(event) => setMovementInput((current) => ({ ...current, warehouse: event.target.value }))} required fullWidth />
              <TextField label="Date" type="date" value={movementInput.movementDate} onChange={(event) => setMovementInput((current) => ({ ...current, movementDate: event.target.value }))} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
            </Stack>
            <TextField label="Notes" value={movementInput.notes} onChange={(event) => setMovementInput((current) => ({ ...current, notes: event.target.value }))} multiline minRows={2} fullWidth />
          </Stack></DialogContent>
          <DialogActions><Button onClick={() => setMovementOpen(false)}>Cancel</Button><Button type="submit" variant="contained" disabled={saving}>Save</Button></DialogActions>
        </Box>
      </Dialog>

      <Dialog open={consumptionOpen} onClose={() => setConsumptionOpen(false)} fullWidth maxWidth="sm">
        <Box component="form" onSubmit={(event) => void submitConsumption(event)}>
          <DialogTitle>Production Consumption</DialogTitle>
          <DialogContent><Stack spacing={2.5} sx={{ pt: 1 }}>
            <TextField select label="Fabric" value={consumptionInput.fabricId} onChange={(event) => setConsumptionInput((current) => ({ ...current, fabricId: event.target.value }))} required fullWidth>{fabrics.map((fabric) => <MenuItem key={fabric.id} value={fabric.id}>{fabric.fabricCode} - {fabric.fabricName}</MenuItem>)}</TextField>
            <TextField label="Quantity (Kg)" type="number" value={consumptionInput.quantityKg} onChange={(event) => setConsumptionInput((current) => ({ ...current, quantityKg: Number(event.target.value) }))} required fullWidth />
            <TextField label="Production Reference" value={consumptionInput.productionReference} onChange={(event) => setConsumptionInput((current) => ({ ...current, productionReference: event.target.value }))} required fullWidth />
            <TextField label="Consumption Date" type="date" value={consumptionInput.consumptionDate} onChange={(event) => setConsumptionInput((current) => ({ ...current, consumptionDate: event.target.value }))} required fullWidth slotProps={{ inputLabel: { shrink: true } }} />
            <TextField label="Notes" value={consumptionInput.notes} onChange={(event) => setConsumptionInput((current) => ({ ...current, notes: event.target.value }))} multiline minRows={2} fullWidth />
          </Stack></DialogContent>
          <DialogActions><Button onClick={() => setConsumptionOpen(false)}>Cancel</Button><Button type="submit" variant="contained" disabled={saving}>Save</Button></DialogActions>
        </Box>
      </Dialog>
    </Stack>
  )
}
