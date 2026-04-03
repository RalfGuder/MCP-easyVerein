# User Story 006: Technische Entscheidung C#

> **GitHub Issue:** [#6 – US-0006 Technische Entscheidung C#](https://github.com/RalfGuder/MCP-easyVerein/issues/6)

## User Story

**Als** Entwickler im Projektteam,
**möchte ich** dass der easyVerein MCP-Server in C# mit .NET 8 entwickelt wird,
**damit** das System plattformübergreifend auf Windows, Linux und macOS eingesetzt werden kann und von einer modernen, wartbaren Architektur profitiert.

## Akzeptanzkriterien

- [x] Das Projekt wird als .NET 8 (LTS) C#-Lösung erstellt *(Solution `MCP.EasyVerein.sln` mit Target Framework `net8.0`)*
- [x] Die Anwendung ist plattformübergreifend lauffähig auf Windows, Linux und macOS *(CI/CD baut auf allen drei Plattformen)*
- [x] Die Architektur folgt den Prinzipien der Clean Architecture *(4 Schichten: Domain, Application, Infrastructure, Server)*
- [x] Ein Dockerfile ist vorhanden und das System kann als Docker-Container bereitgestellt werden *(Multi-Stage Dockerfile, Alpine-basiert)*
- [ ] Die Projektstruktur ist dokumentiert (README mit Aufbau und Schichten) *(Should – noch nicht im README)*
- [x] Ein Build- und Testlauf ist auf allen drei Zielplattformen erfolgreich (CI/CD) *(`.github/workflows/build.yml` mit Matrix: ubuntu, windows, macos)*
- [ ] Die Entscheidung für C# / .NET 8 ist als ADR dokumentiert *(Should – noch nicht erstellt)*

## Aufgaben

- [x] **Projekt-Scaffolding erstellen** als .NET 8 C#-Lösung mit Clean Architecture-Struktur
- [x] **Schichten definieren**: Domain, Application, Infrastructure, Presentation/API
- [x] **Dockerfile erstellen** für Container-Bereitstellung
- [x] **CI/CD-Pipeline** mit Multi-Plattform-Builds (Windows, Linux, macOS)
- [ ] **Dokumentation** erstellen: Projektstruktur, Architekturentscheidung
- [x] **Beispiel-Test** mit xUnit erstellt *(25 Tests in 3 Testprojekten)*

## Technische Hinweise

- .NET 8.0.125 SDK, ModelContextProtocol 1.2.0
- Clean Architecture: Abhängigkeiten zeigen nach innen (Dependency Rule)
- DI via `Microsoft.Extensions.DependencyInjection`
- xUnit 2.4.2 + Moq 4.20.72 + coverlet.collector
- Docker-Support ermöglicht einfache Bereitstellung unabhängig vom Host-Betriebssystem
