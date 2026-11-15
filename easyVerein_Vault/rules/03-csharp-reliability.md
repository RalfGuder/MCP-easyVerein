---
paths:
  - "**/*.cs"
  - "**/*.csproj"
---

# C# Reliability

## MUST

-   Never use `.Result`, `.Wait()`, `GetAwaiter().GetResult()` or `async void` for normal asynchronous work.
-   I/O call chains accept and propagate `CancellationToken`; `ct` is the final parameter. Do not reinterpret caller-requested cancellation as an ordinary error.
-   When rethrowing the caught exception, use `throw;`, never `throw ex;`.
-   Never leave a `catch` block empty or silently swallow an error.

## SHOULD

-   Keep nullable reference types enabled.
-   Express nullability in types. Do not suppress nullable warnings with `#pragma` or indiscriminate null-forgiving `!`.
-   Use `!` only immediately after a condition that establishes non-nullness but is not understood by the compiler.
-   Use concrete parameter types instead of `dynamic` or unexplained `object`.
-   Parse and format externally exchanged numbers and dates with an explicit invariant culture and explicit format where applicable.
-   Preserve the original exception as `InnerException` when wrapping an exception.
-   Do not add exception handling merely to catch and rethrow unchanged errors.
