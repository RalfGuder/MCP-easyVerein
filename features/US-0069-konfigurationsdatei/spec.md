# US-0069: Konfigurationsdatei als zusätzliche Quelle

**Status:** Planned
**Erstellt:** 2026-11-28

## Abhängigkeiten
- US-0068: Das README hat dann einen Abschnitt „Konfiguration“. Diese Spec ergänzt ihn.
- Restpunkt aus US-0001 (#1), Issue #160.

## Ausgangslage
- Heute kommt die Konfiguration aus CLI-Parametern (`--api-key`, `--api-url`, `--api-version`), aus Umgebungsvariablen (`EASYVEREIN_API_KEY`, `EASYVEREIN_API_URL`, `EASYVEREIN_API_VERSION`) und aus Standardwerten.
- Der Schlüssel steht deshalb meist in der `.mcp.json` des Projekts. Diese Datei liegt im Repo-Ordner und kann versehentlich mit eingecheckt oder weitergegeben werden.
- Nebenbei liest der Host heute eine `appsettings.json` aus dem Arbeitsverzeichnis. Das ist nicht dokumentiert, und der Ort hängt davon ab, wo der Server gestartet wird.

## User Stories
- Als Betreiber möchte ich Schlüssel, URL und API-Version einmal in einer Datei in meinem Benutzerprofil hinterlegen, damit der Schlüssel nicht in der `.mcp.json` des Projekts steht.
- Als Betreiber mit mehreren Vereinen oder Testzugängen möchte ich per `--config <pfad>` eine bestimmte Datei wählen, damit ich zwischen Zugängen wechseln kann, ohne die `.mcp.json` umzubauen.

## Außerhalb des Umfangs
- Weitere Einstellungen in der Datei (Transport, Port, Logging) – kommen bei Bedarf mit der jeweiligen Story
- Verschlüsselung oder Ablage im Betriebssystem-Schlüsselbund
- Anlegen der Datei durch den Server (z. B. Assistent oder `--init`)
- Andere Formate als JSON
- Neuladen der Datei zur Laufzeit; Änderungen gelten nach Neustart

## Akzeptanzkriterien

### Fundort
- [ ] AK-1: Angenommen der Server wird mit `--config <pfad>` gestartet und die Datei ist gültig, dann übernimmt er die Werte aus dieser Datei.
- [ ] AK-2: Angenommen `--config` fehlt, aber die Umgebungsvariable `EASYVEREIN_CONFIG` enthält einen Pfad, dann verhält sich der Server wie bei AK-1 mit diesem Pfad. `--config` hat Vorrang vor `EASYVEREIN_CONFIG`.
- [ ] AK-3: Angenommen weder `--config` noch `EASYVEREIN_CONFIG` ist gesetzt, und im Benutzerprofil liegt die Standarddatei, dann übernimmt der Server deren Werte. Unter Windows ist das `%APPDATA%\mcp-easyverein\config.json`, unter Linux und macOS `~/.config/mcp-easyverein/config.json`. Ist `XDG_CONFIG_HOME` gesetzt, gilt `$XDG_CONFIG_HOME/mcp-easyverein/config.json`.
- [ ] AK-4: Angenommen es gibt weder Pfadangabe noch Standarddatei, dann startet der Server ohne Fehler und ohne Warnung zur Datei und verhält sich wie heute.
- [ ] AK-5: Angenommen im Arbeitsverzeichnis liegt eine `appsettings.json` mit `EASYVEREIN_API_KEY`, dann ignoriert der Server diese Datei.

### Inhalt und Priorität
- [ ] AK-6: Angenommen die Datei enthält `{"apiKey": "k", "apiUrl": "https://example.test/api", "apiVersion": "v1.7"}`, dann nutzt der Server diese drei Werte, sofern sie nicht anders gesetzt sind. Groß- und Kleinschreibung der Feldnamen spielt keine Rolle. Alle Felder sind optional.
- [ ] AK-7: Angenommen ein Wert steht in der Datei und zugleich in der Umgebungsvariable, dann gilt die Umgebungsvariable. Steht er zusätzlich als CLI-Parameter, gilt der CLI-Parameter. Die Priorität lautet: CLI-Parameter vor Umgebungsvariable vor Datei vor Standardwert. Das gilt für jeden Wert einzeln, etwa Schlüssel aus der Datei und Version per CLI.
- [ ] AK-8: Angenommen die Datei enthält unbekannte Felder (z. B. `"comment": "…"`), dann ignoriert der Server sie ohne Fehler.
- [ ] AK-9: Angenommen die Datei enthält `"apiVersion": "v9.9"`, dann bricht der Start genauso ab wie bei einer ungültigen Version per CLI-Parameter oder Umgebungsvariable, also mit Vorschlag der nächstmöglichen Version.
- [ ] AK-10: Angenommen der Schlüssel in der Datei beginnt mit `Bearer `, dann wird das Präfix entfernt wie in US-0068 (AK-12).

### Fehler
- [ ] AK-11: Angenommen `--config` oder `EASYVEREIN_CONFIG` zeigt auf eine Datei, die es nicht gibt, dann startet der Server nicht. Auf stderr steht eine Meldung mit dem Pfad und dem Hinweis, dass die Datei fehlt. Der Exit-Code ist ungleich 0.
- [ ] AK-12: Angenommen die Datei (angegeben oder Standarddatei) ist kein gültiges JSON oder enthält für ein Feld keinen Text, sondern z. B. eine Zahl oder ein Objekt, dann startet der Server nicht. Die Meldung auf stderr nennt Pfad und Grund. Der Exit-Code ist ungleich 0.
- [ ] AK-13: Angenommen eine Fehlermeldung zur Datei wird ausgegeben, dann enthält sie nie den Wert des Schlüssels.

### Hilfe und Doku
- [ ] AK-14: Angenommen der Server wird mit `--help` gestartet, dann listet die Hilfe `--config <pfad>` mit `EASYVEREIN_CONFIG`, den Standardort und die neue Priorität.
- [ ] AK-15: Angenommen das README wird gelesen, dann beschreibt der Abschnitt „Konfiguration“ die Datei: Fundorte in Reihenfolge, Beispielinhalt mit Platzhalter, Priorität und den Hinweis, dass der Schlüssel so nicht im Projekt-Ordner liegen muss.

## Edge Cases
- EC-1: Datei ist leer (0 Byte) → wie ungültiges JSON (AK-12).
- EC-2: Datei enthält `{}` → gültig, keine Werte; Server nutzt die übrigen Quellen.
- EC-3: Feld ist leerer Text (`"apiKey": ""`) → gilt als nicht gesetzt; nächste Quelle bzw. Standardwert.
- EC-4: `--config` mit relativem Pfad → relativ zum Arbeitsverzeichnis des Servers.
- EC-5: `--config` ohne Wert (letztes Argument) → Start abbricht mit Meldung wie AK-11.
- EC-6: Datei mit UTF-8-BOM → wird gelesen.
- EC-7: Datei ist nicht lesbar (Rechte) → Abbruch mit Meldung wie AK-11, Grund „nicht lesbar“.
- EC-8: Standarddatei im Profil ist fehlerhaft → Abbruch wie AK-12 (sie wurde gefunden, also soll sie wirken).

## Technische Anforderungen
- stdout bleibt ausschließlich für MCP JSON-RPC; Meldungen nur auf stderr.
- Tests ohne echte easyVerein-Aufrufe und ohne Zugriff auf das echte Benutzerprofil.

## Offene Fragen
- Keine.

## Product Decisions
- Fundorte `--config` > `EASYVEREIN_CONFIG` > Standarddatei im Benutzerprofil: expliziter Pfad für mehrere Zugänge, fester Ort für den Normalfall (2026-11-28).
- Priorität CLI > Umgebung > Datei > Default: Die Datei ist die Grundeinstellung, `.mcp.json`-Umgebungsvariablen und CLI überschreiben sie einzeln (2026-11-28).
- Implizite `appsettings.json` im Arbeitsverzeichnis wird nicht mehr gelesen: Ort hängt vom Startverzeichnis ab, Schlüsseldatei im Repo-Ordner könnte eingecheckt werden (2026-11-28).
- JSON mit sprechenden Feldnamen `apiKey`, `apiUrl`, `apiVersion` (2026-11-28).
- Fehlerhafte oder fehlende angegebene Datei bricht den Start ab: Ein Tippfehler im Pfad soll nicht still mit anderem Zugang laufen (2026-11-28).
