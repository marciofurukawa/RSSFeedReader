# Research: Add RSS Subscriptions

## Decisions

### Application target and framework

- **Decision**: Target .NET 10 (`net10.0`) for the new backend and Blazor
  WebAssembly frontend.
- **Rationale**: The stakeholder documents choose ASP.NET Core and Blazor
  WebAssembly, and the planning environment has .NET SDK 10.0.403 installed.
  Using one supported SDK target for both projects avoids version skew.
- **Alternatives considered**: An earlier .NET target was not indicated by the
  stakeholder documents or an existing project. No existing application project
  or target framework is present in the repository to preserve.

### Application boundaries

- **Decision**: The API owns process-local subscription state and collection
  operations; the browser frontend owns entry and display.
- **Rationale**: This follows the documented responsibilities in
  `StakeholderDocuments/TechStack.md` and keeps state behavior independent from
  the UI.
- **Alternatives considered**: Client-only storage would not exercise the
  stakeholder-selected backend API. A database or shared persistence layer would
  exceed MVP scope.

### Subscription storage and identity

- **Decision**: Keep an ordered in-memory collection of URL strings. Allow duplicate
  values as independent entries; do not add IDs, remove, edit, or persist behavior.
- **Rationale**: The feature specification explicitly allows duplicates and requires
  data to disappear when the application stops. No identity is needed for the
  supported add/list operations.
- **Alternatives considered**: Deduplication, generated IDs, and persistent storage
  add behavior that is explicitly out of scope.

### API contract

- **Decision**: Expose `GET /api/subscriptions` and
  `POST /api/subscriptions`. Return JSON URL values from GET and the accepted URL
  from POST; reject blank input with a client error. See
  [`contracts/subscriptions.openapi.yaml`](./contracts/subscriptions.openapi.yaml).
- **Rationale**: A collection resource supports the only in-scope operations while
  keeping the contract small. A successful POST returns `200 OK` because entries
  do not have a stable individual identifier or location in this MVP.
- **Alternatives considered**: `201 Created` with a `Location` header implies a
  stable address for each entry, but duplicates are allowed and no item-addressing
  operation exists. A more elaborate API is not justified by this feature.

### URL handling and network security

- **Decision**: Treat each submitted URL as inert user data. Do not validate its
  syntax, resolve DNS, fetch it, follow redirects, or parse content. Render it as
  text. Configure CORS for the actual local frontend origin and do not rely on CORS
  as authorization.
- **Rationale**: The MVP explicitly excludes feed operations and URL validation.
  Keeping URLs inert prevents the API from becoming an unintended server-side
  request proxy. Text rendering avoids treating user input as markup.
- **Alternatives considered**: Validating or fetching a feed on add conflicts with
  the MVP boundary and would add network and security behavior.

### Testing approach

- **Decision**: Use API integration tests with `Microsoft.AspNetCore.Mvc.Testing`
  and xUnit-compatible tooling, plus Blazor component behavior tests with bUnit.
  Include a local browser smoke test in the quickstart guide.
- **Rationale**: API tests verify status codes, collection state, duplicates,
  CORS configuration, and absence of feed retrieval. Component tests cover blank
  input, successful list updates, and visible failures. A browser check validates
  local wiring and route behavior.
- **Alternatives considered**: Manual-only UI verification does not provide
  repeatable coverage for the user-visible acceptance behavior. End-to-end browser
  automation is possible but adds more setup than this small MVP needs.

## Repository Findings

- The repository had no solution, application project, test project, or source-code
  directory when this plan was prepared.
- `StakeholderDocuments/ProjectGoals.md` limits the MVP to adding and displaying
  subscriptions, assumes valid URLs, and specifies in-memory state.
- `StakeholderDocuments/TechStack.md` selects an ASP.NET Core Web API backend and
  Blazor WebAssembly frontend and describes a cleanup of demo pages and root-route
  conflicts when scaffolding the frontend.
- The project constitution requires inert URL handling, configured CORS, separation
  between frontend and API, testable behavior, and clear user feedback.
