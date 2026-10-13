namespace BackendService.DTOs
{
    public class AdminDonorDto
    {
        public int Id { get; init; }

        public int UserId { get; init; }

        public string Name { get; init; } = string.Empty;

        public string Phone { get; init; } = string.Empty;

        public bool IsActive { get; init; }

        public DateTime CreatedAt { get; init; }
    }

    public sealed class AdminDonorDetailsDto : AdminDonorDto
    {
        public IReadOnlyList<AdminDonorDonationDto> Donations { get; init; }
            = Array.Empty<AdminDonorDonationDto>();
    }

    public sealed class AdminDonorDonationDto
    {
        public int Id { get; init; }

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

    public sealed class AdminDonorStatusDto
    {
        public int DonorId { get; init; }

        public int UserId { get; init; }

        public bool IsActive { get; init; }

        public int CancelledDonationsCount { get; init; }

        public int CancelledRedemptionsCount { get; init; }
    }
}