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
import ReceiptLongOutlinedIcon from '@mui/icons-material/ReceiptLongOutlined'
import SpaOutlinedIcon from '@mui/icons-material/SpaOutlined'
import SyncAltOutlinedIcon from '@mui/icons-material/SyncAltOutlined'
import BookmarkAddedOutlinedIcon from '@mui/icons-material/BookmarkAddedOutlined'
import ContentCutOutlinedIcon from '@mui/icons-material/ContentCutOutlined'
import WarehouseOutlinedIcon from '@mui/icons-material/WarehouseOutlined'
import { FabricDetailPage } from './fabric/FabricDetailPage'
import { FabricListPage } from './fabric/FabricListPage'
import { FabricStockPage } from './fabric/FabricStockPage'
import { ReservationListPage } from './fabric/ReservationListPage'
import { StockMovementsPage } from './fabric/StockMovementsPage'
import { MasterDataPage } from './masterData/MasterDataPage'
import { ProductDetailPage } from './products/ProductDetailPage'
import { ProductListPage } from './products/ProductListPage'
import { ProductionDashboardPage } from './production/ProductionDashboardPage'
import { ProductionDetailPage } from './production/ProductionDetailPage'
import { ProductionListPage } from './production/ProductionListPage'
import { ProductionOperationPage } from './production/ProductionOperationPages'
import { ProductionTimelinePage } from './production/ProductionTimelinePage'
import { GoodsReceiptPage } from './purchasing/GoodsReceiptPage'
import { PurchaseInvoicePage } from './purchasing/PurchaseInvoicePage'
import { PurchaseOrderDetailPage } from './purchasing/PurchaseOrderDetailPage'
import { PurchaseOrderListPage } from './purchasing/PurchaseOrderListPage'
import { SupplierManagementPage } from './purchasing/SupplierManagementPage'

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

const purchasingModules = [
  { name: 'Purchase Orders', path: '/purchasing/orders', icon: <ReceiptLongOutlinedIcon /> },
  { name: 'Suppliers', path: '/purchasing/suppliers', icon: <DatasetOutlinedIcon /> },
  { name: 'Goods Receipt', path: '/purchasing/goods-receipts', icon: <Inventory2OutlinedIcon /> },
  { name: 'Purchase Invoices', path: '/purchasing/invoices', icon: <PriceCheckOutlinedIcon /> },
]

const fabricModules = [
  { name: 'Fabric List', path: '/fabric/fabrics', icon: <SpaOutlinedIcon /> },
  { name: 'Fabric Stock', path: '/fabric/stock', icon: <Inventory2OutlinedIcon /> },
  { name: 'Stock Movements', path: '/fabric/movements', icon: <SyncAltOutlinedIcon /> },
  { name: 'Reservations', path: '/fabric/reservations', icon: <BookmarkAddedOutlinedIcon /> },
]

const productionModules = [
  { name: 'Dashboard', path: '/production/dashboard', icon: <DashboardOutlinedIcon /> },
  { name: 'Production Orders', path: '/production/orders', icon: <FactoryOutlinedIcon /> },
  { name: 'Cutting', path: '/production/cutting', icon: <ContentCutOutlinedIcon /> },
  { name: 'Workshop Shipment', path: '/production/workshop-shipment', icon: <LocalShippingOutlinedIcon /> },
  { name: 'Workshop Return', path: '/production/workshop-return', icon: <SyncAltOutlinedIcon /> },
  { name: 'Warehouse Entry', path: '/production/warehouse-entry', icon: <WarehouseOutlinedIcon /> },
]

const plannedModules = [
  { name: 'Pattern Management', icon: <AssignmentOutlinedIcon /> },
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
            <ListSubheader sx={{ bgcolor: 'transparent', lineHeight: '32px' }}>Purchasing</ListSubheader>
            {purchasingModules.map((item) => (
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
            <ListSubheader sx={{ bgcolor: 'transparent', lineHeight: '32px' }}>Fabric</ListSubheader>
            {fabricModules.map((item) => (
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
            <ListSubheader sx={{ bgcolor: 'transparent', lineHeight: '32px' }}>Production</ListSubheader>
            {productionModules.map((item) => (
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
            <Route path="/purchasing/orders" element={<PurchaseOrderListPage />} />
            <Route path="/purchasing/orders/:id" element={<PurchaseOrderDetailPage />} />
            <Route path="/purchasing/suppliers" element={<SupplierManagementPage />} />
            <Route path="/purchasing/goods-receipts" element={<GoodsReceiptPage />} />
            <Route path="/purchasing/invoices" element={<PurchaseInvoicePage />} />
            <Route path="/fabric/fabrics" element={<FabricListPage />} />
            <Route path="/fabric/fabrics/:id" element={<FabricDetailPage />} />
            <Route path="/fabric/stock" element={<FabricStockPage />} />
            <Route path="/fabric/movements" element={<StockMovementsPage />} />
            <Route path="/fabric/reservations" element={<ReservationListPage />} />
            <Route path="/production/dashboard" element={<ProductionDashboardPage />} />
            <Route path="/production/orders" element={<ProductionListPage />} />
            <Route path="/production/orders/:id" element={<ProductionDetailPage />} />
            <Route path="/production/orders/:id/timeline" element={<ProductionTimelinePage />} />
            <Route path="/production/cutting" element={<ProductionOperationPage kind="cutting" />} />
            <Route path="/production/workshop-shipment" element={<ProductionOperationPage kind="shipment" />} />
            <Route path="/production/workshop-return" element={<ProductionOperationPage kind="return" />} />
            <Route path="/production/warehouse-entry" element={<ProductionOperationPage kind="warehouse" />} />
          </Routes>
        </Container>
      </Box>
    </Box>
  )
}

export default App
