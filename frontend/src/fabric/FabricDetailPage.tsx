import { useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  MenuItem,
  Paper,
  Snackbar,
  Stack,
  TextField,
  Typography,
} from '@mui/material'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined'
import { getMasterDataItems } from '../masterData/api'
import type { MasterDataItem } from '../masterData/types'
import { getSuppliers } from '../purchasing/api'
import type { Supplier } from '../purchasing/types'
import { createFabric, getFabric, updateFabric } from './api'
import type { FabricInput } from './types'
import { commonText, requiredMessage, trStatus } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

const statuses = ['Active', 'OUT OF STOCK']

const emptyFabric: FabricInput = {
  fabricCode: '',
  fabricName: '',
  supplierId: '',
  colorId: '',
  composition: '',
  width: 0,
  weightGsm: 0,
  unit: 'Kg',
  purchasePrice: 0,
  currentStockKg: 0,
  minimumStock: 0,
  status: 'Active',
  notes: '',
}

export function FabricDetailPage() {
  const navigate = useNavigate()
  const { id } = useParams()
  const isNew = id === 'new'
  const [fabric, setFabric] = useState<FabricInput>(emptyFabric)
  const [suppliers, setSuppliers] = useState<Supplier[]>([])
  const [colors, setColors] = useState<MasterDataItem[]>([])
  const [loading, setLoading] = useState(!isNew)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  useEffect(() => {
    async function loadLookups() {
      const [supplierResult, colorItems] = await Promise.all([getSuppliers(), getMasterDataItems('colors')])
      setSuppliers(supplierResult.items.filter((supplier) => supplier.active))
      setColors(colorItems.filter((item) => item.isActive))
    }

    void loadLookups().catch(() => setError('KumaÅŸ seÃ§im listeleri yÃ¼klenemedi.'))
  }, [])

  useEffect(() => {
    if (isNew || !id) {
      return
    }

    async function loadFabric() {
      setLoading(true)
      setError(null)

      try {
        const data = await getFabric(id!)
        setFabric({
          fabricCode: data.fabricCode,
          fabricName: data.fabricName,
          supplierId: data.supplierId,
          colorId: data.colorId,
          composition: data.composition ?? '',
          width: data.width,
          weightGsm: data.weightGsm,
          unit: data.unit,
          purchasePrice: data.purchasePrice,
          currentStockKg: data.currentStockKg,
          minimumStock: data.minimumStock,
          status: data.status,
          notes: data.notes ?? '',
        })
      } catch (exception) {
        setError(toUserMessage(exception, 'KumaÅŸ kartÄ± yÃ¼klenemedi.'))
      } finally {
        setLoading(false)
      }
    }

    void loadFabric()
  }, [id, isNew])

  const title = useMemo(() => (isNew ? 'KumaÅŸ Ekle' : 'KumaÅŸ DÃ¼zenle'), [isNew])

  function updateField(field: keyof FabricInput, value: string | number) {
    setFabric((current) => ({ ...current, [field]: value }))
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      const submitter = (event.nativeEvent as SubmitEvent).submitter as HTMLButtonElement | null
      const saveAndNew = submitter?.name === 'saveAndNew'

      if (isNew) {
        await createFabric(fabric)
      } else if (id) {
        const { currentStockKg: _currentStockKg, ...payload } = fabric
        await updateFabric(id, payload)
      }

      setSuccess('Kumaş kartı kaydedildi.')
      if (isNew && saveAndNew) {
        setFabric(emptyFabric)
      } else {
        navigate('/fabric/fabrics')
      }
    } catch (exception) {
      setError(toUserMessage(exception, 'KumaÅŸ kartÄ± kaydedilemedi.'))
    } finally {
      setSaving(false)
    }
  }

  return (
    <Stack spacing={3}>
      <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/fabric/fabrics')}>{commonText.back}</Button>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>{title}</Typography>
          <Typography color="text.secondary">KumaÅŸ kartÄ±, stok ve minimum seviye bilgileri.</Typography>
        </Box>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ borderRadius: 1, p: { xs: 2, md: 3 } }}>
        {loading ? (
          <Stack sx={{ minHeight: 260, alignItems: 'center', justifyContent: 'center' }}><CircularProgress /></Stack>
        ) : (
          <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
            <Stack spacing={3}>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Kumaş Kodu" value={isNew ? 'Otomatik oluşturulacaktır' : fabric.fabricCode} onChange={(event) => updateField('fabricCode', event.target.value)} disabled={isNew} helperText={isNew ? 'Kaydettiğinizde sistem tarafından verilir.' : (!fabric.fabricCode.trim() ? requiredMessage('Kumaş kodu') : ' ')} fullWidth />
                <TextField label="KumaÅŸ AdÄ±" value={fabric.fabricName} onChange={(event) => updateField('fabricName', event.target.value)} required helperText={!fabric.fabricName.trim() ? requiredMessage('KumaÅŸ adÄ±') : ' '} fullWidth />
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField select label="TedarikÃ§i" value={fabric.supplierId} onChange={(event) => updateField('supplierId', event.target.value)} required helperText={!fabric.supplierId ? requiredMessage('TedarikÃ§i') : ' '} fullWidth>
                  {suppliers.map((supplier) => <MenuItem key={supplier.id} value={supplier.id}>{supplier.supplierName}</MenuItem>)}
                </TextField>
                <TextField select label="Renk" value={fabric.colorId} onChange={(event) => updateField('colorId', event.target.value)} required helperText={!fabric.colorId ? requiredMessage('Renk') : ' '} fullWidth>
                  {colors.map((color) => <MenuItem key={color.id} value={color.id}>{color.name}</MenuItem>)}
                </TextField>
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Ä°Ã§erik" value={fabric.composition} onChange={(event) => updateField('composition', event.target.value)} fullWidth />
                <TextField label="En" type="number" value={fabric.width} onChange={(event) => updateField('width', Number(event.target.value))} fullWidth />
                <TextField label="Gramaj (gsm)" type="number" value={fabric.weightGsm} onChange={(event) => updateField('weightGsm', Number(event.target.value))} fullWidth />
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Birim" value={fabric.unit} onChange={(event) => updateField('unit', event.target.value)} required fullWidth />
                <TextField label="AlÄ±ÅŸ FiyatÄ±" type="number" value={fabric.purchasePrice} onChange={(event) => updateField('purchasePrice', Number(event.target.value))} fullWidth />
                <TextField label="Mevcut Stok (Kg)" type="number" value={fabric.currentStockKg} onChange={(event) => updateField('currentStockKg', Number(event.target.value))} disabled={!isNew} fullWidth />
              </Stack>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField label="Minimum Stok" type="number" value={fabric.minimumStock} onChange={(event) => updateField('minimumStock', Number(event.target.value))} fullWidth />
                <TextField select label="Durum" value={fabric.status} onChange={(event) => updateField('status', event.target.value)} required fullWidth>
                  {statuses.map((status) => <MenuItem key={status} value={status}>{trStatus(status)}</MenuItem>)}
                </TextField>
              </Stack>
              <TextField label="Notlar" value={fabric.notes} onChange={(event) => updateField('notes', event.target.value)} multiline minRows={3} fullWidth />
              <Stack direction="row" spacing={2} sx={{ justifyContent: 'flex-end' }}>
                <Button onClick={() => navigate('/fabric/fabrics')}>{commonText.cancel}</Button>
                {isNew && <Button type="submit" name="saveAndNew" variant="outlined" startIcon={<SaveOutlinedIcon />} disabled={saving}>Kaydet ve Yeni</Button>}
                <Button type="submit" variant="contained" startIcon={<SaveOutlinedIcon />} disabled={saving}>{commonText.save}</Button>
              </Stack>
            </Stack>
          </Box>
        )}
      </Paper>
      <Snackbar open={!!success} autoHideDuration={3000} onClose={() => setSuccess(null)} message={success} />
    </Stack>
  )
}

