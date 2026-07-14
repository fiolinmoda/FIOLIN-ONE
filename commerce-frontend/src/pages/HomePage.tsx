import { useEffect, useMemo, useState } from 'react'
import { Box, Button, Chip, CircularProgress, Container, Grid, Paper, Stack, Typography } from '@mui/material'
import { Link as RouterLink } from 'react-router-dom'
import { commerceApi, type CommerceHome } from '../api/commerceApi'
import { LazyImage } from '../components/LazyImage'
import { ProductCard } from '../components/ProductCard'

export default function HomePage() {
  const [home, setHome] = useState<CommerceHome | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const controller = new AbortController()
    commerceApi
      .getHome(controller.signal)
      .then(setHome)
      .catch(() => setError('Vitrin verileri şu anda alınamıyor.'))
      .finally(() => setIsLoading(false))

    return () => controller.abort()
  }, [])

  const hero = useMemo(() => home?.heroSlider[0], [home])

  if (isLoading) {
    return (
      <Container maxWidth="xl" sx={{ py: 10, textAlign: 'center' }}>
        <CircularProgress />
      </Container>
    )
  }

  if (error || !home) {
    return (
      <Container maxWidth="xl" sx={{ py: 10 }}>
        <Paper variant="outlined" sx={{ p: 4 }}>
          <Typography>{error}</Typography>
        </Paper>
      </Container>
    )
  }

  return (
    <Stack spacing={8}>
      <Box
        sx={{
          minHeight: { xs: 520, md: 680 },
          display: 'grid',
          alignItems: 'end',
          color: 'white',
          backgroundImage: `linear-gradient(90deg, rgba(0,0,0,.72), rgba(0,0,0,.18)), url(${hero?.imageUrl})`,
          backgroundSize: 'cover',
          backgroundPosition: 'center',
        }}
      >
        <Container maxWidth="xl" sx={{ pb: { xs: 6, md: 10 } }}>
          <Stack spacing={3} sx={{ maxWidth: 680 }}>
            <Chip label="Yeni sezon" sx={{ width: 'fit-content', bgcolor: 'white', color: 'black' }} />
            <Typography variant="h1" sx={{ fontSize: { xs: 42, md: 78 }, lineHeight: 1 }}>
              {hero?.title || 'FIOLIN Yeni Sezon'}
            </Typography>
            <Typography variant="h5" sx={{ color: 'grey.200' }}>
              {hero?.subtitle || 'Günlük şıklık ve zarif gece stilini tek vitrinde buluşturan premium koleksiyon.'}
            </Typography>
            <Button component={RouterLink} to={hero?.linkUrl || '/new-season'} variant="contained" size="large" sx={{ width: 'fit-content' }}>
              Koleksiyonu İncele
            </Button>
          </Stack>
        </Container>
      </Box>

      <Container maxWidth="xl">
        <Stack spacing={6}>
          <Section title="Yeni Sezon" subtitle="ERP ürün kartlarından gelen güncel vitrin ürünleri.">
            {home.newSeason.map((product) => (
              <Grid key={product.id} size={{ xs: 12, sm: 6, md: 3 }}>
                <ProductCard product={product} />
              </Grid>
            ))}
          </Section>

          <Section title="Kategoriler" subtitle="FIOLIN ürün kategorileri.">
            {home.categories.map((category) => (
              <Grid key={category.id} size={{ xs: 12, sm: 6, md: 3 }}>
                <Paper component={RouterLink} to={`/category/${category.slug}`} variant="outlined" sx={{ p: 3, display: 'block', color: 'inherit', textDecoration: 'none' }}>
                  <Typography variant="h6" sx={{ fontWeight: 800 }}>
                    {category.name}
                  </Typography>
                  <Typography color="text.secondary">{category.code}</Typography>
                </Paper>
              </Grid>
            ))}
          </Section>

          <Section title="En Çok Satanlar" subtitle="V1 kapsamında stok hareketlerine göre hazırlanacak alan.">
            {home.bestSellers.map((product) => (
              <Grid key={product.id} size={{ xs: 12, sm: 6, md: 3 }}>
                <ProductCard product={product} />
              </Grid>
            ))}
          </Section>

          <Section title="Yeni Gelenler" subtitle="Son eklenen ürünler ve yeni vitrin seçkileri.">
            {home.newArrivals.map((product) => (
              <Grid key={product.id} size={{ xs: 12, sm: 6, md: 3 }}>
                <ProductCard product={product} />
              </Grid>
            ))}
          </Section>

          <Grid container spacing={3}>
            {home.campaigns.map((banner) => (
              <Grid key={`${banner.title}-${banner.placement}`} size={{ xs: 12, md: 6 }}>
                <Paper sx={{ overflow: 'hidden' }} variant="outlined">
                  <LazyImage src={banner.imageUrl} alt={banner.title} aspectRatio="16 / 9" />
                  <Box sx={{ p: 3 }}>
                    <Typography variant="h5" sx={{ fontWeight: 800 }}>
                      {banner.title}
                    </Typography>
                    <Typography color="text.secondary">{banner.subtitle}</Typography>
                  </Box>
                </Paper>
              </Grid>
            ))}
          </Grid>

          <Paper variant="outlined" sx={{ p: { xs: 3, md: 5 } }}>
            <Typography variant="h4">Instagram Alanı</Typography>
            <Typography color="text.secondary">Sosyal medya vitrin entegrasyonu için hazır alan.</Typography>
          </Paper>

          <Paper variant="outlined" sx={{ p: { xs: 3, md: 5 } }}>
            <Typography variant="h4">Blog</Typography>
            <Typography color="text.secondary">Stil önerileri ve koleksiyon hikayeleri için hazır alan.</Typography>
          </Paper>
        </Stack>
      </Container>
    </Stack>
  )
}

function Section({ title, subtitle, children }: { title: string; subtitle: string; children: React.ReactNode }) {
  return (
    <Stack spacing={3}>
      <Box>
        <Typography variant="h4">{title}</Typography>
        <Typography color="text.secondary">{subtitle}</Typography>
      </Box>
      <Grid container spacing={3}>
        {children}
      </Grid>
    </Stack>
  )
}
