namespace BackendService.DTOs
{
    public sealed class AdminProfileDto
    {
        public int AdminId { get; init; }

        public int UserId { get; init; }

        public string Name { get; init; } = string.Empty;

        public string Phone { get; init; } = string.Empty;

        public bool IsActive { get; init; }

        public DateTime CreatedAt { get; init; }
    }
}