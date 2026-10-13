import { Navigate, Outlet, useLocation } from 'react-router-dom';
import type { UserRole } from '../api/types';
import { roleHome, useAuth } from './AuthContext';

export function PublicOnlyRoute() {
  const { session } = useAuth();
  return session ? <Navigate to={roleHome[session.role]} replace /> : <Outlet />;
}

export function ProtectedRoute({ role }: { role: UserRole }) {
  const { session } = useAuth();
  const location = useLocation();
  if (!session) return <Navigate to="/login" replace state={{ from: location.pathname, reason: 'session' }} />;
  if (session.role !== role) return <Navigate to="/forbidden" replace />;
  return <Outlet />;
}
