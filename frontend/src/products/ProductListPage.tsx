import { useCallback, useEffect, useMemo, useState } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Alert,
  Avatar,
  Box,
  Button,
  Chip,
  CircularProgress,
  Divider,
  IconButton,
  InputAdornment,
  Pagination,
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
import ExpandMoreIcon from '@mui/icons-material/ExpandMore'
import SearchIcon from '@mui/icons-material/Search'
import { deleteProduct, getProducts } from './api'
import type { Product } from './types'
import { confirmDelete } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

type NavigationState = {
  message?: string
}

const pageSize = 12

function numberText(value: number) {
  return value.toLocaleString('tr-TR')
}

function moneyText(value: number) {
  return value > 0
    ? value.toLocaleString('tr-TR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    : '-'
}

export function ProductListPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const [products, setProducts] = useState<Product[]>([])
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  useEffect(() => {
    const state = location.state as NavigationState | null

    if (state?.message) {
      setSuccess(state.message)
      navigate(location.pathname, { replace: true, state: null })
    }
  }, [location.pathname, location.state, navigate])

  const loadProducts = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getProducts(search)
      setProducts(data)
      setPage(1)
    } catch (exception) {
      setError(toUserMessage(exception, 'ÃœrÃ¼nler yÃ¼klenemedi. LÃ¼tfen tekrar deneyiniz.'))
    } finally {
      setLoading(false)
    }
  }, [search])

  useEffect(() => {
    const handle = window.setTimeout(() => {
      void loadProducts()
    }, 250)

    return () => window.clearTimeout(handle)
  }, [loadProducts])

  const pageCount = Math.max(1, Math.ceil(products.length / pageSize))
  const visibleProducts = useMemo(
    () => products.slice((page - 1) * pageSize, page * pageSize),
    [page, products],
  )

  const handleDelete = useCallback(
    async (product: Product) => {
      const confirmed = confirmDelete(`${product.productCode} - ${product.productName}`)

      if (!confirmed) {
        return
      }

      setError(null)

      try {
        await deleteProduct(product.id)
        setSuccess('ÃœrÃ¼n silindi.')
        await loadProducts()
      } catch (exception) {
        setError(toUserMessage(exception, 'ÃœrÃ¼n silinemedi. ÃœrÃ¼n Ã¼retim veya stok kayÄ±tlarÄ±nda kullanÄ±lÄ±yor olabilir.'))
      }
    },
    [loadProducts],
  )

  return (
    <Stack spacing={3}>
      <Stack
        direction={{ xs: 'column', md: 'row' }}
        spacing={2}
        sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}
      >
        <Box>
          <Typography variant="h5" component="h2" sx={{ fontWeight: 800 }}>
            ÃœrÃ¼nler
          </Typography>
          <Typography color="text.secondary">Model kartlarÄ±, renk-beden daÄŸÄ±lÄ±mÄ± ve toplam stok gÃ¶rÃ¼nÃ¼mÃ¼.</Typography>
        </Box>

        <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate('/products/new')}>
          ÃœrÃ¼n Ekle
        </Button>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Model kodu, Ã¼rÃ¼n adÄ±, marka, kategori, renk veya beden ara"
            label="HÄ±zlÄ± Arama"
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

          {loading ? (
            <Stack sx={{ minHeight: 280, alignItems: 'center', justifyContent: 'center' }} spacing={2}>
              <CircularProgress size={32} />
              <Typography color="text.secondary">ÃœrÃ¼n kartlarÄ± yÃ¼kleniyor...</Typography>
            </Stack>
          ) : visibleProducts.length === 0 ? (
            <Stack sx={{ minHeight: 280, alignItems: 'center', justifyContent: 'center' }} spacing={1}>
              <Typography variant="h6" sx={{ fontWeight: 800 }}>
                {search.trim() ? 'AramanÄ±za uygun Ã¼rÃ¼n bulunamadÄ±.' : 'HenÃ¼z Ã¼rÃ¼n kaydÄ± yok.'}
              </Typography>
              <Typography color="text.secondary">Yeni bir Ã¼rÃ¼n kartÄ± oluÅŸturarak baÅŸlayabilirsiniz.</Typography>
            </Stack>
          ) : (
            <Stack spacing={1.5}>
              {visibleProducts.map((product) => (
                <ProductCard
                  key={product.id}
                  product={product}
                  onEdit={() => navigate(`/products/${product.id}`)}
                  onDelete={() => void handleDelete(product)}
                />
              ))}
            </Stack>
          )}

          {products.length > pageSize && (
            <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
              <Typography variant="body2" color="text.secondary">
                {numberText(products.length)} Ã¼rÃ¼n kartÄ±
              </Typography>
              <Pagination count={pageCount} page={page} onChange={(_, value) => setPage(value)} color="primary" />
            </Stack>
          )}
        </Stack>
      </Paper>

      <Snackbar open={!!success} autoHideDuration={3000} onClose={() => setSuccess(null)} message={success} />
    </Stack>
  )
}

