import { createTheme } from '@mui/material/styles'

export const commerceTheme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#151515',
    },
    secondary: {
      main: '#b4935a',
    },
    background: {
      default: '#fafafa',
      paper: '#ffffff',
    },
  },
  shape: {
    borderRadius: 8,
  },
  typography: {
    fontFamily: 'Inter, Arial, sans-serif',
    h1: { fontWeight: 800, letterSpacing: 0 },
    h2: { fontWeight: 800, letterSpacing: 0 },
    h3: { fontWeight: 800, letterSpacing: 0 },
    h4: { fontWeight: 800, letterSpacing: 0 },
    h5: { fontWeight: 800, letterSpacing: 0 },
    button: { fontWeight: 700, letterSpacing: 0, textTransform: 'none' },
  },
})
