# Implementation Plan: Add RSS Subscriptions

**Branch**: `001-add-rss-subscriptions` | **Date**: 2026-10-31 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `specs/001-add-rss-subscriptions/spec.md`

## Summary

Deliver the single-user MVP subscription workflow: accept a non-empty URL, keep it
in memory for the current API process, and show the current list in the browser.
Use the stakeholder-selected ASP.NET Core Web API and Blazor WebAssembly structure.
The browser communicates only with the local subscription API; neither adding nor
listing subscriptions contacts a feed URL. The API contract is defined in
[`contracts/subscriptions.openapi.yaml`](./contracts/subscriptions.openapi.yaml).

## Technical Context

**Language/Version**: C# on .NET 10 (`net10.0`); .NET SDK 10.0.403 is installed in the planning environment.

**Primary Dependencies**: ASP.NET Core Web API; Blazor WebAssembly; xUnit-compatible test projects; `Microsoft.AspNetCore.Mvc.Testing` for API integration tests; bUnit for component behavior tests.

**Storage**: Process-local in-memory subscription collection; no persistence.

**Testing**: `dotnet test` for API and UI test projects; API integration tests and Blazor component tests for acceptance behavior; browser smoke test per [`quickstart.md`](./quickstart.md).

**Target Platform**: Local development on Windows, macOS, and Linux; browser frontend and local .NET backend.

**Project Type**: Web application with separate backend API and frontend.

**Performance Goals**: A successful add is visible in the subscription list within 2 seconds, as required by SC-001.

**Constraints**: Single local user; subscription URLs are not syntactically validated or dereferenced; no feed retrieval, parsing, durable storage, deletion, or background polling; reject empty or whitespace-only submissions; preserve duplicates as separate entries.

**Scale/Scope**: One in-memory subscription list for the current API process; no multi-user or throughput requirement is defined for the MVP.

## Constitution Check

**Pre-design gate**: PASS

- **I. Security at Every Boundary**: Treat URLs as inert input; do not make outbound
  requests. Restrict CORS to configured frontend origins, do not treat CORS as
  authorization, and return no internal exception details.
- **II. MVP Scope Fidelity**: Deliver only add and list behavior with process-local
  storage. No feed parsing, retrieval, URL syntax validation, or deferred features.
- **III. Maintainable Separation of Responsibilities**: Keep subscription state and
  operations in the API; keep input and presentation in the Blazor client.
- **IV. Verifiable Code Quality**: Build and test both projects; cover API behavior
  and UI acceptance behavior with automated tests.
- **V. Clear, Accessible Interaction**: Present a usable URL input and list, update
  after a successful add, and communicate failures without claiming a feed was read.

No constitution exceptions or scope violations are required.

**Post-design gate**: PASS. The API contract retains inert URL values and exposes
only list/add operations; the model uses an in-memory ordered collection; the UI
remains separate from API state; and API/component tests cover the defined behavior.
No database, feed network access, or deferred feature was introduced.

## Project Structure

### Documentation (this feature)

```text
specs/001-add-rss-subscriptions/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── contracts/
    └── subscriptions.openapi.yaml
```

### Source Code (repository root)

```text
backend/
├── RSSFeedReader.Api/
│   ├── Program.cs
│   ├── Subscriptions/
│   │   ├── Subscription.cs
│   │   └── SubscriptionStore.cs
│   └── Properties/
│       └── launchSettings.json
└── RSSFeedReader.Api.Tests/
    ├── SubscriptionEndpointsTests.cs
    └── CorsTests.cs

frontend/
├── RSSFeedReader.UI/
│   ├── Pages/
│   │   └── Subscriptions.razor
│   ├── Services/
│   │   └── SubscriptionApiClient.cs
│   └── wwwroot/
└── RSSFeedReader.UI.Tests/
    └── SubscriptionsPageTests.cs
```

**Structure Decision**: Use the stakeholder-documented backend/frontend separation.
The API owns the in-memory list and exposes only subscription collection operations.
The Blazor UI loads and displays that list and submits new entries through an API
client. Keep projects and tests minimal; do not introduce a database, feed service,
or additional abstraction until an approved feature requires it. This repository
currently has no application source projects, so implementation begins by creating
this structure and using the single-root-route cleanup checklist in
`StakeholderDocuments/TechStack.md` when scaffolding the Blazor project.

## Complexity Tracking

No constitution violations.
