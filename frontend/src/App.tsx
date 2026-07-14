import { useEffect, useMemo, useState } from 'react'
import type { ReactNode } from 'react'
import {
  AppBar,
  Box,
  Chip,
  Collapse,
  Container,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Paper,
  Stack,
  Toolbar,
  Tooltip,
  Typography,
  useMediaQuery,
} from '@mui/material'
import { Navigate, Route, Routes, useLocation, useNavigate } from 'react-router-dom'
import AssessmentOutlinedIcon from '@mui/icons-material/AssessmentOutlined'
import AssignmentOutlinedIcon from '@mui/icons-material/AssignmentOutlined'
import BookmarkAddedOutlinedIcon from '@mui/icons-material/BookmarkAddedOutlined'
import CategoryOutlinedIcon from '@mui/icons-material/CategoryOutlined'
import ContentCutOutlinedIcon from '@mui/icons-material/ContentCutOutlined'
import DashboardOutlinedIcon from '@mui/icons-material/DashboardOutlined'
import DatasetOutlinedIcon from '@mui/icons-material/DatasetOutlined'
import ExpandLessOutlinedIcon from '@mui/icons-material/ExpandLessOutlined'
import ExpandMoreOutlinedIcon from '@mui/icons-material/ExpandMoreOutlined'
import FactoryOutlinedIcon from '@mui/icons-material/FactoryOutlined'
import Inventory2OutlinedIcon from '@mui/icons-material/Inventory2Outlined'
import LocalShippingOutlinedIcon from '@mui/icons-material/LocalShippingOutlined'
import LanguageOutlinedIcon from '@mui/icons-material/LanguageOutlined'
import LocalPrintshopOutlinedIcon from '@mui/icons-material/LocalPrintshopOutlined'
import MenuOpenOutlinedIcon from '@mui/icons-material/MenuOpenOutlined'
import MenuOutlinedIcon from '@mui/icons-material/MenuOutlined'
import PriceCheckOutlinedIcon from '@mui/icons-material/PriceCheckOutlined'
import ReceiptLongOutlinedIcon from '@mui/icons-material/ReceiptLongOutlined'
import SettingsOutlinedIcon from '@mui/icons-material/SettingsOutlined'
import ShoppingCartCheckoutOutlinedIcon from '@mui/icons-material/ShoppingCartCheckoutOutlined'
import SpaOutlinedIcon from '@mui/icons-material/SpaOutlined'
import StorefrontOutlinedIcon from '@mui/icons-material/StorefrontOutlined'
import SyncAltOutlinedIcon from '@mui/icons-material/SyncAltOutlined'
import UploadFileOutlinedIcon from '@mui/icons-material/UploadFileOutlined'
import WarehouseOutlinedIcon from '@mui/icons-material/WarehouseOutlined'
import { DashboardPage } from './dashboard/DashboardPage'
import { FabricDetailPage } from './fabric/FabricDetailPage'
import { FabricListPage } from './fabric/FabricListPage'
import { FabricStockPage } from './fabric/FabricStockPage'
import { ReservationListPage } from './fabric/ReservationListPage'
import { StockMovementsPage } from './fabric/StockMovementsPage'
import { MasterDataPage } from './masterData/MasterDataPage'
import { BarcodePrintPage } from './operations/BarcodePrintPage'
import { GoodsReceiptOperationPage } from './operations/GoodsReceiptOperationPage'
import { OrderPickingPage } from './operations/OrderPickingPage'
import { WorkspaceFoundationPage } from './operations/WorkspaceFoundationPage'
import { ProductDetailPage } from './products/ProductDetailPage'
import { ProductImportPage } from './products/ProductImportPage'
import { ProductListPage } from './products/ProductListPage'
import { ProductionDetailPage } from './production/ProductionDetailPage'
import { ProductionListPage } from './production/ProductionListPage'
import { ProductionOperationPage } from './production/ProductionOperationPages'
import { ProductionTimelinePage } from './production/ProductionTimelinePage'
import { GoodsReceiptPage } from './purchasing/GoodsReceiptPage'
import { PurchaseInvoicePage } from './purchasing/PurchaseInvoicePage'
import { PurchaseOrderDetailPage } from './purchasing/PurchaseOrderDetailPage'
import { PurchaseOrderListPage } from './purchasing/PurchaseOrderListPage'
import { SupplierManagementPage } from './purchasing/SupplierManagementPage'
import { ReportsPage } from './reports/ReportsPage'
import { SalesOrderDetailPage } from './sales/SalesOrderDetailPage'
import { SalesOrderListPage } from './sales/SalesOrderListPage'
import { ProductStockPage } from './warehouse/ProductStockPage'

