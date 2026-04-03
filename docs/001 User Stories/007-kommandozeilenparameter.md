# User Story 007: Kommandozeilenparameter für API-Konfiguration

> **GitHub Issue:** [#12 – US-0007 Kommandozeilenparameter für API-Konfiguration](https://github.com/RalfGuder/MCP-easyVerein/issues/12)

## User Story

**Als** Entwickler oder Administrator,
**möchte ich** API-URL, API-Version und API-Key als Kommandozeilenparameter (`--api-url`, `--api-version`, `--api-key`) beim Start des MCP-Servers übergeben können,
**damit** ich den Server flexibel in verschiedenen Umgebungen (Dev, Test, Prod) betreiben kann, ohne Umgebungsvariablen manuell setzen zu müssen.

## Akzeptanzkriterien

- [ ] Der Server akzeptiert die Parameter `--api-url`, `--api-version` und `--api-key` beim Start
- [ ] Übergebene Kommandozeilenparameter überschreiben gleichnamige Umgebungsvariablen
- [ ] Fehlt ein Parameter, wird der Wert aus der entsprechenden Umgebungsvariable (`EASYVEREIN_API_URL`, `EASYVEREIN_API_VERSION`, `EASYVEREIN_API_KEY`) verwendet
- [ ] Fehlt der Parameter und die Umgebungsvariable, wird eine Warnung ausgegeben und ein Standardwert genutzt
- [ ] Die Parameter sind in der Hilfemeldung (`--help`) dokumentiert

## Aufgaben

- Kommandozeilenparser integrieren (z.B. `System.CommandLine`)
- Parameter `--api-url`, `--api-version`, `--api-key` implementieren
- Priorisierungslogik CLI > Env-Var > Default implementieren
- Warnung bei fehlenden Pflichtangaben implementieren
- Tests nach TDD-Prinzip (zuerst Tests, dann Implementierung) schreiben
- Hilfemeldung (`--help`) ergänzen

## Technische Hinweise

- Prioritätsreihenfolge: CLI-Parameter → Umgebungsvariablen → Standardwerte
- Umgebungsvariablennamen: `EASYVEREIN_API_URL`, `EASYVEREIN_API_VERSION`, `EASYVEREIN_API_KEY`
- Empfohlenes NuGet-Paket: `System.CommandLine`
- TDD: Tests vor der Implementierung schreiben (Red-Green-Refactor)
