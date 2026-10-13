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
export interface PopulationType { id: number; name: string }

export interface AppError {
  status?: number;
  title: string;
  message: string;
  fieldErrors: Record<string, string[]>;
  traceId?: string;
  kind: 'validation' | 'auth' | 'forbidden' | 'not-found' | 'conflict' | 'network' | 'server' | 'unknown';
}
