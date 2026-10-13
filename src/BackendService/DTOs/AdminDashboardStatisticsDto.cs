namespace BackendService.DTOs
{
    public sealed class AdminDashboardStatisticsDto
    {
        public DateTime GeneratedAtUtc { get; init; }

        public AdminDashboardUserStatisticsDto Users { get; init; }
            = new();

        public AdminDashboardDonationStatisticsDto Donations { get; init; }
            = new();

        public AdminDashboardRedemptionStatisticsDto Redemptions { get; init; }
            = new();

        public IReadOnlyList<AdminDashboardPopulationStatisticsDto>
            PopulationTypes { get; init; }
            = Array.Empty<AdminDashboardPopulationStatisticsDto>();
    }

    public sealed class AdminDashboardUserStatisticsDto
    {
        public int DonorsCount { get; init; }

        public int RecipientsCount { get; init; }

        public int ActiveUsersCount { get; init; }

        public int InactiveUsersCount { get; init; }
    }

    public sealed class AdminDashboardDonationStatisticsDto
    {
        public int TotalCount { get; init; }

        public decimal TotalDonationAmount { get; init; }

        public int AvailableDonationCount { get; init; }

        public decimal AvailableDonationBalance { get; init; }

        public int ActiveCount { get; init; }

        public int CompletedCount { get; init; }

        public int CancelledCount { get; init; }

        public int ExpiredCount { get; init; }
    }

    public sealed class AdminDashboardRedemptionStatisticsDto
    {
        public int TotalCount { get; init; }

        public int PendingCount { get; init; }

        public int CompletedCount { get; init; }

        public int RejectedCount { get; init; }

        public int CancelledCount { get; init; }

        public decimal CompletedRedemptionAmount { get; init; }
    }

    public sealed class AdminDashboardPopulationStatisticsDto
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