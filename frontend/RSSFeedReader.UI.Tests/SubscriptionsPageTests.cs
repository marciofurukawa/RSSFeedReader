using System.Net;
using System.Net.Http.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using RSSFeedReader.UI.Pages;
using RSSFeedReader.UI.Services;

namespace RSSFeedReader.UI.Tests;

public sealed class SubscriptionsPageTests : BunitContext
{
    [Fact]
    public async Task Page_LoadsAndDisplaysSubscriptions()
    {
        using var handler = new SubscriptionHandler((request, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new[] { "https://example.test/feed.xml" })
            }));
        RegisterClient(handler);

        var page = Render<Subscriptions>();
        await page.WaitForAssertionAsync(() =>
            Assert.Contains("https://example.test/feed.xml", page.Markup));
    }

    [Fact]
    public async Task Submit_AddsSubscriptionAndShowsItImmediately()
    {
        using var handler = new SubscriptionHandler((request, _) =>
        {
            if (request.Method == HttpMethod.Get)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(Array.Empty<string>())
                });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new { url = "https://example.test/feed.xml" })
            });
        });
        RegisterClient(handler);

        var page = Render<Subscriptions>();
        await page.WaitForAssertionAsync(() => Assert.Empty(page.FindAll("li")));
        page.Find("#subscription-url").Change("https://example.test/feed.xml");
        await page.Find("form").SubmitAsync();

        Assert.Equal("https://example.test/feed.xml", page.Find("li").TextContent);
    }

    [Fact]
    public async Task Submit_BlankUrl_DoesNotSendPostOrAddEntry()
    {
        var postCount = 0;
        using var handler = new SubscriptionHandler((request, _) =>
        {
            if (request.Method == HttpMethod.Post)
            {
                postCount++;
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(Array.Empty<string>())
            });
        });
        RegisterClient(handler);

        var page = Render<Subscriptions>();
        await page.WaitForAssertionAsync(() => Assert.Empty(page.FindAll("li")));
        page.Find("#subscription-url").Change("   ");
        await page.Find("form").SubmitAsync();

        Assert.Equal(0, postCount);
        Assert.Empty(page.FindAll("li"));
    }

    [Fact]
    public async Task Submit_WhenApiFails_ShowsSafeErrorWithoutInternalDetails()
    {
        using var handler = new SubscriptionHandler((request, _) =>
            Task.FromResult(request.Method == HttpMethod.Get
                ? new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(Array.Empty<string>())
                }
                : new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = JsonContent.Create(new { title = "database password secret details" })
                }));
        RegisterClient(handler);

        var page = Render<Subscriptions>();
        await page.WaitForAssertionAsync(() => Assert.Empty(page.FindAll("li")));
        page.Find("#subscription-url").Change("https://example.test/feed.xml");
        await page.Find("form").SubmitAsync();

        Assert.Contains("Unable to add subscription", page.Markup);
        Assert.DoesNotContain("database password", page.Markup);
    }

    [Fact]
    public async Task Submit_DuplicateUrl_ShowsSeparateEntries()
    {
        using var handler = new SubscriptionHandler((request, _) =>
            Task.FromResult(request.Method == HttpMethod.Get
                ? new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(Array.Empty<string>())
                }
                : new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new { url = "https://example.test/feed.xml" })
                }));
        RegisterClient(handler);

        var page = Render<Subscriptions>();
        await page.WaitForAssertionAsync(() => Assert.Empty(page.FindAll("li")));

        for (var index = 0; index < 2; index++)
        {
            page.Find("#subscription-url").Change("https://example.test/feed.xml");
            await page.Find("form").SubmitAsync();
        }

        Assert.Equal(2, page.FindAll("li").Count);
    }

    private void RegisterClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5298/")
        };
        Services.AddSingleton(new SubscriptionApiClient(httpClient));
    }

    private sealed class SubscriptionHandler(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) => sendAsync(request, cancellationToken);
    }
}
