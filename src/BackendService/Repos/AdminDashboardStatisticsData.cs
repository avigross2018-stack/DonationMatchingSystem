namespace BackendService.Repos
{
    // Aggregated database results used by the admin dashboard.
    public sealed class AdminDashboardStatisticsData
    {
        public int DonorsCount { get; init; }

        public int RecipientsCount { get; init; }

        public int ActiveUsersCount { get; init; }

        public int InactiveUsersCount { get; init; }

        public int TotalDonationsCount { get; init; }

        public decimal TotalDonationAmount { get; init; }

        public int AvailableDonationCount { get; init; }

        public decimal AvailableDonationBalance { get; init; }

        public int ActiveDonationsCount { get; init; }

        public int CompletedDonationsCount { get; init; }

        public int CancelledDonationsCount { get; init; }

        public int ExpiredDonationsCount { get; init; }

        public int TotalRedemptionsCount { get; init; }

        public int PendingRedemptionsCount { get; init; }

        public int CompletedRedemptionsCount { get; init; }

        public int RejectedRedemptionsCount { get; init; }

        public int CancelledRedemptionsCount { get; init; }

        public decimal CompletedRedemptionAmount { get; init; }

        public IReadOnlyList<AdminDashboardPopulationStatisticsData>
            PopulationTypes { get; init; }
            = Array.Empty<AdminDashboardPopulationStatisticsData>();
    }

    public sealed class AdminDashboardPopulationStatisticsData
    {
        public int PopulationTypeId { get; init; }

        public string PopulationTypeName { get; init; } = string.Empty;

        public bool IsActive { get; init; }

        public int RecipientsCount { get; init; }

        public int DonationsCount { get; init; }

        public decimal TotalDonationAmount { get; init; }

        public decimal AvailableDonationBalance { get; init; }

        public int PendingRedemptionsCount { get; init; }

        public int CompletedRedemptionsCount { get; init; }

        public int RejectedRedemptionsCount { get; init; }

        public int CancelledRedemptionsCount { get; init; }

        public decimal CompletedRedemptionAmount { get; init; }
    }
}