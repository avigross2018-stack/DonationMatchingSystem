using BackendService.DTOs;
using BackendService.Repos;

namespace BackendService.Services
{
    public sealed class AdminStatisticsService
    {
        private readonly IAdminDashboardStatisticsRepo _statisticsRepo;

        public AdminStatisticsService(
            IAdminDashboardStatisticsRepo statisticsRepo)
        {
            _statisticsRepo = statisticsRepo;
        }

        public async Task<AdminDashboardStatisticsDto> GetStatisticsAsync(
            CancellationToken cancellationToken = default)
        {
            // Use one timestamp for all availability calculations.
            var generatedAtUtc = DateTime.UtcNow;

            var data = await _statisticsRepo.GetStatisticsAsync(
                generatedAtUtc,
                cancellationToken);

            return new AdminDashboardStatisticsDto
            {
                GeneratedAtUtc = generatedAtUtc,

                Users = new AdminDashboardUserStatisticsDto
                {
                    DonorsCount = data.DonorsCount,
                    RecipientsCount = data.RecipientsCount,
                    ActiveUsersCount = data.ActiveUsersCount,
                    InactiveUsersCount = data.InactiveUsersCount
                },

                Donations = new AdminDashboardDonationStatisticsDto
                {
                    TotalCount = data.TotalDonationsCount,
                    TotalDonationAmount = data.TotalDonationAmount,
                    AvailableDonationCount = data.AvailableDonationCount,
                    AvailableDonationBalance = data.AvailableDonationBalance,
                    ActiveCount = data.ActiveDonationsCount,
                    CompletedCount = data.CompletedDonationsCount,
                    CancelledCount = data.CancelledDonationsCount,
                    ExpiredCount = data.ExpiredDonationsCount
                },

                Redemptions = new AdminDashboardRedemptionStatisticsDto
                {
                    TotalCount = data.TotalRedemptionsCount,
                    PendingCount = data.PendingRedemptionsCount,
                    CompletedCount = data.CompletedRedemptionsCount,
                    RejectedCount = data.RejectedRedemptionsCount,
                    CancelledCount = data.CancelledRedemptionsCount,
                    CompletedRedemptionAmount =
                        data.CompletedRedemptionAmount
                },

                PopulationTypes = data.PopulationTypes
                    .Select(population => new
                        AdminDashboardPopulationStatisticsDto
                    {
                        PopulationTypeId = population.PopulationTypeId,
                        PopulationTypeName = population.PopulationTypeName,
                        IsActive = population.IsActive,
                        RecipientsCount = population.RecipientsCount,
                        DonationsCount = population.DonationsCount,
                        TotalDonationAmount = population.TotalDonationAmount,
                        AvailableDonationBalance =
                            population.AvailableDonationBalance,
                        PendingRedemptionsCount =
                            population.PendingRedemptionsCount,
                        CompletedRedemptionsCount =
                            population.CompletedRedemptionsCount,
                        RejectedRedemptionsCount =
                            population.RejectedRedemptionsCount,
                        CancelledRedemptionsCount =
                            population.CancelledRedemptionsCount,
                        CompletedRedemptionAmount =
                            population.CompletedRedemptionAmount
                    })
                    .ToList()
            };
        }
    }
}