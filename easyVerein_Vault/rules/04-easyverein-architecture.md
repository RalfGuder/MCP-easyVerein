---
paths:
  - "src/**/*.cs"
  - "src/**/*.csproj"
---

# MCP-easyVerein Architecture

These rules are repository-specific and override generic guidance where they conflict.

## MUST

-   Keep the dependency direction: Domain -\> nothing; Application -\> Domain; Infrastructure -\> Application + Domain; Server may reference all three.
-   Domain remains free of external package dependencies unless an explicit architectural decision changes this rule.
-   HTTP access is performed only through the typed `IEasyVereinApiClient` / `EasyVereinApiClient` registration. Never create `new HttpClient()`.
-   Tools depend on `IEasyVereinApiClient`, not the concrete client.
-   A `[McpServerToolType]` must be registered with `.WithTools<T>()`.

## SHOULD

A new easyVerein API resource follows the complete endpoint pattern:

1.  Entity plus `{Resource}Fields` in Domain.
2.  Methods in `IEasyVereinApiClient`.
3.  Implementation in `EasyVereinApiClient` plus `{Resource}Query`.
4.  `{Resource}Tools`.
5.  `.WithTools<{Resource}Tools>()` registration.

Tool classes use the established shape:

``` csharp
[McpServerToolType]
public sealed class ResourceTools(IEasyVereinApiClient client)
```

Additional constructor dependencies require a concrete functional
reason.

Client interface operations follow the established signatures:

-   `List...Async` -\> `Task<IReadOnlyList<T>>`, optional filters,
    `CancellationToken ct = default`.
-   `Get...Async` -\> `Task<T?>`, `long id`,
    `CancellationToken ct = default`.
-   `Create...Async` -\> `Task<T>`, entity,
    `CancellationToken ct = default`.
-   `Update...Async` -\> `Task<T>`, `long id`, `object patchData`,
    `CancellationToken ct = default`.
-   `Delete...Async` -\> `Task`, `long id`,
    `CancellationToken ct = default`.

Document deliberate deviations.
