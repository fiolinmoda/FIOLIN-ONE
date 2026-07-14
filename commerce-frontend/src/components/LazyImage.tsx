import { Box } from '@mui/material'

type LazyImageProps = {
  src?: string | null
  alt: string
  aspectRatio?: string
}

export function LazyImage({ src, alt, aspectRatio = '4 / 5' }: LazyImageProps) {
  return (
    <Box
      component="img"
      src={src || 'https://images.unsplash.com/photo-1503342217505-b0a15ec3261c?auto=format&fit=crop&w=900&q=80'}
      alt={alt}
      loading="lazy"
      sx={{
        width: '100%',
        aspectRatio,
        objectFit: 'cover',
        bgcolor: 'grey.100',
      }}
    />
  )
}
