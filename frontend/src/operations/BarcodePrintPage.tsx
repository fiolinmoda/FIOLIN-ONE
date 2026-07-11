import { useCallback, useMemo, useState } from 'react'
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Alert,
  Avatar,
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  CircularProgress,
  Divider,
  IconButton,
  InputAdornment,
  Paper,
  Stack,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material'
import AddOutlinedIcon from '@mui/icons-material/AddOutlined'
import DeleteOutlineOutlinedIcon from '@mui/icons-material/DeleteOutlineOutlined'
import ExpandMoreIcon from '@mui/icons-material/ExpandMore'
import LocalPrintshopOutlinedIcon from '@mui/icons-material/LocalPrintshopOutlined'
import SearchOutlinedIcon from '@mui/icons-material/SearchOutlined'
import { getProducts } from '../products/api'
import type { Product, ProductSizeVariant } from '../products/types'
import {
  BrowserBarcodePrintService,
  type BarcodePrintItem,
  type BarcodePrintLabel,
  type BarcodePrintService,
} from './barcodePrintService'

function numberText(value: number) {
  return value.toLocaleString('tr-TR')
}

function itemKey(modelCode: string, variant: ProductSizeVariant) {
  return `${modelCode}-${variant.variantId}`
}

export function BarcodePrintPage() {
  const printService = useMemo<BarcodePrintService>(() => new BrowserBarcodePrintService(), [])
  const [search, setSearch] = useState('')
  const [products, setProducts] = useState<Product[]>([])
  const [quantities, setQuantities] = useState<Record<string, number>>({})
  const [printItems, setPrintItems] = useState<BarcodePrintItem[]>([])
  const [printLabels, setPrintLabels] = useState<BarcodePrintLabel[]>([])
  const [loading, setLoading] = useState(false)
  const [printing, setPrinting] = useState(false)
  const [error, setError] = useState('')

  const handleSearch = useCallback(async () => {
    if (!search.trim()) {
      setProducts([])
      return
    }

    setLoading(true)
    setError('')

    try {
      const result = await getProducts(search)
      setProducts(result)
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : 'Ürünler aranamadı.')
    } finally {
      setLoading(false)
    }
  }, [search])

  function getQuantity(key: string) {
    return quantities[key] ?? 1
  }

  function updateQuantity(key: string, value: number) {
    setQuantities((current) => ({ ...current, [key]: Math.max(1, value || 1) }))
  }

  function addToPrintList(product: Product, color: string, variant: ProductSizeVariant) {
    const key = itemKey(product.modelCode, variant)
    const quantity = getQuantity(key)

    setPrintItems((current) => {
      const existing = current.find((item) => item.id === variant.variantId)

      if (existing) {
        return current.map((item) =>
          item.id === variant.variantId ? { ...item, quantity: item.quantity + quantity } : item,
        )
      }

      return [
        ...current,
        {
          id: variant.variantId,
          modelCode: product.modelCode,
          productName: product.productName,
          color,
          size: variant.size,
          barcode: variant.barcode,
          quantity,
        },
      ]
    })
  }

  function removeFromPrintList(id: string) {
    setPrintItems((current) => current.filter((item) => item.id !== id))
  }

  async function handlePrint() {
    if (printItems.length === 0) {
      setError('Yazdırılacak etiket bulunmuyor.')
      return
    }

    setPrinting(true)
    setError('')

    try {
      const labels = await printService.prepareLabels(printItems)
      setPrintLabels(labels)
      window.setTimeout(() => {
        printService.print()
        setPrinting(false)
      }, 100)
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : 'Etiketler hazırlanamadı.')
      setPrinting(false)
    }
  }

  return (
    <Stack spacing={3}>
      <style>{printStyles}</style>
      {error && <Alert severity="error" onClose={() => setError('')}>{error}</Alert>}

      <Paper className="screen-only" variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 2 }}>
        <Stack spacing={2}>
          <Typography variant="h6" sx={{ fontWeight: 800 }}>Barkod / Model Kodu Ara</Typography>
          <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', md: '1fr auto' } }}>
            <TextField
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              onKeyDown={(event) => {
                if (event.key === 'Enter') {
                  event.preventDefault()
                  void handleSearch()
                }
              }}
              placeholder="Model kodu, barkod veya ürün adı yazın"
              slotProps={{
                input: {
                  startAdornment: (
                    <InputAdornment position="start">
                      <SearchOutlinedIcon />
                    </InputAdornment>
                  ),
                },
              }}
              fullWidth
              autoFocus
            />
            <Button variant="contained" size="large" disabled={loading} onClick={() => void handleSearch()} startIcon={<SearchOutlinedIcon />}>
              Ara
            </Button>
          </Box>
        </Stack>
      </Paper>

      <Box className="screen-only" sx={{ display: 'grid', gap: 3, gridTemplateColumns: { xs: '1fr', lg: 'minmax(0, 1fr) 360px' } }}>
        <Stack spacing={2.5}>
          {loading && <CircularProgress />}
          {!loading && search.trim() && products.length === 0 && (
            <Paper variant="outlined" sx={{ p: 3, borderRadius: 2 }}>
              <Typography color="text.secondary">Arama sonucunda ürün bulunamadı.</Typography>
            </Paper>
          )}

          {products.map((product) => (
            <ProductPrintCard
              key={product.id}
              product={product}
              getQuantity={getQuantity}
              updateQuantity={updateQuantity}
              addToPrintList={addToPrintList}
            />
          ))}
        </Stack>

        <Paper variant="outlined" sx={{ p: 2.5, borderRadius: 2, alignSelf: 'start', position: { lg: 'sticky' }, top: 96 }}>
          <Stack spacing={2}>
            <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
              <Typography variant="h6" sx={{ fontWeight: 800 }}>Yazdırma Listesi</Typography>
              <Chip label={`${numberText(printItems.reduce((sum, item) => sum + item.quantity, 0))} etiket`} />
            </Stack>
            <Divider />

            {printItems.length === 0 ? (
              <Typography color="text.secondary">Henüz listeye etiket eklenmedi.</Typography>
            ) : (
              <Stack spacing={1}>
                {printItems.map((item) => (
                  <Paper key={item.id} variant="outlined" sx={{ p: 1.5, borderRadius: 1 }}>
                    <Stack direction="row" spacing={1.5} sx={{ justifyContent: 'space-between', alignItems: 'center' }}>
                      <Box>
                        <Typography sx={{ fontWeight: 800 }}>{item.modelCode}</Typography>
                        <Typography variant="body2">{item.color} / {item.size}</Typography>
                        <Typography variant="body2" color="text.secondary">{numberText(item.quantity)} adet</Typography>
                      </Box>
                      <Tooltip title="Listeden çıkar">
                        <IconButton onClick={() => removeFromPrintList(item.id)}>
                          <DeleteOutlineOutlinedIcon />
                        </IconButton>
                      </Tooltip>
                    </Stack>
                  </Paper>
                ))}
              </Stack>
            )}

            <Button
              variant="contained"
              size="large"
              disabled={printing || printItems.length === 0}
              onClick={() => void handlePrint()}
              startIcon={<LocalPrintshopOutlinedIcon />}
            >
              Yazdır
            </Button>
          </Stack>
        </Paper>
      </Box>

      <Box className="print-area">
        {printLabels.map((label) => (
          <BarcodeLabel key={label.id} label={label} />
        ))}
      </Box>
    </Stack>
  )
}

