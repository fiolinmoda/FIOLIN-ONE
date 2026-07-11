import { useEffect, useMemo, useRef, useState } from 'react'
import {
  Alert,
  Avatar,
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  InputAdornment,
  LinearProgress,
  List,
  ListItemButton,
  ListItemText,
  Paper,
  Snackbar,
  Stack,
  TextField,
  Typography,
} from '@mui/material'
import CheckCircleOutlineOutlinedIcon from '@mui/icons-material/CheckCircleOutlineOutlined'
import ErrorOutlineOutlinedIcon from '@mui/icons-material/ErrorOutlineOutlined'
import Inventory2OutlinedIcon from '@mui/icons-material/Inventory2Outlined'
import QrCodeScannerOutlinedIcon from '@mui/icons-material/QrCodeScannerOutlined'
import {
  BrowserBeepFeedback,
  KeyboardBarcodeScanner,
  MockOrderSource,
  NoopShipmentService,
  NoopStockService,
  type PickingOrder,
  type PickingOrderItem,
} from './orderPickingServices'

type ScanState = 'idle' | 'success' | 'error'

function numberText(value: number) {
  return value.toLocaleString('tr-TR')
}

function dateText(value: string) {
  return new Date(value).toLocaleDateString('tr-TR')
}

function orderTotals(order: PickingOrder) {
  return {
    productCount: order.items.length,
    totalQuantity: order.items.reduce((total, item) => total + item.requestedQuantity, 0),
  }
}

