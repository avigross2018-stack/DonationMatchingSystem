import axios, { AxiosError } from 'axios';
import type { AppError } from './types';

const TOKEN_KEY = 'donation-matching-session';

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  timeout: 15_000,
  headers: { 'Content-Type': 'application/json' },
});

apiClient.interceptors.request.use((config) => {
  const token = sessionStorage.getItem(TOKEN_KEY);
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

type ErrorBody = { status?: number; title?: string; detail?: string; message?: string; traceId?: string; errors?: Record<string, string[]> };

export function normalizeError(error: unknown): AppError {
  if (!axios.isAxiosError(error)) {
    return { title: 'שגיאה', message: 'אירעה שגיאה בלתי צפויה.', fieldErrors: {}, kind: 'unknown' };
  }
  const axiosError = error as AxiosError<ErrorBody>;
  const status = axiosError.response?.status;
  const body = axiosError.response?.data;
  if (!axiosError.response) {
    return { title: 'אין חיבור לשרת', message: axiosError.code === 'ECONNABORTED' ? 'הבקשה נמשכה זמן רב מדי. נסו שוב.' : 'לא ניתן להתחבר לשרת. בדקו את החיבור ונסו שוב.', fieldErrors: {}, kind: 'network' };
  }
  const kind: AppError['kind'] = status === 400 ? 'validation' : status === 401 ? 'auth' : status === 403 ? 'forbidden' : status === 404 ? 'not-found' : status === 409 ? 'conflict' : status && status >= 500 ? 'server' : 'unknown';
  const fallback = status === 401 ? 'פרטי ההתחברות אינם תקינים או שהחיבור פג.' : status === 403 ? 'אין לך הרשאה לבצע פעולה זו.' : status === 404 ? 'הפריט המבוקש לא נמצא.' : status === 409 ? 'הנתונים השתנו מאז הטעינה. רעננו ונסו שוב.' : status && status >= 500 ? 'אירעה תקלה בשרת. נסו שוב מאוחר יותר.' : 'לא ניתן להשלים את הפעולה.';
  return {
    status,
    title: body?.title || 'שגיאה',
    message: body?.detail || body?.message || fallback,
    fieldErrors: body?.errors || {},
    traceId: body?.traceId,
    kind,
  };
}

export { TOKEN_KEY };
