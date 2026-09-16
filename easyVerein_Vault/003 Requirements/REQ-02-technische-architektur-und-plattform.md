# REQ-02: Technische Architektur und Plattform

> **Thema:** C# / .NET 8, Clean Architecture und Deployment
> **Herkunft:** [US-0006](https://github.com/RalfGuder/MCP-easyVerein/issues/6)
> **Stand:** 2026-04-04

## Übersicht

Dieses Dokument beschreibt die Anforderungen an die technische Plattform und Architektur des easyVerein MCP-Servers. Die Entscheidung für C# mit .NET 8 und Clean Architecture bildet die Grundlage für alle weiteren Implementierungen.

## Funktionale Requirements

| ID | Requirement | MoSCoW | Herkunft |
|----|-------------|--------|----------|
| FR-017 | Projekt als .NET 8 (LTS) C#-Lösung | Must | US-0006 |
| FR-018 | Dockerfile für Container-Bereitstellung | Must | US-0006 |
| FR-019 | CI/CD-Pipeline mit Multi-Plattform-Builds | Must | US-0006 |
| FR-020 | Projekt-Scaffolding mit Clean Architecture-Struktur | Must | US-0006 |
| FR-021 | Schichten: Domain, Application, Infrastructure, Presentation/API | Must | US-0006 |

### FR-017: .NET 8 (LTS) C#-Lösung

**Priorität:** Must | **Herkunft:** US-0006

Das Projekt wird als .NET 8 Long-Term-Support C#-Lösung erstellt.

**Akzeptanzkriterien:**
- [x] Solution-Datei (.sln) mit .NET 8 Target Framework — `MCP.EasyVerein.sln` mit 4 src + 4 test Projekten
- [x] C# als Programmiersprache
- [x] .NET 8 SDK ist als Voraussetzung dokumentiert — im README

### FR-018: Dockerfile für Container-Bereitstellung

**Priorität:** Must | **Herkunft:** US-0006

Ein Dockerfile ermöglicht die Bereitstellung als Docker-Container.

**Akzeptanzkriterien:**
- [x] Multi-Stage Dockerfile vorhanden (Build + Runtime) — Alpine-basiert
- [x] Container startet den MCP-Server korrekt
- [x] Container-Image ist möglichst schlank (z.B. Alpine-basiert)
- [x] Umgebungsvariablen werden im Container unterstützt

### FR-019: CI/CD-Pipeline mit Multi-Plattform-Builds

**Priorität:** Must | **Herkunft:** US-0006

Die CI/CD-Pipeline muss Builds auf allen drei Zielplattformen ausführen.

**Akzeptanzkriterien:**
- [x] Build-Job für Windows — `.github/workflows/build.yml` matrix
- [x] Build-Job für Linux
- [x] Build-Job für macOS
- [x] Alle drei Builds sind erfolgreich

### FR-020: Clean Architecture-Projekt-Scaffolding

**Priorität:** Must | **Herkunft:** US-0006

Die Projektstruktur folgt den Prinzipien der Clean Architecture.

**Akzeptanzkriterien:**
- [x] Klare Trennung der Verantwortlichkeiten in Schichten — 4 Schichten als separate Projekte
- [x] Abhängigkeiten zeigen nach innen (Dependency Rule) — Domain hat keine externen Abhängigkeiten
- [x] Jede Schicht ist als eigenes Projekt in der Solution — Domain, Application, Infrastructure, Server

### FR-021: Schichtendefinition

**Priorität:** Must | **Herkunft:** US-0006

Die Architektur umfasst vier klar definierte Schichten.

**Akzeptanzkriterien:**
- [x] **Domain:** Geschäftslogik und Entitäten, keine externen Abhängigkeiten — Entities (Member, ContactDetails, Event, Invoice), ValueObjects (ApiVersion, *Fields), Interfaces
- [x] **Application:** Anwendungsfälle und Interfaces — Configuration (EasyVereinConfiguration)
- [x] **Infrastructure:** Implementierung externer Dienste (easyVerein API, Persistenz) — ApiClient (EasyVereinApiClient, Queries)
- [x] **Presentation/API:** MCP-Server-Endpunkte und Transportschicht — Server mit MCP-Tools (MemberTools, InvoiceTools, EventTools, ContactDetailsTools)

---

## Nicht-funktionale Requirements

| ID | Requirement | MoSCoW | Herkunft |
|----|-------------|--------|----------|
| NFR-007 | Plattformübergreifend: Windows, Linux, macOS | Must | US-0006 |
| NFR-008 | Clean Architecture mit strikter Schichtentrennung | Must | US-0006 |
| NFR-009 | Projektstruktur im README dokumentiert | Should | US-0006 |
| NFR-010 | Architekturentscheidung als ADR dokumentiert | Should | US-0006 |
| NFR-011 | Build/Test auf allen Zielplattformen erfolgreich | Must | US-0006 |

### NFR-007: Plattformübergreifende Lauffähigkeit

**Priorität:** Must | **Herkunft:** US-0006

Die Anwendung muss auf Windows, Linux und macOS lauffähig sein.

**Akzeptanzkriterien:**
- [x] Anwendung startet und funktioniert auf Windows
- [x] Anwendung startet und funktioniert auf Linux
- [x] Anwendung startet und funktioniert auf macOS
- [x] Keine plattformspezifischen Abhängigkeiten im Code

### NFR-008: Strikte Schichtentrennung

**Priorität:** Must | **Herkunft:** US-0006

Die Clean Architecture-Schichten müssen strikt eingehalten werden.

**Akzeptanzkriterien:**
- [x] Domain-Schicht hat keine Abhängigkeiten zu äußeren Schichten — nur .NET BCL
- [x] Application-Schicht kennt nur Domain
- [x] Infrastructure implementiert Interfaces aus Application — `IEasyVereinApiClient`
- [x] Dependency Injection wird für die Kopplung verwendet — `Microsoft.Extensions.DependencyInjection` in Program.cs

### NFR-009: Projektstruktur-Dokumentation

**Priorität:** Should | **Herkunft:** US-0006

Die Projektstruktur und der Aufbau der Schichten sind im README dokumentiert.

**Akzeptanzkriterien:**
- [ ] Verzeichnisstruktur ist beschrieben
- [ ] Verantwortlichkeit jeder Schicht ist erklärt
- [ ] Abhängigkeitsrichtung ist visualisiert

### NFR-010: Architecture Decision Record

**Priorität:** Should | **Herkunft:** US-0006

Die Entscheidung für C# / .NET 8 und Clean Architecture ist als ADR oder im README dokumentiert.

**Akzeptanzkriterien:**
- [ ] Kontext und Motivation der Entscheidung sind beschrieben
- [ ] Bewertete Alternativen sind aufgeführt
- [ ] Konsequenzen der Entscheidung sind benannt

### NFR-011: Plattformübergreifende Build-/Test-Erfolge

**Priorität:** Must | **Herkunft:** US-0006

Build und Tests müssen auf allen drei Zielplattformen erfolgreich durchlaufen.

**Akzeptanzkriterien:**
- [x] CI/CD zeigt grüne Builds für Windows, Linux und macOS — CI/CD matrix zeigt grün
- [x] Alle Tests bestehen auf allen drei Plattformen — 46 Tests alle grün
- [x] Keine plattformspezifischen Testfehler

---

## Abhängigkeiten

| Requirement | hängt ab von | Grund |
|-------------|-------------|-------|
| FR-019 | FR-017 | CI/CD-Pipeline setzt .NET 8-Projekt voraus |
| FR-020, FR-021 | FR-017 | Architektur-Scaffolding basiert auf .NET 8-Lösung |
| NFR-011 | FR-019 | Plattform-Tests setzen Multi-Plattform-CI/CD voraus |
| REQ-01 (alle) | FR-020, FR-021 | Server-Implementierung baut auf Architektur auf |
| REQ-03 (FR-029) | FR-017 | Test-Framework setzt .NET 8 voraus |

## Offene Fragen

- [x] Soll ein konkretes DI-Framework vorgegeben werden (z.B. Microsoft.Extensions.DependencyInjection)? — Ja, `Microsoft.Extensions.DependencyInjection` — bereits umgesetzt
- [ ] Sollen Self-Contained Deployments oder Framework-Dependent Deployments bevorzugt werden?