const expandedDrawerWidth = 312
const collapsedDrawerWidth = 88
const sidebarStorageKey = 'fiolin-one-sidebar-collapsed'
const lastPageStorageKey = 'fiolin-one-last-page'
const currentRoles = ['Administrator']

type NavigationPage = {
  title: string
  path: string
  icon: ReactNode
  roles?: string[]
  match?: (pathname: string) => boolean
}

type NavigationGroup = {
  title: string
  icon: ReactNode
  path?: string
  roles?: string[]
  hidden?: boolean
  pages?: NavigationPage[]
  match?: (pathname: string) => boolean
}

type RouteMeta = {
  moduleTitle: string
  pageTitle?: string
}

const dashboardPath = '/dashboard'

function hasRole(roles?: string[]) {
  return !roles || roles.some((role) => currentRoles.includes(role))
}

function matchPath(pathname: string, path: string) {
  return pathname === path || pathname.startsWith(`${path}/`)
}

const navigationGroups: NavigationGroup[] = [
  {
    title: 'Ana Panel',
    path: dashboardPath,
    icon: <DashboardOutlinedIcon />,
    match: (pathname) => pathname === dashboardPath || pathname === '/production/dashboard',
  },
  {
    title: 'Operasyon',
    icon: <WarehouseOutlinedIcon />,
    pages: [
      { title: 'Mal Kabul', path: '/operations/goods-receipt', icon: <Inventory2OutlinedIcon /> },
      { title: 'Depo', path: '/operations/warehouse', icon: <WarehouseOutlinedIcon /> },
      { title: 'Sipariş Toplama', path: '/operations/order-picking', icon: <ShoppingCartCheckoutOutlinedIcon /> },
      { title: 'Barkod Yazdırma', path: '/operations/barcode-print', icon: <LocalPrintshopOutlinedIcon /> },
    ],
  },
  {
    title: 'Kanal Yönetimi',
    icon: <StorefrontOutlinedIcon />,
    pages: [
      { title: 'Trendyol', path: '/channels/trendyol', icon: <StorefrontOutlinedIcon /> },
      { title: 'Web Sitesi', path: '/channels/website', icon: <LanguageOutlinedIcon /> },
    ],
  },
  {
    title: 'Web Yönetimi',
    icon: <LanguageOutlinedIcon />,
    pages: [
      { title: 'Ana Sayfa', path: '/web-management/home', icon: <DashboardOutlinedIcon /> },
      { title: 'Kategoriler', path: '/web-management/categories', icon: <CategoryOutlinedIcon /> },
      { title: 'Koleksiyonlar', path: '/web-management/collections', icon: <DatasetOutlinedIcon /> },
      { title: 'Banner Yönetimi', path: '/web-management/banners', icon: <DatasetOutlinedIcon /> },
      { title: 'Slider Yönetimi', path: '/web-management/sliders', icon: <DatasetOutlinedIcon /> },
      { title: 'Sayfalar', path: '/web-management/pages', icon: <DatasetOutlinedIcon /> },
      { title: 'Menüler', path: '/web-management/menus', icon: <MenuOutlinedIcon /> },
      { title: 'SEO', path: '/web-management/seo', icon: <LanguageOutlinedIcon /> },
      { title: 'Tema Ayarları', path: '/web-management/theme', icon: <SettingsOutlinedIcon /> },
      { title: 'Yayın Yönetimi', path: '/web-management/publishing', icon: <StorefrontOutlinedIcon /> },
    ],
  },
  {
    title: 'Ürün Yönetimi',
    icon: <CategoryOutlinedIcon />,
    hidden: true,
    pages: [
      { title: 'Ürünler', path: '/products', icon: <CategoryOutlinedIcon /> },
      { title: "Excel'den Ürün Aktar", path: '/products/import', icon: <UploadFileOutlinedIcon />, roles: ['Administrator'] },
      { title: 'Varyantlar', path: '/product-variants', icon: <DatasetOutlinedIcon /> },
    ],
  },
  {
    title: 'Satın Alma',
    icon: <ReceiptLongOutlinedIcon />,
    hidden: true,
    pages: [
      { title: 'Tedarikçiler', path: '/purchasing/suppliers', icon: <DatasetOutlinedIcon /> },
      { title: 'Satın Alma Emirleri', path: '/purchasing/orders', icon: <ReceiptLongOutlinedIcon /> },
      { title: 'Mal Kabul', path: '/purchasing/goods-receipts', icon: <Inventory2OutlinedIcon /> },
      { title: 'Alış Faturaları', path: '/purchasing/invoices', icon: <PriceCheckOutlinedIcon /> },
    ],
  },
  {
    title: 'Üretim',
    icon: <FactoryOutlinedIcon />,
    hidden: true,
    pages: [
      { title: 'Üretim Emirleri', path: '/production/orders', icon: <FactoryOutlinedIcon /> },
      { title: 'Kesim', path: '/production/cutting', icon: <ContentCutOutlinedIcon /> },
      { title: 'Atölye Gönderimi', path: '/production/workshop-shipment', icon: <LocalShippingOutlinedIcon /> },
      { title: 'Atölye Dönüşü', path: '/production/workshop-return', icon: <SyncAltOutlinedIcon /> },
      { title: 'Paketleme', path: '/production/packaging', icon: <WarehouseOutlinedIcon /> },
      { title: 'Depo Girişi', path: '/production/warehouse-entry', icon: <WarehouseOutlinedIcon /> },
      { title: 'Süreç Geçmişi', path: '/production/history', icon: <AssignmentOutlinedIcon /> },
    ],
  },
  {
    title: 'Depo',
    icon: <WarehouseOutlinedIcon />,
    hidden: true,
    pages: [
      { title: 'Kumaş Kartları', path: '/fabric/fabrics', icon: <SpaOutlinedIcon /> },
      { title: 'Kumaş Stoğu', path: '/warehouse/fabric-stock', icon: <Inventory2OutlinedIcon /> },
      { title: 'Stok Hareketleri', path: '/fabric/movements', icon: <SyncAltOutlinedIcon /> },
      { title: 'Rezervasyonlar', path: '/fabric/reservations', icon: <BookmarkAddedOutlinedIcon /> },
      { title: 'Ürün Stoğu', path: '/warehouse/product-stock', icon: <Inventory2OutlinedIcon /> },
      { title: 'Barkod', path: '/warehouse/barcodes', icon: <DatasetOutlinedIcon /> },
      { title: 'Sayım', path: '/warehouse/counting', icon: <AssignmentOutlinedIcon /> },
    ],
  },
  {
    title: 'Satış',
    path: '/sales',
    icon: <StorefrontOutlinedIcon />,
    hidden: true,
  },
  {
    title: 'Raporlar',
    path: '/reports',
    icon: <AssessmentOutlinedIcon />,
    hidden: true,
  },
  {
    title: 'Sistem Tanımları',
    icon: <DatasetOutlinedIcon />,
    roles: ['Administrator'],
    hidden: true,
    pages: [
      { title: 'Markalar', path: '/master-data/brands', icon: <DatasetOutlinedIcon />, roles: ['Administrator'] },
      { title: 'Kategoriler', path: '/master-data/categories', icon: <DatasetOutlinedIcon />, roles: ['Administrator'] },
      { title: 'Sezonlar', path: '/master-data/seasons', icon: <DatasetOutlinedIcon />, roles: ['Administrator'] },
      { title: 'Renkler', path: '/master-data/colors', icon: <DatasetOutlinedIcon />, roles: ['Administrator'] },
      { title: 'Bedenler', path: '/master-data/sizes', icon: <DatasetOutlinedIcon />, roles: ['Administrator'] },
      {
        title: 'Kumaş Tipleri',
        path: '/master-data/fabric-types',
        icon: <DatasetOutlinedIcon />,
        roles: ['Administrator'],
      },
    ],
  },
  {
    title: 'Ayarlar',
    path: '/settings',
    icon: <SettingsOutlinedIcon />,
  },
]

