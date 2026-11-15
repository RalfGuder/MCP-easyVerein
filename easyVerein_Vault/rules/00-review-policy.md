# Code Review Policy

These rules are derived from two reviewed rule sets: the MCP-easyVerein repository conventions and the `clean-code-dotnet` guidance.

## Severity

-   **MUST**: A violation can break protocol/behavior, risk data loss or secrets, violate an architectural boundary, lose diagnostic information, or contradict an explicit project requirement.
-   **SHOULD**: Established convention. Deviations require a concrete justification.
-   **CONSIDER**: Useful design/review guidance where evidence is weaker or the existing codebase is inconsistent.

## Review behavior

1.  Review changed code against all applicable `MUST` rules first.
2.  Report `MUST` violations as findings.
3.  Report `SHOULD` violations unless the code or PR contains a convincing reason for the deviation.
4.  Treat `CONSIDER` items as design questions, not automatic defects.
5.  Prefer the repository-specific rule over a generic Clean Code rule when both address the same situation.
6.  Do not copy a legacy pattern merely because it already exists. Prefer the conventions demonstrated by newer code.
7.  Do not invent a convention where these rules or the surrounding code provide no evidence.
8.  For each finding, state the violated rule, the concrete code location, the risk, and the smallest useful correction.
