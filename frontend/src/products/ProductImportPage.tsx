import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
import {
  Alert,
  Box,
  Button,
  Checkbox,
  Divider,
  FormControl,
  FormControlLabel,
  FormLabel,
  LinearProgress,
  MenuItem,
  Paper,
  Radio,
  RadioGroup,
  Snackbar,
  Stack,
  TextField,
  Typography,
} from '@mui/material'
import DownloadOutlinedIcon from '@mui/icons-material/DownloadOutlined'
import RefreshOutlinedIcon from '@mui/icons-material/RefreshOutlined'
import UploadFileOutlinedIcon from '@mui/icons-material/UploadFileOutlined'
import { toUserMessage } from '../common/apiClient'
import { executeProductImport, getProductImportHistory, previewProductImport } from './productImportApi'
import type {
  MissingMasterDataMode,
  ProductImportErrorRow,
  ProductImportHistory,
  ProductImportMapping,
  ProductImportPreview,
  ProductImportPreviewRow,
  ProductImportResult,
} from './importTypes'

type MappingKey = keyof ProductImportMapping

const emptyMapping: ProductImportMapping = {
  modelCode: null,
  barcode: null,
  productName: null,
  brand: null,
  category: null,
  season: null,
  color: null,
  size: null,
  fabricType: null,
  purchasePrice: null,
  salesPrice: null,
  stock: null,
  imageUrl: null,
}

const mappingFields: Array<{ key: MappingKey; label: string; required?: boolean }> = [
  { key: 'modelCode', label: 'Model Kodu', required: true },
  { key: 'barcode', label: 'Barkod', required: true },
  { key: 'productName', label: 'Ürün Adı', required: true },
  { key: 'brand', label: 'Marka' },
  { key: 'category', label: 'Kategori' },
  { key: 'season', label: 'Sezon' },
  { key: 'color', label: 'Renk' },
  { key: 'size', label: 'Beden' },
  { key: 'fabricType', label: 'Kumaş Tipi' },
  { key: 'purchasePrice', label: 'Alış Fiyatı' },
  { key: 'salesPrice', label: 'Satış Fiyatı' },
  { key: 'stock', label: 'Stok' },
  { key: 'imageUrl', label: 'Görsel URL' },
]

const modeLabels: Record<MissingMasterDataMode, string> = {
  Create: 'Otomatik oluştur',
  Skip: 'Eksik kayıt bulunan satırları atla',
  Cancel: 'İçe aktarmayı iptal et',
}

const missingMasterDataLabels = [
  ['Marka', 'brands'],
  ['Kategori', 'categories'],
  ['Sezon', 'seasons'],
  ['Renk', 'colors'],
  ['Beden', 'sizes'],
  ['Kumaş Tipi', 'fabricTypes'],
] as const

function numberText(value: number) {
  return value.toLocaleString('tr-TR')
}

function statusText(status: string) {
  const labels: Record<string, string> = {
    New: 'Yeni ürün',
    Existing: 'Zaten mevcut',
    Skipped: 'Atlandı',
    Error: 'Hatalı',
  }

  return labels[status] ?? status
}

function csvEscape(value: string | number) {
  return `"${String(value).replaceAll('"', '""')}"`
}

