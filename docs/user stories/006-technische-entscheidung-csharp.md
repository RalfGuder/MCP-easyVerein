# User Story 006: Technische Entscheidung C#

> **GitHub Issue:** [#6 – US-0006 Technische Entscheidung C#](https://github.com/RalfGuder/MCP-easyVerein/issues/6)

## User Story

**Als** Entwickler im Projektteam,
**möchte ich** dass der easyVerein MCP-Server in C# mit .NET 8 entwickelt wird,
**damit** das System plattformübergreifend auf Windows, Linux und macOS eingesetzt werden kann und von einer modernen, wartbaren Architektur profitiert.

## Akzeptanzkriterien

- [ ] Das Projekt wird als .NET 8 (LTS) C#-Lösung erstellt
- [ ] Die Anwendung ist plattformübergreifend lauffähig auf Windows, Linux und macOS
- [ ] Die Architektur folgt den Prinzipien der Clean Architecture (strikte Schichtentrennung)
- [ ] Ein Dockerfile ist vorhanden und das System kann als Docker-Container bereitgestellt werden
- [ ] Die Projektstruktur ist dokumentiert (README mit Aufbau und Schichten)
- [ ] Ein Build- und Testlauf ist auf allen drei Zielplattformen erfolgreich (CI/CD)
- [ ] Die Entscheidung für C# / .NET 8 ist als ADR (Architecture Decision Record) oder im README dokumentiert

## Aufgaben

- Projekt-Scaffolding erstellen als .NET 8 C#-Lösung mit Clean Architecture-Struktur
- Schichten definieren: Domain, Application, Infrastructure, Presentation/API
- Dockerfile erstellen für Container-Bereitstellung
- CI/CD-Pipeline erweitern um Multi-Plattform-Builds (Windows, Linux, macOS)
- Dokumentation erstellen: Projektstruktur, Architekturentscheidung, Build-Anleitung
- Beispiel-Test mit gewähltem Test-Framework erstellen (siehe auch US-0003)

## Technische Hinweise

- .NET 8 ist die aktuelle LTS-Version und bietet native Plattformunterstützung für Windows, Linux und macOS
- Clean Architecture ermöglicht lose Kopplung und gute Testbarkeit
- Docker-Support ermöglicht einfache Bereitstellung unabhängig vom Host-Betriebssystem
- Die Architektur sollte kompatibel mit dem MCP-Standard sein (siehe US-0001)
- Test-Framework wird im Rahmen von US-0003 festgelegt (z.B. xUnit, NUnit)
