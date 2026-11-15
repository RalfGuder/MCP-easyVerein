---
paths:
  - "tests/**/*.cs"
  - "src/**/*.cs"
---

# Testing

## MUST

-   New or materially changed production behavior has tests. New production classes use the corresponding mirrored `<Subject>Tests` location used by the repository.
-   New MCP tool classes get their own `{Resource}ToolsTests`.
-   Unit tests never perform real network requests.
-   Tool tests mock `IEasyVereinApiClient`; client HTTP tests use the repository's fake handlers.
-   For validation that must prevent an API call, assert that prevention explicitly with `Times.Never`, `MockBehavior.Strict`, or the equivalent established mechanism.

## SHOULD

-   Test names follow `Method_Scenario_Expectation` where applicable and clearly identify the behavior under test.
-   Each test covers one behavior/scenario.
-   Structure tests as Arrange / Act / Assert when that improves readability.
-   Multiple assertions are acceptable when they jointly verify one behavior or payload.
-   Tests do not share mutable state and do not depend on execution order.
-   Prefer local setup per test; share construction logic through stateless factory helpers such as `CreateClient` or `CreateConfig`.
-   Every bug fix includes a regression test that fails without the fix and names/describes the failed behavior.
-   Client tests verify the outgoing HTTP method, path/query and request body, not merely the returned value.

## CONSIDER

-   If a real API response caused a deserialization defect, anonymize and version that response as a fixture under `Domain.Tests/Fixtures`, copy it to test output, and add a regression test that deserializes it.
