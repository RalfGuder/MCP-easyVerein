# User Story 004: GitHub Flow als Branch-Strategie

> **GitHub Issue:** [#4 – US-0004 GitHub Flow als Branch-Strategie](https://github.com/RalfGuder/MCP-easyVerein/issues/4)

## User Story

**Als** Entwickler im Projektteam,
**möchte ich** eine klar definierte Branch-Strategie auf Basis von GitHub Flow nutzen,
**damit** die Zusammenarbeit im Team einheitlich abläuft, der `main`-Branch immer stabil bleibt und Änderungen nachvollziehbar über Pull Requests integriert werden.

## Akzeptanzkriterien

- [x] `main` ist immer stabil und deploybar *(alle Merges über PRs, Tests grün)*
- [x] Für jede User Story / jedes Issue wird ein eigener Feature-Branch erstellt *(z.B. `feature/US-0001-easyverein-mcp-server`)*
- [x] Branch-Namenskonvention ist definiert: `feature/US-XXXX-kurzbeschreibung` bzw. `fix/kurzbeschreibung` *(angewandt und im README dokumentiert)*
- [x] Änderungen werden über Pull Requests in `main` integriert *(PRs #7, #8, #9, #10)*
- [x] CI/CD-Pipeline prüft Tests vor dem Merge *(`.github/workflows/build.yml` als PR-Check)*
- [ ] Feature-Branches werden nach dem Merge gelöscht *(GitHub-Einstellung noch nicht konfiguriert)*
- [x] Die Branch-Strategie ist im README dokumentiert *(README enthält Branch-Strategie-Abschnitt)*

## Aufgaben

- [x] **GitHub Flow als Standard definieren** und dokumentieren
- [x] **Branch-Namenskonvention festlegen** und dokumentieren
- [x] **README erstellen/aktualisieren** mit Branch-Strategie-Abschnitt
- [ ] **Branch-Schutzregeln** für `main` auf GitHub konfigurieren (optional)
- [ ] **Auto-Delete** für Feature-Branches nach Merge aktivieren

## Technische Hinweise

- GitHub Flow gewählt wegen einfacher Release-Zyklen
- CI/CD-Pipeline prüft auf Ubuntu, Windows und macOS
- Bisherige PRs: #7 (US-006), #8 (Systemvoraussetzungen), #9 (MCP-Konfiguration), #10 (MCP-Server)
- Git Flow (mit `develop`, `release`, `hotfix`) wäre Overhead ohne Mehrwert für dieses Projekt
