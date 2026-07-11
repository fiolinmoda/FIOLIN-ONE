import { Paper, Stack, Typography } from '@mui/material'
import type { ReactNode } from 'react'

type WorkspaceFoundationPageProps = {
  title: string
  description: string
  icon?: ReactNode
}

export function WorkspaceFoundationPage({ title, description, icon }: WorkspaceFoundationPageProps) {
  return (
    <Paper variant="outlined" sx={{ p: { xs: 2.5, md: 4 }, borderRadius: 2 }}>
      <Stack spacing={2}>
        <Stack direction="row" spacing={1.5} sx={{ alignItems: 'center' }}>
          {icon}
          <Typography variant="h6" sx={{ fontWeight: 800 }}>
            {title}
          </Typography>
        </Stack>
        <Typography color="text.secondary">{description}</Typography>
      </Stack>
    </Paper>
  )
}
