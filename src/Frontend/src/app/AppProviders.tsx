import { CacheProvider } from '@emotion/react';
import { CssBaseline, ThemeProvider } from '@mui/material';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useState, type PropsWithChildren } from 'react';
import { AuthProvider } from '../auth/AuthContext';
import { FeedbackProvider } from './FeedbackContext';
import { rtlCache, theme } from './theme';

export function AppProviders({ children }: PropsWithChildren) {
  const [queryClient] = useState(() => new QueryClient({ defaultOptions: { queries: { retry: 1, staleTime: 30_000, refetchOnWindowFocus: false }, mutations: { retry: false } } }));
  return <CacheProvider value={rtlCache}><ThemeProvider theme={theme}><CssBaseline /><QueryClientProvider client={queryClient}><AuthProvider><FeedbackProvider>{children}</FeedbackProvider></AuthProvider></QueryClientProvider></ThemeProvider></CacheProvider>;
}
