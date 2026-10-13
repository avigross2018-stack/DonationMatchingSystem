import { Component, type ErrorInfo, type ReactNode } from 'react';
import { Alert, Box, Button, Typography } from '@mui/material';
export class AppErrorBoundary extends Component<{ children: ReactNode }, { failed: boolean }> {
  state = { failed: false };
  static getDerivedStateFromError() { return { failed: true }; }
  componentDidCatch(error: Error, info: ErrorInfo) { void error; void info; /* Error details intentionally stay out of the UI. */ }
  render() { if (this.state.failed) return <Box sx={{ minHeight: '100vh', display: 'grid', placeItems: 'center', p: 3 }}><Box maxWidth={520}><Typography variant="h2">משהו לא הסתדר</Typography><Alert severity="error" sx={{ my: 2 }}>אירעה שגיאה בתצוגת העמוד. אפשר לטעון מחדש ולנסות שוב.</Alert><Button variant="contained" onClick={() => window.location.reload()}>טעינה מחדש</Button></Box></Box>; return this.props.children; }
}
