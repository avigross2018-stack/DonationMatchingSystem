import { apiClient } from './client';
import type * as T from './types';

export const authApi = {
  login: async (data: T.LoginRequest) => (await apiClient.post<T.LoginResponse>('/Auth/login', data)).data,
  registerDonor: async (data: T.RegisterDonorRequest) => (await apiClient.post<T.RegisterResponse>('/Auth/register/donor', data)).data,
  registerRecipient: async (data: T.RegisterRecipientRequest) => (await apiClient.post<T.RegisterResponse>('/Auth/register/recipient', data)).data,
  changePassword: async (data: T.ChangePasswordRequest) => { await apiClient.put('/Auth/change-password', data); },
};
export const donorApi = {
  profile: async () => (await apiClient.get<T.DonorProfile>('/donors/me')).data,
  update: async (data: T.UpdateDonorRequest) => { await apiClient.put('/donors/me', data); },
  deactivate: async () => { await apiClient.delete('/donors/me'); },
  donations: async () => (await apiClient.get<T.Donation[]>('/donations/show-donations')).data,
  createDonation: async (data: T.CreateDonationRequest) => (await apiClient.post<T.CreateDonationResponse>('/donations', data)).data,
  updateDonation: async (id: number, data: T.UpdateDonationRequest) => { await apiClient.put(`/donations/${id}`, data); },
  cancelDonation: async (id: number) => { await apiClient.post(`/donations/${id}/cancel`); },
};
export const recipientApi = {
  profile: async () => (await apiClient.get<T.RecipientProfile>('/recipients/me')).data,
  update: async (data: T.UpdateRecipientRequest) => { await apiClient.put('/recipients/me', data); },
  redemptions: async () => (await apiClient.get<T.Redemption[]>('/recipients/me/redemptions')).data,
  deactivate: async () => { await apiClient.delete('/recipients/me'); },
};
export const adminApi = {
  profile: async () => (await apiClient.get<T.AdminProfile>('/admin/me')).data,
  redemptions: async (filters: T.AdminRedemptionFilters, pending = false) => (await apiClient.get<T.Page<T.AdminRedemption>>(`/admin/redemptions${pending ? '/pending' : ''}`, { params: filters })).data,
  redemption: async (id: number) => (await apiClient.get<T.AdminRedemption>(`/admin/redemptions/${id}`)).data,
  approve: async (id: number) => (await apiClient.post<T.AdminApproval>(`/admin/redemptions/${id}/approve`)).data,
  reject: async (id: number) => (await apiClient.post<T.AdminRejection>(`/admin/redemptions/${id}/reject`)).data,
};
