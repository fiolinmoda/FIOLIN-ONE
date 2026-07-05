import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import {
  Alert,
  Box,
  Button,
  IconButton,
  InputAdornment,
  Paper,
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

export function ProductListPage() {
  const navigate = useNavigate()
  const [products, setProducts] = useState<Product[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const loadProducts = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getProducts(search)
      setProducts(data)
    } catch (exception) {
      setError(toUserMessage(exception, 'Ürünler yüklenemedi.'))
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
        await loadProducts()
      } catch (exception) {
        setError(toUserMessage(exception, 'Ürün silinemedi.'))
      }
    },
    [loadProducts],
  )

  const columns = useMemo<GridColDef<Product>[]>(
    () => [
      { field: 'productCode', headerName: 'Ürün Kodu', minWidth: 140, flex: 0.7 },
      { field: 'productName', headerName: 'Ürün Adı', minWidth: 220, flex: 1.2 },
      { field: 'brand', headerName: 'Marka', minWidth: 140, flex: 0.8 },
      { field: 'category', headerName: 'Kategori', minWidth: 150, flex: 0.8 },
      { field: 'season', headerName: 'Sezon', minWidth: 120, flex: 0.6 },
      { field: 'status', headerName: 'Durum', minWidth: 120, flex: 0.6, valueFormatter: (value: string) => trStatus(value) },
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
              <IconButton size="small" onClick={() => navigate(`/products/${row.id}`)}>
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Ürünü sil">
              <IconButton size="small" color="error" onClick={() => void handleDelete(row)}>
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
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>
            Ürün Yönetimi
          </Typography>
          <Typography color="text.secondary">
            Model bazlı ürün kartlarını ve satışa açılacak varyantları yönetin.
          </Typography>
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
            placeholder="Ürün kodu veya adına göre ara"
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
        </Stack>
      </Paper>
    </Stack>
  )
}
