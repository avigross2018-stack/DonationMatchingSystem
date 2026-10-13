export type UserRole = 'DONOR' | 'RECIPIENT' | 'ADMIN';
export type DonationStatus = 'ACTIVE' | 'COMPLETED' | 'CANCELLED' | 'EXPIRED' | string;
export type RedemptionStatus = 'PENDING' | 'COMPLETED' | 'CANCELLED' | 'REJECTED' | string;

export interface LoginRequest { phone: string; password: string }
export interface LoginResponse { accessToken: string; tokenType: string }
export interface RegisterResponse { message: string; userId: number }
export interface RegisterDonorRequest { name: string; phone: string; password: string }
export interface RegisterRecipientRequest extends RegisterDonorRequest {
  populationTypeId: number;
  incomeAmount: number;
  amountOfChildren: number;
  requestReason?: string | null;
}
export interface ChangePasswordRequest { currentPassword: string; newPassword: string }

export interface DonorProfile { id: number; name: string; phone: string; createdAt: string }
export interface UpdateDonorRequest { name: string; phone: string }
export interface RecipientProfile {
  id: number; name: string; phone: string; populationTypeId: number; populationTypeName: string;
  incomeAmount: number; amountOfChildren: number; requestReason?: string | null;
  eligibilityAmount: number; remainingEligibility: number;
}
export interface UpdateRecipientRequest { incomeAmount: number; amountOfChildren: number; requestReason?: string | null }
export interface AdminProfile { adminId: number; userId: number; name: string; phone: string; isActive: boolean; createdAt: string }

export interface Donation {
  id: number; populationTypeId: number; totalAmount: number; remainingAmount: number;
  maxAmountPerRecipient: number; description?: string | null; createdAt: string;
  expirationDate?: string | null; status: DonationStatus;
}
export interface CreateDonationRequest {
  populationTypeId: number; totalAmount: number; maxAmountPerRecipient: number;
  description: string; expirationDate: string;
}
export type UpdateDonationRequest = Omit<CreateDonationRequest, 'populationTypeId'>;
export interface CreateDonationResponse { message: string; donationId: number }

export interface Redemption {
  id: number; donationId: number; amount: number; status: RedemptionStatus;
  donationDescription?: string | null; createdAt: string; redeemedAt?: string | null;
}
export interface AdminRedemptionRecipient {
  id: number; userId: number; name: string; phone: string; populationTypeId: number;
  populationTypeName: string; incomeAmount: number; amountOfChildren: number;
  eligibilityAmount: number; remainingEligibility: number;
}
export interface AdminRedemptionDonation {
  id: number; donorId: number; donorName: string; donorPhone: string; populationTypeId: number;
  populationTypeName: string; totalAmount: number; remainingAmount: number;
  maxAmountPerRecipient: number; description?: string | null; createdAt: string;
  expirationDate?: string | null; status: DonationStatus;
}
export interface AdminRedemption extends Redemption {
  recipientId: number; handledByAdminId?: number | null;
  recipient: AdminRedemptionRecipient; donation: AdminRedemptionDonation;
}
export interface Page<T> { items: T[]; page: number; pageSize: number; totalCount: number; totalPages: number }
export interface AdminRedemptionFilters {
  populationTypeId?: number; status?: string; fromDate?: string; toDate?: string; page: number; pageSize: number;
}
export interface AdminApproval {
  redemptionId: number; status: string; approvedAmount: number; handledByAdminId: number;
  redeemedAt: string; donationRemainingAmount: number; recipientRemainingEligibility: number;
}
export interface AdminRejection { redemptionId: number; status: string; handledByAdminId: number }
export interface PopulationType { id: number; name: string; description?: string | null }

export interface AdminDashboardStatistics {
  generatedAtUtc: string;
  users: { donorsCount: number; recipientsCount: number; activeUsersCount: number; inactiveUsersCount: number };
  donations: { totalCount: number; totalDonationAmount: number; availableDonationCount: number; availableDonationBalance: number; activeCount: number; completedCount: number; cancelledCount: number; expiredCount: number };
  redemptions: { totalCount: number; pendingCount: number; completedCount: number; rejectedCount: number; cancelledCount: number; completedRedemptionAmount: number };
  populationTypes: AdminDashboardPopulationStatistics[];
}
export interface AdminDashboardPopulationStatistics {
  populationTypeId: number; populationTypeName: string; isActive: boolean;
  recipientsCount: number; donationsCount: number; totalDonationAmount: number;
  availableDonationBalance: number; pendingRedemptionsCount: number;
  completedRedemptionsCount: number; rejectedRedemptionsCount: number;
  cancelledRedemptionsCount: number; completedRedemptionAmount: number;
}

export interface AdminDonor {
  id: number; userId: number; name: string; phone: string; isActive: boolean; createdAt: string;
}
export interface AdminDonorDonation extends Donation { populationTypeName: string }
export interface AdminDonorDetails extends AdminDonor { donations: AdminDonorDonation[] }
export interface AdminDonorStatus { donorId: number; userId: number; isActive: boolean; cancelledDonationsCount: number; cancelledRedemptionsCount: number }

export interface AdminRecipient {
  id: number; userId: number; name: string; phone: string; isActive: boolean;
  populationTypeId: number; populationTypeName: string; incomeAmount: number;
  amountOfChildren: number; eligibilityAmount: number; remainingEligibility: number;
}
export interface AdminRecipientRedemption extends Redemption {
  donorName: string; donationTotalAmount: number; donationStatus: DonationStatus;
}
export interface AdminRecipientDetails extends AdminRecipient { requestReason?: string | null; redemptions: AdminRecipientRedemption[] }
export interface UpdateAdminRecipientRequest { name: string; phone: string; incomeAmount: number; amountOfChildren: number; requestReason?: string | null }
export interface AdminRecipientStatus { recipientId: number; userId: number; isActive: boolean; cancelledRedemptionsCount: number }

export interface AdminDonation extends Donation { donorId: number; donorName: string; donorPhone: string; populationTypeName: string }
export interface AdminDonationRedemption { id: number; recipientId: number; recipientName: string; status: RedemptionStatus; amount: number; createdAt: string; redeemedAt?: string | null; handledByAdminId?: number | null }
export interface AdminDonationDetails extends AdminDonation { redemptions: AdminDonationRedemption[] }
export interface AdminDonationFilters { donorId?: number; populationTypeId?: number; status?: string; fromDate?: string; toDate?: string; page: number; pageSize: number }
export type UpdateAdminDonationRequest = UpdateDonationRequest;

export interface AdminPopulationType extends PopulationType { baseEligibilityAmount: number; isActive: boolean }
export interface SavePopulationTypeRequest { name: string; description?: string | null; baseEligibilityAmount: number }

export interface AppError {
  status?: number;
  title: string;
  message: string;
  fieldErrors: Record<string, string[]>;
  traceId?: string;
  kind: 'validation' | 'auth' | 'forbidden' | 'not-found' | 'conflict' | 'network' | 'server' | 'unknown';
}
