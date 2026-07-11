import { useEffect, useMemo, useRef, useState } from 'react'
import {
  Alert,
  Autocomplete,
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  Divider,
  InputAdornment,
  Paper,
  Snackbar,
  Stack,
  TextField,
  Typography,
} from '@mui/material'
import Inventory2OutlinedIcon from '@mui/icons-material/Inventory2Outlined'
import QrCodeScannerOutlinedIcon from '@mui/icons-material/QrCodeScannerOutlined'
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined'
import SearchOutlinedIcon from '@mui/icons-material/SearchOutlined'
import { getProducts } from '../products/api'
import type { Product, ProductColorGroup, ProductSizeVariant } from '../products/types'
import { getSuppliers } from '../purchasing/api'
import type { Supplier } from '../purchasing/types'
import {
  createGoodsReceiptOperation,
  findVariantByBarcode,
  getGoodsReceiptVariant,
  type GoodsReceiptVariant,
} from './goodsReceiptApi'

const today = new Date().toISOString().slice(0, 10)

function variantLabel(variant: GoodsReceiptVariant | null) {
  return variant ? `${variant.modelCode} / ${variant.color} / ${variant.size}` : 'Ürün seçilmedi'
}

export function GoodsReceiptOperationPage() {
  const barcodeInputRef = useRef<HTMLInputElement>(null)
  const [suppliers, setSuppliers] = useState<Supplier[]>([])
  const [supplier, setSupplier] = useState<Supplier | null>(null)
  const [transactionDate, setTransactionDate] = useState(today)
  const [description, setDescription] = useState('')
  const [barcode, setBarcode] = useState('')
  const [modelSearch, setModelSearch] = useState('')
  const [products, setProducts] = useState<Product[]>([])
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null)
  const [selectedColor, setSelectedColor] = useState<ProductColorGroup | null>(null)
  const [selectedSize, setSelectedSize] = useState<ProductSizeVariant | null>(null)
  const [variant, setVariant] = useState<GoodsReceiptVariant | null>(null)
  const [purchasePrice, setPurchasePrice] = useState('')
  const [quantity, setQuantity] = useState('1')
  const [shelf, setShelf] = useState('')
  const [box, setBox] = useState('')
  const [loading, setLoading] = useState(false)
  const [lookupLoading, setLookupLoading] = useState(false)
  const [success, setSuccess] = useState('')
  const [error, setError] = useState('')

  useEffect(() => {
    void getSuppliers()
      .then((result) => setSuppliers(result.items.filter((item) => item.active)))
      .catch((exception) => setError(exception instanceof Error ? exception.message : 'Tedarikçiler yüklenemedi.'))
  }, [])

  useEffect(() => {
    const timer = window.setTimeout(() => {
      if (modelSearch.trim().length < 2) {
        setProducts([])
        return
      }

      void getProducts(modelSearch)
        .then(setProducts)
        .catch((exception) => setError(exception instanceof Error ? exception.message : 'Ürünler aranamadı.'))
    }, 250)

    return () => window.clearTimeout(timer)
  }, [modelSearch])

  const colorOptions = selectedProduct?.colorGroups ?? []
  const sizeOptions = selectedColor?.sizes ?? []

  const canSave = Boolean(supplier && variant && Number(quantity) > 0 && Number(purchasePrice) >= 0)

  function applyVariant(nextVariant: GoodsReceiptVariant) {
    setVariant(nextVariant)
    setBarcode(nextVariant.barcode)
    setPurchasePrice(String(nextVariant.lastPurchasePrice || ''))
    setShelf(nextVariant.shelf ?? '')
    setBox(nextVariant.box ?? '')
  }

  async function handleBarcodeLookup() {
    const value = barcode.trim()

    if (!value) {
      setError('Barkod okutunuz veya yazınız.')
      barcodeInputRef.current?.focus()
      return
    }

    setLookupLoading(true)
    setError('')

    try {
      const found = await findVariantByBarcode(value)
      applyVariant(found)
    } catch {
      setVariant(null)
      setError('Barkod sistemde bulunamadı. Model kodu ile arama yapabilirsiniz.')
    } finally {
      setLookupLoading(false)
    }
  }

  async function handleSizeSelect(size: ProductSizeVariant | null) {
    setSelectedSize(size)

    if (!size) {
      setVariant(null)
      return
    }

    setLookupLoading(true)
    setError('')

    try {
      const found = await getGoodsReceiptVariant(size.variantId)
      applyVariant(found)
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : 'Varyant bilgisi yüklenemedi.')
    } finally {
      setLookupLoading(false)
    }
  }

  function clearProductSelection() {
    setBarcode('')
    setModelSearch('')
    setProducts([])
    setSelectedProduct(null)
    setSelectedColor(null)
    setSelectedSize(null)
    setVariant(null)
    setPurchasePrice('')
    setQuantity('1')
    setShelf('')
    setBox('')
    window.setTimeout(() => barcodeInputRef.current?.focus(), 0)
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!supplier) {
      setError('Lütfen tedarikçi seçiniz.')
      return
    }

    if (!variant) {
      setError('Lütfen barkod okutarak veya model seçerek ürün belirleyiniz.')
      barcodeInputRef.current?.focus()
      return
    }

    if (Number(quantity) <= 0) {
      setError('Gelen adet 0’dan büyük olmalıdır.')
      return
    }

    setLoading(true)
    setError('')

    try {
      await createGoodsReceiptOperation({
        supplierId: supplier.id,
        productVariantId: variant.productVariantId,
        transactionDate,
        description: description.trim() || null,
        purchasePrice: Number(purchasePrice || 0),
        quantity: Number(quantity),
        shelf: shelf.trim() || null,
        box: box.trim() || null,
      })
      setSuccess('Mal kabul başarıyla kaydedildi.')
      clearProductSelection()
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : 'Bu mal kabul kaydedilemedi.')
    } finally {
      setLoading(false)
    }
  }

  const productOptionLabel = useMemo(
    () => (product: Product) => `${product.modelCode} - ${product.productName}`,
    [],
  )

  return (
    <Box component="form" onSubmit={handleSubmit}>
      <Stack spacing={3}>
        {error && <Alert severity="error" onClose={() => setError('')}>{error}</Alert>}

        <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 2 }}>
          <Stack spacing={2.5}>
            <Stack direction="row" spacing={1.5} sx={{ alignItems: 'center' }}>
              <Inventory2OutlinedIcon color="primary" />
              <Typography variant="h6" sx={{ fontWeight: 800 }}>Mal Kabul</Typography>
            </Stack>

            <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', md: '1.3fr 0.7fr' } }}>
              <Autocomplete
                options={suppliers}
                value={supplier}
                onChange={(_, value) => setSupplier(value)}
                getOptionLabel={(option) => option.supplierName}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                renderInput={(params) => (
                  <TextField {...params} label="Tedarikçi *" size="medium" autoFocus />
                )}
              />
              <TextField
                label="İşlem Tarihi *"
                type="date"
                value={transactionDate}
                onChange={(event) => setTransactionDate(event.target.value)}
                slotProps={{ inputLabel: { shrink: true } }}
                fullWidth
              />
            </Box>

            <TextField
              label="Açıklama"
              value={description}
              onChange={(event) => setDescription(event.target.value)}
              multiline
              minRows={2}
              fullWidth
            />
          </Stack>
        </Paper>

        <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 2 }}>
          <Stack spacing={2.5}>
            <Typography variant="h6" sx={{ fontWeight: 800 }}>Ürün Ekleme</Typography>

            <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', md: '1fr auto' } }}>
              <TextField
                inputRef={barcodeInputRef}
                label="Barkod okut"
                value={barcode}
                onChange={(event) => setBarcode(event.target.value)}
                onKeyDown={(event) => {
                  if (event.key === 'Enter') {
                    event.preventDefault()
                    void handleBarcodeLookup()
                  }
                }}
                placeholder="Barkodu okutun ve Enter'a basın"
                slotProps={{
                  input: {
                    startAdornment: (
                      <InputAdornment position="start">
                        <QrCodeScannerOutlinedIcon />
                      </InputAdornment>
                    ),
                  },
                }}
                fullWidth
              />
              <Button
                variant="contained"
                size="large"
                onClick={() => void handleBarcodeLookup()}
                disabled={lookupLoading}
                startIcon={<SearchOutlinedIcon />}
              >
                Bul
              </Button>
            </Box>

            <Divider>veya model kodu ile ara</Divider>

            <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', md: '1.2fr 0.8fr 0.8fr' } }}>
              <Autocomplete
                options={products}
                value={selectedProduct}
                inputValue={modelSearch}
                onInputChange={(_, value) => setModelSearch(value)}
                onChange={(_, value) => {
                  setSelectedProduct(value)
                  setSelectedColor(null)
                  setSelectedSize(null)
                  setVariant(null)
                }}
                getOptionLabel={productOptionLabel}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                renderInput={(params) => <TextField {...params} label="Model Kodu" placeholder="En az 2 karakter yazın" />}
              />
              <Autocomplete
                options={colorOptions}
                value={selectedColor}
                onChange={(_, value) => {
                  setSelectedColor(value)
                  setSelectedSize(null)
                  setVariant(null)
                }}
                getOptionLabel={(option) => option.color}
                isOptionEqualToValue={(option, value) => option.colorId === value.colorId}
                disabled={!selectedProduct}
                renderInput={(params) => <TextField {...params} label="Renk" />}
              />
              <Autocomplete
                options={sizeOptions}
                value={selectedSize}
                onChange={(_, value) => void handleSizeSelect(value)}
                getOptionLabel={(option) => option.size}
                isOptionEqualToValue={(option, value) => option.variantId === value.variantId}
                disabled={!selectedColor}
                renderInput={(params) => <TextField {...params} label="Beden" />}
              />
            </Box>
          </Stack>
        </Paper>

        <Card variant="outlined" sx={{ borderRadius: 2 }}>
          <CardContent>
            <Stack spacing={2.5}>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={1.5} sx={{ justifyContent: 'space-between' }}>
                <Box>
                  <Typography variant="overline" color="text.secondary">Seçilen Ürün</Typography>
                  <Typography variant="h6" sx={{ fontWeight: 800 }}>{variantLabel(variant)}</Typography>
                </Box>
                {variant && <Chip label={`Mevcut Stok: ${variant.stock}`} color="primary" variant="outlined" />}
              </Stack>

              <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr 1fr', md: 'repeat(6, 1fr)' } }}>
                <Info label="Model Kodu" value={variant?.modelCode} />
                <Info label="Ürün Adı" value={variant?.productName} />
                <Info label="Renk" value={variant?.color} />
                <Info label="Beden" value={variant?.size} />
                <Info label="Barkod" value={variant?.barcode} />
                <Info label="Son Alış Fiyatı" value={variant ? `${variant.lastPurchasePrice}` : undefined} />
              </Box>

              <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: 'repeat(4, 1fr)' } }}>
                <TextField label="Alış Fiyatı *" type="number" value={purchasePrice} onChange={(event) => setPurchasePrice(event.target.value)} fullWidth />
                <TextField label="Gelen Adet *" type="number" value={quantity} onChange={(event) => setQuantity(event.target.value)} fullWidth />
                <TextField label="Raf" value={shelf} onChange={(event) => setShelf(event.target.value)} fullWidth />
                <TextField label="Koli" value={box} onChange={(event) => setBox(event.target.value)} fullWidth />
              </Box>

              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5} sx={{ justifyContent: 'flex-end' }}>
                <Button type="button" variant="outlined" onClick={clearProductSelection}>Temizle</Button>
                <Button type="submit" variant="contained" size="large" disabled={!canSave || loading} startIcon={<SaveOutlinedIcon />}>
                  Kaydet
                </Button>
              </Stack>
            </Stack>
          </CardContent>
        </Card>
      </Stack>

      <Snackbar open={Boolean(success)} autoHideDuration={3000} onClose={() => setSuccess('')}>
        <Alert severity="success" variant="filled" onClose={() => setSuccess('')}>{success}</Alert>
      </Snackbar>
    </Box>
  )
}

function Info({ label, value }: { label: string; value?: string | null }) {
  return (
    <Box>
      <Typography variant="caption" color="text.secondary">{label}</Typography>
      <Typography sx={{ fontWeight: 700, wordBreak: 'break-word' }}>{value || '-'}</Typography>
    </Box>
  )
}
