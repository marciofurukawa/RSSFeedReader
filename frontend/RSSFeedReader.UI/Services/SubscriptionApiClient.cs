using System.Net.Http.Json;
using System.Text.Json;

namespace RSSFeedReader.UI.Services;

public sealed class SubscriptionApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<string>> GetSubscriptionsAsync(
        CancellationToken cancellationToken = default)
    {
        var subscriptions = await httpClient.GetFromJsonAsync<string[]>(
            "api/subscriptions",
            cancellationToken);

        return subscriptions
            ?? throw new JsonException("The subscription API returned an empty response.");
    }

    public async Task<string> AddSubscriptionAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/subscriptions",
            new AddSubscriptionRequest(url),
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var subscription = await response.Content.ReadFromJsonAsync<SubscriptionResponse>(
            cancellationToken);

        return subscription?.Url
            ?? throw new JsonException("The subscription API returned an empty response.");
    }

    private sealed record AddSubscriptionRequest(string Url);

    private sealed record SubscriptionResponse(string Url);
}