function downloadErrorCsv(rows: ProductImportErrorRow[]) {
  const lines = [
    ['Satır', 'Model Kodu', 'Ürün Adı', 'Hata'].map(csvEscape).join(','),
    ...rows.map((row) => [row.rowNumber, row.modelCode, row.productName, row.reason].map(csvEscape).join(',')),
  ]
  const blob = new Blob([`\uFEFF${lines.join('\n')}`], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = 'urun-import-hatalari.csv'
  link.click()
  URL.revokeObjectURL(url)
}

export function ProductImportPage() {
  const fileInputRef = useRef<HTMLInputElement | null>(null)
  const [file, setFile] = useState<File | null>(null)
  const [preview, setPreview] = useState<ProductImportPreview | null>(null)
  const [mapping, setMapping] = useState<ProductImportMapping>(emptyMapping)
  const [mode, setMode] = useState<MissingMasterDataMode>('Create')
  const [saveProfile, setSaveProfile] = useState(true)
  const [profileName, setProfileName] = useState('Fiolin Ürün Stok Sistemi')
  const [history, setHistory] = useState<ProductImportHistory[]>([])
  const [result, setResult] = useState<ProductImportResult | null>(null)
  const [loading, setLoading] = useState(false)
  const [progress, setProgress] = useState(0)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  const loadHistory = useCallback(async () => {
    try {
      setHistory(await getProductImportHistory())
    } catch {
      setHistory([])
    }
  }, [])

  useEffect(() => {
    void loadHistory()
  }, [loadHistory])

  const mappingIsValid = Boolean(mapping.modelCode && mapping.barcode && mapping.productName)

  const previewColumns = useMemo<GridColDef<ProductImportPreviewRow>[]>(
    () => [
      { field: 'rowNumber', headerName: 'Satır', width: 90 },
      { field: 'modelCode', headerName: 'Model Kodu', minWidth: 150, flex: 0.8 },
      { field: 'productName', headerName: 'Ürün Adı', minWidth: 220, flex: 1.2 },
      { field: 'status', headerName: 'Durum', minWidth: 150, flex: 0.7, valueFormatter: (value: string) => statusText(value) },
      {
        field: 'errors',
        headerName: 'Açıklama',
        minWidth: 260,
        flex: 1.4,
        valueGetter: (_, row) => row.errors.join(' | ') || '-',
      },
    ],
    [],
  )

  const historyColumns = useMemo<GridColDef<ProductImportHistory>[]>(
    () => [
      {
        field: 'importedAt',
        headerName: 'Tarih',
        minWidth: 170,
        flex: 0.9,
        valueFormatter: (value: string) => new Date(value).toLocaleString('tr-TR'),
      },
      { field: 'fileName', headerName: 'Dosya', minWidth: 220, flex: 1.2 },
      { field: 'userName', headerName: 'Kullanıcı', minWidth: 130, flex: 0.7 },
      { field: 'totalRecords', headerName: 'Toplam', width: 110 },
      { field: 'insertedRecords', headerName: 'Eklenen', width: 110 },
      { field: 'existingRecords', headerName: 'Mevcut', width: 110 },
      { field: 'errorRecords', headerName: 'Hatalı', width: 110 },
      {
        field: 'durationMilliseconds',
        headerName: 'Süre',
        width: 120,
        valueFormatter: (value: number) => `${(value / 1000).toLocaleString('tr-TR')} sn`,
      },
    ],
    [],
  )

  async function runPreview(selectedFile: File, nextMapping: ProductImportMapping | null, nextMode = mode) {
    setLoading(true)
    setProgress(20)
    setError(null)
    setResult(null)

    try {
      const data = await previewProductImport(selectedFile, {
        mapping: nextMapping,
        missingMasterDataMode: nextMode,
      })
      setPreview(data)
      setMapping(data.suggestedMapping)
      if (data.savedProfile) {
        setProfileName(data.savedProfile.profileName)
        setSuccess('Kayıtlı alan eşleştirme profili uygulandı.')
      }
      setProgress(100)
    } catch (exception) {
      setError(toUserMessage(exception, 'Excel dosyası okunamadı. Lütfen .xlsx dosyasını kontrol ediniz.'))
      setProgress(0)
    } finally {
      setLoading(false)
    }
  }

  function handleFileChange(event: React.ChangeEvent<HTMLInputElement>) {
    const selectedFile = event.target.files?.[0] ?? null

    if (!selectedFile) {
      return
    }

    if (!selectedFile.name.toLowerCase().endsWith('.xlsx')) {
      setError('Sadece .xlsx uzantılı Excel dosyaları kabul edilir.')
      event.target.value = ''
      return
    }

    setFile(selectedFile)
    setPreview(null)
    setMapping(emptyMapping)
    void runPreview(selectedFile, null)
  }

  function updateMapping(key: MappingKey, value: string) {
    setMapping((current) => ({ ...current, [key]: value || null }))
  }

  function handleModeChange(value: MissingMasterDataMode) {
    setMode(value)

    if (file && preview) {
      void runPreview(file, mapping, value)
    }
  }

  async function handleRefreshPreview() {
    if (!file) {
      setError('Lütfen önce .xlsx dosyası seçiniz.')
      return
    }

    await runPreview(file, mapping, mode)
  }

  async function handleImport() {
    if (!file) {
      setError('Lütfen önce .xlsx dosyası seçiniz.')
      return
    }

    if (!mappingIsValid) {
      setError('Model Kodu, Barkod ve Ürün Adı alanlarını eşleştiriniz.')
      return
    }

    setLoading(true)
    setProgress(35)
    setError(null)

    try {
      setProgress(65)
      const data = await executeProductImport(file, {
        mapping,
        missingMasterDataMode: mode,
        profileName: saveProfile ? profileName : null,
        saveProfile,
      })
      setResult(data)
      setProgress(100)
      setSuccess(`${numberText(data.inserted)} yeni ürün içe aktarıldı.`)
      await loadHistory()
    } catch (exception) {
      setError(toUserMessage(exception, 'Ürünler içe aktarılamadı. Lütfen verileri kontrol ediniz.'))
      setProgress(0)
    } finally {
      setLoading(false)
    }
  }

  return (
    <Stack spacing={3}>
      <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 1 }}>
        <Stack spacing={2.5}>
          <Box>
            <Typography variant="h5" component="h2" sx={{ fontWeight: 800 }}>
              Excel'den Ürün Aktar
            </Typography>
            <Typography color="text.secondary">
              Google E-Tablolar veya Excel ürün listenizi önizleyin, alanları eşleştirin ve yalnızca yeni modelleri FIOLIN ONE'a aktarın.
            </Typography>
          </Box>

          {error && <Alert severity="error">{error}</Alert>}

          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5} sx={{ alignItems: { xs: 'stretch', sm: 'center' } }}>
            <input ref={fileInputRef} type="file" accept=".xlsx" hidden onChange={handleFileChange} />
            <Button
              variant="contained"
              startIcon={<UploadFileOutlinedIcon />}
              onClick={() => fileInputRef.current?.click()}
              disabled={loading}
            >
              .xlsx Dosyası Seç
            </Button>
            <Typography color="text.secondary">{file ? file.name : 'Henüz dosya seçilmedi.'}</Typography>
          </Stack>

          {loading && (
            <Box>
              <LinearProgress variant="determinate" value={progress} />
              <Typography variant="body2" color="text.secondary" sx={{ mt: 0.75 }}>
                İşlem ilerlemesi: %{progress}
              </Typography>
            </Box>
          )}
        </Stack>
      </Paper>

      {preview && (
        <>
          <SummaryPanel title="Önizleme Özeti" items={[
            ['Toplam', preview.summary.total],
            ['Yeni ürün', preview.summary.newProducts],
            ['Zaten mevcut', preview.summary.existingProducts],
            ['Atlanan', preview.summary.skipped],
            ['Hatalı', preview.summary.error],
          ]} />

          <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 1 }}>
            <Stack spacing={2.5}>
              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} sx={{ justifyContent: 'space-between' }}>
                <Box>
                  <Typography variant="h6" sx={{ fontWeight: 800 }}>
                    Alan Eşleştirme
                  </Typography>
                  <Typography color="text.secondary">
                    İlk satır başlık olarak okundu. Model Kodu benzersiz anahtar olarak kullanılır.
                  </Typography>
                </Box>
                <Button startIcon={<RefreshOutlinedIcon />} onClick={() => void handleRefreshPreview()} disabled={loading}>
                  Önizlemeyi Güncelle
                </Button>
              </Stack>

              <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', md: 'repeat(3, 1fr)' } }}>
                {mappingFields.map((field) => (
                  <TextField
                    key={field.key}
                    select
                    size="small"
                    label={`${field.label}${field.required ? ' *' : ''}`}
                    value={mapping[field.key] ?? ''}
                    onChange={(event) => updateMapping(field.key, event.target.value)}
                    error={field.required && !mapping[field.key]}
                    helperText={field.required && !mapping[field.key] ? `${field.label} eşleştirilmelidir.` : ' '}
                    fullWidth
                  >
                    <MenuItem value="">Eşleştirme yok</MenuItem>
                    {preview.headers.map((header) => (
                      <MenuItem key={header} value={header}>
                        {header}
                      </MenuItem>
                    ))}
                  </TextField>
                ))}
              </Box>

              <Divider />

              <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
                <FormControl sx={{ flex: 1 }}>
                  <FormLabel>Eksik sistem tanımı olduğunda</FormLabel>
                  <RadioGroup
                    value={mode}
                    onChange={(event) => handleModeChange(event.target.value as MissingMasterDataMode)}
                  >
                    {Object.entries(modeLabels).map(([value, label]) => (
                      <FormControlLabel key={value} value={value} control={<Radio />} label={label} />
                    ))}
                  </RadioGroup>
                </FormControl>
                <TextField
                  size="small"
                  label="Profil adı"
                  value={profileName}
                  onChange={(event) => setProfileName(event.target.value)}
                  disabled={!saveProfile}
                  fullWidth
                />
              </Stack>

              <FormControlLabel
                control={<Checkbox checked={saveProfile} onChange={(event) => setSaveProfile(event.target.checked)} />}
                label="Bu alan eşleştirmesini profil olarak kaydet"
              />
            </Stack>
          </Paper>

          <MissingMasterDataPanel preview={preview} mode={mode} />

          <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
            <Stack spacing={2}>
              <Typography variant="h6" sx={{ fontWeight: 800 }}>
                İlk 100 Satır Önizleme
              </Typography>
              <Box sx={{ width: '100%', minHeight: 420 }}>
                <DataGrid
                  rows={preview.rows}
                  columns={previewColumns}
                  getRowId={(row) => row.rowNumber}
                  loading={loading}
                  disableRowSelectionOnClick
                  pageSizeOptions={[10, 25, 50]}
                  initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
                  localeText={{
                    noRowsLabel: 'Önizlenecek satır bulunamadı.',
                    noResultsOverlayLabel: 'Sonuç bulunamadı.',
                  }}
                  sx={{ border: 0 }}
                />
              </Box>
            </Stack>
          </Paper>

          <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 1 }}>
            <Stack spacing={2}>
              <Typography variant="h6" sx={{ fontWeight: 800 }}>
                İçe Aktarma Onayı
              </Typography>
              <Typography color="text.secondary">
                Kayıt işlemi yalnızca İçe Aktar düğmesine bastığınızda yapılır. Mevcut model kodları güncellenmez ve yeni kayıt açılmaz.
              </Typography>
              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5}>
                <Button variant="contained" onClick={() => void handleImport()} disabled={loading || !mappingIsValid}>
                  İçe Aktar
                </Button>
                {result?.errorRows.length ? (
                  <Button startIcon={<DownloadOutlinedIcon />} onClick={() => downloadErrorCsv(result.errorRows)}>
                    Hatalı Satırları CSV İndir
                  </Button>
                ) : null}
              </Stack>
            </Stack>
          </Paper>
        </>
      )}

      {result && (
        <SummaryPanel title="İşlem Sonucu" items={[
          ['Toplam ürün', result.summary.total],
          ['Eklenen ürün', result.inserted],
          ['Mevcut ürün', result.existing],
          ['Atlanan ürün', result.skipped],
          ['Oluşturulan marka', result.createdMasterData.brands],
          ['Oluşturulan kategori', result.createdMasterData.categories],
          ['Oluşturulan sezon', result.createdMasterData.seasons],
          ['Oluşturulan renk', result.createdMasterData.colors],
          ['Oluşturulan beden', result.createdMasterData.sizes],
          ['Oluşturulan kumaş tipi', result.createdMasterData.fabricTypes],
          ['Hatalı satırlar', result.error],
        ]} />
      )}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <Typography variant="h6" sx={{ fontWeight: 800 }}>
            İçe Aktarma Geçmişi
          </Typography>
          <Box sx={{ width: '100%', minHeight: 360 }}>
            <DataGrid
              rows={history}
              columns={historyColumns}
              loading={loading}
              disableRowSelectionOnClick
              pageSizeOptions={[10, 25, 50]}
              initialState={{ pagination: { paginationModel: { pageSize: 10 } } }}
              localeText={{
                noRowsLabel: 'Henüz içe aktarma geçmişi yok.',
                noResultsOverlayLabel: 'Sonuç bulunamadı.',
              }}
              sx={{ border: 0 }}
            />
          </Box>
        </Stack>
      </Paper>

      <Snackbar open={!!success} autoHideDuration={3000} onClose={() => setSuccess(null)} message={success} />
    </Stack>
  )
}

