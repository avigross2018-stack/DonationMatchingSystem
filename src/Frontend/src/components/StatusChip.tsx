import { Box, Chip } from '@mui/material';
import { statusLabels, statusTone } from '../lib/formatters';
export function StatusChip({ status }: { status: string }) { const tone = statusTone(status); return <Chip size="small" icon={<Box component="span" sx={{ width: 7, height: 7, borderRadius: '50%', bgcolor: 'currentColor' }} />} label={statusLabels[status] || status} color={tone} variant="outlined" sx={{ bgcolor: tone === 'success' ? '#f0f8f4' : tone === 'warning' ? '#fff8ed' : tone === 'error' ? '#fff4f4' : '#f6f7f6', '& .MuiChip-icon': { mr: '8px', ml: '-2px' } }} />; }
