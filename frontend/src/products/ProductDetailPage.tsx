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
  Tab,
  Tabs,
  TextField,
  Typography,
} from '@mui/material'
import ArrowBackIcon from '@mui/icons-material/ArrowBack'
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined'
import { getMasterDataItems } from '../masterData/api'
import type { MasterDataItem } from '../masterData/types'
import { createProduct, getProduct, updateProduct } from './api'
import { ProductVariantsTab } from './ProductVariantsTab'
import type { ProductInput } from './types'
import { commonText, requiredMessage, trStatus } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

const emptyProduct: ProductInput = {
  productCode: '',
  productName: '',
  brandId: null,
  categoryId: null,
  seasonId: null,
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
  const [success, setSuccess] = useState<string | null>(null)
  const [activeTab, setActiveTab] = useState(0)
  const [brands, setBrands] = useState<MasterDataItem[]>([])
  const [categories, setCategories] = useState<MasterDataItem[]>([])
  const [seasons, setSeasons] = useState<MasterDataItem[]>([])

  useEffect(() => {
    async function loadMasterData() {
      const [brandItems, categoryItems, seasonItems] = await Promise.all([
        getMasterDataItems('brands'),
        getMasterDataItems('categories'),
        getMasterDataItems('seasons'),
      ])

      setBrands(brandItems.filter((item) => item.isActive))
      setCategories(categoryItems.filter((item) => item.isActive))
      setSeasons(seasonItems.filter((item) => item.isActive))
    }

    void loadMasterData().catch(() => setError('Tanımlar yüklenemedi.'))
  }, [])

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
          brandId: data.brandId,
          categoryId: data.categoryId,
          seasonId: data.seasonId,
          status: data.status,
        })
      } catch (exception) {
        setError(toUserMessage(exception, 'Ürün yüklenemedi.'))
      } finally {
        setLoading(false)
      }
    }

    void loadProduct()
  }, [id, isNew])

  const title = useMemo(() => (isNew ? 'Ürün Ekle' : 'Ürün Düzenle'), [isNew])

  function updateField(field: keyof ProductInput, value: string | null) {
    setProduct((current) => ({ ...current, [field]: value }))
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      const submitter = (event.nativeEvent as SubmitEvent).submitter as HTMLButtonElement | null
      const saveAndNew = submitter?.name === 'saveAndNew'

      if (isNew) {
        await createProduct(product)
      } else if (id) {
        await updateProduct(id, product)
      }

      setSuccess('Ürün kaydedildi.')
      if (isNew && saveAndNew) {
        setProduct(emptyProduct)
      } else {
        navigate('/products')
      }
    } catch (exception) {
      setError(toUserMessage(exception, 'Ürün kaydedilemedi.'))
    } finally {
      setSaving(false)
    }
  }

  return (
    <Stack spacing={3}>
      <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/products')}>
          {commonText.back}
        </Button>
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>
            {title}
          </Typography>
          <Typography color="text.secondary">Ürün kartı temel bilgileri</Typography>
        </Box>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ borderRadius: 1 }}>
        {loading ? (
          <Stack sx={{ minHeight: 280, alignItems: 'center', justifyContent: 'center' }}>
            <CircularProgress />
          </Stack>
        ) : (
          <>
            <Tabs
              value={activeTab}
              onChange={(_, value: number) => setActiveTab(value)}
              variant="scrollable"
              scrollButtons="auto"
              sx={{ borderBottom: 1, borderColor: 'divider', px: { xs: 1, md: 2 } }}
            >
              <Tab label="Ürün Bilgileri" />
              <Tab label="Varyantlar" disabled={isNew} />
            </Tabs>

            {activeTab === 0 && (
              <Box component="form" onSubmit={(event) => void handleSubmit(event)} sx={{ p: { xs: 2, md: 3 } }}>
                <Stack spacing={3}>
                  <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                    <TextField
                      label="Ürün Kodu"
                      value={isNew ? 'Otomatik oluşturulacaktır' : product.productCode}
                      onChange={(event) => updateField('productCode', event.target.value)}
                      disabled={isNew}
                      helperText={!product.productCode.trim() ? requiredMessage('Ürün kodu') : ' '}
                      fullWidth
                    />
                    <TextField
                      label="Ürün Adı"
                      value={product.productName}
                      onChange={(event) => updateField('productName', event.target.value)}
                      required
                      helperText={!product.productName.trim() ? requiredMessage('Ürün adı') : ' '}
                      fullWidth
                    />
                  </Stack>

                  <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                    <TextField
                      select
                      label="Marka"
                      value={product.brandId ?? ''}
                      onChange={(event) => updateField('brandId', event.target.value || null)}
                      fullWidth
                    >
                      <MenuItem value="">{commonText.none}</MenuItem>
                      {brands.map((brand) => (
                        <MenuItem key={brand.id} value={brand.id}>
                          {brand.name}
                        </MenuItem>
                      ))}
                    </TextField>
                    <TextField
                      select
                      label="Kategori"
                      value={product.categoryId ?? ''}
                      onChange={(event) => updateField('categoryId', event.target.value || null)}
                      fullWidth
                    >
                      <MenuItem value="">{commonText.none}</MenuItem>
                      {categories.map((category) => (
                        <MenuItem key={category.id} value={category.id}>
                          {category.name}
                        </MenuItem>
                      ))}
                    </TextField>
                  </Stack>

                  <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                    <TextField
                      select
                      label="Sezon"
                      value={product.seasonId ?? ''}
                      onChange={(event) => updateField('seasonId', event.target.value || null)}
                      fullWidth
                    >
                      <MenuItem value="">{commonText.none}</MenuItem>
                      {seasons.map((season) => (
                        <MenuItem key={season.id} value={season.id}>
                          {season.name}
                        </MenuItem>
                      ))}
                    </TextField>
                    <TextField
                      select
                      label="Durum"
                      value={product.status}
                      onChange={(event) => updateField('status', event.target.value)}
                      required
                      fullWidth
                    >
                      {statuses.map((status) => (
                        <MenuItem key={status} value={status}>
                          {trStatus(status)}
                        </MenuItem>
                      ))}
                    </TextField>
                  </Stack>

                  <Stack direction="row" spacing={2} sx={{ justifyContent: 'flex-end' }}>
                    <Button onClick={() => navigate('/products')}>{commonText.cancel}</Button>
                    {isNew && (
                      <Button type="submit" name="saveAndNew" variant="outlined" startIcon={<SaveOutlinedIcon />} disabled={saving}>
                        Kaydet ve Yeni
                      </Button>
                    )}
                    <Button
                      type="submit"
                      variant="contained"
                      startIcon={<SaveOutlinedIcon />}
                      disabled={saving}
                    >
                      {commonText.save}
                    </Button>
                  </Stack>
                </Stack>
              </Box>
            )}

            {activeTab === 1 && id && !isNew && (
              <Box sx={{ p: { xs: 2, md: 3 } }}>
                <ProductVariantsTab productId={id} />
              </Box>
            )}
          </>
        )}
      </Paper>
      <Snackbar open={!!success} autoHideDuration={3000} onClose={() => setSuccess(null)} message={success} />
    </Stack>
  )
}
