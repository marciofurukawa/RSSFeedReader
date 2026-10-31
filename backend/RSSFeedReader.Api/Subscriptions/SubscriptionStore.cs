namespace RSSFeedReader.Api.Subscriptions;

public sealed class SubscriptionStore
{
    private readonly List<Subscription> _subscriptions = [];
    private readonly object _sync = new();

    public IReadOnlyList<Subscription> GetAll()
    {
        lock (_sync)
        {
            return _subscriptions.ToArray();
        }
    }

    public void Add(Subscription subscription)
    {
        ArgumentNullException.ThrowIfNull(subscription);

        lock (_sync)
        {
            _subscriptions.Add(subscription);
        }
    }
}
