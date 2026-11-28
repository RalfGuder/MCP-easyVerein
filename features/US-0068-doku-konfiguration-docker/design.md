# Design: US-0068 Dokumentation vervollständigen und Konfigurationsbeispiele korrigieren

## Ansatz
Überwiegend Dokumentation. Einzige Code-Änderung: Der API-Client entfernt ein vorangestelltes `Bearer `-Präfix aus dem konfigurierten Schlüssel, bevor er den `Authorization`-Header setzt.

## Bausteine
- **README.md** (Deutsch, bestehende Abschnitte bleiben):
  - „Konfiguration“: Tabelle Schlüssel/URL/Version mit CLI-Parameter, Umgebungsvariable, Standardwert; Priorität CLI > Env > Standard; `--help`; Hinweis „Schlüssel ohne `Bearer `“ (ein vorhandenes Präfix wird toleriert).
  - „API-Versionen“: v1.4–v2.0, Standard v2.0, Abbruch beim Start mit Vorschlag der nächstgelegenen Version (Text aus `ApiVersion`).
  - Start: Docker mit `docker run -i --rm -e EASYVEREIN_API_KEY=<token> mcp-easyverein` und `.mcp.json`-Eintrag für den Container; nativ mit `dotnet run --project src/MCP.EasyVerein.Server` und Verweis auf `.mcp.json.example`.
  - „Projektstruktur“: vier Schichten, Abhängigkeitsrichtung, fünf Schritte für neue Endpoints, Link auf die ADR.
  - „Endpoints“: Tabelle der 26 in `Program.cs` registrierten Tool-Klassen mit Operationen (aus den `McpServerTool`-Namen abgeleitet).
- **Dockerfile:** `ENV EASYVEREIN_API_KEY`, `EASYVEREIN_API_URL`, `EASYVEREIN_API_VERSION`.
- **.mcp.json.example:** Platzhalter `<your-easyverein-api-token-here>` ohne Präfix.
- **docs/adr/0001-csharp-dotnet8.md:** Status, Kontext, Entscheidung, Alternativen, Konsequenzen.
- **EasyVereinApiClient:** neue statische Methode `NormalizeApiKey(string?)`:
  Trim; beginnt der Wert (Groß-/Kleinschreibung egal) mit `Bearer` gefolgt von Leerraum oder endet dort (`Bearer` allein), wird das Präfix entfernt und erneut getrimmt. `Bearerxyz` bleibt unverändert (EC-4). `Bearer`/`Bearer ` ergibt einen leeren Schlüssel → Header `Bearer ` wie heute bei leerem Schlüssel (EC-3).

## Tests
xUnit in `EasyVereinApiClientTests`: Header für `abc123`, `Bearer abc123`, `bearer`/`BEARER`, `  Bearer   abc123 `, `Bearer`, `Bearer `, `Bearerxyz`. Doku-AKs werden zusätzlich mit einem Test geprüft, der die Endpoint-Tabelle im README gegen die `.WithTools<T>()`-Registrierungen in `Program.cs` abgleicht. Keine echten API-Aufrufe.
