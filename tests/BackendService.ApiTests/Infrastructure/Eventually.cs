namespace BackendService.ApiTests.Infrastructure;

internal static class Eventually
{
    internal static async Task<T> UntilAsync<T>(
        Func<Task<T>> query,
        Func<T, bool> predicate,
        TimeSpan? timeout = null)
    {
        using var cts = new CancellationTokenSource(timeout ?? TimeSpan.FromSeconds(5));
        while (true)
        {
            var value = await query();
            if (predicate(value)) return value;

            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50), cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new Xunit.Sdk.XunitException("Eventual assertion timed out.");
            }
        }
    }
}
