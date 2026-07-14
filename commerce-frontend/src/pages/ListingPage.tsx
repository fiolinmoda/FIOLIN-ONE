import { useEffect, useState } from 'react'
import { CircularProgress, Container, Grid, Stack, TextField, Typography } from '@mui/material'
import { commerceApi, type CommerceProduct } from '../api/commerceApi'
import { ProductCard } from '../components/ProductCard'

export default function ListingPage({ title = 'Ürünler' }: { title?: string }) {
  const [search, setSearch] = useState('')
  const [products, setProducts] = useState<CommerceProduct[]>([])
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const controller = new AbortController()
    setIsLoading(true)
    commerceApi
      .getProducts(search, controller.signal)
      .then(setProducts)
      .finally(() => setIsLoading(false))

    return () => controller.abort()
  }, [search])

  return (
    <Container maxWidth="xl" sx={{ py: 6 }}>
      <Stack spacing={3}>
        <Typography variant="h3">{title}</Typography>
        <TextField label="Ürün ara" placeholder="Model kodu, barkod veya ürün adı" value={search} onChange={(event) => setSearch(event.target.value)} />
        {isLoading ? (
          <CircularProgress />
        ) : (
          <Grid container spacing={3}>
            {products.map((product) => (
              <Grid key={product.id} size={{ xs: 12, sm: 6, md: 3 }}>
                <ProductCard product={product} />
              </Grid>
            ))}
          </Grid>
        )}
      </Stack>
    </Container>
  )
}
