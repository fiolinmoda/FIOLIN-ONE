import { memo } from 'react'
import { Card, CardActionArea, CardContent, Stack, Typography } from '@mui/material'
import { useNavigate } from 'react-router-dom'
import type { CommerceProduct } from '../api/commerceApi'
import { LazyImage } from './LazyImage'

type ProductCardProps = {
  product: CommerceProduct
}

export const ProductCard = memo(function ProductCard({ product }: ProductCardProps) {
  const navigate = useNavigate()

  return (
    <Card variant="outlined" sx={{ height: '100%', overflow: 'hidden' }}>
      <CardActionArea onClick={() => navigate(`/product/${product.slug}`)} sx={{ height: '100%' }}>
        <LazyImage src={product.imageUrl} alt={product.productName} />
        <CardContent>
          <Stack spacing={0.5}>
            <Typography variant="overline" color="text.secondary">
              {product.modelCode}
            </Typography>
            <Typography variant="subtitle1" sx={{ fontWeight: 800 }}>
              {product.productName}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {product.category || 'FIOLIN'}
            </Typography>
            <Typography variant="subtitle2" sx={{ fontWeight: 800 }}>
              {product.price > 0 ? `${product.price.toLocaleString('tr-TR')} TL` : 'Fiyat yakında'}
            </Typography>
          </Stack>
        </CardContent>
      </CardActionArea>
    </Card>
  )
})
