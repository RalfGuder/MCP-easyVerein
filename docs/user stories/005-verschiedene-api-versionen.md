# User Story 005: Verschiedene API-Versionen

> **GitHub Issue:** [#5 – US-0005 Verschiedene API-Versionen](https://github.com/RalfGuder/MCP-easyVerein/issues/5)

## User Story

**Als** Nutzer des easyVerein MCP-Servers,
**möchte ich** verschiedene Versionen der easyVerein API ansprechen können,
**damit** ich sowohl mit älteren als auch mit neueren API-Versionen arbeiten kann und beim Versionswechsel keine Funktionalität verliere.

## Akzeptanzkriterien

- [ ] Alle verfügbaren easyVerein API-Versionen (v1, v2, ggf. weitere) werden unterstützt
- [ ] Die unterstützten Versionen sind fest im Code hinterlegt und werden bei Bedarf manuell erweitert
- [ ] Pro Serverinstanz wird eine API-Version verwendet
- [ ] Die API-Version ist über eine Umgebungsvariable konfigurierbar (z.B. `EASYVEREIN_API_VERSION=v2`)
- [ ] Die API-Version kann pro MCP-Tool-Aufruf als Parameter überschrieben werden
- [ ] Bei einer nicht unterstützten API-Version wird eine Fehlermeldung mit Vorschlag der nächstmöglichen Version ausgegeben
- [ ] Die Versionskonfiguration ist dokumentiert

## Aufgaben

- **Versionsabstraktion implementieren**, die API-Aufrufe an die jeweilige Version anpasst
- **Konfiguration umsetzen**: Umgebungsvariable als Standard, Parameter-Override pro Aufruf
- **Fehlerbehandlung** bei ungültiger Version: Fehlermeldung mit Vorschlag der nächstmöglichen unterstützten Version
- **Unterstützte Versionen pflegen** als fest hinterlegte Liste im Code
- **Dokumentation** der Versionskonfiguration im README ergänzen

## Technische Hinweise

- Die easyVerein API-Dokumentation ist unter https://easyverein.com/api/ verfügbar
- Unterstützte Versionen werden fest im Code definiert (kein automatisches Discovery)
- Pro Serverinstanz wird eine Standard-Version festgelegt, die pro Aufruf überschreibbar ist
- Die Versionsabstraktion sollte so gestaltet sein, dass neue Versionen mit minimalem Aufwand ergänzt werden können
