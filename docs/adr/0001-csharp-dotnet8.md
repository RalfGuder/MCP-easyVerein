# ADR 0001: C# / .NET 8 als Sprache und Laufzeit

## Status

Angenommen (dokumentiert am 2026-11-28, gilt seit Projektbeginn mit US-0001).

## Kontext

Der MCP-Server läuft lokal neben einem KI-Assistenten (z. B. Claude Code) und bindet die easyVerein-REST-API an. Anforderungen:

- Betrieb auf **Windows**, **Linux** und **macOS** (x86_64 und ARM64), nativ oder als **Docker**-Container.
- Ein offizielles, gepflegtes SDK für das Model Context Protocol mit Stdio-Transport.
- Viele gleichförmige Endpoints (Entity, Client, Tool), die sauber getrennt und gut testbar bleiben sollen (TDD, Coverage ≥ 70 %).
- Lange Wartbarkeit ohne häufige Laufzeit-Upgrades.

## Entscheidung

Der Server wird in **C# auf .NET 8 (LTS)** entwickelt.

- MCP-Anbindung über das offizielle C#-SDK **ModelContextProtocol** (v1.2.0, Stdio-Transport).
- **Clean Architecture** mit vier Schichten: Domain (Entities, Interfaces, Value Objects, ohne externe Abhängigkeiten), Application (Konfiguration), Infrastructure (HTTP-Client, Query-Builder), Server (MCP-Tools, DI, `Program.cs`). Abhängigkeiten zeigen nur nach innen zur Domain.
- Tests mit xUnit, Moq und coverlet; HTTP wird über einen gemockten `HttpMessageHandler` getestet.
- Auslieferung zusätzlich als **Docker**-Image (Build mit SDK 8.0, Laufzeit `runtime:8.0-alpine`).

## Betrachtete Alternativen

- **TypeScript / Node.js:** Referenz-SDK für MCP und schnell aufgesetzt, aber schwächere statische Typisierung der vielen API-Entities und eine zusätzliche Laufzeit (Node) auf den Zielrechnern.
- **Python:** Gutes MCP-SDK und knapper Code, aber Abhängigkeitsverwaltung (venv, Paketversionen) auf drei Betriebssystemen aufwendiger; Typprüfung nur optional.
- **Go:** Einzelne statische Binärdatei, aber zum Entscheidungszeitpunkt kein gleichwertig gepflegtes offizielles MCP-SDK und weniger Erfahrung im Team.
- **.NET 9 (STS):** Neuere Features, aber kürzerer Supportzeitraum als .NET 8 (LTS).

## Konsequenzen

- Plattformunabhängig auf Windows, Linux und macOS lauffähig; nativ wird das .NET 8 SDK (bzw. die Runtime) benötigt, alternativ Docker.
- Starke Typisierung der API-Modelle; Fehler fallen früh beim Build auf.
- Die Schichtentrennung macht neue Endpoints zu einem festen Ablauf aus fünf Schritten (siehe README, Abschnitt „Projektstruktur“).
- Bindung an das C#-MCP-SDK: Neue MCP-Funktionen stehen erst zur Verfügung, wenn das SDK sie unterstützt.
- Das Supportende von .NET 8 (10. November 2026) ist erreicht; der Wechsel auf die nächste LTS-Version (.NET 10) ist eine eigene, noch offene Entscheidung und wird in einer weiteren ADR festgehalten.
