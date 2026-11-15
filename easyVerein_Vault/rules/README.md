# Claude Code rules

This directory is a consolidated `.claude/rules` rule set derived from:

-   the reviewed MCP-easyVerein `code-review-rules.md`;
-   the reviewed `clean-code-dotnet` `code-review-rules.md`.

The generic Clean Code guidance is intentionally separated from repository-specific MCP/easyVerein behavior.

## Files

-   `00-review-policy.md` --- severity and review behavior.
-   `01-naming-and-structure.md` --- C# naming, visibility and file structure.
-   `02-clean-code-design.md` --- method/class design, SOLID-oriented rules and DRY.
-   `03-csharp-reliability.md` --- async, nullable and exception reliability.
-   `04-easyverein-architecture.md` --- repository architecture and endpoint structure.
-   `05-easyverein-api-http-json.md` --- API, HTTP, PATCH, JSON and pagination.
-   `06-mcp-tools-and-security.md` --- MCP boundary, tool shape, stdout and secrets.
-   `07-documentation-and-comments.md` --- XML docs, descriptions and comments.
-   `08-testing.md` --- test structure, mocks, HTTP fakes and regression tests.
-   `09-configuration-and-formatting.md` --- configuration pipeline and formatting.

## Precedence

When a generic rule conflicts with an easyVerein-specific rule, the easyVerein-specific rule wins.

`MUST` means a review finding. `SHOULD` means a deviation needs a reason. `CONSIDER` is a review/design question rather than an automatic defect.

## Deliberately not promoted to rules

Recommendations that the source documents explicitly classify as unsupported/low-confidence are not silently turned into mandatory rules. Examples include a universal URL-escaping policy, universal `IDisposable` guidance derived from these repositories, a fixed `DateTime` vs. `DateTimeOffset` policy, a generic logging framework policy, and a coverage threshold. Add these separately only after the team makes an explicit decision.
