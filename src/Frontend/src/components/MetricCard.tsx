import { Box, Card, CardContent, Stack, Typography } from '@mui/material';

type Tone = 'forest' | 'lime' | 'blue' | 'amber';
const tones: Record<Tone, { bg: string; color: string; line: string }> = {
  forest: { bg: '#e6f1ed', color: '#0c4f44', line: '#70ad9a' },
  lime: { bg: '#eff8dc', color: '#527714', line: '#a8d74c' },
  blue: { bg: '#eaf3f8', color: '#326f91', line: '#77adca' },
  amber: { bg: '#fff3e2', color: '#a65f11', line: '#dfa34e' },
};

export function MetricCard({ label, value, icon, hint, tone = 'forest' }: { label: string; value: string | number; icon: React.ReactNode; hint?: string; tone?: Tone }) {
  const colors = tones[tone];
  return <Card sx={{ height: '100%', position: 'relative', transition: 'transform .2s ease, box-shadow .2s ease', '&:hover': { transform: 'translateY(-2px)', boxShadow: '0 12px 30px rgba(18,45,38,.075)' }, '&:before': { content: '""', position: 'absolute', insetInlineStart: 0, top: 24, bottom: 24, width: 3, borderRadius: '0 4px 4px 0', bgcolor: colors.line } }}><CardContent><Stack direction="row" justifyContent="space-between" alignItems="flex-start" spacing={2}><Box minWidth={0}><Typography color="text.secondary" variant="body2" fontWeight={550}>{label}</Typography><Typography variant="h4" sx={{ mt: 1, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{value}</Typography>{hint && <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: .75 }}>{hint}</Typography>}</Box><Box sx={{ flex: '0 0 auto', display: 'grid', placeItems: 'center', width: 44, height: 44, borderRadius: '13px', bgcolor: colors.bg, color: colors.color }}>{icon}</Box></Stack></CardContent></Card>;
}
