---
paths:
  - "**/*.cs"
---

# C# Coding Rules

- Enable and respect nullable reference types.
- Do not suppress nullable warnings without justification.
- Prefer dependency injection over service location.
- Prefer readonly fields where possible.
- Use async/await consistently.
- Async methods must use the Async suffix.
- Do not use async void except for event handlers.
- Do not block asynchronous operations using .Result or .Wait().