import { Alert, Snackbar } from '@mui/material';
import { createContext, useCallback, useContext, useMemo, useState, type PropsWithChildren } from 'react';

type Severity = 'success' | 'error' | 'warning' | 'info';
interface FeedbackValue { notify: (message: string, severity?: Severity) => void }
const FeedbackContext = createContext<FeedbackValue | null>(null);

export function FeedbackProvider({ children }: PropsWithChildren) {
  const [feedback, setFeedback] = useState<{ message: string; severity: Severity } | null>(null);
  const notify = useCallback((message: string, severity: Severity = 'success') => setFeedback({ message, severity }), []);
  const value = useMemo(() => ({ notify }), [notify]);
  return <FeedbackContext.Provider value={value}>{children}<Snackbar open={Boolean(feedback)} autoHideDuration={5000} onClose={() => setFeedback(null)} anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}><Alert severity={feedback?.severity || 'info'} variant="filled" onClose={() => setFeedback(null)}>{feedback?.message}</Alert></Snackbar></FeedbackContext.Provider>;
}
export function useFeedback() { const value = useContext(FeedbackContext); if (!value) throw new Error('Missing FeedbackProvider'); return value; }
