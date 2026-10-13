import { apiClient } from './client';
import type * as T from './types';

export const populationTypesApi = {
  listActive: async () => (await apiClient.get<T.PopulationType[]>('/population-types')).data,
};

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
  statistics: async () => (await apiClient.get<T.AdminDashboardStatistics>('/admin/dashboard/statistics')).data,
  donors: async () => (await apiClient.get<T.AdminDonor[]>('/admin/donors')).data,
  donor: async (id: number) => (await apiClient.get<T.AdminDonorDetails>(`/admin/donors/${id}`)).data,
  updateDonor: async (id: number, data: T.UpdateDonorRequest) => (await apiClient.put<T.AdminDonor>(`/admin/donors/${id}`, data)).data,
  setDonorStatus: async (id: number, isActive: boolean) => (await apiClient.patch<T.AdminDonorStatus>(`/admin/donors/${id}/status`, { isActive })).data,
  recipients: async () => (await apiClient.get<T.AdminRecipient[]>('/admin/recipients')).data,
  recipient: async (id: number) => (await apiClient.get<T.AdminRecipientDetails>(`/admin/recipients/${id}`)).data,
  updateRecipient: async (id: number, data: T.UpdateAdminRecipientRequest) => (await apiClient.put<T.AdminRecipient>(`/admin/recipients/${id}`, data)).data,
  setRecipientStatus: async (id: number, isActive: boolean) => (await apiClient.patch<T.AdminRecipientStatus>(`/admin/recipients/${id}/status`, { isActive })).data,
  donations: async (filters: T.AdminDonationFilters) => (await apiClient.get<T.Page<T.AdminDonation>>('/admin/donations', { params: filters })).data,
  donation: async (id: number) => (await apiClient.get<T.AdminDonationDetails>(`/admin/donations/${id}`)).data,
  updateDonation: async (id: number, data: T.UpdateAdminDonationRequest) => { await apiClient.put(`/admin/donations/${id}`, data); },
  cancelDonation: async (id: number) => { await apiClient.post(`/admin/donations/${id}/cancel`); },
  populationTypes: async () => (await apiClient.get<T.AdminPopulationType[]>('/admin/population-types')).data,
  populationType: async (id: number) => (await apiClient.get<T.AdminPopulationType>(`/admin/population-types/${id}`)).data,
  createPopulationType: async (data: T.SavePopulationTypeRequest) => (await apiClient.post<T.AdminPopulationType>('/admin/population-types', data)).data,
  updatePopulationType: async (id: number, data: T.SavePopulationTypeRequest) => (await apiClient.put<T.AdminPopulationType>(`/admin/population-types/${id}`, data)).data,
  setPopulationTypeStatus: async (id: number, isActive: boolean) => (await apiClient.patch<T.AdminPopulationType>(`/admin/population-types/${id}/status`, { isActive })).data,
  redemptions: async (filters: T.AdminRedemptionFilters, pending = false) => (await apiClient.get<T.Page<T.AdminRedemption>>(`/admin/redemptions${pending ? '/pending' : ''}`, { params: filters })).data,
  redemption: async (id: number) => (await apiClient.get<T.AdminRedemption>(`/admin/redemptions/${id}`)).data,
  approve: async (id: number) => (await apiClient.post<T.AdminApproval>(`/admin/redemptions/${id}/approve`)).data,
  reject: async (id: number) => (await apiClient.post<T.AdminRejection>(`/admin/redemptions/${id}/reject`)).data,
};