function getStoredSidebarState() {
  return localStorage.getItem(sidebarStorageKey) === 'true'
}

function isVisibleNavigationPath(pathname: string) {
  return navigationGroups
    .filter((group) => !group.hidden && hasRole(group.roles))
    .some((group) => {
      if (group.path && matchPath(pathname, group.path)) {
        return true
      }

      return Boolean(group.pages?.some((page) => hasRole(page.roles) && matchPath(pathname, page.path)))
    })
}

function getLastPage() {
  const storedPage = localStorage.getItem(lastPageStorageKey)
  return storedPage && isVisibleNavigationPath(storedPage) ? storedPage : dashboardPath
}

function isGroupActive(group: NavigationGroup, pathname: string) {
  if (group.match?.(pathname)) {
    return true
  }

  if (group.path && matchPath(pathname, group.path)) {
    return true
  }

  return Boolean(group.pages?.some((page) => page.match?.(pathname) || matchPath(pathname, page.path)))
}

function getRouteMeta(pathname: string): RouteMeta {
  if (pathname === '/' || pathname === dashboardPath || pathname === '/production/dashboard') {
    return { moduleTitle: 'Ana Panel' }
  }

  if (pathname.startsWith('/products/')) {
    if (pathname === '/products/import') {
      return { moduleTitle: 'Ürün Yönetimi', pageTitle: "Excel'den Ürün Aktar" }
    }

    return { moduleTitle: 'Ürün Yönetimi', pageTitle: 'Ürün Detayı' }
  }

  if (pathname === '/product-variants') {
    return { moduleTitle: 'Ürün Yönetimi', pageTitle: 'Varyantlar' }
  }

  if (pathname.startsWith('/purchasing/orders/')) {
    return { moduleTitle: 'Satın Alma', pageTitle: 'Satın Alma Emri Detayı' }
  }

  if (pathname.startsWith('/production/orders/') && pathname.endsWith('/timeline')) {
    return { moduleTitle: 'Üretim', pageTitle: 'Süreç Geçmişi' }
  }

  if (pathname.startsWith('/production/orders/')) {
    return { moduleTitle: 'Üretim', pageTitle: 'Üretim Emri Detayı' }
  }

  if (pathname.startsWith('/fabric/fabrics/')) {
    return { moduleTitle: 'Depo', pageTitle: 'Kumaş Kartı Detayı' }
  }

  for (const group of navigationGroups) {
    if (group.path && matchPath(pathname, group.path)) {
      return { moduleTitle: group.title }
    }

    const page = group.pages?.find((item) => item.match?.(pathname) || matchPath(pathname, item.path))
    if (page) {
      return { moduleTitle: group.title, pageTitle: page.title }
    }
  }

  return { moduleTitle: 'FIOLIN ONE' }
}