export function OrderPickingPage() {
  const inputRef = useRef<HTMLInputElement>(null)
  const services = useMemo(() => ({
    orderSource: new MockOrderSource(),
    stockService: new NoopStockService(),
    shipmentService: new NoopShipmentService(),
    scanner: new KeyboardBarcodeScanner(),
    audio: new BrowserBeepFeedback(),
  }), [])

  const [orders, setOrders] = useState<PickingOrder[]>([])
  const [selectedOrderId, setSelectedOrderId] = useState<string>('')
  const [pickedQuantities, setPickedQuantities] = useState<Record<string, number>>({})
  const [barcode, setBarcode] = useState('')
  const [scanState, setScanState] = useState<ScanState>('idle')
  const [scanMessage, setScanMessage] = useState('Barkod okutmaya hazır.')
  const [success, setSuccess] = useState('')

  useEffect(() => {
    void services.orderSource.getPendingOrders().then((items) => {
      setOrders(items)
      setSelectedOrderId(items[0]?.id ?? '')
    })
  }, [services.orderSource])

  useEffect(() => {
    window.setTimeout(() => inputRef.current?.focus(), 0)
  }, [selectedOrderId])

  const selectedOrder = orders.find((order) => order.id === selectedOrderId) ?? null
  const totalRequired = selectedOrder?.items.reduce((sum, item) => sum + item.requestedQuantity, 0) ?? 0
  const totalPicked = selectedOrder?.items.reduce((sum, item) => sum + Math.min(pickedQuantities[item.id] ?? 0, item.requestedQuantity), 0) ?? 0
  const isComplete = Boolean(selectedOrder && totalRequired > 0 && totalPicked === totalRequired)
  const progress = totalRequired > 0 ? Math.round((totalPicked / totalRequired) * 100) : 0

  function handleScan() {
    if (!selectedOrder) {
      return
    }

    const scannedBarcode = services.scanner.normalize(barcode)
    setBarcode('')
    window.setTimeout(() => inputRef.current?.focus(), 0)

    if (!scannedBarcode) {
      return
    }

    const item = selectedOrder.items.find((candidate) => candidate.barcode === scannedBarcode)

    if (!item) {
      setScanState('error')
      setScanMessage('Yanlış ürün')
      services.audio.error()
      return
    }

    const currentQuantity = pickedQuantities[item.id] ?? 0

    if (currentQuantity >= item.requestedQuantity) {
      setScanState('error')
      setScanMessage('Bu ürün zaten tamamlandı.')
      services.audio.error()
      return
    }

    setPickedQuantities((current) => ({ ...current, [item.id]: currentQuantity + 1 }))
    setScanState('success')
    setScanMessage('✓ Doğru ürün')
    services.audio.success()
  }

  async function completeOrder() {
    if (!selectedOrder || !isComplete) {
      return
    }

    await services.stockService.reserve(selectedOrder)
    await services.shipmentService.complete(selectedOrder)
    setSuccess('Sipariş başarıyla toplandı.')
    setScanState('idle')
    setScanMessage('Barkod okutmaya hazır.')
    setPickedQuantities({})
    setOrders((current) => current.filter((order) => order.id !== selectedOrder.id))
    setSelectedOrderId((current) => {
      const remaining = orders.filter((order) => order.id !== current)
      return remaining[0]?.id ?? ''
    })
    window.setTimeout(() => inputRef.current?.focus(), 0)
  }

  return (
    <Stack spacing={3}>
      <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 2 }}>
        <Stack spacing={2}>
          <Typography variant="h6" sx={{ fontWeight: 900 }}>Toplama Modu</Typography>
          <TextField
            inputRef={inputRef}
            label="Barkod Okut"
            value={barcode}
            onChange={(event) => setBarcode(event.target.value)}
            onKeyDown={(event) => {
              if (event.key === 'Enter') {
                event.preventDefault()
                handleScan()
              }
            }}
            placeholder="USB barkod okuyucu ile okutun"
            size="medium"
            slotProps={{
              input: {
                sx: { fontSize: 28, fontWeight: 800, minHeight: 64 },
                startAdornment: (
                  <InputAdornment position="start">
                    <QrCodeScannerOutlinedIcon fontSize="large" />
                  </InputAdornment>
                ),
              },
            }}
            fullWidth
            autoFocus
          />

          <Alert
            severity={scanState === 'success' ? 'success' : scanState === 'error' ? 'error' : 'info'}
            icon={scanState === 'success' ? <CheckCircleOutlineOutlinedIcon /> : scanState === 'error' ? <ErrorOutlineOutlinedIcon /> : <Inventory2OutlinedIcon />}
            sx={{
              fontSize: 18,
              fontWeight: 800,
              animation: scanState === 'idle' ? 'none' : `${scanState === 'success' ? 'pickSuccess' : 'pickError'} 420ms ease`,
              '@keyframes pickSuccess': {
                '0%': { transform: 'scale(1)', boxShadow: '0 0 0 rgba(46, 125, 50, 0)' },
                '45%': { transform: 'scale(1.015)', boxShadow: '0 0 0 6px rgba(46, 125, 50, 0.15)' },
                '100%': { transform: 'scale(1)', boxShadow: '0 0 0 rgba(46, 125, 50, 0)' },
              },
              '@keyframes pickError': {
                '0%, 100%': { transform: 'translateX(0)' },
                '25%': { transform: 'translateX(-6px)' },
                '50%': { transform: 'translateX(6px)' },
                '75%': { transform: 'translateX(-4px)' },
              },
            }}
          >
            {scanMessage}
          </Alert>
        </Stack>
      </Paper>

      <Box sx={{ display: 'grid', gap: 3, gridTemplateColumns: { xs: '1fr', lg: '360px minmax(0, 1fr)' } }}>
        <Paper variant="outlined" sx={{ borderRadius: 2, overflow: 'hidden' }}>
          <Box sx={{ p: 2.5 }}>
            <Typography variant="h6" sx={{ fontWeight: 900 }}>Bekleyen Siparişler</Typography>
          </Box>
          <List disablePadding>
            {orders.map((order) => {
              const totals = orderTotals(order)

              return (
                <ListItemButton
                  key={order.id}
                  selected={order.id === selectedOrderId}
                  onClick={() => {
                    setSelectedOrderId(order.id)
                    setScanState('idle')
                    setScanMessage('Barkod okutmaya hazır.')
                  }}
                  sx={{ borderTop: 1, borderColor: 'divider', py: 2 }}
                >
                  <ListItemText
                    primary={<Typography sx={{ fontWeight: 900 }}>{order.orderNumber}</Typography>}
                    secondary={
                      <Stack spacing={0.5} sx={{ mt: 0.5 }}>
                        <Typography variant="body2">{order.customerName}</Typography>
                        <Typography variant="body2" color="text.secondary">
                          {numberText(totals.productCount)} ürün / {numberText(totals.totalQuantity)} adet
                        </Typography>
                        <Typography variant="body2" color="text.secondary">{dateText(order.orderDate)}</Typography>
                      </Stack>
                    }
                  />
                </ListItemButton>
              )
            })}
          </List>
        </Paper>

        <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 2 }}>
          {!selectedOrder ? (
            <Typography color="text.secondary">Bekleyen sipariş bulunmuyor.</Typography>
          ) : (
            <Stack spacing={2.5}>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ justifyContent: 'space-between' }}>
                <Box>
                  <Typography variant="h5" sx={{ fontWeight: 900 }}>{selectedOrder.orderNumber}</Typography>
                  <Typography color="text.secondary">{selectedOrder.customerName}</Typography>
                </Box>
                <Button variant="contained" size="large" disabled={!isComplete} onClick={() => void completeOrder()}>
                  Siparişi Tamamla
                </Button>
              </Stack>

              <Stack spacing={1}>
                <Stack direction="row" sx={{ justifyContent: 'space-between' }}>
                  <Typography sx={{ fontWeight: 800 }}>Toplama Durumu</Typography>
                  <Typography>{numberText(totalPicked)} / {numberText(totalRequired)}</Typography>
                </Stack>
                <LinearProgress variant="determinate" value={progress} sx={{ height: 10, borderRadius: 999 }} />
              </Stack>

              <Stack spacing={1.5}>
                {selectedOrder.items.map((item) => (
                  <PickingItemCard
                    key={item.id}
                    item={item}
                    pickedQuantity={pickedQuantities[item.id] ?? 0}
                  />
                ))}
              </Stack>
            </Stack>
          )}
        </Paper>
      </Box>

      <Snackbar open={Boolean(success)} autoHideDuration={3000} onClose={() => setSuccess('')}>
        <Alert severity="success" variant="filled" onClose={() => setSuccess('')}>{success}</Alert>
      </Snackbar>
    </Stack>
  )
}

