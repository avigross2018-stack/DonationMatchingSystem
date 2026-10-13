import '@fontsource/heebo/400.css';
import '@fontsource/heebo/500.css';
import '@fontsource/heebo/700.css';
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { AppErrorBoundary } from './app/AppErrorBoundary';
import { AppProviders } from './app/AppProviders';
import { AppRouter } from './app/AppRouter';

document.documentElement.lang = 'he';
document.documentElement.dir = 'rtl';

createRoot(document.getElementById('root')!).render(<StrictMode><AppErrorBoundary><AppProviders><AppRouter /></AppProviders></AppErrorBoundary></StrictMode>);
