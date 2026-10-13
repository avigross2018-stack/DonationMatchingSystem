export const queryKeys = {
  donorProfile: ['donor', 'profile'] as const,
  donations: ['donor', 'donations'] as const,
  recipientProfile: ['recipient', 'profile'] as const,
  recipientRedemptions: ['recipient', 'redemptions'] as const,
  adminProfile: ['admin', 'profile'] as const,
  adminRedemptions: ['admin', 'redemptions'] as const,
  adminRedemption: (id: number) => ['admin', 'redemption', id] as const,
};