function getBreadcrumb(meta: RouteMeta) {
  return meta.pageTitle ? `${meta.moduleTitle} / ${meta.pageTitle}` : meta.moduleTitle
}

function App() {
  const location = useLocation()
  const navigate = useNavigate()
  const isDesktop = useMediaQuery('(min-width:900px)')
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(getStoredSidebarState)
  const [isMobileOpen, setIsMobileOpen] = useState(false)
  const [openGroups, setOpenGroups] = useState<Record<string, boolean>>(() =>
    Object.fromEntries(navigationGroups.map((group) => [group.title, true])),
  )

  const routeMeta = useMemo(() => getRouteMeta(location.pathname), [location.pathname])
  const drawerWidth = isSidebarCollapsed ? collapsedDrawerWidth : expandedDrawerWidth
  const visibleGroups = navigationGroups.filter((group) => !group.hidden && hasRole(group.roles))

  useEffect(() => {
    if (location.pathname !== '/') {
      localStorage.setItem(lastPageStorageKey, location.pathname)
    }
  }, [location.pathname])

  useEffect(() => {
    localStorage.setItem(sidebarStorageKey, String(isSidebarCollapsed))
  }, [isSidebarCollapsed])

  function navigateTo(path: string) {
    navigate(path)
    setIsMobileOpen(false)
  }

  function handleGroupClick(group: NavigationGroup) {
    if (group.path) {
      navigateTo(group.path)
      return
    }

    setOpenGroups((current) => ({ ...current, [group.title]: !current[group.title] }))
  }

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh', bgcolor: 'background.default' }}>
      <AppBar
        position="fixed"
        elevation={0}
        sx={{
          borderBottom: 1,
          borderColor: 'divider',
          zIndex: (theme) => theme.zIndex.drawer + 1,
        }}
      >
        <Toolbar sx={{ gap: 2 }}>
          <IconButton
            color="inherit"
            edge="start"
            onClick={() => (isDesktop ? setIsSidebarCollapsed((value) => !value) : setIsMobileOpen(true))}
            aria-label={isDesktop && isSidebarCollapsed ? 'Menüyü genişlet' : 'Menüyü daralt'}
          >
            {isDesktop && !isSidebarCollapsed ? <MenuOpenOutlinedIcon /> : <MenuOutlinedIcon />}
          </IconButton>
          <Stack spacing={0.25} sx={{ minWidth: 0 }}>
            <Stack direction="row" spacing={1.5} sx={{ alignItems: 'center' }}>
              <Typography variant="h6" component="div" sx={{ fontWeight: 800, letterSpacing: 0 }}>
                FIOLIN ONE
              </Typography>
              <Chip label="İş Alanı" size="small" color="primary" variant="outlined" />
            </Stack>
            <Typography variant="body2" color="inherit" sx={{ opacity: 0.78 }}>
              {getBreadcrumb(routeMeta)}
            </Typography>
          </Stack>
        </Toolbar>
      </AppBar>

      <Drawer
        variant={isDesktop ? 'permanent' : 'temporary'}
        open={isDesktop || isMobileOpen}
        onClose={() => setIsMobileOpen(false)}
        ModalProps={{ keepMounted: true }}
        sx={{
          width: { md: drawerWidth },
          flexShrink: 0,
          [`& .MuiDrawer-paper`]: {
            width: isDesktop ? drawerWidth : expandedDrawerWidth,
            boxSizing: 'border-box',
            borderRightColor: 'divider',
            transition: (theme) =>
              theme.transitions.create('width', {
                duration: theme.transitions.duration.shorter,
                easing: theme.transitions.easing.easeInOut,
              }),
          },
        }}
      >
        <Toolbar />
        <Box sx={{ overflow: 'auto', px: 1.25, py: 1.5 }}>
          <List disablePadding sx={{ display: 'grid', gap: 0.5 }}>
            {visibleGroups.map((group) => {
              const groupActive = isGroupActive(group, location.pathname)
              const isOpen = openGroups[group.title] || groupActive
              const visiblePages = group.pages?.filter((page) => hasRole(page.roles)) ?? []

              return (
                <Box key={group.title}>
                  <Tooltip title={isSidebarCollapsed && isDesktop ? group.title : ''} placement="right">
                    <ListItemButton
                      selected={groupActive}
                      onClick={() => handleGroupClick(group)}
                      sx={{
                        minHeight: 48,
                        borderRadius: 2,
                        justifyContent: isSidebarCollapsed && isDesktop ? 'center' : 'flex-start',
                        px: isSidebarCollapsed && isDesktop ? 1 : 1.5,
                      }}
                    >
                      <ListItemIcon
                        sx={{
                          color: groupActive ? 'primary.main' : 'text.secondary',
                          minWidth: isSidebarCollapsed && isDesktop ? 0 : 42,
                          justifyContent: 'center',
                        }}
                      >
                        {group.icon}
                      </ListItemIcon>
                      {(!isSidebarCollapsed || !isDesktop) && (
                        <>
                          <ListItemText
                            primary={group.title}
                            slotProps={{ primary: { sx: { fontWeight: groupActive ? 700 : 600 } } }}
                          />
                          {visiblePages.length > 0 &&
                            (isOpen ? <ExpandLessOutlinedIcon /> : <ExpandMoreOutlinedIcon />)}
                        </>
                      )}
                    </ListItemButton>
                  </Tooltip>

                  {visiblePages.length > 0 && (!isSidebarCollapsed || !isDesktop) && (
                    <Collapse in={isOpen} timeout="auto" unmountOnExit>
                      <List disablePadding sx={{ display: 'grid', gap: 0.25, py: 0.5 }}>
                        {visiblePages.map((page) => {
                          const pageActive = page.match?.(location.pathname) || matchPath(location.pathname, page.path)

                          return (
                            <ListItemButton
                              key={page.path}
                              selected={pageActive}
                              onClick={() => navigateTo(page.path)}
                              sx={{
                                borderRadius: 2,
                                ml: 1,
                                minHeight: 40,
                                pl: 2.5,
                              }}
                            >
                              <ListItemIcon sx={{ minWidth: 36, color: pageActive ? 'primary.main' : 'text.secondary' }}>
                                {page.icon}
                              </ListItemIcon>
                              <ListItemText primary={page.title} slotProps={{ primary: { variant: 'body2' } }} />
                            </ListItemButton>
                          )
                        })}
                      </List>
                    </Collapse>
                  )}
                </Box>
              )
            })}
          </List>
        </Box>
      </Drawer>

      <Box component="main" sx={{ flexGrow: 1, minWidth: 0 }}>
        <Toolbar />
        <Container maxWidth="xl" sx={{ py: { xs: 3, md: 4 } }}>
          <Stack spacing={3}>
            <Box>
              <Typography variant="h4" component="h1" sx={{ fontWeight: 800, letterSpacing: 0 }}>
                {routeMeta.pageTitle ?? routeMeta.moduleTitle}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {getBreadcrumb(routeMeta)}
              </Typography>
            </Box>
            <Routes>
              <Route path="/" element={<Navigate to={getLastPage()} replace />} />
              <Route path={dashboardPath} element={<DashboardPage />} />
              <Route path="/production/dashboard" element={<Navigate to={dashboardPath} replace />} />
              <Route
                path="/operations/goods-receipt"
                element={<GoodsReceiptOperationPage />}
              />
              <Route
                path="/operations/warehouse"
                element={
                  <WorkspaceFoundationPage
                    title="Depo"
                    description="Depo operasyonları için V2 çalışma alanı hazır. Stok ve lokasyon akışları bu yapı üzerinden geliştirilecek."
                    icon={<WarehouseOutlinedIcon color="primary" />}
                  />
                }
              />
              <Route
                path="/operations/order-picking"
                element={<OrderPickingPage />}
              />
              <Route path="/operations/barcode-print" element={<BarcodePrintPage />} />
              <Route
                path="/channels/trendyol"
                element={
                  <WorkspaceFoundationPage
                    title="Trendyol"
                    description="Trendyol kanal yönetimi için temel sayfa hazır. Entegrasyon bağlantıları ve işlem ekranları sonraki sprintte eklenecek."
                    icon={<StorefrontOutlinedIcon color="primary" />}
                  />
                }
              />
              <Route
                path="/channels/website"
                element={
                  <WorkspaceFoundationPage
                    title="Web Sitesi"
                    description="Web sitesi kanal yönetimi için temel sayfa hazır. Ürün, stok ve sipariş senkronizasyonu burada geliştirilecek."
                    icon={<LanguageOutlinedIcon color="primary" />}
                  />
                }
              />
              <Route
                path="/web-management/home"
                element={
                  <WorkspaceFoundationPage
                    title="Web Ana Sayfa"
                    description="Web sitesi ana sayfa içerikleri, vitrin blokları ve yayın akışı bu alanda yönetilecek."
                    icon={<DashboardOutlinedIcon color="primary" />}
                  />
                }
              />
              <Route
                path="/web-management/categories"
                element={
                  <WorkspaceFoundationPage
                    title="Web Kategorileri"
                    description="ERP kategori verileri web tarafında salt okunur kullanılır. Kategori vitrin ayarları bu alanda geliştirilecek."
                    icon={<CategoryOutlinedIcon color="primary" />}
                  />
                }
              />
              <Route
                path="/web-management/collections"
                element={
                  <WorkspaceFoundationPage
                    title="Koleksiyonlar"
                    description="Sezon ve koleksiyon bazlı web vitrinleri için yönetim alanı hazır."
                    icon={<DatasetOutlinedIcon color="primary" />}
                  />
                }
              />
              <Route
                path="/web-management/banners"
                element={
                  <WorkspaceFoundationPage
                    title="Banner Yönetimi"
                    description="Kampanya ve vitrin banner içerikleri ilerleyen sprintlerde buradan yönetilecek."
                    icon={<DatasetOutlinedIcon color="primary" />}
                  />
                }
              />
              <Route
                path="/web-management/sliders"
                element={
                  <WorkspaceFoundationPage
                    title="Slider Yönetimi"
                    description="Ana sayfa hero slider görselleri ve bağlantıları için yönetim alanı hazır."
                    icon={<DatasetOutlinedIcon color="primary" />}
                  />
                }
              />
              <Route
                path="/web-management/pages"
                element={
                  <WorkspaceFoundationPage
                    title="Sayfalar"
                    description="Kurumsal ve bilgi sayfalarının CMS yönetimi için temel ekran hazır."
                    icon={<DatasetOutlinedIcon color="primary" />}
                  />
                }
              />
              <Route
                path="/web-management/menus"
                element={
                  <WorkspaceFoundationPage
                    title="Menüler"
                    description="Web sitesi header ve footer menüleri için yönetim alanı hazır."
                    icon={<MenuOutlinedIcon color="primary" />}
                  />
                }
              />
              <Route
                path="/web-management/seo"
                element={
                  <WorkspaceFoundationPage
                    title="SEO"
                    description="Meta başlıkları, açıklamalar ve yapılandırılmış veri ayarları bu alanda geliştirilecek."
                    icon={<LanguageOutlinedIcon color="primary" />}
                  />
                }
              />
              <Route
                path="/web-management/theme"
                element={
                  <WorkspaceFoundationPage
                    title="Tema Ayarları"
                    description="Renk, tipografi ve vitrin tema ayarları için hazırlık ekranı oluşturuldu."
                    icon={<SettingsOutlinedIcon color="primary" />}
                  />
                }
              />
              <Route
                path="/web-management/publishing"
                element={
                  <WorkspaceFoundationPage
                    title="Yayın Yönetimi"
                    description="Web yayın durumu, taslak ve canlı içerik akışı ilerleyen sprintlerde buradan yönetilecek."
                    icon={<StorefrontOutlinedIcon color="primary" />}
                  />
                }
              />
              <Route path="/products" element={<ProductListPage />} />
              <Route path="/products/import" element={<ProductImportPage />} />
              <Route path="/products/:id" element={<ProductDetailPage />} />
              <Route path="/product-variants" element={<WorkspacePlaceholder title="Varyantlar" />} />
              <Route path="/master-data/:type" element={<MasterDataPage />} />
              <Route path="/purchasing/orders" element={<PurchaseOrderListPage />} />
              <Route path="/purchasing/orders/:id" element={<PurchaseOrderDetailPage />} />
              <Route path="/purchasing/suppliers" element={<SupplierManagementPage />} />
              <Route path="/purchasing/goods-receipts" element={<GoodsReceiptPage />} />
              <Route path="/purchasing/invoices" element={<PurchaseInvoicePage />} />
              <Route path="/fabric/fabrics" element={<FabricListPage />} />
              <Route path="/fabric/fabrics/:id" element={<FabricDetailPage />} />
              <Route path="/fabric/stock" element={<Navigate to="/warehouse/fabric-stock" replace />} />
              <Route path="/warehouse/fabric-stock" element={<FabricStockPage />} />
              <Route path="/fabric/movements" element={<StockMovementsPage />} />
              <Route path="/fabric/reservations" element={<ReservationListPage />} />
              <Route path="/warehouse/product-stock" element={<ProductStockPage />} />
              <Route path="/warehouse/barcodes" element={<WorkspacePlaceholder title="Barkod" />} />
              <Route path="/warehouse/counting" element={<WorkspacePlaceholder title="Sayım" />} />
              <Route path="/production/orders" element={<ProductionListPage />} />
              <Route path="/production/orders/:id" element={<ProductionDetailPage />} />
              <Route path="/production/orders/:id/timeline" element={<ProductionTimelinePage />} />
              <Route path="/production/cutting" element={<ProductionOperationPage kind="cutting" />} />
              <Route path="/production/workshop-shipment" element={<ProductionOperationPage kind="shipment" />} />
              <Route path="/production/workshop-return" element={<ProductionOperationPage kind="return" />} />
              <Route path="/production/packaging" element={<WorkspacePlaceholder title="Paketleme" />} />
              <Route path="/production/warehouse-entry" element={<ProductionOperationPage kind="warehouse" />} />
              <Route path="/production/history" element={<WorkspacePlaceholder title="Süreç Geçmişi" />} />
              <Route path="/sales" element={<SalesOrderListPage />} />
              <Route path="/sales/orders/:id" element={<SalesOrderDetailPage />} />
              <Route path="/reports" element={<ReportsPage />} />
              <Route path="/settings" element={<WorkspacePlaceholder title="Ayarlar" />} />
            </Routes>
          </Stack>
        </Container>
      </Box>
    </Box>
  )
}

function WorkspacePlaceholder({ title }: { title: string }) {
  return (
    <Paper variant="outlined" sx={{ p: { xs: 2.5, md: 4 }, borderRadius: 2 }}>
      <Stack spacing={1.5}>
        <Typography variant="h6" sx={{ fontWeight: 800 }}>
          {title}
        </Typography>
        <Typography color="text.secondary">Bu çalışma alanı için sayfa yerleşimi hazır.</Typography>
      </Stack>
    </Paper>
  )
}

export default App
