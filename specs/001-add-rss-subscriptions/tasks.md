# Tasks: Add RSS Subscriptions

**Input**: Design documents from `specs/001-add-rss-subscriptions/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`,
`contracts/subscriptions.openapi.yaml`, `quickstart.md`

**Organization**: Tasks are grouped by user story so the P1 subscription workflow
can be implemented and verified as an independent MVP increment.

## Phase 1: Setup

**Purpose**: Create the application and test projects at the paths used by the plan.

- [X] T001 Create `RSSFeedReader.sln`, the ASP.NET Core API project at `backend/RSSFeedReader.Api/RSSFeedReader.Api.csproj`, the API test project at `backend/RSSFeedReader.Api.Tests/RSSFeedReader.Api.Tests.csproj`, the standalone Blazor WebAssembly project at `frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj`, and the UI test project at `frontend/RSSFeedReader.UI.Tests/RSSFeedReader.UI.Tests.csproj`; target all projects at `net10.0` and add them to the solution.
- [X] T002 Add the project references and test packages to `backend/RSSFeedReader.Api.Tests/RSSFeedReader.Api.Tests.csproj` and `frontend/RSSFeedReader.UI.Tests/RSSFeedReader.UI.Tests.csproj`; use xUnit and `Microsoft.AspNetCore.Mvc.Testing` for API integration tests, and bUnit for Blazor component tests.

## Phase 2: Foundational

**Purpose**: Configure the local API/UI boundary and shared development settings before implementing the subscription journey.

- [X] T003 [P] Configure the API CORS policy in `backend/RSSFeedReader.Api/Program.cs` to allow only the configured frontend development origin; do not treat CORS as authorization and do not expose internal exception details in API error responses.
- [X] T004 [P] Configure consistent, explicit local API and frontend origins in `backend/RSSFeedReader.Api/Properties/launchSettings.json`, `frontend/RSSFeedReader.UI/Properties/launchSettings.json`, and `frontend/RSSFeedReader.UI/wwwroot/appsettings.json`; ensure the frontend API base address matches the API launch URL and the API CORS origin matches the frontend URL.

**Checkpoint**: Projects restore and build, and the local frontend/API origins agree.

## Phase 3: User Story 1 - Add and View Subscriptions (Priority: P1) 🎯 MVP

**Goal**: Let the local user submit a non-empty URL and immediately see it in the current process's subscription list, without fetching or parsing a feed.

**Independent Test**: Start with an empty list; add two non-empty URLs and confirm both appear within 2 seconds, add the same URL again and confirm a second entry appears, submit whitespace and confirm no entry is added, then restart the API and confirm the list is empty. Confirm that no request is made to a submitted feed URL and that API failures are visibly reported.

### Tests for User Story 1

- [X] T005 [P] [US1] Add API integration tests in `backend/RSSFeedReader.Api.Tests/SubscriptionEndpointsTests.cs` for an initially empty GET, successful POST and ordered GET results, duplicate URL entries, rejection of missing/empty/whitespace-only values without changing the list, and user-safe failure responses.
- [X] T006 [P] [US1] Add API CORS integration tests in `backend/RSSFeedReader.Api.Tests/CorsTests.cs` verifying that the configured frontend origin is allowed and an unconfigured origin is not allowed.
- [X] T007 [P] [US1] Add Blazor component tests in `frontend/RSSFeedReader.UI.Tests/SubscriptionsPageTests.cs` verifying initial list display, successful add and immediate update, blank-input behavior, duplicate display, and visible API failure feedback without showing internal exception details.

### Implementation for User Story 1

- [X] T008 [P] [US1] Define the `Subscription` value model in `backend/RSSFeedReader.Api/Subscriptions/Subscription.cs`; its `url` MUST contain at least one non-whitespace character, MUST NOT be syntax-validated or dereferenced, and MUST be rendered as text.
- [X] T009 [US1] Implement the ordered process-local subscription collection in `backend/RSSFeedReader.Api/Subscriptions/SubscriptionStore.cs`; start empty for each API process, append duplicate values as separate entries, and provide no persistence, edit, or delete behavior.
- [X] T010 [US1] Implement `GET /api/subscriptions` and `POST /api/subscriptions` in `backend/RSSFeedReader.Api/Program.cs` according to `specs/001-add-rss-subscriptions/contracts/subscriptions.openapi.yaml`; return list values in insertion order, accept nonblank URLs as inert data, reject blank values, and never fetch or parse a URL.
- [X] T011 [P] [US1] Implement the typed API client in `frontend/RSSFeedReader.UI/Services/SubscriptionApiClient.cs` using the configured local API base address and the GET/POST contract; surface unsuccessful responses as errors for the UI to report and never request a feed URL.
- [X] T012 [US1] Implement the subscription entry form and list in `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`; reject blank submissions, show successful entries immediately after the API accepts them, render URLs as text, and display a clear safe error when an API operation fails.

**Checkpoint**: User Story 1 is independently testable using the API and component tests and the scenarios in `quickstart.md`.

## Phase 4: Polish & Cross-Cutting Concerns

**Purpose**: Verify the complete MVP behavior and ensure developer instructions match the delivered projects.

- [X] T013 Run the API and UI test projects and follow `specs/001-add-rss-subscriptions/quickstart.md`; verify API restart clears subscriptions, successful additions appear within 2 seconds, errors remain user-safe, and no request is sent to any submitted feed URL.
- [X] T014 Update `specs/001-add-rss-subscriptions/quickstart.md` only where needed so its project commands, configured origins, contract examples, and expected validation results match the generated projects.

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; complete before shared configuration or story work.
- **Foundational (Phase 2)**: Depends on Setup; complete before story integration tests and implementation.
- **User Story 1 (Phase 3)**: Depends on Foundational; contains the only user story and MVP scope.
- **Polish (Phase 4)**: Depends on the User Story 1 implementation.

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Phase 2; no dependencies on other user stories.

### Within User Story 1

- Tests T005, T006, and T007 can be authored in parallel after the test projects exist.
- Subscription value model T008 can be implemented independently; store T009 depends on T008.
- Endpoint T010 depends on the store T009.
- API client T011 depends on the shared API contract and can be implemented in parallel with T008-T010.
- UI page T012 depends on the API client T011.
- Run end-to-end verification T013 only after API, client, and page implementation and tests are complete.

## Parallel Execution Examples

### Foundational setup

```text
T003: Configure API CORS and safe error responses in Program.cs
T004: Configure local launch origins and frontend API base address
```

### User Story 1 tests

```text
T005: API subscription endpoint integration tests
T006: API CORS integration tests
T007: Blazor subscription page component tests
```

### User Story 1 implementation

```text
T008: Subscription value model
T011: Frontend subscription API client
```

After T008, T009 -> T010 implement the API path. After T011, T012 implements
the UI path. Both paths converge at T013.

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Setup and Foundational phases.
2. Implement the single P1 story using the API and UI paths.
3. Run API and component tests, then follow the quickstart end-to-end checklist.
4. Stop at subscription add/list; do not add feed fetching, parsing, durable storage,
   deletion, or other deferred features.

### Task Format Validation

All implementation tasks use the required unchecked checklist form with sequential
IDs. User Story 1 tasks carry the `[US1]` label; parallelizable tasks carry `[P]`;
each task names its target file path.
