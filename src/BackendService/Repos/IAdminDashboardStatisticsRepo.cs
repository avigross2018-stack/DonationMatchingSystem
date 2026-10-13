namespace BackendService.Repos
{
    public interface IAdminDashboardStatisticsRepo
    {
        Task<AdminDashboardStatisticsData> GetStatisticsAsync(
            DateTime utcNow,
            CancellationToken cancellationToken = default);
    }
}