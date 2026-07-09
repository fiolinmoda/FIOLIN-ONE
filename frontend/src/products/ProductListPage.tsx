import { useCallback, useEffect, useMemo, useState } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import {
  Alert,
  Box,
  Button,
  IconButton,
  InputAdornment,
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
import { deleteProduct, getProducts } from './api'
import type { Product } from './types'
import { confirmDelete, trStatus } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

type NavigationState = {
  message?: string
}

export function ProductListPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const [products, setProducts] = useState<Product[]>([])
  const [search, setSearch] = useState('')
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
    } catch (exception) {
      setError(toUserMessage(exception, 'Ürünler yüklenemedi. Lütfen tekrar deneyiniz.'))
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

  const handleDelete = useCallback(
    async (product: Product) => {
      const confirmed = confirmDelete(`${product.productCode} - ${product.productName}`)

      if (!confirmed) {
        return
      }

      setError(null)

      try {
        await deleteProduct(product.id)
        setSuccess('Ürün silindi.')
        await loadProducts()
      } catch (exception) {
        setError(toUserMessage(exception, 'Ürün silinemedi. Ürün üretim veya stok kayıtlarında kullanılıyor olabilir.'))
      }
    },
    [loadProducts],
  )

  const columns = useMemo<GridColDef<Product>[]>(
    () => [
      { field: 'productCode', headerName: 'Ürün Kodu', minWidth: 150, flex: 0.7 },
      { field: 'productName', headerName: 'Ürün Adı', minWidth: 240, flex: 1.2 },
      { field: 'brand', headerName: 'Marka', minWidth: 140, flex: 0.8, valueGetter: (_, row) => row.brand ?? '-' },
      { field: 'category', headerName: 'Kategori', minWidth: 150, flex: 0.8, valueGetter: (_, row) => row.category ?? '-' },
      { field: 'season', headerName: 'Sezon', minWidth: 120, flex: 0.6, valueGetter: (_, row) => row.season ?? '-' },
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
            <Tooltip title="Ürünü düzenle">
              <IconButton size="small" onClick={() => navigate(`/products/${row.id}`)} aria-label="Ürünü düzenle">
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Ürünü sil">
              <IconButton size="small" color="error" onClick={() => void handleDelete(row)} aria-label="Ürünü sil">
                <DeleteOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          </Stack>
        ),
      },
    ],
    [handleDelete, navigate],
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
            Ürünler
          </Typography>
          <Typography color="text.secondary">Model kartları ve satışa açılacak varyantların başlangıç noktası.</Typography>
        </Box>

        <Button variant="contained" startIcon={<AddIcon />} onClick={() => navigate('/products/new')}>
          Ürün Ekle
        </Button>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Ürün kodu, adı, marka veya kategori ara"
            label="Hızlı Arama"
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
              rows={products}
              columns={columns}
              loading={loading}
              disableRowSelectionOnClick
              onRowDoubleClick={({ row }) => navigate(`/products/${row.id}`)}
              pageSizeOptions={[10, 25, 50]}
              initialState={{
                pagination: {
                  paginationModel: { pageSize: 10 },
                },
              }}
              localeText={{
                noRowsLabel: search.trim() ? 'Aramanıza uygun ürün bulunamadı.' : 'Henüz ürün kaydı yok.',
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
      <Snackbar open={!!success} autoHideDuration={3000} onClose={() => setSuccess(null)} message={success} />
    </Stack>
  )
}
