import { Box, Stack, Typography } from '@mui/material';
import VolunteerActivismRounded from '@mui/icons-material/VolunteerActivismRounded';

export function BrandMark({ light = false, compact = false }: { light?: boolean; compact?: boolean }) {
  return <Stack direction="row" alignItems="center" spacing={1.25}>
    <Box sx={{ display: 'grid', placeItems: 'center', width: compact ? 38 : 44, height: compact ? 38 : 44, borderRadius: compact ? '12px' : '14px', bgcolor: light ? 'secondary.main' : 'primary.main', color: light ? 'primary.dark' : 'secondary.main', boxShadow: light ? 'none' : '0 8px 22px rgba(12,79,68,.18)' }}><VolunteerActivismRounded fontSize={compact ? 'small' : 'medium'} /></Box>
    <Box><Typography sx={{ fontWeight: 800, fontSize: compact ? '1.05rem' : '1.2rem', lineHeight: 1.1, color: light ? '#fff' : 'primary.dark' }}>חיבור טוב</Typography>{!compact && <Typography variant="caption" sx={{ color: light ? 'rgba(255,255,255,.58)' : 'text.secondary' }}>התאמת תרומות באחריות</Typography>}</Box>
  </Stack>;
}
