
namespace BackendService.DTOs;

public sealed class AdminRedemptionsPageDto
{
    public IReadOnlyList<AdminRedemptionDto> Items { get; init; }
        = Array.Empty<AdminRedemptionDto>();

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages =>
        (int)Math.Ceiling(TotalCount / (double)PageSize);
}

public sealed class AdminRedemptionDto
{
    public int Id { get; init; }

    public int DonationId { get; init; }

    public int RecipientId { get; init; }

    public string Status { get; init; } = string.Empty;

    // For pending requests, this is the proposed amount.
    public decimal Amount { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? RedeemedAt { get; init; }

    public int? HandledByAdminId { get; init; }

    public AdminRedemptionRecipientDto Recipient { get; init; }
        = new();

    public AdminRedemptionDonationDto Donation { get; init; }
        = new();
}

public sealed class AdminRedemptionRecipientDto
{
    public int Id { get; init; }

    public int UserId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Phone { get; init; } = string.Empty;

    public int PopulationTypeId { get; init; }

    public string PopulationTypeName { get; init; } = string.Empty;

    public decimal IncomeAmount { get; init; }

    public int AmountOfChildren { get; init; }

    public decimal EligibilityAmount { get; init; }

    public decimal RemainingEligibility { get; init; }
}

public sealed class AdminRedemptionDonationDto
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

public sealed class AdminRedemptionDecisionDto
{
    public int RedemptionId { get; init; }

    public string Status { get; init; } = string.Empty;

    public decimal ApprovedAmount { get; init; }

    public int HandledByAdminId { get; init; }

    public DateTime RedeemedAt { get; init; }

    public decimal DonationRemainingAmount { get; init; }

    public decimal RecipientRemainingEligibility { get; init; }
}


public sealed class AdminRedemptionRejectionDto
{
    public int RedemptionId { get; init; }

    public string Status { get; init; } = string.Empty;

    public int HandledByAdminId { get; init; }
}
