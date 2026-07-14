import { useEffect, useState } from 'react'
import { Box, Button, Chip, CircularProgress, Container, Grid, Paper, Stack, Typography } from '@mui/material'
import FavoriteBorderOutlinedIcon from '@mui/icons-material/FavoriteBorderOutlined'
import ShoppingBagOutlinedIcon from '@mui/icons-material/ShoppingBagOutlined'
import { useParams } from 'react-router-dom'
import { commerceApi, type CommerceProductDetail } from '../api/commerceApi'
import { LazyImage } from '../components/LazyImage'

export default function ProductPage() {
  const { slug = '' } = useParams()
  const [product, setProduct] = useState<CommerceProductDetail | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const controller = new AbortController()
    commerceApi
      .getProduct(slug, controller.signal)
      .then(setProduct)
      .finally(() => setIsLoading(false))

    return () => controller.abort()
  }, [slug])

  if (isLoading) {
    return (
      <Container maxWidth="xl" sx={{ py: 10, textAlign: 'center' }}>
        <CircularProgress />
      </Container>
    )
  }

  if (!product) {
    return (
      <Container maxWidth="xl" sx={{ py: 8 }}>
        <Typography variant="h4">Ürün bulunamadı.</Typography>
      </Container>
    )
  }

  const colors = [...new Set(product.variants.map((variant) => variant.color))]
  const sizes = [...new Set(product.variants.map((variant) => variant.size))]

  return (
    <Container maxWidth="xl" sx={{ py: { xs: 4, md: 7 } }}>
      <Grid container spacing={5}>
        <Grid size={{ xs: 12, md: 7 }}>
          <LazyImage src={product.imageUrl} alt={product.productName} aspectRatio="4 / 5" />
        </Grid>
        <Grid size={{ xs: 12, md: 5 }}>
          <Stack spacing={3} sx={{ position: { md: 'sticky' }, top: 96 }}>
            <Box>
              <Typography variant="overline" color="text.secondary">
                {product.modelCode}
              </Typography>
              <Typography variant="h3">{product.productName}</Typography>
              <Typography variant="h5" sx={{ mt: 2 }}>
                {product.price > 0 ? `${product.price.toLocaleString('tr-TR')} TL` : 'Fiyat yakında'}
              </Typography>
            </Box>
            <Stack spacing={1}>
              <Typography sx={{ fontWeight: 800 }}>Renk</Typography>
              <Stack direction="row" spacing={1} flexWrap="wrap">
                {colors.map((color) => (
                  <Chip key={color} label={color} />
                ))}
              </Stack>
            </Stack>
            <Stack spacing={1}>
              <Typography sx={{ fontWeight: 800 }}>Beden</Typography>
              <Stack direction="row" spacing={1} flexWrap="wrap">
                {sizes.map((size) => (
                  <Chip key={size} label={size} variant="outlined" />
                ))}
              </Stack>
            </Stack>
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <Button size="large" variant="contained" startIcon={<ShoppingBagOutlinedIcon />}>
                Sepete Ekle
              </Button>
              <Button size="large" variant="outlined" startIcon={<FavoriteBorderOutlinedIcon />}>
                Favorilere Ekle
              </Button>
            </Stack>
            <Paper variant="outlined" sx={{ p: 3 }}>
              <Typography variant="h6">Ürün Bilgisi</Typography>
              <Typography color="text.secondary">
                İndirim, kombin önerileri, yorumlar ve gerçek sepet akışı sonraki fazlarda bu ürün detayı üzerine bağlanacak.
              </Typography>
            </Paper>
          </Stack>
        </Grid>
      </Grid>
    </Container>
  )
}
