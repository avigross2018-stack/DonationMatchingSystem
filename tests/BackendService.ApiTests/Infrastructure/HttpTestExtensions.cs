using System.Net.Http.Headers;

namespace BackendService.ApiTests.Infrastructure;

internal static class HttpTestExtensions
{
    internal static HttpRequestMessage WithBearer(
        this HttpRequestMessage request, string token)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    internal static async Task<JsonElement> ReadJsonAsync(this HttpResponseMessage response) =>
        await response.Content.ReadFromJsonAsync<JsonElement>();
}
