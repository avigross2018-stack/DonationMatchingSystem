namespace BackendService.DTOs
{
    public sealed class AdminDonationsPageDto
    {
        public IReadOnlyList<AdminDonationDto> Items { get; init; }
            = Array.Empty<AdminDonationDto>();

        public int Page { get; init; }

        public int PageSize { get; init; }

        public int TotalCount { get; init; }

        public int TotalPages =>
            (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public class AdminDonationDto
    {
        public int Id { get; init; }

        public int DonorId { get; init; }

        public string DonorName { get; init; } = string.Empty;

        public string DonorPhone { get; init; } = string.Empty;

        public int PopulationTypeId { get; init; }

        public string PopulationTypeName { get; init; } = string.Empty;

        public decimal TotalAmount { get; init; }

        public decimal RemainingAmount { get; init; }

        public decimal MaxAmountPerRecipient { get; init; }

        public string? Description { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime? ExpirationDate { get; init; }

        public string Status { get; init; } = string.Empty;
    }

    public sealed class AdminDonationDetailsDto : AdminDonationDto
    {
        public IReadOnlyList<AdminDonationRedemptionDto> Redemptions
        { get; init; } = Array.Empty<AdminDonationRedemptionDto>();
    }

    public sealed class AdminDonationRedemptionDto
    {
        public int Id { get; init; }

        public int RecipientId { get; init; }

        public string RecipientName { get; init; } = string.Empty;

        public string Status { get; init; } = string.Empty;

        public decimal Amount { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime? RedeemedAt { get; init; }

        public int? HandledByAdminId { get; init; }
    }
}