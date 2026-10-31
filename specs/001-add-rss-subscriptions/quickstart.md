# Quickstart: Add RSS Subscriptions

This guide validates the subscription add/list flow after the projects are created.
The API stores URLs only for its current process; it never fetches feed URLs.

## Prerequisites

- .NET 10 SDK
- A current browser
- The API and frontend projects created at the paths in [`plan.md`](./plan.md)

## Build and test

From the repository root, run:

```powershell
dotnet test .\backend\RSSFeedReader.Api.Tests\RSSFeedReader.Api.Tests.csproj
dotnet test .\frontend\RSSFeedReader.UI.Tests\RSSFeedReader.UI.Tests.csproj
```

Expected: both commands complete successfully. The API tests cover empty startup
state, successful adds, duplicate values, blank-input rejection, and API error
responses. The component tests cover entry submission, immediate list updates,
blank-input behavior, and visible API failures.

## Run and verify end to end

1. Start the API:

   ```powershell
   dotnet run --project .\backend\RSSFeedReader.Api\RSSFeedReader.Api.csproj
   ```

2. In a second terminal, start the frontend:

   ```powershell
   dotnet run --project .\frontend\RSSFeedReader.UI\RSSFeedReader.UI.csproj
   ```

3. Open the frontend URL printed by the run command. Confirm the subscription list
   is initially empty.
4. Enter `https://example.test/feed.xml` and submit. Confirm the exact URL appears
   in the list within 2 seconds.
5. Submit the same URL again. Confirm it appears as a second entry.
6. Submit a whitespace-only value. Confirm the list does not gain an entry.
7. Stop and restart the API, reload the frontend, and confirm the list is empty.
8. Use browser network tools or a test HTTP handler to confirm that adding the URL
   causes no request to `example.test`; requests to the subscription API should
   target only its configured local origin.

## Contract checks

Use the API origin and port configured by the development launch settings:

- `GET /api/subscriptions` returns `200 OK` with `[]` for an empty process-local list.
- `POST /api/subscriptions` with `{"url":"https://example.test/feed.xml"}` returns
  `200 OK` with the accepted URL.
- `POST /api/subscriptions` with a blank value returns `400 Bad Request` and does
  not change the list.
- A subsequent GET returns entries in insertion order, including duplicates.

See [`contracts/subscriptions.openapi.yaml`](./contracts/subscriptions.openapi.yaml)
for the full request and response schema.
