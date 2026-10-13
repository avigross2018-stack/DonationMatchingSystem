import { createContext, useCallback, useContext, useEffect, useMemo, useState, type PropsWithChildren } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { apiClient, TOKEN_KEY } from '../api/client';
import type { UserRole } from '../api/types';

const DOTNET_ROLE = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

interface Session { token: string; userId: number; role: UserRole; expiresAt: number }
interface AuthValue { session: Session | null; ready: boolean; signIn: (token: string) => UserRole; signOut: () => void }
const AuthContext = createContext<AuthValue | null>(null);

function decodeToken(token: string): Session | null {
  try {
    const [, payload] = token.split('.');
    if (!payload) return null;
    const normalized = payload.replace(/-/g, '+').replace(/_/g, '/');
    const parsed = JSON.parse(atob(normalized)) as Record<string, unknown>;
    const role = (parsed.role || parsed[DOTNET_ROLE]) as UserRole | undefined;
    const exp = Number(parsed.exp);
    const userId = Number(parsed.sub);
    if (!['DONOR', 'RECIPIENT', 'ADMIN'].includes(role || '') || !exp || !userId || exp * 1000 <= Date.now()) return null;
    return { token, role: role as UserRole, userId, expiresAt: exp * 1000 };
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: PropsWithChildren) {
  const queryClient = useQueryClient();
  const [session, setSession] = useState<Session | null>(() => {
    const token = sessionStorage.getItem(TOKEN_KEY);
    return token ? decodeToken(token) : null;
  });
  const [ready] = useState(true);

  const signOut = useCallback(() => {
    sessionStorage.removeItem(TOKEN_KEY);
    setSession(null);
    queryClient.clear();
  }, [queryClient]);

  const signIn = useCallback((token: string) => {
    const next = decodeToken(token);
    if (!next) throw new Error('השרת החזיר פרטי התחברות לא תקינים.');
    sessionStorage.setItem(TOKEN_KEY, token);
    setSession(next);
    return next.role;
  }, []);

  useEffect(() => {
    if (!session) sessionStorage.removeItem(TOKEN_KEY);
    const interceptor = apiClient.interceptors.response.use(
      (response) => response,
      (error: unknown) => {
        if ((error as { response?: { status?: number }; config?: { url?: string } }).response?.status === 401 && !(error as { config?: { url?: string } }).config?.url?.includes('/Auth/login')) signOut();
        return Promise.reject(error);
      },
    );
    return () => apiClient.interceptors.response.eject(interceptor);
  }, [session, signOut]);

  useEffect(() => {
    if (!session) return;
    const delay = Math.max(0, session.expiresAt - Date.now());
    const timer = window.setTimeout(signOut, delay);
    return () => window.clearTimeout(timer);
  }, [session, signOut]);

  const value = useMemo(() => ({ session, ready, signIn, signOut }), [session, ready, signIn, signOut]);
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const value = useContext(AuthContext);
  if (!value) throw new Error('useAuth must be used inside AuthProvider');
  return value;
}

export const roleHome: Record<UserRole, string> = { DONOR: '/donor', RECIPIENT: '/recipient', ADMIN: '/admin' };
