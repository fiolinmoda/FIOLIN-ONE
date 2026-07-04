import { useCallback, useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'
import { useParams } from 'react-router-dom'
import { DataGrid, type GridColDef } from '@mui/x-data-grid'
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
  TextField,
  Tooltip,
  Typography,
} from '@mui/material'
import AddIcon from '@mui/icons-material/Add'
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined'
import EditOutlinedIcon from '@mui/icons-material/EditOutlined'
import SearchIcon from '@mui/icons-material/Search'
import {
  createMasterDataItem,
  deleteMasterDataItem,
  getMasterDataItems,
  updateMasterDataItem,
} from './api'
import type { MasterDataInput, MasterDataItem, MasterDataType } from './types'
import { masterDataLabels } from './types'

const emptyItem: MasterDataInput = {
  name: '',
  code: '',
  isActive: true,
  sortOrder: 0,
}

const masterDataTypes = Object.keys(masterDataLabels) as MasterDataType[]

export function MasterDataPage() {
  const { type } = useParams()
  const masterDataType = masterDataTypes.includes(type as MasterDataType)
    ? (type as MasterDataType)
    : 'brands'
  const title = masterDataLabels[masterDataType]
  const [items, setItems] = useState<MasterDataItem[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(false)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingItem, setEditingItem] = useState<MasterDataItem | null>(null)
  const [itemInput, setItemInput] = useState<MasterDataInput>(emptyItem)

  const loadItems = useCallback(async () => {
    setLoading(true)
    setError(null)

    try {
      const data = await getMasterDataItems(masterDataType, search)
      setItems(data)
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : `${title} could not be loaded.`)
    } finally {
      setLoading(false)
    }
  }, [masterDataType, search, title])

  useEffect(() => {
    const handle = window.setTimeout(() => {
      void loadItems()
    }, 250)

    return () => window.clearTimeout(handle)
  }, [loadItems])

  function openAddDialog() {
    setEditingItem(null)
    setItemInput(emptyItem)
    setDialogOpen(true)
  }

  function openEditDialog(item: MasterDataItem) {
    setEditingItem(item)
    setItemInput({
      name: item.name,
      code: item.code,
      isActive: item.isActive,
      sortOrder: item.sortOrder,
    })
    setDialogOpen(true)
  }

  const handleDelete = useCallback(
    async (item: MasterDataItem) => {
      const confirmed = window.confirm(`Delete ${item.name}?`)

      if (!confirmed) {
        return
      }

      setError(null)

      try {
        await deleteMasterDataItem(masterDataType, item.id)
        await loadItems()
      } catch (exception) {
        setError(exception instanceof Error ? exception.message : `${title} could not be deleted.`)
      }
    },
    [loadItems, masterDataType, title],
  )

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      if (editingItem) {
        await updateMasterDataItem(masterDataType, editingItem.id, itemInput)
      } else {
        await createMasterDataItem(masterDataType, itemInput)
      }

      setDialogOpen(false)
      await loadItems()
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : `${title} could not be saved.`)
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo<GridColDef<MasterDataItem>[]>(
    () => [
      { field: 'code', headerName: 'Code', minWidth: 140, flex: 0.7 },
      { field: 'name', headerName: 'Name', minWidth: 220, flex: 1.2 },
      { field: 'sortOrder', headerName: 'Sort', type: 'number', minWidth: 100, flex: 0.4 },
      {
        field: 'isActive',
        headerName: 'Active',
        minWidth: 110,
        flex: 0.4,
        valueFormatter: (value: boolean) => (value ? 'Yes' : 'No'),
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
            <Tooltip title="Edit">
              <IconButton size="small" onClick={() => openEditDialog(row)}>
                <EditOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Delete">
              <IconButton size="small" color="error" onClick={() => void handleDelete(row)}>
                <DeleteOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          </Stack>
        ),
      },
    ],
    [handleDelete],
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
            {title}
          </Typography>
          <Typography color="text.secondary">Manage shared product master data.</Typography>
        </Box>
        <Button variant="contained" startIcon={<AddIcon />} onClick={openAddDialog}>
          Add
        </Button>
      </Stack>

      {error && <Alert severity="error">{error}</Alert>}

      <Paper variant="outlined" sx={{ p: 2, borderRadius: 1 }}>
        <Stack spacing={2}>
          <TextField
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder={`Search ${title.toLowerCase()}`}
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
              rows={items}
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

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} fullWidth maxWidth="sm">
        <Box component="form" onSubmit={(event) => void handleSubmit(event)}>
          <DialogTitle>{editingItem ? `Edit ${title}` : `Add ${title}`}</DialogTitle>
          <DialogContent>
            <Stack spacing={2.5} sx={{ pt: 1 }}>
              <TextField
                label="Name"
                value={itemInput.name}
                onChange={(event) => setItemInput((current) => ({ ...current, name: event.target.value }))}
                required
                fullWidth
              />
              <TextField
                label="Code"
                value={itemInput.code}
                onChange={(event) => setItemInput((current) => ({ ...current, code: event.target.value }))}
                required
                fullWidth
              />
              <TextField
                label="Sort Order"
                type="number"
                value={itemInput.sortOrder}
                onChange={(event) =>
                  setItemInput((current) => ({ ...current, sortOrder: Number(event.target.value) }))
                }
                required
                fullWidth
                slotProps={{
                  htmlInput: {
                    min: 0,
                  },
                }}
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
                label="Active"
              />
            </Stack>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
            <Button type="submit" variant="contained" disabled={saving}>
              Save
            </Button>
          </DialogActions>
        </Box>
      </Dialog>
    </Stack>
  )
}
