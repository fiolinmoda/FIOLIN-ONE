import { Container, Paper, Stack, Typography } from '@mui/material'

export default function SimplePage({ title }: { title: string }) {
  return (
    <Container maxWidth="xl" sx={{ py: 7 }}>
      <Paper variant="outlined" sx={{ p: { xs: 3, md: 5 } }}>
        <Stack spacing={1}>
          <Typography variant="h3">{title}</Typography>
          <Typography color="text.secondary">
            Bu sayfa FIOLIN Commerce Engine sonraki fazları için hazırlandı.
          </Typography>
        </Stack>
      </Paper>
    </Container>
  )
}
