import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
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
  Snackbar,
  Stack,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined'
import EditOutlinedIcon from '@mui/icons-material/EditOutlined'
import SearchIcon from '@mui/icons-material/Search'
import { getMasterDataItems } from '../masterData/api'
import type { MasterDataItem } from '../masterData/types'
import {
  createProductVariant,
  deleteProductVariant,
  getProductVariants,
  updateProductVariant,
} from './api'
import type { ProductVariant, ProductVariantInput } from './types'
import { commonText, confirmDelete, dialogContentSx, dialogPaperSx, requiredMessage, trStatus } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

type ProductVariantsTabProps = {
  productId: string
}

type VariantErrors = {
  colorId?: string
  sizeId?: string
  barcode?: string
  stock?: string
}

const emptyVariant: ProductVariantInput = {
  colorId: '',
  sizeId: '',
  barcode: '',
  trendyolSku: '',
  stock: 0,
  status: 'Active',
}

const statuses = ['Active', 'Passive', 'Draft']

export function ProductVariantsTab({ productId }: ProductVariantsTabProps) {
  const firstFieldRef = useRef<HTMLInputElement>(null)
  const [variants, setVariants] = useState<ProductVariant[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingVariant, setEditingVariant] = useState<ProductVariant | null>(null)
  const [variantInput, setVariantInput] = useState<ProductVariantInput>({ ...emptyVariant })
  const [validationErrors, setValidationErrors] = useState<VariantErrors>({})
  const [colors, setColors] = useState<MasterDataItem[]>([])
  const [sizes, setSizes] = useState<MasterDataItem[]>([])

  useEffect(() => {
    async function loadMasterData() {
      const [colorItems, sizeItems] = await Promise.all([
        getMasterDataItems('colors'),
        getMasterDataItems('sizes'),
      ])

      setColors(colorItems.filter((item) => item.isActive))
      setSizes(sizeItems.filter((item) => item.isActive))
    }

    void loadMasterData().catch(() => setError('Varyant tanımları yüklenemedi. Renk ve beden tanımlarını kontrol ediniz.'))
  }, [])

  const loadVariants = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getProductVariants(productId)
      setVariants(data)
    } catch (exception) {
      setError(toUserMessage(exception, 'Varyantlar yüklenemedi. Lütfen tekrar deneyiniz.'))
    } finally {
      setLoading(false)
    }
  }, [productId])

  useEffect(() => {
    void loadVariants()
  }, [loadVariants])

  const filteredVariants = useMemo(() => {
    const term = search.trim().toLocaleLowerCase('tr-TR')

    if (!term) {
      return variants
    }

    return variants.filter((variant) =>
      [variant.color, variant.size, variant.barcode, variant.trendyolSku ?? '', trStatus(variant.status)]
        .join(' ')
        .toLocaleLowerCase('tr-TR')
        .includes(term),
    )
  }, [search, variants])

  function openAddDialog() {
    setEditingVariant(null)
    setVariantInput({ ...emptyVariant })
    setValidationErrors({})
    setDialogOpen(true)
    window.setTimeout(() => firstFieldRef.current?.focus(), 100)
  }

  function openEditDialog(variant: ProductVariant) {
    setEditingVariant(variant)
    setVariantInput({
      colorId: variant.colorId,
      sizeId: variant.sizeId,
      barcode: variant.barcode,
      trendyolSku: variant.trendyolSku ?? '',
      stock: variant.stock,
      status: variant.status,
    })
    setValidationErrors({})
    setDialogOpen(true)
    window.setTimeout(() => firstFieldRef.current?.focus(), 100)
  }

  function updateField(field: keyof ProductVariantInput, value: string | number) {
    setVariantInput((current) => ({ ...current, [field]: value }))
    setValidationErrors((current) => ({ ...current, [field]: undefined }))
  }

  function validateVariant() {
    const errors: VariantErrors = {}

    if (!variantInput.colorId) {
      errors.colorId = requiredMessage('Renk')
    }

    if (!variantInput.sizeId) {
      errors.sizeId = requiredMessage('Beden')
    }

    if (!variantInput.barcode.trim()) {
      errors.barcode = requiredMessage('Barkod')
    }

    if (Number(variantInput.stock) < 0) {
      errors.stock = 'Stok negatif olamaz.'
    }

    setValidationErrors(errors)

    if (Object.keys(errors).length > 0) {
      setError(Object.values(errors)[0] ?? 'Varyant bilgilerini kontrol ediniz.')
      window.setTimeout(() => firstFieldRef.current?.focus(), 50)
      return false
    }

    return true
  }

  const handleDelete = useCallback(
    async (variant: ProductVariant) => {
      const confirmed = confirmDelete(`${variant.color} / ${variant.size}`)

      if (!confirmed) {
        return
      }

      setError(null)

      try {
        await deleteProductVariant(productId, variant.id)
        setSuccess('Varyant silindi.')
        await loadVariants()
      } catch (exception) {
        setError(toUserMessage(exception, 'Varyant silinemedi. Bu varyant üretim veya stok kayıtlarında kullanılıyor olabilir.'))
      }
    },
    [loadVariants, productId],
  )

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!validateVariant()) {
      return
    }

    setSaving(true)
    setError(null)

    try {
      const payload = {
        ...variantInput,
        barcode: variantInput.barcode.trim(),
        trendyolSku: variantInput.trendyolSku.trim(),
      }

      if (editingVariant) {
        await updateProductVariant(productId, editingVariant.id, payload)
      } else {
        await createProductVariant(productId, payload)
      }

      setDialogOpen(false)
      setSuccess(editingVariant ? 'Varyant güncellendi.' : 'Varyant eklendi.')
      await loadVariants()
    } catch (exception) {
      setError(toUserMessage(exception, 'Varyant kaydedilemedi. Renk, beden ve barkod bilgilerini kontrol ediniz.'))
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo<GridColDef<ProductVariant>[]>(
    () => [
      { field: 'color', headerName: 'Renk', minWidth: 140, flex: 0.9 },
      { field: 'size', headerName: 'Beden', minWidth: 110, flex: 0.6 },
      { field: 'barcode', headerName: 'Barkod', minWidth: 170, flex: 1 },
      { field: 'trendyolSku', headerName: 'Trendyol SKU', minWidth: 170, flex: 1, valueGetter: (_, row) => row.trendyolSku ?? '-' },
      { field: 'stock', headerName: 'Stok', type: 'number', minWidth: 110, flex: 0.5 },
      {
        field: 'status',
        headerName: 'Durum',
        minWidth: 120,
        flex: 0.6,
        valueFormatter: (value: string) => trStatus(value),
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
            <Tooltip title="Varyantı düzenle">
              <IconButton size="small" onClick={() => openEditDialog(row)} aria-label="Varyantı düzenle">
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Varyantı sil">
              <IconButton size="small" color="error" onClick={() => void handleDelete(row)} aria-label="Varyantı sil">
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
    <Stack spacing={2}>
      <Stack
        direction={{ xs: 'column', md: 'row' }}
        spacing={2}
        sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}
      >
        <Box>
          <Typography variant="h6" sx={{ fontWeight: 700 }}>
            Varyantlar
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Satışa açılan renk ve beden kombinasyonları.
          </Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openAddDialog}>
          Varyant Ekle
        </Button>
      </Stack>

      {error && !dialogOpen && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Renk, beden, barkod veya Trendyol SKU ara"
            label="Varyant Ara"
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
          <Box sx={{ width: '100%', minHeight: 420 }}>
            <DataGrid
              rows={filteredVariants}
              columns={columns}
              loading={loading}
              disableRowSelectionOnClick
              pageSizeOptions={[10, 25, 50]}
              initialState={{
                pagination: {
                  paginationModel: { pageSize: 10 },
                },
              }}
              localeText={{
                noRowsLabel: search.trim() ? 'Aramanıza uygun varyant bulunamadı.' : 'Henüz varyant kaydı yok.',
                noResultsOverlayLabel: 'Sonuç bulunamadı.',
                footerRowSelected: (count) => `${count} satır seçildi`,
              }}
              sx={{
                border: 0,
                '& .MuiDataGrid-columnHeaders': {
                  bgcolor: 'background.default',
                },
              }}
            />
          </Box>
        </Stack>
      </Paper>

      <Dialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        fullWidth
        maxWidth="sm"
        slotProps={{ paper: { sx: dialogPaperSx } }}
      >
        <Box component="form" onSubmit={(event) => void handleSubmit(event)} noValidate>
          <DialogTitle>{editingVariant ? 'Varyant Düzenle' : 'Varyant Ekle'}</DialogTitle>
          <DialogContent sx={dialogContentSx}>
            {error && <Alert severity="error" sx={{ mb: 2, whiteSpace: 'pre-line' }}>{error}</Alert>}
            <Stack spacing={2.5} sx={{ pt: 1 }}>
              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
                <TextField
                  select
                  label="Renk *"
                  value={variantInput.colorId}
                  onChange={(event) => updateField('colorId', event.target.value)}
                  error={!!validationErrors.colorId}
                  helperText={validationErrors.colorId ?? ' '}
                  inputRef={firstFieldRef}
                  fullWidth
                >
                  {colors.map((color) => (
                    <MenuItem key={color.id} value={color.id}>
                      {color.name}
                    </MenuItem>
                  ))}
                </TextField>
                <TextField
                  select
                  label="Beden *"
                  value={variantInput.sizeId}
                  onChange={(event) => updateField('sizeId', event.target.value)}
                  error={!!validationErrors.sizeId}
                  helperText={validationErrors.sizeId ?? ' '}
                  fullWidth
                >
                  {sizes.map((size) => (
                    <MenuItem key={size.id} value={size.id}>
                      {size.name}
                    </MenuItem>
                  ))}
                </TextField>
              </Stack>

              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
                <TextField
                  label="Barkod *"
                  value={variantInput.barcode}
                  onChange={(event) => updateField('barcode', event.target.value)}
                  error={!!validationErrors.barcode}
                  helperText={validationErrors.barcode ?? 'Barkod okuyucu ile okutabilir veya elle yazabilirsiniz.'}
                  fullWidth
                />
                <TextField
                  label="Trendyol SKU"
                  value={variantInput.trendyolSku}
                  onChange={(event) => updateField('trendyolSku', event.target.value)}
                  helperText=" "
                  fullWidth
                />
              </Stack>

              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
                <TextField
                  label="Stok"
                  type="number"
                  value={variantInput.stock}
                  onChange={(event) => updateField('stock', Number(event.target.value))}
                  error={!!validationErrors.stock}
                  helperText={validationErrors.stock ?? ' '}
                  fullWidth
                  slotProps={{
                    htmlInput: {
                      min: 0,
                    },
                  }}
                />
                <TextField
                  select
                  label="Durum"
                  value={variantInput.status}
                  onChange={(event) => updateField('status', event.target.value)}
                  fullWidth
                >
                  {statuses.map((status) => (
                    <MenuItem key={status} value={status}>
                      {trStatus(status)}
                    </MenuItem>
                  ))}
                </TextField>
              </Stack>
            </Stack>
          </DialogContent>
          <DialogActions
            sx={{
              position: 'sticky',
              bottom: 0,
              bgcolor: 'background.paper',
              borderTop: 1,
              borderColor: 'divider',
              px: 3,
              py: 1.5,
            }}
          >
            <Button onClick={() => setDialogOpen(false)}>{commonText.cancel}</Button>
            <Button type="submit" variant="contained" disabled={saving}>
              {saving ? 'Kaydediliyor...' : commonText.save}
            </Button>
          </DialogActions>
        </Box>
      </Dialog>
      <Snackbar open={!!success} autoHideDuration={3000} onClose={() => setSuccess(null)} message={success} />
    </Stack>
  )
}
