import { Outlet, NavLink } from 'react-router-dom'
import { AppBar, Box, Button, Container, Stack, Toolbar, Typography } from '@mui/material'

const menuItems = [
  { label: 'Yeni Sezon', path: '/new-season' },
  { label: 'Kategoriler', path: '/categories' },
  { label: 'Blog', path: '/blog' },
  { label: 'İletişim', path: '/contact' },
]

export function SiteLayout() {
  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'background.default' }}>
      <AppBar position="sticky" color="inherit" elevation={0} sx={{ borderBottom: 1, borderColor: 'divider' }}>
        <Container maxWidth="xl">
          <Toolbar disableGutters sx={{ gap: 3, minHeight: 72 }}>
            <Typography component={NavLink} to="/" variant="h5" sx={{ color: 'inherit', textDecoration: 'none', fontWeight: 900 }}>
              FIOLIN
            </Typography>
            <Stack direction="row" spacing={0.5} sx={{ display: { xs: 'none', md: 'flex' }, flex: 1 }}>
              {menuItems.map((item) => (
                <Button key={item.path} component={NavLink} to={item.path} color="inherit">
                  {item.label}
                </Button>
              ))}
            </Stack>
            <Button component={NavLink} to="/search" color="inherit">
              Ara
            </Button>
            <Button component={NavLink} to="/favorites" color="inherit">
              Favoriler
            </Button>
            <Button component={NavLink} to="/cart" variant="contained">
              Sepet
            </Button>
          </Toolbar>
        </Container>
      </AppBar>
      <Outlet />
      <Box component="footer" sx={{ bgcolor: '#151515', color: 'white', mt: 8, py: 6 }}>
        <Container maxWidth="xl">
          <Stack direction={{ xs: 'column', md: 'row' }} spacing={3} sx={{ justifyContent: 'space-between' }}>
            <Box>
              <Typography variant="h5" sx={{ fontWeight: 900 }}>
                FIOLIN
              </Typography>
              <Typography color="grey.400">Premium kadın giyim vitrini.</Typography>
            </Box>
            <Stack direction="row" spacing={2}>
              <Typography>Hakkımızda</Typography>
              <Typography>İletişim</Typography>
              <Typography>Blog</Typography>
            </Stack>
          </Stack>
        </Container>
      </Box>
    </Box>
  )
}
