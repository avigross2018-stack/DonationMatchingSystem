namespace BackendService.Models
{
    public enum MatchingEventType
    {
        RecipientRegistered,
        DonationCreated,
        DonationUpdated,
        FullScan
    }
    public record MatchingEvent(
        MatchingEventType Type,
        int? EntityId = null);
}