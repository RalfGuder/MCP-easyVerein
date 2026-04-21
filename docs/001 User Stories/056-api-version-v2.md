# US-0056 API-Version v2.0 als unterstützte Version hinzufügen

**Issue:** [#69](https://github.com/RalfGuder/MCP-easyVerein/issues/69)

## User Story

**Als** Betreiber eines easyVerein-MCP-Servers, **möchte ich** die API-Version v2.0 explizit konfigurieren können (`EASYVEREIN_API_VERSION=v2.0` oder `--api-version v2.0`), **damit** ich die neue API testen kann, ohne den Default-Stand meiner Produktion zu verändern.

## Akzeptanzkriterien

- [x] `ApiVersion.SupportedVersions` enthält `"v2.0"`.
- [x] `ApiVersion.Create("v2.0")` erzeugt eine gültige Instanz.
- [x] `ApiVersion.Default.Version` liefert weiterhin `"v1.7"`.
- [x] `EasyVereinConfiguration.FromConfiguration` akzeptiert `v2.0`.
- [x] Zwei neue Tests in `ApiVersionTests` erfasst.
- [x] Alle bestehenden Tests bleiben grün.
- [x] `CLAUDE.md` dokumentiert v2.0 als unterstützte Version.

## Aufgaben

- Feature-Branch `feature/US-0056-api-version-v2` anlegen
- TDD: zwei Red-Tests → Green-Implementation → Refactor (entfällt)
- `CLAUDE.md` aktualisieren
- PR gegen `main` erstellen

## Technische Hinweise

- Einzige Code-Änderung: `_supportedVersions`-Array in `src/MCP.EasyVerein.Domain/ValueObjects/ApiVersion.cs`.
- `ApiVersion.Default.Version` bleibt bewusst `"v1.7"`; der Wechsel des Defaults passiert in einem späteren Sub-Projekt (SP 10) nach vollständiger Entity-Migration.
- Siehe Design-Spec: [`docs/superpowers/specs/2026-04-21-api-v2-support-design.md`](../superpowers/specs/2026-04-21-api-v2-support-design.md)

## Kontext

Teil der v2.0-Migration, Sub-Projekt 1 von 10. Folgende Sub-Projekte (SP 2–9) migrieren die Entities (Member, ContactDetails, Invoice, Event, Booking, Calendar, Announcement, BankAccount). Der Default-Wechsel v1.7 → v2.0 ist Sub-Projekt 10 am Ende der Reihe.
