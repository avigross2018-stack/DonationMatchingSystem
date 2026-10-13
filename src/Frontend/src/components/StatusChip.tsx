import { Chip } from '@mui/material';
import { statusLabels, statusTone } from '../lib/formatters';
export function StatusChip({ status }: { status: string }) { const tone = statusTone(status); return <Chip size="small" label={statusLabels[status] || status} color={tone} variant="outlined" sx={{ bgcolor: tone === 'success' ? '#f0f8f4' : tone === 'warning' ? '#fff8ed' : tone === 'error' ? '#fff4f4' : '#f6f7f6' }} />; }
