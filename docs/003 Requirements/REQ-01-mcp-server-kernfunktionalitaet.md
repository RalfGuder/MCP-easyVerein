# REQ-01: MCP-Server Kernfunktionalität

> **Thema:** MCP-Server und easyVerein API-Anbindung
> **Herkunft:** [US-0001](https://github.com/RalfGuder/MCP-easyVerein/issues/1), [US-0005](https://github.com/RalfGuder/MCP-easyVerein/issues/5), [US-0007](https://github.com/RalfGuder/MCP-easyVerein/issues/12)
> **Stand:** 2026-03-30

## Übersicht

Dieses Dokument beschreibt die Kernanforderungen an den easyVerein MCP-Server: den lokalen Serverbetrieb, die Authentifizierung, die CRUD-Operationen auf easyVerein-Ressourcen, die Konfiguration sowie die Unterstützung verschiedener API-Versionen.

## Funktionale Requirements

| ID | Requirement | MoSCoW | Herkunft |
|----|-------------|--------|----------|
| FR-001 | MCP-Server startet lokal und ist über stdio erreichbar | Must | US-0001 |
| FR-002 | Authentifizierung mit easyVerein API-Token | Must | US-0001 |
| FR-003 | Lese-Zugriff auf Mitgliederdaten als MCP-Tool | Must | US-0001 |
| FR-004 | CRUD-Operationen für Mitglieder | Must | US-0001 |
| FR-005 | CRUD-Operationen für Rechnungen/Buchungen | Must | US-0001 |
| FR-006 | CRUD-Operationen für Veranstaltungen | Must | US-0001 |
| FR-007 | CRUD-Operationen für Kontaktdaten | Must | US-0001 |
| FR-008 | Konfiguration über Umgebungsvariablen (API-Token, Basis-URL) | Must | US-0001 |
| FR-009 | Konfiguration über Konfigurationsdatei als Alternative | Should | US-0001 |
| FR-010 | Unterstützung aller verfügbaren easyVerein API-Versionen (v1, v2, weitere) | Must | US-0005 |
| FR-011 | Fest im Code hinterlegte Liste unterstützter API-Versionen | Must | US-0005 |
| FR-012 | Pro Serverinstanz eine Standard-API-Version | Must | US-0005 |
| FR-013 | API-Version konfigurierbar über Umgebungsvariable (`EASYVEREIN_API_VERSION`) | Must | US-0005 |
| FR-014 | API-Version pro MCP-Tool-Aufruf als Parameter überschreibbar | Should | US-0005 |
| FR-015 | Fehlermeldung mit Vorschlag der nächstmöglichen Version bei ungültiger API-Version | Should | US-0005 |
| FR-016 | SSE als zusätzlicher Transportkanal | Could | US-0001 |
| FR-041 | Konfiguration über Kommandozeilenparameter (`--api-url`, `--api-version`, `--api-key`) | Must | US-0007 |
| FR-042 | CLI-Parameter überschreiben Umgebungsvariablen (Priorität: CLI > Env-Var > Default) | Must | US-0007 |
| FR-043 | Warnung bei fehlendem Konfigurationswert, Fallback auf Standardwert | Must | US-0007 |
| FR-044 | `--help`-Parameter dokumentiert alle verfügbaren Startparameter | Must | US-0007 |

### FR-001: MCP-Server lokal über stdio erreichbar

**Priorität:** Must | **Herkunft:** US-0001

Der MCP-Server muss lokal gestartet werden können und über den stdio-Transportkanal erreichbar sein. Dies ist die primäre Kommunikationsschnittstelle gemäß MCP-Standard.

**Akzeptanzkriterien:**
- [ ] Server startet ohne Fehler auf localhost
- [ ] Kommunikation über stdio funktioniert gemäß MCP-Protokoll
- [ ] Server beendet sich sauber bei Abbruch

### FR-002: Authentifizierung mit easyVerein API-Token

**Priorität:** Must | **Herkunft:** US-0001

Der Server muss sich mit einem easyVerein API-Token gegenüber der easyVerein REST-API authentifizieren.

**Akzeptanzkriterien:**
- [ ] API-Token wird aus Konfiguration gelesen
- [ ] Token wird bei jedem API-Aufruf korrekt im Header mitgesendet
- [ ] Ungültige Tokens werden erkannt und mit Fehlermeldung quittiert

### FR-003: Lese-Zugriff auf Mitgliederdaten

**Priorität:** Must | **Herkunft:** US-0001

Mindestens der Lese-Zugriff auf Mitgliederdaten muss als MCP-Tool bereitgestellt werden.

**Akzeptanzkriterien:**
- [ ] MCP-Tool zum Abrufen von Mitgliederlisten verfügbar
- [ ] MCP-Tool zum Abrufen einzelner Mitgliederdaten verfügbar
- [ ] Ergebnisse werden strukturiert zurückgegeben

### FR-004 bis FR-007: CRUD-Operationen

**Priorität:** Must | **Herkunft:** US-0001

CRUD-Operationen (Create, Read, Update, Delete) für die zentralen easyVerein-Ressourcen:

| ID | Ressource | Akzeptanzkriterien |
|----|-----------|-------------------|
| FR-004 | Mitglieder | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-005 | Rechnungen/Buchungen | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-006 | Veranstaltungen | Anlegen, Abfragen, Bearbeiten, Löschen |
| FR-007 | Kontaktdaten | Anlegen, Abfragen, Bearbeiten, Löschen |

**Akzeptanzkriterien (je Ressource):**
- [ ] Alle vier CRUD-Operationen als MCP-Tools verfügbar
- [ ] Eingabeparameter werden validiert
- [ ] Fehler der easyVerein API werden sauber weitergeleitet

### FR-008: Konfiguration über Umgebungsvariablen

**Priorität:** Must | **Herkunft:** US-0001

Der Server muss über Umgebungsvariablen konfigurierbar sein (mindestens API-Token und Basis-URL). Umgebungsvariablen haben Vorrang vor Standardwerten, werden aber durch CLI-Parameter überschrieben (siehe FR-042).

**Akzeptanzkriterien:**
- [ ] `EASYVEREIN_API_KEY` wird als Umgebungsvariable unterstützt
- [ ] `EASYVEREIN_API_URL` wird als Umgebungsvariable unterstützt
- [ ] `EASYVEREIN_API_VERSION` wird als Umgebungsvariable unterstützt
- [ ] Fehlende Pflicht-Variablen führen zu Warnung und Nutzung des Standardwerts (siehe FR-043)

### FR-009: Konfiguration über Konfigurationsdatei

**Priorität:** Should | **Herkunft:** US-0001

Als Alternative zu Umgebungsvariablen soll eine Konfigurationsdatei unterstützt werden.

**Akzeptanzkriterien:**
- [ ] Konfigurationsdatei (z.B. JSON oder YAML) wird unterstützt
- [ ] Umgebungsvariablen haben Vorrang vor Datei-Konfiguration
- [ ] Pfad zur Konfigurationsdatei ist konfigurierbar

### FR-010 bis FR-013: API-Versionierung

**Priorität:** Must | **Herkunft:** US-0005

Die easyVerein API-Versionierung muss vollständig unterstützt werden.

**Akzeptanzkriterien:**
- [ ] Liste unterstützter Versionen ist im Code hinterlegt (FR-011)
- [ ] Standard-Version wird pro Serverinstanz festgelegt (FR-012)
- [ ] Umgebungsvariable `EASYVEREIN_API_VERSION` steuert die Version (FR-013)
- [ ] API-Aufrufe werden an die jeweilige Version angepasst (FR-010)

### FR-014: API-Version pro Aufruf überschreibbar

**Priorität:** Should | **Herkunft:** US-0005

Die API-Version soll pro MCP-Tool-Aufruf als Parameter überschrieben werden können.

**Akzeptanzkriterien:**
- [ ] Optionaler Parameter `api_version` in jedem MCP-Tool
- [ ] Override hat Vorrang vor Serverinstanz-Konfiguration
- [ ] Ungültige Version wird mit Fehlermeldung abgewiesen

### FR-015: Fehlermeldung bei ungültiger API-Version

**Priorität:** Should | **Herkunft:** US-0005

Bei einer nicht unterstützten API-Version wird eine hilfreiche Fehlermeldung mit Vorschlag ausgegeben.

**Akzeptanzkriterien:**
- [ ] Fehlermeldung nennt die angeforderte (ungültige) Version
- [ ] Fehlermeldung schlägt die nächstmögliche unterstützte Version vor
- [ ] Liste aller unterstützten Versionen wird angezeigt

### FR-041: Konfiguration über Kommandozeilenparameter

**Priorität:** Must | **Herkunft:** US-0007

Der Server akzeptiert beim Start die Parameter `--api-url`, `--api-version` und `--api-key`.

**Akzeptanzkriterien:**
- [ ] `--api-url` setzt die Basis-URL der easyVerein API
- [ ] `--api-version` setzt die zu verwendende API-Version
- [ ] `--api-key` setzt den API-Schlüssel für die Authentifizierung
- [ ] Unbekannte Parameter führen zu einer Fehlermeldung mit Hinweis auf `--help`

### FR-042: Prioritätsreihenfolge CLI > Env-Var > Default

**Priorität:** Must | **Herkunft:** US-0007

Konfigurationswerte werden in der Reihenfolge CLI-Parameter → Umgebungsvariablen → Standardwerte aufgelöst.

**Akzeptanzkriterien:**
- [ ] CLI-Parameter überschreiben gleichnamige Umgebungsvariablen
- [ ] Umgebungsvariablen überschreiben Standardwerte
- [ ] Prioritätsreihenfolge ist in `--help` und Dokumentation beschrieben

### FR-043: Warnung bei fehlendem Konfigurationswert

**Priorität:** Must | **Herkunft:** US-0007

Fehlt ein Konfigurationswert auf allen Ebenen (CLI, Env-Var), wird eine Warnung ausgegeben und ein Standardwert verwendet.

**Akzeptanzkriterien:**
- [ ] Warnung enthält den Namen des fehlenden Parameters
- [ ] Warnung nennt den verwendeten Standardwert
- [ ] Server startet trotz Warnung weiter (kein Abbruch)
- [ ] Standardwerte sind im Code dokumentiert

### FR-044: `--help`-Parameter

**Priorität:** Must | **Herkunft:** US-0007

Der Server gibt bei `--help` eine strukturierte Übersicht aller unterstützten Startparameter aus.

**Akzeptanzkriterien:**
- [ ] Alle Parameter (`--api-url`, `--api-version`, `--api-key`) sind aufgeführt
- [ ] Beschreibung, Typ und Standardwert je Parameter sind angegeben
- [ ] Zugehörige Umgebungsvariable je Parameter ist angegeben
- [ ] Prioritätsreihenfolge (CLI > Env-Var > Default) ist beschrieben

### FR-016: SSE als Transportkanal

**Priorität:** Could | **Herkunft:** US-0001

SSE (Server-Sent Events) soll als zusätzlicher Transportkanal neben stdio unterstützt werden.

**Akzeptanzkriterien:**
- [ ] SSE-Transportkanal ist implementiert
- [ ] Transportkanal ist konfigurierbar (stdio oder SSE)

---

## Nicht-funktionale Requirements

| ID | Requirement | MoSCoW | Herkunft |
|----|-------------|--------|----------|
| NFR-001 | Fehlerbehandlung bei ungültigen API-Tokens | Must | US-0001 |
| NFR-002 | Fehlerbehandlung bei Netzwerkfehlern | Must | US-0001 |
| NFR-003 | Konformität mit dem MCP-Standard | Must | US-0001 |
| NFR-004 | Versionsabstraktion erweiterbar mit minimalem Aufwand | Should | US-0005 |
| NFR-005 | Dokumentation: Installations-/Konfigurationsanleitung | Must | US-0001 |
| NFR-006 | Dokumentation der Versionskonfiguration | Should | US-0005 |
| NFR-023 | Dokumentation aller CLI-Parameter und Konfigurationsquellen im README | Must | US-0007 |

### NFR-001: Fehlerbehandlung bei ungültigen API-Tokens

**Priorität:** Must | **Herkunft:** US-0001

Ungültige oder abgelaufene API-Tokens müssen erkannt und mit einer verständlichen Fehlermeldung quittiert werden.

**Akzeptanzkriterien:**
- [ ] HTTP 401/403 von der easyVerein API wird abgefangen
- [ ] Fehlermeldung gibt Hinweis auf Ursache (ungültiger Token)
- [ ] Server stürzt nicht ab, sondern gibt den Fehler strukturiert zurück

### NFR-002: Fehlerbehandlung bei Netzwerkfehlern

**Priorität:** Must | **Herkunft:** US-0001

Netzwerkfehler (Timeout, DNS-Fehler, etc.) müssen sauber behandelt werden.

**Akzeptanzkriterien:**
- [ ] Timeout-Fehler werden erkannt und gemeldet
- [ ] DNS-Auflösungsfehler werden erkannt und gemeldet
- [ ] Verbindungsabbrüche werden sauber behandelt

### NFR-003: Konformität mit dem MCP-Standard

**Priorität:** Must | **Herkunft:** US-0001

Der Server muss dem MCP-Standard (Model Context Protocol) entsprechen.

**Akzeptanzkriterien:**
- [ ] Implementierung folgt der MCP-Spezifikation unter modelcontextprotocol.io
- [ ] Tools werden korrekt über `tools/list` und `tools/call` bereitgestellt
- [ ] Protokoll-Handshake funktioniert mit MCP-kompatiblen Clients

### NFR-004: Erweiterbare Versionsabstraktion

**Priorität:** Should | **Herkunft:** US-0005

Die Versionsabstraktion soll so gestaltet sein, dass neue API-Versionen mit minimalem Aufwand ergänzt werden können.

**Akzeptanzkriterien:**
- [ ] Neue Version kann durch Hinzufügen einer Konfiguration/Klasse ergänzt werden
- [ ] Bestehende Versionen werden durch Erweiterung nicht beeinträchtigt

### NFR-005: Installations- und Konfigurationsdokumentation

**Priorität:** Must | **Herkunft:** US-0001

Eine vollständige Installations- und Konfigurationsanleitung muss im README bereitgestellt werden.

**Akzeptanzkriterien:**
- [ ] Installationsschritte sind dokumentiert
- [ ] Konfigurationsoptionen (Umgebungsvariablen, Datei) sind beschrieben
- [ ] Mindestens ein Beispiel für die Nutzung ist enthalten

### NFR-006: Dokumentation der Versionskonfiguration

**Priorität:** Should | **Herkunft:** US-0005

Die Konfiguration der API-Versionierung muss dokumentiert sein.

**Akzeptanzkriterien:**
- [ ] Unterstützte Versionen sind aufgelistet
- [ ] Konfiguration über Umgebungsvariable ist beschrieben
- [ ] Override-Mechanismus pro Aufruf ist dokumentiert

### NFR-023: Dokumentation aller Konfigurationsquellen

**Priorität:** Must | **Herkunft:** US-0007

Alle Konfigurationsquellen (CLI-Parameter, Umgebungsvariablen, Standardwerte) und ihre Prioritätsreihenfolge sind im README dokumentiert.

**Akzeptanzkriterien:**
- [ ] CLI-Parameter (`--api-url`, `--api-version`, `--api-key`) sind im README beschrieben
- [ ] Zugehörige Umgebungsvariablen je Parameter sind aufgeführt
- [ ] Prioritätsreihenfolge CLI > Env-Var > Default ist erklärt
- [ ] Mindestens ein Beispielaufruf mit CLI-Parametern ist enthalten

---

## Abhängigkeiten

| Requirement | hängt ab von | Grund |
|-------------|-------------|-------|
| FR-003 bis FR-007 | FR-002 | CRUD-Operationen benötigen Authentifizierung |
| FR-010 bis FR-015 | FR-001 | Versionierung setzt laufenden Server voraus |
| FR-014 | FR-012, FR-013 | Override setzt Standard-Version voraus |
| FR-042 | FR-008, FR-041 | Prioritätslogik setzt beide Konfigurationsquellen voraus |
| FR-043 | FR-041, FR-042 | Fallback-Logik setzt Prioritätsauflösung voraus |
| FR-044 | FR-041 | --help setzt definierte CLI-Parameter voraus |
| Alle FR | REQ-02 (NFR-008) | Clean Architecture definiert die Projektstruktur |

## Offene Fragen

- [ ] Welche easyVerein API-Versionen sind aktuell verfügbar und müssen initial unterstützt werden?
- [ ] Gibt es Rate-Limiting seitens der easyVerein API, das berücksichtigt werden muss?
- [ ] Sollen Pagination-Mechanismen für Listenabfragen unterstützt werden?
