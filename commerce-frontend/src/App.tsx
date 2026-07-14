import { lazy, Suspense } from 'react'
import { CssBaseline, LinearProgress, ThemeProvider } from '@mui/material'
import { Navigate, Route, Routes } from 'react-router-dom'
import { SiteLayout } from './layout/SiteLayout'
import { commerceTheme } from './theme'

const HomePage = lazy(() => import('./pages/HomePage'))
const ProductPage = lazy(() => import('./pages/ProductPage'))
const ListingPage = lazy(() => import('./pages/ListingPage'))
const SimplePage = lazy(() => import('./pages/SimplePage'))

function App() {
  return (
    <ThemeProvider theme={commerceTheme}>
      <CssBaseline />
      <Suspense fallback={<LinearProgress />}>
        <Routes>
          <Route element={<SiteLayout />}>
            <Route index element={<HomePage />} />
            <Route path="new-season" element={<ListingPage title="Yeni Sezon" />} />
            <Route path="categories" element={<SimplePage title="Kategoriler" />} />
            <Route path="category/:slug" element={<ListingPage title="Kategori" />} />
            <Route path="product/:slug" element={<ProductPage />} />
            <Route path="search" element={<ListingPage title="Arama" />} />
            <Route path="cart" element={<SimplePage title="Sepet" />} />
            <Route path="favorites" element={<SimplePage title="Favoriler" />} />
            <Route path="account" element={<SimplePage title="Hesabım" />} />
            <Route path="orders" element={<SimplePage title="Siparişlerim" />} />
            <Route path="blog" element={<SimplePage title="Blog" />} />
            <Route path="contact" element={<SimplePage title="İletişim" />} />
            <Route path="about" element={<SimplePage title="Hakkımızda" />} />
            <Route path="404" element={<SimplePage title="Sayfa Bulunamadı" />} />
            <Route path="*" element={<Navigate to="/404" replace />} />
          </Route>
        </Routes>
      </Suspense>
    </ThemeProvider>
  )
}

export default App
