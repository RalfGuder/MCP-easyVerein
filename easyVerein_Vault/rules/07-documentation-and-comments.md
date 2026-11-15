---
paths:
  - "src/**/*.cs"
---

# Documentation and Comments

## MUST

-   In this repository, every production type and member has an English XML `/// <summary>`, including private fields, constants, constructors and helper methods.
-   MCP tool methods and their parameters, except `CancellationToken`, have English `[Description]` attributes.

## SHOULD

-   Descriptions document required inputs, formats and value ranges when those constraints matter to callers.
-   Remove commented-out code and obsolete/dead code. Version control is the history.
-   Do not keep journal comments containing author/date/change history.
-   Prefer names and extracted methods over comments that merely narrate what the code does.

## CONSIDER

-   Comments should primarily explain *why*: non-obvious decisions, constraints, requirements or externally observed behavior.
-   When code intentionally reflects an unusual requirement/API behavior, reference an `FR-...` / `NFR-...` identifier or document the observed API behavior and, where useful, the verification date.