type ProductPrintCardProps = {
  product: Product
  getQuantity: (key: string) => number
  updateQuantity: (key: string, value: number) => void
  addToPrintList: (product: Product, color: string, variant: ProductSizeVariant) => void
}

function ProductPrintCard({ product, getQuantity, updateQuantity, addToPrintList }: ProductPrintCardProps) {
  return (
    <Card variant="outlined" sx={{ borderRadius: 2 }}>
      <CardContent>
        <Stack spacing={2}>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ alignItems: { sm: 'center' } }}>
            <Avatar src={product.imageUrl ?? undefined} variant="rounded" sx={{ width: 96, height: 120, borderRadius: 1 }} />
            <Box sx={{ minWidth: 0 }}>
              <Typography variant="h5" sx={{ fontWeight: 900 }}>{product.modelCode}</Typography>
              <Typography color="text.secondary">{product.productName}</Typography>
            </Box>
          </Stack>

          <Stack spacing={1}>
            {product.colorGroups.map((colorGroup) => (
              <Accordion key={colorGroup.colorId} disableGutters>
                <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                  <Stack direction="row" spacing={1.5} sx={{ alignItems: 'center' }}>
                    <Typography sx={{ fontWeight: 800 }}>{colorGroup.color}</Typography>
                    <Chip size="small" label={`${numberText(colorGroup.totalStock)} stok`} />
                  </Stack>
                </AccordionSummary>
                <AccordionDetails>
                  <Box sx={{ display: 'grid', gap: 1.5, gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))' } }}>
                    {colorGroup.sizes.map((variant) => {
                      const key = itemKey(product.modelCode, variant)

                      return (
                        <Paper key={variant.variantId} variant="outlined" sx={{ p: 1.5, borderRadius: 1 }}>
                          <Stack spacing={1.25}>
                            <Stack direction="row" sx={{ justifyContent: 'space-between', gap: 1 }}>
                              <Typography variant="h6" sx={{ fontWeight: 900 }}>{variant.size}</Typography>
                              <Chip size="small" label={`${numberText(variant.stock)} stok`} />
                            </Stack>
                            <Info label="Barkod" value={variant.barcode} />
                            <Box sx={{ display: 'grid', gap: 1, gridTemplateColumns: '1fr 1fr' }}>
                              <Info label="Raf" value={variant.shelf} />
                              <Info label="Koli" value={variant.box} />
                            </Box>
                            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                              <TextField
                                label="Etiket Adedi"
                                type="number"
                                value={getQuantity(key)}
                                onChange={(event) => updateQuantity(key, Number(event.target.value))}
                                size="small"
                                sx={{ maxWidth: 140 }}
                              />
                              <Button variant="outlined" onClick={() => addToPrintList(product, colorGroup.color, variant)} startIcon={<AddOutlinedIcon />}>
                                Listeye Ekle
                              </Button>
                            </Stack>
                          </Stack>
                        </Paper>
                      )
                    })}
                  </Box>
                </AccordionDetails>
              </Accordion>
            ))}
          </Stack>
        </Stack>
      </CardContent>
    </Card>
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

