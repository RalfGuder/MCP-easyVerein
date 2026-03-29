# User Story 004: GitHub Flow als Branch-Strategie

> **GitHub Issue:** [#4 – US-0004 GitHub Flow als Branch-Strategie](https://github.com/RalfGuder/MCP-easyVerein/issues/4)

## User Story

**Als** Entwickler im Projektteam,
**möchte ich** eine klar definierte Branch-Strategie auf Basis von GitHub Flow nutzen,
**damit** die Zusammenarbeit im Team einheitlich abläuft, der `main`-Branch immer stabil bleibt und Änderungen nachvollziehbar über Pull Requests integriert werden.

## Akzeptanzkriterien

- [ ] `main` ist immer stabil und deploybar
- [ ] Für jede User Story / jedes Issue wird ein eigener Feature-Branch erstellt
- [ ] Branch-Namenskonvention ist definiert: `feature/US-XXXX-kurzbeschreibung` bzw. `fix/kurzbeschreibung`
- [ ] Änderungen werden über Pull Requests in `main` integriert
- [ ] CI/CD-Pipeline prüft Tests vor dem Merge (siehe US-0003)
- [ ] Feature-Branches werden nach dem Merge gelöscht
- [ ] Die Branch-Strategie ist im README dokumentiert

## Aufgaben

- **GitHub Flow als Standard definieren** und im Team kommunizieren
- **Branch-Namenskonvention festlegen** und dokumentieren
- **README erstellen/aktualisieren** mit Branch-Strategie-Abschnitt
- **Branch-Schutzregeln** für `main` auf GitHub konfigurieren (optional)

## Technische Hinweise

- GitHub Flow wurde gewählt, weil es für kleine Teams und einfache Release-Zyklen optimal ist
- Git Flow (mit `develop`, `release`, `hotfix`) wäre Overhead ohne Mehrwert für dieses Projekt
- Die Branch-Strategie ergänzt sich mit der TDD-Entscheidung aus US-0003 (CI/CD prüft Tests vor Merge)
