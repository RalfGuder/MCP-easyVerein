# Architecture Rules

## Services

- Services are registered through Microsoft.Extensions.DependencyInjection.
- Constructor injection is the default dependency mechanism.
- Do not instantiate infrastructure services directly.
- Do not use IServiceProvider as a service locator.

## UI

- ViewModels must not directly access Views.
- UI interaction is abstracted behind services.
- Business logic does not belong in code-behind.