function ProductCard({ product, onEdit, onDelete }: { product: Product; onEdit: () => void; onDelete: () => void }) {
  return (
    <Accordion
      disableGutters
      sx={{
        border: 1,
        borderColor: 'divider',
        borderRadius: 1,
        overflow: 'hidden',
        transition: 'border-color 160ms ease, box-shadow 160ms ease, transform 160ms ease',
        '&:before': { display: 'none' },
        '&:hover': {
          borderColor: 'primary.light',
          boxShadow: 2,
          transform: 'translateY(-1px)',
        },
      }}
    >
      <AccordionSummary expandIcon={<ExpandMoreIcon />} sx={{ p: { xs: 1.5, md: 2 } }}>
        <Stack
          direction={{ xs: 'column', md: 'row' }}
          spacing={2}
          sx={{ width: '100%', alignItems: { xs: 'stretch', md: 'center' }, pr: 1 }}
        >
          <Avatar
            variant="rounded"
            src={product.imageUrl ?? undefined}
            alt={product.productName}
            sx={{ width: 76, height: 96, bgcolor: 'grey.100', color: 'text.secondary', fontWeight: 800 }}
          >
            {product.productName.slice(0, 1).toLocaleUpperCase('tr-TR')}
          </Avatar>

          <Stack spacing={1} sx={{ flex: 1, minWidth: 0 }}>
            <Box>
              <Typography variant="h6" sx={{ fontWeight: 900, lineHeight: 1.1 }}>
                {product.productCode}
              </Typography>
              <Typography sx={{ fontWeight: 700 }}>{product.productName}</Typography>
            </Box>
            <Stack direction="row" spacing={1} sx={{ flexWrap: 'wrap', rowGap: 1 }}>
              <Chip label={product.brand ?? 'Marka yok'} size="small" />
              <Chip label={product.category ?? 'Kategori yok'} size="small" />
              <Chip label={product.season ?? 'Sezon yok'} size="small" />
            </Stack>
          </Stack>

          <Box sx={{ display: 'grid', gap: 1, gridTemplateColumns: { xs: '1fr 1fr', sm: 'repeat(4, 1fr)' }, minWidth: { md: 420 } }}>
            <Metric label="Toplam Renk" value={product.colorCount} />
            <Metric label="Toplam Beden" value={product.sizeCount} />
            <Metric label="Toplam Varyant" value={product.variantCount} />
            <Metric label="Toplam Stok" value={product.totalStock} />
          </Box>

          <Stack direction="row" spacing={0.5} sx={{ justifyContent: { xs: 'flex-end', md: 'center' } }}>
            <Tooltip title="ÃœrÃ¼nÃ¼ dÃ¼zenle">
              <IconButton
                size="small"
                onClick={(event) => {
                  event.stopPropagation()
                  onEdit()
                }}
                aria-label="ÃœrÃ¼nÃ¼ dÃ¼zenle"
              >
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="ÃœrÃ¼nÃ¼ sil">
              <IconButton
                size="small"
                color="error"
                onClick={(event) => {
                  event.stopPropagation()
                  onDelete()
                }}
                aria-label="ÃœrÃ¼nÃ¼ sil"
              >
                <DeleteOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          </Stack>
        </Stack>
      </AccordionSummary>
      <AccordionDetails sx={{ p: 0, bgcolor: 'background.default' }}>
        <Divider />
        {product.colorGroups.length === 0 ? (
          <Typography sx={{ p: 2 }} color="text.secondary">
            Bu Ã¼rÃ¼n iÃ§in henÃ¼z varyant yok.
          </Typography>
        ) : (
          <Stack spacing={1} sx={{ p: { xs: 1.5, md: 2 } }}>
            {product.colorGroups.map((group) => (
              <Accordion key={group.colorId} variant="outlined" disableGutters sx={{ borderRadius: 1, '&:before': { display: 'none' } }}>
                <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                  <Stack direction="row" spacing={1.5} sx={{ alignItems: 'center', width: '100%' }}>
                    <Typography sx={{ fontWeight: 800, flex: 1 }}>
                      {group.color || 'Renk yok'} (Toplam {numberText(group.totalStock)})
                    </Typography>
                    <Chip label={`${group.sizes.length} beden`} size="small" />
                  </Stack>
                </AccordionSummary>
                <AccordionDetails sx={{ pt: 0 }}>
                  <Box
                    sx={{
                      display: 'grid',
                      gap: 1,
                      gridTemplateColumns: {
                        xs: '1fr',
                        sm: 'repeat(2, minmax(0, 1fr))',
                        lg: 'repeat(3, minmax(0, 1fr))',
                      },
                    }}
                  >
                    {group.sizes.map((variant) => (
                      <Box
                        key={variant.variantId}
                        sx={{
                          border: 1,
                          borderColor: 'divider',
                          borderRadius: 1,
                          bgcolor: 'background.paper',
                          p: 1.25,
                        }}
                      >
                        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', justifyContent: 'space-between' }}>
                          <Chip label={variant.size || 'Beden yok'} size="small" color="primary" />
                          <Typography sx={{ fontWeight: 900 }}>{numberText(variant.stock)} stok</Typography>
                        </Stack>
                        <Stack spacing={0.5} sx={{ mt: 1 }}>
                          <VariantInfo label="Alış Fiyatı" value={moneyText(variant.purchasePrice)} />
                          <VariantInfo label="Satış Fiyatı" value={moneyText(variant.salesPrice)} />
                          <VariantInfo label="Barkod" value={variant.barcode || '-'} />
                        </Stack>
                        <Button size="small" onClick={onEdit} sx={{ mt: 1 }}>
                          Varyantı Düzenle
                        </Button>
                      </Box>
                    ))}
                  </Box>
                </AccordionDetails>
              </Accordion>
            ))}
          </Stack>
        )}
      </AccordionDetails>
    </Accordion>
  )
}

function VariantInfo({ label, value }: { label: string; value: string }) {
  return (
    <Stack direction="row" spacing={1} sx={{ justifyContent: 'space-between', alignItems: 'baseline' }}>
      <Typography variant="caption" color="text.secondary">
        {label}
      </Typography>
      <Typography variant="body2" sx={{ fontWeight: 700, textAlign: 'right', overflowWrap: 'anywhere' }}>
        {value}
      </Typography>
    </Stack>
  )
}

function Metric({ label, value }: { label: string; value: number }) {
  return (
    <Box sx={{ border: 1, borderColor: 'divider', borderRadius: 1, px: 1.25, py: 1 }}>
      <Typography variant="caption" color="text.secondary">
        {label}
      </Typography>
      <Typography sx={{ fontWeight: 900, lineHeight: 1.1 }}>{numberText(value)}</Typography>
    </Box>
  )
}
