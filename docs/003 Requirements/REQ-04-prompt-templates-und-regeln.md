# REQ-04: Prompt Templates und Regeln

> **Thema:** MCP Prompt Templates mit eingebetteten Regeln
> **Herkunft:** [US-0002](https://github.com/RalfGuder/MCP-easyVerein/issues/2)
> **Stand:** 2026-03-29

## Übersicht

Dieses Dokument beschreibt die Anforderungen an die MCP Prompt Templates, die wiederkehrende Aufgaben in der Vereinsverwaltung standardisieren. Jeder Prompt enthält eingebettete Regeln für Formatvorgaben, Workflow-Schritte und Validierung und unterstützt Parameter für die individuelle Anpassung.

## Funktionale Requirements

| ID | Requirement | MoSCoW | Herkunft |
|----|-------------|--------|----------|
| FR-030 | Prompts über `prompts/list` und `prompts/get` bereitstellen | Must | US-0002 |
| FR-031 | Parameter-Support in Prompts | Must | US-0002 |
| FR-032 | Eingebettete Regeln für Formatvorgaben | Must | US-0002 |
| FR-033 | Eingebettete Regeln für Workflow-Schritte | Must | US-0002 |
| FR-034 | Eingebettete Regeln für Validierung | Must | US-0002 |
| FR-035 | Prompt Templates für Mitgliederverwaltung | Must | US-0002 |
| FR-036 | Prompt Templates für Rechnungen/Finanzen | Should | US-0002 |
| FR-037 | Prompt Templates für Veranstaltungen | Should | US-0002 |
| FR-038 | Prompt Templates für Sitzungen | Could | US-0002 |
| FR-039 | Prompt Templates für Protokolle | Could | US-0002 |
| FR-040 | Prompt Templates für E-Mails/Serienbriefe | Could | US-0002 |

### FR-030: MCP Prompt Templates bereitstellen

**Priorität:** Must | **Herkunft:** US-0002

Prompts werden als MCP Prompt Templates über die Standard-Endpunkte `prompts/list` und `prompts/get` bereitgestellt.

**Akzeptanzkriterien:**
- [ ] `prompts/list` gibt eine Liste aller verfügbaren Prompts zurück
- [ ] `prompts/get` gibt einen einzelnen Prompt mit allen Details zurück
- [ ] Prompts folgen dem MCP-Standard für Prompt Templates

### FR-031: Parameter-Support

**Priorität:** Must | **Herkunft:** US-0002

Prompts unterstützen Parameter (Platzhalter) für die individuelle Anpassung.

**Akzeptanzkriterien:**
- [ ] Parameter werden im Format `{{parametername}}` definiert
- [ ] Jeder Parameter hat einen Namen, eine Beschreibung und einen Typ
- [ ] Fehlende Pflichtparameter werden beim Abruf als Fehler gemeldet
- [ ] Optionale Parameter haben Standardwerte

### FR-032: Eingebettete Formatvorgaben

**Priorität:** Must | **Herkunft:** US-0002

Jeder Prompt enthält eingebettete Regeln für Formatvorgaben.

**Akzeptanzkriterien:**
- [ ] Ausgabeformat ist pro Prompt definiert (z.B. Tabelle, Liste, Fließtext)
- [ ] Datumsformate, Währungsformate etc. sind festgelegt
- [ ] Formatregeln werden im Prompt-Text eingebettet

### FR-033: Eingebettete Workflow-Schritte

**Priorität:** Must | **Herkunft:** US-0002

Jeder Prompt enthält eingebettete Regeln für Workflow-Schritte.

**Akzeptanzkriterien:**
- [ ] Reihenfolge der Arbeitsschritte ist definiert
- [ ] Abhängigkeiten zwischen Schritten sind berücksichtigt
- [ ] Workflow-Regeln sind im Prompt-Text eingebettet

### FR-034: Eingebettete Validierungsregeln

**Priorität:** Must | **Herkunft:** US-0002

Jeder Prompt enthält eingebettete Regeln für die Validierung von Ein- und Ausgaben.

**Akzeptanzkriterien:**
- [ ] Pflichtfelder sind definiert
- [ ] Wertebereichsprüfungen sind festgelegt
- [ ] Validierungsregeln sind im Prompt-Text eingebettet

### FR-035: Prompt Templates für Mitgliederverwaltung

**Priorität:** Must | **Herkunft:** US-0002

Prompt Templates für den Bereich Mitgliederverwaltung werden als erstes umgesetzt (höchste Priorität).

**Akzeptanzkriterien:**
- [ ] Prompt zum Abfragen/Suchen von Mitgliedern
- [ ] Prompt zum Anlegen neuer Mitglieder
- [ ] Prompt zum Bearbeiten von Mitgliederdaten
- [ ] Jeder Prompt enthält Format-, Workflow- und Validierungsregeln

### FR-036: Prompt Templates für Rechnungen/Finanzen

**Priorität:** Should | **Herkunft:** US-0002

Prompt Templates für den Bereich Rechnungen und Finanzen.

**Akzeptanzkriterien:**
- [ ] Prompt zum Abfragen von Rechnungen
- [ ] Prompt zum Erstellen von Rechnungen
- [ ] Format-, Workflow- und Validierungsregeln eingebettet

### FR-037: Prompt Templates für Veranstaltungen

**Priorität:** Should | **Herkunft:** US-0002

Prompt Templates für den Bereich Veranstaltungen.

**Akzeptanzkriterien:**
- [ ] Prompt zum Abfragen von Veranstaltungen
- [ ] Prompt zum Erstellen/Bearbeiten von Veranstaltungen
- [ ] Format-, Workflow- und Validierungsregeln eingebettet

### FR-038: Prompt Templates für Sitzungen

**Priorität:** Could | **Herkunft:** US-0002

Prompt Templates für den Bereich Sitzungen.

**Akzeptanzkriterien:**
- [ ] Prompt zum Planen und Verwalten von Sitzungen
- [ ] Format-, Workflow- und Validierungsregeln eingebettet

### FR-039: Prompt Templates für Protokolle

**Priorität:** Could | **Herkunft:** US-0002

Prompt Templates für den Bereich Protokolle.

**Akzeptanzkriterien:**
- [ ] Prompt zum Erstellen und Verwalten von Protokollen
- [ ] Format-, Workflow- und Validierungsregeln eingebettet

### FR-040: Prompt Templates für E-Mails/Serienbriefe

**Priorität:** Could | **Herkunft:** US-0002

Prompt Templates für den Bereich E-Mails und Serienbriefe.

**Akzeptanzkriterien:**
- [ ] Prompt zum Erstellen von E-Mails und Serienbriefen
- [ ] Platzhalter für Empfängerdaten (z.B. aus Mitgliederliste)
- [ ] Format-, Workflow- und Validierungsregeln eingebettet

---

## Nicht-funktionale Requirements

| ID | Requirement | MoSCoW | Herkunft |
|----|-------------|--------|----------|
| NFR-020 | Mehrsprachigkeit: mindestens DE und EN | Must | US-0002 |
| NFR-021 | Prompts fest im MCP-Server eingebaut | Must | US-0002 |
| NFR-022 | Jeder Prompt definiert Parameter mit Beschreibung und Typ | Should | US-0002 |

### NFR-020: Mehrsprachigkeit

**Priorität:** Must | **Herkunft:** US-0002

Prompts sind mehrsprachig verfügbar, mindestens auf Deutsch und Englisch.

**Akzeptanzkriterien:**
- [ ] Jeder Prompt ist in Deutsch verfügbar
- [ ] Jeder Prompt ist in Englisch verfügbar
- [ ] Sprachauswahl ist konfigurierbar (z.B. über Parameter oder Serverkonfiguration)
- [ ] Weitere Sprachen können mit minimalem Aufwand ergänzt werden

### NFR-021: Prompts fest im Server eingebaut

**Priorität:** Must | **Herkunft:** US-0002

Prompts werden fest im MCP-Server eingebaut und nicht über ein externes Konfigurationsformat geladen.

**Akzeptanzkriterien:**
- [ ] Prompts sind im Quellcode definiert
- [ ] Keine externe Konfigurationsdatei für Prompt-Inhalte
- [ ] Änderungen an Prompts erfordern ein neues Release

### NFR-022: Parameter-Dokumentation

**Priorität:** Should | **Herkunft:** US-0002

Jeder Prompt definiert seine erwarteten Parameter mit Beschreibung und Typ.

**Akzeptanzkriterien:**
- [ ] Jeder Parameter hat einen eindeutigen Namen
- [ ] Jeder Parameter hat eine verständliche Beschreibung
- [ ] Jeder Parameter hat einen definierten Typ (String, Number, Date, etc.)
- [ ] Pflicht- und optionale Parameter sind gekennzeichnet

---

## Abhängigkeiten

| Requirement | hängt ab von | Grund |
|-------------|-------------|-------|
| Alle FR | REQ-01 (FR-001, NFR-003) | Prompt Templates setzen funktionierenden MCP-Server voraus |
| FR-035 bis FR-040 | FR-030, FR-031 | Bereichs-Prompts bauen auf Template-Infrastruktur auf |
| FR-035 bis FR-040 | REQ-01 (FR-004 bis FR-007) | Prompts nutzen die CRUD-Tools der jeweiligen Ressourcen |

## Offene Fragen

- [ ] Sollen Prompts auch benutzerdefinierte Regeln unterstützen oder nur fest eingebaute?
- [ ] Wie wird die Sprachauswahl technisch umgesetzt (Parameter, Konfiguration, Auto-Detect)?
- [ ] Gibt es Vorlagen oder Beispiele aus der easyVerein-Dokumentation, die als Basis dienen können?
