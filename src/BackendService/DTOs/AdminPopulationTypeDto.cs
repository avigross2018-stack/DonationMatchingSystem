namespace BackendService.DTOs
{
    public sealed class AdminPopulationTypeDto
    {
        public int Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public decimal BaseEligibilityAmount { get; init; }

        public bool IsActive { get; init; }
    }
}