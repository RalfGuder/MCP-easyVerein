---
paths:
  - "**/*.cs"
  - "**/*.csproj"
---

# Naming and Structure

## MUST

-   Project references point inward. Domain code must not acquire infrastructure/application dependencies or unnecessary package dependencies.
-   Preserve public MCP tool names. Tool names are explicit `verb_resource` snake_case names and are not renamed casually.

## SHOULD

-   Use file-scoped namespaces matching the folder structure.
-   Keep one top-level type per file; file and type names match.
-   Use PascalCase for types, methods, properties, enums and constants.
-   Use camelCase for parameters and local variables.
-   Use `_camelCase` for private instance fields.
-   Interfaces use `I` + PascalCase.
-   Async methods outside MCP tool methods end in `Async`.
-   Use established CRUD vocabulary consistently: `List`, `Get`, `Create`, `Update`, `Delete`.
-   Method names describe an action or result with a meaningful verb. Avoid generic names such as `Handle` where a more precise name exists.
-   Avoid Hungarian notation, type prefixes, opaque abbreviations and one-letter names except conventional loop counters and very short lambdas.
-   Boolean names are positive. State predicates normally use `Is...`; avoid double negatives.
-   Reuse the established domain term for the same concept instead of introducing synonyms.
-   Default new implementation/helper types to the narrowest useful visibility. Helpers used only inside a class are `private`; query builders, field constants and internal DTOs remain `internal`.
-   Classes without an intended inheritance role should be sealed where that matches the surrounding design.

## CONSIDER

-   Keep callers close to callees: public entry points first, directly used private helpers below them.
-   Abstract base-class naming must follow the repository's established convention rather than introducing a new suffix convention.
