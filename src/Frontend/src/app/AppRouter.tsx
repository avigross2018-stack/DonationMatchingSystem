import { lazy, Suspense } from 'react';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { ProtectedRoute, PublicOnlyRoute } from '../auth/guards';
import { PageLoading } from '../components/States';
import { AppLayout } from '../layouts/AppLayout';
import { PublicLayout } from '../layouts/PublicLayout';

const publicPages = () => import('../features/auth/PublicPages');
const donorPages = () => import('../features/donor/DonorPages');
const recipientPages = () => import('../features/recipient/RecipientPages');
const adminPages = () => import('../features/admin/AdminPages');
const HomePage = lazy(() => publicPages().then((m) => ({ default: m.HomePage })));
const LoginPage = lazy(() => publicPages().then((m) => ({ default: m.LoginPage })));
const RegisterChoicePage = lazy(() => publicPages().then((m) => ({ default: m.RegisterChoicePage })));
const DonorRegisterPage = lazy(() => publicPages().then((m) => ({ default: m.DonorRegisterPage })));
const RecipientRegisterPage = lazy(() => publicPages().then((m) => ({ default: m.RecipientRegisterPage })));
const ForbiddenPage = lazy(() => publicPages().then((m) => ({ default: m.ForbiddenPage })));
const NotFoundPage = lazy(() => publicPages().then((m) => ({ default: m.NotFoundPage })));
const DonorDashboard = lazy(() => donorPages().then((m) => ({ default: m.DonorDashboard })));
const DonationsPage = lazy(() => donorPages().then((m) => ({ default: m.DonationsPage })));
const DonationFormPage = lazy(() => donorPages().then((m) => ({ default: m.DonationFormPage })));
const DonorProfilePage = lazy(() => donorPages().then((m) => ({ default: m.DonorProfilePage })));
const RecipientDashboard = lazy(() => recipientPages().then((m) => ({ default: m.RecipientDashboard })));
const RecipientRedemptionsPage = lazy(() => recipientPages().then((m) => ({ default: m.RecipientRedemptionsPage })));
const RecipientProfilePage = lazy(() => recipientPages().then((m) => ({ default: m.RecipientProfilePage })));
const AdminDashboard = lazy(() => adminPages().then((m) => ({ default: m.AdminDashboard })));
const AdminRedemptionsPage = lazy(() => adminPages().then((m) => ({ default: m.AdminRedemptionsPage })));
const AdminRedemptionDetailPage = lazy(() => adminPages().then((m) => ({ default: m.AdminRedemptionDetailPage })));
const AdminProfilePage = lazy(() => adminPages().then((m) => ({ default: m.AdminProfilePage })));
const SecurityPage = lazy(() => import('../features/auth/SecurityPage').then((m) => ({ default: m.SecurityPage })));

export function AppRouter() {
  return <BrowserRouter><Suspense fallback={<PageLoading />}><Routes>
    <Route element={<PublicLayout />}><Route index element={<HomePage />} /><Route element={<PublicOnlyRoute />}><Route path="login" element={<LoginPage />} /><Route path="register" element={<RegisterChoicePage />} /><Route path="register/donor" element={<DonorRegisterPage />} /><Route path="register/recipient" element={<RecipientRegisterPage />} /></Route></Route>
    <Route path="forbidden" element={<ForbiddenPage />} />
    <Route element={<ProtectedRoute role="DONOR" />}><Route element={<AppLayout />}><Route path="donor" element={<DonorDashboard />} /><Route path="donor/donations" element={<DonationsPage />} /><Route path="donor/donations/new" element={<DonationFormPage />} /><Route path="donor/donations/:id/edit" element={<DonationFormPage />} /><Route path="donor/profile" element={<DonorProfilePage />} /><Route path="donor/security" element={<SecurityPage />} /></Route></Route>
    <Route element={<ProtectedRoute role="RECIPIENT" />}><Route element={<AppLayout />}><Route path="recipient" element={<RecipientDashboard />} /><Route path="recipient/redemptions" element={<RecipientRedemptionsPage />} /><Route path="recipient/profile" element={<RecipientProfilePage />} /><Route path="recipient/security" element={<SecurityPage />} /></Route></Route>
    <Route element={<ProtectedRoute role="ADMIN" />}><Route element={<AppLayout />}><Route path="admin" element={<AdminDashboard />} /><Route path="admin/redemptions" element={<AdminRedemptionsPage />} /><Route path="admin/redemptions/pending" element={<AdminRedemptionsPage pending />} /><Route path="admin/redemptions/:id" element={<AdminRedemptionDetailPage />} /><Route path="admin/profile" element={<AdminProfilePage />} /><Route path="admin/security" element={<SecurityPage />} /></Route></Route>
    <Route path="404" element={<NotFoundPage />} /><Route path="*" element={<Navigate to="/404" replace />} />
  </Routes></Suspense></BrowserRouter>;
}
