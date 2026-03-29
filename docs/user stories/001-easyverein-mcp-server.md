# User Story 001: easyVerein API als lokalen MCP-Server bereitstellen

> **GitHub Issue:** [#1 – easyVerein API als lokalen MCP-Server bereitstellen](https://github.com/RalfGuder/MCP-easyVerein/issues/1)

## User Story

**Als** Vereinsverwalter, der einen KI-Assistenten wie Claude nutzt,
**möchte ich** über einen lokalen MCP-Server auf die easyVerein API zugreifen können,
**damit** ich Vereinsdaten wie Mitglieder, Rechnungen und Veranstaltungen direkt aus dem KI-Assistenten heraus abfragen und verwalten kann, ohne die easyVerein-Oberfläche manuell bedienen zu müssen.

## Akzeptanzkriterien

- [ ] MCP-Server startet lokal und ist über stdio/SSE erreichbar
- [ ] Authentifizierung mit easyVerein API-Token funktioniert
- [ ] Mindestens Lese-Zugriff auf Mitgliederdaten ist als MCP-Tool verfügbar
- [ ] Dokumentation (README) mit Installations- und Konfigurationsanleitung
- [ ] Fehlerbehandlung bei ungültigen API-Tokens oder Netzwerkfehlern

## Aufgaben

- **MCP-Server implementieren**, der lokal ausgeführt wird und die easyVerein REST-API kapselt
- **Authentifizierung** über easyVerein API-Token unterstützen
- **CRUD-Operationen** für zentrale easyVerein-Ressourcen als MCP-Tools bereitstellen, z.B.:
  - Mitglieder (Abfragen, Anlegen, Bearbeiten)
  - Rechnungen / Buchungen
  - Veranstaltungen
  - Kontaktdaten
- **Konfiguration** über Umgebungsvariablen oder Konfigurationsdatei ermöglichen (API-Token, Basis-URL)

## Technische Hinweise

- Das Projekt nutzt eine MIT-Lizenz
- Die easyVerein API-Dokumentation ist unter https://easyverein.com/api/ verfügbar
- Der MCP-Server sollte dem [MCP-Standard](https://modelcontextprotocol.io/) entsprechen
