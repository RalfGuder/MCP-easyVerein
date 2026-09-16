# User Story 005: Verschiedene API-Versionen

> **GitHub Issue:** [#5 – US-0005 Verschiedene API-Versionen](https://github.com/RalfGuder/MCP-easyVerein/issues/5)

## User Story

**Als** Nutzer des easyVerein MCP-Servers,
**möchte ich** verschiedene Versionen der easyVerein API ansprechen können,
**damit** ich sowohl mit älteren als auch mit neueren API-Versionen arbeiten kann und beim Versionswechsel keine Funktionalität verliere.

## Akzeptanzkriterien

- [x] Alle verfügbaren easyVerein API-Versionen werden unterstützt *(v1.4, v1.5, v1.6, v1.7 in `ApiVersion.SupportedVersions`)*
- [x] Die unterstützten Versionen sind fest im Code hinterlegt *(`ApiVersion._supportedVersions`)*
- [x] Pro Serverinstanz wird eine API-Version verwendet *(Standard: v1.7 in `EasyVereinConfiguration`)*
- [x] Die API-Version ist über eine Umgebungsvariable konfigurierbar (`EASYVEREIN_API_VERSION`) *(umgesetzt in `EasyVereinConfiguration.FromEnvironment()`)*
- [ ] Die API-Version kann pro MCP-Tool-Aufruf als Parameter überschrieben werden *(Should – Infrastruktur vorhanden via `GetVersionedBaseUrl(override)`, aber noch nicht in Tools exponiert)*
- [x] Bei einer nicht unterstützten API-Version wird eine Fehlermeldung mit Vorschlag der nächstmöglichen Version ausgegeben *(umgesetzt in `ApiVersion.Create()` mit `GetClosestVersion()`)*
- [ ] Die Versionskonfiguration ist dokumentiert *(Should – noch nicht im README)*

## Aufgaben

- [x] **Versionsabstraktion implementieren** *(Value Object `ApiVersion` mit Validierung und Vorschlägen)*
- [x] **Konfiguration umsetzen**: Umgebungsvariable als Standard *(`EASYVEREIN_API_VERSION`)*
- [ ] **Parameter-Override** pro MCP-Tool-Aufruf implementieren
- [x] **Fehlerbehandlung** bei ungültiger Version mit Vorschlag *(8 Tests in `ApiVersionTests`)*
- [x] **Unterstützte Versionen pflegen** als fest hinterlegte Liste
- [ ] **Dokumentation** der Versionskonfiguration im README ergänzen

## Technische Hinweise

- Unterstützte Versionen: v1.4, v1.5, v1.6, v1.7 (Standard: v1.7)
- Versionsabstraktion als Value Object `ApiVersion` mit Immutable-Pattern
- `GetClosestVersion()` schlägt bei ungültiger Version die nächste unterstützte vor
- 8 xUnit-Tests für Versionslogik
- Die easyVerein API-Dokumentation ist unter https://easyverein.com/api/ verfügbar
