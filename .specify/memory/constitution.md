<!--
Sync Impact Report
Version change: 1.0.0 -> 1.1.0 (material expansion and project-scope alignment)
Modified principles:
- I. Feed Compatibility -> I. Security at Every Boundary
- II. Clear Reading Experience -> II. MVP Scope Fidelity
- III. User Data and Privacy -> III. Maintainable Separation of Responsibilities
- IV. Testable Behavior -> IV. Verifiable Code Quality
- V. Simplicity and Maintainability -> V. Clear, Accessible Interaction
Added sections: None
Removed sections: None
Follow-up TODOs: None
-->

# RSS Feed Reader Constitution

## Core Principles

### I. Security at Every Boundary
The API MUST treat subscription URLs as user-provided data, not as instructions to
make network requests. In the MVP, the application MUST NOT fetch or parse feeds.
The API's CORS policy MUST allow only configured frontend origins; CORS MUST NOT be
treated as API authorization. The API MUST NOT expose secrets or internal error
details in responses. If a later approved feature fetches feeds, it MUST validate
allowed URL schemes and destinations, block loopback, private, and link-local
addresses, recheck destinations after redirects, and enforce request time and
response size limits. These controls protect the backend from unintended outbound
access while preserving the deliberately limited MVP.

### II. MVP Scope Fidelity
The MVP MUST provide only subscription entry and subscription-list display, using
in-memory storage. It MUST NOT add feed fetching, feed parsing, persistence, or
other deferred features without an approved feature specification. URL validation
is not an MVP requirement; submitted values MUST NOT be dereferenced or used to
initiate network access. This keeps implementation and acceptance criteria aligned
with the project's proof-of-concept goals.

### III. Maintainable Separation of Responsibilities
The ASP.NET Core Web API MUST own subscription operations and in-memory subscription
state; the Blazor WebAssembly frontend MUST provide the input and list interface
through the API. Components MUST keep responsibilities explicit, and dependencies
MUST be added only when required by an approved feature. This preserves the chosen
architecture without introducing complexity ahead of need.

### IV. Verifiable Code Quality
Changes MUST preserve buildable, type-safe code and follow the conventions of the
language and framework being changed. Changes to subscription behavior MUST include
automated tests for the relevant API and user-visible acceptance behavior, including
adding an entry and displaying the resulting list. Tests MUST pass before the change
is considered complete; known failures MUST be reported rather than hidden. This
makes the MVP's small contract reliable and reviewable.

### V. Clear, Accessible Interaction
The frontend MUST make the subscription input, add action, and current subscription
list understandable and usable with the project's supported browser interaction
patterns. Adding a subscription MUST update the displayed list immediately after
the API accepts it. The interface MUST distinguish operational errors from a
successful update and MUST NOT claim that a feed has been checked or refreshed.
This keeps user feedback accurate without implying deferred feed-reader behavior.

## Product and Technical Constraints

The MVP targets a single local user and supports development on Windows, macOS, and
Linux. It uses an ASP.NET Core Web API backend and a Blazor WebAssembly frontend.
Subscriptions are held in memory and are lost when the application stops. Feed
retrieval, parsing, and durable storage are outside the MVP. Any change to these
constraints MUST be specified and approved before implementation.

## Development Workflow

Changes that introduce or materially alter behavior MUST be described in a feature
specification before implementation. Plans and tasks MUST identify the affected
principles, scope boundary, and verification required. Reviewers MUST confirm that
the implementation matches the approved feature scope, relevant automated tests
pass, and security-sensitive behavior is covered before considering the work complete.
Deviations MUST be recorded with their rationale and approved as part of the change.

## Governance

This constitution governs project decisions and MUST be reviewed during specification,
planning, implementation, and change review. Amendments MUST document their rationale,
affected principles, and any migration or compatibility impact. A change that conflicts
with a principle MUST either be revised to comply or receive an explicit, documented
exception as part of the amendment.

Constitution versions follow semantic versioning: MAJOR for backward-incompatible
governance changes or principle removals/redefinitions, MINOR for new principles or
material expansions, and PATCH for clarifications and non-semantic wording changes.
The last-amended date MUST be updated for every amendment. Reviewers MUST verify
compliance with the principles and confirm that required evidence, including tests
and documented exceptions, is present.

**Version**: 1.1.0 | **Ratified**: 2026-10-28 | **Last Amended**: 2026-10-31
