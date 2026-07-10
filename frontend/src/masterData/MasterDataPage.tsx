import { useCallback, useEffect, useMemo, useState } from 'react'
import type { DragEvent, FormEvent } from 'react'
import { useParams } from 'react-router-dom'
import {
  Alert,
  Box,
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  IconButton,
  InputAdornment,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined'
import DragIndicatorIcon from '@mui/icons-material/DragIndicator'
import EditOutlinedIcon from '@mui/icons-material/EditOutlined'
import SearchIcon from '@mui/icons-material/Search'
import {
  createMasterDataItem,
  deleteMasterDataItem,
  getMasterDataItems,
  reorderMasterDataItems,
  updateMasterDataItem,
} from './api'
import type { MasterDataInput, MasterDataItem, MasterDataType } from './types'
import { masterDataLabels, masterDataSingularLabels } from './types'
import { commonText, confirmDelete, dialogContentSx, dialogPaperSx, requiredMessage } from '../common/uiText'
import { toUserMessage } from '../common/apiClient'

const emptyItem: MasterDataInput = {
  name: '',
  isActive: true,
}

const masterDataTypes = Object.keys(masterDataLabels) as MasterDataType[]

function moveItem(items: MasterDataItem[], draggedId: string, targetId: string): MasterDataItem[] {
  const fromIndex = items.findIndex((item) => item.id === draggedId)
  const toIndex = items.findIndex((item) => item.id === targetId)

  if (fromIndex < 0 || toIndex < 0 || fromIndex === toIndex) {
    return items
  }

  const nextItems = [...items]
  const [draggedItem] = nextItems.splice(fromIndex, 1)
  nextItems.splice(toIndex, 0, draggedItem)

  return nextItems
}

export function MasterDataPage() {
  const { type } = useParams()
  const masterDataType = masterDataTypes.includes(type as MasterDataType)
    ? (type as MasterDataType)
    : 'brands'
  const title = masterDataLabels[masterDataType]
  const singularTitle = masterDataSingularLabels[masterDataType]
  const nameLabel = `${singularTitle} Adı`
  const [items, setItems] = useState<MasterDataItem[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [saving, setSaving] = useState(false)
  const [reordering, setReordering] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingItem, setEditingItem] = useState<MasterDataItem | null>(null)
  const [itemInput, setItemInput] = useState<MasterDataInput>(emptyItem)
  const [draggedId, setDraggedId] = useState<string | null>(null)

  const canReorder = useMemo(
    () => !search.trim() && items.length > 1 && !loading && !reordering,
    [items.length, loading, reordering, search],
  )

  const loadItems = useCallback(
    async (nextSearch?: string) => {
      const activeSearch = nextSearch ?? search
      setLoading(true)
      setError(null)

      try {
        const data = await getMasterDataItems(masterDataType, activeSearch)
        setItems(data)
      } catch (exception) {
        setError(toUserMessage(exception, `${title} yüklenemedi.`))
      } finally {
        setLoading(false)
      }
    },
    [masterDataType, search, title],
  )

  useEffect(() => {
    const handle = window.setTimeout(() => {
      void loadItems()
    }, 250)

    return () => window.clearTimeout(handle)
  }, [loadItems])

  function openAddDialog() {
    setEditingItem(null)
    setItemInput(emptyItem)
    setError(null)
    setDialogOpen(true)
  }

  function openEditDialog(item: MasterDataItem) {
    setEditingItem(item)
    setItemInput({
      name: item.name,
      isActive: item.isActive,
    })
    setError(null)
    setDialogOpen(true)
  }

  const handleDelete = useCallback(
    async (item: MasterDataItem) => {
      const confirmed = confirmDelete(item.name)

      if (!confirmed) {
        return
      }

      setError(null)

      try {
        await deleteMasterDataItem(masterDataType, item.id)
        await loadItems()
      } catch (exception) {
        setError(toUserMessage(exception, `${singularTitle} silinemedi.`))
      }
    },
    [loadItems, masterDataType, singularTitle],
  )

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    const trimmedName = itemInput.name.trim()

    if (!trimmedName) {
      setError(requiredMessage(nameLabel))
      setSaving(false)
      return
    }

    const payload: MasterDataInput = {
      name: trimmedName,
      isActive: itemInput.isActive,
    }

    try {
      if (editingItem) {
        await updateMasterDataItem(masterDataType, editingItem.id, payload)
      } else {
        await createMasterDataItem(masterDataType, payload)
        setSearch('')
      }

      setDialogOpen(false)
      setItemInput(emptyItem)
      await loadItems(editingItem ? undefined : '')
    } catch (exception) {
      setError(toUserMessage(exception, `${singularTitle} kaydedilemedi.`))
    } finally {
      setSaving(false)
    }
  }

  function handleDragStart(event: DragEvent<HTMLTableRowElement>, id: string) {
    if (!canReorder) {
      event.preventDefault()
      return
    }

    event.dataTransfer.effectAllowed = 'move'
    setDraggedId(id)
  }

  function handleDragOver(event: DragEvent<HTMLTableRowElement>) {
    if (canReorder) {
      event.preventDefault()
      event.dataTransfer.dropEffect = 'move'
    }
  }

  async function handleDrop(event: DragEvent<HTMLTableRowElement>, targetId: string) {
    event.preventDefault()

    if (!draggedId || draggedId === targetId || !canReorder) {
      setDraggedId(null)
      return
    }

    const previousItems = items
    const nextItems = moveItem(items, draggedId, targetId)

    if (nextItems === items) {
      setDraggedId(null)
      return
    }

    setItems(nextItems)
    setDraggedId(null)
    setReordering(true)
    setError(null)

    try {
      const savedItems = await reorderMasterDataItems(masterDataType, nextItems.map((item) => item.id))
      setItems(savedItems)
    } catch (exception) {
      setItems(previousItems)
      setError(toUserMessage(exception, 'Sıralama kaydedilemedi.'))
    } finally {
      setReordering(false)
    }
  }

  return (
    <Stack spacing={3}>
      <Stack
        direction={{ xs: 'column', md: 'row' }}
        spacing={2}
        sx={{ alignItems: { xs: 'stretch', md: 'center' }, justifyContent: 'space-between' }}
      >
        <Box>
          <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }}>
            {title}
          </Typography>
          <Typography color="text.secondary">Ürün, satın alma ve üretimde kullanılan ortak tanımları yönetin.</Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openAddDialog}>
          Yeni Ekle
        </Button>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder={`${title} içinde ara`}
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
          <TableContainer sx={{ minHeight: 420 }}>
            <Table size="small" aria-label={title}>
              <TableHead>
                <TableRow>
                  <TableCell sx={{ width: 56 }} />
                  <TableCell>Ad</TableCell>
                  <TableCell sx={{ width: 120 }}>Aktif</TableCell>
                  <TableCell align="right" sx={{ width: 120 }} />
                </TableRow>
              </TableHead>
              <TableBody>
                {items.map((item) => (
                  <TableRow
                    key={item.id}
                    draggable={canReorder}
                    hover
                    onDragStart={(event) => handleDragStart(event, item.id)}
                    onDragOver={handleDragOver}
                    onDrop={(event) => void handleDrop(event, item.id)}
                    onDragEnd={() => setDraggedId(null)}
                    sx={{
                      cursor: canReorder ? 'grab' : 'default',
                      opacity: draggedId === item.id ? 0.55 : 1,
                    }}
                  >
                    <TableCell>
                      <Tooltip title="Sırala">
                        <Box
                          component="span"
                          sx={{
                            color: canReorder ? 'text.secondary' : 'action.disabled',
                            display: 'inline-flex',
                            pt: 0.5,
                          }}
                        >
                          <DragIndicatorIcon fontSize="small" />
                        </Box>
                      </Tooltip>
                    </TableCell>
                    <TableCell>
                      <Typography sx={{ fontWeight: 700 }}>{item.name}</Typography>
                    </TableCell>
                    <TableCell>{item.isActive ? commonText.yes : commonText.no}</TableCell>
                    <TableCell align="right">
                      <Stack direction="row" spacing={0.5} sx={{ justifyContent: 'flex-end' }}>
                        <Tooltip title={commonText.edit}>
                          <IconButton size="small" onClick={() => openEditDialog(item)}>
                            <EditOutlinedIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title={commonText.delete}>
                          <IconButton size="small" color="error" onClick={() => void handleDelete(item)}>
                            <DeleteOutlinedIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                      </Stack>
                    </TableCell>
                  </TableRow>
                ))}
                {!loading && items.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={4}>
                      <Typography color="text.secondary" sx={{ py: 4, textAlign: 'center' }}>
                        Henüz kayıt yok.
                      </Typography>
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </Stack>
      </Paper>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} fullWidth maxWidth="xs" slotProps={{ paper: { sx: dialogPaperSx } }}>
        <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
          <DialogTitle>{editingItem ? `${singularTitle} Düzenle` : `${singularTitle} Ekle`}</DialogTitle>
          <DialogContent sx={dialogContentSx}>
            {error && <Alert severity="error" sx={{ mb: 2, whiteSpace: 'pre-line' }}>{error}</Alert>}
            <Stack spacing={2.5} sx={{ pt: 1 }}>
              <TextField
                label={nameLabel}
                value={itemInput.name}
                onChange={(event) => setItemInput((current) => ({ ...current, name: event.target.value }))}
                required
                autoFocus
                helperText={!itemInput.name.trim() ? requiredMessage(nameLabel) : ' '}
                fullWidth
              />
              <FormControlLabel
                control={
                  <Checkbox
                    checked={itemInput.isActive}
                    onChange={(event) =>
                      setItemInput((current) => ({ ...current, isActive: event.target.checked }))
                    }
                  />
                }
                label="Aktif"
              />
            </Stack>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setDialogOpen(false)}>{commonText.cancel}</Button>
            <Button type="submit" variant="contained" disabled={saving}>
              {commonText.save}
            </Button>
          </DialogActions>
        </Box>
      </Dialog>
    </Stack>
  )
}
