import {
  AppBar,
  Box,
  Chip,
  Container,
  Divider,
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Stack,
  Toolbar,
  Typography,
} from '@mui/material'
import AssignmentOutlinedIcon from '@mui/icons-material/AssignmentOutlined'
import CategoryOutlinedIcon from '@mui/icons-material/CategoryOutlined'
import DashboardOutlinedIcon from '@mui/icons-material/DashboardOutlined'
import FactoryOutlinedIcon from '@mui/icons-material/FactoryOutlined'
import Inventory2OutlinedIcon from '@mui/icons-material/Inventory2Outlined'
import LocalShippingOutlinedIcon from '@mui/icons-material/LocalShippingOutlined'
import PriceCheckOutlinedIcon from '@mui/icons-material/PriceCheckOutlined'

const drawerWidth = 280

const futureModules = [
  { name: 'Product Cards', icon: <CategoryOutlinedIcon /> },
  { name: 'Fabric Management', icon: <Inventory2OutlinedIcon /> },
  { name: 'Pattern Management', icon: <AssignmentOutlinedIcon /> },
  { name: 'Production Orders', icon: <FactoryOutlinedIcon /> },
  { name: 'Warehouse', icon: <LocalShippingOutlinedIcon /> },
  { name: 'Finance', icon: <PriceCheckOutlinedIcon /> },
]

function App() {
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
            <Chip label="Foundation" size="small" color="primary" variant="outlined" />
          </Stack>
        </Toolbar>
      </AppBar>

      <Drawer
        variant="permanent"
        sx={{
          width: drawerWidth,
          flexShrink: 0,
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
            {futureModules.map((item) => (
              <ListItem key={item.name}>
                <ListItemIcon>{item.icon}</ListItemIcon>
                <ListItemText primary={item.name} secondary="Planned module" />
              </ListItem>
            ))}
          </List>
        </Box>
      </Drawer>

      <Box component="main" sx={{ flexGrow: 1 }}>
        <Toolbar />
        <Container maxWidth="lg" sx={{ py: 5 }}>
          <Stack spacing={4}>
            <Box>
              <Typography variant="overline" color="primary">
                Clothing manufacturing and wholesale ERP
              </Typography>
              <Typography variant="h3" component="h1" sx={{ mt: 1, fontWeight: 800 }}>
                Project architecture is ready for FIOLIN ONE.
              </Typography>
              <Typography color="text.secondary" sx={{ mt: 2, maxWidth: 760 }}>
                This application shell intentionally contains no business module implementation yet.
                It establishes the frontend foundation that future production, warehouse, dealer,
                barcode, finance, and reporting features can build on.
              </Typography>
            </Box>

            <Divider />

            <Stack direction={{ xs: 'column', md: 'row' }} spacing={3}>
              {['React', 'TypeScript', 'Vite', 'Material UI'].map((item) => (
                <Box
                  key={item}
                  sx={{
                    flex: 1,
                    border: 1,
                    borderColor: 'divider',
                    borderRadius: 1,
                    p: 3,
                    bgcolor: 'background.paper',
                  }}
                >
                  <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
                    {item}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    Frontend foundation
                  </Typography>
                </Box>
              ))}
            </Stack>
          </Stack>
        </Container>
      </Box>
    </Box>
  )
}

export default App
