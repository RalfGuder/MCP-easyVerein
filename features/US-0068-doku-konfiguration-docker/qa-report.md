# QA-Bericht: US-0068 Dokumentation vervollständigen und Konfigurationsbeispiele korrigieren

**Datum:** 2026-11-28 · **Ergebnis:** bestanden

## Akzeptanzkriterien

| AK | Ergebnis | Nachweis |
|----|----------|----------|
| AK-1 Konfiguration (CLI, Env, Default, Priorität, `--help`) | ✅ | README „Konfiguration“; `DocumentationTests.Readme_ContainsConfigurationStartAndStructureSections` |
| AK-2 Schlüssel ohne `Bearer ` | ✅ | README-Hinweis im Abschnitt „Konfiguration“ |
| AK-3 API-Versionen v1.4–v2.0, Standard v2.0, Ablehnung mit Vorschlag | ✅ | README „API-Versionen“; manuell: `--api-version v1.8` → `Fehler: … Nächste unterstützte Version: v1.7 …`, Exit 1 |
| AK-4 Docker `run -i --rm -e EASYVEREIN_API_KEY` | ✅ | README; kein `EASYVEREIN_API_TOKEN` mehr (Test) |
| AK-5 `.mcp.json` für Container | ✅ | README „Einbindung in Claude Code“ (`command: "docker"`) |
| AK-6 nativer Start mit `--project`, Verweis auf `.mcp.json.example` | ✅ | README |
| AK-7 Projektstruktur, Abhängigkeitsrichtung, 5 Schritte, ADR-Link | ✅ | README „Projektstruktur“ |
| AK-8 Endpoint-Tabelle = Program.cs | ✅ | `Readme_EndpointTable_ListsAllRegisteredTools` gleicht alle Tool-Namen der 26 registrierten Klassen ab; manuell gegen `tools/list` (121 Tools) geprüft |
| AK-9 Dockerfile-Variablen | ✅ | `Dockerfile_UsesVariablesReadByTheCode`; Docker-Build nur in CI (lokal kein Docker) |
| AK-10 `.mcp.json.example` ohne Präfix | ✅ | `McpJsonExample_KeyHasNoBearerPrefix` |
| AK-11 ADR | ✅ | `Adr_CSharpDotnet8_HasRequiredSections` |
| AK-12 `Bearer abc123` → Header `Bearer abc123` | ✅ | `Constructor_NormalizesBearerPrefix` |
| AK-13 `abc123` → Header `Bearer abc123` | ✅ | `Constructor_NormalizesBearerPrefix`, `Constructor_SetsAuthorizationHeader` |

## Edge Cases
- EC-1 `bearer`/`BEARER` ✅ · EC-2 Leerzeichen/Tab ✅ · EC-3 nur `Bearer` → wie leerer Schlüssel ✅ · EC-4 `Bearerxyz` unverändert ✅

## Tests
631 Tests grün (Domain 163, Application 13, Infrastructure 252, Server 203).

## Sicherheit
- Keine echten Schlüssel in Beispielen, nur Platzhalter. Im Docker-`.mcp.json`-Beispiel wird der Schlüssel per `-e NAME` ohne Wert durchgereicht (nicht in der Prozessliste).
- Die Fehlermeldung beim Start enthält nur die Version, nicht den Schlüssel.
- Keine Aufrufe der echten API.

## Hinweise
- Die Versionsprüfung lief bisher erst beim ersten Tool-Aufruf (Lazy-DI). Damit die Doku (AK-3, „beim Start“) stimmt, löst `Program.cs` die Konfiguration jetzt vor dem Start auf.
- Der Docker-Build wurde lokal nicht ausgeführt (kein Docker im Container); die Änderung betrifft nur `ENV`-Zeilen.
