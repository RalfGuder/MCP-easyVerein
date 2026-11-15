---
paths:
  - "**/*.cs"
---

# Clean Code and Design

## SHOULD

-   Use guard clauses and early returns for preconditions and exceptional cases. Avoid unnecessary `else` after `return` or  `throw`.
-   A method should perform one coherent task at one abstraction level.
-   A class should have one primary reason to change.
-   Prefer 0-2 method parameters. At 3 parameters, actively consider a parameter object; at 4 or more, require a strong reason not to introduce one.
-   Do not use a boolean flag parameter to switch between distinct behaviors; prefer separately named operations.
-   Do not mutate global or unrelated shared state. Pass inputs explicitly and return results explicitly.
-   Replace business-significant magic strings/numbers with named constants, enums, value objects or domain concepts.
-   Prefer composition to inheritance unless the relationship is genuinely "is-a".
-   Keep interfaces small and role-oriented. An implementation must not be forced to leave members empty or throw because they are irrelevant.
-   Inject replaceable dependencies through constructors and depend on abstractions at boundaries.
-   Do not implement self-managed singletons (`static _instance`, `GetInstance()`); lifetime belongs to the composition root/DI container.
-   Remove duplicated business logic when a stable shared abstraction exists. Do not introduce a worse abstraction merely to eliminate superficial duplication.
-   Use polymorphism when behavior varies by stable type/variant and adding a variant would otherwise require editing a central type switch.

## CONSIDER

-   Encapsulate non-trivial business conditions behind a meaningful method/property or explanatory variable.
-   Prefer `record` for immutable data/parameter objects when the project language version supports it and the type has no behavior requiring class identity.
-   Use primary constructors when the project language version supports them and the constructor only captures dependencies/initial values.
-   Prefer `foreach` or LINQ when an index has no meaning of its own.
-   Prefer modern collection expressions when supported by the project's configured C# version.
