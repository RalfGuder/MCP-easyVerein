# US-0071: Lokaler HTTP-Transport

**Status:** Planned
**Erstellt:** 2026-11-28

## Abhängigkeiten
- US-0069: Die Konfigurationsdatei bekommt mit dieser Spec die Felder `transport` und `port`.
- US-0068: Das README beschreibt dann den Start. Diese Spec ergänzt ihn.
- Restpunkt aus US-0001 (#1), Issue #160. Im MCP-Standard ist „SSE“ inzwischen durch Streamable HTTP abgelöst. Diese Spec meint den aktuellen HTTP-Transport.

## Ausgangslage
- Der Server spricht nur stdio. Jeder Client startet deshalb seinen eigenen Serverprozess. Mehrere Sitzungen auf demselben Rechner können sich keinen laufenden Server teilen.

## User Stories
- Als Betreiber möchte ich den Server einmal lokal starten und mehrere MCP-Clients auf demselben Rechner per HTTP verbinden, damit nicht jede Sitzung einen eigenen Prozess braucht.
- Als Betreiber möchte ich, dass der HTTP-Server nur von meinem Rechner aus erreichbar ist und Browser-Anfragen fremder Webseiten abweist, damit niemand über das Netz oder eine Webseite mit meinem Vereinsschlüssel arbeitet.

## Außerhalb des Umfangs
- Erreichbarkeit aus dem Netz (andere Rechner, VPS, Software Factory, claude.ai-Connector)
- TLS/HTTPS, Login oder Token für Clients
- HTTP-Transport im Docker-Container (Docker bleibt bei stdio)
- Frei wählbare Bind-Adresse
- Der veraltete eigenständige SSE-Transport aus älteren MCP-Spezifikationen
- Mehrere Vereinsschlüssel bzw. ein Schlüssel je Client

## Akzeptanzkriterien

### Auswahl und Standard
- [ ] AK-1: Angenommen der Server wird ohne Transportangabe gestartet, dann spricht er stdio wie bisher.
- [ ] AK-2: Angenommen der Server wird mit `--transport http` gestartet, dann nimmt er MCP-Anfragen per HTTP unter `http://127.0.0.1:8765/mcp` an. Über stdio spricht er dann kein MCP.
- [ ] AK-3: Angenommen der Transport wird über die Umgebungsvariable `EASYVEREIN_TRANSPORT` oder das Feld `transport` der Konfigurationsdatei (US-0069) gesetzt, dann gilt dieselbe Priorität wie bei den anderen Werten: CLI-Parameter vor Umgebungsvariable vor Datei vor Standard `stdio`.
- [ ] AK-4: Angenommen `--port 9000` (bzw. `EASYVEREIN_PORT` oder das Feld `port`) ist gesetzt und der Transport ist `http`, dann lauscht der Server auf Port 9000. Ohne Angabe gilt 8765.
- [ ] AK-5: Angenommen `--transport` hat einen anderen Wert als `stdio` oder `http`, dann startet der Server nicht. Die Meldung auf stderr nennt die gültigen Werte, der Exit-Code ist ungleich 0.
- [ ] AK-6: Angenommen der Port ist keine ganze Zahl von 1 bis 65535, dann startet der Server nicht. Die Meldung auf stderr nennt den gültigen Bereich, der Exit-Code ist ungleich 0.

### Erreichbarkeit und Schutz
- [ ] AK-7: Angenommen der HTTP-Transport läuft, dann ist er nur über die Loopback-Adressen (127.0.0.1 und ::1) erreichbar, nicht über andere Netzwerkschnittstellen des Rechners.
- [ ] AK-8: Angenommen eine Anfrage trägt einen `Origin`-Header, der nicht auf `localhost`, `127.0.0.1` oder `[::1]` zeigt (z. B. `https://example.com`), dann antwortet der Server mit HTTP 403 und führt kein Tool aus.
- [ ] AK-9: Angenommen eine Anfrage ohne `Origin`-Header kommt von einem lokalen MCP-Client, dann wird sie normal verarbeitet.
- [ ] AK-10: Angenommen der Port ist schon belegt, dann startet der Server nicht. Die Meldung auf stderr nennt den Port und schlägt `--port` vor, der Exit-Code ist ungleich 0.

### Funktion
- [ ] AK-11: Angenommen ein MCP-Client verbindet sich per HTTP, dann sind `tools/list`, `tools/call`, `prompts/list` und `prompts/get` mit denselben Tools, Prompts und Antworten verfügbar wie über stdio.
- [ ] AK-12: Angenommen zwei MCP-Clients sind gleichzeitig per HTTP verbunden, dann kann jeder Tools aufrufen, ohne die Antworten des anderen zu erhalten.

### Hilfe und Doku
- [ ] AK-13: Angenommen der Server wird mit `--help` gestartet, dann listet die Hilfe `--transport stdio|http` und `--port <nummer>` mit Umgebungsvariablen und Standardwerten.
- [ ] AK-14: Angenommen das README wird gelesen, dann beschreibt es den HTTP-Start, die URL `http://127.0.0.1:8765/mcp`, einen Beispiel-Eintrag für einen MCP-Client mit HTTP-URL, die Beschränkung auf den eigenen Rechner und dass Docker weiterhin stdio nutzt.

## Edge Cases
- EC-1: `--port` angegeben, Transport `stdio` → Port wird ignoriert, keine Warnung nötig.
- EC-2: `--transport HTTP` (Großschreibung) → gilt wie `http`.
- EC-3: Server wird beendet (Strg+C oder SIGTERM) → offene Verbindungen werden geschlossen, Prozess endet ohne Fehler-Exit-Code.
- EC-4: Anfrage an einen anderen Pfad als `/mcp` → HTTP 404, kein Tool wird ausgeführt.
- EC-5: `Origin: http://localhost:3000` (lokale Webanwendung) → erlaubt (AK-8 lässt localhost-Origins zu).

## Technische Anforderungen
- Bei stdio bleibt stdout ausschließlich für MCP JSON-RPC. Meldungen und Fehler gehen bei beiden Transporten nach stderr.
- Tests ohne echte easyVerein-Aufrufe; HTTP-Tests gegen einen im Test gestarteten Server auf freiem Port.

## Offene Fragen
- Keine.

## Product Decisions
- Nur lokal (Loopback): Der Server hält einen Vereinsschlüssel mit Schreibrechten; Netzbetrieb bräuchte Auth und TLS und ist eigene Story (2026-11-28).
- Origin-Prüfung statt Token: schützt gegen Anfragen fremder Webseiten im Browser (Empfehlung der MCP-Spezifikation), ohne Clients zusätzlich zu konfigurieren (2026-11-28).
- Auswahl über `--transport stdio|http` und `--port`, Standard stdio und 8765 (5000 ist unter macOS oft belegt) (2026-11-28).
- Docker bleibt stdio: Feste Loopback-Bindung wäre im Container von außen nicht erreichbar; eine öffnende Bind-Adresse widerspräche „nur lokal“ (2026-11-28).
- Streamable HTTP statt des veralteten SSE-Transports (2026-11-28).
