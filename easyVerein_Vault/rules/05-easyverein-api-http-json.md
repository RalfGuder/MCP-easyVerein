---
paths:
  - "src/MCP.EasyVerein.Domain/**/*.cs"
  - "src/MCP.EasyVerein.Infrastructure/**/*.cs"
  - "src/MCP.EasyVerein.Server/Tools/**/*.cs"
---

# easyVerein API, HTTP and JSON

## MUST

-   Every HTTP request in `EasyVereinApiClient` goes through `SendWithErrorHandling(...)`.
-   Use the repository's `EnsureSuccessOrThrowAsync` behavior instead of bypassing centralized status handling.
-   Request bodies must have a fixed `Content-Length`. Use the established serialized-content pattern (`BuildJsonContent` or equivalent fixed-length `StringContent`). Do not introduce chunked `PostAsJsonAsync`, `PatchAsJsonAsync` or streamed `JsonContent`.
-   Updates are PATCH operations and contain only explicitly supplied fields. Do not use PUT for resource updates.
-   PATCH dictionaries use `{Resource}Fields` constants as keys.
-   String update values are included only when `HasValue` accepts them; absent, empty and literal `"null"` values must not overwrite existing API data.
-   Nullable value-type update parameters are included only when `.HasValue`.
-   Tool-side validation failures must not accidentally result in a destructive or partial PATCH.

## SHOULD

-   API field/filter names live in `internal static class {Resource}Fields`; do not repeat API field names as string literals in entities, queries or PATCH dictionaries.
-   Use only `System.Text.Json`.
-   Serialized entity properties use `[JsonPropertyName(ResourceFields.X)]`.
-   Reuse the existing property converters (`FlexibleIdConverter`, `FlexibleDecimalConverter`, `FlexibleDateTimeConverter`, `DateOnlyJsonConverter`, `FlexibleReferenceConverter<T>`) instead of creating resource-specific converters when a generic converter suffices.
-   Entity properties mirror API field names in PascalCase. If an API typo/name conflict requires a different C# name, map it explicitly and document the reason.
-   Entities are nullable POCOs implementing `IHasId` where the resource participates in the established entity/reference model.
-   Properties that may be omitted by API field selection are nullable.
-   Public collection results are `IReadOnlyList<T>`.
-   `List...Async` hides pagination from callers and uses the central pagination helper.
-   Each resource uses its `{Resource}Query` object for query construction; do not scatter URL concatenation through callers.
-   Parse/format API dates and numbers with
    `CultureInfo.InvariantCulture`; use `TryParseExact` for fixed formats.
-   `Get...Async` treats HTTP 404 as `null`; the tool turns that into a clear not-found response. Do not apply this behavior to update/delete/list.
