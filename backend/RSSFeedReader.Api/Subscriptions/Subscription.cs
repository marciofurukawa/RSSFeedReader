namespace RSSFeedReader.Api.Subscriptions;

public sealed record Subscription
{
    public Subscription(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("A subscription URL cannot be blank.", nameof(url));
        }

        Url = url;
    }

    public string Url { get; }
}
