using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RSSFeedReader.Api.Tests;

public sealed class SubscriptionEndpointsTests
{
    [Fact]
    public async Task Get_WhenStoreIsEmpty_ReturnsEmptyList()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var subscriptions = await client.GetFromJsonAsync<string[]>("/api/subscriptions");

        Assert.NotNull(subscriptions);
        Assert.Empty(subscriptions);
    }

    [Fact]
    public async Task Post_ThenGet_AppendsSubscriptionsInOrderAndAllowsDuplicates()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        const string url = "https://example.test/feed.xml";

        var firstResponse = await client.PostAsJsonAsync("/api/subscriptions", new { url });
        var secondResponse = await client.PostAsJsonAsync("/api/subscriptions", new { url });
        var subscriptions = await client.GetFromJsonAsync<string[]>("/api/subscriptions");

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        Assert.NotNull(subscriptions);
        Assert.Equal(new[] { url, url }, subscriptions);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Post_WithMissingOrBlankUrl_ReturnsBadRequestWithoutAddingEntry(string? url)
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/subscriptions", new { url });
        var body = await response.Content.ReadAsStringAsync();
        var subscriptions = await client.GetFromJsonAsync<string[]>("/api/subscriptions");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.DoesNotContain("stack", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("exception", body, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(subscriptions!);
    }

    [Fact]
    public async Task Post_WithMissingUrlProperty_ReturnsBadRequest()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/subscriptions", new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
