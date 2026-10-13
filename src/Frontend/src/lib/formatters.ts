export const formatCurrency = (value: number) => new Intl.NumberFormat('he-IL', { style: 'currency', currency: 'ILS', maximumFractionDigits: 2 }).format(value);
export const formatDate = (value?: string | null) => value ? new Intl.DateTimeFormat('he-IL', { dateStyle: 'medium' }).format(new Date(value)) : '—';
export const statusLabels: Record<string, string> = { ACTIVE: 'פעילה', COMPLETED: 'הושלמה', CANCELLED: 'בוטלה', EXPIRED: 'פגה', PENDING: 'ממתין לאישור', REJECTED: 'נדחה' };
export const statusTone = (status: string): 'success' | 'warning' | 'error' | 'default' => status === 'ACTIVE' || status === 'COMPLETED' ? 'success' : status === 'PENDING' ? 'warning' : status === 'CANCELLED' || status === 'REJECTED' || status === 'EXPIRED' ? 'error' : 'default';
