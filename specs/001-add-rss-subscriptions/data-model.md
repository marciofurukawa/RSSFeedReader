# Data Model: Add RSS Subscriptions

## Subscription

Represents one URL entered by the local user during the current API process.

| Field | Type | Rules |
|---|---|---|
| `url` | String | MUST contain at least one non-whitespace character. URL syntax is not validated. The value is not fetched, parsed, or interpreted as markup. |

### Collection behavior

- The API maintains an ordered collection of subscription URL values in memory.
- Identical URL values are allowed and represent separate add operations.
- The collection starts empty when the API process starts.
- All entries are lost when the API process stops.
- The MVP has no edit, delete, persistence, or item-identity operations.

### State transitions

```text
Not in current collection -- successful add(nonblank URL) --> Present in collection
Present in current collection -- add(same URL) --> Another matching entry is appended
Any collection state -- API process stops/restarts --> Empty collection
```

Blank or whitespace-only values do not create an entry.
