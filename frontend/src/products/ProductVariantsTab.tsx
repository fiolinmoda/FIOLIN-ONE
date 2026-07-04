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
import { getMasterDataItems } from '../masterData/api'
import type { MasterDataItem } from '../masterData/types'
import {
  createProductVariant,
  deleteProductVariant,
  getProductVariants,
  updateProductVariant,
} from './api'
import type { ProductVariant, ProductVariantInput } from './types'

type ProductVariantsTabProps = {
  productId: string
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
  const [variants, setVariants] = useState<ProductVariant[]>([])
  const [loading, setLoading] = useState(false)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingVariant, setEditingVariant] = useState<ProductVariant | null>(null)
  const [variantInput, setVariantInput] = useState<ProductVariantInput>(emptyVariant)
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

    void loadMasterData().catch(() => setError('Variant master data could not be loaded.'))
  }, [])

  const loadVariants = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getProductVariants(productId)
      setVariants(data)
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : 'Variants could not be loaded.')
    } finally {
      setLoading(false)
    }
  }, [productId])

  useEffect(() => {
    void loadVariants()
  }, [loadVariants])

  function openAddDialog() {
    setEditingVariant(null)
    setVariantInput(emptyVariant)
    setDialogOpen(true)
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
    setDialogOpen(true)
  }

  function updateField(field: keyof ProductVariantInput, value: string | number) {
    setVariantInput((current) => ({ ...current, [field]: value }))
  }

  const handleDelete = useCallback(
    async (variant: ProductVariant) => {
      const confirmed = window.confirm(`Delete ${variant.color} / ${variant.size} variant?`)

      if (!confirmed) {
        return
      }

      setError(null)

      try {
        await deleteProductVariant(productId, variant.id)
        await loadVariants()
      } catch (exception) {
        setError(exception instanceof Error ? exception.message : 'Variant could not be deleted.')
      }
    },
    [loadVariants, productId],
  )

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      if (editingVariant) {
        await updateProductVariant(productId, editingVariant.id, variantInput)
      } else {
        await createProductVariant(productId, variantInput)
      }

      setDialogOpen(false)
      await loadVariants()
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : 'Variant could not be saved.')
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo<GridColDef<ProductVariant>[]>(
    () => [
      { field: 'color', headerName: 'Color', minWidth: 140, flex: 0.9 },
      { field: 'size', headerName: 'Size', minWidth: 110, flex: 0.6 },
      { field: 'barcode', headerName: 'Barcode', minWidth: 170, flex: 1 },
      { field: 'trendyolSku', headerName: 'Trendyol SKU', minWidth: 170, flex: 1 },
      { field: 'stock', headerName: 'Stock', type: 'number', minWidth: 110, flex: 0.5 },
      { field: 'status', headerName: 'Status', minWidth: 120, flex: 0.6 },
      {
        field: 'actions',
        headerName: '',
        sortable: false,
        filterable: false,
        width: 112,
        align: 'right',
        renderCell: ({ row }) => (
          <Stack direction="row" spacing={0.5} sx={{ justifyContent: 'flex-end', width: '100%' }}>
            <Tooltip title="Edit variant">
              <IconButton size="small" onClick={() => openEditDialog(row)}>
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Delete variant">
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
    <Stack spacing={2}>
      <Stack
        direction={{ xs: 'column', md: 'row' }}
        spacing={2}
        sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}
      >
        <Box>
          <Typography variant="h6" sx={{ fontWeight: 700 }}>
            Variants
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Sellable color and size combinations for this product model.
          </Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openAddDialog}>
          Add Variant
        </Button>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ borderRadius: 1 }}>
        <Box sx={{ width: '100%', minHeight: 420 }}>
          <DataGrid
            rows={variants}
            columns={columns}
            loading={loading}
            disableRowSelectionOnClick
            pageSizeOptions={[10, 25, 50]}
            initialState={{
              pagination: {
                paginationModel: { pageSize: 10 },
              },
            }}
            sx={{
              border: 0,
              '& .MuiDataGrid-columnHeaders': {
                bgcolor: 'background.default',
              },
            }}
          />
        </Box>
      </Paper>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} fullWidth maxWidth="sm">
        <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
          <DialogTitle>{editingVariant ? 'Edit Variant' : 'Add Variant'}</DialogTitle>
          <DialogContent>
            <Stack spacing={2.5} sx={{ pt: 1 }}>
              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
                <TextField
                  select
                  label="Color"
                  value={variantInput.colorId}
                  onChange={(event) => updateField('colorId', event.target.value)}
                  required
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
                  label="Size"
                  value={variantInput.sizeId}
                  onChange={(event) => updateField('sizeId', event.target.value)}
                  required
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
                  label="Barcode"
                  value={variantInput.barcode}
                  onChange={(event) => updateField('barcode', event.target.value)}
                  required
                  fullWidth
                />
                <TextField
                  label="Trendyol SKU"
                  value={variantInput.trendyolSku}
                  onChange={(event) => updateField('trendyolSku', event.target.value)}
                  fullWidth
                />
              </Stack>

              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
                <TextField
                  label="Stock"
                  type="number"
                  value={variantInput.stock}
                  onChange={(event) => updateField('stock', Number(event.target.value))}
                  required
                  fullWidth
                  slotProps={{
                    htmlInput: {
                      min: 0,
                    },
                  }}
                />
                <TextField
                  select
                  label="Status"
                  value={variantInput.status}
                  onChange={(event) => updateField('status', event.target.value)}
                  required
                  fullWidth
                >
                  {statuses.map((status) => (
                    <MenuItem key={status} value={status}>
                      {status}
                    </MenuItem>
                  ))}
                </TextField>
              </Stack>
            </Stack>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
            <Button type="submit" variant="contained" disabled={saving}>
              Save
            </Button>
          </DialogActions>
        </Box>
      </Dialog>
    </Stack>
  )
}
