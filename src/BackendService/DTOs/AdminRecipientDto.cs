namespace BackendService.DTOs
{
    public class AdminRecipientDto
    {
        public int Id { get; init; }

        public int UserId { get; init; }

        public string Name { get; init; } = string.Empty;

        public string Phone { get; init; } = string.Empty;

        public bool IsActive { get; init; }

        public int PopulationTypeId { get; init; }

        public string PopulationTypeName { get; init; } = string.Empty;

        public decimal IncomeAmount { get; init; }

        public int AmountOfChildren { get; init; }

        public decimal EligibilityAmount { get; init; }

        public decimal RemainingEligibility { get; init; }
    }

    public sealed class AdminRecipientDetailsDto : AdminRecipientDto
    {
        public string? RequestReason { get; init; }

        public IReadOnlyList<AdminRecipientRedemptionDto> Redemptions
        { get; init; } = Array.Empty<AdminRecipientRedemptionDto>();
    }

    public sealed class AdminRecipientRedemptionDto
    {
        public int Id { get; init; }

        public int DonationId { get; init; }

        public string Status { get; init; } = string.Empty;

        public decimal Amount { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime? RedeemedAt { get; init; }

        public string DonorName { get; init; } = string.Empty;

        public string? DonationDescription { get; init; }

        public decimal DonationTotalAmount { get; init; }

        public string DonationStatus { get; init; } = string.Empty;
    }

    public sealed class AdminRecipientStatusDto
    {
        public int RecipientId { get; init; }

        public int UserId { get; init; }

        public bool IsActive { get; init; }

        public int CancelledRedemptionsCount { get; init; }
    }
}