function BarcodeLabel({ label }: { label: BarcodePrintLabel }) {
  return (
    <Box className="barcode-label">
      <img src={label.qrDataUrl} alt={label.barcode} className="barcode-label-qr" />
      <Box className="barcode-label-text">
        <strong>{label.modelCode}</strong>
        <span>{label.color} / {label.size}</span>
        <span>{label.productName}</span>
        <span>{label.barcode}</span>
      </Box>
    </Box>
  )
}

const printStyles = `
  .print-area {
    display: none;
  }

  @media print {
    @page {
      size: 70mm 40mm;
      margin: 0;
    }

    body * {
      visibility: hidden !important;
    }

    .print-area,
    .print-area * {
      visibility: visible !important;
    }

    .screen-only {
      display: none !important;
    }

    .print-area {
      display: block !important;
      position: absolute;
      inset: 0 auto auto 0;
      margin: 0;
      padding: 0;
      background: #fff;
    }

    .barcode-label {
      width: 70mm;
      height: 40mm;
      box-sizing: border-box;
      display: grid;
      grid-template-columns: 32mm 1fr;
      gap: 2mm;
      align-items: center;
      padding: 2mm;
      page-break-after: always;
      break-after: page;
      overflow: hidden;
      background: #fff;
      color: #000;
      font-family: Arial, sans-serif;
    }

    .barcode-label:last-child {
      page-break-after: auto;
      break-after: auto;
    }

    .barcode-label-qr {
      width: 30mm;
      height: 30mm;
      object-fit: contain;
    }

    .barcode-label-text {
      display: grid;
      gap: 1.1mm;
      font-size: 8pt;
      line-height: 1.08;
      min-width: 0;
    }

    .barcode-label-text strong {
      font-size: 12pt;
      line-height: 1;
    }

    .barcode-label-text span {
      overflow-wrap: anywhere;
    }
  }
`
