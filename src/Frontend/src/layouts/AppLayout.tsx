import { useMemo, useState } from 'react';
import { AppBar, Avatar, Box, Button, Chip, Divider, Drawer, IconButton, List, ListItemButton, ListItemIcon, ListItemText, Stack, Toolbar, Tooltip, Typography, useMediaQuery, useTheme } from '@mui/material';
import DashboardRounded from '@mui/icons-material/DashboardRounded';
import VolunteerActivismRounded from '@mui/icons-material/VolunteerActivismRounded';
import PersonRounded from '@mui/icons-material/PersonRounded';
import LockRounded from '@mui/icons-material/LockRounded';
import LogoutRounded from '@mui/icons-material/LogoutRounded';
import MenuRounded from '@mui/icons-material/MenuRounded';
import RedeemRounded from '@mui/icons-material/RedeemRounded';
import KeyboardArrowLeftRounded from '@mui/icons-material/KeyboardArrowLeftRounded';
import { NavLink, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { BrandMark } from '../components/BrandMark';

const drawerWidth = 276;
const roleCopy = { DONOR: { label: 'אזור תורם', short: 'תורם', avatar: 'ת' }, RECIPIENT: { label: 'אזור נתרם', short: 'נתרם', avatar: 'נ' }, ADMIN: { label: 'ניהול מערכת', short: 'מנהל', avatar: 'מ' } } as const;
const navByRole = {
  DONOR: [{ to: '/donor', label: 'תמונת מצב', icon: <DashboardRounded /> }, { to: '/donor/donations', label: 'התרומות שלי', icon: <VolunteerActivismRounded /> }, { to: '/donor/profile', label: 'פרופיל אישי', icon: <PersonRounded /> }, { to: '/donor/security', label: 'אבטחה וסיסמה', icon: <LockRounded /> }],
  RECIPIENT: [{ to: '/recipient', label: 'תמונת מצב', icon: <DashboardRounded /> }, { to: '/recipient/redemptions', label: 'המימושים שלי', icon: <RedeemRounded /> }, { to: '/recipient/profile', label: 'פרופיל וזכאות', icon: <PersonRounded /> }, { to: '/recipient/security', label: 'אבטחה וסיסמה', icon: <LockRounded /> }],
  ADMIN: [{ to: '/admin', label: 'מרכז בקרה', icon: <DashboardRounded /> }, { to: '/admin/redemptions/pending', label: 'ממתינים לאישור', icon: <RedeemRounded /> }, { to: '/admin/redemptions', label: 'כל המימושים', icon: <VolunteerActivismRounded /> }, { to: '/admin/profile', label: 'פרופיל מנהל', icon: <PersonRounded /> }, { to: '/admin/security', label: 'אבטחה וסיסמה', icon: <LockRounded /> }],
};

export function AppLayout() {
  const { session, signOut } = useAuth();
  const theme = useTheme();
  const desktop = useMediaQuery(theme.breakpoints.up('md'));
  const [open, setOpen] = useState(false);
  const location = useLocation();
  const activeLabel = useMemo(() => session ? [...navByRole[session.role]].sort((a, b) => b.to.length - a.to.length).find((item) => location.pathname.startsWith(item.to))?.label : '', [location.pathname, session]);
  if (!session) return null;
  const role = roleCopy[session.role];
  const drawer = <Stack sx={{ height: '100%', bgcolor: '#093f37', color: '#fff', backgroundImage: 'linear-gradient(180deg, #0a483e 0%, #07372f 100%)' }}>
    <Box sx={{ p: 2.75 }}><BrandMark light /></Box>
    <Divider sx={{ borderColor: 'rgba(255,255,255,.09)' }} />
    <Box sx={{ px: 2, pt: 2.5 }}><Typography variant="overline" sx={{ px: 1.5, color: 'rgba(255,255,255,.42)' }}>תפריט ראשי</Typography></Box>
    <List sx={{ px: 1.5, py: 1, flex: 1 }}>{navByRole[session.role].map((item) => <ListItemButton key={item.to} component={NavLink} to={item.to} end={item.to.split('/').length === 2} onClick={() => setOpen(false)} sx={{ minHeight: 48, borderRadius: '13px', mb: .5, px: 1.5, color: 'rgba(255,255,255,.7)', transition: 'background-color .16s ease, color .16s ease, transform .16s ease', '& .nav-arrow': { opacity: 0, transform: 'translateX(-4px)', transition: '.16s' }, '&.active': { bgcolor: 'rgba(183,227,90,.13)', color: '#d9f99b', '& .nav-arrow': { opacity: 1, transform: 'translateX(0)' }, '&:before': { content: '""', position: 'absolute', insetInlineStart: 0, width: 3, height: 22, borderRadius: 3, bgcolor: 'secondary.main' } }, '&:hover': { bgcolor: 'rgba(255,255,255,.07)', color: '#fff', transform: 'translateX(-2px)' } }}><ListItemIcon sx={{ minWidth: 39, color: 'inherit' }}>{item.icon}</ListItemIcon><ListItemText primary={item.label} slotProps={{ primary: { fontSize: '.91rem', fontWeight: 600 } }} /><KeyboardArrowLeftRounded className="nav-arrow" fontSize="small" /></ListItemButton>)}</List>
    <Box sx={{ p: 1.5 }}><Box sx={{ p: 1.4, border: '1px solid rgba(255,255,255,.1)', bgcolor: 'rgba(255,255,255,.045)', borderRadius: '15px' }}><Stack direction="row" alignItems="center" spacing={1.2}><Avatar sx={{ width: 38, height: 38, bgcolor: 'secondary.main', color: 'primary.dark', fontWeight: 800 }}>{role.avatar}</Avatar><Box sx={{ flex: 1, minWidth: 0 }}><Typography variant="body2" fontWeight={700}>{role.label}</Typography><Typography variant="caption" sx={{ color: 'rgba(255,255,255,.5)' }}>חיבור מאובטח</Typography></Box><Tooltip title="יציאה"><IconButton onClick={signOut} size="small" sx={{ color: 'rgba(255,255,255,.62)' }} aria-label="יציאה מהמערכת"><LogoutRounded fontSize="small" /></IconButton></Tooltip></Stack></Box></Box>
  </Stack>;
  return <Box sx={{ minHeight: '100vh', bgcolor: 'background.default' }}>
    <AppBar position="fixed" color="inherit" elevation={0} sx={{ width: { md: `calc(100% - ${drawerWidth}px)` }, mr: { md: `${drawerWidth}px` }, borderBottom: '1px solid', borderColor: 'divider', bgcolor: 'rgba(255,255,255,.9)', backdropFilter: 'blur(14px)' }}><Toolbar sx={{ minHeight: '68px !important', px: { xs: 2, md: 3.5 } }}><IconButton onClick={() => setOpen(true)} sx={{ display: { md: 'none' }, ml: 1 }} aria-label="פתיחת תפריט"><MenuRounded /></IconButton><Box><Typography variant="caption" color="text.secondary">{role.label}</Typography><Typography variant="body1" fontWeight={720} lineHeight={1.2}>{activeLabel}</Typography></Box><Box sx={{ flex: 1 }} /><Chip label={role.short} size="small" sx={{ bgcolor: 'primary.light', color: 'primary.dark', display: { xs: 'none', sm: 'inline-flex' } }} /><Button color="inherit" onClick={signOut} startIcon={<LogoutRounded />} sx={{ mr: 1, color: 'text.secondary', display: { xs: 'none', sm: 'inline-flex' } }}>יציאה</Button></Toolbar></AppBar>
    <Box component="nav"><Drawer variant={desktop ? 'permanent' : 'temporary'} anchor="right" open={desktop || open} onClose={() => setOpen(false)} ModalProps={{ keepMounted: true }} sx={{ '& .MuiDrawer-paper': { width: drawerWidth, border: 0 } }}>{drawer}</Drawer></Box>
    <Box component="main" sx={{ mr: { md: `${drawerWidth}px` }, pt: '68px', minHeight: '100vh' }}><Box key={location.pathname} sx={{ p: { xs: 2, sm: 3, lg: 4 }, maxWidth: 1380, mx: 'auto', animation: 'page-in .24s ease-out', '@keyframes page-in': { from: { opacity: 0, transform: 'translateY(5px)' }, to: { opacity: 1, transform: 'translateY(0)' } } }}><Outlet /></Box></Box>
  </Box>;
}
