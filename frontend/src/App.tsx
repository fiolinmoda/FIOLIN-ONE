import {
  AppBar,
  Box,
  Chip,
  Container,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListSubheader,
  ListItemText,
  Stack,
  Toolbar,
  Typography,
  useMediaQuery,
} from '@mui/material'
import { Link as RouterLink, Navigate, Route, Routes, useLocation } from 'react-router-dom'
import AssignmentOutlinedIcon from '@mui/icons-material/AssignmentOutlined'
import CategoryOutlinedIcon from '@mui/icons-material/CategoryOutlined'
import DashboardOutlinedIcon from '@mui/icons-material/DashboardOutlined'
import DatasetOutlinedIcon from '@mui/icons-material/DatasetOutlined'
import FactoryOutlinedIcon from '@mui/icons-material/FactoryOutlined'
import Inventory2OutlinedIcon from '@mui/icons-material/Inventory2Outlined'
import LocalShippingOutlinedIcon from '@mui/icons-material/LocalShippingOutlined'
import PriceCheckOutlinedIcon from '@mui/icons-material/PriceCheckOutlined'
import { MasterDataPage } from './masterData/MasterDataPage'
import { ProductDetailPage } from './products/ProductDetailPage'
import { ProductListPage } from './products/ProductListPage'

const drawerWidth = 280

const modules = [
  { name: 'Product Cards', path: '/products', icon: <CategoryOutlinedIcon />, active: true },
]

const masterDataModules = [
  { name: 'Brands', path: '/master-data/brands', icon: <DatasetOutlinedIcon />, active: true },
  { name: 'Categories', path: '/master-data/categories', icon: <DatasetOutlinedIcon />, active: true },
  { name: 'Seasons', path: '/master-data/seasons', icon: <DatasetOutlinedIcon />, active: true },
  { name: 'Colors', path: '/master-data/colors', icon: <DatasetOutlinedIcon />, active: true },
  { name: 'Sizes', path: '/master-data/sizes', icon: <DatasetOutlinedIcon />, active: true },
  { name: 'Fabric Types', path: '/master-data/fabric-types', icon: <DatasetOutlinedIcon />, active: true },
]

const plannedModules = [
  { name: 'Fabric Management', icon: <Inventory2OutlinedIcon /> },
  { name: 'Pattern Management', icon: <AssignmentOutlinedIcon /> },
  { name: 'Production Orders', icon: <FactoryOutlinedIcon /> },
  { name: 'Warehouse', icon: <LocalShippingOutlinedIcon /> },
  { name: 'Finance', icon: <PriceCheckOutlinedIcon /> },
]

function App() {
  const location = useLocation()
  const isDesktop = useMediaQuery('(min-width:900px)')

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh', bgcolor: 'background.default' }}>
      <AppBar
        position="fixed"
        elevation={0}
        sx={{ borderBottom: 1, borderColor: 'divider', zIndex: (theme) => theme.zIndex.drawer + 1 }}
      >
        <Toolbar>
          <Stack direction="row" spacing={1.5} sx={{ alignItems: 'center' }}>
            <DashboardOutlinedIcon />
            <Typography variant="h6" component="div" sx={{ fontWeight: 700 }}>
              FIOLIN ONE
            </Typography>
            <Chip label="Product Management" size="small" color="primary" variant="outlined" />
          </Stack>
        </Toolbar>
      </AppBar>

      <Drawer
        variant="permanent"
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          display: { xs: 'none', md: 'block' },
          [`& .MuiDrawer-paper`]: {
            width: drawerWidth,
            boxSizing: 'border-box',
            borderRightColor: 'divider',
          },
        }}
      >
        <Toolbar />
        <Box sx={{ overflow: 'auto', py: 2 }}>
          <List dense>
            {modules.map((item) => (
              <ListItem key={item.name} disablePadding>
                <ListItemButton
                  component={item.path ? RouterLink : 'div'}
                  to={item.path}
                  selected={item.path ? location.pathname.startsWith(item.path) : false}
                  disabled={!item.active}
                >
                  <ListItemIcon>{item.icon}</ListItemIcon>
                  <ListItemText primary={item.name} secondary={item.active ? 'Active module' : 'Planned module'} />
                </ListItemButton>
              </ListItem>
            ))}
            <ListSubheader sx={{ bgcolor: 'transparent', lineHeight: '32px' }}>Master Data</ListSubheader>
            {masterDataModules.map((item) => (
              <ListItem key={item.name} disablePadding>
                <ListItemButton
                  component={RouterLink}
                  to={item.path}
                  selected={location.pathname.startsWith(item.path)}
                >
                  <ListItemIcon>{item.icon}</ListItemIcon>
                  <ListItemText primary={item.name} />
                </ListItemButton>
              </ListItem>
            ))}
            <ListSubheader sx={{ bgcolor: 'transparent', lineHeight: '32px' }}>Planned</ListSubheader>
            {plannedModules.map((item) => (
              <ListItem key={item.name} disablePadding>
                <ListItemButton component="div" disabled>
                  <ListItemIcon>{item.icon}</ListItemIcon>
                  <ListItemText primary={item.name} secondary="Planned module" />
                </ListItemButton>
              </ListItem>
            ))}
          </List>
        </Box>
      </Drawer>

      <Box component="main" sx={{ flexGrow: 1 }}>
        <Toolbar />
        {!isDesktop && (
          <Box sx={{ px: 2, pt: 2 }}>
            <Chip icon={<CategoryOutlinedIcon />} label="Product Cards" color="primary" variant="outlined" />
          </Box>
        )}
        <Container maxWidth="xl" sx={{ py: { xs: 3, md: 5 } }}>
          <Routes>
            <Route path="/" element={<Navigate to="/products" replace />} />
            <Route path="/products" element={<ProductListPage />} />
            <Route path="/products/:id" element={<ProductDetailPage />} />
            <Route path="/master-data/:type" element={<MasterDataPage />} />
          </Routes>
        </Container>
      </Box>
    </Box>
  )
}

export default App
