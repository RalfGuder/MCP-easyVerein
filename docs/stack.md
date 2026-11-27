# Stack

> Die Software Factory liest diese Datei vor jeder Prüfung, Architektur, Umsetzung und Abnahme. Verbindlich sind außerdem `CLAUDE.md` im Repo-Root und die Regeln dort.

- **Sprache / Laufzeit:** C# / .NET 8.0
- **Art:** Lokaler MCP-Server (Model Context Protocol, Stdio-Transport) für die easyVerein-API, keine Weboberfläche
- **MCP SDK:** ModelContextProtocol 1.2.0
- **Architektur:** Clean Architecture, 4 Schichten (Domain, Application, Infrastructure, Server), siehe `CLAUDE.md`
- **Datenhaltung:** keine eigene; alle Daten liegen in easyVerein und werden über die REST-API gelesen und geschrieben
- **Login:** API-Key per CLI-Parameter `--api-key` oder Umgebungsvariable `EASYVEREIN_API_KEY`
- **Tests:** xUnit, Moq, coverlet; HTTP wird mit gemocktem `HttpMessageHandler` getestet
- **Paketmanager:** NuGet (`dotnet restore`)
- **Stack-Packs:** keine (die Pakete der Factory gelten für Next.js/Supabase, nicht für dieses Projekt)

## Konventionen

- **TDD:** Erst roter Test, dann Umsetzung, dann Refactoring. Mindest-Coverage 70 %.
- **Muster für neue Endpoints:** Domain-Entity + Fields-ValueObject → `IEasyVereinApiClient` erweitern → `EasyVereinApiClient` + Query-Builder → Server-Tool-Klasse mit MCP-Attributen → Registrierung in `Program.cs` (`.WithTools<T>()`). Bestehende Endpoints (z. B. Lending, InventoryObject) sind die Vorlage.
- **Antwortformat der Tools:** immer über `ToolJson` (kompakt, ohne `null`, Referenzen als ID); `list_*` mit `limit`/`page`.
- **Updates:** ausschließlich PATCH.
- **Dokumentation:** Alle Member, auch private, mit englischen XML-Kommentaren (`/// <summary>`).
- **Commits:** Präfix `feat:`, `fix:`, `test:`, `refactor:`, `docs:`, `chore:`; Deutsch oder Englisch; Bezug auf Issue mit „Verlinkt mit GitHub Issue #X". **Keine `Co-Authored-By`-Zeilen.**
- **Fachliche Doku:** User Stories liegen zusätzlich unter `easyVerein_Vault/001 User Stories/`; die Factory-Spec unter `features/<ID>-<name>/spec.md` ist der Vertrag.

## Verbote für die Factory

- **Keine Aufrufe der echten easyVerein-API.** Es gibt im Container keinen API-Key, und es darf keiner beschafft oder angefordert werden. Alles wird mit Mocks getestet. Echte Vereinsdaten dürfen nie gelesen oder verändert werden.
- Keine Änderungen an `.github/workflows/`, `Konten.md` oder `easyVerein_Vault/006 Daily Notes/`.

## Befehle

Stehen in `.factory/setup.json` (`install`, `checks`). Es gibt keinen Dev-Server (`dev` ist leer).

## Abnahme ohne Oberfläche

Statt Browser-Tests prüft die Abnahme jedes Szenario so:

1. `factory-verify` (Build + alle Tests in Release).
2. Gezielte Tests zu den Szenarien ausführen, z. B. `dotnet test --no-build -c Release --filter "FullyQualifiedName~<Klasse>"`.
3. Den Server per Stdio mit einem Platzhalter-Key starten und per JSON-RPC prüfen, ohne echte API-Aufrufe:
   ```bash
   { printf '%s\n' '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2025-06-18","capabilities":{},"clientInfo":{"name":"abnahme","version":"1"}}}'
     sleep 3
     printf '%s\n' '{"jsonrpc":"2.0","method":"notifications/initialized"}' '{"jsonrpc":"2.0","id":2,"method":"tools/list"}'
     sleep 3; } \
   | timeout 60 dotnet run --no-build -c Release --project src/MCP.EasyVerein.Server -- --api-key abnahme-platzhalter \
   | tail -1 | jq '.result.tools[] | select(.name == "<tool>")'
   ```
   Die Pausen sind nötig: Endet die Eingabe sofort, beendet sich der Server, bevor er antwortet.
   Geprüft werden: neue oder geänderte Tools erscheinen in `tools/list`, Namen, Beschreibungen und Parameter-Schemas (Pflichtfelder, Standardwerte) entsprechen der Spec. `tools/call` nur für Fälle, die ohne API-Zugriff ein Ergebnis liefern (z. B. Validierungsfehler vor dem HTTP-Aufruf).
