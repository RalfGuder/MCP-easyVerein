---
paths:
  - "\**\*"
---

# Configuration and Formatting

## SHOULD

-   Repository formatting and naming policy belongs in `.editorconfig` where practical; automated style rules should not be repeatedly debated in review.
-   C# control-flow and exception blocks use braces even for a single statement.
-   Follow the repository's Allman brace style: opening braces on their own line.
-   Avoid `#region` and decorative separator comments as substitutes for coherent methods/classes.

For MCP-easyVerein configuration, add new settings through the established configuration pipeline:

1.  `EASYVEREIN_*` environment variable.
2.  CLI switch mapping in `Program.cs`.
3.  Default/resolution in `EasyVereinConfiguration` via `IConfiguration`.

Configuration precedence is:

``` text
CLI > environment > default
```

Do not introduce direct `Environment.GetEnvironmentVariable` reads in normal application code when the configuration pipeline can supply the value.
