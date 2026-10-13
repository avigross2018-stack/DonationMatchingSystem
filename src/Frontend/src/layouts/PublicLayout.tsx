import { Box, Button, Container, Stack, Typography } from '@mui/material';
import LoginRounded from '@mui/icons-material/LoginRounded';
import { Link, Outlet } from 'react-router-dom';
import { BrandMark } from '../components/BrandMark';

export function PublicLayout() {
  return <Box sx={{ minHeight: '100vh', display: 'flex', flexDirection: 'column', bgcolor: '#fbfcfa', backgroundImage: 'radial-gradient(circle at 8% 4%, rgba(183,227,90,.14), transparent 26rem)' }}>
    <Box component="header" sx={{ position: 'sticky', top: 0, zIndex: 20, borderBottom: '1px solid rgba(223,231,226,.82)', bgcolor: 'rgba(251,252,250,.86)', backdropFilter: 'blur(16px)' }}>
      <Container maxWidth="xl"><Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ minHeight: 76 }}>
        <Box component={Link} to="/" sx={{ textDecoration: 'none' }} aria-label="חיבור טוב – דף הבית"><BrandMark compact /></Box>
        <Stack component="nav" direction="row" spacing={1} aria-label="ניווט ראשי"><Button component={Link} to="/login" color="inherit" startIcon={<LoginRounded />} sx={{ display: { xs: 'none', sm: 'inline-flex' } }}>כניסה</Button><Button component={Link} to="/register" variant="contained">פתיחת חשבון</Button></Stack>
      </Stack></Container>
    </Box>
    <Box sx={{ flex: 1 }}><Outlet /></Box>
    <Box component="footer" sx={{ mt: 'auto', bgcolor: '#0b3f37', color: '#fff' }}><Container maxWidth="xl"><Stack direction={{ xs: 'column', sm: 'row' }} justifyContent="space-between" alignItems={{ xs: 'flex-start', sm: 'center' }} spacing={2} sx={{ py: 3 }}><BrandMark light compact /><Typography variant="body2" sx={{ color: 'rgba(255,255,255,.62)' }}>סביבת סימולציה להתאמת תרומות · ללא העברת כסף אמיתי</Typography></Stack></Container></Box>
  </Box>;
}