function SummaryPanel({ title, items }: { title: string; items: Array<[string, number]> }) {
  return (
    <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 1 }}>
      <Stack spacing={2}>
        <Typography variant="h6" sx={{ fontWeight: 800 }}>
          {title}
        </Typography>
        <Box sx={{ display: 'grid', gap: 1.5, gridTemplateColumns: { xs: '1fr 1fr', md: 'repeat(5, 1fr)' } }}>
          {items.map(([label, value]) => (
            <Box key={label} sx={{ border: 1, borderColor: 'divider', borderRadius: 1, p: 2 }}>
              <Typography variant="body2" color="text.secondary">
                {label}
              </Typography>
              <Typography variant="h5" sx={{ fontWeight: 800 }}>
                {numberText(value)}
              </Typography>
            </Box>
          ))}
        </Box>
      </Stack>
    </Paper>
  )
}

function MissingMasterDataPanel({ preview, mode }: { preview: ProductImportPreview; mode: MissingMasterDataMode }) {
  const groups = missingMasterDataLabels
    .map(([label, key]) => ({ label, values: preview.missingMasterData[key] }))
    .filter((group) => group.values.length > 0)

  if (groups.length === 0) {
    return (
      <Alert severity="success">
        Eksik sistem tanımı bulunmadı. İçe aktarma mevcut tanımlarla devam edebilir.
      </Alert>
    )
  }

  const explanation: Record<MissingMasterDataMode, string> = {
    Create: 'İçe Aktar dediğinizde aşağıdaki eksik tanımlar otomatik oluşturulacak ve işlem aynı dosya ile devam edecek.',
    Skip: 'İçe Aktar dediğinizde aşağıdaki eksik tanımları içeren satırlar atlanacak.',
    Cancel: 'İçe Aktar dediğinizde eksik tanımlar bulunduğu için işlem kayıt yapmadan iptal edilecek.',
  }

  return (
    <Paper variant="outlined" sx={{ p: { xs: 2, md: 3 }, borderRadius: 1 }}>
      <Stack spacing={2}>
        <Box>
          <Typography variant="h6" sx={{ fontWeight: 800 }}>
            Eksik Sistem Tanımları
          </Typography>
          <Typography color="text.secondary">{explanation[mode]}</Typography>
        </Box>
        <Box sx={{ display: 'grid', gap: 1.5, gridTemplateColumns: { xs: '1fr', md: 'repeat(3, 1fr)' } }}>
          {groups.map((group) => (
            <Box key={group.label} sx={{ border: 1, borderColor: 'divider', borderRadius: 1, p: 2 }}>
              <Typography variant="subtitle2" sx={{ fontWeight: 800, mb: 1 }}>
                {group.label}
              </Typography>
              <Stack spacing={0.75}>
                {group.values.slice(0, 8).map((value) => (
                  <Typography key={value} variant="body2">
                    {value}
                  </Typography>
                ))}
                {group.values.length > 8 && (
                  <Typography variant="body2" color="text.secondary">
                    +{group.values.length - 8} kayıt daha
                  </Typography>
                )}
              </Stack>
            </Box>
          ))}
        </Box>
      </Stack>
    </Paper>
  )
}
