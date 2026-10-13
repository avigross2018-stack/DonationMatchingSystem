using BackendService.Data;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Repos
{
    public sealed class AdminDashboardStatisticsRepo
        : IAdminDashboardStatisticsRepo
    {
        private readonly AppDbContext _appDb;

        public AdminDashboardStatisticsRepo(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task<AdminDashboardStatisticsData> GetStatisticsAsync(
            DateTime utcNow,
            CancellationToken cancellationToken = default)
        {
            // Count active and inactive user accounts.
            var userStatusRows = await _appDb.Users
                .AsNoTracking()
                .GroupBy(user => user.IsActive)
                .Select(group => new UserStatusAggregate
                {
                    IsActive = group.Key,
                    Count = group.Count()
                })
                .ToListAsync(cancellationToken);

            var donorCount = await _appDb.Donors
                .CountAsync(cancellationToken);

            var recipientCount = await _appDb.Recipients
                .CountAsync(cancellationToken);

            // Aggregate recipient counts by population type.
            var recipientPopulationRows = await _appDb.Recipients
                .AsNoTracking()
                .GroupBy(recipient => recipient.PopulationTypeId)
                .Select(group => new RecipientPopulationAggregate
                {
                    PopulationTypeId = group.Key,
                    Count = group.Count()
                })
                .ToListAsync(cancellationToken);

            // Aggregate donation totals and status counts in the database.
            var donationStatusRows = await _appDb.Donations
                .AsNoTracking()
                .GroupBy(donation => donation.Status)
                .Select(group => new DonationStatusAggregate
                {
                    Status = group.Key,
                    Count = group.Count(),
                    TotalAmount =
                        group.Sum(donation =>
                            (decimal?)donation.TotalAmount) ?? 0m,

                    AvailableCount = group.Count(donation =>
                        donation.Status == "ACTIVE" &&
                        (donation.ExpirationDate == null ||
                         donation.ExpirationDate > utcNow) &&
                        donation.RemainingAmount > 0 &&
                        donation.Donor.User.IsActive),

                    AvailableBalance = group.Sum(donation =>
                        donation.Status == "ACTIVE" &&
                        (donation.ExpirationDate == null ||
                         donation.ExpirationDate > utcNow) &&
                        donation.RemainingAmount > 0 &&
                        donation.Donor.User.IsActive
                            ? (decimal?)donation.RemainingAmount
                            : 0m) ?? 0m
                })
                .ToListAsync(cancellationToken);

            // Aggregate donations by population type.
            var donationPopulationRows = await _appDb.Donations
                .AsNoTracking()
                .GroupBy(donation => donation.PopulationTypeId)
                .Select(group => new DonationPopulationAggregate
                {
                    PopulationTypeId = group.Key,
                    Count = group.Count(),

                    TotalAmount =
                        group.Sum(donation =>
                            (decimal?)donation.TotalAmount) ?? 0m,

                    AvailableBalance = group.Sum(donation =>
                        donation.Status == "ACTIVE" &&
                        (donation.ExpirationDate == null ||
                         donation.ExpirationDate > utcNow) &&
                        donation.RemainingAmount > 0 &&
                        donation.Donor.User.IsActive
                            ? (decimal?)donation.RemainingAmount
                            : 0m) ?? 0m
                })
                .ToListAsync(cancellationToken);

            // Aggregate redemption totals and amounts.
            var redemptionStatusRows = await _appDb.Redemptions
                .AsNoTracking()
                .GroupBy(redemption => redemption.Status)
                .Select(group => new RedemptionStatusAggregate
                {
                    Status = group.Key,
                    Count = group.Count(),

                    CompletedAmount =
                        group.Sum(redemption =>
                            redemption.Status == "COMPLETED"
                                ? (decimal?)redemption.Amount
                                : 0m) ?? 0m
                })
                .ToListAsync(cancellationToken);

            // Aggregate redemption statistics by donation population type.
            var redemptionPopulationRows = await _appDb.Redemptions
                .AsNoTracking()
                .GroupBy(redemption =>
                    redemption.Donation.PopulationTypeId)
                .Select(group => new RedemptionPopulationAggregate
                {
                    PopulationTypeId = group.Key,

                    PendingCount = group.Count(redemption =>
                        redemption.Status == "PENDING"),

                    CompletedCount = group.Count(redemption =>
                        redemption.Status == "COMPLETED"),

                    RejectedCount = group.Count(redemption =>
                        redemption.Status == "REJECTED"),

                    CancelledCount = group.Count(redemption =>
                        redemption.Status == "CANCELLED"),

                    CompletedAmount =
                        group.Sum(redemption =>
                            redemption.Status == "COMPLETED"
                                ? (decimal?)redemption.Amount
                                : 0m) ?? 0m
                })
                .ToListAsync(cancellationToken);

            // Include inactive population types to preserve historical reporting.
            var populationTypes = await _appDb.PopulationTypes
                .AsNoTracking()
                .OrderBy(populationType => populationType.Id)
                .Select(populationType => new PopulationTypeAggregate
                {
                    Id = populationType.Id,
                    Name = populationType.Name,
                    IsActive = populationType.IsActive
                })
                .ToListAsync(cancellationToken);

            // Prepare lookup dictionaries to combine the aggregated results.
            var recipientsByPopulation = recipientPopulationRows
                .ToDictionary(row => row.PopulationTypeId);

            var donationsByPopulation = donationPopulationRows
                .ToDictionary(row => row.PopulationTypeId);

            var redemptionsByPopulation = redemptionPopulationRows
                .ToDictionary(row => row.PopulationTypeId);

            var populationStatistics = populationTypes
                .Select(populationType =>
                {
                    recipientsByPopulation.TryGetValue(
                        populationType.Id,
                        out var recipients);

                    donationsByPopulation.TryGetValue(
                        populationType.Id,
                        out var donations);

                    redemptionsByPopulation.TryGetValue(
                        populationType.Id,
                        out var redemptions);

                    return new AdminDashboardPopulationStatisticsData
                    {
                        PopulationTypeId = populationType.Id,
                        PopulationTypeName = populationType.Name,
                        IsActive = populationType.IsActive,

                        RecipientsCount = recipients?.Count ?? 0,

                        DonationsCount = donations?.Count ?? 0,

                        TotalDonationAmount =
                            donations?.TotalAmount ?? 0m,

                        AvailableDonationBalance =
                            donations?.AvailableBalance ?? 0m,

                        PendingRedemptionsCount =
                            redemptions?.PendingCount ?? 0,

                        CompletedRedemptionsCount =
                            redemptions?.CompletedCount ?? 0,

                        RejectedRedemptionsCount =
                            redemptions?.RejectedCount ?? 0,

                        CancelledRedemptionsCount =
                            redemptions?.CancelledCount ?? 0,

                        CompletedRedemptionAmount =
                            redemptions?.CompletedAmount ?? 0m
                    };
                })
                .ToList();

            // Read overall counts from the grouped aggregates.
            var activeDonationRows = donationStatusRows
                .FirstOrDefault(row => row.Status == "ACTIVE");

            var completedDonationRows = donationStatusRows
                .FirstOrDefault(row => row.Status == "COMPLETED");

            var cancelledDonationRows = donationStatusRows
                .FirstOrDefault(row => row.Status == "CANCELLED");

            var expiredDonationRows = donationStatusRows
                .FirstOrDefault(row => row.Status == "EXPIRED");

            var pendingRedemptionRows = redemptionStatusRows
                .FirstOrDefault(row => row.Status == "PENDING");

            var completedRedemptionRows = redemptionStatusRows
                .FirstOrDefault(row => row.Status == "COMPLETED");

            var rejectedRedemptionRows = redemptionStatusRows
                .FirstOrDefault(row => row.Status == "REJECTED");

            var cancelledRedemptionRows = redemptionStatusRows
                .FirstOrDefault(row => row.Status == "CANCELLED");

            return new AdminDashboardStatisticsData
            {
                DonorsCount = donorCount,
                RecipientsCount = recipientCount,

                ActiveUsersCount = userStatusRows
                    .Where(row => row.IsActive)
                    .Sum(row => row.Count),

                InactiveUsersCount = userStatusRows
                    .Where(row => !row.IsActive)
                    .Sum(row => row.Count),

                TotalDonationsCount = donationStatusRows
                    .Sum(row => row.Count),

                TotalDonationAmount = donationStatusRows
                    .Sum(row => row.TotalAmount),

                AvailableDonationCount = donationStatusRows
                    .Sum(row => row.AvailableCount),

                AvailableDonationBalance = donationStatusRows
                    .Sum(row => row.AvailableBalance),

                ActiveDonationsCount =
                    activeDonationRows?.Count ?? 0,

                CompletedDonationsCount =
                    completedDonationRows?.Count ?? 0,

                CancelledDonationsCount =
                    cancelledDonationRows?.Count ?? 0,

                ExpiredDonationsCount =
                    expiredDonationRows?.Count ?? 0,

                TotalRedemptionsCount = redemptionStatusRows
                    .Sum(row => row.Count),

                PendingRedemptionsCount =
                    pendingRedemptionRows?.Count ?? 0,

                CompletedRedemptionsCount =
                    completedRedemptionRows?.Count ?? 0,

                RejectedRedemptionsCount =
                    rejectedRedemptionRows?.Count ?? 0,

                CancelledRedemptionsCount =
                    cancelledRedemptionRows?.Count ?? 0,

                CompletedRedemptionAmount = redemptionStatusRows
                    .Sum(row => row.CompletedAmount),

                PopulationTypes = populationStatistics
            };
        }

        private sealed class UserStatusAggregate
        {
            public bool IsActive { get; init; }
            public int Count { get; init; }
        }

        private sealed class RecipientPopulationAggregate
        {
            public int PopulationTypeId { get; init; }
            public int Count { get; init; }
        }

        private sealed class DonationStatusAggregate
        {
            public string Status { get; init; } = string.Empty;
            public int Count { get; init; }
            public decimal TotalAmount { get; init; }
            public int AvailableCount { get; init; }
            public decimal AvailableBalance { get; init; }
        }

        private sealed class DonationPopulationAggregate
        {
            public int PopulationTypeId { get; init; }
            public int Count { get; init; }
            public decimal TotalAmount { get; init; }
            public decimal AvailableBalance { get; init; }
        }

        private sealed class RedemptionStatusAggregate
        {
            public string Status { get; init; } = string.Empty;
            public int Count { get; init; }
            public decimal CompletedAmount { get; init; }
        }

        private sealed class RedemptionPopulationAggregate
        {
            public int PopulationTypeId { get; init; }
            public int PendingCount { get; init; }
            public int CompletedCount { get; init; }
            public int RejectedCount { get; init; }
            public int CancelledCount { get; init; }
            public decimal CompletedAmount { get; init; }
        }

        private sealed class PopulationTypeAggregate
        {
            public int Id { get; init; }
            public string Name { get; init; } = string.Empty;
            public bool IsActive { get; init; }
        }
    }
}