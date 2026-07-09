import { useCallback, useEffect, useMemo, useState } from 'react'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import { Alert, Box, Chip, InputAdornment, Paper, Stack, TextField, Typography } from '@mui/material'
import SearchIcon from '@mui/icons-material/Search'
import { getProductVariants, getProducts } from '../products/api'
import type { ProductVariant } from '../products/types'
import { toUserMessage } from '../common/apiClient'
import { trStatus } from '../common/uiText'

type ProductStockRow = {
  id: string
  productCode: string
  productName: string
  color: string
  size: string
  barcode: string
  trendyolSku: string | null
  stock: number
  status: string
}

function toRow(productCode: string, productName: string, variant: ProductVariant): ProductStockRow {
  return {
    id: variant.id,
    productCode,
    productName,
    color: variant.color,
    size: variant.size,
    barcode: variant.barcode,
    trendyolSku: variant.trendyolSku,
    stock: variant.stock,
    status: variant.status,
  }
}

export function ProductStockPage() {
  const [rows, setRows] = useState<ProductStockRow[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const loadStock = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const products = await getProducts('')
      const variantsByProduct = await Promise.all(
        products.map(async (product) => ({
          product,
          variants: await getProductVariants(product.id),
        })),
      )

      setRows(
        variantsByProduct.flatMap(({ product, variants }) =>
          variants.map((variant) => toRow(product.productCode, product.productName, variant)),
        ),
      )
    } catch (exception) {
      setError(toUserMessage(exception, 'Ürün stoğu yüklenemedi.'))
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    const handle = window.setTimeout(() => {
      void loadStock()
    }, 250)

    return () => window.clearTimeout(handle)
  }, [loadStock])

  const columns = useMemo<GridColDef<ProductStockRow>[]>(
    () => [
      { field: 'productCode', headerName: 'Ürün Kodu', minWidth: 150, flex: 0.7 },
      { field: 'productName', headerName: 'Ürün', minWidth: 220, flex: 1.2 },
      { field: 'color', headerName: 'Renk', minWidth: 130, flex: 0.6 },
      { field: 'size', headerName: 'Beden', minWidth: 110, flex: 0.5 },
      { field: 'barcode', headerName: 'Barkod', minWidth: 160, flex: 0.8 },
      { field: 'trendyolSku', headerName: 'Trendyol SKU', minWidth: 160, flex: 0.8, valueGetter: (value: string | null) => value || '-' },
      { field: 'stock', headerName: 'Stok', type: 'number', minWidth: 110, flex: 0.5 },
      {
        field: 'status',
        headerName: 'Durum',
        minWidth: 130,
        flex: 0.6,
        renderCell: ({ value }) => <Chip label={trStatus(String(value))} size="small" color={value === 'Active' ? 'success' : 'default'} variant="outlined" />,
      },
    ],
    [],
  )

  const filteredRows = useMemo(() => {
    const term = search.trim().toLocaleLowerCase('tr-TR')

    if (!term) {
      return rows
    }

    return rows.filter((row) =>
      [
        row.productCode,
        row.productName,
        row.color,
        row.size,
        row.barcode,
        row.trendyolSku ?? '',
        trStatus(row.status),
      ].some((value) => value.toLocaleLowerCase('tr-TR').includes(term)),
    )
  }, [rows, search])

  return (
    <Stack spacing={3}>
      <Box>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>Ürün Stoğu</Typography>
        <Typography color="text.secondary">Üretim depo girişlerinden oluşan varyant bazlı mamul stokları.</Typography>
      </Box>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Ürün kodu, ürün adı, renk veya barkod ara"
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
          <Box sx={{ width: '100%', minHeight: 500 }}>
            <DataGrid
              rows={filteredRows}
              columns={columns}
              loading={loading}
              disableRowSelectionOnClick
              pageSizeOptions={[10, 25, 50]}
              initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
              localeText={{ noRowsLabel: search.trim() ? 'Aramanıza uygun ürün stoğu bulunamadı.' : 'Henüz ürün stoğu yok.' }}
              sx={{ border: 0, '& .MuiDataGrid-columnHeaders': { bgcolor: 'background.default' } }}
            />
          </Box>
        </Stack>
      </Paper>
    </Stack>
  )
}
