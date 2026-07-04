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
  Stack,
  TextField,
  Typography,
} from '@mui/material'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined'
import { createProduct, getProduct, updateProduct } from './api'
import type { ProductInput } from './types'

const emptyProduct: ProductInput = {
  productCode: '',
  productName: '',
  brand: '',
  category: '',
  season: '',
  status: 'Active',
}

const statuses = ['Active', 'Passive', 'Draft']

export function ProductDetailPage() {
  const navigate = useNavigate()
  const { id } = useParams()
  const isNew = id === 'new'
  const [product, setProduct] = useState<ProductInput>(emptyProduct)
  const [loading, setLoading] = useState(!isNew)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (isNew || !id) {
      return
    }

    async function loadProduct() {
      setLoading(true)
      setError(null)

      try {
        const data = await getProduct(id!)
        setProduct({
          productCode: data.productCode,
          productName: data.productName,
          brand: data.brand ?? '',
          category: data.category,
          season: data.season ?? '',
          status: data.status,
        })
      } catch (exception) {
        setError(exception instanceof Error ? exception.message : 'Product could not be loaded.')
      } finally {
        setLoading(false)
      }
    }

    void loadProduct()
  }, [id, isNew])

  const title = useMemo(() => (isNew ? 'Add Product' : 'Edit Product'), [isNew])

  function updateField(field: keyof ProductInput, value: string) {
    setProduct((current) => ({ ...current, [field]: value }))
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      if (isNew) {
        await createProduct(product)
      } else if (id) {
        await updateProduct(id, product)
      }

      navigate('/products')
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : 'Product could not be saved.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <Stack spacing={3}>
      <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/products')}>
          Back
        </Button>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>
            {title}
          </Typography>
          <Typography color="text.secondary">Product card details</Typography>
        </Box>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 1 }}>
        {loading ? (
          <Stack sx={{ minHeight: 280, alignItems: 'center', justifyContent: 'center' }}>
            <CircularProgress />
          </Stack>
        ) : (
          <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
            <Stack spacing={3}>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField
                  label="Product Code"
                  value={product.productCode}
                  onChange={(event) => updateField('productCode', event.target.value)}
                  required
                  fullWidth
                />
                <TextField
                  label="Product Name"
                  value={product.productName}
                  onChange={(event) => updateField('productName', event.target.value)}
                  required
                  fullWidth
                />
              </Stack>

              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField
                  label="Brand"
                  value={product.brand}
                  onChange={(event) => updateField('brand', event.target.value)}
                  fullWidth
                />
                <TextField
                  label="Category"
                  value={product.category}
                  onChange={(event) => updateField('category', event.target.value)}
                  required
                  fullWidth
                />
              </Stack>

              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <TextField
                  label="Season"
                  value={product.season}
                  onChange={(event) => updateField('season', event.target.value)}
                  fullWidth
                />
                <TextField
                  select
                  label="Status"
                  value={product.status}
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

              <Stack direction="row" spacing={2} sx={{ justifyContent: 'flex-end' }}>
                <Button onClick={() => navigate('/products')}>Cancel</Button>
                <Button
                  type="submit"
                  variant="contained"
                  startIcon={<SaveOutlinedIcon />}
                  disabled={saving}
                >
                  Save
                </Button>
              </Stack>
            </Stack>
          </Box>
        )}
      </Paper>
    </Stack>
  )
}
