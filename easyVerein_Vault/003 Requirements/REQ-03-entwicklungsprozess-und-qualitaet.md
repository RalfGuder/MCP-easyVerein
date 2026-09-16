# REQ-03: Entwicklungsprozess und Qualitätssicherung

> **Thema:** TDD, CI/CD und GitHub Flow Branch-Strategie
> **Herkunft:** [US-0003](https://github.com/RalfGuder/MCP-easyVerein/issues/3), [US-0004](https://github.com/RalfGuder/MCP-easyVerein/issues/4)
> **Stand:** 2026-04-04

## Übersicht

Dieses Dokument beschreibt die Anforderungen an den Entwicklungsprozess: Test-Driven Development als verbindliche Vorgehensweise, eine CI/CD-Pipeline zur automatischen Qualitätssicherung und GitHub Flow als Branch-Strategie für die Zusammenarbeit im Team.

## Funktionale Requirements

| ID | Requirement | MoSCoW | Herkunft |
|----|-------------|--------|----------|
| FR-022 | CI/CD-Pipeline mit automatischer Testausführung bei jedem Push | Must | US-0003 |
| FR-023 | Pre-Commit Hooks führen Tests vor jedem Commit aus | Must | US-0003 |
| FR-024 | CI/CD prüft Code-Coverage (≥ 70%) | Must | US-0003 |
| FR-025 | Feature-Branch pro User Story / Issue | Must | US-0004 |
| FR-026 | Integration über Pull Requests in main | Must | US-0004 |
| FR-027 | CI/CD prüft Tests vor Merge | Must | US-0003, US-0004 |
| FR-028 | Feature-Branches nach Merge löschen | Should | US-0004 |
| FR-029 | Test-Framework auswählen und einrichten | Must | US-0003 |

### FR-022: CI/CD-Pipeline mit automatischer Testausführung

**Priorität:** Must | **Herkunft:** US-0003

Bei jedem Push wird die CI/CD-Pipeline automatisch ausgelöst und führt alle Tests aus.

**Akzeptanzkriterien:**
- [x] Pipeline wird bei jedem Push auf jeden Branch ausgelöst — `.github/workflows/build.yml` triggert auf main, feature/**, fix/**
- [x] Alle Unit-Tests werden ausgeführt — 46 Tests
- [x] Testergebnisse sind im Pipeline-Log sichtbar
- [x] Pipeline schlägt bei fehlgeschlagenen Tests fehl

### FR-023: Pre-Commit Hooks

**Priorität:** Must | **Herkunft:** US-0003

Pre-Commit Hooks führen Tests automatisch vor jedem Commit aus.

**Akzeptanzkriterien:**
- [ ] Pre-Commit Hook ist im Repository konfiguriert
- [ ] Hook führt Tests vor dem Commit aus
- [ ] Commit wird bei fehlgeschlagenen Tests abgebrochen
- [ ] Hook kann bei Bedarf übersprungen werden (mit explizitem Flag)

### FR-024: Code-Coverage-Prüfung in CI/CD

**Priorität:** Must | **Herkunft:** US-0003

Die CI/CD-Pipeline prüft die Code-Coverage und erzwingt eine Mindest-Coverage von 70%.

**Akzeptanzkriterien:**
- [x] Coverage wird automatisch gemessen — `--collect:"XPlat Code Coverage"` in build.yml
- [x] Coverage-Bericht wird generiert — `coverage.cobertura.xml` als Artifact hochgeladen
- [ ] Pipeline schlägt fehl bei Coverage unter 70% — Schwellenwert noch nicht erzwungen
- [x] Coverage-Wert ist im Pipeline-Log sichtbar — im Artifact verfügbar

### FR-025: Feature-Branches

**Priorität:** Must | **Herkunft:** US-0004

Für jede User Story und jedes Issue wird ein eigener Feature-Branch erstellt.

**Akzeptanzkriterien:**
- [x] Jede Änderung erfolgt auf einem dedizierten Feature-Branch — z.B. `feature/US-0008-api-feldmapping`
- [x] Branch-Namenskonvention wird eingehalten (siehe NFR-016)
- [x] Keine direkten Commits auf `main` — alle Änderungen über PRs

### FR-026: Integration über Pull Requests

**Priorität:** Must | **Herkunft:** US-0004

Änderungen werden ausschließlich über Pull Requests in den `main`-Branch integriert.

**Akzeptanzkriterien:**
- [x] Jede Integration in `main` erfolgt über einen Pull Request — PRs #7, #8, #9, #10, #15 etc.
- [x] Pull Requests enthalten eine Beschreibung der Änderungen
- [ ] Mindestens ein Review vor dem Merge (empfohlen) — kein Review-Enforcement konfiguriert

### FR-027: Tests vor Merge

**Priorität:** Must | **Herkunft:** US-0003, US-0004

Die CI/CD-Pipeline prüft alle Tests, bevor ein Pull Request gemerged werden kann.

**Akzeptanzkriterien:**
- [ ] CI/CD-Status ist als Merge-Voraussetzung konfiguriert — Branch Protection Rules nicht konfiguriert
- [x] Merge ist nur bei grüner Pipeline möglich — praktisch eingehalten, aber nicht technisch erzwungen
- [x] Tests und Coverage werden geprüft — Tests ja, Coverage-Schwellenwert nein

### FR-028: Feature-Branches nach Merge löschen

**Priorität:** Should | **Herkunft:** US-0004

Feature-Branches werden nach dem erfolgreichen Merge automatisch oder manuell gelöscht.

**Akzeptanzkriterien:**
- [ ] Automatisches Löschen nach Merge ist konfiguriert (GitHub-Einstellung)
- [ ] Alternativ: Team-Konvention für manuelles Löschen

### FR-029: Test-Framework auswählen und einrichten

**Priorität:** Must | **Herkunft:** US-0003

Ein geeignetes Test-Framework wird ausgewählt und als Projektabhängigkeit eingerichtet.

**Akzeptanzkriterien:**
- [x] Test-Framework ist ausgewählt (z.B. xUnit, NUnit) — xUnit 2.4.2
- [x] Test-Projekt ist in der Solution eingerichtet — 4 Testprojekte in der Solution
- [x] Mindestens ein Beispieltest ist vorhanden — 46 Tests
- [x] Tests können lokal und in der CI/CD-Pipeline ausgeführt werden

---

## Nicht-funktionale Requirements

| ID | Requirement | MoSCoW | Herkunft |
|----|-------------|--------|----------|
| NFR-012 | Red-Green-Refactor-Zyklus als verbindlicher Entwicklungsprozess | Must | US-0003 |
| NFR-013 | Tests werden vor der Implementierung geschrieben | Must | US-0003 |
| NFR-014 | Mindest-Code-Coverage von 70% | Must | US-0003 |
| NFR-015 | main-Branch ist immer stabil und deploybar | Must | US-0004 |
| NFR-016 | Branch-Namenskonvention einhalten | Must | US-0004 |
| NFR-017 | TDD-Vorgehensweise im README dokumentiert | Should | US-0003 |
| NFR-018 | Branch-Strategie im README dokumentiert | Should | US-0004 |
| NFR-019 | Branch-Schutzregeln für main auf GitHub | Could | US-0004 |

### NFR-012: Red-Green-Refactor-Zyklus

**Priorität:** Must | **Herkunft:** US-0003

Der Red-Green-Refactor-Zyklus ist der verbindliche Entwicklungsprozess für alle neuen Funktionalitäten.

**Akzeptanzkriterien:**
- [x] **Red:** Fehlschlagender Test wird zuerst geschrieben — angewandt bei allen Feature-Implementierungen
- [x] **Green:** Minimaler Code, damit der Test besteht
- [x] **Refactor:** Code und Tests werden verbessert, ohne Verhalten zu ändern
- [ ] Zyklus ist im Team kommuniziert und dokumentiert — nicht formal dokumentiert, aber im Team angewandt

### NFR-013: Test-First-Ansatz

**Priorität:** Must | **Herkunft:** US-0003

Tests werden grundsätzlich vor der eigentlichen Implementierung geschrieben.

**Akzeptanzkriterien:**
- [x] Commit-Historie zeigt Test-Commits vor Implementierungs-Commits
- [x] Kein produktiver Code ohne zugehörigen Test

### NFR-014: Mindest-Code-Coverage

**Priorität:** Must | **Herkunft:** US-0003

Eine Mindest-Code-Coverage von 70% wird sowohl lokal als auch in der CI/CD-Pipeline durchgesetzt.

**Akzeptanzkriterien:**
- [ ] Coverage wird in CI/CD geprüft (siehe FR-024)
- [ ] Coverage wird in Pre-Commit Hooks geprüft (siehe FR-023)
- [ ] Schwellenwert von 70% ist konfigurierbar aber standardmäßig aktiv

### NFR-015: Stabilität des main-Branch

**Priorität:** Must | **Herkunft:** US-0004

Der `main`-Branch ist zu jedem Zeitpunkt stabil und deploybar.

**Akzeptanzkriterien:**
- [x] Alle Tests sind auf `main` grün
- [x] Keine unfertigen Features auf `main`
- [x] `main` kann jederzeit released werden

### NFR-016: Branch-Namenskonvention

**Priorität:** Must | **Herkunft:** US-0004

Alle Branches folgen einer definierten Namenskonvention.

**Akzeptanzkriterien:**
- [x] Feature-Branches: `feature/US-XXXX-kurzbeschreibung`
- [x] Fix-Branches: `fix/kurzbeschreibung`
- [x] Konvention ist dokumentiert und wird eingehalten — in CLAUDE.md und README

### NFR-017: TDD-Dokumentation

**Priorität:** Should | **Herkunft:** US-0003

Die TDD-Vorgehensweise ist im README des Projekts dokumentiert.

**Akzeptanzkriterien:**
- [ ] Red-Green-Refactor-Zyklus ist beschrieben
- [ ] Konventionen für Testbenennung sind festgelegt
- [ ] Beispiel für TDD-Workflow ist enthalten

### NFR-018: Branch-Strategie-Dokumentation

**Priorität:** Should | **Herkunft:** US-0004

Die GitHub Flow Branch-Strategie ist im README dokumentiert.

**Akzeptanzkriterien:**
- [x] Workflow ist beschrieben (Branch erstellen → entwickeln → PR → Merge → Branch löschen) — im README
- [x] Namenskonventionen sind aufgeführt
- [x] Begründung für GitHub Flow (vs. Git Flow) ist angegeben

### NFR-019: Branch-Schutzregeln

**Priorität:** Could | **Herkunft:** US-0004

Branch-Schutzregeln für `main` auf GitHub konfigurieren.

**Akzeptanzkriterien:**
- [ ] Direkte Pushes auf `main` sind gesperrt
- [ ] Mindestens ein Approval für Pull Requests erforderlich
- [ ] CI/CD-Status als Merge-Voraussetzung

---

## Abhängigkeiten

| Requirement | hängt ab von | Grund |
|-------------|-------------|-------|
| FR-022, FR-024, FR-027 | REQ-02 (FR-017) | CI/CD setzt .NET 8-Projekt voraus |
| FR-023 | FR-029 | Pre-Commit Hooks benötigen eingerichtetes Test-Framework |
| FR-029 | REQ-02 (FR-017) | Test-Framework muss zum .NET 8-Stack passen |
| FR-027 | FR-022, FR-026 | Tests vor Merge setzt CI/CD und PR-Workflow voraus |

## Offene Fragen

- [x] Welches Test-Framework soll verwendet werden (xUnit, NUnit, MSTest)? — xUnit 2.4.2 — ausgewählt und eingerichtet
- [x] Soll ein Coverage-Tool vorgegeben werden (z.B. Coverlet)? — coverlet.collector — als Paketabhängigkeit in allen Testprojekten
- [ ] Sollen Pre-Commit Hooks über Husky.NET oder ein anderes Tool verwaltet werden?
