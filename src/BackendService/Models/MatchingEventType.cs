namespace BackendService.Models
{
    public enum MatchingEventType
    {
        RecipientRegistered,
        DonationCreated,
        FullScan
    }
    public record MatchingEvent(
        MatchingEventType Type,
        int? EntityId = null);
}