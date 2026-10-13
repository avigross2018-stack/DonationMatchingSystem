import createCache from '@emotion/cache';
import { alpha, createTheme } from '@mui/material/styles';
import type { Shadows } from '@mui/material/styles';
import rtlPlugin from 'stylis-plugin-rtl';
import { prefixer } from 'stylis';

export const rtlCache = createCache({ key: 'muirtl', stylisPlugins: [prefixer, rtlPlugin] });

const forest = '#0c4f44';
const ink = '#172320';
const lime = '#b7e35a';
const border = '#dfe7e2';

export const theme = createTheme({
  direction: 'rtl',
  palette: {
    mode: 'light',
    primary: { main: forest, dark: '#073a33', light: '#e2f0ec', contrastText: '#ffffff' },
    secondary: { main: lime, dark: '#86b72f', light: '#eff8dc', contrastText: '#17340c' },
    background: { default: '#f5f7f5', paper: '#ffffff' },
    text: { primary: ink, secondary: '#64736e' },
    divider: border,
    warning: { main: '#b86b16', light: '#fff5e8' },
    error: { main: '#bd3d48', light: '#fff0f1' },
    success: { main: '#23795c', light: '#e9f6f0' },
    info: { main: '#326f91', light: '#edf6fa' },
  },
  typography: {
    fontFamily: 'Heebo, Arial, sans-serif',
    h1: { fontSize: 'clamp(2.65rem, 6.2vw, 5.4rem)', fontWeight: 760, lineHeight: 1.02, letterSpacing: '-0.045em' },
    h2: { fontSize: 'clamp(1.8rem, 3vw, 2.55rem)', fontWeight: 750, lineHeight: 1.18, letterSpacing: '-0.028em' },
    h3: { fontSize: '1.24rem', fontWeight: 720, lineHeight: 1.3, letterSpacing: '-0.012em' },
    h4: { fontSize: '1.75rem', fontWeight: 750, letterSpacing: '-0.025em' },
    body1: { fontSize: '1rem', lineHeight: 1.65 },
    body2: { fontSize: '.9rem', lineHeight: 1.55 },
    button: { fontWeight: 700, letterSpacing: 0 },
    overline: { fontSize: '.72rem', fontWeight: 750, letterSpacing: '.08em' },
  },
  shape: { borderRadius: 16 },
  shadows: ['none', '0 1px 2px rgba(18, 45, 38, .04)', '0 2px 8px rgba(18, 45, 38, .05)', '0 8px 24px rgba(18, 45, 38, .07)', '0 14px 36px rgba(18, 45, 38, .08)', ...Array(20).fill('0 18px 50px rgba(18, 45, 38, .1)')] as Shadows,
  components: {
    MuiCssBaseline: { styleOverrides: { html: { scrollBehavior: 'smooth' }, body: { minWidth: 320, backgroundColor: '#f5f7f5' }, '::selection': { backgroundColor: alpha(lime, .55), color: ink }, 'a:focus-visible, button:focus-visible, input:focus-visible, textarea:focus-visible': { outline: `3px solid ${alpha('#78a92d', .35)}`, outlineOffset: 2 }, '@media (prefers-reduced-motion: reduce)': { '*, *::before, *::after': { scrollBehavior: 'auto !important', transitionDuration: '0.01ms !important', animationDuration: '0.01ms !important', animationIterationCount: '1 !important' } } } },
    MuiButton: { defaultProps: { disableElevation: true }, styleOverrides: { root: { borderRadius: 12, minHeight: 44, paddingInline: 20, textTransform: 'none', transition: 'transform .18s ease, background-color .18s ease, box-shadow .18s ease', '&:active:not(:disabled)': { transform: 'translateY(1px)' } }, containedPrimary: { boxShadow: `0 8px 20px ${alpha(forest, .18)}`, '&:hover': { boxShadow: `0 10px 24px ${alpha(forest, .24)}`, backgroundColor: '#0a443b' } }, outlined: { borderColor: '#cbd8d2', '&:hover': { borderColor: forest, backgroundColor: alpha(forest, .035) } } } },
    MuiCard: { defaultProps: { elevation: 0 }, styleOverrides: { root: { border: `1px solid ${border}`, borderRadius: 20, boxShadow: '0 1px 2px rgba(18,45,38,.025)', overflow: 'hidden' } } },
    MuiCardContent: { styleOverrides: { root: { padding: 24, '&:last-child': { paddingBottom: 24 } } } },
    MuiPaper: { styleOverrides: { root: { backgroundImage: 'none' } } },
    MuiTextField: { defaultProps: { fullWidth: true, size: 'small' } },
    MuiOutlinedInput: { styleOverrides: { root: { borderRadius: 12, backgroundColor: '#fbfcfb', transition: 'background-color .15s ease, box-shadow .15s ease', '&:hover': { backgroundColor: '#fff' }, '&.Mui-focused': { backgroundColor: '#fff', boxShadow: `0 0 0 4px ${alpha(forest, .07)}` } }, notchedOutline: { borderColor: '#d8e1dc' } } },
    MuiInputLabel: { styleOverrides: { root: { fontWeight: 500 } } },
    MuiAlert: { styleOverrides: { root: { borderRadius: 14, alignItems: 'center' }, standardInfo: { backgroundColor: '#edf6f3' }, outlinedInfo: { borderColor: '#bcd8cf' } } },
    MuiChip: { styleOverrides: { root: { borderRadius: 9, fontWeight: 650 }, sizeSmall: { height: 27, fontSize: '.76rem' } } },
    MuiTableCell: { styleOverrides: { head: { color: '#687872', fontWeight: 700, fontSize: '.76rem', backgroundColor: '#f8faf8', borderBottom: `1px solid ${border}`, whiteSpace: 'nowrap' }, body: { borderBottom: '1px solid #edf1ee', paddingTop: 15, paddingBottom: 15 } } },
    MuiTableRow: { styleOverrides: { root: { transition: 'background-color .15s ease', '&:last-child td': { borderBottom: 0 } } } },
    MuiDialog: { styleOverrides: { paper: { borderRadius: 22, border: `1px solid ${border}`, boxShadow: '0 28px 80px rgba(12,42,35,.2)' } } },
    MuiSkeleton: { styleOverrides: { root: { borderRadius: 10, backgroundColor: '#e8eeea' } } },
    MuiPaginationItem: { styleOverrides: { root: { borderRadius: 9 } } },
  },
});