function PickingItemCard({ item, pickedQuantity }: { item: PickingOrderItem; pickedQuantity: number }) {
  const completed = pickedQuantity >= item.requestedQuantity

  return (
    <Card
      variant="outlined"
      sx={{
        borderRadius: 2,
        borderColor: completed ? 'success.main' : 'divider',
        bgcolor: completed ? 'success.50' : 'background.paper',
      }}
    >
      <CardContent>
        <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '88px minmax(0, 1fr)', md: '96px minmax(0, 1fr) auto' } }}>
          <Avatar src={item.imageUrl} variant="rounded" sx={{ width: 88, height: 112, borderRadius: 1 }} />
          <Stack spacing={1}>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <Typography variant="h6" sx={{ fontWeight: 900 }}>{item.modelCode}</Typography>
              <Chip label={`${pickedQuantity} / ${item.requestedQuantity}`} color={completed ? 'success' : 'default'} />
            </Stack>
            <Typography>{item.productName}</Typography>
            <Box sx={{ display: 'grid', gap: 1, gridTemplateColumns: { xs: '1fr 1fr', md: 'repeat(6, minmax(0, 1fr))' } }}>
              <Info label="Renk" value={item.color} />
              <Info label="Beden" value={item.size} />
              <Info label="Barkod" value={item.barcode} />
              <Info label="Mevcut Stok" value={numberText(item.availableStock)} />
              <Info label="Raf" value={item.shelf} />
              <Info label="Koli" value={item.box} />
            </Box>
          </Stack>
          <Stack sx={{ alignItems: { xs: 'flex-start', md: 'center' }, justifyContent: 'center' }}>
            {completed ? <Chip color="success" label="Tamamlandı" /> : <Chip variant="outlined" label="Bekliyor" />}
          </Stack>
        </Box>
      </CardContent>
    </Card>
  )
}

function Info({ label, value }: { label: string; value: string }) {
  return (
    <Box sx={{ minWidth: 0 }}>
      <Typography variant="caption" color="text.secondary">{label}</Typography>
      <Typography sx={{ fontWeight: 700, wordBreak: 'break-word' }}>{value}</Typography>
    </Box>
  )
}
