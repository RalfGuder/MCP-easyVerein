# User Story 003: TDD als Designentscheidung

> **GitHub Issue:** [#3 – US-0003 TDD als Designentscheidung](https://github.com/RalfGuder/MCP-easyVerein/issues/3)

## User Story

**Als** Entwickler im Projektteam,
**möchte ich** dass die Software konsequent nach dem TDD-Ansatz (Test-Driven Development) entwickelt wird,
**damit** die Codequalität durch automatisierte Tests von Anfang an sichergestellt ist und Regressionen frühzeitig erkannt werden.

## Akzeptanzkriterien

- [x] Jede neue Funktionalität wird nach dem Red-Green-Refactor-Zyklus entwickelt *(angewandt bei der Implementierung von REQ-01)*
- [x] Tests werden vor der Implementierung geschrieben *(Tests zuerst erstellt, dann Implementierung)*
- [x] Eine CI/CD-Pipeline prüft alle Tests automatisch bei jedem Push *(umgesetzt in `.github/workflows/build.yml`)*
- [ ] Pre-Commit Hooks führen Tests vor jedem Commit automatisch aus *(noch nicht eingerichtet)*
- [ ] Eine Mindest-Code-Coverage von 70% wird durchgesetzt *(Coverage wird gesammelt, Schwellenwert noch nicht erzwungen)*
- [ ] Die TDD-Vorgehensweise ist im README des Projekts dokumentiert *(noch nicht dokumentiert)*
- [x] Das Test-Framework ist festgelegt *(xUnit mit Moq und coverlet.collector)*

## Aufgaben

- [x] **Test-Framework auswählen** und als Projektabhängigkeit einrichten *(xUnit 2.4.2, Moq 4.20.72)*
- [x] **CI/CD-Pipeline konfigurieren** mit automatischer Testausführung *(`.github/workflows/build.yml`)*
- [ ] **CI/CD-Pipeline** Coverage-Prüfung (≥ 70%) erzwingen
- [ ] **Pre-Commit Hooks einrichten**, die Tests vor jedem Commit ausführen
- [ ] **TDD-Workflow im README dokumentieren** (Red-Green-Refactor-Zyklus, Konventionen)
- [x] **Beispieltest erstellen** als Vorlage *(46 Tests in 4 Testprojekten: Domain: 19, Application: 13, Infrastructure: 14, Server: 0)*

## Technische Hinweise

- xUnit als Test-Framework, Moq als Mocking-Bibliothek
- coverlet.collector für Code-Coverage-Erfassung
- CI/CD läuft auf Ubuntu, Windows und macOS
- 4 Testprojekte: Domain.Tests, Application.Tests, Infrastructure.Tests, Server.Tests
- Der Red-Green-Refactor-Zyklus ist verbindlich:
  1. **Red:** Fehlschlagenden Test schreiben
  2. **Green:** Minimalen Code schreiben, damit der Test besteht
  3. **Refactor:** Code und Tests verbessern, ohne Verhalten zu ändern
