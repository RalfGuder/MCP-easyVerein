---
paths:
  - "src/MCP.EasyVerein.Server/Tools/**/*.cs"
  - "src/MCP.EasyVerein.Server/Program.cs"
---

# MCP Tools, Protocol and Security

## MUST

-   stdout belongs to the MCP stdio transport after host startup. Do not add `Console.Write*`, `Console.Out` or console logging after startup. Pre-host help output is the known exception.
-   Every tool method has an explicit `[McpServerTool(Name = "verb_resource")]` name in snake_case. List operations use the plural resource; other CRUD operations use the singular resource.
-   Treat tool names as public API.
-   Tool methods must not leak exceptions across the MCP boundary. Follow the established `try` / `catch (Exception ex)` tool boundary and the repository's error response format.
-   Never return or log API keys, passwords or tokens in clear text.
-   Tools handling secrets mark their description with `SENSITIVE:` and redact secrets from success and exception output.
-   Every tool and every tool parameter except `CancellationToken` has an English `[Description]` that is sufficient for an LLM client to call it correctly.
-   Tool methods accept `CancellationToken ct` as their final required parameter and propagate it.

## SHOULD

Tool methods follow this shape:

``` text
validation -> try -> client call -> serialization/result -> catch
```

-   Tool methods return `Task<string>`.
-   Required parameters are non-nullable.
-   Optional parameters are nullable and use their real type (`bool?`, `decimal?`, etc.), not strings standing in for typed values.
-   Validate known required fields, lengths, ranges and formats before the API call. Invalid input returns a concrete `ERROR:` response and does not call the client.
-   Delete-tool descriptions state authorization requirements and whether deletion is permanent or moves the record to the wastebasket. The success message must agree with the description.
